using Dominio.Interfaces.Database;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos
{
    public class NFSeENotas : ServicoBase
    {
        public NFSeENotas(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Globais

        public async Task<string> SalvarEmpresa(int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            try
            {
                if (empresa != null)
                {
                    string apiKey = empresa.Configuracao.NFSeKeyENotas != null ? empresa.Configuracao.NFSeKeyENotas : empresa.EmpresaPai.Configuracao.NFSeKeyENotas;
                    string url = empresa.Configuracao.NFSeURLENotas != null ? empresa.Configuracao.NFSeURLENotas : empresa.EmpresaPai.Configuracao.NFSeURLENotas;

                    var dadosEmpresa = new
                    {
                        endereco = new
                        {
                            codigoIbgeUf = empresa.Localidade.Estado.CodigoIBGE,
                            codigoIbgeCidade = empresa.Localidade.CodigoIBGE,
                            pais = empresa.Localidade.Pais.Nome,
                            uf = empresa.Localidade.Estado.Sigla,
                            cidade = empresa.Localidade.Descricao,
                            logradouro = empresa.Endereco,
                            numero = empresa.Numero,
                            complemento = empresa.Complemento,
                            bairro = empresa.Bairro,
                            cep = empresa.CEP
                        },
                        id = !string.IsNullOrWhiteSpace(empresa.NFSeIDENotas) ? empresa.NFSeIDENotas : string.Empty,
                        //status = string.Empty,
                        //prazo = 0,
                        //dadosObrigatoriosPreenchidos = true,
                        cnpj = empresa.CNPJ,
                        inscricaoMunicipal = empresa.InscricaoMunicipal,
                        inscricaoEstadual = empresa.InscricaoEstadual,
                        razaoSocial = empresa.RazaoSocial,
                        nomeFantasia = empresa.NomeFantasia,
                        optanteSimplesNacional = empresa.OptanteSimplesNacional,
                        email = empresa.Email,
                        enviarEmailCliente = empresa.StatusEmail == "A" ? true : false,
                        telefoneComercial = empresa.Telefone,
                        incentivadorCultural = false,
                        //regimeEspecialTributacao = empresa.RegimeEspecial.ToString("g"),
                        //aedf = string.Empty,
                        //codigoServicoMunicipal = empresa.Configuracao.ServicoNFSe != null ? empresa.Configuracao.ServicoNFSe.Numero : "",
                        //itemListaServicoLC116 = string.Empty,
                        cnae = empresa.CNAE
                        //aliquotaIss = 0,
                        //descricaoServico = string.Empty
                    };

                    var json = JsonConvert.SerializeObject(dadosEmpresa);

                    var client = HttpClientFactoryWrapper.GetClient(nameof(NFSeENotas));
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + apiKey);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        var endPointUrl = string.Format("v1/empresas");

                        HttpResponseMessage response = client.PostAsync(endPointUrl, content).Result;
                        var jsonResult = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            if (!string.IsNullOrWhiteSpace(jsonResult))
                            {
                                dynamic errorResult = JsonConvert.DeserializeObject(jsonResult);

                                foreach (var msg in errorResult)
                                {
                                    return "Não foi possível atualizar empresa para emissão de NFSe: " + msg.codigo + " " + msg.mensagem;
                                }
                            }
                            else
                                return "Não foi possível atualizar empresa para emissão de NFSe.";
                        }
                        else
                        {
                            dynamic successResult = JsonConvert.DeserializeObject(jsonResult);

                            foreach (var result in successResult)
                            {
                                empresa.NFSeIDENotas = ((Newtonsoft.Json.Linq.JValue)(((Newtonsoft.Json.Linq.JProperty)result).Value)).Value.ToString();
                                repEmpresa.Atualizar(empresa);

                                return "";
                            }
                        }
                    }

                    return "";
                }
                else
                    return "Empresa não localizada. ";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao atualizar empresa para emissão de NFSe enotas : " + ex);

                return "Não foi possível atualizar empresa para emissão de NFSe.";
            }
        }

        public async Task<string> EnviarCertificadoDigital(int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            try
            {
                if (empresa != null)
                {
                    string apiKey = empresa.Configuracao.NFSeKeyENotas != null ? empresa.Configuracao.NFSeKeyENotas : empresa.EmpresaPai.Configuracao.NFSeKeyENotas;
                    string url = empresa.Configuracao.NFSeURLENotas != null ? empresa.Configuracao.NFSeURLENotas : empresa.EmpresaPai.Configuracao.NFSeURLENotas;

                    var dados = new
                    {
                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(empresa.NomeCertificado),
                        senha = empresa.SenhaCertificado
                    };

                    var json = JsonConvert.SerializeObject(dados);

                    var client = HttpClientFactoryWrapper.GetClient(nameof(NFSeENotas));
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Add("Authorization", "Basic " + apiKey);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        var endPointUrl = string.Format("v1/empresas/{0}/certificadoDigital", empresa.NFSeIDENotas);

                        HttpResponseMessage response = client.PostAsync(endPointUrl, content).Result;
                        var jsonResult = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            if (!string.IsNullOrWhiteSpace(jsonResult))
                            {
                                dynamic errorResult = JsonConvert.DeserializeObject(jsonResult);

                                foreach (var msg in errorResult)
                                {
                                    return "Não foi possível enviar certificado digital para emissão de NFSe: " + msg.codigo + " " + msg.mensagem;
                                }
                            }
                            else
                                return "Não foi possível enviar certificado digital para emissão de NFSe.";
                        }
                        else
                        {
                            dynamic successResult = JsonConvert.DeserializeObject(jsonResult);

                            foreach (var result in successResult)
                            {
                                empresa.NFSeIDENotas = ((Newtonsoft.Json.Linq.JValue)(((Newtonsoft.Json.Linq.JProperty)result).Value)).Value.ToString();
                                repEmpresa.Atualizar(empresa);

                                return "";
                            }
                        }
                    }

                    return "";
                }
                else
                    return "Empresa não localizada. ";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao enviar certificado digital para emissão de NFSe enotas : " + ex);

                return "Não foi possível enviar certificado digital para emissão de NFSe.";
            }
        }

        public async Task<string> EnviarNFSe(int codigoNFSe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unitOfWork);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);
            List<Dominio.Entidades.ItemNFSe> itens = repItem.BuscarPorNFSe(nfse.Codigo);

            try
            {
                string apiKey = nfse.Empresa.Configuracao.NFSeKeyENotas != null ? nfse.Empresa.Configuracao.NFSeKeyENotas : nfse.Empresa.EmpresaPai.Configuracao.NFSeKeyENotas;
                string url = nfse.Empresa.Configuracao.NFSeURLENotas != null ? nfse.Empresa.Configuracao.NFSeURLENotas : nfse.Empresa.EmpresaPai.Configuracao.NFSeURLENotas;

                var dadosNFe = new
                {
                    cliente = new
                    {
                        endereco = new
                        {
                            pais = nfse.Tomador.Pais != null ? nfse.Tomador.Pais.Nome : "Brasil",
                            uf = nfse.Tomador.Localidade.Estado.Sigla,
                            cidade = nfse.Tomador.Localidade.Descricao,
                            logradouro = nfse.Tomador.Endereco,
                            numero = nfse.Tomador.Numero,
                            complemento = nfse.Tomador.Complemento,
                            bairro = nfse.Tomador.Bairro,
                            cep = nfse.Tomador.CEP
                        },
                        tipoPessoa = nfse.Tomador.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? "F" : "J",
                        nome = nfse.Tomador.Nome,
                        email = nfse.Tomador.Email,
                        cpfCnpj = nfse.Tomador.CPF_CNPJ,
                        inscricaoMunicipal = nfse.Tomador.InscricaoMunicipal,
                        inscricaoEstadual = nfse.Tomador.IE_RG,
                        telefone = nfse.Tomador.Telefone1
                    },
                    enviarPorEmail = nfse.Tomador.EmailStatus,
                    id = string.Empty,
                    ambienteEmissao = nfse.Ambiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "Homologacao" : "Producao",
                    tipo = "NFS-e",
                    idExterno = nfse.Codigo.ToString(),
                    consumidorFinal = false,
                    indicadorPresencaConsumidor = string.Empty,
                    servico = new
                    {
                        descricao = string.Concat(itens.FirstOrDefault().Servico.Descricao, !string.IsNullOrWhiteSpace(itens.FirstOrDefault().Discriminacao) ? itens.FirstOrDefault().Discriminacao : string.Empty),
                        aliquotaIss = itens.FirstOrDefault().AliquotaISS,
                        issRetidoFonte = nfse.ISSRetido,
                        cnae = !string.IsNullOrWhiteSpace(itens.FirstOrDefault().Servico.CNAE) ? itens.FirstOrDefault().Servico.CNAE : nfse.Empresa.CNAE,
                        codigoServicoMunicipio = itens.FirstOrDefault().Servico.Numero,
                        descricaoServicoMunicipio = itens.FirstOrDefault().Servico.Descricao,
                        itemListaServicoLC116 = itens.FirstOrDefault().Servico.Numero,
                        ufPrestacaoServico = itens.FirstOrDefault().MunicipioIncidencia.Estado.Sigla,
                        municipioPrestacaoServico = itens.FirstOrDefault().MunicipioIncidencia.CodigoIBGE.ToString(),
                        valorCofins = nfse.ValorCOFINS,
                        valorCsll = nfse.ValorCSLL,
                        valorInss = nfse.ValorINSS,
                        valorIr = nfse.ValorIR,
                        valorPis = nfse.ValorPIS
                    },
                    valorTotal = nfse.ValorServicos,
                    idExternoSubstituir = string.Empty,
                    nfeIdSubstitituir = string.Empty
                };

                var json = JsonConvert.SerializeObject(dadosNFe);

                var client = HttpClientFactoryWrapper.GetClient(nameof(NFSeENotas));
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + apiKey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var endPointUrl = string.Format("v1/empresas/{0}/nfes", nfse.Empresa.NFSeIDENotas);

                    HttpResponseMessage response = client.PostAsync(endPointUrl, content).Result;
                    var jsonResult = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        this.ObterRPS(ref nfse, unitOfWork);
                        repNFSe.Atualizar(nfse);

                        dynamic errorResult = JsonConvert.DeserializeObject(jsonResult);

                        foreach (var msg in errorResult)
                            return "Não foi possível emitir a NFSe: " + msg.codigo + " " + msg.mensagem;
                    }
                    else
                    {
                        dynamic successResult = JsonConvert.DeserializeObject(jsonResult);

                        nfse.IDEnotas = successResult.nfeId;
                        this.ObterRPS(ref nfse, unitOfWork);
                        repNFSe.Atualizar(nfse);

                        return "";
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao emitir NFSe enotas: " + ex);

                nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                nfse.RPS.Status = "R";
                nfse.RPS.MensagemRetorno = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - ERRO: Prefeitura indisponível no momento. Tente novamente.");

                repNFSe.Atualizar(nfse);

                return "Não foi possível emitir NFSe.";
            }
        }

        public async Task<string> ConsultarNFSeAsync(int codigoNFSe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unitOfWork);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);
            List<Dominio.Entidades.ItemNFSe> itens = repItem.BuscarPorNFSe(nfse.Codigo);

            try
            {
                string apiKey = nfse.Empresa.Configuracao.NFSeKeyENotas != null ? nfse.Empresa.Configuracao.NFSeKeyENotas : nfse.Empresa.EmpresaPai.Configuracao.NFSeKeyENotas;
                string url = nfse.Empresa.Configuracao.NFSeURLENotas != null ? nfse.Empresa.Configuracao.NFSeURLENotas : nfse.Empresa.EmpresaPai.Configuracao.NFSeURLENotas;

                var client = HttpClientFactoryWrapper.GetClient(nameof(NFSeENotas));
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + apiKey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var endPointUrl = string.Format("v1/empresas/{0}/nfes/{1}", nfse.Empresa.NFSeIDENotas, nfse.IDEnotas);

                HttpResponseMessage response = client.GetAsync(endPointUrl).Result;
                var jsonResult = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (!String.IsNullOrWhiteSpace(jsonResult))
                    {
                        dynamic errorResult = JsonConvert.DeserializeObject(jsonResult);

                        foreach (var msg in errorResult)
                            return "Não foi possível consultar a NFSe: " + msg.codigo + " " + msg.mensagem;
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(jsonResult))
                    {
                        dynamic successResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResult);
                        string statusProcessamento = string.Empty;

                        Dominio.ObjetosDeValor.NFSeENotas.NFSe nfseRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.NFSeENotas.NFSe>(jsonResult);

                        if (nfseRetorno != null)
                        {
                            if (nfseRetorno.status == "Autorizada" || nfseRetorno.status == "Negada" || nfseRetorno.status == "Cancelada" || nfseRetorno.status == "CancelamentoNegado")
                                AtualizarRetornoNFSe(nfse, nfseRetorno, unitOfWork);
                            else
                                return "NFSe código "+ nfseRetorno.idExterno+ " não processada: " + nfseRetorno.status;
                        }
                        else
                            return "NFSe não disponível.";

                        return "";
                    }
                    else
                        return "NFSe não disponível.";
                }

                return "";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao consultar XML NFSe enotas: " + ex);

                return "Não foi possível obter XML da NFSe.";
            }
        }

        public async Task<string> ConsultarXMLNFSe(int codigoNFSe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unitOfWork);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);
            List<Dominio.Entidades.ItemNFSe> itens = repItem.BuscarPorNFSe(nfse.Codigo);

            try
            {
                string apiKey = nfse.Empresa.Configuracao.NFSeKeyENotas != null ? nfse.Empresa.Configuracao.NFSeKeyENotas : nfse.Empresa.EmpresaPai.Configuracao.NFSeKeyENotas;
                string url = nfse.Empresa.Configuracao.NFSeURLENotas != null ? nfse.Empresa.Configuracao.NFSeURLENotas : nfse.Empresa.EmpresaPai.Configuracao.NFSeURLENotas;

                var client = HttpClientFactoryWrapper.GetClient(nameof(NFSeENotas));
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + apiKey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var endPointUrl = string.Format("v1/empresas/{0}/nfes/{1}/xml", nfse.Empresa.NFSeIDENotas, nfse.IDEnotas);

                HttpResponseMessage response = client.GetAsync(endPointUrl).Result;
                var jsonResult = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (!String.IsNullOrWhiteSpace(jsonResult))
                    {
                        dynamic errorResult = JsonConvert.DeserializeObject(jsonResult);

                        foreach (var msg in errorResult)
                            return "Não foi possível obter XML da NFSe: " + msg.codigo + " " + msg.mensagem;
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(jsonResult))
                    {
                        dynamic successResult = JsonConvert.DeserializeObject(jsonResult);

                        if (successResult != null && !string.IsNullOrWhiteSpace(successResult.Result))
                        {
                            Repositorio.XMLNFSe repXML = new Repositorio.XMLNFSe(unitOfWork);
                            Dominio.Entidades.XMLNFSe xml = repXML.BuscarPorNFSe(nfse.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao);

                            if (xml == null)
                                xml = new Dominio.Entidades.XMLNFSe();

                            xml.NFSe = nfse;
                            xml.Tipo = Dominio.Enumeradores.TipoXMLNFSe.Autorizacao;
                            xml.XML = successResult.Result;

                            if (xml.Codigo > 0)
                                repXML.Atualizar(xml);
                            else
                                repXML.Inserir(xml);
                        }

                        return "";
                    }
                    else
                        return "XML não disponível.";

                }
                return "";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao consultar XML NFSe enotas: " + ex);

                return "Não foi possível obter XML da NFSe.";
            }
        }

        #endregion

        #region Métodos Privados

        private void AtualizarRetornoNFSe(Dominio.Entidades.NFSe nfse, Dominio.ObjetosDeValor.NFSeENotas.NFSe nfseRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            if (nfseRetorno.status == "Autorizada")
            {
                nfse.Status = Dominio.Enumeradores.StatusNFSe.Autorizado;
                nfse.CodigoVerificacao = nfseRetorno.codigoVerificacao;
                nfse.Numero = !string.IsNullOrWhiteSpace(nfseRetorno.numero) ? int.Parse(Utilidades.String.OnlyNumbers(nfseRetorno.numero)) : nfse.Numero;
                nfse.RPS.Status = "A";
                nfse.RPS.CodigoRetorno = "";
                nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(nfseRetorno.motivoStatus));
                nfse.RPS.Protocolo = "";
                nfse.RPS.Data = DateTime.Today;

                var retornoXML = this.ConsultarXMLNFSe(nfse.Codigo, unitOfWork);
                if (retornoXML != null && !string.IsNullOrWhiteSpace(retornoXML.Result))
                    Servicos.Log.TratarErro("Problemas ao consultar XML NFSe enotas: " + retornoXML.Result);
            }
            else if (nfseRetorno.status == "Negada")
            {
                nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                nfse.CodigoVerificacao = nfseRetorno.codigoVerificacao;
                nfse.Numero = !string.IsNullOrWhiteSpace(nfseRetorno.numero) ? int.Parse(Utilidades.String.OnlyNumbers(nfseRetorno.numero)) : nfse.Numero;
                nfse.RPS.Status = "R";
                nfse.RPS.CodigoRetorno = "";
                nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(nfseRetorno.motivoStatus));
                nfse.RPS.Protocolo = "";
                nfse.RPS.Data = DateTime.Today;
            }

            repNFSe.Atualizar(nfse);
        }

        private void ObterRPS(ref Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            if (nfse.RPS == null)
            {
                Dominio.Entidades.RPSNFSe rpsNFSe = new Dominio.Entidades.RPSNFSe();

                rpsNFSe.Empresa = nfse.Empresa;
                rpsNFSe.Numero = repRPS.ObterUltimoNumero(nfse.Empresa.Codigo) + 1;
                rpsNFSe.Serie = nfse.Empresa.Configuracao != null ? nfse.Empresa.Configuracao.SerieRPSNFSe : string.Empty;
                rpsNFSe.Status = "P";

                repRPS.Inserir(rpsNFSe);

                nfse.RPS = rpsNFSe;
            }
            else
            {
                nfse.RPS.Serie = nfse.Empresa.Configuracao != null ? nfse.Empresa.Configuracao.SerieRPSNFSe : string.Empty;
                nfse.RPS.Status = "P";
            }

            ServicoNFSe.RPSNFSe rps = new ServicoNFSe.RPSNFSe();

            rps.Emitente = this.ObterEmpresaEmitente(nfse.Empresa);
            rps.Numero = nfse.RPS.Numero;
            rps.Serie = nfse.RPS.Serie;
        }

        private ServicoNFSe.Empresa ObterEmpresaEmitente(Dominio.Entidades.Empresa empresa)
        {
            ServicoNFSe.Empresa empresaEmitente = new ServicoNFSe.Empresa();

            empresaEmitente.Bairro = Utilidades.String.Left(empresa.Bairro, 60);
            empresaEmitente.Cep = Utilidades.String.OnlyNumbers(empresa.CEP);
            empresaEmitente.Cidade = Utilidades.String.Left(empresa.Localidade.Descricao, 60);
            empresaEmitente.CNPJ = Utilidades.String.OnlyNumbers(empresa.CNPJ);
            empresaEmitente.CodigoCidadeIBGE = empresa.Localidade.CodigoIBGE;
            empresaEmitente.Complemento = Utilidades.String.Left(empresa.Complemento, 60);
            empresaEmitente.EmailContador = empresa.EmailContador;
            empresaEmitente.EmailEmitente = empresa.Email;
            empresaEmitente.EnviaEmailContador = empresa.StatusEmail;
            empresaEmitente.EnviaEmailEmitente = empresa.StatusEmailContador;
            empresaEmitente.IE = string.IsNullOrWhiteSpace(empresa.InscricaoEstadual) ? "ISENTO" : empresa.InscricaoEstadual;
            empresaEmitente.IM = empresa.InscricaoMunicipal;
            empresaEmitente.SenhaNFSe = empresa.Configuracao != null ? empresa.Configuracao.SenhaNFSe : string.Empty;
            empresaEmitente.FraseSecretaNFSe = empresa.Configuracao != null ? empresa.Configuracao.FraseSecretaNFSe : string.Empty;
            empresaEmitente.Logradouro = Utilidades.String.Left(empresa.Endereco, 255);
            empresaEmitente.NomeContador = Utilidades.String.Left(empresa.NomeContador, 60);
            empresaEmitente.NomeFantasia = Utilidades.String.Left(empresa.NomeFantasia, 60);
            empresaEmitente.NomeRazao = Utilidades.String.Left(empresa.RazaoSocial, 60);
            empresaEmitente.Numero = Utilidades.String.Left(empresa.Numero, 60);
            empresaEmitente.Status = empresa.Status;
            empresaEmitente.Telefone = Utilidades.String.OnlyNumbers(empresa.Telefone);
            empresaEmitente.TelefoneContador = Utilidades.String.OnlyNumbers(empresa.TelefoneContador);
            empresaEmitente.UF = empresa.Localidade.Estado.Sigla;

            return empresaEmitente;
        }

        #endregion
    }
}
