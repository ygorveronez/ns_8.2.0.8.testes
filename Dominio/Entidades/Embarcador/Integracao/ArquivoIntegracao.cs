namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_ARQUIVO", EntityName = "ArquivoIntegracao", Name = "Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao", NameType = typeof(ArquivoIntegracao))]
    public class ArquivoIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "ARI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "ARI_NOME", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeArquivo { get; set; }
    }
}
