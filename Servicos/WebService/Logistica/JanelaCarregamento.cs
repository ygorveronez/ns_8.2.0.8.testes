using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Logistica.JanelaCarregamento;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.WebService.Logistica
{
    public class JanelaCarregamento : ServicoWebServiceBase
    {
        #region Variaveis Privadas

        readonly Repositorio.UnitOfWork _unitOfWork;
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores

        public JanelaCarregamento(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public Retorno<bool> ControlarLiberacaoTransportadores(ControleLiberacaoTransportadores controleLiberacaoTransportadores)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Embarcador.Carga.Carga(_unitOfWork);

                if (string.IsNullOrWhiteSpace(controleLiberacaoTransportadores.NumeroShipment))
                    throw new ServicoException("Necessário informar o número da viagem (numeroShipment).");

                if (controleLiberacaoTransportadores.Cargas?.Count <= 0)
                    throw new ServicoException("Nenhuma carga informada.");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores> listaAcoesPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores>() {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores.Liberar,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores.Cancelar,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores.Descartar
                };

                if (!listaAcoesPermitidas.Contains(controleLiberacaoTransportadores?.Acao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores.Liberar))
                    throw new ServicoException("Ação informada é inválida.");

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                _unitOfWork.Start();

                foreach (CargaLiberacaoTransportador cargaEmbarcador in controleLiberacaoTransportadores.Cargas)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoEmbarcador(cargaEmbarcador.NumeroCarga);

                    if (carga == null)
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possível encontrar a carga para o número '{cargaEmbarcador.NumeroCarga}'.");

                    cargas.Add(carga);

                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento = repositorioJanelaCarregamento.BuscarPorCarga(carga.Codigo);

                    if (janelaCarregamento != null)
                        cargasJanelaCarregamento.Add(janelaCarregamento);
                }

                // Adiciona as informações nas cargas
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    carga.NumeroViagemIntegracaoLeilao = controleLiberacaoTransportadores.NumeroShipment;
                    carga.DivisoriaIntegracaoLeilao = controleLiberacaoTransportadores.Divisoria;
                    carga.CargaPerigosaIntegracaoLeilao = controleLiberacaoTransportadores.ContemCargaPerigosa;
                    carga.CustoPlanejadoIntegracaoLeilao = controleLiberacaoTransportadores.CustoPlanejado;
                    carga.CustoAtualIntegracaoLeilao = controleLiberacaoTransportadores.CustoAtual;
                    carga.RazaoIntegracaoLeilao = controleLiberacaoTransportadores.Razao;
                    carga.ObservacaoIntegracaoLeilao = controleLiberacaoTransportadores.Observacao;

                    repositorioCarga.Atualizar(carga, _auditado);
                }

                // Atualiza as informaçãoes das janelas de carregamento
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento in cargasJanelaCarregamento)
                {
                    if (controleLiberacaoTransportadores.DataInicio > DateTime.MinValue)
                    {
                        janelaCarregamento.Initialize();

                        servicoCargaJanelaCarregamentoDisponibilidade.AlterarHorarioCarregamento(janelaCarregamento, controleLiberacaoTransportadores.DataInicio, _tipoServicoMultisoftware);
                        Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(_auditado, janelaCarregamento, janelaCarregamento.GetChanges(), "Alterada a data de inicio de carregamento pela integração de leilão.", _unitOfWork);
                    }
                }

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoViagem repositorioJanelaCarregamentoViagem = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoViagem(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(_unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoIntegracao servicoJanelaCarregamentoIntegracao = new Embarcador.Logistica.CargaJanelaCarregamentoIntegracao(_unitOfWork, _tipoServicoMultisoftware);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoViagem servicoCargaJanelaCarregamentoLeilao = new Embarcador.Logistica.CargaJanelaCarregamentoViagem(_unitOfWork, _tipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem viagem = repositorioJanelaCarregamentoViagem.BuscarPorNumeroViagem(controleLiberacaoTransportadores.NumeroShipment);

                servicoCarga.CriarVinculosEntreCargas(cargas, _unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaVinculada> cargasVinculadas = repositorioCargaVinculada.BuscarPorCargas(cargas.Select(o => o.Codigo).Distinct().ToList());

                if (viagem != null)
                {
                    viagem.CargasJanelaCarregamento = cargasJanelaCarregamento;
                    viagem.CargasVinculada = cargasVinculadas;
                    viagem.Acao = controleLiberacaoTransportadores.Acao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores.Liberar;
                    viagem.Divisoria = controleLiberacaoTransportadores.Divisoria;
                    viagem.CargaPerigosa = controleLiberacaoTransportadores.ContemCargaPerigosa;
                    viagem.CustoPlanejado = controleLiberacaoTransportadores.CustoPlanejado;
                    viagem.CustoAtual = controleLiberacaoTransportadores.CustoAtual;
                    viagem.DataInicio = controleLiberacaoTransportadores.DataInicio;
                    viagem.Razao = controleLiberacaoTransportadores.Razao;
                    viagem.Observacao = controleLiberacaoTransportadores.Observacao;
                }
                else
                {
                    viagem = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem()
                    {
                        NumeroViagem = controleLiberacaoTransportadores.NumeroShipment,
                        CargasVinculada = cargasVinculadas,
                        CargasJanelaCarregamento = cargasJanelaCarregamento,
                        Acao = controleLiberacaoTransportadores.Acao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores.Liberar,
                        Divisoria = controleLiberacaoTransportadores.Divisoria,
                        CargaPerigosa = controleLiberacaoTransportadores.ContemCargaPerigosa,
                        CustoPlanejado = controleLiberacaoTransportadores.CustoPlanejado,
                        CustoAtual = controleLiberacaoTransportadores.CustoAtual,
                        DataInicio = controleLiberacaoTransportadores.DataInicio,
                        Razao = controleLiberacaoTransportadores.Razao,
                        Observacao = controleLiberacaoTransportadores.Observacao
                    };
                }

                if (viagem.Codigo > 0)
                    repositorioJanelaCarregamentoViagem.Atualizar(viagem, _auditado);
                else
                    repositorioJanelaCarregamentoViagem.Inserir(viagem, _auditado);

                string arquivoJson = Newtonsoft.Json.JsonConvert.SerializeObject(controleLiberacaoTransportadores, Newtonsoft.Json.Formatting.Indented);
                string arquivoRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(Retorno<bool>.CriarRetornoSucesso(true), Newtonsoft.Json.Formatting.Indented);
                string mensagem = "Integrado controle de liberação da janela de carregamento aos transportadores.";

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao janelaCarregamentoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao()
                {
                    CargaJanelaCarregamentoViagem = viagem,
                    TipoRetornoRecebimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRetornoRecebimento.Recebimento,
                    TipoEvento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEventoIntegracaoJanelaCarregamento.RecebimentoLeilao,
                    DataCriacao = DateTime.Now,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado,
                    Mensagem = mensagem
                };

                repositorioJanelaCarregamentoIntegracao.Inserir(janelaCarregamentoIntegracao);

                servicoJanelaCarregamentoIntegracao.GravarArquivoIntegracao(janelaCarregamentoIntegracao, arquivoRetorno, arquivoJson, mensagem);
                servicoCargaJanelaCarregamentoLeilao.ControlarLiberacaoTransportadores(viagem, controleLiberacaoTransportadores.Acao.Value);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (System.Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
        }

        public Retorno<bool> EventosFluxoPatio(Dominio.ObjetosDeValor.WebService.GestaoPatio.IntegracaoEventosFluxoPatio integracaoEventosFluxoPatio)
        {
            /*  • HTTP_CODE 200 (OK) o Para a mensagem ser considerada entregue com sucesso.
                • HTTP_CODE 3?? ou 4?? (Redirection ou Client Error) Será considerado um erro definitivo do servidor.o A mensagem será descartada e não haverá nova tentativa de envio.
                • Qualquer outro HTTP_CODE Considerado como erro temporário do servidor. Uma nova tentativa de envio será efetuada em alguns minutos.*/

            try
            {
                RegistrarPosicaoVeiculoSubareaAoReceberEvento(integracaoEventosFluxoPatio);

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork, _auditado, _clienteMultisoftware);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);

                if (integracaoEventosFluxoPatio.data == DateTime.MinValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Data inválida. A data precisa ser preenchida.");

                if (string.IsNullOrWhiteSpace(integracaoEventosFluxoPatio.tag))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Tag inválida. A tag deve representar o codigo do equipamento.");

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarFluxoEmAbertoPorEquipamento(integracaoEventosFluxoPatio.tag);

                if (fluxoGestaoPatio == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não existe fluxo patio em aberto para equipamento/tag {integracaoEventosFluxoPatio.tag}.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapa(fluxoGestaoPatio, integracaoEventosFluxoPatio.eventId);

                if (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Todas)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não existe etapa para o eventID {integracaoEventosFluxoPatio.eventId}.", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel);

                _unitOfWork.Start();

                servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, etapa, integracaoEventosFluxoPatio.data);

                servicoFluxoGestaoPatio.LiberarProximaEtapaPorCargaAgrupada(fluxoGestaoPatio, etapa, integracaoEventosFluxoPatio.data);

                _unitOfWork.CommitChanges();

                if ((int)etapa == fluxoGestaoPatio.EtapaAtual)
                {
                    servicoFluxoGestaoPatio.Auditar(fluxoGestaoPatio, "Não avançou etapa");
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível avançar etapa.", 301);
                }
                else
                {
                    servicoFluxoGestaoPatio.Auditar(fluxoGestaoPatio, "Avançou etapa");
                    return Retorno<bool>.CriarRetornoSucesso(true, "Etapa avançou com sucesso.");
                }
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (System.Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
        }

        #endregion

        #region Métodos Privados
        private void RegistrarPosicaoVeiculoSubareaAoReceberEvento(Dominio.ObjetosDeValor.WebService.GestaoPatio.IntegracaoEventosFluxoPatio integracaoEventosFluxoPatio)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio positorioConfiguracaoFluxoPatio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio configuracaoFluxoPatio = positorioConfiguracaoFluxoPatio.BuscarPrimeiroRegistro();
                if (configuracaoFluxoPatio?.RegistrarPosicaoVeiculoSubareaAoReceberEvento ?? false)
                {
                    Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);
                    Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
                    Repositorio.Embarcador.Logistica.SubareaCliente repositorioSubareaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(_unitOfWork);

                    Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorNumeroEquipamento(integracaoEventosFluxoPatio.tag);
                    Dominio.Entidades.Embarcador.Logistica.SubareaCliente subareaCliente = repositorioSubareaCliente.BuscarPorCodigoTag(integracaoEventosFluxoPatio.eventId);
                    if (subareaCliente != null && veiculo != null)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea> polignoSubArea = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(subareaCliente.Area);
                        if (polignoSubArea?.Count > 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint posicaoVeiculo = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.CalcularCentroDaArea(polignoSubArea.Find(p => p.paths != null)?.paths);
                            if (posicaoVeiculo == null) return;

                            Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
                            {
                                Data = integracaoEventosFluxoPatio.data,
                                DataVeiculo = integracaoEventosFluxoPatio.data,
                                DataCadastro = DateTime.Now,
                                IDEquipamento = veiculo.Placa,
                                Veiculo = veiculo,
                                Processar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado,
                                EmAlvo = false,
                                Descricao = $"{posicaoVeiculo.Latitude} - {posicaoVeiculo.Longitude} ({subareaCliente.Descricao})",
                                Latitude = posicaoVeiculo.Latitude,
                                Longitude = posicaoVeiculo.Longitude,
                                Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.BIND
                            };

                            repositorioPosicao.Inserir(novaPosicao);
                            Servicos.Embarcador.Monitoramento.Monitoramento.AtualizarDadosPosicaoAtual(novaPosicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.SemViagem, true, _unitOfWork);
                        }
                    }
                }
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex, "EventosFluxoPatio");
            } //Não deve interferir no fluxo de Patio.
        }
        #endregion
    }
}
