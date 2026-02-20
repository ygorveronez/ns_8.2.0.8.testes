using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class NotaEntrada
    {
        public string TipoRegistro { get; set; }
        public DateTime DataGeracao { get; set; }
        public string CNPJEmpresa { get; set; }
        public string OpcaoBase { get; set; }
        public string Origem { get; set; }
        public string OpcaoRetencao { get; set; }
        public string Brancos { get; set; }
        public string UsoEBS { get; set; }
        public string Sequencia { get; set; }
        public List<NotaEntradaEmitenteDestinatario> Emitentes { get; set; }
        public NotaEntradaTrailler Trailler { get; set; }
    }
}
