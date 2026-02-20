namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_ANEXO", EntityName = "TipoOperacaoAnexo", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo", NameType = typeof(TipoOperacaoAnexo))]
    public class TipoOperacaoAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TOA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "TOA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "TOA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }
    }

}
