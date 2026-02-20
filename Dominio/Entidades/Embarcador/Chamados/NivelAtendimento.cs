using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NIVEL_ATENDIMENTO", EntityName = "NivelAtendimento", Name = "Dominio.Entidades.Embarcador.Chamados.NivelAtendimento", NameType = typeof(NivelAtendimento))]
    public class NivelAtendimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NAT_NIVEL", TypeType = typeof(EscalationList), NotNull = false)]
        public virtual EscalationList Nivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NAT_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NAT_DATA_LIMITE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLimite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NAT_NOTIFICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FoiNotificado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamado Chamado { get; set; }

    }
}
