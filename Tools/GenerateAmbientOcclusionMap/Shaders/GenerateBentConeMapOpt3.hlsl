////////////////////////////////////////////////////////////////////////////////
// Bent Cone Map generator
// This compute shader will generate the bent normal/bent cone over a specific texel and store the result into a target UAV
//
// This is the "optimized" version of the other algorithm
// It's optimized in the sense it doesn't deal with the entire hemisphere of directions but rather a disk
//	subdivided into multiple slices and for each slice we compute the forward & backward horizon (like HBAO)
// Then we approximate the average "bent angle" which is not exactly the half angle between the 2 horizons
//	but a weighted average of the vectors instead.
//
// Our main problem comes from the fact that the vectors should be weighted by the solid angle expressed in the tangent space given by the normal
//	whereas we have no choice than to samples positions along a disc expressed in screen space (it's mandatory otherwise if we do our computations
//	in tangent space then we MUST ray-trace the height field from the tangent space's plane to find where we intersect the height field below,
//	and that would lose all interest as opposed to the brute force method that ray-casts lots of rays).
//
// 
//
//
//
//
//
//
//
// Technically, the brute force algorithm just does:
//		bentNormal = 1/N * Sum{i=1,N}[ (1-V(Wi)).(Wi.N).Wi ]
//
// Meaning we only average the unoccluded directions Wi, weighted by the cosine with the normal (so orthogonal directions have more weight than grazing ones)
// It works without bothering with the solid angle because all rays Wi have equal solid angle 2PI/N
//
// But we could simply find the maximum angle spanned to reach the 2 horizons (front and back)
//	and compute the actual integration of the directions between the 2 horizons, weighted by the cosine with the normal:
//
//		firstMomentBentNormal = Integral{theta0,theta1}[ sin(theta).cos(theta).sin(theta).dtheta ]
//
// Where:
//	� theta0 and theta1 are the front and back horizon angles in [-PI/2,+PI/2], always measured from the normal's direction
//	� The first sin(theta) represents the horizontal deviation of the Wi vector we're summing
//	� The first cos(theta) represents the "cosine weight" of the vector
//	� The second sin(theta) represents the solid angle of the spherical sector sin(theta).dphi reducing with elevation
//
//		firstMomentBentNormal = [1/3 sin^3(theta)]{theta0,theta1}
//
// The angle of the bent normal is then retrieved by computing the arcsine of that value.
//
//		                   ^ N                      
//  Negative Horizon \     |     . AverageN       
//		              \    |    .             __ Positive Horizon     
//		               \   |   .          __        
//		                \  |  .       __            
//		                 \ | .    __                  
//		                  \|. __                     
//		-------------------+----------------------
//
// Computing this for many slices of the hemisphere should yield the same (even better!) bent normal
//	as with the brute force method while requiring even less rays to shoot...
//
////////////////////////////////////////////////////////////////////////////////
//
static const uint	MAX_THREADS = 1024;
static const uint	ANGLE_BINS_COUNT = 16;

static const float	PI = 3.1415926535897932384626433832795;

static const uint	STEP_SIZE = 1;

SamplerState LinearClamp	: register( s0 );

cbuffer	CBInput : register( b0 ) {
	uint2	_textureDimensions;
	uint	_Y0;				// Start scanline for this group
	uint	_raysCount;			// Amount of rays to cast

	uint	_maxStepsCount;		// Maximum amount of steps to take before stopping
	bool	_tile;				// Tiling flag
	float	_texelSize_mm;		// Texel size in millimeters
	float	_displacement_mm;	// Height map max encoded displacement in millimeters
}

Texture2D<float>			_Tex_Height : register( t0 );
RWTexture2D<float4>			_Target : register( u0 );

StructuredBuffer<float3>	_Rays : register( t1 );

Texture2D<float3>			_Tex_Normal : register( t2 );

groupshared float3			gs_ssCenterPosition;		// Initial position, common to all rays
groupshared float3x3		gs_local2World;
groupshared uint3			gs_occlusionDirectionAccumulator = 0;
groupshared uint2			gs_horizonAngleAccumulator = 0;


// Build orthonormal basis from a 3D Unit Vector Without normalization [Frisvad2012])
void BuildOrthonormalBasis( float3 _normal, out float3 _tangent, out float3 _bitangent ) {
	float a = _normal.z > -0.9999999 ? 1.0 / (1.0 + _normal.z) : 0.0;
	float b = -_normal.x * _normal.y * a;

	_tangent = float3( 1.0 - _normal.x*_normal.x*a, b, -_normal.x );
	_bitangent = float3( b, 1.0 - _normal.y*_normal.y*a, -_normal.y );
}

[numthreads( MAX_THREADS, 1, 1 )]
void	CS( uint3 _GroupID : SV_GROUPID, uint3 _GroupThreadID : SV_GROUPTHREADID ) {
	uint2	pixelPosition = uint2( _GroupID.x, _Y0 + _GroupID.y );

	uint	rayIndex = _GroupThreadID.x;
	if ( rayIndex == 0 ) {
		// Build local tangent space to orient rays
		float3	normal = _Tex_Normal[pixelPosition];
		float3	tangent, biTangent;
		BuildOrthonormalBasis( normal, tangent, biTangent );

		gs_local2World[0] = tangent;
		gs_local2World[1] = biTangent;
		gs_local2World[2] = normal;

		gs_ssCenterPosition = float3( pixelPosition + 0.5, (_displacement_mm / _texelSize_mm) * _Tex_Height[pixelPosition] );
	}

	GroupMemoryBarrierWithGroupSync();

	////////////////////////////////////////////////////////////////////////
	// Average unoccluded directions to obtain main "bent normal" direction
	if ( rayIndex < _raysCount ) {
		float2	ssDirection;
		sincos( PI * rayIndex / _raysCount, ssDirection.y, ssDirection.x );

		float	heightFactor = _displacement_mm / _texelSize_mm;

		float3	N = gs_local2World[2];

		// March many pixels around central position in the slice's direction and find the horizons
		float3	ssPosition_Front = float3( pixelPosition + 0.5, 0.0 );
		float3	ssPosition_Back = float3( pixelPosition + 0.5, 0.0 );

		// Project screen-space direction onto tangent plane to determine max possible angles
		float	recZdotN = abs(N.z) > 1e-6 ? 1.0 / N.z : 1e6 * sign(N.z);
		float	hitDistance_Front = -dot( ssDirection, N.xy ) * recZdotN;
		float3	tsDirection_Front = normalize( float3( ssDirection, hitDistance_Front ) );
		float	maxCos_Front = tsDirection_Front.z;
//		float	hitDistance_Back = dot( ssDirection, N.xy ) * recZdotN;
//		float3	tsDirection_Back = normalize( float3( -ssDirection, hitDistance_Back ) );
//		float	maxCos_Back = tsDirection_Back.z;
		float	maxCos_Back = -tsDirection_Front.z;

		// Walk along positive and negative screen space directions to update the front & back horizons
		for ( uint radius=1; radius <= _maxStepsCount; radius++ ) {
			ssPosition_Front.xy += ssDirection;
			ssPosition_Back.xy -= ssDirection;
			if ( _tile ) {
				ssPosition_Front.xy = fmod( ssPosition_Front.xy + _textureDimensions, _textureDimensions );
				ssPosition_Back.xy = fmod( ssPosition_Back.xy + _textureDimensions, _textureDimensions );
			}

			// Sample heights at new position
			ssPosition_Front.z = _Tex_Height.SampleLevel( LinearClamp, ssPosition_Front.xy / _textureDimensions, 0 );
			ssPosition_Front.z *= heightFactor;	// Correct against aspect ratio
			ssPosition_Back.z = _Tex_Height.SampleLevel( LinearClamp, ssPosition_Back.xy / _textureDimensions, 0 );
			ssPosition_Back.z *= heightFactor;	// Correct against aspect ratio

			// Update horizon angles
			float3	ssDeltaPosition_Front = ssPosition_Front - gs_ssCenterPosition;
			float	cos_Front = ssDeltaPosition_Front.z / length( ssDeltaPosition_Front );
			maxCos_Front = max( maxCos_Front, cos_Front );

			float3	ssDeltaPosition_Back = ssPosition_Back - gs_ssCenterPosition;
			float	cos_Back = ssDeltaPosition_Back.z / length( ssDeltaPosition_Back );
			maxCos_Back = max( maxCos_Back, cos_Back );
		}

		// Compute the "average" bent normal weighted by the cos(alpha) where alpha is the angle with the actual normal
		#if 1
			// Half brute force where we perform the integration numerically as a sum...
			// This solution is prefered to the analytical integral that shows some precision artefacts unfortunately...
			//
			float	thetaFront = acos( maxCos_Front );
			float	thetaBack = -acos( maxCos_Back );

			float3	ssBentNormal = 0.001 * N;
			const uint	STEPS_COUNT = 256;
			for ( uint i=0; i < STEPS_COUNT; i++ ) {
				float	theta = lerp( thetaBack, thetaFront, (i+0.5) / STEPS_COUNT );
				float2	scTheta;
				sincos( theta, scTheta.x, scTheta.y );
				float3	ssUnOccludedDirection = float3( scTheta.x * ssDirection, scTheta.y );

				float	cosAlpha = saturate( dot( ssUnOccludedDirection, N ) );

				float	weight = cosAlpha * abs(scTheta.x);		// cos(alpha) * sin(theta).dTheta
				ssBentNormal += weight * ssUnOccludedDirection;
			}

			float	dTheta = (thetaFront - thetaBack) / STEPS_COUNT;
			ssBentNormal *= dTheta;

			uint	dontCare;
			InterlockedAdd( gs_occlusionDirectionAccumulator.x, uint(65536.0 * (1.0 + ssBentNormal.x)), dontCare );
			InterlockedAdd( gs_occlusionDirectionAccumulator.y, uint(65536.0 * (1.0 + ssBentNormal.y)), dontCare );
			InterlockedAdd( gs_occlusionDirectionAccumulator.z, uint(65536.0 * (1.0 + ssBentNormal.z)), dontCare );

		#elif 1
			// Integral computation
			// The result is not as accurate as the method above and some noise artefacts pop up
			//
			// The idea is to integrate the contribution of a direction vector W(theta) = {sin(theta), cos(theta), 0} expressed in the current slice
			//	and weighted with the dot( W, N ) and the solid angle sin(theta).dTheta.
			// W is split into its X and Y integration in the following way:
			//	~X = Integral[0,theta0]{ sin(theta).cos(psi).sin(theta).dtheta } - Integral[0,theta1]{ sin(theta).cos(psi).sin(theta).dtheta }
			//	~Y = Integral[0,theta0]{ cos(theta).cos(psi).sin(theta).dtheta } + Integral[0,theta1]{ cos(theta).cos(psi).sin(theta).dtheta }
			//
			// psi is the angle between W(theta) and N and can be found from the spherical law of cosines (https://en.wikipedia.org/wiki/Spherical_law_of_cosines):
			//	� We are given the length of 2 sides which are
			//		theta, the angle between W and the Z axis (0,0,1)
			//		alpha, the angle between the normal and the Z axis
			//	� We know the azimutal angle phi between the slice and the plane containing Z and the normal
			//
			// Thus cos(psi) = cos(theta).cos(alpha) + sin(theta).sin(alpha).cos(phi)
			//
			// I'll spare you the computation details as it's kind of verbose but in the end we basically have to integrate sin�(x).cos(x) or cos�(x).sin(x)
			//	or even sin^3(x) and re-order the terms properly...
			//
			float	cosAlpha = N.z;// saturate( dot( N, float3( 0, 0, 1 ) ) );
			float	sinAlpha = sqrt( 1.0 - cosAlpha*cosAlpha );
			float	cosPhi = dot( normalize( N.xy ), ssDirection );

			#if 1
				float	cosTheta0 = maxCos_Front;
				float	cosTheta0_2 = cosTheta0 * cosTheta0;
				float	cosTheta0_3 = cosTheta0_2 * cosTheta0;
				float	sinTheta0_2 = 1.0 - cosTheta0_2;
				float	sinTheta0 = sqrt( sinTheta0_2 );
				float	sinTheta0_3 = sinTheta0_2 * sinTheta0;

				float	cosTheta1 = maxCos_Back;
				float	cosTheta1_2 = cosTheta1 * cosTheta1;
				float	cosTheta1_3 = cosTheta1_2 * cosTheta1;
				float	sinTheta1_2 = 1.0 - cosTheta1_2;
				float	sinTheta1 = sqrt( sinTheta1_2 );
				float	sinTheta1_3 = sinTheta1_2 * sinTheta1;
			#else
				// Detailed computaiton
				float	theta0 = acos( maxCos_Front );
				float	theta1 = acos( maxCos_Back );

				float	cosTheta0 = cos( theta0 );
				float	sinTheta0 = sin( theta0 );
				float	cosTheta1 = cos( theta1 );
				float	sinTheta1 = sin( theta1 );
				float	cosTheta0_3 = cosTheta0*cosTheta0*cosTheta0;
				float	sinTheta0_3 = sinTheta0*sinTheta0*sinTheta0;
				float	cosTheta1_3 = cosTheta1*cosTheta1*cosTheta1;
				float	sinTheta1_3 = sinTheta1*sinTheta1*sinTheta1;
			#endif

			float	X = cosAlpha * (sinTheta0_3 - sinTheta1_3) + sinAlpha * cosPhi * (4.0 - 3.0*(cosTheta0 + cosTheta1_3) + cosTheta0_3 + cosTheta1_3) - sinAlpha * (2.0 - 3.0*cosTheta1 + cosTheta1_3);
			float	Y = cosAlpha * (2.0 - cosTheta1_3 - cosTheta0_3) + sinAlpha * cosPhi * (sinTheta0_3 - sinTheta1_3) + sinAlpha * sinTheta1_3;
			float3	ssBentNormal = float3( X * ssDirection, Y );
					ssBentNormal = normalize( ssBentNormal );

			uint	dontCare;
			InterlockedAdd( gs_occlusionDirectionAccumulator.x, uint(65536.0 * (1.0 + ssBentNormal.x)), dontCare );
			InterlockedAdd( gs_occlusionDirectionAccumulator.y, uint(65536.0 * (1.0 + ssBentNormal.y)), dontCare );
			InterlockedAdd( gs_occlusionDirectionAccumulator.z, uint(65536.0 * (1.0 + ssBentNormal.z)), dontCare );

		#else
			// Martin's integral computation (didn't manage to make it work :'()
			float	theta0 = -acos( maxCos_Front );
			float	theta1 = acos( maxCos_Back );

			float	I1 = (pow( cos(theta0), 3.0 ) + pow( cos(theta1), 3.0 )) / 3.0 - (cos(theta0) + cos(theta1)) + 4.0 / 3.0;
			float	I2 = 2.0 - (pow( cos(theta0), 3.0 ) + pow( cos(theta1), 3.0 ));
			float3	ssBentNormal = float3(	N.x * I1 * PI / 2.0,
											N.y * I1 * PI / 2.0,
											N.z * I2 * PI / 3.0
										);

			uint	dontCare;
			InterlockedAdd( gs_occlusionDirectionAccumulator.x, uint(65536.0 * (1.0 + ssBentNormal.x)), dontCare );
			InterlockedAdd( gs_occlusionDirectionAccumulator.y, uint(65536.0 * (1.0 + ssBentNormal.y)), dontCare );
			InterlockedAdd( gs_occlusionDirectionAccumulator.z, uint(65536.0 * (1.0 + ssBentNormal.z)), dontCare );
		#endif

		// Accumulate horizon angles & their variance
		// We're using running variance computation from https://www.johndcook.com/blog/standard_deviation/
		//	Avg(N) = Avg(N-1) + [V(N) - Avg(N-1)] / N
		//	S(N) = S(N-1) + [V(N) - Avg(N-1)] * [V(N) - Avg(N)]
		// And variance = S(finalN) / (finalN-1)
		//
		float	normalizedConeAngle = acos( dot( normalize( ssBentNormal ), N ) ) / (0.5 * PI);

		uint	previousSum;
		InterlockedAdd( gs_horizonAngleAccumulator.x, (uint(65536.0 * normalizedConeAngle) & 0x00FFFFFFU) | 0x01000000U, previousSum );
		uint	previousCount = previousSum >> 24;

		float	previousAverage = (previousSum & 0x00FFFFFFU) / 65536.0 * (previousCount > 1 ? 1.0 / previousCount : 0.0);
		float	newAverage = previousAverage + (normalizedConeAngle - previousAverage) / (previousCount+1.0);

		uint	previousVariance;
		float	variance = (normalizedConeAngle - previousAverage) * (normalizedConeAngle - newAverage);
		InterlockedAdd( gs_horizonAngleAccumulator.y, uint(256.0 * variance), previousVariance );
	}

	GroupMemoryBarrierWithGroupSync();

	////////////////////////////////////////////////////////////////////////
	// Finalize average bent normal direction (unnormalized)
	if ( rayIndex == 0 ) {
		float3	ssBentNormal = float3( gs_occlusionDirectionAccumulator.xyz ) - _raysCount * 65536.0;
				ssBentNormal = normalize( ssBentNormal );

		#if 1
			uint	finalCount = gs_horizonAngleAccumulator.x >> 24;
					finalCount = finalCount == 0 ? 256 : finalCount;	// When rays count == 256, the counter gets overflowed
			float	averageAngle = ((gs_horizonAngleAccumulator.x & 0x00FFFFFFU) / 65536.0) / max( 1, finalCount );
			float	varianceAngle = (gs_horizonAngleAccumulator.y / 256.0) / max( 1, finalCount-1 );
			float	stdDeviation = sqrt( varianceAngle );

//stdDeviation = cos( 0.5 * PI * stdDeviation );

			float	normalWeight = cos( 0.5 * PI * averageAngle );
//			ssBentNormal = normalWeight > 0.0 ? normalWeight * ssBentNormal : gs_local2World[2];
			ssBentNormal = max( 0.01, normalWeight ) * ssBentNormal;
		#else
			float	stdDeviation = 0.0;
		#endif

		ssBentNormal.y = -ssBentNormal.y;	// Normal textures are stored with inverted Y

		_Target[pixelPosition] = float4( 0.5 * (1.0+ssBentNormal), 1.0 - stdDeviation );	// Ready for texture
	}
}