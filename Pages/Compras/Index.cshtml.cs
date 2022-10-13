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
        public List<int> soldList = new List<int>();

        public string Total;
        public bool showAll = false;

        public void OnGet()
        {
            if (Request.Query["showAll"].ToString() != "")
                showAll = bool.Parse(Request.Query["showAll"]);

            try
            {
                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";
                
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT id FROM compra WHERE id IN (SELECT id_compra FROM venda) ORDER BY Id;";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        soldList.Add(reader.GetInt16(0));
                    }
                    connection.Close();
                }

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT SUM(preco) FROM compra;";
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
                    if (showAll)
                        sql = "SELECT * FROM compra ORDER BY Id";
                    else 
                        sql = "SELECT * FROM compra WHERE id NOT IN (SELECT id_compra FROM venda) ORDER BY Id";
                    command = new NpgsqlCommand(sql, connection);
                    reader = command.ExecuteReader();
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
