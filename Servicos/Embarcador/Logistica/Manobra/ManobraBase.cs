using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    class ManobraBase
    {
        #region Atributos Privados Somente Leitura

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ManobraBase(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null)
        {
        }

        public ManobraBase(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void AtualizarLocalAtual(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            if (manobra.Tracao != null)
            {
                manobra.Tracao.LocalAtual = local;

                repositorioVeiculo.Atualizar(manobra.Tracao);
            }

            if (manobra.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in manobra.Reboques)
                {
                    reboque.LocalAtual = local;

                    repositorioVeiculo.Atualizar(reboque);
                }
            }
        }

        private bool IsSituacaoManobraPermiteDesvincular(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            return (manobra.Situacao == SituacaoManobra.EmManobra);
        }

        private bool IsSituacaoManobraPermiteFinalizar(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            return (manobra.Situacao == SituacaoManobra.EmManobra);
        }

        private bool IsSituacaoManobraPermiteRemoverReserva(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            return (manobra.Situacao == SituacaoManobra.Reservada);
        }

        private bool IsSituacaoManobraPermiteVincular(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            return (manobra.Situacao == SituacaoManobra.AguardandoManobra) || (manobra.Situacao == SituacaoManobra.Reservada);
        }

        private bool IsSituacaoManobraTracaoPermiteVincular(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            return (manobraTracao.Situacao == SituacaoManobraTracao.Ocioso);
        }

        private Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao ObterLocal(int codigoLocal)
        {
            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(_unitOfWork);

            return repositorio.BuscarPorCodigo(codigoLocal) ?? throw new ServicoException("Local não encontrado");
        }

        private void RealizarAcaoHigienizacao(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            GestaoPatio.Higienizacao servicoHigienizacao = new GestaoPatio.Higienizacao(_unitOfWork);

            servicoHigienizacao.AtualizarVeiculosParaHigienizado(manobra.Tracao, manobra.Reboques);

            if ((manobra.OcorrenciaPatio != null) && (manobra.OcorrenciaPatio.Situacao == SituacaoOcorrenciaPatio.Pendente))
            {
                Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio repositorioOcorrenciaPatio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio(_unitOfWork);

                manobra.OcorrenciaPatio.Situacao = SituacaoOcorrenciaPatio.Aprovada;

                repositorioOcorrenciaPatio.Atualizar(manobra.OcorrenciaPatio);
            }
        }

        private void RealizarAcaoInicioCarregamento(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            ControleCarregamento servicoControleCarregamento = new ControleCarregamento(_unitOfWork);

            if (manobra.Reboques?.Count > 0)
                servicoControleCarregamento.ChegadaDocaPorVeiculo(manobra.Reboques.FirstOrDefault().Codigo);
            else
                servicoControleCarregamento.ChegadaDocaPorVeiculo(manobra.Tracao.Codigo);
        }

        private void RealizarAcaoManobraFinalizada(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            switch (manobra.Acao.Tipo)
            {
                case TipoManobraAcao.InicioCarregamento:
                    RealizarAcaoInicioCarregamento(manobra);
                    break;

                case TipoManobraAcao.Higienizacao:
                    RealizarAcaoHigienizacao(manobra);
                    break;
            }
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarHistoricoManobra(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, string descricao)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraHistorico historico = new Dominio.Entidades.Embarcador.Logistica.ManobraHistorico()
            {
                Data = DateTime.Now,
                Descricao = descricao,
                Manobra = manobra,
                Usuario = _auditado?.Usuario
            };

            new Repositorio.Embarcador.Logistica.ManobraHistorico(_unitOfWork).Inserir(historico);
        }

        public void AdicionarHistoricoManobraTracao(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            AdicionarHistoricoManobraTracao(manobra, manobraTracao.Tracao, manobraTracao.Motorista);
        }

        public void AdicionarHistoricoManobraTracao(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, Dominio.Entidades.Veiculo tracao, Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico historico = new Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico()
            {
                Manobra = manobra,
                Motorista = motorista,
                Tracao = tracao,
                Usuario = _auditado?.Usuario
            };

            new Repositorio.Embarcador.Logistica.ManobraTracaoHistorico(_unitOfWork).Inserir(historico);
        }

        public Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> FinalizarManobra(int codigoManobra, Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local)
        {
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = repositorioManobra.BuscarPorCodigo(codigoManobra, auditavel: true) ?? throw new ServicoException("Manobra não encontrada");

            if (!IsSituacaoManobraPermiteFinalizar(manobra))
                throw new ServicoException("A situação da manobra não permite a finalização");

            if ((manobra.LocalDestino != null) && (manobra.LocalDestino.Codigo != local.Codigo))
                throw new ServicoException($"O local informado diferente do local de destino da manobra");

            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorioManobraTracao.BuscarPorManobraAtual(codigoManobra);

            try
            {
                _unitOfWork.Start();

                manobra.DataFim = DateTime.Now;
                manobra.Situacao = SituacaoManobra.Finalizada;

                repositorioManobra.Atualizar(manobra);

                AtualizarLocalAtual(manobra, local);
                RealizarAcaoManobraFinalizada(manobra);
                AdicionarHistoricoManobra(manobra, "Manobra finalizada");

                Dominio.Entidades.Usuario veiculoMotorista = null;
                if (manobra.Tracao != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(manobra.Tracao.Codigo);

                if (manobraTracao != null)
                {
                    manobraTracao.ManobraAtual = null;
                    manobraTracao.Situacao = SituacaoManobraTracao.Ocioso;

                    repositorioManobraTracao.Atualizar(manobraTracao);
                    AdicionarHistoricoManobraTracao(manobra, manobraTracao);
                }
                else if (veiculoMotorista != null)
                    AdicionarHistoricoManobraTracao(manobra, manobra.Tracao, veiculoMotorista);

                _unitOfWork.CommitChanges();

                Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> registrosAlterados =
                    new Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao>(manobra, manobraTracao);

                return registrosAlterados;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao ObterManobraTracaoPorMotorista(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = null;
            Dominio.Entidades.Veiculo veiculo = new Repositorio.Veiculo(_unitOfWork).BuscarPorMotorista(motorista.Codigo);

            if (veiculo != null)
                manobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork).BuscarPorTracao(veiculo.Codigo);

            return manobraTracao;
        }

        public Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> RemoverManobraTracaoVinculada(int codigoManobra)
        {
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = repositorioManobra.BuscarPorCodigo(codigoManobra, auditavel: true) ?? throw new ServicoException("Manobra não encontrada");

            if (!IsSituacaoManobraPermiteDesvincular(manobra))
                throw new ServicoException("A situação da manobra não permite a remoção");

            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorioManobraTracao.BuscarAtivaPorManobraAtual(codigoManobra) ?? throw new ServicoException("Tração de manobra não encontrada");

            manobra.DataInicio = null;
            manobra.Situacao = SituacaoManobra.AguardandoManobra;

            manobraTracao.ManobraAtual = null;
            manobraTracao.Situacao = SituacaoManobraTracao.Ocioso;

            try
            {
                _unitOfWork.Start();

                repositorioManobra.Atualizar(manobra);
                repositorioManobraTracao.Atualizar(manobraTracao);

                AdicionarHistoricoManobra(manobra, $"Removida a tração {manobraTracao.Tracao.Placa_Formatada}");

                _unitOfWork.CommitChanges();

                Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> registrosAlterados =
                    new Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao>(manobra, manobraTracao);

                return registrosAlterados;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.Manobra RemoverReservaManobra(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            if (!IsSituacaoManobraPermiteRemoverReserva(manobra))
                throw new ServicoException("A situação da manobra não permite remover a reserva");

            manobra.Situacao = SituacaoManobra.AguardandoManobra;
            manobraTracao.ManobraReservada = null;

            try
            {
                _unitOfWork.Start();

                new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork).Atualizar(manobra);
                new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork).Atualizar(manobraTracao);

                AdicionarHistoricoManobra(manobra, $"Removida a reserva da tração {manobraTracao.Tracao.Placa_Formatada}");

                _unitOfWork.CommitChanges();

                return manobra;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void VincularManobraTracao(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            if (!IsSituacaoManobraPermiteVincular(manobra))
                throw new ServicoException("A situação da manobra não permite o vínculo");

            if (!IsSituacaoManobraTracaoPermiteVincular(manobraTracao))
                throw new ServicoException("A situação da tração de manobra não permite o vínculo");

            if (manobra.Tracao != null)
                throw new ServicoException("A manobra possui uma tração. Não é permitido o vínculo");

            manobra.DataInicio = DateTime.Now;
            manobra.Situacao = SituacaoManobra.EmManobra;

            manobraTracao.ManobraAtual = manobra;
            manobraTracao.Situacao = SituacaoManobraTracao.EmManobra;

            try
            {
                _unitOfWork.Start();

                new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork).Atualizar(manobra);
                new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork).Atualizar(manobraTracao);

                AdicionarHistoricoManobra(manobra, $"Iniciada a manobra com a tração {manobraTracao.Tracao.Placa_Formatada}");

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        #endregion
    }
}
