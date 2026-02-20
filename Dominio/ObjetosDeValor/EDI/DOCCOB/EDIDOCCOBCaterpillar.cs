using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class EDIDOCCOBCaterpillar
    {
        public string IdentificadorRegistro { get; set; }
        public int IdentificadorProcesso { get; set; }
        public int NumeroVersaoTransacao { get; set; }
        public int NumeroControleTransmissao { get; set; }
        public DateTime IdentificacaoGeracaoMovimento { get; set; }
        public string IdentificadorTransmissor { get; set; }
        public string IdentificadorReceptor { get; set; }
        public string CodigoInternoTransmissor { get; set; }
        public string CodigoInternoReceptor { get; set; }
        public string NomeTransmissor { get; set; }
        public string NomeReceptor { get; set; }
        public EDIDOCCOBCaterpillarDT1 DT1 { get; set; }
        public EDIDOCCOBCaterpillarDT3 DT3 { get; set; }
        public List<EDIDOCCOBCaterpillarDT2> Conhecimentos { get; set; }
        public EDIDOCCOBCaterpillarFTP FTP { get; set; }
    }

    public class EDIDOCCOBCaterpillarDT1
    {
        public string IdentificadorRegistro { get; set; }
        public int NumeroDuplicata { get; set; }
        public DateTime DataEmissao { get; set; }
        public int QuantidadeConhecimentos { get; set; }
        public decimal ValorTotalFatura { get; set; }
        public DateTime DataVencimento { get; set; }
        public int TipoDocumento { get; set; }
        public int NumeroExportacao { get; set; }
        public DateTime DataEmbarque { get; set; }
        public int NumeroViagens { get; set; }
        public string LocalDescarga { get; set; }
        public string LocalEmbarque { get; set; }
        public string NumeroConhecimentoEmbarque { get; set; }
        public string Material { get; set; }
        public decimal RetencaoINSS { get; set; }

    }

    public class EDIDOCCOBCaterpillarDT3
    {
        public string IdentificadorRegistro { get; set; }
        public decimal DescontoINSS { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorLiquido { get; set; }
        public string MeioTransporte { get; set; }
        public string NumeroPedido { get; set; }
        public int NumeroExportacao { get; set; }
        public int NumeroAduaneiro { get; set; }
        public decimal ValorDespesas { get; set; }
        public decimal ValorImpostoRenda { get; set; }
    }

    public class EDIDOCCOBCaterpillarDT2
    {
        public string IdentificadorRegistro { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Servicos { get; set; }
        public decimal ValorDolar { get; set; }
        public decimal TaxaDolar { get; set; }
        public decimal ValorServicos { get; set; }
    }

    public class EDIDOCCOBCaterpillarFTP
    {
        public string IdentificadoRegistro { get; set; }
        public int NumeroControleTransmissao { get; set; }
        public int QuantidadeRegistros { get; set; }
        public decimal TotalValores { get; set; }
        public string CategoriaOperacao { get; set; }
    }
}
