using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_MOTIVO_ALTERACAO_POSICAO", EntityName = "MotivoAlteracaoPosicaoFilaCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento", NameType = typeof(MotivoAlteracaoPosicaoFilaCarregamento))]
    public class MotivoAlteracaoPosicaoFilaCarregamento : EntidadeBase, IEquatable<MotivoAlteracaoPosicaoFilaCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FMP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FMP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FMP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(MotivoAlteracaoPosicaoFilaCarregamento other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
