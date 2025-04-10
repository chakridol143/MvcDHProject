namespace MvcDHProject.Models
{
    public interface ICustomerDal
    {
        List<Customer> Customers_Select();
        Customer Customer_Select(int CustId);
        void Customer_Insert(Customer customer);
        void Customer_Update(Customer customer);
        void Customer_Delete(int CustId);
    }
}
