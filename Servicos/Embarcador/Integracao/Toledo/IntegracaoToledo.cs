using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.Toledo
{
    public sealed class IntegracaoToledo
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoToledo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void EnviarCriacaoTicketToledo(Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Servicos.Embarcador.Logistica.Pesagem svcPesagem = new Servicos.Embarcador.Logistica.Pesagem(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            if (string.IsNullOrEmpty(integracao.URLToledo))
            {
                integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPesagem.ProblemaIntegracao = "Não foi configurada a integração com a Toledo, por favor verifique.";
                return;
            }

            ServicoToledo.WS_GUARDIANSoapClient WS_GUARDIANSoapClient = ObterClient(integracao.URLToledo);

            ServicoToledo.PreCadastro preCadastro = ObterPreCadastro(integracaoPesagem);

            integracaoPesagem.DataIntegracao = DateTime.Now;
            integracaoPesagem.NumeroTentativas += 1;

            InspectorBehavior inspector = new InspectorBehavior();
            WS_GUARDIANSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;
            bool sucesso = false;

            try
            {
                int codigoErro;
                string mensagemErro;

                ServicoToledo.Ticket retorno = WS_GUARDIANSoapClient.CadastraTicketGuardian(preCadastro, out codigoErro, out mensagemErro);

                if (codigoErro == 303221)
                {
                    sucesso = true;
                    mensagem = "Integração realizada com sucesso.";

                    Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = integracaoPesagem.Pesagem;
                    pesagem.CodigoPesagem = retorno.Codigo.Trim();
                    pesagem.StatusBalanca = StatusBalanca.TicketCriado;
                    repPesagem.Atualizar(pesagem);

                    svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.PesagemInicial, SituacaoIntegracao.AgRetorno);
                }
                else
                {
                    sucesso = false;
                    mensagem = mensagemErro;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            integracaoPesagem.ProblemaIntegracao = mensagem;
            integracaoPesagem.SituacaoIntegracao = sucesso ? SituacaoIntegracao.AgRetorno : SituacaoIntegracao.ProblemaIntegracao;

            servicoArquivoTransacao.Adicionar(integracaoPesagem, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repPesagemIntegracao.Atualizar(integracaoPesagem);
        }

        public void ConsultarConfirmarTicketsCadastrados()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Servicos.Embarcador.Logistica.Pesagem svcPesagem = new Servicos.Embarcador.Logistica.Pesagem(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            if (integracao != null && !string.IsNullOrEmpty(integracao.URLToledo))
            {
                ServicoToledo.WS_GUARDIANSoapClient WS_GUARDIANSoapClient = ObterClient(integracao.URLToledo);
                InspectorBehavior inspector = new InspectorBehavior();
                WS_GUARDIANSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

                try
                {
                    int codigoErro;
                    string mensagemErro;

                    ServicoToledo.Ticket[] retorno = WS_GUARDIANSoapClient.ExportaTicketsMarcados(out codigoErro, out mensagemErro);

                    if (codigoErro != 0)
                    {
                        Servicos.Log.TratarErro("ExportaTicketsMarcados: " + mensagemErro);
                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Logistica.Pesagem> pesagensParaBloqueio = new List<Dominio.Entidades.Embarcador.Logistica.Pesagem>();
                    foreach (ServicoToledo.Ticket ticket in retorno)
                    {
                        string codigoTicket = ticket.Codigo.Trim();
                        ServicoToledo.OperacaoTicket[] operacoes = ticket.OperacaoTicket;

                        Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repPesagem.BuscarPorCodigoTicket(codigoTicket);
                        if (pesagem == null)
                            continue;

                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = pesagem.Guarita;
                        Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem = null;

                        bool atualizou = false;
                        if (ticket.Estado == (int)TipoIntegracaoBalanca.PesagemInicial)
                        {
                            integracaoPesagem = repPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.PesagemInicial);
                            if (integracaoPesagem == null)
                                integracaoPesagem = svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.PesagemInicial, SituacaoIntegracao.AgRetorno);

                            integracaoPesagem.DataIntegracao = DateTime.Now;
                            integracaoPesagem.NumeroTentativas += 1;

                            decimal pesoBruto = operacoes.Where(o => o.TipoOperacaoCodigo == 2)?.FirstOrDefault()?.Peso ?? 0;
                            if (pesoBruto <= 0)
                                continue;
                            pesagem.PesoInicial = pesoBruto;
                            guarita.PesagemInicial = pesoBruto;

                            repPesagem.Atualizar(pesagem);
                            repCargaJanelaCarregamentoGuarita.Atualizar(guarita);

                            pesagensParaBloqueio.Add(pesagem);
                            atualizou = true;
                        }
                        else if (ticket.Estado == (int)TipoIntegracaoBalanca.PesagemFinal || ticket.Estado == (int)TipoIntegracaoBalanca.Encerrado)
                        {
                            integracaoPesagem = repPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.PesagemFinal);
                            if (integracaoPesagem == null)
                                integracaoPesagem = svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.PesagemFinal, SituacaoIntegracao.AgRetorno);

                            integracaoPesagem.DataIntegracao = DateTime.Now;
                            integracaoPesagem.NumeroTentativas += 1;

                            decimal pesoBruto = operacoes.Where(o => o.TipoOperacaoCodigo == 3)?.LastOrDefault()?.Peso ?? 0;
                            if (pesoBruto <= 0)
                                continue;
                            pesagem.PesoFinal = pesoBruto;
                            guarita.PesagemFinal = pesoBruto;
                            if (ticket.Estado == (int)TipoIntegracaoBalanca.Encerrado)
                                pesagem.StatusBalanca = StatusBalanca.Encerrado;

                            repPesagem.Atualizar(pesagem);
                            repCargaJanelaCarregamentoGuarita.Atualizar(guarita);

                            atualizou = true;
                        }

                        if (atualizou)
                        {
                            integracaoPesagem.ProblemaIntegracao = "Integração realizada com sucesso.";
                            integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                            servicoArquivoTransacao.Adicionar(integracaoPesagem, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

                            repPesagemIntegracao.Atualizar(integracaoPesagem);
                        }
                    }

                    if (retorno.Count() > 0)
                    {
                        codigoErro = WS_GUARDIANSoapClient.ConfirmaLeituraTicketsMarcados(out mensagemErro);

                        if (codigoErro != 0)
                        {
                            Servicos.Log.TratarErro("ConfirmaLeituraTicketsMarcados: " + mensagemErro);
                            return;
                        }
                        else
                            AplicarManutencaoTickets(numeroOperacao: 1, pesagens: pesagensParaBloqueio, integracao: integracao);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        public void AplicarManutencaoTickets(int numeroOperacao, List<Dominio.Entidades.Embarcador.Logistica.Pesagem> pesagens, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao)
        {
            foreach (Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem in pesagens)
            {
                AplicarManutencaoTicket(numeroOperacao, pesagem, integracao);
            }
        }

        /// <summary>
        /// numOperacao (Inteiro - Obrigatório) = Número que indica qual operação será realizada no Ticket. 
        /// Valores possíveis: 1 = Bloquear, 2 = Desbloquear, 3 = Encerrar, 4 = Cancelar.
        /// </summary>
        public bool AplicarManutencaoTicket(int numeroOperacao, Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = null, Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem = null)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Servicos.Embarcador.Logistica.Pesagem svcPesagem = new Servicos.Embarcador.Logistica.Pesagem(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            if (integracao == null)
                integracao = repIntegracao.Buscar();

            ServicoToledo.WS_GUARDIANSoapClient WS_GUARDIANSoapClient = ObterClient(integracao.URLToledo);
            InspectorBehavior inspector = new InspectorBehavior();
            WS_GUARDIANSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;
            bool sucesso = false;

            try
            {
                if (integracaoPesagem == null)
                {
                    if (numeroOperacao == 1)//Bloquear
                    {
                        integracaoPesagem = repPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.Bloqueado);
                        if (integracaoPesagem == null)
                            integracaoPesagem = svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.Bloqueado, SituacaoIntegracao.AgIntegracao);
                    }
                    else if (numeroOperacao == 2)//Desbloquear
                    {
                        integracaoPesagem = repPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.AguardandoLiberacao);
                        if (integracaoPesagem == null)
                            integracaoPesagem = svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.AguardandoLiberacao, SituacaoIntegracao.AgIntegracao);
                    }
                }

                integracaoPesagem.DataIntegracao = DateTime.Now;
                integracaoPesagem.NumeroTentativas += 1;

                bool retorno = WS_GUARDIANSoapClient.ManutencaoTicket(pesagem.CodigoPesagem, null, null, numeroOperacao, "WS G", "01");

                if (retorno)
                {
                    if (numeroOperacao == 1)//Bloquear
                    {
                        pesagem.StatusBalanca = StatusBalanca.TicketBloqueado;
                        repPesagem.Atualizar(pesagem);
                    }
                    else if (numeroOperacao == 2)//Desbloquear
                    {
                        pesagem.StatusBalanca = StatusBalanca.TicketDesbloqueado;
                        repPesagem.Atualizar(pesagem);

                        svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.PesagemFinal, SituacaoIntegracao.AgRetorno);
                    }

                    sucesso = true;
                    mensagem = "Integração realizada com sucesso.";
                }
                else
                {
                    sucesso = false;
                    mensagem = "Alteração de estado de ticket não foi executada com sucesso.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            integracaoPesagem.ProblemaIntegracao = mensagem;
            integracaoPesagem.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;

            servicoArquivoTransacao.Adicionar(integracaoPesagem, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repPesagemIntegracao.Atualizar(integracaoPesagem);

            return sucesso;
        }

        /// <summary>
        /// Para teste interno de consulta de tickets existentes
        /// </summary>
        public void ConsultarTicketPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoToledo.WS_GUARDIANSoapClient WS_GUARDIANSoapClient = ObterClient(integracao.URLToledo);
            InspectorBehavior inspector = new InspectorBehavior();
            WS_GUARDIANSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                int codigoErro;
                string mensagemErro;

                ServicoToledo.Ticket[] retorno = WS_GUARDIANSoapClient.ConsultaTicketsPorPeriodo(dataInicial, dataFinal, "WS G", "01", out codigoErro, out mensagemErro);

                if (codigoErro != 0)
                {
                    Servicos.Log.TratarErro("ConsultaTicketsPorPeriodo: " + mensagemErro);
                    return;
                }

                foreach (ServicoToledo.Ticket ticket in retorno)
                {

                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public bool RefazUltimaOperacaoAtiva(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem = null)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Servicos.Embarcador.Logistica.Pesagem svcPesagem = new Servicos.Embarcador.Logistica.Pesagem(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoToledo.WS_GUARDIANSoapClient WS_GUARDIANSoapClient = ObterClient(integracao.URLToledo);
            InspectorBehavior inspector = new InspectorBehavior();
            WS_GUARDIANSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            if (integracaoPesagem == null)
                integracaoPesagem = repPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.RefazerPesagem);
            if (integracaoPesagem == null)
                integracaoPesagem = svcPesagem.GerarIntegracoes(pesagem, TipoIntegracaoBalanca.RefazerPesagem, SituacaoIntegracao.AgIntegracao);

            integracaoPesagem.DataIntegracao = DateTime.Now;
            integracaoPesagem.NumeroTentativas += 1;

            string mensagem;
            bool sucesso = false;

            try
            {
                int codigoErro;
                string mensagemErro;

                WS_GUARDIANSoapClient.RefazUltimaOperacaoAtiva(pesagem.CodigoPesagem, null, null, out codigoErro, out mensagemErro);

                if (codigoErro != 0)
                    mensagem = mensagemErro;
                else
                {
                    sucesso = true;
                    mensagem = "Integração realizada com sucesso.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            integracaoPesagem.ProblemaIntegracao = mensagem;
            integracaoPesagem.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;

            servicoArquivoTransacao.Adicionar(integracaoPesagem, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repPesagemIntegracao.Atualizar(integracaoPesagem);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private ServicoToledo.PreCadastro ObterPreCadastro(Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = integracaoPesagem.Pesagem.Guarita.Carga;

            ServicoToledo.PreCadastro preCadastro = new ServicoToledo.PreCadastro
            {
                DataPesagem = integracaoPesagem.Pesagem.DataPesagem,
                PlacaCarreta = carga.Veiculo?.Placa,
            };

            /*if (carga.Motoristas.Count > 0)
            {
                Dominio.Entidades.Usuario motorista = carga.ListaMotorista.FirstOrDefault();
                preCadastro.Motorista = new ServicoToledo.MotoristaIntegracao
                {
                    Codigo = motorista.CodigoIntegracao,
                    Nome = motorista.Nome,
                    Cpf = motorista.CPF,
                    Rg = motorista.RG,
                    Cnh = motorista.NumeroHabilitacao,
                    DataNascimento = motorista.DataNascimento
                };
            }

            if (carga.Empresa != null)//Transportadora
            {
                Dominio.Entidades.Empresa empresa = carga.Empresa;
                preCadastro.Transportadora = new ServicoToledo.TransportadoraIntegracao
                {
                    Codigo = empresa.CodigoIntegracao,
                    Descricao = empresa.NomeFantasia,
                    RazaoSocial = empresa.RazaoSocial,
                    Cnpj = empresa.CNPJ,
                    InscricaoEstadual = empresa.InscricaoEstadual,
                    Endereco = empresa.Endereco,
                    Municipio = empresa.Localidade?.Descricao,
                    UF = empresa.Localidade?.Estado?.Sigla,
                    Cep = empresa.CEP,
                    TipoDocumento = 1
                };
            }*/

            return preCadastro;
        }

        private static ServicoToledo.WS_GUARDIANSoapClient ObterClient(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoToledo.WS_GUARDIANSoapClient(binding, endpointAddress);
        }

        #endregion
    }
}
