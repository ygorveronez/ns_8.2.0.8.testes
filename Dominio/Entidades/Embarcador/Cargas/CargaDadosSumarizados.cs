using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_DADOS_SUMARIZADOS", EntityName = "CargaDadosSumarizados", Name = "Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados", NameType = typeof(CargaDadosSumarizados))]
    public class CargaDadosSumarizados : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoTomadores", Column = "CDS_CODIGO_INTEGRACAO_TOMADORES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string CodigoIntegracaoTomadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tomadores", Column = "CDS_TOMADORES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Tomadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Filiais", Column = "CDS_FILIAIS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Filiais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TiposDeOperacao", Column = "CDA_TIPOS_DE_OPERACAO", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string TiposDeOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoExpedidores", Column = "CDS_CODIGO_INTEGRACAO_EXPEDIDORES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string CodigoIntegracaoExpedidores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Expedidores", Column = "CDS_EXPEDIDORES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Expedidores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoRecebedores", Column = "CDS_CODIGO_INTEGRACAO_RECEBEDORES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string CodigoIntegracaoRecebedores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Recebedores", Column = "CDS_RECEBEDORES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Recebedores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoRemetentes", Column = "CDS_CODIGO_INTEGRACAO_REMETENTES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string CodigoIntegracaoRemetentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Remetentes", Column = "CDS_REMETENTES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Remetentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemetentesReais", Column = "CDS_REMETENTES_REAIS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string RemetentesReais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoDestinatarios", Column = "CDS_CODIGO_INTEGRACAO_DESTINATARIOS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string CodigoIntegracaoDestinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProvedoresOS", Column = "CDS_PROVEDORES_OS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ProvedoresOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Destinatarios", Column = "CDS_DESTINATARIOS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Destinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinatariosReais", Column = "CDS_DESTINATARIOS_REAIS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string DestinatariosReais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Origens", Column = "CDS_ORIGENS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Origens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Destinos", Column = "CDS_DESTINOS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PaisOrigens", Column = "CDS_PAIS_ORIGENS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string PaisOrigens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFOrigens", Column = "CDS_UF_ORIGENS", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string UFOrigens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PaisDestinos", Column = "CDS_PAIS_DESTINOS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string PaisDestinos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFDestinos", Column = "CDS_UF_DESTINOS", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string UFDestinos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Veiculos", Column = "CDS_VEICULOS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motoristas", Column = "CDS_MOTORISTAS", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroColetas", Column = "CDS_NUMERO_COLETAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroColetas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEntregas", Column = "CDS_NUMERO_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEntregas { get; set; }

        /// <summary>
        /// Quantidade de Destinatários distintos dos pedidos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEntregasFinais", Column = "CDS_NUMERO_ENTREGAS_FINAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroEntregasFinais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_QUANTIDADE_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeVolumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_QUANTIDADE_SKU", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeSKU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_POSSUI_PRODUTO_PERIGOSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiProdutoPerigoso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_QUANTIDADE_VOLUMES_NF", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeVolumesNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Distancia", Column = "CDS_DISTANCIA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal Distancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCTeAnteriorFilialEmissora", Column = "CDS_POSSUI_CTE_ANTERIOR_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCTeAnteriorFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCTe", Column = "CDS_POSSUI_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFS", Column = "CDS_POSSUI_NFS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFSManual", Column = "CDS_POSSUI_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiAverbacaoCTe", Column = "CDS_POSSUI_AVERBACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiAverbacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiAverbacaoMDFe", Column = "CDS_POSSUI_AVERBACAO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiAverbacaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoValePedagio", Column = "CDS_POSSUI_INTEGRACAO_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiRedespacho", Column = "CDS_POSSUI_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubContratantes", Column = "CDS_SUB_CONTRATANTES", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string SubContratantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotal", Column = "CDS_PESO_TOTAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquidoTotal", Column = "CDS_PESO_LIQUIDO_TOTAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquidoTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_OBSERVACAO_EMISSAO_CARGA", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_OBSERVACAO_EMISSAO_CARGA_TOMADOR", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaoCargaTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_OBSERVACAO_EMISSAO_CARGA_TIPO_OPERACAO", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaoCargaTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bookings", Column = "CDS_BOOKINGS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string Bookings { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Onda", Column = "CDS_ONDA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Onda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClusterRota", Column = "CDS_CLUSTER_ROTA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClusterRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoInicioViagem", Column = "CDS_DATA_PREVISAO_INICIO_VIAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumesTotal", Column = "CDS_VOLUMES_TOTAL", TypeType = typeof(int), Scale = 3, Precision = 18, NotNull = false)]
        public virtual int VolumesTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CubagemTotal", Column = "CDS_CUBAGEM_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal CubagemTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalProdutos", Column = "CDS_VALOR_TOTAL_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RotaEmbarcador", Column = "CDS_ROTA_EMBARCADOR", Type = "StringClob", NotNull = false)]
        public virtual string RotaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_DATA_ULTIMA_LIBERACAO", Type = "StringClob", NotNull = false)]
        public virtual string DataUltimaLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_USUARIO_CRIACAO_REMESSA", Type = "StringClob", NotNull = false)]
        public virtual string UsuarioCriacaoRemessa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_NUMERO_ORDEM", Type = "StringClob", NotNull = false)]
        public virtual string NumeroOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_PORTAL_RETIRA_EMPRESA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PortalRetiraEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_REGIAO_DESTINO", Type = "StringClob", NotNull = false)]
        public virtual string RegiaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Reentrega", Column = "CDS_REENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Reentrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasItinerario", Column = "CDS_DIAS_ITINERARIO", TypeType = typeof(int), Scale = 3, Precision = 18, NotNull = false)]
        public virtual int DiasItinerario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiAntecipacaoICMS", Column = "CDS_POSSUI_ANTECIPACAO_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiAntecipacaoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPedidoCliente", Column = "CDS_CODIGO_PEDIDO_CLIENTE", Type = "StringClob", NotNull = false)]
        public virtual string CodigoPedidoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalReentrega", Column = "CDS_PESO_TOTAL_REENTREGA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalReentrega { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestinatarios", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_DADOS_SUMARIZADOS_DESTINATARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestinatarios { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesRemetentes", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_DADOS_SUMARIZADOS_REMETENTES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesRemetentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosEtiquetasCorreios", Column = "CDS_NUMEROS_ETIQUETAS_CORREIOS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string NumerosEtiquetasCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PLPsCorreios", Column = "CDS_PLPS_CORREIOS", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string PLPsCorreios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioFixoCarregamento", Column = "CDS_HORARIO_CARREGAMENTO_FIXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioFixoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_DATA_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_DATA_PREVISAO_SAIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ZonasTransporte", Column = "CDS_ZONAS_TRANSPORTE", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ZonasTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCargaDivisaoCapacidade", Column = "CDS_MDC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade ModeloVeicularCargaDivisaoCapacidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemAgrupamentoDivisao", Column = "CDS_ORDEM_AGRUPAMENTO_DIVISAO", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemAgrupamentoDivisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalParqueamento", Column = "CDS_LOCAL_PARQUEAMENTO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string LocalParqueamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoEmbarcador", Column = "CDS_NUMERO_PEDIDO_EMBARCADOR", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string NumeroPedidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoriaPedidos", Column = "CDS_VALOR_TOTAL_MERCADORIA_PEDIDOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoriaPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoInformadaPeloTransportador", Column = "CDS_OBSERVACAO_INFORMADA_PELO_TRANSPORTADOR", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoInformadaPeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDS_EXCECAO_CAB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExcecaoCab { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CentrosDeDistribuicao", Column = "CDS_CENTROS_DE_DISTRIBUICAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string CentrosDeDistribuicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoFrete", Column = "CDS_CUSTO_FRETE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CustoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRelatorioDeEmbarque", Column = "CDS_OBSERVACAO_RELATORIO_DE_EMBARQUE", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoRelatorioDeEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaTrecho", Column = "CDS_CARGA_TRECHO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada? CargaTrecho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFrotasVeiculos", Column = "CDS_NUMEROS_FROTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroFrotasVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GlobalStatus", Column = "CDS_GLOBAL_STATUS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string GlobalStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCTesAnterioresComoCTeFilialEmissora", Column = "CDS_UTILIZAR_CTES_ANTERIORES_COMO_CTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCTesAnterioresComoCTeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FiliaisVenda", Column = "CDS_FILIAIS_VENDA", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string FiliaisVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CanalVenda", Column = "CDS_CANAIS_VENDA", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAlteracaoFrete", Column = "CDS_USUARIO_ALTERACAO_FRETE", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string UsuarioAlteracaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RoteirizarNaEtapaDeDadosTransporte", Column = "CDS_ROTEIRIZAR_NA_ETAPA_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizarNaEtapaDeDadosTransporte { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados)this.MemberwiseClone();
        }
    }
}
