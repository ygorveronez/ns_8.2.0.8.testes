using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class Manobra
    {
        #region Atributos Privados Somente Leitura

        private readonly ManobraBase _servicoManobraBase;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Manobra(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null)
        {
        }

        public Manobra(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _servicoManobraBase = new ManobraBase(unitOfWork, auditado);
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void AdicionarHistoricoManobraTracao(Dominio.Entidades.Embarcador.Logistica.Manobra manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico historico = new Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico()
            {
                Manobra = manobra,
                Motorista = manobraTracao.Motorista,
                Tracao = manobraTracao.Tracao,
                Usuario = _auditado?.Usuario
            };

            new Repositorio.Embarcador.Logistica.ManobraTracaoHistorico(_unitOfWork).Inserir(historico);
        }

        private void GerarOcorrenciaPatioChecklist(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            if (manobra.Acao.Tipo != TipoManobraAcao.Checklist)
                return;

            GestaoPatio.OcorrenciaPatio servicoOcorrenciaPatio = new GestaoPatio.OcorrenciaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo tipo = servicoOcorrenciaPatio.ObterTipoOcorrenciaPorTipo(TipoOcorrenciaPatio.Checklist);

            if (tipo != null)
            {
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioAdicionar ocorrenciaPatioAdicionar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioAdicionar()
                {
                    CentroCarregamento = manobra.CentroCarregamento,
                    Descricao = "Ocorrência de checklist",
                    Reboques = manobra.Reboques,
                    Tipo = tipo,
                    TipoLancamento = TipoLancamento.Automatico,
                    Tracao = manobra.Tracao
                };

                servicoOcorrenciaPatio.Adicionar(ocorrenciaPatioAdicionar);
            }
        }

        private bool IsSituacaoManobraTracaoPermiteVincular(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            return (manobraTracao.Situacao == SituacaoManobraTracao.Ocioso);
        }

        private bool IsSituacaoPermiteRemoverManobra(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            return (manobra.Situacao == SituacaoManobra.AguardandoManobra) || (manobra.Situacao == SituacaoManobra.Reservada);
        }

        private bool IsSituacaoPermiteRemoverManobraTracao(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            return (manobraTracao.Situacao == SituacaoManobraTracao.Ocioso) || (manobraTracao.Situacao == SituacaoManobraTracao.SemMotorista);
        }

        private void NotificarManobraAlterada(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAlteracao()
            {
                CentroCarregamento = manobra.CentroCarregamento.Codigo
            };

            new Hubs.Manobra().NotificarTodosManobraAlterada(alteracao);
        }

        private void NotificarManobraTracaoAlterada(Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao)
        {
            if (manobraTracao != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraTracaoDados manobraTracaoDados = new Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraTracaoDados()
                {
                    AcaoAtual = manobraTracao.ManobraAtual?.Acao?.Descricao,
                    CentroCarregamento = manobraTracao.CentroCarregamento.Codigo,
                    ClasseCor = manobraTracao.Situacao.ObterClasseCor(),
                    Codigo = manobraTracao.Codigo,
                    DescricaoSituacao = manobraTracao.Situacao.ObterDescricao(),
                    Motorista = manobraTracao.Motorista?.Descricao,
                    Placa = manobraTracao.Tracao.Placa_Formatada,
                    Situacao = manobraTracao.Situacao,
                    Tracao = manobraTracao.Tracao.Codigo,
                    Transportador = manobraTracao.Motorista?.Empresa?.Descricao
                };

                new Hubs.Manobra().NotificarTodosManobraTracaoAlterada(manobraTracaoDados);
            }
        }

        private void NotificarMobile(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(manobra.Tracao.Codigo);

            if (veiculoMotorista != null)
            {
                HubsMobile.NotificacaoMobile servicoNotificacaoMobile = new HubsMobile.NotificacaoMobile();
                Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados dadosNotificacao = new Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados()
                {
                    Assunto = "Notificação de manobra",
                    CentroCarregamento = manobra.CentroCarregamento,
                    Mensagem = $"Manobra {manobra.Acao.Descricao} adicionada.{(manobra.LocalDestino == null ? "" : $" Por favor, dirija-se ao local {manobra.LocalDestino.DescricaoAcao}")}",
                    Tipo = TipoNotificacaoMobile.ManobraAdicionada,
                    Usuario = veiculoMotorista
                };

                servicoNotificacaoMobile.Notificar(dadosNotificacao, _unitOfWork);
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(int codigoCentroCarregamento)
        {
            if (codigoCentroCarregamento == 0)
                throw new ServicoException("O centro de carregamento deve ser informado");

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorio = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);

            return repositorio.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ServicoException("Centro de carregamento não encontrado");
        }

        private Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao ObterLocal(int codigoLocal)
        {
            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(_unitOfWork);

            return repositorio.BuscarPorCodigo(codigoLocal) ?? throw new ServicoException("Local não encontrado");
        }

        private Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao ObterLocalDestino(int codigoLocalDestino)
        {
            if (codigoLocalDestino == 0)
                return null;

            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorio = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(_unitOfWork);

            return repositorio.BuscarPorCodigo(codigoLocalDestino);
        }

        private Dominio.Entidades.Embarcador.Logistica.ManobraAcao ObterManobraAcao(int codigoManobraAcao)
        {
            if (codigoManobraAcao == 0)
                throw new ServicoException("Ação de manobra deve ser informada");

            Repositorio.Embarcador.Logistica.ManobraAcao repositorio = new Repositorio.Embarcador.Logistica.ManobraAcao(_unitOfWork);

            return repositorio.BuscarPorCodigo(codigoManobraAcao) ?? throw new ServicoException("Ação de manobra não encontrada");
        }

        private Dominio.Entidades.Usuario ObterMotorista(int codigoMotorista)
        {
            if (codigoMotorista == 0)
                return null;

            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);

            return repositorioMotorista.BuscarPorCodigo(codigoMotorista);
        }

        private Dominio.Entidades.Veiculo ObterTracao(int codigoTracao)
        {
            if (codigoTracao == 0)
                throw new ServicoException("A tração deve ser informada");

            Repositorio.Veiculo repositorio = new Repositorio.Veiculo(_unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTracao) ?? throw new ServicoException("Tração não encontrada");
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(int codigoVeiculo)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            return repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) ?? throw new ServicoException("Veículo não encontrado");
        }

        private void ValidarLocalDestino(Dominio.Entidades.Embarcador.Logistica.Manobra manobra)
        {
            if ((manobra.LocalDestino == null) && manobra.Acao.LocalDestinoObrigatorio)
                throw new ServicoException("O local de destino é obrigatório para a ação informada");
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarManobra(int codigoCentroCarregamento, int codigoVeiculo, int codigoManobraAcao, int codigoLocalDestino)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(codigoVeiculo);

            Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar manobraAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar()
            {
                CentroCarregamento = ObterCentroCarregamento(codigoCentroCarregamento),
                Acao = ObterManobraAcao(codigoManobraAcao),
                LocalDestino = ObterLocalDestino(codigoLocalDestino),
                LocalDestinoObrigatorio = true,
                Reboques = veiculo.IsTipoVeiculoTracao() ? veiculo.VeiculosVinculados?.ToList() : new List<Dominio.Entidades.Veiculo>() { veiculo },
                Tracao = veiculo.IsTipoVeiculoTracao() ? veiculo : null
            };

            AdicionarManobra(manobraAdicionar);
        }

        public void AdicionarManobra(Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar manobraAdicionar)
        {
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = new Dominio.Entidades.Embarcador.Logistica.Manobra();

            manobra.CentroCarregamento = manobraAdicionar.CentroCarregamento;
            manobra.Acao = manobraAdicionar.Acao;
            manobra.DataCriacao = DateTime.Now;
            manobra.DataMaximaIniciar = DateTime.Now;
            manobra.LocalDestino = manobraAdicionar.LocalDestino;
            manobra.OcorrenciaPatio = manobraAdicionar.OcorrenciaPatio;
            manobra.Reboques = manobraAdicionar.Reboques?.ToList();
            manobra.Situacao = SituacaoManobra.AguardandoManobra;
            manobra.Tracao = manobraAdicionar.Tracao;

            if (manobraAdicionar.LocalDestinoObrigatorio)
                ValidarLocalDestino(manobra);

            try
            {
                _unitOfWork.Start();

                new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork).Inserir(manobra);

                _servicoManobraBase.AdicionarHistoricoManobra(manobra, "Adicionada a manobra");

                if (manobraAdicionar.Tracao != null)
                {
                    manobra.DataInicio = DateTime.Now;
                    manobra.Situacao = SituacaoManobra.EmManobra;

                    _servicoManobraBase.AdicionarHistoricoManobra(manobra, $"Iniciada a manobra com a tração {manobraAdicionar.Tracao.Placa_Formatada} automaticamente");
                    NotificarMobile(manobra);
                }

                GerarOcorrenciaPatioChecklist(manobra);

                _unitOfWork.CommitChanges();

                NotificarManobraAlterada(manobra);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void AdicionarManobraTracao(int codigoCentroCarregamento, int codigoTracao, int codigoMotorista)
        {
            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracaoExistente = repositorioManobraTracao.BuscarPorTracao(codigoTracao);

            if (manobraTracaoExistente != null)
                throw new ServicoException($"A tração já está adicionada como manobra no CD {manobraTracaoExistente.CentroCarregamento.Descricao}");

            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = new Dominio.Entidades.Embarcador.Logistica.ManobraTracao();

            manobraTracao.CentroCarregamento = ObterCentroCarregamento(codigoCentroCarregamento);
            manobraTracao.ManobraReservada = null;
            manobraTracao.Motorista = ObterMotorista(codigoMotorista);
            manobraTracao.Situacao = (manobraTracao.Motorista == null) ? SituacaoManobraTracao.SemMotorista : SituacaoManobraTracao.Ocioso;
            manobraTracao.Tracao = ObterTracao(codigoTracao);
            manobraTracao.Tracao.TipoManobra = true;

            try
            {
                _unitOfWork.Start();

                repositorioManobraTracao.Inserir(manobraTracao);
                new Repositorio.Veiculo(_unitOfWork).Atualizar(manobraTracao.Tracao);

                _unitOfWork.CommitChanges();

                NotificarManobraTracaoAlterada(manobraTracao);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void FinalizarManobra(int codigoManobra, int codigoLocal)
        {
            Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local = ObterLocal(codigoLocal);

            Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> registrosAlterados = _servicoManobraBase.FinalizarManobra(codigoManobra, local);

            NotificarManobraAlterada(registrosAlterados.Item1);
            NotificarManobraTracaoAlterada(registrosAlterados.Item2);
        }

        public void NotificarManobraAlterada(int codigoManobra)
        {
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = repositorioManobra.BuscarPorCodigo(codigoManobra);

            if (manobra != null)
                NotificarManobraAlterada(manobra);
        }

        public void NotificarManobraTracaoAlterada(int codigoManobraTracao)
        {
            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobra = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorioManobra.BuscarPorCodigo(codigoManobraTracao);

            NotificarManobraTracaoAlterada(manobraTracao);
        }

        public void NotificarManobraTracaoRemovida(int codigoManobraTracao)
        {
            new Hubs.Manobra().NotificarTodosManobraTracaoRemovida(codigoManobraTracao);
        }

        public void RemoverManobra(int codigoManobra)
        {
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = repositorioManobra.BuscarPorCodigo(codigoManobra, auditavel: true) ?? throw new ServicoException("Manobra não encontrada");

            if (!IsSituacaoPermiteRemoverManobra(manobra))
                throw new ServicoException("A situação da manobra não permite a remoção");

            try
            {
                _unitOfWork.Start();

                manobra.DataFim = DateTime.Now;
                manobra.Situacao = SituacaoManobra.Cancelada;

                repositorioManobra.Atualizar(manobra);

                _servicoManobraBase.AdicionarHistoricoManobra(manobra, "Manobra cancelada");

                _unitOfWork.CommitChanges();

                NotificarManobraAlterada(manobra);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void RemoverManobraTracao(int codigoManobraTracao)
        {
            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorioManobraTracao.BuscarPorCodigo(codigoManobraTracao, auditavel: true) ?? throw new ServicoException("Tração de manobra não encontrada");

            if (!IsSituacaoPermiteRemoverManobraTracao(manobraTracao))
                throw new ServicoException("A situação da tração de manobra não permite a remoção");

            try
            {
                _unitOfWork.Start();

                manobraTracao.Tracao.TipoManobra = false;

                new Repositorio.Veiculo(_unitOfWork).Atualizar(manobraTracao.Tracao);

                repositorioManobraTracao.Deletar(manobraTracao);

                _unitOfWork.CommitChanges();

                NotificarManobraTracaoRemovida(codigoManobraTracao);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void RemoverManobraTracaoVinculada(int codigoManobra)
        {
            Tuple<Dominio.Entidades.Embarcador.Logistica.Manobra, Dominio.Entidades.Embarcador.Logistica.ManobraTracao> registrosAlterados = _servicoManobraBase.RemoverManobraTracaoVinculada(codigoManobra);

            NotificarManobraAlterada(registrosAlterados.Item1);
            NotificarManobraTracaoAlterada(registrosAlterados.Item2);
        }

        public void RemoverReservaManobra(int codigoManobra)
        {
            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = repositorioManobra.BuscarPorCodigo(codigoManobra) ?? throw new ServicoException("Manobra não encontrada");

            Repositorio.Embarcador.Logistica.ManobraTracao repositorioManobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = repositorioManobraTracao.BuscarPorManobraReservada(codigoManobra) ?? throw new ServicoException("Tração de manobra não encontrada");

            _servicoManobraBase.RemoverReservaManobra(manobra, manobraTracao);

            NotificarManobraAlterada(manobra);
        }

        public void VincularManobraTracao(int codigoManobra, int codigoManobraTracao)
        {
            Dominio.Entidades.Embarcador.Logistica.Manobra manobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork).BuscarPorCodigo(codigoManobra) ?? throw new ServicoException("Manobra não encontrada");
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = new Repositorio.Embarcador.Logistica.ManobraTracao(_unitOfWork).BuscarPorCodigo(codigoManobraTracao) ?? throw new ServicoException("Tração de manobra não encontrada");

            _servicoManobraBase.VincularManobraTracao(manobra, manobraTracao);

            NotificarManobraAlterada(manobra);
            NotificarManobraTracaoAlterada(manobraTracao);
        }

        #endregion

        #region Métodos Públicos de Consulta

        public Dominio.Entidades.Embarcador.Logistica.ManobraAcao ObterManobraAcao(int codigoCentroCarregamento, TipoManobraAcao tipo)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao repositorioCentroCarregamentoManobraAcao = new Repositorio.Embarcador.Logistica.CentroCarregamentoManobraAcao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao centroCarregamentoManobraAcao = repositorioCentroCarregamentoManobraAcao.BuscarPorCentroCarregamentoETipoAcao(codigoCentroCarregamento, tipo);

            return centroCarregamentoManobraAcao?.Acao;
        }

        public Dominio.Entidades.Embarcador.Logistica.Manobra ObterManobraAtual(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = _servicoManobraBase.ObterManobraTracaoPorMotorista(motorista);

            if (manobraTracao != null)
                return manobraTracao.ManobraAtual;

            Repositorio.Embarcador.Logistica.Manobra repositorioManobra = new Repositorio.Embarcador.Logistica.Manobra(_unitOfWork);

            return repositorioManobra.BuscarManobrasPorMotorista(motorista.Codigo).FirstOrDefault();
        }

        #endregion
    }
}
