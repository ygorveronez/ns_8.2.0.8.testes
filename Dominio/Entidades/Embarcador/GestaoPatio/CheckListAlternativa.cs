namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_ALTERNATIVA", EntityName = "CheckListAlternativa", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa", NameType = typeof(CheckListAlternativa))]
    public class CheckListAlternativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLA_OPCAO_IMPEDITIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OpcaoImpeditiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLA_VALOR", TypeType = typeof(int), NotNull = true)]
        public virtual int Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLA_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListOpcoes", Column = "CLO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListOpcoes CheckListOpcoes { get; set; }
    }
}