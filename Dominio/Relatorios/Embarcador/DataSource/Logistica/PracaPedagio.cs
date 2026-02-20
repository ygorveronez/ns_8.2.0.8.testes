using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class PracaPedagio
    {
        #region Propriedades
        
        public int Codigo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public string Rodovia { get; set; }
        public decimal KM { get; set; }
        private bool Situacao { get; set; }
        public string Concessionaria { get; set; }
        public decimal ValorTarifa { get; set; }
        private DateTime DataTarifa { get; set; }
        public string ModeloVeicularCargaTarifa { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoAtivo
        {
            get { return Situacao ? "Ativo" : "Inativo"; }
        }

        public string DataTarifaFormatada
        {
            get { return DataTarifa != DateTime.MinValue ? DataTarifa.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion

    }
}
