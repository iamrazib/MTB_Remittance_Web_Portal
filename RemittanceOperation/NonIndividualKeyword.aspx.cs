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
    public partial class NonIndividualKeyword : System.Web.UI.Page
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

            lblCompanyNameSearchResult.Text = "";
            lblCompanyNameSaveResult.Text = "";
            lblRemoveStatus.Text = "";
            btnCompanyNameSearch_Click(sender, e);


            lblCompanyAccountNoSearchResult.Text = "";
            lblCompanyAccountSaveResult.Text = "";
            lblRemoveCompanyAccountStatus.Text = "";
            btnCompanyAccountSearch_Click(sender, e);
        }

        protected void btnCompanyNameSearch_Click(object sender, EventArgs e)
        {
            DataTable dtCompanyName = mg.GetBEFTNCompanyName();
            dataGridViewCompanyName.DataSource = null;
            dataGridViewCompanyName.DataSource = dtCompanyName;
            dataGridViewCompanyName.DataBind();
        }

        protected void btnSearchCompanyNameIsExist_Click(object sender, EventArgs e)
        {
            if (!txtCompanyNameSearchInput.Text.Trim().Equals(""))
            {
                DataTable dtRes = mg.SearchCompanyNameByInput(txtCompanyNameSearchInput.Text.Trim());
                if (dtRes.Rows.Count > 0)
                {
                    string outputmsg = "";
                    for (int ii = 0; ii < dtRes.Rows.Count; ii++)
                    {
                        outputmsg += "Id: " + dtRes.Rows[ii]["ID"] + ", Val: " + dtRes.Rows[ii]["COMPANYNAME"];
                        outputmsg += "\n";
                    }
                    lblCompanyNameSearchResult.Text = outputmsg;
                }
                else
                {
                    lblCompanyNameSearchResult.Text = "NOTHING FOUND";
                }
            }
        }

        protected void btnCompanyNameSave_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxCompanyName.Text, "\n");
            string compName = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                compName = lines[i].ToString().Trim();
                if (!compName.Equals(""))
                {
                    mg.SaveNewCompanyName(compName.ToUpper());
                }
            }

            lblCompanyNameSaveResult.Text = "Database Updated...";
            textBoxCompanyName.Text = "";
            btnCompanyNameSearch_Click(sender, e);            
        }

        protected void btnRemoveCompanyName_Click(object sender, EventArgs e)
        {
            string txtId = txtCompId.Text.Trim();
            if (!txtId.Equals(""))
            {
                try
                {
                    int compId = Convert.ToInt32(txtId);
                    bool stats = mg.RemoveCompanyNameById(compId);
                    lblRemoveStatus.Text = "Database Updated...";
                    btnCompanyNameSearch_Click(sender, e);
                    txtCompId.Text = "";
                }
                catch (Exception ex)
                {
                    lblRemoveStatus.Text = "ERROR in Operation...";
                }
            }
        }



        protected void btnCompanyAccountSearch_Click(object sender, EventArgs e)
        {
            DataTable dtCompanyAc = mg.GetBEFTNCompanyAccountNo();
            dataGridViewCompanyAccount.DataSource = null;
            dataGridViewCompanyAccount.DataSource = dtCompanyAc;
            dataGridViewCompanyAccount.DataBind();
        }

        protected void btnCompanyAccountSave_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxCompanyAccount.Text, "\n");
            string compAcno = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                compAcno = lines[i].ToString().Trim();
                if (!compAcno.Equals(""))
                {
                    mg.SaveNewCompanyAccountNo(compAcno.ToUpper());
                }
            }

            lblCompanyAccountSaveResult.Text = "Database Updated...";
            textBoxCompanyAccount.Text = "";
            btnCompanyAccountSearch_Click(sender, e); 
        }

        protected void btnSearchCompanyAccountIsExist_Click(object sender, EventArgs e)
        {
            if (!txtCompanyAccountSearchInput.Text.Trim().Equals(""))
            {
                DataTable dtRes = mg.SearchCompanyAccountByInput(txtCompanyAccountSearchInput.Text.Trim());
                if (dtRes.Rows.Count > 0)
                {
                    string outputmsg = "";
                    for (int ii = 0; ii < dtRes.Rows.Count; ii++)
                    {
                        outputmsg += "Id: " + dtRes.Rows[ii]["ID"] + ", Val: " + dtRes.Rows[ii]["CompanyAccountNo"];
                        outputmsg += "\n";
                    }
                    lblCompanyAccountNoSearchResult.Text = outputmsg;
                }
                else
                {
                    lblCompanyAccountNoSearchResult.Text = "NOTHING FOUND";
                }
            }
        }

        protected void btnRemoveCompanyAccountNo_Click(object sender, EventArgs e)
        {
            string txtId = txtCompAccId.Text.Trim();
            if (!txtId.Equals(""))
            {
                try
                {
                    int compId = Convert.ToInt32(txtId);
                    bool stats = mg.RemoveCompanyAccountById(compId);
                    lblRemoveCompanyAccountStatus.Text = "Database Updated...";
                    btnCompanyAccountSearch_Click(sender, e);
                    txtCompAccId.Text = "";
                }
                catch (Exception ex)
                {
                    lblRemoveCompanyAccountStatus.Text = "ERROR in Operation...";
                }
            }
        }


    }
}