namespace Dominio.Entidades.Embarcador.Alcada
{
    public class Alcada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Property(0, Column = "BAL_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BAL_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BAL_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao Juncao { get; set; }
    }
}
