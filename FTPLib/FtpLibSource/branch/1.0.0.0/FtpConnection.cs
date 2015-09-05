using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FtpLib
{
    public class FtpConnection : IDisposable
    {
        public FtpConnection(string host)
        {
            _host = host;
        }

        public FtpConnection(string host, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;
        }

        public void Open()
        {
            _hInternet = WININET.InternetOpen(
                System.Environment.UserName,
                WININET.INTERNET_OPEN_TYPE_PRECONFIG,
                null,
                null,
                WININET.INTERNET_FLAG_SYNC);

            if (_hInternet == IntPtr.Zero)
            {
                Error();
            }
        }

        public void Login()
        {
            Login(_username, _password);
        }
        public void Login(string username, string password)
        {
            _hConnect = WININET.InternetConnect(_hInternet,
                _host,
                WININET.INTERNET_DEFAULT_FTP_PORT,
                username,
                password,
                WININET.INTERNET_SERVICE_FTP,
                WININET.INTERNET_FLAG_PASSIVE,
                IntPtr.Zero);

            if (_hConnect == IntPtr.Zero)
            {
                Error();
            }
        }

        public void SetCurrentDirectory(string directory)
        {
            int ret = WININET.FtpSetCurrentDirectory(
                _hConnect,
                directory);

            if (ret == 0)
            {
                Error();
            }
        }
        public void SetLocalDirectory(string directory)
        {
            if(Directory.Exists(directory))
                System.Environment.CurrentDirectory = directory;
            else
                throw new InvalidDataException(String.Format("{0} is not a directory!", directory));
        }

        public string GetCurrentDirectory()
        {
            int buffLength = WINAPI.MAX_PATH + 1;
            StringBuilder str = new StringBuilder(buffLength);
            int ret = WININET.FtpGetCurrentDirectory(_hConnect, str, ref buffLength);

            if (ret == 0)
            {
                Error();
                return null;
            }

            return str.ToString();
        }
        public FtpDirectoryInfo GetCurrentDirectoryInfo()
        {
            string dir = GetCurrentDirectory();
            return new FtpDirectoryInfo(this, dir);
        }

        public void GetFile(string remoteFile, bool failIfExists)
        {
            GetFile(remoteFile, remoteFile, failIfExists);
        }
        public void GetFile(string remoteFile, string localFile, bool failIfExists)
        {
            int ret = WININET.FtpGetFile(_hConnect,
                 remoteFile,
                 localFile,
                 failIfExists,
                 WINAPI.FILE_ATTRIBUTE_NORMAL,
                 WININET.FTP_TRANSFER_TYPE_BINARY,
                 IntPtr.Zero);

            if (ret == 0)
            {
                Error();
            }
        }

        public void PutFile(string fileName)
        {
            PutFile(fileName, Path.GetFileName(fileName));
        }
        public void PutFile(string localFile, string remoteFile)
        {
            int ret = WININET.FtpPutFile(_hConnect,
                localFile,
                remoteFile,
                WININET.FTP_TRANSFER_TYPE_BINARY,
                IntPtr.Zero);

            if (ret == 0)
            {
                Error();
            }
        }

        public void RenameFile(string existingFile, string newFile)
        {
            int ret = WININET.FtpRenameFile(_hConnect, existingFile, newFile);

            if (ret == 0)
                Error();
        }

        public void RemoveFile(string fileName)
        {
            int ret = WININET.FtpDeleteFile(_hConnect, fileName);

            if (ret == 0)
            {
                Error();
            }
        }
        
        public void RemoveDirectory(string directory)
        {
            int ret = WININET.FtpRemoveDirectory(_hConnect, directory);
            if (ret == 0)
                Error();
        }
       
        /// <summary>
        /// List all files and directories in the current working directory.
        /// </summary>
        /// <returns>A list of file and directory names.</returns>
        [Obsolete("Use GetFiles or GetDirectories instead.")]
        public List<string> List()
        {
            return List(null, false);
        }
        /// <summary>
        /// Provides backwards compatibility
        /// </summary>
        /// <param name="mask">The file mask used in the search.</param>
        /// <returns>A list of file matching the mask.</returns>
        [Obsolete("Use GetFiles or GetDirectories instead.")]
        public List<string> List(string mask)
        {
            return List(mask, false);
        }
        private List<string> List(bool onlyDirectories)
        {
            return List(null, onlyDirectories);
        }
        private List<string> List(string mask, bool onlyDirectories)
        {
            WINAPI.WIN32_FIND_DATA findData = new WINAPI.WIN32_FIND_DATA();

            IntPtr hFindFile = WININET.FtpFindFirstFile(
                _hConnect,
                mask,
                ref findData,
                WININET.INTERNET_FLAG_NO_CACHE_WRITE,
                IntPtr.Zero);
            try
            {
                List<string> files = new List<string>();
                if (hFindFile == IntPtr.Zero)
                {
                    if (Marshal.GetLastWin32Error() == WINAPI.ERROR_NO_MORE_FILES)
                    {
                        return files;
                    }
                    else
                    {
                        Error();
                        return files;
                    }
                }

                if (onlyDirectories && (findData.dfFileAttributes & WINAPI.FILE_ATTRIBUTE_DIRECTORY) == WINAPI.FILE_ATTRIBUTE_DIRECTORY)
                {
                    files.Add(new string(findData.fileName).TrimEnd('\0'));
                }
                else if (!onlyDirectories)
                {
                    files.Add(new string(findData.fileName).TrimEnd('\0'));
                }

                findData = new WINAPI.WIN32_FIND_DATA();
                while (WININET.InternetFindNextFile(hFindFile, ref findData) != 0)
                {
                    if (onlyDirectories && (findData.dfFileAttributes & WINAPI.FILE_ATTRIBUTE_DIRECTORY) == WINAPI.FILE_ATTRIBUTE_DIRECTORY)
                    {
                        files.Add(new string(findData.fileName).TrimEnd('\0'));
                    }
                    else if (!onlyDirectories)
                    {
                        files.Add(new string(findData.fileName).TrimEnd('\0'));
                    }
                    findData = new WINAPI.WIN32_FIND_DATA();
                }

                if (Marshal.GetLastWin32Error() != WINAPI.ERROR_NO_MORE_FILES)
                    Error();

                return files;
            }
            finally
            {
                if(hFindFile != IntPtr.Zero)
                    WININET.InternetCloseHandle(hFindFile);
            }
        }

        public FtpFileInfo[] GetFiles()
        {
            return GetFiles(GetCurrentDirectory()); 
        }
        public FtpFileInfo[] GetFiles(string mask) 
        {
            WINAPI.WIN32_FIND_DATA findData = new WINAPI.WIN32_FIND_DATA();

            IntPtr hFindFile = WININET.FtpFindFirstFile(
                _hConnect,
                mask,
                ref findData,
                WININET.INTERNET_FLAG_NO_CACHE_WRITE,
                IntPtr.Zero);
            try
            {
                List<FtpFileInfo> files = new List<FtpFileInfo>();
                if (hFindFile == IntPtr.Zero)
                {
                    if (Marshal.GetLastWin32Error() == WINAPI.ERROR_NO_MORE_FILES)
                    {
                        return files.ToArray();
                    }
                    else
                    {
                        Error();
                        return files.ToArray();
                    }
                }

                if ((findData.dfFileAttributes & WINAPI.FILE_ATTRIBUTE_DIRECTORY) != WINAPI.FILE_ATTRIBUTE_DIRECTORY)
                {
                    FtpFileInfo file = new FtpFileInfo(this, new string(findData.fileName).TrimEnd('\0'));
                    file.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
                    file.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
                    file.CreationTime = findData.ftCreationTime.ToDateTime();
                    file.Attributes = (FileAttributes)findData.dfFileAttributes;
                    files.Add(file);
                }

                findData = new WINAPI.WIN32_FIND_DATA();
                while (WININET.InternetFindNextFile(hFindFile, ref findData) != 0)
                {
                    if ((findData.dfFileAttributes & WINAPI.FILE_ATTRIBUTE_DIRECTORY) != WINAPI.FILE_ATTRIBUTE_DIRECTORY)
                    {
                        FtpFileInfo file = new FtpFileInfo(this, new string(findData.fileName).TrimEnd('\0'));
                        file.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
                        file.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
                        file.CreationTime = findData.ftCreationTime.ToDateTime();
                        file.Attributes = (FileAttributes)findData.dfFileAttributes;
                        files.Add(file);
                    }

                    findData = new WINAPI.WIN32_FIND_DATA();
                }

                if (Marshal.GetLastWin32Error() != WINAPI.ERROR_NO_MORE_FILES)
                    Error();

                return files.ToArray();
            }
            finally
            {
                if (hFindFile != IntPtr.Zero)
                    WININET.InternetCloseHandle(hFindFile);
            }
        }

        public FtpDirectoryInfo[] GetDirectories()
        {
            return GetDirectories(this.GetCurrentDirectory());
        }
        public FtpDirectoryInfo[] GetDirectories(string path) 
        {
            WINAPI.WIN32_FIND_DATA findData = new WINAPI.WIN32_FIND_DATA();
            
            IntPtr hFindFile = WININET.FtpFindFirstFile(
                _hConnect,
                path,
                ref findData,
                WININET.INTERNET_FLAG_NO_CACHE_WRITE,
                IntPtr.Zero);
            try
            {
                List<FtpDirectoryInfo> directories = new List<FtpDirectoryInfo>();

                if (hFindFile == IntPtr.Zero)
                {
                    if (Marshal.GetLastWin32Error() == WINAPI.ERROR_NO_MORE_FILES)
                    {
                        return directories.ToArray();
                    }
                    else
                    {
                        Error();
                        return directories.ToArray();
                    }
                }

                if ((findData.dfFileAttributes & WINAPI.FILE_ATTRIBUTE_DIRECTORY) == WINAPI.FILE_ATTRIBUTE_DIRECTORY)
                {
                    FtpDirectoryInfo dir = new FtpDirectoryInfo(this, new string(findData.fileName).TrimEnd('\0'));
                    dir.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
                    dir.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
                    dir.CreationTime = findData.ftCreationTime.ToDateTime();
                    dir.Attributes = (FileAttributes)findData.dfFileAttributes;
                    directories.Add(dir);
                }

                findData = new WINAPI.WIN32_FIND_DATA();

                while (WININET.InternetFindNextFile(hFindFile, ref findData) != 0)
                {
                    if ((findData.dfFileAttributes & WINAPI.FILE_ATTRIBUTE_DIRECTORY) == WINAPI.FILE_ATTRIBUTE_DIRECTORY)
                    {
                        FtpDirectoryInfo dir = new FtpDirectoryInfo(this, new string(findData.fileName).TrimEnd('\0'));
                        dir.LastAccessTime = findData.ftLastAccessTime.ToDateTime();
                        dir.LastWriteTime = findData.ftLastWriteTime.ToDateTime();
                        dir.CreationTime = findData.ftCreationTime.ToDateTime();
                        dir.Attributes = (FileAttributes)findData.dfFileAttributes;
                        directories.Add(dir);
                    }

                    findData = new WINAPI.WIN32_FIND_DATA();
                }

                if (Marshal.GetLastWin32Error() != WINAPI.ERROR_NO_MORE_FILES)
                    Error();

                return directories.ToArray();
            }
            finally
            {
                if (hFindFile != IntPtr.Zero)
                    WININET.InternetCloseHandle(hFindFile);
            }
        }

        public bool DirectoryExists(string path)
        {
            WINAPI.WIN32_FIND_DATA findData = new WINAPI.WIN32_FIND_DATA();

            IntPtr hFindFile = WININET.FtpFindFirstFile(
                _hConnect,
                path,
                ref findData,
                WININET.INTERNET_FLAG_NO_CACHE_WRITE,
                IntPtr.Zero);
            try
            {
                if (hFindFile == IntPtr.Zero && Marshal.GetLastWin32Error() != WINAPI.ERROR_NO_MORE_FILES)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                if (hFindFile != IntPtr.Zero)
                    WININET.InternetCloseHandle(hFindFile);
            }

        }

        public bool FileExists(string path)
        {
            WINAPI.WIN32_FIND_DATA findData = new WINAPI.WIN32_FIND_DATA();

            IntPtr hFindFile = WININET.FtpFindFirstFile(
                _hConnect,
                path,
                ref findData,
                WININET.INTERNET_FLAG_NO_CACHE_WRITE,
                IntPtr.Zero);
            try
            {
                if (hFindFile == IntPtr.Zero)
                {
                    return false;
                }

                return true;
            }
            finally
            {
                if (hFindFile != IntPtr.Zero)
                    WININET.InternetCloseHandle(hFindFile);
            }
        }


        public string SendCommand(string cmd)
        {
            int result;
            IntPtr dataSocket = new IntPtr();
            switch(cmd)
            {
                case "PASV":
                    result = WININET.FtpCommand(_hConnect, false, WININET.FTP_TRANSFER_TYPE_ASCII, cmd, IntPtr.Zero, ref dataSocket);
                    break;
                default:
                    result = WININET.FtpCommand(_hConnect, true, WININET.FTP_TRANSFER_TYPE_ASCII, cmd, IntPtr.Zero, ref dataSocket);
                    break;
            }

            int BUFFER_SIZE = 8192;

            if(result == 0){
                Error();
            }
            else if(dataSocket != IntPtr.Zero)
            {
                StringBuilder buffer = new StringBuilder(BUFFER_SIZE);
                int bytesRead = 0;

                do
                {
                    result = WININET.InternetReadFile(dataSocket, buffer, BUFFER_SIZE, ref bytesRead);
                } while (result == 1 && bytesRead > 1);

                return buffer.ToString();
                
            }

            return "";
        }

        public void Close()
        {
            WININET.InternetCloseHandle(_hConnect);
            _hConnect = IntPtr.Zero;

            WININET.InternetCloseHandle(_hInternet);
            _hInternet = IntPtr.Zero;
        }

        private string InternetLastResponseInfo(ref int code)
        {
            int BUFFER_SIZE = 8192;

            StringBuilder buff = new StringBuilder(BUFFER_SIZE);
            WININET.InternetGetLastResponseInfo(ref code, buff, ref BUFFER_SIZE);
            return buff.ToString();
        }

        private void Error()
        {
            int code = Marshal.GetLastWin32Error();

            if (code == WININET.ERROR_INTERNET_EXTENDED_ERROR)
            {
                string errorText = InternetLastResponseInfo(ref code);
                throw new FtpException(code, errorText);
            }
            else
            {
                throw new Win32Exception(code);
            }
        }

        private IntPtr _hInternet;
        private IntPtr _hConnect;

        private string _host;
        private string _username;
        private string _password;

        #region IDisposable Members

        public void Dispose()
        {
            if(_hConnect != IntPtr.Zero)
                WININET.InternetCloseHandle(_hConnect);

            if(_hInternet != IntPtr.Zero)
                WININET.InternetCloseHandle(_hInternet);
        }

        #endregion
    }
}
