using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TABELA_FRETE_CLIENTE", EntityName = "CargaTabelaFreteCliente", Name = "Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente", NameType = typeof(CargaTabelaFreteCliente))]
    public class CargaTabelaFreteCliente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CTC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixo", Column = "CTC_VALOR_FIXO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSobreNF", Column = "CTC_PERC_NOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualSobreNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CTC_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_OBSERVACAO_TERCEIRO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabelaFreteFilialEmissora", Column = "CTC_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTC_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente)this.MemberwiseClone();
        }

        //#region Propriedades Obsoletas

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "CotacaoMoedaUtilizado", Column = "CTC_COTACAO_MOEDA_UTILIZADO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = true)]
        //public virtual decimal CotacaoMoedaUtilizado { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cotacao", Column = "MCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Embarcador.Moedas.Cotacao Cotacao { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalNFs", Column = "CTC_VALOR_TOTAL_NFS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        //public virtual decimal ValorTotalNFs { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdValorem", Column = "CTC_VALOR_AD_VALOREM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        //public virtual decimal ValorAdValorem { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoFrete", Column = "CTC_VALOR_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        //public virtual decimal ValorMinimoFrete { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "CTC_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        //public virtual decimal ValorPedagio { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescarga", Column = "CTC_VALOR_DESCARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        //public virtual decimal ValorDescarga { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "CTC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? DataInicio { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "CTC_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? DataFim { get; set; }

        //[Obsolete("Será removida. Não utilizar.")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CTC_TIPO", TypeType = typeof(Enumeradores.TipoFreteValor), NotNull = true)]
        //public virtual Enumeradores.TipoFreteValor Tipo { get; set; }

        //#endregion

        public virtual bool Equals(CargaTabelaFreteCliente other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
