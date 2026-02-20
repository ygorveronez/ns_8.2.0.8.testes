using Dominio.ObjetosDeValor.Enumerador;
using EmissaoCTe.Integracao.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace EmissaoCTe.Integracao
{
    public class ConhecimentoDeTransporteEletronico : BaseService, IConhecimentoDeTransporteEletronico
    {
        #region Metodos Publicos

        public Retorno<int> IntegrarCTePorTxt(int codigoEmpresaPai, string nomeArquivo, string arquivoTexto)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            string retorno = string.Empty;

            try
            {
                byte[] textBytes = System.Text.Encoding.Default.GetBytes(arquivoTexto);
                MemoryStream memoStream = new MemoryStream(textBytes);

                Dominio.NDDigital.v104.Emissao.Arquivo arquivo = new Dominio.NDDigital.v104.Emissao.Arquivo(memoStream);

                memoStream.Dispose();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);

                foreach (Dominio.NDDigital.v104.Emissao.Registro11000 registroCTe in arquivo.CTes)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(registroCTe.emit.CNPJ);

                    if (empresa == null || empresa.EmpresaPai == null || empresa.EmpresaPai.Codigo != codigoEmpresaPai)
                        return new Retorno<int>() { Mensagem = retorno + "A empresa (" + registroCTe.emit.CNPJ + ") não foi encontrada. ", Status = false };

                    if (empresa.Status != "A")
                        return new Retorno<int>() { Mensagem = retorno + "A empresa (" + registroCTe.emit.CNPJ + ") está inativa. ", Status = false };

                    if (empresa.StatusFinanceiro == "B")
                        return new Retorno<int>() { Mensagem = retorno + "A empresa (" + registroCTe.emit.CNPJ + ") está com pendências, contate o setor de cadastros para maiores informações.", Status = false };

                    if (empresa.Configuracao == null)
                        return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada. ", Status = false };

                    if (arquivo.CTes.First().rem.infNFe.Count > 0)
                    {
                        if (repDocumentoCTe.ContarPorChaveEStatus(arquivo.CTes.First().rem.infNFe.First().chave, new string[] { "A", "E", "P" }) > 0)
                            return new Retorno<int>() { Mensagem = "O arquivo possui NF-e já importada em um CT-e.", Status = false, Objeto = -100 };
                    }

                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorTxt(registroCTe, empresa.Codigo, unidadeDeTrabalho);

                    if (!this.AdicionarRegistroIntegrado(cte, registroCTe.infIntegracao.infCarga.numCarga, registroCTe.infIntegracao.infCarga.numUnidade, nomeArquivo, arquivoTexto, Dominio.Enumeradores.TipoArquivoIntegracao.TXT, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho))
                    {
                        unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar o CT-e na lista de integrações.", Status = false };
                    }

                    unidadeDeTrabalho.CommitChanges();

                    if (!servicoCTe.Emitir(cte.Codigo, empresa.Codigo, unidadeDeTrabalho))
                        retorno += "O CT-e nº " + cte.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                    if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                        retorno += "O CT-e nº " + cte.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                    retorno += "CT-e nº " + cte.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";
                }

                return new Retorno<int>() { Mensagem = retorno, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha ao integrar algum dos CT-es. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> CancelarCTePorTxt(int codigoEmpresaPai, string nomeArquivo, string arquivoTexto)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (string.IsNullOrWhiteSpace(arquivoTexto))
                    return new Retorno<int>() { Mensagem = "Arquivo inválido.", Status = false };

                byte[] textBytes = System.Text.Encoding.Default.GetBytes(arquivoTexto);
                MemoryStream memoStream = new MemoryStream(textBytes);

                Dominio.NDDigital.v104.Cancelamento.Arquivo arquivoCancelamento = new Dominio.NDDigital.v104.Cancelamento.Arquivo(memoStream);

                memoStream.Dispose();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(arquivoCancelamento.R00010.chCTe);

                if (cte == null)
                    return new Retorno<int>() { Mensagem = "CT-e (" + arquivoCancelamento.R00010.chCTe + ") não encontrado.", Status = false };

                Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPorCTe(cte.Codigo);

                if (integracao == null)
                    return new Retorno<int>() { Mensagem = "Registro de integração do CT-e (" + cte.Chave + ") não encontrado.", Status = false };

                if (cte.Empresa.EmpresaPai.Codigo != codigoEmpresaPai)
                    return new Retorno<int>() { Mensagem = "Empresa do CT-e não pertence à empresa solicitada.", Status = false };

                if (cte.Status != "A")
                    return new Retorno<int>() { Mensagem = "O status do CT-e não permite o cancelamento do mesmo.", Status = false };

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(cte.Codigo, cte.Empresa.Codigo, arquivoCancelamento.R00010.xJust, unitOfWork))
                    return new Retorno<int>() { Mensagem = "Não foi possível enviar o CT-e (" + cte.Chave + ") para cancelamento.", Status = false };

                if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") na fila de consulta.", Status = false };

                if (!this.AdicionarRegistroIntegrado(cte, integracao.NumeroDaCarga, integracao.NumeroDaUnidade, nomeArquivo, arquivoTexto, Dominio.Enumeradores.TipoArquivoIntegracao.TXT, Dominio.Enumeradores.TipoIntegracao.Cancelamento, unitOfWork))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") nos registros de integração, consulte o sistema da empresa." };

                return new Retorno<int>() { Mensagem = "CT-e (" + cte.Chave + ") em processo de cancelamento.", Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao cancelar o CT-e.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> EnviarXMLNFeParaIntegracao(Stream arquivo)
        {
            try
            {
                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosIntegracao"];

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, string.Concat(nomeArquivo, ".xml"));

                using (Stream fStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminho))
                {
                    arquivo.CopyTo(fStream);
                }

                arquivo.Close();

                return new Retorno<string>() { Objeto = nomeArquivo, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao salvar o arquivo.", Status = false };
            }
        }

        public Retorno<int> IntegrarCTePorXMLNFe(string token, int codigoEmpresa, string nomeArquivo)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return new Retorno<int>() { Mensagem = "Token inválido.", Status = false };

                var caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosIntegracao"], string.Concat(token, ".xml"));

                System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));

                string xmlNotaFiscal = reader.ReadToEnd();

                reader.BaseStream.Position = 0;

                var notaFiscal = MultiSoftware.NFe.Servicos.Leitura.Ler(reader.BaseStream);

                reader.Dispose();

                Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                if (notaFiscal.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                    return this.IntegrarCTePorXMLNFe((MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notaFiscal, codigoEmpresa, nomeArquivo, xmlNotaFiscal, unidadeDeTrabalho);
                else if (notaFiscal.GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                    return this.IntegrarCTePorXMLNFe((MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)notaFiscal, codigoEmpresa, nomeArquivo, xmlNotaFiscal, unidadeDeTrabalho);
                else
                    return new Retorno<int>() { Mensagem = "A NF-e não é uma versão suportada ou não está processada.", Status = false };
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao integrar o CT-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarCTe(Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token)
        {
            try
            {
                Servicos.Log.TratarErro("IntegrarCTe - CTe: " + (cte != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cte) : string.Empty));
                Servicos.Log.TratarErro("IntegrarCTe - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("IntegrarCTe - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                if (ConfigurationManager.AppSettings["GerarCTesIntegrarDocumentoPorThread"] == "SIM")
                    return this.EmitirCTePorObjetoNew(cte, cnpjEmpresaAdministradora, token);
                else
                    return this.EmitirCTePorObjeto(cte, cnpjEmpresaAdministradora, token);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha: IntegrarCTe " + ex);

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas na emissão de CT-e!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas na emissão no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#else
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#endif                    
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }



                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao integrar o CT-e.", Status = false };
            }
        }

        public Retorno<int> IntegrarCTeAguardarConfirmacao(Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token)
        {
            try
            {
                Servicos.Log.TratarErro("IntegrarCTeAguardarConfirmacao - CTe: " + (cte != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cte) : string.Empty));
                Servicos.Log.TratarErro("IntegrarCTeAguardarConfirmacao - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("IntegrarCTeAguardarConfirmacao - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                return this.EmitirCTePorObjetoAguardarConfirmacao(cte, cnpjEmpresaAdministradora, token);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha: IntegrarCTe " + ex);

                try
                {

                    Servicos.Email svcEmail = new Servicos.Email();

                    string ambiente = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    string assunto = ambiente + " - Problemas na emissão de CT-e!";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<p>Atenção, problemas na emissão no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                    sb.Append("Erro: ").Append(ex).Append("</p><br /> <br />");

                    System.Text.StringBuilder ss = new System.Text.StringBuilder();
                    ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#else
                    svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br", 587, null, 0, true, null, false);
#endif                    
                }
                catch (Exception exptEmail)
                {
                    Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
                }



                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao integrar o CT-e.", Status = false };
            }
        }

        public Retorno<int> CancelarCTe(string cnpjEmpresaAdministradora, string chaveCTe, string justificativa, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                if (string.IsNullOrWhiteSpace(chaveCTe) || chaveCTe.Length != 44)
                    return new Retorno<int>() { Mensagem = "Chave do CT-e inválida (" + chaveCTe + ").", Status = false };

                if (string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "CNPJ da Empresa inválido (" + cnpjEmpresaAdministradora + ").", Status = false };

                if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 20)
                    return new Retorno<int>() { Mensagem = "Justificativa inválida (" + justificativa + ").", Status = false };

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(chaveCTe);

                if (cte == null)
                    return new Retorno<int>() { Mensagem = "CT-e não encontrado.", Status = false };

                if (cte.Empresa.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "Empresa administradora inválida para a emissão do CT-e.", Status = false };

                if (cte.Empresa.EmpresaPai.Configuracao != null && token != cte.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (cte.Status != "A")
                    return new Retorno<int>() { Mensagem = "O status do CT-e não permite o cancelamento do mesmo.", Status = false };

                Dominio.Entidades.IntegracaoCTe integracao = repIntegracao.BuscarPorCTe(cte.Codigo);

                if (integracao == null)
                    return new Retorno<int>() { Mensagem = "Registro de integração do CT-e (" + cte.Chave + ") não encontrado.", Status = false };

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(cte.Codigo, cte.Empresa.Codigo, justificativa, unidadeDeTrabalho))
                    return new Retorno<int>() { Mensagem = "Não foi possível enviar o CT-e (" + cte.Chave + ") para cancelamento.", Status = false };

                if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") na fila de consulta.", Status = false };

                if (!this.AdicionarRegistroIntegrado(cte, integracao.NumeroDaCarga, integracao.NumeroDaUnidade, "", "", Dominio.Enumeradores.TipoArquivoIntegracao.TXT, Dominio.Enumeradores.TipoIntegracao.Cancelamento, unidadeDeTrabalho))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") nos registros de integração, consulte o sistema da empresa." };

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                svcCTe.AtualizarIntegracaoRetornoCTe(cte, unidadeDeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, $"Cancelamento de CTe nº{cte.Numero}, método: CancelarCTe", unidadeDeTrabalho);
                return new Retorno<int>() { Mensagem = "CT-e (" + cte.Chave + ") em processo de cancelamento.", Status = true };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao cancelar o CTe: " + ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao cancelar o CT-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> InutilizarCTe(string cnpjEmpresaAdministradora, string cnpjEmpresaEmitente, int numeroCTe, int serieCTe, string justificativa, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                return new Retorno<int>() { Mensagem = "Inutilização descontinuada pelo Sefaz.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                if (numeroCTe <= 0)
                    return new Retorno<int>() { Mensagem = "Número do CT-e inválido (" + numeroCTe.ToString() + ").", Status = false };

                if (serieCTe <= 0)
                    return new Retorno<int>() { Mensagem = "Série do CT-e inválida (" + serieCTe.ToString() + ").", Status = false };

                if (string.IsNullOrWhiteSpace(cnpjEmpresaEmitente))
                    return new Retorno<int>() { Mensagem = "CNPJ da Empresa Emitente inválido (" + cnpjEmpresaEmitente + ").", Status = false };

                if (string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "CNPJ da Empresa Administradora inválido (" + cnpjEmpresaAdministradora + ").", Status = false };

                if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 20)
                    return new Retorno<int>() { Mensagem = "Justificativa inválida (" + justificativa + ").", Status = false };

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresaEmitente);

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "Empresa não encontrada (" + cnpjEmpresaAdministradora + ").", Status = false };

                if (empresa.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "Empresa administradora inválida para a inutilização do CT-e (" + cnpjEmpresaAdministradora + ").", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorNumeroESerie(empresa.Codigo, numeroCTe, serieCTe, "57", empresa.TipoAmbiente);

                if (cte == null)
                    return new Retorno<int>() { Mensagem = "CT-e não encontrado.", Status = false };

                if (cte.Status != "R" && cte.Status != "S")
                    return new Retorno<int>() { Mensagem = "O status do CT-e não permite o cancelamento do mesmo.", Status = false };

                Dominio.Entidades.IntegracaoCTe integracao = repIntegracao.BuscarPorCTe(cte.Codigo);

                if (integracao == null)
                    return new Retorno<int>() { Mensagem = "Registro de integração do CT-e (" + cte.Chave + ") não encontrado.", Status = false };

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                if (!svcCTe.Inutilizar(cte.Codigo, cte.Empresa.Codigo, justificativa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe))
                    return new Retorno<int>() { Mensagem = "Não foi possível enviar o CT-e (" + cte.Chave + ") para cancelamento.", Status = false };

                if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") na fila de consulta.", Status = false };

                if (!this.AdicionarRegistroIntegrado(cte, integracao.NumeroDaCarga, integracao.NumeroDaUnidade, "", "", Dominio.Enumeradores.TipoArquivoIntegracao.TXT, Dominio.Enumeradores.TipoIntegracao.Inutilizacao, unidadeDeTrabalho))
                    return new Retorno<int>() { Mensagem = "Não foi possível adicionar o CT-e (" + cte.Chave + ") nos registros de integração, consulte o sistema da empresa." };

                return new Retorno<int>() { Mensagem = "CT-e (" + cte.Chave + ") em processo de inutilização.", Status = true };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao inutilizar o CT-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> AlterarCTe(int codigoCTe, Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString());
                Servicos.Log.TratarErro("AlterarCTe - CTe: " + (cte != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cte) : string.Empty));
                Servicos.Log.TratarErro("AlterarCTe - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("AlterarCTe - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAlterar = repCTe.BuscarPorCodigo(codigoCTe);
                var integracaoCTe = repIntegracaoCTe.BuscarPorCTe(codigoCTe);

                if (cteAlterar == null)
                {
                    Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString() + " nao encontrado.");
                    return new Retorno<int>() { Mensagem = "CT-e nao encontrado.", Objeto = codigoCTe, Status = false };
                }

                if (cteAlterar.Empresa.EmpresaPai == null || cteAlterar.Empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                {
                    Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString() + " nao pertence a esta empresa.");
                    return new Retorno<int>() { Mensagem = "O CT-e nao pertence a esta empresa.", Objeto = codigoCTe, Status = false };
                }

                if (cteAlterar.Status != "R" && cteAlterar.Status != "S" && !string.IsNullOrWhiteSpace(cteAlterar.Status))
                {
                    Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString() + " status " + cteAlterar.Status + " nao permite a alteracao.");
                    return new Retorno<int>() { Mensagem = "O status do CT-e nao permite a alteracao do mesmo.", Objeto = codigoCTe, Status = false };
                }

                if (cte != null)
                {
                    if (cte.Emitente == null || cteAlterar.Empresa.CNPJ != Utilidades.String.OnlyNumbers(cte.Emitente.CNPJ))
                    {
                        Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString() + " nao pertence a esta empresa");
                        return new Retorno<int>() { Mensagem = "O CT-e nao pertence a esta empresa.", Objeto = codigoCTe, Status = false };
                    }

                    if (integracaoCTe != null && !string.IsNullOrWhiteSpace(integracaoCTe.Arquivo) && JsonConvert.SerializeObject(cte) == integracaoCTe.Arquivo)
                    {
                        Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString() + " reemitindo sem alterar dados.");

                        if (!svcCTe.Emitir(cteAlterar.Codigo, cteAlterar.Empresa.Codigo, unidadeDeTrabalho))
                            return new Retorno<int>() { Mensagem = "O CT-e " + cteAlterar.Numero.ToString() + " da empresa " + cteAlterar.Empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.", Status = false };

                        if (!this.AdicionarCTeNaFilaDeConsulta(cteAlterar))
                            return new Retorno<int>() { Mensagem = "O CT-e " + cteAlterar.Numero.ToString() + " da empresa " + cteAlterar.Empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.", Status = false };

                        return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = cteAlterar.Codigo };
                    }
                    else
                    {

                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();

                        Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString() + " deletando dados.");
                        if (svcCTe.Deletar(cteAlterar.Codigo, cteAlterar.Empresa.Codigo, false, unidadeDeTrabalho))
                        {
                            Servicos.Log.TratarErro("AlterarCTe - Codigo: " + codigoCTe.ToString() + " reemitindo CTe.");
                            Retorno<int> retorno = this.EmitirCTePorObjeto(cte, cnpjEmpresaAdministradora, token, cteAlterar.Codigo, unidadeDeTrabalho);

                            if (retorno.Objeto > 0)
                                unidadeDeTrabalho.CommitChanges();
                            else
                                unidadeDeTrabalho.Rollback();

                            return retorno;
                        }
                        else
                        {
                            unidadeDeTrabalho.Rollback();

                            return new Retorno<int>() { Mensagem = "Nao foi possível alterar o CT-e, verifique o status do mesmo.", Status = false };
                        }
                    }
                }
                else
                {
                    return this.ReenviarCTe(cteAlterar, unidadeDeTrabalho);
                }
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro("AlterarCTe - " + ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao alterar o CT-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> EnviarXMLCTe(string xml, string cnpjEmpresaPai, string cnpjEmpresaEmitente, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);
                Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unidadeDeTrabalho, auditado: Auditado);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresaEmitente);

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não foi encontrada.", Status = false };

                if (empresa.Status != "A")
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não está ativa.", Status = false };

                if (empresa.StatusFinanceiro == "B")
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") está com pendências, contate o setor de cadastros para maiores informações.", Status = false };

                if (!string.IsNullOrWhiteSpace(cnpjEmpresaPai))
                    if (empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaPai))
                        return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaPai + ") não está vinculada ou não pode ter CT-es para esta empresa (" + cnpjEmpresaEmitente + ").", Status = false };

                if (token == "")
                    token = null;

                if (!string.IsNullOrWhiteSpace(cnpjEmpresaPai))
                {
                    if ((empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token) && (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token))
                        return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(token))
                        return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                    if (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };
                }

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não está configurada.", Status = false };

                if (string.IsNullOrWhiteSpace(xml))
                    return new Retorno<int>() { Mensagem = "XML inválido inválido.", Status = false };

                Stream arquivo = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? ""));

                object retorno = servicoCTe.GerarCTeAnterior(arquivo, empresa.Codigo, string.Empty, string.Empty, unidadeDeTrabalho, null, true);

                if (retorno != null)
                {
                    try
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = repCTe.BuscarPorCodigo(((Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno).Codigo);
                        {
                            if (cteIntegrado.Status == "A")
                                serCTEsImportados.DestinarCTeImportadoParaSeuDestino(cteIntegrado, unidadeDeTrabalho, Conexao.StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, "", null);
                            else if (cteIntegrado.Status == "C")
                                serCTEsImportados.VerificarCTeImportadoPertenceAlgumaCargaParaCancelamento(cteIntegrado, unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, configuracao);
                        }

                        if (cteIntegrado != null)
                            return new Retorno<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = cteIntegrado.Codigo };
                        else
                            return new Retorno<int>() { Mensagem = "Conhecimento de transporte não importado.", Status = false };
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        return new Retorno<int>() { Mensagem = retorno.ToString(), Status = false };
                    }
                }
                else
                {
                    return new Retorno<int>() { Mensagem = "Conhecimento de transporte não importado.", Status = false };
                }

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Conhecimento de transporte não importado.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> EnviarXMLCTeDeEnvio(string xmlEnvio, string chCTe, string nProt, string cStat, string tpAmb, string cnpjEmpresaPai, string cnpjEmpresaEmitente, string tipoVeiculo, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("XML: " + xmlEnvio);
                Servicos.Log.TratarErro("chCTe: " + chCTe);
                Servicos.Log.TratarErro("nProt: " + nProt);
                Servicos.Log.TratarErro("tpAmb: " + tpAmb);
                Servicos.Log.TratarErro("cnpjEmpresaPai: " + cnpjEmpresaPai);
                Servicos.Log.TratarErro("cnpjEmpresaEmitente: " + cnpjEmpresaEmitente);
                Servicos.Log.TratarErro("tipoVeiculo: " + tipoVeiculo);
                Servicos.Log.TratarErro("Token: " + token);

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresaEmitente);

                if (chCTe == null || chCTe == "")
                {
                    Servicos.Log.TratarErro("Validação: Chave CT-e (chCTe) não foi enviada.");
                    return new Retorno<int>() { Mensagem = "Chave CT-e (chCTe) não foi enviada.", Status = false };
                }

                if (nProt == null || nProt == "")
                {
                    Servicos.Log.TratarErro("Validação: Número Protocolo (nProt) não foi enviada.");
                    return new Retorno<int>() { Mensagem = "Número Protocolo (nProt) não foi enviada.", Status = false };
                }

                if (cStat == null || cStat == "")
                {
                    Servicos.Log.TratarErro("Validação: Código do status da resposta (cStat) não foi enviada.");
                    return new Retorno<int>() { Mensagem = "Código do status da resposta (cStat) não foi enviada.", Status = false };
                }

                if (tpAmb == null || tpAmb == "")
                {
                    Servicos.Log.TratarErro("Validação: Identificação do Ambiente (tpAmb : 1 – Produção / 2 – Homologação) não informado.");
                    return new Retorno<int>() { Mensagem = "Identificação do Ambiente (tpAmb : 1 – Produção / 2 – Homologação) não informado.", Status = false };
                }

                if (empresa == null)
                {
                    Servicos.Log.TratarErro("Validação: A empresa (" + cnpjEmpresaEmitente + ") não foi encontrada.");
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não foi encontrada.", Status = false };
                }

                if (empresa.Status != "A")
                {
                    Servicos.Log.TratarErro("Validação: A empresa (" + cnpjEmpresaEmitente + ") não está ativa.");
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não está ativa.", Status = false };
                }

                if (empresa.StatusFinanceiro == "B")
                {
                    Servicos.Log.TratarErro("Validação: A empresa (" + cnpjEmpresaEmitente + ") está com pendências, contate o setor de cadastros para maiores informações..");
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") está com pendências, contate o setor de cadastros para maiores informações..", Status = false };
                }

                if (!string.IsNullOrWhiteSpace(cnpjEmpresaPai))
                    if (empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaPai))
                    {
                        Servicos.Log.TratarErro("A empresa administradora (" + cnpjEmpresaPai + ") não está vinculada ou não pode ter CT-es para esta empresa (" + cnpjEmpresaEmitente + ").");
                        return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaPai + ") não está vinculada ou não pode ter CT-es para esta empresa (" + cnpjEmpresaEmitente + ").", Status = false };
                    }

                if (token == "")
                    token = null;

                if (!string.IsNullOrWhiteSpace(cnpjEmpresaPai))
                {
                    if ((empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token) && (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token))
                    {
                        Servicos.Log.TratarErro("Validação: Token de acesso inválido.");
                        return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        Servicos.Log.TratarErro("Validação: Token de acesso inválido.");
                        return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };
                    }

                    if (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token)
                    {
                        Servicos.Log.TratarErro("Validação: Token de acesso inválido.");
                        return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };
                    }
                }

                if (empresa.Configuracao == null)
                {
                    Servicos.Log.TratarErro("A empresa (" + cnpjEmpresaEmitente + ") não está configurada.");
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não está configurada.", Status = false };
                }

                if (string.IsNullOrWhiteSpace(xmlEnvio))
                {
                    Servicos.Log.TratarErro("XML inválido inválido.");
                    return new Retorno<int>() { Mensagem = "XML inválido inválido.", Status = false };
                }

                Stream arquivo = new MemoryStream(Encoding.Default.GetBytes(xmlEnvio ?? ""));

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = null;
                object retorno = null;

                try
                {
                    retorno = servicoCTe.GerarCTeXMLEnvio(arquivo, chCTe, nProt, cStat, tpAmb, tipoVeiculo, empresa.Codigo, unidadeDeTrabalho, null, true);

                    cteIntegrado = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                }
                catch (Exception ex)
                {
                    try
                    {
                        Servicos.Log.TratarErro("1 - Erro ao gerar CTe XML Envio: " + ex);
                        retorno = null;

                        unidadeDeTrabalho.Dispose();
                        System.Threading.Thread.Sleep(1000);
                        unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

                        Stream arquivo1 = new MemoryStream(Encoding.Default.GetBytes(xmlEnvio ?? ""));
                        retorno = servicoCTe.GerarCTeXMLEnvio(arquivo1, chCTe, nProt, cStat, tpAmb, tipoVeiculo, empresa.Codigo, unidadeDeTrabalho, null, true);

                        cteIntegrado = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro("2 - Erro ao gerar CTe XML Envio: " + ex2);
                        retorno = null;

                        unidadeDeTrabalho.Dispose();
                        System.Threading.Thread.Sleep(1000);
                        unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

                        Stream arquivo2 = new MemoryStream(Encoding.Default.GetBytes(xmlEnvio ?? ""));
                        retorno = servicoCTe.GerarCTeXMLEnvio(arquivo2, chCTe, nProt, cStat, tpAmb, tipoVeiculo, empresa.Codigo, unidadeDeTrabalho, null, true);

                        cteIntegrado = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                    }
                }

                if (retorno != null)
                {
                    try
                    {
                        if (cteIntegrado != null)
                        {
                            Repositorio.UnitOfWork unidadeDeTrabalhoCarga = new Repositorio.UnitOfWork(Conexao.StringConexao);
                            try
                            {
                                try
                                {
                                    if (servicoCTe.GerarCargaCTe(cteIntegrado.Codigo, "", "", tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, Conexao.StringConexao, unidadeDeTrabalhoCarga) <= 0)
                                    {
                                        unidadeDeTrabalhoCarga.Dispose();
                                        System.Threading.Thread.Sleep(1000);
                                        unidadeDeTrabalhoCarga = new Repositorio.UnitOfWork(Conexao.StringConexao);

                                        servicoCTe.GerarCargaCTe(cteIntegrado.Codigo, "", "", tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, Conexao.StringConexao, unidadeDeTrabalhoCarga);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro("Erro ao gerar carga XML CTe Envio: " + ex);
                                }
                            }
                            finally
                            {
                                unidadeDeTrabalhoCarga.Dispose();
                            }

                            return new Retorno<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = cteIntegrado.Codigo };
                        }
                        else
                            return new Retorno<int>() { Mensagem = "Conhecimento de transporte não importado.", Status = false };
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Erro ao importar XML CTe Envio " + retorno.ToString() + " : " + ex);
                        return new Retorno<int>() { Mensagem = "Não foi possível processar XML enviado.", Status = false };
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("CTe não importado de XML CTe Envio");
                    return new Retorno<int>() { Mensagem = "CTenão importado.", Status = false };
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao integrar XML CTe Envio: " + ex);
                return new Retorno<int>() { Mensagem = "CT-e não importado, falha no XML enviado.", Status = false };
            }
            finally
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> SolicitarReimpressaoCTe(int codigoCTe, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new Retorno<int>() { Mensagem = "CT-e encontrado.", Status = false };

                if (cte.Status != "A")
                    return new Retorno<int>() { Mensagem = "CT-e precisa estar autorizado para solicitar reimpressão. Status atual: " + cte.DescricaoStatus, Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ((cte.Empresa.CNPJ_SemFormato));

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cte.Empresa.CNPJ_SemFormato + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir CT-es para esta empresa (" + empresa.CNPJ + ").", Status = false };

                if (empresa.Status != "A")
                    return new Retorno<int>() { Mensagem = "A empresa(" + empresa.CNPJ + ") está inativa.", Status = false };

                if (empresa.StatusFinanceiro == "B")
                    return new Retorno<int>() { Mensagem = "A empresa(" + empresa.CNPJ + ") está com pendências, contate o setor de cadastros para maiores informações.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada.", Status = false };

                if (token == "")
                    token = null;

                if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);
                List<Dominio.Entidades.IntegracaoCTe> listaIntegracaoCTe = repIntegracaoCTe.BuscarPorCTeETipo(codigoCTe, Dominio.Enumeradores.TipoIntegracao.Emissao);

                if (listaIntegracaoCTe.Count() > 0)
                {
                    foreach (Dominio.Entidades.IntegracaoCTe integracaoCTe in listaIntegracaoCTe)
                    {
                        integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                        repIntegracaoCTe.Atualizar(integracaoCTe);
                    }

                    return new Retorno<int>() { Mensagem = "Reimpressão solicitada com sucesso.", Status = true };
                }
                else
                    return new Retorno<int>() { Mensagem = "Nenhuma integração encontrada para o CT-e.", Status = false };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha genérica ao solicitar reimpressão o CT-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> ConfirmarEmissaoCTe(int codigoCTe, string cnpjEmpresaAdministradora, string token)
        {
            Servicos.Log.TratarErro("ConfirmarEmissaoCTe - Codigo: " + codigoCTe.ToString());

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new Retorno<int>() { Mensagem = "CT-e não encontrado.", Status = false };

                if (cte.Status != "P")
                    return new Retorno<int>() { Mensagem = "CT-e não esta aguardando confirmação para emissão. Status atual: " + cte.DescricaoStatus, Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ((cte.Empresa.CNPJ_SemFormato));

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cte.Empresa.CNPJ_SemFormato + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir CT-es para esta empresa (" + empresa.CNPJ + ").", Status = false };

                if (empresa.Status != "A")
                    return new Retorno<int>() { Mensagem = "A empresa(" + empresa.CNPJ + ") está inativa.", Status = false };

                if (empresa.StatusFinanceiro == "B")
                    return new Retorno<int>() { Mensagem = "A empresa(" + empresa.CNPJ + ") está com pendências, contate o setor de cadastros para maiores informações.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada.", Status = false };

                if (token == "")
                    token = null;

                if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);
                Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPorCTeTipoStatus(codigoCTe, Dominio.Enumeradores.TipoIntegracao.Emissao, Dominio.Enumeradores.StatusIntegracao.AguardandoConfirmacao);

                if (integracaoCTe != null)
                {
                    integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe;
                    repIntegracaoCTe.Atualizar(integracaoCTe);

                    return new Retorno<int>() { Mensagem = "Confirmação realizada com sucesso.", Status = true };
                }
                else
                    return new Retorno<int>() { Mensagem = "Nenhuma integração aguardando confirmação encontrada para o CT-e.", Status = false };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha genérica ao realziar confirmação.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> ValidarCadastroVeiculo(string placa, string cnpjTransportador, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("ValidarCadastroVeiculo - Placa: " + (!string.IsNullOrWhiteSpace(placa) ? placa : string.Empty));
                Servicos.Log.TratarErro("ValidarCadastroVeiculo - cnpjTransportador: " + (!string.IsNullOrWhiteSpace(cnpjTransportador) ? cnpjTransportador : string.Empty));
                Servicos.Log.TratarErro("ValidarCadastroVeiculo - cnpjEmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("ValidarCadastroVeiculo - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                if (string.IsNullOrWhiteSpace(placa))
                    return new Retorno<int>() { Mensagem = "Placa inválida (" + placa + ").", Status = false };

                if (string.IsNullOrWhiteSpace(cnpjTransportador))
                    return new Retorno<int>() { Mensagem = "CNPJ do Transportador inválido (" + cnpjTransportador + ").", Status = false };

                if (string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "CNPJ da Empresa inválido (" + cnpjEmpresaAdministradora + ").", Status = false };

                if (string.IsNullOrWhiteSpace(token))
                    return new Retorno<int>() { Mensagem = "Token inválido (" + token + ").", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unidadeDeTrabalho);

                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(cnpjTransportador);

                if (transportador == null)
                    return new Retorno<int>() { Mensagem = "Transportador (" + cnpjTransportador + ") não encontrado.", Status = false };

                if (transportador.Status != "A")
                    return new Retorno<int>() { Mensagem = "Transportador (" + cnpjTransportador + ") não esta ativo.", Status = false };

                if (transportador.StatusFinanceiro == "B")
                    return new Retorno<int>() { Mensagem = "Transportador (" + cnpjTransportador + ") está com pendências, contate o setor de cadastros para maiores informações.", Status = false };

                if (transportador.DataFinalCertificado == null || transportador.DataFinalCertificado < DateTime.Today)
                    return new Retorno<int>() { Mensagem = "Transportador (" + cnpjTransportador + ") não possui certificado valido configurado.", Status = false };

                if (transportador.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "Empresa administradora inválida para integração.", Status = false };

                if (transportador.EmpresaPai.Configuracao != null && token != transportador.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(transportador.Codigo, placa);

                if (veiculo == null)
                    return new Retorno<int>() { Mensagem = "Veiculo placa " + placa + " nao possui cadastro para o transportador CNPJ " + cnpjTransportador + ".", Status = false };

                string exigirCadastroVeiculo = System.Configuration.ConfigurationManager.AppSettings["VeiculoExigeCadastroMotorista"];
                if (string.IsNullOrWhiteSpace(exigirCadastroVeiculo))
                    exigirCadastroVeiculo = "SIM";

                if (exigirCadastroVeiculo == "SIM" && repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo) == null)
                    return new Retorno<int>() { Mensagem = "Veiculo placa " + placa + " UF:" + veiculo.Estado.Sigla + " nao possui motorista vinculado.", Status = false };

                Retorno<int> retorno = new Retorno<int>() { Mensagem = "Veiculo placa " + placa + " UF:" + veiculo.Estado.Sigla + " possui cadastro ativo no transportador CNPJ " + cnpjTransportador + ".", Status = true };
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha: ValidarCadastroVeiculo " + ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao ValidarCadastroVeiculo", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<RetornoImposto> CalcularImpostos(Dominio.ObjetosDeValor.CalculoImposto.Dados dados, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("CalcularImpostos - Dados: " + (dados != null ? Newtonsoft.Json.JsonConvert.SerializeObject(dados) : string.Empty));
                Servicos.Log.TratarErro("CalcularImpostos - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("CalcularImpostos - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                if (dados == null)
                    return new Retorno<RetornoImposto>() { Mensagem = "Nenhum dado enviado.", Status = false };

                if (dados.Transportador == null || string.IsNullOrWhiteSpace(dados.Transportador.CNPJ))
                    return new Retorno<RetornoImposto>() { Mensagem = "Emitente não informado.", Status = false };

                if (dados.Remetente == null || string.IsNullOrWhiteSpace(dados.Remetente.CPFCNPJ))
                    return new Retorno<RetornoImposto>() { Mensagem = "Remetente não informado.", Status = false };

                if (dados.Remetente.CodigoAtividade <= 0)
                    return new Retorno<RetornoImposto>() { Mensagem = "Atividade do Remetente não informado.", Status = false };

                if (dados.Destinatario == null || string.IsNullOrWhiteSpace(dados.Destinatario.CPFCNPJ))
                    return new Retorno<RetornoImposto>() { Mensagem = "Destinatario não informado.", Status = false };

                if (dados.Destinatario.CodigoAtividade <= 0)
                    return new Retorno<RetornoImposto>() { Mensagem = "Atividade do Destinatario não informado.", Status = false };

                if (dados.IBGEOrigemPrestacao <= 0)
                    return new Retorno<RetornoImposto>() { Mensagem = "IBGE Origem da prestação não informado.", Status = false };

                if (dados.IBGETerminoPrestacao <= 0)
                    return new Retorno<RetornoImposto>() { Mensagem = "IBGE Termino da prestação não informado.", Status = false };

                //if (dados.ValorFrete <= 0)
                //    return new Retorno<RetornoImposto>() { Mensagem = "Valor do frete não informado.", Status = false };

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(unidadeDeTrabalho);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(dados.Transportador.CNPJ));

                if (empresa == null)
                    return new Retorno<RetornoImposto>() { Mensagem = "Emissor CNPJ " + dados.Transportador.CNPJ + " não localizado.", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<RetornoImposto>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.Entidades.Localidade localidadeOrigem = repLocalidade.BuscarPorCodigoIBGE(dados.IBGEOrigemPrestacao);

                if (localidadeOrigem == null)
                    return new Retorno<RetornoImposto>() { Mensagem = "Localidade Origem " + dados.IBGEOrigemPrestacao + " não localizada.", Status = false };

                Dominio.Entidades.Localidade localidadeTermino = repLocalidade.BuscarPorCodigoIBGE(dados.IBGETerminoPrestacao);

                if (localidadeTermino == null)
                    return new Retorno<RetornoImposto>() { Mensagem = "Localidade Termino " + dados.IBGETerminoPrestacao + " não localizada.", Status = false };

                Dominio.Entidades.Cliente remetente = svcCTe.ObterCliente(empresa, dados.Remetente, unidadeDeTrabalho);

                if (remetente == null)
                    return new Retorno<RetornoImposto>() { Mensagem = "Não foi possível cadastrar remetente CNPJ " + dados.Remetente.CPFCNPJ, Status = false };

                Dominio.Entidades.Cliente destinatario = svcCTe.ObterCliente(empresa, dados.Destinatario, unidadeDeTrabalho);

                if (destinatario == null)
                    return new Retorno<RetornoImposto>() { Mensagem = "Não foi possível cadastrar destinatario CNPJ " + dados.Remetente.CPFCNPJ, Status = false };

                Dominio.Enumeradores.OpcaoSimNao simplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                if (empresa.OptanteSimplesNacional && empresa.Localidade.Estado.Sigla == localidadeOrigem.Estado.Sigla)
                    simplesNacional = Dominio.Enumeradores.OpcaoSimNao.Sim;

                bool incluirICMS = false;
                decimal percentualInclusaoICMS = 100;

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMSMultiCTe(empresa, simplesNacional, remetente, destinatario, dados.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? destinatario : remetente, localidadeOrigem, localidadeTermino, false, ref incluirICMS, ref percentualInclusaoICMS, dados.ValorFrete, dados.TipoServico, dados.CodigoProdutoEmbarcador, unidadeDeTrabalho);

                if (regraICMS == null)
                    return new Retorno<RetornoImposto>() { Mensagem = "Não foi possível calcular impostos.", Status = false };

                if (simplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim || regraICMS.CST == "SN")
                {
                    Retorno<RetornoImposto> retorno = new Retorno<RetornoImposto> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                    retorno.Objeto = new RetornoImposto()
                    {
                        CFOP = regraICMS.CFOP,
                        CSTICMS = "SN",
                        AliquotaICMS = 0,
                        BaseCalculoICMS = 0,
                        ValorICMS = 0,
                        ValorFrete = dados.ValorFrete,
                        IBGEMunicipioGerador = empresa.Localidade.CodigoIBGE
                    };
                    return retorno;
                }
                else
                {
                    Retorno<RetornoImposto> retorno = new Retorno<RetornoImposto> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                    decimal baseCalculoICMS = 0;
                    decimal valorICMS = 0;

                    if (dados.ValorFrete > 0)
                    {
                        baseCalculoICMS = dados.ValorFrete;
                        if (regraICMS.ValorBaseCalculoICMS == 0)
                            baseCalculoICMS = 0;
                        if (baseCalculoICMS > 0 && regraICMS.PercentualReducaoBC > 0)
                            baseCalculoICMS -= decimal.Round(baseCalculoICMS * (regraICMS.PercentualReducaoBC / 100), 2, MidpointRounding.ToEven);

                        if (regraICMS.Aliquota > 0 && baseCalculoICMS > 0)
                            valorICMS = baseCalculoICMS * (regraICMS.Aliquota / 100);
                    }

                    retorno.Objeto = new RetornoImposto()
                    {
                        CFOP = regraICMS.CFOP,
                        CSTICMS = regraICMS.CST,
                        AliquotaICMS = regraICMS.Aliquota,
                        BaseCalculoICMS = baseCalculoICMS,
                        ValorICMS = valorICMS,
                        ValorFrete = dados.ValorFrete,
                        IBGEMunicipioGerador = empresa.Localidade.CodigoIBGE,
                        Mensagem = regraICMS.ObservacaoCTe
                    };
                    return retorno;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha: CalcularImpostos " + ex);

                return new Retorno<RetornoImposto>() { Mensagem = "Ocorreu uma falha generica ao calcular impostos.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarCTeCTeComplementar(string chaveCTeComplementado, decimal valorComplemento, string observacao, string cnpjEmpresaAdministradora, string token)
        {
            Servicos.Log.TratarErro("IntegrarCTeCTeComplementar - ChaveCTeComplementado: " + (!string.IsNullOrWhiteSpace(chaveCTeComplementado) ? chaveCTeComplementado : string.Empty));
            Servicos.Log.TratarErro("IntegrarCTeCTeComplementar - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
            Servicos.Log.TratarErro("IntegrarCTeCTeComplementar - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return new Retorno<int>() { Mensagem = "Token inválido.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora));

                if (empresaPai == null)
                    return new Retorno<int>() { Mensagem = "CNPJ empresa administradora " + cnpjEmpresaAdministradora + " não localizado.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = repCTe.BuscarPorChave(chaveCTeComplementado);

                if (cteComplementado == null)
                    return new Retorno<int>() { Mensagem = "CT-e nao encontrado.", Objeto = 0, Status = false };

                if (cteComplementado.Status != "A")
                    return new Retorno<int>() { Mensagem = "CT-e complementado deve estar Autorizado.", Objeto = 0, Status = false };

                if (cteComplementado.Empresa.Configuracao != null && cteComplementado.Empresa.Configuracao.DiasParaEmissaoDeCTeComplementar > 0)
                {
                    if (cteComplementado.DataAutorizacao.Value.AddDays(cteComplementado.Empresa.Configuracao.DiasParaEmissaoDeCTeComplementar) < DateTime.Today)
                        return new Retorno<int>() { Mensagem = "Fora do prazo para emissão de complemento, " + cteComplementado.Empresa.Configuracao.DiasParaEmissaoDeCTeComplementar.ToString() + " dias.", Objeto = 0, Status = false };

                }
                else if (empresaPai.Configuracao != null && empresaPai.Configuracao.DiasParaEmissaoDeCTeComplementar > 0)
                {
                    if (cteComplementado.DataAutorizacao.Value.AddDays(empresaPai.Configuracao.DiasParaEmissaoDeCTeComplementar) < DateTime.Today)
                        return new Retorno<int>() { Mensagem = "Fora do prazo para emissão de complemento, " + empresaPai.Configuracao.DiasParaEmissaoDeCTeComplementar.ToString() + " dias.", Objeto = 0, Status = false };
                }

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                    unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                else
                    unidadeDeTrabalho.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementar = svcCTe.GerarCTeComplementar(cteComplementado, valorComplemento, observacao, unidadeDeTrabalho);

                if (cteComplementar != null)
                    unidadeDeTrabalho.CommitChanges();
                else
                    unidadeDeTrabalho.Rollback();

                if (!svcCTe.Emitir(cteComplementar.Codigo, cteComplementar.Empresa.Codigo, unidadeDeTrabalho))
                    return new Retorno<int>() { Mensagem = "O CT-e nº " + cteComplementar.Numero.ToString() + " da empresa " + cteComplementar.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.", Status = false };

                if (!this.AdicionarCTeNaFilaDeConsulta(cteComplementar))
                    return new Retorno<int>() { Mensagem = "O CT-e nº " + cteComplementar.Numero.ToString() + " da empresa " + cteComplementar.Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta.", Status = false };

                return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = cteComplementar.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha: IntegrarCTeCTeComplementar " + ex);

                unidadeDeTrabalho.Rollback();

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha generica ao integrar o CT-e complementar.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Retorno<int> IntegrarCTePorXMLNFe(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal, int codigoEmpresa, string nomeArquivo, string xmlNotaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (notaFiscal.NFe.infNFe.transp == null || notaFiscal.NFe.infNFe.transp.transporta == null)
                return new Retorno<int>() { Mensagem = "Tag transp/transporta não encontrada.", Status = false };

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorEmpresaPai(codigoEmpresa, notaFiscal.NFe.infNFe.transp.transporta.Item);

            if (empresa == null)
                return new Retorno<int>() { Mensagem = "Empresa emissora não encontrada.", Status = false };

            if (empresa.Status != "A")
                return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") está inativa. ", Status = false };

            if (empresa.StatusFinanceiro == "B")
                return new Retorno<int>() { Mensagem = "A empresa(" + empresa.CNPJ + ") está com pendências, contate o setor de cadastros para maiores informações.", Status = false };

            if (empresa.Configuracao == null)
                return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada.", Status = false };

            int numeroDaCarga = 0;
            decimal valorDoFrete = 0m;

            if (notaFiscal.NFe.infNFe.transp != null && notaFiscal.NFe.infNFe.transp.vol != null && notaFiscal.NFe.infNFe.transp.vol.Length > 0)
            {
                if (notaFiscal.NFe.infNFe.transp.vol[0].lacres != null && notaFiscal.NFe.infNFe.transp.vol[0].lacres.Length > 0)
                    int.TryParse(notaFiscal.NFe.infNFe.transp.vol[0].lacres[0].nLacre, out numeroDaCarga);

                if (notaFiscal.NFe.infNFe.transp.vol.Length > 1 && notaFiscal.NFe.infNFe.transp.vol[1].lacres != null && notaFiscal.NFe.infNFe.transp.vol[1].lacres.Length > 0)
                    decimal.TryParse(notaFiscal.NFe.infNFe.transp.vol[1].lacres[0].nLacre, out valorDoFrete);
            }

            if (empresa.Configuracao.TipoGeracaoCTeWS == Dominio.Enumeradores.TipoGeracaoCTeWS.GerarCTe)
            {
                return this.EmitirCTe(empresa, notaFiscal, numeroDaCarga, valorDoFrete, nomeArquivo, xmlNotaFiscal, unidadeDeTrabalho);
            }
            else if (empresa.Configuracao.TipoGeracaoCTeWS == Dominio.Enumeradores.TipoGeracaoCTeWS.GerarMDFe)
            {
                return this.SalvarArquivoParaEmissaoDeMDFe(empresa, numeroDaCarga, xmlNotaFiscal, unidadeDeTrabalho);
            }
            else
            {
                return new Retorno<int>() { Mensagem = "Tipo de geração de CT-e não configurado na empresa.", Status = false };
            }
        }

        private Retorno<int> IntegrarCTePorXMLNFe(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc notaFiscal, int codigoEmpresa, string nomeArquivo, string xmlNotaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (notaFiscal.NFe.infNFe.transp == null || notaFiscal.NFe.infNFe.transp.transporta == null)
                return new Retorno<int>() { Mensagem = "Tag transp/transporta não encontrada.", Status = false };

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorEmpresaPai(codigoEmpresa, notaFiscal.NFe.infNFe.transp.transporta.Item);

            if (empresa == null)
                return new Retorno<int>() { Mensagem = "Empresa emissora não encontrada.", Status = false };

            if (empresa.Status != "A")
                return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") está inativa. ", Status = false };

            if (empresa.StatusFinanceiro == "B")
                return new Retorno<int>() { Mensagem = "A empresa(" + empresa.CNPJ + ") está com pendências, contate o setor de cadastros para maiores informações.", Status = false };

            if (empresa.Configuracao == null)
                return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada.", Status = false };

            int numeroDaCarga = 0;
            decimal valorDoFrete = 0m;

            if (notaFiscal.NFe.infNFe.transp != null && notaFiscal.NFe.infNFe.transp.vol != null && notaFiscal.NFe.infNFe.transp.vol.Length > 0)
            {
                if (notaFiscal.NFe.infNFe.transp.vol[0].lacres != null && notaFiscal.NFe.infNFe.transp.vol[0].lacres.Length > 0)
                    int.TryParse(notaFiscal.NFe.infNFe.transp.vol[0].lacres[0].nLacre, out numeroDaCarga);

                if (notaFiscal.NFe.infNFe.transp.vol.Length > 1 && notaFiscal.NFe.infNFe.transp.vol[1].lacres != null && notaFiscal.NFe.infNFe.transp.vol[1].lacres.Length > 0)
                    decimal.TryParse(notaFiscal.NFe.infNFe.transp.vol[1].lacres[0].nLacre, out valorDoFrete);
            }

            if (empresa.Configuracao.TipoGeracaoCTeWS == Dominio.Enumeradores.TipoGeracaoCTeWS.GerarCTe)
            {
                return this.EmitirCTe(empresa, notaFiscal, numeroDaCarga, valorDoFrete, nomeArquivo, xmlNotaFiscal, unidadeDeTrabalho);
            }
            else if (empresa.Configuracao.TipoGeracaoCTeWS == Dominio.Enumeradores.TipoGeracaoCTeWS.GerarMDFe)
            {
                return this.SalvarArquivoParaEmissaoDeMDFe(empresa, numeroDaCarga, xmlNotaFiscal, unidadeDeTrabalho);
            }
            else
            {
                return new Retorno<int>() { Mensagem = "Tipo de geração de CT-e não configurado na empresa.", Status = false };
            }
        }

        private Retorno<int> EmitirCTePorObjeto(Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token, int codigoCTe = 0, Repositorio.UnitOfWork unitOfWork = null)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (cte == null)
                    return new Retorno<int>() { Mensagem = "O CT-e nao deve ser nulo para a integracao.", Status = false };

                if (cte.Emitente == null)
                    return new Retorno<int>() { Mensagem = "O Emitente nao pode ser nulo.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cte.Emitente.CNPJ));

                string erros = this.ValidarCTe(cte, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new Retorno<int>() { Mensagem = erros, Status = false };

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = null;

                string bloquearIntegracaoDuplicada = System.Configuration.ConfigurationManager.AppSettings["BloquearIntegracaoDuplicada"];
                if (bloquearIntegracaoDuplicada == "SIM" && cte.Documentos != null && cte.Documentos.Count > 0 && codigoCTe == 0)
                {
                    Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                    Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                    string chaveNFe = cte.Documentos.FirstOrDefault().ChaveNFE;

                    Servicos.Log.TratarErro("Validando duplicidade integracao ref. chave NFe: " + chaveNFe, "DuplicidadeIntegracao");
                    if (chaveNFe != null)
                    {
                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentosCTe.BuscarCTesPorChaveNFe(chaveNFe, empresa.Codigo);

                        Servicos.Log.TratarErro("CTEs com mesma NFe: " + listaCTes.Count().ToString(), "DuplicidadeIntegracao");

                        foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNota in listaCTes)
                        {
                            Servicos.Log.TratarErro("CTe " + cteNota.Numero + " status: " + cteNota.DescricaoStatus, "DuplicidadeIntegracao");

                            if (cteNota.Status == "A" || cteNota.Status == "R" || cteNota.Status == "E" || cteNota.Status == "Y" || cteNota.Status == "P" || string.IsNullOrWhiteSpace(cteNota.Status))
                            {
                                Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPorCTe(cteNota.Codigo);

                                if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Arquivo) && JsonConvert.SerializeObject(cte) == integracao.Arquivo)
                                {
                                    cteIntegrado = integracao.CTe;
                                    Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteIntegrado.Numero + cteIntegrado.Empresa.CNPJ + " Integração: " + JsonConvert.SerializeObject(cte), "DuplicidadeIntegracao");
                                    break;
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteNota.Numero + cteNota.Empresa.CNPJ + " integração diferente da integração anterior.", "DuplicidadeIntegracao");
                                }
                            }
                        }
                    }
                }

                string retorno = string.Empty;

                if (cteIntegrado == null)
                {
                    if (unitOfWork == null)
                    {
                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();
                    }

                    cteIntegrado = servicoCTe.GerarCTePorObjeto(cte, codigoCTe, unidadeDeTrabalho, "1", 0, !string.IsNullOrEmpty(cte.FinalizarCarga) && codigoCTe == 0 ? "Y" : "E", null, 0, null, empresa);

                    if (!this.AdicionarRegistroIntegrado(cteIntegrado, cte.NumeroCarga, cte.NumeroUnidade, "", JsonConvert.SerializeObject(cte), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho, cte.CodigoControleInternoCliente, cte.CodigoTipoOperacao, cte.Romaneio, cte.TipoVeiculo, cte.TipoCalculo, cte.ValorDespesa, cte.FinalizarCarga))
                    {
                        if (unitOfWork == null)
                            unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar o CT-e na lista de integracoes.", Status = false };
                    }

                    if (unitOfWork == null)
                        unidadeDeTrabalho.CommitChanges();

                }
                else if (cteIntegrado.Status == "R" || string.IsNullOrWhiteSpace(cteIntegrado.Status))
                {
                    if (unitOfWork == null)
                    {
                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();
                    }

                    if (servicoCTe.Deletar(cteIntegrado.Codigo, cteIntegrado.Empresa.Codigo, false, unidadeDeTrabalho))
                    {
                        cteIntegrado = servicoCTe.GerarCTePorObjeto(cte, cteIntegrado.Codigo, unidadeDeTrabalho, "1", 0, !string.IsNullOrEmpty(cte.FinalizarCarga) && codigoCTe == 0 ? "Y" : "E", null, 0, null, empresa);

                        if (unitOfWork == null)
                            unidadeDeTrabalho.CommitChanges();
                    }
                    else
                    {
                        if (unitOfWork == null)
                            unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Nao foi possível integrar o CT-e, verifique o status do mesmo.", Status = false };
                    }
                }

                if (cteIntegrado.Status == "E")
                {
                    if (!servicoCTe.Emitir(cteIntegrado.Codigo, empresa.Codigo, unidadeDeTrabalho))
                        retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.";

                    if (!this.AdicionarCTeNaFilaDeConsulta(cteIntegrado))
                        retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.";
                }
                else if (cteIntegrado.Status == "Y" && !string.IsNullOrWhiteSpace(cte.FinalizarCarga) && cte.FinalizarCarga.ToUpper() == "TRUE")
                {
                    this.AdicionarCTeNaFilaDeConsulta(cteIntegrado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cteIntegrado, $"Integração de CTe nº{cteIntegrado.Numero}, método: IntegrarCTe", unidadeDeTrabalho);

                if (string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = cteIntegrado.Codigo };
                else
                    return new Retorno<int>() { Mensagem = retorno, Status = false, Objeto = cteIntegrado.Codigo };
            }
            catch (Exception)
            {
                if (unitOfWork == null && unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();

                throw;
            }
            finally
            {
                if (unitOfWork == null && unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        private Retorno<int> EmitirCTePorObjetoNew(Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (cte == null)
                    return new Retorno<int>() { Mensagem = "O CT-e nao deve ser nulo para a integracao.", Status = false };

                if (cte.Emitente == null)
                    return new Retorno<int>() { Mensagem = "O Emitente nao pode ser nulo.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cte.Emitente.CNPJ));

                string erros = this.ValidarCTe(cte, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new Retorno<int>() { Mensagem = erros, Status = false };

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = null;
                bool integracaoDuplicidadeNFe = false;
                bool integracaoDuplicidadeObservacaoCarga = false;

                string bloquearIntegracaoDuplicada = System.Configuration.ConfigurationManager.AppSettings["BloquearIntegracaoDuplicada"];

                if (bloquearIntegracaoDuplicada == "SIM" && cte.Documentos != null && cte.Documentos.Count > 0)
                {
                    Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                    Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                    string chaveNFe = cte.Documentos.FirstOrDefault().ChaveNFE;

                    Servicos.Log.TratarErro("Validando duplicidade integracao ref. chave NFe: " + chaveNFe, "DuplicidadeIntegracao");
                    if (chaveNFe != null)
                    {
                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentosCTe.BuscarCTesPorChaveNFe(chaveNFe, empresa.Codigo);

                        Servicos.Log.TratarErro("CTEs com mesma NFe: " + listaCTes.Count().ToString(), "DuplicidadeIntegracao");

                        foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNota in listaCTes)
                        {
                            Servicos.Log.TratarErro("CTe " + cteNota.Numero + " status: " + cteNota.DescricaoStatus, "DuplicidadeIntegracao");

                            if (cteNota.Status == "A" || cteNota.Status == "R" || cteNota.Status == "E" || cteNota.Status == "Y" || cteNota.Status == "P" || string.IsNullOrWhiteSpace(cteNota.Status))
                            {
                                string validarDuplicidadeCTeComNFeECarga = System.Configuration.ConfigurationManager.AppSettings["ValidarDuplicidadeCTeComNFeECarga"];
                                string bloquearIntegracaoDuplicadaPorObservacaoCarga = System.Configuration.ConfigurationManager.AppSettings["BloquearIntegracaoDuplicadaPorObservacaoCarga"];

                                Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPorCTe(cteNota.Codigo);

                                if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Arquivo) && bloquearIntegracaoDuplicadaPorObservacaoCarga == "SIM" && !string.IsNullOrWhiteSpace(cte.ObservacaoDaCarga) && cte.TipoCTe == Dominio.Enumeradores.TipoCTE.Normal)
                                {
                                    Dominio.ObjetosDeValor.CTe.CTe dadosCTeAnterior = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTe>(integracao.Arquivo);

                                    if (cte.ObservacaoDaCarga == dadosCTeAnterior.ObservacaoDaCarga)
                                    {
                                        integracaoDuplicidadeObservacaoCarga = true;
                                        cteIntegrado = integracao.CTe;
                                        Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteIntegrado.Numero + cteIntegrado.Empresa.CNPJ + " Integração: " + JsonConvert.SerializeObject(cte), "DuplicidadeIntegracao");
                                        break;
                                    }
                                }
                                else if (integracao != null && validarDuplicidadeCTeComNFeECarga == "SIM" && integracao.NumeroDaCarga == cte.NumeroCarga && cte.TipoCTe == Dominio.Enumeradores.TipoCTE.Normal)
                                {
                                    integracaoDuplicidadeNFe = true;
                                    cteIntegrado = integracao.CTe;
                                    Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteIntegrado.Numero + cteIntegrado.Empresa.CNPJ + " Integração: " + JsonConvert.SerializeObject(cte), "DuplicidadeIntegracao");
                                    break;
                                }
                                else if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Arquivo) && JsonConvert.SerializeObject(cte) == integracao.Arquivo)
                                {
                                    cteIntegrado = integracao.CTe;
                                    Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteIntegrado.Numero + cteIntegrado.Empresa.CNPJ + " Integração: " + JsonConvert.SerializeObject(cte), "DuplicidadeIntegracao");
                                    break;
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteNota.Numero + cteNota.Empresa.CNPJ + " integração diferente da integração anterior.", "DuplicidadeIntegracao");
                                }
                            }
                        }
                    }
                }

                string retorno = string.Empty;

                if (cteIntegrado == null)
                {
                    Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unidadeDeTrabalho);
                    Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                    Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                    unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                    Dominio.Entidades.ModalTransporte modal = repModalTransporte.BuscarPorCodigo(1, false);
                    Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumentoFiscal.BuscarPorModelo("57");
                    Dominio.Entidades.EmpresaSerie empresaSerie = repSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.CTe); //servicoCTe.ObterSerie(empresa, empresa.Localidade.Estado.Sigla, empresa.Localidade.Estado.Sigla, empresa.Localidade.Estado.Sigla, unidadeDeTrabalho);

                    Dominio.Entidades.CFOP cfop = ObterCFOP(cte, empresa, unidadeDeTrabalho);

                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    cteIntegrado = servicoCTe.GerarCTeTemporarioIntegracao(cte, empresa, modal, modelo, empresaSerie, cfop, 0, unidadeDeTrabalho);

                    if (!this.AdicionarRegistroCTeIntegrado(cteIntegrado, cte.NumeroCarga, cte.NumeroUnidade, "", cte.CodigoTipoOperacao, JsonConvert.SerializeObject(cte), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho, cte.CodigoControleInternoCliente, cte.Romaneio, cte.TipoVeiculo, cte.TipoCalculo, cte.ValorDespesa, Dominio.Enumeradores.StatusIntegracao.AguardandoGeracaoCTe))
                    {
                        unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar o CT-e na lista de integracoes.", Status = false };
                    }

                    unidadeDeTrabalho.CommitChanges();
                    this.AdicionarCTeNaFilaDeConsulta(null);

                }
                else if (cteIntegrado.Status == "R" || string.IsNullOrWhiteSpace(cteIntegrado.Status))
                {
                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    if (servicoCTe.Deletar(cteIntegrado.Codigo, cteIntegrado.Empresa.Codigo, false, unidadeDeTrabalho))
                    {
                        cteIntegrado = servicoCTe.GerarCTePorObjeto(cte, cteIntegrado.Codigo, unidadeDeTrabalho, "1", 0, !string.IsNullOrEmpty(cte.FinalizarCarga) ? "Y" : "E", null, 0, null, empresa);

                        unidadeDeTrabalho.CommitChanges();
                    }
                    else
                    {
                        unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Nao foi possível integrar o CT-e, verifique o status do mesmo.", Status = false };
                    }
                }

                if (cteIntegrado.Status == "E")
                {
                    if (!servicoCTe.Emitir(cteIntegrado.Codigo, empresa.Codigo, unidadeDeTrabalho))
                        retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.";

                    if (!this.AdicionarCTeNaFilaDeConsulta(cteIntegrado))
                        retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.";
                }
                else if (cteIntegrado.Status == "Y" && !string.IsNullOrWhiteSpace(cte.FinalizarCarga) && cte.FinalizarCarga.ToUpper() == "TRUE")
                {
                    this.AdicionarCTeNaFilaDeConsulta(cteIntegrado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cteIntegrado, $"Integração de CTe nº{cteIntegrado.Numero}, método: IntegrarCTe", unidadeDeTrabalho);

                if (string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = !integracaoDuplicidadeNFe ? (!integracaoDuplicidadeObservacaoCarga ? "Integracao realizada com sucesso." : "CTe ja integrado.") : "CTe ja integrado com mesma NFe", Status = true, Objeto = cteIntegrado.Codigo };
                else
                    return new Retorno<int>() { Mensagem = retorno, Status = false, Objeto = cteIntegrado.Codigo };
            }
            catch (Exception)
            {
                unidadeDeTrabalho.Rollback();

                throw;
            }
            finally
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        private Retorno<int> EmitirCTePorObjetoAguardarConfirmacao(Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (cte == null)
                    return new Retorno<int>() { Mensagem = "O CT-e nao deve ser nulo para a integracao.", Status = false };

                if (cte.Emitente == null)
                    return new Retorno<int>() { Mensagem = "O Emitente nao pode ser nulo.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cte.Emitente.CNPJ));

                string erros = this.ValidarCTe(cte, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new Retorno<int>() { Mensagem = erros, Status = false };

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = null;

                string bloquearIntegracaoDuplicada = System.Configuration.ConfigurationManager.AppSettings["BloquearIntegracaoDuplicada"];
                if (bloquearIntegracaoDuplicada == "SIM" && cte.Documentos != null && cte.Documentos.Count > 0)
                {
                    Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
                    Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                    string chaveNFe = cte.Documentos.FirstOrDefault().ChaveNFE;

                    Servicos.Log.TratarErro("Validando duplicidade integracao ref. chave NFe: " + chaveNFe, "DuplicidadeIntegracao");
                    if (chaveNFe != null)
                    {
                        List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repDocumentosCTe.BuscarCTesPorChaveNFe(chaveNFe, empresa.Codigo);

                        Servicos.Log.TratarErro("CTEs com mesma NFe: " + listaCTes.Count().ToString(), "DuplicidadeIntegracao");

                        foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNota in listaCTes)
                        {
                            Servicos.Log.TratarErro("CTe " + cteNota.Numero + " status: " + cteNota.DescricaoStatus, "DuplicidadeIntegracao");

                            if (cteNota.Status == "A" || cteNota.Status == "R" || cteNota.Status == "E" || cteNota.Status == "Y" || cteNota.Status == "P" || cteNota.Status == "Y" || string.IsNullOrWhiteSpace(cteNota.Status))
                            {
                                Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPorCTe(cteNota.Codigo);

                                if (integracao != null && !string.IsNullOrWhiteSpace(integracao.Arquivo) && JsonConvert.SerializeObject(cte) == integracao.Arquivo)
                                {
                                    cteIntegrado = integracao.CTe;
                                    Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteIntegrado.Numero + cteIntegrado.Empresa.CNPJ + " Integração: " + JsonConvert.SerializeObject(cte), "DuplicidadeIntegracao");
                                    break;
                                }
                                else
                                {
                                    Servicos.Log.TratarErro("Integração em duplicidade CTe " + cteNota.Numero + cteNota.Empresa.CNPJ + " integração diferente da integração anterior.", "DuplicidadeIntegracao");
                                }
                            }
                        }
                    }
                }

                string retorno = string.Empty;

                if (cteIntegrado == null)
                {
                    Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unidadeDeTrabalho);
                    Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                    Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                    unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                    Dominio.Entidades.ModalTransporte modal = repModalTransporte.BuscarPorCodigo(1, false);
                    Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumentoFiscal.BuscarPorModelo("57");
                    Dominio.Entidades.EmpresaSerie empresaSerie = repSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.CTe);

                    Dominio.Entidades.CFOP cfop = ObterCFOP(cte, empresa, unidadeDeTrabalho);

                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();

                    cteIntegrado = servicoCTe.GerarCTeTemporarioIntegracao(cte, empresa, modal, modelo, empresaSerie, cfop, 0, unidadeDeTrabalho);

                    if (!this.AdicionarRegistroCTeIntegrado(cteIntegrado, cte.NumeroCarga, cte.NumeroUnidade, "", cte.CodigoTipoOperacao, JsonConvert.SerializeObject(cte), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho, cte.CodigoControleInternoCliente, cte.Romaneio, cte.TipoVeiculo, cte.TipoCalculo, cte.ValorDespesa, Dominio.Enumeradores.StatusIntegracao.AguardandoConfirmacao))
                    {
                        unidadeDeTrabalho.Rollback();

                        return new Retorno<int>() { Mensagem = "Ocorreu uma falha e nao foi possivel adicionar o CT-e na lista de integracoes.", Status = false };
                    }

                    unidadeDeTrabalho.CommitChanges();
                    this.AdicionarCTeNaFilaDeConsulta(null);

                }
                else if (cteIntegrado.Status == "R" || cteIntegrado.Status == "S" || string.IsNullOrWhiteSpace(cteIntegrado.Status))
                {
                    if (ConfigurationManager.AppSettings["NaoRegerarCTeRejeitado"] == "SIM" && !string.IsNullOrWhiteSpace(cteIntegrado.Status))
                    {
                        cteIntegrado.Status = "E";
                        repCTe.Atualizar(cteIntegrado);
                    }
                    else
                    {
                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unidadeDeTrabalho.Start();

                        if (servicoCTe.Deletar(cteIntegrado.Codigo, cteIntegrado.Empresa.Codigo, false, unidadeDeTrabalho))
                        {
                            cteIntegrado = servicoCTe.GerarCTePorObjeto(cte, cteIntegrado.Codigo, unidadeDeTrabalho, "1", 0, !string.IsNullOrEmpty(cte.FinalizarCarga) ? "Y" : "E", null, 0, null, empresa);

                            unidadeDeTrabalho.CommitChanges();
                        }
                        else
                        {
                            unidadeDeTrabalho.Rollback();

                            return new Retorno<int>() { Mensagem = "Nao foi possível integrar o CT-e, verifique o status do mesmo.", Status = false };
                        }
                    }
                }

                if (cteIntegrado.Status == "E")
                {
                    if (!servicoCTe.Emitir(cteIntegrado.Codigo, empresa.Codigo, unidadeDeTrabalho))
                        retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.";

                    if (!this.AdicionarCTeNaFilaDeConsulta(cteIntegrado))
                        retorno += "O CT-e " + cteIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.";
                }
                else if (cteIntegrado.Status == "Y" && !string.IsNullOrWhiteSpace(cte.FinalizarCarga) && cte.FinalizarCarga.ToUpper() == "TRUE")
                {
                    this.AdicionarCTeNaFilaDeConsulta(cteIntegrado);
                }

                if (string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = "Integracao realizada com sucesso.", Status = true, Objeto = cteIntegrado.Codigo };
                else
                    return new Retorno<int>() { Mensagem = retorno, Status = false, Objeto = cteIntegrado.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("EmitirCTePorObjetoAguardarConfirmacao: " + ex);

                unidadeDeTrabalho.Rollback();

                throw;
            }
            finally
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        private void GerarMDFeCTesCarga(Dominio.Entidades.Empresa empresa, int numeroDaUnidade, int numeroDaCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaIntegracaoCTe = repIntegracaoCTe.BuscarCTesPorCarga(empresa.Codigo, numeroDaUnidade, numeroDaCarga, "");
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaIntegracaoCTeAutorizados = repIntegracaoCTe.BuscarCTesPorCarga(empresa.Codigo, numeroDaUnidade, numeroDaCarga, "A");

                while (listaIntegracaoCTeAutorizados.Count() < listaIntegracaoCTe.Count())
                {
                    System.Threading.Thread.Sleep(5000);
                    listaIntegracaoCTeAutorizados = repIntegracaoCTe.BuscarCTesPorCarga(empresa.Codigo, numeroDaUnidade, numeroDaCarga, "A");
                }


                if (listaIntegracaoCTe.Count > 0)
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = svcMDFe.GerarMDFePorCTes(empresa, listaIntegracaoCTe, unidadeDeTrabalho);

                    if (svcMDFe.Emitir(mdfe, unidadeDeTrabalho))
                        AdicionarMDFeNaFilaDeConsulta(mdfe);
                    else
                        Servicos.Log.TratarErro("Ocorreu uma falha e o MDF-e automatico não pode ser emitido.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao gerar MDFe automatico: " + ex);
            }
        }

        private Retorno<int> ReenviarCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork = null)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = unitOfWork ?? new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (cte == null)
                    return new Retorno<int>() { Mensagem = "O CT-e não deve ser nulo para a integração.", Status = false };

                Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

                if (unitOfWork == null)
                {
                    if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                        unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                    else
                        unidadeDeTrabalho.Start();
                }

                string retorno = string.Empty;

                if (!servicoCTe.Emitir(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho))
                    retorno += "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";

                if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                    retorno += "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta.";

                if (string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = cte.Codigo };
                else
                    return new Retorno<int>() { Mensagem = retorno, Status = false, Objeto = cte.Codigo };
            }
            catch
            {
                if (unitOfWork == null && unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();

                throw;
            }
            finally
            {
                if (unitOfWork == null && unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Dispose();
            }
        }

        private Retorno<int> SalvarArquivoParaEmissaoDeMDFe(Dominio.Entidades.Empresa empresa, int numeroDaCarga, string xmlNotaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.IntegracaoMDFeArquivo repIntegracaoArquivoMDFe = new Repositorio.IntegracaoMDFeArquivo(unidadeDeTrabalho);

            Dominio.Entidades.IntegracaoMDFeArquivo integracaoMDFeArquivo = new Dominio.Entidades.IntegracaoMDFeArquivo();

            integracaoMDFeArquivo.NumeroDaCarga = numeroDaCarga;
            integracaoMDFeArquivo.NumeroDaUnidade = 0;
            integracaoMDFeArquivo.Arquivo = xmlNotaFiscal;
            integracaoMDFeArquivo.Empresa = empresa;

            repIntegracaoArquivoMDFe.Inserir(integracaoMDFeArquivo);

            return new Retorno<int>() { Mensagem = "Integração para MDF-e gerada com sucesso.", Objeto = 0, Status = true };
        }

        private Retorno<int> EmitirCTe(Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc notaFiscal, int numeroDaCarga, decimal valorDoFrete, string nomeArquivo, string xmlNotaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

            if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
            else
                unidadeDeTrabalho.Start();

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorNFe(notaFiscal, empresa.Codigo, valorDoFrete, 0, string.Empty, false, null, unidadeDeTrabalho);

            if (!this.AdicionarRegistroIntegrado(cte, numeroDaCarga, 0, nomeArquivo, xmlNotaFiscal, Dominio.Enumeradores.TipoArquivoIntegracao.NFe, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho))
            {
                unidadeDeTrabalho.Rollback();

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar o CT-e na lista de integrações.", Status = false };
            }

            unidadeDeTrabalho.CommitChanges();

            string retorno = string.Empty;

            if (!servicoCTe.Emitir(cte.Codigo, empresa.Codigo))
                retorno += "O CT-e foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

            if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                retorno += "O CT-e foi salvo, porém, não foi possível adicioná-lo na fila de consulta.";

            if (!string.IsNullOrWhiteSpace(retorno))
                return new Retorno<int>() { Mensagem = retorno, Status = false, Objeto = cte.Codigo };
            else
                return new Retorno<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = cte.Codigo };
        }

        private Retorno<int> EmitirCTe(Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc notaFiscal, int numeroDaCarga, decimal valorDoFrete, string nomeArquivo, string xmlNotaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

            if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
            else
                unidadeDeTrabalho.Start();

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorNFe(notaFiscal, empresa.Codigo, valorDoFrete, 0, string.Empty, false, null, unidadeDeTrabalho);

            if (!this.AdicionarRegistroIntegrado(cte, numeroDaCarga, 0, nomeArquivo, xmlNotaFiscal, Dominio.Enumeradores.TipoArquivoIntegracao.NFe, Dominio.Enumeradores.TipoIntegracao.Emissao, unidadeDeTrabalho))
            {
                unidadeDeTrabalho.Rollback();

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar o CT-e na lista de integrações.", Status = false };
            }

            unidadeDeTrabalho.CommitChanges();

            string retorno = string.Empty;

            if (!servicoCTe.Emitir(cte.Codigo, empresa.Codigo))
                retorno += "O CT-e foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

            if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                retorno += "O CT-e foi salvo, porém, não foi possível adicioná-lo na fila de consulta.";

            if (!string.IsNullOrWhiteSpace(retorno))
                return new Retorno<int>() { Mensagem = retorno, Status = false, Objeto = cte.Codigo };
            else
                return new Retorno<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = cte.Codigo };
        }

        private bool AdicionarCTeNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                if (cte == null || cte.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    return true;

                string postData = "CodigoCTe=" + (cte != null ? cte.Codigo : 0);
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["WebServiceConsultaCTe"], "IntegracaoCTe/AdicionarNaFilaDeConsulta"));

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


        private bool AdicionarMDFeNaFilaDeConsulta(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            try
            {
                if (mdfe.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    return true;

                string postData = "CodigoMDFe=" + mdfe.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["WebServiceConsultaCTe"], "IntegracaoMDFe/AdicionarNaFilaDeConsulta"));

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

        private bool AdicionarRegistroIntegrado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, int numeroDaCarga, int numeroDaUnidade, string nomeArquivo, string arquivo, Dominio.Enumeradores.TipoArquivoIntegracao tipoArquivo, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho, string codigoControleInternoCliente = null, string tipoOperacao = null, string romaneio = null, string tipoVeiculo = null, string tipoCalculo = null, decimal valorDespesa = 0, string finalizarCarga = "")
        {
            try
            {
                string gerarCargaCTeEmbarcador = System.Configuration.ConfigurationManager.AppSettings["GerarCargaCTe"];

                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.IntegracaoCTe integracao = new Dominio.Entidades.IntegracaoCTe();

                integracao.CTe = cte;
                integracao.Arquivo = arquivo;
                integracao.NumeroDaCarga = numeroDaCarga;
                integracao.NumeroDaUnidade = numeroDaUnidade;
                integracao.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                integracao.TipoArquivo = tipoArquivo;
                integracao.NomeArquivo = nomeArquivo;
                integracao.Tipo = tipoIntegracao;
                integracao.CodigoControleInternoCliente = codigoControleInternoCliente;
                integracao.CodigoTipoOperacao = tipoOperacao;

                integracao.Romaneio = romaneio;
                integracao.TipoVeiculo = tipoVeiculo;
                integracao.TipoCalculo = tipoCalculo;
                integracao.ValorDespesa = valorDespesa;
                integracao.GerouCargaEmbarcador = gerarCargaCTeEmbarcador == "SIM" ? false : true;  //Quando false roda uma thread que gera as cargas para o embarcador              
                integracao.FinalizarCarga = !string.IsNullOrWhiteSpace(finalizarCarga) ? finalizarCarga.ToLower() : null;
                integracao.FinalizouCarga = false;

                repIntegracao.Inserir(integracao);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private string ValidarCTe(Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Empresa empresa, string cnpjEmpresaAdministradora, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            string exigirCadastroVeiculo = System.Configuration.ConfigurationManager.AppSettings["CTeExigeVeiculoCadastro"];

            if (empresa == null)
                return "A empresa (" + cte.Emitente.CNPJ + ") nao foi encontrada.";

            if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                return "A empresa administradora (" + cnpjEmpresaAdministradora + ") nao esta vinculada ou nao pode emitir CT-es para esta empresa (" + empresa.CNPJ + ").";

            if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                return "Token de acesso invalido.";

            if (empresa.Status != "A")
                return "A empresa (" + empresa.CNPJ + ") esta inativa.";

            if (empresa.StatusFinanceiro == "B")
                return "A empresa (" + empresa.CNPJ + ") esta com pendencias, contate o setor de cadastros para maiores informacoes.";

            if (empresa.Configuracao == null)
                return "A empresa (" + empresa.CNPJ + ") nao esta configurada.";

            if (cte.Serie > 0)
            {
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                if (repEmpresaSerie.BuscarPorSerie(empresa.Codigo, cte.Serie, Dominio.Enumeradores.TipoSerie.CTe) == null)
                    return "A empresa (" + empresa.CNPJ + ") nao possui a serie " + cte.Serie.ToString() + " liberada para a emissao de CT-e.";
            }
            else if (empresa.Configuracao.SerieInterestadual == null || empresa.Configuracao.SerieIntraestadual == null)
                return "A empresa (" + empresa.CNPJ + ") nao possui uma serie configurada para a emissao de CT-e.";

            decimal valorLimiteFrete = empresa.Configuracao != null && empresa.Configuracao.ValorLimiteFrete > 0 ? empresa.Configuracao.ValorLimiteFrete : empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.ValorLimiteFrete > 0 ? empresa.EmpresaPai.Configuracao.ValorLimiteFrete : 0;
            if (valorLimiteFrete > 0 && (cte.ValorFrete > 0 || cte.ValorAReceber > 0))
            {
                if (cte.ValorAReceber > 0 && cte.ValorAReceber > valorLimiteFrete)
                    return "Valor a Receber do Frete R$" + cte.ValorAReceber.ToString("n2", cultura) + " é maior do que o valor limite permitido.";
                if (cte.ValorFrete > 0 && cte.ValorFrete > valorLimiteFrete)
                    return "Valor do Frete R$" + cte.ValorFrete.ToString("n2", cultura) + " é maior do que o valor limite permitido.";
            }

            StringBuilder erros = new StringBuilder();

            if (cte.Remetente == null)
                erros.Append("Remetente nao pode ser nulo. ");
            else
            {
                if (cte.Remetente.CodigoAtividade <= 0)
                    erros.Append("Atividade do remetente invalida. ");
                if (string.IsNullOrWhiteSpace(cte.Remetente.CPFCNPJ) && !cte.Remetente.Exportacao)
                    erros.Append("CNPJ do remetente invalido. ");
                if (string.IsNullOrWhiteSpace(cte.Remetente.RazaoSocial))
                    erros.Append("Razao social do remetente invalida. ");
            }

            if (cte.Destinatario == null)
                erros.Append("Destinatario nao pode ser nulo. ");
            else
            {
                if (cte.Destinatario.CodigoAtividade <= 0)
                    erros.Append("Atividade do destinatario invalida. ");
                if (string.IsNullOrWhiteSpace(cte.Destinatario.CPFCNPJ) && !cte.Destinatario.Exportacao)
                    erros.Append("CNPJ do destinatario invalido. ");
                if (string.IsNullOrWhiteSpace(cte.Destinatario.RazaoSocial))
                    erros.Append("Razao social do destinatario invalida. ");
            }

            if (cte.TomadorDoCTe == null)
                erros.Append("Tomador nao pode ser nulo. ");
            else
            {
                if (cte.TomadorDoCTe.CodigoAtividade <= 0)
                    erros.Append("Atividade do tomador invalida. ");
                if (string.IsNullOrWhiteSpace(cte.TomadorDoCTe.CPFCNPJ) && !cte.TomadorDoCTe.Exportacao)
                    erros.Append("CNPJ do tomador invalido. ");
                if (string.IsNullOrWhiteSpace(cte.TomadorDoCTe.RazaoSocial))
                    erros.Append("Razao social do tomador invalida. ");
            }

            if (cte.CodigoIBGECidadeInicioPrestacao <= 0)
                erros.Append("Codigo da cidade de início da prestação invalido. ");

            if (cte.CodigoIBGECidadeTerminoPrestacao <= 0)
                erros.Append("Codigo da cidade de termino da prestação invalido. ");

            List<Dominio.Entidades.Veiculo> veiculosCadastrados = new List<Dominio.Entidades.Veiculo>();

            if (cte.Veiculos != null && cte.Veiculos.Count > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                foreach (var veiculo in cte.Veiculos)
                {
                    if (string.IsNullOrWhiteSpace(veiculo.Placa) || veiculo.Placa.Length != 7)
                    {
                        erros.Append("A placa do veículo (" + veiculo.Placa + ") é invalida. ");
                    }
                    else
                    {
                        Dominio.Entidades.Veiculo veiculoCadastrado = repVeiculo.BuscarPorPlaca(empresa.Codigo, veiculo.Placa);

                        if (veiculoCadastrado == null)
                        {
                            if (exigirCadastroVeiculo == "SIM")
                                erros.Append("O veiculo " + veiculo.Placa + " não possui cadastro. ");
                            else
                            {
                                if (string.IsNullOrWhiteSpace(veiculo.Renavam) || veiculo.Renavam.Trim().Length < 9 || veiculo.Renavam.Trim().Length > 11)
                                    erros.Append("O RENAVAM do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoCarroceria))
                                    erros.Append("O tipo de carroceria do veeculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoPropriedade))
                                    erros.Append("O tipo de proriedade do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoRodado))
                                    erros.Append("O tipo de rodado do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (string.IsNullOrWhiteSpace(veiculo.TipoVeiculo))
                                    erros.Append("O tipo de veiculo do veiculo (" + veiculo.Placa + ") invalido. ");

                                if (veiculo.Tara > 999999)
                                    erros.Append("A tara do veiculo invalida (deve possuir de 1 a 6 digitos). ");

                                if (veiculo.CapacidadeKG > 999999)
                                    erros.Append("A capacidade em KG do veiculo invalida (deve possuir de 1 a 6 digitos). ");

                                if (veiculo.CapacidadeM3 > 999)
                                    erros.Append("A capacidade em M3 do veiculo invalida (deve possuir de 1 a 3 digitos). ");
                            }
                        }
                        else
                        {
                            veiculosCadastrados.Add(veiculoCadastrado);
                        }
                    }
                }
            }

            if (cte.Motoristas != null && cte.Motoristas.Count > 0)
            {
                foreach (var motorista in cte.Motoristas)
                {
                    if (string.IsNullOrWhiteSpace(motorista.CPF) || motorista.CPF.Trim().Length != 11)
                        erros.Append("O CPF do motorista (" + motorista.CPF + ") invalido. ");

                    Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                    Dominio.Entidades.Usuario motoristaCadastrado = repMotorista.BuscarMotoristaPorCPF(empresa.Codigo, motorista.CPF);

                    if (motoristaCadastrado == null && string.IsNullOrWhiteSpace(motorista.Nome))
                        erros.Append("O nome do motorista invalido. ");
                }
            }

            if (cte.NumeroCarga <= 0)
                erros.Append("Numero da carga invalido. ");

            if (string.IsNullOrWhiteSpace(cte.ProdutoPredominante))
                erros.Append("Produto predominante invalido. ");

            if (empresa.EmpresaPai.Configuracao == null || empresa.EmpresaPai.Configuracao.UtilizaTabelaDeFrete == false)
                if (cte.ValorFrete <= 0)
                    erros.Append("Valor do frete invalido. ");

            if (cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                if (cte.PercentualICMSIncluirNoFrete <= 0m)
                    erros.Append("Percentual de ICMS a incluir no frete invalido. ");

            if (cte.Emitente.OptanteSimplesNacional == null || cte.Emitente.OptanteSimplesNacional == false)
            {
                if (cte.ICMS != null)
                {
                    if (!(new List<string>() { "00", "20", "40", "41", "51", "60", "90", "91", "" }.Contains(cte.ICMS.CST)))
                        erros.Append("CST do ICMS invalida. ");

                    Repositorio.AliquotaDeICMS repAliquota = new Repositorio.AliquotaDeICMS(unidadeDeTrabalho);
                    if (repAliquota.BuscarPorAliquota(empresa.Codigo, cte.ICMS.Aliquota) == null)
                    {
                        Dominio.Entidades.AliquotaDeICMS aliquota = new Dominio.Entidades.AliquotaDeICMS();
                        aliquota.Aliquota = cte.ICMS.Aliquota;
                        aliquota.Empresa = empresa;
                        aliquota.Status = "A";
                        repAliquota.Inserir(aliquota);
                    }

                }
            }

            if (cte.Documentos != null && cte.Documentos.Count > 0)
            {
                List<Dominio.Enumeradores.TipoDocumentoCTe> tipos = (from obj in cte.Documentos where obj != null select obj.Tipo).Distinct().ToList();

                if (tipos.Count() > 1)
                {
                    erros.Append("Somente um tipo de documento deve ser informado para o CT-e. ");
                }
                else
                {
                    foreach (var documento in cte.Documentos)
                    {
                        if (documento == null)
                        {
                            erros.Append("O documento nao pode ser nulo. ");
                        }
                        else
                        {
                            DateTime data;
                            int inteiro;

                            if (string.IsNullOrWhiteSpace(documento.ChaveNFE) && (string.IsNullOrWhiteSpace(documento.Numero) || !int.TryParse(documento.Numero, out inteiro) || inteiro <= 0))
                                erros.Append("O numero do documento nao pode ser nulo e deve ser maior que zero. ");

                            if (string.IsNullOrWhiteSpace(documento.ChaveNFE) && (string.IsNullOrWhiteSpace(documento.DataEmissao) || !DateTime.TryParseExact(documento.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data)))
                                erros.Append("A data de emissao do documento invalida ou esta no formato incorreto (dd/MM/yyyy HH:mm:ss). ");

                            //if (documento.Valor <= 0m) removido porcausa da APTI
                            //    erros += "O valor do documento deve ser maior que zero. ";

                            if (documento.Tipo == Dominio.Enumeradores.TipoDocumentoCTe.NFe)
                            {
                                if (string.IsNullOrWhiteSpace(documento.ChaveNFE) || documento.ChaveNFE.Length != 44)
                                    erros.Append("A chave da NF-e (" + documento.ChaveNFE + ") nao pode ser nula e deve possuir 44 dígitos. ");
                            }
                            else if (documento.Tipo == Dominio.Enumeradores.TipoDocumentoCTe.NF)
                            {
                                if (string.IsNullOrWhiteSpace(documento.ModeloDocumentoFiscal) || (documento.ModeloDocumentoFiscal != "01" && documento.ModeloDocumentoFiscal != "04"))
                                    erros.Append("O modelo do documento (" + documento.ModeloDocumentoFiscal + ") invalido (01 ou 04). ");

                                if (string.IsNullOrWhiteSpace(documento.Serie) || documento.Serie.Length > 3)
                                    erros.Append("A serie do documento (" + documento.Serie + ") invalida. ");

                                if (string.IsNullOrWhiteSpace(documento.CFOP) || documento.CFOP.Length != 4)
                                    erros.Append("A CFOP do documento (" + documento.CFOP + ") invalida. ");

                                if (documento.Peso <= 0m)
                                    erros.Append("O peso do documento (" + documento.Peso.ToString("n2") + ") invalido. ");
                            }
                            else if (documento.Tipo == Dominio.Enumeradores.TipoDocumentoCTe.Outros)
                            {
                                if (string.IsNullOrWhiteSpace(documento.ModeloDocumentoFiscal) || (documento.ModeloDocumentoFiscal != "00" && documento.ModeloDocumentoFiscal != "99"))
                                    erros.Append("O modelo do documento (" + documento.ModeloDocumentoFiscal + ") invalido (00 ou 99). ");

                                if (string.IsNullOrWhiteSpace(documento.Descricao))
                                    erros.Append("A descricao do documento nao pode ser vazia ou nula. ");
                            }
                        }
                    }
                }
            }
            else
            {
                erros.Append("Nenhum documento informado. ");
            }

            if (cte.QuantidadesCarga != null && cte.QuantidadesCarga.Count > 0)
            {
                List<string> listaUnidades = new List<string>() { "00", "01", "02", "03", "04", "05" };

                foreach (var quantidade in cte.QuantidadesCarga)
                {
                    if (quantidade == null)
                    {
                        erros.Append("A quantidade da carga nao pode ser nula. ");
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(quantidade.Descricao))
                            erros.Append("A descricao da quantidade da carga nao pode ser vazia ou nula. ");

                        if (quantidade.Quantidade <= 0m)
                            erros.Append("A quantidade da quantidade da carga deve ser maior que zero. ");

                        if (!listaUnidades.Contains(quantidade.UnidadeMedida))
                            erros.Append("A unidade de medida (" + quantidade.UnidadeMedida + ") da quantidade da carga invalida. ");
                    }
                }
            }
            else
            {
                erros.Append("Nenhuma quantidade da carga informada. ");
            }

            if (cte.Seguros != null && cte.Seguros.Count > 0)
            {
                foreach (var seguro in cte.Seguros)
                {
                    if (seguro == null)
                    {
                        erros.Append("O seguro nao pode ser nulo. ");
                    }
                    else
                    {
                        if (seguro.NomeSeguradora != null && seguro.NomeSeguradora.Length > 30)
                            erros.Append("O nome da seguradora (" + seguro.NomeSeguradora + ") do seguro deve possuir ate 30 caracteres. ");

                        if (seguro.NumeroApolice != null && seguro.NumeroApolice.Length > 20)
                            erros.Append("O número da apolice (" + seguro.NumeroApolice + ") do seguro deve possuir ate 20 caracteres. ");

                        if (seguro.NumeroAverbacao != null && seguro.NumeroAverbacao.Length > 40)
                            erros.Append("O número da averbacao (" + seguro.NumeroAverbacao + ") do seguro deve possuir ate 40 caracteres. ");
                    }
                }
            }
            //Não é mais obrigatório na versão 3.0
            //else
            //{
            //    erros.Append("Seguro obrigatorio. ");
            //}

            return erros.ToString();
        }

        private bool AdicionarRegistroCTeIntegrado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, int numeroDaCarga, int numeroDaUnidade, string tipoOperacao, string nomeArquivo, string arquivo, Dominio.Enumeradores.TipoArquivoIntegracao tipoArquivo, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho = null, string codigoControleInternoCliente = null, string romaneio = null, string tipoVeiculo = null, string tipoCalculo = null, decimal valorDespesa = 0, Dominio.Enumeradores.StatusIntegracao status = Dominio.Enumeradores.StatusIntegracao.Pendente)
        {
            try
            {
                Repositorio.IntegracaoCTe repIntegracao = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.IntegracaoCTe integracao = new Dominio.Entidades.IntegracaoCTe();

                integracao.CTe = cte;
                integracao.Arquivo = arquivo;
                integracao.NumeroDaCarga = numeroDaCarga;
                integracao.NumeroDaUnidade = numeroDaUnidade;
                integracao.Status = status;
                integracao.TipoArquivo = tipoArquivo;
                integracao.NomeArquivo = nomeArquivo;
                integracao.Tipo = tipoIntegracao;
                integracao.CodigoControleInternoCliente = codigoControleInternoCliente;
                integracao.CodigoTipoOperacao = tipoOperacao;

                integracao.Romaneio = romaneio;
                integracao.TipoVeiculo = tipoVeiculo;
                integracao.TipoCalculo = tipoCalculo;
                integracao.ValorDespesa = valorDespesa;
                integracao.GerouCargaEmbarcador = false;

                repIntegracao.Inserir(integracao);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private Dominio.Entidades.CFOP ObterCFOP(Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);

            if (cte == null || empresa == null)
                return repCFOP.BuscarPorNumero(5932);

            Dominio.Entidades.CFOP cfop = null;

            if (cte.Recebedor != null && cte.Recebedor.CodigoIBGECidade > 0)
                cfop = ObterCFOPPorParticipante(cte, empresa, unidadeDeTrabalho, cte.Recebedor);

            if (cfop == null && cte.Destinatario != null && cte.Destinatario.CodigoIBGECidade > 0 && !cte.Destinatario.Exportacao)
                cfop = ObterCFOPPorParticipante(cte, empresa, unidadeDeTrabalho, cte.Destinatario);

            if (cfop == null)
                cfop = repCFOP.BuscarPorNumero(5932);

            return cfop;
        }

        private Dominio.Entidades.CFOP ObterCFOPPorParticipante(Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.CTe.Cliente participante)
        {
            if (participante == null || participante.CodigoIBGECidade <= 0)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Dominio.Entidades.Localidade localidadeTermino = repLocalidade.BuscarPorCodigoIBGE(participante.CodigoIBGECidade);
            Dominio.Entidades.Localidade localidadeInicio = repLocalidade.BuscarPorCodigoIBGE(cte.CodigoIBGECidadeInicioPrestacao);

            if (localidadeTermino == null || localidadeInicio == null)
                return null;

            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(unidadeDeTrabalho);
            int codigoAtividadeRemetente = cte.Remetente != null ? cte.Remetente.CodigoAtividade : 3;
            int codigoAtividadeDestinatario = participante.CodigoAtividade > 0 ? participante.CodigoAtividade : 3;

            Dominio.Entidades.Aliquota aliquota = repAliquota.BuscarParaCalculoDoICMS(empresa.Localidade.Estado.Sigla, localidadeInicio.Estado.Sigla, localidadeTermino.Estado.Sigla, codigoAtividadeRemetente, codigoAtividadeDestinatario);

            if (aliquota == null && cte.TomadorDoCTe != null)
                aliquota = repAliquota.BuscarParaCalculoDoICMS(empresa.Localidade.Estado.Sigla, localidadeInicio.Estado.Sigla, localidadeTermino.Estado.Sigla, cte.TomadorDoCTe.CodigoAtividade);

            return aliquota?.CFOP;
        }

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceCTe;
        }

        #endregion
    }
}
