using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.clientes
{
    public class EditarModel : PageModel
    {
        public cliente cliente = new cliente();
        public int newId = 0;
        public int oldId;
        public double TotalVendas = 0;
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet()
        {
            int id = int.Parse(Request.Query["id"]);

            try
            {
                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM cliente FULL OUTER JOIN (SELECT cliente, SUM(preco) AS vendas FROM venda GROUP BY cliente) AS c ON c.cliente = cliente.id WHERE cliente.Id = @id";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            cliente.Id = reader.GetInt16(0);
                            newId = cliente.Id;
                            oldId = cliente.Id;
                            cliente.Nome = reader.GetString(1);
                            cliente.Email = reader.GetString(2);
                            cliente.Morada = reader.GetString(3);
                            try {
                                cliente.Vendas = reader.GetDouble(5);
                                TotalVendas += cliente.Vendas;
                            } catch (Exception e){
                                cliente.Vendas = 0;
                            }

                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            cliente.Id = int.Parse(Request.Form["id"]);
            newId = int.Parse(Request.Form["newId"]);
            oldId = cliente.Id;
            cliente.Nome = Request.Form["nome"];
            cliente.Email = Request.Form["email"];
            cliente.Morada = Request.Form["morada"];

            if (cliente.Nome.Length == 0 || cliente.Email.Length == 0 || cliente.Morada.Length == 0)
            {
                errorMessage = "Todos os campos são obrigatórios.";
                return;
            }

            try
            {
                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE cliente " +
                                 "SET id=@newid, nome=@nome, email=@email, morada=@morada " +
                                 "WHERE id=@id";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@newid", newId);
                        command.Parameters.AddWithValue("@nome", cliente.Nome);
                        command.Parameters.AddWithValue("@email", cliente.Email);
                        command.Parameters.AddWithValue("@morada", cliente.Morada);
                        command.Parameters.AddWithValue("@id", cliente.Id);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            sucessMessage = "Cliente editado com sucesso";
            Response.Redirect("/Clientes/Index");
        }
    }
}
