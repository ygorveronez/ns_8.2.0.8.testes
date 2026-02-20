using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class DocumentoSaida
    {
        public DateTime DataGeracao { get; set; }
        public string CNPJEmpresa { get; set; }
        public string OpcaoBases { get; set; }
        public string Origem { get; set; }
        public string OpcaoRetencao { get; set; }
        public int Sequencia { get; set; }
        public List<DocumentoSaidaRemetente> Remetentes { get; set; }
        public DocumentoSaidaTrailer Trailer { get; set; }
    }
}
