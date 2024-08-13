using System;
using System.Diagnostics;
using System.Linq;
using UELib.Core;

namespace UELib.Engine.Classes
{
    public interface IUnrealDecompilableClass : IUnrealSerializableClass
    {
        string Decompile();
    }

    public struct UMaterialUniformExpression : IUnrealSerializableClass
    {
        public UName TypeName;
        public IUnrealDecompilableClass ExpressionType;

        public void Deserialize(IUnrealStream stream)
        {
            TypeName = new UName(stream);
            var type = Type.GetType("UELib.Engine.Classes." + TypeName);
            Debug.Assert(type != null, $"{TypeName} does not exist");
            ExpressionType = (IUnrealDecompilableClass)Activator.CreateInstance(type);
            ExpressionType.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            return UDecompilingState.Tabs + TypeName + "={\r\n" + ExpressionType.Decompile() +
                   "\r\n" + UDecompilingState.Tabs + "}\r\n";
        }
    }

    public struct FMaterialUniformExpressionVectorParameter : IUnrealDecompilableClass
    {
        public UName ParameterName;
        public ULinearColor DefaultValue;

        public void Deserialize(IUnrealStream stream)
        {
            ParameterName = new UName(stream);
            DefaultValue.Serialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs +
                         $"ParameterName={ParameterName},DefaultValue=(R={DefaultValue.R},G={DefaultValue.G},B={DefaultValue.B},A={DefaultValue.A})";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionScalarParameter : IUnrealDecompilableClass
    {
        public UName ParameterName;
        public float DefaultValue;

        public void Deserialize(IUnrealStream stream)
        {
            ParameterName = new UName(stream);
            stream.Read(out DefaultValue);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + $"ParameterName={ParameterName},DefaultValue={DefaultValue}";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionTextureParameter : IUnrealDecompilableClass
    {
        public UName ParameterName;
        public FMaterialUniformExpressionTexture Base;

        public void Deserialize(IUnrealStream stream)
        {
            ParameterName = new UName(stream);
            Base.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + $"ParameterName={ParameterName}";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionFlipBookTextureParameter : IUnrealDecompilableClass
    {
        public FMaterialUniformExpressionTexture Base;

        public void Deserialize(IUnrealStream stream)
        {
            Base.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            return UDecompilingState.Tabs + Base.Decompile();
        }
    }

    public struct FMaterialUniformExpressionSine : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression X;
        public bool bIsCosine;

        public void Deserialize(IUnrealStream stream)
        {
            X.Deserialize(stream);
            stream.Read(out bIsCosine);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "X=\r\n" + $"{X.Decompile()}" + "\r\n" + UDecompilingState.Tabs +
                         "}\r\n" +
                         UDecompilingState.Tabs + $",bIsCosine={bIsCosine}";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionConstant : IUnrealDecompilableClass
    {
        public void Deserialize(IUnrealStream stream)
        {
            Value.Deserialize(stream);
            stream.Read(out ValueType);
        }

        public void Serialize(IUnrealStream stream)
        {
        }

        ULinearColor Value;
        byte ValueType;

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs +
                         $"ValueType={ValueType},Value=(R={Value.R},G={Value.G},B={Value.B},A={Value.A})";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionPeriodic : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression X;

        public void Deserialize(IUnrealStream stream)
        {
            X.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "X=\r\n" + $"{X.Decompile()}" + "\r\n" + UDecompilingState.Tabs +
                         "}\r\n";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionTime : IUnrealDecompilableClass
    {
        public void Deserialize(IUnrealStream stream)
        {
        }

        public void Serialize(IUnrealStream stream)
        {
        }

        public string Decompile()
        {
            return UDecompilingState.Tabs;
        }
    }

    public struct FMaterialUniformExpressionRealTime : IUnrealDecompilableClass
    {
        public void Deserialize(IUnrealStream stream)
        {
        }

        public void Serialize(IUnrealStream stream)
        {
        }

        public string Decompile()
        {
            return UDecompilingState.Tabs;
        }
    }

    public struct FMaterialUniformExpressionAbs : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression X;

        public void Deserialize(IUnrealStream stream)
        {
            X.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "X=\r\n" + $"{X.Decompile()}" + "\r\n" + UDecompilingState.Tabs +
                         "}\r\n";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionClamp : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression Input;
        public UMaterialUniformExpression Min;
        public UMaterialUniformExpression Max;

        public void Deserialize(IUnrealStream stream)
        {
            Input.Deserialize(stream);
            Min.Deserialize(stream);
            Max.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "Input=\r\n" + $"{Input.Decompile()}" + "\r\n" +
                         UDecompilingState.Tabs +
                         "}\r\n" + UDecompilingState.Tabs + "Min=\r\n" + $"{Min.Decompile()}" + "\r\n" +
                         UDecompilingState.Tabs +
                         "}\r\n" + UDecompilingState.Tabs + "Max=\r\n" + $"{Max.Decompile()}" + "\r\n}\r\n";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionMin : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression A;
        public UMaterialUniformExpression B;

        public void Deserialize(IUnrealStream stream)
        {
            A.Deserialize(stream);
            B.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "A=\r\n" + $"{A.Decompile()}" + "\r\n" + UDecompilingState.Tabs +
                         UDecompilingState.Tabs + "B=\r\n" + $"{B.Decompile()}" +
                         "\r\n" + UDecompilingState.Tabs;
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionMax : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression A;
        public UMaterialUniformExpression B;

        public void Deserialize(IUnrealStream stream)
        {
            A.Deserialize(stream);
            B.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "A=\r\n" + $"{A.Decompile()}" + "\r\n" + UDecompilingState.Tabs +
                         UDecompilingState.Tabs + "B=\r\n" + $"{B.Decompile()}" +
                         "\r\n" + UDecompilingState.Tabs;
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionFoldedMath : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression A;
        public UMaterialUniformExpression B;
        public byte Op;

        public void Deserialize(IUnrealStream stream)
        {
            A.Deserialize(stream);
            B.Deserialize(stream);
            stream.Read(out Op);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "A=\r\n" + $"{A.Decompile()}" + "\r\n" + UDecompilingState.Tabs +
                         UDecompilingState.Tabs + "B=\r\n" + $"{B.Decompile()}" +
                         "\r\n" + UDecompilingState.Tabs + $"Op={Op}";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionAppendVector : IUnrealDecompilableClass
    {
        public UMaterialUniformExpression A;
        public UMaterialUniformExpression B;
        public uint NumComponentsA;

        public void Deserialize(IUnrealStream stream)
        {
            A.Deserialize(stream);
            B.Deserialize(stream);
            stream.Read(out NumComponentsA);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + "A=\r\n" + $"{A.Decompile()}" + "\r\n" + UDecompilingState.Tabs +
                         UDecompilingState.Tabs + "B=\r\n" + $"{B.Decompile()}" +
                         "\r\n" + UDecompilingState.Tabs + $"NumComponentsA={NumComponentsA}";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct FMaterialUniformExpressionTexture : IUnrealDecompilableClass
    {
        public int TextureIndex;

        public void Deserialize(IUnrealStream stream)
        {
            stream.Read(out TextureIndex);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            UDecompilingState.AddTab();
            var output = UDecompilingState.Tabs + $"TextureIndex={TextureIndex}";
            UDecompilingState.RemoveTab();
            return output;
        }
    }

    public struct UShaderFrequencyUniformExpressions : IUnrealSerializableClass
    {
        public UArray<UMaterialUniformExpression> UniformVectorExpressions;
        public UArray<UMaterialUniformExpression> UniformScalarExpressions;
        public UArray<UMaterialUniformExpression> Uniform2DTextureExpressions;

        public void Deserialize(IUnrealStream stream)
        {
            stream.ReadArray(out UniformVectorExpressions);
            stream.ReadArray(out UniformScalarExpressions);
            stream.ReadArray(out Uniform2DTextureExpressions);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct UUniformExpressionSet : IUnrealSerializableClass
    {
        public UShaderFrequencyUniformExpressions PixelExpressions;
        public UArray<UMaterialUniformExpression> UniformCubeTextureExpressions;
        public UShaderFrequencyUniformExpressions VertexExpressions;
        public UShaderFrequencyUniformExpressions HullExpressions;
        public UShaderFrequencyUniformExpressions DomainExpressions;

        public void Deserialize(IUnrealStream stream)
        {
            PixelExpressions.Deserialize(stream);
            stream.ReadArray(out UniformCubeTextureExpressions);
            VertexExpressions.Deserialize(stream);
            HullExpressions.Deserialize(stream);
            DomainExpressions.Deserialize(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Decompile()
        {
            var output = UDecompilingState.Tabs + "UniformExpressionSet=\r\n";
            UDecompilingState.AddTabs(1);

            output += UDecompilingState.Tabs + "PixelUniformExpressions=\r\n";
            UDecompilingState.AddTabs(1);

            output += UDecompilingState.Tabs + "UniformVectorExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = PixelExpressions.UniformVectorExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "UniformScalarExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = PixelExpressions.UniformScalarExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "Uniform2DTextureExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = PixelExpressions.Uniform2DTextureExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            UDecompilingState.RemoveTabs(1);
            output += "\r\n" +UDecompilingState.Tabs;

            output += UDecompilingState.Tabs + "UniformCubeTextureExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = UniformCubeTextureExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            UDecompilingState.RemoveTabs(1);
            output += "\r\n" +UDecompilingState.Tabs;

            output += UDecompilingState.Tabs + "VertexExpressions=\r\n";
            UDecompilingState.AddTabs(1);

            output += UDecompilingState.Tabs + "UniformVectorExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = VertexExpressions.UniformVectorExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "UniformScalarExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = VertexExpressions.UniformScalarExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "Uniform2DTextureExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = VertexExpressions.Uniform2DTextureExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            UDecompilingState.RemoveTabs(1);
            output += "\r\n" +UDecompilingState.Tabs;

            output += UDecompilingState.Tabs + "HullExpressions=\r\n";
            UDecompilingState.AddTabs(1);

            output += UDecompilingState.Tabs + "UniformVectorExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = HullExpressions.UniformVectorExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "UniformScalarExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = HullExpressions.UniformScalarExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "Uniform2DTextureExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = HullExpressions.Uniform2DTextureExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            UDecompilingState.RemoveTabs(1);
            output += "\r\n" +UDecompilingState.Tabs;

            output += UDecompilingState.Tabs + "DomainExpressions=\r\n";
            output += "\r\n" +UDecompilingState.Tabs;

            output += UDecompilingState.Tabs + "UniformVectorExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = DomainExpressions.UniformVectorExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "UniformScalarExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = DomainExpressions.UniformScalarExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            output += UDecompilingState.Tabs + "Uniform2DTextureExpressions=\r\n";
            UDecompilingState.AddTabs(1);
            output = DomainExpressions.Uniform2DTextureExpressions.Aggregate(output,
                (current, expression) => current + expression.Decompile());
            UDecompilingState.RemoveTabs(1);
            output += "\r\n";

            UDecompilingState.RemoveTabs(1);
            output += "\r\n" +UDecompilingState.Tabs;

            UDecompilingState.RemoveTabs(1);
            output += "\r\n" +UDecompilingState.Tabs;
            
            return output;
        }
    }
}