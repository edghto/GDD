#include "pch.h"
#include "WPFileStream.h"
#include "Windows.h"
#include "WPStorageException.h"

namespace WPStorage
{
	/* Implementation of friend functions (used for members with unmarshalable types) */
	void HandleSetter(WPFileStream^ stream, HANDLE handle)
	{
		stream->m_fileHandle = handle;
	}
	HANDLE HandleGetter(WPFileStream^ stream)
	{
		return stream->m_fileHandle;
	}
	/* end of friend functions */


	void WPFileStream::Flush() 
	{
		(void)FlushFileBuffers(m_fileHandle);
	}

	int64 WPFileStream::Position()
	{
		return 0;
	}

	int64 WPFileStream::Length() 
	{
		return 0;
	}

	int64 WPFileStream::Seek(int64 offset, int method)
	{
		LARGE_INTEGER offsetLI;
		LARGE_INTEGER positionLI;

		offsetLI.QuadPart = offset;
		BOOL result = SetFilePointerEx(m_fileHandle, offsetLI, &positionLI, method);
		if (TRUE != result)
		{
			throw WPStorageException::GetException(HRESULT_FROM_WIN32(GetLastError()));
		}

		return positionLI.QuadPart;
	}

	int WPFileStream::Read(Platform::WriteOnlyArray<uint8>^ buffer, int count)
	{
		DWORD read = 0;

		char* data = (char*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY | HEAP_NO_SERIALIZE, count);
		if (data == nullptr)
		{
			return -2;
		}

		BOOL result = ReadFile(
			m_fileHandle,
			data,
			count,
			&read,
			NULL);

		if (TRUE != result)
		{
			return -1;
		}

		for (unsigned int i = 0; i < read; ++i)
		{
			buffer[i] = data[i];
		}
		HeapFree(GetProcessHeap(), HEAP_ZERO_MEMORY | HEAP_NO_SERIALIZE, data);

		return read;
	}

	int WPFileStream::Write(const Platform::Array<uint8>^ buffer, int count)
	{
		DWORD written = 0;

		BOOL result = WriteFile(
			m_fileHandle,
			buffer->Data,
			count,
			&written,
			NULL);

		if (TRUE != result)
		{
			return 0;
		}

		return written;
	}
}
