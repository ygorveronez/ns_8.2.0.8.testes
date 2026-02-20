namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class ObjetoOrdemServicoServicos
    {
        public Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota Servico { get; set; }
        public Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo UltimaManutencao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoServicoVeiculoOrdemServicoFrota TipoManutencao { get; set; }
        public decimal CustoEstimado { get; set; }
        public decimal CustoMedio { get; set; }
        public string Observacao { get; set; }
        public int TempoEstimado { get; set; }
    }
}
