using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_IMPORTACAO_PEDAGIO", EntityName = "ImportacaoPedagio", Name = "Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio", NameType = typeof(ImportacaoPedagio))]
    public class ImportacaoPedagio : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Planilha", Column = "IMP_PLANILHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Planilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLinhas", Column = "IMP_QTDE_LINHAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeLinhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "IMP_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "IMP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "IMP_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioProcessamento", Column = "IMP_DATA_INICIO_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimProcessamento", Column = "IMP_DATA_FIM_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimProcessamento { get; set; }

        public virtual TimeSpan? Tempo()
        {
            if (!DataFimProcessamento.HasValue)
                return null;

            return (DataFimProcessamento.Value - DataInicioProcessamento.Value);
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
