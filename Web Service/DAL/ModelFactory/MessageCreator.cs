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
                Content = Convert.ToString(src["Content"]),
                Attachments = Convert.ToString(src["Attachments"])
            };
        }
    }
}
