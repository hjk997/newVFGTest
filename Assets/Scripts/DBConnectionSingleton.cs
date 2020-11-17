using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

namespace Assets.Scripts
{
    public class DBConnectionSingleton : MonoBehaviour
    {
        private static IDbConnection instance;
        private static IDbCommand dbCommand;

        public DBConnectionSingleton(){ }
        
        public static IDbConnection getIDBConnection(){
            if (instance == null)
            {
                
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/virtual_farm_game.db";

                instance = (IDbConnection)new SqliteConnection(conn);
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
                
            return instance;

        }

        public static IDbCommand getIDbCommand()
        {
            if (instance == null)
            {
                string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/virtual_farm_game.db";

                instance = (IDbConnection)new SqliteConnection(conn);
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