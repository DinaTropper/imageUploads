using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace imageUpload.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddImage(Image i)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images (ImagePath, Password, Views) VALUES (@path, @password, @views)SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@path", i.ImagePath);
            cmd.Parameters.AddWithValue("@password", i.Password);
            cmd.Parameters.AddWithValue("@views", i.Views);
            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public Image ViewImage(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Images WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Image
            {
                Id = (int)reader["Id"],
                ImagePath = (string)reader["ImagePath"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };
        }
        public void AddViewCount(int id, Image i)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Images SET Views = @views + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@views", i.Views);
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public int GetViewCount(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Views FROM Images WHERE Id = @id SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (int)cmd.ExecuteScalar();

        }
    }
}
