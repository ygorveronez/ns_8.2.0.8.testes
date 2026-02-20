using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Script.Serialization;



namespace EmissaoCTe.API
{

    public class GerarCTesEmails
    {
        private int Tempo = 60000; //1 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaConsultaEmailCaixaEntrada;
        private static GerarCTesEmails Instance;

        public static GerarCTesEmails GetInstance()
        {
            if (Instance == null)
                Instance = new GerarCTesEmails();

            return Instance;
        }

        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaConsultaEmailCaixaEntrada == null)
                ListaConsultaEmailCaixaEntrada = new ConcurrentQueue<int>();

            if (!ListaTasks.ContainsKey(idEmpresa))
            {
                this.IniciarThread(idEmpresa, stringConexao);
            }
        }

        private void IniciarThread(int idEmpresa, string stringConexao)
        {
            var filaConsulta = new ConcurrentQueue<int>();

            filaConsulta.Enqueue(idEmpresa);

            Task task = new Task(() =>
            {
#if DEBUG
                System.Threading.Thread.Sleep(6666);
                Tempo = 500;
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            verificarEmails(unidadeDeTrabalho, stringConexao);
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de consulta de objetos cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de consulta de objetos cancelada: ", abortThread));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        System.Threading.Thread.Sleep(Tempo);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task à fila.");
        }

        private void verificarEmails(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Email.EmailCaixaEntrada repEmailCaixaEntrada = new Repositorio.Embarcador.Email.EmailCaixaEntrada(unitOfWork);
            List<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada> emails = repEmailCaixaEntrada.BuscarPorTipoServico(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, 0, 200);

            if (emails != null && emails.Count == 0)
                Tempo = 120000; //2 MINUTOS 
            else
                Tempo = 500;
#if DEBUG
            Tempo = 500;
#endif

            foreach (Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada email in emails)
            {
                bool reimportar = false;
                if (email.Anexos.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Email.EmailAnexos anexo in email.Anexos)
                    {
                        string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                        string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "Entrada");
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidNomeArquivo + extensao);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            if (anexo.ArquivoZipado)
                            {
                                //Servicos.Log.TratarErro("O arquivo " + anexo.NomeArquivo + " está compactado, por favor envie os arquivos descompactados ou compactados na extensão .zip .");
                                //enviarEmailProblemaCTe("infra@multisoftware.com.br", "O arquivo " + anexo.NomeArquivo + " está compactado, por favor envie os arquivos descompactados ou compactados na extensão .zip .", stringConexao);
                                this.LogErro("O arquivo está compactado, por favor envie os arquivos descompactados ou compactados na extensão .zip .", "", anexo.NomeArquivo, 0, stringConexao, false);
                            }
                            else
                            {
                                if (extensao == ".xml")
                                {
                                    System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation));
                                    try
                                    {
                                        reimportar = false;
                                        processarXML(memoryStream, email, ref reimportar, unitOfWork, stringConexao);
                                        memoryStream.Dispose();
                                        memoryStream.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        memoryStream.Dispose();
                                        memoryStream.Close();
                                        //Servicos.Log.TratarErro("Email assunto " + email.Assunto + ". Xml enviado " + anexo.NomeArquivo + " não importado: " + ex);
                                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Email assunto " + email.Assunto + ". Xml enviado " + anexo.NomeArquivo + " não importado: " + ex, stringConexao);
                                        this.LogErro("Email assunto " + email.Assunto + ". Xml enviado não importado: " + ex, "", anexo.NomeArquivo, 0, stringConexao, false);
                                    }
                                }
                                else if (extensao == ".csv")
                                {
                                    System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation));
                                    try
                                    {
                                        processarCSV(memoryStream, anexo.NomeArquivo, email, unitOfWork, stringConexao);
                                        memoryStream.Dispose();
                                        memoryStream.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        memoryStream.Dispose();
                                        memoryStream.Close();
                                        //Servicos.Log.TratarErro("Email assunto " + email.Assunto + ". CSV enviado " + anexo.NomeArquivo + " não importado: " + ex);
                                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Email assunto " + email.Assunto + ". CSV enviado " + anexo.NomeArquivo + " não importado: " + ex, stringConexao);
                                        this.LogErro("Email assunto " + email.Assunto + ". CSV enviado não importado: " + ex, "", anexo.NomeArquivo, 0, stringConexao, false);
                                    }
                                }
                            }
                            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                            {
                                Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                            }
                        }

                    }
                    if (!reimportar)
                        repEmailCaixaEntrada.Deletar(email);
                }
                else
                {
                    if (!reimportar)
                        repEmailCaixaEntrada.Deletar(email);
                }
            }

            this.EmitirCTesAguardandoEmissao(unitOfWork);
        }

        private void processarCSV(System.IO.Stream csv, string nomeArquivo, Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada email, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            #region Mapeamento
            var mapeamento = new
            {
                NumeroNFe = 0,
                ValorNFe = 1,
                PesoBruto = 11,
                ChaveNFe = 13,

                Destinatario = 2,
                Endereco = 4,
                Complemento = 5,
                Bairro = 6,
                Municipio = 7,
                UF = 8,
                CEP = 9,
                Telefone = 10,
                CNPJCPFDestinatario = 12,
                IEDestinatario = 23,

                CNPJEmitenteNFe = 14,
                IEEmitenteNFe = 15,
                LogradouroEmitenteNFe = 16,
                BairroEmitenteNFe = 17,
                CEPEmitenteNFe = 18,
                IBGEEmitenteNFe = 19,

                IBGERemetente = 20,
                IBGEDestinatario = 21,

                ValorFrete = 22,
                Observacao = 3,

                CNPJTransportadora = 24,
                CodigoCalculoFrete = 25
            };
            #endregion

            #region Repositorios/Entidades
            // Repositorios e servicos
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
            Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);

            List<Dominio.ObjetosDeValor.CTe.CTe> ctesIntegracao = new List<Dominio.ObjetosDeValor.CTe.CTe>() { };
            #endregion

            // Numero de colunas esperada
            int numeroColunasObrigatorio = 26;

            // Converte arquivo
            List<List<string>> linhas = this.ConverteArquivo(csv);
            int index = 0;

            // Cultura
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            // Controle de debug
            bool DEBUGARSUCESSO = false;

            // Log de inicio
            if (DEBUGARSUCESSO)
                this.LogErro("Iniciado o processo de emissão por CSV. Total de linhas do arquivo: " + linhas.Count().ToString(), null, nomeArquivo, null, stringConexao, false);

            foreach (List<string> linha in linhas)
            {
                // Valida quantia de colunas
                if (linha.Count() != numeroColunasObrigatorio)
                {
                    this.LogErro("O número de colunas da linha é inválido. Esperado " + numeroColunasObrigatorio.ToString() + " coluna(s) e Recebido " + linha.Count().ToString() + " coluna(s).", null, nomeArquivo, index, stringConexao, true);
                    continue;
                }

                // Transportadora
                string cnpjTransportadora = linha[mapeamento.CNPJTransportadora];
                Dominio.Entidades.Empresa transportadora = repEmpresa.BuscarPorCNPJ(cnpjTransportadora);

                // Codigo Calculo Frete
                string codigoCalculoFrete = linha[mapeamento.CodigoCalculoFrete];

                if (transportadora != null)
                {
                    #region IBGE
                    int IBGERemetente, IBGEDestinatario, IBGEEmitenteNFe = 0;

                    int.TryParse(linha[mapeamento.IBGERemetente], out IBGERemetente);
                    int.TryParse(linha[mapeamento.IBGEDestinatario], out IBGEDestinatario);
                    int.TryParse(linha[mapeamento.IBGEEmitenteNFe], out IBGEEmitenteNFe);

                    // Verifica se localidade existem
                    if (
                        repLocalidade.BuscarPorCodigoIBGE(IBGERemetente) == null ||
                        repLocalidade.BuscarPorCodigoIBGE(IBGEDestinatario) == null ||
                        repLocalidade.BuscarPorCodigoIBGE(IBGEEmitenteNFe) == null
                    )
                    {
                        // Buscar por UF/Cidade quando nao encontra...
                        string cidade = "";
                        string estado = "";
                        string cep = "";

                        // Destinatario 
                        cidade = linha[mapeamento.Municipio];
                        estado = linha[mapeamento.UF];
                        cep = linha[mapeamento.CEP];

                        // Por cidade estado
                        Dominio.Entidades.Localidade localidadeDestinatario = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(cidade), estado);

                        if (localidadeDestinatario != null)
                            IBGEDestinatario = localidadeDestinatario.CodigoIBGE;
                        else
                        {
                            // Se nao encontrar, tenta buscar por CEP
                            localidadeDestinatario = repLocalidade.BuscarPorCEP(Utilidades.String.OnlyNumbers(cep.Substring(0, 3)));

                            if (localidadeDestinatario != null)
                            {
                                IBGEDestinatario = localidadeDestinatario.CodigoIBGE;
                            }
                            else
                            {
                                this.LogErro("IBGE Remetente ou IBGE Destinatário ou IBGE Emitente NF-e é inválido.", transportadora.NomeFantasia, nomeArquivo, index, stringConexao, false);
                                continue;
                            }

                        }
                    }
                    #endregion

                    #region Documento
                    decimal valorDocumento = 0, pesoDocumento = 0;

                    try
                    {
                        valorDocumento = decimal.Parse(linha[mapeamento.ValorNFe], cultura);
                    }
                    catch (Exception e)
                    {
                        this.LogErro("Erro ao converter Valor NF-e (" + linha[mapeamento.ValorNFe] + ").", transportadora.NomeFantasia, nomeArquivo, index, stringConexao, false);
                    }
                    try
                    {
                        pesoDocumento = decimal.Parse(linha[mapeamento.PesoBruto], cultura);
                    }
                    catch (Exception e)
                    {
                        this.LogErro("Erro ao converter Peso Bruto (" + linha[mapeamento.PesoBruto] + ").", transportadora.NomeFantasia, nomeArquivo, index, stringConexao, false);
                    }

                    Dominio.ObjetosDeValor.CTe.Documento documento = new Dominio.ObjetosDeValor.CTe.Documento()
                    {
                        Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NFe,
                        Numero = linha[mapeamento.NumeroNFe],
                        Serie = this.SerieDaChave(linha[mapeamento.ChaveNFe]),
                        Valor = valorDocumento,
                        DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        Peso = pesoDocumento,
                        ChaveNFE = linha[mapeamento.ChaveNFe]
                    };
                    #endregion

                    #region Clientes
                    // Emitente
                    Dominio.ObjetosDeValor.CTe.Empresa emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
                    {
                        CNPJ = linha[mapeamento.CNPJTransportadora],
                        Atualizar = false
                    };



                    #region Destinatario
                    // Destinatario
                    Dominio.ObjetosDeValor.CTe.Cliente destinatario = new Dominio.ObjetosDeValor.CTe.Cliente()
                    {
                        CPFCNPJ = linha[mapeamento.CNPJCPFDestinatario],
                        RazaoSocial = linha[mapeamento.Destinatario],
                        Endereco = Utilidades.String.RemoveDiacritics(linha[mapeamento.Endereco]),
                        Numero = "S/N",
                        Complemento = linha[mapeamento.Complemento],
                        Bairro = linha[mapeamento.Bairro],
                        Cidade = linha[mapeamento.Municipio],
                        CodigoIBGECidade = IBGEDestinatario,
                        CEP = linha[mapeamento.CEP],
                        Telefone1 = linha[mapeamento.Telefone],
                        RGIE = linha[mapeamento.IEDestinatario],
                        NaoAtualizarEndereco = true
                    };
                    // Atividade
                    string tipoCPFCNPJDestinatario = destinatario.CPFCNPJ.Length == 14 ? "J" : "F";
                    destinatario.CodigoAtividade = Servicos.Atividade.ObterAtividade(transportadora.Codigo, tipoCPFCNPJDestinatario, stringConexao, 0, unitOfWork).Codigo;
                    if (string.IsNullOrWhiteSpace(destinatario.RGIE)) destinatario.RGIE = "ISENTO";
                    #endregion



                    #region Remetente
                    // Verifica se o remetente esta cadastrado
                    Dominio.ObjetosDeValor.CTe.Cliente remetente;
                    double CPFCNPJremetente = 0;
                    double.TryParse(linha[mapeamento.CNPJEmitenteNFe], out CPFCNPJremetente);
                    Dominio.Entidades.Cliente entRemetente = repCliente.BuscarPorCPFCNPJ(CPFCNPJremetente);
                    if (entRemetente == null)
                    {
                        this.LogErro("Remetente (" + CPFCNPJremetente + ") não está cadastrado", transportadora.NomeFantasia, nomeArquivo, index, stringConexao, false);
                        continue;
                    }
                    else
                    {
                        remetente = new Dominio.ObjetosDeValor.CTe.Cliente()
                        {
                            CPFCNPJ = linha[mapeamento.CNPJEmitenteNFe],
                            RazaoSocial = entRemetente.Nome,
                            NomeFantasia = entRemetente.NomeFantasia,
                            Endereco = Utilidades.String.RemoveDiacritics(linha[mapeamento.LogradouroEmitenteNFe]),
                            Numero = "S/N",
                            Bairro = linha[mapeamento.BairroEmitenteNFe],
                            CodigoIBGECidade = IBGERemetente,
                            CEP = linha[mapeamento.CEPEmitenteNFe],
                            Telefone1 = linha[mapeamento.Telefone],
                            RGIE = linha[mapeamento.IEEmitenteNFe],
                            Emails = entRemetente.Email,
                            StatusEmails = !string.IsNullOrWhiteSpace(entRemetente.EmailStatus) ? entRemetente.EmailStatus.Equals("A") : false,
                            NaoAtualizarEndereco = true
                        };
                        // Remetente
                        string tipoCPFCNPJRemetente = remetente.CPFCNPJ.Length == 14 ? "J" : "F";
                        remetente.CodigoAtividade = Servicos.Atividade.ObterAtividade(transportadora.Codigo, tipoCPFCNPJRemetente, stringConexao, 0, unitOfWork).Codigo;
                        if (string.IsNullOrWhiteSpace(remetente.RGIE)) remetente.RGIE = "ISENTO";
                    }
                    #endregion

                    #endregion

                    #region ValorFrete
                    decimal valorFrete = 0;
                    try
                    {
                        valorFrete = decimal.Parse(linha[mapeamento.ValorFrete], cultura);
                    }
                    catch (Exception e)
                    {
                        this.LogErro("Erro ao converter Valor Frete (" + linha[mapeamento.ValorFrete] + ").", transportadora.NomeFantasia, nomeArquivo, index, stringConexao, false);
                    }
                    #endregion

                    #region Quantidades de Carega
                    // Quantidade do tipo Kilograma (KG)
                    Dominio.ObjetosDeValor.CTe.QuantidadeCarga cargaKilograma = new Dominio.ObjetosDeValor.CTe.QuantidadeCarga()
                    {
                        UnidadeMedida = "01", // 01 - KG
                        Descricao = "Kilograma",
                        Quantidade = pesoDocumento
                    };

                    List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga> quantidadesDeCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>() {
                        cargaKilograma
                    };
                    #endregion

                    #region Seguro
                    Dominio.ObjetosDeValor.CTe.Seguro seguro = new Dominio.ObjetosDeValor.CTe.Seguro()
                    {
                        Tipo = Dominio.Enumeradores.TipoSeguro.Emitente_CTE
                    };

                    List<Dominio.ObjetosDeValor.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.CTe.Seguro>() {
                        seguro
                    };
                    #endregion

                    #region CTe
                    Dominio.ObjetosDeValor.CTe.CTe cteIntegracao = new Dominio.ObjetosDeValor.CTe.CTe()
                    {
                        Emitente = emitente,
                        TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago,
                        DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        TipoCTe = Dominio.Enumeradores.TipoCTE.Normal,
                        TipoServico = Dominio.Enumeradores.TipoServico.Normal,
                        Retira = Dominio.Enumeradores.OpcaoSimNao.Nao,
                        TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente,
                        CodigoIBGECidadeInicioPrestacao = IBGERemetente,
                        CodigoIBGECidadeTerminoPrestacao = IBGEDestinatario,
                        Remetente = remetente,
                        Destinatario = destinatario,
                        ValorFrete = valorFrete,
                        ValorTotalMercadoria = valorDocumento,
                        ProdutoPredominante = !string.IsNullOrWhiteSpace(transportadora.Configuracao?.OutrasCaracteristicas) ? transportadora.Configuracao.OutrasCaracteristicas : "DIVERSOS",
                        Lotacao = Dominio.Enumeradores.OpcaoSimNao.Nao,
                        ObservacoesGerais = linha[mapeamento.Observacao] + (!string.IsNullOrWhiteSpace(linha[mapeamento.CodigoCalculoFrete]) ? " " + linha[mapeamento.CodigoCalculoFrete] : string.Empty),
                        IncluirICMSNoFrete = transportadora.Configuracao?.IncluirICMSNoFrete != null ? transportadora.Configuracao.IncluirICMSNoFrete : Dominio.Enumeradores.OpcaoSimNao.Nao,
                        PercentualICMSIncluirNoFrete = 100,
                        OutrasCaracteristicasDaCarga = !string.IsNullOrWhiteSpace(transportadora.Configuracao?.OutrasCaracteristicas) ? transportadora.Configuracao.OutrasCaracteristicas : string.Empty,
                        Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>() { documento },
                        QuantidadesCarga = quantidadesDeCarga,
                        Seguros = seguros,
                        ExibeICMSNaDACTE = true,
                        indicadorGlobalizado = Dominio.Enumeradores.OpcaoSimNao.Nao,
                        ValorCargaAverbacao = valorDocumento,
                        indicadorIETomador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS,
                        CodigoTabelaFreteIntegracao = !string.IsNullOrWhiteSpace(codigoCalculoFrete) ? codigoCalculoFrete : string.Empty
                    };

                    ctesIntegracao.Add(cteIntegracao);
                    #endregion
                }
                else
                {
                    this.LogErro("A tranportadora (" + cnpjTransportadora + ") não está cadastrada.", null, nomeArquivo, index, stringConexao, false);
                }

                index++;
            }

            // Log processamento de cte
            if (DEBUGARSUCESSO)
                this.LogErro("Iniciado o processamento dos CT-e. Total de conhecimentos: " + ctesIntegracao.Count().ToString(), null, nomeArquivo, null, stringConexao, false);

            // Gera os conhecimento
            string empresaErro = "";
            int totalDocumentosEmitidos = 0;
            try
            {
                // Lista de conhecimentos para enviar ao integrador
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                // Inicia transacao
                //unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                //foreach (Dominio.ObjetosDeValor.CTe.CTe cte in ctesIntegracao)
                for (var i = 0; i < ctesIntegracao.Count; i++)
                {
                    // Busca configuracao de e-mail
                    double cnpjCliente;
                    double.TryParse(ctesIntegracao[i].Remetente.CPFCNPJ, out cnpjCliente);

                    Dominio.Entidades.Empresa transportadora = repEmpresa.BuscarPorCNPJ(ctesIntegracao[i].Emitente.CNPJ);
                    Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = this.ConfiguracaoEmailCSV(email.ConfigEmail.Codigo, transportadora.Codigo, cnpjCliente, unitOfWork);

                    if (configuracaoEmissaoEmail != null)
                    {
                        // Salva o nome da empresa (para caso houver algum erro)
                        empresaErro = transportadora.NomeFantasia;

                        // Gera CT-e e deixa com salvo
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = servicoCte.GerarCTePorObjeto(ctesIntegracao[i], 0, unitOfWork, "1", 0, "S", null);

                        if (DEBUGARSUCESSO)
                            this.LogErro("O CT-e " + conhecimento.Numero.ToString() + "-" + conhecimento.Serie.Numero.ToString() + " da empresa " + conhecimento.Empresa.CNPJ + " foi gerado com sucesso.", null, null, null, stringConexao, false);

                        // Se configurado para emitir, chama servico para emitir
                        if (conhecimento != null && (configuracaoEmissaoEmail.Emitir == Dominio.Enumeradores.OpcaoSimNao.Sim && conhecimento.ValorFrete > 0))
                            ctes.Add(conhecimento);
                    }
                    else
                    {
                        this.LogErro("Configuração para emissão da NF-e " + ctesIntegracao[i].Documentos.FirstOrDefault().ChaveNFE + " não foi localizada", null, null, null, stringConexao, false);
                    }
                }
                //unitOfWork.CommitChanges();


                // Percorre lista para integrar
                for (var i = 0; i < ctes.Count; i++) //foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento in ctes)                
                {
                    if (!servicoCte.Emitir(ctes[i].Codigo, ctes[i].Empresa.Codigo, unitOfWork))
                        this.LogErro("O CT-e " + ctes[i].Numero.ToString() + "-" + ctes[i].Serie.Numero.ToString() + " da empresa " + ctes[i].Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao envia-lo ao Sefaz.", null, null, null, stringConexao, false);
                    else
                    {
                        if (ctes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(4, ctes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, stringConexao);
                        totalDocumentosEmitidos++;

                        if (DEBUGARSUCESSO)
                            this.LogErro("O CT-e " + ctes[i].Numero.ToString() + "-" + ctes[i].Serie.Numero.ToString() + " da empresa " + ctes[i].Empresa.CNPJ + " foi emitido com sucesso.", null, null, null, stringConexao, false);
                    }
                }
            }
            catch (Exception ex)
            {
                //    if (unitOfWork.Transacao != null && unitOfWork.Transacao.IsActive)
                //        unitOfWork.Rollback();

                this.LogErro("Ocorreu um erro ao gerar CT-e por CSV.", empresaErro, null, null, stringConexao, false);
                //Servicos.Log.TratarErro(ex);
            }
            finally
            {
                // Log processamento de cte
                this.LogErro("Fim do processamento do CSV. Total de conhecimentos emitidos: " + totalDocumentosEmitidos.ToString(), null, nomeArquivo, null, stringConexao, false);
            }
        }

        private void LogErro(string log, string transportadora, string nomeArquivo, int? linha, string stringConexao, bool enviarEmail)
        {
            string prefixoLog = "Emissão Email; ";

            if (!string.IsNullOrWhiteSpace(transportadora))
                prefixoLog = prefixoLog + "Transportadora " + transportadora + "; ";

            if (!string.IsNullOrWhiteSpace(nomeArquivo))
                prefixoLog = prefixoLog + "Arquivo " + nomeArquivo + "; ";

            if (linha != null)
                prefixoLog = prefixoLog + "Linha " + linha.ToString() + "; ";

            log = prefixoLog + "\n\r" + log;

            Servicos.Log.TratarErro(log, "ImportacaoEmail");
            if (enviarEmail)
                enviarEmailProblemaCTe("infra@multisoftware.com.br", log, stringConexao);
        }

        private Dominio.Entidades.ConfiguracaoEmissaoEmail ConfiguracaoEmailCSV(int codigoConfig, int codigoEmpresa, double cnpjCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);

            Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(codigoConfig, codigoEmpresa, cnpjCliente);

            if (configuracaoEmissaoEmail == null)
                configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(codigoConfig, codigoEmpresa, 0);

            return configuracaoEmissaoEmail;
        }

        private string SerieDaChave(string chave)
        {
            string serie = chave.Substring(22, 3);
            serie = serie.TrimStart('0'); // Remove 0 a esquerda

            return serie;
        }

        private List<List<string>> ConverteArquivo(Stream stream)
        {
            StreamReader csvreader = new StreamReader(stream);
            List<List<string>> linhas = new List<List<string>>();

            int j = 0;

            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(';');
                List<string> celulas = new List<string>();

                for (j = 0; j < values.Length; j++)
                {
                    celulas.Add(values[j].Trim());
                }

                linhas.Add(celulas);
            }
            csvreader.Close();

            return linhas;
        }

        private void processarXML(System.IO.Stream xml, Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada email, ref bool reimportar, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Servicos.NFe servicoNFe = new Servicos.NFe(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            dynamic xmlNotaFiscal = MultiSoftware.NFe.Servicos.Leitura.Ler(xml);

            if (xmlNotaFiscal != null && (xmlNotaFiscal.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc) || xmlNotaFiscal.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)))
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);

                string cnpjEmpresa = xmlNotaFiscal.NFe.infNFe.transp != null && xmlNotaFiscal.NFe.infNFe.transp.transporta != null ? xmlNotaFiscal.NFe.infNFe.transp.transporta.Item : "0";
                string chaveNFe = Utilidades.String.OnlyNumbers(xmlNotaFiscal.protNFe.infProt.chNFe);

                Dominio.Entidades.Empresa empresa = null;
                Dominio.Entidades.Cliente cliente = null;
                try
                {
                    empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresa);
                    cliente = servicoNFe.ObterEmitente(xmlNotaFiscal.NFe.infNFe.emit, empresa != null ? empresa.Codigo : 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    reimportar = true;
                    throw;
                }

                if (empresa != null)
                {
                    Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(email.ConfigEmail.Codigo, empresa.Codigo, cliente.CPF_CNPJ);

                    if (configuracaoEmissaoEmail == null)
                        configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(email.ConfigEmail.Codigo, empresa.Codigo, 0);

                    if (configuracaoEmissaoEmail == null)
                    {
                        //Servicos.Log.TratarErro("Configuração para emissão da NF-e " + chaveNFe + " não foi localizada");
                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Configuração para emissão da NF-e " + chaveNFe + " não foi localizada", stringConexao);
                        this.LogErro("Configuração para emissão da NF-e " + chaveNFe + " não foi localizada", "", "", 0, stringConexao, false);
                    }
                    else
                    {
                        if (configuracaoEmissaoEmail.Tipo == "F") //Frimesa
                            this.GerarEmissaoFrimesa(xmlNotaFiscal, empresa, unitOfWork, stringConexao);
                        else
                            this.GerarEmissaoAutomatica(xmlNotaFiscal, configuracaoEmissaoEmail, unitOfWork, stringConexao);
                    }
                }
                else
                {
                    //Servicos.Log.TratarErro("Transportadora da NF-e " + chaveNFe + " não foi localizada");
                    //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Transportadora da NF-e " + chaveNFe + " não foi localizada", stringConexao);
                    this.LogErro("Transportadora da NF-e " + chaveNFe + " não foi localizada", "", "", 0, stringConexao, false);
                }
            }
            else
            {
                dynamic xmlCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(xml);
                if (xmlCTe != null)
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cte = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)xmlCTe;
                    if (cte != null)
                    {
                        string cnpjEmpresa = cte.CTe.infCte.emit.Item;

                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                        Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresa);

                        if (empresa != null)
                        {
                            Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.BuscarPorEmpresaTipoDocumento(email.ConfigEmail.Codigo, empresa.Codigo, Dominio.Enumeradores.TipoDocumento.CTe);

                            if (configuracaoEmissaoEmail != null)
                            {
                                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                                object retorno = svcCTe.GerarCTeAnterior(xml, empresa.Codigo, string.Empty, string.Empty, unitOfWork, null, true, false);
                                if (retorno != null)
                                {
                                    if (retorno.GetType() == typeof(string))
                                        this.LogErro("Email assunto " + email.Assunto + ". Não foi possível importar CTe para a empresa " + cnpjEmpresa, "", "", 0, stringConexao, false);
                                }
                                else
                                {
                                    this.LogErro("Email assunto " + email.Assunto + ". Não foi possível importar CTe", "", "", 0, stringConexao, false);
                                }
                            }
                            else
                                this.LogErro("Email assunto " + email.Assunto + ". Configuração para importação de CTe não encontrada para a empresa " + cnpjEmpresa, "", "", 0, stringConexao, false);
                        }
                        else
                        {
                            this.LogErro("Email assunto " + email.Assunto + ". Empresa " + cnpjEmpresa + " não possui cadastro.", "", "", 0, stringConexao, false);
                        }
                    }
                    else
                    {
                        this.LogErro("Email assunto " + email.Assunto + ". XML CTe não foi importado " + xml.ToString(), "", "", 0, stringConexao, false);
                    }
                }
                else
                {
                    Servicos.Subcontratacao svcSubcontratacao = new Servicos.Subcontratacao(unitOfWork);
                    Dominio.ObjetosDeValor.Subcontratacao subcontratacaoImportada = svcSubcontratacao.ObterDadosXNLYamalog(xml);

                    if (subcontratacaoImportada != null)
                    {
                        Dominio.Entidades.Subcontratacao subcontratacao = svcSubcontratacao.SalvarSubcontratacao(subcontratacaoImportada, unitOfWork);
                        if (subcontratacao == null)
                            this.LogErro("Email assunto " + email.Assunto + ". dados de Yamalog não foram importados " + xml.ToString(), "", "", 0, stringConexao, false);
                    }
                    else
                    {
                        //Servicos.Log.TratarErro("Email assunto " + email.Assunto + ". XML não foi importado " + xml.ToString());
                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Email assunto " + email.Assunto + ". XML não foi importado " + xml.ToString(), stringConexao);
                        this.LogErro("Email assunto " + email.Assunto + ". XML não foi importado " + xml.ToString(), "", "", 0, stringConexao, false);
                    }
                }
            }
        }

        private void GerarEmissaoFrimesa(dynamic xmlNotaFiscal, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            Servicos.NFSe serNFSe = new Servicos.NFSe(unitOfWork);
            Servicos.NFe serNFe = new Servicos.NFe(unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.CargaFrimesa repCargaFrimesa = new Repositorio.CargaFrimesa(unitOfWork);
            Repositorio.CargaFrimesaDocumentos repCargaFrimesaDocumentos = new Repositorio.CargaFrimesaDocumentos(unitOfWork);

            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            if (empresa != null)
            {
                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
                dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);
            }
            DateTime dataEmissao = new DateTime(dataFuso.Year, dataFuso.Month, dataFuso.Day, 0, 0, 0);

            DateTime dataNFe = DateTime.Today;
            DateTime.TryParseExact(xmlNotaFiscal.NFe.infNFe.ide.dhEmi.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeLocal, out dataNFe);

            string cnpjEmpresa = xmlNotaFiscal.NFe.infNFe.transp != null && xmlNotaFiscal.NFe.infNFe.transp.transporta != null ? xmlNotaFiscal.NFe.infNFe.transp.transporta.Item : "0";
            Dominio.Entidades.Cliente emitenteNFe = serNFe.ObterEmitente(xmlNotaFiscal.NFe.infNFe.emit, empresa.Codigo);
            Dominio.Entidades.Cliente destinatarioNFe = serNFe.ObterDestinatario(xmlNotaFiscal.NFe.infNFe.dest, empresa.Codigo);
            string chaveNFe = Utilidades.String.OnlyNumbers(xmlNotaFiscal.protNFe.infProt.chNFe);
            string placa = this.ObterPlacaVeiculo(xmlNotaFiscal.NFe.infNFe.transp);
            string natureza = xmlNotaFiscal.NFe.infNFe.ide.natOp;
            string observacao = string.Empty;
            bool nfeDevolucao = natureza.Contains("DEVOLUÇÃO") || natureza.Contains("Devol.");

            if (!nfeDevolucao)
            {
                if (empresa != null)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, placa);
                    if (veiculo != null)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorNFePlaca(empresa.Codigo, empresa.TipoAmbiente, "", "", chaveNFe, "", DateTime.MinValue, new string[] { "A" });
                        Dominio.Entidades.NFSe nfse = new Dominio.Entidades.NFSe();

                        if (cte != null)
                        {
                            //Servicos.Log.TratarErro("Transportadora " + cnpjEmpresa + ": CTe " + cte.Numero + " já autorizado com a nota " + chaveNFe);
                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Transportadora " + cnpjEmpresa + ": CTe " + cte.Numero + " já autorizado com a nota " + chaveNFe, stringConexao);
                            this.LogErro("CTe " + cte.Numero + " já autorizado com a nota " + chaveNFe, cnpjEmpresa, "", 0, stringConexao, false);
                        }
                        else
                        {
                            Dominio.Entidades.CargaFrimesa cargaFrimesa = repCargaFrimesa.BuscarPorVeiculoRotaData(empresa.Codigo, veiculo.Codigo, 0, dataNFe);
                            decimal valorFreteTabela = cargaFrimesa != null ? cargaFrimesa.ValorFrete : 0;
                            observacao = cargaFrimesa != null ? " Rota: " + cargaFrimesa.DescricaoRota + " - Veículo: " + cargaFrimesa.DescricaoVeiculo + " - Tipo: " + cargaFrimesa.DescricaoTipo : string.Empty;
                            if (cargaFrimesa != null && !string.IsNullOrWhiteSpace(cargaFrimesa.DescricaoCarga))
                                observacao = " Carga: " + cargaFrimesa.DescricaoCarga + " - " + observacao;

                            if (cargaFrimesa != null && cargaFrimesa.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe && empresa.Configuracao != null && !string.IsNullOrEmpty(empresa.Configuracao.SerieRPSNFSe))
                            {
                                nfse = repNFSe.BuscarPorVeiculo(empresa.Codigo, empresa.TipoAmbiente, veiculo.Codigo, dataEmissao, null);
                                if (nfse == null)
                                {
                                    nfse = serNFSe.GerarNFSePorNFe(xmlNotaFiscal, empresa, veiculo.Codigo, false, observacao, valorFreteTabela, Dominio.Enumeradores.StatusNFSe.EmDigitacao, unitOfWork, stringConexao);
                                    if (cargaFrimesa != null)
                                    {
                                        Dominio.Entidades.CargaFrimesaDocumentos cargaFrimesaDocumentos = new Dominio.Entidades.CargaFrimesaDocumentos();
                                        cargaFrimesaDocumentos.CargaFrimesa = cargaFrimesa;
                                        cargaFrimesaDocumentos.NFSe = nfse;
                                        repCargaFrimesaDocumentos.Inserir(cargaFrimesaDocumentos);
                                    }
                                }
                                else
                                {
                                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao)
                                    {
                                        var adicionarNota = serNFSe.AdicionarNotaNFSeDigitacao(nfse, xmlNotaFiscal, unitOfWork);
                                        if (adicionarNota.Trim() != "")
                                        {
                                            //Servicos.Log.TratarErro("NF-e " + chaveNFe + " NFS-e " + nfse.Numero + " da transportadora " + cnpjEmpresa + ": " + adicionarNota);
                                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " NFSe-e " + nfse.Numero + " da transportadora " + cnpjEmpresa + ": " + adicionarNota, stringConexao);
                                            this.LogErro("NF-e " + chaveNFe + " NFSe-e " + nfse.Numero + ": " + adicionarNota, cnpjEmpresa, "", 0, stringConexao, false);
                                        }
                                    }
                                }

                                if (cargaFrimesa != null && nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao && cargaFrimesa.ValorFretePlanilha == false)
                                {
                                    cargaFrimesa.ValorAdicionalPeso = FrimesaCalcularAdicionalPesoNFSe(nfse.Codigo, cargaFrimesa, unitOfWork, stringConexao);
                                    repCargaFrimesa.Atualizar(cargaFrimesa);
                                }
                            }
                            else
                            {
                                if (cargaFrimesa != null && cargaFrimesa.Rota != null && cargaFrimesa.Rota.PermiteAgruparCargas == false) //Emite os CTes agrupando por Remetente e Destinatário
                                    cte = repCTe.BuscarPorNFePlaca(empresa.Codigo, empresa.TipoAmbiente, emitenteNFe.CPF_CNPJ_SemFormato, destinatarioNFe.CPF_CNPJ_SemFormato, "", veiculo.Placa, dataEmissao, new string[] { "S", "A" });
                                else //Quando configurado para agrupar emite os CTes como DIVERSOS
                                    cte = repCTe.BuscarPorNFePlaca(empresa.Codigo, empresa.TipoAmbiente, "", "", "", veiculo.Placa, dataEmissao, new string[] { "S", "A" });

                                if (cte == null)
                                {
                                    if (cargaFrimesa != null && cargaFrimesa.ValorFretePlanilha == false) //Frimesa do Rio de Janeiro o Frete deve ser Apagar e Tomador Remetente
                                    {
                                        if (cargaFrimesa != null && cargaFrimesa.Rota != null && cargaFrimesa.Rota.PermiteAgruparCargas == false) //Quando configurado para não agrupar gera um CTe agrupando por Remetente e Destinatário
                                            cte = serCTe.GerarCTePorNFe(xmlNotaFiscal, empresa.Codigo, valorFreteTabela, 0, observacao, true, null, unitOfWork, false, false, Dominio.Enumeradores.TipoPagamento.A_Pagar, Dominio.Enumeradores.TipoTomador.Remetente);
                                        else //Quando configurado para agrupar emite os CTes como DIVERSOS
                                            cte = serCTe.GerarCTePorNFe(xmlNotaFiscal, empresa.Codigo, valorFreteTabela, 0, observacao, true, null, unitOfWork, false, true, Dominio.Enumeradores.TipoPagamento.A_Pagar, Dominio.Enumeradores.TipoTomador.Remetente);
                                    }
                                    else
                                        cte = serCTe.GerarCTePorNFe(xmlNotaFiscal, empresa.Codigo, valorFreteTabela, 0, observacao, true, null, unitOfWork, false, true, Dominio.Enumeradores.TipoPagamento.Pago, Dominio.Enumeradores.TipoTomador.Remetente);

                                    if (cargaFrimesa != null)
                                    {
                                        List<Dominio.Entidades.CargaFrimesaDocumentos> documentosCarga = repCargaFrimesaDocumentos.BuscarPorCargaFrimesa(cargaFrimesa.Codigo);
                                        int documentosAutorizados = 0;
                                        try
                                        {
                                            if (documentosCarga != null && documentosCarga.Count > 0)
                                                documentosAutorizados = (from o in documentosCarga where o.CTe != null && o.CTe.Status == "A" select o).Count();
                                        }
                                        catch (Exception ex)
                                        {
                                            this.LogErro("Ocorreu erro ao verificar documentos emitidos da carga: " + ex, cte.Empresa.CNPJ, null, null, stringConexao, false);
                                        }

                                        if (documentosAutorizados == 0)
                                        {
                                            Dominio.Entidades.CargaFrimesaDocumentos cargaFrimesaDocumentos = new Dominio.Entidades.CargaFrimesaDocumentos();
                                            cargaFrimesaDocumentos.CargaFrimesa = cargaFrimesa;
                                            cargaFrimesaDocumentos.CTe = cte;
                                            repCargaFrimesaDocumentos.Inserir(cargaFrimesaDocumentos);
                                        }
                                    }
                                }
                                else if (cte.Status == "S")
                                {
                                    var adicionarNota = serCTe.AdicionarNotaCTeDigitacao(cte, xmlNotaFiscal, unitOfWork);
                                    if (adicionarNota.Trim() != "")
                                    {
                                        //Servicos.Log.TratarErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cnpjEmpresa + ": " + adicionarNota);
                                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cnpjEmpresa + ": " + adicionarNota, stringConexao);
                                        this.LogErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + ": " + adicionarNota, cnpjEmpresa, "", 0, stringConexao, false);
                                    }
                                }
                                else if (cte.Status == "A")
                                {
                                    Dominio.Entidades.CargaFrimesa cargaCTe = repCargaFrimesa.BuscarPorCTe(cte.Codigo);
                                    if (cargaCTe != null)
                                    {
                                        //Servicos.Log.TratarErro("NF-e " + chaveNFe + " da transportadora " + cnpjEmpresa + " enviada após emissão do CTe " + cte.Numero);
                                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " da transportadora " + cnpjEmpresa + " enviada após emissão do CTe " + cte.Numero, stringConexao);
                                        this.LogErro("NF-e " + chaveNFe + " enviada após emissão do CTe " + cte.Numero, cnpjEmpresa, "", 0, stringConexao, false);
                                    }
                                    else
                                    {
                                        cte = null;
                                        cte = serCTe.GerarCTePorNFe(xmlNotaFiscal, empresa.Codigo, valorFreteTabela, 0, observacao, true, null, unitOfWork, false, true, Dominio.Enumeradores.TipoPagamento.A_Pagar, Dominio.Enumeradores.TipoTomador.Remetente);
                                        if (cargaFrimesa != null)
                                        {
                                            Dominio.Entidades.CargaFrimesaDocumentos cargaFrimesaDocumentos = new Dominio.Entidades.CargaFrimesaDocumentos();
                                            cargaFrimesaDocumentos.CargaFrimesa = cargaFrimesa;
                                            cargaFrimesaDocumentos.CTe = cte;
                                            repCargaFrimesaDocumentos.Inserir(cargaFrimesaDocumentos);
                                        }
                                    }
                                }

                                if (cargaFrimesa != null && cte != null && cte.Status == "S" && cargaFrimesa.ValorFretePlanilha == false)
                                {
                                    cargaFrimesa.ValorAdicionalPeso = FrimesaCalcularAdicionalPesoCTe(cte.Codigo, cargaFrimesa, unitOfWork, stringConexao);
                                    repCargaFrimesa.Atualizar(cargaFrimesa);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Servicos.Log.TratarErro("Veiculo da transportadora não cadastrado. CNPJ " + cnpjEmpresa + " Placa " + placa + " NFe " + chaveNFe);
                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Veiculo da transportadora não cadastrado. CNPJ " + cnpjEmpresa + " Placa " + placa + " NFe " + chaveNFe, stringConexao);
                        this.LogErro("Veiculo da transportadora não cadastrado. Placa " + placa + " NFe " + chaveNFe, cnpjEmpresa, "", 0, stringConexao, false);
                    }
                }
                else
                {
                    //Servicos.Log.TratarErro("Transportadora não cadadastrada. CNPJ " + cnpjEmpresa + " NFe " + chaveNFe);
                    //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Transportadora não cadadastrada. CNPJ " + cnpjEmpresa + " NFe " + chaveNFe, stringConexao);
                    this.LogErro("Transportadora não cadadastrada. NFe " + chaveNFe, cnpjEmpresa, "", 0, stringConexao, false);
                }
            }
            else
            {
                //Servicos.Log.TratarErro("Nota fiscal de Devolução. Transportadora CNPJ " + cnpjEmpresa + " NFe " + chaveNFe);
                this.LogErro("Nota fiscal de Devolução. NFe " + chaveNFe, cnpjEmpresa, "", 0, stringConexao, false);
            }
        }

        private void GerarEmissaoAutomatica(dynamic xmlNotaFiscal, Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Servicos.NFe serNFe = new Servicos.NFe(unitOfWork);
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            Servicos.NFSe serNFSe = new Servicos.NFSe(unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.EmissaoEmail repEmissaoEmail = new Repositorio.EmissaoEmail(unitOfWork);

            Dominio.Entidades.EmissaoEmail emissaoEmail = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
            Dominio.Entidades.NFSe nfse = null;
            Dominio.Entidades.Veiculo veiculo = null;
            Dominio.Entidades.Cliente clienteRemetente = serNFe.ObterEmitente(xmlNotaFiscal.NFe.infNFe.emit, configuracaoEmissaoEmail.Empresa != null ? configuracaoEmissaoEmail.Empresa.Codigo : 0);
            Dominio.Entidades.Cliente clienteDestinatario = serNFe.ObterDestinatario(xmlNotaFiscal.NFe.infNFe.dest, configuracaoEmissaoEmail.Empresa != null ? configuracaoEmissaoEmail.Empresa.Codigo : 0);

            Dominio.Enumeradores.TipoDocumento tipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;

            DateTime dataEmissao = DateTime.Now.AddHours(-6); //Data de parametro para verificação se já teve emissão
            DateTime dataNFe = DateTime.Today;
            DateTime.TryParseExact(xmlNotaFiscal.NFe.infNFe.ide.dhEmi.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.AssumeLocal, out dataNFe);

            string chaveNFe = Utilidades.String.OnlyNumbers(xmlNotaFiscal.protNFe.infProt.chNFe);
            string placa = this.ObterPlacaVeiculo(xmlNotaFiscal.NFe.infNFe.transp);
            string obsNFe = xmlNotaFiscal.NFe.infNFe.infAdic.infCpl;
            string observacao = string.Empty;

            bool empresaEmiteNFSe = !string.IsNullOrWhiteSpace(configuracaoEmissaoEmail.Empresa.Configuracao.SerieRPSNFSe);
            bool agruparNotas = false;

            if (!string.IsNullOrWhiteSpace(placa))
                veiculo = repVeiculo.BuscarPorPlaca(configuracaoEmissaoEmail.Empresa.Codigo, placa);

            if (xmlNotaFiscal.NFe.infNFe.emit.enderEmit.cMun == xmlNotaFiscal.NFe.infNFe.dest.enderDest.cMun)
            {
                if (configuracaoEmissaoEmail.TipoDocumento == Dominio.Enumeradores.TipoDocumento.Todos || configuracaoEmissaoEmail.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    if (!string.IsNullOrEmpty(configuracaoEmissaoEmail.Empresa.Configuracao.SerieRPSNFSe))
                        tipoDocumento = Dominio.Enumeradores.TipoDocumento.NFSe;
                    else if (configuracaoEmissaoEmail.TipoDocumento == Dominio.Enumeradores.TipoDocumento.Todos)
                        tipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;
                    else if (configuracaoEmissaoEmail.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        tipoDocumento = Dominio.Enumeradores.TipoDocumento.Nenhum;
                        //Servicos.Log.TratarErro("NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " é municipal e transportadora não está configurado para emitir NFS-e.");
                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " é municipal e transportadora não está configurado para emitir NFS-e", stringConexao);
                        this.LogErro("NF-e " + chaveNFe + " é municipal e transportadora não está configurado para emitir NFS-e", configuracaoEmissaoEmail.Empresa.CNPJ, "", 0, stringConexao, false);
                    }

                }
                else if (configuracaoEmissaoEmail.TipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    tipoDocumento = Dominio.Enumeradores.TipoDocumento.Nenhum;
                    //Servicos.Log.TratarErro("NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " é municipal e está configurado para emitir apenas CT-e.");
                    //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " é municipal e está configurado para emitir apenas CT-e.", stringConexao);
                    this.LogErro("NF-e " + chaveNFe + " é municipal e está configurado para emitir apenas CT-e.", configuracaoEmissaoEmail.Empresa.CNPJ, "", 0, stringConexao, false);
                }
            }
            else
                tipoDocumento = Dominio.Enumeradores.TipoDocumento.CTe;


            if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                cte = repCTe.BuscarPorNFePlaca(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, "", "", chaveNFe, "", DateTime.MinValue, new string[] { "S", "A", "R" });
                if (cte == null)
                {
                    //if (!string.IsNullOrWhiteSpace(placa))
                    //    veiculo = repVeiculo.BuscarPorPlaca(configuracaoEmissaoEmail.Empresa.Codigo, placa);

                    if (configuracaoEmissaoEmail.Agrupar == Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail.Placa && veiculo != null)
                        cte = repCTe.BuscarPorNFePlaca(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, "", "", "", veiculo.Placa, dataEmissao, new string[] { "S", "A", "R", "M" });
                    else if (configuracaoEmissaoEmail.Agrupar == Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail.Destinatario && clienteDestinatario != null)
                        cte = repCTe.BuscarPorDestinatario(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, clienteRemetente?.CPF_CNPJ_SemFormato ?? string.Empty, clienteDestinatario.CPF_CNPJ_SemFormato, dataEmissao, new string[] { "S", "A", "R", "M" });
                    else if (configuracaoEmissaoEmail.Agrupar == Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail.Observacao && !string.IsNullOrWhiteSpace(configuracaoEmissaoEmail.PalavraChave) && !string.IsNullOrWhiteSpace(obsNFe))
                    {
                        string palavraAgrupador = this.ExtrairPalavraAgrupador(obsNFe, configuracaoEmissaoEmail.PalavraChave, configuracaoEmissaoEmail.TamanhoPalavra);
                        if (!string.IsNullOrWhiteSpace(palavraAgrupador))
                        {
                            cte = repCTe.BuscarPorObservacao(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, palavraAgrupador, dataEmissao, new string[] { "S", "A", "R", "M" });

                            if (configuracaoEmissaoEmail.PalavraChave == "Transp: ")
                            {
                                int.TryParse(palavraAgrupador, out int palavraAgrupadoraInteriro);

                                observacao = string.Concat("DOCTRANSP: ", palavraAgrupadoraInteriro > 0 ? palavraAgrupadoraInteriro.ToString() : palavraAgrupador);

                                if (cte == null)
                                    cte = repCTe.BuscarPorObservacao(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, palavraAgrupadoraInteriro.ToString(), dataEmissao, new string[] { "S", "A", "R", "M" });
                            }
                            else
                                observacao = string.Concat(configuracaoEmissaoEmail.PalavraChave, palavraAgrupador);
                            agruparNotas = true;
                        }
                    }

                    if (cte == null)
                    {
                        cte = serCTe.GerarCTePorNFe(xmlNotaFiscal, configuracaoEmissaoEmail.Empresa.Codigo, 0, 0, observacao, true, null, unitOfWork, configuracaoEmissaoEmail.Emitir == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false, false, null, null);

                        if (!serCTe.CalcularFretePorTabelaDeFrete(ref cte, cte.Empresa.Codigo, unitOfWork, true) || cte.ValorFrete <= 0)//if (!serCTe.CalcularFretePorTabelaDeFreteEICMS(ref cte, unitOfWork) || cte.ValorFrete <= 0)
                        {
                            if (agruparNotas)
                                cte.Status = "M";
                            else
                                cte.Status = "S";
                            repCTe.Atualizar(cte);
                            //Servicos.Log.TratarErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cte.Empresa.CNPJ + ": não foi possível calcular frete.");
                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cte.Empresa.CNPJ + ": não foi possível calcular frete.", stringConexao);
                            this.LogErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + ": não foi possível calcular frete.", cte.Empresa.CNPJ, "", 0, stringConexao, false);
                        }
                        else
                        {
                            if (agruparNotas)
                            {
                                cte.Status = "M";
                                repCTe.Atualizar(cte);
                            }
                        }

                        emissaoEmail = new Dominio.Entidades.EmissaoEmail();
                        emissaoEmail.TipoDocumento = tipoDocumento;
                        emissaoEmail.CTe = cte;
                        emissaoEmail.Data = DateTime.Now;
                        emissaoEmail.Empresa = configuracaoEmissaoEmail.Empresa;
                        repEmissaoEmail.Inserir(emissaoEmail);

                        if (cte.Status == "E")
                        {
                            if (!serCTe.Emitir(cte.Codigo, cte.Empresa.Codigo))
                            {
                                cte.Status = "S";
                                repCTe.Atualizar(cte);
                                //Servicos.Log.TratarErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cte.Empresa.CNPJ + ": ocorreu uma falha e não foi possível emitir.");
                                //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cte.Empresa.CNPJ + ": ocorreu uma falha e não foi possível emitir.", stringConexao);
                                this.LogErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + ": ocorreu uma falha e não foi possível emitir.", cte.Empresa.CNPJ, "", 0, stringConexao, false);
                            }
                            else if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                                FilaConsultaCTe.GetInstance().QueueItem(4, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, stringConexao);
                        }
                    }
                    else if (cte.Status == "S" || cte.Status == "R" || cte.Status == "M")
                    {
                        var adicionarNota = serCTe.AdicionarNotaCTeDigitacao(cte, xmlNotaFiscal, unitOfWork);
                        if (adicionarNota.Trim() != "")
                        {
                            //Servicos.Log.TratarErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + ": " + adicionarNota);
                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + ": " + adicionarNota, stringConexao);
                            this.LogErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + ": " + adicionarNota, configuracaoEmissaoEmail.Empresa.CNPJ, "", 0, stringConexao, false);
                        }
                        else
                        {
                            if (!serCTe.CalcularFretePorTabelaDeFrete(ref cte, cte.Empresa.Codigo, unitOfWork, true) || cte.ValorFrete <= 0)//if (!serCTe.CalcularFretePorTabelaDeFreteEICMS(ref cte, unitOfWork))
                            {
                                cte.Status = "S";
                                repCTe.Atualizar(cte);
                                //Servicos.Log.TratarErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cte.Empresa.CNPJ + ": não foi possível calcular frete.");
                                //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " CT-e " + cte.Numero + " da transportadora " + cte.Empresa.CNPJ + ": não foi possível calcular frete.", stringConexao);
                                this.LogErro("NF-e " + chaveNFe + " CT-e " + cte.Numero + ": não foi possível calcular frete.", cte.Empresa.CNPJ, "", 0, stringConexao, false);
                            }
                        }
                    }
                    else if (cte.Status == "A")
                    {
                        //Servicos.Log.TratarErro("NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " enviada após emissão do CTe " + cte.Numero);
                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " enviada após emissão do CTe " + cte.Numero, stringConexao);
                        this.LogErro("NF-e " + chaveNFe + " da transportadora enviada após emissão do CTe " + cte.Numero, configuracaoEmissaoEmail.Empresa.CNPJ, "", 0, stringConexao, false);
                    }
                }
                else
                {
                    //Servicos.Log.TratarErro("Transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + ": CTe " + cte.Numero + " já existe com a nota " + chaveNFe);
                    //enviarEmailProblemaCTe("infra@multisoftware.com.br", "Transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + ": CTe " + cte.Numero + " já existe com a nota " + chaveNFe, stringConexao);
                    this.LogErro("CTe " + cte.Numero + " já existe com a nota " + chaveNFe, configuracaoEmissaoEmail.Empresa.CNPJ, "", 0, stringConexao, false);
                }
            }
            else if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                if (configuracaoEmissaoEmail.Agrupar == Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail.Placa)
                    nfse = repNFSe.BuscarPorVeiculo(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, veiculo?.Codigo ?? -1, dataEmissao, null);
                else if (configuracaoEmissaoEmail.Agrupar == Dominio.Enumeradores.TipoAgrupamentoEmissaoEmail.Destinatario)
                    nfse = repNFSe.BuscarPorVeiculo(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, veiculo?.Codigo ?? -1, dataEmissao, null);
                else
                    nfse = repNFSe.BuscarPorTomador(configuracaoEmissaoEmail.Empresa.Codigo, configuracaoEmissaoEmail.Empresa.TipoAmbiente, clienteRemetente.CPF_CNPJ_SemFormato, dataEmissao, null);

                if (nfse == null)
                {
                    nfse = serNFSe.GerarNFSePorNFe(xmlNotaFiscal, configuracaoEmissaoEmail.Empresa, veiculo != null ? veiculo.Codigo : 0, false, observacao, 0, configuracaoEmissaoEmail.Emitir == Dominio.Enumeradores.OpcaoSimNao.Sim ? Dominio.Enumeradores.StatusNFSe.Enviado : Dominio.Enumeradores.StatusNFSe.EmDigitacao, unitOfWork, stringConexao);
                    emissaoEmail = new Dominio.Entidades.EmissaoEmail();
                    emissaoEmail.TipoDocumento = tipoDocumento;
                    emissaoEmail.NFSe = nfse;
                    emissaoEmail.Data = DateTime.Now;
                    emissaoEmail.Empresa = configuracaoEmissaoEmail.Empresa;
                    repEmissaoEmail.Inserir(emissaoEmail);

                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Enviado)
                    {
                        if (!serNFSe.Emitir(nfse))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.EmDigitacao;
                            repNFSe.Atualizar(nfse);
                            //Servicos.Log.TratarErro("NF-e " + chaveNFe + " NFS-e " + nfse.Numero + " da transportadora " + nfse.Empresa.CNPJ + ": ocorreu uma falha e não foi possível emitir.");
                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " NFS-e " + nfse.Numero + " da transportadora " + nfse.Empresa.CNPJ + ": ocorreu uma falha e não foi possível emitir.", stringConexao);
                            this.LogErro("NF-e " + chaveNFe + " NFS-e " + nfse.Numero + ": ocorreu uma falha e não foi possível emitir.", nfse.Empresa.CNPJ, "", 0, stringConexao, false);
                        }
                        else
                            FilaConsultaCTe.GetInstance().QueueItem(4, nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, stringConexao);
                    }
                }
                else
                {
                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao)
                    {
                        var adicionarNota = serNFSe.AdicionarNotaNFSeDigitacao(nfse, xmlNotaFiscal, unitOfWork);
                        if (adicionarNota.Trim() != "")
                        {
                            //Servicos.Log.TratarErro("NF-e " + chaveNFe + " NFSe-e " + nfse.Numero + " da transportadora " + configuracaoEmissaoEmail.Empresa + ": " + adicionarNota);
                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " NFSe-e " + nfse.Numero + " da transportadora " + configuracaoEmissaoEmail.Empresa + ": " + adicionarNota, stringConexao);
                            this.LogErro("NF-e " + chaveNFe + " NFSe-e " + nfse.Numero + ": " + adicionarNota, configuracaoEmissaoEmail.Empresa.CNPJ, "", 0, stringConexao, false);
                        }
                    }
                    else
                    {
                        //Servicos.Log.TratarErro("NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " enviada após emissão da NFSe " + nfse.Numero);
                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", "NF-e " + chaveNFe + " da transportadora " + configuracaoEmissaoEmail.Empresa.CNPJ + " enviada após emissão da NFSe " + nfse.Numero, stringConexao);
                        this.LogErro("NF-e " + chaveNFe + " enviada após emissão da NFSe " + nfse.Numero, configuracaoEmissaoEmail.Empresa.CNPJ, "", 0, stringConexao, false);
                    }
                }
            }
        }

        private void enviarEmailProblemaCTe(string remetente, string conteudo, string stringConexao, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = null, Repositorio.UnitOfWork unitOfWork = null, System.Net.Mail.Attachment xmlAnexo = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            if (email != null)
            {
                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                Servicos.PreCTe serPreCte = new Servicos.PreCTe(unitOfWork);
                List<System.Net.Mail.Attachment> anexos = null;
                if (preCte != null)
                {
                    anexos = new List<System.Net.Mail.Attachment>();
                    string xml = serPreCte.BuscarXMLPreCte(preCte);
                    MemoryStream stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(xml));
                    System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(stream, string.Concat("pre_cte_" + preCte.Codigo, ".xml"));
                    anexos.Add(anexo);
                }

                conteudo += "<hr/>";

                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                {
                    conteudo += email.MensagemRodape.Replace("#qLinha#", "<br/>");
                }

                serEmail.EnviarEmail(email.Email, email.Email, email.Senha, remetente, "", "", "Problemas emissão automática CTe", conteudo, email.Smtp, anexos, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp);
            }
        }

        private string ObterPlacaVeiculo(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.Items != null)
                {
                    string placa = string.Empty;

                    foreach (var item in infNFeTransp.Items)
                        if (item.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TVeiculo))
                        {
                            MultiSoftware.NFe.v310.NotaFiscal.TVeiculo veiculo = (MultiSoftware.NFe.v310.NotaFiscal.TVeiculo)item;
                            return veiculo.placa;
                        }
                }
            }
            return string.Empty;
        }

        private string ObterPlacaVeiculo(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTransp infNFeTransp)
        {
            if (infNFeTransp != null)
            {
                if (infNFeTransp.Items != null)
                {
                    string placa = string.Empty;

                    foreach (var item in infNFeTransp.Items)
                        if (item.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TVeiculo))
                        {
                            MultiSoftware.NFe.v400.NotaFiscal.TVeiculo veiculo = (MultiSoftware.NFe.v400.NotaFiscal.TVeiculo)item;
                            return veiculo.placa;
                        }
                }
            }
            return string.Empty;
        }

        private decimal FrimesaCalcularAdicionalPesoCTe(int codigoCTe, Dominio.Entidades.CargaFrimesa cargaFrimesa, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            try
            {
                Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

                Repositorio.CargaFrimesaDocumentos repCargaFrimesaDocumentos = new Repositorio.CargaFrimesaDocumentos(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCargaFrimesaDocumentos.BuscarCTePorCargaFrimesa(cargaFrimesa.Codigo);
                decimal pesoCTe = 0;
                if (listaCTes != null && listaCTes.Count > 0)
                {
                    for (var i = 0; i < listaCTes.Count(); i++)
                        pesoCTe += repInformacaoCargaCTE.ObterPesoKg(listaCTes[i].Codigo);
                }

                if (pesoCTe == 0)
                    pesoCTe = repInformacaoCargaCTE.ObterPesoKg(cte.Codigo);

                decimal valorAdicional = 0;

                if (cte != null && cargaFrimesa != null)
                {
                    decimal valorFrete = cargaFrimesa.ValorFrete;

                    if (valorFrete > 0 && cargaFrimesa.TipoVeiculo != null && cargaFrimesa.TipoVeiculo.PesoBruto > 0 && pesoCTe > cargaFrimesa.TipoVeiculo.PesoBruto)
                    {
                        decimal pesoAdicional = pesoCTe - cargaFrimesa.TipoVeiculo.PesoBruto;
                        valorAdicional = Math.Round(Math.Round((valorFrete / cargaFrimesa.TipoVeiculo.PesoBruto), 4, MidpointRounding.ToEven) * pesoAdicional, 2, MidpointRounding.ToEven);

                        if (!serCTe.AlterarValorFreteCTe(ref cte, valorFrete + valorAdicional, 0, 0, 0, unitOfWork))
                        {
                            string mensagemErro = cte != null ? "Empresa: " + cte.Empresa.CNPJ + " CTe: " + cte.Numero : string.Empty + " Não foi possível calcular adicional de peso.";
                            //Servicos.Log.TratarErro(mensagemErro);
                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", mensagemErro, stringConexao);
                            this.LogErro(mensagemErro, cte.Empresa.CNPJ, "", 0, stringConexao, false);
                        }
                    }
                }

                return valorAdicional;
            }
            catch (Exception ex)
            {
                string mensagemErro = "Não foi possível calcular adicional de peso do CTe código " + codigoCTe + " : " + ex;
                //Servicos.Log.TratarErro(mensagemErro);
                //enviarEmailProblemaCTe("infra@multisoftware.com.br", mensagemErro, stringConexao);
                this.LogErro(mensagemErro, "", "", 0, stringConexao, false);
                return 0;
            }
        }

        private decimal FrimesaCalcularAdicionalPesoNFSe(int codigoNFSe, Dominio.Entidades.CargaFrimesa cargaFrimesa, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            try
            {
                Servicos.NFSe serNFSe = new Servicos.NFSe(unitOfWork);

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                decimal valorAdicional = 0;

                if (nfse != null && cargaFrimesa != null)
                {
                    decimal valorFrete = cargaFrimesa.ValorFrete;
                    decimal pesoNFSe = nfse.PesoKG;

                    if (valorFrete > 0 && cargaFrimesa.TipoVeiculo != null && cargaFrimesa.TipoVeiculo.PesoBruto > 0 && pesoNFSe > cargaFrimesa.TipoVeiculo.PesoBruto)
                    {
                        decimal pesoAdicional = pesoNFSe - cargaFrimesa.TipoVeiculo.PesoBruto;
                        valorAdicional = Math.Round(Math.Round((valorFrete / cargaFrimesa.TipoVeiculo.PesoBruto), 4, MidpointRounding.ToEven) * pesoAdicional, 2, MidpointRounding.ToEven);

                        if (!serNFSe.AlterarValorServicoNFSe(nfse, valorFrete + valorAdicional, unitOfWork))
                        {
                            string mensagemErro = nfse != null ? "Empresa: " + nfse.Empresa.CNPJ + " CTe: " + nfse.Numero : string.Empty + " Não foi possível calcular adicional de peso.";
                            //Servicos.Log.TratarErro(mensagemErro);
                            //enviarEmailProblemaCTe("infra@multisoftware.com.br", mensagemErro, stringConexao);
                            this.LogErro(mensagemErro, nfse.Empresa.CNPJ, "", 0, stringConexao, false);
                        }
                    }
                }

                return valorAdicional;
            }
            catch (Exception ex)
            {
                string mensagemErro = "Não foi possível calcular adicional de peso da NFSe código " + codigoNFSe + " : " + ex;
                //Servicos.Log.TratarErro(mensagemErro);
                //enviarEmailProblemaCTe("infra@multisoftware.com.br", mensagemErro, stringConexao);
                this.LogErro(mensagemErro, "", "", 0, stringConexao, false);
                return 0;
            }

        }

        private bool AdicionarCTeNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                if (cte.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    return true;

                string postData = "CodigoCTe=" + cte.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["UriSistemaEmissaoCTe"], "/IntegracaoCTe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool AdicionarNFSeNaFilaDeConsulta(Dominio.Entidades.NFSe nfse)
        {
            try
            {
                string postData = "CodigoNFSe=" + nfse.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["UriSistemaEmissaoCTe"], "/IntegracaoNFSe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private string ExtrairPalavraAgrupador(string obsNFSe, string palavraChave, int tamanhoPalavra)
        {
            if (string.IsNullOrWhiteSpace(obsNFSe) || string.IsNullOrWhiteSpace(palavraChave) || tamanhoPalavra <= 0)
                return string.Empty;

            int posicaoPalavraChave = obsNFSe.IndexOf(palavraChave);
            if (posicaoPalavraChave < 0)
                return string.Empty;
            int inicioPalavraAgrupador = posicaoPalavraChave + palavraChave.Length;

            return obsNFSe.Substring(inicioPalavraAgrupador, tamanhoPalavra);
        }

        private void EmitirCTesAguardandoEmissao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = null;
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarPorStatusEPeriodo(0, new string[] { "M" }, DateTime.Today.AddDays(-1), DateTime.Today);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            for (var i = 0; i < listaCTes.Count; i++)
            {
                configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(0, listaCTes[i].Empresa.Codigo, double.Parse(listaCTes[i].Remetente.CPF_CNPJ));

                if (configuracaoEmissaoEmail == null)
                    configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(0, listaCTes[i].Empresa.Codigo, 0);

                if (listaCTes[i].ValorAReceber > 0 && configuracaoEmissaoEmail != null && configuracaoEmissaoEmail.Emitir == Dominio.Enumeradores.OpcaoSimNao.Sim)
                {
                    if (svcCTe.Emitir(listaCTes[i].Codigo, listaCTes[i].Empresa.Codigo, unitOfWork))
                    {
                        if (listaCTes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(4, listaCTes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, unitOfWork.StringConexao);

                        if (configuracaoEmissaoEmail.GerarMDFe == Dominio.Enumeradores.OpcaoSimNao.Sim)
                        {
                            Repositorio.GerarMDFe repGerarMDFe = new Repositorio.GerarMDFe(unitOfWork);
                            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);
                            List<Dominio.Entidades.VeiculoCTE> velculosCTe = repVeiculoCTe.BuscarPorCTe(listaCTes[i].Empresa.Codigo, listaCTes[i].Codigo);

                            if (velculosCTe != null && velculosCTe.Count > 0)
                            {
                                Dominio.Entidades.GerarMDFe gerarMDFe = new Dominio.Entidades.GerarMDFe();
                                gerarMDFe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;

                                if (gerarMDFe.CTEs == null)
                                    gerarMDFe.CTEs = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                                gerarMDFe.CTEs.Add(listaCTes[i]);

                                repGerarMDFe.Inserir(gerarMDFe);
                            }
                            else
                            {
                                this.LogErro("CT-e " + listaCTes[i].Numero + ": sem veículo para gerar MDFe.", listaCTes[i].Empresa.CNPJ, "", 0, unitOfWork.StringConexao, false);

                                Dominio.Entidades.GerarMDFe gerarMDFe = new Dominio.Entidades.GerarMDFe();
                                gerarMDFe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                                gerarMDFe.Mensagem = "CT - e " + listaCTes[i].Numero + ": sem veículo para gerar MDFe.";
                                if (gerarMDFe.CTEs == null)
                                    gerarMDFe.CTEs = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                                gerarMDFe.CTEs.Add(listaCTes[i]);

                                repGerarMDFe.Inserir(gerarMDFe);
                            }
                        }
                    }
                    else
                    {
                        //Servicos.Log.TratarErro("Importação e-mail - CT-e " + listaCTes[i].Numero + " da transportadora " + listaCTes[i].Empresa.CNPJ + ": ocorreu uma falha e não foi possível emitir.");
                        //enviarEmailProblemaCTe("infra@multisoftware.com.br", " CT-e " + listaCTes[i].Numero + " da transportadora " + listaCTes[i].Empresa.CNPJ + ": ocorreu uma falha e não foi possível emitir.", unitOfWork.StringConexao);
                        this.LogErro("CT-e " + listaCTes[i].Numero + ": ocorreu uma falha e não foi possível emitir.", listaCTes[i].Empresa.CNPJ, "", 0, unitOfWork.StringConexao, false);
                    }
                }
                else
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTes[i].Codigo);
                    if (cte != null)
                    {
                        cte.Status = "S";
                        repCTe.Atualizar(cte);
                    }
                }
            }
        }
    }
}

