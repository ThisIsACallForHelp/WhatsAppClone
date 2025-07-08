using System;
using System.Data;
using System.Net.Mail;
using Data;
using Microsoft.AspNetCore.Mvc.Formatters;
namespace Web_Service
{
    public class MessageCreator : IModelCreator<Message>
    {
        public Message CreateModel(IDataReader src)
        {
            return new Message
            {
                SenderID = Convert.ToString(src["SenderID"]),
                SentAt = Convert.ToDateTime(src["SentAt"]),
                ID = Convert.ToString(src["MessageID"]),
                Attachments = Convert.ToString(src["Attachments"]),
                SenderPublicKeyBase64 = Convert.ToString(src["SenderPublicKeyBase64"]),
                SenderSigningKeyBase64 = Convert.ToString(src["SenderSigningKeyBase64"]),
                CipherTextBase64 = Convert.ToString(src["CipherTextBase64"]),
                HmacBase64 = Convert.ToString(src["HmacBase64"]),
                IVBase64 = Convert.ToString(src["IVBase64"]),
                SignatureBase64 = Convert.ToString(src["SignatureBase64"]),
                ChatID = Convert.ToString(src["ChatID"])
            };
        }
    }
}
