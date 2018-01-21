using UnityEngine;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Security.Cryptography;

public class DataBaseConnection : Singleton<DataBaseConnection>
{
    //Don't know why but this script must be in main folder :-(

    [SerializeField] string localHost;
    [SerializeField] string user;
    [SerializeField] string password;
    [SerializeField] string dateBaseName ;

    MySqlConnection mySQLconnection;
    MySqlCommand mySQLcommand;
    MySqlDataReader mySQLreader;

    public bool debugMode;

    static DataBaseConnection dataBaseConnectionInstance;

    int currentLoginID;
   // public int CurrentLoginID { get { return currentLoginID; } }

    private void Awake()
    {
        if(debugMode)
        {
            Debug.Log("debug mode is on, your LoginID = 1");
            currentLoginID = 1;
        }

        DontDestroyOnLoad(this);
        if (dataBaseConnectionInstance == null)
        {
            dataBaseConnectionInstance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }

    public List<string> GetProfessionsName()
    {
        List<string> professionList = new List<string>();
        if (Connect())
        {
            string command = "select Nazwa from Profesja";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();
            while (mySQLreader.Read())
            {
                professionList.Add(mySQLreader["Nazwa"].ToString());
                //Debug.Log(mySQLreader["Nazwa"].ToString());
            }
            mySQLreader.Close();
            mySQLconnection.Close();
        }
        else
        {
            Debug.Log("database connection interrupt");
        }
        return professionList;
    }

    [ContextMenu("CreateEntity()")]
    void CreateEntity()
    {
        CreateEntity("SUPERPOSTAC", 8);
    }

    public int GetProfessionID (string professionName)
    {
        if (Connect())
        {
            string command = "Select IDProfesji from Profesja where Nazwa = '" + professionName + "';";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int professionID = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return professionID;
        }
        else
        {
            return -1;
        }
    }

    int CheckAvailabilityEntityName(string name)
    {
        if (Connect())
        {
            string command = "Select ImiePostaci from Postacie where ImiePostaci = '" + name + "';";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);

            mySQLreader = mySQLcommand.ExecuteReader();
            while (mySQLreader.Read())
            {
                //Debug.Log(mySQLreader["ImiePostaci"].ToString());
                return 0;
            }
            mySQLreader.Close();
            mySQLconnection.Close();
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public int CreateEntity(string name, int professionID)
    {
        int checkEntityResult = CheckAvailabilityEntityName(name);

        if (checkEntityResult ==0)
        {
            return 0;
        }
        else if (checkEntityResult == -1)
        {
            return -1;
        }
        else
        {
            if (Connect())
            {
                string command = "Select PoczatkoweZycie,PoczatkowaSila from Profesja where IDprofesji = " + professionID + ";";
                mySQLcommand = new MySqlCommand(command, mySQLconnection);
                mySQLreader = mySQLcommand.ExecuteReader();

                int poczatkoweZycie = 0;
                int poczatkowaSila = 0;

                while (mySQLreader.Read())
                {
                    poczatkoweZycie = System.Convert.ToInt32(mySQLreader["PoczatkoweZycie"]);
                    poczatkowaSila = System.Convert.ToInt32(mySQLreader["PoczatkowaSila"]);
                }

                mySQLreader.Close();

                command = "INSERT INTO Postacie (IDuzytkownika,ImiePostaci,IDProfesji,Sila,Zycie)" +
                                        "VALUES('" + currentLoginID + "', '" + name + "', '" + professionID + "', '" + poczatkowaSila + "', '" + poczatkoweZycie + "');";

                mySQLcommand = new MySqlCommand(command, mySQLconnection);
                mySQLcommand.ExecuteNonQuery();

                mySQLconnection.Close();
                return 1;
            }
            else
            {
                Debug.Log("database connection interrupt");
                return -1;
            }
        }
    }


    public void LoginValidate(string login, string password)
    {
        password = HashingMD5(password);

        if (CheckExistingPlayer(login) == 1)
        {
            if (CheckPasswordCorrect(login, password) == 1)
            {               
                int loginID = GetLoginID(login);
                if (loginID != -1)
                {                    
                    ShowStatement("Logging, please wait!");
                    Logged(loginID);
                }
                else
                {
                    Debug.Log("database interrupt");
                }
            }
            else if (CheckPasswordCorrect(login, password) == 0)
            {
                ShowStatement("Password is incorrect!");
            }
            else
            {
                Debug.Log("database interrupt");
            }
        }
        else if (CheckExistingPlayer(login) == 0)
        {
            ShowStatement("Player doesn't exists!");
        }
        else
        {
            Debug.Log("database interrupt");
            ShowStatement("database is not connected");
        } 
    }

    void Logged(int loginID)
    {
        currentLoginID = loginID;

        var loggedCmd = new LoggedCmd();
        loggedCmd.LoginID = loginID;
        Mediator.Instance.Publish<LoggedCmd>(loggedCmd);
    }

    /// <summary>
    /// <para> return -1 if database connection is interrupt </para>
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    int GetLoginID(string login)
    {
        if (Connect())
        {
            string command = "SELECT ID from Uzytkownik where Login = '" + login + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int getLoginID = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return getLoginID;           
        }
        return -1;
    }

    /// <summary>
    /// <para>return -1 if database connection is interrupt</para> 
    /// <para>return 0 if password is incorrect</para>
    /// <para>return 1 if password is correct</para>
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    int CheckPasswordCorrect(string login, string password)
    {
        if (Connect())
        {
            string command = "SELECT Haslo from Uzytkownik where Login = '" + login + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            string getPassword = System.Convert.ToString(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            if (password == getPassword)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        return -1;
    }

    bool Connect()
    {       
        mySQLconnection = new MySqlConnection("server=" + localHost + "; user=" + user + "; database=" + dateBaseName + ";");
        try
        {
            mySQLconnection.Open();
        }
        catch (System.Exception e)
        {
            //Debug.Log("Connection state is closed");
            return false;
        }
        if (mySQLconnection.State == System.Data.ConnectionState.Open)
        {
            return true;
        }
        return false; //?
    }

    void AddAccount(string login, string password)
    {
        if (CheckExistingPlayer(login) == 0)
        {
            if (Connect())
            {
                password = HashingMD5(password);
                string command = "INSERT INTO `uzytkownik` (`Login`, `Haslo`) VALUES('" + login + "', '" + password + "');";
                mySQLcommand = new MySqlCommand(command, mySQLconnection);
                mySQLcommand.ExecuteNonQuery();
                Debug.Log("New user added");
                ShowStatement("New user added");
                mySQLconnection.Close();
            }
            else
            {
                Debug.Log("database interrupt");
            }
        }
        else if (CheckExistingPlayer(login) == 1)
        {
            Debug.Log("this user can't be added");
            ShowStatement("this user already exsits");
        }
        else
        {
            Debug.Log("database interrupt");
            ShowStatement("database is not connected");
        }       
    }

    public string[] EntitiesName(ref string[] entities)
    {
        if (Connect())
        {
            string command = "select ImiePostaci from Postacie where IDUzytkownika = '" + currentLoginID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();

            int i = 0;
            while (mySQLreader.Read())
            {
                entities[i++] = mySQLreader["ImiePostaci"].ToString();            
            }
            mySQLreader.Close();
            mySQLconnection.Close();
            return entities;
        }
        else
        {
            return entities;
        }
    }

    public int EntityQuantity() 
    {
        if (Connect())
        {
            string command = "SELECT Count(IDPostaci) from Postacie where IDuzytkownika = '" + currentLoginID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int number = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return number;
        }
        else
        {
            return -1;
        }
    }

    string HashingMD5(string password)
    {
        string salt = "dataBazych";
        password += salt;
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] bytes = md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(password));
        string result = System.BitConverter.ToString(bytes).Replace("-", System.String.Empty);
        return result;
    }

    /// <summary>
    /// <para> return -1 if database connection is interrupt </para>
    /// <para> return 0 if login doesn't exist </para>
    /// <para> return 1 if login exists </para>
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    int CheckExistingPlayer(string login)
    {
        if (Connect())
        {
            string command = "SELECT Count(Login) from Uzytkownik where Login = '" + login + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int number = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            if (number == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return -1;
        }
    }

    void DeletePlayer()
    {
        // TODO
    }

    void UpdatePlayer()
    {
        // TODO 
    }

    public void Register (string login, string password)
    {
        AddAccount(login, password);
    }

    void ShowStatement(string statement)
    {
        var cmd = new ShowStatementCmd();
        cmd.Statement = statement;
        Mediator.Instance.Publish<ShowStatementCmd>(cmd);
    }



    [ContextMenu("ShowPlayers()")]
    void ShowPlayers()
    {
        if (Connect())
        {
            string command = "select * from Uzytkownik";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();
            while (mySQLreader.Read())
            {
                Debug.Log(mySQLreader["ID"].ToString() + " " + mySQLreader["Login"].ToString());
            }
            mySQLreader.Close();
            mySQLconnection.Close();
        }
        else
        {
            Debug.Log("database connection is interrupt");
        }
    }

    [ContextMenu("AddRandomPlayerTest()")]
    void AddRandomPlayerTest()
    {
        string name = System.Guid.NewGuid().ToString();
        name = name.Substring(0, 30);
        Debug.Log("try to add player - " + name);
        AddAccount(name, "haslo");
    }

    [ContextMenu("AddTheSamePlayerTest()")]
    void AddTheSamePlayerTest()
    {
        string name = "Mac";
        Debug.Log("try to add player - " + name);
        AddAccount(name, "haslo");
    }

    [ContextMenu("HashingMD5TEST()")]
    void HashingMD5TEST()
    {
        string test = HashingMD5("Mac");
        Debug.Log(test);
    }

}
