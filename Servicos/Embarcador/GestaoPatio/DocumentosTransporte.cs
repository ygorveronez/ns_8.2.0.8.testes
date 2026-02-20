using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class DocumentosTransporte : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaLiberarAutomaticamente
    {
        #region Construtores

        public DocumentosTransporte(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public DocumentosTransporte(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.DocumentosTransporte, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentosTransporteAvancar documentosTransporteAvancar)
        {
            if (documentosTransporte == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!documentosTransporte.EtapaDocumentosTransporteLiberada)
                throw new ServicoException("A liberação dos documentos de transporte ainda não foi autorizada.");

            documentosTransporte.Initialize();
            documentosTransporte.DataDocumentosTransporteInformado = DateTime.Now;

            if (documentosTransporteAvancar != null)
            {
                documentosTransporte.NumeroCTe = documentosTransporteAvancar.NumeroCTe;
                documentosTransporte.NumeroMDFe = documentosTransporteAvancar.NumeroMDFe;
                documentosTransporte.Brix = documentosTransporteAvancar.Brix;
                documentosTransporte.Ratio = documentosTransporteAvancar.Ratio;
                documentosTransporte.Oleo = documentosTransporteAvancar.Oleo;
            }

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork).Atualizar(documentosTransporte, _auditado);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (documentosTransporte != null)
                return;

            documentosTransporte = new Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaDocumentosTransporteLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada
            };

            repositorioDocumentosTransporte.Inserir(documentosTransporte);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentosTransporteAvancar documentosTransporteAvancar)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorCodigo(documentosTransporteAvancar.Codigo);

            AvancarEtapa(documentosTransporte?.FluxoGestaoPatio, documentosTransporte, documentosTransporteAvancar);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentosTransporte != null)
            {
                documentosTransporte.Carga = carga;
                repositorioDocumentosTransporte.Atualizar(documentosTransporte);
            }
        }

        public void LiberarProximaEtapaAutomaticamente(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio.Carga == null) || fluxoGestaoPatio.Carga.SituacaoCarga.IsSituacaoCargaNaoFaturada())
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (configuracaoGestaoPatio.DocumentosTransportePermiteAvancarAutomaticamenteAposGerarDocumentos)
                LiberarProximaEtapa(fluxoGestaoPatio);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentosTransporte != null)
            {
                documentosTransporte.Carga = cargaNova;
                repositorioDocumentosTransporte.Atualizar(documentosTransporte);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataDocumentosTransportePrevista.HasValue)
                fluxoGestaoPatio.DataDocumentosTransportePrevista = preSetTempoEtapa.DataDocumentosTransportePrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, documentosTransporte, documentosTransporteAvancar: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataDocumentosTransportePrevista = preSetTempoEtapa.DataDocumentosTransportePrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataDocumentosTransporte.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoDocumentosTransporte = tempoEtapaAnterior;
            fluxoGestaoPatio.DataDocumentosTransporte = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentosTransporte != null)
            {
                documentosTransporte.EtapaDocumentosTransporteLiberada = true;
                repositorioDocumentosTransporte.Atualizar(documentosTransporte);
            }

            if (fluxoGestaoPatio.CargaBase.IsCarga())
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = (Dominio.Entidades.Embarcador.Cargas.Carga)fluxoGestaoPatio.CargaBase;

                carga.EtapaFaturamentoLiberado = true;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(carga);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataDocumentosTransporte;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataDocumentosTransportePrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentosTransporte != null)
            {
                documentosTransporte.EtapaDocumentosTransporteLiberada = false;
                repositorioDocumentosTransporte.Atualizar(documentosTransporte);
            }

            if (fluxoGestaoPatio.CargaBase.IsCarga())
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = (Dominio.Entidades.Embarcador.Cargas.Carga)fluxoGestaoPatio.CargaBase;

                carga.EtapaFaturamentoLiberado = false;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWork).Atualizar(carga);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoDocumentosTransporte = 0;
            fluxoGestaoPatio.DataDocumentosTransporte = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataDocumentosTransportePrevista.HasValue)
                fluxoGestaoPatio.DataDocumentosTransporteReprogramada = fluxoGestaoPatio.DataDocumentosTransportePrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
