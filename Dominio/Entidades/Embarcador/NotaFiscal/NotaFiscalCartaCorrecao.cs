using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_CARTA_CORRECAO", EntityName = "NotaFiscalCartaCorrecao", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao", NameType = typeof(NotaFiscalCartaCorrecao))]
    public class NotaFiscalCartaCorrecao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalCartaCorrecao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "NCC_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NCC_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NCC_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.StatusNFe Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscalObservacaoCartaCorrecao", Column = "NOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscalObservacaoCartaCorrecao NotaFiscalObservacaoCartaCorrecao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "NCC_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLote", Column = "NCC_NUMERO_LOTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroLote { get; set; }
        public virtual bool Equals(NotaFiscalCartaCorrecao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
