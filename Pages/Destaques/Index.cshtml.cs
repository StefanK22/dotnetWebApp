﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Npgsql;
using Microsoft.VisualBasic;

namespace Sales.Pages.Destaques
{
    public class DestaquesModel : PageModel
    {

        public List<Destaque> destaquesList = new List<Destaque>();
        public string Total;

        public void OnGet()
        {
            try
            {

                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT ROUND(SUM(custo)::numeric, 2) FROM destaque;";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        try {
                            Total = reader.GetDouble(0).ToString() + "€";
                        }
                        catch (System.Exception)
                        {
                            Total = "0.00€";   
                        }
                    }
                    connection.Close();
                    connection.Open();
                    sql = "SELECT * FROM destaque ORDER BY Id";
                    command = new NpgsqlCommand(sql, connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Destaque destaque = new Destaque();
                        destaque.Id = reader.GetInt16(0);
                        destaque.Tipo = reader.GetString(1);
                        destaque.Custo = reader.GetDouble(2);
                        destaque.Anuncio = reader.GetInt16(3);

                        destaquesList.Add(destaque);
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

    public class Destaque
    {
        public int Id;
        public String Tipo;
        public double Custo;
        public int Anuncio;
    }
}
