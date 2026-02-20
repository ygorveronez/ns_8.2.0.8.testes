using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    [CustomAuthorize("TorreControle/DetalhesTorre")]
    public class DetalhesTorreController : BaseController
    {
        #region Construtores

        public DetalhesTorreController(Conexao conexao) : base(conexao)
        {
        }

        #endregion

        #region Atributos protegidos

        protected const string MASCARA_DATA_HM = "dd/MM/yyyy HH:mm";
        protected const string VALOR_NULO = "-";

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.TorreControle.DetalhesTorre servicoDetalhesTorre = new Servicos.Embarcador.TorreControle.DetalhesTorre(unitOfWork, cancellationToken);
                int codigo = Request.GetIntParam("codigo");
                return new JsonpResult(await servicoDetalhesTorre.ObterDadosPesquisaAsync(codigo));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.FalhaConsulta);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarCamposDetalhesTorre(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.TorreControle.DetalhesTorre servicoDetalhesTorre = new Servicos.Embarcador.TorreControle.DetalhesTorre(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.TorreControle.DadosAtualizarDetalhesTorre dadosAtualizarDetalhesTorre = new Dominio.ObjetosDeValor.Embarcador.TorreControle.DadosAtualizarDetalhesTorre
                {
                    Codigo = Request.GetIntParam("codigo"),
                    Critico = Request.GetBoolParam("Critico"),
                    Observacao = Request.GetStringParam("Observacao"),
                    CodigoStatusViagem = Request.GetIntParam("Status"),
                    DataInicioStatus = Request.GetDateTimeParam("DataInicialStatus")
                };
                await servicoDetalhesTorre.AtualizarCamposDetalhesTorre(dadosAtualizarDetalhesTorre, TipoServicoMultisoftware, Cliente, Auditado);
                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.CamposAtualizados);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.FalhaAtualizacao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion
    }
}