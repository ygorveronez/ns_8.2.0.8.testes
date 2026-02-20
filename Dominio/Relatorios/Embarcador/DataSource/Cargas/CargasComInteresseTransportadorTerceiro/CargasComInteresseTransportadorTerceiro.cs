using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.CargasComInteresseTransportadorTerceiro
{
    public sealed class CargasComInteresseTransportadorTerceiro
    {
        public int Codigo { get; set; }
        public SituacaoCargaJanelaCarregamentoTransportador Situacao { get; set; }
        private DateTime DataCriacaoCarga { get; set; }
        public string DescricaoCentroCarregamento { get; set; }
        public string DescricaoCarga { get; set; }
        public string DescricaoVeiculo { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public int Posicao { get; set; }
        public string RealizouACarga { get; set; }
        public string Motivo { get; set; }

        public string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoDescricao
        {
            get { return Situacao.ObterDescricao(); }
        }
    }
}
