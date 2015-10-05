#include "pch.h"
#include "StorageFile.h"

namespace WPStorage
{
	WPStorageFile::WPStorageFile(String^ fileName, int fileSize, int attrs) :
		m_fileName(fileName),
		m_fileSize(fileSize),
		m_attrs(attrs)
	{
	}

	bool WPStorageFile::IsDirectory()
	{
		return (0 != (m_attrs & FILE_ATTRIBUTE_DIRECTORY));
	}

	int WPStorageFile::FileSize()
	{
		return m_fileSize;
	}

	String^ WPStorageFile::FileName()
	{
		return m_fileName;
	}
}
