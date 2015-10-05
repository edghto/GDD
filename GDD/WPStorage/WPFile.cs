using WPStorage;

namespace GDD
{
    public class WPFile : File
    {
        private WPStorageFile item;

        public WPFile(WPStorageFile item)
        {
            this.item = item;
        }

        public override string Title
        {
            get
            {
                return item.FileName();
            }
        }

        public override long Length
        {
            get
            {
                return item.FileSize();
            }
        }

        public override bool IsDirectory
        {
            get
            {
                return item.IsDirectory();
            }
        }
    }
}
