using System.Runtime.InteropServices;

namespace UELib.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct USHAHash : IUnrealSerializableClass, IUnrealAtomicStruct
    {
        public byte[] Hash;
        public void Deserialize(IUnrealStream stream)
        {
            Hash = new byte[20];
            for (var i = 0; i < Hash.Length; i++)
            {
                stream.Read(out Hash[i]);
            }
        }

        public void Serialize(IUnrealStream stream)
        {
            foreach (var b in Hash)
            {
                stream.Write(b);
            }
        }
    }
}