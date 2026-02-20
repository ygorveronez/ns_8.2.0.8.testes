using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPOS_MOTIVO_ATENDIMENTOS", EntityName = "TiposMotivoAtendimento", Name = "Dominio.Entidades.Embarcador.Chamados.TiposMotivoAtendimento", NameType = typeof(TiposMotivoAtendimento))]
    public class TiposMotivoAtendimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TMA_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento TipoMotivoAtendimento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.TipoMotivoAtendimento.ObterDescricao();
            }
        }
    }
}