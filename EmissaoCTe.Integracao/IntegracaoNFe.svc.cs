using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace EmissaoCTe.Integracao
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "IntegracaoNFe" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select IntegracaoNFe.svc or IntegracaoNFe.svc.cs at the Solution Explorer and start debugging.
    public class IntegracaoNFe : IIntegracaoNFe
    {
        public Retorno<List<RetornoImpressora>> ConsultarImpressora(int numeroUnidade, string status, string nomeImpressora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (token == "")
                    token = null;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(1);

                if (empresa != null)
                {
                    if (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<List<RetornoImpressora>>() { Mensagem = "Token de acesso inválido.", Status = false };
                }

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unidadeDeTrabalho);

                List<Dominio.Entidades.Impressora> listaImpressoras = repImpressora.Buscar(numeroUnidade, status, nomeImpressora, "N");

                if (listaImpressoras == null || listaImpressoras.Count == 0)
                    return new Retorno<List<RetornoImpressora>>() { Mensagem = "Nenhuma Impressora localizada.", Status = false };

                Retorno<List<RetornoImpressora>> retorno = new Retorno<List<RetornoImpressora>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoImpressora> dadosRetorno = new List<RetornoImpressora>();

                foreach (Dominio.Entidades.Impressora impressora in listaImpressoras)
                {
                    RetornoImpressora retornoImpressora = new RetornoImpressora()
                    {
                        Codigo = impressora.Codigo,
                        NumeroUnidade = impressora.NumeroDaUnidade,
                        NomeImpressora = impressora.NomeImpressora,
                        Status = impressora.Status
                    };

                    dadosRetorno.Add(retornoImpressora);
                }

                retorno.Objeto = dadosRetorno;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("ConsultarImpressora: " + ex);
                return new Retorno<List<RetornoImpressora>>() { Mensagem = "Ocorreu uma falha ao consultar impressora.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoNFe>> BuscarNFeImpressao(int codigoEmpresaPai, string numeroCarga, int numeroUnidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe repCargaImpressaoNFe = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (token == "")
                    token = null;

                if (empresaPai == null)
                    return new Retorno<List<RetornoNFe>>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<RetornoNFe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe> listaCargaImpressaoNFe = repCargaImpressaoNFe.Buscar(numeroUnidade, numeroCarga, situacao);

                Retorno<List<RetornoNFe>> retorno = new Retorno<List<RetornoNFe>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoNFe> dadosRetorno = new List<RetornoNFe>();

                foreach (Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe cargaImpressaoNFe in listaCargaImpressaoNFe)
                {
                    RetornoNFe retornoNFSe = new RetornoNFe()
                    {
                        Codigo = cargaImpressaoNFe.Codigo,
                        CargaPedido = cargaImpressaoNFe.CargaPedido != null ? cargaImpressaoNFe.CargaPedido.Codigo : 0,
                        NumeroNFe = cargaImpressaoNFe.Numero,
                        SerieNFe = cargaImpressaoNFe.Serie,
                        ChaveNFe = cargaImpressaoNFe.Chave,
                        XML = cargaImpressaoNFe.XML
                    };

                    dadosRetorno.Add(retornoNFSe);
                }

                retorno.Objeto = dadosRetorno;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("BuscarNFeImpressao: " + ex);
                return new Retorno<List<RetornoNFe>>() { Mensagem = "Ocorreu uma falha ao obter os dados das NFe para Impressão.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>> BuscarBoletoImpressao(int codigoEmpresaPai, string numeroCarga, int numeroUnidade, int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto repCargaImpressaoBoleto = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (token == "")
                    token = null;

                if (empresaPai == null)
                    return new Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>>() { Mensagem = "Token de acesso inválido.", Status = false };

                List<int> listaPedidos = repPedido.BuscarCodigosPedidosPorCargaPedido(cargaPedido);

                if (listaPedidos == null || listaPedidos.Count == 0)
                    return new Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>>() { Mensagem = "Nenhum pedido encontrado para solicitar impressão de boleto", Status = false };

                //Alterado para buscar os boletos através da lista de pedidos, caso da Casaredo onde o boleto esta associado a outra carga_pedido com mesmo pedido
                List<Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto> listaCargaImpressaoBoleto = repCargaImpressaoBoleto.Buscar(numeroUnidade, numeroCarga, listaPedidos, situacao);

                Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto> dadosRetorno = new List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>();

                foreach (Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto cargaImpressaoBoleto in listaCargaImpressaoBoleto)
                {
                    Dominio.ObjetosDeValor.WebService.Impressao.Boleto retornoMDFe = new Dominio.ObjetosDeValor.WebService.Impressao.Boleto()
                    {
                        Codigo = cargaImpressaoBoleto.Codigo,
                        CodigoBanco = cargaImpressaoBoleto.CodigoBanco,
                        NomeBanco = cargaImpressaoBoleto.NomeBanco,
                        Agencia = cargaImpressaoBoleto.Agencia,
                        NossoNumero = cargaImpressaoBoleto.NossoNumero,
                        DataVencimento = cargaImpressaoBoleto.DataVencimento,
                        LinhaDigitavel = cargaImpressaoBoleto.LinhaDigitavel,
                        CodigoBarras = cargaImpressaoBoleto.CodigoBarras,
                        NumeroDocumento = cargaImpressaoBoleto.NumeroDocumento,
                        DataDocumento = cargaImpressaoBoleto.DataDocumento,
                        LocalPagamento = cargaImpressaoBoleto.LocalPagamento,
                        EspecieDocumento = cargaImpressaoBoleto.EspecieDocumento,
                        Aceite = cargaImpressaoBoleto.Aceite,
                        DataProcessamento = cargaImpressaoBoleto.DataProcessamento,
                        UsoBanco = cargaImpressaoBoleto.UsoBanco,
                        Carteira = cargaImpressaoBoleto.Carteira,
                        EspecieMoeda = cargaImpressaoBoleto.EspecieMoeda,
                        Quantidade = cargaImpressaoBoleto.Quantidade,
                        Instrucoes = cargaImpressaoBoleto.Instrucoes,
                        InstucoesAdicionais = cargaImpressaoBoleto.InstrucoesAdicional,
                        SacadoCNPJ = cargaImpressaoBoleto.SacadoCNPJ,
                        SacadoIE = cargaImpressaoBoleto.SacadoIE,
                        SacadoNome = cargaImpressaoBoleto.SacadoNome,
                        SacadoRua = cargaImpressaoBoleto.SacadoRua,
                        SacadoNumero = cargaImpressaoBoleto.SacadoNumero,
                        SacadoComplemento = cargaImpressaoBoleto.SacadoComplemento,
                        SacadoCEP = cargaImpressaoBoleto.SacadoCEP,
                        SacadoBairro = cargaImpressaoBoleto.SacadoBairro,
                        SacadoCidade = cargaImpressaoBoleto.SacadoCidade,
                        SacadoEstado = cargaImpressaoBoleto.SacadoEstado,
                        CedendeCodigo = cargaImpressaoBoleto.CedenteCodigo,
                        CedenteCNPJ = cargaImpressaoBoleto.CedenteCNPJ,
                        CedenteIE = cargaImpressaoBoleto.CedenteIE,
                        CedenteNome = cargaImpressaoBoleto.CedenteNome,
                        CedenteRua = cargaImpressaoBoleto.CedenteRua,
                        CedenteNumero = cargaImpressaoBoleto.CedenteNumero,
                        CedenteComplemento = cargaImpressaoBoleto.CedenteComplemento,
                        CedenteCEP = cargaImpressaoBoleto.CedenteCEP,
                        CedenteBairro = cargaImpressaoBoleto.CedenteBairro,
                        CedenteCidade = cargaImpressaoBoleto.CedenteCidade,
                        CedenteEstado = cargaImpressaoBoleto.CedenteEstado,
                        ValorDocumeno = cargaImpressaoBoleto.ValorDocumento,
                        ValorDescontoAcrescimo = cargaImpressaoBoleto.ValorDescontoAcrescimo,
                        ValorCobrado = cargaImpressaoBoleto.ValorCobrado,
                        DigitoBanco = cargaImpressaoBoleto.DigitoBanco,
                        CIP = cargaImpressaoBoleto.CIP
                    };

                    dadosRetorno.Add(retornoMDFe);
                }

                retorno.Objeto = dadosRetorno;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("BuscarBoletoImpressao: " + ex);
                return new Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>>() { Mensagem = "Ocorreu uma falha ao obter os dados do boleto para Impressão.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<object> AlterarSituacao(int codigoEmpresaPai, int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string pdfNFe, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe repCargaImpressaoNFe = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (token == "")
                    token = null;

                if (empresaPai == null)
                    return new Retorno<object>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<object>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe cargaImpressaoNFe = repCargaImpressaoNFe.BuscarPorCodigo(codigo);

                if (cargaImpressaoNFe != null)
                {
                    string caminhoPDF = string.Empty;

                    if (!string.IsNullOrWhiteSpace(pdfNFe))
                    {
                        //Servicos.Log.TratarErro(pdfNFe);

                        string diretorio = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "DANFE");
                        if (!string.IsNullOrWhiteSpace(diretorio))
                        {
                            try
                            {
                                caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(diretorio, cargaImpressaoNFe.Chave + ".pdf");
                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                {
                                    byte[] decodedData = Convert.FromBase64String(pdfNFe);
                                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                                }
                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                    Servicos.Log.TratarErro("AlterarSituacao: Não foi possível salvar PDF da NFe " + cargaImpressaoNFe.Chave);

                            }
                            catch (Exception ex)
                            {
                                caminhoPDF = string.Empty;
                                Servicos.Log.TratarErro("AlterarSituacao: Falha ao salvar PDF NFe" + ex);
                            }
                        }
                        else
                            Servicos.Log.TratarErro("AlterarSituacao: Sem diretório configurado para salvar PDF da NFe " + cargaImpressaoNFe.Chave);
                    }

                    cargaImpressaoNFe.SituacaoImpressao = situacao;
                    cargaImpressaoNFe.CaminhoPDF = caminhoPDF;
                    repCargaImpressaoNFe.Atualizar(cargaImpressaoNFe);

                    return new Retorno<object>() { Mensagem = "Situação alterada com sucesso.", Status = true };
                }
                else
                    return new Retorno<object>() { Mensagem = "Carga Impressão NFe não encontrada para este codigo.", Status = false };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("AlterarSituacao: " + ex);
                return new Retorno<object>() { Mensagem = "Ocorreu uma falha ao salvar a integração.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<object> AlterarSituacaoBoleto(int codigoEmpresaPai, int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string pdfBoleto, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto repCargaImpressaoBoleto = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (token == "")
                    token = null;

                if (empresaPai == null)
                    return new Retorno<object>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<object>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto cargaImpressaoBoleto = repCargaImpressaoBoleto.BuscarPorCodigo(codigo);

                if (cargaImpressaoBoleto != null)
                {
                    string caminhoPDF = string.Empty;

                    if (!string.IsNullOrWhiteSpace(pdfBoleto))
                    {
                        //Servicos.Log.TratarErro(pdfBoleto);

                        string diretorio = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "BOLETOS");
                        if (!string.IsNullOrWhiteSpace(diretorio))
                        {
                            try
                            {
                                caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(diretorio, cargaImpressaoBoleto.NossoNumero + ".pdf");
                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                {
                                    byte[] decodedData = System.Text.Encoding.Default.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, System.Convert.FromBase64String(pdfBoleto))).ToArray();
                                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, decodedData);
                                }
                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                    Servicos.Log.TratarErro("AlterarSituacaoBoleto: Não foi possível salvar PDF boleto " + cargaImpressaoBoleto.NossoNumero);

                            }
                            catch (Exception ex)
                            {
                                caminhoPDF = string.Empty;
                                Servicos.Log.TratarErro("AlterarSituacaoBoleto: Falha ao salvar PDF boleto" + ex);
                            }
                        }
                        else
                            Servicos.Log.TratarErro("AlterarSituacaoBoleto: Sem diretório configurado para salvar PDF boleto " + cargaImpressaoBoleto.NossoNumero);
                    }

                    cargaImpressaoBoleto.CaminhoPDF = caminhoPDF;
                    cargaImpressaoBoleto.SituacaoImpressao = situacao;
                    repCargaImpressaoBoleto.Atualizar(cargaImpressaoBoleto);

                    return new Retorno<object>() { Mensagem = "Situação alterada com sucesso.", Status = true };
                }
                else
                    return new Retorno<object>() { Mensagem = "Carga Impressão Boleto não encontrada para este codigo.", Status = false };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("AlterarSituacaoBoleto: " + ex);
                return new Retorno<object>() { Mensagem = "Ocorreu uma falha ao salvar a integração.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<string> EnviarArquivoXMLNFeParaAverbacao(Stream arquivo)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.AverbacaoNFe repAverbacaoNFe = new Repositorio.AverbacaoNFe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                Retorno<string> retorno = new Retorno<string>();
                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosXMLNFeAverbacao"];

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, string.Concat(nomeArquivo, ".xml"));

                StreamReader readerSalvar = new StreamReader(arquivo);

                string conteudoArquivo = readerSalvar.ReadToEnd();

                Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, RemoveTroublesomeCharacters(conteudoArquivo), Encoding.UTF8);

                arquivo.Close();
                arquivo.Dispose();
                readerSalvar.Dispose();
                readerSalvar.Close();

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                {
                    System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                    Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

                    try
                    {
                        nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork);
                    }
                    catch (Exception)
                    {
                        retorno.Mensagem = "O xml enviado não é de uma nota fiscal autorizada.";
                        retorno.Status = false;
                        Servicos.Log.TratarErro("O xml enviado não é de uma nota fiscal autorizada, avulsa.");
                        reader.Dispose();
                    }

                    if (nfXml != null)
                    {
                        Servicos.Log.TratarErro("EnviarArquivoXMLNFeParaAverbacao - Chave: " + nfXml.Chave);

                        if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                        {
                            unitOfWork.Rollback();

                            retorno.Status = false;
                            retorno.Mensagem += erro;

                            return retorno;
                        }


                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFisca.BuscarPorChave(xmlNotaFiscal.Chave);

                        if (xmlNotaFiscalExiste != null)
                        {
                            unitOfWork.Rollback();

                            retorno.Status = true;
                            retorno.Mensagem = "NFe já recebida com sucesso " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            retorno.Objeto = nomeArquivo;

                            Dominio.Entidades.AverbacaoNFe averbacaoExistente = repAverbacaoNFe.BuscarPorChaveNFe(xmlNotaFiscal.Chave);
                            if (averbacaoExistente == null || (averbacaoExistente.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso && averbacaoExistente.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado))
                            {
                                bool jaExistente = true;
                                if (averbacaoExistente == null)
                                {
                                    averbacaoExistente = new Dominio.Entidades.AverbacaoNFe();
                                    averbacaoExistente.XMLNotaFiscal = xmlNotaFiscal;
                                    averbacaoExistente.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

                                    jaExistente = false;
                                }

                                averbacaoExistente.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                                averbacaoExistente.CodigoUsuario = empresaPai.Configuracao.CodigoSeguroATM;
                                averbacaoExistente.Usuario = empresaPai.Configuracao.UsuarioSeguroATM;
                                averbacaoExistente.Senha = empresaPai.Configuracao.SenhaSeguroATM;

                                if (jaExistente)
                                    repAverbacaoNFe.Atualizar(averbacaoExistente);
                                else
                                    repAverbacaoNFe.Inserir(averbacaoExistente);

                                Servicos.Embarcador.Integracao.ATM.ATMIntegracao.AverbarNFe(averbacaoExistente, unitOfWork);
                            }

                            //Buscar se já existe uma averbação e se não for sucesso reenvia-la.

                            return retorno;
                        }

                        xmlNotaFiscal.SemCarga = true;

                        xmlNotaFiscal.TipoNotaFiscalIntegrada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.Faturamento;
                        repXMLNotaFisca.Inserir(xmlNotaFiscal);

                        unitOfWork.CommitChanges();

                        Dominio.Entidades.AverbacaoNFe averbacao = new Dominio.Entidades.AverbacaoNFe();
                        averbacao.XMLNotaFiscal = xmlNotaFiscal;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                        averbacao.CodigoUsuario = empresaPai.Configuracao.CodigoSeguroATM;
                        averbacao.Usuario = empresaPai.Configuracao.UsuarioSeguroATM;
                        averbacao.Senha = empresaPai.Configuracao.SenhaSeguroATM;

                        repAverbacaoNFe.Inserir(averbacao);

                        Servicos.Embarcador.Integracao.ATM.ATMIntegracao.AverbarNFe(averbacao, unitOfWork);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                    }

                    if (retorno.Status)
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                }
                else
                {
                    retorno.Objeto = "";
                    retorno.Status = false;
                    retorno.Mensagem += "Não foi possível ler XML, verifique e envie novamente";

                    return retorno;
                }

                retorno.Status = true;
                retorno.Mensagem = "NFe recebida com sucesso " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Objeto = nomeArquivo;

                Servicos.Log.TratarErro("EnviarArquivoXMLNFeParaAverbacao: " + nomeArquivo);

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao receber o arquivo.", Status = false };
            }
        }

        public Retorno<string> EnviarStringXMLNFeParaAverbacao(string xmlNFe, string token)
        {
            ValidarToken(token);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.AverbacaoNFe repAverbacaoNFe = new Repositorio.AverbacaoNFe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
             
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();



                Retorno<string> retorno = new Retorno<string>();
                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosXMLNFeAverbacao"];

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, string.Concat(nomeArquivo, ".xml"));

                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                byte[] decodedData = encoding.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, encoding, Convert.FromBase64String(xmlNFe))).ToArray();
                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, decodedData);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                {
                    System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                    Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

                    try
                    {
                        nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork);
                    }
                    catch (Exception)
                    {
                        retorno.Mensagem = "O xml enviado não é de uma nota fiscal autorizada.";
                        retorno.Status = false;
                        Servicos.Log.TratarErro("O xml enviado não é de uma nota fiscal autorizada, avulsa.");
                        reader.Dispose();
                    }

                    if (nfXml != null)
                    {
                        Servicos.Log.TratarErro("EnviarStringXMLNFeParaAverbacao - Chave: " + nfXml.Chave);

                        if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                        {
                            unitOfWork.Rollback();

                            retorno.Status = false;
                            retorno.Mensagem += erro;

                            return retorno;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFisca.BuscarPorChave(xmlNotaFiscal.Chave);

                        if (xmlNotaFiscalExiste != null)
                        {
                            unitOfWork.Rollback();

                            retorno.Status = true;
                            retorno.Mensagem = "NFe já recebida com sucesso " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            retorno.Objeto = nomeArquivo;

                            Dominio.Entidades.AverbacaoNFe averbacaoExistente = repAverbacaoNFe.BuscarPorChaveNFe(xmlNotaFiscal.Chave);
                            if (averbacaoExistente == null || (averbacaoExistente.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso && averbacaoExistente.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado))
                            {
                                bool jaExistente = true;
                                if (averbacaoExistente == null)
                                {
                                    averbacaoExistente = new Dominio.Entidades.AverbacaoNFe();
                                    averbacaoExistente.XMLNotaFiscal = xmlNotaFiscal;
                                    averbacaoExistente.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

                                    jaExistente = false;
                                }

                                averbacaoExistente.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                                averbacaoExistente.CodigoUsuario = empresaPai.Configuracao.CodigoSeguroATM;
                                averbacaoExistente.Usuario = empresaPai.Configuracao.UsuarioSeguroATM;
                                averbacaoExistente.Senha = empresaPai.Configuracao.SenhaSeguroATM;

                                if (jaExistente)
                                    repAverbacaoNFe.Atualizar(averbacaoExistente);
                                else
                                    repAverbacaoNFe.Inserir(averbacaoExistente);

                                Servicos.Embarcador.Integracao.ATM.ATMIntegracao.AverbarNFe(averbacaoExistente, unitOfWork);
                            }

                            //Buscar se já existe uma averbação e se não for sucesso reenvia-la.

                            return retorno;
                        }

                        xmlNotaFiscal.SemCarga = true;

                        xmlNotaFiscal.TipoNotaFiscalIntegrada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.Faturamento;
                        repXMLNotaFisca.Inserir(xmlNotaFiscal);

                        unitOfWork.CommitChanges();

                        Dominio.Entidades.AverbacaoNFe averbacao = new Dominio.Entidades.AverbacaoNFe();
                        averbacao.XMLNotaFiscal = xmlNotaFiscal;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                        averbacao.CodigoUsuario = empresaPai.Configuracao.CodigoSeguroATM;
                        averbacao.Usuario = empresaPai.Configuracao.UsuarioSeguroATM;
                        averbacao.Senha = empresaPai.Configuracao.SenhaSeguroATM;

                        repAverbacaoNFe.Inserir(averbacao);

                        Servicos.Embarcador.Integracao.ATM.ATMIntegracao.AverbarNFe(averbacao, unitOfWork);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                    }

                    if (retorno.Status)
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                }
                else
                {
                    retorno.Objeto = "";
                    retorno.Status = false;
                    retorno.Mensagem += "Não foi possível ler XML, verifique e envie novamente";

                    return retorno;
                }

                retorno.Status = true;
                retorno.Mensagem = "NFe recebida com sucesso " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Objeto = nomeArquivo;

                Servicos.Log.TratarErro("EnviarStringXMLNFeParaAverbacao: " + nomeArquivo);

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao receber o arquivo.", Status = false };
            }
        }


        private static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }

        private void ValidarToken(string token = null)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(
                Conexao.StringConexao,
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                OperationContext context = OperationContext.Current;
                MessageProperties properties = context.IncomingMessageProperties;
                MessageHeaders headers = context.IncomingMessageHeaders;

                if (string.IsNullOrWhiteSpace(token))
                {
                    if (headers.FindHeader("Token", "Token") == -1)
                        throw new FaultException("Token inválido. Adicione a tag do token no header (cabeçalho) da requisição, conforme exemplo: <Token xmlns='Token'>seu token</Token>");

                    token = Convert.ToString(headers.GetHeader<string>("Token", "Token"));
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(1);

                if (empresa != null)
                {
                    if (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token)
                    {
                        Servicos.Log.TratarErro("Token " + token + " inválido.");

                        throw new FaultException("Token inválido. Verifique se o token informado é o mesmo autorizado para a integração.");
                    }
                }
            }
        }
    }
}
