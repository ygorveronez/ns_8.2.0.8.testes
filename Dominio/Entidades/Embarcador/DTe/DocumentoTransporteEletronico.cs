using System;

namespace Dominio.Entidades.Embarcador.DTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DTE", EntityName = "DocumentoTransporteEletronico", Name = "Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico", NameType = typeof(DocumentoTransporteEletronico))]
    public class DocumentoTransporteEletronico : EntidadeBase, IEquatable<DocumentoTransporteEletronico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "DTE_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "DTE_SERIE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "DTE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "DTE_TIPO_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoAmbiente), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "DTE_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "DTE_STATUS", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "DTE_MOTIVO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "DTE_PROTOCOLO", TypeType = typeof(int), NotNull = false)]
        public virtual int Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRecibo", Column = "DTE_NUMERO_RECIBO", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string NumeroRecibo { get; set; }


        public virtual bool Equals(DocumentoTransporteEletronico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
