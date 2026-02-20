namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class MovimentacaoEstoqueQuantidade
    {
        public int Quantidade { get; set; }

        public Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet SituacaoDevolucao { get; set; }
    }
}
