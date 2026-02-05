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
            string sql = $@"INSERT INTO Token(TokenStr, UserID, CreatedAt, ExpiresAt, BrowserConnectID)
                                        VALUES(@TokenStr,@AuthUserID, @CreatedAt, @ExpiresAt, @BrowserConnectID)";
            
            base.dbContext.AddParameters("@TokenStr", token.TokenStr);
            base.dbContext.AddParameters("@UserID", token.AuthUserID);
            base.dbContext.AddParameters("@CreatedAt", token.CreatedAt.ToString());
            base.dbContext.AddParameters("@ExpiresAt", token.ExpiresAt.ToString());
            base.dbContext.AddParameters("@BrowserConnectID", token.BrowserConnectID);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(Token token)
        {
            string sql = $@"UPDATE Token SET UserID = @AuthUserID, 
                                             CreatedAt = @CreatedAt,
                                             ExpiresAt = @ExpiresAt,
                                             BrowserConnectID = @BrowserConnectID
                                         WHERE TokenStr = @TokenStr";
            base.dbContext.AddParameters("@UserID", token.AuthUserID);
            base.dbContext.AddParameters("@CreatedAt", token.CreatedAt.ToString());
            base.dbContext.AddParameters("@ExpiresAt", token.ExpiresAt.ToString());
            base.dbContext.AddParameters("@BrowserConnectID", token.BrowserConnectID);
            base.dbContext.AddParameters("@TokenStr", token.TokenStr);
            return base.dbContext.Update(sql) > 0;
        }
        public bool Delete(Token token)
        {
            string sql = $@"DELETE FROM Token WHERE TokenStr = @TokenStr";
            base.dbContext.AddParameters("@TokenStr", token.TokenStr);
            return base.dbContext.Delete(sql) > 0;
        }
        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM Token WHERE TokenStr = @TokenStr";
            base.dbContext.AddParameters("@TokenStr", ID);
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
            string sql = $@"SELECT * FROM Token WHERE TokenStr = @TokenStr";
            base.dbContext.AddParameters("@TokenStr", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.TokenCreator.CreateModel(reader);
            }
        }
    }
}
