using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_COMISSAO_MOTORISTA_MODELO_VEICULO", EntityName = "TabelaComissaoMotoristaModeloVeiculo", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo", NameType = typeof(TabelaComissaoMotoristaModeloVeiculo))]
    public class TabelaComissaoMotoristaModeloVeiculo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaModeloVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeiculo", Column = "VMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeiculo Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoMotorista", Column = "TCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoMotorista TabelaComissaoMotorista { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(TabelaComissaoMotoristaModeloVeiculo other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
