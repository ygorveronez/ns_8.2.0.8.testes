using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Servicos.Embarcador.Documentos
{
    public class EnvioDocumentacaoLote : ServicoBase
    {
        public EnvioDocumentacaoLote() : base() { }

        #region Métodos Publicos
        public static void ProcessarNotasSemPDF(Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                if (!(configuracaoTMS.ArmazenarPDFDANFE))
                    return;

                string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos;
                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "DANFE Documentos Emitidos");

                List<int> codigosDocumentacao = repXMLNotaFiscal.BuscarNotasPendentesGeradaoDANFE();
                for (var i = 0; i < codigosDocumentacao.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repXMLNotaFiscal.BuscarPorCodigo(codigosDocumentacao[i]);
                    notaFiscal.XML = Zeus.Embarcador.ZeusNFe.Zeus.RetornarXMLNotaFiscal(notaFiscal, diretorioDocumentosFiscais, unidadeTrabalho);

                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDANFE, notaFiscal.Chave + ".pdf");
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho) && !string.IsNullOrWhiteSpace(notaFiscal.XML))
                    {
                        try
                        {
                            if (notaFiscal.XML.Contains("</nfeProc>"))
                                Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminho, false, false);
                            else if (notaFiscal.XML.Contains("</NFe>"))
                                Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminho, false, true);
                            else
                                Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, notaFiscal.XML, caminho, true, false);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("NFE " + notaFiscal.Chave + " Erro " + ex, "ProcessarNotasSemPDF");
                        }
                    }

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        notaFiscal.GerouPDF = true;
                    else
                        notaFiscal.GerouPDF = false;

                    repXMLNotaFiscal.Atualizar(notaFiscal);

                    unidadeTrabalho.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void ProcessarEnvioDocumentacaoLote(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, string host)
        {
            try
            {
                Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unidadeTrabalho);

                List<long> codigosDocumentacaoImpressao = repEnvioDocumentacaoLote.BuscarCodigosPorSituacao(TipoImpressaoLote.Impressao, SituacaoEnvioDocumentacaoLote.Aguardando, "Codigo", "asc", 0, 3);
                for (var i = 0; i < codigosDocumentacaoImpressao.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote = repEnvioDocumentacaoLote.BuscarPorCodigo(codigosDocumentacaoImpressao[i]);

                    if (!ImpressaoDocumentacaoLote(out string erro, documentacaoLote, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, stringConexaoAdmin, host))
                        throw new Exception(erro);

                    unidadeTrabalho.FlushAndClear();
                }

                List<long> codigosDocumentacao = repEnvioDocumentacaoLote.BuscarCodigosPorSituacao(TipoImpressaoLote.Email, SituacaoEnvioDocumentacaoLote.Aguardando, "Codigo", "asc", 0, 10);
                for (var i = 0; i < codigosDocumentacao.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote = repEnvioDocumentacaoLote.BuscarPorCodigo(codigosDocumentacao[i]);

                    if (!ProcessarEnvioDocumentacaoLote(out string erro, documentacaoLote, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, stringConexaoAdmin, host))
                        throw new Exception(erro);

                    unidadeTrabalho.FlushAndClear();
                }

                ProcessarEnvioAutomaticoDocumentacao(unidadeTrabalho, stringConexao, tipoServicoMultisoftware, stringConexaoAdmin, host);

            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void ProcessarEnvioDocumentacaoFinalizacaoCarga(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, string host)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracao = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();

                if (!configuracao.AtivarEnvioDocumentacaoFinalizacaoCarga)
                    return;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                List<int> codigosCarga = repCarga.BuscarCodigosCargasAguardandoEnvioDocumentacao("Codigo", "asc", 0, 5);
                for (var i = 0; i < codigosCarga.Count; i++)
                {
                    if (!EnviarDocumentacaoFinalizacaoCarga(out string erro, codigosCarga[i], unidadeTrabalho, stringConexao, tipoServicoMultisoftware, stringConexaoAdmin, host))
                        throw new Exception(erro);

                    unidadeTrabalho.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }


        #endregion

        #region Métodos Privados
        private static bool ProcessarEnvioAutomaticoDocumentacao(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, string host)
        {
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unidadeTrabalho);

            IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacao> documentacaoPendente = repEnvioDocumentacaoLote.BuscarDocumentacaoAutomaticaPendente();
            for (int i = 0; i < documentacaoPendente.Count; i++)
            {
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote envioDocumentacaoLote = new Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote()
                {
                    Container = null,
                    DataGeracao = DateTime.Now,
                    EmailInformadoManualmente = string.Empty,
                    EnvioAutomatico = true,
                    FoiAnulado = Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos,
                    FoiSubstituido = Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos,
                    FormaEnvioDocumentacao = FormaEnvioDocumentacao.Padrao,
                    GrupoPessoas = null,
                    ModalEnvioDocumentacao = ModalEnvioDocumentacao.PortaDestino,
                    NotificadoOperador = false,
                    NumeroBooking = string.Empty,
                    NumeroControle = string.Empty,
                    NumeroFiscal = 0,
                    NumeroOS = string.Empty,
                    PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacaoPendente[i].CodigoViagem),
                    ProvedorOS = null,
                    QuantidadeTentativaEnvio = 0,
                    Retorno = string.Empty,
                    Situacao = SituacaoEnvioDocumentacaoLote.Aguardando,
                    SituacaoEnvioDocumentacao = SituacaoEnvioDocumentacao.Todos,
                    TerminalDestino = repTerminal.BuscarPorCodigo(documentacaoPendente[i].CodigoTerminal),
                    TerminalOrigem = null,
                    TipoImpressaoLote = TipoImpressaoLote.Email,
                    Usuario = null
                };
                envioDocumentacaoLote.TiposProposta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();
                envioDocumentacaoLote.TiposProposta.Add(TipoPropostaMultimodal.CargaFechada);
                envioDocumentacaoLote.TiposProposta.Add(TipoPropostaMultimodal.CargaFracionada);

                repEnvioDocumentacaoLote.Inserir(envioDocumentacaoLote);

                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule = repSchedule.BuscarPorCodigo(documentacaoPendente[i].CodigoSchedule);
                schedule.EnviouDocumentacaoAutomatica = true;
                repSchedule.Atualizar(schedule);

                unidadeTrabalho.CommitChanges();

                unidadeTrabalho.FlushAndClear();
            }

            return true;
        }

        private static bool ImpressaoDocumentacaoLote(out string erro, Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, string host)
        {
            Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

            int codigoUsuario = documentacaoLote.Usuario?.Codigo ?? 0;
            string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos;
            string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoRelatorios;
            string caminhoArquivosAnexos = $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" })}\\";

            List<int> codigosCTes = new List<int>();
            if (documentacaoLote.CTes != null && documentacaoLote.CTes.Count > 0)
                codigosCTes = documentacaoLote.CTes.Select(c => c.Codigo).ToList();
            else
                codigosCTes = RetornaTodosCodigosConhecimentos(documentacaoLote, unidadeTrabalho);


            Zeus.Embarcador.ZeusNFe.Zeus.GerarPDFTodosDocumentos(documentacaoLote.Codigo, codigosCTes, stringConexao, documentacaoLote.Usuario.Codigo, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, "Cargas/ImpressaoLoteCarga", caminhoArquivosAnexos);

            erro = string.Empty;
            return true;
        }

        private static bool ProcessarEnvioDocumentacaoLote(out string erro, Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, string host)
        {
            Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote repEnvioDocumentacaoLote = new Repositorio.Embarcador.Documentos.EnvioDocumentacaoLote(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

            List<double> cnpjTomadores = new List<double>();
            List<string> numerosBooking = new List<string>();
            List<int> codigosContainer = new List<int>();
            List<string> codigosEmpresaDestino = new List<string>();
            List<int> codigosViagem = new List<int>();
            List<int> codigosTerminalDestino = new List<int>();
            List<double> cnpjProvedores = new List<double>();

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = null;
            List<int> codigosCTes = new List<int>();
            if (documentacaoLote.CTes != null && documentacaoLote.CTes.Count > 0)
                codigosCTes = documentacaoLote.CTes.Select(c => c.Codigo).ToList();
            else
                codigosCTes = RetornaTodosCodigosConhecimentos(documentacaoLote, unidadeTrabalho);

            ctes = repCTe.BuscarPorCodigo(codigosCTes);

            if (documentacaoLote.ModalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortoDestino && ctes != null && ctes.Count > 0)
            {
                cnpjTomadores = ctes.Where(c => c.TomadorPagador != null).Select(c => c.TomadorPagador.Cliente.CPF_CNPJ).Distinct().ToList();
                numerosBooking = ctes.Where(c => c.NumeroBooking != null && c.NumeroBooking != "").Select(c => c.NumeroBooking).Distinct().ToList();
                codigosViagem = ctes.Where(c => c.Viagem != null).Select(c => c.Viagem.Codigo).Distinct().ToList();
                codigosTerminalDestino = ctes.Where(c => c.TerminalDestino != null).Select(c => c.TerminalDestino.Codigo).Distinct().ToList();
                codigosContainer = repCTe.BuscarCodigosContaineres(codigosCTes);

                bool retorno = false;
                foreach (var cnpjTomador in cnpjTomadores)
                {
                    retorno = ConfigurarEnviarEmailPessoa(documentacaoLote, documentacaoLote.ModalEnvioDocumentacao, !string.IsNullOrEmpty(documentacaoLote.EmailInformadoManualmente), documentacaoLote.EmailInformadoManualmente, documentacaoLote.FormaEnvioDocumentacao, cnpjTomador, codigosViagem, codigosTerminalDestino, null, numerosBooking, codigosContainer, ctes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Tomador, unidadeTrabalho, "", adminStringConexao, host, stringConexao);
                    if (!retorno)
                        break;
                }
                if (retorno)
                {
                    documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Finalizado;
                    documentacaoLote.Retorno = "Sucesso";
                    if (documentacaoLote.Usuario != null)
                        serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Cargas/ImpressaoLoteCarga", string.Format(Localization.Resources.Zeus.DocumentacaoLote.DocumentacaoOrigemGeradoSucesso, documentacaoLote.PedidoViagemNavio?.Descricao ?? "", documentacaoLote.TerminalOrigem?.Descricao ?? ""), IconesNotificacao.sucesso, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                }
                else
                {
                    documentacaoLote.QuantidadeTentativaEnvio += 1;
                    if (documentacaoLote.EnvioAutomatico && documentacaoLote.QuantidadeTentativaEnvio < 30)
                    {
                        documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Aguardando;
                        EnviarEmailProblemaEnvioAutomatico("Não foi possível gerar todos os PDFs", documentacaoLote, documentacaoLote.TerminalDestino.Empresa, unidadeTrabalho);
                    }
                    else
                        documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Falha;
                    documentacaoLote.Retorno = "Não foi possível gerar todos os PDFs";
                    if (documentacaoLote.Usuario != null)
                        serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Cargas/ImpressaoLoteCarga", string.Format(Localization.Resources.Zeus.DocumentacaoLote.DocumentacaoOrigemNaoFoiPossivelGerar, documentacaoLote.PedidoViagemNavio?.Descricao ?? "", documentacaoLote.TerminalOrigem?.Descricao ?? ""), IconesNotificacao.falha, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                }
                repEnvioDocumentacaoLote.Atualizar(documentacaoLote);
            }
            else if (documentacaoLote.ModalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortaDestino)
            {
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDestinoPorta = repCTe.BuscarPorCodigoEDestinoPorta(codigosCTes);

                if (ctesDestinoPorta != null && ctesDestinoPorta.Count > 0)
                {
                    numerosBooking = ctesDestinoPorta.Where(c => c.NumeroBooking != null && c.NumeroBooking != "").Select(c => c.NumeroBooking).Distinct().ToList();
                    codigosViagem = ctesDestinoPorta.Where(c => c.Viagem != null).Select(c => c.Viagem.Codigo).Distinct().ToList();
                    codigosTerminalDestino = ctesDestinoPorta.Where(c => c.TerminalDestino != null).Select(c => c.TerminalDestino.Codigo).Distinct().ToList();
                    codigosContainer = repCTe.BuscarCodigosContaineres(codigosCTes);
                    cnpjProvedores = ctesDestinoPorta.Where(c => c.ClienteProvedorOS != null).Select(c => c.ClienteProvedorOS.CPF_CNPJ).Distinct().ToList();
                    codigosEmpresaDestino = ctesDestinoPorta.Where(c => c.TerminalDestino != null && c.TerminalDestino.Empresa != null).Select(c => c.TerminalDestino.Empresa.CNPJ).Distinct().ToList();

                    bool retorno = false;

                    foreach (var cnpjProvedor in cnpjProvedores)
                    {
                        retorno = ConfigurarEnviarEmailPessoa(documentacaoLote, documentacaoLote.ModalEnvioDocumentacao, !string.IsNullOrEmpty(documentacaoLote.EmailInformadoManualmente), documentacaoLote.EmailInformadoManualmente, documentacaoLote.FormaEnvioDocumentacao, cnpjProvedor, codigosViagem, codigosTerminalDestino, cnpjTomadores, numerosBooking, codigosContainer, ctesDestinoPorta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.ProvedorOS, unidadeTrabalho, "", adminStringConexao, host, stringConexao);
                        if (!retorno)
                            break;
                    }
                    foreach (var codigoEmpresa in codigosEmpresaDestino)
                    {
                        double.TryParse(codigoEmpresa, out double cnpjEmpresa);
                        if (cnpjEmpresa > 0)
                        {
                            retorno = ConfigurarEnviarEmailPessoa(documentacaoLote, documentacaoLote.ModalEnvioDocumentacao, !string.IsNullOrEmpty(documentacaoLote.EmailInformadoManualmente), documentacaoLote.EmailInformadoManualmente, documentacaoLote.FormaEnvioDocumentacao, cnpjEmpresa, codigosViagem, codigosTerminalDestino, cnpjTomadores, numerosBooking, codigosContainer, ctesDestinoPorta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Empresa, unidadeTrabalho, codigoEmpresa, adminStringConexao, host, stringConexao);
                            if (!retorno)
                                break;
                        }
                    }

                    if (retorno)
                    {
                        documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Finalizado;
                        documentacaoLote.Retorno = "Sucesso";
                        if (documentacaoLote.Usuario != null)
                            serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Cargas/ImpressaoLoteCarga", string.Format(Localization.Resources.Zeus.DocumentacaoLote.DocumentacaoOrigemGeradoSucesso, documentacaoLote.PedidoViagemNavio?.Descricao ?? "", documentacaoLote.TerminalOrigem?.Descricao ?? ""), IconesNotificacao.sucesso, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                    }
                    else
                    {
                        documentacaoLote.QuantidadeTentativaEnvio += 1;
                        if (documentacaoLote.EnvioAutomatico && documentacaoLote.QuantidadeTentativaEnvio < 30)
                        {
                            documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Aguardando;
                            EnviarEmailProblemaEnvioAutomatico("Não foi possível gerar todos os PDFs", documentacaoLote, documentacaoLote.TerminalDestino.Empresa, unidadeTrabalho);
                        }
                        else
                            documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Falha;
                        documentacaoLote.Retorno = "Não foi possível gerar todos os PDFs";
                        if (documentacaoLote.Usuario != null)
                            serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Cargas/ImpressaoLoteCarga", string.Format(Localization.Resources.Zeus.DocumentacaoLote.DocumentacaoOrigemNaoFoiPossivelGerar, documentacaoLote.PedidoViagemNavio?.Descricao ?? "", documentacaoLote.TerminalOrigem?.Descricao ?? ""), IconesNotificacao.falha, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                    }
                    repEnvioDocumentacaoLote.Atualizar(documentacaoLote);
                }
                else
                {
                    documentacaoLote.QuantidadeTentativaEnvio += 1;
                    if (documentacaoLote.EnvioAutomatico && documentacaoLote.QuantidadeTentativaEnvio < 3)
                    {
                        documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Aguardando;
                        EnviarEmailProblemaEnvioAutomatico("Nenhum CT-e localizado.", documentacaoLote, documentacaoLote.TerminalDestino.Empresa, unidadeTrabalho);
                    }
                    else
                        documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Falha;

                    documentacaoLote.Retorno = "Nenhum CT-e selecionado.";
                    if (documentacaoLote.Usuario != null)
                        serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Cargas/ImpressaoLoteCarga", string.Format(Localization.Resources.Zeus.DocumentacaoLote.DocumentacaoOrigemNaoFoiPossivelGerar, documentacaoLote.PedidoViagemNavio?.Descricao ?? "", documentacaoLote.TerminalOrigem?.Descricao ?? ""), IconesNotificacao.falha, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                    repEnvioDocumentacaoLote.Atualizar(documentacaoLote);
                }
            }
            else
            {
                documentacaoLote.QuantidadeTentativaEnvio += 1;
                if (documentacaoLote.EnvioAutomatico && documentacaoLote.QuantidadeTentativaEnvio < 3)
                {
                    documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Aguardando;
                    EnviarEmailProblemaEnvioAutomatico("Nenhum CT-e localizado.", documentacaoLote, documentacaoLote.TerminalDestino.Empresa, unidadeTrabalho);
                }
                else
                    documentacaoLote.Situacao = SituacaoEnvioDocumentacaoLote.Falha;
                documentacaoLote.Retorno = "Nenhum CT-e selecionado.";
                if (documentacaoLote.Usuario != null)
                    serNotificacao.GerarNotificacao(documentacaoLote.Usuario, 0, "Cargas/ImpressaoLoteCarga", string.Format(Localization.Resources.Zeus.DocumentacaoLote.DocumentacaoOrigemNaoFoiPossivelGerar, documentacaoLote.PedidoViagemNavio?.Descricao ?? "", documentacaoLote.TerminalOrigem?.Descricao ?? ""), IconesNotificacao.falha, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                repEnvioDocumentacaoLote.Atualizar(documentacaoLote);
            }
            erro = string.Empty;
            return true;
        }

        private static List<int> RetornaTodosCodigosConhecimentos(Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            double provedorOS = documentacaoLote.ProvedorOS?.CPF_CNPJ ?? 0;

            string numeroBooking = documentacaoLote.NumeroBooking;
            string numeroOS = documentacaoLote.NumeroOS;
            string numeroControle = documentacaoLote.NumeroControle;

            int numeroFiscal = documentacaoLote.NumeroFiscal;

            int codigoContainer = documentacaoLote.Container?.Codigo ?? 0;
            int codigoPedidoViagemDirecao = documentacaoLote.PedidoViagemNavio?.Codigo ?? 0;
            int codigoTerminalOrigem = documentacaoLote.TerminalOrigem?.Codigo ?? 0;
            int codigoTerminalDestino = documentacaoLote.TerminalDestino?.Codigo ?? 0;
            int codigoGrupoPessoa = documentacaoLote.GrupoPessoas?.Codigo ?? 0;

            Dominio.Enumeradores.OpcaoSimNaoPesquisa foiAnulado = documentacaoLote.FoiAnulado;
            Dominio.Enumeradores.OpcaoSimNaoPesquisa foiSubstituido = documentacaoLote.FoiSubstituido;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao modalEnvioDocumentacao = documentacaoLote.ModalEnvioDocumentacao;
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal> tipoServico = documentacaoLote.TiposServicos?.ToList() ?? null;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacao situacaoEnvio = documentacaoLote.SituacaoEnvioDocumentacao;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = documentacaoLote.TiposProposta?.ToList() ?? null;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            List<int> codigosCTesTransbordo = new List<int>();
            List<int> codigosCTes = new List<int>();
            codigosCTesTransbordo = repCarga.ConsultarCodigosTransbordoConhecimentosParaImpressaoLote(modalEnvioDocumentacao, foiAnulado, foiSubstituido, tipoServico, situacaoEnvio, provedorOS, numeroBooking, numeroOS, numeroControle, numeroFiscal, codigoContainer, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, codigoGrupoPessoa, null, tipoPropostaMultimodal);
            codigosCTes = repCarga.ConsultarCodigosConhecimentosParaImpressaoLote(modalEnvioDocumentacao, foiAnulado, foiSubstituido, tipoServico, situacaoEnvio, provedorOS, numeroBooking, numeroOS, numeroControle, numeroFiscal, codigoContainer, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, codigoGrupoPessoa, null, tipoPropostaMultimodal);
            codigosCTes.AddRange(codigosCTesTransbordo);

            return codigosCTes.Distinct().ToList();
        }

        private static bool ConfigurarEnviarEmailPessoa(Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao modalEnvioDocumentacao, bool informarEmailEnvio, string emailEnvio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao formaEnvioDocumentacao, double cnpjPessoa, List<int> codigosViagem, List<int> codigosTerminalDestino, List<double> cnpjsTomadores, List<string> numerosBooking, List<int> codigosContainer, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao entidade, Repositorio.UnitOfWork unitOfWork, string cnpjEmpresa, string adminStringConexao, string host, string stringConexao)
        {
            if (cnpjPessoa == 0)
                return false;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = repClienteURLAcesso.BuscarPorURL(host);

            int codigoUsuario = documentacaoLote.Usuario?.Codigo ?? 0;
            string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos;
            string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoRelatorios;
            string caminhoArquivosAnexos = $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" })}\\";

            string emailsEnvio = "";
            string assuntoEmail = "";
            string corpoEmail = "";
            string urlAcesso = clienteAcesso.URLAcesso;

            Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
            if (pessoa == null)
                return false;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao tipoAgrupamentoEnvio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao.Nenhum;
            if (modalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortaDestino)
            {
                if (pessoa.TipoAgrupamentoEnvioDocumentacao.HasValue)
                    tipoAgrupamentoEnvio = pessoa.TipoAgrupamentoEnvioDocumentacaoPorta.Value;
                else if (pessoa.GrupoPessoas != null && pessoa.GrupoPessoas.TipoAgrupamentoEnvioDocumentacaoPorta.HasValue)
                    tipoAgrupamentoEnvio = pessoa.GrupoPessoas.TipoAgrupamentoEnvioDocumentacaoPorta.Value;
            }
            else
            {
                if (pessoa.TipoAgrupamentoEnvioDocumentacao.HasValue)
                    tipoAgrupamentoEnvio = pessoa.TipoAgrupamentoEnvioDocumentacao.Value;
                else if (pessoa.GrupoPessoas != null && pessoa.GrupoPessoas.TipoAgrupamentoEnvioDocumentacao.HasValue)
                    tipoAgrupamentoEnvio = pessoa.GrupoPessoas.TipoAgrupamentoEnvioDocumentacao.Value;
            }

            if (pessoa.NaoUsarConfiguracaoFaturaGrupo && !string.IsNullOrWhiteSpace(pessoa.EmailEnvioDocumentacao) && (!string.IsNullOrWhiteSpace(pessoa.EmailEnvioDocumentacaoPorta) || pessoa.FormaEnvioDocumentacaoPorta.HasValue || pessoa.FormaEnvioDocumentacao.HasValue))
            {
                if (formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.Padrao && modalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortaDestino && pessoa.FormaEnvioDocumentacaoPorta.HasValue)
                {
                    if (pessoa.FormaEnvioDocumentacaoPorta.HasValue)
                        formaEnvioDocumentacao = pessoa.FormaEnvioDocumentacaoPorta.Value;
                    if (!string.IsNullOrWhiteSpace(pessoa.EmailEnvioDocumentacaoPorta))
                        emailsEnvio = pessoa.EmailEnvioDocumentacaoPorta;
                }
                else if (formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.Padrao && modalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortoDestino && pessoa.FormaEnvioDocumentacao.HasValue)
                {
                    if (pessoa.FormaEnvioDocumentacao.HasValue)
                        formaEnvioDocumentacao = pessoa.FormaEnvioDocumentacao.Value;
                    if (!string.IsNullOrWhiteSpace(pessoa.EmailEnvioDocumentacao))
                        emailsEnvio = pessoa.EmailEnvioDocumentacao;
                }
            }
            else if (pessoa.GrupoPessoas != null && (!string.IsNullOrWhiteSpace(pessoa.GrupoPessoas?.EmailEnvioDocumentacao) || !string.IsNullOrWhiteSpace(pessoa.GrupoPessoas?.EmailEnvioDocumentacaoPorta) || pessoa.GrupoPessoas.FormaEnvioDocumentacaoPorta.HasValue || pessoa.GrupoPessoas.FormaEnvioDocumentacao.HasValue))
            {
                if (formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.Padrao && modalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortaDestino && pessoa.GrupoPessoas.FormaEnvioDocumentacaoPorta.HasValue)
                {
                    if (pessoa.GrupoPessoas.FormaEnvioDocumentacaoPorta.HasValue)
                        formaEnvioDocumentacao = pessoa.GrupoPessoas.FormaEnvioDocumentacaoPorta.Value;
                    if (!string.IsNullOrWhiteSpace(pessoa.GrupoPessoas.EmailEnvioDocumentacaoPorta))
                        emailsEnvio = pessoa.GrupoPessoas.EmailEnvioDocumentacaoPorta;
                }
                else if (formaEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao.Padrao && modalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortoDestino && pessoa.GrupoPessoas.FormaEnvioDocumentacao.HasValue)
                {
                    if (pessoa.GrupoPessoas.FormaEnvioDocumentacao.HasValue)
                        formaEnvioDocumentacao = pessoa.GrupoPessoas.FormaEnvioDocumentacao.Value;
                    if (!string.IsNullOrWhiteSpace(pessoa.GrupoPessoas.EmailEnvioDocumentacao))
                        emailsEnvio = pessoa.GrupoPessoas.EmailEnvioDocumentacao;
                }
            }

            if (informarEmailEnvio && !string.IsNullOrWhiteSpace(emailEnvio))
                emailsEnvio = emailEnvio;

            if (modalEnvioDocumentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalEnvioDocumentacao.PortaDestino)
            {
                if (!string.IsNullOrWhiteSpace(pessoa.AssuntoDocumentacaoPorta))
                    assuntoEmail = pessoa.AssuntoDocumentacaoPorta;
                else if (pessoa.GrupoPessoas != null && !string.IsNullOrWhiteSpace(pessoa.GrupoPessoas?.AssuntoDocumentacaoPorta))
                    assuntoEmail = pessoa.GrupoPessoas.AssuntoDocumentacaoPorta;

                if (!string.IsNullOrWhiteSpace(pessoa.CorpoEmailDocumentacaoPorta))
                    corpoEmail = pessoa.CorpoEmailDocumentacaoPorta;
                else if (pessoa.GrupoPessoas != null && !string.IsNullOrWhiteSpace(pessoa.GrupoPessoas?.CorpoEmailDocumentacaoPorta))
                    corpoEmail = pessoa.GrupoPessoas.CorpoEmailDocumentacaoPorta;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(pessoa.AssuntoDocumentacao))
                    assuntoEmail = pessoa.AssuntoDocumentacao;
                else if (pessoa.GrupoPessoas != null && !string.IsNullOrWhiteSpace(pessoa.GrupoPessoas?.AssuntoDocumentacao))
                    assuntoEmail = pessoa.GrupoPessoas.AssuntoDocumentacao;

                if (!string.IsNullOrWhiteSpace(pessoa.CorpoEmailDocumentacao))
                    corpoEmail = pessoa.CorpoEmailDocumentacao;
                else if (pessoa.GrupoPessoas != null && !string.IsNullOrWhiteSpace(pessoa.GrupoPessoas?.CorpoEmailDocumentacao))
                    corpoEmail = pessoa.GrupoPessoas.CorpoEmailDocumentacao;
            }

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesFiltrados = null;
            if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Empresa)
            {
                ctesFiltrados = ctes.Where(o => o.TerminalDestino != null && o.TerminalDestino.Empresa != null && o.TerminalDestino.Empresa.CNPJ == cnpjEmpresa).ToList();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresa);
                if (empresa != null && empresa.StatusEmail == "A" && !string.IsNullOrWhiteSpace(empresa.Email))
                    emailsEnvio += (!string.IsNullOrWhiteSpace(emailsEnvio) ? ";" : "") + empresa.Email;
                if (empresa != null && empresa.StatusEmailAdministrativo == "A" && !string.IsNullOrWhiteSpace(empresa.EmailAdministrativo))
                    emailsEnvio += (!string.IsNullOrWhiteSpace(emailsEnvio) ? ";" : "") + empresa.EmailAdministrativo;
            }
            else if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Tomador)
                ctesFiltrados = ctes.Where(o => o.TomadorPagador.Cliente.CPF_CNPJ == cnpjPessoa).ToList();
            else if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Destinatario)
                ctesFiltrados = ctes.Where(o => o.Destinatario.Cliente.CPF_CNPJ == cnpjPessoa).ToList();
            else if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.ProvedorOS)
                ctesFiltrados = ctes.Where(o => o.ClienteProvedorOS != null && o.ClienteProvedorOS.CPF_CNPJ == cnpjPessoa).ToList();

            bool retornoEnviarTodos = true;
            if (tipoAgrupamentoEnvio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao.CTeVVDTerminalDestino && codigosViagem != null && codigosViagem.Count > 0 && codigosTerminalDestino != null && codigosTerminalDestino.Count > 0)
            {
                foreach (var codigoViagem in codigosViagem)
                {
                    foreach (var codigoTerminal in codigosTerminalDestino)
                    {
                        var ctesEnvio = ctesFiltrados.Where(o => o.Viagem.Codigo == codigoViagem && o.TerminalDestino.Codigo == codigoTerminal).ToList();
                        if (ctesEnvio != null && ctesEnvio.Count > 0)
                        {
                            bool retornoEnviar = EnviarEmailLoteCTE(documentacaoLote, urlAcesso, formaEnvioDocumentacao, ctesEnvio, pessoa, emailsEnvio, assuntoEmail, corpoEmail, codigoUsuario, stringConexao, caminhoArquivos, diretorioDocumentosFiscais, caminhoRelatorios, entidade, unitOfWork, caminhoArquivosAnexos);
                            if (!retornoEnviar)
                                retornoEnviarTodos = false;
                        }
                    }
                }
            }
            else if (tipoAgrupamentoEnvio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao.BookingVVDTerminalDestino && numerosBooking != null && numerosBooking.Count > 0 && codigosViagem != null && codigosViagem.Count > 0 && codigosTerminalDestino != null && codigosTerminalDestino.Count > 0)
            {
                foreach (var numeroBooking in numerosBooking)
                {
                    foreach (var codigoViagem in codigosViagem)
                    {
                        foreach (var codigoTerminal in codigosTerminalDestino)
                        {
                            var ctesEnvio = ctesFiltrados.Where(o => o.NumeroBooking == numeroBooking && o.Viagem.Codigo == codigoViagem && o.TerminalDestino.Codigo == codigoTerminal).ToList();
                            if (ctesEnvio != null && ctesEnvio.Count > 0)
                            {
                                bool retornoEnviar = EnviarEmailLoteCTE(documentacaoLote, urlAcesso, formaEnvioDocumentacao, ctesEnvio, pessoa, emailsEnvio, assuntoEmail, corpoEmail, codigoUsuario, stringConexao, caminhoArquivos, diretorioDocumentosFiscais, caminhoRelatorios, entidade, unitOfWork, caminhoArquivosAnexos);
                                if (!retornoEnviar)
                                    retornoEnviarTodos = false;
                            }
                        }
                    }
                }
            }
            else if (tipoAgrupamentoEnvio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao.ContainerVVDTerminalDestino && codigosContainer != null && codigosContainer.Count > 0 && codigosViagem != null && codigosViagem.Count > 0 && codigosTerminalDestino != null && codigosTerminalDestino.Count > 0)
            {
                foreach (var codigoContainer in codigosContainer)
                {
                    foreach (var codigoViagem in codigosViagem)
                    {
                        foreach (var codigoTerminal in codigosTerminalDestino)
                        {
                            var ctesEnvio = ctesFiltrados.Where(o => o.Containers.Any(c => c.Container.Codigo == codigoContainer) && o.Viagem.Codigo == codigoViagem && o.TerminalDestino.Codigo == codigoTerminal).ToList();
                            if (ctesEnvio != null && ctesEnvio.Count > 0)
                            {
                                bool retornoEnviar = EnviarEmailLoteCTE(documentacaoLote, urlAcesso, formaEnvioDocumentacao, ctesEnvio, pessoa, emailsEnvio, assuntoEmail, corpoEmail, codigoUsuario, stringConexao, caminhoArquivos, diretorioDocumentosFiscais, caminhoRelatorios, entidade, unitOfWork, caminhoArquivosAnexos);
                                if (!retornoEnviar)
                                    retornoEnviarTodos = false;
                            }
                        }
                    }
                }
            }
            else if (tipoAgrupamentoEnvio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao.VVDTerminalDestino && codigosViagem != null && codigosViagem.Count > 0 && codigosTerminalDestino != null && codigosTerminalDestino.Count > 0)
            {
                foreach (var codigoViagem in codigosViagem)
                {
                    foreach (var codigoTerminal in codigosTerminalDestino)
                    {
                        var ctesEnvio = ctesFiltrados.Where(o => o.Viagem.Codigo == codigoViagem && o.TerminalDestino.Codigo == codigoTerminal).ToList();
                        if (ctesEnvio != null)
                        {
                            bool retornoEnviar = EnviarEmailLoteCTE(documentacaoLote, urlAcesso, formaEnvioDocumentacao, ctesEnvio, pessoa, emailsEnvio, assuntoEmail, corpoEmail, codigoUsuario, stringConexao, caminhoArquivos, diretorioDocumentosFiscais, caminhoRelatorios, entidade, unitOfWork, caminhoArquivosAnexos);
                            if (!retornoEnviar)
                                retornoEnviarTodos = false;
                        }
                    }
                }
            }
            else
            {
                bool retornoEnviar = EnviarEmailLoteCTE(documentacaoLote, urlAcesso, formaEnvioDocumentacao, ctesFiltrados, pessoa, emailsEnvio, assuntoEmail, corpoEmail, codigoUsuario, stringConexao, caminhoArquivos, diretorioDocumentosFiscais, caminhoRelatorios, entidade, unitOfWork, caminhoArquivosAnexos);
                if (!retornoEnviar)
                    retornoEnviarTodos = false;
            }
            unitOfWorkAdmin.Dispose();
            return retornoEnviarTodos;
        }

        private static bool EnviarEmailLoteCTE(Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote, string urlAcesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao formaEnvioDocumentacao, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesFiltrados, Dominio.Entidades.Cliente pessoa, string emailsEnvio, string assuntoEmail, string corpoEmail, int codigoUsuario, string stringConexao, string caminhoArquivos, string diretorioDocumentosFiscais, string caminhoRelatorios, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao entidade, Repositorio.UnitOfWork unitOfWork, string caminhoArquivosAnexos)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            List<int> codigosCTesEnvio = new List<int>();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null;
            if (documentacaoLote.Usuario != null)
            {
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    Usuario = documentacaoLote.Usuario
                };
            }
            else
            {
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    Empresa = documentacaoLote.TerminalDestino.Empresa
                };
            }

            if (ctesFiltrados != null && ctesFiltrados.Count > 0 && !string.IsNullOrWhiteSpace(emailsEnvio))
            {
                if (documentacaoLote.EnvioAutomatico && ctesFiltrados.Any(c => c.DocumentacaoEnviadaAutomatica == true) && !ctesFiltrados.Any(c => c.DocumentacaoEnviadaAutomatica == null) && !ctesFiltrados.Any(c => c.DocumentacaoEnviadaAutomatica == false))
                    return true;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasCTe = repCTe.BuscarXMLNotaFiscalPorChaves(ctesFiltrados.Select(c => c.Chave).ToList());
                List<Dominio.Entidades.Embarcador.Pedidos.Container> containeres = repContainerCTE.BuscarContainerPorCTe(ctesFiltrados.Select(c => c.Codigo).ToList());
                if (!string.IsNullOrWhiteSpace(assuntoEmail))
                {
                    assuntoEmail = assuntoEmail.Replace("#Viagem", string.Join(", ", ctesFiltrados.Where(o => o.Viagem != null).Select(o => o.Viagem.Descricao).Distinct().ToList()));
                    assuntoEmail = assuntoEmail.Replace("#Remetente", string.Join(", ", ctesFiltrados.Where(o => o.Remetente != null).Select(o => o.Remetente.Descricao).Distinct().ToList()));
                    assuntoEmail = assuntoEmail.Replace("#Destinatario", string.Join(", ", ctesFiltrados.Where(o => o.Destinatario != null).Select(o => o.Destinatario.Descricao).Distinct().ToList()));
                    assuntoEmail = assuntoEmail.Replace("#Booking", string.Join(", ", ctesFiltrados.Where(o => o.NumeroBooking != null).Select(o => o.NumeroBooking).Distinct().ToList()));
                    assuntoEmail = assuntoEmail.Replace("#PortoOrigem", string.Join(", ", ctesFiltrados.Where(o => o.PortoOrigem != null).Select(o => o.PortoOrigem.Descricao).Distinct().ToList()));
                    assuntoEmail = assuntoEmail.Replace("#PortoDestino", string.Join(", ", ctesFiltrados.Where(o => o.PortoDestino != null).Select(o => o.PortoDestino.Descricao).Distinct().ToList()));
                    if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Tomador)
                        assuntoEmail = assuntoEmail.Replace("#Tomador", pessoa.NomeCNPJ);
                    else
                        assuntoEmail = assuntoEmail.Replace("#Tomador", string.Join(", ", ctesFiltrados.Where(o => o.TomadorPagador != null).Select(o => o.TomadorPagador.Descricao).Distinct().ToList()));
                    if (notasCTe != null && notasCTe.Count > 0)
                        assuntoEmail = assuntoEmail.Replace("#NumeroNFCliente", string.Join(", ", notasCTe.Select(o => o.Numero).Distinct().ToList()));

                    assuntoEmail = assuntoEmail.Replace("#NumeroFiscal", string.Join(", ", ctesFiltrados.Select(o => o.Numero).Distinct().ToList()));
                    if (containeres != null && containeres.Count > 0)
                        assuntoEmail = assuntoEmail.Replace("#NumeroContainer", string.Join(", ", containeres.Select(o => o.Numero).Distinct().ToList()));
                    assuntoEmail = assuntoEmail.Replace("#NumeroControleSVM", string.Join(", ", ctesFiltrados.Where(o => o.NumeroControleSVM != null).Select(o => o.NumeroControleSVM).Distinct().ToList()));
                }

                if (!string.IsNullOrWhiteSpace(corpoEmail))
                {
                    corpoEmail = corpoEmail.Replace("#QuebraLinha", "<br/>");
                    corpoEmail = corpoEmail.Replace("#NumeroFiscal", string.Join(", ", ctesFiltrados.Select(o => o.Numero).Distinct().ToList()));
                    corpoEmail = corpoEmail.Replace("#Viagem", string.Join(", ", ctesFiltrados.Where(o => o.Viagem != null).Select(o => o.Viagem.Descricao).Distinct().ToList()));
                    corpoEmail = corpoEmail.Replace("#Remetente", string.Join(", ", ctesFiltrados.Where(o => o.Remetente != null).Select(o => o.Remetente.Descricao).Distinct().ToList()));
                    corpoEmail = corpoEmail.Replace("#Destinatario", string.Join(", ", ctesFiltrados.Where(o => o.Destinatario != null).Select(o => o.Destinatario.Descricao).Distinct().ToList()));
                    corpoEmail = corpoEmail.Replace("#Booking", string.Join(", ", ctesFiltrados.Where(o => o.NumeroBooking != null).Select(o => o.NumeroBooking).Distinct().ToList()));
                    corpoEmail = corpoEmail.Replace("#PortoOrigem", string.Join(", ", ctesFiltrados.Where(o => o.PortoOrigem != null).Select(o => o.PortoOrigem.Descricao).Distinct().ToList()));
                    corpoEmail = corpoEmail.Replace("#PortoDestino", string.Join(", ", ctesFiltrados.Where(o => o.PortoDestino != null).Select(o => o.PortoDestino.Descricao).Distinct().ToList()));
                    if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Tomador)
                        corpoEmail = corpoEmail.Replace("#Tomador", pessoa.NomeCNPJ);
                    else
                        corpoEmail = corpoEmail.Replace("#Tomador", string.Join(", ", ctesFiltrados.Where(o => o.TomadorPagador != null).Select(o => o.TomadorPagador.Descricao).Distinct().ToList()));
                    if (notasCTe != null && notasCTe.Count > 0)
                    {
                        corpoEmail = corpoEmail.Replace("#NumeroNFCliente", string.Join(", ", notasCTe.Select(o => o.Numero).Distinct().ToList()));
                        corpoEmail = corpoEmail.Replace("#NumeroControleCliente", string.Join(", ", notasCTe.Select(o => o.NumeroControleCliente).Distinct().ToList()));
                    }
                    if (containeres != null && containeres.Count > 0)
                        corpoEmail = corpoEmail.Replace("#NumeroContainer", string.Join(", ", containeres.Select(o => o.Numero).Distinct().ToList()));
                    corpoEmail = corpoEmail.Replace("#NumeroControleSVM", string.Join(", ", ctesFiltrados.Where(o => o.NumeroControleSVM != null).Select(o => o.NumeroControleSVM).Distinct().ToList()));
                }
                string msgAuditoria = "";
                if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Empresa)
                    msgAuditoria = "Envio o PDF em lote para a Empresa. E-mail(s): " + emailsEnvio;
                else if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Tomador)
                    msgAuditoria = "Envio o PDF em lote para o Tomador. E-mail(s): " + emailsEnvio;
                else if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.Destinatario)
                    msgAuditoria = "Envio o PDF em lote para o Destinatário. E-mail(s): " + emailsEnvio;
                else if (entidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EntidadeEnvioDocumentacao.ProvedorOS)
                    msgAuditoria = "Envio o PDF em lote para o Provedor da O.S. E-mail(s): " + emailsEnvio;

                if (documentacaoLote.EnvioAutomatico)
                    msgAuditoria = "Envio Automático " + msgAuditoria;
                else if (documentacaoLote.Usuario != null)
                    msgAuditoria = "Operador: " + documentacaoLote.Usuario.Nome + " " + msgAuditoria;

                string nomeArquivo = string.Join(", ", ctesFiltrados.Where(o => o.Viagem != null).Select(o => o.Viagem.Descricao).Distinct().ToList()) + " - ";
                nomeArquivo += string.Join(", ", ctesFiltrados.Where(o => o.TomadorPagador != null).Select(o => o.TomadorPagador.Descricao).Distinct().ToList());

                foreach (var cte in ctesFiltrados)
                    codigosCTesEnvio.Add(cte.Codigo);

                List<Dominio.ObjetosDeValor.NFSe.DocumentoRetornoPDFZeus> retornoEnviarEmail = Zeus.Embarcador.ZeusNFe.Zeus.EnviarEmailPDFTodosDocumentos(auditado, out string msgRetorno, urlAcesso, formaEnvioDocumentacao, codigosCTesEnvio, stringConexao, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, emailsEnvio, assuntoEmail, corpoEmail, nomeArquivo, caminhoArquivosAnexos, unitOfWork, false, false, null, "");
                bool retorno = retornoEnviarEmail.Any(obj => obj.Sucesso);

                if (!string.IsNullOrWhiteSpace(msgRetorno))
                {
                    if (documentacaoLote.EnvioAutomatico)
                        msgRetorno = "Envio Automático " + msgRetorno;
                    else if (documentacaoLote.Usuario != null)
                        msgRetorno = "Operador: " + documentacaoLote.Usuario.Nome + " " + msgRetorno;
                }

                foreach (var cte in ctesFiltrados)
                {
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, msgRetorno, unitOfWork);
                    cte.DataUltimoEnvioDocumentacao = DateTime.Now;
                    cte.UltimoEmailEnvioDocumentacao = retorno ? emailsEnvio : msgRetorno;
                    cte.DocumentacaoEnviada = retorno;
                    cte.DocumentacaoEnviadaAutomatica = documentacaoLote.EnvioAutomatico ? retorno : false;

                    repCTe.Atualizar(cte);
                }

                return retorno;
            }
            return false;
        }
        private static void EnviarEmailProblemaEnvioAutomatico(string erro, Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote documentacaoLote, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);

            try
            {
                string assunto = "Problemas no envio Automático da Documentação Porta";
                string corpoEmail = "Não foi possível enviar a documentação de forma automática para o modal porta: <br/>";
                corpoEmail += "Motivo: " + erro + " <br/>";
                corpoEmail += "Viagem: " + (documentacaoLote.PedidoViagemNavio?.Descricao ?? "") + " <br/>";
                corpoEmail += "Terminal Destino: " + (documentacaoLote.TerminalDestino?.Descricao ?? "") + " <br/><br/><br/>";
                corpoEmail += "Tentativa " + documentacaoLote.QuantidadeTentativaEnvio.ToString() + " de 30 <br/>";
                corpoEmail += "Após a última tentativa deverá realizar o envio manualmente pela tela de Documentação em Lote <br/><br/>";

                List<string> emailsEnvio = new List<string>();
                if (empresa != null && !string.IsNullOrWhiteSpace(empresa.Email))
                    emailsEnvio.AddRange(empresa.Email.Split(';').ToList());
                if (empresa != null && !string.IsNullOrWhiteSpace(empresa.EmailAdministrativo))
                    emailsEnvio.AddRange(empresa.EmailAdministrativo.Split(';').ToList());
                if (emailsEnvio != null && emailsEnvio.Count > 0 && email != null)
                    Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, corpoEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "FaturamentoLote");
            }
        }

        private static bool EnviarDocumentacaoFinalizacaoCarga(out string erro, int codigoCarga, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, string host)
        {
            erro = "";
            List<string> emails = new List<string>();
            List<string> clientesGrupos = new List<string>();

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);
            Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);

            if (email == null)
            {
                Servicos.Log.TratarErro("Não há um e-mail configurado para realizar o envio.");
                erro = "Não há um e-mail configurado para realizar o envio.";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = repCargaPedido.BuscarPorCarga(codigoCarga);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Empresa = carga.Empresa
            };
            int codigoUsuario = carga.Operador?.Codigo ?? 0;
            string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos;
            string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
            string caminhoArquivosAnexos = $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" })}\\";
            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoRelatorios;

            foreach (var pedido in pedidos)
            {
                if (pedido.Pedido.Remetente != null && pedido.Pedido.Remetente.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Remetente.Email))
                {
                    emails.AddRange(pedido.Pedido.Remetente.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Remetente.Nome);
                }
                if (pedido.Pedido.Destinatario != null && pedido.Pedido.Destinatario.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Destinatario.Email))
                {
                    emails.AddRange(pedido.Pedido.Destinatario.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Destinatario.Nome);
                }
                if (pedido.Pedido.Expedidor != null && pedido.Pedido.Expedidor.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Expedidor.Email))
                {
                    emails.AddRange(pedido.Pedido.Expedidor.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Expedidor.Nome);
                }
                if (pedido.Pedido.Recebedor != null && pedido.Pedido.Recebedor.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Recebedor.Email))
                {
                    emails.AddRange(pedido.Pedido.Recebedor.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Recebedor.Nome);
                }
                if (pedido.Pedido.Tomador != null && pedido.Pedido.Tomador.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Tomador.Email))
                {
                    emails.AddRange(pedido.Pedido.Tomador.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Tomador.Nome);
                }
                if (pedido.Pedido.SubContratante != null && pedido.Pedido.SubContratante.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.SubContratante.Email))
                {
                    emails.AddRange(pedido.Pedido.SubContratante.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.SubContratante.Nome);
                }
                if (pedido.Pedido.Terceiro != null && pedido.Pedido.Terceiro.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Terceiro.Email))
                {
                    emails.AddRange(pedido.Pedido.Terceiro.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Terceiro.Nome);
                }

                //grupo
                if (pedido.Pedido.GrupoPessoas != null && pedido.Pedido.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.GrupoPessoas.Descricao);
                }
                if (pedido.Pedido.Remetente != null && pedido.Pedido.Remetente.GrupoPessoas != null && pedido.Pedido.Remetente.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Remetente.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.Remetente.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Remetente.GrupoPessoas.Descricao);
                }
                if (pedido.Pedido.Destinatario != null && pedido.Pedido.Destinatario.GrupoPessoas != null && pedido.Pedido.Destinatario.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Destinatario.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.Destinatario.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Destinatario.GrupoPessoas.Descricao);
                }
                if (pedido.Pedido.Expedidor != null && pedido.Pedido.Expedidor.GrupoPessoas != null && pedido.Pedido.Expedidor.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Expedidor.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.Expedidor.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Expedidor.GrupoPessoas.Descricao);
                }
                if (pedido.Pedido.Recebedor != null && pedido.Pedido.Recebedor.GrupoPessoas != null && pedido.Pedido.Recebedor.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Recebedor.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.Recebedor.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Recebedor.GrupoPessoas.Descricao);
                }
                if (pedido.Pedido.Tomador != null && pedido.Pedido.Tomador.GrupoPessoas != null && pedido.Pedido.Tomador.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Tomador.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.Tomador.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Tomador.GrupoPessoas.Descricao);
                }
                if (pedido.Pedido.SubContratante != null && pedido.Pedido.SubContratante.GrupoPessoas != null && pedido.Pedido.SubContratante.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.SubContratante.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.SubContratante.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.SubContratante.GrupoPessoas.Descricao);
                }
                if (pedido.Pedido.Terceiro != null && pedido.Pedido.Terceiro.GrupoPessoas != null && pedido.Pedido.Terceiro.GrupoPessoas.EnviarAutomaticamenteDocumentacaoCarga && !string.IsNullOrWhiteSpace(pedido.Pedido.Terceiro.GrupoPessoas.Email))
                {
                    emails.AddRange(pedido.Pedido.Terceiro.GrupoPessoas.Email.Split(';').ToList());
                    clientesGrupos.Add(pedido.Pedido.Terceiro.GrupoPessoas.Descricao);
                }
            }

            string msgRetorno = "";
            if (emails != null && emails.Count > 0 && clientesGrupos != null && clientesGrupos.Count > 0)
            {
                clientesGrupos = clientesGrupos.Distinct().ToList();
                emails = emails.Distinct().ToList();
                msgRetorno = "Documentação enviada para " + string.Join("; ", clientesGrupos.ToArray()) + " . E-mail(s): " + string.Join("; ", emails.ToArray());

                string assunto = "Documentação da carga " + carga.CodigoCargaEmbarcador + " - " + carga.Empresa.Descricao;
                string corpoEmail = "Olá <br/>";
                corpoEmail += "Segue em anexo a documentação da carga " + carga.CodigoCargaEmbarcador + " emitida por " + carga.Empresa.Descricao;

                List<int> ctes = repCargaCTe.BuscarCodigosCTesNFesAutorizadosPorCarga(carga.Codigo, false, false);
                List<int> mdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(carga.Codigo);
                List<int> valePedagios = repCargaValePedagio.BuscarCodigosValePedagiosSemPararPorCarga(carga.Codigo);

                //byte[] arquivo = Zeus.Embarcador.ZeusNFe.Zeus.GerarPDFTodosDocumentosEObterBytes(ctes, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, caminhoArquivosAnexos, unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLotePDFsTMS(codigoCarga, ctes, mdfes, valePedagios, unidadeTrabalho, tipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = new Dominio.Entidades.Embarcador.Terceiros.ContratoFrete();

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R011_ContratoDeFrete, tipoServicoMultisoftware, "Contrato de Transporte", "Terceiros", "ContratoFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unidadeTrabalho, false, false, 9, "Arial", false, 120);
                byte[] pdfContent = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho).GerarFaturaPadrao(contratoFrete, unidadeTrabalho, tipoServicoMultisoftware, relatorioTemp, codigoCarga);

                if (arquivo == null)
                    msgRetorno = "Nenhum arquivo encontrado para realizar o envio automático desta carga, favor realize o envio manualmente.";
                else
                {
                    List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                    attachments.Add(new System.Net.Mail.Attachment(arquivo, "DocumentacaoCarga" + carga.CodigoCargaEmbarcador + "_Documentos.zip"));

                    if (pdfContent != null)
                    {
                        MemoryStream stream = new MemoryStream(pdfContent);
                        Attachment anexo = new Attachment(stream, "ContratoFrete.pdf", "application/pdf");
                        attachments.Add(anexo);
                    }

                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, null, emails.ToArray(), assunto, corpoEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);
                    if (!sucesso)
                        msgRetorno = "Problemas ao enviar e-mail da documentação automática: " + mensagemErro + ", favor realize o envio manualmente.";
                }
            }
            else
                msgRetorno = "Não foi encontrado nenhum e-mail configurado para o envio automatizado da documentação, favor realize o envio manualmente.";

            carga.AguardandoEnvioDocumentacaoLote = false;
            repCarga.Atualizar(carga);
            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, msgRetorno, unidadeTrabalho);
            return true;
        }

        #endregion
    }
}