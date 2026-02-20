using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FimHigienizacao : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaLiberarAutomaticamente
    {
        #region Construtores

        public FimHigienizacao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public FimHigienizacao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.FimHigienizacao, cliente) { }

        #endregion

        #region Métodos Privados

        private void AtualizarVeiculosParaHigienizado(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            Higienizacao servicoHigienizacao = new Higienizacao(_unitOfWork);

            servicoHigienizacao.AtualizarVeiculosParaHigienizado(cargaBase);
        }

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao)
        {
            if (fimHigienizacao == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!fimHigienizacao.EtapaFimHigienizacaoLiberada)
                throw new ServicoException("A liberação do fim da higienização ainda não foi autorizada.");

            if (!IsVeiculosInformados(fluxoGestaoPatio.CargaBase))
                throw new ServicoException("Não é possível realizar a liberação do fim da higienização sem veículos.");

            AvancarEtapa(fimHigienizacao, fluxoGestaoPatio);
        }

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fimHigienizacao.DataHigienizacaoFinalizada = DateTime.Now;
            fimHigienizacao.Situacao = SituacaoFimHigienizacao.HigienizacaoFinalizada;
            
            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork).Atualizar(fimHigienizacao, _auditado);

            AtualizarVeiculosParaHigienizado(fluxoGestaoPatio.CargaBase);
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
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (fimHigienizacao != null)
                return;
            
            fimHigienizacao = new Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaFimHigienizacaoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoFimHigienizacao.AguardandoFimHigienizacao
            };

            repositorioFimHigienizacao.Inserir(fimHigienizacao);
        }

        public void Avancar(int codigo)
        {
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorCodigo(codigo, auditavel: true);

            AvancarEtapa(fimHigienizacao?.FluxoGestaoPatio, fimHigienizacao);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimHigienizacao != null)
            {
                fimHigienizacao.Carga = carga;
                repositorioFimHigienizacao.Atualizar(fimHigienizacao);
            }
        }

        public void LiberarProximaEtapaAutomaticamente(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimHigienizacao == null)
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (configuracaoGestaoPatio.FimHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados && IsVeiculosInformados(fimHigienizacao.FluxoGestaoPatio.CargaBase) && IsVeiculosHigienizados(fimHigienizacao.FluxoGestaoPatio.CargaBase))
            {
                fimHigienizacao.Initialize();
                AvancarEtapa(fimHigienizacao, fimHigienizacao.FluxoGestaoPatio);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimHigienizacao != null)
            {
                fimHigienizacao.Carga = cargaNova;
                repositorioFimHigienizacao.Atualizar(fimHigienizacao);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataFimHigienizacaoPrevista.HasValue)
                fluxoGestaoPatio.DataFimHigienizacaoPrevista = preSetTempoEtapa.DataFimHigienizacaoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, fimHigienizacao);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataFimHigienizacaoPrevista = preSetTempoEtapa.DataFimHigienizacaoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataFimHigienizacao.HasValue)
                return false;
            
            fluxoGestaoPatio.TempoAguardandoFimHigienizacao = tempoEtapaAnterior;
            fluxoGestaoPatio.DataFimHigienizacao = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimHigienizacao != null)
            {
                fimHigienizacao.EtapaFimHigienizacaoLiberada = true;

                repositorioFimHigienizacao.Atualizar(fimHigienizacao);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimHigienizacao;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimHigienizacaoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (fimHigienizacao != null)
            {
                fimHigienizacao.EtapaFimHigienizacaoLiberada = true;

                repositorioFimHigienizacao.Atualizar(fimHigienizacao);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoFimHigienizacao = 0;
            fluxoGestaoPatio.DataFimHigienizacao = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataFimHigienizacaoPrevista.HasValue)
                fluxoGestaoPatio.DataFimHigienizacaoReprogramada = fluxoGestaoPatio.DataFimHigienizacaoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
