using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.CTeAgrupado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_AGRUPADO", EntityName = "CargaCTeAgrupado", Name = "Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado", NameType = typeof(CargaCTeAgrupado))]
    public class CargaCTeAgrupado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_NUMERO", TypeType = typeof(int), NotNull = true, Unique = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_OBSERVACAO_CTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_GERAR_CTE_POR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCTePorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_NUMERO_CARGAS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string NumeroCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_MENSAGEM", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCA_ENVIOU_DOCUMENTOS_PARA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviouDocumentosParaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0,  Column = "CCA_GERANDO_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_AGRUPADO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeAgrupadoCarga", Column = "CAC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_AGRUPADO_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeAgrupadoCTe", Column = "CCE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_AGRUPADO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeAgrupadoIntegracao", Column = "CCI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao> Integracoes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Numero.ToString();
            }
        }
    }
}
