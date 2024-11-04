﻿namespace RealChatApp
{
    public class Message
    {
        public User Sender { get; set; }
        public string TextMessage { get; set; }
        public string ChatName { get; set; }
        public long ChatID { get; set; }
        public DateTime Date { get; set; }

        public Message(User sender, string textmessage, string chatname, long chatid, DateTime date)
        {
            Sender = sender;
            TextMessage = textmessage;
            ChatName = chatname;
            ChatID = chatid;
            Date = date;
        }
    }
}