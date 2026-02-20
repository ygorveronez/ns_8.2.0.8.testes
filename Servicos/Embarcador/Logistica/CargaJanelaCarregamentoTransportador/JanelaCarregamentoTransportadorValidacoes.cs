using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class JanelaCarregamentoTransportadorValidacoes
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public JanelaCarregamentoTransportadorValidacoes(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware) : this(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador: null) { }

        public JanelaCarregamentoTransportadorValidacoes(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ValidarApoliceSeguro(int codigoCarga, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte)
        {
            if (empresa == null || !empresa.UsarTipoOperacaoApolice)
                return;

            Repositorio.Embarcador.Transportadores.TransportadorAverbacao repositorioTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);

            if (centroCarregamento == null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(codigoCarga, empresa.Codigo);
                if (cargaJanelaCarregamentoTransportador != null)
                    centroCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento;
            }

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao> transportadorAverbacoes = repositorioTransportadorAverbacao.BuscarPorTransportador(empresa.Codigo);

            if (configuracaoDadosTransporte.ExigirQueApolicePropriaTransportadorEstejaValida && transportadorAverbacoes.Count == 0)
                throw new ServicoException("Não há apólice de seguro vigente configurada no cadastro do transportador!");

            int diasToleranciaVencimento = centroCarregamento?.BloqueioMarcacaoInteresseAntesDiasVencimentoCertificadoApoliceSeguro ?? 0;
            if (diasToleranciaVencimento <= 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Transportadores.TransportadorAverbacao transportadorAverbacao in transportadorAverbacoes)
            {
                if (transportadorAverbacao.ApoliceSeguro.FimVigencia.AddDays(-diasToleranciaVencimento) < DateTime.Now.Date)
                    throw new ServicoException($"Não é possível marcar interesse na carga pois sua apólice de seguro ({transportadorAverbacao.ApoliceSeguro.Seguradora.Descricao}) está vencida ou irá vencer nos próximos {diasToleranciaVencimento} dias.");
            }
        }

        public void ValidarCertificadoDigital(Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Empresa empresa)
        {
            if (empresa == null)
                return;

            int diasToleranciaVencimento = centroCarregamento?.BloqueioMarcacaoInteresseAntesDiasVencimentoCertificadoApoliceSeguro ?? 0;
            if (diasToleranciaVencimento <= 0)
                return;

            if (!empresa.EmissaoDocumentosForaDoSistema && (!empresa.DataFinalCertificado.HasValue || empresa.DataFinalCertificado.Value.AddDays(-diasToleranciaVencimento) < DateTime.Now.Date))
                throw new ServicoException($"Não é possível marcar interesse na carga pois seu certificado digital está vencido ou irá vencer nos próximos {diasToleranciaVencimento} dias.");
        }

        public void ValidarDadosViaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Veiculo> veiculos, Dominio.Entidades.Empresa empresa, out string mensagemRetorno)
        {
            mensagemRetorno = "";

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            if (empresa == null || veiculos.Count == 0)
                return;

            if (carga.Filial?.NaoValidarVeiculoIntegracao ?? false)
                return;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(carga, _unitOfWork);

            if (!integracoesPorTipoOperacaoETipoCarga.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz))
                return;

            var servicoUltragaz = new Servicos.Embarcador.Integracao.Ultragaz.IntegracaoUltragaz(_unitOfWork, _tipoServicoMultisoftware);

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                var retornoIntegracao = servicoUltragaz.ValidarSituacaoTransportadorEVeiculo(carga, empresa, veiculo);

                if (!retornoIntegracao.TransportadorValido && !mensagemRetorno.Contains("transportador"))
                    mensagemRetorno += string.IsNullOrWhiteSpace(mensagemRetorno) ? "O transportador não está apto para efetuar o transporte." : " O transportador não está apto para efetuar o transporte.";

                if (!retornoIntegracao.VeiculoValido)
                    mensagemRetorno += string.IsNullOrWhiteSpace(mensagemRetorno) ? $"O veículo {veiculo.Placa} não está apto a efetuar o transporte." : $" O veículo {veiculo.Placa} não está apto a efetuar o transporte.";
            }
        }

        public void ValidacoesVeiculo(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (veiculo == null)
                return;

            ValidarRenavam(veiculo);

            if (carga == null)
                return;

            ValidarDataChecklist(empresa, veiculo, carga);
            ValidarDisponibilidadeVeiculo(veiculo, carga);
        }

        public void ValidarEspelhamentoVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual)
        {
            DateTime dataAtual = DateTime.Now;
            DateTime dataMinima = dataAtual.AddMinutes(-ObterConfiguracaoEmbarcador().TempoSemPosicaoParaVeiculoPerderSinal);

            if (posicaoAtual == null || posicaoAtual.DataVeiculo < dataMinima)
                throw new ServicoException("Não é possível selecionar o veículo (" + veiculo.Placa + ") pois o mesmo não está espelhado.");
        }

        public void ValidarLimiteCargasParaMotorista(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Usuario motorista)
        {
            int limiteCargasPorDia = cargaJanelaCarregamento.CentroCarregamento?.LimiteCargasPorMotoristaPorDia ?? 0;
            int limiteCargasAtivas = cargaJanelaCarregamento.CentroCarregamento?.LimiteDeCargasAtivasPorMotorista ?? 0;

            if (limiteCargasPorDia <= 0 && limiteCargasAtivas <= 0)
                return;

            if (motorista == null)
                return;

            DateTime dataCarregamento = cargaJanelaCarregamento.InicioCarregamento;
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (limiteCargasPorDia > 0)
            {
                int quantidadeCargasPorMotoristaEDia = repCarga.ContarCargasPorDiaEMotorista(dataCarregamento, motorista.Codigo, somenteCargasAtivas: false);

                if (quantidadeCargasPorMotoristaEDia >= limiteCargasPorDia)
                    throw new ServicoException($"O limite de cargas para o motorista {motorista.Nome} foi atingido");
            }

            if (limiteCargasAtivas > 0)
            {
                int quantidadeCargasAtivasPorMotorista = repCarga.ContarCargasPorDiaEMotorista(dataCarregamento, motorista.Codigo, somenteCargasAtivas: true);

                if (quantidadeCargasAtivasPorMotorista >= limiteCargasAtivas)
                    throw new ServicoException($"O limite de cargas para o motorista {motorista.Nome} foi atingido");
            }
        }

        public void ValidarLimiteCargasParaVeiculo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Veiculo veiculo)
        {
            int limiteCargas = cargaJanelaCarregamento.CentroCarregamento?.LimiteCargasPorVeiculoPorDia ?? 0;

            if (limiteCargas <= 0)
                return;

            if (veiculo == null)
                return;

            DateTime dataCarregamento = cargaJanelaCarregamento.InicioCarregamento;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            int quantidadeCargasParaOVeiculo = repCarga.ContarCargasPorDiaEVeiculos(dataCarregamento, veiculo.Codigo);

            if (quantidadeCargasParaOVeiculo >= limiteCargas)
                throw new ServicoException($"O limite de cargas para o veículo {veiculo.Placa} foi atingido");
        }

        public void ValidarMDFeEmAberto(Dominio.Entidades.Veiculo veiculo)
        {
            if (!ObterConfiguracaoEmbarcador().BloquearVeiculosComMdfeEmAberto)
                return;

            Carga.MDFe servicoCargaMDFe = new Carga.MDFe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargasMDFeEncerramento = servicoCargaMDFe.ObterCargasEncerramentoPorPlacaVeiculo(veiculo.Placa, _unitOfWork);

            if (cargasMDFeEncerramento.Count > 0)
                throw new ServicoException($"Existe um MDF - e em aberto para a placa { veiculo.Placa }, antes de disponibiliza - lo é necessário encerrar o MDF - e.");
        }

        public void ValidarPlaca(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresa)
        {
            if (empresa == null)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            int cargaAberto = repositorioCarga.BuscarCargaEmAbertoPorVeiculoEmpresa(empresa.Codigo, veiculo.Placa);

            if (cargaAberto > 0)
                throw new ServicoException($"A placa {veiculo.Placa} está alocada em uma carga ainda não finalizada.");

            ValidarMDFeEmAberto(veiculo);

            Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento veiculoDisponivelCarregamento = repositorioVeiculoDisponivelCarregamento.BuscarPorVeiculoEmpresa(veiculo.Codigo, empresa.Codigo);

            if (veiculoDisponivelCarregamento != null)
                throw new ServicoException($"A placa {veiculo.Placa} já foi disponibilizada para transporte.", errorCode: ObterConfiguracaoEmbarcador().LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao ? CodigoExcecao.SelecaoQualquerVeiculoNaoConfirmada : CodigoExcecao.NaoEspecificado);
        }

        public void ValidarValorFrete(decimal valorFreteTransportador, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, bool multiplasCargas)
        {
            if (valorFreteTransportador <= 0 && multiplasCargas)
                throw new ServicoException("Necessário informar valores.");
            else if (valorFreteTransportador <= 0)
                throw new ServicoException("Valor do frete deve ser maior que zero.");

            if (ObterConfiguracaoJanelaCarregamento().LiberarCargaParaCotacaoAoLiberarParaTransportadores)
                return;

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete janelaComMenorLance = new CargaJanelaCarregamentoTransportadorConsulta(_unitOfWork).ObterCargaJanelaCarregamentoTransportadorComMenorLance(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, considerarCargasVinculadas: false);
            decimal menorLance = janelaComMenorLance?.ValorTotalFrete ?? cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga?.ValorFrete ?? 0m;

            if (menorLance <= 0)
                return;

            string campoMenorLance = janelaComMenorLance == null ? "valor original do frete da carga" : "valor do menor lance";

            if (valorFreteTransportador > menorLance)
                throw new ServicoException($"O valor informado não pode ser superior ao {campoMenorLance} ({menorLance.ToString("C2")}).");

            Dominio.Entidades.Embarcador.Logistica.LancesCarregamento configuracaoLance = new Repositorio.Embarcador.Logistica.LancesCarregamento(_unitOfWork).BuscarPorNumeroLance(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Rodada, centroCarregamento.Codigo);
            decimal percentualTolerancia = configuracaoLance != null ? configuracaoLance.PorcentagemLance : centroCarregamento?.PercentualMaximoDiferencaValorCotacao ?? 0m;

            if (percentualTolerancia <= 0)
                return;

            decimal percentualDiferenca = 100 - ((valorFreteTransportador * 100.0m) / menorLance);

            if (percentualDiferenca < percentualTolerancia)
                throw new ServicoException($"O valor informado deve ter uma diferença mínima de {percentualTolerancia.ToString("n2")}% do {campoMenorLance}. O valor do lance deve ser menor ou igual a {(menorLance - (menorLance * percentualTolerancia / 100.0m)).ToString("C2", new CultureInfo("pt-BR"))}");
        }

        public void ValidarVeiculosPorCliente(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Empresa empresa)
        {
            if (!(cargaJanelaCarregamento.CentroCarregamento?.PermiteTransportadorInformarSomenteVeiculosRestritosAosClientes ?? false))
                return;

            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamento.Carga;

            List<Dominio.Entidades.Embarcador.Logistica.VeiculoDisponivelCarregamento> veiculosDisponiveisDoTransportador = repositorioVeiculoDisponivelCarregamento.BuscarVeiculosDisponiveisPorEmpresa(empresa?.Codigo ?? 0);
            List<Dominio.Entidades.Cliente> destinatariosDaCarga = carga.Pedidos?.Select(x => x.Pedido.Destinatario).Distinct().ToList() ?? new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatariosComRestricaoVeicular(destinatariosDaCarga.Select(d => d.CPF_CNPJ).ToList());

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centro in centrosDescarregamento)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosPermitidos = centro.VeiculosPermitidos.ToList();
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosTransportador = veiculosDisponiveisDoTransportador.Select(v => v.Veiculo.ModeloVeicularCarga).ToList();

                if (!modelosTransportador.Any(t => modelosPermitidos.Contains(t)))
                    throw new ServicoException($"Veículo não permitido para o cliente {centro.Destinatario.Descricao}. Os veículos permitidos são: {string.Join(", ", modelosPermitidos.Select(m => m.Descricao))}.");
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private void ValidarDisponibilidadeVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportador || configuracaoEmbarcador.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao)
                return;

            string codigoCargaAberto = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).BuscaNumeroDaCargaEmAbertoPorVeiculo(carga.Codigo, veiculo.Placa);

            if (!string.IsNullOrWhiteSpace(codigoCargaAberto))
                throw new ServicoException($"Não é possível selecionar este veículo pois ele está alocado em uma carga que está em processo de alocação (carga: {codigoCargaAberto}).");
        }

        private void ValidarDataChecklist(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!ObterConfiguracaoEmbarcador().UsarDataChecklistVeiculo)
                return;

            if (carga.TipoDeCarga?.NaoValidarDataCheckList ?? false)
                return;

            if (empresa?.UsarTipoOperacaoApolice ?? false)
            {
                bool possuiApolice = new Repositorio.Embarcador.Seguros.ApoliceSeguro(_unitOfWork).BuscarSePossuiApoliceCadastradaPorEmpresa(empresa.Codigo) && (carga.TipoDeCarga?.ExigeVeiculoRastreado ?? false);

                if (possuiApolice)
                    return;
            }

            if (!veiculo.DataUltimoChecklist.HasValue || veiculo.DataUltimoChecklist.Value < DateTime.Now)
                throw new ServicoException("Checklist do veículo inexistente ou vencido, entre em contato com a Logística.");
        }

        private void ValidarRenavam(Dominio.Entidades.Veiculo veiculo)
        {
            if (string.IsNullOrWhiteSpace(veiculo.Renavam))
                throw new ServicoException($"Não é possível selecionar o veículo ({veiculo.Placa}), pois o mesmo não possui o RENAVAM informado.");
        }

        #endregion Métodos Privados
    }
}
