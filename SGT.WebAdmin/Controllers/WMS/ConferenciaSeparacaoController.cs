using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("Cargas/CargaControleExpedicao")]
    public class ConferenciaSeparacaoController : BaseController
    {
		#region Construtores

		public ConferenciaSeparacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridProdutosConferidos();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

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

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Expedicao"), out int codigoExpedicao);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao expedicao = repCargaControleExpedicao.BuscarPorCodigo(codigoExpedicao);

                if (!ValidaConferencia(expedicao.Carga.Codigo, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Inicia transacao
                unitOfWork.Start();

                expedicao.SituacaoCargaControleExpedicao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaControleExpedicao.Liberada;
                expedicao.Carga.SeparacaoConferida = true;

                repCargaControleExpedicao.Atualizar(expedicao);
                repCarga.Atualizar(expedicao.Carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, expedicao, null, "Finalizou a conferência", unitOfWork);

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


        public async Task<IActionResult> ValidaCodigoBarras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);

                // Converte valores
                int.TryParse(Request.Params("Carga"), out int carga);
                string codigoBarras = Request.Params("CodigoBarras") ?? string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume;

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao conferencia = repConferenciaSeparacao.BuscarPorCargaECodigoBarras(carga, codigoBarras, tipoMercadoria);
                Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produto = repSeparacaoProdutoEmbarcador.BuscarPorCargaECodigoBarra(carga, codigoBarras);

                var retorno = new
                {
                    JaConferido = conferencia != null,
                    Conferido = conferencia?.Quantidade ?? 0,
                    Quantidade = produto?.Quantidade ?? 0,
                    CodigoBarrasValido = produto == null ? repSeparacaoProdutoEmbarcador.ValidaCodigoBarrasPorCarga(codigoBarras, carga) : true,
                    //CodigoBarrasValido = (from o in controleExpedicao.Separacao.Produtos where o.ProdutoEmbarcadorLote.CodigoBarras == codigoBarras select o).Count() > 0
                };

                // Retorna sucesso
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
                Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);

                // Converte valores
                int.TryParse(Request.Params("Expedicao"), out int expedicao);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);
                if (quantidade == 0)
                    quantidade = 1;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume;

                string codigoBarras = Request.Params("CodigoBarras") ?? string.Empty;

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao conferencia = repConferenciaSeparacao.BuscarPorExpedicaoECodigoBarras(expedicao, codigoBarras, tipoMercadoria);
                if (conferencia == null)
                {
                    conferencia = new Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao()
                    {
                        CodigoBarras = codigoBarras.ToUpper(),
                        Expedicao = repCargaControleExpedicao.BuscarPorCodigo(expedicao)
                    };
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    if (conferencia.QuantidadeFaltante <= 0)
                        return new JsonpResult(false, true, "Código de barras já conferido.");
                }

                // Vincula dados
                conferencia.Quantidade += quantidade;
                conferencia.TipoRecebimentoMercadoria = tipoMercadoria;

                // Valida entidade
                if (!ValidaEntidade(conferencia, out string erro))
                    return new JsonpResult(false, true, erro);

                // Inicia transacao
                unitOfWork.Start();

                int carga = conferencia.Expedicao.Carga.Codigo;
                Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produto = repSeparacaoProdutoEmbarcador.BuscarPorCargaECodigoBarra(carga, codigoBarras);

                if (produto.Quantidade < conferencia.Quantidade)
                    return new JsonpResult(false, true, "A conferência ultrapassa a quantia máxima de " + produto.Quantidade.ToString("n3") + ".");

                // Persiste dados
                if (conferencia.Codigo > 0)
                    repConferenciaSeparacao.Atualizar(conferencia);
                else
                    repConferenciaSeparacao.Inserir(conferencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, conferencia, null, "Adicionou " + quantidade.ToString("n3") + " a conferência.", unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
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

        #region Métodos Privados
        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao conferencia, out string msgErro)
        {
            msgErro = "";

            if (conferencia.CodigoBarras.Length == 0)
            {
                msgErro = "Código de Barras é obrigatório.";
                return false;
            }

            if (conferencia.Quantidade == 0)
            {
                msgErro = "Quantidade é obrigatório.";
                return false;
            }

            if (conferencia.Expedicao == null)
            {
                throw new Exception("Expedição null.");
            }

            return true;
        }


        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaConferencia(int carga, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.WMS.ValidacaoConferenciaExpedicao> produtos = new List<Dominio.ObjetosDeValor.Embarcador.WMS.ValidacaoConferenciaExpedicao>();
            msgErro = "";

            Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
            Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador repSeparacaoProdutoEmbarcador = new Repositorio.Embarcador.WMS.SeparacaoProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.WMS.SelecaoCarga repSelecaoCarga = new Repositorio.Embarcador.WMS.SelecaoCarga(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume;

            // Conferencia zerada
            if (repConferenciaSeparacao.QuantidadeConferidaPorCarga(carga, tipoMercadoria) == 0)
            {
                msgErro = "Nenhuma conferência registrada.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> conferencia = repConferenciaSeparacao.BuscarPorCarga(carga, tipoMercadoria);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutos = repSelecaoCarga.BuscarProdutosCarga(carga);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaProdutosNaoConferidos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote> listaLotesProdutos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();
            for (int i = 0, s = listaProdutos.Count; i < s; i++)
            {
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote> lote = repSeparacaoProdutoEmbarcador.BuscarPorProdutoECarga(listaProdutos[i].Produto.Codigo, carga);
                if (lote == null)
                {
                    msgErro = "O lote do produto " + listaProdutos[i].Produto.Descricao + " não foi encontrado.";
                    return false;
                }

                listaLotesProdutos.AddRange(lote);
            }
            listaLotesProdutos = listaLotesProdutos.Distinct().ToList();

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                for (int i = 0, s = listaLotesProdutos.Count; i < s; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoPedido = (from p in listaProdutos where p.Produto.Codigo == listaLotesProdutos[i].ProdutoEmbarcador.Codigo select p).FirstOrDefault();
                    List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> conferenciaProdutos = (from c in conferencia where listaLotesProdutos[i].CodigoBarras.ToUpper().Contains(c.CodigoBarras.ToUpper()) select c).ToList();
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotas = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorCarga(carga);

                    if (conferenciaProdutos == null || conferenciaProdutos.Count == 0)                    
                        listaProdutosNaoConferidos.Add(produtoPedido);                    
                    else if (listaNotas != null && listaNotas.Count > 0)
                    {
                        foreach (var notaFiscal in listaNotas)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaVolume = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorCarga(carga, notaFiscal.Emitente.CPF_CNPJ, notaFiscal.Destinatario.CPF_CNPJ, notaFiscal.Numero);

                            if (notaVolume != null && notaVolume.Volumes > conferenciaProdutos.Sum(c => c.Quantidade))
                            {
                                for (int k = 0; k < notaVolume.Volumes; k++)
                                {
                                    if (conferenciaProdutos.Any(c => c.CodigoBarras == notaVolume.EtiquetaCodigoBarrasWMS(k + 1)))
                                    {
                                        produtos.Add(new Dominio.ObjetosDeValor.Embarcador.WMS.ValidacaoConferenciaExpedicao()
                                        {
                                            Produto = produtoPedido?.Produto?.Descricao ?? "Diversos",
                                            Faltante = notaVolume.Volumes - conferenciaProdutos.Sum(c => c.Quantidade)
                                        });
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (conferenciaProdutos == null && produtoPedido != null)
                    {
                        listaProdutosNaoConferidos.Add(produtoPedido);
                    }
                }
            }
            else
            {
                // Produtos que não constam na lista
                for (int i = 0, s = listaLotesProdutos.Count; i < s; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoPedido = (from p in listaProdutos where p.Produto.Codigo == listaLotesProdutos[i].ProdutoEmbarcador.Codigo select p).FirstOrDefault();
                    Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao conferenciaProduto = (from c in conferencia where listaLotesProdutos[i].CodigoBarras.ToUpper().Contains(c.CodigoBarras.ToUpper()) select c).FirstOrDefault();

                    if (produtoPedido != null && conferenciaProduto != null && produtoPedido.Quantidade > conferenciaProduto.Quantidade)
                    {
                        produtos.Add(new Dominio.ObjetosDeValor.Embarcador.WMS.ValidacaoConferenciaExpedicao()
                        {
                            Produto = produtoPedido.Produto.Descricao,
                            Faltante = produtoPedido.Quantidade - conferenciaProduto.Quantidade
                        });
                    }
                    else if (conferenciaProduto == null)
                    {
                        listaProdutosNaoConferidos.Add(produtoPedido);
                    }
                }
            }


            if (produtos.Count > 0)
            {
                List<string> produtosInsuficientes = (from p in produtos select "<li>" + p.Produto + " - " + p.Faltante.ToString("n3") + "</li>").ToList();

                msgErro = "Os seguintes produtos não foram conferidos por completo:<br>";
                msgErro += "<ul>" + String.Join("", produtosInsuficientes) + "</ul>";
                return false;
            }

            // Produtos que não constam na lista
            if (listaProdutosNaoConferidos.Count > 0)
            {
                List<string> produtosNaoConferidos = (from p in listaProdutosNaoConferidos select "<li>" + p.Produto.Descricao + "</li>").ToList();

                msgErro = "Os seguintes produtos não foram conferidos:<br>";
                msgErro += "<ul>" + String.Join("", produtosNaoConferidos) + "</ul>";
                return false;
            }

            return true;
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridProdutosConferidos()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Carga").Nome("Carga").Tamanho(15);
            grid.Prop("CodigoBarras").Nome("Cod. de Barras").Tamanho(15);
            grid.Prop("Quantidade").Nome("Quantidade").Tamanho(10).Align(Models.Grid.Align.right);

            return grid;
        }


        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Carga") propOrdenar = "Expedicao.Carga.CodigoCargaEmbarcador";
        }


        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao = new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Expedicao"), out int codigoExpedicao);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao expedicao = repCargaControleExpedicao.BuscarPorCodigo(codigoExpedicao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                tipoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume;
            // Consulta
            List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> listaGrid = repConferenciaSeparacao.Consultar(tipoMercadoria, expedicao.Carga.Codigo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repConferenciaSeparacao.ContarConsulta(tipoMercadoria, expedicao.Carga.Codigo);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Carga = obj.Expedicao.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                            CodigoBarras = obj.CodigoBarras,
                            Quantidade = obj.Quantidade.ToString("n3"),
                        };

            return lista.ToList();
        }
        #endregion
    }
}
