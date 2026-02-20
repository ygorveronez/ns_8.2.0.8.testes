namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    /// <summary>
    /// Uma janela de tempo com dados de início / fim.
    /// </summary>
    public class TimeWindow
    {
        public TimeWindow()
        {
            this.start = -1;
            this.end = -1;
            this.time = -1;
        }

        public TimeWindow(int start, int end, int time)
        {
            this.start = start;
            this.end = end;
            this.time = time;
        }

        //janela.ini.Hour* 60 + janela.ini.Minute
        /// <summary>
        /// Inicio da janena em minutos..
        /// </summary>
        public int start;
        /// <summary>
        /// Fim da janela em minutos
        /// </summary>
        public int end;
        /// <summary>
        /// Tempo de atendimento em minutos...
        /// </summary>
        public int time;
    }
}
