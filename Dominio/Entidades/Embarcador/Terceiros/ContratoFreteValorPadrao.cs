using System;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TERCEIRO_VALOR_PADRAO", EntityName = "ContratoFreteValorPadrao", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao", NameType = typeof(ContratoFreteValorPadrao))]
    public class ContratoFreteValorPadrao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TRANSPORTADOR_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente TransportadorTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_APENAS_QUANDO_EMITIR_CIOT", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasQuandoEmitirCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual bool Equals(ContratoFreteValorPadrao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
