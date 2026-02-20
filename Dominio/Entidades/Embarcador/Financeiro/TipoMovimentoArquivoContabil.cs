using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_MOVIMENTO_ARQUIVO_CONTABIL", EntityName = "TipoMovimentoArquivoContabil", Name = "Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil", NameType = typeof(TipoMovimentoArquivoContabil))]
    public class TipoMovimentoArquivoContabil : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TAC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TAC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposMovimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_MOVIMENTO_ARQUIVO_CONTABIL_TIPOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoMovimento", Column = "TIM_CODIGO")]
        public virtual ICollection<TipoMovimento> TiposMovimentos { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
