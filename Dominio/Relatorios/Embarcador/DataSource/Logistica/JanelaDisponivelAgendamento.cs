using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class JanelaDisponivelAgendamento
    {
        public string Destino { get; set; }
        public string Hora { get; set; }
        public string TipoDeCarga { get; set; }
        public string QuantidadeItens { get; set; }
        public string Data { get; set; }
        public int QuantidadeJanelasDisponiveis { get; set; }
        public int QuantidadeJanelasOcupadas { get; set; }
        public int QuantidadeJanelasCadastradas { get; set; }
        public string JanelaExclusiva { get; set; }
        public string DataChegadaPlanejada { get; set; }
        public string Dia { get; set; }
        public string Mes { get; set; }
        public string GrupoProduto { get; set; }
        public string AgendaExtra { get; set; }
        public DateTime InicioDescarregamento { get; set; }
    }
}
