using RemittanceOperation.AppCode;
using RemittanceOperation.ModelClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace RemittanceOperation.DBUtility
{
    public class Manager
    {
        static ConnectionInfo connInfo = new ConnectionInfo();

        public string nrbworkConnectionString = connInfo.getNrbWorkConnString();
        public string drConnectionString = connInfo.getConnStringDR();
        public string remittanceDbLvConnectionString = connInfo.getConnStringRemitLv();
        //public string remittanceDbOldConnectionString = connInfo.getOldConnString();

        public string remittanceUATConnectionString = connInfo.getRemitUATConnString();

        MTBDBManager dbManager = null;

        internal DataTable GetUsersList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();
                //string query = "SELECT [UserRMCode]+' - '+[UserFullName] FROM [NRBWork].[dbo].[WEB_UserInfo] WHERE [isActive]=1";
                string query = "SELECT [UserRMCode]+' - '+[UserFullName]+' ('+FileProcessUserType+')' FROM [NRBWork].[dbo].[WEB_UserInfo] WHERE [isActive]=1 ORDER BY UserTypeId , UserFullName ";
                dt = dbManager.GetDataTable(query.Trim());
            }
            catch (Exception ex) { }
            finally { dbManager.CloseDatabaseConnection(); }
            return dt;
        }

        internal bool isPasswordMatch(string userRmCode, string usrPass, ref string userId, ref string userName, ref string userEmail
            , ref string fileProcessUserType, ref string IsMailReceive, ref string roleName, ref string userMenuList, string sessionId)
        {
            bool isFound = false;
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                //string queryDateCheck = "SELECT * FROM [NRBWork].[dbo].[WEB_UserInfo]  WHERE [UserRMCode]='" + userRmCode + "' AND [UserPassword]='" + usrPass + "' AND [isActive]=1";

                string queryUserCheck = "SELECT ui.[AutoId],ui.[UserId],ui.[UserRMCode],ui.[UserFullName],ui.[UserEmail],ui.[isActive],ui.[UserTypeId],ui.[FileProcessUserType],ui.[IsMailReceive], ur.RoleName "
                    + " FROM [NRBWork].[dbo].[WEB_UserInfo] ui INNER JOIN [NRBWork].[dbo].[WEB_UserRole] ur "
                    + " ON ui.UserTypeId=ur.RoleId  AND ur.isActive=1  AND ui.isActive=1 "
                    + " AND ui.[UserRMCode]='" + userRmCode + "' AND ui.[UserPassword]='" + usrPass + "' AND ui.[isActive]=1";

                DataTable dt = dbManager.GetDataTable(queryUserCheck);
                if (dt.Rows.Count > 0)
                {
                    isFound = true;
                    userId = dt.Rows[0]["UserId"].ToString();
                    userName = dt.Rows[0]["UserFullName"].ToString();
                    userEmail = dt.Rows[0]["UserEmail"].ToString();
                    fileProcessUserType = dt.Rows[0]["FileProcessUserType"].ToString();
                    IsMailReceive = dt.Rows[0]["IsMailReceive"].ToString();
                    roleName = dt.Rows[0]["RoleName"].ToString();
                    int roleId = Convert.ToInt32(dt.Rows[0]["UserTypeId"]);

                    //update login time
                    string updStmt = "UPDATE [NRBWork].[dbo].[WEB_UserInfo] SET [LastLoginTime]=getdate(), [SessionID]='" + sessionId + "' WHERE [UserId]='" + userId + "' AND [UserRMCode]='" + userRmCode + "'";
                    dbManager.ExcecuteCommand(updStmt); 
                   
                    //find menu for this user
                    userMenuList = GetThisUserMenuByRole(roleId);
                }
                else
                {
                    isFound = false;
                }
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return isFound;
        }

        private string GetThisUserMenuByRole(int roleId)
        {
            StringBuilder str = new StringBuilder();
            string menuUrl = "", menuTitle = "", pageLink = "";
            int menuParentId = 0;

            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUserMenuChild = "";
                string queryUserMenuParent = "SELECT m.[AutoId], m.[MenuId], m.[MenuTitle], m.[MenuURL], m.[MenuParentId] "
                    + " FROM [NRBWork].[dbo].[WEB_UserMenu] m inner join [NRBWork].[dbo].[WEB_RoleMenuMapping] rmm "
                    + " ON m.MenuId=rmm.MenuId AND rmm.RoleId=" + roleId + " AND m.[isActive]=1 AND m.MenuURL='#' ORDER BY m.[MenuPrioritySL], m.[SlNo]";

                DataTable dtMenuParent = dbManager.GetDataTable(queryUserMenuParent);
                DataTable dtMenuChild = new DataTable();

                if (dtMenuParent.Rows.Count > 0)
                {
                    str.Append("<div><a href=\"Home.aspx\" title=\"Home\">Home</a></div>");

                    for (int menuCount = 0; menuCount < dtMenuParent.Rows.Count; menuCount++)
                    {
                        menuUrl = dtMenuParent.Rows[menuCount]["MenuURL"].ToString();
                        menuTitle = dtMenuParent.Rows[menuCount]["MenuTitle"].ToString();
                        menuParentId = Convert.ToInt32(dtMenuParent.Rows[menuCount]["MenuParentId"]);

                        if (menuUrl.Equals("#"))  // parent menu
                        {
                            pageLink = "<a href=\"" + menuUrl + "\" title=\"" + menuTitle + "\">" + menuTitle + "</a>";
                            str.Append("<div class=\"menuheaders\">" + pageLink + "</div>");
                            
                            //------ child menu add -------
                            queryUserMenuChild = "SELECT m.[AutoId], m.[MenuId], m.[MenuTitle], m.[MenuURL], m.[MenuParentId] "
                                + " FROM [NRBWork].[dbo].[WEB_UserMenu] m INNER JOIN [NRBWork].[dbo].[WEB_RoleMenuMapping] rmm "
                                + " ON m.MenuId=rmm.MenuId AND rmm.RoleId=" + roleId + " AND m.[isActive]=1 AND rmm.isActive=1 AND m.MenuParentId=" + menuParentId + " AND m.[MenuURL]<>'#' ORDER BY m.[MenuId], m.[SlNo]";

                            dtMenuChild = dbManager.GetDataTable(queryUserMenuChild);

                            if (dtMenuChild.Rows.Count > 0)
                            {
                                str.Append("<ul class=\"menucontents\">");

                                for (int chmenuCount = 0; chmenuCount < dtMenuChild.Rows.Count; chmenuCount++)
                                {
                                    menuUrl = dtMenuChild.Rows[chmenuCount]["MenuURL"].ToString();
                                    menuTitle = dtMenuChild.Rows[chmenuCount]["MenuTitle"].ToString();

                                    pageLink = "<a href=\"" + menuUrl + "\">" + menuTitle + "</a>";
                                    str.Append("<li>" + pageLink + "</li>");
                                }
                                str.Append("</ul>");
                            }

                            //----- child menu add ------
                        }
                    }//for end

                }//if
            }
            catch (Exception ex) {   }
            return str.ToString();
        }

        internal void UpdateLogoutTime(string userId)
        {
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string updStmt = "UPDATE [NRBWork].[dbo].[WEB_UserInfo] SET [LogOutTime]=getdate() WHERE [UserId]='" + userId + "'";
                dbManager.ExcecuteCommand(updStmt);
            }
            catch (Exception ex){  }
            finally {  dbManager.CloseDatabaseConnection();  }
        }

        //internal DataTable GetAPIBasedExchBalance()
        //{
        //    DataTable dt = new DataTable();
        //    //try
        //    //{
        //    //    connNrbWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
        //    //    connNrbWorkSystem.Open();
        //    //}
        //    //catch (Exception exc) { }

        //    string query = "SELECT [PartyId],[ExchangeHouseName] 'Exchange House',[NRTAccount],[WalletAccount],[NRTBalance],convert(varchar, [LastUpdateTime], 120) LastUpdate, case when PartyTypeId=1 then 'WageEarners' else 'ServiceRemittance' end ExhType  FROM [NRBWork].[dbo].[APIBasedExchangeHouseInfo] Where [isActive]=1 ORDER BY [PartyId]";
        //    SqlDataAdapter sdaExchs = new SqlDataAdapter(query, nrbworkConnectionString);
        //    sdaExchs.Fill(dt);
        //    return dt;
        //}

        /*
        internal DataTable GetFileBasedExchBalance()
        {
            DataTable dt = new DataTable();
            //string query = "SELECT ROW_NUMBER() OVER(order by [ExchangeHouseName]) AS Sl,[ExchangeHouseName] 'Exchange House',[NRTAccount],[NRTBalance],convert(varchar, [LastUpdateTime], 120) LastUpdate  FROM [NRBWork].[dbo].[FileBasedExchangeHouseInfo] Where [isActive]=1 ORDER BY [Sl]";
            string query = "SELECT [PartyId],[ExchangeHouseName] 'Exchange House',[NRTAccount],[WalletAccount], [NRTBalance],convert(varchar, [LastUpdateTime], 120) LastUpdate, case when PartyTypeId=1 then 'WageEarners' else 'ServiceRemittance' end ExhType FROM [NRBWork].[dbo].[FileBasedExchangeHouseInfo] Where [isActive]=1 ORDER BY [PartyId] DESC ";
            SqlDataAdapter sdaExchs = new SqlDataAdapter(query, nrbworkConnectionString);
            sdaExchs.Fill(dt);
            return dt;
        } */

        /*
        internal string GetTotalExchBalance()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            //float totalAmount = 0;
            //float totalDistinctAmount = 0f;
            decimal totalDistinctAmount = 0;

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                //string query = "SELECT isnull(sum([NRTBalance]),0) FROM [NRBWork].[dbo].[APIBasedExchangeHouseInfo] Where [isActive]=1";
                //SqlDataAdapter sdaExchs = new SqlDataAdapter(query, connNRBWorkSystem);
                //sdaExchs.Fill(dt);
                //totalAmount = Convert.ToSingle(dt.Rows[0][0]);

                //--------------------------------------------------------
                DataTable dtAPIallDistinct = new DataTable();
                string queryDistinct = "SELECT distinct NRTAccount, round([NRTBalance],2) FROM [NRBWork].[dbo].[APIBasedExchangeHouseInfo] Where [isActive]=1 and [NRTBalance]>0";
                SqlDataAdapter sdaExchsDistinct = new SqlDataAdapter(queryDistinct, connNRBWorkSystem);
                sdaExchsDistinct.Fill(dtAPIallDistinct);

                for (int ai = 0; ai < dtAPIallDistinct.Rows.Count; ai++)
                {
                    totalDistinctAmount += Convert.ToDecimal(dtAPIallDistinct.Rows[ai][1]);
                }

                DataTable dtFileAllDistinct = new DataTable();
                queryDistinct = "SELECT distinct NRTAccount, round([NRTBalance],2) FROM [NRBWork].[dbo].[FileBasedExchangeHouseInfo] Where [isActive]=1 and [NRTBalance]>0";
                sdaExchsDistinct = new SqlDataAdapter(queryDistinct, connNRBWorkSystem);
                sdaExchsDistinct.Fill(dtFileAllDistinct);

                for (int fi = 0; fi < dtFileAllDistinct.Rows.Count; fi++)
                {
                    totalDistinctAmount += Convert.ToDecimal(dtFileAllDistinct.Rows[fi][1]);
                }

                //--------------------------------------------------------

                //dt = new DataTable();
                //query = "SELECT isnull(sum([NRTBalance]),0) FROM [NRBWork].[dbo].[FileBasedExchangeHouseInfo] Where [isActive]=1";
                //sdaExchs = new SqlDataAdapter(query, connNRBWorkSystem);
                //sdaExchs.Fill(dt);
                //totalAmount += Convert.ToSingle(dt.Rows[0][0]);
            }
            catch (Exception exc) { }

            return String.Format("{0:0.00}", totalDistinctAmount);
        }
        */

        /*
        internal string GetTotalWageEarnersExchBalance()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            decimal totalAmount = 0;

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                //string query = "SELECT isnull(sum([NRTBalance]),0) FROM [NRBWork].[dbo].[APIBasedExchangeHouseInfo] Where [isActive]=1 and PartyTypeId=1";
                //SqlDataAdapter sdaExchs = new SqlDataAdapter(query, connNRBWorkSystem);
                //sdaExchs.Fill(dt);
                //totalAmount = Convert.ToSingle(dt.Rows[0][0]);

                string queryDistinct = "SELECT distinct NRTAccount, [NRTBalance] FROM [NRBWork].[dbo].[APIBasedExchangeHouseInfo] Where [isActive]=1 and PartyTypeId=1 and [NRTBalance]>0";
                SqlDataAdapter sdaExchsDistinct = new SqlDataAdapter(queryDistinct, connNRBWorkSystem);
                sdaExchsDistinct.Fill(dt);

                for (int ai = 0; ai < dt.Rows.Count; ai++)
                {
                    totalAmount += Convert.ToDecimal(dt.Rows[ai][1]);
                }

                dt = new DataTable();
                queryDistinct = "SELECT distinct NRTAccount, [NRTBalance] FROM [NRBWork].[dbo].[FileBasedExchangeHouseInfo] Where [isActive]=1 and PartyTypeId=1 and [NRTBalance]>0";
                sdaExchsDistinct = new SqlDataAdapter(queryDistinct, connNRBWorkSystem);
                sdaExchsDistinct.Fill(dt);

                for (int fi = 0; fi < dt.Rows.Count; fi++)
                {
                    totalAmount += Convert.ToDecimal(dt.Rows[fi][1]);
                }

                //dt = new DataTable();
                //query = "SELECT isnull(sum([NRTBalance]),0) FROM [NRBWork].[dbo].[FileBasedExchangeHouseInfo] Where [isActive]=1 and PartyTypeId=1";
                //sdaExchs = new SqlDataAdapter(query, connNRBWorkSystem);
                //sdaExchs.Fill(dt);
                //totalAmount += Convert.ToSingle(dt.Rows[0][0]);
            }
            catch (Exception exc) { }

            return String.Format("{0:0.00}", totalAmount);
        }
        */

        /*
        internal string GetTotalServiceRemExchBalance()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            decimal totalAmount = 0;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT isnull(sum([NRTBalance]),0) FROM [NRBWork].[dbo].[APIBasedExchangeHouseInfo] Where [isActive]=1 and PartyTypeId=2";
                SqlDataAdapter sdaExchs = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaExchs.Fill(dt);
                totalAmount = Convert.ToDecimal(dt.Rows[0][0]);

                dt = new DataTable();
                query = "SELECT isnull(sum([NRTBalance]),0) FROM [NRBWork].[dbo].[FileBasedExchangeHouseInfo] Where [isActive]=1 and PartyTypeId=2";
                sdaExchs = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaExchs.Fill(dt);
                totalAmount += Convert.ToDecimal(dt.Rows[0][0]);
            }
            catch (Exception exc) { }

            return String.Format("{0:0.00}", totalAmount);
        }*/

        internal DataTable GetUserInfoByUserId(string userId)
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryDateCheck = "SELECT [AutoId],[UserId],[UserRMCode],[UserFullName],[UserPassword],[UserEmail],[isActive],[UserTypeId] FROM [NRBWork].[dbo].[WEB_UserInfo]  WHERE [UserId]='" + userId + "'";
                dt = dbManager.GetDataTable(queryDateCheck);
            }
            catch (Exception ex) { }
            finally{ dbManager.CloseDatabaseConnection(); }
            return dt;
        }

        internal DataTable GetUserInfoByUserRMCode(string userRmCode)
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryDateCheck = "SELECT [AutoId],[UserId],[UserRMCode],[UserFullName],[UserPassword],[UserEmail],[isActive],[UserTypeId] FROM [NRBWork].[dbo].[WEB_UserInfo]  WHERE [UserRMCode]='" + userRmCode + "'";
                dt = dbManager.GetDataTable(queryDateCheck);
            }
            catch (Exception ex) { }
            finally{ dbManager.CloseDatabaseConnection(); }
            return dt;
        }

        internal bool ChangePassword(string userId, string currPass, string newPass)
        {
            SqlConnection connNrbWorkSystem = null;

            try
            {
                connNrbWorkSystem = new SqlConnection(nrbworkConnectionString);
                connNrbWorkSystem.Open();
            }
            catch (Exception exc) { }

            string updStmt = "UPDATE [NRBWork].[dbo].[WEB_UserInfo] set [UserPassword]='" + newPass + "' WHERE [UserId]='" + userId + "' AND [UserPassword]='" + currPass + "'";
            SqlCommand _cmd = new SqlCommand(updStmt, connNrbWorkSystem);
            try { _cmd.ExecuteNonQuery(); }
            catch (Exception exc) { return false; }
            _cmd.Dispose();

            return true;
        }

        internal DataTable GetBEFTNDataFromNewSystem(string refNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); } 
            

                //string query = "select (select userid from Users u where u.PartyId=b.PartyId) Exch, ltrim(rtrim(b.RefNo))RefNo, "
                //        + " case when b.PaymentStatus=1 then 'Received' when b.PaymentStatus=2 then 'Canceled' when b.PaymentStatus=3 then 'Queued' "
                //        + " when b.PaymentStatus=4 then 'Uploded' when b.PaymentStatus=5 then 'Success' when b.PaymentStatus=6 then 'Returned' when b.PaymentStatus=7 then 'Ready for Disburse' "
                //        + " when b.PaymentStatus=8 then 'Disbursed' when b.PaymentStatus=9 then 'Stopped' else '' end Status, "
                //        + " (Select BE.Remarks From BEFTNExecutionStatus BE Where BE.BeftnTransactionId = BT.ID ) Remarks,  "  //BT.[ResponseData],
                //        + " b.BeneficiaryBankName bank, b.BeneficiaryBankBranchName branch, b.Amount, b.UplodedBy, "
                //        + " case when b.UplodedBy='AutoProcess' then  convert(varchar, BT.RequestDate, 20) else convert(varchar, b.UploadTime, 20) end RequestTime "
                //        + " , case when b.UplodedBy='AutoProcess' then  convert(varchar, BT.ResponseDate, 20) else convert(varchar, b.UploadTime, 20) end ProcessTime "
                //        + " ,convert(varchar, b.[ReturnedTime], 20) ReturnTime, ReturnedReason  "
                //        + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b left join [RemittanceDB].[dbo].[BEFTNTransaction] BT on b.AutoId = BT.BEFTNRequestId "
                //        + " WHERE b.[RefNo]='" + refNo + "'"
                //        + " ORDER BY BT.ID desc";

                string query = "select (SELECT u.UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId= b.[PartyId]) Exch,[RefNo], "
                        + " case when IsIncentive=0 or IsIncentive=99 then 'Main Txn' else 'Inctv Txn' end TxnType, "
                        + " case when PaymentStatus=1 then 'Received' "
                        + " when PaymentStatus=2 then 'Canceled' "
                        + " when PaymentStatus=3 then 'Queued' "
                        + " when PaymentStatus=4 then 'Uploded' "
                        + " when PaymentStatus=5 then 'Success'	"
                        + " when PaymentStatus=6 then 'Returned' "
                        + " when PaymentStatus=7 then 'Ready for Disburse' "
                        + " when PaymentStatus=8 then 'Disbursed' "
                        + " when PaymentStatus=9 then 'Stopped' "
                        + " when PaymentStatus=10 then 'Incentive Pending' "
                        + " when PaymentStatus=11 then 'Required Due Diligence' "
                        + " when PaymentStatus=12 then 'Non-Individual customer' "
                        + " when PaymentStatus=13 then 'Principal Amount Above 5 Lacs' "
                        + " when PaymentStatus=14 then 'Not Eligible For Incentive' "
                        + " else STR(PaymentStatus) end Status, "
                        + " (SELECT TOP 1 BE.Remarks From [RemittanceDB].[dbo].[BEFTNExecutionStatus] BE Where BE.BeftnTransactionId in( "
                        + "	SELECT max(BT.ID) from [RemittanceDB].[dbo].[BEFTNTransaction] BT where b.AutoId = BT.BEFTNRequestId ) order by BE.ID desc ) Remarks,"
                        + " [SenderName],[SenderAddress],[BeneficiaryName],[BeneficiaryAddress],[BeneficiaryAccountNo] AccountNo,[BeneficiaryBankName] BankName, "
                        + " [BeneficiaryBankBranchName] BranchName,[DestinationRoutingNO] RoutingNo,[Amount], [UplodedBy], "
                        + " REPLACE(CONVERT(CHAR(11), [RequestTime], 106),' ','-')+' '+convert(varchar, [RequestTime], 108)[RequestTime], "
                        + " REPLACE(CONVERT(CHAR(11), [LastProcessingTime], 106),' ','-')+' '+convert(varchar, [LastProcessingTime], 108) ProcessTime, PaymentDescription "
                        + " ,REPLACE(CONVERT(CHAR(11), [ReturnedTime], 106),' ','-') ReturnTime, ReturnedReason "
                        + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b where RefNo='" + refNo + "'";

                SqlDataAdapter sdaBEFTN = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTN.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetMTBDataFromNewSystem(string refNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select (SELECT userid FROM [RemittanceDB].[dbo].[Users] u where u.PartyId=ft.PartyId) Exch, ltrim(rtrim(ft.RefNo))RefNo, "
                + " case when ft.PaymentStatus=1 then 'Failed To Process' when ft.PaymentStatus=2 then 'Canceled' when ft.PaymentStatus=3 then 'Queued'  when ft.PaymentStatus=4 then 'Uploded' when ft.PaymentStatus=5 then 'Success' "
                + " when ft.PaymentStatus=6 then 'Returned' when ft.PaymentStatus=7 then 'Ready for Disburse' when ft.PaymentStatus=8 then 'Disbursed'  when ft.PaymentStatus=9 then 'Stopped' else '' end Status, "
                + " ft.TransactionCode TrCode, ft.BeneficiaryAccountNo BeneAcc, ft.BeneficiaryName benf, ft.SenderName sender, convert(varchar, ft.TransDate, 20)TransDate, ft.TrAmount Amt, "//ft.IsSuccess "
                + " ft.SenderCountry, case when isIncentive=5 then 'Inctv Success' when isIncentive=10 then 'Inctv Not Processed' else 'Inct Failed' end InctvStat, convert(varchar, ft.IncentiveProcessingTime, 20) InctvProcessTime "
                + " FROM [RemittanceDB].[dbo].[FundTransferRequest] ft  where [RefNo]='" + refNo + "'";

                SqlDataAdapter sdaMTB = new SqlDataAdapter(query, connDRSystem);
                sdaMTB.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBkashRegularDataFromNewSystem(string refNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo, "
                + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status, "
                + " mw.RemitReceiverMSISDN AccNo, mw.RemitReceiverAmount amount, mw.CBResponseDescription bKashStat, mw.CBapprovalCode BkashAppr, convert(varchar, mw.RequestTime, 20)RecvTime, convert(varchar, mw.MTBProcessTime, 20)ProcessTime, convert(varchar, mw.CBReceiveTime, 20) BkashResp,  tr.CBSValueDate "
                + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw left join [RemittanceDB].[dbo].[Transactions] tr ON mw.TranTxnId=tr.SessionId  where mw.[TranTxnId] LIKE '" + refNo + "%' and tr.IsSuccess=1"
                + " UNION ALL "
                + " select (select userid from Users u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo, "
                + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status, "
                + " mw.RemitReceiverMSISDN AccNo, mw.RemitReceiverAmount amount, mw.CBResponseDescription bKashStat, mw.CBapprovalCode BkashAppr, convert(varchar, mw.RequestTime, 20)RecvTime, convert(varchar, mw.MTBProcessTime, 20)ProcessTime, convert(varchar, mw.CBReceiveTime, 20) BkashResp,  tr.CBSValueDate "
                + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransferArchive] mw left join [RemittanceDB].[dbo].[Transactions] tr ON mw.TranTxnId=tr.SessionId  where mw.[TranTxnId] LIKE '" + refNo + "%' and tr.IsSuccess=1 ";

                SqlDataAdapter sdabKashReg = new SqlDataAdapter(query, connDRSystem);
                sdabKashReg.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBkashDirectDataFromNewSystem(string refNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=ds.PartyID) Exch, ds.TxnId RefNo, ds.ResponseDescription Status, "
                    + " ds.BeneficiaryWalletNumber AccNo, round(CONVERT(float, ds.Amount),2) amount, ds.ProcessedTimestamp RequestTime, convert(varchar, tr.ResponseDate, 20)ProcessTime, ds.ApprovalCode BkashAppr,"
                    + " ds.BeneficiaryName, ds.SenderName, ds.SourceCountryCode SrcCountry, case when ds.StatusID=2 then 'Processed' else 'Pending' end ProcStats "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds left join [RemittanceDB].[dbo].[Transactions] tr on ds.TxnId=tr.SessionId WHERE ds.TxnId = '" + refNo + "' and tr.IsSuccess=1";


                SqlDataAdapter sdabKashDir = new SqlDataAdapter(query, connDRSystem);
                sdabKashDir.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetCashTxnDataFromNewSystem(string refNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=ri.PartyId) Exch, ltrim(rtrim(ri.[RemitenceId])) RefNo, "
                    + " case when ri.Status=1 then 'Received' when ri.Status=2 then 'Canceled' when ri.Status=3 then 'Queued' when ri.Status=5 then 'Success' when ri.Status=6 then 'Returned' when ri.Status=7 then 'Ready for Disburse' when ri.Status=8 then 'Disbursed' when ri.Status=9 then 'Stopped' else '' end Statss, "
                    + " ri.SenderName, ri.BeneficiaryName BeneName, ri.BeneficiaryMobileNo BeneMobile, ri.Amount, convert(varchar, ri.RequestTime, 20)RequestTime, convert(varchar, orp.PayDateTime, 20)ProcessTime, orp.contactNumber, orp.address "
                    + " FROM [RemittanceDB].[dbo].[Remittanceinfo] ri left join [RemittanceDB].[dbo].[OtcRemitancePayment] orp "
                    + " on ri.RemitenceId=orp.RemitanceId  where ri.[RemitenceId]= '" + refNo + "'";

                SqlDataAdapter sdaCash = new SqlDataAdapter(query, connDRSystem);
                sdaCash.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }

            return dt;
        }

        internal DataTable GetRippleTxnDataFromNewSystem(string refNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (Select TOP 1 rpa.[UserName]  FROM [RemittanceDB].[dbo].[RipplePartnerAccountDetails] rpa Where rpa.NodeAddress=rpd.validator) Exch, "
                + " [sender_ETE_ID] PIN_No,[payment_id],[payment_type],[payment_state],[payment_module] Mode, Round(CONVERT(float, [quote_amount]),2) Amount, "
                + " [connector_role],[modified_at],[contract_created_at] created_at,[payment_request_time] request_time "
                + " FROM [RemittanceDB].[dbo].[RippleGETPaymentData] rpd  where [sender_ETE_ID]= '" + refNo + "'";

                SqlDataAdapter sdaRipl = new SqlDataAdapter(query, connDRSystem);
                sdaRipl.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        /*
        internal DataTable GetDataFromOldSystem(string refNo, string payMode)
        {
            SqlConnection connOldSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connOldSystem = new SqlConnection(connInfo.getOldConnString());
                if (connOldSystem.State.Equals(ConnectionState.Closed)) { connOldSystem.Open(); }
                            

                if (payMode.Equals("BEFTN"))
                {
                    string query = "select (select userid from Users u where u.PartyId=b.PartyId) Exch, b.RefNo, "
                        + " case when b.PaymentStatus=1 then 'Received' when b.PaymentStatus=2 then 'Canceled' when b.PaymentStatus=3 then 'Queued' "
                        + " when b.PaymentStatus=4 then 'Uploded' when b.PaymentStatus=5 then 'Success' when b.PaymentStatus=6 then 'Returned' when b.PaymentStatus=7 then 'Ready for Disburse' "
                        + " when b.PaymentStatus=8 then 'Disbursed' when b.PaymentStatus=9 then 'Stopped' else '' end Status,"
                        + " b.BeneficiaryName Benf, b.BeneficiaryBankName bank, b.BeneficiaryBankBranchName branch, b.Amount, convert(varchar, b.RequestTime, 20)RequestTime, b.UplodedBy, convert(varchar, b.UploadTime, 20) UploadTime "
                        + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b  where [RefNo]='" + refNo + "'";

                    SqlCommand cmd = new SqlCommand(query, connOldSystem);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter sdaBEFTN = new SqlDataAdapter(cmd);
                    sdaBEFTN.Fill(dt);
                }
                if (payMode.Equals("MTB"))
                {
                    string query = "select (select userid from Users u where u.PartyId=ft.PartyId) Exch, ft.RefNo, "
                        + " case when ft.PaymentStatus=1 then 'Received' when ft.PaymentStatus=2 then 'Canceled' when ft.PaymentStatus=3 then 'Queued'  when ft.PaymentStatus=4 then 'Uploded' when ft.PaymentStatus=5 then 'Success' "
                        + " when ft.PaymentStatus=6 then 'Returned' when ft.PaymentStatus=7 then 'Ready for Disburse' when ft.PaymentStatus=8 then 'Disbursed'  when ft.PaymentStatus=9 then 'Stopped' else '' end Status, "
                        + " ft.TransactionCode TrCode, ft.BeneficiaryAccountNo BeneAcc, ft.BeneficiaryName benf, convert(varchar, ft.TransDate, 20)TransDate, ft.TrAmount Amt, ft.IsSuccess "
                        + " FROM [RemittanceDB].[dbo].[FundTransferRequest] ft  where [RefNo]='" + refNo + "'";

                    SqlCommand cmd = new SqlCommand(query, connOldSystem);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter sdaMTB = new SqlDataAdapter(cmd);
                    sdaMTB.Fill(dt);
                }
                if (payMode.Equals("BkashRegular"))
                {
                    string query = "select (select userid from Users u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo, "
                        + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status, "
                        + " mw.RemitReceiverMSISDN AccNo, mw.RemitReceiverAmount amount, convert(varchar, mw.RequestTime, 20)RequestTime, mw.CBResponseDescription bKashStat, mw.CBapprovalCode BkashAppr, convert(varchar, mw.CBReceiveTime, 20) BkashRespTime, convert(varchar, mw.MTBProcessTime, 20)MTBProcessTime "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw  where [TranTxnId] LIKE '" + refNo + "%'";

                    SqlCommand cmd = new SqlCommand(query, connOldSystem);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter sdabKashReg = new SqlDataAdapter(cmd);
                    sdabKashReg.Fill(dt);
                }
                if (payMode.Equals("BkashDirect"))
                {
                    string query = "select (select userid from Users u where u.PartyId=ds.PartyID) Exch, ds.TxnId RefNo, ds.ResponseDescription Status, "
                    + " ds.BeneficiaryWalletNumber AccNo, round(CONVERT(float, ds.Amount),2) amount, ds.ProcessedTimestamp RequestTime, "
                    + " ds.ResponseDescription bKashStat, ds.ApprovalCode BkashAppr,"
                    + " ds.BeneficiaryName, ds.SenderName, ds.SourceCountryCode SrcCountry, case when ds.StatusID=2 then 'Processed' else 'Pending' end ProcStats "
                    + " FROM MobileWalletRemitDirectSettlement ds WHERE ds.TxnId = '" + refNo + "'";

                    SqlCommand cmd = new SqlCommand(query, connOldSystem);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter sdabKashDir = new SqlDataAdapter(cmd);
                    sdabKashDir.Fill(dt);
                }
                if (payMode.Equals("Cash"))
                {                   
                    string query = "select (select userid from Users u where u.PartyId=ri.PartyId) Exch, ltrim(rtrim(ri.[RemitenceId])) RefNo, "
                        + " case when ri.Status=1 then 'Received' when ri.Status=2 then 'Canceled' when ri.Status=3 then 'Queued' when ri.Status=5 then 'Success' when ri.Status=6 then 'Returned' when ri.Status=7 then 'Ready for Disburse' when ri.Status=8 then 'Disbursed' when ri.Status=9 then 'Stopped' else '' end Statss, "
                        + " ri.SenderName, ri.BeneficiaryName BeneName, ri.BeneficiaryMobileNo BeneMobile, ri.Amount, convert(varchar, ri.RequestTime, 20)RequestTime, convert(varchar, orp.PayDateTime, 20)ProcessTime, orp.contactNumber, orp.address "
                        + " FROM [RemittanceDB].[dbo].[Remittanceinfo] ri left join OtcRemitancePayment orp "
                        + " on ri.RemitenceId=orp.RemitanceId  where ri.[RemitenceId]= '" + refNo + "'";

                    SqlCommand cmd = new SqlCommand(query, connOldSystem);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter sdaCash = new SqlDataAdapter(cmd);
                    sdaCash.Fill(dt);
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connOldSystem != null && connOldSystem.State.Equals(ConnectionState.Open)) { connOldSystem.Close(); }
            }
            return dt;
        }
        */

        internal DataTable GetDataUsingMobileNoFromNewSystem(string mobileNo, string payMode)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }
                            

                if (payMode.Equals("BkashRegular"))
                {
                    string query = "select Exch, RefNo, Status, AccNo, amount, RequestTime, bKashStat, BkashAppr, BkashRespTime, MTBProcessTime "
                        + " from ( "
                        + " select (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo,  "
                        + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status,  "
                        + " mw.RemitReceiverMSISDN AccNo, mw.RemitReceiverAmount Amount, convert(varchar, mw.RequestTime, 20)RequestTime, mw.CBResponseDescription bKashStat, mw.CBapprovalCode BkashAppr, convert(varchar, mw.CBReceiveTime, 20) BkashRespTime, convert(varchar, mw.MTBProcessTime, 20)MTBProcessTime  "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw  where RemitReceiverMSISDN like '%" + mobileNo + "%' "
                        + " UNION ALL "
                        + " select (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo,  "
                        + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status,  "
                        + " mw.RemitReceiverMSISDN AccNo, mw.RemitReceiverAmount amount, convert(varchar, mw.RequestTime, 20)RequestTime, mw.CBResponseDescription bKashStat, mw.CBapprovalCode BkashAppr, convert(varchar, mw.CBReceiveTime, 20) BkashRespTime, convert(varchar, mw.MTBProcessTime, 20)MTBProcessTime  "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransferArchive] mw  where RemitReceiverMSISDN like '%" + mobileNo + "%' "
                        + " ) p "
                        + " order by RequestTime desc";

                    SqlCommand cmd = new SqlCommand(query, connDRSystem);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter sdabKashReg = new SqlDataAdapter(cmd);
                    sdabKashReg.Fill(dt);
                }
                if (payMode.Equals("BkashDirect"))
                {
                    string query = "select ds.[ID], (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=ds.PartyID) Exch, ds.TxnId RefNo, ds.ResponseDescription Status, "
                    + " ds.BeneficiaryWalletNumber AccNo, round(CONVERT(float, ds.Amount),2) Amount, convert(varchar, tr.ResponseDate, 20)ProcessTime, ds.ApprovalCode BkashAppr,"
                    + " ds.BeneficiaryName, ds.SenderName, ds.SourceCountryCode SrcCountry, case when ds.StatusID=2 then 'Processed' else 'Pending' end ProcStats "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds left join [RemittanceDB].[dbo].Transactions tr "
                    + " on ds.TxnId=tr.SessionId WHERE ds.[BeneficiaryWalletNumber] like '%" + mobileNo + "%' ORDER BY ds.[ID] desc";

                    SqlCommand cmd = new SqlCommand(query, connDRSystem);
                    cmd.CommandTimeout = 30;
                    SqlDataAdapter sdabKashDir = new SqlDataAdapter(cmd);
                    sdabKashDir.Fill(dt);
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        /*
        internal DataTable GetDataUsingMobileNoFromOldSystem(string mobileNo, string payMode)
        {
            SqlConnection connOldSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connOldSystem = new SqlConnection(connInfo.getOldConnString());
                if (connOldSystem.State.Equals(ConnectionState.Closed)) { connOldSystem.Open(); }
                            

                if (payMode.Equals("BkashRegular"))
                {
                    try
                    {
                        string query = "SELECT (select userid from Users u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo,  "
                            + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status,  "
                            + " mw.RemitReceiverMSISDN AccNo, mw.RemitReceiverAmount Amount, convert(varchar, mw.RequestTime, 20)RequestTime, mw.CBResponseDescription bKashStat, mw.CBapprovalCode BkashAppr, convert(varchar, mw.CBReceiveTime, 20) BkashRespTime, convert(varchar, mw.MTBProcessTime, 20)MTBProcessTime  "
                            + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw  WHERE RemitReceiverMSISDN like '%" + mobileNo + "%' "
                            + " ORDER BY RequestTime DESC";

                        SqlCommand cmd = new SqlCommand(query, connOldSystem);
                        cmd.CommandTimeout = 30;
                        SqlDataAdapter sdabKashReg = new SqlDataAdapter(cmd);
                        sdabKashReg.Fill(dt);
                    }
                    catch (Exception excp)
                    { }
                }
                if (payMode.Equals("BkashDirect"))
                {
                    try
                    {
                        string query = "select [ID], (select userid from Users u where u.PartyId=ds.PartyID) Exch, ds.TxnId RefNo, ds.ResponseDescription Status, "
                        + " ds.BeneficiaryWalletNumber AccNo, round(CONVERT(float, ds.Amount),2) Amount, ds.ProcessedTimestamp RequestTime, ds.ApprovalCode BkashAppr,"
                        + " ds.BeneficiaryName, ds.SenderName, ds.SourceCountryCode SrcCountry, case when ds.StatusID=2 then 'Processed' else 'Pending' end ProcStats "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds WHERE ds.[BeneficiaryWalletNumber] like '%" + mobileNo + "%' ORDER BY [ID] desc";

                        SqlCommand cmd = new SqlCommand(query, connOldSystem);
                        cmd.CommandTimeout = 30;
                        SqlDataAdapter sdabKashDir = new SqlDataAdapter(cmd);
                        sdabKashDir.Fill(dt);
                    }
                    catch (Exception excp)
                    { }
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connOldSystem != null && connOldSystem.State.Equals(ConnectionState.Open)) { connOldSystem.Close(); }
            }
            return dt;
        }
        */

        internal DataTable GetDataByRefNo(string refno, string paymentMode)
        {
            SqlConnection connDRSystem = null;

            DataTable dt = new DataTable();
            SqlDataAdapter sdaExchs;
            string query = "";

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }                
            

                if (paymentMode.Equals("BEFTN"))
                {
                    query = "select (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=b.PartyId) Exch, ltrim(rtrim(b.RefNo))RefNo, "
                     + " case when b.PaymentStatus=1 then 'Received' when b.PaymentStatus=2 then 'Canceled' when b.PaymentStatus=3 then 'Queued' when b.PaymentStatus=4 then 'Uploded' "
                     + " when b.PaymentStatus=5 then 'Success' when b.PaymentStatus=6 then 'Returned' when b.PaymentStatus=7 then 'Ready for Disburse' when b.PaymentStatus=8 then 'Disbursed' "
                     + " when b.PaymentStatus=9 then 'Stopped' else '' end Status, "
                     + " b.SenderName Sender, b.BeneficiaryName Benefcry, b.BeneficiaryBankName Bank, b.BeneficiaryBankBranchName Branch, b.DestinationRoutingNO RoutingNo, "
                    + " b.BeneficiaryAccountNo AccountNo, b.Amount, convert(varchar, b.RequestTime, 20)RequestTime, b.UplodedBy ProcessBy, "
                    + " case when b.UplodedBy='AutoProcess' then  convert(varchar, b.LastProcessingTime, 20) else convert(varchar, b.UploadTime, 20) end ProcessTime, " //BE.Remarks  "
                    + " (select BE.Remarks from [RemittanceDB].[dbo].[BEFTNExecutionStatus] BE where BE.BEFTNRequestId =b.AutoId and ID in( select max(ID) from [RemittanceDB].[dbo].[BEFTNExecutionStatus] BE where BE.BEFTNRequestId =b.AutoId))Remarks, "
                    + " case when b.IsIncentive=0 or b.IsIncentive=99 then 'Main Txn'  else 'Inctv Txn' end TxnType "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b "
                    + " where b.RefNo ='" + refno + "' and b.IsIncentive IN(0,99) ";

                }
                else if (paymentMode.Equals("MTBAC"))
                {
                    query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=b.PartyId) Exch, ltrim(rtrim([RefNo]))RefNo, "
                        + " case when b.PaymentStatus=1 then 'Failed To Process' when b.PaymentStatus=2 then 'Canceled' when b.PaymentStatus=3 then 'Queued' when b.PaymentStatus=4 then 'Uploded' "
                        + " when b.PaymentStatus=5 then 'Success' when b.PaymentStatus=6 then 'Returned' when b.PaymentStatus=7 then 'Ready for Disburse' when b.PaymentStatus=8 then 'Disbursed' "
                        + " when b.PaymentStatus=9 then 'Stopped' else '' end Status, [TransactionCode] TxnCode,[FromAccount],[BeneficiaryAccountNo]AccountNo,[TrAmount] Amount,[BeneficiaryName],convert(varchar, b.[TransDate], 20) TxnDate, "
                        + " [SenderName],[IsSuccess], "
                        + " b.SenderCountry, case when b.isIncentive=5 then 'Inctv Success' when b.isIncentive=10 then 'Inctv Not Processed' else 'Inct Failed' end InctvStat, convert(varchar, b.IncentiveProcessingTime, 20) InctvProcessTime "
                        + " FROM [RemittanceDB].[dbo].[FundTransferRequest] b  where RefNo='" + refno + "'";
                }
                else if (paymentMode.Equals("BKASH"))
                {
                    query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo, "
                        + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status, "
                        + " mw.RemitReceiverMSISDN Receiver, mw.RemitReceiverAmount Amount, mw.SenderMSISDN Sender, mw.CBResponseDescription bKashMsg, mw.CBapprovalCode bKashCode, convert(varchar, mw.RequestTime, 20)RecvTime, convert(varchar, mw.MTBProcessTime, 20)ProcessTime, convert(varchar, mw.CBReceiveTime, 20) BkashResp,  tr.CBSValueDate "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw left join [RemittanceDB].[dbo].[Transactions] tr ON mw.TranTxnId=tr.SessionId  where [TranTxnId] LIKE '" + refno + "%'"
                        + " UNION ALL "
                        + "SELECT (select userid from Users u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo, "
                        + " case when mw.RemitStatus=3 then 'Queued' when mw.RemitStatus=6 then 'Canceled' when mw.RemitStatus=5 then 'Success' else '' end Status, "
                        + " mw.RemitReceiverMSISDN Receiver, mw.RemitReceiverAmount Amount, mw.SenderMSISDN Sender, mw.CBResponseDescription bKashMsg, mw.CBapprovalCode bKashCode, convert(varchar, mw.RequestTime, 20)RecvTime, convert(varchar, mw.MTBProcessTime, 20)ProcessTime, convert(varchar, mw.CBReceiveTime, 20) BkashResp,  tr.CBSValueDate "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransferArchive] mw left join [RemittanceDB].[dbo].[Transactions] tr ON mw.TranTxnId=tr.SessionId  where [TranTxnId] LIKE '" + refno + "%'";

                }
                else if (paymentMode.Equals("BKASHDIRECT"))
                {
                    query = "select ID, (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=ds.PartyID) Exch, ds.TxnId RefNo, ds.ResponseDescription Status, "
                    + " ds.BeneficiaryWalletNumber AccNo, round(CONVERT(float, ds.Amount),2) Amount, ds.ProcessedTimestamp RequestTime, ds.ApprovalCode BkashAppr,"
                    + " ds.BeneficiaryName, ds.SenderName, ds.SourceCountryCode SrcCountry, case when ds.StatusID=2 then 'Processed' else 'Pending' end ProcStats "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds WHERE ds.TxnId = '" + refno + "'";
                }
                else   // cash
                {
                    query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=b.PartyId) Exch, ltrim(rtrim(b.RemitenceId)) RefNo, "
                        + " case when b.Status=1 then 'Received' when b.Status=2 then 'Canceled' when b.Status=3 then 'Queued' when b.Status=4 then 'Uploded' "
                        + " when b.Status=5 then 'Success' when b.Status=6 then 'Returned' when b.Status=7 then 'Ready for Disburse' when b.Status=8 then 'Disbursed' "
                        + " when b.Status=9 then 'Stopped' else '' end Status, b.PinNo TxnCode, b.SenderName, b.BeneficiaryName, b.Amount, convert(varchar, b.RequestTime, 20) RequestTime, "
                        + " convert(varchar, b.ClearingDate, 20) ProcessDate, b.ClearingUser ProcessUser "
                        + " FROM [RemittanceDB].[dbo].[Remittanceinfo] b WHERE b.RemitenceId='" + refno + "'";
                }

                if (connDRSystem.State.Equals(ConnectionState.Open))
                {
                    sdaExchs = new SqlDataAdapter(query, connDRSystem);
                    sdaExchs.Fill(dt);
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;

        }

        internal DataTable GetBEFTNTxnByRefNo(string refNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }


                string query = "select (select u.UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId= b.[PartyId]) Exch,[RefNo], "
                        + " case when PaymentStatus=1 then 'Received' "
                        + " when PaymentStatus=2 then 'Canceled' "
                        + " when PaymentStatus=3 then 'Queued' "
                        + " when PaymentStatus=4 then 'Uploded' "
                        + " when PaymentStatus=5 then 'Success'	"
                        + " when PaymentStatus=6 then 'Returned' "
                        + " when PaymentStatus=7 then 'Ready for Disburse' "
                        + " when PaymentStatus=8 then 'Disbursed' "
                        + " when PaymentStatus=9 then 'Stopped' "
                        + " else '' end Status, [SenderName],[SenderAddress],[BeneficiaryName],[BeneficiaryAddress],[BeneficiaryAccountNo] AccountNo,[BeneficiaryBankName] BankName, "
                        + " [BeneficiaryBankBranchName] BranchName,[DestinationRoutingNO] RoutingNo,[Amount], "
                        + " case when [IsPaymentSuccess]=1 then 'yes' else 'no' end IsSuccess,[UplodedBy], "
                        + " REPLACE(CONVERT(CHAR(11), [RequestTime], 106),' ','-')+' '+convert(varchar, [RequestTime], 108)[RequestTime], "
                        + " REPLACE(CONVERT(CHAR(11), [UploadTime], 106),' ','-')+' '+convert(varchar, [UploadTime], 108)[UploadTime], "
                        + " REPLACE(CONVERT(CHAR(11), [LastProcessingTime], 106),' ','-')+' '+convert(varchar, [LastProcessingTime], 108) ProcessTime "
                        + " ,REPLACE(CONVERT(CHAR(11), [ReturnedTime], 106),' ','-') ReturnTime, ReturnedReason "
                        + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b where RefNo='" + refNo + "'";

                SqlDataAdapter sdaExchs = new SqlDataAdapter(query, connDRSystem);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBEFTNCompanyName()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select ID, COMPANYNAME from [RemittanceDB].[dbo].[BEFTNCompanyName] order by ID DESC";
                SqlDataAdapter sdaBEFTNCompany = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNCompany.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable SearchCompanyNameByInput(string name)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }
            
                string query = "SELECT ID, COMPANYNAME from [RemittanceDB].[dbo].[BEFTNCompanyName] where upper(COMPANYNAME) like upper('%" + name + "%')";
                SqlDataAdapter sdaComp = new SqlDataAdapter(query, connDRSystem);
                sdaComp.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool SaveNewCompanyName(string compName)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                compName = compName.ToUpper();
                
                string insStmt = "IF NOT EXISTS (SELECT * FROM [RemittanceDB].[dbo].[BEFTNCompanyName]  WHERE upper([COMPANYNAME]) LIKE '%" + compName + "%') "
                + " BEGIN  INSERT INTO [RemittanceDB].[dbo].[BEFTNCompanyName] ([COMPANYNAME])  VALUES ('" + compName + "')  END ";
                SqlCommand _cmd = new SqlCommand(insStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();                                
            }
            catch (Exception exc) { }
            return true;
        }


        #region CompanyAccountNo

        internal DataTable GetBEFTNCompanyAccountNo()
        {
            SqlConnection connDRSystem = new SqlConnection(connInfo.getConnStringDR());
            DataTable dt = new DataTable();
            try
            {
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT ID, [CompanyAccountNo], convert(varchar, [CREATEDATE], 20)CREATE_DATE FROM [RemittanceDB].[dbo].[BEFTNCompanyAccountNo] order by ID DESC";
                SqlDataAdapter sdaBEFTNCompAc = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNCompAc.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable SearchCompanyAccountByInput(string acNo)
        {
            SqlConnection connDRSystem = new SqlConnection(connInfo.getConnStringDR());
            DataTable dt = new DataTable();
            try
            {                
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT ID, CompanyAccountNo from [RemittanceDB].[dbo].[BEFTNCompanyAccountNo] where upper(CompanyAccountNo) like upper('%" + acNo + "%')";
                SqlDataAdapter sdaComp = new SqlDataAdapter(query, connDRSystem);
                sdaComp.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool SaveNewCompanyAccountNo(string compAcNo)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                compAcNo = compAcNo.ToUpper();

                string insStmt = "IF NOT EXISTS (SELECT * FROM [RemittanceDB].[dbo].[BEFTNCompanyAccountNo]  WHERE upper([CompanyAccountNo]) LIKE '%" + compAcNo + "%') "
                + " BEGIN  INSERT INTO [RemittanceDB].[dbo].[BEFTNCompanyAccountNo] ([CompanyAccountNo])  VALUES ('" + compAcNo + "')  END ";
                SqlCommand _cmd = new SqlCommand(insStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            return true;
        }

        internal bool RemoveCompanyAccountById(int compId)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string delStmt = "DELETE FROM [RemittanceDB].[dbo].[BEFTNCompanyName] WHERE [ID]=" + compId + "";
                SqlCommand _cmd = new SqlCommand(delStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }
        #endregion



        internal DataTable GetBEFTNTxnPendingToProcess()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }


                string query = "SELECT PartyId, (select u.UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId=b.PartyId) Exh, count(*) COUNT, case when b.IsIncentive=0 then 'MainTxn' else 'Inctv Txn' end TxnType "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b WHERE PaymentStatus=1 GROUP BY PartyId, IsIncentive  ORDER BY IsIncentive, Exh ";

                SqlDataAdapter sdaBEFTNTxnPending = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNTxnPending.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal string GetBEFTNTxnPendingToProcessCount()
        {
            SqlConnection connDRSystem = null;
            string pendingTxn = "";
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }


                string query = "SELECT count(*) FROM [RemittanceDB].[dbo].BEFTNRequest b WHERE PaymentStatus=1 and IsIncentive in(0,99)";
                SqlDataAdapter sdaBEFTNTxnPendingCount = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNTxnPendingCount.Fill(dt);

                pendingTxn = "Main: " + Convert.ToInt32(dt.Rows[0][0]).ToString();

                dt = new DataTable();
                query = "SELECT count(*) FROM [RemittanceDB].[dbo].BEFTNRequest b WHERE PaymentStatus=1 and IsIncentive=1";
                sdaBEFTNTxnPendingCount = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNTxnPendingCount.Fill(dt);

                pendingTxn += " , Incentive: " + Convert.ToInt32(dt.Rows[0][0]).ToString();
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return pendingTxn;
        }

        internal DataTable GetBEFTNOnProcessing(ref string onProcessingCount)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            DataTable dtCount = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (select u.UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId=b.PartyId ) Exh, b.RefNo, b.Amount, convert(varchar, b.RequestTime, 20)RequestTime"
                    + " , case when b.IsIncentive=0 then 'MainTxn' else 'Inctv Txn' end TxnType "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b "
                    + " WHERE PaymentStatus=3";
                SqlDataAdapter sdaBEFTNOnProcess = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNOnProcess.Fill(dt);


                query = "SELECT count(*) FROM [RemittanceDB].[dbo].[BEFTNRequest] b WHERE PaymentStatus=3 AND IsIncentive in(0,99)";
                sdaBEFTNOnProcess = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNOnProcess.Fill(dtCount);
                onProcessingCount = "Main: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();

                dtCount = new DataTable();
                query = "SELECT count(*) FROM [RemittanceDB].[dbo].[BEFTNRequest] b WHERE PaymentStatus=3 AND IsIncentive=1";
                sdaBEFTNOnProcess = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNOnProcess.Fill(dtCount);
                onProcessingCount += ", Incentive: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBEFTNOnProcessingForDownload()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (select u.UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId=b.PartyId ) Exh, b.RefNo, b.Amount, convert(varchar, b.RequestTime, 20)RequestTime "
                    + " , case when b.IsIncentive=0 then 'MainTxn' else 'IncentiveTxn' end TxnType , b.BeneficiaryAccountNo, "
                    + " case when b.IsIncentive=0 then (select replace(u.AccountNo,'-','') from [RemittanceDB].[dbo].Users u where u.PartyId=b.PartyId ) else '" + CConstants.BB_IMPREST_GL + "' end DebitAcNo "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b WHERE PaymentStatus=3";
                
                SqlDataAdapter sdaBEFTNOnProcess = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNOnProcess.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBEFTNTxnAutoProcessed(string fromdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT TOP 200 [BEFTNRequestId] ReqId,[ReferenceNo],(select u.UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId=bt.PartyId ) Exhouse, "
                    + " cast([Amount] as decimal(10,2))Amount, convert(varchar, [TransactionDate], 20) TransactionDate, [Status] "
                    + " , (SELECT top 1 BES.Remarks FROM [RemittanceDB].[dbo].BEFTNExecutionStatus BES where BES.BEFTNRequestId=bt.BEFTNRequestId)Remarks "
                    + " , case when bt.Reason like '%Regular%' then 'MainTxn' else 'Inctv Txn' end TxnType "
                    + " FROM [RemittanceDB].[dbo].[BEFTNTransaction] bt "
                    + " WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' "
                    + " ORDER BY [ID] DESC";

                SqlDataAdapter sdaBEFTNTxnAutoProcessed = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNTxnAutoProcessed.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBEFTNTxnAutoProcessedForDownload(string fromdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [BEFTNRequestId] ReqId,[ReferenceNo],(select u.UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId=bt.PartyId ) Exhouse, "
                    + " cast([Amount] as decimal(10,2))Amount, convert(varchar, [TransactionDate], 20) TransactionDate, [Status] "
                    + " , (SELECT top 1 BES.Remarks FROM [RemittanceDB].[dbo].BEFTNExecutionStatus BES where BES.BEFTNRequestId=bt.BEFTNRequestId)Remarks "
                    + " , case when bt.Reason like '%Regular%' then 'MainTxn' else 'Inctv Txn' end TxnType "
                    + " FROM [RemittanceDB].[dbo].[BEFTNTransaction] bt "
                    + " WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' "
                    + " ORDER BY [ID] DESC";

                SqlDataAdapter sdaBEFTNTxnAutoProcessed = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNTxnAutoProcessed.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetAutoProcessSuccessFailCount(string fromdt, string todt, ref string successCount, ref string failCount)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            DataTable dtCount = new DataTable();
            string query = "";
            SqlDataAdapter sdaSuccessFailCount;

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }
                            
                query = "SELECT count([Status]) FROM [RemittanceDB].[dbo].[BEFTNTransaction] bt WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' AND bt.Reason LIKE '%Regular%' AND [Status]='Success'";
                sdaSuccessFailCount = new SqlDataAdapter(query, connDRSystem);
                sdaSuccessFailCount.Fill(dtCount);
                successCount = "Success >> Main: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();

                dtCount = new DataTable();
                query = "SELECT count([Status]) FROM [RemittanceDB].[dbo].[BEFTNTransaction] bt WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' AND bt.Reason NOT LIKE '%Regular%' AND [Status]='Success'";
                sdaSuccessFailCount = new SqlDataAdapter(query, connDRSystem);
                sdaSuccessFailCount.Fill(dtCount);
                successCount += " , Incentive: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();

                //-------------------
                dtCount = new DataTable();
                //query = "SELECT count([Status]) FROM [RemittanceDB].[dbo].[BEFTNTransaction] bt WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' AND bt.Reason like '%Regular%' AND [Status]<>'Success'";

                query = "SELECT count([Status]) FROM [RemittanceDB].[dbo].[View_BEFTNFailedReport] fr  WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' AND fr.Reason LIKE '%Regular%'";
                sdaSuccessFailCount = new SqlDataAdapter(query, connDRSystem);
                sdaSuccessFailCount.Fill(dtCount);
                failCount = "Failed >> Main: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();                              

                dtCount = new DataTable();
                query = "SELECT count([Status]) FROM [RemittanceDB].[dbo].[View_BEFTNFailedReport] fr WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' AND fr.Reason NOT LIKE '%Regular%' ";
                sdaSuccessFailCount = new SqlDataAdapter(query, connDRSystem);
                sdaSuccessFailCount.Fill(dtCount);
                failCount += ", Incentive: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();

            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        //internal DataTable GetBEFTNAutoProcessedFailed(string fromdt, string todt)
        //{
        //    SqlConnection connDRSystem = null;
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        connDRSystem = new SqlConnection(connInfo.getConnStringDR());
        //        connDRSystem.Open();
        //    }
        //    catch (Exception exc) { }

        //    string query = "SELECT [UserId] Exh,[ReferenceNo],[Amount],[CrAccountNo] ExhAcc, convert(varchar, [TransactionDate], 20)TransDate,[BeneficiaryName],[Status],[Remarks], /* [TransactionId], */ [BEFTNRequestId] EFTReqId "
        //        + " FROM [RemittanceDB].[dbo].[View_BEFTNFailedReport] "
        //        + " WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' "
        //        + " ORDER BY TransactionDate DESC";

        //    SqlDataAdapter sdaBEFTNAutoProcessedFailed = new SqlDataAdapter(query, connDRSystem);
        //    sdaBEFTNAutoProcessedFailed.Fill(dt);
        //    return dt;
        //}

        internal DataTable GetBEFTNAutoProcessedFailedWithErrorResp(string fromdt, string todt, ref string failCount)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            DataTable dtCount = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [UserId] Exh,[ReferenceNo],cast(CONVERT(float, [Amount]) as numeric(10,2))Amount,[CrAccountNo] BeneAcc, convert(varchar, [TransactionDate], 20)TransDate,[BeneficiaryName],[Status],[Remarks], "
                    + " case when fr.Reason like '%Regular%' then 'MainTxn' else 'Inctv Txn' end TxnType "
                    + " , (SELECT top 1 bt.[ResponseData] FROM [RemittanceDB].[dbo].[BEFTNTransaction] bt WHERE BEFTNRequestId =fr.[BEFTNRequestId]) RespData "
                    + " FROM [RemittanceDB].[dbo].[View_BEFTNFailedReport] fr "
                    + " WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' "
                    + " ORDER BY TransactionDate DESC";

                SqlDataAdapter sdaBEFTNAutoProcessedFailed = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNAutoProcessedFailed.Fill(dt);

                dtCount = new DataTable();                
                query = "SELECT count([Status]) FROM [RemittanceDB].[dbo].[View_BEFTNFailedReport] fr  WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' AND fr.Reason LIKE '%Regular%'";
                SqlDataAdapter sdaFailCount = new SqlDataAdapter(query, connDRSystem);
                sdaFailCount.Fill(dtCount);
                failCount = "Failed >> Main: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();

                dtCount = new DataTable();
                query = "SELECT count([Status]) FROM [RemittanceDB].[dbo].[View_BEFTNFailedReport] fr WHERE convert(varchar, [TransactionDate], 23) between '" + fromdt + "' AND '" + todt + "' AND fr.Reason NOT LIKE '%Regular%' ";
                sdaFailCount = new SqlDataAdapter(query, connDRSystem);
                sdaFailCount.Fill(dtCount);
                failCount += ", Incentive: " + Convert.ToInt32(dtCount.Rows[0][0]).ToString();
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }



        internal DataTable GetInvalidEFTRoutingList()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                //string query = "Select (select userid from Users u where u.PartyId=BR.PartyId) Exch, BR.RefNo, BR.BeneficiaryName Beneficiary, BR.BeneficiaryAccountNo AccountNo, BR.BeneficiaryBankName BankName, BR.BeneficiaryBankBranchName BranchName, BR.Amount, BR.RequestTime, BR.DestinationRoutingNO RoutingNo "
                //    + " FROM [RemittanceDB].[dbo].BEFTNRequest BR "
                //    + " where  BR.PaymentStatus=1  and len(LTRIM(RTRIM(BR.DestinationRoutingNO)))<>9";

                // changed: 11-Jun-2022 , included NBL invalid list

                string query = "Select (SELECT userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=BR.PartyId) Exch, BR.RefNo, BR.BeneficiaryName Beneficiary, BR.BeneficiaryAccountNo AccountNo, BR.BeneficiaryBankName BankName, BR.BeneficiaryBankBranchName BranchName, BR.Amount, BR.RequestTime, BR.DestinationRoutingNO RoutingNo "
                    + " FROM [RemittanceDB].[dbo].BEFTNRequest BR "
                    + " where  BR.PaymentStatus=1  and len(LTRIM(RTRIM(BR.DestinationRoutingNO)))<>9 "
                    + " UNION ALL "
                    + " select case when be.AGENT_CODE ='065' then 'NBLSingapore' else 'NBLMalaysia' end Exch, be.REFERENCE RefNo,  be.BENEFICIARY_NAME Beneficiary, be.ACCOUNT_NO AccountNo, be.BANK_NAME BankName, be.BRANCH_NAME BranchName, be.AMOUNT Amount, be.TXN_RECEIVE_DATE RequestTime, be.ROUTING_NUMBER RoutingNo "
                    + " FROM [RemittanceDB].[dbo].[NBLRequestData] be "
                    + " where be.PAYMENT_MODE='BEFTN' and be.TXN_STATUS='RECEIVED' and len(LTRIM(RTRIM(be.ROUTING_NUMBER)))<>9";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaBEFTN = new SqlDataAdapter(cmd);
                sdaBEFTN.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }
                
        internal bool UpdateInvalidRoutingWithCorrectRoutingNumber(string pinNo, string routingNo)
        {
            SqlConnection connNewSystem = null;
            int rowsAffected = 0;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }


                string updStmt = "UPDATE [RemittanceDB].[dbo].[BEFTNRequest] SET [DestinationRoutingNO]=@RoutingNO "
                + " WHERE [RefNo]=@RefNo AND [PaymentStatus] in(1,7)";

                SqlCommand _cmd = new SqlCommand(updStmt, connNewSystem);

                _cmd.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = pinNo.Trim();
                _cmd.Parameters.Add("@RoutingNO", SqlDbType.VarChar).Value = routingNo;

                try
                {
                    rowsAffected = _cmd.ExecuteNonQuery();
                }
                catch (Exception exc) { return false; }
                _cmd.Dispose();

                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
            }
            return false;
        }

        internal bool UpdateNBLInvalidRoutingWithCorrectRoutingNumber(string pinNo, string routingNo)
        {
            SqlConnection connNewSystem = null;
            int rowsAffected = 0;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                string updStmt = "UPDATE [RemittanceDB].[dbo].[NBLRequestData] SET [ROUTING_NUMBER]=@RoutingNO "
                + " WHERE [REFERENCE]=@RefNo AND PAYMENT_MODE='BEFTN' and TXN_STATUS='RECEIVED'";

                SqlCommand _cmd = new SqlCommand(updStmt, connNewSystem);

                _cmd.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = pinNo.Trim();
                _cmd.Parameters.Add("@RoutingNO", SqlDbType.VarChar).Value = routingNo;

                try
                {
                    rowsAffected = _cmd.ExecuteNonQuery();
                }
                catch (Exception exc) { return false; }
                _cmd.Dispose();

                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
            }
            return false;
        }

        internal void InsertAutoFetchLog(string userId, string methodName, string responseMessage)
        {
            SqlConnection connNewSystem = null;
            int rowsAffected = 0;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }
                
                string query = "INSERT INTO [RemittanceDB].[dbo].[APIDataAutoFetchLog]([UserId],[MethodName],[ResponseMessage]) VALUES('" + userId + "', '" + methodName + "', '" + responseMessage + "')";
                SqlCommand _cmd = new SqlCommand(query, connNewSystem);

                try
                {
                    rowsAffected = _cmd.ExecuteNonQuery();
                }
                catch (Exception exc) { }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)){ connNewSystem.Close(); }
            }
        }

        //GCC
        internal bool InsertIntoGCCDataTable(string userId, GCCServiceClient.CashTxnResponse cashDetails, string downloadBranch, string downloadUser)
        {
            bool stats = true;
            SqlConnection openCon = null;

            try
            {
                openCon = new SqlConnection(connInfo.getConnStringRemitLv());
                SqlCommand cmdSaveAcData = new SqlCommand();

                if (openCon.State.Equals(ConnectionState.Closed)){  openCon.Open();  }

                cmdSaveAcData.CommandType = CommandType.StoredProcedure;
                cmdSaveAcData.CommandText = "GCCSpInsertAccountAndCashTxnData";
                cmdSaveAcData.Connection = openCon;

                cmdSaveAcData.Parameters.Add("@TransactionNo", SqlDbType.VarChar).Value = cashDetails.TransactionNo.Trim();
                cmdSaveAcData.Parameters.Add("@AmountToPay", SqlDbType.Float).Value = Math.Round(Convert.ToDouble(cashDetails.AmountToPay), 2);
                cmdSaveAcData.Parameters.Add("@ResponseCode", SqlDbType.VarChar).Value = cashDetails.ResponseCode == null ? "" : cashDetails.ResponseCode.Trim();
                cmdSaveAcData.Parameters.Add("@ResponseMessage", SqlDbType.VarChar).Value = cashDetails.ResponseMessage == null ? "" : cashDetails.ResponseMessage.Trim();
                cmdSaveAcData.Parameters.Add("@Status", SqlDbType.VarChar).Value = cashDetails.Status == null ? "" : cashDetails.Status.Trim();
                cmdSaveAcData.Parameters.Add("@Successful", SqlDbType.VarChar).Value = cashDetails.Successful == null ? "" : cashDetails.Successful.Trim();
                cmdSaveAcData.Parameters.Add("@TransactionDate", SqlDbType.VarChar).Value = cashDetails.TransactionDate == null ? "" : cashDetails.TransactionDate.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiveCountryCode", SqlDbType.VarChar).Value = cashDetails.ReceiveCountryCode == null ? "" : cashDetails.ReceiveCountryCode.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiveCountryName", SqlDbType.VarChar).Value = cashDetails.ReceiveCountryName == null ? "" : cashDetails.ReceiveCountryName.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiveCurrencyCode", SqlDbType.VarChar).Value = cashDetails.ReceiveCurrencyCode == null ? "" : cashDetails.ReceiveCurrencyCode.Trim();
                cmdSaveAcData.Parameters.Add("@PurposeName", SqlDbType.VarChar).Value = cashDetails.PurposeName == null ? "" : cashDetails.PurposeName.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiverName", SqlDbType.VarChar).Value = cashDetails.ReceiverFirstName.Trim() + " " + cashDetails.ReceiverLastName.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiverNationality", SqlDbType.VarChar).Value = cashDetails.ReceiverNationality == null ? "" : cashDetails.ReceiverNationality.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiverAddress", SqlDbType.VarChar).Value = cashDetails.ReceiverAddress == null ? "" : cashDetails.ReceiverAddress.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiverCity", SqlDbType.VarChar).Value = cashDetails.ReceiverCity == null ? "" : cashDetails.ReceiverCity.Trim();
                cmdSaveAcData.Parameters.Add("@ReceiverContactNo", SqlDbType.VarChar).Value = cashDetails.ReceiverContactNo == null ? "" : cashDetails.ReceiverContactNo.Trim();
                cmdSaveAcData.Parameters.Add("@SenderAddress", SqlDbType.VarChar).Value = cashDetails.SenderAddress == null ? "" : cashDetails.SenderAddress.Trim();
                cmdSaveAcData.Parameters.Add("@SendCountryCode", SqlDbType.VarChar).Value = cashDetails.SendCountryCode == null ? "" : cashDetails.SendCountryCode.Trim();
                cmdSaveAcData.Parameters.Add("@SendCountryName", SqlDbType.VarChar).Value = cashDetails.SendCountryName == null ? "" : cashDetails.SendCountryName.Trim();
                cmdSaveAcData.Parameters.Add("@SenderCity", SqlDbType.VarChar).Value = cashDetails.SenderCity == null ? "" : cashDetails.SenderCity.Trim();
                cmdSaveAcData.Parameters.Add("@SenderContactNo", SqlDbType.VarChar).Value = cashDetails.SenderContactNo == null ? "" : cashDetails.SenderContactNo.Trim();
                cmdSaveAcData.Parameters.Add("@SenderName", SqlDbType.VarChar).Value = cashDetails.SenderFirstName.Trim() + " " + cashDetails.SenderLastName.Trim();
                cmdSaveAcData.Parameters.Add("@SenderNationality", SqlDbType.VarChar).Value = cashDetails.SenderNationality == null ? "" : cashDetails.SenderNationality.Trim();
                cmdSaveAcData.Parameters.Add("@SenderIncomeSource", SqlDbType.VarChar).Value = cashDetails.SenderIncomeSourceName == null ? "" : cashDetails.SenderIncomeSourceName.Trim();
                cmdSaveAcData.Parameters.Add("@SenderOccupation", SqlDbType.VarChar).Value = cashDetails.SenderOccupationName == null ? "" : cashDetails.SenderOccupationName.Trim();
                cmdSaveAcData.Parameters.Add("@SenderIDExpiryDate", SqlDbType.VarChar).Value = cashDetails.SenderIDExpiryDate == null ? "" : cashDetails.SenderIDExpiryDate.Trim();
                cmdSaveAcData.Parameters.Add("@SenderIDNumber", SqlDbType.VarChar).Value = cashDetails.SenderIDNumber == null ? "" : cashDetails.SenderIDNumber.Trim();
                cmdSaveAcData.Parameters.Add("@SenderIDPlaceOfIssue", SqlDbType.VarChar).Value = cashDetails.SenderIDPlaceOfIssue == null ? "" : cashDetails.SenderIDPlaceOfIssue.Trim();
                cmdSaveAcData.Parameters.Add("@SenderIDTypeName", SqlDbType.VarChar).Value = cashDetails.SenderIDTypeName == null ? "" : cashDetails.SenderIDTypeName.Trim();
                cmdSaveAcData.Parameters.Add("@TxnReceiveDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmdSaveAcData.Parameters.Add("@TxnStatus", SqlDbType.VarChar).Value = "RECEIVED";

                cmdSaveAcData.Parameters.Add("@BankAccountNo", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@BankName", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@BankBranchName", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@BankBranchCode", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@SentDate", SqlDbType.DateTime).Value = Convert.ToDateTime(cashDetails.TransactionDate);
                cmdSaveAcData.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = Convert.ToDateTime(cashDetails.TransactionDate);
                cmdSaveAcData.Parameters.Add("@PayinCurrencyCode", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@PayinAmount", SqlDbType.Float).Value = 0;
                cmdSaveAcData.Parameters.Add("@ExchangeRate", SqlDbType.Float).Value = 0;

                cmdSaveAcData.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = "CASH";
                cmdSaveAcData.Parameters.Add("@DownloadBranch", SqlDbType.VarChar).Value = downloadBranch;
                cmdSaveAcData.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@GccPayMode", SqlDbType.VarChar).Value = "COC";

                try
                {
                    int k = cmdSaveAcData.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    InsertAutoFetchLog(userId, "InsertIntoGCCDataTable", "Error ! InsertIntoGCCDataTable" + ex.ToString());
                    stats = false;
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (openCon != null && openCon.State.Equals(ConnectionState.Open)) { openCon.Close(); }
            }
            return stats;
        }

        //GCC
        internal void MoveGCCDataIntoRemitInfoTable(string txnNum, int idType, string idNumber, string mobileNum, string kycAddrs, string downloadUser)
        {
            try
            {
                SqlConnection connNewSystem = null;
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                SqlCommand cmdSaveData = new SqlCommand();

                if (connNewSystem.State.Equals(ConnectionState.Closed))
                {
                    connNewSystem.Open();
                }

                cmdSaveData.CommandType = CommandType.StoredProcedure;
                cmdSaveData.CommandText = "GCCSpCashDataMoveToRemitInfoTable";
                cmdSaveData.Connection = connNewSystem;

                cmdSaveData.Parameters.Add("@reference", SqlDbType.VarChar).Value = txnNum;
                cmdSaveData.Parameters.Add("@idType", SqlDbType.Int).Value = idType;
                cmdSaveData.Parameters.Add("@idNumber", SqlDbType.VarChar).Value = idNumber;
                cmdSaveData.Parameters.Add("@mobileNumber", SqlDbType.VarChar).Value = mobileNum;
                cmdSaveData.Parameters.Add("@kycAddress", SqlDbType.VarChar).Value = kycAddrs;
                cmdSaveData.Parameters.Add("@downloadUser", SqlDbType.VarChar).Value = downloadUser;

                try
                {
                    int k = cmdSaveData.ExecuteNonQuery();
                }
                catch (Exception ex) { }
                finally
                {
                    if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
                }
            }
            catch (Exception exc) { }
        }

        internal void UpdateTxnStatusIntoGCCTable(string TransactionNo, string confirmFlag, string remarks, string downloadUser)
        {
            SqlConnection connNewSystem = null;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }


                string query = "";
                if (confirmFlag.Equals("C"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[GCCRequestData] SET [TxnStatus]='PAID', [TxnPaymentDate]=getdate(), [ClearingUser]='" + downloadUser + "', [ClearingDate]=getdate()  WHERE [TransactionNo]='" + TransactionNo + "'";                    
                }

                if (confirmFlag.Equals("Processed"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[GCCRequestData] SET [Status]='" + confirmFlag + "', [TxnStatus]='PROCESSED'  WHERE [TransactionNo]='" + TransactionNo + "'";                    
                }
                else if (confirmFlag.Equals("Paid"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[GCCRequestData] SET [Status]='" + confirmFlag + "', [TxnStatus]='PAID',  [TxnPaymentDate]=getdate(), [ClearingUser]='" + downloadUser + "', [ClearingDate]=getdate()  WHERE [TransactionNo]='" + TransactionNo + "'";
                }


                SqlCommand _cmd = new SqlCommand(query, connNewSystem);
                try
                {
                    int k = _cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { }

            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)){  connNewSystem.Close();   }
            }
        }


        internal DataTable GetBdAllBanklist()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT distinct [Bank Code],[Agent Name] FROM [RemittanceDB].[dbo].[BANK_BRANCH] ORDER BY [Agent Name]";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaBank = new SqlDataAdapter(cmd);
                sdaBank.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)){ connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetDistrictListByBankCode(string bankCode)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT distinct [District] FROM [RemittanceDB].[dbo].[BANK_BRANCH] WHERE [Bank Code]='" + bankCode + "' order by [District]";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaDist = new SqlDataAdapter(cmd);
                sdaDist.Fill(dt);
            }
            catch (Exception exc) { }
            finally{  if (connDRSystem.State.Equals(ConnectionState.Open)){  connDRSystem.Close();  }  }
            return dt;
        }

        internal DataTable GetBranchListByBankCodeAndDistrictName(string bankCode, string distName)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [Branch Name] FROM [RemittanceDB].[dbo].[BANK_BRANCH] WHERE [Bank Code]='" + bankCode + "' and [District]='" + distName + "' order by [Branch Name]";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaDist = new SqlDataAdapter(cmd);
                sdaDist.Fill(dt);
            }
            catch (Exception exc) { }
            finally{  if (connDRSystem.State.Equals(ConnectionState.Open)){  connDRSystem.Close(); }   }
            return dt;
        }

        internal DataTable GetRoutingInfosByWhereClause(string whereClause)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [MTB Sl No] Sl,[Bank Code] BankCode,[Agent Name] BankName,[Branch Name] BranchName,[District],[Routing Number] Routing,[IsActiveForBEFTNAP] IsActive FROM [RemittanceDB].[dbo].[BANK_BRANCH] " + whereClause + " ORDER BY BankName, BranchName, District";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaDist = new SqlDataAdapter(cmd);
                sdaDist.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open))
                {
                    connDRSystem.Close();
                }
            }
            return dt;
        }

        internal bool UpdateRoutingInfo(int idVal, string rtNum, string brName, string bankCode, string bankName, string dist)
        {
            SqlConnection connNewSystem = null;
            SqlCommand cmdUpdateData = new SqlCommand();
            string updateData = "";
            bool updateSuccess = false;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                updateData = "UPDATE [RemittanceDB].[dbo].[BANK_BRANCH] SET [Routing Number]=@Routing_No, [Branch Name]=@brnch, [Bank Code]=@bnkCode, [Agent Name]=@bnkName, [City Name]=@dist, [District]=@dist WHERE [MTB Sl No] = @AutoId ";

                cmdUpdateData.CommandText = updateData;
                cmdUpdateData.Connection = connNewSystem;
                cmdUpdateData.Parameters.Add("@Routing_No", SqlDbType.VarChar).Value = rtNum;
                cmdUpdateData.Parameters.Add("@brnch", SqlDbType.VarChar).Value = brName;
                cmdUpdateData.Parameters.Add("@AutoId", SqlDbType.Int).Value = idVal;

                cmdUpdateData.Parameters.Add("@bnkCode", SqlDbType.VarChar).Value = bankCode;
                cmdUpdateData.Parameters.Add("@bnkName", SqlDbType.VarChar).Value = bankName.ToUpper().Trim();
                cmdUpdateData.Parameters.Add("@dist", SqlDbType.VarChar).Value = dist.ToUpper().Trim();

                try
                {
                    cmdUpdateData.ExecuteNonQuery();
                    updateSuccess = true;
                }
                catch (Exception ec)
                {
                    updateSuccess = false;
                    throw ec;
                }
            }
            catch (Exception exc)
            {
                updateSuccess = false;
                throw exc;
            }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return updateSuccess;
        }


        internal DataTable GetBdAllDistrictlist()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT distinct [District] FROM [RemittanceDB].[dbo].[BANK_BRANCH] ORDER BY [District]";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaDist = new SqlDataAdapter(cmd);
                sdaDist.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();   }
            }
            return dt;
        }

        internal bool IsRoutingNumberAlreadyExists(string routingNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT * FROM [BANK_BRANCH] WHERE [Routing Number]='" + routingNo + "' ";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaRt = new SqlDataAdapter(cmd);
                sdaRt.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();   }
            }
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        internal int GetLastRecordNumber()
        {
            SqlConnection connNewSystem = null;
            DataTable dt = new DataTable();
            int slNo = 0;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                string query = "SELECT max(CAST([MTB Sl No] AS int)) FROM [RemittanceDB].[dbo].[BANK_BRANCH]";
                SqlCommand cmd = new SqlCommand(query, connNewSystem);
                SqlDataAdapter sdaRecNum = new SqlDataAdapter(cmd);
                sdaRecNum.Fill(dt);
                slNo = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception exc)
            {
            }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return slNo;
        }

        internal bool SaveRoutingInfo(int slNo, string bankCode, string bankName, string brName, string districtName, string rtNum)
        {
            SqlConnection connNewSystem = null;
            SqlCommand cmdInsertData = new SqlCommand();
            string insertQry = "";
            bool insertSuccess = false;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                insertQry = "INSERT INTO [dbo].[BANK_BRANCH]([MTB Sl No],[MTB Code],[Bank Code],[Agent Name],[Branch Name],[City Name],[District],[Routing Number],[Country],[Contact Details],[IsActiveForBEFTNAP]) "
                    + " VALUES (@MTBSlNo,@MTBCode,@BankCode,@BankName,@BranchName,@City, @District,@RoutingNumber,@Country,@ContactDetails,@IsActiveForBEFTNAP)";

                cmdInsertData.CommandText = insertQry;
                cmdInsertData.Connection = connNewSystem;
                cmdInsertData.Parameters.Add("@MTBSlNo", SqlDbType.VarChar).Value = slNo.ToString();
                cmdInsertData.Parameters.Add("@MTBCode", SqlDbType.VarChar).Value = slNo.ToString();
                cmdInsertData.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = bankCode;
                cmdInsertData.Parameters.Add("@BankName", SqlDbType.VarChar).Value = bankName;
                cmdInsertData.Parameters.Add("@BranchName", SqlDbType.VarChar).Value = brName;
                cmdInsertData.Parameters.Add("@City", SqlDbType.VarChar).Value = districtName;
                cmdInsertData.Parameters.Add("@District", SqlDbType.VarChar).Value = districtName;
                cmdInsertData.Parameters.Add("@RoutingNumber", SqlDbType.VarChar).Value = rtNum;
                cmdInsertData.Parameters.Add("@Country", SqlDbType.VarChar).Value = "Bangladesh";
                cmdInsertData.Parameters.Add("@ContactDetails", SqlDbType.VarChar).Value = "mtbremittance@mutualtrustbank.com";
                cmdInsertData.Parameters.Add("@IsActiveForBEFTNAP", SqlDbType.Bit).Value = 1;

                try
                {
                    cmdInsertData.ExecuteNonQuery();
                    insertSuccess = true;
                }
                catch (Exception ec)
                {
                    insertSuccess = false;
                    throw ec;
                }
            }
            catch (Exception exc)
            {
                insertSuccess = false;
                throw exc;
            }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return insertSuccess;
        }

        internal int BEFTNMarkedSuccessManually(string pinNo, string remarks, string userId)
        {
            SqlConnection connNewSystem = null;
            int rc = 0;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                int reqId = GetAutoIdsFromPinNo(pinNo);

                SqlCommand sql_cmnd = new SqlCommand("[dbo].[BEFTN_Remittance_MarkedSuccessManually]", connNewSystem);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@BEFTNRequestId", SqlDbType.Int).Value = reqId;
                sql_cmnd.Parameters.AddWithValue("@Remarks", SqlDbType.NVarChar).Value = remarks;
                sql_cmnd.Parameters.AddWithValue("@ADCAppsUser", SqlDbType.NVarChar).Value = userId;
                rc = sql_cmnd.ExecuteNonQuery();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return rc;
        }

        private int GetAutoIdsFromPinNo(string pinNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select autoid from [RemittanceDB].[dbo].[BEFTNRequest] where RefNo ='" + pinNo.Trim() + "' AND IsIncentive in(0,99) and PaymentStatus NOT IN(5,6)";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaDist = new SqlDataAdapter(cmd);
                sdaDist.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)){ connDRSystem.Close();  }
            }
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        internal int BEFTNMarkedCancelledManually(string pinNo, string remarks, string userId)
        {
            SqlConnection connNewSystem = null;
            int rc = 0;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                int reqId = GetAutoIdsFromPinNo(pinNo);

                SqlCommand sql_cmnd = new SqlCommand("[dbo].[BEFTN_Remittance_Manually_MarkedCancelled]", connNewSystem);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@BEFTNRequestId", SqlDbType.Int).Value = reqId;
                sql_cmnd.Parameters.AddWithValue("@Remarks", SqlDbType.NVarChar).Value = remarks;
                sql_cmnd.Parameters.AddWithValue("@ADCAppsUser", SqlDbType.NVarChar).Value = userId;
                rc = sql_cmnd.ExecuteNonQuery();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return rc;
        }

        internal int BEFTNMarkedCancelRippleTxnManually(string pinNo, string remarks, string userId)
        {
            SqlConnection connNewSystem = null;
            int rc = 0;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                int reqId = GetAutoIdsFromPinNo(pinNo);

                SqlCommand sql_cmnd = new SqlCommand("[dbo].[BEFTN_Remittance_Manually_MarkedCancelled]", connNewSystem);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@BEFTNRequestId", SqlDbType.Int).Value = reqId;
                sql_cmnd.Parameters.AddWithValue("@Remarks", SqlDbType.NVarChar).Value = remarks;
                sql_cmnd.Parameters.AddWithValue("@ADCAppsUser", SqlDbType.NVarChar).Value = userId;
                rc = sql_cmnd.ExecuteNonQuery();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return rc;
        }

        internal DataTable GetEFTTxnInfoByAutoId(string pinNo)
        {
            int reqId = GetAutoIdsFromPinNo(pinNo);
            DataTable dt = new DataTable();
            SqlConnection connDRSystem = null;

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT * FROM [RemittanceDB].[dbo].[BEFTNRequest] b where b.AutoId=" + reqId + " ";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaBEFTN = new SqlDataAdapter(cmd);
                sdaBEFTN.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool InsertIntoBEFTPaymentReturnTable(DataTable dtEftRecInfo, string loggedUserName)
        {
            SqlConnection connNewSystem = null;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                string insStmt = "INSERT INTO [RemittanceDB].[dbo].[BEFTNPaymentReturn]([BeftnId],[PartyId],[RefNo],[FromAccount],[ToAccountNo],[Amount],[Time],[ReturnBy]) "
                + " VALUES(@BeftnId,@PartyId,@RefNo,@FromAccount,@ToAccountNo,@Amount,@Time,@ReturnBy)";

                SqlCommand _cmd = new SqlCommand(insStmt, connNewSystem);

                _cmd.Parameters.Add("@BeftnId", SqlDbType.Int).Value = Convert.ToInt32(dtEftRecInfo.Rows[0]["AutoId"].ToString().Trim());
                _cmd.Parameters.Add("@PartyId", SqlDbType.Int).Value = Convert.ToInt32(dtEftRecInfo.Rows[0]["PartyId"].ToString().Trim());
                _cmd.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = dtEftRecInfo.Rows[0]["RefNo"].ToString().Trim();
                _cmd.Parameters.Add("@FromAccount", SqlDbType.VarChar).Value = dtEftRecInfo.Rows[0]["FromAccount"].ToString().Trim();
                _cmd.Parameters.Add("@ToAccountNo", SqlDbType.VarChar).Value = dtEftRecInfo.Rows[0]["FromAccount"].ToString().Trim();
                _cmd.Parameters.Add("@Amount", SqlDbType.Float).Value = Convert.ToSingle(dtEftRecInfo.Rows[0]["Amount"].ToString().Trim());
                _cmd.Parameters.Add("@Time", SqlDbType.DateTime).Value = DateTime.Now;
                _cmd.Parameters.Add("@ReturnBy", SqlDbType.VarChar).Value = loggedUserName;

                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }

            return true;
        }

        internal bool InsertIntoBEFTStatusLogTable(DataTable dtEftRecInfo, string loggedUserName)
        {
            SqlConnection connNewSystem = null;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                string insStmt = "INSERT INTO [RemittanceDB].[dbo].[BEFTNStatusLog]([RefNo],[ClientId],[CurrentStatus],[ChangeBy],[Time]) "
                + " VALUES(@RefNo,@ClientId,@CurrentStatus,@ChangeBy,@Time)";

                SqlCommand _cmd = new SqlCommand(insStmt, connNewSystem);

                _cmd.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = dtEftRecInfo.Rows[0]["RefNo"].ToString().Trim();
                _cmd.Parameters.Add("@ClientId", SqlDbType.Int).Value = Convert.ToInt32(dtEftRecInfo.Rows[0]["PartyId"].ToString().Trim());
                _cmd.Parameters.Add("@CurrentStatus", SqlDbType.Int).Value = 6;
                _cmd.Parameters.Add("@ChangeBy", SqlDbType.VarChar).Value = loggedUserName;
                _cmd.Parameters.Add("@Time", SqlDbType.DateTime).Value = DateTime.Now;

                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }

            return true;
        }

        internal bool UpdateBEFTStatus(DataTable dtEftRecInfo, string loggedUserName)
        {
            SqlConnection connNewSystem = null;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                string insStmt = "UPDATE [RemittanceDB].[dbo].[BEFTNRequest] SET [PaymentStatus]=6, [Upload]=1, [UplodedBy]=@UplodedBy, UploadTime=@UploadTime "
                + " WHERE [RefNo]=@RefNo AND [AutoId]=@AutoId";

                SqlCommand _cmd = new SqlCommand(insStmt, connNewSystem);

                _cmd.Parameters.Add("@AutoId", SqlDbType.Int).Value = Convert.ToInt32(dtEftRecInfo.Rows[0]["AutoId"].ToString().Trim());
                _cmd.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = dtEftRecInfo.Rows[0]["RefNo"].ToString().Trim();
                _cmd.Parameters.Add("@UplodedBy", SqlDbType.VarChar).Value = loggedUserName;
                _cmd.Parameters.Add("@UploadTime", SqlDbType.DateTime).Value = DateTime.Now;

                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }

            return true;
        }

        internal DataTable LoadGlobalExchList()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT cast(PartyId as varchar)+' - '+UserId FROM [RemittanceDB].[dbo].[Users] WHERE isActive=1 AND PartyId NOT IN(10005,10010) ORDER BY UserId";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetBkashRegTxn(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=m.ExchangeHouseID) ExcName,[TranTxnId],[RemitReceiverMSISDN] WalletNo,[RemitReceiverAmount] Amount,[responseMessage] RespMsg "
                    + " ,[ConversationID] ConvId,[CBResponseDescription] CBRespDesc,[RemitStatus] Status,convert(varchar, [RequestTime], 120) RecvTime, convert(varchar, [MTBProcessTime], 120) ProcessTime, convert(varchar, [CBReceiveTime], 120) BkashRespTime, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] m left join [RemittanceDB].[dbo].[Transactions] tr ON m.TranTxnId=tr.SessionId "
                    + " where [ExchangeHouseID]=" + exhId
                    + " and ( convert(varchar, [MTBProcessTime], 23) between '" + frmdt + "' and '" + todt + "' ) "
                    + " order by [MTBProcessTime] desc";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)){ connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetBkashRegTxnSuccessSumr(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select [RemitStatus] FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] m left join [RemittanceDB].[dbo].[Transactions] tr ON m.TranTxnId=tr.SessionId "
                    + " where [ExchangeHouseID]=" + exhId
                    + " and ( convert(varchar, [MTBProcessTime], 23) between '" + frmdt + "' and '" + todt + "' ) and [RemitStatus]=5 ";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)){   connDRSystem.Close();    }
            }
            return dt;
        }

        internal DataTable GetBkashRegTxnUnSuccessSumr(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select [RemitStatus] FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] m left join [RemittanceDB].[dbo].[Transactions] tr ON m.TranTxnId=tr.SessionId "
                    + " where [ExchangeHouseID]=" + exhId
                    + " and ( convert(varchar, [RequestTime], 23) between '" + frmdt + "' and '" + todt + "' ) and [RemitStatus]<>5";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)){ connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetBEFTNDetailTxn(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=b.PartyId) Exch, ltrim(rtrim(b.RefNo))RefNo, "
                    + " case when b.PaymentStatus=1 then 'Received' when b.PaymentStatus=2 then 'Canceled' when b.PaymentStatus=3 then 'Queued' "
                    + " when b.PaymentStatus=4 then 'Uploded' when b.PaymentStatus=5 then 'Success' when b.PaymentStatus=6 then 'Returned' when b.PaymentStatus=7 then 'Ready for Disburse' "
                    + " when b.PaymentStatus=8 then 'Disbursed' when b.PaymentStatus=9 then 'Stopped' else '' end Status, "
                    + " (select BE.Remarks from BEFTNExecutionStatus BE where BE.BEFTNRequestId =b.AutoId and ID in( select max(ID) from BEFTNExecutionStatus BE where BE.BEFTNRequestId =b.AutoId))Remarks, "
                    + " convert(varchar, b.RequestTime, 20) RequestTime, "
                    + " case when b.UplodedBy='AutoProcess' then  convert(varchar, b.LastProcessingTime, 20) else convert(varchar, b.UploadTime, 20) end ProcessTime, "
                    + " b.BeneficiaryName Benf, b.[BeneficiaryAccountNo] AccountNo, b.BeneficiaryBankName bank, b.Amount, b.UplodedBy "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b  WHERE b.PartyId=" + exhId
                    + " and ( convert(varchar, b.[RequestTime], 23) between '" + frmdt + "' and '" + todt + "' ) "
                    + " and b.isIncentive in(0,99) "
                    + " order by b.[RequestTime] desc";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetBEFTNSumrTxn(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT b.PaymentStatus, count(*) FROM [RemittanceDB].[dbo].[BEFTNRequest] b where b.PartyId=" + exhId
                    + " and ( convert(varchar, [RequestTime], 23) between '" + frmdt + "' and '" + todt + "' ) and b.isIncentive in(0,99) "
                    + " GROUP BY PaymentStatus ORDER BY PaymentStatus";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetMTBAcDetailTxn(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=ft.PartyId) Exch, ltrim(rtrim(RefNo))RefNo, "
                    + " case when ft.PaymentStatus=1 then 'Received' when ft.PaymentStatus=2 then 'Canceled' when ft.PaymentStatus=3 then 'Queued' "
                    + " when ft.PaymentStatus=4 then 'Uploded' when ft.PaymentStatus=5 then 'Success' when ft.PaymentStatus=6 then 'Returned' when ft.PaymentStatus=7 then 'Ready for Disburse' "
                    + " when ft.PaymentStatus=8 then 'Disbursed' when ft.PaymentStatus=9 then 'Stopped' else '' end Status, "
                    + " ft.BeneficiaryAccountNo AccountNo, ft.BeneficiaryName, convert(varchar, TransDate, 120) TrxnDate, ft.TrAmount, ft.IsSuccess "
                    + " FROM [RemittanceDB].[dbo].FundTransferRequest ft WHERE  PartyId=" + exhId
                    + " and convert(varchar, TransDate, 23) BETWEEN '" + frmdt + "'  and '" + todt + "' "
                    + " ORDER BY TransDate desc";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetMTBAcSuccTxn(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select count(*) cnt from [RemittanceDB].[dbo].[FundTransferRequest] ft where  PartyId=" + exhId + " and ft.PaymentStatus=5 "
                    + " and convert(varchar, TransDate, 23) between '" + frmdt + "'  and '" + todt + "' ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();   }
            }
            return dt;
        }

        internal DataTable GetCashDetailTxn(int exhId, string frmdt, string todt)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT (select userid from [RemittanceDB].[dbo].[Users] u where u.PartyId=ri.PartyId) Exch, ltrim(rtrim([RemitenceId]))RefNo, [PinNo],"
                    + " case when ri.Status=1 then 'Received' when ri.Status=2 then 'Canceled' when ri.Status=3 then 'Queued' "
                    + " when ri.Status=4 then 'Uploded' when ri.Status=5 then 'Success' when ri.Status=6 then 'Returned' when ri.Status=7 then 'Ready for Disburse' "
                    + " when ri.Status=8 then 'Disbursed' when ri.Status=9 then 'Stopped' else '' end Status, [SenderName],"
                    + " [BeneficiaryName],[BeneficiaryAddress],[BeneficiaryMobileNo],[Amount],convert(varchar, [RequestTime], 120) RequestTime "
                    + " FROM [RemittanceDB].[dbo].[Remittanceinfo] ri WHERE ri.PartyId=" + exhId
                    + " AND convert(varchar, [RequestTime], 23) between '" + frmdt + "'  and '" + todt + "' "
                    + " ORDER BY [RequestTime] desc";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetReturnEmailStatsByDate(string fromdt)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string query = "SELECT [AutoId] SL,[PartyId],(Select EHIL.[ExchangeHouseFullName] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] EHIL Where EHIL.PartyId=RESS.PartyId)ExchangeHouse,"
                    + " [ReturnRowCount]No_of_Records, [MailSendStatus] MailStats, convert(varchar, [ReturnDate], 23)ReturnDate, convert(varchar, [MailSendDate], 20)MailSendDate "
                    + " FROM [NRBWork].[dbo].[BEFTNReturnEmailSendStatus] RESS Where convert(varchar, [ReturnDate], 23)='" + fromdt + "' ORDER BY [AutoId] desc";

                SqlDataAdapter sdaRetMail = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaRetMail.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)){ connNRBWorkSystem.Close();  }
            }
            return dt;
        }

        internal DataTable GetBEFTNReturnTxnList(string dtValueFrom, string dtValueTo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT convert(varchar, [ReturnedTime], 23)ReturnDate, (Select UserId from [RemittanceDB].[dbo].[Users] u where u.PartyId=br.PartyId) Exch,[RefNo],"
                    + " (Select ps.[Status] FROM [RemittanceDB].[dbo].[PaymentStatus] ps where ps.StatusID= br.PaymentStatus) Status,"
                    + " [BeneficiaryName],[BeneficiaryAccountNo] AccountNo,[BeneficiaryBankName] BankName,[BeneficiaryBankBranchName] BranchName,[DestinationRoutingNO] RoutingNo, cast(round([Amount],2) as numeric(16,2)) Amount, "//round([Amount],2) Amount, "
                    + " convert(varchar, [RequestTime], 20)RequestDate, convert(varchar, [LastProcessingTime], 20) ProcessDate, [ReturnedReason] "
                    + " ,(Select AccountNo from [RemittanceDB].[dbo].[Users] u where u.PartyId=br.PartyId) ExhAccNo "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] br WHERE [PaymentStatus]=6 AND br.IsIncentive IN(0,99) "
                    + " and convert(varchar, [ReturnedTime], 23) BETWEEN '" + dtValueFrom + "' AND '" + dtValueTo + "' order by Exch ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }


        //NRBWork
        internal DataTable GetBEFTNReturnEmailExchList()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT [PartyId],[UserId],[ExchangeHouseFullName] ExchangeHouseName, isnull([ToAddress],'')ToAddress,isnull([CcAddress],'')CcAddress,isnull([BccAddress],'')BccAddress "
                    + " FROM [NRBWork].[dbo].[ExchangeHouseInfoList] Where [isActive]=1 AND isnull([ToAddress],'')<>'' ORDER BY [ExchangeHouseFullName]";
                SqlDataAdapter sdaRetMail = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaRetMail.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }


        internal DataTable GetReturnEmailSendStatsByDateAndExhId(int exhId, string dtValue)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT [AutoId] SL,[PartyId],(Select EHIL.[ExchangeHouseFullName] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] EHIL Where EHIL.PartyId=RESS.PartyId)ExchangeHouse,"
                    + " [ReturnRowCount] RetCnt,[MailSendStatus] MailSend, convert(varchar, [MailSendDate], 20)SendDate, convert(varchar, [ReturnDate], 23)ReturnDate "
                    + " FROM [NRBWork].[dbo].[BEFTNReturnEmailSendStatus] RESS Where [PartyId]=" + exhId + " AND convert(varchar, [ReturnDate], 23)='" + dtValue + "'";

                SqlDataAdapter sdaRetMail = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaRetMail.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal bool InsertExhMailSendStatus(string partyId, string userId, int retRowCount, bool mailSendStats, string returnDate)
        {
            SqlConnection connNRBWorkSystem = null;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string mailStat = mailSendStats ? "YES" : "NO";
                string insQry = "INSERT INTO [NRBWork].[dbo].[BEFTNReturnEmailSendStatus]([PartyId],[UserId],[ReturnRowCount],[MailSendStatus],[ReturnDate]) "
                    + " VALUES('" + partyId + "', '" + userId + "', " + retRowCount + ", '" + mailStat + "','" + returnDate + "')";

                SqlCommand _cmd = new SqlCommand(insQry, connNRBWorkSystem);
                try { _cmd.ExecuteNonQuery(); return true; }
                catch (Exception exc) { }
                _cmd.Dispose();

                return false;
            }
            catch (Exception exc) { return false; }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
        }


        internal DataTable GetReturnInfoFromBEFTNRequestTable(int exhId, string dtValue)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "select p.ReturnDate, p.PartyId, p.Exch, sum(p.Amount) amt, count(*) cnt "
                    + " from ( "
                    + " SELECT convert(varchar, [ReturnedTime], 23)ReturnDate, br.PartyId, (Select UserId from [RemittanceDB].[dbo].Users u where u.PartyId=br.PartyId) Exch, "
                    + " cast(round([Amount],2) as numeric(16,2)) Amount "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] br  WHERE [PaymentStatus]=6 AND br.IsIncentive in(0,99) and convert(varchar, [ReturnedTime], 23)='" + dtValue + "' and PartyId=" + exhId
                    + " )p "
                    + " group by ReturnDate, PartyId, Exch";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetReturnMailInfoFromBEFTNReturnEmailSendStatusTable(int exhId, string dtValue)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();
                string query = "SELECT [MailSendStatus] MailStats, convert(varchar, [MailSendDate], 20)MailSendDate "
                + " FROM [NRBWork].[dbo].[BEFTNReturnEmailSendStatus] RESS Where convert(varchar, [ReturnDate], 23)='" + dtValue + "' and [PartyId]=" + exhId;
                SqlDataAdapter sdaRetMail = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaRetMail.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetEFTReturnMailStatus(int exhId, string dtValue)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();
                string query = "SELECT * FROM [NRBWork].[dbo].[BEFTNReturnEmailSendStatus] Where convert(varchar, [ReturnDate], 23)='" + dtValue + "' and [PartyId]=" + exhId;
                SqlDataAdapter sdaRetMail = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaRetMail.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetExhEmailInfoByPartyId(int PartyId)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT [PartyId],[UserId],[ToAddress],[CcAddress],[BccAddress],[ExchangeHouseFullName] ExchangeHouseName FROM [NRBWork].[dbo].[ExchangeHouseInfoList] "
                + " WHERE [isActive]=1 AND [PartyId]=" + PartyId + "";
                SqlDataAdapter sdaRetMail = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaRetMail.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetExhReturnTxnByPartyId(string partyId, string returnDate, string exhouseName)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [PartyId], RefNo, BeneficiaryAccountNo, BeneficiaryName, BeneficiaryBankName, Amount, "
                    + "'" + exhouseName + "' ExchName, "
                    + " convert(varchar, RequestTime, 23)RequestDate, convert(varchar, LastProcessingTime, 23)ProcessDate, convert(varchar, [ReturnedTime], 23)ReturnedDate, [ReturnedReason] "
                    + " FROM [RemittanceDB].[dbo].BEFTNRequest br WHERE PaymentStatus=6 AND br.IsIncentive in(0,99) AND PartyId=" + partyId
                    + " AND convert(varchar, [ReturnedTime], 23)='" + returnDate + "' "
                    + " ORDER BY AutoId DESC";

                SqlDataAdapter sdaRetInfo = new SqlDataAdapter(query, connDRSystem);
                sdaRetInfo.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)){  connDRSystem.Close();  }
            }
            return dt;
        }

        internal bool UpdateExhMailSendStatus(int slNo, string partyId, bool mailSendStats)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string mailStat = mailSendStats ? "YES" : "NO";
                string updQry = "UPDATE [NRBWork].[dbo].[BEFTNReturnEmailSendStatus] SET [MailSendStatus]='" + mailStat + "', [MailSendDate]=getdate()  WHERE [AutoId]=" + slNo + " AND [PartyId]=" + partyId + "";

                SqlCommand _cmd = new SqlCommand(updQry, connNRBWorkSystem);
                try { _cmd.ExecuteNonQuery(); return true; }
                catch (Exception exc) { }
                _cmd.Dispose();
                return false;
            }
            catch (Exception exc) { return false; }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
        }

        internal DataTable GetDirectMTOName()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT cast(PartyId as varchar)+'-'+UserId FROM [RemittanceDB].[dbo].[Users] WHERE isActive=1 and upper([UserId]) like upper('%Direct%') ORDER BY PartyId";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetRippleMTOName()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT cast(PartyId as varchar)+'-'+UserId FROM [RemittanceDB].[dbo].[Users] WHERE isActive=1 and upper([UserId]) like upper('%Ripple%') ORDER BY PartyId";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetServiceRemMTOName()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT cast(PartyId as varchar)+'-'+UserId FROM [RemittanceDB].[dbo].[Users] WHERE isActive=1 and PartyTypeId=2 ORDER BY PartyId";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetDirectModeTxnDetailByPartyIdAndDate(string dtDirect, int prtyId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                if (prtyId == 0)
                {                    
                    query = "SELECT ds.[ID],ds.[TxnId] PIN, ds.[PartyID],ds.[MtoName],ds.[ResponseDescription]RespDesc, ds.[ProcessedTimestamp] ProcessTime,"
                        + " CONVERT(float, ds.[Amount])Amount,ds.[BeneficiaryWalletNumber]Account, ds.[isSuccess] Success,ds.[Reason],ds.[StatusID] Status "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds  "
                        + " where ds.[ProcessedTimestamp] like '" + dtDirect + "%' order by ID desc";
                }
                else
                {                    
                    query = "SELECT ds.[ID],ds.[TxnId] PIN, ds.[PartyID],ds.[MtoName],ds.[ResponseDescription]RespDesc, ds.[ProcessedTimestamp] ProcessTime,"
                        + " CONVERT(float, ds.[Amount])Amount,ds.[BeneficiaryWalletNumber]Account, ds.[isSuccess] Success,ds.[Reason],ds.[StatusID] Status "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds "
                        + " where ds.PartyID=" + prtyId + " AND ds.[ProcessedTimestamp] like '" + dtDirect + "%' order by ID desc";
                }

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) {  connDRSystem.Close();   }
            }
            return dt;
        }

        internal DataTable GetDirectModeTxnPendingByPartyIdAndDate(string dtDirect, int prtyId, ref string pendingSumOfAmount)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            DataTable dtDirectAMLAmount = new DataTable();
            string query = "", querySum = "";

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                if (prtyId == 0)
                {                    
                    query = "SELECT ds.[ID],ds.[TxnId] PIN, ds.[PartyID],ds.[MtoName],ds.[ResponseDescription]RespDesc, ds.ProcessedTimestamp ProcessTime,"
                        + " CONVERT(float, ds.[Amount])Amount,ds.[BeneficiaryWalletNumber]Account, ds.[isSuccess] Success,ds.[SenderNameAMLScore] SendScore,ds.[BeneficiaryNameAMLScore] BeneScore "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds "
                        + " where ds.[ProcessedTimestamp] like '" + dtDirect + "%'  AND ds.[StatusID] in(1,3) order by ID desc";

                    querySum = "SELECT isnull(sum(CONVERT(float, ds.[Amount])),0) FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds "
                        + " where ds.[ProcessedTimestamp] like '" + dtDirect + "%'  AND ds.[StatusID] in(1,3) ";
                }
                else
                {                   
                    query = "SELECT ds.[ID],ds.[TxnId] PIN, ds.[PartyID],ds.[MtoName],ds.[ResponseDescription]RespDesc, ds.ProcessedTimestamp ProcessTime,"
                        + " CONVERT(float, ds.[Amount])Amount,ds.[BeneficiaryWalletNumber]Account, ds.[isSuccess] Success,ds.[SenderNameAMLScore] SendScore,ds.[BeneficiaryNameAMLScore] BeneScore "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds "
                        + " where ds.PartyID=" + prtyId + " AND ds.[ProcessedTimestamp] like '" + dtDirect + "%' AND ds.[StatusID] in(1,3) order by ID desc";

                    querySum = "SELECT isnull(sum(CONVERT(float, ds.[Amount])),0) FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds "
                        + " where ds.PartyID=" + prtyId + " AND ds.[ProcessedTimestamp] like '" + dtDirect + "%' AND ds.[StatusID] in(1,3) ";
                }

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);

                cmd = new SqlCommand(querySum, connDRSystem);
                sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dtDirectAMLAmount);

                pendingSumOfAmount = dtDirectAMLAmount.Rows[0][0].ToString();

            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)){ connDRSystem.Close(); }
            }
            return dt;
        }


        internal DataTable GetDirectModeSummaryTxn(string dtDirect)
        {
            SqlConnection connDRSystem = new SqlConnection(connInfo.getConnStringDR());
            DataTable dt = new DataTable();

            try
            {
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }
                
                string qrySumr = "SELECT [PartyID],[MtoName] PartyName, count(*) 'No of Txn' "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds "
                    + " where ds.[ProcessedTimestamp] like '" + dtDirect + "%' "
                    + " group by  [PartyID],[MtoName] order by [PartyID]";

                SqlCommand cmd = new SqlCommand(qrySumr, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally{ if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close();  }  }
            return dt;
        }

        internal DataTable GetBkashDirectPendingByPartyId(int prtyId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT [ID],[PartyID],[TxnId],[MtoName],CONVERT(float, [Amount]) Amount,[BeneficiaryName],[BeneficiaryWalletNumber] BeneWallet "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds where ds.PartyID=" + prtyId + " AND [StatusID]=1 order by ID";

                SqlDataAdapter sdaInfo = new SqlDataAdapter(query, connDRSystem);
                sdaInfo.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open))
                {
                    connDRSystem.Close();
                }
            }
            return dt;
        }

        internal DataTable GetBkashDirectMTOAccountNumberByPartyId(int prtyId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT [AccountNo], [MobileWalletPaymentAccount] FROM [RemittanceDB].[dbo].[Users] WHERE [PartyId]=" + prtyId + "";
                SqlDataAdapter sdaInfo = new SqlDataAdapter(query, connDRSystem);
                sdaInfo.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open))
                {
                    connDRSystem.Close();
                }
            }
            return dt;
        }

        // Lv Conn
        internal bool UpdateRemitStatusDirectSettlementTable(string autoid, string txnId, string fromAcc, string toAcc, string journal)
        {
            SqlConnection connNewSystem = null;
            bool execStat = false;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                connNewSystem.Open();

                string updQry = "UPDATE [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] SET [StatusID]=2, [isSuccess]=1, [FromAccTraceNO]='" + journal + "', [FromAccount]='" + fromAcc + "', [ToAccount]='" + toAcc + "'  WHERE [ID]=" + autoid + " AND [TxnId]='" + txnId + "'";
                SqlCommand _cmd = new SqlCommand(updQry, connNewSystem);

                try { _cmd.ExecuteNonQuery(); execStat = true; }
                catch (Exception exc) { }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return execStat;
        }

        internal bool IsTrxnExistInDirectRemitProcessTable(string partyId, string autoid)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT * FROM [RemittanceDB].[dbo].[MobileWalletDirectRemitProcess] WHERE [UserId]='" + partyId + "' AND [TranRefTxnId]=" + autoid;
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaRt = new SqlDataAdapter(cmd);
                sdaRt.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open))
                {
                    connDRSystem.Close();
                }
            }

            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        internal void SaveTrxnIntoDirectRemitProcessTable(string partyId, string autoid)
        {
            SqlConnection connNewSystem = null;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                connNewSystem.Open();

                string insQry = "INSERT INTO [dbo].[MobileWalletDirectRemitProcess]([UserId],[TranRefTxnId],[ProcessTime],[Remarks]) VALUES ('" + partyId + "','" + autoid + "', getdate(),'')";
                SqlCommand _cmd = new SqlCommand(insQry, connNewSystem);

                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
        }

        internal DataTable GetDirectModeAllPendingTxn(ref string sumOfPendingTxn)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            DataTable dtSumAmount = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT ds.[ID],ds.[TxnId],ds.[PartyID],ds.[MtoName],ds.[ResponseDescription]RespDesc, ds.ProcessedTimestamp RequestTime, "
                    + " CONVERT(float, ds.[Amount])Amount,ds.[BeneficiaryWalletNumber]Account, ds.[isSuccess] Success, ds.[StatusID] "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds  WHERE ds.[StatusID] in(1,3) order by ds.ID desc";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);

                string querySum = "SELECT round(isnull(sum(CONVERT(float, [Amount])),0),2) FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] WHERE [StatusID] in(1,3) ";
                cmd = new SqlCommand(querySum, connDRSystem);
                sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dtSumAmount);

                sumOfPendingTxn = dtSumAmount.Rows[0][0].ToString();
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open))
                {
                    connDRSystem.Close();
                }
            }
            return dt;
        }


        //---------------- weekly summary --------------------
        internal DataTable GetBEFTNTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'BEFTN', count(*), isnull(sum(p.amount),0) FROM ( "
                    + " SELECT * FROM [RemittanceDB].[dbo].BEFTNRequest WHERE paymentstatus=5 and convert(varchar, UploadTime, 23) between '" + dt1 + "' AND '" + dt2 + "'"
                    + " UNION "
                    + " SELECT * FROM [RemittanceDB].[dbo].BEFTNRequest WHERE paymentstatus=5 and convert(varchar, LastProcessingTime, 23) between '" + dt1 + "' AND '" + dt2 + "'"
                    + " )p ";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetMTBTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'MTB Ac',count(*), isnull(sum(tramount),0) FROM [RemittanceDB].[dbo].FundTransferRequest WHERE paymentstatus=5 and convert(varchar, TransDate, 23) between '" + dt1 + "' AND '" + dt2 + "'";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetCashTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'Cash',count(*), isnull(sum([Amount]),0) FROM [RemittanceDB].[dbo].[Remittanceinfo] WHERE [Status]=8 and convert(varchar, [ClearingDate], 23) between '" + dt1 + "' AND '" + dt2 + "'";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashRegTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'bKash', count(*) cnt, isnull(sum(CONVERT(float, RemitReceiverAmount)),0) amt FROM [RemittanceDB].[dbo].MobileWalletRemitTransfer mr INNER JOIN [RemittanceDB].[dbo].[Users] ur on mr.ExchangeHouseID=ur.PartyID and ur.PartyTypeId<>2 WHERE remitstatus=5 and convert(varchar, MTBProcessTime, 23) between '" + dt1 + "' AND '" + dt2 + "'";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashServiceRemTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'bKash', count(*) cnt, isnull(sum(CONVERT(float, RemitReceiverAmount)),0) amt FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mr INNER JOIN [RemittanceDB].[dbo].[Users] ur on mr.ExchangeHouseID=ur.PartyID and ur.PartyTypeId=2 WHERE remitstatus=5 and convert(varchar, MTBProcessTime, 23) between '" + dt1 + "' AND '" + dt2 + "'";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashDirectTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                //string query = "SELECT 'bKash', count(*) cnt, isnull(sum(CONVERT(float, ds.[Amount])),0) amt "
                //    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds inner join [RemittanceDB].[dbo].MobileWalletDirectRemitProcess drp on ds.ID=drp.TranRefTxnId "
                //    + " and convert(varchar, drp.ProcessTime, 23) between '" + dt1 + "' AND '" + dt2 + "'";

                string query = "SELECT 'bKash', count(*) cnt, isnull(sum(CONVERT(float, ds.[Amount])),0) amt "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds inner join [RemittanceDB].[dbo].Transactions txn on ds.TxnId=txn.SessionId "
                    + " and convert(varchar, txn.TransactionDate, 23) between '" + dt1 + "' AND '" + dt2 + "'";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetRippleTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dtRippleTxn = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'Ripple',count(*), round(isnull(sum(CONVERT(float, [quote_amount])),0),2) FROM [RemittanceDB].[dbo].[RippleGETPaymentData] WHERE payment_type='REGULAR' AND payment_state not in('FAILED','LOCKED') AND payment_module='WALLET' AND convert(varchar, [modified_at], 23) between '" + dt1 + "' AND '" + dt2 + "'";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dtRippleTxn);

                for (int rr = 0; rr < dtRippleTxn.Rows.Count; rr++)
                {
                    string amt = dtRippleTxn.Rows[rr][2].ToString();
                    if (amt == null || amt.Equals(""))
                    {
                        dtRippleTxn.Rows[rr][2] = 0;
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dtRippleTxn;
        }

        internal DataTable GetRippleReturnTxnAllExchSummary(string dt1, string dt2)
        {
            SqlConnection connDRSystem = null;
            DataTable dtRippleReturnTxn = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'Ripple',count(*), round(isnull(sum(CONVERT(float, [quote_amount])),0),2) FROM [RemittanceDB].[dbo].[RippleGETPaymentData] WHERE payment_type='RETURN' AND payment_state not in('FAILED','LOCKED') AND payment_module='WALLET' AND convert(varchar, [modified_at], 23) between '" + dt1 + "' AND '" + dt2 + "'";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dtRippleReturnTxn);

                for (int rr = 0; rr < dtRippleReturnTxn.Rows.Count; rr++)
                {
                    string amt = dtRippleReturnTxn.Rows[rr][2].ToString();
                    if (amt == null || amt.Equals(""))
                    {
                        dtRippleReturnTxn.Rows[rr][2] = 0;
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dtRippleReturnTxn;
        }

        //---------------- weekly summary --------------------

        internal DataTable GetAPIBasedExchListSummary()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "Select cast(PartyId as varchar)+' - '+UserId from [RemittanceDB].[dbo].[Users] WHERE isActive=1 AND [PartyId] NOT IN(10005,10010) order by [UserId]";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBEFTNTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();
                               
                // if CBSDate is null then consider processing date
                //string query = "SELECT 'BEFTN', count(*), round(isnull(sum(CONVERT(float, Amount)),0),2) FROM [RemittanceDB].[dbo].BEFTNRequest "
                //    +" WHERE PartyId=" + exhId + " AND paymentstatus=5 and IsIncentive in(99,0) and "
                //    +" ( (convert(varchar, CBSValueDate, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "') "
                //    + " OR (convert(varchar, LastProcessingTime, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "') )";

                string query = "SELECT 'BEFTN', count(*), round(isnull(sum(CONVERT(float, p.Amount)),0),2) FROM ( "
                    + " SELECT AutoId, Amount FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and (convert(varchar, CBSValueDate, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "')"
                    + " UNION "
                    + " SELECT AutoId, Amount FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and CBSValueDate is null and (convert(varchar, LastProcessingTime, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "') "
                    + " )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBEFTNReturnTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                //string query = "SELECT 'BEFTN', count(*), round(isnull(sum(CONVERT(float, p.Amount)),0),2) FROM ( "
                //    + " SELECT * FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND paymentstatus=6 and ReturnedReason is not null and IsIncentive in(99,0) and convert(varchar, UploadTime, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "'"
                //    + " UNION  "
                //    + " SELECT * FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND paymentstatus=6 and ReturnedReason is not null and IsIncentive in(99,0) and convert(varchar, ReturnedTime, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "'"
                //    + " )p ";

                string query = "SELECT 'BEFTN', count(*), round(isnull(sum(CONVERT(float, Amount)),0),2) FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND paymentstatus=6 and ReturnedReason IS NOT NULL and IsIncentive in(99,0) and convert(varchar, CBSValueDate, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "'";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetMTBTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                //string query = "SELECT 'MTB Ac',count(*), round(isnull(sum(CONVERT(float, TrAmount)),0),2)  FROM [RemittanceDB].[dbo].FundTransferRequest WHERE PartyId=" + exhId + " AND PaymentStatus=5 and convert(varchar, TransDate, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "'"; 

                string query = "SELECT 'MTB Ac',count(*), round(isnull(sum(CONVERT(float, p.Amount)),0),2) FROM ( "
                    + " select tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.IsSuccess "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('101', '102', '201', '202') "
                    + " and CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "' and tr.IsSuccess = 1 and tr.UserId=" + exhId
                    + " )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetCashTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            DataTable dt = new DataTable();
            SqlConnection connNRBWorkSystem = null;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string query = "SELECT 'Cash', count(*), round(isnull(sum(CONVERT(float, Amount)),0),2) FROM [NRBWork].[dbo].[CashTxnPanBankReport] WHERE ([ReportFromDate]>='" + dtValue1 + "' AND [ReportToDate]<='" + dtValue2 + "') and [PartyId]=" + exhId;

                if (exhId == 10008) // NEC MONEY TRANSFER  only Agent Point Cash will count
                {
                    query = "SELECT 'Cash', count(*), round(isnull(sum(CONVERT(float, Amount)),0),2) FROM [NRBWork].[dbo].[CashTxnPanBankReport] WHERE ([ReportFromDate]>='" + dtValue1 + "' AND [ReportToDate]<='" + dtValue2 + "') and [PartyId]=" + exhId + " AND BranchCode=113";
                }
                
                SqlCommand cmd = new SqlCommand(query, connNRBWorkSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashRegTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                //string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, RemitReceiverAmount)),0),2) amt FROM [RemittanceDB].[dbo].MobileWalletRemitTransfer WHERE ExchangeHouseID=" + exhId + " AND RemitStatus=5 AND CBresponseCode=3000 and convert(varchar, MTBProcessTime, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "'";

                string query = "SELECT 'bKash', count(*), round(isnull(sum(CONVERT(float, p.Amount)),0),2) FROM ( "
                    + " SELECT ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('105','205') "
                    + " and (CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and tr.IsSuccess = 1 and tr.UserId=" + exhId + ""
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('105','205') "
                    + " and (CONVERT(DATETIME, trn.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) "
                    + " )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            DataTable dt = new DataTable();
            try
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) connNRBWorkSystem.Open();

                string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, [DestAmount])),0),2) amt FROM [NRBWork].[dbo].[BkashTerrapayReportCommissionData] WHERE PartyId=" + exhId + " AND ([ReportFromDate]= '" + dtValue1 + "' AND [ReportToDate]='" + dtValue2 + "')";

                SqlCommand cmd = new SqlCommand(query, connNRBWorkSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }


        internal DataTable GetbKashDirectTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();
                              
                //string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, ds.[Amount])),0),2) amt "
                //            + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] ds inner join [RemittanceDB].[dbo].Transactions txn on ds.TxnId=txn.SessionId "
                //            + " and convert(varchar, txn.TransactionDate, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "' AND ds.[PartyID]=" + exhId;

                string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, p.Amount)),0),2) amt "
                    + " FROM ( SELECT tr.SessionId PINNumber, tr.Amount, convert(varchar, tr.TransactionDate, 23)TransDate, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] d INNER JOIN [RemittanceDB].[dbo].Transactions tr  ON d.TxnId = tr.SessionId  AND d.PartyID = tr.UserId "
                    + " WHERE d.StatusID = 2 AND tr.IsSuccess = 1 AND tr.Type = '107' AND (CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "')"
                    + " and tr.UserId=" + exhId + ")p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }


        internal DataTable GetbKashRegSpecialDateCaseSummaryByExchId(string dtValue1, string previousMonthLastDayDate, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, p.Amount)),0),2) amt "
                    + " FROM ( SELECT tr.ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate, mw.MTBProcessTime "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw inner join [RemittanceDB].[dbo].Transactions tr on mw.TranTxnId=tr.SessionId "
                    + " WHERE tr.[Type] IN ('105','205') and tr.IsSuccess = 1 and tr.UserId=" + exhId + " and mw.ExchangeHouseID=" + exhId + " and  convert(varchar, mw.MTBProcessTime, 23)='" + dtValue1 + "'  AND CONVERT(DATETIME, tr.CBSValueDate, 105) ='" + previousMonthLastDayDate + "' "
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('105','205') "
                    + " and CONVERT(DATETIME, trn.CBSValueDate, 105) = '" + previousMonthLastDayDate + "' and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashRegForwardDateCBSIssueSummaryByExchId(string dtValue2, string forwardDate, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, p.Amount)),0),2) amt "
                    + " FROM ( select tr.ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate, mw.MTBProcessTime "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw inner join [RemittanceDB].[dbo].Transactions tr on mw.TranTxnId=tr.SessionId "
                    + " WHERE tr.[Type] IN ('105','205') and tr.IsSuccess = 1 and tr.UserId=" + exhId + " and mw.ExchangeHouseID=" + exhId + " and  convert(varchar, mw.MTBProcessTime, 23)='" + dtValue2 + "'  AND CONVERT(DATETIME, tr.CBSValueDate, 105) ='" + forwardDate + "' "
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('105','205') "
                    + " and CONVERT(DATETIME, trn.CBSValueDate, 105) = '" + forwardDate + "' and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }


        // SERVICE REM BKASH
        internal DataTable GetbKashServiceRemTxnSummaryByExchId(string dtValue1, string dtValue2, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();
                                
                string query = "SELECT 'bKash', count(*), round(isnull(sum(CONVERT(float, p.Amount)),0),2) FROM ( "
                    + " SELECT ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('106') "
                    + " and (CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and tr.IsSuccess = 1 and tr.UserId=" + exhId + ""
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('106') "
                    + " and (CONVERT(DATETIME, trn.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) "
                    + " )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashServiceRemSpecialDateCaseSummaryByExchId(string dtValue1, string previousMonthLastDayDate, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, p.Amount)),0),2) amt "
                    + " FROM ( select tr.ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate, mw.MTBProcessTime "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw inner join [RemittanceDB].[dbo].Transactions tr on mw.TranTxnId=tr.SessionId "
                    + " WHERE tr.[Type] IN ('106') and tr.IsSuccess = 1 and tr.UserId=" + exhId + " and mw.ExchangeHouseID=" + exhId + " and  convert(varchar, mw.MTBProcessTime, 23)='" + dtValue1 + "'  AND CONVERT(DATETIME, tr.CBSValueDate, 105) ='" + previousMonthLastDayDate + "' "
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('106') "
                    + " and CONVERT(DATETIME, trn.CBSValueDate, 105) = '" + previousMonthLastDayDate + "' and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetbKashServiceRemForwardDateCBSIssueSummaryByExchId(string dtValue2, string forwardDate, int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT 'bKash', count(*) cnt, round(isnull(sum(CONVERT(float, p.Amount)),0),2) amt "
                    + " FROM ( SELECT tr.ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate, mw.MTBProcessTime "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw inner join [RemittanceDB].[dbo].Transactions tr on mw.TranTxnId=tr.SessionId "
                    + " WHERE tr.[Type] IN ('106') and tr.IsSuccess = 1 and tr.UserId=" + exhId + " and mw.ExchangeHouseID=" + exhId + " and  convert(varchar, mw.MTBProcessTime, 23)='" + dtValue2 + "'  AND CONVERT(DATETIME, tr.CBSValueDate, 105) ='" + forwardDate + "' "
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('106') "
                    + " and CONVERT(DATETIME, trn.CBSValueDate, 105) = '" + forwardDate + "' and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) )p ";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }



        internal DataTable GetAPIActiveExchList()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();
                                
                string query = "SELECT [PartyId],[UserId], [ExchangeHouseShortName] ExShortName,[ExchangeHouseFullName] ExFullName,[NRTAccount], [PartyTypeId] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] WHERE [isActive]=1 AND [PartyId]<>0 ORDER BY [ExchangeHouseShortName]";
                SqlCommand cmd = new SqlCommand(query, connNRBWorkSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetRippleWalletTxnSummaryByDate(string dtValue1, string dtValue2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT validator, count(*), round(isnull(sum(CONVERT(float, quote_amount)),0),2) "
                    + " FROM [RemittanceDB].[dbo].[RippleGETPaymentData] "
                    + " WHERE payment_type='REGULAR' and payment_state not in('FAILED','LOCKED') and payment_module='WALLET'"
                    + " and convert(varchar, [modified_at], 23) between '" + dtValue1 + "' AND '" + dtValue2 + "' "
                    + " group by validator";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetRippleWalletReturnTxnSummaryByDate(string dtValue1, string dtValue2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT quote_receiver_address, count(*), round(isnull(sum(CONVERT(float, quote_amount)),0),2) "
                    + " FROM [RemittanceDB].[dbo].[RippleGETPaymentData] "
                    + " WHERE payment_type='RETURN' and payment_state not in('FAILED','LOCKED') and payment_module='WALLET'"
                    + " and convert(varchar, [modified_at], 23) between '" + dtValue1 + "' AND '" + dtValue2 + "' "
                    + " group by quote_receiver_address";
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetExchList()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                //string query = "SELECT [exhId],[exhName] FROM [NRBWork].[dbo].[ExchangeHousesList] WHERE [isActive]=1 ORDER BY [exhName]";
                string query = "SELECT [AutoId],[ExchangeHouseFullName] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] WHERE [isActive]=1 ORDER BY [ExchangeHouseFullName]";
                SqlDataAdapter sdaExh = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaExh.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable CashTxnThisPinInfoInDatabase(string pinnumber)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT convert(varchar, dc.[InputDate], 105)InputDate, isnull(dc.[JournalNo],'')JournalNo, isnull(dc.[PINNumber],'')PINNumber, isnull(dc.[Amount],'0')Amount, "
                + " (SELECT ex.[ExchangeHouseFullName] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] ex where ex.[AutoId]= dc.[ExHouseId])exhName,"
                + " (SELECT ui.[UserFullName] FROM [NRBWork].[dbo].[WEB_UserInfo] ui where ui.[UserRMCode]=[PaymentUserId])UserName "
                + " FROM [NRBWork].[dbo].[CashTxnDuplicateCheck] dc WHERE LTRIM(RTRIM(dc.[PINNumber]))='" + pinnumber.Trim() + "'";
                SqlDataAdapter sdaExh = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaExh.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal bool SaveCashTxnData(string pinnumber, string journal, string amount, string exchId, string benfName, string userRmCode)
        {
            SqlConnection connNRBWorkSystem = null;
            SqlCommand cmdSaveData = new SqlCommand();
            string saveData = "";
            bool insertSuccess = false;

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                saveData = "INSERT into [NRBWork].[dbo].[CashTxnDuplicateCheck]([InputDate],[JournalNo],[PINNumber],[Amount],[ExHouseId],[Beneficiary],[PaymentUserId])"
                    + " VALUES (@InputDate,@JournalNo,@PINNumber,@Amount,@ExHouseId,@Beneficiary,@PaymentUserId)";

                cmdSaveData.CommandText = saveData;
                cmdSaveData.Connection = connNRBWorkSystem;

                cmdSaveData.Parameters.Add("@InputDate", SqlDbType.Date).Value = DateTime.Now;
                cmdSaveData.Parameters.Add("@JournalNo", SqlDbType.VarChar).Value = journal.Trim();
                cmdSaveData.Parameters.Add("@PINNumber", SqlDbType.VarChar).Value = pinnumber;
                cmdSaveData.Parameters.Add("@Amount", SqlDbType.VarChar).Value = amount;
                cmdSaveData.Parameters.Add("@ExHouseId", SqlDbType.Int).Value = exchId.Equals("") ? 0 : Convert.ToInt32(exchId);
                cmdSaveData.Parameters.Add("@Beneficiary", SqlDbType.VarChar).Value = benfName.ToUpper();
                cmdSaveData.Parameters.Add("@PaymentUserId", SqlDbType.VarChar).Value = userRmCode;

                try
                {
                    cmdSaveData.ExecuteNonQuery();
                    insertSuccess = true;
                }
                catch (Exception ec)
                {
                    insertSuccess = false;
                    throw ec;
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }

            return insertSuccess;
        }

        internal DataTable GetSavedCashPassingDataByDates(string fromDate, string toDate)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT [AutoId] SL, convert(varchar, dc.[InputDate], 105)InputDate, dc.[JournalNo], LTRIM(RTRIM(dc.[PINNumber]))PINNumber, dc.[Amount], "
                    + " (SELECT ex.[ExchangeHouseFullName] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] ex where ex.[AutoId]= dc.[ExHouseId])ExchName, [Beneficiary], "
                    //+ " (SELECT fbuc.[UserName] FROM [NRBWork].[dbo].[FileBasedCashTxnUserCredential] fbuc where fbuc.[UserId]=[PaymentUserId])UserName, "
                    + " (SELECT ui.[UserFullName] FROM [NRBWork].[dbo].[WEB_UserInfo] ui where ui.[UserRMCode]=[PaymentUserId])UserName, "
                    + " (convert(varchar, [TxnInsertDate], 105)+' '+convert(varchar, [TxnInsertDate], 24)) TxnInputTime "
                    + " FROM [NRBWork].[dbo].[CashTxnDuplicateCheck] dc WHERE dc.[InputDate] BETWEEN '" + fromDate + "' AND '" + toDate + "' order by SL desc";
                SqlDataAdapter sdaExh = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaExh.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable SearchTransactionByJournalOrPin(string journalOrPin)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string queryData = "select [AutoId], replace(convert(varchar, [TxnInsertDate], 106),' ','-') + ' '+ convert(varchar, [TxnInsertDate], 24) TxnProcessDate, "
                    + " [JournalNo],[PINNumber],[Amount], isnull((SELECT ex.[ExchangeHouseFullName] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] ex where ex.[AutoId]= [ExHouseId]),'') ExHouse, isnull([Beneficiary],'')Beneficiary, "
                    //+ " (SELECT [UserName] FROM [NRBWork].[dbo].[FileBasedCashTxnUserCredential] WHERE [UserId]=[PaymentUserId]) PaymentUser "
                    + " (SELECT ui.[UserFullName] FROM [NRBWork].[dbo].[WEB_UserInfo] ui where ui.[UserRMCode]=[PaymentUserId])PaymentUser "
                    + " FROM [NRBWork].[dbo].[CashTxnDuplicateCheck] WHERE [JournalNo]='" + journalOrPin + "' or PINNumber='" + journalOrPin + "'";
                SqlDataAdapter sdaExh = new SqlDataAdapter(queryData, connNRBWorkSystem);
                sdaExh.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetGCCPaidTxnReportList(string dtValueFrm, string dtValueTo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string queryData = "SELECT [TransactionNo],[AmountToPay] Amount,[ReceiverName],[SenderName],[TxnStatus],[PaymentMode],convert(varchar, [TxnReceiveDate], 20)ReceiveDate, convert(varchar, [TxnPaymentDate], 20)PaymentDate, "
                    +" [PurposeName],[ReceiverAddress],[ReceiverContactNo],[SenderAddress],[SendCountryName],[SenderCity],[SenderContactNo],[SenderIncomeSource],[SenderOccupation], convert(varchar, [SentDate], 23)SentDate, "
                    +" [Remarks], convert(varchar, [CancelDate], 20)CancelDate  "
                    + " FROM [RemittanceDB].[dbo].[GCCRequestData] WHERE PaymentMode='CASH' AND convert(date, [TxnPaymentDate]) BETWEEN '" + dtValueFrm + "' AND '" + dtValueTo + "' "
                    + " order by AutoId desc";

                SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool InsertIntoICDataTable(string userId, ICTCServiceClient.CashTxnDetails cashDetails, string downloadBranch, string downloadUser)
        {
            bool stats = true;
            SqlConnection connNewSystem = null;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                connNewSystem.Open();
                SqlCommand cmdSaveAcData = new SqlCommand();

                cmdSaveAcData.CommandType = CommandType.StoredProcedure;
                cmdSaveAcData.CommandText = "ICTCSpInsertAccountAndCashTxnData";
                cmdSaveAcData.Connection = connNewSystem;

                cmdSaveAcData.Parameters.Add("@Ictc_Number", SqlDbType.VarChar).Value = cashDetails.ICTC_Number.Trim();
                cmdSaveAcData.Parameters.Add("@Agent_Ordernumber", SqlDbType.VarChar).Value = cashDetails.Agent_OrderNumber == null ? "" : cashDetails.Agent_OrderNumber.Trim();
                cmdSaveAcData.Parameters.Add("@Remitter_Name", SqlDbType.VarChar).Value = cashDetails.Remitter_Name == null ? "" : cashDetails.Remitter_Name.Trim();
                cmdSaveAcData.Parameters.Add("@Remitter_Address", SqlDbType.VarChar).Value = cashDetails.Remitter_Address == null ? "" : cashDetails.Remitter_Address.Trim();
                cmdSaveAcData.Parameters.Add("@Remitter_Idtype", SqlDbType.VarChar).Value = cashDetails.Remitter_IDType == null ? "" : cashDetails.Remitter_IDType.Trim();
                cmdSaveAcData.Parameters.Add("@Remitter_Iddtl", SqlDbType.VarChar).Value = cashDetails.Remitter_IDDtl == null ? "" : cashDetails.Remitter_IDDtl.Trim();
                cmdSaveAcData.Parameters.Add("@Originating_Country", SqlDbType.VarChar).Value = cashDetails.Originating_Country == null ? "" : cashDetails.Originating_Country.Trim();
                cmdSaveAcData.Parameters.Add("@Delivery_Mode", SqlDbType.VarChar).Value = cashDetails.Delivery_Mode == null ? "" : cashDetails.Delivery_Mode.Trim();
                cmdSaveAcData.Parameters.Add("@Paying_Amount", SqlDbType.Float).Value = Math.Round(Convert.ToDouble(cashDetails.Paying_Amount), 2);
                cmdSaveAcData.Parameters.Add("@Payingagent_Commshare", SqlDbType.Float).Value = Math.Round(Convert.ToDouble(cashDetails.PayingAgent_CommShare), 2);
                cmdSaveAcData.Parameters.Add("@Paying_Currency", SqlDbType.VarChar).Value = cashDetails.Paying_Currency == null ? "" : cashDetails.Paying_Currency.Trim();
                cmdSaveAcData.Parameters.Add("@Paying_Agent", SqlDbType.VarChar).Value = cashDetails.Paying_Agent == null ? "" : cashDetails.Paying_Agent.Trim();
                cmdSaveAcData.Parameters.Add("@Paying_Agentname", SqlDbType.VarChar).Value = cashDetails.Paying_AgentName == null ? "" : cashDetails.Paying_AgentName.Trim();
                cmdSaveAcData.Parameters.Add("@Beneficiary_Name", SqlDbType.VarChar).Value = cashDetails.Beneficiary_Name == null ? "" : cashDetails.Beneficiary_Name.Trim();
                cmdSaveAcData.Parameters.Add("@Beneficiary_Address", SqlDbType.VarChar).Value = cashDetails.Beneficiary_Address == null ? "" : cashDetails.Beneficiary_Address.Trim();
                cmdSaveAcData.Parameters.Add("@Beneficiary_City", SqlDbType.VarChar).Value = cashDetails.Beneficiary_City == null ? "" : cashDetails.Beneficiary_City.Trim();
                cmdSaveAcData.Parameters.Add("@Destination_Country", SqlDbType.VarChar).Value = cashDetails.Destination_Country == null ? "" : cashDetails.Destination_Country.Trim();
                cmdSaveAcData.Parameters.Add("@Beneficiary_Telno", SqlDbType.VarChar).Value = cashDetails.Beneficiary_TelNo == null ? "" : cashDetails.Beneficiary_TelNo.Trim();
                cmdSaveAcData.Parameters.Add("@Beneficiary_Mobileno", SqlDbType.VarChar).Value = cashDetails.Beneficiary_MobileNo == null ? "" : cashDetails.Beneficiary_MobileNo.Trim();
                cmdSaveAcData.Parameters.Add("@Expected_Benefid", SqlDbType.VarChar).Value = cashDetails.Expected_BenefID == null ? "" : cashDetails.Expected_BenefID.Trim();
                cmdSaveAcData.Parameters.Add("@Bank_Address", SqlDbType.VarChar).Value = cashDetails.Bank_Address == null ? "" : cashDetails.Bank_Address.Trim();
                cmdSaveAcData.Parameters.Add("@Bank_Account_Number", SqlDbType.VarChar).Value = cashDetails.Bank_Account_Number == null ? "" : cashDetails.Bank_Account_Number.Trim();
                cmdSaveAcData.Parameters.Add("@Bank_Name", SqlDbType.VarChar).Value = cashDetails.Bank_Name == null ? "" : cashDetails.Bank_Name.Trim();
                cmdSaveAcData.Parameters.Add("@Purpose_Remit", SqlDbType.VarChar).Value = cashDetails.Purpose_Remit == null ? "" : cashDetails.Purpose_Remit.Trim();
                cmdSaveAcData.Parameters.Add("@Message_Payeebranch", SqlDbType.VarChar).Value = cashDetails.Message_PayeeBranch == null ? "" : cashDetails.Message_PayeeBranch.Trim();
                cmdSaveAcData.Parameters.Add("@Bank_Branchcode", SqlDbType.VarChar).Value = cashDetails.Bank_BranchCode == null ? "" : cashDetails.Bank_BranchCode.Trim();
                cmdSaveAcData.Parameters.Add("@Settlement_Rate", SqlDbType.Float).Value = cashDetails.Settlement_Rate == null ? 0 : Math.Round(Convert.ToDouble(cashDetails.Settlement_Rate), 2);
                cmdSaveAcData.Parameters.Add("@Prin_Setl_Amount", SqlDbType.Float).Value = cashDetails.PrinSettlement_Amount == null ? 0 : Math.Round(Convert.ToDouble(cashDetails.PrinSettlement_Amount), 2);
                cmdSaveAcData.Parameters.Add("@Transaction_Sentdate", SqlDbType.VarChar).Value = cashDetails.Transaction_SentDate == null ? "" : cashDetails.Transaction_SentDate.Trim();
                cmdSaveAcData.Parameters.Add("@Remitter_Nationality", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@Remitter_Dob", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@Remitter_City", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@Remitter_Telno", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@Remitter_Mobileno", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@Beneficiary_Nationality", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@Confirm_Download_Txn", SqlDbType.VarChar).Value = "";
                cmdSaveAcData.Parameters.Add("@Txn_Receive_Date", SqlDbType.DateTime).Value = DateTime.Now;
                cmdSaveAcData.Parameters.Add("@Txn_Status", SqlDbType.VarChar).Value = "RECEIVED";

                cmdSaveAcData.Parameters.Add("@Payment_Mode", SqlDbType.VarChar).Value = "CASH";
                cmdSaveAcData.Parameters.Add("@Downloadbranch", SqlDbType.VarChar).Value = downloadBranch;
                cmdSaveAcData.Parameters.Add("@Downloaduser", SqlDbType.VarChar).Value = downloadUser;
                cmdSaveAcData.Parameters.Add("@Clearingdate", SqlDbType.DateTime).Value = DateTime.Now;
                cmdSaveAcData.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = "";

                try
                {
                    int k = cmdSaveAcData.ExecuteNonQuery();
                    stats = true;
                }
                catch (Exception ex)
                {
                    InsertAutoFetchLog(userId, "InsertIntoICDataTable", "Error ! InsertIntoICDataTable" + ex.ToString());
                    stats = false;
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
            }
            return stats;
        }

        internal void UpdateConfirmDownloadAccountCreditTxnICTCTable(string ICTC_Number, string confirmFlag, string remarks)
        {
            SqlConnection connNewSystem = null;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                connNewSystem.Open();

                string query = "";
                if (confirmFlag.Equals("D"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[ICTCRequestData] SET [CONFIRM_DOWNLOAD_TXN]='" + confirmFlag + "' WHERE [ICTC_NUMBER]='" + ICTC_Number + "'";
                }
                else if (confirmFlag.Equals("Y"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[ICTCRequestData] SET [CONFIRM_DOWNLOAD_TXN]='" + confirmFlag + "', [TXN_STATUS]='PAID', [TXN_PAYMENT_DATE]=getdate()  WHERE [ICTC_NUMBER]='" + ICTC_Number + "'";
                }
                else if (confirmFlag.Equals("U"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[ICTCRequestData] SET [CONFIRM_DOWNLOAD_TXN]='" + confirmFlag + "', [TXN_STATUS]='UNLOCK', [REMARKS]='" + remarks + "'  WHERE [ICTC_NUMBER]='" + ICTC_Number + "'";
                }
                else
                {
                    query = "UPDATE [RemittanceDB].[dbo].[ICTCRequestData] SET [CONFIRM_DOWNLOAD_TXN]='" + confirmFlag + "', [TXN_STATUS]='ERROR', [REMARKS]='" + remarks + "'  WHERE [ICTC_NUMBER]='" + ICTC_Number + "'";
                }

                SqlCommand _cmd = new SqlCommand(query, connNewSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
            }
        }

        internal void MoveICTCDataIntoRemitInfoTable(string icNum, int idType, string idNumber, string mobileNum, string kycAddrs, string downloadUser)
        {
            try
            {
                SqlConnection connNewSystem = null;
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                SqlCommand cmdSaveData = new SqlCommand();

                if (connNewSystem.State.Equals(ConnectionState.Closed))
                {
                    connNewSystem.Open();
                }

                cmdSaveData.CommandType = CommandType.StoredProcedure;
                cmdSaveData.CommandText = "ICTCSpCashDataMoveToRemitInfoTable";
                cmdSaveData.Connection = connNewSystem;

                cmdSaveData.Parameters.Add("@reference", SqlDbType.VarChar).Value = icNum;
                cmdSaveData.Parameters.Add("@idType", SqlDbType.Int).Value = idType;
                cmdSaveData.Parameters.Add("@idNumber", SqlDbType.VarChar).Value = idNumber;
                cmdSaveData.Parameters.Add("@mobileNumber", SqlDbType.VarChar).Value = mobileNum;
                cmdSaveData.Parameters.Add("@kycAddress", SqlDbType.VarChar).Value = kycAddrs;
                cmdSaveData.Parameters.Add("@downloadUser", SqlDbType.VarChar).Value = downloadUser;

                try
                {
                    int k = cmdSaveData.ExecuteNonQuery();
                }
                catch (Exception ex) { }
                finally
                {
                    if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
                }
            }
            catch (Exception exc) { }
        }

        internal DataTable GetICTCPaidTxnList(string dtValueFrm, string dtValueTo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string queryData = "SELECT [ICTC_NUMBER],[REMITTER_NAME] REMITTER,[BENEFICIARY_NAME] BENEFICIARY,[PAYING_AMOUNT] AMOUNT,[PAYMENT_MODE] MODE,[TXN_STATUS] STATUS,"
                    +" [TXN_PAYMENT_DATE] PAYMENT_DATE,[TXN_RECEIVE_DATE] RECEIVE_DATE,[ORIGINATING_COUNTRY] ORG_COUNTRY,[BENEFICIARY_ADDRESS], "
                    + " [PURPOSE_REMIT],[TRANSACTION_SENTDATE] SENTDATE, [REMARKS],[CANCEL_DATE] "
                    + " FROM [RemittanceDB].[dbo].[ICTCRequestData] WHERE convert(date, [TXN_PAYMENT_DATE]) BETWEEN '" + dtValueFrm + "' AND '" + dtValueTo + "' "
                    + " AND PAYMENT_MODE='CASH' AND TXN_STATUS='PAID' "
                    + " order by AutoId desc";
                SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }


        internal DataTable GetPullAPIBkashTxnByExhPayMode(string exh, string payMode, string fromDate, string toDate)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                if (exh.Equals("NBL"))
                {
                    if (payMode.Equals("bkash"))
                    {
                        string queryData = "SELECT AutoId SL, case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end Party, REFERENCE, "
                            + " REMITTER_CONTACT REMITTER, BENEFICIARY_NAME BENEFICIARY, ACCOUNT_NO, AMOUNT BdtAmt, TXN_RECEIVE_DATE RECV_DATE, "
                            + " TXN_STATUS STATUS, TXN_PAYMENT_DATE PAY_DATE,[CONFIRM_DOWNLOAD_TXN] CONF, remarks "
                            + " FROM [RemittanceDB].[dbo].[NBLRequestData] "
                            + " where PAYMENT_MODE='BKASH' AND convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' order by [AutoID] desc";
                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("mtbac"))
                    {
                        string queryData = "SELECT [AutoId] SL,case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end Party,[REFERENCE],[TXN_STATUS] STATUS,"
                            + " [AMOUNT],[TXN_RECEIVE_DATE] RCV_DATE,[TXN_PAYMENT_DATE] PAY_DATE,"
                            + " [BENEFICIARY_NAME] BENEFICIAY,[REMITTER_NAME] REMITTER,[ACCOUNT_NO],[BRANCH_NAME],[CONFIRM_DOWNLOAD_TXN] CONF, remarks "
                            + " FROM [RemittanceDB].[dbo].[NBLRequestData] "
                            + " where PAYMENT_MODE='OWNBANK' AND convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' order by [AutoID] desc";
                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("beftn"))
                    {
                        string queryData = "SELECT [AutoId] SL,case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end Party,[REFERENCE],[TXN_STATUS] STATUS, "
                            + " [AMOUNT],[TXN_RECEIVE_DATE] RCV_DATE,[TXN_PAYMENT_DATE] PAY_DATE,[BENEFICIARY_NAME] BENEFICIAY,[REMITTER_NAME] REMITTER,[ACCOUNT_NO],"
                            + " [BANK_NAME],[BRANCH_NAME],[CONFIRM_DOWNLOAD_TXN] CONF FROM [RemittanceDB].[dbo].[NBLRequestData]"
                            + " where PAYMENT_MODE='BEFTN' AND convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' order by [AutoID] desc";
                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("cash"))
                    {
                        string queryData = "SELECT [AutoId] SL, case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end Party,[REFERENCE], [REMITTER_NAME],[BENEFICIAY_NAME],[BDT_AMOUNT] "
                            + " ,[RATE],[CURRENCY_NAME] CURR,[TRX_TOKEN],[TXN_RECEIVE_DATE],[MOBILE_NUMBER],[TXN_STATUS],[TXN_PAYMENT_DATE] "
                            + " FROM [RemittanceDB].[dbo].[NBLDataForCashTxn] "
                            + " where convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' order by [AutoID] desc";
                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                }
                else if (exh.Equals("ICTC")) // ICTC
                {
                    if (payMode.Equals("bkash"))
                    {

                    }
                    else if (payMode.Equals("mtbac"))
                    {
                        string queryData = "SELECT ROW_NUMBER() OVER(order by [AutoId]) AS SL, ICTC_NUMBER PIN, TXN_STATUS STATUS, DELIVERY_MODE MODE, PAYING_AMOUNT AMOUNT, "
                        + " BENEFICIARY_NAME BENE, BANK_ACCOUNT_NUMBER ACCOUNT_NUM, CONFIRM_DOWNLOAD_TXN CONF, TXN_RECEIVE_DATE RECEIVED, TXN_PAYMENT_DATE PAYMENT, REMARKS, CANCEL_DATE "
                        + " FROM [RemittanceDB].[dbo].[ICTCRequestData] "
                        + " WHERE convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' AND PAYMENT_MODE='OWNBANK' order by [AutoId] desc";

                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("beftn"))
                    {
                        string queryData = "SELECT ROW_NUMBER() OVER(order by [AutoId]) AS SL, ICTC_NUMBER PIN, TXN_STATUS STATUS, DELIVERY_MODE MODE, PAYING_AMOUNT AMOUNT,"
                            + " BENEFICIARY_NAME BENE, ORIGINATING_COUNTRY ORG_CNTY,  CONFIRM_DOWNLOAD_TXN CONF, transaction_sentdate SENTDATE, TXN_RECEIVE_DATE RECEIVED, TXN_PAYMENT_DATE PAYMENT, REMARKS, CANCEL_DATE  "
                            + " FROM [RemittanceDB].[dbo].[ICTCRequestData] "
                            + " WHERE convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' AND PAYMENT_MODE='BEFTN' order by [AutoId] desc";

                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("cash"))
                    {
                        string queryData = "SELECT ROW_NUMBER() OVER(order by [AutoId]) AS SL, ICTC_NUMBER PIN, TXN_STATUS STATUS, DELIVERY_MODE MODE, PAYING_AMOUNT AMOUNT,"
                            + " BENEFICIARY_NAME BENE, ORIGINATING_COUNTRY ORG_CNTY,  CONFIRM_DOWNLOAD_TXN CONF, transaction_sentdate SENTDATE, TXN_RECEIVE_DATE RECEIVED, TXN_PAYMENT_DATE PAYMENT, REMARKS, CANCEL_DATE  "
                            + " FROM [RemittanceDB].[dbo].[ICTCRequestData] "
                            + " where convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' AND PAYMENT_MODE='CASH' order by [AutoId] desc";

                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("invalid"))
                    {
                        string queryData = "SELECT ROW_NUMBER() OVER(order by [AutoId]) AS SL, ICTC_NUMBER PIN, TXN_STATUS STATUS, DELIVERY_MODE MODE, PAYING_AMOUNT AMOUNT,"
                            + " BENEFICIARY_NAME BENE, BANK_NAME,  CONFIRM_DOWNLOAD_TXN CONF, transaction_sentdate SENTDATE, TXN_RECEIVE_DATE RECEIVED, TXN_PAYMENT_DATE PAYMENT, REMARKS, CANCEL_DATE  "
                            + " FROM [RemittanceDB].[dbo].[ICTCRequestData] "
                            + " where convert(date, TXN_RECEIVE_DATE) between '" + fromDate + "' AND '" + toDate + "' AND PAYMENT_MODE='INVALID' order by [AutoId] desc";

                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                }
                else if (exh.Equals("GCC"))
                {
                    if (payMode.Equals("bkash"))
                    {
                        string queryData = "SELECT [AutoId] SL,[TransactionNo],[AmountToPay] Amount,[TxnStatus],[PaymentMode],[TxnReceiveDate] "
                            + " ,[TxnPaymentDate],[ReceiverName],[ReceiverAddress],[ReceiverContactNo],[SenderName],[SendCountryName],[SenderIncomeSource],[SenderOccupation],[BankAccountNo],[BankName] "
                            + " ,[BankBranchName],[BankBranchCode],[SentDate],[PayinCurrencyCode],[PayinAmount],[GccPayMode],[Remarks],[CancelDate] "
                            + " FROM [RemittanceDB].[dbo].[GCCRequestData]  where convert(date, TxnReceiveDate) between '" + fromDate + "' AND '" + toDate + "' AND [PaymentMode]='BKASH' order by [AutoId] desc";

                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("mtbac"))
                    {
                        string queryData = "SELECT [AutoId] SL,[TransactionNo],[AmountToPay] Amount,[TxnStatus],[PaymentMode],[TxnReceiveDate] "
                            + " ,[TxnPaymentDate],[ReceiverName],[ReceiverAddress],[ReceiverContactNo],[SenderName],[SendCountryName],[SenderIncomeSource],[SenderOccupation],[BankAccountNo],[BankName] "
                            + " ,[BankBranchName],[BankBranchCode],[SentDate],[PayinCurrencyCode],[PayinAmount],[GccPayMode],[Remarks],[CancelDate] "
                            + " FROM [RemittanceDB].[dbo].[GCCRequestData]  where convert(date, TxnReceiveDate) between '" + fromDate + "' AND '" + toDate + "' AND [PaymentMode]='OWNBANK' order by [AutoId] desc";

                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("beftn"))
                    {
                        string queryData = "SELECT [AutoId] SL,[TransactionNo],[AmountToPay] Amount,[TxnStatus],[PaymentMode],[TxnReceiveDate] "
                            + " ,[TxnPaymentDate],[ReceiverName],[ReceiverAddress],[ReceiverContactNo],[SenderName],[SendCountryName],[SenderIncomeSource],[SenderOccupation],[BankAccountNo],[BankName] "
                            + " ,[BankBranchName],[BankBranchCode],[SentDate],[PayinCurrencyCode],[PayinAmount],[GccPayMode],[Remarks],[CancelDate] "
                            + " FROM [RemittanceDB].[dbo].[GCCRequestData]  where convert(date, TxnReceiveDate) between '" + fromDate + "' AND '" + toDate + "' AND [PaymentMode]='BEFTN' order by [AutoId] desc";
                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                    else if (payMode.Equals("cash"))
                    {
                        string queryData = "SELECT [AutoId] SL,[TransactionNo],[AmountToPay] Amount,[TxnStatus],[PaymentMode],[TxnReceiveDate] "
                            + " ,[TxnPaymentDate],[ReceiverName],[ReceiverAddress],[ReceiverContactNo],[SenderName],[SendCountryName],[SenderIncomeSource],[SenderOccupation],[BankAccountNo],[BankName] "
                            + " ,[BankBranchName],[BankBranchCode],[SentDate],[PayinCurrencyCode],[PayinAmount],[GccPayMode],[Remarks],[CancelDate] "
                            + " FROM [RemittanceDB].[dbo].[GCCRequestData]  where convert(date, TxnReceiveDate) between '" + fromDate + "' AND '" + toDate + "' AND [PaymentMode]='CASH' order by [AutoId] desc";
                        SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                        SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                        sdaTxn.Fill(dt);
                    }
                }
                else
                { }
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetAPIDataAutoFetchLog(string fromDate, string toDate, string userId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                if (userId.Equals(""))
                {
                    string queryData = "SELECT [AutoId] SL,[UserId], convert(varchar, [RequestTime], 120)RequestTime, [MethodName],[ResponseMessage] "
                        + " FROM [RemittanceDB].[dbo].[APIDataAutoFetchLog] "
                        + " WHERE convert(date, RequestTime) between '" + fromDate + "' AND '" + toDate + "' order by AutoId desc ";

                    SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                    SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                    sdaTxn.Fill(dt);
                }
                else
                {
                    string queryData = "SELECT [AutoId] SL,[UserId], convert(varchar, [RequestTime], 120)RequestTime, [MethodName],[ResponseMessage] "
                        + " FROM [RemittanceDB].[dbo].[APIDataAutoFetchLog] "
                        + " WHERE [UserId]='" + userId + "' AND  convert(date, RequestTime) between '" + fromDate + "' AND '" + toDate + "' order by AutoId desc ";

                    SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                    SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                    sdaTxn.Fill(dt);
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable LoadManualEFTExhouseList()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = " SELECT ltrim(rtrim(STR([PartyId])))+' - '+[ExchangeHouseName] FROM [NRBWork].[dbo].[ManualEFTBasedExchangeHouseInfo] WHERE [isActive]=1";
                SqlDataAdapter sdaLst = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaLst.Fill(dt);
            }
            catch (Exception exc) { }
            return dt;
        }

        internal DataTable GetBranchNameByRoutingCode(string routingNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string sqlQuery = "SELECT [MTB Sl No],[MTB Code],[Bank Code],[Agent Name],[Branch Name],[City Name],[District],[Routing Number],[Country] "
                    + " FROM [RemittanceDB].[dbo].[BANK_BRANCH] WHERE [Routing Number]='" + routingNo + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                try { if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return dt;
        }

        internal DataTable GetAuthorizerAndSuperAdminEmailList()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT [UserId],[UserRMCode],[UserFullName],[UserEmail],[FileProcessUserType] FROM [NRBWork].[dbo].[WEB_UserInfo] WHERE ([FileProcessUserType]='Authorizer' OR [FileProcessUserType]='Admin' OR [FileProcessUserType]='SuperAdmin') AND [isActive]=1 AND [IsMailReceive]='Y'";
                SqlDataAdapter sdaLst = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaLst.Fill(dt);
            }
            catch (Exception exc) { }
            return dt;
        }

        internal bool IsThisTransactionExistBeforeManualEFTProcess(int exhId, string refNo)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT * FROM [NRBWork].[dbo].[ManualEFTFileData] WHERE [PartyId]=" + exhId + " AND [RefNo]='" + refNo + "' AND [Status] IN('PROCESSED','RECEIVED') ";
                SqlDataAdapter sdaLst = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaLst.Fill(dt);
            }
            catch (Exception exc) { }

            if (dt.Rows.Count > 0)
                return true; // txn exist before
            return false; // txn not exist, go forward
        }

        internal bool SaveExhDataManualEFTProcess(int exhId, DataRow dataRow, string loggedUser, string BatchId)
        {
            SqlConnection connNRBWorkSystem = null;
            string refNo, BeneficiaryName, AccountNo, BankName, BranchName, RoutingNo, BeneficiaryAddress, RemitterName, RemitterAddress, Purpose, PayMode, BeneficiaryContactNo;
            Double Amount;
            string saveData = "";

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                SqlCommand cmdSaveData = new SqlCommand();

                refNo = Convert.ToString(dataRow["RefNo"]);
                BeneficiaryName = Convert.ToString(dataRow["BeneficiaryName"]);
                AccountNo = Convert.ToString(dataRow["AccountNo"]);
                BankName = Convert.ToString(dataRow["BankName"]);
                BranchName = Convert.ToString(dataRow["BranchName"]);
                RoutingNo = Convert.ToString(dataRow["RoutingNo"]);
                Amount = Math.Round(Convert.ToDouble(dataRow["Amount"]), 2);
                BeneficiaryAddress = Convert.ToString(dataRow["BeneficiaryAddress"]);
                RemitterName = Convert.ToString(dataRow["RemitterName"]);
                RemitterAddress = Convert.ToString(dataRow["RemitterAddress"]);
                Purpose = Convert.ToString(dataRow["Purpose"]);
                PayMode = Convert.ToString(dataRow["PayMode"]);
                BeneficiaryContactNo = Convert.ToString(dataRow["BeneficiaryContactNo"]);

                saveData = "INSERT INTO [dbo].[ManualEFTFileData]([PartyId],[RefNo],[PaymentMode],[BeneficiaryName],[BeneficiaryAddress],[BeneficiaryAccountNo],[BankName],"
                    + " [BranchName],[RoutingNo],[Amount],[SenderName],[SenderAddress],[Purpose],[UplodeBy],[UploadTime],[IsSuccess],[Status],[BeneficiaryContactNo],[BatchId])"
                    + " VALUES(@PartyId,@RefNo,@PaymentMode,@BeneficiaryName,@BeneficiaryAddress,@BeneficiaryAccountNo,@BankName,@BranchName,@RoutingNo,@Amount,"
                    + " @SenderName,@SenderAddress,@Purpose,@UplodeBy,@UploadTime,@IsSuccess,@Status,@BeneficiaryContactNo,@BatchId)";

                cmdSaveData = new SqlCommand();
                cmdSaveData.CommandText = saveData;
                cmdSaveData.Connection = connNRBWorkSystem;

                cmdSaveData.Parameters.Add("@PartyId", SqlDbType.Int).Value = exhId;
                cmdSaveData.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = (refNo == null ? "" : refNo.ToString().Trim());
                cmdSaveData.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = (PayMode == null ? "" : PayMode.ToString().Trim());
                cmdSaveData.Parameters.Add("@BeneficiaryName", SqlDbType.VarChar).Value = (BeneficiaryName == null ? "" : BeneficiaryName.ToString().Trim());
                cmdSaveData.Parameters.Add("@BeneficiaryAddress", SqlDbType.VarChar).Value = (BeneficiaryAddress == null ? "" : BeneficiaryAddress.ToString().Trim());
                cmdSaveData.Parameters.Add("@BeneficiaryAccountNo", SqlDbType.VarChar).Value = (AccountNo == null ? "" : AccountNo.ToString().Trim());
                cmdSaveData.Parameters.Add("@BankName", SqlDbType.VarChar).Value = (BankName == null ? "" : BankName.ToString().Trim());
                cmdSaveData.Parameters.Add("@BranchName", SqlDbType.VarChar).Value = (BranchName == null ? "" : BranchName.ToString().Trim());
                cmdSaveData.Parameters.Add("@RoutingNo", SqlDbType.VarChar).Value = (RoutingNo == null ? "" : RoutingNo.ToString().Trim());
                cmdSaveData.Parameters.Add("@Amount", SqlDbType.Float).Value = Amount;
                cmdSaveData.Parameters.Add("@SenderName", SqlDbType.VarChar).Value = (RemitterName == null ? "" : RemitterName.ToString().Trim());
                cmdSaveData.Parameters.Add("@SenderAddress", SqlDbType.VarChar).Value = (RemitterAddress == null ? "" : RemitterAddress.ToString().Trim());
                cmdSaveData.Parameters.Add("@Purpose", SqlDbType.VarChar).Value = (Purpose == null ? "" : (Purpose.ToString().Trim().Length > 100 ? Purpose.ToString().Trim().Substring(0, 95) : Purpose.ToString().Trim()));
                cmdSaveData.Parameters.Add("@BeneficiaryContactNo", SqlDbType.VarChar).Value = (BeneficiaryContactNo == null ? "" : (BeneficiaryContactNo.ToString().Trim().Length > 50 ? BeneficiaryContactNo.ToString().Trim().Substring(48) : BeneficiaryContactNo.ToString().Trim()));
                cmdSaveData.Parameters.Add("@UplodeBy", SqlDbType.VarChar).Value = (loggedUser == null ? "" : loggedUser.ToString().Trim());
                cmdSaveData.Parameters.Add("@UploadTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmdSaveData.Parameters.Add("@IsSuccess", SqlDbType.Bit).Value = true;
                cmdSaveData.Parameters.Add("@Status", SqlDbType.VarChar).Value = "UPLOADED";
                cmdSaveData.Parameters.Add("@BatchId", SqlDbType.VarChar).Value = BatchId;

                try
                {
                    cmdSaveData.ExecuteNonQuery();
                }
                catch (Exception ec)
                {
                    return false;
                }
                finally
                {
                    try { if (connNRBWorkSystem != null && connNRBWorkSystem.State == ConnectionState.Open) { connNRBWorkSystem.Close(); } }
                    catch (SqlException sqlException) { throw sqlException; }
                }
            }
            catch (Exception exc) { }

            return true;
        }

        internal void ChangeStatusFromUploadedToReceivedManualEFTProcess(string ticks)
        {
            SqlConnection connNRBWorkSystem = null;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "UPDATE [NRBWork].[dbo].[ManualEFTFileData] SET [Status]='RECEIVED'  WHERE [BatchId]='" + ticks + "'";
                SqlCommand _cmd = new SqlCommand(query, connNRBWorkSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNRBWorkSystem != null && connNRBWorkSystem.State == ConnectionState.Open) { connNRBWorkSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
        }

        internal DataTable GetSummaryTellerUploadedRecordManualEFTProcess(string ticks)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT PaymentMode, count(*) no_of_txn,  round(sum(amount),2) TotalAmount FROM [NRBWork].[dbo].[ManualEFTFileData] WHERE  [BatchId]='" + ticks + "' group by PaymentMode";
                SqlDataAdapter sdaLst = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaLst.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                try { if (connNRBWorkSystem != null && connNRBWorkSystem.State == ConnectionState.Open) { connNRBWorkSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return dt;
        }

        internal DataTable GetBEFTNDuplicateTxn(string fromdt, string todt, string opsType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                if (opsType.Equals("SEARCH"))
                {
                    query = "select p.Exch, p.ReferenceNo PIN_No, p.ExchAcc, p.BeneAcc, p.RoutingNo, "
                        + " (select top 1 [Agent Name] from [RemittanceDB].[dbo].[BANK_BRANCH] where [Bank Code]=p.bankCode) BankName, "
                        + " p.Amount, convert(varchar, p.TransactionDate, 20)TransDate, p.RequestData, p.ResponseData, p.JournalNoBEFTN JournalNo, p.LogId, p.Status "
                        + " FROM ( "
                        + " select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=aa.PartyId) Exch, "
                        + " aa.ReferenceNo, aa.CrAccountNo ExchAcc, aa.DrAccountNo BeneAcc, (select br.DestinationRoutingNO from [RemittanceDB].[dbo].BEFTNRequest br where br.AutoId=aa.BEFTNRequestid)RoutingNo, "
                        + " SUBSTRING((select br.DestinationRoutingNO from [RemittanceDB].[dbo].BEFTNRequest br where br.AutoId=aa.BEFTNRequestid), 1, 3) bankCode, "
                        + " aa.Amount, aa.TransactionDate, aa.RequestData, aa.ResponseData, aa.JournalNoBEFTN, aa.LogId, aa.Status "
                        + " FROM [RemittanceDB].[dbo].BEFTNTransaction aa WHERE  aa.BEFTNRequestid in (  "
                            + " select bb.BEFTNRequestid from [RemittanceDB].[dbo].BEFTNTransaction bb  where bb.isSuccess = 1 and "
                            + " convert(varchar, bb.transactiondate, 23) between '" + fromdt + "' AND '" + todt + "' group by bb.BEFTNRequestid having count(*) > 1 ) "
                        + " )p "
                        + " order by ReferenceNo ";
                }
                else
                {
                    query = "select p.Exch, p.ReferenceNo PIN_No, p.ExchAcc, p.BeneAcc, p.RoutingNo, "
                        + " (select top 1 [Agent Name] from [RemittanceDB].[dbo].[BANK_BRANCH] where [Bank Code]=p.bankCode) BankName, "
                        + " p.Amount, convert(varchar, p.TransactionDate, 20)TransDate, p.JournalNoBEFTN JournalNo, p.LogId, p.Status "
                        + " FROM ( "
                        + " select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=aa.PartyId) Exch, "
                        + " aa.ReferenceNo, aa.CrAccountNo ExchAcc, aa.DrAccountNo BeneAcc, (select br.DestinationRoutingNO from [RemittanceDB].[dbo].BEFTNRequest br where br.AutoId=aa.BEFTNRequestid)RoutingNo, "
                        + " SUBSTRING((select br.DestinationRoutingNO from [RemittanceDB].[dbo].BEFTNRequest br where br.AutoId=aa.BEFTNRequestid), 1, 3) bankCode, "
                        + " aa.Amount, aa.TransactionDate, aa.JournalNoBEFTN, aa.LogId, aa.Status "
                        + " FROM [RemittanceDB].[dbo].BEFTNTransaction aa WHERE  aa.BEFTNRequestid in (  "
                            + " select bb.BEFTNRequestid from [RemittanceDB].[dbo].BEFTNTransaction bb  where bb.isSuccess = 1 and "
                            + " convert(varchar, bb.transactiondate, 23) between '" + fromdt + "' AND '" + todt + "' group by bb.BEFTNRequestid having count(*) > 1 ) "
                        + " )p "
                        + " order by ReferenceNo ";
                }

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetIndividualTxnByWhereClause(string exh, string whereClause)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                if (exh.Equals("ICTC"))
                {
                    string ictcQuery = "SELECT [AutoId] SL,[ICTC_NUMBER],[REMITTER_NAME] REMITTER,[ORIGINATING_COUNTRY] ORG_CNTR,[DELIVERY_MODE],[PAYING_AMOUNT],[PAYING_CURRENCY],[PAYING_AGENT],[PAYING_AGENTNAME] "
                        + " ,[BENEFICIARY_NAME] BENEF,[BENEFICIARY_ADDRESS] BENE_ADDR,[BENEFICIARY_MOBILENO] BENE_MOB,[BANK_ACCOUNT_NUMBER] ACC_NUM,[BANK_NAME],[PURPOSE_REMIT],[BANK_BRANCHCODE] RoutingNo "
                        + " ,[TRANSACTION_SENTDATE] SENTDATE,[CONFIRM_DOWNLOAD_TXN] CONFIRMD,[TXN_RECEIVE_DATE] RECEIVE_DT,[TXN_STATUS],[PAYMENT_MODE],[TXN_PAYMENT_DATE] PAYMENT_DT,[ClearingDate],[REMARKS],[CANCEL_DATE] "
                        + " FROM [RemittanceDB].[dbo].[ICTCRequestData] " + whereClause + " order by [AutoID] desc";

                    SqlDataAdapter sdaIC = new SqlDataAdapter(ictcQuery, connDRSystem);
                    sdaIC.Fill(dt);
                }
                else if (exh.Equals("GCC"))
                {
                    string gccQuery = "SELECT [AutoId] SL, [TransactionNo],[AmountToPay] Amount,[TxnStatus],[PaymentMode],[TxnReceiveDate] "
                        + " ,[TxnPaymentDate],[ReceiverName],[ReceiverAddress],[ReceiverContactNo],[SenderName],[SendCountryName],[SenderIncomeSource],[SenderOccupation],[BankAccountNo],[BankName] "
                        + " ,[BankBranchName],[BankBranchCode],[SentDate],[PayinCurrencyCode],[PayinAmount],[GccPayMode],[Remarks],[CancelDate] "
                        + " FROM [RemittanceDB].[dbo].[GCCRequestData] " + whereClause + " order by [AutoId] desc";

                    SqlDataAdapter sdaGC = new SqlDataAdapter(gccQuery, connDRSystem);
                    sdaGC.Fill(dt);
                }
                else if (exh.Equals("NBL"))
                {
                    string bkashQuery = "SELECT AutoId SL, 'bKash'TxnType, AGENT_CODE Agent, case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end PartyId, REFERENCE PIN, "
                        + " REMITTER_CONTACT Remitter, BENEFICIARY_NAME Beneficiary, ACCOUNT_NO AccountNo, AMOUNT Amount, TXN_RECEIVE_DATE RecvDate, "
                        + " TXN_STATUS Status, TXN_PAYMENT_DATE PayDate, Remarks "
                        + " FROM [RemittanceDB].[dbo].[NBLRequestData] " + whereClause + " and PAYMENT_MODE='BKASH' order by [AutoID] desc";

                    SqlDataAdapter sdaBkash = new SqlDataAdapter(bkashQuery, connDRSystem);
                    sdaBkash.Fill(dt);

                    if (dt.Rows.Count < 1)
                    {
                        dt = new DataTable();
                        string mtbQuery = "SELECT [AutoId] SL,'MTBAc' TxnType, AGENT_CODE Agent, case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end PartyId, "
                            + " REFERENCE PIN,[TXN_STATUS] Status,[ACCOUNT_NO] AccountNo,[REMITTER_NAME] Remitter,[BENEFICIARY_NAME] Beneficiary,[AMOUNT] Amount,[BANK_NAME] Bank,[BRANCH_NAME] Branch "
                            + " ,[SOURCE_OF_FUND] FundSource,[CONFIRM_DOWNLOAD_TXN] Confirmed,[TXN_RECEIVE_DATE] RecvDate "
                            + " ,[TXN_PAYMENT_DATE] PayDate,[remarks] Remarks,[cancel_date] CancelDate "
                            + " FROM [RemittanceDB].[dbo].[NBLRequestData] " + whereClause + " AND PAYMENT_MODE='OWNBANK' order by [AutoId] desc";
                        SqlDataAdapter sdaMtb = new SqlDataAdapter(mtbQuery, connDRSystem);
                        sdaMtb.Fill(dt);

                        if (dt.Rows.Count < 1)
                        {
                            dt = new DataTable();
                            string beftnQuery = "SELECT [AutoId] SL,'BEFTN' TxnType, AGENT_CODE Agent, case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end PartyId, [REFERENCE] PIN,[TXN_STATUS] Status,[REMITTER_NAME] Remitter,[BENEFICIARY_NAME] Beneficiary,[ACCOUNT_NO] AccountNo "
                                + " ,[BANK_NAME] Bank,[BRANCH_NAME] Branch,[ROUTING_NUMBER] Routing,[AMOUNT] Amount,[SOURCE_OF_FUND] FundSource,[CONFIRM_DOWNLOAD_TXN] Confirmed,[TXN_RECEIVE_DATE] RecvDate "
                                + " ,[TXN_PAYMENT_DATE] PayDate FROM [RemittanceDB].[dbo].[NBLRequestData] " + whereClause + " AND PAYMENT_MODE='BEFTN' order by [AutoId] desc";
                            SqlDataAdapter sdaBeftn = new SqlDataAdapter(beftnQuery, connDRSystem);
                            sdaBeftn.Fill(dt);

                            if (dt.Rows.Count < 1)
                            {
                                dt = new DataTable();
                                string cashQuery = "SELECT [AutoId] SL,'Cash' TxnType, AGENT_CODE Agent, case when AGENT_CODE ='065' then 'Singapore' else 'Malaysia' end PartyId, [REFERENCE] PIN,[TXN_STATUS] Status,[REMITTER_NAME] Remitter,[BENEFICIARY_NAME] Beneficiary, "
                                    + " [BDT_AMOUNT] Amount,[SENT_ON],[TXN_RECEIVE_DATE] RecvDate, [TXN_PAYMENT_DATE] PayDate, [ID_NUMBER] IdNumber,[MOBILE_NUMBER] MobileNo,[BEN_ADDRESS] BenAddr "
                                    + " FROM [RemittanceDB].[dbo].[NBLDataForCashTxn] " + whereClause + " order by [AutoId] desc";
                                SqlDataAdapter sdaCash = new SqlDataAdapter(cashQuery, connDRSystem);
                                sdaCash.Fill(dt);
                            }
                        }
                    }

                }
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetRemitCertificateDataFromSystem(string p_sys, string accountNo, string dtValueFrom, string dtValueTo, string payMode)
        {
            DataTable dt = new DataTable();

            //SqlConnection connOldSystem = null;
            SqlConnection connDRSystem = null;

            if (payMode.Equals("BEFTN"))
            {
                if (p_sys.Equals("OLD"))
                {
                    /*
                    try
                    {
                        connOldSystem = new SqlConnection(connInfo.getOldConnString());
                        connOldSystem.Open();

                        string query = "select ROW_NUMBER() OVER (ORDER BY RequestTime) SL, convert(varchar, b.RequestTime, 104) RequestDate, convert(varchar, b.UploadTime, 104) CreditDate, "
                            + " b.RefNo ReferenceNo, REPLACE((select userid from [RemittanceDB].[dbo].Users u where u.PartyId=b.PartyId),'Live','') ExchangeHouse, b.SenderName, b.Amount, "
                            + " b.BeneficiaryName Beneficiary, b.BeneficiaryAccountNo AccountNo, b.BeneficiaryBankName Bank, b.BeneficiaryBankBranchName Branch, b.DestinationRoutingNO RoutingNo "
                            + " FROM [RemittanceDB].[dbo].BEFTNRequest b where RequestTime between '" + dtValueFrom + "' and '" + dtValueTo + "' "
                            + " and b.PaymentStatus=5 "
                            + " and BeneficiaryAccountNo like '%" + accountNo + "%' "
                            + " order by RequestTime asc";

                        SqlCommand cmd = new SqlCommand(query, connOldSystem);
                        SqlDataAdapter sdaBEFTN = new SqlDataAdapter(cmd);
                        sdaBEFTN.Fill(dt);
                    }
                    catch (Exception exc) { }

                    */
                }
                else  // new system BEFTN
                {
                    try
                    {
                        connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                        connDRSystem.Open();

                        string query = "select ROW_NUMBER() OVER (ORDER BY RequestTime) SL, convert(varchar, b.RequestTime, 104) RequestDate,  "
                            + " case when b.UplodedBy='AutoProcess' then  convert(varchar, b.LastProcessingTime, 104) else convert(varchar, b.UploadTime, 104) end CreditDate,"
                            + " b.RefNo ReferenceNo, REPLACE((select userid from [RemittanceDB].[dbo].Users u where u.PartyId=b.PartyId),'Live','') ExchangeHouse, b.SenderName, b.Amount, "
                            + " b.BeneficiaryName Beneficiary, b.BeneficiaryAccountNo AccountNo, b.BeneficiaryBankName Bank, b.BeneficiaryBankBranchName Branch, b.DestinationRoutingNO RoutingNo "
                            + " FROM [RemittanceDB].[dbo].BEFTNRequest b where RequestTime between '" + dtValueFrom + "' and '" + dtValueTo + "' "
                            + " and b.PaymentStatus=5 and b.IsIncentive in(0,99) "
                            + " and BeneficiaryAccountNo like '%" + accountNo + "%' "
                            + " order by RequestTime asc";

                        SqlDataAdapter sdaNewSys = new SqlDataAdapter(query, connDRSystem);
                        sdaNewSys.Fill(dt);
                        return dt;
                    }
                    catch (Exception exc) { }
                }
            }//BEFTN paymode if end
            else   // MTB Ac paymode start
            {
                if (p_sys.Equals("OLD"))
                {
                    /*
                    try
                    {
                        connOldSystem = new SqlConnection(connInfo.getOldConnString());
                        connOldSystem.Open();

                        string query = "select ROW_NUMBER() OVER (ORDER BY TransDate) SL, convert(varchar, ft.TransDate, 104) RequestDate, convert(varchar, ft.TransDate, 104) CreditDate, ft.RefNo ReferenceNo, "
                        + " REPLACE((select userid from [RemittanceDB].[dbo].Users u where u.PartyId=ft.PartyId),'Live','') ExchangeHouse, ft.SenderName, ft.TrAmount Amount, "
                        + " ft.BeneficiaryName Beneficiary, ft.BeneficiaryAccountNo AccountNo, 'MTB' Bank, '' Branch, '' RoutingNo "
                        + " FROM [RemittanceDB].[dbo].FundTransferRequest ft where ft.TransDate between '" + dtValueFrom + "' and '" + dtValueTo + "'"
                        + " and ft.PaymentStatus=5 "
                        + " and ft.BeneficiaryAccountNo like '%" + accountNo + "%' "
                        + " order by ft.TransDate asc";

                        SqlCommand cmd = new SqlCommand(query, connOldSystem);
                        //cmd.CommandTimeout = 50;
                        SqlDataAdapter sdaAcCrd = new SqlDataAdapter(cmd);
                        sdaAcCrd.Fill(dt);
                    }
                    catch (Exception exc) { }
                    */
                }
                else
                {
                    try
                    {
                        connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                        connDRSystem.Open();

                        string query = "select ROW_NUMBER() OVER (ORDER BY TransDate) SL, convert(varchar, ft.TransDate, 104) RequestDate, convert(varchar, ft.TransDate, 104) CreditDate, ft.RefNo ReferenceNo, "
                        + " REPLACE((select userid from [RemittanceDB].[dbo].Users u where u.PartyId=ft.PartyId),'Live','') ExchangeHouse, ft.SenderName, ft.TrAmount Amount, "
                        + " ft.BeneficiaryName Beneficiary, ft.BeneficiaryAccountNo AccountNo, 'MTB' Bank, '' Branch, '' RoutingNo "
                        + " FROM [RemittanceDB].[dbo].FundTransferRequest ft where ft.TransDate between '" + dtValueFrom + "' and '" + dtValueTo + "'"
                        + " and ft.PaymentStatus=5 "
                        + " and ft.BeneficiaryAccountNo like '%" + accountNo + "%' "
                        + " order by ft.TransDate asc";

                        SqlDataAdapter sdaNewSys = new SqlDataAdapter(query, connDRSystem);
                        sdaNewSys.Fill(dt);
                        return dt;
                    }
                    catch (Exception exc) { }
                }
            }
            return dt;
        }


        internal DataTable GetMTBAcDuplicateTxn(string fromdt, string todt, string opsType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=tr.UserId)Exch, "
                    + " tr.SessionId PINNumber, tr.DrAccountNo ExchAccNum, tr.CrAccountNo CustAccNum, tr.Amount, convert(varchar, tr.TransactionDate, 23)TransDate, tr.Reason Remarks, tr.Status, tr.IsSuccess "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.SessionId in ( "
                    + " SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn "
                    + " where trn.[Type] IN ('101', '102', '201', '202') "
                    + " and convert(varchar, trn.TransactionDate, 23) between '" + fromdt + "' AND '" + todt + "' "
                    //+ " and trn.IsSuccess = 1 "
                    + " group by trn.USERID, trn.SessionId, trn.DrAccountNo "
                    + " having count(*)>1 "
                    + " ) "
                    + " order by tr.TransactionDate desc, tr.SessionId";

                //if (opsType.Equals("SEARCH"))
                //{                    
                //}
                //else
                //{                    
                //}

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBkashDuplicateTxn(string fromdt, string todt, string opsType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=tr.UserId)Exch, "
                    + " tr.SessionId PINNumber, tr.DrAccountNo ExchAccNum, tr.Amount, convert(varchar, tr.TransactionDate, 23)TransDate, tr.Reason Remarks, tr.Status, tr.IsSuccess "
                    + " , tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.SessionId in ( "
                    + " SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn "
                    + " where trn.[Type] IN ('105','205') "
                    + " and convert(varchar, trn.TransactionDate, 23) between '" + fromdt + "' AND '" + todt + "' "
                    + " and trn.IsSuccess = 1 "
                    + " group by trn.USERID, trn.SessionId,  trn.DrAccountNo "
                    + " having count(*)>1 "
                    + " ) "
                    + " order by tr.SessionId";

                //if (opsType.Equals("SEARCH"))
                //{                    
                //}
                //else
                //{                    
                //}

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetMTBAcSameBeneficiaryMultiTxn(string fromdt, string todt, string opsType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=trx.UserId)Exch, "
                       + " trx.SessionId PINNumber, trx.CrAccountNo CustAccNum, trx.Amount, convert(varchar, trx.TransactionDate, 23)TransDate, trx.Reason Remarks, trx.Status, "
                       + " trx.CBSTransactionCodeDr Journal, trx.CBSUniqueNumber, trx.CBSValueDate "
                    + " from [RemittanceDB].[dbo].transactions trx  "
                    + " where trx.CrAccountNo in "
                    + " ( "
                       + "  select t.CrAccountNo "
                       + "  from [RemittanceDB].[dbo].Transactions  t  "
                       + "  where t.Type IN ('101', '102', '201', '202')  "
                       + "  and convert(varchar, t.TransactionDate, 23) between '" + fromdt + "' AND '" + todt + "' "
                       + "  group by t.CrAccountNo "
                       + "  having count (1) > 1 "
                    + " ) "
                    + " and trx.Type IN ('101', '102', '201', '202') "
                    + " and convert(varchar, trx.TransactionDate, 23) between '" + fromdt + "' AND '" + todt + "' "
                    + " order by trx.CrAccountNo,trx.SessionId";

                //if (opsType.Equals("SEARCH"))
                //{                    
                //}
                //else
                //{                    
                //}

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool ConvertDirectModePendingStatusToRecvdStatus()
        {
            SqlConnection connNewSystem = null;
            bool updateStat = false;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                connNewSystem.Open();

                string query = "UPDATE [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] SET [StatusID]=1  WHERE [StatusID]=3";
                SqlCommand _cmd = new SqlCommand(query, connNewSystem);
                try
                {
                    _cmd.ExecuteNonQuery();
                    updateStat = true;
                    return updateStat;
                }
                catch (Exception exc)
                {
                    string err = exc.Message;
                }
                _cmd.Dispose();
            }
            catch (Exception ex) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            return updateStat;
        }

        internal DataTable GetUserWisePerformedTxnSummaryByDates(string fromDate, string toDate)
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string queryData = "select distinct dc.[PaymentUserId] UserId, (SELECT ui.[UserFullName] FROM [NRBWork].[dbo].[WEB_UserInfo] ui where ui.[UserRMCode]=dc.[PaymentUserId])UserName, count(*) No_of_Txn "
                + " FROM [NRBWork].[dbo].[CashTxnDuplicateCheck] dc where convert(varchar,dc.[TxnInsertDate], 23) BETWEEN '" + fromDate + "' AND '" + toDate + "' group by dc.[PaymentUserId] order by count(*) desc ";

                SqlCommand cmd = new SqlCommand(queryData, connNRBWorkSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }


        // EzRemit Save Data
        internal bool InsertCashDataIntoEzRemitDataTable(string userId, EzRemitServiceClient.CashTxnDetails cashDetails, string downloadBranch, string downloadUser)
        {
            bool stats = true;

            try
            {
                SqlConnection openCon = new SqlConnection(connInfo.getConnStringRemitLv());
                SqlCommand cmdSaveAcData = new SqlCommand();

                if (openCon.State.Equals(ConnectionState.Closed))
                {
                    openCon.Open();
                }

                cmdSaveAcData.CommandType = CommandType.StoredProcedure;
                cmdSaveAcData.CommandText = "EzRemitSpInsertCashTxnData";
                cmdSaveAcData.Connection = openCon;

                cmdSaveAcData.Parameters.Add("@ResponseCode", SqlDbType.VarChar).Value = cashDetails.ResponseCode;
                cmdSaveAcData.Parameters.Add("@ResponseMessage", SqlDbType.VarChar).Value = cashDetails.ResponseMessage;
                cmdSaveAcData.Parameters.Add("@SecurityNumber", SqlDbType.VarChar).Value = cashDetails.SecurityNumber;
                cmdSaveAcData.Parameters.Add("@TransactionDate", SqlDbType.VarChar).Value = cashDetails.TransactionDate == null ? "" : cashDetails.TransactionDate.Trim();
                cmdSaveAcData.Parameters.Add("@TransactionNumber", SqlDbType.VarChar).Value = cashDetails.TransactionNumber == null ? "" : cashDetails.TransactionNumber.Trim();
                cmdSaveAcData.Parameters.Add("@TransactionStatus", SqlDbType.VarChar).Value = cashDetails.TransactionStatus == null ? "" : cashDetails.TransactionStatus.Trim();
                cmdSaveAcData.Parameters.Add("@TypeOfTransaction", SqlDbType.VarChar).Value = cashDetails.TypeOfTransaction == null ? "" : cashDetails.TypeOfTransaction.Trim();
                
                cmdSaveAcData.Parameters.Add("@FxAmount", SqlDbType.Float).Value = Math.Round(Convert.ToDouble(cashDetails.TransactionPaymentDetails.FxAmount), 2);
                cmdSaveAcData.Parameters.Add("@FxCurrencyCode", SqlDbType.VarChar).Value = cashDetails.TransactionPaymentDetails.FxCurrencyCode == null ? "" : cashDetails.TransactionPaymentDetails.FxCurrencyCode.Trim();
                cmdSaveAcData.Parameters.Add("@LocalAmount", SqlDbType.VarChar).Value = cashDetails.TransactionPaymentDetails.LocalAmount == null ? "" : cashDetails.TransactionPaymentDetails.LocalAmount.ToString();
                cmdSaveAcData.Parameters.Add("@LocalCurrencyCode", SqlDbType.VarChar).Value = cashDetails.TransactionPaymentDetails.LocalCurrencyCode == null ? "" : cashDetails.TransactionPaymentDetails.LocalCurrencyCode.Trim();
                cmdSaveAcData.Parameters.Add("@MktRate", SqlDbType.VarChar).Value = cashDetails.TransactionPaymentDetails.MktRate == null ? "" : cashDetails.TransactionPaymentDetails.MktRate.ToString();
                cmdSaveAcData.Parameters.Add("@Rate", SqlDbType.VarChar).Value = cashDetails.TransactionPaymentDetails.Rate == null ? "" : cashDetails.TransactionPaymentDetails.Rate.ToString();
                cmdSaveAcData.Parameters.Add("@TotalLocalAmount", SqlDbType.VarChar).Value = cashDetails.TransactionPaymentDetails.TotalLocalAmount == null ? "" : cashDetails.TransactionPaymentDetails.TotalLocalAmount.ToString();
                  
                cmdSaveAcData.Parameters.Add("@SenderCustomerName", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustomerName == null ? "" : cashDetails.SendingCustomer.CustomerName.Trim();
                cmdSaveAcData.Parameters.Add("@SenderCustomerAddress", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustomerAddress == null ? "" : cashDetails.SendingCustomer.CustomerAddress.Trim();
                cmdSaveAcData.Parameters.Add("@SenderCustNationality", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustNationality.Trim() + " " + cashDetails.SendingCustomer.CustNationality.Trim();
                cmdSaveAcData.Parameters.Add("@SenderCustTelephoneNumber", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustTelephoneNumber == null ? "" : cashDetails.SendingCustomer.CustTelephoneNumber.Trim();
                cmdSaveAcData.Parameters.Add("@SenderCustMobileNumber", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustMobileNumber == null ? "" : cashDetails.SendingCustomer.CustMobileNumber.Trim();
                cmdSaveAcData.Parameters.Add("@SendingBranchAddress", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.BranchAddress == null ? "" : cashDetails.SendingCustomer.BranchAddress.Trim();
                cmdSaveAcData.Parameters.Add("@SendingContactPerson", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.ContactPerson == null ? "" : cashDetails.SendingCustomer.ContactPerson.Trim();
                cmdSaveAcData.Parameters.Add("@SendingCustBankName", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustBankName == null ? "" : cashDetails.SendingCustomer.CustBankName.Trim();
                //cmdSaveAcData.Parameters.Add("@CustBankBranchName", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustBankBranchName == null ? "" : cashDetails.SendingCustomer.CustBankBranchName.Trim();
                cmdSaveAcData.Parameters.Add("@SendingCustCountry", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustCountry == null ? "" : cashDetails.SendingCustomer.CustCountry.Trim();
                cmdSaveAcData.Parameters.Add("@SendingCustCountryCode", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustCountryCode == null ? "" : cashDetails.SendingCustomer.CustCountryCode.Trim();
                cmdSaveAcData.Parameters.Add("@SendingCustID", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustID == null ? "" : cashDetails.SendingCustomer.CustID.Trim();
                cmdSaveAcData.Parameters.Add("@CustIDType", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.CustIDType == null ? "" : cashDetails.SendingCustomer.CustIDType.Trim();
                cmdSaveAcData.Parameters.Add("@Relationship", SqlDbType.VarChar).Value = cashDetails.SendingCustomer.Relationship == null ? "" : cashDetails.SendingCustomer.Relationship.Trim();

                cmdSaveAcData.Parameters.Add("@BeneName", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.Name == null ? "" : cashDetails.TransactionBeneficiary.Name.Trim();
                cmdSaveAcData.Parameters.Add("@BeneNationality", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.Nationality == null ? "" : cashDetails.TransactionBeneficiary.Nationality.Trim();
                cmdSaveAcData.Parameters.Add("@BeneMobileNumber", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.MobileNumber == null ? "" : cashDetails.TransactionBeneficiary.MobileNumber.Trim();
                cmdSaveAcData.Parameters.Add("@BeneContactPerson", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.ContactPerson == null ? "" : cashDetails.TransactionBeneficiary.ContactPerson.Trim();
                cmdSaveAcData.Parameters.Add("@BeneContactTelephoneNo", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.ContactTelephoneNo == null ? "" : cashDetails.TransactionBeneficiary.ContactTelephoneNo.Trim();
                cmdSaveAcData.Parameters.Add("@BeneAddress", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.Address == null ? "" : cashDetails.TransactionBeneficiary.Address.Trim();
                cmdSaveAcData.Parameters.Add("@FundSource", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.FundSource == null ? "" : cashDetails.TransactionBeneficiary.FundSource.Trim();
                cmdSaveAcData.Parameters.Add("@Purpose", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.Purpose == null ? "" : cashDetails.TransactionBeneficiary.Purpose.Trim();
                cmdSaveAcData.Parameters.Add("@BeneBankName", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.BankName == null ? "" : cashDetails.TransactionBeneficiary.BankName.Trim();
                cmdSaveAcData.Parameters.Add("@BeneBranchName", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.BranchName.Trim() + " " + cashDetails.TransactionBeneficiary.BranchName.Trim();
                cmdSaveAcData.Parameters.Add("@BeneBranchAddress", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.BranchAddress == null ? "" : cashDetails.TransactionBeneficiary.BranchAddress.Trim();
                cmdSaveAcData.Parameters.Add("@BenCountry", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.BenCountry == null ? "" : cashDetails.TransactionBeneficiary.BenCountry.Trim();
                cmdSaveAcData.Parameters.Add("@BenID", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.BenID == null ? "" : cashDetails.TransactionBeneficiary.BenID.Trim();
                cmdSaveAcData.Parameters.Add("@IdNumber", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.IdNumber == null ? "" : cashDetails.TransactionBeneficiary.IdNumber.Trim();
                cmdSaveAcData.Parameters.Add("@Idtype", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.Idtype == null ? "" : cashDetails.TransactionBeneficiary.Idtype.Trim();
                cmdSaveAcData.Parameters.Add("@BeneRecipientName", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.RecipientName == null ? "" : cashDetails.TransactionBeneficiary.RecipientName.Trim();
                cmdSaveAcData.Parameters.Add("@BeneTelephoneNumber", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.TelephoneNumber == null ? "" : cashDetails.TransactionBeneficiary.TelephoneNumber.Trim();
                cmdSaveAcData.Parameters.Add("@RecipientMessage", SqlDbType.VarChar).Value = cashDetails.TransactionBeneficiary.RecipientMessage == null ? "" : cashDetails.TransactionBeneficiary.RecipientMessage.Trim();
                                              
                cmdSaveAcData.Parameters.Add("@TxnReceiveDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmdSaveAcData.Parameters.Add("@TxnStatus", SqlDbType.VarChar).Value = "RECEIVED";
                cmdSaveAcData.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = "CASH";
                cmdSaveAcData.Parameters.Add("@DownloadBranch", SqlDbType.VarChar).Value = downloadBranch;
                
                try
                {
                    int k = cmdSaveAcData.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    InsertAutoFetchLog(userId, "InsertIntoEzRemitDataTable", "Error ! InsertIntoEzRemitDataTable" + ex.ToString());
                    stats = false;
                }
            }
            catch (Exception exc) { stats = false; }

            return stats;
        }

        internal DataTable GetMTBAcFailedOrSuccessTxn(string fromdt, string todt, string opsType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                if (opsType.Equals("FAILED"))
                {
                    query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=ft.PartyId) Exch, ft.RefNo, ft.PaymentStatus Status, ft.BeneficiaryAccountNo BenAcc, "
                        + " ft.BeneficiaryName, convert(varchar, ft.transdate, 20) TxnDate, ft.SenderName, ft.SenderCountry, ft.TrAmount Amount, ft.IsSuccess "
                        + " FROM [RemittanceDB].[dbo].FundTransferRequest ft "
                        + " where convert(varchar, ft.transdate,23) between '" + fromdt + "' and '" + todt + "' "
                        + " and ft.PaymentStatus<>5 "
                        + " order by ft.transdate desc";
                }
                else
                {
                    query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=ft.PartyId) Exch, ft.RefNo, ft.PaymentStatus Status, ft.BeneficiaryAccountNo BenAcc, "
                        + " ft.BeneficiaryName, convert(varchar, ft.transdate, 20) TxnDate, ft.SenderName, ft.SenderCountry, ft.TrAmount Amount, ft.IsSuccess "
                        + " FROM [RemittanceDB].[dbo].FundTransferRequest ft "
                        + " where convert(varchar, ft.transdate,23) between '" + fromdt + "' and '" + todt + "' "
                        + " and ft.PaymentStatus=5 "
                        + " order by ft.transdate desc";
                }                

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetBkashAcFailedOrSuccessTxn(string fromdt, string todt, string opsType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                if (opsType.Equals("FAILED"))
                {
                    query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo, "
                        + " case when RemitStatus=1 then 'Received'  when RemitStatus=2 then 'Canceled'  when RemitStatus=3 then 'Queued'  when RemitStatus=4 then 'bKash Acknlg' "
                        + " when RemitStatus=5 then 'Success' when RemitStatus=6 then 'Cancelled' when RemitStatus=7 then 'Technical Error'  else '' end Status, "
                        + " convert(varchar, mw.RequestTime, 20) RequestTime, convert(varchar, mw.MTBProcessTime, 20) ProcessTime, mw.RemitReceiverMSISDN WalletNo, mw.RemitReceiverAmount Amount, mw.SenderNationality, "
                        + " mw.responseMessage bkResp, mw.CBResponseDescription CallBackResp, mw.CBReceiveTime CallBackTime "
                        + " FROM [RemittanceDB].[dbo].MobileWalletRemitTransfer mw  where convert(varchar, mw.RequestTime,23) between '" + fromdt + "' and '" + todt + "' "
                        + " and mw.RemitStatus<>5 ORDER BY mw.AutoID desc";
                }
                else
                {
                    query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=mw.ExchangeHouseID) Exch, mw.TranTxnId RefNo, "
                        + " case when RemitStatus=1 then 'Received'  when RemitStatus=2 then 'Canceled'  when RemitStatus=3 then 'Queued'  when RemitStatus=4 then 'bKash Acknlg' "
                        + " when RemitStatus=5 then 'Success' when RemitStatus=6 then 'Cancelled' when RemitStatus=7 then 'Technical Error'  else '' end Status, "
                        + " convert(varchar, mw.RequestTime, 20) RequestTime, convert(varchar, mw.MTBProcessTime, 20) ProcessTime, mw.RemitReceiverMSISDN WalletNo, mw.RemitReceiverAmount Amount, mw.SenderNationality, "
                        + " mw.responseMessage bkResp, mw.CBResponseDescription CallBackResp, mw.CBReceiveTime CallBackTime "
                        + " FROM [RemittanceDB].[dbo].MobileWalletRemitTransfer mw  where convert(varchar, mw.RequestTime,23) between '" + fromdt + "' and '" + todt + "' "
                        + " and mw.RemitStatus=5 ORDER BY mw.AutoID desc";
                }
                
                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetAPIRequestLogByReferenceNo(string val)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                query = "SELECT [LogId],[UserId],[RequesterLocaltion],[RequestTime],[Authenticated] Auth,[RequestCode],[ResponseCode] "
                    + " FROM [RemittanceDB].[dbo].[RequestLog] Where RequestCode like '%" + val + "%' Order By LogId desc";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaExchs = new SqlDataAdapter(cmd);
                sdaExchs.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal void MoveEzRemitDataIntoRemitInfoTable(string txnNum, int idType, string idNumber, string mobileNum, string kycAddrs, string downloadUser)
        {
            try
            {
                SqlConnection connNewSystem = null;
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                SqlCommand cmdSaveData = new SqlCommand();

                if (connNewSystem.State.Equals(ConnectionState.Closed))
                {
                    connNewSystem.Open();
                }

                cmdSaveData.CommandType = CommandType.StoredProcedure;
                cmdSaveData.CommandText = "EzRemitSpCashDataMoveToRemitInfoTable";
                cmdSaveData.Connection = connNewSystem;

                cmdSaveData.Parameters.Add("@SecurityNumber", SqlDbType.VarChar).Value = txnNum;
                cmdSaveData.Parameters.Add("@idType", SqlDbType.Int).Value = idType;
                cmdSaveData.Parameters.Add("@idNumber", SqlDbType.VarChar).Value = idNumber;
                cmdSaveData.Parameters.Add("@mobileNumber", SqlDbType.VarChar).Value = mobileNum;
                cmdSaveData.Parameters.Add("@kycAddress", SqlDbType.VarChar).Value = kycAddrs;
                cmdSaveData.Parameters.Add("@downloadUser", SqlDbType.VarChar).Value = downloadUser;

                try
                {
                    int k = cmdSaveData.ExecuteNonQuery();
                }
                catch (Exception ex) { }
                finally
                {
                    if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
                }
            }
            catch (Exception exc) { }
        }

        internal void UpdateTxnStatusIntoEzRemitTable(string TransactionNo, string confirmFlag, string remarks, string downloadUser)
        {
            SqlConnection connNewSystem = null;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                connNewSystem.Open();

                string query = "";

                if (confirmFlag.Equals("C"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[EzRemitRequestData] SET [TxnStatus]='PAID', [TxnPaymentDate]=getdate(), [ClearingUser]='" + downloadUser + "', [ClearingDate]=getdate()  WHERE [SecurityNumber]='" + TransactionNo + "'";
                }
                else if (confirmFlag.Equals("E"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[EzRemitRequestData] SET [TxnStatus]='ERROR', [ClearingUser]='" + downloadUser + "', [ClearingDate]=getdate(), [Remarks]='" + remarks + "'  WHERE [SecurityNumber]='" + TransactionNo + "'";
                }

                SqlCommand _cmd = new SqlCommand(query, connNewSystem);

                try
                {
                    int k = _cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { }                
            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open))
                {
                    connNewSystem.Close();
                }
            }
        }


        internal DataTable GetEzRemitCashPaidTxnList(string dtValueFrm, string dtValueTo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string queryData = "SELECT [SecurityNumber],[PaymentMode] Mode,[FxAmount] Amount,[BeneName] Beneficiary,[SenderCustomerName] Sender,[TxnStatus] Status,"
                    + " [TxnReceiveDate] ReceiveDate,[TxnPaymentDate] PaymentDate,[BeneTelephoneNumber] BeneTelephone,[BeneAddress],[BeneNationality],[Relationship],[FundSource],[Purpose],"
                    + " [SenderCustNationality] SenderNationality,[SenderCustTelephoneNumber] SenderTelephone, "
                    + " [SendingContactPerson],[SendingCustBankName] SenderBankName,[SendingCustCountry] SendingCountry,[TransactionDate] "
                    + " FROM [RemittanceDB].[dbo].[EzRemitRequestData]  WHERE convert(date, [TxnPaymentDate]) BETWEEN '" + dtValueFrm + "' AND '" + dtValueTo + "' "
                    + " AND [TxnStatus]='PAID' AND [PaymentMode]='CASH' order by AutoId desc";
                SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetRMSUserList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "SELECT ui.[AutoId] SL,ui.[UserId],ui.[UserRMCode] RMCode,ui.[UserFullName] UserName,ui.[UserEmail], ur.RoleName, case when ui.[isActive]=1 then 'Y' else 'N' end IsActive,ui.[FileProcessUserType] UserType, ui.[IsMailReceive] MailRecv "
                    + " FROM [NRBWork].[dbo].[WEB_UserInfo] ui INNER JOIN [NRBWork].[dbo].[WEB_UserRole] ur "
                    + " ON ui.UserTypeId=ur.RoleId  AND ur.isActive=1 ORDER BY ui.UserTypeId , ui.UserFullName ";
                dt = dbManager.GetDataTable(queryUser);                
            }
            catch (Exception ex){    }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return dt;
        }        

        internal DataTable GetReturnEmailDetails()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "SELECT [AutoId] SL,[PartyId],[UserId],[ExchangeHouseFullName] ExchangeHouseName,[ToAddress],[CcAddress],[isActive] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] order by [ExchangeHouseFullName]";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return dt;
        }

        //internal bool UpdateReturnEmailAddress(int sl, int partyId, string toAddr, string ccAddr)
        //{
        //    bool updStat = false;
        //    try
        //    {
        //        dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
        //        dbManager.OpenDatabaseConnection();

        //        string updStmt = "UPDATE [NRBWork].[dbo].[BEFTNReturnEmailAddress] SET [ToAddress]='" + toAddr + "', [CcAddress]='" + ccAddr + "'  WHERE [AutoId]=" + sl + " AND [PartyId]=" + partyId + "";
        //        updStat = dbManager.ExcecuteCommand(updStmt);
        //    }
        //    catch (Exception ex) { }
        //    finally
        //    {
        //        dbManager.CloseDatabaseConnection();
        //    }
        //    return updStat;
        //}

        internal DataTable GetRMSUserRole()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "SELECT [RoleId],[RoleName]  FROM [NRBWork].[dbo].[WEB_UserRole] WHERE [isActive]=1";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally{ dbManager.CloseDatabaseConnection();}
            return dt;
        }

        internal DataTable GetRMSUserRoleList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();
                string queryUser = "SELECT [AutoId] SL, [RoleId],[RoleName], CASE WHEN [isActive]=1 THEN 'Y' ELSE 'N' END isActive  FROM [NRBWork].[dbo].[WEB_UserRole] ORDER BY [AutoId]";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally { dbManager.CloseDatabaseConnection(); }
            return dt;
        }

        internal bool UpdateWebUserInfo(string sl, string uId, string uName, string uMail, string uRole, string uActv, string uTyp, string mailRcv)
        {
            bool updateStats = false;
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryRole = "SELECT RoleId FROM [NRBWork].[dbo].[WEB_UserRole] WHERE RoleName='" + uRole + "'";
                dt = dbManager.GetDataTable(queryRole);
                int roleid = Convert.ToInt32(dt.Rows[0][0]);

                string queryUser = "UPDATE [NRBWork].[dbo].[WEB_UserInfo] SET [UserFullName]='" + uName + "', [UserEmail]='" + uMail + "', [isActive]=" + Convert.ToInt32(uActv) + ""
                    + " , [UserTypeId]=" + roleid + ", [FileProcessUserType]='" + uTyp + "', [IsMailReceive]='" + mailRcv + "'  WHERE [AutoId]=" + Convert.ToInt32(sl) + " AND [UserId]='" + uId + "'";

                updateStats = dbManager.ExcecuteCommand(queryUser);            
            }
            catch (Exception ex) { updateStats = false; }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return updateStats;
        }

        internal bool SaveWebUserInfo(string uId, string uRmCode, string uName, string uMail, string uRole, string uActv, string uTyp, string mailRcv)
        {
            bool saveStats = false;
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryRole = "SELECT RoleId FROM [NRBWork].[dbo].[WEB_UserRole] WHERE RoleName='" + uRole + "'";
                dt = dbManager.GetDataTable(queryRole);
                int roleid = Convert.ToInt32(dt.Rows[0][0]);

                string encryptUserPass = Utility.HashSHA1Decryption("1"); // default pass set as 1

                string queryUser = "INSERT INTO [NRBWork].[dbo].[WEB_UserInfo]([UserId],[UserRMCode],[UserFullName],[UserPassword],[UserEmail],[isActive],[UserTypeId],[FileProcessUserType],[IsMailReceive]) "
                    + " VALUES('" + uId + "', '" + uRmCode + "','" + uName + "','" + encryptUserPass + "', '" + uMail + "'," + Convert.ToInt32(uActv) + "," + roleid + ", '" + uTyp + "', '" + mailRcv + "') ";

                saveStats = dbManager.ExcecuteCommand(queryUser);
            }
            catch (Exception ex) { saveStats = false; }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return saveStats;
        }

        internal bool IsUserExistsAlready(string uId)
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();
                string queryUser = "SELECT [AutoId],[UserId] FROM [NRBWork].[dbo].[WEB_UserInfo] WHERE [UserId]='" + uId + "'";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) {  }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }

            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        
        internal DataTable GetExchangeHouseInfoList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "SELECT [AutoId] SL,[PartyId],[UserId], CASE WHEN [PartyTypeId]=1 THEN 'WageEarners' ELSE 'ServiceRemittance' END ExhType, "
                    + " [ExchangeHouseFullName] ExHouseName,[ExchangeHouseCountry] ExHCountry,[CompanyType],[NRTAccount],isnull([WalletAccount],'')WalletAccount,[USDAccount],isnull([AEDAccount],'')AEDAccount,"
                    + " CASE WHEN [isActive]=1 THEN 'YES' ELSE 'NO' END IsActive,isnull([ToAddress],'')ToAddress,isnull([CcAddress],'')CcAddress,[ApiOrFile] "
                    + " ,[BEFTNBDTRate],[BKASHBDTRate],[CASHBDTRate],[ACCREDITBDTRate],[BEFTNUSDRate],[BKASHUSDRate],[CASHUSDRate],[ACCREDITUSDRate], [CommCurrency],[ExchangeHouseShortName],[isCOC] "
                    + " FROM [NRBWork].[dbo].[ExchangeHouseInfoList] ORDER BY [ExchangeHouseFullName]";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return dt;
        }

        internal DataTable GetRMSUserMenuList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                //string query = "SELECT [AutoId] SL,[MenuId],[MenuTitle],[MenuURL],[MenuParentId], CASE WHEN [isActive]=1 THEN 'Y' ELSE 'N' END IsActive,[SlNo],[MenuPrioritySL] FROM [NRBWork].[dbo].[WEB_UserMenu] ORDER BY [AutoId]";
                string query = "SELECT m1.[AutoId] SL,m1.[MenuId],m1.[MenuTitle],m1.[MenuURL], (select m2.MenuTitle FROM [NRBWork].[dbo].[WEB_UserMenu] m2 where m2.[MenuId]=m1.[MenuParentId]) ParentMenu, CASE WHEN [isActive]=1 THEN 'Y' ELSE 'N' END IsActive, m1.[SlNo], m1.[MenuPrioritySL] FROM [NRBWork].[dbo].[WEB_UserMenu] m1 ORDER BY m1.[MenuPrioritySL], m1.[SlNo]";
                dt = dbManager.GetDataTable(query);
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return dt;
        }

        internal DataTable GetRMSParentMenuList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "SELECT [MenuId],[MenuTitle]  FROM [NRBWork].[dbo].[WEB_UserMenu] WHERE [MenuURL]='#' order by [MenuId]";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return dt;
        }

        internal bool SaveWebUserMenu(string menuTitle, string menuUrl, int parentValue, int menuActivity)
        {
            bool insStats = false;
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                int SlNo = 1, menuPrioritySl = -1, menuParentId = -1;

                string queryNewId = "SELECT max(MenuId)+1 FROM [NRBWork].[dbo].[WEB_UserMenu] ";
                dt = dbManager.GetDataTable(queryNewId);
                int newMenuid = Convert.ToInt32(dt.Rows[0][0]);

                queryNewId = "SELECT max([MenuPrioritySL])+1 FROM [NRBWork].[dbo].[WEB_UserMenu] ";
                dt = dbManager.GetDataTable(queryNewId);
                menuPrioritySl = Convert.ToInt32(dt.Rows[0][0]);

                if (parentValue == -2)  // new Parent menu
                {
                    SlNo = 1;
                    menuParentId = newMenuid;
                }
                else  // existing Parent
                {
                    string querySlNo = "SELECT max([SlNo])+1 FROM [NRBWork].[dbo].[WEB_UserMenu] where [MenuParentId]=" + parentValue;
                    dt = dbManager.GetDataTable(querySlNo);
                    SlNo = Convert.ToInt32(dt.Rows[0][0]);
                    menuParentId = parentValue;

                    string queryPrioritySlNo = "SELECT [MenuPrioritySL] FROM [NRBWork].[dbo].[WEB_UserMenu] where [MenuParentId]=" + parentValue;
                    dt = dbManager.GetDataTable(queryPrioritySlNo);
                    menuPrioritySl = Convert.ToInt32(dt.Rows[0][0]);
                }

                string queryUser = "INSERT INTO [NRBWork].[dbo].[WEB_UserMenu]([MenuId],[MenuTitle],[MenuURL],[MenuParentId],[isActive],[SlNo],[MenuPrioritySL]) VALUES( "
                    + newMenuid + ", '" + menuTitle + "','" + menuUrl + "'," + menuParentId + "," + menuActivity + "," + SlNo + "," + menuPrioritySl + ")";

                dbManager.ExcecuteCommand(queryUser);
                insStats = true;
            }
            catch (Exception ex) { insStats = false; }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }

            return insStats;
        }

        internal bool UpdateWebUserMenu(int slNo, int menuId, string menuTitle, string menuUrl, int parentValue, int menuActivity, int internalSlNo)
        {
            bool updStats = false;
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryNewId = "SELECT TOP 1 [MenuPrioritySL] FROM [NRBWork].[dbo].[WEB_UserMenu] WHERE [MenuParentId]=" + parentValue;
                DataTable dt = dbManager.GetDataTable(queryNewId);
                int menuPrioritySl = Convert.ToInt32(dt.Rows[0][0]);

                string queryUser = "UPDATE [NRBWork].[dbo].[WEB_UserMenu] SET [MenuTitle]='" + menuTitle + "',[MenuURL]='" + menuUrl + "', [MenuParentId]=" + parentValue + ", "
                    + " [isActive]=" + menuActivity + ", [SlNo]=" + internalSlNo + ", [MenuPrioritySL]=" + menuPrioritySl + " WHERE [AutoId]=" + slNo + " AND [MenuId]=" + menuId;

                dbManager.ExcecuteCommand(queryUser);
                updStats = true;
            }
            catch (Exception ex) { updStats = false; }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return updStats;
        }

        internal DataTable GetMappedUnmappedMenuListByRoleId(int roleId)
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "SELECT p.AutoId SL, p.MenuId, p.ParentMenu , p.MenuTitle, p.MenuURL, "
                    + " case when p.rmmnu is null then 'NOT ASSIGNED' else 'ASSIGNED' end MappingStatus "
                    + " FROM ( "
                    + " SELECT um.AutoId, um.MenuId, (Select m.MenuTitle FROM [NRBWork].[dbo].[WEB_UserMenu] m WHERE m.MenuId=um.MenuParentId) ParentMenu , um.MenuTitle, um.MenuURL, "
                    + " (select rm.MenuId from [NRBWork].[dbo].[WEB_RoleMenuMapping] rm where rm.RoleId=" + roleId + " and um.MenuId=rm.MenuId) rmmnu, um.[MenuPrioritySL] "
                    + " FROM [NRBWork].[dbo].[WEB_UserMenu] um )p "
                    + " order by p.[MenuPrioritySL]";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return dt;
        }

        internal bool AssignRoleMenu(int roleId, int menuId)
        {
            bool updStats = false;
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUpd = "";
                string queryUser = "SELECT * FROM [NRBWork].[dbo].[WEB_RoleMenuMapping] WHERE [RoleId]=" + roleId + " AND [MenuId]=" + menuId;
                dt = dbManager.GetDataTable(queryUser);

                if (dt.Rows.Count > 0)
                {
                    queryUpd = "UPDATE [NRBWork].[dbo].[WEB_RoleMenuMapping] SET [MakerDate]=GETDATE(), [isActive]=1  WHERE [RoleId]=" + roleId + " AND [MenuId]=" + menuId;
                }
                else
                {
                    queryUpd = "INSERT INTO [NRBWork].[dbo].[WEB_RoleMenuMapping]([RoleId],[MenuId],[isActive]) VALUES(" + roleId + ", " + menuId + ", 1)";
                }

                dbManager.ExcecuteCommand(queryUpd);
                updStats = true;
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }

            return updStats;
        }

        internal bool DeAssignRoleMenu(int roleId, int menuId)
        {
            bool updStats = false;
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUpd = "";
                string queryUser = "SELECT * FROM [NRBWork].[dbo].[WEB_RoleMenuMapping] WHERE [RoleId]=" + roleId + " AND [MenuId]=" + menuId;
                dt = dbManager.GetDataTable(queryUser);

                if (dt.Rows.Count > 0)
                {
                    queryUpd = "DELETE FROM [NRBWork].[dbo].[WEB_RoleMenuMapping] WHERE [RoleId]=" + roleId + " AND [MenuId]=" + menuId;
                    dbManager.ExcecuteCommand(queryUpd);
                    updStats = true;
                }                
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return updStats;
        }

        internal bool UpdateWebUserRole(int slNo, int roleId, string roleNm, int roleActivity)
        {
            bool updStats = false;
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "UPDATE [NRBWork].[dbo].[WEB_UserRole] SET [RoleName]='" + roleNm + "',[isActive]=" + roleActivity + " WHERE [AutoId]=" + slNo + " AND [RoleId]=" + roleId;
                dbManager.ExcecuteCommand(queryUser);
                updStats = true;
            }
            catch (Exception ex) { updStats = false; }
            finally{ dbManager.CloseDatabaseConnection(); }
            return updStats;
        }

        internal bool SaveWebUserRole(string rolename)
        {
            bool insStats = false;            
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryNewId = "SELECT max([RoleId])+1 FROM [NRBWork].[dbo].[WEB_UserRole] ";
                DataTable dt = dbManager.GetDataTable(queryNewId);
                int newRoleid = Convert.ToInt32(dt.Rows[0][0]);

                string queryUser = "INSERT INTO [NRBWork].[dbo].[WEB_UserRole]([RoleId],[RoleName],[isActive]) VALUES(" + newRoleid + ", '" + rolename.Trim().ToUpper() + "',1)";
                dbManager.ExcecuteCommand(queryUser);
                insStats = true;
            }
            catch (Exception ex) { insStats = false; }
            finally{ dbManager.CloseDatabaseConnection(); }
            return insStats;
        }



        internal DataTable GetOwnBankData(string dtValue1, string dtValue2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT convert(varchar, ftr.[TransDate], 20)'Txn Date', t.CBSValueDate, "
                    + " case when usr.PartyTypeId=1 then 'Wage' when usr.PartyTypeId=2 then 'Service' else 'Others' end Type, "
                    + " (SELECT u.UserId FROM [RemittanceDB].[dbo].Users u  where u.PartyId = ftr.PartyId)'Exchange House', "
                    + " ltrim(rtrim([RefNo]))'Reference No', [TrAmount] Amount, ltrim(rtrim([BeneficiaryName]))'Beneficiary Name', REPLACE([BeneficiaryAccountNo],'-','')BeneficiaryAccountNo, 'MTB' BankName, "
                    + " (SELECT [BranchName] FROM [RemittanceDB].[dbo].[BranchDisbursementAcc] where [BranchId] = SUBSTRING([BeneficiaryAccountNo], 1 , 4) )BranchName,"
                    + " [SenderName], [SenderCountry], [SenderAddress], [SenderPhone], "                    
                    + " (Select Status from [RemittanceDB].[dbo].PaymentStatus where StatusID =  [PaymentStatus])'Remittance Status' "
                    + " FROM [RemittanceDB].[dbo].[FundTransferRequest] ftr JOIN [RemittanceDB].[dbo].[Users] usr "
                    + " ON ftr.PartyId=usr.PartyId INNER JOIN [RemittanceDB].[dbo].[Transactions] t ON ftr.RefNo = t.SessionId "
                    + " WHERE t.IsSuccess = 1 AND t.Type IN ('101', '102', '201', '202')  AND CONVERT(DATETIME, t.CBSValueDate, 105) BETWEEN '" + dtValue1 + "' and '" + dtValue2 + "' "
                    + " and PaymentStatus = 5 ORDER BY CONVERT(DATETIME, t.CBSValueDate, 105), ftr.[TransDate] ";

                SqlDataAdapter sdaOwn = new SqlDataAdapter(query, connDRSystem);
                sdaOwn.Fill(dt);
            }
            catch (Exception exc) { }
            return dt;
        }


        internal bool ResetPassword(string userRmCode)
        {
            bool updStats = false;
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, nrbworkConnectionString);
                dbManager.OpenDatabaseConnection();

                string encryptPass = Utility.HashSHA1Decryption("1");

                string queryUser = "UPDATE [NRBWork].[dbo].[WEB_UserInfo] SET [UserPassword]='" + encryptPass + "' WHERE [UserRMCode]='" + userRmCode + "'";
                dbManager.ExcecuteCommand(queryUser);
                updStats = true;
            }
            catch (Exception ex) { updStats = false; }
            finally { dbManager.CloseDatabaseConnection(); }
            return updStats;
        }

        internal DataTable GetBEFTNIncentiveEligibleList(string dtValueFrom, string dtValueTo, string incTyp)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }
                              
                string query = "SELECT SL,EXCHNAGE_HOUSE, PIN_NO, SENDER_NAME, SENDER_ADDRS, BENE_NAME, BENE_ACCOUNT, PRINCIPAL, INCENTIVE, TRANSACTION_DATE "
                        + " , (SELECT top 1 tp.PaymentDescription FROM [RemittanceDB].[dbo].[BEFTNRequest] tp WHERE tp.PartyId=p.PartyId and tp.RefNo=p.PIN_NO and tp.IsIncentive=0) SOURCE_OF_FUND "
                        + " FROM( "
                        + " SELECT t.autoid SL, t.PartyId , (SELECT u.userid  FROM [RemittanceDB].[dbo].[users] u  WHERE u.PartyId = t.PartyId) EXCHNAGE_HOUSE, "
                        + " t.RefNo PIN_NO, t.SenderName SENDER_NAME, t.SenderAddress SENDER_ADDRS, t.BeneficiaryName BENE_NAME, t.BeneficiaryAccountNo BENE_ACCOUNT, "
                        + " cast((t.Amount * 100) / 2.5 AS DECIMAL(20, 2)) PRINCIPAL, t.Amount INCENTIVE, t.RequestTime  TRANSACTION_DATE "
                        + " FROM [RemittanceDB].[dbo].[BEFTNRequest] t "
                        + " WHERE t.PaymentStatus = " + incTyp
                        + " AND ( convert(varchar, t.RequestTime, 23) BETWEEN '" + dtValueFrom + "' AND '" + dtValueTo + "') "
                        + " )p ORDER BY TRANSACTION_DATE";

                SqlDataAdapter sdaInct = new SqlDataAdapter(query, connDRSystem);
                sdaInct.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }



        internal void UpdateICTCConfirmDownloadAccountCreditTxnTable(string ICTC_Number, string confirmFlag, string remarks)
        {
            SqlConnection connNewSystem = null;
            //bool updateStat = false;

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State == ConnectionState.Closed) { connNewSystem.Open(); }                
                string query = "";

                if (confirmFlag.Equals("D"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[ICTCRequestData] SET [CONFIRM_DOWNLOAD_TXN]='" + confirmFlag + "' WHERE [ICTC_NUMBER]='" + ICTC_Number + "'";
                }
                else if (confirmFlag.Equals("Y"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[ICTCRequestData] SET [CONFIRM_DOWNLOAD_TXN]='" + confirmFlag + "', [TXN_STATUS]='PAID', [TXN_PAYMENT_DATE]=getdate(), [ClearingDate]=getdate()  WHERE [ICTC_NUMBER]='" + ICTC_Number + "'";
                }
                else
                {
                    query = "UPDATE [RemittanceDB].[dbo].[ICTCRequestData] SET [CONFIRM_DOWNLOAD_TXN]='" + confirmFlag + "', [TXN_STATUS]='ERROR', [REMARKS]='" + remarks + "', [CANCEL_DATE]=getdate()  WHERE [ICTC_NUMBER]='" + ICTC_Number + "'";
                }

                SqlCommand _cmd = new SqlCommand(query, connNewSystem);
                try{
                    _cmd.ExecuteNonQuery();
                }
                catch (Exception exc){   string err = exc.Message;  }
                _cmd.Dispose();
            }
            catch (Exception ex) { }
            finally
            {
                try { if (connNewSystem != null && connNewSystem.State == ConnectionState.Open) { connNewSystem.Close(); } }
                catch (SqlException sqlException) { throw sqlException; }
            }
            //return updateStat;
        }




        internal bool UpdateExchangeHouseInfo(ExchangeHouseInfo ehi)
        {
            bool updateStats = false;
            SqlConnection connNRBWorkSystem = null;

            try
            {                
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                SqlCommand cmdUpdateData = new SqlCommand();

                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed))
                {
                    connNRBWorkSystem.Open();
                }

                string updQueryUser = "UPDATE [NRBWork].[dbo].[ExchangeHouseInfoList] SET [PartyId]=@PartyId, [UserId]=@UserId, [PartyTypeId]=@PartyTypeId, [NRTAccount]=@NRTAccount, "
                    + "[WalletAccount]=@WalletAccount, [USDAccount]=@USDAccount, [AEDAccount]=@AEDAccount, [ExchangeHouseFullName]=@ExchangeHouseFullName, "
                    + " [ExchangeHouseCountry]=@ExchangeHouseCountry, [CompanyType]=@CompanyType, [isActive]=@isActive, [ToAddress]=@ToAddress, [CcAddress]=@CcAddress,"
                    + " [ApiOrFile]=@ApiOrFile, [BEFTNBDTRate]=@BEFTNBDTRate, [BKASHBDTRate]=@BKASHBDTRate, [CASHBDTRate]=@CASHBDTRate, [ACCREDITBDTRate]=@ACCREDITBDTRate,"
                    + " [BEFTNUSDRate]=@BEFTNUSDRate, [BKASHUSDRate]=@BKASHUSDRate, [CASHUSDRate]=@CASHUSDRate, [ACCREDITUSDRate]=@ACCREDITUSDRate, [CommCurrency]=@CommCurrency, [ExchangeHouseShortName]=@ExchangeHouseShortName, [isCOC]=@isCOC "
                    + " WHERE [AutoId]=@sl";

                cmdUpdateData.CommandType = CommandType.Text;
                cmdUpdateData.CommandText = updQueryUser;
                cmdUpdateData.Connection = connNRBWorkSystem;

                cmdUpdateData.Parameters.Add("@PartyId", SqlDbType.Int).Value = Convert.ToInt32(ehi.partyId);
                cmdUpdateData.Parameters.Add("@UserId", SqlDbType.VarChar).Value = ehi.userId.Trim();
                cmdUpdateData.Parameters.Add("@PartyTypeId", SqlDbType.Int).Value = Convert.ToInt32(ehi.exhType);
                cmdUpdateData.Parameters.Add("@NRTAccount", SqlDbType.VarChar).Value = ehi.nrtAccount.Trim();
                cmdUpdateData.Parameters.Add("@WalletAccount", SqlDbType.VarChar).Value = ehi.walletAccount.Trim();
                cmdUpdateData.Parameters.Add("@USDAccount", SqlDbType.VarChar).Value = ehi.usdAccount.Trim();
                cmdUpdateData.Parameters.Add("@AEDAccount", SqlDbType.VarChar).Value = ehi.aedAccount.Trim();
                cmdUpdateData.Parameters.Add("@ExchangeHouseFullName", SqlDbType.VarChar).Value = ehi.exhName.Trim();
                cmdUpdateData.Parameters.Add("@ExchangeHouseCountry", SqlDbType.VarChar).Value = ehi.exhCountry.Trim();
                cmdUpdateData.Parameters.Add("@CompanyType", SqlDbType.VarChar).Value = ehi.companyType.Trim();
                cmdUpdateData.Parameters.Add("@isActive", SqlDbType.Int).Value = Convert.ToInt32(ehi.activeStat);
                cmdUpdateData.Parameters.Add("@ToAddress", SqlDbType.VarChar).Value = ehi.toMailAddr.Trim();
                cmdUpdateData.Parameters.Add("@CcAddress", SqlDbType.VarChar).Value = ehi.ccMailAddr.Trim();
                cmdUpdateData.Parameters.Add("@ApiOrFile", SqlDbType.VarChar).Value = ehi.apiorfile.Trim();

                cmdUpdateData.Parameters.Add("@BEFTNBDTRate", SqlDbType.Float).Value = ehi.EftBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.EftBdtRate), 2);
                cmdUpdateData.Parameters.Add("@BKASHBDTRate", SqlDbType.Float).Value = ehi.BkashBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.BkashBdtRate), 2);
                cmdUpdateData.Parameters.Add("@CASHBDTRate", SqlDbType.Float).Value = ehi.CashBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.CashBdtRate), 2);
                cmdUpdateData.Parameters.Add("@ACCREDITBDTRate", SqlDbType.Float).Value = ehi.AcBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.AcBdtRate), 2);
                cmdUpdateData.Parameters.Add("@BEFTNUSDRate", SqlDbType.Float).Value = ehi.EftUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.EftUsdRate), 2);
                cmdUpdateData.Parameters.Add("@BKASHUSDRate", SqlDbType.Float).Value = ehi.BkashUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.BkashUsdRate), 2);
                cmdUpdateData.Parameters.Add("@CASHUSDRate", SqlDbType.Float).Value = ehi.CashUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.CashUsdRate), 2);
                cmdUpdateData.Parameters.Add("@ACCREDITUSDRate", SqlDbType.Float).Value = ehi.AcUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.AcUsdRate), 2);

                cmdUpdateData.Parameters.Add("@CommCurrency", SqlDbType.VarChar).Value = ehi.commissionCurrency.Trim();
                cmdUpdateData.Parameters.Add("@ExchangeHouseShortName", SqlDbType.VarChar).Value = ehi.exhShortName.Trim();
                cmdUpdateData.Parameters.Add("@isCOC", SqlDbType.VarChar).Value = ehi.isCOC.Trim();
                
                cmdUpdateData.Parameters.Add("@sl", SqlDbType.Int).Value = Convert.ToInt32(ehi.slNo);

                try
                {
                    int k = cmdUpdateData.ExecuteNonQuery();
                    updateStats = true;
                }
                catch (Exception ex) { }                
            }
            catch (Exception ex) { updateStats = false; }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return updateStats;
        }

        internal bool SaveExchangeHouseInfo(ExchangeHouseInfo ehi)
        {
            bool insertStats = false;
            SqlConnection connNRBWorkSystem = null;

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                SqlCommand cmdSaveData = new SqlCommand();
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)){ connNRBWorkSystem.Open();  }

                string saveQueryUser = "INSERT INTO [dbo].[ExchangeHouseInfoList]([PartyId],[UserId],[PartyTypeId],[NRTAccount],[WalletAccount],[USDAccount],[AEDAccount],[ExchangeHouseFullName],[ExchangeHouseCountry],[CompanyType], "
                    + " [isActive],[ToAddress],[CcAddress],[ApiOrFile],[BEFTNBDTRate],[BKASHBDTRate],[CASHBDTRate],[ACCREDITBDTRate],[BEFTNUSDRate],[BKASHUSDRate],[CASHUSDRate],[ACCREDITUSDRate],[CommCurrency],[ExchangeHouseShortName],[isCOC]) "
                    + " VALUES (@PartyId, @UserId, @PartyTypeId, @NRTAccount, @WalletAccount, @USDAccount, @AEDAccount, @ExchangeHouseFullName, @ExchangeHouseCountry,"
                    + " @CompanyType, @isActive, @ToAddress, @CcAddress,@ApiOrFile, @BEFTNBDTRate, @BKASHBDTRate, @CASHBDTRate, @ACCREDITBDTRate,"
                    + " @BEFTNUSDRate, @BKASHUSDRate, @CASHUSDRate, @ACCREDITUSDRate,@CommCurrency,@ExchangeHouseShortName,@isCOC)";

                cmdSaveData.CommandType = CommandType.Text;
                cmdSaveData.CommandText = saveQueryUser;
                cmdSaveData.Connection = connNRBWorkSystem;

                cmdSaveData.Parameters.Add("@PartyId", SqlDbType.Int).Value = ehi.partyId.Equals("") ? 0 : Convert.ToInt32(ehi.partyId);
                cmdSaveData.Parameters.Add("@UserId", SqlDbType.VarChar).Value = ehi.userId;
                cmdSaveData.Parameters.Add("@PartyTypeId", SqlDbType.Int).Value = Convert.ToInt32(ehi.exhType);
                cmdSaveData.Parameters.Add("@NRTAccount", SqlDbType.VarChar).Value = ehi.nrtAccount.Trim();
                cmdSaveData.Parameters.Add("@WalletAccount", SqlDbType.VarChar).Value = ehi.walletAccount.Trim();
                cmdSaveData.Parameters.Add("@USDAccount", SqlDbType.VarChar).Value = ehi.usdAccount.Trim();
                cmdSaveData.Parameters.Add("@AEDAccount", SqlDbType.VarChar).Value = ehi.aedAccount.Trim();
                cmdSaveData.Parameters.Add("@ExchangeHouseFullName", SqlDbType.VarChar).Value = ehi.exhName.Trim();
                cmdSaveData.Parameters.Add("@ExchangeHouseCountry", SqlDbType.VarChar).Value = ehi.exhCountry.Trim();
                cmdSaveData.Parameters.Add("@CompanyType", SqlDbType.VarChar).Value = ehi.companyType.Trim();
                cmdSaveData.Parameters.Add("@isActive", SqlDbType.Int).Value = 1;
                cmdSaveData.Parameters.Add("@ToAddress", SqlDbType.VarChar).Value = ehi.toMailAddr.Trim();
                cmdSaveData.Parameters.Add("@CcAddress", SqlDbType.VarChar).Value = ehi.ccMailAddr.Trim();
                cmdSaveData.Parameters.Add("@ApiOrFile", SqlDbType.VarChar).Value = ehi.apiorfile.Trim();

                cmdSaveData.Parameters.Add("@BEFTNBDTRate", SqlDbType.Float).Value = ehi.EftBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.EftBdtRate), 2);
                cmdSaveData.Parameters.Add("@BKASHBDTRate", SqlDbType.Float).Value = ehi.BkashBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.BkashBdtRate), 2);
                cmdSaveData.Parameters.Add("@CASHBDTRate", SqlDbType.Float).Value = ehi.CashBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.CashBdtRate), 2);
                cmdSaveData.Parameters.Add("@ACCREDITBDTRate", SqlDbType.Float).Value = ehi.AcBdtRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.AcBdtRate), 2);
                cmdSaveData.Parameters.Add("@BEFTNUSDRate", SqlDbType.Float).Value = ehi.EftUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.EftUsdRate), 2);
                cmdSaveData.Parameters.Add("@BKASHUSDRate", SqlDbType.Float).Value = ehi.BkashUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.BkashUsdRate), 2);
                cmdSaveData.Parameters.Add("@CASHUSDRate", SqlDbType.Float).Value = ehi.CashUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.CashUsdRate), 2);
                cmdSaveData.Parameters.Add("@ACCREDITUSDRate", SqlDbType.Float).Value = ehi.AcUsdRate.Equals("") ? 0 : Math.Round(Convert.ToSingle(ehi.AcUsdRate), 2);
                cmdSaveData.Parameters.Add("@CommCurrency", SqlDbType.VarChar).Value = ehi.commissionCurrency.Trim();
                cmdSaveData.Parameters.Add("@ExchangeHouseShortName", SqlDbType.VarChar).Value = ehi.exhShortName.Trim();
                cmdSaveData.Parameters.Add("@isCOC", SqlDbType.VarChar).Value = ehi.isCOC.Trim();
                                
                try
                {
                    int k = cmdSaveData.ExecuteNonQuery();
                    insertStats = true;
                }
                catch (Exception ex) { }
            }
            catch (Exception ex) { insertStats = false; }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return insertStats;
        }

        internal DataTable GetExhouseBalanceNew()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT [PartyId],[UserId],[ExchangeHouseFullName] ExHName,[NRTAccount],[WalletAccount],[USDAccount],[AEDAccount] "
                + " ,[NRTBalance],[USDAccountBalance] USDBalance,[AEDAccountBalance] AEDBalance,convert(varchar, [BalanceLastUpdateTime], 120) LastUpdate,[ApiOrFile] 'Api/File' "
                + " FROM [NRBWork].[dbo].[ExchangeHouseInfoList] WHERE [isActive]=1 order by [AutoId]";
                SqlDataAdapter sdaBalance = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaBalance.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal string GetTotalExchBalanceNew()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            decimal totalAmount = 0;

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                DataTable dtAPIallDistinct = new DataTable();
                string queryDistinct = "SELECT distinct [NRTAccount], round([NRTBalance],2) FROM [NRBWork].[dbo].[ExchangeHouseInfoList] Where [isActive]=1 and [NRTBalance]>0";
                SqlDataAdapter sdaExchsDistinct = new SqlDataAdapter(queryDistinct, connNRBWorkSystem);
                sdaExchsDistinct.Fill(dtAPIallDistinct);

                for (int ai = 0; ai < dtAPIallDistinct.Rows.Count; ai++)
                {
                    totalAmount += Convert.ToDecimal(dtAPIallDistinct.Rows[ai][1]);
                }                                
            }
            catch (Exception exc) { }
            return String.Format("{0:0.00}", totalAmount);
        }

        internal string GetTotalWageEarnersExchBalanceNew()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            decimal totalAmount = 0;

            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string queryDistinct = "SELECT distinct [NRTAccount], [NRTBalance] FROM [NRBWork].[dbo].[ExchangeHouseInfoList] Where [isActive]=1 and PartyTypeId=1 and [NRTBalance]>0";
                SqlDataAdapter sdaExchsDistinct = new SqlDataAdapter(queryDistinct, connNRBWorkSystem);
                sdaExchsDistinct.Fill(dt);

                for (int ai = 0; ai < dt.Rows.Count; ai++)
                {
                    totalAmount += Convert.ToDecimal(dt.Rows[ai][1]);
                }               
            }
            catch (Exception exc) { }
            return String.Format("{0:0.00}", totalAmount);
        }

        internal string GetTotalServiceRemExchBalanceNew()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            decimal totalAmount = 0;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT isnull(sum([NRTBalance]),0) FROM [NRBWork].[dbo].[ExchangeHouseInfoList] Where [isActive]=1 and PartyTypeId=2";
                SqlDataAdapter sdaExchs = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaExchs.Fill(dt);
                totalAmount = Convert.ToDecimal(dt.Rows[0][0]);
            }
            catch (Exception exc) { }
            return String.Format("{0:0.00}", totalAmount);
        }

        internal bool IsExistThisEntryIntoExchangeHouseInfoTable(string userId, ref int exchangeHouseInfoId)
        {
            DataTable dt = new DataTable();
            exchangeHouseInfoId = -1;
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceDbLvConnectionString);
                dbManager.OpenDatabaseConnection();
                string query = "SELECT [Id],[PartyId],[UserId] FROM [RemittanceDB].[dbo].[ExchangeHouseInfo] where UserId='" + userId + "'";

                //string query = "SELECT [AutoId],[PartyId],[UserId] FROM [RemittanceDB].[dbo].[ExchangeHouseInfoList] where [UserId]='" + userId + "'";
                dt = dbManager.GetDataTable(query.Trim());
            }
            catch (Exception ex) { }
            finally { dbManager.CloseDatabaseConnection(); }

            if (dt.Rows.Count > 0)
            {
                exchangeHouseInfoId = Convert.ToInt32(dt.Rows[0]["Id"]);
                return true;
            }
            return false;
        }

        internal bool UpdateExhInfoIntoRemittanceDbTable(ExchangeHouseInfo ehi, int exchangeHouseInfoId)
        {
            bool updateStat = false;
            SqlConnection connLiveSystem = null;
            SqlCommand cmdUpdateData = new SqlCommand();

            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                connLiveSystem.Open();

                string updQueryUser = "UPDATE [RemittanceDB].[dbo].[ExchangeHouseInfo] SET [PartyId]=@PartyId, [UserId]=@UserId, [isActive]=@isActive, [NRTAccountNo]=@NRTAccount, "
                    + "[MobileWalletAccount]=@WalletAccount, [ExchangeHouseFullName]=@ExchangeHouseFullName, [ExchangeHouseCountry]=@ExchangeHouseCountry, [CompanyType]=@CompanyType "
                    + " WHERE [Id]=@id";

                cmdUpdateData.CommandType = CommandType.Text;
                cmdUpdateData.CommandText = updQueryUser;
                cmdUpdateData.Connection = connLiveSystem;

                cmdUpdateData.Parameters.Add("@PartyId", SqlDbType.Int).Value = Convert.ToInt32(ehi.partyId);
                cmdUpdateData.Parameters.Add("@UserId", SqlDbType.VarChar).Value = ehi.userId.Trim();
                cmdUpdateData.Parameters.Add("@isActive", SqlDbType.Int).Value = Convert.ToInt32(ehi.activeStat);
                cmdUpdateData.Parameters.Add("@NRTAccount", SqlDbType.VarChar).Value = ehi.nrtAccount.Trim();
                cmdUpdateData.Parameters.Add("@WalletAccount", SqlDbType.VarChar).Value = ehi.walletAccount.Trim();
                cmdUpdateData.Parameters.Add("@ExchangeHouseFullName", SqlDbType.VarChar).Value = ehi.exhName.Trim();
                cmdUpdateData.Parameters.Add("@ExchangeHouseCountry", SqlDbType.VarChar).Value = ehi.exhCountry.Trim();
                cmdUpdateData.Parameters.Add("@CompanyType", SqlDbType.VarChar).Value = ehi.companyType.Trim();

                cmdUpdateData.Parameters.Add("@id", SqlDbType.Int).Value = exchangeHouseInfoId;

                try
                {
                    int k = cmdUpdateData.ExecuteNonQuery();
                    updateStat = true;
                }
                catch (Exception ex) { }   
            }
            catch (Exception ex) { updateStat = false; }
            finally { if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); } }

            return updateStat;
        }



        internal string GetRippleValidatorByPartyId(int exhId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string qryNodeAddr = "select top 1 NodeAddress FROM [RemittanceDB].[dbo].RipplePartnerAccountDetails where PartyId=" + exhId;
                SqlCommand cmd = new SqlCommand(qryNodeAddr, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return Convert.ToString(dt.Rows[0][0]); 
        }

        internal DataTable GetRippleBkashTxnSummaryByExchId(string dtValue1, string dtValue2, string validator)
        {
            SqlConnection connDRSystem = null;
            DataTable dtbKashRipple = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string rippQry = "SELECT count(*) cnt, round(isnull(sum(CONVERT(float, quote_amount)),0),2) amt "
                + " FROM [RemittanceDB].[dbo].RippleGETPaymentData where validator='" + validator + "' and payment_state='COMPLETED' and payment_type='REGULAR' and payment_module='WALLET' "
                + " and convert(varchar, payment_request_time, 23) between '" + dtValue1 + "' and '" + dtValue2 + "'";
                SqlDataAdapter sdabKashRipple = new SqlDataAdapter(rippQry, connDRSystem);
                sdabKashRipple.Fill(dtbKashRipple);

            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dtbKashRipple; 
        }

        internal DataTable GetExchCommissionData()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT [AutoId],[PartyId],[UserId], "
                    + " case when [BEFTNBDTRate]=0 then [BEFTNUSDRate] else [BEFTNBDTRate] end EFTRate, "
                    + " case when [BKASHBDTRate]=0 then [BKASHUSDRate] else [BKASHBDTRate] end BKASHRate, "
                    + " case when [CASHBDTRate]=0 then [CASHUSDRate] else [CASHBDTRate] end CASHRate, "
                    + " case when [ACCREDITBDTRate]=0 then [ACCREDITUSDRate] else [ACCREDITBDTRate] end ACCRate, [CommCurrency] "
                    + " FROM [NRBWork].[dbo].[ExchangeHouseInfoList] where [isActive]=1 and PartyId<>0 order by PartyId";

                SqlCommand cmd = new SqlCommand(query, connNRBWorkSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal void InsertAlipayFileInfoIntoDB(string sourceFileFullPath, string fileType, string rptDt)
        {
            SqlConnection connNRBWorkSystem = null;            
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string queryIns = "INSERT INTO [NRBWork].[dbo].[AliPaySftpFileUploadInfo]([FileName],[FileType],[ReportDate]) VALUES (@fileName,@fileType,@dateTimeVl)";
                SqlCommand _cmd = new SqlCommand(queryIns, connNRBWorkSystem);

                _cmd.Parameters.Add("@fileName", SqlDbType.VarChar).Value = sourceFileFullPath;
                _cmd.Parameters.Add("@fileType", SqlDbType.VarChar).Value = fileType;
                _cmd.Parameters.Add("@dateTimeVl", SqlDbType.Date).Value = rptDt;

                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc)
                {
                    //MessageBox.Show(exc.ToString());
                }
                _cmd.Dispose();
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
        }

        internal DataTable LoadAlipayUploadedFileList()
        {
            SqlConnection connNRBWorkSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                connNRBWorkSystem.Open();

                string query = "SELECT TOP 10 [AutoId] SL, convert(varchar, [UploadDate], 20)UploadDate,[FileType],[ReportDate],[FileName] FROM [NRBWork].[dbo].[AliPaySftpFileUploadInfo] ORDER BY [AutoId] DESC";

                SqlCommand cmd = new SqlCommand(query, connNRBWorkSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetPendingDataByRefNoTxnType(string refno, string TxnType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                if (TxnType.Equals("Main"))
                {
                    query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=b.PartyId ) Exh, FromAccount DebitAc, RefNo, "
                    + " case when b.IsIncentive=0 then 'MainTxn' else 'IncentiveTxn' end TxnType , "
                    + " case when PaymentStatus=1 then 'Received' "
                    + " when PaymentStatus=2 then 'Canceled' "
                    + " when PaymentStatus=3 then 'Queued' "
                    + " when PaymentStatus=5 then 'Success'	"
                    + " when PaymentStatus=6 then 'Returned' "
                    + " when PaymentStatus=7 then 'Ready for Disburse' "
                    + " when PaymentStatus=8 then 'Disbursed' "
                    + " when PaymentStatus=10 then 'Incentive Pending' "
                    + " else STR(PaymentStatus) end Status, "
                    + " Amount, BeneficiaryName, BeneficiaryAccountNo, BeneficiaryBankName, "
                    + " DestinationRoutingNO RoutingNo, RequestTime "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b  where [PaymentStatus]=3 and RefNo='" + refno + "' and IsIncentive in(0)";
                }
                else
                {
                    query = "select (select u.UserId from [RemittanceDB].[dbo].Users u where u.PartyId=b.PartyId ) Exh, FromAccount DebitAc, RefNo, "
                    + " case when b.IsIncentive=0 then 'MainTxn' else 'IncentiveTxn' end TxnType , "
                    + " case when PaymentStatus=1 then 'Received' "
                    + " when PaymentStatus=2 then 'Canceled' "
                    + " when PaymentStatus=3 then 'Queued' "
                    + " when PaymentStatus=5 then 'Success'	"
                    + " when PaymentStatus=6 then 'Returned' "
                    + " when PaymentStatus=7 then 'Ready for Disburse' "
                    + " when PaymentStatus=8 then 'Disbursed' "
                    + " when PaymentStatus=10 then 'Incentive Pending' "
                    + " else STR(PaymentStatus) end Status, "
                    +" Amount, BeneficiaryName, BeneficiaryAccountNo, BeneficiaryBankName, "
                    + " DestinationRoutingNO RoutingNo, RequestTime "
                    + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b  where [PaymentStatus]=3 and RefNo='" + refno + "' and IsIncentive in(1)";
                }

                SqlDataAdapter sdaBEFTNOnProcess = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNOnProcess.Fill(dt);
            }
            catch (Exception exc) { }
            return dt;
        }

        internal bool ChangeBEFTNTxnStatusToReceived(string pinNo, string TxnType)
        {
            SqlConnection connNewSystem = null;
            int rowsAffected = 0;
            string updStmt="";

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }
               
                if (TxnType.Equals("Main"))
                {
                    updStmt = "UPDATE [RemittanceDB].[dbo].[BEFTNRequest] SET PaymentStatus=1 WHERE [RefNo]='" + pinNo + "' AND IsIncentive in(0,99)";
                }
                else
                {
                    updStmt = "UPDATE [RemittanceDB].[dbo].[BEFTNRequest] SET PaymentStatus=1 WHERE [RefNo]='" + pinNo + "' AND IsIncentive in(1)";
                }

                SqlCommand _cmd = new SqlCommand(updStmt, connNewSystem);
                
                try
                {
                    rowsAffected = _cmd.ExecuteNonQuery();
                }
                catch (Exception exc) { return false; }
                finally
                {
                    if (connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
                }
                _cmd.Dispose();

                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception exc) { }
            return false;
        }

        internal double GetBEFTNActualCommAmount(string dtValue1, string dtValue2, int exhId, double commPercentage, int maxComm)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT isnull(sum(case when p.comm>" + maxComm + " then " + maxComm + " else p.comm end),0) actualComm "
                    + " from ( select cast(round((Amount* " + commPercentage + "),2) as decimal (10,2)) comm "
                    + " FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and convert(varchar, CBSValueDate, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "')p";

                SqlCommand cmd = new SqlCommand(query, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return Convert.ToDouble(dt.Rows[0][0]);
        }

        internal object GetBKASHActualCommAmount(string dtValue1, string dtValue2, int exhId, double commPercentage, int maxComm)
        {
            //SqlConnection connDRSystem = null;

            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            DataTable dt = new DataTable();
            try
            {
                //connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                //connDRSystem.Open();

                //string query = "SELECT isnull(sum(case when p.comm>" + maxComm + " then " + maxComm + " else p.comm end),0) actualComm "
                //    + " from ( select cast(round((CONVERT(float, mw.RemitReceiverAmount)* " + commPercentage + "),2) as decimal (10,2)) comm "
                //    + " FROM [RemittanceDB].[dbo].MobileWalletRemitTransfer mw inner join [RemittanceDB].[dbo].Transactions t on mw.TranTxnId=t.SessionId WHERE mw.ExchangeHouseID=" + exhId + " and mw.RemitStatus=5 and convert(varchar, t.ResponseDate, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "')p";

                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string query = "SELECT isnull(sum(case when p.comm>" + maxComm + " then " + maxComm + " else p.comm end),0) actualComm "
                    + " FROM ( select cast(round((CONVERT(float, [DestAmount]) * " + commPercentage + "),2) as decimal (10,2)) comm "
                    + " FROM [NRBWork].[dbo].[BkashTerrapayReportCommissionData] "
                    + " WHERE PartyId=" + exhId + " AND ([ReportFromDate] = '" + dtValue1 + "' AND [ReportToDate]= '" + dtValue2 + "') "
                    + " )p ";


                SqlCommand cmd = new SqlCommand(query, connNRBWorkSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return Convert.ToDouble(dt.Rows[0][0]);
        }

        internal bool UpdateEFTReturnStatus(string pinNo, string returnDt, string retReason, string txnType)
        {
            bool stats = false;
            SqlConnection connNewSystem = null;
            string query = "";

            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                if (txnType.Equals("MAIN"))
                {
                    query = "UPDATE [RemittanceDB].[dbo].[BEFTNRequest] SET [PaymentStatus]=6, [ReturnedTime]='" + returnDt + "', [ReturnedReason]='" + retReason + "'  WHERE [RefNo]='" + pinNo + "' and [IsIncentive]=0 ";
                }
                else
                {
                    query = "UPDATE [RemittanceDB].[dbo].[BEFTNRequest] SET [PaymentStatus]=6, [ReturnedTime]='" + returnDt + "', [ReturnedReason]='" + retReason + "'  WHERE [RefNo]='" + pinNo + "' and [IsIncentive]=1 ";
                }
                SqlCommand _cmd = new SqlCommand(query, connNewSystem);
                try
                {
                    _cmd.ExecuteNonQuery();
                    stats = true;
                }
                catch (Exception exc)
                {
                    string err = exc.Message;
                    stats = false;
                }
                _cmd.Dispose();

            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
            }

            return stats;
        }

        internal void BEFTNIncentiveMarkedSuccessManually(string pinNo, string remarks, string userId)
        {
            SqlConnection connNewSystem = null;
            try
            {
                connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                string updQuery = "";

            }
            catch (Exception exc) { }
            finally
            {
                if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }
            }
        }

        internal DataTable GetBEFTNForeignerSenderName()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [ID],[SENDERNAME] SENDER_NAME, convert(varchar, [CREATEDATE], 20)CREATE_DATE  FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER] order by ID desc";
                SqlDataAdapter sdaBEFTNSender = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNSender.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable SearchForeignerSenderNameByInput(string name)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT ID, SENDERNAME from [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER] where upper(SENDERNAME) LIKE upper('%" + name + "%')";
                SqlDataAdapter sdaComp = new SqlDataAdapter(query, connDRSystem);
                sdaComp.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool SaveNewForeignSenderName(string senderName)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                DataTable dt = new DataTable();
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                //string sql = "select max(id)+1 FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER]";
                //SqlDataAdapter sdaComp = new SqlDataAdapter(sql, connLiveSystem);
                //sdaComp.Fill(dt);
                //int senderId = Convert.ToInt32(dt.Rows[0][0]);

                //string insStmt = "IF NOT EXISTS (SELECT * FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER]  WHERE upper([SENDERNAME]) = '" + senderName + "') "
                //    + " BEGIN  INSERT INTO [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER] ([ID],[SENDERNAME])  VALUES (" + senderId + ",'" + senderName + "')  END ";

                string insStmt = "IF NOT EXISTS (SELECT * FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER]  WHERE upper([SENDERNAME]) = '" + senderName + "') "
                    + " BEGIN  INSERT INTO [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER] ([SENDERNAME])  VALUES ('" + senderName + "')  END ";

                SqlCommand _cmd = new SqlCommand(insStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }

        internal bool RemoveForeignerSenderById(int senderId)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string delStmt = "DELETE FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER] WHERE [ID]=" + senderId + "";
                SqlCommand _cmd = new SqlCommand(delStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }

        internal bool RemoveCompanyNameById(int compId)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string delStmt = "DELETE FROM [RemittanceDB].[dbo].[BEFTNCompanyName] WHERE [ID]=" + compId + "";
                SqlCommand _cmd = new SqlCommand(delStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }


        internal DataTable GetBEFTNForeignerSenderAccount()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [ID],[ACCOUNT_NO], convert(varchar, [CREATEDATE], 20)CREATE_DATE  FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER_ACCOUNT] order by ID desc";
                SqlDataAdapter sdaBEFTNSender = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNSender.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable SearchForeignerSenderAccountByInput(string name)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT ID, ACCOUNT_NO from [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER_ACCOUNT] where upper(ACCOUNT_NO) LIKE upper('%" + name + "%')";
                SqlDataAdapter sdaComp = new SqlDataAdapter(query, connDRSystem);
                sdaComp.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool SaveNewForeignSenderAccount(string senderAccount)
        {
            SqlConnection connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
            try
            {
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string insStmt = "IF NOT EXISTS (SELECT * FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER_ACCOUNT]  WHERE upper([ACCOUNT_NO]) = '" + senderAccount + "') "
                    + " BEGIN  INSERT INTO [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER_ACCOUNT] ([ACCOUNT_NO])  VALUES ('" + senderAccount + "')  END ";

                SqlCommand _cmd = new SqlCommand(insStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }

        internal bool RemoveForeignerSenderAccountById(int senderId)
        {
            SqlConnection connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
            try
            {                
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string delStmt = "DELETE FROM [RemittanceDB].[dbo].[BEFTN_SENDER_FOREIGNER_ACCOUNT] WHERE [ID]=" + senderId + "";
                SqlCommand _cmd = new SqlCommand(delStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }


        
        /*
        internal DataSet GetAllPaymodeCommissionDataByExchangeHouse(int exhId, string exhName, string dtValue1, string dtValue2, string previousMonthLastDayDate, 
            string forwardDate)
        {
            SqlConnection connDRSystem = null;
            DataSet ds = new DataSet();

            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string finalQuery = "";

                string eftQuery = "SELECT PartyId, RefNo, case when PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, Amount, CBSValueDate TxnDate FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and (convert(varchar, CBSValueDate, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "')"
                    + " UNION ALL "
                    + " SELECT PartyId, RefNo, case when PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, Amount, LastProcessingTime TxnDate  FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and CBSValueDate is null and (convert(varchar, LastProcessingTime, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "'); ";

                string mtbQuery = "select tr.UserId, tr.SessionId PINNumber, tr.Amount, tr.CrAccountNo, tr.Reason Remarks, tr.IsSuccess, tr.TransactionDate, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('101', '102', '201', '202') "
                    + " and CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "' and tr.IsSuccess = 1 and tr.UserId=" + exhId + " ORDER BY CONVERT(DATETIME, tr.CBSValueDate, 105);";

                string cashQuery = "SELECT PartyId, RemitenceId, Amount,RequestTime, ClearingDate FROM [RemittanceDB].[dbo].Remittanceinfo WHERE PartyId=" + exhId + " AND Status=8 and convert(varchar, ClearingDate, 23) between '" + dtValue1 + "' AND '" + dtValue2 + "';";

                
                "SELECT ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate, tr.TransactionDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('105','205') "
                    + " and (CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and tr.IsSuccess = 1 and tr.UserId=" + exhId
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('105','205') "
                    + " and (CONVERT(DATETIME, trn.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) "
                    

                    + " UNION "
                    + " SELECT tr.ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate, tr.TransactionDate "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitDirectSettlement] d INNER JOIN [RemittanceDB].[dbo].Transactions tr  ON d.TxnId = tr.SessionId  AND d.PartyID = tr.UserId "
                    + " WHERE d.StatusID = 2 AND tr.IsSuccess = 1 AND tr.Type = '107' AND (CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') "
                    + " and tr.UserId=" + exhId + " "                   
                    + " UNION "
                    + " SELECT ID, tr.SessionId PINNumber, tr.Amount, tr.Reason Remarks, tr.CBSTransactionCodeDr Journal, tr.CBSUniqueNumber, tr.CBSValueDate, tr.TransactionDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('106') "
                    + " and (CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and tr.IsSuccess = 1 and tr.UserId=" + exhId
                    + " and tr.SessionId not in ( SELECT trn.SessionId FROM [RemittanceDB].[dbo].Transactions trn where trn.[Type] IN ('106') "
                    + " and (CONVERT(DATETIME, trn.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "') and trn.UserId=" + exhId
                    + " and trn.IsSuccess = 1 group by trn.UserId, trn.SessionId having count(*)>1 ) "
                    
                    + "";

                finalQuery = eftQuery + mtbQuery + cashQuery + bkashQuery;

                using (SqlCommand cmd = new SqlCommand(finalQuery))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = connDRSystem;
                        sda.SelectCommand = cmd;                            
                        sda.Fill(ds);
                        ds.Tables[0].TableName = "BEFTNData";
                        ds.Tables[1].TableName = "MTBData";
                        ds.Tables[2].TableName = "CashData";
                        ds.Tables[3].TableName = "bKashData";                           
                    }
                }               
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return ds;
        }
        */


        internal DataSet GetEFTMTBAcModeCommissionDataByExchangeHouseNEW(int exhId, string exhName, string dtValue1, string dtValue2)
        {
            SqlConnection connDRSystem = new SqlConnection(connInfo.getConnStringDR());
            DataSet ds = new DataSet();

            try
            {
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }
                
                string finalQuery = "";

                string eftQuery = "SELECT PartyId, RefNo, case when PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, Amount, CBSValueDate TxnDate FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and (convert(varchar, CBSValueDate, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "')"
                    + " UNION ALL "
                    + " SELECT PartyId, RefNo, case when PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, Amount, LastProcessingTime TxnDate  FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and CBSValueDate is null and (convert(varchar, LastProcessingTime, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "'); ";

                string mtbQuery = "select tr.UserId, tr.SessionId PINNumber, tr.Amount, tr.CrAccountNo, tr.Reason Remarks, tr.IsSuccess, tr.TransactionDate, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('101', '102', '201', '202') "
                    + " and CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "' and tr.IsSuccess = 1 and tr.UserId=" + exhId + " ORDER BY CONVERT(DATETIME, tr.CBSValueDate, 105);";
                                
                finalQuery = eftQuery + mtbQuery;

                using (SqlCommand cmd = new SqlCommand(finalQuery))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = connDRSystem;
                        sda.SelectCommand = cmd;
                        sda.Fill(ds);
                        ds.Tables[0].TableName = "BEFTNData";
                        ds.Tables[1].TableName = "MTBData";
                    }
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return ds;
        }

        
        internal DataSet GetCashBkashModeCommissionDataByExchangeHouseNEW(int exhId, string exhName, string dtValue1, string dtValue2)
        {
            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            DataSet ds = new DataSet();

            try
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string finalQuery = "";                                
                string cashQuery = "SELECT [PartyId],[ExchangeHouseName],[RefNo],[BeneficiaryName],[Amount],[PaymentDate], [BranchCode], [BranchName], [ReportFromDate],[ReportToDate] "
                    + " FROM [NRBWork].[dbo].[CashTxnPanBankReport] WHERE PartyId=" + exhId + " AND [ReportFromDate]='" + dtValue1 + "' AND [ReportToDate]='" + dtValue2 + "'  order by AutoId;";


                string bkashQuery = "Select comm.[PartyId], exh.ExchangeHouseShortName ExhName, comm.[RefNo], comm.ReceiverWallet, comm.[DestAmount], comm.ConversationId, comm.bKashTxnId, comm.ReceiverName,  comm.[TxnMode], comm.ReportFromDate, comm.ReportToDate, comm.ReceivedTime, comm.LastUpdateTime "
                    + " FROM [NRBWork].[dbo].[BkashTerrapayReportCommissionData] comm inner join [NRBWork].[dbo].[ExchangeHouseInfoList] exh ON comm.PartyId=exh.PartyId "
                    + " WHERE (comm.[ReportFromDate]='" + dtValue1 + "' AND comm.[ReportToDate]='" + dtValue2 + "') AND comm.PartyId=" + exhId + "";

                finalQuery = cashQuery + bkashQuery;

                using (SqlCommand cmd = new SqlCommand(finalQuery))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = connNRBWorkSystem;
                        sda.SelectCommand = cmd;
                        sda.Fill(ds);
                        ds.Tables[0].TableName = "CashData";
                        ds.Tables[1].TableName = "bKashData";
                    }
                }
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return ds;
        }

        /*
        internal DataSet GetAllPaymodeCommissionDataByExchangeHouseNEW(int exhId, string exhName, string dtValue1, string dtValue2)
        {
            SqlConnection connDRSystem = new SqlConnection(connInfo.getConnStringDR());
            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            DataSet ds = new DataSet();

            try
            {
                //connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string finalQuery = "";

                string eftQuery = "SELECT PartyId, RefNo, case when PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, Amount, CBSValueDate TxnDate FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and (convert(varchar, CBSValueDate, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "')"
                    + " UNION ALL "
                    + " SELECT PartyId, RefNo, case when PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, Amount, LastProcessingTime TxnDate  FROM [RemittanceDB].[dbo].BEFTNRequest WHERE PartyId=" + exhId + " AND PaymentStatus in(5,6) and IsIncentive in(99,0) and CBSValueDate is null and (convert(varchar, LastProcessingTime, 23) BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "'); ";

                string mtbQuery = "select tr.UserId, tr.SessionId PINNumber, tr.Amount, tr.CrAccountNo, tr.Reason Remarks, tr.IsSuccess, tr.TransactionDate, tr.CBSValueDate "
                    + " FROM [RemittanceDB].[dbo].Transactions tr where tr.[Type] IN ('101', '102', '201', '202') "
                    + " and CONVERT(DATETIME, tr.CBSValueDate, 105) between '" + dtValue1 + "' AND '" + dtValue2 + "' and tr.IsSuccess = 1 and tr.UserId=" + exhId + " ORDER BY CONVERT(DATETIME, tr.CBSValueDate, 105);";

                string cashQuery = "SELECT [PartyId],[ExchangeHouseName],[RefNo],[BeneficiaryName],[Amount],[PaymentDate], [BranchCode], [BranchName], [ReportFromDate],[ReportToDate] "
                    + " FROM [NRBWork].[dbo].[CashTxnPanBankReport] WHERE PartyId=" + exhId + " AND [ReportFromDate]='" + dtValue1 + "' AND [ReportToDate]='" + dtValue2 + "'  order by AutoId;";


                string bkashQuery = "";

                finalQuery = eftQuery + mtbQuery + cashQuery + bkashQuery;

                using (SqlCommand cmd = new SqlCommand(finalQuery))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = connDRSystem;
                        sda.SelectCommand = cmd;
                        sda.Fill(ds);
                        ds.Tables[0].TableName = "BEFTNData";
                        ds.Tables[1].TableName = "MTBData";
                        ds.Tables[2].TableName = "CashData";
                        ds.Tables[3].TableName = "bKashData";
                    }
                }  
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return ds;
        }
        */


        internal DataTable GetBkashReversalFailedList(string dtValue1, string dtValue2)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                connDRSystem.Open();

                string query = "SELECT mw.AutoID, tr.ID, mw.ExchangeHouseID PartyId, mw.TranTxnId PIN,  mw.RemitReceiverMSISDN WalletNo, mw.RemitReceiverAmount Amount, mw.CBresponseCode bKashResp, mw.CBResponseDescription bKashRespDesc, mw.RemitStatus, convert(varchar, mw.RequestTime, 20)RequestTime, convert(varchar, mw.MTBProcessTime, 20)ProcessTime, mw.isReversed, tr.CBSTransactionCodeDr Journal, tr.CBSValueDate, tr.CBSUniqueNumber "
                    + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] mw left join [RemittanceDB].[dbo].[Transactions] tr on mw.TranTxnId=tr.SessionId WHERE isReversed=2 AND mw.MTBProcessTime BETWEEN '" + dtValue1 + "' AND '" + dtValue2 + "'  and tr.Reason like '%REV%'  ORDER BY AutoID DESC";

                SqlDataAdapter sdaRev = new SqlDataAdapter(query, connDRSystem);
                sdaRev.Fill(dt);
            }
            catch (Exception exc) { }
            return dt;
        }

        internal bool SaveBranchCashTxnFileIntoDB(DataTable branchCashTxnData, ref int CashTxnSaveCount)
        {
            bool insertStats = false;
            int saveCount = 0;
            int partyId = 0;

            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

            string nameMatchQry = "SELECT [AutoId],[PartyId],[ExchShortName],[ExchFullName] FROM [NRBWork].[dbo].[CashTxnPanBankNameMatching] WHERE [isActive]=1";
            DataTable dtExhShortName = new DataTable();
            SqlDataAdapter sdaShortName = new SqlDataAdapter(nameMatchQry, connNRBWorkSystem);
            sdaShortName.Fill(dtExhShortName);
            //--------------------
            
            for (int ii = 0; ii < branchCashTxnData.Rows.Count; ii++)
            {
                try
                {
                    if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                    partyId = GetPartyIdByExchangeHouseName(branchCashTxnData.Rows[ii]["ExchangeHouseName"].ToString().Trim().ToUpper(), dtExhShortName);

                    SqlCommand cmdSaveData = new SqlCommand();

                    string saveQueryUser = "INSERT INTO [dbo].[CashTxnPanBankReport]([RefNo],[RemitterName],[RemitterDistrict],[SendingCountry],[Amount],[BeneficiaryName],"
                        + " [ExchangeHouseName],[PaymentDate],[BranchCode],[BranchName],[BeneficiaryNID],[BeneficiaryMobile],[PaymentMode],[ReportFromDate],[ReportToDate],[PartyId])"
                        + " VALUES (@RefNo,@RemitterName,@RemitterDistrict,@SendingCountry,@Amount,@BeneficiaryName,@ExchangeHouseName,@PaymentDate,@BranchCode "
                        + " ,@BranchName,@BeneficiaryNID,@BeneficiaryMobile,@PaymentMode,@ReportFromDate,@ReportToDate,@PartyId)";

                    cmdSaveData.CommandType = CommandType.Text;
                    cmdSaveData.CommandText = saveQueryUser;
                    cmdSaveData.Connection = connNRBWorkSystem;

                    cmdSaveData.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["RefNo"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@RemitterName", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["RemitterName"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@RemitterDistrict", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["RemitterDistrict"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@SendingCountry", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["SendingCountry"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@Amount", SqlDbType.Float).Value = Math.Round(Convert.ToSingle(branchCashTxnData.Rows[ii]["Amount"]), 2);
                    cmdSaveData.Parameters.Add("@BeneficiaryName", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["BeneficiaryName"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@ExchangeHouseName", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["ExchangeHouseName"].ToString().Trim();                    
                    cmdSaveData.Parameters.Add("@PaymentDate", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["PaymentDate"];
                    cmdSaveData.Parameters.Add("@BranchCode", SqlDbType.Int).Value = Convert.ToInt32(branchCashTxnData.Rows[ii]["BranchCode"]);
                    cmdSaveData.Parameters.Add("@BranchName", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["BranchName"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@BeneficiaryNID", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["BeneficiaryNID"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@BeneficiaryMobile", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["BeneficiaryMobile"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = branchCashTxnData.Rows[ii]["PaymentMode"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@ReportFromDate", SqlDbType.Date).Value = Convert.ToDateTime(branchCashTxnData.Rows[ii]["ReportFromDate"]);
                    cmdSaveData.Parameters.Add("@ReportToDate", SqlDbType.Date).Value = Convert.ToDateTime(branchCashTxnData.Rows[ii]["ReportToDate"]);
                    cmdSaveData.Parameters.Add("@PartyId", SqlDbType.Int).Value = partyId;

                    try
                    {
                        int k = cmdSaveData.ExecuteNonQuery();
                        insertStats = true;
                        saveCount++;
                    }
                    catch (Exception ex) { }
                }
                catch (Exception ex) { insertStats = false; }
                finally
                {
                    if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
                }
            }

            CashTxnSaveCount = saveCount;
            return insertStats;
        }

        private int GetPartyIdByExchangeHouseName(string exhNameRpt, DataTable dtExhShortName)
        {
            int pId = 0;

            for (int ii = 0; ii < dtExhShortName.Rows.Count; ii++)
            {
                if (exhNameRpt.Contains(dtExhShortName.Rows[ii]["ExchShortName"].ToString().Trim().ToUpper()))
                {
                    pId = Convert.ToInt32(dtExhShortName.Rows[ii]["PartyId"]);
                    break;
                }
            }

            return pId;
        }

        internal DataTable GetIncentiveFTReportData(string dtValue1, string dtValue2, int partyId, int tranStatus, int tranType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string queryData = "";

                if (tranType == 101)
                {
                    /*
                     ExchangeHouse, ReferenceNumber, BeneName, BeneAccount, RemitterName, RemitterCountry, 
                     BeneBank, BeneBranch,  RoutingNumber,  PrincipalTranDate, IncentiveTranDate, 
                     PrincipalAmount, IncentiveAmount, ExchangeHouseAccount, RemittancePurpose, TrxRemarks, PaymentStatus, IsIncentive 
                     */
                    if (partyId > 0)
                    {
                        queryData = "SELECT ExchangeHouse, ReferenceNumber, BeneName, BeneAccount, RemitterName, RemitterCountry, '' BeneBank, '' BeneBranch, '' RoutingNumber, "
                            + " PrincipalTranDate, IncentiveTranDate, PrincipalAmount, IncentiveAmount, ExchangeHouseAccount, '' RemittancePurpose, TrxRemarks, "
                            + " case when SuccessStatus=1 then 'Yes' else 'No' end PaymentStatus, case when IsIncentive=5 then 'Paid' else 'NotPaid' end  IsIncentive "
                            + " FROM [view_IncentiveFTReport] WHERE CAST(IncentiveTranDate AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) "
                            + " AND IsIncentive = CASE " + tranStatus + " WHEN 1 THEN 5  ELSE 3 END  AND PartyId = " + partyId
                            + " ORDER BY autoid DESC";
                    }
                    else
                    {
                        queryData = "SELECT ExchangeHouse, ReferenceNumber, BeneName, BeneAccount, RemitterName, RemitterCountry, '' BeneBank, '' BeneBranch, '' RoutingNumber, "
                            + " PrincipalTranDate, IncentiveTranDate, PrincipalAmount, IncentiveAmount, ExchangeHouseAccount, '' RemittancePurpose, TrxRemarks, "
                            + " case when SuccessStatus=1 then 'Yes' else 'No' end PaymentStatus, case when IsIncentive=5 then 'Paid' else 'NotPaid' end  IsIncentive "
                            + " FROM [view_IncentiveFTReport] WHERE CAST(IncentiveTranDate AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) "
                            + " AND IsIncentive = CASE " + tranStatus + " WHEN 1 THEN 5  ELSE 3 END "
                            + " ORDER BY autoid DESC";
                    }
                }                 

                SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable GetIncentiveEFTReportData(string dtValue1, string dtValue2, int partyId, int tranStatus, int tranType)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string queryData = "";

                if (tranStatus == 1)  // success
                {
                    if (partyId > 0)
                    {
                        queryData = "SELECT U.UserId ExchangeHouse, vfsr.ReferenceNo ReferenceNumber, vfsr.BeneficiaryName BeneName, vfsr.BeneficiaryAccountNo BeneAccount, vfsr.SenderName RemitterName, vfsr.SenderAddress RemitterCountry, "
                            + " vfsr.BeneficiaryBankName BeneBank, vfsr.BeneficiaryBankBranchName BeneBranch,  vfsr.DestinationRoutingNO RoutingNumber,  br.RequestTime PrincipalTranDate, vfsr.TransactionDate IncentiveTranDate, "
                            + " br.Amount PrincipalAmount, vfsr.Amount IncentiveAmount, U.AccountNo ExchangeHouseAccount, br.PaymentDescription RemittancePurpose, vfsr.Reason TrxRemarks, "
                            + " case when vfsr.PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, vfsr.IsIncentive IsIncentive "
                            + " FROM "
                            + " ( "
                            + "    SELECT   BT.BEFTNRequestId, BT.ReferenceNo, BT.PartyId, BT.DrAccountNo, BT.CrAccountNo, BT.Type, BT.TransactionDate, BT.Reason, BT.JournalNoMTBDeduction, BT.JournalNoBEFTN, BT.Status, BR.SenderName, "
                            + "              BR.SenderAddress, BR.BeneficiaryName, BR.BeneficiaryAddress, ISNULL(BR.BeneficiaryAccountNo, '') AS BeneficiaryAccountNo, ISNULL(BR.BeneficiaryBankAccountType, '') AS BeneficiaryBankAccountType,  "
                            + "              ISNULL(BR.BeneficiaryBankName, '') AS BeneficiaryBankName, ISNULL(BR.BeneficiaryBankBranchName, '') AS BeneficiaryBankBranchName, ISNULL(BR.DestinationRoutingNO, '') AS DestinationRoutingNO,  "
                            + "              ISNULL(BR.Amount, '') AS Amount, ISNULL(BR.PaymentDescription, '') AS PaymentDescription, ISNULL(BR.StanNo, '') AS StanNo, ISNULL(BR.RequestString, '') AS RequestString, ISNULL(BR.ResponseString, '')  "
                            + "              AS ResponseString, ISNULL(BR.CurrencyId, '0') AS CurrencyId, ISNULL(BR.FromAccount, '') AS FromAccount, ISNULL(BR.ToAccountNo, '') AS ToAccountNo, ISNULL(BR.TransactionCode, '') AS TransactionCode,  "
                            + "              ISNULL(BR.PayemntBy, '') AS PayemntBy, ISNULL(BR.SCode, '') AS SCode, ISNULL(BR.UplodedBy, '') AS UplodedBy, BR.PaymentStatus, ISNULL(BR.LastProcessingTime, '') AS LastProcessingTime, ISNULL(BR.IsException,  "
                            + "              '0') AS IsException, ISNULL(BR.ExceptionRemarks, '') AS ExceptionRemarks, U.UserId, BT.ID AS TransactionId, BR.IsIncentive  "
                            + "     FROM    dbo.BEFTNRequest AS BR INNER JOIN  dbo.BEFTNTransaction AS BT ON BR.AutoId = BT.BEFTNRequestId INNER JOIN dbo.Users AS U ON U.PartyId = BT.PartyId "
                            + "     WHERE  (BR.PaymentStatus in(5,6) AND (BT.IsSuccess = 1) ) "
                            + " ) vfsr, BEFTNRequest br,  Users u "
                            + " WHERE vfsr.PartyId = U.PartyId AND br.PartyId = u.PartyId "
                            + " AND vfsr.PartyId = " + partyId
                            + " AND vfsr.ReferenceNo = br.RefNo  AND vfsr.IsIncentive = 1  AND br.IsIncentive = 0 "
                            + " AND (CAST(vfsr.TransactionDate AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) "
                            + " OR CAST(vfsr.LastProcessingTime AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) ) "
                            + " ORDER BY vfsr.LastProcessingTime DESC";
                    }
                    else
                    {
                        queryData = "SELECT U.UserId ExchangeHouse, vfsr.ReferenceNo ReferenceNumber, vfsr.BeneficiaryName BeneName, vfsr.BeneficiaryAccountNo BeneAccount, vfsr.SenderName RemitterName, vfsr.SenderAddress RemitterCountry, "
                            + " vfsr.BeneficiaryBankName BeneBank, vfsr.BeneficiaryBankBranchName BeneBranch,  vfsr.DestinationRoutingNO RoutingNumber,  br.RequestTime PrincipalTranDate, vfsr.TransactionDate IncentiveTranDate, "
                            + " br.Amount PrincipalAmount, vfsr.Amount IncentiveAmount, U.AccountNo ExchangeHouseAccount, br.PaymentDescription RemittancePurpose, vfsr.Reason TrxRemarks, "
                            + " case when vfsr.PaymentStatus=5 then 'Success' else 'Return' end PaymentStatus, vfsr.IsIncentive IsIncentive "
                            + " FROM "
                            + " ( "
                            + "    SELECT   BT.BEFTNRequestId, BT.ReferenceNo, BT.PartyId, BT.DrAccountNo, BT.CrAccountNo, BT.Type, BT.TransactionDate, BT.Reason, BT.JournalNoMTBDeduction, BT.JournalNoBEFTN, BT.Status, BR.SenderName, "
                            + "              BR.SenderAddress, BR.BeneficiaryName, BR.BeneficiaryAddress, ISNULL(BR.BeneficiaryAccountNo, '') AS BeneficiaryAccountNo, ISNULL(BR.BeneficiaryBankAccountType, '') AS BeneficiaryBankAccountType,  "
                            + "              ISNULL(BR.BeneficiaryBankName, '') AS BeneficiaryBankName, ISNULL(BR.BeneficiaryBankBranchName, '') AS BeneficiaryBankBranchName, ISNULL(BR.DestinationRoutingNO, '') AS DestinationRoutingNO,  "
                            + "              ISNULL(BR.Amount, '') AS Amount, ISNULL(BR.PaymentDescription, '') AS PaymentDescription, ISNULL(BR.StanNo, '') AS StanNo, ISNULL(BR.RequestString, '') AS RequestString, ISNULL(BR.ResponseString, '')  "
                            + "              AS ResponseString, ISNULL(BR.CurrencyId, '0') AS CurrencyId, ISNULL(BR.FromAccount, '') AS FromAccount, ISNULL(BR.ToAccountNo, '') AS ToAccountNo, ISNULL(BR.TransactionCode, '') AS TransactionCode,  "
                            + "              ISNULL(BR.PayemntBy, '') AS PayemntBy, ISNULL(BR.SCode, '') AS SCode, ISNULL(BR.UplodedBy, '') AS UplodedBy, BR.PaymentStatus, ISNULL(BR.LastProcessingTime, '') AS LastProcessingTime, ISNULL(BR.IsException,  "
                            + "              '0') AS IsException, ISNULL(BR.ExceptionRemarks, '') AS ExceptionRemarks, U.UserId, BT.ID AS TransactionId, BR.IsIncentive  "
                            + "     FROM    dbo.BEFTNRequest AS BR INNER JOIN  dbo.BEFTNTransaction AS BT ON BR.AutoId = BT.BEFTNRequestId INNER JOIN dbo.Users AS U ON U.PartyId = BT.PartyId "
                            + "     WHERE  (BR.PaymentStatus in(5,6) AND (BT.IsSuccess = 1) ) "
                            + " ) vfsr, BEFTNRequest br,  Users u "
                            + " WHERE vfsr.PartyId = U.PartyId AND br.PartyId = u.PartyId "
                            + " AND vfsr.ReferenceNo = br.RefNo  AND vfsr.IsIncentive = 1  AND br.IsIncentive = 0 "
                            + " AND (CAST(vfsr.TransactionDate AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) "
                            + " OR CAST(vfsr.LastProcessingTime AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) ) "
                            + " ORDER BY vfsr.LastProcessingTime DESC";
                    }
                }
                else
                {
                    if (partyId > 0)
                    {
                        queryData = "SELECT U.UserId ExchangeHouse, vfsr.ReferenceNo ReferenceNumber, vfsr.BeneficiaryName BeneName, vfsr.BeneficiaryAccountNo BeneAccount, vfsr.SenderName RemitterName, vfsr.SenderAddress RemitterCountry, "
                            + " vfsr.BeneficiaryBankName BeneBank, vfsr.BeneficiaryBankBranchName BeneBranch,  vfsr.DestinationRoutingNO RoutingNumber,  br.RequestTime PrincipalTranDate, vfsr.TransactionDate IncentiveTranDate, "
                            + " br.Amount PrincipalAmount, vfsr.Amount IncentiveAmount, U.AccountNo ExchangeHouseAccount, br.PaymentDescription RemittancePurpose, vfsr.Reason TrxRemarks, "
                            + " case when vfsr.PaymentStatus=2 then 'Canceled' when vfsr.PaymentStatus=7 then 'Ready for Disburse' else str(vfsr.PaymentStatus) end PaymentStatus, vfsr.IsIncentive IsIncentive "
                            + " FROM [View_BEFTNFailedReport] vfsr, BEFTNRequest br,  Users u "
                            + " WHERE vfsr.PartyId = U.PartyId AND br.PartyId = u.PartyId "
                            + " AND vfsr.PartyId = " + partyId
                            + " AND vfsr.ReferenceNo = br.RefNo  AND vfsr.IsIncentive = 1  AND br.IsIncentive = 0 "
                            + " AND (CAST(vfsr.TransactionDate AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) "
                            + " OR CAST(vfsr.LastProcessingTime AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) ) "
                            + " ORDER BY vfsr.LastProcessingTime DESC";
                    }
                    else
                    {
                        queryData = "SELECT U.UserId ExchangeHouse, vfsr.ReferenceNo ReferenceNumber, vfsr.BeneficiaryName BeneName, vfsr.BeneficiaryAccountNo BeneAccount, vfsr.SenderName RemitterName, vfsr.SenderAddress RemitterCountry, "
                            + " vfsr.BeneficiaryBankName BeneBank, vfsr.BeneficiaryBankBranchName BeneBranch,  vfsr.DestinationRoutingNO RoutingNumber,  br.RequestTime PrincipalTranDate, vfsr.TransactionDate IncentiveTranDate, "
                            + " br.Amount PrincipalAmount, vfsr.Amount IncentiveAmount, U.AccountNo ExchangeHouseAccount, br.PaymentDescription RemittancePurpose, vfsr.Reason TrxRemarks, "
                            + " case when vfsr.PaymentStatus=2 then 'Canceled' when vfsr.PaymentStatus=7 then 'Ready for Disburse' else str(vfsr.PaymentStatus) end PaymentStatus, vfsr.IsIncentive IsIncentive "
                            + " FROM [View_BEFTNFailedReport] vfsr, BEFTNRequest br,  Users u "
                            + " WHERE vfsr.PartyId = U.PartyId AND br.PartyId = u.PartyId "
                            + " AND vfsr.ReferenceNo = br.RefNo  AND vfsr.IsIncentive = 1  AND br.IsIncentive = 0 "
                            + " AND (CAST(vfsr.TransactionDate AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) "
                            + " OR CAST(vfsr.LastProcessingTime AS date) BETWEEN CAST('" + dtValue1 + "' AS date) AND CAST('" + dtValue2 + "' AS date) ) "
                            + " ORDER BY vfsr.LastProcessingTime DESC";
                    }
                }

                SqlCommand cmd = new SqlCommand(queryData, connDRSystem);
                SqlDataAdapter sdaTxn = new SqlDataAdapter(cmd);
                sdaTxn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal void Delete_CashTxnPanBankReport_TableDataByDate(string fromdt, string todt)
        {
            SqlConnection connNRBWorkSystem = null;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string delStmt = "DELETE FROM [NRBWork].[dbo].[CashTxnPanBankReport] WHERE [ReportFromDate]='" + fromdt + "' AND [ReportToDate]='" + todt + "'";
                SqlCommand _cmd = new SqlCommand(delStmt, connNRBWorkSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) {  }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
        }

        internal string GetCOCExchangeHouseCount()
        {
            DataTable dt = new DataTable();
            SqlConnection connNRBWorkSystem = null;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string cntStmt = "select count(*) FROM [NRBWork].[dbo].[ExchangeHouseInfoList] where isCOC='Y'";
                SqlDataAdapter sdaCount = new SqlDataAdapter(cntStmt, connNRBWorkSystem);                
                sdaCount.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }

            return dt.Rows[0][0].ToString();
        }

        internal void Delete_BkashTerrapayCommission_TableDataByModeDate(string fromdt, string todt, string mode, int id)
        {
            SqlConnection connNRBWorkSystem = null;
            string delStmt = "";
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                if (id == 1)
                {
                    delStmt = "DELETE FROM [NRBWork].[dbo].[BkashTerrapayReportCommissionData] WHERE [ReportFromDate]='" + fromdt + "' AND [ReportToDate]='" + todt + "' AND [TxnMode]='" + mode + "'";
                }
                else
                {
                    delStmt = "DELETE FROM [NRBWork].[dbo].[BkashTerrapayReportCommissionData] WHERE [ReportFromDate]='" + fromdt + "' AND [ReportToDate]='" + todt + "' AND [TxnMode]='" + mode + "' AND [PartyId]=" + id;
                }
                
                SqlCommand _cmd = new SqlCommand(delStmt, connNRBWorkSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
        }

        internal bool SaveBkashTerrapayCommissionDataIntoDB(TerrapayReportData rptData)
        {
            bool success = false;

            SqlConnection connNRBWorkSystem = null;
            try
            {
                connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                SqlCommand cmdSaveData = new SqlCommand();

                string saveQueryUser = "INSERT INTO [dbo].[BkashTerrapayReportCommissionData]([TxnNo],[ReceivedTime],[LastUpdateTime],[ConversationId],[PartnerTxnId],[MTOName],[SenderMSISDN],[SourceCountry],[Amount],[ReceiverWallet],[bKashTxnId],[ReceiverName],[SenderName],[ReportFromDate],[ReportToDate],[PartyId],[TxnMode]) "
                    + " VALUES (@TxnNo,@ReceivedTime,@LastUpdateTime,@ConversationId,@PartnerTxnId,@MTOName,@SenderMSISDN,@SourceCountry,@Amount,@ReceiverWallet,@bKashTxnId,@ReceiverName,@SenderName,@ReportFromDate,@ReportToDate,@PartyId,@TxnMode)";

                cmdSaveData.CommandType = CommandType.Text;
                cmdSaveData.CommandText = saveQueryUser;
                cmdSaveData.Connection = connNRBWorkSystem;

                cmdSaveData.Parameters.Add("@TxnNo", SqlDbType.Int).Value = rptData.TxnNo;
                cmdSaveData.Parameters.Add("@ReceivedTime", SqlDbType.VarChar).Value = rptData.ReceivedTime;
                cmdSaveData.Parameters.Add("@LastUpdateTime", SqlDbType.VarChar).Value = rptData.LastUpdateTime;
                cmdSaveData.Parameters.Add("@ConversationId", SqlDbType.VarChar).Value = rptData.ConversationId;
                cmdSaveData.Parameters.Add("@PartnerTxnId", SqlDbType.VarChar).Value = rptData.PartnerTxnId;
                cmdSaveData.Parameters.Add("@MTOName", SqlDbType.VarChar).Value = rptData.MTOName;
                cmdSaveData.Parameters.Add("@SenderMSISDN", SqlDbType.VarChar).Value = rptData.SenderMSISDN;
                cmdSaveData.Parameters.Add("@SourceCountry", SqlDbType.VarChar).Value = rptData.SourceCountry;
                cmdSaveData.Parameters.Add("@Amount", SqlDbType.Float).Value = Math.Round(rptData.Amount, 2);
                cmdSaveData.Parameters.Add("@ReceiverWallet", SqlDbType.VarChar).Value = rptData.ReceiverWallet;
                cmdSaveData.Parameters.Add("@bKashTxnId", SqlDbType.VarChar).Value = rptData.bKashTxnId;
                cmdSaveData.Parameters.Add("@ReceiverName", SqlDbType.VarChar).Value = rptData.ReceiverName;
                cmdSaveData.Parameters.Add("@SenderName", SqlDbType.VarChar).Value = rptData.SenderName;
                cmdSaveData.Parameters.Add("@ReportFromDate", SqlDbType.Date).Value = Convert.ToDateTime(rptData.ReportFromDate);
                cmdSaveData.Parameters.Add("@ReportToDate", SqlDbType.Date).Value = Convert.ToDateTime(rptData.ReportToDate);
                cmdSaveData.Parameters.Add("@PartyId", SqlDbType.Int).Value = rptData.PartyId;
                cmdSaveData.Parameters.Add("@TxnMode", SqlDbType.VarChar).Value = rptData.TxnMode;

                try
                {
                    int k = cmdSaveData.ExecuteNonQuery();
                    success = true;
                }
                catch (Exception ex) { success = false; }
            }
            catch (Exception exc) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }

            return success;
        }

        internal bool SaveBkashTerrapayCommissionDataIntoDB(DataTable bkashTxnData, ref int TxnSaveCount)
        {
            bool insertStats = false;
            int saveCount = 0;
            string saveQueryUser;
           
            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }
            SqlCommand cmdSaveData = new SqlCommand();

            for (int ii = 0; ii < bkashTxnData.Rows.Count; ii++)
            {
                try
                {
                    if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }
                    cmdSaveData = new SqlCommand();

                    saveQueryUser = "INSERT INTO [dbo].[BkashTerrapayReportCommissionData]([TxnNo],[ReceivedTime],[LastUpdateTime],[ConversationId],[PartnerTxnId],[PartnerName],[MTOName],[SenderMSISDN],[SourceCountry],[SourceCurrency],[SourceAmount],[DestAmount],[ReceiverWallet],[bKashTxnId],[SenderName],[ReceiverName],[ReceiverAddress],[ReportFromDate],[ReportToDate],[RefNo],[PartyId],[TxnMode]) "
                    + " VALUES (@TxnNo, @ReceivedTime, @LastUpdateTime, @ConversationId, @PartnerTxnId, @PartnerName, @MTOName, @SenderMSISDN,@SourceCountry, @SourceCurrency, @SourceAmount, @DestAmount, @ReceiverWallet, @bKashTxnId, @SenderName, @ReceiverName, @ReceiverAddress, @ReportFromDate, @ReportToDate, @RefNo, @PartyId, @TxnMode)";

                    cmdSaveData.CommandType = CommandType.Text;
                    cmdSaveData.CommandText = saveQueryUser.ToString();
                    cmdSaveData.Connection = connNRBWorkSystem;

                    cmdSaveData.Parameters.Add("@TxnNo", SqlDbType.Int).Value = bkashTxnData.Rows[ii]["TxnNo"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@ReceivedTime", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["ReceivedTime"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@LastUpdateTime", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["LastUpdateTime"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@ConversationId", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["ConversationId"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@PartnerTxnId", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["PartnerTxnId"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@PartnerName", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["PartnerName"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@MTOName", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["MTOName"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@SenderMSISDN", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["SenderMSISDN"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@SourceCountry", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["SourceCountry"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@SourceCurrency", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["SourceCurrency"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@SourceAmount", SqlDbType.Float).Value = Math.Round(Convert.ToSingle(bkashTxnData.Rows[ii]["SourceAmount"]), 2);
                    cmdSaveData.Parameters.Add("@DestAmount", SqlDbType.Float).Value = Math.Round(Convert.ToSingle(bkashTxnData.Rows[ii]["DestAmount"]), 2);
                    cmdSaveData.Parameters.Add("@ReceiverWallet", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["ReceiverWallet"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@bKashTxnId", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["bKashTxnId"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@SenderName", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["SenderName"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@ReceiverName", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["ReceiverName"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@ReceiverAddress", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["ReceiverAddress"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@ReportFromDate", SqlDbType.Date).Value = Convert.ToDateTime(bkashTxnData.Rows[ii]["ReportFromDate"].ToString().Trim());
                    cmdSaveData.Parameters.Add("@ReportToDate", SqlDbType.Date).Value = Convert.ToDateTime(bkashTxnData.Rows[ii]["ReportToDate"].ToString().Trim());
                    cmdSaveData.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["RefNo"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@PartyId", SqlDbType.Int).Value = bkashTxnData.Rows[ii]["PartyId"].ToString().Trim();
                    cmdSaveData.Parameters.Add("@TxnMode", SqlDbType.VarChar).Value = bkashTxnData.Rows[ii]["TxnMode"].ToString().Trim();

                    try
                    {
                        int k = cmdSaveData.ExecuteNonQuery();
                        insertStats = true;
                        saveCount++;
                    }
                    catch (Exception ex) { }
                }
                catch (Exception ex) { insertStats = false; }
                finally{ if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }    }  
              
            } //for

            TxnSaveCount = saveCount;
            return insertStats;
        }


        internal bool SaveBkashTerrapayCommissionDataRowWiseIntoDB(DataRow dataRow)
        {
            bool insertStats = false;
            //int saveCount = 0;
            string saveQueryUser;

            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }
            SqlCommand cmdSaveData = new SqlCommand();

            try
                {
                saveQueryUser = "INSERT INTO [dbo].[BkashTerrapayReportCommissionData]([TxnNo],[ReceivedTime],[LastUpdateTime],[ConversationId],[PartnerTxnId],[PartnerName],[MTOName],[SenderMSISDN],[SourceCountry],[SourceCurrency],[SourceAmount],[DestAmount],[ReceiverWallet],[bKashTxnId],[SenderName],[ReceiverName],[ReceiverAddress],[ReportFromDate],[ReportToDate],[RefNo],[PartyId],[TxnMode]) "
                        + " VALUES (@TxnNo, @ReceivedTime, @LastUpdateTime, @ConversationId, @PartnerTxnId, @PartnerName, @MTOName, @SenderMSISDN,@SourceCountry, @SourceCurrency, @SourceAmount, @DestAmount, @ReceiverWallet, @bKashTxnId, @SenderName, @ReceiverName, @ReceiverAddress, @ReportFromDate, @ReportToDate, @RefNo, @PartyId, @TxnMode)";

                cmdSaveData.CommandType = CommandType.Text;
                cmdSaveData.CommandText = saveQueryUser;
                cmdSaveData.Connection = connNRBWorkSystem;

                cmdSaveData.Parameters.Add("@TxnNo", SqlDbType.Int).Value = dataRow["TxnNo"].ToString().Trim();
                cmdSaveData.Parameters.Add("@ReceivedTime", SqlDbType.VarChar).Value = dataRow["ReceivedTime"].ToString().Trim();
                cmdSaveData.Parameters.Add("@LastUpdateTime", SqlDbType.VarChar).Value = dataRow["LastUpdateTime"].ToString().Trim();
                cmdSaveData.Parameters.Add("@ConversationId", SqlDbType.VarChar).Value = dataRow["ConversationId"].ToString().Trim();
                cmdSaveData.Parameters.Add("@PartnerTxnId", SqlDbType.VarChar).Value = dataRow["PartnerTxnId"].ToString().Trim();
                cmdSaveData.Parameters.Add("@PartnerName", SqlDbType.VarChar).Value = dataRow["PartnerName"].ToString().Trim();
                cmdSaveData.Parameters.Add("@MTOName", SqlDbType.VarChar).Value = dataRow["MTOName"].ToString().Trim();
                cmdSaveData.Parameters.Add("@SenderMSISDN", SqlDbType.VarChar).Value = dataRow["SenderMSISDN"].ToString().Trim();
                cmdSaveData.Parameters.Add("@SourceCountry", SqlDbType.VarChar).Value = dataRow["SourceCountry"].ToString().Trim();
                cmdSaveData.Parameters.Add("@SourceCurrency", SqlDbType.VarChar).Value = dataRow["SourceCurrency"].ToString().Trim();
                cmdSaveData.Parameters.Add("@SourceAmount", SqlDbType.Float).Value = Math.Round(Convert.ToSingle(dataRow["SourceAmount"]), 2);
                cmdSaveData.Parameters.Add("@DestAmount", SqlDbType.Float).Value = Math.Round(Convert.ToSingle(dataRow["DestAmount"]), 2);
                cmdSaveData.Parameters.Add("@ReceiverWallet", SqlDbType.VarChar).Value = dataRow["ReceiverWallet"].ToString().Trim();
                cmdSaveData.Parameters.Add("@bKashTxnId", SqlDbType.VarChar).Value = dataRow["bKashTxnId"].ToString().Trim();
                cmdSaveData.Parameters.Add("@SenderName", SqlDbType.VarChar).Value = dataRow["SenderName"].ToString().Trim();
                cmdSaveData.Parameters.Add("@ReceiverName", SqlDbType.VarChar).Value = dataRow["ReceiverName"].ToString().Trim();
                cmdSaveData.Parameters.Add("@ReceiverAddress", SqlDbType.VarChar).Value = dataRow["ReceiverAddress"].ToString().Trim();
                cmdSaveData.Parameters.Add("@ReportFromDate", SqlDbType.Date).Value = Convert.ToDateTime(dataRow["ReportFromDate"].ToString().Trim());
                cmdSaveData.Parameters.Add("@ReportToDate", SqlDbType.Date).Value = Convert.ToDateTime(dataRow["ReportToDate"].ToString().Trim());
                cmdSaveData.Parameters.Add("@RefNo", SqlDbType.VarChar).Value = dataRow["RefNo"].ToString().Trim();
                cmdSaveData.Parameters.Add("@PartyId", SqlDbType.Int).Value = dataRow["PartyId"].ToString().Trim();
                cmdSaveData.Parameters.Add("@TxnMode", SqlDbType.VarChar).Value = dataRow["TxnMode"].ToString().Trim();

                try
                {
                    int k = cmdSaveData.ExecuteNonQuery();
                    insertStats = true;
                    //saveCount++;
                }
                catch (Exception ex) { }
            }
            catch (Exception ex) { insertStats = false; }
            finally { if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); } }  

            return insertStats;
        }

        
        internal bool ChangeBkashIsReversedStatusForInitiateReversal(string pin)
        {
            bool stats = false;
            SqlConnection connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
            try
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string query = "UPDATE [RemittanceDB].[dbo].[MobileWalletRemitTransfer] SET [isReversed]=0, [ReversalRemarks]='' WHERE [RemitStatus]=6 AND [isReversed]=2 AND [TranTxnId]='" + pin + "'";

                SqlCommand _cmd = new SqlCommand(query, connLiveSystem);
                try{
                    _cmd.ExecuteNonQuery();
                    stats = true;
                }
                catch (Exception exc){
                    string err = exc.Message;
                    stats = false;
                }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return stats;
        }


        #region FREELANCER_KEYWORD
        internal DataTable GetBEFTNFreelanceKeywordName()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [ID],[KEYWORDS], convert(varchar, [CREATEDATE], 20)CREATE_DATE  FROM [RemittanceDB].[dbo].[BEFTN_FREELANCER] order by ID desc";
                SqlDataAdapter sdaBEFTNFreelance = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNFreelance.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable SearchFreelanceKeywordNameByInput(string name)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT ID, KEYWORDS from [RemittanceDB].[dbo].[BEFTN_FREELANCER] where upper(KEYWORDS) = upper('" + name + "')";
                SqlDataAdapter sdaComp = new SqlDataAdapter(query, connDRSystem);
                sdaComp.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool SaveNewFreelanceKeywordName(string keywordName)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                DataTable dt = new DataTable();
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string insStmt = "IF NOT EXISTS (SELECT * FROM [RemittanceDB].[dbo].[BEFTN_FREELANCER]  WHERE upper([KEYWORDS]) = '" + keywordName + "') "
                    + " BEGIN  INSERT INTO [RemittanceDB].[dbo].[BEFTN_FREELANCER] ([KEYWORDS])  VALUES ('" + keywordName + "')  END ";

                SqlCommand _cmd = new SqlCommand(insStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }

        internal bool RemoveFreelanceKeywordId(int keywordId)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string delStmt = "DELETE FROM [RemittanceDB].[dbo].[BEFTN_FREELANCER] WHERE [ID]=" + keywordId + "";
                SqlCommand _cmd = new SqlCommand(delStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }

        #endregion


        #region FREELANCER_ACCOUNTNO
        internal DataTable GetBEFTNFreelanceAccountNo()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT [ID],[BENEFICIARY_ACCOUNT], convert(varchar, [CREATEDATE], 20)CREATE_DATE  FROM [RemittanceDB].[dbo].[BEFTN_FREELANCER_ACCOUNTNO] order by ID desc";
                SqlDataAdapter sdaBEFTNFreelance = new SqlDataAdapter(query, connDRSystem);
                sdaBEFTNFreelance.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal DataTable SearchFreelanceAccountNoByInput(string accountNo)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT ID, BENEFICIARY_ACCOUNT from [RemittanceDB].[dbo].[BEFTN_FREELANCER_ACCOUNTNO] where upper(BENEFICIARY_ACCOUNT) = upper('" + accountNo + "')";
                SqlDataAdapter sdaComp = new SqlDataAdapter(query, connDRSystem);
                sdaComp.Fill(dt);
            }
            catch (Exception exc) { }
            finally
            {
                if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }
            }
            return dt;
        }

        internal bool RemoveFreelanceAccountNo(int keywordId)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string delStmt = "DELETE FROM [RemittanceDB].[dbo].[BEFTN_FREELANCER_ACCOUNTNO] WHERE [ID]=" + keywordId + "";
                SqlCommand _cmd = new SqlCommand(delStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }

        internal bool SaveNewFreelanceAccountNo(string accountNo)
        {
            SqlConnection connLiveSystem = null;
            try
            {
                DataTable dt = new DataTable();
                connLiveSystem = new SqlConnection(connInfo.getConnStringRemitLv());
                if (connLiveSystem.State.Equals(ConnectionState.Closed)) { connLiveSystem.Open(); }

                string insStmt = "IF NOT EXISTS (SELECT * FROM [RemittanceDB].[dbo].[BEFTN_FREELANCER_ACCOUNTNO]  WHERE upper([BENEFICIARY_ACCOUNT]) = '" + accountNo + "') "
                    + " BEGIN  INSERT INTO [RemittanceDB].[dbo].[BEFTN_FREELANCER_ACCOUNTNO] ([BENEFICIARY_ACCOUNT])  VALUES ('" + accountNo + "')  END ";

                SqlCommand _cmd = new SqlCommand(insStmt, connLiveSystem);
                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc) { return false; }
                _cmd.Dispose();
            }
            catch (Exception exc) { }
            finally
            {
                if (connLiveSystem != null && connLiveSystem.State.Equals(ConnectionState.Open)) { connLiveSystem.Close(); }
            }
            return true;
        }

        #endregion

        
        internal DataTable GetNBLPendingOrFailedTxnList(string dtValueFrom, string dtValueTo, string partyId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                string query = "SELECT REFERENCE, REMITTER_NAME REMITTER, BENEFICIARY_NAME BENEFICIARY, ACCOUNT_NO, BRANCH_NAME BRANCH, AMOUNT, AGENT_COUNTRY_NAME AGENT_COUNTRY, TXN_RECEIVE_DATE, TXN_STATUS, Remarks, CancelDate Cancel_Date "
                        + " FROM [RemittanceDB].[dbo].[NBLRequestData] "
                        + " WHERE AGENT_CODE = '" + partyId + "' and TXN_STATUS<>'PAID' AND PAYMENT_MODE='BKASH' "
                        + " AND (convert(varchar, TXN_RECEIVE_DATE, 23) BETWEEN '" + dtValueFrom + "' AND '" + dtValueTo + "') "
                        + " and REFERENCE not like '%x' "
                        + " ORDER BY TXN_RECEIVE_DATE desc";

                SqlDataAdapter sdaInct = new SqlDataAdapter(query, connDRSystem);
                sdaInct.Fill(dt);
            }
            catch (Exception exc) { }
            finally{ if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); }   }
            return dt;
        }

        internal void InsertALipayFileUploadInfoIntoLogTable(string fileName, string fileType, DateTime rptDt)
        {
            bool insertStats = false;
            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());

            try
            {                
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }

                string queryIns = "INSERT INTO [NRBWork].[dbo].[AliPaySftpFileUploadInfo]([FileName],[FileType],[ReportDate]) VALUES (@fileName,@fileType,@dateTimeVl)";
                SqlCommand _cmd = new SqlCommand(queryIns, connNRBWorkSystem);

                _cmd.Parameters.Add("@fileName", SqlDbType.VarChar).Value = fileName;
                _cmd.Parameters.Add("@fileType", SqlDbType.VarChar).Value = fileType;
                _cmd.Parameters.Add("@dateTimeVl", SqlDbType.Date).Value = rptDt;

                try { _cmd.ExecuteNonQuery(); }
                catch (Exception exc)
                {
                    //MessageBox.Show(exc.ToString());
                }
                _cmd.Dispose();

            }
            catch (Exception ex) { insertStats = false; }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
        }


        internal DataTable GetAlipayPreviousUploadFileList()
        {
            SqlConnection connNRBWorkSystem = new SqlConnection(connInfo.getNrbWorkConnString());
            DataTable dt = new DataTable();

            try
            {
                if (connNRBWorkSystem.State.Equals(ConnectionState.Closed)) { connNRBWorkSystem.Open(); }
                string query = "SELECT TOP 100 [AutoId] SL, convert(varchar, [UploadDate], 20)UploadDate,[FileType],[ReportDate],[FileName] FROM [NRBWork].[dbo].[AliPaySftpFileUploadInfo] ORDER BY [AutoId] DESC";
                SqlDataAdapter sdaOwn = new SqlDataAdapter(query, connNRBWorkSystem);
                sdaOwn.Fill(dt);
            }
            catch (Exception ex) { }
            finally
            {
                if (connNRBWorkSystem != null && connNRBWorkSystem.State.Equals(ConnectionState.Open)) { connNRBWorkSystem.Close(); }
            }
            return dt;
        }

        #region UAT screen
        internal DataTable GetUATExchangeHouseUserList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "select Id, PartyId, UserId, Password, isActive, AccountNo, MobileWalletPaymentAccount, PartyTypeId "
                    +" FROM [RemittanceDB].[dbo].[Users] where UserId not like 'USER%' and UserId not like 'SOHEL%' and PartyId>=10001 order by PartyId desc";                
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return dt;
        }
        internal bool UpdateUATExchUserInfo(int sl, string pId, string uId, string EncrPass, int uActv, string nrt, string wallet, int partyType)
        {
            bool updateStats = false;
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "UPDATE [RemittanceDB].[dbo].[Users] SET [PartyId]='" + pId + "', [UserId]='" + uId + "',[Password]='" + EncrPass + "', [isActive]=" + uActv + ""
                    + " , [AccountNo]='" + nrt + "', [MobileWalletPaymentAccount]='" + wallet + "', [PartyTypeId]=" + partyType + "  WHERE [Id]=" + sl;

                updateStats = dbManager.ExcecuteCommand(queryUser);
            }
            catch (Exception ex) { updateStats = false; }
            finally
            {
                dbManager.CloseDatabaseConnection();
            }
            return updateStats;
        }

        internal bool IsUATUserIdExistsAlready(string uId)
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();
                string queryUser = "SELECT [Id],[UserId] FROM [RemittanceDB].[dbo].[Users] WHERE [UserId]='" + uId + "'";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally{ dbManager.CloseDatabaseConnection();  }

            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        internal bool IsUATPartyIdExistsAlready(string pId)
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();
                string queryUser = "SELECT [Id],[PartyId] FROM [RemittanceDB].[dbo].[Users] WHERE [PartyId]='" + pId + "'";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally{  dbManager.CloseDatabaseConnection();  }

            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        internal bool SaveUATExchUserInfo(string pId, string uId, string pass, string nrt, string wallet, int partyType)
        {
            bool saveStats = false;
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();

                string queryUser = "INSERT INTO [RemittanceDB].[dbo].[Users]([PartyId],[UserId],[Password],[isActive],[AccountNo],[BEFTNAccountNo],[BEFTNSatelementAccountNo],[MobileWalletPaymentAccount],[PartyTypeId],[isIncentiveAllowed]) "
                    + " VALUES(" + pId + ",'" + uId + "','" + pass + "', 1, '" + nrt + "','" + nrt + "','" + nrt + "','" + wallet + "', " + partyType + ", 1) ";

                saveStats = dbManager.ExcecuteCommand(queryUser);
            }
            catch (Exception ex) { saveStats = false; }
            finally{ dbManager.CloseDatabaseConnection();   }
            return saveStats;
        }

        internal DataTable GetUATActiveExchangeHouseUserList()
        {
            DataTable dt = new DataTable();
            try
            {
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();
                string queryUser = "SELECT cast(PartyId as varchar)+' - '+UserId   FROM [RemittanceDB].[dbo].[Users] where UserId not like 'USER%' and UserId not like 'SOHEL%' and PartyId>=10001 and isActive=1 order by UserId";
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally {  dbManager.CloseDatabaseConnection();  }
            return dt;
        }

        internal DataTable GetUATRequestLogByPartyIdReqTypeId(int partyId, int reqTypeId, string userId)
        {
            DataTable dt = new DataTable();
            try
            {
                string queryUser = "";
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();

                if (reqTypeId == 0)
                {
                    queryUser = "SELECT [LogId],[UserId],convert(varchar, [RequestTime], 20)RequestTime,[Authenticated] Auth,[RequestCode],[ResponseCode] "
                    + " FROM [RemittanceDB].[dbo].[RequestLog]  where UserId='" + userId + "' order by LogId desc";
                }
                else
                {
                    queryUser = "SELECT [LogId],[UserId],convert(varchar, [RequestTime], 20)RequestTime,[Authenticated] Auth,[RequestCode],[ResponseCode] "
                    + " FROM [RemittanceDB].[dbo].[RequestLog]  where UserId='" + userId + "' AND RequestTypeId=" + reqTypeId + " order by LogId desc";
                }                
                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally { dbManager.CloseDatabaseConnection(); }
            return dt;
        }
               

        internal DataTable GetUATDataByPartyIdPaymentMode(int partyId, string userId, string paymode, string dtValueFrom, string dtValueTo)
        {
            DataTable dt = new DataTable();
            try
            {
                string queryUser = "";
                dbManager = new MTBDBManager(MTBDBManager.DatabaseType.SqlServer, remittanceUATConnectionString);
                dbManager.OpenDatabaseConnection();

                if (paymode.Equals("BEFTN"))
                {
                    queryUser = "SELECT RefNo, convert(varchar, RequestTime, 20) RequestTime, BeneficiaryAccountNo BeneficiaryAccount, Amount, SenderName, SenderAddress, BeneficiaryName, BeneficiaryAddress, BeneficiaryBankName BankName, BeneficiaryBankBranchName BranchName, DestinationRoutingNO RoutingNo, PaymentDescription "
                        + " FROM [RemittanceDB].[dbo].[BEFTNRequest] where PartyId=" + partyId
                        + " AND convert(varchar, RequestTime, 23) BETWEEN '" + dtValueFrom + "' and '" + dtValueTo + "'  order by AutoId desc";
                }
                else if (paymode.Equals("MTB"))
                {
                    queryUser = "SELECT RefNo, PaymentStatus,convert(varchar, TransDate, 20) TransDate, BeneficiaryName, BeneficiaryAccountNo, TrAmount Amount, SenderName, SenderPhone, SenderAddress, SenderCountry, Beneficiaryinfo, MsgForBeneficiary, MsgSource,  Reason Remarks "
                        + " FROM [RemittanceDB].[dbo].[FundTransferRequest] where PartyId=" + partyId
                        + " AND convert(varchar, TransDate, 23) BETWEEN '" + dtValueFrom + "' and '" + dtValueTo + "'  order by AutoTransId desc";
                }
                else if (paymode.Equals("CASH"))
                {
                    queryUser = "SELECT RemitenceId RefNo, Status, convert(varchar, RequestTime, 20) RequestTime, BeneficiaryName, BeneficiaryAddress, BeneficiaryMobileNo, Amount, SenderName, SenderAddress, SenderMobileNo "
                        + " FROM [RemittanceDB].[dbo].[Remittanceinfo] where PartyId=" + partyId
                        + " AND convert(varchar, RequestTime, 23) BETWEEN '" + dtValueFrom + "' and '" + dtValueTo + "'  order by AutoId desc";
                }
                else if (paymode.Equals("WALLET"))
                {
                    queryUser = "SELECT TranTxnId RefNo, convert(varchar, RequestTime, 20) RequestTime, TranSendingCountry SendingCountry, TranSendingCurrency SendingCurrency, TranSendingAmount SendingAmount, RemitReceiverMSISDN ReceiverWalletNo, RemitReceiverAmount ReceiverAmount, SenderFirstName, SenderLastName, SenderMSISDN, SenderDOB, SenderPOB, SenderDocumentNumber, SenderAddress, SenderNationality, PaymentInstrumentEntity, RemitStatus "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletRemitTransfer] where ExchangeHouseID=" + partyId
                        + " AND convert(varchar, RequestTime, 23) BETWEEN '" + dtValueFrom + "' and '" + dtValueTo + "'  order by AutoID desc";
                }
                else if (paymode.Equals("WALLBeneVald"))
                {
                    queryUser = "SELECT AutoID, convert(varchar, RequestTime, 20) RequestTime, ReqMSISDN, ReqFirstName, ReqLastName, ReqFullName, ResponseCode, responseMessage, ConversationID, CBTransResponseCode CallbackRespCode, CBTransResponseDescription CallbackDescription, CBWaFirstName CallbackFirstName, CBWaLastName CallbackLastName, CBWaFullName CallbackFullName, CBReceiveTime CallbackReceiveTime "
                        + " FROM [RemittanceDB].[dbo].[MobileWalletBeneficiaryInformation] where ExchangeHouseID=" + partyId
                        + " AND convert(varchar, RequestTime, 23) BETWEEN '" + dtValueFrom + "' and '" + dtValueTo + "'  order by AutoID desc";
                }

                dt = dbManager.GetDataTable(queryUser);
            }
            catch (Exception ex) { }
            finally { dbManager.CloseDatabaseConnection(); }
            return dt;
        }

        #endregion        
        
            
        internal DataTable GetBEFTNPaymentSuccessButCBSValueDateNull(string dtValue1, string dtValue2, int partyId)
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                if (partyId == 0)
                {
                    query = "select (select u.UserId from Users u where u.PartyId=b.PartyId) ExchName, FromAccount ExhAccount, RefNo, BeneficiaryName,  Amount, PaymentDescription, convert(varchar, RequestTime, 20)RequestTime, convert(varchar, LastProcessingTime, 20)ProcessTime,CBSValueDate "
                            + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b "
                            + " WHERE (convert(varchar, RequestTime, 23) between '" + dtValue1 + "' and '" + dtValue2 + "') "
                            + " AND PaymentStatus=5 and IsIncentive IN(0,99) and CBSValueDate is null  "
                            + " ORDER BY AutoId desc";
                }
                else
                {
                    query = "select (select u.UserId from Users u where u.PartyId=b.PartyId) ExchName, FromAccount ExhAccount, RefNo, BeneficiaryName,  Amount, PaymentDescription, convert(varchar, RequestTime, 20)RequestTime, convert(varchar, LastProcessingTime, 20)ProcessTime,CBSValueDate "
                            + " FROM [RemittanceDB].[dbo].[BEFTNRequest] b "
                            + " WHERE (convert(varchar, RequestTime, 23) between '" + dtValue1 + "' and '" + dtValue2 + "') "
                            + " AND PartyId=" + partyId + " AND PaymentStatus=5 and IsIncentive IN(0,99) and CBSValueDate is null  "
                            + " ORDER BY AutoId desc";
                }
                SqlDataAdapter sdaTxn = new SqlDataAdapter(query, connDRSystem);
                sdaTxn.Fill(dt);
            }
            catch (Exception exc) { }
            finally { if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); } }
            return dt;
        }

        internal bool UpdateEmptyBEFTNValueDate(string pinNumber, string dtValue1)
        {
            bool stats = false;
            SqlConnection connNewSystem = new SqlConnection(connInfo.getConnStringRemitLv());
            string query = "";

            try
            {
                if (connNewSystem.State.Equals(ConnectionState.Closed)) { connNewSystem.Open(); }

                query = "UPDATE [RemittanceDB].[dbo].[BEFTNRequest] SET [CBSValueDate]=@CBSValueDate  WHERE [RefNo]='" + pinNumber + "' and [CBSValueDate] IS NULL AND [IsIncentive]=0 AND [PaymentStatus]=5 ";
                SqlCommand cmdUpdateData = new SqlCommand();

                cmdUpdateData.CommandType = CommandType.Text;
                cmdUpdateData.CommandText = query;
                cmdUpdateData.Connection = connNewSystem;

                cmdUpdateData.Parameters.Add("@CBSValueDate", SqlDbType.Date).Value = Convert.ToDateTime(dtValue1);

                try
                {
                    int k = cmdUpdateData.ExecuteNonQuery();
                    stats = true;
                }
                catch (Exception ex) { stats = false; }
                cmdUpdateData.Dispose();
            }
            catch (Exception exc) { stats = false; }
            finally{ if (connNewSystem != null && connNewSystem.State.Equals(ConnectionState.Open)) { connNewSystem.Close(); }  }
            return stats;
        }

        internal DataTable GetExistingRoutingNumberList()
        {
            SqlConnection connDRSystem = null;
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                connDRSystem = new SqlConnection(connInfo.getConnStringDR());
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                query = "SELECT [MTB SL NO] SL, [BANK CODE], [AGENT NAME] BANK, [BRANCH NAME], DISTRICT, [ROUTING NUMBER], ISACTIVEFORBEFTNAP 'ACTIVE FOR EFT' "
                            + " FROM [RemittanceDB].[dbo].[BANK_BRANCH] "
                            + " ORDER BY [MTB SL NO] DESC";
                
                SqlDataAdapter sdaTxn = new SqlDataAdapter(query, connDRSystem);
                sdaTxn.Fill(dt);
            }
            catch (Exception exc) { }
            finally { if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); } }
            return dt;
        }

        internal DataTable GetWalletFreelanceData(string dtValue1, string dtValue2)
        {
            SqlConnection connDRSystem = new SqlConnection(connInfo.getConnStringDR());
            DataTable dt = new DataTable();
            string query = "";
            try
            {
                if (connDRSystem.State.Equals(ConnectionState.Closed)) { connDRSystem.Open(); }

                query = "SELECT (Select u.UserId from [RemittanceDB].[dbo].[Users] u WHERE u.PartyId=mt.ExchangeHouseID)ExchangeHouse , TranTxnId PIN_NO, "
                + " convert(varchar, RequestTime, 20)ReqTime, TranSendingCountry SendCountry, RemitReceiverCountry RcvCountry, SenderNationality, RemitReceiverAmount Amount, "
                + " RemitReceiverMSISDN WalletNo,  CBResponseDescription bKashResp, "
                + " CASE WHEN RemitStatus=5 THEN 'Success' WHEN RemitStatus=6 THEN 'Cancelled'  WHEN RemitStatus=3 THEN 'Queued'  WHEN RemitStatus=4 THEN 'Acknowledged' ELSE STR(RemitStatus) END Status , "
                + " convert(varchar, MTBProcessTime, 20)ProcessTime, isReversed "
                + " FROM [RemittanceDB].[dbo].MobileWalletRemitTransfer mt WHERE mt.TranTxnId in( "
                + " SELECT [RefNo] FROM [RemittanceDB].[dbo].[MobileWalletFreeLanceTransaction] WHERE convert(varchar, [TransactionTime], 23) between '" + dtValue1 + "' and '" + dtValue2 + "' ) "
                + " order by mt.MTBProcessTime desc";

                SqlDataAdapter sdaTxn = new SqlDataAdapter(query, connDRSystem);
                sdaTxn.Fill(dt);
            }
            catch (Exception exc) { }
            finally { if (connDRSystem != null && connDRSystem.State.Equals(ConnectionState.Open)) { connDRSystem.Close(); } }
            return dt;
        }
    }
}