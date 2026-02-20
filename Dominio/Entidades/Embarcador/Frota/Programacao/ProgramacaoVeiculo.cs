using System;

namespace Dominio.Entidades.Embarcador.Frota.Programacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROGRAMACAO_VEICULO", EntityName = "ProgramacaoVeiculo", Name = "Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoVeiculo", NameType = typeof(ProgramacaoVeiculo))]
    public class ProgramacaoVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pallets", Column = "PVE_PALLETS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Pallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "PVE_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTermino", Column = "PVE_DATA_TERMINO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTermino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoSituacao", Column = "PSI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoSituacao ProgramacaoSituacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoEspecialidade", Column = "PES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoEspecialidade ProgramacaoEspecialidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoAlocacao", Column = "PAL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoAlocacao ProgramacaoAlocacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoLicenciamento", Column = "PLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoLicenciamento ProgramacaoLicenciamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Veiculo?.Placa ?? string.Empty;
            }
        }
    }
}
