namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ConsultaRota
    {
        public string codigoCliente { get; set; }
        public string nomeRota { get; set; }
        public ContratoFreteOrigemEndereco origem { get; set; }
        public ContratoFreteOrigemEndereco destino { get; set; }
    }
}
