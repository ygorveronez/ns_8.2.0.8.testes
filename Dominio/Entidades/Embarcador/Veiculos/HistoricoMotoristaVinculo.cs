using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_MOTORISTA_VINCULO", EntityName = "HistoricoMotoristaVinculo", Name = "Dominio.Entidades.Embarcador.Transportadores.HistoricoMotoristaVinculo", NameType = typeof(HistoricoMotoristaVinculo))]
    public class HistoricoMotoristaVinculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HMV_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_USUARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasVinculado", Column = "HMV_DIAS_VINCULADO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVinculado { get; set; }

    }
}