using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
namespace Sales.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public String Profit;
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        try 
        {
            String connectionString = "Server=localhost; Database=stefan.d.k; User ID=stefan.d.k";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT ROUND((SUM(preco) - (SELECT SUM(preco) + (SELECT SUM(custo) "
                        + "FROM destaque) FROM compra))::numeric , 2) FROM venda;";
                NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    try {
                        Profit = reader.GetDouble(0).ToString() + "€";
                    }
                    catch (System.Exception)
                    {
                        Profit = "0.00€";   
                    }
                }
                connection.Close();
            }
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }
}


