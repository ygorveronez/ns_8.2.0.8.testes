using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace EmissaoCTe.Integracao
{
    public class NotaFiscalDeServicoEletronica : INotaFiscalDeServicoEletronica
    {
        #region Métodos Públicos

        public Retorno<int> IntegrarNFSe(Dominio.ObjetosDeValor.NFSe.NFSe nfse, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("IntegrarNFSe - NFSe: " + (nfse != null ? Newtonsoft.Json.JsonConvert.SerializeObject(nfse) : string.Empty));
                Servicos.Log.TratarErro("IntegrarNFSe - EmpresaAdministradora: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaAdministradora) ? cnpjEmpresaAdministradora : string.Empty));
                Servicos.Log.TratarErro("IntegrarNFSe - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                if (nfse == null)
                    return new Retorno<int>() { Mensagem = "A NFS-e não deve ser nula para a integração.", Status = false };

                if (nfse.Emitente == null)
                    return new Retorno<int>() { Mensagem = "O Emitente não pode ser nulo.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfse.Emitente.CNPJ));

                string erros = this.ValidarNFSe(nfse, empresa, cnpjEmpresaAdministradora, token, unidadeDeTrabalho);

                if (!string.IsNullOrEmpty(erros))
                    return new Retorno<int>() { Mensagem = erros, Status = false };

                unidadeDeTrabalho.Start();

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfseIntegrada = svcNFSe.GerarNFSePorObjeto(nfse, unidadeDeTrabalho);

                if (!this.AdicionarRegistroIntegrado(nfseIntegrada, "", JsonConvert.SerializeObject(nfse), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao, unidadeDeTrabalho))
                {
                    unidadeDeTrabalho.Rollback();

                    return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar a NFS-e na lista de integrações.", Status = false };
                }

                unidadeDeTrabalho.CommitChanges();

                string retorno = string.Empty;

                if (!svcNFSe.Emitir(nfseIntegrada, unidadeDeTrabalho))
                    retorno += "A NFS-e nº " + nfseIntegrada.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salva, porém, ocorreu uma falha ao emiti-la.";

                if (!this.AdicionarNFSeNaFilaDeConsulta(nfseIntegrada))
                    retorno += "A NFS-e nº " + nfseIntegrada.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salva, porém, não foi possível adicioná-la na fila de consulta.";

                if (string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = nfseIntegrada.Codigo };
                else
                    return new Retorno<int>() { Mensagem = retorno, Status = false, Objeto = nfseIntegrada.Codigo };

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha genérica ao integrar a NFS-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> CancelarNFSe(string cnpjEmpresaAdministradora, int codigoNFSe, string justificativa, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 20)
                    return new Retorno<int>() { Mensagem = "Justificativa inválida (" + justificativa + ").", Status = false };

                Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoNFSe> integracoes = repIntegracaoNFSe.Buscar(codigoNFSe, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao);

                if (integracoes.Count() <= 0)
                    return new Retorno<int>() { Mensagem = "NFS-e não encontrada.", Status = false };

                if (integracoes[0].NFSe.Empresa.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir NFS-es para esta empresa (" + integracoes[0].NFSe.Empresa.CNPJ + ").", Status = false };

                if (integracoes[0].NFSe.Empresa.EmpresaPai.Configuracao != null && integracoes[0].NFSe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (integracoes[0].NFSe.Status != Dominio.Enumeradores.StatusNFSe.Autorizado)
                    return new Retorno<int>() { Mensagem = "O status da NFS-e não permite o cancelamento da mesma.", Status = false };

                unidadeDeTrabalho.Start();

                integracoes[0].NFSe.JustificativaCancelamento = justificativa;

                repNFSe.Atualizar(integracoes[0].NFSe);

                if (!this.AdicionarRegistroIntegrado(integracoes[0].NFSe, "", "", Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoNFSe.Cancelamento, unidadeDeTrabalho))
                {
                    unidadeDeTrabalho.Rollback();

                    return new Retorno<int>() { Mensagem = "Ocorreu uma falha e não foi possível adicionar a NFS-e na lista de integrações.", Status = false, Objeto = integracoes[0].NFSe.Codigo };
                }

                if (!svcNFSe.Cancelar(integracoes[0].NFSe.Codigo, unidadeDeTrabalho))
                {
                    unidadeDeTrabalho.Rollback();

                    return new Retorno<int>() { Mensagem = "Ocorreu uma falha na comunicação com o Web Service do município e não foi possível cancelar a NFS-e.", Status = false, Objeto = integracoes[0].NFSe.Codigo };
                }

                unidadeDeTrabalho.CommitChanges();

                if (!this.AdicionarNFSeNaFilaDeConsulta(integracoes[0].NFSe))
                {
                    return new Retorno<int>() { Mensagem = "O cancelamento da NFS-e nº " + integracoes[0].NFSe.Numero.ToString() + " da empresa " + integracoes[0].NFSe.Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta.", Status = true, Objeto = integracoes[0].NFSe.Codigo };
                }
                else
                {
                    return new Retorno<int>() { Mensagem = "O cancelamento da NFS-e nº " + integracoes[0].NFSe.Numero.ToString() + " da empresa " + integracoes[0].NFSe.Empresa.CNPJ + " foi integrado com sucesso! ", Status = true, Objeto = integracoes[0].NFSe.Codigo };
                }
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao cancelar a NFS-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> ExcluirNFSe(string cnpjEmpresaAdministradora, int codigoNFSe, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoNFSe> integracoes = repIntegracaoNFSe.Buscar(codigoNFSe, Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao);

                if (integracoes.Count() <= 0)
                    return new Retorno<int>() { Mensagem = "NFS-e não encontrada.", Status = false };

                if (integracoes[0].NFSe.Empresa.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir NFS-es para esta empresa (" + integracoes[0].NFSe.Empresa.CNPJ + ").", Status = false };

                if (integracoes[0].NFSe.Empresa.EmpresaPai.Configuracao != null && integracoes[0].NFSe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (integracoes[0].NFSe.Status != Dominio.Enumeradores.StatusNFSe.EmDigitacao && integracoes[0].NFSe.Status != Dominio.Enumeradores.StatusNFSe.Rejeicao)
                    return new Retorno<int>() { Mensagem = "O status da NFS-e não permite a exclusão da mesma.", Status = false };

                unidadeDeTrabalho.Start();

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                svcNFSe.Deletar(integracoes[0].NFSe.Codigo, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new Retorno<int>() { Mensagem = "A NFS-e foi excluída com sucesso.", Status = true, Objeto = integracoes[0].NFSe.Codigo };

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorre uma falha ao excluir a NFS-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ValidarNFSe(Dominio.ObjetosDeValor.NFSe.NFSe nfse, Dominio.Entidades.Empresa empresa, string cnpjEmpresaAdministradora, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (empresa == null)
                return "A empresa (" + nfse.Emitente.CNPJ + ") não foi encontrada.";

            if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                return "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir NFS-es para esta empresa (" + empresa.CNPJ + ").";

            if (empresa.EmpresaPai.Configuracao != null && token != empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe)
                return "Token de acesso inválido.";

            if (empresa.Status != "A")
                return "A empresa (" + empresa.CNPJ + ") está inativa.";

            if (empresa.StatusFinanceiro == "B")
                return "A empresa (" + empresa.CNPJ + ") está com pendências, contate o setor de cadastros para maiores informações..";

            if (empresa.Configuracao == null)
                return "A empresa (" + empresa.CNPJ + ") não está configurada.";

            if (string.IsNullOrWhiteSpace(empresa.Configuracao.SerieRPSNFSe))
                return "A empresa (" + empresa.CNPJ + ") não possui uma série de RPS configurada para a emissão de NFS-e.";

            if (empresa.Configuracao.SerieNFSe == null)
                return "A empresa (" + empresa.CNPJ + ") não possui uma série configurada para a emissão de NFS-e.";

            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
            Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.CodigoIBGECidadePrestacaoServico, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);
            Dominio.Entidades.NaturezaNFSe naturezaMultiCTe = servicoNFSe.ObterNaturezaNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.CodigoIBGECidadePrestacaoServico, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);

            //Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            //List<Dominio.Entidades.NFSe> nfsesPendentes = repNFSe.BuscarNFSesPendentes(empresa.Codigo);

            //if (nfsesPendentes.Count > 0)
            //{
            //    string r = "Existem NFS-es pendentes para a empresa (" + empresa.CNPJ + "), não sendo possível emitir uma nova NFS-e até que as mesmas sejam autorizadas, canceladas ou excluídas. Protocolos: ";

            //    foreach (Dominio.Entidades.NFSe nfsePendente in nfsesPendentes)
            //        r += nfsePendente.Codigo.ToString() + ", ";

            //    return r.Substring(0, r.Length - 2) + ".";
            //}

            StringBuilder erros = new StringBuilder();

            if (nfse.Tomador == null)
                erros.Append("Tomador não pode ser nulo. ");
            else if (nfse.Tomador.CodigoAtividade <= 0)
                erros.Append("Atividade do tomador inválida. ");

            if (nfse.Intermediario != null && nfse.Intermediario.CodigoAtividade <= 0)
                erros.Append("Atividade do intermediário inválida. ");

            if (nfse.CodigoIBGECidadePrestacaoServico <= 0)
                erros.Append("Código da cidade de prestação do serviço inválido. ");

            if (nfse.Natureza != null || naturezaMultiCTe == null)
            {
                if (nfse.Natureza == null)
                {
                    erros.Append("Natureza é vazia ou nula. ");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(nfse.Natureza.Descricao))
                        erros.Append("Descrição da natureza é inválida. ");

                    if (nfse.Natureza.Numero <= 0)
                        erros.Append("Número da natureza é inválido. ");
                }
            }

            if (nfse.Itens == null || nfse.Itens.Count <= 0 || nfse.Itens.Count > 1)
            {
                erros.Append("Quantidade de itens inválida, o total suportado é de 1 item por nota. ");
            }
            else
            {
                foreach (var item in nfse.Itens)
                {
                    if (item == null)
                        erros.Append("Item é vazio ou nulo. ");

                    if (item.CodigoIBGECidade <= 0)
                        erros.Append("Código da cidade do item inválido. ");

                    if (item.CodigoIBGECidadeIncidencia <= 0)
                        erros.Append("Código da cidade de incidência do item inválido. ");

                    if (!item.ServicoPrestadoNoPais && item.CodigoPaisPrestacaoServico <= 0)
                        erros.Append("Código do país de prestação do serviço inválido. ");

                    if (item.Quantidade <= 0)
                        erros.Append("Quantidade do item inválida. ");

                    if (item.ValorServico <= 0)
                        erros.Append("Valor do serviço do item inválido. ");                  
                   
                    if (item.Servico != null || servicoMultiCTe == null)
                    {
                        if (item.ValorTotal <= 0)
                            erros.Append("Valor total do item inválido. ");

                        if (item.Servico == null)
                        {
                            erros.Append("Serviço do item é vazio ou nulo. ");
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(item.Servico.Descricao))
                                erros.Append("Descrição do serviço do item inválida. ");

                            if (string.IsNullOrWhiteSpace(item.Servico.Numero))
                                erros.Append("Número do serviço do item inválido. ");

                            //if (item.Servico.Aliquota <= 0)
                            //    erros.Append("Alíquota do serviço do item inválida. ");
                        }
                    }
                }
            }

            if (servicoMultiCTe == null && nfse.ValorServicos <= 0)
                erros.Append("Valor dos serviços inválido. ");

            return erros.ToString();
        }

        private bool AdicionarNFSeNaFilaDeConsulta(Dominio.Entidades.NFSe nfse)
        {
            try
            {
                string postData = "CodigoNFSe=" + nfse.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["WebServiceConsultaCTe"], "IntegracaoNFSe/AdicionarNaFilaDeConsulta"));

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

        private bool AdicionarRegistroIntegrado(Dominio.Entidades.NFSe nfse, string nomeArquivo, string arquivo, Dominio.Enumeradores.TipoArquivoIntegracao tipoArquivo, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.IntegracaoNFSe repIntegracao = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);

                Dominio.Entidades.IntegracaoNFSe integracao = new Dominio.Entidades.IntegracaoNFSe();

                integracao.NFSe = nfse;
                integracao.Arquivo = arquivo;
                integracao.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                integracao.TipoArquivo = tipoArquivo;
                integracao.NomeArquivo = nomeArquivo;
                integracao.Tipo = tipoIntegracao;
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

        #endregion
    }
}
