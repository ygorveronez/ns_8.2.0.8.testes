using System.Collections.Generic;
using Dominio.ObjetosDeValor.WebService.Pedido;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class Produto
    {
        public int Codigo { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public string CodigoGrupoProduto { get; set; }
        public string CodigocEAN { get; set; }
        public string DescricaoGrupoProduto { get; set; }
        public string NumeroPedidoCompra { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal Quantidade { get; set; }
        public decimal QuantidadePlanejada { get; set; }
        public string UnidadeMedida { get; set; }
        public string UnidadeMedidaPeso { get; set; }
        public decimal QuantidadeEmbalagem { get; set; }
        public decimal PesoTotalEmbalagem { get; set; }
        public decimal MetroCubito { get; set; }
        public string CodigoDocumentacao { get; set; }

        /// <summary>
        /// Essa é a observação do Produto em si. É usado para criar/editar a observação do ProdutoEmbarcador.
        /// Não confundir com a ObservacaoCarga abaixo.
        /// </summary>
        public string Observacao { get; set; }

        /// <summary>
        /// Essa observação é a observação desse produto especificamente para a carga que está sendo criada na chamada do método AdicionarCarga.
        /// </summary>
        public string ObservacaoCarga { get; set; }

        public bool InativarCadastro { get; set; }
        public bool Atualizar { get; set; }
        public string SetorLogistica { get; set; }
        public string ClasseLogistica { get; set; }
        public string CodigoNCM { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Lote> ProdutoLotes { get; set; }
        public List<ProdutoDivisaoCapacidade> ProdutoDivisoesCapacidade { get; set; }
        public decimal QuantidadePallet { get; set; }
        public int QuantidadePorCaixa { get; set; }
        public int QuantidadeCaixasVazias { get; set; }
        public int QuantidadeCaixasVaziasPlanejadas { get; set; }
        public int QuantidadeCaixaPorPallet { get; set; }
        public decimal Lastro { get; set; }
        public decimal Camada { get; set; }
        public decimal Altura { get; set; }
        public decimal Largura { get; set; }
        public decimal Comprimento { get; set; }
        public LinhaSeparacao LinhaSeparacao { get; set; }
        public Produtos.EnderecoProduto EnderecoProduto { get; set; }
        public MarcaProduto MarcaProduto { get; set; }
        public TipoEmbalagem TipoEmbalagem { get; set; }
        public bool PalletFechado { get; set; }
        public string CSTICMS { get; set; }
        public string OrigemMercadoria { get; set; }
        public string CodigoNFCI { get; set; }
        public string CodigoEAN { get; set; }
        public string CanalDistribuicao { get; set; }
        public string SiglaModalidade { get; set; }
        public int? Imuno { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDesconto { get; set; }
        public FinalidadeProduto FinalidadeProduto { get; set; }
        public string IdDemanda { get; set; }

        public string SiglaUnidade { get; set; }
        public string TemperaturaTransporte { get; set; }
        public decimal PesoLiquidoUnitario { get; set; }
        public decimal QtdPalet { get; set; }
        public decimal AlturaCM { get; set; }
        public decimal LarguraCM { get; set; }
        public decimal ComprimentoCM { get; set; }
        public int QuantidadeCaixa { get; set; }
        public string CodigoOrganizacao { get; set; }
        public string Canal { get; set; }
        public string Setor { get; set; }
        public List<Organizacao> Organizacao { get; set; }
        public List<Filiais> Filiais { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Produtos.Conversao> Conversao { get; set; }
        public bool ProdutoSimulado { get; set; }
        public string CodigoArmazem { get; set; }
        public string CamposPersonalizados { get; set; }
        public bool Mesclar { get; set; }
        public string UnidadeMedidaSecundaria { get; set; }
        public decimal QuantidadeSecundaria { get; set; }

    }
}
