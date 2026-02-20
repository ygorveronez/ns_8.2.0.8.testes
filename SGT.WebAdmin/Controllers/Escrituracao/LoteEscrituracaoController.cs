using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/LoteEscrituracao")]
    public class LoteEscrituracaoController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repositorioDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                double cpnjTomador;
                double.TryParse(Request.Params("Tomador"), out cpnjTomador);

                int codigoFilial;
                int.TryParse(Request.Params("Filial"), out codigoFilial);

                int codigoEmpresa;
                int.TryParse(Request.Params("Transportador"), out codigoEmpresa);

                int codigoModeloDocumentoFiscal;
                int.TryParse(Request.Params("ModeloDocumentoFiscal"), out codigoModeloDocumentoFiscal);

                int codigoTipoOperacao;
                int.TryParse(Request.Params("TipoOperacao"), out codigoTipoOperacao);

                // Busca os documentos selecionados
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentos = BuscarDocumentosSelecionados(unitOfWork, out string erro);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (documentos.Any(obj => obj.Carga != null && (obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)))
                        erro += "Enquanto as notas estavam sendo selecionadas uma carga das notas selecionadas foi cancelada, por favor, refaça o processo.";

                    if (documentos.Any(obj => obj.CargaOcorrencia != null && (obj.CargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada)))
                        erro += "Enquanto as notas estavam sendo selecionadas uma ocorrencia das notas selecionadas foi cancelada, por favor, refaça o processo.";
                }

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                if (documentos.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum documento selecionado.");

                if (documentos.Any(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.Bloqueado))
                    throw new ControllerException("Não é possível gerar lotes com documentos bloqueados.");

                Dominio.Entidades.Cliente tomador = null;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Empresa empresa = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigoModeloDocumentoFiscal);

                if (cpnjTomador > 0D)
                    tomador = repCliente.BuscarPorCPFCNPJ(cpnjTomador);
                if (codigoFilial > 0)
                    filial = repFilial.BuscarPorCodigo(codigoFilial);
                if (codigoEmpresa > 0)
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                if (codigoTipoOperacao > 0)
                    tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                Servicos.Embarcador.Escrituracao.Escrituracao servicoEscrituracao = new Servicos.Embarcador.Escrituracao.Escrituracao(unitOfWork, Auditado);
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao = servicoEscrituracao.AdicionarLoteEscrituracao(dataInicio, dataFim, tomador, filial, empresa, tipoOperacao, modeloDocumentoFiscal, documentos);

                Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork);
                servicoProvisao.GerarExtornoProvisaoAposEscrituracao(documentos);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    Codigo = loteEscrituracao.Codigo
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.LoteEscrituracao repEscrituracaoManual = new Repositorio.Embarcador.Escrituracao.LoteEscrituracao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao loteEscrituracao = repEscrituracaoManual.BuscarPorCodigo(codigo);

                // Valida
                if (loteEscrituracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    loteEscrituracao.Codigo,
                    loteEscrituracao.Situacao,
                    DataInicial = loteEscrituracao.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = loteEscrituracao.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    loteEscrituracao.Numero,
                    Filial = loteEscrituracao.Filial != null ? new { loteEscrituracao.Filial.Codigo, loteEscrituracao.Filial.Descricao } : new { Codigo = 0, Descricao = "" },
                    TipoOperacao = loteEscrituracao.TipoOperacao != null ? new { loteEscrituracao.TipoOperacao.Codigo, loteEscrituracao.TipoOperacao.Descricao } : new { Codigo = 0, Descricao = "" },
                    Transportador = new { Codigo = loteEscrituracao.Empresa?.Codigo ?? 0, Descricao = loteEscrituracao.Empresa?.Descricao ?? "" },
                    ModeloDocumentoFiscal = new { Codigo = loteEscrituracao.ModeloDocumentoFiscal?.Codigo ?? 0, Descricao = loteEscrituracao.ModeloDocumentoFiscal?.Descricao ?? "" },
                    Tomador = new { Codigo = loteEscrituracao.Tomador?.Codigo ?? 0, Descricao = loteEscrituracao.Tomador?.Descricao ?? string.Empty },
                    loteEscrituracao.DescricaoSituacao
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaDocumento()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaDocumento(exportarPesquisa: true);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = ObterGridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                ObterPropriedadeOrdenar(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumento()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDocumento(exportarPesquisa: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> BuscarDocumentosSelecionados(Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> listaBusca = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();

            // Valida filtros
            int transportador = 1;
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                int.TryParse(Request.Params("Transportador"), out transportador);

            int.TryParse(Request.Params("Filial"), out int filial);
            double.TryParse(Request.Params("Tomador"), out double tomador);

            //if (filial == 0 || tomador == 0)
            //{
            //    erro = "Filial e Tomador são obrigatórios.";
            //    return null;
            //}

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && tomador == 0D)
            {
                erro = "Tomador é obrigatório.";
                return null;
            }


            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);
            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    bool exportarPesquisa = false;
                    int totalRegistros = 0;
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        PropriedadeOrdenar = "Codigo"
                    };

                    ExecutaPesquisaDocumento(ref listaBusca, ref totalRegistros, parametrosConsulta, exportarPesquisa, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    listaBusca.Remove(new Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao() { Codigo = (int)dynNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                foreach (var dynSelecionada in listaSelecionadas)
                    listaBusca.Add(repDocumentoEscrituracao.BuscarPorCodigo((int)dynSelecionada.Codigo, false));
            }

            // Retorna lista
            return listaBusca;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, bool somenteAtivo, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.LoteEscrituracao repLoteEscrituracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracao(unitOfWork);

            // Dados do filtro
            int transportador = 0;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                transportador = this.Empresa.Codigo;
            else
                int.TryParse(Request.Params("Empresa"), out transportador);


            DateTime dataInicio;
            DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

            DateTime dataFim;
            DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);


            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("Carga"), out int carga);
            int.TryParse(Request.Params("Ocorrencia"), out int ocorrencia);
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("NumeroDOC"), out int numeroDOC);
            int.TryParse(Request.Params("LocalidadePrestacao"), out int localidadePrestacao);
            double.TryParse(Request.Params("Tomador"), out double tomador);
            int.TryParse(Request.Params("Situacao"), out int situacao);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao situacaoLoteEscrituracao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao)situacao;

            // Consulta
            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao> listaGrid = repLoteEscrituracao.Consultar(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoLoteEscrituracao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLoteEscrituracao.ContarConsulta(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoLoteEscrituracao);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            DataInicio = obj.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                            DataFim = obj.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                            Tomador = obj.Tomador?.Descricao ?? "",
                            TipoOperacao = obj.TipoOperacao?.Descricao ?? "",
                            Transportador = obj.Empresa?.Descricao ?? "",
                            Filial = obj.Filial?.Descricao ?? "",
                            obj.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportarPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repositorioDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

            Servicos.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao servicoConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao = new Servicos.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(unitOfWork);

            List<dynamic> listaRetornar = new List<dynamic>();
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracao()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoLoteEscrituracao = Request.GetIntParam("Codigo"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoModeloDocumento = Request.GetIntParam("ModeloDocumentoFiscal"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                IntervaloParaEscrituracaoDocumento = servicoConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao.ObterIntervalo(),
            };

            listaGrid = repositorioDocumentoEscrituracao.Consultar(filtrosPesquisa, parametrosConsulta);
            totalRegistros = repositorioDocumentoEscrituracao.ContarConsulta(filtrosPesquisa);

            if (listaGrid.Count > 0)
            {
                string nomeArquivoEDI = ObterNomeArquivoEDI(unitOfWork, filtrosPesquisa.CodigoLoteEscrituracao, exportarPesquisa);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao documento in listaGrid)
                {
                    listaRetornar.Add(new
                    {
                        documento.Codigo,
                        CodigoFilial = documento.Filial?.Codigo ?? 0,
                        CodigoTomador = documento.CTe.TomadorPagador?.Cliente.Codigo ?? 0,
                        CodigoTransportador = documento.CTe.Empresa.Codigo,
                        Transportador = documento.CTe.Empresa.Descricao,
                        Documento = documento.CTe.Numero.ToString() + " - " + documento.CTe.Serie.Numero.ToString(),
                        Tipo = documento.CTe.ModeloDocumentoFiscal.Abreviacao,
                        CodigoModeloDocumento = documento.CTe.ModeloDocumentoFiscal.Codigo,
                        ModeloDocumento = documento.CTe.ModeloDocumentoFiscal.Descricao,
                        FechamentoFrete = documento.FechamentoFrete?.Numero.ToString() ?? "",
                        Ocorrencia = documento.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? "",
                        Carga = documento.Carga?.CodigoCargaEmbarcador ?? "",
                        DataEmissao = documento.CTe.DataEmissao?.ToString("dd/MM/yyyy") ?? "",
                        Tomador = documento.CTe.TomadorPagador?.Cliente.Descricao ?? string.Empty,
                        Filial = documento.Filial?.Descricao ?? "",
                        ValorFrete = documento.CTe.ValorAReceber.ToString("n2"),
                        NomeArquivoEDI = nomeArquivoEDI,
                        LiberarDocumentosEmitidosQuandoEntregaForConfirmada = documento.Carga?.TipoOperacao?.ConfiguracaoEmissao?.LiberarDocumentosEmitidosQuandoEntregaForConfirmada ?? false,
                    });
                }
            }

            return listaRetornar;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Inicio", "DataInicio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Fim", "DataFim", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 20, Models.Grid.Align.left, true);
            //grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaDocumento(bool exportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);


                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("CodigoTomador", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("CodigoModeloDocumento", false);
                grid.AdicionarCabecalho("ModeloDocumento", false);
                grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, true);

                if (repFechamentoFrete.VerificarExistemFechamento())
                    grid.AdicionarCabecalho("Fechamento de Contrato", "FechamentoFrete", 8, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Emissão NF-e", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 15, Models.Grid.Align.right, true);

                if (exportarPesquisa)
                    grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivoEDI", 20, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> listaGrid = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
                int totalRegistros = 0;
                var lista = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, parametrosConsulta, exportarPesquisa, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterNomeArquivoEDI(Repositorio.UnitOfWork unitOfWork, int codigoLoteEscrituracao, bool exportarPesquisa)
        {
            if (exportarPesquisa & (codigoLoteEscrituracao > 0))
            {
                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repositorio = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao integracao = repositorio.BuscarPorLoteEscrituracao(codigoLoteEscrituracao).FirstOrDefault();

                if (integracao != null)
                    return Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, "", unitOfWork);
            }

            return "";
        }

        private void ObterPropriedadeOrdenar(ref string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Tomador")
                propriedadeOrdenar = "Tomador.Nome";
            else if (propriedadeOrdenar == "Transportador")
                propriedadeOrdenar = "Empresa.RazaoSocial";
            else if (propriedadeOrdenar == "DataInicio")
                propriedadeOrdenar = "DataInicial";
            else if (propriedadeOrdenar == "DataFim")
                propriedadeOrdenar = "DataFinal";
            else if (propriedadeOrdenar == "DescricaoSituacao")
                propriedadeOrdenar = "Situacao";
        }

        private string ObterPropriedadeOrdenarDocumentos(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Documento")
                return "CTe.Numero";

            if (propriedadeOrdenar == "Carga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "FechamentoFrete")
                return "FechamentoFrete.Numero";

            if (propriedadeOrdenar == "Ocorrencia")
                return "CargaOcorrencia.NumeroOcorrencia";

            if (propriedadeOrdenar == "Tipo")
                return "CTe.ModeloDocumentoFiscal.Abreviacao";

            if (propriedadeOrdenar == "Destinatario")
                return "Destinatario.Nome";

            if (propriedadeOrdenar == "Tomador")
                return "CTe.TomadorPagador.Cliente.Nome";

            if (propriedadeOrdenar == "DataEmissao")
                return "CTe.DataEmissao";

            if (propriedadeOrdenar == "Transportador")
                return "CTe.Empresa.RazaoSocial";

            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "ValorFrete")
                return "CTe.ValorFrete";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
