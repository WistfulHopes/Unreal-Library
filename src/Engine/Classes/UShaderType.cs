using System.Runtime.InteropServices;
using UELib.Core;

namespace UELib.Engine.Classes
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public class UShaderType : IUnrealSerializableClass
    {
        public UName FactoryName;
        public bool bIsValid;
        
        public void Deserialize(IUnrealStream stream)
        {
            FactoryName = new UName(stream);
        }

        public void Serialize(IUnrealStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}