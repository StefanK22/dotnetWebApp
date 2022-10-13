using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Compras
{
    public class CriarModel : PageModel
    {
        public Compra compra = new Compra();
        public bool showAll;
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet()
        {
            try
            {
                showAll = bool.Parse(Request.Query["showAll"]);
                compra.DataCompra = DateOnly.FromDateTime(DateTime.Now);
                compra.DataRecebimento = DateOnly.FromDateTime(DateTime.Now);
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
                    int id = 1;
                    string nextId = "SELECT id FROM compra WHERE id >= ALL(SELECT id FROM compra)";
                    NpgsqlCommand command = new NpgsqlCommand(nextId, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                        id = reader.GetInt16(0) + 1;
                    connection.Close();
                    connection.Open();
                    string sql = "INSERT INTO compra " + "(id, nome_produto, link, data_compra, data_recebimento, preco) VALUES "
                     + "(@id, @produto, @link, @data_c, @data_r, @preco);";
                    using (command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
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

            compra.NomeProduto = "";
            compra.Link = "";
            compra.DataCompra = DateOnly.FromDateTime(DateTime.Now);
            compra.DataRecebimento = DateOnly.FromDateTime(DateTime.Now);
            compra.Preco = 0.00;
            sucessMessage = "Nova Compra adicionado com sucesso";

            if (showAll)
                Response.Redirect("/Compras/Index?showAll=true");
            else
                Response.Redirect("/Compras/Index?showAll=false");
        }
    }
}
