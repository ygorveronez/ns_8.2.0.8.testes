using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;

namespace Servicos.Embarcador.CIOT
{
    public class Target
    {
        #region Métodos Globais

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = ObterConfiguracaoTarget(ciot.ConfiguracaoCIOT, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

                ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Target;

                if (ciot.Motorista == null)
                    ciot.Motorista = cargaCIOT.Carga.Motoristas.FirstOrDefault();

                if (ciot.Contratante == null)
                    ciot.Contratante = cargaCIOT.Carga.Empresa;

                if (ciot.Veiculo == null)
                {
                    ciot.Veiculo = cargaCIOT.Carga.Veiculo;
                    ciot.VeiculosVinculados = cargaCIOT.Carga.VeiculosVinculados.ToList();
                }

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCIOT.Carga.Codigo);

                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.Target);

                if (IntegrarParticipante(cargaCIOT, cargaPedido.Pedido.Destinatario, configuracao, unitOfWork, out mensagemErro) &&
                        IntegrarTransportador(cargaCIOT, configuracao, modalidadeTerceiro, unitOfWork, out mensagemErro, tipoPagamentoCIOT) &&
                        IntegrarMotorista(cargaCIOT, configuracao, unitOfWork, out mensagemErro) &&
                        AssociarSubstituirCartao(cargaCIOT, modalidadeTerceiro, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT) &&
                        IntegrarOperacaoTransporte(cargaCIOT, modalidadeTerceiro, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT) &&
                        DeclararOperacaoTransporte(cargaCIOT, configuracao, unitOfWork, out mensagemErro))
                    sucesso = true;
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = $"Falha ao realizar a integração da Target: {excecao.Message}";
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração da Target.";
            }

            if (!sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = ObterConfiguracaoTarget(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ServicoTarget.EncerramentoOperacaoTransporteRequest encerramentoOperacao = new ServicoTarget.EncerramentoOperacaoTransporteRequest()
            {
                CodigoOperacao = ciot.ProtocoloAutorizacao.ToInt(),
            };

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(cargaCIOT.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;

            if (cargaRotaFrete != null)
                pontosRota = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota != null && pontosRota.Count > 0)
            {
                Dominio.Entidades.Localidade localidadeOrigem = pontosRota.FirstOrDefault().ClienteOutroEndereco != null ? pontosRota.FirstOrDefault().ClienteOutroEndereco.Localidade : pontosRota.FirstOrDefault().Cliente?.Localidade ?? null;

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ultimoPontoDestino = pontosRota.Where(o => o.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega).LastOrDefault();
                Dominio.Entidades.Localidade localidadeDestino = ultimoPontoDestino == null ? null : ultimoPontoDestino.ClienteOutroEndereco != null ? ultimoPontoDestino.ClienteOutroEndereco.Localidade : ultimoPontoDestino.Cliente?.Localidade ?? null;

                ServicoTarget.OperacaoTransporteViagemRequest[] viagens = new ServicoTarget.OperacaoTransporteViagemRequest[1];
                viagens[0] = new ServicoTarget.OperacaoTransporteViagemRequest();
                viagens[0].MunicipioOrigemCodigoIBGE = localidadeOrigem?.CodigoIBGE ?? 0;
                viagens[0].MunicipioDestinoCodigoIBGE = localidadeDestino?.CodigoIBGE ?? 0;
                viagens[0].NCM = cargaCIOT.Carga.TipoDeCarga?.NCM?.Left(4) ?? "";
                viagens[0].PesoCarga = Math.Round(repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo), 2, MidpointRounding.ToEven);
                viagens[0].QuantidadeViagens = 1;

                encerramentoOperacao.Viagens = viagens;
            }


            ServicoTarget.EncerramentoOperacaoTransporteResponse retorno = svcFreteTMS.EncerrarOperacaoTransporte(autenticacao, encerramentoOperacao);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Operação de transporte encerrada com sucesso.";

                cargaCIOT.CIOT.ProtocoloEncerramento = retorno.ProtocoloEncerramento;
                cargaCIOT.CIOT.DataEncerramento = retorno.DataEncerramento;
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                cargaCIOT.CIOT.Mensagem = ciotIntegracaoArquivo.Mensagem;
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = ObterConfiguracaoTarget(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ciot.MotivoCancelamento = "CANCELAMENTO GERADO PELO OPERADOR";

            ServicoTarget.CancelamentoOperacaoRequest cancelamentoOperacao = new ServicoTarget.CancelamentoOperacaoRequest()
            {
                IdOperacao = ciot.ProtocoloAutorizacao.ToInt(),
                MotivoCancelamento = ciot.MotivoCancelamento
            };

            ServicoTarget.CancelamentoOperacaoResponse retorno = svcFreteTMS.CancelarOperacaoTransporte(autenticacao, cancelamentoOperacao);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Operação de transporte cancelada com sucesso.";

                cargaCIOT.CIOT.DataCancelamento = retorno.DataCancelamento;
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                cargaCIOT.CIOT.Mensagem = ciotIntegracaoArquivo.Mensagem;
                cargaCIOT.CIOT.ProtocoloCancelamento = retorno.ProtocoloCancelamento;
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool IntegrarOperacaoTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            string cartaoMotoristaTarget = string.Empty;
            if (configuracao.ConsultarCartaoMotorista)
            {
                string retornoCartao = string.Empty;
                retornoCartao = ObterCartaoMotorista(configuracao, cargaCIOT.Motorista?.CPF, out cartaoMotoristaTarget);
                if (!string.IsNullOrWhiteSpace(retornoCartao) && cargaCIOT.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    mensagemErro = retornoCartao;

                    return false;
                }
            }

            ServicoTarget.OperacaoTransporteRequest operacaoTransporte = ObterOperacaoTransporte(cargaCIOT, cartaoMotoristaTarget, configuracao, modalidadeTerceiro, unitOfWork, tipoPagamentoCIOT);

            ServicoTarget.OperacaoTransporteResponse retorno = svcFreteTMS.CadastrarAtualizarOperacaoTransporte(autenticacao, operacaoTransporte);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Operação de transporte inserida/atualizada com sucesso.";

                cargaCIOT.CIOT.ProtocoloAutorizacao = retorno.IdOperacaoTransporte.ToString();
                cargaCIOT.CIOT.Mensagem = ciotIntegracaoArquivo.Mensagem;
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool DeclararOperacaoTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ServicoTarget.DeclaracaoOperacaoTransporteRequest declaracao = new ServicoTarget.DeclaracaoOperacaoTransporteRequest()
            {
                IdOperacaoTransporte = cargaCIOT.CIOT.ProtocoloAutorizacao.ToInt()
            };

            ServicoTarget.DeclaracaoOperacaoTransporteResponse retorno = svcFreteTMS.DeclararOperacaoTransporte(autenticacao, declaracao);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Operação de transporte declarada com sucesso.";

                cargaCIOT.CIOT.Numero = retorno.NumeroCIOT;
                cargaCIOT.CIOT.CodigoVerificador = retorno.ProtocoloCIOT;
                cargaCIOT.CIOT.DataAbertura = retorno.DataHoraRegistro;
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                cargaCIOT.CIOT.Mensagem = ciotIntegracaoArquivo.Mensagem;
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool IntegrarTransportador(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            string nome = null, sobrenome = null, razaoSocial = null;
            DateTime? dataNascimento = null;

            if (cargaCIOT.CIOT.Transportador.Tipo == "F")
            {
                int index = cargaCIOT.CIOT.Transportador.Nome.IndexOf(" ");

                nome = cargaCIOT.CIOT.Transportador.Nome.Substring(0, index);

                if (index > -1)
                    sobrenome = cargaCIOT.CIOT.Transportador.Nome.Substring(index + 1);

                dataNascimento = cargaCIOT.CIOT.Transportador.DataNascimento ?? cargaCIOT.CIOT.Transportador.DataCadastro ?? DateTime.Now;
            }
            else
            {
                razaoSocial = cargaCIOT.CIOT.Transportador.Nome;
            }

            int estadoCivil = ObterEstadoCivil(cargaCIOT.CIOT.Transportador.EstadoCivil).ToInt();

            long.TryParse(Utilidades.String.OnlyNumbers(cargaCIOT.CIOT.Transportador.Telefone1), out long telefone);
            long.TryParse(Utilidades.String.OnlyNumbers(cargaCIOT.CIOT.Transportador.Telefone2), out long celular);

            ServicoTarget.TransportadorRequest transportador = new ServicoTarget.TransportadorRequest()
            {
                Instrucao = 1,
                RNTRC = modalidadeTerceiro.RNTRC,
                CPFCNPJ = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                Nome = nome,
                Sobrenome = sobrenome,
                RazaoSocial = razaoSocial,
                DataNascimento = dataNascimento,
                NomeFantasia = cargaCIOT.CIOT.Transportador.Tipo != "F" ? cargaCIOT.CIOT.Transportador.NomeFantasia : string.Empty,
                Sexo = "S",
                DataInscricao = cargaCIOT.CIOT.Transportador.DataCadastro.HasValue ? cargaCIOT.CIOT.Transportador.DataCadastro.Value.AddDays(-1) : DateTime.Now.AddDays(-1),
                Endereco = cargaCIOT.CIOT.Transportador.Endereco,
                NumeroEndereco = cargaCIOT.CIOT.Transportador.Numero,
                EnderecoComplemento = cargaCIOT.CIOT.Transportador.Complemento,
                Bairro = cargaCIOT.CIOT.Transportador.Bairro,
                CEP = cargaCIOT.CIOT.Transportador.CEP,
                CodigoIBGEMunicipio = cargaCIOT.CIOT.Transportador.Localidade.CodigoIBGE,
                IdentificadorEndereco = "Comercial",
                Email = cargaCIOT.CIOT.Transportador.Email,
                EstadoCivil = estadoCivil,
                TelefoneFixo = telefone > 0L ? (long?)telefone : null,
                TelefoneCelular = celular > 0L ? (long?)celular : null,
                Naturalidade = cargaCIOT.CIOT.Transportador.Localidade?.DescricaoCidadeEstado,
                Nacionalidade = cargaCIOT.CIOT.Transportador.Pais?.Nome ?? cargaCIOT.CIOT.Transportador.Localidade?.Estado?.Pais?.Descricao,
            };

            if (tipoPagamentoCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
            {
                string[] conta = cargaCIOT.CIOT.Transportador.NumeroConta?.Split('-') ?? new string[] { };

                transportador.CodigoBanco = cargaCIOT.CIOT.Transportador.Banco?.Numero.ToString();
                transportador.CodigoAgencia = cargaCIOT.CIOT.Transportador.Agencia;
                transportador.DigitoAgencia = cargaCIOT.CIOT.Transportador.DigitoAgencia;
                transportador.ContaCorrente = cargaCIOT.CIOT.Transportador.NumeroConta;
                if (conta.Length > 1)
                    transportador.DigitoContaCorrente = conta[1];
                transportador.FlagContaPoupanca = cargaCIOT.CIOT.Transportador.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança;
            }

            if (cargaCIOT.CIOT.Transportador.Tipo == "F")
            {
                if (!string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Transportador.RG_Passaporte))
                    transportador.RG = cargaCIOT.CIOT.Transportador.RG_Passaporte;

                if (cargaCIOT.CIOT.Transportador.OrgaoEmissorRG.HasValue)
                    transportador.RG = cargaCIOT.CIOT.Transportador.OrgaoEmissorRG.Value.ObterDescricao();
            }

            ServicoTarget.TransportadorResponse retorno = svcFreteTMS.CadastrarAtualizarTransportador(autenticacao, transportador);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Transportador cadastrado/atualizado com sucesso.";
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso || cargaCIOT.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool IntegrarMotorista(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            Dominio.Entidades.Usuario motorista = cargaCIOT.CIOT.Motorista;
            Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato pai = motorista.Contatos?.Where(o => o.TipoParentesco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParentesco.Pai)?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato mae = motorista.Contatos?.Where(o => o.TipoParentesco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParentesco.Mae)?.FirstOrDefault() ?? null;

            int index = motorista.Nome.IndexOf(" ");

            string nome = index > -1 ? motorista.Nome.Substring(0, index) : motorista.Nome;
            string sobrenome = string.Empty;

            if (index > -1)
                sobrenome = motorista.Nome.Substring(index + 1);

            ServicoTarget.MotoristaRequest motoristaRequest = new ServicoTarget.MotoristaRequest()
            {
                Instrucao = 1,
                CPFCNPJTransportador = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                Nome = nome,
                Sobrenome = sobrenome,
                CPF = motorista.CPF,
                NumeroRG = motorista.RG,
                OrgaoEmissorRg = motorista.OrgaoEmissorRG?.ObterDescricao(),
                DataNascimento = motorista.DataNascimento,
                Sexo = motorista.Sexo == Sexo.Feminino ? "F" : "M",
                EstadoCivil = ObterEstadoCivil(motorista.EstadoCivil),
                NomePai = pai != null ? pai.Nome : "Não Informado",
                NomeMae = mae != null ? mae.Nome : "Não Informado",
                Email = motorista.Email,
                Telefone = Utilidades.String.OnlyNumbers(motorista.Telefone),
                TelefoneCelular = Utilidades.String.OnlyNumbers(motorista.Celular),
                Nacionalidade = "Brasileiro",
                Endereco = motorista.Endereco,
                NumeroEndereco = motorista.NumeroEndereco,
                EnderecoComplemento = motorista.Complemento,
                CEP = motorista.CEP,
                Bairro = motorista.Bairro,
                CodigoIBGEMunicipio = motorista.Localidade?.CodigoIBGE,
                Ativo = true
            };

            ServicoTarget.MotoristaResponse retorno = svcFreteTMS.CadastrarAtualizarMotorista(autenticacao, motoristaRequest);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Motorista cadastrado/atualizado com sucesso.";
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso || cargaCIOT.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool IntegrarParticipante(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ServicoTarget.ParticipanteRequest participante = new ServicoTarget.ParticipanteRequest()
            {
                Instrucao = 1,
                IdDmTipoPessoa = cliente.Tipo == "F" ? 1 : 2,
                Nome = !string.IsNullOrWhiteSpace(cliente.NomeFantasia) ? cliente.NomeFantasia : cliente.Nome,
                RazaoSocial = cliente.Nome,
                CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                Endereco = cliente.Endereco,
                Bairro = cliente.Bairro,
                CEP = cliente.CEP,
                MunicipioCodigoIBGE = cliente.Localidade.CodigoIBGE,
                Ativo = true,
                Email = cliente.Email,
            };

            if (!string.IsNullOrEmpty(cliente.Telefone1))
                participante.Telefone = string.Format("({0}){1}", cliente.Telefone1.Substring(0, 2), cliente.Telefone1.Substring(2));

            ServicoTarget.ParticipanteResponse retorno = svcFreteTMS.CadastrarAtualizarParticipante(autenticacao, participante);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Participante cadastrado/atualizado com sucesso.";
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso || cargaCIOT.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool FinalizarOperacaoTransporte(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = ObterConfiguracaoTarget(ciot.ConfiguracaoCIOT, unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ServicoTarget.FinalizacaoOperacaoTransporteRequest finalizacaoOperacao = new ServicoTarget.FinalizacaoOperacaoTransporteRequest()
            {
                IdOperacaoTransporte = ciot.ProtocoloAutorizacao.ToInt()
            };

            ServicoTarget.FinalizacaoOperacaoTransporteResponse retorno = svcFreteTMS.FinalizarOperacaoTransporte(autenticacao, finalizacaoOperacao);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado;
                ciot.Mensagem = "Operação de transporte finalizada com sucesso.";
                ciot.DataAutorizacaoPagamento = retorno.DataHoraFinalizacao;

                ciotIntegracaoArquivo.Mensagem = ciot.Mensagem;
            }
            else
            {
                ciot.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                ciotIntegracaoArquivo.Mensagem = ciot.Mensagem;

                mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            if (sucesso)
            {
                return true;
            }
            else
            {
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                return false;
            }
        }

        public bool AssociarSubstituirCartao(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            mensagemErro = null;

            if (!configuracao.AssociarCartaoMotoristaTransportador)
                return true;

            ServicoTarget.AssociacaoSubstituicaoCartaoRequest associacaoSubstituicaoCartao = ObterAssociacaoSubstituicaoCartao(cargaCIOT, modalidadeTerceiro, tipoPagamentoCIOT);
            if (associacaoSubstituicaoCartao == null)
                return true;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ServicoTarget.AssociacaoSubstituicaoCartaoResponse retorno = svcFreteTMS.AssociarSubstituirCartao(autenticacao, associacaoSubstituicaoCartao);

            bool sucesso = false;

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
            };

            if (retorno.Erro == null)
            {
                sucesso = true;

                ciotIntegracaoArquivo.Mensagem = "Associação de cartão concluída com sucesso.";
                cargaCIOT.CIOT.Mensagem = ciotIntegracaoArquivo.Mensagem;
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                if (retorno.Erro.CodigoErro == 99) //Irá seguir o fluxo quando retorna código 99: "O Cartão informado já se encontra associado a um outro Portador."
                    sucesso = true;
                else
                    mensagemErro = ciotIntegracaoArquivo.Mensagem;
            }

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (sucesso || cargaCIOT.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                return true;
            else
                return false;
        }

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            if (justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto)
            {
                mensagemErro = "A operadora Target não possui integração de Desconto para movimento financeiro.";
                return false;
            }

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = ObterConfiguracaoTarget(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);
            
            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                ServicoTarget.PagamentoAvulsoCartaoRequest pagamentoAvulsoCartao = ObterPagamentoAvulsoCartao(cargaCIOT, cargaCIOT.CIOT, justificativa, valorMovimento, unitOfWork);
                ServicoTarget.PagamentoAvulsoCartaoResponse retorno = svcFreteTMS.RealizarPagamentoAvulsoCartao(autenticacao, pagamentoAvulsoCartao);

                bool sucesso = false;

                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                };

                if (retorno.Erro == null)
                {
                    sucesso = true;

                    ciotIntegracaoArquivo.Mensagem = "Registro de pagamento avulso adicional concluída com sucesso.";
                    cargaCIOT.CIOT.Mensagem = ciotIntegracaoArquivo.Mensagem;
                }
                else
                {
                    ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                    mensagemErro = ciotIntegracaoArquivo.Mensagem;
                }

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                repCIOT.Atualizar(cargaCIOT.CIOT);

                if (sucesso)
                    return true;
                else
                {
                    mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                    return false;
                }


            }
            else
            {
                ServicoTarget.ParcelaAdicionalRequest parcelaAdicional = ObterParcelaAdicional(cargaCIOT, cargaCIOT.CIOT, justificativa, valorMovimento, unitOfWork);
                ServicoTarget.ParcelaAdicionalResponse retorno = svcFreteTMS.RegistrarParcelaAdicional(autenticacao, parcelaAdicional);

                bool sucesso = false;

                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                };

                if (retorno.Erro == null)
                {
                    sucesso = true;

                    ciotIntegracaoArquivo.Mensagem = "Registro de parcela adicional concluída com sucesso.";
                    cargaCIOT.CIOT.Mensagem = ciotIntegracaoArquivo.Mensagem;
                }
                else
                {
                    ciotIntegracaoArquivo.Mensagem = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                    mensagemErro = ciotIntegracaoArquivo.Mensagem;
                }

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                repCIOT.Atualizar(cargaCIOT.CIOT);

                if (sucesso)
                    return true;
                else
                {
                    mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

                    return false;
                }
            }
        }

        public byte[] ImprimirCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = ObterConfiguracaoTarget(ciot.ConfiguracaoCIOT, unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ServicoTarget.EmissaoDocumentoRequest emissaoDocumento = new ServicoTarget.EmissaoDocumentoRequest()
            {
                Tipo = 1,
                IdEntidade = ciot.ProtocoloAutorizacao.ToInt()
            };

            ServicoTarget.EmissaoDocumentoResponse retorno = svcFreteTMS.EmitirDocumento(autenticacao, emissaoDocumento);


            if (retorno.Erro == null)
                return retorno.DocumentoBinario;
            else
                mensagemErro = $"{retorno.Erro.CodigoErro} - {retorno.Erro.MensagemErro} (BusinessRequestId: {retorno.Erro.BusinessRequestId})";

            return null;
        }

        #endregion

        #region Métodos Privados

        private string ObterEstadoCivil(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil? estadoCivil)
        {
            switch (estadoCivil)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil.Outros: return "0";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil.Solteiro: return "1";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil.Casado: return "2";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil.Divorciado: return "5";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil.Desquitado: return "6";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoCivil.Viuvo: return "3";
                default: return "0";
            }
        }


        private ServicoTarget.OperacaoTransporteRequest ObterOperacaoTransporte(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string cartaoMotoristaTarget, Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCIOT.Carga.Codigo);

            decimal valorAdiantamento = cargaCIOT.ContratoFrete.ValorAdiantamento;
            decimal valorAbastecimento = cargaCIOT.ContratoFrete.ValorAbastecimento;
            decimal valorFrete = cargaCIOT.ContratoFrete.ValorBruto;
            decimal valorIRRF = cargaCIOT.ContratoFrete.ValorIRRF;
            decimal valorINSS = cargaCIOT.ContratoFrete.ValorINSS;
            decimal valorSESTSENAT = cargaCIOT.ContratoFrete.ValorSEST + cargaCIOT.ContratoFrete.ValorSENAT;
            decimal valorSaldo = cargaCIOT.ContratoFrete.SaldoAReceber;
            decimal tarifaSaque = cargaCIOT.ContratoFrete.TarifaSaque;
            decimal tarifaTransferencia = cargaCIOT.ContratoFrete.TarifaTransferencia;

            if (cargaCIOT.ContratoFrete.ReterImpostosContratoFrete && cargaCIOT.ContratoFrete.TransportadorTerceiro?.Tipo == "F")
            {
                valorIRRF = 0m;
                valorINSS = 0m;
                valorSESTSENAT = 0m;
            }

            int categoriaVeiculo = cargaCIOT.Carga.ModeloVeicularCarga?.CategoriaVeiculoTarget ?? 0;

            if (categoriaVeiculo <= 0)
                categoriaVeiculo = 8;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(cargaCIOT.Carga?.Codigo ?? 0);

            List<ServicoTarget.DadosQuitacaoFreteDocumentosRequest> dadosQuitacaoFreteDocumentosRequest = cargaCTes.Select(o => new ServicoTarget.DadosQuitacaoFreteDocumentosRequest()
            {
                DocumentoGeradoDestino = false,
                NomeDocumento = o.CTe?.ModeloDocumentoFiscal?.Abreviacao,
                NumeroIdentificadorDocumento = o.CTe.Numero.ToString(),
                Obrigatorio = false
            }).ToList();

            DateTime dataHoraInicio = DateTime.SpecifyKind(cargaPedido.Pedido.DataPrevisaoSaida ?? DateTime.Now, DateTimeKind.Unspecified);
            DateTime dataHoraTermino = DateTime.SpecifyKind(cargaPedido.Pedido.PrevisaoEntrega ?? DateTime.Now.AddHours(2), DateTimeKind.Unspecified);
            if (dataHoraTermino <= dataHoraInicio.AddHours(1))
                dataHoraTermino = dataHoraInicio.AddDays(2);

            ServicoTarget.OperacaoTransporteRequest operacaoTransporte = new ServicoTarget.OperacaoTransporteRequest()
            {
                Instrucao = 1,
                IdOperacaoTransporte = cargaCIOT.CIOT.ProtocoloAutorizacao?.ToNullableInt(),
                NCM = cargaCIOT.Carga.TipoDeCarga?.NCM?.Left(4) ?? "",
                ProprietarioCarga = cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? 2 : 1,
                PesoCarga = Math.Round(repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo), 2, MidpointRounding.ToEven),
                TipoOperacao = modalidadeTerceiro.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado ? 3 : 1,
                MunicipioOrigemCodigoIBGE = cargaPedido.Origem.CodigoIBGE,
                MunicipioDestinoCodigoIBGE = cargaPedido.Destino.CodigoIBGE,
                DataHoraInicio = dataHoraInicio,
                DataHoraTermino = dataHoraTermino,
                CPFCNPJContratado = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                ValorFrete = valorFrete,
                ValorCombustivel = valorAbastecimento,
                ValorImpostoINSS = valorINSS,
                ValorImpostoIRRF = valorIRRF,
                ValorImpostoSestSenat = valorSESTSENAT,
                ValorImpostoISS = 0,
                ValorDescontoAntecipado = 0,
                ParcelaUnica = (valorAdiantamento + valorAbastecimento) <= 0,
                ModoCompraValePedagio = 4,
                CategoriaVeiculo = categoriaVeiculo,
                NomeMotorista = cargaCIOT.CIOT.Motorista?.Nome,
                CPFMotorista = Utilidades.String.OnlyNumbers(cargaCIOT.CIOT.Motorista?.CPF),
                RNTRCMotorista = modalidadeTerceiro.RNTRC,
                DeduzirImpostos = true,
                TarifasBancarias = tarifaSaque + tarifaTransferencia,
                QuantidadeTarifasBancarias = 8,
                IdIntegrador = cargaCIOT.Codigo.ToString(),
                CPFCNPJParticipanteDestinatario = cargaPedido.Pedido.Destinatario.CPF_CNPJ_SemFormato,
                Quitacao = false,
                DadosQuitacao = new ServicoTarget.DadosQuitacaoFreteRequest()
                {
                    ValorMercadoria = Math.Round(repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(cargaCIOT.Carga.Codigo), 2, MidpointRounding.ToEven),
                    PesoCarregadoMercadoria = Math.Round(repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo), 2, MidpointRounding.ToEven),
                    QuantidadeCarregada = repPedidoXMLNotaFiscal.BuscarVolumesPorCarga(cargaCIOT.Carga.Codigo),
                    TipoCalculoAvaria = 4,
                    EncerraNaANTT = true,
                    QuitaEmTodosTerminais = true,
                    DocumentosQuitacao = dadosQuitacaoFreteDocumentosRequest.ToArray()
                },
            };

            List<ServicoTarget.OperacaoTransporteVeiculoRequest> veiculos = new List<ServicoTarget.OperacaoTransporteVeiculoRequest>();

            if (cargaCIOT.CIOT.Veiculo != null)
            {
                veiculos.Add(new ServicoTarget.OperacaoTransporteVeiculoRequest()
                {
                    Placa = cargaCIOT.CIOT.Veiculo.Placa,
                    RNTRC = string.Format("{0:00000000}", modalidadeTerceiro.RNTRC)
                });
            }

            if (cargaCIOT.CIOT.VeiculosVinculados.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo veiculo in cargaCIOT.CIOT.VeiculosVinculados)
                {
                    veiculos.Add(new ServicoTarget.OperacaoTransporteVeiculoRequest()
                    {
                        Placa = veiculo.Placa,
                        RNTRC = string.Format("{0:00000000}", modalidadeTerceiro.RNTRC)
                    });
                }
            }

            operacaoTransporte.Veiculos = veiculos.ToArray();

            List<ServicoTarget.OperacaoTransporteParcelaRequest> parcelas = new List<ServicoTarget.OperacaoTransporteParcelaRequest>();

            string cartaoPagamento = null,
                   codigoBanco = null,
                   agenciaDeposito = null,
                   contaDeposito = null,
                   digitoContaDeposito = null;

            int formaPagamento = tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Deposito ? 2 : 1;
            bool flagContaPoupanca = tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Deposito && cargaCIOT.CIOT.Transportador.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança ? true : false;

            if (modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
            {
                if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
                    cartaoPagamento = !string.IsNullOrWhiteSpace(cartaoMotoristaTarget) ? cartaoMotoristaTarget : cargaCIOT.CIOT.Motorista.NumeroCartao;
                else
                {
                    string[] conta = cargaCIOT.CIOT.Motorista.NumeroConta?.Split('-') ?? new string[] { };

                    codigoBanco = cargaCIOT.CIOT.Motorista.Banco?.Numero.ToString();
                    agenciaDeposito = cargaCIOT.CIOT.Motorista.Agencia;
                    contaDeposito = cargaCIOT.CIOT.Motorista.NumeroConta;

                    if (conta.Length > 1)
                        digitoContaDeposito = conta[1];
                };
            }
            else
            {
                if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
                    cartaoPagamento = modalidadeTerceiro.NumeroCartao;
                else
                {
                    string[] conta = cargaCIOT.CIOT.Transportador.NumeroConta?.Split('-') ?? new string[] { };

                    codigoBanco = cargaCIOT.CIOT.Transportador.Banco?.Numero.ToString();
                    agenciaDeposito = cargaCIOT.CIOT.Transportador.Agencia;
                    contaDeposito = cargaCIOT.CIOT.Transportador.NumeroConta;

                    if (conta.Length > 1)
                        digitoContaDeposito = conta[1];
                };
            }

            int numeroParcela = 1;

            if (valorAdiantamento > 0)
            {
                DateTime dataVencimentoAdiantamento = DateTime.SpecifyKind(DateTime.Now.AddDays(cargaCIOT.ContratoFrete.DiasVencimentoAdiantamento).Date, DateTimeKind.Unspecified);

                parcelas.Add(new ServicoTarget.OperacaoTransporteParcelaRequest()
                {
                    DescricaoParcela = "Adiantamento",
                    Valor = valorAdiantamento,
                    NumeroParcela = numeroParcela,
                    DataVencimento = ObterDataFixaVencimentoCiot(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork) ?? dataVencimentoAdiantamento,
                    TipoDaParcela = 1,
                    FormaPagamento = formaPagamento,
                    CartaoPagamento = cartaoPagamento,
                    CodigoBanco = codigoBanco,
                    AgenciaDeposito = agenciaDeposito,
                    ContaDeposito = contaDeposito,
                    DigitoContaDeposito = digitoContaDeposito,
                    ProcessarAutomaticamente = true,
                    FlagContaPoupanca = flagContaPoupanca
                });

                numeroParcela++;
            }

            DateTime dataVencimentoSaldo = DateTime.SpecifyKind(Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(cargaCIOT.ContratoFrete).Date, DateTimeKind.Unspecified);

            parcelas.Add(new ServicoTarget.OperacaoTransporteParcelaRequest()
            {
                DescricaoParcela = "Saldo",
                Valor = valorSaldo,
                NumeroParcela = numeroParcela,
                DataVencimento = ObterDataFixaVencimentoCiot(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork) ?? dataVencimentoSaldo,
                TipoDaParcela = 3,
                FormaPagamento = formaPagamento,
                CartaoPagamento = cartaoPagamento,
                CodigoBanco = codigoBanco,
                AgenciaDeposito = agenciaDeposito,
                ContaDeposito = contaDeposito,
                DigitoContaDeposito = digitoContaDeposito,
                ProcessarAutomaticamente = cargaCIOT.CIOT.ConfiguracaoCIOT.HabilitarQuitacaoAutomaticaPagamentosPendentes,
                FlagContaPoupanca = flagContaPoupanca
            });

            operacaoTransporte.Parcelas = parcelas.ToArray();

            return operacaoTransporte;
        }

        private DateTime? ObterDataFixaVencimentoCiot(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            if (!configuracaoCIOT.HabilitarDataFixaVencimentoCIOT)
                return null;

            Repositorio.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento repConfiguracaoCIOTDataFixaVencimento = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento(unitOfWork);
            List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento> listaConfiguracaoCIOTDataFixaVencimento = repConfiguracaoCIOTDataFixaVencimento.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            if (!listaConfiguracaoCIOTDataFixaVencimento.Any())
                return null;

            DateTime dataAtual = DateTime.Today;
            int mesAtual = dataAtual.Month;
            int anoAtual = dataAtual.Year;
            int ultimoDiaDoMes = DateTime.DaysInMonth(anoAtual, mesAtual);

            foreach (Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento configuracao in listaConfiguracaoCIOTDataFixaVencimento)
            {
                DateTime dataInicial = new DateTime(anoAtual, mesAtual, Math.Min(configuracao.DiaInicialEmissao, ultimoDiaDoMes));
                DateTime dataFinal = new DateTime(anoAtual, mesAtual, Math.Min(configuracao.DiaFinalEmissao, ultimoDiaDoMes));

                if (dataInicial > dataFinal)
                {
                    if (dataInicial < dataAtual && dataFinal.AddMonths(1) >= dataAtual)
                        return CalcularDataVencimento(dataAtual, configuracao.DiaVencimentoCIOT);

                    if (dataInicial.AddMonths(-1) <= dataAtual && dataFinal > dataAtual)
                        return CalcularDataVencimento(dataAtual, configuracao.DiaVencimentoCIOT);
                }
                else
                {
                    if (dataInicial <= dataAtual && dataFinal >= dataAtual)
                        return CalcularDataVencimento(dataAtual, configuracao.DiaVencimentoCIOT);
                }
            }

            return null;
        }

        private DateTime CalcularDataVencimento(DateTime dataAtual, int diaVencimentoCIOT)
        {
            if (diaVencimentoCIOT < dataAtual.Day)
                return new DateTime(ObterAnoDataFixa(dataAtual), dataAtual.AddMonths(1).Month, diaVencimentoCIOT);

            return new DateTime(dataAtual.Year, dataAtual.Month, diaVencimentoCIOT);
        }

        private int ObterAnoDataFixa(DateTime dataAtual)
        {
            return dataAtual.Month == 12 ? dataAtual.AddYears(1).Year : dataAtual.Year;
        }

        private ServicoTarget.AssociacaoSubstituicaoCartaoRequest ObterAssociacaoSubstituicaoCartao(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            string numeroCartao,
                   cpfPortador,
                   cnpjCartaoEmpresarial;

            if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
            {
                if (modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
                    numeroCartao = cargaCIOT.CIOT.Motorista.NumeroCartao;
                else
                    numeroCartao = modalidadeTerceiro.NumeroCartao;

                cpfPortador = cargaCIOT.CIOT.Motorista.CPF;
                cnpjCartaoEmpresarial = modalidadeTerceiro.ModalidadePessoas.Cliente.Tipo == "J" ? modalidadeTerceiro.ModalidadePessoas.Cliente.CPF_CNPJ_SemFormato : string.Empty;
            }
            else
                return null;

            ServicoTarget.AssociacaoSubstituicaoCartaoRequest associacaoSubstituicaoCartao = new ServicoTarget.AssociacaoSubstituicaoCartaoRequest()
            {
                NumeroNovoCartao = numeroCartao,
                CpfPortadorCartao = cpfPortador,
                CnpjCartaoEmpresarial = cnpjCartaoEmpresarial
            };

            return associacaoSubstituicaoCartao;
        }


        private ServicoTarget.ParcelaAdicionalRequest ObterParcelaAdicional(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.Target);

            string numeroCartao = string.Empty;

            if (modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
            {
                if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
                    numeroCartao = cargaCIOT.CIOT.Motorista.NumeroCartao;
            }
            else
            {
                if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
                    numeroCartao = modalidadeTerceiro.NumeroCartao;
            }

            DateTime dataVencimentoSaldo = DateTime.SpecifyKind(Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(cargaCIOT.ContratoFrete).Date, DateTimeKind.Unspecified);

            ServicoTarget.ParcelaAdicionalRequest parcelaAdicional = new ServicoTarget.ParcelaAdicionalRequest()
            {
                IdOperacaoTransporte = ciot.ProtocoloAutorizacao.ToInt(),
                NumeroCartao = numeroCartao,
                Valor = valorMovimento,
                DataVencimento = dataVencimentoSaldo,
                Descricao = justificativa.Descricao,
                Automatica = false
            };

            return parcelaAdicional;
        }

        private Dominio.Entidades.Embarcador.CIOT.CIOTTarget ObterConfiguracaoTarget(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CIOT.CIOTTarget repCIOTTarget = new Repositorio.Embarcador.CIOT.CIOTTarget(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = repCIOTTarget.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            return configuracao;
        }

        private static ServicoTarget.FreteTMSServiceClient ObterClientTarget(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            url = url.ToLower();

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoTarget.FreteTMSServiceClient(binding, endpointAddress);
        }

        private ServicoTarget.PagamentoAvulsoCartaoRequest ObterPagamentoAvulsoCartao(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(cargaCIOT.Carga?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.Target);


            string numeroCartao = string.Empty;

            if (modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
            {
                if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
                    numeroCartao = cargaCIOT.CIOT.Motorista.NumeroCartao;
            }
            else
            {
                if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
                    numeroCartao = modalidadeTerceiro.NumeroCartao;
            }

            ServicoTarget.PagamentoAvulsoCartaoRequest parcelaAdicional = new ServicoTarget.PagamentoAvulsoCartaoRequest()
            {
                NumeroCartao = numeroCartao,
                Valor = valorMovimento,
                Comentario = justificativa.Descricao,
                //IdCentroDeCusto = 
                //NSU = 
                IdIntegrador = cargaCIOT.Codigo.ToString(),
                ItemFinanceiro = cargaCIOT.CIOT.Numero,
                ProcessarManualmente = false,
                NumeroDocumentoEmbarque = cargaCTes.Count > 0 ? cargaCTes.FirstOrDefault().CTe.Numero.ToString() : cargaCIOT.Carga.Numero,
                Placa = cargaCIOT.Carga.Veiculo?.Placa ?? string.Empty
            };

            return parcelaAdicional;
        }

        private string ObterCartaoMotorista(Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao, string cpfMotorista, out string cartaoMotorista)
        {
            cartaoMotorista = string.Empty;

            if (configuracao == null || string.IsNullOrWhiteSpace(cpfMotorista))
            {
                return string.Empty;
            }

            InspectorBehavior inspector = new InspectorBehavior();

            Servicos.ServicoTarget.FreteTMSServiceClient svcFreteTMS = ObterClientTarget(configuracao.URLWebService);
            svcFreteTMS.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoTarget.AutenticacaoRequest autenticacao = new ServicoTarget.AutenticacaoRequest()
            {
                Usuario = configuracao.Usuario,
                Senha = configuracao.Senha
            };

            ServicoTarget.BuscaCartoesRequest buscaRequest = new ServicoTarget.BuscaCartoesRequest()
            {
                CPFCNPJ = cpfMotorista
            };

            var retorno = svcFreteTMS.BuscarCartoesPortador(autenticacao, buscaRequest);

            if (retorno != null && retorno.Erro == null)
            {
                cartaoMotorista = retorno.ListaCartoesAtivos.FirstOrDefault().NumeroCartao;
                return string.Empty;
            }
            else
            {
                Servicos.Log.TratarErro(inspector.LastRequestXML);
                Servicos.Log.TratarErro(inspector.LastResponseXML);
                return (retorno.Erro?.MensagemErro ?? "Target não retornou cartão motorista CPF " + cpfMotorista);
            }
        }


        #endregion
    }
}
