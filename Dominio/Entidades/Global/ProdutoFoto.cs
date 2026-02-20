namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_FOTO", EntityName = "ProdutoFoto", Name = "Dominio.Entidades.ProdutoFoto", NameType = typeof(ProdutoFoto))]
    public class ProdutoFoto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PFT_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "PFT_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "PFT_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PFT_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        public virtual bool Equals(ProdutoFoto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
