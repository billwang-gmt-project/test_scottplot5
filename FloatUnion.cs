using System.Runtime.InteropServices;

// # IEEE-754 Floating Point Converter
// ## Reference
// - [IEEE-754 Floating Point Converter](https://www.h-schmidt.net/FloatConverter/IEEE754.html)
[StructLayout(LayoutKind.Explicit)]
struct FloatUnion
{
    [FieldOffset(0)]
    public uint uintValue;

    [FieldOffset(0)]
    public float floatValue;
}








