using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.AcompanhamentoEntrega
{
    [CustomAuthorize("Cargas/AcompanhamentoEntregaTempoConfiguracao")]
    public class AcompanhamentoEntregaTempoConfiguracaoController : BaseController
    {
		#region Construtores

		public AcompanhamentoEntregaTempoConfiguracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaConfiguracao = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repAcompanhamentoEntregaConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);
                acompanhamentoEntregaConfiguracao = repAcompanhamentoEntregaConfiguracao.BuscarConfiguracao();

                return new JsonpResult(acompanhamentoEntregaConfiguracao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                unitOfWork.Start();
                bool inserir = false;

                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaConfiguracao = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repAcompanhamentoEntregaConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork);
                acompanhamentoEntregaConfiguracao = repAcompanhamentoEntregaConfiguracao.BuscarConfiguracao();

                if (acompanhamentoEntregaConfiguracao == null)
                {
                    inserir = true;
                    acompanhamentoEntregaConfiguracao = new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();
                }

                acompanhamentoEntregaConfiguracao.SaidaEmTempo = Request.GetTimeParam("SaidaEmTempo");
                acompanhamentoEntregaConfiguracao.SaidaAtraso1 = Request.GetTimeParam("SaidaAtraso1");
                acompanhamentoEntregaConfiguracao.SaidaAtraso2 = Request.GetTimeParam("SaidaAtraso2");
                acompanhamentoEntregaConfiguracao.SaidaAtraso3 = Request.GetTimeParam("SaidaAtraso3");
                acompanhamentoEntregaConfiguracao.EmtransitoEmTempo = Request.GetTimeParam("EmtransitoEmTempo");
                acompanhamentoEntregaConfiguracao.EmTrasitoAtraso1 = Request.GetTimeParam("EmTrasitoAtraso1");
                acompanhamentoEntregaConfiguracao.EmTrasitoAtraso2 = Request.GetTimeParam("EmTrasitoAtraso2");
                acompanhamentoEntregaConfiguracao.EmTrasitoAtraso3 = Request.GetTimeParam("EmTrasitoAtraso3");
                acompanhamentoEntregaConfiguracao.DestinoEmTempo = Request.GetTimeParam("DestinoEmTempo");
                acompanhamentoEntregaConfiguracao.DestinoAtraso1 = Request.GetTimeParam("DestinoAtraso1");
                acompanhamentoEntregaConfiguracao.DestinoAtraso2 = Request.GetTimeParam("DestinoAtraso2");
                acompanhamentoEntregaConfiguracao.DestinoAtraso3 = Request.GetTimeParam("DestinoAtraso3");

                if (inserir)
                    repAcompanhamentoEntregaConfiguracao.Inserir(acompanhamentoEntregaConfiguracao);
                else
                    repAcompanhamentoEntregaConfiguracao.Atualizar(acompanhamentoEntregaConfiguracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
