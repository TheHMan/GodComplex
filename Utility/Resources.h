//////////////////////////////////////////////////////////////////////////
// Resource Loading Helpers
//
#pragma once


// Don't forget to add any new include file pair down here \/
#define REGISTERED_INCLUDE_FILES	\
	{ "Inc/Global.fx", IDR_SHADER_INCLUDE_GLOBAL },	\



#include "..\GodComplex.h"
#include "d3dcommon.h"

class Material;
class IVertexFormatDescriptor;


// Loads a binary resource in memory
//	_pResourceType, type of binary resource to load
//	_pResourceSize, an optional pointer to an int that will contain the size of the loaded resource
const U8*	LoadResourceBinary( U16 _ResourceID, const char* _pResourceType, U32* _pResourceSize=NULL );

// Loads a text shader resource in memory
// IMPORTANT NOTE: You MUST destroy the returned pointed once you're done with it !
char*		LoadResourceShader( U16 _ResourceID, U32& _CodeSize );

// Create a full-fledged material given the shader resource ID and the vertex format
Material*	CreateMaterial( U16 _ShaderResourceID, const IVertexFormatDescriptor& _Format, const char* _pEntryPointVS, const char* _pEntryPointGS, const char* _pEntryPointPS, D3D_SHADER_MACRO* _pMacros=NULL );
