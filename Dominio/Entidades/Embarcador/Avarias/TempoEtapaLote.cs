using System;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TEMPO_ETAPA_LOTE", EntityName = "TempoEtapaLote", Name = "Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote", NameType = typeof(TempoEtapaLote))]
    public class TempoEtapaLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TEL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Lote", Column = "LAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.Lote Lote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAV_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaLote), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaLote Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAV_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Entrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAV_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Saida { get; set; }

        public virtual string DescricaoEtapa
        {
            get
            {
                switch (Etapa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.AutorizacaoLote:
                        return "Autorização Lote";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote:
                        return "Criação Lote";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.IntegracaoLote:
                        return "Integração Lote";
                    default:
                        return "";
                }
            }
        }
        public virtual string Descricao
        {
            get
            {
                return "Tempo na etapa do lote " + (this.Lote?.Descricao ?? "");
            }
        }
    }
}
