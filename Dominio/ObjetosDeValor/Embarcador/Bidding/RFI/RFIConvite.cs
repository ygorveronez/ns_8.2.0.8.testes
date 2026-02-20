using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.RFI
{
    public class RFIConvite
    {
        public int Codigo { get; set; }
        public bool Iniciado { get; set; }
        public bool Situacao { get; set; }
        public StatusRFIConvite StatusRFIConvite { get; set; }
        public string Descricao { get; set; }
        public string DataInicio { get; set; }
        public string DataLimite { get; set; }
        public string DescritivoConvite { get; set; }
        public bool ExigirPreenchimentoChecklistConvitePeloTransportador { get; set; }
        public string PrazoAceiteConvite { get; set; }
        public DateTime TempoRestante { get; set; }        
        public StatusRFIConvite Etapa { get; set; }
        public List<RFIConvidado> Convidados { get; set; }
        public List<RFIConviteAnexo> Anexos { get; set; }
        public RFIChecklist RFIChecklist { get; set; }
    }
}
