using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Servicos.Database;
using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class HealthCheckController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HealthCheckController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
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

                using (var connection = new SqlConnection(Conexao.StringConexao(HttpContext.Request, _memoryCache, _configuration, _webHostEnvironment)))
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
}
