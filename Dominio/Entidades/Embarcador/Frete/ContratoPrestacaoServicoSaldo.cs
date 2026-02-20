using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_PRESTACAO_SERVICO_SALDO", EntityName = "ContratoPrestacaoServicoSaldo", Name = "Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo", NameType = typeof(ContratoPrestacaoServicoSaldo))]
    public class ContratoPrestacaoServicoSaldo : EntidadeBase, IEntidade, IEquatable<ContratoPrestacaoServicoSaldo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CSS_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CSS_DESCRICAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLancamento", Column = "CSS_TIPO_LANCAMENTO", TypeType = typeof(TipoLancamento), NotNull = true)]
        public virtual TipoLancamento TipoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentacao", Column = "CSS_TIPO_MOVIMENTACAO", TypeType = typeof(TipoMovimentacaoContratoPrestacaoServico), NotNull = true)]
        public virtual TipoMovimentacaoContratoPrestacaoServico TipoMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSS_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSS_VALOR_UTILIZADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorUtilizado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoPrestacaoServico", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoPrestacaoServico ContratoPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public virtual bool Equals(ContratoPrestacaoServicoSaldo other)
        {
            return (this.Codigo == other.Codigo);
        }

        public virtual decimal ObterValorDisponivel()
        {
            return ContratoPrestacaoServico.ValorTeto - ValorUtilizado;
        }

        public virtual decimal ObterValorEntrada()
        {
            return TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Entrada ? Valor : 0m;
        }

        public virtual decimal ObterValorSaida()
        {
            return TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Saida ? Valor : 0m;
        }
    }
}
