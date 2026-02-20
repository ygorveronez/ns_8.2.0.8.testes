using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Global
{
    sealed class ConsultaPessoa : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa>
    {
        #region Construtores

        public ConsultaPessoa() : base(tabela: ObterTabela()) { }

        #endregion

        #region Métodos Privados

        private static string ObterTabela()
        {
            return
                @"( 
                    select Cliente.CLI_CGCCPF CNPJCPF,						   
                        Cliente.CLI_CODIGO_INTEGRACAO CodigoIntegracao,						   
                        Cliente.CLI_EMAIL Email,						   
                        Cliente.CLI_NOMEFANTASIA NomeFantasia,						   
                        Cliente.CLI_OBSERVACAO Observacao,						   
                        Cliente.CLI_NOME RazaoSocial,						   
                        Cliente.CLI_FISJUR TipoPessoa,
                        Cliente.CLI_BANCO_AGENCIA Agencia,						   
                        Cliente.CLI_BANCO_DIGITO_AGENCIA Digito,		
                        Cliente.CLI_BANCO_NUMERO_CONTA NumeroConta,		
                        Cliente.CLI_BANCO_TIPO_CONTA TipoConta,	
                        Cliente.CLI_ATIVO Ativo,	
                        Cliente.CLI_DATACAD DataCadastro,
                        Cliente.CLI_PIS_PASEP PISPASEP,
                        Cliente.CLI_DATA_NASCIMENTO DataNascimento,
                        Cliente.CLI_INDICADOR_IE IndicadorIE,
                        Cliente.CLI_AGUARDANDO_CONFERENCIA_INFORMACAO AguardandoConferenciaInformacao,

                        Cliente.CLI_CODIGO_DOCUMENTO CodigoDocumento,
                        Cliente.CLI_LATIDUDE Latitude,
                        Cliente.CLI_LONGITUDE Longitude,
                        Cliente.CLI_BAIRRO Bairro,
                        Cliente.CLI_CEP CEP,
                        Cliente.CLI_COMPLEMENTO Complemento,
                        Cliente.CLI_ENDERECO Endereco,
                        Cliente.CLI_IERG IE,
                        Cliente.CLI_NUMERO Numero,
                        Cliente.CLI_FONE TelefonePrincipal,
                        Cliente.CLI_FAX TelefoneSecundario,
                        Cliente.LOC_CODIGO CodigoLocalidade,
                        Cliente.CLI_RAIO_METROS Raio,
                        Cliente.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS ExigeAgendamento,
						   
                        Cliente.ATI_CODIGO CodigoAtividade,                           
                        Cliente.GRP_CODIGO CodigoGrupoPessoa,
                        Cliente.CLI_CGCCPF_PORTADOR_CONTA CodigoClientePortadorConta,
                        Cliente.BCO_CODIGO CodigoBanco,
                           
                        Cliente.CLI_TIPO_ENVIO_FATURA TipoEnvioFatura,
                        Cliente.CLI_TIPO_PRAZO_FATURAMENTO TipoPrazoFaturamento,
                        Cliente.CLI_DIA_DE_PRAZO_FATURA DiasPrazoFaturamento,
                        Cliente.CLI_FORMA_GERACAO_TITULO_FATURA FormaGeracaoTituloFatura,

                        SUBSTRING((SELECT DISTINCT ', ' + 
                        (CASE WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 1 THEN 'Domingo'
                        WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 2 THEN 'Segunda-Feira'
                        WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 3 THEN 'Terça-Feira'
                        WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 4 THEN 'Quarta-Feira'
                        WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 5 THEN 'Quinta-Feira'
                        WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 6 THEN 'Sexta-Feira'
                        ELSE 'Sábado' END)
                        FROM T_CLIENTE_DIA_SEMANA_FATURA diasSemana
                        WHERE diasSemana.CLI_CGCCPF = Cliente.CLI_CGCCPF  FOR XML PATH('')), 3, 1000) DiaSemana, 

                        SUBSTRING((SELECT DISTINCT ', ' + CAST(diasMes.CLI_DIA_MES_FATURA AS NVARCHAR(20))
                        FROM T_CLIENTE_DIA_MES_FATURA diasMes
                        WHERE diasMes.CLI_CGCCPF = Cliente.CLI_CGCCPF  FOR XML PATH('')), 3, 1000) DiaMes,

                        Cliente.CLI_CONTA_FORNECEDOR_EBS ContaContabil, Cliente.BCO_CODIGO_DOC CodigoBancoDOC,
                        Cliente.CLI_ESTADOCIVIL,
                        Cliente.CLI_SEXO,
                        Cliente.CLI_TIPO_FORNECEDOR,
                        Cliente.CLI_CODIGO_CATEGORIA_TRABALHADOR,
                        Cliente.CLI_FUNCAO,
                        Cliente.CLI_PAGAMENTO_EM_BANCO,
                        Cliente.CLI_FORMA_PAGAMENTO_ESOCIAL,
                        Cliente.CLI_TIPO_AUTONOMO,
                        Cliente.CLI_CODIGO_RECEITA,
                        Cliente.CLI_TIPO_PAGAMENTO_BANCARIO,
                        Cliente.CLI_NAO_DESCONTA_IRRF,
					    Cliente.CTP_CODIGO CodigoCategoria,
                        Cliente.CLI_REGIME_TRIBUTARIO RegimeTributario,
                        Cliente.CLI_TIPO_AREA,
                        Cliente.CLI_TIPO_INTEGRACAO_LBC,
                        Cliente.CLI_DATACAD
                        from T_CLIENTE Cliente
                      
                      union
                      
                    select Cliente.CLI_CGCCPF CNPJCPF,						   
		                    Cliente.CLI_CODIGO_INTEGRACAO CodigoIntegracao,						   
		                    Cliente.CLI_EMAIL Email,						   
		                    Cliente.CLI_NOMEFANTASIA NomeFantasia,						   
		                    Cliente.CLI_OBSERVACAO Observacao,						   
		                    Cliente.CLI_NOME RazaoSocial,						   
		                    Cliente.CLI_FISJUR TipoPessoa,
		                    Cliente.CLI_BANCO_AGENCIA Agencia,						   
		                    Cliente.CLI_BANCO_DIGITO_AGENCIA Digito,		
		                    Cliente.CLI_BANCO_NUMERO_CONTA NumeroConta,
		                    Cliente.CLI_BANCO_TIPO_CONTA TipoConta,	
                            Cliente.CLI_ATIVO Ativo,	
                            Cliente.CLI_DATACAD DataCadastro,
                            Cliente.CLI_PIS_PASEP PISPASEP,
                            Cliente.CLI_DATA_NASCIMENTO DataNascimento,
                            Cliente.CLI_INDICADOR_IE IndicadorIE,
                            Cliente.CLI_AGUARDANDO_CONFERENCIA_INFORMACAO AguardandoConferenciaInformacao,

                            ClienteOutroEndereco.COE_CODIGO_DOCUMENTO CodigoDocumento,
                            ClienteOutroEndereco.COE_LATIDUDE Latitude,
                            ClienteOutroEndereco.COE_LONGITUDE Longitude,
                            ClienteOutroEndereco.COE_BAIRRO Bairro,
		                    ClienteOutroEndereco.COE_CEP CEP,
                            ClienteOutroEndereco.COE_COMPLEMENTO Complemento,
                            ClienteOutroEndereco.COE_ENDERECO Endereco,
		                    ClienteOutroEndereco.COE_IERG IE,
                            ClienteOutroEndereco.COE_NUMERO Numero,
                            ClienteOutroEndereco.COE_FONE TelefonePrincipal,
		                    '' TelefoneSecundario,
                            ClienteOutroEndereco.LOC_CODIGO CodigoLocalidade,
						   
		                    Cliente.ATI_CODIGO CodigoAtividade,                           
                            Cliente.GRP_CODIGO CodigoGrupoPessoa,
                            Cliente.CLI_CGCCPF_PORTADOR_CONTA CodigoClientePortadorConta,
                            Cliente.BCO_CODIGO CodigoBanco,
                            Cliente.CLI_TIPO_ENVIO_FATURA TipoEnvioFatura,
                            Cliente.CLI_TIPO_PRAZO_FATURAMENTO TipoPrazoFaturamento,
                            Cliente.CLI_DIA_DE_PRAZO_FATURA DiasPrazoFaturamento,
                            Cliente.CLI_FORMA_GERACAO_TITULO_FATURA FormaGeracaoTituloFatura,
                            Cliente.CLI_RAIO_METROS Raio,
                            Cliente.CLI_EXIGE_QUE_ENTREGAS_SEJAM_AGENDADAS ExigeAgendamento,

                            SUBSTRING((SELECT DISTINCT ', ' + 
                            (CASE WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 1 THEN 'Domingo'
                            WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 2 THEN 'Segunda-Feira'
                            WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 3 THEN 'Terça-Feira'
                            WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 4 THEN 'Quarta-Feira'
                            WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 5 THEN 'Quinta-Feira'
                            WHEN diasSemana.CLI_DIA_SEMANA_FATURA = 6 THEN 'Sexta-Feira'
                            ELSE 'Sábado' END)
                            FROM T_CLIENTE_DIA_SEMANA_FATURA diasSemana
                            WHERE diasSemana.CLI_CGCCPF = Cliente.CLI_CGCCPF  FOR XML PATH('')), 3, 1000) DiaSemana, 

                            SUBSTRING((SELECT DISTINCT ', ' + CAST(diasMes.CLI_DIA_MES_FATURA AS NVARCHAR(20))
                            FROM T_CLIENTE_DIA_MES_FATURA diasMes
                            WHERE diasMes.CLI_CGCCPF = Cliente.CLI_CGCCPF  FOR XML PATH('')), 3, 1000) DiaMes,

                            Cliente.CLI_CONTA_FORNECEDOR_EBS ContaContabil, Cliente.BCO_CODIGO_DOC CodigoBancoDOC,
                            Cliente.CLI_ESTADOCIVIL,
                            Cliente.CLI_SEXO,
                            Cliente.CLI_TIPO_FORNECEDOR,
                            Cliente.CLI_CODIGO_CATEGORIA_TRABALHADOR,
                            Cliente.CLI_FUNCAO,
                            Cliente.CLI_PAGAMENTO_EM_BANCO,
                            Cliente.CLI_FORMA_PAGAMENTO_ESOCIAL,
                            Cliente.CLI_TIPO_AUTONOMO,
                            Cliente.CLI_CODIGO_RECEITA,
                            Cliente.CLI_TIPO_PAGAMENTO_BANCARIO,
                            Cliente.CLI_NAO_DESCONTA_IRRF,
							Cliente.CTP_CODIGO CodigoCategoria,
                            Cliente.CLI_REGIME_TRIBUTARIO RegimeTributario,
                            Cliente.CLI_TIPO_AREA,
                            Cliente.CLI_TIPO_INTEGRACAO_LBC,
                            Cliente.CLI_DATACAD
                      from T_CLIENTE_OUTRO_ENDERECO ClienteOutroEndereco
                      join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = ClienteOutroEndereco.CLI_CGCCPF                      
                ) as Pessoa";
        }

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            if (!joins.Contains(" Localidade "))
                joins.Append(" join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Pessoa.CodigoLocalidade ");
        }

        private void SetarJoinsEstado(StringBuilder joins)
        {
            SetarJoinsLocalidade(joins);

            if (!joins.Contains(" Estado "))
                joins.Append(" join T_UF Estado on Estado.UF_SIGLA = Localidade.UF_SIGLA ");
        }

        private void SetarJoinsPais(StringBuilder joins)
        {
            SetarJoinsLocalidade(joins);

            if (!joins.Contains(" Pais "))
                joins.Append(" join T_PAIS Pais on Pais.PAI_CODIGO = Localidade.PAI_CODIGO ");
        }

        private void SetarJoinsAtividade(StringBuilder joins)
        {
            if (!joins.Contains(" Atividade "))
                joins.Append(" join T_ATIVIDADES Atividade on Atividade.ATI_CODIGO = Pessoa.CodigoAtividade ");
        }

        private void SetarJoinsBancoDOC(StringBuilder joins)
        {
            if (!joins.Contains(" BancoDOC "))
                joins.Append(" left outer join T_BANCO BancoDOC on BancoDOC.BCO_CODIGO = Pessoa.CodigoBancoDOC ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoas "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Pessoa.CodigoGrupoPessoa ");
        }

        private void SetarJoinsClientePortadorConta(StringBuilder joins)
        {
            if (!joins.Contains(" ClientePortadorConta "))
                joins.Append(" left join T_CLIENTE ClientePortadorConta on ClientePortadorConta.CLI_CGCCPF = Pessoa.CodigoClientePortadorConta ");
        }

        private void SetarJoinsBanco(StringBuilder joins)
        {
            if (!joins.Contains(" Banco "))
                joins.Append(" left join T_BANCO Banco on Banco.BCO_CODIGO = Pessoa.CodigoBanco ");
        }

        private void SetarJoinsCategoriaPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" Categoria "))
                joins.Append(" left join T_CATEGORIA_PESSOA Categoria on Categoria.CTP_CODIGO = Pessoa.CodigoCategoria ");
        }

        private void SetarJoinsModalidadePessoasTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" ModalidadePessoasTransportador "))
                joins.Append(" left join T_CLIENTE_MODALIDADE ModalidadePessoasTransportador on ModalidadePessoasTransportador.CPF_CNPJ = Pessoa.CNPJCPF and ModalidadePessoasTransportador.MOD_TIPO = 3 ");
        }

        private void SetarJoinsModalidadeTransportadoraPessoa(StringBuilder joins)
        {
            SetarJoinsModalidadePessoasTransportador(joins);

            if (!joins.Contains(" ModalidadeTransportadoraPessoas "))
                joins.Append(" left join T_CLIENTE_MODALIDADE_TRANSPORTADORAS ModalidadeTransportadoraPessoas on ModalidadeTransportadoraPessoas.MOD_CODIGO = ModalidadePessoasTransportador.MOD_CODIGO ");
        }

        private void SetarJoinsTipoTerceiro(StringBuilder joins)
        {
            if (!joins.Contains(" TipoTerceiro "))
                joins.Append(" left join T_TIPO_TERCEIRO TipoTerceiro on TipoTerceiro.TPT_CODIGO = ModalidadeTransportadoraPessoas.TPT_CODIGO ");
        }

        private void SetarJoinsLocaisAreas(StringBuilder joins)
        {
            if (!joins.Contains(" Locais "))
                joins.Append(" left join T_LOCAIS Locais on Locais.LOC_CODIGO = Pessoa.CodigoLocalidade ");
        }

        private void SetarJoinsVendedor(StringBuilder joins)
        {
            if (!joins.Contains(" Vendedor "))
                joins.Append(" LEFT JOIN T_PESSOA_FUNCIONARIO Funcionario_pf on Pessoa.cnpjcpf = Funcionario_pf.CLI_CGCCPF left join T_FUNCIONARIO Funcionario on Funcionario_pf.FUN_CODIGO = Funcionario.FUN_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtroPesquisa)
        {
            switch (propriedade)
            {
                case "EstadoCivilFormatado":
                    if (!select.Contains(" EstadoCivil, "))
                    {
                        select.Append("Pessoa.CLI_ESTADOCIVIL as EstadoCivil, ");
                        groupBy.Append("Pessoa.CLI_ESTADOCIVIL, ");
                    }
                    break;
                case "SexoFormatado":
                    if (!select.Contains(" Sexo, "))
                    {
                        select.Append("Pessoa.CLI_SEXO as Sexo, ");
                        groupBy.Append("Pessoa.CLI_SEXO, ");
                    }
                    break;
                case "TipoFornecedor":
                    if (!select.Contains(" TipoFornecedor, "))
                    {
                        select.Append("Pessoa.CLI_TIPO_FORNECEDOR as TipoFornecedor, ");
                        groupBy.Append("Pessoa.CLI_TIPO_FORNECEDOR, ");
                    }
                    break;
                case "CodigoCategoriaTrabalhador":
                    if (!select.Contains(" CodigoCategoriaTrabalhador, "))
                    {
                        select.Append("Pessoa.CLI_CODIGO_CATEGORIA_TRABALHADOR as CodigoCategoriaTrabalhador, ");
                        groupBy.Append("Pessoa.CLI_CODIGO_CATEGORIA_TRABALHADOR, ");
                    }
                    break;
                case "Funcao":
                    if (!select.Contains(" Funcao, "))
                    {
                        select.Append("Pessoa.CLI_FUNCAO as Funcao, ");
                        groupBy.Append("Pessoa.CLI_FUNCAO, ");
                    }
                    break;
                case "PagamentoEmBanco":
                    if (!select.Contains(" PagamentoEmBanco, "))
                    {
                        select.Append("Pessoa.CLI_PAGAMENTO_EM_BANCO as PagamentoEmBanco, ");
                        groupBy.Append("Pessoa.CLI_PAGAMENTO_EM_BANCO, ");
                    }
                    break;
                case "FormaPagamentoeSocial":
                    if (!select.Contains(" FormaPagamentoeSocial, "))
                    {
                        select.Append("Pessoa.CLI_FORMA_PAGAMENTO_ESOCIAL as FormaPagamentoeSocial, ");
                        groupBy.Append("Pessoa.CLI_FORMA_PAGAMENTO_ESOCIAL, ");
                    }
                    break;
                case "BancoDOC":
                    if (!select.Contains(" BancoDOC, "))
                    {
                        select.Append("CAST(BancoDOC.BCO_NUMERO AS VARCHAR(9)) as BancoDOC, ");
                        groupBy.Append("BancoDOC.BCO_NUMERO, BancoDOC.BCO_CODIGO, ");

                        SetarJoinsBancoDOC(joins);
                    }
                    break;
                case "Categoria":
                    if (!select.Contains(" Categoria, "))
                    {
                        select.Append("Categoria.CTP_DESCRICAO as Categoria, ");
                        groupBy.Append("Categoria.CTP_DESCRICAO, ");

                        SetarJoinsCategoriaPessoa(joins);
                    }
                    break;
                case "TipoAutonomo":
                    if (!select.Contains(" TipoAutonomo, "))
                    {
                        select.Append("Pessoa.CLI_TIPO_AUTONOMO as TipoAutonomo, ");
                        groupBy.Append("Pessoa.CLI_TIPO_AUTONOMO, ");
                    }
                    break;
                case "CodigoReceita":
                    if (!select.Contains(" CodigoReceita, "))
                    {
                        select.Append("Pessoa.CLI_CODIGO_RECEITA as CodigoReceita, ");
                        groupBy.Append("Pessoa.CLI_CODIGO_RECEITA, ");
                    }
                    break;
                case "TipoPagamentoBancario":
                    if (!select.Contains(" TipoPagamentoBancario, "))
                    {
                        select.Append("Pessoa.CLI_TIPO_PAGAMENTO_BANCARIO as TipoPagamentoBancario, ");
                        groupBy.Append("Pessoa.CLI_TIPO_PAGAMENTO_BANCARIO, ");
                    }
                    break;
                case "NaoDescontaIRRF":
                    if (!select.Contains(" NaoDescontaIRRF, "))
                    {
                        select.Append("Pessoa.CLI_NAO_DESCONTA_IRRF as NaoDescontaIRRF, ");
                        groupBy.Append("Pessoa.CLI_NAO_DESCONTA_IRRF, ");
                    }
                    break;
                case "Atividade":
                    if (!select.Contains(" Atividade, "))
                    {
                        select.Append("(CAST(Atividade.ATI_CODIGO AS VARCHAR(9)) + ' - ' + Atividade.ATI_DESCRICAO) as Atividade, ");
                        groupBy.Append("Atividade.ATI_CODIGO, Atividade.ATI_DESCRICAO, ");

                        SetarJoinsAtividade(joins);
                    }
                    break;

                case "Bairro":
                    if (!select.Contains(" Bairro, "))
                    {
                        select.Append("Pessoa.Bairro as Bairro, ");
                        groupBy.Append("Pessoa.Bairro, ");
                    }
                    break;

                case "CEP":
                    if (!select.Contains(" CEP, "))
                    {
                        select.Append("Pessoa.CEP as CEP, ");
                        groupBy.Append("Pessoa.CEP, ");
                    }
                    break;

                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        select.Append("Localidade.LOC_DESCRICAO as Cidade, ");
                        groupBy.Append("Localidade.LOC_DESCRICAO, ");

                        SetarJoinsEstado(joins);
                    }
                    break;

                case "UF":
                    if (!select.Contains(" UF, "))
                    {
                        select.Append("Estado.UF_SIGLA as UF, ");
                        groupBy.Append("Estado.UF_SIGLA, ");

                        SetarJoinsEstado(joins);
                    }
                    break;

                case "Pais":
                    if (!select.Contains(" Pais, "))
                    {
                        select.Append("Pais.PAI_NOME as Pais, ");
                        groupBy.Append("Pais.PAI_NOME, ");

                        SetarJoinsPais(joins);
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("Pessoa.CodigoIntegracao as CodigoIntegracao, ");
                        groupBy.Append("Pessoa.CodigoIntegracao, ");
                    }
                    break;

                case "Complemento":
                    if (!select.Contains(" Complemento, "))
                    {
                        select.Append("Pessoa.Complemento as Complemento, ");
                        groupBy.Append("Pessoa.Complemento, ");
                    }
                    break;

                case "CPFCNPJFormatado":
                    if (!select.Contains(" CPFCNPJ, "))
                    {
                        select.Append("Pessoa.CNPJCPF as CPFCNPJ, Pessoa.TipoPessoa as TipoPessoa, ");
                        groupBy.Append("Pessoa.CNPJCPF, Pessoa.TipoPessoa, ");
                    }
                    break;

                case "Email":
                    if (!select.Contains(" Email, "))
                    {
                        select.Append("Pessoa.Email as Email, ");
                        groupBy.Append("Pessoa.Email, ");
                    }
                    break;

                case "Endereco":
                    if (!select.Contains(" Endereco, "))
                    {
                        select.Append("Pessoa.Endereco as Endereco, ");
                        groupBy.Append("Pessoa.Endereco, ");
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas, "))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO as GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "IE":
                    if (!select.Contains(" IE, "))
                    {
                        select.Append("Pessoa.IE as IE, ");
                        groupBy.Append("Pessoa.IE, ");
                    }
                    break;

                case "NomeFantasia":
                    if (!select.Contains(" NomeFantasia, "))
                    {
                        select.Append("Pessoa.NomeFantasia as NomeFantasia, ");
                        groupBy.Append("Pessoa.NomeFantasia, ");
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("Pessoa.Numero as Numero, ");
                        groupBy.Append("Pessoa.Numero, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("Pessoa.Observacao as Observacao, ");
                        groupBy.Append("Pessoa.Observacao, ");
                    }
                    break;

                case "RazaoSocial":
                    if (!select.Contains(" RazaoSocial, "))
                    {
                        select.Append("Pessoa.RazaoSocial as RazaoSocial, ");
                        groupBy.Append("Pessoa.RazaoSocial, ");
                    }
                    break;

                case "TelefonePrincipal":
                    if (!select.Contains(" TelefonePrincipal, "))
                    {
                        select.Append("Pessoa.TelefonePrincipal as TelefonePrincipal, ");
                        groupBy.Append("Pessoa.TelefonePrincipal, ");
                    }
                    break;

                case "TelefoneSecundario":
                    if (!select.Contains(" TelefoneSecundario, "))
                    {
                        select.Append("Pessoa.TelefoneSecundario as TelefoneSecundario, ");
                        groupBy.Append("Pessoa.TelefoneSecundario, ");
                    }
                    break;

                case "PortadorConta":
                    if (!select.Contains(" PortadorConta, "))
                    {
                        select.Append("ClientePortadorConta.CLI_NOME as PortadorConta, ");
                        groupBy.Append("ClientePortadorConta.CLI_NOME, ");

                        SetarJoinsClientePortadorConta(joins);
                    }
                    break;

                case "Banco":
                    if (!select.Contains(" Banco, "))
                    {
                        select.Append("Banco.BCO_DESCRICAO as Banco, ");
                        groupBy.Append("Banco.BCO_DESCRICAO, ");

                        SetarJoinsBanco(joins);
                    }
                    break;

                case "Agencia":
                    if (!select.Contains(" Agencia, "))
                    {
                        select.Append("Pessoa.Agencia as Agencia, ");
                        groupBy.Append("Pessoa.Agencia, ");
                    }
                    break;

                case "Digito":
                    if (!select.Contains(" Digito, "))
                    {
                        select.Append("Pessoa.Digito as Digito, ");
                        groupBy.Append("Pessoa.Digito, ");
                    }
                    break;

                case "NumeroConta":
                    if (!select.Contains(" NumeroConta, "))
                    {
                        select.Append("Pessoa.NumeroConta as NumeroConta, ");
                        groupBy.Append("Pessoa.NumeroConta, ");
                    }
                    break;

                case "TipoContaFormatada":
                    if (!select.Contains(" TipoConta, "))
                    {
                        select.Append("Pessoa.TipoConta as TipoConta, ");
                        groupBy.Append("Pessoa.TipoConta, ");
                    }
                    break;

                case "PISPASEP":
                    if (!select.Contains(" PISPASEP, "))
                    {
                        select.Append("Pessoa.PISPASEP as PISPASEP, ");
                        groupBy.Append("Pessoa.PISPASEP, ");
                    }
                    break;

                case "DataNascimentoFormatada":
                    if (!select.Contains(" DataNascimento, "))
                    {
                        select.Append("Pessoa.DataNascimento as DataNascimento, ");
                        groupBy.Append("Pessoa.DataNascimento, ");
                    }
                    break;

                case "LatitudeFormatada":
                    if (!select.Contains(" Latitude, "))
                    {
                        select.Append("Pessoa.Latitude as Latitude, ");
                        groupBy.Append("Pessoa.Latitude, ");
                    }
                    break;

                case "LongitudeFormatada":
                    if (!select.Contains(" Longitude, "))
                    {
                        select.Append("Pessoa.Longitude as Longitude, ");
                        groupBy.Append("Pessoa.Longitude, ");
                    }
                    break;

                case "IndicadorIEFormatado":
                    if (!select.Contains(" IndicadorIE, "))
                    {
                        select.Append("Pessoa.IndicadorIE as IndicadorIE, ");
                        groupBy.Append("Pessoa.IndicadorIE, ");
                    }
                    break;

                case "CodigoDocumento":
                    if (!select.Contains(" CodigoDocumento, "))
                    {
                        select.Append("Pessoa.CodigoDocumento as CodigoDocumento, ");
                        groupBy.Append("Pessoa.CodigoDocumento, ");
                    }
                    break;

                case "Bloqueado":
                    if (!select.Contains(" Bloqueado, "))
                    {
                        select.Append("(CASE WHEN GrupoPessoas.GRP_BLOQUEADO = 1 THEN 'Sim' ELSE 'Não' END) as Bloqueado, ");
                        groupBy.Append("GrupoPessoas.GRP_BLOQUEADO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "DataBloqueioFormatada":
                    if (!select.Contains(" DataBloqueio, "))
                    {
                        select.Append("GrupoPessoas.GRP_DATA_BLOQUEIO as DataBloqueio, ");
                        groupBy.Append("GrupoPessoas.GRP_DATA_BLOQUEIO, ");
                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "AguardandoConferenciaInformacao":
                    if (!select.Contains(" AguardandoConferenciaInformacao, "))
                    {
                        select.Append("(CASE WHEN Pessoa.AguardandoConferenciaInformacao = 1 THEN 'Sim' ELSE 'Não' END) as AguardandoConferenciaInformacao, ");
                        groupBy.Append("Pessoa.AguardandoConferenciaInformacao, ");
                    }
                    break;

                case "TipoEnvioFaturaFormatado":
                    if (!select.Contains(" TipoEnvioFatura, "))
                    {
                        select.Append("Pessoa.TipoEnvioFatura as TipoEnvioFatura, ");
                        groupBy.Append("Pessoa.TipoEnvioFatura, ");
                    }
                    break;

                case "TipoPrazoFaturamentoFormatado":
                    if (!select.Contains(" TipoPrazoFaturamento, "))
                    {
                        select.Append("Pessoa.TipoPrazoFaturamento as TipoPrazoFaturamento, ");
                        groupBy.Append("Pessoa.TipoPrazoFaturamento, ");
                    }
                    break;

                case "FormaGeracaoTituloFaturaFormatado":
                    if (!select.Contains(" FormaGeracaoTituloFatura, "))
                    {
                        select.Append("Pessoa.FormaGeracaoTituloFatura as FormaGeracaoTituloFatura, ");
                        groupBy.Append("Pessoa.FormaGeracaoTituloFatura, ");
                    }
                    break;

                case "DiasPrazoFaturamento":
                    if (!select.Contains(" DiasPrazoFaturamento, "))
                    {
                        select.Append("CAST(Pessoa.DiasPrazoFaturamento AS NVARCHAR(20)) as DiasPrazoFaturamento, ");
                        groupBy.Append("Pessoa.DiasPrazoFaturamento, ");
                    }
                    break;

                case "DiaSemana":
                    if (!select.Contains(" DiaSemana, "))
                    {
                        select.Append("Pessoa.DiaSemana as DiaSemana, ");
                        groupBy.Append("Pessoa.DiaSemana, ");
                    }
                    break;

                case "DiaMes":
                    if (!select.Contains(" DiaMes, "))
                    {
                        select.Append("Pessoa.DiaMes as DiaMes, ");
                        groupBy.Append("Pessoa.DiaMes, ");
                    }
                    break;

                case "ContaContabil":
                    if (!select.Contains(" ContaContabil, "))
                    {
                        select.Append("Pessoa.ContaContabil as ContaContabil, ");
                        groupBy.Append("Pessoa.ContaContabil, ");
                    }
                    break;

                case "RegimeTributarioFormatado":
                    if (!select.Contains(" RegimeTributario, "))
                    {
                        select.Append("Pessoa.RegimeTributario as RegimeTributario, ");
                        groupBy.Append("Pessoa.RegimeTributario, ");
                    }
                    break; 

                case "Raio":
                    if (!select.Contains(" Raio, "))
                    {
                        select.Append("Pessoa.Raio as Raio, ");
                        groupBy.Append("Pessoa.Raio, ");
                    }
                    break;

                case "ExigeAgendamentoDescricao":
                    if (!select.Contains(" ExigeAgendamento, "))
                    {
                        select.Append("Pessoa.ExigeAgendamento as ExigeAgendamento, ");
                        groupBy.Append("Pessoa.ExigeAgendamento, ");
                    }
                    break;

                case "RNTRC":
                    if (!select.Contains(" RNTRC, "))
                    {
                        select.Append("ModalidadeTransportadoraPessoas.MOT_RNTRC as RNTRC, ");
                        groupBy.Append("ModalidadeTransportadoraPessoas.MOT_RNTRC, ");

                        SetarJoinsModalidadeTransportadoraPessoa(joins);
                    }
                    break;

                case "TipoAreaDescricao":
                    if (!select.Contains(" TipoArea, "))
                    {
                        select.Append("Pessoa.CLI_TIPO_AREA as TipoArea, ");
                        groupBy.Append("Pessoa.CLI_TIPO_AREA, ");

                        SetarJoinsLocaisAreas(joins);
                    }
                    break;

                case "Modalidade":
                    if (!select.Contains(" Modalidade, "))
                    {
                        select.Append($@"SUBSTRING((SELECT DISTINCT ', ' + CASE
											WHEN clienteModalidade.MOD_TIPO = 2
												THEN '{TipoModalidade.Fornecedor.ObterDescricao()}'
											WHEN clienteModalidade.MOD_TIPO = 3
												THEN '{TipoModalidade.TransportadorTerceiro.ObterDescricao()}'
											ELSE
												'{TipoModalidade.Cliente.ObterDescricao()}'
											END
                            FROM T_CLIENTE _cliente
								left join T_CLIENTE_MODALIDADE clienteModalidade on _cliente.CLI_CGCCPF = clienteModalidade.CPF_CNPJ
                            WHERE _cliente.CLI_CGCCPF = Pessoa.CNPJCPF  FOR XML PATH('')), 3, 1000) Modalidade, ");

                        if (!groupBy.Contains("Pessoa.CNPJCPF"))
                            groupBy.Append("Pessoa.CNPJCPF, ");
                    }
                    break;

                case "Filiais":
                    if (!select.Contains(" Filiais, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + _cliente.CLI_NOME
                                                    FROM T_CLIENTE_FILIAL filial
								                        join T_CLIENTE _cliente on _cliente.CLI_CGCCPF = filial.CLI_CGCCPF
                                                    WHERE _cliente.CLI_CGCCPF = Pessoa.CNPJCPF  FOR XML PATH('')), 3, 1000) Filiais, ");

                        if (!groupBy.Contains("Pessoa.CNPJCPF"))
                            groupBy.Append("Pessoa.CNPJCPF, ");
                    }
                    break;

                case "TipoClienteIntegracao":
                case "TipoClienteIntegracaoFormatado":
                    if (!select.Contains(" TipoClienteIntegracao, "))
                    {
                        select.Append("Pessoa.CLI_TIPO_INTEGRACAO_LBC TipoClienteIntegracao, ");
                        groupBy.Append("Pessoa.CLI_TIPO_INTEGRACAO_LBC, ");
                    }
                    break;

                case "DataIntegracao":
                case "DataIntegracaoFormatada":
                    if (!select.Contains(" DataIntegracao, "))
                    {
                        select.Append("Pessoa.CLI_DATACAD DataIntegracao, ");
                        groupBy.Append("Pessoa.CLI_DATACAD, ");
                    }
                    break;

                case "TipoTerceiro":
                    if (!select.Contains(" TipoTerceiro, "))
                    {
                        select.Append("TipoTerceiro.TPT_DESCRICAO as TipoTerceiro, ");
                        groupBy.Append("TipoTerceiro.TPT_DESCRICAO, ");

                        SetarJoinsModalidadeTransportadoraPessoa(joins);
                        SetarJoinsTipoTerceiro(joins);
                    }
                    break;

                case "Vendedor":
                    if (!select.Contains(" Vendedor, "))
                    {
                        select.Append("STRING_AGG(funcionario.FUN_NOME, ', ') as Vendedor, ");
                        if (!groupBy.Contains("Funcionario.FUN_NOME"))
                            SetarJoinsVendedor(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                where.Append($" and Pessoa.CodigoGrupoPessoa = {filtrosPesquisa.CodigoGrupoPessoa}");

            if (filtrosPesquisa.CodigoLocalidade > 0)
                where.Append($" and Pessoa.CodigoLocalidade = {filtrosPesquisa.CodigoLocalidade}");

            if (filtrosPesquisa.Estado.Count > 0)
            {
                where.Append($" and Estado.UF_SIGLA in ({string.Join(",", filtrosPesquisa.Estado.Select(x => "'"+ x + "'"))})");

                SetarJoinsEstado(joins);
            }

            if (filtrosPesquisa.CodigoAtividade > 0)
                where.Append($" and Pessoa.CodigoAtividade = {filtrosPesquisa.CodigoAtividade}");

            if (filtrosPesquisa.Situacao.HasValue)
            {
                if (filtrosPesquisa.Situacao.Value)
                    where.Append($" and Pessoa.Ativo = 1");
                else
                    where.Append($" and (Pessoa.Ativo = 0 or Pessoa.Ativo IS NULL)");
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and CAST(Pessoa.DataCadastro AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and CAST(Pessoa.DataCadastro AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'");

            if (filtrosPesquisa.TipoPessoa != TipoPessoaCadastro.Todas)
            {
                if (filtrosPesquisa.TipoPessoa == TipoPessoaCadastro.Fisica)
                    where.Append($" and Pessoa.TipoPessoa = 'F'");
                else if (filtrosPesquisa.TipoPessoa == TipoPessoaCadastro.Juridica)
                    where.Append($" and Pessoa.TipoPessoa = 'J'");
                else if (filtrosPesquisa.TipoPessoa == TipoPessoaCadastro.Exterior)
                    where.Append($" and Pessoa.TipoPessoa = 'E'");
            }

            if (filtrosPesquisa.ModalidadePessoa.Count > 0)
            {
                if (filtrosPesquisa.ModalidadePessoa.Contains(TipoModalidade.Cliente))
                    where.Append($" and ( Pessoa.CNPJCPF in (select CPF_CNPJ from T_CLIENTE_MODALIDADE where MOD_TIPO in ({string.Join(",", filtrosPesquisa.ModalidadePessoa.Select(x => x.GetHashCode()))})) or Pessoa.CNPJCPF not in (select CPF_CNPJ from T_CLIENTE_MODALIDADE) )"); // SQL-INJECTION-SAFE

                else
                    where.Append($" and Pessoa.CNPJCPF in (select CPF_CNPJ from T_CLIENTE_MODALIDADE where MOD_TIPO in ({string.Join(",", filtrosPesquisa.ModalidadePessoa.Select(x => x.GetHashCode()))}))");
            }


            if (filtrosPesquisa.SomenteSemCodigoIntegracao.HasValue && filtrosPesquisa.SomenteSemCodigoIntegracao.Value)
                where.Append($" and (Pessoa.CodigoIntegracao = '' or Pessoa.CodigoIntegracao IS NULL)");
            
            if (filtrosPesquisa.ExibeSomenteComCodigoIntegracao.HasValue && filtrosPesquisa.ExibeSomenteComCodigoIntegracao.Value)
            {
                where.Append(" and (Pessoa.CodigoIntegracao <> '' and Pessoa.CodigoIntegracao IS NOT NULL)");
             }

            if (filtrosPesquisa.Bloqueado != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                if (filtrosPesquisa.Bloqueado == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    where.Append($" and GrupoPessoas.GRP_BLOQUEADO = 1");
                else
                    where.Append($" and (GrupoPessoas.GRP_BLOQUEADO = 0 or GrupoPessoas.GRP_BLOQUEADO IS NULL)");

                SetarJoinsGrupoPessoas(joins);
            }

            if (filtrosPesquisa.AguardandoConferenciaInformacao != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                if (filtrosPesquisa.AguardandoConferenciaInformacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    where.Append($" and Pessoa.AguardandoConferenciaInformacao = 1");
                else
                    where.Append($" and (Pessoa.AguardandoConferenciaInformacao = 0 or Pessoa.AguardandoConferenciaInformacao IS NULL)");
            }

            if (filtrosPesquisa.ComGeolocalizacao != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
            {
                if (filtrosPesquisa.ComGeolocalizacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                    where.Append($" and Pessoa.Latitude IS NOT NULL and Pessoa.Longitude IS NOT NULL ");
                else
                    where.Append($" and Pessoa.Latitude IS NULL and Pessoa.Longitude IS NULL ");
            }

            if (filtrosPesquisa.SomenteSemContaContabil.HasValue && filtrosPesquisa.SomenteSemContaContabil.Value)
                where.Append($" and (Pessoa.ContaContabil = '' or Pessoa.ContaContabil IS NULL)");

            if (filtrosPesquisa.CodigoCategoria > 0)
                where.Append($" and Pessoa.CodigoCategoria = {filtrosPesquisa.CodigoCategoria}");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and Pessoa.CNPJCPF in (select CLI_CGCCPF from T_CLIENTE_DADOS where EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa})");
        }

        #endregion
    }
}
