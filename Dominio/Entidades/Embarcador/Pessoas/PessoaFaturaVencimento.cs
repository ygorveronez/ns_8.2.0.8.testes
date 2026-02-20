namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_FATURA_VENCIMENTO", EntityName = "PessoaFaturaVencimento", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento", NameType = typeof(PessoaFaturaVencimento))]
    public class PessoaFaturaVencimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaInicial", Column = "PFV_DIA_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaFinal", Column = "PFV_DIA_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaVencimento", Column = "PFV_DIA_VENCIMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }
    }
}
