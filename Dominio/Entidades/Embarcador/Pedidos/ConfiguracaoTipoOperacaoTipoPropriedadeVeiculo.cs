using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_TIPO_PROPRIEDADE_VEICULO", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo", NameType = typeof(ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo))]
    public class ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTV_TIPO_PROPRIEDADE_VEICULO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo TipoPropriedadeVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTV_TIPO_PROPRIETARIO_VEICULO", TypeType = typeof(Dominio.Enumeradores.TipoProprietarioVeiculo), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoProprietarioVeiculo TipoProprietarioVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposTerceiros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TIPO_OPERACAO_TIPO_PROPRIEDADE_VEICULO_TIPO_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerceiro", Column = "TPT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> TiposTerceiros { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configurações de Tipo Propriedade Veículo";
            }
        }
    }
}
