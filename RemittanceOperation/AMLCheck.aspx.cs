using RemittanceOperation.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

namespace RemittanceOperation
{
    public partial class AMLCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
            }
            else
            {
                Response.Redirect("Login.aspx");//CSessionName.F_LOGIN_PAGE);
            }
        }

        protected void btnAMLScore_Click(object sender, EventArgs e)
        {
            string AMLscore = GetAMLMatchScore(txtName.Text.Trim());
            lblAMLScoreVal.Text = AMLscore;
        }

        private string GetAMLMatchScore(string name)
        {
            string score = "";
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string BaseURL = "http://192.168.67.17/SRN/api/SanctionLists/GetScore/?name=";
                    string finalURL = BaseURL + name;

                    string data = wc.DownloadString(finalURL);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc = JsonToXML(data);
                    XmlNodeList parentNode = xmlDoc.GetElementsByTagName("MatchScore");

                    foreach (XmlNode childrenNode in parentNode)
                    {
                        score = childrenNode.InnerText.ToString();
                        break;
                    }

                }

                return score;
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }

        private XmlDocument JsonToXML(string json)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
                {
                    XElement xml = XElement.Load(reader);
                    doc.LoadXml(xml.ToString());
                }

                return doc;
            }
            catch (Exception ex)
            {
                return doc;
            }
        }
    }
}