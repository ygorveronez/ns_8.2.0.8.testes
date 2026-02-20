namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate =  true, Table = "T_CONFIGURACAO_CONTROLE_INTEGRACAO_CARGA_EDI", EntityName = "ConfiguracaoControleIntegracaoCargaEDI", Name = "Dominio.Entidades.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI", NameType = typeof(ConfiguracaoControleIntegracaoCargaEDI))]
    public class ConfiguracaoControleIntegracaoCargaEDI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessarSomentePrioritario", Column = "CCE_SOMENTE_PRIORITARIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ProcessarSomentePrioritario { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
