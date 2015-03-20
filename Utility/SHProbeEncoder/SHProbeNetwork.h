//////////////////////////////////////////////////////////////////////////
// SH Probe Network
//
// Defines an array of probes linked together through their neighbors
//
//
#pragma once

#include "SHProbeEncoder.h"

class	SHProbeNetwork
{
public:		// CONSTANTS

	static const U32		MAX_PROBE_NEIGHBORS = 4;			// Only keep the 4 most significant neighbors
	static const U32		MAX_PROBE_UPDATES_PER_FRAME = 32;	// Update a maximum of 32 probes per frame


public:		// NESTED TYPES

	// The static probe structure that we read from disk and stream/keep in memory when probes need updating
	struct	SHProbe
	{
		U32				ProbeID;					// The ID is simply the probe's index in the array of probes
		Scene::Probe*	pSceneProbe;

		// Static SH infos
		float			pSHOcclusion[9];			// The pre-computed SH that gives back how much of the environment is perceived in a given direction
		float3			pSHStaticLighting[9];		// The pre-computed SH that gives back how much the probe perceives of indirectly bounced static lighting on static geometry

		// Geometric infos
		float			MeanDistance;				// Mean distance of all scene pixels
		float			MeanHarmonicDistance;		// Mean harmonic distance (1/sum(1/distance)) of all scene pixels
		float			MinDistance;				// Distance to closest scene pixel
		float			MaxDistance;				// Distance to farthest scene pixel
		float3			BBoxMin;					// Dimensions of the bounding box (axis-aligned) of the scene pixels
		float3			BBoxMax;

		// Generic reflective surfaces infos
		struct Sample {
			float3			Position;				// The position of the dynamic surface
			float3			Normal;					// The normal of the dynamic surface's plane
			float3			Tangent;				// The longest principal axis of the samples's points cluster (scaled by the length of the axis)
			float3			BiTangent;				// The shortest principal axis of the samples's points cluster (scaled by the length of the axis)
			float			Radius;					// An average radius for the sample so we can better filter shadows
			float3			Albedo;					// The albedo of the dynamic surface (not currently used, for info purpose)
			float3			F0;						// Surface's Fresnel coefficient
			float			SHFactor;				// The ratio of pixels occupied by the sample area compared to the total amount of original pixels
//			float			pSHBounce[9];			// The pre-computed SH that gives back how much the probe perceives of indirectly bounced dynamic lighting on static geometry
		}				pSamples[SHProbeEncoder::PROBE_SAMPLES_COUNT];

		// Emissive surfaces infos
		U32				EmissiveSurfacesCount;		// The amount of emissive surfaces for that probe
		struct EmissiveSurface {
			float3				Position;			// The position of the emissive surface
			float3				Normal;				// The normal of the emissive surface's plane
			float3				Tangent;			// The longest principal axis of the surface's points cluster (scaled by the length of the axis)
			float3				BiTangent;			// The shortest principal axis of the surface's points cluster (scaled by the length of the axis)
			Scene::Material*	pEmissiveMaterial;	// Direct pointer to the material
			float				pSHEmissive[9];		// The pre-computed SH that gives back how much the probe emits light
		}				pEmissiveSurfaces[SHProbeEncoder::MAX_PROBE_EMISSIVE_SURFACES];

		// Neighbor probes infos
		float			NearestProbeDistance;
		float			FarthestProbeDistance;
		struct NeighborProbeInfos {
			U32				ProbeID;				// ID of the neighbor probe
			float			Distance;				// Average distance to the probe
			float			SolidAngle;				// Perceived solid angle covered by the probe
			float3			Direction;				// Average direction to the probe
			float			SH[9];					// Convolution SH to use to isolate the contribution of the probe's SH this probe should perceive
		}				pNeighborProbeInfos[MAX_PROBE_NEIGHBORS];


//		// ===== Software Computation Section =====
//		float3			pSHBouncedLight[9];			// The resulting bounced irradiance * light(static+dynamic) + emissive for current frame (only valid if CPU computed)
// 
// 		// Clears the light bounce accumulator
// 		void			ClearLightBounce( const float3 _pSHAmbient[9] );
// 
// 		// Computes the product of SHLight and SHBounce to get the SH coefficients for the bounced light
// 		void			AccumulateLightBounce( const float3 _pSHSet[9] );
	};

	struct DynamicUpdateParms {

		U32		MaxProbeUpdatesPerFrame;// Maximum amount of probes we can update each frame

		float3	AmbientSkySH[9];		// The SH coefficients used for the ambient sky term
		float3	BounceFactorSun;		// Bounce factor for the Sun
		float3	BounceFactorSky;		// Bounce factor for the sky
		float3	BounceFactorDynamic;	// Bounce factor for dynamic lights
		float3	BounceFactorStatic;		// Bounce factor for static lights
		float3	BounceFactorEmissive;	// Bounce factor for emissive materials
		float3	BounceFactorNeighbors;	// Bounce factor for neighbor probes
	};

	class IRenderSceneDelegate {
	public: virtual void	operator()( Material& _Material ) = 0;
	};
	class IQueryMaterial {
	public: virtual Scene::Material*	operator()( U32 _MaterialID ) = 0;
	};


private:	// RUNTIME STRUCTURES

#pragma pack( push, 4 )

	struct CBProbe {		// Used by NeighborProbes splatting
		float3		CurrentProbePosition;
		U32			NeighborProbeID;
		float3		NeighborProbePosition;
 	};

	struct CBUpdateProbes {	// Used by probes dynamic update
		U32			ProbesCount;
		float3		StaticLightingBoost;

		float3		SkyBoost;
		float		__PAD0;

		float3		SunBoost;
		float		__PAD1;

		float3		DynamicLightsBoost;
		float		__PAD2;

//		float4		AmbientSH[9];		// Ambient sky (padded!)
//		float		SunBoost;	// Merged into the last float4
// 		float		EmissiveBoost;
// 		float		NeighborProbesContributionBoost;
 	};


	// Runtime probes buffer that we'll use to light objects
	struct RuntimeProbe  {
		float3		Position;
		float		Radius;
//		float3		pSH[9];

		// Neighbor probes
//		U16			NeighborProbeIDs[4];			// IDs of the 4 most significant neighbors
	};

	struct SHCoeffs1 {
		float		pSH[9];
	};

	struct SHCoeffs3 {
		float3		pSH[9];
	};

	// Probes update buffers
	struct RuntimeProbeUpdateInfo
	{
		U32			Index;							// The index of the probe we're updating
// 		U32			SamplesStart;					// Index of the first sample for the probe
// 		U32			SamplesCount;					// Amount of samples for the probe
		U32			EmissiveSurfacesStart;			// Index of the first emissive surface for the probe
		U32			EmissiveSurfacesCount;			// Amount of emissive surfaces for the probe

// 		float3		SHStatic[9];					// Precomputed static SH (static geometry + static lights)
// 		float		SHOcclusion[9];					// Directional ambient occlusion for the probe

		// Neighbor probes
//		U32			NeighborProbeIDs[4];			// The IDs of the 4 most significant neighbor probes
		float4		NeighborProbeSH[9];				// The SH coefficients to convolve the neighbor's SH with to obtain their contribution to this probe
	};

	struct	RuntimeProbeUpdateSampleInfo
	{
		float3		Position;						// World position of the sampling point
		float3		Normal;							// World normal of the sampling point
		float3		Albedo;							// Albedo of the sample
		float		Radius;							// Radius of the sampling point's disc approximation (set to 0 to discard sample)
	};

	struct	RuntimeProbeUpdateEmissiveSurfaceInfo
	{
		float3		EmissiveColor;					// Color of the emissive material
		float		SH[9];							// SH for the surface
	};

	struct RuntimeProbeNetworkInfos
	{
		U32			ProbeIDs[2];					// The IDs of the 2 connected probes
		float2		NeighborsSolidAngles;			// Their perception of each other's solid angle
	};

#pragma pack( pop )


private:	// BUILD TIME STRUCTURES

	struct ProbeInfluence {
		U32		ProbeID;
		double	Influence;
	};

	class MeshWithAdjacency {
	public:
		class	Primitive {
		public:
			struct	Face {
				U32				V[3];				// Vertex indices
				float3			P;					// Center position
				float3			N;					// Face normal
				Face*			pAdjacent[3];		// Adjacent faces for each edge (an edge start from a vertex and ends at vertex + 1)
				ProbeInfluence*	pProbeInfluence;	// The probe influence for this face
				U32				LastVisitIndex;		// Index of the last visit pass

				void	SetAdjacency( int _V0, int _V1, Face* _AdjacentFace ) {
					for ( int EdgeIndex=0; EdgeIndex < 3; EdgeIndex++ ) {
						if ( V[EdgeIndex] == _V0 && V[(EdgeIndex+1)%3] == _V1 ) {
							pAdjacent[EdgeIndex] = _AdjacentFace;
							return;
						}
					}
					ASSERT( false, "Failed to retrieve adjacent edge!" );
				}
				bool	RecursePropagateProbeInfluences( U32 _PassIndex );
			};

		private:

			struct EdgeKey {
				int	V0, V1;
				static U32		GetHash( const EdgeKey& _key )							{
					return _key.V0 ^ _key.V1;
				}
				static int		Compare( const EdgeKey& _key0, const EdgeKey& _key1 )	{
					int	k00 = _key0.V0 < _key0.V1 ? _key0.V0 : _key0.V1;
					int	k01 = _key0.V0 < _key0.V1 ? _key0.V1 : _key0.V0;
					int	k10 = _key1.V0 < _key1.V1 ? _key1.V0 : _key1.V1;
					int	k11 = _key1.V0 < _key1.V1 ? _key1.V1 : _key1.V0;
					if ( k00 == k10 && k01 == k11 ) return 0;
					return 1;	// We don't care about ordering at the moment...
				}
			};
			struct FacePair {
				Face*	F0;
				Face*	F1;
			};

		public:
			int					m_FacesCount;
			Face*				m_pFaces;
			int					m_VerticesCount;
			ProbeInfluence**	m_pVerticesProbeInfluence;

			~Primitive() { SAFE_DELETE_ARRAY( m_pFaces ); SAFE_DELETE_ARRAY( m_pVerticesProbeInfluence ); }
			void	Build( const Scene::Mesh::Primitive& _Primitive, ProbeInfluence* _pProbeInfluencePerFace );
			bool	PropagateProbeInfluences( U32 _PassIndex );
			void	AssignNearestProbe( U32 _ProbesCount, const float3* _LocalProbePositions );
			void	RedistributeProbeIDs2Vertices( ProbeInfluence** _ppProbeInfluences ) const;
		};

		float4x4		m_World2Local;

		int				m_PrimitivesCount;
		Primitive*		m_pPrimitives;

		~MeshWithAdjacency() { SAFE_DELETE_ARRAY( m_pPrimitives ); }

		void	Build( const Scene::Mesh& _Mesh, ProbeInfluence* _pProbeInfluencePerFace );
		bool	PropagateProbeInfluences( U32 _PassIndex );
		void	AssignNearestProbe( U32 _ProbesCount, const SHProbe* _pProbes );
		void	RedistributeProbeIDs2Vertices( ProbeInfluence**& _ppProbeInfluences ) const;
	};


private:	// FIELDS
	
	Device*					m_pDevice;
	U32						m_ErrorCode;

	// The list of probes in the scene
	U32						m_ProbesCount;
	U32						m_MaxProbesCount;
	SHProbe*				m_pProbes;

	Primitive*				m_pScreenQuad;

	Texture2D*				m_pRTCubeMap;

	Material*				m_pMatRenderCubeMap;		// Renders the scene into a cubemap
	Material*				m_pMatRenderNeighborProbe;	// Renders the neighbor probes as planes to form a 3D vorono� cell
	ComputeShader*			m_pCSUpdateProbeDynamicSH;	// Dynamically update probes (spread across several frames)
	ComputeShader*			m_pCSAccumulateProbeSH;		// Dynamically update probes' SH by accumulating static + sky + dynamic SH (done each frame)

	Octree<const SHProbe*>	m_ProbeOctree;				// Scene octree containing probes, queried by dynamic objects

	// Constant buffers
 	CB<CBProbe>*			m_pCB_Probe;
 	CB<CBUpdateProbes>*		m_pCB_UpdateProbes;

	// Runtime probes
	SB<RuntimeProbe>*		m_pSB_RuntimeProbes;		// (SRV) Position + Radius + info for each probe

	SB<SHCoeffs3>*			m_ppSB_RuntimeSHStatic[2];	// (SRV) 2 sets of static SH (2 sets of lights, A and B, render in these)
	SB<SHCoeffs1>*			m_pSB_RuntimeSHAmbient;		// (SRV) 1 set of ambient sky SH 
	SB<SHCoeffs3>*			m_pSB_RuntimeSHDynamic;		// (UAV) 1 sets of dynamic SH (updated in real time across several frames)
	SB<SHCoeffs3>*			m_pSB_RuntimeSHDynamicSun;	// (UAV) 1 sets of dynamic SH for the Sun (updated in real time across several frames)
	SB<SHCoeffs3>*			m_pSB_RuntimeSHFinal;		// (UAV) The sum of all the above, updated each frame...

	// Probes Update
	SB<RuntimeProbeUpdateInfo>*					m_pSB_RuntimeProbeUpdateInfos;		// (SRV) Update info for each probe we're updating (e.g. index, emissive surface index/count, neighbor influences, etc.)
	SB<RuntimeProbeUpdateSampleInfo>*			m_pSB_RuntimeProbeSamples;			// (SRV) Info for each probe sample we're updating (position, normal, albedo, radius)
	SB<RuntimeProbeUpdateEmissiveSurfaceInfo>*	m_pSB_RuntimeProbeEmissiveSurfaces;	// (SRV) Info for each emissive surface we're updating (color, SH)
	SB<SHCoeffs1>*								m_pSB_RuntimeProbeSamplesSH;		// (SRV) SH for each sample direction

	// Additional vertex stream containing probe IDs for each vertex
	Primitive*				m_pPrimProbeIDs;

	// Probes network debug
	SB<RuntimeProbeNetworkInfos>*	m_pSB_RuntimeProbeNetworkInfos;

	// Queue of probe indices to update each frame
	// TODO! I'm only storing the index of the sequence of probes I'll update each frame
	int						m_ProbeUpdateIndex;

	// The encoder that will render cube maps and process them to generate runtime probe data
	SHProbeEncoder			m_ProbeEncoder;

	// List of probe influences for each face of the scene
	List< ProbeInfluence >	m_ProbeInfluencePerFace;


public:

	SHProbeNetwork();
	~SHProbeNetwork();

	void			Init( Device& _Device, Primitive& _ScreenQuad );
	void			Exit();

	void			PreAllocateProbes( int _ProbesCount );

	void			AddProbe( Scene::Probe& _Probe );
	U32				GetProbesCount() const	{ return m_ProbesCount; }

	Primitive*		GetProbeIDVertexStream() const	{ return m_pPrimProbeIDs; }

	// Runtime use
	void			UpdateDynamicProbes( DynamicUpdateParms& _Parms );
	U32				GetNearestProbe( const float3& _wsPosition ) const;

	// Build/Load/Save
	void			PreComputeProbes( const char* _pPathToProbes, IRenderSceneDelegate& _RenderScene, Scene& _Scene, U32 _TotalFacesCount );
	void			LoadProbes( const char* _pPathToProbes, IQueryMaterial& _QueryMaterial, const float3& _SceneBBoxMin, const float3& _SceneBBoxMax );

private:

	void			BuildProbeInfluenceVertexStream( Scene& _Scene, const char* _pPathToStreamFile );

friend static void	CopyProbeNetworkConnection( int _EntryIndex, SHProbeNetwork::RuntimeProbeNetworkInfos& _Value, void* _pUserData );

};
