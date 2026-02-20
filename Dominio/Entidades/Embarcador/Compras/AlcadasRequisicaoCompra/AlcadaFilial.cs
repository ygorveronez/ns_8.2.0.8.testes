namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_REQUISICAO_MERCADORIA_FILIAL", EntityName = "Compras.AlcadaFilial", Name = "Dominio.Entidades.Embarcador.Compras.AlcadaFilial", NameType = typeof(AlcadaFilial))]
    public class AlcadaFilial : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasRequisicaoMercadoria", Column = "RRM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasRequisicaoMercadoria RegrasRequisicaoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Filial { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Filial?.RazaoSocial ?? string.Empty;
            }
        }

        public virtual Dominio.Entidades.Empresa PropriedadeAlcada
        {
            get
            {
                return this.Filial;
            }
            set
            {
                this.Filial = value;
            }
        }
    }
}