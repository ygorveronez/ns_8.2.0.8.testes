using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoReversa
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWorkContainer _unitOfWorkContainer;
        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion

        #region Construtores

        public FilaCarregamentoVeiculoReversa(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, usuario: null) { }

        public FilaCarregamentoVeiculoReversa(Repositorio.UnitOfWorkContainer unitOfWorkContainer) : this(unitOfWorkContainer, usuario: null) { }

        public FilaCarregamentoVeiculoReversa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWorkContainer = new Repositorio.UnitOfWorkContainer(unitOfWork);
            _usuario = usuario;
        }

        public FilaCarregamentoVeiculoReversa(Repositorio.UnitOfWorkContainer unitOfWorkContainer, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWorkContainer = unitOfWorkContainer;
            _usuario = usuario;
        }

        #endregion

        #region Métodos Privados

        private void AdicionarManobraFimDescarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            if (!filaCarregamentoVeiculoReversa.CentroCarregamento.UtilizarControleManobra)
                return;

            if (!IsGerarManobraFimDescarregamento(filaCarregamentoVeiculoReversa))
                return;

            Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar manobraAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar()
            {
                CentroCarregamento = filaCarregamentoVeiculoReversa.CentroCarregamento,
                Acao = filaCarregamentoVeiculoReversa.CentroCarregamento.AcaoManobraPadraoFimReversa,
                Reboques = filaCarregamentoVeiculoReversa.ConjuntoVeiculo.Reboques?.ToList(),
                Tracao = filaCarregamentoVeiculoReversa.ConjuntoVeiculo.Tracao
            };

            new Manobra(_unitOfWorkContainer.UnitOfWork).AdicionarManobra(manobraAdicionar);
        }

        private void AdicionarManobraInicioDescarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            if (!filaCarregamentoVeiculoReversa.CentroCarregamento.UtilizarControleManobra)
                return;

            if (!IsGerarManobraInicioDescarregamento(filaCarregamentoVeiculoReversa))
                return;

            Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar manobraAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar()
            {
                CentroCarregamento = filaCarregamentoVeiculoReversa.CentroCarregamento,
                Acao = filaCarregamentoVeiculoReversa.CentroCarregamento.AcaoManobraPadraoInicioReversa,
                LocalDestino = filaCarregamentoVeiculoReversa.LocalDescarregamento,
                LocalDestinoObrigatorio = true,
                Reboques = filaCarregamentoVeiculoReversa.ConjuntoVeiculo.Reboques?.ToList(),
                Tracao = filaCarregamentoVeiculoReversa.ConjuntoVeiculo.Tracao
            };

            new Manobra(_unitOfWorkContainer.UnitOfWork).AdicionarManobra(manobraAdicionar);
        }

        private bool IsGerarManobraFimDescarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            return (
                filaCarregamentoVeiculoReversa.CentroCarregamento.UtilizarControleManobra &&
                filaCarregamentoVeiculoReversa.CentroCarregamento.AcaoManobraPadraoFimReversa != null
            );
        }

        private bool IsGerarManobraInicioDescarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            return (
                filaCarregamentoVeiculoReversa.CentroCarregamento.UtilizarControleManobra &&
                filaCarregamentoVeiculoReversa.CentroCarregamento.AcaoManobraPadraoInicioReversa != null
            );
        }

        private bool IsLiberarFilaCarregamentoVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            return (filaCarregamentoVeiculoReversa.FilaCarregamentoVeiculo != null);
        }

        private bool IsSituacaoPermiteCancelarDescarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            return (filaCarregamentoVeiculoReversa.Situacao == SituacaoFilaCarregamentoVeiculoReversa.AguardandoDescarregamento);
        }

        private bool IsSituacaoPermiteFinalizarDescarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            return (filaCarregamentoVeiculoReversa.Situacao == SituacaoFilaCarregamentoVeiculoReversa.EmDescarregamento);
        }

        private bool IsSituacaoPermiteIniciarDescarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            return (filaCarregamentoVeiculoReversa.Situacao == SituacaoFilaCarregamentoVeiculoReversa.AguardandoDescarregamento);
        }

        private bool IsUtilizaFilaCarregamentoReversa()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWorkContainer.UnitOfWork);

            return repConfiguracaoTMS.BuscarConfiguracaoPadrao().UtilizarFilaCarregamentoReversa;
        }

        private void NotificarFilaCarregamentoReversaAlterada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoReversaAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoReversaAlteracao()
            {
                CodigoCentroCarregamento = filaCarregamentoVeiculoReversa.CentroCarregamento.Codigo,
                CodigoGrupoModeloVeicularCarga = filaCarregamentoVeiculoReversa.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular?.Codigo ?? 0,
                CodigoModeloVeicularCarga = filaCarregamentoVeiculoReversa.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
            };

            new Hubs.FilaCarregamentoReversa().NotificarTodosFilaAlterada(alteracao);
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(int codigo)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWorkContainer.UnitOfWork);

            return repositorioCentroCarregamento.BuscarPorCodigo(codigo) ?? throw new ServicoException("Centro de carregamento não encontrado.");
        }

        private Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao ObterLocal(int codigoLocal)
        {
            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPorCodigo(codigoLocal) ?? throw new ServicoException("Local não encontrado");
        }

        private Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga ObterTipoRetornoCarga(int codigoTipoRetornoCarga)
        {
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorio = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoRetornoCarga) ?? throw new ServicoException("Tipo de retorno de carga não encontrado");
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(int codigo)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorioVeiculo.BuscarPorCodigo(codigo) ?? throw new ServicoException("Veículo não encontrado.");
        }

        private void RemoverReversaFilaCarregamentoVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa)
        {
            if (IsLiberarFilaCarregamentoVeiculo(filaCarregamentoVeiculoReversa))
            {
                FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new FilaCarregamentoVeiculo(_unitOfWorkContainer, OrigemAlteracaoFilaCarregamento.Embarcador);

                servicoFilaCarregamentoVeiculo.RemoverReversa(filaCarregamentoVeiculoReversa.FilaCarregamentoVeiculo);
            }
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (!IsUtilizaFilaCarregamentoReversa() || (filaCarregamentoVeiculo.Tipo != TipoFilaCarregamentoVeiculo.Reversa) || (filaCarregamentoVeiculo.CentroCarregamento == null))
                return;

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(filaCarregamentoVeiculo.ConjuntoVeiculo);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa()
            {
                CentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento,
                ConjuntoVeiculo = conjuntoVeiculo,
                DataCriacao = DateTime.Now,
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                Situacao = SituacaoFilaCarregamentoVeiculoReversa.AguardandoDescarregamento,
                TipoRetornoCarga = filaCarregamentoVeiculo.TipoRetornoCarga
            };

            Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repositorioFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa repositorioFilaCarregamentoVeiculoReversa = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(_unitOfWorkContainer.UnitOfWork);

            try
            {
                _unitOfWorkContainer.Start();

                repositorioFilaCarregamentoConjuntoVeiculo.Inserir(conjuntoVeiculo);
                repositorioFilaCarregamentoVeiculoReversa.Inserir(filaCarregamentoVeiculoReversa);

                new FilaCarregamentoVeiculoReversaHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(filaCarregamentoVeiculoReversa, "Adicionada a reversa");

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaCarregamentoReversaAlterada(filaCarregamentoVeiculoReversa);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void Adicionar(int codigoVeiculo, int codigoCentroCarregamento, int codigoTipoRetornoCarga)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(codigoVeiculo);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(veiculo);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa()
            {
                CentroCarregamento = ObterCentroCarregamento(codigoCentroCarregamento),
                ConjuntoVeiculo = conjuntoVeiculo,
                DataCriacao = DateTime.Now,
                Situacao = SituacaoFilaCarregamentoVeiculoReversa.AguardandoDescarregamento,
                TipoRetornoCarga = ObterTipoRetornoCarga(codigoTipoRetornoCarga)
            };

            Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repositorioFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa repositorioFilaCarregamentoVeiculoReversa = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(_unitOfWorkContainer.UnitOfWork);

            try
            {
                _unitOfWorkContainer.Start();

                repositorioFilaCarregamentoConjuntoVeiculo.Inserir(conjuntoVeiculo);
                repositorioFilaCarregamentoVeiculoReversa.Inserir(filaCarregamentoVeiculoReversa);

                new FilaCarregamentoVeiculoReversaHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(filaCarregamentoVeiculoReversa, "Adicionada a reversa");

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaCarregamentoReversaAlterada(filaCarregamentoVeiculoReversa);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void CancelarDescarregamento(int codigoFilaCarregamentoReversa)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa repositorioFilaCarregamentoVeiculoReversa = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa = repositorioFilaCarregamentoVeiculoReversa.BuscarPorCodigo(codigoFilaCarregamentoReversa, auditavel: true) ?? throw new ServicoException("Fila de reversa não encontrada");

            if (!IsSituacaoPermiteCancelarDescarregamento(filaCarregamentoVeiculoReversa))
                throw new ServicoException("Situação não permite cancelar o descarregamento");

            filaCarregamentoVeiculoReversa.DataFim = DateTime.Now;
            filaCarregamentoVeiculoReversa.Situacao = SituacaoFilaCarregamentoVeiculoReversa.Cancelada;

            try
            {
                _unitOfWorkContainer.Start();

                repositorioFilaCarregamentoVeiculoReversa.Atualizar(filaCarregamentoVeiculoReversa);

                _unitOfWorkContainer.CommitChanges();

                new FilaCarregamentoVeiculoReversaHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(filaCarregamentoVeiculoReversa, "Reversa cancelada");

                NotificarFilaCarregamentoReversaAlterada(filaCarregamentoVeiculoReversa);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }

            RemoverReversaFilaCarregamentoVeiculo(filaCarregamentoVeiculoReversa);
            AdicionarManobraFimDescarregamento(filaCarregamentoVeiculoReversa);
        }

        public void FinalizarDescarregamento(int codigoFilaCarregamentoReversa)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa repositorioFilaCarregamentoVeiculoReversa = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa = repositorioFilaCarregamentoVeiculoReversa.BuscarPorCodigo(codigoFilaCarregamentoReversa, auditavel: true) ?? throw new ServicoException("Fila de reversa não encontrada");

            if (!IsSituacaoPermiteFinalizarDescarregamento(filaCarregamentoVeiculoReversa))
                throw new ServicoException("Situação não permite finalizar o descarregamento");

            filaCarregamentoVeiculoReversa.DataFim = DateTime.Now;
            filaCarregamentoVeiculoReversa.Situacao = SituacaoFilaCarregamentoVeiculoReversa.Finalizada;

            try
            {
                _unitOfWorkContainer.Start();

                repositorioFilaCarregamentoVeiculoReversa.Atualizar(filaCarregamentoVeiculoReversa);

                new FilaCarregamentoVeiculoReversaHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(filaCarregamentoVeiculoReversa, "Finalizado o descarregamento");

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaCarregamentoReversaAlterada(filaCarregamentoVeiculoReversa);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }

            RemoverReversaFilaCarregamentoVeiculo(filaCarregamentoVeiculoReversa);
            AdicionarManobraFimDescarregamento(filaCarregamentoVeiculoReversa);
        }

        public void IniciarDescarregamento(int codigoFilaCarregamentoReversa, int codigoLocal)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa repositorioFilaCarregamentoVeiculoReversa = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa filaCarregamentoVeiculoReversa = repositorioFilaCarregamentoVeiculoReversa.BuscarPorCodigo(codigoFilaCarregamentoReversa, auditavel: false) ?? throw new ServicoException("Fila de reversa não encontrada");

            if (!IsSituacaoPermiteIniciarDescarregamento(filaCarregamentoVeiculoReversa))
                throw new ServicoException("Situação não permite iniciar o descarregamento");

            filaCarregamentoVeiculoReversa.DataInicio = DateTime.Now;
            filaCarregamentoVeiculoReversa.LocalDescarregamento = ObterLocal(codigoLocal);
            filaCarregamentoVeiculoReversa.Situacao = SituacaoFilaCarregamentoVeiculoReversa.EmDescarregamento;

            try
            {
                _unitOfWorkContainer.Start();

                repositorioFilaCarregamentoVeiculoReversa.Atualizar(filaCarregamentoVeiculoReversa);

                new FilaCarregamentoVeiculoReversaHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(filaCarregamentoVeiculoReversa, $"Iniciado o descarregamento no local {filaCarregamentoVeiculoReversa.LocalDescarregamento.DescricaoAcao}");

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaCarregamentoReversaAlterada(filaCarregamentoVeiculoReversa);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }

            AdicionarManobraInicioDescarregamento(filaCarregamentoVeiculoReversa);
        }

        #endregion
    }
}
