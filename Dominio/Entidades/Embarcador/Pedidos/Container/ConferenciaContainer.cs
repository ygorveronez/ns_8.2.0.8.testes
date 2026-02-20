using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFERENCIA_CONTAINER", EntityName = "ConferenciaContainer", Name = "Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer", NameType = typeof(ConferenciaContainer))]
    public class ConferenciaContainer : EntidadeBase
    {
        public ConferenciaContainer()
        {
            DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Container Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "CCT_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConferencia", Column = "CCT_DATA_CONFERENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConferencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioConferiu { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CCT_SITUACAO", TypeType = typeof(SituacaoConferenciaContainer), NotNull = true)]
        public virtual SituacaoConferenciaContainer Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CCT_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Conferência do container da carga {Carga.CodigoCargaEmbarcador}";
            }
        }
    }
}
