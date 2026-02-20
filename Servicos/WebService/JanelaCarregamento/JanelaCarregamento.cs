using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento;
using System;
using System.Linq;

namespace Servicos.WebService.JanelaCarregamento
{
    public class JanelaCarregamento : ServicoWebServiceBase
    {

        #region Atributos Globais

        readonly private Repositorio.UnitOfWork _unitOfWork;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly private protected string _adminStringConexao;

        #endregion

        #region Construtores

        public JanelaCarregamento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { _unitOfWork = unitOfWork; }
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

        #region Métodos Publicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarEtapaFluxoPatio(InformarEtapaFluxoPatio dadosRequest)
        {
            if (dadosRequest.ProtocoloCarga <= 0 && (string.IsNullOrEmpty(dadosRequest.NumeroCarga) || string.IsNullOrEmpty(dadosRequest.CodigoFilial)))
                throw new WebServiceException("O protocolo da carga ou Numero da Carga e Código de Integracao da Filial deve ser informado.");

            _unitOfWork.Start();

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(dadosRequest.ProtocoloCarga);
            if (carga == null)
                carga = repositorioCarga.BuscarPorCodigoCargaEmbarcadorECodigoIntegracaoFilial(dadosRequest.NumeroCarga, dadosRequest.CodigoFilial);

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();

            if (carga == null)
                throw new WebServiceException("A carga informada não foi encontrada.");

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork, _auditado, _clienteMultisoftware);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

            if ((fluxoGestaoPatio == null || carga.OcultarNoPatio) && carga.CargaAgrupamento != null && !(configuracaoGestaoPatio?.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas ?? false))
                fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga.CargaAgrupamento);

            if (!string.IsNullOrWhiteSpace(dadosRequest.DataLacre)) 
            {
                Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);
                
                if (inicioCarregamento != null)
                {
                    DateTime? dataLacreInformada = dadosRequest.DataLacre.ToNullableDateTime();
                    if (!dataLacreInformada.HasValue)
                        throw new WebServiceException("Dados inválidos, data do lacre não está no formato correto dd/MM/yyyy HH:mm:ss.");

                    inicioCarregamento.DataLacreInicioCarregamento = dataLacreInformada;
                    repositorioInicioCarregamento.Atualizar(inicioCarregamento);
                }
            }

            if (!string.IsNullOrWhiteSpace(dadosRequest.Observacao))
            {
                servicoFluxoGestaoPatio.SalvarObservacaoPorEtapa(fluxoGestaoPatio.Codigo, dadosRequest.EtapaFluxo, dadosRequest.Observacao);
            }

            if (!string.IsNullOrWhiteSpace(dadosRequest.Doca))
            {
                Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCarga(carga.Codigo);
                
                carga.NumeroDoca = dadosRequest.Doca;
                carga.NumeroDocaEncosta = dadosRequest.Doca;
                repositorioCarga.Atualizar(carga);

                docaCarregamento.NumeroDoca = dadosRequest.Doca;
                repositorioDocaCarregamento.Atualizar(docaCarregamento);
            }

            if (fluxoGestaoPatio == null)
                throw new WebServiceException("A carga informada não possui fluxo de pátio.");

            if (fluxoGestaoPatio.DataFinalizacaoFluxo != null)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioDestino = servicoFluxoGestaoPatio.ObterFluxoGestaoPatioDestino(carga);
                if (fluxoGestaoPatioDestino != null)
                    fluxoGestaoPatio = fluxoGestaoPatioDestino;
            }

            Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

            if ((dadosRequest.EtapaFluxo == EtapaFluxoGestaoPatio.Guarita) && (sequenciaGestaoPatio?.GuaritaEntradaPermiteInformacoesPesagem ?? false))//Pesagem Inicial
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

                guarita.PesagemInicial = dadosRequest.Peso;

                repositorioCargaGuarita.Atualizar(guarita);
            }
            else if ((dadosRequest.EtapaFluxo == EtapaFluxoGestaoPatio.InicioViagem) && (sequenciaGestaoPatio?.GuaritaSaidaPermiteInformacoesPesagem ?? false)) //Pesagem Final
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

                guarita.PesagemFinal = dadosRequest.Peso;

                repositorioCargaGuarita.Atualizar(guarita);
            }

            bool forcarAvanco = false;

            if (sequenciaGestaoPatio?.CheckListUtilizarVigencia ?? false)
                forcarAvanco = fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.CheckList && dadosRequest.EtapaFluxo != EtapaFluxoGestaoPatio.CheckList;

            if (dadosRequest.EtapaFluxo == EtapaFluxoGestaoPatio.Posicao)
            {
                _unitOfWork.Rollback();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }

            DateTime? dataEtapaInformada = dadosRequest.DataEtapa.ToNullableDateTime();

            if (!dataEtapaInformada.HasValue)
                throw new WebServiceException("Dados inválidos, data não está no formato correto dd/MM/yyyy HH:mm:ss.");

            if (dadosRequest.EtapaFluxo == EtapaFluxoGestaoPatio.Todas)
                dadosRequest.EtapaFluxo = fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual;

            if (!forcarAvanco && dadosRequest.EtapaFluxo != fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual && !configuracaoGestaoPatio.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas)
                throw new WebServiceException($"A atual etapa do fluxo de pátio ({fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual.ObterDescricao()}) não é a informada.");

            if (forcarAvanco && !fluxoGestaoPatio.GetEtapas().Any(obj => obj.EtapaFluxoGestaoPatio == dadosRequest.EtapaFluxo))
                throw new WebServiceException($"O fluxo da carga não possui a etapa informada ({dadosRequest.EtapaFluxo.ObterDescricao()}).");

            servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, dadosRequest.EtapaFluxo, dataEtapaInformada.Value);
            servicoFluxoGestaoPatio.LiberarProximaEtapaPorCargaAgrupada(fluxoGestaoPatio, dadosRequest.EtapaFluxo, dataEtapaInformada.Value);

            _unitOfWork.CommitChanges();

            Servicos.Log.TratarErro("Sucesso", "InformarEtapaFluxoPatio");
            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarEtapaFluxoPatioPorPlaca(Dominio.ObjetosDeValor.WebService.Carga.AvancoFluxoPatioPorPlaca corpo)
        {
            string formatoData = "dd/MM/yyyy HH:mm:ss";

            try
            {
                if (string.IsNullOrEmpty(corpo.PlacaVeiculo))
                    throw new WebServiceException("A placa do veículo deve ser informada.");

                if (!DateTime.TryParseExact(corpo.DataEtapa, formatoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataEtapa))
                    throw new WebServiceException($"A data de finalização da etapa deve ser informada no formato {formatoData}.");

                if (!TryParseEtapaFluxoGestaoPatio(corpo.EtapaAtualFluxo, out EtapaFluxoGestaoPatio etapaAtualFluxo))
                    throw new WebServiceException("Etapa não reconhecida.");

                _unitOfWork.Start();

                Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new(_unitOfWork, _auditado, _clienteMultisoftware);
                Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new(_unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAvancada retorno = servicoFluxoGestaoPatio.ObterFluxoGestaoPatioPorEtapaEPlacaMaisAntigo(etapaAtualFluxo, corpo.PlacaVeiculo);

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = retorno?.Entidade;

                if (fluxoGestaoPatio == null)
                    throw new WebServiceException("Nenhum fluxo de pátio nesta etapa com essa placa foi encontrado.");

                servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual, dataEtapa);

                _unitOfWork.CommitChanges();

                string mensagem = retorno.HouveramOutrosResultados ? 
                    $"Foi avançado o Fluxo de Pátio da carga {fluxoGestaoPatio.Carga?.CodigoCargaEmbarcador ?? "(carga não localizada)"} com a placa {fluxoGestaoPatio.Veiculo?.Placa ?? "(placa não localizada)"} para a etapa {((int)fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual > 0 ? fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual.ObterDescricao() : "indefinida")} em situação {fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio.ObterDescricao()}" : 
                    null;

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, mensagem);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
             
                if (excecao is BaseException)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao avançar a etapa.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private static bool TryParseEtapaFluxoGestaoPatio(int valor, out EtapaFluxoGestaoPatio resultado)
        {
            if (Enum.IsDefined(typeof(EtapaFluxoGestaoPatio), valor))
            {
                resultado = (EtapaFluxoGestaoPatio)valor;
                return true;
            }
            resultado = default;
            return false;
        }
    }

    #endregion
}