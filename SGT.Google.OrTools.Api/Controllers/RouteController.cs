using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Google.OrTools.Api.Controllers
{
    [RoutePrefix("route")]
    public class RouteController : ApiController
    {

        #region  " Variáveis Privadas "

        private string _key = "348969859b3838c3aef99b607688a849"; // "md5(Multi-Software)";
        private string _server_osrm = "http://191.232.193.82/osrm/";

        #endregion

        #region " Métodos privados "

        private string ValidarServerOsrm(string server)
        {
            if (server == null) server = string.Empty;

            if (string.IsNullOrEmpty(server))
                server = _server_osrm;

            if (!server.Contains("/osrm"))
            {
                if (server.EndsWith("/"))
                    server += "osrm/";
                else
                    server += "/osrm/";
            }

            if (!server.Contains("http"))
                server = "http://" + server;

            return server;
        }

        private void GerarLogResultado(string guid, Services.GoogleOrTools.Resultado resultado)
        {
            if (resultado == null)
                return;

            this.GerarLogResultado(guid, new List<Services.GoogleOrTools.Resultado>() { resultado });
        }

        private void GerarLogResultado(string guid, List<Services.GoogleOrTools.Resultado> resultado)
        {
            try
            {
                string json = "ERRO";
                if (resultado != null)
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(resultado);
                Services.Errors.GravaLogEmTxt(guid + " - " + json, "Resultado.:");
            }
            catch (Exception ex)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao processar resultado da rota com Google OrTools: {ex}", "CatchNoAction");
            }
        }

        #endregion

        #region " Métodos Públicos "

        [HttpPost]
        [Route("tsp/{key}/{problema?}")]
        [System.Web.Http.Description.ResponseType(typeof(Services.GoogleOrTools.Resultado))]
        public HttpResponseMessage TSP(string key, [FromBody] Models.Tsp problema)
        {
            if (key == _key)
            {
                problema.ServerOsrm = this.ValidarServerOsrm(problema.ServerOsrm);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(problema);
                try
                {
                    string guid = Guid.NewGuid().ToString();

                    Services.Errors.GravaLogEmTxt(guid + " - " + json, Request.RequestUri.AbsoluteUri);

                    var resultado = new Services.GoogleOrTools.TSP().Resolver(problema);

                    this.GerarLogResultado(guid, resultado);

                    if (resultado != null)
                    {
                        return Request.CreateResponse<Models.Response<Services.GoogleOrTools.Resultado>>(HttpStatusCode.OK,
                                                                                 new Models.Response<Services.GoogleOrTools.Resultado>()
                                                                                 {
                                                                                     status = true,
                                                                                     msg = "TSP - Otimização gerada com sucesso.",
                                                                                     result = resultado
                                                                                 });
                    }
                    else
                    {
                        return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.OK,
                                                                                 new Models.Response<string>()
                                                                                 {
                                                                                     status = false,
                                                                                     msg = "TSP - Não foi possível otimizar os locais.",
                                                                                     result = string.Empty
                                                                                 });
                    }

                }
                catch (Exception ex)
                {
                    Services.Errors.GravaLog(ex, string.Format("{0} -> {1}", "TSP", json));
                    return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.InternalServerError,
                                                                   new Models.Response<string>()
                                                                   {
                                                                       status = false,
                                                                       msg = ex.Message
                                                                   });
                }
            }
            else
                return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.Unauthorized,
                                                                       new Models.Response<string>()
                                                                       {
                                                                           status = false,
                                                                           msg = "Api Key inválida."
                                                                       });

        }

        [HttpPost]
        [Route("cvrp/{key}/{problema?}")]
        [System.Web.Http.Description.ResponseType(typeof(List<Services.GoogleOrTools.Resultado>))]
        public HttpResponseMessage CVRP(string key, [FromBody] Models.VrpCapacity problema)
        {
            if (key == _key)
            {
                problema.ServerOsrm = this.ValidarServerOsrm(problema.ServerOsrm);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(problema);
                try
                {
                    string guid = Guid.NewGuid().ToString();

                    Services.Errors.GravaLogEmTxt(guid + " - " + json, Request.RequestUri.AbsoluteUri);

                    //Vamos filtrar somente os pedidos com peso <= peso máximo de transporte..
                    double capacMaxTransporte = (from veiculo in problema.Veiculos select veiculo.Capacidade).Max();
                    problema.Locais = (from local in problema.Locais where local.PesoTotal <= capacMaxTransporte select local).ToList();
                    // Filtrando apenas locais aonde possui algum veiculo sem restrição...
                    problema.Locais = (from obj in problema.Locais
                                       where obj.Deposito || obj.VeiculosRestritos == null || problema.Veiculos.Any(v => !obj.VeiculosRestritos.Any(r => r == v.CodigoModelo))
                                       select obj).ToList();

                    List<Services.GoogleOrTools.Resultado> resultado = new Services.GoogleOrTools.CVRP().Resolver(problema);

                    this.GerarLogResultado(guid, resultado);

                    if (resultado != null)
                    {
                        return Request.CreateResponse<Models.Response<List<Services.GoogleOrTools.Resultado>>>(HttpStatusCode.OK,
                                                                                 new Models.Response<List<Services.GoogleOrTools.Resultado>>()
                                                                                 {
                                                                                     status = true,
                                                                                     msg = "CVRP - Gerado com sucesso.",
                                                                                     result = resultado,
                                                                                     qtde = resultado?.Count ?? 0
                                                                                 });
                    }
                    else
                    {
                        return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.OK,
                                                                                 new Models.Response<string>()
                                                                                 {
                                                                                     status = false,
                                                                                     msg = "CVRP - Não foi possível separar os locais em veículos.",
                                                                                     result = string.Empty
                                                                                 });
                    }

                }
                catch (Exception ex)
                {
                    Services.Errors.GravaLog(ex, string.Format("{0} -> {1}", "CVRP", json));
                    return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.InternalServerError,
                                                                   new Models.Response<string>()
                                                                   {
                                                                       status = false,
                                                                       msg = ex.Message
                                                                   });
                }
            }
            else
                return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.Unauthorized,
                                                                       new Models.Response<string>()
                                                                       {
                                                                           status = false,
                                                                           msg = "Api Key inválida."
                                                                       });
        }

        [HttpPost]
        [Route("cvrptw/{key}/{problema?}")]
        [System.Web.Http.Description.ResponseType(typeof(List<Services.GoogleOrTools.Resultado>))]
        public HttpResponseMessage CVRPTW(string key, [FromBody] Models.VrpCapacity problema)
        {
            if (key == _key)
            {
                problema.ServerOsrm = this.ValidarServerOsrm(problema.ServerOsrm);

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(problema);
                try
                {
                    string guid = Guid.NewGuid().ToString();

                    Services.Errors.GravaLogEmTxt(guid + " - " + json, Request.RequestUri.AbsoluteUri);

                    //Vamos filtrar somente os pedidos com peso <= peso máximo de transporte..
                    double capacMaxTransporte = (from veiculo in problema.Veiculos select veiculo.Capacidade).Max();
                    problema.Locais = (from local in problema.Locais where local.PesoTotal <= capacMaxTransporte select local).ToList();
                    // Filtrando apenas locais aonde possui algum veiculo sem restrição...
                    problema.Locais = (from obj in problema.Locais
                                       where obj.Deposito || obj.VeiculosRestritos == null || problema.Veiculos.Any(v => !obj.VeiculosRestritos.Any(r => r == v.CodigoModelo))
                                       select obj).ToList();

                    var resultado = new Services.GoogleOrTools.CVRPTW().Resolver(problema);

                    this.GerarLogResultado(guid, resultado);

                    if (resultado != null)
                    {
                        return Request.CreateResponse<Models.Response<List<Services.GoogleOrTools.Resultado>>>(HttpStatusCode.OK,
                                                                                 new Models.Response<List<Services.GoogleOrTools.Resultado>>()
                                                                                 {
                                                                                     status = true,
                                                                                     msg = "CVRPTW - Gerado com sucesso.",
                                                                                     result = resultado,
                                                                                     qtde = resultado?.Count ?? 0
                                                                                 });
                    }
                    else
                    {
                        return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.OK,
                                                                                 new Models.Response<string>()
                                                                                 {
                                                                                     status = false,
                                                                                     msg = "CVRPTW - Não foi possível separar os locais em veículos. Verifique os parâmetros de tempo.",
                                                                                     result = string.Empty
                                                                                 });
                    }

                }
                catch (Exception ex)
                {
                    Services.Errors.GravaLog(ex, string.Format("{0} -> {1}", "CVRPTW", json));

                    return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.InternalServerError,
                                                                   new Models.Response<string>()
                                                                   {
                                                                       status = false,
                                                                       msg = ex.Message
                                                                   });
                }
            }
            else
                return Request.CreateResponse<Models.Response<string>>(HttpStatusCode.Unauthorized,
                                                                       new Models.Response<string>()
                                                                       {
                                                                           status = false,
                                                                           msg = "Api Key inválida."
                                                                       });
        }

        #endregion

    }
}

