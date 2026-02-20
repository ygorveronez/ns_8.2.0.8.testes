using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_CTE_EMITIDO_FORA_EMBARCADOR", EntityName = "ImportacaoCTeEmitidoForaEmbarcador", Name = "Dominio.Entidades.Embarcador.CTe.ImportacaoCTeEmitidoForaEmbarcador", NameType = typeof(ImportacaoCTeEmitidoForaEmbarcador))]
    public class ImportacaoCTeEmitidoForaEmbarcador : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Planilha", Column = "ICF_PLANILHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Planilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLinhas", Column = "ICF_QTDE_LINHAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeLinhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "ICF_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ICF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoCTeEmitidoForaEmbarcador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "ICF_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioProcessamento", Column = "ICF_DATA_INICIO_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimProcessamento", Column = "ICF_DATA_FIM_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalSegundosProcessamento", Column = "ICF_TOTAL_SEGUNDOS_PROCESSAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? TotalSegundosProcessamento { get; set; }

        public virtual TimeSpan? Tempo()
        {
            if (TotalSegundosProcessamento != null)
                return TimeSpan.FromSeconds(TotalSegundosProcessamento.Value);
            else
                return null;
        }

        public virtual string Descricao
        {
            get
            {
                return Planilha;
            }
        }

    }
}
