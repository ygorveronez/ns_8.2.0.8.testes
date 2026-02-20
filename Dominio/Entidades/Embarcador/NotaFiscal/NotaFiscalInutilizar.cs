using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_INUTILIZAR", EntityName = "NotaFiscalInutilizar", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar", NameType = typeof(NotaFiscalInutilizar))]
    public class NotaFiscalInutilizar : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "NFN_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "NFN_JUSTIFICATIVA", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroInicial", Column = "NFN_NUMERO_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFinal", Column = "NFN_NUMERO_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Modelo", Column = "NFN_MODELO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Modelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NFN_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusNFe Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie EmpresaSerie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NotaFiscal?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(NotaFiscalInutilizar other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
