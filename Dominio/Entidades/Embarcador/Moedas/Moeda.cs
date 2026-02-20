namespace Dominio.Entidades.Embarcador.Moedas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOEDA", EntityName = "Moeda", Name = "Dominio.Entidades.Embarcador.Moedas.Moeda", NameType = typeof(Moeda))]
    public class Moeda : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Simbolo", Column = "MDA_SIMBOLO", TypeType = typeof(string), NotNull = false)]
        public virtual string Simbolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MDA_DESCRICAO", TypeType = typeof(string), NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMoeda", Column = "MDA_CODIGO_MOEDA", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "MDA_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        public virtual bool Equals(Moeda other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
