using System;

namespace Dominio.Entidades.Embarcador.Frota.Programacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROGRAMACAO_MOTORISTA", EntityName = "ProgramacaoMotorista", Name = "Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista", NameType = typeof(ProgramacaoMotorista))]
    public class ProgramacaoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioFerias", Column = "PMO_DATA_INICIO_FERIAS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioFerias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimFerias", Column = "PMO_DATA_FIM_FERIAS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimFerias { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoSituacao", Column = "PSI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoSituacao ProgramacaoSituacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoEspecialidade", Column = "PES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoEspecialidade ProgramacaoEspecialidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProgramacaoAlocacao", Column = "PAL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProgramacaoAlocacao ProgramacaoAlocacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
