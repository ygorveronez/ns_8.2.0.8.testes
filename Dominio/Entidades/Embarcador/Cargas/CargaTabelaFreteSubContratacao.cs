using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TABELA_FRETE_SUBCONTRATACAO", EntityName = "CargaTabelaFreteSubContratacao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao", NameType = typeof(CargaTabelaFreteSubContratacao))]
    public class CargaTabelaFreteSubContratacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TRANSPORTADOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente TransportadorTerceiro { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalRecebeCTeParaSubContratacao", Column = "CTS_VALOR_TOTAL_RECEBER_CTE_PARA_SUBCONTRATACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalRecebeCTeParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalICMSCTeParaSubContratacao", Column = "CTS_VALOR_TOTAL_ICMS_CTE_PARA_SUBCONTRATACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalICMSCTeParaSubContratacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobrado", Column = "CTS_PERCENTUAL_COBRADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualCobrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CTS_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao)this.MemberwiseClone();
        }

        public virtual bool Equals(CargaTabelaFreteSubContratacao other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
