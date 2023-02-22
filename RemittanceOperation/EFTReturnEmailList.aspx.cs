using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class EFTReturnEmailList : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        string roleName = "", UserType = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                UserType = Session[CSessionName.S_FILE_PROCESS_USER_TYPE].ToString();

                if (!UserType.Equals("SuperAdmin"))
                {
                    if (!UserType.Equals("Authorizer"))
                    {
                        lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                        lblUserAuthorizationMsg.ForeColor = Color.Red;
                    }
                }
                else
                {
                    lblUserAuthorizationMsg.Text = "";
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if(!IsPostBack)
            {
                LoadReturnEmailAddrs();
            }
        }

        private void LoadReturnEmailAddrs()
        {
            dGridViewReturnEmailAddrs.Columns.Clear();
            DataTable dtReturnEmailAddrs = mg.GetReturnEmailDetails();

            dGridViewReturnEmailAddrs.DataSource = null;
            dGridViewReturnEmailAddrs.DataSource = dtReturnEmailAddrs;
            dGridViewReturnEmailAddrs.DataBind();
        }

        //protected void dGridViewReturnEmailAddrs_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    lblUpdateSuccMsg.Text = "";
        //    lblUpdateErrorMsg.Text = "";
            
        //    // SL,[PartyId],[UserId],[ExchangeHouseName],[ToAddress],[CcAddress],[isActive]
        //    txtAutoId.Text = dGridViewReturnEmailAddrs.SelectedRow.Cells[1].Text;
        //    txtPartyId.Text = dGridViewReturnEmailAddrs.SelectedRow.Cells[2].Text;
        //    txtUserId.Text = dGridViewReturnEmailAddrs.SelectedRow.Cells[3].Text;
        //    txtExhName.Text = dGridViewReturnEmailAddrs.SelectedRow.Cells[4].Text;
        //    txtToAddress.Text = dGridViewReturnEmailAddrs.SelectedRow.Cells[5].Text;
        //    txtCCAddress.Text = dGridViewReturnEmailAddrs.SelectedRow.Cells[6].Text;

        //    if(txtCCAddress.Text.Contains("&nbsp;"))
        //    {
        //        txtCCAddress.Text = "";
        //    }
        //}

        //protected void btnUpdateMTOReturnEmail_Click(object sender, EventArgs e)
        //{
        //    if (UserType.Equals("SuperAdmin") || UserType.Equals("Authorizer"))
        //    {
        //        lblUpdateErrorMsg.Text = "";

        //        int sl = Convert.ToInt32(txtAutoId.Text);
        //        int partyId = Convert.ToInt32(txtPartyId.Text);
        //        string toAddr = txtToAddress.Text;
        //        string ccAddr = txtCCAddress.Text;

        //        bool stat = mg.UpdateReturnEmailAddress(sl, partyId, toAddr, ccAddr);
        //        if(stat)
        //        {
        //            lblUpdateSuccMsg.Text = "Successfully Updated";
        //        }
        //        else
        //        {
        //            lblUpdateErrorMsg.Text = "Update Error !!! ";
        //        }
        //    }
        //    else
        //    {
        //        lblUpdateErrorMsg.Text = "You are NOT Authorized to UPDATE !!!";
        //    }
        //}

        protected void btnSearchReturnEmails_Click(object sender, EventArgs e)
        {
            LoadReturnEmailAddrs();
        }

    }
}