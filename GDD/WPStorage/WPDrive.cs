using Windows.Storage;

namespace GDD
{
    public class WPDrive : Drive
    {
        public StorageFolder storageFolder;

        public string Name { get { return storageFolder.DisplayName; } }

        public WPDrive(StorageFolder storageFolder)
        {
            this.storageFolder = storageFolder;
        }
    }
}