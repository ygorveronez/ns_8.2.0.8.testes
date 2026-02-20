using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MANOBRA_ACAO", EntityName = "ManobraAcao", Name = "Dominio.Entidades.Embarcador.Logistica.ManobraAcao", NameType = typeof(ManobraAcao))]
    public class ManobraAcao : EntidadeBase, IEquatable<ManobraAcao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MAC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalDestinoObrigatorio", Column = "MAC_LOCAL_DESTINO_OBRIGATORIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LocalDestinoObrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MAC_TIPO", TypeType = typeof(TipoManobraAcao), NotNull = false)]
        public virtual TipoManobraAcao Tipo { get; set; }

        public virtual string DescricaoLocalDestinoObrigatorio
        {
            get { return LocalDestinoObrigatorio ? "Sim" : "Não"; }
        }

        public virtual bool Equals(ManobraAcao other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
