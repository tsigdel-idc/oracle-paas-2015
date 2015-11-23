using System;

// Fixing redirection loop problem on the login page. A new attribute with the same name (AuthorizeAttribute) in
// the default namespace (important!) will be automatically picked up by compiler instead of MVC's standard one.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
{
    protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
    {
        if (filterContext.HttpContext.Request.IsAuthenticated)
        {
            filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
        }
        else
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}