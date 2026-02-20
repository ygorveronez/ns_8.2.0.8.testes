using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA", EntityName = "RegrasMultaAtrasoRetirada", Name = "Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada", NameType = typeof(RegrasMultaAtrasoRetirada))]
    public class RegrasMultaAtrasoRetirada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RMA_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualInclusao", Column = "RMA_PERCENTUAL_INCLUSAO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RMA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Estados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_ESTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> Estados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Cidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_CIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Cidades { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoOperacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Pedidos.TipoOperacao> TipoOperacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CEPs", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_CEP")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasMultaAtrasoRetiradaCEP")]
        public virtual ICollection<RegrasMultaAtrasoRetiradaCEP> CEPs { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_MULTA_ATRASO_RETIRADA_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> Clientes { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}