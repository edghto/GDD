//using System;
//using System.IO;

//namespace GDD
//{
//    class TranslateSeekOrigin
//    {
//        public static int Translate(SeekOrigin origin)
//        {
//            switch (origin)
//            {
//                case SeekOrigin.Begin:
//                    return 0;
//                case SeekOrigin.Current:
//                    return 1; 
//                case SeekOrigin.End:
//                    return 2;
//                default:
//                    throw new InvalidDataException("Seek origin is not valid");
//            }
//        }
//    }

//    public class WPFileStream : Stream, IDisposable
//    {
//        private WPStorage.WPFileStream streamImpl;
//        private bool _CanRead;
//        private long _Length;

//        public WPFileStream(WPStorage.WPFileStream streamImpl, bool read, long length)
//        {
//            this.streamImpl = streamImpl;
//            _CanRead = read;
//            _Length = length;
//        }

//        public override bool CanRead { get { return _CanRead; } }

//        public override bool CanSeek { get { return true; } }

//        public override bool CanWrite { get { return !_CanRead; } }

//        public override long Length { get { return _Length; } }

//        public override long Position
//        {
//            get { return streamImpl.Seek(0, TranslateSeekOrigin.Translate(SeekOrigin.Current)); }
//            set { throw new NotImplementedException(); }
//        }

//        public override void Flush() { streamImpl.Flush(); }

//        public override int Read(byte[] buffer, int offset, int count)
//        {
//            return streamImpl.Read(buffer, count);
//        }

//        public override long Seek(long offset, SeekOrigin origin)
//        {
//            return streamImpl.Seek(offset, TranslateSeekOrigin.Translate(origin));
//        }

//        public override void SetLength(long value)
//        {
//            throw new NotImplementedException();
//        }

//        public override void Write(byte[] buffer, int offset, int count)
//        {
//            streamImpl.Write(buffer, count);
//        }

//        public new void Dispose()
//        {
//            base.Dispose();
//            new WPStorage.StorageProxy().CloseStream(streamImpl);
//        }
//    }
//}
