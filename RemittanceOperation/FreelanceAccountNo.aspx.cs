using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class FreelanceAccountNo : System.Web.UI.Page
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

            btnFreelanceAccountNoSearch_Click(sender, e);
            lblFreelanceAccountNoSearchResult.Text = "";
            lblFreelanceAccountNoSaveResult.Text = "";
            lblRemoveStatus.Text = "";
        }

        protected void dataGridViewFreelanceAccountNo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[1].Attributes["width"] = "170px"; //BENEFICIARY_ACCOUNT
            e.Row.Cells[2].Attributes["width"] = "130px"; //CREATEDATE
        }

        protected void btnSearchFreelanceAccountNoIsExist_Click(object sender, EventArgs e)
        {
            if (!txtFreelanceAccountNoSearchInput.Text.Trim().Equals(""))
            {
                DataTable dtRes = mg.SearchFreelanceAccountNoByInput(txtFreelanceAccountNoSearchInput.Text.Trim());
                if (dtRes.Rows.Count > 0)
                {
                    string outputmsg = "";
                    for (int ii = 0; ii < dtRes.Rows.Count; ii++)
                    {
                        outputmsg += "Id: " + dtRes.Rows[ii]["ID"] + ", Val: " + dtRes.Rows[ii]["BENEFICIARY_ACCOUNT"];
                        outputmsg += "\n";
                    }
                    lblFreelanceAccountNoSearchResult.Text = outputmsg;
                }
                else
                {
                    lblFreelanceAccountNoSearchResult.Text = "NOTHING FOUND";
                }
            }
        }

        protected void btnRemoveFreelanceAccountNo_Click(object sender, EventArgs e)
        {
            string txtId = txtAccountNoId.Text.Trim();
            if (!txtId.Equals(""))
            {
                try
                {
                    int keywordId = Convert.ToInt32(txtId);
                    bool stats = mg.RemoveFreelanceAccountNo(keywordId);
                    lblRemoveStatus.Text = "Database Updated...";
                    btnFreelanceAccountNoSearch_Click(sender, e);
                    txtAccountNoId.Text = "";
                }
                catch (Exception ex)
                {
                    lblRemoveStatus.Text = "ERROR in Operation...";
                }
            }
        }

        protected void btnFreelanceAccountNoSave_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxFreelanceAccountNo.Text, "\n");
            string keywordName = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                keywordName = lines[i].ToString().Trim();
                if (!keywordName.Equals(""))
                {
                    mg.SaveNewFreelanceAccountNo(keywordName.ToUpper());
                }
            }

            lblFreelanceAccountNoSaveResult.Text = "Database Updated...";
            btnFreelanceAccountNoSearch_Click(sender, e);

            textBoxFreelanceAccountNo.Text = "";
        }

        protected void btnFreelanceAccountNoSearch_Click(object sender, EventArgs e)
        {
            DataTable dtFreelanceAccountNo = mg.GetBEFTNFreelanceAccountNo();

            dataGridViewFreelanceAccountNo.DataSource = null;
            dataGridViewFreelanceAccountNo.DataSource = dtFreelanceAccountNo;
            dataGridViewFreelanceAccountNo.DataBind();
        }
    }
}