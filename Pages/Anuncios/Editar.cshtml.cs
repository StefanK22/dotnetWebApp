using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Anuncios
{
    public class EditarModel : PageModel
    {
        public Anuncio anuncio = new Anuncio();
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
                    String sql = "SELECT * FROM anuncio WHERE Id = @id";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            anuncio.Id = reader.GetInt16(0);
                            newId = anuncio.Id;
                            anuncio.Titulo = reader.GetString(1);
                            anuncio.NomeProduto = reader.GetString(2);
                            anuncio.Link = reader.GetString(3);
                            anuncio.DataInicio = (DateOnly) reader.GetDate(4);
                            anuncio.DataFim = (DateOnly) reader.GetDate(5);
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

                anuncio.Id = int.Parse(Request.Form["id"]);
                newId = int.Parse(Request.Form["newId"]);
                anuncio.Titulo = Request.Form["titulo"];                     
                anuncio.NomeProduto = Request.Form["nomeProduto"];
                anuncio.Link = Request.Form["link"];
                if (Request.Form["dataIncio"] == "")
                {
                    errorMessage = "A data de inicio do anuncio é obrigatória.";
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
                    String sql = "UPDATE anuncio " +
                                 "SET id=@newid, titulo=@titulo, nome_produto=@produto, link=@link, dataInicio=@data_i, dataFim=@data_f " +
                                 "WHERE id=@id";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@newid", newId);
                        command.Parameters.AddWithValue("@titulo", anuncio.Titulo);
                        command.Parameters.AddWithValue("@produto", anuncio.NomeProduto);
                        command.Parameters.AddWithValue("@link", anuncio.Link);
                        command.Parameters.AddWithValue("@data_i", anuncio.DataInicio);
                        command.Parameters.AddWithValue("@data_f", anuncio.DataFim);
                        command.Parameters.AddWithValue("@id", anuncio.Id);

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

            sucessMessage = "Anuncio editado com sucesso";
            Response.Redirect("/Anuncios/Index");
        }
    }
}
