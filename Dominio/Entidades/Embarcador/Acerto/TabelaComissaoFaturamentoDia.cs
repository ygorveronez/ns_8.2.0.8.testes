using System;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_COMISSAO_MOTORISTA_FATURAMENTO_DIA", EntityName = "TabelaComissaoFaturamentoDia", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia", NameType = typeof(TabelaComissaoFaturamentoDia))]
    public class TabelaComissaoFaturamentoDia : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturamentoInicial", Column = "TFD_FATURAMENTO_INICIAL", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal FaturamentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturamentoFinal", Column = "TFD_FATURAMENTO_FINAL", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal FaturamentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAcrescimoComissao", Column = "TMM_PERCENTUAL_ACRESCIMO_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAcrescimoComissao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaComissaoMotorista", Column = "TCM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaComissaoMotorista TabelaComissaoMotorista { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual bool Equals(TabelaComissaoFaturamentoDia other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
