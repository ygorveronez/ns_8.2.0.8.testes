namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_ENVIO_INTEGRADOR", EntityName = "FilaEnvioIntegrador", Name = "Dominio.Entidades.FilaEnvioIntegrador", NameType = typeof(FilaEnvioIntegrador))]
    public class FilaEnvioIntegrador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FEI_CODIGO_TIPO_ENVIO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoTipoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FEI_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual FilaEnvioIntegrador Clonar()
        {
            return (FilaEnvioIntegrador)this.MemberwiseClone();
        }
    }
}
