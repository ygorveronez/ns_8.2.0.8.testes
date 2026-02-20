using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_COMISSAO_MOTORISTA_ROTA_FRETE", EntityName = "TabelaComissaoMotoristaRotaFrete", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete", NameType = typeof(TabelaComissaoMotoristaRotaFrete))]

    public class TabelaComissaoMotoristaRotaFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TRF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBonificacao", Column = "TRF_VALOR_BONIFICACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_BONIFICACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoMotorista", Column = "TCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoMotorista TabelaComissaoMotorista { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(TabelaComissaoMotoristaRotaFrete other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
