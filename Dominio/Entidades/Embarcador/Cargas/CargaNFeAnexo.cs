namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_NFE_ANEXO", EntityName = "CargaNFeAnexo", Name = "Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo", NameType = typeof(CargaNFeAnexo))]
    public class CargaNFeAnexo : Anexo.Anexo<Carga>
    {
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Carga EntidadeAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcultarParaTransportador", Column = "CNA_OCULTAR_PARA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarParaTransportador { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo)this.MemberwiseClone();
        }
    }
}
