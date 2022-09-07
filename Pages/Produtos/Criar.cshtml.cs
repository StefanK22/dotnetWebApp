using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Produtos
{
    public class CriarModel : PageModel
    {
        public Produto produto = new Produto();
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            produto.Nome = Request.Form["nome"];
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
                    String sql = "INSERT INTO produto " + "(nome, descr) VALUES " + "(@nome, @descr);";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@nome", produto.Nome);
                        command.Parameters.AddWithValue("@descr", produto.Descr);

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

            produto.Nome = "";
            produto.Descr = "";
            sucessMessage = "Novo Produto adicionado com sucesso";

            Response.Redirect("/Produtos/Index");
        }
    }
}
