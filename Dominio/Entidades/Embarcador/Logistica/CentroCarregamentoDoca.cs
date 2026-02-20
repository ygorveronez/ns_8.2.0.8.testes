using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_DOCA", EntityName = "CentroCarregamentoDoca", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca", NameType = typeof(CentroCarregamentoDoca))]
    public class CentroCarregamentoDoca : EntidadeBase, IEquatable<CentroCarregamentoDoca>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CCD_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CCD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CCD_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        public virtual bool Equals(CentroCarregamentoDoca other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
