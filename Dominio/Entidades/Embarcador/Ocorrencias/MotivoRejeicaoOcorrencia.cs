using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_REJEICAO_OCORRENCIA", EntityName = "MotivoRejeicaoOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia", NameType = typeof(MotivoRejeicaoOcorrencia))]
    public class MotivoRejeicaoOcorrencia : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRO_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MRO_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.AprovacaoRejeicao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.AprovacaoRejeicao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRO_NAO_PERMITIR_ABRIR_OCORRENCIA_DUPLICADA_REJEICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAbrirOcorrenciaDuplicadaRejeicao { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(MotivoRejeicaoOcorrencia other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
