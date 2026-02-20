namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_VALOR_FRETE_ADVALOREM", EntityName = "RegrasAdValorem", Name = "Dominio.Entidades.Embarcador.Frete.RegrasAdValorem", NameType = typeof(RegrasAdValorem))]
    public class RegrasAdValorem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoTabelaFrete", Column = "RAF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoTabelaFrete RegrasAutorizacaoTabelaFrete { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RFA_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RFA_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete Juncao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "RFA_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = true)]
        public virtual decimal Valor { get; set; }

    }

}
