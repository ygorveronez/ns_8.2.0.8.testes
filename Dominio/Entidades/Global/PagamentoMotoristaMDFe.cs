using System;
using System.Linq;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA_MDFE", EntityName = "PagamentoMotoristaMDFe", Name = "Dominio.Entidades.PagamentoMotoristaMDFe", NameType = typeof(PagamentoMotoristaMDFe))]
    public class PagamentoMotoristaMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "PMM_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "PMM_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PMM_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSSSENAT", Column = "PMM_VALOR_INSS_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorINSSSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSESTSENAT", Column = "PMM_VALOR_SEST_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorSESTSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "PMM_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorImpostoRenda", Column = "PMM_VALOR_IMPOSTO_RENDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorImpostoRenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Deduzir", Column = "PMM_DEDUZIR", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = true)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Deduzir { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PMM_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PMM_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "PMM_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SalarioMotorista", Column = "PMM_SALARIO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SalarioMotorista { get; set; }

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

        public virtual int NumeroMDFe
        {
            get
            {
                return this.MDFe.Numero;
            }
        }

        public virtual int SerieMDFe
        {
            get
            {
                return this.MDFe.Serie.Numero;
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

        public virtual string CPFCNPJProprietario
        {
            get
            {
                if (this.MDFe.Veiculos.FirstOrDefault() != null && !string.IsNullOrWhiteSpace(this.MDFe.Veiculos.FirstOrDefault().CPFCNPJProprietario))
                    return this.MDFe.Veiculos.FirstOrDefault().CPFCNPJProprietario;
                else
                    return this.MDFe.Empresa.CNPJ;
            }
        }

        public virtual string NomeProprietario
        {
            get
            {
                if (this.MDFe.Veiculos.FirstOrDefault() != null && !string.IsNullOrWhiteSpace(this.MDFe.Veiculos.FirstOrDefault().NomeProprietario))
                    return this.MDFe.Veiculos.FirstOrDefault().NomeProprietario;
                else
                    return this.MDFe.Empresa.RazaoSocial;
            }
        }
    }
}
