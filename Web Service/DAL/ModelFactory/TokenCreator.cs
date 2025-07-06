using System;
using System.Data;
using Data;
namespace Web_Service
{
    public class TokenCreator : IModelCreator<Token>
    {
        public Token CreateModel(IDataReader src)
        {
            return new Token()
            {
                ExpiresAt = Convert.ToDateTime(src["ExpiresAt"]),
                ID = Convert.ToString(src["TokenID"]),
                UserID = Convert.ToString(src["UserID"]),
                CreatedAt = Convert.ToDateTime(src["CreatedAt"])
            };
        }
    }
}
