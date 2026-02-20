using System;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.Integracao.Transportador
{
    public class MDFe : IMDFe
    {
        public Retorno<List<int>> ObterProtocolos(string cnpj, string token, string dataInicial, string dataFinal, string serie)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);

                if (empresa == null)
                    return new Retorno<List<int>>() { Mensagem = "Transportador não encontrado.", Status = false };

                bool tokenEmpresaPai = false;
                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                {
                    if (empresa.EmpresaPai != null && empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe == token)
                        tokenEmpresaPai = true;
                    else
                        return new Retorno<List<int>>() { Mensagem = "Token de acesso inválido.", Status = false };
                }

                //if (!string.IsNullOrWhiteSpace(dataInicial) && !string.IsNullOrWhiteSpace(dataFinal) && tokenEmpresaPai)
                //{ //pesquisa com datas só quando for pela empresa Pai GPA (Feito para transportadoras alterarem o token
                //    return new Retorno<List<int>>() { Mensagem = "Token inválido.", Status = false };
                //}

                DateTime dtInicial, dtFinal;
                DateTime.TryParseExact(dataInicial, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtInicial);
                DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFinal);

                int numeroSerie = 0;
                int.TryParse(serie, out numeroSerie);

                if ((dtInicial == DateTime.MinValue || dtFinal == DateTime.MinValue) && string.IsNullOrWhiteSpace(serie))
                    return new Retorno<List<int>>() { Mensagem = "Deve ser enviado uma Data Inicial e Final ou um número de série.", Status = false };

                if (dtInicial == DateTime.MinValue && string.IsNullOrWhiteSpace(serie))
                    return new Retorno<List<int>>() { Mensagem = "Data inicial inválida (formato: dd/MM/yyyy). Quando não enviado um período deve ser enviado a série.", Status = false };

                if (dtFinal == DateTime.MinValue && string.IsNullOrWhiteSpace(serie))
                    return new Retorno<List<int>>() { Mensagem = "Data final inválida (formato: dd/MM/yyyy). Quando não enviado um período deve ser enviado a série", Status = false };

                if (dtInicial > DateTime.MinValue && dtFinal > DateTime.MinValue && (dtFinal - dtInicial).TotalDays > 45 && string.IsNullOrWhiteSpace(serie))
                    return new Retorno<List<int>>() { Mensagem = "Período inválido (máximo permitido: 45 dias).", Status = false };

                bool retornarEncerramento = System.Configuration.ConfigurationManager.AppSettings["RetornarXMLEncerramentoMDFeTransportadora"] == "SIM";

                List<int> protocolos = repMDFe.BuscarListaCodigos(empresa.Codigo, dtInicial, dtFinal, numeroSerie, retornarEncerramento);

                return new Retorno<List<int>>() { Status = true, Mensagem = "Protocolos obtidos com sucesso.", Objeto = protocolos.Distinct().ToList() };
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new Retorno<List<int>>() { Mensagem = "Ocorreu uma falha ao obter os protocolos dos MDF-es.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoConsultaXML>> ObterXML(string cnpj, string token, List<int> protocolos)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.XMLMDFe repXML = new Repositorio.XMLMDFe(unidadeDeTrabalho);
                Repositorio.RetornoXMLMDFe repRetornoXMLMDFe = new Repositorio.RetornoXMLMDFe(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);

                if (empresa == null)
                    return new Retorno<List<RetornoConsultaXML>>() { Mensagem = "Transportador não encontrado.", Status = false };
                
                bool tokenEmpresaPai = false;
                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                {
                    if (empresa.EmpresaPai != null && empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe == token)
                        tokenEmpresaPai = true;
                    else
                        return new Retorno<List<RetornoConsultaXML>>() { Mensagem = "Token de acesso inválido.", Status = false };
                }


                if (protocolos == null || protocolos.Count <= 0 || protocolos.Count > 100)
                    return new Retorno<List<RetornoConsultaXML>>() { Mensagem = "Total de protocolos inválido (mínimo 1, máximo 100).", Status = false };

                bool retornarXMLEncerramento = System.Configuration.ConfigurationManager.AppSettings["RetornarXMLEncerramentoMDFeTransportadora"] == "SIM";

                List<Dominio.Entidades.XMLMDFe> xmls = repXML.BuscarPorMDFe(protocolos, empresa.Codigo, retornarXMLEncerramento);

                List<RetornoConsultaXML> retornos = (from obj in xmls
                                                     select new RetornoConsultaXML()
                                                     {
                                                         Chave = obj.MDFe.Chave,
                                                         Protocolo = obj.MDFe.Codigo,
                                                         TipoXML = obj.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Autorizacao ? TipoXML.Autorizacao :
                                                                               (obj.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Cancelamento ? TipoXML.Cancelamento : TipoXML.Encerramento),
                                                         XML = obj.XML
                                                     }).ToList();

                if (tokenEmpresaPai)
                {
                    foreach (Dominio.Entidades.XMLMDFe xml in xmls)
                    {
                        Dominio.Entidades.RetornoXMLMDFe retornoXMLMDFe = new Dominio.Entidades.RetornoXMLMDFe();
                        retornoXMLMDFe.MDFe = xml.MDFe;
                        retornoXMLMDFe.Status = xml.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? "A" : xml.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ? "C" : xml.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ? "E" : "O";
                        repRetornoXMLMDFe.Inserir(retornoXMLMDFe);
                    }
                }

                return new Retorno<List<RetornoConsultaXML>>() { Status = true, Mensagem = "XML obtido com sucesso.", Objeto = retornos };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<List<RetornoConsultaXML>>() { Mensagem = "Ocorreu uma falha ao obter o XML dos MDF-es.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> EnviarEventoMDFe(string chaveMDFe, string cnpj, string protocolo, Dominio.Enumeradores.TipoIntegracaoMDFe tipoEvento, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                if (string.IsNullOrWhiteSpace(cnpj))
                    return new Retorno<int>() { Mensagem = "CNPJ do transportador inválido.", Status = false };

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "Transportador não encontrado.", Status = false };

                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(chaveMDFe))
                    return new Retorno<int>() { Mensagem = "Chave do MDFe inválida.", Status = false };


                if (string.IsNullOrWhiteSpace(protocolo) || protocolo.Length > 20)
                    return new Retorno<int>() { Mensagem = "Protocolo do evento inválido.", Status = false };

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorChave(chaveMDFe);

                if (mdfe == null)
                    return new Retorno<int>() { Mensagem = "MDFe não localizado.", Status = false };

                Dominio.Enumeradores.StatusMDFe? statusMDFe = null;
                if (tipoEvento == Dominio.Enumeradores.TipoIntegracaoMDFe.Cancelamento)
                    statusMDFe = Dominio.Enumeradores.StatusMDFe.Cancelado;
                else if (tipoEvento == Dominio.Enumeradores.TipoIntegracaoMDFe.Encerramento)
                    statusMDFe = Dominio.Enumeradores.StatusMDFe.Encerrado;

                if (statusMDFe == null)
                    return new Retorno<int>() { Mensagem = "Status para o evento enviado não localizado.", Status = false };

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    mdfe.Status = statusMDFe == Dominio.Enumeradores.StatusMDFe.Cancelado ? Dominio.Enumeradores.StatusMDFe.Cancelado : Dominio.Enumeradores.StatusMDFe.Encerrado;
                    mdfe.MensagemRetornoSefaz = statusMDFe == Dominio.Enumeradores.StatusMDFe.Cancelado ? "Evento de cancelamento recebido via WS" : "Evento de encerramento recebido via WS";
                    mdfe.MensagemStatus = null;
                    if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                        mdfe.ProtocoloEncerramento = protocolo;
                    else
                        mdfe.ProtocoloCancelamento = protocolo;

                    repMDFe.Atualizar(mdfe);
                }
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado)
                    return new Retorno<int>() { Mensagem = "Status do MDFe (Cancelado) não permite atualização.", Status = false };
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new Retorno<int>() { Mensagem = "Status do MDFe (Encerrado) não permite atualização.", Status = false };
                else
                    return new Retorno<int>() { Mensagem = "Status do MDFe não permite atualização.", Status = false };


                return new Retorno<int>() { Status = true, Mensagem = "Evento vinculado com sucesso ao MDFe.", Objeto = mdfe.Codigo };
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao obter os protocolos dos MDF-es.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
