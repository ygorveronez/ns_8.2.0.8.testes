using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FimCarregamento : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public FimCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public FimCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.FimCarregamento, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimCarregamentoAvancarEtapa fimCarregamentoAvancarEtapa)
        {
            if (fimCarregamento == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!fimCarregamento.EtapaFimCarregamentoLiberada)
                throw new ServicoException("A liberação do fim do carregamento ainda não foi autorizada.");

            if (ObterConfiguracaoGestaoPatio().FimCarregamentoPermiteAvancarSomenteDadosTransporteInformados && !fluxoGestaoPatio.CargaBase.IsDadosTransporteInformados())
                throw new ServicoException("Não é possivel finalizar o carregamento sem dados de transporte informados.");

            fimCarregamento.Initialize();
            fimCarregamento.DataCarregamentoFinalizado = DateTime.Now;
            fimCarregamento.Situacao = SituacaoFimCarregamento.CarregamentoFinalizado;

            if (fimCarregamentoAvancarEtapa != null)
                fimCarregamento.Pesagem = fimCarregamentoAvancarEtapa.Pesagem;

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork).Atualizar(fimCarregamento, _auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (fimCarregamento != null)
                return;

            fimCarregamento = new Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaFimCarregamentoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoFimCarregamento.AguardandoFimCarregamento
            };

            repositorioFimCarregamento.Inserir(fimCarregamento);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimCarregamentoAvancarEtapa fimCarregamentoAvancarEtapa)
        {
            Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorCodigo(fimCarregamentoAvancarEtapa.Codigo);

            AvancarEtapa(fimCarregamento?.FluxoGestaoPatio, fimCarregamento, fimCarregamentoAvancarEtapa);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimCarregamento != null)
            {
                fimCarregamento.Carga = carga;
                repositorioFimCarregamento.Atualizar(fimCarregamento);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimCarregamento != null)
            {
                fimCarregamento.Carga = cargaNova;
                repositorioFimCarregamento.Atualizar(fimCarregamento);
            }
        }

        public byte[] ObterPdfSinteseMateriais(Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento)
        {
            return ReportRequest.WithType(ReportType.SinteseMateriais)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoFimCarregamento", fimCarregamento.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataFimCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFimCarregamentoPrevista = preSetTempoEtapa.DataFimCarregamentoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, fimCarregamento, fimCarregamentoAvancarEtapa: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataFimCarregamentoPrevista = preSetTempoEtapa.DataFimCarregamentoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataFimCarregamento.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgFimCarregamento = tempoEtapaAnterior;
            fluxoGestaoPatio.DataFimCarregamento = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimCarregamento != null)
            {
                fimCarregamento.EtapaFimCarregamentoLiberada = true;
                repositorioFimCarregamento.Atualizar(fimCarregamento);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimCarregamento;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimCarregamentoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimCarregamento != null)
            {
                fimCarregamento.EtapaFimCarregamentoLiberada = false;
                repositorioFimCarregamento.Atualizar(fimCarregamento);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgFimCarregamento = 0;
            fluxoGestaoPatio.DataFimCarregamento = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataFimCarregamentoPrevista.HasValue)
                fluxoGestaoPatio.DataFimCarregamentoReprogramada = fluxoGestaoPatio.DataFimCarregamentoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
