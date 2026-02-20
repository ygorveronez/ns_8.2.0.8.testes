namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContabil
    {
        public int PlanoConta { get; set; }
        public int PlanoContaContraPartida { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }
        public bool ComponentesDeFreteDoTipoDescontoNaoDevemSomar { get; set; }
    }
}
