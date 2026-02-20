using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class FilaCarregamentoVeiculoHistorico
    {
        public int Codigo { get; set; }

        public System.DateTime Data { get; set; }

        public string DataFormatada
        {
            get { return Data.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string Descricao { get; set; }

        public string FilaCarregamentoVeiculo { get; set; }

        public string ModeloVeicular { get; set; }

        public string NomeMotorista { get; set; }

        public string NomeUsuario { get; set; }

        public int Posicao { get; set; }

        public string Reboques { get; set; }

        public TipoFilaCarregamentoVeiculoHistorico Tipo { get; set; }

        public string TipoDescricao
        {
            get { return Tipo.ObterDescricao(); }
        }

        public string Tracao { get; set; }

        public string ObservacaoHistoricoVinculo { get; set; }
    }
}
