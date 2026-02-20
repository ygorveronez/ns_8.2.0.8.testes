namespace SGT.Intercab
{
    public sealed class WindowsBackgroundService : BackgroundServiceBase
    {
        private Dictionary<string, Thread> _threadsExecutando = new Dictionary<string, Thread>();
        private static int _quantidadeThreadsExecutar = 4;
        private static string _caminhoArquivos = string.Empty;
        private static string _caminhoArquivosIntegracao = string.Empty;

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            _caminhoArquivos = _configuration.GetValue<string>("CaminhoArquivos")!;
            _caminhoArquivosIntegracao = _configuration.GetValue<string>("CaminhoArquivosIntegracao")!;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Iniciando o serviço...");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Parando o serviço...");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.BuscarComFetch();

                        if (configuracaoIntegracaoIntegracaoEMP == null)
                            continue;

                        int quantidadeThreadsDisponiveis = 0;
                        int quantidadeThreadsAtivas = _threadsExecutando.Count;

                        for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                        {
                            if (_threadsExecutando.ElementAt(i).Value.IsAlive)
                                continue;

                            _threadsExecutando.Remove(_threadsExecutando.ElementAt(i).Key);
                        }

                        quantidadeThreadsDisponiveis = _quantidadeThreadsExecutar - _threadsExecutando.Count;

                        if (quantidadeThreadsDisponiveis <= 0)
                        {
                            await Task.Delay(1000, cancellationToken);
                            continue;
                        }

                        string keyVessel = "IntegracaoVessel";
                        if (configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoRecebimentoNavioEMP && !_threadsExecutando.Any(o => o.Key.Equals(keyVessel)))
                        {
                            System.Threading.Thread thread = new System.Threading.Thread(() => IntegrarConsumerEMPVessel(configuracaoIntegracaoIntegracaoEMP, cancellationToken));
                            thread.Start();
                            _threadsExecutando.Add(keyVessel, thread);
                        }

                        string keyCustomer = "IntegracaoCustomer";
                        if (configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoRecebimentoPessoaEMP && !_threadsExecutando.Any(o => o.Key.Equals(keyCustomer)))
                        {
                            System.Threading.Thread thread = new System.Threading.Thread(() => IntegrarConsumerEMPCustomer(configuracaoIntegracaoIntegracaoEMP, cancellationToken));
                            thread.Start();
                            _threadsExecutando.Add(keyCustomer, thread);
                        }

                        string keyBooking = "IntegracaoBooking";
                        if (configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoBooking && !_threadsExecutando.Any(o => o.Key.Equals(keyBooking)))
                        {
                            System.Threading.Thread thread = new System.Threading.Thread(() => IntegrarConsumerEMPBooking(configuracaoIntegracaoIntegracaoEMP, cancellationToken));
                            thread.Start();
                            _threadsExecutando.Add(keyBooking, thread);
                        }

                        string keyContainer = "IntegracaoContainer";
                        if (configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoContainerEMP && !_threadsExecutando.Any(o => o.Key.Equals(keyContainer)))
                        {
                            System.Threading.Thread thread = new System.Threading.Thread(() => IntegrarConsumerEMPContainer(configuracaoIntegracaoIntegracaoEMP, cancellationToken));
                            thread.Start();
                            _threadsExecutando.Add(keyContainer, thread);
                        }


                        string keyContainerNFTP = "IntegracaoContainerNFTP";
                        if (configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP && !_threadsExecutando.Any(o => o.Key.Equals(keyContainerNFTP)))
                        {
                            System.Threading.Thread thread = new System.Threading.Thread(() => IntegrarConsumerNFTPEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken));
                            thread.Start();
                            _threadsExecutando.Add(keyContainer, thread);
                        }

                        string keySchedule = "IntegracaoSchedule";
                        if (configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoRecebimentoScheduleEMP && !_threadsExecutando.Any(o => o.Key.Equals(keySchedule)))
                        {
                            System.Threading.Thread thread = new System.Threading.Thread(() => IntegrarConsumerEMPSchedule(configuracaoIntegracaoIntegracaoEMP, cancellationToken));
                            thread.Start();
                            _threadsExecutando.Add(keyContainer, thread);
                        }
                    }

                    Servicos.Log.TratarErro($"Aguardando 1 segundo para nova execução...");

                    await Task.Delay(30000, cancellationToken);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        #region Métodos Privados

        private void IntegrarConsumerEMPVessel(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.Intercab.IntegracaoVessel svcIntegracaoVessel = new Servicos.Embarcador.Integracao.Intercab.IntegracaoVessel(unitOfWork);
                    svcIntegracaoVessel.IntegrarConsumerEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void IntegrarConsumerEMPCustomer(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.Intercab.IntegracaoCustomer svcIntegracaoCustomer = new Servicos.Embarcador.Integracao.Intercab.IntegracaoCustomer(unitOfWork);
                    svcIntegracaoCustomer.IntegrarConsumerEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void IntegrarConsumerEMPBooking(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.Intercab.IntegracaoBooking svcIntegracaoBooking = new Servicos.Embarcador.Integracao.Intercab.IntegracaoBooking(unitOfWork, TipoServicoMultisoftware);
                    svcIntegracaoBooking.IntegrarConsumerEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void IntegrarConsumerEMPContainer(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.Intercab.IntegracaoContainer svcIntegracaoContainer = new Servicos.Embarcador.Integracao.Intercab.IntegracaoContainer(unitOfWork, TipoServicoMultisoftware);
                    svcIntegracaoContainer.IntegrarConsumerEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void IntegrarConsumerNFTPEMP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.Intercab.IntegracaoContainer svcIntegracaoContainer = new Servicos.Embarcador.Integracao.Intercab.IntegracaoContainer(unitOfWork, TipoServicoMultisoftware);
                    svcIntegracaoContainer.IntegrarConsumerNFTPEMP(configuracaoIntegracaoIntegracaoEMP, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void IntegrarConsumerEMPSchedule(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP, CancellationToken cancellationToken)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.Intercab.IntegracaoSchedule svcIntegracaoSchedule = new Servicos.Embarcador.Integracao.Intercab.IntegracaoSchedule(unitOfWork, TipoServicoMultisoftware);
                    svcIntegracaoSchedule.IntegrarConsumerEMP(configuracaoIntegracaoEMP, cancellationToken);
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