using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.MDFe
{
    public sealed class FiltroPesquisaMDFesAverbadosRelatorio
    {
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoSeguradora { get; set; }
        public Dominio.Enumeradores.StatusAverbacaoMDFe? Status { get; set; }
        public int CodigoModeloDocumentoFiscal { get; set; }
        public List <int> CodigosFiliais { get; set; }
        public List <double> CodigosRecebedores { get; set; }
    }
}
