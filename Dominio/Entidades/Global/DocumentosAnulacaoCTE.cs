using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_DOCS_ANULACAO", EntityName = "DocumentosAnulacaoCTE", Name = "Dominio.Entidades.DocumentosAnulacaoCTE", NameType = typeof(DocumentosAnulacaoCTE))]
    public class DocumentosAnulacaoCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContribuinteICMS", Column = "NFA_CONTRIBUINTE", TypeType = typeof(Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Enumeradores.OpcaoSimNao ContribuinteICMS { set; get; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "NFA_CHAVE", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "NFA_CNPJ", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NFA_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "NFA_SERIE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Subserie", Column = "NFA_SUB_SERIE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Subserie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "NFA_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NFA_DATAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "NFA_TIPO", TypeType = typeof(Enumeradores.TipoDocumentoAnulacao), NotNull = false)]
        public virtual Enumeradores.TipoDocumentoAnulacao Tipo { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
