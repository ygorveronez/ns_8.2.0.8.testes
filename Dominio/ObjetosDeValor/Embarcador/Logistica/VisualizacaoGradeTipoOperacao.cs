namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class VisualizacaoGradeTipoOperacao
    {
        public int Periodo { get; set; }

        public int TipoOperacao { get; set; }

        public string Descricao { get; set; }

        public int Capacidade { get; set; }

        public int Ocupadas { get; set; }

        public int Total { get; set; }

        //public int Total => Capacidade - Ocupadas;
    }
}
