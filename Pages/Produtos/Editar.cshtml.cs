using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Produtos
{
    public class EditarModel : PageModel
    {
        public Produto produto = new Produto();
        public string newName = "";
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet()
        {
            string nome = Request.Query["nome"];

            try
            {
                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM produto WHERE nome = @nome";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@nome", nome);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            produto.Nome = reader.GetString(0);
                            newName = produto.Nome;
                            produto.Descr = reader.GetString(1);
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
            produto.Nome = Request.Form["nome"];
            newName = Request.Form["newName"];
            produto.Descr = Request.Form["descr"];

            if (produto.Nome.Length == 0)
            {
                errorMessage = "O campo 'Nome' é obrigatório.";
                return;
            }

            try
            {
                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE produto" +
                                " SET nome=@newname, descr=@descr" +
                                " WHERE nome=@nome";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@newname", newName);
                        command.Parameters.AddWithValue("@descr", produto.Descr);
                        command.Parameters.AddWithValue("@nome", produto.Nome);

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

            sucessMessage = "Produto editado com sucesso";
            Response.Redirect("/Produtos/Index");
        }
    }
}
