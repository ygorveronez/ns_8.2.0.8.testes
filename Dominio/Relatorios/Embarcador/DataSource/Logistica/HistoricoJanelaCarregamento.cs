using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class HistoricoJanelaCarregamento
    {
        public int Codigo { get; set; }
        private DateTime DataRecusa { get; set; }
        public string DescricaoCentroCarregamento { get; set; }
        public string DescricaoCarga { get; set; }
        public string DescricaoMotivoRecusa { get; set; }
        public string JustificativaRecusa { get; set; }
        public string Descricao { get; set; }
        public string DescricaoVeiculo { get; set; }
        public string DataRecusaFormatada
        {
            get { return DataRecusa != DateTime.MinValue ? DataRecusa.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }


    }
}
