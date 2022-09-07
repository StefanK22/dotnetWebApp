using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Npgsql;
using Microsoft.VisualBasic;

namespace Sales.Pages.Anuncios
{
    public class AnunciosModel : PageModel
    {

        public List<Anuncio> anunciosList = new List<Anuncio>();

        public void OnGet()
        {
            try
            {

                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM anuncio ORDER BY Id";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Anuncio anuncio = new Anuncio();
                        anuncio.Id = reader.GetInt16(0);
                        anuncio.Titulo = reader.GetString(1);
                        anuncio.NomeProduto = reader.GetString(2);
                        anuncio.Link = reader.GetString(3);
                        anuncio.DataInicio = (DateOnly) reader.GetDate(4);
                        anuncio.DataFim = (DateOnly) reader.GetDate(5);

                        anunciosList.Add(anuncio);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }

    public class Anuncio
    {
        public int Id;
        public String Titulo;
        public String NomeProduto;
        public String Link;
        public DateOnly DataInicio;
        public DateOnly DataFim;
    }
}
