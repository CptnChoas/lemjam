using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace LemJam
{
    public class Database
    {
        private string connectionString = @"Data Source=C:\Users\Lenni\Desktop\Entwicklung\lemjam.sqlite";

        SQLiteConnection con;

        public Database()
        {
            con = new SQLiteConnection(connectionString);
            con.Open();
            
        }

        public List<PathItem> GetPathItems()
        {
            List<PathItem> items = new List<PathItem>();
            SQLiteCommand com = con.CreateCommand();

            com.CommandText = "SELECT * FROM PathPool";

            using (SQLiteDataReader reader = com.ExecuteReader())
            {
                while(reader.Read())
                {
                    items.Add(PathItem.FromDatabase(reader));
                }
            }

            return items;
        }

        public List<FileItem> GetFileItems()
        {
            List<FileItem> items = new List<FileItem>();

            SQLiteCommand com = con.CreateCommand();

            com.CommandText = "SELECT * FROM Items";

            using (SQLiteDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(FileItem.FromDatabase(reader));
                }
            }

            return items;
        }

        public void Store(IDatabase item)
        {
            SQLiteCommand com = con.CreateCommand();

            StringBuilder sb = new StringBuilder();
            StringBuilder valueString = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, object> kvp in item.Fields) {


                com.Parameters.AddWithValue("$" + i, kvp.Value);

                if (sb.Length != 0)
                {
                    valueString.Append(",");
                    sb.Append(",");
                }
                sb.Append(kvp.Key);

                valueString.Append("$");
                valueString.Append(i++);
            }

            com.CommandText = "Insert into " + item.TableName + " VALUES (" + valueString.ToString() + ");";

            com.Prepare();

            if (com.ExecuteNonQuery() == 0)
                throw new Exception("Insert failed for table " + item.TableName + "!");
        }

        public void Update(IDatabase item)
        {
            SQLiteCommand com = con.CreateCommand();

            StringBuilder sb = new StringBuilder();
            int i = 50;
            foreach (KeyValuePair<string, object> kvp in item.PrimaryKeys)
            {

                com.Parameters.Add(new SQLiteParameter("@" + i, kvp.Value));

                if (sb.Length != 0)
                    sb.Append(" AND ");

                sb.Append(kvp.Key);
                sb.Append(" = @");
                sb.Append(i++);
            }

            com.CommandText = "SELECT COUNT(*) FROM " + item.TableName + " WHERE " + sb.ToString();

            object blub = com.ExecuteScalar();
            if ((int)blub == 0)
            {
                Store(item);
                return;
            }

            //com.Reset();

            StringBuilder updateSb = new StringBuilder();
            int j = 0;
            foreach (KeyValuePair<string, object> kvp in item.Fields)
            {
                com.Parameters.AddWithValue("@" + j, kvp.Value);

                if (updateSb.Length != 0)
                    updateSb.Append(",");

                updateSb.Append(kvp.Key);
                updateSb.Append("=@");
                updateSb.Append(j++);
            }

            com.CommandText = "UPDATE " + item.TableName + " SET " + updateSb.ToString() + " WHERE " + sb.ToString() + "";

            if (com.ExecuteNonQuery() == 0)
                throw new Exception("Update failed for table " + item.TableName + "!");
        }

        public void Delete(IDatabase item)
        {
            SQLiteCommand com = con.CreateCommand();

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<string, object> kvp in item.PrimaryKeys)
            {
                com.Parameters.AddWithValue("$" + i, kvp.Value.ToString());

                if (sb.Length != 0)
                    sb.Append(" AND ");

                sb.Append(kvp.Key);
                sb.Append("=$");
                sb.Append(i++);
            }

            com.CommandText = "DELETE FROM " + item.TableName + " WHERE " + sb.ToString();

            if ((int)com.ExecuteScalar() == 0)
            {
                throw new Exception("DELETE Failed!");
            }
        }
    }
}
