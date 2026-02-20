namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILIAL_MODELO_VEICULAR_CARGA", EntityName = "FilialModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga", NameType = typeof(FilialModeloVeicularCarga))]
    public class FilialModeloVeicularCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarOrdemEmbarque", Column = "FMV_INTEGRAR_ORDEM_EMBARQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarOrdemEmbarque { get; set; }

        public virtual string Descricao
        {
            get { return $"Configuração do Modelo Veicular de Carga {ModeloVeicularCarga.Descricao}"; }
        }
    }
}
