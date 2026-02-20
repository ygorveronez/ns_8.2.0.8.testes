using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MERCADO_LIVRE_HANDLING_UNIT", EntityName = "MercadoLivreHandlingUnit", Name = "Dominio.Entidades.Embarcardor.Integracao.MercadoLivreHandlingUnit", NameType = typeof(MercadoLivreHandlingUnit))]
    public class MercadoLivreHandlingUnit : EntidadeBase
    {
        public MercadoLivreHandlingUnit() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MHU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_ID", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rota", Column = "MHU_ROTA", TypeType = typeof(int), NotNull = false)]
        public virtual int Rota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_FACILITY", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Facility { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_DATE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Date { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_CHANNEL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Channel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_FACILITY_ID", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string FacilityID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitSituacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitSituacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_MENSAGEM", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_TIPO_INTEGRACAO_MERCADO_LIVRE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre? TipoIntegracaoMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_DATA_INCLUSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_DATA_INICIO_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_DATA_FIM_PROCESSAR_FISCAL_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimProcessarFiscalData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_DATA_FIM_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MHU_DATA_CONFIRMAR_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmarProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvancarEtapaDocumentosParaEmissaoAutomaticamente", Column = "MHU_AVANCAR_ETAPA_DOCUMENTOS_PARA_EMISSAO_AUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarEtapaDocumentosParaEmissaoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosIntegracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MERCADO_LIVRE_HANDLING_UNIT_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MLI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosIntegracao { get; set; }
    }
}
