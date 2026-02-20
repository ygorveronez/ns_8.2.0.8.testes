using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_CLASSIFICACAO", EntityName = "RotaFreteClassificacao", Name = "Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao", NameType = typeof(RotaFreteClassificacao))]
    public class RotaFreteClassificacao : EntidadeBase, IEquatable<RotaFreteClassificacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RFC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFC_CLASSE", TypeType = typeof(RotaFreteClasse), NotNull = true)]
        public virtual RotaFreteClasse Classe { get; set; }

        public virtual bool Equals(RotaFreteClassificacao other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
