using System;
using System.Collections.Generic;
using System.Text;
using Tamir.SharpSsh;


namespace SharepSSHTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Sftp sftp = new Sftp("[SERVER]", "[USERNAME]", string.Empty);
            sftp.AddIdentityFile("[Path to Private kEY]");
            sftp.Connect();
            sftp.Put(@"D:\temp\blog\feed.csv", "[PATH OF FILE ON SERVER]");
            sftp.Delete("[PATH OF FILE ON SERVER]");
            
        }
    }
}
