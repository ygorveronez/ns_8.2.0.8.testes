namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Frota
{
    public sealed class Proprietario
    {
        public Pessoas.Empresa TransportadorTerceiro { get; set; }

        public Dominio.Enumeradores.TipoProprietarioVeiculo TipoTACVeiculo { get; set; }

        public string CIOT { get; set; }

        public Pessoas.Pessoa FornecedorValePedagio { get; set; }

        public Pessoas.Pessoa ResponsavelValePedagio { get; set; }

        public string NumeroCompraValePedagio { get; set; }

        public decimal ValorValePedagio { get; set; }
    }
}
