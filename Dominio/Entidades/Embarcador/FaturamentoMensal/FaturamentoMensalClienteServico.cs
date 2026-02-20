using System;

namespace Dominio.Entidades.Embarcador.FaturamentoMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURAMENTO_MENSAL_CLIENTE_SERVICO", EntityName = "FaturamentoMensalClienteServico", Name = "Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico", NameType = typeof(FaturamentoMensalClienteServico))]
    public class FaturamentoMensalClienteServico : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SemDocumento", Column = "FCS_SEM_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SemDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "FCS_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFatura", Column = "FCS_VALOR_FATURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFatura", Column = "FCS_OBSERVACAO_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoMensal", Column = "FME_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoMensal FaturamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoMensalCliente", Column = "FMC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoMensalCliente FaturamentoMensalCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal.NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico NotaFiscalServico { get; set; }

        public virtual bool Equals(FaturamentoMensalClienteServico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
