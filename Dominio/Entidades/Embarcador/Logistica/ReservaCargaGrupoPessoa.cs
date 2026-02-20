using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RESERVA_CARGA_GRUPO_PESSOA", EntityName = "ReservaCargaGrupoPessoa", Name = "Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa", NameType = typeof(ReservaCargaGrupoPessoa))]
    public class ReservaCargaGrupoPessoa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0,  Column = "RCG_DATA_RESERVA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataReserva { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PrevisaoCarregamento", Column = "PRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento PrevisaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCG_QUANTIDADE_RESERVADA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeReservada { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.GrupoPessoas?.Descricao ?? string.Empty;
            }
        }
    }
}
