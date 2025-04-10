
namespace MvcDHProject.Models
{
    public class CustomerExcelDal : ICustomerDal
    {
        List<Customer> ICustomerDal.Customers_Select()
        {
            throw new NotImplementedException();
        }

        void ICustomerDal.Customer_Delete(int CustId)
        {
            throw new NotImplementedException();
        }

        void ICustomerDal.Customer_Insert(Customer customer)
        {
            throw new NotImplementedException();
        }

        Customer ICustomerDal.Customer_Select(int CustId)
        {
            throw new NotImplementedException();
        }

        void ICustomerDal.Customer_Update(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
