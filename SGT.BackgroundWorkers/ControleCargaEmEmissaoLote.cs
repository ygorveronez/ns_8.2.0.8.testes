using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class ControleCargaEmEmissaoLote : LongRunningProcessBase<ControleCargaEmEmissaoLote>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SolicitarEmissaoCargasEmEmissao(unitOfWork);
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        private void SolicitarEmissaoCargasEmEmissao(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);
                Servicos.Global.OrquestradorFila orquestrador = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.SolicitarEmissaoCargasEmEmissao);

                List<int> cargas = orquestrador.Ordenar((limiteRegistros) => repositorioCarga.BuscarCodigosCargasEmEmissao(maximoRegistros: limiteRegistros, controlePorLote: true));

                int codigoCarga = 0;

                for (int i = 0; i < cargas.Count; i++)
                {
                    try
                    {
                        unitOfWork.Start();
                        codigoCarga = cargas[i];
                        servicoCargaCTe.EmitirCTes(codigoCarga);
                        unitOfWork.CommitChanges();
                        orquestrador.RegistroLiberadoComSucesso(codigoCarga);

                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        orquestrador.RegistroComFalha(codigoCarga, ex.Message);
                        Servicos.Log.TratarErro(ex);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion Métodos Privados
    }
}
