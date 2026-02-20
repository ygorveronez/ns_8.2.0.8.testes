using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGT.WebAdmin.Models.BI;


namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.PB
{
    [Area("BusinessIntelligence")]
    public class BIController : BaseController
    {
        #region Construtores

        public BIController(Conexao conexao) : base(conexao)
        {

        }

        #endregion

        private static int _reportID;
        private Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport GetConfiguracaoBIReport(int codigoReport, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.BI.ConfigracaoBIReport repConfigracaoBIReport = new Repositorio.Embarcador.BI.ConfigracaoBIReport(unitOfWork);

            return repConfigracaoBIReport.BuscarReport(codigoReport);
        }

        private Dominio.Entidades.Embarcador.BI.ConfiguracaoBI GetConfiguracaoBI(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.BI.ConfigracaoBI repConfigracaoBI = new Repositorio.Embarcador.BI.ConfigracaoBI(unitOfWork);

            return repConfigracaoBI.BuscarPadrao();
        }

        private EmbedParms GetReportParams(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.BI.ConfiguracaoBI configBI = GetConfiguracaoBI(unitOfWork);
            Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport configBIReport = GetConfiguracaoBIReport(_reportID, unitOfWork);

            EmbedParms embededparam = new EmbedParms
            {
                ApplicationId = configBI.ApplicationId,
                Username = configBI.UserName,
                Password = configBI.Password,
                WorkspaceId = Guid.Parse(configBIReport.WorkspaceID),
                ReportID = Guid.Parse(configBIReport.ReportID),
                CodigoFormulario = configBIReport.CodigoFormulario,
                TokenAutentication = configBIReport.TokenAutentication,
            };

            if (configBIReport.TokenExpiration.HasValue && !string.IsNullOrWhiteSpace(configBIReport.Token) && !string.IsNullOrWhiteSpace(configBIReport.TokenId))
                embededparam.EmbedToken = new Microsoft.PowerBI.Api.Models.EmbedToken(configBIReport.Token,
                                                                                      Guid.Parse(configBIReport.TokenId),
                                                                                      configBIReport.TokenExpiration.Value);

            return embededparam;
        }

        public EmbedConfig GetReport(Repositorio.UnitOfWork unitOfWork)
        {

            EmbedService _embedService = new EmbedService(GetReportParams(unitOfWork));

            int idUsuario = this.Usuario.Codigo;
            string cnpjClienteFornecedor = this.Usuario.ClienteFornecedor?.CPF_CNPJ_SemFormato ?? "";
            string cnpjTransportador = this.Usuario.Empresa?.CNPJ_SemFormato ?? "";

            Task.Run(async () => { bool emb = await _embedService.EmbedReport(idUsuario, cnpjClienteFornecedor, cnpjTransportador, unitOfWork); }).Wait();

            return _embedService.EmbedConfig;

        }

        [CustomAuthorize("BusinessIntelligence/BI/Report")]
        public async Task<IActionResult> Report(int ID)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Int16 UtilizaMenu = 0;
                Int16.TryParse(Request.Params("UtilizaMenu"), out UtilizaMenu);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repPortal = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortal = repPortal.BuscarPrimeiroRegistro();

                _reportID = ID;
                ViewBag.Message = GetReport(unitOfWork);
                ViewBag.UtilizaMenu = UtilizaMenu;
                ViewBag.DesabilitarFiltrosBI = configuracaoPortal?.DesabilitarFiltrosBI ?? false;

                return View();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "BI");
                unitOfWork.Dispose();
                throw;
            }
        }

        //[CustomAuthorize("BusinessIntelligence/BI/Report")]
        //public async Task<IActionResult> Report(int ID)
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI repositorioPermissaoAcessoUsuarioBI = new Repositorio.Embarcador.BI.PermissaoAcessoUsuarioBI(unitOfWork);

        //        Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI permissaoAcessoUsuarioBI = repositorioPermissaoAcessoUsuarioBI.BuscarPorUsuarioECodigoReportBI(Usuario.Codigo, ID);

        //        _reportID = ID;

        //        if (permissaoAcessoUsuarioBI != null)
        //            ViewBag.Message = GetReport(unitOfWork);

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex, "BI");
        //        unitOfWork.Dispose();
        //        throw ex;
        //    }
        //}
    }
}
