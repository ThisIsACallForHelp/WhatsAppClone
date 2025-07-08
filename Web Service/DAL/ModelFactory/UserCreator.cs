using Data;
using System.Data;
namespace Web_Service
{
    public class UserCreator : IModelCreator<User>
    {
        public User CreateModel(IDataReader src)
        {
            return new User
            {
                ID = Convert.ToString(src["UserID"]),
                FirstName = Convert.ToString(src["FirstName"]),
                LastName = Convert.ToString(src["LastName"]),
                Email = Convert.ToString(src["Email"]),
                Password = Convert.ToString(src["Password"]),
                PhoneNumber = Convert.ToString(src["PhoneNumber"]),
                Avatar = "https://localhost:7189/Users/" + Convert.ToString(src["Avatar"]),
            };
        }
    }
}
