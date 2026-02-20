using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.AbastecimentoInterno
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTO_ENTRADA_TANQUE", EntityName = "MovimentoEntradaTanque", Name = "Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque", NameType = typeof(MovimentoEntradaTanque))]
    public class MovimentoEntradaTanque : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MET_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MET_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraNotaFiscal", Column = "MET_DATA_HORA_NOTA_FISCAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHoraNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Produtos.LocalArmazenamentoProduto LocalArmazenamentoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOleo", Column = "TOL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frotas.TipoOleo TipoOleo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraEntrada", Column = "MET_DATA_HORA_ENTRADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHoraEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MET_QUANTIDADE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QuantidadeLitros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MET_SALDO_INICIAL_MOVIMENTACAO_ENTRADA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? SaldoInicialAntesMovimentacaoEntrada { get; set; }
    }
}