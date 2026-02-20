using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.WebService.Carga
{
    public class ProdutosPedido : ServicoBase
    {
        #region Construtores
        
        public ProdutosPedido() : base() { }
        public ProdutosPedido(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Métodos Públicos

        public void AtualizarProdutosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            AtualizarProdutosPedido(pedido, carga, cargaIntegracao.Produtos, cargaIntegracao.ProdutoPredominante, cargaIntegracao.CubagemTotal, cargaIntegracao.PesoBruto, cargaIntegracao.ValorTotalPedido, cargaIntegracao.NumeroPaletesFracionado, ref stMensagem, configuracao, unitOfWork, auditado, atualizarPesoPedidoComPesoProduto: false);
        }

        public void AtualizarProdutosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoPredominante, decimal cubagemTotal, decimal pesoBruto, decimal valorTotalPedido, decimal numeroPaletesFracionado, ref StringBuilder stMensagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool atualizarPesoPedidoComPesoProduto)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);

            if (carga != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(carga.CargaAgrupamento?.Codigo ?? carga.Codigo, pedido.Codigo);

                if (cargaPedido != null)
                    repCargaPedidoProduto.DeletarCargaPedidoProdutoPorCodigoCargaPedidoViaQuery(cargaPedido.Codigo);
                else
                    stMensagem.Append("Não foi possível localizar o vínculo entre a carga e o pedido informado; ");
            }

            List<int> codigosPedidoProdutoCarregamento = repCarregamentoPedidoProduto.CodigosCarregamentoPedidoProdutoPorPedido(pedido.Codigo);

            if (codigosPedidoProdutoCarregamento.Count > 0)
                repCarregamentoPedidoProduto.DeletarCarregamentoPedidoProdutoPorCodigosCarregamentoPedidoProdutoViaQuery(codigosPedidoProdutoCarregamento);

            repPedidoProduto.DeletarPedidoProdutoPorCodigoPedidoViaQuery(pedido.Codigo);

            AdicionarProdutosPedido(pedido, produtosIntegracao, produtoPredominante, cubagemTotal, pesoBruto, valorTotalPedido, numeroPaletesFracionado, configuracao, ref stMensagem, unitOfWork, auditado, atualizarPesoPedidoComPesoProduto);
        }

        public void RemoverTransbordo(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo> transbordos, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosDoPedido = repPedidoTransbordo.BuscarPorPedido(pedido.Codigo);
            if (transbordosDoPedido != null && transbordosDoPedido.Count > 0)
            {
                foreach (var transbordo in transbordosDoPedido)
                    repPedidoTransbordo.Deletar(transbordo);
            }
        }

        public void SalvarTransbordo(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo> transbordos, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool encerrarMDFeAutomaticamente)
        {
            Servicos.WebService.Carga.Pedido svcPedido = new Pedido(unitOfWork);

            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordosDoPedido = repPedidoTransbordo.BuscarPorPedido(pedido.Codigo);
            foreach (var transbordo in transbordosDoPedido)
                repPedidoTransbordo.Deletar(transbordo);

            foreach (var transbordo in transbordos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = svcPedido.SalvarNavio(transbordo.Navio, ref stMensagem, Auditado, false);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = svcPedido.SalvarTerminalPorto(transbordo.Terminal, ref stMensagem, Auditado);
                Dominio.Entidades.Embarcador.Pedidos.Porto porto = svcPedido.SalvarPorto(transbordo.Porto, ref stMensagem, Auditado);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = svcPedido.SalvarViagem(transbordo.Viagem, ref stMensagem, Auditado, encerrarMDFeAutomaticamente);

                Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordo = repPedidoTransbordo.BuscarTransbordoExistente(transbordo.Sequencia, pedido.Codigo);
                if (pedidoTransbordo != null)
                {
                    pedidoTransbordo.Navio = viagem?.Navio ?? navio;
                    pedidoTransbordo.Porto = porto;
                    pedidoTransbordo.Sequencia = transbordo.Sequencia;
                    pedidoTransbordo.Terminal = terminal;
                    pedidoTransbordo.PedidoViagemNavio = viagem;

                    repPedidoTransbordo.Atualizar(pedidoTransbordo);
                }
                else
                {
                    pedidoTransbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo()
                    {
                        Navio = viagem?.Navio ?? navio,
                        Pedido = pedido,
                        Porto = porto,
                        Sequencia = transbordo.Sequencia,
                        Terminal = terminal,
                        PedidoViagemNavio = viagem,
                    };
                    repPedidoTransbordo.Inserir(pedidoTransbordo);
                }
            }
        }

        public string AjustarBasesParaProdutos(List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, ref List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto, ref List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhasSeparacao, ref List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> enderecosProdutos, ref List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> tiposEmbalagem, ref List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcaProdutos, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosVinculados = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto> gruposProdutos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao> linhaSeparacaos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.LinhaSeparacao>();
            List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto> enderecosProduto = new List<Dominio.ObjetosDeValor.Embarcador.Produtos.EnderecoProduto>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem> tipoEmbalagens = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoEmbalagem>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto> marcas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.MarcaProduto>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in produtosIntegracao)
            {
                if (!string.IsNullOrWhiteSpace(produtocargaIntegracao.CodigoGrupoProduto) && !string.IsNullOrWhiteSpace(produtocargaIntegracao.DescricaoGrupoProduto))
                {
                    if (!gruposProdutos.Any(obj => obj.Codigo == produtocargaIntegracao.CodigoGrupoProduto))
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto grupoProduto = new Dominio.ObjetosDeValor.Embarcador.NFe.GrupoProduto();
                        grupoProduto.Codigo = produtocargaIntegracao.CodigoGrupoProduto;
                        grupoProduto.Descricao = produtocargaIntegracao.DescricaoGrupoProduto;
                        gruposProdutos.Add(grupoProduto);
                    }
                }
                else
                {
                    // Só cria o erro caso a flag abaixo não esteja marcada
                    if (configuracaoCargaIntegracao?.AceitarPedidosComPendenciasDeProdutos != true)
                        return "Grupo de Produto não informado;";

                }

                if (produtocargaIntegracao.LinhaSeparacao != null && !string.IsNullOrWhiteSpace(produtocargaIntegracao.LinhaSeparacao.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtocargaIntegracao.LinhaSeparacao.Descricao))
                {
                    if (!linhaSeparacaos.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.LinhaSeparacao.CodigoIntegracao))
                        linhaSeparacaos.Add(produtocargaIntegracao.LinhaSeparacao);
                }

                if (produtocargaIntegracao.EnderecoProduto != null && !string.IsNullOrWhiteSpace(produtocargaIntegracao.EnderecoProduto?.CodigoIntegracao ?? "") && !string.IsNullOrWhiteSpace(produtocargaIntegracao.EnderecoProduto?.Descricao ?? ""))
                {
                    if (!enderecosProduto.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.EnderecoProduto.CodigoIntegracao))
                        enderecosProduto.Add(produtocargaIntegracao.EnderecoProduto);
                }

                if (produtocargaIntegracao.TipoEmbalagem != null && !string.IsNullOrWhiteSpace(produtocargaIntegracao.TipoEmbalagem.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtocargaIntegracao.TipoEmbalagem.Descricao))
                {
                    if (!tipoEmbalagens.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.TipoEmbalagem.CodigoIntegracao))
                        tipoEmbalagens.Add(produtocargaIntegracao.TipoEmbalagem);
                }


                if (produtocargaIntegracao.MarcaProduto != null && !string.IsNullOrWhiteSpace(produtocargaIntegracao.MarcaProduto.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtocargaIntegracao.MarcaProduto.Descricao))
                {
                    if (!marcas.Any(obj => obj.CodigoIntegracao == produtocargaIntegracao.MarcaProduto.CodigoIntegracao))
                        marcas.Add(produtocargaIntegracao.MarcaProduto);
                }
            }

            gruposProduto = servicoProdutoEmbarcador.IntegrarGruposProduto(gruposProdutos);
            linhasSeparacao = servicoProdutoEmbarcador.IntegrarLinhasSeparacao(linhaSeparacaos, auditado);
            enderecosProdutos = servicoProdutoEmbarcador.IntegrarEnderecosProdutos(enderecosProduto, auditado);
            tiposEmbalagem = servicoProdutoEmbarcador.IntegrarTiposEmbalagem(tipoEmbalagens);
            marcaProdutos = servicoProdutoEmbarcador.IntegrarMarcasProduto(marcas, auditado);
            return "";
        }

        public void ObterBasesParaCadastroProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao, List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto, List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhasSeparacao, List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> enderecosProdutos, List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> tiposEmbalagem, List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcaProdutos, ref Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto, ref Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao, ref Dominio.Entidades.Embarcador.Produtos.EnderecoProduto enderecoProduto, ref Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem, ref Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
            grupoProduto = (from obj in gruposProduto where obj.CodigoGrupoProdutoEmbarcador == produtocargaIntegracao.CodigoGrupoProduto select obj).FirstOrDefault();

            if (produtocargaIntegracao.LinhaSeparacao != null)
                linhaSeparacao = (from obj in linhasSeparacao where obj.CodigoIntegracao == produtocargaIntegracao.LinhaSeparacao.CodigoIntegracao select obj).FirstOrDefault();

            if (produtocargaIntegracao.EnderecoProduto != null)
                enderecoProduto = (from obj in enderecosProdutos where obj.CodigoIntegracao == produtocargaIntegracao.EnderecoProduto.CodigoIntegracao select obj).FirstOrDefault();

            if (produtocargaIntegracao.TipoEmbalagem != null)
                tipoEmbalagem = (from obj in tiposEmbalagem where obj.CodigoIntegracao == produtocargaIntegracao.TipoEmbalagem.CodigoIntegracao select obj).FirstOrDefault();

            if (produtocargaIntegracao.MarcaProduto != null)
                marcaProduto = (from obj in marcaProdutos where obj.CodigoIntegracao == produtocargaIntegracao.MarcaProduto.CodigoIntegracao select obj).FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> VincularProdutosAoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, decimal cubagemTotal, decimal pesoBruto, decimal valorTotalPedido, decimal numeroPaletesFracionado, bool utilizarPesoProdutoParaCalcularPesoCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool atualizarPesoPedidoComPesoProduto, bool habilitarCadastroArmazem)
        {
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosVinculados = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProdutoLote repositorioPedidoProdutoLote = new Repositorio.Embarcador.Pedidos.PedidoProdutoLote(unitOfWork);
            Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProduto = null;
            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhasSeparacao = null;
            List<Dominio.Entidades.Embarcador.Produtos.EnderecoProduto> enderecosProdutos = null;
            List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> tiposEmbalagem = null;
            List<Dominio.Entidades.Embarcador.Produtos.MarcaProduto> marcaProdutos = null;

            string retorno = AjustarBasesParaProdutos(produtosIntegracao, ref gruposProduto, ref linhasSeparacao, ref enderecosProdutos, ref tiposEmbalagem, ref marcaProdutos, configuracaoCargaIntegracao, Auditado, unitOfWork);

            if (!string.IsNullOrWhiteSpace(retorno))
            {
                stMensagem.Append(retorno);
                return null;
            }

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            if (!configuracao.SempreInserirNovoProdutoPorIntegracao)
                produtos = repProdutoEmbarcador.BuscarPorCodigosEmbarcadorPadraoLimpo((from obj in produtosIntegracao select obj.CodigoProduto).Distinct().ToList());

            if (produtosIntegracao.Count > 0 && valorTotalPedido <= 0)
                pedido.ValorTotalNotasFiscais = 0;

            if (cubagemTotal == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                pedido.CubagemTotal = 0;

            if (utilizarPesoProdutoParaCalcularPesoCarga)
            {
                pedido.PesoTotal = 0;
                pedido.PesoLiquidoTotal = 0;
            }
            else if (pesoBruto == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                pedido.PesoTotal = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in produtosIntegracao)
            {
                if (string.IsNullOrWhiteSpace(produtocargaIntegracao?.CodigoProduto))
                    stMensagem.Append("Produto não informado; ");

                if (stMensagem.Length > 0)
                    continue;

                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = null;
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = null;
                Dominio.Entidades.Embarcador.Produtos.EnderecoProduto enderecoProduto = null;
                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = null;
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = null;
                ObterBasesParaCadastroProduto(produtocargaIntegracao, gruposProduto, linhasSeparacao, enderecosProdutos, tiposEmbalagem, marcaProdutos, ref grupoProduto, ref linhaSeparacao, ref enderecoProduto, ref tipoEmbalagem, ref marcaProduto);

                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                pedidoProduto.Produto = servicoProdutoEmbarcador.IntegrarProduto(produtos, configuracao, produtocargaIntegracao.CodigoProduto, produtocargaIntegracao.DescricaoProduto, produtocargaIntegracao.PesoUnitario, grupoProduto, produtocargaIntegracao.MetroCubito, Auditado, produtocargaIntegracao.CodigoDocumentacao, produtocargaIntegracao.Atualizar, produtocargaIntegracao.CodigoNCM, produtocargaIntegracao.QuantidadePorCaixa, produtocargaIntegracao.QuantidadeCaixaPorPallet, produtocargaIntegracao.Altura, produtocargaIntegracao.Largura, produtocargaIntegracao.Comprimento, linhaSeparacao, tipoEmbalagem, marcaProduto, produtocargaIntegracao.UnidadeMedida, produtocargaIntegracao.Observacao, "", produtocargaIntegracao?.CodigoEAN ?? string.Empty);
                pedidoProduto.Pedido = pedido;
                produtosVinculados.Add(pedidoProduto.Produto);
                pedidoProduto.Observacao = !string.IsNullOrWhiteSpace(produtocargaIntegracao.Observacao) ? produtocargaIntegracao.Observacao : pedidoProduto.Produto.Observacao;
                pedidoProduto.ValorProduto = produtocargaIntegracao.ValorUnitario;
                pedidoProduto.PrecoUnitario = produtocargaIntegracao.ValorUnitario;
                pedidoProduto.QuantidadeEmbalagem = produtocargaIntegracao.QuantidadeEmbalagem;
                pedidoProduto.PesoTotalEmbalagem = produtocargaIntegracao.PesoTotalEmbalagem;
                pedidoProduto.Quantidade = produtocargaIntegracao.Quantidade;
                pedidoProduto.QuantidadePlanejada = produtocargaIntegracao.QuantidadePlanejada;
                pedidoProduto.PesoUnitario = (produtocargaIntegracao.Atualizar || utilizarPesoProdutoParaCalcularPesoCarga) ? pedidoProduto.Produto.PesoUnitario : produtocargaIntegracao.PesoUnitario;
                pedidoProduto.PalletFechado = produtocargaIntegracao.PalletFechado;
                pedidoProduto.CamposPersonalizados = produtocargaIntegracao.CamposPersonalizados;
                pedidoProduto.UnidadeMedidaSecundaria = produtocargaIntegracao.UnidadeMedidaSecundaria;
                pedidoProduto.QuantidadeSecundaria = produtocargaIntegracao.QuantidadeSecundaria;

                int quantidadePorCaixa = pedidoProduto.Produto.QuantidadeCaixa > 0 ? pedidoProduto.Produto.QuantidadeCaixa : (pedidoProduto.Produto.GrupoProduto?.QuantidadePorCaixa ?? 0);

                if (!configuracao.CubagemPorCaixa || quantidadePorCaixa == 0)
                    pedidoProduto.MetroCubico = configuracao.MetroCubicoPorUnidadePedidoProdutoIntegracao ? pedidoProduto.Produto.MetroCubito * pedidoProduto.Quantidade : pedidoProduto.Produto.MetroCubito;
                else
                {
                    int resto = (int)Math.Ceiling(pedidoProduto.Quantidade / quantidadePorCaixa);
                    pedidoProduto.MetroCubico = pedidoProduto.Produto.MetroCubito * resto;
                }

                if (pedidoProduto.MetroCubico == 0 && produtocargaIntegracao.MetroCubito > 0)
                    pedidoProduto.MetroCubico = produtocargaIntegracao.MetroCubito;

                pedidoProduto.SetorLogistica = produtocargaIntegracao.SetorLogistica;
                pedidoProduto.ClasseLogistica = produtocargaIntegracao.ClasseLogistica;
                pedidoProduto.QuantidadePalet = produtocargaIntegracao.QuantidadePallet;
                pedidoProduto.CanalDistribuicao = produtocargaIntegracao.CanalDistribuicao;
                pedidoProduto.SiglaModalidade = produtocargaIntegracao.SiglaModalidade;
                pedidoProduto.AlturaCM = pedidoProduto.Produto.AlturaCM;
                pedidoProduto.LarguraCM = pedidoProduto.Produto.LarguraCM;
                pedidoProduto.ComprimentoCM = pedidoProduto.Produto.ComprimentoCM;
                pedidoProduto.QuantidadeCaixaPorPallet = pedidoProduto.Produto.QuantidadeCaixaPorPallet;
                pedidoProduto.QuantidadeCaixa = (produtocargaIntegracao.QuantidadePorCaixa > 0) ? produtocargaIntegracao.QuantidadePorCaixa : pedidoProduto.Produto.QuantidadeCaixa;
                pedidoProduto.QuantidadeCaixasVazias = produtocargaIntegracao.QuantidadeCaixasVazias;
                pedidoProduto.QuantidadeCaixasVaziasPlanejadas = produtocargaIntegracao.QuantidadeCaixasVaziasPlanejadas;
                pedidoProduto.LinhaSeparacao = pedidoProduto.Produto.LinhaSeparacao;
                pedidoProduto.EnderecoProduto = enderecoProduto;
                pedidoProduto.TipoEmbalagem = pedidoProduto.Produto.TipoEmbalagem;
                pedidoProduto.PalletFechado = pedidoProduto.PalletFechado;
                pedidoProduto.ImunoPlanejado = produtocargaIntegracao.Imuno;
                pedidoProduto.IdDemanda = produtocargaIntegracao.IdDemanda;
                pedidoProduto.CodigoOrganizacao = produtocargaIntegracao.CodigoOrganizacao;
                pedidoProduto.Setor = produtocargaIntegracao.Setor;
                pedidoProduto.Canal = produtocargaIntegracao.Canal;

                if (pedidoProduto.QuantidadePalet == 0 && pedidoProduto.QuantidadeCaixaPorPallet > 0 && pedidoProduto.QuantidadeCaixa > 0)
                {
                    decimal caixas = Math.Ceiling(pedidoProduto.Quantidade / pedidoProduto.QuantidadeCaixa);
                    pedidoProduto.QuantidadePalet = Math.Round((caixas / pedidoProduto.QuantidadeCaixaPorPallet), 4, MidpointRounding.ToEven);
                }

                if (habilitarCadastroArmazem)
                    VincularArmazemPedidoProduto(produtocargaIntegracao, pedidoProduto, repFilialArmazem, ref stMensagem);

                pedido.ValorTotalNotasFiscais += (pedidoProduto.ValorProduto * pedidoProduto.Quantidade);
                pedido.MaiorAlturaProdutoEmCentimetros = Math.Max(pedido.MaiorAlturaProdutoEmCentimetros, pedidoProduto.AlturaCM);
                pedido.MaiorLarguraProdutoEmCentimetros = Math.Max(pedido.MaiorLarguraProdutoEmCentimetros, pedidoProduto.LarguraCM);
                pedido.MaiorComprimentoProdutoEmCentimetros = Math.Max(pedido.MaiorComprimentoProdutoEmCentimetros, pedidoProduto.ComprimentoCM);
                pedido.MaiorVolumeProdutoEmCentimetros = Math.Max(pedido.MaiorVolumeProdutoEmCentimetros, (pedidoProduto.AlturaCM + pedidoProduto.LarguraCM + pedidoProduto.ComprimentoCM));

                //TODO: Ver ASSAI, os cabeças mandam a quantidade de caixas, sendo que precisamos da quantidade unitária, 
                // então vamos multiplicar o peso unitário pela quantidade por Caixa para manter o peso unitário da caixa
                // para que o processo de Roteriização evite de quebrar uma caixa em 2 carregamentos...
                // e o Peso da embalagem, estamos recebendo o peso total da caixa.. vamos igrnoprar
                if (configuracao.FatorMetroCubicoProdutoEmbarcadorIntegracao == 1000000)
                {
                    pedidoProduto.PesoUnitario = (pedidoProduto.Produto.PesoUnitario * pedidoProduto.Produto.QuantidadeCaixa);
                    pedidoProduto.PesoTotalEmbalagem = 0;
                }

                if (cubagemTotal == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                    pedido.CubagemTotal += pedidoProduto.MetroCubico;

                if (utilizarPesoProdutoParaCalcularPesoCarga)
                {
                    pedido.PesoTotal += pedidoProduto.PesoProduto;
                    pedido.PesoSaldoRestante += pedidoProduto.PesoProduto;
                    pedido.PesoLiquidoTotal += pedidoProduto.PesoLiquidoProduto;
                }
                else if (pesoBruto == 0 && configuracao.UsarPesoProdutoSumarizacaoCarga)
                {
                    pedido.PesoTotal += pedidoProduto.PesoTotal;
                    pedido.PesoSaldoRestante += pedidoProduto.PesoTotal;
                }

                if (numeroPaletesFracionado == 0)
                    pedido.NumeroPaletesFracionado += pedidoProduto.QuantidadePalet;

                repPedidoProduto.Inserir(pedidoProduto);

                if (produtocargaIntegracao.ProdutoLotes?.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Lote produtoLote in produtocargaIntegracao.ProdutoLotes)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote pedidoProdutoLote = new Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote();

                        pedidoProdutoLote.PedidoProduto = pedidoProduto;
                        pedidoProdutoLote.NumeroLote = produtoLote.NumeroLote;
                        pedidoProdutoLote.DataFabricacao = produtoLote.DataFabricacao;
                        pedidoProdutoLote.DataValidade = produtoLote.DataValidade;
                        pedidoProdutoLote.Quantidade = produtoLote.QuantidadeLote;

                        repositorioPedidoProdutoLote.Inserir(pedidoProdutoLote);
                    }
                }
            }

            if (atualizarPesoPedidoComPesoProduto)
            {
                pedido.PesoLiquidoTotal = produtosIntegracao.Sum(o => (o.PesoLiquidoUnitario * o.Quantidade));
                pedido.PesoTotal = produtosIntegracao.Sum(o => (o.PesoUnitario * o.Quantidade));
                pedido.QtVolumes = (int)Math.Round(produtosIntegracao.Sum(o => o.Quantidade));
            }

            repPedido.Atualizar(pedido);

            if (pedido.ValorTotalNotasFiscais > 0)
            {
                Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
                servicoMontagemCarga.AtualizarSituacaoExigeIscaPorPedido(pedido);
            }

            return produtosVinculados;
        }

        public void AdicionarProdutosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            AdicionarProdutosPedido(pedido, cargaIntegracao.Produtos, cargaIntegracao.ProdutoPredominante, cargaIntegracao.CubagemTotal, cargaIntegracao.PesoBruto, cargaIntegracao.ValorTotalPedido, cargaIntegracao.NumeroPaletesFracionado, configuracao, ref stMensagem, unitOfWork, auditado, atualizarPesoPedidoComPesoProduto: false);
        }

        public void AdicionarProdutosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoPredominante, decimal cubagemTotal, decimal pesoBruto, decimal valorTotalPedido, decimal numeroPaletesFracionado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool atualizarPesoPedidoComPesoProduto)
        {
            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProdutoLote repositorioPedidoProdutoLote = new Repositorio.Embarcador.Pedidos.PedidoProdutoLote(unitOfWork);
            Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarPrimeiroRegistro();
            bool utilizarPesoProdutoParaCalcularPesoCarga = configuracaoGeralCarga.UtilizarPesoProdutoParaCalcularPesoCarga;

            bool habilitarCadastroArmazem = configuracaoGeral?.HabilitarCadastroArmazem ?? false;

            if (configuracao.UtilizaEmissaoMultimodal && produtoPredominante != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();

                if (!string.IsNullOrWhiteSpace(produtoPredominante.CodigoProduto))
                {

                    List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
                    if (!configuracao.SempreInserirNovoProdutoPorIntegracao)
                    {
                        Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador prod = repProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoPredominante.CodigoProduto);
                        if (prod != null)
                            produtos.Add(prod);
                        //else
                        //      stMensagem.Append("Produto não informado ou não cadastrado no sistema; ");
                    }

                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = null;
                    if (!string.IsNullOrWhiteSpace(produtoPredominante.CodigoGrupoProduto))
                        grupoProduto = servicoProdutoEmbarcador.IntegrarGrupoProduto(produtoPredominante.CodigoGrupoProduto, produtoPredominante.DescricaoGrupoProduto);

                    Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = null;
                    if (produtoPredominante.MarcaProduto != null && !string.IsNullOrWhiteSpace(produtoPredominante.MarcaProduto.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtoPredominante.MarcaProduto.Descricao))
                        marcaProduto = servicoProdutoEmbarcador.IntegrarMarca(produtoPredominante.MarcaProduto.CodigoIntegracao, produtoPredominante.MarcaProduto.Descricao, auditado);


                    Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = null;
                    if (produtoPredominante.LinhaSeparacao != null && !string.IsNullOrWhiteSpace(produtoPredominante.LinhaSeparacao.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtoPredominante.LinhaSeparacao.Descricao))
                        linhaSeparacao = servicoProdutoEmbarcador.IntegrarLinhaSeparacao(produtoPredominante.LinhaSeparacao.CodigoIntegracao, produtoPredominante.LinhaSeparacao.Descricao, produtoPredominante.LinhaSeparacao.Roteiriza, produtoPredominante.LinhaSeparacao?.Filial?.CodigoIntegracao ?? "", produtoPredominante.LinhaSeparacao?.NivelPrioridade ?? 0, auditado);

                    if (linhaSeparacao != null && linhaSeparacao.Ativo == false)
                        stMensagem.Append("A linha de separação informada (" + linhaSeparacao.Descricao + ") está inativa.");
                    else
                    {
                        Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = null;
                        if (produtoPredominante.TipoEmbalagem != null && !string.IsNullOrWhiteSpace(produtoPredominante.TipoEmbalagem.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtoPredominante.TipoEmbalagem.Descricao))
                            tipoEmbalagem = servicoProdutoEmbarcador.IntegrarTipoEmbalagem(produtoPredominante.TipoEmbalagem.CodigoIntegracao, produtoPredominante.TipoEmbalagem.Descricao);

                        pedidoProduto.Pedido = pedido;
                        pedidoProduto.Produto = servicoProdutoEmbarcador.IntegrarProduto(produtos, configuracao, produtoPredominante.CodigoProduto, produtoPredominante.DescricaoProduto, produtoPredominante.PesoUnitario, grupoProduto, produtoPredominante.MetroCubito, auditado, produtoPredominante.CodigoDocumentacao, produtoPredominante.Atualizar, produtoPredominante.CodigoNCM, produtoPredominante.QuantidadePorCaixa, produtoPredominante.QuantidadeCaixaPorPallet, produtoPredominante.Altura, produtoPredominante.Largura, produtoPredominante.Comprimento, linhaSeparacao, tipoEmbalagem, marcaProduto, produtoPredominante.UnidadeMedida, produtoPredominante.Observacao, "", produtoPredominante?.CodigoEAN ?? string.Empty);
                        pedidoProduto.Observacao = pedidoProduto.Produto.Observacao;
                        pedidoProduto.ValorProduto = produtoPredominante.ValorUnitario;
                        pedidoProduto.PrecoUnitario = produtoPredominante.ValorUnitario;
                        pedidoProduto.QuantidadeEmbalagem = produtoPredominante.QuantidadeEmbalagem;
                        pedidoProduto.AlturaCM = pedidoProduto.Produto.AlturaCM;
                        pedidoProduto.LarguraCM = pedidoProduto.Produto.LarguraCM;
                        pedidoProduto.ComprimentoCM = pedidoProduto.Produto.ComprimentoCM;
                        pedidoProduto.MetroCubico = pedidoProduto.Produto.MetroCubito;
                        pedidoProduto.QuantidadeCaixaPorPallet = pedidoProduto.Produto.QuantidadeCaixaPorPallet;
                        pedidoProduto.ImunoPlanejado = produtoPredominante.Imuno;
                        pedidoProduto.QuantidadeCaixa = produtoPredominante.QuantidadePorCaixa > 0 ? produtoPredominante.QuantidadePorCaixa : pedidoProduto.Produto.QuantidadeCaixa;
                        pedidoProduto.QuantidadeCaixasVazias = produtoPredominante.QuantidadeCaixasVazias;
                        pedidoProduto.QuantidadeCaixasVaziasPlanejadas = produtoPredominante.QuantidadeCaixasVaziasPlanejadas;
                        pedidoProduto.LinhaSeparacao = pedidoProduto.Produto.LinhaSeparacao;
                        pedidoProduto.TipoEmbalagem = pedidoProduto.Produto.TipoEmbalagem;
                        pedidoProduto.Quantidade = produtoPredominante.Quantidade;
                        pedidoProduto.QuantidadePlanejada = produtoPredominante.QuantidadePlanejada;
                        pedidoProduto.PesoUnitario = pedidoProduto.Produto.PesoUnitario;
                        pedidoProduto.PesoTotalEmbalagem = produtoPredominante.PesoTotalEmbalagem;
                        pedidoProduto.CamposPersonalizados = pedidoProduto.CamposPersonalizados;
                        pedidoProduto.UnidadeMedidaSecundaria = pedidoProduto.UnidadeMedidaSecundaria;
                        pedidoProduto.QuantidadeSecundaria = pedidoProduto.QuantidadeSecundaria;

                        //TODO: Ver ASSAI, os cabeças mandam a quantidade de caixas, sendo que precisamos da quantidade unitária, 
                        // então vamos multiplicar o peso unitário pela quantidade por Caixa para manter o peso unitário da caixa
                        // para que o processo de Roteriização evite de quebrar uma caixa em 2 carregamentos...
                        // e o Peso da embalagem, estamos recebendo o peso total da caixa.. vamos igrnoprar
                        if (configuracao.FatorMetroCubicoProdutoEmbarcadorIntegracao == 1000000)
                        {
                            pedidoProduto.PesoUnitario = (pedidoProduto.Produto.PesoUnitario * pedidoProduto.Produto.QuantidadeCaixa);
                            pedidoProduto.PesoTotalEmbalagem = 0;
                        }
                        pedidoProduto.PalletFechado = produtoPredominante.PalletFechado;

                        if (habilitarCadastroArmazem)
                            VincularArmazemPedidoProduto(produtoPredominante, pedidoProduto, repFilialArmazem, ref stMensagem);

                        if (stMensagem.Length <= 0)
                        {
                            repPedidoProduto.Inserir(pedidoProduto);

                            if (produtosIntegracao?.Count > 0)
                            {
                                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto in produtosIntegracao)
                                {
                                    if (produto.ProdutoLotes != null)
                                    {
                                        foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Lote produtoLote in produto.ProdutoLotes)
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote pedidoProdutoLote = new Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote();

                                            pedidoProdutoLote.PedidoProduto = pedidoProduto;
                                            pedidoProdutoLote.NumeroLote = produtoLote.NumeroLote;
                                            pedidoProdutoLote.DataFabricacao = produtoLote.DataFabricacao;
                                            pedidoProdutoLote.DataValidade = produtoLote.DataValidade;
                                            pedidoProdutoLote.Quantidade = produtoLote.QuantidadeLote;

                                            repositorioPedidoProdutoLote.Inserir(pedidoProdutoLote);
                                        }
                                    }
                                }
                            }

                            pedido.MaiorAlturaProdutoEmCentimetros = Math.Max(pedido.MaiorAlturaProdutoEmCentimetros, pedidoProduto.AlturaCM);
                            pedido.MaiorLarguraProdutoEmCentimetros = Math.Max(pedido.MaiorLarguraProdutoEmCentimetros, pedidoProduto.LarguraCM);
                            pedido.MaiorComprimentoProdutoEmCentimetros = Math.Max(pedido.MaiorComprimentoProdutoEmCentimetros, pedidoProduto.ComprimentoCM);
                            pedido.MaiorVolumeProdutoEmCentimetros = Math.Max(pedido.MaiorVolumeProdutoEmCentimetros, (pedidoProduto.AlturaCM + pedidoProduto.LarguraCM + pedidoProduto.ComprimentoCM));

                            repPedido.Atualizar(pedido);
                        }
                    }
                }
                else
                {
                    stMensagem.Append("Produto não informado; ");
                }
            }
            else if (produtosIntegracao != null)
            {
                VincularProdutosAoPedido(pedido, configuracao, produtosIntegracao, cubagemTotal, pesoBruto, valorTotalPedido, numeroPaletesFracionado, utilizarPesoProdutoParaCalcularPesoCarga, configuracaoCargaIntegracao, ref stMensagem, unitOfWork, auditado, atualizarPesoPedidoComPesoProduto, habilitarCadastroArmazem);
            }
            else if (produtoPredominante != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();

                if (!string.IsNullOrWhiteSpace(produtoPredominante.CodigoProduto))
                {

                    List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
                    if (!configuracao.SempreInserirNovoProdutoPorIntegracao)
                    {
                        Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador prod = repProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoPredominante.CodigoProduto);
                        if (prod != null)
                            produtos.Add(prod);
                        //else
                        //      stMensagem.Append("Produto não informado ou não cadastrado no sistema; ");
                    }

                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = null;
                    if (!string.IsNullOrWhiteSpace(produtoPredominante.CodigoGrupoProduto))
                        grupoProduto = servicoProdutoEmbarcador.IntegrarGrupoProduto(produtoPredominante.CodigoGrupoProduto, produtoPredominante.DescricaoGrupoProduto);

                    Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = null;
                    if (produtoPredominante.MarcaProduto != null && !string.IsNullOrWhiteSpace(produtoPredominante.MarcaProduto.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtoPredominante.MarcaProduto.Descricao))
                        marcaProduto = servicoProdutoEmbarcador.IntegrarMarca(produtoPredominante.MarcaProduto.CodigoIntegracao, produtoPredominante.MarcaProduto.Descricao, auditado);

                    Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = null;
                    if (produtoPredominante.LinhaSeparacao != null && !string.IsNullOrWhiteSpace(produtoPredominante.LinhaSeparacao.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtoPredominante.LinhaSeparacao.Descricao))
                        linhaSeparacao = servicoProdutoEmbarcador.IntegrarLinhaSeparacao(produtoPredominante.LinhaSeparacao.CodigoIntegracao, produtoPredominante.LinhaSeparacao.Descricao, produtoPredominante.LinhaSeparacao.Roteiriza, produtoPredominante.LinhaSeparacao?.Filial?.CodigoIntegracao ?? "", produtoPredominante.LinhaSeparacao?.NivelPrioridade ?? 0, auditado);
                    if (linhaSeparacao != null && linhaSeparacao.Ativo == false)
                        stMensagem.Append("A linha de separação informada (" + linhaSeparacao.Descricao + ") está inativa.");
                    else
                    {
                        Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = null;
                        if (produtoPredominante.TipoEmbalagem != null && !string.IsNullOrWhiteSpace(produtoPredominante.TipoEmbalagem.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produtoPredominante.TipoEmbalagem.Descricao))
                            tipoEmbalagem = servicoProdutoEmbarcador.IntegrarTipoEmbalagem(produtoPredominante.TipoEmbalagem.CodigoIntegracao, produtoPredominante.TipoEmbalagem.Descricao);

                        pedidoProduto.Pedido = pedido;
                        pedidoProduto.Produto = servicoProdutoEmbarcador.IntegrarProduto(produtos, configuracao, produtoPredominante.CodigoProduto, produtoPredominante.DescricaoProduto, produtoPredominante.PesoUnitario, grupoProduto, produtoPredominante.MetroCubito, auditado, produtoPredominante.CodigoDocumentacao, produtoPredominante.Atualizar, produtoPredominante.CodigoNCM, produtoPredominante.QuantidadePorCaixa, produtoPredominante.QuantidadeCaixaPorPallet, produtoPredominante.Altura, produtoPredominante.Largura, produtoPredominante.Comprimento, linhaSeparacao, tipoEmbalagem, marcaProduto, produtoPredominante.UnidadeMedida, produtoPredominante.Observacao, "", produtoPredominante.CodigoEAN);
                        pedidoProduto.Observacao = pedidoProduto.Produto.Observacao;
                        pedidoProduto.ValorProduto = produtoPredominante.ValorUnitario;
                        pedidoProduto.PrecoUnitario = produtoPredominante.ValorUnitario;
                        pedidoProduto.QuantidadeEmbalagem = produtoPredominante.QuantidadeEmbalagem;
                        pedidoProduto.PesoTotalEmbalagem = produtoPredominante.PesoTotalEmbalagem;
                        pedidoProduto.Quantidade = produtoPredominante.Quantidade;
                        pedidoProduto.QuantidadePlanejada = produtoPredominante.QuantidadePlanejada;
                        pedidoProduto.PesoUnitario = pedidoProduto.Produto.PesoUnitario;
                        pedidoProduto.PalletFechado = produtoPredominante.PalletFechado;
                        pedidoProduto.AlturaCM = pedidoProduto.Produto.AlturaCM;
                        pedidoProduto.LarguraCM = pedidoProduto.Produto.LarguraCM;
                        pedidoProduto.ComprimentoCM = pedidoProduto.Produto.ComprimentoCM;
                        pedidoProduto.MetroCubico = pedidoProduto.Produto.MetroCubito;
                        pedidoProduto.QuantidadeCaixaPorPallet = pedidoProduto.Produto.QuantidadeCaixaPorPallet;
                        pedidoProduto.QuantidadeCaixa = produtoPredominante.QuantidadePorCaixa > 0 ? produtoPredominante.QuantidadePorCaixa : pedidoProduto.Produto.QuantidadeCaixa;
                        pedidoProduto.QuantidadeCaixasVazias = produtoPredominante.QuantidadeCaixasVazias;
                        pedidoProduto.QuantidadeCaixasVaziasPlanejadas = produtoPredominante.QuantidadeCaixasVaziasPlanejadas;
                        pedidoProduto.LinhaSeparacao = pedidoProduto.Produto.LinhaSeparacao;
                        pedidoProduto.TipoEmbalagem = pedidoProduto.Produto.TipoEmbalagem;
                        pedidoProduto.CamposPersonalizados = pedidoProduto.CamposPersonalizados;
                        pedidoProduto.UnidadeMedidaSecundaria = pedidoProduto.UnidadeMedidaSecundaria;
                        pedidoProduto.QuantidadeSecundaria = pedidoProduto.QuantidadeSecundaria;

                        if (habilitarCadastroArmazem)
                            VincularArmazemPedidoProduto(produtoPredominante, pedidoProduto, repFilialArmazem, ref stMensagem);

                        if (stMensagem.Length <= 0)
                        {
                            repPedidoProduto.Inserir(pedidoProduto);

                            pedido.MaiorAlturaProdutoEmCentimetros = Math.Max(pedido.MaiorAlturaProdutoEmCentimetros, pedidoProduto.AlturaCM);
                            pedido.MaiorLarguraProdutoEmCentimetros = Math.Max(pedido.MaiorLarguraProdutoEmCentimetros, pedidoProduto.LarguraCM);
                            pedido.MaiorComprimentoProdutoEmCentimetros = Math.Max(pedido.MaiorComprimentoProdutoEmCentimetros, pedidoProduto.ComprimentoCM);
                            pedido.MaiorVolumeProdutoEmCentimetros = Math.Max(pedido.MaiorVolumeProdutoEmCentimetros, (pedidoProduto.AlturaCM + pedidoProduto.LarguraCM + pedidoProduto.ComprimentoCM));

                            repPedido.Atualizar(pedido);
                        }
                    }
                }
                else
                {
                    stMensagem.Append("Produto não informado; ");
                }
            }

            CalcularConversaoPedidoPaletizado(pedido, unitOfWork);
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.Produto ConveterObjetoProduto(Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto)
        {
            if (cargaPedidoProduto != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                produtoIntegracao.CodigoProduto = cargaPedidoProduto.Produto.CodigoProdutoEmbarcador;

                if (cargaPedidoProduto.Produto.GrupoProduto != null)
                {
                    produtoIntegracao.CodigoGrupoProduto = cargaPedidoProduto.Produto.GrupoProduto.CodigoGrupoProdutoEmbarcador;
                    produtoIntegracao.DescricaoGrupoProduto = cargaPedidoProduto.Produto.GrupoProduto.Descricao;
                }
                produtoIntegracao.DescricaoProduto = cargaPedidoProduto.Produto.Descricao;
                produtoIntegracao.PesoTotalEmbalagem = cargaPedidoProduto.PesoTotalEmbalagem;
                produtoIntegracao.PesoUnitario = cargaPedidoProduto.PesoUnitario;
                produtoIntegracao.Quantidade = cargaPedidoProduto.Quantidade;
                produtoIntegracao.QuantidadePlanejada = cargaPedidoProduto.QuantidadePlanejada;
                produtoIntegracao.QuantidadeEmbalagem = cargaPedidoProduto.QuantidadeEmbalagem;
                produtoIntegracao.ValorUnitario = cargaPedidoProduto.ValorUnitarioProduto;
                produtoIntegracao.CodigoDocumentacao = cargaPedidoProduto.Produto.CodigoDocumentacao;
                produtoIntegracao.MetroCubito = cargaPedidoProduto.Produto.MetroCubito;


                return produtoIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.Produto ConveterObjetoProduto(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador)
        {
            if (produtoEmbarcador != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                produtoIntegracao.CodigoProduto = produtoEmbarcador.CodigoProdutoEmbarcador;
                produtoIntegracao.Codigo = produtoEmbarcador.Codigo;
                produtoIntegracao.Atualizar = true;

                if (produtoEmbarcador.GrupoProduto != null)
                {
                    produtoIntegracao.CodigoGrupoProduto = produtoEmbarcador.GrupoProduto.CodigoGrupoProdutoEmbarcador;
                    produtoIntegracao.DescricaoGrupoProduto = produtoEmbarcador.GrupoProduto.Descricao;
                }
                produtoIntegracao.DescricaoProduto = produtoEmbarcador.Descricao;
                produtoIntegracao.PesoUnitario = produtoEmbarcador.PesoUnitario;
                produtoIntegracao.Quantidade = produtoEmbarcador.QuantidadeCaixa;
                produtoIntegracao.CodigoDocumentacao = produtoEmbarcador.CodigoDocumentacao;
                produtoIntegracao.MetroCubito = produtoEmbarcador.MetroCubito;
                produtoIntegracao.InativarCadastro = produtoEmbarcador.Ativo ? false : true;
                produtoIntegracao.CodigocEAN = produtoEmbarcador.CodigoCEAN;
                produtoIntegracao.CodigoGrupoProduto = produtoEmbarcador.GrupoPessoas?.Codigo.ToString("D") ?? "0";
                produtoIntegracao.CodigoNCM = produtoEmbarcador.CodigoNCM;

                produtoIntegracao.SiglaUnidade = produtoEmbarcador.SiglaUnidade;
                produtoIntegracao.TemperaturaTransporte = produtoEmbarcador.TemperaturaTransporte;
                produtoIntegracao.PesoLiquidoUnitario = produtoEmbarcador.PesoLiquidoUnitario;
                produtoIntegracao.QtdPalet = produtoEmbarcador.QtdPalet;
                produtoIntegracao.AlturaCM = produtoEmbarcador.AlturaCM;
                produtoIntegracao.LarguraCM = produtoEmbarcador.LarguraCM;
                produtoIntegracao.ComprimentoCM = produtoEmbarcador.ComprimentoCM;
                produtoIntegracao.Observacao = produtoEmbarcador.Observacao;
                produtoIntegracao.QuantidadeCaixa = produtoEmbarcador.QuantidadeCaixa;
                produtoIntegracao.QuantidadeCaixaPorPallet = produtoEmbarcador.QuantidadeCaixaPorPallet;

                return produtoIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.Produto ConveterObjetoProduto(Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto)
        {
            if (pedidoProduto != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                produtoIntegracao.CodigoProduto = pedidoProduto.Produto.CodigoProdutoEmbarcador;

                if (pedidoProduto.Produto.GrupoProduto != null)
                {
                    produtoIntegracao.CodigoGrupoProduto = pedidoProduto.Produto.GrupoProduto.CodigoGrupoProdutoEmbarcador;
                    produtoIntegracao.DescricaoGrupoProduto = pedidoProduto.Produto.GrupoProduto.Descricao;
                }
                produtoIntegracao.DescricaoProduto = pedidoProduto.Produto.Descricao;
                produtoIntegracao.PesoTotalEmbalagem = pedidoProduto.PesoTotalEmbalagem;
                produtoIntegracao.PesoUnitario = pedidoProduto.PesoUnitario;
                produtoIntegracao.Quantidade = pedidoProduto.Quantidade;
                produtoIntegracao.QuantidadePlanejada = pedidoProduto.QuantidadePlanejada;
                produtoIntegracao.QuantidadeEmbalagem = pedidoProduto.QuantidadeEmbalagem;
                produtoIntegracao.ValorUnitario = pedidoProduto.ValorProduto;
                produtoIntegracao.CodigoDocumentacao = pedidoProduto.Produto.CodigoDocumentacao;
                produtoIntegracao.MetroCubito = pedidoProduto.Produto.MetroCubito;
                produtoIntegracao.Observacao = pedidoProduto.Observacao;
                produtoIntegracao.CodigoEAN = pedidoProduto.Produto.CodigoEAN;
                produtoIntegracao.CodigocEAN = pedidoProduto.Produto.CodigoCEAN;

                return produtoIntegracao;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Métodos Privados

        private void VincularArmazemPedidoProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto, Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto, Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazem, ref StringBuilder stMensagem)
        {
            if (!string.IsNullOrEmpty(produto.CodigoArmazem))
            {
                Dominio.Entidades.Embarcador.Filiais.FilialArmazem filialArmazem = repFilialArmazem.BuscarPorCodigoIntegracao(produto.CodigoArmazem);
                if (filialArmazem == null)
                    stMensagem.Append("Não foi encontrado um armazém para o código de integração " + produto.CodigoArmazem + " na base da Multisoftware");
                else
                    pedidoProduto.FilialArmazem = filialArmazem;
            }
            else
                stMensagem.Append("É necessário informar o código de armazém do produto. Código do produto: " + produto.CodigoProduto);
        }

        private void CalcularConversaoPedidoPaletizado(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork _unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
            if (configuracaoPedido == null)
                return;

            Repositorio.Embarcador.Pedidos.PedidoAdicional repPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repPedidoAdicional.BuscarPorPedido(pedido.Codigo);

            if (pedidoAdicional == null)
                return;

            if (!(pedidoAdicional.PedidoPaletizado ?? false))
                return;

            Repositorio.Embarcador.Pedidos.PedidoProduto repProdutoPedido = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repProdutoPedido.BuscarPorPedido(pedido.Codigo);
            if (pedidoProdutos.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedidoProdutos)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao prodTabelaConversao = pedidoProduto.Produto.TabelaConversao.FirstOrDefault();
                if (prodTabelaConversao == null)
                    continue;

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProduto.BuscarPorCodigo(pedidoProduto.Produto.Codigo);
                if (produto == null)
                    continue;

                pedidoProduto.MetroCubico = pedidoProduto.Quantidade * prodTabelaConversao.QuantidadePara;
                pedidoProduto.QuantidadePalet = produto.QuantidadeCaixaPorPallet > 0 ? pedidoProduto.Quantidade / produto.QuantidadeCaixaPorPallet : 0;

                repProdutoPedido.Atualizar(pedidoProduto);
            }

            pedido.CubagemTotal = CalcularCubagemTotalProdutos(pedidoProdutos);
            pedido.NumeroPaletesFracionado = CalcularPalletsTotalProdutos(pedidoProdutos);
        }

        private decimal CalcularPalletsTotalProdutos(List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos)
        {
            decimal total = 0;
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in pedidoProdutos)
            {
                total += produto.QuantidadePalet;
            }
            return total;
        }

        private decimal CalcularCubagemTotalProdutos(List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos)
        {
            decimal total = 0;
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produto in pedidoProdutos)
            {
                total += produto.MetroCubico;
            }
            return total;
        }

        #endregion
    }
}