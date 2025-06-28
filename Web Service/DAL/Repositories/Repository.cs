namespace Web_Service
{
    public class Repository
    {
        protected DBContext dbContext;
        protected ModelFactory modelFactory;
        public Repository(DBContext dbContext)
        {
            this.dbContext = dbContext;
            this.modelFactory = new ModelFactory();
        }
        public int GetLastID()
        {
            string sql = "SELECT @@identity";
            return Convert.ToInt32(this.dbContext.ReadValue(sql));
        }
    }
}
