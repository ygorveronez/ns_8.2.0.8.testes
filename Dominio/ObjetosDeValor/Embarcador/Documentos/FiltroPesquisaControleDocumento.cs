using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class FiltroPesquisaControleDocumento
    {
        public int CodigoFilial { get; set; }
        public List<SituacaoControleDocumento> SituacaoControleDocumento { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public int Empresa { get; set; }
        public int CodigoModeloDocumentoFiscal { get; set; }
        public int CodigoCarga { get; set; }
        public int NFe { get; set; }
        public int CodigoTransportador { get; set; }
        public List<int> CodigosPortfolio { get; set; }
        public List<int> CodigosIrregularidade { get; set; }
        public List<int> CodigosSetor { get; set; }
        public int CodigoUsuario { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataGeracaoIrregularidadeInicial { get; set; }
        public DateTime DataGeracaoIrregularidadeFinal { get; set; }
        public bool TransportadorLogado { get; set; }
      
    }
}
