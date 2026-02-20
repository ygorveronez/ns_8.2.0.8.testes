using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DISPONIBILIDADE_VEICULO", EntityName = "DisponibilidadeVeiculo", Name = "Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo", NameType = typeof(DisponibilidadeVeiculo))]
    public class DisponibilidadeVeiculo : EntidadeBase
    {
        public DisponibilidadeVeiculo() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DVE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "DVE_EM_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        //public virtual DateTime? EmViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DVE_DISPONIVEL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Disponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DVE_PREVISAO_DISPONIBILIDADE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PrevisaoDisponibilidade { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Disponibilidade de Veículo";
            }
        }
    }
}