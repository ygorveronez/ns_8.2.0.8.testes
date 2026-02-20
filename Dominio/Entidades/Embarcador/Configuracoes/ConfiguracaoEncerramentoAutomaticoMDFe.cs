namespace Dominio.Entidades.Embarcador.Configuracoes
{
    /// <summary>
    /// Configurações gerais referentes ao Encerramento automatico da MDFe
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_ENCERRAMENTO_AUTOMATICO_MDFE", EntityName = "ConfiguracaoEncerramentoAutomaticoMDFe", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe", NameType = typeof(ConfiguracaoEncerramentoAutomaticoMDFe))]

    public class ConfiguracaoEncerramentoAutomaticoMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_DIAS_ENCERRAMENTO_AUTOMATICO_MDFE", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasEncerramentoAutomaticoMDFE { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para encerramento automatico MDF-e"; }
        }
    }
}
