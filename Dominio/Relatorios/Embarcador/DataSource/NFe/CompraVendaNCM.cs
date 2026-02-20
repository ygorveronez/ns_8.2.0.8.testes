using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class CompraVendaNCM
    {
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public string COFINS { get; set; }
        public string PIS { get; set; }
        public string NCM { get; set; }
        public string DescricaoTipo { get; set; }
        public Int64 Numero { get; set; }
        public UnidadeDeMedida UnidadeMedida { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Valor { get; set; }

        public string UnidadeMedidaFormatada
        {
            get { return UnidadeMedida.ObterSigla(); }
        }
    }
}
