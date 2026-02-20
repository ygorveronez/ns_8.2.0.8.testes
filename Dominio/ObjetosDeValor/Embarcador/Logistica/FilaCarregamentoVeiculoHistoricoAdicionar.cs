namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoHistoricoAdicionar
    {
        public string Descricao { get; set; }

        public Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo FilaCarregamentoVeiculo { get; set; }

        public Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento MotivoAlteracaoPosicaoFilaCarregamento { get; set; }

        public Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento MotivoRetiradaFilaCarregamento { get; set; }

        public Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem MotivoSelecaoMotoristaForaOrdem { get; set; }

        public Enumeradores.OrigemAlteracaoFilaCarregamento OrigemAlteracao { get; set; }

        public Enumeradores.TipoFilaCarregamentoVeiculoHistorico Tipo { get; set; }
        public string Observacao { get; set; }
    }
}
