using MySql.Data.MySqlClient;
using System.Data;


namespace RealChatApp
{
    public class DataBase
    {
        // Method that controls if there already is a chat in Chat table.
        public static long CheckChatTable(string chatName)
        {
            long chatId = -1;// If a chat is found the method will retur´n the chat_id, if not found -1 is returned.

            // Create a password string for env variables.
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            // Connection string to the database is created.
            string connStr = $"server=localhost;user=root;database=new_schema;port=3306;password={password}";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Checks if chatName is present in the Chat table. If present: chat_id is reused, otherwise a new chat is created.
                    string sqlCheckChat = "SELECT chat_id FROM Chat WHERE chatname = @chatName";

                    MySqlCommand cmdCheckChat = new MySqlCommand(sqlCheckChat, conn); 

                    cmdCheckChat.Parameters.AddWithValue("@chatName", chatName); 

                    object chatIdObj = cmdCheckChat.ExecuteScalar(); 

                    if (chatIdObj != null)
                    {
                        chatId = Convert.ToInt64(chatIdObj);
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine("Ett fel uppstod:" + ex.ToString());
                }
                return chatId;
            }
        }

        public static long CreateChat(string chatName)
        {
            long chatId = -1; 

            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            string connStr = $"server=localhost;user=root;database=new_schema;port=3306;password={password}";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sqlChat = "INSERT INTO Chat (chatname, chat_created) VALUES (@chatName, NOW())";

                    MySqlCommand cmdChat = new MySqlCommand(sqlChat, conn);

                    cmdChat.Parameters.AddWithValue("@chatName", chatName); 

                    cmdChat.ExecuteNonQuery();

                    chatId = cmdChat.LastInsertedId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ett fel uppstod" + ex.ToString());
                }
            }
            return chatId;
        }



        public static long CheckUserTable(string userName)
        {
            long userId = -1; 

            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            string connStr = $"server=localhost;user=root;database=new_schema;port=3306;password={password}";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sqlCheckUser = "SELECT user_id FROM Users WHERE username = @userName";

                    MySqlCommand cmdCheckUser = new MySqlCommand(sqlCheckUser, conn);

                    cmdCheckUser.Parameters.AddWithValue("@userName", userName);

                    object userIdObj = cmdCheckUser.ExecuteScalar();

                    if (userIdObj != null)
                    {
                        userId = Convert.ToInt64(userIdObj);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ett fel har inträffat" + ex.ToString());
                }
            }
            return userId;
        }

        public static long CreateUser(string userName)
        {
            long userId = -1;

            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            string connStr = $"server=localhost;user=root;database=new_schema;port=3306;password={password}";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sqlUser = "INSERT INTO Users (username, created_at) VALUES (@userName, NOW())";

                    MySqlCommand cmdUser = new MySqlCommand(sqlUser, conn);

                    cmdUser.Parameters.AddWithValue("@userName", userName);

                    cmdUser.ExecuteNonQuery();

                    userId = cmdUser.LastInsertedId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ett fel uppstod" + ex.ToString());
                }
            }
            return userId;
        }


        public static string GenerateSqlString(string chatName, string userName, string messageText)
        {
            long chatId = CheckChatTable(chatName);
            if (chatId == -1)
            {
                chatId = CreateChat(chatName);
            }

            long userId = CheckUserTable(userName);
            if (userId == -1)
            {
                userId = CreateUser(userName);
            }
            string sqlMessage = "INSERT INTO Messages (user_id, message, chat_id, message_sent) VALUES (@userId, @messageText, @chatId, NOW())";

            return sqlMessage;
        }



        // Coordinates check and create. Creates a new chat and user if they already not exist, also adds a message.
        public static void RunTransaction(string sqlMessage, long userId, string messageText, long chatId)
        {
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            string connStr = $"server=localhost;user=root;database=new_schema;port=3306;password={password}";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    using (MySqlCommand cmdMessages = new MySqlCommand(sqlMessage, conn))
                    {
                        cmdMessages.Parameters.AddWithValue("@userId", userId);
                        cmdMessages.Parameters.AddWithValue("@messageText", messageText);
                        cmdMessages.Parameters.AddWithValue("@chatId", chatId);
                        cmdMessages.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ett fel uppstod" + ex.ToString());
                }
            }
        }


        public static DataTable Fetchmessages(long chatId)
        {
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

            string connStr = $"server=localhost;user=root;database=new_schema;port=3306;password={password}";

            string sqlFetchJoinMessagesChatAndUsers = $"SELECT Messages.user_id, Messages.message, Messages.chat_id, Messages.message_sent, Chat.chatname, Users.username FROM Chat JOIN Messages ON Chat.chat_id = Messages.chat_id JOIN Users ON Users.user_id = Messages.user_id ORDER BY Messages.message_sent";

            MySqlConnection conn = new MySqlConnection(connStr);

            MySqlDataAdapter Adapter = new MySqlDataAdapter(sqlFetchJoinMessagesChatAndUsers, conn);
                
                    MySqlCommandBuilder cb = new MySqlCommandBuilder(Adapter);
                    
                    DataTable dt = new DataTable();
                    Adapter.Fill(dt);
                    return dt;    
        }
    }
}
