namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFreteVeiculos
    {
        public ContratoFreteVeiculosCavalo cavalo { get; set; }
        public ContratoFreteVeiculosCarretas[] carretas { get; set; }

        public string numeroViagemCliente;
        public string operador;
    }
}
