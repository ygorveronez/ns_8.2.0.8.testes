using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VEICULO_CUSTO_FIXO", EntityName = "CustoFixo", Name = "Dominio.Entidades.CustoFixo", NameType = typeof(CustoFixo))]
    public class CustoFixo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCustoFixo", Column = "TCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCustoFixo TipoDeCustoFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "VEF_DESCRICAO", TypeType = typeof(string), NotNull = true, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "VEF_VALOR_TOTAL", TypeType = typeof(decimal), NotNull = true, Scale = 6, Precision = 18)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rateio", Column = "VEF_RATEIO", TypeType = typeof(int), NotNull = true)]
        public virtual int Rateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "VEF_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "VEF_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VEF_STATUS", TypeType = typeof(string), NotNull = true, Length = 1)]
        public virtual string Status { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "I":
                        return "Inativo";
                    case "A":
                        return "Ativo";
                    default:
                        return "";
                }
            }
        }

        public virtual string PlacaVeiculo
        {
            get
            {
                if (this.Veiculo != null)
                    return this.Veiculo.Placa;
                else
                    return string.Empty;
            }
        }

        public virtual decimal ValorMensal
        {
            get
            {
                return this.Rateio > 0 ? this.ValorTotal / this.Rateio : 0;
            }
        }

        public virtual string DescricaoTipoCustoFixo
        {
            get
            {
                return this.TipoDeCustoFixo.Descricao;
            }
        }

        public virtual string Motorista
        {
            get
            {
                if (this.Funcionario != null)
                    return this.Funcionario.CPF_Formatado + " " + this.Funcionario.Nome;
                else
                    return string.Empty;
            }
        }
    }
}
