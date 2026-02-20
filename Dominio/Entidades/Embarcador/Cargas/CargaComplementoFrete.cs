using System;
namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_COMPLEMENTO_FRETE", EntityName = "CargaComplementoFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete", NameType = typeof(CargaComplementoFrete))]
    public class CargaComplementoFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoCredito", Column = "SCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito SolicitacaoCredito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "CCF_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComplementoOriginal", Column = "CCF_VALOR_COMPLEMENTO_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorComplementoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComplemento", Column = "CCF_VALOR_COMPLEMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoComplementoFrete", Column = "CCF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete SituacaoComplementoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinoComplemento", Column = "CCF_DESTINO_COMPLEMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DestinoComplemento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DestinoComplemento DestinoComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "CCF_MOTIVO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAdicionalFrete", Column = "MAF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete MotivoAdicionalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComponenteFilialEmissora", Column = "CCF_COMPONENTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponenteFilialEmissora { get; set; }


        public virtual Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get
            {
                if (this.ComponenteFrete != null)
                    return ComponenteFrete?.Descricao ?? string.Empty;
                return this.Carga?.CodigoCargaEmbarcador ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                if (this.SituacaoComplementoFrete == ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgConfirmacaoUso)
                    return "Ag. Confirmação de Uso";
                if (this.SituacaoComplementoFrete == ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgAprovacao)
                    return "Ag Aprovação";
                if (this.SituacaoComplementoFrete == ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Utilizada)
                    return "Utilizado";
                if (this.SituacaoComplementoFrete == ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.AgEmissaoCTeComplementar)
                    return "Ag. Emissão CT-e Complementar";
                if (this.SituacaoComplementoFrete == ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.EmEmissaoCTeComplementar)
                    return "Em Emissão CT-e Complementar";
                if (this.SituacaoComplementoFrete == ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Rejeitada)
                    return "Rejeitada";
                if (this.SituacaoComplementoFrete == ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.Estornada)
                    return "Estornada";
                else
                    return "";
            }
        }

        public virtual bool Equals(CargaComplementoFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
