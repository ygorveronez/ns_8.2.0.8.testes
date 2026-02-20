using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoEncerramentoCarga
{
    [CustomAuthorize("Cargas/FluxoEncerramentoCarga")]
    public class FluxoEncerramentoCargaController : BaseController
    {
        #region Construtores

        public FluxoEncerramentoCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repcargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFluxoEncerramentoCarga filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "DataEncerramento", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motivo", "Motivo", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento> cargasRegistroEncerramento = repcargaRegistroEncerramento.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repcargaRegistroEncerramento.ContarConsulta(filtrosPesquisa));

                var retorno = (from obj in cargasRegistroEncerramento
                               select new
                               {
                                   obj.Codigo,
                                   Carga = obj.Carga.CodigoCargaEmbarcador,
                                   DataEncerramento = obj.DataEncerramento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                   Remetente = obj.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                                   Destinatario = obj.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                                   Situacao = obj.Situacao.Descricao(),
                                   Motivo = obj.NotaEncerramento ?? string.Empty,
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarEncerramento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                string motivoEncerramento = Request.GetStringParam("Motivo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Carga não foi encontrada.");

                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = new Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento();

                cargaRegistroEncerramento.Carga = carga;
                cargaRegistroEncerramento.NotaEncerramento = motivoEncerramento;
                cargaRegistroEncerramento.Usuario = this.Usuario;
                cargaRegistroEncerramento.DataEncerramento = null;
                cargaRegistroEncerramento.Situacao = SituacaoEncerramentoCarga.AgEncerramentoDocumentos;

                repCargaRegistroEncerramento.Inserir(cargaRegistroEncerramento, Auditado);

                return new JsonpResult(cargaRegistroEncerramento.Codigo);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o encerramento.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCodigo(codigo);

                if (cargaRegistroEncerramento == null)
                    return new JsonpResult(false, true, "Registro de encerramento não encontrado.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga situacaoCIOT = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga.Sucesso;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga situacaoMDFes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga.Sucesso;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga situacaoIntegracoes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga.Sucesso;

                if (situacaoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga.Sucesso)
                {
                    if (cargaRegistroEncerramento.Situacao == SituacaoEncerramentoCarga.AgEncerramentoCIOT)
                        situacaoCIOT = SituacaoEncerramentoDocumentosCarga.Encerrando;
                    else if (cargaRegistroEncerramento.Situacao == SituacaoEncerramentoCarga.RejeicaoEncerramento && cargaRegistroEncerramento.Carga.CargaCIOTs.Any(o => o.CIOT != null && o.CIOT.Situacao != SituacaoCIOT.Encerrado && o.CIOT.Situacao != SituacaoCIOT.Cancelado))
                        situacaoCIOT = SituacaoEncerramentoDocumentosCarga.Rejeicao;
                }

                var statusMDFes = BuscarStatusEncerramentoMDFe(cargaRegistroEncerramento.Carga?.CargaMDFes);

                if (situacaoMDFes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga.Sucesso)
                {
                    if (cargaRegistroEncerramento.Situacao == SituacaoEncerramentoCarga.AgEncerramentoMDFe)
                        situacaoMDFes = statusMDFes;
                    else if (cargaRegistroEncerramento.Situacao == SituacaoEncerramentoCarga.RejeicaoEncerramento && cargaRegistroEncerramento.Carga.CargaMDFes.Any(o => o.MDFe != null && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao && o.SistemaEmissor != SistemaEmissor.OutrosEmissores))
                        situacaoMDFes = statusMDFes;

                    if (situacaoMDFes != statusMDFes)
                        situacaoMDFes = statusMDFes;
                }

                if (situacaoIntegracoes == SituacaoEncerramentoDocumentosCarga.Sucesso)
                {
                    if (cargaRegistroEncerramento.Situacao == SituacaoEncerramentoCarga.AgIntegracao)
                        situacaoIntegracoes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga.Encerrando;
                    //else if (integracoes.Any(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) ||
                    //         repcargaRegistroEncerramentoCargaIntegracao.ContarPorSituacaoConsulta(cargaRegistroEncerramento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                    //    situacaoIntegracoes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga.Rejeicao;
                }

                var retorno = new
                {
                    cargaRegistroEncerramento.Codigo,
                    Carga = new
                    {
                        cargaRegistroEncerramento.Carga.Codigo,
                        cargaRegistroEncerramento.Carga.CodigoCargaEmbarcador,
                        Remetente = cargaRegistroEncerramento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                        Origem = cargaRegistroEncerramento.Carga.DadosSumarizados?.Origens ?? string.Empty,
                        Destinatario = cargaRegistroEncerramento.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                        Destino = cargaRegistroEncerramento.Carga.DadosSumarizados?.Destinos ?? string.Empty,
                        Situacao = cargaRegistroEncerramento.Carga.DescricaoSituacaoCarga
                    },
                    Motivo = cargaRegistroEncerramento.NotaEncerramento,
                    cargaRegistroEncerramento.DescricaoSituacao,
                    DataEncerramento = cargaRegistroEncerramento.DataEncerramento?.ToString("dd/MM/yyyy HH:mm"),
                    Situacao = cargaRegistroEncerramento.Situacao,
                    SituacaoMDFes = situacaoMDFes,
                    SituacaoCIOT = situacaoCIOT,
                    SituacaoIntegracoes = situacaoIntegracoes,
                    MotivoRejeicao = cargaRegistroEncerramento.MotivoRejeicao ?? string.Empty,
                    PossuiIntegracao = (bool?)cargaRegistroEncerramento.PossuiIntegracao ?? false,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaObterDetalhesCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                return new JsonpResult(new
                {
                    EmTransporte = carga.SituacaoCarga == SituacaoCarga.EmTransporte,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Falha ao validar a carga.");
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
                int codigoEncerramento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaEncerramento.BuscarPorCodigo(codigoEncerramento);

                if (cargaRegistroEncerramento.Situacao != SituacaoEncerramentoCarga.RejeicaoEncerramento)
                    return new JsonpResult(false, true, "Situação não permite reenvio");

                unitOfWork.Start();

                cargaRegistroEncerramento.Situacao = SituacaoEncerramentoCarga.AgEncerramentoDocumentos;

                repCargaEncerramento.Atualizar(cargaRegistroEncerramento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaRegistroEncerramento, null, "Reenviou encerramento carga", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaRegistroEncerramento.Carga, null, "Reenviou encerramento carga", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Falha ao reenviar encerramento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFluxoEncerramentoCarga ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFluxoEncerramentoCarga filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFluxoEncerramentoCarga()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetNullableEnumParam<SituacaoEncerramentoCarga>("Situacao"),
            };

            return filtroPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                propriedadeOrdenar = "Carga.Codigo";

            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoDocumentosCarga BuscarStatusEncerramentoMDFe(IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes)
        {
            if (cargaMDFes.Count() == 0)
                return SituacaoEncerramentoDocumentosCarga.Sucesso;

            if (cargaMDFes.Any(o => o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado))
                return SituacaoEncerramentoDocumentosCarga.Rejeicao;

            return SituacaoEncerramentoDocumentosCarga.Sucesso;
        }

        #endregion
    }
}
