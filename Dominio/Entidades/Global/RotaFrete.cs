using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE", DynamicUpdate = true, EntityName = "RotaFrete", Name = "Dominio.Entidades.Rota", NameType = typeof(RotaFrete))]
    public class RotaFrete : EntidadeBase
    {
        public RotaFrete()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.WebService.Integradora Integradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco RemetenteOutroEndereco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DISTRIBUIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Distribuidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Localidades.Regiao RegiaoDestino { get; set; }

        /// <summary>
        /// Na integração, quando não informado o expedidor porém a rota da carga estiver essa propriedade atribuidada irá adicionar como expedidor em todos os pedidos onde a origem do mesmo for difernete da origem da rota (localidade).
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_EXPEDIDOR_PEDIDOS_DIFERENTE_ORIGEM_ROTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ExpedidorPedidosDiferenteOrigemRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ROF_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "ROF_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_CODIGO_INTEGRACAO_CIOT", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteAgruparCargas", Column = "ROF_PERMITE_AGRUPAR_CARGAS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteAgruparCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "ROF_TIPO_DOCUMENTO", TypeType = typeof(Enumeradores.TipoDocumento), NotNull = false)]
        public virtual Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ROF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_TENTATIVAS_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativasIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_FINALIZAR_VIAGEM_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FinalizarViagemAutomaticamente { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "TempoDeViagemEmHoras", Column = "ROF_TEMPO_VIAGEM_EM_HORAS", TypeType = typeof(int), NotNull = false)]
        //public virtual int TempoDeViagemEmHoras { get; set; }

        ///<summary>
        ///Esse campo pode ser por dias ou minutos, dependendo do valor do campo "PadraoTempo"
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDeViagemEmMinutos", Column = "ROF_TEMPO_VIAGEM_EM_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDeViagemEmMinutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_PADRAO_TEMPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos? PadraoTempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VelocidadeMediaCarregado", Column = "ROF_VELOCIDADE_MEDIA_CARREGADO", TypeType = typeof(int), NotNull = false)]
        public virtual int VelocidadeMediaCarregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VelocidadeMediaVazio", Column = "ROF_VELOCIDADE_MEDIA_VAZIO", TypeType = typeof(int), NotNull = false)]
        public virtual int VelocidadeMediaVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_QUILOMETROS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Quilometros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_DETALHES", Type = "StringClob", NotNull = false)]
        public virtual string Detalhes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ROF_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FilialDistribuidora", Column = "ROF_FILIAL_DISTRIBUIDORA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string FilialDistribuidora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_CODIGO_INTEGRACAO_NOX", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoNOX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_POSSUI_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_VINCULAR_MOTORISTA_FILA_CARREGAMENTO_MANUALMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool VincularMotoristaFilaCarregamentoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_GERAR_REDESPACHO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarRedespachoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PolilinhaRota", Column = "ROF_POLINHA_ROTA", Type = "StringClob", NotNull = false)]
        public virtual string PolilinhaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacao", Column = "ROF_TIPO_ULTIMO_PONTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }

        /// <summary>
        /// Disponível para escolha apenas quando preencher a aba Estado Destino
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacaoPorEstado", Column = "ROF_TIPO_ULTIMO_PONTO_POR_ESTADO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? TipoUltimoPontoRoteirizacaoPorEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_ROTA_ROTEIRIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RotaRoteirizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_APENAS_OBTER_PRACAS_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApenasObterPracasPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_ROTA_ROTEIRIZADA_POR_LOCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RotaRoteirizadaPorLocal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRoteririzacao", Column = "ROF_DATA_ROTEIRIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRoteririzacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsultaPracasPedido", Column = "ROF_DATA_CONSULTA_PRACAS_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConsultaPracasPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicioCarregamento", Column = "ROF_HORA_INICIO_CARREGAMENTO", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan? HoraInicioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraLimiteSaidaCD", Column = "ROF_HORA_LIMITE_SAIDA_CD", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HoraLimiteSaidaCD { get; set; }


        //[NHibernate.Mapping.Attributes.Property(0, Name = "TempoCarregamento", Column = "ROF_TEMPO_CARREGAMENTO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        //public virtual TimeSpan? TempoCarregamento { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarga", Column = "ROF_TEMPO_DESCARGA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        //public virtual TimeSpan? TempoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescargaTicks", Column = "ROF_TEMPO_DESCARGA_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoDescargaTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoCarregamentoTicks", Column = "ROF_TEMPO_CARREGAMENTO_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoCarregamentoTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_MOTIVO_FALHA_ROTEIRIZACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoFalhaRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_ESTADO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Dominio.Entidades.Estado> EstadosOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LocalidadesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_LOCALIDADE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Localidade> LocalidadesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_CLIENTE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> ClientesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Estados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_ESTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Dominio.Entidades.Estado> Estados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Localidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_LOCALIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteLocalidade", Column = "RFL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFreteLocalidade> Localidades { get; set; }

        //[NHibernate.Mapping.Attributes.Set(0, Name = "Destinatarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_DESTINATARIO")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        //public virtual ICollection<Dominio.Entidades.Cliente> Destinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Destinatarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_DESTINATARIO_ORDEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteDestinatarios", Column = "RFD_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFreteDestinatarios> Destinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Fronteiras", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_FRONTEIRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteFronteira", Column = "RFF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFreteFronteira> Fronteiras { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Coletas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_COLETA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Coletas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PontoPassagemPreDefinido", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_PONTO_PRE_DEFINIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PontoPassagemPreDefinido", Column = "RPD_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido> PontoPassagemPreDefinido { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PostosFiscais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_POSTO_FISCAL_ROTA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PostoFiscalRotaFrete", Column = "PFR_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.PostoFiscalRotaFrete> PostosFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Restricoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_RESTRICAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteRestricao", Column = "RFR_CODIGO")]
        public virtual ICollection<RotaFreteRestricao> Restricoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Empresas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteEmpresa", Column = "RFE_CODIGO")]
        public virtual ICollection<RotaFreteEmpresa> Empresas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PracasPedagio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_PRACAS_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PracaPedagio", Column = "PRP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> PracasPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CEPsDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_CEP")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteCEP", Column = "ROC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> CEPsDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RotaFreteFiliais", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_FILIAIS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteFiliais", Column = "RFF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFreteFiliais> RotaFreteFiliais { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RotaFreteTiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_TIPOS_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFreteTiposCarga", Column = "RFT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFreteTiposCarga> RotaFreteTiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacaoRotaFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacaoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsarDistribuidorComoTransportadorRota", Column = "ROF_USAR_DISTRIBUIDOR_TRANSPORTADOR_ROTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UsarDistribuidorComoTransportadorRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDaRoteirizacao", Column = "ROF_SITUACAO_ROTEIRIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao SituacaoDaRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_PRE_CARGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_TIPO_CARREGAMENTO_IDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo? TipoCarregamentoIda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_TIPO_CARREGAMENTO_VOLTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo? TipoCarregamentoVolta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoValePedagio", Column = "ROF_CODIGO_INTEGRACAO_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoValePedagioRetorno", Column = "ROF_CODIGO_INTEGRACAO_VALE_PEDAGIO_RETORNO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracaoValePedagioRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoGerenciadoraRisco", Column = "ROF_CODIGO_INTEGRACAO_GERENCIADORA_RISCO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracaoGerenciadoraRisco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFreteClassificacao", Column = "RFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao Classificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_ADICIONADO_VIA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionadoViaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_POSSUI_TRANSPORTADORES_EXCLUSIVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiTransportadoresExclusivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_ROTA_EXCLUSIVA_COMPRA_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RotaExclusivaCompraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_VALIDAR_PARA_QUALQUER_DESTINATARIO_INFORMADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarParaQualquerDestinatarioInformado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_POSSUI_VEICULOS_INFORMADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiVeiculosInformados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_VOLTAR_PELO_MESMO_CAMINHO_IDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VoltarPeloMesmoCaminhoIda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROF_UTILIZAR_DISTANCIA_ROTA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDistanciaRotaCarga { get; set; }

        #region Propriedades Virtuais

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual int ObterTempoViagemEmMinutos()
        {
            if (TempoDeViagemEmMinutos > 0)
                return TempoDeViagemEmMinutos;

            //if (TempoDeViagemEmHoras > 0)
            //    return TempoDeViagemEmHoras * 60;

            decimal mediaKmHora = 50;

            return (int)(Math.Round((Quilometros / mediaKmHora), 0) * 60);
        }

        public virtual TimeSpan TempoCarregamento
        {
            get { return TimeSpan.FromTicks(TempoCarregamentoTicks); }
            set { TempoCarregamentoTicks = value.Ticks; }
        }

        public virtual TimeSpan TempoDescarga
        {
            get { return TimeSpan.FromTicks(TempoDescargaTicks); }
            set { TempoDescargaTicks = value.Ticks; }
        }

        public virtual bool DestinoExato { get; set; }
        #endregion
    }
}