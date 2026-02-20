namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class FluxoGestaoPatioDadosReiniciar
    {
        public Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        public int CodigoMotivoRetiradaFilaCarregamento { get; set; }

        public string Motivo { get; set; }

        public bool RemoverDadosTransporte { get; set; }
        
        public bool RemoverVeiculoFilaCarregamento { get; set; }
    }
}
