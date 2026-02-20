using System;
using CsvHelper.Configuration.Attributes;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class PlanilhaEmailCanhotoRejeitado
    {
        [Name("Canhoto")]
        [Index(0)]
        public virtual int Numero { get; set; }

        [Name("Data da Nota Fiscal")]
        [Index(1)]
        public virtual DateTime? DataEmissao { get; set; }

        [Name("Número da Nota Fiscal")]
        [Index(2)]
        public virtual int? NumeroNotaFiscal { get; set; }

        [Name("Chave de Acesso")]
        [Index(3)]
        public virtual string ChaveAcessoNotaFiscal { get; set; }

        [Name("Cliente")]
        [Index(4)]
        public virtual string Destinatario { get; set; }

        [Name("Número da Carga (DT)")]
        [Index(5)]
        public virtual string NumeroCarga { get; set; }

        [Name("Status Digitalização do Canhoto")]
        [Index(6)]
        public virtual string SituacaoDigitalizacaoCanhoto { get; set; }

        [Name("Status Recebimento Físico do Canhoto")]
        [Index(7)]
        public virtual string SituacaoRecebimentoCanhoto { get; set; }

        [Name("Quantidade de Dias desde a emissão da Nota Fiscal")]
        [Index(8)]
        public virtual int DiferencaDiasDesdeEmissaoNotaFiscal { get; set; }
    }
}