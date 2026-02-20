using System;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTIFICACAO", EntityName = "Notificacao", Name = "Dominio.Entidades.Embarcador.Notificacoes.Notificacao", NameType = typeof(Notificacao))]
    public class Notificacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Notificacoes.Notificacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nota", Column = "NOT_NOTA", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string Nota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_GEROU_NOTIFICACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioGerouNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataNotificacao", Column = "NOT_DATA_NOTIFICACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoNotificacao", Column = "NOT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao SituacaoNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNotificacao", Column = "NOT_TIPO_NOTIFICACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao TipoNotificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPagina", Column = "NOT_URL_PAGINA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string URLPagina { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Icone", Column = "NOT_ICONE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao Icone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IconeCorFundo", Column = "NOT_ICONE_COR_FUNDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor IconeCorFundo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoObjetoNotificacao", Column = "NOT_CODIGO_OBJETO_NOTIFICACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoObjetoNotificacao { get; set; }

        public virtual bool Equals(Notificacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                if (SituacaoNotificacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas)
                    return "Todas";

                if (SituacaoNotificacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Nova)
                    return "Não lida";

                if (SituacaoNotificacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Lida)
                    return "Lida";

                return "";
            }
        }

    }
}
