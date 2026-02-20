using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_ESTOQUE", EntityName = "EstoquePallet", Name = "Dominio.Entidades.Embarcador.Pallets.EstoquePallet", NameType = typeof(EstoquePallet))]
    public class EstoquePallet : EntidadeBase
    {
        #region Propriedades Públicas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PES_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaturezaMovimentacao", Column = "PES_NATUREZA_MOVIMENTACAO", TypeType = typeof(NaturezaMovimentacaoEstoquePallet), NotNull = true)]
        public virtual NaturezaMovimentacaoEstoquePallet NaturezaMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PES_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PES_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDescartada", Column = "PES_QUANTIDADE_DESCARTADA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDescartada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoTotal", Column = "PES_SALDO_TOTAL", TypeType = typeof(int), NotNull = true)]
        public virtual int SaldoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLancamento", Column = "PES_TIPO_LANCAMENTO", TypeType = typeof(TipoLancamento), NotNull = true)]
        public virtual TipoLancamento TipoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentacao", Column = "PES_TIPO_MOVIMENTACAO", TypeType = typeof(TipoMovimentacaoEstoquePallet), NotNull = true)]
        public virtual TipoMovimentacaoEstoquePallet TipoMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DevolucaoPallet", Column = "PDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DevolucaoPallet Devolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoBaixaPallets", Column = "CBP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CancelamentoBaixaPallets CancelamentoBaixaPallets { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Setor", Column = "SET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Setor Setor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoDevolucaoPallet", Column = "PSD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SituacaoDevolucaoPallet SituacaoDevolucaoPallet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPallets", Column = "FEP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoPallets Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PES_ADICIONAR_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarAoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        public virtual string Descricao
        {
            get {
                if (Cliente != null)
                    return Cliente.Descricao;

                if (Filial != null)
                    return Filial.Descricao;

                if (Transportador != null)
                    return Transportador.Descricao;

                return string.Empty;
            }
        }

        #endregion

        #region Métodos Públicos

        public virtual int ObterQuantidadeEntrada()
        {
            return TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada ? Quantidade : 0;
        }

        public virtual int ObterQuantidadeSaida()
        {
            return TipoMovimentacao == TipoMovimentacaoEstoquePallet.Saida ? Quantidade : 0;
        }

        public virtual string ObterTipoLancamento()
        {
            return TipoLancamento.ObterDescricao();
        }

        #endregion
    }
}
