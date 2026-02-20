using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroPesquisaRelatorioAuditoriaCTe
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public int NumeroDocumentoInicial { get; set; }
        public int NumeroDocumentoFinal { get; set; }
        public int CodigoCarga { get; set; }
        public double CpfCnpjTomador { get; set; }
        public int modeloDocumento { get; set; }
        public int codigoFilial { get; set; }
        public List<string> statusCTe { get; set; }
        public int grupoPessoas { get; set; }
        public int codigoTransportador { get; set; }
    }
}
