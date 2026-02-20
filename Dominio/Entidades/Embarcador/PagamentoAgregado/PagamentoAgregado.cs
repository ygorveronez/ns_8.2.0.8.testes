using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_AGREGADO", EntityName = "PagamentoAgregado", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado", NameType = typeof(PagamentoAgregado))]
    public class PagamentoAgregado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PAA_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DATA_INICIAL_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DATA_FINAL_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_VALOR", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado StatusPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_APROVADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioAprovador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DATA_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PagamentoAgregadoAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_PAGAMENTO_AGREGADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaPagamentoAgregado", Column = "AAP_CODIGO")]
        public virtual ICollection<AprovacaoAlcadaPagamentoAgregado> PagamentoAgregadoAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_AGREGADO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoAgregadoAnexo", Column = "PAX_CODIGO")]
        public virtual IList<PagamentoAgregadoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR_FATURA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente TomadorFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_NUMERO_FATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_COMPETENCIA_MES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.Mes), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.Mes CompetenciaMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_COMPETENCIA_QUINZENA", TypeType = typeof(Quinzena), NotNull = false)]
        public virtual Quinzena CompetenciaQuinzena { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAA_DESCRICAO_COMPETENCIA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoCompetencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosContratos", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CC.CFT_NUMERO_CONTRATO AS NVARCHAR(20))
                                                                                            FROM T_CONTRATO_FRETE_TERCEIRO CC
                                                                                            WHERE CC.PAA_CODIGO = PAA_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosContratos { get; set; }

        public virtual string Descricao
        {
            get { return this.Cliente?.Nome ?? string.Empty; }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        #endregion
    }
}
