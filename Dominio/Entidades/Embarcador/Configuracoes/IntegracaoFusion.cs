namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_FUSION", EntityName = "IntegracaoFusion", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFusion", NameType = typeof(IntegracaoFusion))]
    public class IntegracaoFusion : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIF_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_URL_INTEGRACAO_PEDIDO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIF_URL_INTEGRACAO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIF_TOKEN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Token { get; set; }
    }
}
