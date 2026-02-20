using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_ALERTA", EntityName = "ConfiguracaoAlerta", Name = "Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta", NameType = typeof(ConfiguracaoAlerta))]
    public class ConfiguracaoAlerta : EntidadeBase, IEquatable<ConfiguracaoAlerta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertarAposVencimento", Column = "CFA_ALERTAR_APOS_VENCIMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AlertarAposVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertarTransportador", Column = "CFA_ALERTAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertarTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CFA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAlertarAntesVencimento", Column = "CFA_DIAS_ALERTAR_ANTES_VENCIMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasAlertarAntesVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasRepetirAlerta", Column = "CFA_DIAS_REPETIR_ALERTA", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasRepetirAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CFA_TIPO", TypeType = typeof(TipoConfiguracaoAlerta), NotNull = true)]
        public virtual TipoConfiguracaoAlerta Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Usuarios", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_ALERTA_USUARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Usuarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigosRejeicoes", Column = "CFA_CODIGOS_REJEICOES", TypeType = typeof(string), Length = 1600, NotNull = false)]
        public virtual string CodigosRejeicoes { get; set; }

        public virtual string Descricao
        {
            get { return $"Configuração de Alerta de {Tipo.ObterDescricao()}"; }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(ConfiguracaoAlerta other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
