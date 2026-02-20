namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class Proprietario
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa TransportadorTerceiro { get; set; }
        public Dominio.Enumeradores.TipoProprietarioVeiculo TipoTACVeiculo { get; set; }
        public string CIOT { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa FornecedorValePedagio { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ResponsavelValePedagio { get; set; }
        public string NumeroCompraValePedagio { get; set; }
        public decimal ValorValePedagio { get; set; }

    }
}
