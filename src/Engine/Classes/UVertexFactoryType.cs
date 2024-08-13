using UELib.Core;

namespace UELib.Engine.Classes
{
    public enum EVertexFactorySupport : byte
    {
        bUsedWithMaterials = 1,
        bSupportsStaticLighting = 2,
        bSupportsDynamicLighting = 3,
        bSupportsPrecisePrevWorldPos = 4,
        bUsesLocalToWorld = 5,
    }
    
    public struct UVertexFactoryType : IUnrealSerializableClass
    {
        public uint HashIndex;
        public string Name;
        public string ShaderFilename;
        public UName TypeName;
        public EVertexFactorySupport Supports;
        public int MinPackageVersion;
        public int MinLicenseePackageVersion;
        
        public void Deserialize(IUnrealStream stream)
        {
            TypeName = new UName(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}