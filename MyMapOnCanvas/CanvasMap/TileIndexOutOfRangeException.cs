using System;
using System.Runtime.Serialization;

namespace CanvasMap
{
    [Serializable]
    public class TileIndexOutOfRangeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TileIndexOutOfRangeException()
        {
        }

        public TileIndexOutOfRangeException(string message)
            : base(message)
        {
        }

        public TileIndexOutOfRangeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected TileIndexOutOfRangeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}