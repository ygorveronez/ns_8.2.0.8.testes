using System;


namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LEILAO_PARTICIPANTE_HISTORICO_LANCE", EntityName = "LeilaoParticipanteHistoricoLance", Name = "Dominio.Entidades.Embarcador.Cargas.LeilaoParticipanteHistoricoLance", NameType = typeof(LeilaoParticipanteHistoricoLance))]
    public class LeilaoParticipanteHistoricoLance : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipanteHistoricoLance>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LHL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LeilaoParticipante", Column = "LEP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante LeilaoParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLance", Column = "LHL_DATA_LANCE", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLance { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLance", Column = "LHL_VALOR_LANCE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorLance { get; set; }


        public virtual bool Equals(LeilaoParticipanteHistoricoLance other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
