using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Npgsql;
using Microsoft.VisualBasic;

namespace Sales.Pages.Compras
{
    public class ComprasModel : PageModel
    {

        public List<Compra> comprasList = new List<Compra>();

        public void OnGet()
        {
            try
            {

                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM compra ORDER BY Id";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Compra compra = new Compra();
                        compra.Id = reader.GetInt16(0);
                        compra.NomeProduto = reader.GetString(1);
                        compra.Link = reader.GetString(2);
                        compra.DataCompra = (DateOnly) reader.GetDate(3);
                        compra.DataRecebimento = (DateOnly) reader.GetDate(4);
                        compra.Preco = reader.GetDouble(5);

                        comprasList.Add(compra);
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

    public class Compra
    {
        public int Id;
        public String NomeProduto;
        public String Link;
        public DateOnly DataCompra;
        public DateOnly DataRecebimento;
        public double Preco;
    }
}
