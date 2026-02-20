using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_LAUDO", EntityName = "GestaoDevolucaoLaudo", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo", NameType = typeof(GestaoDevolucaoLaudo))]

    public class GestaoDevolucaoLaudo : EntidadeBase
    {
        public GestaoDevolucaoLaudo()
        {
            DataCriacao = DateTime.Now;
        }

        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GDL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "GDL_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAnalise", Column = "GDL_DATA_ANALISE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAnalise { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "GDL_MOTIVO", TypeType = typeof(string), NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "GDL_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoLaudo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoLaudo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Produtos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DEVOLUCAO_LAUDO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GDL_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "GestaoDevolucaoLaudoProduto")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto> Produtos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCompensacao", Column = "GDL_NUMERO_COMPENSACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string NumeroCompensacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompensacao", Column = "GDL_DATA_COMPENSACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompensacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "GDL_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "GDL_VALOR_LAUDO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GDL_CAMINHO_ARQUIVO_LAUDO", Type = "StringClob", NotNull = false, Lazy = true)]
        public virtual string CaminhoArquivoLaudo { get; set; }

        #endregion

        #region Atributos Virtuais
        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
        #endregion
    }
}
