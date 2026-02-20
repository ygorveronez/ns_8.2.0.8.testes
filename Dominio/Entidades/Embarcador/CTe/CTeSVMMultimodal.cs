namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_SVM_MULTIMODAL", EntityName = "CTeSVMMultimodal", Name = "Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal", NameType = typeof(CTeSVMMultimodal))]
    public class CTeSVMMultimodal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        /// <summary>
        /// CT-e SVM
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO_SVM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTeSVM { get; set; }

        /// <summary>
        /// CT-e Multimodal
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO_MULTIMODAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTeMultimodal { get; set; }

        /// <summary>
        /// Carga Multimodal
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_MULTIMODAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga CargaMultimodal { get; set; }
    }
}
