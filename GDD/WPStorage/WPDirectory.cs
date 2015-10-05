using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using WPStorage;

namespace GDD
{
    public class WPDirectory : IDrive
    {
        private StorageProxy proxy;
        private Stack<WPFile> currentDirectory = new Stack<WPFile>();

        private ObservableCollection<StorageFolder> _AvailableLocations;
        public ObservableCollection<StorageFolder> AvailableLocations
        {
            get
            {
                return _AvailableLocations;
            }
        }

        ObservableCollection<Drive> IDrive.Drives
        {
            get
            {
                ObservableCollection<Drive> list = new ObservableCollection<Drive>();
                foreach (var drive in AvailableLocations)
                {
                    list.Add(new WPDrive(drive));
                }
                return list;
            }
        }

        private StorageFolder CurrentLocationFolder;
        Drive IDrive.CurrentDrive
        {
            get
            {
                return new WPDrive(CurrentLocationFolder);
            }

            set
            {
                WPDrive drive = value as WPDrive;
                if (_AvailableLocations.Contains(drive.storageFolder))
                {
                    CurrentLocationFolder = drive.storageFolder;
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
                if (_AvailableLocations == null)
                {
                    _AvailableLocations = new ObservableCollection<StorageFolder>();
                }
                _AvailableLocations.Clear();
                PopulateLocalStorage();
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

        public WPDirectory()
        {
            proxy = new StorageProxy();
        }

        private void PopulateLocalStorage()
        {
            CurrentLocationFolder = ApplicationData.Current.LocalFolder;
            _AvailableLocations.Add(ApplicationData.Current.LocalFolder);
            var list = KnownFolders.RemovableDevices.GetFoldersAsync().AsTask().Result;
            foreach (var item in list)
            {
                CurrentLocationFolder = item;
                _AvailableLocations.Add(item);
            }
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
                WPFile dir = newDir as WPFile;
                dir.Id = GetCurrentDir() + "\\" + dir.Title;
                currentDirectory.Push(dir);
            }
            return true;
        }
        
        public string GetCurrentDir()
        {
            string dir = currentDirectory.Count > 0 ? currentDirectory.Peek().Id : CurrentLocationFolder.Path;
            if(dir.EndsWith("\\"))
            {
                dir = dir.Remove(dir.Length - 1);
            }
            return dir;
        }

        public ObservableCollection<File> GetListing()
        {
            string dir = GetCurrentDir();
            dir += "\\*";
            return GetListing(dir);
        }

        public ObservableCollection<File> GetListing(object dir)
        {
            return GetListingAsync(dir).Result;
        }

        public async Task<ObservableCollection<File>> GetListingAsync(object dir)
        {
            ObservableCollection<File> listing = new ObservableCollection<File>();
            
            var currentDir = dir as string;
            foreach (var item in proxy.GetFiles(currentDir))
            {
                WPFile file = new WPFile(item);
                file.Id = GetCurrentDir() + "\\" + file.Title;
                listing.Add(file);
            }

            if(listing.Count == 0 && currentDirectory.Count != 0)
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
            return new File() { Title = title, Id = target, IsDirectory = false };
        }

        public async Task<bool> CopyTo(object dst, Stream src)
        {
            File dstFile = dst as File;

            var dstWriteStream = new WPFileStream(proxy.OpenWriteStream(dstFile.Id), false, 0);
            return await new Copier().CopyAsync(dstWriteStream, src);
        }
    }
}