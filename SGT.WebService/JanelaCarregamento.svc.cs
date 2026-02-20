
using CoreWCF;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento;
using Dominio.ObjetosDeValor.WebService.Logistica.JanelaCarregamento;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class JanelaCarregamento(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IJanelaCarregamento
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarFluxoPatioPendenteIntegracao(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoDadosInvalidos("O limite não pode ser superior a 100");

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

                int totalRegistros = repositorioFluxoPatio.ContarFluxosAgIntegracao();
                List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> cargasRetornar = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

                if (totalRegistros > 0)
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> listaFluxoPatio = repositorioFluxoPatio.BuscarFluxosPendentesDeIntegracao((int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio in listaFluxoPatio)
                    {
                        Dominio.ObjetosDeValor.WebService.Carga.Protocolos cargaRetornar = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos()
                        {
                            protocoloIntegracaoCarga = fluxoPatio.Carga.Protocolo
                        };

                        cargasRetornar.Add(cargaRetornar);
                    }
                }

                Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> paginacaoCargas = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>()
                {
                    NumeroTotalDeRegistro = totalRegistros,
                    Itens = cargasRetornar
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoSucesso(paginacaoCargas);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os fluxos de pátio pendentes");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.GestaoPatio.FluxoPatio> BuscarFluxoPatioPorProtocolo(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocoloCarga);

                if (carga == null)
                    throw new WebServiceException("Não foi possível encontrar a carga com o protocolo informado.");

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio = repositorioFluxoPatio.BuscarPorCargaETipo(carga.Codigo, TipoFluxoGestaoPatio.Origem);

                if (fluxoPatio == null)
                    throw new WebServiceException("Não foi possível encontrar um fluxo de pátio referente a carga com o protocolo informado.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoPatio.Codigo);

                if (guarita == null)
                    throw new WebServiceException("Não foi possível encontrar a pesagem para essa carga.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada etapaAtualFluxoPatio = servicoFluxoPatioConfiguracaoEtapa.ObterDescricaoEtapa(fluxoPatio);

                decimal pesagemInicial = guarita.PesagemInicial;
                decimal pesagemFinal = guarita.PesagemFinal;
                decimal pesoLiquidoKg = guarita.PesagemInicial - guarita.PesagemFinal;
                decimal pesoLiquidoCXS = pesoLiquidoKg / (decimal)40.80;
                decimal pesoLiquidoPosPerdas = (((decimal)1.0 - (guarita.PorcentagemPerda / 100)) * pesoLiquidoCXS);

                Dominio.ObjetosDeValor.WebService.GestaoPatio.FluxoPatio informacoesPesagem = new Dominio.ObjetosDeValor.WebService.GestaoPatio.FluxoPatio()
                {
                    PesagemFinal = pesagemFinal,
                    PesagemInicial = pesagemInicial,
                    DataConclusaoPesagem = guarita.DataSaidaGuarita?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                    CaixasPosPerda = pesoLiquidoPosPerdas,
                    DataConclusaoDocumentos = fluxoPatio.DataFimViagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                    EtapaFluxoGestaoPatioAtual = etapaAtualFluxoPatio.Enumerador,
                    DescricaoEtapaPatioAtual = $"{etapaAtualFluxoPatio?.Descricao ?? string.Empty} - {fluxoPatio.Descricao}",
                };

                return Retorno<Dominio.ObjetosDeValor.WebService.GestaoPatio.FluxoPatio>.CriarRetornoSucesso(informacoesPesagem);
            }
            catch (BaseException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.GestaoPatio.FluxoPatio>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.WebService.GestaoPatio.FluxoPatio>.CriarRetornoExcecao("Ocorreu uma falha ao pesquisar fluxo de patio por protocolo");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarFluxoPatioPendenteIntegracao(int protocoloIntegracaoCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocoloIntegracaoCarga <= 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo da carga deve ser informado");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                if (carga == null)
                    throw new WebServiceException("Não foi possível encontrar a carga referente ao protocolo informado");

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio = repositorioFluxoGestaoPatio.BuscarPorCargaETipo(carga.Codigo, TipoFluxoGestaoPatio.Origem);

                if (fluxoPatio == null)
                    throw new WebServiceException("A carga informada não possui um fluxo de pátio");

                if (fluxoPatio.PendenteIntegracao)
                {
                    fluxoPatio.PendenteIntegracao = false;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoPatio, "Confirmou a integração do fluxo de pátio.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do fluxo de pátio");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> InformarEtapaFluxoPatio(int? protocoloCarga, EtapaFluxoGestaoPatio etapaFluxo, string dataEtapa, decimal? peso, string numeroCarga, string codigoFilial, string dataLacre, string doca, string observacao)
            {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.JanelaCarregamento.JanelaCarregamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).InformarEtapaFluxoPatio(new InformarEtapaFluxoPatio()
                {
                    CodigoFilial = codigoFilial,
                    DataEtapa = dataEtapa,
                    EtapaFluxo = etapaFluxo,
                    NumeroCarga = numeroCarga,
                    Peso = peso.HasValue ? peso.Value : 0,
                    ProtocoloCarga = protocoloCarga.HasValue ? protocoloCarga.Value : 0,
                    DataLacre = dataLacre,
                    Doca = doca,
                    Observacao = observacao
                }));
            });
        }

        public Retorno<bool> RetornarEtapaFluxoPatio(int protocoloCarga, EtapaFluxoGestaoPatio etapaFluxo)
        {
            Servicos.Log.TratarErro($"RetornarEtapaFluxoPatio - ProtocoloCarga = {protocoloCarga.ToString()} | Etapa = {etapaFluxo.ObterDescricao()}", "RetornarEtapaFluxoPatio");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (protocoloCarga <= 0)
                    throw new WebServiceException("O protocolo da carga deve ser informado");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarPorProtocolo(protocoloCarga);

                if (carga == null)
                    throw new WebServiceException("A carga informada não foi encontrada");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = null;

                if (carga.OcultarNoPatio && (carga.CargaAgrupamento != null))
                    fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga.CargaAgrupamento);
                else
                    fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

                if (fluxoGestaoPatio == null)
                    throw new WebServiceException("O fluxo da carga não foi encontrado");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();

                if (!configuracaoGestaoPatio.IsPermiteVoltar(etapaFluxo))
                    throw new WebServiceException("A etapa atual não permite retornar");

                if (!fluxoGestaoPatio.GetEtapas().Any(e => e.EtapaFluxoGestaoPatio == etapaFluxo))
                    throw new WebServiceException("O fluxo da carga não possui a etapa informada");

                if (fluxoGestaoPatio.GetEtapaAtual().EtapaFluxoGestaoPatio != etapaFluxo)
                    throw new WebServiceException("A etapa informada não é a etapa atual do fluxo");

                servicoFluxoGestaoPatio.VoltarEtapa(fluxoGestaoPatio, etapaFluxo, null);

                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro("Sucesso", "RetornarEtapaFluxoPatio");
                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao.Message, "RetornarEtapaFluxoPatio");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro("Ocorreu uma falha ao retornar a etapa do fluxo de pátio", "RetornarEtapaFluxoPatio");
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao retornar a etapa do fluxo de pátio");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> InformarEtapaFluxoPatioPorPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, EtapaFluxoGestaoPatio etapaFluxo, string dataEtapa)
        {
            Servicos.Log.TratarErro($"RetornarEtapaFluxoPatio - protocolo = {protocolo.ToString()} | Etapa = {etapaFluxo.ObterDescricao()} | Data {dataEtapa}", "RetornarEtapaFluxoPatio");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocolo.protocoloIntegracaoCarga <= 0)
                    throw new WebServiceException("O protocolo da carga deve ser informado");

                if (protocolo.protocoloIntegracaoPedido <= 0)
                    throw new WebServiceException("O protocolo do pedido deve ser informado");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiroPorProtocoloCargaEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (cargaPedido == null)
                    throw new WebServiceException("O não foi encontrado nenhum pedido para os protocolos informados");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(cargaPedido.Carga);

                if (fluxoGestaoPatio.DataFinalizacaoFluxo != null)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioDestino = servicoFluxoGestaoPatio.ObterFluxoGestaoPatioDestino(cargaPedido.Carga);
                    if (fluxoGestaoPatioDestino != null)
                        fluxoGestaoPatio = fluxoGestaoPatioDestino;
                }


                if (fluxoGestaoPatio == null)
                    throw new WebServiceException("A carga não possui fluxo de pátio");

                if (cargaPedido.ProximaEtapaFluxoGestaoPatioLiberada)
                    throw new WebServiceException("A atual etapa do fluxo de pátio já foi informada para o pedido");

                if (etapaFluxo == EtapaFluxoGestaoPatio.Posicao)
                {
                    unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoSucesso(true);
                }

                DateTime? dataEtapaInformada = dataEtapa.ToNullableDateTime();

                if (!dataEtapaInformada.HasValue)
                    throw new WebServiceException("Dados inválidos, data não está no formato correto dd/MM/yyyy HH:mm:ss");

                if (etapaFluxo == EtapaFluxoGestaoPatio.Todas)
                    etapaFluxo = fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual;

                if (etapaFluxo != fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)
                    throw new WebServiceException("A atual etapa do fluxo de pátio não é a informada");

                cargaPedido.ProximaEtapaFluxoGestaoPatioLiberada = true;

                repositorioCargaPedido.Atualizar(cargaPedido);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoGestaoPatio, $"Informou a etapa {servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(fluxoGestaoPatio).Descricao} para o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador}", unitOfWork);

                if (repositorioCargaPedido.ProximaEtapaFluxoGestaoPatioLiberadaPorCarga(cargaPedido.Carga.Codigo))
                    servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, etapaFluxo, dataEtapaInformada.Value);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao cancelar a reserva de Carregamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> InformarEtapaFluxoPatioPorPlaca(Dominio.ObjetosDeValor.WebService.Carga.AvancoFluxoPatioPorPlaca corpo)
        {
            Servicos.Log.TratarErro($"InformarEtapaFluxoPatioPorPlaca - protocolo = {corpo.ToString()} | Etapa = {corpo.EtapaAtualFluxo} | Data {corpo.DataEtapa}", "InformarEtapaFluxoPatioPorPlaca");

            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.JanelaCarregamento.JanelaCarregamento(
                    unitOfWork,
                    TipoServicoMultisoftware,
                    Cliente,
                    Auditado,
                    ClienteAcesso,
                    Conexao.createInstance(_serviceProvider).AdminStringConexao
                ).InformarEtapaFluxoPatioPorPlaca(corpo));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento> BuscarDisponibilidadeEntrega(Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.DisponibilidadeCarregamento disponibilidadeCarregamento)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                ArmazenarLogIntegracao(disponibilidadeCarregamento, unitOfWork);

                if (string.IsNullOrEmpty(disponibilidadeCarregamento.DataHoraEntrega))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("Informe a data prevista de entrega. ");

                if (disponibilidadeCarregamento.PreCarga == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("É obrigatório informar a pré carga. ");

                if ((disponibilidadeCarregamento.Destinatarios == null) || (disponibilidadeCarregamento.Destinatarios.Count == 0))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("É obrigatório informar os destinatários da carga. ");

                if (!DateTime.TryParseExact(disponibilidadeCarregamento.DataHoraEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataHoraEntrega))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("A data de entrega precisa estar no padrão dd/MM/yyyy HH:mm:ss.");

                bool domingo = DiaSemanaHelper.ObterDiaSemana(dataHoraEntrega) == DiaSemana.Domingo;

                if (domingo)
                    dataHoraEntrega = dataHoraEntrega.AddDays(-1);

                unitOfWork.Start();

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorNumeroPreCarga(disponibilidadeCarregamento.PreCarga.NumeroPreCarga);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null;

                if (preCarga == null)
                {
                    string mensagemErroCriarPreCarga = "";
                    Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);
                    preCarga = servicoPreCarga.CriarPreCarga(disponibilidadeCarregamento.PreCarga, disponibilidadeCarregamento.Destinatarios, dataHoraEntrega, ref mensagemErroCriarPreCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                    if (!string.IsNullOrWhiteSpace(mensagemErroCriarPreCarga))
                        throw new WebServiceException(mensagemErroCriarPreCarga);

                    if (preCarga?.Destinatarios == null || preCarga.Destinatarios.Count <= 0)
                        throw new WebServiceException("Não foi localizado o cliente para a pré carga.");

                    cargaJanelaCarregamento = servicoCargaJanelaCarregamento.AdicionarPorPreCarga(preCarga.Codigo);

                    if (cargaJanelaCarregamento.CentroCarregamento == null)
                        throw new WebServiceException("Não foi encontrado um centro de carregamento compatível com a filial e o tipo de carga informados.");

                    if (cargaJanelaCarregamento.PreCarga?.Rota == null)
                        throw new WebServiceException("Não foi localizado uma rota de frete compatível com a carga.");
                }
                else
                {
                    cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);

                    if ((cargaJanelaCarregamento != null) && cargaJanelaCarregamento.CarregamentoReservado)
                    {
                        unitOfWork.Rollback();

                        return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDuplicidadeRequisicao("A disponibilidade de carregamento já foi realizada para está pré carga", new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento()
                        {
                            DataHoraPrevisaoCarregamento = cargaJanelaCarregamento.DataReservada.Value.ToString("dd/MM/yyyy HH:mm"),
                            PossuiRota = true,
                            ProtocoloReserva = cargaJanelaCarregamento.Codigo,
                            SituacaoReserva = SituacaoReserva.Reservado,
                            TempoEstimadoDeViagem = ((int)Math.Ceiling((decimal)cargaJanelaCarregamento.PreCarga.Rota.TempoDeViagemEmMinutos / 60)).ToString()
                        });
                    }

                    preCarga.DataPrevisaoEntrega = dataHoraEntrega;

                    repositorioPreCarga.Atualizar(preCarga);

                    if (cargaJanelaCarregamento == null)
                        cargaJanelaCarregamento = servicoCargaJanelaCarregamento.AdicionarPorPreCarga(preCarga.Codigo);
                    else
                        servicoCargaJanelaCarregamento.AtualizarPorPreCarga(cargaJanelaCarregamento, atualizarProgramacaoCarregamento: false);
                }

                string mensagemErroValidarHorarios = "";
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento reservaCarregamento = ValidarHorarios(disponibilidadeCarregamento, preCarga, cargaJanelaCarregamento, dataHoraEntrega, ref mensagemErroValidarHorarios, unitOfWork);

                if (reservaCarregamento == null)
                {
                    if (domingo)
                        throw new WebServiceException("Não é possível realizar uma reserva para entregar a carga no Domingo e no sábado não tem mais disponibilidade.");

                    throw new WebServiceException(mensagemErroValidarHorarios);
                }

                reservaCarregamento.TempoEstimadoDeViagem = ((int)Math.Ceiling((decimal)cargaJanelaCarregamento.PreCarga.Rota.TempoDeViagemEmMinutos / 60)).ToString();
                reservaCarregamento.ProtocoloReserva = cargaJanelaCarregamento.Codigo;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, "Realizou a disponibilidade de entrega", unitOfWork);

                unitOfWork.CommitChanges();

                Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>()
                {
                    DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Objeto = reservaCarregamento,
                    Status = true
                };

                if ((reservaCarregamento.SituacaoReserva == SituacaoReserva.Reservado) || (reservaCarregamento.SituacaoReserva == SituacaoReserva.AtendePrazoEntrega))
                    retorno.CodigoMensagem = CodigoMensagemRetorno.Sucesso;
                else
                    retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;

                return retorno;
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoExcecao("Ocorreu uma falha ao verificar a disponibilidade de Carregamento. ");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento> AtualizarReserva(int protocoloReserva, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.DisponibilidadeCarregamento disponibilidadeCarregamento)
        {
            ValidarToken();

            if (protocoloReserva <= 0)
                return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de reserva");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                ArmazenarLogIntegracao(disponibilidadeCarregamento, unitOfWork);

                if (string.IsNullOrEmpty(disponibilidadeCarregamento.DataHoraEntrega))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("Informe a data prevista de entrega. ");

                if ((disponibilidadeCarregamento.Destinatarios == null) || (disponibilidadeCarregamento.Destinatarios.Count == 0))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("É obrigatório informar os destinatários da carga. ");

                if (!DateTime.TryParseExact(disponibilidadeCarregamento.DataHoraEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataHoraEntrega))
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("A data de entrega precisa estar no padrão dd/MM/yyyy HH:mm:ss.");

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(protocoloReserva);

                if (cargaJanelaCarregamento == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("O protocolo informado não pertence a uma reserva válida. ");

                if (cargaJanelaCarregamento.Carga != null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("Não é possível alterar a data da reserva pois a carga já foi gerada. ");

                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = cargaJanelaCarregamento.PreCarga;

                if (preCarga == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos("Não foi localizada uma pré carga para a carga informada. ");

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);

                unitOfWork.Start();

                preCarga.DataPrevisaoEntrega = dataHoraEntrega;

                repositorioPreCarga.Atualizar(preCarga);

                servicoCargaJanelaCarregamento.AtualizarPorPreCarga(cargaJanelaCarregamento, atualizarProgramacaoCarregamento: true);

                string mensagemErroValidarHorarios = "";
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento reservaCarregamento = ValidarHorarios(disponibilidadeCarregamento, preCarga, cargaJanelaCarregamento, dataHoraEntrega, ref mensagemErroValidarHorarios, unitOfWork);

                if (reservaCarregamento == null)
                    throw new WebServiceException(mensagemErroValidarHorarios);

                reservaCarregamento.TempoEstimadoDeViagem = ((int)Math.Ceiling((decimal)cargaJanelaCarregamento.PreCarga.Rota.TempoDeViagemEmMinutos / 60)).ToString();
                reservaCarregamento.ProtocoloReserva = cargaJanelaCarregamento.Codigo;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, "Realizou a alteração na reserva da entrega", unitOfWork);

                unitOfWork.CommitChanges();

                Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>()
                {
                    DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Objeto = reservaCarregamento,
                    Status = true
                };

                if ((reservaCarregamento.SituacaoReserva == SituacaoReserva.Reservado) || (reservaCarregamento.SituacaoReserva == SituacaoReserva.AtendePrazoEntrega))
                    retorno.CodigoMensagem = CodigoMensagemRetorno.Sucesso;
                else
                    retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;

                return retorno;
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a atualizar a reserva de Carregamento. ");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarReserva(int protocoloReserva)
        {
            ValidarToken();

            if (protocoloReserva <= 0)
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de reserva");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(protocoloReserva);

                if (cargaJanelaCarregamento == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo informado não pertence a uma reserva válida. ");

                if (cargaJanelaCarregamento.Situacao != SituacaoCargaJanelaCarregamento.AgAprovacaoComercial)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A atual situação da carga ({cargaJanelaCarregamento.DescricaoSituacao}) não permite que a mesma tenha a confirmação de reserva feita.");

                if (!cargaJanelaCarregamento.DataReservada.HasValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não é possível confirma essa reserva pois a carga não possui uma data sugerida.");

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);
                bool possuiDisponibilidade = servicoCargaJanelaCarregamentoDisponibilidade.IsPossuiDisponibilidadeCarregamentoDia(cargaJanelaCarregamento, cargaJanelaCarregamento.DataReservada.Value, cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga, cargaJanelaCarregamento.PreCarga.Rota);

                if (!possuiDisponibilidade)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não é possível confirmar essa reserva na data previamente sugerida, pois a mesma já foi completamente preenchida, por favor, solicite uma nova data de reserva.");

                cargaJanelaCarregamento.DataCarregamentoProgramada = cargaJanelaCarregamento.DataReservada.Value;
                cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.DataCarregamentoProgramada;
                cargaJanelaCarregamento.TerminoCarregamento = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
                cargaJanelaCarregamento.CarregamentoReservado = true;

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, "Confirmou a reserva", unitOfWork);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a reserva de Carregamento. ");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> CancelarReserva(int protocoloReserva)
        {
            ValidarToken();

            if (protocoloReserva <= 0)
                return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de reserva");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(protocoloReserva);

                if (cargaJanelaCarregamento == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo informado não pertence a uma reserva válida. ");

                if (
                    cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.ProntaParaCarregamento ||
                    cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador ||
                    cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgAceiteTransportador
                )
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A atual situação da carga ({cargaJanelaCarregamento.DescricaoSituacao}) não permite que a mesma tenha a confirmação de reserva feita.");

                if (!cargaJanelaCarregamento.DataReservada.HasValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não é possível confirma essa reserva pois a carga não possui uma data sugerida.");

                cargaJanelaCarregamento.CarregamentoReservado = false;

                repositorioCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaCarregamento, "Cancelou a reserva", unitOfWork);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao cancelar a reserva de Carregamento. ");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ReceberPlaca(string placa)
        {
            ValidarToken();

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<bool> ControlarLiberacaoTransportadores(ControleLiberacaoTransportadores controleLiberacaoTransportadores)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Logistica.JanelaCarregamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ControlarLiberacaoTransportadores(controleLiberacaoTransportadores));
            });
        }

        public Retorno<bool> RejeitarCarga(int protocoloIntegracaoCarga, string motivoRejeicaoCarga, string cnpjTransportador)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocoloIntegracaoCarga <= 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo da carga deve ser informado");

                if (string.IsNullOrEmpty(cnpjTransportador) && integradora.Empresa == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O transportador precisa ser informado");

                if (integradora?.Empresa != null && !string.IsNullOrEmpty(cnpjTransportador))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Seu token de acesso não permite informar o transportador para está ação");

                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = null;
                if (integradora.Empresa != null)
                    empresa = integradora.Empresa;
                else
                    empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjTransportador));

                if (empresa == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O transportador informado não foi localizado na base do embarcador");

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(protocoloIntegracaoCarga, empresa, retornarCargasOriginais: true);
                servicoCargaJanelaCarregamentoTransportador.RejeitarCarga(protocoloIntegracaoCarga, motivoRejeicaoCarga, cargasJanelaCarregamentoTransportador, Cliente, null, TipoServicoMultisoftware, Auditado, Conexao.createInstance(_serviceProvider).AdminStringConexao);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao rejeitar a oferta");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AceitarCarga(int protocoloIntegracaoCarga, string cnpjTransportador)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocoloIntegracaoCarga <= 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo da carga deve ser informado");

                if (string.IsNullOrEmpty(cnpjTransportador) && integradora.Empresa == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O transportador precisa ser informado");

                if (integradora?.Empresa != null && !string.IsNullOrEmpty(cnpjTransportador))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Seu token de acesso não permite informar o transportador para está ação");

                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Empresa empresa = null;
                if (integradora.Empresa != null)
                    empresa = integradora.Empresa;
                else
                    empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjTransportador));

                if (empresa == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O transportador informado não foi localizado na base do embarcador");

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(protocoloIntegracaoCarga, empresa, retornarCargasOriginais: true);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == protocoloIntegracaoCarga select o).FirstOrDefault();


                if (cargaJanelaCarregamentoTransportadorReferencia == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos(Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

                if (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgAceite)
                    return Retorno<bool>.CriarRetornoDadosInvalidos(Localization.Resources.Logistica.JanelaCarregamentoTransportador.situacaoDaCargaNaoPermiteConfirmacao);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga;

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return Retorno<bool>.CriarRetornoDadosInvalidos(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AAtualSituacaoDaCargaNaoPermiteConfirmacao, carga.DescricaoSituacaoCarga));

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                servicoCargaJanelaCarregamentoTransportador.InformarAceiteCargasTransportador(cargaJanelaCarregamentoTransportadorReferencia, cargasJanelaCarregamentoTransportador, empresa, null, Auditado, configuracaoEmbarcador, TipoServicoMultisoftware, Conexao.createInstance(_serviceProvider).AdminStringConexao, Cliente, unitOfWork);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao rejeitar a oferta");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento ValidarHorarios(Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.DisponibilidadeCarregamento disponibilidadeCarregamento, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, DateTime dataHoraEntrega, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento();
            configuracaoDisponibilidadeCarregamento.ValidarSomenteEmResevas = true;
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade serDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoDisponibilidadeCarregamento);

            List<int> modelos = new List<int>();

            if (preCarga.ModeloVeicularCarga != null)
                modelos.Add(preCarga.ModeloVeicularCarga.Codigo);

            DateTime dia = cargaJanelaCarregamento.DataCarregamentoProgramada;
            bool voltouLimiteData = false;
            int dias = 0;

            bool atendePrazo = true;
            bool diaAtual = true;
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento reservaCarregamento = null;
            while (true)
            {
                if (dia >= DateTime.Now)
                {
                    if (diaAtual && dia == DateTime.Now.Date && DateTime.Now > DateTime.Now.Date.AddHours(17))
                    {
                        diaAtual = false;
                        atendePrazo = false;
                        dia = dia.AddDays(1);
                    }
                    else
                    {
                        diaAtual = false;
                        bool possuiDisponibilidade = serDisponibilidadeCarregamento.IsPossuiDisponibilidadeCarregamentoDia(cargaJanelaCarregamento, dia, cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga, cargaJanelaCarregamento.PreCarga.Rota);

                        if (possuiDisponibilidade)
                        {
                            reservaCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento();
                            if (atendePrazo)
                            {
                                reservaCarregamento.DataHoraPrevisaoCarregamento = cargaJanelaCarregamento.DataCarregamentoProgramada.ToString("dd/MM/yyyy HH:mm");

                                DateTime dataProgramadaEntrega = dia.AddMinutes(cargaJanelaCarregamento.PreCarga.Rota?.TempoDeViagemEmMinutos ?? 0);
                                if (dataProgramadaEntrega < dataHoraEntrega)
                                    dataProgramadaEntrega = dataHoraEntrega;

                                reservaCarregamento.DataHoraConsegueEntregar = dataProgramadaEntrega.ToString("dd/MM/yyyy HH:mm");

                                if (disponibilidadeCarregamento.ReservarHorarioSeAtendeEntrega)
                                {
                                    reservaCarregamento.SituacaoReserva = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoReserva.Reservado;
                                    cargaJanelaCarregamento.CarregamentoReservado = true;
                                }
                                else
                                {
                                    reservaCarregamento.SituacaoReserva = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoReserva.AtendePrazoEntrega;
                                    cargaJanelaCarregamento.CarregamentoReservado = false;
                                }
                            }
                            else
                            {
                                reservaCarregamento.SituacaoReserva = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoReserva.NaoAtendePrazoEntrega;
                                reservaCarregamento.DataHoraPrevisaoCarregamento = dia.ToString("dd/MM/yyyy HH:mm");

                                DateTime dataConsegueEntrega = dia.AddMinutes(cargaJanelaCarregamento.PreCarga.Rota?.TempoDeViagemEmMinutos ?? 0);
                                if (dataConsegueEntrega < dataHoraEntrega)
                                    dataConsegueEntrega = dataHoraEntrega;

                                reservaCarregamento.DataHoraConsegueEntregar = dataConsegueEntrega.ToString("dd/MM/yyyy HH:mm");
                            }

                            cargaJanelaCarregamento.DataCarregamentoProgramada = dia;
                            cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.DataCarregamentoProgramada;
                            cargaJanelaCarregamento.TerminoCarregamento = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(cargaJanelaCarregamento.TempoCarregamento);
                            cargaJanelaCarregamento.DataReservada = dia;
                            repCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);

                            break;
                        }
                        else
                        {
                            if (!voltouLimiteData)
                            {
                                //verifica nos dias anteriores (exemplo: se o dia cair em um domingo e não tiver carga, tenta fazer o carregamento no sabado).
                                bool possuiRestricaoRota = serDisponibilidadeCarregamento.IsPossuiRestricaoRota(cargaJanelaCarregamento, cargaJanelaCarregamento.PreCarga.Rota);

                                if (possuiRestricaoRota && dia.AddDays(-1) >= DateTime.Now)
                                {
                                    dia = dia.AddDays(-1);
                                    dias--;
                                }
                                else
                                {
                                    voltouLimiteData = true;
                                    dia = dia.AddDays(1);
                                    atendePrazo = false;
                                }
                            }
                            else
                            {
                                dia = dia.AddDays(1);
                                atendePrazo = false;
                            }
                        }
                    }
                }
                else
                {
                    atendePrazo = false;
                    dia = dia.AddDays(1);
                }

                if (dias > 30)
                {
                    mensagem = "Não é possível enviar um pedido para o modelo de veículo " + cargaJanelaCarregamento.PreCarga.ModeloVeicularCarga.Descricao +
                        " na rota " + cargaJanelaCarregamento.PreCarga.Rota.Descricao + ", pois a logística não atende esse pedido nestas configurações.";
                    break;
                }
                dias++;
            }
            return reservaCarregamento;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override string ObterCaminhoArquivoLog(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid() + "_integracaoPreCarga.txt");
        }

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceJanelaCarregamento;
        }

        #endregion
    }
}
