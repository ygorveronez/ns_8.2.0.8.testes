using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Entidades.Embarcador.Pessoas;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_CARREGAMENTO_PRODUTIVIDADE", EntityName = "CentroCarregamentoProdutividade", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoProdutividade", NameType = typeof(CentroCarregamentoProdutividade))]
    public class CentroCarregamentoProdutividade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Picking", Column = "CCP_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Picking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Separacao", Column = "CCP_Separacao", TypeType = typeof(int), NotNull = true)]
        public virtual int Separacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Carregamento", Column = "CCP_Carregamento", TypeType = typeof(int), NotNull = true)]
        public virtual int Carregamento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasTrabalho", Column = "CCP_HORAS_TRABALHO", TypeType = typeof(int))]
        public virtual int HorasTrabalho { get; set; }

        public virtual string Descricao
        {
            get { return CentroCarregamento.Descricao + (GrupoPessoas != null ? " - " + GrupoPessoas.Descricao : string.Empty); }
        }
    }
}