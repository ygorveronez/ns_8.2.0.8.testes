using System;

namespace Dominio.Entidades.Embarcador.GestaoPallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTACAO_PALLET", EntityName = "MovimentacaoPallet", Name = "Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet", NameType = typeof(MovimentacaoPallet))]
    public class MovimentacaoPallet : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public MovimentacaoPallet()
        {
            DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleEstoquePallet", Column = "CPT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleEstoquePallet ControleEstoquePallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimentoNF", Column = "MPT_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimentoNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallets", Column = "MPT_QUANTIDADE_PALLETS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadePallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "MPT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoPallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoPallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ResponsavelMovimentacaoPallet", Column = "MPT_RESPONSAVEL_MOVIMENTACAO_PALLET", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet ResponsavelMovimentacaoPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPallet", Column = "MPT_REGRA_PALLET", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.RegraPallet), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.RegraPallet RegraPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MPT_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLancamento", Column = "MPT_TIPO_LANCAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento TipoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuebraRegraInformada", Column = "MPT_QUEBRA_REGRA_INFORMADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool QuebraRegraInformada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentacao", Column = "MPT_TIPO_MOVIMENTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida TipoMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "MPT_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial FilialDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucaoLaudo", Column = "GDL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Devolucao.GestaoDevolucaoLaudo Laudo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RecebimentoValePallet", Column = "RVP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RecebimentoValePallet ValePallet { get; set; }

        public virtual string Descricao
        {
            get { return $"Movimentação de Pallet"; }
        }
    }
}