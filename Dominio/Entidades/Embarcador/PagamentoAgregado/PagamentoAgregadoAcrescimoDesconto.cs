namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_AGREGADO_ACRESCIMO_DESCONTO", EntityName = "PagamentoAgregadoAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto", NameType = typeof(PagamentoAgregadoAcrescimoDesconto))]
    public class PagamentoAgregadoAcrescimoDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }      

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAD_VALOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PagamentoAgregado", Column = "PAA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado PagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "ContratoFreteAcrescimoDesconto", Column = "CAD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto ContratoFreteAcrescimoDesconto { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
