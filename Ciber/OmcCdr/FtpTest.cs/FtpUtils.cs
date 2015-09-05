using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Strata8.Wireless.Cdr
{
    public class FtpUtils
    {

        /// <summary>
        /// method to get a directory listing of the FTP site
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <returns></returns>
        public static List<string> ListDirectoryOnFtpSite(Uri ftpUri, string user, string pass)
        {
            string dirListing = string.Empty;
            List<string> dirList = new List<string>();

            // The serverUri parameter should use the ftp:// scheme.
            // It contains the name of the server file that is to be deleted.
            // Example: ftp://contoso.com/someFile.txt.
            // 
            try
            {
                if (ftpUri.Scheme != Uri.UriSchemeFtp)
                {
                    return dirList;
                }
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                NetworkCredential nc = new NetworkCredential(user, pass);
                CredentialCache cc = new CredentialCache();
                cc.Add(ftpUri, "Basic", nc);
                request.Credentials = cc;

                // get the FTP directory list to determine new file to download
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.ASCII);

                // maybe read each line here and get the latest file
                dirListing = sr.ReadToEnd();
                dirList = FtpUtils.ParseDirectoryList(dirListing);

                response.Close();

            }
            catch (SystemException se)
            {
                Console.WriteLine("ECaught:" + se.Message + se.StackTrace);

            }

            return dirList;

        }


        /// <summary>
        /// private method to parse the directory listing and return the last file name in the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<string> ParseDirectoryList(string list)
        {
            // we want the last line in the list of lines
            char[] trim = new char[] { ' ' };
            string fileName = string.Empty;
            List<string> fileList = new List<string>();

            // first split each line
            string[] lines = list.Split('\n');

            foreach (string aline in lines)
            {

                // split the line and grab the 
                string[] str = aline.Split(trim);
                // add each line to the list
                // fileList.Add(str[str.Length-1].Substring(0, str.Length -1 ));

                fileName = str[str.Length - 1];
                if (fileName.Length > 0)
                    fileList.Add(fileName.Substring(0, fileName.Length - 1));

                // we get the last filename in the list
                //string theLine = lines[lines.Length - 2];
                // fileName = theLine.Substring(0, theLine.Length - 1);

            }

            return fileList;
        }


        /// <summary>
        /// method to download the contents of a file from a remote URI
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string GetFileFromSite(Uri ftpUri, string user, string pass)
        {
            string fileString = string.Empty;

            // The serverUri parameter should start with the ftp:// scheme.
            if (ftpUri.Scheme != Uri.UriSchemeFtp)
            {
                return string.Empty;
            }
            // Get the object used to communicate with the server.
            WebClient request = new WebClient();

            // This example assumes the FTP site uses anonymous logon.
            NetworkCredential nc = new NetworkCredential(user, pass);
            CredentialCache cc = new CredentialCache();
            cc.Add(ftpUri, "Basic", nc);
            request.Credentials = cc;

            try
            {

                byte[] newFileData = request.DownloadData(ftpUri.ToString());
                fileString = System.Text.Encoding.UTF8.GetString(newFileData);
                //Console.WriteLine(fileString);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }
            return fileString;
        }

    }

}
