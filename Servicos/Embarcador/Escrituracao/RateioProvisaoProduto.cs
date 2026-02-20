using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public class RateioProvisaoProduto
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public RateioProvisaoProduto(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion Construtores  

        #region Métodos Públicos

        public void ReatearPorGrupoDeProduto(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanciro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if (!(configuracaoFinanciro.RateioProvisaoPorGrupoProduto ?? false) || provisao.DocumentosProvisao.Count == 0)
                return;

            Repositorio.Embarcador.Rateio.RateioProvisaoProduto repositorioRateioProvisaoProduto = new Repositorio.Embarcador.Rateio.RateioProvisaoProduto(_unitOfWork);

            if (repositorioRateioProvisaoProduto.ExistePorProvisao(provisao))
                return;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = provisao.DocumentosProvisao.FirstOrDefault();

            if (documentoProvisao == null)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repositorioPedidoXMLNotaFiscal.BuscarProdutosPorXMLNotaFiscal(documentoProvisao.XMLNotaFiscal.Codigo, documentoProvisao.Carga.Codigo);

            decimal pesoTotalProdutos = pedidoProdutos.Sum(x => x.PesoTotal);
            if (pesoTotalProdutos == 0)
                return;

            decimal valorParaRateio = provisao.ValorProvisao / pesoTotalProdutos;

            var produtosProGrupo = pedidoProdutos
                .GroupBy(x => x.Produto.GrupoProduto)
                .Select(group => new
                {
                    Grupo = group.Key,
                    Produtos = group.ToList(),
                    PesoTotal = group.Sum(x => x.PesoTotal),
                    QuantidadeTotal = group.Sum(x => x.Quantidade)
                });

            List<Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto> rateioProvisaoProdutos = produtosProGrupo
                .Select(group => new Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto
                {
                    Produtos = group.Produtos,
                    GrupoProduto = group.Grupo,
                    PesoTotalProdutos = group.PesoTotal,
                    Provisao = provisao,
                    QuantidadeTotalProdutos = group.QuantidadeTotal,
                    ValorTotalRateio = Math.Floor(valorParaRateio * group.PesoTotal * 100) / 100
                }).ToList();

            decimal somaValorTotalRateio = rateioProvisaoProdutos.Sum(x => x.ValorTotalRateio);
            decimal diferenca = provisao.ValorProvisao - somaValorTotalRateio;

            if (diferenca != 0)
            {
                var ultimoItem = rateioProvisaoProdutos.LastOrDefault();
                if (ultimoItem != null)
                {
                    ultimoItem.ValorTotalRateio += diferenca;
                }
            }

            foreach (var rateioProvisaoProduto in rateioProvisaoProdutos)
                repositorioRateioProvisaoProduto.Inserir(rateioProvisaoProduto);
        }


        #endregion
    }
}
