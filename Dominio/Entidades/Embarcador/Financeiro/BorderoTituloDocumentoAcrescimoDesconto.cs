namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BORDERO_TITULO_DOCUMENTO_ACRESCIMO_DESCONTO", EntityName = "BorderoTituloDocumentoAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto", NameType = typeof(BorderoTituloDocumentoAcrescimoDesconto))]
    public class BorderoTituloDocumentoAcrescimoDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BorderoTituloDocumento", Column = "BTD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BorderoTituloDocumento BorderoTituloDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BAD_TIPO_JUSTIFICATIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa TipoJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BAD_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BAD_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DescricaoTipoJustificativa;
            }
        }

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
