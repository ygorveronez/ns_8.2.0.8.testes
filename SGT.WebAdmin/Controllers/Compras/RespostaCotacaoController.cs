using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize("Compras/RespostaCotacao")]
    public class RespostaCotacaoController : BaseController
    {
		#region Construtores

		public RespostaCotacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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


        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repCotacaoRetornoFornecedorProduto = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoProduto repCotacaoProduto = new Repositorio.Embarcador.Compras.CotacaoProduto(unitOfWork);
                Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                double fornecedor = 501624627;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    fornecedor = this.Usuario.ClienteFornecedor.CPF_CNPJ;
                }

                // Busca informacoes
                List<Dominio.Entidades.Embarcador.Compras.CotacaoProduto> produtosCotacao = repCotacaoProduto.BuscarPorCotacao(codigo);
                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repCotacaoRetornoFornecedorProduto.BuscarPorCotacaoEFornecedor(codigo, fornecedor);
                Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto retornoAux = retornos.FirstOrDefault();

                if (retornos.Count == 0 && produtosCotacao.Count > 0)
                {
                    unitOfWork.Start();

                    foreach (var produto in produtosCotacao)
                    {
                        Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto retornoProduto = new Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto();
                        retornoProduto.CondicaoPagamento = null;
                        retornoProduto.CotacaoFornecedor = repCotacaoFornecedor.BuscarPorCodigoFornecedor(codigo, fornecedor);
                        retornoProduto.CotacaoProduto = produto;
                        retornoProduto.DataPrevisaoEntrega = null;
                        retornoProduto.GerarOrdemCompra = false;
                        retornoProduto.Observacao = string.Empty;
                        retornoProduto.QuantidadeRetorno = 0;
                        retornoProduto.ValorTotalRetorno = 0;
                        retornoProduto.ValorUnitarioRetorno = 0;
                        retornoProduto.Marca = string.Empty;

                        repCotacaoRetornoFornecedorProduto.Inserir(retornoProduto);
                    }
                    unitOfWork.CommitChanges();

                    retornos = repCotacaoRetornoFornecedorProduto.BuscarPorCotacaoEFornecedor(codigo, fornecedor);
                    retornoAux = retornos.FirstOrDefault();
                }

                // Formata retorno
                var retorno = new
                {
                    Codigo = codigo,
                    DataPrevisaoEntrega = retornoAux?.DataPrevisaoEntrega?.ToString("dd/MM/yyyy") ?? string.Empty,
                    CondicaoPagamento = retornoAux?.CondicaoPagamento != null ? new { retornoAux.CondicaoPagamento.Codigo, retornoAux.CondicaoPagamento.Descricao } : null,
                    Observacao = retornoAux?.Observacao ?? string.Empty,
                    Produtos = (from obj in retornos
                                select new
                                {
                                    obj.Codigo,
                                    Produto = obj.CotacaoProduto.Produto.Descricao,
                                    ValorUnitario = obj.CotacaoProduto.ValorUnitario.ToString("n4"),
                                    ValorUnitarioRetorno = obj.ValorUnitarioRetorno.ToString("n4"),
                                    Quantidade = obj.CotacaoProduto.Quantidade.ToString("n4"),
                                    QuantidadeRetorno = obj.QuantidadeRetorno.ToString("n4"),
                                    Marca = !string.IsNullOrWhiteSpace(obj.Marca) ? obj.Marca : string.Empty
                                }).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repCotacaoRetornoFornecedorProduto = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unitOfWork);
                Repositorio.Embarcador.Compras.Cotacao repCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);
                Repositorio.Embarcador.Compras.CondicaoPagamento repCondicaoPagamento = new Repositorio.Embarcador.Compras.CondicaoPagamento(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoCotacao);
                List<dynamic> retornos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Produtos"));

                DateTime.TryParse(Request.Params("DataPrevisaoEntrega"), out DateTime dataPrevisaoEntrega);
                int.TryParse(Request.Params("CondicaoPagamento"), out int codigoCondicaoPagamento);
                string observacao = Request.Params("Observacao") ?? string.Empty;

                Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao = repCotacao.BuscarPorCodigo(codigoCotacao);
                Dominio.Entidades.Embarcador.Compras.CondicaoPagamento condicaoPagamento = repCondicaoPagamento.BuscarPorCodigo(codigoCondicaoPagamento);

                if (cotacao == null || retornos == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cotacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.AguardandoRetorno)
                    return new JsonpResult(false, true, "Situação não permite essa operação.");

                if (dataPrevisaoEntrega == DateTime.MinValue)
                    return new JsonpResult(false, true, "Data de Previsão é obrigatório.");

                // Persiste dados
                unitOfWork.Start();

                cotacao.Initialize();
                Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = repCotacao.Atualizar(cotacao, Auditado);

                foreach (dynamic dynRetorno in retornos)
                {
                    int.TryParse((string)dynRetorno.Codigo, out int codigo);
                    Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto retorno = repCotacaoRetornoFornecedorProduto.BuscarPorCodigo(codigo);

                    if (retorno == null) continue;

                    decimal quantidade = Utilidades.Decimal.Converter((string)dynRetorno.QuantidadeRetorno);
                    decimal valorUnitario = Utilidades.Decimal.Converter((string)dynRetorno.ValorUnitarioRetorno);
                    string marca = (string)dynRetorno.Marca;

                    retorno.Initialize();
                    retorno.QuantidadeRetorno = quantidade;
                    retorno.ValorUnitarioRetorno = valorUnitario;
                    retorno.ValorTotalRetorno = quantidade * valorUnitario;
                    retorno.DataPrevisaoEntrega = dataPrevisaoEntrega;
                    retorno.CondicaoPagamento = condicaoPagamento;
                    retorno.Observacao = observacao;
                    retorno.Marca = marca;

                    repCotacaoRetornoFornecedorProduto.Atualizar(retorno, Auditado, historicoPai);
                }

                Servicos.Embarcador.Compras.CotacaoCompra.AtualizaCotacaoVencedora(cotacao, unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome("Descrição").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Numero").Nome("Número").Tamanho(20).Align(Models.Grid.Align.right);
            grid.Prop("DataEmissao").Nome("Data Emissão").Tamanho(20).Align(Models.Grid.Align.center);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Compras.Cotacao repRespostaCotacao = new Repositorio.Embarcador.Compras.Cotacao(unitOfWork);

            // Dados do filtro
            double fornecedor = 501624627;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                fornecedor = this.Usuario.ClienteFornecedor.CPF_CNPJ;
            }

            // Consulta
            List<Dominio.Entidades.Embarcador.Compras.CotacaoCompra> listaGrid = repRespostaCotacao.ConsultarCotacoesPendentes(fornecedor, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRespostaCotacao.ContarConsultaCotacoesPendentes(fornecedor);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            obj.Descricao,
                            DataEmissao = obj.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty
                        };

            return lista.ToList();
        }

        #endregion
    }
}
