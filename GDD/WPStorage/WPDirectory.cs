using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GDD
{
    public class WPDirectory : IDrive, INotifyPropertyChanged
    {
        private Stack<WPFile> currentDirectory = new Stack<WPFile>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Drive CurrentDrive
        {
            get
            {
                return new WPDrive(CurrentLocationFolder);
            }

            set
            {
                WPDrive drive = value as WPDrive;
                if (drive != null)
                {
                    CurrentLocationFolder = drive.storageFolder;
                    currentDirectory.Clear();
                    RaisePropertyChanged("Name");
                    RaisePropertyChanged("CurrentDrive");
                }
                else
                {
                    throw new InvalidDataException("Provided object is not valid WPDrive");
                }
            }
        }

        public string Name
        {
            get
            {
                return CurrentLocationFolder.DisplayName;
            }
        }

        public bool IsInteractive
        {
            get
            {
                return true;
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

        public StorageFolder CurrentLocationFolder { get; private set; }

        public WPDirectory()
        {
            CurrentLocationFolder = ApplicationData.Current.LocalFolder;
        }

        public bool ChangeDirectory(object newDir)
        {
            File file = newDir as File;
            if (file.Title == "..")
            {
                if(currentDirectory.Count > 0)
                    currentDirectory.Pop();
            }
            else
            {
                var dir = newDir as WPFile;
                dir.Id = GetCurrentDir() + "\\" + dir.Title;
                currentDirectory.Push(dir);
            }
            return true;
        }
        
        public File GetCurrentDir()
        {
            return currentDirectory.Count > 0 ? 
                currentDirectory.Peek() : new WPFile(CurrentLocationFolder);
        }

        public async Task<Collection<File>> GetListingAsync()
        {
            return await GetListingAsync(GetCurrentDir());
        }

        public async Task<Collection<File>> GetListingAsync(object dir)
        {
            Collection<File> listing = new Collection<File>();

            var currentDir = (dir as WPFile).folderItem;
            foreach (var item in await currentDir.GetFoldersAsync())
            {
                listing.Add(new WPFile(item));
            }

            foreach (var item in await currentDir.GetFilesAsync())
            {
                var props = await item.GetBasicPropertiesAsync();
                listing.Add(new WPFile(item, props));
            }

            if (currentDirectory.Count != 0)
            {
                listing.Add(new File()
                {
                    IsDirectory = true,
                    Title = "..",
                });
            }

            return listing;
        }

        public File GetNewFile(string title, string target)
        {
            return new TargetWPFile((GetCurrentDir() as WPFile).folderItem, title);
        }

        public async Task<bool> CopyTo(object dst, Stream src)
        {
            var dstFile = dst as TargetWPFile;

            var dstStream = await dstFile.folder.OpenStreamForWriteAsync(
                dstFile.fileName, CreationCollisionOption.GenerateUniqueName);
            await new Copier().CopyAsync(dstStream, src);
            //await src.CopyToAsync(dstStream);
            return true;
        }
    }
}