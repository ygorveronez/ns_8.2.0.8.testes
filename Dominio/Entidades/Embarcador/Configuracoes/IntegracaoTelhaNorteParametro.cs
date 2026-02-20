namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PARAMETRO_INTEGRACAO_TELHA_NORTE", EntityName = "IntegracaoTelhaNorteParametro", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro", NameType = typeof(IntegracaoTelhaNorteParametro))]
    public class IntegracaoTelhaNorteParametro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PTN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTN_CHAVE", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PTN_VALOR", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Valor { get; set; }
    }
}
