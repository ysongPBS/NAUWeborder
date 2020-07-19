using pbsweborderAdmin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace WebOrder
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private string name; // field

        protected void Application_Start(Object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            var request = ((System.Web.HttpApplication)sender).Request;
            string strPathName = request.QueryString["dky"];
            // string appUrl = HttpRuntime.AppDomainAppVirtualPath;
            if (strPathName == null && request.AppRelativeCurrentExecutionFilePath == "~/" )
            {
              Response.Redirect("http://www.pbsus.com");
            }
            else
            {
                if (request.AppRelativeCurrentExecutionFilePath == "~/home/SendOrderToPOS" ||
                     request.AppRelativeCurrentExecutionFilePath == "~/home/SchangeOrderStatus" ||
                     request.AppRelativeCurrentExecutionFilePath == "~/home/getOrderStatusFromPOS")
                {
                    strPathName = GetGroupID(GetPathName(request.QueryString["storeid"]));
                }
                else
                {
                    if (strPathName != "" && strPathName != null)
                    {
                        strPathName = GetGroupID(strPathName);
                    }
                    else
                        strPathName = "";
                }
            }

            if (strPathName != null && strPathName != "" )
            {
                
                RouteConfig.RegisterRoutes(RouteTable.Routes, strPathName);
            }

        }


        private string GetPathName(string storeid)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = string.Format(@"SELECT pathName FROM [dbo].[HQModels] WHERE  storeid = {0}", Convert.ToInt32(storeid));
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0).ToString();
                    }
                }
                else
                    return "";
                connection.Close();
            }
            return "";
        }

        private string GetGroupID(string strPathName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("", connection))
            {
                command.CommandText = string.Format(@"SELECT GroupID FROM [dbo].[HQModels] WHERE  groupLevel = 2 AND pathName = '{0}'", strPathName);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0).ToString();
                    }
                }
                else
                    return "";
                connection.Close();
            }
            return "";

        }




    }
}
