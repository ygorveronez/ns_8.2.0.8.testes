namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class InformarDocaAvancarEtapa
    {
        public int Codigo { get; set; }

        public int CodigoLocalCarregamento { get; set; }

        public string NumeroDoca { get; set; }

        public bool PossuiLaudo { get; set; }
        public bool EtapaAntecipada { get; set; }
    }
}
