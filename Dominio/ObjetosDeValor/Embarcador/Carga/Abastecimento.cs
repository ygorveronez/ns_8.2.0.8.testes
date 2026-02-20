namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Abastecimento
    {
        public string PlacaVeiculo { get; set; }
        public string CPFMotorista { get; set; }
        public string CNPJPosto { get; set; }
        public string CodigoProduto { get; set; }
        public int KM { get; set; }
        public decimal Litros { get; set; }
        public decimal ValorUnitario { get; set; }
        public string Data { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
