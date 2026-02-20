using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class ControleCarregamento
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ControleCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void AtualizarVeiculosParaNaoHigienizados(Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento)
        {
            GestaoPatio.Higienizacao servicoHigienizacao = new GestaoPatio.Higienizacao(_unitOfWork);

            servicoHigienizacao.AtualizarVeiculosParaNaoHigienizado(controleCarregamento.JanelaCarregamento.CargaBase);
        }

        private void GerarManobraCarregamentoFinalizado(Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento)
        {
            if (!controleCarregamento.JanelaCarregamento.CentroCarregamento.UtilizarControleManobra)
                return;

            if (!IsGerarManobraCarregamentoFinalizado(controleCarregamento))
                return;

            Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar manobraAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.ManobraAdicionar()
            {
                CentroCarregamento = controleCarregamento.JanelaCarregamento.CentroCarregamento,
                Acao = controleCarregamento.JanelaCarregamento.CentroCarregamento.AcaoManobraPadraoFimCarregamento,
                Reboques = controleCarregamento.JanelaCarregamento.CargaBase.VeiculosVinculados?.ToList(),
                Tracao = controleCarregamento.JanelaCarregamento.CargaBase.Veiculo
            };

            new Manobra(_unitOfWork).AdicionarManobra(manobraAdicionar);
        }

        private bool IsGerarManobraCarregamentoFinalizado(Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento)
        {
            return (
                controleCarregamento.JanelaCarregamento.CentroCarregamento != null &&
                controleCarregamento.JanelaCarregamento.CentroCarregamento.UtilizarControleManobra &&
                controleCarregamento.JanelaCarregamento.CentroCarregamento.AcaoManobraPadraoFimCarregamento != null
            );
        }

        private bool IsSituacaoPermiteChegadaDoca(Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento)
        {
            return (controleCarregamento.Situacao == SituacaoControleCarregamento.Aguardando);
        }

        private bool IsSituacaoPermiteFinalizar(Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento)
        {
            return (controleCarregamento.Situacao == SituacaoControleCarregamento.EmCarregamento);
        }

        private bool IsSituacaoPermiteIniciar(Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento)
        {
            return (controleCarregamento.Situacao == SituacaoControleCarregamento.EmDoca);
        }

        private bool IsVeiculosHigienizados(Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento)
        {
            GestaoPatio.Higienizacao servicoHigienizacao = new GestaoPatio.Higienizacao(_unitOfWork);

            return servicoHigienizacao.IsVeiculosHigienizados(controleCarregamento.JanelaCarregamento.CargaBase);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento> ObterTodos(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento filtrosPesquisa)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "asc",
                PropriedadeOrdenar = "DataInicio"
            };

            Repositorio.Embarcador.Logistica.ControleCarregamento repositorioControleCarregamento = new Repositorio.Embarcador.Logistica.ControleCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento> listaControleCarregamento = repositorioControleCarregamento.Consultar(filtrosPesquisa, parametrosConsulta);

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento> listaControleCarregamentoRetornar = (
                from controleCarregamento in listaControleCarregamento
                select new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento()
                {
                    Codigo = controleCarregamento.Codigo,
                    CodigoCentroCarregamento = controleCarregamento.JanelaCarregamento.CentroCarregamento?.Codigo ?? 0,
                    CodigoVeiculo = controleCarregamento.JanelaCarregamento.CargaBase?.Veiculo?.Codigo ?? 0,
                    DataCriacao = controleCarregamento.DataCriacao.ToString("dd/MM/yy HH:mm"),
                    DataInicioCarregamento = controleCarregamento.DataInicio.HasValue ? controleCarregamento.DataInicio.Value.ToString("dd/MM/yy HH:mm") : "",
                    Local = controleCarregamento.JanelaCarregamento.PreCarga?.LocalCarregamento?.DescricaoAcao ?? "",
                    Placa = controleCarregamento.JanelaCarregamento.CargaBase?.RetornarPlacas ?? "",
                    Transportador = controleCarregamento.JanelaCarregamento.CargaBase?.Empresa?.Descricao ?? ""
                }
            ).ToList();

            return listaControleCarregamentoRetornar;
        }

        #endregion

        #region Métodos Públicos

        public void Criar(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento)
        {
            Repositorio.Embarcador.Logistica.ControleCarregamento repositorioControleCarregamento = new Repositorio.Embarcador.Logistica.ControleCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento = repositorioControleCarregamento.BuscarPorJanelaCarregamento(janelaCarregamento.Codigo);

            if (controleCarregamento == null)
            {
                controleCarregamento = new Dominio.Entidades.Embarcador.Logistica.ControleCarregamento()
                {
                    DataCriacao = DateTime.Now,
                    JanelaCarregamento = janelaCarregamento,
                    Situacao = SituacaoControleCarregamento.Aguardando
                };

                repositorioControleCarregamento.Inserir(controleCarregamento);
            }
        }

        public void ChegadaDoca(int codigoControleCarregamento)
        {
            Repositorio.Embarcador.Logistica.ControleCarregamento repositorioControleCarregamento = new Repositorio.Embarcador.Logistica.ControleCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento = repositorioControleCarregamento.BuscarPorCodigo(codigoControleCarregamento) ?? throw new ServicoException("Controle de carregamento não encontrado");

            if (!IsSituacaoPermiteChegadaDoca(controleCarregamento))
                throw new ServicoException("A situação não permite informar a chegada na doca");

            controleCarregamento.Situacao = SituacaoControleCarregamento.EmDoca;

            try
            {
                _unitOfWork.Start();

                GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(controleCarregamento.JanelaCarregamento.CargaBase);

                repositorioControleCarregamento.Atualizar(controleCarregamento);
                servicoFluxoGestaoPatio.AvancarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.InformarDoca);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void ChegadaDocaPorVeiculo(int codigoVeiculo)
        {
            Repositorio.Embarcador.Logistica.ControleCarregamento repositorioControleCarregamento = new Repositorio.Embarcador.Logistica.ControleCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento = repositorioControleCarregamento.BuscarAguardandoPorVeiculo(codigoVeiculo);

            if (controleCarregamento != null)
            {
                GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(controleCarregamento.JanelaCarregamento.CargaBase);

                controleCarregamento.Situacao = SituacaoControleCarregamento.EmDoca;

                repositorioControleCarregamento.Atualizar(controleCarregamento);
                servicoFluxoGestaoPatio.AvancarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.InformarDoca);
            }
        }

        public void Finalizar(int codigoControleCarregamento)
        {
            Repositorio.Embarcador.Logistica.ControleCarregamento repositorioControleCarregamento = new Repositorio.Embarcador.Logistica.ControleCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento = repositorioControleCarregamento.BuscarPorCodigo(codigoControleCarregamento) ?? throw new ServicoException("Controle de carregamento não encontrado");

            if (!IsSituacaoPermiteFinalizar(controleCarregamento))
                throw new ServicoException("A situação não permite finalizar o carregamento");

            controleCarregamento.DataFinalizacao = DateTime.Now;
            controleCarregamento.Situacao = SituacaoControleCarregamento.Finalizado;

            try
            {
                _unitOfWork.Start();

                GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(controleCarregamento.JanelaCarregamento.CargaBase);

                repositorioControleCarregamento.Atualizar(controleCarregamento);
                AtualizarVeiculosParaNaoHigienizados(controleCarregamento);
                servicoFluxoGestaoPatio.AvancarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.FimCarregamento);

                _unitOfWork.CommitChanges();

                GerarManobraCarregamentoFinalizado(controleCarregamento);
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void Iniciar(int codigoControleCarregamento)
        {
            Repositorio.Embarcador.Logistica.ControleCarregamento repositorioControleCarregamento = new Repositorio.Embarcador.Logistica.ControleCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ControleCarregamento controleCarregamento = repositorioControleCarregamento.BuscarPorCodigo(codigoControleCarregamento) ?? throw new ServicoException("Controle de carregamento não encontrado");

            if (!IsSituacaoPermiteIniciar(controleCarregamento))
                throw new ServicoException("A situação não permite iniciar o carregamento");

            if (!IsVeiculosHigienizados(controleCarregamento))
                throw new ServicoException("Não é possível iniciar o carregamento com veículo não higienizado");

            controleCarregamento.DataInicio = DateTime.Now;
            controleCarregamento.Situacao = SituacaoControleCarregamento.EmCarregamento;

            try
            {
                _unitOfWork.Start();

                GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(controleCarregamento.JanelaCarregamento.CargaBase);

                repositorioControleCarregamento.Atualizar(controleCarregamento);
                servicoFluxoGestaoPatio.AvancarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioCarregamento);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        #endregion

        #region Métodos Públicos de Consulta

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento> ObterTodosEmCarregamento()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento()
            {
                Situacoes = new List<SituacaoControleCarregamento>() { SituacaoControleCarregamento.EmCarregamento }
            };

            return ObterTodos(filtrosPesquisa);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento> ObterTodosEmDoca()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento()
            {
                Situacoes = new List<SituacaoControleCarregamento>() { SituacaoControleCarregamento.EmDoca }
            };

            return ObterTodos(filtrosPesquisa);
        }

        #endregion
    }
}
