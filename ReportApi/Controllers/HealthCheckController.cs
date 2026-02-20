
using Servicos.Database;
using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net;
using System.Web.Mvc;
using ReportApi.Common;

namespace ReportApi.Controllers
{

    public class HealthCheckController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var healty = false;

                using (var connection = new SqlConnection(ConnectionFactory.StringConexaoAdmin))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        var result = command.ExecuteScalar();

                        if (result != null && (int)result == 1)
                        {
                            healty = true;
                        }
                    }
                }

                using (var connection = new SqlConnection(ConnectionFactory.StringConexao))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        var result = command.ExecuteScalar();

                        if (result != null && (int)result == 1)
                        {
                            healty = true;
                        }
                    }
                }

                if (healty)
                {
                    return Json(new { Status = "Healthy" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    return Json(new { Status = "Unhealthy" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                return Json(new { Status = "Unhealthy", Error = ex.Message }, JsonRequestBehavior.AllowGet);
            };
        }
    }
}