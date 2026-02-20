using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace EmissaoCTe.Integracao.Transportador
{
    public class CTe : ICTe
    {
        public Retorno<List<int>> ObterProtocolos(string cnpj, string token, string dataInicial, string dataFinal, string serie)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

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

                if (!string.IsNullOrWhiteSpace(dataInicial) && !string.IsNullOrWhiteSpace(dataFinal) && tokenEmpresaPai)
                { //pesquisa com datas só quando for pela empresa Pai GPA (Feito para transportadoras alterarem o token
                    return new Retorno<List<int>>() { Mensagem = "Token inválido.", Status = false };
                }

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

                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TimeOutObterProtocolosCTes"], out int timeOutConsulta);
                string filtroPorDataEvento = System.Configuration.ConfigurationManager.AppSettings["FiltroPorDataEvento"];

                string utilizarConsultaProtocolosPorSQL = System.Configuration.ConfigurationManager.AppSettings["UtilizarConsultaProtocolosPorSQL"];

                string quantidadeDiasRetroativo = System.Configuration.ConfigurationManager.AppSettings["QuantidadeDiasRetroativo"];

                string quantidadeProtocolosRetorno = System.Configuration.ConfigurationManager.AppSettings["QuantidadeProtocolosRetorno"];

                List<int> protocolos = null;

                if (utilizarConsultaProtocolosPorSQL == "SIM" && tokenEmpresaPai)
                {
                    int.TryParse(quantidadeProtocolosRetorno, out int quantidadeConsulta);                    
                    int.TryParse(quantidadeDiasRetroativo, out int diasRetroativos);

                    IList<Dominio.ObjetosDeValor.WebService.CTe.ProtocoloCTe> protocolosCTe = repCTe.BuscarListaCodigosPorSQL(empresa.Codigo, dtInicial, dtFinal, numeroSerie, timeOutConsulta, "", filtroPorDataEvento == "SIM", diasRetroativos, quantidadeConsulta > 0 ? quantidadeConsulta : 100);

                    protocolos = new List<int>();
                    foreach (Dominio.ObjetosDeValor.WebService.CTe.ProtocoloCTe prot in protocolosCTe)
                        protocolos.Add(prot.CodigoCTe);
                }
                else
                    protocolos = repCTe.BuscarListaCodigos(empresa.Codigo, dtInicial, dtFinal, numeroSerie, timeOutConsulta, "", filtroPorDataEvento == "SIM");

                if (System.Configuration.ConfigurationManager.AppSettings["GravarLogProtocolosCTe"] == "SIM")
                {
                    OperationContext context = OperationContext.Current;
                    MessageProperties properties = context.IncomingMessageProperties;
                    RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    string ipOrigem = endpoint.Address;

                    Servicos.Log.TratarErro("ObterProtocolos | Origem = " + ipOrigem + " | CNPJ = " + cnpj + " | Data Inicio = " + dataInicial + " | Data Fim = " + dataFinal + " | Serie = " + serie + " - retorno: " + String.Join(", ", protocolos.Distinct().ToArray()), cnpj);
                }

                return new Retorno<List<int>>() { Status = true, Mensagem = "Protocolos obtidos com sucesso.", Objeto = protocolos.Distinct().ToList() };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ObterProtocolosCTe");

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas ao obter protocolos CTe!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas ao obter protocolos CTe no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeDeTrabalho);
#else
                    svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeDeTrabalho);
#endif                    
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }

                return new Retorno<List<int>>() { Mensagem = "Ocorreu uma falha ao obter os protocolos dos CT-es.", Status = false };
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
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.RetornoXMLCTe repRetornoXMLCTe = new Repositorio.RetornoXMLCTe(unidadeDeTrabalho);

                Servicos.CTe svcCTE = new Servicos.CTe(unidadeDeTrabalho);

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

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCte.BuscarCTesPorCodigos(protocolos);

                List<RetornoConsultaXML> retorno = (from obj in ctes
                                                    where obj.Status == "A"
                                                    select new RetornoConsultaXML()
                                                    {
                                                        Chave = obj.Chave,
                                                        Protocolo = obj.Codigo,
                                                        TipoXML = TipoXML.Autorizacao,
                                                        XML = svcCTE.ObterStringXMLAutorizacao(obj, unidadeDeTrabalho)
                                                    }).ToList();

                retorno.AddRange((from obj in ctes
                                  where obj.Status == "C"
                                  select new RetornoConsultaXML()
                                  {
                                      Chave = obj.Chave,
                                      Protocolo = obj.Codigo,
                                      TipoXML = TipoXML.Cancelamento,
                                      XML = svcCTE.ObterStringXMLCancelamento(obj, unidadeDeTrabalho)
                                  }).ToList());

                if (tokenEmpresaPai)
                {
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                    {
                        Dominio.Entidades.RetornoXMLCTe retornoXMLCTe = new Dominio.Entidades.RetornoXMLCTe();
                        retornoXMLCTe.CTe = cte;
                        retornoXMLCTe.Status = cte.Status;
                        repRetornoXMLCTe.Inserir(retornoXMLCTe);
                    }
                }

                if (System.Configuration.ConfigurationManager.AppSettings["GravarLogProtocolosCTe"] == "SIM")
                {
                    OperationContext context = OperationContext.Current;
                    MessageProperties properties = context.IncomingMessageProperties;
                    RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    string ipOrigem = endpoint.Address;

                    Servicos.Log.TratarErro("ObterXML | Origem = " + ipOrigem + " | CNPJ = " + cnpj + " | Protocolos: " + String.Join(", ", protocolos.Distinct().ToArray()), cnpj);
                }

                return new Retorno<List<RetornoConsultaXML>>() { Status = true, Mensagem = "XML obtido com sucesso.", Objeto = retorno };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ObterXML");

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas ao obter XML CTe!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas ao obter XML CTe no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeDeTrabalho);
#else
                    svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeDeTrabalho);
#endif                    
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }

                return new Retorno<List<RetornoConsultaXML>>() { Mensagem = "Ocorreu uma falha ao obter o XML dos CT-es.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
