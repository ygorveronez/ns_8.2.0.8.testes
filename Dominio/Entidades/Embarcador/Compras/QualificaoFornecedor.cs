using System;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_QUALIFICACAO_FORNECEDOR", EntityName = "QualificaoFornecedor", Name = "Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor", NameType = typeof(QualificaoFornecedor))]
    public class QualificaoFornecedor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "QFO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompra", Column = "ORC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.OrdemCompra OrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QFO_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataQualificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QFO_CRITERIO_PRAZO_ENTREGA_PONTUALIDADE", TypeType = typeof(int))]
        public virtual int CriterioPrazoEntregaPontualidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QFO_CRITERIO_CARACTERISTICA_ESPECIFICACOES", TypeType = typeof(int))]
        public virtual int CriterioCaracteristicaEspecificacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QFO_CRITERIO_QUANTIDADE_RECEBIDA", TypeType = typeof(int))]
        public virtual int CriterioQuantidadeRecebida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QFO_CRITERIO_INTEGRIDADE_FISICA", TypeType = typeof(int))]
        public virtual int CriterioIntegridadeFisica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "QFO_CRITERIO_ATENDIMENTO", TypeType = typeof(int))]
        public virtual int CriterioAtendimento { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Fornecedor?.Descricao ?? string.Empty;
            }
        }
    }
}
