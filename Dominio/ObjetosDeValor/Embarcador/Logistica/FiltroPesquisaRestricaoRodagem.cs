namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRestricaoRodagem
    {
        public int CodigoCentroCarregamento { get; set; }

        public Enumeradores.DiaSemana? DiaSemana { get; set; }

        public int? FinalPlaca { get; set; }
    }
}
