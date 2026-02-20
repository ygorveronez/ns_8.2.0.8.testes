using System;

namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE_PARCELA", EntityName = "FechamentoFreteParcela", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela", NameType = typeof(FechamentoFreteParcela))]
    public class FechamentoFreteParcela : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FFP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "FFP_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimo", Column = "FFP_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Acrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "FFP_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "FFP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "FFP_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoParcela", Column = "FFP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela SituacaoParcela { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.SituacaoParcela)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.Atrazada:
                        return "Atrazada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto:
                        return "Em aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.Quitada:
                        return "Quitada";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.DataVencimento.ToString("dd/MM/yyyy");
            }
        }

        public virtual bool Equals(FechamentoFreteParcela other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
