using Microsoft.AspNetCore.Mvc;

namespace MvcDHProject.Controllers
{
    public class ErrorController : Controller
    {
        [Route("ClientError/{StatusCode}")]
        public IActionResult ClientHandleError(int StatusCode)
        {
            switch (StatusCode) {
                case 400:
                    ViewBag.ErrorTitle= "Bad Request";
                    ViewBag.ErrorDescription="Server cannot or will not process the request due to a client error.";
                break;
                case 401:
                    ViewBag.ErrorTitle="UnAuthorized";
                    ViewBag.ErrorDescription = "The request sent to the server lacks valid authentication credentials.";
                    break;
                case 402:
                    ViewBag.ErrorTitle= "Payment Required";
                    ViewBag.ErrorDescription="It's a client error response code that indicates that the requested API or service is not functional due to a lack of payment.";
                    break;
                case 403:
                    ViewBag.ErrorTitle="Forebidden";
                    ViewBag.ErrorDescription="Access denied. You don't have permission to access the requested resource.";
                    break;
                case 404:
                    ViewBag.ErrorTitle="Not Found";
                    ViewBag.ErrorDescription="The requested resource could not be found on this server.";
                    break;
            }
            return View("ClientError");
        }
    }
}
