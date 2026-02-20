using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/LoteContabilizacao")]
    public class LoteContabilizacaoController : BaseController
    {
		#region Construtores

		public LoteContabilizacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> documentos = BuscarMovimentosSelecionados(unitOfWork);

                if (documentos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum movimento encontrado para criar o lote.");

                Repositorio.Embarcador.Financeiro.LoteContabilizacao repLoteContabilizacao = new Repositorio.Embarcador.Financeiro.LoteContabilizacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil repDocumentoExportacaoContabil = new Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao = new Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao();

                PreencherEntidade(loteContabilizacao, unitOfWork);

                unitOfWork.Start();

                loteContabilizacao.Situacao = SituacaoLoteContabilizacao.AgIntegracao;
                loteContabilizacao.Usuario = Usuario;
                loteContabilizacao.Numero = repLoteContabilizacao.BuscarUltimoNumero() + 1;

                repLoteContabilizacao.Inserir(loteContabilizacao, Auditado);
                repDocumentoExportacaoContabil.SetarLoteContabilizacao(loteContabilizacao.Codigo, documentos.Select(o => o.Codigo).ToList());

                unitOfWork.CommitChanges();

                return new JsonpResult(new { loteContabilizacao.Codigo });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.LoteContabilizacao repLoteContabilizacao = new Repositorio.Embarcador.Financeiro.LoteContabilizacao(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao = repLoteContabilizacao.BuscarPorCodigo(codigo, false);

                if (loteContabilizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    loteContabilizacao.Codigo,
                    loteContabilizacao.Numero,
                    loteContabilizacao.Situacao,
                    loteContabilizacao.Tipo,
                    Tomador = new { Descricao = loteContabilizacao.Tomador?.Descricao ?? string.Empty, Codigo = loteContabilizacao.Tomador?.Codigo ?? 0 },
                    DataInicial = loteContabilizacao.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataFinal = loteContabilizacao.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                    Usuario = new { Descricao = loteContabilizacao.Usuario?.Descricao ?? string.Empty, Codigo = loteContabilizacao.Usuario?.Codigo ?? 0 },
                    DataGeracaoLote = loteContabilizacao.DataGeracaoLote?.ToString("dd/MM/yyyy"),
                    ModeloDocumentoFiscal = new { Codigo = loteContabilizacao.ModeloDocumentoFiscal?.Codigo ?? 0, Descricao = loteContabilizacao.ModeloDocumentoFiscal?.Descricao ?? string.Empty },
                    Empresa = new { Descricao = loteContabilizacao.Empresa?.Descricao ?? string.Empty, Codigo = loteContabilizacao.Empresa?.Codigo ?? 0 },
                    loteContabilizacao.NumeroDocumento
                });
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

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
        public async Task<IActionResult> ExportarPesquisaMovimento()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaMovimento(exportarPesquisa: true);
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
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumento()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaMovimento(exportarPesquisa: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao loteContabilizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            int codigoEmpresa = Request.GetIntParam("Empresa");
            int codigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal");

            double cpfCnpjTomador = Request.GetDoubleParam("Tomador");

            loteContabilizacao.DataFinal = Request.GetNullableDateTimeParam("DataFim");
            loteContabilizacao.DataGeracaoLote = DateTime.Now;
            loteContabilizacao.DataInicial = Request.GetNullableDateTimeParam("DataInicio");
            loteContabilizacao.NumeroDocumento = Request.GetStringParam("NumeroDocumento");
            loteContabilizacao.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            loteContabilizacao.ModeloDocumentoFiscal = codigoModeloDocumentoFiscal > 0 ? repModeloDocumentoFiscal.BuscarPorCodigo(codigoModeloDocumentoFiscal, false) : null;
            loteContabilizacao.Tomador = cpfCnpjTomador > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;
            loteContabilizacao.Tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao>("Tipo");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int numero = Request.GetIntParam("Numero");
                int codigoEmpresa = Request.GetIntParam("Empresa");

                double cpfCnpjTomador = Request.GetDoubleParam("Tomador");

                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicio");
                DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFim");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao? tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao>("Tipo");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteContabilizacao>("Situacao");

                string numeroDocumento = Request.GetStringParam("NumeroDocumento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Financeiro.LoteContabilizacao repLoteContabilizacao = new Repositorio.Embarcador.Financeiro.LoteContabilizacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacao> lotesContabilizacoes = repLoteContabilizacao.Consultar(numero, codigoEmpresa, cpfCnpjTomador, dataInicial, dataFinal, tipo, situacao, numeroDocumento, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repLoteContabilizacao.ContarConsulta(numero, codigoEmpresa, cpfCnpjTomador, dataInicial, dataFinal, tipo, situacao, numeroDocumento);

                var retorno = (from obj in lotesContabilizacoes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   DataInicial = obj.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   DataFinal = obj.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   Tomador = obj.Tomador?.Descricao,
                                   Empresa = obj.Empresa?.Descricao,
                                   Tipo = obj.Tipo?.ObterDescricao() ?? "Todos",
                                   Situacao = obj.Situacao.ObterDescricao()
                               }).ToList();

                grid.AdicionaRows(retorno);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private void ExecutaPesquisaMovimento(ref List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportarPesquisa, Repositorio.UnitOfWork unitOfWork, bool selecaoParaAdicionar = false)
        {
            Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil repDocumentoContabilizacao = new Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa = ObterFiltrosConsulta(selecaoParaAdicionar);

            totalRegistros = repDocumentoContabilizacao.ContarConsultaNovo(filtrosPesquisa);
            // TODO: ToList Cast
            if (totalRegistros > 0)
                listaGrid = repDocumentoContabilizacao.ConsultarNovo(filtrosPesquisa, parametrosConsulta).ToList();
        }

        private Models.Grid.Grid ObterGridPesquisaMovimento(bool exportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTomador", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("CodigoModeloDocumento", false);
                grid.AdicionarCabecalho("ModeloDocumento", false);
                grid.AdicionarCabecalho("Documento", "Documento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Movimento", "DataMovimento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, true);

                if (exportarPesquisa)
                    grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivoEDI", 20, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);

                List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> listaGrid = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil>();
                int totalRegistros = 0;

                ExecutaPesquisaMovimento(ref listaGrid, ref totalRegistros, parametrosConsulta, exportarPesquisa, unitOfWork);

                grid.AdicionaRows(listaGrid);
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
            return propriedadeOrdenar;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> BuscarMovimentosSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil repDocumentoExportacaoContabil = new Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> listaBusca = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil>();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                PropriedadeOrdenar = "Codigo",
                DirecaoOrdenar = "asc"
            };

            int totalRegistros = 0;

            ExecutaPesquisaMovimento(ref listaBusca, ref totalRegistros, parametrosConsulta, false, unitOfWork, true);

            return listaBusca;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao ObterFiltrosConsulta(bool incluirMultiplaSelecao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtros = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao()
            {
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoLoteContabilizacao = Request.GetIntParam("Codigo"),
                CodigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao>("Tipo"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento")
            };

            if (incluirMultiplaSelecao)
            {
                filtros.SelecionarTodos = Request.GetBoolParam("SelecionarTodos");
                filtros.CodigosSelecionados = Request.GetListParam<int>("MovimentosSelecionados");
            }

            return filtros;
        }

        #endregion
    }
}
