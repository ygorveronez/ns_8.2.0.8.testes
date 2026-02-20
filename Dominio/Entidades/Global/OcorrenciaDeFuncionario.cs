using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_MOTORISTA", EntityName = "OcorrenciaDeFuncionario", Name = "Dominio.Entidades.OcorrenciaDeFuncionario", NameType = typeof(OcorrenciaDeFuncionario))]
    public class OcorrenciaDeFuncionario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrencia", Column = "HIT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrencia TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeCadastro", Column = "HIM_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDeCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "HIM_DESCRICAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDaOcorrencia", Column = "HIM_DATA_HIST", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "HIM_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoDeOcorrencia
        {
            get
            {
                return this.TipoDeOcorrencia != null ? this.TipoDeOcorrencia.Descricao : string.Empty;
            }
        }

        public virtual string DescricaoVeiculo
        {
            get
            {
                return this.Veiculo != null ? this.Veiculo.Placa : string.Empty;
            }
        }

        public virtual string DescricaoFuncionario
        {
            get
            {
                return this.Funcionario != null ? string.Concat(this.Funcionario.CPF, " - ", this.Funcionario.Nome) : string.Empty;
            }
        }
    }
}
