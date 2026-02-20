using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class InicioHigienizacao : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaLiberarAutomaticamente
    {
        #region Construtores

        public InicioHigienizacao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public InicioHigienizacao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.InicioHigienizacao, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao)
        {
            if (inicioHigienizacao == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!inicioHigienizacao.EtapaInicioHigienizacaoLiberada)
                throw new ServicoException("A liberação do início da higienização ainda não foi autorizada.");

            if (!IsVeiculosInformados(fluxoGestaoPatio.CargaBase))
                throw new ServicoException("Não é possível realizar a liberação do início da higienização sem veículos.");

            inicioHigienizacao.Initialize();

            AvancarEtapa(inicioHigienizacao, fluxoGestaoPatio);
        }

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            inicioHigienizacao.DataHigienizacaoIniciada = DateTime.Now;
            inicioHigienizacao.Situacao = SituacaoInicioHigienizacao.HigienizacaoIniciada;

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork).Atualizar(inicioHigienizacao, _auditado);
        }

        private bool IsVeiculosHigienizados(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            Higienizacao servicoHigienizacao = new Higienizacao(_unitOfWork);

            return servicoHigienizacao.IsVeiculosHigienizados(cargaBase);
        }

        private bool IsVeiculosInformados(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            if (cargaBase.ModeloVeicularCarga?.NumeroReboques > 0)
                return cargaBase.VeiculosVinculados?.Count > 0;

            return cargaBase.Veiculo != null;
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioInicioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioInicioHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (inicioHigienizacao != null)
                return;

            inicioHigienizacao = new Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaInicioHigienizacaoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoInicioHigienizacao.AguardandoInicioHigienizacao
            };

            repositorioInicioHigienizacao.Inserir(inicioHigienizacao);
        }

        public void Avancar(int codigo)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioInicioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioInicioHigienizacao.BuscarPorCodigo(codigo);

            AvancarEtapa(inicioHigienizacao?.FluxoGestaoPatio, inicioHigienizacao);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao higienizacao = repositorioHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (higienizacao != null)
            {
                higienizacao.Carga = carga;
                repositorioHigienizacao.Atualizar(higienizacao);
            }
        }

        public void LiberarProximaEtapaAutomaticamente(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioHigienizacao == null)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (configuracaoGestaoPatio.InicioHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados && IsVeiculosInformados(inicioHigienizacao.FluxoGestaoPatio.CargaBase) && IsVeiculosHigienizados(inicioHigienizacao.FluxoGestaoPatio.CargaBase))
            {
                inicioHigienizacao.Initialize();
                AvancarEtapa(inicioHigienizacao, inicioHigienizacao.FluxoGestaoPatio);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao higienizacao = repositorioHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (higienizacao != null)
            {
                higienizacao.Carga = cargaNova;
                repositorioHigienizacao.Atualizar(higienizacao);
            }
        }

        public byte[] GerarPdfViaCega(Dominio.Entidades.Embarcador.Cargas.Carga cargaViaCega)
        {
            return ReportRequest.WithType(ReportType.ViaCega)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", cargaViaCega.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataInicioHigienizacaoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioHigienizacaoPrevista = preSetTempoEtapa.DataInicioHigienizacaoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, inicioHigienizacao);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataInicioHigienizacaoPrevista = preSetTempoEtapa.DataInicioHigienizacaoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataInicioHigienizacao.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoInicioHigienizacao = tempoEtapaAnterior;
            fluxoGestaoPatio.DataInicioHigienizacao = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioHigienizacao != null)
            {
                inicioHigienizacao.EtapaInicioHigienizacaoLiberada = true;

                repositorioHigienizacao.Atualizar(inicioHigienizacao);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioHigienizacao;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioHigienizacaoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (inicioHigienizacao != null)
            {
                inicioHigienizacao.EtapaInicioHigienizacaoLiberada = true;

                repositorioHigienizacao.Atualizar(inicioHigienizacao);
            }

        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoInicioHigienizacao = 0;
            fluxoGestaoPatio.DataInicioHigienizacao = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataInicioHigienizacaoPrevista.HasValue)
                fluxoGestaoPatio.DataInicioHigienizacaoReprogramada = fluxoGestaoPatio.DataInicioHigienizacaoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
