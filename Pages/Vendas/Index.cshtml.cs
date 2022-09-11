using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Npgsql;
using Microsoft.VisualBasic;

namespace Sales.Pages.Vendas
{
    public class VendasModel : PageModel
    {

        public List<Venda> vendasList = new List<Venda>();

        public void OnGet()
        {
            try
            {

                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM venda ORDER BY Id";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Venda venda = new Venda();
                        venda.Id = reader.GetInt16(0);
                        venda.CompraId = reader.GetInt16(1);
                        venda.Preco = reader.GetDouble(2);
                        venda.Data = (DateOnly) reader.GetDate(3);
                        venda.Cliente = reader.GetInt16(4);
                        
                        vendasList.Add(venda);
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

    public class Venda
    {
        public int Id;
        public int CompraId;
        public double Preco;
        public DateOnly Data;
        public int Cliente;
        public string Detalhes;
    }
}
