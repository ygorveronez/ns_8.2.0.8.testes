using System;

namespace Dominio.Entidades.Embarcador.FaturamentoMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_EMISSAO_NFE_VALOR", EntityName = "PlanoEmissaoNFeValor", Name = "Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor", NameType = typeof(PlanoEmissaoNFeValor))]
    public class PlanoEmissaoNFeValor : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PNV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PNV_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObservacaoFaturamentoMensal", Column = "PNV_TIPO_OBSERVACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal TipoObservacaoFaturamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PNV_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeInicial", Column = "PNV_QTD_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFinal", Column = "PNV_QTD_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoEmissaoNFe", Column = "PEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoEmissaoNFe PlanoEmissaoNFe { get; set; }

        public virtual bool Equals(PlanoEmissaoNFeValor other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
