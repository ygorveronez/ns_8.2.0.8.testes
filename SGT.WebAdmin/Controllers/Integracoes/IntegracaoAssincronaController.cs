using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.IntegracaoAssincrona;
using Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.IO.Compression;
using System.Threading;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [AllowAuthenticate]
    [CustomAuthorize("Integracoes/IntegracaoAssincrona")]
    public class IntegracaoAssincronaController : BaseController
    {
        #region Propriedades Privadas

        private readonly IProcessamentoTarefaRepository _repositorioProcessamentoTarefa;
        private readonly IRequestDocumentoRepository _repositorioRequestDocumento;
        private readonly ITarefaIntegracao _repositorioTarefaIntegracao;

        #endregion Propriedades Privadas

        #region Construtores

        public IntegracaoAssincronaController(Conexao conexao, IProcessamentoTarefaRepository repositorioProcessamentoTarefa, IRequestDocumentoRepository repositorioRequestDocumento, ITarefaIntegracao repositorioTarefaIntegracao) : base(conexao)
        {
            _repositorioProcessamentoTarefa = repositorioProcessamentoTarefa;
            _repositorioRequestDocumento = repositorioRequestDocumento;
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
        }

        #endregion Construtores

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            try
            {
                Grid grid = await ObterGridPesquisa(cancellationToken);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar integração assíncronas.");
            }
        }

        public async Task<IActionResult> PesquisaExportar(CancellationToken cancellationToken)
        {
            try
            {
                Grid grid = await ObterGridPesquisa(cancellationToken);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);

                return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            string codigo = Request.GetStringParam("Codigo");

            Dominio.Entidades.ProcessadorTarefas.ProcessamentoTarefa tarefa = await _repositorioProcessamentoTarefa.ObterPorIdAsync(codigo, cancellationToken);

            if (tarefa == null)
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

            var etapas = string.Join(" | ", tarefa.Etapas.Select(etapa => etapa.TipoFormatado).ToList());

            var retorno = new
            {
                tarefa.Codigo,
                TipoRequisicao = tarefa.TipoRequestFormatado,
                Status = tarefa.StatusFormatado,
                DataCriacao = tarefa.CriadoEmFormatado,
                EtapaAtual = tarefa.EtapaAtualFormatada,
                Etapas = etapas
            };

            return new JsonpResult(retorno);
        }

        public async Task<IActionResult> BuscarIntegracoesIntegradora(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoTarefa = Request.GetStringParam("Codigo");
                SituacaoIntegracao? situacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Grid grid = new Grid(Request);

                grid.header = new List<Head>();

                grid.AdicionarCabecalho("Código", "Codigo", 25, Align.center, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 25, Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 25, Align.center, false);
                grid.AdicionarCabecalho("Data", "Data", 25, Align.center, false);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Align.center, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 50, Align.left, false);

                List<Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao> listaTarefaIntegracao = await _repositorioTarefaIntegracao.ObterIntegracaoIntegradora(codigoTarefa, situacaoIntegracao, cancellationToken);

                var retorno = listaTarefaIntegracao.Select(tarefaIntegracao => new
                {
                    tarefaIntegracao.Codigo,
                    Tipo = tarefaIntegracao.TipoIntegracaoFormatada,
                    Situacao = tarefaIntegracao.SituacaoIntegracaoFormatada,
                    tarefaIntegracao.Tentativas,
                    Data = tarefaIntegracao.DataCriacaoFormatada,
                    Mensagem = tarefaIntegracao.ProblemaIntegracao,
                    DT_RowColor = tarefaIntegracao.SituacaoIntegracao.ObterCorLinha(),
                    DT_FontColor = tarefaIntegracao.SituacaoIntegracao.ObterCorFonte(),
                }).OrderByDescending(arquivo => arquivo.Data).ToList();

                grid.setarQuantidadeTotal(retorno.Count);
                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarHistoricoIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoIntegracao = Request.GetStringParam("Codigo");

                Grid grid = new Grid(Request);

                grid.header = new List<Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 25, Align.center, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 25, Align.center, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 50, Align.left, false);

                Dominio.Entidades.ProcessadorTarefas.TarefaIntegracao integracao = await _repositorioTarefaIntegracao.ObterUmPorExpressaoAsync(o => o.Id == codigoIntegracao, cancellationToken);

                var retorno = integracao.Arquivos.Select(arquivo => new
                {
                    arquivo.Codigo,
                    Data = arquivo.DataCriacaoFormatada,
                    Tipo = integracao.SituacaoIntegracaoFormatada,
                    Mensagem = integracao.ProblemaIntegracao
                }).OrderByDescending(arquivo => arquivo.Data).ToList();

                grid.setarQuantidadeTotal(retorno.Count);
                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> DownloadArquivoRequisicaoIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoTarefa = Request.GetStringParam("Codigo");

                Dominio.Entidades.ProcessadorTarefas.ProcessamentoTarefa tarefa = await _repositorioProcessamentoTarefa.ObterPorIdAsync(codigoTarefa, cancellationToken);

                if (tarefa == null)
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Dominio.Entidades.ProcessadorTarefas.RequestDocumento requestDoc = await _repositorioRequestDocumento.ObterUmPorExpressaoAsync(r => r.Id == tarefa.RequestId, cancellationToken);

                if (requestDoc == null)
                    return new JsonpResult(true, false, "Request não encontrado.");

                using MemoryStream memoryStream = new MemoryStream();
                using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    if (requestDoc.Dados != null)
                    {
                        ZipArchiveEntry requisicaoEntry = zipArchive.CreateEntry(string.Concat(tarefa.Codigo, ".", "json"));

                        using (Stream entryStream = requisicaoEntry.Open())
                        using (StreamWriter writer = new StreamWriter(entryStream))
                        {
                            string json = requestDoc.Dados.ToJson(new MongoDB.Bson.IO.JsonWriterSettings 
                            { 
                                OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict,
                                Indent = true
                            });
                            await writer.WriteAsync(json);
                        }
                    }
                }

                byte[] arquivoZip = memoryStream.ToArray();

                return Arquivo(arquivoZip, "application/zip", "Requisição Integração Assíncrona.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoArquivo = Request.GetStringParam("Codigo");

                Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao arquivo = await _repositorioTarefaIntegracao.ObterArquivoIntegracaoPorCodigo(codigoArquivo, cancellationToken);

                if (arquivo == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                byte[] arquivoZip = await CriarArquivoZip(arquivo);

                return Arquivo(arquivoZip, "application/zip", "Arquivos Integração Assíncrona.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private async Task<Grid> ObterGridPesquisa(CancellationToken cancellationToken)
        {
            Grid grid = ObterCabecalhosGridPesquisa();

            FiltroPesquisaIntegracaoAssincrona filtrosPesquisa = ObterFiltrosPesquisa();
            long totalLinhas = await _repositorioProcessamentoTarefa.ContarComFiltrosAsync(filtrosPesquisa, cancellationToken);

            if (totalLinhas == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar);

            List<Dominio.Entidades.ProcessadorTarefas.ProcessamentoTarefa> tarefas = await _repositorioProcessamentoTarefa.ObterPaginadoComFiltrosAsync(filtrosPesquisa, parametrosConsulta, cancellationToken);

            var retorno = tarefas.Select(tarefa => new
            {
                tarefa.Codigo,
                DataCriacao = tarefa.CriadoEmFormatado,
                StatusTarefa = tarefa.StatusFormatado,
                TipoRequisicao = tarefa.TipoRequestFormatado,
                EtapaAtual = tarefa.EtapaAtualFormatada,
                EtapaAtualMensagem = tarefa.EtapaAtualMensagemFormatada,
                TentativasEtapaAtual = tarefa.ObterEtapaAtual?.Tentativas.ToString() ?? string.Empty,
                DT_RowColor = tarefa.Status.ObterCorLinha(),
                DT_FontColor = tarefa.Status.ObterCorFonte()
            });

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal((int)totalLinhas);

            return grid;
        }

        private FiltroPesquisaIntegracaoAssincrona ObterFiltrosPesquisa()
        {
            FiltroPesquisaIntegracaoAssincrona filtrosPesquisa = new FiltroPesquisaIntegracaoAssincrona()
            {
                Pedido = Request.GetStringParam("Pedido"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                NumeroCarga = Request.GetNullableIntParam("NumeroCarga"),
                DataInicialIntegracao = Request.GetNullableDateTimeParam("DataInicialIntegracao"),
                DataFinalIntegracao = Request.GetNullableDateTimeParam("DataFinalIntegracao"),
                StatusTarefa = Request.GetNullableEnumParam<StatusTarefa>("StatusTarefa"),
                TipoRequest = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.TipoRequest>("TipoRequest"),
                TipoEtapaAtual = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores.TipoEtapaTarefa>("TipoEtapaAtual"),
                JobId = Request.GetStringParam("JobId")
            };

            return filtrosPesquisa;
        }

        private Grid ObterCabecalhosGridPesquisa()
        {
            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("Código", "Codigo", 6, Align.center, false);
            grid.AdicionarCabecalho("Data", "DataCriacao", 6, Align.center, false);
            grid.AdicionarCabecalho("Situação", "StatusTarefa", 4, Align.center, false);
            grid.AdicionarCabecalho("Tipo Requisição", "TipoRequisicao", 6, Align.center, false);
            grid.AdicionarCabecalho("Etapa Atual", "EtapaAtual", 5, Align.center, false);
            grid.AdicionarCabecalho("Tentativas Etapa", "TentativasEtapaAtual", 5, Align.center, false);
            grid.AdicionarCabecalho("Mensagem Etapa Atual", "EtapaAtualMensagem", 6, Align.center, false);

            return grid;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        private async Task<byte[]> CriarArquivoZip(Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao arquivo)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                if (arquivo.ArquivoRequisicao != null)
                {
                    ZipArchiveEntry requisicaoEntry = zipArchive.CreateEntry(string.Concat("requisicao-", arquivo.Identifcador, ".", arquivo.Tipo));

                    using (Stream entryStream = requisicaoEntry.Open())
                    using (StreamWriter writer = new StreamWriter(entryStream))
                    {
                        string json = arquivo.ArquivoRequisicao.ToJson(new MongoDB.Bson.IO.JsonWriterSettings 
                        { 
                            OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict,
                            Indent = true
                        });
                        await writer.WriteAsync(json);
                    }
                }

                if (arquivo.ArquivoResposta != null)
                {
                    ZipArchiveEntry respostaEntry = zipArchive.CreateEntry(string.Concat("retorno-", arquivo.Identifcador, ".", arquivo.Tipo));

                    using (Stream entryStream = respostaEntry.Open())
                    using (StreamWriter writer = new StreamWriter(entryStream))
                    {
                        string json = arquivo.ArquivoResposta.ToJson(new MongoDB.Bson.IO.JsonWriterSettings 
                        { 
                            OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict,
                            Indent = true
                        });
                        await writer.WriteAsync(json);
                    }
                }
            }

            return memoryStream.ToArray();
        }

        #endregion Métodos Privados
    }
}