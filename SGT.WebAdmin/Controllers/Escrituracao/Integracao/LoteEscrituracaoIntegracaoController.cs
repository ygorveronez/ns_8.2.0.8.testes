using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais" }, "Escrituracao/LoteEscrituracao")]
    public class LoteEscrituracaoIntegracaoController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion


        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteEscrituracao;
                int.TryParse(Request.Params("LoteEscrituracao"), out codigoLoteEscrituracao);

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesLote = repLoteEscrituracaoIntegracao.BuscarTipoIntegracaoPorLoteEscrituracao(codigoLoteEscrituracao);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repLoteEscrituracaoEDIIntegracao.BuscarTipoIntegracaoPorLoteEscrituracao(codigoLoteEscrituracao);

                return new JsonpResult(new
                {
                    TiposIntegracoesLote = tiposIntegracoesLote,
                    TiposIntegracoesEDI = tiposIntegracoesEDI
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao))
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoLoteEscrituracao;
                int.TryParse(Request.Params("LoteEscrituracao"), out codigoLoteEscrituracao);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                //Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Escrituracao.LoteEscrituracao repLoteEscrituracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracao(unidadeDeTrabalho);
                //Repositorio.Embarcador.Cargas.CargaLog repLog = new Repositorio.Embarcador.Cargas.CargaLog(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao = repLoteEscrituracao.BuscarPorCodigo(codigoLoteEscrituracao);

                if (loteEscrituracao == null)
                    return new JsonpResult(true, false, "Escrituracao  não encontrada.");

                if (loteEscrituracao.Situacao !=  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.AgIntegracao)
                    return new JsonpResult(true, false, "A situação da Escrituracao não permite a finalização da etapa.");

                //if (loteEscrituracao.GerandoIntegracoes)
                //    return new JsonpResult(false, true, "O sistema ainda está gerando as integrações, não sendo possível finalizar a etapa. Aguarde alguns minutos e tente novamente.");

                unidadeDeTrabalho.Start();

                //Dominio.Entidades.Embarcador.Cargas.CargaLog log = new Dominio.Entidades.Embarcador.Cargas.CargaLog();

                //log.Acao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCarga.FinalizarEtapaIntegracao;
                //log.Carga = loteEscrituracao;
                //log.Data = DateTime.Now;
                //log.Usuario = Usuario;

                //repLog.Inserir(log);

                loteEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.Finalizado;


                repLoteEscrituracao.Atualizar(loteEscrituracao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteEscrituracao, null, "Finalizou.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                //svcHubCarga.InformarCargaAtualizada(loteEscrituracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
