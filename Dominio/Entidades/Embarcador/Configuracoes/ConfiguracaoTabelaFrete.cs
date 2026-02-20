namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_TABELA_FRETE", EntityName = "ConfiguracaoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete", NameType = typeof(ConfiguracaoTabelaFrete))]
    public class ConfiguracaoTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_PERMITE_INFORMAR_LEAD_TIME_TABELA_FRETE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarLeadTimeTabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_EXIGIR_ALIQUOTA_NO_MUNICIPIO_DE_PRESTACAO_PARA_CALCULO_DE_FRETE_EM_FRETES_MUNICIPAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_UTILIZAR_INTEGRACAO_ALTERACAO_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarIntegracaoAlteracaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_UTILIZAR_VIGENCIA_CONFIGURACAO_DESCARGA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarVigenciaConfiguracaoDescargaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_MOSTRAR_REGISTROS_SOMENTE_COM_VALOR_NA_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MostrarRegistroSomenteComValoresNaAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_OBRIGATORIO_INFORMAR_TRANSPORTADOR_AJUSTE_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarTransportadorAjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_OBRIGATORIO_INFORMAR_CONTRATO_TRANSPORTE_FRETE_AJUSTE_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_NAO_BUSCAR_AUTOMATICAMENTE_VIGENCIA_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoBuscarAutomaticamenteVigenciaTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_IMPORTAR_TABELA_FRETE_CLIENTE_INFORMANDO_ORIGENS_DESTINOS_EM_DIFERENTES_COLUNAS_NO_MESMO_ARQUIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_GRAVAR_AUDITORIA_IMPORTAR_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GravarAuditoriaImportarTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_ARREDONDAR_VALOR_DO_COMPONENTE_DE_PEDAGIO_PARA_PROXIMO_INTEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArredondarValorDoComponenteDePedagioParaProximoInteiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_SALVAR_PLACAS_VEICULOS_AO_SALVAR_MODELOS_VEICULOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarPlacasVeiculosAoSalvarModelosVeiculos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_NAO_PERMITE_EDICOES_EM_VALORES_NA_CONSULTA_DE_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTF_NAO_UTILIZAR_REGIAO_PARA_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarRegiaoParaCalcularFrete { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para Tabela de Frete"; }
        }
    }
}
