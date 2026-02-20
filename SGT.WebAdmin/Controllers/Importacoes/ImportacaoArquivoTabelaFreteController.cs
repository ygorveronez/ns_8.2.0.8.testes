using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete;
using Newtonsoft.Json;
using OfficeOpenXml;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Importacoes
{
    [CustomAuthorize("Importacoes/ImportacaoArquivoTabelaFrete")]
    public class ImportacaoArquivoTabelaFreteController : BaseController
    {
        #region Construtores

        public ImportacaoArquivoTabelaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais 

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Arquivo", "NomeArquivo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Linhas", "QuantidadeLinhas", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data da importação", "DataImportacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Início do processamento", "DataInicioProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fim do processamento", "DataFimProcessamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tempo", "Tempo", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 15, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaImportacaoTabelaFrete filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int total = repImportacaoTabelaFrete.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> lista = total > 0 ? repImportacaoTabelaFrete.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete>();

                grid.AdicionaRows(lista.Select(row => new
                {
                    row.Codigo,
                    row.NomeArquivo,
                    row.QuantidadeLinhas,
                    row.DataImportacao,
                    Usuario = row.Usuario?.Nome ?? "",
                    row.DataInicioProcessamento,
                    row.DataFimProcessamento,
                    Tempo = row.Tempo(),
                    row.Situacao,
                    DescricaoSituacao = row.Situacao.ObterDescricao(),
                    row.Mensagem
                }).ToList());

                grid.setarQuantidadeTotal(total);

                return new JsonpResult(grid);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as importações de pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Linhas()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Models.Grid.Grid grid = ConsultarLinhas(codigo);
                if (grid == null) return new JsonpResult(false, "Importação não encontrada.");
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as linhas da importação de pedidos.");
            }
        }

        //public async Task<IActionResult> Colunas()
        //{
        //    try
        //    {
        //        int codigo = Request.GetIntParam("Codigo");
        //        Models.Grid.Grid grid = ConsultarColunas(codigo);
        //        if (grid == null) return new JsonpResult(false, "Linha não encontrada.");
        //        return new JsonpResult(grid);

        //        return new JsonpResult(true);
        //    }
        //    catch (Exception excecao)
        //    {
        //        Servicos.Log.TratarErro(excecao);
        //        return new JsonpResult(false, "Ocorreu uma falha ao consultar as colunas da linha da importação de pedidos.");
        //    }
        //}

        //salvar os dados do arquivo em linhas/coluanas para a Thred buscar as linhas/colunas pendentes para processar
        public async Task<IActionResult> ImportarDadosArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string tokenArquivo = Request.GetStringParam("tokenArquivo");
                string NomeArquivo = Request.GetStringParam("NomeArquivo");

                if (string.IsNullOrWhiteSpace(tokenArquivo))
                    return new JsonpResult(false, true, "Arquivo não foi enviado corretamente.");

                dynamic Parametros = JsonConvert.DeserializeObject<object>(Request.GetStringParam("Parametros"));
                bool existeParametroEntregaExcedente = false;
                bool existeParametroPorEntrega = false;

                foreach (dynamic parametro in Parametros)
                {
                    string item = (string)parametro.ItemParametroBase;

                    if (!existeParametroPorEntrega)
                        existeParametroPorEntrega = item.StartsWith("5_");

                    if (!existeParametroEntregaExcedente)
                        existeParametroEntregaExcedente = item.Contains("EntregaExcedente_");
                }

                if (existeParametroEntregaExcedente && !existeParametroPorEntrega)
                    return new JsonpResult(false, "Obrigatório informar coluna para valor por entrega quando tiver coluna para entrega excedente.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro parametrosImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro()
                {
                    CodigoDestino = Request.GetIntParam("Destino"),
                    CodigoEmpresa = Request.GetIntParam("Empresa"),
                    CodigoOrigem = Request.GetIntParam("Origem"),
                    CodigoTabelaFrete = Request.GetIntParam("TabelaFrete"),
                    CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                    CodigoVigencia = Request.GetIntParam("Vigencia"),
                    FreteValidoParaQualquerDestino = Request.GetBoolParam("FreteValidoParaQualquerDestino"),
                    FreteValidoParaQualquerOrigem = Request.GetBoolParam("FreteValidoParaQualquerOrigem"),
                    IndiceColunaCepDestino = Request.GetIntParam("ColunaCEPDestino"),
                    IndiceColunaCepDestinoDiasUteis = Request.GetIntParam("ColunaCEPDestinoDiasUteis"),
                    IndiceColunaTomadorDestino = Request.GetIntParam("ColunaTomadorDestino"),
                    IndiceColunaCepOrigem = Request.GetIntParam("ColunaCEPOrigem"),
                    IndiceColunaClienteDestino = Request.GetIntParam("ColunaClienteDestino"),
                    IndiceColunaClienteOrigem = Request.GetIntParam("ColunaClienteOrigem"),
                    IndiceColunaCodigoIntegracao = Request.GetIntParam("ColunaCodigoIntegracao"),
                    IndiceColunaDestino = Request.GetIntParam("ColunaDestino"),
                    IndiceColunaEstadoDestino = Request.GetIntParam("ColunaEstadoDestino"),
                    IndiceColunaEstadoOrigem = Request.GetIntParam("ColunaEstadoOrigem"),
                    IndiceColunaOrigem = Request.GetIntParam("ColunaOrigem"),
                    IndiceColunaParametroBase = Request.GetIntParam("ColunaParametrosBase"),
                    IndiceColunaRegiaoDestino = Request.GetIntParam("colunaRegiaoDestino"),
                    IndiceColunaRegiaoOrigem = Request.GetIntParam("colunaRegiaoOrigem"),
                    IndiceColunaRotaDestino = Request.GetIntParam("ColunaRotaDestino"),
                    IndiceColunaRotaOrigem = Request.GetIntParam("ColunaRotaOrigem"),
                    IndiceColunaTransportador = Request.GetIntParam("ColunaTransportador"),
                    IndiceLinhaIniciarImportacao = Request.GetIntParam("LinhaInicioDados"),
                    NaoAtualizarValoresZerados = Request.GetBoolParam("NaoAtualizarValoresZerados"),
                    NaoValidarTabelasExistentes = Request.GetBoolParam("NaoValidarTabelasExistentes"),
                    IndiceColunaFronteira = Request.GetIntParam("ColunaFronteira"),
                    IndiceColunaPrioridadeUso = Request.GetIntParam("ColunaPrioridadeUso"),
                    Moeda = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda"),
                    TipoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao>("TipoPagamento"),
                    IndiceColunaCanalEntrega = Request.GetIntParam("ColunaCanalEntrega"),
                    IndiceColunaTipoOperacao = Request.GetIntParam("ColunaTipoOperacao"),
                    IndiceColunaTipoDeCarga = Request.GetIntParam("ColunaTipoDeCarga"),
                    IndiceColunaLeadTimeDias = Request.GetIntParam("ColunaLeadTimeDias")
                };

                unitOfWork.Start();

                int quantidadeLinhas = RetornaQuantidadeLinhasArquivo(tokenArquivo, parametrosImportacao.IndiceLinhaIniciarImportacao);
                Servicos.Embarcador.Importacao.ImportacaoTabelaFrete servicoImportacaoTabelaFrete = new Servicos.Embarcador.Importacao.ImportacaoTabelaFrete(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = servicoImportacaoTabelaFrete.GerarEntidadesImportacaoTabelaFrete(parametrosImportacao, quantidadeLinhas, Parametros, Usuario, Auditado, NomeArquivo, tokenArquivo);

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //salvar arquivo enviado; e retornando Token para posterior importação dos dados
        public async Task<IActionResult> ImportarArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repositorioConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repositorioConfiguracaoAmbiente.BuscarPrimeiroRegistro();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, false, "Por favor selecione um arquivo para importação de tabela de frete.");

                dynamic result = SalvarArquivoImportacaoTemporario(unitOfWork, configuracaoAmbiente);

                return new JsonpResult(result);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o arquivo para importação da tabela de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reprocessar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete importacaoTabelaFrete = repImportacaoTabelaFrete.BuscarPorCodigo(codigo, true);

                if (importacaoTabelaFrete == null) return new JsonpResult(false, "Importação não encontrada.");
                if (importacaoTabelaFrete.Situacao != SituacaoImportacaoTabelaFrete.Erro && importacaoTabelaFrete.Situacao != SituacaoImportacaoTabelaFrete.Sucesso && importacaoTabelaFrete.Situacao != SituacaoImportacaoTabelaFrete.Cancelado) return new JsonpResult(false, "É possivel reprocessar apenas importações que já foram processadas ou canceladas.");

                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha repImportacaoTabelaFreteLinha = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha(unitOfWork);
                repImportacaoTabelaFreteLinha.ReprocessarLinhasPendentesImportacaoTabelaFrete(importacaoTabelaFrete.Codigo);

                importacaoTabelaFrete.Situacao = SituacaoImportacaoTabelaFrete.Pendente;
                importacaoTabelaFrete.DataInicioProcessamento = null;
                importacaoTabelaFrete.DataFimProcessamento = null;
                importacaoTabelaFrete.Mensagem = null;
                repImportacaoTabelaFrete.Atualizar(importacaoTabelaFrete);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Planilha marcada como pendente para reprocessamento.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a planilha importada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);

                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro repImportacaoTabelaFreteParametro = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteParametro(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete importacaoTabelaFrete = repImportacaoTabelaFrete.BuscarPorCodigo(codigo, true);
                if (importacaoTabelaFrete == null) return new JsonpResult(false, "Importação não encontrada.");

                repImportacaoTabelaFreteParametro.DeletarPorImportacao(importacaoTabelaFrete.Codigo);
                repImportacaoTabelaFrete.Deletar(importacaoTabelaFrete);
                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Importação de planilha excluída com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir a planilha importada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete importacaoTabelaFrete = repImportacaoTabelaFrete.BuscarPorCodigo(codigo, true);

                if (importacaoTabelaFrete == null) return new JsonpResult(false, "Importação não encontrada.");
                if (importacaoTabelaFrete.Situacao != SituacaoImportacaoTabelaFrete.Pendente && importacaoTabelaFrete.Situacao != SituacaoImportacaoTabelaFrete.Processando) return new JsonpResult(false, "É possivel cancelar apenas importações que estão pendentes ou processando.");

                importacaoTabelaFrete.Situacao = SituacaoImportacaoTabelaFrete.Cancelado;
                importacaoTabelaFrete.Mensagem = "Por " + Usuario.Nome + " em " + DateTime.Now;

                repImportacaoTabelaFrete.Atualizar(importacaoTabelaFrete);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, importacaoTabelaFrete, "Importacao cancelada pelo Usuario", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Importação da planilha cancelada com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar a planilha importada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                if (codigo > 0)
                {

                    Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete importacaoTabelaFrete = repImportacaoTabelaFrete.BuscarPorCodigo(codigo, true);

                    if (importacaoTabelaFrete != null)
                    {

                        string guidArquivo = importacaoTabelaFrete.TokenArquivo + ".xlsx";

                        string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao);
                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        {
                            byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                            if (data != null)
                            {
                                return Arquivo(data, "application/xlsx", importacaoTabelaFrete.NomeArquivo);
                            }
                        }
                    }
                }
                return new JsonpResult(false, false, "Arquivo não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo.");
            }
        }

        #endregion

        #region Métodos Privados

        private int RetornaQuantidadeLinhasArquivo(string token, int indiceInicioLinha)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Importacao.ImportacaoTabelaFrete servImportacaoTabelaFrete = new Servicos.Embarcador.Importacao.ImportacaoTabelaFrete(unitOfWork);

            string arq = servImportacaoTabelaFrete.ObterArquivoTemporario(token, unitOfWork);
            ExcelPackage arquivoExcel = new ExcelPackage(Utilidades.IO.FileStorageService.Storage.OpenRead(arq));
            ExcelWorksheet planilha = arquivoExcel.Workbook.Worksheets.First();
            if ((arquivoExcel == null) || (arquivoExcel.Workbook.Worksheets.Count == 0))
                return 0;
            else
                return planilha.Dimension.End.Row;
        }

        private dynamic SalvarArquivoImportacaoTemporario(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente)
        {
            Servicos.DTO.CustomFile arquivo = HttpContext.GetFile();
            string extensao = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
            long byteCount = arquivo.Length > 0 ? arquivo.Length / 1024 : 0;
            long maxUploadSize = configuracaoAmbiente.PermitirInformarCapacidadeMaximaParaUploadArquivos ? (configuracaoAmbiente.CapacidadeMaximaParaUploadArquivos * 1000) : 10000;

            if (!extensao.Equals(".xlsx"))
                throw new ControllerException("A extensão do arquivo é inválida.");

            if (byteCount > maxUploadSize)
                throw new ControllerException($"Tamanho do arquivo inválido, tamanho máximo permitido para importação {maxUploadSize} MB.");

            string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
            string nomeArquivo = arquivo.FileName;
            string caminho = $"{Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao}";

            arquivo.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensao));

            var result = new { Token = guidArquivo, NomeOriginal = nomeArquivo };

            return result;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaImportacaoTabelaFrete ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaImportacaoTabelaFrete filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaImportacaoTabelaFrete()
            {
                CodigoUsuario = Request.GetIntParam("Funcionario"),
                NomeArquivo = Request.GetStringParam("Planilha"),
                DataImportacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataImportacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete>("Situacao"),
                Mensagem = Request.GetStringParam("Mensagem"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ConsultarLinhas(int codigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete importacaoTabelaFrete = repImportacaoTabelaFrete.BuscarPorCodigo(codigo, false);

                if (importacaoTabelaFrete == null)
                    return null;

                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha repImportacaoTabelaFreteLinha = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha> lista = repImportacaoTabelaFreteLinha.BuscarPorImportacaoTabelaFrete(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 45, Models.Grid.Align.left, false);

                grid.AdicionaRows(lista.Select(row => new
                {
                    row.Codigo,
                    row.Situacao,
                    row.Numero,
                    DescricaoSituacao = row.Situacao.ObterDescricao(),
                    row.Mensagem
                }).ToList());

                grid.setarQuantidadeTotal(lista.Count);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //private Models.Grid.Grid ConsultarColunas(int codigo)
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
        //    try
        //    {
        //        Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha repImportacaoTabelaFreteLinha = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha(unitOfWork);
        //        List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha> ImportacaoTabelaLinha = repImportacaoTabelaFreteLinha.BuscarPorImportacaoTabelaFrete(codigo);
        //        if (ImportacaoTabelaLinha == null) return null;

        //        Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna repImportacaoTabelaFreteLinhaColuna = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna(unitOfWork);
        //        List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna> listaTabelaColuna = repImportacaoTabelaFreteLinhaColuna.BuscarPorLinha(codigo);

        //        Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
        //        grid.AdicionarCabecalho("Codigo", false);
        //        grid.AdicionarCabecalho("Nome Campo", "NomeCampo", 30, Models.Grid.Align.right, false);
        //        grid.AdicionarCabecalho("Valor", "Valor", 70, Models.Grid.Align.left, false);

        //        var listaRetornar = (
        //            from row in listaTabelaColuna
        //            select new
        //            {
        //                row.Codigo,
        //                row.NomeCampo,
        //                row.Valor
        //            }
        //        ).ToList();

        //        grid.AdicionaRows(listaRetornar);
        //        grid.setarQuantidadeTotal(listaRetornar.Count());

        //        return grid;
        //    }
        //    catch (Exception excecao)
        //    {
        //        throw excecao;
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}



        #endregion
    }
}
