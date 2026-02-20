namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_API_CONEXAO_GERADOR_EXCEL_KMM", EntityName = "ConfiguracaoAPIConexaoGeradorExcelKMM", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM", NameType = typeof(ConfiguracaoAPIConexaoGeradorExcelKMM))]
    public class ConfiguracaoAPIConexaoGeradorExcelKMM: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAPIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarApiDeConexaoComGeradorExcelKMM", Column = "CAPIC_USAR_API_DE_CONEXAO_COM_GERADOR_EXCEL_KMM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarApiDeConexaoComGeradorExcelKMM { get; set; }
    }
}
