#pragma once
#include "StorageFile.h"

using namespace Platform;

namespace WPStorage
{
	class StorageFactory
	{
	private:
		StorageFactory() {}

	public:
		static WPStorageFile^ GetStorageFile(WIN32_FIND_DATA data)
		{
			return ref new WPStorageFile(
				ref new String(data.cFileName),
				(data.nFileSizeHigh << sizeof(DWORD)) | (data.nFileSizeLow),
				(int)data.dwFileAttributes);
		}
	};
}