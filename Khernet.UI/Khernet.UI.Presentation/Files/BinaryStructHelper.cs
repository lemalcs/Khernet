using System;
using System.Runtime.InteropServices;

namespace Khernet.UI
{
    public static class BinaryStructHelper
    {
        /// <summary>
        /// Converts an struct to byte array
        /// </summary>
        /// <typeparam name="T">The type of struct</typeparam>
        /// <param name="st">The struct to convert to</param>
        /// <returns></returns>
        public static byte[] StructToByteArray<T>(T st) where T : struct
        {
            IntPtr pointer = IntPtr.Zero;

            try
            {
                //Get size of struct in bytes
                int size = Marshal.SizeOf(typeof(T));

                //Assing memory from unmanaged memory
                pointer = Marshal.AllocHGlobal(size);

                //Get pointer to unmanaged memory
                Marshal.StructureToPtr(st, pointer, true);

                //Copy struct bytes
                byte[] result = new byte[size];
                Marshal.Copy(pointer, result, 0, size);

                return result;
            }
            finally
            {
                //Free unmanaged memory
                if (pointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(pointer);
            }
        }

        /// <summary>
        /// Convert a byte array to struct
        /// </summary>
        /// <typeparam name="T">The struct to get</typeparam>
        /// <param name="st">The byte array</param>
        /// <returns></returns>
        public static T ByteArrayToStruct<T>(byte[] st) where T : struct
        {
            IntPtr pointer = IntPtr.Zero;

            try
            {
                //Get size of struct in bytes
                int size = Marshal.SizeOf(typeof(T));

                //Assing memory from unmanaged memory
                pointer = Marshal.AllocHGlobal(size);

                //Copy bytes to unmanaged memory
                Marshal.Copy(st, 0, pointer, size);

                //Get struct from bytes array
                object result = Marshal.PtrToStructure(pointer, typeof(T));

                return (T)result;
            }
            finally
            {
                //Free unmanaged memory
                if (pointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(pointer);
            }
        }

        /// <summary>
        /// Get struct size
        /// </summary>
        /// <param name="st">The type of struct</param>
        /// <returns></returns>
        public static int GetStructSize(Type st)
        {
            return Marshal.SizeOf(st);
        }
    }
}
