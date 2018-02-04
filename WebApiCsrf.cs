using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Providers
{
    public class WebApiCsrf : ActionFilterAttribute
    {
        public string Header { get; set; }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            string _context = filterContext.Request.Headers.FirstOrDefault(i => i.Key == Header).Value.ElementAt(0).ToString();
            bool Result = ValidateRequestHeader(_context);

            if (Result == false)
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Something went wrong") };

            base.OnActionExecuting(filterContext);
        }


        private bool ValidateRequestHeader(string csrf)
        {
            string cookieToken = "";
            string formToken = "";

            if (!String.IsNullOrEmpty(csrf))
            {
                string[] tokens = csrf.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            try
            {
                AntiForgery.Validate(cookieToken, formToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}