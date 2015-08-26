using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using System.Web.Services;
using System.Web.Services.Protocols;

namespace Strata8.Telephony.Provisioning.Services
{
    public class BworksProvisioner
    {
        private BworksService bp;
        private string xml_header =  @"<BroadsoftDocument protocol=""OCI"" xmlns=""C"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">";
        private string xml_footer = @"</BroadsoftDocument>";
        private string newLine = "\n";
        private string sessionId = null;
        private string loginId = "admin";
        private string pass = "admin";
        private string testDigest = "dc70779bf8461b5a1e6aea58f636d5c0";
        private TCPManager tcpmgr = null;
   
        public BworksProvisioner()
        {
            bp = new BworksService();

            this.sessionId = "208.99.195.197" + "," + this.ToString().GetHashCode() + "," + System.DateTime.Now.Millisecond;
        }

        public bool ProcessLogIn()
        {
            try
            {
                // tcpmgr = new TCPManager();
                //Send the auth message
                String request = GetAuthMsg();
                
                bp = new BWProvisioningServiceService();                
                string xmlResponse = bp.processOCIMessage(request);
                //string xmlResponse = bp.sendMsg(request);

                //String xmlResponse = OCIUtilClient.SendOCIMsg(request);
                System.Console.WriteLine("Results:" + xmlResponse);

                if (xmlResponse != null)
                {
                    int indx1 = xmlResponse.IndexOf("AuthenticationResponse");
                    if (indx1 < 0)
                    {
                        System.Console.WriteLine("RequestAuthentication" + " Invalid: " + xmlResponse);
                        return false;
                    }

                    indx1 = xmlResponse.IndexOf("<nonce>");
                    int indx2 = xmlResponse.IndexOf("</nonce>");
                    if (indx2 <= indx1)
                    {
                        System.Console.WriteLine("InvalidAuthenticationResponse: " + xmlResponse);
                        return false;
                    }

                    indx1 += "<nonce>".Length;
                    String nonceValue = xmlResponse.Substring(indx1, indx2-indx1);

                    // hash the password
                    // MD5 Encrypted Password Calculation
                    //String pass = EncryptPassword( nonceValue );
                    String pass = OCIUtilClient.ComputeMessageDigest(this.pass, nonceValue);

                    // submit the login request message
                    String LoginRequest = this.GetLoginMsg(pass);
                    //xmlResponse = bp.sendMsg(request);

                    OCIUtilClient.SendOCIMsg(LoginRequest);


                    if (xmlResponse != null)
                    {
                        indx1 = xmlResponse.IndexOf("LoginResponse");
                        if (indx1 < 0)
                        {
                            System.Console.WriteLine("LoginRequest" + " command failed");
                            return false;
                        }
                        System.Console.WriteLine("LoginRequest" + " command successful\n");
                    }
                    else
                    {
                        System.Console.WriteLine("LoginRequest" + " command unsuccessful\n");
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("ECaught:" + ex.Message + "  StackTrace: " + ex.StackTrace);

            }

            return true;

        }//ProcessLogIn

        /// <summary>
        /// methed to request Authenitcation 
        /// </summary>
        /// <returns></returns>
        public string GetAuthMsg()
        {
            StringBuilder xmlRequest = new StringBuilder();
            xmlRequest.Append(xml_header + newLine);
            xmlRequest.Append( tab(1) + @"<sessionId xmlns="""">" + sessionId + "</sessionId>" + newLine);
            xmlRequest.Append(tab(1) + @"<command xsi:type=""AuthenticationRequest"" xmlns="""">" + newLine);
            xmlRequest.Append(tab(1) + @"<userId>admin</userId>" + newLine);
            xmlRequest.Append(tab(1) + @"</command>" + newLine);
            xmlRequest.Append(xml_footer + newLine);
            return xmlRequest.ToString();
        }


        public string GetLoginMsg(String passwordDigest)
        {
            StringBuilder xmlRequest = new StringBuilder();
            xmlRequest.Append(xml_header + newLine);
            xmlRequest.Append(tab(1) + @"<sessionId xmlns="""">" + sessionId + "</sessionId>" + newLine);
            xmlRequest.Append(tab(1) + @"<command xsi:type=""LoginRequest"" xmlns="""">" + newLine);
            xmlRequest.Append(tab(1) + @"<userId>admin</userId>" + newLine);
            xmlRequest.Append(tab(1) + @"<signedPassword>" + passwordDigest + "</signedPassword>" + newLine);
            xmlRequest.Append(tab(1) + @"</command>" + newLine);
            xmlRequest.Append(xml_footer + newLine);
            return xmlRequest.ToString();
        }

        private String tab(int num)
        {
            String rv = "";
            while (num-- > 0)
            {
                rv += "    ";
            }
            return rv;
        }

        /// <summary>
        /// method to compute the message digest of the user's plain password using 
        /// the SHA algorithm.
        /// </summary>
        /// <param name="nonceValue"></param>
        /// <returns>encrypted password to use</returns>
        public String EncryptPassword( string nonceValue )
        {
            string S1;
            string S2;
            byte[] data;
        
            // encrypted password to return
            string ePass = null;

             //using SHA to compute the message digest
            //UnicodeEncoding unicode = new UnicodeEncoding();
            UTF8Encoding unicode = new UTF8Encoding();
            data = unicode.GetBytes(this.pass);

            SHA1 sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            byte[] result = sha.ComputeHash(data);
                       
            // for every 4 bits in the 160-bit digest, starting from the first bit,
            // it is converted into a character in ASCII Hex format (0-9, a-f).  The 
            // result is a 40-character string.
            S1 = ConvertDigest( result, sha.HashSize ); 

            // create S2 that is used to create the digest using MD5
            // S2 = nonceValue + : + S1 (nonce is from the bworks auth response message
            S2 = nonceValue + ":" + S1;

            byte[] s2Bytes = unicode.GetBytes(S2);

            // calculate the message digest of S2 using the MD5 algorithm
            // This is one implementation of the abstract class MD5. 
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result2 = md5.ComputeHash( s2Bytes );// result is not encoded as the above for every 4 bits ...

            // for every 4 bits in the 128-bit digest starting from the 1st bit, it is 
            // converted into a character in the ASCII Hex format (0-9,a-f).  The result
            // is a 32 character string,e.g., dc70779bf8461b5a1e6aea58f636d5c0.
            ePass = ConvertDigest(result2, md5.HashSize);
            // this string is used as the password in the command:

            return ePass;

        }// EncryptPassword

        public string ConvertDigest(byte[] digestBytes, int hashSize)
        {

            char[] asciiDigestBytes = new char[hashSize/4];
            for (int i = 0; i < ( hashSize / 8); i++)
            {
                int outputOffset = i * 2;
                byte thisByte = digestBytes[i];
                char upperNibble = toAsciiHexNibble((thisByte & 0xF0) >> 4);
                char lowerNibble = toAsciiHexNibble(thisByte & 0x0F);

                // original
                asciiDigestBytes[outputOffset] =  upperNibble;
                asciiDigestBytes[outputOffset + 1] =  lowerNibble; 
            }

            String S1 = new String(asciiDigestBytes);

            return S1; 
        
        }//ConvertDigest

        private char toAsciiHexNibble(int hexValue)
        {
            char returnValue = '!';   // default
            if ((hexValue >= 0) && (hexValue <= 9))
            {
                returnValue = (char)((int)'0' + hexValue);
            }
            else if ((hexValue >= 0x000A) && (hexValue <= 0x000F))
            {
                returnValue = (char)((int)'a' + (hexValue - 0x000A));
            }
            return returnValue;
        }
    }
}
