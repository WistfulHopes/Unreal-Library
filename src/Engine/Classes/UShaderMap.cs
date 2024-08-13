using UELib.Core;

namespace UELib.Engine.Classes
{
    public class UShaderMap : IUnrealSerializableClass
    {
        public UMap<UShaderType, UShaderRef> Shaders;
        
        public void Deserialize(IUnrealStream stream)
        {                
            var length = stream.ReadInt32();
            Shaders = new UMap<UShaderType, UShaderRef>(length);
            for (var i = 0; i < length; ++i)
            {
                var key = new UShaderType();
                key.Deserialize(stream);
                var value = new UShaderRef();
                value.Deserialize(stream);
                Shaders.Add(key, value);
            }
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    public class UMeshMaterialShaderMap : UShaderMap
    {
        public UVertexFactoryType VertexFactoryType;
        
        public void Deserialize(IUnrealStream stream)
        {
            base.Deserialize(stream);
            VertexFactoryType.Deserialize(stream);
        }
    }

    public class UMaterialShaderMap : UShaderMap
    {
        public UArray<UMeshMaterialShaderMap> MeshShaderMaps;
        public UGuid MaterialId;
        public string FriendlyName;
        public UStaticParameterSet StaticParameters;
        public UUniformExpressionSet UniformExpressionSet;
        public EShaderPlatform Platform;
        
        public void Deserialize(IUnrealStream stream)
        {
            base.Deserialize(stream);
            var c = stream.ReadLength();
            MeshShaderMaps = new UArray<UMeshMaterialShaderMap>(c);
            for (var i = 0; i < c; ++i)
            {
                var element = new UMeshMaterialShaderMap();
                element.Deserialize(stream);
                MeshShaderMaps.Add(element);
            }

            MaterialId.Deserialize(stream);
            stream.Read(out FriendlyName);

            StaticParameters.Deserialize(stream);
            
            if (stream.Version >= 656) UniformExpressionSet.Deserialize(stream);

            stream.Read(out int tempPlatform);
            Platform = (EShaderPlatform)tempPlatform;
        }
    }
}