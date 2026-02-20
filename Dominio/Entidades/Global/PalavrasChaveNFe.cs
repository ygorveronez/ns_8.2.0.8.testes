namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALAVRAS_CHAVE_NFE", EntityName = "PalavrasChaveNFe", Name = "Dominio.Entidades.PalavrasChaveNFe", NameType = typeof(PalavrasChaveNFe))]

    public class PalavrasChaveNFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Palavra", Column = "PCN_PALAVRA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Palavra { get; set; }

    }
}
