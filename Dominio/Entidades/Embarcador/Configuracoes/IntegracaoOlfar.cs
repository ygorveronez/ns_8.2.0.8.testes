namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_OLFAR", EntityName = "IntegracaoOlfar", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOlfar", NameType = typeof(IntegracaoOlfar))]
    public class IntegracaoOlfar : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "COL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "COL_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }
    }
}