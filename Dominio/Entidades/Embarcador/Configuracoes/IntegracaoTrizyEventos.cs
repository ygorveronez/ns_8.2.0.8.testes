namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_TRIZY_EVENTOS", EntityName = "IntegracaoTrizyEventos", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizyEventos", NameType = typeof(IntegracaoTrizyEventos))]
    public class IntegracaoTrizyEventos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIT_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIT_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_TOKEN", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Token { get; set; }
    }
}
