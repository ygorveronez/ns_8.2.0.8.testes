namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_CARGA_OBSERVACAO", EntityName = "CheckListObservacao", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao", NameType = typeof(CheckListObservacao))]
    public class CheckListObservacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_CATEGORIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList Categoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }
    }
}