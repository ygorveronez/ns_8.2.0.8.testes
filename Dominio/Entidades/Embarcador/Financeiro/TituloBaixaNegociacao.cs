using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_NEGOCIACAO", EntityName = "TituloBaixaNegociacao", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao", NameType = typeof(TituloBaixaNegociacao))]
    public class TituloBaixaNegociacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TBN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixa", Column = "TIB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixa TituloBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TBN_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "TBN_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimo", Column = "TBN_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Acrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "TBN_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "TBN_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "TBN_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBoleto", Column = "TBN_NUMERO_BOLETO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoFaturaParcela", Column = "TBN_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela SituacaoFaturaParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaParcela", Column = "TBN_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo FormaParcela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_PORTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Portador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "TBN_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TBN_VALOR_AJUSTADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorAjustadoManualmente { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTitulo", Formula = @"ISNULL((SELECT TOP 1 ISNULL(T.TIT_CODIGO, 0) FROM T_TITULO T WHERE T.TBN_CODIGO = TBN_CODIGO), 0)", TypeType = typeof(int), Lazy = true)]
        public virtual int CodigoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTitulo", Formula = @"ISNULL((SELECT TOP 1 ISNULL(T.TIT_STATUS, 1) FROM T_TITULO T WHERE T.TBN_CODIGO = TBN_CODIGO), 1)", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo), Lazy = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo StatusTitulo { get; set; }

        public virtual string Descricao
        {
            get { return this.Sequencia.ToString() + " - " + (this.TituloBaixa.Descricao); }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                if (this.CodigoTitulo > 0)
                {
                    return this.StatusTitulo.ObterDescricao();
                }
                else
                {
                    switch (this.SituacaoFaturaParcela)
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
        }

        public virtual bool Equals(TituloBaixaNegociacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
