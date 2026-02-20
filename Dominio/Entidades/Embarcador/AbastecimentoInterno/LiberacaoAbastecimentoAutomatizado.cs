using Dominio.Entidades.Embarcador.Frotas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.AbastecimentoInterno
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LIBERACAO_ABASTECIMENTO_AUTOMATIZADO", EntityName = "LiberacaoAbastecimentoAutomatizado", Name = "Dominio.Entidades.Embarcador.Frotas.LiberacaoAbastecimentoAutomatizado", NameType = typeof(LiberacaoAbastecimentoAutomatizado))]
    public class LiberacaoAbastecimentoAutomatizado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BombaAbastecimento", Column = "ABB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BombaAbastecimento BombaAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraUltimaExecucao", Column = "LAA_DATA_HORA_ULTIMA_EXECUCAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraUltimaExecucao { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLimite", Column = "LAA_VALOR_LIMITE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorLimite { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLimite", Column = "LAA_QUANTIDADE_LIMITE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeLimite { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "PosicaoAbastecimento", Column = "LAA_POSICAO_ABASTECIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int PosicaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAbastecimento", Column = "LAA_SITUACAO_ABASTECIMENTO", TypeType = typeof(SituacaoIntegracaoAbastecimento), NotNull = true)]
        public virtual SituacaoIntegracaoAbastecimento SituacaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAbastecimento", Column = "LAA_DATA_ABASTECIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemAtual", Column = "LAA_QUILOMETRAGEM_ATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int QuilometragemAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimaQuilometragem", Column = "LAA_ULTIMA_QUILOMETRAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int UltimaQuilometragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometrosRodados", Column = "LAA_QUILOMETROS_RODADOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuilometrosRodados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LIBERACAO_ABASTECIMENTO_AUTOMATIZADO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LiberacaoAbastecimentoAutomatizadoIntegracao", Column = "AAI_CODIGO")]
        public virtual IList<LiberacaoAbastecimentoAutomatizadoIntegracao> Integracoes { get; set; }

    }
}