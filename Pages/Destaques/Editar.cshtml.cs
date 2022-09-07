using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Destaques
{
    public class EditarModel : PageModel
    {
        public Destaque destaque = new Destaque();
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
                    String sql = "SELECT * FROM destaque WHERE Id = @id";

                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            destaque.Id = reader.GetInt16(0);
                            newId = destaque.Id;
                            destaque.Tipo = reader.GetString(1);
                            destaque.Custo = reader.GetDouble(2);
                            destaque.Anuncio = reader.GetInt16(3);
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
                destaque.Id = int.Parse(Request.Form["id"]);
                newId = int.Parse(Request.Form["newId"]);
                destaque.Tipo = Request.Form["tipo"];
                string custo = Request.Form["custo"];
                destaque.Custo = double.Parse(custo.Replace(".", ","));
                destaque.Anuncio = int.Parse(Request.Form["anuncio"]);
                
                if (destaque.Tipo.Length == 0 || destaque.Custo == 0 || destaque.Anuncio == 0)
                {
                    errorMessage = "Todos os campos são obrigatórios.";
                    return;
                }


                String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE destaque " +
                                 "SET id=@newid, tipo=@tipo, custo=@custo, anuncio=@anuncio " +
                                 "WHERE id=@id";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", destaque.Id);
                        command.Parameters.AddWithValue("@newid", newId);
                        command.Parameters.AddWithValue("@tipo", destaque.Tipo);
                        command.Parameters.AddWithValue("@custo", destaque.Custo);
                        command.Parameters.AddWithValue("@anuncio", destaque.Anuncio);

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

            sucessMessage = "Destaque editado com sucesso";
            Response.Redirect("/Destaques/Index");
        }
    }
}
