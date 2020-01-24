using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Scada.model.DBs;

namespace Scada.core
{
    public class DB : IDisposable
    {
        private User activeUser;

        public User ActiveUser
        {
            get { return activeUser; }
            set { activeUser = value; }
        }

        private string dbName;

        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        private string dbPassword;

        public string DbPassword
        {
            get { return dbPassword; }
            set { dbPassword = value; }
        }

        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public DB(string dbName, string dbPassword = "")
        {
            this.DbName = dbName;
            this.DbPassword = dbPassword;
            if (dbPassword == "")
            {
                ConnectionString = "Data Source=" + DbName;
            }
            else
            {
                ConnectionString = "Data Source=" + DbName + ";" + "Password=" + DbPassword;
            }
        }

        /// <summary>
        /// if not exist db file => create db file
        /// </summary>
        /// <param name="dbName"></param>
        public void CheckDB()
        {
            if (!File.Exists(DbName))
                SQLiteConnection.CreateFile(DbName);
            CreateTables();
        }

        /// <summary>
        /// SQLite create tables if no exist tables 
        /// </summary>
        public void CreateTables()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + DbName))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {

                    command.CommandText = "SELECT name FROM sqlite_master WHERE name='Users'";
                    var name = command.ExecuteScalar();

                    // check account table exist or not 
                    // if exist do nothing 
                    if (name != null && name.ToString() == "Users")
                        return;
                    // acount table not exist, create table and insert 
                    command.CommandText = "CREATE TABLE 'Users' ( `User_ID` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `UserName` TEXT NOT NULL, `UserPassword` TEXT NOT NULL, `SuperUser` INTEGER DEFAULT 0, `Enable` INTEGER DEFAULT 1, `Authorization` INTEGER DEFAULT 0, `Email` TEXT, `Name` TEXT, `SecondName` TEXT, `Surname` TEXT, `Title` TEXT, `Position` TEXT, `PhotoAddress` TEXT, `CardID` TEXT, `DateTime` TEXT DEFAULT '17.04.2018 00:00:00' )";
                    command.ExecuteNonQuery();
                    AddUser(); // Add admin user
                }
                con.Close();

                //con.SetPassword(DbPassword);
            }
        }

        /// <summary>
        /// SQLite Check Exist Records in UserName Column by Same Name
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public bool IsThereRecordbyUserName(string UserName)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT UserName FROM Users WHERE UserName='" + UserName + "'";
                    var name = command.ExecuteScalar();
                    if (name != null && name.ToString() == UserName)
                    {
                        return true;
                    }
                }
                con.Close();
            }
            return false;
        }

        /// <summary>
        /// SQLite Check Exist Records in CardID Column by Same Name
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public bool IsThereRecordbyCardID(string CardID)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT CardID FROM Users WHERE CardID='" + CardID + "'";

                    var name = command.ExecuteScalar();

                    if (name != null && name.ToString() == CardID)
                    {
                        return true;
                    }
                }

                con.Close();
            }
            return false;
        }

        /// <summary>
        /// SQLite Check Exist Records for Update Operation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsThereRecordbyUserName(User user)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT UserName FROM Users WHERE UserName = @UserName AND User_ID != @User_ID";

                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@UserName", user.UserName));
                    command.Parameters.Add(new SQLiteParameter("@User_ID", user.User_ID));

                    var name = command.ExecuteScalar();

                    if (name != null && name.ToString() == user.UserName)
                    {
                        return true;
                    }
                }
                con.Close();
            }
            return false;
        }

        /// <summary>
        /// SQLite Check Exist Records for Update Operation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsThereRecordbyCardID(User user)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT CardID FROM Users WHERE CardID = @CardID AND User_ID != @User_ID";

                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@CardID", user.CardID));
                    command.Parameters.Add(new SQLiteParameter("@User_ID", user.User_ID));

                    var cardID = command.ExecuteScalar();

                    if (cardID != null && cardID.ToString() == user.CardID)
                    {
                        return true;
                    }
                }
                con.Close();
            }
            return false;
        }

        /// <summary>
        /// SQLite Check Exist Records in User Column by Same Name Expect Id
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public bool IsThereRecord(string UserName, long User_ID)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT UserName FROM Users WHERE UserName='" + UserName + "' AND User_ID !=" + User_ID.ToString() + "";
                    var name = command.ExecuteScalar();
                    if (name != null && name.ToString() == UserName)
                    {
                        return true;
                    }
                }
                con.Close();
            }
            return false;
        }


        /// <summary>
        /// SQLite Check Exist Records 
        /// </summary>
        /// <param name="User_ID"></param>
        /// <returns></returns>
        public bool IsThereRecordByID(long User_ID)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT User_ID FROM Users WHERE User_ID ==" + User_ID.ToString() + "";
                    var userID = command.ExecuteScalar();
                    if (userID != null && (long)userID == User_ID)
                    {
                        return true;
                    }
                }
                con.Close();
            }
            return false;
        }

        public List<User> GetEditableUsers(short CurrentAuthLevel, object OwnUser_ID)
        {
            List<User> dbUserList = new List<User>();
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Users Where Authorization < " + CurrentAuthLevel + " And User_ID != " + OwnUser_ID + " order by Authorization desc";
                    SQLiteDataReader row = command.ExecuteReader();

                    while (row.Read())
                    {
                        dbUserList.Add(new User
                        {
                            User_ID = (long)row["User_ID"],
                            UserName = (string)row["UserName"],
                            UserPassword = core.CryptorEngine.Decrypt((string)row["UserPassword"], true),
                            SuperUser = Convert.ToInt32(row["SuperUser"]) == 0 ? false : true,
                            Enable = Convert.ToInt32(row["Enable"]) == 0 ? false : true,
                            Authorization = Convert.ToInt32(row["Authorization"]),
                            Email = (row["Email"] == DBNull.Value) ? string.Empty : (string)row["Email"],
                            Name = (row["Name"] == DBNull.Value) ? string.Empty : (string)row["Name"],
                            SecondName = (row["SecondName"] == DBNull.Value) ? string.Empty : (string)row["SecondName"],
                            Surname = (row["Surname"] == DBNull.Value) ? string.Empty : (string)row["Surname"],
                            Title = (row["Title"] == DBNull.Value) ? string.Empty : (string)row["Title"],
                            Position = (row["Position"] == DBNull.Value) ? string.Empty : (string)row["Position"],
                            PhotoAddress = (row["PhotoAddress"] == DBNull.Value) ? string.Empty : (string)row["PhotoAddress"],
                            CardID = (row["CardID"] == DBNull.Value) ? string.Empty : (string)row["CardID"],
                            DateTime = (row["DateTime"] == DBNull.Value || row["DateTime"].ToString() == "") ? DateTime.Now : DateTime.Parse(row["DateTime"].ToString())
                        });
                    }
                }
                con.Close();
            }
            return dbUserList;
        }

        public User GetUser(long User_ID)
        {
            User user = null;
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Users Where  Enable == @Enable And SuperUser == @SuperUser AND User_ID == @User_ID";

                    command.CommandType = System.Data.CommandType.Text;

                    command.Parameters.Add(new SQLiteParameter("@User_ID", User_ID));
                    command.Parameters.Add(new SQLiteParameter("@SuperUser", false));
                    command.Parameters.Add(new SQLiteParameter("@Enable", true));

                    SQLiteDataReader row = command.ExecuteReader();

                    if (row.Read())
                    {
                        user = new User
                        {
                            User_ID = (long)row["User_ID"],
                            UserName = (string)row["UserName"],
                            UserPassword = core.CryptorEngine.Decrypt((string)row["UserPassword"], true),
                            SuperUser = Convert.ToInt32(row["SuperUser"]) == 0 ? false : true,
                            Enable = Convert.ToInt32(row["Enable"]) == 0 ? false : true,
                            Authorization = Convert.ToInt32(row["Authorization"]),
                            Email = (row["Email"] == DBNull.Value) ? string.Empty : (string)row["Email"],
                            Name = (row["Name"] == DBNull.Value) ? string.Empty : (string)row["Name"],
                            SecondName = (row["SecondName"] == DBNull.Value) ? string.Empty : (string)row["SecondName"],
                            Surname = (row["Surname"] == DBNull.Value) ? string.Empty : (string)row["Surname"],
                            Title = (row["Title"] == DBNull.Value) ? string.Empty : (string)row["Title"],
                            Position = (row["Position"] == DBNull.Value) ? string.Empty : (string)row["Position"],
                            PhotoAddress = (row["PhotoAddress"] == DBNull.Value) ? string.Empty : (string)row["PhotoAddress"],
                            CardID = (row["CardID"] == DBNull.Value) ? string.Empty : (string)row["CardID"],
                            DateTime = (row["DateTime"] == DBNull.Value || row["DateTime"].ToString() == "") ? DateTime.Now : DateTime.Parse(row["DateTime"].ToString())
                        };
                    }

                }
            }
            return user;
        }

        /// <summary>
        /// SQLite Get All Records
        /// </summary>
        /// <returns></returns>
        public List<User> ListUsers()
        {
            List<User> dbUserList = new List<User>();
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Users order by Id desc";
                    SQLiteDataReader row = command.ExecuteReader();
                    while (row.Read())
                    {
                        dbUserList.Add(new User
                        {
                            User_ID = (long)row["User_ID"],
                            UserName = (string)row["UserName"],
                            UserPassword = core.CryptorEngine.Decrypt((string)row["UserPassword"], true),
                            SuperUser = Convert.ToInt32(row["SuperUser"]) == 0 ? false : true,
                            Enable = Convert.ToInt32(row["Enable"]) == 0 ? false : true,
                            Authorization = Convert.ToInt32(row["Authorization"]),
                            Email = (row["Email"] == DBNull.Value) ? string.Empty : (string)row["Email"],
                            Name = (row["Name"] == DBNull.Value) ? string.Empty : (string)row["Name"],
                            SecondName = (row["SecondName"] == DBNull.Value) ? string.Empty : (string)row["SecondName"],
                            Surname = (row["Surname"] == DBNull.Value) ? string.Empty : (string)row["Surname"],
                            Title = (row["Title"] == DBNull.Value) ? string.Empty : (string)row["Title"],
                            Position = (row["Position"] == DBNull.Value) ? string.Empty : (string)row["Position"],
                            PhotoAddress = (row["PhotoAddress"] == DBNull.Value) ? string.Empty : (string)row["PhotoAddress"],
                            CardID = (row["CardID"] == DBNull.Value) ? string.Empty : (string)row["CardID"],
                            DateTime = (row["DateTime"] == DBNull.Value || row["DateTime"].ToString() == "") ? DateTime.Now : DateTime.Parse(row["DateTime"].ToString())
                        });
                    }
                }
                con.Close();
            }
            return dbUserList;
        }

        /// <summary>
        /// SQLite Get Records by Authorization Level
        /// </summary>
        /// <returns></returns>
        public List<User> ListUsers(int authorizationLevel)
        {
            List<User> dbUserList = new List<User>();
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "select * from Users where Authotization >= " + authorizationLevel + " order by Id desc";
                    SQLiteDataReader row = command.ExecuteReader();
                    while (row.Read())
                    {
                        dbUserList.Add(new User
                        {
                            User_ID = (long)row["User_ID"],
                            UserName = (string)row["UserName"],
                            UserPassword = core.CryptorEngine.Decrypt((string)row["UserPassword"], true),
                            SuperUser = Convert.ToInt32(row["SuperUser"]) == 0 ? false : true,
                            Enable = Convert.ToInt32(row["Enable"]) == 0 ? false : true,
                            Authorization = Convert.ToInt32(row["Authorization"]),
                            Email = (row["Email"] == DBNull.Value) ? string.Empty : (string)row["Email"],
                            Name = (row["Name"] == DBNull.Value) ? string.Empty : (string)row["Name"],
                            SecondName = (row["SecondName"] == DBNull.Value) ? string.Empty : (string)row["SecondName"],
                            Surname = (row["Surname"] == DBNull.Value) ? string.Empty : (string)row["Surname"],
                            Title = (row["Title"] == DBNull.Value) ? string.Empty : (string)row["Title"],
                            Position = (row["Position"] == DBNull.Value) ? string.Empty : (string)row["Position"],
                            PhotoAddress = (row["PhotoAddress"] == DBNull.Value) ? string.Empty : (string)row["PhotoAddress"],
                            CardID = (row["CardID"] == DBNull.Value) ? string.Empty : (string)row["CardID"],
                            DateTime = (row["DateTime"] == DBNull.Value || row["DateTime"].ToString() == "") ? DateTime.Now : DateTime.Parse(row["DateTime"].ToString())
                        });
                    }
                }
                con.Close();
            }
            return dbUserList;
        }

        /// <summary>
        /// SQLite Insert Record
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public sbyte AddUser(ref User data)
        {

            try
            {
                if (IsThereRecordbyUserName(data.UserName))
                {
                    return -1; // Same User Name Already Exist
                }

                if (data.CardID != null && data.CardID.Length > 0 && IsThereRecordbyCardID(data.CardID))
                {
                    return -2; // Same Card Already Exist
                }

                using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();

                    using (SQLiteCommand command = new SQLiteCommand(con))
                    {
                        command.CommandText = "insert into Users (UserName,UserPassword,SuperUser,Enable,Authorization,Email,Name,SecondName,Surname,Title,Position,PhotoAddress,CardID,DateTime) " +
                            "values (@UserName,@UserPassword,@SuperUser,@Enable,@Authorization,@Email,@Name,@SecondName,@Surname,@Title,@Position,@PhotoAddress,@CardID,@DateTime); " +
                            "select last_insert_rowid();";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SQLiteParameter("@UserName", data.UserName));
                        command.Parameters.Add(new SQLiteParameter("@UserPassword", core.CryptorEngine.Encrypt(data.UserPassword, true)));
                        command.Parameters.Add(new SQLiteParameter("@SuperUser", data.SuperUser));
                        command.Parameters.Add(new SQLiteParameter("@Enable", data.Enable));
                        command.Parameters.Add(new SQLiteParameter("@Authorization", data.Authorization));
                        command.Parameters.Add(new SQLiteParameter("@Email", data.Email));
                        command.Parameters.Add(new SQLiteParameter("@Name", data.Name));
                        command.Parameters.Add(new SQLiteParameter("@SecondName", data.SecondName));
                        command.Parameters.Add(new SQLiteParameter("@Surname", data.Surname));
                        command.Parameters.Add(new SQLiteParameter("@Title", data.Title));
                        command.Parameters.Add(new SQLiteParameter("@Position", data.Position));
                        command.Parameters.Add(new SQLiteParameter("@PhotoAddress", data.PhotoAddress));
                        command.Parameters.Add(new SQLiteParameter("@CardID", data.CardID));
                        command.Parameters.Add(new SQLiteParameter("@DateTime", data.DateTime));
                        object obj = command.ExecuteScalar();
                        data.User_ID = (long)obj;
                    }

                    con.Close();

                    return 1; // Successful
                }
            }
            catch (Exception)
            {
                return 0;// Error
            }

        }

        /// <summary>
        /// SQLite Insert Record - Predefined User
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public sbyte AddUser()
        {
            User user = new User
            {
                Authorization = 3, //Administrator
                Enable = true,
                SuperUser = true,
                UserName = "admin",
                UserPassword = "admin", 
                DateTime = DateTime.Now
            };

            return AddUser(ref user);
        }

        /// <summary>
        /// SQLite Delete Record
        /// </summary>
        /// <param name="User_ID"></param>
        /// <returns>-1 an Error</returns>
        public sbyte DeleteUser(long User_ID)
        {
            sbyte status;
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = "delete from Users where User_ID=" + User_ID.ToString() + "";
                    status = (sbyte)command.ExecuteNonQuery();
                }
                con.Close();
            }
            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>-1 an Error</returns>
        public sbyte UpdateUser(ref User user, long activeUserID)
        {
            sbyte status;

            try
            {
                if (!IsThereRecordByID(user.User_ID))
                {
                    return -1; // User not found
                }


                if (user.CardID.Length > 0 && IsThereRecordbyCardID(user))
                {
                    return -2; // Same Card Already Exist
                }

                using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();

                    using (SQLiteCommand command = new SQLiteCommand(con))
                    {
                        command.CommandText = "update Users set UserName=@UserName, UserPassword=@UserPassword, SuperUser=@SuperUser, Enable=@Enable, " +
                            "Authorization=@Authorization, Email=@Email, Name=@Name, SecondName=@SecondName, Surname=@Surname, Title=@Title, Position=@Position," +
                            "PhotoAddress=@PhotoAddress, CardID=@CardID , DateTime=@DateTime where User_ID=@User_ID; " + "";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SQLiteParameter("@User_ID", user.User_ID));
                        command.Parameters.AddWithValue("@UserName", user.UserName);
                        command.Parameters.AddWithValue("@UserPassword", core.CryptorEngine.Encrypt(user.UserPassword,true));
                        command.Parameters.AddWithValue("@SuperUser", user.SuperUser ? 1 : 0);
                        command.Parameters.AddWithValue("@Enable", user.Enable ? 1 : 0);
                        command.Parameters.AddWithValue("@Authorization", user.Authorization);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@SecondName", user.SecondName);
                        command.Parameters.AddWithValue("@Surname", user.Surname);
                        command.Parameters.AddWithValue("@Title", user.Email);
                        command.Parameters.AddWithValue("@Position", user.Position);
                        command.Parameters.AddWithValue("@PhotoAddress", user.PhotoAddress);
                        command.Parameters.AddWithValue("@CardID", user.CardID);
                        command.Parameters.AddWithValue("@DateTime", user.DateTime.ToString());
                        status = (sbyte)command.ExecuteNonQuery();
                    }

                    con.Close();
                }

                if (user.User_ID == activeUserID)
                {
                    ActiveUser = user;
                }

                return 1; // Successful
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return 0;// Exception
            }

        }

        /// <summary>
        /// SignIn Wiht UserName And Password
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="UserPassword"></param>
        /// <returns></returns>
        public bool SignInWithPassword(string UserName, string UserPassword)
        {

            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "select * from Users where UserName=@UserName and UserPassword=@UserPassword";

                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@UserPassword", core.CryptorEngine.Encrypt(UserPassword, true));

                    using (SQLiteDataReader row = command.ExecuteReader())
                    {
                        if (row.Read())
                        {
                            ActiveUser = new User
                            {
                                User_ID = (long)row["User_ID"],
                                UserName = (string)row["UserName"],
                                UserPassword = core.CryptorEngine.Decrypt((string)row["UserPassword"],true),
                                SuperUser = Convert.ToInt32(row["SuperUser"]) == 0 ? false : true,
                                Enable = Convert.ToInt32(row["Enable"]) == 0 ? false : true,
                                Authorization = Convert.ToInt32(row["Authorization"]),
                                Email = (row["Email"] == DBNull.Value) ? string.Empty : (string)row["Email"],
                                Name = (row["Name"] == DBNull.Value) ? string.Empty : (string)row["Name"],
                                SecondName = (row["SecondName"] == DBNull.Value) ? string.Empty : (string)row["SecondName"],
                                Surname = (row["Surname"] == DBNull.Value) ? string.Empty : (string)row["Surname"],
                                Title = (row["Title"] == DBNull.Value) ? string.Empty : (string)row["Title"],
                                Position = (row["Position"] == DBNull.Value) ? string.Empty : (string)row["Position"],
                                PhotoAddress = (row["PhotoAddress"] == DBNull.Value) ? string.Empty : (string)row["PhotoAddress"],
                                CardID = (row["CardID"] == DBNull.Value) ? string.Empty : (string)row["CardID"],
                                DateTime = (row["DateTime"] == DBNull.Value || row["DateTime"].ToString() == "") ? DateTime.Now : DateTime.Parse(row["DateTime"].ToString())
                            };

                            return true;
                        }
                    }

                    ActiveUser = null;
                }
                con.Close();
            }
            return false;
        }

        /// <summary>
        /// SignIn Wiht UserName And Password
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public bool SignInWithCardID(string CardID)
        {
            using (SQLiteConnection con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (SQLiteCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Users WHERE CardID=@CardID";

                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@CardID", CardID);

                    using (SQLiteDataReader row = command.ExecuteReader())
                    {
                        if (row.Read())
                        {
                            ActiveUser = new User
                            {
                                User_ID = (long)row["User_ID"],
                                UserName = (string)row["UserName"],
                                UserPassword = core.CryptorEngine.Decrypt((string)row["UserPassword"],true),
                                SuperUser = Convert.ToInt32(row["SuperUser"]) == 0 ? false : true,
                                Enable = Convert.ToInt32(row["Enable"]) == 0 ? false : true,
                                Authorization = Convert.ToInt32(row["Authorization"]),
                                Email = (row["Email"] == DBNull.Value) ? string.Empty : (string)row["Email"],
                                Name = (row["Name"] == DBNull.Value) ? string.Empty : (string)row["Name"],
                                SecondName = (row["SecondName"] == DBNull.Value) ? string.Empty : (string)row["SecondName"],
                                Surname = (row["Surname"] == DBNull.Value) ? string.Empty : (string)row["Surname"],
                                Title = (row["Title"] == DBNull.Value) ? string.Empty : (string)row["Title"],
                                Position = (row["Position"] == DBNull.Value) ? string.Empty : (string)row["Position"],
                                PhotoAddress = (row["PhotoAddress"] == DBNull.Value) ? string.Empty : (string)row["PhotoAddress"],
                                CardID = (row["CardID"] == DBNull.Value) ? string.Empty : (string)row["CardID"],
                                DateTime = (row["DateTime"] == DBNull.Value || row["DateTime"].ToString() == "") ? DateTime.Now : DateTime.Parse(row["DateTime"].ToString())
                            };

                            return true;
                        }
                    }

                    ActiveUser = null;
                }
                con.Close();
            }
            return false;
        }

        void IDisposable.Dispose()
        {

        }
    }

    public static class DBStatic
    {
        public static DB db = new DB("Users");
    }
}
