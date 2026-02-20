using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_VEICULAR_CARGA_ESTEPE", EntityName = "ModeloVeicularCargaEstepe", Name = "Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe", NameType = typeof(ModeloVeicularCargaEstepe))]
    public class ModeloVeicularCargaEstepe : EntidadeBase, IEquatable<ModeloVeicularCargaEstepe>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MES_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public virtual string Descricao
        {
            get { return $"Estepe {Numero}"; }
        }

        public virtual bool Equals(ModeloVeicularCargaEstepe other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
