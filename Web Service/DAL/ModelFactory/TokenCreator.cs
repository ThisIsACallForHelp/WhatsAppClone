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
                TokenStr = Convert.ToString(src["TokenStr"]),
                ExpiresAt = Convert.ToDateTime(src["ExpiresAt"]),
                CreatedAt = Convert.ToDateTime(src["CreatedAt"]),
                AuthUserID = Convert.ToString(src["AuthUserID"]),
                BrowserConnectID = Convert.ToString(src["BrowserConnectID"]),
            };
        }
    }
}
