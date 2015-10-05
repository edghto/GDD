#pragma once

namespace WPStorage
{
	class WPStorageException
	{
		WPStorageException() {}
	public:
		static Exception^ GetException(HRESULT hresult)
		{
			String ^message = ref new String();
			LPTSTR errorText = reinterpret_cast<LPTSTR>(
				HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY | HEAP_NO_SERIALIZE, 128));

			if (NULL != errorText)
			{
				FormatMessage(
					// use system message tables to retrieve error text
					FORMAT_MESSAGE_FROM_SYSTEM
					// Important! will fail otherwise, since we're not 
					// (and CANNOT) pass insertion parameters
					| FORMAT_MESSAGE_IGNORE_INSERTS,
					NULL,    // unused with FORMAT_MESSAGE_FROM_SYSTEM
					hresult,
					MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
					(LPTSTR)&errorText,  // output 
					0, // minimum size for output buffer
					NULL);   // arguments - see note 

				String ^message = ref new String(errorText);
				HeapFree(GetProcessHeap(), HEAP_ZERO_MEMORY | HEAP_NO_SERIALIZE, errorText);
			}

			return ref new Exception(hresult, message);
		}
	};
}
