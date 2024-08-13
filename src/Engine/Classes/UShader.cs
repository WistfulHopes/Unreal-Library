using System.Runtime.InteropServices;
using UELib.Core;

namespace UELib.Engine.Classes
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct UShaderTarget : IUnrealSerializableClass
    {
        public byte Frequency;
        public byte Platform;
        public void Deserialize(IUnrealStream stream)
        {
            stream.Read(out Platform);
            stream.Read(out Frequency);
        }

        public void Serialize(IUnrealStream stream)
        {
            stream.Write(Platform);
            stream.Write(Frequency);
        }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct UShaderKey
    {
        public UArray<byte> Code;
        public uint ParameterMapCRC;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct UShaderRef : IUnrealSerializableClass
    {
        public UGuid ShaderId;
        public UShaderType ShaderType;
        
        public void Deserialize(IUnrealStream stream)
        {
            ShaderId.Deserialize(stream);
            ShaderType = new UShaderType();
            ShaderType.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct UShader : IUnrealSerializableClass
    {
        public UShaderKey Key;
        public UShaderTarget Target;
        public UShaderType Type;
        public UGuid Id;
        public USHAHash Hash;
        public uint NumInstructions;
        public UArray<byte> Buffer;

        public void Deserialize(IUnrealStream stream)
        {
            Target.Deserialize(stream);
            stream.ReadArray(out Key.Code);
            stream.Read(out Key.ParameterMapCRC);
            Id.Deserialize(stream);
            Type = new UShaderType();
            Type.Deserialize(stream);
            Hash.Deserialize(stream);
            stream.Read(out NumInstructions);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}