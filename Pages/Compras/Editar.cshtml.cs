using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Compras
{
    public class EditarModel : PageModel
    {
        public Compra compra = new Compra();
        public int newId = 0;
        public bool showAll;
        public String errorMessage = "";
        public String sucessMessage = "";
        public void OnGet()
        {
            int id = int.Parse(Request.Query["id"]);
            showAll = bool.Parse(Request.Query["showAll"]);

            try
            {
                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM compra WHERE Id = @id";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            compra.Id = reader.GetInt16(0);
                            newId = compra.Id;
                            compra.NomeProduto = reader.GetString(1);
                            compra.Link = reader.GetString(2);
                            compra.DataCompra = (DateOnly) reader.GetDate(3);
                            compra.DataRecebimento = (DateOnly) reader.GetDate(4);
                            compra.Preco = reader.GetDouble(5);
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
                showAll = bool.Parse(Request.Query["showAll"]);
                compra.Id = int.Parse(Request.Form["id"]);
                newId = int.Parse(Request.Form["newId"]);
                compra.NomeProduto = Request.Form["nomeProduto"];
                compra.Link = Request.Form["link"];
                string preco = Request.Form["preco"];
                compra.Preco = double.Parse(preco.Replace(".", ","));
                if (Request.Form["dataCompra"] == "")
                {
                    errorMessage = "A data de compra é obrigatória.";
                    return;
                } else
                {
                    compra.DataCompra = DateOnly.Parse(Request.Form["dataCompra"]);
                }
                if (Request.Form["dataRecebimento"] != "")
                    compra.DataRecebimento = DateOnly.Parse(Request.Form["dataRecebimento"]);
                else
                    compra.DataRecebimento = DateOnly.MinValue.AddDays(1);
                    

                if (compra.NomeProduto.Length == 0 || compra.Link.Length == 0 || compra.Preco == 0)
                {
                    errorMessage = "Todos os campos são obrigatórios.";
                    return;
                }


                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE compra " +
                                 "SET id=@newid, nome_produto=@produto, link=@link, data_compra=@data_c, data_recebimento=@data_r, preco=@preco " +
                                 "WHERE id=@id";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", compra.Id);
                        command.Parameters.AddWithValue("@newid", newId);
                        command.Parameters.AddWithValue("@produto", compra.NomeProduto);
                        command.Parameters.AddWithValue("@link", compra.Link);
                        command.Parameters.AddWithValue("@data_c", compra.DataCompra);
                        command.Parameters.AddWithValue("@data_r", compra.DataRecebimento);
                        command.Parameters.AddWithValue("@preco", compra.Preco);

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

            sucessMessage = "Compra editada com sucesso";
            if (showAll)
                Response.Redirect("/Compras/Index?showAll=true");
            else
                Response.Redirect("/Compras/Index?showAll=false");
        }
    }
}
