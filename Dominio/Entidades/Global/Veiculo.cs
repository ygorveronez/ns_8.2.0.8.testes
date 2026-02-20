using Dominio.Entidades.Embarcador.Veiculos;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_VEICULO", EntityName = "Veiculo", Name = "Dominio.Entidades.Veiculo", NameType = typeof(Veiculo))]
    public class Veiculo : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public Veiculo()
        {
            DataCadastro = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Empresas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_EMPRESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual IList<Empresa> Empresas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Equipamentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_EQUIPAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Equipamento", Column = "EQP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Veiculos.Equipamento> Equipamentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_EMPLACAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade? LocalidadeEmplacamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemAtual", Column = "VEI_KMATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KilometragemAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemAnterior", Column = "VEI_KMANTERIOR", TypeType = typeof(int), NotNull = false)]
        public virtual int KilometragemAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "VEI_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chassi", Column = "VEI_CHASSI", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Chassi { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "VEI_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "VEI_DATA_ATULIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompra", Column = "VEI_DATACOMPRA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeGerenciadoraRisco", Column = "VEI_DATA_VALIDADE_GERENCIADORA_RISCO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeGerenciadoraRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimoChecklist", Column = "VEI_DATA_ULTIMO_CHECKLIST", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoChecklist { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_DATA_VALIDADE_ANTT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeLiberacaoSeguradora", Column = "VEI_DATA_VALIDADE_LIBERACAO_SEGURADORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeLiberacaoSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAquisicao", Column = "VEI_VALORAQUIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAquisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeTanque", Column = "VEI_CAPTANQUE", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeTanque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeMaximaTanque", Column = "VEI_CAPACIDADE_MAXIMA_TANQUE", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal CapacidadeMaximaTanque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeM3", Column = "VEI_CAP_M3", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeM3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLicenca", Column = "VEI_DATALICENCA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLicenca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoFabricacao", Column = "VEI_ANO", TypeType = typeof(int), NotNull = false)]
        public virtual int AnoFabricacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoModelo", Column = "VEI_ANOMODELO", TypeType = typeof(int), NotNull = false)]
        public virtual int AnoModelo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PosicaoAtual", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_POSICAO_ATUAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PosicaoAtual", Column = "POA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> PosicaoAtual { get; set; }

        /// <summary>
        /// Q - QUITADO
        /// A - ALIENADO
        /// F - FINANCIADO
        /// O - OUTROS
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "VEI_SITUACAO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Situacao { get; set; }

        /// <summary>
        /// A - ATIVO
        /// I - INATIVO
        /// </summary>
        //[NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VEI_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status
        {
            get
            {
                //SalvarLog(System.Environment.StackTrace);
                return Ativo ? "A" : "I";
            }
            set
            {
                //SalvarLog(System.Environment.StackTrace);
                Ativo = value == "A";
            }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "VEI_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PendenteIntegracaoEmbarcador", Column = "VEI_PENDENTE_INTEGRACAO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMotor", Column = "VEI_NUMMOTOR", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NumeroMotor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaPadrao", Column = "VEI_MEDIAPADRAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "VEI_OBS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoGarantiaPlena", Column = "VEI_VCTOGARANTIAPLENA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoGarantiaPlena { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoGarantiaEscalonada", Column = "VEI_VCTOGARANTIAESCALONADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoGarantiaEscalonada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Renavam", Column = "VEI_RENAVAM", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string Renavam { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Contrato", Column = "VEI_CONTRATO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Contrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tara", Column = "VEI_TARA", TypeType = typeof(int), NotNull = false)]
        public virtual int Tara { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XCampo", Column = "VEI_XCAMPO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string XCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XTexto", Column = "VEI_XTEXTO", TypeType = typeof(string), Length = 160, NotNull = false)]
        public virtual string XTexto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorContainerAverbacao", Column = "VEI_VALOR_CONTAINER_AVERBACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorContainerAverbacao { get; set; }

        /// <summary>
        /// P - PROPRIO
        /// T - TERCEIRO
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "VEI_TIPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        /// <summary>
        /// G - GASOLINA
        /// D - DIESEL S-500
        /// E - ETANOL
        /// I - DIESEL S-10
        /// O - OUTROS
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCombustivel", Column = "VEI_TIPOCOMBUSTIVEL", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoCombustivel { get; set; }

        /// <summary>
        /// 0 - TRACAO
        /// 1 - REBOQUE
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculo", Column = "VEI_TIPOVEICULO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string TipoVeiculo { get; set; }

        /// <summary>
        /// 00 - NAO APLICADO
        /// 01 - TRUCK
        /// 02 - TOCO
        /// 03 - CAVALO
        /// 04 - VAN
        /// 05 - UTILITARIO
        /// 06 - OUTROS
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRodado", Column = "VEI_TIPORODADO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoRodado { get; set; }

        /// <summary>
        /// 00 - NAO APLICADO
        /// 01 - ABERTA
        /// 02 - FECHADA / BAU
        /// 03 - GRANEL
        /// 04 - PORTA CONTAINER
        /// 05 - SIDER
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarroceria", Column = "VEI_TIPO_CARROCERIA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeKG", Column = "VEI_CAP_KG", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeKG { get; set; }

        //[Obsolete("Migrado para lista Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "NomeMotorista", Column = "VEI_NOME_MOTORISTA", TypeType = typeof(string), Length = 200, NotNull = false)]
        //public virtual string NomeMotorista { get; set; }

        //[Obsolete("Migrado para lista Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista")]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "CPFMotorista", Column = "VEI_CPF_MOTORISTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        //public virtual string CPFMotorista { get; set; }

        //[Obsolete("Migrado para lista Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeiculo", Column = "VMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeiculo Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaVeiculo", Column = "VMA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MarcaVeiculo Marca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloCarroceria", Column = "MCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.ModeloCarroceria ModeloCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoVeiculo", Column = "VTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoVeiculo TipoDoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFrota", Column = "VEI_NUMERO_FROTA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRemocaoVinculo", Column = "VEI_DATA_REMOCAO_VINCULO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRemocaoVinculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.SegmentoVeiculo SegmentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CPF_CNPJ", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente LocalAtualFisicoDoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "VEI_LATITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "VEI_LONGITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoManobra", Column = "VEI_TIPO_MANOBRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoManobra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AreaVeiculoPosicao", Column = "AVP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Logistica.AreaVeiculoPosicao LocalAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Higienizado", Column = "VEI_HIGIENIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Higienizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmViagem", Column = "VEI_EM_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_NAO_INTEGRAR_OPENTECH", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIntegrarOpentech { get; set; }

        #region Proprietário

        [NHibernate.Mapping.Attributes.Property(0, Name = "RNTRC", Column = "VEI_RNTRC", TypeType = typeof(int), NotNull = false)]
        public virtual int RNTRC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoProprietario", Column = "VEI_TIPO_PROPRIETARIO", TypeType = typeof(Enumeradores.TipoProprietarioVeiculo), NotNull = false)]
        public virtual Enumeradores.TipoProprietarioVeiculo TipoProprietario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamentoCIOT", Column = "VEI_TIPO_PAGAMENTO_CIOT", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe? TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChavePIX", Column = "VEI_TIPO_CHAVE_PIX_CIOT", TypeType = typeof(Dominio.ObjetosDeValor.Enumerador.TipoChavePix), Length = 200, NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Enumerador.TipoChavePix TipoChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChavePIXCIOT", Column = "VEI_CHAVE_PIX_CIOT", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChavePIXCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgenciaCIOT", Column = "VEI_AGENCIA_CIOT", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string AgenciaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContaCIOT", Column = "VEI_CONTA_CIOT", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string ContaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaPagamentoCIOT", Column = "VEI_FORMA_PAGAMENTO_CIOT", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento? FormaPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteCIOT", Column = "VEI_VALOR_FRETE_CIOT", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorFreteCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamentoCIOT", Column = "VEI_VALOR_ADIANTAMENTO_CIOT", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorAdiantamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoCIOT", Column = "VEI_DATA_VENCIMENTO_CIOT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJInstituicaoPagamentoCIOT", Column = "VEI_CNPJ_INSTITUICAO_PAGAMENTO_CIOT", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJInstituicaoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTe", Column = "VEI_OBS_CTE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "VEI_PROPRIETARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Proprietario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "VEI_LOCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Locador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CIOT", Column = "VEI_CIOT", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual String CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TAF", Column = "VEI_TAF", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string TAF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NroRegEstadual", Column = "VEI_NUMERO_REG_ESTADUAL", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string NroRegEstadual { get; set; }

        #endregion

        #region Pedágio

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "VEI_FORNECEDOR_VALE_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "VEI_RESPONSAVEL_VALE_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ResponsavelValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCompraValePedagio", Column = "VEI_NUMERO_COMPRA_VALE_PEDAGIO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual String NumeroCompraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagio", Column = "VEI_VALOR_VALE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiTagValePedagio", Column = "VEI_POSSUI_TAG_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiTagValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigenciaTagValePedagio", Column = "VEI_DATA_INICIO_VIGENCIA_TAG_VALE_PEDAGIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioVigenciaTagValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimVigenciaTagValePedagio", Column = "VEI_DATA_FIM_VIGENCIA_TAG_VALE_PEDAGIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimVigenciaTagValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoComprarValePedagio", Column = "VEI_NAO_COMPRAR_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoComprarValePedagioRetorno", Column = "VEI_NAO_COMPRAR_VALE_PEDAGIO_RETORNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagioRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_MEIO_COMPRA_VALE_PEDAGIO_TARGET", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget? ModoCompraValePedagioTarget { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaDeducaoValePedagio", Column = "VEI_FORMA_DEDUCAO_VALE_PEDAGIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaDeducaoValePedagio), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaDeducaoValePedagio? FormaDeducaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCartaoValePedagio", Column = "VEI_NUMERO_CARTAO_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroCartaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCartaoAbastecimento", Column = "VEI_NUMERO_CARTAO_ABASTECIMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroCartaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TagSemParar", Column = "VEI_TAG_SEM_PARAR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TagSemParar { get; set; }

        #endregion Pedágio

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoVazio", Column = "VEI_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvisadoCarregamento", Column = "VEI_AVISADO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisadoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_ATUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraVeiculoVazio", Column = "VEI_DATA_HORA_VEICULO_VAZIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraVeiculoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraAvisoCarregamento", Column = "VEI_DATA_HORA_AVISO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraAvisoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_SITUACAO_VEICULO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo? SituacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_SITUACAO_CADASTRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo SituacaoCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_TIPO_FROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFrota), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFrota? TipoFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraPrevisaoDisponivel", Column = "VEI_DATA_HORA_PREVISAO_DISPONIVEL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHoraPrevisaoDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "VEI_RESPONSAVEL_CIOT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ResponsavelCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoBloqueado", Column = "VEI_VEICULO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoBloqueio", Column = "VEI_MOTIVO_BLOQUEIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MotivoBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSuspensaoInicio", Column = "VEI_SUSPENSAO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSuspensaoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSuspensaoFim", Column = "VEI_SUSPENSAO_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSuspensaoFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_COR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Cor { get; set; } // Campo Descontinuado

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CorVeiculo", Column = "CDV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.CorVeiculo CorVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_PADRAO_EMISSAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PadraoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_FATOR_EMISSAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string FatorEmissao { get; set; }

        #region Rastreador

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_POSSUI_RASTREADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiRastreador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_POSSUI_CONTROLE_DISPONIBILIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiControleDisponibilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_POSSUI_TRAVA_QUINTA_RODA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiTravaQuintaDeRoda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_POSSUI_TELEMETRIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiTelemetria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_VEICULO_UTILIZANDO_TRANSPORTE_FROTAS_DEDICADAS_OU_FIDELIZADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_POSSUI_IMOBILIZADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiImobilizador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_NUMERO_EQUIPAMENTO_RASTREADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroEquipamentoRastreador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TecnologiaRastreador", Column = "TRA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.TecnologiaRastreador TecnologiaRastreador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoComunicacaoRastreador", Column = "TCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.TipoComunicacaoRastreador TipoComunicacaoRastreador { get; set; }

        #endregion

        #region Bovinos

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarreta", Column = "VEI_TIPO_CARRETA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarreta), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarreta? TipoCarreta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMaterialGaiola", Column = "VEI_TIPO_MATERIAL_GAIOLA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMaterial), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMaterial? TipoMaterialGaiola { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMaterialPiso", Column = "VEI_TIPO_MATERIAL_PISO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMaterial), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMaterial? TipoMaterialPiso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSistemaElevacao", Column = "VEI_TIPO_SISTEMA_ELEVACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistemaElevacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistemaElevacao? TipoSistemaElevacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCurrais", Column = "VEI_QUANTIDADE_CURRAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCurrais { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario FuncionarioResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPlotagem", Column = "VEI_TIPO_PLOTAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem TipoPlotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ViradaHodometro", Column = "VEI_VIRADA_HODOMETRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ViradaHodometro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_POSSUI_LOCALIZADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiLocalizador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KilometragemVirada", Column = "VEI_KM_VIRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int KilometragemVirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarIntegracaoParaFilaCarregamento", Column = "VEI_NAO_VALIDAR_INTEGRACAO_FILA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarIntegracaoParaFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarConsultarAbastecimentoAngelLira", Column = "VEI_ATIVAR_CONSULTA_ABASTECIMENTO_ANGELLIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarConsultarAbastecimentoAngelLira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial FilialCarregamento { get; set; }

        /// <summary>
        /// Prop Exclusiva da Marfrig!!! #51075 Usado na lista de frota: ListaDiariaController
        /// Não substitui o Paletizado do Modelo Veicular (ModeloVeicular.VeiculoPaletizado)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Paletizado", Column = "VEI_PALETIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Paletizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PosicaoReboque", Column = "VEI_POSICAO_REBOQUE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PosicaoReboque), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PosicaoReboque? PosicaoReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataValidadeAdicionalCarroceria", Column = "VEI_DATA_VALIDADE_ADICIONAL_CARROCERIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataValidadeAdicionalCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "VEI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "VEI_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VEI_VEICULO_ALUGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoAlugado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CIOTEmitidoContratanteDiferenteEmbarcador", Column = "VEI_CIOT_EMITIDO_CONTRATANTE_DIFERENTE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CIOTEmitidoContratanteDiferenteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialCIOTTemporario", Column = "VEI_DATA_INICIAL_CIOT_TEMPORARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialCIOTTemporario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalCIOTTemporario", Column = "VEI_DATA_FINAL_CIOT_TEMPORARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalCIOTTemporario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeTanqueArla", Column = "VEI_CAPACIDADE_TANQUE_ARLA", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeTanqueArla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigencia", Column = "VEI_DATA_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_FILIAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaFilial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_VEICULO_COOPERADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaVeiculoCooperado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoCooperado", Column = "VEI_VEICULO_COOPERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoCooperado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_CONJUNTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEC_CODIGO_PAI", ForeignKey = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", ForeignKey = "VEI_CODIGO", Column = "VEC_CODIGO_FILHO", NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual IList<Dominio.Entidades.Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculosTracao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_CONJUNTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEC_CODIGO_FILHO", ForeignKey = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", ForeignKey = "VEI_CODIGO", Column = "VEC_CODIGO_PAI", NotFound = NHibernate.Mapping.Attributes.NotFoundMode.Ignore)]
        public virtual IList<Dominio.Entidades.Veiculo> VeiculosTracao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LicencasVeiculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_LICENCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LicencaVeiculo", Column = "VLI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> LicencasVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculoMotoristas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_MOTORISTAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoMotoristas", Column = "VEM_CODIGO")]
        public virtual IList<Dominio.Entidades.VeiculoMotoristas> VeiculoMotoristas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Motoristas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoMotorista", Column = "VMT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> Motoristas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoIntegracao", Column = "INT_CODIGO")]
        public virtual IList<Embarcador.Veiculos.VeiculoIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pneus", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_PNEU")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoPneu", Column = "VPN_CODIGO")]
        public virtual IList<VeiculoPneu> Pneus { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Estepes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_ESTEPE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoEstepe", Column = "VES_CODIGO")]
        public virtual IList<VeiculoEstepe> Estepes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposIntegracaoValePedagio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_TIPO_INTEGRACAO_VALE_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoIntegracao", Column = "TPI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> TiposIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculoLiberacaoGR", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VEICULO_LIBERACAO_GR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "VeiculoLiberacaoGR", Column = "VLG_CODIGO")]
        public virtual IList<VeiculoLiberacaoGR> VeiculoLiberacaoGR { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFPrimeiroMotorista", Formula = @"ISNULL((SELECT TOP 1 motorista1.FUN_CPF FROM T_VEICULO_MOTORISTA motoristaVeiculo INNER JOIN T_FUNCIONARIO motorista1 ON motoristaVeiculo.FUN_CODIGO = motorista1.FUN_CODIGO WHERE motoristaVeiculo.VEI_CODIGO = VEI_CODIGO), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string CPFPrimeiroMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomePrimeiroMotorista", Formula = @"ISNULL((SELECT TOP 1 motorista1.FUN_NOME FROM T_VEICULO_MOTORISTA motoristaVeiculo INNER JOIN T_FUNCIONARIO motorista1 ON motoristaVeiculo.FUN_CODIGO = motorista1.FUN_CODIGO WHERE motoristaVeiculo.VEI_CODIGO = VEI_CODIGO), '')", TypeType = typeof(string), Lazy = true)]
        public virtual string NomePrimeiroMotorista { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (SituacaoVeiculo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao:
                        return "Em Manutenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem:
                        return "Em Viagem";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel:
                        return "Disponível";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Indisponivel:
                        return "Indisponível";
                    default:
                        return "Disponível";
                }
            }
        }

        public virtual string DescricaoVeiculoVazio
        {
            get
            {
                switch (VeiculoVazio)
                {
                    case true:
                        return "Vazio";
                    case false:
                        return "Carregado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoMarca
        {
            get
            {
                return this.Marca != null ? this.Marca.Descricao : string.Empty;
            }
        }

        public virtual string DescricaoModelo
        {
            get
            {
                return this.Modelo != null ? this.Modelo.Descricao : string.Empty;
            }
        }

        public virtual string DescricaoComMarcaModelo
        {
            get
            {
                return Descricao + (Marca != null && Modelo != null ? " (" + Marca.Descricao + " " + Modelo.Descricao + ")" : string.Empty);
            }
        }

        public virtual string DescricaoComModeloVeicularCarga
        {
            get
            {
                return $"{Descricao}{(ModeloVeicularCarga != null ? $" ({ModeloVeicularCarga.Descricao})" : "")}";
            }
        }

        public virtual string DescricaoEstado
        {
            get
            {
                return this.Estado != null ? this.Estado.Sigla : string.Empty;
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case "P":
                        return Localization.Resources.Enumeradores.TipoPropriedadeVeiculo.Proprio;
                    case "T":
                        return Localization.Resources.Enumeradores.TipoPropriedadeVeiculo.Terceiros;
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoCombustivel
        {
            get
            {
                switch (TipoCombustivel)
                {
                    case "G":
                        return Localization.Resources.Enumeradores.TipoCombustivel.Gasolina;
                    case "D":
                        return Localization.Resources.Enumeradores.TipoCombustivel.DieselQuinhentos;
                    case "I":
                        return Localization.Resources.Enumeradores.TipoCombustivel.DieselDez;
                    case "E":
                        return Localization.Resources.Enumeradores.TipoCombustivel.Etanol;
                    case "O":
                        return Localization.Resources.Enumeradores.TipoCombustivel.Diesel;
                    case "S":
                        return Localization.Resources.Enumeradores.TipoCombustivel.GasNatural;
                    case "N":
                        return Localization.Resources.Enumeradores.TipoCombustivel.Outros;
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoVeiculo
        {
            get
            {
                switch (TipoVeiculo)
                {
                    case "0":
                        return Localization.Resources.Enumeradores.TipoVeiculo.Tracao;
                    case "1":
                        return Localization.Resources.Enumeradores.TipoVeiculo.Reboque;
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoRodado
        {
            get
            {
                switch (TipoRodado)
                {
                    case "00":
                        return Localization.Resources.Enumeradores.TipoRodadoVeiculo.NaoAplicado;
                    case "01":
                        return Localization.Resources.Enumeradores.TipoRodadoVeiculo.Truck;
                    case "02":
                        return Localization.Resources.Enumeradores.TipoRodadoVeiculo.Toco;
                    case "03":
                        return Localization.Resources.Enumeradores.TipoRodadoVeiculo.Cavalo;
                    case "04":
                        return Localization.Resources.Enumeradores.TipoRodadoVeiculo.Van;
                    case "05":
                        return Localization.Resources.Enumeradores.TipoRodadoVeiculo.Utilitario;
                    case "06":
                        return Localization.Resources.Enumeradores.TipoRodadoVeiculo.Outros;
                    default:
                        return "";
                }
            }
        }

        public virtual int TipoVeiculoOpentech
        {
            get
            {
                switch (TipoRodado)
                {
                    case "00": //Não Aplicado
                        return 2; //CARRETA
                    case "01": //Truck
                        return 3;
                    case "02": //Toco
                        return 4;
                    case "03": //Cavalo
                        return 1;
                    //case "04":
                    //    return "Van";
                    case "05": //Utilitário
                        return 25;
                    //case "06":
                    //    return "Outros";
                    default:
                        return 25; //UTILITARIO
                }
            }
        }

        public virtual string DescricaoTipoCarroceria
        {
            get
            {
                switch (TipoCarroceria)
                {
                    case "00":
                        return Localization.Resources.Veiculos.Veiculo.NaoAplicado;
                    case "01":
                        return Localization.Resources.Veiculos.Veiculo.Aberta;
                    case "02":
                        return Localization.Resources.Veiculos.Veiculo.FechadaBau;
                    case "03":
                        return Localization.Resources.Veiculos.Veiculo.Granel;
                    case "04":
                        return Localization.Resources.Veiculos.Veiculo.PortaContainer;
                    case "05":
                        return Localization.Resources.Veiculos.Veiculo.Sider;
                    default:
                        return "";
                }
            }
        }

        public virtual int TipoCarroceriaoOpentech
        {
            get
            {
                switch (TipoCarroceria)
                {
                    case "00": //Não Aplicado
                        return 39; //NAO TEM
                    case "01": //Aberta
                        return 20; //ABERTA
                    case "02": //Fechada/Baú
                        return 16; //FECHADA
                    //case "03": //Granel
                    //    return "";
                    case "04": //Porta Container
                        return 7; //S.REBOQUE/CONTAINER
                    //case "05": //Sider
                    //    return "";
                    default:
                        return 16; //FECHADA
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Placa;
            }
        }

        public virtual string Placa_Formatada
        {
            get
            {
                return string.Concat(this.Placa.Substring(0, 3), "-", this.Placa.Substring(3, 4));
            }
        }

        public virtual string PlacaConcatenada
        {
            get
            {
                string placa = this.Placa;
                if (!string.IsNullOrWhiteSpace(this.NumeroFrota))
                    placa += $" ({this.NumeroFrota})";

                return placa;
            }
        }

        public virtual bool IsTipoVeiculoReboque()
        {
            return (TipoVeiculo == "1");
        }

        public virtual bool IsTipoVeiculoTracao()
        {
            return (TipoVeiculo == "0");
        }

        #endregion
    }
}
