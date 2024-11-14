using MySql.Data.MySqlClient;
using System.Data;


namespace RealChatApp
{
    public class DataBase
    {
        private readonly string ConnStr;


        public DataBase() 
        {
            string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
            ConnStr = $"server=localhost;user=root;database=new_schema;port=3306;password={password}";
        }


        // Method that controls if there already is a chat in Chat table. 
        public MySqlConnection CreateConnection()
        {
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            return conn;
        }


        public long GetChatId(string chatName)
        {
            long chatId = -1;
            
            try
            {
                using (MySqlConnection conn = CreateConnection())
                {
                    string sqlGetChatId = @"
                        SELECT 
                            chat_id 
                        FROM 
                            Chat 
                        WHERE
                            chatname = @chatName";
                    using (MySqlCommand cmdGetChatId = new MySqlCommand(sqlGetChatId, conn))
                    {
                        cmdGetChatId.Parameters.AddWithValue("@chatName", chatName);
                        object chatIdObj = cmdGetChatId.ExecuteScalar();

                        if (chatIdObj != null)
                        {
                            chatId = Convert.ToInt64(chatIdObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred in GetUserId():" + ex.ToString());
            }
            return chatId;
        }


        public long CreateChat(string chatName)
        { 
            long chatId = -1;
            try
            {
                using (MySqlConnection conn = CreateConnection())
                {
                    string sqlChat = @"
                        INSERT INTO 
                            Chat (chatname, chat_created) 
                        VALUES 
                            (@chatName, NOW())";

                    using MySqlCommand cmdCreateChat = new MySqlCommand(sqlChat, conn);
                    cmdCreateChat.Parameters.AddWithValue("@chatName", chatName);
                    cmdCreateChat.ExecuteNonQuery();
                    chatId = cmdCreateChat.LastInsertedId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in CreateChat():" + ex.ToString());
            }
            return chatId;
        }


        public long? FetchUserIdFromDatabase(string username)
        {
            string sqlFetchUserId = @"
                            SELECT
                                user_id
                            FROM
                                Users
                            WHERE
                                username = @username";
            using (MySqlConnection conn = CreateConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlFetchUserId, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    try
                    {
                        object userIdObject = cmd.ExecuteScalar();

                        if (userIdObject != null)
                        {
                            return Convert.ToInt64(userIdObject);
                        }
                        else
                        {
                            Console.WriteLine("The user was not found");
                            return null;
                        }
                    }
                    catch (Exception ex) 
                    {
                        Console.WriteLine($"An error occured in FetchUserIdFromDatabase(): {ex.Message}");
                        return null;
                    }
                }
            }
        }


        public long GetUserId(string userName)
        {
            long userId = FetchUserIdFromDatabase(userName) ?? CreateUser(userName);
            return userId;
        }


        public long CreateUser(string userName)
        {
            long userId = -1;

            try
            {
                using (MySqlConnection conn = CreateConnection())
                {
                    string sqlUser = @"
                        INSERT INTO 
                            Users (username, created_at) 
                        VALUES 
                            (@userName, NOW())";

                    MySqlCommand cmdUser = new MySqlCommand(sqlUser, conn);
                    cmdUser.Parameters.AddWithValue("@userName", userName);
                    cmdUser.ExecuteNonQuery();
                    userId = cmdUser.LastInsertedId;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error has occurred in CreateUser()" + ex.ToString());
            }
            return userId;
        }


        public string GenerateSqlString(string chatName, string userName, string messageText)
        {
            long chatId = GetChatId(chatName);

            if (chatId == -1)
            {
                chatId = CreateChat(chatName);
            }

            long userId = GetUserId(userName);

            if (userId == -1)
            {
                userId = CreateUser(userName);
            }

            string sqlMessage = @"
                INSERT INTO 
                    Messages (user_id, message, chat_id, message_sent) 
                VALUES 
                    (@userId, @messageText, @chatId, NOW())";

            return sqlMessage;
        }


        // Coordinates check and create. Creates a new chat and user if they already not exist, also adds a message.
        public void InsertChatMessage(string sqlMessage, long userId, string messageText, long chatId)
        {
            using (MySqlConnection conn = CreateConnection())
            {
                try
                {
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
                    Console.WriteLine("An error occurred in InsertChatMessage" + ex.ToString());
                }
            }
        }


        // Fetches historical chat messages and populates the chat window.
        public DataTable Fetchmessages(long chatId)
        {
            string sqlFetchJoinMessagesChatAndUsers = @"
                SELECT
                    Messages.user_id, 
                    Messages.message, 
                    Messages.chat_id, 
                    Messages.message_sent,
                    Chat.chatname, 
                    Users.username 
                FROM 
                    Chat 
                INNER JOIN Messages ON Chat.chat_id = Messages.chat_id 
                INNER JOIN Users ON Users.user_id = Messages.user_id 
                WHERE Chat.chat_id = @chatId
                ORDER BY Messages.message_sent";

            using (MySqlConnection conn = CreateConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sqlFetchJoinMessagesChatAndUsers, conn))
                { 
                    cmd.Parameters.AddWithValue("@chatId", chatId);
                    MySqlDataAdapter Adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    Adapter.Fill(dt);

                    return dt;
                }
            }
        }
    }
}
