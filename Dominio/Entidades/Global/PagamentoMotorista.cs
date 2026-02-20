using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA", EntityName = "PagamentoMotorista", Name = "Dominio.Entidades.PagamentoMotorista", NameType = typeof(PagamentoMotorista))]
    public class PagamentoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PMO_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "PMO_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "PMO_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PMO_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSSSENAT", Column = "PMO_VALOR_INSS_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorINSSSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSESTSENAT", Column = "PMO_VALOR_SEST_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorSESTSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostoRenda", Column = "PMO_VALOR_IMPOSTO_RENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorImpostoRenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "PMO_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutros", Column = "PMO_VALOR_OUTROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOutros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Deduzir", Column = "PMO_DEDUZIR", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = true)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Deduzir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PMO_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PMO_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "PMO_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SalarioMotorista", Column = "PMO_SALARIO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SalarioMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }


        //[NHibernate.Mapping.Attributes.Property(0, Name = "CNPJContratante", Column = "PMO_CNPJ_CONTRATANTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        //public virtual string CNPJContratante { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "CNPJContratado", Column = "PMO_CNPJ_CONTRATADO", TypeType = typeof(string), Length = 14, NotNull = false)]
        //public virtual string CNPJContratado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSEST", Column = "PMO_VALOR_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSENAT", Column = "PMO_VALOR_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSS", Column = "PMO_VALOR_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseAcumuladaIR", Column = "PMO_BASE_ACUMULADA_IR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseAcumuladaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseAcumuladaINSS", Column = "PMO_BASE_ACUMULADA_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseAcumuladaINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMO_ALIQUOTA_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMO_ALIQUOTA_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMO_ALIQUOTA_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMO_ALIQUOTA_IR", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteAcumulado", Column = "PMO_VALOR_FRETE_ACUMULADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteAcumulado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMO_ALIQUOTA_INCRA", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaINCRA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINCRA", Column = "PMO_VALOR_INCRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorINCRA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMO_ALIQUOTA_SALARIO_EDUCACAO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaSalarioEducacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSalarioEducacao", Column = "PMO_VALOR_SALARIO_EDUCACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorSalarioEducacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMO_ALIQUOTA_INSS_CONTRATANTE", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal AliquotaINSSContratante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSSContratante", Column = "PMO_VALOR_INSS_CONTRATANTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorINSSContratante { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetidoINSSAcumuladoContratado", Column = "PMO_VALOR_INSS_RETIDO_ACUMULADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetidoINSSAcumuladoContratado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDeducaoImpostoRetidoFonteIRRF", Column = "PMO_VALOR_DEDUCAO_IRRF_RETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDeducaoImpostoRetidoFonteIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetidoSESTSENATAcumuladoContratado", Column = "PMO_VALOR_RETIDO_SESTSENAT_ACUMULADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetidoSESTSENATAcumuladoContratado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDependentes", Column = "PMO_QUANTIDADE_DEPENDENTES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDependentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoIRRFDependentes", Column = "PMO_VALOR_DESCONTO_IRRF_DEPENDENTES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescontoIRRFDependentes { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ctes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_MOTORISTA_CTES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PMO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoMotoristaCtes", Column = "DCT_CODIGO")]
        public virtual IList<Dominio.Entidades.PagamentoMotoristaCtes> Ctes { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "PISProprietario", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + cd.DCL_PIS
                                                                                        FROM T_CLIENTE_DADOS cd
                                                                                         JOIN T_VEICULO ve on ve.VEI_PROPRIETARIO = cd.CLI_CGCCPF
                                                                                        WHERE ve.VEI_CODIGO = VEI_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string PISProprietario { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (Status)
                {
                    case "P":
                        return "Pendente";
                    case "A":
                        return "Pago";
                    case "C":
                        return "Cancelado";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual int NumeroCTe
        {
            get
            {
                return this.Ctes.Count > 0 ? this.Ctes.FirstOrDefault().ConhecimentoDeTransporteEletronico.Numero : 0;
            }
        }

        public virtual int SerieCTe
        {
            get
            {
                return this.Ctes.Count > 0 ? this.Ctes.FirstOrDefault().ConhecimentoDeTransporteEletronico.Serie.Numero : 0;
            }
        }

        public virtual string CPFMotorista
        {
            get
            {
                return this.Motorista.CPF_Formatado;
            }
        }

        public virtual string NomeMotorista
        {
            get
            {
                return this.Motorista.Nome;
            }
        }

        public virtual string PISMotorista
        {
            get
            {
                return this.Motorista.PIS;
            }
        }

        public virtual string Proprietario
        {
            get
            {
                return this.Veiculo != null && this.Veiculo.Proprietario != null ? this.Veiculo.Proprietario.CPF_CNPJ_Formatado + " " + this.Veiculo.Proprietario.Nome : string.Empty;
            }
        }

        //public virtual string PISProprietario
        //{
        //    get
        //    {
        //        return this.Veiculo != null && this.Veiculo.Proprietario != null ? this.Veiculo.Proprietario.PISPASEP : string.Empty;
        //    }
        //}

        public virtual string DataNacimentoProprietario
        {
            get
            {
                return this.Veiculo != null && this.Veiculo.Proprietario != null ? this.Veiculo.Proprietario.DataNascimento.HasValue ? this.Veiculo.Proprietario.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty;
            }
        }

        public virtual string DataNacimentoMotorista
        {
            get
            {
                return this.Motorista.DataNascimento.HasValue ? this.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

    }
}

