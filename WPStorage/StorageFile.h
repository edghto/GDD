#pragma once

using namespace Platform;


namespace WPStorage
{
	public ref class WPStorageFile sealed
	{
	private:
		bool m_isDirectory;
		unsigned long m_attrs;
		long m_fileSize;
		String^ m_fileName;

	public:
		WPStorageFile(String^ fileName, int fileSize, int attrs);
		bool IsDirectory();
		int FileSize();
		String^ FileName();
	};
}