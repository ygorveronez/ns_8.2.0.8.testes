using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM", EntityName = "ItemParametroBaseCalculoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete", NameType = typeof(ItemParametroBaseCalculoTabelaFrete))]
    public class ItemParametroBaseCalculoTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametroBaseCalculoTabelaFrete", Column = "TBC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParametroBaseCalculoTabelaFrete ParametroBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObjeto", Column = "TPI_TIPO_OBJETO", TypeType = typeof(TipoParametroBaseTabelaFrete), NotNull = true)]
        public virtual TipoParametroBaseTabelaFrete TipoObjeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "TPI_TIPO_VALOR", TypeType = typeof(TipoCampoValorTabelaFrete), NotNull = true)]
        public virtual TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPI_SITUACAO", TypeType = typeof(SituacaoItemParametroBaseCalculoTabelaFrete), NotNull = false)]
        public virtual SituacaoItemParametroBaseCalculoTabelaFrete Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "StatusAssinaturaContrato", Column = "STC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual StatusAssinaturaContrato StatusAceiteValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteIntegracao", Column = "TPI_PENDENTE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoObjeto", Column = "TPI_CODIGO_OBJETO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoObjeto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornoIntegracao", Column = "TPI_RETORNO_INTEGRACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string RetornoIntegracao { get; set; }

        /// <summary>
        /// Campo utilizado para armazenar o valor atual. Para calculos utilizar o campo ValorParaCalculo
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TPI_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        /// <summary>
        /// Campo utilizado para armazenar o valor antigo (não é utilizado para cálculo).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginal", Column = "TPI_VALOR_ORIGINAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginal { get; set; }

        /// <summary>
        /// Campo utilizado para calculos.
        /// </summary>
        public virtual decimal ValorParaCalculo
        {
            get
            {
                return UtilizarValorOriginalParaCalculo ? ValorOriginal : Valor;
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.TipoObjeto.ObterDescricao();
            }
        }

        public virtual string ValorFormatado
        {
            get { return Valor.ToString("n6"); }
        }

        public virtual string ValorParaCalculoFormatado
        {
            get { return ValorParaCalculo.ToString("n6"); }
        }

        public virtual bool UtilizarValorOriginalParaCalculo
        {
            get { return (Situacao != SituacaoItemParametroBaseCalculoTabelaFrete.Ativo); }
        }

        public virtual ItemParametroBaseCalculoTabelaFrete Clonar()
        {
            return (ItemParametroBaseCalculoTabelaFrete)this.MemberwiseClone();
        }
    }
}
