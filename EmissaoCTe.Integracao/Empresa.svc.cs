using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmissaoCTe.Integracao
{
    public class Empresa : IEmpresa
    {
        #region Métodos Públicos

        public Retorno<string> SalvarEmpresaEmissora(Dominio.ObjetosDeValor.Empresa empresa, string cnpjEmpresaPai, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (empresa == null)
                    return new Retorno<string>() { Mensagem = "Empresa não pode ser nula.", Status = false };

                Servicos.Log.TratarErro("Salvar empresa emissora: " + JsonConvert.SerializeObject(empresa));

                //return new Retorno<string>() { Mensagem = "Método desabilitado.", Status = false };

                if (string.IsNullOrWhiteSpace(cnpjEmpresaPai))
                    return new Retorno<string>() { Mensagem = "CNPJ da empresa pai inválido.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjEmpresaPai));

                if (empresaPai == null || empresaPai.EmpresaAdministradora == null)
                    return new Retorno<string>() { Mensagem = "A empresa pai não foi encontrada.", Objeto = empresa.CNPJ, Status = false };

                if (!empresaPai.Status.Equals("A"))
                    return new Retorno<string>() { Mensagem = "A empresa pai está inativa.", Objeto = empresa.CNPJ, Status = false };

                if (!empresaPai.EmpresaAdministradora.Status.Equals("A"))
                    return new Retorno<string>() { Mensagem = "A empresa administradora está inativa.", Objeto = empresa.CNPJ, Status = false };

                Dominio.Entidades.Empresa empresaEmissora = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(empresa.CNPJ));

                if (empresaEmissora != null && empresaEmissora.EmpresaPai.CNPJ != empresaPai.CNPJ)
                    return new Retorno<string>() { Mensagem = "A empresa emissora já se encontra cadastrada para outra empresa pai.", Objeto = empresa.CNPJ, Status = false };

                if (empresaPai.Configuracao != null && token != empresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<string>() { Mensagem = "Token de acesso inválido.", Objeto = empresa.CNPJ, Status = false };

                unidadeDeTrabalho.Start();

                this.PreencherEmpresa(empresa, ref empresaEmissora, unidadeDeTrabalho);

                //empresaEmissora.EmpresaPai = empresaPai;

                string mensagemErro = this.ValidarEmpresa(empresaEmissora);

                if (mensagemErro.Length > 0)
                {
                    unidadeDeTrabalho.Rollback();

                    return new Retorno<string>() { Mensagem = mensagemErro.ToString(), Objeto = empresa.CNPJ, Status = false };
                }

                if (empresaEmissora.Codigo <= 0)
                {
                    empresaEmissora.SerieCTeDentro = empresaPai.SerieCTeDentro;
                    empresaEmissora.SerieCTeFora = empresaPai.SerieCTeFora;
                    empresaEmissora.SerieMDFe = empresaPai.SerieMDFe;

                    repEmpresa.Inserir(empresaEmissora);

                    this.SalvarSeriePadrao(empresaEmissora, unidadeDeTrabalho);
                    this.SalvarUsuarioPadrao(empresaEmissora, unidadeDeTrabalho);
                    this.ReplicarDadosPadroes(empresaEmissora, unidadeDeTrabalho);
                }
                else
                {
                    //repEmpresa.Atualizar(empresaEmissora);
                }

                unidadeDeTrabalho.CommitChanges();

                return new Retorno<string>() { Mensagem = "Integração realizada com sucesso.", Objeto = empresa.CNPJ, Status = true };
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new Retorno<string>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<RetornoValidarEmpresa> ValidarEmpresaEmissoraParaEmissao(string cnpjEmpresaEmissora, string cnpjEmpresaPai)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresaEmissora);

                if (empresa == null)
                    return new Retorno<RetornoValidarEmpresa>() { Mensagem = "Empresa emissora não encontrada!", Status = false };

                if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != cnpjEmpresaPai)
                    return new Retorno<RetornoValidarEmpresa>() { Mensagem = "A empresa emissora não pertence a esta empresa pai!", Status = false };

                List<string> retorno = new List<string>();

                if (string.IsNullOrWhiteSpace(empresa.InscricaoEstadual) || (empresa.InscricaoEstadual.Length < 2 || empresa.InscricaoEstadual.Length > 14))
                    retorno.Add("A inscrição estadual deve ter no mínimo 2 e no máximo 14 dígitos.");

                if (string.IsNullOrWhiteSpace(empresa.RegistroANTT) || empresa.RegistroANTT.Length != 8)
                    retorno.Add("O RNTRC deve ter exatamente 8 dígitos.");

                if (string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                    retorno.Add("O certificado não está configurado.");

                if (!empresa.DataFinalCertificado.HasValue || empresa.DataFinalCertificado.Value <= DateTime.Now)
                    retorno.Add("O certificado está vencido.");

                if (empresa.Status == "I")
                    retorno.Add("Cadastro está inativo.");

                if (retorno.Count > 0)
                    return new Retorno<RetornoValidarEmpresa>() { Mensagem = "A empresa não está apta para a emissão de CT-e.", Objeto = new RetornoValidarEmpresa() { Inconsistencias = retorno, Ambiente = empresa.TipoAmbiente }, Status = false };

                return new Retorno<RetornoValidarEmpresa>() { Mensagem = "Empresa apta para a emissão de CT-e.", Objeto = new RetornoValidarEmpresa() { Ambiente = empresa.TipoAmbiente }, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<RetornoValidarEmpresa>() { Mensagem = "Ocorreu uma falha genérica ao validar a empresa emissora.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<object> SalvarEmpresaEmissoraIntegracao(Dominio.ObjetosDeValor.Empresa empresa, string cnpjEmpresaPai, string chaveAcesso)
        {
            Servicos.Log.TratarErro("Salvar empresa emissora integraçao: " + empresa.CNPJ);
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            Retorno<object> retorno = new Retorno<object>();

            try
            {
                if (!string.IsNullOrWhiteSpace(chaveAcesso) && !string.IsNullOrWhiteSpace(cnpjEmpresaPai))
                {
                    string cnpjEmpresaAdministradora = Servicos.Criptografia.Descriptografar(chaveAcesso, "#CH4V3%%CNPJ@1NT3GR4c4o%%Mu1T1$$#");

                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                    Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCNPJ(cnpjEmpresaPai, cnpjEmpresaAdministradora);

                    if (empresaPai != null && empresaPai.Status.Equals("A") && empresaPai.EmpresaAdministradora.Status.Equals("A"))
                    {
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                        Dominio.Entidades.Empresa empresaEmissora = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(empresa.CNPJ), Utilidades.String.OnlyNumbers(cnpjEmpresaPai), Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora));

                        if (empresaEmissora == null)
                        {
                            if (repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(empresa.CNPJ)) != null)
                            {
                                retorno.Status = false;
                                retorno.Mensagem = "Já existe uma empresa com este CNPJ cadastrada.";
                                return retorno;
                            }
                        }

                        this.PreencherEmpresa(empresa, ref empresaEmissora, unidadeDeTrabalho);

                        empresaEmissora.EmpresaPai = empresaPai;

                        string mensagemErro = this.ValidarEmpresa(empresaEmissora);
                        if (mensagemErro.Length > 0)
                        {
                            retorno.Status = false;
                            retorno.Mensagem = mensagemErro;
                        }
                        else
                        {
                            if (empresaEmissora.Codigo <= 0)
                                repEmpresa.Inserir(empresaEmissora);
                            else
                                repEmpresa.Atualizar(empresaEmissora);

                            this.EnviarEmailEmpresaIntegrada(empresaEmissora, unidadeDeTrabalho);

                            retorno.Status = true;
                            retorno.Mensagem = "Empresa salva com sucesso.";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "Empresa administradora não encontrada.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Dados inválidos.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.Mensagem = string.Concat("Ocorreu uma falha genérica ao salvar a empresa: ", ex.ToString());
            }
            unidadeDeTrabalho.Dispose();
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Empresa> ObterDetalhesEmpresa(string cnpjEmpresaEmissora, string cnpjEmpresaPai, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresaEmissora);

                if (empresa == null)
                    return new Retorno<Dominio.ObjetosDeValor.Empresa>() { Mensagem = "Empresa emissora não encontrada!", Status = false };

                if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != cnpjEmpresaPai)
                    return new Retorno<Dominio.ObjetosDeValor.Empresa>() { Mensagem = "A empresa emissora não pertence a esta empresa pai!", Status = false };

                List<string> retorno = new List<string>();

                if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<Dominio.ObjetosDeValor.Empresa>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.ObjetosDeValor.Empresa dados = new Dominio.ObjetosDeValor.Empresa()
                {
                    ANTT = empresa.RegistroANTT,
                    Bairro = empresa.Bairro,
                    CEP = empresa.CEP,
                    CNAE = empresa.CNAE,
                    CNPJ = empresa.CNPJ,
                    CodigoIBGECidade = empresa.Localidade.CodigoIBGE,
                    Complemento = empresa.Complemento,
                    Contato = empresa.Contato,
                    DataEmissaoANTT = empresa.DataEmissaoANTT.HasValue ? empresa.DataEmissaoANTT.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataFinalCertificado = empresa.DataFinalCertificado.HasValue ? empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataInicialCertificado = empresa.DataInicialCertificado.HasValue ? empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataValidadeANTT = empresa.DataValidadeANTT.HasValue ? empresa.DataValidadeANTT.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Email = empresa.Email,
                    EmailAdministrativo = empresa.EmailAdministrativo,
                    EmailContador = empresa.EmailContador,
                    Endereco = empresa.Endereco,
                    Fax = empresa.Fax,
                    Inscricao_ST = empresa.Inscricao_ST,
                    InscricaoEstadual = empresa.InscricaoEstadual,
                    NomeContador = empresa.NomeContador,
                    NomeFantasia = empresa.NomeFantasia,
                    Numero = empresa.Numero,
                    OptanteSimplesNacional = empresa.OptanteSimplesNacional,
                    RazaoSocial = empresa.RazaoSocial,
                    Responsavel = empresa.Responsavel,
                    //SenhaCertificado = empresa.SenhaCertificado,
                    SerieCertificado = empresa.SerieCertificado,
                    StatusEmail = empresa.StatusEmail == "A" ? true : false,
                    StatusEmailAdministrativo = empresa.StatusEmailAdministrativo == "A" ? true : false,
                    StatusEmailContador = empresa.StatusEmailContador == "A" ? true : false,
                    Suframa = empresa.Suframa,
                    Telefone = empresa.Telefone,
                    TelefoneContador = empresa.TelefoneContador,
                    TelefoneContato = empresa.TelefoneContato,
                    Status = empresa.Status == "A" ? "Ativo" : "Inativo"
                };

                return new Retorno<Dominio.ObjetosDeValor.Empresa>() { Mensagem = "Dados retornados com sucesso.", Status = true, Objeto = dados };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<Dominio.ObjetosDeValor.Empresa>() { Mensagem = "Ocorreu uma falha ao obter os dados da empresa emissora.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<string> EnviarVencimentoCertificado(string cnpj, string nome, string dataVencimento, string ambiente, string homologacao, string email, string telefone, Dominio.ObjetosDeValor.Empresa empresa)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("EnviarVencimentoCertificado - Ambiente: " + ambiente);
                Servicos.Log.TratarErro("EnviarVencimentoCertificado - CNPJ: " + cnpj);
                Servicos.Log.TratarErro("EnviarVencimentoCertificado - Vencimento: " + dataVencimento);
                Servicos.Log.TratarErro("EnviarVencimentoCertificado - Empresa: " + (empresa != null ? Newtonsoft.Json.JsonConvert.SerializeObject(empresa) : string.Empty));

                DateTime data;
                DateTime.TryParseExact(dataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                Repositorio.VencimentoCertificado repVencimentoCertificado = new Repositorio.VencimentoCertificado(unidadeDeTrabalho);
                Dominio.Entidades.VencimentoCertificado vencimento = repVencimentoCertificado.BuscarUltimoPorCNPJeAmbiente(cnpj, ambiente);

                if (vencimento == null)
                {
                    vencimento = new Dominio.Entidades.VencimentoCertificado();
                    vencimento.CNPJ = cnpj;
                    vencimento.Nome = Utilidades.String.Left(nome, 100);
                    vencimento.DataVencimento = data;
                    vencimento.Ambiente = Utilidades.String.Left(ambiente, 500);
                    vencimento.Email = Utilidades.String.Left(email, 500);
                    vencimento.Telefone = Utilidades.String.Left(telefone, 500);
                    vencimento.Status = "P";
                    vencimento.Homologacao = homologacao;
                    repVencimentoCertificado.Inserir(vencimento);
                }
                else if (vencimento != null && vencimento.DataVencimento < data)
                {
                    string log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " Novo vencimento: ", dataVencimento);
                    vencimento.Status = "C";//Atualização Confirmada
                    vencimento.Log = !string.IsNullOrWhiteSpace(vencimento.Log) ? string.Concat(vencimento.Log, " / ", log) : log;
                    repVencimentoCertificado.Atualizar(vencimento);

                    Dominio.Entidades.VencimentoCertificado vencimentoNovo = repVencimentoCertificado.BuscarPorCNPJeVencimentoAmbiente(cnpj, data, ambiente);
                    if (vencimentoNovo == null)
                    {
                        vencimentoNovo = new Dominio.Entidades.VencimentoCertificado();
                        vencimentoNovo.CNPJ = cnpj;
                        vencimentoNovo.Nome = Utilidades.String.Left(nome, 100);
                        vencimentoNovo.DataVencimento = data;
                        vencimentoNovo.Ambiente = Utilidades.String.Left(ambiente, 500);
                        vencimentoNovo.Email = Utilidades.String.Left(email, 500);
                        vencimentoNovo.Telefone = Utilidades.String.Left(telefone, 500);
                        vencimentoNovo.Status = "P";
                        vencimentoNovo.Homologacao = homologacao;
                        repVencimentoCertificado.Inserir(vencimentoNovo);
                    }
                    else
                    {
                        if (vencimento.Status != "P")
                        {
                            log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " Atualizado registro: ", dataVencimento);
                            vencimento.Log = log;
                        }
                        vencimentoNovo.DataVencimento = data;
                        vencimentoNovo.Nome = Utilidades.String.Left(nome, 100);
                        vencimentoNovo.Email = Utilidades.String.Left(email, 500);
                        vencimentoNovo.Telefone = Utilidades.String.Left(telefone, 500);
                        vencimentoNovo.Status = "P";
                        vencimentoNovo.Homologacao = homologacao;
                        repVencimentoCertificado.Atualizar(vencimentoNovo);
                    }
                }
                else if (vencimento != null && vencimento.DataVencimento > data)
                {
                    string log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " Vencimento atualizado: ", dataVencimento);
                    vencimento.Status = "P";
                    vencimento.Log = !string.IsNullOrWhiteSpace(vencimento.Log) ? string.Concat(vencimento.Log, " / ", log) : log;
                    vencimento.Nome = Utilidades.String.Left(nome, 100);
                    vencimento.Email = Utilidades.String.Left(email, 500);
                    vencimento.Telefone = Utilidades.String.Left(telefone, 500);
                    vencimento.Homologacao = homologacao;
                    repVencimentoCertificado.Atualizar(vencimento);
                }
                else if (vencimento != null)
                {
                    vencimento.Nome = Utilidades.String.Left(nome, 100);
                    vencimento.Email = Utilidades.String.Left(email, 500);
                    vencimento.Telefone = Utilidades.String.Left(telefone, 500);
                    vencimento.Homologacao = homologacao;
                    repVencimentoCertificado.Atualizar(vencimento);
                }

                if (empresa != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(empresa.CNPJ)))
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                    Dominio.Entidades.Empresa empresaCadastro = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(empresa.CNPJ));
                    if (empresaCadastro == null)
                    {
                        this.PreencherEmpresa(empresa, ref empresaCadastro, unidadeDeTrabalho);

                        empresaCadastro.Observacao = "Empresa cadastrada pelo ambiente " + ambiente + " em " + DateTime.Now.ToString("ddMMyyy hh:mm") + ".";

                        repEmpresa.Inserir(empresaCadastro);
                    }
                }

                return new Retorno<string>() { Mensagem = "Integração realizada com sucesso.", Objeto = cnpj, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<string>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<RetornoValidarEmpresa> BuscarSeguroTransportador(string cnpjTransportador, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ApoliceDeSeguro repApoliceDeSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);
                               
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador);

                if (empresa == null)
                    return new Retorno<RetornoValidarEmpresa>() { Mensagem = "Transportador não encontrado!", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<RetornoValidarEmpresa>() { Mensagem = "Token de acesso inválido.", Status = false };

                List<Dominio.Entidades.ApoliceDeSeguro> listaApolices = repApoliceDeSeguro.BuscarPorCliente(empresa.Codigo, empresa.EmpresaPai != null ? empresa.Codigo : 0, 0);

                List<string> retorno = new List<string>();

                if (listaApolices == null || listaApolices.Count == 0)
                    retorno.Add("Nenhuma Apólice de seguro valida disponível.");

                if (empresa.Configuracao == null || empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido)
                    retorno.Add("Nenhuma Integradora configurada para realizar a averbação.");

                if (retorno.Count > 0)
                    return new Retorno<RetornoValidarEmpresa>() { Mensagem = "", Objeto = new RetornoValidarEmpresa() { Inconsistencias = retorno, Ambiente = empresa.TipoAmbiente }, Status = false };

                return new Retorno<RetornoValidarEmpresa>() { Mensagem = "Empresa possui a apolice "+ listaApolices.FirstOrDefault().NumeroApolice + " com validade até "+ listaApolices.FirstOrDefault().DataFimVigencia.Value.ToString("dd/MM/yyyy") + " disponível para emissão.", Objeto = new RetornoValidarEmpresa() { Ambiente = empresa.TipoAmbiente }, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<RetornoValidarEmpresa>() { Mensagem = "Ocorreu uma falha genérica ao validar a empresa emissora.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void EnviarEmailEmpresaIntegrada(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            StringBuilder sbMensagemEmail = new StringBuilder();
            sbMensagemEmail.Append("<p>O sistema de integração de empresas realizou a integração da seguinte empresa:</p><p>Código: ").Append(empresa.Codigo).Append("<br />");
            sbMensagemEmail.Append("CNPJ: ").Append(empresa.CNPJ).Append("<br />");
            sbMensagemEmail.Append("Nome: ").Append(empresa.RazaoSocial).Append("<br /></p><p>Atenciosamente,<br />Equipe MultiCTe</p>");
            Servicos.Email svcEmail = new Servicos.Email(unidadeDeTrabalho);
            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "cesar@multisoftware.com.br", "", "", "[Sistema de Integração - MultiCTe] Empresa integrada com sucesso.", sbMensagemEmail.ToString(), string.Empty, null, null, true, string.Empty, 0, unidadeDeTrabalho);
        }

        private string ValidarEmpresa(Dominio.Entidades.Empresa empresa)
        {
            StringBuilder mensagemErro = new StringBuilder();

            if (!Utilidades.Validate.ValidarCNPJ(empresa.CNPJ))
                mensagemErro.Append("CNPJ inválido, ");

            if (string.IsNullOrWhiteSpace(empresa.Bairro))
                mensagemErro.Append("Bairro inválido, ");

            if (empresa.CEP.Length != 8)
                mensagemErro.Append("CEP inválido, ");

            if (string.IsNullOrWhiteSpace(empresa.Endereco))
                mensagemErro.Append("Endereço inválido, ");

            if (string.IsNullOrWhiteSpace(empresa.Numero))
                mensagemErro.Append("Número inválido, ");

            if (empresa.Localidade == null)
                mensagemErro.Append("Código do IBGE da cidade inválido, ");

            if (string.IsNullOrWhiteSpace(empresa.RazaoSocial))
                mensagemErro.Append("Razão social inválida, ");

            if (string.IsNullOrWhiteSpace(empresa.NomeFantasia))
                mensagemErro.Append("Nome fantasia inválido, ");

            if (empresa.RegistroANTT.Length != 8)
                mensagemErro.Append("RNTRC (Registro ANTT) inválido, ");

            if (empresa.EmpresaAdministradora != null && empresa.EmpresaPai != null)
                mensagemErro.Append("Não é possível converter uma empresa existente para emissora ou administradora, ");

            if (!string.IsNullOrWhiteSpace(empresa.Email))
            {
                var emails = empresa.Email.Split(';');

                foreach (string email in emails)
                    if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                        mensagemErro.Append(string.Concat("E-mail (", email, ") inválido, "));
            }

            if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo))
            {
                var emails = empresa.EmailAdministrativo.Split(';');

                foreach (string email in emails)
                    if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                        mensagemErro.Append(string.Concat("E-mail Administrativo (", email, ") inválido, "));
            }

            if (!string.IsNullOrWhiteSpace(empresa.EmailContador))
            {
                var emails = empresa.EmailContador.Split(';');

                foreach (string email in emails)
                    if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                        mensagemErro.Append(string.Concat("E-mail do Contador (", email, ") inválido, "));
            }

            if (mensagemErro.Length > 0)
                mensagemErro = mensagemErro.Remove(mensagemErro.Length - 2, 2).Append(".");

            return mensagemErro.ToString();
        }

        private void PreencherEmpresa(Dominio.ObjetosDeValor.Empresa origem, ref Dominio.Entidades.Empresa destino, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            if (destino == null)
                destino = new Dominio.Entidades.Empresa();

            destino.Bairro = origem.Bairro;
            destino.CEP = Utilidades.String.OnlyNumbers(origem.CEP);
            destino.CNAE = origem.CNAE;
            destino.Complemento = origem.Complemento;
            destino.Contato = origem.Contato;
            destino.DataAtualizacao = DateTime.Now;

            DateTime dataEmissaoANTT, dataFinalCertificado, dataInicialCertificado, dataValidadeANTT = DateTime.MinValue;
            DateTime.TryParseExact(origem.DataEmissaoANTT, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoANTT);
            DateTime.TryParseExact(origem.DataFinalCertificado, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCertificado);
            DateTime.TryParseExact(origem.DataInicialCertificado, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCertificado);
            DateTime.TryParseExact(origem.DataValidadeANTT, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataValidadeANTT);

            if (dataEmissaoANTT != DateTime.MinValue)
                destino.DataEmissaoANTT = dataEmissaoANTT;
            else
                destino.DataEmissaoANTT = null;

            if (dataValidadeANTT != DateTime.MinValue)
                destino.DataValidadeANTT = dataValidadeANTT;
            else
                destino.DataValidadeANTT = null;

            destino.Email = origem.Email;
            destino.EmailAdministrativo = origem.EmailAdministrativo;
            destino.EmailContador = origem.EmailContador;
            destino.Endereco = origem.Endereco;
            destino.Fax = origem.Fax;
            destino.Inscricao_ST = Utilidades.String.OnlyNumbers(origem.Inscricao_ST);
            destino.InscricaoEstadual = Utilidades.String.OnlyNumbers(origem.InscricaoEstadual);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            destino.Localidade = repLocalidade.BuscarPorCodigoIBGE(origem.CodigoIBGECidade);

            destino.NomeContador = origem.NomeContador;
            destino.NomeFantasia = origem.NomeFantasia;
            destino.Numero = origem.Numero;
            if (origem.OptanteSimplesNacional.HasValue)
                destino.OptanteSimplesNacional = origem.OptanteSimplesNacional.HasValue ? origem.OptanteSimplesNacional.Value : false;
            destino.RazaoSocial = origem.RazaoSocial;
            destino.RegistroANTT = Utilidades.String.OnlyNumbers(origem.ANTT);
            destino.Responsavel = origem.Responsavel;
            destino.Suframa = origem.Suframa;
            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(origem.Telefone)))
                destino.Telefone = Utilidades.String.OnlyNumbers(origem.Telefone);
            destino.TelefoneContador = Utilidades.String.OnlyNumbers(origem.TelefoneContador);
            destino.TelefoneContato = Utilidades.String.OnlyNumbers(origem.TelefoneContato);
            destino.Status = origem.Status;
            destino.UsarTipoOperacaoApolice = false;
            destino.DataCadastro = DateTime.Now.Date;
            destino.TransportadorAdministrador = true;

            if (destino.Codigo <= 0)
            {
                destino.EmpresaPai = repEmpresa.BuscarPorCodigo(1);
                destino.CNPJ = Utilidades.String.OnlyNumbers(origem.CNPJ);
                destino.Status = "A";
                destino.StatusEmail = !origem.StatusEmail ? "I" : "A";
                destino.StatusEmailAdministrativo = !origem.StatusEmailAdministrativo ? "I" : "A";
                destino.StatusEmailContador = !origem.StatusEmailContador ? "I" : "A";
                destino.SenhaCertificado = origem.SenhaCertificado;
                destino.SerieCertificado = origem.SerieCertificado;
                destino.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Homologacao;

                if (dataFinalCertificado != DateTime.MinValue)
                    destino.DataFinalCertificado = dataFinalCertificado;
                else
                    destino.DataFinalCertificado = null;

                if (dataInicialCertificado != DateTime.MinValue)
                    destino.DataInicialCertificado = dataInicialCertificado;
                else
                    destino.DataFinalCertificado = null;
            }
        }

        private void SalvarSeriePadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);

            if (repEmpresaSerie.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                if (empresa.EmpresaPai.SerieCTeDentro > 0)
                {
                    empresa.SerieCTeDentro = empresa.EmpresaPai.SerieCTeDentro;
                    if (repEmpresaSerie.BuscarPorSerie(empresa.Codigo, empresa.EmpresaPai.SerieCTeDentro, Dominio.Enumeradores.TipoSerie.CTe) == null)
                    {
                        Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                        serie.Empresa = empresa;
                        serie.Numero = empresa.EmpresaPai.SerieCTeDentro;
                        serie.Status = "A";
                        serie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                        repEmpresaSerie.Inserir(serie);
                    }
                }
                if (empresa.EmpresaPai.SerieCTeFora > 0)
                {
                    empresa.SerieCTeFora = empresa.EmpresaPai.SerieCTeFora;
                    if (repEmpresaSerie.BuscarPorSerie(empresa.Codigo, empresa.EmpresaPai.SerieCTeFora, Dominio.Enumeradores.TipoSerie.CTe) == null)
                    {
                        Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                        serie.Empresa = empresa;
                        serie.Numero = empresa.EmpresaPai.SerieCTeFora;
                        serie.Status = "A";
                        serie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                        repEmpresaSerie.Inserir(serie);
                    }
                }
                if (empresa.EmpresaPai.SerieMDFe > 0)
                {
                    empresa.SerieMDFe = empresa.EmpresaPai.SerieMDFe;
                    if (repEmpresaSerie.BuscarPorSerie(empresa.Codigo, empresa.EmpresaPai.SerieMDFe, Dominio.Enumeradores.TipoSerie.MDFe) == null)
                    {
                        Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                        serie.Empresa = empresa;
                        serie.Numero = empresa.EmpresaPai.SerieMDFe;
                        serie.Status = "A";
                        serie.Tipo = Dominio.Enumeradores.TipoSerie.MDFe;
                        repEmpresaSerie.Inserir(serie);
                    }
                }
                if (empresa.EmpresaPai.SerieCTeDentro > 0 || empresa.EmpresaPai.SerieCTeFora > 0 || empresa.EmpresaPai.SerieMDFe > 0)
                    this.ReplicarConfig(empresa, unidadeTrabalho);


                if (empresa.EmpresaPai.SerieCTeDentro > 0 && empresa.EmpresaPai.SerieCTeFora > 0)
                {
                    Dominio.Entidades.EmpresaSerie serieCTe = new Dominio.Entidades.EmpresaSerie();

                    serieCTe.Empresa = empresa;
                    serieCTe.Numero = 1;
                    serieCTe.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                    serieCTe.Status = "A";
                    repEmpresaSerie.Inserir(serieCTe);
                }
            }

        }

        private void SalvarUsuarioPadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);

            Dominio.Entidades.Usuario usuarioAux = repUsuario.BuscarPorLogin(empresa.CNPJ);

            if (usuarioAux == null)
            {
                Repositorio.Setor repSetor = new Repositorio.Setor(unidadeTrabalho);

                Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();

                usuario.Setor = repSetor.BuscarPorCodigo(1);
                usuario.CPF = empresa.CNPJ;
                usuario.Email = "";
                usuario.Empresa = empresa;
                usuario.Endereco = empresa.Endereco;
                usuario.Localidade = empresa.Localidade;
                usuario.Login = empresa.CNPJ;
                usuario.Senha = empresa.CNPJ.Substring(0, 5);
                usuario.Status = "A";
                usuario.Tipo = "U";
                usuario.Telefone = empresa.Telefone;
                usuario.Nome = empresa.RazaoSocial;
                usuario.Email = string.IsNullOrWhiteSpace(empresa.Email) ? string.Empty : empresa.Email.Split(';')[0];
                usuario.UsuarioAdministrador = true;

                if (empresa.EmpresaPai != null)
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                else if (empresa.EmpresaAdministradora != null)
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Admin;

                repUsuario.Inserir(usuario);

                //this.EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario);
            }
        }

        private void EnviarEmailDeNotificacaoDeUsuarioCadastrado(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(usuario.Empresa.Email))
                return;

            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Seus dados para acesso ao sistema MultiNFe são:<br /><br />");
            sb.Append("Usuário: ").Append(usuario.Login).Append("<br />");
            sb.Append("Senha: ").Append(usuario.Senha).Append("</p><br />");

            if (usuario.Empresa.EmpresaPai != null && !string.IsNullOrWhiteSpace(usuario.Empresa.EmpresaPai.URLSistema))
                sb.Append("Para utilizar o sistema MultiNFe acesse ").Append(usuario.Empresa.EmpresaPai.URLSistema).Append(".");
            else
                sb.Append("Para utilizar o sistema MultiNFe acesse http://www.commerce.inf.br/ e utilize a opção de Login.");

            System.Text.StringBuilder ss = new System.Text.StringBuilder();
            ss.Append("Commerce Sistemas - http://www.commerce.inf.br/ <br />");
            ss.Append("Fone/Fax: (49)3025-9500 <br />");
            ss.Append("E-mail: suporte@commerce.inf.br");

            svcEmail.EnviarEmail("nfe@commerce.inf.br", "nfe@commerce.inf.br", "cesaoexp18", usuario.Empresa.Email.Split(',')[0], "", "", "MultiNFe - Dados para Acesso ao Sistema", sb.ToString(), "smtp.commerce.inf.br", null, ss.ToString(), false, "suporte@commerce.inf.br");
        }

        private void ReplicarDadosPadroes(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            this.ReplicarPlanosDeContas(empresa, unidadeTrabalho);
            this.ReplicarEixosDeVeiculos(empresa, unidadeTrabalho);
            this.ReplicarAliquotasDeICMS(empresa, unidadeTrabalho);
        }

        private void ReplicarAliquotasDeICMS(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.AliquotaDeICMS repAliquota = new Repositorio.AliquotaDeICMS(unidadeTrabalho);
            if (repAliquota.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.AliquotaDeICMS> aliquotas = repAliquota.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);
                foreach (Dominio.Entidades.AliquotaDeICMS aliquota in aliquotas)
                {
                    Dominio.Entidades.AliquotaDeICMS aliquotaNova = new Dominio.Entidades.AliquotaDeICMS();

                    aliquotaNova.Aliquota = aliquota.Aliquota;
                    aliquotaNova.Empresa = empresa;
                    aliquotaNova.Status = aliquota.Status;

                    repAliquota.Inserir(aliquota);
                }
            }
        }

        private void ReplicarPlanosDeContas(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.PlanoDeConta repPlanosDeContas = new Repositorio.PlanoDeConta(unidadeTrabalho);
            if (repPlanosDeContas.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.PlanoDeConta> planosDeContas = repPlanosDeContas.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);
                foreach (Dominio.Entidades.PlanoDeConta plano in planosDeContas)
                {
                    Dominio.Entidades.PlanoDeConta planoNovo = new Dominio.Entidades.PlanoDeConta();
                    planoNovo.Conta = plano.Conta;
                    planoNovo.ContaContabil = plano.ContaContabil;
                    planoNovo.Descricao = plano.Descricao;
                    planoNovo.Empresa = empresa;
                    planoNovo.Status = plano.Status;
                    planoNovo.Tipo = plano.Tipo;
                    planoNovo.TipoDeConta = plano.TipoDeConta;
                    repPlanosDeContas.Inserir(planoNovo);
                }
            }
        }

        private void ReplicarEixosDeVeiculos(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EixoVeiculo repEixoDeVeiculo = new Repositorio.EixoVeiculo(unidadeTrabalho);

            if (repEixoDeVeiculo.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.EixoVeiculo> listaEixos = repEixoDeVeiculo.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);
                foreach (Dominio.Entidades.EixoVeiculo eixo in listaEixos)
                {
                    Dominio.Entidades.EixoVeiculo eixoNovo = new Dominio.Entidades.EixoVeiculo();
                    eixoNovo.Descricao = eixo.Descricao;
                    eixoNovo.Dianteiro = eixo.Dianteiro;
                    eixoNovo.Empresa = empresa;
                    eixoNovo.Interno_Externo = eixo.Interno_Externo;
                    eixoNovo.OrdemEixo = eixo.OrdemEixo;
                    eixoNovo.Posicao = eixo.Posicao;
                    eixoNovo.Status = eixo.Status;
                    eixoNovo.Tipo = eixo.Tipo;
                    repEixoDeVeiculo.Inserir(eixoNovo);
                }
            }
        }

        private void ReplicarConfig(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);

            if (empresa.EmpresaPai != null)
            {
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(empresa.EmpresaPai.Codigo);

                bool inserir = false;

                if (empresaPai != null && empresaPai.Configuracao != null)
                {
                    if (empresa.Configuracao == null)
                    {
                        inserir = true;
                        empresa.Configuracao = new Dominio.Entidades.ConfiguracaoEmpresa();
                    }

                    empresa.Configuracao.DiasParaEmissaoDeCTeAnulacao = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeAnulacao;
                    empresa.Configuracao.DiasParaEmissaoDeCTeComplementar = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeComplementar;
                    empresa.Configuracao.DiasParaEmissaoDeCTeSubstituicao = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeSubstituicao;
                    empresa.Configuracao.DiasParaEntrega = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeSubstituicao;
                    empresa.Configuracao.ProdutoPredominante = empresa.EmpresaPai.Configuracao.ProdutoPredominante;
                    empresa.Configuracao.OutrasCaracteristicas = empresa.EmpresaPai.Configuracao.OutrasCaracteristicas;
                    empresa.Configuracao.ResponsavelSeguro = empresa.EmpresaPai.Configuracao.ResponsavelSeguro;
                    empresa.Configuracao.TipoImpressao = empresa.EmpresaPai.Configuracao.TipoImpressao;
                    if (empresa.SerieCTeFora != null && empresa.SerieCTeFora > 0)
                        empresa.Configuracao.SerieInterestadual = repSerie.BuscarPorSerie(empresa.Codigo, empresa.SerieCTeFora, Dominio.Enumeradores.TipoSerie.CTe);
                    if (empresa.SerieCTeDentro != null && empresa.SerieCTeDentro > 0)
                        empresa.Configuracao.SerieIntraestadual = repSerie.BuscarPorSerie(empresa.Codigo, empresa.SerieCTeDentro, Dominio.Enumeradores.TipoSerie.CTe);
                    if (empresa.SerieMDFe != null && empresa.SerieMDFe > 0)
                        empresa.Configuracao.SerieMDFe = repSerie.BuscarPorSerie(empresa.Codigo, empresa.SerieMDFe, Dominio.Enumeradores.TipoSerie.MDFe);

                    if (inserir)
                        repEmpresa.Inserir(empresa);
                    else
                        repEmpresa.Atualizar(empresa);
                }
            }
        }

        #endregion
    }
}
