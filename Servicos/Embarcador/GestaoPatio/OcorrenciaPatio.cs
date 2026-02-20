using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class OcorrenciaPatio
    {
        #region Atributos Privados

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public OcorrenciaPatio(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public OcorrenciaPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void AtualizarDataValidadeGerenciadoraRisco(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo?.ModeloVeicularCarga?.DiasRealizarProximoChecklist > 0)
            {
                veiculo.DataValidadeGerenciadoraRisco = DateTime.Now.Date.AddDays(veiculo.ModeloVeicularCarga.DiasRealizarProximoChecklist);

                new Repositorio.Veiculo(_unitOfWork).Atualizar(veiculo);
            }
        }

        private void GerarManobraHigienizacao(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio)
        {
            if (ocorrenciaPatio.OcorrenciaPatioTipo.Tipo != TipoOcorrenciaPatio.Higienizacao)
                return;

            if (!ocorrenciaPatio.CentroCarregamento.UtilizarControleManobra)
                return;

            Logistica.Manobra servicoManobra = new Logistica.Manobra(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ManobraAcao acao = servicoManobra.ObterManobraAcao(ocorrenciaPatio.CentroCarregamento.Codigo, TipoManobraAcao.Higienizacao);

            if (acao != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar manobraAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar()
                {
                    CentroCarregamento = ocorrenciaPatio.CentroCarregamento,
                    Acao = acao,
                    LocalDestino = null,
                    LocalDestinoObrigatorio = false,
                    OcorrenciaPatio = ocorrenciaPatio,
                    Reboques = ocorrenciaPatio.Reboques,
                    Tracao = ocorrenciaPatio.Tracao
                };

                servicoManobra.AdicionarManobra(manobraAdicionar);
            }
        }

        private bool IsSituacaoPermiteAprovar(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio)
        {
            return (ocorrenciaPatio.Situacao == SituacaoOcorrenciaPatio.Pendente);
        }

        private bool IsPermiteExcluir(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio)
        {
            return ((ocorrenciaPatio.TipoLancamento == TipoLancamento.Manual) && (ocorrenciaPatio.Situacao == SituacaoOcorrenciaPatio.Pendente));
        }

        private bool IsSituacaoPermiteReprovar(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio)
        {
            return (ocorrenciaPatio.Situacao == SituacaoOcorrenciaPatio.Pendente);
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(int codigoCentroCarregamento)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);

            return repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ServicoException("Centro de carregamento não encontrado");
        }

        private Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ObterTipo(int codigoTipo)
        {
            Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorioOcorrenciaPatioTipo = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(_unitOfWork);

            return repositorioOcorrenciaPatioTipo.BuscarPorCodigo(codigoTipo) ?? throw new ServicoException("Tipo de ocorrência de pátio não encontrado");
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(int codigoVeiculo)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            return repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) ?? throw new ServicoException("Veículo não encontrado");
        }

        private void RealizarAcaoChecklist(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio)
        {
            AtualizarDataValidadeGerenciadoraRisco(ocorrenciaPatio.Tracao);

            if (ocorrenciaPatio.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in ocorrenciaPatio.Reboques)
                    AtualizarDataValidadeGerenciadoraRisco(reboque);
            }
        }

        private void RealizarAcaoHigienizacao(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio)
        {
            Higienizacao servicoHigienizacao = new Higienizacao(_unitOfWork);

            servicoHigienizacao.AtualizarVeiculosParaHigienizado(ocorrenciaPatio.Tracao, ocorrenciaPatio.Reboques);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioDados dados)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(dados.CodigoVeiculo);

            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioAdicionar ocorrenciaPatioAdicionar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioAdicionar()
            {
                CentroCarregamento = ObterCentroCarregamento(dados.CodigoCentroCarregamento),
                Descricao = dados.Descricao,
                Reboques = veiculo.IsTipoVeiculoTracao() ? veiculo.VeiculosVinculados?.ToList() : new List<Dominio.Entidades.Veiculo>() { veiculo },
                Tipo = ObterTipo(dados.CodigoTipo),
                TipoLancamento = dados.TipoLancamento,
                Tracao = veiculo.IsTipoVeiculoTracao() ? veiculo : null
            };

            Adicionar(ocorrenciaPatioAdicionar);
        }

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioAdicionar ocorrenciaPatioAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio repositorioOcorrenciaPatio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio(_unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio = new Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio()
            {
                CentroCarregamento = ocorrenciaPatioAdicionar.CentroCarregamento,
                DataGeracao = DateTime.Now,
                Descricao = ocorrenciaPatioAdicionar.Descricao,
                Situacao = SituacaoOcorrenciaPatio.Pendente,
                OcorrenciaPatioTipo = ocorrenciaPatioAdicionar.Tipo,
                TipoLancamento = ocorrenciaPatioAdicionar.TipoLancamento,
                Tracao = ocorrenciaPatioAdicionar.Tracao,
                Reboques = ocorrenciaPatioAdicionar.Reboques.ToList()
            };

            repositorioOcorrenciaPatio.Inserir(ocorrenciaPatio, _auditado);

            GerarManobraHigienizacao(ocorrenciaPatio);
        }

        public void Aprovar(int codigoOcorrenciaPatio)
        {
            Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio repositorioOcorrenciaPatio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio = repositorioOcorrenciaPatio.BuscarPorCodigo(codigoOcorrenciaPatio, auditavel: true) ?? throw new ServicoException("Ocorrência de pátio não encontrada");

            if (!IsSituacaoPermiteAprovar(ocorrenciaPatio))
                throw new ServicoException("A situação da ocorrência de pátio não permite aprovação");

            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = null;

            ocorrenciaPatio.Situacao = SituacaoOcorrenciaPatio.Aprovada;

            try
            {
                _unitOfWork.Start();

                repositorioOcorrenciaPatio.Atualizar(ocorrenciaPatio, _auditado);

                switch (ocorrenciaPatio.OcorrenciaPatioTipo.Tipo)
                {
                    case TipoOcorrenciaPatio.Checklist:
                        RealizarAcaoChecklist(ocorrenciaPatio);
                        filasCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.RemoverChecklist(ocorrenciaPatio);
                        break;

                    case TipoOcorrenciaPatio.Higienizacao:
                        RealizarAcaoHigienizacao(ocorrenciaPatio);
                        break;
                }

                _unitOfWork.CommitChanges();

                if (filasCarregamentoVeiculo?.Count > 0)
                    servicoFilaCarregamentoVeiculo.NotificarAlteracoes(filasCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void Excluir(int codigoOcorrenciaPatio)
        {
            Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio repositorioOcorrenciaPatio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio = repositorioOcorrenciaPatio.BuscarPorCodigo(codigoOcorrenciaPatio, auditavel: true) ?? throw new ServicoException("Ocorrência de pátio não encontrada");

            if (!IsPermiteExcluir(ocorrenciaPatio))
                throw new ServicoException("A situação da ocorrência de pátio não permite a exclusão");

            try
            {
                _unitOfWork.Start();

                repositorioOcorrenciaPatio.Deletar(ocorrenciaPatio, _auditado);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void Reprovar(int codigoOcorrenciaPatio)
        {
            Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio repositorioOcorrenciaPatio = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio = repositorioOcorrenciaPatio.BuscarPorCodigo(codigoOcorrenciaPatio, auditavel: true) ?? throw new ServicoException("Ocorrência de pátio não encontrada");

            if (!IsSituacaoPermiteReprovar(ocorrenciaPatio))
                throw new ServicoException("A situação da ocorrência de pátio não permite reprovação");

            ocorrenciaPatio.Situacao = SituacaoOcorrenciaPatio.Reprovada;

            repositorioOcorrenciaPatio.Atualizar(ocorrenciaPatio, _auditado);
        }

        #endregion

        #region Métodos Públicos de Consulta

        public Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ObterTipoOcorrenciaPorTipo(TipoOcorrenciaPatio tipo)
        {
            Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorioOcorrenciaPatioTipo = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo ocorrenciaPatioTipo = repositorioOcorrenciaPatioTipo.BuscarAtivoPorTipo(tipo);

            return ocorrenciaPatioTipo;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo> ObterTiposOcorrencia()
        {
            Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo repositorioOcorrenciaPatioTipo = new Repositorio.Embarcador.GestaoPatio.OcorrenciaPatioTipo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo> ocorrenciasPatioTipo = repositorioOcorrenciaPatioTipo.BuscarAtivos();

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo> ocorrenciasPatioTipoRetornar = (
                from ocorrenciaPatioTipo in ocorrenciasPatioTipo
                select new Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo()
                {
                    Codigo = ocorrenciaPatioTipo.Codigo,
                    Descricao = ocorrenciaPatioTipo.Descricao
                }
            ).ToList();

            return ocorrenciasPatioTipoRetornar;
        }

        #endregion
    }
}
