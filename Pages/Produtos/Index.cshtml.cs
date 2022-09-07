using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Npgsql;

namespace Sales.Pages.Produtos
{
    public class ProdutosModel : PageModel
    {

        public List<Produto> produtosList = new List<Produto>();

        public void OnGet()
        {
            try
            {

                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM produto ORDER BY nome";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Produto produto = new Produto();
                        produto.Nome = reader.GetString(0);
                        produto.Descr = reader.GetString(1);
                        produtosList.Add(produto);
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

    public class Produto
    {
        public String Nome;
        public String Descr;
    }
}
