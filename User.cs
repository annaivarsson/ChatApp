namespace RealChatApp
{
    public class User
    {
        public string Name { get; set; }
        public long UserId { get; set; }


        public User(string username, long userid)
        {
            Name = username;
            UserId = userid;
        }


        public void CreateUser(string username)
        {

        }
    }
}