using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Encerramento
{
    [CustomAuthorize("Cargas/EncerramentoCarga")]
    public class EncerramentoCargaController : BaseController
    {
		#region Construtores

		public EncerramentoCargaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> EncerrarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/EncerramentoCarga");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.EncerramentoCarga_EncerrarCarga))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                int codCarga = int.Parse(Request.Params("Carga"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFE = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Logistica.GuaritaTMS repGuaritaTMS = new Repositorio.Embarcador.Logistica.GuaritaTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFE.BuscarPorCarga(carga.Codigo);

                if (ConfiguracaoEmbarcador.ObrigarTerGuaritaParaLancamentoEFinalizacaoCarga)
                {
                    if (!repGuaritaTMS.ContemRegistroGuaritaCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada))
                        return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoEncontradoRegistroEntradaGuaritaFavorRealizeLancamento);
                }

                if (carga.TipoOperacao?.HabilitarCobrancaEstadiaAutomaticaPeloTracking ?? false)
                {
                    if (carga.DataInicioViagem == null || carga.DataFimViagem == null)
                        return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoPossivelEncerrarCargaNaoTerDataInicioViagemDataFinalViagem);
                }

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte && !carga.CargaEmitidaParcialmente)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, Localization.Resources.Cargas.EncerramentoCarga.SolicitouEncerramentoCarga, unitOfWork);
                    carga.DataEncerramentoCarga = DateTime.Now;
                    repCarga.Atualizar(carga);

                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    if (cargaMDFEs.Any(obj => obj.MDFe != null && (ConfiguracaoEmbarcador.PermiteEncerrarMDFeEmitidoNoEmbarcador || obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
                    && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao))
                    {
                        serCarga.SolicitarEncerramentoMDFeAutomaticamente(carga, unitOfWork, TipoServicoMultisoftware, WebServiceConsultaCTe, Auditado, this.Usuario);
                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        Repositorio.Embarcador.Cargas.CargaNFS repCargaNFS = new Repositorio.Embarcador.Cargas.CargaNFS(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> cargaNFSs = repCargaNFS.BuscarPorCarga(carga.Codigo);

                        if (cargaNFSs.Count == 0 || !cargaNFSs.Exists(obj => obj.NotaFiscalServico == null))
                        {

                            serCarga.LiberarSituacaoDeCargaFinalizada(carga, unitOfWork, TipoServicoMultisoftware, Auditado, this.Usuario);
                            unitOfWork.CommitChanges();
                            return new JsonpResult(true);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, Localization.Resources.Cargas.EncerramentoCarga.NaopossivelEncerrarCargaExisteNFSnaoInformada);
                        }
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    if (!carga.CargaEmitidaParcialmente)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.EncerramentoCarga.NaoPossivelEncerrarCargaAtualSituacao, carga.DescricaoSituacaoCarga));
                    else
                        return new JsonpResult(false, true, Localization.Resources.Cargas.EncerramentoCarga.NaoPossivelEncerrarCargaEstejaParcialmenteEmitida);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.EncerramentoCarga.OcorreuFalhaBuscarDadosEncerramentoMDFe);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        #endregion

        #region Métodos

        #endregion

    }
}
