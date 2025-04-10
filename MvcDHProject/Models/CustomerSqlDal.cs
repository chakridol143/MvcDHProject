
namespace MvcDHProject.Models
{
    public class CustomerSqlDal : ICustomerDal
    {
        private readonly MVCCoreDbContext dc;

        public CustomerSqlDal(MVCCoreDbContext dc)
        {
            this.dc = dc;
        }
        public List<Customer> Customers_Select()
        {
            return dc.Customers.Where(c=>c.Status == true).ToList();
        }
        public Customer Customer_Select(int CustId)
        {
            return dc.Customers.Find(CustId);
        }
        public void Customer_Insert(Customer customer)
        {
            customer.Status = true;
            dc.Customers.Add(customer);
            dc.SaveChanges();
        }
        public void Customer_Update(Customer customer)
        {
            customer.Status = true;
            dc.Customers.Update(customer);
            dc.SaveChanges();
        }
        public void Customer_Delete(int CustId)
        {
            var customer = dc.Customers.Find(CustId);
            customer.Status = false;
            //dc.Customers.Remove(customer);
            dc.SaveChanges();
        }
    }
}
