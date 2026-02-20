using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaDocumentoParaEmissaoNFSManual
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }
        public Dominio.ObjetosDeValor.Localidade LocalidadePrestacao { get; set; }
        public string Chave { get; set; }
        public int Numero { get; set; }
        public string Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public decimal ValorPrestacaoServico { get; set; }
        public decimal BaseCalculoISS { get; set; }
        public decimal PercentualAliquotaISS { get; set; }
        public decimal Peso { get; set; }
    }
}
