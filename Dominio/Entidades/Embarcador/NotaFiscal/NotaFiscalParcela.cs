using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_PARCELA", EntityName = "NotaFiscalParcela", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela", NameType = typeof(NotaFiscalParcela))]
    public class NotaFiscalParcela : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "NFP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "NFP_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimo", Column = "NFP_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Acrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NFP_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "NFP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "NFP_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Forma", Column = "NFP_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo Forma { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "NFP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        public virtual bool Equals(NotaFiscalParcela other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto:
                        return "Em Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.Atrazada:
                        return "Atrasada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.Quitada:
                        return "Quitada";
                    default:
                        return "";
                }
            }
        }

        public virtual Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela Clonar()
        {
            return (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela)this.MemberwiseClone();
        }
    }
}
