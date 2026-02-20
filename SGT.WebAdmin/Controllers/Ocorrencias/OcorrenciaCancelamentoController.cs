using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Ocorrencias
{
    [CustomAuthorize(new string[] { "ValidarOcorrencia" }, "Ocorrencias/OcorrenciaCancelamento")]
    public class OcorrenciaCancelamentoController : BaseController
    {
		#region Construtores

		public OcorrenciaCancelamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarCTesOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                int codOcorrencia = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codOcorrencia);

                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao)// se está com pendencia na emissão o sistema tente ver se autorizou os rejeitados para o fluxo andar.
                {
                    Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                    serOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, WebServiceConsultaCTe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, false, Auditado, WebServiceOracle, clienteMultisoftware: Cliente);
                }
                string propOrdenacao = "";

                Models.Grid.Grid grid = MontarGridCTes(ref propOrdenacao);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrencia.Codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCargaCTeComplementoInfo.ContarPorCTEsOcorrencia(ocorrencia.Codigo));

                grid.AdicionaRows(MontarListaCTes(cargaCTesComplementoInfo));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Ocorrencia"), out int codigoOcorrencia);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia tipoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento;

                if (!VerificaTipoDeCancelamento(codigoOcorrencia, unitOfWork, out tipoCancelamento, out string erro))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(new
                {
                    TipoCancelamento = tipoCancelamento
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreUmaFalhaAoValidarOsDados );
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Ocorrencias/OcorrenciaCancelamento");

                return AdicionarCancelamento(null, permissoesPersonalizadas, unitOfWork);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreuUmaFalhaAoGerarCancelamento );
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarComoCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Ocorrencias/OcorrenciaCancelamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.OcorrenciaCancelamento_AdicionarComoCancelamento))
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.VoceNaoPossuiPermissaoParaAdicionarCancelamento);

                return AdicionarCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento, permissoesPersonalizadas, unitOfWork);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreuUmaFalhaAoGerarCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoCancelamento);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (ocorrenciaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.RejeicaoCancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.SituacaoCancelamentoNaoPermiteReenvioMesmo);

                unitOfWork.Start();

                ocorrenciaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.EmCancelamento;
                ocorrenciaCancelamento.EnviouCTesParaCancelamento = false;
                RegistrarUsuarioEmissaoCTe(ocorrenciaCancelamento.Ocorrencia.Codigo, unitOfWork);

                repOcorrenciaCancelamento.Atualizar(ocorrenciaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrenciaCancelamento, null, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenviouParaCancelamento, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrenciaCancelamento.Ocorrencia, null, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenviouParaCancelamento, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreUmaFalhaAoReenviarCancelamento );
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void RegistrarUsuarioEmissaoCTe(int codigoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesOcorrencia = repCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(codigoOcorrencia);
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctesOcorrencia)
            {
                cte.UsuarioEmissaoCTe = Usuario;
                repCTe.Atualizar(cte);
            }
        }

        public async Task<IActionResult> ReenviarCancelamentoComoAnulacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Ocorrencias/OcorrenciaCancelamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.OcorrenciaCancelamento_ReenviarCancelamentoComoAnulacao))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.VoceNaoPossuiPermissaoParaEstaAcao);

                int.TryParse(Request.Params("Codigo"), out int codigoCancelamento);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (ocorrenciaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.RejeicaoCancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.SituacaoCancelamentoNaoPermiteReenvioMesmo);

                if (ocorrenciaCancelamento.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.TipoCancelamentoNaoPermiteQueEleSejaReenciadoComUmaaAnulacao);

                unitOfWork.Start();

                ocorrenciaCancelamento.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Anulacao;
                ocorrenciaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.EmCancelamento;
                ocorrenciaCancelamento.EnviouCTesParaCancelamento = false;

                repOcorrenciaCancelamento.Atualizar(ocorrenciaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrenciaCancelamento, null, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenviouCancelamentoOcorrenciaComoAnulacao, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrenciaCancelamento.Ocorrencia, null, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ReenviouCancelamentoOcorrenciaComoAnulacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreuUmaFalhaAoReenviarCancelamentoComAnulacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarSemInutilizarCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Ocorrencias/OcorrenciaCancelamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.OcorrenciaCancelamento_LiberarCancelamentoComCTeNaoInutilizado))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.VoceNaoPossuiPermissaoParaEstaAcao);

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorCodigo(codigo);

                if (ocorrenciaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.CancelamentoNaoEncontrado);

                if (ocorrenciaCancelamento.Situacao != SituacaoCancelamentoOcorrencia.RejeicaoCancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.SituacaoCancelamentoNaoPermiteLiberacao);

                unitOfWork.Start();

                ocorrenciaCancelamento.Situacao = SituacaoCancelamentoOcorrencia.EmCancelamento;
                ocorrenciaCancelamento.LiberarCancelamentoComCTeNaoInutilizado = true;

                repOcorrenciaCancelamento.Atualizar(ocorrenciaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrenciaCancelamento, null, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.LiberouCancelamentoOcorrenciaSemInutilizar, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreuUmaFalhaAoLiberarCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao repositorioOcorrenciaCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamentoIntegracao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorCodigo(codigo);

                if (ocorrenciaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.CancelamentoNaoEncontradoAtualizePaginaNovamente);

                /*
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoDocumentoCarga situacaoCTes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoDocumentoCarga.Sucesso;
                
                if (cargaCancelamento.Carga.CargaCTes.Count() > 0)
                {
                    if (cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && (o.CTe.ModeloDocumentoFiscal.Numero == "57" || o.CTe.ModeloDocumentoFiscal.Numero == "39") && (o.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento || o.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)))
                        situacaoCTes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoDocumentoCarga.Cancelando;
                    else if (cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && (o.CTe.ModeloDocumentoFiscal.Numero == "57" || o.CTe.ModeloDocumentoFiscal.Numero == "39") && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada && cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento))
                        situacaoCTes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoDocumentoCarga.Rejeicao;
                    else if (cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && (o.CTe.ModeloDocumentoFiscal.Numero == "57" || o.CTe.ModeloDocumentoFiscal.Numero == "39") && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada && o.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada && cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento))
                        situacaoCTes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoDocumentoCarga.Cancelando;
                }
                */

                bool possuiIntegracao = repositorioOcorrenciaCancelamentoIntegracao.ContarPorOcorrenciaCancelamento(ocorrenciaCancelamento.Codigo, situacao: null) > 0;
                bool possuiIntegracaoCTe = repOcorrenciaCTeCancelamentoIntegracao.ContarPorOcorrenciaCancelamento(ocorrenciaCancelamento.Codigo, situacao: null) > 0;
                bool permiteLiberarSemInutilizacao = false;

                if (ocorrenciaCancelamento.Situacao == SituacaoCancelamentoOcorrencia.RejeicaoCancelamento)
                    permiteLiberarSemInutilizacao = repCargaCTeComplementoInfo.ExisteCTesParaLiberarSemInutilizacao(ocorrenciaCancelamento.Ocorrencia.Codigo);

                var retorno = new
                {
                    ocorrenciaCancelamento.Codigo,
                    ocorrenciaCancelamento.Situacao,
                    permiteLiberarSemInutilizacao,
                    PossuiIntegracao = possuiIntegracao,
                    PossuiIntegracaoCTe = possuiIntegracaoCTe,
                    DadosCancelamento = new
                    {
                        ocorrenciaCancelamento.Codigo,
                        Ocorrencia = new { ocorrenciaCancelamento.Ocorrencia.Codigo, Descricao = ocorrenciaCancelamento.Ocorrencia.NumeroOcorrencia },
                        DataCancelamento = ocorrenciaCancelamento.DataCancelamento?.ToString("dd/MM/yyyy"),
                        UsuarioSolicitou = ocorrenciaCancelamento.Usuario?.Nome ?? string.Empty,
                        Motivo = ocorrenciaCancelamento.MotivoCancelamento,
                        ocorrenciaCancelamento.Tipo,
                    },
                    Documentos = new
                    {
                        ocorrenciaCancelamento.Codigo,
                        MensagemRejeicaoCancelamento = ocorrenciaCancelamento.MensagemRejeicaoCancelamento ?? string.Empty
                    },
                    Resumo = new
                    {
                        Ocorrencia = ocorrenciaCancelamento.Ocorrencia.NumeroOcorrencia,
                        DataCancelamento = ocorrenciaCancelamento.DataCancelamento?.ToString("dd/MM/yyyy"),
                        Operador = ocorrenciaCancelamento.Usuario?.Nome ?? string.Empty,
                        Tipo = ocorrenciaCancelamento.DescricaoTipo,
                        Situacao = ocorrenciaCancelamento.DescricaoSituacao,
                    },
                    //cargaCancelamento.MensagemRejeicaoCancelamento,
                    //SituacaoCTes = situacaoCTes
                    Anexos = (from anexo in ocorrenciaCancelamento.Anexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo,
                              }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorreUmaFalhaAoObterDetalhesCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "Ocorrencia") propOrdenar = "Ocorrencia.NumeroOcorrencia";
            else if (propOrdenar == "Solicitante") propOrdenar = "Usuario.Nome";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Ocorrencia, "Ocorrencia", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Carga, "Carga", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.TipoOcorrencia, "TipoOcorrencia", 17, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Solicitante, "Solicitante", 17, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.DataCancelamento, "DataCancelamento", 15, Models.Grid.Align.center, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Tipo, "Tipo", 10, Models.Grid.Align.left, true);
            else
                grid.AdicionarCabecalho("Tipo", false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Situacao, "Situacao", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("NumeroOcorrencia"), out int numeroOcorrencia);
            int.TryParse(Request.Params("NumeroCTe"), out int numeroCTe);
            int.TryParse(Request.Params("Operador"), out int codigoOperador);

            string numeroOS = Request.GetStringParam("NumeroOS");
            string numeroBooking = Request.GetStringParam("NumeroBooking");
            string numeroControle = Request.GetStringParam("NumeroControle");

            string carga = Request.Params("Carga") ?? string.Empty;
            List<int> codigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            List<double> codigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia situacaoAux))
                situacao = situacaoAux;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia? tipo = null;
            if (Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia tipoAux))
                tipo = tipoAux;

            // Consulta
            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> listaGrid = repOcorrenciaCancelamento.Consultar(numeroOcorrencia, numeroCTe, codigoOperador, carga, dataInicial, dataFinal, situacao, tipo, numeroOS, numeroBooking, numeroControle, codigosFiliais, codigosRecebedor, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repOcorrenciaCancelamento.ContarConsulta(numeroOcorrencia, numeroCTe, codigoOperador, carga, dataInicial, dataFinal, situacao, tipo, numeroOS, numeroBooking, numeroControle, codigosFiliais, codigosRecebedor);

            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Ocorrencia = obj.Ocorrencia.NumeroOcorrencia,
                             Carga = obj.Ocorrencia.Carga?.CodigoCargaEmbarcador,
                             TipoOcorrencia = obj.Ocorrencia.TipoOcorrencia?.Descricao,
                             Solicitante = obj.Usuario?.Nome ?? string.Empty,
                             DataCancelamento = obj.DataCancelamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                             Situacao = obj.DescricaoSituacao,
                             Tipo = obj.DescricaoTipo,
                         }).ToList();

            return lista.ToList();
        }

        private bool VerificaTipoDeCancelamento(int codigoOcorrencia, Repositorio.UnitOfWork unitOfWork, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia tipoCancelamento, out string erro)
        {
            erro = "";
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            tipoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento;

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> ctesOcorrencia = repCargaCTeComplementoInfo.BuscarPorOcorrencia(codigoOcorrencia);

            if (ocorrencia == null)
            {
                erro = Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorrenciaNaoEncontrada;
                return false;
            }

            if (ctesOcorrencia.Any(infoCte =>
            infoCte.CTe != null &&
            infoCte.CTe.ModeloDocumentoFiscal.Numero == "57" &&
            infoCte.CTe.Status == "A" &&
            repCargaCTe.ObterSistemaEmissorCTeOcorrencia(infoCte.Codigo) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe &&
            ((infoCte.CTe.LocalidadeEmissao.Estado.Sigla != "MT" &&
              infoCte.CTe.DataRetornoSefaz < DateTime.Now.AddDays(-7)) ||
             (infoCte.CTe.LocalidadeEmissao.Estado.Sigla == "MT" &&
              infoCte.CTe.DataRetornoSefaz < DateTime.Now.AddHours(-2)))))
            {
                tipoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Anulacao;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> nfses = ctesOcorrencia.Where(infoCTe => infoCTe.CTe != null && repCargaCTe.ObterSistemaEmissorCTeOcorrencia(infoCTe.Codigo) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && infoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && infoCTe.CTe.Status == "A").ToList();

                if (nfses.Count > 0)
                {
                    bool permiteAnularNFSe = true;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in nfses)
                    {
                        //Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, cargaCTeComplementoInfo.CTe.LocalidadeInicioPrestacao.Codigo) ?? repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, 0);
                        Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, cargaCTeComplementoInfo.CTe.LocalidadeInicioPrestacao.Codigo, cargaCTeComplementoInfo.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", cargaCTeComplementoInfo.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, cargaCTeComplementoInfo.CTe.TomadorPagador.Localidade?.Codigo ?? 0);
                        if (configuracaoNFSe == null)
                            configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, cargaCTeComplementoInfo.CTe.LocalidadeInicioPrestacao.Codigo, cargaCTeComplementoInfo.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", 0, 0);
                        if (configuracaoNFSe == null)
                            configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, cargaCTeComplementoInfo.CTe.LocalidadeInicioPrestacao.Codigo, "", cargaCTeComplementoInfo.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, 0);
                        if (configuracaoNFSe == null)
                            configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, 0, cargaCTeComplementoInfo.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", cargaCTeComplementoInfo.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, 0);
                        if (configuracaoNFSe == null)
                            configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, 0, "", 0, cargaCTeComplementoInfo.CTe.TomadorPagador.Localidade?.Codigo ?? 0);
                        if (configuracaoNFSe == null)
                            configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTeComplementoInfo.CTe.Empresa.Codigo, 0, "", 0, 0);

                        if (configuracaoNFSe == null || !configuracaoNFSe.PermiteAnular || cargaCTeComplementoInfo.CTe.DataRetornoSefaz > DateTime.Now.AddDays(-configuracaoNFSe.PrazoCancelamento))
                        {
                            permiteAnularNFSe = false;
                            break;
                        }
                    }

                    if (permiteAnularNFSe)
                        tipoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Anulacao;
                }
            }

            //if (tipoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Anulacao && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            //{
            //    erro = "Não é possível cancelar CT-e(s) que foram emitidos a mais de 7 dias.";
            //    return false;
            //}

            return true;
        }

        private JsonpResult AdicionarCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia? tipoCancelamentoFixo, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Ocorrencia serCargaOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

            int.TryParse(Request.Params("Ocorrencia"), out int codigoOcorrencia);
            string motivo = Request.Params("Motivo") ?? string.Empty;

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamentoExistente = repOcorrenciaCancelamento.BuscarPorOcorrencia(codigoOcorrencia);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia tipoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento;

            string erro = string.Empty;

            if (tipoCancelamentoFixo.HasValue)
                tipoCancelamento = tipoCancelamentoFixo.Value;
            else
            {
                if (!VerificaTipoDeCancelamento(codigoOcorrencia, unitOfWork, out tipoCancelamento, out erro))
                    return new JsonpResult(false, true, erro);
            }

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 20)
                return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.MotivoDevePossuirVinteCaracteres);

            if (ocorrencia == null)
                return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.OcorrenciaNaoEncontrada);

            if (ocorrenciaCancelamentoExistente != null)
                return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.JaExisteSolicitacaoCancelamentoParaEstaOcorrencia);

            if (tipoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Anulacao && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.OcorrenciaCancelamento_Anular))
                return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.VoceNaoPossuiPermissaoParaRealizarAnulacaoCarga);

            if (!serCargaOcorrencia.VerificarSeOcorrenciaPermiteCancelamento(out string mensagem, ocorrencia, unitOfWork, TipoServicoMultisoftware))
                return new JsonpResult(false, true, mensagem);

            if (ocorrencia.OcorrenciaRecebidaDeIntegracao)
                return new JsonpResult(false, true, Localization.Resources.Ocorrencias.OcorrenciaCancelamento.NaoEPossivelCancelarUmaOcorrenciaRecebidaViaIntegraca);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento
            {
                Ocorrencia = ocorrencia,
                DataCancelamento = DateTime.Now,
                MotivoCancelamento = motivo,
                Tipo = tipoCancelamento,
                Usuario = Usuario,
                SituacaoOcorrenciaNoCancelamento = ocorrencia.SituacaoOcorrencia,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.EmCancelamento,
                EnviouCTesParaCancelamento = false
            };

            repOcorrenciaCancelamento.Inserir(ocorrenciaCancelamento, Auditado);
            RegistrarUsuarioEmissaoCTe(ocorrencia.Codigo, unitOfWork);

            unitOfWork.CommitChanges();

            return new JsonpResult(new
            {
                ocorrenciaCancelamento.Codigo
            });
        }

        private Models.Grid.Grid MontarGridCTes(ref string propOrdenacao)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoCTe", false);
            grid.AdicionarCabecalho("CodigoCTE", false);
            grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
            grid.AdicionarCabecalho("CodigoEmpresa", false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Numero, "Numero", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Serie, "Serie", 8, Models.Grid.Align.center, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Tomador, "Tomador", 18, Models.Grid.Align.left, true);
            }
            else
            {
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Doc, "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Remetente, "Remetente", 16, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Destinatario, "Destinatario", 16, Models.Grid.Align.left, true);
            }

            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.ValorReceber, "ValorFrete", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Aliquota, "Aliquota", 5, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.Status, "Status", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.OcorrenciaCancelamento.RetornoSEFAZ, "RetornoSefaz", 15, Models.Grid.Align.left, false, true);

            return grid;
        }

        private dynamic MontarListaCTes(List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo)
        {
            var lista = (from obj in cargaCTesComplementoInfo
                         where obj.CTe != null
                         select new
                         {
                             obj.Codigo,
                             CodigoCTE = obj.CTe.Codigo,
                             obj.CTe.DescricaoTipoServico,
                             NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                             AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                             CodigoEmpresa = obj.CTe.Empresa.Codigo,
                             obj.CTe.Numero,
                             SituacaoCTe = obj.CTe.Status,
                             Serie = obj.CTe.Serie.Numero,
                             Remetente = obj.CTe.Remetente?.Descricao,
                             Destinatario = obj.CTe.Destinatario?.Descricao,
                             ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                             Aliquota = obj.CTe.AliquotaICMS > 0 ? obj.CTe.AliquotaICMS.ToString("n2") : obj.CTe.AliquotaISS.ToString("n4"),
                             Status = obj.CTe.DescricaoStatus,
                             RetornoSefaz = !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "") : "",
                             Tomador = obj.CTe.TomadorPagador != null ? obj.CTe.TomadorPagador.Nome + "(" + obj.CTe.TomadorPagador.CPF_CNPJ_Formatado + ")" : string.Empty,
                         }).ToList();

            return lista;
        }

        #endregion
    }
}
