using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_GHOST", EntityName = "IntegracaoGhost", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost", NameType = typeof(IntegracaoGhost))]
    public class IntegracaoGhost : Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>,  IEquatable<IntegracaoGhost>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ITG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDestino", Column = "ITG_TIPO_DESTINO", TypeType = typeof(TipoDestinoGhost), NotNull = true)]
        public virtual TipoDestinoGhost TipoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Guid", Column = "ITG_GUID", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Guid { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveRequisicao", Column = "ITG_CHAVE_REQUISICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChaveRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_GHOST_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ITG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(IntegracaoGhost other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string Descricao => $"Protocolo: {Codigo}";
    }
}
