using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CCE_INTEGRACAO", EntityName = "CartaCorrecaoIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao", NameType = typeof(CartaCorrecaoIntegracao))]
    public class CartaCorrecaoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CartaCorrecaoCCEIntegracaoArquivo>, IEquatable<CartaCorrecaoIntegracao>

    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CartaDeCorrecaoEletronica", Column = "CCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CartaDeCorrecaoEletronica  CartaCorrecao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARTA_CCE_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CartaCorrecaoCCEIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CartaCorrecaoCCEIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "CCI_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PreProtocolo", Column = "CCI_PRE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PreProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoColeta", Column = "CCI_INTEGRACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoColeta { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Protocolo;
            }
        }

        public virtual bool Equals(CartaCorrecaoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}

