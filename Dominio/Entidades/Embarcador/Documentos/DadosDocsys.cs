using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DADOS_DOCSYS", EntityName = "DadosDocsys", Name = "Dominio.Entidades.Embarcador.Documentos.DadosDocsys", NameType = typeof(DadosDocsys))]
    public class DadosDocsys : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "DAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "vescode", Column = "DAD_VESCODE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string vescode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "vessel", Column = "DAD_VESSEL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string vessel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "voy", Column = "DAD_VOY", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string voy { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "dir", Column = "DAD_DIR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string dir { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "pol", Column = "DAD_POL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string pol { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "polname", Column = "DAD_POLNAME", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string polname { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "pod", Column = "DAD_POD", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string pod { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "podname", Column = "DAD_PODNAME", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string podname { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "blading", Column = "DAD_BLADING", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string blading { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BLVersion", Column = "DAD_BL_VERSION", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string BLVersion { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorrCore", Column = "DAD_CORR_CODE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CorrCore { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UBLI", Column = "DAD_UBLI", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UBLI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BOOKNO", Column = "DAD_BOOKNO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string BOOKNO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VoucherDate", Column = "TDO_VOUCHER_DATE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VoucherDate { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DACSTransf", Column = "TDO_DACS_TRANSF", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DACSTransf { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInclusao", Column = "TDO_DATA_INCLUSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VoucherNO", Column = "DAD_VOUCHER_NO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string VoucherNO { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Duplicado", Column = "DAD_DUPLICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Duplicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "DAD_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }
    }
}
