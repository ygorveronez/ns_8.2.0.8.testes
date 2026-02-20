namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_FATURA_VENCIMENTO", EntityName = "GrupoPessoasFaturaVencimento", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento", NameType = typeof(GrupoPessoasFaturaVencimento))]
    public class GrupoPessoasFaturaVencimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaInicial", Column = "GFV_DIA_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaFinal", Column = "GFV_DIA_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaVencimento", Column = "GFV_DIA_VENCIMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }
    }
}
