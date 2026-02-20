using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.CONEMB
{
    public class EDICONEMBCaterpillarExportacao
    {
        public string IdentificadoRegistro { get; set; }
        public int IdentificadorProcesso { get; set; }
        public int NumeroVersaoTransacao { get; set; }
        public int NumeroControleTransmissao { get; set; }
        public DateTime IdentificadorGeracaoMovimento { get; set; }
        public string IdentificadorTransmissor { get; set; }
        public string IdentificadorReceptor { get; set; }
        public string CodigoInternoTransmissor { get; set; }
        public string CodigoInternoReceptor { get; set; }
        public string NomeTransmissor { get; set; }
        public string NomeReceptor { get; set; }
        public List<ConhecimentoCaterpillarExportacao> Conhecimentos { get; set; }
        public RodapeCaterpillarExportacao FTP { get; set; }
    }

    public class ConhecimentoCaterpillarExportacao
    {
        public string IdentificadoRegistro { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public int QuantidadeNotas { get; set; }
        public decimal ValorNotas { get; set; }
        public decimal Valor { get; set; }
        public int CodigoFiscal { get; set; }
        public string ModalidadeFrete { get; set; }
        public string SituacaoTributaria { get; set; }
        public decimal Peso { get; set; }
        public decimal Volume { get; set; }
        public decimal BaseICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public string IdentificacaoLocalEntrega { get; set; }
        public string LocalColeta { get; set; }
        public string UnidadePeso { get; set; }
        public string UnidadeVolume { get; set; }
        public ComplementoConhecimentoCaterpillarExportacao CT2 { get; set; }
        public DestinatarioConhecimentoCaterpillarExportacao CT4 { get; set; }
        public DestinatarioConhecimentoCaterpillarExportacao CT5 { get; set; }
        public List<NotasConhecimentoCaterpillarExportacao> Notas { get; set; }

    }

    public class ComplementoConhecimentoCaterpillarExportacao
    {
        public string IdentificadoRegistro { get; set; }
        public decimal ValorSEC { get; set; }
        public decimal ValorITR { get; set; }
        public decimal ValorDespacho { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorAdeme { get; set; }
        public decimal ValorADValorem { get; set; }
        public decimal FretePeso { get; set; }
        public decimal ValorSUFRAMA { get; set; }
        public decimal OutrosValores { get; set; }
        public decimal ValorIRRF { get; set; }
    }

    public class DestinatarioConhecimentoCaterpillarExportacao
    {
        public string IdentificadoRegistro { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string IE { get; set; }
        public string Endereco { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
    }

    public class NotasConhecimentoCaterpillarExportacao
    {
        public string IdentificadoRegistro { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string NumeroEmbarque { get; set; }
        public string NumeroExportacao { get; set; }
        public string CodigoFabricaDestino { get; set; }
        public string TipoMercadoria { get; set; }
    }

    public class RodapeCaterpillarExportacao
    {
        public string IdentificadoRegistro { get; set; }
        public int NumeroControleTransmissao { get; set; }
        public int QuantidadeRegistros { get; set; }
        public decimal TotalValores { get; set; }
        public string CategoriaOperacao { get; set; }
    }
}
