namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_BAIXA_AGRUPADO_DOCUMENTO_ACRESCIMO_DESCONTO", EntityName = "TituloBaixaAgrupadoDocumentoAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto", NameType = typeof(TituloBaixaAgrupadoDocumentoAcrescimoDesconto))]
    public class TituloBaixaAgrupadoDocumentoAcrescimoDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TituloBaixaAgrupadoDocumento", Column = "TBD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento TituloBaixaAgrupadoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoJustificativa", Column = "TAD_TIPO_JUSTIFICATIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa TipoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAD_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAD_VALOR_MOEDA", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TAD_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        /// <summary>
        /// Indica se é referente à variação cambial, pois é um registro de controle do sistema, não podendo ser alterado operacionalmente
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TAD_VARIACAO_CAMBIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VariacaoCambial { get; set; }

        public virtual string DescricaoTipoJustificativa
        {
            get
            {
                switch (TipoJustificativa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto:
                        return "Desconto";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo:
                        return "Acréscimo";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
