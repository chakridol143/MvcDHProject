using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MvcDHProject.Models
{
    public class CustomerXmlDal : ICustomerDal
    {
        DataSet ds;
        public CustomerXmlDal()
        {
            ds = new DataSet();
            ds.ReadXml("Customer.Xml");
            ds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns["CustId"] };
        }
        public List<Customer> Customers_Select()
        {
            List<Customer> customers = new List<Customer>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Customer customer = new Customer
                {
                    CustId = Convert.ToInt32(dr["CustId"]),
                    Name = Convert.ToString(dr["Name"]),
                    City = Convert.ToString(dr["City"]),
                    Balance = Convert.ToDecimal(dr["Balance"]),
                    Status = Convert.ToBoolean(dr["Status"])
                };
                customers.Add(customer);
            }
            return customers;
        }
        public Customer Customer_Select(int CustId)
        {
            DataRow dr = ds.Tables[0].Rows.Find(CustId);
            Customer customer = new Customer
            {
                CustId = Convert.ToInt32(dr["CustId"]),
                Name = Convert.ToString(dr["Name"]),
                Balance = Convert.ToDecimal(dr["Balance"]),
                City = Convert.ToString(dr["City"]),
                Status = Convert.ToBoolean(dr["Status"])
            };
            return customer;
        }
        public void Customer_Insert(Customer customer)
        {
            DataRow dr = ds.Tables[0].NewRow();
            dr["CustId"] = customer.CustId;
            dr["Name"] = customer.Name;
            dr["City"] = customer.City;
            dr["Balance"] = customer.Balance;
            dr["Status"] = customer.Status;
            ds.Tables[0].Rows.Add(dr);
            ds.WriteXml("Customer.xml");
        }
        public void Customer_Update(Customer customer)
        {
            DataRow dr = ds.Tables[0].Rows.Find(customer.CustId);
            int index = ds.Tables[0].Rows.IndexOf(dr);
            ds.Tables[0].Rows[index]["Name"] = customer.Name;
            ds.Tables[0].Rows[index]["City"] = customer.City;
            ds.Tables[0].Rows[index]["Balance"] = customer.Balance;
            ds.WriteXml("Customer.Xml");
        }
        public void Customer_Delete(int CustId)
        {
            DataRow dr = ds.Tables[0].Rows.Find(CustId);
            int index = ds.Tables[0].Rows.IndexOf(dr);
            ds.Tables[0].Rows[index].Delete();
            ds.WriteXml("Customer.Xml");
        }
    }
}
