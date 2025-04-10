using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDHProject.Models;

namespace MvcDHProject.Controllers
{
        [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerDal dal;

        public CustomerController(ICustomerDal dal)
        {
            this.dal = dal;
        }
        [AllowAnonymous]
        public ViewResult DisplayCustomers()
        {
            return View(dal.Customers_Select());
        }
        public ViewResult DisplayCustomer(int CustId)
        {
            return View(dal.Customer_Select(CustId));
        }
        public ViewResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public RedirectToActionResult AddCustomer(Customer customer)
        {
            dal.Customer_Insert(customer);
            return RedirectToAction("DisplayCustomers",customer);
        }
        public ViewResult EditCustomer(int CustId)
        {
            return View(dal.Customer_Select(CustId));
        }
        [HttpPost]
        public RedirectToActionResult EditCustomer(Customer customer)
        {
            dal.Customer_Update(customer);
            return RedirectToAction("DisplayCustomers",customer);
        }
        public RedirectToActionResult DeleteCustomer(int CustId)
        {
            dal.Customer_Delete(CustId);
            return RedirectToAction("DisplayCustomers");
        }
    }
}
