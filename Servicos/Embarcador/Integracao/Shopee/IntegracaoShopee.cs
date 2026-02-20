using Flurl.Http;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Shopee
{
    public class IntegracaoShopee
    {
        #region Atributos Globais

        private string Token { get; set; }
        private string _url { get; set; }
        private string _usuario;
        private string _password;
        private dynamic _access_token;
        private object _semaforo;
        private object _semaforolstCTeTerceiroXMLParaPersistir;
        private object _semaforoDeletes;
        private bool _lockDeletes = false;
        private int _SleepPasso2 = 2;
        private bool _integrarPacotes = false;
        private int _SleepFilaVazia = 1000;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Servicos.Embarcador.Carga.CTeSubContratacao _servicoCTeSubContratacao;
        private Servicos.Embarcador.CTe.CTe _serCte;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoTMS;
        private List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML> _lstCTeTerceiroXMLParaPersistir = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();
        private Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe _cacheObjetoValorCTeFull;

        #endregion

        #region Construtores

        public IntegracaoShopee(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string _StringConexao, bool lockDeletes, int timesleep)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            _lockDeletes = lockDeletes;
            _SleepPasso2 = timesleep;
            _serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            _servicoCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            Repositorio.Embarcador.Configuracoes.IntegracaoShopee repIntegracaoShopee = new Repositorio.Embarcador.Configuracoes.IntegracaoShopee(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoShopee IntegracaoShopee = repIntegracaoShopee.BuscarPrimeiroRegistro();

            if (IntegracaoShopee == null)
            {
                Servicos.Log.TratarErro($"Integração Shopee não configurada.");
                return;
            }

            _integrarPacotes = IntegracaoShopee.PossuiIntegracaoPacote;

            _url = IntegracaoShopee.EndpointPacote?.Replace("docs#/", "")?.Replace("docs#", "");
            if (!_url.EndsWith("/"))
                _url = _url + $"/";

            _usuario = IntegracaoShopee.Usuario;
            _password = IntegracaoShopee.Senha;
            _semaforo = new object();
            _semaforolstCTeTerceiroXMLParaPersistir = new object();
            _semaforoDeletes = new object();
            // Cache 
            _configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();
            GetToken();
        }

        #endregion

        #region Metodos Publicos 

        public IntegracaoShopee StartPasso1(List<Dominio.Entidades.Embarcador.Cargas.Pacote> pacotes, int threadsEmParalelo, string arquivoPasso1, Repositorio.UnitOfWork unitOfWork)
        {
            if (_integrarPacotes)
            {
                Servicos.Log.TratarErro($"Iniciando processamento de {pacotes.Count} pacotes.", arquivoPasso1);
                int tamanhoDoLote = threadsEmParalelo;
                for (int i = 0; i < pacotes.Count; i += tamanhoDoLote)
                {
                    DateTime dt = DateTime.Now;
                    List<Dominio.Entidades.Embarcador.Cargas.Pacote> _IdsPacote = pacotes.Skip(i).Take(tamanhoDoLote).ToList();
                    Servicos.Log.TratarErro($"Iniciando lote {i}.");
                    DateTime tempo = DateTime.Now;

                    List<System.Threading.Thread> lstThreds = new List<System.Threading.Thread>();
                    foreach (var pacote in _IdsPacote)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(() => ResquestPacote(pacote));
                        lstThreds.Add(thread);
                        thread.Start();
                    }

                    while (lstThreds.Where(x => x.IsAlive == true).Count() > 0) { }

                    Servicos.Log.TratarErro($"Finalizou lote {i}. tamanho {_IdsPacote.Count}, tempo total {(DateTime.Now - tempo).TotalSeconds} segundos.  {_IdsPacote.Count / (DateTime.Now - tempo).TotalSeconds} pacotes por segundo ", arquivoPasso1);

                    Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroXML repCTeTerceiroXML = new Repositorio.Embarcador.CTe.CTeTerceiroXML(unitOfWork);

                    unitOfWork.Start();

                    repCTeTerceiroXML.Inserir(_lstCTeTerceiroXMLParaPersistir, "T_CTE_TERCEIRO_XML");
                    foreach (var xml in _lstCTeTerceiroXMLParaPersistir)
                    {
                        xml.Pacote.CTeTerceiroXML = xml;
                        repPacote.Atualizar(xml.Pacote);
                    }

                    unitOfWork.CommitChanges();
                    unitOfWork.FlushAndClear();
                    _lstCTeTerceiroXMLParaPersistir = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();
                }
            }
            if (pacotes.Count < 25)
                System.Threading.Thread.Sleep(_SleepFilaVazia);

            return this;
        }

        public IntegracaoShopee StartPasso2(List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes, int threadsEmParalelo, string stringConexao, string arquivoPAsso2, Repositorio.UnitOfWork unitOfWork)
        {
            if (_integrarPacotes && cargaPedidoPacotes.Count > 0)
            {
                Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                Servicos.Log.TratarErro($"Iniciando processamento de {cargaPedidoPacotes.Count} pacotes Passo2. Vinclular CTE Terceiro e pacote.", arquivoPAsso2);
                int tamanhoDoLote = threadsEmParalelo;
                if (cargaPedidoPacotes == null)
                    cargaPedidoPacotes = repPacote.BuscarPacotesPendentesIntegracaoPasso2();

                #region PREPARA LISTAS  

                var lstCteParaImportar = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
                foreach (var cargaPedidoPacote in cargaPedidoPacotes)
                {
                    try
                    {
                        lstCteParaImportar.Add(ConvertercteProcEmCTeTerceiro(MultiSoftware.CTe.Servicos.Leitura.Ler(cargaPedidoPacote.Pacote.CTeTerceiroXML.XML.ToStream())));
                    }
                    catch (Exception)
                    {
                        List<string> comandos = new List<string>();
                        comandos.Add($" UPDATE T_PACOTE SET PCT_MENSAGEM_INTEGRACAO = 'Erro passo2 ConvertercteProcEmCTeTerceiro', PCT_SITUACAO_INTEGRACAO = 3 WHERE PCT_CODIGO = {cargaPedidoPacote.Pacote.Codigo} "); // SQL-INJECTION-SAFE
                        repPacote.ComandSql(comandos);
                    }
                }

                PreparaCachePasso2(lstCteParaImportar, cargaPedidoPacotes, arquivoPAsso2, unitOfWork);

                #endregion

                for (int i = 0; i < cargaPedidoPacotes.Count; i += tamanhoDoLote)
                {
                    DateTime dt = DateTime.Now;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> _cargaPedidoPacotes = cargaPedidoPacotes.Skip(i).Take(tamanhoDoLote).ToList();
                    Servicos.Log.TratarErro($"Iniciando lote {i}.", arquivoPAsso2);
                    DateTime tempo = DateTime.Now;
                    List<System.Threading.Thread> lstThreds = new List<System.Threading.Thread>();

                    foreach (var cargaPedidoPacote in _cargaPedidoPacotes)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(() => ProcessarIntegracaoPasso2(cargaPedidoPacote, stringConexao));
                        lstThreds.Add(thread);
                        thread.Start();
                        System.Threading.Thread.Sleep(_SleepPasso2);
                    }

                    while (lstThreds.Where(x => x.IsAlive == true).Count() > 0) { }

                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Servicos.Log.TratarErro($"Finalizou lote {i}. tamanho {_cargaPedidoPacotes.Count}, tempo total {(DateTime.Now - tempo).TotalSeconds} segundos.  {_cargaPedidoPacotes.Count / (DateTime.Now - tempo).TotalSeconds} pacotes por segundo ", arquivoPAsso2);
                }
            }

            if (cargaPedidoPacotes.Count < 25)
                System.Threading.Thread.Sleep(_SleepFilaVazia);

            return this;
        }

        public void StartPasso1DownloadDocumentos(int threadsEmParalelo, string arquivoPasso1, Repositorio.UnitOfWork unitOfWork, CancellationToken token)
        {
            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);

            List<int> pacotes = repPacote.BuscarPacotesPendentesIntegracaoPasso1Download();
            //int teste = 0;
            //pacotes.Add(teste);
            //threadsEmParalelo = 1;

            while (pacotes.Count > 0)
            {
                List<Task> lstTasks = new List<Task>();

                if (pacotes.Count <= threadsEmParalelo)
                {
                    Task newTask = new Task(() => { ResquestPacotes(pacotes, unitOfWork.StringConexao); });
                    lstTasks.Add(newTask);
                    newTask.Start();
                }
                else
                {
                    decimal decimalBlocos = Math.Ceiling(((decimal)pacotes.Count) / threadsEmParalelo);
                    int blocos = (int)Math.Truncate(decimalBlocos);

                    for (int i = 0; i < threadsEmParalelo; i++)
                    {
                        List<int> pacotesThread = pacotes.Skip(i * blocos).Take(blocos).ToList();
                        Task newTask = new Task(() => { ResquestPacotes(pacotesThread, unitOfWork.StringConexao); });
                        lstTasks.Add(newTask);
                        newTask.Start();

                        Thread.Sleep(100);
                    }
                }

                Task.WaitAll(lstTasks.ToArray());

                if (token.IsCancellationRequested)
                    return;

                pacotes = repPacote.BuscarPacotesPendentesIntegracaoPasso1Download();
            }
        }

        public void StartPasso2VincularDocumentosCarga(int threadsEmParalelo, Repositorio.UnitOfWork unitOfWork, CancellationToken token)
        {
            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes = repPacote.BuscarPacotesPendentesIntegracaoPasso2();

            if (cargaPedidoPacotes.Count > 0)
            {
                //Agrupar Registros por Carga
                var queryGroup = cargaPedidoPacotes.GroupBy(x => new { x.CargaPedido }).Select(y => new { CargaPedido = y.Key.CargaPedido, listDoc = y });

                List<Task> lstTasks = new List<Task>();
                foreach (var item in queryGroup)
                {
                    Task newTask = new Task(() => { ProcessarIntegracaoPasso2VincularDocumentosCarga(item.CargaPedido, item.listDoc.ToList(), unitOfWork.StringConexao); });
                    lstTasks.Add(newTask);
                    newTask.Start();

                    System.Threading.Thread.Sleep(_SleepPasso2);
                }

                Task.WaitAll(lstTasks.ToArray());
            }
        }

        public string ProcessarIntegracaoPasso1(Dominio.Entidades.Embarcador.Cargas.Pacote pacote, string xml)
        {
            Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML cteTerceiroXML = _servicoCTeSubContratacao.Passo1De2CriacaoCTeTerceiroXMLPorPacote(pacote, xml);
            if (cteTerceiroXML != null)
            {
                lock (_semaforolstCTeTerceiroXMLParaPersistir)
                {
                    _lstCTeTerceiroXMLParaPersistir.Add(cteTerceiroXML);
                }
                return "";
            }
            else
            {
                return "XML Não pode ser vazio";
            }
        }

        public string ProcessarIntegracaoPasso2(Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote, string _StringConexao, Dominio.Entidades.Embarcador.Cargas.PacoteWebHook pacoteWebHook = null)
        {
            string mensagem = "";
            string LogKey = "";
            string arquivoErroPasso2 = $"Erro_passo2_{DateTime.Now.ToString("yyyyMMdd")}";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    try
                    {
                        unitOfWork.Start();
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null;
                        Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteProc;
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe = null;
                        Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = new Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente();
                        Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                        if (cargaPedidoPacote == null) // quando for pacoteWebHook apenas importamos o CTE terceiro. Porteriormente quando o pedido e carga forem criados para transportar os pacotes faremos o VincularCTeCargaPedidoPacote diretamente com entidade Pacote 
                        {
                            cteProc = ConvertercteProcEmCTeTerceiro(MultiSoftware.CTe.Servicos.Leitura.Ler(pacoteWebHook.CTeTerceiroXML.XML.ToStream()));
                            LogKey = pacoteWebHook.LogKey;
                        }
                        else
                        {
                            cteProc = ConvertercteProcEmCTeTerceiro(MultiSoftware.CTe.Servicos.Leitura.Ler(cargaPedidoPacote.Pacote.CTeTerceiroXML.XML.ToStream()));
                            LogKey = cargaPedidoPacote.Pacote.LogKey;
                            if (cargaPedidoPacote.Pacote.CTeTerceiroXML.CTeTerceiro != null)
                                cteTerceiro = cargaPedidoPacote.Pacote.CTeTerceiroXML.CTeTerceiro;
                        }

                        bool bIncluiu = false;
                        if (cteTerceiro == null)
                        {
                            bIncluiu = true;
                            cteTerceiro = _servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref mensagem, CargaCTe, cteProc, null, false, 0, false, false, _tipoServicoMultisoftware, LogKey, _cacheObjetoValorCTeFull, objetoValorPersistente);
                        }

                        if (mensagem == "" && cargaPedidoPacote != null)
                            mensagem = new Servicos.Embarcador.Pacote.Pacote(unitOfWork, _tipoServicoMultisoftware).VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro, _cacheObjetoValorCTeFull?.pedidoCTesParaSubContratacao, _cacheObjetoValorCTeFull?.ConfiguracaoTMS, false, _cacheObjetoValorCTeFull, objetoValorPersistente).GetAwaiter().GetResult();

                        Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
                        Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
                        Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
                        Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete repCTeTerceiroComponenteFrete = new Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                        Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                        Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
                        Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

                        if (_lockDeletes)
                        {
                            lock (_semaforoDeletes)
                            {
                                if (bIncluiu)
                                {
                                    repCTeTerceiro.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().Distinct().ToList());
                                    repCTeTerceiroQuantidade.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>().Distinct().ToList());
                                    repCTeTerceiroNFe.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>().Distinct().ToList());
                                    repCTeTerceiroComponenteFrete.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>().Distinct().ToList());
                                    repParticipanteCTe.ComandSql(objetoValorPersistente.lstDelete.OfType<string>().Distinct().ToList());
                                }

                                repCliente.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Cliente>().Distinct().ToList());
                                repClienteDescarga.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>().Distinct().ToList());
                                repModalidadeClientePessoas.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>().Distinct().ToList());
                                repPedidoCTeParaSubContratacao.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>().Distinct().ToList());
                            }
                        }
                        else
                        {
                            if (bIncluiu)
                            {
                                repCTeTerceiro.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().Distinct().ToList());
                                repCTeTerceiroQuantidade.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>().Distinct().ToList());
                                repCTeTerceiroNFe.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>().Distinct().ToList());
                                repCTeTerceiroComponenteFrete.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>().Distinct().ToList());
                                repParticipanteCTe.ComandSql(objetoValorPersistente.lstDelete.OfType<string>().Distinct().ToList());
                            }

                            repCliente.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Cliente>().Distinct().ToList());
                            repClienteDescarga.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>().Distinct().ToList());
                            repModalidadeClientePessoas.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>().Distinct().ToList());
                            repPedidoCTeParaSubContratacao.Deletar(objetoValorPersistente.lstDelete.OfType<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>().Distinct().ToList());
                        }

                        repModalidadeClientePessoas.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>().ToList(), "T_CLIENTE_MODALIDADE_CLIENTES");
                        repParticipanteCTe.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.ParticipanteCTe>().ToList(), "T_CTE_PARTICIPANTE");

                        //tratamento para cliente já existentes na tabela T_CLIENTE
                        var inserirCliente = objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Cliente>().ToList();
                        List<double> result = new List<double>(
                                    from obj in inserirCliente
                                    select obj.CPF_CNPJ
                                ).ToList();

                        var clienteCadastrados = repCliente.BuscarPorCNPJsCPFs(result);
                        if (clienteCadastrados.Count > 0)
                        {
                            foreach (var cliente in clienteCadastrados)
                            {
                                inserirCliente.Remove(cliente);
                            }

                            if (inserirCliente.Count > 0)
                            {
                                repCliente.Inserir(inserirCliente);
                            }
                        }
                        else
                        {
                            repCliente.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Cliente>().ToList(), "T_CLIENTE");
                        }

                        repCTeTerceiro.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().ToList(), "T_CTE_TERCEIRO");
                        repCTeTerceiroQuantidade.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>().ToList(), "T_CTE_TERCEIRO_QUANTIDADE");
                        repCTeTerceiroNFe.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>().ToList(), "T_CTE_TERCEIRO_NFE");
                        repCTeTerceiroComponenteFrete.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>().ToList(), "T_CTE_TERCEIRO_COMPONENTE_FRETE");
                        repClienteDescarga.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>().ToList(), "T_CLIENTE_DESCARGA");
                        repPedidoCTeParaSubContratacao.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>().Distinct().ToList(), "T_PEDIDO_CTE_PARA_SUB_CONTRATACAO");

                        Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                        repParticipanteCTe.ComandSql(objetoValorPersistente.lstDelete.OfType<string>().Distinct().ToList());
                        List<string> comandos = new List<string>();
                        int situacaoIntegracao = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                        if (cargaPedidoPacote == null)
                            comandos.Add($" UPDATE T_PACOTE_WEBHOOK SET PWT_MENSAGEM_INTEGRACAO = 'Integrado passo2 com sucesso.', PWT_SITUACAO_INTEGRACAO = {situacaoIntegracao} WHERE PWT_CODIGO = {pacoteWebHook.Codigo} ");
                        else
                            comandos.Add($" UPDATE T_PACOTE SET PCT_MENSAGEM_INTEGRACAO = 'Integrado passo2 com sucesso.', PCT_SITUACAO_INTEGRACAO = {situacaoIntegracao} WHERE PCT_CODIGO = {cargaPedidoPacote.Pacote.Codigo} ");

                        repPacote.ComandSql(comandos);

                        foreach (var upd in objetoValorPersistente.lstUpdate.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().Distinct().ToList())
                            comandos.Add($"  UPDATE T_CTE_TERCEIRO SET CPS_IDENTIFICACAO_PACOTE = '{LogKey}' WHERE CPS_CODIGO = {upd.Codigo} "); 

                        if (cargaPedidoPacote == null)
                            comandos.Add($"  UPDATE T_CTE_TERCEIRO_XML SET CPS_CODIGO = {cteTerceiro.Codigo} WHERE CTX_CODIGO = {pacoteWebHook.CTeTerceiroXML.Codigo} ");
                        else
                            comandos.Add($"  UPDATE T_CTE_TERCEIRO_XML SET CPS_CODIGO = {cteTerceiro.Codigo} WHERE CTX_CODIGO = {cargaPedidoPacote.Pacote.CTeTerceiroXML.Codigo} "); 

                        repPacote.ComandSql(comandos);
                        unitOfWork.CommitChanges();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            //unitOfWork.Rollback();
                            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                            int situacaoIntegracao = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            List<string> comandos = new List<string>();
                            Servicos.Log.TratarErro($"Erro pacote 01 => {LogKey}:", arquivoErroPasso2);
                            Servicos.Log.TratarErro(e, arquivoErroPasso2);

                            if (cargaPedidoPacote == null)
                                comandos.Add($" UPDATE T_PACOTE_WEBHOOK SET PWT_MENSAGEM_INTEGRACAO = 'Erro passo2.', PWT_SITUACAO_INTEGRACAO = {situacaoIntegracao} WHERE PWT_CODIGO = {pacoteWebHook.Codigo} ");
                            else
                                comandos.Add($" UPDATE T_PACOTE SET PCT_MENSAGEM_INTEGRACAO = 'Erro passo2.', PCT_SITUACAO_INTEGRACAO = {situacaoIntegracao} WHERE PCT_CODIGO = {cargaPedidoPacote.Pacote.Codigo} "); // SQL-INJECTION-SAFE

                            repPacote.ComandSql(comandos);
                            unitOfWork.CommitChanges();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            catch (Exception e)
            {
                mensagem = "";
                Servicos.Log.TratarErro($"Erro pacote 02 => {LogKey}:", arquivoErroPasso2);
                Servicos.Log.TratarErro(e, arquivoErroPasso2);
            }

            return mensagem;
        }

        public void ProcessarIntegracaoPasso2VincularDocumentosCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargasPedidoPacote, string _StringConexao)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);
                    Servicos.Cliente servicoCliente = new Servicos.Cliente();

                    int limiteDocumentoAdicionaisPorCTeTerceiro = 500;
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacoteDocumentoPrincipal = null;
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null;
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidade = null;
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe = null;

                    List<Task> lstTasks = new List<Task>();
                    int ultimoPacote = cargasPedidoPacote.Max(o => o.Codigo);
                    int totalDocumentosAdicionadosDocumentoPrincipal = 0;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote in cargasPedidoPacote.OrderBy(o => o.Codigo))
                    {
                        try
                        {
                            if (cargaPedidoPacoteDocumentoPrincipal == null)
                            {
                                cargaPedidoPacoteDocumentoPrincipal = repCargaPedidoPacote.BuscarDocumentoPrincipal(cargaPedido.Codigo, limiteDocumentoAdicionaisPorCTeTerceiro);
                                totalDocumentosAdicionadosDocumentoPrincipal = cargaPedidoPacoteDocumentoPrincipal?.TotalDocumentosAdicionais ?? 0;

                                if (cargaPedidoPacoteDocumentoPrincipal != null)
                                {
                                    cteTerceiro = cargaPedidoPacoteDocumentoPrincipal.Pacote.CTeTerceiroXML.CTeTerceiro;
                                    cteTerceiroQuantidade = repCTeTerceiroQuantidade.BuscarPorCTeTerceiroEUnidadeMedida(cteTerceiro.Codigo, Dominio.Enumeradores.UnidadeMedida.KG);
                                    cteTerceiroNFe = repCTeTerceiroNFe.BuscarPrimeiroPorCTeTerceiro(cteTerceiro.Codigo);
                                }
                            }

                            if (cargaPedidoPacoteDocumentoPrincipal == null)
                            {
                                Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = new Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente();
                                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteProc = ConvertercteProcEmCTeTerceiro(MultiSoftware.CTe.Servicos.Leitura.Ler(cargaPedidoPacote.Pacote.CTeTerceiroXML.XML.ToStream()));
                                Dominio.Entidades.Cliente emitente = null;
                                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoEmissor = servicoCliente.ConverterParaTransportadorTerceiro(cteProc.Emitente, "emitente do CT-e", unitOfWork);

                                if (retornoConversaoEmissor.Status)
                                    emitente = retornoConversaoEmissor.cliente;
                                else
                                {
                                    throw new Exception(retornoConversaoEmissor.Mensagem);
                                }

                                unitOfWork.Start();

                                try
                                {
                                    // Setar documento principal
                                    cargaPedidoPacote.DocumentoPrincipal = true;
                                    repCargaPedidoPacote.Atualizar(cargaPedidoPacote);

                                    string LogKey = cargaPedidoPacote.Pacote.LogKey;
                                    string mensagem = null;
                                    cteTerceiro = _servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref mensagem, null, cteProc, null, false, 0, false, false, _tipoServicoMultisoftware, LogKey, _cacheObjetoValorCTeFull, objetoValorPersistente);

                                    if (mensagem == "" && cargaPedidoPacote != null)
                                        mensagem = new Servicos.Embarcador.Pacote.Pacote(unitOfWork, _tipoServicoMultisoftware).VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro, _cacheObjetoValorCTeFull?.pedidoCTesParaSubContratacao, _cacheObjetoValorCTeFull?.ConfiguracaoTMS, false, _cacheObjetoValorCTeFull, objetoValorPersistente).GetAwaiter().GetResult();

                                    // Limpar documentos adicionais
                                    repCTeTerceiroDocumentoAdicional.RemoverPorCTeTerceiro(cteTerceiro.Codigo);

                                    Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
                                    Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete repCTeTerceiroComponenteFrete = new Repositorio.Embarcador.CTe.CTeTerceiroComponenteFrete(unitOfWork);
                                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                                    Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                                    Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
                                    Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

                                    repModalidadeClientePessoas.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>().ToList(), "T_CLIENTE_MODALIDADE_CLIENTES");
                                    repCliente.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Cliente>().ToList(), "T_CLIENTE");
                                    repParticipanteCTe.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.ParticipanteCTe>().ToList(), "T_CTE_PARTICIPANTE");
                                    repCTeTerceiro.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().ToList(), "T_CTE_TERCEIRO");
                                    repCTeTerceiroQuantidade.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>().ToList(), "T_CTE_TERCEIRO_QUANTIDADE");
                                    repCTeTerceiroNFe.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>().ToList(), "T_CTE_TERCEIRO_NFE");
                                    repCTeTerceiroComponenteFrete.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>().ToList(), "T_CTE_TERCEIRO_COMPONENTE_FRETE");
                                    repClienteDescarga.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>().ToList(), "T_CLIENTE_DESCARGA");
                                    repPedidoCTeParaSubContratacao.Inserir(objetoValorPersistente.lstInsert.OfType<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>().Distinct().ToList(), "T_PEDIDO_CTE_PARA_SUB_CONTRATACAO");

                                    repParticipanteCTe.ComandSql(objetoValorPersistente.lstDelete.OfType<string>().Distinct().ToList());

                                    Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                                    List<string> comandos = new List<string>();
                                    int situacaoIntegracao = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                                    foreach (var upd in objetoValorPersistente.lstUpdate.OfType<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().Distinct().ToList())
                                        comandos.Add($"  UPDATE T_CTE_TERCEIRO SET CPS_IDENTIFICACAO_PACOTE = '{LogKey}' WHERE CPS_CODIGO = {upd.Codigo} "); 

                                    comandos.Add($" UPDATE T_PACOTE SET PCT_MENSAGEM_INTEGRACAO = 'Integrado passo2 com sucesso.', PCT_SITUACAO_INTEGRACAO = {situacaoIntegracao} WHERE PCT_CODIGO = {cargaPedidoPacote.Pacote.Codigo} ");
                                    comandos.Add($"  UPDATE T_CTE_TERCEIRO_XML SET CPS_CODIGO = {cteTerceiro.Codigo} WHERE CTX_CODIGO = {cargaPedidoPacote.Pacote.CTeTerceiroXML.Codigo} "); // SQL-INJECTION-SAFE
                                    repPacote.ComandSql(comandos);

                                    unitOfWork.CommitChanges();

                                    totalDocumentosAdicionadosDocumentoPrincipal = 0;
                                    cargaPedidoPacoteDocumentoPrincipal = cargaPedidoPacote;
                                    cteTerceiroQuantidade = repCTeTerceiroQuantidade.BuscarPorCTeTerceiroEUnidadeMedida(cteTerceiro.Codigo, Dominio.Enumeradores.UnidadeMedida.KG);
                                    cteTerceiroNFe = repCTeTerceiroNFe.BuscarPrimeiroPorCTeTerceiro(cteTerceiro.Codigo);
                                }
                                catch (Exception)
                                {
                                    unitOfWork.Rollback();
                                    throw;
                                }
                            }
                            else
                            {
                                // Se o número de tarefas simultâneas atingir o limite
                                if (lstTasks.Count >= 30)
                                {
                                    // Espera a conclusão de qualquer tarefa para liberar espaço para uma nova
                                    Task concluida = Task.WhenAny(lstTasks.ToArray()).Result;

                                    // Remove a tarefa concluída da lista
                                    lstTasks.Remove(concluida);
                                }

                                totalDocumentosAdicionadosDocumentoPrincipal++;
                                bool totalizarDocumentos = false;
                                if (cargaPedidoPacote.Codigo == ultimoPacote || totalDocumentosAdicionadosDocumentoPrincipal >= limiteDocumentoAdicionaisPorCTeTerceiro)
                                {
                                    totalizarDocumentos = true;

                                    //Aguarda finalizar threads anteriores para efetuar a totalização
                                    Task.WaitAll(lstTasks.ToArray());
                                }

                                Task newTask = new Task(() => { ProcessarIntegracaoPasso2DocumentoAdicional(cargaPedidoPacote.Codigo, cargaPedidoPacoteDocumentoPrincipal.Codigo, cteTerceiro.Codigo, cteTerceiroQuantidade.Codigo, cteTerceiroNFe.Codigo, totalizarDocumentos, _StringConexao); });
                                lstTasks.Add(newTask);
                                newTask.Start();

                                if (totalizarDocumentos)
                                {
                                    unitOfWork.Flush();
                                    cargaPedidoPacoteDocumentoPrincipal = null;
                                    cteTerceiro = null;
                                    cteTerceiroQuantidade = null;
                                    cteTerceiroNFe = null;
                                    totalDocumentosAdicionadosDocumentoPrincipal = 0;
                                    lstTasks = new List<Task>();
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    Task.WaitAll(lstTasks.ToArray());
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }
        }

        #region Integração via WebHook

        public void IntegrartPasso1ComPassa1WebHook(string arquivoPasso1, string _StringConexao)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    try
                    {
                        Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                        List<string> comandos = new List<string>();
                        comandos.Add($" UPDATE T_PACOTE  SET T_PACOTE.CTX_CODIGO = T_PACOTE_WEBHOOK.CTX_CODIGO from T_PACOTE inner join T_PACOTE_WEBHOOK ON PCT_LOG_KEY = PWT_LOG_KEY left join T_CTE_TERCEIRO_XML on T_PACOTE.CTX_CODIGO = T_PACOTE.CTX_CODIGO where(T_PACOTE.CTX_CODIGO is null) and(T_PACOTE.TPI_CODIGO is not null) and(T_PACOTE.PCT_SITUACAO_INTEGRACAO = 0 or T_PACOTE.PCT_SITUACAO_INTEGRACAO = 2) ");
                        repPacote.ComandSql(comandos);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e, arquivoPasso1);
                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, arquivoPasso1);
            }
        }

        public IntegracaoShopee StartPasso1WebHook(List<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook> pacotesWebHook, int threadsEmParalelo, string arquivoPasso1, Repositorio.UnitOfWork unitOfWork)
        {
            if (_integrarPacotes && pacotesWebHook.Count > 0)
            {
                Servicos.Log.TratarErro($"Iniciando processamento de {pacotesWebHook.Count} pacotes.", arquivoPasso1);
                int tamanhoDoLote = threadsEmParalelo;
                for (int i = 0; i < pacotesWebHook.Count; i += tamanhoDoLote)
                {
                    DateTime dt = DateTime.Now;
                    List<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook> _IdsPacote = pacotesWebHook.Skip(i).Take(tamanhoDoLote).ToList();
                    Servicos.Log.TratarErro($"Iniciando requisiçoes do lote {i}.", arquivoPasso1);
                    DateTime tempo = DateTime.Now;

                    List<System.Threading.Thread> lstThreds = new List<System.Threading.Thread>();
                    foreach (var pacoteWebHook in _IdsPacote)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(() => ResquestPacoteWebHook(pacoteWebHook));
                        lstThreds.Add(thread);
                        thread.Start();
                    }

                    while (lstThreds.Where(x => x.IsAlive == true).Count() > 0) { }

                    Servicos.Log.TratarErro($"Finalizou requisições do lote {i}. tamanho {_IdsPacote.Count}, tempo total {(DateTime.Now - tempo).TotalSeconds} segundos.  {_IdsPacote.Count / (DateTime.Now - tempo).TotalSeconds} pacotes por segundo ", arquivoPasso1);

                    dt = DateTime.Now;
                    Servicos.Log.TratarErro($"Iniciando persistencia do lote {i}.");
                    tempo = DateTime.Now;

                    Repositorio.Embarcador.Cargas.PacoteWebHook repPacoteWebHook = new Repositorio.Embarcador.Cargas.PacoteWebHook(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroXML repCTeTerceiroXML = new Repositorio.Embarcador.CTe.CTeTerceiroXML(unitOfWork);

                    unitOfWork.Start();

                    repCTeTerceiroXML.Inserir(_lstCTeTerceiroXMLParaPersistir, "T_CTE_TERCEIRO_XML");
                    foreach (var xml in _lstCTeTerceiroXMLParaPersistir)
                    {
                        xml.PacoteWebHook.CTeTerceiroXML = xml;
                        repPacoteWebHook.Atualizar(xml.PacoteWebHook);
                    }
                    unitOfWork.CommitChanges();
                    unitOfWork.FlushAndClear();
                    _lstCTeTerceiroXMLParaPersistir = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();

                    Servicos.Log.TratarErro($"Finalizou persistencia do lote {i}. tamanho {_IdsPacote.Count}, tempo total {(DateTime.Now - tempo).TotalSeconds} segundos.  {_IdsPacote.Count / (DateTime.Now - tempo).TotalSeconds} pacotes por segundo ", arquivoPasso1);
                }
            }

            if (pacotesWebHook.Count < 25)
                System.Threading.Thread.Sleep(_SleepFilaVazia);

            return this;
        }

        public IntegracaoShopee StartPasso2WebHook(List<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook> pacotesWebHook, int threadsEmParalelo, string stringConexao, string arquivoPAsso2, Repositorio.UnitOfWork unitOfWork)
        {
            if (_integrarPacotes && pacotesWebHook.Count > 0)
            {
                Repositorio.Embarcador.Cargas.PacoteWebHook repPacote = new Repositorio.Embarcador.Cargas.PacoteWebHook(unitOfWork);
                Servicos.Log.TratarErro($"Iniciando processamento cache de {pacotesWebHook.Count} pacotes Passo2. Vinclular CTE Terceiro e pacote.", arquivoPAsso2);
                int tamanhoDoLote = threadsEmParalelo;
                if (pacotesWebHook == null)
                    pacotesWebHook = repPacote.BuscarPacotesPendentesIntegracaoPasso2WebHook();

                #region PREPARA LISTAS  

                var lstCteParaImportar = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
                foreach (var pacoteWebHook in pacotesWebHook)
                {
                    try
                    {
                        lstCteParaImportar.Add(ConvertercteProcEmCTeTerceiro(MultiSoftware.CTe.Servicos.Leitura.Ler(pacoteWebHook.CTeTerceiroXML.XML.ToStream())));
                    }
                    catch (Exception)
                    {
                        List<string> comandos = new List<string>();
                        comandos.Add($" UPDATE T_PACOTE_WEBHOOK SET PWT_MENSAGEM_INTEGRACAO = 'Erro passo2 ConvertercteProcEmCTeTerceiro.', PWT_SITUACAO_INTEGRACAO = 3 WHERE PWT_CODIGO = {pacoteWebHook.Codigo} "); // SQL-INJECTION-SAFE
                        repPacote.ComandSql(comandos);
                    }
                }

                PreparaCachePasso2(lstCteParaImportar, null, arquivoPAsso2, unitOfWork);

                #endregion

                for (int i = 0; i < pacotesWebHook.Count; i += tamanhoDoLote)
                {
                    DateTime dt = DateTime.Now;
                    List<Dominio.Entidades.Embarcador.Cargas.PacoteWebHook> _pacotesWebHook = pacotesWebHook.Skip(i).Take(tamanhoDoLote).ToList();
                    Servicos.Log.TratarErro($"Iniciando CriarCTeTerceiro lote {i}.", arquivoPAsso2);
                    DateTime tempo = DateTime.Now;
                    List<System.Threading.Thread> lstThreds = new List<System.Threading.Thread>();

                    foreach (var pacoteWebHook in _pacotesWebHook)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(() => ProcessarIntegracaoPasso2(null, stringConexao, pacoteWebHook));
                        lstThreds.Add(thread);
                        thread.Start();
                        System.Threading.Thread.Sleep(2);
                    }

                    while (lstThreds.Where(x => x.IsAlive == true).Count() > 0) { }

                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Servicos.Log.TratarErro($"Finalizou CriarCTeTerceiro lote {i}. tamanho {_pacotesWebHook.Count}, tempo total {(DateTime.Now - tempo).TotalSeconds} segundos.  {_pacotesWebHook.Count / (DateTime.Now - tempo).TotalSeconds} pacotes por segundo ", arquivoPAsso2);
                }
            }

            if (pacotesWebHook.Count < 25)
                System.Threading.Thread.Sleep(_SleepFilaVazia);

            return this;
        }

        public string ProcessarIntegracaoPasso1WebHook(Dominio.Entidades.Embarcador.Cargas.PacoteWebHook pacoteWebHook, string xml)
        {
            Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML cteTerceiroXML = _servicoCTeSubContratacao.Passo1De2CriacaoCTeTerceiroXMLPorPacoteWebHook(pacoteWebHook, xml);
            if (cteTerceiroXML != null)
            {
                lock (_semaforolstCTeTerceiroXMLParaPersistir)
                {
                    _lstCTeTerceiroXMLParaPersistir.Add(cteTerceiroXML);
                }
                return "";
            }
            else
            {
                return "XML Não pode ser vazio";
            }
        }

        #endregion

        #endregion

        #region Metodos Privados

        private IntegracaoShopee GetToken()
        {
            IFlurlResponse response = (_url + "login")
                                       .WithHeader("accept", "application/json")
                                       .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                                       .PostUrlEncodedAsync(new
                                       {
                                           grant_type = "",
                                           username = _usuario,
                                           password = _password,
                                           scope = "",
                                           client_id = "",
                                           client_secret = ""
                                       }).Result;


            string jsonResponse = response.ResponseMessage.Content.ReadAsStringAsync().Result;

            dynamic retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(jsonResponse);

            if (!response.ResponseMessage.IsSuccessStatusCode)
                Servicos.Log.TratarErro($"Problema ao acessar o Token {response.StatusCode}");
            else
            {
                _access_token = retorno.access_token;
                Servicos.Log.TratarErro($"Token autorizado");
            }

            return this;
        }

        private void ResquestPacote(Dominio.Entidades.Embarcador.Cargas.Pacote pacote, int recursivo = 0)
        {
            var client = new RestClient(_url);
            var request = new RestRequest($"/recover-cte/{pacote.LogKey}/", Method.POST);
            request.AddHeader("accept", "application/xml");
            request.AddHeader("Authorization", $"Bearer {_access_token}");
            //RestResponse response = await client.ExecuteAsync(request);
            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                if (recursivo < 3 && response.Content.Contains("Invalid credentials"))
                {
                    lock (_semaforo)
                    {
                        GetToken();
                        recursivo++;
                        ResquestPacote(pacote, recursivo);
                    }
                }
                else
                {
                    if (pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    else
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    if (response.Content.Length == 0)
                        pacote.MensagemIntegracao = response.ErrorMessage;
                    else
                        pacote.MensagemIntegracao = response.Content.Substring(0, response.Content.Length > 100 ? 99 : response.Content.Length - 1).Replace('"', ' ').Replace('\\', ' ');
                }
            }
            else
            {
                string retorno = ProcessarIntegracaoPasso1(pacote, response.Content.ToString().Replace("'", ""));
                if (retorno == "")
                {
                    pacote.MensagemIntegracao = "Integrado passo1 com sucesso.";
                    pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                }
                else
                {
                    if (pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    else
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pacote.MensagemIntegracao = retorno;
                }
            }
        }

        private void ResquestPacotes(List<int> pacotes, string _StringConexao)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroXML repCTeTerceiroXML = new Repositorio.Embarcador.CTe.CTeTerceiroXML(unitOfWork);

                    foreach (int codigoPacote in pacotes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Pacote pacote = repPacote.BuscarPorCodigo(codigoPacote);

                        int tentativa = 0;

                        while (tentativa <= 3)
                        {
                            string urlWebService = null;
                            urlWebService = _url + $"recover-cte/{pacote.LogKey}/";

                            var request = urlWebService
                                            .WithHeader("User-Agent", "NomeDoNavegador")
                                            .WithHeader("Content-Type", "application/xml");

                            if (!string.IsNullOrWhiteSpace(_access_token))
                                request = request.WithHeader("Authorization", $"Bearer {_access_token}");

                            var response = request.PostAsync().Result;

                            string xmlRetorno = response.ResponseMessage.Content.ReadAsStringAsync().Result;

                            try
                            {
                                if (!response.ResponseMessage.IsSuccessStatusCode)
                                {
                                    if (tentativa < 3 && xmlRetorno.Contains("Invalid credentials"))
                                    {
                                        tentativa++;
                                        GetToken();
                                        continue;
                                    }
                                    else
                                    {
                                        if (pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                                            pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                                        else
                                            pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                                        if (xmlRetorno.Length == 0)
                                            pacote.MensagemIntegracao = ($"Falha StatusCode: {response.ResponseMessage.StatusCode}");
                                        else
                                            pacote.MensagemIntegracao = xmlRetorno.Substring(0, xmlRetorno.Length > 100 ? 99 : xmlRetorno.Length - 1).Replace('"', ' ').Replace('\\', ' ');
                                    }
                                }
                                else
                                {
                                    string xml = xmlRetorno.ToString().Replace("'", "");

                                    if (!string.IsNullOrEmpty(xml))
                                    {
                                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML cteTerceiroXML = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML();
                                        cteTerceiroXML.XML = xml;
                                        cteTerceiroXML.Pacote = pacote;
                                        repCTeTerceiroXML.Inserir(cteTerceiroXML);

                                        pacote.MensagemIntegracao = "Integrado passo1 com sucesso.";
                                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                                        pacote.CTeTerceiroXML = cteTerceiroXML;
                                    }
                                    else
                                    {
                                        if (pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                                            pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                                        else
                                            pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                        pacote.MensagemIntegracao = "XML Não pode ser vazio";
                                    }
                                }

                                repPacote.Atualizar(pacote);
                            }
                            catch (Exception)
                            {
                                throw;
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }
        }

        private void ResquestPacoteWebHook(Dominio.Entidades.Embarcador.Cargas.PacoteWebHook pacote, int recursivo = 0)
        {
            var client = new RestClient(_url);
            var request = new RestRequest($"/recover-cte/{pacote.LogKey}/", Method.POST);
            request.AddHeader("accept", "application/xml");
            request.AddHeader("Authorization", $"Bearer {_access_token}");
            //RestResponse response = await client.ExecuteAsync(request);
            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                if (recursivo < 3 && response.Content.Contains("Invalid credentials"))
                {
                    lock (_semaforo)
                    {
                        GetToken();
                        recursivo++;
                        ResquestPacoteWebHook(pacote, recursivo);
                    }
                }
                else
                {
                    if (pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    else
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    if (response.Content.Length == 0)
                        pacote.MensagemIntegracao = response.ErrorMessage;
                    else
                        pacote.MensagemIntegracao = response.Content.Substring(0, response.Content.Length > 100 ? 99 : response.Content.Length - 1).Replace('"', ' ').Replace('\\', ' ');
                }
            }
            else
            {
                string retorno = ProcessarIntegracaoPasso1WebHook(pacote, response.Content.ToString().Replace("'", ""));
                if (retorno == "")
                {
                    pacote.MensagemIntegracao = "Integrado passo1 WebHook com sucesso.";
                    pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                }
                else
                {
                    if (pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    else
                        pacote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pacote.MensagemIntegracao = retorno;
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConvertercteProcEmCTeTerceiro(dynamic objCTe)
        {
            Type tipoCTe = objCTe.GetType();
            if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc) ||
                tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc) ||
                tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
            {
                return _serCte.ConverterProcCTeParaCTePorObjeto(objCTe);
            }
            else
            {
                return null;
            }
        }

        private void PreparaCachePasso2(List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> lstObjetosDeValorCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes, string arquivoPAsso2, Repositorio.UnitOfWork unitOfWork)
        {
            _cacheObjetoValorCTeFull = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
            _cacheObjetoValorCTeFull.CacheAtivo = true;

            Servicos.Log.TratarErro($"Inicio Preparando cache GrupoPessoas.", arquivoPAsso2);

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            _cacheObjetoValorCTeFull.lstGrupoPessoas = repGrupoPessoas.BuscarTodos();

            Servicos.Log.TratarErro($"Fim GrupoPessoas. Inicio CFOP ", arquivoPAsso2);

            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            _cacheObjetoValorCTeFull.lstCFOP = repCFOP.BuscarTodos();

            Servicos.Log.TratarErro($"Fim CFOP. Inicio Banco.", arquivoPAsso2);

            List<string> chavesAcesso = lstObjetosDeValorCTe.Select(x => x.Chave).ToList();
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            _cacheObjetoValorCTeFull.lstBancos = repBanco.BuscarTodos();

            Servicos.Log.TratarErro($"Fim Banco. Inicio ConfiguracaoTMS.", arquivoPAsso2);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            _cacheObjetoValorCTeFull.ConfiguracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();

            Servicos.Log.TratarErro($"Fim ConfiguracaoTMS. Inicio ConfiguracaoWebService.", arquivoPAsso2);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            _cacheObjetoValorCTeFull.configWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            Servicos.Log.TratarErro($"Fim ConfiguracaoWebService. Inicio Localidade.", arquivoPAsso2);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            _cacheObjetoValorCTeFull.lstLocalidades = repLocalidade.BuscarTodosComRegiao();

            Servicos.Log.TratarErro($"Fim Localidade. Inicio Pais.", arquivoPAsso2);

            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
            _cacheObjetoValorCTeFull.lstPais = repPais.BuscarTodos();

            Servicos.Log.TratarErro($"Fim Pais Inicio Regiao.", arquivoPAsso2);
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            _cacheObjetoValorCTeFull.lstRegiao = repRegiao.BuscarTodos();

            Servicos.Log.TratarErro($"Fim Regiao Inicio RateioFormula.", arquivoPAsso2);
            Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            _cacheObjetoValorCTeFull.lstRateioFormula = repRateioFormula.BuscarTodos();

            Servicos.Log.TratarErro($"Fim RateioFormula. Inicio TipoIntegracao.", arquivoPAsso2);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            _cacheObjetoValorCTeFull.lstTiposIntegracao = repTipoIntegracao.BuscarAtivos();

            Servicos.Log.TratarErro($"Fim Tipo Integracao. Inicio Atividade.", arquivoPAsso2);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            _cacheObjetoValorCTeFull.lstAtividade = repAtividade.BuscarTodos();

            Servicos.Log.TratarErro($"Fim Atividade. Inicio Empresa.", arquivoPAsso2);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            _cacheObjetoValorCTeFull.lstEmpresa = repEmpresa.BuscarTodos();

            Servicos.Log.TratarErro($"Fim Empresa. Inicio CategoriaPessoa.", arquivoPAsso2);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
            _cacheObjetoValorCTeFull.lstCategoriaPessoa = repCategoriaPessoa.BuscarTodos();

            Servicos.Log.TratarErro($"Fim CategoriaPessoas Inicio ModalidadePessoas.", arquivoPAsso2);
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            _cacheObjetoValorCTeFull.lstModalidadePessoas = repModalidadePessoas.BuscarTodos();

            Servicos.Log.TratarErro($"Fim ModalidadePessoas. Inicio ModalidadeClientePessoas.", arquivoPAsso2);
            Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
            _cacheObjetoValorCTeFull.lstModalidadeClientePessoas = repModalidadeClientePessoas.BuscarTodos();

            Servicos.Log.TratarErro($"Fim ModalidadeClientePessoas. Inicio ModalidadeFornecedorPessoas.", arquivoPAsso2);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            _cacheObjetoValorCTeFull.lstModalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarTodos();

            Servicos.Log.TratarErro($"Fim ModalidadeFornecedorPessoas. Inicio ModalidadeTransportadoraPessoas.", arquivoPAsso2);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            _cacheObjetoValorCTeFull.lstModalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarTodos();

            Servicos.Log.TratarErro($"Fim ModalidadeTransportadoraPessoas. Fim ClienteDescarga.", arquivoPAsso2);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            _cacheObjetoValorCTeFull.lstCodigosClienteDescarga = repClienteDescarga.BuscarTodosCodigos();

            Servicos.Log.TratarErro($"Fim ClienteDescarga. Inicio Cliente.", arquivoPAsso2);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            _cacheObjetoValorCTeFull.lstCacheIndexClientes = repCliente.BuscarClienteIndex(2);

            Servicos.Log.TratarErro($"Fim Cliente. Inicio CTeTerceiro.", arquivoPAsso2);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            _cacheObjetoValorCTeFull.LstCtesTerceiro = repCTeTerceiro.BuscarPorChave(chavesAcesso);

            Servicos.Log.TratarErro($"Fim CTeTerceiro. Inicio CTeTerceiroQuantidade.", arquivoPAsso2);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            _cacheObjetoValorCTeFull.lstCTeTerceiroQuantidade = repCTeTerceiroQuantidade.BuscarPorCTeParaSubContratacao(_cacheObjetoValorCTeFull.LstCtesTerceiro.Select(x => x.Codigo).ToList());

            //Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);
            //List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes = repositorioCargaPedidoPacote.BuscarCargaPedidoPacoteLoggiKey(codigosPacotes, cargaPedido);
            //Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);
            //List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiros = repCTeTerceiro.BuscarPorIdentificacaoPacote(codigosPacotes);
            Servicos.Log.TratarErro($"Fim CTeTerceiroQuantidade. Inicio PedidoCTeParaSubContratacao.", arquivoPAsso2);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            _cacheObjetoValorCTeFull.pedidoCTesParaSubContratacao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            if (cargaPedidoPacotes != null)
            {
                foreach (var cargaPedido in cargaPedidoPacotes.Select(x => x.CargaPedido).Distinct())
                {
                    _cacheObjetoValorCTeFull.pedidoCTesParaSubContratacao.AddRange(repPedidoCTeParaSubcontratacao.BuscarPorCTeSubContratacaoECargaPedido(_cacheObjetoValorCTeFull.LstCtesTerceiro, cargaPedido));
                }
            }

            Servicos.Log.TratarErro($"Fim PedidoCTeParaSubContratacao. Inicio Montagem de listas .", arquivoPAsso2);

            var listaDestinatarios = lstObjetosDeValorCTe.Where(x => x.Destinatario.CodigoIntegracao != null && x.Destinatario.CodigoIntegracao != "").Select(x => x.Destinatario.CodigoIntegracao).ToList();
            var listaRemetentes = lstObjetosDeValorCTe.Where(x => x.Remetente.CodigoIntegracao != null && x.Remetente.CodigoIntegracao != "").Select(x => x.Remetente.CodigoIntegracao).ToList();
            var listaCombinada = listaDestinatarios.Union(listaRemetentes).Distinct().ToList();
            var listaCPFCNPJDestinatarios = lstObjetosDeValorCTe.Where(x => x.Remetente.CPFCNPJ != null && x.Remetente.CPFCNPJ != "").Select(x => x.Destinatario.CPFCNPJ).ToList();
            var listaCPFCNPJRemetentes = lstObjetosDeValorCTe.Where(x => x.Remetente.CPFCNPJ != null && x.Remetente.CPFCNPJ != "").Select(x => x.Remetente.CPFCNPJ).ToList();
            var listaCPFCNPJCombinada = listaCPFCNPJDestinatarios.Union(listaCPFCNPJRemetentes).Distinct().ToList();

            _cacheObjetoValorCTeFull.lstCNPJsCPFs = new List<double>();

            foreach (var _cpfCnpj in listaCPFCNPJCombinada)
            {
                double cpfCnpj = 0f;
                double.TryParse(_cpfCnpj, out cpfCnpj);
                _cacheObjetoValorCTeFull.lstCNPJsCPFs.Add(cpfCnpj);
            }

            _cacheObjetoValorCTeFull.lstCNPJsCPFs = _cacheObjetoValorCTeFull.lstCNPJsCPFs.Distinct().ToList();
            _cacheObjetoValorCTeFull.lstClientes = repCliente.BuscarPorCNPJsCPFs(_cacheObjetoValorCTeFull.lstCNPJsCPFs);
            _cacheObjetoValorCTeFull.lstClientes = _cacheObjetoValorCTeFull.lstClientes.Union(repCliente.BuscarPorCodigosIntegracao(listaCombinada)).Distinct().ToList();

            Servicos.Log.TratarErro($"Fim Montagem de listas.", arquivoPAsso2);

            //var listaGrupoPessoaDestinatarios = lstObjetosDeValorCTe.Select(x => x.Destinatario.GrupoPessoa.CodigoIntegracao).ToList();
            //var listaGrupoPessoaRemetentes = lstObjetosDeValorCTe.Select(x => x.Destinatario.GrupoPessoa.CodigoIntegracao).ToList();
            //var listaGrupoPessoaCombinada = listaGrupoPessoaDestinatarios.Union(listaGrupoPessoaRemetentes).Distinct().ToList();

            //cacheObjetoValorCTeFull.lstGrupoPessoas = repGrupoPessoa.BuscarPorCodigoIntegracao(grupoPessoa.CodigoIntegracao);
            //Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            //Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            //Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoCliente = repGrupoPessoa.BuscarPorCodigoIntegracao(grupoPessoa.CodigoIntegracao);

            //    _cacheObjetoValorCTeFull.PersistenciaInsertCliente = new List<Dominio.Entidades.Cliente>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertRegiao = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertOutroEndereco = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertEnderecos = new List<Dominio.Entidades.Localidade>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertClienteOutroEndereco = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertModalidadePessoas = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertModalidadeClientePessoas = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertModalidadeFornecedorPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertModalidadeTransportadoraPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertClienteDescarga = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertCTeTerceiro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertCTeTerceiroQuantidade = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertCTeTerceiroSeguro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertCTeTerceiroOutrosDocumentos = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertCTeTerceiroNotaFiscal = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNotaFiscal>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertCTeTerceiroNFe = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertCTeTerceiroComponenteFrete = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete>();
            //    _cacheObjetoValorCTeFull.PersistenciaDeletarCTeTerceiro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
            //    _cacheObjetoValorCTeFull.PersistenciaInsertParticipanteCTe = new List<Dominio.Entidades.ParticipanteCTe>();
            //    _cacheObjetoValorCTeFull.PersistenciaUpdateCliente = new List<Dominio.Entidades.Cliente>();
            //}
        }

        private HttpClient CriarRequisicao(string url, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoShopee));
            requisicao.DefaultRequestHeaders.Add("User-Agent", "NomeDoNavegador");
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            if (!string.IsNullOrWhiteSpace(token))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return requisicao;
        }

        private void ProcessarIntegracaoPasso2DocumentoAdicional(int codigoCargaPedidoPacote, int codigoCargaPedidoPacoteDocumentoPrincipal, int codigoCteTerceiro, int codigoCteTerceiroQuantidade, int codigoCteTerceiroNFe, bool totalizarDocumentos, string _StringConexao)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);
                    Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);
                    Servicos.Cliente servicoCliente = new Servicos.Cliente();

                    Dominio.Entidades.Cliente emitente = null;
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteProc = null;
                    var cargaPedidoPacote = repCargaPedidoPacote.BuscarPorCodigo(codigoCargaPedidoPacote);
                    var cteTerceiro = repCTeTerceiro.BuscarPorCodigo(codigoCteTerceiro);
                    var cteTerceiroQuantidade = repCTeTerceiroQuantidade.BuscarPorCodigo(codigoCteTerceiroQuantidade);

                    cteProc = ConvertercteProcEmCTeTerceiro(MultiSoftware.CTe.Servicos.Leitura.Ler(cargaPedidoPacote.Pacote.CTeTerceiroXML.XML.ToStream()));
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoEmissor = servicoCliente.ConverterParaTransportadorTerceiro(cteProc.Emitente, "emitente do CT-e", unitOfWork);

                    if (retornoConversaoEmissor.Status)
                        emitente = retornoConversaoEmissor.cliente;
                    else
                    {
                        throw new Exception(retornoConversaoEmissor.Mensagem);
                    }

                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional documentoAdicional = repCTeTerceiroDocumentoAdicional.BuscarPorCTeTerceiroEChaveAcesso(cteTerceiro.Codigo, cteProc.Chave);

                    unitOfWork.Start();

                    try
                    {
                        if (documentoAdicional == null)
                        {
                            string LogKey = cargaPedidoPacote.Pacote.LogKey;
                            string medida = cteTerceiroQuantidade.TipoMedida;

                            documentoAdicional = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional();
                            documentoAdicional.Chave = cteProc.Chave;
                            documentoAdicional.CTeTerceiro = cteTerceiro;
                            documentoAdicional.Numero = cteProc.Numero;
                            documentoAdicional.Emitente = emitente;
                            documentoAdicional.ValorTotalMercadoria = cteProc.InformacaoCarga?.ValorTotalCarga ?? 0m;
                            documentoAdicional.Quantidade = cteProc.QuantidadesCarga?.Where(o => o.Medida == medida).Sum(o => o.Quantidade) ?? 0m;

                            repCTeTerceiroDocumentoAdicional.Inserir(documentoAdicional);

                            List<string> comandos = new List<string>();
                            int situacaoIntegracao = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            comandos.Add($" UPDATE T_PACOTE SET PCT_MENSAGEM_INTEGRACAO = 'Integrado passo2 com sucesso.', PCT_SITUACAO_INTEGRACAO = {situacaoIntegracao} WHERE PCT_CODIGO = {cargaPedidoPacote.Pacote.Codigo} ");
                            comandos.Add($"  UPDATE T_CTE_TERCEIRO_XML SET CPS_CODIGO = {cteTerceiro.Codigo} WHERE CTX_CODIGO = {cargaPedidoPacote.Pacote.CTeTerceiroXML.Codigo} "); // SQL-INJECTION-SAFE
                            repPacote.ComandSql(comandos);
                        }

                        // Totalizar Documentos
                        if (totalizarDocumentos)
                        {
                            var cteTerceiroNFe = repCTeTerceiroNFe.BuscarPorCodigo(codigoCteTerceiroNFe);
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiroAtualizar = repCTeTerceiro.BuscarPorCodigo(cteTerceiro.Codigo);
                            cteTerceiroAtualizar.ValorTotalMercadoria = cteTerceiroAtualizar.ValorTotalMercadoriaOriginal + repCTeTerceiroDocumentoAdicional.BuscarTotalValorMercadoria(cteTerceiroAtualizar.Codigo);
                            repCTeTerceiro.Atualizar(cteTerceiroAtualizar);

                            Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFeAtualizar = repCTeTerceiroNFe.BuscarPorCodigo(cteTerceiroNFe.Codigo);
                            if (cteTerceiroNFeAtualizar != null)
                            {
                                cteTerceiroNFeAtualizar.ValorTotal = cteTerceiroAtualizar.ValorTotalMercadoria;
                                repCTeTerceiroNFe.Atualizar(cteTerceiroNFeAtualizar);
                            }

                            Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidadeAtualizar = repCTeTerceiroQuantidade.BuscarPorCodigo(cteTerceiroQuantidade.Codigo);
                            if (cteTerceiroQuantidadeAtualizar != null)
                            {
                                string medida = cteTerceiroQuantidadeAtualizar.TipoMedida;
                                cteTerceiroQuantidadeAtualizar.Quantidade = cteTerceiroQuantidadeAtualizar.QuantidadeOriginal + repCTeTerceiroDocumentoAdicional.BuscarTotalQuantidade(cteTerceiroAtualizar.Codigo);
                                repCTeTerceiroQuantidade.Atualizar(cteTerceiroQuantidadeAtualizar);
                            }

                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacoteDocumentoPrincipal = repCargaPedidoPacote.BuscarPorCodigo(codigoCargaPedidoPacoteDocumentoPrincipal);
                            cargaPedidoPacoteDocumentoPrincipal.TotalDocumentosAdicionais = repCTeTerceiroDocumentoAdicional.BuscarTotalDocumentoAdicionalPorTerceiro(cteTerceiro.Codigo);
                            repCargaPedidoPacote.Atualizar(cargaPedidoPacoteDocumentoPrincipal);
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }
        }

        #endregion

    }
}

/*
 querys de verificação de pendencia

select PCT_MENSAGEM_INTEGRACAO ,pacote0_.* from T_PACOTE pacote0_ 
left outer join T_CTE_TERCEIRO_XML cteterceir1_  on pacote0_.CTX_CODIGO=cteterceir1_.CTX_CODIGO 
where ( pacote0_.CTX_CODIGO is null) 
and (pacote0_.TPI_CODIGO is not null) 
and (pacote0_.PCT_SITUACAO_INTEGRACAO=0 or pacote0_.PCT_SITUACAO_INTEGRACAO=2)

select pacote3_.PCT_MENSAGEM_INTEGRACAO ,cargapedid0_.* from T_CARGA_PEDIDO_PACOTE cargapedid0_ 
inner join T_PACOTE pacote1_  on cargapedid0_.PCT_CODIGO=pacote1_.PCT_CODIGO 
inner join T_CTE_TERCEIRO_XML cteterceir2_ on pacote1_.CTX_CODIGO=cteterceir2_.CTX_CODIGO 
left outer join T_PACOTE pacote3_ on cargapedid0_.PCT_CODIGO=pacote3_.PCT_CODIGO 
left outer join T_CTE_TERCEIRO_XML cteterceir4_ on pacote3_.CTX_CODIGO=cteterceir4_.CTX_CODIGO 
where ( pacote1_.CTX_CODIGO is not null ) 
and ( cteterceir2_.CPS_CODIGO is null) 
and ( pacote1_.TPI_CODIGO is not null )  and (pacote1_.PCT_SITUACAO_INTEGRACAO=0 
or pacote1_.PCT_SITUACAO_INTEGRACAO=2);
    
// estudar inclusao deste cara na query de cima left join T_CTE_TERCEIRO CTeTerceiro on CTeTerceiro.CPS_CODIGO = cteterceir2_.CPS_CODIGO 


// select da tela de carga 
SELECT COUNT(CTeTerceiro.CPS_CODIGO) FROM T_CTE_TERCEIRO CTeTerceiro                            
LEFT JOIN T_PACOTE Pacote ON Pacote.PCT_LOG_KEY = CTeTerceiro.CPS_IDENTIFICACAO_PACOTE                            
LEFT JOIN T_CARGA_PEDIDO_PACOTE CargaPedidoPacote ON Pacote.PCT_CODIGO = CargaPedidoPacote.PCT_CODIGO                           
LEFT JOIN T_CARGA_PEDIDO CargaPedido on CargaPedidoPacote.CPE_CODIGO = CargaPedido.CPE_CODIGO                           
WHERE CargaPedido.car_codigo = 110543




UPDATE T_PACOTE SET TPI_CODIGO = 192 , PCT_SITUACAO_INTEGRACAO= 0 , CTX_CODIGO = null
WHERE PCT_CODIGO IN ( select P.PCT_CODIGO from T_CARGA_PEDIDO_PACOTE CPP
INNER JOIN T_CARGA_PEDIDO CP ON  CP.CPE_CODIGO = CPP.CPE_CODIGO 
INNER JOIN T_CARGA C ON CP.CAR_CODIGO = C.CAR_CODIGO 
INNER JOIN T_PACOTE P ON P.PCT_CODIGO = CPP.PCT_CODIGO
WHERE C.CAR_CODIGO = 1677)


delete from  T_CTE_TERCEIRO_XML where PCT_CODIGO in (
select P.PCT_CODIGO from T_CARGA_PEDIDO_PACOTE CPP
INNER JOIN T_CARGA_PEDIDO CP ON  CP.CPE_CODIGO = CPP.CPE_CODIGO 
INNER JOIN T_CARGA C ON CP.CAR_CODIGO = C.CAR_CODIGO 
INNER JOIN T_PACOTE P ON P.PCT_CODIGO = CPP.PCT_CODIGO
WHERE C.CAR_CODIGO = 1677)
 
 
 
 */