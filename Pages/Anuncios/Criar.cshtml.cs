using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Anuncios
{
    public class CriarModel : PageModel
    {
        public Anuncio anuncio = new Anuncio();
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet()
        {
            try
            {
                anuncio.DataInicio = DateOnly.FromDateTime(DateTime.Now);
                anuncio.DataFim = DateOnly.FromDateTime(DateTime.Now);
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
                anuncio.Titulo = Request.Form["titulo"];                     
                anuncio.NomeProduto = Request.Form["nomeProduto"];
                anuncio.Link = Request.Form["link"];
                if (Request.Form["dataInicio"] == "")
                {
                    errorMessage = "A data de inicio é obrigatória.";
                    return;
                } else
                {
                    anuncio.DataInicio = DateOnly.Parse(Request.Form["dataInicio"]);
                }
                if (Request.Form["dataFim"] != "")
                    anuncio.DataFim = DateOnly.Parse(Request.Form["dataFim"]);
                else
                    anuncio.DataFim = DateOnly.MinValue.AddDays(1);
                    
                if (anuncio.Titulo.Length == 0 || anuncio.NomeProduto.Length == 0 || anuncio.Link.Length == 0)
                {
                    errorMessage = "Todos os campos são obrigatórios.";
                    return;
                }

                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    int id = 1;
                    string nextId = "SELECT id FROM anuncio WHERE id >= ALL(SELECT id FROM anuncio)";
                    NpgsqlCommand command = new NpgsqlCommand(nextId, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                        id = reader.GetInt16(0) + 1;
                    connection.Close();
                    connection.Open();
                    string sql = "INSERT INTO anuncio " + "(id, titulo, nome_produto, link, dataInicio, dataFim) VALUES "
                     + "(@id, @titulo, @produto, @link, @data_i, @data_f);";
                    using (command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@titulo", anuncio.Titulo);
                        command.Parameters.AddWithValue("@produto", anuncio.NomeProduto);
                        command.Parameters.AddWithValue("@link", anuncio.Link);
                        command.Parameters.AddWithValue("@data_i", anuncio.DataInicio);
                        command.Parameters.AddWithValue("@data_f", anuncio.DataFim);

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

            anuncio.Titulo = "";
            anuncio.NomeProduto = "";
            anuncio.Link = "";
            anuncio.DataInicio = DateOnly.FromDateTime(DateTime.Now);
            anuncio.DataFim = DateOnly.FromDateTime(DateTime.Now);
            sucessMessage = "Novo Anuncio adicionado com sucesso";

            Response.Redirect("/Anuncios/Index");
        }
    }
}
