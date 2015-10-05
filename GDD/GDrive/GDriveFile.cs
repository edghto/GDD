namespace GDD
{
    public class GDriveFile : File
    {
        private Google.Apis.Drive.v2.Data.File item;
        public Google.Apis.Drive.v2.Data.File GetFile()
        {
            return item;
        }

        public GDriveFile(Google.Apis.Drive.v2.Data.File item)
        {
            this.item = item;
        }

        public override string Id
        {
            get
            {
                return item.Id;
            }
        }

        public override string Title
        {
            get
            {
                return item.Title;
            }
        }

        public override bool IsDirectory
        {
            get
            {
                return (item.MimeType == "application/vnd.google-apps.folder" ? true : false);
            }
        }

        public override long Length
        {
            get
            {
                return (long)item.FileSize;
            }
        }
    }
}
