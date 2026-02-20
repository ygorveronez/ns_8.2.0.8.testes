using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_VEICULO_HISTORICO", EntityName = "FilaCarregamentoVeiculoHistorico", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico", NameType = typeof(FilaCarregamentoVeiculoHistorico))]
    public class FilaCarregamentoVeiculoHistorico: EntidadeBase, IEquatable<FilaCarregamentoVeiculoHistorico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FVH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "FVH_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FVH_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "FVH_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FVH_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculoHistorico), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculoHistorico Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FVH_ORIGEM_ALTERACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemAlteracaoFilaCarregamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemAlteracaoFilaCarregamento OrigemAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoVeiculo", Column = "FLV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoVeiculo FilaCarregamentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAlteracaoPosicaoFilaCarregamento", Column = "FMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoAlteracaoPosicaoFilaCarregamento MotivoAlteracaoPosicaoFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRetiradaFilaCarregamento", Column = "FMR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRetiradaFilaCarregamento MotivoRetiradaFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoSelecaoMotoristaForaOrdem", Column = "FMS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoSelecaoMotoristaForaOrdem MotivoSelecaoMotoristaForaOrdem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FVH_OBSERVACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual bool Equals(FilaCarregamentoVeiculoHistorico other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual string ObterPosicao()
        {
            return Posicao > 0 ? Posicao.ToString() : "";
        }
    }
}
