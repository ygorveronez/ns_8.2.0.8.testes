namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONVERSAO_UNIDADE_MEDIDA", EntityName = "ConversaoUnidadeMedida", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ConversaoUnidadeMedida", NameType = typeof(ConversaoUnidadeMedida))]
    public class ConversaoUnidadeMedida : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CUM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CUM_DESCRICAO", TypeType = typeof(string), NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Fator", Column = "CUM_FATOR", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal Fator { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedidaOrigem", Column = "CUM_UNIDADE_MEDIDA_ORIGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida UnidadeMedidaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMedidaDestino", Column = "CUM_UNIDADE_MEDIDA_DESTINO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida UnidadeMedidaDestino { get; set; }

    }
}
