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
    public partial class FreelanceKeyword : System.Web.UI.Page
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

            btnFreelanceKeywordSearch_Click(sender, e);
            lblFreelanceKeywordNameSearchResult.Text = "";
            lblFreelanceKeywordNameSaveResult.Text = "";
            lblRemoveStatus.Text = "";
        }

        protected void btnFreelanceKeywordSearch_Click(object sender, EventArgs e)
        {
            DataTable dtFreelanceKeywordName = mg.GetBEFTNFreelanceKeywordName();

            dataGridViewFreelanceKeywordName.DataSource = null;
            dataGridViewFreelanceKeywordName.DataSource = dtFreelanceKeywordName;
            dataGridViewFreelanceKeywordName.DataBind();
        }

        protected void btnSearchFreelanceKeywordNameIsExist_Click(object sender, EventArgs e)
        {
            if (!txtFreelanceKeywordNameSearchInput.Text.Trim().Equals(""))
            {
                DataTable dtRes = mg.SearchFreelanceKeywordNameByInput(txtFreelanceKeywordNameSearchInput.Text.Trim());
                if (dtRes.Rows.Count > 0)
                {
                    string outputmsg = "";
                    for (int ii = 0; ii < dtRes.Rows.Count; ii++)
                    {
                        outputmsg += "Id: " + dtRes.Rows[ii]["ID"] + ", Val: " + dtRes.Rows[ii]["KEYWORDS"];
                        outputmsg += "\n";
                    }
                    lblFreelanceKeywordNameSearchResult.Text = outputmsg;
                }
                else
                {
                    lblFreelanceKeywordNameSearchResult.Text = "NOTHING FOUND";
                }
            }
        }

        protected void dataGridViewFreelanceKeywordName_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[1].Attributes["width"] = "350px"; //SENDERNAME
            e.Row.Cells[2].Attributes["width"] = "150px"; //CREATEDATE
        }

        protected void btnFreelanceKeywordNameSave_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxFreelanceKeywordName.Text, "\n");
            string keywordName = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                keywordName = lines[i].ToString().Trim();
                if (!keywordName.Equals(""))
                {
                    mg.SaveNewFreelanceKeywordName(keywordName.ToUpper());
                }
            }

            lblFreelanceKeywordNameSaveResult.Text = "Database Updated...";
            btnFreelanceKeywordSearch_Click(sender, e);

            textBoxFreelanceKeywordName.Text = "";
        }

        protected void btnRemoveFreelanceKeyword_Click(object sender, EventArgs e)
        {
            string txtId = txtkeywordId.Text.Trim();
            if (!txtId.Equals(""))
            {
                try
                {
                    int keywordId = Convert.ToInt32(txtId);
                    bool stats = mg.RemoveFreelanceKeywordId(keywordId);
                    lblRemoveStatus.Text = "Database Updated...";
                    btnFreelanceKeywordSearch_Click(sender, e);
                    txtkeywordId.Text = "";
                }
                catch (Exception ex)
                {
                    lblRemoveStatus.Text = "ERROR in Operation...";
                }
            }
        }
    }
}