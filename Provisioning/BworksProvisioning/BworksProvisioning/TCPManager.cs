
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace Strata8.Telephony.Provisioning.Services
{
    class TCPManager
    {
        private string m_serverToSendData = System.Configuration.ConfigurationManager.AppSettings["BworksServer"];
        private Int32 m_portToSendData = Convert.ToInt32(ConfigurationManager.AppSettings["BworksServerPort"]);    
        private IPAddress[] rAddress = Dns.GetHostAddresses(Dns.GetHostName());
             
        private static TcpClient m_client = null;

       public bool ConnectToServer()
        {
           // connect and maintain the connection
           m_client = new TcpClient(m_serverToSendData, m_portToSendData);
           return true;
        }

        public void Disconnect()
        {
            // Close everything.
            // stream.Close();
            m_client.Close();

        }

        public string ConnectAndSend(String message)
        {
            try
            {
                // prepend our IP address
                StringBuilder sb = new StringBuilder();
                //sb.Append(rAddress[0] + ",");
                //sb.Append(message);

                // Create a TcpClient.
               // TcpClient client = new TcpClient(m_serverToSendData, m_portToSendData);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message.ToString());

                // Get a client stream for reading and writing. 
                NetworkStream stream = m_client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                //Int32 bytes = stream.Read(data, 0, data.Length);
                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);

                // Check to see if this NetworkStream is readable.
                if (stream.CanRead)
                {
                    byte[] myReadBuffer = new byte[1024];
                    StringBuilder myCompleteMessage = new StringBuilder();
                    int numberOfBytesRead = 0;

                    // Incoming message may be larger than the buffer size.
                    do
                    {
                        numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                    }
                    while ( stream.DataAvailable );

                    // Print out the received message to the console.
                    Console.WriteLine("You received the following message : " +
                                                 myCompleteMessage);
                }
                else
                {
                    Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
                }


                return responseData;

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                return null;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return null;
            }

        }//senddata
    }
}