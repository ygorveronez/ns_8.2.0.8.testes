namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class ConfiguracaoContabil
    {
        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }
    }
}
