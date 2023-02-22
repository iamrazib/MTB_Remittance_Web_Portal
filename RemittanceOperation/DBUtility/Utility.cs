using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace RemittanceOperation.DBUtility
{
    public class Utility
    {
        public const string GCCSecurityCode = "GCC@@#@#10082021";
        public const string downloadBranch = "0100";
        public const string downloadUser = "1208";
        public const string GCCUserId = "GCCRemit";


        public static string HashSHA1Decryption(string value)
        {
            var shaSHA1 = System.Security.Cryptography.SHA1.Create();
            var inputEncodingBytes = Encoding.ASCII.GetBytes(value);
            var hashString = shaSHA1.ComputeHash(inputEncodingBytes);

            var stringBuilder = new StringBuilder();
            for (var x = 0; x < hashString.Length; x++)
            {
                stringBuilder.Append(hashString[x].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        internal static string[] GetExcelSheetNames(string excelFile)
        {
            OleDbConnection objConn = null;
            DataTable dt = null;

            try
            {                
                String connString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + excelFile + ";Extended Properties=Excel 8.0;";
                objConn = new OleDbConnection(connString);
                objConn.Open();
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                
                if (dt == null) { return null; }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                // Loop through all of the sheets if you want too...
                //for (int j = 0; j < excelSheets.Length; j++)
                //{
                // Query each excel sheet.
                //}
                
                return excelSheets;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }
                if (dt != null) { dt.Dispose(); }
            }
        }

        internal static DataTable GetExcelDataFromFirstSheet(string filepath, string filename, string sheetName)
        {
            DataTable dtResult = new DataTable();
            OleDbConnection conn = null;
            //oledbConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filepath + "\\" + filename + "; Extended Properties=Excel 8.0;";

            string oledbConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + filepath + "\\" + filename + "; Extended Properties=Excel 8.0;";
            conn = new OleDbConnection(oledbConnectionString);
            if (conn.State == ConnectionState.Closed){ conn.Open();  }

            string cmdText = "Select * from [" + sheetName + "]";
            OleDbCommand command = new OleDbCommand(cmdText, conn);
            OleDbDataAdapter objAdapter = new OleDbDataAdapter(command);
            DataSet objDataset = new DataSet();
            objAdapter.Fill(objDataset, "ExcelDataTable");
            dtResult = objDataset.Tables[0];
            //int rowcnt = dtResult.Rows.Count;
            conn.Close();
            return dtResult;
        }

        
        internal static void DeleteOldDaysFiles(string fileFolderPath)
        {
            string[] files = Directory.GetFiles(fileFolderPath);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                try
                {
                    //DateTime lastWrite = fi.LastWriteTime;
                    //DateTime lact = fi.LastAccessTime;
                    //DateTime ct = fi.CreationTime;

                    if (fi.LastWriteTime < DateTime.Now || fi.CreationTime < DateTime.Now)
                    {
                        fi.Delete();
                    }
                }
                catch (Exception exc)
                { }
            }
        }
    }
}