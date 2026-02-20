using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_SEM_PARAR", EntityName = "IntegracaoSemParar", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar", NameType = typeof(IntegracaoSemParar))]
    public class IntegracaoSemParar : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIS_USUARIO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIS_SENHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_FORNECEDOR_VP", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente FornecedorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CIS_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "CIS_CNPJ", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_DIAS_PRAZO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasPrazo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao1", Column = "CIS_OBSERVACAO_1", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao2", Column = "CIS_OBSERVACAO_2", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao3", Column = "CIS_OBSERVACAO_3", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao4", Column = "CIS_OBSERVACAO_4", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao4 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao5", Column = "CIS_OBSERVACAO_5", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao5 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao6", Column = "CIS_OBSERVACAO_6", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao6 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeRpt", Column = "CIS_RPT_VALE_PEDAGIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeRpt { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_DISTANCIA_MINIMA_QUADRANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int DistanciaMinimaQuadrante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_UTILIZAR_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool UtilizarModeoVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_BUSCAR_PRACAS_NA_GERACAO_DA_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool BuscarPracasNaGeracaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_COMPRAR_RETORNO_VAZIO_SEPARADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ComprarRetornoVazioSeparado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPontoSemParar", Column = "CIS_TIPO_PONTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPontoSemParar), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPontoSemParar TipoPontoSemParar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_NAO_COMPRAR_VALE_PEDAGIO_VEICULO_SEM_TAG", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoComprarValePedagioVeiculoSemTag { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_CONSULTAR_VALE_PEDAGIO_PARA_ROTA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ConsultarValorPedagioParaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_CONSULTAR_E_COMPRAR_VALE_PEDAGIO_PARA_ROTA_FRETE_EMBARCADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ConsultarEComprarPedagioFreteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_CONSULTAR_VEICULO_POSSUI_CADASTRO_SEM_PARAR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ConsultarSeVeiculoPossuiCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoConsultaRota", Column = "CIS_TIPO_CONSULTA_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota TipoConsultaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_NOTIFICAR_TRANSPORTADOR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_GERAR_REGISTRO_MESMO_SE_ROTA_NAO_POSSUIR_PRACA_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_COMPRAR_SOMENTE_NO_MES_VIGENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComprarSomenteNoMesVigente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_CONSULTAR_EXTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_QUANTIDADE_DIAS_CONSULTAR_EXTRATO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasConsultarExtrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracaoRest", Column = "CIS_URL_INTEGRACAO_REST", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UrlIntegracaoRest { get; set; }

        [Obsolete("Configuração temporária, não será mais usada.", true)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_ENVIAR_DADOS_REGULATORIOS_ANTT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDadosRegulatorioANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoBuscarPracasNaGeracaoDaCarga", Column = "CIS_TIPO_BUSCAR_PRACAS_NA_GERACAO_DA_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoBuscarPracasNaGeracaoDaCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoBuscarPracasNaGeracaoDaCarga? TipoBuscarPracasNaGeracaoDaCarga { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Integração Sem Parar"; }
        }
    }
}
