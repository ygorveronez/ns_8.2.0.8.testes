namespace Dominio.Entidades.Embarcador.Relatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTOMATIZACAO_GERACAO_RELATORIO_DADOS_CONSULTA", EntityName = "AutomatizacaoGeracaoRelatorioDadosConsulta", Name = "Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioDadosConsulta", NameType = typeof(AutomatizacaoGeracaoRelatorioDadosConsulta))]
    public class AutomatizacaoGeracaoRelatorioDadosConsulta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ADC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ADC_RELATORIO_TEMPORARIO", Type = "StringClob", NotNull = false)]
        public virtual string RelatorioTemporario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ADC_PARAMETROS_CONSULTA", Type = "StringClob", NotNull = false)]
        public virtual string ParametrosConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ADC_FILTROS_PESQUISA", Type = "StringClob", NotNull = false)]
        public virtual string FiltrosPesquisa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ADC_PROPRIEDADES", Type = "StringClob", NotNull = false)]
        public virtual string Propriedades { get; set; }
    }
}
