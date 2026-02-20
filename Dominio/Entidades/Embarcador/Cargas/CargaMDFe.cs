namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE", EntityName = "CargaMDFe", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFe", NameType = typeof(CargaMDFe))]
    public class CargaMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscaisManual", Column = "MDM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual MDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaEmissor", Column = "PCO_SISTEMA_EMISSOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor SistemaEmissor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaLocaisPrestacao", Column = "CLP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaLocaisPrestacao CargaLocaisPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MDFeAnteriorCargaParcial", Column = "CMD_MDFE_ANTERIOR_CARGA_PARCIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool MDFeAnteriorCargaParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmEncerramento", Column = "CMD_EM_ENCERRAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmEncerramento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Carga?.Descricao ?? string.Empty) + " - " + (this.MDFe?.Descricao ?? string.Empty);
            }
        }
    }
}
