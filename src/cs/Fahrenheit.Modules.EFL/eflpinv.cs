using System.Runtime.InteropServices;

namespace Fahrenheit.Modules.EFL;

internal unsafe class EFLPInvoke {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] 
    public static extern nint CreateFile(
            string fileName,
            uint desiredAccess,
            uint shareMode,
            nint securityAttributes,
            uint creationDisposition,
            uint flagsAndAttributes,
            nint templateFile);

    [DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetDiskFreeSpace(
            char *lpRootPathName,
            out uint sectorsPerCluster,
            out uint bytesPerSector,
            out uint numberOfFreeClusters,
            out uint totalNumberOfClusters);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(nint hHandle);
}
