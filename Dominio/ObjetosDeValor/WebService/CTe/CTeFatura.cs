using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class CTeFatura
    {
        public long ProtocoloCTe { get; set; }
        public string ChaveCTe { get; set; }
        public string SituacaoCTe { get; set; }
        public string NumeroCarga { get; set; }
        public decimal ValorCTe { get; set; }
        public string MensagemSefaz { get; set; }
        public string DataEmissao { get; set; }
        public string DataEvento { get; set; }
        public string CNPJEmissor { get; set; }

        public int Protocolo { get; set; }
        public string Chave { get; set; }
        public string NumeroControle { get; set; }
        public int Numero { get; set; }
        public string PDFFatura { get; set; }
        public string PDFBoleto { get; set; }
        public string PDFCTe { get; set; }
        public string NumeroBoleto { get; set; }
        public int NumeroFaturaIndividual { get; set; }
        public int NumeroFaturaAgrupado { get; set; }
        public string DataVencimento { get; set; }
        public decimal ValorIndividual { get; set; }
        public decimal ValorAgrupado { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorTotal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Favorecido { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Empresa { get; set; }
        public Embarcador.Carga.Viagem Viagem { get; set; }
        public string Banco { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura? SituacaoFatura { get; set; }
        public DateTime? DataEmissaoFatura { get; set; }
        public string DataEmissaoFaturaFormatado { get; set; }
        public string DataEmissaoFormatado { get; set; }
        public string DataVencimentoFormatado { get; set; }
        public bool GerarFaturamentoAVista { get; set; }
        public string NumeroBooking { get; set; }
        public bool RolagemCarga { get; set; }
    }
}
