using Dominio.Entidades.Embarcador.Compras;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CLIENTE", EntityName = "Cliente", Name = "Dominio.Entidades.Cliente", NameType = typeof(Cliente))]
    public class Cliente : EntidadeBase, IEquatable<Cliente>
    {
        #region Construtores

        public Cliente()
        {
        }

        #endregion

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "CPF_CNPJ", Type = "System.Double", Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "assigned")]
        public virtual double CPF_CNPJ { get; set; }

        public virtual long Codigo
        {
            get { return (long)this.CPF_CNPJ; }
        }

        /// <summary>
        /// Da aba Configuração Emissão, que é um componente com o cadastro de Tipo de Operação e Grupo de Pessoas
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoPessoaEmissao", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaEmissao ConfiguracaoEmissao { get; set; }

        /// <summary>
        /// Da aba Configuração Fatura, que é um componente com o cadastro de Grupo de Pessoas
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoPessoaFatura", Column = "CPF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.ConfiguracaoPessoa.ConfiguracaoPessoaFatura ConfiguracaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CategoriaPessoa", Column = "CTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pessoas.CategoriaPessoa Categoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais Pais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atividade", Column = "ATI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Atividade Atividade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_NASCIMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeNascimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        /// <summary>
        /// F - Física 
        /// J - Jurídica
        /// E - Exterior
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CLI_FISJUR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Tipo { get; set; }

        /// <summary>
        /// IE: se vazio utilizar ISENTO.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IE_RG", Column = "CLI_IERG", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string IE_RG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrgaoEmissorRG", Column = "CLI_ORGAO_EMISSOR_RG", TypeType = typeof(ObjetosDeValor.Enumerador.OrgaoEmissorRG), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.OrgaoEmissorRG? OrgaoEmissorRG { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_RG", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoRG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataNascimento", Column = "CLI_DATA_NASCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataNascimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sexo", Column = "CLI_SEXO", TypeType = typeof(ObjetosDeValor.Enumerador.Sexo), NotNull = false)]
        public virtual ObjetosDeValor.Enumerador.Sexo? Sexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CLI_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LogAtividade", Column = "CLI_LOG_ATIVIDADE", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string LogAtividade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFantasia", Column = "CLI_NOMEFANTASIA", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string NomeFantasia { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeVisoesBI", Column = "CLI_NOME_VISOES_BI", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string NomeVisoesBI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Endereco", Column = "CLI_ENDERECO", TypeType = typeof(string), Length = 120, NotNull = false)]
        public virtual string Endereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CLI_NUMERO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Complemento", Column = "CLI_COMPLEMENTO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Complemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bairro", Column = "CLI_BAIRRO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string Bairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CEP", Column = "CLI_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone1", Column = "CLI_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone2", Column = "CLI_FAX", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CLI_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailStatus", Column = "CLI_EMAIL_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string EmailStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContato", Column = "CLI_EMAILCONTATO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContatoStatus", Column = "CLI_EMAILCONTATO_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string EmailContatoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContador", Column = "CLI_EMAILCONTADOR", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailContador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailContadorStatus", Column = "CLI_EMAILCONTADOR_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string EmailContadorStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "CLI_DATACAD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoSuframa", Column = "CLI_INSCRICAO_SUFRAMA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoSuframa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoMunicipal", Column = "CLI_INSCRICAO_MUNICIPAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cidade", Column = "CLI_CIDADE", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RG_Passaporte", Column = "CLI_RG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string RG_Passaporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CLI_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "CLI_LATIDUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "CLI_LONGITUDE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CoordenadaConferida", Column = "CLI_COORDENADA_CONFERIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CoordenadaConferida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RaioEmMetros", Column = "CLI_RAIO_METROS", TypeType = typeof(int), NotNull = false)]
        public virtual int? RaioEmMetros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoClienteIntegracaoLBC", Column = "CLI_TIPO_INTEGRACAO_LBC", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoClienteIntegracaoLBC), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoClienteIntegracaoLBC? TipoClienteIntegracaoLBC { get; set; }

        /// <summary>
        /// Propriedade para atualizar automaticamente o ponto de apoio em cada roteirização para o ponto mais próximo.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "AtualizarPontoApoioMaisProximoAutomaticamente", Column = "CLI_PONTO_DE_APOIO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarPontoApoioMaisProximoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Locais", Column = "LLC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Locais PontoDeApoio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Area", Column = "CLI_AREA", Type = "StringClob", NotNull = false)]
        public virtual string Area { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArea", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArea), Column = "CLI_TIPO_AREA", NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoArea? TipoArea { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LATIDUDE_TRANSBORDO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LatitudeTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LONGITUDE_TRANSBORDO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LongitudeTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLocalizacao", Column = "CLI_TIPO_LOCALIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao TipoLocalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLogradouro", Column = "CLI_TIPO_LOGRADOURO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro? TipoLogradouro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeDocumentos", Column = "CLI_TIPO_EMISSAO_CTE_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoEmissaoCTeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeParticipantes", Column = "CLI_TIPO_EMISSA_CTE_PARTICIPANTES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes TipoEmissaoCTeParticipantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoIntramunicipal", Column = "CLI_TIPO_EMISSAO_INTRAMUNICIPAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTeEmitidoNoEmbarcador", Column = "CLI_CTE_EMITIDO_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTeEmitidoNoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO_EXCLUSIVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormulaExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeDocumentosExclusivo", Column = "CLI_TIPO_EMISSAO_CTE_CLIENTE_EXCLUSIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos? TipoEmissaoCTeDocumentosExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_EMPRESA_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoImportacaoNotaFiscal", Column = "AIN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal ArquivoImportacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCadastroContaBancaria", Column = "CLI_UTILIZAR_CADASTRO_CONTA_BANCARIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCadastroContaBancaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_PORTADOR_CONTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClientePortadorConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "CLI_BANCO_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "CLI_BANCO_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "CLI_BANCO_NUMERO_CONTA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "CLI_BANCO_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmail", Column = "CLI_EMAIL_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmail), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmail TipoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEndereco", Column = "CLI_ENDERECO_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco TipoEndereco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTDE", Column = "CLI_VALOR_TDE", TypeType = typeof(decimal), NotNull = true, Scale = 2, Precision = 18)]
        public virtual decimal ValorTDE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnderecoDigitado", Column = "CLI_ENDERECO_DIGITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnderecoDigitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoST", Column = "CLI_INSCRICAO_ST", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string InscricaoST { get; set; }

        [Obsolete("Migrado para lista DiasSemanaFatura")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaSemana", Column = "CLI_DIA_SEMANA_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? DiaSemana { get; set; }

        [Obsolete("Migrado para lista DiasMesFatura")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaMesFatura", Column = "CLI_DIA_MES_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiaMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_FORMA_GERACAO_TITULO_FATURA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaGeracaoTituloFatura), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaGeracaoTituloFatura? FormaGeracaoTituloFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteFinalDeSemana", Column = "CLI_PERMITE_FINAL_SEMANA_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermiteFinalDeSemana { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeCanhotoFisico", Column = "CLI_EXIGE_CANHOTO_FISICO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExigeCanhotoFisico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ARMAZENA_CANHOTO_FISICO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ArmazenaCanhotoFisicoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteOcorrenciasFinalizadoras", Column = "CLI_SOMENTE_OCORRENCIA_FINALIZADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SomenteOcorrenciasFinalizadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturarSomenteOcorrenciasFinalizadoras", Column = "CLI_FATURAR_SOMENTE_OCORRENCIA_FINALIZADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? FaturarSomenteOcorrenciasFinalizadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarFaturaAteReceberCanhotos", Column = "CLI_NAO_GERAR_FATURA_ATE_RECEBER_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoGerarFaturaAteReceberCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorIE", Column = "CLI_INDICADOR_IE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE? IndicadorIE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteTomadorFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFatura", Column = "CLI_OBSERVACAO_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CLI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPrazoFaturamento", Column = "CLI_TIPO_PRAZO_FATURAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento? TipoPrazoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasDePrazoFatura", Column = "CLI_DIA_DE_PRAZO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasDePrazoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroPedido", Column = "CLI_EXIGIR_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUsarConfiguracaoEmissaoGrupo", Column = "CLI_NAO_USAR_CONFIGURACAO_EMISSAO_GRUPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUsarConfiguracaoEmissaoGrupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUsarConfiguracaoFaturaGrupo", Column = "CLI_NAO_USAR_CONFIGURACAO_FATURA_GRUPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUsarConfiguracaoFaturaGrupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CLI_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgruparMovimentoFinanceiroPorPedido", Column = "CLI_AGRUPAR_MOVIMENTO_FINANCEIRO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparMovimentoFinanceiroPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_VALIDAR_NOTA_FISCAL_EXISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarNotaFiscalExistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_VALIDAR_NOTAS_FISCAIS_COM_DIFERENTES_PORTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarNotasFiscaisComDiferentesPortos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DESCRICAO_COMPONENTE_FRETE_EMBARCADOR", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DescricaoComponenteFreteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarTituloPorDocumentoFiscal", Column = "CLI_GERAR_TITULO_POR_DOCUMENTO_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloPorDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_TITULO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_FATURA_AUTOMATICA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturaAutomaticaCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_FATURAMENTO_A_VISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturamentoAVista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VALE_PEDAGIO_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValePedagioObrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarValoresCTeImportadoQuandoTomador", Column = "CLI_NAO_VALIDAR_VALORES_CTE_IMPORTADO_QUANDO_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarValoresCTeImportadoQuandoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.TipoPagamentoRecebimento FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContaFornecedorEBS", Column = "CLI_CONTA_FORNECEDOR_EBS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ContaFornecedorEBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarDuplicataNotaEntrada", Column = "CLI_GERAR_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntervaloDiasDuplicataNotaEntrada", Column = "CLI_INTERVALO_DIAS_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IntervaloDiasDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaPadraoDuplicataNotaEntrada", Column = "CLI_DIA_PADRAO_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaPadraoDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParcelasDuplicataNotaEntrada", Column = "CLI_PARCELAS_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int ParcelasDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IgnorarDuplicataRecebidaXMLNotaEntrada", Column = "CLI_IGNORAR_DUPLICATA_RECEBIDA_XML_NOTA_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarDuplicataRecebidaXMLNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_POSSUI_RESTRICAO_TRAFEGO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiRestricaoTrafego { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PONTO_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PontoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DIGITALIZA_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DigitalizaCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_EMITIR_CTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirCTeFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_EMITIR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PROVISIONAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisionarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DISPONIBILIZAR_DOCUMENTOS_PARA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_QUITAR_DOCUMENTO_AUTOMATICAMENTE_AO_GERAR_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool QuitarDocumentoAutomaticamenteAoGerarLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DISPONIBILIZAR_DOCUMENTOS_PARA_LOTE_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaLoteEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DISPONIBILIZAR_DOCUMENTOS_PARA_LOTE_ESCRITURACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaLoteEscrituracaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ESCRITURAR_SOMENTE_DOCUMENTOS_EMITIDOS_PARA_NFES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscriturarSomenteDocumentosEmitidosParaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataViradaProvisao", Column = "CLI_DATA_VIRADA_PROVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataViradaProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCompanhia", Column = "CLI_CODIGO_COMPANHIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCompanhia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoSap", Column = "CLI_CODIGO_SAP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoSap { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Referencia", Column = "CLI_REFERENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Referencia { get; set; }

        /// <summary>
        /// Utilizar outro modelo de documento quanto for emissão municipal (NFS-e ou NFS Manual)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_UTILIZAR_OUTRO_MODELO_DOCUMENTO_EMISSAO_MUNICIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarOutroModeloDocumentoEmissaoMunicipal { get; set; }

        /// <summary>
        /// Modelo de documento fiscal para emissões municipais
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_EMISSAO_MUNICIPAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalEmissaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_REGIME_TRIBUTARIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario? RegimeTributario { get; set; }

        /// <summary>
        /// Caso o valor do frete enviado pelo embarcador seja diferente do valor calculado pela tabela de frete, é necessário autorização para emissão
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_BLOQUEAR_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearDiferencaValorFreteEmbarcador { get; set; }

        /// <summary>
        /// Só bloqueia se a diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete seja maior que um percentual
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PERCENTUAL_BLOQUEAR_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualBloquearDiferencaValorFreteEmbarcador { get; set; }

        /// <summary>
        /// Caso exija a emissão automática de um complemento da diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_EMITIR_COMPLEMENTO_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirComplementoDiferencaFreteEmbarcador { get; set; }

        /// <summary>
        /// Tipo de ocorrência para a emissão automática de um complemento da diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_COMPLEMENTO_DIFERENCA_FRETE_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaComplementoDiferencaFreteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_OCORRENCIA_SEM_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaSemTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_TIPO_OCORRENCIA_SEM_TABELA_FRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaSemTabelaFrete { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_MDFE_TRANSBORDO_SEM_CONSIDERAR_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMDFeTransbordoSemConsiderarOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_IMPORTAR_REDESPACHO_INTERMEDIARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EMITENTE_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente EmitenteImportacaoRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ExpedidorImportacaoRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente RecebedorImportacaoRedespachoIntermediario { get; set; }

        /// <summary>
        /// Descrição do item utilizado para obter o peso do CT-e para subcontratação.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DESCRICAO_ITEM_PESO_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoItemPesoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DESCRICAO_CARAC_TRANSP_CTE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CaracteristicaTransporteCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_ATUALIZAR_DADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAtualizarDados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_FUNCIONARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Funcionario { get; set; }

        /// <summary>
        /// O acesso de clientes no portal foi centralizado considerando
        /// o fluxo que já existia de acesso fornecedor foi utilizado como acesso geral.
        /// Considere essas informação como informações do acesso ao portal
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ATIVAR_ACESSO_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarAcessoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_HAB_FORNECEDOR_LANCAMENTO_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFornecedorParaLancamentoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_COMPARTILHAR_ACESSO_ENTRE_GRUPO_PESSOAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CompartilharAcessoEntreGrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VISUALIZAR_APENAS_PARA_PEDIDOS_DESTE_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarApenasParaPedidoDesteTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DESABILITAR_CANCELAMENTO_AGENDAMENTO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesabilitarCancelamentoAgendamentoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PIS_PASEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string PISPASEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_REGEX_VALIDACAO_NUMERO_PEDIDO_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegexValidacaoNumeroPedidoEmbarcador { get; set; }

        /// <summary>
        /// Deve ser utilizada como informação na etapa de frete da carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_OBSERVACAO_EMISSAO_CARGA", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TIPO_ENVIO_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe? TipoEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VALOR_MAXIMO_EMISSAO_PENDENTE_PAGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMaximoEmissaoPendentePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VALOR_LIMITE_FATURAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorLimiteFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasEmAbertoAposVencimento", Column = "CLI_DIA_EM_ABERTO_APOS_VENCIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasEmAbertoAposVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NOME_SOCIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NomeSocio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CPF_SOCIO", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPFSocio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PROFISSAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Profissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TITULO_ELEITORAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TituloEleitoral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ZONA_ELEITORAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ZonaEleitoral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_SECAO_ELEITORAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SecaoEleitoral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NUMERO_CEI", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroCEI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarPedidoColeta", Column = "CLI_GERAR_PEDIDO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarPedidoBloqueado", Column = "CLI_GERAR_PEDIDO_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoBloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InstituicaoGovernamental", Column = "CLI_INSTITUICAO_GOVERNAMENTAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InstituicaoGovernamental { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_RECEBEDOR_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente RecebedorColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CLIENTE_PAI", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClientePai { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.TipoOperacao TipoOperacaoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_CTE_EMITIDO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrenciaCTeEmitidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LayoutsEDI", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_LAYOUT_EDI")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteLayoutEDI", Column = "CLY_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ClienteLayoutEDI> LayoutsEDI { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ClienteDescargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_DESCARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteDescarga", Column = "CLD_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ClienteDescarga> ClienteDescargas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Enderecos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_OUTRO_ENDERECO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteOutroEndereco", Column = "COE_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ClienteOutroEndereco> Enderecos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_DOCUMENTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteDocumentacao", Column = "CDO_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ClienteDocumentacao> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Emails", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_OUTRO_EMAIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteOutroEmail", Column = "COE_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ClienteOutroEmail> Emails { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ApolicesSeguro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_APOLICE_SEGURO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ApoliceSeguro", Column = "APS_CODIGO")]
        public virtual ICollection<Embarcador.Seguros.ApoliceSeguro> ApolicesSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Modalidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_MODALIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPF_CNPJ")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModalidadePessoas", Column = "MOD_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ModalidadePessoas> Modalidades { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ClienteConfiguracoesComponentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_CONFIGURACAO_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteConfiguracaoComponentes", Column = "CLC_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ClienteConfiguracaoComponentes> ClienteConfiguracoesComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Contatos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_CONTATO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaContato", Column = "PCO_CODIGO")]
        public virtual IList<Embarcador.Contatos.PessoaContato> Contatos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Licencas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_LICENCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaLicenca", Column = "PLI_CODIGO")]
        public virtual IList<Embarcador.Pessoas.PessoaLicenca> Licencas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Vendedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_FUNCIONARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaFuncionario", Column = "PFU_CODIGO")]
        public virtual IList<Embarcador.Pessoas.PessoaFuncionario> Vendedores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Rotas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROTA_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF_DISTRIBUIDOR")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RotaFrete", Column = "ROF_CODIGO")]
        public virtual IList<RotaFrete> Rotas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Componentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_COMPONENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteComponente", Column = "CLC_CODIGO")]
        public virtual IList<Embarcador.Pessoas.ClienteComponente> Componentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaIntegracao", Column = "INT_CODIGO")]
        public virtual IList<Embarcador.Pessoas.PessoaIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RestricoesFilaCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_RESTRICAO_FILA_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteRestricaoFilaCarregamento", Column = "CRF_CODIGO")]
        public virtual ICollection<ClienteRestricaoFilaCarregamento> RestricoesFilaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeoLocalizacaoStatus", Column = "CLI_GEOLOCALIZACAO_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus GeoLocalizacaoStatus { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeoLocalizacaoTipo", Column = "CLI_GEOLOCALIZACAO_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoTipo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoTipo GeoLocalizacaoTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeoLocalizacaoRaioLocalidade", Column = "CLI_GEOLOCALIZACAO_RAIO_LOCALIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoRaioLocalidade), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoRaioLocalidade GeoLocalizacaoRaioLocalidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCobrancaMultimodal", Column = "CLI_TIPO_COBRANCA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalPropostaMultimodal", Column = "CLI_MODAL_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultimodal", Column = "CLI_TIPO_SERVICO_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal TipoServicoMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaMultimodal", Column = "CLI_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDosDestinatario", Column = "CLI_BLOQUEAR_EMISSAO_DESTINATARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDeEntidadeSemCadastro", Column = "CLI_BLOQUEAR_EMISSAO_ENTIDADES_SEM_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDeEntidadeSemCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "CLI_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumentoFornecedor", Column = "CLI_CODIGO_DOCUMENTO_FORNECEDOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumentoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesBloquearEmissaoDosDestinatario", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_CLIENTE_BLOQUEAR_EMISSAO_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO")]
        public virtual ICollection<Cliente> ClientesBloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CODIGO_PORTUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoPortuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmailFatura", Column = "CLI_ASSUNTO_EMAIL_FATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string AssuntoEmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmailFatura", Column = "CLI_CORPO_EMAIL_FATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CorpoEmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_BOLETO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarBoletoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ENVIAR_ARQUIVOS_DESCOMPACTADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarArquivosDescompactados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_ENVIAR_EMAIL_FATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEmailFaturaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvioFatura", Column = "CLI_TIPO_ENVIO_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura? TipoEnvioFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoFatura", Column = "CLI_TIPO_AGRUPAMENTO_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura? TipoAgrupamentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroControleCliente", Column = "CLI_EXIGIR_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroControleCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReplicarNumeroControleCliente", Column = "CLI_REPLICAR_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarNumeroControleCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroNumeroReferenciaCliente", Column = "CLI_EXIGIR_NUMERO_REFERENCIA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroNumeroReferenciaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VerificarUnidadeNegocioPorDestinatario", Column = "CLI_VERIFICAR_UNIDADE_NEGOCIO_POR_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VerificarUnidadeNegocioPorDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReplicarNumeroReferenciaTodasNotasCarga", Column = "CLI_REPLICAR_NUMERO_REFERENCIA_TODAS_NOTAS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarNumeroReferenciaTodasNotasCarga { get; set; }

        /*
         * Se ativada, no app será necessário digitalizar a folha inteira da nota, em vez de apenas o canhoto
         */
        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitalizacaoCanhotoInteiro", Column = "CLI_DIGITALIZAR_CANHOTO_INTEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DigitalizacaoCanhotoInteiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCUITRUT", Column = "CLI_NUMERO_CUIT_RUIT", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroCUITRUT { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DiasSemanaFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_DIA_SEMANA_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CLI_DIA_SEMANA_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> DiasSemanaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DiasMesFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_DIA_MES_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CLI_DIA_MES_FATURA", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> DiasMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlvoEstrategico", Column = "CLI_ALVO_ESTRATEGICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlvoEstrategico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Celular", Column = "CLI_CELULAR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Celular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_OBSERVACAO_CTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_OBSERVACAO_CTE_TERCEIRO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_CIOT_PARA_TODAS_AS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOTParaTodasAsCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NOMENCLATURA_ARQUIVOS_DOWNLOAD_CTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeNomenclaturaArquivosDownloadCTe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Subareas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SUBAREA_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SubareaCliente", Column = "SAC_CODIGO")]
        public virtual IList<Embarcador.Logistica.SubareaCliente> Subareas { get; set; }

        #region Envio E-mail em lote

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_EMAIL_ENVIO_DOCUMENTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ASSUNTO_EMAIL_DOCUMENTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AssuntoDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CORPO_EMAIL_DOCUMENTACAO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string CorpoEmailDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoEnvioDocumentacao", Column = "CLI_TIPO_AGRUPAMENTO_ENVIO_DOCUMENTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao? TipoAgrupamentoEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEnvioDocumentacao", Column = "CLI_FORMA_ENVIO_DOCUMENTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao? FormaEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_EMAIL_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailEnvioDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ASSUNTO_EMAIL_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AssuntoDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CORPO_EMAIL_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string CorpoEmailDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoEnvioDocumentacaoPorta", Column = "CLI_TIPO_AGRUPAMENTO_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao? TipoAgrupamentoEnvioDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEnvioDocumentacaoPorta", Column = "CLI_FORMA_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao? FormaEnvioDocumentacaoPorta { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_OBSERVACAO_INTERNA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailFatura", Column = "CLI_EMAIL_FATURA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_VEICULO_OBSERVACAO_NOTA_PARA_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? LerVeiculoObservacaoNotaParaAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PROCESSAR_ABASTECIMENTO_AUTO_RECEBER_XML_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_PLACA_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerPlacaObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_PLACA_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerPlacaObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_CHASSI_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerChassiObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_CHASSI_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerChassiObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_KM_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerKMObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_KM_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerKMObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_HORIMETRO_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerHorimetroObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_LER_HORIMETRO_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerHorimetroObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_FATURAMENTO_MULTIPLA_PARCELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturamentoMultiplaParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_QUANTIDADE_PARCELAS_FATURAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string QuantidadeParcelasFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_SEMPRE_CONSIDERAR_VALOR_ORCADO_FECHAMENTO_ORDEM_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreConsiderarValorOrcadoFechamentoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaAnexo", Column = "PEA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Vencimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_FORNECEDOR_VENCIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ClienteFornecedorVencimento", Column = "CFV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento> Vencimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VALOR_FRETE_LIQUIDO_DEVE_SER_VALOR_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFreteLiquidoDeveSerValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VALOR_FRETE_LIQUIDO_DEVE_SER_VALOR_A_RECEBER_SEM_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFreteLiquidoDeveSerValorAReceberSemICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_SENHA_LIBERACAO_MOBILE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SenhaLiberacaoMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_SENHA_CONFIRMACAO_COLETA_ENTREGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaConfirmacaoColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_GERAR_OCORRENCIA_COMPLEMENTO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaComplementoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_COMPLEMENTO_SUBCONTRATACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaComplementoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_FRONTEIRA_ALFANDEGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FronteiraAlfandega { get; set; }

        // Se for fronteira, tem também essa campo para integração do MIC/DTA
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAduanaDestino", Column = "CLI_CODIGO_ADUANEIRO_DESTINO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoAduanaDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TEMPO_MEDIO_PERMANCENCIA_FRONTEIRA", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoMedioPermanenciaFronteira { get; set; } // Em minutos

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CODIGO_ADUANEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string CodigoAduaneiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CODIGO_URF_ADUANEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string CodigoURFAduaneiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CODIGO_RA_ADUANEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string CodigoRAAduaneiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoTipoPagamento", Column = "PTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento PedidoTipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_PERMITIR_VINCULAR_CTE_COMPLEMENTAR_EM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirVincularCTeComplementarEmCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_AGUARDANDO_CONFERENCIA_INFORMACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoConferenciaInformacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "CLI_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Financeiro.BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ENVIAR_BOLETO_POR_EMAIL_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarBoletoPorEmailAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ENVIAR_DOCUMENTACAO_FATURAMENTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentacaoFaturamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoFinanceira", Column = "CLI_SITUACAO_FINANCEIRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira? SituacaoFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracaoSituacaoFinanceira", Column = "CLI_DATA_ALTERACAO_SITUACAO_FINANCEIRA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracaoSituacaoFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TitulosBloqueioFinanceiro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_TITULO_BLOQUEIO_FINANCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Titulo", Column = "TIT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.Titulo> TitulosBloqueioFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EstadoCivil", Column = "CLI_ESTADOCIVIL", TypeType = typeof(EstadoCivil), NotNull = false)]
        public virtual EstadoCivil? EstadoCivil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_COTACAO_ESPECIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal CotacaoEspecial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_MOTIVO_BLOQUEIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DATA_BLOQUEIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TIPO_FORNECEDOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CODIGO_CATEGORIA_TRABALHADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoCategoriaTrabalhador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_FUNCAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Funcao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PAGAMENTO_EM_BANCO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PagamentoEmBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_FORMA_PAGAMENTO_ESOCIAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FormaPagamentoeSocial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO_DOC", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco BancoDOC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TIPO_AUTONOMO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoAutonomo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CODIGO_RECEITA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoReceita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TIPO_PAGAMENTO_BANCARIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TipoPagamentoBancario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_DESCONTA_IRRF", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NaoDescontaIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTituloFornecedor", Column = "CLI_FORMA_TITULO_FORNECEDOR", TypeType = typeof(FormaTitulo), NotNull = false)]
        public virtual FormaTitulo? FormaTituloFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjIpef", Column = "CLI_CNPJ_IPEF", TypeType = typeof(string), Length = 18, NotNull = false)]
        public virtual string CnpjIpef { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChavePix", Column = "CLI_TIPO_CHAVE_PIX", TypeType = typeof(TipoChavePix), NotNull = false)]
        public virtual TipoChavePix? TipoChavePix { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CHAVE_PIX", TypeType = typeof(string), Length = 36, NotNull = false)]
        public virtual string ChavePix { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeradoViaPortal", Column = "CLI_GERADO_VIA_PORTAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoViaPortal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeQueEntregasSejamAgendadas", Column = "CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeQueEntregasSejamAgendadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_DISPONIBILIZAR_DOCUMENTOS_PARA_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaNFsManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ENVIAR_AUTMATICAMENTE_DOCUMENTACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarAutomaticamenteDocumentacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AreaRedex", Column = "CLI_AREA_REDEX", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AreaRedex { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Armador", Column = "CLI_ARMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Armador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_PERMITE_AGENDAR_COM_VIAGEM_INICIADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAgendarComViagemIniciada { get; set; }

        //PROPRIEDADES REMOVIDAS PARA CLASSE PESSOAARMADOR
        //[NHibernate.Mapping.Attributes.Property(0, Name = "DiasFreetime", Column = "CLI_DIAS_FREETIME", TypeType = typeof(int), NotNull = false)]
        //public virtual int? DiasFreetime { get; set; }
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ValorDariaAposFreetime", Column = "CLI_VALOR_DIARIA_APOS_FREETIME", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        //public virtual decimal? ValorDariaAposFreetime { get; set; }

        //PROPRIEDADES REMOVIDAS PARA CLASSE OutrosCodigosIntegracao
        //[NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoVtex", Column = "CLI_CODIGO_INTEGRACAO_VTEX", TypeType = typeof(string), Length = 50, NotNull = false)]
        //public virtual string CodigoIntegracaoVtex { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_HABILITAR_PERIODO_VENCIMENTO_ESPECIFICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarPeriodoVencimentoEspecifico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_UTILIZAR_CONFIGURACOES_DE_COMPROVANTES_DO_GRUPO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoCarregamentoTicks", Column = "CLI_TEMPO_CARREGAMENTO_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoCarregamentoTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarregamentoTicks", Column = "CLI_TEMPO_DESCARREGAMENTO_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoDescarregamentoTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaAtualizacao", Column = "CLI_DATA_ULTIMA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaAtualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "CLI_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarSolicitacaoSuprimentoDeGas", Column = "CLI_HABILITAR_SOLICITACAO_GAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarSolicitacaoSuprimentoDeGas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeEtiquetagem", Column = "CLI_EXIGE_ETIQUETAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeEtiquetagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VALOR_MINIMO_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMinimoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VALOR_MINIMO_ENTREGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMinimoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_ENVIAR_DOCUMENTACAO_CTE_AVERBACAO_SEGUNDA_INSTANCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentacaoCTeAverbacaoSegundaInstancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoDadosBancarios", Column = "CLI_CODIGO_INTEGRACAO_DADOS_BANCARIOS", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegracaoDadosBancarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoDuplicataNotaEntrada", Column = "CLI_CODIGO_INTEGRACAO_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracaoDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO_REDESPACHO_AREA_REDEX", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.TipoOperacao TipoOperacaoPadraoRedespachoAreaRedex { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarMDFeEmitidoPeloEmbarcador", Column = "CLI_OBRIGATORIO_INFORMAR_MDFE_EMITIDO_PELO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarMDFeEmitidoPeloEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "OutrosCodigosIntegracao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_OUTROS_CODIGOS_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CLI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual ICollection<string> OutrosCodigosIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EhPontoDeApoio", Column = "CLI_EH_PONTO_DE_APOIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EhPontoDeApoio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarValorMinimoMercadoriaEntregaMontagemCarregamento", Column = "CLI_VALIDAR_VALOR_MINIMO_MERCADORIA_ENTREGA_MONTAGEM_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarValorMinimoMercadoriaEntregaMontagemCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoComprarValePedagio", Column = "CLI_NAO_COMPRAR_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoComprarValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "CLI_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_ARMAZEM_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ArmazemResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CondicaoPagamento", Column = "COP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CondicaoPagamento CondicaoPagamentoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO_VALE_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegradoraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_NAO_EXIGIR_DIGITALIZACAO_DO_CANHOTO_PARA_ESTE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoExigirDigitalizacaoDoCanhotoParaEsteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_VISUALIZAR_APENAS_ALGUNS_DETERMINADOS_GRUPOS_DE_PESSOAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "GruposPessoas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_GRUPO_PESSOAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoas", Column = "GRP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> GruposPessoas { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposComprovante", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_TIPO_COMPROVANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoComprovante", Column = "CTC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> TiposComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FilialCliente", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF_FILIAL")]
        public virtual ICollection<Dominio.Entidades.Cliente> FilialCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAplicarChecklistMultiMobile", Column = "CLI_NAO_APLICAR_CHECKLIST_MULTI_MOBILE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAplicarChecklistMultiMobile { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAlternativo", Column = "CLI_CODIGO_ALTERNATIVO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoAlternativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExcecaoCheckinFilaH", Column = "CLI_EXCECAO_CHECKIN_FILA_H", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExcecaoCheckinFilaH { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigarInformarDataEntregaClienteAoBaixarCanhotos", Column = "CLI_OBRIGAR_INFORMAR_DATA_ENTREGA_CLIENTE_AO_BAIXAR_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarDataEntregaClienteAoBaixarCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirComprovantesLiberacaoPagamentoContratoFrete", Column = "CLI_EXIGIR_COMPROVANTE_LIBERACAO_PAGAMENTO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirComprovantesLiberacaoPagamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FazParteGrupoEconomico", Column = "CLI_FAZ_PARTE_GRUPO_ECONOMICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FazParteGrupoEconomico { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PessoaDataFixaVencimento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_DATA_FIXA_VENCIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "PessoaDataFixaVencimento")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento> PessoaDataFixaVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_AVISO_VENCIMETO_HABILITAR_CONFIGURACAO_PERSONALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoHabilitarConfiguracaoPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvisoVencimetoQunatidadeDias", Column = "CLI_AVISO_VENCIMETO_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int AvisoVencimetoQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_AVISO_VENCIMETO_ENVIAR_DIARIAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoEnviarDiariamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaHabilitarConfiguracaoPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaQunatidadeDias", Column = "CLI_COBRANCA_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int CobrancaQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_AVISO_VENCIMETO_NAO_ENVIAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoNaoEnviarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_COBRANCA_NAO_ENVIAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaNaoEnviarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPallet", Column = "CLI_REGRA_PALLET", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraPallet), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraPallet RegraPallet { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ContasBancarias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE_CONTAS_BANCARIAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLI_CGCCPF")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContaBancaria", Column = "COB_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria> ContasBancarias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_RKST", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string RKST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_MDGCODE", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string MDGCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_CMDID", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CMDID { get; set; }

        #region Mercado Livre

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TIPO_INTEGRACAO_MERCADO_LIVRE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre? TipoIntegracaoMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_INTEGRACAO_MERCADO_LIVRE_CONSULTA_ROTAFACILITY_AUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_INTEGRACAO_MERCADO_LIVRE_AVANCAR_ETAPA_NFE_AUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_TIPO_ACRESCIMO_DECRESCIMO_DATA_PREVISAO_SAIDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida TipoTempoAcrescimoDecrescimoDataPrevisaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks", Column = "CLI_ACRESCENTAR_DATA_PREVISAO_SAIDA_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks { get; set; }

        public virtual TimeSpan TempoAcrescimoDecrescimoDataPrevisaoSaida
        {
            get { return TimeSpan.FromTicks(TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks); }
            set { TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks = value.Ticks; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MesoRegiao", Column = "MRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Localidades.MesoRegiao MesoRegiao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Localidades.Regiao Regiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLI_POSSUI_FILIAL_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiFilialCliente { get; set; }

        #endregion

        #endregion

        #region Propriedades Virtuais

        public virtual string DescricaoIndicadorIE
        {
            get
            {
                switch (this.IndicadorIE)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS:
                        return "1 - Contribuinte ICMS (informar a IE do destinatário);";
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento:
                        return "2 - Contribuinte isento de Inscrição no cadastro de Contribuintes do ICMS;";
                    case ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte:
                        return "9  - Não Contribuinte, que pode ou não possuir Inscrição Estadual no Cadastro de Contribuintes do ICMS.";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoContaBanco
        {
            get
            {
                switch (this.TipoContaBanco)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente:
                        return "Corrente";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança:
                        return "Poupança";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Salario:
                        return "Salário";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                string descricao = "";

                string nome = this.Nome;
                if (this.PontoTransbordo)
                    nome = this.NomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.CodigoIntegracao))
                    descricao += this.CodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.Tipo))
                    descricao += " (" + this.CPF_CNPJ_Formatado + ")";

                return descricao;
            }
        }

        public virtual string EnderecoCompleto
        {
            get
            {
                List<string> dadosEndereco = new List<string>();

                if (!string.IsNullOrWhiteSpace(Endereco))
                    dadosEndereco.Add(Endereco);

                if (!string.IsNullOrWhiteSpace(Bairro))
                    dadosEndereco.Add(Bairro);

                if (!string.IsNullOrWhiteSpace(Numero))
                    dadosEndereco.Add(Numero);

                return string.Join(", ", dadosEndereco);
            }
        }

        public virtual string EnderecoCompletoCidadeeEstado
        {
            get
            {
                return $"{EnderecoCompleto} - {Localidade.DescricaoCidadeEstado}";
            }
        }

        public virtual string EnderecoCompletoCidadeeEstadoFone
        {
            get
            {
                return Telefone1 == null && Telefone2 == null ? EnderecoCompletoCidadeeEstado : $"{EnderecoCompletoCidadeeEstado} - {Telefone1 ?? Telefone2}";
            }
        }

        public virtual string NomeCNPJ
        {
            get
            {
                string descricao = "";

                if (!string.IsNullOrWhiteSpace(this.Nome))
                    descricao += this.Nome;
                if (!string.IsNullOrWhiteSpace(this.Tipo))
                    descricao += $" ({this.CPF_CNPJ_Formatado})";

                return descricao;
            }
        }

        public virtual string NomeCNPJLimitado
        {
            get
            {
                string descricao = "";

                if (!string.IsNullOrWhiteSpace(this.Nome))
                    descricao += this.Nome;
                if (!string.IsNullOrWhiteSpace(this.Tipo))
                    descricao += $" ({this.CPF_CNPJ_Formatado})";

                if (!string.IsNullOrEmpty(descricao) && descricao.Length > 60)
                    descricao = descricao.Substring(0, 60);

                return descricao;
            }
        }

        public virtual string DescricaoTipoLogradouro
        {
            get
            {
                switch (this.TipoLogradouro)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Avenida:
                        return "Avenida";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Estrada:
                        return "Estrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros:
                        return "Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Praca:
                        return "Praça";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rodovia:
                        return "Rodovia";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rua:
                        return "Rua";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Travessa:
                        return "Travessa";
                    default:
                        return "";
                }
            }
        }

        public virtual string CPF_CNPJ_Formatado
        {
            get
            {
                if (this.Tipo?.Equals("E") ?? false)
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return (this.Tipo?.Equals("J") ?? false) ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", this.CPF_CNPJ);
                }
            }
        }

        public virtual string CPF_CNPJ_Formatado_AX
        {
            get
            {

                if (this.Tipo.Equals("E"))
                {
                    return "000000000000-00";
                }
                else
                {
                    return this.Tipo.Equals("J") ? String.Format(@"{0:000000000000\-00}", this.CPF_CNPJ) : String.Format(@"{0:000000000\-00}", this.CPF_CNPJ);
                }
            }
        }

        public virtual string CPF_CNPJ_SemFormato
        {
            get
            {
                if (this.Tipo != null && this.Tipo.Equals("E"))
                {
                    return "00000000000000";
                }
                else
                {
                    return this.Tipo != null && this.Tipo.Equals("J") ? String.Format(@"{0:00000000000000}", this.CPF_CNPJ) : String.Format(@"{0:00000000000}", this.CPF_CNPJ);
                }
            }
        }

        public virtual string CEPFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CEP) ? string.Format(@"{0:00\.000\-000}", long.Parse(this.CEP)) : string.Empty; }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual string RaizCnpj
        {
            get
            {
                return CPF_CNPJ.ToString().Substring(0, 8);
            }
        }

        public virtual string RaizCnpjSemFormato
        {
            get
            {
                return CPF_CNPJ_SemFormato.Substring(0, 8);
            }
        }

        public virtual TimeSpan TempoCarregamento
        {
            get { return TimeSpan.FromTicks(TempoCarregamentoTicks); }
            set { TempoCarregamentoTicks = value.Ticks; }
        }

        public virtual TimeSpan TempoDescarregamento
        {
            get { return TimeSpan.FromTicks(TempoDescarregamentoTicks); }
            set { TempoDescarregamentoTicks = value.Ticks; }
        }

        public virtual bool Equals(Cliente other)
        {
            if (other == null)
                return false;
            else if (other.CPF_CNPJ == CPF_CNPJ)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return CPF_CNPJ.GetHashCode();
        }

        public virtual bool IsPossuiRestricaoFilaCarregamento(Embarcador.Cargas.TipoDeCarga tipoCarga, Enumeradores.TipoTomador tipo)
        {
            int totalRestricoesFilaCarregamento = (
                from restricao in RestricoesFilaCarregamento
                where (
                          (restricao.TipoCarga?.Codigo == (tipoCarga?.Codigo ?? -1)) &&
                          ((restricao.Tipo == Enumeradores.TipoTomador.NaoInformado) || (restricao.Tipo == tipo))
                      )
                select restricao
            ).Count();

            return totalRestricoesFilaCarregamento > 0;
        }

        public virtual bool PossuiGeolocalizacao()
        {
            return !string.IsNullOrWhiteSpace(Latitude) && !string.IsNullOrWhiteSpace(Longitude);
        }

        public virtual string ChavePix_Formatado
        {
            get
            {
                switch (this.TipoChavePix)
                {
                    case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.CPFCNPJ:
                        return this.ChavePix;
                    case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Email:
                        return this.ChavePix;
                    case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Aleatoria:
                        return this.ChavePix;
                    case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Celular:
                        return this.ChavePix.ObterTelefoneFormatado();
                    default: return string.Empty;
                }

            }
        }
    }

    #endregion

    public class ClienteIndex
    {
        public ClienteIndex(int tamanhoGrupo)
        {
            this.IndicesCnpfCpf = new List<string>();
            this.tamanhoGrupo = tamanhoGrupo;
        }
        public string GrupoIndice { get; set; }
        public List<string> IndicesCnpfCpf { get; set; }
        public int tamanhoGrupo { get; set; }


        public bool BuscarGrupoCnpjCpf(double indice, List<ClienteIndex> lstClienteIndex)
        {
            string grupoIndex = indice.ToString().Substring(0, lstClienteIndex.ElementAt(0).tamanhoGrupo);
            ClienteIndex clienteIndex = lstClienteIndex.Where(o => o.GrupoIndice == grupoIndex).FirstOrDefault();
            if (clienteIndex == null)
                return false;

            return clienteIndex.IndicesCnpfCpf.Where(o => o == indice.ToString()).FirstOrDefault() != null ? true : false;
        }

        public List<ClienteIndex> MontarListaIndex(List<string> lstCodigos)
        {
            List<Dominio.Entidades.ClienteIndex> lstIndices = new List<Dominio.Entidades.ClienteIndex>();
            Dominio.Entidades.ClienteIndex indice = new Dominio.Entidades.ClienteIndex(this.tamanhoGrupo);
            lstCodigos = lstCodigos.OrderBy(x => x).ToList();
            foreach (var codigo in lstCodigos)
            {
                var grupoIndex = codigo.Substring(0, this.tamanhoGrupo);
                if (indice.GrupoIndice != grupoIndex)
                {
                    indice = new Dominio.Entidades.ClienteIndex(this.tamanhoGrupo);
                    indice.GrupoIndice = grupoIndex;
                    lstIndices.Add(indice);
                }
                indice.IndicesCnpfCpf.Add(codigo);
            }
            return lstIndices;
        }
    }
}

