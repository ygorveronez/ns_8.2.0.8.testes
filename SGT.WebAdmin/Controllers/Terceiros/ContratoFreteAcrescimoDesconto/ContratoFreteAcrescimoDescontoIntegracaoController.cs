using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros.ContratoFreteAcrescimoDesconto
{
    [CustomAuthorize("Terceiros/ContratoFreteAcrescimoDesconto")]
    public class ContratoFreteAcrescimoDescontoIntegracaoController : BaseController
    {
		#region Construtores

		public ContratoFreteAcrescimoDescontoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("TipoIntegracao", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao contratoFreteIntegracaoArquivo = repContratoFreteAcrescimoDescontoIntegracao.BuscarPorCodigo(codigo, false);
                if (contratoFreteIntegracaoArquivo.TipoIntegracao.Tipo == TipoIntegracao.CIOT)
                {
                    List<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo> integracoesArquivos = repContratoFreteAcrescimoDescontoIntegracao.BuscarArquivosPorIntegracao(codigo, grid.inicio, grid.limite);
                    grid.setarQuantidadeTotal(repContratoFreteAcrescimoDescontoIntegracao.ContarBuscarArquivosPorIntegracao(codigo));

                    var retorno = (from obj in integracoesArquivos
                                   select new
                                   {
                                       TipoIntegracao = TipoIntegracao.CIOT,
                                       obj.Codigo,
                                       Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                       obj.DescricaoTipo,
                                       obj.Mensagem
                                   }).ToList();

                    grid.AdicionaRows(retorno);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(contratoFreteIntegracaoArquivo.ContratoFreteAcrescimoDesconto.ContratoFrete.Codigo);
                    grid.setarQuantidadeTotal(contratoFrete.ArquivosTransacao.Count());

                    var retorno = (from obj in contratoFrete.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                                   select new
                                   {
                                       TipoIntegracao = contratoFreteIntegracaoArquivo?.TipoIntegracao?.Tipo ?? TipoIntegracao.NaoInformada,
                                       obj.Codigo,
                                       Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                       obj.DescricaoTipo,
                                       obj.Mensagem
                                   }).ToList();

                    grid.AdicionaRows(retorno);
                }

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
                TipoIntegracao tipoIntegracao = Request.GetEnumParam<TipoIntegracao>("TipoIntegracao");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(unitOfWork);
                if (tipoIntegracao == TipoIntegracao.CIOT)
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo arquivoIntegracao = repContratoFreteAcrescimoDescontoIntegracao.BuscarIntegracaoCIOTPorCodigo(codigo);

                    if (arquivoIntegracao == null)
                        return new JsonpResult(true, false, "Histórico não encontrado.");

                    if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                        return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                    byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                    return Arquivo(arquivo, "application/zip", "Arquivos Integração.zip");
                }
                else
                {
                    Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);
                    Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo arquivoIntegracao = repContratoFreteIntegracaoArquivo.BuscarPorCodigo(codigo);

                    if (arquivoIntegracao == null)
                        return new JsonpResult(true, false, "Histórico não encontrado.");

                    if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                        return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                    byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                    return Arquivo(arquivo, "application/zip", "Arquivos Integração.zip");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
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

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracao = repContratoFreteAcrescimoDescontoIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                if (integracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Não é possível integrar nessa situação!");

                if (integracao.ContratoFreteAcrescimoDesconto.Situacao != SituacaoContratoFreteAcrescimoDesconto.FalhaIntegracao && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(false, true, "O contrato não está mais com falha, não sendo possível integrar nessa situação!");

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.ContratoFreteAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.AgIntegracao;

                repContratoFreteAcrescimoDescontoIntegracao.Atualizar(integracao);
                repContratoFreteAcrescimoDesconto.Atualizar(integracao.ContratoFreteAcrescimoDesconto);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.ContratoFreteAcrescimoDesconto, "Solicitou o reenvio da integração", unitOfWork);

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
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("CIOT", "NumeroCIOT", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integradora", "Integradora", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao> listaIntegracoes = repContratoFreteAcrescimoDescontoIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repContratoFreteAcrescimoDescontoIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        NumeroCIOT = integracao.CIOT?.Numero,
                        Integradora = integracao.TipoIntegracao.Tipo == TipoIntegracao.CIOT ? integracao.CIOT?.Operadora.ObterDescricao() ?? "" : integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repContratoFreteAcrescimoDescontoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repContratoFreteAcrescimoDescontoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repContratoFreteAcrescimoDescontoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repContratoFreteAcrescimoDescontoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

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

        public async Task<IActionResult> LiberarComIntegracaoRejeitada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto serContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Contrato não encontrado.");

                if (contrato.Situacao != SituacaoContratoFreteAcrescimoDesconto.FalhaIntegracao)
                    return new JsonpResult(false, true, "Só é possível liberar quando a integração estiver rejeitada.");

                unitOfWork.Start();

                serContratoFreteAcrescimoDesconto.AplicarValorNoContratoFrete(contrato, TipoServicoMultisoftware, Auditado, false);

                contrato.Situacao = SituacaoContratoFreteAcrescimoDesconto.Finalizado;
                repContratoFreteAcrescimoDesconto.Atualizar(contrato);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, "Liberou mesmo com integração rejeitada", unitOfWork);

                unitOfWork.CommitChanges();

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

        public async Task<IActionResult> ReaplicarValorRejeitado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto serContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Contrato não encontrado.");

                if (contrato.Situacao != SituacaoContratoFreteAcrescimoDesconto.AplicacaoValorRejeitado)
                    return new JsonpResult(false, true, $"Só é possível a tentativa de aplicar o valor novamente na situação { SituacaoContratoFreteAcrescimoDesconto.AplicacaoValorRejeitado.ObterDescricao() }.");

                unitOfWork.Start();

                serContratoFreteAcrescimoDesconto.AplicarValorNoContratoFrete(contrato, TipoServicoMultisoftware, Auditado, false);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, "Aplicou o valor rejeitado ao contrato de frete", unitOfWork);

                unitOfWork.CommitChanges();

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
                return new JsonpResult(false, "Ocorreu uma falha ao reaplicar o valor rejeitado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "NumeroCIOT")
                return "CIOT.Numero";
            if (propriedadeOrdenar == "Integradora")
                return "CIOT.Operadora";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
