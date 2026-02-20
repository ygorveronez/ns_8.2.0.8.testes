using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.GestaoDevolucao
{
    public sealed class GestaoDevolucao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly CancellationToken _cancellationToken;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.Embarcador.Devolucao.GestaoDevolucaoImportacao _repositorioGestaoDevolucaoImportacao;
        private readonly Repositorio.Embarcador.Devolucao.GestaoDevolucao _repositorioGestaoDevolucao;
        private readonly Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa _repositorioGestaoDevolucaoEtapa;
        private readonly Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo _repositorioGestaoDevolucaoLaudo;
        private readonly Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail _repositorioConfiguracaoModeloEmail;
        private readonly Repositorio.Usuario _repositorioUsuario;
        private readonly Repositorio.Embarcador.Chamados.Chamado _repositorioChamado;
        private readonly Repositorio.Embarcador.Devolucao.GestaoDevolucaoProduto _repositorioGestaoDevolucaoProduto;
        private readonly Repositorio.Embarcador.Produtos.ProdutoEmbarcador _repositorioProdutoEmbarcador;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private readonly Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes _repositorioConfiguracaoPaletes;
        private readonly Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO _repositorioGestaoDevolucaoNFDxNFO;
        private readonly Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares _repositorioGestaoDevolucaoDadosComplementares;
        #endregion Atributos

        #region Construtores

        public GestaoDevolucao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;

            _repositorioGestaoDevolucaoImportacao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoImportacao(_unitOfWork);
            _repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
            _repositorioGestaoDevolucaoEtapa = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa(_unitOfWork);
            _repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(_unitOfWork);
            _repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(_unitOfWork);
            _repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            _repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            _repositorioGestaoDevolucaoProduto = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoProduto(_unitOfWork);
            _repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            _repositorioConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(_unitOfWork);
            _repositorioGestaoDevolucaoNFDxNFO = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO(_unitOfWork);
            _repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(_unitOfWork);

            _auditado = auditado;
            _clienteMultisoftware = clienteMultisoftware;
            _cancellationToken = cancellationToken;
        }

        public GestaoDevolucao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : this(unitOfWork, auditado, clienteMultisoftware, cancellationToken: default)
        {
        }

        #endregion Construtores

        #region Métodos Privados
        private async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> CriarGestaoDevolucao(Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ParametrosGestaoDevolucao parametrosGestaoDevolucao)
        {
            if (parametrosGestaoDevolucao.NotasFiscaisDeOrigem == null || parametrosGestaoDevolucao.NotasFiscaisDeOrigem.Count == 0) return null;

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao();
            gestaoDevolucao.Initialize();

            gestaoDevolucao.OrigemRecebimento = parametrosGestaoDevolucao.OrigemRecebimento;
            gestaoDevolucao.Geracao = parametrosGestaoDevolucao.Geracao;
            gestaoDevolucao.Tipo = TipoGestaoDevolucao.NaoDefinido;

            gestaoDevolucao.CargaOrigem = parametrosGestaoDevolucao.Carga;
            gestaoDevolucao.Transportador = parametrosGestaoDevolucao.Transportador;
            gestaoDevolucao.Filial = parametrosGestaoDevolucao.Filial;

            gestaoDevolucao.PosEntrega = parametrosGestaoDevolucao.PosEntrega;
            gestaoDevolucao.Aprovada = false;
            gestaoDevolucao.Procedente = true;
            gestaoDevolucao.TipoFluxoGestaoDevolucao = parametrosGestaoDevolucao.TipoFluxoGestaoDevolucao;

            gestaoDevolucao.TipoNotas = ObterTipoNotasGestaoDevolucao(parametrosGestaoDevolucao.NotasFiscaisDeOrigem.ToList());

            await _repositorioGestaoDevolucao.InserirAsync(gestaoDevolucao);

            if (parametrosGestaoDevolucao.Produtos != null)
            {
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador = _repositorioProdutoEmbarcador.BuscarPorCodigoCEAN(parametrosGestaoDevolucao.Produtos.Select(p => p.CodigoCEAN).ToList());
                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> gestaoDevolucaoProdutos = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>();

                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produto in parametrosGestaoDevolucao.Produtos)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = produtosEmbarcador.Find(p => p.CodigoCEAN == produto.CodigoCEAN);

                    gestaoDevolucaoProdutos.Add(new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto()
                    {
                        GestaoDevolucao = gestaoDevolucao,
                        NotaFiscalDevolucao = gestaoDevolucao.NotaFiscalDevolucao,
                        Produto = produtoEmbarcador,
                        PesoUnitario = produtoEmbarcador?.PesoUnitario ?? 0,
                        Quantidade = produto.QuantidadeComercial,
                        ValorUnitario = produto.ValorUnitarioComercial,
                        ProdutoDescricao = $"{produto.CodigoCEAN} - {produto.Descricao}",
                    });
                }
                await _repositorioGestaoDevolucaoProduto.InserirAsync(gestaoDevolucaoProdutos);
            }

            Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem repositorioGestaoDevolucaoNotaFiscalOrigem = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao repositorioGestaoDevolucaoNotaFiscalDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao(_unitOfWork);

            if (parametrosGestaoDevolucao.NotasFiscaisDeOrigem != null)
            {
                if (gestaoDevolucao.NotasFiscaisDeOrigem == null)
                    gestaoDevolucao.NotasFiscaisDeOrigem = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaOrigem in parametrosGestaoDevolucao.NotasFiscaisDeOrigem)
                {
                    Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem gestaoDevolucaoNotaFiscalOrigem = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem()
                    {
                        XMLNotaFiscal = notaOrigem,
                        GestaoDevolucao = gestaoDevolucao
                    };

                    await repositorioGestaoDevolucaoNotaFiscalOrigem.InserirAsync(gestaoDevolucaoNotaFiscalOrigem);

                }
            }

            if (parametrosGestaoDevolucao.NotasFiscaisDeDevolucao != null)
            {
                if (gestaoDevolucao.NotasFiscaisDevolucao == null)
                    gestaoDevolucao.NotasFiscaisDevolucao = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaDevolucao in parametrosGestaoDevolucao.NotasFiscaisDeDevolucao)
                {
                    Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao gestaoDevolucaoNotaFiscalDevolucao = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao()
                    {
                        XMLNotaFiscal = notaDevolucao,
                        GestaoDevolucao = gestaoDevolucao
                    };

                    await repositorioGestaoDevolucaoNotaFiscalDevolucao.InserirAsync(gestaoDevolucaoNotaFiscalDevolucao);
                }
            }

            await _repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

            Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), $"Inclusão via {gestaoDevolucao.OrigemRecebimento.ObterDescricao()}", _unitOfWork);
            return gestaoDevolucao;
        }

        private string TrocarTagsPorValor(string texto, List<TagValorGestaoDevolucao> listaTagValor)
        {
            foreach (TagValorGestaoDevolucao tagValor in listaTagValor.Where(t => texto.Contains(t.Tag)))
                texto = texto.Replace(tagValor.Tag, tagValor.Valor);

            return texto;
        }

        private string BuscarCorpoEmail(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail)
        {
            #region Definição das Tags
            List<TagValorGestaoDevolucao> listaTagValor = new List<TagValorGestaoDevolucao>();
            listaTagValor.Add(new TagValorGestaoDevolucao() { Tag = "#TagNFO", Valor = "" });
            listaTagValor.Add(new TagValorGestaoDevolucao() { Tag = "#TagNFD", Valor = "" });
            listaTagValor.Add(new TagValorGestaoDevolucao() { Tag = "#TagNumeroDevolucao", Valor = "" });
            listaTagValor.Add(new TagValorGestaoDevolucao() { Tag = "#TagRazaoSocialCNPJTransportador", Valor = "" });
            #endregion

            ObterDadosTags(gestaoDevolucao, listaTagValor);

            return TrocarTagsPorValor(modeloEmail.Corpo, listaTagValor);
        }

        private List<TagValorGestaoDevolucao> ObterDadosTags(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, List<TagValorGestaoDevolucao> listaTagValor)
        {
            string valor = string.Empty;
            foreach (TagValorGestaoDevolucao tagValor in listaTagValor)
            {
                switch (tagValor.Tag)
                {
                    case "#TagNFO":
                        valor = gestaoDevolucao.NotaFiscalOrigem?.Numero.ToString() ?? "-";
                        break;
                    case "#TagNFD":
                        valor = gestaoDevolucao.NotaFiscalDevolucao?.Numero.ToString() ?? "-";
                        break;
                    case "#TagNumeroDevolucao":
                        valor = gestaoDevolucao.Codigo.ToString();
                        break;
                    case "#TagRazaoSocialCNPJTransportador":
                        valor = gestaoDevolucao.Transportador?.NomeCNPJ?.ToString() ?? "-";
                        break;
                    default: break;
                }

                if (!string.IsNullOrEmpty(valor))
                    tagValor.Valor = valor;
            }

            return listaTagValor;
        }

        private async Task InformarGestaoDevolucaoAtualizada(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, int codigoClienteMultisoftware, Servicos.SignalR.Hubs.GestaoDevolucaoHubs hub, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAtualizacaoGestaoDevolucao tipoAtualizacaoGestaoDevolucao)
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao> retornoItens = await MontarItensGrid(new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>() { gestaoDevolucao });
                Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao gestaoDevolucaoGrid = retornoItens[0];

                gestaoDevolucaoGrid.MovimentarEtapa = tipoAtualizacaoGestaoDevolucao;

                Servicos.SignalR.Hubs.GestaoDevolucao hubGestaoDevolucao = new SignalR.Hubs.GestaoDevolucao();
                hubGestaoDevolucao.InformarGestaoDevolucaoAtualizada(gestaoDevolucaoGrid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "GestaoDevolucao");
            }
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao>> MontarItensGrid(List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> gestaoDevolucoes)
        {
            Repositorio.Embarcador.Pessoas.ClienteComplementar repositorioClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao repositorioGestaoDevolucaoNotaFiscalDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem repositorioGestaoDevolucaoNotaFiscalOrigem = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem(_unitOfWork);

            List<long> codigoDevolucao = gestaoDevolucoes.Select(d => d.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa> etapasDevolucoes = await _repositorioGestaoDevolucaoEtapa.BuscarEtapaPorCodigoGestaoAsync(codigoDevolucao);
            IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> chamados = await _repositorioChamado.BuscarChamadosPorGestaodevolucaoAsync(codigoDevolucao);
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> clientesComplementares = await repositorioClienteComplementar.BuscarPorGestoesDevolucaoAsync(gestaoDevolucoes);
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> produtosDevolucoes = await _repositorioGestaoDevolucaoProduto.BuscarPorGestoesDevolucoesAsync(codigoDevolucao);

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares> dadosComplementaresDevolucoes = await repositorioGestaoDevolucaoDadosComplementares.BuscarPorGestoesDevolucoesAsync(codigoDevolucao);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(_unitOfWork).BuscarConfiguracaoPadraoAsync();
            List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao> gestaoDevolucaoGrid = new List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao>();

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> notasDevolucao = await repositorioGestaoDevolucaoNotaFiscalDevolucao.BuscarPorCodigoGestaoDevolucaoAsync(codigoDevolucao);
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> notasOrigem = await repositorioGestaoDevolucaoNotaFiscalOrigem.BuscarPorCodigoGestaoDevolucaoAsync(codigoDevolucao);

            foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao devolucao in gestaoDevolucoes)
            {
                var itens = Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao.ObterItemGrid(devolucao, notasOrigem, notasDevolucao, etapasDevolucoes, chamados, clientesComplementares, produtosDevolucoes, dadosComplementaresDevolucoes, configuracaoPaletes);
                gestaoDevolucaoGrid.Add(itens);
            }

            return gestaoDevolucaoGrid;
        }

        private List<DadosNotasFiscaisChamado> ObterDadosNotasFiscaisChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos)
        {
            List<DadosNotasFiscaisChamado> notasFiscaisRetornar = new List<DadosNotasFiscaisChamado>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
            {
                if (cargaEntregaNotaFiscal.Chamado?.Codigo == chamado.Codigo || chamado.XMLNotasFiscais.Contains(cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal))
                {
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> notaFiscalProdutosRetornar = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaNotaFiscalProdutos = cargaEntregaProdutos.Where(produto => produto.QuantidadeDevolucao > 0 && produto.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo).ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaNotaFiscalProduto in cargaEntregaNotaFiscalProdutos)
                    {
                        notaFiscalProdutosRetornar.Add(
                            new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos
                            {
                                CodigoCEAN = cargaEntregaNotaFiscalProduto.Produto.CodigoCEAN,
                                QuantidadeComercial = cargaEntregaNotaFiscalProduto.QuantidadeDevolucao,
                                ValorUnitarioComercial = cargaEntregaNotaFiscalProduto.ValorDevolucao,
                            });
                    }

                    notasFiscaisRetornar.Add(new DadosNotasFiscaisChamado
                    {
                        NotaFiscal = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal,
                        Produtos = notaFiscalProdutosRetornar,
                    });
                }
            }

            return notasFiscaisRetornar;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarPedidoECarga(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.Entidades.Embarcador.Filiais.Filial filial, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = PreencherObjetoCargaIntegracao(gestaoDevolucao, filial, _configuracaoEmbarcador);

            Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido servicoProdutoPedido = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(_unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            StringBuilder mensagemErro = new StringBuilder();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, gestaoDevolucao.DadosComplementares.TipoOperacao, ref mensagemErro, _tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, buscarCargaPorTransportador: false, ignorarPedidosInseridosManualmente: true, configuracaoTMS: _configuracaoEmbarcador);

            if (mensagemErro.Length > 0)
                throw new ServicoException(mensagemErro.ToString());

            servicoProdutoPedido.AdicionarProdutosPedido(pedido, _configuracaoEmbarcador, cargaIntegracao, ref mensagemErro, _unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref codigoCargaExistente, _unitOfWork, _tipoServicoMultisoftware, false, false, null, _configuracaoEmbarcador, null, "", filial, gestaoDevolucao.DadosComplementares.TipoOperacao);

            if (mensagemErro.Length > 0)
                throw new ServicoException(mensagemErro.ToString());

            if (cargaPedido != null)
            {
                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_unitOfWork);
                bool alteradoTipoCarga = false;
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDevolucao = ObterNotasDaDevolucao(gestaoDevolucao);
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in notasDevolucao)
                {
                    serCargaNotaFiscal.InserirNotaCargaPedido(nota, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, _configuracaoEmbarcador, false, out alteradoTipoCarga, _auditado);
                }

                servicoRateioFrete.GerarComponenteICMS(cargaPedido, false, _unitOfWork);

                if (cargaPedido.CargaPedidoFilialEmissora)
                    servicoRateioFrete.GerarComponenteICMS(cargaPedido, true, _unitOfWork);

                servicoRateioFrete.GerarComponenteISS(cargaPedido, false, _unitOfWork);
                servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, false);

                carga = cargaPedido.Carga;
                servicoCarga.FecharCarga(carga, _unitOfWork, _tipoServicoMultisoftware, ClienteMultisoftware, recriarRotas: false, adicionarJanelaDescarregamento: false, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: false);

                carga.CargaFechada = true;
                carga.NumeroSequenciaCarga = cargaIntegracao.NumeroCarga.ToInt();

                repositorioCarga.Atualizar(carga);
                repositorioPedido.Atualizar(pedido);
            }

            return carga;
        }

        private Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao PreencherObjetoCargaIntegracao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(_unitOfWork.StringConexao);

            string numeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork).ToString();
            string codigoFilialEmbarcador = filial?.CodigoFilialEmbarcador ?? string.Empty;
            int numeroPedidoEmbarcador = _configuracaoEmbarcador.UtilizarNumeroPreCargaPorFilial ? repositorioPedido.ObterProximoCodigo(filial) : repositorioPedido.ObterProximoCodigo();

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao
            {
                Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = codigoFilialEmbarcador },
                NumeroCarga = numeroCarga,
                NumeroPedidoEmbarcador = numeroPedidoEmbarcador.ToString(),
                DataPrevisaoChegadaDestinatario = gestaoDevolucao.DadosComplementares.DataDescarregamento?.ToString("dd/MM/yyyy HH:mm:ss"),
                ModeloVeicular = gestaoDevolucao.DadosComplementares.Veiculo?.ModeloVeicularCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = gestaoDevolucao.DadosComplementares.Veiculo.ModeloVeicularCarga.CodigoIntegracao } : null,
                Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = gestaoDevolucao.DadosComplementares.DestinatarioAgendamento.CPF_CNPJ_SemFormato },
                Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = gestaoDevolucao.DadosComplementares.RemetenteAgendamento?.CPF_CNPJ_SemFormato },
                TipoCargaEmbarcador = gestaoDevolucao.DadosComplementares.TipoDeCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = gestaoDevolucao.DadosComplementares.TipoDeCarga.CodigoTipoCargaEmbarcador } : gestaoDevolucao.DadosComplementares.TipoOperacao?.TipoDeCargaPadraoOperacao != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = gestaoDevolucao.DadosComplementares.TipoOperacao.TipoDeCargaPadraoOperacao.CodigoTipoCargaEmbarcador } : null,
                ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto() { DescricaoProduto = "Diversos", CodigoProduto = "Diversos" },
                DataColeta = gestaoDevolucao.DadosComplementares.DataCarregamento.HasValue ? $"{gestaoDevolucao.DadosComplementares.DataCarregamento:dd/MM/yyyy} 00:00:00" : string.Empty,
                DataPrevisaoEntrega = gestaoDevolucao.DadosComplementares.DataDescarregamento?.ToDateTimeString(true) ?? string.Empty,
                TransportadoraEmitente = gestaoDevolucao.DadosComplementares.Transportador != null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = gestaoDevolucao.DadosComplementares.Transportador.CNPJ } : null,
                DataInicioCarregamento = gestaoDevolucao.DadosComplementares.DataCarregamento?.ToDateTimeString(true),
                TipoOperacao = serWSTipoOperacao.ConverterObjetoTipoOperacao(gestaoDevolucao.DadosComplementares.TipoOperacao),
                Veiculo = gestaoDevolucao.DadosComplementares.Veiculo != null ? (new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo() { Placa = gestaoDevolucao.DadosComplementares.Veiculo.Placa }) : null,
            };

            if (gestaoDevolucao.DadosComplementares.Motorista != null)
                cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>() { new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista() { CPF = gestaoDevolucao.DadosComplementares.Motorista.CPF } };

            cargaIntegracao.Recebedor = null;

            return cargaIntegracao;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterNotasDaDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO repositorioGestaoDevolucaoNFDxNFO = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> listaNotasOrigem = repositorioGestaoDevolucaoNFDxNFO.BuscarPorNFD(gestaoDevolucao.NotasFiscaisDevolucao.Select(nota => nota.XMLNotaFiscal.Codigo).ToList());

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNFD = gestaoDevolucao.NotasFiscaisDevolucao.Select(nota => nota.XMLNotaFiscal).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNFO = gestaoDevolucao.NotasFiscaisDeOrigem.Select(nota => nota.XMLNotaFiscal).Where(nota => !listaNotasOrigem.Select(n => n.NFO.Codigo).Contains(nota.Codigo)).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDevolucao = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            notasDevolucao.AddRange(listaNFD);
            notasDevolucao.AddRange(listaNFO);
            return notasDevolucao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotasGestaoDevolucao ObterTipoNotasGestaoDevolucao(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDeOrigem)
        {
            if (notasFiscaisDeOrigem.TrueForAll(nota => nota.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet))
                return TipoNotasGestaoDevolucao.Pallet;
            else if (notasFiscaisDeOrigem.TrueForAll(nota => !(nota.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet)))
                return TipoNotasGestaoDevolucao.Mercadoria;

            return TipoNotasGestaoDevolucao.Mista;
        }
        #endregion

        #region Métodos Públicos

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> GerarGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao gestaoDevolucaoImportacao, GeracaoGestaoDevolucao geracaoGestaoDevolucao, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDeOrigem)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalDevolucao = null;
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNotaFiscalDevolucao = null;

            if (gestaoDevolucaoImportacao.XML_NFD == null) throw new ServicoException("XML da NFD é obrigatório");

            try
            {
                (xmlNotaFiscalDevolucao, dadosNotaFiscalDevolucao) = ObterDadosNotaFiscalDevolucao(gestaoDevolucaoImportacao.XML_NFD);
                gestaoDevolucaoImportacao.NotaFiscalDevolucao = xmlNotaFiscalDevolucao;
            }
            catch (Exception ex)
            {
                throw new ServicoException("Falha ao processar XML: " + ex.Message);
            }

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repositorioCanhoto.BuscarPorChaves(notasFiscaisDeOrigem?.Select(nota => nota?.Chave ?? string.Empty).ToList() ?? new List<string>() { string.Empty });

            Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ParametrosGestaoDevolucao parametrosGestaoDevolucao = new Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ParametrosGestaoDevolucao()
            {
                Geracao = geracaoGestaoDevolucao,
                OrigemRecebimento = gestaoDevolucaoImportacao.OrigemRecebimento,
                NotaFiscalOrigem = gestaoDevolucaoImportacao.NotaFiscalOrigem ?? notasFiscaisDeOrigem?[0] ?? null,
                NotasFiscaisDeOrigem = notasFiscaisDeOrigem ?? new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>() { },
                NotasFiscaisDeDevolucao = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>() { gestaoDevolucaoImportacao.NotaFiscalDevolucao },
                Produtos = dadosNotaFiscalDevolucao?.Produtos,
                Carga = gestaoDevolucaoImportacao.Carga,
                Transportador = gestaoDevolucaoImportacao.Carga?.Empresa,
                Filial = gestaoDevolucaoImportacao.Carga?.Filial,
                PosEntrega = (canhotos?.Count == 0) || (canhotos?.Exists(canhoto => canhoto.DataDigitalizacao.HasValue && gestaoDevolucaoImportacao.DataDocumento.HasValue && canhoto.DataDigitalizacao.Value.Date != gestaoDevolucaoImportacao.DataDocumento.Value.Date) ?? false),
            };

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await CriarGestaoDevolucao(parametrosGestaoDevolucao);

            if (gestaoDevolucao != null)
            {
                gestaoDevolucaoImportacao.EmDevolucao = true;
                gestaoDevolucaoImportacao.GestaoDevolucao = gestaoDevolucao;
                _repositorioGestaoDevolucaoImportacao.Atualizar(gestaoDevolucaoImportacao);

                if (gestaoDevolucao.PosEntrega)
                {
                    DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.PosEntrega);
                    DefinirEtapaAtualGestaoDevolucao(gestaoDevolucao, EtapaGestaoDevolucao.GestaoDeDevolucao);
                    AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                    GerarIntegracoes(gestaoDevolucao, TipoIntegracaoGestaoDevolucao.SalesforcePosEntrega, new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Salesforce });
                }
                else
                {
                    DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.NaoDefinido);
                    DefinirEtapaAtualGestaoDevolucao(gestaoDevolucao, EtapaGestaoDevolucao.DefinicaoTipoDevolucao);
                }
            }
            //else
            //{
            //    GerarIntegracoes(gestaoDevolucao, TipoIntegracaoGestaoDevolucao.SalesforceNFeNaoCompativel, new List<TipoIntegracao>() { TipoIntegracao.Salesforce });
            //}

            return gestaoDevolucao;
        }

        public (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe DadosNFe) ObterDadosNotaFiscalDevolucao(string baseXML)
        {
            string XML = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(baseXML));

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);


            Servicos.NFe serNFe = new Servicos.NFe(_unitOfWork, _auditado);
            Servicos.Embarcador.NFe.NFe servicoNfe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalDevolucao = null;
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNotaFiscalDevolucao = null;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            byte[] byteArray = Encoding.UTF8.GetBytes(XML);
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using StreamReader stReaderXML = new StreamReader(memoryStream);

            string error = string.Empty;
            stReaderXML.BaseStream.Position = 0;
            dadosNotaFiscalDevolucao = serNFe.ObterDocumentoPorXML(stReaderXML.BaseStream, _unitOfWork, false, false);
            if (servicoNfe.BuscarDadosNotaFiscal(out error, out xmlNotaFiscalDevolucao, stReaderXML, _unitOfWork, dadosNotaFiscalDevolucao, true, false, true, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, null, null, _auditado, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
            {
                if (xmlNotaFiscalDevolucao.Codigo > 0)
                    repositorioNotaFiscal.Atualizar(xmlNotaFiscalDevolucao);
                else
                    repositorioNotaFiscal.Inserir(xmlNotaFiscalDevolucao);
            }
            else
                Servicos.Log.TratarErro(error, "GestaoDevolucao");

            return (xmlNotaFiscalDevolucao, dadosNotaFiscalDevolucao);
        }

        public void GerarIntegracoes(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGestaoDevolucao tipoIntegracaoGestaoDevolucao, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> enumTiposIntegracoes)
        {
            if (enumTiposIntegracoes == null)
            {
                enumTiposIntegracoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Salesforce,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YPE
                };
            }

            Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repositorioGestaoDevolucaoIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracoes = repositorioTipoIntegracao.BuscarPorTiposAsync(enumTiposIntegracoes).GetAwaiter().GetResult();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracoes)
            {
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao gestaoDevolucaoIntegracao = repositorioGestaoDevolucaoIntegracao.BuscarPorGestaoDevolucaoETipo(gestaoDevolucao.Codigo, tipoIntegracao.Codigo, tipoIntegracaoGestaoDevolucao) ??
                    new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao();

                gestaoDevolucaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                gestaoDevolucaoIntegracao.ProblemaIntegracao = SituacaoIntegracao.AgIntegracao.ObterDescricao();
                gestaoDevolucaoIntegracao.NumeroTentativas = 0;
                gestaoDevolucaoIntegracao.DataIntegracao = DateTime.Now;

                gestaoDevolucaoIntegracao.GestaoDevolucao = gestaoDevolucao;
                gestaoDevolucaoIntegracao.TipoIntegracao = tipoIntegracao;
                gestaoDevolucaoIntegracao.TipoIntegracaoGestaoDevolucao = tipoIntegracaoGestaoDevolucao;


                if (gestaoDevolucaoIntegracao.Codigo > 0)
                    repositorioGestaoDevolucaoIntegracao.Atualizar(gestaoDevolucaoIntegracao);
                else
                    repositorioGestaoDevolucaoIntegracao.Inserir(gestaoDevolucaoIntegracao);
            }
        }

        public void ProcessarIntegracoesPendentes()
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao repositorioGestaoDevolucaoIntegracao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao> integracoesPendentes = repositorioGestaoDevolucaoIntegracao.BuscarIntegracoesPendentes();

            foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoIntegracao gestaoDevolucaoIntegracao in integracoesPendentes)
            {
                try
                {
                    switch (gestaoDevolucaoIntegracao.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Salesforce:
                            new Servicos.Embarcador.Integracao.Salesforce.IntegracaoSalesforce(_unitOfWork).IntegrarDevolucaoNotificacaoCRM(gestaoDevolucaoIntegracao);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YPE:
                            if (gestaoDevolucaoIntegracao.TipoIntegracaoGestaoDevolucao == TipoIntegracaoGestaoDevolucao.SAPLaudo)
                                new Servicos.Embarcador.Integracao.YPE.IntegracaoYPE(_unitOfWork).IntegrarGestaoDevolucaoSAPLaudo(gestaoDevolucaoIntegracao);
                            break;
                        default:
                            gestaoDevolucaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            gestaoDevolucaoIntegracao.ProblemaIntegracao = "Integração não implementada.";
                            repositorioGestaoDevolucaoIntegracao.Atualizar(gestaoDevolucaoIntegracao);
                            break;
                    }

                    if (gestaoDevolucaoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado &&
                        !repositorioGestaoDevolucaoIntegracao.ExistePendenteParaGestaoDevolucaoETipo(gestaoDevolucaoIntegracao.GestaoDevolucao.Codigo, gestaoDevolucaoIntegracao.TipoIntegracao.Codigo, gestaoDevolucaoIntegracao.TipoIntegracaoGestaoDevolucao) &&
                        gestaoDevolucaoIntegracao.GestaoDevolucao.Tipo != TipoGestaoDevolucao.PosEntrega)
                    {
                        AvancarEtapaGestaoDevolucao(gestaoDevolucaoIntegracao.GestaoDevolucao, "Avanço automático por finalização de integrações pendentes.");
                    }
                    else if (gestaoDevolucaoIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    {
                        RejeitarEtapaGestaoDevolucao(gestaoDevolucaoIntegracao.GestaoDevolucao.EtapaAtual, "Etapa rejeitada por falha na integração.");
                        InformarGestaoDevolucaoAtualizada(gestaoDevolucaoIntegracao.GestaoDevolucao, _clienteMultisoftware?.Codigo ?? 199, SignalR.Hubs.GestaoDevolucaoHubs.InformarGestaoDevolucaoAtualizada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAtualizacaoGestaoDevolucao.AtualizarMesmaEtapa);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "GestaoDevolucaoIntegracao");
                }
            }
        }

        public void GerarLaudo(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoProduto repositorioGestaoDevolucaoProduto = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo laudo = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo()
            {
                Produtos = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto>()
            };

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> gestaoDevolucaoProdutos = repositorioGestaoDevolucaoProduto.BuscarPorGestaoDevolucao(gestaoDevolucao.Codigo);
            IEnumerable<IGrouping<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>> produtosAgrupados = gestaoDevolucaoProdutos.GroupBy(p => p.Produto);
            foreach (IGrouping<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> produto in produtosAgrupados)
            {
                laudo.Produtos.Add(new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto()
                {
                    Laudo = laudo,
                    Produto = produto.Key,
                    QuantidadeOrigem = produto.Sum(p => p.Quantidade),
                    ProdutoDescricao = produto.FirstOrDefault()?.ProdutoDescricao ?? "-",
                });
            }

            _repositorioGestaoDevolucaoLaudo.Inserir(laudo);

            gestaoDevolucao.Laudo = laudo;
            _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
        }

        public void DefinirTipoGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoDevolucao tipoGestaoDevolucao)
        {
            gestaoDevolucao.Tipo = tipoGestaoDevolucao;

            if (gestaoDevolucao.Etapas?.Count == 0 || gestaoDevolucao.Etapas == null)
                gestaoDevolucao.Etapas = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa>();

            int etapasExistentes = gestaoDevolucao.Etapas.Count;

            foreach (var etapa in gestaoDevolucao.Tipo.ObterEtapasDevolucao(gestaoDevolucao.TipoNotas).Select((etapa, ordem) => new { etapa, ordem }))
            {
                gestaoDevolucao.Etapas.Add(new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa()
                {
                    GestaoDevolucao = gestaoDevolucao,
                    Etapa = etapa.etapa,
                    SituacaoEtapa = (etapa.etapa == EtapaGestaoDevolucao.GestaoDeDevolucao) ? SituacaoEtapaGestaoDevolucao.Finalizada : SituacaoEtapaGestaoDevolucao.NaoIniciada,
                    Ordem = etapa.ordem + etapasExistentes
                });
            }

            _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);

            if (tipoGestaoDevolucao != TipoGestaoDevolucao.NaoDefinido)
                Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), $"Definição do tipo: {tipoGestaoDevolucao.ObterDescricao()}", _unitOfWork);
        }

        public void DefinirEtapaAtualGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao etapaGestaoDevolucao)
        {
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa gestaoDevolucaoEtapa = _repositorioGestaoDevolucaoEtapa.BuscarEtapaPorEtapaECodigoGestao(gestaoDevolucao.Codigo, etapaGestaoDevolucao);

            if (gestaoDevolucaoEtapa != null)
            {
                gestaoDevolucao.EtapaAtual = gestaoDevolucaoEtapa;
                gestaoDevolucaoEtapa.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.EmAndamento;
                if (!gestaoDevolucaoEtapa.DataInicio.HasValue)
                    gestaoDevolucaoEtapa.DataInicio = DateTime.Now;
                _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
            }
        }

        private void AvancarEtapaGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, string observacao, bool trocaTipo)
        {
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa etapaAtual = _repositorioGestaoDevolucaoEtapa.BuscarPorCodigo(gestaoDevolucao.EtapaAtual.Codigo, false);

            if (trocaTipo)
            {
                gestaoDevolucao.EtapaAtual = null;
            }
            else
            {
                etapaAtual.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.Finalizada;
                etapaAtual.DataFim = DateTime.Now;
                etapaAtual.Observacao = observacao;
                _repositorioGestaoDevolucaoEtapa.Atualizar(etapaAtual);
            }

            _unitOfWork.Flush();

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa gestaoDevolucaoProximaEtapa = _repositorioGestaoDevolucaoEtapa.BuscarEtapaPorCodigoGestaoEOrdem(gestaoDevolucao.Codigo, trocaTipo ? 1 : gestaoDevolucao.EtapaAtual.Ordem + 1);
            if (gestaoDevolucaoProximaEtapa != null)
            {
                gestaoDevolucaoProximaEtapa.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.EmAndamento;
                gestaoDevolucaoProximaEtapa.DataInicio = DateTime.Now;
                _repositorioGestaoDevolucaoEtapa.Atualizar(gestaoDevolucaoProximaEtapa);

                _unitOfWork.Flush();

                //#24450 - Gambiarra para testar se buscando novamente a entidade, ela atualiza corretamente o atributo EtapaAtual.
                gestaoDevolucao = _repositorioGestaoDevolucao.BuscarPorCodigo(gestaoDevolucao.Codigo);
                gestaoDevolucao.EtapaAtual = gestaoDevolucaoProximaEtapa;
                _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
            }

            _unitOfWork.Flush();

            Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), $"Avançou Etapa: {etapaAtual.Etapa.ObterDescricao()}", _unitOfWork);

            InformarGestaoDevolucaoAtualizada(gestaoDevolucao, _clienteMultisoftware?.Codigo ?? 199, SignalR.Hubs.GestaoDevolucaoHubs.InformarGestaoDevolucaoAtualizada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAtualizacaoGestaoDevolucao.AvancarEtapa).GetAwaiter().GetResult();
        }

        public void AvancarEtapaGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, bool trocaTipo)
        {
            AvancarEtapaGestaoDevolucao(gestaoDevolucao, "Finalizado ao avançar etapa", trocaTipo);
        }

        public void AvancarEtapaGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            AvancarEtapaGestaoDevolucao(gestaoDevolucao, false);
        }

        public void AvancarEtapaGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, string observacao)
        {
            AvancarEtapaGestaoDevolucao(gestaoDevolucao, observacao, false);
        }

        public void VoltarEtapaGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, string observacao = "Aberto ao voltar etapa")
        {
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa etapaAtual = _repositorioGestaoDevolucaoEtapa.BuscarPorCodigo(gestaoDevolucao.EtapaAtual.Codigo, false);
            RejeitarEtapaGestaoDevolucao(etapaAtual, observacao);

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa gestaoDevolucaoEtapaAnterior = _repositorioGestaoDevolucaoEtapa.BuscarEtapaPorCodigoGestaoEOrdem(gestaoDevolucao.Codigo, etapaAtual.Ordem - 1);
            if (gestaoDevolucaoEtapaAnterior != null)
            {
                gestaoDevolucaoEtapaAnterior.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.EmAndamento;
                gestaoDevolucaoEtapaAnterior.DataInicio = DateTime.Now;
                gestaoDevolucaoEtapaAnterior.DataFim = null;
                _repositorioGestaoDevolucaoEtapa.Atualizar(gestaoDevolucaoEtapaAnterior);

                gestaoDevolucao.EtapaAtual = gestaoDevolucaoEtapaAnterior;
                _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
            }

            Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), $"Voltou Etapa: {etapaAtual.Etapa.ObterDescricao()}", _unitOfWork);

            InformarGestaoDevolucaoAtualizada(gestaoDevolucao, _clienteMultisoftware?.Codigo ?? 199, SignalR.Hubs.GestaoDevolucaoHubs.InformarGestaoDevolucaoAtualizada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAtualizacaoGestaoDevolucao.VoltarEtapa).GetAwaiter().GetResult();
        }

        public void RejeitarEtapaGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa gestaoDevolucaoEtapa, string observacao = "")
        {
            gestaoDevolucaoEtapa.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.Rejeitada;
            gestaoDevolucaoEtapa.DataFim = DateTime.Now;
            gestaoDevolucaoEtapa.Observacao = observacao;
            _repositorioGestaoDevolucaoEtapa.Atualizar(gestaoDevolucaoEtapa);
        }

        public void AlterarTipoGestaoDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, TipoGestaoDevolucao tipoGestaoDevolucao)
        {
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa> listaEtapasAtuais = _repositorioGestaoDevolucaoEtapa.BuscarEtapaPorCodigoGestao(gestaoDevolucao.Codigo);

            gestaoDevolucao.Etapas = null;

            DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.NaoDefinido);
            AvancarEtapaGestaoDevolucao(gestaoDevolucao, trocaTipo: true);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao> etapasDevolucao = gestaoDevolucao.Tipo.ObterEtapasDevolucao(gestaoDevolucao.TipoNotas);
            for (int i = 0; i < gestaoDevolucao.Tipo.ObterEtapasDevolucao(gestaoDevolucao.TipoNotas).Count; i++)
                AvancarEtapaGestaoDevolucao(gestaoDevolucao);

            DefinirTipoGestaoDevolucao(gestaoDevolucao, tipoGestaoDevolucao);

            ICollection<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa> listaEtapasAtualizada = gestaoDevolucao.Etapas;

            foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa etapaAtualizada in listaEtapasAtualizada)
            {
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa etapaAntiga = listaEtapasAtuais.Find(etapaAntiga => etapaAntiga.Etapa == etapaAtualizada.Etapa);

                if (etapaAntiga != null)
                {
                    etapaAtualizada.SituacaoEtapa = etapaAntiga.SituacaoEtapa;
                    etapaAtualizada.DataInicio = etapaAntiga.DataInicio;
                    etapaAtualizada.DataFim = etapaAntiga.DataFim;
                    etapaAtualizada.Observacao = etapaAntiga.Observacao;
                }
            }

            _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);

        }

        public bool EnviarEmailGestaoCustoContabil(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo repositorioConfiguracaoModeloEmailAnexo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail configuracaoModeloEmail = _repositorioConfiguracaoModeloEmail.BuscarPorTipoModeloEmail(TipoModeloEmail.GestaoCustoContabilDevolucao).FirstOrDefault();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo configuracaoModeloEmailAnexo = repositorioConfiguracaoModeloEmailAnexo.BuscarPorModeloEmail(configuracaoModeloEmail?.Codigo ?? 0);

            Servicos.Email svcEmail = new Servicos.Email(_unitOfWork);


            string corpoEmail = BuscarCorpoEmail(gestaoDevolucao, configuracaoModeloEmail);
            corpoEmail = corpoEmail + configuracaoModeloEmail.RodaPe;
            string caminhoAnexo = configuracaoModeloEmailAnexo.CaminhoArquivo;

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoAnexo))
                return false;

            byte[] anexoArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoAnexo);
            anexos.Add(new System.Net.Mail.Attachment(new MemoryStream(anexoArquivo), $"Anexo: {configuracaoModeloEmailAnexo.NomeArquivo}"));

            if (!svcEmail.EnviarEmail("", "", "", gestaoDevolucao.Transportador.Email, "", "", configuracaoModeloEmail.Assunto, corpoEmail, "", anexos, "", false, "", 0, _unitOfWork))
            {
                Log.TratarErro($"Falha ao enviar o e-mail");
                return false;
            }

            return true;
        }

        public bool EnviarEmailCenarioPosEntrega(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail configuracaoModeloEmail = _repositorioConfiguracaoModeloEmail.BuscarPorTipoModeloEmail(TipoModeloEmail.ImprocedenciaCenarioPosEntregaDevolucao).FirstOrDefault();
            List<string> emails = _repositorioUsuario.BuscarEmailsUsuariosParaNotificacaoDevolucaoImprocedente();

            if (configuracaoModeloEmail == null)
                return false;

            Servicos.Email svcEmail = new Servicos.Email(_unitOfWork);

            string corpoEmail = BuscarCorpoEmail(gestaoDevolucao, configuracaoModeloEmail);
            corpoEmail = corpoEmail + configuracaoModeloEmail.RodaPe;

            foreach (string email in emails)
            {
                if (!svcEmail.EnviarEmail("", "", "", email, "", "", configuracaoModeloEmail.Assunto, corpoEmail, "", null, "", false, "", 0, _unitOfWork))
                {
                    Log.TratarErro($"Falha ao enviar o e-mail");
                    return false;
                }
            }

            return true;
        }

        public bool EnviarEmailNotificacaoTransportadorDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Servicos.Email svcEmail = new Servicos.Email(_unitOfWork);

            List<KeyValuePair<string, string>> linhas = new List<KeyValuePair<string, string>>();

            linhas.Add(new KeyValuePair<string, string>($"Nº Devolução ", gestaoDevolucao.Codigo.ToString()));
            linhas.Add(new KeyValuePair<string, string>("NF Devolução ", gestaoDevolucao.NotaFiscalDevolucao?.Numero.ToString() ?? "0"));
            linhas.Add(new KeyValuePair<string, string>("NF Origem ", gestaoDevolucao.NotaFiscalOrigem?.Numero.ToString() ?? "0"));
            linhas.Add(new KeyValuePair<string, string>("Prazo ", $" 72horas a partir da data - {gestaoDevolucao.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss")} "));
            linhas.Add(new KeyValuePair<string, string>("Transportador ", gestaoDevolucao.Transportador.Descricao));

            List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();
            mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
            {
                Destinatarios = new List<string> { gestaoDevolucao.Transportador.Email },
                Assunto = "Devolução de Mercadoria Pendente Para Análise",
                Corpo = Servicos.Email.TemplateCorpoEmail("Devolução de Mercadoria Pendente Para Análise", linhas, "Atenção: Existem devoluções de mercadoria pendentes para sua Análise", "Gestão Devolução " + cliente.Descricao, null, "E-mail enviado automaticamente.")
            });

            Servicos.Email.EnviarMensagensAsync(mensagens, _unitOfWork);

            return true;
        }

        public void FinalizarEtapasAnteriores(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa etapaAtual)
        {
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa> gestaoDevolucaoEtapaAnteriores = _repositorioGestaoDevolucaoEtapa.BuscarEtapasAnteriores(gestaoDevolucao.Codigo, etapaAtual);

            foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa etapa in gestaoDevolucaoEtapaAnteriores)
            {
                etapa.SituacaoEtapa = SituacaoEtapaGestaoDevolucao.Finalizada;
                etapa.DataInicio = etapa.DataInicio.HasValue ? etapa.DataInicio.Value : DateTime.Now;
                etapa.DataFim = DateTime.Now;
                etapa.Observacao = "Finalizada via avanço de etapa";
                _repositorioGestaoDevolucaoEtapa.Atualizar(etapa);
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao ObterItemGrid(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao devolucao, List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> notasOrigem, List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> notasDevolucao, List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa> etapasDevolucoes, IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> chamados, List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> clientesComplementares, List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> produtosDevolucoes, List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares> dadosComplementaresDevolucoes, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes)
        {
            List<int> codigosNotasFicaisOrigem = devolucao.NotasFiscaisDeOrigem.Select(nota => nota.XMLNotaFiscal.Codigo).ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> chamadosDaDevolucao = chamados.Where(cha => codigosNotasFicaisOrigem.Contains(cha.CodigoNotaFiscal)).OrderByDescending(c => c.DataCriacao).ToList();

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares gestaoDevolucaoDadosComplementares = dadosComplementaresDevolucoes.FirstOrDefault(d => d.Codigo == devolucao.Codigo);

            IList<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> notasDeOrigemDevolucaoAtual = notasOrigem.Where(x => x.GestaoDevolucao.Codigo == devolucao.Codigo).ToArray();
            IList<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> notasDeDevolucaoDevolucaoAtual = notasDevolucao.Where(x => x.GestaoDevolucao.Codigo == devolucao.Codigo).ToArray();

            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.ClienteDevolucao ClienteDevolucao = ObterCliente(clientesComplementares, notasDeOrigemDevolucaoAtual, notasDeDevolucaoDevolucaoAtual, gestaoDevolucaoDadosComplementares);

            Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado primeiroChamado = chamadosDaDevolucao.FirstOrDefault();
            bool possuiChamado = primeiroChamado != null;

            Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao itemGrid = new Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucao()
            {
                Codigo = devolucao.Codigo.ToString(),
                OrigemRecebimento = devolucao.OrigemRecebimento.ObterDescricao(),
                OrigemGeracao = devolucao.Geracao.ObterDescricao(),
                SituacaoDevolucao = devolucao.SituacaoDevolucao,
                SituacaoDevolucaoDescricao = devolucao.SituacaoDevolucao.ObterDescricao(),
                NFOrigem = devolucao.NotasFiscaisDeOrigem != null && devolucao.NotasFiscaisDeOrigem.Count > 0 ? string.Join(", ", devolucao.NotasFiscaisDeOrigem.Select(nota => nota.XMLNotaFiscal.Numero.ToString()).ToList()) : "-",
                NFDevolucao = devolucao.NotasFiscaisDevolucao != null && devolucao.NotasFiscaisDevolucao.Count > 0 ? string.Join(", ", devolucao.NotasFiscaisDevolucao.Select(nota => nota.XMLNotaFiscal.Numero.ToString()).ToList()) : "-",
                DataEmissaoNFDevolucao = devolucao.NotaFiscalDevolucao?.DataEmissao.ToString("dd/MM/yyyy") ?? "-",
                Transportador = devolucao.Transportador?.NomeCNPJ ?? "-",
                CargaOrigem = devolucao.CargaOrigem?.CodigoCargaEmbarcador ?? "-",
                CargaDevolucao = devolucao.CargaDevolucao?.CodigoCargaEmbarcador ?? "-",
                Filial = devolucao.CargaOrigem?.Filial?.Descricao ?? "-",
                Tomadores = devolucao.CargaOrigem?.Pedidos != null && devolucao.CargaOrigem.Pedidos.Any(pedido => pedido.Tomador != null) ? string.Join(",", devolucao.CargaOrigem?.Pedidos?.Select(pedido => pedido?.Tomador?.NomeCNPJ ?? string.Empty)) : "-",
                Aprovado = devolucao.AprovadaDescricao,
                Laudo = devolucao.LaudoDescricao,
                TipoDevolucaoDescricao = devolucao.Tipo == 0 ? "Definir" : devolucao.Tipo.ObterDescricao(),
                TipoFluxoGestaoDevolucaoDescricao = devolucao.TipoFluxoGestaoDevolucao.ObterDescricao(),
                TipoDevolucao = devolucao.Tipo,
                PosEntrega = devolucao.PosEntregaDescricao,
                ComPendenciaFinanceira = devolucao.ComPendenciaFinanceiraDescricao,
                Atendimentos = possuiChamado ? string.Join(", ", chamadosDaDevolucao.Select(chamados => chamados.NumeroChamado).Distinct().ToList()) : "-",
                EtapaAtual = devolucao.EtapaAtual.Etapa,
                EtapaAtualDescricao = devolucao.EtapaAtual.Etapa.ObterDescricao(),
                TipoNotasDevolucao = devolucao.TipoNotas.ObterDescricao(),
                Etapas = etapasDevolucoes.Count > 0 ?
                        Newtonsoft.Json.JsonConvert.SerializeObject(etapasDevolucoes.Where(o => o.GestaoDevolucao.Codigo == devolucao.Codigo).OrderBy(e => e.Ordem).Select(etapa => new Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ItemGridGestaoDevolucaoEtapa
                        {
                            Etapa = (int)etapa.Etapa,
                            Descricao = etapa.Etapa.ObterDescricao(),
                            SituacaoEtapa = (int)etapa.SituacaoEtapa,
                            Ordem = etapa.Ordem,
                            Observacao = etapa.Observacao ?? string.Empty,
                            DataInicio = etapa.DataInicio?.ToString() ?? "",
                            DataFim = etapa.DataFim?.ToString() ?? ""
                        }).ToList()) :
                        null,
                PrazoEscolhaTipoDevolucao = devolucao.DataCriacao.AddHours(72).ToString(),
                ControleFinalizacaoDevolucaoDescricao = gestaoDevolucaoDadosComplementares != null ? System.BoolExtensions.ObterDescricao(gestaoDevolucaoDadosComplementares.ControleFinalizacaoDevolucao) : "-",
                Valor = ClienteDevolucao.Valor,
                StatusAtendimento = possuiChamado ? primeiroChamado.Situacao.ObterDescricao() : "-",
                MotivoAtendimento = possuiChamado ? primeiroChamado.MotivoChamadoDescricao : "-",
                DataAtendimento = possuiChamado ? primeiroChamado.DataCriacaoFormatada.ToString() : "-",
                Ordem = devolucao.ObservacaoOrdemRemessa ?? "-",
                Remessa = devolucao.ObservacaoOrdemRemessa ?? "-",
                Volume = produtosDevolucoes.Count > 0 ? produtosDevolucoes.Count(p => p.Codigo == devolucao.Codigo) : 0,
                DataAgendamento = ObterDescricaoPeriodo(gestaoDevolucaoDadosComplementares) ?? "-",
                NLaudo = devolucao.Laudo != null ? devolucao.Laudo?.NumeroCompensacao : "-",
                DataNFD = string.Join(", ", notasDeDevolucaoDevolucaoAtual?.Select(nota => nota.XMLNotaFiscal.DataEmissao.ToDateString())) ?? "-",
                DataCanhoto = string.Join(", ", notasDeDevolucaoDevolucaoAtual?.Select(nota => nota.XMLNotaFiscal.Canhoto?.DataDigitalizacao.ToDateString())) ?? "-",
                ClienteCPFCNPJ = ClienteDevolucao.CPF_CNPJ,
                ClienteNome = ClienteDevolucao.Nome,
                EscritorioVendas = ClienteDevolucao.EscritorioVendas,
                EquipeVendas = ClienteDevolucao.EquipeVendas,
                DocContabil = ClienteDevolucao.ContaContabil,
                TipoRecusa = ObterTipoRecusa(notasDeOrigemDevolucaoAtual, notasDeDevolucaoDevolucaoAtual),
                Aprovacao = System.BoolExtensions.ObterDescricao(devolucao.Aprovada),
                Custo = chamadosDaDevolucao.Count > 0 ? System.BoolExtensions.ObterDescricao(chamadosDaDevolucao.FirstOrDefault().FreteRetornoDevolucao) : "-"
            };
            return itemGrid;
        }

        private static decimal? ObterSomaNotas(IEnumerable<dynamic> notas)
        {
            if (notas == null || !notas.Any())
                return 0;

            return notas.Sum(nota => (decimal?)nota.XMLNotaFiscal.Valor ?? 0);
        }

        private static string ObterDescricaoPeriodo(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares gestaoDevolucaoDadosComplementares)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaMovimentacaoPallet consultaMovimentacaoPallet = new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaMovimentacaoPallet
            {
                DataDescarregamento = gestaoDevolucaoDadosComplementares?.DataDescarregamento,
                PeriodoDescarregamentoHoraInicio = gestaoDevolucaoDadosComplementares?.PeriodoDescarregamento?.HoraInicio.ToString(@"hh\:mm"),
                PeriodoDescarregamentoHoraTermino = gestaoDevolucaoDadosComplementares?.PeriodoDescarregamento?.HoraTermino.ToString(@"hh\:mm")
            };

            return consultaMovimentacaoPallet.DataAgendamentoDevolucao;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.ClienteDevolucao ObterCliente(List<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> clientesComplementares, IEnumerable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> notasDeOrigemDevolucaoAtual, IEnumerable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> notasDeDevolucaoDevolucaoAtual, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares gestaoDevolucaoDadosComplementares)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.ClienteDevolucao cliente = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.ClienteDevolucao
            {
                CPF_CNPJ = "-",
                Nome = "-",
                EscritorioVendas = "-",
                EquipeVendas = "-",
                ContaContabil = gestaoDevolucaoDadosComplementares?.ContaContabil ?? "-",
                Valor = "-"
            };

            decimal somaOrigem = ObterSomaNotas(notasDeOrigemDevolucaoAtual) ?? 0;
            decimal somaDevolucao = ObterSomaNotas(notasDeDevolucaoDevolucaoAtual) ?? 0;
            decimal valorTotal = somaOrigem + somaDevolucao;

            if (valorTotal > 0)
                cliente.Valor = valorTotal.ToString("N2");

            if (notasDeDevolucaoDevolucaoAtual == null || !notasDeDevolucaoDevolucaoAtual.Any())
                return cliente;

            var codigosDestinatarios = notasDeDevolucaoDevolucaoAtual
                .Select(n => n.XMLNotaFiscal.Destinatario.Codigo)
                .Distinct()
                .ToList();

            var clientesRelacionados = clientesComplementares
                .Where(cli => cli.Cliente != null && codigosDestinatarios.Contains(cli.Cliente.Codigo))
                .ToList();

            if (clientesRelacionados.Any())
            {
                cliente.CPF_CNPJ = string.Join(", ", clientesRelacionados.Select(cli => cli.Cliente.Codigo).Distinct());
                cliente.Nome = string.Join(", ", clientesRelacionados.Select(cli => cli.Cliente.Nome).Distinct());
                cliente.EscritorioVendas = string.Join(", ", clientesRelacionados.Select(cli => cli.EscritorioVendas).Distinct());
                cliente.EquipeVendas = string.Join(", ", clientesRelacionados.Select(cli => cli.EquipeVendas).Distinct());
            }

            return cliente;
        }

        public static string ObterTipoRecusa(IList<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> notasDeOrigemDevolucaoAtual, IList<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> notasDeDevolucaoDevolucaoAtual)
        {
            bool temNotasOrigem = notasDeOrigemDevolucaoAtual != null && notasDeOrigemDevolucaoAtual.Count > 0;
            bool temNotasDevolucao = notasDeDevolucaoDevolucaoAtual != null && notasDeDevolucaoDevolucaoAtual.Count > 0;

            if (temNotasOrigem && temNotasDevolucao)
                return "Mista";
            else if (temNotasOrigem)
                return "Total";
            else if (temNotasDevolucao)
                return "Parcial";
            else
                return "-";
        }

        public void AtualizarGestaoDevolucaoNaGrid(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            InformarGestaoDevolucaoAtualizada(gestaoDevolucao, _clienteMultisoftware?.Codigo ?? 199, SignalR.Hubs.GestaoDevolucaoHubs.InformarGestaoDevolucaoAtualizada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAtualizacaoGestaoDevolucao.AtualizarGrid);
        }

        public async Task AtualizarAsync(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new(_unitOfWork);

            await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao, Auditado: _auditado);
        }

        public async Task AtualizarComDadosComplementaresAsync(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new(_unitOfWork);


            var changes = gestaoDevolucao.DadosComplementares.GetCurrentChanges();

            gestaoDevolucao.SetExternalChanges(changes);

            await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao, Auditado: _auditado);
            await repositorioGestaoDevolucaoDadosComplementares.AtualizarAsync(gestaoDevolucao.DadosComplementares);
        }

        public byte[] ObterPDFLaudo(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo)
        {
            if (!string.IsNullOrEmpty(gestaoDevolucaoLaudo.CaminhoArquivoLaudo) && Utilidades.IO.FileStorageService.Storage.Exists(gestaoDevolucaoLaudo.CaminhoArquivoLaudo))
            {
                return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(gestaoDevolucaoLaudo.CaminhoArquivoLaudo);
            }
            else
            {
                try
                {
                    ReportResult reportResult = ReportRequest.WithType(ReportType.GestaoDevolucaoLaudo)
                        .WithExecutionType(ExecutionType.Sync)
                        .AddExtraData("CodigoGestaoDevolucaoLaudo", gestaoDevolucaoLaudo.Codigo.ToString())
                        .CallReport();

                    gestaoDevolucaoLaudo.CaminhoArquivoLaudo = reportResult.FullPath;
                    _repositorioGestaoDevolucaoLaudo.Atualizar(gestaoDevolucaoLaudo);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucaoLaudo, gestaoDevolucaoLaudo.GetChanges(), $"Geração do PDF {reportResult.FileName}", _unitOfWork);
                    return reportResult.GetContentFile();
                }
                catch (Exception ex)
                {
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucaoLaudo, $"Falha na geração do PDF: {ex.Message}", _unitOfWork);
                    return new byte[0];
                }
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga AdicionarCargaDevolucao(Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracaoDescarregamento = new() { NaoPermitirBuscarOutroPeriodo = true };
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCNPJ(gestaoDevolucao.DadosComplementares.DestinatarioAgendamento.CPF_CNPJ_SemFormato) ?? throw new ServicoException($"Não foi possível localizar a filial.");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = GerarPedidoECarga(gestaoDevolucao, filial, _clienteMultisoftware, _configuracaoEmbarcador, _tipoServicoMultisoftware);

            gestaoDevolucao.CargaDevolucao = carga;
            _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);

            if (gestaoDevolucao.Tipo == TipoGestaoDevolucao.Agendamento)
            {
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega = new Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega()
                {
                    Carga = carga,
                    ModeloVeicularCarga = carga.ModeloVeicularCarga,
                    Motorista = carga.Motoristas?.FirstOrDefault()?.Nome ?? gestaoDevolucao.DadosComplementares.Motorista?.Nome ?? "",
                    Placa = carga.Veiculo?.Placa ?? gestaoDevolucao.DadosComplementares.Veiculo?.Placa ?? "",
                    TipoDeCarga = carga.TipoDeCarga,
                    Transportador = carga.Empresa?.Descricao ?? gestaoDevolucao.DadosComplementares.Transportador?.Descricao ?? "",
                    DataAgendamento = gestaoDevolucao.DadosComplementares.DataDescarregamento.Value,
                    Destinatario = gestaoDevolucao.DadosComplementares.RemetenteAgendamento,
                    Remetente = gestaoDevolucao.DadosComplementares.RemetenteAgendamento,
                    Senha = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper(),
                    SenhaAgendamento = repositorioAgendamentoEntrega.ObterProximaSenhaAgendamento(),
                    Situacao = SituacaoAgendamentoEntrega.Agendado,
                };

                repositorioAgendamentoEntrega.Inserir(agendamentoEntrega);

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(_unitOfWork, _configuracaoEmbarcador, configuracaoDescarregamento);
                servicoJanelaDescarregamento.Adicionar(gestaoDevolucao.CargaDevolucao, gestaoDevolucao.DadosComplementares.DataDescarregamento ?? DateTime.Now, _tipoServicoMultisoftware, gestaoDevolucao.DadosComplementares.PeriodoDescarregamento);

            }
            return carga;
        }

        public void MovimentarEtapaCargaDevolucao(Dominio.Entidades.Embarcador.Cargas.Carga cargaDevolucao, bool avancar, string observacao = null)
        {
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = _repositorioGestaoDevolucao.BuscarPorCodigoCarga(cargaDevolucao.Codigo);
            if (gestaoDevolucao != null)
            {
                if (avancar)
                    AvancarEtapaGestaoDevolucao(gestaoDevolucao, observacao);
                else
                    VoltarEtapaGestaoDevolucao(gestaoDevolucao, observacao);
            }
        }

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> GerarDevolucaoPallet(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscais, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Enumeradores.TipoTomador? tipoTomadorNota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoDevolucao tipoFluxoGestaoDevolucao = TipoFluxoGestaoDevolucao.Normal)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado repCargaEntregaProdutoChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado(_unitOfWork);


            List<int> codigosNotas = xmlNotasFiscais.Select(nota => nota.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xMLNotasFiscaisProdutos = null;
            IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> chamados = null;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargasEntregaProdutosChamado = null;
            List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> notaFiscalProdutosRetornar = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();

            if (xmlNotasFiscais.FirstOrDefault().TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet)
                xMLNotasFiscaisProdutos = repositorioXMLNotaFiscalProduto.BuscarPorNotaFiscais(codigosNotas);
            else
            {
                chamados = _repositorioChamado.BuscarChamadosPorCodigosNotasFiscais(codigosNotas);
                cargasEntregaProdutosChamado = repCargaEntregaProdutoChamado.BuscarPorChamados(chamados.Select(chamado => chamado.Codigo).ToList());
            }

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in xmlNotasFiscais)
            {
                if (xMLNotasFiscaisProdutos != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produto = xMLNotasFiscaisProdutos.FirstOrDefault(p => p.Codigo == notaFiscal.Codigo);

                    if (produto != null)
                    {
                        notaFiscalProdutosRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos
                        {
                            CodigoCEAN = produto.Produto.CodigoCEAN,
                            QuantidadeComercial = produto.Quantidade,
                            ValorUnitarioComercial = produto.ValorProduto,
                        });
                    }
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> chamadosNota = chamados.Where(c => c.CodigoNotaFiscal == notaFiscal.Codigo).ToList();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado chamado in chamadosNota)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> produtosChamado = cargasEntregaProdutosChamado.Where(p => p.Chamado.Codigo == chamado.Codigo).ToList();

                        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado produtoChamado in produtosChamado)
                        {
                            notaFiscalProdutosRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos
                            {
                                CodigoCEAN = produtoChamado.Produto.CodigoCEAN,
                                QuantidadeComercial = produtoChamado.QuantidadeDevolucao,
                                ValorUnitarioComercial = produtoChamado.Produto.ValorTotal,
                            });
                        }
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> gestaoDevolucaoNFDxNFO = _repositorioGestaoDevolucaoNFDxNFO.BuscarPorNFO(codigosNotas);

            bool posEntrega = PosEntrega(gestaoDevolucaoNFDxNFO);

            Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ParametrosGestaoDevolucao parametrosGestaoDevolucao = new Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.ParametrosGestaoDevolucao
            {
                Geracao = GeracaoGestaoDevolucao.Manual,
                NotasFiscaisDeOrigem = xmlNotasFiscais,
                NotasFiscaisDeDevolucao = gestaoDevolucaoNFDxNFO?.Select(gd => gd.NFD).ToList() ?? null,
                Produtos = notaFiscalProdutosRetornar,
                Carga = carga,
                Transportador = carga.Empresa,
                Filial = carga.Filial,
                PosEntrega = posEntrega,
                TipoFluxoGestaoDevolucao = tipoFluxoGestaoDevolucao,
                OrigemRecebimento = (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ? OrigemGestaoDevolucao.PortalEmbarcador :
                                    (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) ? OrigemGestaoDevolucao.PortalTransportador : OrigemGestaoDevolucao.PortalCliente
            };

            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await CriarGestaoDevolucao(parametrosGestaoDevolucao);
            if (gestaoDevolucao != null)
            {
                if (tipoTomadorNota != null && gestaoDevolucao.DadosComplementares == null)
                {
                    Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares gestaoDevolucaoDadosComplementares = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares()
                    {
                        GestaoDevolucao = gestaoDevolucao,
                        TipoTomador = tipoTomadorNota
                    };

                    gestaoDevolucao.DadosComplementares = gestaoDevolucaoDadosComplementares;
                    await _repositorioGestaoDevolucaoDadosComplementares.InserirAsync(gestaoDevolucaoDadosComplementares);
                }

                if (gestaoDevolucao.TipoFluxoGestaoDevolucao == TipoFluxoGestaoDevolucao.Simples)
                {
                    DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.Simplificado);
                    DefinirEtapaAtualGestaoDevolucao(gestaoDevolucao, EtapaGestaoDevolucao.GeracaoLaudo);
                }

                else if (gestaoDevolucao.PosEntrega)
                {
                    DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.PosEntrega);
                    DefinirEtapaAtualGestaoDevolucao(gestaoDevolucao, EtapaGestaoDevolucao.GestaoDeDevolucao);
                    AvancarEtapaGestaoDevolucao(gestaoDevolucao);
                }

                else if (gestaoDevolucao.TipoFluxoGestaoDevolucao == TipoFluxoGestaoDevolucao.Normal)
                {
                    DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.NaoDefinido);
                    DefinirEtapaAtualGestaoDevolucao(gestaoDevolucao, EtapaGestaoDevolucao.DefinicaoTipoDevolucao);
                }
            }
            return gestaoDevolucao;
        }

        private bool PosEntrega(List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO> gestaoDevolucaoNFDxNFO)
        {
            bool posEntrega = false;
            Dominio.Entidades.Embarcador.Canhotos.Canhoto[] canhotosNFOs = gestaoDevolucaoNFDxNFO.Select(nfdxnfo => nfdxnfo.NFO.Canhoto).ToArray();

            if (canhotosNFOs.Length == 0)
                posEntrega = true;
            else if (gestaoDevolucaoNFDxNFO.Count > 0)
            {
                for (int i = 0; i < canhotosNFOs.Length; i++)
                {
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = canhotosNFOs[i];

                    if (canhoto.DataDigitalizacao.HasValue)
                    {
                        int nfoCanhoto = canhoto.XMLNotaFiscal.Codigo;
                        DateTime canhotoDataDigitalizacao = canhoto.DataDigitalizacao.Value;

                        if (gestaoDevolucaoNFDxNFO.Exists(nfdxnfo => nfdxnfo.NFO.Codigo == nfoCanhoto && nfdxnfo.NFD.DataEmissao.Date != canhotoDataDigitalizacao.Date))
                        {
                            posEntrega = true;
                            break;
                        }
                    }
                }
            }

            return posEntrega;
        }

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> SalvarPosEntrega(string observacao, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao posEntrega)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(_unitOfWork);

            gestaoDevolucao.PosEntrega = (posEntrega == SimNao.Sim);

            if (!string.IsNullOrEmpty(observacao) && gestaoDevolucao.DadosComplementares != null)
            {
                gestaoDevolucao.DadosComplementares.ObservacaoAprovacao = observacao;
                await repositorioGestaoDevolucaoDadosComplementares.AtualizarAsync(gestaoDevolucao.DadosComplementares);
            }

            await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

            return gestaoDevolucao;
        }

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> AprovarPosEntrega(TipoGestaoDevolucao tipoGestaoDevolucao, Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao, Dominio.Entidades.Usuario Usuario)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares repositorioGestaoDevolucaoDadosComplementares = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares(_unitOfWork);

            if (gestaoDevolucao.PosEntrega)
            {
                if (gestaoDevolucao.DadosComplementares != null)
                {
                    gestaoDevolucao.DadosComplementares.UsuarioAprovacao = Usuario;
                    await repositorioGestaoDevolucaoDadosComplementares.AtualizarAsync(gestaoDevolucao.DadosComplementares);
                }

                AvancarEtapaGestaoDevolucao(gestaoDevolucao);
                gestaoDevolucao.Aprovada = true;

                await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);

                GerarIntegracoes(gestaoDevolucao, TipoIntegracaoGestaoDevolucao.SalesforcePosEntrega, new List<TipoIntegracao>() { TipoIntegracao.Salesforce });
            }
            else
            {
                AlterarTipoGestaoDevolucao(gestaoDevolucao, tipoGestaoDevolucao);
                AvancarEtapaGestaoDevolucao(gestaoDevolucao);

                await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);
            }
            await repositorioGestaoDevolucaoDadosComplementares.AtualizarAsync(gestaoDevolucao.DadosComplementares);

            return gestaoDevolucao;
        }

        public void CancelarPorCargaDevolucao(int codigoCarga, string motivoCancelamento)
        {
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = _repositorioGestaoDevolucao.BuscarPorCodigoCarga(codigoCarga);
            if (gestaoDevolucao != null)
            {
                gestaoDevolucao.SituacaoDevolucao = SituacaoGestaoDevolucao.Cancelada;

                if (gestaoDevolucao.DadosComplementares != null)
                    gestaoDevolucao.DadosComplementares.ObservacaoCancelamento = motivoCancelamento;

                _repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
                AtualizarGestaoDevolucaoNaGrid(gestaoDevolucao);
            }
        }
        #endregion

        #region Classes Auxiliares
        public class TagValorGestaoDevolucao
        {
            public string Tag { get; set; }
            public string Valor { get; set; }
        }
        public class DadosNotasFiscaisChamado
        {
            public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscal { get; set; }
            public List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> Produtos { get; set; }
        }
        #endregion
    }
}
