using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace Sales.Pages.Destaques
{
    public class CriarModel : PageModel
    {
        public Destaque destaque = new Destaque();
        public String errorMessage = "";
        public String sucessMessage = "";

        public void OnGet(){}
        public void OnPost()
        {          
            try
            {                          
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
                    int id = 1;
                    string nextId = "SELECT id FROM destaque WHERE id >= ALL(SELECT id FROM destaque)";
                    NpgsqlCommand command = new NpgsqlCommand(nextId, connection);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                        id = reader.GetInt16(0) + 1;
                    connection.Close();
                    connection.Open();
                    string sql = "INSERT INTO destaque " + "(id, tipo, custo, anuncio) VALUES "
                     + "(@id, @tipo, @custo, @anuncio);";
                    using (command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
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

            destaque.Tipo = "";
            destaque.Custo = 0;
            destaque.Anuncio = 0;
            sucessMessage = "Nova Compra adicionado com sucesso";

            Response.Redirect("/Destaques/Index");
        }
    }
}
