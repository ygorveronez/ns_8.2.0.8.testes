namespace Dominio.ObjetosDeValor.WebService.Rest
{
    public class DadosTransporteCarga
    {
        public string NumeroCarga { get; set; }
        public Veiculo VeiculoTracao { get; set; }
        public Veiculo VeiculoReboque1 { get; set; }
        public Veiculo VeiculoReboque2 { get; set; }
        public Motorista Motorista { get; set; }
        public Motorista MotoristaAdicional { get; set; }
        public string TipoCheckin { get; set; }
        public string DataCheckout { get; set; }
    }
}
