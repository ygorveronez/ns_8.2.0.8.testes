using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/LoteEscrituracaoMiro")]
    public class LoteEscrituracaoMiroController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoMiroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

                DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
                DateTime dataFim = Request.GetDateTimeParam("DataFim");

                double cpnjTomador = Request.GetDoubleParam("Tomador");

                int codigoFilial = Request.GetIntParam("Filial");
                int codigoEmpresa = Request.GetIntParam("Transportador");
                int codigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal");

                // Busca os documentos selecionados
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> documentos = BuscarDocumentosSelecionados(unitOfWork, out string erro);

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                if (documentos.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum documento selecionado.");

                Dominio.Entidades.Cliente tomador = null;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Empresa empresa = null;
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigoModeloDocumentoFiscal);

                if (cpnjTomador > 0D)
                    tomador = repCliente.BuscarPorCPFCNPJ(cpnjTomador);
                if (codigoFilial > 0)
                    filial = repFilial.BuscarPorCodigo(codigoFilial);
                if (codigoEmpresa > 0)
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                unitOfWork.Start();

                Servicos.Embarcador.Escrituracao.Escrituracao servicoEscrituracao = new Servicos.Embarcador.Escrituracao.Escrituracao(unitOfWork, Auditado);
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento = servicoEscrituracao.AdicionarLoteEscrituracaoCancelamento(dataInicio, dataFim, tomador, filial, empresa, modeloDocumentoFiscal, documentos);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    loteEscrituracaoCancelamento.Codigo
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
                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento repLoteEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento loteEscrituracaoCancelamento = repLoteEscrituracaoCancelamento.BuscarPorCodigo(codigo);

                // Valida
                if (loteEscrituracaoCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    loteEscrituracaoCancelamento.Codigo,
                    loteEscrituracaoCancelamento.Situacao,
                    DataInicial = loteEscrituracaoCancelamento.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = loteEscrituracaoCancelamento.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    loteEscrituracaoCancelamento.Numero,
                    Filial = new { Codigo = loteEscrituracaoCancelamento.Filial?.Codigo ?? 0, Descricao = loteEscrituracaoCancelamento.Filial?.Descricao ?? "" },
                    Transportador = new { Codigo = loteEscrituracaoCancelamento.Empresa?.Codigo ?? 0, Descricao = loteEscrituracaoCancelamento.Empresa?.Descricao ?? "" },
                    ModeloDocumentoFiscal = new { Codigo = loteEscrituracaoCancelamento.ModeloDocumentoFiscal?.Codigo ?? 0, Descricao = loteEscrituracaoCancelamento.ModeloDocumentoFiscal?.Descricao ?? "" },
                    Tomador = new { Codigo = loteEscrituracaoCancelamento.Tomador?.Codigo ?? 0, Descricao = loteEscrituracaoCancelamento.Tomador?.Descricao ?? "" },
                    DescricaoSituacao = loteEscrituracaoCancelamento.Situacao.ObterDescricao()
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
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = ObterGridPesquisa();

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiro repositorioLoteEscrituracaoMiro = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiro(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.LoteEscrituracaoMiroFiltro filtrosPesquisa = ObterFiltroPesquisa();
                List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro> listaLotes = repositorioLoteEscrituracaoMiro.Consultar(filtrosPesquisa, grid.ObterParametrosConsulta());

                int totalRegistros = repositorioLoteEscrituracaoMiro.ContaConsulta(filtrosPesquisa);
                var lotesRetornar = (from lote in listaLotes
                                     select new
                                     {
                                         NumeroLote = lote.Codigo,
                                         CodigoTransportador = lote?.Carga?.Empresa?.Codigo ?? 0,
                                         Transportador = lote?.Empresa?.Descricao ?? string.Empty,
                                         Carga = lote.Carga?.CodigoCargaEmbarcador ?? "",
                                         Data = lote?.DataGeracaoLote?.ToString("dd/MM/yyyy") ?? "",
                                     }).ToList();

                grid.AdicionaRows(lotesRetornar);
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
                return new JsonpResult(ObterGridPesquisaDocumento());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        //[AllowAuthenticate]
        //public async Task<IActionResult> ExportarPesquisaDocumento()
        //{
        //    try
        //    {
        //        Models.Grid.Grid grid = ObterGridPesquisaDocumento(exportarPesquisa: true);
        //        byte[] arquivoBinario = grid.GerarExcel();

        //        if (arquivoBinario != null)
        //            return Arquivo(arquivoBinario, grid.extensaoCSV, $"{grid.tituloExportacao}.{grid.extensaoCSV}");

        //        return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
        //    }
        //    catch (Exception excecao)
        //    {
        //        Servicos.Log.TratarErro(excecao);

        //        return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
        //    }
        //}
        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> BuscarDocumentosSelecionados(Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> listaBusca = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            int transportador = 1;
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                int.TryParse(Request.Params("Transportador"), out transportador);

            int.TryParse(Request.Params("Filial"), out int filial);
            double.TryParse(Request.Params("Tomador"), out double tomador);

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

                    //ExecutaPesquisaDocumento(ref listaBusca, ref totalRegistros, parametrosConsulta, exportarPesquisa, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    listaBusca.RemoveAll(o => o.Codigo == (int)dynNaoSelecionada.Codigo);
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                foreach (var dynSelecionada in listaSelecionadas)
                    listaBusca.Add(repDocumentoEscrituracaoCancelamento.BuscarPorCodigo((int)dynSelecionada.Codigo, false));
            }

            // Retorna lista
            return listaBusca;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, bool somenteAtivo, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento repLoteEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento(unitOfWork);

            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");

            int filial = Request.GetIntParam("Filial");
            int carga = Request.GetIntParam("Carga");
            int ocorrencia = Request.GetIntParam("Ocorrencia");
            int numero = Request.GetIntParam("Numero");
            int numeroDOC = Request.GetIntParam("NumeroDOC");
            int localidadePrestacao = Request.GetIntParam("LocalidadePrestacao");
            int transportador = Request.GetIntParam("Empresa");

            double tomador = Request.GetDoubleParam("Tomador");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento>("Situacao");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                transportador = this.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> listaGrid = repLoteEscrituracaoCancelamento.Consultar(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLoteEscrituracaoCancelamento.ContarConsulta(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            DataInicio = obj.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                            DataFim = obj.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                            Tomador = obj.Tomador?.Descricao ?? "",
                            Transportador = obj.Empresa?.Descricao ?? "",
                            Filial = obj.Filial?.Descricao ?? "",
                            Situacao = obj.Situacao.ObterDescricao()
                        };

            return lista.ToList();
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Número Lote", "Codigo", 2, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "Carga", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true);
            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo",false);
                grid.AdicionarCabecalho("Carga", "NumeroCarga", 3, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Chave", "Chave", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Numero", "Numero", 2, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Empresa", "Empresa", 4, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 2, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 4, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);
                Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.LoteEscrituracaoMiroFiltro filtrosPesquisa = ObterFiltroPesquisa();
                List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> listaLotes = repositorioControleDocumento.Consultar(filtrosPesquisa, parametrosConsulta);

                int totalRegistros = repositorioControleDocumento.ContarConsulta(filtrosPesquisa);
                var lotesRetornar = (from doc in listaLotes
                                     select new
                                     {
                                         doc.Codigo,
                                         NumeroCarga = doc?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                         Chave = doc?.CargaCTe?.CTe?.Chave ?? string.Empty,
                                         Numero = doc?.CargaCTe?.CTe?.Numero ?? 0,
                                         Empresa = doc?.Carga?.Empresa?.CNPJ ?? string.Empty,
                                         DataEmissao = doc?.CTe.DataEmissao?.ToString("dd/MM/yyyy"),
                                         Tipo = doc?.CTe?.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao()
                                     }).ToList();

                grid.AdicionaRows(lotesRetornar);
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

        private string ObterPropriedadeOrdenarDocumentos(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Documento")
                return "CTe.Numero";

            if (propriedadeOrdenar == "Carga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Tipo")
                return "CTe.ModeloDocumentoFiscal.Abreviacao";

            if (propriedadeOrdenar == "DataEmissao")
                return "CTe.DataEmissao";

            if (propriedadeOrdenar == "ValorFrete")
                return "CTe.ValorFrete";

            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.LoteEscrituracaoMiroFiltro ObterFiltroPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.LoteEscrituracaoMiroFiltro()
            {
                Carga = Request.GetIntParam("Carga"),
                DataFim = Request.GetNullableDateTimeParam("DataInicio"),
                DataInicio = Request.GetNullableDateTimeParam("DataFim"),
                Chave = Request.GetStringParam("Chave"),
                Empresa = Request.GetIntParam("Empresa"),
                Situacao = Request.GetEnumParam<SituacaoLoteEscrituracaoMiro>("Sitaucao"),
                NumeroLote = Request.GetIntParam("Numero")
            };
        }
        #endregion
    }
}
