#pragma once

using namespace Platform;

namespace WPStorage
{
	ref class WPFileStream;

	void HandleSetter(WPFileStream^ stream, HANDLE hFile);
	HANDLE HandleGetter(WPFileStream^ stream);

	public ref class WPFileStream sealed
	{
		friend void HandleSetter(WPFileStream^ stream, HANDLE handle);
		friend HANDLE HandleGetter(WPFileStream^ stream);

	private:
		HANDLE m_fileHandle;

	public:
		int64 Length();
		int64 Position();
		void Flush();
		int64 Seek(int64 offset, int method);
		int WPFileStream::Read(Platform::WriteOnlyArray<uint8>^ buffer, int count);
		int WPFileStream::Write(const Platform::Array<uint8>^ buffer, int count);
	};
}
