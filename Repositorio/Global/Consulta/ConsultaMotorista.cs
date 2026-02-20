using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaMotorista : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista>
    {
        #region Construtores

        public ConsultaMotorista() : base(tabela: "T_FUNCIONARIO as Motorista ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            if (!joins.Contains(" Localidade "))
                joins.Append(" left join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Motorista.LOC_CODIGO ");
        }
        private void SetarJoinsLocalidadeNascimento(StringBuilder joins)
        {
            if (!joins.Contains(" LocalidadeNascimento "))
                joins.Append(" left join T_LOCALIDADES LocalidadeNascimento on LocalidadeNascimento.LOC_CODIGO = Motorista.LOC_CODIGO_NASCIMENTO ");
        }
        private void SetarJoinsLocalidadeCNH(StringBuilder joins)
        {
            if (!joins.Contains(" Localidade_CNH "))
                joins.Append(" left join T_LOCALIDADES Localidade_CNH on Localidade_CNH.LOC_CODIGO = Motorista.LOC_CODIGO_CNH ");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            if (!joins.Contains(" GestorTabela "))
                joins.Append(" left join T_FUNCIONARIO GestorTabela on GestorTabela.FUN_CODIGO = Motorista.FUN_CODIGO_GESTOR ");
        }

        private void SetarJoinsBanco(StringBuilder joins)
        {
            if (!joins.Contains(" Banco "))
                joins.Append(" left join T_BANCO Banco on Banco.BCO_CODIGO = Motorista.BCO_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Motorista.EMP_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Motorista.FIL_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" left join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = Motorista.CRE_CODIGO ");
        }

        private void SetarJoinsCargo(StringBuilder joins)
        {
            if (!joins.Contains(" Cargo "))
                joins.Append(" left join T_CARGO_PESSOA Cargo on Cargo.CRG_CODIGO = Motorista.CRG_CODIGO ");
        }

        private void SetarJoinsClienteAgregado(StringBuilder joins)
        {
            if (!joins.Contains(" PessoaAgregado "))
                joins.Append(" left join T_CLIENTE PessoaAgregado on PessoaAgregado.CLI_CGCCPF = Motorista.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Motorista.FUN_CODIGO Codigo, ");
                        groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "Nome":
                    if (!select.Contains(" Nome, "))
                    {
                        select.Append("Motorista.FUN_NOME Nome, ");
                        groupBy.Append("Motorista.FUN_NOME, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("Motorista.FUN_CODIGO_INTEGRACAO CodigoIntegracao, ");
                        groupBy.Append("Motorista.FUN_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "CPF":
                    if (!select.Contains(" CPF, "))
                    {
                        select.Append("Motorista.FUN_CPF CPF, ");
                        groupBy.Append("Motorista.FUN_CPF, ");
                    }
                    break;

                case "RG":
                    if (!select.Contains(" RG, "))
                    {
                        select.Append("Motorista.FUN_RG RG, ");
                        groupBy.Append("Motorista.FUN_RG, ");
                    }
                    break;

                case "CategoriaCNH":
                    if (!select.Contains(" CategoriaCNH, "))
                    {
                        select.Append("Motorista.FUN_CATEGORIA CategoriaCNH, ");
                        groupBy.Append("Motorista.FUN_CATEGORIA, ");
                    }
                    break;

                case "EmissorRGDescricao":
                    if (!select.Contains(" EmissorRG, "))
                    {
                        select.Append("Motorista.FUN_ORGAO_EMISSOR_RG EmissorRG, ");
                        groupBy.Append("Motorista.FUN_ORGAO_EMISSOR_RG, ");
                    }
                    break;

                case "DataEmissaoRGFormatada":
                    if (!select.Contains(" DataEmissaoRG, "))
                    {
                        select.Append("Motorista.FUN_DATA_EMISSAO_RG DataEmissaoRG, ");
                        groupBy.Append("Motorista.FUN_DATA_EMISSAO_RG, ");
                    }
                    break;

                case "Celular":
                    if (!select.Contains(" Celular, "))
                    {
                        select.Append("Motorista.FUN_CELULAR Celular, ");
                        groupBy.Append("Motorista.FUN_CELULAR, ");
                    }
                    break;

                case "PISPASEP":
                    if (!select.Contains(" PISPASEP, "))
                    {
                        select.Append("Motorista.FUN_PIS PISPASEP, ");
                        groupBy.Append("Motorista.FUN_PIS, ");
                    }
                    break;

                case "DataAdmissaoFormatada":
                    if (!select.Contains(" DataAdmissao, "))
                    {
                        select.Append("Motorista.FUN_DATAADMISAO DataAdmissao, ");
                        groupBy.Append("Motorista.FUN_DATAADMISAO, ");
                    }
                    break;

                case "DataNascimentoFormatada":
                    if (!select.Contains(" DataNascimento, "))
                    {
                        select.Append("Motorista.FUN_DATANASC DataNascimento, ");
                        groupBy.Append("Motorista.FUN_DATANASC, ");
                    }
                    break;

                case "CNH":
                    if (!select.Contains(" CNH, "))
                    {
                        select.Append("Motorista.FUN_NUMHABILITACAO CNH, ");
                        groupBy.Append("Motorista.FUN_NUMHABILITACAO, ");
                    }
                    break;

                case "RenachCNH":
                    if (!select.Contains(" RenachCNH, "))
                    {
                        select.Append("Motorista.FUN_RENACH RenachCNH, ");
                        groupBy.Append("Motorista.FUN_RENACH, ");
                    }
                    break;

                case "DataValidadeCNHFormatada":
                    if (!select.Contains(" ValidadeCNH, "))
                    {
                        select.Append("Motorista.FUN_VECTOHABILITACAO ValidadeCNH, ");
                        groupBy.Append("Motorista.FUN_VECTOHABILITACAO, ");
                    }
                    break;

                case "DataValidadeSeguradoraFormatada":
                    if (!select.Contains(" ValidadeSeguradora, "))
                    {
                        select.Append("Motorista.FUN_DATA_VALIDADE_LIBERACAO_SEGURADORA ValidadeSeguradora, ");
                        groupBy.Append("Motorista.FUN_DATA_VALIDADE_LIBERACAO_SEGURADORA, ");
                    }
                    break;

                case "Telefone":
                    if (!select.Contains(" Telefone, "))
                    {
                        select.Append("Motorista.FUN_FONE Telefone, ");
                        groupBy.Append("Motorista.FUN_FONE, ");
                    }
                    break;

                case "Endereco":
                    if (!select.Contains(" Endereco, "))
                    {
                        select.Append("Motorista.FUN_ENDERECO Endereco, ");
                        groupBy.Append("Motorista.FUN_ENDERECO, ");
                    }
                    break;

                case "ComplementoEndereco":
                    if (!select.Contains(" ComplementoEndereco, "))
                    {
                        select.Append("Motorista.FUN_COMPLEMENTO ComplementoEndereco, ");
                        groupBy.Append("Motorista.FUN_COMPLEMENTO, ");
                    }
                    break;

                case "BloqueadoDescricao":
                    if (!select.Contains(" Bloqueado, "))
                    {
                        select.Append("Motorista.FUN_BLOQUEADO Bloqueado, ");
                        groupBy.Append("Motorista.FUN_BLOQUEADO, ");
                    }
                    break;

                case "MotivoBloqueio":
                    if (!select.Contains(" MotivoBloqueio, "))
                    {
                        select.Append("Motorista.FUN_MOTIVO_BLOQUEIO MotivoBloqueio, ");
                        groupBy.Append("Motorista.FUN_MOTIVO_BLOQUEIO, ");
                    }
                    break;

                case "CEP":
                    if (!select.Contains(" CEP, "))
                    {
                        select.Append("Motorista.FUN_CEP CEP, ");
                        groupBy.Append("Motorista.FUN_CEP, ");
                    }
                    break;

                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        select.Append("Localidade.LOC_DESCRICAO Cidade, ");
                        groupBy.Append("Localidade.LOC_DESCRICAO, ");

                        SetarJoinsLocalidade(joins);
                    }
                    break;

                case "Estado":
                    if (!select.Contains(" Estado, "))
                    {
                        select.Append("Localidade.UF_SIGLA Estado, ");
                        groupBy.Append("Localidade.UF_SIGLA, ");

                        SetarJoinsLocalidade(joins);
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "TipoMotorista":
                    if (!select.Contains(" TipoMotorista, "))
                    {
                        select.Append(@"CASE
	                                        WHEN Motorista.FUN_TIPO_MOTORISTA = 1 THEN 'Próprio'
	                                        WHEN Motorista.FUN_TIPO_MOTORISTA = 2 THEN 'Terceiro'
	                                        ELSE 'Não definido'
                                        END TipoMotorista, ");
                        groupBy.Append("Motorista.FUN_TIPO_MOTORISTA, ");
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append(@"CASE
	                                        WHEN Motorista.FUN_STATUS = 'A' THEN 'Ativo'
	                                        WHEN Motorista.FUN_STATUS = 'I' THEN 'Inativo'
	                                        ELSE 'Não definido'
                                        END Situacao,");

                        groupBy.Append("Motorista.FUN_STATUS, ");
                    }
                    break;

                case "FichaAtiva":
                    if (!select.Contains(" FichaAtiva, "))
                    {
                        select.Append("CASE WHEN Motorista.FUN_ATIVAR_FICHA_MOTORISTA = 1 THEN 'Sim' ELSE 'Não' END FichaAtiva,");
                        groupBy.Append("Motorista.FUN_ATIVAR_FICHA_MOTORISTA, ");
                    }
                    break;

                case "GerarComissao":
                    if (!select.Contains(" GerarComissao, "))
                    {
                        select.Append("CASE WHEN Motorista.FUN_NAO_GERA_COMISSAO_ACERTO = 1 THEN 'Sim' ELSE 'Não' END GerarComissao,");
                        groupBy.Append("Motorista.FUN_NAO_GERA_COMISSAO_ACERTO, ");
                    }
                    break;

                case "UsuarioMobile":
                    if (!select.Contains(" UsuarioMobile, "))
                    {
                        select.Append("CASE WHEN Motorista.FUN_CODIGO_MOBILE > 0 THEN 'Sim' ELSE 'Não' END UsuarioMobile,");
                        groupBy.Append("Motorista.FUN_CODIGO_MOBILE, ");
                    }
                    break;

                case "NaoBloquearAcessoSimultaneo":
                    if (!select.Contains(" NaoBloquearAcessoSimultaneo, "))
                    {
                        select.Append("CASE WHEN Motorista.FUN_NAO_BLOQUEAR_ACESSO_SIMULTANEO = 1 THEN 'Sim' ELSE 'Não' END NaoBloquearAcessoSimultaneo,");
                        groupBy.Append("Motorista.FUN_NAO_BLOQUEAR_ACESSO_SIMULTANEO, ");
                    }
                    break;

                case "Banco":
                    if (!select.Contains(" Banco, "))
                    {
                        select.Append("Banco.BCO_DESCRICAO Banco, ");
                        groupBy.Append("Banco.BCO_DESCRICAO, ");

                        SetarJoinsBanco(joins);
                    }
                    break;

                case "Agencia":
                    if (!select.Contains(" Agencia, "))
                    {
                        select.Append("Motorista.FUN_BANCO_AGENCIA Agencia, ");
                        groupBy.Append("Motorista.FUN_BANCO_AGENCIA, ");
                    }
                    break;

                case "Digito":
                    if (!select.Contains(" Digito, "))
                    {
                        select.Append("Motorista.FUN_BANCO_DIGITO_AGENCIA Digito, ");
                        groupBy.Append("Motorista.FUN_BANCO_DIGITO_AGENCIA, ");
                    }
                    break;

                case "NumeroConta":
                    if (!select.Contains(" NumeroConta, "))
                    {
                        select.Append("Motorista.FUN_BANCO_NUMERO_CONTA NumeroConta, ");
                        groupBy.Append("Motorista.FUN_BANCO_NUMERO_CONTA, ");
                    }
                    break;

                case "NumeroCartao":
                    if (!select.Contains(" NumeroCartao, "))
                    {
                        select.Append("Motorista.FUN_NUMERO_CARTAO NumeroCartao, ");
                        groupBy.Append("Motorista.FUN_NUMERO_CARTAO, ");
                    }
                    break;

                case "TipoContaFormatada":
                    if (!select.Contains(" TipoConta, "))
                    {
                        select.Append("Motorista.FUN_BANCO_TIPO_CONTA TipoConta, ");
                        groupBy.Append("Motorista.FUN_BANCO_TIPO_CONTA, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Transportadora":
                    if (!select.Contains(" Transportadora, "))
                    {
                        select.Append("Empresa.EMP_RAZAO Transportadora, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + v.VEI_PLACA
	                                    FROM T_VEICULO v	                                
	                                    WHERE v.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO FOR XML PATH('')), 3, 1000) Veiculo, ");

                        if (!groupBy.Contains("Motorista.FUN_CODIGO, "))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "NumeroFrota":
                    if (!select.Contains(" NumeroFrota, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + v.VEI_NUMERO_FROTA
	                                    FROM T_VEICULO v	                                
	                                    WHERE v.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO FOR XML PATH('')), 3, 1000) NumeroFrota, ");

                        if (!groupBy.Contains("Motorista.FUN_CODIGO, "))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "SaldoDiaria":
                    if (!select.Contains(" SaldoDiaria, "))
                    {
                        select.Append("Motorista.FUN_SALDO_DIARIA SaldoDiaria, ");
                        groupBy.Append("Motorista.FUN_SALDO_DIARIA, ");
                    }
                    break;

                case "SaldoAdiantamento":
                    if (!select.Contains(" SaldoAdiantamento, "))
                    {
                        select.Append("Motorista.FUN_SALDO_ADIANTAMENTO SaldoAdiantamento, ");
                        groupBy.Append("Motorista.FUN_SALDO_ADIANTAMENTO, ");
                    }
                    break;

                case "DiasTrabalhado":
                    if (!select.Contains(" DiasTrabalhado, "))
                    {
                        select.Append("Motorista.FUN_DIAS_TRABALHADO DiasTrabalhado, ");
                        groupBy.Append("Motorista.FUN_DIAS_TRABALHADO, ");
                    }
                    break;

                case "DiasFolgaRetirado":
                    if (!select.Contains(" DiasFolgaRetirado, "))
                    {
                        select.Append("Motorista.FUN_DIAS_FOLGA_RETIRADO DiasFolgaRetirado, ");
                        groupBy.Append("Motorista.FUN_DIAS_FOLGA_RETIRADO, ");
                    }
                    break;

                case "DataEmissaoCNHFormatada":
                    if (!select.Contains(" DataEmissaoCNH, "))
                    {
                        select.Append("Motorista.FUN_DATAHABILITACAO DataEmissaoCNH, ");
                        groupBy.Append("Motorista.FUN_DATAHABILITACAO, ");
                    }
                    break;

                case "NumeroProntuario":
                    if (!select.Contains(" NumeroProntuario, "))
                    {
                        select.Append("Motorista.FUN_NUMERO_PRONTUARIO NumeroProntuario, ");
                        groupBy.Append("Motorista.FUN_NUMERO_PRONTUARIO, ");
                    }
                    break;

                case "Cargo":
                    if (!select.Contains(" Cargo, "))
                    {
                        select.Append("Motorista.FUN_CARGO Cargo, ");
                        groupBy.Append("Motorista.FUN_CARGO, ");
                    }
                    break;

                case "AposentadoriaFormatada":
                    if (!select.Contains(" Aposentado, "))
                    {
                        select.Append("Motorista.FUN_APOSENTADORIA Aposentado, ");
                        groupBy.Append("Motorista.FUN_APOSENTADORIA, ");
                    }
                    break;

                case "CargoMotorista":
                    if (!select.Contains(" CargoMotorista, "))
                    {
                        select.Append("Cargo.CRG_DESCRICAO CargoMotorista, ");
                        groupBy.Append("Cargo.CRG_DESCRICAO, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "CNPJTransportadorFormatado":
                    if (!select.Contains("CNPJTransportador, "))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;


                case "EstadoRG":
                    if (!select.Contains(" EstadoRG, "))
                    {
                        select.Append("Motorista.UF_RG EstadoRG, ");
                        groupBy.Append("Motorista.UF_RG, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "NumRegistroCNH":
                    if (!select.Contains(" NumRegistroCNH, "))
                    {
                        select.Append("Motorista.FUN_NUMERO_REGISTRO_HABILITACAO NumRegistroCNH, ");
                        groupBy.Append("Motorista.FUN_NUMERO_REGISTRO_HABILITACAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "DataPrimeiraCNHFormatada":
                    if (!select.Contains(" DataPrimeiraCNH, "))
                    {
                        select.Append("Motorista.FUN_DATA_PRIMEIRA_HABILITACAO DataPrimeiraCNH, ");
                        groupBy.Append("Motorista.FUN_DATA_PRIMEIRA_HABILITACAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Email":
                    if (!select.Contains(" Email, "))
                    {
                        select.Append("Motorista.FUN_EMAIL Email, ");
                        groupBy.Append("Motorista.FUN_EMAIL, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Bairro":
                    if (!select.Contains(" Bairro, "))
                    {
                        select.Append("Motorista.FUN_BAIRRO Bairro, ");
                        groupBy.Append("Motorista.FUN_BAIRRO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("Motorista.FUN_NUMERO_ENDERECO Numero, ");
                        groupBy.Append("Motorista.FUN_NUMERO_ENDERECO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "TipoLogradouroFormatada":
                    if (!select.Contains(" TipoLogradouro, "))
                    {
                        select.Append("Motorista.FUN_TIPO_LOGRADOURO TipoLogradouro, ");
                        groupBy.Append("Motorista.FUN_TIPO_LOGRADOURO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;


                case "EstadoCivilFormatada":
                    if (!select.Contains(" EstadoCivil, "))
                    {
                        select.Append("Motorista.FUN_ESTADOCIVIL EstadoCivil, ");
                        groupBy.Append("Motorista.FUN_ESTADOCIVIL, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "CorRacaFormatada":
                    if (!select.Contains(" CorRaca, "))
                    {
                        select.Append("Motorista.FUN_COR_RACA CorRaca, ");
                        groupBy.Append("Motorista.FUN_COR_RACA, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "EscolaridadeFormatada":
                    if (!select.Contains(" Escolaridade, "))
                    {
                        select.Append("Motorista.FUN_ESCOLARIDADE Escolaridade, ");
                        groupBy.Append("Motorista.FUN_ESCOLARIDADE, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "TituloEleitoral":
                    if (!select.Contains(" TituloEleitoral, "))
                    {
                        select.Append("Motorista.FUN_TITULO_ELEITORAL TituloEleitoral, ");
                        groupBy.Append("Motorista.FUN_TITULO_ELEITORAL, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "ZonaEleitoral":
                    if (!select.Contains(" ZonaEleitoral, "))
                    {
                        select.Append("Motorista.FUN_ZONA_ELEITORAL ZonaEleitoral, ");
                        groupBy.Append("Motorista.FUN_ZONA_ELEITORAL, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "SecaoEleitoral":
                    if (!select.Contains(" SecaoEleitoral, "))
                    {
                        select.Append("Motorista.FUN_SECAO_ELEITORAL SecaoEleitoral, ");
                        groupBy.Append("Motorista.FUN_SECAO_ELEITORAL, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "DataExpedicaoCTPSFormatada":
                    if (!select.Contains(" DataExpedicaoCTPS, "))
                    {
                        select.Append("Motorista.FUN_DATA_EXPEDICAO_CTPS DataExpedicaoCTPS, ");
                        groupBy.Append("Motorista.FUN_DATA_EXPEDICAO_CTPS, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "EstadoExpedicaoCTPS":
                    if (!select.Contains(" EstadoExpedicaoCTPS, "))
                    {
                        select.Append("Motorista.UF_CTPS EstadoExpedicaoCTPS, ");
                        groupBy.Append("Motorista.UF_CTPS, ");

                        SetarJoinsCargo(joins);
                    }
                    break;

                case "LocalidadeNascimento":
                    if (!select.Contains(" LocalidadeNascimento, "))
                    {
                        select.Append("LocalidadeNascimento.LOC_DESCRICAO LocalidadeNascimento, ");
                        groupBy.Append("LocalidadeNascimento.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeNascimento(joins);
                    }
                    break;


                case "TipoParentescoFormatada":
                    if (!select.Contains(" TipoParentesco, "))
                    {
                        select.Append(@"(select TOP 1 (select TOP 1 FuncionarioContato.CFO_TIPO_PARENTESCO from T_FUNCIONARIO_CONTATO FuncionarioContato
                                    LEFT JOIN T_FUNCIONARIO Motorista ON FuncionarioContato.FCO_CODIGO = Motorista.FUN_CODIGO
                                    order by FuncionarioContato.FCO_CODIGO ) GrauParentesco from T_FUNCIONARIO Motorista
                                    LEFT JOIN T_FUNCIONARIO_CONTATO FuncionarioContato ON FuncionarioContato.FCO_CODIGO = Motorista.FUN_CODIGO) TipoParentesco, ");

                        groupBy.Append("Motorista.FUN_CODIGO, ");
                        SetarJoinsCargo(joins);
                    }
                    break;

                case "PessoaAgregado":
                    if (!select.Contains(" PessoaAgregado, "))
                    {
                        select.Append("PessoaAgregado.CLI_NOME PessoaAgregado, ");
                        groupBy.Append("PessoaAgregado.CLI_NOME, ");

                        SetarJoinsClienteAgregado(joins);
                    }
                    break;

                case "NumeroCTPS":
                    if (!select.Contains(" NumeroCTPS, "))
                    {
                        select.Append("Motorista.FUN_NUMERO_CTPS NumeroCTPS, ");
                        groupBy.Append("Motorista.FUN_NUMERO_CTPS, ");

                        SetarJoinsClienteAgregado(joins);
                    }
                    break;

                case "SerieCTPS":
                    if (!select.Contains(" SerieCTPS, "))
                    {
                        select.Append("Motorista.FUN_SERIE_CTPS SerieCTPS, ");
                        groupBy.Append("Motorista.FUN_SERIE_CTPS, ");
                    }
                    break;


                case "DadosBancarios":
                    if (!select.Contains(" DadosBancarios, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CONCAT('Banco: ' + ISNULL(Banco.BCO_DESCRICAO ,''), ', Ag.: ' + ISNULL(DadosBanco.MDB_DIGITO_AGENCIA ,''), ', Dígito: ' + ISNULL(DadosBanco.MDB_DIGITO_AGENCIA ,''), ', Nº Conta: ' + ISNULL(DadosBanco.MDB_NUMERO_CONTA ,''), ', Tipo: ' + (CASE WHEN DadosBanco.MDB_TIPO_CONTA = 0 THEN 'Nenhum' WHEN DadosBanco.MDB_TIPO_CONTA = 1 THEN 'Corrente' WHEN DadosBanco.MDB_TIPO_CONTA = 2 THEN 'Poupança' ELSE 'Salário' END), ', Obs: ' + ISNULL(DadosBanco.MDB_OBSERVACAO_CONTA ,''))
                                        FROM T_MOTORISTA_DADO_BANCARIO DadosBanco
                                        inner join T_BANCO Banco ON Banco.BCO_CODIGO = DadosBanco.BCO_CODIGO
                                        WHERE DadosBanco.FUN_CODIGO = Motorista.FUN_CODIGO FOR XML PATH('')), 3, 10000) DadosBancarios, ");

                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");

                    }
                    break;

                case "ContatoNenhum":
                    if (!select.Contains(" ContatoNenhum, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Nenhum'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 0) ContatoNenhum, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoOutro":
                    if (!select.Contains(" ContatoOutro, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Outro'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 1) ContatoOutro, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoPai":
                    if (!select.Contains(" ContatoPai, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Pai'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 2) ContatoPai, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoMae":
                    if (!select.Contains(" ContatoMae, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Mae'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 3) ContatoMae,");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoFilhos":
                    if (!select.Contains(" ContatoFilhos, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Filhos'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 4) ContatoFilhos, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoIrmao":
                    if (!select.Contains(" ContatoIrmao, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Irmao'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 5) ContatoIrmao, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoAvo":
                    if (!select.Contains(" ContatoAvo, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Nenhum'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 0) ContatoAvo, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoTio":
                    if (!select.Contains(" ContatoTio, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Tio'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 8) ContatoTio, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoSobrinho":
                    if (!select.Contains(" ContatoSobrinho, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Sobrinho'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 9) ContatoSobrinho, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoBisavo":
                    if (!select.Contains(" ContatoBisavo, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Bisavo'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 10) ContatoBisavo, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoBisneto":
                    if (!select.Contains(" ContatoBisneto, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Bisneto'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 11) ContatoBisneto, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoPrimo":
                    if (!select.Contains(" ContatoPrimo, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Primo'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 12) ContatoPrimo, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoTrisavo":
                    if (!select.Contains(" ContatoTrisavo, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Trisavo'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 13) ContatoTrisavo, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoTrineto":
                    if (!select.Contains(" ContatoTrineto, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Trineto'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 14) ContatoTrineto, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoTipoAvo":
                    if (!select.Contains(" ContatoTipoAvo, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'TioAvo'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 15) ContatoTipoAvo, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoSobrinhoNeto":
                    if (!select.Contains(" ContatoSobrinhoNeto, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'SobrinhoNeto'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 16) ContatoSobrinhoNeto, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "ContatoEsposo":
                    if (!select.Contains(" ContatoEsposo, "))
                    {
                        select.Append("(SELECT TOP(1) ISNULL(CONCAT(C.CFO_NOME, 'Esposo'), '') FROM T_FUNCIONARIO_CONTATO C WHERE C.FUN_CODIGO = Motorista.FUN_CODIGO AND C.CFO_TIPO_PARENTESCO = 17) ContatoEsposo, ");
                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;

                case "CodigoSegurancaCNH":
                    if (!select.Contains(" CodigoSegurancaCNH "))
                    {
                        select.Append("Motorista.FUN_CODIGO_SEGURANCA_CNH CodigoSegurancaCNH, ");
                        groupBy.Append("Motorista.FUN_CODIGO_SEGURANCA_CNH, ");
                    }
                    break;

                case "Gestor":
                    if (!select.Contains(" Gestor, "))
                    {
                        select.Append("GestorTabela.FUN_NOME Gestor, ");

                        if (!groupBy.Contains("GestorTabela.FUN_NOME"))
                            groupBy.Append("GestorTabela.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "LocalidadeMunicipioEstadoCNH":
                    if (!select.Contains(" LocalidadeMunicipioCNH "))
                    {
                        select.Append("Localidade_CNH.LOC_DESCRICAO LocalidadeMunicipioCNH, ");
                        groupBy.Append("Localidade_CNH.LOC_DESCRICAO, ");
                        SetarJoinsLocalidadeCNH(joins);
                    }
                    if (!select.Contains(" LocalidadeEstadoCNH "))
                    {
                        select.Append("Localidade_CNH.UF_SIGLA LocalidadeEstadoCNH, ");
                        groupBy.Append("Localidade_CNH.UF_SIGLA, ");
                        SetarJoinsLocalidadeCNH(joins);
                    }
                    break;

                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("QuantidadeEPI"))
                    {
                        select.Append(@"(select SUM(funcionarioEPI.FEP_QUANTIDADE) from T_FUNCIONARIO_EPI funcionarioEPI
                                    where funcionarioEPI.FUN_CODIGO = Motorista.FUN_CODIGO and funcionarioEPI.EPI_CODIGO = " + codigoDinamico + ") " + propriedade + ", ");

                        if (!groupBy.Contains("Motorista.FUN_CODIGO"))
                            groupBy.Append("Motorista.FUN_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMotorista filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            //string pattern = "yyyy-MM-dd";

            where.Append(" AND Motorista.FUN_TIPO = 'M'");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" AND Motorista.FUN_CODIGO IN (SELECT ISNULL(V.FUN_CODIGO_MOTORISTA, 0) FROM T_VEICULO V WHERE V.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($" AND Motorista.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Nome))
                where.Append($" AND Motorista.FUN_NOME LIKE '%{filtrosPesquisa.Nome}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CPF))
                where.Append($" AND Motorista.FUN_CPF LIKE '{filtrosPesquisa.CPF}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                where.Append($" AND Motorista.FUN_CODIGO_INTEGRACAO LIKE '%{filtrosPesquisa.CodigoIntegracao}%'");

            if (filtrosPesquisa.TipoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos)
                where.Append($" AND Motorista.FUN_TIPO_MOTORISTA = {(int)filtrosPesquisa.TipoMotorista}");

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append(" AND Motorista.FUN_STATUS = 'A'");
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append(" AND Motorista.FUN_STATUS = 'I'");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" AND Motorista.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.CodigoTipoLicenca > 0)
                where.Append($" AND Motorista.FUN_CODIGO IN (SELECT LL.FUN_CODIGO FROM T_MOTORISTA_LICENCA LL WHERE LL.LIC_CODIGO = {filtrosPesquisa.CodigoTipoLicenca})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.SituacaoColaborador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Todos)
                where.Append($" AND Motorista.FUN_SITUACAO_COLABORADOR = {(int)filtrosPesquisa.SituacaoColaborador}");

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                where.Append($" AND Motorista.CRE_CODIGO = {filtrosPesquisa.CodigoCentroResultado}");

            if (filtrosPesquisa.Aposentadoria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Aposentadoria.Todos)
                where.Append(" AND Motorista.FUN_APOSENTADORIA = " + filtrosPesquisa.Aposentadoria.ToString("D"));

            if (filtrosPesquisa.CargoMotorista > 0)
                where.Append($" AND Motorista.CRG_CODIGO = {filtrosPesquisa.CargoMotorista} ");

            if (filtrosPesquisa.Bloqueado.HasValue)
            {
                if (!filtrosPesquisa.Bloqueado.Value)
                    where.Append($" AND (Motorista.FUN_BLOQUEADO = {Convert.ToInt32(filtrosPesquisa.Bloqueado.Value)} OR Motorista.FUN_BLOQUEADO IS NULL)");
                else
                    where.Append($" AND Motorista.FUN_BLOQUEADO = {Convert.ToInt32(filtrosPesquisa.Bloqueado.Value)}");
            }

            if (filtrosPesquisa.UsuarioMobile.HasValue)
            {
                if (!filtrosPesquisa.UsuarioMobile.Value)
                    where.Append($" AND Motorista.FUN_CODIGO_MOBILE = {Convert.ToInt32(filtrosPesquisa.UsuarioMobile.Value)}");
                else
                    where.Append($" AND Motorista.FUN_CODIGO_MOBILE > 0");
            }

            if (filtrosPesquisa.NaoBloquearAcessoSimultaneo.HasValue)
            {
                if (!filtrosPesquisa.NaoBloquearAcessoSimultaneo.Value)
                    where.Append($" AND Motorista.FUN_NAO_BLOQUEAR_ACESSO_SIMULTANEO = {Convert.ToInt32(filtrosPesquisa.NaoBloquearAcessoSimultaneo.Value)}");
                else
                    where.Append($" AND Motorista.FUN_NAO_BLOQUEAR_ACESSO_SIMULTANEO > 0");
            }

            if (filtrosPesquisa.CodigoGestor > 0)
            {
                where.Append($" AND Motorista.FUN_CODIGO_GESTOR = {filtrosPesquisa.CodigoGestor} ");
            }
        }

        #endregion
    }
}
