using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.CONEMB
{
    public class EDICONEMBCaterpillarImportacao
    {
        public string Identificador { get; set; }
        public DateTime Data { get; set; }
        public int QuantidadeRegistro { get; set; }
        public List<ComponenteCaterpillarImportacao> Componentes { get; set; }
    }

    public class ComponenteCaterpillarImportacao
    {
        public string TipoLancamento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public string Embarque { get; set; }
        public string NumeroDIEmbarque { get; set; }
        public string MasterBL { get; set; }
        public string NumeroHouse { get; set; }
        public int QuantidadeRegistro { get; set; }
        public decimal ValorDespesaMoeda { get; set; }
        public string CodigoMoeda { get; set; }
        public decimal ValorDespesaMoedaNacional { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public int NumeroDias { get; set; }
        public int NumeroContainer { get; set; }
        public string TipoContainer { get; set; }
        public string NumeroCTe { get; set; }
        public string SerieCTe { get; set; }
        public DateTime DataCTe { get; set; }
        public string CNPJEmitente { get; set; }
        public decimal BaseICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal BaseICMSST { get; set; }
        public decimal AliquotaICMSST { get; set; }
        public decimal ValorICMSST { get; set; }
    }
}
