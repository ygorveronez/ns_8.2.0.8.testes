namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_FORNECEDOR_VENCIMENTO", EntityName = "GrupoPessoasFornecedorVencimento", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento", NameType = typeof(GrupoPessoasFornecedorVencimento))]
    public class GrupoPessoasFornecedorVencimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaEmissaoInicial", Column = "GFV_DIA_EMISSAO_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaEmissaoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaEmissaoFinal", Column = "GFV_DIA_EMISSAO_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaEmissaoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vencimento", Column = "GFV_VENCIMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int Vencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        public virtual string DataEmissao
        {
            get { return DiaEmissaoInicial + " até " + DiaEmissaoFinal; }
        }
    }
}
