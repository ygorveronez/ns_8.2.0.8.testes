using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHECK_LIST_CARGA", EntityName = "CheckListCarga", Name = "Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga", NameType = typeof(CheckListCarga))]
    public class CheckListCarga : EntidadeCargaBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_DATA_ABERTURA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_DATA_LIBERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaCheckListLiberado", Column = "CLC_ETAPA_CHECK_LIST_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EtapaCheckListLiberado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LicencaInvalida", Column = "CLC_LICENCA_INVALIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LicencaInvalida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_REAVALIADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Reavaliada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_SITUACAO", TypeType = typeof(SituacaoCheckList), NotNull = false)]
        public virtual SituacaoCheckList Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_APLICACAO", TypeType = typeof(AplicacaoOpcaoCheckList), NotNull = false)]
        public virtual AplicacaoOpcaoCheckList Aplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Aprovador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Perguntas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHECK_LIST_CARGA_PERGUNTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CheckListCargaPergunta", Column = "CLP_CODIGO")]
        public virtual ICollection<CheckListCargaPergunta> Perguntas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_ETAPA_CHECKLIST", TypeType = typeof(EtapaCheckList), NotNull = false)]
        public virtual EtapaCheckList EtapaCheckList { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListCargaVigencia", Column = "CCV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListCargaVigencia CheckListCargaVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EditadoRetroativo", Column = "CLC_EDITADO_RETROATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EditadoRetroativo { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Checklist da carga {Carga.CodigoCargaEmbarcador}" : $"Checklist da pré carga {PreCarga.NumeroPreCarga}"; }
        }

        public virtual string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoAplicacao
        {
            get { return this.Aplicacao.ObterDescricao(); }
        }

        public virtual EtapaFluxoGestaoPatio EtapaFluxoGestaoPatio
        {
            get
            {
                if (EtapaCheckList == EtapaCheckList.AvaliacaoDescarga)
                    return EtapaFluxoGestaoPatio.AvaliacaoDescarga;

                return EtapaFluxoGestaoPatio.CheckList;
            }
        }
    }
}