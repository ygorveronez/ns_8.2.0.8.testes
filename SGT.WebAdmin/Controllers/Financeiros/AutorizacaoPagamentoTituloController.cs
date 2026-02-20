using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/AutorizacaoPagamentoTitulo")]
    public class AutorizacaoPagamentoTituloController : BaseController
    {
		#region Construtores

		public AutorizacaoPagamentoTituloController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa = ObterFiltrosPesquisa();
                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                DateTime? dataAutorizacao = Request.GetNullableDateTimeParam("DataAutorizacao");

                List<int> codigosTitulos = new List<int>();

                if (!selecionarTodos)
                    codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaTitulos"));
                else
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarAutorizacaoPagamento(filtrosPesquisa, "Codigo", "desc", 0, 0);
                    if (listaTitulos != null && listaTitulos.Count > 0)
                        codigosTitulos = listaTitulos.Select(o => o.Codigo).Distinct().ToList();
                }

                unitOfWork.Start();

                if (codigosTitulos?.Count > 0)
                {
                    if (dataAutorizacao.HasValue && dataAutorizacao.Value > DateTime.MinValue)
                        repTitulo.InformarDataAutorizacao(codigosTitulos, dataAutorizacao.Value);
                    else
                        repTitulo.RemoverDataAutorizacao(codigosTitulos);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes dos títulos selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int codigoTitulo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Valor");

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo, true);

                if (titulo == null)
                    return new JsonpResult(false, false, "Título não encontrado.");

                unitOfWork.Start();

                titulo.Observacao = observacao;
                repTitulo.Atualizar(titulo, Auditado);

                unitOfWork.CommitChanges();

                var resultado = new
                {
                    titulo.Codigo,
                    titulo.Pessoa.Descricao,
                    DataEmissao = titulo.DataEmissao?.ToString("dd/MM/yyyy") ?? "",
                    DataVencimento = titulo.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                    titulo.NumeroDocumentoTituloOriginal,
                    DataAutorizacao = titulo.DataAutorizacao?.ToString("dd/MM/yyyy") ?? "",
                    ValorOriginal = titulo.ValorOriginal.ToString("n2"),
                    CentroResultado = titulo.CentrosResultado,
                    titulo.Observacao,
                    titulo.NossoNumero
                };

                return new JsonpResult(resultado, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a observação do título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDocumentosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa = ObterFiltrosPesquisa();

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

                List<int> codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaTitulos"));

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ObterDocumentosSelecionadosAutorizacaoPagamento(selecionarTodos, codigosTitulos, filtrosPesquisa);

                return new JsonpResult(new
                {
                    QuantidadeTitulosSelecionados = listaTitulos.Count,
                    ValorOriginalTitulosSelecionados = listaTitulos.Count > 0 ? listaTitulos.Select(obj => obj.ValorOriginal).Sum().ToString("n2") : 0.ToString("n2")
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os totalizadores.");
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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento()
            {
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                Fornecedor = Request.GetDoubleParam("Fornecedor"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                NumeroTitulo = Request.GetIntParam("NumeroTitulo"),
                SituacaoAutorizacao = Request.GetEnumParam<SituacaoAutorizacao>("SituacaoAutorizacao"),
                TipoTituloNegociacao = Request.GetEnumParam<TipoTituloNegociacao>("TipoTituloNegociacao"),
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                SituacaoBoletoTitulo = Request.GetEnumParam<SituacaoBoletoTitulo>("SituacaoBoletoTitulo"),
                TiposDocumento = Request.GetListEnumParam<TipoDocumentoPesquisaTitulo>("TipoDocumento"),
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.EditableCell editableValorString = new Models.Grid.EditableCell(TipoColunaGrid.aString, 1000);
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Fornecedor", "Pessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Documento", "NumeroDocumentoTituloOriginal", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código de Barras", "NossoNumero", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Título", "Codigo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Autorização", "DataAutorizacao", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "ValorOriginal", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 30, Models.Grid.Align.left, false, false, false, false, true, editableValorString);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarAutorizacaoPagamento(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarConsultaAutorizacaoPagamento(filtrosPesquisa));

                var lista = (from p in listaTitulos
                             select new
                             {
                                 p.Codigo,
                                 Pessoa = p.Pessoa.Descricao,
                                 DataEmissao = p.DataEmissao?.ToString("dd/MM/yyyy") ?? "",
                                 DataVencimento = p.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                                 p.NumeroDocumentoTituloOriginal,
                                 DataAutorizacao = p.DataAutorizacao?.ToString("dd/MM/yyyy") ?? "",
                                 ValorOriginal = p.ValorOriginal.ToString("n2"),
                                 CentroResultado = p.CentrosResultado,
                                 Observacao = p.Observacao,
                                 p.NossoNumero
                             }).ToList();

                grid.AdicionaRows(lista);

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
        #endregion
    }
}
