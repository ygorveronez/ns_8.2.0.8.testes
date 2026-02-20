using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 30000)]

    public class EncerramentoCarga : LongRunningProcessBase<EncerramentoCarga>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Log.GravarInfo("Inicio VerificarCargasEncerramentoComDocumentosPendentesEncerramento", "EncerramentoCarga");
            VerificarCargasEncerramentoComDocumentosPendentesEncerramento(unitOfWork);
            Servicos.Log.GravarInfo("Fim VerificarCargasEncerramentoComDocumentosPendentesEncerramento.  Inicio VerificarCargasEncerramentoAgEncerramentoMDFe", "EncerramentoCarga");
            VerificarCargasEncerramentoAgEncerramentoMDFe(unitOfWork);
            Servicos.Log.GravarInfo("Fim VerificarCargasEncerramentoAgEncerramentoMDFe.  Inicio VerificarCargasEncerramentoAgIntegracoes", "EncerramentoCarga");
            VerificarCargasEncerramentoAgIntegracoes(unitOfWork);
            Servicos.Log.GravarInfo("Fim VerificarCargasEncerramentoAgIntegracoes.  Inicio IniciarIntegracoesComCarga", "EncerramentoCarga");
            IniciarIntegracoesComCarga(unitOfWork);
            Servicos.Log.GravarInfo("Fim IniciarIntegracoesComCarga.  Inicio VerificarIntegracoesCargaPendentes", "EncerramentoCarga");
            VerificarIntegracoesCargaPendentes(unitOfWork);
            Servicos.Log.GravarInfo("Fim VerificarIntegracoesCargaPendentes.  Inicio VerificarCargasEncerramentoIntegradas", "EncerramentoCarga");
            VerificarCargasEncerramentoIntegradas(unitOfWork);
            Servicos.Log.GravarInfo("Fim VerificarCargasEncerramentoIntegradas.", "EncerramentoCarga");
        }

        public void VerificarCargasEncerramentoComDocumentosPendentesEncerramento(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaEncerramento serCargaEncerramento = new Servicos.Embarcador.Carga.CargaEncerramento();
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);

            List<int> codigosCargasEncerramento = repCargaRegistroEncerramento.BuscarCodigosCargasEncerramentoPendentesPorSituacao(SituacaoEncerramentoCarga.AgEncerramentoDocumentos);

            foreach (int codigoCargaEncerramento in codigosCargasEncerramento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCodigo(codigoCargaEncerramento);
                serCargaEncerramento.VerificarEncerramentoCIOT(cargaRegistroEncerramento, _tipoServicoMultisoftware, unitOfWork);
            }
        }

        public void VerificarCargasEncerramentoAgEncerramentoMDFe(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaEncerramento serCargaEncerramento = new Servicos.Embarcador.Carga.CargaEncerramento();
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);

            List<int> codigosCargasEncerramento = repCargaRegistroEncerramento.BuscarCodigosCargasEncerramentoPendentesPorSituacao(SituacaoEncerramentoCarga.AgEncerramentoMDFe);

            foreach (int codigoCargaEncerramento in codigosCargasEncerramento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCodigo(codigoCargaEncerramento);
                serCargaEncerramento.VerificarEncerramentoMDFe(cargaRegistroEncerramento, _tipoServicoMultisoftware, _auditado, _webServiceConsultaCTe, unitOfWork);
            }
        }

        public void VerificarCargasEncerramentoAgIntegracoes(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaEncerramento serCargaEncerramento = new Servicos.Embarcador.Carga.CargaEncerramento();
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);

            List<int> codigosCargasEncerramento = repCargaRegistroEncerramento.BuscarCodigosCargasEncerramentoIntegracoesPendentes();

            foreach (int codigoCargaEncerramento in codigosCargasEncerramento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCodigo(codigoCargaEncerramento);

                serCargaEncerramento.VerificarIntegracoesCargaEncerramento(cargaRegistroEncerramento, unitOfWork);
            }
        }

        public void IniciarIntegracoesComCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaEncerramento serCargaEncerramento = new Servicos.Embarcador.Carga.CargaEncerramento();
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);

            List<int> codigosCargasEncerramento = repCargaRegistroEncerramento.BuscarCodigosCargasEncerramentoPossuiIntegracaoPendente();

            foreach (int codigoCargaEncerramento in codigosCargasEncerramento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCodigo(codigoCargaEncerramento);
                serCargaEncerramento.IniciarIntegracoesCargaEncerramentoCarga(cargaRegistroEncerramento, unitOfWork);
            }
        }

        public void VerificarIntegracoesCargaPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaEncerramento serCargaEncerramento = new Servicos.Embarcador.Carga.CargaEncerramento();
            serCargaEncerramento.VerificarIntegracoesCargaPendentes(unitOfWork);
        }

        public void VerificarCargasEncerramentoIntegradas(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaEncerramento serCargaEncerramento = new Servicos.Embarcador.Carga.CargaEncerramento(_auditado, _tipoServicoMultisoftware);

            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);

            List<int> codigosCargasEncerramento = repCargaRegistroEncerramento.BuscarCodigosCargasEncerramentoPendentesPorSituacao(SituacaoEncerramentoCarga.AgIntegracao);

            foreach (int codigoCargaEncerramento in codigosCargasEncerramento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCodigo(codigoCargaEncerramento);

                if (cargaRegistroEncerramento.PossuiIntegracao && !repCargaCargaIntegracao.PossuiIntegracoesPendentes(codigoCargaEncerramento))
                    serCargaEncerramento.FinalizarCargaEncerramento(codigoCargaEncerramento, unitOfWork);
                else if (cargaRegistroEncerramento.EncerrarSemIntegracao)
                    serCargaEncerramento.FinalizarCargaEncerramento(codigoCargaEncerramento, unitOfWork);
            }
        }
    }
}