using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class Expedicao : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public Expedicao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null, cliente: null) { }

        public Expedicao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public Expedicao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.Expedicao, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao)
        {
            if (cargaControleExpedicao == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(cargaControleExpedicao.FluxoGestaoPatio);

            if (sequenciaGestaoPatio?.ExpedicaoConfirmarPlaca ?? true)
                throw new ServicoException("Liberação da informação do término do carregamento não permitida.");

            if (!(sequenciaGestaoPatio?.GuaritaEntradaPermiteInformacoesPesagem ?? false))
            {
                if (!cargaControleExpedicao.EtapaExpedicaoLiberada && cargaControleExpedicao.FluxoGestaoPatio != null)
                    throw new ServicoException("A liberação da informação do término do carregamento ainda não foi autorizada.");

                cargaControleExpedicao.EtapaExpedicaoLiberada = true;
                cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.Liberada;
                cargaControleExpedicao.DataConfirmacao = DateTime.Now;

                if (!cargaControleExpedicao.DataInicioCarregamento.HasValue)
                    cargaControleExpedicao.DataInicioCarregamento = cargaControleExpedicao.DataConfirmacao;

                new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork).Atualizar(cargaControleExpedicao);
            }
            if (fluxoGestaoPatio != null)
                LiberarProximaEtapa(fluxoGestaoPatio);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (cargaControleExpedicao == null)
            {
                if (fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga != null)
                    cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCarga(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga.Codigo);
                else
                    cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorPreCarga(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga.Codigo);
            }

            if (cargaControleExpedicao == null)
            {
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio);

                cargaControleExpedicao = new Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao()
                {
                    FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                    Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                    PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                    Placa = "",
                    Doca = "",
                    EtapaExpedicaoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada
                };

                if (sequenciaGestaoPatio?.ExpedicaoInformarInicioCarregamento ?? false)
                    cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.AgInicioCarregamento;
                else if (sequenciaGestaoPatio?.ExpedicaoInformarTerminoCarregamento ?? false)
                    cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.AguardandoLiberacao;
                else
                    cargaControleExpedicao.SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.Liberada;

                cargaControleExpedicao.DataConfirmacao = DateTime.Now;

                repositorioCargaControleExpedicao.Inserir(cargaControleExpedicao);
            }
            else
            {
                cargaControleExpedicao.EtapaExpedicaoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada;
                cargaControleExpedicao.FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio;

                repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);
            }
        }

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCarga(carga.Codigo);

            if (cargaControleExpedicao != null)
                return;

            cargaControleExpedicao = new Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao()
            {
                Carga = carga,
                DataConfirmacao = DateTime.Now,
                Placa = "",
                Doca = "",
                SituacaoCargaControleExpedicao = SituacaoCargaControleExpedicao.AguardandoLiberacao
            };

            repositorioCargaControleExpedicao.Inserir(cargaControleExpedicao);
        }

        public void Avancar(int codigo)
        {
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorCodigo(codigo);

            Avancar(cargaControleExpedicao);
        }

        public void Avancar(Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao)
        {
            AvancarEtapa(cargaControleExpedicao?.FluxoGestaoPatio, cargaControleExpedicao);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (cargaControleExpedicao != null)
            {
                cargaControleExpedicao.Carga = carga;
                repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (cargaControleExpedicao != null)
            {
                cargaControleExpedicao.Carga = cargaNova;
                repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataInicioCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioCarregamentoPrevista = preSetTempoEtapa.DataInicioCarregamentoPrevista.Value;

            if (preSetTempoEtapa.DataFimCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFimCarregamentoPrevista = preSetTempoEtapa.DataFimCarregamentoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, cargaControleExpedicao);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataInicioCarregamentoPrevista = preSetTempoEtapa.DataInicioCarregamentoPrevista ?? dataPrevista;
            fluxoGestaoPatio.DataFimCarregamentoPrevista = preSetTempoEtapa.DataFimCarregamentoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataInicioCarregamento.HasValue && fluxoGestaoPatio.DataFimCarregamento.HasValue)
                return false;

            if (!fluxoGestaoPatio.DataInicioCarregamento.HasValue)
            {
                fluxoGestaoPatio.TempoAgInicioCarregamento = tempoEtapaAnterior;
                fluxoGestaoPatio.DataInicioCarregamento = data;
            }

            if (!fluxoGestaoPatio.DataFimCarregamento.HasValue)
            {
                fluxoGestaoPatio.TempoAgFimCarregamento = tempoEtapaAnterior;
                fluxoGestaoPatio.DataFimCarregamento = data;
            }

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (cargaControleExpedicao != null)
            {
                cargaControleExpedicao.EtapaExpedicaoLiberada = true;
                repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return (fluxoGestaoPatio.DataInicioCarregamento.HasValue && !fluxoGestaoPatio.DataFimCarregamento.HasValue) ? fluxoGestaoPatio.DataInicioCarregamento : fluxoGestaoPatio.DataFimCarregamento;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimCarregamentoPrevista ?? fluxoGestaoPatio.DataInicioCarregamentoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repositorioCargaControleExpedicao = new Repositorio.Embarcador.Cargas.CargaControleExpedicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao cargaControleExpedicao = repositorioCargaControleExpedicao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (cargaControleExpedicao != null)
            {
                cargaControleExpedicao.EtapaExpedicaoLiberada = false;
                repositorioCargaControleExpedicao.Atualizar(cargaControleExpedicao);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgInicioCarregamento = 0;
            fluxoGestaoPatio.DataInicioCarregamento = null;

            fluxoGestaoPatio.TempoAgFimCarregamento = 0;
            fluxoGestaoPatio.DataFimCarregamento = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataInicioCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioCarregamentoReprogramada = fluxoGestaoPatio.DataInicioCarregamentoPrevista.Value.Add(tempoReprogramar);

            if (fluxoGestaoPatio.DataFimCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFimCarregamentoReprogramada = fluxoGestaoPatio.DataFimCarregamentoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
