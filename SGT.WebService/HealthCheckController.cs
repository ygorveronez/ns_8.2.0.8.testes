using Microsoft.AspNetCore.Mvc;
using Servicos.Database;
using SGT.WebService;
using System.Data.SqlClient;
using System.Net;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class HealthCheckController : Controller
{
    IServiceProvider _serviceProvider;
    public HealthCheckController(IServiceProvider serviceProvider) 
    {
        _serviceProvider = serviceProvider;
    }



    public async Task<IActionResult> CheckHealth()
    {
        try
        {
            var healty = false;

            using (var connection = new SqlConnection(ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT 1", connection))
                {
                    var result = await command.ExecuteScalarAsync();

                    if (result != null && (int)result == 1)
                    {
                        healty = true;
                    }
                }
            }

            using (var connection = new SqlConnection(Conexao.createInstance(_serviceProvider).StringConexao))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SELECT 1", connection))
                {
                    var result = await command.ExecuteScalarAsync();

                    if (result != null && (int)result == 1)
                    {
                        healty = true;
                    }
                }
            }

            if (healty)
                return Ok(new { Status = "Healthy" });
            else
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, new { Status = "Unhealthy" });
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.ServiceUnavailable, new { Status = "Unhealthy" });
        }
    }
}