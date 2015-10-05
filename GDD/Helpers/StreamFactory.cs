using System;
using System.IO;
using System.Threading.Tasks;

namespace GDD
{
    public class StreamFactory
    {
        public static Stream GetStream(File file, FileAccess access)
        {
            switch(access)
            {
                case FileAccess.Read: return GetStreamReader(file);
                case FileAccess.Write: return GetStreamWriter(file);
                default: return null;
            }
        }

        public static Stream GetStreamReader(File file)
        {
            if (file is GDriveFile)
            {
                return GDrive.Proxy.GetInstance().Download(((GDriveFile)file).GetFile());
            }
#if DESKTOP_SUPPORT
            else if (file is PCFile)
            {
                return new FileStream(file.Id, FileMode.Open, FileAccess.Read);
            }
#endif
            else if (file is WPFile)
            {
                WPStorage.StorageProxy proxy = new WPStorage.StorageProxy();
                return new WPFileStream(proxy.OpenReadStream(file.Id), true, file.Length);
            }
            else
            {
                return null;
            }
        }

        public static Stream GetStreamWriter(File file)
        {
            throw new NotImplementedException();
//            if (file is GDriveFile)
//            {
//                return GDrive.Proxy.GetInstance().Upload(((GDriveFile)file).GetFile());
//            }
//#if DESKTOP_SUPPORT
//            else if (file is PCFile)
//            {
//                return new FileStream(file.Id, FileMode.OpenOrCreate, FileAccess.Write);
//            }
//#endif
//            else if (file is WPFile)
//            {
//                return Task.FromResult(((WPFile)file).GetFile().OpenStreamForWriteAsync()).Result.Result;
//            }
//            else
//            {
//                return null;
//            }
        }

    }
}