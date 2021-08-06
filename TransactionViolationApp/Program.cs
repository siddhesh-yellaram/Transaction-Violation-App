using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionViolationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string conString = ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(conString);
            try
            {
                sqlCon.Open();

                var serverName = sqlCon.DataSource;
                var database = sqlCon.Database;
                var state = sqlCon.State;
                var connectiontTimeout = sqlCon.ConnectionTimeout;

                string initCPrice = "", initMPrice = "";

                Console.WriteLine("Server Name: " + serverName);
                Console.WriteLine("Database Name: " + database);
                Console.WriteLine("Connection State: " + state);
                Console.WriteLine("Connection Timeout: " + connectiontTimeout + " sec.");
                Console.WriteLine();

                Console.Write("Enter Cust Id: ");
                string custId = Console.ReadLine();
                Console.Write("\nEnter Merchant Id: ");
                string merId = Console.ReadLine();
                Console.Write("\nEnter Purchase Amount: ");
                string purchase = Console.ReadLine();

                string getInitCust = "select c_price from mcustomers where c_id = @CustId";
                SqlCommand cmd = new SqlCommand(getInitCust, sqlCon);
                SqlParameter initCust = new SqlParameter();
                initCust.ParameterName = "@CustId";
                initCust.Value = custId; 
                cmd.Parameters.Add(initCust);

                string getInitMer = "select m_price from merchant where m_id = @MercId";
                SqlCommand cmd1 = new SqlCommand(getInitMer, sqlCon);
                SqlParameter initMer = new SqlParameter();
                initMer.ParameterName = "@MercId";
                initMer.Value = merId; 
                cmd1.Parameters.Add(initMer);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    initCPrice = reader[0].ToString();
                }
                reader.Close();

                string upCust = "Update mcustomers set c_price = @TotAmountRem where c_id = @CustId";
                SqlCommand cmd2 = new SqlCommand(upCust, sqlCon);
                SqlParameter totalAmtRem = new SqlParameter();
                SqlParameter cust = new SqlParameter();
                totalAmtRem.ParameterName = "@TotAmountRem";
                cust.ParameterName = "@CustId";
                string remVal = (Int32.Parse(initCPrice) - Int32.Parse(purchase)).ToString();
                //Console.WriteLine(remVal);
                totalAmtRem.Value = remVal;
                cust.Value = custId;
                cmd2.Parameters.Add(totalAmtRem);
                cmd2.Parameters.Add(cust);
                cmd2.ExecuteNonQuery();

                SqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    initMPrice = reader1[0].ToString();
                }
                reader1.Close();

                string upMerc = "Update merchant set m_price = @TotAmountAdd where m_id = @MercId";
                SqlCommand cmd3 = new SqlCommand(upMerc, sqlCon);
                SqlParameter totalAmtAdd = new SqlParameter();
                SqlParameter merc = new SqlParameter();
                totalAmtAdd.ParameterName = "@TotAmountAdd";
                merc.ParameterName = "@MercId";
                string totVal = (Int32.Parse(initMPrice) + Int32.Parse(purchase)).ToString();
                //Console.WriteLine(totVal);
                totalAmtAdd.Value = totVal;
                merc.Value = merId;
                cmd3.Parameters.Add(totalAmtAdd);
                cmd3.Parameters.Add(merc);
                cmd3.ExecuteNonQuery();

                SqlCommand printCustDet = new SqlCommand("select * from mcustomers",sqlCon);
                SqlDataReader reader2 = printCustDet.ExecuteReader();
                Console.WriteLine("\nDisplay Details from Customers:");
                while (reader2.Read())
                {
                    Console.WriteLine("Customer Id: "+reader2[0].ToString()+" Name: "+reader2[1].ToString()+" Tot Price: "+reader2[2].ToString());
                }
                reader2.Close();

                SqlCommand printMercDet = new SqlCommand("select * from merchant", sqlCon);
                SqlDataReader reader3 = printMercDet.ExecuteReader();
                Console.WriteLine("\nDisplay Details from Merchants:");
                while (reader3.Read())
                {
                    Console.WriteLine("Merchant Id: " + reader3[0].ToString() + " Name: " + reader3[1].ToString() + " Tot Price: " + reader3[2].ToString());
                }
                reader3.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
            Console.ReadLine();
        }
    }
}
