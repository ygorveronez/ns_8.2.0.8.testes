using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LEILAO_PARTICIPANTE", EntityName = "LeilaoParticipante", Name = "Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante", NameType = typeof(LeilaoParticipante))]
    public class LeilaoParticipante : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Leilao", Column = "LEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Leilao Leilao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLance", Column = "LEP_DATA_LANCE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLance { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLance", Column = "LEP_VALOR_LANCE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLance { get; set; }

        public virtual bool Equals(LeilaoParticipante other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
