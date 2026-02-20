using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_PATIO_CONFIGURACAO_ETAPA", EntityName = "GestaoPatioConfiguracaoEtapa", Name = "Dominio.Entidades.Embarcador.Filiais.GestaoPatioConfiguracaoEtapa", NameType = typeof(GestaoPatioConfiguracaoEtapa))]
    public class GestaoPatioConfiguracaoEtapa: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GCE_ETAPA", TypeType = typeof(EtapaFluxoGestaoPatio), NotNull = true)]
        public virtual EtapaFluxoGestaoPatio Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GCE_SITUACAO_CONFIRMACAO", TypeType = typeof(SituacaoConfirmacaoEtapaFluxoGestaoPatio), NotNull = true)]
        public virtual SituacaoConfirmacaoEtapaFluxoGestaoPatio SituacaoConfirmacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GCE_TIPO", TypeType = typeof(TipoFluxoGestaoPatio), NotNull = false)]
        public virtual TipoFluxoGestaoPatio? Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiFiliaisExclusivas", Column = "GCE_POSSUI_FILIAIS_EXCLUSIVAS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiFiliaisExclusivas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEdicaoDadosTransporteJanelaTransportador", Column = "GCE_BLOQUEAR_EDICAO_DADOS_TRANSPORTE_JANELA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEdicaoDadosTransporteJanelaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEdicaoVeiculosCarga", Column = "GCE_BLOQUEAR_EDICAO_VEICULOS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEdicaoVeiculosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FiliaisExclusivas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_PATIO_CONFIGURACAO_ETAPA_FILIAL_EXCLUSIVA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GCE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filial> FiliaisExclusivas { get; set; }
    }
}
