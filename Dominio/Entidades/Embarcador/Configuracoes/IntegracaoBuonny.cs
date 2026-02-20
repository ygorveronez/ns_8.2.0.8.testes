namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_BUONNY", EntityName = "IntegracaoBuonny", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuonny", NameType = typeof(IntegracaoBuonny))]
    public class IntegracaoBuonny : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIB_CNPJ_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIB_TOKEN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Token { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Buonny";
            }
        }

    }
}
