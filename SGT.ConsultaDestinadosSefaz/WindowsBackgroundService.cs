using System.Collections.Specialized;
using System.Threading;

namespace SGT.ConsultaDestinadosSefaz
{
    public sealed class WindowsBackgroundService : BackgroundServiceBase
    {
        private System.Threading.Thread _threadNFeDestinados;
        private System.Threading.Thread _threadCTeDestinados;
        private System.Threading.Thread _threadMDFeDestinados;
        private System.Threading.Thread _threadNFSeDestinados;
        private bool _consultaNFeDestinados;
        private bool _consultaCTeDestinados;
        private bool _consultaMDFeDestinados;
        private bool _consultaNFSeDestinadasSaoPauloSP;
        private string _nfseDestinadasSaoPauloSPCNPJs;
        private string _defaultCulture;

        // Variável para controlar os CNPJs com bloqueio por uso indevido.
        // Um CNPJ sai automaticamente da lista após uma hora do inicio do bloqueio.
        private Dictionary<string, DateTime> nfeUsoIndevidoPorCNPJ = new Dictionary<string, DateTime>();
        private Dictionary<string, DateTime> nfeSemDocumentoPorCNPJ = new Dictionary<string, DateTime>();
        private Dictionary<string, DateTime> cteUsoIndevidoPorCNPJ = new Dictionary<string, DateTime>();
        private Dictionary<string, DateTime> cteSemDocumentoPorCNPJ = new Dictionary<string, DateTime>();

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            _consultaNFeDestinados = _configuration.GetValue<bool>("AppSettings:ConsultaNFeDestinados");
            _consultaCTeDestinados = _configuration.GetValue<bool>("AppSettings:ConsultaCTeDestinados");
            _consultaMDFeDestinados = _configuration.GetValue<bool>("AppSettings:ConsultaMDFeDestinados");
            _consultaNFSeDestinadasSaoPauloSP = _configuration.GetValue<bool>("AppSettings:ConsultaNFSeDestinadasSaoPauloSP");
            _nfseDestinadasSaoPauloSPCNPJs = _configuration.GetValue<string>("NFSeDestinadasSaoPauloSP:Cnpjs");
            _defaultCulture = _configuration.GetValue<string>("AppSettings:defaultCulture");
            MaxPoolSize = 0;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Servicos.Log.GravarInfo($"Parando o serviço...");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_defaultCulture))
                _defaultCulture = "pt-BR";

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(_defaultCulture);

            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);
            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(StringConexao);

            Servicos.Log.GravarInfo("Serviço Consulta Destinados Sefaz iniciado.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_consultaNFeDestinados && _threadNFeDestinados == null)
                    {
                        _threadNFeDestinados = new System.Threading.Thread(() => ProcessarThreadConsultaNFeDestinados(cancellationToken));
                        _threadNFeDestinados.Start();
                    }

                    if (_consultaCTeDestinados && _threadCTeDestinados == null)
                    {
                        _threadCTeDestinados = new System.Threading.Thread(() => ProcessarThreadConsultaCTeDestinados(cancellationToken));
                        _threadCTeDestinados.Start();
                    }

                    if (_consultaMDFeDestinados && _threadMDFeDestinados == null)
                    {
                        _threadMDFeDestinados = new System.Threading.Thread(() => ProcessarThreadConsultaMDFeDestinados(cancellationToken));
                        _threadMDFeDestinados.Start();
                    }

                    if (_consultaNFSeDestinadasSaoPauloSP && _threadNFSeDestinados == null)
                    {
                        _threadNFSeDestinados = new System.Threading.Thread(() => ProcessarThreadConsultaNFSeDestinadas(cancellationToken));
                        _threadNFSeDestinados.Start();
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                await Task.Delay(new TimeSpan(1, 0, 0), cancellationToken);
            }
        }

        #region Buscar NF-e Destinados

        private async void ProcessarThreadConsultaNFeDestinados(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    BuscarNFesDestinados();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "NotasDestinadas");
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(300000, cancellationToken);
            }
        }

        private void BuscarNFesDestinados()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                try
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.GravarAdvertencia("Não é TMS, Multi NFe e Multiembarcador. " + TipoServicoMultisoftware.ToString(), "NotasDestinadas");
                        return;
                    }

                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                    List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                    List<int> codigosEmpresas = new List<int>();
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();
                    else
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasDocumentosDestinados();

                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        try
                        {
                            string codigoStatusRetornoSefaz;
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                            Servicos.Log.GravarInfo("Buscando empresa " + empresa.CNPJ_Formatado, "NotasDestinadas");

                            #region Validar Uso Indevido

                            // Verifica se o CNPJ está atualmente com bloqueio por uso indevido
                            if (nfeUsoIndevidoPorCNPJ.ContainsKey(empresa.CNPJ))
                            {
                                if (DateTime.Now.Subtract(nfeUsoIndevidoPorCNPJ[empresa.CNPJ]) >= new TimeSpan(1, 10, 0))
                                {
                                    nfeUsoIndevidoPorCNPJ.Remove(empresa.CNPJ);
                                }
                                else
                                {
                                    Servicos.Log.GravarAdvertencia(string.Format(@"Processo Abortado! Rejeição por uso indevido as {0}, próxima consulta será executada 1 horas após a rejeição.", nfeUsoIndevidoPorCNPJ[empresa.CNPJ].ToString("dd/MM/yyyy HH:mm:ss")), "NotasDestinadas");
                                    continue;
                                }
                            }

                            // Verifica se o CNPJ se a última consulta retornou documento
                            if (nfeSemDocumentoPorCNPJ.ContainsKey(empresa.CNPJ))
                            {
                                if (DateTime.Now.Subtract(nfeSemDocumentoPorCNPJ[empresa.CNPJ]) >= new TimeSpan(1, 10, 0))
                                {
                                    nfeSemDocumentoPorCNPJ.Remove(empresa.CNPJ);
                                }
                                else
                                {
                                    Servicos.Log.GravarAdvertencia(string.Format(@"Processo Abortado! Última consulta não retornou documento as {0}, próxima consulta será executada 1 horas após.", nfeSemDocumentoPorCNPJ[empresa.CNPJ].ToString("dd/MM/yyyy HH:mm:ss")), "NotasDestinadas");
                                    continue;
                                }
                            }

                            #endregion Validar Uso Indevido

                            if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(codigoEmpresa, unitOfWork.StringConexao, cnpjsNaoImportar, out string msgErro, out codigoStatusRetornoSefaz, 0, null, TipoServicoMultisoftware))
                                Servicos.Log.GravarAdvertencia("Consultando documentos: " + msgErro, "NotasDestinadas");

                            #region Adicionar CNPJ Verificação Uso Indevido

                            if (codigoStatusRetornoSefaz == "656")
                            {
                                if (nfeUsoIndevidoPorCNPJ.ContainsKey(empresa.CNPJ))
                                    nfeUsoIndevidoPorCNPJ[empresa.CNPJ] = DateTime.Now;
                                else
                                    nfeUsoIndevidoPorCNPJ.Add(empresa.CNPJ, DateTime.Now);
                            }

                            if (codigoStatusRetornoSefaz == "137")
                            {
                                if (nfeSemDocumentoPorCNPJ.ContainsKey(empresa.CNPJ))
                                    nfeSemDocumentoPorCNPJ[empresa.CNPJ] = DateTime.Now;
                                else
                                    nfeSemDocumentoPorCNPJ.Add(empresa.CNPJ, DateTime.Now);
                            }

                            #endregion  Adicionar CNPJ Verificação Uso Indevido
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Erro buscando empresa " + codigoEmpresa.ToString() + " - " + ex, "NotasDestinadas");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

        #endregion Buscar NF-e Destinados


        #region Buscar CT-e Destinados

        private async void ProcessarThreadConsultaCTeDestinados(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    BuscarCTEsDestinados();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "BuscarCTEsDestinados");
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(300000, cancellationToken);
            }
        }

        private void BuscarCTEsDestinados()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                try
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.GravarAdvertencia("Não é TMS, Multi NFe e Multiembarcador. " + TipoServicoMultisoftware.ToString(), "BuscarCTEsDestinados");
                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                    List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                    List<int> codigosEmpresas = new List<int>();
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();
                    else
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivas(cnpjsNaoImportar);

                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        try
                        {
                            string codigoStatusRetornoSefaz;
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                            Servicos.Log.GravarInfo("Buscando empresa " + empresa.CNPJ_Formatado, "BuscarCTEsDestinados");

                            #region Validar Uso Indevido

                            // Verifica se o CNPJ está atualmente com bloqueio por uso indevido
                            if (cteUsoIndevidoPorCNPJ.ContainsKey(empresa.CNPJ))
                            {
                                if (DateTime.Now.Subtract(cteUsoIndevidoPorCNPJ[empresa.CNPJ]) >= new TimeSpan(1, 10, 0))
                                {
                                    cteUsoIndevidoPorCNPJ.Remove(empresa.CNPJ);
                                }
                                else
                                {
                                    Servicos.Log.GravarAdvertencia(string.Format(@"Processo Abortado! Rejeição por uso indevido as {0}, próxima consulta será executada 1 horas após a rejeição.", cteUsoIndevidoPorCNPJ[empresa.CNPJ].ToString("dd/MM/yyyy HH:mm:ss")), "BuscarCTEsDestinados");
                                    continue;
                                }
                            }

                            // Verifica se o CNPJ se a última consulta retornou documento
                            if (cteSemDocumentoPorCNPJ.ContainsKey(empresa.CNPJ))
                            {
                                if (DateTime.Now.Subtract(cteSemDocumentoPorCNPJ[empresa.CNPJ]) >= new TimeSpan(1, 10, 0))
                                {
                                    cteSemDocumentoPorCNPJ.Remove(empresa.CNPJ);
                                }
                                else
                                {
                                    Servicos.Log.GravarAdvertencia(string.Format(@"Processo Abortado! Última consulta não retornou documento as {0}, próxima consulta será executada 1 horas após.", cteSemDocumentoPorCNPJ[empresa.CNPJ].ToString("dd/MM/yyyy HH:mm:ss")), "BuscarCTEsDestinados");
                                    continue;
                                }
                            }

                            #endregion Validar Uso Indevido

                            string msgErro = string.Empty;
                            if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosCTeEmpresa(codigoEmpresa, string.Empty, unitOfWork.StringConexao, 0, ref msgErro, out codigoStatusRetornoSefaz, null, TipoServicoMultisoftware))
                                Servicos.Log.GravarAdvertencia("Consultando documentos: " + msgErro, "BuscarCTEsDestinados");

                            #region  Adicionar CNPJ Verificação Uso Indevido

                            if (codigoStatusRetornoSefaz == "656")
                            {
                                if (cteUsoIndevidoPorCNPJ.ContainsKey(empresa.CNPJ))
                                    cteUsoIndevidoPorCNPJ[empresa.CNPJ] = DateTime.Now;
                                else
                                    cteUsoIndevidoPorCNPJ.Add(empresa.CNPJ, DateTime.Now);
                            }

                            if (codigoStatusRetornoSefaz == "137")
                            {
                                if (cteSemDocumentoPorCNPJ.ContainsKey(empresa.CNPJ))
                                    cteSemDocumentoPorCNPJ[empresa.CNPJ] = DateTime.Now;
                                else
                                    cteSemDocumentoPorCNPJ.Add(empresa.CNPJ, DateTime.Now);
                            }

                            #endregion  Adicionar CNPJ Verificação Uso Indevido
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "BuscarCTEsDestinados");
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "BuscarCTEsDestinados");
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

        #endregion Buscar CT-e Destinados


        #region Buscar MDF-e Destinados

        private async void ProcessarThreadConsultaMDFeDestinados(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    BuscarMDFesDestinados();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(300000, cancellationToken);
            }
        }

        private void BuscarMDFesDestinados()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                try
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.GravarAdvertencia("Não é TMS, Multi NFe e Multiembarcador. " + TipoServicoMultisoftware.ToString());
                        return;
                    }

                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                    List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                    List<int> codigosEmpresas = new List<int>();
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();
                    else
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivas(cnpjsNaoImportar);

                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        Servicos.Log.GravarInfo("Buscando empresa " + codigoEmpresa.ToString());
                        string msgErro = string.Empty;
                        if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosMDFeEmpresa(codigoEmpresa, string.Empty, unitOfWork.StringConexao, 0, ref msgErro, null, TipoServicoMultisoftware))
                            Servicos.Log.GravarAdvertencia("Erro consultando documentos MDF-e: " + msgErro);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

        #endregion Buscar MDF-e Destinados


        #region Buscar NFS-e Destinados

        private async void ProcessarThreadConsultaNFSeDestinadas(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    BuscarNFSeDestinadasSaoPauloSP();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(60000, cancellationToken);
            }
        }

        private void BuscarNFSeDestinadasSaoPauloSP()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                {
                    Servicos.Log.GravarAdvertencia("Cliente não é TMS, Multi NFe ou Multiembarcador. " + TipoServicoMultisoftware.ToString());
                    return;
                }

                if (string.IsNullOrEmpty(_nfseDestinadasSaoPauloSPCNPJs))
                {
                    Servicos.Log.GravarAdvertencia("CLiente não é TMS, Multi NFe ou Multiembarcador.");
                    return;
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                string[] cnpjs = _nfseDestinadasSaoPauloSPCNPJs.Split(',');

                foreach (string cnpj in cnpjs)
                {
                    try
                    {
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);
                        Servicos.Log.GravarInfo("Buscando empresa " + cnpj);

                        if (empresa == null)
                            Servicos.Log.GravarAdvertencia("Empresa não encontrada no repositorio:" + cnpj);
                        else if (!repEmpresa.EmpresaAtivaDocumentosDestinados(empresa.Codigo))
                            Servicos.Log.GravarAdvertencia("Documentos destinados não está habilitado na empresa " + empresa.NomeCNPJ);
                        else if (string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado) || empresa.DataFinalCertificado < DateTime.Today)
                            Servicos.Log.GravarAdvertencia("Certificado vencido ou inexistente na empresa " + empresa.NomeCNPJ);
                        else
                        {
                            Servicos.Embarcador.Documentos.NotaFiscalServicoDestinada servicoNFSeDestinada = new Servicos.Embarcador.Documentos.NotaFiscalServicoDestinada(unitOfWork);

                            if (!servicoNFSeDestinada.ConsultarNFSeDestinadaSaoPauloSP(DateTime.Today, DateTime.Today, empresa.Codigo, unitOfWork.StringConexao))
                                Servicos.Log.GravarAdvertencia("Falha ao consultar NFSe destianda Sao Paulo SP empresa:" + empresa.NomeCNPJ);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Erro buscando empresa " + cnpj + " - " + ex);
                    }
                }
            }
        }

        #endregion Buscar NFS-e Destinados
    }
}