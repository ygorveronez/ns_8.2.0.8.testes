namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_MERCOSUL", EntityName = "ConfiguracaoMercosul", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMercosul", NameType = typeof(ConfiguracaoMercosul))]
    public class ConfiguracaoMercosul : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CME_UTILIZAR_MESMO_NUMERO_CRT_CANCELAMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMesmoNumeroCRTCancelamentos { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CME_UTILIZAR_MESMO_NUMERO_MICDTA_CANCELAMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarMesmoNumeroMICDTACancelamentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CME_UTILIZAR_MESMO_NUMERO_CRT_AVERBACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCRTAverbacao { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para Mercosul"; }
        }
    }
}
