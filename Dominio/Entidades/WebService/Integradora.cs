using System;
using System.Collections.Generic;

namespace Dominio.Entidades.WebService
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRADORA", EntityName = "Integradora", Name = "Dominio.Entidades.WebService.Integradora", NameType = typeof(Integradora))]
    public class Integradora : EntidadeBase, IEquatable<Integradora>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "INT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "INT_TOKEN", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIndicadorIntegracao", Column = "INT_TIPO_INDICADOR_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIndicadorIntegracao TipoIndicadorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRADORA_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PermissoesWebservice", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PERMISSAO_WEBSERVICE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "PermissaoWebservice")]
        public virtual ICollection<PermissaoWebservice> PermissoesWebservice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TodosWebServicesLiberados", Column = "INT_TODOS_WEB_SERVICES_LIBERADOS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool TodosWebServicesLiberados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "EQP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAutenticacao", Column = "INT_TIPO_AUTENTICACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAutenticacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAutenticacao TipoAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "INT_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "INT_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoExpiracao", Column = "INT_TEMPO_EXPIRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoExpiracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenTemporario", Column = "INT_TOKEN_TEMPORARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TokenTemporario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataExpiracao", Column = "INT_DATA_EXPIRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataExpiracao { get; set; }

        #endregion Propriedades

        #region Métodos com Regras

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(Integradora other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion Métodos com Regras
    }
}
