using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
using System.Data.SQLite;
using System.Data;


namespace DBAccess
{
    public class DBAccessClass
    {
        String decryptedDBPassword;
        SQLiteConnection conn;

        
        //Function to Decrypt DB Password

        public String decryptPassword()
        {
            String dbpassword = System.Configuration.ConfigurationManager.AppSettings["DBPASSWORD"].ToString();
            String key = System.Configuration.ConfigurationManager.AppSettings["AESKEY"].ToString();
            String IV = System.Configuration.ConfigurationManager.AppSettings["IV"].ToString();
            
            byte[] encryptedBytes = Convert.FromBase64String(dbpassword);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            aes.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] decryptedbytes = crypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            decryptedDBPassword = System.Text.ASCIIEncoding.ASCII.GetString(decryptedbytes);
           
           // checking if password is successfully decrypted

            Console.WriteLine(decryptedDBPassword);

            return decryptedDBPassword;

        }

        // open connection

        public void openConnection()
        {

            String dbserver = System.Configuration.ConfigurationManager.AppSettings["DBSERVER"].ToString();
            String dbname = System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString();
            String dbusername = System.Configuration.ConfigurationManager.AppSettings["DBUSERNAME"].ToString();
            
            String dbpassword = decryptPassword();
            
            Console.WriteLine("Decrypted dbpasword: "+dbpassword);

            String connectionString = "Data Source = "+dbserver+"; Initial Catalog="+dbname+"; User Id="+dbusername+"; Password="+dbpassword+";Integrated Security=false";
           
            Console.WriteLine(connectionString);

            conn = new SQLiteConnection("Data Source=dvta.sqlite;Version=3;"); // Hardcoded connection string to use SQLite
            //conn.ConnectionString = connectionString; Not required for SQLITE
            conn.Open();

        }

        // user login
        public SQLiteDataReader checkLogin(String clientusername,String clientpassword)
        {
            
            String username = clientusername;
            String password = clientpassword;

            String sqlcmd = "SELECT * FROM users where username='"+username+"' and password='"+password+"'";
            Console.WriteLine(sqlcmd);
           
           
            SQLiteCommand cmd = new SQLiteCommand(sqlcmd, conn);
           
            /*
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);

           
            int numrowsreturned = dt.Rows.Count;
            return numrowsreturned;
            */

            SQLiteDataReader dtr = cmd.ExecuteReader();

            return dtr;

        }
     
        
   
        // User Registration

        public bool RegisterUser(String clientusername, String clientpassword, String clientemailid)
        {

            bool output = false;
            int isadmin = 0;
            string sqlquery = "insert into users (username, password, email, isadmin) VALUES ('" + clientusername + "','" + clientpassword + "','" + clientemailid + "'," + isadmin + ")";
            SQLiteCommand cmd = new SQLiteCommand(sqlquery, conn);

            try
            {
                cmd.ExecuteNonQuery();
                output = true;
               
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            return output;

        }
        public bool addExpenses(String addDt, String additem, String addprice,String addemail,String addTime)
        {

            bool output = false;
            string sqlquery = "INSERT INTO expenses (email, item, price, date, time) VALUES ('" + addemail + "','" + additem + "','" + addprice + "','" + addDt + "','" + addTime + "')";
            SQLiteCommand cmd = new SQLiteCommand(sqlquery, conn);

            try
            {
                cmd.ExecuteNonQuery();
                output = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return output;

        }

        public DataTable viewExpenses(String emailid)
        {
            
            SQLiteCommand objCommand = new SQLiteCommand("select item, price, date,time from expenses where email='"+emailid+"'", conn);

            SQLiteDataReader rdr = objCommand.ExecuteReader();

            DataTable objData = new DataTable();

            objData.Load(rdr);

            return objData;

         }
        
        public DataTable getExpensesOfAll()
        {
            
            SQLiteCommand objCommand = new SQLiteCommand("select * from expenses", conn);

            SQLiteDataReader rdr = objCommand.ExecuteReader();

            DataTable objData = new DataTable();

            objData.Load(rdr);

            return objData;

         }
      

        public bool clearExpenses(String emailid)
        {
            bool output = false;
            String sqlcmd = "DELETE FROM expenses where email='" + emailid + "'";
            SQLiteCommand cmd = new SQLiteCommand(sqlcmd, conn);

            try
            {
                cmd.ExecuteNonQuery();
                output = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return output;

        }


        //close connection
        public void closeConnection()
        {
            conn.Close();

        }

    }
}
