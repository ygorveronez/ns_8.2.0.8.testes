using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos
{
    public class TruckPad : ServicoBase
    {        
        public TruckPad(Repositorio.UnitOfWork unidadeDeTrabalho) : base(unidadeDeTrabalho) { }

        #region Métodos Publicos 

        public bool AbrirCIOT(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                string authorization = string.Empty;
                string companyID = string.Empty;

                if (this.Login(ciot, ref authorization, ref companyID, unidadeDeTrabalho))
                {
                    this.CriarCIOT(ciot, authorization, companyID, unidadeDeTrabalho);

                    return true;
                }
                else
                    throw new Exception("Não foi possível fazer login na TruckPad.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "TruckPad");
                throw new Exception(ex.Message);
            }
        }

        public bool EncerrarCIOTAberto(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                string authorization = string.Empty;
                string companyID = string.Empty;

                if (this.Login(ciot, ref authorization, ref companyID, unidadeDeTrabalho))
                {
                    this.EncerrarCIOT(ciot, authorization, companyID, unidadeDeTrabalho);

                    return true;
                }
                else
                    throw new Exception("Não foi possível fazer login na TruckPad.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "TruckPad");
                throw new Exception(ex.Message);
            }
        }

        public void CancelarCIOTAberto(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                string authorization = string.Empty;
                string companyID = string.Empty;

                if (this.Login(ciot, ref authorization, ref companyID, unidadeDeTrabalho))
                {
                    this.CancelarCIOT(ciot, authorization, companyID, unidadeDeTrabalho);
                }
                else
                    throw new Exception("Não foi possível fazer login na TruckPad.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "TruckPad");
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Métodos Privados 

        private bool Login(Dominio.Entidades.CIOTSigaFacil ciot, ref string authorization, ref string companyID, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.ObjetosDeValor.TruckPad.Login login = new Dominio.ObjetosDeValor.TruckPad.Login();
            login.user_email = ciot.Empresa.Configuracao.TruckPadUser; //  "infra@multisoftware.com.br";
            login.user_password = ciot.Empresa.Configuracao.TruckPadPassword; // "multi@testes";
            string endPoint = ciot.Empresa.Configuracao.TruckPadURL + "/login/external"; //"https://ciot.api.staging.truckpay.io"

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(TruckPad));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            string mensagemErro = string.Empty;

            try
            {
                jsonRequest = JsonConvert.SerializeObject(login, Formatting.Indented);
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoDados = JsonConvert.DeserializeObject<dynamic>(retorno);

                        if (retornoDados == null)
                            throw new ServicoException("Login não retornou informações, contate a TruckPad.");

                        companyID = (string)retornoDados.data.company_id;
                        authorization = (string)retornoDados.data.token;

                        if (string.IsNullOrWhiteSpace(companyID))
                            throw new ServicoException("Company Id não retornado, contate a TruckPad.");

                        if (string.IsNullOrWhiteSpace(authorization))
                            throw new ServicoException("Token não retornado, contate a TruckPad.");

                        return true;
                    }
                    else
                    {
                        throw new ServicoException("Login não teve retorno de sucesso, contate a TruckPad.");
                    }
                }
                else
                {
                    throw new ServicoException("Login não teve retorno de sucesso, contate a TruckPad.");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "TruckPad");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "TruckPad");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "TruckPad");
                throw new Exception(excecao.Message);
            }
        }

        private bool CriarCIOT(Dominio.Entidades.CIOTSigaFacil ciot, string authorization, string companyID, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = ciot.Empresa.Configuracao.TruckPadURL + "/ciot/emissor";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(TruckPad));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", authorization);
            client.DefaultRequestHeaders.Add("company", companyID);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            string mensagemErro = string.Empty;

            try
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
                Dominio.Entidades.DadosCliente dadosTransportador = repDadosCliente.Buscar(ciot.Empresa.Codigo, ciot.Transportador.CPF_CNPJ);

                Dominio.ObjetosDeValor.TruckPad.CIOT ciotTruckPad = new Dominio.ObjetosDeValor.TruckPad.CIOT();
                ciotTruckPad.contractor_operation_type = "3"; //3 = TAC-Agregado
                ciotTruckPad.date_departure = ciot.DataInicioViagem.ToString("dd/MM/yyyy");
                ciotTruckPad.date_finish = ciot.DataTerminoViagem.ToString("dd/MM/yyyy");
                ciotTruckPad.toll_value = "0.00"; //Valor Pedágio
                ciotTruckPad.freight_value_gross = "0.00"; //Valor Frete Liquido
                ciotTruckPad.fare_amount = "0.00";
                ciotTruckPad.fare_quantity = "0";
                ciotTruckPad.hired = new List<Dominio.ObjetosDeValor.TruckPad.Contratado>();

                Dominio.ObjetosDeValor.TruckPad.Contratado contratado = new Dominio.ObjetosDeValor.TruckPad.Contratado();
                contratado.hired_type = "1"; //1 = Contratado                
                contratado.hired_doc = new List<Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento>();
                Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento documento = new Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento();
                documento.hired_doc_type = ciot.Transportador.Tipo == "J" ? "1" : "2";
                documento.hired_doc_number = ciot.Transportador.CPF_CNPJ_SemFormato;
                contratado.hired_doc.Add(documento);

                Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento documentoRNTRC = new Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento();
                documentoRNTRC.hired_doc_type = ciot.Transportador.Tipo == "J" ? "6" : "5";
                documentoRNTRC.hired_doc_number = string.Format("{0:00000000}", ciot.Veiculo.RNTRC);
                contratado.hired_doc.Add(documentoRNTRC);

                contratado.hired_name = ciot.Transportador.Nome;
                contratado.hired_account_bank = dadosTransportador?.Banco?.Numero.ToString() ?? string.Empty;
                contratado.hired_account_agency = dadosTransportador?.Agencia ?? string.Empty;
                contratado.hired_account_agency_digit = dadosTransportador?.DigitoAgencia ?? string.Empty;
                contratado.hired_account_number = dadosTransportador?.NumeroConta ?? string.Empty;
                contratado.hired_account_type = dadosTransportador == null ? string.Empty : dadosTransportador.TipoConta == Dominio.ObjetosDeValor.Enumerador.TipoConta.Corrente ? "1" : "2";
                contratado.hired_company_doc_number = ciot.Empresa.CNPJ_SemFormato;
                ciotTruckPad.hired.Add(contratado);

                //Dominio.ObjetosDeValor.TruckPad.Contratado subContratante = new Dominio.ObjetosDeValor.TruckPad.Contratado();
                //subContratante.hired_type = "2"; //Sub-contratante
                //ciotTruckPad.hired.Add(subContratante);

                Dominio.ObjetosDeValor.TruckPad.Contratado motorista = new Dominio.ObjetosDeValor.TruckPad.Contratado();
                motorista.hired_type = "3"; //3 = motorista                
                motorista.hired_doc = new List<Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento>();
                Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento documentoMotorista = new Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento();
                documentoMotorista.hired_doc_type = "2";
                documentoMotorista.hired_doc_number = ciot.Motorista.CPF_Formatado;
                motorista.hired_doc.Add(documentoMotorista);
                motorista.hired_name = ciot.Motorista.Nome;
                motorista.hired_account_bank = ciot.Motorista.Banco?.Numero.ToString() ?? string.Empty;
                motorista.hired_account_agency = ciot.Motorista.Agencia ?? string.Empty;
                motorista.hired_account_agency_digit = ciot.Motorista.DigitoAgencia ?? string.Empty;
                motorista.hired_account_number = ciot.Motorista.NumeroConta ?? string.Empty;
                motorista.hired_account_type = ciot.Motorista.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente ? "1" : "2";
                motorista.hired_company_doc_number = ciot.Empresa.CNPJ_SemFormato;
                ciotTruckPad.hired.Add(motorista);

                ciotTruckPad.vehicle = new List<Dominio.ObjetosDeValor.TruckPad.Veiculo>();
                Dominio.ObjetosDeValor.TruckPad.Veiculo veiculo = new Dominio.ObjetosDeValor.TruckPad.Veiculo();
                veiculo.vehicle_plate = ciot.Veiculo.Placa;
                veiculo.vehicle_rntrc = string.Format("{0:00000000}", ciot.Veiculo.RNTRC);
                //veiculo.vehicle_category = string.Empty;

                ciotTruckPad.vehicle.Add(veiculo);

                jsonRequest = JsonConvert.SerializeObject(ciotTruckPad, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoDados = JsonConvert.DeserializeObject<dynamic>(retorno);

                        if (retornoDados == null)
                            throw new ServicoException("Abertura CIOT não retornou informações, contate a TruckPad.");

                        string codigoIdentificacaoOperacao = (string)retornoDados.data[0].id_freight;
                        string codigoVerificador = (string)retornoDados.data[0].protocol_freight;

                        if (string.IsNullOrWhiteSpace(codigoIdentificacaoOperacao))
                            throw new ServicoException("Código Identificacao Operação não retornado, contate a TruckPad.");

                        if (string.IsNullOrWhiteSpace(codigoVerificador))
                            throw new ServicoException("Código Verificador não retornado, contate a TruckPad.");

                        ciot.NumeroCIOT = codigoIdentificacaoOperacao;
                        ciot.CodigoVerificadorCIOT = codigoVerificador;

                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto;
                        ciot.CodigoRetorno = "0";
                        ciot.MensagemRetorno = "Operação realizada com sucesso.";

                        repCIOT.Atualizar(ciot);

                        AdicionaLog(jsonRequest, jsonResponse, ciot, unidadeDeTrabalho);

                        return true;
                    }
                    else
                    {
                        throw new ServicoException("Não teve retorno ao emitir o CIOT, contate a TruckPad.");
                    }
                }
                else
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoDados = JsonConvert.DeserializeObject<dynamic>(retorno);

                        string codigoRetorno = (string)retornoDados.cod_message;
                        if (codigoRetorno.Length == 2)
                        {
                            string mensagemRetorno = (string)retornoDados.message.Rejeicao;
                            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                            {
                                ciot.MensagemRetorno = mensagemRetorno;
                                ciot.CodigoRetorno = codigoRetorno;
                                repCIOT.Atualizar(ciot);

                                throw new ServicoException(mensagemRetorno);
                            }
                        }
                        else
                        {

                            string mensagemRetorno = (string)retornoDados.message.Mensagem;
                            ciot.MensagemRetorno = codigoRetorno + " - " + mensagemRetorno;
                            ciot.CodigoRetorno = "9";
                            repCIOT.Atualizar(ciot);

                            throw new ServicoException(ciot.MensagemRetorno);
                        }
                    }

                    throw new ServicoException("Não teve retorno de sucesso ao emitir o CIOT, contate a TruckPad.");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "TruckPad");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "TruckPad");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "TruckPad");

                AdicionaLog(jsonRequest, jsonResponse, ciot, unidadeDeTrabalho);
                throw new Exception(excecao.Message);
            }
        }

        private bool EncerrarCIOT(Dominio.Entidades.CIOTSigaFacil ciot, string authorization, string companyID, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = ciot.Empresa.Configuracao.TruckPadURL + "/ciot/close";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(TruckPad));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", authorization);
            client.DefaultRequestHeaders.Add("company", companyID);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            string mensagemErro = string.Empty;

            try
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOTSigaFacil = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.DadosCliente dadosTransportador = repDadosCliente.Buscar(ciot.Empresa.Codigo, ciot.Transportador.CPF_CNPJ);
                List<Dominio.Entidades.CTeCIOTSigaFacil> ctesCIOT = repCTeCIOTSigaFacil.BuscarCTesPorCIOT(ciot.Codigo);

                Dominio.ObjetosDeValor.TruckPad.EncerramentoCIOT ciotTruckPad = new Dominio.ObjetosDeValor.TruckPad.EncerramentoCIOT();
                ciotTruckPad.id_freight = ciot.NumeroCIOT;
                ciotTruckPad.protocol_freight = ciot.CodigoVerificadorCIOT;
                ciotTruckPad.contractor_doc_number = ciot.Empresa.CNPJ_Formatado;
                ciotTruckPad.freight_value_gross = ciot.ValorFrete.ToString("n2").Replace(".","").Replace(",",".");
                ciotTruckPad.date_finish = ciot.DataTerminoViagem.ToString("dd/MM/yyyy");
                ciotTruckPad.fare_amount = ciot.ValorTarifaFrete.ToString("n2").Replace(".", "").Replace(",", ".");
                ciotTruckPad.fare_quantity = "1";
                ciotTruckPad.payment_method = "2"; // 1 = IPEF(TruckPad Pay) 2 = Depósito em conta do contratado
                ciotTruckPad.travel_routes = new List<Dominio.ObjetosDeValor.TruckPad.Rota>();

                if (ctesCIOT != null && ctesCIOT.Count > 0)
                {
                    Dominio.ObjetosDeValor.TruckPad.Rota rota = new Dominio.ObjetosDeValor.TruckPad.Rota();
                    rota.source_city_ibge = ctesCIOT.FirstOrDefault().CTe.LocalidadeInicioPrestacao.CodigoIBGE.ToString();
                    rota.source_city = ctesCIOT.FirstOrDefault().CTe.LocalidadeInicioPrestacao.Descricao;
                    rota.source_state = ctesCIOT.FirstOrDefault().CTe.LocalidadeInicioPrestacao.Estado.Descricao;
                    rota.source_cep = ctesCIOT.FirstOrDefault().CTe.Expedidor != null ? ctesCIOT.FirstOrDefault().CTe.Expedidor.CEP : (ctesCIOT.FirstOrDefault().CTe.Remetente?.CEP ?? string.Empty);

                    rota.destination_city_ibge = ctesCIOT.FirstOrDefault().CTe.LocalidadeTerminoPrestacao.CodigoIBGE.ToString();
                    rota.destination_city = ctesCIOT.FirstOrDefault().CTe.LocalidadeTerminoPrestacao.Descricao;
                    rota.destination_state = ctesCIOT.FirstOrDefault().CTe.LocalidadeTerminoPrestacao.Estado.Descricao;
                    rota.destination_cep = ctesCIOT.FirstOrDefault().CTe.Recebedor != null ? ctesCIOT.FirstOrDefault().CTe.Recebedor.CEP : (ctesCIOT.FirstOrDefault().CTe.Destinatario?.CEP ?? string.Empty);

                    rota.load_nature = ciot.NaturezaCarga.CodigoNatureza;
                    rota.load_weight = ciot.PesoBruto.ToString("n2").Replace(".", "").Replace(",", ".");
                    rota.travel_quantity = ctesCIOT.Count.ToString();

                    ciotTruckPad.travel_routes.Add(rota);
                }

                ciotTruckPad.toll_value = ciot.ValorPedagio.ToString("n2").Replace(".", "").Replace(",", ".");
                //ciotTruckPad.fuel_value = .ToString("n2");
                ciotTruckPad.contractor_operation_type = "3"; //3 = TAC-Agregado

                ciotTruckPad.hired = new List<Dominio.ObjetosDeValor.TruckPad.Contratado>();

                Dominio.ObjetosDeValor.TruckPad.Contratado contratado = new Dominio.ObjetosDeValor.TruckPad.Contratado();
                contratado.hired_type = "1"; //1 = Contratado                
                contratado.hired_doc = new List<Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento>();
                Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento documento = new Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento();
                documento.hired_doc_type = ciot.Transportador.Tipo == "J" ? "1" : "2";
                documento.hired_doc_number = ciot.Transportador.CPF_CNPJ_SemFormato;
                contratado.hired_doc.Add(documento);

                Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento documentoRNTRC = new Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento();
                documentoRNTRC.hired_doc_type = ciot.Transportador.Tipo == "J" ? "6" : "5";
                documentoRNTRC.hired_doc_number = string.Format("{0:00000000}", ciot.Veiculo.RNTRC);
                contratado.hired_doc.Add(documentoRNTRC);

                contratado.hired_name = ciot.Transportador.Nome;
                contratado.hired_account_bank = dadosTransportador?.Banco?.Numero.ToString() ?? string.Empty;
                contratado.hired_account_agency = dadosTransportador?.Agencia ?? string.Empty;
                contratado.hired_account_agency_digit = dadosTransportador?.DigitoAgencia ?? string.Empty;
                contratado.hired_account_number = dadosTransportador?.NumeroConta ?? string.Empty;
                contratado.hired_account_type = dadosTransportador == null ? string.Empty : dadosTransportador.TipoConta == Dominio.ObjetosDeValor.Enumerador.TipoConta.Corrente ? "1" : "2";
                contratado.hired_company_doc_number = ciot.Empresa.CNPJ_SemFormato;
                ciotTruckPad.hired.Add(contratado);


                Dominio.ObjetosDeValor.TruckPad.Contratado motorista = new Dominio.ObjetosDeValor.TruckPad.Contratado();
                motorista.hired_type = "3"; //3 = motorista                
                motorista.hired_doc = new List<Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento>();
                Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento documentoMotorista = new Dominio.ObjetosDeValor.TruckPad.ContratadoDocumento();
                documentoMotorista.hired_doc_type = "2";
                documentoMotorista.hired_doc_number = ciot.Motorista.CPF_Formatado;
                motorista.hired_doc.Add(documentoMotorista);
                motorista.hired_name = ciot.Motorista.Nome;
                motorista.hired_account_bank = ciot.Motorista.Banco?.Numero.ToString() ?? string.Empty;
                motorista.hired_account_agency = ciot.Motorista.Agencia ?? string.Empty;
                motorista.hired_account_agency_digit = ciot.Motorista.DigitoAgencia ?? string.Empty;
                motorista.hired_account_number = ciot.Motorista.NumeroConta ?? string.Empty;
                motorista.hired_account_type = ciot.Motorista.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente ? "1" : "2";
                motorista.hired_company_doc_number = ciot.Empresa.CNPJ_SemFormato;
                ciotTruckPad.hired.Add(motorista);

                jsonRequest = JsonConvert.SerializeObject(ciotTruckPad, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PutAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoDados = JsonConvert.DeserializeObject<dynamic>(retorno);

                        if (retornoDados == null)
                            throw new ServicoException("Encerramento não retornou informações, contate a TruckPad.");

                        string mensagem = (string)retornoDados.message;
                        string codigoIdentificacaoOperacao = (string)retornoDados.data[0].CodigoIdentificacaoOperacao;
                        string protocoloEncerramento = (string)retornoDados.data[0].Protocolo;
                        string dataEncerramento = (string)retornoDados.data[0].DataEncerramento;


                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Encerrado;
                        ciot.MensagemRetornoCancelamento = mensagem + " - Data " + dataEncerramento + " - Protocolo " + protocoloEncerramento;
                        ciot.MensagemRetornoCancelamento = mensagem;
                        ciot.CodigoRetornoCancelamento = "";
                        if (!string.IsNullOrWhiteSpace(dataEncerramento))
                            mensagem += " - Data " + dataEncerramento;
                        if (!string.IsNullOrWhiteSpace(protocoloEncerramento))
                            mensagem += " - Protocolo " + protocoloEncerramento;

                        repCIOT.Atualizar(ciot);

                        AdicionaLog(jsonRequest, jsonResponse, ciot, unidadeDeTrabalho);

                        return true;
                    }
                    else
                    {
                        throw new ServicoException("Não teve retorno ao emitir o CIOT, contate a TruckPad.");
                    }
                }
                else
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoDados = JsonConvert.DeserializeObject<dynamic>(retorno);

                        string codigoRetorno = (string)retornoDados.cod_message;

                        var menssage = retornoDados.message;

                        string mensagemRetorno = menssage != null ? (string)menssage.Mensagem : codigoRetorno;
                        if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                        {
                            ciot.MensagemRetornoCancelamento = mensagemRetorno;
                            ciot.CodigoRetornoCancelamento = Utilidades.String.Left(codigoRetorno, 2);
                            repCIOT.Atualizar(ciot);

                            throw new ServicoException(mensagemRetorno);
                        }
                    }

                    throw new ServicoException("Não teve retorno de sucesso ao emitir o CIOT, contate a TruckPad.");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "TruckPad");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "TruckPad");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "TruckPad");

                AdicionaLog(jsonRequest, jsonResponse, ciot, unidadeDeTrabalho);
                throw new Exception(excecao.Message);
            }
        }

        private bool CancelarCIOT(Dominio.Entidades.CIOTSigaFacil ciot, string authorization, string companyID, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = ciot.Empresa.Configuracao.TruckPadURL + "/ciot/cancel";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(TruckPad));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("authorization", authorization);
            client.DefaultRequestHeaders.Add("company", companyID);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            string mensagemErro = string.Empty;

            try
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
                Repositorio.CTeCIOTSigaFacil repCTeCIOTSigaFacil = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);

                Dominio.Entidades.DadosCliente dadosTransportador = repDadosCliente.Buscar(ciot.Empresa.Codigo, ciot.Transportador.CPF_CNPJ);
                List<Dominio.Entidades.CTeCIOTSigaFacil> ctesCIOT = repCTeCIOTSigaFacil.BuscarCTesPorCIOT(ciot.Codigo);

                Dominio.ObjetosDeValor.TruckPad.CancelamentoCIOT ciotTruckPad = new Dominio.ObjetosDeValor.TruckPad.CancelamentoCIOT();
                ciotTruckPad.id_freight = ciot.NumeroCIOT;
                ciotTruckPad.protocol_freight = ciot.CodigoVerificadorCIOT;
                ciotTruckPad.contractor_doc_number = ciot.Empresa.CNPJ_Formatado;
                ciotTruckPad.observation = ciot.MotivoCancelamento;

                jsonRequest = JsonConvert.SerializeObject(ciotTruckPad, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PutAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoDados = JsonConvert.DeserializeObject<dynamic>(retorno);

                        if (retornoDados == null)
                            throw new ServicoException("Cancelamento não retornou informações, contate a TruckPad.");

                        string mensagem = (string)retornoDados.message;
                        string codigoIdentificacaoOperacao = (string)retornoDados.data[0].CodigoIdentificacaoOperacao;
                        string protocolo = (string)retornoDados.data[0].Protocolo;
                        string dataCancelamento = (string)retornoDados.data[0].DataCancelamento;

                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Cancelado;
                        ciot.MensagemRetornoCancelamento = mensagem;
                        if (!string.IsNullOrWhiteSpace(dataCancelamento))
                            mensagem += " - Data " + dataCancelamento;
                        if (!string.IsNullOrWhiteSpace(protocolo))
                            mensagem += " - Protocolo " + protocolo;

                        repCIOT.Atualizar(ciot);

                        AdicionaLog(jsonRequest, jsonResponse, ciot, unidadeDeTrabalho);

                        return true;
                    }
                    else
                    {
                        throw new ServicoException("Não teve retorno ao emitir o CIOT, contate a TruckPad.");
                    }
                }
                else
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic retornoDados = JsonConvert.DeserializeObject<dynamic>(retorno);

                        string codigoRetorno = (string)retornoDados.cod_message;
                        string mensagemRetorno = (string)retornoDados.message;
                        if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                        {
                            ciot.MensagemRetornoCancelamento = mensagemRetorno;
                            ciot.CodigoRetornoCancelamento = codigoRetorno;
                            repCIOT.Atualizar(ciot);

                            throw new ServicoException(mensagemRetorno);
                        }
                    }

                    throw new ServicoException("Não teve retorno de sucesso ao emitir o CIOT, contate a TruckPad.");
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "TruckPad");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "TruckPad");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "TruckPad");

                AdicionaLog(jsonRequest, jsonResponse, ciot, unidadeDeTrabalho);
                throw new Exception(excecao.Message);
            }
        }

        private void AdicionaLog(string jsonRequet, string jsonResult, Dominio.Entidades.CIOTSigaFacil ciot, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CIOTSigaFacilLogXML repCIOTSigaFacilLogXML = new Repositorio.CIOTSigaFacilLogXML(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacilLogXML log = new Dominio.Entidades.CIOTSigaFacilLogXML()
            {
                CIOT = ciot,
                DataHora = DateTime.Now,
                Requisicao = jsonRequet,
                Resposta = jsonResult
            };

            repCIOTSigaFacilLogXML.Inserir(log);
        }

        #endregion

    }
}
