using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.WebService.Entrega
{
    public class Entrega : ServicoWebServiceBase
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
        public Entrega(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Metodos Publicos


        private List<string> ObterPlacas(int codigoCarga, IList<Dominio.ObjetosDeValor.Embarcador.Carga.VeiculoVinculadoMaisVeiculaDaCarga> VeiculosCargas)
        {
            List<string> placas = new List<string>();
            foreach (var item in VeiculosCargas.Where(x => x.CodigoCarga == codigoCarga))
                placas.Add(item.Placa);
            return placas;
        }


        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>> ObterEntregasPorPeriodo(Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodo obterEntregasPorPeriodo)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarCargaEntreguePorPeriodo(obterEntregasPorPeriodo.destino, obterEntregasPorPeriodo.data_inicio, obterEntregasPorPeriodo.data_fim);
            Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>() { Itens = new List<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>(), NumeroTotalDeRegistro = 0 };

            if (cargasEntrega.Count == 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>>.CriarRetornoDadosInvalidos("Não foi localizado nenhuma Carga Entrega no período e destino indicado.");

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.VeiculoVinculadoMaisVeiculaDaCarga> VeiculosCargas = repCarga.BuscarVeiculosDaCarga(cargasEntrega.Select(x => x.Carga.Codigo).ToList());
            foreach (var cargaEntrega in cargasEntrega)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);
                int quantidadePacotes = repositorioCargaPedidoPacote.BuscarQuantidadePacotesPorCargaPedidos(cargaPedidos.Select(o => o.Codigo).ToList());
                List<string> logKeysPacotes = ObterLogKeysPacotes(cargaPedidos.Select(cp => cp.Codigo).ToList());

                retorno.Itens.Add(new Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse()
                {
                    numero_carga = cargaEntrega.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    previsao_chegada_inicio = cargaEntrega.DataPrevista,
                    modelo_veiculo = cargaEntrega.Carga?.ModeloVeicularCarga.Descricao ?? string.Empty,
                    total_pacotes = quantidadePacotes,
                    pacotes = logKeysPacotes,
                    nome_motorista = cargaEntrega.Carga?.NomePrimeiroMotorista ?? string.Empty,
                    cpf_motorista = cargaEntrega.Carga?.CPFPrimeiroMotorista ?? string.Empty,
                    nome_transportadora = cargaEntrega.Carga?.Empresa?.RazaoSocial ?? string.Empty,
                    previsa_chegada_atualizada = cargaEntrega.DataPrevista,
                    data_carregamento = cargaPedidos?.FirstOrDefault()?.Pedido?.DataInicialColeta,
                    tendencia_atraso = cargaEntrega.Situacao.ObterDescricao(),
                    placas = ObterPlacas(cargaEntrega.Carga.Codigo, VeiculosCargas),
                    nome_origem = cargaPedidos?.FirstOrDefault()?.Pedido?.Remetente?.NomeCNPJ ?? string.Empty,
                    codigo_origem = cargaPedidos?.FirstOrDefault()?.Pedido?.Remetente?.CodigoIntegracao ?? string.Empty,
                });
            }

            retorno.NumeroTotalDeRegistro = retorno.Itens.Count;
            return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>>.CriarRetornoSucesso(retorno);
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> ConsultarDetalhesEntregaPorProtocoloCarga(int protocoloDaCarga)
        {

            Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega> retorno = new Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>(),
                NumeroTotalDeRegistro = 0
            };

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorProtocoloCarga(protocoloDaCarga);

            if (cargaEntregas == null || cargaEntregas.Count == 0)
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>>.CriarRetornoExcecao("Não encontrado entregas para carga protocolo " + protocoloDaCarga.ToString());


            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> listaCargaEntregaChaveNfe = repCargaEntregaChaveNfe.BuscarPorCargasEntregas((from o in cargaEntregas select o.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntregas((from obj in cargaEntregas select obj.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> correnciasColetaEntrega = repOcorrenciaColetaEntrega.BuscarPorCodigosEntregas((from obj in cargaEntregas select obj.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                retorno.Itens.Add(ObterEntrega(cargaEntrega, listaCargaEntregaChaveNfe, cargaEntregaPedidos, correnciasColetaEntrega, _unitOfWork));

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou entregas pendentes de integração", _unitOfWork);
            return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega ObterEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> listaCargaEntregaChaveNfe, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> OcorrenciaColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega entrega = new Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega();
            entrega.DataRegistro = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";

            double latitude = 0;
            if (cargaEntrega.LatitudeFinalizada.HasValue)
                latitude = (double)cargaEntrega.LatitudeFinalizada.Value;
            else
                latitude = (cargaEntrega.Cliente?.Latitude ?? "0").ToDouble();

            double longitude = 0;
            if (cargaEntrega.LongitudeFinalizada.HasValue)
                longitude = (double)cargaEntrega.LongitudeFinalizada.Value;
            else
                longitude = (cargaEntrega.Cliente?.Longitude ?? "0").ToDouble();

            entrega.Ponto = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint() { Latitude = latitude, Longitude = longitude };
            entrega.ProtocoloCarga = cargaEntrega.Carga.Codigo;
            entrega.ProtocoloEntrega = cargaEntrega.Codigo;

            entrega.DataInicioViagemCarga = cargaEntrega.Carga.DataInicioViagem.HasValue ? cargaEntrega.Carga.DataInicioViagem.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
            entrega.DataFimViagemCarga = cargaEntrega.Carga.DataFimViagem.HasValue ? cargaEntrega.Carga.DataFimViagem.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
            entrega.CodigoCargaEmbarcador = cargaEntrega.Carga.CodigoCargaEmbarcador;
            entrega.CodigoFilialCarga = cargaEntrega.Carga.Filial?.CodigoFilialEmbarcador;
            entrega.DetalhesEntrega = ObterDetalhesEntrega(cargaEntrega, listaCargaEntregaChaveNfe, cargaEntregaPedidos);
            entrega.DetalhesPedidos = ObterDetalhesPedidos(cargaEntrega, cargaEntregaPedidos);
            entrega.ProtocolosPedidos = (from obj in cargaEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj.CargaPedido.Pedido.Codigo).ToList();
            entrega.OcorrenciasEntrega = ObterOcorrenciasEntrega(cargaEntrega, OcorrenciaColetaEntrega, unitOfWork);

            return entrega;
        }

        public Retorno<bool> AtualizarPrevisaoEntrega(Dominio.ObjetosDeValor.Embarcador.ControleEntrega.AtualizarPrevisaoEntrega atualizarPrevisaoEntrega)
        {

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(atualizarPrevisaoEntrega.protolocoPedido);


                if (string.IsNullOrWhiteSpace(atualizarPrevisaoEntrega.dataPrevisaoEntrega))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Data de Previsão Entrega inválida ou não informada.");

                DateTime.TryParse(atualizarPrevisaoEntrega.dataPrevisaoEntrega, out DateTime dataPrevisaoEntrega);

                if (dataPrevisaoEntrega == DateTime.MinValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Data de Previsão Entrega inválida ou não informada.");


                if (pedido == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Protocolo do Pedido inválido ou não informado.");

                _unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarCargaEntregaLiberadaPorPedido(pedido.Codigo);

                List<Dominio.Entidades.Usuario> motoristas = repositorioUsuario.BuscarTodosMotoristasAtivos(0, 50);

                pedido.PrevisaoEntrega = dataPrevisaoEntrega;

                repositorioPedido.Atualizar(pedido);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaCargaEntrega = null;

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
                {
                    cargaEntrega.DataPrevista = dataPrevisaoEntrega;
                    ultimaCargaEntrega = cargaEntrega;

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaEntrega, null, string.Format(Localization.Resources.Cargas.Carga.AlterouDataPrevisaoEntrega, dataPrevisaoEntrega.ToDateTimeString()), _unitOfWork);
                    repositorioCargaEntrega.Atualizar(cargaEntrega);
                }

                if (ultimaCargaEntrega != null) Servicos.Auditoria.Auditoria.Auditar(_auditado, ultimaCargaEntrega.Carga, null, string.Format(Localization.Resources.Cargas.Carga.AlterouDataPrevisaoEntregaCarga, dataPrevisaoEntrega.ToDateTimeString(), ultimaCargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? "-"), _unitOfWork);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, "Previsão de entrega atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar a data previsão de entrega.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmacaoCarregamentoEntregaPedido(List<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ConfirmacaoCarregamentoEntregaPedido> confirmacaoCarregamentoEntregaPedido)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                if (confirmacaoCarregamentoEntregaPedido.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Nenhum dado enviado.");

                foreach (Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ConfirmacaoCarregamentoEntregaPedido confirmacao in confirmacaoCarregamentoEntregaPedido)
                {
                    if (confirmacao.ListaMs.Count == 0)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("Nenhum dado dos pedidos enviado.");

                    _unitOfWork.Start();

                    foreach (Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ListaMs ms in confirmacao.ListaMs)
                    {
                        if (ms.ListaShipments.Count == 0)
                            return Retorno<bool>.CriarRetornoDadosInvalidos($"Nenhum dado de Shipment na lista do MS Número: {ms.NumeroMs}.");

                        foreach (Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ListaPedidos shipment in ms.ListaShipments)
                        {
                            string numeroDaCarga = shipment.NumeroShipment;

                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarCargaPorCodigoCargaEmbarcador(numeroDaCarga);

                            if (carga == null)
                                return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possível encontrar uma carga para o número {shipment.NumeroShipment}");

                            carga.DataCarregamentoCarga = confirmacao.DataAgenda;

                            repositorioCarga.Atualizar(carga);
                        }
                    }
                    _unitOfWork.CommitChanges();
                }

                return Retorno<bool>.CriarRetornoSucesso(true, "Data de carregamento da carga atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex.Message);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar a data de carregamento da carga.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AgendamentoEntregaPedido(Dominio.ObjetosDeValor.Embarcador.ControleEntrega.AgendamentoEntregaPedido agendamentoEntregaPedido)
        {
            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                string numeroOrdemPedido = agendamentoEntregaPedido.order_container;
                DateTime dataAgendamentoEntrega = DateTime.Parse(agendamentoEntregaPedido.suggested_scheduling_date ?? "", new System.Globalization.CultureInfo("pt-BR"));
                TipoAgendamentoEntrega tipoAgendamentoEntrega = EnumTipoAgendamentoEntregaHelper.ObterEnumPorDescricao(Utilidades.String.RemoveDiacritics(agendamentoEntregaPedido.address_type.ToLower()));

                if (string.IsNullOrEmpty(numeroOrdemPedido))
                    throw new WebServiceException("Número de Ordem é obrigatório.");
                if (dataAgendamentoEntrega == DateTime.MinValue)
                    throw new WebServiceException("Data de agendamento inválida.");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorNumeroOrdem(numeroOrdemPedido) ?? throw new WebServiceException("Não foi possível encontrar o pedido com esse número de ordem.");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(pedido.Codigo) ?? throw new WebServiceException("Não foi possível encontrar uma carga para esse pedido.");

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(_unitOfWork, _auditado);
                servicoAgendamentoEntregaPedido.SalvarDataSugestaoEntrega(dataAgendamentoEntrega, carga.Codigo);
                servicoAgendamentoEntregaPedido.InformarTipoAgendamentoEntrega(pedido, tipoAgendamentoEntrega);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, "Data de agendamento sugerida atualizada com sucesso.");
            }
            catch (WebServiceException ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex.Message);
                return Retorno<bool>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex.Message);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar a data de agendamento sugerida.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        #endregion
        #region Metodos Privados



        private Dominio.ObjetosDeValor.WebService.Entrega.EntregaDetalhes ObterDetalhesEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> listaCargaEntregaChaveNfe, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos)
        {
            Dominio.ObjetosDeValor.WebService.Entrega.EntregaDetalhes detalhes = new Dominio.ObjetosDeValor.WebService.Entrega.EntregaDetalhes();
            detalhes.Cliente = cargaEntrega.Cliente?.Nome ?? "";
            detalhes.ClienteCPFCNPJ = cargaEntrega.Cliente?.CPF_CNPJ ?? 0;
            detalhes.SequenciaPrevista = cargaEntrega.Ordem;
            detalhes.SequenciaRealizada = cargaEntrega.OrdemRealizada;
            detalhes.Peso = (from o in cargaEntregaPedidos where o.CargaEntrega.Codigo == cargaEntrega.Codigo select o.CargaPedido.Peso).Sum();
            detalhes.DataEntradaRaio = cargaEntrega.DataEntradaRaio ?? null;
            detalhes.DataSaidaRaio = cargaEntrega.DataSaidaRaio ?? null;
            detalhes.DataEntrega = cargaEntrega.DataFim ?? cargaEntrega.DataConfirmacao;
            detalhes.EnderecoEntrega = cargaEntrega.Cliente?.EnderecoCompleto ?? "";
            detalhes.LocalidadeEntrega = cargaEntrega.Cliente?.Localidade.DescricaoCidadeEstado ?? "";
            detalhes.NotasFiscais = cargaEntrega.NotasFiscais?.Count > 0 ? string.Join(",", from notas in cargaEntrega.NotasFiscais select notas.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero) : string.Empty;
            detalhes.ChavesNotas = (from o in listaCargaEntregaChaveNfe where o.CargaEntrega.Codigo == cargaEntrega.Codigo select o.ChaveNfe).ToList();
            detalhes.DescricaoSituacao = cargaEntrega.DescricaoSituacao;
            detalhes.JustificativaEntregaForaRaio = cargaEntrega.JustificativaEntregaForaRaio;
            detalhes.DataInicioEntrega = cargaEntrega.DataInicio;
            detalhes.DataFimEntrega = cargaEntrega.DataFim ?? cargaEntrega.DataConfirmacao;
            TimeSpan? diferencaCarregamentoOuDescarregamento = (cargaEntrega.DataFim ?? cargaEntrega.DataConfirmacao) - cargaEntrega.DataInicio;
            detalhes.TempoEntrega = diferencaCarregamentoOuDescarregamento != null ? diferencaCarregamentoOuDescarregamento?.Hours + "h" + diferencaCarregamentoOuDescarregamento?.Minutes + "min" : null;
            detalhes.CPFRecebedor = cargaEntrega.DadosRecebedor?.CPF ?? "";
            detalhes.NomeRecebedor = cargaEntrega.DadosRecebedor?.Nome ?? "";
            detalhes.DataRecebimento = cargaEntrega.DadosRecebedor?.DataEntrega;
            detalhes.Coleta = cargaEntrega.Coleta;
            detalhes.CodigoIntegracaoCliente = cargaEntrega.Cliente.CodigoIntegracao;
            detalhes.CodigoIntegracaoFilial = cargaEntrega.Carga.Filial != null ? (string.IsNullOrEmpty(cargaEntrega.Carga.Filial.CodigoFilialEmbarcador)
                                        ? (cargaEntrega.Carga.Filial.OutrosCodigosIntegracao != null && cargaEntrega.Carga.Filial.OutrosCodigosIntegracao.Any()
                                        ? string.Join(", ", cargaEntrega.Carga.Filial.OutrosCodigosIntegracao)
                                        : "") : (cargaEntrega.Carga.Filial.OutrosCodigosIntegracao != null && cargaEntrega.Carga.Filial.OutrosCodigosIntegracao.Any()
                                        ? $"{cargaEntrega.Carga.Filial.CodigoFilialEmbarcador}, {string.Join(", ", cargaEntrega.Carga.Filial.OutrosCodigosIntegracao)}"
                                        : cargaEntrega.Carga.Filial.CodigoFilialEmbarcador)) : "";
            detalhes.DataEmissaoNota = cargaEntrega.NotasFiscais?.Select(nota => nota.PedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao.ToString()).ToList() ?? new List<string>();
            detalhes.DataRejeicaoEntrega = cargaEntrega.DataRejeitado;
            detalhes.ObservacaoEntrega = cargaEntrega.Observacao;

            return detalhes;
        }

        private List<Dominio.ObjetosDeValor.WebService.Entrega.PedidoDetalhes> ObterDetalhesPedidos(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos)
        {
            List<Dominio.ObjetosDeValor.WebService.Entrega.PedidoDetalhes> detalhesRetorno = new List<Dominio.ObjetosDeValor.WebService.Entrega.PedidoDetalhes>();

            ICollection<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> notasFiscaisCarga = cargaEntrega.NotasFiscais;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidosDaEntrega = cargaEntregaPedidos.Where(o => o.CargaEntrega.Codigo == cargaEntrega.Codigo).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidosDaEntrega)
            {
                IEnumerable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> notasPedido = notasFiscaisCarga.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaEntregaPedido.CargaPedido.Codigo);

                var numerosNotas = string.Join(",", (from notas in notasPedido select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero));
                var valorTotal = notasPedido.Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor);

                Dominio.ObjetosDeValor.WebService.Entrega.PedidoDetalhes pedidoDetalhe = new Dominio.ObjetosDeValor.WebService.Entrega.PedidoDetalhes();
                pedidoDetalhe.DataPrevisaoEntrega = cargaEntregaPedido.CargaPedido.Pedido.DataPrevisaoChegadaDestinatario ?? cargaEntrega.DataPrevista;
                pedidoDetalhe.NumeroPedidoEmbarcador = cargaEntregaPedido.CargaPedido.Pedido.NumeroPedidoEmbarcador;
                pedidoDetalhe.NumeroPedido = cargaEntregaPedido.CargaPedido.Pedido.Numero;
                pedidoDetalhe.ProtocoloPedido = cargaEntregaPedido.CargaPedido.Pedido.Protocolo;
                pedidoDetalhe.ValorNFs = valorTotal;// cargaEntregaPedido.CargaPedido != null && cargaEntregaPedido.CargaPedido.NotasFiscais?.Count > 0 ? (from notas in cargaEntregaPedido.CargaPedido.NotasFiscais select notas.XMLNotaFiscal?.Valor).Sum() : 0;
                pedidoDetalhe.Vendedor = cargaEntregaPedido.CargaPedido.Pedido.Vendedor;
                pedidoDetalhe.NotasFiscais = numerosNotas;// cargaEntregaPedido.CargaPedido != null && cargaEntregaPedido.CargaPedido.NotasFiscais?.Count > 0 ? string.Join(",", (from notas in cargaEntregaPedido.CargaPedido.NotasFiscais select notas.XMLNotaFiscal?.Numero)) : string.Empty;
                detalhesRetorno.Add(pedidoDetalhe);

            }

            return detalhesRetorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes> ObterOcorrenciasEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> OcorrenciaColetaEntrega, Repositorio.UnitOfWork unitOFWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrenciEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOFWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repositorioOcorrenciEntregaAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOFWork);
            List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes> RetornoOcorrencias = new List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes>();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> OcorrenciaColetasDaEntrega = OcorrenciaColetaEntrega.Where(x => x.CargaEntrega.Codigo == cargaEntrega.Codigo).ToList();
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrenciaEntrega in OcorrenciaColetasDaEntrega)
            {
                Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes ocorrenciaRetorno = new Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes();
                ocorrenciaRetorno.Data = ocorrenciaEntrega.DataOcorrencia;
                ocorrenciaRetorno.Observacao = "";
                ocorrenciaRetorno.Tipo = ocorrenciaEntrega.TipoDeOcorrencia.Descricao;
                ocorrenciaRetorno.Situacao = "";

                RetornoOcorrencias.Add(ocorrenciaRetorno);
            }

            return RetornoOcorrencias;

        }

        private List<string> ObterLogKeysPacotes(List<int> codigosCargaPedidos)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Pacote> pacotes = repositorioCargaPedidoPacote.BuscarPacotesPorCargaPedidos(codigosCargaPedidos);
            List<string> logKeys = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Cargas.Pacote pacote in pacotes)
                logKeys.Add(pacote.LogKey);
            return logKeys;
        }

        #endregion

    }
}
