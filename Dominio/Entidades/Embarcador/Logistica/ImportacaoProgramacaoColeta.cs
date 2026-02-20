using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPORTACAO_PROGRAMACAO_COLETA", EntityName = "ImportacaoProgramacaoColeta", Name = "Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta", NameType = typeof(ImportacaoProgramacaoColeta))]
    public class ImportacaoProgramacaoColeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroImportacao", Column = "IPC_NUMERO_IMPORTACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRepeticoes", Column = "IPC_NUMERO_REPETICOES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroRepeticoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntervaloDiasGeracao", Column = "IPC_INTERVALO_DIAS_GERACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloDiasGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoImportacaoProgramacaoColeta", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoProgramacaoColeta), Column = "IPC_SITUACAO_IMPORTACAO_PROGRAMACAO_COLETA", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoProgramacaoColeta SituacaoImportacaoProgramacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "IPC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeRepeticoesEfetuadas", Column = "IPC_QUANTIDADE_REPETICOES_EFETUADAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeRepeticoesEfetuadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaGeracaoAutomatica", Column = "IPC_DATA_ULTIMA_GERACAO_AUTOMATICA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaGeracaoAutomatica { get; set; }

        public virtual string Descricao
        {
            get { return NumeroImportacao.ToString(); }
        }
    }
}