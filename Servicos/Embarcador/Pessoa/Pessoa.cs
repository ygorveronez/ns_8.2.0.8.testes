using Newtonsoft.Json;
using System;
using System.Data.Common;
using System.Net;

namespace Servicos.Embarcador.Pessoa
{
    public class Pessoa
    {
        public static Dominio.Entidades.Cliente CriarPessoa(string cnpj, string tipoPessoa, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (cnpj == "00000000000000")
                    return null;
                else if (!Utilidades.Validate.ValidarCNPJ(cnpj))
                    return null;

                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                var json_data = string.Empty;
                using (var w = new WebClient())
                {
                    w.Encoding = System.Text.Encoding.UTF8;
                    json_data = w.DownloadString("https://www.receitaws.com.br/v1/cnpj/" + cnpj);
                }

                if (string.IsNullOrWhiteSpace(json_data))
                    return null;

                var retornoReceitaWS = JsonConvert.DeserializeObject<dynamic>(json_data);

                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = ConverterObjetoRetornoPessoa(retornoReceitaWS);

                if (pessoa != null)
                {
                    pessoa.CPFCNPJ = cnpj;
                    Servicos.Cliente serCliente = new Cliente(unitOfWork.StringConexao);
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(pessoa, tipoPessoa, unitOfWork);
                    if (retorno.Status)
                        return retorno.cliente;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Converter(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa()
            {
                CNPJ = pessoa.CPFCNPJ,
                CodigoIntegracao = pessoa.CodigoIntegracao,
                Emails = pessoa.Email,
                Endereco = pessoa.Endereco,
                IE = pessoa.RGIE,
                InscricaoMunicipal = pessoa.IM,
                NomeFantasia = pessoa.NomeFantasia,
                RazaoSocial = pessoa.RazaoSocial
            };

            return empresa;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Converter(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
            {
                CPFCNPJ = empresa.CNPJ,
                CodigoIntegracao = empresa.CodigoIntegracao,
                Email = empresa.Email,
                Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                {
                    Logradouro = !string.IsNullOrWhiteSpace(empresa.Endereco) && empresa.Endereco.Length >= 80 ? empresa.Endereco.Substring(0, 75) : empresa.Endereco,
                    Bairro = !string.IsNullOrWhiteSpace(empresa.Bairro) && empresa.Bairro.Length >= 40 ? empresa.Bairro.Substring(0, 35) : empresa.Bairro,
                    CEP = empresa.CEP,
                    Cidade = new Dominio.ObjetosDeValor.Localidade()
                    {
                        IBGE = empresa.Localidade.CodigoIBGE
                    },
                    Numero = empresa.Numero,
                    Complemento = !string.IsNullOrWhiteSpace(empresa.Complemento) && empresa.Complemento.Length >= 60 ? empresa.Complemento.Substring(0, 55) : empresa.Complemento,
                    Telefone = empresa.Telefone
                },
                CodigoAtividade = 4,
                RGIE = empresa.InscricaoEstadual,
                TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica,
                IM = empresa.InscricaoMunicipal,
                NomeFantasia = empresa.NomeFantasia,
                RazaoSocial = !string.IsNullOrWhiteSpace(empresa.RazaoSocial) && empresa.RazaoSocial.Length >= 80 ? empresa.RazaoSocial.Substring(0, 75) : empresa.RazaoSocial,
            };

            return pessoa;
        }

        public static Dominio.Entidades.Cliente Converter(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente()
            {
                Atividade = repAtividade.BuscarPrimeiraAtividade(),
                Bairro = empresa.Endereco != null && !string.IsNullOrWhiteSpace(empresa.Endereco.Bairro) && empresa.Endereco.Bairro.Length >= 40 ? empresa.Endereco.Bairro.Substring(0, 35) : empresa.Endereco.Bairro,
                CEP = empresa.Endereco.CEP,
                Cidade = empresa.Endereco.Cidade.Descricao,
                Localidade = repLocalidade.BuscarPorCodigoIBGE(empresa.Endereco.Cidade.IBGE),
                Complemento = !string.IsNullOrWhiteSpace(empresa.Endereco.Complemento) && empresa.Endereco.Complemento.Length >= 60 ? empresa.Endereco.Complemento.Substring(0, 55) : empresa.Endereco.Complemento,
                CPF_CNPJ = double.Parse(empresa.CNPJ),
                DataCadastro = DateTime.Now,
                Email = empresa.Emails,
                IE_RG = empresa.IE,
                EnderecoDigitado = false,
                IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS,
                Nome = !string.IsNullOrWhiteSpace(empresa.RazaoSocial) && empresa.RazaoSocial.Length >= 80 ? empresa.RazaoSocial.Substring(0, 75) : empresa.RazaoSocial,
                Numero = empresa.Endereco.Numero,
                Telefone1 = "",
                Tipo = "J",
                TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros,
                TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros,
                NomeFantasia = !string.IsNullOrWhiteSpace(empresa.RazaoSocial) && empresa.RazaoSocial.Length >= 80 ? empresa.RazaoSocial.Substring(0, 75) : empresa.RazaoSocial,
                EmailStatus = "A",
                Endereco = !string.IsNullOrWhiteSpace(empresa.Endereco.Logradouro) && empresa.Endereco.Logradouro.Length >= 80 ? empresa.Endereco.Logradouro.Substring(0, 75) : empresa.Endereco.Logradouro
            };

            return cliente;
        }

        public static Dominio.Entidades.Cliente Converter(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);

            Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente()
            {
                Atividade = repAtividade.BuscarPrimeiraAtividade(),
                Bairro = !string.IsNullOrWhiteSpace(empresa.Bairro) && empresa.Bairro.Length >= 40 ? empresa.Bairro.Substring(0, 35) : empresa.Bairro,
                CEP = empresa.CEP,
                Cidade = empresa.Localidade.Descricao,
                Localidade = empresa.Localidade,
                Complemento = !string.IsNullOrWhiteSpace(empresa.Complemento) && empresa.Complemento.Length >= 60 ? empresa.Complemento.Substring(0, 55) : empresa.Complemento,
                CPF_CNPJ = double.Parse(empresa.CNPJ_SemFormato),
                DataCadastro = DateTime.Now,
                Email = empresa.Email,
                IE_RG = empresa.InscricaoEstadual,
                EnderecoDigitado = false,
                IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS,
                Nome = !string.IsNullOrWhiteSpace(empresa.RazaoSocial) && empresa.RazaoSocial.Length >= 80 ? empresa.RazaoSocial.Substring(0, 75) : empresa.RazaoSocial,
                Numero = empresa.Numero,
                Telefone1 = empresa.Telefone,
                Tipo = "J",
                TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros,
                TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros,
                NomeFantasia = !string.IsNullOrWhiteSpace(empresa.RazaoSocial) && empresa.RazaoSocial.Length >= 80 ? empresa.RazaoSocial.Substring(0, 75) : empresa.RazaoSocial,
                EmailStatus = "A",
                Endereco = !string.IsNullOrWhiteSpace(empresa.Endereco) && empresa.Endereco.Length >= 80 ? empresa.Endereco.Substring(0, 75) : empresa.Endereco,
                Banco = empresa.Banco,
                TipoContaBanco = empresa.TipoContaBanco,
                Agencia = empresa.Agencia,
                DigitoAgencia = empresa.DigitoAgencia,
                NumeroConta = empresa.NumeroConta
            };
            return cliente;
        }


        private static Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoRetornoPessoa(dynamic retornoReceitaWS)
        {
            if (!string.IsNullOrWhiteSpace((string)retornoReceitaWS.status) && !string.IsNullOrWhiteSpace((string)retornoReceitaWS.message))
            {
                return null;
            }
            else
            {

                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                pessoa.AtualizarEnderecoPessoa = true;
                pessoa.ClienteExterior = false;

                pessoa.CNAE = "";
                if (retornoReceitaWS.atividade_principal.Count > 0)
                    pessoa.CNAE = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.atividade_principal[0].code);

                pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                pessoa.Endereco.Bairro = ((string)retornoReceitaWS.bairro).ToUpper();
                pessoa.Endereco.CEP = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.cep);
                pessoa.Endereco.CEPSemFormato = (string)retornoReceitaWS.cep;

                pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                pessoa.Endereco.Cidade.Descricao = ((string)retornoReceitaWS.municipio).ToUpper();
                pessoa.Endereco.Cidade.SiglaUF = ((string)retornoReceitaWS.uf).ToUpper();

                pessoa.Endereco.Complemento = ((string)retornoReceitaWS.complemento).ToUpper();
                pessoa.Endereco.Logradouro = ((string)retornoReceitaWS.logradouro).ToUpper();
                pessoa.Endereco.Numero = ((string)retornoReceitaWS.numero).ToUpper();

                pessoa.Endereco.Telefone = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.telefone);

                if (pessoa.Endereco.Telefone.Length > 15)
                    pessoa.Endereco.Telefone = "";

                pessoa.NomeFantasia = ((string)retornoReceitaWS.fantasia).ToUpper();
                pessoa.RazaoSocial = ((string)retornoReceitaWS.nome).ToUpper();
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                pessoa.RGIE = "";

                return pessoa;
            }
        }

        public static Dominio.Entidades.Cliente ConverterFuncionario(Dominio.Entidades.Usuario funcionario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (funcionario.Localidade == null)
                return null;

            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);

            Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente()
            {
                Atividade = repAtividade.BuscarPrimeiraAtividade(),
                Bairro = funcionario.Bairro,
                CEP = funcionario.CEP,
                Cidade = funcionario.Localidade?.Descricao ?? "",
                Localidade = funcionario.Localidade,
                Complemento = funcionario.Complemento,
                CPF_CNPJ = double.Parse(funcionario.CPF),
                DataCadastro = DateTime.Now,
                Email = funcionario.Email,
                IE_RG = "ISENTO",
                EnderecoDigitado = false,
                IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte,
                Nome = funcionario.Nome,
                Numero = funcionario.NumeroEndereco,
                Telefone1 = funcionario.Telefone,
                Tipo = funcionario.CPF.Length > 11 ? "J" : "F",
                TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros,
                TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros,
                NomeFantasia = funcionario.Nome,
                EmailStatus = "A",
                Endereco = funcionario.Endereco,
                Ativo = true
            };

            return cliente;
        }

        public static void VerificarCargasEmitidasAnteriormente(double cnpjPessoa, bool alterarTodasCargas, DbConnection connection, DbTransaction transaction)
        {
            VerificarCargasEmitidasAnteriormente(Utilidades.String.OnlyNumbers(cnpjPessoa.ToString("n0")), alterarTodasCargas, connection, transaction);
        }

        public static void VerificarCargasEmitidasAnteriormente(string cnpjPessoa, bool alterarTodasCargas, DbConnection connection, DbTransaction transaction)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = 300;
            command.Transaction = transaction;

            
            string baseQueryTemplate = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE_PARTICIPANTE P
                        JOIN T_CTE C ON C.{0} = P.PCT_CODIGO
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE P.CLI_CODIGO = @ClienteCodigo";

            
            string[] joinConditions = {
                "CON_REMETENTE_CTE",
                "CON_DESTINATARIO_CTE",
                "CON_EXPEDIDOR_CTE",
                "CON_RECEBEDOR_CTE",
                "CON_TOMADOR_CTE"
            };

            
            DbParameter clienteParameter = command.CreateParameter();
            clienteParameter.ParameterName = "@ClienteCodigo";
            clienteParameter.Value = cnpjPessoa;
            command.Parameters.Add(clienteParameter);

            
            foreach (string joinCondition in joinConditions)
            {
                command.CommandText = string.Format(baseQueryTemplate, joinCondition);

                if (!alterarTodasCargas)
                {
                    command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 20)";
                }

                command.ExecuteNonQuery();
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Converter(Dominio.Entidades.Cliente cliente)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
            {
                CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                CodigoIntegracao = cliente.CodigoIntegracao,
                Email = cliente.Email,
                Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                {
                    Logradouro = cliente.Endereco,
                    Bairro = cliente.Bairro,
                    CEP = cliente.CEP,
                    Cidade = new Dominio.ObjetosDeValor.Localidade()
                    {
                        IBGE = cliente.Localidade.CodigoIBGE
                    },
                    Numero = cliente.Numero,
                    Complemento = cliente.Complemento,
                    Telefone = cliente.Telefone1,
                    Telefone2 = cliente.Telefone2
                },
                CodigoAtividade = cliente.Atividade.Codigo,
                RGIE = cliente.IE_RG,
                TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica,
                IM = cliente.InscricaoMunicipal,
                NomeFantasia = cliente.NomeFantasia,
                RazaoSocial = cliente.Nome,
                AtualizarEnderecoPessoa = false
            };

            return pessoa;
        }

    }
}
