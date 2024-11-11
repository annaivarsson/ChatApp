namespace RealChatApp
{
    public class Chat
    {
        public List<User> Users = new List<User>();

        public List<Message> Messages = new List<Message>();
        public string ChatName { get; set; }
        public int ChatId { get; set; }
        public DateTime Date { get; set; }


        public Chat(List<User> users, int chatid, List<Message> messages, string chatname, DateTime date)
        {
            Users = users;
            Messages = messages;
            ChatName = chatname;
            ChatId = chatid;
            Date = date;
        }


        public Chat()
        {

        }


        public void CreateMessage(User sender, string textmessage, string chatname, long chatid, DateTime date)
        {
            Message message = new Message(sender, textmessage, chatname, chatid, date);
            Messages.Add(message);
        }

    }
}
