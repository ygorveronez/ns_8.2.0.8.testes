namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class InformarDocaSalvarDadosTransporteCarga
    {
        public int CodigoCarga { get; set; }

        public int CodigoLocalCarregamento { get; set; }

        public string NumeroDoca { get; set; }

        public bool PossuiLaudo { get; set; }
    }
}