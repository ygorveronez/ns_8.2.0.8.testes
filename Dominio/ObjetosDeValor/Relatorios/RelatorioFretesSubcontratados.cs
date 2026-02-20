using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioFretesSubcontratados
    {
        public int CodigoFreteSubcontratado { get; set; }

        public string Filial { get; set; }

        public string Parceiro { get; set; }

        public int CTe { get; set; }

        public int NFe { get; set; }

        public DateTime DataEntrada { get; set; }

        public string Remetente { get; set; }

        public string Destinatario { get; set; }

        public string Cidade { get; set; }

        public Dominio.Enumeradores.TipoFreteSubcontratado Tipo { get; set; }

        public string DescricaoTipo
        {
            get
            {
                return this.Tipo.ToString("G");
            }
        }

        public decimal Peso { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal ValorTaxaAdicional { get; set; }

        public decimal ValorFreteLiquido { get; set; }

        public decimal ValorTDA { get; set; }

        public decimal ValorTDE { get; set; }

        public decimal ValorCarroDedicado { get; set; }

        public decimal ValorComissao { get; set; }

        public decimal PercentualComissao { get; set; }

        public decimal ValorTotalComissao { get; set; }

        public DateTime? DataEntrega { get; set; }

        public string Observacao { get; set; }

        public string Motorista { get; set; }

    }
}
