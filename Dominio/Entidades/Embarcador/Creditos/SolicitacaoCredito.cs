using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_CREDITO", EntityName = "SolicitacaoCredito", Name = "Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito", NameType = typeof(SolicitacaoCredito))]
    public class SolicitacaoCredito : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_SOLICITANTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_SOLICITADO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSolicitacao", Column = "SCR_DATA_SOLICITACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "SCR_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSolicitado", Column = "SCR_VALOR_SOLICITADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorSolicitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLiberado", Column = "SCR_VALOR_LIBERADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorLiberado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CREDITOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Creditor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoSolicitacaoCredito", Column = "SRC_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito SituacaoSolicitacaoCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoSolicitacao", Column = "SRC_MOTIVO_SOLICITACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string MotivoSolicitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornoSolicitacao", Column = "SRC_RETORNO_SOLICITACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string RetornoSolicitacao { get; set; }

        public virtual Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito Clonar()
        {
            return (Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito)this.MemberwiseClone();
        }
        public virtual string DescricaoSituacao
        {
            get
            {
                if (this.SituacaoSolicitacaoCredito == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Liberado)
                    return "Liberado";
                if (this.SituacaoSolicitacaoCredito == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.AgLiberacao)
                    return "Ag. Liberação";
                if (this.SituacaoSolicitacaoCredito == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Estornado)
                    return "Estornado";
                if (this.SituacaoSolicitacaoCredito == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Utilizado)
                    return "Nova";
                if (this.SituacaoSolicitacaoCredito == ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Rejeitado)
                    return "Rejeitado";
                else
                    return "";
            }
        }


        public virtual bool Equals(SolicitacaoCredito other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
