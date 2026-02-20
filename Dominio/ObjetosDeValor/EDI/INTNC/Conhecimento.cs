using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.INTNC
{
    public class Conhecimento
    {
        public int IdNotaCobranca { get; set; }
        public int NumeroNotaCobranca { get; set; }
        public long SequencialUnico { get; set; }
        public string CodModelo { get; set; }
        public string NumeroNC { get; set; }
        public string SerieNC { get; set; }
        public string TipoPessoaTransportadora { get; set; }
        public string CodigoTransportadora { get; set; }
        public string CNPJTransportadora { get; set; }
        public string IETransportadora { get; set; }
        public string TipoPessoaResponsavelFrete { get; set; }
        public string CodResponsavelFrete { get; set; }
        public string CNPJResponsavelFrete { get; set; }

        public string TipoPessoaRemetente { get; set; }
        public string CodRemetente { get; set; }
        public string CNPJRemetente { get; set; }
        public string UFRemetente { get; set; }

        public string TipoPessoaDestinatario { get; set; }
        public string CodDestinatario { get; set; }
        public string CNPJDestinatario { get; set; }
        public string UFDestinatario { get; set; }

        public string DataRegistroNC { get; set; }
        public DateTime? DataAtual { get; set; }
        public DateTime? DataEmissaoNC { get; set; }
        public DateTime? DataEmissaoNCMesContabil { get; set; }
        public DateTime? DataCancelamento { get; set; }

        public DateTime? DataVencimento { get; set; }
        public string CodNaturezaOperacao { get; set; }
        public int CodNaturezaOperacaoInt { get; set; }
        public string CodNaturezaOperacaoCompra { get; set; }
        public int CodNaturezaOperacaoCompraInt { get; set; }
        public decimal ValorFreteApagarNC { get; set; }
        public decimal ValorTotalMercadoria { get; set; }
        public decimal ValorDescontoNC { get; set; }
        public int CodigoUnidadeNegocio { get; set; }
        public decimal Zero { get; set; }
        public string SubstituicaoTributariaICMS { get; set; }
        public string UFOrigemFrete { get; set; }
        public string UFDestinoFrete { get; set; }
        public string IBGEOrigemFrete { get; set; }
        public string IBGEDestinoFrete { get; set; }
        public string TipoCTe { get; set; }
        public string CodFatura { get; set; }
        public decimal ValorFreteNC { get; set; }

        public decimal valorItem { get; set; }
        public decimal ValorPrestacao { get; set; }
        public int NumeroDoc { get; set; }


        public string TipoICMS { get; set; }

        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorBaseICMS { get; set; }

        public decimal AliquotaICMSST { get; set; }
        public decimal ValorICMSST { get; set; }
        public decimal ValorBaseICMSST { get; set; }

        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorBaseISS { get; set; }


        public decimal ValorBaseICMSDiferencialAliquota { get; set; }
        public decimal PercentualDiferencialAliquota { get; set; }

        public string GrupoCFOP { get; set; }
        public string CodigoTipoNota { get; set; }
        public string FinalidadeOperacao { get; set; }
        public int IdNotaEntrada { get; set; }
        public string CFOP { get; set; }
        public string TipoFrete { get; set; }
        public string CodCFOPGrupo { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal PesoBruto { get; set; }
        public int QtdVolumes { get; set; }
        public string ChaveCTe { get; set; }
        public DateTime? DataHoraAutorizacaoCTe { get; set; }
        public string ProtocoloCTe { get; set; }
        public List<Imposto> Impostos { get; set; }
        public List<NotaFiscal> Notas { get; set; }
        public List<NotaFiscal> NotasFiscaisCTes { get; set; }
        public List<ColetaEntrega> ColetasEntregas { get; set; }
        public List<Tomador> Tomadores { get; set; }
        public Conhecimento ItemConhecimento { get; set; }
        public decimal BasePIS { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal AliquotaPIS { get; set; }
        public decimal BaseCofins { get; set; }
        public decimal ValorCofins { get; set; }
        public decimal AliquotaCofins { get; set; }
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
    }
}
