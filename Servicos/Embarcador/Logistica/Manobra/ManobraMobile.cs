using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class ManobraMobile
    {
        #region Atributos Privados Somente Leitura

        private readonly ManobraBase _servicoManobraBase;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _urlBaseOrigemRequisicao;

        #endregion

        #region Construtores

        public ManobraMobile(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, urlBaseOrigemRequisicao: string.Empty)
        {
        }

        public ManobraMobile(Repositorio.UnitOfWork unitOfWork, string urlBaseOrigemRequisicao)
        {
            _servicoManobraBase = new ManobraBase(unitOfWork);
            _unitOfWork = unitOfWork;
            _urlBaseOrigemRequisicao = urlBaseOrigemRequisicao;
        }

        #endregion

        #region Métodos Privados

        private bool IsSituacaoManobraPermiteReserva(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            return (manobra.Situacao == SituacaoManobra.AguardandoManobra);
        }

        private bool IsSituacaoManobraTracaoPermiteDesvincularMotorista(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            return (manobraTracao.Situacao == SituacaoManobraTracao.Ocioso);
        }

        private bool IsSituacaoManobraTracaoPermiteIniciarIntervalo(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            return (manobraTracao.Situacao == SituacaoManobraTracao.Ocioso);
        }

        private bool IsSituacaoManobraTracaoPermiteFinalizarIntervalo(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            return (manobraTracao.Situacao == SituacaoManobraTracao.EmIntervalo);
        }

        private bool IsSituacaoManobraTracaoPermiteReserva(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            return (manobraTracao.Situacao == SituacaoManobraTracao.Ocioso) || (manobraTracao.Situacao == SituacaoManobraTracao.EmManobra);
        }

        private void NotificarManobraAlterada(int codigoManobra)
        {
            new Hubs.Manobra().CriarNotificaoManobraAlteradaOutroAmbiente(codigoManobra, _urlBaseOrigemRequisicao);
        }

        private void NotificarManobraTracaoRemovida(int codigoManobra)
        {
            new Hubs.Manobra().CriarNotificaoManobraTracaoRemovidaOutroAmbiente(codigoManobra, _urlBaseOrigemRequisicao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra ObterManobraMobileRetornar(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            GestaoPatio.Higienizacao servicoHigienizacao = new GestaoPatio.Higienizacao(_unitOfWork);
            bool veiculoHigienizado = servicoHigienizacao.IsVeiculosHigienizados(manobra.Tracao, manobra.Reboques);

            return new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra()
            {
                Codigo = manobra.Codigo,
                Acao = manobra.Acao.Descricao,
                Higienizado = veiculoHigienizado,
                HigienizadoDescricao = (veiculoHigienizado ? "Higienizado" : "Não Higienizado"),
                LocalAtual = manobra.ObterLocalAtual()?.DescricaoAcao ?? "",
                LocalDestino = manobra.LocalDestino?.DescricaoAcao ?? "",
                PlacaTracao = manobra.Tracao?.Placa_Formatada,
                PlacasReboques = string.Join(", ", (from reboque in manobra.Reboques select reboque.Placa_Formatada)),
                TempoAguardando = manobra.ObterTempoAguardando(),
                Situacao = manobra.Situacao
            };
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao DesvincularManobraTracao(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorioManobraTracao.BuscarPorTracao(veiculo.Codigo);

            if (manobraTracao?.Motorista?.Codigo != motorista.Codigo)
                return null;

            if (!IsSituacaoManobraTracaoPermiteDesvincularMotorista(manobraTracao))
                throw new ServicoException($"A situação da tração de manobra {veiculo.Placa_Formatada} não permite desvincular o motorista da tração atual");

            manobraTracao.Motorista = null;
            manobraTracao.Situacao = SituacaoManobraTracao.SemMotorista;

            repositorioManobraTracao.Atualizar(manobraTracao);

            return manobraTracao;
        }

        public void IniciarIntervalo(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = _servicoManobraBase.ObterManobraTracaoPorMotorista(motorista) ?? throw new ServicoException("Tração de manobra não encontrada");

            if (!IsSituacaoManobraTracaoPermiteIniciarIntervalo(manobraTracao))
                throw new ServicoException("A situação da tração de manobra não permite iniciar o intervalo");

            manobraTracao.Situacao = SituacaoManobraTracao.EmIntervalo;

            try
            {
                _unitOfWork.Start();

                new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork).Atualizar(manobraTracao);

                _unitOfWork.CommitChanges();

                NotificarManobraTracaoAlterada(manobraTracao.Codigo);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void FinalizarIntervalo(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = _servicoManobraBase.ObterManobraTracaoPorMotorista(motorista) ?? throw new ServicoException("Tração de manobra não encontrada");

            if (!IsSituacaoManobraTracaoPermiteFinalizarIntervalo(manobraTracao))
                throw new ServicoException("A situação da tração de manobra não permite finalizar o intervalo");

            manobraTracao.Situacao = SituacaoManobraTracao.Ocioso;

            try
            {
                _unitOfWork.Start();

                new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork).Atualizar(manobraTracao);

                _unitOfWork.CommitChanges();

                NotificarManobraTracaoAlterada(manobraTracao.Codigo);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void FinalizarManobra(int codigoManobra, Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local)
        {
            Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> registrosAlterados = _servicoManobraBase.FinalizarManobra(codigoManobra, local);

            NotificarManobraAlterada(registrosAlterados.Item1.Codigo);

            if (registrosAlterados.Item2 != null)
                NotificarManobraTracaoAlterada(registrosAlterados.Item2.Codigo);
        }

        public void NotificarManobraTracaoAlterada(int codigoManobra)
        {
            new Hubs.Manobra().CriarNotificaoManobraTracaoAlteradaOutroAmbiente(codigoManobra, _urlBaseOrigemRequisicao);
        }

        public void RemoverManobraTracaoVinculada(int codigoManobra)
        {
            Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> registrosAlterados = _servicoManobraBase.RemoverManobraTracaoVinculada(codigoManobra);

            NotificarManobraAlterada(registrosAlterados.Item1.Codigo);
            NotificarManobraTracaoAlterada(registrosAlterados.Item2.Codigo);
        }

        public void RemoverReservaManobra(Dominio.Entidades.Usuario motorista, int codigoManobra)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = _servicoManobraBase.ObterManobraTracaoPorMotorista(motorista) ?? throw new ServicoException("Tração de manobra não encontrada");
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = repositorioManobra.BuscarPorCodigo(codigoManobra) ?? throw new ServicoException("Manobra não encontrada");

            if (manobraTracao.ManobraReservada?.Codigo != manobra.Codigo)
                throw new ServicoException("Não é possivel remover uma reserva que não pertença a sua tração de manobra");

            _servicoManobraBase.RemoverReservaManobra(manobra, manobraTracao);

            NotificarManobraAlterada(codigoManobra);
        }

        public void ReservarManobra(Dominio.Entidades.Usuario motorista, int codigoManobra)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = _servicoManobraBase.ObterManobraTracaoPorMotorista(motorista) ?? throw new ServicoException("Tração de manobra não encontrada");
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = repositorioManobra.BuscarPorCodigo(codigoManobra) ?? throw new ServicoException("Manobra não encontrada");

            if (!IsSituacaoManobraPermiteReserva(manobra))
                throw new ServicoException("A situação da manobra não permite a reserva");

            if (!IsSituacaoManobraTracaoPermiteReserva(manobraTracao))
                throw new ServicoException("A situação da tração de manobra não permite a reserva");

            if (manobraTracao.ManobraReservada != null)
                throw new ServicoException("A tração de manobra já possui uma reserva");

            manobra.Situacao = SituacaoManobra.Reservada;
            manobraTracao.ManobraReservada = manobra;

            try
            {
                _unitOfWork.Start();

                repositorioManobra.Atualizar(manobra);
                new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork).Atualizar(manobraTracao);

                _servicoManobraBase.AdicionarHistoricoManobra(manobra, $"Manobra reservada para a tração {manobraTracao.Tracao.Placa_Formatada}");

                _unitOfWork.CommitChanges();

                NotificarManobraAlterada(codigoManobra);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void VincularManobraTracao(Dominio.Entidades.Usuario motorista, int codigoManobra, string placa)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = _servicoManobraBase.ObterManobraTracaoPorMotorista(motorista) ?? throw new ServicoException("Tração de manobra não encontrada");
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork).BuscarPorCodigo(codigoManobra) ?? throw new ServicoException("Manobra não encontrada");

            if (manobra.Tracao != null)
                throw new ServicoException("A situação da manobra não permite o vínculo");

            if (manobra.Reboques.FirstOrDefault().Placa != placa)
                throw new ServicoException("O reboque informado não é o mesmo da manobra");

            _servicoManobraBase.VincularManobraTracao(manobra, manobraTracao);

            NotificarManobraAlterada(manobra.Codigo);
            NotificarManobraTracaoAlterada(manobraTracao.Codigo);
        }

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao VincularManobraTracao(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorioManobraTracao.BuscarPorTracao(veiculo.Codigo);

            if (manobraTracao == null)
                return null;

            manobraTracao.Motorista = motorista;

            if (manobraTracao.Situacao == SituacaoManobraTracao.SemMotorista)
                manobraTracao.Situacao = SituacaoManobraTracao.Ocioso;

            repositorioManobraTracao.Atualizar(manobraTracao);

            return manobraTracao;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra> ObterManobrasPorMotorista(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.Manobra> manobras = repositorioManobra.BuscarManobrasPorMotorista(motorista.Codigo);

            return (
               from manobra in manobras
               select ObterManobraMobileRetornar(manobra)
           ).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra> ObterManobrasPorMotoristaTracaoManobra(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = _servicoManobraBase.ObterManobraTracaoPorMotorista(motorista) ?? throw new ServicoException("Tração de manobra não encontrada");
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraDisponivel filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraDisponivel()
            {
                CodigoCentroCarregamento = manobraTracao.CentroCarregamento.Codigo,
                CodigoManobraAtual = manobraTracao.ManobraAtual?.Codigo ?? 0,
            };

            List<Dominio.Entidades.Embarcador.Logistica.Manobra> listaManobraDisponiveis = repositorioManobra.BuscarDisponivel(filtrosPesquisa);

            return (
                from manobra in listaManobraDisponiveis
                select ObterManobraMobileRetornar(manobra)
            ).ToList();
        }

        #endregion
    }
}
