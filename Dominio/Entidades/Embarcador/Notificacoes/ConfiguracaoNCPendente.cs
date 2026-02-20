using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_NC_PENDENTE", EntityName = "ConfiguracaoNCPendente", Name = "Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente", NameType = typeof(ConfiguracaoNCPendente))]
    public class ConfiguracaoNCPendente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CNP_NOME", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CNP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarTransportador", Column = "CNP_NOTIFICAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CFA_TIPO", TypeType = typeof(TipoConfiguracaoNCPendente), NotNull = true)]
        public virtual TipoConfiguracaoNCPendente Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Setor", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_NC_PENDENTE_SETOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Setor", Column = "SET_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Setor> Setor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filial", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_NC_PENDENTE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Filiais.Filial> Filial { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ItemNaoConformidade", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_NC_PENDENTE_ITEM_NC")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemNaoConformidade", Column = "INC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> ItemNaoConformidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoOperacao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_NC_PENDENTE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Usuarios", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_NC_PENDENTE_USUARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Usuarios { get; set; }

        public virtual string Descricao
        {
            get { return $"Configuração de envio de e-mail de não conformidade pendente {this.Nome}"; }
        }
    }    
}
