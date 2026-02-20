using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_DETALHE_CONHECIMENTO", EntityName = "TituloBaixaDetalheConhecimento", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento", NameType = typeof(TituloBaixaDetalheConhecimento))]
    public class TituloBaixaDetalheConhecimento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDetalheConhecimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixa", Column = "TIB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixa TituloBaixa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_DESCONTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDesconto", Column = "TDC_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_ACRESCIMO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAcrescimo", Column = "TDC_VALOR_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPago", Column = "TDC_VALOR_PAGO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TDC_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Observacao { get; set; }

        public virtual bool Equals(TituloBaixaDetalheConhecimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
