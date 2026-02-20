using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_NFS_MANUAL", EntityName = "RegrasAutorizacaoNFSManual", Name = "Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual", NameType = typeof(RegrasAutorizacaoNFSManual))]
    public class RegrasAutorizacaoNFSManual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RAN_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RAN_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RAN_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RAN_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeAprovacao", Column = "RAN_PRIORIDADE_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrioridadeAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RAN_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAN_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportadora", Column = "RAN_TRANSPORTADORA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTomador", Column = "RAN_TOMADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorPrestacaoServico", Column = "RAN_VALOR_PRESTACAO_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorValorPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoNFSManual", Column = "REQ_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual Requisito { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_NFS_MANUAL_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_NFS_MANUAL_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasFilialNFSManual", Column = "RFN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual> RegrasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTransportadora", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_NFS_MANUAL_TRANSPORTADORA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasTransportadoraNFSManual", Column = "RTN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual> RegrasTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTomador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_NFS_MANUAL_TOMADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasTomadorNFSManual", Column = "RMN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual> RegrasTomador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasValorPrestacaoServico", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_NFS_MANUAL_VALOR_PRESTACAO_SERVICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasValorPrestacaoServicoNFSManual", Column = "RMV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual> RegrasValorPrestacaoServico { get; set; }

        public virtual bool Equals(RegrasAutorizacaoNFSManual other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}