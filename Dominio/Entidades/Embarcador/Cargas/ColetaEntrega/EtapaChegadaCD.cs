using System;

namespace Dominio.Entidades.Embarcador.Cargas.ColetaEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_ENTREGA_CHEGADA_CD", EntityName = "EtapaChegadaCD", Name = "Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD", NameType = typeof(EtapaChegadaCD))]
    public class EtapaChegadaCD : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoColetaEntrega", Column = "FCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega FluxoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInformada", Column = "ECC_DATA_INFORMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInformada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaLiberada", Column = "ECC_ETAPA_LIBERADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ECC_OBSERVACAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECC_LOCAL_VEICULO_FLUXO_COLETA_ENTREGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalVeiculoFluxoColetaEntrega), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalVeiculoFluxoColetaEntrega LocalVeiculoFluxoColetaEntrega { get; set; }

        public virtual string Descricao
        {
            get
            {
                return FluxoColetaEntrega.Carga.CodigoCargaEmbarcador;
            }
        }

    }
}
