using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Net;

namespace Servicos.Embarcador.PagamentoMotorista
{
    public sealed class Target
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Target(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void EmitirPagamentoMotorista(int codigoPagamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral repConfiguracaoIntegracaoTargetGeral = new Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotorista.BuscarPorCodigo(codigoPagamento);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(codigoPagamento);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral configuracaoIntegracaoTarget = repConfiguracaoIntegracaoTargetGeral.Buscar();

            if (!(configuracaoIntegracaoTarget?.PossuiIntegracaoEmpresa ?? false))
                throw new ServicoException("Pagamento via Target não está configurado!");

           
            if (pagamento.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > 0)
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
                if (IntegrarNovoTipoDespesa(pagamento, pagamentoEnvio, pagamentoRetorno, configuracaoIntegracaoTarget))
                {
                    IntegrarDespesa(pagamento, pagamentoEnvio, pagamentoRetorno, configuracaoTMS, configuracaoIntegracaoTarget, tipoServicoMultisoftware, auditado);
                }
                repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
            }
            else
            {
                SalvarPagamentoSemValor(ref pagamentoEnvio, ref pagamento);
            }

            repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            repPagamentoMotorista.Atualizar(pagamento);
          
        }

        public bool EstornarPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, out string mensagemErro, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral repConfiguracaoIntegracaoTargetGeral = new Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral configuracaoIntegracaoTarget = repConfiguracaoIntegracaoTargetGeral.Buscar();

            if (!(configuracaoIntegracaoTarget?.PossuiIntegracaoEmpresa ?? false))
                throw new ServicoException("Pagamento via Target não está configurado!");

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoTarget.Empresa.TargetEmpresaTMSServiceClient targetEmpresaTMSServiceClient = ObterClient(configuracaoIntegracaoTarget.URLEmpresa);
            InspectorBehavior inspector = new InspectorBehavior();
            targetEmpresaTMSServiceClient.Endpoint.EndpointBehaviors.Add(inspector);

            bool sucesso;
            string codigoRetorno = string.Empty;

            try
            {
                ServicoTarget.Empresa.BuscaPacoteDespesaRequest buscaPacoteDespesa = new ServicoTarget.Empresa.BuscaPacoteDespesaRequest
                {
                    IdCronogramaDespesa = pagamentoMotorista.CodigoViagem
                };

                ServicoTarget.Empresa.BuscaPacoteDespesaResponse response = targetEmpresaTMSServiceClient.BuscarPacotesDespesa(ObterAutenticacao(configuracaoIntegracaoTarget), buscaPacoteDespesa);

                if (response.ComErro)
                {
                    codigoRetorno = response.Erro.CodigoErro.ToString();
                    mensagemErro = response.Erro.MensagemErro;
                    sucesso = false;
                }
                else
                {
                    int codigoDespesa = response.Itens[0].ItensDespesa[0].IdDespesa;

                    ServicoTarget.Empresa.CancelaReativaDespesaRequest cancelaReativaDespesa = new ServicoTarget.Empresa.CancelaReativaDespesaRequest
                    {
                        IdDespesa = codigoDespesa
                    };

                    ServicoTarget.Empresa.CancelaReativaDespesaResponse responseCancelamento = targetEmpresaTMSServiceClient.CancelarDespesaDoPacote(ObterAutenticacao(configuracaoIntegracaoTarget), cancelaReativaDespesa);

                    if (responseCancelamento.ComErro)
                    {
                        codigoRetorno = responseCancelamento.Erro.CodigoErro.ToString();
                        mensagemErro = responseCancelamento.Erro.MensagemErro;
                        sucesso = false;
                    }
                    else
                    {
                        mensagemErro = "Estornado com Sucesso";
                        sucesso = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = ex.Message;
                sucesso = false;
            }

            pagamentoRetorno.CodigoRetorno = codigoRetorno;
            pagamentoRetorno.DescricaoRetorno = mensagemErro;
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
            pagamentoRetorno.ArquivoRetorno = inspector.LastResponseXML;

            pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);

            repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private void IntegrarDespesa(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral configuracaoIntegracaoTarget, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoTarget.Empresa.TargetEmpresaTMSServiceClient targetEmpresaTMSServiceClient = ObterClient(configuracaoIntegracaoTarget.URLEmpresa);
            InspectorBehavior inspector = new InspectorBehavior();
            targetEmpresaTMSServiceClient.Endpoint.EndpointBehaviors.Add(inspector);

            bool sucesso;
            string codigoRetorno = string.Empty;
            string mensagemRetorno;

            try
            {
                ServicoTarget.Empresa.CadastroPacoteDespesaRequest cadastroPacoteDespesa = ObterCadastroPacoteDespesa(pagamento);
                ServicoTarget.Empresa.CadastroPacoteDespesaResponse response = targetEmpresaTMSServiceClient.CadastrarNovoPacoteDespesa(ObterAutenticacao(configuracaoIntegracaoTarget), cadastroPacoteDespesa);

                if (response.ComErro)
                {
                    codigoRetorno = response.Erro.CodigoErro.ToString();
                    mensagemRetorno = response.Erro.MensagemErro;
                    sucesso = false;
                }
                else
                {
                    pagamento.CodigoViagem = response.IdCronogramaDespesa;
                    mensagemRetorno = "Integrado com sucesso";
                    sucesso = true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemRetorno = ex.Message;
                sucesso = false;
            }

            pagamentoEnvio.Retorno = mensagemRetorno;
            pagamentoEnvio.Data = DateTime.Now;
            pagamentoEnvio.ArquivoEnvio = inspector.LastRequestXML;
            pagamentoEnvio.TipoIntegracaoPagamentoMotorista = TipoIntegracaoPagamentoMotorista.Target;

            pagamentoRetorno.CodigoRetorno = codigoRetorno;
            pagamentoRetorno.DescricaoRetorno = mensagemRetorno;
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamento;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
            pagamentoRetorno.ArquivoRetorno = inspector.LastResponseXML;

            pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);

            if (!sucesso)
            {
                pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
        }

        private bool IntegrarNovoTipoDespesa(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral configuracaoIntegracaoTarget)
        {
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = pagamento.PagamentoMotoristaTipo;

            if (pagamentoMotoristaTipo.CodigoIntegracao.ToInt() > 0)
                return true;

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(_unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoTarget.Empresa.TargetEmpresaTMSServiceClient targetEmpresaTMSServiceClient = ObterClient(configuracaoIntegracaoTarget.URLEmpresa);
            InspectorBehavior inspector = new InspectorBehavior();
            targetEmpresaTMSServiceClient.Endpoint.EndpointBehaviors.Add(inspector);

            bool sucesso;
            string codigoRetorno = string.Empty;
            string mensagemRetorno;
            try
            {
                ServicoTarget.Empresa.CadastroTipoDespesaRequest cadastroTipoDespesa = new ServicoTarget.Empresa.CadastroTipoDespesaRequest
                {
                    NomeDespesa = pagamentoMotoristaTipo.Descricao,
                    DescricaoOuComentario = pagamentoMotoristaTipo.Descricao
                };

                ServicoTarget.Empresa.CadastroTipoDespesaResponse response = targetEmpresaTMSServiceClient.CadastrarNovoTipoDespesa(ObterAutenticacao(configuracaoIntegracaoTarget), cadastroTipoDespesa);

                if (response.ComErro)
                {
                    codigoRetorno = response.Erro.CodigoErro.ToString();
                    mensagemRetorno = response.Erro.MensagemErro;
                    sucesso = false;
                }
                else
                {
                    pagamentoMotoristaTipo.CodigoIntegracao = response.IdTipoDespesa.ToString();
                    repPagamentoMotoristaTipo.Atualizar(pagamentoMotoristaTipo);
                    mensagemRetorno = "Tipo de Despesa integrada com sucesso";
                    sucesso = true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemRetorno = ex.Message;
                sucesso = false;
            }

            if (sucesso)
                return true;

            pagamentoEnvio.Retorno = mensagemRetorno;
            pagamentoEnvio.Data = DateTime.Now;
            pagamentoEnvio.ArquivoEnvio = inspector.LastRequestXML;
            pagamentoEnvio.TipoIntegracaoPagamentoMotorista = TipoIntegracaoPagamentoMotorista.Target;

            pagamentoRetorno.CodigoRetorno = codigoRetorno;
            pagamentoRetorno.DescricaoRetorno = mensagemRetorno;
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamento;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
            pagamentoRetorno.ArquivoRetorno = inspector.LastResponseXML;

            pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);

            pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
            pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            return false;
        }

        private void SalvarPagamentoSemValor(ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            pagamentoEnvio.ArquivoEnvio = "";
            pagamentoEnvio.Data = DateTime.Now;
            pagamentoEnvio.TipoIntegracaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.Target;
            pagamentoEnvio.Retorno = "Não foi enviado para Target devido saldo do motorista";
            pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
        }

        private ServicoTarget.Empresa.CadastroPacoteDespesaRequest ObterCadastroPacoteDespesa(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = pagamento.PagamentoMotoristaTipo;

            ServicoTarget.Empresa.CadastroPacoteDespesaRequest cadastroPacoteDespesa = new ServicoTarget.Empresa.CadastroPacoteDespesaRequest
            {
                NomePacoteDespesa = pagamentoMotoristaTipo.Descricao,
                DescricaoDetalhada = pagamentoMotoristaTipo.Descricao,
                ItensDespesa = ObterItensDespesaParaCadastroPacote(pagamento)
            };

            return cadastroPacoteDespesa;
        }

        private ServicoTarget.Empresa.ItemDespesaParaCadastroPacoteRequest[] ObterItensDespesaParaCadastroPacote(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = pagamento.PagamentoMotoristaTipo;

            ServicoTarget.Empresa.ItemDespesaParaCadastroPacoteRequest itemDespesaParaCadastroPacote = new ServicoTarget.Empresa.ItemDespesaParaCadastroPacoteRequest
            {
                IdTipoDespesa = pagamentoMotoristaTipo.CodigoIntegracao.ToInt(),
                IdFuncionario = pagamento.Motorista.CodigoIntegracao.ToInt(),
                DataHoraVencimento = pagamento.Data,
                ValorPrevisto = pagamento.Valor,
                ProcessarAutomaticamente = true,
                Descricao = pagamentoMotoristaTipo.Descricao,
                NumeroDocumento = pagamento.Numero.ToString(),
                NumeroCartao = !string.IsNullOrWhiteSpace(pagamento.Motorista.NumeroCartao) ? pagamento.Motorista.NumeroCartao : null
            };

            List<ServicoTarget.Empresa.ItemDespesaParaCadastroPacoteRequest> lista = new List<ServicoTarget.Empresa.ItemDespesaParaCadastroPacoteRequest>();
            lista.Add(itemDespesaParaCadastroPacote);
            return lista.ToArray();
        }

        private ServicoTarget.Empresa.AutenticacaoRequest ObterAutenticacao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral configuracaoIntegracaoTargetGeral)
        {
            return new ServicoTarget.Empresa.AutenticacaoRequest
            {
                Usuario = configuracaoIntegracaoTargetGeral.UsuarioEmpresa,
                Senha = configuracaoIntegracaoTargetGeral.SenhaEmpresa
            };
        }

        private ServicoTarget.Empresa.TargetEmpresaTMSServiceClient ObterClient(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoTarget.Empresa.TargetEmpresaTMSServiceClient(binding, endpointAddress);
        }

        #endregion
    }
}
