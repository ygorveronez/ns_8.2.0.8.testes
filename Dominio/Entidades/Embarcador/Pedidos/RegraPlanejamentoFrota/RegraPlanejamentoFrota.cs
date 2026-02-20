using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PLANEJAMENTO_FROTA", EntityName = "RegraPlanejamentoFrota", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota", NameType = typeof(RegraPlanejamentoFrota))]
    public class RegraPlanejamentoFrota : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RPF_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_NUMERO_SEQUENCIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_VIGENCIA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_VIGENCIA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RPF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDeDaMercadoria", Column = "RPF_VALOR_DE_DA_MERCADORIA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorDeDaMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAteDaMercadoria", Column = "RPF_VALOR_ATE_DA_MERCADORIA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorAteDaMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_DESCRICAO_ORIGEM", Type = "StringClob", NotNull = false)]
        public virtual string DescricaoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TFC_DESCRICAO_DESTINO", Type = "StringClob", NotNull = false)]
        public virtual string DescricaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasVeiculosComRastreadorAtivo", Column = "RPF_APENAS_VEICULOS_COM_RASTREADOR_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasVeiculosComRastreadorAtivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasVeiculoQuePossuiTravaQuintaRoda", Column = "RPF_APENAS_VEICULO_QUE_POSSUI_TRAVA_QUINTA_RODA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasVeiculoQuePossuiTravaQuintaRoda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasVeiculoQuePossuiImobilizador", Column = "RPF_APENAS_VEICULO_QUE_POSSUI_IMOBILIZADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasVeiculoQuePossuiImobilizador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasTracaoComIdadeMaxima", Column = "RPF_APENAS_TRACAO_COM_IDADE_MAXIMA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasTracaoComIdadeMaxima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasReboqueComIdadeMaxima", Column = "RPF_APENAS_REBOQUE_COM_IDADE_MAXIMA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasReboqueComIdadeMaxima { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimitarPelaAlturaCarreta", Column = "RPF_LIMITAR_PELA_ALTURA_CARRETA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LimitarPelaAlturaCarreta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasComInformacoesDeIscaInformadaNoPedido", Column = "RPF_APENAS_COM_INFORMACOES_DE_ISCA_INFORMADA_NO_PEDIDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasComInformacoesDeIscaInformadaNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApenasComInformacoesDeEscoltaInformadaNoPedido", Column = "RPF_APENAS_COM_INFORMACOES_DE_ESCOLTA_INFORMADA_NO_PEDIDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ApenasComInformacoesDeEscoltaInformadaNoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimitarPelaAlturaCavalo", Column = "RPF_LIMITAR_PELA_ALTURA_CAVALO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool LimitarPelaAlturaCavalo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_IDADE_MAXIMA_TRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int IdadeMaximaTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_IDADE_MAXIMA_REBOQUE", TypeType = typeof(int), NotNull = false)]
        public virtual int IdadeMaximaReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_QUANTIDADE_ISCA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeIsca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_QUANTIDADE_ESCOLTA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEscolta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetrosAlturaCarreta", Column = "RPF_METROS_ALTURA_CARRETA", TypeType = typeof(decimal), Scale = 4, Precision = 10, NotNull = false)]
        public virtual decimal MetrosAlturaCarreta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetrosAlturaCavalo", Column = "RPF_METROS_ALTURA_CAVALO", TypeType = typeof(decimal), Scale = 4, Precision = 10, NotNull = false)]
        public virtual decimal MetrosAlturaCavalo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPF_TIPO_FROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFrota), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFrota? TipoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCarga", Column = "RPF_QUANTIDADE_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoQuantidadeCarga", Column = "RPF_QUANTIDADE_PERIODO", TypeType = typeof(int), NotNull = false)]
        public virtual int PeriodoQuantidadeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPeriodoQuantidadeCarga", Column = "RPF_TIPO_PERIODO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno TipoPeriodoQuantidadeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarQuantidadeVeiculoEReboque", Column = "RPF_VALIDAR_QTD_VEICULO_REBOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarQuantidadeVeiculoEReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarPorQuantidadeMotorista", Column = "RPF_VALIDAR_POR_QUANTIDADE_MOTORISTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidarPorQuantidadeMotorista { get; set; }

        #region Origens

        [NHibernate.Mapping.Attributes.Set(0, Name = "Origens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Origens { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_ESTADO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PaisesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_PAIS_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pais", Column = "PAI_CODIGO")]
        public virtual ICollection<Pais> PaisesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegioesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_REGIAO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Regiao", Column = "REG_CODIGO")]
        public virtual ICollection<Localidades.Regiao> RegioesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CLIENTE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_ROTA_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CEPsOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CEP_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegraPlanejamentoFrotaCEPOrigem")]
        public virtual ICollection<RegraPlanejamentoFrotaCEPOrigem> CEPsOrigem { get; set; }

        #endregion Origens



        #region Destinos

        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_ESTADO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PaisesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_PAIS_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Pais", Column = "PAI_CODIGO")]
        public virtual ICollection<Pais> PaisesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegioesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_REGIAO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Regiao", Column = "REG_CODIGO")]
        public virtual ICollection<Localidades.Regiao> RegioesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CLIENTE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RotasDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_ROTA_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.RotaFrete> RotasDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CEPsDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CEP_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegraPlanejamentoFrotaCEPDestino")]
        public virtual ICollection<RegraPlanejamentoFrotaCEPDestino> CEPsDestino { get; set; }

        #endregion Destinos



        #region Listas Relacionais

        [NHibernate.Mapping.Attributes.Set(0, Name = "GrupoPessoas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_GRUPO_PESSOAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoas", Column = "GRP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposDeCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> TiposDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CentrosResultado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CENTRO_RESULTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroResultado", Column = "CRE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> CentrosResultado { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeicularCargaTracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_MODELO_VEICULAR_CARGA_TRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeicularCargaTracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeicularCargaReboque", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_MODELO_VEICULAR_CARGA_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeicularCargaReboque { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TecnologiaRastreadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_TECNOLOGIA_RASTREADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TecnologiaRastreador", Column = "TRA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador> TecnologiaRastreadores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Licencas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_LICENCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Licenca", Column = "LIC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Configuracoes.Licenca> Licencas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeicularesCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> ModelosVeicularesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "NiveisCooperados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_NIVEL_COOPERADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoTerceiro", Column = "TPT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> NiveisCooperados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LiberacoesGR", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_LIBERACAO_GR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Licenca", Column = "LIC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Configuracoes.Licenca> LiberacoesGR { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LiberacoesGRVeiculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_LIBERACAO_GR_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Licenca", Column = "LIC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Configuracoes.Licenca> LiberacoesGRVeiculo { get; set; }

        #endregion Listas Relacionais



        #region Listas ENUM'S

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposPropriedade", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_TIPO_PROPRIEDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "RPF_TIPO_PROPRIEDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade> TiposPropriedade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarroceria", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_TIPO_CARROCERIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "RPF_TIPO_CARROCERIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria> TiposCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposProprietarioVeiculo", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_TIPO_PROPRIETARIO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "RPF_TIPO_PROPRIETARIO_VEICULO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo> TiposProprietarioVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CategoriasHabilitacao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CATEGORIA_HABILITACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "RPF_CATEGORIA_HABILITACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao> CategoriasHabilitacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposRodado", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_TIPO_RODADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "RPF_TIPO_RODADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRodado), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoRodado> TiposRodado { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CondicaoLicencas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CONDICAO_LICENCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "RPF_CONDICAO_LICENCA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca> CondicaoLicencas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CondicaoLiberacoesGR", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PLANEJAMENTO_FROTA_CONDICAO_LIBERACAO_GR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "RPF_CONDICAO_LIBERACAO_GR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR> CondicaoLiberacoesGR { get; set; }


        #endregion Listas ENUM'S



        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}