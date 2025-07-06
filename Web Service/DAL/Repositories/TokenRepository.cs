using Data;
using System.Data;
namespace Web_Service
{
    public class TokenRepository : Repository, IRepository<Token>
    {
        public TokenRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(Token token)
        {
            string sql = $@"INSERT INTO Token(TokenID, UserID, CreatedAt, ExpiresAt)
                                        VALUES(@TokenID,@ UserID, @CreatedAt, @ExpiresAt)";
            base.dbContext.AddParameters("@TokenID", token.ID);
            base.dbContext.AddParameters("@UserID", token.UserID);
            base.dbContext.AddParameters("@CreatedAt", token.CreatedAt.ToString());
            base.dbContext.AddParameters("@ExpiresAt", token.ExpiresAt.ToString());
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(Token token)
        {
            string sql = $@"UPDATE Token SET UserID = @UserID, 
                                             CreatedAt = CreatedAt,
                                             ExpiresAt = ExpiresAt
                                         WHERE TokenID = @TokenID";
            base.dbContext.AddParameters("@UserID", token.UserID);
            base.dbContext.AddParameters("@CreatedAt", token.CreatedAt.ToString());
            base.dbContext.AddParameters("@ExpiresAt", token.ExpiresAt.ToString());
            base.dbContext.AddParameters("@TokenID", token.ID);
            return base.dbContext.Update(sql) > 0;
        }
        public bool Delete(Token token)
        {
            string sql = $@"DELETE FROM Token WHERE TokenID = @TokenID";
            base.dbContext.AddParameters("@TokenID", token.ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM Token WHERE TokenID = @TokenID";
            base.dbContext.AddParameters("@TokenID", ID);
            return base.dbContext.Delete(sql) > 0;
        }

        public List<Token> GetAll()
        {
            string sql = $@"SELECT * FROM Token";
            List<Token> tokens = new List<Token>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    tokens.Add(this.modelFactory.TokenCreator.CreateModel(reader));
                }
                return tokens;
            }
        }

        public Token GetByID(string ID)
        {
            string sql = $@"SELECT * FROM Token WHERE TokenID = @TokenID";
            base.dbContext.AddParameters("@TokenID", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.TokenCreator.CreateModel(reader);
            }
        }
    }
}
