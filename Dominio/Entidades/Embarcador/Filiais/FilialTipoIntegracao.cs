namespace Dominio.Entidades.Embarcador.Filiais;

[NHibernate.Mapping.Attributes.Class(0, Table = "T_FILIAL_TIPO_INTEGRACAO", EntityName = nameof(FilialTipoIntegracao), Name = "Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao", NameType = typeof(FilialTipoIntegracao))]
public class FilialTipoIntegracao : EntidadeBase
{
    [NHibernate.Mapping.Attributes.Id(0, Name = nameof(Codigo), Type = "System.Int32", Column = "FTI_CODIGO")]
    [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
    public virtual int Codigo { get; set; }

    [NHibernate.Mapping.Attributes.ManyToOne(0, Class = nameof(Filial), Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
    public virtual Filial Filial { get; set; }

    [NHibernate.Mapping.Attributes.ManyToOne(0, Class = nameof(Cargas.TipoIntegracao), Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
    public virtual Cargas.TipoIntegracao TipoIntegracao { get; set; }
}
