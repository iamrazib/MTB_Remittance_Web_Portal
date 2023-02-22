using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class NBLTxnModification : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        string exh = "NBL";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string txnStat = "", whereClause = "";
            string refNo = textBoxRefNo.Text.Trim();

            if (!String.IsNullOrEmpty(refNo))
            {
                if (whereClause.Equals("") && !refNo.Equals(""))
                {
                    whereClause = " WHERE [REFERENCE]='" + refNo + "'";
                }

                DataTable aDTable = mg.GetIndividualTxnByWhereClause(exh, whereClause);
                dataGridViewTxnSearch.DataSource = null;
                dataGridViewTxnSearch.DataSource = aDTable;
                dataGridViewTxnSearch.DataBind();

            }
        }
    }
}