using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaRegraQualidadeMonitoramento
    {
        public string Descricao { get; set; }
        public TipoRegraQualidadeMonitoramento? TipoRegra { get; set; }
        public bool? Ativo { get; set; }
    }
}
