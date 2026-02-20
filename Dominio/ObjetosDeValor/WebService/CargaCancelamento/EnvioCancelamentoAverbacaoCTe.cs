using System;

namespace Dominio.ObjetosDeValor.WebService.CargaCancelamento
{
    public class EnvioCancelamentoAverbacaoCTe
    {
        public string ChaveCTe { get; set; }
        public string Protocolo { get; set; }
        public string CodigoRetorno { get; set; }
        public string MensagemRetorno { get; set; }
        public int CodigoIntegracao { get; set; }
        public DateTime? DataRetorno { get; set; }
        public Enumeradores.StatusAverbacaoCTe Status { get; set; }
        public Enumeradores.TipoAverbacaoCTe Tipo { get; set; }
    }
}
