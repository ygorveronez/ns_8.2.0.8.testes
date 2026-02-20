using System;

namespace Dominio.ObjetosDeValor.WebService.CargaCancelamento
{
    public class EnvioCancelamentoCTe
    {
        public string ChaveCTe { get; set; }
        public string Status { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public DateTime? DataAnulacao { get; set; }
        public string ProtocoloCancelamentoInutilizacao { get; set; }
        public string MensagemRetornoSefaz { get; set; }
        public DateTime? DataRetornoSefaz { get; set; }
        public string ObservacaoCancelamento { get; set; }
        public string Cancelado { get; set; }
    }
}
