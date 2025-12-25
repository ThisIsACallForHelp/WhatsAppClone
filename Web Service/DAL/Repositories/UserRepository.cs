using Data;
using System.Collections.Generic;
using System.Data;
namespace Web_Service
{
    public class UserRepository : Repository, IRepository<User>
    {
        public UserRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(User user)
        {
            DHEncryption dh = new DHEncryption();
            user.RecipientPublicKeyBase64 = Convert.ToBase64String(dh.PublicKey);
            user.RecipientSigningKeyBase64 = Convert.ToBase64String(dh.PublicSigningKey);
            string sql = $@"INSERT INTO FrutigerUser([UserID], [PhoneNumber], [FirstName], [LastName], [Email], [Password], [Avatar], [RecipientPublicKeyBase64], [RecipientSigningKeyBase64])
                VALUES(@UserID, @PhoneNumber, @FirstName, @LastName, @Email, @Password, @Avatar, @RecipientPublicKeyBase64, @RecipientSigningKeyBase64)";
            base.dbContext.AddParameters("@UserID", user.ID);
            base.dbContext.AddParameters("@PhoneNumber", user.PhoneNumber);
            base.dbContext.AddParameters("@FirstName", user.FirstName);
            base.dbContext.AddParameters("@LastName", user.LastName);
            base.dbContext.AddParameters("@Email", user.Email);
            base.dbContext.AddParameters("@Password", user.Password);
            base.dbContext.AddParameters("@Avatar", user.Avatar);
            base.dbContext.AddParameters("@RecipientPublicKeyBase64", user.RecipientPublicKeyBase64);
            base.dbContext.AddParameters("@RecipientSigningKeyBase64", user.RecipientSigningKeyBase64);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(User user)
        {
            string sql = $@"UPDATE User SET PhoneNumber = @PhoneNumber,
                                            FirstName = @FirstName, 
                                            LastName = @LastName, 
                                            Email = @Email, 
                                            Password = @Password
                                            Avatar = @Avatar
                                        WHERE UserID = @UserID";
            base.dbContext.AddParameters("@UserID", user.ID);
            base.dbContext.AddParameters("@FirstName", user.FirstName);
            base.dbContext.AddParameters("@PhoneNumber", user.PhoneNumber);
            base.dbContext.AddParameters("@LastName", user.LastName);
            base.dbContext.AddParameters("@Email", user.Email);
            base.dbContext.AddParameters("@Password", user.Password);
            return base.dbContext.Update(sql) > 0;
        }

        public bool Delete(User user)
        {
            string sql = $@"DELETE FROM User WHERE UserID = @UserID";
            base.dbContext.AddParameters("@UserID", user.ID);
            return base.dbContext.Delete(sql) > 0;
        }

        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM User WHERE UserID = @UserID";
            base.dbContext.AddParameters("@UserID", ID);
            return base.dbContext.Delete(sql) > 0;
        }

        public User GetByID(string ID)
        {
            string sql = $@"SELECT * FROM FrutigerUser WHERE [UserID] = @ID";
            base.dbContext.AddParameters("@ID", ID);
            Console.WriteLine("query -> " + sql);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.UserCreator.CreateModel(reader);
            }
        }
        public List<User> GetAll()
        {
            List<User> users = new List<User>();    
            string sql = $@"SELECT * FROM User";
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    users.Add(this.modelFactory.UserCreator.CreateModel(reader));
                }
                return users;
            }
        }

        public User Register(User user)
        {
            if (Create(user))
            {
                return user;
            }
            return new User();
        }
    }
}
