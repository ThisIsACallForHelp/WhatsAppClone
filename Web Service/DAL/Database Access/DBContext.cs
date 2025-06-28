using System.Data;
using System.Data.OleDb;
namespace Web_Service
{
    public class DBContext : IDBContext
    {
        OleDbConnection connection; //connection
        OleDbCommand command; //command
        OleDbTransaction transaction; //transaction

        static DBContext dbContext; //DBContext
        static object obj = new object();

        private DBContext() //Constructor 
        {
            this.connection = new OleDbConnection(); //new connection
            this.connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Directory.GetCurrentDirectory() + "\\DB Data\\WhatsApp.accdb";
            //DB link
            this.command = new OleDbCommand();
            //new command
            this.command = this.connection.CreateCommand();
            //create a command 
        }
        public static DBContext GetInstance()
        //function that creates an object
        {
            lock (obj)
            {
                if (dbContext == null)
                //check if the there is an object
                {
                    dbContext = new DBContext();
                    //if not - create one
                }
                return dbContext; //return it regardless
            }
        }
        public void OpenConnection() //open the connection to the DB
        {
            this.connection.Open();
        }
        public void CloseConnection() //close the connection to the DB
        {
            this.connection.Close();
            if (this.transaction != null) //if there is a transaction
            {
                this.transaction.Dispose(); //dispose of it 
            }
        }
        public void BeginTransaction() //begin the transaction
        {
            this.transaction = this.connection.BeginTransaction();
            this.command.Transaction = this.transaction;
        }
        public void Commit()
        // accept the transaction 
        {
            this.transaction.Commit();
        }
        public void RollBack()
        //cancel the transaction 
        {
            this.transaction.Rollback();
        }
        public int ChangeDB(string sql)
        //make a change in the DB
        {
            this.command.CommandText = sql;
            return this.command.ExecuteNonQuery();
        }
        public int Create(string sql)
        //create something in the DB
        {
            return this.ChangeDB(sql);
            //go to the function that changes things in the DB
        }
        public int Delete(string sql)
        //Delete from DB
        {
            return this.ChangeDB(sql);
            //go to the function that changes things in the DB
        }
        public IDataReader Read(string sql)
        //Read tables from DB 
        {
            this.command.CommandText = sql;
            return this.command.ExecuteReader();
        }
        public object ReadValue(string sql)
        //Read single value 
        {
            this.command.CommandText = sql;
            return this.command.ExecuteScalar();
        }
        public int Update(string sql)
        //Update the DB
        {
            return this.ChangeDB(sql);
        }
        public void AddParameters(string ParamName, string ParamValue)
        //Add parameters with a value to coommand
        {
            this.command.Parameters.Add(new OleDbParameter(ParamName, ParamValue));
        }
        public void ClearParameters()
        //clear the parameters 
        {
            this.command.Parameters.Clear();
        }
        public string GetLastInsertedID()
        //get the Id of the last inserted table 
        {
            string sql = "Select @@Identity";
            return ReadValue(sql).ToString();
        }
    }
}
