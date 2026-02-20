using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOCAL_ARMAZENAMENTO_HISTORICO", EntityName = "LocalArmazenamentoHistorico", Name = "Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico", NameType = typeof(LocalArmazenamentoHistorico))]
    public class LocalArmazenamentoHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Produtos.LocalArmazenamentoProduto LocalArmazenamentoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentacaoAbastecimentoSaida", Column = "MAS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida MovimentacaoAbastecimentoSaida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MovimentoEntradaTanque", Column = "MET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.AbastecimentoInterno.MovimentoEntradaTanque MovimentoEntradaTanque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentacao", Column = "ALO_TIPO_ACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoAbastecimento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoAbastecimento TipoMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAH_SALDO_ANTERIOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? SaldoAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAH_SALDO_ATUAL", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal SaldoAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "LAH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        public virtual string DescricaoTipoMovimentacao
        {
            get
            {
                switch (this.TipoMovimentacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoAbastecimento.Entrada:
                        return "Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoAbastecimento.Saida:
                        return "Saída";
                    default:
                        return "";
                }
            }
        }
        public virtual decimal QuantidadeMovimento
        {
            get {
                switch (this.TipoMovimentacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoAbastecimento.Entrada:
                        return this.SaldoAtual - (this.SaldoAnterior ?? 0);
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoAbastecimento.Saida:
                        return (this.SaldoAnterior ?? 0) - this.SaldoAtual;
                    default:
                        return 0;
                }
            }
        }

    }
}
