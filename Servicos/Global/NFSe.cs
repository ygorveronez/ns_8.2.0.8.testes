using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.CTe;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Utilidades.Extensions;

namespace Servicos
{
    public class NFSe : ServicoBase
    {
        #region Propriedades

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo _configuracaoArquivo;

        #endregion

        #region Construtores

        public NFSe() : base() { }

        public NFSe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Métodos Globais

        public bool Emitir(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            //Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

            bool utilizaEnotas = nfse.Empresa.Configuracao != null && nfse.Empresa.Configuracao.NFSeIntegracaoENotas && !string.IsNullOrWhiteSpace(nfse.Empresa.NFSeIDENotas) ? true : false;

            if (!utilizaEnotas)
            {
                try
                {
                    ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);
                    ServicoNFSe.NFSe nfseImportar = this.ObterNFSeParaEmissao(nfse, unitOfWork);
                    ServicoNFSe.ResultadoInteger retorno = svcNFSe.ImportarNFSe(nfseImportar);

                    if (retorno.Valor <= 0)
                    {
                        nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                        nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                        nfse.RPS.Status = "R";

                        repNFSe.Atualizar(nfse);

                        Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                        return false;
                    }
                    else
                    {
                        nfse.RPS.CodigoIntegracao = retorno.Valor;
                        nfse.RPS.MensagemRetorno = "NFS-e em processamento.";
                        nfse.Status = Dominio.Enumeradores.StatusNFSe.Enviado;
                        nfse.RPS.Status = "E";

                        repNFSe.Atualizar(nfse);

                        SalvarIntegracaoRetornoNFSe(nfse, nfse.Empresa, unitOfWork);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                    nfse.RPS.Status = "R";
                    nfse.RPS.MensagemRetorno = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - ERRO: Prefeitura indisponível no momento. Tente novamente.");

                    repNFSe.Atualizar(nfse);

                    return false;
                }
            }
            else
            {
                try
                {
                    Servicos.NFSeENotas svcNFSeEnotas = new NFSeENotas(unitOfWork);
                    System.Threading.Tasks.Task<string> retornoNFSe = svcNFSeEnotas.EnviarNFSe(nfse.Codigo, unitOfWork);
                    if (retornoNFSe != null && !string.IsNullOrWhiteSpace(retornoNFSe.Result))
                    {
                        nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                        nfse.RPS.Status = "R";
                        nfse.RPS.MensagemRetorno = retornoNFSe.Result;

                        repNFSe.Atualizar(nfse);

                        return false;
                    }
                    nfse.RPS.CodigoIntegracao = 0;
                    nfse.RPS.MensagemRetorno = "NFS-e em processamento.";
                    nfse.Status = Dominio.Enumeradores.StatusNFSe.Enviado;
                    nfse.RPS.Status = "E";

                    repNFSe.Atualizar(nfse);

                    return true;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                    nfse.RPS.Status = "R";
                    nfse.RPS.MensagemRetorno = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - ERRO: Prefeitura indisponível no momento. Tente novamente.");

                    repNFSe.Atualizar(nfse);

                    return true;
                }
            }
        }

        public bool AdicionarNFSeNaFilaDeConsulta(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                string configWebServiceConsultaCTe = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().WebServiceConsultaCTe;
                if (configWebServiceConsultaCTe == null || configWebServiceConsultaCTe == "")
                    configWebServiceConsultaCTe = "http://localhost/CTe/";

                string postData = "CodigoNFSe=" + nfse.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(configWebServiceConsultaCTe, "IntegracaoNFSe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                Dictionary<string, object> retorno = result.FromJson<Dictionary<string, object>>();

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao adicionar na fila: " + ex);
                return false;
            }
        }

        public bool EmitirNFSe(int codigoCte, Repositorio.UnitOfWork unitOfWork = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCte);

            try
            {
                if (cte.SistemaEmissor != null && cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Migrate)
                {
                    Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate serMigrate = new Embarcador.Integracao.Migrate.IntegracaoMigrate(unitOfWork);
                    return serMigrate.EmitirNFSe(cte, unitOfWork);
                }

                if (cte.RPS != null && ConsultarNFSePendente(cte, unitOfWork))
                    return true;

                ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                ServicoNFSe.NFSe nfseImportar = this.ObterNFSeParaEmissao(cte, unitOfWork);

                ServicoNFSe.ResultadoInteger retorno = svcNFSe.ImportarNFSe(nfseImportar);

                if (retorno.Valor <= 0)
                {
                    cte.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
                    cte.Status = "R";
                    cte.RPS.Status = "R";
                    cte.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);

                    repCTe.Atualizar(cte);
                    repRPSNFSe.Atualizar(cte.RPS);
                    Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);

                    return false;
                }
                else
                {
                    cte.CodigoCTeIntegrador = retorno.Valor;
                    cte.MensagemRetornoSefaz = "NFS-e em processamento.";
                    cte.DataIntegracao = DateTime.Now;

                    cte.RPS.CodigoIntegracao = retorno.Valor;
                    cte.RPS.MensagemRetorno = "NFS-e em processamento.";
                    cte.Status = "E";
                    cte.RPS.Status = "E";

                    repCTe.Atualizar(cte);
                    repRPSNFSe.Atualizar(cte.RPS);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                string mensagemErro = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - ERRO: Prefeitura indisponível no momento. Tente novamente.");

                if (cte != null)
                {
                    cte.Status = "R";
                    cte.MensagemRetornoSefaz = mensagemErro;

                    repCTe.Atualizar(cte);

                    if (cte.RPS != null)
                    {
                        cte.RPS.Status = "R";
                        cte.RPS.MensagemRetorno = mensagemErro;

                        repRPSNFSe.Atualizar(cte.RPS);
                    }
                }

                return false;
            }
        }

        public bool CancelarNFSe(int codigoCte, Repositorio.UnitOfWork unitOfWork = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCte);

            if (cte?.RPS == null)
                return false;

            if (cte.SistemaEmissor != null && cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Migrate)
            {
                Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate serMigrate = new Embarcador.Integracao.Migrate.IntegracaoMigrate(unitOfWork);
                serMigrate.CancelarNFSe(cte, unitOfWork);

                return true;
            }

            ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

            ServicoNFSe.ResultadoInteger ret = svcNFSe.CancelarNFSe(cte.RPS?.CodigoIntegracao ?? 0);

            cte.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(ret.Info.Mensagem, " - ", ret.Info.MensagemOriginal));

            if (ret.Info.Tipo == "OK")
            {
                cte.Status = "K";

                repCTe.Atualizar(cte);

                return true;
            }
            else
            {
                repCTe.Atualizar(cte);

                return false;
            }
        }

        public bool Cancelar(int codigoNFSe, Repositorio.UnitOfWork unitOfWork = null, bool gerarLog = false, Dominio.Entidades.Usuario usuario = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

            ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

            ServicoNFSe.ResultadoInteger ret = svcNFSe.CancelarNFSe(nfse.RPS.CodigoIntegracao);

            nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(ret.Info.Mensagem, " - ", ret.Info.MensagemOriginal));

            if (gerarLog && usuario != null)
                nfse.Log = string.Concat(nfse.Log, " Enviado cancelamento por ", usuario.CPF, " - ", usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

            if (ret.Info.Tipo == "OK")
            {
                nfse.Status = Dominio.Enumeradores.StatusNFSe.EmCancelamento;

                repNFSe.Atualizar(nfse);

                SalvarIntegracaoRetornoNFSe(nfse, nfse.Empresa, unitOfWork);

                return true;
            }
            else
            {
                repNFSe.Atualizar(nfse);

                return false;
            }
        }

        public void SalvarIntegracaoRetornoNFSe(Dominio.Entidades.NFSe nfse, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NFSeIntegracaoRetorno repNFSeIntegracaoRetorno = new Repositorio.NFSeIntegracaoRetorno(unidadeDeTrabalho);
            if (empresa.EmpresaPai?.TiposIntegracao != null && empresa.EmpresaPai?.TiposIntegracao.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in empresa.EmpresaPai?.TiposIntegracao)
                {
                    if (tipoIntegracao.Ativo && tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog ||
                        tipoIntegracao.Ativo && tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao)
                    {
                        Dominio.Entidades.NFSeIntegracaoRetorno NFSeIntegracaoRetorno = new Dominio.Entidades.NFSeIntegracaoRetorno();
                        NFSeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                        NFSeIntegracaoRetorno.NFSe = nfse;
                        NFSeIntegracaoRetorno.TipoIntegracao = tipoIntegracao;
                        repNFSeIntegracaoRetorno.Inserir(NFSeIntegracaoRetorno);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(empresa.Configuracao?.WsIntegracaoEnvioNFSeEmbarcadorTMS))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);
                if (tipoIntegracao == null)
                {
                    tipoIntegracao = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracao();
                    tipoIntegracao.Descricao = "MultiEmbarcador TMS";
                    tipoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador;
                    tipoIntegracao.Ativo = true;
                    repTipoIntegracao.Inserir(tipoIntegracao);
                }

                Dominio.Entidades.NFSeIntegracaoRetorno nfseIntegracaoRetorno = repNFSeIntegracaoRetorno.BuscarUltipoPorPorNFSeTipo(nfse.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);
                if (nfseIntegracaoRetorno == null)
                {
                    nfseIntegracaoRetorno = new Dominio.Entidades.NFSeIntegracaoRetorno();
                    nfseIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                    nfseIntegracaoRetorno.NFSe = nfse;
                    nfseIntegracaoRetorno.TipoIntegracao = tipoIntegracao;
                    repNFSeIntegracaoRetorno.Inserir(nfseIntegracaoRetorno);
                }
                else
                {
                    nfseIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                    repNFSeIntegracaoRetorno.Atualizar(nfseIntegracaoRetorno);
                }
            }
        }

        public void AtualizarIntegracaoRetornoNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico NFSe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NFSeIntegracaoRetorno repNFSeIntegracaoRetorno = new Repositorio.NFSeIntegracaoRetorno(unidadeDeTrabalho);
            List<Dominio.Entidades.NFSeIntegracaoRetorno> listaIntegracoes = repNFSeIntegracaoRetorno.BuscarPorNFSe(NFSe.Codigo);
            foreach (Dominio.Entidades.NFSeIntegracaoRetorno integracao in listaIntegracoes)
            {
                integracao.NumeroTentativas = 0;
                integracao.ProblemaIntegracao = string.Empty;
                integracao.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando;
                repNFSeIntegracaoRetorno.Inserir(integracao);
            }
        }

        public Dominio.Entidades.NFSe Consultar(int codigoNFSe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            try
            {
                using ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                Servicos.Log.GravarDebug($"Consultar nfse.Status: {nfse.Status}", "NFSe");

                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Enviado || nfse.Status == Dominio.Enumeradores.StatusNFSe.Pendente)
                {
                    ServicoNFSe.RetornoNFSe retorno = svcNFSe.ConsultarNFSePorCodigo(nfse.RPS.CodigoIntegracao);

                    Servicos.Log.GravarDebug($"Consultar retorno.Info.Tipo: {retorno.Info.Tipo}, retorno.Status: {retorno.Status}", "NFSe");

                    unidadeDeTrabalho.Start();

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        if (retorno.Status.Equals("E"))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                            nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetorno, " - ", retorno.MensagemRetorno));
                        }
                        else if (retorno.Status.Equals("I"))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Enviado;
                            nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetorno, " - ", retorno.MensagemRetorno));
                        }
                        else if (retorno.Status.Equals("D") || retorno.Status.Equals("M"))
                        {
                            if (repNFSe.VerificaNFSeJaAutorizada(retorno.Numero, nfse.Serie.Codigo, nfse.Empresa.Codigo, nfse.DataEmissao.Year, nfse.Ambiente, retorno.CodigoVerificacao) > 0)
                            {
                                nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                                nfse.RPS.MensagemRetorno = "NFSe não processada na prefeitura, favor aguardar e reenviar.";
                            }
                            else
                            {
                                nfse.Status = Dominio.Enumeradores.StatusNFSe.Autorizado;
                                nfse.RPS.Status = "A";
                                nfse.RPS.CodigoRetorno = retorno.CodigoRetorno;
                                nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                                nfse.RPS.Protocolo = retorno.Protocolo;
                                nfse.RPS.Data = retorno.DataRetorno;
                                nfse.CodigoVerificacao = retorno.CodigoVerificacao;
                                nfse.Numero = retorno.Numero;
                                nfse.NumeroPrefeitura = retorno.NumeroPrefeitura;
                                if (retorno.DataEmissao != null)
                                    nfse.DataEmissao = retorno.DataEmissao;

                                if (retorno.NumeroRPS > 0)
                                    nfse.RPS.Numero = retorno.NumeroRPS;

                                this.ObterESalvarXMLAutorizacao(nfse.Codigo, unidadeDeTrabalho, retorno);

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorRPS(nfse.RPS.Codigo);
                                if (cte != null && cte.Status != "A" && cte.Status != "C")
                                {
                                    cte.Numero = retorno.Numero;

                                    cte.Status = "A";
                                    cte.MensagemRetornoSefaz = nfse.RPS.MensagemRetorno;
                                    cte.Protocolo = nfse.RPS.Protocolo;
                                    cte.DataAutorizacao = nfse.RPS.Data;
                                    cte.TipoControle = retorno.Numero;

                                    repCTe.Atualizar(cte);
                                }
                                else if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().GerarCTeDasNFSeAutorizadas.Value) //Gerar um CTe da NFSe
                                {
                                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                                    cte = servicoNFSe.ConverterNFSeEmCTe(nfse, unidadeDeTrabalho, false);
                                }

                                Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unidadeDeTrabalho);
                                svcLsTranslog.SalvarNFSeParaIntegracao(nfse.Codigo, nfse.Empresa.Codigo, unidadeDeTrabalho);
                            }
                        }
                        else
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                            nfse.RPS.Status = "R";
                            nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                        }
                    }
                    else
                    {
                        nfse.Status = Dominio.Enumeradores.StatusNFSe.Rejeicao;
                        nfse.RPS.Status = "R";
                        nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                    }

                    Servicos.Log.GravarDebug($"Consultar antes de repNFSe.Atualizar(nfse)", "NFSe");

                    repNFSe.Atualizar(nfse);

                    unidadeDeTrabalho.CommitChanges();

                    Servicos.Log.GravarDebug($"Consultar depois de unidadeDeTrabalho.CommitChanges()", "NFSe");
                }

                return nfse;
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro("Consultar NFSe " + ex);

                throw;
            }
        }

        public Dominio.Entidades.NFSe ConsultarCancelamento(int codigoNFSe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.EmCancelamento)
                {
                    ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                    ServicoNFSe.RetornoCancelamentoNFSe retorno = svcNFSe.ConsultarCancelamentoNFSe(nfse.RPS.CodigoIntegracao);

                    unidadeDeTrabalho.Start();

                    if (retorno.Info.Tipo.Equals("OK"))
                    {
                        if (retorno.Status.Equals("E"))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Autorizado;
                            nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetorno, " - ", retorno.MensagemRetorno));
                        }
                        else if (retorno.Status.Equals("I"))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.EmCancelamento;
                            nfse.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetorno, " - ", retorno.MensagemRetorno));
                        }
                        else if (retorno.Status.Equals("C"))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Cancelado;
                            nfse.RPS.Status = "C";
                            nfse.RPS.CodigoRetorno = retorno.CodigoRetorno;
                            nfse.RPS.MensagemRetorno = retorno.MensagemRetorno;
                            nfse.RPS.ProtocoloCancelamento = retorno.Protocolo;
                            nfse.RPS.DataProtocolo = retorno.DataRetorno;

                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorRPS(nfse.RPS.Codigo);
                            if (cte != null)
                            {
                                cte.Status = "C";
                                cte.MensagemRetornoSefaz = retorno.MensagemRetorno;
                                cte.ProtocoloCancelamentoInutilizacao = retorno.Protocolo;
                                cte.DataCancelamento = retorno.DataRetorno;

                                repCTe.Atualizar(cte);
                            }
                        }
                    }

                    repNFSe.Atualizar(nfse);
                }

                unidadeDeTrabalho.CommitChanges();

                return nfse;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();

                throw;
            }
        }

        public bool ConsultarNFSePendente(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.Status != "E" && cte.Status != "P")
                return false;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

            ServicoNFSe.RetornoNFSe retorno = svcNFSe.ConsultarNFSePorCodigo(cte.RPS.CodigoIntegracao);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            if (retorno.Info.Tipo.Equals("OK"))
            {
                if (retorno.Status.Equals("E"))
                {
                    string mensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetorno, " - ", retorno.MensagemRetorno));

                    cte.Status = "R";
                    cte.MensagemRetornoSefaz = mensagemRetorno;
                    cte.RPS.MensagemRetorno = mensagemRetorno;
                    cte.StatusIntegrador = retorno.Status;
                }
                else if (retorno.Status.Equals("I"))
                {
                    string mensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.DescricaoStatus, " - ", retorno.CodigoRetorno, " - ", retorno.MensagemRetorno));

                    cte.Status = "E";
                    cte.MensagemRetornoSefaz = mensagemRetorno;
                    cte.RPS.MensagemRetorno = mensagemRetorno;
                    cte.StatusIntegrador = retorno.Status;
                }
                else if (retorno.Status.Equals("D") || retorno.Status.Equals("M"))
                {
                    if (repCTe.VerificaNFSeJaAutorizada(retorno.Numero, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.DataEmissao.Value.Year, cte.TipoAmbiente, retorno.CodigoVerificacao) > 0)
                    {
                        cte.Status = "R";
                        cte.RPS.MensagemRetorno = "NFSe não processada na prefeitura, favor aguardar e reenviar.";
                        cte.CodigoCTeIntegrador = 0;
                    }
                    else
                    {
                        string mensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
                        DateTime dataAutorizacao = retorno.DataEmissao != DateTime.MinValue ? retorno.DataEmissao : DateTime.Now;

                        cte.Status = "A";
                        cte.MensagemRetornoSefaz = mensagemRetorno;
                        cte.Protocolo = retorno.CodigoVerificacao;

                        cte.DataRetornoSefaz = retorno.DataRetorno;
                        cte.DataAutorizacao = dataAutorizacao;
                        cte.Chave = "";
                        cte.StatusIntegrador = retorno.Status;

                        cte.RPS.Status = "A";
                        cte.RPS.CodigoRetorno = retorno.CodigoRetorno;
                        cte.RPS.MensagemRetorno = mensagemRetorno;
                        cte.RPS.Protocolo = retorno.Protocolo;
                        cte.RPS.Data = dataAutorizacao;
                        cte.Numero = retorno.Numero;

                        cte.NumeroPrefeituraNFSe = retorno.NumeroPrefeitura;
                        cte.DataEmissao = dataAutorizacao;

                        if (retorno.NumeroRPS > 0)
                            cte.RPS.Numero = retorno.NumeroRPS;

                        int quantidadeNFSes = repCTe.ContarCTePorChaveUnica(cte.Numero, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.TipoAmbiente);
                        cte.TipoControle = quantidadeNFSes + 1;

                        if (configuracaoWebService.NaoPermitirGerarNFSeComMesmaNumeracao)
                        {
                            int quantidadeNFSesMesmaNumeracao = repCTe.ContarCTePorChaveUnicaEStatus(cte.Numero, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.TipoAmbiente, new string[] { "A", "R" });
                            if (quantidadeNFSesMesmaNumeracao > 1)
                            {
                                cte.Status = "R";
                                cte.RPS.Status = "R";
                                cte.MensagemRetornoSefaz = "Rejeitada por duplicidade de numeração na prefeitura";
                            }
                        }

                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

                        if (cargaCTe != null && cargaCTe.Carga.problemaCTE)
                        {
                            cargaCTe.Carga.PossuiPendencia = false;
                            cargaCTe.Carga.problemaCTE = false;
                            cargaCTe.Carga.MotivoPendencia = string.Empty;
                            repCarga.Atualizar(cargaCTe.Carga);
                        }
                    }
                }
            }
            else
            {
                cte.Status = "R";
                cte.RPS.Status = "R";
                cte.RPS.MensagemRetorno = Utilidades.String.ReplaceInvalidCharacters(string.Concat(retorno.Info.Mensagem, " - ", retorno.Info.MensagemOriginal));
            }

            repCTe.Atualizar(cte);

            if (cte.Status == "A")
            {
                ObterDANFSECTe(cte.Codigo, unitOfWork);
                serCTe.ObterESalvarXMLNFSAutorizacao(cte.Codigo, cte.Empresa.Codigo, false, retorno, unitOfWork);
            }

            return true;
        }

        public void ObterESalvarXMLAutorizacao(int codigoNFSe, Repositorio.UnitOfWork unidadeDeTrabalho, ServicoNFSe.RetornoNFSe retorno = null)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.XMLNFSe repXML = new Repositorio.XMLNFSe(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

            if (nfse != null)
            {
                if (retorno == null)
                {
                    ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                    retorno = svcNFSe.ConsultarNFSePorCodigo(nfse.RPS.CodigoIntegracao);
                }

                if (!string.IsNullOrWhiteSpace(retorno.XML))
                {
                    Dominio.Entidades.XMLNFSe xml = repXML.BuscarPorNFSe(nfse.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao);

                    if (xml == null)
                        xml = new Dominio.Entidades.XMLNFSe();

                    xml.NFSe = nfse;
                    xml.Tipo = Dominio.Enumeradores.TipoXMLNFSe.Autorizacao;
                    xml.XML = retorno.XML;

                    if (xml.Codigo > 0)
                        repXML.Atualizar(xml);
                    else
                        repXML.Inserir(xml);
                }
            }
        }
        public byte[] ObterXMLAutorizacaoCTe(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                Servicos.ServicoNFSe.RetornoNFSe retorno = svcNFSe.ConsultarNFSePorCodigo(cte.RPS.CodigoIntegracao);

                if (!string.IsNullOrWhiteSpace(retorno.XML))
                {
                    byte[] data = System.Text.Encoding.Default.GetBytes(retorno.XML);

                    return data;
                }
            }

            return null;
        }

        public byte[] ObterDANFSE(int codigoNFSe, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

            if (nfse != null)
            {
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, "NFSe", nfse.Empresa.CNPJ, nfse.Codigo.ToString() + "_" + nfse.Numero.ToString() + "_" + nfse.Serie.Numero.ToString()) + ".pdf";

                ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                ServicoNFSe.RetornoNFSe retorno = svcNFSe.ConsultarNFSePorCodigo(nfse.RPS.CodigoIntegracao);

                if (!string.IsNullOrWhiteSpace(retorno.DANFSE))
                {
                    string texto = Encoding.UTF8.GetString(Convert.FromBase64String(retorno.DANFSE));
                    var windows1252 = Encoding.GetEncoding("Windows-1252");
                    byte[] decodedData = windows1252.GetBytes(texto);

                    if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    {
                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                    }

                    return decodedData;
                }
            }

            return null;
        }

        public byte[] ObterDANFSECTe(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho = null, bool buscarSomentePorNumero = false)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ObterConfiguracaoArquivo(unidadeDeTrabalho).CaminhoRelatorios, "NFSe", cte.Empresa.CNPJ, (!buscarSomentePorNumero ? (cte.Codigo.ToString() + "_") : "") + cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString()) + ".pdf";

                if (/*string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().BuscarPDFNFSeOracle) &&*/ Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                else if (cte.RPS != null)
                {
                    if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Migrate)
                    {
                        Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate serMigrate = new Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate(unidadeDeTrabalho);
                        byte[] pdf = serMigrate.ObterDANFSE(cte, unidadeDeTrabalho);
                        return pdf;
                    }
                    else
                    {
                        ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                        ServicoNFSe.RetornoNFSe retorno = svcNFSe.ConsultarNFSePorCodigo(cte.RPS.CodigoIntegracao);

                        if (retorno != null && !string.IsNullOrWhiteSpace(retorno.DANFSE))
                        {
                            byte[] decodedData = System.Text.Encoding.Default.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, System.Convert.FromBase64String(retorno.DANFSE))).ToArray();

                            if (!string.IsNullOrWhiteSpace(ObterConfiguracaoArquivo(unidadeDeTrabalho).CaminhoRelatorios))
                            {
                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                            }

                            return decodedData;
                        }
                    }
                }
                else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repNFSManual.BuscarPorCTe(codigoCTe);

                    if (nfsManual != null && !string.IsNullOrWhiteSpace(nfsManual.DadosNFS.ImagemNFS))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(nfsManual.DadosNFS.ImagemNFS))
                            throw new ServicoException($"O arquivo da NFS-e {nfsManual.DadosNFS.Numero} não foi encontrado.");

                        return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nfsManual.DadosNFS.ImagemNFS);
                    }
                }
            }

            return null;
        }

        public string ObterDANFSEString(int codigoNFSe, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            string stringPDF = null;

            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

            if (nfse != null)
            {
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, "NFSe", nfse.Empresa.CNPJ, nfse.Codigo.ToString() + "_" + nfse.Numero.ToString() + "_" + nfse.Serie.Numero.ToString()) + ".pdf";

                ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

                ServicoNFSe.RetornoNFSe retorno = svcNFSe.ConsultarNFSePorCodigo(nfse.RPS.CodigoIntegracao);

                if (!string.IsNullOrWhiteSpace(retorno.DANFSE) && retorno.DANFSE != "SEM PDF")
                {
                    byte[] decodedData = System.Text.Encoding.Default.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, System.Convert.FromBase64String(retorno.DANFSE))).ToArray();

                    if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    {
                        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                    }

                    stringPDF = Convert.ToBase64String(decodedData);
                }
            }

            return stringPDF;

        }

        public byte[] ObterXML(int codigoNFSe, Dominio.Enumeradores.TipoXMLNFSe tipo, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unidadeDeTrabalho);

            Dominio.Entidades.XMLNFSe xml = repXMLNFSe.BuscarPorNFSe(codigoNFSe, tipo);

            if (xml != null && !string.IsNullOrWhiteSpace(xml.XML))
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(xml.XML);

                return data;
            }

            return null;
        }

        public string ObterXMLString(int codigoNFSe, Dominio.Enumeradores.TipoXMLNFSe tipo, Repositorio.UnitOfWork unidadeDeTrabalho = null)
        {
            unidadeDeTrabalho = unidadeDeTrabalho ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unidadeDeTrabalho);

            Dominio.Entidades.XMLNFSe xml = repXMLNFSe.BuscarPorNFSe(codigoNFSe, tipo);

            if (xml != null)
                return xml.XML;
            else
                return null;
        }

        public Dominio.Entidades.NFSe GerarNFSePorNFe(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, Dominio.Entidades.Empresa empresa, int codigoVeiculo, bool issRetido, string observacoes, decimal valorServico, Dominio.Enumeradores.StatusNFSe status, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            NFe servicoNFe = new NFe(unidadeDeTrabalho);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();
            Dominio.Entidades.Cliente cliente = servicoNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa.Codigo);

            nfse.CodigoIBGECidadePrestacaoServico = cliente.Localidade.CodigoIBGE;
            nfse.ISSRetido = issRetido;
            nfse.OutrasInformacoes = observacoes;
            nfse.PesoKg = ObterPesoNFe(nfe.NFe.infNFe.transp.vol, unidadeDeTrabalho);
            if (codigoVeiculo > 0)
                nfse.CodigoVeiculo = codigoVeiculo;

            nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            {
                CNPJ = empresa.CNPJ,
                Atualizar = false
            };

            nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = cliente.Bairro,
                CEP = cliente.CEP,
                CodigoAtividade = cliente.Atividade.Codigo,
                CodigoIBGECidade = cliente.Localidade.CodigoIBGE,
                CodigoPais = cliente.Localidade.Estado.Pais.Codigo.ToString(),
                Complemento = cliente.Complemento,
                CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                Endereco = cliente.Endereco,
                Exportacao = false,
                NomeFantasia = cliente.NomeFantasia,
                RazaoSocial = cliente.Nome,
                RGIE = cliente.IE_RG,
                Numero = !string.IsNullOrWhiteSpace(cliente.Numero) && cliente.Numero.Length > 2 ? cliente.Numero : "S/N",
                Telefone1 = cliente.Telefone1,
                Telefone2 = cliente.Telefone2
            };

            nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();

            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
            Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, nfse.CodigoIBGECidadePrestacaoServico != empresa.Localidade.CodigoIBGE, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);

            nfse.Itens.Add(new Dominio.ObjetosDeValor.NFSe.Item()
            {
                CodigoIBGECidade = nfse.CodigoIBGECidadePrestacaoServico,
                CodigoIBGECidadeIncidencia = nfse.CodigoIBGECidadePrestacaoServico,
                Discriminacao = observacoes,
                CodigoPaisPrestacaoServico = int.Parse(empresa.Localidade.Pais.Sigla),
                Quantidade = 1,
                ServicoPrestadoNoPais = true,
                ExigibilidadeISS = 1,
                ValorServico = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                BaseCalculoISS = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                AliquotaISS = servicoMultiCTe?.Aliquota ?? 0,
                ValorISS = servicoMultiCTe != null ? valorServico * (servicoMultiCTe.Aliquota / 100) : 0, //(valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100))) * empresa.Configuracao.ServicoNFSe.Aliquota / 100,
                ValorTotal = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
            });

            decimal pesoNota = ObterPesoNFe(nfe.NFe.infNFe.transp.vol, unidadeDeTrabalho);

            if (!DateTime.TryParseExact(nfe.protNFe.infProt.dhRecbto.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoNota))
                dataEmissaoNota = DateTime.Now;

            nfse.Documentos = new List<Dominio.ObjetosDeValor.NFSe.Documentos>() {
                new Dominio.ObjetosDeValor.NFSe.Documentos(){
                    ChaveNFE = nfe.protNFe.infProt.chNFe,
                    Numero = nfe.protNFe.infProt.chNFe.Substring(25, 9),
                    Serie = nfe.protNFe.infProt.chNFe.Substring(22, 3),
                    Peso = pesoNota,
                    Valor = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                    DataEmissao = dataEmissaoNota.ToString("dd/MM/yyyy")
                }
            };

            NFSe svcNFSe = new NFSe(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfseIntegrada = this.GerarNFSePorObjeto(nfse, unidadeDeTrabalho, status);

            return nfseIntegrada;
        }

        public Dominio.Entidades.NFSe GerarNFSePorNFe(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, Dominio.Entidades.Empresa empresa, int codigoVeiculo, bool issRetido, string observacoes, decimal valorServico, Dominio.Enumeradores.StatusNFSe status, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            NFe servicoNFe = new NFe(unidadeDeTrabalho);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();
            Dominio.Entidades.Cliente cliente = servicoNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa.Codigo);

            nfse.CodigoIBGECidadePrestacaoServico = cliente.Localidade.CodigoIBGE;
            nfse.ISSRetido = issRetido;
            nfse.OutrasInformacoes = observacoes;
            nfse.PesoKg = ObterPesoNFe(nfe.NFe.infNFe.transp.vol, unidadeDeTrabalho);
            if (codigoVeiculo > 0)
                nfse.CodigoVeiculo = codigoVeiculo;

            nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            {
                CNPJ = empresa.CNPJ,
                Atualizar = false
            };

            nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = cliente.Bairro,
                CEP = cliente.CEP,
                CodigoAtividade = cliente.Atividade.Codigo,
                CodigoIBGECidade = cliente.Localidade.CodigoIBGE,
                CodigoPais = cliente.Localidade.Estado.Pais.Codigo.ToString(),
                Complemento = cliente.Complemento,
                CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                Endereco = cliente.Endereco,
                Exportacao = false,
                NomeFantasia = cliente.NomeFantasia,
                RazaoSocial = cliente.Nome,
                RGIE = cliente.IE_RG,
                Numero = !string.IsNullOrWhiteSpace(cliente.Numero) && cliente.Numero.Length > 2 ? cliente.Numero : "S/N",
                Telefone1 = cliente.Telefone1,
                Telefone2 = cliente.Telefone2
            };

            nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();

            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
            Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, nfse.CodigoIBGECidadePrestacaoServico != empresa.Localidade.CodigoIBGE, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);

            nfse.Itens.Add(new Dominio.ObjetosDeValor.NFSe.Item()
            {
                CodigoIBGECidade = nfse.CodigoIBGECidadePrestacaoServico,
                CodigoIBGECidadeIncidencia = nfse.CodigoIBGECidadePrestacaoServico,
                Discriminacao = observacoes,
                CodigoPaisPrestacaoServico = int.Parse(empresa.Localidade.Pais.Sigla),
                Quantidade = 1,
                ServicoPrestadoNoPais = true,
                ExigibilidadeISS = 1,
                ValorServico = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                BaseCalculoISS = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                AliquotaISS = servicoMultiCTe?.Aliquota ?? 0,
                ValorISS = servicoMultiCTe != null ? valorServico * (servicoMultiCTe.Aliquota / 100) : 0, //(valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100))) * empresa.Configuracao.ServicoNFSe.Aliquota / 100,
                ValorTotal = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
            });

            decimal pesoNota = ObterPesoNFe(nfe.NFe.infNFe.transp.vol, unidadeDeTrabalho);

            if (!DateTime.TryParseExact(nfe.protNFe.infProt.dhRecbto.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoNota))
                dataEmissaoNota = DateTime.Now;

            nfse.Documentos = new List<Dominio.ObjetosDeValor.NFSe.Documentos>() {
                new Dominio.ObjetosDeValor.NFSe.Documentos(){
                    ChaveNFE = nfe.protNFe.infProt.chNFe,
                    Numero = nfe.protNFe.infProt.chNFe.Substring(25, 9),
                    Serie = nfe.protNFe.infProt.chNFe.Substring(22, 3),
                    Peso = pesoNota,
                    Valor = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                    DataEmissao = dataEmissaoNota.ToString("dd/MM/yyyy")
                }
            };

            NFSe svcNFSe = new NFSe(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfseIntegrada = this.GerarNFSePorObjeto(nfse, unidadeDeTrabalho, status);

            return nfseIntegrada;
        }

        public Dominio.Entidades.NFSe GerarNFSePorNFeMercadoLivre(List<Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal> listaNotas, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, int codigoVeiculo, string observacoes, decimal valorServico, Dominio.Enumeradores.StatusNFSe status, Repositorio.UnitOfWork unidadeDeTrabalho, string stringConexao)
        {
            NFe servicoNFe = new NFe(unidadeDeTrabalho);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();

            nfse.CodigoIBGECidadePrestacaoServico = expedidor != null ? expedidor.Localidade.CodigoIBGE : tomador.Localidade.CodigoIBGE;
            nfse.OutrasInformacoes = observacoes;
            if (codigoVeiculo > 0)
                nfse.CodigoVeiculo = codigoVeiculo;

            nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            {
                CNPJ = empresa.CNPJ,
                Atualizar = false
            };

            nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = tomador.Bairro,
                CEP = tomador.CEP,
                CodigoAtividade = tomador.Atividade.Codigo,
                CodigoIBGECidade = tomador.Localidade.CodigoIBGE,
                CodigoPais = tomador.Localidade.Estado.Pais.Codigo.ToString(),
                Complemento = tomador.Complemento,
                CPFCNPJ = tomador.CPF_CNPJ_SemFormato,
                Endereco = tomador.Endereco,
                Exportacao = false,
                NomeFantasia = tomador.NomeFantasia,
                RazaoSocial = tomador.Nome,
                RGIE = tomador.IE_RG,
                Numero = !string.IsNullOrWhiteSpace(tomador.Numero) && tomador.Numero.Length > 2 ? tomador.Numero : "S/N",
                Telefone1 = tomador.Telefone1,
                Telefone2 = tomador.Telefone2
            };

            nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();

            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
            Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, nfse.CodigoIBGECidadePrestacaoServico != empresa.Localidade.CodigoIBGE, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);

            nfse.Itens.Add(new Dominio.ObjetosDeValor.NFSe.Item()
            {
                CodigoIBGECidade = nfse.CodigoIBGECidadePrestacaoServico,
                CodigoIBGECidadeIncidencia = nfse.CodigoIBGECidadePrestacaoServico,
                Discriminacao = observacoes,
                CodigoPaisPrestacaoServico = int.Parse(empresa.Localidade.Pais.Sigla),
                Quantidade = 1,
                ServicoPrestadoNoPais = true,
                ExigibilidadeISS = 1,
                ValorServico = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                BaseCalculoISS = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
                AliquotaISS = servicoMultiCTe?.Aliquota ?? 0,
                ValorISS = servicoMultiCTe != null ? valorServico * (servicoMultiCTe.Aliquota / 100) : 0, //(valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100))) * empresa.Configuracao.ServicoNFSe.Aliquota / 100,
                ValorTotal = valorServico,//valorServico / (1 - (empresa.Configuracao.ServicoNFSe.Aliquota / 100)),
            });

            nfse.ISSRetido = servicoMultiCTe.ISSRetido;

            nfse.Documentos = new List<Dominio.ObjetosDeValor.NFSe.Documentos>();
            foreach (Dominio.ObjetosDeValor.MercadoLivre.NotaFiscal nota in listaNotas)
            {
                Dominio.ObjetosDeValor.NFSe.Documentos notaFiscal = new Dominio.ObjetosDeValor.NFSe.Documentos()
                {
                    ChaveNFE = nota.Chave,
                    Numero = nota.Numero > 0 ? nota.Numero.ToString() : nota.Chave.Substring(25, 9),
                    Serie = nota.Serie > 0 ? nota.Serie.ToString() : nota.Chave.Substring(22, 3),
                    Peso = 0,
                    Valor = nota.Valor,
                    DataEmissao = DateTime.Today.ToString("dd/MM/yyyy")
                };

                nfse.Documentos.Add(notaFiscal);
            }

            NFSe svcNFSe = new NFSe(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfseIntegrada = this.GerarNFSePorObjeto(nfse, unidadeDeTrabalho, status);

            return nfseIntegrada;
        }

        public string AdicionarNotaNFSeDigitacao(Dominio.Entidades.NFSe nfse, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

                if (nfe != null)
                {
                    Dominio.Entidades.DocumentosNFSe documentoNFSe = repDocumentosNFSe.BuscarPorNFSeENFe(nfse.Codigo, nfe.protNFe.infProt.chNFe);

                    if (documentoNFSe == null)
                    {
                        decimal pesoNFe = ObterPesoNFe(nfe.NFe.infNFe.transp.vol, unidadeDeTrabalho);

                        if (!DateTime.TryParseExact(nfe.protNFe.infProt.dhRecbto.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoNota))
                            dataEmissaoNota = DateTime.Now;

                        documentoNFSe = new Dominio.Entidades.DocumentosNFSe();
                        documentoNFSe.NFSe = nfse;
                        documentoNFSe.Chave = nfe.protNFe.infProt.chNFe;
                        documentoNFSe.Numero = nfe.protNFe.infProt.chNFe.Substring(25, 9);
                        documentoNFSe.Serie = nfe.protNFe.infProt.chNFe.Substring(22, 3);
                        documentoNFSe.Peso = pesoNFe;
                        documentoNFSe.Valor = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura);
                        documentoNFSe.DataEmissao = dataEmissaoNota;

                        repDocumentosNFSe.Inserir(documentoNFSe);

                        if (pesoNFe > 0)
                        {
                            nfse.PesoKG += pesoNFe;
                            repNFSe.Atualizar(nfse);
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao adicionar dados da NFSe " + ex);
                return "Problemas ao adicionar dados da NFSe " + ex;
            }
        }

        public string AdicionarNotaNFSeDigitacao(Dominio.Entidades.NFSe nfse, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

                if (nfe != null)
                {
                    Dominio.Entidades.DocumentosNFSe documentoNFSe = repDocumentosNFSe.BuscarPorNFSeENFe(nfse.Codigo, nfe.protNFe.infProt.chNFe);

                    if (documentoNFSe == null)
                    {
                        decimal pesoNFe = ObterPesoNFe(nfe.NFe.infNFe.transp.vol, unidadeDeTrabalho);

                        if (!DateTime.TryParseExact(nfe.protNFe.infProt.dhRecbto.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoNota))
                            dataEmissaoNota = DateTime.Now;

                        documentoNFSe = new Dominio.Entidades.DocumentosNFSe();
                        documentoNFSe.NFSe = nfse;
                        documentoNFSe.Chave = nfe.protNFe.infProt.chNFe;
                        documentoNFSe.Numero = nfe.protNFe.infProt.chNFe.Substring(25, 9);
                        documentoNFSe.Serie = nfe.protNFe.infProt.chNFe.Substring(22, 3);
                        documentoNFSe.Peso = pesoNFe;
                        documentoNFSe.Valor = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura);
                        documentoNFSe.DataEmissao = dataEmissaoNota;

                        repDocumentosNFSe.Inserir(documentoNFSe);

                        if (pesoNFe > 0)
                        {
                            nfse.PesoKG += pesoNFe;
                            repNFSe.Atualizar(nfse);
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao adicionar dados da NFSe " + ex);
                return "Problemas ao adicionar dados da NFSe " + ex;
            }
        }

        public bool AlterarValorServicoNFSe(Dominio.Entidades.NFSe nfse, decimal valorServico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.ItemNFSe repItensNFSe = new Repositorio.ItemNFSe(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(nfse.Empresa.Codigo);
                List<Dominio.Entidades.ItemNFSe> itens = repItensNFSe.BuscarPorNFSe(nfse.Codigo);

                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, nfse.LocalidadePrestacaoServico.CodigoIBGE != empresa.Localidade.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unidadeDeTrabalho);

                decimal valorISS = servicoMultiCTe != null ? valorServico * (servicoMultiCTe.Aliquota / 100) : 0;
                decimal baseCalculoIBSCBS = valorServico;

                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                {
                    BaseCalculo = baseCalculoIBSCBS,
                    ValoAbaterBaseCalculo = valorISS,
                    CodigoLocalidade = nfse.LocalidadePrestacaoServico.Codigo,
                    SiglaUF = nfse.LocalidadePrestacaoServico.Estado.Sigla,
                    CodigoTipoOperacao = 0,
                    Empresa = empresa
                });

                foreach (Dominio.Entidades.ItemNFSe item in itens)
                {
                    item.ValorServico = valorServico;
                    item.BaseCalculoISS = valorServico;
                    item.AliquotaISS = servicoMultiCTe?.Aliquota ?? 0;
                    item.ValorISS = servicoMultiCTe != null ? valorServico * (servicoMultiCTe.Aliquota / 100) : 0;
                    item.ValorTotal = valorServico;

                    item.NBS = servicoMultiCTe?.NBS ?? "";
                    item.CodigoIndicadorOperacao = impostoIBSCBS?.CodigoIndicadorOperacao ?? "";
                    item.CSTIBSCBS = impostoIBSCBS?.CST ?? "";
                    item.ClassificacaoTributariaIBSCBS = impostoIBSCBS?.ClassificacaoTributaria ?? "";
                    item.BaseCalculoIBSCBS = impostoIBSCBS?.BaseCalculo ?? 0;
                    item.OutrasAliquotas = impostoIBSCBS != null ? new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas { Codigo = impostoIBSCBS.CodigoOutraAliquota } : null;
                    item.AliquotaIBSEstadual = impostoIBSCBS?.AliquotaIBSEstadual ?? 0;
                    item.PercentualReducaoIBSEstadual = impostoIBSCBS?.PercentualReducaoIBSEstadual ?? 0;
                    item.ValorIBSEstadual = impostoIBSCBS?.ValorIBSEstadual ?? 0;
                    item.AliquotaIBSMunicipal = impostoIBSCBS?.AliquotaIBSMunicipal ?? 0;
                    item.PercentualReducaoIBSMunicipal = impostoIBSCBS?.PercentualReducaoIBSMunicipal ?? 0;
                    item.ValorIBSMunicipal = impostoIBSCBS?.ValorIBSMunicipal ?? 0;
                    item.AliquotaCBS = impostoIBSCBS?.AliquotaCBS ?? 0;
                    item.PercentualReducaoCBS = impostoIBSCBS?.PercentualReducaoCBS ?? 0;
                    item.ValorCBS = impostoIBSCBS?.ValorCBS ?? 0;

                    repItensNFSe.Atualizar(item);
                }


                nfse.AliquotaISS = servicoMultiCTe?.Aliquota ?? 0;
                nfse.BaseCalculoISS = valorServico;
                nfse.ValorISS = servicoMultiCTe != null ? valorServico * (servicoMultiCTe.Aliquota / 100) : 0;
                nfse.ValorServicos = valorServico;
                if (impostoIBSCBS != null)
                {
                    nfse.OutrasAliquotas = new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas { Codigo = impostoIBSCBS.CodigoOutraAliquota };
                    nfse.NBS = servicoMultiCTe.NBS;
                    nfse.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
                    nfse.CSTIBSCBS = impostoIBSCBS.CST;
                    nfse.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
                    nfse.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                    nfse.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                    nfse.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                    nfse.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                    nfse.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                    nfse.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                    nfse.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                    nfse.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                    nfse.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                    nfse.ValorCBS = impostoIBSCBS.ValorCBS;
                }

                repNFSe.Atualizar(nfse);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao alterar valor serviços NFSe " + ex);
                return false;
            }
        }

        public Dominio.Entidades.NFSe GerarNFSePorObjeto(Dominio.ObjetosDeValor.NFSe.NFSe nfseIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Enumeradores.StatusNFSe status = Dominio.Enumeradores.StatusNFSe.Pendente)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.NotaFiscalServico repNotaFiscalServico = new Repositorio.NotaFiscalServico(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(nfseIntegracao.Emitente.CNPJ);

                if (unitOfWork == null)
                    unidadeDeTrabalho.Start();

                if (nfseIntegracao.Emitente.Atualizar)
                    Servicos.Empresa.AtualizarEmpresa(empresa.Codigo, nfseIntegracao.Emitente, unidadeDeTrabalho);

                DateTime dataEmissao;
                if (!DateTime.TryParseExact(nfseIntegracao.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                    dataEmissao = DateTime.Now;

                List<string> listaOutrasInformacoes = new List<string>();
                if (!string.IsNullOrWhiteSpace(nfseIntegracao.OutrasInformacoes))
                    listaOutrasInformacoes.Add(nfseIntegracao.OutrasInformacoes);

                if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoPadraoNFSe))
                    listaOutrasInformacoes.Add(empresa.Configuracao.ObservacaoPadraoNFSe);

                string outrasInformacoes = string.Join(" - ", listaOutrasInformacoes);

                Dominio.Entidades.NFSe nfse = new Dominio.Entidades.NFSe();

                if (nfseIntegracao.NumeroRPS.HasValue)
                    nfse.NumeroRPS = nfseIntegracao.NumeroRPS;
                nfse.Ambiente = empresa.TipoAmbiente;
                nfse.DataEmissao = dataEmissao;
                nfse.DataIntegracao = DateTime.Now;
                nfse.AliquotaISS = nfseIntegracao.AliquotaISS;
                nfse.Empresa = empresa;
                nfse.ISSRetido = nfseIntegracao.ISSRetido;
                nfse.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigoIBGE(nfseIntegracao.CodigoIBGECidadePrestacaoServico);

                Dominio.Entidades.NotaFiscalServico nfs = new Dominio.Entidades.NotaFiscalServico();
                nfs.TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica;
                repNotaFiscalServico.Inserir(nfs);
                nfse.NFS = nfs;//new Dominio.Entidades.NotaFiscalServico() { TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica };

                nfse.NumeroSubstituicao = nfseIntegracao.NumeroSubstituicao;
                nfse.OutrasInformacoes = outrasInformacoes.Length > 255 ? outrasInformacoes.Substring(0, 255) : outrasInformacoes;
                nfse.Serie = empresa.Configuracao.SerieNFSe;
                nfse.SerieSubstituicao = nfseIntegracao.SerieSubstituicao;
                nfse.Status = status;
                nfse.ValorCOFINS = nfseIntegracao.ValorCOFINS;
                nfse.ValorCSLL = nfseIntegracao.ValorCSLL;
                nfse.ValorDeducoes = nfseIntegracao.ValorDeducoes;
                nfse.ValorDescontoCondicionado = nfseIntegracao.ValorDescontoCondicionado;
                nfse.ValorDescontoIncondicionado = nfseIntegracao.ValorDescontoIncondicionado;
                nfse.ValorINSS = nfseIntegracao.ValorINSS;
                nfse.ValorIR = nfseIntegracao.ValorIR;
                nfse.ValorISSRetido = nfseIntegracao.ValorISSRetido;
                nfse.ValorOutrasRetencoes = nfseIntegracao.ValorOutrasRetencoes;
                nfse.ValorPIS = nfseIntegracao.ValorPIS;
                nfse.PesoKG = nfseIntegracao.PesoKg;
                if (nfseIntegracao.CodigoVeiculo > 0)
                    nfse.Veiculo = repVeiculo.BuscarPorCodigo(nfseIntegracao.CodigoVeiculo);


                if (nfseIntegracao.IBSCBS != null)
                {
                    Dominio.ObjetosDeValor.CTe.IBSCBS iBSCBS = nfseIntegracao.IBSCBS;
                    nfse.NBS = iBSCBS.NBS;
                    nfse.CodigoIndicadorOperacao = iBSCBS.CodigoIndicadorOperacao;
                    nfse.CSTIBSCBS = iBSCBS.CSTIBSCBS;
                    nfse.ClassificacaoTributariaIBSCBS = iBSCBS.ClassificacaoTributariaIBSCBS;
                    nfse.BaseCalculoIBSCBS = iBSCBS.BaseCalculoIBSCBS;
                    nfse.AliquotaIBSEstadual = iBSCBS.AliquotaIBSEstadual;
                    nfse.PercentualReducaoIBSEstadual = iBSCBS.PercentualReducaoIBSEstadual;
                    nfse.ValorIBSEstadual = iBSCBS.ValorIBSEstadual;
                    nfse.AliquotaIBSMunicipal = iBSCBS.AliquotaIBSMunicipal;
                    nfse.PercentualReducaoIBSMunicipal = iBSCBS.PercentualReducaoIBSMunicipal;
                    nfse.ValorIBSMunicipal = iBSCBS.ValorIBSMunicipal;
                    nfse.AliquotaCBS = iBSCBS.AliquotaCBS;
                    nfse.PercentualReducaoCBS = iBSCBS.PercentualReducaoCBS;
                    nfse.ValorCBS = iBSCBS.ValorCBS;
                }

                if (nfseIntegracao.Natureza == null)
                {
                    Dominio.Entidades.NaturezaNFSe naturezaMultiCTe = this.ObterNaturezaNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.LocalidadePrestacaoServico.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unidadeDeTrabalho);
                    if (naturezaMultiCTe != null)
                        nfse.Natureza = naturezaMultiCTe;
                }
                else
                    this.ObterNatureza(ref nfse, nfseIntegracao.Natureza, unidadeDeTrabalho);

                this.ObterParticipante(ref nfse, nfseIntegracao.Intermediario, Dominio.Enumeradores.TipoClienteNotaFiscalServico.Intermediario, unidadeDeTrabalho);
                this.ObterParticipante(ref nfse, nfseIntegracao.Tomador, Dominio.Enumeradores.TipoClienteNotaFiscalServico.Tomador, unidadeDeTrabalho);

                if (empresa.Localidade.Codigo != nfse.Tomador.Localidade.Codigo)
                    nfse.Tomador.InscricaoMunicipal = "";

                nfse.Numero = repNFSe.ObterUltimoNumero(empresa.Codigo, empresa.Configuracao.SerieNFSe.Codigo) + 1;

                repNFSe.Inserir(nfse);

                this.SalvarItens(ref nfse, nfseIntegracao.Itens, null, Dominio.Enumeradores.OpcaoSimNao.Nao, "", unidadeDeTrabalho);
                this.SalvarDocumentos(nfse, nfseIntegracao.Documentos, unidadeDeTrabalho);

                if (nfse.ISSRetido && nfse.ValorISSRetido == 0 && nfse.ValorISS > 0)
                    nfse.ValorISSRetido = nfse.ValorISS;

                repNFSe.Atualizar(nfse);

                if (empresa.Configuracao != null && empresa.Configuracao.UtilizaTabelaDeFrete && nfse.ValorServicos <= 0)
                    this.CalcularFretePorTabelaDeFrete(ref nfse, empresa.Codigo, unidadeDeTrabalho, true, "");

                nfse.NFS.NFSe = nfse;

                if (unitOfWork == null)
                    unidadeDeTrabalho.CommitChanges();

                return nfse;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (unitOfWork == null && unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();

                throw;
            }
        }

        public Dominio.Entidades.NFSe GerarNFSePorObjetoObjetoCTe(Dominio.ObjetosDeValor.CTe.CTeNFSe nfseIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.Enumeradores.StatusNFSe status = Dominio.Enumeradores.StatusNFSe.Pendente, int codigoNFSe = 0)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.NotaFiscalServico repNotaFiscalServico = new Repositorio.NotaFiscalServico(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(nfseIntegracao.Emitente.CNPJ);

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Start();

                if (nfseIntegracao.Emitente.Atualizar)
                    Servicos.Empresa.AtualizarEmpresa(empresa.Codigo, nfseIntegracao.Emitente, unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfse = codigoNFSe > 0 ? repNFSe.BuscarPorCodigo(codigoNFSe) : new Dominio.Entidades.NFSe();

                DateTime dataEmissao = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(nfseIntegracao.DataEmissao))
                    DateTime.TryParseExact(string.Concat(nfseIntegracao.DataEmissao.Substring(0, 16), ":00"), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
                DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

                nfse.DataEmissao = dataEmissao == DateTime.MinValue ? dataFuso : dataEmissao;

                if (nfseIntegracao.NumeroRPS.HasValue)
                    nfse.NumeroRPS = nfseIntegracao.NumeroRPS;
                nfse.Ambiente = empresa.TipoAmbiente;
                nfse.DataIntegracao = DateTime.Now;
                nfse.Empresa = empresa;
                nfse.Serie = empresa.Configuracao.SerieNFSe;
                nfse.Status = status;
                nfse.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigoIBGE(nfseIntegracao.CodigoIBGECidadeInicioPrestacao);
                nfse.Natureza = this.ObterNaturezaNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.LocalidadePrestacaoServico.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unidadeDeTrabalho);

                List<string> listaOutrasInformacoes = new List<string>();
                if (!string.IsNullOrWhiteSpace(nfseIntegracao.ObservacoesGerais))
                    listaOutrasInformacoes.Add(nfseIntegracao.ObservacoesGerais);

                if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoPadraoNFSe))
                    listaOutrasInformacoes.Add(empresa.Configuracao.ObservacaoPadraoNFSe);

                string outrasInformacoes = string.Join(" - ", listaOutrasInformacoes);

                if (codigoNFSe == 0)
                {
                    Dominio.Entidades.NotaFiscalServico nfs = new Dominio.Entidades.NotaFiscalServico();
                    nfs.TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica;
                    repNotaFiscalServico.Inserir(nfs);
                    nfse.NFS = nfs;
                }

                Dominio.Entidades.ServicoNFSe servicoMultiCTe = this.ObterServicoNFSe(empresa, nfse.LocalidadePrestacaoServico.CodigoIBGE != empresa.Localidade.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unidadeDeTrabalho, nfseIntegracao.ServicoNFSe);

                nfse.SerieSubstituicao = string.Empty;
                nfse.OutrasInformacoes = outrasInformacoes.Length > 255 ? outrasInformacoes.Substring(0, 255) : outrasInformacoes;
                nfse.ValorServicos = nfseIntegracao.ValorFrete;

                nfse.ISSRetido = false;
                nfse.AliquotaISS = nfseIntegracao.TributacaoNFSe?.AliquotaISS ?? servicoMultiCTe?.Aliquota ?? 0;
                nfse.BaseCalculoISS = nfseIntegracao.TributacaoNFSe?.BaseCalculoISS ?? (nfse.AliquotaISS > 0 ? nfseIntegracao.ValorFrete : 0);
                nfse.ValorISS = nfseIntegracao.TributacaoNFSe?.ValorISS ?? 0;

                nfse.ValorCOFINS = nfseIntegracao.TributacaoNFSe?.ValorCOFINS ?? 0;
                nfse.ValorCSLL = nfseIntegracao.TributacaoNFSe?.ValorCSLL ?? 0;
                nfse.ValorDeducoes = nfseIntegracao.TributacaoNFSe?.ValorDeducoes ?? 0;
                nfse.ValorDescontoCondicionado = nfseIntegracao.TributacaoNFSe?.ValorDescontoCondicionado ?? 0;
                nfse.ValorDescontoIncondicionado = nfseIntegracao.TributacaoNFSe?.ValorDescontoIncondicionado ?? 0;
                nfse.ValorINSS = nfseIntegracao.TributacaoNFSe?.ValorINSS ?? 0;
                nfse.ValorIR = nfseIntegracao.TributacaoNFSe?.ValorIR ?? 0;
                nfse.ValorOutrasRetencoes = nfseIntegracao.TributacaoNFSe?.ValorOutrasRetencoes ?? 0;
                nfse.ValorPIS = nfseIntegracao.TributacaoNFSe?.ValorPIS ?? 0;

                if (nfseIntegracao.IBSCBS != null)
                {
                    IBSCBS iBSCBS = nfseIntegracao.IBSCBS;

                    nfse.NBS = iBSCBS.NBS;
                    nfse.CodigoIndicadorOperacao = iBSCBS.CodigoIndicadorOperacao;
                    nfse.CSTIBSCBS = iBSCBS.CSTIBSCBS;
                    nfse.ClassificacaoTributariaIBSCBS = iBSCBS.ClassificacaoTributariaIBSCBS;
                    nfse.BaseCalculoIBSCBS = iBSCBS.BaseCalculoIBSCBS;
                    nfse.AliquotaIBSEstadual = iBSCBS.AliquotaIBSEstadual;
                    nfse.PercentualReducaoIBSEstadual = iBSCBS.PercentualReducaoIBSEstadual;
                    nfse.ValorIBSEstadual = iBSCBS.ValorIBSEstadual;
                    nfse.AliquotaIBSMunicipal = iBSCBS.AliquotaIBSMunicipal;
                    nfse.PercentualReducaoIBSMunicipal = iBSCBS.PercentualReducaoIBSMunicipal;
                    nfse.ValorIBSMunicipal = iBSCBS.ValorIBSMunicipal;
                    nfse.AliquotaCBS = iBSCBS.AliquotaCBS;
                    nfse.PercentualReducaoCBS = iBSCBS.PercentualReducaoCBS;
                    nfse.ValorCBS = iBSCBS.ValorCBS;
                }
                nfse.PesoKG = 0;
                if (nfseIntegracao.Veiculos?.Count > 0)
                    nfse.Veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, nfseIntegracao.Veiculos.FirstOrDefault().Placa);

                this.ObterParticipante(ref nfse, nfseIntegracao.TomadorDoCTe, Dominio.Enumeradores.TipoClienteNotaFiscalServico.Tomador, unidadeDeTrabalho);

                nfse.Numero = repNFSe.ObterUltimoNumero(empresa.Codigo, empresa.Configuracao.SerieNFSe.Codigo) + 1;

                if (codigoNFSe == 0)
                    repNFSe.Inserir(nfse);
                else
                    repNFSe.Atualizar(nfse);

                this.SalvarItens(ref nfse, null, nfseIntegracao.ICMS, nfseIntegracao.IncluirICMSNoFrete, nfseIntegracao.ISSNFSeRetido, unidadeDeTrabalho, nfseIntegracao.TributacaoNFSe, servicoMultiCTe);
                this.SalvarDocumentos(nfse, nfseIntegracao.Documentos, nfseIntegracao, unidadeDeTrabalho);

                if (nfse.ISSRetido && nfse.ValorISSRetido == 0 && nfse.ValorISS > 0)
                    nfse.ValorISSRetido = nfse.ValorISS;

                repNFSe.Atualizar(nfse);

                if (empresa.Configuracao != null && empresa.Configuracao.UtilizaTabelaDeFrete && nfse.ValorServicos <= 0)
                    this.CalcularFretePorTabelaDeFrete(ref nfse, nfse.Empresa.Codigo, unidadeDeTrabalho, nfseIntegracao.ICMS != null ? false : true, nfseIntegracao.CodigoTabelaFreteIntegracao);

                if (codigoNFSe == 0)
                    nfse.NFS.NFSe = nfse;

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.CommitChanges();

                return nfse;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();

                throw;
            }
        }

        public Dominio.Entidades.NFSe GerarNFSeTemporariaPorObjetoObjetoCTe(Dominio.ObjetosDeValor.CTe.CTeNFSe nfseIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.NotaFiscalServico repNotaFiscalServico = new Repositorio.NotaFiscalServico(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(nfseIntegracao.Emitente.CNPJ);

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Start();

                Dominio.Entidades.NFSe nfse = new Dominio.Entidades.NFSe();

                DateTime dataEmissao = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(nfseIntegracao.DataEmissao))
                    DateTime.TryParseExact(string.Concat(nfseIntegracao.DataEmissao.Substring(0, 16), ":00"), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
                DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

                nfse.DataEmissao = dataEmissao == DateTime.MinValue ? dataFuso : dataEmissao;

                nfse.Ambiente = empresa.TipoAmbiente;
                nfse.DataIntegracao = DateTime.Now;
                nfse.Empresa = empresa;
                nfse.Serie = empresa.Configuracao.SerieNFSe;
                nfse.Status = Dominio.Enumeradores.StatusNFSe.Pendente;
                nfse.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigoIBGE(nfseIntegracao.CodigoIBGECidadeInicioPrestacao);
                nfse.Natureza = this.ObterNaturezaNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.LocalidadePrestacaoServico.CodigoIBGE, nfse.LocalidadePrestacaoServico.Codigo, unidadeDeTrabalho);

                List<string> listaOutrasInformacoes = new List<string>();
                if (!string.IsNullOrWhiteSpace(nfseIntegracao.ObservacoesGerais))
                    listaOutrasInformacoes.Add(nfseIntegracao.ObservacoesGerais);

                if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoPadraoNFSe))
                    listaOutrasInformacoes.Add(empresa.Configuracao.ObservacaoPadraoNFSe);

                string outrasInformacoes = string.Join(" - ", listaOutrasInformacoes);

                Dominio.Entidades.NotaFiscalServico nfs = new Dominio.Entidades.NotaFiscalServico();
                nfs.TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica;
                repNotaFiscalServico.Inserir(nfs);
                nfse.NFS = nfs;

                nfse.Numero = 0;
                repNFSe.Inserir(nfse);

                nfse.NFS.NFSe = nfse;

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.CommitChanges();

                return nfse;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();

                throw;
            }
        }

        public Dominio.Entidades.NFSe GerarNFSePorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, Dominio.Enumeradores.StatusNFSe status = Dominio.Enumeradores.StatusNFSe.Pendente)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.NotaFiscalServico repNotaFiscalServico = new Repositorio.NotaFiscalServico(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Empresa.CNPJ);

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Start();
                Dominio.Entidades.NFSe nfse = new Dominio.Entidades.NFSe();

                List<string> listaOutrasInformacoes = new List<string>();
                if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais))
                    listaOutrasInformacoes.Add(cte.ObservacoesGerais);

                if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoPadraoNFSe))
                    listaOutrasInformacoes.Add(empresa.Configuracao.ObservacaoPadraoNFSe);

                string outrasInformacoes = string.Join(" - ", listaOutrasInformacoes);

                nfse.Ambiente = empresa.TipoAmbiente;
                nfse.DataEmissao = cte.DataEmissao.Value;
                nfse.DataIntegracao = DateTime.Now;
                nfse.Empresa = empresa;
                nfse.Serie = empresa.Configuracao.SerieNFSe;
                nfse.Status = status;
                nfse.LocalidadePrestacaoServico = cte.LocalidadeInicioPrestacao;
                nfse.Natureza = this.ObterNaturezaNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.LocalidadePrestacaoServico.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unidadeDeTrabalho);

                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, nfse.LocalidadePrestacaoServico.CodigoIBGE != empresa.Localidade.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unidadeDeTrabalho);

                Dominio.Entidades.NotaFiscalServico nfs = new Dominio.Entidades.NotaFiscalServico();
                nfs.TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica;
                repNotaFiscalServico.Inserir(nfs);
                nfse.NFS = nfs;
                nfse.SerieSubstituicao = string.Empty;
                nfse.OutrasInformacoes = outrasInformacoes.Length > 255 ? outrasInformacoes.Substring(0, 255) : outrasInformacoes;
                nfse.ISSRetido = false;
                nfse.AliquotaISS = servicoMultiCTe?.Aliquota ?? 0;
                nfse.BaseCalculoISS = nfse.AliquotaISS > 0 ? cte.BaseCalculoICMS : 0;
                nfse.ValorServicos = cte.ValorFrete;
                decimal valorComponentes = cte.ComponentesPrestacao.Where(o => o.Nome != "FRETE VALOR" && o.Nome != "IMPOSTOS").Sum(o => o.Valor);
                nfse.ValorServicos += valorComponentes;

                nfse.ValorCOFINS = 0;
                nfse.ValorCSLL = 0;
                nfse.ValorDeducoes = 0;
                nfse.ValorDescontoCondicionado = 0;
                nfse.ValorDescontoIncondicionado = 0;
                nfse.ValorINSS = 0;
                nfse.ValorIR = 0;
                nfse.ValorISSRetido = 0;
                nfse.ValorOutrasRetencoes = 0;
                nfse.ValorPIS = 0;

                nfse.NBS = cte.NBS;
                nfse.CodigoIndicadorOperacao = cte.CodigoIndicadorOperacao;
                nfse.CSTIBSCBS = cte.CSTIBSCBS;
                nfse.ClassificacaoTributariaIBSCBS = cte.ClassificacaoTributariaIBSCBS;
                nfse.BaseCalculoIBSCBS = cte.BaseCalculoIBSCBS;
                nfse.AliquotaIBSEstadual = cte.AliquotaIBSEstadual;
                nfse.PercentualReducaoIBSEstadual = cte.PercentualReducaoIBSEstadual;
                nfse.ValorIBSEstadual = cte.ValorIBSEstadual;
                nfse.AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal;
                nfse.PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal;
                nfse.ValorIBSMunicipal = cte.ValorIBSMunicipal;
                nfse.AliquotaCBS = cte.AliquotaCBS;
                nfse.PercentualReducaoCBS = cte.PercentualReducaoCBS;
                nfse.ValorCBS = cte.ValorCBS;


                nfse.PesoKG = 0;
                if (cte.Veiculos?.Count > 0)
                    nfse.Veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, cte.Veiculos.FirstOrDefault().Placa);

                this.ObterParticipanteCTe(ref nfse, cte.Tomador, Dominio.Enumeradores.TipoClienteNotaFiscalServico.Tomador, unidadeDeTrabalho);

                nfse.Numero = repNFSe.ObterUltimoNumero(empresa.Codigo, empresa.Configuracao.SerieNFSe.Codigo) + 1;

                repNFSe.Inserir(nfse);

                this.SalvarItens(ref nfse, null, null, cte.IncluirICMSNoFrete, "", unidadeDeTrabalho);
                if (cte.Documentos != null && cte.Documentos.Count > 0)
                    this.SalvarDocumentosCTe(nfse, cte.Documentos.ToList(), unidadeDeTrabalho);

                if (nfse.ISSRetido && nfse.ValorISSRetido == 0 && nfse.ValorISS > 0)
                    nfse.ValorISSRetido = nfse.ValorISS;

                string observacoes = nfse.OutrasInformacoes;
                if (cte.Documentos != null && cte.Documentos.Count > 0 && !string.IsNullOrWhiteSpace(cte.Documentos.FirstOrDefault().Numero))
                    observacoes = !string.IsNullOrWhiteSpace(observacoes) ? string.Concat(observacoes, " NFe ", cte.Documentos.FirstOrDefault().Numero) : string.Concat("NFe ", cte.Documentos.FirstOrDefault().Numero);
                if (cte.Destinatario != null)
                    observacoes = !string.IsNullOrWhiteSpace(observacoes) ? string.Concat(observacoes, " Destino ", cte.Destinatario.Nome) : string.Concat("Destino ", cte.Destinatario.Nome);
                if (cte.Destinatario.Localidade != null)
                    observacoes = !string.IsNullOrWhiteSpace(observacoes) ? string.Concat(observacoes, " ", cte.Destinatario.Localidade.Descricao, " ", cte.Destinatario.Localidade.Estado.Sigla) : string.Concat(cte.Destinatario.Localidade.Descricao, " ", cte.Destinatario.Localidade.Estado.Sigla);
                if (!string.IsNullOrWhiteSpace(cte.Destinatario.CEP))
                    observacoes = string.Concat(observacoes, " CEP ", cte.Destinatario.CEP);
                if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoPadraoNFSe))
                    observacoes = string.Concat(observacoes, " - ", empresa.Configuracao.ObservacaoPadraoNFSe);

                nfse.OutrasInformacoes = observacoes.Length > 255 ? observacoes.Substring(0, 255) : observacoes;

                nfse.NBS = cte.NBS;
                nfse.CodigoIndicadorOperacao = cte.CodigoIndicadorOperacao;
                nfse.CSTIBSCBS = cte.CSTIBSCBS;
                nfse.ClassificacaoTributariaIBSCBS = cte.ClassificacaoTributariaIBSCBS;
                nfse.BaseCalculoIBSCBS = cte.BaseCalculoIBSCBS;
                nfse.AliquotaIBSEstadual = cte.AliquotaIBSEstadual;
                nfse.PercentualReducaoIBSEstadual = cte.PercentualReducaoIBSEstadual;
                nfse.ValorIBSEstadual = cte.ValorIBSEstadual;
                nfse.AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal;
                nfse.PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal;
                nfse.ValorIBSMunicipal = cte.ValorIBSMunicipal;
                nfse.AliquotaCBS = cte.AliquotaCBS;
                nfse.PercentualReducaoCBS = cte.PercentualReducaoCBS;
                nfse.ValorCBS = cte.ValorCBS;

                repNFSe.Atualizar(nfse);

                nfse.NFS.NFSe = nfse;

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.CommitChanges();

                return nfse;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();

                throw;
            }
        }

        public void Deletar(int codigoNFSe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);
            Repositorio.ItemNFSe repItemNFSe = new Repositorio.ItemNFSe(unidadeDeTrabalho);
            Repositorio.ParcelaCobrancaNFSe repParcelaCobrancaNFSe = new Repositorio.ParcelaCobrancaNFSe(unidadeDeTrabalho);
            Repositorio.CobrancaNFSe repCobrancaNFSe = new Repositorio.CobrancaNFSe(unidadeDeTrabalho);
            Repositorio.ParticipanteNFSe repParticipanteNFSe = new Repositorio.ParticipanteNFSe(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.EmissaoEmail repEmissaoEmail = new Repositorio.EmissaoEmail(unidadeDeTrabalho);

            Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

            if (nfse != null && (nfse.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao || nfse.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao))
            {
                if (nfse.RPS != null)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorRPS(nfse.RPS.Codigo);
                    if (cte != null)
                    {
                        cte.RPS = null;
                        cte.Status = "P";
                        cte.MensagemRetornoSefaz = "NFSE VINCULADA DELETADA DO SISTEMA";
                        repCTe.Atualizar(cte);
                    }
                }

                repIntegracaoNFSe.DeletarPorNFSe(codigoNFSe);
                repItemNFSe.DeletarPorNFSe(codigoNFSe);
                repParcelaCobrancaNFSe.DeletarPorNFSe(codigoNFSe);
                repCobrancaNFSe.DeletarPorNFSe(codigoNFSe);
                repEmissaoEmail.DeletarPorNFSe(codigoNFSe);
                repNFSe.Deletar(nfse);
            }
            else
            {
                throw new Exception("O status da NFS-e não permite a exclusão da mesma.");
            }
        }

        public Dominio.Entidades.NFSe GerarNFSePorListaNFe(List<Dominio.ObjetosDeValor.NFeAdmin> notasFiscais, int codigoEmpresa, DateTime dataEmissao, decimal valorFrete = 0, decimal valorTotalMercadoria = 0, string observacao = "", bool gerarLog = false, Dominio.Entidades.Usuario usuario = null, List<Dominio.Entidades.Veiculo> veiculos = null)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                NFe servicoNFe = new NFe(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                unidadeDeTrabalho.Start();

                Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();
                Dominio.Entidades.Cliente clienteRemetente = null;

                if (notasFiscais[0].NFe2 != null)
                    clienteRemetente = servicoNFe.ObterEmitente(notasFiscais[0].NFe2.NFe.infNFe.emit, codigoEmpresa);
                else if (notasFiscais[0].NFe3 != null)
                    clienteRemetente = servicoNFe.ObterEmitente(notasFiscais[0].NFe3.NFe.infNFe.emit, codigoEmpresa);

                nfse.CodigoIBGECidadePrestacaoServico = clienteRemetente.Localidade.CodigoIBGE;
                nfse.ISSRetido = false;
                nfse.OutrasInformacoes = observacao;

                decimal valorServicos = valorFrete > 0 ? valorFrete : ((from obj in notasFiscais where obj.NFe2 != null select decimal.Parse(obj.NFe2.NFe.infNFe.total.ICMSTot.vFrete, cultura)).Sum() +
                                                               (from obj in notasFiscais where obj.NFe3 != null select decimal.Parse(obj.NFe3.NFe.infNFe.total.ICMSTot.vFrete, cultura)).Sum());

                decimal valorMercadoria = valorTotalMercadoria > 0 ? valorTotalMercadoria : ((from obj in notasFiscais where obj.NFe2 != null select decimal.Parse(obj.NFe2.NFe.infNFe.total.ICMSTot.vNF, cultura)).Sum() +
                                                                                             (from obj in notasFiscais where obj.NFe3 != null select decimal.Parse(obj.NFe3.NFe.infNFe.total.ICMSTot.vNF, cultura)).Sum());

                decimal peso = 0;

                foreach (Dominio.ObjetosDeValor.NFeAdmin notaFiscal in notasFiscais)
                {
                    if (notaFiscal.NFe2 != null)
                    {
                        if (notaFiscal.NFe2.NFe.infNFe.transp != null && notaFiscal.NFe2.NFe.infNFe.transp.vol != null)
                        {
                            foreach (MultiSoftware.NFe.NotaFiscal.TNFeInfNFeTranspVol vol in notaFiscal.NFe2.NFe.infNFe.transp.vol)
                                peso += vol.pesoB != null ? decimal.Parse(vol.pesoB, cultura) : vol.pesoL != null ? decimal.Parse(vol.pesoL, cultura) : 0;
                        }
                    }
                    else if (notaFiscal.NFe3 != null)
                    {
                        if (notaFiscal.NFe3.NFe.infNFe.transp != null && notaFiscal.NFe3.NFe.infNFe.transp.vol != null)
                        {
                            foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspVol vol in notaFiscal.NFe3.NFe.infNFe.transp.vol)
                                peso += vol.pesoB != null ? decimal.Parse(vol.pesoB, cultura) : vol.pesoL != null ? decimal.Parse(vol.pesoL, cultura) : 0;
                        }
                    }
                }

                nfse.PesoKg = peso;

                if (veiculos != null && veiculos.Count > 0)
                    nfse.CodigoVeiculo = veiculos.FirstOrDefault().Codigo;

                nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
                {
                    CNPJ = empresa.CNPJ,
                    Atualizar = false
                };

                nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
                {
                    Bairro = clienteRemetente.Bairro,
                    CEP = clienteRemetente.CEP,
                    CodigoAtividade = clienteRemetente.Atividade.Codigo,
                    CodigoIBGECidade = clienteRemetente.Localidade.CodigoIBGE,
                    CodigoPais = clienteRemetente.Localidade.Estado.Pais.Codigo.ToString(),
                    Complemento = clienteRemetente.Complemento,
                    CPFCNPJ = clienteRemetente.CPF_CNPJ_SemFormato,
                    Endereco = clienteRemetente.Endereco,
                    Exportacao = false,
                    NomeFantasia = clienteRemetente.NomeFantasia,
                    RazaoSocial = clienteRemetente.Nome,
                    RGIE = clienteRemetente.IE_RG,
                    Numero = !string.IsNullOrWhiteSpace(clienteRemetente.Numero) && clienteRemetente.Numero.Length > 2 ? clienteRemetente.Numero : "S/N",
                    Telefone1 = clienteRemetente.Telefone1,
                    Telefone2 = clienteRemetente.Telefone2
                };

                nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();

                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, nfse.CodigoIBGECidadePrestacaoServico != empresa.Localidade.CodigoIBGE, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);

                nfse.Itens.Add(new Dominio.ObjetosDeValor.NFSe.Item()
                {
                    CodigoIBGECidade = nfse.CodigoIBGECidadePrestacaoServico,
                    CodigoIBGECidadeIncidencia = nfse.CodigoIBGECidadePrestacaoServico,
                    Discriminacao = observacao,
                    CodigoPaisPrestacaoServico = int.Parse(empresa.Localidade.Pais.Sigla),
                    Quantidade = 1,
                    ServicoPrestadoNoPais = true,
                    ExigibilidadeISS = 1,
                    ValorServico = valorServicos,
                    BaseCalculoISS = valorServicos,
                    AliquotaISS = servicoMultiCTe?.Aliquota ?? 0,
                    ValorISS = servicoMultiCTe != null ? valorServicos * (servicoMultiCTe.Aliquota / 100) : 0,
                    ValorTotal = valorServicos,
                });

                NFSe svcNFSe = new NFSe(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfseIntegrada = this.GerarNFSePorObjeto(nfse, unidadeDeTrabalho, Dominio.Enumeradores.StatusNFSe.Pendente);

                unidadeDeTrabalho.CommitChanges();

                return nfseIntegrada;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.NFSe CancelarNFSeProcessada(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unidadeDeTrabalho);

            nfse.Status = Dominio.Enumeradores.StatusNFSe.Cancelado;
            repNFSe.Atualizar(nfse);

            if (nfse.RPS != null)
            {
                nfse.RPS.Status = "C";
                nfse.RPS.MensagemRetorno = "NFS-e cancelada pela importacao.";
                nfse.RPS.DataProtocolo = DateTime.Now;
                repRPS.Atualizar(nfse.RPS);
            }

            return nfse;
        }

        public Dominio.Entidades.NFSe GravarNFSeProcessada(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseIntegracao, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.EmpresaSerie serie, Repositorio.UnitOfWork unidadeDeTrabalho, int codigoNFSe = 0)
        {
            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.NotaFiscalServico repNotaFiscalServico = new Repositorio.NotaFiscalServico(unidadeDeTrabalho);

                DateTime dataEmissao;
                if (!DateTime.TryParseExact(nfseIntegracao.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                    dataEmissao = DateTime.Now;

                List<string> listaOutrasInformacoes = new List<string>();
                if (!string.IsNullOrWhiteSpace(nfseIntegracao?.OutrasInformacoes))
                    listaOutrasInformacoes.Add(nfseIntegracao.OutrasInformacoes);

                empresa = repEmpresa.BuscarPorCodigo(empresa.Codigo);
                if (!string.IsNullOrWhiteSpace(empresa.Configuracao?.ObservacaoPadraoNFSe))
                    listaOutrasInformacoes.Add(empresa.Configuracao.ObservacaoPadraoNFSe);

                string outrasInformacoes = string.Join(" - ", listaOutrasInformacoes);


                Dominio.Entidades.NFSe nfse = null;
                if (codigoNFSe > 0)
                    nfse = repNFSe.BuscarPorCodigo(codigoNFSe);
                else
                    nfse = new Dominio.Entidades.NFSe();

                nfse.Ambiente = nfseIntegracao.Ambiente;
                nfse.DataEmissao = dataEmissao;
                nfse.DataIntegracao = DateTime.Now;
                nfse.AliquotaISS = nfseIntegracao.AliquotaISS;
                nfse.Empresa = empresa;
                nfse.ISSRetido = nfseIntegracao.ISSRetido;
                nfse.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigoIBGE(nfseIntegracao.CodigoIBGECidadePrestacaoServico);
                nfse.CFOP = !string.IsNullOrWhiteSpace(nfseIntegracao.CFOP) ? nfseIntegracao.CFOP : string.Empty;

                Dominio.Entidades.NotaFiscalServico nfs = new Dominio.Entidades.NotaFiscalServico();
                nfs.TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica;
                repNotaFiscalServico.Inserir(nfs);
                nfse.NFS = nfs;

                nfse.OutrasInformacoes = outrasInformacoes.Length > 255 ? outrasInformacoes.Substring(0, 255) : outrasInformacoes;
                nfse.Status = !string.IsNullOrWhiteSpace(nfseIntegracao.Status) && nfseIntegracao.Status.Substring(0, 1) == "A" ? Dominio.Enumeradores.StatusNFSe.Autorizado : !string.IsNullOrWhiteSpace(nfseIntegracao.Status) && nfseIntegracao.Status.Substring(0, 1) == "P" ? Dominio.Enumeradores.StatusNFSe.AguardandoAutorizacaoRPS : Dominio.Enumeradores.StatusNFSe.Cancelado;
                nfse.ValorCOFINS = nfseIntegracao.ValorCOFINS;
                nfse.ValorCSLL = nfseIntegracao.ValorCSLL;
                nfse.ValorDeducoes = nfseIntegracao.ValorDeducoes;
                nfse.ValorDescontoCondicionado = nfseIntegracao.ValorDescontoCondicionado;
                nfse.ValorDescontoIncondicionado = nfseIntegracao.ValorDescontoIncondicionado;
                nfse.ValorINSS = nfseIntegracao.ValorINSS;
                nfse.ValorIR = nfseIntegracao.ValorIR;
                nfse.ValorISSRetido = nfseIntegracao.ValorISSRetido;
                nfse.ValorOutrasRetencoes = nfseIntegracao.ValorOutrasRetencoes;
                nfse.ValorPIS = nfseIntegracao.ValorPIS;
                nfse.BaseCalculoISS = nfseIntegracao.BaseCalculoISS;
                nfse.ValorISS = nfseIntegracao.ValorISS > 0 ? nfseIntegracao.ValorISS : nfseIntegracao.ValorISSRetido;
                nfse.AliquotaISS = nfseIntegracao.AliquotaISS;
                nfse.ValorServicos = nfseIntegracao.ValorServicos;

                if (nfseIntegracao.IBSCBS != null)
                {
                    nfse.NBS = nfseIntegracao.IBSCBS.NBS;
                    nfse.CodigoIndicadorOperacao = nfseIntegracao.IBSCBS.CodigoIndicadorOperacao;
                    nfse.CSTIBSCBS = nfseIntegracao.IBSCBS.CSTIBSCBS;
                    nfse.ClassificacaoTributariaIBSCBS = nfseIntegracao.IBSCBS.ClassificacaoTributariaIBSCBS;
                    nfse.BaseCalculoIBSCBS = nfseIntegracao.IBSCBS.BaseCalculoIBSCBS;
                    nfse.AliquotaIBSEstadual = nfseIntegracao.IBSCBS.AliquotaIBSEstadual;
                    nfse.PercentualReducaoIBSEstadual = nfseIntegracao.IBSCBS.PercentualReducaoIBSEstadual;
                    nfse.ValorIBSEstadual = nfseIntegracao.IBSCBS.ValorIBSEstadual;
                    nfse.AliquotaIBSMunicipal = nfseIntegracao.IBSCBS.AliquotaIBSMunicipal;
                    nfse.PercentualReducaoIBSMunicipal = nfseIntegracao.IBSCBS.PercentualReducaoIBSMunicipal;
                    nfse.ValorIBSMunicipal = nfseIntegracao.IBSCBS.ValorIBSMunicipal;
                    nfse.AliquotaCBS = nfseIntegracao.IBSCBS.AliquotaCBS;
                    nfse.PercentualReducaoCBS = nfseIntegracao.IBSCBS.PercentualReducaoCBS;
                    nfse.ValorCBS = nfseIntegracao.IBSCBS.ValorCBS;
                }

                if (veiculo != null)
                    nfse.Veiculo = veiculo;

                this.ObterNatureza(ref nfse, nfseIntegracao.Natureza, unidadeDeTrabalho);

                this.ObterParticipante(ref nfse, nfseIntegracao.Tomador, Dominio.Enumeradores.TipoClienteNotaFiscalServico.Tomador, unidadeDeTrabalho);

                int.TryParse(nfseIntegracao.Serie, out int serieNFSe);

                if (serie == null)
                    serie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieNFSe, Dominio.Enumeradores.TipoSerie.NFSe);
                if (serie == null)
                    serie = empresa.Configuracao?.SerieNFSe;

                nfse.Serie = serie;
                nfse.Numero = nfseIntegracao.Numero;

                if (codigoNFSe > 0)
                    repNFSe.Atualizar(nfse);
                else
                    repNFSe.Inserir(nfse);

                this.SalvarItensNFSeProcessada(ref nfse, nfseIntegracao.Itens, unidadeDeTrabalho);

                if (nfseIntegracao.Documentos != null && nfseIntegracao.Documentos.Count > 0)
                {
                    this.SalvarDocumentosNFSeProcessada(ref nfse, nfseIntegracao.Documentos, unidadeDeTrabalho);
                    repNFSe.Atualizar(nfse);
                }

                nfse.NFS.NFSe = nfse;

                this.SalvarRPSNFSeProcessada(nfse, nfseIntegracao.NumeroRPS, nfseIntegracao.SerieRPS, nfseIntegracao.DataEmissaoRPS, nfseIntegracao.HoraEmissaoRPS, nfseIntegracao.Protocolo, nfseIntegracao.DataProtocolo, unidadeDeTrabalho);

                return nfse;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                throw;
            }
        }

        public Dominio.Entidades.NFSe GravarNFSeProcessadaTemporaria(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseIntegracao, Dominio.Entidades.Empresa empresa, Dominio.Entidades.EmpresaSerie serie, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.NotaFiscalServico repNotaFiscalServico = new Repositorio.NotaFiscalServico(unidadeDeTrabalho);

                DateTime dataEmissao;
                if (!DateTime.TryParseExact(nfseIntegracao.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                    dataEmissao = DateTime.Now;

                Dominio.Entidades.NFSe nfse = new Dominio.Entidades.NFSe();

                nfse.Ambiente = nfseIntegracao.Ambiente;
                nfse.DataEmissao = dataEmissao;
                nfse.DataIntegracao = DateTime.Now;
                nfse.Empresa = empresa;
                nfse.LocalidadePrestacaoServico = empresa.Localidade;
                nfse.Status = Dominio.Enumeradores.StatusNFSe.Pendente;
                nfse.Serie = serie;
                nfse.Numero = nfseIntegracao.Numero;

                repNFSe.Inserir(nfse);

                return nfse;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                throw;
            }
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico ConverterNFSeSaoPauloSPEmCte(Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpNFe nfse, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.ModalTransporte repModalTransporta = new Repositorio.ModalTransporte(unitOfWork);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Servicos.Cliente serCliente = new Cliente(StringConexao);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Dominio.Entidades.Empresa empresaPestrador = repEmpresa.BuscarEmpresaPorCNPJ(nfse.CPFCNPJPrestador.Item);
            try
            {

                cte.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;
                cte.Protocolo = nfse.ChaveNFe.CodigoVerificacao;
                cte.DataEmissao = nfse.DataEmissaoNFe;
                cte.DataIntegracao = DateTime.Now;
                cte.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPrimeiroRegistro();
                cte.Empresa = empresaPestrador;
                cte.ISSRetido = nfse.ISSRetido;
                cte.Numero = (int)nfse.ChaveNFe.NumeroNFe;
                cte.ObservacoesGerais = nfse.Discriminacao.Length > 255 ? nfse.Discriminacao.Substring(0, 255) : nfse.Discriminacao;
                cte.Status = nfse.StatusNFe == Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas.tpStatusNFe.C ? "C" : "A";

                Dominio.Entidades.ParticipanteCTe tomador = repParticipanteCTe.BuscarPorCPFCNPJ(nfse.CPFCNPJTomador.Item);
                if (tomador == null)
                    tomador = serCliente.ConverterClienteParaParticipanteCTe(repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.CPFCNPJTomador.Item)));

                cte.TomadorPagador = tomador;
                cte.Destinatario = tomador;

                Dominio.Entidades.ParticipanteCTe remetente = repParticipanteCTe.BuscarPorCPFCNPJ(nfse.CPFCNPJPrestador.Item);

                if (remetente == null)
                {
                    remetente = serCliente.ConverterClienteParaParticipanteCTe(repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.CPFCNPJPrestador.Item)));
                    if (remetente == null)
                    {
                        remetente = new Dominio.Entidades.ParticipanteCTe
                        {
                            CPF_CNPJ = nfse.CPFCNPJPrestador.Item,
                            Tipo = nfse.CPFCNPJPrestador.Item.Length == 14 ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica,
                            Nome = nfse.RazaoSocialPrestador,
                            Bairro = nfse.EnderecoPrestador.Bairro,
                            CEP = nfse.EnderecoPrestador.CEP.ToString(),
                            Numero = nfse.EnderecoPrestador.NumeroEndereco,
                            Endereco = nfse.EnderecoPrestador.Logradouro,
                            Localidade = repLocalidade.BuscarPorCodigoIBGE(nfse.EnderecoTomador.Cidade)
                        };
                    }
                    repParticipanteCTe.Inserir(remetente);
                }

                cte.Remetente = remetente;
                cte.LocalidadeEmissao = remetente.Localidade;

                cte.Serie = repSerie.BuscarPorEmpresaNumeroTipo(empresaPestrador?.Codigo ?? 0, nfse.ChaveRPS?.SerieRPS?.ToInt() ?? 0, Dominio.Enumeradores.TipoSerie.NFSe);

                if (cte.Serie == null)
                {
                    cte.Serie = new Dominio.Entidades.EmpresaSerie()
                    {
                        Empresa = empresaPestrador,
                        NaoGerarCargaAutomaticamente = true,
                        Tipo = Dominio.Enumeradores.TipoSerie.NFSe,
                        Status = "A",
                        Numero = nfse.ChaveRPS?.SerieRPS?.ToInt() ?? 0
                    };
                    repSerie.Inserir(cte.Serie);
                }

                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    cte.OutrosTomador = tomador;

                cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
                cte.TipoCTE = Dominio.Enumeradores.TipoCTE.Normal;
                cte.ValorCOFINS = nfse.ValorCOFINS;
                cte.ValorCSLL = nfse.ValorCSLL;
                cte.ValorDeducoes = nfse.ValorDeducoes;
                cte.ValorINSS = nfse.ValorINSS;
                cte.ValorIR = nfse.ValorIR;
                cte.ValorISS = nfse.ValorISS;
                cte.ValorISSRetido = nfse.ValorISS;
                cte.ValorPIS = nfse.ValorPIS;
                cte.ValorFrete = nfse.ValorServicos;
                cte.ValorAReceber = nfse.ValorServicos;
                cte.ValorPrestacaoServico = nfse.ValorServicos;
                cte.ModalTransporte = repModalTransporta.BuscarPorCodigo(1, false);
                cte.CFOP = repCFOP.BuscarPorNumero(5932);
                cte.Empresa = repEmpresa.BuscarPorCNPJ(nfse.CPFCNPJPrestador.Item);

                if (cte.Empresa == null)
                {
                    Servicos.Log.TratarErro("CNPJ: " + nfse.CPFCNPJPrestador.Item + " não está cadastrado como transportador.");
                    return null;
                }

                repCTe.Inserir(cte);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao Gerar CTe de NFSe São Paulo/SP: " + ex);
                return null;
            }

            return cte;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico ConverterNFSeEmCTe(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork, bool controlarTransacao = true)
        {
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);
                Servicos.Cliente serCliente = new Cliente(StringConexao);
                Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);
                Repositorio.SeguroCTE repSeguroCTe = new Repositorio.SeguroCTE(unitOfWork);
                Repositorio.ItemNFSe repItemNFSe = new Repositorio.ItemNFSe(unitOfWork);
                Repositorio.NFSeItem repNFSeItem = new Repositorio.NFSeItem(unitOfWork);
                Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unitOfWork);
                Repositorio.ModalTransporte repModalTransporta = new Repositorio.ModalTransporte(unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
                Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);
                Repositorio.MotoristaCTE repMotoristaCTE = new Repositorio.MotoristaCTE(unitOfWork);
                Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

                if (controlarTransacao)
                    unitOfWork.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

                if (nfse != null)
                {
                    List<string> listaOutrasInformacoes = new List<string>();
                    if (!string.IsNullOrWhiteSpace(nfse.OutrasInformacoes))
                        listaOutrasInformacoes.Add(nfse.OutrasInformacoes);

                    if (!string.IsNullOrWhiteSpace(nfse.Empresa.Configuracao.ObservacaoPadraoNFSe))
                        listaOutrasInformacoes.Add(nfse.Empresa.Configuracao.ObservacaoPadraoNFSe);

                    string outrasInformacoes = string.Join(" - ", listaOutrasInformacoes);

                    cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
                    cte.AliquotaISS = nfse.AliquotaISS;
                    cte.TipoAmbiente = nfse.Ambiente;
                    cte.BaseCalculoISS = nfse.BaseCalculoISS;

                    cte.Protocolo = nfse.CodigoVerificacao;

                    cte.DataEmissao = nfse.DataEmissao;
                    cte.DataIntegracao = nfse.DataIntegracao;
                    cte.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPrimeiroRegistro();
                    cte.Empresa = nfse.Empresa;
                    cte.ISSRetido = nfse.ISSRetido;
                    cte.CodigoCTeIntegrador = nfse.RPS != null ? nfse.RPS.CodigoIntegracao : 0;
                    cte.ObservacaoCancelamento = nfse.JustificativaCancelamento;
                    cte.LocalidadeEmissao = cte.Empresa.Localidade;
                    cte.LocalidadeInicioPrestacao = nfse.LocalidadePrestacaoServico;
                    cte.LocalidadeTerminoPrestacao = nfse.LocalidadePrestacaoServico;
                    cte.NaturezaNFSe = nfse.Natureza;
                    cte.Numero = nfse.Numero;
                    cte.NumeroPrefeituraNFSe = nfse.NumeroPrefeitura;
                    cte.NumeroSubstituicao = nfse.NumeroSubstituicao;
                    cte.ObservacoesGerais = outrasInformacoes.Length > 255 ? outrasInformacoes.Substring(0, 255) : outrasInformacoes;
                    cte.RPS = nfse.RPS;
                    cte.Serie = nfse.Serie;
                    cte.SerieSubstituicao = nfse.SerieSubstituicao;
                    cte.Status = nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "A" : (nfse.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao ? "R" : (nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? "C" : "N"));

                    List<Dominio.Entidades.DocumentosNFSe> listaDocumentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);

                    Dominio.Entidades.Cliente remetenteNFe = null;
                    if (listaDocumentos != null && listaDocumentos.Count > 0 && !string.IsNullOrWhiteSpace(listaDocumentos.FirstOrDefault().Emitente))
                        remetenteNFe = repCliente.BuscarPorCPFCNPJ(double.Parse(listaDocumentos.FirstOrDefault().Emitente));

                    Dominio.Entidades.Cliente destinatarioNFe = null;
                    if (listaDocumentos != null && listaDocumentos.Count > 0 && !string.IsNullOrWhiteSpace(listaDocumentos.FirstOrDefault().Destino))
                        destinatarioNFe = repCliente.BuscarPorCPFCNPJ(double.Parse(listaDocumentos.FirstOrDefault().Destino));

                    Dominio.Entidades.ParticipanteCTe tomador = serCliente.ConverterClienteParaParticipanteCTe(repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.Tomador.CPF_CNPJ)));
                    repParticipanteCTe.Inserir(tomador);
                    cte.TomadorPagador = tomador;
                    Dominio.Entidades.ParticipanteCTe remetente = serCliente.ConverterClienteParaParticipanteCTe(repCliente.BuscarPorCPFCNPJ(remetenteNFe != null ? remetenteNFe.CPF_CNPJ : double.Parse(nfse.Tomador.CPF_CNPJ)));
                    repParticipanteCTe.Inserir(remetente);
                    cte.Remetente = remetente;
                    Dominio.Entidades.ParticipanteCTe detinatario = serCliente.ConverterClienteParaParticipanteCTe(repCliente.BuscarPorCPFCNPJ(destinatarioNFe != null ? destinatarioNFe.CPF_CNPJ : double.Parse(nfse.Tomador.CPF_CNPJ)));
                    cte.Destinatario = detinatario;
                    cte.Remetente = remetente;
                    cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    cte.TipoTomador = remetente.CPF_CNPJ == tomador.CPF_CNPJ ? Dominio.Enumeradores.TipoTomador.Remetente : detinatario.CPF_CNPJ == tomador.CPF_CNPJ ? Dominio.Enumeradores.TipoTomador.Destinatario : Dominio.Enumeradores.TipoTomador.Outros;
                    if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                        cte.OutrosTomador = tomador;
                    cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
                    cte.TipoCTE = Dominio.Enumeradores.TipoCTE.Normal;
                    cte.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
                    //if (cte.Status == "A")
                    //    cte.TipoControle = 1;
                    //else
                    cte.TipoControle = repCTe.BuscarUltimoTipoControlePorModelo(cte.ModeloDocumentoFiscal.Codigo) + 1; //Como prefeitura de PASSOG MG começa a numeração do 1 todo ano é necessário buscar novamente o código

                    cte.ValorCOFINS = nfse.ValorCOFINS;
                    cte.ValorCSLL = nfse.ValorCSLL;
                    cte.ValorDeducoes = nfse.ValorDeducoes;
                    cte.ValorDescontoCondicionado = nfse.ValorDescontoCondicionado;
                    cte.ValorDescontoIncondicionado = nfse.ValorDescontoIncondicionado;
                    cte.ValorINSS = nfse.ValorINSS;
                    cte.ValorIR = nfse.ValorIR;
                    cte.ValorISS = nfse.ValorISS;
                    cte.ValorISSRetido = nfse.ValorISSRetido;
                    cte.ValorOutrasRetencoes = nfse.ValorOutrasRetencoes;
                    cte.ValorPIS = nfse.ValorPIS;
                    cte.ValorFrete = nfse.ValorServicos;
                    cte.ValorAReceber = nfse.ValorServicos;
                    cte.ValorPrestacaoServico = nfse.ValorServicos;
                    cte.CFOP = repCFOP.BuscarPorNumero(5932);

                    cte.OutrasAliquotas = nfse.OutrasAliquotas;
                    cte.NBS = nfse.NBS;
                    cte.CodigoIndicadorOperacao = nfse.CodigoIndicadorOperacao;
                    cte.CSTIBSCBS = nfse.CSTIBSCBS;
                    cte.ClassificacaoTributariaIBSCBS = nfse.ClassificacaoTributariaIBSCBS;
                    cte.BaseCalculoIBSCBS = nfse.BaseCalculoIBSCBS;
                    cte.AliquotaIBSEstadual = nfse.AliquotaIBSEstadual;
                    cte.PercentualReducaoIBSEstadual = nfse.PercentualReducaoIBSEstadual;
                    cte.ValorIBSEstadual = nfse.ValorIBSEstadual;
                    cte.AliquotaIBSMunicipal = nfse.AliquotaIBSMunicipal;
                    cte.PercentualReducaoIBSMunicipal = nfse.PercentualReducaoIBSMunicipal;
                    cte.ValorIBSMunicipal = nfse.ValorIBSMunicipal;
                    cte.AliquotaCBS = nfse.AliquotaCBS;
                    cte.PercentualReducaoCBS = nfse.PercentualReducaoCBS;
                    cte.ValorCBS = nfse.ValorCBS;

                    cte.ModalTransporte = repModalTransporta.BuscarPorCodigo(1, false);

                    if (listaDocumentos != null && listaDocumentos.Count > 0)
                        cte.ValorTotalMercadoria = listaDocumentos.Sum(o => o.Valor);

                    repCTe.Inserir(cte);

                    if (listaDocumentos != null && listaDocumentos.Count > 0)
                    {
                        foreach (Dominio.Entidades.DocumentosNFSe documentos in listaDocumentos)
                        {
                            Dominio.Entidades.DocumentosCTE doc = new Dominio.Entidades.DocumentosCTE();
                            doc.Numero = !string.IsNullOrWhiteSpace(documentos.Numero) ? documentos.Numero : Utilidades.Chave.ObterNumero(documentos.Chave).ToString();
                            doc.Serie = documentos.Serie;
                            doc.CFOP = "";
                            doc.ChaveNFE = documentos.Chave;
                            doc.CTE = cte;
                            doc.DataEmissao = documentos.DataEmissao.HasValue ? documentos.DataEmissao.Value : DateTime.Today;
                            doc.Descricao = "";
                            doc.DescricaoCTe = "";
                            doc.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("55");
                            doc.Peso = documentos.Peso;
                            doc.Valor = documentos.Valor;
                            doc.ValorProdutos = documentos.Valor;
                            doc.Volume = 0;
                            if (!string.IsNullOrWhiteSpace(doc.ChaveNFE))
                                doc.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(doc.ChaveNFE);

                            repDocumentosCTE.Inserir(doc);
                        }
                    }

                    Dominio.Entidades.InformacaoCargaCTE infoCTe = new Dominio.Entidades.InformacaoCargaCTE();
                    infoCTe.CTE = cte;
                    infoCTe.Quantidade = nfse.PesoKG;
                    infoCTe.Tipo = "Kilograma";
                    infoCTe.UnidadeMedida = "01";
                    repInformacaoCargaCTe.Inserir(infoCTe);

                    Dominio.Entidades.SeguroCTE seguroCte = new Dominio.Entidades.SeguroCTE();
                    seguroCte.CNPJSeguradora = "";
                    seguroCte.CTE = cte;
                    seguroCte.NomeSeguradora = "";
                    seguroCte.NumeroAverbacao = "";
                    seguroCte.Tipo = Dominio.Enumeradores.TipoSeguro.Remetente;
                    seguroCte.Valor = 0;
                    repSeguroCTe.Inserir(seguroCte);

                    List<Dominio.Entidades.ItemNFSe> itens = repItemNFSe.BuscarPorNFSe(nfse.Codigo);

                    foreach (Dominio.Entidades.ItemNFSe item in itens)
                    {
                        Dominio.Entidades.NFSeItem nfseItem = new Dominio.Entidades.NFSeItem();
                        nfseItem.AliquotaISS = item.AliquotaISS;
                        nfseItem.BaseCalculoISS = item.BaseCalculoISS;
                        nfseItem.CTe = cte;
                        nfseItem.Discriminacao = item.Discriminacao;
                        nfseItem.ExigibilidadeISS = item.ExigibilidadeISS;
                        nfseItem.IncluirISSNoFrete = item.IncluirISSNoFrete;
                        nfseItem.Municipio = item.Municipio;
                        nfseItem.MunicipioIncidencia = item.MunicipioIncidencia;
                        nfseItem.PaisPrestacaoServico = item.PaisPrestacaoServico;
                        nfseItem.Quantidade = item.Quantidade;
                        nfseItem.Servico = item.Servico;
                        nfseItem.ServicoPrestadoNoPais = item.ServicoPrestadoNoPais;
                        nfseItem.ValorDeducoes = item.ValorDeducoes;
                        nfseItem.ValorDescontoCondicionado = item.ValorDescontoCondicionado;
                        nfseItem.ValorDescontoIncondicionado = item.ValorDescontoIncondicionado;
                        nfseItem.ValorISS = item.ValorISS;
                        nfseItem.ValorServico = item.ValorServico;
                        nfseItem.ValorTotal = item.ValorTotal;

                        nfseItem.OutrasAliquotas = nfse.OutrasAliquotas;
                        nfseItem.NBS = nfse.NBS;
                        nfseItem.CodigoIndicadorOperacao = nfse.CodigoIndicadorOperacao;
                        nfseItem.CSTIBSCBS = nfse.CSTIBSCBS;
                        nfseItem.ClassificacaoTributariaIBSCBS = nfse.ClassificacaoTributariaIBSCBS;
                        nfseItem.BaseCalculoIBSCBS = nfse.BaseCalculoIBSCBS;
                        nfseItem.AliquotaIBSEstadual = nfse.AliquotaIBSEstadual;
                        nfseItem.PercentualReducaoIBSEstadual = nfse.PercentualReducaoIBSEstadual;
                        nfseItem.ValorIBSEstadual = nfse.ValorIBSEstadual;
                        nfseItem.AliquotaIBSMunicipal = nfse.AliquotaIBSMunicipal;
                        nfseItem.PercentualReducaoIBSMunicipal = nfse.PercentualReducaoIBSMunicipal;
                        nfseItem.ValorIBSMunicipal = nfse.ValorIBSMunicipal;
                        nfseItem.AliquotaCBS = nfse.AliquotaCBS;
                        nfseItem.PercentualReducaoCBS = nfse.PercentualReducaoCBS;
                        nfseItem.ValorCBS = nfse.ValorCBS;

                        repNFSeItem.Inserir(nfseItem);
                    }

                    if (nfse.Veiculo != null)
                    {
                        Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE();
                        veiculoCTe.Veiculo = nfse.Veiculo;
                        veiculoCTe.SetarDadosVeiculo(nfse.Veiculo);
                        veiculoCTe.CTE = cte;
                        veiculoCTe.Placa = nfse.Veiculo.Placa;
                        veiculoCTe.Estado = nfse.Veiculo.Estado;
                        veiculoCTe.TipoVeiculo = nfse.Veiculo.TipoVeiculo;

                        repVeiculoCTe.Inserir(veiculoCTe);

                        Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(nfse.Veiculo.Codigo);
                        if (veiculoMotorista != null)
                        {
                            Dominio.Entidades.MotoristaCTE motoristaCTe = new Dominio.Entidades.MotoristaCTE();
                            motoristaCTe.CTE = cte;
                            motoristaCTe.CPFMotorista = veiculoMotorista.CPF;
                            motoristaCTe.NomeMotorista = veiculoMotorista.Nome;
                            repMotoristaCTE.Inserir(motoristaCTe);
                        }
                    }

                    Dominio.Entidades.XMLNFSe xmlNFN = repXMLNFSe.BuscarPorNFSe(nfse.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao);
                    if (xmlNFN != null)
                    {
                        Dominio.Entidades.XMLCTe xmlCTe = new Dominio.Entidades.XMLCTe();
                        xmlCTe.CTe = cte;
                        xmlCTe.Tipo = Dominio.Enumeradores.TipoXMLCTe.Autorizacao;
                        xmlCTe.XML = xmlNFN.XML;
                        repXMLCTe.Inserir(xmlCTe);
                    }
                    repCTe.Atualizar(cte);
                }

                if (controlarTransacao)
                    unitOfWork.CommitChanges();

                return cte;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao Gerar CTe de NFSe: " + ex);

                if (controlarTransacao)
                    unitOfWork.Rollback();
                return null;
            }
        }

        public string ConsultarProtocoloPrefeituraNFSe(int codigoNFSe, string protocoloNFSe, Repositorio.UnitOfWork unitOfWork = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(StringConexao);

            Repositorio.ConhecimentoDeTransporteEletronico repNFSe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

            ServicoNFSe.uNFSeServiceTSSoapClient svcNFSe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNFSe.uNFSeServiceTSSoapClient, ServicoNFSe.uNFSeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uNFSeServiceTS);

            ServicoNFSe.ResultadoInteger retorno = svcNFSe.ConsultarProtocoloNFSePrefeitura(nfse.CodigoCTeIntegrador, protocoloNFSe);

            if (retorno.Info.Tipo == "OK")
            {
                nfse.Status = "E";
                repNFSe.Atualizar(nfse);

                return string.Empty;
            }
            else
                return Utilidades.String.ReplaceInvalidCharacters(retorno.Info.Mensagem);
        }

        public Dominio.Entidades.ServicoNFSe ObterServicoNFSe(Dominio.Entidades.Empresa empresa, bool servicoForaDoMunicipioPrestador, int ibgePresatacaoServico, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.CTe.ServicoNFSe servicoNFSe = null)
        {
            if (servicoNFSe != null)
            {
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);
                Dominio.Entidades.ServicoNFSe servico = repServicoNFSe.BuscarPorNumero(empresa.Codigo, servicoNFSe.Numero);
                bool inserirNovo = false;
                if (servico == null)
                {
                    servico = new Dominio.Entidades.ServicoNFSe();
                    inserirNovo = true;
                }

                servico.Numero = servicoNFSe.Numero;
                servico.Descricao = servicoNFSe.Descricao;
                servico.CodigoTributacao = servicoNFSe.CodigoTributacao;
                servico.CNAE = servicoNFSe.CNAE;
                servico.Empresa = empresa;
                servico.Status = "A";

                if (inserirNovo)
                    repServicoNFSe.Inserir(servico);
                else
                    repServicoNFSe.Atualizar(servico);

                return servico;
            }
            else
            {
                if (empresa == null && empresa.Configuracao == null)
                    return null;

                if (ibgePresatacaoServico > 0)
                {
                    Repositorio.ServicoNFsePorCidade repServicoNFsePorCidade = new Repositorio.ServicoNFsePorCidade(unitOfWork);
                    Dominio.Entidades.ServicoNFsePorCidade servicoNFsePorCidade = repServicoNFsePorCidade.BuscarPorCidadeIBGE(empresa.Configuracao.Codigo, ibgePresatacaoServico);
                    if (servicoNFsePorCidade != null)
                        return servicoNFsePorCidade.ServicoNFSe;
                }

                if (servicoForaDoMunicipioPrestador && empresa.Configuracao.ServicoNFSeFora != null)
                    return empresa.Configuracao.ServicoNFSeFora;
                else
                    return empresa.Configuracao.ServicoNFSe;
            }
        }

        public Dominio.Entidades.NaturezaNFSe ObterNaturezaNFSe(Dominio.Entidades.Empresa empresa, bool servicoForaDoMunicipioPrestador, int ibgePresatacaoServico, Repositorio.UnitOfWork unitOfWork)
        {
            if (empresa == null && empresa.Configuracao == null)
                return null;

            if (ibgePresatacaoServico > 0)
            {
                Repositorio.ServicoNFsePorCidade repServicoNFsePorCidade = new Repositorio.ServicoNFsePorCidade(unitOfWork);
                Dominio.Entidades.ServicoNFsePorCidade servicoNFsePorCidade = repServicoNFsePorCidade.BuscarPorCidadeIBGE(empresa.Configuracao.Codigo, ibgePresatacaoServico);
                if (servicoNFsePorCidade != null)
                    return servicoNFsePorCidade.NaturezaNFSe;
            }

            if (servicoForaDoMunicipioPrestador && empresa.Configuracao.ServicoNFSeFora != null)
                return empresa.Configuracao.NaturezaNFSeFora;
            else
                return empresa.Configuracao.NaturezaNFSe;
        }

        public object ObterDocumentoPorXMLNFSeAutorizada(Stream xml, Dominio.Entidades.Empresa empresa)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseIntegracao = new Dominio.ObjetosDeValor.NFSe.NFSeProcessada();

            unitOfWork.Start();

            try
            {
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                xml.Position = 0;
                XDocument doc = XDocument.Load(xml);
                object retorno = null;

                if (empresa.Localidade.CodigoIBGE == 3534401) //Osasco - SP
                    retorno = this.ConverterXMLNFSeOsasco(nfseIntegracao, doc, empresa, unitOfWork, cultura);

                if (retorno != null)
                {
                    if (retorno.GetType() == typeof(string))
                        return (string)retorno;
                    else
                    {
                        Dominio.Entidades.EmpresaSerie serie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, nfseIntegracao.Serie.ToInt(), Dominio.Enumeradores.TipoSerie.NFSe);
                        if (serie == null)
                            serie = empresa.Configuracao?.SerieNFSe;

                        Dominio.Entidades.NFSe nfseExiste = repNFSe.BuscarPorNumeroEStatus(empresa.Codigo, nfseIntegracao.Numero, nfseIntegracao.Serie.ToInt(), Dominio.Enumeradores.StatusNFSe.Autorizado, empresa.TipoAmbiente);
                        if (nfseExiste != null)
                            return "Nota Fiscal já importada no sistema";

                        Dominio.Entidades.NFSe nfse = this.GravarNFSeProcessada(nfseIntegracao, empresa, null, null, unitOfWork);
                        this.ConverterNFSeEmCTe(nfse, unitOfWork, false);

                        unitOfWork.CommitChanges();

                        return null;
                    }
                }

                unitOfWork.Rollback();
                return "Importação do XML para a prefeitura desta cidade não está disponível, favor fazer o lançamento manual.";
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.NFSe GerarNFSePorXMLNotaFiscalEletronica(List<Dominio.Entidades.XMLNotaFiscalEletronica> notasFiscais, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                Servicos.Cliente serCliente = new Servicos.Cliente();

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();
                Dominio.Entidades.Cliente clienteTomador = notasFiscais[0].UtilizarContratanteComoTomador && notasFiscais[0].Contratante != null ? notasFiscais[0].Contratante : notasFiscais[0].Emitente;
                nfse.CodigoIBGECidadePrestacaoServico = notasFiscais[0].UtilizarContratanteComoTomador && notasFiscais[0].Contratante != null ? notasFiscais[0].Contratante.Localidade.CodigoIBGE : clienteTomador.Localidade.CodigoIBGE;

                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
                DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

                nfse.DataEmissao = dataFuso.ToString("dd/MM/yyyy HH:mm");
                nfse.ISSRetido = false;
                //nfse.OutrasInformacoes = observacao;

                decimal valorServicos = (from obj in notasFiscais select obj.ValorDoFrete).Sum();
                decimal valorNotasFiscais = (from obj in notasFiscais select obj.Valor).Sum();
                nfse.PesoKg = (from obj in notasFiscais select obj.Peso).Sum(); ;
                string codigoTabelaFreteIntegracao = string.Empty;

                nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
                {
                    CNPJ = empresa.CNPJ,
                    Atualizar = false
                };

                nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
                {
                    Bairro = clienteTomador.Bairro,
                    CEP = clienteTomador.CEP,
                    CodigoAtividade = clienteTomador.Atividade.Codigo,
                    CodigoIBGECidade = clienteTomador.Localidade.CodigoIBGE,
                    CodigoPais = clienteTomador.Localidade.Estado.Pais.Codigo.ToString(),
                    Complemento = clienteTomador.Complemento,
                    CPFCNPJ = clienteTomador.CPF_CNPJ_SemFormato,
                    Endereco = clienteTomador.Endereco,
                    Exportacao = false,
                    NomeFantasia = clienteTomador.NomeFantasia,
                    RazaoSocial = clienteTomador.Nome,
                    RGIE = clienteTomador.IE_RG,
                    Numero = !string.IsNullOrWhiteSpace(clienteTomador.Numero) && clienteTomador.Numero.Length > 2 ? clienteTomador.Numero : "S/N",
                    Telefone1 = clienteTomador.Telefone1,
                    Telefone2 = clienteTomador.Telefone2
                };

                nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();
                Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, nfse.CodigoIBGECidadePrestacaoServico != empresa.Localidade.CodigoIBGE, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);
                nfse.Itens.Add(new Dominio.ObjetosDeValor.NFSe.Item()
                {
                    CodigoIBGECidade = nfse.CodigoIBGECidadePrestacaoServico,
                    CodigoIBGECidadeIncidencia = nfse.CodigoIBGECidadePrestacaoServico,
                    Discriminacao = servicoMultiCTe.Descricao,
                    CodigoPaisPrestacaoServico = int.Parse(empresa.Localidade.Pais.Sigla),
                    Quantidade = 1,
                    ServicoPrestadoNoPais = true,
                    ExigibilidadeISS = 1,
                    ValorServico = valorServicos,
                    BaseCalculoISS = valorServicos,
                    AliquotaISS = servicoMultiCTe?.Aliquota ?? 0,
                    ValorISS = empresa.OptanteSimplesNacional && empresa.Localidade.CodigoIBGE == 3518800 ? 0 : servicoMultiCTe != null ? nfse.ValorServicos * (servicoMultiCTe.Aliquota / 100) : 0, //Quando empresa do Simples e de Guarulhos o valor do ISS deve zer zerado
                    ValorTotal = valorServicos,
                });

                nfse.Documentos = new List<Dominio.ObjetosDeValor.NFSe.Documentos>();
                for (int i = 0; i < notasFiscais.Count(); i++)
                {
                    string serieNota = !string.IsNullOrWhiteSpace(notasFiscais[i].Chave) && notasFiscais[i].Chave.Length == 44 ? notasFiscais[i].Chave.Substring(22, 3) : "0";

                    nfse.Documentos.Add(new Dominio.ObjetosDeValor.NFSe.Documentos()
                    {
                        ChaveNFE = notasFiscais[i].Chave,
                        DataEmissao = notasFiscais[i].Numero,
                        Numero = notasFiscais[i].Numero,
                        Serie = serieNota,
                        Peso = notasFiscais[i].Peso,
                        Valor = notasFiscais[i].Valor,
                        EmitenteNFe = new Dominio.ObjetosDeValor.CTe.Cliente()
                        {
                            CPFCNPJ = notasFiscais[i].Emitente.CPF_CNPJ_SemFormato,
                            Bairro = notasFiscais[i].Emitente.Bairro,
                            CEP = notasFiscais[i].Emitente.CEP,
                            CodigoAtividade = notasFiscais[i].Emitente.Atividade.Codigo,
                            CodigoIBGECidade = notasFiscais[i].Emitente.Localidade.CodigoIBGE,
                            Endereco = notasFiscais[i].Emitente.Endereco,
                            NomeFantasia = notasFiscais[i].Emitente.NomeFantasia,
                            Numero = notasFiscais[i].Emitente.Numero,
                            RazaoSocial = notasFiscais[i].Emitente.Nome,
                            RGIE = notasFiscais[i].Emitente.IE_RG
                        },
                        DestinatarioNFe = new Dominio.ObjetosDeValor.CTe.Cliente()
                        {
                            CPFCNPJ = notasFiscais[i].Destinatario.CPF_CNPJ_SemFormato,
                            Bairro = notasFiscais[i].Destinatario.Bairro,
                            CEP = notasFiscais[i].Destinatario.CEP,
                            CodigoAtividade = notasFiscais[i].Destinatario.Atividade.Codigo,
                            CodigoIBGECidade = notasFiscais[i].Destinatario.Localidade.CodigoIBGE,
                            Endereco = notasFiscais[i].Destinatario.Endereco,
                            NomeFantasia = notasFiscais[i].Destinatario.NomeFantasia,
                            Numero = notasFiscais[i].Destinatario.Numero,
                            RazaoSocial = notasFiscais[i].Destinatario.Nome,
                            RGIE = notasFiscais[i].Destinatario.IE_RG
                        }
                    });
                }

                Dominio.Entidades.NFSe nfseIntegrada = this.GerarNFSePorObjeto(nfse, unidadeDeTrabalho, Dominio.Enumeradores.StatusNFSe.EmDigitacao);

                return nfseIntegrada;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();
                throw;
            }
        }

        #endregion

        #region Métodos Privados

        #region Emissão

        private ServicoNFSe.ExigibilidadeISS ObterExibilidadeISS(Dominio.Enumeradores.ExigibilidadeISS exigibilidadeISS)
        {
            switch (exigibilidadeISS)
            {
                case Dominio.Enumeradores.ExigibilidadeISS.Exigivel: return ServicoNFSe.ExigibilidadeISS.Exigivel;
                case Dominio.Enumeradores.ExigibilidadeISS.NaoInicidencia: return ServicoNFSe.ExigibilidadeISS.NaoIncidencia;
                case Dominio.Enumeradores.ExigibilidadeISS.Isencao: return ServicoNFSe.ExigibilidadeISS.Isencao;
                case Dominio.Enumeradores.ExigibilidadeISS.Exportacao: return ServicoNFSe.ExigibilidadeISS.Exportacao;
                case Dominio.Enumeradores.ExigibilidadeISS.Imunidade: return ServicoNFSe.ExigibilidadeISS.Imunidade;
                case Dominio.Enumeradores.ExigibilidadeISS.SuspensaDecisaoJudicial: return ServicoNFSe.ExigibilidadeISS.SuspensaDecisaoJudicial;
                case Dominio.Enumeradores.ExigibilidadeISS.SuspensaProcessoAdministrativo: return ServicoNFSe.ExigibilidadeISS.SuspensaProcessoAdministrativo;
                default: return ServicoNFSe.ExigibilidadeISS.Exigivel;
            }
        }

        private ServicoNFSe.NFSe ObterNFSeParaEmissao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte == null)
                throw new ArgumentNullException(nameof(cte), "CTe não informado para emissão da NFSe.");

            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork), "UnitOfWork não informado para emissão da NFSe.");

            // Repositórios
            Repositorio.NFSeItem repNFSeItem = new Repositorio.NFSeItem(unitOfWork);
            Repositorio.Embarcador.CTe.CTeParcela repParcelaNFSe = new Repositorio.Embarcador.CTe.CTeParcela(unitOfWork);

            List<Dominio.Entidades.NFSeItem> itens = repNFSeItem.BuscarPorCTe(cte.Codigo);
            List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas = repParcelaNFSe.BuscarPorNFSe(cte.Codigo);

            ServicoNFSe.NFSe nfseImportar = new ServicoNFSe.NFSe();

            nfseImportar.BaseCalculoISS = cte.BaseCalculoISS;

            Dominio.Entidades.Localidade localTermino = cte.LocalidadeTerminoPrestacao;
            Dominio.Entidades.Estado estadoTermino = localTermino?.Estado;

            int codigoCidadeIBGE = localTermino != null ? localTermino.CodigoIBGE : 0;
            int codigoUFIBGE = estadoTermino != null ? estadoTermino.CodigoIBGE : 0;

            nfseImportar.CodigoIBGECidade = string.Format("{0:0000000}", codigoCidadeIBGE);
            nfseImportar.CodigoIBGEUF = string.Format("{0:00}", codigoUFIBGE);

            // Data de emissão
            DateTime dataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Now;
            nfseImportar.DataEmissao = dataEmissao.ToString("dd/MM/yyyy HH:mm:00");

            // Intermediário
            nfseImportar.Intermediario = this.ObterIntermediario(cte.Intermediario);

            // Retenção ISS
            nfseImportar.ISSRetido = cte.ISSRetido ? "R" : "N";

            // Itens
            nfseImportar.Itens = (this.ObterItens(itens, cte) ?? new List<ServicoNFSe.ItemNFSe>()).ToArray();

            // Natureza
            Dominio.Entidades.NaturezaNFSe natureza = cte.NaturezaNFSe;
            nfseImportar.Natureza = natureza != null ? natureza.Numero : 0;

            // Numerações e regime
            nfseImportar.Numero = cte.Numero;
            nfseImportar.NumeroSubstituicao = cte.NumeroSubstituicao;

            Dominio.Entidades.Empresa empresa = cte.Empresa;
            nfseImportar.Regime = (int)empresa.RegimeEspecial;

            // RPS
            nfseImportar.RPS = this.ObterRPS(cte, unitOfWork);

            // Série
            string serieNumero = cte.Serie?.Numero.ToString() ?? default(int).ToString();
            nfseImportar.Serie = serieNumero;
            nfseImportar.SerieSubstituicao = cte.SerieSubstituicao ?? string.Empty;

            // Simples Nacional
            nfseImportar.Simples = empresa.OptanteSimplesNacional ? 1 : 0;

            // Tipo ambiente
            nfseImportar.TipoAmbiente = cte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao
                ? ServicoNFSe.TipoAmbiente.Producao
                : ServicoNFSe.TipoAmbiente.Homologacao;

            // Tomador
            nfseImportar.Tomador = this.ObterTomador(cte.Tomador) ?? new ServicoNFSe.TomadorNFSe();

            // Aliquota e valores
            nfseImportar.AliquotaISS = cte.AliquotaISS;
            nfseImportar.ValorCSLL = cte.ValorCSLL;
            nfseImportar.ValorDeducoes = cte.ValorDeducoes;
            nfseImportar.ValorDescontoCondicionado = cte.ValorDescontoCondicionado;
            nfseImportar.ValorDescontoIncondicionado = cte.ValorDescontoIncondicionado;
            nfseImportar.ValorINSS = cte.ValorINSS;
            nfseImportar.ValorIR = cte.ValorIR;
            nfseImportar.ValorISS = cte.ValorISS;
            nfseImportar.ValorISSRetido = cte.ValorISSRetido;
            nfseImportar.ValorOutrasRetencoes = cte.ValorOutrasRetencoes;
            nfseImportar.ValorCBS = cte.ValorCBS;
            nfseImportar.ValorIBSMunicipal = cte.ValorIBSMunicipal;
            nfseImportar.ValorIBSEstadual = cte.ValorIBSEstadual;
            nfseImportar.ValorBaseCalculoIBSCBS = cte.BaseCalculoIBSCBS;

            nfseImportar.BaseCalculoPIS = cte.BasePIS;
            nfseImportar.BaseCalculoCOFINS = cte.BaseCOFINS;
            nfseImportar.AliquotaPIS = cte.AliquotaPIS;
            nfseImportar.AliquotaCOFINS = cte.AliquotaCOFINS;
            nfseImportar.ValorCOFINS = cte.ValorCOFINS;
            nfseImportar.ValorPIS = cte.ValorPIS;


            int codigoLocalEmpresa = empresa.Localidade.Codigo;
            int codigoLocalTomador = cte.Tomador?.Localidade?.Codigo ?? 0;
            if (codigoLocalEmpresa != codigoLocalTomador)
                nfseImportar.Tomador.IM = string.Empty;

            if (cte.ISSRetido)
                nfseImportar.ValorServicos = cte.BaseCalculoISS;
            else
                nfseImportar.ValorServicos = cte.ValorAReceber;

            string duplicatas = string.Empty;
            if (parcelas != null && parcelas.Count > 0)
            {
                StringBuilder sb = new StringBuilder(" - Duplicatas:");
                foreach (Dominio.Entidades.Embarcador.CTe.CTeParcela parcela in parcelas)
                {
                    int seq = parcela.Sequencia;
                    decimal val = parcela.Valor;
                    string dtVenc = parcela.DataVencimento.HasValue
                        ? parcela.DataVencimento.Value.ToString("dd/MM/yyyy")
                        : string.Empty;

                    sb.AppendFormat(" {0} - {1:n2}-{2};", seq, val, dtVenc);
                }
                duplicatas = sb.ToString();
            }

            string discriminacaoItem = itens != null && itens.Count > 0
                ? (itens.FirstOrDefault()?.Discriminacao ?? string.Empty)
                : string.Empty;

            string observacoesGerais = cte.ObservacoesGerais ?? string.Empty;

            nfseImportar.OutrasInformacoes = string.Concat(observacoesGerais, duplicatas, discriminacaoItem).Trim();

            return nfseImportar;
        }

        private ServicoNFSe.NFSe ObterNFSeParaEmissao(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unitOfWork);
            Repositorio.CodigosServicoNFSe repCodigosServico = new Repositorio.CodigosServicoNFSe(unitOfWork);

            List<Dominio.Entidades.ItemNFSe> itens = repItem.BuscarPorNFSe(nfse.Codigo);
            List<Dominio.Entidades.CodigosServicoNFSe> codigosServicosNFSe = repCodigosServico.BuscarPorConfiguracao(nfse.Empresa.Configuracao.Codigo);
            ServicoNFSe.NFSe nfseImportar = new ServicoNFSe.NFSe();

            List<string> listaOutrasInformacoes = new List<string>();
            if (!string.IsNullOrWhiteSpace(nfse.OutrasInformacoes))
                listaOutrasInformacoes.Add(nfse.OutrasInformacoes);

            string outrasInformacoes = string.Join(" - ", listaOutrasInformacoes);

            nfseImportar.BaseCalculoISS = nfse.BaseCalculoISS;
            nfseImportar.CodigoIBGECidade = nfse.Empresa.Localidade.CodigoIBGE == 5208707 && !string.IsNullOrWhiteSpace(nfse.LocalidadePrestacaoServico.CodigoNFSeGoiania) ? nfse.LocalidadePrestacaoServico.CodigoNFSeGoiania : string.Format("{0:0000000}", nfse.LocalidadePrestacaoServico.CodigoIBGE);
            nfseImportar.CodigoIBGEUF = string.Format("{0:00}", nfse.LocalidadePrestacaoServico.Estado.CodigoIBGE);
            nfseImportar.DataEmissao = nfse.DataEmissao.ToString("dd/MM/yyyy HH:mm:00");
            nfseImportar.Intermediario = this.ObterIntermediario(nfse.Intermediario);
            nfseImportar.ISSRetido = nfse.ISSRetido ? "R" : "N";
            nfseImportar.Itens = this.ObterItens(itens, nfse.Empresa, codigosServicosNFSe).ToArray();
            nfseImportar.Natureza = nfse.Natureza.Numero;
            nfseImportar.Numero = nfse.Numero;
            nfseImportar.NumeroSubstituicao = nfse.NumeroSubstituicao;
            nfseImportar.Regime = (int)nfse.Empresa.RegimeEspecial;
            nfseImportar.RPS = this.ObterRPS(ref nfse, unitOfWork);
            nfseImportar.Serie = nfse.Serie.Numero.ToString();
            nfseImportar.SerieSubstituicao = nfse.SerieSubstituicao;
            nfseImportar.Simples = nfse.Empresa.OptanteSimplesNacional ? 1 : 0;
            nfseImportar.TipoAmbiente = nfse.Ambiente == Dominio.Enumeradores.TipoAmbiente.Producao ? ServicoNFSe.TipoAmbiente.Producao : ServicoNFSe.TipoAmbiente.Homologacao;
            nfseImportar.Tomador = this.ObterTomador(nfse.Tomador, nfse.Empresa);
            nfseImportar.AliquotaISS = nfse.AliquotaISS;
            nfseImportar.ValorCSLL = nfse.ValorCSLL;
            nfseImportar.ValorDeducoes = nfse.ValorDeducoes;
            nfseImportar.ValorDescontoCondicionado = nfse.ValorDescontoCondicionado;
            nfseImportar.ValorDescontoIncondicionado = nfse.ValorDescontoIncondicionado;
            nfseImportar.ValorINSS = nfse.ValorINSS;
            nfseImportar.ValorIR = nfse.ValorIR;
            nfseImportar.ValorISS = nfse.ValorISS;
            nfseImportar.ValorISSRetido = nfse.ValorISSRetido;
            nfseImportar.ValorOutrasRetencoes = nfse.ValorOutrasRetencoes;
            nfseImportar.ValorServicos = nfse.ValorServicos;
            nfseImportar.ValorCBS = nfse.ValorCBS;
            nfseImportar.ValorIBSMunicipal = nfse.ValorIBSMunicipal;
            nfseImportar.ValorIBSEstadual = nfse.ValorIBSEstadual;
            nfseImportar.ValorBaseCalculoIBSCBS = nfse.BaseCalculoIBSCBS;
            nfseImportar.OutrasInformacoes = !string.IsNullOrWhiteSpace(outrasInformacoes) ? outrasInformacoes.Replace("&", " ") : string.Empty;

            nfseImportar.BaseCalculoPIS = nfse.BaseCalculoPIS;
            nfseImportar.BaseCalculoCOFINS = nfse.BaseCalculoCOFINS;
            nfseImportar.AliquotaPIS = nfse.AliquotaPIS;
            nfseImportar.AliquotaCOFINS = nfse.AliquotaCOFINS;

            nfseImportar.ValorPIS = nfse.ValorPIS;
            nfseImportar.ValorCOFINS = nfse.ValorCOFINS;

            if (nfse.Empresa.Localidade.Codigo != nfse.Tomador.Localidade.Codigo)
                nfseImportar.Tomador.IM = "";

            return nfseImportar;
        }

        private ServicoNFSe.IntermediarioNFSe ObterIntermediario(Dominio.Entidades.ParticipanteCTe intermediario)
        {
            if (intermediario != null)
            {
                ServicoNFSe.IntermediarioNFSe intermediarioImportar = new ServicoNFSe.IntermediarioNFSe();

                intermediarioImportar.CPFCNPJ = intermediario.CPF_CNPJ;
                intermediarioImportar.IM = intermediario.InscricaoMunicipal;
                intermediarioImportar.RazaoSocial = intermediario.Nome;

                return intermediarioImportar;
            }
            else
            {
                return null;
            }
        }

        private ServicoNFSe.IntermediarioNFSe ObterIntermediario(Dominio.Entidades.ParticipanteNFSe intermediario)
        {
            if (intermediario != null)
            {
                ServicoNFSe.IntermediarioNFSe intermediarioImportar = new ServicoNFSe.IntermediarioNFSe();

                intermediarioImportar.CPFCNPJ = intermediario.CPF_CNPJ;
                intermediarioImportar.IM = intermediario.InscricaoMunicipal;
                intermediarioImportar.RazaoSocial = intermediario.Nome;

                return intermediarioImportar;
            }
            else
            {
                return null;
            }
        }

        private ServicoNFSe.TomadorNFSe ObterTomador(Dominio.Entidades.ParticipanteCTe tomador)
        {
            ServicoNFSe.TomadorNFSe tomadorImportar = new ServicoNFSe.TomadorNFSe();

            tomadorImportar.Bairro = tomador.Bairro;
            tomadorImportar.CEP = tomador.CEP;
            tomadorImportar.Cidade = Utilidades.String.Left(tomador.Exterior ? tomador.Cidade : tomador.Localidade.Descricao, 60);
            tomadorImportar.CodigoIBGECidade = tomador.Exterior ? "9999999" : string.Format("{0:0000000}", tomador.Localidade.CodigoIBGE);
            tomadorImportar.CodigoIBGEPais = tomador.Exterior ? string.Format("{0:0000}", tomador.Pais != null ? int.Parse(tomador.Pais.Sigla) : tomador.Localidade != null ? int.Parse(tomador.Localidade.Estado.Pais.Sigla) : int.Parse(tomador.Cliente.Localidade.Estado.Pais.Sigla)) : string.Format("{0:0000}", int.Parse(tomador.Localidade.Estado.Pais.Sigla));
            tomadorImportar.CodigoIBGEUF = tomador.Exterior ? "99" : string.Format("{0:00}", tomador.Localidade.Estado.CodigoIBGE);
            tomadorImportar.Complemento = tomador.Complemento;
            tomadorImportar.CPFCNPJ = tomador.CPF_CNPJ;
            tomadorImportar.DocumentoEstrangeiro = "";
            tomadorImportar.Emails = tomador.EmailStatus ? tomador.Email : string.Empty;
            tomadorImportar.IE = tomador.IE_RG;
            tomadorImportar.IM = tomador.InscricaoMunicipal;
            tomadorImportar.Logradouro = tomador.Endereco;
            tomadorImportar.NomeFantasia = tomador.NomeFantasia;
            tomadorImportar.Numero = tomador.Numero;
            tomadorImportar.Pais = Utilidades.String.Left(tomador.Exterior ? (tomador.Pais != null ? tomador.Pais.Nome : tomador.Localidade != null ? tomador.Localidade.Estado.Pais.Nome : tomador.Cliente.Localidade.Estado.Pais.Nome) : tomador.Localidade.Estado.Pais.Nome, 60);
            tomadorImportar.RazaoSocial = tomador.Nome;
            tomadorImportar.Telefone = Utilidades.String.OnlyNumbers(tomador.Telefone1);
            tomadorImportar.UF = tomador.Localidade?.Estado.Sigla ?? "EX";

            return tomadorImportar;
        }

        private ServicoNFSe.TomadorNFSe ObterTomador(Dominio.Entidades.ParticipanteNFSe tomador, Dominio.Entidades.Empresa empresa)
        {
            if (tomador == null)
                return null;

            ServicoNFSe.TomadorNFSe tomadorImportar = new ServicoNFSe.TomadorNFSe();

            tomadorImportar.Bairro = tomador.Bairro;
            tomadorImportar.CEP = tomador.CEP;
            tomadorImportar.Cidade = Utilidades.String.Left(tomador.Exterior ? tomador.Cidade : tomador.Localidade.Descricao, 60);
            tomadorImportar.CodigoIBGECidade = tomador.Exterior ? "9999999" : empresa.Localidade.CodigoIBGE == 5208707 && !string.IsNullOrWhiteSpace(tomador.Localidade.CodigoNFSeGoiania) ? tomador.Localidade.CodigoNFSeGoiania : string.Format("{0:0000000}", tomador.Localidade.CodigoIBGE);
            tomadorImportar.CodigoIBGEPais = tomador.Exterior ? string.Format("{0:0000}", int.Parse(tomador.Pais.Sigla)) : string.Format("{0:0000}", int.Parse(tomador.Localidade.Estado.Pais.Sigla));
            tomadorImportar.CodigoIBGEUF = tomador.Exterior ? "99" : string.Format("{0:00}", tomador.Localidade.Estado.CodigoIBGE);
            tomadorImportar.Complemento = tomador.Complemento;
            tomadorImportar.CPFCNPJ = tomador.CPF_CNPJ;
            tomadorImportar.DocumentoEstrangeiro = tomador.NumeroDocumentoExterior;
            tomadorImportar.Emails = tomador.EmailStatus ? tomador.Email : string.Empty;
            tomadorImportar.IE = tomador.IE_RG;
            tomadorImportar.IM = tomador.InscricaoMunicipal;
            tomadorImportar.Logradouro = tomador.Endereco;
            tomadorImportar.NomeFantasia = tomador.NomeFantasia;
            tomadorImportar.Numero = tomador.Numero;
            tomadorImportar.Pais = Utilidades.String.Left(tomador.Exterior ? (tomador.Pais?.Nome ?? tomador.Localidade.Estado.Pais.Nome) : tomador.Localidade.Estado.Pais.Nome, 60);
            tomadorImportar.RazaoSocial = tomador.Nome;
            tomadorImportar.Telefone = Utilidades.String.OnlyNumbers(tomador.Telefone1);
            tomadorImportar.UF = tomador.Localidade?.Estado.Sigla ?? "EX";

            return tomadorImportar;
        }

        private List<Servicos.ServicoNFSe.ItemNFSe> ObterItens(List<Dominio.Entidades.NFSeItem> itens, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            List<ServicoNFSe.ItemNFSe> itensImportar = new List<ServicoNFSe.ItemNFSe>();

            foreach (Dominio.Entidades.NFSeItem item in itens)
            {
                ServicoNFSe.ItemNFSe itemImportar = new ServicoNFSe.ItemNFSe()
                {
                    AliquotaISS = item.AliquotaISS,
                    BaseCalculoISS = item.BaseCalculoISS,
                    CodigoIBGECidade = string.Format("{0:0000000}", cte.LocalidadeTerminoPrestacao.CodigoIBGE),
                    CodigoIBGECidadeIncidencia = string.Format("{0:0000000}", cte.LocalidadeTerminoPrestacao.CodigoIBGE),
                    CodigoIBGEPais = string.Format("{0:0000}", int.Parse(item.PaisPrestacaoServico.Sigla)),
                    CodigoServico = item.Servico.Numero.ToString(),
                    CodigoTributacaoMunicipio = item.Servico.CodigoTributacao,
                    DescontoCondicionado = item.ValorDescontoCondicionado,
                    DescontoIncondicionado = item.ValorDescontoIncondicionado,
                    DescricaoServico = item.Servico.Descricao.Length > 80 ? item.Servico.Descricao.Substring(0, 80) : item.Servico.Descricao,
                    Discriminacao = item.Discriminacao,
                    ExigibilidadeISS = ObterExibilidadeISS(item.ExigibilidadeISS),
                    PrestadoPais = item.ServicoPrestadoNoPais,
                    Quantidade = item.Quantidade,
                    ValorDeducoes = item.ValorDeducoes,
                    ValorServico = item.ValorServico,
                    ValorTotal = item.ValorTotal,
                    ValorUnitario = item.ValorServico,
                    cnae = item.Servico.CNAE,
                    NBS = item.NBS,
                    CodigoIndicadorOperacao = item.CodigoIndicadorOperacao,
                    CSTIBSCBS = item.CSTIBSCBS,
                    CodigoClassificacaoTributariaIBSCBS = item.ClassificacaoTributariaIBSCBS,
                    BaseCalculoIBSCBS = item.BaseCalculoIBSCBS,
                    AliquotaIBSEstadual = item.AliquotaIBSEstadual,
                    PercentualReducaoIBSEstadual = item.PercentualReducaoIBSEstadual,
                    ValorIBSEstadual = item.ValorIBSEstadual,
                    AliquotaIBSMunicipal = item.AliquotaIBSMunicipal,
                    PercentualReducaoIBSMunicipal = item.PercentualReducaoIBSMunicipal,
                    ValorIBSMunicipal = item.ValorIBSMunicipal,
                    AliquotaCBS = item.AliquotaCBS,
                    PercentualReducaoCBS = item.PercentualReducaoCBS,
                    ValorCBS = item.ValorCBS,

                    CSTPIS = item.CSTPIS,
                    CSTCOFINS = item.CSTCOFINS,
                    BaseCalculoPIS = item.BaseCalculoPIS,
                    BaseCalculoCOFINS = item.BaseCalculoCOFINS,
                    AliquotaPIS = item.AliquotaPIS,
                    AliquotaCOFINS = item.AliquotaCOFINS,
                    ValorPIS = item.ValorPIS,
                    ValorCOFINS = item.ValorCOFINS,
                };

                itensImportar.Add(itemImportar);
            }

            return itensImportar;
        }

        private List<ServicoNFSe.ItemNFSe> ObterItens(List<Dominio.Entidades.ItemNFSe> itens, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.CodigosServicoNFSe> codigosServicosNFSe)
        {
            List<ServicoNFSe.ItemNFSe> itensImportar = new List<ServicoNFSe.ItemNFSe>();

            int codigoIBGEMunicipioGoiania = 5208707;
            bool municipioGoiania = (empresa.Localidade.CodigoIBGE == codigoIBGEMunicipioGoiania);
            string codigoTributacao, numeroTributacao, CNAE;
            foreach (Dominio.Entidades.ItemNFSe item in itens)
            {
                Dominio.Entidades.CodigosServicoNFSe codigosServicoNFSe = codigosServicosNFSe.Where(x => x.CodigoTributacao == item.Servico.CodigoTributacao).FirstOrDefault();
                codigoTributacao = codigosServicoNFSe?.CodigoTributacaoPrefeitura ?? item.Servico.CodigoTributacao;
                numeroTributacao = codigosServicoNFSe?.NumeroTributacaoPrefeitura ?? item.Servico.Numero.ToString();
                CNAE = codigosServicoNFSe?.CNAE ?? (!string.IsNullOrWhiteSpace(item.Servico.CNAE) ? item.Servico.CNAE : empresa?.CNAE ?? string.Empty);

                ServicoNFSe.ItemNFSe itemImportar = new ServicoNFSe.ItemNFSe()
                {
                    AliquotaISS = item.AliquotaISS,
                    BaseCalculoISS = item.BaseCalculoISS,
                    CodigoIBGECidade = municipioGoiania && !string.IsNullOrWhiteSpace(item.Municipio.CodigoNFSeGoiania) ? item.Municipio.CodigoNFSeGoiania : string.Format("{0:0000000}", item.Municipio.CodigoIBGE),
                    CodigoIBGECidadeIncidencia = municipioGoiania && !string.IsNullOrWhiteSpace(item.MunicipioIncidencia.CodigoNFSeGoiania) ? item.MunicipioIncidencia.CodigoNFSeGoiania : string.Format("{0:0000000}", item.MunicipioIncidencia.CodigoIBGE),
                    CodigoIBGEPais = string.Format("{0:0000}", int.Parse(item.PaisPrestacaoServico.Sigla)),
                    CodigoServico = numeroTributacao,
                    CodigoTributacaoMunicipio = codigoTributacao,
                    DescontoCondicionado = item.ValorDescontoCondicionado,
                    DescontoIncondicionado = item.ValorDescontoIncondicionado,
                    DescricaoServico = item.Servico.Descricao.Length > 80 ? item.Servico.Descricao.Substring(0, 80) : item.Servico.Descricao,
                    Discriminacao = !string.IsNullOrWhiteSpace(item.Discriminacao) ? item.Discriminacao.Replace("&", " ") : string.Empty,
                    ExigibilidadeISS = ObterExibilidadeISS(item.ExigibilidadeISS),
                    PrestadoPais = item.ServicoPrestadoNoPais,
                    Quantidade = item.Quantidade,
                    ValorDeducoes = item.ValorDeducoes,
                    ValorServico = item.ValorServico,
                    ValorTotal = item.ValorTotal,
                    ValorUnitario = item.ValorServico,
                    cnae = CNAE,
                    NBS = item.NBS,
                    CodigoIndicadorOperacao = item.CodigoIndicadorOperacao,
                    CSTIBSCBS = item.CSTIBSCBS,
                    CodigoClassificacaoTributariaIBSCBS = item.ClassificacaoTributariaIBSCBS,
                    BaseCalculoIBSCBS = item.BaseCalculoIBSCBS,
                    AliquotaIBSEstadual = item.AliquotaIBSEstadual,
                    PercentualReducaoIBSEstadual = item.PercentualReducaoIBSEstadual,
                    ValorIBSEstadual = item.ValorIBSEstadual,
                    AliquotaIBSMunicipal = item.AliquotaIBSMunicipal,
                    PercentualReducaoIBSMunicipal = item.PercentualReducaoIBSMunicipal,
                    ValorIBSMunicipal = item.ValorIBSMunicipal,
                    AliquotaCBS = item.AliquotaCBS,
                    PercentualReducaoCBS = item.PercentualReducaoCBS,
                    ValorCBS = item.ValorCBS,

                    CSTPIS = item.CSTPIS,
                    CSTCOFINS = item.CSTCOFINS,
                    BaseCalculoPIS = item.BaseCalculoPIS,
                    BaseCalculoCOFINS = item.BaseCalculoCOFINS,
                    AliquotaPIS = item.AliquotaPIS,
                    AliquotaCOFINS = item.AliquotaCOFINS,
                    ValorPIS = item.ValorPIS,
                    ValorCOFINS = item.ValorCOFINS
                };

                itensImportar.Add(itemImportar);
            }

            return itensImportar;
        }

        public ServicoNFSe.RPSNFSe ObterRPS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.NFSePreNFSItupeva repNFSePreNFSItupeva = new Repositorio.NFSePreNFSItupeva(unitOfWork);

            Dominio.Entidades.NFSePreNFSItupeva preNFSe = null;

            if (cte.RPS == null)
            {
                Dominio.Entidades.RPSNFSe rpsNFSe = new Dominio.Entidades.RPSNFSe();

                rpsNFSe.Empresa = cte.Empresa;
                rpsNFSe.Numero = repRPS.ObterUltimoNumero(cte.Empresa.Codigo) + 1;
                rpsNFSe.Serie = !string.IsNullOrWhiteSpace(cte.SerieRPS) ? cte.SerieRPS : (cte.Empresa.Configuracao != null ? cte.Empresa.Configuracao.SerieRPSNFSe : string.Empty);
                rpsNFSe.Status = "P";
                repRPS.Inserir(rpsNFSe);
                cte.RPS = rpsNFSe;
                repCTe.Atualizar(cte);

                if (preNFSe == null)
                {
                    preNFSe = repNFSePreNFSItupeva.BuscarPendente(cte.Empresa.Codigo);
                    if (preNFSe != null)
                    {
                        preNFSe.RPSNFSe = rpsNFSe;
                        repNFSePreNFSItupeva.Atualizar(preNFSe);
                    }
                }
            }
            else
            {
                //nfse.RPS.Numero = repRPS.ObterUltimoNumero(nfse.Empresa.Codigo) + 1; //Betha exige que o RPS possua uma sequência, tirado o incremento.
                cte.RPS.Serie = !string.IsNullOrWhiteSpace(cte.SerieRPS) ? cte.SerieRPS : (cte.Empresa.Configuracao != null ? cte.Empresa.Configuracao.SerieRPSNFSe : string.Empty);
                cte.RPS.Status = "P";

                repCTe.Atualizar(cte);

                preNFSe = repNFSePreNFSItupeva.BuscarPreNFSePorRPS(cte.RPS.Codigo);
                if (preNFSe == null)
                {
                    preNFSe = repNFSePreNFSItupeva.BuscarPendente(cte.Empresa.Codigo);

                    if (preNFSe != null)
                    {
                        preNFSe.RPSNFSe = cte.RPS;
                        repNFSePreNFSItupeva.Atualizar(preNFSe);
                    }
                }
            }

            ServicoNFSe.RPSNFSe rps = new ServicoNFSe.RPSNFSe();

            rps.Emitente = this.ObterEmpresaEmitente(cte.Empresa);
            rps.Numero = cte.RPS.Numero;
            rps.Serie = cte.RPS.Serie;

            if (preNFSe != null)
            {
                rps.PreNFSeCodigoValidacao = preNFSe.NumeroValidacao;
                rps.PreNFSeNumeroBloco = preNFSe.NumeroBloco;
                rps.PreNFSeNumeroSequencial = preNFSe.NumeroSequencia;
            }

            return rps;
        }

        public ServicoNFSe.RPSNFSe ObterRPS(ref Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            Repositorio.NFSePreNFSItupeva repNFSePreNFSItupeva = new Repositorio.NFSePreNFSItupeva(unitOfWork);
            Dominio.Entidades.NFSePreNFSItupeva preNFSe = null;

            if (nfse.RPS == null)
            {
                Dominio.Entidades.RPSNFSe rpsNFSe = new Dominio.Entidades.RPSNFSe();

                rpsNFSe.Empresa = nfse.Empresa;
                rpsNFSe.Numero = nfse.NumeroRPS.HasValue && nfse.NumeroRPS.Value > 0 ? nfse.NumeroRPS.Value : repRPS.ObterUltimoNumero(nfse.Empresa.Codigo) + 1;
                rpsNFSe.Serie = nfse.Empresa.Configuracao != null ? nfse.Empresa.Configuracao.SerieRPSNFSe : string.Empty;
                rpsNFSe.Status = "P";

                repRPS.Inserir(rpsNFSe);

                nfse.RPS = rpsNFSe;

                repNFSe.Atualizar(nfse);

                if (preNFSe == null)
                {
                    preNFSe = repNFSePreNFSItupeva.BuscarPendente(nfse.Empresa.Codigo);
                    if (preNFSe != null)
                    {
                        preNFSe.RPSNFSe = rpsNFSe;
                        repNFSePreNFSItupeva.Atualizar(preNFSe);
                    }
                }
            }
            else
            {
                Dominio.Entidades.NFSe nfseAnterior = repNFSe.BuscarPorRPSSituacaoAmbiente(nfse.Empresa.Codigo, nfse.RPS.Numero, Dominio.Enumeradores.StatusNFSe.Autorizado, nfse.Ambiente);
                if (nfseAnterior != null)
                    nfse.RPS.Numero = repRPS.ObterUltimoNumero(nfse.Empresa.Codigo) + 1;

                nfse.RPS.Serie = nfse.Empresa.Configuracao != null ? nfse.Empresa.Configuracao.SerieRPSNFSe : string.Empty;
                nfse.RPS.Status = "P";

                repNFSe.Atualizar(nfse);

                preNFSe = repNFSePreNFSItupeva.BuscarPreNFSePorRPS(nfse.RPS.Codigo);
                if (preNFSe == null)
                {
                    preNFSe = repNFSePreNFSItupeva.BuscarPendente(nfse.Empresa.Codigo);

                    if (preNFSe != null)
                    {
                        preNFSe.RPSNFSe = nfse.RPS;
                        repNFSePreNFSItupeva.Atualizar(preNFSe);
                    }
                }
            }

            ServicoNFSe.RPSNFSe rps = new ServicoNFSe.RPSNFSe();

            rps.Emitente = this.ObterEmpresaEmitente(nfse.Empresa);
            rps.Numero = nfse.RPS.Numero;
            rps.Serie = nfse.RPS.Serie;

            if (preNFSe != null)
            {
                rps.PreNFSeCodigoValidacao = preNFSe.NumeroValidacao;
                rps.PreNFSeNumeroBloco = preNFSe.NumeroBloco;
                rps.PreNFSeNumeroSequencial = preNFSe.NumeroSequencia;
            }

            return rps;
        }

        private void SalvarRPSNFSeProcessada(Dominio.Entidades.NFSe nfse, int numeroRPS, string serieRPS, string dataEmissao, string horaEmissao, string protocolo, string dataProtocolo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            Dominio.Entidades.RPSNFSe rpsNFSe = new Dominio.Entidades.RPSNFSe();

            rpsNFSe.Empresa = nfse.Empresa;
            rpsNFSe.Numero = nfse.NumeroRPS.HasValue && nfse.NumeroRPS.Value > 0 ? nfse.NumeroRPS.Value : (numeroRPS > 0 ? numeroRPS : nfse.Numero);
            rpsNFSe.Serie = !string.IsNullOrWhiteSpace(serieRPS) ? serieRPS : nfse.Serie.Numero.ToString();
            rpsNFSe.Status = nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "A" : nfse.Status == Dominio.Enumeradores.StatusNFSe.AguardandoAutorizacaoRPS ? "P" : "C";
            DateTime data;
            if (!DateTime.TryParseExact(dataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                data = DateTime.Now;
            rpsNFSe.Data = data;
            rpsNFSe.Protocolo = protocolo;

            DateTime dataProt;
            if (!DateTime.TryParseExact(dataProtocolo, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataProt))
                dataProt = DateTime.Now;
            rpsNFSe.DataProtocolo = DateTime.Now;
            rpsNFSe.Hora = horaEmissao;

            if (rpsNFSe.Status == "C")
                rpsNFSe.MensagemRetorno = "NFS-e cancelada pela importacao.";
            else
            if (rpsNFSe.Status == "P")
                rpsNFSe.MensagemRetorno = "RPS importado, aguardando NFSe.";
            else
                rpsNFSe.MensagemRetorno = "NFS-e importada ja processada.";

            repRPS.Inserir(rpsNFSe);

            nfse.RPS = rpsNFSe;

            repNFSe.Atualizar(nfse);
        }

        private ServicoNFSe.Empresa ObterEmpresaEmitente(Dominio.Entidades.Empresa empresa)
        {
            ServicoNFSe.Empresa empresaEmitente = new ServicoNFSe.Empresa();

            empresaEmitente.Bairro = Utilidades.String.Left(empresa.Bairro, 60);
            empresaEmitente.Cep = Utilidades.String.OnlyNumbers(empresa.CEP);
            empresaEmitente.Cidade = Utilidades.String.Left(empresa.Localidade.Descricao, 60);
            empresaEmitente.CNPJ = Utilidades.String.OnlyNumbers(empresa.CNPJ);
            empresaEmitente.CodigoCidadeIBGE = empresa.Localidade.CodigoIBGE == 5208707 && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(empresa.Localidade.CodigoNFSeGoiania)) ? int.Parse(Utilidades.String.OnlyNumbers(empresa.Localidade.CodigoNFSeGoiania)) : empresa.Localidade.CodigoIBGE;
            empresaEmitente.Complemento = Utilidades.String.Left(empresa.Complemento, 60);
            empresaEmitente.EmailContador = empresa.EmailContador;
            empresaEmitente.EmailEmitente = empresa.Email;
            empresaEmitente.EnviaEmailContador = empresa.StatusEmail;
            empresaEmitente.EnviaEmailEmitente = empresa.StatusEmailContador;
            empresaEmitente.IE = string.IsNullOrWhiteSpace(empresa.InscricaoEstadual) ? "ISENTO" : empresa.InscricaoEstadual;
            empresaEmitente.IM = empresa.InscricaoMunicipal;
            empresaEmitente.SenhaNFSe = empresa.Configuracao != null ? empresa.Configuracao.SenhaNFSe : string.Empty;
            empresaEmitente.FraseSecretaNFSe = empresa.Configuracao != null ? empresa.Configuracao.FraseSecretaNFSe : string.Empty;
            empresaEmitente.Logradouro = Utilidades.String.Left(empresa.Endereco, 255);
            empresaEmitente.NomeContador = Utilidades.String.Left(empresa.NomeContador, 60);
            empresaEmitente.NomeFantasia = Utilidades.String.Left(empresa.NomeFantasia, 60);
            empresaEmitente.NomeRazao = Utilidades.String.Left(empresa.RazaoSocial, 60);
            empresaEmitente.Numero = Utilidades.String.Left(empresa.Numero, 60);
            empresaEmitente.Status = empresa.Status;
            empresaEmitente.Telefone = Utilidades.String.OnlyNumbers(empresa.Telefone);
            empresaEmitente.TelefoneContador = Utilidades.String.OnlyNumbers(empresa.TelefoneContador);
            empresaEmitente.UF = empresa.Localidade.Estado.Sigla;

            return empresaEmitente;
        }

        public System.IO.MemoryStream ObterLoteDeDANFSE(List<string> codigos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                throw new Exception("O caminho para os download da DANFSE não está disponível.");

            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            MemoryStream fZip = new MemoryStream();

            using (ZipOutputStream zipOStream = new ZipOutputStream(fZip))
            {
                zipOStream.SetLevel(9);

                foreach (string codigo in codigos)
                {
                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(int.Parse(codigo));
                    if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                    {
                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, "NFSe", nfse.Empresa.CNPJ, nfse.Codigo.ToString() + "_" + nfse.Numero.ToString() + "_" + nfse.Serie.Numero.ToString()) + ".pdf";

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                            this.ObterDANFSE(nfse.Codigo);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        {
                            byte[] dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

                            ZipEntry entry = new ZipEntry(string.Concat("NFSe", nfse.Empresa.CNPJ, nfse.Codigo.ToString(), "_", nfse.Numero.ToString(), "_", nfse.Serie.Numero.ToString(), ".pdf"));

                            entry.DateTime = DateTime.Now;

                            zipOStream.PutNextEntry(entry);
                            zipOStream.Write(dacte, 0, dacte.Length);
                            zipOStream.CloseEntry();
                        }
                    }
                }
                zipOStream.IsStreamOwner = false;
                zipOStream.Close();
            }

            fZip.Position = 0;

            return fZip;
        }

        public System.IO.MemoryStream ObterLoteDeDANFSE(List<int> codigos, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios))
                throw new Exception("O caminho para os download da DANFSE não está disponível.");

            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

            MemoryStream fZip = new MemoryStream();

            using (ZipOutputStream zipOStream = new ZipOutputStream(fZip))
            {
                zipOStream.SetLevel(9);

                foreach (int codigo in codigos)
                {
                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigo);
                    if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                    {
                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, "NFSe", nfse.Empresa.CNPJ, nfse.Codigo.ToString() + "_" + nfse.Numero.ToString() + "_" + nfse.Serie.Numero.ToString()) + ".pdf";

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                            this.ObterDANFSE(nfse.Codigo);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        {
                            byte[] dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

                            ZipEntry entry = new ZipEntry(string.Concat("NFSe", nfse.Empresa.CNPJ, nfse.Codigo.ToString(), "_", nfse.Numero.ToString(), "_", nfse.Serie.Numero.ToString(), ".pdf"));

                            entry.DateTime = DateTime.Now;

                            zipOStream.PutNextEntry(entry);
                            zipOStream.Write(dacte, 0, dacte.Length);
                            zipOStream.CloseEntry();
                        }
                    }
                }
                zipOStream.IsStreamOwner = false;
                zipOStream.Close();
            }

            fZip.Position = 0;

            return fZip;
        }

        public System.IO.MemoryStream ObterLoteDeXML(List<int> codigos, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unitOfWork);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            List<Dominio.Entidades.XMLNFSe> xmls = repXMLNFSe.BuscarPorCodigosNFSe(codigos, codigoEmpresa);

            foreach (Dominio.Entidades.XMLNFSe xml in xmls)
            {
                byte[] arquivo = System.Text.Encoding.Default.GetBytes(xml.XML);
                ZipEntry entry = new ZipEntry(string.Concat("NFSe", xml.NFSe.Empresa.CNPJ, "_", xml.NFSe.Numero.ToString(), "_", xml.NFSe.Serie.Numero.ToString(), (xml.Tipo == Dominio.Enumeradores.TipoXMLNFSe.Cancelamento ? "-CancNFSe" : ""), ".xml"));

                entry.DateTime = DateTime.Now;
                zipOStream.PutNextEntry(entry);
                zipOStream.Write(arquivo, 0, arquivo.Length);
                zipOStream.CloseEntry();
            }

            xmls = null;

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        #endregion

        #region Geração
        public bool CalcularFretePorTabelaDeFrete(ref Dominio.Entidades.NFSe nfse, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool calcularImpostos = false, string codigoIntegracao = "")
        {
            if (string.IsNullOrWhiteSpace(nfse.Tomador.CPF_CNPJ))
                return false;

            Repositorio.FreteFracionadoUnidade repFreteFracionadoUnidade = new Repositorio.FreteFracionadoUnidade(unidadeDeTrabalho);
            Repositorio.FreteFracionadoValor repFreteFracionadoValor = new Repositorio.FreteFracionadoValor(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.Frete repFretePorPeso = new Repositorio.Frete(unidadeDeTrabalho);
            Repositorio.FretePorValor repFretePorValor = new Repositorio.FretePorValor(unidadeDeTrabalho);
            Repositorio.FretePorTipoDeVeiculo repFretePorTipoVeiculo = new Repositorio.FretePorTipoDeVeiculo(unidadeDeTrabalho);
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

            double cpfCnpjTomador = double.Parse(nfse.Tomador.CPF_CNPJ);
            int ibgeLocalidadePrestacao = nfse.LocalidadePrestacaoServico.CodigoIBGE;
            int codigoLocalidadePrestacao = nfse.LocalidadePrestacaoServico.Codigo;

            decimal valorFrete = 0;
            decimal valorAdValorem = 0;
            decimal valorGris = 0;
            decimal valorExedentePeso = 0;
            decimal valorExedenteValor = 0;
            decimal pesoMaximoTabela = 0;
            decimal valorMaximoTabela = 0;
            decimal pesoTotalNFSe = 0;
            decimal valorTotalNFe = 0;
            decimal valorPedagio = 0;
            decimal valorDescarga = 0;
            decimal valorSeguro = 0;
            decimal valorOutros = 0;
            decimal valorMinimo = 0;
            decimal valorTAS = 0;

            List<Dominio.Entidades.FreteFracionadoUnidade> listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(codigoEmpresa, "A", cpfCnpjTomador, Dominio.Enumeradores.TipoTomador.Remetente, ibgeLocalidadePrestacao);

            if (listaFreteFracionadoUnidade.Count > 0)
            {
                List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                pesoMaximoTabela = listaFreteFracionadoUnidade.Max(x => x.PesoAte);
                valorTotalNFe = documentos.Sum(o => o.Valor);
                pesoTotalNFSe = documentos.Sum(x => x.Peso);

                if (pesoTotalNFSe > pesoMaximoTabela)
                {
                    Dominio.Entidades.FreteFracionadoUnidade frete = listaFreteFracionadoUnidade.LastOrDefault();

                    valorFrete = frete.ValorFrete;
                    valorAdValorem = frete.PercentualAdValorem > 0 ? valorTotalNFe * (frete.PercentualAdValorem / 100) : 0;
                    valorGris = frete.PercentualGris > 0 ? valorTotalNFe * (frete.PercentualGris / 100) : 0;
                    valorPedagio = frete.ValorPedagio;
                    valorTAS = frete.ValorTAS;
                    decimal pesoExcedente = pesoTotalNFSe - pesoMaximoTabela;
                    valorExedentePeso = pesoExcedente * frete.ValorExcedente;

                    if (frete.ValorMinimoGris > 0 && valorGris < frete.ValorMinimoGris)
                        valorGris = frete.ValorMinimoGris;
                    if (frete.ValorMinimoAdValorem > 0 && valorAdValorem < frete.ValorMinimoAdValorem)
                        valorAdValorem = frete.ValorMinimoAdValorem;

                    if (frete.ValorPorUnidadeMedida > 0 && pesoTotalNFSe > 0)
                        valorFrete += (frete.ValorPorUnidadeMedida * pesoTotalNFSe);
                }
                else
                {
                    for (int i = 0; i < listaFreteFracionadoUnidade.Count; i++)
                    {
                        if (listaFreteFracionadoUnidade[i].PesoDe <= pesoTotalNFSe && listaFreteFracionadoUnidade[i].PesoAte >= pesoTotalNFSe)
                        {
                            valorFrete = listaFreteFracionadoUnidade[i].ValorFrete;
                            valorAdValorem = listaFreteFracionadoUnidade[i].PercentualAdValorem > 0 ? valorTotalNFe * (listaFreteFracionadoUnidade[i].PercentualAdValorem / 100) : 0;
                            valorGris = listaFreteFracionadoUnidade[i].PercentualGris > 0 ? valorTotalNFe * (listaFreteFracionadoUnidade[i].PercentualGris / 100) : 0;
                            valorPedagio = listaFreteFracionadoUnidade[i].ValorPedagio;
                            valorTAS = listaFreteFracionadoUnidade[i].ValorTAS;
                            valorExedentePeso = 0;

                            if (listaFreteFracionadoUnidade[i].ValorMinimoGris > 0 && valorGris < listaFreteFracionadoUnidade[i].ValorMinimoGris)
                                valorGris = listaFreteFracionadoUnidade[i].ValorMinimoGris;
                            if (listaFreteFracionadoUnidade[i].ValorMinimoAdValorem > 0 && valorAdValorem < listaFreteFracionadoUnidade[i].ValorMinimoAdValorem)
                                valorAdValorem = listaFreteFracionadoUnidade[i].ValorMinimoAdValorem;

                            if (listaFreteFracionadoUnidade[i].ValorPorUnidadeMedida > 0 && pesoTotalNFSe > 0)
                                valorFrete += (listaFreteFracionadoUnidade[i].ValorPorUnidadeMedida * pesoTotalNFSe);

                            break;
                        }
                    }
                }
            }
            else
            {
                List<Dominio.Entidades.FreteFracionadoValor> listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(codigoEmpresa, "A", cpfCnpjTomador, Dominio.Enumeradores.TipoTomador.Remetente, ibgeLocalidadePrestacao);

                if (listaFreteFracionadoValor.Count > 0)
                {
                    List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                    valorMaximoTabela = listaFreteFracionadoValor.Max(x => x.ValorAte);
                    valorTotalNFe = documentos.Sum(o => o.Valor);

                    if (valorTotalNFe > valorMaximoTabela)
                    {
                        Dominio.Entidades.FreteFracionadoValor frete = listaFreteFracionadoValor.LastOrDefault();

                        valorFrete = frete.TipoValor == "P" ? valorTotalNFe * (frete.ValorFrete / 100) : frete.ValorFrete;
                        valorAdValorem = frete.PercentualAdValorem > 0 ? valorTotalNFe * (frete.PercentualAdValorem / 100) : 0;
                        valorGris = frete.PercentualGris > 0 ? valorTotalNFe * (frete.PercentualGris / 100) : 0;
                        valorPedagio = frete.ValorPedagio;
                        valorTAS = frete.ValorTAS;
                        decimal valorExcedente = valorTotalNFe - valorMaximoTabela;
                        valorExedenteValor = valorExcedente * frete.ValorExcedente;

                        if (frete.ValorMinimoGris > 0 && valorGris < frete.ValorMinimoGris)
                            valorGris = frete.ValorMinimoGris;
                        if (frete.ValorMinimoAdValorem > 0 && valorAdValorem < frete.ValorMinimoAdValorem)
                            valorAdValorem = frete.ValorMinimoAdValorem;
                    }
                    else
                    {
                        for (int i = 0; i < listaFreteFracionadoValor.Count; i++)
                        {
                            if (listaFreteFracionadoValor[i].ValorDe <= valorTotalNFe && listaFreteFracionadoValor[i].ValorAte >= valorTotalNFe)
                            {
                                valorFrete = listaFreteFracionadoValor[i].TipoValor == "P" ? valorTotalNFe * (listaFreteFracionadoValor[i].ValorFrete / 100) : listaFreteFracionadoValor[i].ValorFrete;
                                valorAdValorem = listaFreteFracionadoValor[i].PercentualAdValorem > 0 ? valorTotalNFe * (listaFreteFracionadoValor[i].PercentualAdValorem / 100) : 0;
                                valorGris = listaFreteFracionadoValor[i].PercentualGris > 0 ? valorTotalNFe * (listaFreteFracionadoValor[i].PercentualGris / 100) : 0;
                                valorPedagio = listaFreteFracionadoValor[i].ValorPedagio;
                                valorTAS = listaFreteFracionadoValor[i].ValorTAS;
                                valorExedenteValor = 0;

                                if (listaFreteFracionadoValor[i].ValorMinimoGris > 0 && valorGris < listaFreteFracionadoValor[i].ValorMinimoGris)
                                    valorGris = listaFreteFracionadoValor[i].ValorMinimoGris;
                                if (listaFreteFracionadoValor[i].ValorMinimoAdValorem > 0 && valorAdValorem < listaFreteFracionadoValor[i].ValorMinimoAdValorem)
                                    valorAdValorem = listaFreteFracionadoValor[i].ValorMinimoAdValorem;

                                break;
                            }
                        }
                    }
                }
                else
                {
                    Dominio.Entidades.Frete fretePorPeso = repFretePorPeso.BuscarPorOrigemEDestino(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true);

                    if (fretePorPeso != null)
                    {
                        List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                        decimal quantidade = documentos.Sum(o => o.Peso);
                        decimal valorNotas = documentos.Sum(o => o.Valor);

                        valorFrete = (fretePorPeso.ValorFrete * (quantidade > fretePorPeso.QuantidadeExcedente && fretePorPeso.QuantidadeExcedente > 0 && fretePorPeso.ValorExcedente > 0 ? fretePorPeso.QuantidadeExcedente : quantidade));// + fretePorPeso.ValorSeguro + fretePorPeso.OutrosValores;

                        valorPedagio = fretePorPeso.ValorPedagio;
                        if (fretePorPeso.ValorPedagioPerc > 0)
                            valorPedagio += (fretePorPeso.ValorPedagioPerc * fretePorPeso.ValorPedagioPerc);

                        valorSeguro = fretePorPeso.ValorSeguro;
                        valorOutros = fretePorPeso.OutrosValores;
                        valorDescarga = fretePorPeso.ValorDescarga;

                        if (fretePorPeso.PercentualGris > 0 && valorNotas > 0)
                            valorGris = (valorNotas * (fretePorPeso.PercentualGris / 100));
                        if (fretePorPeso.PercentualAdValorem > 0 && valorNotas > 0)
                            valorAdValorem = (valorNotas * (fretePorPeso.PercentualAdValorem / 100));

                        if (fretePorPeso.ValorExcedente > 0 && fretePorPeso.QuantidadeExcedente < quantidade)
                            valorFrete += ((quantidade - fretePorPeso.QuantidadeExcedente) * fretePorPeso.ValorExcedente);

                        valorMinimo = fretePorPeso.ValorMinimoFrete;
                    }
                    else
                    {
                        //Busca tabela valor com cliente e com localidade destino
                        Dominio.Entidades.FretePorValor fretePorValor = repFretePorValor.BuscarParaCalculo(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true, null, codigoIntegracao);

                        if (fretePorValor != null)
                        {
                            List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                            decimal valorTotal = documentos.Sum(o => o.Valor);
                            decimal valorSobrePercentual = (valorTotal * (fretePorValor.PercentualSobreNF / 100));

                            valorPedagio = fretePorValor.ValorPedagio;

                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual)
                            {
                                valorFrete = valorSobrePercentual + fretePorValor.ValorMinimoFrete;
                            }
                            else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                            { //Minimo Garantido
                                valorFrete = fretePorValor.ValorMinimoFrete > valorSobrePercentual ? fretePorValor.ValorMinimoFrete : valorSobrePercentual;
                            }
                            else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF)
                            { //Percentual
                                valorFrete = valorSobrePercentual;
                            }
                        }
                        else
                        {
                            //Busca tabela valor com cliente e sem localidade destino
                            fretePorValor = repFretePorValor.BuscarParaCalculo(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true, null, codigoIntegracao);

                            if (fretePorValor != null)
                            {
                                List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                                decimal valorTotal = documentos.Sum(o => o.Valor);
                                decimal valorSobrePercentual = (valorTotal * (fretePorValor.PercentualSobreNF / 100));

                                valorPedagio = fretePorValor.ValorPedagio;

                                if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual)
                                {
                                    valorFrete = valorSobrePercentual + fretePorValor.ValorMinimoFrete;
                                }
                                else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                                { //Minimo Garantido
                                    valorFrete = fretePorValor.ValorMinimoFrete > valorSobrePercentual ? fretePorValor.ValorMinimoFrete : valorSobrePercentual;
                                }
                                else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF)
                                { //Percentual
                                    valorFrete = valorSobrePercentual;
                                }
                            }
                            else
                            {
                                //Busca tabela valor sem cliente e com localidade destino
                                fretePorValor = repFretePorValor.BuscarParaCalculo(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true, null, codigoIntegracao);

                                if (fretePorValor != null)
                                {
                                    List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                                    decimal valorTotal = documentos.Sum(o => o.Valor);
                                    decimal valorSobrePercentual = (valorTotal * (fretePorValor.PercentualSobreNF / 100));

                                    valorPedagio = fretePorValor.ValorPedagio;

                                    if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual)
                                    {
                                        valorFrete = valorSobrePercentual + fretePorValor.ValorMinimoFrete;
                                    }
                                    else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                                    { //Minimo Garantido
                                        valorFrete = fretePorValor.ValorMinimoFrete > valorSobrePercentual ? fretePorValor.ValorMinimoFrete : valorSobrePercentual;
                                    }
                                    else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF)
                                    { //Percentual
                                        valorFrete = valorSobrePercentual;
                                    }
                                }
                                else
                                {
                                    Dominio.Entidades.Veiculo veiculo = nfse.Veiculo;

                                    if (veiculo != null && !string.IsNullOrWhiteSpace(veiculo.TipoVeiculo))
                                    {
                                        Dominio.Entidades.FretePorTipoDeVeiculo fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoEDescricaoTipoVeiculo(codigoEmpresa, cpfCnpjTomador, cpfCnpjTomador, veiculo.TipoDoVeiculo.Descricao ?? string.Empty, "A", true);

                                        if (fretePorTipoVeiculo != null)
                                        {
                                            List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                                            decimal valorTotal = documentos.Sum(o => o.Valor);

                                            valorFrete = fretePorTipoVeiculo.ValorFrete;
                                            valorPedagio = fretePorTipoVeiculo.ValorPedagio;
                                            valorDescarga = fretePorTipoVeiculo.ValorDescarga;

                                            if (fretePorTipoVeiculo.PercentualGris > 0 && valorTotal > 0)
                                                valorGris = (valorTotal * (fretePorTipoVeiculo.PercentualGris / 100));
                                            if (fretePorTipoVeiculo.PercentualAdValorem > 0 && valorTotal > 0)
                                                valorAdValorem = (valorTotal * (fretePorTipoVeiculo.PercentualAdValorem / 100));
                                            else if (fretePorTipoVeiculo.ValorAdValorem > 0)
                                                valorAdValorem = fretePorTipoVeiculo.ValorAdValorem;
                                        }
                                        else
                                        {
                                            fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoEDescricaoTipoVeiculo(codigoEmpresa, cpfCnpjTomador, 0, veiculo.TipoDoVeiculo.Descricao ?? string.Empty, "A", true);

                                            if (fretePorTipoVeiculo != null)
                                            {
                                                List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                                                decimal valorTotal = documentos.Sum(o => o.Valor);

                                                valorFrete = fretePorTipoVeiculo.ValorFrete;
                                                valorPedagio = fretePorTipoVeiculo.ValorPedagio;
                                                valorDescarga = fretePorTipoVeiculo.ValorDescarga;

                                                if (fretePorTipoVeiculo.PercentualGris > 0 && valorTotal > 0)
                                                    valorGris = (valorTotal * (fretePorTipoVeiculo.PercentualGris / 100));
                                                if (fretePorTipoVeiculo.PercentualAdValorem > 0 && valorTotal > 0)
                                                    valorAdValorem = (valorTotal * (fretePorTipoVeiculo.PercentualAdValorem / 100));
                                                else if (fretePorTipoVeiculo.ValorAdValorem > 0)
                                                    valorAdValorem = fretePorTipoVeiculo.ValorAdValorem;
                                            }
                                            else
                                            {
                                                fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorLocalidadeOrigemDestinoEDescricaoTipoVeiculo(codigoEmpresa, codigoLocalidadePrestacao, codigoLocalidadePrestacao, veiculo.TipoDoVeiculo.Descricao ?? string.Empty, "A", true, null);

                                                if (fretePorTipoVeiculo != null)
                                                {
                                                    List<Dominio.Entidades.DocumentosNFSe> documentos = repDocumentosNFSe.BuscarPorNFSe(nfse.Codigo);
                                                    decimal valorTotal = documentos.Sum(o => o.Valor);

                                                    valorFrete = fretePorTipoVeiculo.ValorFrete;
                                                    valorPedagio = fretePorTipoVeiculo.ValorPedagio;
                                                    valorDescarga = fretePorTipoVeiculo.ValorDescarga;

                                                    if (fretePorTipoVeiculo.PercentualGris > 0 && valorTotal > 0)
                                                        valorGris = (valorTotal * (fretePorTipoVeiculo.PercentualGris / 100));

                                                    if (fretePorTipoVeiculo.PercentualAdValorem > 0 && valorTotal > 0)
                                                        valorAdValorem = (valorTotal * (fretePorTipoVeiculo.PercentualAdValorem / 100));
                                                    else if (fretePorTipoVeiculo.ValorAdValorem > 0)
                                                        valorAdValorem = fretePorTipoVeiculo.ValorAdValorem;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (valorFrete > 0)
            {
                this.SalvarValoresFrete(ref nfse, valorFrete + valorExedentePeso + valorExedenteValor, valorAdValorem, valorPedagio, valorGris, valorDescarga, calcularImpostos, valorSeguro, valorOutros, valorMinimo, valorTAS, unidadeDeTrabalho);
                return true;
            }
            else
                return false;
        }

        public decimal CalcularFretePorNotaImportada(Dominio.Entidades.XMLNotaFiscalEletronica notaImportada, int codigoEmpresa, string codigoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (notaImportada.Emitente == null)
                return 0;

            Repositorio.FreteFracionadoUnidade repFreteFracionadoUnidade = new Repositorio.FreteFracionadoUnidade(unidadeDeTrabalho);
            Repositorio.FreteFracionadoValor repFreteFracionadoValor = new Repositorio.FreteFracionadoValor(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.Frete repFretePorPeso = new Repositorio.Frete(unidadeDeTrabalho);
            Repositorio.FretePorValor repFretePorValor = new Repositorio.FretePorValor(unidadeDeTrabalho);
            Repositorio.FretePorTipoDeVeiculo repFretePorTipoVeiculo = new Repositorio.FretePorTipoDeVeiculo(unidadeDeTrabalho);
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            double cpfCnpjTomador = notaImportada.Emitente.CPF_CNPJ;
            int ibgeLocalidadePrestacao = notaImportada.Emitente.Localidade.CodigoIBGE;
            int codigoLocalidadePrestacao = notaImportada.Emitente.Localidade.Codigo;
            if (notaImportada.FormaDePagamento != "0")
            {
                cpfCnpjTomador = notaImportada.Destinatario.CPF_CNPJ;
                ibgeLocalidadePrestacao = notaImportada.Destinatario.Localidade.CodigoIBGE;
                codigoLocalidadePrestacao = notaImportada.Destinatario.Localidade.Codigo;
            }

            if (notaImportada.UtilizarContratanteComoTomador && notaImportada.Contratante != null)
            {
                cpfCnpjTomador = notaImportada.Contratante.CPF_CNPJ;
                ibgeLocalidadePrestacao = notaImportada.Contratante.Localidade.CodigoIBGE;
                codigoLocalidadePrestacao = notaImportada.Contratante.Localidade.Codigo;
            }

            decimal valorFrete = 0;
            decimal valorAdValorem = 0;
            decimal valorGris = 0;
            decimal valorExedentePeso = 0;
            decimal valorExedenteValor = 0;
            decimal pesoMaximoTabela = 0;
            decimal valorMaximoTabela = 0;
            decimal pesoTotalNFSe = 0;
            decimal valorTotalNFe = 0;
            decimal valorPedagio = 0;
            decimal valorDescarga = 0;
            decimal valorSeguro = 0;
            decimal valorOutros = 0;
            decimal valorMinimo = 0;
            decimal valorTAS = 0;

            List<Dominio.Entidades.FreteFracionadoUnidade> listaFreteFracionadoUnidade = repFreteFracionadoUnidade.Buscar(codigoEmpresa, "A", cpfCnpjTomador, null, ibgeLocalidadePrestacao);

            if (listaFreteFracionadoUnidade.Count > 0)
            {
                pesoMaximoTabela = listaFreteFracionadoUnidade.Max(x => x.PesoAte);
                valorTotalNFe = notaImportada.Valor;
                pesoTotalNFSe = notaImportada.Peso;

                if (pesoTotalNFSe > pesoMaximoTabela)
                {
                    Dominio.Entidades.FreteFracionadoUnidade frete = listaFreteFracionadoUnidade.LastOrDefault();

                    valorFrete = frete.ValorFrete;
                    valorAdValorem = frete.PercentualAdValorem > 0 ? valorTotalNFe * (frete.PercentualAdValorem / 100) : 0;
                    valorGris = frete.PercentualGris > 0 ? valorTotalNFe * (frete.PercentualGris / 100) : 0;
                    valorPedagio = frete.ValorPedagio;
                    valorTAS = frete.ValorTAS;
                    decimal pesoExcedente = pesoTotalNFSe - pesoMaximoTabela;
                    valorExedentePeso = pesoExcedente * frete.ValorExcedente;

                    if (frete.ValorMinimoGris > 0 && valorGris < frete.ValorMinimoGris)
                        valorGris = frete.ValorMinimoGris;
                    if (frete.ValorMinimoAdValorem > 0 && valorAdValorem < frete.ValorMinimoAdValorem)
                        valorAdValorem = frete.ValorMinimoAdValorem;

                    if (frete.ValorPorUnidadeMedida > 0 && pesoTotalNFSe > 0)
                        valorFrete += (frete.ValorPorUnidadeMedida * pesoTotalNFSe);
                }
                else
                {
                    for (int i = 0; i < listaFreteFracionadoUnidade.Count; i++)
                    {
                        if (listaFreteFracionadoUnidade[i].PesoDe <= pesoTotalNFSe && listaFreteFracionadoUnidade[i].PesoAte >= pesoTotalNFSe)
                        {
                            valorFrete = listaFreteFracionadoUnidade[i].ValorFrete;
                            valorAdValorem = listaFreteFracionadoUnidade[i].PercentualAdValorem > 0 ? valorTotalNFe * (listaFreteFracionadoUnidade[i].PercentualAdValorem / 100) : 0;
                            valorGris = listaFreteFracionadoUnidade[i].PercentualGris > 0 ? valorTotalNFe * (listaFreteFracionadoUnidade[i].PercentualGris / 100) : 0;
                            valorPedagio = listaFreteFracionadoUnidade[i].ValorPedagio;
                            valorTAS = listaFreteFracionadoUnidade[i].ValorTAS;
                            valorExedentePeso = 0;

                            if (listaFreteFracionadoUnidade[i].ValorMinimoGris > 0 && valorGris < listaFreteFracionadoUnidade[i].ValorMinimoGris)
                                valorGris = listaFreteFracionadoUnidade[i].ValorMinimoGris;
                            if (listaFreteFracionadoUnidade[i].ValorMinimoAdValorem > 0 && valorAdValorem < listaFreteFracionadoUnidade[i].ValorMinimoAdValorem)
                                valorAdValorem = listaFreteFracionadoUnidade[i].ValorMinimoAdValorem;

                            if (listaFreteFracionadoUnidade[i].ValorPorUnidadeMedida > 0 && pesoTotalNFSe > 0)
                                valorFrete += (listaFreteFracionadoUnidade[i].ValorPorUnidadeMedida * pesoTotalNFSe);

                            break;
                        }
                    }
                }
            }
            else
            {
                List<Dominio.Entidades.FreteFracionadoValor> listaFreteFracionadoValor = repFreteFracionadoValor.Buscar(codigoEmpresa, "A", cpfCnpjTomador, null, ibgeLocalidadePrestacao);

                if (listaFreteFracionadoValor.Count > 0)
                {
                    valorMaximoTabela = listaFreteFracionadoValor.Max(x => x.ValorAte);
                    valorTotalNFe = notaImportada.Valor;

                    if (valorTotalNFe > valorMaximoTabela)
                    {
                        Dominio.Entidades.FreteFracionadoValor frete = listaFreteFracionadoValor.LastOrDefault();

                        valorFrete = frete.TipoValor == "P" ? valorTotalNFe * (frete.ValorFrete / 100) : frete.ValorFrete;
                        valorAdValorem = frete.PercentualAdValorem > 0 ? valorTotalNFe * (frete.PercentualAdValorem / 100) : 0;
                        valorGris = frete.PercentualGris > 0 ? valorTotalNFe * (frete.PercentualGris / 100) : 0;
                        valorPedagio = frete.ValorPedagio;
                        valorTAS = frete.ValorTAS;
                        decimal valorExcedente = valorTotalNFe - valorMaximoTabela;
                        valorExedenteValor = valorExcedente * frete.ValorExcedente;

                        if (frete.ValorMinimoGris > 0 && valorGris < frete.ValorMinimoGris)
                            valorGris = frete.ValorMinimoGris;
                        if (frete.ValorMinimoAdValorem > 0 && valorAdValorem < frete.ValorMinimoAdValorem)
                            valorAdValorem = frete.ValorMinimoAdValorem;
                    }
                    else
                    {
                        for (int i = 0; i < listaFreteFracionadoValor.Count; i++)
                        {
                            if (listaFreteFracionadoValor[i].ValorDe <= valorTotalNFe && listaFreteFracionadoValor[i].ValorAte >= valorTotalNFe)
                            {
                                valorFrete = listaFreteFracionadoValor[i].TipoValor == "P" ? valorTotalNFe * (listaFreteFracionadoValor[i].ValorFrete / 100) : listaFreteFracionadoValor[i].ValorFrete;
                                valorAdValorem = listaFreteFracionadoValor[i].PercentualAdValorem > 0 ? valorTotalNFe * (listaFreteFracionadoValor[i].PercentualAdValorem / 100) : 0;
                                valorGris = listaFreteFracionadoValor[i].PercentualGris > 0 ? valorTotalNFe * (listaFreteFracionadoValor[i].PercentualGris / 100) : 0;
                                valorPedagio = listaFreteFracionadoValor[i].ValorPedagio;
                                valorTAS = listaFreteFracionadoValor[i].ValorTAS;
                                valorExedenteValor = 0;

                                if (listaFreteFracionadoValor[i].ValorMinimoGris > 0 && valorGris < listaFreteFracionadoValor[i].ValorMinimoGris)
                                    valorGris = listaFreteFracionadoValor[i].ValorMinimoGris;
                                if (listaFreteFracionadoValor[i].ValorMinimoAdValorem > 0 && valorAdValorem < listaFreteFracionadoValor[i].ValorMinimoAdValorem)
                                    valorAdValorem = listaFreteFracionadoValor[i].ValorMinimoAdValorem;

                                break;
                            }
                        }
                    }
                }
                else
                {
                    Dominio.Entidades.Frete fretePorPeso = repFretePorPeso.BuscarPorOrigemEDestino(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true);

                    if (fretePorPeso != null)
                    {
                        decimal quantidade = notaImportada.Peso;
                        decimal valorNotas = notaImportada.Valor;

                        valorFrete = (fretePorPeso.ValorFrete * (quantidade > fretePorPeso.QuantidadeExcedente && fretePorPeso.QuantidadeExcedente > 0 && fretePorPeso.ValorExcedente > 0 ? fretePorPeso.QuantidadeExcedente : quantidade));// + fretePorPeso.ValorSeguro + fretePorPeso.OutrosValores;

                        valorPedagio = fretePorPeso.ValorPedagio;
                        if (fretePorPeso.ValorPedagioPerc > 0)
                            valorPedagio += (fretePorPeso.ValorPedagioPerc * fretePorPeso.ValorPedagioPerc);

                        valorSeguro = fretePorPeso.ValorSeguro;
                        valorOutros = fretePorPeso.OutrosValores;
                        valorDescarga = fretePorPeso.ValorDescarga;

                        if (fretePorPeso.PercentualGris > 0 && valorNotas > 0)
                            valorGris = (valorNotas * (fretePorPeso.PercentualGris / 100));
                        if (fretePorPeso.PercentualAdValorem > 0 && valorNotas > 0)
                            valorAdValorem = (valorNotas * (fretePorPeso.PercentualAdValorem / 100));

                        if (fretePorPeso.ValorExcedente > 0 && fretePorPeso.QuantidadeExcedente < quantidade)
                            valorFrete += ((quantidade - fretePorPeso.QuantidadeExcedente) * fretePorPeso.ValorExcedente);

                        valorMinimo = fretePorPeso.ValorMinimoFrete;
                    }
                    else
                    {
                        //Busca tabela valor com cliente e com localidade destino
                        Dominio.Entidades.FretePorValor fretePorValor = repFretePorValor.BuscarParaCalculo(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true, null, codigoIntegracao);

                        if (fretePorValor != null)
                        {
                            decimal valorTotal = notaImportada.Valor;
                            decimal valorSobrePercentual = (valorTotal * (fretePorValor.PercentualSobreNF / 100));

                            valorPedagio = fretePorValor.ValorPedagio;

                            if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual)
                            {
                                valorFrete = valorSobrePercentual + fretePorValor.ValorMinimoFrete;
                            }
                            else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                            { //Minimo Garantido
                                valorFrete = fretePorValor.ValorMinimoFrete > valorSobrePercentual ? fretePorValor.ValorMinimoFrete : valorSobrePercentual;
                            }
                            else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF)
                            { //Percentual
                                valorFrete = valorSobrePercentual;
                            }
                        }
                        else
                        {
                            //Busca tabela valor com cliente e sem localidade destino
                            fretePorValor = repFretePorValor.BuscarParaCalculo(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true, null, codigoIntegracao);

                            if (fretePorValor != null)
                            {
                                decimal valorTotal = notaImportada.Valor;
                                decimal valorSobrePercentual = (valorTotal * (fretePorValor.PercentualSobreNF / 100));

                                valorPedagio = fretePorValor.ValorPedagio;

                                if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual)
                                {
                                    valorFrete = valorSobrePercentual + fretePorValor.ValorMinimoFrete;
                                }
                                else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                                { //Minimo Garantido
                                    valorFrete = fretePorValor.ValorMinimoFrete > valorSobrePercentual ? fretePorValor.ValorMinimoFrete : valorSobrePercentual;
                                }
                                else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF)
                                { //Percentual
                                    valorFrete = valorSobrePercentual;
                                }
                            }
                            else
                            {
                                //Busca tabela valor sem cliente e com localidade destino
                                fretePorValor = repFretePorValor.BuscarParaCalculo(codigoEmpresa, cpfCnpjTomador, codigoLocalidadePrestacao, true, null, codigoIntegracao);

                                if (fretePorValor != null)
                                {
                                    decimal valorTotal = notaImportada.Valor;
                                    decimal valorSobrePercentual = (valorTotal * (fretePorValor.PercentualSobreNF / 100));

                                    valorPedagio = fretePorValor.ValorPedagio;

                                    if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoMaisPercentual)
                                    {
                                        valorFrete = valorSobrePercentual + fretePorValor.ValorMinimoFrete;
                                    }
                                    else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.ValorMinimoGarantido)
                                    { //Minimo Garantido
                                        valorFrete = fretePorValor.ValorMinimoFrete > valorSobrePercentual ? fretePorValor.ValorMinimoFrete : valorSobrePercentual;
                                    }
                                    else if (fretePorValor.Tipo == Dominio.Enumeradores.TipoFreteValor.SomentePercentualSobreNF)
                                    { //Percentual
                                        valorFrete = valorSobrePercentual;
                                    }
                                }
                                else
                                {
                                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(codigoEmpresa, notaImportada.Placa);

                                    if (veiculo != null && !string.IsNullOrWhiteSpace(veiculo.TipoVeiculo))
                                    {
                                        Dominio.Entidades.FretePorTipoDeVeiculo fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoEDescricaoTipoVeiculo(codigoEmpresa, cpfCnpjTomador, cpfCnpjTomador, veiculo.TipoDoVeiculo.Descricao ?? string.Empty, "A", true);

                                        if (fretePorTipoVeiculo != null)
                                        {
                                            decimal valorTotal = notaImportada.Valor;

                                            valorFrete = fretePorTipoVeiculo.ValorFrete;
                                            valorPedagio = fretePorTipoVeiculo.ValorPedagio;
                                            valorDescarga = fretePorTipoVeiculo.ValorDescarga;

                                            if (fretePorTipoVeiculo.PercentualGris > 0 && valorTotal > 0)
                                                valorGris = (valorTotal * (fretePorTipoVeiculo.PercentualGris / 100));
                                            if (fretePorTipoVeiculo.PercentualAdValorem > 0 && valorTotal > 0)
                                                valorAdValorem = (valorTotal * (fretePorTipoVeiculo.PercentualAdValorem / 100));
                                            else if (fretePorTipoVeiculo.ValorAdValorem > 0)
                                                valorAdValorem = fretePorTipoVeiculo.ValorAdValorem;
                                        }
                                        else
                                        {
                                            fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorOrigemDestinoEDescricaoTipoVeiculo(codigoEmpresa, cpfCnpjTomador, 0, veiculo.TipoDoVeiculo.Descricao ?? string.Empty, "A", true);

                                            if (fretePorTipoVeiculo != null)
                                            {
                                                decimal valorTotal = notaImportada.Valor;

                                                valorFrete = fretePorTipoVeiculo.ValorFrete;
                                                valorPedagio = fretePorTipoVeiculo.ValorPedagio;
                                                valorDescarga = fretePorTipoVeiculo.ValorDescarga;

                                                if (fretePorTipoVeiculo.PercentualGris > 0 && valorTotal > 0)
                                                    valorGris = (valorTotal * (fretePorTipoVeiculo.PercentualGris / 100));
                                                if (fretePorTipoVeiculo.PercentualAdValorem > 0 && valorTotal > 0)
                                                    valorAdValorem = (valorTotal * (fretePorTipoVeiculo.PercentualAdValorem / 100));
                                                else if (fretePorTipoVeiculo.ValorAdValorem > 0)
                                                    valorAdValorem = fretePorTipoVeiculo.ValorAdValorem;
                                            }
                                            else
                                            {
                                                fretePorTipoVeiculo = repFretePorTipoVeiculo.BuscarPorLocalidadeOrigemDestinoEDescricaoTipoVeiculo(codigoEmpresa, codigoLocalidadePrestacao, codigoLocalidadePrestacao, veiculo.TipoDoVeiculo.Descricao ?? string.Empty, "A", true, null);

                                                if (fretePorTipoVeiculo != null)
                                                {
                                                    decimal valorTotal = notaImportada.Valor;

                                                    valorFrete = fretePorTipoVeiculo.ValorFrete;
                                                    valorPedagio = fretePorTipoVeiculo.ValorPedagio;
                                                    valorDescarga = fretePorTipoVeiculo.ValorDescarga;

                                                    if (fretePorTipoVeiculo.PercentualGris > 0 && valorTotal > 0)
                                                        valorGris = (valorTotal * (fretePorTipoVeiculo.PercentualGris / 100));

                                                    if (fretePorTipoVeiculo.PercentualAdValorem > 0 && valorTotal > 0)
                                                        valorAdValorem = (valorTotal * (fretePorTipoVeiculo.PercentualAdValorem / 100));
                                                    else if (fretePorTipoVeiculo.ValorAdValorem > 0)
                                                        valorAdValorem = fretePorTipoVeiculo.ValorAdValorem;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            valorFrete = (valorFrete + valorPedagio + valorAdValorem + valorGris + valorDescarga + valorSeguro + valorOutros + valorTAS);
            if (valorFrete < valorMinimo)
                valorFrete = valorMinimo;

            return valorFrete;
        }

        private void ObterParticipante(ref Dominio.Entidades.NFSe nfse, Dominio.ObjetosDeValor.CTe.Cliente participante, Dominio.Enumeradores.TipoClienteNotaFiscalServico tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (participante != null)
            {
                double cpfCnpj = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(participante.CPFCNPJ), out cpfCnpj);

                if (cpfCnpj > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                    if (!participante.Exportacao)
                    {
                        if (!string.IsNullOrWhiteSpace(participante.RazaoSocial)) //Só cadastra se foram enviados os dados
                        {
                            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
                            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                            bool inserir = false;

                            if (cliente == null)
                            {
                                inserir = true;
                                cliente = new Dominio.Entidades.Cliente();
                            }

                            cliente.CPF_CNPJ = cpfCnpj;
                            cliente.Bairro = Utilidades.String.Left(participante.Bairro, 60);
                            cliente.CEP = participante.CEP;
                            cliente.Complemento = !string.IsNullOrWhiteSpace(participante.Complemento) && participante.Complemento.Length > 2 ? Utilidades.String.Left(participante.Complemento, 60) : null;
                            cliente.Email = participante.Emails;
                            cliente.EmailStatus = participante.StatusEmails ? "A" : "I";
                            cliente.EmailContador = participante.EmailsContador;
                            cliente.EmailContadorStatus = participante.StatusEmailsContador ? "A" : "I";
                            cliente.EmailContato = participante.EmailsContato;
                            cliente.EmailContatoStatus = participante.StatusEmailsContato ? "A" : "I";
                            cliente.Endereco = Utilidades.String.Left(participante.Endereco, 255);
                            cliente.IE_RG = participante.RGIE;
                            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(participante.CodigoIBGECidade);
                            cliente.Nome = Utilidades.String.Left(participante.RazaoSocial, 60);
                            cliente.NomeFantasia = Utilidades.String.Left(participante.NomeFantasia, 60);
                            cliente.Numero = Utilidades.String.Left(participante.Numero, 60);
                            cliente.Telefone1 = Utilidades.String.OnlyNumbers(participante.Telefone1);
                            cliente.Telefone2 = Utilidades.String.OnlyNumbers(participante.Telefone2);
                            cliente.Tipo = Utilidades.String.OnlyNumbers(participante.CPFCNPJ).Length == 11 ? "F" : "J";
                            cliente.Atividade = participante.CodigoAtividade > 0 ? repAtividade.BuscarPorCodigo(participante.CodigoAtividade) : Servicos.Atividade.ObterAtividade(nfse.Empresa.Codigo, cliente.Tipo, StringConexao, 0, unidadeDeTrabalho);
                            cliente.Ativo = true;
                            //cliente.CodigoIntegracao = participante.CodigoCliente;


                            if (inserir)
                            {
                                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                                {
                                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                                    if (grupoPessoas != null)
                                    {
                                        cliente.GrupoPessoas = grupoPessoas;
                                    }
                                }
                                cliente.DataCadastro = DateTime.Now;
                                cliente.DataUltimaAtualizacao = DateTime.Now;
                                cliente.Integrado = false;
                                repCliente.Inserir(cliente);
                            }
                            else
                            {
                                cliente.DataUltimaAtualizacao = DateTime.Now;
                                cliente.Integrado = false;
                                repCliente.Atualizar(cliente);
                            }
                        }

                        if (cliente != null)
                            nfse.SetarParticipante(cliente, tipo);
                    }
                    else if (participante.Exportacao)
                    {
                        Repositorio.Pais repPais = new Repositorio.Pais(unidadeDeTrabalho);

                        nfse.SetarParticipanteExportacao(participante, tipo, repPais.BuscarPorSigla(participante.CodigoPais));
                    }
                }
            }
            else
            {
                Dominio.Entidades.ParticipanteNFSe part = nfse.ObterParticipante(tipo);

                if (part != null)
                {
                    Repositorio.ParticipanteNFSe repParticipante = new Repositorio.ParticipanteNFSe(unidadeDeTrabalho);

                    nfse.SetarParticipante(null, tipo);

                    repParticipante.Deletar(part);
                }
            }
        }

        private void ObterParticipanteCTe(ref Dominio.Entidades.NFSe nfse, Dominio.Entidades.ParticipanteCTe participante, Dominio.Enumeradores.TipoClienteNotaFiscalServico tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (participante != null && !participante.Exterior)
            {
                double cpfCnpj = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(participante.CPF_CNPJ), out cpfCnpj);

                if (cpfCnpj > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                    if (cliente != null)
                        nfse.SetarParticipante(cliente, tipo);
                }
            }
            else
            {
                Dominio.Entidades.ParticipanteNFSe part = nfse.ObterParticipante(tipo);

                if (part != null)
                {
                    Repositorio.ParticipanteNFSe repParticipante = new Repositorio.ParticipanteNFSe(unidadeDeTrabalho);

                    nfse.SetarParticipante(null, tipo);

                    repParticipante.Deletar(part);
                }
            }
        }

        private void SalvarCliente(Dominio.ObjetosDeValor.CTe.Cliente participante, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (participante != null)
            {
                double cpfCnpj = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(participante.CPFCNPJ), out cpfCnpj);

                if (cpfCnpj > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                    if (!participante.Exportacao)
                    {
                        if (!string.IsNullOrWhiteSpace(participante.RazaoSocial)) //Só insere o cliente se tiver todos dados.
                        {
                            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
                            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                            bool inserir = false;

                            if (cliente == null)
                            {
                                inserir = true;
                                cliente = new Dominio.Entidades.Cliente();
                            }

                            cliente.CPF_CNPJ = cpfCnpj;
                            cliente.Bairro = Utilidades.String.Left(participante.Bairro, 60);
                            cliente.CEP = participante.CEP;
                            cliente.Complemento = !string.IsNullOrWhiteSpace(participante.Complemento) && participante.Complemento.Length > 2 ? Utilidades.String.Left(participante.Complemento, 60) : null;
                            cliente.Email = participante.Emails;
                            cliente.EmailStatus = participante.StatusEmails ? "A" : "I";
                            cliente.EmailContador = participante.EmailsContador;
                            cliente.EmailContadorStatus = participante.StatusEmailsContador ? "A" : "I";
                            cliente.EmailContato = participante.EmailsContato;
                            cliente.EmailContatoStatus = participante.StatusEmailsContato ? "A" : "I";
                            cliente.Endereco = Utilidades.String.Left(participante.Endereco, 255);
                            cliente.IE_RG = participante.RGIE;
                            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(participante.CodigoIBGECidade);
                            cliente.Nome = Utilidades.String.Left(participante.RazaoSocial, 60);
                            cliente.NomeFantasia = Utilidades.String.Left(participante.NomeFantasia, 60);
                            cliente.Numero = Utilidades.String.Left(participante.Numero, 60);
                            cliente.Telefone1 = Utilidades.String.OnlyNumbers(participante.Telefone1);
                            cliente.Telefone2 = Utilidades.String.OnlyNumbers(participante.Telefone2);
                            cliente.Tipo = Utilidades.String.OnlyNumbers(participante.CPFCNPJ).Length == 11 ? "F" : "J";
                            cliente.Atividade = participante.CodigoAtividade > 0 ? repAtividade.BuscarPorCodigo(participante.CodigoAtividade) : repAtividade.BuscarPrimeiraAtividade(); ;
                            //cliente.CodigoIntegracao = participante.CodigoCliente;
                            cliente.Ativo = true;
                            if (inserir)
                            {
                                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                                {
                                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                                    if (grupoPessoas != null)
                                    {
                                        cliente.GrupoPessoas = grupoPessoas;
                                    }
                                }
                                cliente.DataCadastro = DateTime.Now;
                                cliente.DataUltimaAtualizacao = DateTime.Now;
                                cliente.Integrado = false;
                                repCliente.Inserir(cliente);
                            }
                            else
                            {
                                cliente.DataUltimaAtualizacao = DateTime.Now;
                                cliente.Integrado = false;
                                repCliente.Atualizar(cliente);
                            }
                        }
                    }
                }
            }
        }

        private void ObterNatureza(ref Dominio.Entidades.NFSe nfse, Dominio.ObjetosDeValor.NFSe.Natureza natureza, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);

            Dominio.Entidades.NaturezaNFSe nat = repNatureza.BuscarPorNumero(nfse.Empresa.Codigo, natureza.Numero);

            if (nat == null)
            {
                nat = new Dominio.Entidades.NaturezaNFSe();

                nat.Descricao = natureza.Descricao;
                nat.Empresa = nfse.Empresa;
                nat.Numero = natureza.Numero;
                nat.Status = "A";

                repNatureza.Inserir(nat);
            }

            nfse.Natureza = nat;
        }

        private void SalvarItens(ref Dominio.Entidades.NFSe nfse, List<Dominio.ObjetosDeValor.NFSe.Item> itens, Dominio.ObjetosDeValor.CTe.ImpostoICMS impostos, Dominio.Enumeradores.OpcaoSimNao incluirISSNoFrete, string issRetidoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.CTe.TributacaoNFSe tributacaoNFSe = null, Dominio.Entidades.ServicoNFSe servico = null)
        {
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Pais repPais = new Repositorio.Pais(unidadeDeTrabalho);

            bool inclusaoISSConfigurada = false;
            bool incluirISSNFSeLocalidadeTomadorDiferentePrestador = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().IncluirISSNFSeLocalidadeTomadorDiferentePrestador.Value;

            if (incluirISSNFSeLocalidadeTomadorDiferentePrestador)
            {
                if (nfse.Tomador.Localidade != nfse.Empresa.Localidade)
                    incluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                else
                    incluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
                inclusaoISSConfigurada = true;
            }

            if (servico == null)
                servico = this.ObterServicoNFSe(nfse.Empresa, nfse.Empresa.Localidade.CodigoIBGE != nfse.LocalidadePrestacaoServico.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unidadeDeTrabalho);

            if (servico != null && servico.ISSIncluso != Dominio.Enumeradores.InclusaoISSNFSe.ConformeIntegracao)
            {
                if (servico.ISSIncluso == Dominio.Enumeradores.InclusaoISSNFSe.SempreIncliur)
                    incluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                else if (servico.ISSIncluso == Dominio.Enumeradores.InclusaoISSNFSe.NuncaIncluir)
                    incluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
                inclusaoISSConfigurada = true;
            }

            if (itens != null && itens.Count() > 0)
            {
                foreach (Dominio.ObjetosDeValor.NFSe.Item item in itens)
                {
                    Dominio.Entidades.ItemNFSe itemNFSe = new Dominio.Entidades.ItemNFSe();

                    itemNFSe.BaseCalculoISS = item.BaseCalculoISS;
                    if (item.Discriminacao != null && item.Discriminacao != "")
                        itemNFSe.Discriminacao = item.Discriminacao;
                    else if (nfse.OutrasInformacoes != null && nfse.OutrasInformacoes != "")
                        itemNFSe.Discriminacao = "-" + nfse.OutrasInformacoes;
                    else if (nfse.OutrasInformacoes != null && nfse.OutrasInformacoes != "")
                        itemNFSe.Discriminacao = "-" + nfse.OutrasInformacoes;
                    else
                        itemNFSe.Discriminacao = item.Discriminacao;

                    itemNFSe.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)item.ExigibilidadeISS;
                    itemNFSe.Municipio = repLocalidade.BuscarPorCodigoIBGE(item.CodigoIBGECidade);
                    itemNFSe.MunicipioIncidencia = repLocalidade.BuscarPorCodigoIBGE(item.CodigoIBGECidadeIncidencia);
                    itemNFSe.NFSe = nfse;
                    itemNFSe.PaisPrestacaoServico = repPais.BuscarPorSigla(string.Format("{0:00000}", item.CodigoPaisPrestacaoServico));
                    itemNFSe.Quantidade = item.Quantidade;
                    itemNFSe.ServicoPrestadoNoPais = item.ServicoPrestadoNoPais;
                    itemNFSe.ValorDeducoes = item.ValorDeducoes;
                    itemNFSe.ValorDescontoCondicionado = item.ValorDescontoCondicionado;
                    itemNFSe.ValorDescontoIncondicionado = item.ValorDescontoIncondicionado;
                    itemNFSe.ValorServico = item.ValorServico;

                    if (item.Servico == null && servico != null)
                    {
                        itemNFSe.Servico = servico;
                        itemNFSe.AliquotaISS = item.AliquotaISS > 0 ? item.AliquotaISS : servico.Aliquota;
                        nfse.AliquotaISS = item.AliquotaISS > 0 ? item.AliquotaISS : servico.Aliquota;
                        if (!string.IsNullOrWhiteSpace(issRetidoIntegracao))
                            nfse.ISSRetido = issRetidoIntegracao.ToUpper() == "SIM";
                        else
                            nfse.ISSRetido = servico.ISSRetido != null ? servico.ISSRetido : false;

                        if (inclusaoISSConfigurada)
                            itemNFSe.IncluirISSNoFrete = incluirISSNoFrete;
                        else
                            itemNFSe.IncluirISSNoFrete = item.ISSInclusoValorTotal ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;

                        if (itemNFSe.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && item.ValorISS > 0)
                        {
                            itemNFSe.ValorServico = item.ValorServico;
                            itemNFSe.ValorTotal = itemNFSe.ValorServico * itemNFSe.Quantidade;

                            itemNFSe.ValorTotal = Math.Round(itemNFSe.ValorTotal / (1 - (itemNFSe.AliquotaISS / 100)), 2, MidpointRounding.ToEven);
                            itemNFSe.BaseCalculoISS = itemNFSe.ValorTotal;
                            itemNFSe.ValorISS = Math.Round((itemNFSe.AliquotaISS / 100) * itemNFSe.BaseCalculoISS, 2, MidpointRounding.ToEven);
                        }
                        else
                        {
                            if (item.ValorTotal > 0)
                                itemNFSe.ValorTotal = item.ValorTotal;
                            else
                                itemNFSe.ValorTotal = itemNFSe.ValorServico * itemNFSe.Quantidade;

                            if (item.BaseCalculoISS > 0)
                                itemNFSe.BaseCalculoISS = item.BaseCalculoISS;
                            else
                                itemNFSe.BaseCalculoISS = itemNFSe.ValorTotal - itemNFSe.ValorDescontoIncondicionado - itemNFSe.ValorDeducoes;
                            if (item.ValorISS > 0)
                                itemNFSe.ValorISS = item.ValorISS;
                            else
                                itemNFSe.ValorISS = (itemNFSe.AliquotaISS / 100) * itemNFSe.BaseCalculoISS;
                        }
                    }
                    else
                    {
                        itemNFSe.ValorISS = item.ValorISS;
                        itemNFSe.ValorTotal = item.ValorTotal;
                        itemNFSe.AliquotaISS = item.AliquotaISS;
                        this.ObterServico(ref itemNFSe, item.Servico, unidadeDeTrabalho);
                    }

                    if (nfse.AliquotaISS == 0 && nfse.Empresa.Configuracao != null && servico != null)
                    {
                        nfse.AliquotaISS = servico.Aliquota;
                        if (!string.IsNullOrWhiteSpace(issRetidoIntegracao))
                            nfse.ISSRetido = issRetidoIntegracao.ToUpper() == "SIM";
                        else
                            nfse.ISSRetido = servico.ISSRetido != null ? servico.ISSRetido : false;
                    }

                    if (item.IBSCBS != null)
                    {
                        Dominio.ObjetosDeValor.CTe.IBSCBS iBSCBS = item.IBSCBS;
                        itemNFSe.NBS = iBSCBS.NBS;
                        itemNFSe.CodigoIndicadorOperacao = iBSCBS.CodigoIndicadorOperacao;
                        itemNFSe.CSTIBSCBS = iBSCBS.CSTIBSCBS;
                        itemNFSe.ClassificacaoTributariaIBSCBS = iBSCBS.ClassificacaoTributariaIBSCBS;
                        itemNFSe.BaseCalculoIBSCBS = iBSCBS.BaseCalculoIBSCBS;
                        itemNFSe.AliquotaIBSEstadual = iBSCBS.AliquotaIBSEstadual;
                        itemNFSe.PercentualReducaoIBSEstadual = iBSCBS.PercentualReducaoIBSEstadual;
                        itemNFSe.ValorIBSEstadual = iBSCBS.ValorIBSEstadual;
                        itemNFSe.AliquotaIBSMunicipal = iBSCBS.AliquotaIBSMunicipal;
                        itemNFSe.PercentualReducaoIBSMunicipal = iBSCBS.PercentualReducaoIBSMunicipal;
                        itemNFSe.ValorIBSMunicipal = iBSCBS.ValorIBSMunicipal;
                        itemNFSe.AliquotaCBS = iBSCBS.AliquotaCBS;
                        itemNFSe.PercentualReducaoCBS = iBSCBS.PercentualReducaoCBS;
                        itemNFSe.ValorCBS = iBSCBS.ValorCBS;
                    }

                    nfse.BaseCalculoISS += itemNFSe.BaseCalculoISS;
                    nfse.ValorISS += itemNFSe.ValorISS;
                    nfse.ValorServicos += itemNFSe.ValorTotal;

                    repItem.Inserir(itemNFSe);
                }
            }
            else
            {
                Dominio.Entidades.ItemNFSe itemNFSe = new Dominio.Entidades.ItemNFSe();

                itemNFSe.BaseCalculoISS = nfse.BaseCalculoISS;
                if (nfse.OutrasInformacoes != null && nfse.OutrasInformacoes != "")
                    itemNFSe.Discriminacao = "#" + nfse.OutrasInformacoes;
                else
                    itemNFSe.Discriminacao = "Serviço de transporte municipal";

                //itemNFSe.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)item.ExigibilidadeISS;
                itemNFSe.Municipio = nfse.LocalidadePrestacaoServico;
                itemNFSe.MunicipioIncidencia = nfse.LocalidadePrestacaoServico;
                itemNFSe.NFSe = nfse;
                itemNFSe.PaisPrestacaoServico = repPais.BuscarPorSigla("01058");
                itemNFSe.Quantidade = 1;
                itemNFSe.ServicoPrestadoNoPais = true;
                itemNFSe.ValorServico = nfse.ValorServicos;
                itemNFSe.ValorDeducoes = 0;
                itemNFSe.ValorDescontoCondicionado = 0;
                itemNFSe.ValorDescontoIncondicionado = 0;

                itemNFSe.Servico = servico;
                itemNFSe.AliquotaISS = tributacaoNFSe?.AliquotaISS ?? servico.Aliquota;

                if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples.Value && nfse.Empresa.OptanteSimplesNacional && impostos != null && impostos.Aliquota == 0)
                    impostos = null;

                if (tributacaoNFSe != null)
                {
                    itemNFSe.BaseCalculoISS = tributacaoNFSe.BaseCalculoISS;
                    itemNFSe.AliquotaISS = tributacaoNFSe.AliquotaISS;
                    itemNFSe.ValorISS = tributacaoNFSe.ValorISS;
                    itemNFSe.ValorTotal = tributacaoNFSe.BaseCalculoISS;

                    if ((itemNFSe.ValorServico < itemNFSe.ValorTotal) && ((itemNFSe.ValorTotal - itemNFSe.ValorServico) == itemNFSe.ValorISS))
                        itemNFSe.IncluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                }
                else if (impostos != null)
                {
                    itemNFSe.BaseCalculoISS = impostos.BaseCalculo;
                    itemNFSe.AliquotaISS = impostos.Aliquota;
                    itemNFSe.ValorISS = impostos.Valor;
                    itemNFSe.ValorTotal = impostos.BaseCalculo;

                    if ((itemNFSe.ValorServico < itemNFSe.ValorTotal) && ((itemNFSe.ValorTotal - itemNFSe.ValorServico) == itemNFSe.ValorISS))
                        itemNFSe.IncluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                }
                else if (incluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && itemNFSe.AliquotaISS > 0)
                {
                    itemNFSe.IncluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;

                    itemNFSe.ValorTotal = itemNFSe.ValorServico * itemNFSe.Quantidade;

                    itemNFSe.ValorTotal = Math.Round(itemNFSe.ValorTotal / (1 - (itemNFSe.AliquotaISS / 100)), 2, MidpointRounding.ToEven);
                    itemNFSe.BaseCalculoISS = itemNFSe.ValorTotal;
                    itemNFSe.ValorISS = Math.Round((itemNFSe.AliquotaISS / 100) * itemNFSe.BaseCalculoISS, 2, MidpointRounding.ToEven);
                }
                else
                {
                    itemNFSe.IncluirISSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    itemNFSe.ValorTotal = itemNFSe.ValorServico * itemNFSe.Quantidade;
                    itemNFSe.BaseCalculoISS = itemNFSe.AliquotaISS > 0 ? itemNFSe.ValorTotal - itemNFSe.ValorDescontoIncondicionado - itemNFSe.ValorDeducoes : 0;
                    itemNFSe.ValorISS = itemNFSe.AliquotaISS > 0 ? (itemNFSe.AliquotaISS / 100) * itemNFSe.BaseCalculoISS : 0;
                }

                itemNFSe.NBS = nfse.NBS;
                itemNFSe.CodigoIndicadorOperacao = nfse.CodigoIndicadorOperacao;
                itemNFSe.CSTIBSCBS = nfse.CSTIBSCBS;
                itemNFSe.ClassificacaoTributariaIBSCBS = nfse.ClassificacaoTributariaIBSCBS;
                itemNFSe.BaseCalculoIBSCBS = nfse.BaseCalculoIBSCBS;
                itemNFSe.AliquotaIBSEstadual = nfse.AliquotaIBSEstadual;
                itemNFSe.PercentualReducaoIBSEstadual = nfse.PercentualReducaoIBSEstadual;
                itemNFSe.ValorIBSEstadual = nfse.ValorIBSEstadual;
                itemNFSe.AliquotaIBSMunicipal = nfse.AliquotaIBSMunicipal;
                itemNFSe.PercentualReducaoIBSMunicipal = nfse.PercentualReducaoIBSMunicipal;
                itemNFSe.ValorIBSMunicipal = nfse.ValorIBSMunicipal;
                itemNFSe.AliquotaCBS = nfse.AliquotaCBS;
                itemNFSe.PercentualReducaoCBS = nfse.PercentualReducaoCBS;
                itemNFSe.ValorCBS = nfse.ValorCBS;

                nfse.BaseCalculoISS = itemNFSe.BaseCalculoISS;
                nfse.ValorISS = itemNFSe.ValorISS;
                nfse.ValorServicos = itemNFSe.ValorTotal;
                if (!string.IsNullOrWhiteSpace(issRetidoIntegracao))
                    nfse.ISSRetido = issRetidoIntegracao.ToUpper() == "SIM";
                else
                    nfse.ISSRetido = servico.ISSRetido != null ? servico.ISSRetido : false;
                nfse.AliquotaISS = itemNFSe.AliquotaISS;

                repItem.Inserir(itemNFSe);
            }
        }

        private void SalvarDocumentos(Dominio.Entidades.NFSe nfse, List<Dominio.ObjetosDeValor.CTe.Documento> documentosIntegracao, Dominio.ObjetosDeValor.CTe.CTeNFSe nfseIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

            if (documentosIntegracao != null && documentosIntegracao.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.CTe.Documento documentoIntegracao in documentosIntegracao)
                {
                    Dominio.Entidades.DocumentosNFSe documento = new Dominio.Entidades.DocumentosNFSe();

                    documento.NFSe = nfse;
                    documento.Chave = documentoIntegracao.ChaveNFE;
                    documento.Numero = !string.IsNullOrWhiteSpace(documentoIntegracao.Numero) ? documentoIntegracao.Numero : !string.IsNullOrWhiteSpace(documentoIntegracao.ChaveNFE) && documentoIntegracao.ChaveNFE.Length == 44 ? documentoIntegracao.ChaveNFE.Substring(25, 9) : string.Empty;
                    documento.Serie = !string.IsNullOrWhiteSpace(documentoIntegracao.Serie) ? documentoIntegracao.Serie : !string.IsNullOrWhiteSpace(documentoIntegracao.ChaveNFE) && documentoIntegracao.ChaveNFE.Length == 44 ? documentoIntegracao.ChaveNFE.Substring(22, 3) : string.Empty;
                    documento.Valor = documentoIntegracao.Valor;
                    documento.Peso = documentoIntegracao.Peso;
                    documento.DataEmissao = DateTime.Now.Date;
                    if (nfseIntegracao != null && nfseIntegracao.Remetente != null)
                        documento.Emitente = nfseIntegracao.Remetente.CPFCNPJ;
                    if (nfseIntegracao != null && nfseIntegracao.Destinatario != null)
                        documento.Destino = nfseIntegracao.Destinatario.CPFCNPJ;

                    DateTime dataEmissao;
                    if (DateTime.TryParseExact(documentoIntegracao.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                        documento.DataEmissao = dataEmissao;

                    repDocumentosNFSe.Inserir(documento);
                }
            }
        }

        private void SalvarDocumentosCTe(Dominio.Entidades.NFSe nfse, List<Dominio.Entidades.DocumentosCTE> documentosCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

            if (documentosCTe != null && documentosCTe.Count > 0)
            {
                foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
                {
                    Dominio.Entidades.DocumentosNFSe documento = new Dominio.Entidades.DocumentosNFSe();

                    documento.NFSe = nfse;
                    documento.Chave = documentoCTe.ChaveNFE;
                    documento.Numero = documentoCTe.Numero;
                    documento.Serie = documentoCTe.SerieOuSerieDaChave;
                    documento.Valor = documentoCTe.Valor;
                    documento.Peso = documentoCTe.Peso;
                    documento.Emitente = documentoCTe.CTE.Remetente != null ? documentoCTe.CTE.Remetente.CPF_CNPJ_SemFormato : string.Empty;
                    documento.Destino = documentoCTe.CTE.Destinatario != null ? documentoCTe.CTE.Destinatario.CPF_CNPJ_SemFormato : string.Empty;
                    documento.DataEmissao = documentoCTe.DataEmissao;

                    repDocumentosNFSe.Inserir(documento);
                }
            }
        }

        private void SalvarDocumentos(Dominio.Entidades.NFSe nfse, List<Dominio.ObjetosDeValor.NFSe.Documentos> documentosIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

            if (documentosIntegracao != null && documentosIntegracao.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.NFSe.Documentos documentoIntegracao in documentosIntegracao)
                {
                    Dominio.Entidades.DocumentosNFSe documento = new Dominio.Entidades.DocumentosNFSe();

                    documento.NFSe = nfse;
                    documento.Chave = documentoIntegracao.ChaveNFE;
                    documento.Numero = documentoIntegracao.Numero;
                    documento.Serie = documentoIntegracao.Serie;
                    documento.Valor = documentoIntegracao.Valor;
                    documento.Peso = documentoIntegracao.Peso;
                    documento.DataEmissao = DateTime.Now.Date;

                    DateTime dataEmissao;
                    if (DateTime.TryParseExact(documentoIntegracao.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                        documento.DataEmissao = dataEmissao;

                    repDocumentosNFSe.Inserir(documento);
                }
            }
        }

        private void ObterServico(ref Dominio.Entidades.ItemNFSe item, Dominio.ObjetosDeValor.NFSe.Servico servico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ServicoNFSe repServico = new Repositorio.ServicoNFSe(unidadeDeTrabalho);

            Dominio.Entidades.ServicoNFSe serv = repServico.BuscarPorNumero(item.NFSe.Empresa.Codigo, servico.Numero);

            if (serv == null)
            {
                serv = new Dominio.Entidades.ServicoNFSe();

                serv.Aliquota = servico.Aliquota;
                serv.CNAE = servico.CNAE;
                serv.CodigoTributacao = servico.CodigoTributacao;
                serv.Descricao = servico.Descricao;
                serv.Empresa = item.NFSe.Empresa;
                serv.Numero = servico.Numero;
                serv.Status = "A";

                repServico.Inserir(serv);
            }

            item.Servico = serv;
        }

        private decimal ObterPesoNFe(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspVol[] volumes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            decimal peso = 0;
            if (volumes != null)
            {
                foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspVol vol in volumes)
                    peso += vol.pesoB != null ? decimal.Parse(vol.pesoB, cultura) : vol.pesoL != null ? decimal.Parse(vol.pesoL, cultura) : 0;
            }
            return peso;
        }

        private decimal ObterPesoNFe(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspVol[] volumes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            decimal peso = 0;
            if (volumes != null)
            {
                foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspVol vol in volumes)
                    peso += vol.pesoB != null ? decimal.Parse(vol.pesoB, cultura) : vol.pesoL != null ? decimal.Parse(vol.pesoL, cultura) : 0;
            }
            return peso;
        }

        private void SalvarItensNFSeProcessada(ref Dominio.Entidades.NFSe nfse, List<Dominio.ObjetosDeValor.NFSe.Item> itens, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Pais repPais = new Repositorio.Pais(unidadeDeTrabalho);
            Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unidadeDeTrabalho);

            if (itens != null && itens.Count() > 0)
            {
                foreach (Dominio.ObjetosDeValor.NFSe.Item item in itens)
                {
                    Dominio.Entidades.ItemNFSe itemNFSe = new Dominio.Entidades.ItemNFSe();

                    itemNFSe.Discriminacao = item.Discriminacao;
                    itemNFSe.BaseCalculoISS = item.BaseCalculoISS;
                    itemNFSe.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)item.ExigibilidadeISS;
                    itemNFSe.Municipio = repLocalidade.BuscarPorCodigoIBGE(item.CodigoIBGECidade);
                    itemNFSe.MunicipioIncidencia = repLocalidade.BuscarPorCodigoIBGE(item.CodigoIBGECidadeIncidencia);
                    itemNFSe.NFSe = nfse;
                    itemNFSe.PaisPrestacaoServico = item.CodigoPaisPrestacaoServico > 0 ? repPais.BuscarPorSigla(string.Format("{0:00000}", item.CodigoPaisPrestacaoServico)) : repPais.BuscarPorSigla("01058");
                    itemNFSe.Quantidade = item.Quantidade;
                    itemNFSe.ServicoPrestadoNoPais = item.ServicoPrestadoNoPais;
                    itemNFSe.ValorDeducoes = item.ValorDeducoes;
                    itemNFSe.ValorDescontoCondicionado = item.ValorDescontoCondicionado;
                    itemNFSe.ValorDescontoIncondicionado = item.ValorDescontoIncondicionado;
                    itemNFSe.ValorServico = item.ValorServico;
                    itemNFSe.ValorISS = item.ValorISS;
                    itemNFSe.ValorTotal = item.ValorTotal;
                    itemNFSe.AliquotaISS = item.AliquotaISS;
                    itemNFSe.IncluirISSNoFrete = item.ISSInclusoValorTotal ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;

                    if (item.IBSCBS != null)
                    {
                        Dominio.ObjetosDeValor.CTe.IBSCBS iBSCBS = item.IBSCBS;
                        itemNFSe.NBS = iBSCBS.NBS;
                        itemNFSe.CodigoIndicadorOperacao = iBSCBS.CodigoIndicadorOperacao;
                        itemNFSe.CSTIBSCBS = iBSCBS.CSTIBSCBS;
                        itemNFSe.ClassificacaoTributariaIBSCBS = iBSCBS.ClassificacaoTributariaIBSCBS;
                        itemNFSe.BaseCalculoIBSCBS = iBSCBS.BaseCalculoIBSCBS;
                        itemNFSe.AliquotaIBSEstadual = iBSCBS.AliquotaIBSEstadual;
                        itemNFSe.PercentualReducaoIBSEstadual = iBSCBS.PercentualReducaoIBSEstadual;
                        itemNFSe.ValorIBSEstadual = iBSCBS.ValorIBSEstadual;
                        itemNFSe.AliquotaIBSMunicipal = iBSCBS.AliquotaIBSMunicipal;
                        itemNFSe.PercentualReducaoIBSMunicipal = iBSCBS.PercentualReducaoIBSMunicipal;
                        itemNFSe.ValorIBSMunicipal = iBSCBS.ValorIBSMunicipal;
                        itemNFSe.AliquotaCBS = iBSCBS.AliquotaCBS;
                        itemNFSe.PercentualReducaoCBS = iBSCBS.PercentualReducaoCBS;
                        itemNFSe.ValorCBS = iBSCBS.ValorCBS;
                    }

                    Dominio.Entidades.ServicoNFSe servico = repServicoNFSe.BuscarPorNumero(nfse.Empresa.Codigo, item.Servico.Numero);
                    if (servico == null)
                    {
                        servico = new Dominio.Entidades.ServicoNFSe();
                        servico.Empresa = nfse.Empresa;
                        servico.Numero = item.Servico.Numero;
                        servico.CNAE = !string.IsNullOrWhiteSpace(item.Servico.CNAE) ? item.Servico.CNAE : string.Empty;
                        servico.CodigoTributacao = item.Servico.CodigoTributacao;
                        servico.Descricao = item.Servico.Descricao;
                        servico.Status = "A";

                        repServicoNFSe.Inserir(servico);
                    }

                    itemNFSe.Servico = servico;
                    repItem.Inserir(itemNFSe);
                }
            }
        }

        private void SalvarDocumentosNFSeProcessada(ref Dominio.Entidades.NFSe nfse, List<Dominio.ObjetosDeValor.NFSe.Documentos> documentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

            if (documentos != null && documentos.Count() > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Servicos.Cliente serCliente = new Cliente(unidadeDeTrabalho.StringConexao);


                foreach (Dominio.ObjetosDeValor.NFSe.Documentos documento in documentos)
                {
                    Dominio.Entidades.DocumentosNFSe documentoNFSe = new Dominio.Entidades.DocumentosNFSe();

                    DateTime dataEmissao;
                    if (!DateTime.TryParseExact(documento.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                        dataEmissao = DateTime.Now;

                    documentoNFSe.NFSe = nfse;
                    documentoNFSe.Chave = documento.ChaveNFE;
                    documentoNFSe.DataEmissao = dataEmissao;
                    documentoNFSe.Numero = documento.Numero;
                    documentoNFSe.Serie = documento.Serie;
                    documentoNFSe.Valor = documento.Valor;
                    documentoNFSe.Peso = documento.Peso;

                    if (documento.EmitenteNFe != null && !string.IsNullOrWhiteSpace(documento.EmitenteNFe.CPFCNPJ))
                    {
                        this.SalvarCliente(documento.EmitenteNFe, unidadeDeTrabalho);
                        documentoNFSe.Emitente = documento.EmitenteNFe.CPFCNPJ;
                    }
                    if (documento.DestinatarioNFe != null && !string.IsNullOrWhiteSpace(documento.DestinatarioNFe.CPFCNPJ))
                    {
                        this.SalvarCliente(documento.DestinatarioNFe, unidadeDeTrabalho);
                        documentoNFSe.Destino = documento.DestinatarioNFe.CPFCNPJ;
                    }

                    repDocumentosNFSe.Inserir(documentoNFSe);

                    nfse.PesoKG += documentoNFSe.Peso;
                }
            }
        }
        private void SalvarValoresFrete(ref Dominio.Entidades.NFSe nfse, decimal valorFrete, decimal valorAdicional, decimal valorPedagio, decimal valorGris, decimal valorDescarga, bool calcularImpostos, decimal valorSeguro, decimal valorOutros, decimal valorMinimo, decimal valorTAS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unitOfWork);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);
            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);

            valorFrete = (valorFrete + valorPedagio + valorAdicional + valorGris + valorDescarga + valorSeguro + valorOutros + valorTAS);
            if (valorFrete < valorMinimo)
                valorFrete = valorMinimo;

            decimal valorISS = 0;
            decimal valorISSRetido = 0;
            decimal valorTotalItem = 0;
            bool incluirISSNoFrete = false;

            // Busca o item de prestacao
            Dominio.Entidades.ItemNFSe itemNFSe = repItem.BuscarPorNFSe(nfse.Codigo).FirstOrDefault();

            if (itemNFSe == null)
                throw new Exception("Nenhum item da NFS-e encontrado. É necessário haver pelo menos 1 item para setar os valores de frete");

            itemNFSe.ValorServico = Math.Round(valorFrete, 2, MidpointRounding.ToEven);
            if (itemNFSe.ExigibilidadeISS == 0)
                itemNFSe.ExigibilidadeISS = Dominio.Enumeradores.ExigibilidadeISS.Exigivel;

            // Calcula valores
            valorTotalItem = (itemNFSe.ValorServico * itemNFSe.Quantidade) - itemNFSe.ValorDescontoIncondicionado - itemNFSe.ValorDeducoes;
            itemNFSe.ValorTotal = valorTotalItem;
            itemNFSe.BaseCalculoISS = valorTotalItem;

            // Calcula Valores ISS
            if (itemNFSe.AliquotaISS > 0 && itemNFSe.BaseCalculoISS > 0)
                valorISS = itemNFSe.BaseCalculoISS * (itemNFSe.AliquotaISS / 100);
            itemNFSe.ValorISS = valorISS;
            incluirISSNoFrete = itemNFSe.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && itemNFSe.AliquotaISS > 0;
            if (incluirISSNoFrete && itemNFSe.AliquotaISS > 0)
                itemNFSe.ValorTotal = itemNFSe.BaseCalculoISS;

            decimal baseCalculoIBSCBS = itemNFSe.ValorTotal;


            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
            {
                BaseCalculo = baseCalculoIBSCBS,
                ValoAbaterBaseCalculo = itemNFSe.ValorISS,
                CodigoLocalidade = nfse.LocalidadePrestacaoServico.Codigo,
                SiglaUF = nfse.LocalidadePrestacaoServico.Estado.Sigla,
                CodigoTipoOperacao = 0,
                Empresa = nfse.Empresa
            });

            Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(nfse.Empresa, nfse.LocalidadePrestacaoServico.CodigoIBGE != nfse.Empresa.Localidade.CodigoIBGE, nfse.LocalidadePrestacaoServico.CodigoIBGE, unitOfWork);

            itemNFSe.OutrasAliquotas = impostoIBSCBS != null ? new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas { Codigo = impostoIBSCBS.CodigoOutraAliquota } : null;
            itemNFSe.NBS = servicoMultiCTe?.NBS ?? "";
            itemNFSe.CodigoIndicadorOperacao = impostoIBSCBS?.CodigoIndicadorOperacao ?? "";
            itemNFSe.CSTIBSCBS = impostoIBSCBS?.CST ?? "";
            itemNFSe.ClassificacaoTributariaIBSCBS = impostoIBSCBS?.ClassificacaoTributaria ?? "";
            itemNFSe.BaseCalculoIBSCBS = impostoIBSCBS?.BaseCalculo ?? 0;
            itemNFSe.AliquotaIBSEstadual = impostoIBSCBS?.AliquotaIBSEstadual ?? 0;
            itemNFSe.PercentualReducaoIBSEstadual = impostoIBSCBS?.PercentualReducaoIBSEstadual ?? 0;
            itemNFSe.ValorIBSEstadual = impostoIBSCBS?.ValorIBSEstadual ?? 0;
            itemNFSe.AliquotaIBSMunicipal = impostoIBSCBS?.AliquotaIBSMunicipal ?? 0;
            itemNFSe.PercentualReducaoIBSMunicipal = impostoIBSCBS?.PercentualReducaoIBSMunicipal ?? 0;
            itemNFSe.ValorIBSMunicipal = impostoIBSCBS?.ValorIBSMunicipal ?? 0;
            itemNFSe.AliquotaCBS = impostoIBSCBS?.AliquotaCBS ?? 0;
            itemNFSe.PercentualReducaoCBS = impostoIBSCBS?.PercentualReducaoCBS ?? 0;
            itemNFSe.ValorCBS = impostoIBSCBS?.ValorCBS ?? 0;

            // Seta totais
            nfse.ValorServicos = itemNFSe.ValorTotal;
            nfse.ValorDeducoes = itemNFSe.ValorDeducoes;
            nfse.ValorDescontoCondicionado = itemNFSe.ValorDescontoCondicionado;
            nfse.ValorDescontoIncondicionado = itemNFSe.ValorDescontoIncondicionado;

            if (impostoIBSCBS != null)
            {
                nfse.OutrasAliquotas = new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas { Codigo = impostoIBSCBS.CodigoOutraAliquota };
                nfse.NBS = servicoMultiCTe?.NBS;
                nfse.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
                nfse.CSTIBSCBS = impostoIBSCBS.CST;
                nfse.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
                nfse.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                nfse.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                nfse.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                nfse.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                nfse.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                nfse.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                nfse.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                nfse.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                nfse.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                nfse.ValorCBS = impostoIBSCBS.ValorCBS;
            }

            nfse.AliquotaISS = itemNFSe.AliquotaISS;
            nfse.BaseCalculoISS = itemNFSe.BaseCalculoISS;
            nfse.ValorISS = itemNFSe.ValorISS;

            // Calcular ISS Retido
            if (nfse.ISSRetido)
                valorISSRetido = nfse.ValorISS;
            nfse.ValorISSRetido = valorISSRetido;

            // Calcular ISS Incluso
            if (incluirISSNoFrete)
            {
                decimal baseISS = itemNFSe.BaseCalculoISS;
                decimal valorISSIncluso = 0;

                if (itemNFSe.AliquotaISS > 0)
                    baseISS += (baseISS / ((100 - itemNFSe.AliquotaISS) / 100)) - baseISS;

                if (itemNFSe.AliquotaISS > 0 && baseISS > 0)
                    valorISSIncluso = baseISS * (itemNFSe.AliquotaISS / 100);

                itemNFSe.BaseCalculoISS = baseISS;
                itemNFSe.ValorISS = valorISS;

                if (itemNFSe.AliquotaISS > 0)
                    itemNFSe.ValorTotal = baseISS;
            }


            repItem.Atualizar(itemNFSe);
        }

        #endregion

        #region Importação

        private object ConverterXMLNFSeOsasco(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseIntegracao, XDocument doc, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, System.Globalization.CultureInfo cultura)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            XElement xDados = doc.Element("NFE").Element("NotaFiscalRelatorioDTO");
            XElement xIBSCBS = doc.Element("NFE")?.Element("NotaFiscalRelatorioDTO")?.Element("IBSCBS") ?? null;
            XElement xDadosPrestador = xDados.Element("Prestador");
            XElement xDadosTomador = xDados.Element("Tomador");
            XElement xDadosPrestadorEndereco = xDadosPrestador.Element("Endereco");
            XElement xDadosTomadorEndereco = xDadosTomador.Element("Endereco");

            if (empresa.CNPJ != xDadosPrestador.Element("CNPJ").Value)
                return "Emitente da nota não é a mesma da sua empresa";

            Dominio.Entidades.Localidade localidadeTomador = repLocalidade.BuscarPorCEP(xDadosTomadorEndereco.Element("CEP").Value);
            if (localidadeTomador == null)
                localidadeTomador = repLocalidade.BuscarPorDescricaoEUF(xDadosTomadorEndereco.Element("Cidade").Value, xDadosTomadorEndereco.Element("Estado").Value);
            if (localidadeTomador == null)
                return "Cidade do Tomador não localizada no sistema";
            nfseIntegracao.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                CPFCNPJ = xDadosTomador.Element("CNPJ").Value,
                RazaoSocial = xDadosTomador.Element("Nome").Value,

                Endereco = xDadosTomadorEndereco.Element("Logradouro").Value,
                Numero = xDadosTomadorEndereco.Element("Numero").Value,
                Complemento = xDadosTomadorEndereco.Element("Complemento").Value,
                Bairro = xDadosTomadorEndereco.Element("Bairro").Value,
                Cidade = xDadosTomadorEndereco.Element("Cidade").Value,
                CEP = xDadosTomadorEndereco.Element("CEP").Value,
                CodigoIBGECidade = localidadeTomador.CodigoIBGE
            };

            nfseIntegracao.DataEmissao = xDados.Element("DataEmissao").Value.ToNullableDateTime("yyyy-MM-ddTHH:mm:ss.fffffffzzz").Value.ToString("dd/MM/yyyy");
            nfseIntegracao.Numero = xDados.Element("Numero").Value.ToInt();
            nfseIntegracao.Serie = xDados.Element("Serie").Value.ToInt().ToString();
            nfseIntegracao.OutrasInformacoes = xDados.Element("InformacoesAdicionais").Value.Trim();

            nfseIntegracao.ValorISS = decimal.Parse(xDados.Element("ValorIss").Value, cultura);
            nfseIntegracao.ValorServicos = decimal.Parse(xDados.Element("Valor").Value, cultura);
            nfseIntegracao.BaseCalculoISS = decimal.Parse(xDados.Element("BaseCalculo").Value, cultura);
            nfseIntegracao.AliquotaISS = decimal.Parse(xDados.Element("Aliquota").Value, cultura);

            string descricaoAtividade = xDados.Element("DescricaoAtividade").Value.Trim().Substring(0, 5);
            Dominio.ObjetosDeValor.NFSe.Servico servico = new Dominio.ObjetosDeValor.NFSe.Servico();
            servico.Aliquota = nfseIntegracao.AliquotaISS;
            servico.Descricao = xDados.Element("DescricaoServicos").Value.Replace("'", "").Trim();
            servico.CodigoTributacao = Utilidades.String.OnlyNumbers(descricaoAtividade);
            servico.Numero = Utilidades.String.OnlyNumbers(descricaoAtividade);

            nfseIntegracao.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>
            {
                new Dominio.ObjetosDeValor.NFSe.Item
                {
                    BaseCalculoISS = nfseIntegracao.BaseCalculoISS,
                    AliquotaISS = nfseIntegracao.AliquotaISS,
                    ValorISS = nfseIntegracao.ValorISS,
                    ValorServico = nfseIntegracao.ValorServicos,
                    ValorTotal = nfseIntegracao.ValorServicos,

                    Discriminacao = servico.Descricao,
                    Servico = servico,
                    Quantidade = 1,
                    ServicoPrestadoNoPais = true,
                    ExigibilidadeISS = 1,
                    CodigoIBGECidade = empresa.Localidade.CodigoIBGE,
                    CodigoIBGECidadeIncidencia = localidadeTomador.CodigoIBGE,
                    CodigoPaisPrestacaoServico = 1058
                }
            };

            if (xIBSCBS != null)
            {
                nfseIntegracao.IBSCBS = new IBSCBS
                {
                    AliquotaCBS = decimal.Parse(xIBSCBS.Element("AliquotaCBS").Value, cultura),
                    AliquotaIBSEstadual = decimal.Parse(xIBSCBS.Element("AliquotaIBSEstadual").Value, cultura),
                    AliquotaIBSMunicipal = decimal.Parse(xIBSCBS.Element("AliquotaIBSMunicipal").Value, cultura),
                    BaseCalculoIBSCBS = decimal.Parse(xIBSCBS.Element("BaseCalculoIBSCBS").Value, cultura),
                    ClassificacaoTributariaIBSCBS = xIBSCBS.Element("ClassificacaoTributariaIBSCBS").Value,
                    CodigoIndicadorOperacao = xIBSCBS.Element("CodigoIndicadorOperacao").Value,
                    CSTIBSCBS = xIBSCBS.Element("CSTIBSCBS").Value,
                    NBS = xIBSCBS.Element("NBS").Value,
                    PercentualReducaoCBS = decimal.Parse(xIBSCBS.Element("PercentualReducaoCBS").Value, cultura),
                    PercentualReducaoIBSEstadual = decimal.Parse(xIBSCBS.Element("PercentualReducaoIBSEstadual").Value, cultura),
                    PercentualReducaoIBSMunicipal = decimal.Parse(xIBSCBS.Element("PercentualReducaoIBSMunicipal").Value, cultura),
                    ValorCBS = decimal.Parse(xIBSCBS.Element("ValorCBS").Value, cultura),
                    ValorIBSEstadual = decimal.Parse(xIBSCBS.Element("ValorIBSEstadual").Value, cultura),
                    ValorIBSMunicipal = decimal.Parse(xIBSCBS.Element("ValorIBSMunicipal").Value, cultura),
                };

            }

            Dominio.Entidades.NaturezaNFSe natureza = null;
            if (empresa.Localidade.CodigoIBGE != localidadeTomador.CodigoIBGE)
                natureza = empresa.Configuracao?.NaturezaNFSeFora;
            else
                natureza = empresa.Configuracao?.NaturezaNFSe;

            if (natureza == null)
                return "Natureza de Operação não configurada na empresa";
            nfseIntegracao.Natureza = new Dominio.ObjetosDeValor.NFSe.Natureza()
            {
                Numero = natureza.Numero,
                Descricao = natureza.Descricao
            };

            nfseIntegracao.SerieRPS = xDados.Element("Serie").Value;
            nfseIntegracao.DataEmissaoRPS = nfseIntegracao.DataEmissao;
            nfseIntegracao.Status = "A";
            nfseIntegracao.CodigoIBGECidadePrestacaoServico = empresa.Localidade.CodigoIBGE;
            nfseIntegracao.Ambiente = empresa.TipoAmbiente;

            return nfseIntegracao;
        }

        #endregion

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo ObterConfiguracaoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            if (_configuracaoArquivo == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                _configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();
                return _configuracaoArquivo;
            }

            return _configuracaoArquivo;
        }

        #endregion
    }
}
