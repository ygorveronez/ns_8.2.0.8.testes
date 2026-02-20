using System;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS_ANALISE", EntityName = "ChamadoTMSAnalise", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise", NameType = typeof(ChamadoTMSAnalise))]
    public class ChamadoTMSAnalise : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoTMS", Column = "CHT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChamadoTMS Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Autor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_DATA_ANALISE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_OBSERVACAO", Type = "StringClob", NotNull = true)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Autor?.Descricao ?? string.Empty) + " - " + (this.Chamado?.Descricao ?? string.Empty);
            }
        }
    }
}
