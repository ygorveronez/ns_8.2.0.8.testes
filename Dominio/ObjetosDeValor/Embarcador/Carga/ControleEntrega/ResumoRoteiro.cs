using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class ResumoRoteiro
    {
        public string Etapa { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public int Total { get; set; }
        public decimal Freetime { get; set; }
        public decimal Exedente { get; set; }

        public decimal ValorHora { get; set; }
        public decimal ValorTotal { get; set; }

        public string InicioFormatada
        { get { return Inicio != DateTime.MinValue ? Inicio.ToString("dd/MM/yyyy HH:mm") : string.Empty; } }

        public string FimFormatada
        { get { return Fim != DateTime.MinValue ? Fim.ToString("dd/MM/yyyy HH:mm") : string.Empty; } }

        public TimeSpan TotalHoras
        {
            get
            {
                return TimeSpan.FromMinutes((double)this.Total);
            }
        }

        public TimeSpan FreetimeHoras
        {
            get
            {
                return TimeSpan.FromMinutes((double)this.Freetime);
            }
        }

        public TimeSpan ExedenteHoras
        {
            get
            {
                return TimeSpan.FromMinutes((double)this.Exedente);
            }
        }

        public string TotalHorasFormatado
        {
            get
            {
                return $"{(int)TotalHoras.TotalHours:d3}:{TotalHoras:mm}";
            }
        }

        public string FreetimeHorasFormatado
        {
            get
            {
                return $"{(int)FreetimeHoras.TotalHours:d3}:{FreetimeHoras:mm}";
            }
        }

        public string ExedenteHorasFormatado
        {
            get
            {
                return $"{(int)ExedenteHoras.TotalHours:d3}:{ExedenteHoras:mm}";
            }
        }
    }
}
