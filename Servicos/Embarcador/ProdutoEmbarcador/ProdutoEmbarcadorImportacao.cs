using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.ProdutoEmbarcador
{
    public sealed class ProdutoEmbarcadorImportacao
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> _gruposProdutoExistentes;
        private readonly List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> _produtosEmbarcadorExistentes;

        #endregion

        #region Construtores

        public ProdutoEmbarcadorImportacao(Repositorio.UnitOfWork unitOfWork, Dictionary<string, dynamic> dados, List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProdutoExistentes, List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcadorExistentes)
        {
            _dados = dados;
            _unitOfWork = unitOfWork;
            _gruposProdutoExistentes = gruposProdutoExistentes;
            _produtosEmbarcadorExistentes = produtosEmbarcadorExistentes;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ObterProdutoEmbarcador()
        {
            var codigoProdutoEmbarcadorBuscar = string.Empty;

            if (_dados.TryGetValue("CodigoProdutoEmbarcador", out var codigoProdutoEmbarcador))
                codigoProdutoEmbarcadorBuscar = (string)codigoProdutoEmbarcador;

            if (string.IsNullOrWhiteSpace(codigoProdutoEmbarcadorBuscar))
                throw new ImportacaoException("Código de Integração não informado");

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = _produtosEmbarcadorExistentes.Find(o => o.CodigoProdutoEmbarcador == codigoProdutoEmbarcadorBuscar);

            if (produto == null)
            {
                produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();
                produto.CodigoProdutoEmbarcador = codigoProdutoEmbarcadorBuscar;
            }
            else
                produto.Initialize();

            return produto;
        }

        private string ObterDescricao()
        {
            var descricaoRetornar = string.Empty;

            if (_dados.TryGetValue("Descricao", out var descricao))
                descricaoRetornar = (string)descricao;

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ImportacaoException("Descrição não informada.");

            return descricaoRetornar.Trim();
        }

        private string ObterTemperaturaTransporte(string valor)
        {
            var temperaturaTransporteRetornar = valor;

            if (_dados.TryGetValue("TemperaturaTransporte", out var temperaturaTransporte))
                temperaturaTransporteRetornar = (string)temperaturaTransporte;

            return temperaturaTransporteRetornar;
        }

        private Dominio.Entidades.Embarcador.Produtos.GrupoProduto ObterGrupoProduto()
        {
            var codigoIntegracaoBuscar = string.Empty;

            if (_dados.TryGetValue("GrupoProduto", out var codigoIntegracao))
                codigoIntegracaoBuscar = (string)codigoIntegracao;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoBuscar))
                throw new ImportacaoException("Grupo de Produto não informado");

            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = _gruposProdutoExistentes.Find(o => o.CodigoGrupoProdutoEmbarcador == codigoIntegracaoBuscar.Trim());

            if (grupoProduto == null)
                throw new ImportacaoException("Grupo de Produto não encontrado");

            return grupoProduto;
        }

        private int ObterQuantidadeCaixa(int valor)
        {
            int quantidadePorCaixaRetornar = valor;

            if (_dados.TryGetValue("QuantidadePorCaixa", out var quantidadePorCaixa))
                quantidadePorCaixaRetornar = ((string)quantidadePorCaixa).ToInt();

            return quantidadePorCaixaRetornar;
        }

        private string ObterCodigoDocumentacao(string valor)
        {
            var codigoDocumentacaoRetornar = valor;

            if (_dados.TryGetValue("CodigoDocumentacao", out var codigoDocumentacao))
                codigoDocumentacaoRetornar = (string)codigoDocumentacao;

            return codigoDocumentacaoRetornar;
        }

        private decimal ObterPesoUnitario(decimal valor)
        {
            decimal pesoUnitarioRetornar = valor;

            if (_dados.TryGetValue("PesoUnitario", out var pesoUnitario))
                pesoUnitarioRetornar = ((string)pesoUnitario).ToDecimal();

            return pesoUnitarioRetornar;
        }

        private int ObterQuantidadeCaixaPorPallet(int valor)
        {
            int quantidadeCaixaPorPalletRetornar = valor;

            if (_dados.TryGetValue("QuantidadeCaixaPorPallet", out var quantidadeCaixaPorPallet))
                quantidadeCaixaPorPalletRetornar = ((string)quantidadeCaixaPorPallet).ToInt();

            return quantidadeCaixaPorPalletRetornar;
        }

        private decimal ObterAlturaCM(decimal valor)
        {
            decimal alturaCMRetornar = valor;

            if (_dados.TryGetValue("AlturaCM", out var alturaCM))
                alturaCMRetornar = ((string)alturaCM).ToDecimal();

            return alturaCMRetornar;
        }

        private decimal ObterLarguraCM(decimal valor)
        {
            decimal larguraCMRetornar = valor;

            if (_dados.TryGetValue("LarguraCM", out var larguraCM))
                larguraCMRetornar = ((string)larguraCM).ToDecimal();

            return larguraCMRetornar;
        }

        private decimal ObterComprimentoCM(decimal valor)
        {
            decimal comprimentoCMRetornar = valor;

            if (_dados.TryGetValue("ComprimentoCM", out var comprimentoCM))
                comprimentoCMRetornar = ((string)comprimentoCM).ToDecimal();

            return comprimentoCMRetornar;
        }

        private decimal ObterMetroCubico(decimal valor)
        {
            decimal metroCubicoRetornar = valor;

            if (_dados.TryGetValue("MetroCubico", out var metroCubico))
                metroCubicoRetornar = ((string)metroCubico).ToDecimal();

            return metroCubicoRetornar;
        }

        private string ObterCodigoNCM(string valor)
        {
            var codigoNCMRetornar = valor;

            if (_dados.TryGetValue("CodigoNCM", out var codigoNCM))
                codigoNCMRetornar = (string)codigoNCM;

            return codigoNCMRetornar;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ObterProdutoEmbarcadorImportar()
        {
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = ObterProdutoEmbarcador();

            produtoEmbarcador.Ativo = true;
            produtoEmbarcador.Descricao = ObterDescricao();
            produtoEmbarcador.GrupoProduto = ObterGrupoProduto();
            produtoEmbarcador.TemperaturaTransporte = ObterTemperaturaTransporte(produtoEmbarcador.TemperaturaTransporte);
            produtoEmbarcador.QuantidadeCaixa = ObterQuantidadeCaixa(produtoEmbarcador.QuantidadeCaixa);
            produtoEmbarcador.CodigoDocumentacao = ObterCodigoDocumentacao(produtoEmbarcador.CodigoDocumentacao);
            produtoEmbarcador.PesoUnitario = ObterPesoUnitario(produtoEmbarcador.PesoUnitario);
            produtoEmbarcador.QuantidadeCaixaPorPallet = ObterQuantidadeCaixaPorPallet(produtoEmbarcador.QuantidadeCaixaPorPallet);
            produtoEmbarcador.AlturaCM = ObterAlturaCM(produtoEmbarcador.AlturaCM);
            produtoEmbarcador.LarguraCM = ObterLarguraCM(produtoEmbarcador.LarguraCM);
            produtoEmbarcador.ComprimentoCM = ObterComprimentoCM(produtoEmbarcador.ComprimentoCM);
            produtoEmbarcador.MetroCubito = ObterMetroCubico(produtoEmbarcador.MetroCubito);
            produtoEmbarcador.CodigoNCM = ObterCodigoNCM(produtoEmbarcador.CodigoNCM);
            produtoEmbarcador.Integrado = false;

            return produtoEmbarcador;
        }

        #endregion
    }
}
