using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRAZO_SITUACAO", EntityName = "PrazoSituacaoCarga", Name = "Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga", NameType = typeof(PrazoSituacaoCarga))]
    public class PrazoSituacaoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCarga", Column = "CPS_SITUACAO_CARGA", TypeType = typeof(SituacaoCargaJanelaCarregamento), NotNull = true)]
        public virtual SituacaoCargaJanelaCarregamento SituacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "CPS_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarTransportadorPorEmailAoEsgotarPrazo", Column = "CPS_NOTIFICAR_TRANSPORTADOR_POR_EMAIL_AO_ESGOTAR_PRAZO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorPorEmailAoEsgotarPrazo { get; set; }

        public virtual string Descricao
        {
            get { return this.DescricaoSituacaoCarga; }
        }

        public virtual string DescricaoSituacaoCarga
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        public virtual string DescricaoTempo
        {
            get
            {
                TimeSpan horas = TimeSpan.FromMinutes(Tempo);

                return string.Format("{0:00}:{1:00}", Math.Floor(horas.TotalHours), horas.Minutes);
            }
        }

    }
}
