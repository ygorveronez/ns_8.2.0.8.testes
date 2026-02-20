namespace Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento
{
    public sealed class FilaCarregamentoVeiculo
    {
        public int Codigo { get; set; }

        public string ModeloVeicular { get; set; }

        public string Motorista { get; set; }

        public string Posicao { get; set; }

        public string Reboques { get; set; }

        public string Tracao { get; set; }

        public Enumeradores.SituacaoFilaCarregamentoVeiculo Situacao { get; set; }

        public Enumeradores.TipoFilaCarregamentoVeiculo Tipo { get; set; }
    }
}
