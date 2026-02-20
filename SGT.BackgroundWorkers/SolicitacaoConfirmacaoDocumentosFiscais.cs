using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class SolicitacaoConfirmacaoDocumentosFiscais : LongRunningProcessBase<SolicitacaoConfirmacaoDocumentosFiscais>
    {

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            
            await SolicitarConfirmacaoDocumentosFiscaisAsync(unitOfWork, cancellationToken);
        }

        #endregion

        #region Metodos Privados

        private async Task SolicitarConfirmacaoDocumentosFiscaisAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            try
            {
                // Obter configurações necessárias
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repConfiguracaoTMS.BuscarPrimeiroRegistroAsync();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.SolicitacaoConfirmacaoDocumentosFiscais);

                List<int> listaCodigosCargas = servicoOrquestradorFila.Ordenar((limiteRegistros) => repCarga.BuscarCodigosCargasSolicitarConfirmacaoDocumentosFiscais(limiteRegistros));

                for (int i = 0; i < listaCodigosCargas.Count; i++)
                {
                    
                    int codigoCarga = listaCodigosCargas[i];
                    
                    try
                    {
                        await unitOfWork.StartAsync();
                        // Buscar CargaPedidos para esta carga
                        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repositorioCargaPedido.BuscarPorCargaAsync(codigoCarga);

                        if (cargaPedidos == null || cargaPedidos.Count == 0)
                        {
                            await repCarga.AtualizarDataSolicitacaoConfirmacaoDocumentosFiscaisPorCodigoCargaAsync(codigoCarga, null, cancellationToken);
                            await unitOfWork.CommitChangesAsync();
                            continue;
                        }                        
                        
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimoCargaPedido = cargaPedidos[cargaPedidos.Count - 1];

                        Servicos.WebService.NFe.NotaFiscal.SolicitarConfirmacaoDocumentosFiscais(
                            ultimoCargaPedido,
                            cargaPedidos,
                            configuracao,
                            _tipoServicoMultisoftware,
                            _auditado,
                            unitOfWork
                        );
                        
                        await unitOfWork.CommitChangesAsync();
                        
                        servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoCarga);
                        
                        Log.TratarErro($"Solicitada confirmação de documentos fiscais por servidor de integração. Carga: { ultimoCargaPedido.Carga.Codigo}", "ConfirmarEnvioDosDocumentos");                        
                        
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        await unitOfWork.RollbackAsync();

                        await repCarga.AtualizarDataSolicitacaoConfirmacaoDocumentosFiscaisPorCodigoCargaAsync(codigoCarga, DateTime.Now.AddSeconds(30), cancellationToken);
                        servicoOrquestradorFila.RegistroComFalha(codigoCarga, ex.Message);
                        
                    }

                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion
    }
}
