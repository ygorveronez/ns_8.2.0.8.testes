using System;
using System.Windows;
using System.Windows.Input;

namespace SGT.GerenciadorApp
{
    public class ConfiguracaoEBSViewModel : ObservableObject
    {
        #region Fields

        private ConfiguracaoEBSModel _atualConfiguracaoEBS;
        private ICommand _saveConfiguracaoEBSCommand;
        private ICommand _migrarDadosCommand;
        private Window _window;

        #endregion

        #region Public Properties/Commands

        public ConfiguracaoEBSViewModel(Window window)
        {
            _window = window;
        }

        public ConfiguracaoEBSModel AtualConfiguracaoEBS
        {
            get
            {
                if (_atualConfiguracaoEBS == null)
                    GetConfiguracaoEBS();

                return _atualConfiguracaoEBS;
            }
            set
            {
                if (value != _atualConfiguracaoEBS)
                {
                    _atualConfiguracaoEBS = value;
                    OnPropertyChanged("AtualConfiguracaoEBS");
                }
            }
        }

        public ICommand SaveConfiguracaoEBSCommand
        {
            get
            {
                if (_saveConfiguracaoEBSCommand == null)
                {
                    _saveConfiguracaoEBSCommand = new RelayCommand(
                        param => SaveConfiguracaoEBS(),
                        param => (AtualConfiguracaoEBS != null)
                    );
                }
                return _saveConfiguracaoEBSCommand;
            }
        }

        public ICommand MigrarDadosCommand
        {
            get
            {
                if (_migrarDadosCommand == null)
                {
                    _migrarDadosCommand = new RelayCommand(
                        param => MigraEssaCaralhaToda()
                    );
                }
                return _migrarDadosCommand;
            }
        }

        #endregion

        #region Private Helpers

        private void GetConfiguracaoEBS()
        {
            string[] codigosClientes = System.Configuration.ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            int.TryParse(codigosClientes[0], out int codigoClienteMultisoftware);

            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin))
            {
                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao))
                {
                    Repositorio.Embarcador.Configuracoes.EBS repConfiguracaoEBS = new Repositorio.Embarcador.Configuracoes.EBS(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.Configuracoes.EBS ebs = repConfiguracaoEBS.Buscar();

                    ConfiguracaoEBSModel config = new ConfiguracaoEBSModel();
                    config.CaminhoSalvarArquivos = ebs?.CaminhoSalvarArquivo ?? string.Empty;
                    config.DiasRetroativos = ebs?.DiasRetroativos ?? 0;
                    config.EmailsEnvioArquivos = ebs?.Emails ?? string.Empty;
                    config.HoraExecucao = ebs?.HoraExecucao.ToString(@"hh\:mm") ?? string.Empty;
                    AtualConfiguracaoEBS = config;
                }
            }
        }

        private void SaveConfiguracaoEBS()
        {
            string[] codigosClientes = System.Configuration.ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            int.TryParse(codigosClientes[0], out int codigoClienteMultisoftware);

            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin))
            {
                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao))
                {
                    Repositorio.Embarcador.Configuracoes.EBS repConfiguracaoEBS = new Repositorio.Embarcador.Configuracoes.EBS(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.Configuracoes.EBS ebs = repConfiguracaoEBS.Buscar();

                    if (ebs == null)
                        ebs = new Dominio.Entidades.Embarcador.Configuracoes.EBS();

                    ebs.CaminhoSalvarArquivo = AtualConfiguracaoEBS.CaminhoSalvarArquivos;
                    ebs.DiasRetroativos = AtualConfiguracaoEBS.DiasRetroativos;
                    ebs.Emails = AtualConfiguracaoEBS.EmailsEnvioArquivos;
                    ebs.HoraExecucao = TimeSpan.ParseExact(AtualConfiguracaoEBS.HoraExecucao, @"hh\:mm", null);

                    if (ebs.Codigo > 0)
                        repConfiguracaoEBS.Atualizar(ebs);
                    else
                        repConfiguracaoEBS.Inserir(ebs);

                    MessageBox.Show("Configuração salva com sucesso!", "Sucesso!", MessageBoxButton.OK);

                    _window.Close();
                }
            }
        }

        private void MigraEssaCaralhaToda()
        {
            //Servicos.Log.TratarErro("Iniciou migração dos dados dos CT-es para a tabela de documento faturamento...");

            //string stringConexao = "Data Source=ank27fvlel.database.windows.net,1433;Initial Catalog=ControleCTe123;User Id=MultiCTe;Password=Multi@2015;";

            string stringConexao = "Data Source=192.168.0.125;Initial Catalog=ControleCTe;User Id=sa;Password=Multi@2017;";

            using (Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao))//App.StringConexao))
            {
                try
                {
                    Migrations.Migrator.MigrateToLatest(stringConexao);

                    Servicos.Embarcador.Veiculo.ReceitaDespesa.ProcessarReceitasEDespesas(unidadeTrabalho);


                    //Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotosRetroativosNFe(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unidadeTrabalho);

                    //Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotosRetroativosCTe(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unidadeTrabalho);

                    //Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

                    //List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = repGrupoPessoas.BuscarQueArmazenaCanhotoCTe();

                    //foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas in gruposPessoas)
                    //{
                    //    Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotosRetroativosCTe(grupoPessoas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unidadeTrabalho);
                    //}

                    //Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                    //Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                    //Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                    /////DANONE
                    //List<int> codigosCargaCTe = repDocumentoFaturamento.BuscarCodigosCargaCTeNaoFaturadosDanone();

                    //foreach (int codigoCargaCTe in codigosCargaCTe)
                    //{
                    //    unidadeTrabalho.FlushAndClear();

                    //    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                    //    unidadeTrabalho.Start();

                    //    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cargaCTe.Carga, cargaCTe.CTe, cargaCTe.CargaCTeComplementoInfo?.CargaOcorrencia, unidadeTrabalho);

                    //    unidadeTrabalho.CommitChanges();
                    //}

                    //List<int> codigosCargaCTe = repDocumentoFaturamento.BuscarCodigosCargaCTeNaoFaturadosSemAvon();

                    //foreach (int codigoCargaCTe in codigosCargaCTe)
                    //{
                    //    unidadeTrabalho.FlushAndClearr();

                    //    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                    //    unidadeTrabalho.Start();

                    //    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cargaCTe.Carga, cargaCTe.CTe, cargaCTe.CargaCTeComplementoInfo?.CargaOcorrencia, unidadeTrabalho);

                    //    unidadeTrabalho.CommitChanges();
                    //}

                    //List<int> codigosCargaCTeOcorrenciaAvon = repDocumentoFaturamento.BuscarCodigosCargaCTeDeOcorrenciaNaoFaturadosAvon();

                    //foreach (int codigoCargaCTe in codigosCargaCTeOcorrenciaAvon)
                    //{
                    //    unidadeTrabalho.FlushAndClear();

                    //    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                    //    unidadeTrabalho.Start();

                    //    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cargaCTe.Carga, cargaCTe.CTe, cargaCTe.CargaCTeComplementoInfo.CargaOcorrencia, unidadeTrabalho);

                    //    unidadeTrabalho.CommitChanges();
                    //}

                    //List<int> codigosCargasAvon = repDocumentoFaturamento.BuscarCodigosCargaNaoFaturadasAvon();

                    //foreach (int codigoCarga in codigosCargasAvon)
                    //{
                    //    unidadeTrabalho.FlushAndClear();

                    //    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    //    unidadeTrabalho.Start();

                    //    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorCarga(carga, unidadeTrabalho);

                    //    unidadeTrabalho.CommitChanges();
                    //}

                    //Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                    //Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeTrabalho);

                    //List<int> codigosCTeSemVeiculo = repCTe.BuscarCodigoCTeVersao30EmCargaSemVeiculoParaMigracao();

                    //foreach (int codigoCTe in codigosCTeSemVeiculo)
                    //{
                    //    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(codigoCTe);

                    //    if (cargaCTe == null)
                    //        continue;

                    //    if (cargaCTe.CTe.Veiculos.Count > 0)
                    //        continue;

                    //    if (cargaCTe.Carga.Veiculo != null)
                    //    {
                    //        Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE();

                    //        veiculoCTe.ImportadoCarga = true;
                    //        veiculoCTe.CTE = cargaCTe.CTe;
                    //        veiculoCTe.Veiculo = cargaCTe.Carga.Veiculo;
                    //        veiculoCTe.SetarDadosVeiculo(cargaCTe.Carga.Veiculo);

                    //        repVeiculoCTe.Inserir(veiculoCTe);

                    //        foreach (Dominio.Entidades.Veiculo reboque in cargaCTe.Carga.VeiculosVinculados)
                    //        {
                    //            Dominio.Entidades.VeiculoCTE reboqueCTe = new Dominio.Entidades.VeiculoCTE();

                    //            reboqueCTe.ImportadoCarga = true;
                    //            reboqueCTe.CTE = cargaCTe.CTe;
                    //            reboqueCTe.Veiculo = reboque;
                    //            reboqueCTe.SetarDadosVeiculo(reboque);

                    //            repVeiculoCTe.Inserir(reboqueCTe);
                    //        }
                    //    }
                    //}



                    //Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(unidadeTrabalho);

                    //Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote cargaCTeIntegracaoLote = repCargaCTeIntegracaoLote.BuscarPorCodigo(325306);

                    //Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura svcIntegracaoNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura("");

                    //Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura.EnviarRetornoDT(cargaCTeIntegracaoLote, cargaCTeIntegracaoLote.CTes.ToList(), unidadeTrabalho);


                    _window.Close();
                }
                catch (Exception ex)
                {
                    unidadeTrabalho.Rollback();
                    Servicos.Log.TratarErro(ex);
                    MessageBox.Show("BOLOR: " + ex.ToString(), "FALHA!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}
