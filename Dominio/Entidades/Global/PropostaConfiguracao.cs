namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROPOSTA_CONFIGURACOES", EntityName = "PropostaConfiguracao", Name = "Dominio.Entidades.PropostaConfiguracao", NameType = typeof(PropostaConfiguracao))]
    public class PropostaConfiguracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasValidade", Column = "PRC_DIAS_VALIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasValidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoCustosAdicionais", Column = "PRC_CUSTOS_ADICIONAIS", Type = "StringClob", NotNull = false)]
        public virtual string TextoCustosAdicionais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoFormaCobranca", Column = "PRC_FORMA_COBRANCA", Type = "StringClob", NotNull = false)]
        public virtual string TextoFormaCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoCTRN", Column = "PRC_CTRN", Type = "StringClob", NotNull = false)]
        public virtual string TextoCTRN { get; set; }
    }
}
