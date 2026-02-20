using Dominio.Interfaces.Embarcador.Entidade;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_PRESTACAO_SERVICO", EntityName = "ContratoPrestacaoServico", Name = "Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico", NameType = typeof(ContratoPrestacaoServico))]
    public class ContratoPrestacaoServico : EntidadeBase, IEntidade, IEquatable<ContratoPrestacaoServico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CPS_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "CPS_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "CPS_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CPS_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPS_VALOR_TETO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTeto { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_PRESTACAO_SERVICO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_PRESTACAO_SERVICO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filiais.Filial> Filiais { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(ContratoPrestacaoServico other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
