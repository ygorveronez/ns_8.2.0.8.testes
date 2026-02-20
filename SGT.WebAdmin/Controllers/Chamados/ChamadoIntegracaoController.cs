using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ChamadoOcorrencia")]
    public class ChamadoIntegracaoController : BaseController
    {
		#region Construtores

		public ChamadoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao integracao = repChamadoIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repChamadoIntegracao.BuscarArquivoHistoricoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(false, true, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao integracao = repChamadoIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                if (integracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Não é possível integrar nessa situação!");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = integracao.Chamado;
                if (chamado.Situacao != SituacaoChamado.FalhaIntegracao && chamado.Situacao != SituacaoChamado.Finalizado)
                    return new JsonpResult(false, true, "O atendimento não está mais com falha, não sendo possível integrar nessa situação!");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                if (chamado.Situacao != SituacaoChamado.Finalizado)//Não altera a situação do atendimento quando já foi finalizado com integração rejeitada
                {
                    chamado.Situacao = SituacaoChamado.AgIntegracao;
                    repChamado.Atualizar(chamado);
                }

                repChamadoIntegracao.Atualizar(integracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, "Solicitou o reenvio da integração", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao integrar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Arquivo", "NomeArquivo", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integradora", "Integradora", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Senha SAP", "SenhaSAP", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status Devolução", "StatusDevolucao", 15, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                TipoIntegracao tipoIntegracaoNotaFiscal = TipoIntegracao.JJ;
                TipoIntegracao tipoIntegracaoArquivo = TipoIntegracao.Isis;

                Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> listaIntegracoes = repChamadoIntegracao.Consultar(codigo, situacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repChamadoIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        integracao.NomeArquivo,
                        NotaFiscal = integracao.CargaEntregaNotaFiscal?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                        Integradora = integracao.TipoIntegracao.Tipo.ObterDescricao(),
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        SenhaSAP = integracao.SenhaDevolucao,
                        StatusDevolucao = integracao.StatusDevolucao,
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                if (!listaIntegracoes.Any(o => o.TipoIntegracao.Tipo == tipoIntegracaoNotaFiscal))
                    grid.OcultarCabecalho("NotaFiscal");

                if (!listaIntegracoes.Any(o => o.TipoIntegracao.Tipo == tipoIntegracaoArquivo))
                    grid.OcultarCabecalho("NomeArquivo");

                if (!listaIntegracoes.Any(o => o.TipoIntegracao.Tipo == TipoIntegracao.JJ))
                {
                    grid.OcultarCabecalho("SenhaSAP");
                    grid.OcultarCabecalho("StatusDevolucao");
                }

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar as integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repChamadoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repChamadoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repChamadoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repChamadoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarComIntegracaoRejeitada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Servicos.Embarcador.Chamado.Chamado serChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Situacao != SituacaoChamado.FalhaIntegracao)
                    return new JsonpResult(false, true, "Só é possível liberar quando a integração estiver rejeitada.");

                unitOfWork.Start();

                serChamado.FinalizarChamadoAnaliseDevolucao(chamado, unitOfWork, Auditado, TipoServicoMultisoftware, Cliente);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, "Finalizou mesmo com integração rejeitada", unitOfWork);

                unitOfWork.CommitChanges();

                if ((chamado.Situacao == SituacaoChamado.Finalizado || chamado.Situacao == SituacaoChamado.LiberadaOcorrencia) && chamado.CargaEntrega != null && chamado.CargaEntrega.Carga != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(chamado.CargaEntrega.Carga.Codigo, chamado.Codigo);
                    if (cargaEvento != null)
                    {
                        servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                    }
                }

                serChamado.EnviarEmailChamadoFinalizado(chamado, unitOfWork);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao liberar integração rejeitada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarMultiplasIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento repositorioChamadoInformacaoFechamento = new Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoIntegracao repositorioChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                int codigoChamado = Request.GetIntParam("Codigo");

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> listaChamadoIntegracao = repositorioChamadoIntegracao.BuscarPorChamado(codigoChamado);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado, true);

                int quantiaIntegrada = 0;
                int quantiaNaoIntegrada = 0;

                foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao in listaChamadoIntegracao)
                {
                    if (chamadoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado || chamadoIntegracao.SituacaoIntegracao == SituacaoIntegracao.AgRetorno)
                        quantiaIntegrada++;
                    else
                        quantiaNaoIntegrada++;
                }

                if (quantiaIntegrada == 0 && quantiaNaoIntegrada == 0 && repositorioChamadoInformacaoFechamento.PossuiPorChamado(codigoChamado))
                {
                    servicoChamado.GerarIntegracoes(chamado, unitOfWork, Auditado, TipoServicoMultisoftware);
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> listaChamadoIntegracaoRejeitada = repositorioChamadoIntegracao.BuscarPorCodigoChamadoSituacao(codigoChamado, SituacaoIntegracao.ProblemaIntegracao);

                    if (listaChamadoIntegracaoRejeitada?.Count() == 0)
                        return new JsonpResult(false, true, "Nenhuma integração com falha foi encontrada.");

                    foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao integracao in listaChamadoIntegracao)
                    {
                        if (integracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                            continue;

                        integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                        repositorioChamadoIntegracao.Atualizar(integracao);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, "Solicitou o reenvio das integrações", unitOfWork);
                    }
                }

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o reenvio de múltiplas integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
