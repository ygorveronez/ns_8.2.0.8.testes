using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ClienteController : ApiController
    {

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                double cpfCnpj = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJ"]), out cpfCnpj);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                var filtrosPesquisa = new Dominio.ObjetosDeValor.FiltroPesquisaCliente()
                {
                    CpfCnpj = cpfCnpj,
                    Nome = Request.Params["Nome"],
                    Tipo = Request.Params["Tipo"],
                    GeoLocalizacaoTipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoTipo.Todos
                };

                List<Dominio.Entidades.Cliente> listaClientes = repCliente.Consultar(filtrosPesquisa, propriedadeOrdenacao: "Nome", direcaoOrdenacao: "asc", inicioRegistros: inicioRegistros, maximoRegistros: 50);
                int countClientes = repCliente.ContarConsulta(filtrosPesquisa);

                var retorno = from obj in listaClientes
                              select new
                              {
                                  obj.Tipo,
                                  obj.Nome,
                                  NomeFantasia = string.IsNullOrEmpty(obj.NomeFantasia) ? "" : obj.NomeFantasia,
                                  CPFCNPJ = obj.Tipo.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.CPF_CNPJ),
                                  obj.IE_RG,
                                  Localidade = string.Concat(obj.Localidade.Descricao, "-", obj.Localidade.Estado.Sigla)
                              };

                return Json(retorno, true, null, new string[] { "Tipo", "Razão Social|25", "Nome Fantasia|20", "CPF/CNPJ|15", "RG/IE|15", "Localidade|15" }, countClientes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os clientes.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesPorCPFCNPJ()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                double cpf_cnpj = 0F;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPF_CNPJ"]), out cpf_cnpj);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpf_cnpj);

                object retorno = null;

                if (cliente != null)
                {
                    Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);
                    Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(this.EmpresaUsuario.Codigo, cliente.CPF_CNPJ);

                    retorno = new
                    {
                        CPF_CNPJ = cliente.Tipo.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", cliente.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", cliente.CPF_CNPJ),
                        CodigoAtividade = cliente.Atividade.Codigo,
                        DescricaoAtividade = cliente.Atividade.Descricao,
                        cliente.Bairro,
                        cliente.CEP,
                        cliente.Complemento,
                        DataCadastro = string.Format("{0:dd/MM/yyyy HH:mm}", cliente.DataCadastro),
                        cliente.Email,
                        cliente.EmailStatus,
                        cliente.EmailContador,
                        cliente.EmailContadorStatus,
                        cliente.EmailContato,
                        cliente.EmailContatoStatus,
                        cliente.Endereco,
                        cliente.IE_RG,
                        cliente.InscricaoMunicipal,
                        CodigoLocalidade = cliente.Localidade.Codigo,
                        UF = cliente.Localidade.Estado.Sigla,
                        cliente.Localidade.CodigoIBGE,
                        Localidade = cliente.Localidade.Descricao,
                        Cidades = from obj in repLocalidade.BuscarPorUF(cliente.Localidade.Estado.Sigla, this.EmpresaUsuario.Codigo) select new { obj.Codigo, obj.Descricao },
                        cliente.Nome,
                        NomeFantasia = cliente.NomeFantasia,
                        cliente.Numero,
                        cliente.Telefone1,
                        cliente.Telefone2,
                        cliente.Tipo,
                        SalvarEndereco = true,
                        Agencia = dadosCliente?.Agencia ?? string.Empty,
                        DigitoAgencia = dadosCliente?.DigitoAgencia ?? string.Empty,
                        NumeroConta = dadosCliente?.NumeroConta ?? string.Empty,
                        TipoConta = dadosCliente?.TipoConta?.ToString("d") ?? string.Empty,
                        DescricaoBanco = dadosCliente?.Banco?.Descricao ?? string.Empty,
                        CodigoBanco = dadosCliente?.Banco?.Codigo ?? 0,
                        OrgaoEmissorRG = cliente.OrgaoEmissorRG?.ToString("d") ?? string.Empty,
                        Sexo = cliente.Sexo?.ToString("d") ?? string.Empty,
                        EstadoRG = cliente.EstadoRG?.Sigla ?? string.Empty,
                        DataNascimento = cliente.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        cliente.ValorTDE,
                        NumeroCartao = dadosCliente?.NumeroCartao ?? string.Empty,
                        InscricaoST = cliente.InscricaoST,
                        PercentualRetencaoICMSST = dadosCliente?.PercentualRetencaoICMSST.ToString("n2") ?? string.Empty,
                        PIS = dadosCliente?.PIS ?? string.Empty,
                        EmailTransportador = dadosCliente?.Email,
                        EmailTransportadorStatus = dadosCliente?.EmailStatus,
                        InscricaoSuframa = !string.IsNullOrWhiteSpace(cliente.InscricaoSuframa) ? cliente.InscricaoSuframa : string.Empty,
                        NomeFantasiaTransportador = dadosCliente?.NomeFantasia ?? string.Empty,
                        ArmazenaNotasParaGerarPorPeriodo = dadosCliente?.ArmazenaNotasParaGerarPorPeriodo ?? false,
                        NaoAverbarQuandoTerceiro = dadosCliente?.NaoAverbarQuandoTerceiro ?? false
                    };
                }
                return Json(retorno, true, null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os detalhes do cliente.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarTaxas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                double cnpjCliente;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Cliente"]), out cnpjCliente);

                decimal valorTDE = 0;
                decimal.TryParse(Request.Params["ValorTDE"], out valorTDE);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjCliente);

                cliente.ValorTDE = valorTDE;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);

                unitOfWork.CommitChanges();

                return Json<bool>(true, true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Cliente Taxas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult ConsultarClienteReceita()
        {
            try
            {                
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);

                if (Utilidades.Validate.ValidarCNPJ(cnpj))
                {
                    ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                    OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                    MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                    OperationContext.Current.OutgoingMessageHeaders.Add(header);
                    ConsultaCNPJ.RetornoOfRequisicaoFazendaPessoaJuridicaDggAjPvf requisicaoCNPJ = consultaCNPJ.SolicitarRequisicaoFazendaPessoaJuridica();

                    if (requisicaoCNPJ.Status)
                    {
                        string base64String = Convert.ToBase64String(requisicaoCNPJ.Objeto.Captcha, 0, requisicaoCNPJ.Objeto.Captcha.Length);
                        string htmlstr = "data:image/png;base64," + base64String;

                        var retorno = new
                        {
                            chaptcha = htmlstr,
                            Cookies = requisicaoCNPJ.Objeto.Cookies
                        };
                        return Json(retorno, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, requisicaoCNPJ.Mensagem);
                    }
                }
                else if (Utilidades.Validate.ValidarCPF(cnpj))
                {
                    return Json<bool>(false, false, "Consulta na Receita disponível apenas para CNPJ.");
                }
                else
                {
                    return Json<bool>(false, false, "CNPJ informado é inválido, por favor, verifique e tente novamente.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar dados na Receita.");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult InformarCaptchaReceita()
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica();
                requisicaoSefaz.Cookies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.WebService.CookieDinamico>>((string)Request.Params["Cookies"]);
                string CNPJ = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                string captcha = Request.Params["Captcha"];

                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = consultaCNPJ.ConsultarPessoaJuridicaFazenda(requisicaoSefaz, CNPJ, captcha);

                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                        List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, retorno.Objeto.Pessoa.Endereco.Cidade.Descricao, this.EmpresaUsuario.Codigo);
                        Dominio.ObjetosDeValor.ConsultaReceita retornoDados = new Dominio.ObjetosDeValor.ConsultaReceita
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP)),
                            Localidade = listaLocalidades.Count() == 1 ? new
                            {
                                Codigo = listaLocalidades[0].Codigo,
                                Descricao = listaLocalidades[0].DescricaoCidadeEstado,
                                UF = listaLocalidades[0].Estado.Sigla
                            } : null,
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Length > 60 ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "").Substring(0, 60) : retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : "S/N",
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Substring(0, 10) : string.Empty,
                            Fantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : retorno.Objeto.Pessoa.NomeFantasia,
                            Nome = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial,
                            Cidades = from obj in repLocalidade.BuscarPorUF(listaLocalidades[0].Estado.Sigla, this.EmpresaUsuario.Codigo)
                                      select new
                                      {
                                          obj.Codigo,
                                          obj.Descricao
                                      },
                            EnderecoDigitado = true
                        };

                        // Fixa tamanho da string
                        retornoDados.Bairro = LimitarTamanho(retornoDados.Bairro, 40);
                        retornoDados.Endereco = LimitarTamanho(retornoDados.Endereco, 80);
                        retornoDados.Numero = LimitarTamanho(retornoDados.Numero, 60);
                        retornoDados.Fantasia = LimitarTamanho(retornoDados.Fantasia, 80);
                        retornoDados.Nome = LimitarTamanho(retornoDados.Nome, 80);

                        return Json(retornoDados, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return Json<bool>(false, false, retorno.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao carregar dados da receita.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult ConsultarClienteSintegraCentralizado()
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica();
                string CNPJ = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);

                if (string.IsNullOrWhiteSpace(CNPJ))
                    return Json<bool>(false, false, "Nenhum CNPJ Informado.");

                ConsultaCNPJ.ConsultaCNPJClient consultaCNPJ = new ConsultaCNPJ.ConsultaCNPJClient();
                OperationContextScope scope = new OperationContextScope(consultaCNPJ.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = consultaCNPJ.ConsultarCadastroCentralizado(CNPJ);

                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                        Dominio.Entidades.Localidade localidade = retorno.Objeto.Pessoa.Endereco.Cidade.IBGE > 0 ? repLocalidade.BuscarPorCodigoIBGE(retorno.Objeto.Pessoa.Endereco.Cidade.IBGE) : null;
                        if (localidade == null)
                        {
                            List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, Utilidades.String.RemoveDiacritics(retorno.Objeto.Pessoa.Endereco.Cidade.Descricao), this.EmpresaUsuario.Codigo);
                            if (listaLocalidades.Count > 0)
                                localidade = listaLocalidades.FirstOrDefault();
                        }


                        Dominio.ObjetosDeValor.ConsultaReceita retornoDados = new Dominio.ObjetosDeValor.ConsultaReceita
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP)),
                            Localidade = localidade != null ? new
                            {
                                Codigo = localidade.Codigo,
                                Descricao = localidade.DescricaoCidadeEstado,
                                UF = localidade.Estado.Sigla
                            } : null,
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Length > 60 ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "").Substring(0, 60) : retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : "S/N",
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Substring(0, 10) : string.Empty,
                            Fantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : retorno.Objeto.Pessoa.NomeFantasia,
                            Nome = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial,
                            Cidades = from obj in repLocalidade.BuscarPorUF(localidade != null ? localidade.Estado.Sigla : string.Empty, this.EmpresaUsuario.Codigo)
                                      select new
                                      {
                                          obj.Codigo,
                                          obj.Descricao
                                      },
                            EnderecoDigitado = true,
                            InscricaoEstadual = retorno.Objeto.Pessoa.RGIE
                        };

                        // Fixa tamanho da string
                        retornoDados.Bairro = LimitarTamanho(retornoDados.Bairro, 40);
                        retornoDados.Endereco = LimitarTamanho(retornoDados.Endereco, 80);
                        retornoDados.Numero = LimitarTamanho(retornoDados.Numero, 60);
                        retornoDados.Fantasia = LimitarTamanho(retornoDados.Fantasia, 80);
                        retornoDados.Nome = LimitarTamanho(retornoDados.Nome, 80);

                        return Json(retornoDados, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return Json<bool>(false, false, retorno.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao carregar dados do sintegra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult ConsultarCNPJReceitaWS()
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica();
                string CNPJ = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);

                if (string.IsNullOrWhiteSpace(CNPJ))
                    return Json<bool>(false, false, "Nenhum CNPJ Informado.");


                if (this.EmpresaUsuario.CNPJ != "13969629000196" && Request.UrlReferrer != null && (Request.UrlReferrer.AbsoluteUri.Contains("Empresas.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresasEmissoras.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("Series.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EncerramentoManualMDFe.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresaContrato.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresaEDI.aspx")))
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(CNPJ);

                    if (empresa != null && this.EmpresaUsuario.Codigo != empresa.EmpresaPai?.Codigo)
                        return Json<bool>(false, false, "Empresa não encontrada.");
                }


                var json_data = string.Empty;
                using (var w = new WebClient())
                {
                    w.Encoding = System.Text.Encoding.UTF8;
                    json_data = w.DownloadString("https://www.receitaws.com.br/v1/cnpj/" + CNPJ);
                }

                ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno = new ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf();
                var retornoReceitaWS = JsonConvert.DeserializeObject<dynamic>(json_data);

                ConverterObjetoRetornoWS(ref retorno, retornoReceitaWS);

                if (retorno.Status)
                {
                    if (retorno.Objeto.ConsultaValida)
                    {
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                        Dominio.Entidades.Localidade localidade = retorno.Objeto.Pessoa.Endereco.Cidade.IBGE > 0 ? repLocalidade.BuscarPorCodigoIBGE(retorno.Objeto.Pessoa.Endereco.Cidade.IBGE) : null;
                        if (localidade == null)
                        {
                            List<Dominio.Entidades.Localidade> listaLocalidades = repLocalidade.BuscarPorUFDescricao(retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF, Utilidades.String.RemoveDiacritics(retorno.Objeto.Pessoa.Endereco.Cidade.Descricao), this.EmpresaUsuario.Codigo);
                            if (listaLocalidades.Count > 0)
                                localidade = listaLocalidades.FirstOrDefault();
                        }


                        Dominio.ObjetosDeValor.ConsultaReceita retornoDados = new Dominio.ObjetosDeValor.ConsultaReceita
                        {
                            Email = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Email) ? retorno.Objeto.Pessoa.Email.Replace("*", "") : retorno.Objeto.Pessoa.Email,
                            Bairro = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Bairro) ? retorno.Objeto.Pessoa.Endereco.Bairro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Bairro,
                            CEP = String.Format(@"{0:00\.000\-000}", Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.CEP)),
                            Localidade = localidade != null ? new
                            {
                                Codigo = localidade.Codigo,
                                Descricao = localidade.DescricaoCidadeEstado,
                                UF = localidade.Estado.Sigla
                            } : null,
                            Complemento = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Complemento) ? retorno.Objeto.Pessoa.Endereco.Complemento.Length > 60 ? retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "").Substring(0, 60) : retorno.Objeto.Pessoa.Endereco.Complemento.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Complemento,
                            Endereco = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Logradouro) ? retorno.Objeto.Pessoa.Endereco.Logradouro.Replace("*", "") : retorno.Objeto.Pessoa.Endereco.Logradouro,
                            Numero = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.Endereco.Numero) ? retorno.Objeto.Pessoa.Endereco.Numero.Replace("*", "") : "S/N",
                            TelefonePrincipal = Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Length > 0 ? Utilidades.String.OnlyNumbers(retorno.Objeto.Pessoa.Endereco.Telefone).Substring(0, 10) : string.Empty,
                            Fantasia = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.NomeFantasia) ? retorno.Objeto.Pessoa.NomeFantasia.Replace("*", "") : retorno.Objeto.Pessoa.NomeFantasia,
                            Nome = !string.IsNullOrWhiteSpace(retorno.Objeto.Pessoa.RazaoSocial) ? retorno.Objeto.Pessoa.RazaoSocial.Replace("*", "") : retorno.Objeto.Pessoa.RazaoSocial,
                            Cidades = from obj in repLocalidade.BuscarPorUF(localidade != null ? localidade.Estado.Sigla : string.Empty, this.EmpresaUsuario.Codigo)
                                      select new
                                      {
                                          obj.Codigo,
                                          obj.Descricao
                                      },
                            EnderecoDigitado = true,
                            InscricaoEstadual = retorno.Objeto.Pessoa.RGIE
                        };

                        // Fixa tamanho da string
                        retornoDados.Bairro = LimitarTamanho(retornoDados.Bairro, 40);
                        retornoDados.Endereco = LimitarTamanho(retornoDados.Endereco, 80);
                        retornoDados.Numero = LimitarTamanho(retornoDados.Numero, 60);
                        retornoDados.Fantasia = LimitarTamanho(retornoDados.Fantasia, 80);
                        retornoDados.Nome = LimitarTamanho(retornoDados.Nome, 80);

                        return Json(retornoDados, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, retorno.Objeto.MensagemReceita);
                    }
                }
                else
                {
                    return Json<bool>(false, false, retorno.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao carregar dados do sintegra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string tipoArquivo = Request.Params["TipoArquivo"];

                double cpfCnpjCliente;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Cliente"]), out cpfCnpjCliente);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Cliente cliente = null;

                if (cpfCnpjCliente > 0)
                    cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                IList<Dominio.ObjetosDeValor.Relatorios.RelatorioClientes> listaClientes = repCliente.BuscarRemetentesPorCTes(this.Usuario.Series.Select(o => o.Codigo).ToArray(), dataInicial, dataFinal, cliente != null ? cliente.CPF_CNPJ_SemFormato : string.Empty);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Cliente", cliente != null ? cliente.Nome : string.Empty));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Clientes", listaClientes));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioClientes.rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioClientes." + arquivo.FileNameExtension);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        private string LimitarTamanho(string str, int limit)
        {
            if (str.Length > limit) str = str.Substring(0, limit);

            return str;
        }

        private void ConverterObjetoRetornoWS(ref ConsultaCNPJ.RetornoOfConsultaReceitaPessoaJuridicaDggAjPvf retorno, dynamic retornoReceitaWS)
        {
            if (!string.IsNullOrWhiteSpace((string)retornoReceitaWS.status) && !string.IsNullOrWhiteSpace((string)retornoReceitaWS.message))
            {
                retorno.Status = false;
                retorno.CodigoMensagem = 0;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyy");
                retorno.Mensagem = (string)retornoReceitaWS.message;
                retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
                retorno.Objeto.ConsultaValida = false;
                retorno.Objeto.MensagemReceita = (string)retornoReceitaWS.message;
            }
            else
            {
                retorno.Status = true;
                retorno.CodigoMensagem = 1;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyy");
                retorno.Mensagem = "Sucesso";
                retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
                retorno.Objeto.ConsultaValida = true;
                retorno.Objeto.MensagemReceita = "Sucesso";
                retorno.Objeto.Pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                retorno.Objeto.Pessoa.AtualizarEnderecoPessoa = true;
                retorno.Objeto.Pessoa.ClienteExterior = false;

                retorno.Objeto.Pessoa.CNAE = "";
                if (retornoReceitaWS.atividade_principal.Count > 0)
                    retorno.Objeto.Pessoa.CNAE = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.atividade_principal[0].code);

                retorno.Objeto.Pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                retorno.Objeto.Pessoa.Endereco.Bairro = ((string)retornoReceitaWS.bairro).ToUpper();
                retorno.Objeto.Pessoa.Endereco.CEP = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.cep);
                retorno.Objeto.Pessoa.Endereco.CEPSemFormato = (string)retornoReceitaWS.cep;

                retorno.Objeto.Pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                retorno.Objeto.Pessoa.Endereco.Cidade.Descricao = ((string)retornoReceitaWS.municipio).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Cidade.SiglaUF = ((string)retornoReceitaWS.uf).ToUpper();

                retorno.Objeto.Pessoa.Endereco.Complemento = ((string)retornoReceitaWS.complemento).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Logradouro = ((string)retornoReceitaWS.logradouro).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Numero = ((string)retornoReceitaWS.numero).ToUpper();
                retorno.Objeto.Pessoa.Endereco.Telefone = Utilidades.String.OnlyNumbers((string)retornoReceitaWS.telefone);

                retorno.Objeto.Pessoa.NomeFantasia = ((string)retornoReceitaWS.fantasia).ToUpper();
                retorno.Objeto.Pessoa.RazaoSocial = ((string)retornoReceitaWS.nome).ToUpper();
                retorno.Objeto.Pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                retorno.Objeto.Pessoa.RGIE = "";
            }

        }
    }
}
