using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURAMENTO_LOTE", EntityName = "FaturamentoLote", Name = "Dominio.Entidades.Embarcador.Fatura.FaturamentoLote", NameType = typeof(FaturamentoLote))]
    public class FaturamentoLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "FAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "FAL_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "FAL_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFatura", Column = "FAL_DATA_FATURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "FAL_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "FAL_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAL_NUMERO_BOOKING", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FAL_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "FAL_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "FAL_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "FAL_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCTe", Column = "FAL_TIPO_CTE", TypeType = typeof(Dominio.Enumeradores.TipoCTE), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCTE? TipoCTe { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturamentoAutomatico", Column = "FAL_FATURAMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificadoOperador", Column = "FAL_NOTIFICADO_OPERADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificadoOperador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAFaturar", Column = "FAL_DATA_A_FATURAR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAFaturar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarFaturamentoParaClientesExclusivos", Column = "FAL_GERAR_FATURAMENTO_PARA_CLIENTES_EXCLUSIVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturamentoParaClientesExclusivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsultarTodos", Column = "FAL_CONSULTAR_TODOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarTodos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoPropostaMultimodal", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURAMENTO_LOTE_TIPO_PROPOSTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "FAL_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = true)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TipoPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Faturas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURAMENTO_LOTE_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Fatura", Column = "FAT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Fatura.Fatura> Faturas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURAMENTO_LOTE_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaLoteCTe", Column = "FLC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe> CTes { get; set; }
    }
}
