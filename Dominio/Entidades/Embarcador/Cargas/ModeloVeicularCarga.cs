using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_VEICULAR_CARGA", EntityName = "ModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga", NameType = typeof(ModeloVeicularCarga))]
    public class ModeloVeicularCarga : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_MODELO_CALCULO_FRANQUIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloCalculoFranquia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoModeloVeicular", Column = "MVG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoModeloVeicular GrupoModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.ContainerTipo ContainerTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_CODIGO_INTEGRACAO_GERENCIADORA_RISCO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoGerenciadoraRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MVC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadePesoTransporte", Column = "MVC_CAPACIDADE_PESO_TRANSPORTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal CapacidadePesoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaPesoExtra", Column = "MVC_TOLERANCIA_PESO_EXTRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ToleranciaPesoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaPesoMenor", Column = "MVC_TOLERANCIA_PESO_MENOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ToleranciaPesoMenor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cubagem", Column = "MVC_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Cubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaMinimaCubagem", Column = "MVC_TOLERANCIA_MINIMA_CUBAGEM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ToleranciaMinimaCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoPaletizado", Column = "MVC_VEICULO_PALETIZADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool VeiculoPaletizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloControlaCubagem", Column = "MVC_MODELO_CONTROLA_CUBAGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ModeloControlaCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_MODELO_ACEITA_LOCALIZADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModeloVeicularAceitaLocalizador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPaletes", Column = "MVC_NUMERO_PALETES", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaMinimaPaletes", Column = "MVC_TOLERANCIA_MINIMA_PALLETS", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaMinimaPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcupacaoCubicaPaletes", Column = "MVC_OCUPACAO_CUBICA_PALETES", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal OcupacaoCubicaPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEixos", Column = "MVC_NUMERO_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_PADRAO_EIXOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo? PadraoEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasRealizarProximoChecklist", Column = "MVC_DIAS_REALIZAR_PROXIMO_CHECKLIST", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasRealizarProximoChecklist { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CalibragemAposKm", Column = "MVC_CALIBRAGEM_APOS_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int CalibragemAposKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEstepes", Column = "MVC_QUANTIDADE_ESTEPES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEstepes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAlertaManutencao", Column = "MVC_GERAR_ALERTA_MANUTENCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAlertaManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirDefinicaoReboquePedido", Column = "MVC_EXIGIR_DEFINICAO_REBOQUE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirDefinicaoReboquePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarLicencaVeiculo", Column = "MVC_VALIDAR_LICENCA_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarLicencaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_INTEGRAR_DADOS_TRANSPORTE_BRASILRISK_AO_ATUALIZAR_VEICULO_NA_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertarOperadorPesoExcederCapacidade", Column = "MVC_ALERTAR_PESO_EXCEDER_CAPACIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertarOperadorPesoExcederCapacidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSolicitarNoChecklist", Column = "MVC_NAO_SOLICITAR_NO_CHECKLIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSolicitarNoChecklist { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FatorEmissaoCO2", Column = "MVC_FATOR_EMISSAO_CO2", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal FatorEmissaoCO2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirInformacaoLacreJanelaCarregamentoPortalTransportador", Column = "MVC_EXIGIR_INFORMACAO_LACRE_JANELA_CARREGAMENTO_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformacaoLacreJanelaCarregamentoPortalTransportador { get; set; }

        /// <summary>
        /// Puta gambiarra que fiz pq aqui não tem tempo nem pra respira (Rodrigo Romanovski)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloTracaoReboquePadrao", Column = "MVC_MODELO_TRACAO_REBOQUE_PADRAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ModeloTracaoReboquePadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MVC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MVC_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeCapacidade", Column = "MVC_UNIDADE_CAPACIDADE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade UnidadeCapacidade { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_DE_CARGA_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoCargaModeloVeicular", Column = "CMV_CODIGO")]
        public virtual ICollection<TipoCargaModeloVeicular> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Eixos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_VEICULAR_CARGA_EIXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCargaEixo", Column = "MEX_CODIGO")]
        public virtual ICollection<ModeloVeicularCargaEixo> Eixos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Estepes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_VEICULAR_CARGA_ESTEPE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCargaEstepe", Column = "MES_CODIGO")]
        public virtual ICollection<ModeloVeicularCargaEstepe> Estepes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CodigosIntegracao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_VEICULAR_CODIGOS_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "MVC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual ICollection<string> CodigosIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Produtos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_VEICULAR_CARGA_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO")]
        public virtual ICollection<Produtos.ProdutoEmbarcador> Produtos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DivisoesCapacidade", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_VEICULAR_CARGA_DIVISAO_CAPACIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCargaDivisaoCapacidade", Column = "MDC_CODIGO")]
        public virtual ICollection<ModeloVeicularCargaDivisaoCapacidade> DivisoesCapacidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "GruposProdutos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_VEICULAR_CARGA_GRUPO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoProduto", Column = "GPR_CODIGO")]
        public virtual ICollection<Produtos.GrupoProduto> GruposProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_CODIGO_INTEGRACAO_GOLDEN_SERVICE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoGoldenService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_CODIGO_TIPO_CARGA_ANTT", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoTipoCargaANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_VELOCIDADE_MEDIA", TypeType = typeof(int), NotNull = false)]
        public virtual int VelocidadeMedia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReboques", Column = "MVC_NUMERO_REBOQUES", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroReboques { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_CUSTO_PNEU_KM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal? CustoPneuKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_NUMERO_EIXOS_SUSPENSOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroEixosSuspensos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Altura", Column = "MVC_ALTURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Altura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Largura", Column = "MVC_LARGURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comprimento", Column = "MVC_COMPRIMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Comprimento { get; set; }

        /// <summary>
        /// Valida se no app o Motorista pode ou não colocar um valor nas divisões de capacidade maior do que o máximo estabelecido. Utilizado para 
        /// produtos vão separados em divisões do veículo, como Suínos, gado e leite.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarCapacidadeMaximaNoApp", Column = "MVC_VALIDAR_CAPACIDADE_MAXIMA_NO_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarCapacidadeMaximaNoApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePaletes", Column = "MVC_QUANTIDADE_PALETES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadePaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "MVC_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_TEMPO_EMISSAO_FLUXO_PATIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoEmissaoFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_TIPO_SEMIRREBOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoSemirreboque { get; set; }

        #endregion Propriedades

        #region Propriedades - Repom

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_TIPO_VEICULO_REPOM", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoVeiculoRepom { get; set; }

        #endregion Propriedades - Repom

        #region Propriedades - Pamcard

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_TIPO_VEICULO_PAMCARD", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoVeiculoPamcard { get; set; }

        #endregion Propriedades - Pamcard

        #region Propriedades - Target

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_CATEGORIA_VEICULO_TARGET", TypeType = typeof(int), NotNull = false)]
        public virtual int CategoriaVeiculoTarget { get; set; }

        #endregion Propriedades - Target

        #region Propriedades - A52

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_TIPO_VEICULO_A52", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string TipoVeiculoA52 { get; set; }

        #endregion Propriedades - A52

        #region Propriedades - Gadle

        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_TIPO_VEICULO_GADLE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string TipoVeiculoGadle { get; set; }

        #endregion Propriedades - Gadle

        #region Propriedades - Bovinos

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArrobaMinima", Column = "MVC_ARROBA_MINIMA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ArrobaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArrobaMaxima", Column = "MVC_ARROBA_MAXIMA", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ArrobaMaxima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CabecaMinima", Column = "MVC_CABECA_MINIMA", TypeType = typeof(int), NotNull = false)]
        public virtual int CabecaMinima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CabecaMaxima", Column = "MVC_CABECA_MAXIMA", TypeType = typeof(int), NotNull = false)]
        public virtual int CabecaMaxima { get; set; }

        #endregion Propriedades - Bovinos

        #region Propriedades - Trizy
        [NHibernate.Mapping.Attributes.Property(0, Column = "MVC_LAYOUT_SUPER_APP_ID", TypeType = typeof(string), Length = 24, NotNull = false)]
        public virtual string LayoutSuperAppId { get; set; }

        #endregion Propriedades - Trizy

        #region Propriedades com Regras

        public virtual string DescricaoAtivo
        {
            get
            {
                return this.Ativo ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo;
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual bool Equals(ModeloVeicularCarga other)
        {
            return (other.Codigo == this.Codigo);
        }

        public virtual decimal ObterCubagemDisponivel()
        {
            return ModeloControlaCubagem ? 0 : Cubagem - ObterOcupacaoCubicaPaletes();
        }

        public virtual decimal ObterOcupacaoCubicaPaletes()
        {
            return VeiculoPaletizado ? OcupacaoCubicaPaletes : 0m;
        }

        #endregion Métodos Públicos
    }
}
