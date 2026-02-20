using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_COMISSAO_MOTORISTA_SEGMENTO", EntityName = "TabelaComissaoMotoristaSegmento", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento", NameType = typeof(TabelaComissaoMotoristaSegmento))]
    public class TabelaComissaoMotoristaSegmento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaSegmento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.SegmentoVeiculo SegmentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoMotorista", Column = "TCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoMotorista TabelaComissaoMotorista { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(TabelaComissaoMotoristaSegmento other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
