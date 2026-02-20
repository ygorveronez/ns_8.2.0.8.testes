using System;

namespace Dominio.Entidades.Embarcador.Cargas.ColetaEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_ENTREGA_CHEGADA_FORNECEDOR", EntityName = "EtapaChegadaFornecedor", Name = "Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor", NameType = typeof(EtapaChegadaFornecedor))]
    public class EtapaChegadaFornecedor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoColetaEntrega", Column = "FCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega FluxoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInformada", Column = "ECF_DATA_INFORMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInformada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaLiberada", Column = "ECF_ETAPA_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ECF_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return FluxoColetaEntrega.Carga.CodigoCargaEmbarcador;
            }
        }

    }
}
