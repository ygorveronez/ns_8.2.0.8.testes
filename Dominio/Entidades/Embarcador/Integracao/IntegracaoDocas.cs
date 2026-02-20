namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_DOCAS", EntityName = "IntegracaoDocas", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas", NameType = typeof(IntegracaoDocas))]
    public class IntegracaoDocas : Dominio.Entidades.Embarcador.Integracao.Integracao
    {
        public IntegracaoDocas() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "IDO_NOME", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoMedioCarregamento", Column = "IDO_TEMPO_MEDIO_CARREGAMENTO_SEGUNDOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMedioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "IDO_ID_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

    }
}
