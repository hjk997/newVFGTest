using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class dbScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/virtual_farm_game.db";

		// IDbConnection
		IDbConnection dbconn;
		dbconn = (IDbConnection)new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.

		if (dbconn.State == ConnectionState.Open)
		{
			Debug.Log("success");
		}
        else
        {
			Debug.Log("fail");
		}

		// IDbCommand
		IDbCommand dbcmd = dbconn.CreateCommand();

		//"SELECT Colum,··· FROM TableName";

		string[] ttag = { "farm1", "farm2", "farm3", "farm4", "farm5"};

		string sqlQuery = "";
		dbcmd.CommandText = sqlQuery;
		dbcmd.ExecuteNonQuery();

		// Closed Db
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}

}
