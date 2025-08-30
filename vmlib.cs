using System;
using Vmmsharp;

namespace Nightfall_Vmm_Lib
{
    public class vmlib
    {
        //Reading funcs
        public static int ByteArrayToInt(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length != 4)
                throw new ArgumentException("Array muss genau 4 Bytes lang sein.");

            return BitConverter.ToInt32(bytes, 0);
        }

        public static ulong ReadPointer(VmmProcess proc, ulong baseAddress, params ulong[] offsets)
        {
            ulong addr = baseAddress;

            for (int i = 0; i < offsets.Length; i++)
            {
                //Read 8 bytes (x64bit)
                byte[] buffer = proc.MemRead(addr, 8);
                if (buffer == null || buffer.Length != 8)
                    throw new Exception("Error reading out memory");

                addr = BitConverter.ToUInt64(buffer, 0);
                if (addr == 0)
                    throw new Exception("Null pointer encountered");

                //Add offset if not the last iteration
                if (i < offsets.Length)
                    addr += offsets[i];
            }

            return addr;
        }
        public static float ReadFloat(VmmProcess proc, ulong address)
        {
            byte[] buffer = proc.MemRead(address, 4);
            if (buffer == null || buffer.Length != 4)
                throw new Exception("Error reading float value");

            return BitConverter.ToSingle(buffer, 0);
        }
        public static double ReadDouble(VmmProcess proc, ulong address)
        {
            byte[] buffer = proc.MemRead(address, 8);
            if (buffer == null || buffer.Length != 8)
                throw new Exception("Error reading double value");

            return BitConverter.ToDouble(buffer, 0);
        }

        public static int ReadInt32(VmmProcess proc, ulong address)
        {
            byte[] buffer = proc.MemRead(address, 4);
            if (buffer == null || buffer.Length != 4)
                throw new Exception("Error reading integer value");

            return BitConverter.ToInt32(buffer, 0);
        }

        public static byte ReadByte(VmmProcess proc, ulong address)
        {
            byte[] buffer = proc.MemRead(address, 1);
            if (buffer == null || buffer.Length != 1)
                throw new Exception("Error reading byte value");

            return buffer[0];
        }

        public static byte[] ReadByteArray(VmmProcess proc, ulong address, uint length)
        {
            byte[] buffer = proc.MemRead(address, length);
            if (buffer == null || buffer.Length != length)
                throw new Exception("Error reading byte array");

            return buffer;
        }

        //Write funcs
        public static void WriteFloat(VmmProcess proc, ulong address, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!proc.MemWrite(address, buffer))
                throw new Exception("Error writing float value");
        }

        public static void WriteDouble(VmmProcess proc, ulong address, double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!proc.MemWrite(address, buffer))
                throw new Exception("Error writing double value");
        }

        public static void WriteInt32(VmmProcess proc, ulong address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!proc.MemWrite(address, buffer))
                throw new Exception("Error writing integer value");
        }

        public static void WriteByte(VmmProcess proc, ulong address, byte value)
        {
            byte[] buffer = new byte[] { value };
            if (!proc.MemWrite(address, buffer))
                throw new Exception("Error writing byte value");
        }

        public static void WriteByteArray(VmmProcess proc, ulong address, byte[] values)
        {
            if (!proc.MemWrite(address, values))
                throw new Exception("Error writing byte array");
        }
    }
}