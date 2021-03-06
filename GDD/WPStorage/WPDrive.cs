﻿using Windows.Storage;

namespace GDD
{
    public class WPDrive : Drive
    {
        public StorageFolder storageFolder;
        private object currentLocationFolder;

        public string Name { get { return storageFolder.DisplayName; } }

        public WPDrive(StorageFolder storageFolder)
        {
            this.storageFolder = storageFolder;
        }

        public WPDrive(object currentLocationFolder)
        {
            this.currentLocationFolder = currentLocationFolder;
        }
    }

}