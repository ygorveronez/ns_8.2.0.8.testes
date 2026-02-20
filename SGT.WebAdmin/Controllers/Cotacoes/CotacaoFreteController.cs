using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cotacoes
{
    [CustomAuthorize("Cotacoes/CotacaoFrete")]
    public class CotacaoFreteController : BaseController
    {
		#region Construtores

		public CotacaoFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Cotacoes.FiltroPesquisaCotacaoFrete filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pedido", "Pedido", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cotação", "Cotacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Cotação", "DataCriacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Expedidor", "Expedidor", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Cotação", "ValorCotacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação Pedido", "SituacaoPedido", 10, Models.Grid.Align.left, false);

                Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> cotacoes = repCotacaoPedido.ConsultarParaCotacaoFrete(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repCotacaoPedido.ContarConsultaParaCotacaoFrete(filtrosPesquisa));

                var lista = (from p in cotacoes
                             select new
                             {
                                 p.Codigo,
                                 Cotacao = p.Codigo,
                                 DataCriacao = p.DataCriacao?.ToDateTimeString() ?? string.Empty,
                                 Pedido = p.Pedido?.NumeroPedidoEmbarcador ?? "",
                                 Expedidor = p.Pedido?.Remetente.Descricao ?? p.SolicitacaoCotacao.Remetente.Descricao,
                                 Destino = p.Destino.DescricaoCidadeEstado,
                                 Transportador = p.Empresa.Descricao,
                                 ValorCotacao = p.ValorCotacao.ToString("n2"),
                                 SituacaoPedido = p.Pedido?.SituacaoPedido.ObterDescricao() ?? "Em Cotação"
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> BuscarTransportadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao = ObterObjetoCotacao(unitOfWork);

                Servicos.Embarcador.Pedido.Cotacao serCotacao = new Servicos.Embarcador.Pedido.Cotacao(unitOfWork);
                List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao> cotacoes = serCotacao.ObterCotacoes(cotacao, _conexao.AdminStringConexao, TipoServicoMultisoftware, true);

                if (cotacoes.Count == 0)
                    return new JsonpResult(false, true, "Nenhum transportador foi encontrado!");

                var retorno = new
                {
                    ListaTransportador = cotacoes.Select(o => new
                    {
                        Transportador = new { CNPJ = o.Transportador.CNPJ, Descricao = o.Transportador.RazaoSocial + " (" + o.Transportador.Endereco.Cidade.Descricao + " - " + o.Transportador.Endereco.Cidade.SiglaUF + ")" },
                        ValorCotacao = o.ValorCotacao.ValorTotalCotacao.ToString("n2"),
                        ValorFrete = o.ValorCotacao.Valor.ToString("n2"),
                        o.PrazoEntrega,
                        o.DataPrazoEntrega,
                        o.DataPrevisaoColeta,
                        DistanciaRaioKM = o.DistanciaRaioKM.ToString("n0"),
                        RetornoCompleto = o,
                        o.CanalEntrega
                    }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (BaseException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os transportadores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarCotacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao = ObterObjetoCotacao(unitOfWork);
                Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao retornoCotacao = ObterObjetoRetornoCotacao();

                Servicos.Embarcador.Pedido.Cotacao serCotacao = new Servicos.Embarcador.Pedido.Cotacao(unitOfWork);
                serCotacao.GerarCotacao(cotacao, retornoCotacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a cotação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Cotacoes.FiltroPesquisaCotacaoFrete ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Cotacoes.FiltroPesquisaCotacaoFrete()
            {
                DataCotacaoInicial = Request.GetDateTimeParam("DataCotacaoInicial"),
                DataCotacaoFinal = Request.GetDateTimeParam("DataCotacaoFinal"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CpfCnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                NumeroCotacao = Request.GetIntParam("NumeroCotacao")
            };
        }

        private Dominio.ObjetosDeValor.WebService.Pedido.Cotacao ObterObjetoCotacao(Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpjExpedidor = Request.GetDoubleParam("Expedidor");
            double cpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");
            string cepDestino = Request.GetStringParam("CEPDestino");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            decimal valorNotaFiscal = Request.GetDecimalParam("ValorNotaFiscal");
            dynamic dynMercadorias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Mercadorias"));

            if (dynMercadorias?.Count == 0)
                throw new ControllerException("Nenhuma mercadoria foi selecionada!");

            if (cpfCnpjDestinatario == 0 && cepDestino == string.Empty)
                throw new ControllerException("É necessário que o Destinatário ou o CEP de Destino esteja preenchido.");

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);

            Dominio.Entidades.Cliente expedidor = repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor);
            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

            Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao = new Dominio.ObjetosDeValor.WebService.Pedido.Cotacao();
            cotacao.Expedidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa(); //serPessoa.ConverterObjetoPessoa(expedidor);
            cotacao.Expedidor.CPFCNPJ = expedidor.CPF_CNPJ_SemFormato;
            cotacao.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            cotacao.Destinatario.CPFCNPJ = destinatario?.CPF_CNPJ_SemFormato ?? string.Empty;
            cotacao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacao { CodigoEmbarcador = tipoOperacao.CodigoIntegracao };
            cotacao.EnderecoDestino = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco { CEP = cepDestino };
            cotacao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
            cotacao.ValorTotalMercadoria = valorNotaFiscal;

            foreach (dynamic mercadoria in dynMercadorias)
            {
                int codigo = ((string)mercadoria.Produto.Codigo).ToInt();
                decimal quantidade = ((string)mercadoria.Quantidade).ToDecimal();
                decimal peso = ((string)mercadoria.Peso).ToDecimal();
                decimal largura = ((string)mercadoria.Largura).ToDecimal();
                decimal comprimento = ((string)mercadoria.Comprimento).ToDecimal();
                decimal altura = ((string)mercadoria.Altura).ToDecimal();
                bool produtoSimulado = ((string)mercadoria.TipoProduto).ToBool();
                int codigoGrupoProduto = ((string)mercadoria.GrupoProduto.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigo);

                if (produtoEmbarcador != null)
                {
                    largura = produtoEmbarcador.LarguraCM;
                    comprimento = produtoEmbarcador.ComprimentoCM;
                    peso = produtoEmbarcador.PesoUnitario;
                    altura = produtoEmbarcador.AlturaCM;
                }

                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto
                {
                    CodigoProduto = produtoEmbarcador?.CodigoProdutoEmbarcador ?? string.Empty,
                    DescricaoProduto = produtoEmbarcador?.Descricao ?? string.Empty,
                    CodigoGrupoProduto = codigoGrupoProduto > 0 ? codigoGrupoProduto.ToString() : produtoEmbarcador?.GrupoProduto?.Codigo.ToString() ?? string.Empty,
                    DescricaoGrupoProduto = produtoEmbarcador?.GrupoProduto?.Descricao ?? string.Empty,
                    Quantidade = quantidade,
                    Largura = largura,
                    Comprimento = comprimento,
                    Altura = altura,
                    PesoUnitario = peso,
                    ProdutoSimulado = produtoSimulado,
                };

                cotacao.Produtos.Add(produto);
            }

            return cotacao;
        }

        private Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao ObterObjetoRetornoCotacao()
        {
            dynamic dynTransportadorEscolhido = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TransportadorEscolhido"));

            Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao retornoCotacao = new Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao
            {
                EnderecoDestino = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
                {
                    Bairro = dynTransportadorEscolhido.RetornoCompleto.EnderecoDestino.Bairro,
                    CEP = dynTransportadorEscolhido.RetornoCompleto.EnderecoDestino.CEP,
                    Logradouro = dynTransportadorEscolhido.RetornoCompleto.EnderecoDestino.Logradouro,
                    Cidade = new Dominio.ObjetosDeValor.Localidade
                    {
                        IBGE = dynTransportadorEscolhido.RetornoCompleto.EnderecoDestino.Cidade.IBGE
                    }
                },
                Transportador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa
                {
                    CNPJ = dynTransportadorEscolhido.Codigo
                },
                DataPrazoEntrega = dynTransportadorEscolhido.DataPrazoEntrega,
                PrazoEntrega = ((string)dynTransportadorEscolhido.PrazoEntrega).ToInt(),
                ValorCotacao = new Dominio.ObjetosDeValor.WebService.Pedido.ValorCotacao
                {
                    FreteProprio = dynTransportadorEscolhido.RetornoCompleto.ValorCotacao.FreteProprio,
                    ValorTotalCotacao = dynTransportadorEscolhido.RetornoCompleto.ValorCotacao.ValorTotalCotacao,
                    Valor = dynTransportadorEscolhido.RetornoCompleto.ValorCotacao.Valor,
                },
                DistanciaRaioKM = ((string)dynTransportadorEscolhido.RetornoCompleto.DistanciaRaioKM).ToDecimal()
            };

            retornoCotacao.ValorCotacao.Componentes = new List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente>();
            foreach (var componente in dynTransportadorEscolhido.RetornoCompleto.ValorCotacao.Componentes)
            {
                Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente retornoCalculoFreteValoresComponente = new Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente();
                retornoCalculoFreteValoresComponente.Descricao = componente.Descricao;
                retornoCalculoFreteValoresComponente.Valor = ((string)componente.Valor).ToDecimal();
                retornoCotacao.ValorCotacao.Componentes.Add(retornoCalculoFreteValoresComponente);
            }

            return retornoCotacao;
        }

        #endregion
    }
}
