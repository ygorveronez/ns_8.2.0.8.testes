using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 7000)]

    public class ControleCargaEmFinalizacaoEmissao : LongRunningProcessBase<ControleCargaEmFinalizacaoEmissao>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Servicos.Embarcador.Carga.CargaDistribuidor servicoCargaDistribuidor = new Servicos.Embarcador.Carga.CargaDistribuidor(unitOfWork, _tipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            SolicitarFinalizacaoCargas(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            servicoCargaDistribuidor.GerarCargaProximoTrechoIndividualPorPedido();
            servicoCargaDistribuidor.GerarCargaProximoTrechoAposEmissao();


            if (configuracaoTMS.GerarDACTEOutrosDocumentosAutomaticamente)
                SolicitarGeracaoDACTEOutrosDocumentos(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }

        private void SolicitarFinalizacaoCargas(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork);

                List<int> codigosCargas = repCarga.BuscarCodigosCargasEmFinalizacao(10, false);

                foreach (int codigoCarga in codigosCargas)
                {
                    Servicos.Log.TratarErro($"Iniciou - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    svcDocumentos.FinalizarCargaEmFinalizacao(codigoCarga, tipoServicoMultisoftware, unitOfWork, _auditado);
                    Servicos.Log.TratarErro($"Terminou - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void SolicitarGeracaoDACTEOutrosDocumentos(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Servicos.DACTE svcDACTE = new Servicos.DACTE(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<int> codigosCTes = repCTe.BuscarOutrosDocumentosParaGeracaoDACTE(10);

                foreach (int codigoCTe in codigosCTes)
                {
                    if (svcDACTE.GerarPorProcesso(codigoCTe, unitOfWork) != null)
                        repCTe.SetarDACTEGerado(codigoCTe);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }


    }
}