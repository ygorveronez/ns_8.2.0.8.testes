using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_IRREGULARRIDADE", EntityName = "HistoricoIrregularidade", Name = "Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade", NameType = typeof(HistoricoIrregularidade))]
    public class HistoricoIrregularidade : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HII_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleDocumento", Column = "COD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleDocumento ControleDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoIrregularidade", Column = "MTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade MotivoIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PortfolioModuloControle", Column = "PMC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle Porfolio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Irregularidade", Column = "IRR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HII_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HII_DATA_IRREGULARIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HII_SITUACAO_IRREGULARIDADE", TypeType = typeof(SituacaoIrregularidade), NotNull = false)]
        public virtual SituacaoIrregularidade SituacaoIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HII_SERVICO_RESPONSAVEL", TypeType = typeof(ServicoResponsavel), NotNull = false)]
        public virtual ServicoResponsavel ServicoResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HII_SEQUENCIA_TRATATIVA", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaTrataviva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HII_APROVADOR_IRREGULARIDADE", TypeType = typeof(AprovadorIrregularidade), NotNull = false)]
        public virtual AprovadorIrregularidade AprovadorIrregularidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HII_CODIGO_GRUPO_TIPO_OPERACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoGrupoTipoOperacao { get; set; }

    }
}
