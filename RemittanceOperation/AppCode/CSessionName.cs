using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemittanceOperation.AppCode
{
    public class CSessionName
    {
        public CSessionName()
        {
        }
        
        public const string S_CURRENT_USER_RM = "oUserRm";
        public const string S_CURRENT_USERID = "oUserId";
        public const string S_CURRENT_USER_FULL_NAME = "oUserName";
        public const string S_CURRENT_USER_EMAIL = "oUserEmail";
        public const string IS_SESSION_EXPIRED = "SessionExpired";
        public const string S_CURRENT_SESSION = "Starting current session";
        public const string S_LOGIN_TIME = "Login Time";
        public const string S_SESSION_ID = "SessionID";
        public const string S_MENU_SESSION = "MenuSessionID";

        public const string S_FILE_PROCESS_USER_TYPE = "FileProcessUserType";
        public const string S_IS_MAIL_RECEIVE = "IsMailReceive";
        public const string S_ROLE_NAME = "roleName";

        public const string F_LOGIN_PAGE = "FORMS/frmLogin.aspx";
        public const string F_HOME_PAGE = "FORMS/frmHome.aspx";
    }
}