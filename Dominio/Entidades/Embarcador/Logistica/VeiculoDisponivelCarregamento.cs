using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_DISPONIVEL_CARREGAMENTO", EntityName = "VeiculoDisponivelCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento", NameType = typeof(VeiculoDisponivelCarregamento))]
    public class VeiculoDisponivelCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIndisponibilizacaoVeiculo", Column = "VDC_DATA_INDISPONIBILIZACAO_VEICULO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataIndisponibilizacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDisponibilizacaoVeiculo", Column = "VDC_DATA_DISPONIBILIZACAO_VEICULO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDisponibilizacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_INDISPONIBILIZACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioIndisponibilizou { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_DISPONIBILIZOU", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioDisponibilizou { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Disponivel", Column = "VDC_DISPONIVEL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Disponivel { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Veiculo?.Descricao ?? string.Empty;
            }
        }

    }
}
