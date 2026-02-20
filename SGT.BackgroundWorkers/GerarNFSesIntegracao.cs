using Dominio.Excecoes.Embarcador;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class GerarNFSesIntegracao : LongRunningProcessBase<GerarNFSesIntegracao>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarNFSesPendentes(unitOfWork);
        }


        private void VerificarNFSesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unitOfWork);

            List<int> listaIntegracaoNFSe = repIntegracaoNFSe.BuscarIntegracaoNFSeAguardandoGeracao(5);

            Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

            for (var i = 0; i < listaIntegracaoNFSe.Count; i++)
            {
                Dominio.Entidades.IntegracaoNFSe integracaoNFse = repIntegracaoNFSe.BuscarPorCodigo(listaIntegracaoNFSe[i]);

                if (integracaoNFse != null)
                {
                    if (integracaoNFse.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Pendente)
                    {
                        try
                        {
                            Dominio.ObjetosDeValor.CTe.CTeNFSe documento = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTeNFSe>(integracaoNFse.Arquivo);

                            unitOfWork.Start();

                            Dominio.Entidades.NFSe nfse = svcNFSe.GerarNFSePorObjetoObjetoCTe(documento, unitOfWork, Dominio.Enumeradores.StatusNFSe.Enviado, integracaoNFse.NFSe.Codigo);
                            svcNFSe.ObterRPS(ref nfse, unitOfWork);

                            unitOfWork.CommitChanges();

                            if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Enviado)
                            {
                                if (!svcNFSe.Emitir(nfse, unitOfWork))
                                    throw new ServicoException("NFSe" + nfse.Numero.ToString() + " da empresa " + nfse.Empresa.CNPJ + " foi salva, porem, ocorreu uma falha ao enviar-la a prefeitura.");

                                if (!svcNFSe.AdicionarNFSeNaFilaDeConsulta(nfse, unitOfWork))
                                    throw new ServicoException("NFSe " + nfse.Numero.ToString() + " da empresa " + nfse.Empresa.CNPJ + " foi salva, porem, nao foi possivel adiciona-la na fila de consulta.");

                                integracaoNFse.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                                repIntegracaoNFSe.Atualizar(integracaoNFse);
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao gerar NFSe codigo " + integracaoNFse.NFSe.Codigo + ": " + ex, "GerarNFSeIntegracao");
                            unitOfWork.Rollback();
                            throw;
                        }
                    }
                    else
                    {
                        integracaoNFse.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                        repIntegracaoNFSe.Atualizar(integracaoNFse);
                    }
                }
                else
                    Servicos.Log.TratarErro("Integração codigo " + listaIntegracaoNFSe[i].ToString() + " não localzada.", "GerarNFSeIntegracao");

                unitOfWork.FlushAndClear();
            }
        }

        #endregion Métodos Protegidos
    }
}