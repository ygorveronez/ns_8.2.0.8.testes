namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public sealed class CargaJanelaCarregamentoTransportadorValorFrete
    {
        public Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador CargaJanelaCarregamentoTransportador { get; set; }

        public decimal ValorComponentesFrete { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorTotalFrete
        {
            get
            {
                return ValorFrete + ValorComponentesFrete;
            }
        }
    }
}
