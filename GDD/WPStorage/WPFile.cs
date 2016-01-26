using Windows.Storage;
using Windows.Storage.FileProperties;

namespace GDD
{

    public class TargetWPFile : File
    {
        public StorageFolder folder;
        public string fileName;

        public TargetWPFile(StorageFolder folder, string fileName)
        {
            this.folder = folder;
            this.fileName = fileName;
        }

        public override string Title
        {
            get
            {
                return fileName;
            }
        }

        public override long Length
        {
            get
            {
                return 0;
            }
        }

        public override bool IsDirectory
        {
            get
            {
                return false;
            }
        }
    }

    public class WPFile : File
    {
        public Windows.Storage.StorageFile fileItem = null;
        public Windows.Storage.StorageFolder folderItem = null;
        private object currentLocationFolder;
        private long size;
        private BasicProperties props;

        public WPFile(Windows.Storage.StorageFile item, BasicProperties props)
        {
            this.fileItem = item;
            this.props = props;
            size = (long)props.Size;
        }

        public WPFile(Windows.Storage.StorageFolder item)
        {
            this.folderItem = item;
        }

        public WPFile(object currentLocationFolder)
        {
            this.currentLocationFolder = currentLocationFolder;
        }

        public override string Id
        {
            get
            {
                return fileItem == null ? folderItem.Path: fileItem.Path;
            }
        }
        
        public override string Title
        {
            get
            {
                return fileItem == null ? folderItem.DisplayName : fileItem.DisplayName;
            }
        }

        public override long Length
        {
            get
            {
                return size;
            }
        }

        public override bool IsDirectory
        {
            get
            {
                return fileItem == null;
            }
        }
    }
}
