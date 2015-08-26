using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace TruMobility.Reporting.CDR
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //CreateDoc();
            Application.Run(new Form1());
        }

        public static void CreateDoc()
        {
            object _misValue = System.Reflection.Missing.Value;
            try
            {
                var excelApp = new Excel.Application();
                excelApp.Visible = true;
                excelApp.Workbooks.Add();
                Excel._Worksheet excelWorkSheet = (Excel.Worksheet)excelApp.ActiveSheet;

                // header
                excelWorkSheet.Cells[1, 10] = "Avio Group Daily Call Report";
                Excel.Range worksheetRange = excelWorkSheet.get_Range(1, 10);
                worksheetRange.MergeCells(10);
                worksheetRange.Font.Bold = true;

                excelWorkSheet.Cells[2, 1] = "CalledNumber";
                excelWorkSheet.Cells[2, 2] = "CallingNumber";
                excelWorkSheet.Cells[2, 3] = "Inbound/Outbound";
                excelWorkSheet.Cells[2, 4] = "CallDate";
                excelWorkSheet.Cells[2, 5] = "CallDuration";
                worksheetRange = excelWorkSheet.get_Range(2, 5);
                worksheetRange.Font.Bold = true;
                excelWorkSheet.Columns[1].AutoFit();
                excelWorkSheet.Columns[2].AutoFit();


                // save
                excelWorkSheet.SaveAs(@"d:\apps\logs\Test-Excel.xls"); // Excel.XlFileFormat.xlWorkbookNormal, _misValue, _misValue, _misValue, _misValue, Excel.XlSaveAsAccessMode.xlExclusive, _misValue, _misValue, _misValue, _misValue, _misValue);
                excelApp.Quit();
                //excelWorkSheet.Close(true, _misValue, _misValue);

            }
            catch (Exception e)
            {
                Console.WriteLine("ExceptionCaught::CreateDoc():" + e.Message);
            }


        }
    }
}
