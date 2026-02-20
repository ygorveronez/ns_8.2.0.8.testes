using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.AbastecimentoInterno
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTACAO_ABASTECIMENTO_SAIDA", EntityName = "MovimentacaoAbastecimentoSaida", Name = "Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida", NameType = typeof(MovimentacaoAbastecimentoSaida))]
    public class MovimentacaoAbastecimentoSaida : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "MAS_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAbastecimento", Column = "MAS_TIPO_ABASTECIMENTO", TypeType = typeof(ModoAbastecimento), NotNull = false)]
        public virtual ModoAbastecimento TipoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Produtos.LocalArmazenamentoProduto LocalArmazenamentoProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BombaAbastecimento", Column = "ABB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frotas.BombaAbastecimento BombaAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOleo", Column = "TOL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frotas.TipoOleo TipoOleo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MAS_QUANTIDADE_LITROS", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal QuantidadeLitros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MAS_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MAS_HODOMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Hodometro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MAS_QUANTIDADE_ARLA32", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QuantidadeArla32 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MET_SALDO_INICIAL_MOVIMENTACAO_SAIDA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? SaldoInicialAntesMovimentacaoSaida { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoTipoAbastecimento => TipoAbastecimento.ObterDescricao();

        #endregion
    }
}