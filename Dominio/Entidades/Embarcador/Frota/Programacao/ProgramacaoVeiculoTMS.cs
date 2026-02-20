using System;

namespace Dominio.Entidades.Embarcador.Frota.Programacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROGRAMACAO_VEICULO_TMS", EntityName = "ProgramacaoVeiculoTMS", Name = "Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculoTMS", NameType = typeof(ProgramacaoVeiculoTMS))]
    public class ProgramacaoVeiculoTMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDisponivelInicio", Column = "PVT_DATA_DISPONIVEL_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDisponivelInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDisponivel", Column = "PVT_DATA_DISPONIVEL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacaoPlanejamento", Column = "PVT_DATA_CRIACAO_PLANEJAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacaoPlanejamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade CidadeEstado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoSituacaoTMS", Column = "PST_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoSituacaoTMS ProgramacaoSituacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Folga", Column = "PVT_FOLGA", TypeType = typeof(int), NotNull = false)]
        public virtual int Folga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PVT_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 300)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Veiculo?.Placa ?? string.Empty;
            }
        }
    }
}
