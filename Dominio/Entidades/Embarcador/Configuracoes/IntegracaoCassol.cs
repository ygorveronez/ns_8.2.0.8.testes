namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CASSOL", EntityName = "IntegracaoCassol", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCassol", NameType = typeof(IntegracaoCassol))]
    public class IntegracaoCassol : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIC_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIC_TOKEN", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIC_URL_INTEGRACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string URLIntegracao { get; set; }
    }
}