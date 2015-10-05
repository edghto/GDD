#pragma once

using namespace Platform;

namespace WPStorage
{
	ref class WPStorageFile;
	ref class WPFileStream;

    public ref class StorageProxy sealed
    {
    public:
		StorageProxy();

		/*
		 * Retrievs collection of files at location determined by path
		 *   - path - string with following pattern "D:\\Windows\\*".
		 *
		 * Make notice about the asterix at the end of path. Yes, it 
		 * uses path as wildcard mask for FindFirstFileEx.
		 *
		 * Returns list of WPStorageFile (empty list if no file is find).
		 */
		Windows::Foundation::Collections::IVector<WPStorageFile^>^ GetFiles(String^ path);

		WPFileStream^ OpenWriteStream(String^ path);

		WPFileStream^ OpenReadStream(String^ path);

		void CloseStream(WPFileStream^ stream);
    };
}