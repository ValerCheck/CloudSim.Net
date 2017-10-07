using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CloudSim.Sharp
{
    public class Log
    {
        private static string LINE_SEPARATOR = Environment.NewLine;

        private static Stream _output;

        private static bool _disabled;

        public static bool IsDisabled => _disabled;

        public static void Write(string message)
        {
            if (!IsDisabled)
            {
                try
                {                                    
                    using (var writer = new StreamWriter(GetOutput(), Encoding.Default, 512, true))
                    {
                        writer.AutoFlush = true;
                        writer.Write(message);
                    }
                }
                catch(Exception e)
                {
                    Debug.Write(e.StackTrace);
                }
            }
        }

        public static void Write(object message)
        {
            Write(message.ToString());
        }

        public static void WriteLine(string message)
        {
            Write(message + LINE_SEPARATOR);
        }

        public static void WriteLine()
        {
            Write(LINE_SEPARATOR);
        }

        public static void Format(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        public static void FormatLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args) + LINE_SEPARATOR);
        }

        public static void SetDisabled(bool disabled)
        {
            _disabled = disabled;
        }

        public static void Disable()
        {
            SetDisabled(true);         
        }

        public static void Enable()
        {
            SetDisabled(false);
        }

        public static Stream GetOutput()
        {
            if (_output == null)
            {
                SetOutput(new MemoryStream());
            }
            return _output;
        }

        public static void SetOutput(Stream output)
        {
            _output = output;
        }

        public static void WriteConcatLine(params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var arg in args)
                sb.Append(arg);

            WriteLine(sb.ToString());            
        }

        public static string GetLogText()
        {
            string result = "";
            try
            {
                using (var reader = new StreamReader(GetOutput(), Encoding.Default, false, 512, true))
                {
                    reader.BaseStream.Position = 0;
                    result = reader.ReadToEnd();
                    reader.BaseStream.Position = reader.BaseStream.Length;
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.StackTrace);
            }
            return result;
        }
    }
}
