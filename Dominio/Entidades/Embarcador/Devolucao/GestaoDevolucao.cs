using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO", EntityName = "GestaoDevolucao", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao", NameType = typeof(GestaoDevolucao))]

    public class GestaoDevolucao : EntidadeBase
    {
        public GestaoDevolucao()
        {
            DataCriacao = DateTime.Now;
        }

        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GDV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }
        #endregion

        #region Propriedades de Enumeradores
        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemRecebimento", Column = "GDV_ORIGEM_RECEBIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemGestaoDevolucao OrigemRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Geracao", Column = "GDV_GERACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeracaoGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeracaoGestaoDevolucao Geracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "GDV_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoDevolucao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNotas", Column = "GDV_TIPO_NOTAS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotasGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotasGestaoDevolucao TipoNotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFluxoGestaoDevolucao", Column = "GDV_TIPO_FLUXO_GESTAO_DEVOLUCAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoDevolucao TipoFluxoGestaoDevolucao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDevolucao", Column = "GDV_SITUACAO_DEVOLUCAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoDevolucao SituacaoDevolucao { get; set; }
        #endregion

        #region Propriedades de Entidades
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_DEVOLUCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucaoEtapa", Column = "GDV_ETAPA_ATUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucaoEtapa EtapaAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucaoLaudo", Column = "GDL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucaoLaudo Laudo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucaoDadosComplementares", Column = "GDC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucaoDadosComplementares DadosComplementares { get; set; }
        #endregion

        #region Propriedades
        [NHibernate.Mapping.Attributes.Property(0, Name = "Aprovada", Column = "GDV_APROVADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Aprovada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PosEntrega", Column = "GDV_POS_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PosEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Procedente", Column = "GDV_PROCEDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Procedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComPendenciaFinanceira", Column = "GDV_COM_PENDENCIA_FINANCEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComPendenciaFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "GDV_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoProcedencia", Column = "GDV_OBSERVACAO_PROCEDENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoProcedencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOrdemRemessa", Column = "GDV_OBSERVACAO_ORDEM_E_REMESSA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoOrdemRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotaFiscalPermuta", Column = "GDV_NUMERO_NOTA_FISCAL_PERMUTA", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroNotaFiscalPermuta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieNotaFiscalPermuta", Column = "GDV_SERIE_NOTA_FISCAL_PERMUTA", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SerieNotaFiscalPermuta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotaFiscalDevolucao", Column = "GDV_NUMERO_NOTA_FISCAL_DEVOLUCAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroNotaFiscalDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieNotaFiscalDevolucao", Column = "GDV_SERIE_NOTA_FISCAL_DEVOLUCAO", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SerieNotaFiscalDevolucao { get; set; }
        #endregion

        #region Collections
        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscaisDevolucao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL_DEVOLUCAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GDV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GestaoDevolucaoNotaFiscalDevolucao", Column = "GND_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> NotasFiscaisDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NotasFiscaisDeOrigem", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GDV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GestaoDevolucaoNotaFiscalOrigem", Column = "GNF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> NotasFiscaisDeOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Etapas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DEVOLUCAO_ETAPA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GDV_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "GestaoDevolucaoEtapa")]
        public virtual ICollection<GestaoDevolucaoEtapa> Etapas { get; set; }
        #endregion

        #region Atributos Virtuais
        public virtual string Descricao
        {
            get
            {
                return NotaFiscalDevolucao?.Descricao ?? NotaFiscalOrigem?.Descricao ?? Codigo.ToString();
            }
        }
        public virtual string AprovadaDescricao
        {
            get
            {
                return Aprovada ? "Sim" : "Não";
            }
        }
        public virtual string LaudoDescricao
        {
            get
            {
                return Laudo != null ? "Sim" : "Não";
            }
        }
        public virtual string PosEntregaDescricao
        {
            get
            {
                return PosEntrega ? "Sim" : "Não";
            }
        }
        public virtual string ProcedenteDescricao
        {
            get
            {
                return Procedente ? "Sim" : "Não";
            }
        }
        public virtual string ComPendenciaFinanceiraDescricao
        {
            get
            {
                return ComPendenciaFinanceira ? "Sim" : "Não";
            }
        }
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscalOrigem
        {
            get
            {
                return NotasFiscaisDeOrigem?.FirstOrDefault()?.XMLNotaFiscal;
            }
        }
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscalDevolucao
        {
            get
            {
                return NotasFiscaisDevolucao?.FirstOrDefault()?.XMLNotaFiscal;
            }
        }
        #endregion
    }
}
