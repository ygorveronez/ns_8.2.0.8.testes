using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_AGRUPADO", EntityName = "TituloBaixaAgrupado", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado", NameType = typeof(TituloBaixaAgrupado))]
    public class TituloBaixaAgrupado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixa", Column = "TIB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixa TituloBaixa { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_DATA_BASE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_DATA_BAIXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_PAGO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_ACRESCIMO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }

        /// <summary>
        /// Valor em Título + Acréscimo + Desconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_TOTAL_A_PAGAR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalAPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_PAGO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPagoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_DESCONTO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDescontoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_ACRESCIMO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimoMoeda { get; set; }

        /// <summary>
        /// Valor em Título + Acréscimo + Desconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TIA_VALOR_TOTAL_A_PAGAR_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalAPagarMoeda { get; set; }

        public virtual bool Equals(TituloBaixaAgrupado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
        public virtual string Descricao
        {
            get
            {
                return this.Titulo?.Descricao ?? string.Empty;
            }
        }
    }
}
