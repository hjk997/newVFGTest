using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using Mono.Data.SqliteClient;
using System.IO;
using System;

namespace Assets.Scripts
{
    public class DBConnectionSingleton : MonoBehaviour
    {
        private static IDbConnection instance;
        private static IDbCommand dbCommand;

        public DBConnectionSingleton() { }

        public static IDbCommand getIDbCommand()
        {
            if (instance == null)
            {
                string filename;

                if (Application.platform == RuntimePlatform.Android) {
                    filename = Application.persistentDataPath + "/virtual_farm_game.db";
                    if (File.Exists(filename))
                    {
                        WWW loadDb = new WWW("jar:file://" + Application.dataPath +"!assets/virtual_farm_game.db");
                        loadDb.bytesDownloaded.ToString();
                        while (!loadDb.isDone)
                        {
                            File.WriteAllBytes(filename, loadDb.bytes);
                        }
                    }
                    else
                    {
                        filename = Application.dataPath + "/virtual_farm_game.db";
                        if (File.Exists(filename))
                        {
                            File.Copy(Application.streamingAssetsPath + "virtual_farm_game.db", filename);
                        }
                    }
                }
                else
                {
                    filename =  Application.dataPath + "/StreamingAssets/virtual_farm_game.db";
                }
                
                instance = (IDbConnection)new Mono.Data.Sqlite.SqliteConnection("URI=file:" + filename);
                instance.Open(); //Open connection to the database.

                //if (instance.State == ConnectionState.Open)
                //{
                //    Debug.Log("success");
                //}
                //else
                //{
                //    Debug.Log("fail");
                //}

            }

            if (dbCommand == null)
            {
                dbCommand = instance.CreateCommand();
            }

            return dbCommand;
        }

    }
}