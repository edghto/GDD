using GDrive;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GDD
{
    public class GDriveDirectory : IDrive, INotifyPropertyChanged
    {
        private Stack<GDriveFile> currentDirectory;

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Name
        {
            get
            {
                return "GDrive";
            }
        }

        public bool IsInteractive
        {
            get
            {
                return false;
            }
        }

        public ObservableCollection<Drive> Drives
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Drive CurrentDrive
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private ObservableCollection<File> _FileCollection;
        public ObservableCollection<File> FileCollection
        {
            get
            {
                if (_FileCollection == null)
                    _FileCollection = new ObservableCollection<File>();
                return _FileCollection;
            }
            set
            {
                _FileCollection = value;
                RaisePropertyChanged("FileCollection");
            }
        }

        public GDriveDirectory()
        {
            currentDirectory = new Stack<GDriveFile>();
        }

        public bool ChangeDirectory(object newDir)
        {
            File file = newDir as File;
            if (file.Title == "..")
            {
                currentDirectory.Pop();
            }
            else
            {
                currentDirectory.Push(newDir as GDriveFile);
            }
            return true;
        }

        public File GetCurrentDir()
        {
            return currentDirectory.Count > 0 ? currentDirectory.Peek() : new File()
            {
                IsDirectory = true,
                Id = "root",
                Title = "root"
            };
        }

        public async Task<Collection<File>> GetListingAsync(object dir)
        {
            string dirStr = dir as string;
            Collection<File> listing = new Collection<File>();

            if (currentDirectory.Count != 0)
            {
                listing.Add(new File()
                {
                    IsDirectory = true,
                    Title = "..",
                });
            }

            foreach (var item in await Task.FromResult(Proxy.GetInstance().GetListing(dirStr)))
            {
                listing.Add(new GDriveFile(item));
            }

            return listing;
        }
        
        public async Task<Collection<File>> GetListingAsync()
        {
            string dirId = "root";
            if(currentDirectory.Count != 0)
            {
                dirId = currentDirectory.Peek().Id;
            }

            return await GetListingAsync(dirId);
        }

        private static string GetMimeType(string fileName)
        {
            /* copied as is from tutorial - what a shame :( */
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            //Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            //if (regKey != null && regKey.GetValue("Content Type") != null)
            //    mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public File GetNewFile(string title, string target)
        {
            char separator = '\\'; //System.IO.Path.DirectorySeparatorChar;
            string parent = target.Substring(0, target.IndexOf(separator));
            Google.Apis.Drive.v2.Data.File file = new Google.Apis.Drive.v2.Data.File();

            file.Title = title;
            file.Description = "Google Drive Downloader";
            file.MimeType = GetMimeType(title);
            file.Parents = new List<Google.Apis.Drive.v2.Data.ParentReference>()
            {
                new Google.Apis.Drive.v2.Data.ParentReference() { Id = parent }
            };

            return new GDriveFile(file);
        }

        public async Task<bool> CopyTo(object dst, Stream src)
        {
            File dstFile = dst as File;
            await GDrive.Proxy.GetInstance().Upload(src, ((GDriveFile)dstFile).GetFile());
            return true;
        }
    }
}