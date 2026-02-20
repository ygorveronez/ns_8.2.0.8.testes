using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/LoteEscrituracaoCancelamento")]
    public class LoteEscrituracaoCancelamentoController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoCancelamentoController(Conexao conexao) : base(conexao) { }

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

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportarPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento repDocumentoEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento(unitOfWork);

            dynamic listaRetornar = new List<dynamic>();

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracaoCancelamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracaoCancelamento()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoLoteEscrituracaoCancelamento = Request.GetIntParam("Codigo"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoModeloDocumento = Request.GetIntParam("ModeloDocumentoFiscal"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim")
            };

            listaGrid = repDocumentoEscrituracaoCancelamento.Consultar(filtrosPesquisa, parametrosConsulta);
            totalRegistros = repDocumentoEscrituracaoCancelamento.ContarConsulta(filtrosPesquisa);

            if (listaGrid.Count > 0)
            {
                string nomeArquivoEDI = ObterNomeArquivoEDI(unitOfWork, filtrosPesquisa.CodigoLoteEscrituracaoCancelamento, exportarPesquisa);

                listaRetornar = (from documento in listaGrid
                                 select new
                                 {
                                     documento.Codigo,
                                     CodigoFilial = documento.Filial?.Codigo ?? 0,
                                     CodigoTomador = documento.CTe.TomadorPagador.Cliente.Codigo,
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
                                     DataCancelamento = documento.CTe.DataCancelamento?.ToString("dd/MM/yyyy") ?? "",
                                     Tomador = documento.CTe.TomadorPagador.Cliente.Descricao,
                                     Filial = documento.Filial?.Descricao ?? "",
                                     ValorFrete = documento.CTe.ValorAReceber.ToString("n2"),
                                     NomeArquivoEDI = nomeArquivoEDI
                                 }).ToList();
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
            //grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);

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

                grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                }
                else
                {
                    grid.AdicionarCabecalho("Empresa/Filial", "Transportador", 20, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 15, Models.Grid.Align.right, true);

                if (exportarPesquisa)
                    grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivoEDI", 20, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);

                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> listaGrid = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

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
                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao integracao = repLoteEscrituracaoCancelamentoEDIIntegracao.BuscarPrimeiroPorLoteEscrituracao(codigoLoteEscrituracao);

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
