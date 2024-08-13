using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UELib.Core;
using System.Runtime.InteropServices;

namespace UELib.Engine.Classes
{
    public enum EShaderPlatform : byte
    {
        SP_PCD3D_SM3 = 0,
        SP_PS3 = 1,
        SP_XBOXD3D = 2,
        SP_PCD3D_SM4 = 3,
        SP_PCD3D_SM5 = 4,
        SP_NGP = 5,
        SP_PCOGL = 6,
        SP_WIIU = 7,

        SP_NumPlatforms = 8,
        SP_NumBits = 4,
    }

    [UnrealRegisterClass]
    public class UShaderCache : UObject
    {
        public UShaderCache()
        {
            ShouldDeserializeOnDemand = true;
            Shaders = new UArray<UShader>();
            InvalidShaders = new UArray<UArray<byte>>();
            ShaderCode = new List<List<byte>>();
            MaterialShaderMaps = new UArray<UMaterialShaderMap>();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct UIndividualCompressedShaderInfo : IUnrealSerializableClass
        {
            public ushort ChunkIndex;
            public ushort UncompressedCodeLength;
            public int UncompressedCodeOffset;

            public void Deserialize(IUnrealStream stream)
            {
                stream.Read(out ChunkIndex);
                stream.Read(out UncompressedCodeOffset);
                stream.Read(out UncompressedCodeLength);
            }

            public void Serialize(IUnrealStream stream)
            {
                stream.Write(ChunkIndex);
                stream.Write(UncompressedCodeOffset);
                stream.Write(UncompressedCodeLength);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct UCompressedShaderCodeChunk : IUnrealSerializableClass
        {
            public int UncompressedSize;
            public UArray<byte> CompressedCode;

            public void Deserialize(IUnrealStream stream)
            {
                stream.Read(out UncompressedSize);
                stream.ReadArray(out CompressedCode);
            }

            public void Serialize(IUnrealStream stream)
            {
                throw new NotImplementedException();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct UTypeSpecificCompressedShaderCode : IUnrealSerializableClass
        {
            public UMap<UGuid, UIndividualCompressedShaderInfo> CompressedShaderInfos;
            public UArray<UCompressedShaderCodeChunk> CodeChunks;

            public void Deserialize(IUnrealStream stream)
            {
                var length = stream.ReadInt32();
                CompressedShaderInfos = new UMap<UGuid, UIndividualCompressedShaderInfo>(length);
                for (var i = 0; i < length; ++i)
                {
                    var key = new UGuid();
                    key.Deserialize(stream);
                    var value = new UIndividualCompressedShaderInfo();
                    value.Deserialize(stream);
                    CompressedShaderInfos.Add(key, value);
                }

                stream.ReadArray(out CodeChunks);
            }

            public void Serialize(IUnrealStream stream)
            {
                throw new NotImplementedException();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct UCompressedShaderCodeCache : IUnrealSerializableClass
        {
            public uint NumRefs;
            public EShaderPlatform Platform;
            public UMap<UShaderType, UTypeSpecificCompressedShaderCode> ShaderTypeCompressedShaderCode;

            public void Deserialize(IUnrealStream stream)
            {
                var length = stream.ReadInt32();
                ShaderTypeCompressedShaderCode = new UMap<UShaderType, UTypeSpecificCompressedShaderCode>(length);
                for (var i = 0; i < length; ++i)
                {
                    var key = new UShaderType();
                    key.Deserialize(stream);
                    var value = new UTypeSpecificCompressedShaderCode();
                    value.Deserialize(stream);
                    ShaderTypeCompressedShaderCode.Add(key, value);
                }
            }

            public void Serialize(IUnrealStream stream)
            {
                throw new NotImplementedException();
            }
        }

        public UCompressedShaderCodeCache CompressedCache { get; private set; }
        public byte Platform { get; private set; }
        public UArray<UShader> Shaders;
        public UArray<UArray<byte>> InvalidShaders { get; private set; }
        public List<List<byte>> ShaderCode { get; private set; }
        public UArray<UMaterialShaderMap> MaterialShaderMaps;
        public int ShaderCachePriority;

        private void DeserializeShaders()
        {
            int numShaders;
            Buffer.Read(out numShaders);

            for (var shaderIndex = 0; shaderIndex < numShaders; shaderIndex++)
            {
                UShaderType shaderType = new UShaderType();
                UGuid shaderId = new UGuid();
                shaderType.Deserialize(Buffer);
                shaderId.Deserialize(Buffer);

                if (Buffer.Version > 796)
                {
                    USHAHash savedHash = new USHAHash();
                    savedHash.Deserialize(Buffer);
                }

                Buffer.Read(out int skipOffset);
                var current = Buffer.AbsolutePosition;
                var shaderSize = skipOffset - current;

                var serializations = new UArray<ushort>();
                Buffer.ReadArray(out serializations);

                var shader = new UShader();
                shader.Deserialize(Buffer);
                Shaders.Add(shader);

                Buffer.AbsolutePosition = current;

                var shaderData = new UArray<byte>();
                for (var i = 0; i < shaderSize; i++)
                {
                    Buffer.Read(out byte b);
                    shaderData.Add(b);
                }

                InvalidShaders.Add(shaderData);
            }
        }

        protected override void Deserialize()
        {
            base.Deserialize();

            if (Buffer.Version >= 805)
            {
                Buffer.Read(out ShaderCachePriority);
            }

            if (Buffer.Version >= 538)
            {
                Buffer.Read(out byte temp);
                Platform = temp;
                if (Buffer.Version < 711 && Platform == 4)
                {
                    Platform = (byte)EShaderPlatform.SP_PCD3D_SM4;
                }

                if (Buffer.Version < 796)
                {
                    var length = Buffer.ReadInt32();
                    var dummy = new UMap<UShaderType, uint>(length);
                    for (var i = 0; i < length; ++i)
                    {
                        var key = new UShaderType();
                        key.Deserialize(Buffer);
                        uint value = 0;
                        Buffer.Read(out value);
                        dummy.Add(key, value);
                    }
                }
            }
            else
            {
                Buffer.Read(out byte temp);
                Platform = temp;
                var length = Buffer.ReadInt32();
                var dummy = new UMap<UShaderType, uint>(length);
                for (var i = 0; i < length; ++i)
                {
                    var key = new UShaderType();
                    key.Deserialize(Buffer);
                    uint value = 0;
                    Buffer.Read(out value);
                    dummy.Add(key, value);
                }

                var length2 = Buffer.ReadInt32();
                var dummy2 = new UMap<UVertexFactoryType, uint>(length2);
                for (var i = 0; i < length; ++i)
                {
                    var key = new UVertexFactoryType();
                    key.Deserialize(Buffer);
                    uint value = 0;
                    Buffer.Read(out value);
                    dummy2.Add(key, value);
                }
            }

            if (Buffer.Version >= 672)
            {
                CompressedCache = new UCompressedShaderCodeCache
                {
                    Platform = (EShaderPlatform)Platform
                };
                CompressedCache.Deserialize(Buffer);
            }

            DeserializeShaders();

            Buffer.Read(out int numMaterialShaderMaps);

            for (var i = 0; i < numMaterialShaderMaps; i++)
            {
                var staticParameters = new UStaticParameterSet();
                staticParameters.Deserialize(Buffer);

                var shaderMapVersion = 0;
                var shaderMapLicenseeVersion = 0;
                if (Buffer.Version >= 660)
                {
                    Buffer.Read(out shaderMapVersion);
                    Buffer.Read(out shaderMapLicenseeVersion);
                }

                Buffer.Read(out int skipOffset);

                var shaderMap = new UMaterialShaderMap();
                shaderMap.Deserialize(Buffer);
                MaterialShaderMaps.Add(shaderMap);
            }
        }

        public override string Decompile()
        {
            if (ShouldDeserializeOnDemand)
            {
                BeginDeserializing();
            }

            if (ImportTable != null)
            {
                return $"// Cannot decompile import {Name}";
            }

            string output = $"begin object name={Name} class={Class.Name}" +
                            "\r\n";

            UDecompilingState.AddTabs(1);
            output += UDecompilingState.Tabs + $"Platform={(EShaderPlatform)Platform}" + "\r\n";
            for (var i = 0; i < MaterialShaderMaps.Count; i++)
            {
                output += UDecompilingState.Tabs +
                          $"MaterialShaderMap_{i}=(FriendlyName={MaterialShaderMaps[i].FriendlyName})" + "\r\n";
            }

            UDecompilingState.RemoveTabs(1);

            return $"{output}{UDecompilingState.Tabs}object end" +
                   $"\r\n{UDecompilingState.Tabs}" +
                   $"// Reference: {Class.Name}'{GetOuterGroup()}'";
        }


        public string DecompileShaderMap(string name)
        {
            if (ShouldDeserializeOnDemand)
            {
                BeginDeserializing();
            }

            if (ImportTable != null)
            {
                return $"// Cannot decompile import {Name}";
            }

            var shaderMaps = MaterialShaderMaps.FindAll(map => map.FriendlyName == name);

            if (shaderMaps.Count == 0) return $"//Failed to find material {name}!";

            string output = "";

            var idx = 0;
            foreach (var shaderMap in shaderMaps)
            {
                output += $"begin shadermap name={shaderMap.FriendlyName}_{idx}" +
                          "\r\n";

                UDecompilingState.AddTabs(1);

                output += shaderMap.StaticParameters.Decompile();
                output += shaderMap.UniformExpressionSet.Decompile();

                for (var i = 0; i < shaderMap.MeshShaderMaps.Count; i++)
                {
                    output += UDecompilingState.Tabs +
                              $"MeshShaderMap_{i}=(VertexFactoryType={shaderMap.MeshShaderMaps[i].VertexFactoryType.TypeName},Shaders=" +
                              "\r\n";
                    UDecompilingState.AddTabs(1);

                    foreach (var shader in from shaderRef in shaderMap.MeshShaderMaps[i].Shaders.Values
                             from shader in Shaders
                             where shaderRef.ShaderId.GetHashCode() == shader.Id.GetHashCode()
                             select shader)
                    {
                        output += UDecompilingState.Tabs +
                                  $"Code={Convert.ToBase64String(shader.Key.Code.ToArray())}" + "\r\n";
                        output += UDecompilingState.Tabs +
                                  $"Type={shader.Type.FactoryName}" + "\r\n";
                        output += UDecompilingState.Tabs +
                                  $"Target=(Platform={shader.Target.Platform},Frequency={shader.Target.Frequency})" +
                                  "\r\n";
                        output += UDecompilingState.Tabs +
                                  $"Type={shader.NumInstructions}" + "\r\n";
                    }

                    UDecompilingState.RemoveTabs(1);
                    output += ")" + "\r\n";
                }

                UDecompilingState.RemoveTabs(1);

                output += $"{UDecompilingState.Tabs}shadermap end" +
                          $"\r\n{UDecompilingState.Tabs}" +
                          $"// Reference: '{shaderMap.FriendlyName}'" + "\r\n";
                idx++;
            }

            return output;
        }
    }
}