using System;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_VEICULO_VINCULO", EntityName = "HistoricoVeiculoVinculo", Name = "Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo", NameType = typeof(HistoricoVeiculoVinculo))]
    public class HistoricoVeiculoVinculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HVV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "HVV_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KmAtualModificacao", Column = "HVV_KM_ATUAL_MODIFICACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int KmAtualModificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KmRodado", Column = "HVV_KM_RODADO", TypeType = typeof(int), NotNull = false)]
        public virtual int KmRodado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasVinculado", Column = "HVV_DIAS_VINCULADO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasVinculado { get; set; }

    }
}
