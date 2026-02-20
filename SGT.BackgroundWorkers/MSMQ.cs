using Servicos.Embarcador.Integracao.HUB;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SGT.BackgroundWorkers
{
    public class MSMQ
    {
        private ConcurrentQueue<int> ListaConsultaEmissoes;
        private ConcurrentQueue<int> ListaOfertasHUBOfertas;
        private static readonly Lazy<MSMQ> Instance = new Lazy<MSMQ>(() => new MSMQ());

        private MSMQ()
        {
            if (ListaConsultaEmissoes == null)
                ListaConsultaEmissoes = new ConcurrentQueue<int>();

            if (ListaOfertasHUBOfertas == null)
                ListaOfertasHUBOfertas = new ConcurrentQueue<int>();
        }

        public static MSMQ GetInstance()
        {
            return Instance.Value;
        }

        public void QueueItem(Repositorio.UnitOfWork unitOfWork, int codigoClienteMultisoftware, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceOracle = "")
        {
            lock (ListaConsultaEmissoes)
            {
                if (ListaConsultaEmissoes == null)
                    ListaConsultaEmissoes = new ConcurrentQueue<int>();

                if (!ListaConsultaEmissoes.Contains(codigoClienteMultisoftware))
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                        tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                        tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin ||
                        tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Servicos.MSMQ.MSMQ.ListenerPrivateMessage(unitOfWork, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, stringConexao, tipoServicoMultisoftware, codigoClienteMultisoftware);
                        Servicos.MSMQ.MSMQ.ListenerPrivateMessage(unitOfWork, Dominio.MSMQ.MSMQQueue.SGTMobile, stringConexao, tipoServicoMultisoftware, 0);
                    }
                    else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        Servicos.MSMQ.MSMQ.ListenerPrivateMessage(unitOfWork, Dominio.MSMQ.MSMQQueue.MultiCTe, stringConexao, tipoServicoMultisoftware, codigoClienteMultisoftware);
                    else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                        Servicos.MSMQ.MSMQ.ListenerPrivateMessage(unitOfWork, Dominio.MSMQ.MSMQQueue.Terceiros, stringConexao, tipoServicoMultisoftware, codigoClienteMultisoftware);
                    else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                        Servicos.MSMQ.MSMQ.ListenerPrivateMessage(unitOfWork, Dominio.MSMQ.MSMQQueue.Fornecedor, stringConexao, tipoServicoMultisoftware, codigoClienteMultisoftware);

                    ListaConsultaEmissoes.Enqueue(codigoClienteMultisoftware);
                }
            }
        }
    }
}