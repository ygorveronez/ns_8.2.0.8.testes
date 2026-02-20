namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_CARGA_ASSINATURA", EntityName = "CheckListCargaAssinatura", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura", NameType = typeof(CheckListCargaAssinatura))]
    public class CheckListCargaAssinatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAS_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAS_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAS_TIPO_ASSINATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaCheckListCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaCheckListCarga TipoAssinatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCarga", Column = "CLC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListCarga CheckList { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

    }
}
