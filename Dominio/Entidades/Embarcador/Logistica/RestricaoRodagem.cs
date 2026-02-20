using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RESTRICAO_RODAGEM", EntityName = "RestricaoRodagem", Name = "Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem", NameType = typeof(RestricaoRodagem))]
    public class RestricaoRodagem : EntidadeBase, IEquatable<RestricaoRodagem>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaSemana", Column = "RRO_DIA_SEMANA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana DiaSemana { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalPlaca", Column = "RRO_FINAL_PLACA", TypeType = typeof(int), NotNull = true)]
        public virtual int FinalPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRO_HORA_INCIAL", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRO_HORA_FINAL", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RESTRICAO_RODAGEM_CLIENTE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(RestricaoRodagem other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
