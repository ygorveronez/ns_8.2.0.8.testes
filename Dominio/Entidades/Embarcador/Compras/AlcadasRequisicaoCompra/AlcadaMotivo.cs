namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_REQUISICAO_MERCADORIA_MOTIVO", EntityName = "AlcadaMotivo", Name = "Dominio.Entidades.Embarcador.Compras.AlcadaMotivo", NameType = typeof(AlcadaMotivo))]
    public class AlcadaMotivo : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasRequisicaoMercadoria", Column = "RRM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasRequisicaoMercadoria RegrasRequisicaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCompra", Column = "MCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.MotivoCompra Motivo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Motivo?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Embarcador.Compras.MotivoCompra PropriedadeAlcada
        {
            get
            {
                return this.Motivo;
            }
            set
            {
                this.Motivo = value;
            }
        }
    }
}