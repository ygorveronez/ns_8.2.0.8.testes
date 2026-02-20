using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class ImportacaoCTeEmitidoForaEmbarcadorAdicionar
    {
        public int Tipo { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string Chave { get; set; }
        public DateTime? DataEmissao { get; set; }
        public Dominio.Entidades.Empresa Empresa { get; set; }
        public Dominio.Entidades.Cliente Remetente { get; set; }
        public Dominio.Entidades.Cliente Destinatario { get; set; }
        public Dominio.Entidades.Cliente Recebedor { get; set; }
        public Dominio.Entidades.Cliente Tomador { get; set; }
        public Dominio.Entidades.Cliente Expedidor { get; set; }
        public Dominio.Entidades.Localidade MunicipioInicio { get; set; }
        public Dominio.Entidades.Localidade MunicipioFim { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }
        public int QuantidadeCTe { get; set; }
        public int QuantidadeNFe { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotalMercadoria { get; set; }
        public decimal PesoBaseCalculo { get; set; }
        public decimal BaseICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal TotalImpostos { get; set; }
        public decimal TotalFrete { get; set; }
        public decimal FretePeso { get; set; }
        public decimal GrisAdv { get; set; }
        public decimal Imposto { get; set; }
        public decimal Pedagio { get; set; }        
        public decimal Taxas { get; set; }

    }
}