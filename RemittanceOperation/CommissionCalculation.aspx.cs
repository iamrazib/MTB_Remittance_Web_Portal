﻿using ClosedXML.Excel;
using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class CommissionCalculation : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtAllPayModeAllExhouseData = new DataTable();
        static DataTable dtAllPayModeAllExhouseCommissionData = new DataTable();

        string TxnRemarks = "TT CHARGE ALL TXNS ";
        string VatRemarks = "15% VAT on ";
        string CommRemarks = "TT Commission ";
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                DateTime thisMonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                
                //dtpickerMonthlyCommFrom.Text = thisMonthFirstDay.ToString("yyyy-MM-dd");
                //dtpickerMonthlyCommTo.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                dtpickerMonthlyCommFrom.Text = thisMonthFirstDay.AddMonths(-1).ToString("yyyy-MM-dd");
                dtpickerMonthlyCommTo.Text = thisMonthFirstDay.AddDays(-1).ToString("yyyy-MM-dd");


                LoadAPIBasedExchListSummary();
                //LoadPaymentMode();
            }
        }

        //private void LoadPaymentMode()
        //{
        //    cbPaymentMode.Items.Clear();
        //    cbPaymentMode.Items.Add("--Select--");
        //    cbPaymentMode.Items.Add("BEFTN");
        //    cbPaymentMode.Items.Add("MTB");
        //    cbPaymentMode.Items.Add("BKASH");
        //    cbPaymentMode.Items.Add("CASH");
        //    cbPaymentMode.SelectedIndex = 0;
        //}

        private void LoadAPIBasedExchListSummary()
        {
            DataTable dtExchs = mg.GetAPIActiveExchList();
            cbExchWiseSumr.Items.Clear();
            cbExchWiseSumr.Items.Add("--Select--");

            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                if (!dtExchs.Rows[rows]["PartyId"].ToString().Equals("0"))
                {
                    cbExchWiseSumr.Items.Add(dtExchs.Rows[rows]["PartyId"] + "-" + dtExchs.Rows[rows]["ExShortName"]);
                }
            }
            cbExchWiseSumr.SelectedIndex = 0;
        }

        protected void btnCalculateCommission_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerMonthlyCommFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerMonthlyCommTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            DateTime previousMonthLastDay;
            string previousMonthLastDayDate, forwardDate;

            dtAllPayModeAllExhouseCommissionData = new DataTable();

            dtAllPayModeAllExhouseCommissionData.Columns.Add("PartyId");//0
            dtAllPayModeAllExhouseCommissionData.Columns.Add("ExHouseName");//1
            dtAllPayModeAllExhouseCommissionData.Columns.Add("NRTAccount");//2
            dtAllPayModeAllExhouseCommissionData.Columns.Add("BEFTNCount");//3
            dtAllPayModeAllExhouseCommissionData.Columns.Add("MTBCount");//4
            dtAllPayModeAllExhouseCommissionData.Columns.Add("CashCount");//5
            dtAllPayModeAllExhouseCommissionData.Columns.Add("bKashCount");//6

            dtAllPayModeAllExhouseCommissionData.Columns.Add("EFTRate");//7
            dtAllPayModeAllExhouseCommissionData.Columns.Add("BKASHRate");//8
            dtAllPayModeAllExhouseCommissionData.Columns.Add("CASHRate");//9
            dtAllPayModeAllExhouseCommissionData.Columns.Add("ACCRate");//10

            dtAllPayModeAllExhouseCommissionData.Columns.Add("EFTCalculatedCommission");//11
            dtAllPayModeAllExhouseCommissionData.Columns.Add("BKASHCalculatedCommission");//12
            dtAllPayModeAllExhouseCommissionData.Columns.Add("CASHCalculatedCommission");//13
            dtAllPayModeAllExhouseCommissionData.Columns.Add("MTBCalculatedCommission");//14
            dtAllPayModeAllExhouseCommissionData.Columns.Add("Currency");//15
            dtAllPayModeAllExhouseCommissionData.Columns.Add("TotalCommission");//16


            DataRow drowTotal;
            int exhId;
            string exhName, nrtAcc, partyTypeId;
            int totalEFTCount = 0, totalMTBCount = 0, totalBkashCount = 0, totalCashCount = 0;

            decimal totalBDTCommAmount = 0, totalUSDCommAmount = 0;

            DataTable dtExhList = mg.GetAPIActiveExchList();

            // this method fetch each EXCH commission rate
            DataTable dtExchCommList = mg.GetExchCommissionData();



            previousMonthLastDay = dateTime1.AddDays(-1);
            previousMonthLastDayDate = previousMonthLastDay.ToString("yyyy-MM-dd");
            forwardDate = dateTime2.AddDays(1).ToString("yyyy-MM-dd");


            for (int ii = 0; ii < dtExhList.Rows.Count; ii++)
            {
                exhId = Convert.ToInt32(dtExhList.Rows[ii]["PartyId"]);
                exhName = dtExhList.Rows[ii]["ExShortName"].ToString();
                nrtAcc = dtExhList.Rows[ii]["NRTAccount"].ToString();
                partyTypeId = dtExhList.Rows[ii]["PartyTypeId"].ToString();

                DataTable dtBEFTN = mg.GetBEFTNTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                //DataTable dtBEFTNReturn = mg.GetBEFTNReturnTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtOwnAcCredit = mg.GetMTBTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtCashCredit = mg.GetCashTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                
                //DataTable dtbKashReg = mg.GetbKashRegTxnSummaryByExchId(dtValue1, dtValue2, exhId);

                DataTable dtbKash = mg.GetbKashTxnSummaryByExchId(dtValue1, dtValue2, exhId);

                //---------------------------------
                //if (exhId == 10014)
                //{
                //    int a = 1;
                //}
                
                /*
                DataTable dtbKashRegSpecialCase = mg.GetbKashRegSpecialDateCaseSummaryByExchId(dtValue1, previousMonthLastDayDate, exhId);
                if (dtbKashRegSpecialCase.Rows.Count > 0)
                {
                    dtbKashReg.Rows[0][1] = Convert.ToInt32(dtbKashReg.Rows[0][1]) + Convert.ToInt32(dtbKashRegSpecialCase.Rows[0][1]);
                    dtbKashReg.Rows[0][2] = Convert.ToDecimal(dtbKashReg.Rows[0][2]) + Convert.ToDecimal(dtbKashRegSpecialCase.Rows[0][2]);
                }

                DataTable dtbKashRegForwardDateCBSIssue = mg.GetbKashRegForwardDateCBSIssueSummaryByExchId(dtValue2, forwardDate, exhId);
                if (dtbKashRegSpecialCase.Rows.Count > 0)
                {
                    dtbKashReg.Rows[0][1] = Convert.ToInt32(dtbKashReg.Rows[0][1]) + Convert.ToInt32(dtbKashRegForwardDateCBSIssue.Rows[0][1]);
                    dtbKashReg.Rows[0][2] = Convert.ToDecimal(dtbKashReg.Rows[0][2]) + Convert.ToDecimal(dtbKashRegForwardDateCBSIssue.Rows[0][2]);
                }
                */
                //---------------------------------

                /*
                if (partyTypeId.Equals("2"))   // service rem
                {
                    DataTable dtbKashServiceRem = mg.GetbKashServiceRemTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                    if (dtbKashServiceRem.Rows.Count > 0)
                    {
                        dtbKashReg.Rows[0][1] = Convert.ToInt32(dtbKashReg.Rows[0][1]) + Convert.ToInt32(dtbKashServiceRem.Rows[0][1]);
                        dtbKashReg.Rows[0][2] = Convert.ToDecimal(dtbKashReg.Rows[0][2]) + Convert.ToDecimal(dtbKashServiceRem.Rows[0][2]);
                    }
                }
                */

                /*
                if (exhName.ToUpper().Contains("DIRECT") || exhName.ToUpper().Contains("WORLD FIRST"))
                {
                    DataTable dtbKashDir = mg.GetbKashDirectTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                    if (dtbKashDir.Rows.Count > 0)
                    {
                        dtbKashReg.Rows[0][1] = Convert.ToInt32(dtbKashReg.Rows[0][1]) + Convert.ToInt32(dtbKashDir.Rows[0][1]);
                        dtbKashReg.Rows[0][2] = Convert.ToDecimal(dtbKashReg.Rows[0][2]) + Convert.ToDecimal(dtbKashDir.Rows[0][2]);
                    }
                }
                */

                //------------------ripple ----------
                //if (exhName.ToUpper().Contains("RIPPLE"))
                //{
                //    string validator = mg.GetRippleValidatorByPartyId(exhId);
                //    DataTable dtbKashRipple = mg.GetRippleBkashTxnSummaryByExchId(dtValue1, dtValue2, validator);
                //    if (dtbKashRipple.Rows.Count > 0)
                //    {
                //        dtbKashReg.Rows[0][1] = Convert.ToInt32(dtbKashReg.Rows[0][1]) + Convert.ToInt32(dtbKashRipple.Rows[0][0]);
                //        dtbKashReg.Rows[0][2] = Convert.ToDecimal(dtbKashReg.Rows[0][2]) + Convert.ToDecimal(dtbKashRipple.Rows[0][1]);
                //    }
                //}
                //-----------------------------------


                drowTotal = dtAllPayModeAllExhouseCommissionData.NewRow();
                drowTotal[0] = exhId;  //PartyId
                drowTotal[1] = exhName; //ExHouseName
                drowTotal[2] = nrtAcc;
                drowTotal[3] = dtBEFTN.Rows[0][1].ToString(); // BEFTNCount
                drowTotal[4] = dtOwnAcCredit.Rows[0][1].ToString(); // MTBCount
                drowTotal[5] = dtCashCredit.Rows[0][1].ToString(); // CashCount
                drowTotal[6] = dtbKash.Rows[0][1].ToString(); // bKashCount

                drowTotal[7] = GetPartyRate(exhId, "EFT", dtExchCommList);
                drowTotal[8] = GetPartyRate(exhId, "BKASH", dtExchCommList);
                drowTotal[9] = GetPartyRate(exhId, "CASH", dtExchCommList);
                drowTotal[10] = GetPartyRate(exhId, "MTBAC", dtExchCommList);

                totalEFTCount = Convert.ToInt32(drowTotal[3]); // +Convert.ToInt32(drowTotal[3]);
                totalMTBCount = Convert.ToInt32(drowTotal[4]);
                totalCashCount = Convert.ToInt32(drowTotal[5]);
                totalBkashCount = Convert.ToInt32(drowTotal[6]);

                //----------- calc total amount
                drowTotal[11] = Convert.ToDecimal(totalEFTCount) * Convert.ToDecimal(drowTotal[7]);    //EFT
                drowTotal[12] = Convert.ToDecimal(totalBkashCount) * Convert.ToDecimal(drowTotal[8]);  //BKASH
                drowTotal[13] = Convert.ToDecimal(totalCashCount) * Convert.ToDecimal(drowTotal[9]);   //CASH
                drowTotal[14] = Convert.ToDecimal(totalMTBCount) * Convert.ToDecimal(drowTotal[10]);   //MTB AC

                if (exhId == 10034) // THUNES S.REM
                {
                    drowTotal[11] = GetBEFTNSpecialCommAmount(dtValue1, dtValue2, exhId, 0.025, 20);// 2.5% or 20 which ever is lower
                    drowTotal[12] = GetBKASHSpecialCommAmount(dtValue1, dtValue2, exhId, 0.025, 20);
                }

                if (exhId == 10048) // PMMAX S.REM
                {
                    drowTotal[11] = GetBEFTNSpecialCommAmount(dtValue1, dtValue2, exhId, 0.03, 100);// 3% or 100 which ever is lower
                    drowTotal[12] = GetBKASHSpecialCommAmount(dtValue1, dtValue2, exhId, 0.03, 100);
                }

                if (exhId == 10053)  // PIPO
                {
                    drowTotal[11] = 0;
                    drowTotal[12] = GetBKASHSpecialCommAmount(dtValue1, dtValue2, exhId, 0.0225, 60);// 2.25% or 60 which ever is lower
                }


                drowTotal[15] = GetPartyCurrency(exhId, dtExchCommList);
                drowTotal[16] = Convert.ToDecimal(drowTotal[11]) + Convert.ToDecimal(drowTotal[12]) + Convert.ToDecimal(drowTotal[13]) + Convert.ToDecimal(drowTotal[14]);

                dtAllPayModeAllExhouseCommissionData.Rows.Add(drowTotal);


                if (drowTotal[15].ToString().Equals("BDT"))
                {
                    totalBDTCommAmount += Convert.ToDecimal(drowTotal[16]);
                }
                else
                {
                    totalUSDCommAmount += Convert.ToDecimal(drowTotal[16]);
                }

            }

            dataGridViewCalculateCommission.DataSource = null;
            dataGridViewCalculateCommission.DataSource = dtAllPayModeAllExhouseCommissionData;
            dataGridViewCalculateCommission.DataBind();

            lblTotalBDTCommission.Text = String.Format("{0:0.00}", totalBDTCommAmount);
            lblTotalUSDCommission.Text = String.Format("{0:0.00}", totalUSDCommAmount);
        }

        private object GetBKASHSpecialCommAmount(string dtValue1, string dtValue2, int exhId, double commPercentage, int maxComm)
        {
            return mg.GetBKASHActualCommAmount(dtValue1, dtValue2, exhId, commPercentage, maxComm);
        }

        private object GetBEFTNSpecialCommAmount(string dtValue1, string dtValue2, int exhId, double commPercentage, int maxComm)
        {
            return mg.GetBEFTNActualCommAmount(dtValue1, dtValue2, exhId, commPercentage, maxComm);
        }

        private object GetPartyCurrency(int exhId, DataTable dtExchCommList)
        {
            object curr = "";
            for (int kk = 0; kk < dtExchCommList.Rows.Count; kk++)
            {
                if (Convert.ToInt32(dtExchCommList.Rows[kk]["PartyId"]) == exhId)
                {
                    curr = dtExchCommList.Rows[kk]["CommCurrency"]; break;
                }
            }

            if (exhId == 0)
                curr = "BDT";

            return curr;
        }

        private object GetPartyRate(int exhId, string rateField, DataTable dtExchCommList)
        {
            object rateVal = 0;

            for (int kk = 0; kk < dtExchCommList.Rows.Count; kk++)
            {
                if (Convert.ToInt32(dtExchCommList.Rows[kk]["PartyId"]) == exhId)
                {
                    if (rateField.Equals("EFT"))
                    {
                        rateVal = dtExchCommList.Rows[kk]["EFTRate"]; break;
                    }
                    else if (rateField.Equals("BKASH"))
                    {
                        rateVal = dtExchCommList.Rows[kk]["BKASHRate"]; break;
                    }
                    else if (rateField.Equals("CASH"))
                    {
                        rateVal = dtExchCommList.Rows[kk]["CASHRate"]; break;
                    }
                    else
                    {
                        rateVal = dtExchCommList.Rows[kk]["ACCRate"]; break;
                    }
                }
            }

            return rateVal;
        }

        //protected void btnDownloadMonthlyCommissionDataAsExcel_Click(object sender, EventArgs e)
        //{
        //    string fileName = "MonthlyCommissionSummaryData__" + dtpickerMonthlyCommFrom.Text + "_to_" + dtpickerMonthlyCommTo.Text + ".xls";

            
        //    if (dtAllPayModeAllExhouseCommissionData.Rows.Count > 0)
        //    {
        //        StringWriter tw = new StringWriter();
        //        HtmlTextWriter hw = new HtmlTextWriter(tw);
        //        DataGrid dgGrid = new DataGrid();
        //        dgGrid.DataSource = dtAllPayModeAllExhouseCommissionData;
        //        dgGrid.DataBind();

        //        foreach (DataGridItem item in dgGrid.Items)
        //        {
        //            for (int j = 0; j < item.Cells.Count; j++)
        //            {
        //                if (j == 0 || j == 1 || j == 2)
        //                {
        //                    item.Cells[j].Attributes.Add("style", "mso-number-format:\\@");
        //                }
        //                else if (j >= 3 && j <= 6)
        //                {
        //                    item.Cells[j].Attributes.Add("style", "mso-number-format:0");
        //                }
        //                else
        //                {
        //                    item.Cells[j].Attributes.Add("style", "mso-number-format:0\\.00");
        //                }
        //            }
        //        }

        //        dgGrid.RenderControl(hw);
        //        Response.ContentType = "application/vnd.ms-excel";
        //        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + "");
        //        this.EnableViewState = false;
        //        Response.Write(tw.ToString());
        //        Response.End();
        //    }
            

        //    /*
        //    if (dtAllPayModeAllExhouseCommissionData.Rows.Count > 0)
        //    {
        //        using (XLWorkbook wbComm = new XLWorkbook())
        //        {
        //            wbComm.Worksheets.Add(dtAllPayModeAllExhouseCommissionData);  //Add DataTable as Worksheet.

        //            string headerValue = "attachment;filename=" + "MonthlyCommissionSummaryData__" + dtpickerMonthlyCommFrom.Text + "_to_" + dtpickerMonthlyCommTo.Text + ".xlsx";

        //            //Export the Excel file.
        //            Response.Clear();
        //            Response.Buffer = true;
        //            Response.Charset = "";
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", headerValue);
        //            using (MemoryStream MyMemoryStream = new MemoryStream())
        //            {
        //                wbComm.SaveAs(MyMemoryStream);
        //                MyMemoryStream.WriteTo(Response.OutputStream);
        //                Response.Flush();
        //                Response.End();
        //            }
        //        }
        //    }
        //    */
        //}

        protected void btnDownloadAutoDebitForEFTMTBCashAsExcel_Click(object sender, EventArgs e)
        {
            if (dtAllPayModeAllExhouseCommissionData.Rows.Count > 0)
            {
                DataTable dtAutoDebitTxn = new DataTable();
                dtAutoDebitTxn.Columns.Add("ACC/GL");//0
                dtAutoDebitTxn.Columns.Add("Dr/Cr");//1
                dtAutoDebitTxn.Columns.Add("ACC/GL NO");//2
                dtAutoDebitTxn.Columns.Add("Amount");//3
                dtAutoDebitTxn.Columns.Add("Remarks");//4
                dtAutoDebitTxn.Columns.Add("ExchangeName");//5


                DataTable dtAutoDebitBkashTxn = new DataTable();
                dtAutoDebitBkashTxn.Columns.Add("ACC/GL");//0
                dtAutoDebitBkashTxn.Columns.Add("Dr/Cr");//1
                dtAutoDebitBkashTxn.Columns.Add("ACC/GL NO");//2
                dtAutoDebitBkashTxn.Columns.Add("Amount");//3
                dtAutoDebitBkashTxn.Columns.Add("Remarks");//4
                dtAutoDebitBkashTxn.Columns.Add("ExchangeName");//5
                

                DataRow drowTotal;
                string exHouseName = "", CrRemarks = "";
                decimal totalCommAmount, vatAmount, coopFundAmount, remainAmount, commissionAmount;

                DateTime dateTime1 = DateTime.ParseExact(dtpickerMonthlyCommFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dtpickerMonthlyCommTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string dtRange = dateTime1.ToString("dd.MM.yy") + "-" + dateTime2.ToString("dd.MM.yy");
                string monthYearName = dateTime1.ToString("MMM").ToUpper() + " " + dateTime1.Year;

                for (int ii = 0; ii < dtAllPayModeAllExhouseCommissionData.Rows.Count; ii++)
                {
                    if (dtAllPayModeAllExhouseCommissionData.Rows[ii]["NRTAccount"].ToString().Contains("1000210000371")) // for TRANSFAST
                    {
                        drowTotal = dtAutoDebitTxn.NewRow();
                        drowTotal[0] = "SB/CA";
                        drowTotal[1] = "DR";
                        drowTotal[2] = dtAllPayModeAllExhouseCommissionData.Rows[ii]["NRTAccount"].ToString();
                        exHouseName = dtAllPayModeAllExhouseCommissionData.Rows[ii]["ExHouseName"].ToString();

                        totalCommAmount = Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["EFTCalculatedCommission"]) + Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["CASHCalculatedCommission"]) + Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["MTBCalculatedCommission"]);

                        drowTotal[3] = Math.Round(totalCommAmount, 2);

                        int totalTxnCount = Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["BEFTNCount"]) + Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["MTBCount"]) + Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["CashCount"]);

                        drowTotal[4] = TxnRemarks + totalTxnCount + " " + dtRange;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitTxn.Rows.Add(drowTotal);
                        

                        coopFundAmount = Math.Round(((totalCommAmount * CConstants.COOP_FUND_PERCENTAGE) / 100), 2);
                        remainAmount = Math.Round((totalCommAmount - coopFundAmount), 2);
                        vatAmount = Math.Round(((remainAmount * 15) / 115), 2);
                        commissionAmount = Math.Round((totalCommAmount - coopFundAmount - vatAmount), 2);

                        drowTotal = dtAutoDebitTxn.NewRow();
                        drowTotal[0] = "GL";
                        drowTotal[1] = "CR";
                        drowTotal[2] = CConstants.VAT_GL;
                        drowTotal[3] = vatAmount;
                        drowTotal[4] = VatRemarks + exHouseName + " " + dtRange;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitTxn.Rows.Add(drowTotal);

                        drowTotal = dtAutoDebitTxn.NewRow();
                        drowTotal[0] = "GL";
                        drowTotal[1] = "CR";
                        drowTotal[2] = CConstants.COMMISSION_GL;
                        drowTotal[3] = commissionAmount;
                        drowTotal[4] = CommRemarks + exHouseName + " " + dtRange;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitTxn.Rows.Add(drowTotal);

                        drowTotal = dtAutoDebitTxn.NewRow();
                        drowTotal[0] = "GL";
                        drowTotal[1] = "CR";
                        drowTotal[2] = CConstants.COOP_FUND_GL;
                        drowTotal[3] = coopFundAmount;
                        drowTotal[4] = "COOP " + exHouseName + " " + dtRange;
                        drowTotal[5] = exHouseName;
                                                                        
                        dtAutoDebitTxn.Rows.Add(drowTotal);
                    }
                    else
                    {
                        drowTotal = dtAutoDebitTxn.NewRow();
                        drowTotal[0] = "SB/CA";
                        drowTotal[1] = "DR";
                        drowTotal[2] = dtAllPayModeAllExhouseCommissionData.Rows[ii]["NRTAccount"].ToString();
                        exHouseName = dtAllPayModeAllExhouseCommissionData.Rows[ii]["ExHouseName"].ToString();

                        totalCommAmount = Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["EFTCalculatedCommission"]) + Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["CASHCalculatedCommission"]) + Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["MTBCalculatedCommission"]);

                        drowTotal[3] = Math.Round(totalCommAmount, 2);

                        int totalTxnCount = Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["BEFTNCount"]) + Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["MTBCount"]) + Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["CashCount"]);

                        drowTotal[4] = TxnRemarks + totalTxnCount + " " + dtRange;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitTxn.Rows.Add(drowTotal);


                        drowTotal = dtAutoDebitTxn.NewRow();
                        drowTotal[0] = "GL";
                        drowTotal[1] = "CR";
                        drowTotal[2] = CConstants.VAT_GL;
                        vatAmount = Math.Round(((totalCommAmount * 15) / 115), 2);
                        drowTotal[3] = vatAmount;
                        drowTotal[4] = VatRemarks + exHouseName + " " + dtRange;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitTxn.Rows.Add(drowTotal);


                        drowTotal = dtAutoDebitTxn.NewRow();
                        drowTotal[0] = "GL";
                        drowTotal[1] = "CR";
                        drowTotal[2] = CConstants.COMMISSION_GL;
                        drowTotal[3] = Math.Round((totalCommAmount - vatAmount), 2);
                        drowTotal[4] = CommRemarks + exHouseName + " " + dtRange;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitTxn.Rows.Add(drowTotal);
                    }
                }

                // for bkash -----------------------------

                for (int ii = 0; ii < dtAllPayModeAllExhouseCommissionData.Rows.Count; ii++)
                {
                    if (Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["bKashCount"]) > 0)  // if bkash count > 0
                    {
                        drowTotal = dtAutoDebitBkashTxn.NewRow();
                        drowTotal[0] = "SB/CA";
                        drowTotal[1] = "DR";
                        drowTotal[2] = dtAllPayModeAllExhouseCommissionData.Rows[ii]["NRTAccount"].ToString();
                        exHouseName = dtAllPayModeAllExhouseCommissionData.Rows[ii]["ExHouseName"].ToString();

                        totalCommAmount = Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["BKASHCalculatedCommission"]);

                        drowTotal[3] = Math.Round(totalCommAmount, 2);

                        int totalTxnCount = Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["bKashCount"]);

                        drowTotal[4] = "BKASH TXN CHRG - Txn: " + totalTxnCount + " - M/O " + monthYearName;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitBkashTxn.Rows.Add(drowTotal);


                        drowTotal = dtAutoDebitBkashTxn.NewRow();
                        drowTotal[0] = "GL";
                        drowTotal[1] = "CR";
                        drowTotal[2] = CConstants.BKASH_COMMISSION_PAYABLE;
                        drowTotal[3] = Math.Round(totalCommAmount, 2);

                        CrRemarks = "BKASH TXN CHRG M/O " + monthYearName + " " + exHouseName;
                        if (CrRemarks.Length > 48)
                        {
                            CrRemarks = CrRemarks.Substring(0, 47);
                        }

                        drowTotal[4] = CrRemarks;
                        drowTotal[5] = exHouseName;
                        dtAutoDebitBkashTxn.Rows.Add(drowTotal);
                    }
                }//for END

                //------------------

               
                string headerValue = "attachment;filename=Commission_Calculation_AutoDebit_" + dtRange + ".xlsx";
                if (dtAutoDebitTxn.Rows.Count > 0)
                {
                    dtAutoDebitTxn.TableName = "Commission_AutoDebit";
                    dtAutoDebitBkashTxn.TableName = "bKash_Comm_AutoDebit";

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dtAutoDebitTxn);
                        wb.Worksheets.Add(dtAutoDebitBkashTxn);

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", headerValue);
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
                //-------------------
            }
        }


        protected void btnDownloadExhWiseCommissionDataAsExcel_Click(object sender, EventArgs e)
        {

            if (cbExchWiseSumr.SelectedIndex != 0)
            {
                DateTime dateTime1 = DateTime.ParseExact(dtpickerMonthlyCommFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dtpickerMonthlyCommTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
                string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

                DateTime previousMonthLastDay = dateTime1.AddDays(-1);
                string previousMonthLastDayDate = previousMonthLastDay.ToString("yyyy-MM-dd");
                string forwardDate = dateTime2.AddDays(1).ToString("yyyy-MM-dd");


                int exhId = Convert.ToInt32(cbExchWiseSumr.Text.Split('-')[0]);
                string exhName = cbExchWiseSumr.Text.Split('-')[1].Trim();

                //DataSet ds = mg.GetAllPaymodeCommissionDataByExchangeHouse(exhId, exhName, dtValue1, dtValue2, previousMonthLastDayDate, forwardDate);

                DataSet dsEFTMTB = mg.GetEFTMTBAcModeCommissionDataByExchangeHouseNEW(exhId, exhName, dtValue1, dtValue2);
                DataSet dsCSHBKS = mg.GetCashBkashModeCommissionDataByExchangeHouseNEW(exhId, exhName, dtValue1, dtValue2);

                string headerValue = "attachment;filename=" + exhName + "_" + dtValue1 + "_to_" + dtValue2 + ".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt1 in dsEFTMTB.Tables)
                    {
                        wb.Worksheets.Add(dt1);  //Add DataTable as Worksheet.
                    }

                    foreach (DataTable dt2 in dsCSHBKS.Tables)
                    {
                        wb.Worksheets.Add(dt2);  //Add DataTable as Worksheet.
                    }

                    //Export the Excel file.
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", headerValue);
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }

        }

        protected void btnDownloadMonthlyCommissionDataAsExcelV2_Click(object sender, EventArgs e)
        {            
            string headerValue = "attachment;filename=MonthlyCommissionSummaryData__" + dtpickerMonthlyCommFrom.Text + "_to_" + dtpickerMonthlyCommTo.Text + ".xlsx";

            if (dtAllPayModeAllExhouseCommissionData.Rows.Count > 0)
            {
                DataSet ds = new DataSet();
                dtAllPayModeAllExhouseCommissionData.TableName = "MonthlyCommissionSummaryData";
                ds.Tables.Add(dtAllPayModeAllExhouseCommissionData);
                
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtAllPayModeAllExhouseCommissionData);

                    //Export the Excel file.
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", headerValue);
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }

        //protected void btnDownloadAutoDebitForBkashAsExcel_Click(object sender, EventArgs e)
        //{
        //    if (dtAllPayModeAllExhouseCommissionData.Rows.Count > 0)
        //    {
        //        DataTable dtAutoDebitBkashTxn = new DataTable();

        //        dtAutoDebitBkashTxn.Columns.Add("ACC/GL");//0
        //        dtAutoDebitBkashTxn.Columns.Add("Dr/Cr");//1
        //        dtAutoDebitBkashTxn.Columns.Add("ACC/GL NO");//2
        //        dtAutoDebitBkashTxn.Columns.Add("Amount");//3
        //        dtAutoDebitBkashTxn.Columns.Add("Remarks");//4
        //        dtAutoDebitBkashTxn.Columns.Add("ExchangeName");//5


        //        DataRow drowTotal;
        //        string exHouseName = "", CrRemarks = "";
        //        decimal totalCommAmount;

        //        DateTime dateTime1 = DateTime.ParseExact(dtpickerMonthlyCommFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //        DateTime dateTime2 = DateTime.ParseExact(dtpickerMonthlyCommTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        //        string dtRange = dateTime1.ToString("dd.MM.yy") + "-" + dateTime2.ToString("dd.MM.yy");

        //        string monthYearName = dateTime1.ToString("MMM").ToUpper() + " " + dateTime1.Year;

        //        for (int ii = 0; ii < dtAllPayModeAllExhouseCommissionData.Rows.Count; ii++)
        //        {
        //            if (Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["bKashCount"]) > 0)  // if bkash count > 0
        //            {
        //                drowTotal = dtAutoDebitBkashTxn.NewRow();
        //                drowTotal[0] = "SB/CA";
        //                drowTotal[1] = "DR";
        //                drowTotal[2] = dtAllPayModeAllExhouseCommissionData.Rows[ii]["NRTAccount"].ToString();
        //                exHouseName = dtAllPayModeAllExhouseCommissionData.Rows[ii]["ExHouseName"].ToString();

        //                totalCommAmount = Convert.ToDecimal(dtAllPayModeAllExhouseCommissionData.Rows[ii]["BKASHCalculatedCommission"]);

        //                drowTotal[3] = Math.Round(totalCommAmount, 2);

        //                int totalTxnCount = Convert.ToInt32(dtAllPayModeAllExhouseCommissionData.Rows[ii]["bKashCount"]);

        //                drowTotal[4] = "BKASH TXN CHRG - Count: " + totalTxnCount + " - M/O " + monthYearName;
        //                drowTotal[5] = exHouseName;
        //                dtAutoDebitBkashTxn.Rows.Add(drowTotal);

                        
        //                drowTotal = dtAutoDebitBkashTxn.NewRow();
        //                drowTotal[0] = "GL";
        //                drowTotal[1] = "CR";
        //                drowTotal[2] = CConstants.BKASH_COMMISSION_PAYABLE;
        //                drowTotal[3] = Math.Round(totalCommAmount, 2);

        //                CrRemarks = "BKASH TXN CHRG M/O " + monthYearName + " " + exHouseName;
        //                if (CrRemarks.Length > 48)
        //                {
        //                    CrRemarks = CrRemarks.Substring(0, 47);
        //                }

        //                drowTotal[4] = CrRemarks;
        //                drowTotal[5] = exHouseName;
        //                dtAutoDebitBkashTxn.Rows.Add(drowTotal);
        //            }
        //        }//for END

        //        string headerValue = "attachment;filename=bKash_Commission_Calculation_AutoDebit_" + dtRange + ".xlsx";
        //        if (dtAutoDebitBkashTxn.Rows.Count > 0)
        //        {
        //            dtAutoDebitBkashTxn.TableName = "bKash_Comm_AutoDebit";
        //            using (XLWorkbook wb = new XLWorkbook())
        //            {
        //                wb.Worksheets.Add(dtAutoDebitBkashTxn);

        //                Response.Clear();
        //                Response.Buffer = true;
        //                Response.Charset = "";
        //                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                Response.AddHeader("content-disposition", headerValue);
        //                using (MemoryStream MyMemoryStream = new MemoryStream())
        //                {
        //                    wb.SaveAs(MyMemoryStream);
        //                    MyMemoryStream.WriteTo(Response.OutputStream);
        //                    Response.Flush();
        //                    Response.End();
        //                }
        //            }
        //        }


        //    }
        //}

       
    }
}