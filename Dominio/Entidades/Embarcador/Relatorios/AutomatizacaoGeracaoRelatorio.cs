using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Relatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTOMATIZACAO_GERACAO_RELATORIO", EntityName = "AutomatizacaoGeracaoRelatorio", Name = "Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio", NameType = typeof(AutomatizacaoGeracaoRelatorio))]
    public class AutomatizacaoGeracaoRelatorio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AGR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Relatorio", Column = "REL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Relatorio Relatorio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AutomatizacaoGeracaoRelatorioDadosConsulta", Column = "ADC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.False, Cascade = "delete")]
        public virtual AutomatizacaoGeracaoRelatorioDadosConsulta DadosConsulta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_ENVIAR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_ENVIAR_PARA_FTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarParaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_TIPO_ARQUIVO", TypeType = typeof(TipoArquivoGeracaoRelatorio), NotNull = true)]
        public virtual TipoArquivoGeracaoRelatorio TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_OCORRENCIA_GERACAO", TypeType = typeof(OcorrenciaGeracaoRelatorio), NotNull = true)]
        public virtual OcorrenciaGeracaoRelatorio OcorrenciaGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_HORA_GERACAO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_DIA_GERACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_SEGUNDA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarSegunda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_TERCA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarTerca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_QUARTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarQuarta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_QUINTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarQuinta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_SEXTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarSexta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_SABADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarSabado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_DOMINGO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarDomingo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_GERAR_SOMENTE_EM_DIA_UTIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarSomenteEmDiaUtil { get; set; }

        /// <summary>
        /// Não alterar. Data utilizada para a atualização dos filtros de pesquisa ao gerar o relatório.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_DATA_BASE", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AGR_DATA_PROXIMA_GERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataProximaGeracao { get; set; }
    }
}
