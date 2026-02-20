using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar
{
    public class RetornoConsultaTarifasPracasPedagio
    {
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public string Erro { get; set; }
        public string XMLRequest { get; set; }
        public string XMLResponse { get; set; }
    }
}
