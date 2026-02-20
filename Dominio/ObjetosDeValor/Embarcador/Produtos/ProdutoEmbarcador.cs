namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class ProdutoEmbarcador
    {
        public int Codigo { get; set; }
        public string CodigoProdutoEmbarcador { get; set; }
        public string CodigoDocumentacao { get; set; }
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public string CodigoCEAN { get; set; }
        public int GrupoProduto { get; set; }
        public bool Ativo { get; set; }
        public bool? Integrado { get; set; }
        public int TipoPessoa { get; set; }
        public int GrupoPessoas { get; set; }
        public int CheckList { get; set; }
        public double Cliente { get; set; }
        public int TipoDeCarga { get; set; }
        public string TemperaturaTransporte { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal FatorConversao { get; set; }
        public decimal PesoLiquidoUnitario { get; set; }
        public string SiglaUnidade { get; set; }
        public bool ExibirExpedicaoEmTempoReal { get; set; }
        public bool DescontarPesoProdutoCalculoFrete { get; set; }
        public bool DescontarValorProdutoCalculoFrete { get; set; }
        public bool ObrigatorioGuiaTransporteAnimal { get; set; }
        public bool ObrigatorioNFProdutor { get; set; }
        public int QuantidadeCaixa { get; set; }
        public int QuantidadeCaixaPorPallet { get; set; }
        public decimal QtdPalet { get; set; }
        public decimal AlturaCM { get; set; }
        public decimal LarguraCM { get; set; }
        public decimal ComprimentoCM { get; set; }
        public decimal MetroCubito { get; set; }
        public bool PossuiIntegracaoColetaMobile { get; set; }
        public bool ObrigatorioInformarTemperatura { get; set; }
        public string CodigoNCM { get; set; }
        public bool ExigeInformarCaixas { get; set; }
        public bool ExigeInformarImunos { get; set; }
        public string CodigoEAN { get; set; }

        public string ChaveNF { get; set; }
    }
}
