using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACEITE_DEBITO", EntityName = "AceiteDebito", Name = "Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito", NameType = typeof(AceiteDebito))]
    public class AceiteDebito : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACD_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACD_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACD_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACD_OBSERVACAO", TypeType = typeof(string), NotNull = true, Length = 500)]
        public virtual string Observacao { get; set; }

        
        public virtual string DescricaoSituacao
        {
            get
            {
                if (this.Situacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.AgAceite)
                    return "Ag. Aceite";
                if (this.Situacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.Aprovado)
                    return "Aprovado";
                if (this.Situacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.Rejeitado)
                    return "Rejeitado";
                else
                    return "";
            }
        }

        public virtual string Descricao
        {
            get
            {
                return "Aceite Ocorrência nº" + (this.Ocorrencia?.NumeroOcorrencia ?? 0).ToString();
            }
        }
    }

}
