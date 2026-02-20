using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTAINER", EntityName = "Container", Name = "Dominio.Entidades.Embarcador.Pedidos.Container", NameType = typeof(Container))]
    public class Container : EntidadeBase, IEquatable<Container>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CTR_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CTR_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CTR_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CTR_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "CTR_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tara", Column = "CTR_TARA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Tara { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CTR_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropriedade", Column = "CTR_PROPRIEDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.InformacaoManuseio), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer TipoPropriedade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarregamentoNavio", Column = "CTR_TIPO_CARREGAMENTO_NAVIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoNavio), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoNavio TipoCarregamentoNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo ContainerTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_METROS_CUBICOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetrosCubicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTR_DATA_ULTIMA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaAtualizacao { get; set; } = null;

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "CTR_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_ARMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteArmador { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(Container other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
