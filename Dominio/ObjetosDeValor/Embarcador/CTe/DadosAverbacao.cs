using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class DadosAverbacao
    {
        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe CTe { get; set; }
        public string ChaveCTe { get; set; }
        public string ChaveNFe { get; set; }
        public string NumeroCarga { get; set; }
        public string NummeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string Protocolo { get; set; }
        public int tentativasIntegracao { get; set; }
        public string CodigoRetorno { get; set; }
        public string MensagemRetorno { get; set; }
        public int CodigoIntegracao { get; set; }
        public DateTime? DataRetorno { get; set; }
        public Dominio.Enumeradores.TipoAverbacaoCTe Tipo { get; set; }
        public Dominio.Enumeradores.StatusAverbacaoCTe Status { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento SituacaoFechamento { get; set; }
        public decimal Adicional { get; set; }
        public decimal IOF { get; set; }
        public Dominio.Enumeradores.IntegradoraAverbacao SeguradoraAverbacao { get; set; }
        public string Averbacao { get; set; }
        public ApoliceSeguro ApoliceSeguroAverbacao { get; set; }
        public decimal Desconto { get; set; }
        public decimal? Percentual { get; set; }
        public Dominio.Enumeradores.FormaAverbacaoCTE Forma { get; set; }
    }
}
