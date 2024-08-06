#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <windows.h>

static LPCSTR g_szTxtUuidFileName = "9c7d6346-ca8d-4340-b514-0339e498a37d.txt";
static LPCWSTR g_wszNotInstalledStr = L"Not Installed";

typedef HRESULT (*lpFnGetAvailableCoreWebView2BrowserVersionString)(PCWSTR browserExecutableFolder, LPWSTR *versionInfo);

int main(int argc, char **argv)
{
    HMODULE hLib = NULL;
    if (argc > 1)
    {
        hLib = LoadLibraryA(argv[1]);
    }

    if (hLib == NULL)
    {
        fprintf(stderr, "cannot load %s from as x86 build executable", argv[1]);
        return 1;
    }

    lpFnGetAvailableCoreWebView2BrowserVersionString GetAvailableCoreWebView2BrowserVersionString = (lpFnGetAvailableCoreWebView2BrowserVersionString)GetProcAddress(hLib, "GetAvailableCoreWebView2BrowserVersionString");
    if (GetAvailableCoreWebView2BrowserVersionString == NULL)
    {
        fprintf(stderr, "cannot load function GetAvailableCoreWebView2BrowserVersionString from dll %s", argv[1]);
        FreeLibrary(hLib);
        return 1;
    }

    LPWSTR wszOutVerStr = NULL;
    HRESULT hr = GetAvailableCoreWebView2BrowserVersionString(NULL, &wszOutVerStr);

    if (hr == HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND))
    {
        FILE *fp = fopen(g_szTxtUuidFileName, "w");
        fwrite(g_wszNotInstalledStr, sizeof(WCHAR), wcslen(g_wszNotInstalledStr), fp);
        fclose(fp);
    }
    else if (hr == HRESULT_FROM_WIN32(S_OK))
    {
        FILE *fp = fopen(g_szTxtUuidFileName, "w");
        fwrite(wszOutVerStr, sizeof(WCHAR), wcslen(wszOutVerStr), fp);
        CoTaskMemFree(wszOutVerStr);
        fclose(fp);
    }
    else
    {
        fprintf(stderr, "call GetAvailableCoreWebView2BrowserVersionString get a error code: %x", hr);
        FreeLibrary(hLib);
        return 1;
    }

    FreeLibrary(hLib);
    return 0;
}
