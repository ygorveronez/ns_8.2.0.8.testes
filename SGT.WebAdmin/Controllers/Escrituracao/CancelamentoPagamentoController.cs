using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/CancelamentoPagamento")]
    public class CancelamentoPagamentoController : BaseController
    {
		#region Construtores

		public CancelamentoPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento repMotivoCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                DateTime? dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                DateTime? dataFim = Request.GetNullableDateTimeParam("DataFim");

                double tomador = Request.GetDoubleParam("Tomador");

                int filial = Request.GetIntParam("Filial");
                int empresa = Request.GetIntParam("Transportador");
                int carga = Request.GetIntParam("Carga");
                int ocorrencia = Request.GetIntParam("Ocorrencia");
                List<int> codigosPagamento = Request.GetListParam<int>("Pagamento");
                int motivoCancelamento = Request.GetIntParam("MotivoCancelamento");


                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento
                {
                    DataInicial = dataInicio,
                    DataFinal = dataFim,
                    DataCriacao = DateTime.Now,
                    MotivoCancelamento = repMotivoCancelamentoPagamento.BuscarPorCodigo(motivoCancelamento),
                    Numero = repCancelamentoPagamento.ObterProximoNumero(),
                    Tomador = tomador > 0 ? repCliente.BuscarPorCPFCNPJ(tomador) : null,
                    Filial = filial > 0 ? repFilial.BuscarPorCodigo(filial) : null,
                    Empresa = empresa > 0 ? repEmpresa.BuscarPorCodigo(empresa) : null,
                    Pagamentos = new List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>(),
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.EmCancelamento
                };

                List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = codigosPagamento.Count() > 0 ? repPagamento.BuscarPorCodigo(codigosPagamento) : new List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();

                if (repDocumentoProvisao.ExisteProvisaoCanceladaPorPagamentos(pagamentos.Select(obj => obj.Codigo).Distinct().ToList()))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(true, false, "As provisões já foram canceladas para a contrapartida desses documentos, desta forma não é possível mais efetivar esse cancelamento.");
                }

                List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> pagamentosIntegracoes = repPagamentoIntegracao.BuscarPorPagamento(pagamentos.Select(obj => obj.Codigo).Distinct().ToList());

                if (ExistemPagamentosIntegracaoComSituacaoAgRetornoOuIntegrado(pagamentosIntegracoes))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(true, false, "Não é possível gerar o cancelamento porque há documentos com situações integrados ou aguardando retorno da integração.");
                }

                foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentos)
                    cancelamento.Pagamentos.Add(pagamento);

                if (cancelamento.MotivoCancelamento == null)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(true, false, "É obrigatório selecionar o motivo do cancelamento.");
                }

                repCancelamentoPagamento.Inserir(cancelamento, Auditado);
                SetarDocumentosSelecionados(cancelamento, carga, ocorrencia, dataInicio ?? DateTime.MinValue, dataFim ?? DateTime.MinValue, tomador, filial, empresa, codigosPagamento, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    cancelamento.Codigo
                });
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
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
                Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento = repCancelamentoPagamento.BuscarPorCodigo(codigo);

                // Valida
                if (cancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    cancelamento.Codigo,
                    cancelamento.Situacao,
                    DataInicial = cancelamento.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = cancelamento.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    cancelamento.Numero,
                    cancelamento.GerandoMovimentoFinanceiro,
                    cancelamento.MotivoRejeicaoFechamentoCancelamentoPagamento,
                    Pagamento = (from o in cancelamento.Pagamentos
                                 select new
                                 {
                                     o.Codigo,
                                     o.Descricao
                                 }).ToList(),
                    Filial = cancelamento.Filial != null ? new { cancelamento.Filial.Codigo, cancelamento.Filial.Descricao } : new { Codigo = 0, Descricao = "" },
                    Transportador = new { Codigo = cancelamento.Empresa?.Codigo ?? 0, Descricao = cancelamento.Empresa?.Descricao ?? "" },
                    Tomador = new { Codigo = cancelamento.Tomador?.Codigo ?? 0, Descricao = cancelamento.Tomador?.Nome ?? "" },
                    Carga = new { Codigo = cancelamento.Carga?.Codigo ?? 0, Descricao = cancelamento.Carga?.Descricao ?? "" },
                    Ocorrencia = new { Codigo = cancelamento.CargaOcorrencia?.Codigo ?? 0, Descricao = cancelamento.CargaOcorrencia?.Descricao ?? "" },
                    MotivoCancelamento = new { Codigo = cancelamento.MotivoCancelamento?.Codigo ?? 0, Descricao = cancelamento.MotivoCancelamento?.Descricao ?? "" },
                    cancelamento.DescricaoSituacao,
                    Resumo = new
                    {
                        cancelamento.Codigo,
                        Transportador = cancelamento.Empresa?.Descricao ?? string.Empty,
                        Filial = cancelamento.Filial?.Descricao ?? string.Empty,
                        Numero = cancelamento.Numero.ToString(),
                        Motivo = cancelamento.MotivoCancelamento?.Descricao ?? string.Empty,
                        Pagamento = string.Join(", ", cancelamento.Pagamentos.Select(o => o.Descricao)),
                        DataInicial = cancelamento.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFinal = cancelamento.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Carga = cancelamento.Carga?.Descricao ?? "",
                        Ocorrencia = cancelamento.CargaOcorrencia?.Descricao ?? "",
                        Situacao = cancelamento.DescricaoSituacao,
                        QuantidadeDocumentos = cancelamento.QuantidadeDocsCancelamentoPagamento.ToString(),
                        ValorCancelamentoPagamento = cancelamento.ValorCancelamentoPagamento.ToString("n2"),
                    }
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
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

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaDocumentos()
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

        #endregion

        #region Métodos Privados

        private dynamic ExecutaPesquisa(ref int totalRegistros, bool somenteAtivo, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unitOfWork);

            // Dados do filtro
            int transportador = 0;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                transportador = this.Empresa.Codigo;
            else
                transportador = Request.GetIntParam("Transportador");

            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFim = Request.GetDateTimeParam("DataFim");

            int filial = Request.GetIntParam("Filial");
            int carga = Request.GetIntParam("Carga");
            int ocorrencia = Request.GetIntParam("Ocorrencia");
            int numero = Request.GetIntParam("Numero");
            int numeroDOC = Request.GetIntParam("NumeroDOC");
            int localidadePrestacao = Request.GetIntParam("LocalidadePrestacao");
            int tomador = Request.GetIntParam("Tomador");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento>("Situacao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> listaGrid = repCancelamentoPagamento.Consultar(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCancelamentoPagamento.ContarConsulta(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            DataInicio = obj.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                            DataFim = obj.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                            Tomador = obj.Tomador?.Descricao ?? "",
                            QuantidadeDocumentos = obj.QuantidadeDocsCancelamentoPagamento,
                            ValorCancelamentoPagamento = obj.ValorCancelamentoPagamento.ToString("n2"),
                            Transportador = obj.Empresa?.Descricao ?? "",
                            Filial = obj.Filial?.Descricao ?? "",
                            obj.DescricaoSituacao
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportarPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            List<dynamic> listaRetornar = new List<dynamic>();
            int codigoCancelamentoPagamento = Request.GetIntParam("Codigo");
            List<int> pagamentos = Request.GetListParam<int>("Pagamento");

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPesquisa = ObterFiltroPesquisaDocumento();

            listaGrid = repositorioDocumentoFaturamento.Consultar(pagamentos, codigoCancelamentoPagamento, filtroPesquisa, false, false, false, parametrosConsulta, null);
            totalRegistros = repositorioDocumentoFaturamento.ContarConsulta(pagamentos, codigoCancelamentoPagamento, filtroPesquisa, false, false, false, null);

            if (listaGrid.Count > 0)
            {
                string nomeArquivoEDI = ObterNomeArquivoEDI(unitOfWork, codigoCancelamentoPagamento, exportarPesquisa);

                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento in listaGrid)
                {
                    listaRetornar.Add(new
                    {
                        documento.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                        SituacaoCTe = documento.CTe?.Status ?? "",
                        CodigoEmpresa = documento.Empresa.Codigo,
                        CodigoCTE = documento.CTe?.Codigo ?? 0,
                        documento.Codigo,
                        CodigoFilial = documento.Filial?.Codigo ?? 0,
                        CodigoTomador = documento.Tomador.Codigo,
                        Pagamento = documento.Pagamento?.Numero.ToString() ?? "",
                        CodigoTransportador = documento.Empresa.Codigo,
                        Transportador = documento.Empresa.Descricao,
                        Documento = documento.Numero + " - " + documento.EmpresaSerie?.Numero.ToString() ?? "",
                        Tipo = documento.ModeloDocumentoFiscal.Abreviacao,
                        Carga = documento.CargaPagamento?.CodigoCargaEmbarcador ?? "",
                        Ocorrencia = documento.CargaOcorrenciaPagamento?.NumeroOcorrencia.ToString() ?? "",
                        FechamentoFrete = documento.FechamentoFrete?.Numero.ToString() ?? "",
                        DataEmissao = documento.DataEmissao.ToString("dd/MM/yyyy"),
                        Tomador = documento.Tomador.Descricao,
                        Filial = documento.Filial?.Descricao ?? "",
                        TipoDocumentoCreditoDebito = documento.CTe?.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebitoDescricao ?? "",
                        ValorFrete = documento.ValorAFaturar.ToString("n2"),
                        NomeArquivoEDI = nomeArquivoEDI
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
            grid.AdicionarCabecalho("Data Inicio", "DataInicio", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Fim", "DataFim", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Qnt. Docs", "QuantidadeDocumentos", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor", "ValorCancelamentoPagamento", 10, Models.Grid.Align.center, true);
            //grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaDocumento(bool exportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoTomador", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pagamento", "Pagamento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, true);
                if (repFechamentoFrete.VerificarExistemFechamento())
                    grid.AdicionarCabecalho("Fechamento de Contrato", "FechamentoFrete", 8, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Emissão NF-e", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 10, Models.Grid.Align.right, true);

                if (exportarPesquisa)
                    grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivoEDI", 20, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaGrid = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
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

        private string ObterNomeArquivoEDI(Repositorio.UnitOfWork unitOfWork, int codigoCancelamentoPagamento, bool exportarPesquisa)
        {
            if (exportarPesquisa & (codigoCancelamentoPagamento > 0))
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao repositorio = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao integracao = repositorio.BuscarPorCancelamentoPagamento(codigoCancelamentoPagamento).FirstOrDefault();

                if (integracao != null)
                    return Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, true, unitOfWork);
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
            else if (propriedadeOrdenar == "QuantidadeDocumentos")
                propriedadeOrdenar = "QuantidadeDocsCancelamentoPagamento";
            else if (propriedadeOrdenar == "DescricaoSituacao")
                propriedadeOrdenar = "Situacao";
            else if (propriedadeOrdenar == "Filial")
                propriedadeOrdenar = "Filial.Descricao";

        }

        private string ObterPropriedadeOrdenarDocumentos(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Documento")
                propriedadeOrdenar = "Numero";
            else if (propriedadeOrdenar == "Carga")
                propriedadeOrdenar = "CargaPagamento.CodigoCargaEmbarcador";
            else if (propriedadeOrdenar == "FechamentoFrete")
                return "FechamentoFrete.Numero";
            else if (propriedadeOrdenar == "Ocorrencia")
                propriedadeOrdenar = "CargaOcorrenciaPagamento.NumeroOcorrencia";
            else if (propriedadeOrdenar == "Tipo")
                propriedadeOrdenar = "ModeloDocumentoFiscal.Abreviacao";
            else if (propriedadeOrdenar == "Destinatario")
                propriedadeOrdenar = "Destinatario.Nome";
            else if (propriedadeOrdenar == "Tomador")
                propriedadeOrdenar = "Tomador.Nome";
            else if (propriedadeOrdenar == "Transportador")
                propriedadeOrdenar = "Empresa.RazaoSocial";
            else if (propriedadeOrdenar == "TipoDocumentoCreditoDebito")
                propriedadeOrdenar = "CTe.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito";
            else if (propriedadeOrdenar == "Filial")
                propriedadeOrdenar = "Filial.Descricao";
            else if (propriedadeOrdenar == "ValorFrete")
                propriedadeOrdenar = "ValorAFaturar";

            return propriedadeOrdenar;
        }

        private void SetarDocumentosSelecionados(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento, int carga, int ocorrencia, DateTime dataInicio, DateTime dataFim, double tomador, int filial, int empresa, List<int> codigosPagamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamento repCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            int totalRegistros = 0;
            decimal valorCancelamentoPagamento = 0;
            bool todosSelecionados = Request.GetBoolParam("SelecionarTodos");

            if (todosSelecionados)
            {
                List<int> codigosNaoSelecionadas = new List<int>();
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("DocumentosNaoSelecionadas"));
                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    codigosNaoSelecionadas.Add((int)dynNaoSelecionada.Codigo);

                cancelamento.QuantidadeDocsCancelamentoPagamento = repDocumentoFaturamento.SetarDocumentosParaCancelamentoPagamento(cancelamento.Codigo, carga, ocorrencia, dataInicio, dataFim, tomador, filial, empresa, codigosPagamentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmCancelamento, codigosNaoSelecionadas);
                cancelamento.ValorCancelamentoPagamento = repDocumentoFaturamento.ValorTotalPorCancelamentoPagamento(cancelamento.Codigo);
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("DocumentosSelecionadas"));
                foreach (var dynSelecionada in listaSelecionadas)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo((int)dynSelecionada.Codigo, false);
                    if (documentoFaturamento.Pagamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Finalizado)
                    {
                        documentoFaturamento.CancelamentoPagamento = cancelamento;
                        documentoFaturamento.MovimentoFinanceiroGerado = false;
                        documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmCancelamento;
                        repDocumentoFaturamento.Atualizar(documentoFaturamento);
                        totalRegistros++;
                        valorCancelamentoPagamento += documentoFaturamento.ValorAFaturar;
                    }
                }
                cancelamento.ValorCancelamentoPagamento = valorCancelamentoPagamento;
                cancelamento.QuantidadeDocsCancelamentoPagamento = totalRegistros;
            }

            cancelamento.GerandoMovimentoFinanceiro = true;
            repCancelamentoPagamento.Atualizar(cancelamento);

            Servicos.Embarcador.Escrituracao.CancelamentoPagamento.ValidacaoParaPermitirGeraLoteCancelamento(cancelamento, unitOfWork);
        }

        private bool ExistemPagamentosIntegracaoComSituacaoAgRetornoOuIntegrado(List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> pagamentosIntegracoes)
        {
            return pagamentosIntegracoes.Any(r => (r.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ||
                r.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) && r.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz);
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento ObterFiltroPesquisaDocumento()
        {
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtro = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento()
            {
                CodigoPagamento = Request.GetIntParam("Codigo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoOcorrencia = Request.GetIntParam("Ocorrencia"),
                ModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                TipoOperacaoDocumento = Request.GetIntParam("TipoOperacaoDocumento"),
                PagamentoFinalizados = true,
                PagamentoLiberado = false,
                TipoDocumentoGerado = Request.GetNullableEnumParam<TipoDocumentoGerado>("TipoDocumentoGerado"),
                SituacaoPagamentoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Liquidado,
            };

            if (filtro.CodigoPagamento > 0)
                filtro.SituacaoPagamentoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Todos;

            int codigoTransportador = Request.GetIntParam("Transportador");
            if (codigoTransportador > 0)
                filtro.CodigosTransportador = new List<int> { codigoTransportador };

            int codigoFilial = Request.GetIntParam("Filial");
            if (codigoFilial > 0)
                filtro.CodigosFilial = new List<int> { codigoFilial };

            double codigoTomador = Request.GetDoubleParam("Tomador");
            if (codigoTomador > 0D)
                filtro.CodigosTomador = new List<double> { codigoTomador };

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            if (codigoTipoOperacao > 0)
                filtro.CodigosTiposOperacao = new List<int> { codigoTipoOperacao };

            return filtro;
        }

        #endregion
    }
}
