using System;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using TruMobility.Utils.Logging;

namespace TruMobility.Reporting.CDR.Groups
{
    public class CreateExcelDoc 
    {
        public static string comma = ",";
        private static CdrDbProcessor _db = new CdrDbProcessor();
        private static string _group = System.Configuration.ConfigurationManager.AppSettings["Group"];

        public static void CreateCSV(List<UserCallReport> reportList, string fileName)
        {

            // create the header
            StringBuilder sb = new StringBuilder("Extension, UserNumber, InboundCalls, OutboundCalls,TotalCalls, TotalCallTime");
            LogFileMgr.Instance.WriteToFile(fileName, sb.ToString());

            // create the data
            foreach (UserCallReport userReport in reportList)
            {
                // let's get the extension for each user
                string ext = _db.GetExtension(userReport.UserNumber);
                StringBuilder b = new StringBuilder(ext + comma + userReport.UserNumber + comma + userReport.TotalInboundCalls + comma + userReport.TotalOutboundCalls + comma + userReport.TotalCalls +
                    comma +  userReport.TotalCallTime );
                LogFileMgr.Instance.WriteToFile(fileName, b.ToString());              
          
                // for each user get the call detail
                //foreach (CallInfo ci in userReport.Calls)
                //{
                //    StringBuilder b = new StringBuilder(ext + comma + userReport.UserNumber + comma + ci.CalledNumber + comma + ci.CallingNumber + comma + ci.Direction +
                //        comma + ci.CallDate.ToString("s") + comma + ci.Duration.ToString("g"));
                //    LogFileMgr.Instance.WriteToFile(fileName, b.ToString());
                //}
            }

        }

        /// <summary>
        /// method to create a list of all calls per user
        /// </summary>
        /// <param name="reportList"></param>
        /// <param name="fileName"></param>
        public static void CreateUserCSVReport(List<UserCallReport> reportList, string fileName)
        {
            // create the header
            StringBuilder sb = new StringBuilder("UserNumber, Direction, CallingNumber,CalledNumber, CallDate, AnswerIndicator, CallDuration");
            LogFileMgr.Instance.WriteToFile(fileName, sb.ToString());

            // create the data
            foreach (UserCallReport userReport in reportList)
            {
                List<CallInfo> ciList = userReport.Calls;

                // for each user get the call detail
                foreach (CallInfo ci in ciList)  //(CallInfo ci in userReport.Calls)
                {
                    // let's get the extension for each user
                    // string ext = _db.GetExtension(userReport.UserNumber);
                    StringBuilder b = new StringBuilder(ci.UserNumber + comma + ci.Direction + comma + ci.CallingNumber + comma + ci.CalledNumber + comma + ci.CallDate);
                    b.Append(comma + ci.AnswerIndicator + comma + ci.Duration);
                    LogFileMgr.Instance.WriteToFile(fileName, b.ToString());
                } // for each user in the group

            } // for each group

        } // user csv call report
    

        public static void CreateDoc(List<UserCallReport> reportList, string fileName )
        {
            object _misValue = System.Reflection.Missing.Value;
            try
            {
                var excelApp = new Excel.Application();
                excelApp.Visible = false;
                excelApp.Interactive = false;

                excelApp.Workbooks.Add(_misValue);
                Excel.Worksheet excelWorkSheet = (Excel.Worksheet)excelApp.ActiveSheet;

                // header
                excelWorkSheet.Cells[1, 7] = "Group Daily Call Report";
                //excelWorkSheet.Cells.Merge(excelWorkSheet.get_Range(1, 10));
                //Excel.Range worksheetRange = excelWorkSheet.get_Range(excelWorkSheet.Cells[1,10]);
                //worksheetRange.Font.Bold = true;
                excelWorkSheet.Cells[2, 1] = "CalledNumber";
                excelWorkSheet.Cells[2, 2] = "CallingNumber";
                excelWorkSheet.Cells[2, 3] = "Inbound/Outbound";
                excelWorkSheet.Cells[2, 4] = "CallDate";
                excelWorkSheet.Cells[2, 5] = "CallDuration";
                //worksheetRange = excelWorkSheet.get_Range(excelWorkSheet.Cells[2, 5]);
               // worksheetRange.Font.Bold = true;

                int indx = 2;
                foreach (UserCallReport userReport in reportList)
                {
                    
                    foreach (CallInfo ci in userReport.Calls)
                    {
                        indx++;

                        excelWorkSheet.Cells[indx, 1] = ci.CalledNumber;
                        excelWorkSheet.Cells[indx, 2] = ci.CallingNumber;
                        excelWorkSheet.Cells[indx, 3] = ci.Direction;
                        excelWorkSheet.Cells[indx, 4] = ci.CallDate.ToString("s");
                        excelWorkSheet.Cells[indx, 5] = ci.Duration.ToString("g");

                    }
                }

                // auto fit 
                excelWorkSheet.Columns[1].AutoFit();
                excelWorkSheet.Columns[2].AutoFit();
                excelWorkSheet.Columns[3].AutoFit();
                excelWorkSheet.Columns[4].AutoFit();
                excelWorkSheet.Columns[5].AutoFit();
                // save
                excelWorkSheet.SaveAs(fileName); //,Excel.XlFileFormat.xlWorkbookNormal, _misValue, _misValue, _misValue, _misValue, Excel.XlSaveAsAccessMode.xlExclusive, _misValue, _misValue, _misValue, _misValue, _misValue);
                excelApp.ActiveWorkbook.Close();
                excelApp.Quit();
                
                //excelWorkSheet.Close(true, _misValue, _misValue);
                
            }
            catch (Exception e)
            {
                LogFileMgr.Instance.WriteToLogFile("ExceptionCaught::CreateDoc():" + e.Message);
            }


        }


    }//class
}
