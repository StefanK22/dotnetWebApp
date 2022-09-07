using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.clientes
{
    public class CriarModel : PageModel
    {
        public cliente cliente = new cliente();
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
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
                    int id = 1;
                    string nextId = "SELECT id FROM cliente WHERE id >= ALL(SELECT id FROM cliente)";
                    NpgsqlCommand cmd = new NpgsqlCommand(nextId, connection);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        id = reader.GetInt16(0) + 1;
                    connection.Close();
                    connection.Open();
                    String sql = "INSERT INTO cliente " + "(id, nome, email, morada) VALUES " + "(@id, @Nome, @Email, @Morada);";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@Nome", cliente.Nome);
                        command.Parameters.AddWithValue("@Email", cliente.Email);
                        command.Parameters.AddWithValue("@Morada", cliente.Morada);

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

            cliente.Nome = "";
            cliente.Email = "";
            cliente.Morada = "";
            sucessMessage = "Novo Cliente adicionado com sucesso";

            Response.Redirect("/Clientes/Index");
        }
    }
}
