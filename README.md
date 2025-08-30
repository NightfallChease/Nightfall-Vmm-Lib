# Nightfall-Vmm-Lib

Lightweight helper library around the Vmmsharp API that simplifies safe memory read / write operations for external tooling.

> Target Framework: .NET Framework 4.8.1  
> Language Version: C# 7.3

## Features
- Typed memory reads (byte, int, float, double, arbitrary byte[])
- Typed memory writes
- Multi-level pointer resolution with offset chain (`ReadPointer`)
- Argument validation & basic error reporting

## Getting Started
1. Add a reference to the Vmmsharp library (ensure its native/runtime dependencies are satisfied).
2. Include the `Nightfall_Vmm_Lib` namespace.
3. Use the static `vmlib` methods with an existing `VmmProcess` instance.

## Example
```csharp
using Nightfall_Vmm_Lib;
using Vmmsharp;

// Acquire a VmmProcess (implementation depends on Vmmsharp usage)
VmmProcess proc = /* obtain process instance */;

// Example: Resolve a multi-level pointer and read a float
ulong baseAddress = 0x140000000; // module base / known static address
ulong finalAddress = vmlib.ReadPointer(proc, baseAddress, 0x10, 0x20, 0x18);
float value = vmlib.ReadFloat(proc, finalAddress);

// Write a new value
vmlib.WriteFloat(proc, finalAddress, value + 1.0f);
```

## API Overview
| Method | Description |
|--------|-------------|
| `ReadPointer(VmmProcess, ulong, params ulong[])` | Follows a pointer chain (x64) applying each offset. Throws on null/intermediate failure. |
| `ReadFloat / ReadDouble / ReadInt32 / ReadByte` | Reads a primitive value. |
| `ReadByteArray` | Reads an arbitrary length buffer. |
| `WriteFloat / WriteDouble / WriteInt32 / WriteByte` | Writes primitive values. |
| `WriteByteArray` | Writes a byte buffer. |
| `ByteArrayToInt` | Utility: converts exactly 4 bytes to an `int`. |

## Pointer Reading Notes
`ReadPointer` expects that each dereferenced 8-byte value is a valid non-zero address. If a null (0) is encountered an exception is thrown early to prevent accidental writes to address 0.

## Error Handling
All methods throw `Exception` (or argument-related exceptions) on failure. Wrap calls if you need graceful degradation:
```csharp
try
{
    int hp = vmlib.ReadInt32(proc, address);
}
catch (Exception ex)
{
    // handle / log
}
```

## Safety & Responsibility
This library directly manipulates another process' memory. Ensure you:
- Comply with legal and ethical guidelines.
- Have permission to inspect or modify the target process.
- Understand that misuse can cause instability or crashes.

## Extending
Potential enhancements:
- Add async variants.
- Add span-based overloads (requires newer C# / framework adjustments).
- Introduce result types instead of throwing for performance-sensitive loops.

## License
Provide a license file (e.g. MIT) if you plan to distribute. (Currently unspecified.)

## Contributing
1. Fork & branch
2. Make changes (follow existing style)
3. Submit PR with clear description

## Disclaimer
For educational and diagnostic tooling purposes only.
