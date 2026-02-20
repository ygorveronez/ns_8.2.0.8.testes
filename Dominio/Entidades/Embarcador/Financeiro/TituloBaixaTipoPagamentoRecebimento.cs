using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_TIPO_PAGAMENTO_RECEBIMENTO", EntityName = "TituloBaixaTipoPagamentoRecebimento", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento", NameType = typeof(TituloBaixaTipoPagamentoRecebimento))]
    public class TituloBaixaTipoPagamentoRecebimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixa", Column = "TIB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixa TituloBaixa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento TipoPagamentoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TTP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "TPR_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "TPR_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "TPR_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "TPR_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TituloBaixa?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(TituloBaixaTipoPagamentoRecebimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
