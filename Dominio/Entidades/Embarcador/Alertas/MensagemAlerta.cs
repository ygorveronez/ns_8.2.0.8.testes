using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Alertas
{
    public abstract class MensagemAlerta<TEntidadeMensagemAlerta> : EntidadeBase
        where TEntidadeMensagemAlerta : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Construtores

        public MensagemAlerta()
        {
            DataCriacao = DateTime.Now;
            Mensagens = new List<string>();
        }

        #endregion Construtores

        #region Propriedades 

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloquear", Column = "MAL_BLOQUEAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloquear { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Confirmada", Column = "MAL_CONFIRMADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Confirmada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "MAL_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConfirmacao", Column = "MAL_DATA_CONFIRMACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MAL_TIPO", TypeType = typeof(TipoMensagemAlerta), NotNull = true)]
        public virtual TipoMensagemAlerta Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioConfirmacao { get; set; }

        public virtual string Titulo
        {
            get
            {
                return Tipo.ObterTituloMensagem();
            }
        }

        public virtual bool UtilizarConfirmacao
        {
            get
            {
                return Tipo.UtilizarConfirmacaoMensagem();
            }
        }

        #endregion Propriedades

        #region Propriedades Abstratas

        public abstract TEntidadeMensagemAlerta Entidade { get; set; }

        public abstract ICollection<string> Mensagens { get; set; }

        #endregion Propriedades Abstratas
    }
}
