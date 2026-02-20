namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_RAIZ_CNPJ", EntityName = "GrupoPessoasRaizCNPJ", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ", NameType = typeof(GrupoPessoasRaizCNPJ))]
    public class GrupoPessoasRaizCNPJ : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RaizCNPJ", Column = "GRC_CNPJ", TypeType = typeof(string), Length = 14, NotNull = true)]
        public virtual string RaizCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarPessoasMesmaRaiz", Column = "GRC_ADICIONAR_PESSOAS_MESMA_RAIZ", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AdicionarPessoasMesmaRaiz { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.RaizCNPJ;
            }
        }

        public virtual string FormatarRaiz
        {
            get
            {
                return this.RaizCNPJ;
            }
        }

        public virtual bool Equals(GrupoPessoasRaizCNPJ other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
