using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Vendas
{
    public class CriarModel : PageModel
    {
        public Venda venda = new Venda();
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet()
        {
            try
            {
                venda.Data = DateOnly.FromDateTime(DateTime.Now);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {          
            try
            {                          
                venda.CompraId = int.Parse(Request.Form["compraId"]);
                venda.Cliente = int.Parse(Request.Form["cliente"]);
                string preco = Request.Form["preco"];
                venda.Preco = double.Parse(preco.Replace(".", ","));
                venda.Detalhes = Request.Form["detalhes"];
                if (Request.Form["data"] == "")
                {
                    errorMessage = "A data de compra é obrigatória.";
                    return;
                } else
                    venda.Data = DateOnly.Parse(Request.Form["data"]);

                if (venda.CompraId == 0 || venda.Cliente == 0 || venda.Preco == 0)
                {
                    errorMessage = "Todos os campos são obrigatórios.";
                    return;
                }

                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    int id = 1;
                    string nextId = "SELECT id FROM venda WHERE id >= ALL(SELECT id FROM venda)";
                    NpgsqlCommand command = new NpgsqlCommand(nextId, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                        id = reader.GetInt16(0) + 1;
                    connection.Close();
                    connection.Open();
                    string sql = "INSERT INTO venda " + "(id, id_compra, preco, data_venda, cliente, detalhes) VALUES "
                     + "(@id, @compraId, @preco, @data, @cliente, @detalhes);";
                    using (command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@compraId", venda.CompraId);
                        command.Parameters.AddWithValue("@preco", venda.Preco);
                        command.Parameters.AddWithValue("@data", venda.Data);
                        command.Parameters.AddWithValue("@cliente", venda.Cliente);
                        command.Parameters.AddWithValue("@detalhes", venda.Detalhes);

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

            venda.CompraId = 0;
            venda.Preco = 0;
            venda.Data = DateOnly.FromDateTime(DateTime.Now);
            venda.Cliente = 0;
            venda.Detalhes = "";
            sucessMessage = "Nova Compra adicionado com sucesso";

            Response.Redirect("/Vendas/Index");
        }
    }
}
