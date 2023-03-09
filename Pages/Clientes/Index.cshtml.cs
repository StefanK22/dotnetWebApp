using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Npgsql;

namespace Sales.Pages.clientes
{
    public class clientesModel : PageModel
    {

        public List<cliente> clientesList = new List<cliente>();

        public void OnGet()
        {
            try
            {
                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM cliente FULL OUTER JOIN (SELECT cliente, ROUND(SUM(preco)::numeric, 2) AS vendas FROM venda GROUP BY cliente) AS c ON c.cliente = cliente.id ORDER BY id";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cliente cliente = new cliente();
                        cliente.Id = reader.GetInt16(0);
                        cliente.Nome = reader.GetString(1);
                        cliente.Email = reader.GetString(2);
                        cliente.Morada = reader.GetString(3);
                        try {
                            cliente.Vendas = reader.GetDouble(5);
                        } catch (Exception e){
                            cliente.Vendas = 0;
                        }


                        clientesList.Add(cliente);
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

    public class cliente
    {
        public int Id;
        public String Nome;
        public String Email;
        public String Morada;
        public double Vendas;
    }
}
