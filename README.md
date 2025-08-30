# Nightfall-Vmm-Lib

Lightweight helper library around the Vmmsharp API that simplifies safe memory read / write operations for external tooling.

> Target Framework: .NET Framework 4.8.1  
> Language Version: C# 7.3

## Table of Contents
- [Features](#features)
- [Getting Started](#getting-started)
- [Examples](#examples)
- [API Overview](#api-overview)
  - [Memory Reading Functions](#memory-reading-functions)
  - [Memory Writing Functions](#memory-writing-functions)
  - [Utility Functions](#utility-functions)
- [Function-Specific Notes](#function-specific-notes)
- [Error Handling](#error-handling)
- [Safety & Responsibility](#safety--responsibility)
- [Extending](#extending)
- [License](#license)
- [Contributing](#contributing)
- [Disclaimer](#disclaimer)

## Features
- Typed memory reads (byte, int, float, double, arbitrary byte[])
- Typed memory writes
- Multi-level pointer resolution with offset chain (`ReadPointer`)
- Argument validation & basic error reporting

## Getting Started
1. Add a reference to the Vmmsharp library (ensure its native/runtime dependencies are satisfied).
2. Include the `Nightfall_Vmm_Lib` namespace.
3. Use the static `vmlib` methods with an existing `VmmProcess` instance.

## Examples

### Basic Memory Reading
```csharp
using Nightfall_Vmm_Lib;
using Vmmsharp;

// Acquire a VmmProcess (implementation depends on Vmmsharp usage)
VmmProcess proc = /* obtain process instance */;

// Read different data types from memory
ulong address = 0x140000000;
float playerHealth = vmlib.ReadFloat(proc, address);
int playerLevel = vmlib.ReadInt32(proc, address + 0x4);
byte playerStatus = vmlib.ReadByte(proc, address + 0x8);
double preciseValue = vmlib.ReadDouble(proc, address + 0x10);

// Read a byte array (e.g., player name as bytes)
byte[] nameBytes = vmlib.ReadByteArray(proc, address + 0x20, 32);
```

### Pointer Chain Resolution
```csharp
// Multi-level pointer example (common in games)
ulong baseAddress = 0x140000000; // module base address
ulong playerObjectAddr = vmlib.ReadPointer(proc, baseAddress, 0x10, 0x20, 0x18);
float playerHealth = vmlib.ReadFloat(proc, playerObjectAddr + 0x100);

// Another example: inventory system
ulong inventoryAddr = vmlib.ReadPointer(proc, baseAddress, 0x8, 0x150, 0x28);
int itemCount = vmlib.ReadInt32(proc, inventoryAddr + 0x10);
```

### Memory Writing
```csharp
// Modify values in memory
ulong healthAddress = 0x12345678;
vmlib.WriteFloat(proc, healthAddress, 100.0f); // Set health to 100

// Write different data types
vmlib.WriteInt32(proc, healthAddress + 0x4, 50); // Set level to 50
vmlib.WriteByte(proc, healthAddress + 0x8, 1);   // Set status flag

// Write byte arrays
byte[] newName = System.Text.Encoding.UTF8.GetBytes("Player");
vmlib.WriteByteArray(proc, healthAddress + 0x20, newName);
```

### Utility Functions
```csharp
// Convert raw bytes to integer
byte[] rawData = vmlib.ReadByteArray(proc, someAddress, 4);
int convertedValue = vmlib.ByteArrayToInt(rawData);

// Error handling example
try
{
    float hp = vmlib.ReadFloat(proc, address);
    vmlib.WriteFloat(proc, address, hp + 10.0f);
}
catch (Exception ex)
{
    Console.WriteLine($"Memory operation failed: {ex.Message}");
}
```

## API Overview

### Memory Reading Functions

#### `ReadPointer(VmmProcess proc, ulong baseAddress, params ulong[] offsets)`
Follows a multi-level pointer chain (x64 architecture) by applying each offset sequentially.
- **Parameters:**
  - `proc`: The VmmProcess instance to read from
  - `baseAddress`: Starting memory address (usually a module base or static address)
  - `offsets`: Variable number of offset values to follow through the pointer chain
- **Returns:** `ulong` - Final resolved memory address
- **Throws:** `Exception` if memory read fails or null pointer (0) is encountered
- **Usage:** Ideal for traversing complex data structures or resolving game object pointers

#### `ReadFloat(VmmProcess proc, ulong address)`
Reads a 32-bit floating-point value from memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to read from
  - `address`: Memory address to read from
- **Returns:** `float` - The 4-byte floating-point value
- **Throws:** `Exception` if memory read fails or buffer size is incorrect

#### `ReadDouble(VmmProcess proc, ulong address)`
Reads a 64-bit double-precision floating-point value from memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to read from
  - `address`: Memory address to read from
- **Returns:** `double` - The 8-byte double-precision value
- **Throws:** `Exception` if memory read fails or buffer size is incorrect

#### `ReadInt32(VmmProcess proc, ulong address)`
Reads a 32-bit signed integer from memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to read from
  - `address`: Memory address to read from
- **Returns:** `int` - The 4-byte signed integer value
- **Throws:** `Exception` if memory read fails or buffer size is incorrect

#### `ReadByte(VmmProcess proc, ulong address)`
Reads a single byte from memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to read from
  - `address`: Memory address to read from
- **Returns:** `byte` - The single byte value
- **Throws:** `Exception` if memory read fails

#### `ReadByteArray(VmmProcess proc, ulong address, uint length)`
Reads an arbitrary-length byte array from memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to read from
  - `address`: Starting memory address to read from
  - `length`: Number of bytes to read
- **Returns:** `byte[]` - Array containing the read bytes
- **Throws:** `Exception` if memory read fails or actual length differs from requested

### Memory Writing Functions

#### `WriteFloat(VmmProcess proc, ulong address, float value)`
Writes a 32-bit floating-point value to memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to write to
  - `address`: Memory address to write to
  - `value`: The float value to write
- **Throws:** `Exception` if memory write operation fails

#### `WriteDouble(VmmProcess proc, ulong address, double value)`
Writes a 64-bit double-precision floating-point value to memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to write to
  - `address`: Memory address to write to
  - `value`: The double value to write
- **Throws:** `Exception` if memory write operation fails

#### `WriteInt32(VmmProcess proc, ulong address, int value)`
Writes a 32-bit signed integer to memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to write to
  - `address`: Memory address to write to
  - `value`: The integer value to write
- **Throws:** `Exception` if memory write operation fails

#### `WriteByte(VmmProcess proc, ulong address, byte value)`
Writes a single byte to memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to write to
  - `address`: Memory address to write to
  - `value`: The byte value to write
- **Throws:** `Exception` if memory write operation fails

#### `WriteByteArray(VmmProcess proc, ulong address, byte[] values)`
Writes an array of bytes to memory.
- **Parameters:**
  - `proc`: The VmmProcess instance to write to
  - `address`: Starting memory address to write to
  - `values`: Array of bytes to write
- **Throws:** `Exception` if memory write operation fails

### Utility Functions

#### `ByteArrayToInt(byte[] bytes)`
Converts a 4-byte array to a 32-bit signed integer.
- **Parameters:**
  - `bytes`: Byte array to convert (must be exactly 4 bytes)
- **Returns:** `int` - The converted integer value
- **Throws:** 
  - `ArgumentNullException` if bytes array is null
  - `ArgumentException` if array length is not exactly 4 bytes
- **Usage:** Useful for converting raw memory data to integer values

## Function-Specific Notes

### Pointer Chain Resolution (`ReadPointer`)
- Designed for x64 architecture (reads 8-byte pointers)
- Each intermediate address is validated to be non-zero
- Throws immediately if any step in the chain returns a null pointer
- Offsets are applied sequentially: `base -> (base + offset1) -> (result + offset2) -> ...`
- Last offset in the chain is applied to get the final address

### Memory Alignment Considerations
- No automatic alignment checking - ensure addresses are properly aligned for data types
- `float` and `int` values: typically 4-byte aligned
- `double` values: typically 8-byte aligned
- Misaligned reads may work but could impact performance

### Data Type Specifics
- **Float/Double**: Uses `BitConverter` for IEEE 754 compliance
- **Integer**: Reads as little-endian (standard x86/x64 byte order)
- **Byte Arrays**: Direct memory copy, no endianness conversion
- **ByteArrayToInt**: Expects little-endian 4-byte input

### Performance Tips
- Use `ReadByteArray` for bulk operations rather than multiple single reads
- Cache resolved pointer addresses when possible to avoid repeated chain traversal
- Consider the overhead of exception handling in performance-critical loops

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
