using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EmissaoCTe.Integracao
{
    public class IntegracaoCTe : IIntegracaoCTe
    {
        #region Métodos Públicos

        public Retorno<List<RetornoCTe>> Buscar(Dominio.Enumeradores.StatusIntegracao statusIntegracao, int codigoEmpresaPai, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (token == "")
                    token = null;

                if (empresaPai == null)
                    return new Retorno<List<RetornoCTe>>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && token != empresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<List<RetornoCTe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (ConfigurationManager.AppSettings["ControlarConsultasImpressao"] == "SIM")
                {
                    if (!Servicos.StatusConsultaImpressaoUnidade.VerificarStatusConsultaImpressaoUnidade(numeroUnidade, Dominio.Enumeradores.TipoObjetoConsulta.CTe, true, unidadeDeTrabalho))
                    {
                        System.Threading.Thread.Sleep(5000);
                        return new Retorno<List<RetornoCTe>>() { Mensagem = "Outra consulta esta em andamento para mesma Unidade e Documento.", Status = false };
                    }
                }

                int.TryParse(ConfigurationManager.AppSettings["RegistrosImpressaoCTe"], out int registrosImpressao);

                if (registrosImpressao == 0)
                    registrosImpressao = 25;

                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoCTe> listaIntegracoes = repIntegracao.Buscar(codigoEmpresaPai, numeroCarga, numeroUnidade, registrosImpressao, statusIntegracao);

                Retorno<List<RetornoCTe>> retorno = new Retorno<List<RetornoCTe>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoCTe> dadosRetorno = new List<RetornoCTe>();

                foreach (Dominio.Entidades.IntegracaoCTe integracao in listaIntegracoes)
                {
                    RetornoCTe retCTe = new RetornoCTe()
                    {
                        CodigoCTe = integracao.CTe.Codigo,
                        ChaveCTe = integracao.CTe.Chave,
                        CNPJEmpresa = integracao.CTe.Empresa.CNPJ,
                        Tipo = integracao.Tipo,
                        NomeArquivo = integracao.NomeArquivo,
                        StatusCTe = integracao.CTe.Status,
                        SerieCTe = integracao.CTe.Serie.Numero,
                        DataEmissao = integracao.CTe.DataEmissao.HasValue ? integracao.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        MensagemRetorno = integracao.CTe.MensagemStatus != null ? integracao.CTe.MensagemStatus.MensagemDoErro : integracao.CTe.MensagemRetornoSefaz,
                        NumeroCTe = integracao.CTe.Numero,
                        NumeroProtocolo = integracao.CTe.Protocolo,
                        NumeroRecibo = integracao.CTe.NumeroRecibo,
                        Ambiente = integracao.CTe.TipoAmbiente,
                        Arquivo = integracao.CTe.Status == "R" ? integracao.Arquivo : string.Empty,
                        XML = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? ObterRetornoXML(integracao, Dominio.Enumeradores.TipoIntegracao.Todos, unidadeDeTrabalho) : string.Empty),
                        PDF = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoPDF(integracao, unidadeDeTrabalho) : string.Empty),
                        CONEMB = integracao.CTe.Documentos.Count > 0 ? ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.CONEMB) ? this.ObterRetornoCONEMB(integracao, unidadeDeTrabalho) : string.Empty) : string.Empty,
                        TXT = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.TXT) ? this.ObterRetornoTxt(integracao, unidadeDeTrabalho) : string.Empty)
                    };

                    if (!dadosRetorno.Any(o => o.CodigoCTe.Equals(retCTe.CodigoCTe)))
                        dadosRetorno.Add(retCTe);
                }

                retorno.Objeto = dadosRetorno;

                if (ConfigurationManager.AppSettings["ControlarConsultasImpressao"] == "SIM")
                    Servicos.StatusConsultaImpressaoUnidade.VerificarStatusConsultaImpressaoUnidade(numeroUnidade, Dominio.Enumeradores.TipoObjetoConsulta.CTe, false, unidadeDeTrabalho);

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoCTe>>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoCTe>> BuscarPorCodigoMDFe(int codigoMDFe, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Retorno<List<RetornoCTe>> retorno = new Retorno<List<RetornoCTe>> { Mensagem = "Retorno realizado com sucesso.", Status = true, Objeto = new List<RetornoCTe>() };

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentosMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new Retorno<List<RetornoCTe>>() { Mensagem = "MDF-e não encontrado.", Status = false };

                if (mdfe.Empresa.EmpresaPai != null && mdfe.Empresa.EmpresaPai.Configuracao != null && token != mdfe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<List<RetornoCTe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                List<int> codigosCTes = repDocumentosMDFe.BuscarCodigosDeCTesPorMDFe(codigoMDFe);

                foreach (int codigoCTe in codigosCTes)
                {
                    List<Dominio.Entidades.IntegracaoCTe> integracoes = repIntegracaoCTe.BuscarPorCTeETipo(codigoCTe, Dominio.Enumeradores.TipoIntegracao.Emissao);

                    if (integracoes.Count() > 0)
                    {
                        retorno.Objeto.Add(new RetornoCTe()
                        {
                            CodigoCTe = integracoes[0].CTe.Codigo,
                            NumeroCTe = integracoes[0].CTe.Numero,
                            DataEmissao = integracoes[0].CTe.DataEmissao.HasValue ? integracoes[0].CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                            SerieCTe = integracoes[0].CTe.Serie.Numero,
                            ChaveCTe = integracoes[0].CTe.Chave,
                            NumeroProtocolo = integracoes[0].CTe.Protocolo,
                            NumeroRecibo = integracoes[0].CTe.NumeroRecibo,
                            CNPJEmpresa = integracoes[0].CTe.Empresa.CNPJ,
                            Tipo = integracoes[0].Tipo,
                            NomeArquivo = integracoes[0].NomeArquivo,
                            StatusCTe = integracoes[0].CTe.Status,
                            Ambiente = integracoes[0].CTe.TipoAmbiente,
                            MensagemRetorno = integracoes[0].CTe.MensagemStatus != null ? integracoes[0].CTe.MensagemStatus.MensagemDoErro : integracoes[0].CTe.MensagemRetornoSefaz,
                            Arquivo = integracoes[0].CTe.Status == "R" ? integracoes[0].Arquivo : string.Empty,
                            XML = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoXML(integracoes[0], Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho) : string.Empty),
                            PDF = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoPDF(integracoes[0], unidadeDeTrabalho) : string.Empty),
                            CONEMB = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.CONEMB) ? this.ObterRetornoCONEMB(integracoes[0], unidadeDeTrabalho) : string.Empty),
                            TXT = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.TXT) ? this.ObterRetornoTxt(integracoes[0], unidadeDeTrabalho) : string.Empty)
                        });
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoCTe>>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<object> Alterar(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.StatusIntegracao statusIntegracao, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (token == "")
                    token = null;

                if (cte == null)
                    return new Retorno<object>() { Mensagem = "CT-e não encontrado.", Status = false };

                if (cte.Empresa.EmpresaPai.Configuracao != null && token != cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<object>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoCTe> integracoes = repIntegracao.BuscarPorCTeETipo(codigoCTe, tipoIntegracao);

                if (integracoes.Count() > 0)
                {

                    foreach (Dominio.Entidades.IntegracaoCTe integracao in integracoes)
                    {
                        integracao.Status = statusIntegracao;

                        repIntegracao.Atualizar(integracao);
                    }

                    return new Retorno<object>() { Mensagem = "Integração alterada com sucesso.", Status = true };
                }
                else
                {
                    return new Retorno<object>() { Mensagem = "Integração não encontrada para este CT-e.", Status = false };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<object>() { Mensagem = "Ocorreu uma falha ao salvar a integração.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<RetornoCTe> BuscarPorCodigoCTe(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token, string codificarUTF8)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                bool vCodificarUTF8 = false;
                if (string.IsNullOrWhiteSpace(codificarUTF8) || codificarUTF8 == "S")
                    vCodificarUTF8 = true;

                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoCTe> integracoes = repIntegracao.BuscarPorCTeETipo(codigoCTe, tipoIntegracao);

                Retorno<RetornoCTe> retorno = new Retorno<RetornoCTe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoCTe> dadosRetorno = new List<RetornoCTe>();

                if (integracoes.Count() > 0)
                {
                    if (integracoes[0].CTe.Empresa.EmpresaPai != null && integracoes[0].CTe.Empresa.EmpresaPai.Configuracao != null && token != integracoes[0].CTe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                        return new Retorno<RetornoCTe>() { Mensagem = "Token de acesso inválido.", Status = false };

                    retorno.Objeto = new RetornoCTe()
                    {
                        CodigoCTe = integracoes[0].CTe.Codigo,
                        NumeroCTe = integracoes[0].CTe.Numero,
                        DataEmissao = integracoes[0].CTe.DataEmissao.HasValue ? integracoes[0].CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        SerieCTe = integracoes[0].CTe.Serie.Numero,
                        ChaveCTe = integracoes[0].CTe.Status != "R" && integracoes[0].CTe.Status != "E" && integracoes[0].CTe.Status != "S" && integracoes[0].CTe.Status != "P" ? integracoes[0].CTe.Chave : string.Empty,
                        NumeroProtocolo = !String.IsNullOrWhiteSpace(integracoes[0].CTe.Protocolo) ? integracoes[0].CTe.Protocolo : string.Empty,
                        NumeroRecibo = !String.IsNullOrWhiteSpace(integracoes[0].CTe.NumeroRecibo) ? integracoes[0].CTe.NumeroRecibo : string.Empty,
                        CNPJEmpresa = integracoes[0].CTe.Empresa.CNPJ,
                        Tipo = integracoes[0].Tipo,
                        NomeArquivo = !String.IsNullOrWhiteSpace(integracoes[0].NomeArquivo) ? integracoes[0].NomeArquivo : string.Empty,
                        StatusCTe = integracoes[0].CTe.Status,
                        Ambiente = integracoes[0].CTe.TipoAmbiente > 0 ? integracoes[0].CTe.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Homologacao,
                        MensagemRetorno = integracoes[0].CTe.Status == "E" ? "CT-e em processamento." : integracoes[0].CTe.MensagemStatus != null ? integracoes[0].CTe.MensagemStatus.MensagemDoErro : !string.IsNullOrWhiteSpace(integracoes[0].CTe.MensagemRetornoSefaz) ? integracoes[0].CTe.MensagemRetornoSefaz : string.Empty,
                        Arquivo = integracoes[0].CTe.Status == "R" ? integracoes[0].Arquivo : string.Empty,
                        XML = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoXML(integracoes[0], tipoIntegracao, unidadeDeTrabalho) : string.Empty),
                        PDF = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoPDF(integracoes[0], unidadeDeTrabalho, vCodificarUTF8) : string.Empty),
                        CONEMB = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.CONEMB) ? this.ObterRetornoCONEMB(integracoes[0], unidadeDeTrabalho) : string.Empty),
                        TXT = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.TXT) ? this.ObterRetornoTxt(integracoes[0], unidadeDeTrabalho) : string.Empty),
                        PDFBIN = tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDFBIN ? this.ObterRetornoPDF(integracoes[0], unidadeDeTrabalho, vCodificarUTF8, true) : string.Empty,
                        ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                        {
                            Aliquota = integracoes[0].CTe.AliquotaICMS,
                            BaseCalculo = integracoes[0].CTe.BaseCalculoICMS,
                            CST = integracoes[0].CTe.CST == "91" ? "90" : !string.IsNullOrWhiteSpace(integracoes[0].CTe.CST) ? integracoes[0].CTe.CST : string.Empty,
                            PercentualReducaoBaseCalculo = integracoes[0].CTe.PercentualReducaoBaseCalculoICMS,
                            Valor = integracoes[0].CTe.ValorICMS,
                            ValorCreditoPresumido = integracoes[0].CTe.ValorPresumido,
                            ValorDevido = integracoes[0].CTe.ValorICMSDevido
                        },
                        JustificativaCancelamento = integracoes[0].CTe.Status == "C" ? integracoes[0].CTe.ObservacaoCancelamento : string.Empty,
                        CodigoRetorno = integracoes[0].CTe.MensagemStatus?.CodigoDoErro ?? 0
                    };
                }
                else
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                    if (cte != null && ((tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Cancelamento && cte.Status == "C") ||
                                                (tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Emissao && cte.Status == "A")))
                    {
                        if (cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                            return new Retorno<RetornoCTe>() { Mensagem = "Token de acesso inválido.", Status = false };

                        retorno.Objeto = new RetornoCTe()
                        {
                            CodigoCTe = cte.Codigo,
                            NumeroCTe = cte.Numero,
                            DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                            SerieCTe = cte.Serie.Numero,
                            ChaveCTe = cte.Status != "R" && cte.Status != "E" && cte.Status != "S" && cte.Status != "P" ? cte.Chave : string.Empty,
                            NumeroProtocolo = !String.IsNullOrWhiteSpace(cte.Protocolo) ? cte.Protocolo : string.Empty,
                            NumeroRecibo = !String.IsNullOrWhiteSpace(cte.NumeroRecibo) ? cte.NumeroRecibo : string.Empty,
                            CNPJEmpresa = cte.Empresa.CNPJ,
                            Tipo = cte.Status == "C" ? Dominio.Enumeradores.TipoIntegracao.Cancelamento : Dominio.Enumeradores.TipoIntegracao.Emissao,
                            NomeArquivo = string.Empty,
                            StatusCTe = cte.Status,
                            Ambiente = cte.TipoAmbiente,
                            MensagemRetorno = cte.MensagemStatus != null ? cte.MensagemStatus.MensagemDoErro : !String.IsNullOrWhiteSpace(cte.MensagemRetornoSefaz) ? cte.MensagemRetornoSefaz : string.Empty,
                            Arquivo = string.Empty,
                            XML = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterXMLCTE(cte, tipoIntegracao, unidadeDeTrabalho) : string.Empty),
                            PDF = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterPDFCTe(cte, tipoIntegracao, unidadeDeTrabalho, vCodificarUTF8) : string.Empty),
                            CONEMB = string.Empty,
                            TXT = string.Empty,
                            ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                            {
                                Aliquota = cte.AliquotaICMS,
                                BaseCalculo = cte.BaseCalculoICMS,
                                CST = cte.CST == "91" ? "90" : !string.IsNullOrWhiteSpace(cte.CST) ? cte.CST : string.Empty,
                                PercentualReducaoBaseCalculo = cte.PercentualReducaoBaseCalculoICMS,
                                Valor = cte.ValorICMS,
                                ValorCreditoPresumido = cte.ValorPresumido,
                                ValorDevido = cte.ValorICMSDevido
                            },
                            JustificativaCancelamento = cte.Status == "C" ? cte.ObservacaoCancelamento : string.Empty
                        };
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoCTe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        public Retorno<RetornoCTeMDFe> BuscarCTeMDFePorCodigoCTe(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token, string codificarUTF8)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                bool vCodificarUTF8 = false;
                if (string.IsNullOrWhiteSpace(codificarUTF8) || codificarUTF8 == "S")
                    vCodificarUTF8 = true;

                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repCTeMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoCTe> integracoes = repIntegracao.BuscarPorCTeETipo(codigoCTe, tipoIntegracao);

                Retorno<RetornoCTeMDFe> retorno = new Retorno<RetornoCTeMDFe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoCTeMDFe> dadosRetorno = new List<RetornoCTeMDFe>();

                if (integracoes.Count() > 0)
                {
                    if (integracoes[0].CTe.Empresa.EmpresaPai != null && integracoes[0].CTe.Empresa.EmpresaPai.Configuracao != null && token != integracoes[0].CTe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                        return new Retorno<RetornoCTeMDFe>() { Mensagem = "Token de acesso inválido.", Status = false };

                    List<int> listaCodigoMDFe = repCTeMDFe.BuscarCodigoDeMDFesPorCTe(codigoCTe);
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

                    if (listaCodigoMDFe.Count > 0)
                        mdfe = repMDFe.BuscarPorCodigo(listaCodigoMDFe.FirstOrDefault(), integracoes[0].CTe.Empresa.Codigo, Dominio.Enumeradores.StatusMDFe.Autorizado);

                    RetornoMDFe retornoMDFe = null;

                    if (mdfe != null)
                    {
                        retornoMDFe = new RetornoMDFe()
                        {
                            CodigoMDFe = mdfe.Codigo,
                            ChaveMDFe = mdfe.Chave,
                            CNPJEmpresa = mdfe.Empresa.CNPJ,
                            Tipo = Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao,
                            NomeArquivo = string.Empty,
                            Arquivo = string.Empty,
                            StatusMDFe = mdfe.Status,
                            DataEmissao = mdfe.DataEmissao.HasValue ? mdfe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                            NumeroMDFe = mdfe.Numero,
                            SerieMDFe = mdfe.Serie.Numero,
                            MensagemRetorno = mdfe.MensagemStatus != null ? string.Concat(mdfe.MensagemStatus.CodigoDoErro, " - ", mdfe.MensagemStatus.MensagemDoErro) : mdfe.MensagemRetornoSefaz
                        };

                        this.ObterArquivosRetornoMDFe(ref retornoMDFe, mdfe, tipoRetorno, unidadeDeTrabalho);
                    }

                    retorno.Objeto = new RetornoCTeMDFe()
                    {
                        CodigoCTe = integracoes[0].CTe.Codigo,
                        NumeroCTe = integracoes[0].CTe.Numero,
                        DataEmissao = integracoes[0].CTe.DataEmissao.HasValue ? integracoes[0].CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        SerieCTe = integracoes[0].CTe.Serie.Numero,
                        ChaveCTe = integracoes[0].CTe.Chave,
                        NumeroProtocolo = integracoes[0].CTe.Protocolo,
                        NumeroRecibo = integracoes[0].CTe.NumeroRecibo,
                        CNPJEmpresa = integracoes[0].CTe.Empresa.CNPJ,
                        Tipo = integracoes[0].Tipo,
                        NomeArquivo = integracoes[0].NomeArquivo,
                        StatusCTe = integracoes[0].CTe.Status,
                        Ambiente = integracoes[0].CTe.TipoAmbiente,
                        MensagemRetorno = integracoes[0].CTe.MensagemStatus != null ? integracoes[0].CTe.MensagemStatus.MensagemDoErro : integracoes[0].CTe.MensagemRetornoSefaz,
                        Arquivo = integracoes[0].CTe.Status == "R" ? integracoes[0].Arquivo : string.Empty,
                        XML = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoXML(integracoes[0], tipoIntegracao, unidadeDeTrabalho) : string.Empty),
                        PDF = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoPDF(integracoes[0], unidadeDeTrabalho, vCodificarUTF8) : string.Empty),
                        CONEMB = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.CONEMB) ? this.ObterRetornoCONEMB(integracoes[0], unidadeDeTrabalho) : string.Empty),
                        TXT = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.TXT) ? this.ObterRetornoTxt(integracoes[0], unidadeDeTrabalho) : string.Empty),
                        ICMS = new Dominio.ObjetosDeValor.CTe.ImpostoICMS()
                        {
                            Aliquota = integracoes[0].CTe.AliquotaICMS,
                            BaseCalculo = integracoes[0].CTe.BaseCalculoICMS,
                            CST = integracoes[0].CTe.CST == "91" ? "90" : integracoes[0].CTe.CST,
                            PercentualReducaoBaseCalculo = integracoes[0].CTe.PercentualReducaoBaseCalculoICMS,
                            Valor = integracoes[0].CTe.ValorICMS,
                            ValorCreditoPresumido = integracoes[0].CTe.ValorPresumido,
                            ValorDevido = integracoes[0].CTe.ValorICMSDevido
                        },
                        JustificativaCancelamento = integracoes[0].CTe.Status == "C" ? integracoes[0].CTe.ObservacaoCancelamento : string.Empty,
                        MDFe = retornoMDFe != null ? retornoMDFe : null
                    };
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoCTeMDFe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        public Retorno<RetornoCTe> BuscarPorNumeroDaNota(string cnpjEmpresaEmitente, string numeroNotaFiscal, string serieNotaFiscal, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresaEmitente);

                if (empresa == null)
                    return new Retorno<RetornoCTe>() { Mensagem = "Empresa emitente não encontrada.", Status = false };

                if (empresa.EmpresaPai != null && empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<RetornoCTe>() { Mensagem = "Token de acesso inválido." };

                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);
                Dominio.Entidades.IntegracaoCTe integracao = repIntegracao.BuscarPorNumeroDaNotaFiscal(cnpjEmpresaEmitente, numeroNotaFiscal, serieNotaFiscal, tipoIntegracao);

                Retorno<RetornoCTe> retorno = new Retorno<RetornoCTe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoCTe> dadosRetorno = new List<RetornoCTe>();

                if (integracao != null)
                {
                    retorno.Objeto = new RetornoCTe()
                    {
                        CodigoCTe = integracao.CTe.Codigo,
                        NumeroCTe = integracao.CTe.Numero,
                        SerieCTe = integracao.CTe.Serie.Numero,
                        DataEmissao = integracao.CTe.DataEmissao.HasValue ? integracao.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        ChaveCTe = integracao.CTe.Chave,
                        NumeroProtocolo = integracao.CTe.Protocolo,
                        NumeroRecibo = integracao.CTe.NumeroRecibo,
                        CNPJEmpresa = integracao.CTe.Empresa.CNPJ,
                        Tipo = integracao.Tipo,
                        NomeArquivo = integracao.NomeArquivo,
                        StatusCTe = integracao.CTe.Status,
                        Ambiente = integracao.CTe.TipoAmbiente,
                        MensagemRetorno = integracao.CTe.MensagemStatus != null ? integracao.CTe.MensagemStatus.MensagemDoErro : integracao.CTe.MensagemRetornoSefaz,
                        Arquivo = integracao.CTe.Status == "R" ? integracao.Arquivo : string.Empty,
                        XML = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoXML(integracao, tipoIntegracao, unidadeDeTrabalho) : string.Empty),
                        PDF = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF) ? this.ObterRetornoPDF(integracao, unidadeDeTrabalho) : string.Empty),
                        CONEMB = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.CONEMB) ? this.ObterRetornoCONEMB(integracao, unidadeDeTrabalho) : string.Empty),
                        TXT = ((tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.TXT) ? this.ObterRetornoTxt(integracao, unidadeDeTrabalho) : string.Empty)
                    };
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoCTe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

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

                List<Dominio.Entidades.Impressora> listaImpressoras = repImpressora.Buscar(numeroUnidade, status, nomeImpressora, "C");

                if (listaImpressoras == null || listaImpressoras.Count == 0)
                    return new Retorno<List<RetornoImpressora>>() { Mensagem = "Nenhuma Impressora localizada.", Status = false };

                Retorno<List<RetornoImpressora>> retorno = new Retorno<List<RetornoImpressora>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoImpressora> dadosRetorno = new List<RetornoImpressora>();

                foreach (Dominio.Entidades.Impressora impressora in listaImpressoras)
                {
                    RetornoImpressora retornoMDFe = new RetornoImpressora()
                    {
                        Codigo = impressora.Codigo,
                        NumeroUnidade = impressora.NumeroDaUnidade,
                        NomeImpressora = impressora.NomeImpressora,
                        Status = impressora.Status
                    };

                    dadosRetorno.Add(retornoMDFe);
                }

                retorno.Objeto = dadosRetorno;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoImpressora>>() { Mensagem = "Ocorreu uma falha ao consultar impressora.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoDetalhesCTe>> BuscarDetalhesCTePorPeriodo(string dataInicial, string dataFinal, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dtInicial, dtFinal;
                DateTime.TryParseExact(dataInicial, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtInicial);
                DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFinal);

                if ((dtInicial == DateTime.MinValue || dtFinal == DateTime.MinValue))
                    return new Retorno<List<RetornoDetalhesCTe>>() { Mensagem = "Deve ser enviado uma Data Inicial e Final.", Status = false };

                if (dtInicial > DateTime.MinValue && dtFinal > DateTime.MinValue && (dtFinal - dtInicial).TotalDays > 5)
                    return new Retorno<List<RetornoDetalhesCTe>>() { Mensagem = "Período inválido (máximo permitido: 5 dias).", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

                if (empresaPai == null)
                    return new Retorno<List<RetornoDetalhesCTe>>() { Mensagem = "Não existe empresa pai configurada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<RetornoDetalhesCTe>>() { Mensagem = "Token invalido.", Status = false };

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.ValePedagioMDFe repValePedagioMDFe = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);
                Repositorio.InformacaoCargaCTE repInformacoesCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarCTesPorPeriodo(dtInicial, dtFinal);

                Retorno<List<RetornoDetalhesCTe>> retorno = new Retorno<List<RetornoDetalhesCTe>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoDetalhesCTe> dadosRetorno = new List<RetornoDetalhesCTe>();

                if (listaCTes.Count() > 0)
                {
                    for (var i = 0; i < listaCTes.Count; i++)
                    {
                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigoCTe(listaCTes[i].Codigo);
                        List<Dominio.Entidades.ValePedagioMDFe> valesPedagioMDFe = mdfe != null ? repValePedagioMDFe.BuscarPorMDFe(mdfe.Codigo) : null;

                        RetornoDetalhesCTe dadoRetorno = new RetornoDetalhesCTe();
                        dadoRetorno.NumeroCTe = listaCTes[i].Numero;
                        dadoRetorno.SerieCTe = listaCTes[i].Serie?.Numero ?? 0;
                        dadoRetorno.SituacaoCTe = listaCTes[i].DescricaoStatus;
                        dadoRetorno.ChaveCTe = listaCTes[i].Chave;
                        dadoRetorno.ChaveMDFe = mdfe?.Chave ?? string.Empty;
                        dadoRetorno.DataEmissao = listaCTes[i].DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
                        dadoRetorno.DataAutorizacao = listaCTes[i].DataRetornoSefaz.HasValue ? listaCTes[i].DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : listaCTes[i].DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
                        dadoRetorno.PesoCTe = repInformacoesCargaCTe.ObterPesoKg(listaCTes[i].Codigo);
                        dadoRetorno.ValorFrete = listaCTes[i].ValorAReceber;
                        dadoRetorno.ValorICMS = listaCTes[i].ValorICMS;
                        dadoRetorno.AliquotaICMS = listaCTes[i].AliquotaICMS;
                        dadoRetorno.ValorPedagioMDFe = valesPedagioMDFe != null && valesPedagioMDFe.Count > 0 ? (from o in valesPedagioMDFe select o.ValorValePedagio).Sum() : 0;


                        dadosRetorno.Add(dadoRetorno);
                    }
                    retorno.Objeto = dadosRetorno;
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoDetalhesCTe>>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoNFe>> BuscarNFeImpressaoPorCTe(int codigoEmpresaPai, int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (token == "")
                    token = null;

                if (empresaPai == null)
                    return new Retorno<List<RetornoNFe>>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<RetornoNFe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                if (cte == null)
                    return new Retorno<List<RetornoNFe>>() { Mensagem = "CTe codigo " + codigoCTe.ToString() + " não localizado.", Status = false };

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDoCTe = repXMLNotaFiscal.BuscarPorCTe(cte);

                if (notasFiscaisDoCTe == null || notasFiscaisDoCTe.Count == 0)
                    return new Retorno<List<RetornoNFe>>() { Mensagem = "Nenhum XML de nota fiscal para o CTe codigo " + codigoCTe.ToString() + ".", Status = false };

                Retorno<List<RetornoNFe>> retorno = new Retorno<List<RetornoNFe>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoNFe> dadosRetorno = new List<RetornoNFe>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscaisDoCTe)
                {
                    RetornoNFe retornoNFSe = new RetornoNFe()
                    {
                        Codigo = notaFiscal.Codigo,
                        CargaPedido = 0,
                        NumeroNFe = notaFiscal.Numero,
                        SerieNFe = notaFiscal.Serie,
                        ChaveNFe = notaFiscal.Chave,
                        XML = notaFiscal.XML
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


        #endregion

        #region Métodos Privados

        private string ObterRetornoCONEMB(Dominio.Entidades.IntegracaoCTe integracaoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (integracaoCTe.CTe.Status.Equals("A") && integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao)
                return new MultiSoftware.EDI.CONEMB.v30A.Gerador(integracaoCTe.CTe.Empresa, integracaoCTe.CTe.Codigo, unidadeDeTrabalho).GerarString();
            else
                return string.Empty;
        }

        private string ObterRetornoPDF(Dominio.Entidades.IntegracaoCTe integracaoCTe, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true, bool retornarComoBinario = false)
        {
            if ((integracaoCTe.CTe.Status.Equals("A") || integracaoCTe.CTe.Status.Equals("Q") || integracaoCTe.CTe.Status.Equals("F")) && integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao)
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                return servicoCTe.ObterDACTE(integracaoCTe.CTe.Codigo, integracaoCTe.CTe.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8, retornarComoBinario);
            }
            else
            {
                return string.Empty;
            }
        }

        private string ObterRetornoPDFBIN(Dominio.Entidades.IntegracaoCTe integracaoCTe, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true)
        {
            if (integracaoCTe.CTe.Status.Equals("A") && integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao)
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                return servicoCTe.ObterDACTE(integracaoCTe.CTe.Codigo, integracaoCTe.CTe.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8);
            }
            else
            {
                return string.Empty;
            }
        }

        private string ObterRetornoTxt(Dominio.Entidades.IntegracaoCTe integracaoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (integracaoCTe.CTe.Status.Equals("A") && integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao)
                return new Servicos.NDDigital(unidadeDeTrabalho).GerarRetornoEmissao(integracaoCTe.CTe.Codigo, unidadeDeTrabalho).GerarRegistro();
            else if (integracaoCTe.CTe.Status.Equals("C") && integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Cancelamento)
                return new Servicos.NDDigital(unidadeDeTrabalho).GerarRetornoCancelamento(integracaoCTe.CTe.Codigo, unidadeDeTrabalho).GerarRegistro();
            else
                return string.Empty;
        }

        private string ObterRetornoXML(Dominio.Entidades.IntegracaoCTe integracaoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unidadeDeTrabalho);

            Dominio.Entidades.XMLCTe xml = null;

            if (integracaoCTe.CTe.Status.Equals("A") && (integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao || tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Todos))
            {
                xml = repXMLCTe.BuscarPorCTe(integracaoCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
            }

            //Se não achou o XML busca do Oracle
            if (xml == null)
            {
                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                if (integracaoCTe.CTe.Status.Equals("A") && (integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Emissao || tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Todos))
                {
                    if (integracaoCTe.CTe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech)
                        svcCTe.ObterXMLAutorizacao(integracaoCTe.CTe, unidadeDeTrabalho, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                    else
                        svcCTe.ObterESalvarXMLAutorizacao(integracaoCTe.CTe, false, null, unidadeDeTrabalho);

                    xml = repXMLCTe.BuscarPorCTe(integracaoCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                }
                else if (integracaoCTe.CTe.Status.Equals("C") && (integracaoCTe.Tipo == Dominio.Enumeradores.TipoIntegracao.Cancelamento || tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Todos))
                {
                    if (integracaoCTe.CTe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech)
                        svcCTe.ObterXMLCancelamento(integracaoCTe.CTe, unidadeDeTrabalho, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                    else
                        svcCTe.ObterESalvarXMLCancelamento(integracaoCTe.CTe, integracaoCTe.CTe.Empresa.Codigo, false, null, unidadeDeTrabalho);

                    xml = repXMLCTe.BuscarPorCTe(integracaoCTe.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);
                }
            }

            return xml != null ? xml.XML : String.Empty;
        }

        private string ObterXMLCTE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unidadeDeTrabalho);

            Dominio.Entidades.XMLCTe xml = null;

            if (cte.Status.Equals("A") && tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Emissao)
            {
                xml = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
            }
            else if (cte.Status.Equals("C") && tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Cancelamento)
            {
                xml = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);
            }

            if (xml == null)
            {
                if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech)
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                    svcCTe.ObterXMLAutorizacao(cte, unidadeDeTrabalho, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                    xml = repXMLCTe.BuscarPorCTe(cte.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                }
            }

            return xml != null ? xml.XML : string.Empty;
        }

        private string ObterPDFCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true)
        {
            if (cte.Status.Equals("A") && tipoIntegracao == Dominio.Enumeradores.TipoIntegracao.Emissao)
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                return servicoCTe.ObterDACTE(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8);
            }
            else
            {
                return string.Empty;
            }
        }

        private void ObterArquivosRetornoMDFe(ref RetornoMDFe retorno, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true)
        {
            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

            Servicos.ServicoMDFe.RetornoMDFe retornoMDFe = null;

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                retornoMDFe = servicoMDFe.ObterDadosIntegradosMDFe(mdfe.Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho);
                retorno.XML = retornoMDFe.XML;
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                string ambiente = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                if (!string.IsNullOrWhiteSpace(ambiente) && ambiente == "APTI")
                    retorno.PDF = servicoMDFe.ObterDAMDFE(mdfe.Codigo, unidadeDeTrabalho); //Buscar no Oracle -> APTI
                else
                    retorno.PDF = servicoMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8);

            }

        }


        #endregion
    }
}
