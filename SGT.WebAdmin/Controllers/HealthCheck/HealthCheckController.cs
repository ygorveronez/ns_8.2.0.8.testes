using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Servicos.Database;
using Servicos.Embarcador.Configuracoes;
using SGT.BackgroundWorkers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace SGT.WebAdmin.Controllers.HealthCheck
{

    public class HealthCheckController : BaseController
    {
        #region Construtores

        public HealthCheckController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index()
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

                using (var connection = new SqlConnection(_conexao.StringConexao))
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