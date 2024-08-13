using System.Linq;
using UELib.Core;

namespace UELib.Engine.Classes
{
    public struct UStaticSwitchParameter : IUnrealSerializableClass
    {
        public UName ParameterName;
        public bool Value;
        public bool Override;
        public UGuid ExpressionGuid;

        public void Deserialize(IUnrealStream stream)
        {
            ParameterName = new UName(stream);
            stream.Read(out Value);
            stream.Read(out Override);
            ExpressionGuid.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct FStaticComponentMaskParameter : IUnrealSerializableClass
    {
        public UName ParameterName;
        public bool R, G, B, A;
        public bool Override;
        public UGuid ExpressionGUID;

        public void Deserialize(IUnrealStream stream)
        {
            ParameterName = new UName(stream);
            stream.Read(out R);
            stream.Read(out G);
            stream.Read(out B);
            stream.Read(out A);
            stream.Read(out Override);
            ExpressionGUID.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct FNormalParameter : IUnrealSerializableClass
    {
        public UName ParameterName;
        public byte CompressionSettings;
        public bool Override;
        public UGuid ExpressionGUID;

        public void Deserialize(IUnrealStream stream)
        {
            ParameterName = new UName(stream);
            stream.Read(out CompressionSettings);
            stream.Read(out Override);
            ExpressionGUID.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct FStaticTerrainLayerWeightParameter : IUnrealSerializableClass
    {
        public UName ParameterName;
        public bool Override;
        public UGuid ExpressionGUID;

        public int WeightmapIndex;

        public void Deserialize(IUnrealStream stream)
        {
            ParameterName = new UName(stream);
            stream.Read(out WeightmapIndex);
            stream.Read(out Override);
            ExpressionGUID.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct UStaticParameterSet : IUnrealSerializableClass
    {
        public UGuid BaseMaterialId;
        public UArray<UStaticSwitchParameter> StaticSwitchParameters;
        public UArray<FStaticComponentMaskParameter> StaticComponentMaskParameters;
        public UArray<FNormalParameter> NormalParameters;
        public UArray<FStaticTerrainLayerWeightParameter> TerrainLayerWeightParameters;

        public void Deserialize(IUnrealStream stream)
        {
            BaseMaterialId.Deserialize(stream);
            stream.ReadArray(out StaticSwitchParameters);
            stream.ReadArray(out StaticComponentMaskParameters);
            if (stream.Version >= 631) stream.ReadArray(out NormalParameters);
            if (stream.Version >= 714) stream.ReadArray(out TerrainLayerWeightParameters);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            var output = UDecompilingState.Tabs + "StaticParameterSet=\r\n";
            UDecompilingState.AddTabs(1);

            output += UDecompilingState.Tabs + "StaticSwitchParameters=\r\n";
            UDecompilingState.AddTabs(1);

            output = StaticSwitchParameters.Aggregate(output,
                (current, staticSwitch) => current + (UDecompilingState.Tabs +
                                                      $"ParameterName={staticSwitch.ParameterName},Value={staticSwitch.Value},Override={staticSwitch.Override}" +
                                                      "\r\n"));
            UDecompilingState.RemoveTabs(1);

            UDecompilingState.RemoveTabs(1);
            return output;
        }
    }
}