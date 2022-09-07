using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Vendas
{
    public class EditarModel : PageModel
    {
        public Venda venda = new Venda();
        public int newId = 0;
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
                    String sql = "SELECT * FROM venda WHERE Id = @id";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            venda.Id = reader.GetInt16(0);
                            newId = venda.Id;
                            venda.CompraId = reader.GetInt16(1);
                            venda.Preco = reader.GetDouble(2);
                            venda.Data = (DateOnly) reader.GetDate(3);
                            venda.cliente = reader.GetInt16(4);
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
            try
            {
                venda.Id = int.Parse(Request.Form["id"]);
                newId = int.Parse(Request.Form["newId"]);
                venda.CompraId = int.Parse(Request.Form["compraId"]);
                venda.cliente = int.Parse(Request.Form["cliente"]);
                string preco = Request.Form["preco"];
                venda.Preco = double.Parse(preco.Replace(".", ","));
                if (Request.Form["data"] == "")
                {
                    errorMessage = "A data de venda é obrigatória.";
                    return;
                } else
                    venda.Data = DateOnly.Parse(Request.Form["data"]);

                if (venda.CompraId == 0 || venda.cliente == 0 || venda.Preco == 0)
                {
                    errorMessage = "Todos os campos são obrigatórios.";
                    return;
                }


                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE venda " +
                                 "SET id=@newid, id_compra=@compra, preco=@preco, data_venda=@data, cliente=@cliente " +
                                 "WHERE id=@id";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", venda.Id);
                        command.Parameters.AddWithValue("@newid", newId);
                        command.Parameters.AddWithValue("@compra", venda.CompraId);
                        command.Parameters.AddWithValue("@preco", venda.Preco);
                        command.Parameters.AddWithValue("@data", venda.Data);
                        command.Parameters.AddWithValue("@cliente", venda.cliente);

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

            sucessMessage = "Venda editada com sucesso";
            Response.Redirect("/Vendas/Index");
        }
    }
}
