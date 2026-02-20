using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_ELECTROLUX_DOCUMENTO_TRANSPORTE", EntityName = "IntegracaoElectroluxDocumentoTransporte", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte", NameType = typeof(IntegracaoElectroluxDocumentoTransporte))]
    public class IntegracaoElectroluxDocumentoTransporte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "IDT_NUMERO", TypeType = typeof(long), NotNull = true)]
        public virtual long Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotfis", Column = "IDT_NUMERO_NOTFIS", TypeType = typeof(string), NotNull = false)]
        public virtual string NumeroNotfis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "IDT_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "IDT_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "IDT_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IDT_GERADO_POR_NOTFIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoPorNOTFIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "IDT_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_ELECTROLUX_DOCUMENTO_TRANSPORTE_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IDT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "IntegracaoElectroluxConsultaLog", Column = "INE_CODIGO")]
        public virtual ICollection<IntegracaoElectroluxConsultaLog> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_INTEGRACAO_ELECTROLUX")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IDT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaIntegracaoElectrolux", Column = "CIE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoElectrolux> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_ELECTROLUX_DOCUMENTO_TRANSPORTE_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IDT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "IntegracaoElectroluxDocumentoTransporteNotaFiscal", Column = "INF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal> NotasFiscais { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }
    }
}
