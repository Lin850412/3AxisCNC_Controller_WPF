using System;
using System.Text;
using System.Runtime.InteropServices;

namespace IMICNC_WPF
{
    class CompilerAndInterpreter
    {
        [DllImport("CompilerFuncsDLL.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Compile")]
        public static extern bool Compile(StringBuilder inFilePath);

        [DllImport("InterpreterFuncsDLL.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "readFile")]
        public static extern void readFile(StringBuilder file);

        [DllImport("InterpreterFuncsDLL.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InterpretAll")]
        public static extern void InterpretAll(double programOriginX, double programOriginY, double programOriginZ);

        [DllImport("InterpreterFuncsDLL.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InterpretOneLine")]
        public static extern int InterpretOneLine(double programOriginX, double programOriginY, double programOriginZ);
    }
}
