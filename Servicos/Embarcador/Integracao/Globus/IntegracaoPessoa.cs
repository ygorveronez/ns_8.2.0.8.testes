using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;
using Newtonsoft.Json;
using System;
using Dominio.ObjetosDeValor.Enumerador;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus;
using Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos;
using System.Text.RegularExpressions;
using Dominio.Excecoes.Embarcador;

namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        #region Métodos Públicos

        public void IntegrarPessoa(Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao pessoaIntegracao)
        {
            var servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            var repositorioPessoaIntegracao = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork);
            var repositorioPessoa = new Repositorio.Cliente(_unitOfWork);

            object pessoaObj = null;
            string jsonRequisicao = "", jsonRetorno = "";

            pessoaIntegracao.DataIntegracao = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(pessoaIntegracao.Pessoa.CodigoIntegracao))
            {
                pessoaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                pessoaIntegracao.ProblemaIntegracao = $"Pessoa já integrada com Globus! Código ({pessoaIntegracao.Pessoa.CodigoIntegracao}) ";

                repositorioPessoaIntegracao.Atualizar(pessoaIntegracao);
                return;
            }

            try
            {
                pessoaIntegracao.NumeroTentativas++;
                if (_configuracaoIntegracao.ShortCodeParticipante == 0 || String.IsNullOrWhiteSpace(_configuracaoIntegracao.TokenParticipante) || String.IsNullOrWhiteSpace(_configuracaoIntegracao.URLWebServiceParticipante))
                    throw new Exception("Processo abortado, configuração de participantes não encontrada!");

                string token = ObterToken("Autenticacao/GerarToken", _configuracaoIntegracao.URLWebServiceParticipante, _configuracaoIntegracao.ShortCodeParticipante, _configuracaoIntegracao.TokenParticipante, false);
                if (token == null)
                    throw new Exception("Não foi possível realizar a comunicação com Globus");

                foreach (var modalidade in pessoaIntegracao.Pessoa.Modalidades)
                {
                    string metodoEnvio = "";

                    if (modalidade.TipoModalidade == TipoModalidade.Cliente)
                    {
                        pessoaObj = ObterCliente(pessoaIntegracao.Pessoa);
                        metodoEnvio = "Cliente/Inserir";
                    }
                    else if (modalidade.TipoModalidade == TipoModalidade.Fornecedor)
                    {
                        pessoaObj = ObterFornecedor(pessoaIntegracao.Pessoa);
                        metodoEnvio = "Fornecedor/Inserir";
                    }
                    else
                        break;

                    var retWS = this.Transmitir(pessoaObj, metodoEnvio, token, _configuracaoIntegracao.URLWebServiceParticipante);

                    pessoaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                    pessoaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    jsonRequisicao = retWS.jsonRequisicao;
                    jsonRetorno = retWS.jsonRetorno;

                    var successResponse = JsonConvert.DeserializeObject<retornoWebServicePessoa>(jsonRetorno);
                    if (successResponse != null && successResponse.success && successResponse.data.idInterno != null)
                        pessoaIntegracao.Pessoa.CodigoIntegracao = successResponse.data.idInterno;
                    else if (successResponse != null && !successResponse.success && successResponse.errors != null)
                    {
                        var errorMessage = successResponse.errors[0];
                        var idMatch = Regex.Match(errorMessage, @"\(ID:\s*(\d+)\)");
                        if (idMatch.Success)
                            pessoaIntegracao.Pessoa.CodigoIntegracao = idMatch.Groups[1].Value;
                    }
                    else
                        throw new Exception(successResponse?.errors != null ? string.Join("; ", successResponse.errors) : "Erro ao interpretar o retorno do Globus");

                    repositorioPessoa.Atualizar(pessoaIntegracao.Pessoa);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                pessoaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pessoaIntegracao.ProblemaIntegracao = message;
            }
            servicoArquivoTransacao.Adicionar(pessoaIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioPessoaIntegracao.Atualizar(pessoaIntegracao);
        }

        #endregion

        #region Métodos Privados
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Cliente ObterCliente(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Cliente();

            retorno.Email = pessoa?.Email ?? "";

            retorno.RazaoSocial = !string.IsNullOrWhiteSpace(pessoa?.Nome) && pessoa.Nome.Length > 150 ? pessoa.Nome.Substring(0, 150) : pessoa?.Nome ?? "";
            retorno.NomeFantasia = !string.IsNullOrWhiteSpace(pessoa?.NomeFantasia) && pessoa.NomeFantasia.Length > 30 ? pessoa.NomeFantasia.Substring(0, 30) :
                                                                                                                         pessoa?.NomeFantasia ?? "";
            if (string.IsNullOrWhiteSpace(retorno.NomeFantasia) || retorno.NomeFantasia.Length <= 1)
                retorno.NomeFantasia = retorno.RazaoSocial.Length > 30 ? retorno.RazaoSocial.Substring(0, 30) : retorno.RazaoSocial;

            retorno.Documentos = ObterPessoaDocumentos(pessoa);
            retorno.Endereco = ObterPessoaEndereco(pessoa);
            retorno.Telefone = pessoa?.Telefone1 ?? pessoa?.Telefone2 ?? "";
            retorno.ContaPlanoFinanceiro = "";
            retorno.ContasContabeis = "";

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Fornecedor ObterFornecedor(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Fornecedor();

            retorno.Autonomo = ObterPessoaAutonomo(pessoa);
            retorno.Email = pessoa?.Email ?? "";

            retorno.RazaoSocial = !string.IsNullOrWhiteSpace(pessoa?.Nome) && pessoa.Nome.Length > 150 ? pessoa.Nome.Substring(0, 150) : pessoa?.Nome ?? "";
            retorno.NomeFantasia = !string.IsNullOrWhiteSpace(pessoa?.NomeFantasia) && pessoa.NomeFantasia.Length > 30 ? pessoa.NomeFantasia.Substring(0, 30)
                                                                                                                       : pessoa?.NomeFantasia ?? "";
            if (string.IsNullOrWhiteSpace(retorno.NomeFantasia) || retorno.NomeFantasia.Length <= 1)
                retorno.NomeFantasia = retorno.RazaoSocial.Length > 30 ? retorno.RazaoSocial.Substring(0, 30) : retorno.RazaoSocial;

            retorno.Documentos = ObterPessoaDocumentos(pessoa);
            retorno.Endereco = ObterPessoaEndereco(pessoa);
            retorno.Telefone = pessoa?.Telefone1 ?? pessoa?.Telefone2 ?? "";
            retorno.ContaPlanoFinanceiro = "";
            retorno.ContasContabeis = "";


            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Pagamento ObterPagamento(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Pagamento();

            retorno.ContaBancaria = ObterContaBancaria(pessoa);
            retorno.Pix = ObterPix(pessoa);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Pix ObterPix(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Pix();

            retorno.Tipo = pessoa.TipoChavePix.HasValue
                                ? pessoa.TipoChavePix switch
                                {
                                    TipoChavePix.CPFCNPJ => "Cnpj",
                                    TipoChavePix.Email => "Email",
                                    TipoChavePix.Celular => "Telefone",
                                    TipoChavePix.Aleatoria => "Aleatorio",
                                    _ => "Nenhuma"
                                }
                                : "Nenhuma";

            if (pessoa.Tipo == "F" && pessoa?.TipoChavePix == TipoChavePix.CPFCNPJ)
                retorno.Tipo = "Cpf";

            retorno.Chave = pessoa.ChavePix ?? "";


            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.ContaBancaria ObterContaBancaria(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.ContaBancaria();
            var partes = (pessoa?.NumeroConta ?? "0-0").Split('-');

            retorno.Tipo = pessoa.TipoContaBanco == TipoContaBanco.Corrente ? "corrente" : "poupanca";
            retorno.Banco = pessoa?.Banco?.Numero ?? 0;
            retorno.Agencia = int.TryParse(pessoa?.Agencia ?? "0", out int agencia) ? agencia : 0;
            retorno.DigitoAgencia = int.TryParse(pessoa?.DigitoAgencia ?? "0", out int digito) ? digito : 0;
            retorno.Conta = int.TryParse(partes[0], out int conta) ? conta : 0;
            retorno.DigitoConta = int.TryParse(partes.ElementAtOrDefault(1) ?? "0", out int digitoConta) ? digitoConta : 0;

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Autonomo> ObterPessoaAutonomo(Dominio.Entidades.Cliente pessoa)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Autonomo> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Autonomo>();

            var autonomo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Autonomo();
            autonomo.Id = int.TryParse(pessoa?.CodigoIntegracao ?? "0", out int numero) ? numero : 0;
            retorno.Add(autonomo);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Documento> ObterPessoaDocumentos(Dominio.Entidades.Cliente pessoa)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Documento> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Documento>();

            if (pessoa?.Tipo == "F")
            {
                if (!string.IsNullOrEmpty(pessoa.CPF_CNPJ_SemFormato))
                {
                    var cpf = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Documento();
                    cpf.Tipo = "Cpf";
                    cpf.Numero = pessoa.CPF_CNPJ_SemFormato;
                    retorno.Add(cpf);
                }

                if (!string.IsNullOrEmpty(pessoa.RG_Passaporte))
                {
                    var rg = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Documento();
                    rg.Tipo = "Rg";
                    rg.Numero = pessoa.RG_Passaporte;
                    retorno.Add(rg);
                }
            }
            else if (pessoa?.Tipo == "J")
            {
                if (!string.IsNullOrEmpty(pessoa.CPF_CNPJ_SemFormato))
                {
                    var cnpj = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Documento();
                    cnpj.Tipo = "Cnpj";
                    cnpj.Numero = pessoa.CPF_CNPJ_SemFormato;
                    retorno.Add(cnpj);
                }
            }

            if (!string.IsNullOrEmpty(pessoa.IE_RG))
            {
                var ie = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Documento();
                ie.Tipo = "InscricaoEstadual";
                ie.Numero = pessoa.IE_RG;
                retorno.Add(ie);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Endereco ObterPessoaEndereco(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Endereco();

            retorno.Cep = pessoa?.CEP ?? "";
            retorno.NumeroEndereco = int.TryParse(pessoa?.Numero ?? "0", out int numero) ? numero : 0;
            retorno.ComplementoEndereco = (pessoa?.Complemento.Length > 30) ? pessoa?.Complemento.Substring(0, 30) : pessoa?.Complemento;

            return retorno;
        }

        #endregion
    }
}