using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class FilaCarregamentoMotorista : FilaCarregamentoBase
    {
        #region Construtores

        public FilaCarregamentoMotorista(Repositorio.UnitOfWork unitOfWork, OrigemAlteracaoFilaCarregamento origemAlteracao) : base(unitOfWork, origemAlteracao) { }

        public FilaCarregamentoMotorista(Repositorio.UnitOfWorkContainer unitOfWorkContainer, OrigemAlteracaoFilaCarregamento origemAlteracao) : base(unitOfWorkContainer, origemAlteracao) { }

        #endregion

        #region Métodos Privados

        private void NotificarFilaAlterada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao();

            alteracao.Tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoMotorista);
            alteracao.CentrosCarregamento.Add(filaCarregamentoMotorista.CentroCarregamento.Codigo);

            if (filaCarregamentoMotorista.ConjuntoVeiculo != null)
            {
                alteracao.ModelosVeicularesCarga.Add(filaCarregamentoMotorista.ConjuntoVeiculo.ModeloVeicularCarga.Codigo);

                if (filaCarregamentoMotorista.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular != null)
                    alteracao.GruposModelosVeicularesCarga.Add(filaCarregamentoMotorista.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo);
            }

            new Hubs.FilaCarregamento().NotificarTodosFilaAlterada(alteracao);
        }

        private void Remover(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista)
        {
            ValidarSituacaoPermiteRemover(filaCarregamentoMotorista);

            Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorioFilaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork);

            filaCarregamentoMotorista.Situacao = SituacaoFilaCarregamentoMotorista.Removido;

            repositorioFilaCarregamentoMotorista.Atualizar(filaCarregamentoMotorista);
        }

        public void ValidarJornadaTrabalhoIniciada(Dominio.Entidades.Usuario motorista)
        {
            Transportadores.MotoristaJornada servicoJornada = new Transportadores.MotoristaJornada(_unitOfWorkContainer);

            servicoJornada.ValidarJornadaTrabalhoIniciada(motorista);
        }

        private void ValidarMotoristaDisponivelAdicionarFilaCarregamento(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorioFilaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = repositorioFilaCarregamentoMotorista.BuscarAtivaPorMotorista(motorista.Codigo);

            if (filaCarregamentoMotorista != null)
            {
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer, _origemAlteracao);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFilaSemValidacaoNulo(filaCarregamentoMotorista.Motorista.Codigo);

                if (filaCarregamentoVeiculo != null)
                    throw new ServicoException(servicoFilaCarregamentoVeiculo.ObterDescricaoMotoristaIndisponivelAdicionarFilaCarregamento(filaCarregamentoVeiculo));

                throw new ServicoException("O motorista já está na fila de carregamento");
            }
        }

        private void ValidarMotoristaNaFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista)
        {
            FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, _origemAlteracao);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFilaSemValidacaoNulo(filaCarregamentoMotorista.Motorista.Codigo);

            if (filaCarregamentoVeiculo != null)
            {
                if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmTransicao)
                    throw new ServicoException("O motorista já está na fila de carregamento em transição");

                throw new ServicoException(servicoFilaCarregamentoVeiculo.ObterDescricaoMotoristaIndisponivelAdicionarFilaCarregamento(filaCarregamentoVeiculo));
            }
        }

        private void ValidarSituacaoPermiteRemover(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista)
        {
            if (filaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.ReboqueAtrelado)
                throw new ServicoException($"O motorista ainda está com reboque atrelado.");
        }

        private void ValidarTransportadorIgual(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Veiculo veiculo)
        {
            if ((motorista.Empresa != null) && (veiculo.Empresa != null) && (motorista.Empresa.Codigo != veiculo.Empresa.Codigo))
                throw new ServicoException("Transportador do motorista e do veículo do motorista diferentes");
        }

        #endregion

        #region Métodos Privados de Consulta

        private bool IsEmpresaTipoAmbienteHomologacao()
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarEmpresaPai();

            return (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao);
        }

        private bool IsIntegracaoBuonnyConfigurada()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioIntegracao.Buscar();

            return !string.IsNullOrWhiteSpace(configuracaoIntegracao?.URLRestProducaoBuonny);
        }

        private bool IsRealizarIntegracaoGerenciadoraEmHomologacao()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();

            return configuracao?.RealizarIntegracaoGerenciadoraEmHomologacao ?? false;
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamentoPorCodigo(int codigoCentroCarregamento)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWorkContainer.UnitOfWork);

            return repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ServicoException("Centro de carregamento não encontrado.");
        }

        private decimal ObterDistanciaCentroCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista, string latitudeMotorista, string longitudeMotorista)
        {
            if (filaCarregamentoMotorista.CentroCarregamento.DistanciaMinimaEntrarFilaCarregamento <= 0)
                return 0m;

            if (!IsRealizarIntegracaoGerenciadoraEmHomologacao() && IsEmpresaTipoAmbienteHomologacao())
                return 0m;

            decimal distancia = 0;

            if (IsIntegracaoBuonnyConfigurada())
                distancia = ObterDistanciaCentroCarregamentoIntegracaoBuonny(filaCarregamentoMotorista);
            else
            {
                if (string.IsNullOrWhiteSpace(filaCarregamentoMotorista.CentroCarregamento.Latitude) || string.IsNullOrWhiteSpace(filaCarregamentoMotorista.CentroCarregamento.Longitude))
                    throw new ServicoException("Latitude e longitude não configuradas para o centro de carregamento.");

                if (string.IsNullOrWhiteSpace(latitudeMotorista) || string.IsNullOrWhiteSpace(longitudeMotorista))
                    return 0m;

                distancia = (decimal)new Global.CalculadoraGeolocalizacao().ObterDistanciaEmKilometros(filaCarregamentoMotorista.CentroCarregamento.Latitude, filaCarregamentoMotorista.CentroCarregamento.Longitude, latitudeMotorista, longitudeMotorista);
            }

            if (distancia > filaCarregamentoMotorista.CentroCarregamento.DistanciaMinimaEntrarFilaCarregamento)
                throw new ServicoException($"A distância atual ({distancia} km) é superior a mínima definida pelo centro de carregamento de {filaCarregamentoMotorista.CentroCarregamento.DistanciaMinimaEntrarFilaCarregamento} km");

            return distancia;
        }

        private decimal ObterDistanciaCentroCarregamentoIntegracaoBuonny(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista)
        {
            if (filaCarregamentoMotorista.ConjuntoVeiculo.Tracao == null)
                return 0m;

            decimal? distancia = null;

            try
            {
                string mensagemErro = string.Empty;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoDistanciaAlvo retornoIntegracaoBuonny = Integracao.Buonny.IntegracaoBuonny.DistanciaPlacaAlvo(filaCarregamentoMotorista.ConjuntoVeiculo.Tracao.Placa, filaCarregamentoMotorista.CentroCarregamento.Filial.CodigoFilialEmbarcador, ref mensagemErro, _unitOfWorkContainer.UnitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    Log.TratarErro(mensagemErro);
                else if (!string.IsNullOrWhiteSpace(retornoIntegracaoBuonny?.distancia))
                    distancia = retornoIntegracaoBuonny.distancia.Replace(".", ",").ToDecimal();
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }

            if (!distancia.HasValue)
                throw new ServicoException("Consulta Buonny não retornou a distância para o CD, entrar em contato com a transportadora.");

            return distancia.Value;
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularCarga(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo.IsTipoVeiculoTracao() && (veiculo.VeiculosVinculados?.Count > 0))
                return veiculo.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;

            return veiculo.ModeloVeicularCarga;
        }

        private Dominio.Entidades.Veiculo ObterVeiculoPorMotorista(int codigoMotorista)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorioVeiculo.BuscarPorMotorista(codigoMotorista) ?? throw new ServicoException("Veículo do motorista não encontrado, entrar em contato com a transportadora.");
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista Adicionar(int codigoMotorista)
        {
            return Adicionar(codigoMotorista, latitude: "", longitude: "", lojaProximidade: false);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista Adicionar(int codigoMotorista, string latitude, string longitude, bool lojaProximidade)
        {
            Dominio.Entidades.Usuario motorista = ObterMotorista(codigoMotorista) ?? throw new ServicoException("Motorista não encontrado.");

            ValidarMotoristaDisponivelAdicionarFilaCarregamento(motorista);
            ValidarJornadaTrabalhoIniciada(motorista);

            Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorMotorista(motorista.Codigo);

            if (veiculo.IsTipoVeiculoReboque())
                throw new ServicoException("Não é possivel adicionar o motorista na fila somente com um reboque.");

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculoMotorista = Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(veiculo);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamentoPorVeiculo(veiculo);

            ValidarMotorista(motorista, centroCarregamento?.NaoValidarIntegracaoGR ?? false);
            ValidarConjuntoVeiculo(conjuntoVeiculoMotorista, centroCarregamento?.NaoValidarIntegracaoGR ?? false);
            ValidarTransportadorIgual(motorista, veiculo);
            ValidarConjuntoVeiculoExclusivoOutroCentroCarregamento(conjuntoVeiculoMotorista, centroCarregamento);

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista()
            {
                ConjuntoVeiculo = conjuntoVeiculoMotorista,
                CentroCarregamento = centroCarregamento,
                DataEntrada = DateTime.Now,
                LojaProximidade = lojaProximidade,
                Motorista = motorista,
                Situacao = SituacaoFilaCarregamentoMotorista.Disponivel
            };

            filaCarregamentoMotorista.DistanciaCentroCarregamento = ObterDistanciaCentroCarregamento(filaCarregamentoMotorista, latitude, longitude);

            Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repositorioFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorioFilaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork);

            repositorioFilaCarregamentoConjuntoVeiculo.Inserir(conjuntoVeiculoMotorista);
            repositorioFilaCarregamentoMotorista.Inserir(filaCarregamentoMotorista);

            return filaCarregamentoMotorista;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista Adicionar(int codigoMotorista, int codigoCentroCarregamento, int codigoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Usuario motorista = ObterMotorista(codigoMotorista) ?? throw new ServicoException("Motorista não encontrado.");

            ValidarMotoristaDisponivelAdicionarFilaCarregamento(motorista);
            ValidarJornadaTrabalhoIniciada(motorista);

            Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? ObterVeiculoPorCodigo(codigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamentoPorCodigo(codigoCentroCarregamento);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculoMotorista = null;

            ValidarMotorista(motorista, centroCarregamento?.NaoValidarIntegracaoGR ?? false);

            if (veiculo != null)
            {
                if (veiculo.IsTipoVeiculoReboque())
                    throw new ServicoException("Não é possivel adicionar o motorista na fila somente com um reboque.");

                conjuntoVeiculoMotorista = Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(veiculo);

                ValidarConjuntoVeiculo(conjuntoVeiculoMotorista, centroCarregamento?.NaoValidarIntegracaoGR ?? false);
                ValidarTransportadorIgual(motorista, veiculo);
                ValidarConjuntoVeiculoExclusivoOutroCentroCarregamento(conjuntoVeiculoMotorista, centroCarregamento);
            }

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista()
            {
                ConjuntoVeiculo = conjuntoVeiculoMotorista,
                CentroCarregamento = centroCarregamento,
                DataEntrada = DateTime.Now,
                DistanciaCentroCarregamento = 0,
                LojaProximidade = false,
                Motorista = motorista,
                Situacao = SituacaoFilaCarregamentoMotorista.Disponivel
            };

            try
            {
                _unitOfWorkContainer.Start();

                if (conjuntoVeiculoMotorista != null)
                    new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork).Inserir(conjuntoVeiculoMotorista);

                new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork).Inserir(filaCarregamentoMotorista);

                FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, _origemAlteracao);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.VincularFilaCarregamentoMotorista(filaCarregamentoMotorista, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                if (filaCarregamentoVeiculo == null)
                    NotificarFilaAlterada(filaCarregamentoMotorista);
                else
                    servicoFilaCarregamentoVeiculo.NotificarAlteracao(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }

            return filaCarregamentoMotorista;
        }

        public void AlterarSituacao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista, SituacaoFilaCarregamentoMotorista novaSituacao)
        {
            if (filaCarregamentoMotorista != null)
            {
                Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorioFilaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork);

                filaCarregamentoMotorista.Situacao = novaSituacao;

                repositorioFilaCarregamentoMotorista.Atualizar(filaCarregamentoMotorista);
            }
        }

        public void Remover(int codigoFilaCarregamentoMotorista)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorioFilaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = repositorioFilaCarregamentoMotorista.BuscarPorCodigo(codigoFilaCarregamentoMotorista) ?? throw new ServicoException("Fila do motorista não encontrada");

            ValidarMotoristaNaFilaCarregamento(filaCarregamentoMotorista);
            Remover(filaCarregamentoMotorista);
            NotificarFilaAlterada(filaCarregamentoMotorista);
        }

        public void RemoverPorMotoristaJornadaFinalizada(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorioFilaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = repositorioFilaCarregamentoMotorista.BuscarAtivaPorMotorista(motorista.Codigo);

            if (filaCarregamentoMotorista != null)
            {
                Remover(filaCarregamentoMotorista);
                NotificarFilaAlterada(filaCarregamentoMotorista);
            }
        }

        #endregion
    }
}
