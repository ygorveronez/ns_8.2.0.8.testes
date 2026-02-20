using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FINANCIAMENTO_PARCELA", EntityName = "ContratoFinanciamentoParcela", Name = "Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela", NameType = typeof(ContratoFinanciamentoParcela))]
    public class ContratoFinanciamentoParcela : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "CFP_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "CFP_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "CFP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CFP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAcrescimo", Column = "CFP_VALOR_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CFP_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarras", Column = "CFP_CODIGO_BARRAS", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CodigoBarras { get; set; }        

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFinanciamento", Column = "CFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFinanciamento ContratoFinanciamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FINANCIAMENTO_PARCELA_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFinanciamentoParcelaValor", Column = "CPV_CODIGO")]
        public virtual IList<ContratoFinanciamentoParcelaValor> Valores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontos", Formula = @" (SELECT ISNULL(SUM(CASE WHEN V.CPV_TIPO = 1 THEN V.CPV_VALOR ELSE 0 END), 0)
                                                                                        FROM T_CONTRATO_FINANCIAMENTO_PARCELA P
                                                                                        JOIN T_CONTRATO_FINANCIAMENTO_PARCELA_VALOR V ON V.CFP_CODIGO = P.CFP_CODIGO
                                                                                        WHERE P.CFP_CODIGO = CFP_CODIGO) ", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorDescontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAcrescimos", Formula = @" (SELECT ISNULL(SUM(CASE WHEN V.CPV_TIPO = 2 THEN V.CPV_VALOR ELSE 0 END), 0)
                                                                                        FROM T_CONTRATO_FINANCIAMENTO_PARCELA P
                                                                                        JOIN T_CONTRATO_FINANCIAMENTO_PARCELA_VALOR V ON V.CFP_CODIGO = P.CFP_CODIGO
                                                                                        WHERE P.CFP_CODIGO = CFP_CODIGO) ", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorAcrescimos { get; set; }

        public virtual decimal DemaisValores
        {
            get { return this.ValorAcrescimos - this.ValorDescontos; }
        }

        public virtual bool Equals(ContratoFinanciamentoParcela other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
