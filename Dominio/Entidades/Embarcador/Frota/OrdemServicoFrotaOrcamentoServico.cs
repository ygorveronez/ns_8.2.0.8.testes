using Dominio.Entidades.Embarcador.Anexo;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_ORCAMENTO_SERVICO", EntityName = "OrdemServicoFrotaOrcamentoServico", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico", NameType = typeof(OrdemServicoFrotaOrcamentoServico))]
    public class OrdemServicoFrotaOrcamentoServico : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OOS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OSO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaOrcamento", Column = "OSO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaOrcamento Orcamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaServicoVeiculo", Column = "OSS_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaServicoVeiculo Manutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "OOS_VALOR_PRODUTOS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMaoObra", Column = "OOS_VALOR_MAO_OBRA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorMaoObra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrcadoPor", Column = "OOS_ORCADO_POR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string OrcadoPor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "OOS_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Imagem", Column = "OOS_IMAGEM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Imagem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ORDEM_SERVICO_FROTA_ORCAMENTO_SERVICO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OOS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OrdemServicoFrotaOrcamentoServicoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<OrdemServicoFrotaOrcamentoServicoAnexo> Anexos { get; set; }

        public virtual bool Equals(ControleArquivo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
