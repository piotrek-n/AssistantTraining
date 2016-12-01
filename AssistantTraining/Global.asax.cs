using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AssistantTraining
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            App_Start.BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
         
            Response.Clear();


            if (IsAjaxRequest())
            {
                Response.Write("Application_Error");
            }
            else
            {

                var exception = Server.GetLastError();
                if (exception == null)
                    return;

                //var mail = new MailMessage { From = new MailAddress("automated@contoso.com") };
                //mail.To.Add(new MailAddress("administrator@contoso.com"));
                //mail.Subject = "Site Error at " + DateTime.Now;
                //mail.Body = "Error Description: " + exception.Message;
                //var server = new SmtpClient { Host = "your.smtp.server" };
                //server.Send(mail);

                //Redirect to an error page
                Response.Redirect("/Home/Index");
                Response.End();

            }

            Server.ClearError();
        }

        private bool IsAjaxRequest()
        {
            //The easy way
            bool isAjaxRequest = (Request["X-Requested-With"] == "XMLHttpRequest")
            || ((Request.Headers != null)
            && (Request.Headers["X-Requested-With"] == "XMLHttpRequest"));

            //If we are not sure that we have an AJAX request or that we have to return JSON 
            //we fall back to Reflection
            if (!isAjaxRequest)
            {
                try
                {
                    //The controller and action
                    string controllerName = Request.RequestContext.
                                            RouteData.Values["controller"].ToString();
                    string actionName = Request.RequestContext.
                                        RouteData.Values["action"].ToString();

                    //We create a controller instance
                    DefaultControllerFactory controllerFactory = new DefaultControllerFactory();
                    Controller controller = controllerFactory.CreateController(
                    Request.RequestContext, controllerName) as Controller;

                    //We get the controller actions
                    ReflectedControllerDescriptor controllerDescriptor =
                    new ReflectedControllerDescriptor(controller.GetType());
                    ActionDescriptor[] controllerActions =
                    controllerDescriptor.GetCanonicalActions();

                    //We search for our action
                    foreach (ReflectedActionDescriptor actionDescriptor in controllerActions)
                    {
                        if (actionDescriptor.ActionName.ToUpper().Equals(actionName.ToUpper()))
                        {
                            //If the action returns JsonResult then we have an AJAX request
                            if (actionDescriptor.MethodInfo.ReturnType
                            .Equals(typeof(JsonResult)))
                                return true;
                        }
                    }
                }
                catch
                {

                }
            }

            return isAjaxRequest;
        }

    }
}