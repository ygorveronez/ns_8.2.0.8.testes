using Servicos.ServicoSigaFacil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Servicos
{
    public class SigaFacil : ServicoBase
    {        
        public SigaFacil(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.TokenSigaFacil GerarToken(int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.TokenSigaFacil repToken = new Repositorio.TokenSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.TokenSigaFacil token = repToken.BuscarPorData(codigoEmpresa, DateTime.Now.Date);

            if (token == null)
            {
                token = new Dominio.Entidades.TokenSigaFacil();

                token.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                token.Data = DateTime.Now.Date;

                string cnpjEmpresa = token.Empresa.CNPJ;

#if DEBUG
                cnpjEmpresa = "13969629000196";
#endif

                ServicoSigaFacil.WSFreteUnikClient svcSigaFacil = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoSigaFacil.WSFreteUnikClient, ServicoSigaFacil.WSFreteUnik>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SigaFacil_WSFreteUnik);

                string strSenha = svcSigaFacil.gerarToken(token.Empresa.Configuracao.CodigoContratanteSigaFacil, cnpjEmpresa);

                if (!string.IsNullOrWhiteSpace(strSenha))
                {
                    token.Token = Criptografia.DescriptografarDES(strSenha, token.Empresa.Configuracao.ChaveCriptograficaSigaFacil);

                    repToken.Inserir(token);
                }
                else
                {
                    throw new Exception("O token de acesso ao Siga Fácil retornou vazio ou nulo.");
                }
            }

            return token;
        }

        public void EmitirCIOT(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);
            Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
            List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = repCTeCIOT.BuscarPorCIOT(codigoCIOT);

            Dominio.Entidades.TokenSigaFacil token = this.GerarToken(ciot.Empresa.Codigo, unidadeDeTrabalho);

            ServicoSigaFacil.WSFreteUnikClient svcSigaFacil = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoSigaFacil.WSFreteUnikClient, ServicoSigaFacil.WSFreteUnik>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SigaFacil_WSFreteUnik);

            string cnpjEmpresa = ciot.Empresa.CNPJ;

#if DEBUG
            cnpjEmpresa = "13969629000196";
#endif

            this.IntegrarTransportador(ciot, token, unidadeDeTrabalho);
            this.IntegrarMotorista(ciot, token, unidadeDeTrabalho);

            ServicoSigaFacil.reqCarregamentoFreteViagemBean viagem = new ServicoSigaFacil.reqCarregamentoFreteViagemBean()
            {
                cdGrupoDocETF = (int)ciot.DocumentosObrigatorios,
                codigoCiot = 0L,
                codigoFilialContratante = 1,
                codigoMunicipioDestino = string.Format("{0:0000000}", ciot.Destino.CodigoIBGE),
                codigoNaturezaCarga = ciot.NaturezaCarga.CodigoNatureza,
                codigoMunicipioOrigem = string.Format("{0:0000000}", ciot.Origem.CodigoIBGE),
                cpfCnpjContratado = ciot.Transportador.CPF_CNPJ_SemFormato,
                cpfCnpjContratante = cnpjEmpresa,
                cpfCnpjDestinatario = ctes.First().CTe.Destinatario.CPF_CNPJ_SemFormato,
                cpfCnpjInteressado = cnpjEmpresa,
                cpfCnpjTransportador = ciot.Transportador.CPF_CNPJ_SemFormato,
                cpfMotorista = ciot.Motorista.CPF,
                data = DateTime.Now.ToString("dd/MM/yyyy"),
                dataFimViagem = ciot.DataTerminoViagem.ToString("dd/MM/yyyy"),
                dataInicioViagem = ciot.DataInicioViagem.ToString("dd/MM/yyyy"),
                filialTransporte = 1,
                flagCliente = "1",
                hora = DateTime.Now.ToString("HH:mm:ss"),
                idTac = (int)ciot.CategoriaTransportador,
                nomeCondutor = Utilidades.String.Left(ciot.Motorista.Nome, 50),
                nrCartaoAssociacao = ciot.NumeroCartaoMotorista,
                nrContrato = "0",
                nsuContratante = new string('0', 3 - ciot.Empresa.Configuracao.CodigoContratanteSigaFacil.Length) + ciot.Empresa.Configuracao.CodigoContratanteSigaFacil + string.Format("{0:00000000000}", ciot.NSU),
                numeroAgencia = "0",
                numeroBanco = "0",
                numeroContaCorrente = "0",
                numeroViagem = ciot.Numero,
                pesoCarga = (from obj in ctes select obj.PesoBruto).Sum().ToString("F2", cultura),
                prAdtoTAC = 0D,
                prAdtoVinculado = 100D,
                prQuitacaoTAC = 0D,
                prQuitacaoVinculado = 100D,
                razaoSocialTransportador = Utilidades.String.Left(ciot.Transportador.Nome, 50),
                regraQuitacaoAdiantamento = ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC ? ciot.RegraAdiantamento == Dominio.Enumeradores.RegraQuitacaoAdiantamento.Filial ? 6 : 4 : ciot.RegraAdiantamento == Dominio.Enumeradores.RegraQuitacaoAdiantamento.Filial ? 3 : 1,
                regraQuitacaoQuitacao = ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC ? ciot.RegraQuitacao == Dominio.Enumeradores.RegraQuitacaoQuitacao.Filial ? 6 : 4 : ciot.RegraQuitacao == Dominio.Enumeradores.RegraQuitacaoQuitacao.Filial ? 3 : 1,
                rntrcContratado = string.Format("{0:00000000}", ciot.Veiculo.RNTRC),
                rntrcContratante = ciot.Empresa.RegistroANTT,
                rntrcTransportador = string.Format("{0:00000000}", ciot.Veiculo.RNTRC),
                tipoViagem = (int)ciot.TipoViagem,
                tipoViagemANTT = 1,
                veiculoOperacaoTransp = new ServicoSigaFacil.veiculoOperacaoTranspBean[] {
                    new ServicoSigaFacil.veiculoOperacaoTranspBean() {
                        placa = ciot.Veiculo.Placa,
                        rntrc = string.Format("{0:00000000}", ciot.Veiculo.RNTRC)
                    }
                },
                listaCTRC = (from obj in ctes
                             where obj.CodigoRetorno != "00"
                             select new ServicoSigaFacil.carregamentoFreteCTRCBean()
                             {
                                 codResposta = "00",
                                 especie = obj.EspecieMercadoria,
                                 exigeDataEntregaPOS = 2,
                                 exigePesoChegada = (int)obj.ExigePesoChegada,
                                 filialConhecimento = 1,
                                 filialTransporte = 1,
                                 idCobraDiferencaFrete = (int)obj.RecalculoFrete,
                                 idTipoQuebra = (int)obj.TipoQuebra,
                                 idTipoTolerancia = (int)obj.TipoTolerancia,
                                 nsuContratante = new string('0', 3 - ciot.Empresa.Configuracao.CodigoContratanteSigaFacil.Length) + ciot.Empresa.Configuracao.CodigoContratanteSigaFacil + string.Format("{0:00000000000}", obj.NSU),
                                 numeroCTRC = obj.CTe.Numero.ToString(),
                                 numeroEmpresa = int.Parse(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil),
                                 numeroNotaFiscal = obj.NumeroNotaFiscal,
                                 numeroSerie = obj.CTe.Serie.Numero.ToString(),
                                 percentTolerancia = obj.PercentualTolerancia,
                                 percentToleranciaSpecified = true,
                                 percentToleranciaSup = obj.PercentualToleranciaSuperior,
                                 percentToleranciaSupSpecified = true,
                                 pesoBruto = obj.PesoBruto,
                                 pesoBrutoSpecified = true,
                                 pesoLotacao = obj.PesoLotacao,
                                 pesoLotacaoSpecified = true,
                                 quantidade = obj.QuantidadeMercadoria,
                                 quantidadeSpecified = true,
                                 tipoPeso = (int)obj.TipoPeso,
                                 valorAdiantamento = obj.ValorAdiantamento,
                                 valorAdiantamentoSpecified = true,
                                 valorCartaoPedagio = -obj.ValorCartaoPedagio,
                                 valorCartaoPedagioSpecified = true,
                                 valorFrete = obj.ValorFrete,
                                 valorFreteSpecified = true,
                                 valorINSS = -obj.ValorINSS,
                                 valorINSSSpecified = true,
                                 valorIRRF = -obj.ValorIRRF,
                                 valorIRRFSpecified = true,
                                 valorMercadoriaKG = obj.ValorMercadoriaKG,
                                 valorMercadoriaKGSpecified = true,
                                 valorOutrosDescontos = -obj.ValorOutrosDescontos,
                                 valorOutrosDescontosSpecified = true,
                                 valorPedagio = -obj.ValorPedagio,
                                 valorPedagioSpecified = true,
                                 valorSeguro = -obj.ValorSeguro,
                                 valorSeguroSpecified = true,
                                 valorSenat = -obj.ValorSENAT,
                                 valorSenatSpecified = true,
                                 valorSest = -obj.ValorSEST,
                                 valorSestSpecified = true,
                                 valorTarifaEmissaoCartao = -obj.ValorTarifaEmissaoCartao,
                                 valorTarifaEmissaoCartaoSpecified = true,
                                 valorTarifaFrete = obj.ValorTarifaFrete,
                                 valorTarifaFreteSpecified = true,
                                 valorTotalMercadoria = obj.ValorTotalMercadoria,
                                 valorTotalMercadoriaSpecified = true,
                             }).ToArray()
            };

            //#if DEBUG

            //            InspectorBehavior inspector = new InspectorBehavior();

            //            svcSigaFacil.Endpoint.EndpointBehaviors.Add(inspector);

            //#endif

            ServicoSigaFacil.respCarregamentoViagemBean retorno = svcSigaFacil.carregarViagemCTRC(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil, token.Token, viagem);

            //#if DEBUG

            //            var xml = inspector.LastRequestXML;

            //#endif

            ciot.CodigoRetorno = retorno.codResposta;
            ciot.MensagemRetorno = retorno.msgErro;
            ciot.NumeroContrato = retorno.nrContrato;
            ciot.NumeroCIOT = retorno.codigoIdentificacaoOperacao;

            if (ciot.CodigoRetorno == "00")
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;
                ciot.MensagemRetorno = "Transação Ok";
            }
            else if (ciot.CodigoRetorno == "99")
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                ciot.MensagemRetorno = "Erro Interno de Processamento";
            }

            repCIOT.Atualizar(ciot);

            if (retorno.listaCTRCResp != null)
            {
                foreach (ServicoSigaFacil.respCarregamentoFreteCTRCBean cteRet in retorno.listaCTRCResp)
                {
                    Dominio.Entidades.CTeCIOTSigaFacil cte = (from obj in ctes where obj.CTe.Numero == int.Parse(cteRet.numeroCTRC) && obj.CTe.Serie.Numero == int.Parse(cteRet.numeroSerie) select obj).FirstOrDefault();

                    cte.CodigoRetorno = cteRet.codResposta;
                    cte.NSUFastCred = cteRet.nsuFast;
                    cte.NumeroContrato = cteRet.nrContrato;

                    repCTeCIOT.Atualizar(cte);
                }
            }
        }

        public void CancelarCIOT(int codigoCIOT, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unidadeDeTrabalho);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            Dominio.Entidades.TokenSigaFacil token = this.GerarToken(ciot.Empresa.Codigo, unidadeDeTrabalho);

            ServicoSigaFacil.WSFreteUnikClient svcSigaFacil = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoSigaFacil.WSFreteUnikClient, ServicoSigaFacil.WSFreteUnik>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SigaFacil_WSFreteUnik);

            ServicoSigaFacil.reqCancelamentoBean cancelamento = new ServicoSigaFacil.reqCancelamentoBean()
            {
                codigoContratante = int.Parse(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil),
                codigoFilial = 1,
                data = DateTime.Now.ToString("dd/MM/yyyy"),
                filialTransporteFilialConhecimento = 1,
                hora = DateTime.Now.ToString("HH:mm:ss"),
                motivoCancelamento = 1,
                nsuContratante = new string('0', 3 - ciot.Empresa.Configuracao.CodigoContratanteSigaFacil.Length) + ciot.Empresa.Configuracao.CodigoContratanteSigaFacil + string.Format("{0:00000000000}", ciot.NSU),
                nsuFast0210 = "0",
                numeroEmpresa = int.Parse(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil),
                numeroOperacao = ciot.NumeroContrato,
                numeroSerie = "0",
                numeroViagemNumeroCTRC = ciot.Numero.ToString(),
                tpCancelamento = 0
            };

            ServicoSigaFacil.respCancelamentoBean retorno = svcSigaFacil.cancelamento(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil, token.Token, cancelamento);

            if (retorno != null)
            {
                ciot.CodigoRetornoCancelamento = retorno.codResposta;
                ciot.MensagemRetornoCancelamento = retorno.msgErro;
                ciot.NumeroContratoCancelamento = retorno.numeroOperacao;

                if (ciot.CodigoRetornoCancelamento == "00")
                {
                    ciot.MensagemRetornoCancelamento = "Transação Ok";
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Cancelado;
                }
                else if (ciot.CodigoRetornoCancelamento == "99")
                    ciot.MensagemRetornoCancelamento = "Erro Interno de Processamento";

            }

            repCIOT.Atualizar(ciot);
        }

        public void IntegrarTransportador(Dominio.Entidades.CIOTSigaFacil ciot, Dominio.Entidades.TokenSigaFacil token, Repositorio.UnitOfWork unitOfWork)
        {
            ServicoSigaFacil.WSFreteUnikClient svcSigaFacil = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoSigaFacil.WSFreteUnikClient, ServicoSigaFacil.WSFreteUnik>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SigaFacil_WSFreteUnik);

            ServicoSigaFacil.respTransportadorBean retornoConsulta = svcSigaFacil.getTransportador(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil, token.Token, ciot.Transportador.CPF_CNPJ_SemFormato);

            if (retornoConsulta.codResposta == "48")
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                ServicoSigaFacil.reqCadTransportadorBean transp = new ServicoSigaFacil.reqCadTransportadorBean();

                transp.bairro = ciot.Transportador.Bairro;
                transp.cep = Utilidades.String.OnlyNumbers(ciot.Transportador.CEP);
                transp.cidade = ciot.Transportador.Localidade.Descricao;
                transp.codigoContratante = int.Parse(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil);
                transp.codigoFilial = 1;
                transp.complemento = ciot.Transportador.Complemento;
                transp.cpfCnpj = ciot.Transportador.CPF_CNPJ_SemFormato;
                transp.data = DateTime.Now.ToString("dd/MM/yyyy");
                transp.endereco = ciot.Transportador.Endereco;
                transp.fantasia = ciot.Transportador.NomeFantasia;
                transp.fisicajuridica = ciot.Transportador.Tipo == "F" ? 0 : 1;
                transp.hora = DateTime.Now.ToString("HH:mm:ss");
                transp.idestado = ciot.Transportador.Localidade.Estado.Sigla;
                transp.ierg = ciot.Transportador.IE_RG;
                transp.nsuContratante = new string('0', 3 - ciot.Empresa.Configuracao.CodigoContratanteSigaFacil.Length) + ciot.Empresa.Configuracao.CodigoContratanteSigaFacil + string.Format("{0:00000000000}", ciot.Empresa.Configuracao.ProximoNSUSigaFacil);
                transp.razaoSocial = ciot.Transportador.Nome;
                transp.rntrc = string.Format("{0:00000000}", ciot.Veiculo.RNTRC);
                transp.telefone = ciot.Transportador.Telefone1;

                ServicoSigaFacil.respCadTransportadorBean retornoCadastro = svcSigaFacil.integrarTransportador(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil, token.Token, transp);

                repEmpresa.Atualizar(ciot.Empresa);
            }
        }

        public void IntegrarMotorista(Dominio.Entidades.CIOTSigaFacil ciot, Dominio.Entidades.TokenSigaFacil token, Repositorio.UnitOfWork unitOfWork)
        {
            ServicoSigaFacil.WSFreteUnikClient svcSigaFacil = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoSigaFacil.WSFreteUnikClient, ServicoSigaFacil.WSFreteUnik>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SigaFacil_WSFreteUnik);

            ServicoSigaFacil.respValidaMotoristaBean retornoConsulta = svcSigaFacil.validaMotorista(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil, token.Token, ciot.Motorista.CPF);

            if (retornoConsulta.codResposta == "48")
            {
                ServicoSigaFacil.reqCadMotoristaBean dadosMotorista = new ServicoSigaFacil.reqCadMotoristaBean();

                dadosMotorista.estadoCivil = 5;
                dadosMotorista.idEscolaridade = 5;
                dadosMotorista.idSexo = 1;
                dadosMotorista.qualificacaoProfissional = 3;
                dadosMotorista.orgaoExpedidorRG = "SSP";
                dadosMotorista.dtExpedicaoRG = DateTime.Now.ToString("dd/MM/yyyy");
                dadosMotorista.numcasa = 1;

                dadosMotorista.bairro = ciot.Transportador.Bairro;
                dadosMotorista.cep = Utilidades.String.OnlyNumbers(ciot.Transportador.CEP);
                dadosMotorista.cidade = ciot.Motorista.Localidade.Descricao;
                dadosMotorista.codigoContratante = int.Parse(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil);
                dadosMotorista.codigoFilial = 1;
                dadosMotorista.complemento = ciot.Motorista.Complemento;
                dadosMotorista.cpf = ciot.Motorista.CPF;
                dadosMotorista.data = DateTime.Now.ToString("dd/MM/yyyy");
                dadosMotorista.dtNascimento = ciot.Motorista.DataNascimento.HasValue ? ciot.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                dadosMotorista.email = ciot.Motorista.Email;
                dadosMotorista.endereco = Utilidades.String.Left(ciot.Motorista.Endereco, 5);
                dadosMotorista.hora = DateTime.Now.ToString("HH:mm:ss");
                dadosMotorista.idEstado = ciot.Motorista.Localidade.Estado.Sigla;
                dadosMotorista.ierg = ciot.Motorista.RG;
                dadosMotorista.nome = ciot.Motorista.Nome;
                dadosMotorista.nsuContratante = new string('0', 3 - ciot.Empresa.Configuracao.CodigoContratanteSigaFacil.Length) + ciot.Empresa.Configuracao.CodigoContratanteSigaFacil + string.Format("{0:00000000000}", ciot.Empresa.Configuracao.ProximoNSUSigaFacil);
                dadosMotorista.rntrc = string.Format("{0:00000000}", ciot.Veiculo.RNTRC);
                dadosMotorista.telefone = ciot.Motorista.Telefone;

                ServicoSigaFacil.respCadMotoristaBean retornoCadastro = svcSigaFacil.integrarMotorista(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil, token.Token, dadosMotorista);

            }

            if (retornoConsulta.codResposta == "40" || retornoConsulta.codResposta == "48")
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                ServicoSigaFacil.reqIncluirAssociarCartaoBean dadosCartao = new ServicoSigaFacil.reqIncluirAssociarCartaoBean();

                dadosCartao.codigoContratante = int.Parse(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil);
                dadosCartao.codigoFilial = 1;
                dadosCartao.cpfMotorista = ciot.Motorista.CPF;
                dadosCartao.data = DateTime.Now.ToString("dd/MM/yyyy");
                dadosCartao.hora = DateTime.Now.ToString("HH:mm:ss");
                dadosCartao.idtac = (int)ciot.CategoriaTransportador;
                dadosCartao.nomeMotorista = ciot.Motorista.Nome;
                dadosCartao.nsuContratante = new string('0', 3 - ciot.Empresa.Configuracao.CodigoContratanteSigaFacil.Length) + ciot.Empresa.Configuracao.CodigoContratanteSigaFacil + string.Format("{0:00000000000}", ciot.Empresa.Configuracao.ProximoNSUSigaFacil);
                dadosCartao.trilha2 = ciot.Motorista.NumeroCartao;

                ServicoSigaFacil.respIncluirAssociarCartaoBean retornoAssociacao = svcSigaFacil.incluirAssociarCartao(ciot.Empresa.Configuracao.CodigoContratanteSigaFacil, token.Token, dadosCartao);

                repEmpresa.Atualizar(ciot.Empresa);
            }
        }
    }

    public class InspectorBehavior : IEndpointBehavior
    {
        private bool RemoveActionHeader { get; set; }
        public InspectorBehavior(bool removeActionHeader = false)
        {
            RemoveActionHeader = removeActionHeader;
        }

        public string LastRequestXML
        {
            get
            {
                return myMessageInspector.LastRequestXML;
            }
        }

        public string LastResponseXML
        {
            get
            {
                return myMessageInspector.LastResponseXML;
            }
        }


        private MyMessageInspector myMessageInspector = new MyMessageInspector();
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }


        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            myMessageInspector.RemoveActionHeader = RemoveActionHeader;

            clientRuntime.ClientMessageInspectors.Add(myMessageInspector);
        }
    }

    public class MyMessageInspector : IClientMessageInspector
    {
        public bool RemoveActionHeader { get; set; }
        public string LastRequestXML { get; private set; }
        public string LastResponseXML { get; private set; }
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            LastResponseXML = reply.ToString();
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            if (RemoveActionHeader)
            {
                int headerIndexOfAction = request.Headers.FindHeader("Action", "http://schemas.microsoft.com/ws/2005/05/addressing/none");
                request.Headers.RemoveAt(headerIndexOfAction);
            }

            LastRequestXML = request.ToString();
            return request;
        }
    }
}
