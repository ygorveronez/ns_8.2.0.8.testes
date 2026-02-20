using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class BIController : ApiSemAutenticacaoController
    {
        private static int _reportID;
        private Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport GetConfiguracaoBIReport(int codigoReport, Repositorio.UnitOfWork unitOfWork)
        {            
            try
            {
                var rep = new Repositorio.Embarcador.BI.ConfigracaoBIReport(unitOfWork);

                return rep.BuscarReport(codigoReport);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "BI");
                return null;
            }
        }

        private Dominio.Entidades.Embarcador.BI.ConfiguracaoBI GetConfiguracaoBI(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                var rep = new Repositorio.Embarcador.BI.ConfigracaoBI(unitOfWork);

                return rep.BuscarPadrao();


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "BI");
                return null;
            }

        }

        private Models.BI.EmbedParms GetReportParams()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                var configBI = GetConfiguracaoBI(unitOfWork);
                var configBIReport = GetConfiguracaoBIReport(_reportID,unitOfWork);


                var embededparam = new Models.BI.EmbedParms
                {
                    ApplicationId = configBI.ApplicationId,
                    Username = configBI.UserName,
                    Password = configBI.Password,
                    WorkspaceId = configBIReport.WorkspaceID,
                    ReportID = configBIReport.ReportID
                };

                if (configBIReport.TokenExpiration.HasValue && !string.IsNullOrWhiteSpace(configBIReport.Token) && !string.IsNullOrWhiteSpace(configBIReport.TokenId))
                    embededparam.EmbedToken = new Microsoft.PowerBI.Api.V2.Models.EmbedToken(configBIReport.Token, configBIReport.TokenId, configBIReport.TokenExpiration);

                return embededparam;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Models.BI.EmbedConfig GetReport(Repositorio.UnitOfWork unitOfWork)
        {

            var _embedService = new EmbedService(GetReportParams());


            Task.Run(async () => { var emb = await _embedService.EmbedReport(); }).Wait();


            return _embedService.EmbedConfig;
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult Report()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["ID"], out int ID);

                _reportID = ID;

                Models.BI.EmbedConfig report = GetReport(unitOfWork);

                if (!string.IsNullOrWhiteSpace(report.ErrorMessage))
                    return Json<bool>(false, false, report.ErrorMessage); //return Json(new { Sucesso = false, Erro = report.ErrorMessage });                

                var retorno = new
                {
                    embedUrl = report.EmbedUrl,
                    accessToken = report.EmbedToken.Token,
                    embedReportId = report.Id
                };

                return Json(retorno, true); //return Json(new { Sucesso = true, Objeto = retorno, JsonRequestBehavior.AllowGet });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "BI");
                return Json<bool>(false, false, "Não disponível."); //return Json(new { Sucesso = false, Erro = "Visão não disponível." });
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}