using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class RevisaoSMDetalhamentoCheckList
    {
        public string SolicitarCheckList { get; set; }
        public string TipoEquipamento { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public string NomeContato { get; set; }
        public string FoneContato { get; set; }
        public string EmailContato { get; set; }
        public string FoneMotorista { get; set; }
    }
}
