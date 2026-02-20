using System;

namespace Dominio.Entidades.Embarcador.Relatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RELATORIO_CONTROLE_GERACAO", EntityName = "RelatorioControleGeracao", Name = "Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao", NameType = typeof(RelatorioControleGeracao))]

    public class RelatorioControleGeracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Relatorio", Column = "REL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Relatorios.Relatorio Relatorio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RelatorioControleGeracaoDadosConsulta", Column = "RCD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.False, Cascade = "delete")]
        public virtual Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta DadosConsulta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AutomatizacaoGeracaoRelatorio", Column = "AGR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio AutomatizacaoGeracaoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_GUID_ARQUIVO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_TITULO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_DATA_INICIO_GERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicioGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_FINAL_GERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinalGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_TIPO_ARQUIVO_RELATORIO", TypeType = typeof(Enumeradores.TipoArquivoRelatorio), NotNull = true)]
        public virtual Enumeradores.TipoArquivoRelatorio TipoArquivoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_SITUACAO_GERACAO_RELATORIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio SituacaoGeracaoRelatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_CODIGO_ENTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoEntidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_GERAR_POR_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPorServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_TIPO_SERVICO_MULTISOFTWARE", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = false)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        public virtual bool Equals(RelatorioControleGeracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoTipoArquivo
        {
            get
            {
                if (this.TipoArquivoRelatorio == Enumeradores.TipoArquivoRelatorio.PDF)
                    return "PDF";

                if (this.TipoArquivoRelatorio == Enumeradores.TipoArquivoRelatorio.XLS)
                    return "Excel";

                if (this.TipoArquivoRelatorio == Enumeradores.TipoArquivoRelatorio.DOC)
                    return "Doc";

                if (this.TipoArquivoRelatorio == Enumeradores.TipoArquivoRelatorio.RTF)
                    return "Rtf";

                return "";

            }
        }


    }
}
