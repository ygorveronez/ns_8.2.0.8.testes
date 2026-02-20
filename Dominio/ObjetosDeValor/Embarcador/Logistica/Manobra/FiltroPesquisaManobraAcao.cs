namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaManobraAcao
    {
        public int CodigoCentroCarregamento { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.TipoManobraAcao? Tipo { get; set; }
    }
}
