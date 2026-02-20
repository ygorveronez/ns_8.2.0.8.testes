using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRE_AGRUPAMENTO_ROTEIRIZACAO", EntityName = "PreAgrupamentoCargaRoteirizacao", Name = "Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao", NameType = typeof(PreAgrupamentoCargaRoteirizacao))]
    public class PreAgrupamentoCargaRoteirizacao : EntidadeBase, IEquatable<PreAgrupamentoCargaRoteirizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreAgrupamentoCargaAgrupador", Column = "PAA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreAgrupamentoCargaAgrupador Agrupador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdEntrega", Column = "PAR_ID_ENTREGA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IdEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJDestinatario", Column = "PAR_CNPJ_DESTINATARIO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoChegada", Column = "PAC_DATA_PREVISAO_CHEGADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioDescarregamento", Column = "PAC_DATA_INICIO_DESCARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "PAC_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarregamentoMinutos", Column = "PAC_TEMPO_DESCARREGAMENTO_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDescarregamentoMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioJanelaDescarga", Column = "PAC_DATA_INICIO_JANELA_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? InicioJanelaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FimJanelaDescarga", Column = "PAC_DATA_FIM_JANELA_DESCARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? FimJanelaDescarga { get; set; }

        public virtual bool Equals(PreAgrupamentoCargaRoteirizacao other)
        {
            return (this.Codigo == other.Codigo);
        }

    }
}
