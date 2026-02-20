using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.EDI
{
    public class StartupRetornoMagalog : ServicoBase
    {
        public StartupRetornoMagalog(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        private int tempoThread = 5000;
        private int codigoFilial = 0;
        private string dataInicioIntegracao = string.Empty;
        private string dataFimIntegracao = string.Empty;

        public void Iniciar(int filial, string dataInicio, string dataFim, int tamanhoStack)
        {
            Thread thread = new Thread(new ThreadStart(ExecutarThread), tamanhoStack);
            thread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            thread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            thread.IsBackground = true;
            codigoFilial = filial;
            dataInicioIntegracao = dataInicio;
            dataFimIntegracao = dataFim;
            thread.Start();
        }

        private void ExecutarThread()
        {
            Servicos.Log.TratarErro("Iniciou Task");
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(tempoThread);
                    
                    using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho);
                        VerificarCargaIntegracaoPendentesPorFilial(unidadeDeTrabalho);
                        unidadeDeTrabalho.Dispose();
                    }
                }
                catch (System.ServiceModel.CommunicationException com)
                {
                    Servicos.Log.TratarErro("Comunication: " + com);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (TimeoutException ti)
                {
                    Servicos.Log.TratarErro("Time out: " + ti);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    System.Threading.Thread.Sleep(tempoThread);
                }
            }
        }

        private void VerificarCargaIntegracaoPendentesPorFilial(Repositorio.UnitOfWork unitOfWork)
        {
            string StringConexao = unitOfWork.StringConexao;

            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            //todo: ver a possibilidade de tornar dinamico;
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 100;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            DateTime dataInicioFiltro = DateTime.MinValue;
            DateTime dataFimFiltro = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(dataFimIntegracao))
                DateTime.TryParseExact(dataFimIntegracao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataFimFiltro);

            if (!string.IsNullOrWhiteSpace(dataInicioIntegracao))
                DateTime.TryParseExact(dataInicioIntegracao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataInicioFiltro);

            List<int> integracoesPendentes = repCargaCargaIntegracao.BuscarCodigoIntegracoesPendentes(dataInicioFiltro, dataFimFiltro, codigoFilial, numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

            Servicos.Log.TratarErro("Iniciou envio de " + integracoesPendentes.Count() + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "VerificarCargaIntegracaoPendentes");

            for (var i = 0; i < integracoesPendentes.Count; i++)
            {
                var integracaoPendente = repCargaCargaIntegracao.BuscarPorCodigo(integracoesPendentes[i]);

                Servicos.Log.TratarErro("Iniciou carga protocolo " + integracaoPendente.Carga.Codigo.ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "VerificarCargaIntegracaoPendentes");

                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog:
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarCarga(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao:
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarCargaEscrituracao(integracaoPendente, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogMDFe:
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarMDFeManual(integracaoPendente, unitOfWork);
                        break;                    
                    default:
                        break;
                }

                Servicos.Log.TratarErro("Terminou carga protocolo " + integracaoPendente.Carga.Codigo.ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "VerificarCargaIntegracaoPendentes");

                unitOfWork.FlushAndClear();
            }

            Servicos.Log.TratarErro("Finalizou " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "VerificarCargaIntegracaoPendentes");

        }

    }
}

