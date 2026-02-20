using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COBRANCA_MANUAL", EntityName = "CobrancaManual", Name = "Dominio.Entidades.Embarcador.Financeiro.CobrancaManual", NameType = typeof(CobrancaManual))]
    public class CobrancaManual : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.CobrancaManual>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ocorrencia", Column = "CMA_OCORRENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDocumento", Column = "CMA_VALOR_DOCUMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorJuros", Column = "CMA_VALOR_JUROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorJuros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CMA_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "CMA_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CMA_OBSERVACAO", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "CMA_DOCUMENTO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "CMA_TIPO_DOCUMENTO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTitulo", Column = "CMA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo TipoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_DOCUMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoValorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_JUROS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoValorJuros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "CMA_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo FormaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMA_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Provisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMA_TITULO_JA_FOI_PAGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TituloJaFoiPago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMA_REPETIR_LANCAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RepetirLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMA_DIVIDIR_LANCAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DividirLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMA_SIMULAR_PARCELAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SimularParcelas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Periodicidade", Column = "CMA_PERIODICIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Periodicidade), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.Periodicidade Periodicidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaVencimento", Column = "CMA_DIA_VENCIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "CMA_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "CMA_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "CMA_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "CMA_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Titulos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Titulo", Column = "TIT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Financeiro.Titulo> Titulos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(CobrancaManual other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
