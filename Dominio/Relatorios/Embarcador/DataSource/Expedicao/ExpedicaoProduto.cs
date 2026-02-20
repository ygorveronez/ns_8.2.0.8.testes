using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Expedicao
{
    public class ExpedicaoProduto
    {
        public string CodigoProduto { get; set; }
        public string Produto { get; set; }
        public string CodigoGrupoProduto { get; set; }
        public string GrupoProduto { get; set; }
        public string UnidadeDeMedida { get; set; }
        public string Filial { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PesoUnitarioProduto { get; set; }

        public decimal PesoTotal 
        {
            get { return Math.Round(Quantidade * PesoUnitarioProduto, 2, MidpointRounding.AwayFromZero); }
        }
    }
}
