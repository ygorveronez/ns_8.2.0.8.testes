using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.KMM;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Servicos.ServicoMDFe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Métodos Públicos
        public void IntegrarPessoaMotorista(Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao pessoaIntegracao)
        {
            Repositorio.Embarcador.Pessoas.PessoaIntegracao repositorioPessoaIntegracao = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            pessoaIntegracao.DataIntegracao = DateTime.Now;
            pessoaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.Entidades.Cliente pessoa = null;
                Dominio.Entidades.Usuario motorista = null;

                if (pessoaIntegracao != null)
                {
                    pessoa = pessoaIntegracao.Pessoa;
                    motorista = repMotorista.BuscarMotoristasPorCPF(pessoaIntegracao.Pessoa.CPF_CNPJ.ToString()).FirstOrDefault() ?? null;

                }

                string codigoExternoPessoa = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(pessoa?.CPF_CNPJ.ToString() ?? motorista.CPF, TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                if (string.IsNullOrEmpty(codigoExternoPessoa))
                {
                    codigoExternoPessoa = this.ObterCodigoPessoa(configuracaoIntegracaoKMM, pessoa, motorista);
                }

                string codigoExternoEndereco = this.ObterCodigoEndereco(configuracaoIntegracaoKMM, pessoa, motorista, codigoExternoPessoa);

                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa objetoPessoa = ObterPessoa(pessoa, motorista, configuracaoIntegracaoKMM, codigoExternoPessoa, codigoExternoEndereco);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "manipulaPessoa" },
                    { "parameters", objetoPessoa }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                this.ValidarCodigoErroRetorno(ref retWS, pessoa, motorista, configuracaoIntegracaoKMM);

                this.PrencherCodigoExternoRetorno(retWS, pessoa?.CPF_CNPJ.ToString() ?? motorista?.CPF ?? "");

                pessoaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                pessoaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pessoaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                pessoaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(pessoaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioPessoaIntegracao.Atualizar(pessoaIntegracao);
        }
        public void IntegrarPessoaMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            motoristaIntegracao.DataIntegracao = DateTime.Now;
            motoristaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.Entidades.Cliente pessoa = null;
                Dominio.Entidades.Usuario motorista = null;

                if (motoristaIntegracao != null)
                {
                    motorista = motoristaIntegracao.Motorista;
                    pessoa = repPessoa.BuscarPorCPFCNPJ(Convert.ToDouble(motoristaIntegracao.Motorista.CPF ?? "0"));
                }

                string codigoExternoPessoa = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(pessoa?.CPF_CNPJ.ToString() ?? motorista.CPF, TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                if (string.IsNullOrEmpty(codigoExternoPessoa))
                {
                    codigoExternoPessoa = this.ObterCodigoPessoa(configuracaoIntegracaoKMM, pessoa, motorista);
                }

                string codigoExternoEndereco = this.ObterCodigoEndereco(configuracaoIntegracaoKMM, pessoa, motorista, codigoExternoPessoa);

                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa objetoPessoa = ObterPessoa(pessoa, motorista, configuracaoIntegracaoKMM, codigoExternoPessoa, codigoExternoEndereco);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "manipulaPessoa" },
                    { "parameters", objetoPessoa }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                this.ValidarCodigoErroRetorno(ref retWS, pessoa, motorista, configuracaoIntegracaoKMM);

                this.PrencherCodigoExternoRetorno(retWS, pessoa?.CPF_CNPJ.ToString() ?? motorista?.CPF ?? "");

                motoristaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                motoristaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                motoristaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(motoristaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioMotoristaIntegracao.Atualizar(motoristaIntegracao);
        }
        #endregion

        #region Métodos Privados

        private void ValidarCodigoErroRetorno(ref Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM)
        {
            if(retWS.CodigoErro > 0)
            {
                List<int> codigoErroLimparCodigosExternos = new List<int> { 00002, 00003, 00004, 00005, 00006, 00007 };

                int erro = retWS.CodigoErro;

                if (codigoErroLimparCodigosExternos.Any(x => x == erro))
                {
                    Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);
                    var integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(pessoa?.CPF_CNPJ.ToString() ?? motorista.CPF, TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM) ?? null;

                    if (integracaoCodigoExterno != null)
                        repIntegracaoCodigoExterno.Deletar(integracaoCodigoExterno);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa objetoPessoa = ObterPessoa(pessoa, motorista, configuracaoIntegracaoKMM, null, null);

                    Hashtable request = new Hashtable
                    {
                        { "module", "M1076" },
                        { "operation", "manipulaPessoa" },
                        { "parameters", objetoPessoa }
                    };

                    retWS = this.Transmitir(configuracaoIntegracaoKMM, request);
                }
            }

        }
        private void PrencherCodigoExternoRetorno(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, string CPFCNPJ)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPessoa retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPessoa>(retWS.jsonRetorno);
                var campos = retorno?.Result?.Params;
                if (campos != null)
                {
                    if(campos.CodPessoa > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(CPFCNPJ, TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM);
                        if(integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.Pessoa;
                            integracaoCodigoExterno.CPF_CNPJ = CPFCNPJ;
                            integracaoCodigoExterno.CodigoExterno = campos.CodPessoa.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);
                        }
                    }

                    if ((campos.Enderecos.FirstOrDefault()?.CodEndereco ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(CPFCNPJ, TipoCodigoExternoIntegracao.PessoaEndereco, TipoIntegracao.KMM);

                        if(integracaoCodigoExterno == null)
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();

                        integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.PessoaEndereco;
                        integracaoCodigoExterno.CPF_CNPJ = CPFCNPJ;
                        integracaoCodigoExterno.CodigoExterno = campos.Enderecos.FirstOrDefault()?.CodEndereco.ToString();
                        integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;

                        if (integracaoCodigoExterno.Codigo == 0)
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);
                        else
                            repIntegracaoCodigoExterno.Atualizar(integracaoCodigoExterno);

                    }
                }
            }
        }

        private string PrencherCodigoGetCodPessoa(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, string CPFCNPJ)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPessoa retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPessoa>(retWS.jsonRetorno);
                var campos = retorno?.Result;
                if (campos != null)
                {
                    if ((campos.CodPessoa ?? 0 ) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(CPFCNPJ, TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.Pessoa;
                            integracaoCodigoExterno.CPF_CNPJ = CPFCNPJ;
                            integracaoCodigoExterno.CodigoExterno = campos.CodPessoa.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);

                            return integracaoCodigoExterno.CodigoExterno;
                        }
                    }
                }
            }

            return "";
        }

        private string PrencherCodigoGetCodEndereco(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, string CPFCNPJ)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPessoa retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPessoa>(retWS.jsonRetorno);
                var campos = retorno?.Result;
                if (campos != null)
                {
                    if ((campos.CodEndereco ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(CPFCNPJ, TipoCodigoExternoIntegracao.PessoaEndereco, TipoIntegracao.KMM);

                        if(integracaoCodigoExterno == null)
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();

                        integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.PessoaEndereco;
                        integracaoCodigoExterno.CPF_CNPJ = CPFCNPJ;
                        integracaoCodigoExterno.CodigoExterno = campos.CodEndereco.ToString();
                        integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;

                        if (integracaoCodigoExterno.Codigo == 0 )
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);
                        else
                            repIntegracaoCodigoExterno.Atualizar(integracaoCodigoExterno);


                        return integracaoCodigoExterno.CodigoExterno;
                    }
                }
            }

            return "";
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa ObterPessoa(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, string codigoPessoaExterno, string codigoExternoEndereco)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa();

            retorno.Operation = "UPDATE";

            // TIPO: PF: Pessoa Fisica; PJ: Pessoa Juridica; PE: Pessoa Extrangeira;
            if (pessoa != null)
            {
                retorno.CodPessoa = string.IsNullOrEmpty(codigoPessoaExterno) ? null : codigoPessoaExterno;
                retorno.Tipo = pessoa?.Tipo == "E" ? "PE" : pessoa?.Tipo == "J" ? "PJ" : "PF";
                retorno.InscricaoEstadual = string.IsNullOrEmpty(pessoa.IE_RG) ? "ISENTO" : pessoa.IE_RG;
                retorno.CNAE = pessoa?.Tipo == "J" ? "4930202" : null;
                retorno.CodContabil = null;
                retorno.DataCadastro = pessoa.DataCadastro?.ToString("yyyy-MM-dd");
                retorno.Observacoes = pessoa.Observacao;
                retorno.Modalidades = this.ObterPessoaModalidade(pessoa, motorista);

                if (pessoa?.Tipo == "J")
                {
                    retorno.PessoaJuridica = this.ObterPessoaPessoaJuridica(pessoa);
                    retorno.IdExterno = pessoa.CPF_CNPJ.ToString().PadLeft(14, '0');
                }
                else if (pessoa?.Tipo == "F")
                {
                    retorno.PessoaFisica = this.ObterPessoaPessoaFisica(pessoa, motorista);
                    retorno.IdExterno = pessoa.CPF_CNPJ.ToString().PadLeft(11, '0');
                }
                else if (pessoa?.Tipo == "E")
                {
                    retorno.PessoaEstrangeira = this.ObterPessoaPessoaEstrangeira(pessoa);
                    retorno.IdExterno = pessoa.Nome;
                }
            }
            else
            {
                retorno.DataCadastro = motorista.DataAdmissao?.ToString("yyyy-MM-dd") ?? motorista.DataCadastro?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
                retorno.CodPessoa = string.IsNullOrEmpty(codigoPessoaExterno) ? null : codigoPessoaExterno;
                retorno.Tipo = motorista.Tipo == "J" ? "PJ" : "PF";
                retorno.InscricaoEstadual = "ISENTO";
                retorno.IdExterno = motorista.CPF.ToString().PadLeft(11, '0');
                retorno.CNAE = null;
                retorno.CodContabil = null;
                retorno.Observacoes = motorista.Observacao;
                retorno.Modalidades = this.ObterPessoaModalidade(pessoa, motorista);

                retorno.PessoaFisica = this.ObterPessoaPessoaFisica(pessoa, motorista);
            }

            retorno.Documentos = this.ObterPessoaDocumento(pessoa, motorista);
            retorno.RNTRC = this.ObterPessoaRNTRC(pessoa);
            retorno.Enderecos = this.ObterPessoaEnderecos(pessoa, motorista, codigoExternoEndereco);
            retorno.Telefones = this.ObterPessoaTelefones(pessoa, motorista);
            retorno.Emails = this.ObterPessoaEmails(pessoa, motorista);
            retorno.ContasBancarias = ObterPessoaContasBancarias(pessoa, motorista);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa> ObterPessoaModalidade(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa> retorno = null;

            if (pessoa != null)
            {
                //NumModalidade: 5 - Cliente/Fornecedor; 6 - Motorista; 7 - Proprietário de Veículo Terceiro;
                bool bCliente = false;
                if (pessoa.Modalidades.Where(x => x.TipoModalidade == TipoModalidade.Cliente).FirstOrDefault() != null)
                    bCliente = true;

                bool bFornecedor = false;
                if (pessoa.Modalidades.Where(x => x.TipoModalidade == TipoModalidade.Fornecedor).FirstOrDefault() != null)
                    bFornecedor = true;

                bool bTransportadorTerceiro = false;
                if (pessoa.Modalidades.Where(x => x.TipoModalidade == TipoModalidade.TransportadorTerceiro).FirstOrDefault() != null)
                    bTransportadorTerceiro = true;

                if (bCliente || bFornecedor || bTransportadorTerceiro)
                {
                    if (retorno == null)
                        retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa>();

                    if (bCliente)
                    {
                        var pessoafisica = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa();
                        pessoafisica.Operation = "INSERT";
                        pessoafisica.NumModalidade = 13;
                        pessoafisica.Ativo = "true";
                        retorno.Add(pessoafisica);
                    }
                    if (bFornecedor)
                    {
                        var pessoafisica = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa();
                        pessoafisica.Operation = "INSERT";
                        pessoafisica.NumModalidade = 14;
                        pessoafisica.Ativo = "true";
                        retorno.Add(pessoafisica);
                    }

                    if (bTransportadorTerceiro)
                    {
                        var pessoajuridica = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa();
                        pessoajuridica.Operation = "INSERT";
                        pessoajuridica.NumModalidade = 7;
                        pessoajuridica.Ativo = "true";
                        retorno.Add(pessoajuridica);
                    }
                }
            }

            if(motorista != null)
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa>();

                var modalidade = new ModalidadePessoa();
                modalidade.Ativo = "true";
                modalidade.NumModalidade = 8;
                modalidade.Operation = "INSERT";
                retorno.Add(modalidade);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaFisica ObterPessoaPessoaFisica(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaFisica();

            retorno.Operation = "INSERT";

            retorno.CPF = pessoa?.CPF_CNPJ.ToString().PadLeft(11, '0') ?? motorista?.CPF.ToString().PadLeft(11, '0') ?? "" ;
            retorno.Nome = pessoa?.Nome ?? motorista?.Nome ?? "";
            retorno.NomeSocial = pessoa?.Nome ?? motorista?.Nome ?? "";
            retorno.Sexo = (pessoa?.Sexo ?? motorista?.Sexo) == Dominio.ObjetosDeValor.Enumerador.Sexo.Masculino ? "M" : "F";
            retorno.DataNascimento = motorista?.DataNascimento?.ToString("yyyy-MM-dd") ?? pessoa?.DataNascimento?.ToString("yyyy-MM-dd");
            retorno.EstadoCivil = this.ObterEstadoCivilKMM(pessoa?.EstadoCivil ?? motorista?.EstadoCivil);
            retorno.RacaCor = this.ObterRacaCorKMM(null);
            retorno.GrauInstrucao = motorista?.Escolaridade?.ObterDescricao() ?? "";
            retorno.CpfMae = null;
            retorno.NomeMae = motorista?.FiliacaoMotoristaMae;
            retorno.CpfPai = null;
            retorno.NomePai = motorista?.FiliacaoMotoristaPai;

            if (pessoa?.LocalidadeNascimento != null)
            {
                retorno.PaisNacionalidadeESocial = pessoa.LocalidadeNascimento.Pais.Sigla.Substring(0, pessoa.LocalidadeNascimento.Pais.Sigla.Length - 1).ToInt().ToString().PadLeft(3, '0');
                retorno.PaisNascimentoESocial = pessoa.LocalidadeNascimento.Pais.Sigla.Substring(0, pessoa.LocalidadeNascimento.Pais.Sigla.Length - 1).ToInt().ToString().PadLeft(3, '0');
                retorno.Naturalidade = new Naturalidade();
                retorno.Naturalidade.NatMunicipioId = null;
                retorno.Naturalidade.NaturalidadeDesc = pessoa.LocalidadeNascimento.Descricao;
                retorno.Naturalidade.NaturalidadeUF = pessoa.LocalidadeNascimento.Estado.Sigla;
                retorno.Naturalidade.NatMunicipioIBGE = pessoa.LocalidadeNascimento.CodigoIBGE.ToString();
                retorno.Naturalidade.NatPaisId = pessoa.LocalidadeNascimento.Pais.Sigla.ToInt();
            }
            else if(motorista?.LocalidadeNascimento != null)
            {
                retorno.PaisNacionalidadeESocial = motorista.LocalidadeNascimento.Pais.Sigla.Substring(0, motorista.LocalidadeNascimento.Pais.Sigla.Length - 1).ToInt().ToString().PadLeft(3, '0');
                retorno.PaisNascimentoESocial = motorista.LocalidadeNascimento.Pais.Sigla.Substring(0, motorista.LocalidadeNascimento.Pais.Sigla.Length - 1).ToInt().ToString().PadLeft(3, '0');
                retorno.Naturalidade = new Naturalidade();
                retorno.Naturalidade.NatMunicipioId = null;
                retorno.Naturalidade.NaturalidadeDesc = motorista.LocalidadeNascimento.Descricao;
                retorno.Naturalidade.NaturalidadeUF = motorista.LocalidadeNascimento.Estado.Sigla;
                retorno.Naturalidade.NatMunicipioIBGE = motorista.LocalidadeNascimento.CodigoIBGE.ToString();
                retorno.Naturalidade.NatPaisId = motorista.LocalidadeNascimento.Pais.Sigla.ToInt();
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaJuridica ObterPessoaPessoaJuridica(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaJuridica();

            retorno.Operation = "INSERT";
            retorno.CNPJ = pessoa?.CPF_CNPJ.ToString().PadLeft(14, '0');
            retorno.NomeFantasia = pessoa.NomeFantasia;
            retorno.RazaoSocial = pessoa.Nome;
            retorno.InscricaoEstadual = string.IsNullOrEmpty(pessoa.IE_RG) ? "ISENTO" : pessoa.IE_RG;
            retorno.RazaoSocialResumida = pessoa.Nome;
            retorno.AtividadeFiscal = 9;

            //Tipo de Lucro.Obrigatório para pessoa Jurídica: 1 - Lucro Real; 2 - Lucro Presumido; 3 - Simples Nacional;
            switch (pessoa?.RegimeTributario)
            {
                case RegimeTributario.LucroReal:
                    retorno.TipoLucro = 1;
                    break;
                case RegimeTributario.LucroPresumido:
                    retorno.TipoLucro = 2;
                    break;
                case RegimeTributario.SimplesNacional:
                    retorno.TipoLucro = 3;
                    break;
            }

            //Regime tributário: 1 - Simples Nacional; 2 - Simples Nacional - Excesso de receita bruta; 3 - Regime Normal; 4 - MEI;
            if (pessoa?.RegimeTributario == RegimeTributario.LucroReal || pessoa?.RegimeTributario == RegimeTributario.LucroPresumido)
                retorno.RegimeTributario = 3;
            else if (pessoa?.RegimeTributario == RegimeTributario.SimplesNacional)
                retorno.RegimeTributario = 2;

            //retorno.Alvara = ??;
            //retorno.CertificadoOTM = ??;
            //retorno.Suframa = ??;


            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaEstrangeira ObterPessoaPessoaEstrangeira(Dominio.Entidades.Cliente pessoa)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaEstrangeira();

            retorno.nome = pessoa.Nome;
            retorno.nome_fantasia = pessoa.NomeFantasia;
            if (!string.IsNullOrEmpty(pessoa.RG_Passaporte))
            {
                retorno.tipo_documento = "Passaporte";
                retorno.numero = pessoa.RG_Passaporte;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Documento ObterPessoaDocumento(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Documento retorno = null;

            if (pessoa?.Tipo == "F" || motorista != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Documento();
                if (!string.IsNullOrEmpty(pessoa?.PISPASEP))
                {
                    retorno.PisPasepNit = Regex.Replace(pessoa.PISPASEP, @"[^\d]", "");
                }
                else if (!string.IsNullOrEmpty(motorista?.PIS))
                {
                    retorno.PisPasepNit = Regex.Replace(motorista.PIS, @"[^\d]", "");
                    
                }

                if (!string.IsNullOrEmpty(motorista?.RG))
                {
                    var carteiraIdentidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.CarteiraIdentidade();
                    carteiraIdentidade.Numero = motorista.RG;
                    carteiraIdentidade.DataEmissao = motorista.DataEmissaoRG?.ToString("yyyy-MM-dd");
                    carteiraIdentidade.OrgaoEmissor = motorista.OrgaoEmissorRG?.ToString();
                    carteiraIdentidade.UF = motorista.EstadoRG?.Descricao;
                    retorno.CarteiraIdentidade = carteiraIdentidade;
                }
                else if (!string.IsNullOrEmpty(pessoa?.RG_Passaporte))
                {
                    var carteiraIdentidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.CarteiraIdentidade();
                    carteiraIdentidade.Numero = pessoa.RG_Passaporte;
                    carteiraIdentidade.DataEmissao = null;
                    carteiraIdentidade.OrgaoEmissor = pessoa.OrgaoEmissorRG?.ToString();
                    carteiraIdentidade.UF = pessoa.EstadoRG?.Descricao;
                    retorno.CarteiraIdentidade = carteiraIdentidade;
                }

                if (!string.IsNullOrEmpty(pessoa?.TituloEleitoral))
                {
                    var tituloEleitor = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.TituloEleitor();
                    tituloEleitor.Numero = pessoa.TituloEleitoral;
                    //tituloEleitor.DV = ??;
                    tituloEleitor.Secao = pessoa.SecaoEleitoral;
                    tituloEleitor.Zona = pessoa.ZonaEleitoral;

                    retorno.TituloEleitor = tituloEleitor;
                } 
                else if (!string.IsNullOrEmpty(motorista?.TituloEleitoral))
                {
                    var tituloEleitor = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.TituloEleitor();
                    tituloEleitor.Numero = motorista.TituloEleitoral;
                    //tituloEleitor.DV = ??;
                    tituloEleitor.Secao = motorista.SecaoEleitoral;
                    tituloEleitor.Zona = motorista.ZonaEleitoral;
                    retorno.TituloEleitor = tituloEleitor;
                }

                if (!string.IsNullOrEmpty(motorista?.NumeroHabilitacao))
                {
                    var cnh = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.CNH();
                    cnh.NumeroRegistro = motorista.NumeroRegistroHabilitacao;
                    cnh.Numero = motorista.NumeroHabilitacao;
                    cnh.Categoria = motorista.Categoria;
                    cnh.UF = motorista.UFEmissaoCNH?.Sigla;
                    cnh.OrgaoEmissor = motorista.UFEmissaoCNH?.Sigla;
                    cnh.DataEmissao = motorista.DataHabilitacao?.ToString("yyyy-MM-dd");
                    cnh.DataPrimeiraHabilitacao = motorista.DataPrimeiraHabilitacao?.ToString("yyyy-MM-dd");
                    cnh.DataValidade = motorista.DataVencimentoHabilitacao?.ToString("yyyy-MM-dd");
                    cnh.Renach = motorista.RenachHabilitacao;
                    retorno.CNH = cnh;
                }

                if (!string.IsNullOrEmpty(motorista?.NumeroCTPS))
                {
                    var carteiraTrabalho = new CarteiraTrabalho();
                    carteiraTrabalho.Numero = motorista.NumeroCTPS;
                    carteiraTrabalho.Serie = motorista.SerieCTPS;
                    carteiraTrabalho.UF = motorista.EstadoCTPS?.Sigla;
                    retorno.CarteiraTrabalho = carteiraTrabalho;
                }

                retorno.CarteiraReservista = null;
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.EnderecoKMM> ObterPessoaEnderecos(Dominio.Entidades.Cliente pessoa , Dominio.Entidades.Usuario motorista, string codigoExternoEndereco)
        {
            var retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.EnderecoKMM>();

            var endereco = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.EnderecoKMM();
            endereco.Operation = "UPDATE";

            int codEndereco = Convert.ToInt32(string.IsNullOrEmpty(codigoExternoEndereco) ? null : codigoExternoEndereco);

            if (pessoa?.Localidade != null && !string.IsNullOrEmpty(pessoa?.Localidade.CEP))
            {
                endereco.CodEndereco = codEndereco > 0 ? codEndereco : null ;
                endereco.EnderecoPadrao = true;
                endereco.Municipio = pessoa.Localidade?.Descricao;
                endereco.UF = pessoa.Localidade?.Estado.Sigla;
                //endereco.MunicipioId = ??;
                if (pessoa.Localidade?.CodigoIBGE.ToString().IndexOf("9999999") < 0)
                {
                    endereco.MunicipioIBGE = pessoa.Localidade?.CodigoIBGE.ToString();
                }
                else
                {
                    endereco.InsereMunicipioEx = 1;
                }
                if (pessoa.Localidade?.Pais != null)
                {
                    endereco.CodigoBacen = pessoa.Localidade.Pais.Sigla.ToInt();
                }

                if (!string.IsNullOrEmpty(pessoa.CEP))
                {
                    endereco.IdExterno = pessoa.CEP;
                    endereco.CEP = pessoa.CEP;
                }
                else
                {
                    endereco.IdExterno = pessoa.Localidade?.CEP;
                    endereco.CEP = pessoa.Localidade?.CEP;
                }

                endereco.Logradouro = pessoa.Endereco;
                endereco.Numero = pessoa.Numero;
                endereco.Complemento = pessoa.Complemento;
                endereco.Bairro = pessoa.Bairro;
                endereco.Tipo = 1;
                endereco.AtividadeFiscal = 9; //Não Informado
                endereco.InscricaoEstadual = string.IsNullOrEmpty(pessoa.IE_RG) ? "ISENTO" : pessoa.IE_RG;
            }
            else if (motorista?.Localidade != null && !string.IsNullOrEmpty(motorista?.Localidade.CEP))
            {

                if (!string.IsNullOrEmpty(motorista.CEP))
                {
                    endereco.IdExterno = motorista.CEP;
                    endereco.CEP = motorista.CEP;
                }
                else
                {
                    endereco.IdExterno = motorista.Localidade?.CEP;
                    endereco.CEP = motorista.Localidade?.CEP;
                }

                endereco.CodEndereco = codEndereco > 0 ? codEndereco : null;
                endereco.EnderecoPadrao = true;
                endereco.Municipio = motorista.Localidade?.Descricao;
                endereco.UF = motorista.Localidade?.Estado.Sigla;
                //endereco.MunicipioId = ??;
                endereco.MunicipioIBGE = motorista.Localidade?.CodigoIBGE.ToString();
                if (motorista.Localidade?.Pais != null)
                {
                    endereco.CodigoBacen = motorista.Localidade.Pais.Sigla.ToInt();
                }
                endereco.Logradouro = motorista.Endereco;
                endereco.Numero = motorista.NumeroEndereco;
                endereco.Complemento = motorista.Complemento;
                endereco.Bairro = motorista.Bairro;
                endereco.Tipo = 1;
                endereco.AtividadeFiscal = 9; //Não Informado
                endereco.InscricaoEstadual = "ISENTO";
            }
            else
            {
                throw new Exception("Verifique se a pessoa possui localidade vinculada e se a mesma possui um CEP.");
            }

            retorno.Add(endereco);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone> ObterPessoaTelefones(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone> retorno = null;

            if (!string.IsNullOrEmpty(pessoa?.Telefone1))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone>();

                var telefone1 = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone();
                telefone1.Operation = "UPDATE";
                telefone1.TelefonePadrao = true;
                telefone1.Tipo = 1;

                var retObterTelefoneKMM = this.ObterTelefoneKMM(pessoa?.Pais?.Sigla, pessoa.Telefone1);
                telefone1.IdExterno = retObterTelefoneKMM.ToString();
                telefone1.DDI = retObterTelefoneKMM.ddi;
                telefone1.DDD = retObterTelefoneKMM.ddd;
                telefone1.Prefixo = retObterTelefoneKMM.prefixo;
                telefone1.Numero = retObterTelefoneKMM.numero;
                retorno.Add(telefone1);

            }
            else if (!string.IsNullOrEmpty(motorista?.Telefone))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone>();
                var telefone1 = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone();

                telefone1.Operation = "UPDATE";
                telefone1.TelefonePadrao = true;
                telefone1.Tipo = 1;

                var retObterTelefoneKMM = this.ObterTelefoneKMM(motorista?.Localidade?.Pais?.Sigla, motorista.Telefone);
                telefone1.IdExterno = retObterTelefoneKMM.ToString();
                telefone1.DDI = retObterTelefoneKMM.ddi;
                telefone1.DDD = retObterTelefoneKMM.ddd;
                telefone1.Prefixo = retObterTelefoneKMM.prefixo;
                telefone1.Numero = retObterTelefoneKMM.numero;

                retorno.Add(telefone1);
            }

            if (!string.IsNullOrEmpty(pessoa?.Telefone2))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone>();

                var telefone2 = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone();
                telefone2.Operation = "UPDATE";
                telefone2.TelefonePadrao = false;
                telefone2.Tipo = 1;

                var retObterTelefoneKMM = this.ObterTelefoneKMM(pessoa?.Pais?.Sigla, pessoa.Telefone2);
                telefone2.IdExterno = retObterTelefoneKMM.ToString();
                telefone2.DDI = retObterTelefoneKMM.ddi;
                telefone2.DDD = retObterTelefoneKMM.ddd;
                telefone2.Prefixo = retObterTelefoneKMM.prefixo;
                telefone2.Numero = retObterTelefoneKMM.numero;
                retorno.Add(telefone2);
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email> ObterPessoaEmails(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email> retorno = null;
            if (pessoa?.Email != null && !string.IsNullOrEmpty(pessoa?.Email))
            {
                retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email>();

                var email = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email();
                email.Operation = "UPDATE";
                email.IdExterno = pessoa.Email.Length > 200 ? pessoa.Email.Substring(0, 200) : pessoa.Email;
                email.EmailPadrao = true;

                int arroba = pessoa.Email.IndexOf('@');
                if (arroba > -1)
                {
                    email.Username = pessoa.Email.Substring(0, arroba);
                    email.Provedor = pessoa.Email.Substring(arroba + 1);
                }
                else
                {
                    email.Username = pessoa.Email;
                    email.Provedor = pessoa.Email;
                }

                email.Tipo = 1;
                email.Proprietario = pessoa.Nome;

                retorno.Add(email);
            }
            else if (motorista?.Email != null && !string.IsNullOrEmpty(motorista?.Email))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email>();

                var email = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email();
                email.Operation = "UPDATE";
                email.IdExterno = motorista.Email.Length > 200 ? motorista.Email.Substring(0, 200) : motorista.Email;
                //email.EmailId = ??;
                email.EmailPadrao = true;

                int arroba = motorista.Email.IndexOf('@');
                if (arroba > -1)
                {
                    email.Username = motorista.Email.Substring(0, arroba);
                    email.Provedor = motorista.Email.Substring(arroba + 1);
                }
                else
                {
                    email.Username = motorista.Email;
                    email.Provedor = motorista.Email;
                }

                email.Tipo = 1;
                email.Proprietario = motorista.Nome;

                retorno.Add(email);
            }

            if (pessoa?.Emails != null && pessoa?.Emails.Count > 0)
            {
                foreach (var item in pessoa.Emails)
                {
                    if (retorno == null)
                        retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email>();

                    var email = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email();
                    email.Operation = "UPDATE";
                    email.IdExterno = item.Email.Length > 200 ? item.Email.Substring(0, 200) : item.Email;
                    //email.EmailId = ??;
                    email.EmailPadrao = item.TipoEmail == TipoEmail.Principal ? true : false;

                    int arroba = item.Email.IndexOf('@');
                    if (arroba > -1)
                    {
                        email.Username = item.Email.Substring(0, arroba);
                        email.Provedor = item.Email.Substring(arroba + 1);
                    }
                    else 
                    {
                        email.Username = item.Email;
                        email.Provedor = item.Email;
                    }

                    email.Tipo = 1;
                    email.Proprietario = pessoa.Nome;

                    retorno.Add(email);
                }
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria> ObterPessoaContasBancarias(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria> retorno = null;

            if (!string.IsNullOrEmpty(pessoa?.NumeroConta) && pessoa?.Banco != null)
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria>();

                var contaBancaria = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria();

                contaBancaria.Operation = "UPDATE";
                contaBancaria.IdExterno = pessoa.Banco.Numero.ToString() + pessoa.NumeroConta;
                contaBancaria.Padrao = true;
                contaBancaria.Tipo = 1;
                contaBancaria.Banco = pessoa.Banco.Numero.ToString();
                contaBancaria.Agencia = pessoa.Agencia;
                contaBancaria.ContaAgDv = pessoa.DigitoAgencia;
                contaBancaria.Conta = pessoa.NumeroConta.Substring(0, pessoa.NumeroConta.Length - 1);
                contaBancaria.ContaDv = pessoa.NumeroConta.Substring(pessoa.NumeroConta.Length - 1);
                contaBancaria.CodPessoaTitular = null;
                retorno.Add(contaBancaria);
            }
            else if (!string.IsNullOrEmpty(motorista?.NumeroConta) && motorista?.Banco != null)
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria>();

                var contaBancaria = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria();

                contaBancaria.Operation = "UPDATE";
                contaBancaria.IdExterno = motorista.Banco?.Codigo.ToString() + motorista.NumeroConta;
                contaBancaria.Padrao = true;
                contaBancaria.Tipo = 1;
                contaBancaria.Banco = motorista.Banco?.Codigo.ToString();
                contaBancaria.Agencia = motorista.Agencia;
                contaBancaria.ContaAgDv = motorista.DigitoAgencia;
                contaBancaria.Conta = motorista.NumeroConta;
                contaBancaria.CodPessoaTitular = null;
                retorno.Add(contaBancaria);
            }

            return retorno;
        }

        private int ObterEstadoCivilKMM(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil? estadoCivil)
        {
            int retorno = -1;

            switch (estadoCivil)
            {
                case EstadoCivil.Solteiro: retorno = 1; break;
                case EstadoCivil.Casado: retorno = 9; break;
                case EstadoCivil.Divorciado:
                case EstadoCivil.Desquitado:
                    retorno = 7;
                    break;
                case EstadoCivil.Viuvo: retorno = 5; break;
                default: retorno = 1; break;
            }

            return retorno;
        }

        private int ObterRacaCorKMM(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorRaca? corRaca)
        {
            int retorno = -1;

            switch (corRaca)
            {
                case CorRaca.Amarela: retorno = 4; break;
                case CorRaca.Branca: retorno = 1; break;
                case CorRaca.Indigena: retorno = 5; break;
                case CorRaca.Parda: retorno = 3; break;
                case CorRaca.Preta: retorno = 2; break;
                case CorRaca.SemInformacao: retorno = 6; break;
                default: retorno = 6; break;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoObterTelefone ObterTelefoneKMM(string paissigla, string telefoneCadastro)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoObterTelefone();

            string ddd = null;
            string telefone = null;

            Regex buscarDDD = new Regex(@"^\((?<ddd>[0-9]{2})\) ?(?<fone>[0-9-]+)");
            var ret = buscarDDD.Match(telefoneCadastro);
            if (ret.Success)
            {
                ddd = ret.Groups["ddd"].Value;
                telefone = Regex.Replace(ret.Groups["fone"].Value, "[^0-9]", "");
            }
            else
            {
                string telefoneCompleto = Regex.Replace(telefoneCadastro, "[^0-9]", "");

                if (!string.IsNullOrEmpty(telefoneCompleto))
                {
                    if (telefoneCompleto.Length >= 10)
                    {
                        ddd = telefoneCompleto.Substring(0, 2);
                        telefone = telefoneCompleto.Substring(2, (telefoneCompleto.Length - 2));
                    }
                    else
                    {
                        ddd = "42";
                        telefone = telefoneCompleto;
                    }
                }
            }

            if (!string.IsNullOrEmpty(telefone))
            {
                retorno.ddi = paissigla == "BRASIL" ? "55" : null;
                retorno.ddd = ddd;

                if (telefone.Length >= 9)
                {
                    retorno.prefixo = telefone.Substring(0, 5);
                    retorno.numero = telefone.Substring(5, (telefone.Length - 5));
                }
                else if(telefone.Length >= 5)
                {
                    retorno.prefixo = telefone.Substring(0, 4);
                    retorno.numero = telefone.Substring(4, (telefone.Length - 4));
                }
                else
                {
                    retorno.prefixo = null;
                    retorno.numero = telefone;
                }
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RNTRC ObterPessoaRNTRC(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RNTRC retorno = null;

            if (pessoa?.Modalidades != null)
            {
                var transportador = pessoa.Modalidades.Where(x => x.TipoModalidade == TipoModalidade.TransportadorTerceiro).FirstOrDefault();

                if (transportador != null)
                {
                    Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(_unitOfWork);
                    var modTransp = repModalidadeTransportadoraPessoas.BuscarPorModalidade(transportador.Codigo);

                    if (modTransp?.RNTRC != null)
                    {
                        if (retorno == null)
                            retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RNTRC();

                        DateTime dataVencimento = DateTime.Now;
                        if (modTransp.DataVencimentoRNTRC != null)
                        {
                            dataVencimento = ((DateTime)modTransp.DataVencimentoRNTRC);
                        }
                        else
                        {
                            dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day, 0, 0, 0).AddYears(5);
                        }

                        retorno.Numero = modTransp?.RNTRC;
                        retorno.DataEmissao = modTransp.DataEmissaoRNTRC == null ? null : ((DateTime)modTransp.DataEmissaoRNTRC).ToString("yyyy-MM-dd");
                        retorno.DataVencimento = dataVencimento.ToString("yyyy-MM-dd");
                        retorno.TipoTransportador = pessoa?.Tipo == "J" ? "ETC" : "TAC";
                    }
                }
            }

            return retorno;
        }

        public string ObterCodigoPessoa(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            Hashtable objetoPessoa = new Hashtable();
            objetoPessoa.Add("cod_pessoa", repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(pessoa?.CPF_CNPJ.ToString() ?? motorista?.CPF.ToString(), TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM)?.CodigoExterno ?? "");

            if (pessoa != null)
            {
                if (pessoa.Tipo.Equals("E"))
                {
                    objetoPessoa.Add("cnpj_cpf", "");
                    objetoPessoa.Add("pessoa_estrangeira_nome", pessoa.Nome ?? "");
                    objetoPessoa.Add("pessoa_estrangeira_numero", pessoa.Codigo);
                }
                else
                {
                    objetoPessoa.Add("cnpj_cpf", pessoa?.CPF_CNPJ_SemFormato ?? motorista?.CPF ?? "");
                    objetoPessoa.Add("pessoa_estrangeira_nome", "");
                    objetoPessoa.Add("pessoa_estrangeira_numero", "");
                }
            }
            else
            {
                if (motorista?.MotoristaEstrangeiro ?? false)
                {
                    objetoPessoa.Add("cnpj_cpf", "");
                    objetoPessoa.Add("pessoa_estrangeira_nome", motorista.Nome ?? "");
                    objetoPessoa.Add("pessoa_estrangeira_numero", motorista.CPF);
                }
                else
                {
                    objetoPessoa.Add("cnpj_cpf", pessoa?.CPF_CNPJ_SemFormato ?? motorista?.CPF ?? "");
                    objetoPessoa.Add("pessoa_estrangeira_nome", "");
                    objetoPessoa.Add("pessoa_estrangeira_numero", "");
                }
            }

            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getCodPessoa" },
                    { "parameters", objetoPessoa }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodPessoa(retWS, pessoa?.CPF_CNPJ.ToString() ?? motorista?.CPF ?? "");

        }

        public string ObterCodigoEndereco(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Usuario motorista, string codigoExternoPessoa)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            Hashtable objetoEndereco = new Hashtable();
            if (pessoa != null)
            {
                objetoEndereco.Add("cod_pessoa", codigoExternoPessoa ?? "");
                objetoEndereco.Add("cod_endereco", "");
                objetoEndereco.Add("cep", pessoa.CEP ?? "");
                objetoEndereco.Add("complemento", pessoa.Complemento ?? "");
                objetoEndereco.Add("numero", pessoa.Numero ?? "");
                objetoEndereco.Add("codigo_postal", "");
            }
            else
            {
                objetoEndereco.Add("cod_pessoa", codigoExternoPessoa ?? "");
                objetoEndereco.Add("cod_endereco", "");
                objetoEndereco.Add("cep", motorista.CEP ?? "");
                objetoEndereco.Add("complemento", motorista.Complemento ?? "");
                objetoEndereco.Add("numero", motorista.NumeroEndereco ?? "");
                objetoEndereco.Add("codigo_postal", "");
            }

            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getCodEndereco" },
                    { "parameters", objetoEndereco }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodEndereco(retWS, pessoa?.CPF_CNPJ.ToString() ?? motorista?.CPF ?? "");

        }
        #endregion
    }
}