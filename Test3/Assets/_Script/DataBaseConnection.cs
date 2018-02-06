using UnityEngine;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

public class DataBaseConnection : Singleton<DataBaseConnection>
{
    //Don't know why but this script must be in main folder :-(

    [SerializeField] string localHost = "localhost";
    [SerializeField] string user = "root";
    [SerializeField] string password;
    [SerializeField] string dateBaseName = "supergierka" ;

    MySqlConnection mySQLconnection;
    MySqlCommand mySQLcommand;
    MySqlDataReader mySQLreader;

    public bool debugMode;

    static DataBaseConnection dataBaseConnectionInstance;

    int currentLoginID;
    int currentEntityID;

    public int CurrentEntityID { get { return currentEntityID; } set { currentEntityID = value; } }

    private void Awake()
    {
        if(debugMode)
        {
            Debug.Log("debug mode is on, your LoginID = 1");
            currentLoginID = 1;

            Debug.Log("debug mode is on, your currentEntityID = 101");
            currentEntityID = 101;
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
        name = name.Trim();
        Debug.Log(name);
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

                 command = "INSERT INTO Postacie (IDuzytkownika,ImiePostaci,IDProfesji,Zycie,Sila,Poziom,Doswiadczenie)" +
                                        "VALUES('" + currentLoginID + "', '" + name + "', '" + professionID + "', '" + poczatkoweZycie + "', '" + poczatkowaSila + "','1','0');";                 

                Debug.Log(command);

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

    public void EntitiesName(ref string[] entities)
    {
        Array.Clear(entities, 0, entities.Length);
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
        }
    }

    public void GetTotalityStatistics (int entityID, ref List<string> totalityStatistics)
    {
        totalityStatistics.Clear();

        if (Connect())
        {
            totalityStatistics.Add (GetEntityTotalityPower(entityID).ToString() + " Sila");
            totalityStatistics.Add (GetEntityTotalityHealth(entityID).ToString() + " Zycie");
            mySQLconnection.Close();
        }
    }

    public int AddExp(double exp)
    {
        if(Connect())
        {          
            double totalityExp = GetExp() + exp;
            string command = "UPDATE `postacie` SET `Doswiadczenie` = '" + totalityExp + "' WHERE `postacie`.`IDPostaci` = "+currentEntityID+";";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLcommand.ExecuteNonQuery();

            CheckLevelUp(totalityExp);
            mySQLconnection.Close();
            return 1;
        }
        else
        {
            return -1;
        }
    }

    void CheckLevelUp(double exp)
    {        
        do
        {
            int currentLevel = GetEntityLevel();
            double neededExp = Player.ExpToNextLevel(currentLevel);
            if (exp > neededExp)
            {
                LevelUp(currentLevel);
            }
            else
            {
                break;
            }
        } while (true);
    }

    void LevelUp(int currentLevel)
    {
        string command = "UPDATE `postacie` SET `Poziom` = '" + (currentLevel+1) + "' WHERE `postacie`.`IDPostaci` = " + currentEntityID + ";";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        mySQLcommand.ExecuteNonQuery();
        IncreaseStatistics();
    }

    void IncreaseStatistics()
    {
        IncreaseHealth();
        IncreasePower();       
    }

    void IncreaseHealth()
    {
        int hp = (GetEntityHealth() + 10);
        string command = "UPDATE `postacie` SET `Zycie` = '" + hp + "' WHERE `postacie`.`IDPostaci` = " + currentEntityID + ";";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        mySQLconnection.Open(); // ???
        mySQLcommand.ExecuteNonQuery();
    }

    void IncreasePower()
    {
        string command = "UPDATE `postacie` SET `Sila` = '" + (GetEntityPower() + 10) + "' WHERE `postacie`.`IDPostaci` = " + currentEntityID + ";";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        mySQLconnection.Open(); // ???
        mySQLcommand.ExecuteNonQuery();
    }

    int GetEntityLevel()
    {
        string command = "SELECT Poziom From Postacie WHERE IDPostaci = '" + currentEntityID + "'";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        int lvl = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
        return lvl;
    }

    double GetExp()
    {
        string command = "SELECT Doswiadczenie From Postacie WHERE IDPostaci = '" + currentEntityID + "'";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        double exp = System.Convert.ToDouble(mySQLcommand.ExecuteScalar());
        return exp;
    }

    public int GetEntityTotalityPower()
    {
        if (Connect())
        {
            int power = GetEntityTotalityPower(currentEntityID); 
            mySQLconnection.Close();
            return power;
        }
        else
        {
            return -1;
        }
    }

    public int GetEntityTotalityHealth()
    {
        if (Connect())
        {
            int hp = GetEntityTotalityHealth(currentEntityID);
            mySQLconnection.Close();
            return hp;
        }
        else
        {
            return -1;
        }
    }

    int GetEntityTotalityPower(int entityID)
    {
        int power = EntityPower(entityID) + EntityWeaponsPower(entityID);
        return power;
    }

    int EntityPower(int entityID)
    {
        string command = "SELECT Sila From Postacie WHERE IDPostaci = '" + entityID + "'";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        int power = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
        return power;
    }

    int EntityWeaponsPower(int entityID)
    {
        string command  = "SELECT Sum(Sila) From Przedmioty p inner join przedmiotypostaci pp on p.IDPrzedmiotu = pp.IDPrzedmiotu  WHERE pp.IDPostaci = '" + entityID + "' GROUP by p.Sila ";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        int power = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
        return power;
    }

    int GetEntityTotalityHealth(int entityID)
    {
        int health = EntityHealth(entityID) + EntityWeaponsHealth(entityID);
        return health;
    }

    public int GetEntityWeaponsPower()
    {
        if (Connect())
        {
            int power = EntityWeaponsPower(currentEntityID);
            mySQLconnection.Close();
            return power;
        }
        else
        {
            return -1;
        }
    }

    public int GetRandomEnemyID()
    {
        if (Connect())
        {
            List<int> monstersID = new List<int>();
            string command = "select IDPotwora from Potwory";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();

            while (mySQLreader.Read())
            {
                monstersID.Add(System.Convert.ToInt32(mySQLreader["IDPotwora"]));
              //  Debug.Log(mySQLreader["IDPotwora"]);
            }
            mySQLreader.Close();
            int rand = UnityEngine.Random.Range(0, monstersID.Count);            
            mySQLconnection.Close();
            return monstersID[rand];
        }
        else
        {
            return -1;
        }
    }

    public int DeleteEntity(int entityID)
    {
        if(Connect())
        {
            DeleteFinishedAdventures(entityID);
            DeleteEntitysWeapons(entityID);
            DeleteSelectedEntity(entityID);
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public int _DeleteEntity(int entityID)
    {
        if (Connect())
        {
            DeleteFinishedAdventures(entityID);
            DeleteEntitysWeapons(entityID);
            DeleteSelectedEntity(entityID);
            return 1;
        }
        else
        {
            return -1;
        }
    }

    void DeleteSelectedEntity(int entityID)
    {
        string command = "DELETE FROM `Postacie` WHERE IDPostaci = '" + entityID + "';";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        mySQLcommand.ExecuteNonQuery();
    }

    void DeleteFinishedAdventures(int entityID)
    {
        string command = "DELETE FROM `WykonanePrzygody` WHERE IDPostaci = '" + entityID + "';";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        mySQLcommand.ExecuteNonQuery();
    }

    void DeleteEntitysWeapons(int entityID)
    {
        string command = "DELETE FROM `PrzedmiotyPostaci` WHERE IDPostaci = '" + entityID + "';";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        mySQLcommand.ExecuteNonQuery();
    }


    int EntityHealth(int entityID)
    {
        string command = "SELECT Zycie From Postacie WHERE IDPostaci = '" + entityID + "'";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        int health = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
        return health;
    }

    int EntityWeaponsHealth(int entityID)
    {
        string command = "SELECT Sum(Zycie) From Przedmioty p inner join przedmiotypostaci pp on p.IDPrzedmiotu = pp.IDPrzedmiotu  WHERE pp.IDPostaci = '" + entityID + "' GROUP by p.Sila ";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        int health = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
        return health;
    }

    public int GetEntityWeaponsHealth()
    {
        if (Connect())
        {
            int hp = EntityWeaponsHealth(currentEntityID);
            mySQLconnection.Close();
            return hp;
        }
        else
        {
            return -1;
        }
    }

    public int GetAdventureID(string title)
    {
        if (Connect())
        {
            string command = "SELECT IDPrzygody From Przygody WHERE Tytul = '" + title + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int id = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return id;
        }
        else
        {
            return -1;
        }
    }

    public int GetEntityID(string entityName)
    {
        if (Connect())
        {
            string command = "SELECT IDPostaci from Postacie where ImiePostaci = '" + entityName + "'";
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

    public int GetAdventureRequiredLevel(string title)
    {
        if (Connect())
        {
            string command = "SELECT WymaganyPoziom FROM Przygody WHERE Tytul = '" + title + "'";
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

    public string GetAdventureDescripton(string title)
    {
        if (Connect())
        {
            string descripton="";
            string command = "select Opis from Przygody p where tytul= '" + title + "'";
           // Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();

            while (mySQLreader.Read())
            {
                descripton = mySQLreader["Opis"].ToString();
            }
            mySQLreader.Close();
            mySQLconnection.Close();
            return descripton;
        }
        else
        {
            return "database error";
        }
    }

    public string GetMonsterName(int id)
    {
        if (Connect())
        {
            string command = "SELECT Nazwa from Potwory where IDPotwora = '" + id + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            string name = System.Convert.ToString(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return name;
        }
        else
        {
            return "DB interrupt";
        }
    }

    string _GetMonsterName(int id)
    {
        string command = "select Nazwa from Potwory where IDPotwora = '" + id + "'";
        //  Debug.Log(command);
        string name="";
        mySQLcommand = new MySqlCommand(command, mySQLconnection);
        mySQLreader = mySQLcommand.ExecuteReader();

        while (mySQLreader.Read())
        {
            name = mySQLreader["Nazwa"].ToString();
        }
        mySQLreader.Close();
        return name;
    }

    public void GetAdventureMonsters(ref List<string>monsters, string title)
    {
        monsters.Clear();
        if (Connect())
        {           
            string command = "select Potwory from Przygody where Tytul = '" + title + "'";
       //     Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();
     
            string tmp="";
            int quantity;
            int monsterID;
            string monster;
            while (mySQLreader.Read())
            {
                tmp = mySQLreader["Potwory"].ToString();
            }
            mySQLreader.Close();
            try
            {
                int i = 0;
                do
                {
                    i = tmp.IndexOf(",");
                    if (i == -1)
                    {
                        quantity = Int32.Parse(tmp.Substring(0, tmp.IndexOf(" ")));
                        monsterID = Int32.Parse(tmp.Substring(tmp.IndexOf(" ") + 1));
                    }
                    else
                    {
                        quantity = Int32.Parse(tmp.Substring(0, tmp.IndexOf(" ")));
                        monsterID = Int32.Parse(tmp.Substring(tmp.IndexOf(" ") - 1, tmp.IndexOf(",") - 1));
                        tmp = tmp.Substring(tmp.IndexOf(",") + 1);

                    }
                    
                    monsters.Add(quantity + "x " + _GetMonsterName(monsterID));

                } while (i != -1);
            }
            catch (Exception e)
            {
                Debug.Log("something in DB is wrong!");
            }       
        }
    }

    public int GetCurrentLevel()
    {
        if (Connect())
        {
            string command = "SELECT Poziom from Postacie where IDPostaci = '" + currentEntityID + "'";
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

    public string GetAdventurePrize(string title)
    {
        if (Connect())
        {
            string prize = "";
            string command = "select Nazwa from Przedmioty p inner join Przygody przy ON przy.IDPrzedmiotuZaNagrode = p.IDPrzedmiotu where przy.tytul= '" + title + "'";
          //  Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();

            while (mySQLreader.Read())
            {
                prize = mySQLreader["Nazwa"].ToString();
            }
            mySQLreader.Close();
            mySQLconnection.Close();
            return prize;
        }
        else
        {
            return "database error";
        }
    }

    public int GetAdventurePrize(int adventureID)
    {
        if (Connect())
        {
            string command = "select IDPrzedmiotu from Przedmioty p inner join Przygody przy ON przy.IDPrzedmiotuZaNagrode = p.IDPrzedmiotu where przy.IDPrzygody= '" + adventureID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int prizeID = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return prizeID;
        }
        else
        {
            return -1;
        }
    }

    public int AddWeapon(int weaponID)
    {
        if (Connect())
        {
            string command = "INSERT INTO `PrzedmiotyPostaci` (`IDPostaci`,`IDPrzedmiotu`) VALUES ('" + currentEntityID + "', '" + weaponID + "')";
           // Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLcommand.ExecuteNonQuery();
            mySQLconnection.Close();
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public int SetFinishedAdventage(int adventureID)
    {
        if (Connect())
        {
            string command = "INSERT INTO `wykonaneprzygody` (`IDPostaci`,`IDPrzygody`) VALUES ('" + currentEntityID + "', '" + adventureID + "')";
         //   Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLcommand.ExecuteNonQuery();
            mySQLconnection.Close();
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public void GetFinishedAdventureTitle(ref List<string> titles)
    {
        titles.Clear();
        if (Connect())
        {
            string command = "select Tytul from Przygody p inner join WykonanePrzygody wp ON p.IDPrzygody = wp.IDPrzygody where IDPostaci = '" + currentEntityID + "'";
            //Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();

            while (mySQLreader.Read())
            {
                titles.Add(mySQLreader["Tytul"].ToString());
            }
            mySQLreader.Close();
            mySQLconnection.Close();
        }
        else
        {
            titles.Add("database error");
        }
    }


    public void GetUnfinishedAdventureTitle(ref List<string> titles)
    {
        titles.Clear();
        if (Connect())
        {
            string command = "select Tytul from Przygody where IDPrzygody NOT IN ( select IDPrzygody from WykonanePrzygody where IDPostaci = '" + currentEntityID + "')";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();

            while (mySQLreader.Read())
            {
                titles.Add(mySQLreader["Tytul"].ToString());              
            }
            mySQLreader.Close();
            mySQLconnection.Close();
        }
        else
        {
            titles.Add("database error");
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

    public int GetAdventureExp(int adventureID)
    {
        if (Connect())
        {
            string command = "SELECT IloscExp from Przygody where IDPrzygody = '" + adventureID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int exp = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return exp;
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
        login.Trim();
        AddAccount(login, password);
    }

    void ShowStatement(string statement)
    {
        var cmd = new ShowStatementCmd();
        cmd.Statement = statement;
        Mediator.Instance.Publish<ShowStatementCmd>(cmd);
    }

    public int GetMonsterPower(string name)
    {
        if (Connect())
        {
            string command = "SELECT Sila from Potwory where Nazwa = '" + name + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int power = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return power;
        }
        else
        {
            return -1;
        }
    }

    public int GetMonsterPower(int id)
    {
        if (Connect())
        {
            string command = "SELECT Sila from Potwory where IDPotwora = '" + id + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int power = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return power;
        }
        else
        {
            return -1;
        }
    }

    public double GetMonsterExp(string name)
    {
        if (Connect())
        {
            string command = "SELECT Doswiadczenie from Potwory where Nazwa = '" + name + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            double exp = System.Convert.ToDouble(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return exp;
        }
        else
        {
            return -1;
        }
    }

    public double GetMonsterExp(int id)
    {
        if (Connect())
        {
            string command = "SELECT Doswiadczenie from Potwory where IDPotwora = '" + id + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            double exp = System.Convert.ToDouble(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return exp;
        }
        else
        {
            return -1;
        }
    }

    public int GetMonsterHealth(string name)
    {
        if (Connect())
        {
            string command = "SELECT Zycie from Potwory where Nazwa = '" + name + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int health = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return health;
        }
        else
        {
            return -1;
        }
    }



    public int GetMonsterHealth(int id)
    {
        if (Connect())
        {
            string command = "SELECT Zycie from Potwory where IDPotwora = '" + id + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int health = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return health;
        }
        else
        {
            return -1;
        }
    }

    public int GetEntityHealth()
    {
        if (Connect())
        {
            string command = "SELECT Zycie from Postacie where IDPostaci = '" + currentEntityID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int health = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return health;
        }
        else
        {
            return -1;
        }
    }

    public int GetWeaponHealth(int weaponID)
    {
        if (Connect())
        {
            string command = "SELECT Zycie from Przedmioty where IDPrzedmiotu = '" + weaponID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int health = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return health;
        }
        else
        {
            return -1;
        }
    }

    public int GetEntityPower()
    {
        if (Connect())
        {
            string command = "SELECT Sila from Postacie where IDPostaci = '" + currentEntityID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int power = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return power;
        }
        else
        {
            return -1;
        }
    }

    public int GetWeaponPower(int weaponID)
    {
        if (Connect())
        {
            string command = "SELECT Sila from Przedmioty where IDPrzedmiotu = '" + weaponID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int power = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return power;
        }
        else
        {
            return -1;
        }
    }

    public int GetWeaponID(string name)
    {
        if (Connect())
        {
            string command = "SELECT IDPrzedmiotu from Przedmioty where Nazwa = '" + name + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            int id = System.Convert.ToInt32(mySQLcommand.ExecuteScalar());
            mySQLconnection.Close();
            return id;
        }
        else
        {
            return -1;
        }
    }

    public int DeleteAccount()
    {
        if(Connect())
        {
            List<int> entities = new List<int>();
            string command = "SELECT IDPostaci from Postacie where IDUzytkownika = '" + currentLoginID + "'";
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();
            while (mySQLreader.Read())
            {
                int id = System.Convert.ToInt32(mySQLreader["IDPostaci"]);
                entities.Add(id);
            }
            mySQLreader.Close();

            for (int i = 0; i < entities.Count; i++)
            {
                _DeleteEntity(entities[i]);                
            }

            mySQLconnection.Close();
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public void GetEntityWeapons(ref List<string>weaponsName)
    {
        if (Connect())
        {
            string command = "select Nazwa from Przedmioty p inner join PrzedmiotyPostaci pp ON p.IDPrzedmiotu = pp.IDPrzedmiotu WHERE pp.IDPostaci = '" + currentEntityID + "';";
            Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();
            while (mySQLreader.Read())
            {
                Debug.Log(mySQLreader["Nazwa"].ToString());
                weaponsName.Add(mySQLreader["Nazwa"].ToString());
            }
            mySQLreader.Close();
            mySQLconnection.Close();
        }
        else
        {
            weaponsName.Add("DB interrupt");
        }
    }

    public string GetEntityName()
    {
        if (Connect())
        {
            string name="";
            string command = "select ImiePostaci from Postacie where IDPostaci = '" + currentEntityID + "';";
          //  Debug.Log(command);
            mySQLcommand = new MySqlCommand(command, mySQLconnection);
            mySQLreader = mySQLcommand.ExecuteReader();
            while (mySQLreader.Read())
            {
                name = mySQLreader["ImiePostaci"].ToString();
            }
            mySQLreader.Close();
            mySQLconnection.Close();
            return name;
        }
        else
        {
            return ("DB interrupt");
        }
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
