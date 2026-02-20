using System;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_MOTIVO_CHAMADO_TIPO_INTEGRACAO", EntityName = "GrupoMotivoChamadoTipoIntegracao", Name = "Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao", NameType = typeof(GrupoMotivoChamadoTipoIntegracao))]
    public class GrupoMotivoChamadoTipoIntegracao : EntidadeBase, IEquatable<GrupoMotivoChamadoTipoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GMT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoMotivoChamado", Column = "GMC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoMotivoChamado GrupoMotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracao", Column = "GMT_TIPO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegracao { get; set; }


        public virtual bool Equals(GrupoMotivoChamadoTipoIntegracao other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
