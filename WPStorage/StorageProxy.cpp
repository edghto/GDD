#include "pch.h"
#include "StorageProxy.h"
#include "StorageFile.h"
#include "StorageFactory.h"
#include "WPFileStream.h"
#include "Windows.h"
#include "WPStorageException.h"

namespace WPStorage
{
	StorageProxy::StorageProxy()
	{
	}

	Windows::Foundation::Collections::IVector<WPStorageFile^>^ StorageProxy::GetFiles(String^ path)
	{
		Windows::Foundation::Collections::IVector<WPStorageFile^>^ list =
			ref new Collections::Vector<WPStorageFile^>();
		WIN32_FIND_DATA data;
		HANDLE hFile = FindFirstFileEx(path->Data(), FindExInfoBasic, &data, FindExSearchNameMatch, NULL, 0);

		if (hFile == INVALID_HANDLE_VALUE)
		{
			return list;
		}

		while (FindNextFile(hFile, &data) != 0 || GetLastError() != ERROR_NO_MORE_FILES)
		{
			list->Append(StorageFactory::GetStorageFile(data));
		}
		
		return list;
	}

	WPFileStream^ StorageProxy::OpenWriteStream(String^ path)
	{
		HANDLE hFile = CreateFile2(
			path->Data(), 
			GENERIC_WRITE,
			0, // not sharing with other process,
			CREATE_NEW,
			NULL);

		if (hFile == INVALID_HANDLE_VALUE)
		{
			throw WPStorageException::GetException(HRESULT_FROM_WIN32(GetLastError()));
		}

		WPFileStream^ stream = ref new WPFileStream();
		HandleSetter(stream, hFile);

		return stream;
	}

	WPFileStream^ StorageProxy::OpenReadStream(String^ path)
	{
		HANDLE hFile = CreateFile2(
			path->Data(),
			GENERIC_READ,
			0, // not sharing with other process,
			OPEN_EXISTING,
			NULL);

		if (hFile == INVALID_HANDLE_VALUE)
		{
			throw WPStorageException::GetException(HRESULT_FROM_WIN32(GetLastError()));
		}

		WPFileStream^ stream = ref new WPFileStream();
		HandleSetter(stream, hFile);

		return stream;
	}

	void StorageProxy::CloseStream(WPFileStream^ stream)
	{
		HANDLE hFile = HandleGetter(stream);
		if (TRUE != CloseHandle(hFile))
		{
			throw WPStorageException::GetException(HRESULT_FROM_WIN32(GetLastError()));
		}
	}
}
