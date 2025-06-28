using System.Data;

namespace Web_Service
{
    public interface IDBContext
    {
        void OpenConnection(); //open the connection
        void CloseConnection(); //close the connection
        IDataReader Read(string sql); //read the entity from DB
        object ReadValue(string sql); //read single value from DB
        int Update(string sql); //Update entity in DB
        int Create(string sql); //Insert a new entity to the DB
        int Delete(string sql); //Delete entity from the DB
        void Commit(); //Commit transaction 
        void RollBack(); //cancel transaction 
        void BeginTransaction(); //Begin the transaction 
    }
}
