using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Roteirizador
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTEIRIZADOR_INTEGRACAO", EntityName = "RoteirizadorIntegracao", Name = "Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao", NameType = typeof(RoteirizadorIntegracao))]
    public class RoteirizadorIntegracao : EntidadeBase, IIntegracaoComArquivo<RoteirizadorIntegracaoArquivo>, IEquatable<RoteirizadorIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "RIN_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "RIN_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "RIN_TIPO", TypeType = typeof(TipoRoteirizadorIntegracao), NotNull = true)]
        public virtual TipoRoteirizadorIntegracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "RIN_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoIntegracao), NotNull = true)]
        public virtual SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "RIN_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "RIN_PROBLEMA_INTEGRACAO", Type = "StringClob", NotNull = true)]
        public virtual string ProblemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTEIRIZADOR_INTEGRACAO_ARQUIVOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RIN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RoteirizadorIntegracaoArquivo", Column = "RIA_CODIGO")]
        public virtual ICollection<RoteirizadorIntegracaoArquivo> ArquivosTransacao { get; set; }

        /// <summary>
        /// Usuário responsável pela integração da roteirização.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        public RoteirizadorIntegracao()
        {
            DataCriacao = DateTime.Now;
            DataIntegracao = DateTime.Now;
        }

        public virtual bool Equals(RoteirizadorIntegracao other)
        {
            return Codigo == other.Codigo;
        }
    }
}
