using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]
    public class EmissaoDocumentos : LongRunningProcessBase<EmissaoDocumentos>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCargasPendentesEmissao(unitOfWork, _codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _webServiceConsultaCTe, _auditado);
            SolicitarEmissaoCTesComplementaresRejeitados(unitOfWork);
            SolicitarReemissaoCTesComplementaresRejeitados(unitOfWork);
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        private void VerificarCargasPendentesEmissao(Repositorio.UnitOfWork unitOfWork, int idEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.Documentos servicoCargaDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.VerificarCargasPendentesEmissao);
            List<int> cargasPendeciaEmissao = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCarga.BuscarCargasPendentesEmissao(limiteRegistros, controlePorLote: false));

            for (var i = 0; i < cargasPendeciaEmissao.Count; i++)
            {
                int codigoCarga = cargasPendeciaEmissao[i];

                try
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                    servicoCargaDocumentos.VerificarPendeciasEmissaoDocumentosCarga(carga, idEmpresa, tipoServicoMultisoftware, webServiceConsultaCTe, Auditado, unitOfWork);
                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoCarga);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    servicoOrquestradorFila.RegistroComFalha(codigoCarga, excecao.Message);
                }
            }
        }

        private void SolicitarEmissaoCTesComplementaresRejeitados(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);

            servicoCargaCTe.EmitirCTesComplementaresRejeitados();
        }

        private void SolicitarReemissaoCTesComplementaresRejeitados(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);

            servicoCargaCTe.ReemitirCTesComplementaresRejeitados();
        }

        #endregion Métodos Privados
    }
}