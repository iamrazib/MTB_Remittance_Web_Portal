using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class GlobalDataCheck : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if(!IsPostBack)
            {
                LoadGlobalExchList();

                dTPickerFromGlobalChk.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dTPickerToGlobalChk.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void LoadGlobalExchList()
        {
            DataTable dtExchs = mg.LoadGlobalExchList();
            comboBoxGlobalExh.Items.Clear();
            comboBoxGlobalExh.Items.Add("--Select--");

            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                comboBoxGlobalExh.Items.Add(dtExchs.Rows[rows][0].ToString());
            }

            comboBoxGlobalExh.SelectedIndex = 0;
        }

        protected void btnGlobalBkashSearch_Click(object sender, EventArgs e)
        {
            int idx = comboBoxGlobalExh.SelectedIndex;
            if (idx != 0)
            {
                int totalRec = 0, succs = 0, unsuccs = 0;

                DateTime dateTime1 = DateTime.ParseExact(dTPickerFromGlobalChk.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dTPickerToGlobalChk.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string frmdt = dateTime1.ToString("yyyy-MM-dd");
                string todt = dateTime2.ToString("yyyy-MM-dd");
                
                int exhId = Convert.ToInt32(comboBoxGlobalExh.Text.Split('-')[0]);


                DataTable aDTbKashReg = mg.GetBkashRegTxn(exhId, frmdt, todt);
                dataGridViewGlobalBkashTxn.DataSource = null;
                dataGridViewGlobalBkashTxn.DataSource = aDTbKashReg;
                dataGridViewGlobalBkashTxn.DataBind();
                totalRec = aDTbKashReg.Rows.Count;


                DataTable aDataTableSuccSumr = mg.GetBkashRegTxnSuccessSumr(exhId, frmdt, todt);
                succs = aDataTableSuccSumr.Rows.Count;

                DataTable aDataTableUnSuccSumr = mg.GetBkashRegTxnUnSuccessSumr(exhId, frmdt, todt);
                unsuccs = aDataTableUnSuccSumr.Rows.Count;

                lblGlobalDataBkashCount.Text = "Total: " + totalRec;
                lblGlobalDataBkashUnsuccCount.Text = "Success: " + succs + " , Unsuccess/Pending: " + unsuccs;
                //-----------------------------------------------------------------------

                //--------------------------- BEFTN ------------------------------------

                DataTable aDTableBEFTN = mg.GetBEFTNDetailTxn(exhId, frmdt, todt);
                dataGridViewGlobalBEFTNTxn.DataSource = null;
                dataGridViewGlobalBEFTNTxn.DataSource = aDTableBEFTN;
                dataGridViewGlobalBEFTNTxn.DataBind();

                lblGlobalDataBEFTNCount.Text = "Total: " + aDTableBEFTN.Rows.Count;

                DataTable aDataTableEFTSumr = mg.GetBEFTNSumrTxn(exhId, frmdt, todt);
                lblGlobalDataBeftnSuccUnsucsCount.Text = "";

                for (int sumrCnt = 0; sumrCnt < aDataTableEFTSumr.Rows.Count; sumrCnt++)
                {
                    if (aDataTableEFTSumr.Rows[sumrCnt][0].ToString().Equals("1"))
                    {
                        lblGlobalDataBeftnSuccUnsucsCount.Text = " Recvd:" + aDataTableEFTSumr.Rows[sumrCnt][1].ToString();
                    }
                    if (aDataTableEFTSumr.Rows[sumrCnt][0].ToString().Equals("5"))
                    {
                        lblGlobalDataBeftnSuccUnsucsCount.Text += "  Success:" + aDataTableEFTSumr.Rows[sumrCnt][1].ToString();
                    }
                    if (aDataTableEFTSumr.Rows[sumrCnt][0].ToString().Equals("7"))
                    {
                        lblGlobalDataBeftnSuccUnsucsCount.Text += "  ReadyToDisburse:" + aDataTableEFTSumr.Rows[sumrCnt][1].ToString();
                    }
                }

                //-----------------------------------------------------------------------

                //--------------------------- MTB ------------------------------------

                DataTable aDataTableMTB = mg.GetMTBAcDetailTxn(exhId, frmdt, todt);
                DataTable aDataTableMTBSucsCnt = mg.GetMTBAcSuccTxn(exhId, frmdt, todt);

                dataGridViewGlobalMTBTxn.DataSource = null;
                dataGridViewGlobalMTBTxn.DataSource = aDataTableMTB;
                dataGridViewGlobalMTBTxn.DataBind();

                lblGlobalDataMTBCount.Text = "Total: " + aDataTableMTB.Rows.Count + ", Success: " + aDataTableMTBSucsCnt.Rows[0][0]; 
                
                //-----------------------------------------------------------------------

                //--------------------------- CASH ------------------------------------

                DataTable aDataTableCASH = mg.GetCashDetailTxn(exhId, frmdt, todt);
                dataGridViewGlobalCASHTxn.DataSource = null;
                dataGridViewGlobalCASHTxn.DataSource = aDataTableCASH;
                dataGridViewGlobalCASHTxn.DataBind();

                lblGlobalDataCASHCount.Text = "Total: " + aDataTableCASH.Rows.Count;
                //-----------------------------------------------------------------------

            }

        }
    }
}