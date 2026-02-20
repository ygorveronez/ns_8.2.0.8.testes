using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_CAMPO_OBRIGATORIO", EntityName = "PessoaCampoObrigatorio", Name = "Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampoObrigatorio", NameType = typeof(PessoaCampoObrigatorio))]
    public class PessoaCampoObrigatorio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Campos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_CAMPO_OBRIGATORIO_CAMPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaCampo", Column = "PCA_CODIGO")]
        public virtual ICollection<PessoaCampo> Campos { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString() ?? string.Empty; }
        }

        public virtual string DescricaoCliente
        {
            get { return Cliente ? "Sim" : "Não"; }
        }

        public virtual string DescricaoFornecedor
        {
            get { return Fornecedor ? "Sim" : "Não"; }
        }

        public virtual string DescricaoTerceiro
        {
            get { return Terceiro ? "Sim" : "Não"; }
        }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
