using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TEMPO_AJUSTE_TABELA_FRETE", EntityName = "TempoEtapaAjusteTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete", NameType = typeof(TempoEtapaAjusteTabelaFrete))]
    public class TempoEtapaAjusteTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AjusteTabelaFrete", Column = "TFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AjusteTabelaFrete AjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TEF_ETAPA", TypeType = typeof(EtapaAjusteTabelaFrete), NotNull = true)]
        public virtual EtapaAjusteTabelaFrete Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TEF_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Entrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TEF_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Saida { get; set; }

        public virtual string DescricaoEtapa
        {
            get { return Etapa.ObterDescricao(); }
        }
    }
}
