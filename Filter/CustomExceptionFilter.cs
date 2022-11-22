using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Practice_CoreApp_04_08.Filter
{
    public class CustomExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        public override void OnException(ExceptionContext filtercontext)
        {
            if (filtercontext.Exception is NotImplementedException)
            {

            }
            else
            {
                
                Log.Error(filtercontext.Exception.ToString());
                filtercontext.Result = new ViewResult()
                {
                    ViewName = "ErrorPage"
                };
                filtercontext.ExceptionHandled = true;
            }
        }
    }
}
