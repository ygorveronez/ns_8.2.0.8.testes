using Dominio.ObjetosDeValor.Enumerador;
using EmissaoCTe.Integracao.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace EmissaoCTe.Integracao
{
    public class ManifestoEletronicoDeDocumentosFiscais : BaseService, IManifestoEletronicoDeDocumentosFiscais
    {
        #region Métodos Públicos

        public Retorno<int> IntegrarMDFePorCTes(int[] codigosCTes, string observacaoMDFe, string cnpjEmpresaEmitente, string cnpjEmpresaPai, int numeroUnidade, string token, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - Codigos CTe: " + Newtonsoft.Json.JsonConvert.SerializeObject(codigosCTes));
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - EmpresaEmitente: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaEmitente) ? cnpjEmpresaEmitente : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - EmpresaPai: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaPai) ? cnpjEmpresaPai : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - Compra Vale Pedagio: " + Newtonsoft.Json.JsonConvert.SerializeObject(valePedagioCompra));
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - produtoPredominante: " + Newtonsoft.Json.JsonConvert.SerializeObject(produtoPredominante));
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - listaCIOT: " + Newtonsoft.Json.JsonConvert.SerializeObject(listaCIOT));
                Servicos.Log.TratarErro("IntegrarMDFePorCTes - informacoesPagamento: " + Newtonsoft.Json.JsonConvert.SerializeObject(informacoesPagamento));

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjEmpresaEmitente));

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaPai))
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaPai + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + cnpjEmpresaEmitente + ").", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não está configurada.", Status = false };

                if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não esta liberada para emissão de MDF-e", Status = false };

                if (empresa.Configuracao.SerieMDFe == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não possui uma série configurada para a emissão de MDF-e.", Status = false };

                codigosCTes = codigosCTes.Distinct().ToArray();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(empresa.Codigo, codigosCTes);

                if (ctes.Count() != codigosCTes.Count())
                    return new Retorno<int>() { Mensagem = "Alguns CT-es não foram encontrados para a emissão do MDF-e.", Status = false };

                retorno = this.ValidarMDFe(unidadeDeTrabalho, empresa, ctes, true, valePedagioCompra);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = retorno, Status = false };

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = servicoMDFe.GerarMDFePorCTes(empresa, ctes, unidadeDeTrabalho, null, null, string.Empty, null, null, null, 0, observacaoMDFe, null, null, null, null, true, null, valePedagioCompra, null, null, null, null, null, null, produtoPredominante, listaCIOT, informacoesPagamento);

                bool pendenteCompraValePedagio = false;
                //Se possui compra de vale pedágio pendente não emite
                Repositorio.ValePedagioMDFeCompra repValePedagioMDFeCompra = new Repositorio.ValePedagioMDFeCompra(unidadeDeTrabalho);
                pendenteCompraValePedagio = repValePedagioMDFeCompra.BuscarListaPorMDFeTipoStatus(mdfe.Codigo, Dominio.Enumeradores.TipoIntegracaoValePedagio.Autorizacao, Dominio.Enumeradores.StatusIntegracaoValePedagio.Pendente).Count > 0;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, $"Integração de MDF-e nº{mdfe.Numero}, método: IntegrarMDFePorCTes", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                if (!this.AdicionarRegistroIntegrado(mdfe, "", numeroUnidade.ToString(), "", "", Dominio.Enumeradores.TipoArquivoIntegracao.CTe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, DateTime.MinValue, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi integrado, porém, não foi possível salvar o registro de integração. ";

                if (pendenteCompraValePedagio)
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio;
                    repMDFe.Atualizar(mdfe);

                }
                else
                {
                    if (!servicoMDFe.Emitir(mdfe, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                    if (!this.AdicionarMDFeNaFilaDeConsulta(mdfe))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";
                }

                retorno += "MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = mdfe.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();

                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha genérica ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarMDFePorTxt(int codigoEmpresaPai, string nomeArquivo, string arquivoTexto)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string[] dadosArquivo = arquivoTexto.Split(';');

                int numeroCarga, numeroUnidade = 0;

                int.TryParse(dadosArquivo[1], out numeroCarga);
                int.TryParse(dadosArquivo[2], out numeroUnidade);

                DateTime dataEncerramento;

                DateTime.TryParseExact(dadosArquivo[3], "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                if (numeroCarga <= 0)
                    return new Retorno<int>() { Mensagem = "O número da carga é inválido. ", Status = false };

                if (numeroUnidade <= 0)
                    return new Retorno<int>() { Mensagem = "O número da unidade é inválido. ", Status = false };

                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                if (repIntegracaoMDFe.ContarMDFeExistente(codigoEmpresaPai, numeroCarga.ToString(), numeroUnidade.ToString()) > 0)
                    return new Retorno<int>() { Mensagem = "Já existe um MDF-e para esta unidade / carga.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(dadosArquivo[0]);

                if (empresa != null && empresa.EmpresaPai != null && empresa.EmpresaPai.Codigo == codigoEmpresaPai)
                {
                    if (empresa.Configuracao == null)
                        return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada. ", Status = false };

                    if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                        return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não esta liberada para emissão de MDF-e", Status = false };

                    if (empresa.Configuracao.SerieMDFe == null)
                        return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não possui uma série cadastrada para a emissão de MDF-e.", Status = false };

                    Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = servicoMDFe.GerarMDFePorCargaEUnidade(empresa, numeroCarga, numeroUnidade);

                    if (!servicoMDFe.Emitir(mdfe, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                    if (!this.AdicionarMDFeNaFilaDeConsulta(mdfe))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                    if (!this.AdicionarRegistroIntegrado(mdfe, numeroCarga.ToString(), numeroUnidade.ToString(), nomeArquivo, arquivoTexto, Dominio.Enumeradores.TipoArquivoIntegracao.TXT, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, dataEncerramento, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                    retorno += "MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";
                }
                else
                {
                    return new Retorno<int>() { Mensagem = retorno + "A empresa (" + dadosArquivo[0] + ") não foi encontrada. ", Status = false };
                }

                return new Retorno<int>() { Mensagem = retorno, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarMDFe(int codigoEmpresaPai, int numeroCarga, int numeroUnidade, string dataHoraEncerramento, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (numeroCarga <= 0)
                    return new Retorno<int>() { Mensagem = "O número da carga é inválido.", Status = false };

                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                if (repIntegracaoMDFe.ContarMDFeExistente(codigoEmpresaPai, numeroCarga.ToString(), numeroUnidade.ToString()) > 0)
                    return new Retorno<int>() { Mensagem = "Já existe um MDF-e para esta unidade / carga.", Status = false };

                DateTime dataEncerramento;

                DateTime.TryParseExact(dataHoraEncerramento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.IntegracaoCTe integracao = repIntegracaoCTe.BuscarPrimeiroRegistro(codigoEmpresaPai, numeroCarga, numeroUnidade, Dominio.Enumeradores.TipoIntegracao.Emissao, new Dominio.Enumeradores.StatusIntegracao[] { Dominio.Enumeradores.StatusIntegracao.Integrado, Dominio.Enumeradores.StatusIntegracao.Impresso }, "A");

                Dominio.Entidades.Empresa empresa = integracao != null ? integracao.CTe.Empresa : null;

                if (empresa != null && empresa.EmpresaPai != null && empresa.EmpresaPai.Codigo == codigoEmpresaPai)
                {
                    if (empresa.Configuracao == null)
                        return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada. ", Status = false };

                    if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                        return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não esta liberada para emissão de MDF-e", Status = false };

                    if (empresa.Configuracao.SerieMDFe == null)
                        return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não possui uma série cadastrada para a emissão de MDF-e.", Status = false };

                    Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = servicoMDFe.GerarMDFePorCargaEUnidade(empresa, numeroCarga, numeroUnidade, produtoPredominante, listaCIOT, informacoesPagamento);

                    if (!servicoMDFe.Emitir(mdfe, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                    if (!this.AdicionarMDFeNaFilaDeConsulta(mdfe))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                    if (!this.AdicionarRegistroIntegrado(mdfe, numeroCarga.ToString(), numeroUnidade.ToString(), "", "", Dominio.Enumeradores.TipoArquivoIntegracao.NFe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, dataEncerramento, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                    retorno += "MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                    return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = mdfe.Codigo };
                }
                else
                {
                    return new Retorno<int>() { Mensagem = retorno + "A empresa não foi encontrada ou não existem integrações para esta carga/unidade. ", Status = false };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarMDFePorObjeto(Dominio.ObjetosDeValor.MDFe.MDFe mdfe, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            string retorno = string.Empty;
            try
            {
                retorno = this.ValidarMDFe(mdfe);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = retorno, Status = false };

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(mdfe.Emitente.CNPJ));

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + mdfe.Emitente.CNPJ + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + empresa.CNPJ + ").", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada. ", Status = false };

                if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não esta liberada para emissão de MDF-e", Status = false };

                if (empresa.Configuracao.SerieMDFe == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não possui uma série cadastrada para a emissão de MDF-e.", Status = false };

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeIntegrado = servicoMDFe.GerarMDFePorObjeto(empresa, mdfe, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                if (!servicoMDFe.Emitir(mdfeIntegrado, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                if (!this.AdicionarMDFeNaFilaDeConsulta(mdfeIntegrado))
                    retorno += "O MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                if (!this.AdicionarRegistroIntegrado(mdfeIntegrado, mdfe.NumeroCarga.ToString(), "", "", JsonConvert.SerializeObject(mdfe), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, DateTime.MinValue, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                retorno += "MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = mdfeIntegrado.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Rollback();

                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarMDFePorChaveCTe(Dominio.ObjetosDeValor.MDFe.MDFe mdfe, string cnpjEmpresaAdministradora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            string retorno = string.Empty;
            try
            {
                retorno = this.ValidarMDFe(mdfe);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = retorno, Status = false };

                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(mdfe.Emitente.CNPJ));

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + mdfe.Emitente.CNPJ + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai == null || empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaAdministradora))
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + empresa.CNPJ + ").", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada. ", Status = false };

                if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não esta liberada para emissão de MDF-e", Status = false };

                if (empresa.Configuracao.SerieMDFe == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não possui uma série cadastrada para a emissão de MDF-e.", Status = false };

                if (mdfe?.ControlaNumeroSerieForaDoEmbarcador ?? false)
                {
                    if (mdfe.NumeroMDFe <= 0)
                        return new Retorno<int>() { Mensagem = "Quando o controle de número e série fora do embarcador está ativo, o número do MDF-e é obrigatório.", Status = false };

                    if (mdfe.SerieMDFe <= 0)
                        return new Retorno<int>() { Mensagem = "Quando o controle de número e série fora do embarcador está ativo, a série do MDF-e é obrigatória.", Status = false };
                }

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                if (mdfe.Emitente.Atualizar)
                {
                    string erroValidacao = this.ValidarDadosParaAtualizacao(mdfe.Emitente);
                    if (!string.IsNullOrWhiteSpace(erroValidacao))
                        return new Retorno<int>() { Mensagem = erroValidacao, Status = false };

                    empresa = Servicos.Empresa.AtualizarEmpresa(empresa.Codigo, mdfe.Emitente, unidadeDeTrabalho);
                }

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeIntegrado = servicoMDFe.GerarMDFePorChaveCTe(empresa, mdfe, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                if (!servicoMDFe.Emitir(mdfeIntegrado, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                if (!this.AdicionarMDFeNaFilaDeConsulta(mdfeIntegrado))
                    retorno += "O MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                if (!this.AdicionarRegistroIntegrado(mdfeIntegrado, mdfe.NumeroCarga.ToString(), "", "", JsonConvert.SerializeObject(mdfe), Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, DateTime.MinValue, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                retorno += "MDF-e nº " + mdfeIntegrado.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = mdfeIntegrado.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Rollback();

                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarMDFePorNFe(int codigoEmpresaPai, int numeroCarga, int numeroUnidade, string dataHoraEncerramento, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (numeroCarga <= 0)
                    return new Retorno<int>() { Mensagem = "O número da carga é inválido. ", Status = false };

                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                if (repIntegracaoMDFe.ContarMDFeExistente(codigoEmpresaPai, numeroCarga.ToString(), numeroUnidade.ToString()) > 0)
                    return new Retorno<int>() { Mensagem = "Já existe um MDF-e para esta unidade / carga.", Status = false };

                DateTime dataEncerramento;
                DateTime.TryParseExact(dataHoraEncerramento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                Repositorio.IntegracaoMDFeArquivo repIntegracaoMDFeArquivo = new Repositorio.IntegracaoMDFeArquivo(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoMDFeArquivo> arquivos = repIntegracaoMDFeArquivo.Buscar(numeroCarga, numeroUnidade);

                if (arquivos.Count() <= 0)
                    return new Retorno<int>() { Mensagem = "Nenhum arquivo para esta unidade / carga.", Status = false };

                List<object> notasFiscais = new List<object>();

                foreach (Dominio.Entidades.IntegracaoMDFeArquivo arquivo in arquivos)
                {
                    MemoryStream msArquivo = new MemoryStream(Encoding.Default.GetBytes(arquivo.Arquivo));

                    notasFiscais.Add(MultiSoftware.NFe.Servicos.Leitura.Ler(msArquivo));

                    msArquivo.Dispose();
                }

                Dominio.Entidades.Empresa empresa = arquivos[0].Empresa;

                if (empresa.EmpresaPai == null || empresa.EmpresaPai.Codigo != codigoEmpresaPai)
                    return new Retorno<int>() { Mensagem = retorno + "A empresa administradora não foi encontrada ou não está vinculada para autorização de MDF-es para esta empresa. ", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não está configurada. ", Status = false };

                if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                    return new Retorno<int>() { Mensagem = "A empresa (" + empresa.CNPJ + ") não esta liberada para emissão de MDF-e", Status = false };

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = servicoMDFe.GerarMDFePorNotasFiscais(empresa, notasFiscais, produtoPredominante, listaCIOT, informacoesPagamento);

                if (!servicoMDFe.Emitir(mdfe, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                if (!this.AdicionarMDFeNaFilaDeConsulta(mdfe))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                if (!this.AdicionarRegistroIntegrado(mdfe, numeroCarga.ToString(), numeroUnidade.ToString(), "", "", Dominio.Enumeradores.TipoArquivoIntegracao.NFe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, dataEncerramento, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                retorno += "MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarMDFePorCodigoCTes(int[] codigosCTes, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> veiculos, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> motoristas, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> reboques, string observacaoFisco, string observacaoContribuinte, string token, int numeroUnidade, string numeroUnidadeString, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros, List<Dominio.ObjetosDeValor.ValePedagioIntegracao> valesPedagio, Dominio.ObjetosDeValor.MDFe.DadosMDFe dadosMDFe, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> listaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Codigos CTe: " + Newtonsoft.Json.JsonConvert.SerializeObject(codigosCTes));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Veiculos: " + Newtonsoft.Json.JsonConvert.SerializeObject(veiculos));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Motoristas: " + Newtonsoft.Json.JsonConvert.SerializeObject(motoristas));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Reboques: " + Newtonsoft.Json.JsonConvert.SerializeObject(reboques));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Seguros: " + Newtonsoft.Json.JsonConvert.SerializeObject(seguros));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Vales Pedagio: " + Newtonsoft.Json.JsonConvert.SerializeObject(valesPedagio));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - DadosMDFe: " + Newtonsoft.Json.JsonConvert.SerializeObject(dadosMDFe));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - NfesGlobalizadas: " + Newtonsoft.Json.JsonConvert.SerializeObject(nfesGlobalizadas));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - produtoPredominante: " + Newtonsoft.Json.JsonConvert.SerializeObject(produtoPredominante));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - NumeroUnidade: " + (numeroUnidade != null ? numeroUnidade.ToString() : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - NumeroUnidadeString: " + (!string.IsNullOrWhiteSpace(numeroUnidadeString) ? numeroUnidadeString : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - ObservacaoFisco: " + (!string.IsNullOrWhiteSpace(observacaoFisco) ? observacaoFisco : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - ObservacaoContribuinte: " + (!string.IsNullOrWhiteSpace(observacaoContribuinte) ? observacaoContribuinte : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - listaCIOT: " + Newtonsoft.Json.JsonConvert.SerializeObject(listaCIOT));
                Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - informacoesPagamento: " + Newtonsoft.Json.JsonConvert.SerializeObject(informacoesPagamento));


                var arquivo = string.Concat(" - Veiculos ", JsonConvert.SerializeObject(veiculos), " - Motoristas: ", JsonConvert.SerializeObject(motoristas), " - Reboques: ", JsonConvert.SerializeObject(reboques), " - CTes:", JsonConvert.SerializeObject(codigosCTes), " - Seguros:", JsonConvert.SerializeObject(seguros), " - ObsFisco: ", JsonConvert.SerializeObject(observacaoFisco), " - ObsCont: ", JsonConvert.SerializeObject(observacaoContribuinte), " - Token: ", JsonConvert.SerializeObject(token), " - Unidade: ", JsonConvert.SerializeObject(numeroUnidade));

                string configMDFeAgrupaCTesMesmaCarga = ConfigurationManager.AppSettings["MDFeAgrupaCTesMesmaCarga"];
                if (configMDFeAgrupaCTesMesmaCarga == null || configMDFeAgrupaCTesMesmaCarga == "")
                    configMDFeAgrupaCTesMesmaCarga = "NAO";

                string validaAverbacoesCTe = ConfigurationManager.AppSettings["ValidaAverbacoesCTe"];
                if (validaAverbacoesCTe == null || validaAverbacoesCTe == "")
                    validaAverbacoesCTe = "NAO";

                string exigirCadastroVeiculo = System.Configuration.ConfigurationManager.AppSettings["CTeExigeVeiculoCadastro"];
                if (exigirCadastroVeiculo == null || exigirCadastroVeiculo == "")
                    exigirCadastroVeiculo = "NAO";

                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = null;

                if (codigosCTes != null && codigosCTes.Count() > 0)
                {
                    codigosCTes = codigosCTes.Distinct().ToArray();

                    ctes = repCTe.BuscarPorCodigo(0, codigosCTes);

                    if (ctes.Count() != codigosCTes.Count())
                        return new Retorno<int>() { Mensagem = "Alguns CT-es não foram encontrados para a emissão do MDF-e.", Status = false };
                }
                else if (nfesGlobalizadas == null || nfesGlobalizadas.Count() == 0)
                    return new Retorno<int>() { Mensagem = "Nenhum documento recebido para a geração do MDF-e.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = null;

                if (dadosMDFe != null && !string.IsNullOrWhiteSpace(dadosMDFe.CNPJEmissor))
                {
                    if (!Utilidades.Validate.ValidarCNPJ(dadosMDFe.CNPJEmissor))
                        return new Retorno<int>() { Mensagem = "CNPJ Emissor (" + dadosMDFe.CNPJEmissor + ") é inválido.", Status = false };

                    empresa = repEmpresa.BuscarPorCNPJ(dadosMDFe.CNPJEmissor);
                }
                else
                    empresa = repEmpresa.BuscarPorCodigo(ctes.FirstOrDefault().Empresa.Codigo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = null;

                if (empresa == null)
                {
                    if (ctes.Count > 0)
                        return new Retorno<int>() { Mensagem = "A empresa (" + ctes.FirstOrDefault().Empresa.CNPJ + ") não foi encontrada.", Status = false };
                    if (dadosMDFe != null && !string.IsNullOrWhiteSpace(dadosMDFe.CNPJEmissor))
                        return new Retorno<int>() { Mensagem = "A empresa (" + dadosMDFe.CNPJEmissor + ") não foi encontrada.", Status = false };
                    else
                        return new Retorno<int>() { Mensagem = "A empresa emitente não foi encontrada.", Status = false };
                }

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa emitente não está configurada.", Status = false };

                if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                    return new Retorno<int>() { Mensagem = "A empresa emitente não esta liberada para emissão de MDF-e", Status = false };

                if (empresa.Configuracao.SerieMDFe == null)
                    return new Retorno<int>() { Mensagem = "A empresa emitente não possui uma série configurada para a emissão de MDF-e.", Status = false };

                retorno = this.ValidarMDFe(unitOfWork, empresa, ctes, false);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = retorno, Status = false };

                if (veiculos == null || veiculos.Count == 0)
                    return new Retorno<int>() { Mensagem = "Sem veículos para geração do MDF-e.", Status = false };
                else
                if (veiculos.Count > 1)
                    return new Retorno<int>() { Mensagem = "Mais de uma placa enviada como veículo, demais placas devem ser enviadas como Reboque.", Status = false };
                else
                {
                    if (string.IsNullOrWhiteSpace(veiculos[0].Placa))
                        return new Retorno<int>() { Mensagem = "Obrigatório informar a placa de todos Veículos.", Status = false };

                    veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, veiculos[0].Placa);
                    if (exigirCadastroVeiculo == "SIM")
                    {
                        if (veiculo == null)
                        {
                            Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Placa " + veiculos[0].Placa + " não possui cadastro para o transportador " + empresa.CNPJ);
                            return new Retorno<int>() { Mensagem = "Placa " + veiculos[0].Placa + " não possui cadastro para o transportador " + empresa.CNPJ, Status = false };
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(veiculos[0].RENAVAM))
                            return new Retorno<int>() { Mensagem = "Obrigatório informar o Renavam de todos Veículos.", Status = false };
                        else if (veiculos[0].RENAVAM.Length < 9 || veiculos[0].RENAVAM.Length > 11)
                            return new Retorno<int>() { Mensagem = "Renavam Veículos deve possuír entre 9 e 11 digitos.", Status = false };

                        if (veiculos[0].ProprietarioTerceiro != null)
                        {
                            if (string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(veiculos[0].ProprietarioTerceiro.CPFCNPJ)))
                                return new Retorno<int>() { Mensagem = "Obrigatório informar o CNPJ do proprietário terceiro.", Status = false };
                            if (string.IsNullOrWhiteSpace(veiculos[0].ProprietarioTerceiro.RazaoSocial))
                                return new Retorno<int>() { Mensagem = "Obrigatório informar a Razão Social do proprietário terceiro.", Status = false };
                            if (string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(veiculos[0].ProprietarioTerceiro.RGIE)))
                                return new Retorno<int>() { Mensagem = "Obrigatório informar a Inscrição Estadual do proprietário terceiro.", Status = false };
                            if (veiculos[0].ProprietarioTerceiro.CodigoIBGECidade <= 0)
                                return new Retorno<int>() { Mensagem = "Obrigatório informar o IBGE do proprietário terceiro.", Status = false };
                            if (string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(veiculos[0].RNTRC)))
                                return new Retorno<int>() { Mensagem = "Obrigatório informar a RNTRC do veículo quando proprietário terceiro.", Status = false };
                        }
                    }
                }



                if (motoristas == null || motoristas.Count == 0)
                {
                    Dominio.Entidades.Usuario motoristaPrincipal = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                    if (veiculo == null || motoristaPrincipal == null)
                    {
                        if (veiculo != null)
                        {
                            Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Placa " + veiculo.Placa + " não possui motorista cadastrado no transportador " + empresa.CNPJ);
                            return new Retorno<int>() { Mensagem = "Placa " + veiculo.Placa + " não possui motorista cadastrado no transportador " + empresa.CNPJ, Status = false };
                        }
                        else
                        {
                            Servicos.Log.TratarErro("IntegrarMDFePorCodigoCTes - Sem motorista cadastrado no transportador" + empresa.CNPJ);
                            return new Retorno<int>() { Mensagem = "Sem motorista cadastrado no transportador" + empresa.CNPJ, Status = false };
                        }
                    }
                    else
                    {
                        if (motoristas == null)
                            motoristas = new List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao>();

                        Dominio.ObjetosDeValor.MotoristaMDFeIntegracao motoristaVeiculo = new Dominio.ObjetosDeValor.MotoristaMDFeIntegracao();
                        motoristaVeiculo.CPF = motoristaPrincipal.CPF;
                        motoristaVeiculo.Nome = motoristaPrincipal.Nome;

                        motoristas.Add(motoristaVeiculo);

                        if (veiculo.VeiculoMotoristas != null && veiculo.VeiculoMotoristas.Count > 0)
                        {
                            foreach (Dominio.Entidades.VeiculoMotoristas motoristaAdicional in veiculo.VeiculoMotoristas)
                            {
                                Dominio.ObjetosDeValor.MotoristaMDFeIntegracao motoristaVeiculoAdicional = new Dominio.ObjetosDeValor.MotoristaMDFeIntegracao();
                                motoristaVeiculoAdicional.CPF = motoristaAdicional.Motorista.CPF;
                                motoristaVeiculoAdicional.Nome = motoristaAdicional.Motorista.Nome;

                                motoristas.Add(motoristaVeiculoAdicional);
                            }
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < motoristas.Count(); i++)
                    {
                        if (string.IsNullOrWhiteSpace(motoristas[i].CPF))
                            return new Retorno<int>() { Mensagem = "Obrigatório informar o CPF de todos Motoristas.", Status = false };
                        if (string.IsNullOrWhiteSpace(motoristas[i].Nome))
                            return new Retorno<int>() { Mensagem = "Obrigatório informar o Nome de todos Motoristas.", Status = false };
                    }
                }

                if (reboques != null && reboques.Count > 0)
                {
                    for (var i = 0; i < reboques.Count(); i++)
                    {
                        if (string.IsNullOrWhiteSpace(reboques[i].Placa))
                            return new Retorno<int>() { Mensagem = "Obrigatório informar a placa de todos Reboques.", Status = false };

                        veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, reboques[i].Placa);
                        if (exigirCadastroVeiculo == "SIM")
                        {
                            if (veiculo == null)
                                return new Retorno<int>() { Mensagem = "Placa " + reboques[i].Placa + " não possui cadastro para o transportador " + empresa.CNPJ, Status = false };
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(reboques[i].RENAVAM))
                                return new Retorno<int>() { Mensagem = "Obrigatório informar o Renavam de todos Reboques.", Status = false };
                            else if (reboques[i].RENAVAM.Length < 9 || reboques[i].RENAVAM.Length > 11)
                                return new Retorno<int>() { Mensagem = "Renavam Reboques deve possuír entre 9 e 11 digitos.", Status = false };


                            if (reboques[i].ProprietarioTerceiro != null)
                            {
                                if (string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(reboques[i].ProprietarioTerceiro.CPFCNPJ)))
                                    return new Retorno<int>() { Mensagem = "Obrigatório informar o CNPJ do proprietário terceiro.", Status = false };
                                if (string.IsNullOrWhiteSpace(reboques[i].ProprietarioTerceiro.RazaoSocial))
                                    return new Retorno<int>() { Mensagem = "Obrigatório informar a Razão Social do proprietário terceiro.", Status = false };
                                if (string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(reboques[i].ProprietarioTerceiro.RGIE)))
                                    return new Retorno<int>() { Mensagem = "Obrigatório informar a Inscrição Estadual do proprietário terceiro.", Status = false };
                                if (reboques[i].ProprietarioTerceiro.CodigoIBGECidade <= 0)
                                    return new Retorno<int>() { Mensagem = "Obrigatório informar o IBGE do proprietário terceiro.", Status = false };
                                if (string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(reboques[i].RNTRC)))
                                    return new Retorno<int>() { Mensagem = "Obrigatório informar a RNTRC do veículo quando proprietário terceiro.", Status = false };
                            }
                        }
                    }
                }

                string bloquearIntegracaoDuplicada = System.Configuration.ConfigurationManager.AppSettings["BloquearIntegracaoMDFeDuplicada"];
                if (bloquearIntegracaoDuplicada == "SIM" && ctes != null && ctes.Count > 0)
                {
                    Servicos.Log.TratarErro("Validando duplicidade MDFe integracao ref. CTe codigo: " + ctes.FirstOrDefault().Codigo, "DuplicidadeIntegracaoMDFe");

                    Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentosMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
                    Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unitOfWork);

                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeAnterior = repDocumentosMDFe.BuscarMDFesPorCTeEStatus(ctes.FirstOrDefault().Codigo, Dominio.Enumeradores.StatusMDFe.Autorizado);

                    if (mdfeAnterior != null)
                    {
                        Servicos.Log.TratarErro("MDFe " + mdfeAnterior.Chave + " duplicidade integracao ref. CTe codigo: " + ctes.FirstOrDefault().Codigo, "DuplicidadeIntegracaoMDFe");

                        List<Dominio.Entidades.IntegracaoMDFe> listaIntegracaoMDFe = repIntegracaoMDFe.BuscarPorMDFeETipo(mdfeAnterior.Codigo, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao);

                        if (listaIntegracaoMDFe != null && listaIntegracaoMDFe.Count > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(listaIntegracaoMDFe.FirstOrDefault().Arquivo) && arquivo == listaIntegracaoMDFe.FirstOrDefault().Arquivo)
                                return new Retorno<int>() { Mensagem = "Solicitação de MDFe recebida em duplicidade. MDFe " + mdfeAnterior.Chave + " já autorizado para os dados enviados.", Status = false, Objeto = mdfeAnterior.Codigo };
                            else
                                Servicos.Log.TratarErro("Integração em duplicidade MDFe integração diferente da integração anterior.", "DuplicidadeIntegracao");
                        }
                        else
                            Servicos.Log.TratarErro("Não localizada integração referente MDFe anterior " + mdfeAnterior.Codigo.ToString(), "DuplicidadeIntegracao");
                    }
                }

                Dominio.Entidades.Localidade localidadeCarregamento = null;
                if (dadosMDFe != null)
                {
                    if (dadosMDFe.Numero > 0)
                    {
                        if (dadosMDFe.Serie == 0)
                            return new Retorno<int>() { Mensagem = "Obrigatório informar a série quando o número do MDFe é informado.", Status = false };

                        var serieMDFe = repSerie.BuscarPorEmpresaNumeroTipo(empresa.Codigo, dadosMDFe.Serie, Dominio.Enumeradores.TipoSerie.MDFe);

                        if (serieMDFe == null)
                            return new Retorno<int>() { Mensagem = "Série " + dadosMDFe.Serie.ToString() + " não possui configuração.", Status = false };

                        var mdfeAnterior = repMDFe.BuscarPorNumeroESerie(empresa.Codigo, dadosMDFe.Numero, serieMDFe);
                        if (mdfeAnterior != null)
                            return new Retorno<int>() { Mensagem = "Já existe MDFe com mesmo número " + mdfeAnterior.Numero + " e série " + mdfeAnterior.Serie.Numero + " para a empresa " + empresa.CNPJ + ".", Status = false };
                    }

                    if (dadosMDFe.IBGEOrigem > 0)
                    {
                        localidadeCarregamento = repLocalidade.BuscarPorCodigoIBGE(dadosMDFe.IBGEOrigem);
                        if (localidadeCarregamento == null)
                            return new Retorno<int>() { Mensagem = "IBGE Origem (" + dadosMDFe.IBGEOrigem + ") não localizado.", Status = false };
                    }
                }

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

                string cargaAgrupar = string.Empty;
                if (configMDFeAgrupaCTesMesmaCarga == "SIM" && ctes != null && ctes.Count > 0)
                {
                    cargaAgrupar = !string.IsNullOrWhiteSpace(numeroUnidadeString) ? numeroUnidadeString : numeroUnidade.ToString();
                    if (!string.IsNullOrWhiteSpace(cargaAgrupar) && cargaAgrupar != "0")
                    {
                        System.Threading.Thread.Sleep(2000);
                        if (!VerificaStatusMDFeAnterior(cargaAgrupar, empresa.Codigo, unitOfWork))
                            return new Retorno<int>() { Mensagem = "MDF-e anterior em processamento, aguarde alguns minutos para emitir este MDF-e novamente.", Status = false };
                    }
                }

                if (validaAverbacoesCTe == "SIM" && (seguros == null || seguros.Count == 0))
                {
                    for (var i = 0; i < ctes.Count(); i++)
                    {
                        if (!this.ValidaAverbacaoCTe(ctes[i].Codigo, empresa.Codigo, unitOfWork))
                            return new Retorno<int>() { Mensagem = "Existem CT-es pendente de averbação, aguarde alguns minutos para emitir este MDF-e novamente.", Status = false };
                    }
                }

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

                if (ctes != null && ctes.Count > 0)
                    mdfe = servicoMDFe.GerarMDFePorCodigoCTes(empresa, ctes, veiculos, motoristas, reboques, observacaoFisco, observacaoContribuinte, cargaAgrupar, seguros, valesPedagio, dadosMDFe, localidadeCarregamento, produtoPredominante, listaCIOT, informacoesPagamento, unitOfWork);
                else if (nfesGlobalizadas != null && nfesGlobalizadas.Count > 0)
                    mdfe = servicoMDFe.GerarMDFePorNFEsGlobalizadas(empresa, nfesGlobalizadas, veiculos, motoristas, reboques, observacaoFisco, observacaoContribuinte, cargaAgrupar, seguros, valesPedagio, dadosMDFe, localidadeCarregamento, produtoPredominante, listaCIOT, informacoesPagamento, unitOfWork);

                if (mdfe == null)
                    retorno += "O MDF-e não foi gerado, verifique os dados enviados na integração. ";

                if (!string.IsNullOrWhiteSpace(cargaAgrupar) && cargaAgrupar != "0")
                    CancelarMDFeAnterior(cargaAgrupar, mdfe.Codigo, mdfe.EstadoDescarregamento.Sigla, empresa.Codigo, unitOfWork);

                if (!servicoMDFe.Emitir(mdfe, unitOfWork))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                if (!this.AdicionarMDFeNaFilaDeConsulta(mdfe))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                if (!this.AdicionarRegistroIntegrado(mdfe, cargaAgrupar, numeroUnidade.ToString(), "", arquivo, Dominio.Enumeradores.TipoArquivoIntegracao.CTe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, DateTime.MinValue, unitOfWork))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                retorno += "MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = mdfe.Codigo };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Servicos.Log.TratarErro("Codigos CTe: " + Newtonsoft.Json.JsonConvert.SerializeObject(codigosCTes));
                Servicos.Log.TratarErro("veiculos: " + Newtonsoft.Json.JsonConvert.SerializeObject(veiculos));
                Servicos.Log.TratarErro("reboques: " + Newtonsoft.Json.JsonConvert.SerializeObject(reboques));
                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha genérica ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<int> EncerrarMDFe(string cnpjEmpresaAdministradora, int codigoMDFe, string dataHoraEncerramento, int codigoIBGEMunicipioEncerramento, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataEncerramento;
                DateTime.TryParseExact(dataHoraEncerramento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoMDFe> integracoesMDFe = repIntegracaoMDFe.Buscar(codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao);
                Dominio.Entidades.Localidade municipioEncerramento = repLocalidade.BuscarPorCodigoIBGE(codigoIBGEMunicipioEncerramento);

                if (integracoesMDFe.Count() <= 0)
                    return new Retorno<int>() { Mensagem = "MDF-e não encontrado.", Status = false };

                if (integracoesMDFe[0].MDFe.Empresa.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + integracoesMDFe[0].MDFe.Empresa.CNPJ + ").", Status = false };

                if (integracoesMDFe[0].MDFe.Empresa.EmpresaPai.Configuracao != null && integracoesMDFe[0].MDFe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (municipioEncerramento == null)
                    return new Retorno<int>() { Mensagem = "Município de encerramento não encontrado.", Status = false };

                if (integracoesMDFe[0].MDFe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return new Retorno<int>() { Mensagem = "O status do MDF-e não permite o encerramento do mesmo.", Status = false };

                integracoesMDFe[0].MDFe.MunicipioEncerramento = municipioEncerramento;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracoesMDFe[0].MDFe, $"Encerramento de MDF-e nº{integracoesMDFe[0].MDFe.Numero}, método: EncerrarMDFe", unidadeDeTrabalho);

                repIntegracaoMDFe.Atualizar(integracoesMDFe[0]);

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                string retorno = string.Empty;

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(integracoesMDFe[0].MDFe.SistemaEmissor).EncerrarMdfe(integracoesMDFe[0].MDFe.Codigo, integracoesMDFe[0].MDFe.Empresa.Codigo, dataEncerramento != DateTime.MinValue ? dataEncerramento : DateTime.Now, unidadeDeTrabalho))
                    retorno += "O encerramento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                if (!this.AdicionarMDFeNaFilaDeConsulta(integracoesMDFe[0].MDFe))
                    retorno += "O encerramento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                svcMDFe.SalvarLogEncerramentoMDFe(integracoesMDFe[0].MDFe.Chave, integracoesMDFe[0].MDFe.Protocolo, dataEncerramento != DateTime.MinValue ? dataEncerramento : DateTime.Now, integracoesMDFe[0].MDFe.Empresa, integracoesMDFe[0].MDFe.Empresa.Localidade, "Encerramento solicitado via WebService", unidadeDeTrabalho);

                if (!this.AdicionarRegistroIntegrado(integracoesMDFe[0].MDFe, integracoesMDFe[0].NumeroDaCarga, integracoesMDFe[0].NumeroDaUnidade, "", "", Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoMDFe.Encerramento, DateTime.MinValue, unidadeDeTrabalho))
                    retorno += "O encerramento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                retorno += "O encerramento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = integracoesMDFe[0].MDFe.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao encerrar o MDF-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<bool> EncerrarMDFeExterno(Dominio.ObjetosDeValor.MDFe.EncerramentoMDFeExterno encerramentoMDFeExterno)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataEncerramento;
                DateTime.TryParseExact(encerramentoMDFeExterno.DataHoraEncerramento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                Dominio.Entidades.Localidade municipioEncerramento = repLocalidade.BuscarPorCodigoIBGE(encerramentoMDFeExterno.CodigoIBGEMunicipalEncerramento);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPorCNPJ(encerramentoMDFeExterno.CnpjEmpresa);

                if (encerramentoMDFeExterno.Chave.Length != 44)
                    return new Retorno<bool>() { Mensagem = "Chave de acesso inválida.", Status = false };

                if (empresa == null)
                    return new Retorno<bool>() { Mensagem = "Empresa não encontrada.", Status = false };

                if (empresa.EmpresaPai.CNPJ != encerramentoMDFeExterno.CnpjEmpresaAdministradora)
                    return new Retorno<bool>() { Mensagem = "A empresa administradora (" + encerramentoMDFeExterno.CnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + empresa.CNPJ + ").", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != encerramentoMDFeExterno.Token)
                    return new Retorno<bool>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (municipioEncerramento == null)
                    return new Retorno<bool>() { Mensagem = "Município de encerramento não encontrado.", Status = false };

                Servicos.Log.TratarErro("EncerrarMDFeExterno - Chave: " + encerramentoMDFeExterno.Chave + "da empresa " + empresa.CNPJ);

                string retorno = string.Empty;

                if (repMDFe.ContemMDFePorChave(encerramentoMDFeExterno.Chave))
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFeExistente = repMDFe.BuscarPorChave(encerramentoMDFeExterno.Chave);

                    if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(MDFeExistente.SistemaEmissor).EncerrarMdfe(MDFeExistente.Codigo, empresa.Codigo, dataEncerramento != DateTime.MinValue ? dataEncerramento : DateTime.Now, unidadeDeTrabalho))
                        retorno += "O encerramento do MDF-e de chave " + MDFeExistente.Chave + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                }
                else
                {
                    TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
                    bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(dataEncerramento);
                    string fusoHorario = horarioVerao ? servicoMDFe.AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : servicoMDFe.AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

                    Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno = new Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno()
                    {
                        Ambiente = empresa.TipoAmbiente,
                        Chave = encerramentoMDFeExterno.Chave,
                        CodigoMunicipioEncerramento = municipioEncerramento.CodigoIBGE,
                        CodigoUFEncerramento = municipioEncerramento.Estado.CodigoIBGE,
                        DataEncerramento = dataEncerramento != DateTime.MinValue ? dataEncerramento : DateTime.Now,
                        DataEvento = dataEncerramento != DateTime.MinValue ? dataEncerramento : DateTime.Now,
                        Empresa = empresa,
                        Protocolo = encerramentoMDFeExterno.Protocolo,
                        FusoHorario = fusoHorario
                    };

                    if (!servicoMDFe.EncerrarMDFeEmissorExterno(mdfeEmissorExterno, unidadeDeTrabalho))
                        retorno += "O encerramento do MDF-e de chave " + encerramentoMDFeExterno.Chave + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                }

                retorno += "O encerramento do MDF-e de chave " + encerramentoMDFeExterno.Chave + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<bool>() { Mensagem = retorno, Status = true, Objeto = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha ao encerrar o MDF-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> CancelarMDFe(string cnpjEmpresaAdministradora, int codigoMDFe, string justificativa, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 20)
                    return new Retorno<int>() { Mensagem = "Justificativa inválida (" + justificativa + ").", Status = false };

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoMDFe> integracoesMDFe = repIntegracaoMDFe.Buscar(codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao);

                if (integracoesMDFe.Count() <= 0)
                    return new Retorno<int>() { Mensagem = "MDF-e não encontrado.", Status = false };

                if (integracoesMDFe[0].MDFe.Empresa.EmpresaPai.CNPJ != cnpjEmpresaAdministradora)
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaAdministradora + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + integracoesMDFe[0].MDFe.Empresa.CNPJ + ").", Status = false };

                if (integracoesMDFe[0].MDFe.Empresa.EmpresaPai.Configuracao != null && integracoesMDFe[0].MDFe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (integracoesMDFe[0].MDFe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return new Retorno<int>() { Mensagem = "O status do MDF-e não permite o cancelamento do mesmo.", Status = false };

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                string retorno = string.Empty;

                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(integracoesMDFe[0].MDFe.Empresa.FusoHorario);
                DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                if (fusoHorarioEmpresa != TimeZoneInfo.Local)
                    dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(integracoesMDFe[0].MDFe.SistemaEmissor).CancelarMdfe(integracoesMDFe[0].MDFe.Codigo, integracoesMDFe[0].MDFe.Empresa.Codigo, justificativa, unidadeDeTrabalho, dataFuso))
                    retorno += "O cancelamento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao cancela-lo. ";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracoesMDFe[0].MDFe, $"Cancelamento de MDF-e nº{integracoesMDFe[0].MDFe.Numero}, método: CancelarMDFe", unidadeDeTrabalho);

                if (!this.AdicionarMDFeNaFilaDeConsulta(integracoesMDFe[0].MDFe))
                    retorno += "O cancelamento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                if (!this.AdicionarRegistroIntegrado(integracoesMDFe[0].MDFe, integracoesMDFe[0].NumeroDaCarga, integracoesMDFe[0].NumeroDaUnidade, "", "", Dominio.Enumeradores.TipoArquivoIntegracao.Objeto, Dominio.Enumeradores.TipoIntegracaoMDFe.Cancelamento, DateTime.MinValue, unidadeDeTrabalho))
                    retorno += "O cancelamento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                retorno += "O cancelamento do MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " integrado com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = integracoesMDFe[0].MDFe.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao cancelar o MDF-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> ReemitirMDFe(int codigoMDFe, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoMDFe> integracoesMDFe = repIntegracaoMDFe.Buscar(codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao);

                if (integracoesMDFe.Count() <= 0)
                    return new Retorno<int>() { Mensagem = "MDF-e não encontrado.", Status = false };

                if (integracoesMDFe[0].MDFe.Empresa.EmpresaPai.Configuracao != null && integracoesMDFe[0].MDFe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (integracoesMDFe[0].MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao)
                    return new Retorno<int>() { Mensagem = "O status do MDF-e não permite emitir novamente.", Status = false };

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                string retorno = string.Empty;

                if (!servicoMDFe.Emitir(integracoesMDFe[0].MDFe, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                if (!this.AdicionarMDFeNaFilaDeConsulta(integracoesMDFe[0].MDFe))
                    retorno += "O MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                retorno += "MDF-e nº " + integracoesMDFe[0].MDFe.Numero.ToString() + " da empresa " + integracoesMDFe[0].MDFe.Empresa.CNPJ + " reemitido com sucesso! ";

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = integracoesMDFe[0].MDFe.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao reemitir o MDF-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<int> IntegrarMDFePorCTesEPlaca(int[] codigosCTes, string cnpjEmpresaEmitente, string cnpjEmpresaPai, int numeroUnidade, string placaTracao, string placaReboque, string token)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("Codigos CTe: " + Newtonsoft.Json.JsonConvert.SerializeObject(codigosCTes));
                Servicos.Log.TratarErro("CNPJ Empresa Emitente: " + cnpjEmpresaEmitente);
                Servicos.Log.TratarErro("CNPJ Empresa Pai: " + cnpjEmpresaPai);
                Servicos.Log.TratarErro("Unidade: " + numeroUnidade.ToString());
                Servicos.Log.TratarErro("Traçao: " + placaTracao);
                Servicos.Log.TratarErro("Reboque: " + placaReboque);
                Servicos.Log.TratarErro("Token: " + token);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjEmpresaEmitente));

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaPai))
                    return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaPai + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + cnpjEmpresaEmitente + ").", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não está configurada.", Status = false };

                if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não esta liberada para emissão de MDF-e", Status = false };

                if (empresa.Configuracao.SerieMDFe == null)
                    return new Retorno<int>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não possui uma série configurada para a emissão de MDF-e.", Status = false };

                if (string.IsNullOrWhiteSpace(placaTracao))
                    return new Retorno<int>() { Mensagem = "Obrigatório que seja informada uma placa de um veículo tração.", Status = false };

                Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorPlaca(empresa.Codigo, placaTracao);
                if (veiculoTracao == null)
                    Servicos.Log.TratarErro("Não encontrado cadastro para o veículo tração placa " + placaTracao + ".");// return new Retorno<int>() { Mensagem = "Não encontrado cadastro para o veículo tração placa "+placaTracao+" .", Status = false };
                if (veiculoTracao != null && veiculoTracao.TipoVeiculo == "1")
                    Servicos.Log.TratarErro("Placa " + placaTracao + " enviada como tração está cadastrada como reboque.");// return new Retorno<int>() { Mensagem = "Placa " + placaTracao + " enviada como tração está cadastrada como reboque.", Status = false };

                Dominio.Entidades.Veiculo veiculoReboque = repVeiculo.BuscarPorPlaca(empresa.Codigo, placaReboque);
                if (veiculoReboque != null)
                {
                    if (veiculoReboque.TipoVeiculo == "0")
                        Servicos.Log.TratarErro("Placa " + placaReboque + " enviada como reboque está cadastrada como tração.");// return new Retorno<int>() { Mensagem = "Placa " + placaReboque + " enviada como reboque está cadastrada como tração.", Status = false };
                }
                else if (!string.IsNullOrWhiteSpace(placaReboque) && veiculoReboque == null)
                    Servicos.Log.TratarErro("Não encontrado cadastro para o veículo reboque placa " + placaTracao + ".");

                codigosCTes = codigosCTes.Distinct().ToArray();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(empresa.Codigo, codigosCTes);

                if (ctes.Count() != codigosCTes.Count())
                    return new Retorno<int>() { Mensagem = "Alguns CT-es não foram encontrados para a emissão do MDF-e.", Status = false };

                retorno = this.ValidarMDFe(unidadeDeTrabalho, empresa, ctes);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<int>() { Mensagem = retorno, Status = false };

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = servicoMDFe.GerarMDFePorCTesEPlacas(empresa, ctes, veiculoTracao, veiculoReboque, unidadeDeTrabalho);


                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                Dominio.Entidades.VeiculoMDFe veiculoMDFeTracao = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);

                if (veiculoMDFeTracao != null) //Apenas emite se possui veiculo Tração
                {
                    if (!servicoMDFe.Emitir(mdfe, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, $"Integração de MDF-e nº{mdfe.Numero}, método: IntegrarMDFe", unidadeDeTrabalho);

                    if (!this.AdicionarMDFeNaFilaDeConsulta(mdfe))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";
                }
                else
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmDigitacao;
                    mdfe.MensagemRetornoSefaz = "MDFe não emitido pois está sem veículo tração.";
                    repMDFe.Atualizar(mdfe);
                }

                if (!this.AdicionarRegistroIntegrado(mdfe, "", numeroUnidade.ToString(), "", "", Dominio.Enumeradores.TipoArquivoIntegracao.CTe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, DateTime.MinValue, unidadeDeTrabalho))
                    retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                retorno += "MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                Servicos.Log.TratarErro(retorno);

                return new Retorno<int>() { Mensagem = retorno, Status = true, Objeto = mdfe.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = retorno + "Ocorreu uma falha genérica ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<int>> IntegrarMDFePorCTesDestinosDiferentes(int[] codigosCTes, string cnpjEmpresaEmitente, string cnpjEmpresaPai, int numeroUnidade, int numeroCarga, string observacaoMDFe, string token)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("IntegrarMDFePorCTesDestinosDiferentes - Codigos CTe: " + Newtonsoft.Json.JsonConvert.SerializeObject(codigosCTes));
                Servicos.Log.TratarErro("IntegrarMDFePorCTesDestinosDiferentes - CNPJEmpresaEmitente: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaEmitente) ? cnpjEmpresaEmitente : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCTesDestinosDiferentes - CNPJEmpresaPai: " + (!string.IsNullOrWhiteSpace(cnpjEmpresaPai) ? cnpjEmpresaPai : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCTesDestinosDiferentes - NumeroUnidade: " + numeroUnidade.ToString());
                Servicos.Log.TratarErro("IntegrarMDFePorCTesDestinosDiferentes - NumeroCarga: " + numeroCarga.ToString());
                Servicos.Log.TratarErro("IntegrarMDFePorCTesDestinosDiferentes - ObservacaoMDFe: " + (!string.IsNullOrWhiteSpace(observacaoMDFe) ? observacaoMDFe : string.Empty));
                Servicos.Log.TratarErro("IntegrarMDFePorCTesDestinosDiferentes - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjEmpresaEmitente));

                if (empresa == null)
                    return new Retorno<List<int>>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai.CNPJ != Utilidades.String.OnlyNumbers(cnpjEmpresaPai))
                    return new Retorno<List<int>>() { Mensagem = "A empresa administradora (" + cnpjEmpresaPai + ") não está vinculada ou não pode emitir MDF-es para esta empresa (" + cnpjEmpresaEmitente + ").", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<int>>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<List<int>>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não está configurada.", Status = false };

                if (empresa.Configuracao.BloquearEmissaoMDFeWS)
                    return new Retorno<List<int>>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não esta liberada para emissão de MDF-e", Status = false };

                if (empresa.Configuracao.SerieMDFe == null)
                    return new Retorno<List<int>>() { Mensagem = "A empresa (" + cnpjEmpresaEmitente + ") não possui uma série configurada para a emissão de MDF-e.", Status = false };

                codigosCTes = codigosCTes.Distinct().ToArray();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(empresa.Codigo, codigosCTes);

                if (ctes.Count() != codigosCTes.Count())
                    return new Retorno<List<int>>() { Mensagem = "Alguns CT-es não foram encontrados para a emissão do MDF-e.", Status = false };

                retorno = this.ValidarMDFeDestinosDiferentes(unidadeDeTrabalho, empresa, ctes);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new Retorno<List<int>>() { Mensagem = retorno, Status = false };

                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFe = servicoMDFe.GerarMDFePorCTesDestinosDiferentes(empresa, ctes, observacaoMDFe, unidadeDeTrabalho);

                List<int> listaMDFes = new List<int>();

                foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in listaMDFe)
                {
                    if (!servicoMDFe.Emitir(mdfe, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo. ";

                    if (!this.AdicionarMDFeNaFilaDeConsulta(mdfe))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ";

                    if (!this.AdicionarRegistroIntegrado(mdfe, numeroCarga.ToString(), numeroUnidade.ToString(), "", "", Dominio.Enumeradores.TipoArquivoIntegracao.CTe, Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao, DateTime.MinValue, unidadeDeTrabalho))
                        retorno += "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " foi emitido, porém, não foi possível salvar o registro de integração. ";

                    retorno += "MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + empresa.CNPJ + " integrado com sucesso! ";

                    listaMDFes.Add(mdfe.Codigo);
                }

                return new Retorno<List<int>>() { Mensagem = retorno, Status = true, Objeto = listaMDFes };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<int>>() { Mensagem = retorno + "Ocorreu uma falha genérica ao integrar o MDF-e. ", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT> IntegrarCIOTPorCTes(Dominio.ObjetosDeValor.CIOT.CIOT ciot, string token)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("IntegrarCIOT - " + Newtonsoft.Json.JsonConvert.SerializeObject(ciot));
                Servicos.Log.TratarErro("IntegrarCIOT - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                if (ciot.CodigosCTes == null || ciot.CodigosCTes.Count() == 0)
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Nenhum CTe.", Status = false };

                if (ciot.Veiculo == null || string.IsNullOrWhiteSpace(ciot.Veiculo.Placa))
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Nenhum veículo informado.", Status = false };

                if (ciot.Motorista == null || string.IsNullOrWhiteSpace(ciot.Motorista.CPF))
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Nenhum motorista informado.", Status = false };

                int[] codigosCTes = ciot.CodigosCTes.Distinct().ToArray();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.NaturezaCargaANTT repNaturezaCargaANTT = new Repositorio.NaturezaCargaANTT(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(0, codigosCTes);

                if (ctes.Count() != codigosCTes.Count())
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Alguns CT-es não foram encontrados.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(ctes.FirstOrDefault().Empresa.Codigo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = null;

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = null;

                if (empresa == null)
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "A empresa (" + ctes.FirstOrDefault().Empresa.CNPJ + ") não foi encontrada.", Status = false };

                if (empresa.EmpresaPai.Configuracao != null && empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (empresa.Configuracao == null)
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "A empresa (" + ctes.FirstOrDefault().Empresa.CNPJ + ") não está configurada.", Status = false };

                if ((empresa.EmpresaPai.Configuracao == null || !empresa.EmpresaPai.Configuracao.TipoIntegradoraCIOT.HasValue) && (empresa.Configuracao == null || !empresa.Configuracao.TipoIntegradoraCIOT.HasValue))
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Ambiente/transportador sem configuração para geração de CIOT", Status = false };

                if (string.IsNullOrWhiteSpace(ciot.Veiculo.RENAVAM))
                {
                    veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, ciot.Veiculo.Placa);
                    if (veiculo == null)
                        return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Veículo " + ciot.Veiculo.Placa + " não cadastrado para o transportador " + empresa.CNPJ + ".", Status = false };
                }

                if (string.IsNullOrWhiteSpace(ciot.Motorista.CNH))
                {
                    motorista = repMotorista.BuscarMotoristaPorCPF(empresa.Codigo, ciot.Motorista.CPF);
                    if (motorista == null)
                        return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Veículo " + ciot.Veiculo.Placa + " não cadastrado para o transportador " + empresa.CNPJ + ".", Status = false };
                }

                if (string.IsNullOrWhiteSpace(ciot.NaturezaCarga) || repNaturezaCargaANTT.BuscarPorNatureza(ciot.NaturezaCarga) == null)
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Natureza " + ciot.NaturezaCarga + " está inválida.", Status = false };

                //Processar CIOT
                unitOfWork.Start();

                Servicos.CIOT servicoCIOT = new Servicos.CIOT(unitOfWork);
                Dominio.Entidades.CIOTSigaFacil ciotGerado = servicoCIOT.GerarCIOT(ciot, empresa, unitOfWork);
                unitOfWork.CommitChanges();

                Dominio.ObjetosDeValor.CIOT.RetornoCIOT retornoCIOT = new Dominio.ObjetosDeValor.CIOT.RetornoCIOT();
                retornoCIOT.ProtocoloIntegracao = ciotGerado.Codigo;
                retornoCIOT.NumeroSequencial = ciotGerado.Numero;
                retornoCIOT.StatusCIOT = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;

                try
                {
                    if (ciotGerado != null)
                    {
                        servicoCIOT.Emitir(ciotGerado.Codigo, unitOfWork); //Enviar para eFrete ou criar Thread

                        if (ciotGerado.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                        {
                            retornoCIOT.NumeroCIOT = ciotGerado.NumeroCIOT;
                            retornoCIOT.StatusCIOT = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;

                            return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "CIOT autorizado com Sucesso.", Status = true, Objeto = retornoCIOT };
                        }
                        else if (ciotGerado.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                        {
                            retornoCIOT.StatusCIOT = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                            return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = ciotGerado.MensagemRetorno, Status = true, Objeto = retornoCIOT };
                        }

                        return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "CIOT Integrado com Sucesso, aguardando processamento IPEF.", Status = true, Objeto = retornoCIOT };
                    }
                    else
                        return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = retorno + "Não foi possíve gerar CIOT. ", Status = false };
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retornoCIOT.StatusCIOT = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "CIOT Integrado com Sucesso, porém houve falha aao integrar com IPEF.", Status = true, Objeto = retornoCIOT };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (unitOfWork != null)
                    unitOfWork.Rollback();

                return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = retorno + "Ocorreu uma falha genérica ao integrar CIOT. ", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT> ConsultarCIOTPorProtocolo(int protocolo, string token)
        {
            string retorno = string.Empty;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.Log.TratarErro("ConsultarCIOTPorProtocolo - Protocolo: " + protocolo.ToString());
                Servicos.Log.TratarErro("ConsultarCIOTPorProtocolo - Token: " + (!string.IsNullOrWhiteSpace(token) ? token : string.Empty));

                if (protocolo <= 0)
                    return new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "Protocolo inválido.", Status = false };

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(unitOfWork);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(protocolo);

                if (ciot != null)
                {
                    Dominio.ObjetosDeValor.CIOT.RetornoCIOT retornoCIOT = new Dominio.ObjetosDeValor.CIOT.RetornoCIOT();
                    retornoCIOT.ProtocoloIntegracao = ciot.Codigo;
                    retornoCIOT.NumeroSequencial = ciot.Numero;

                    if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado)
                    {
                        retornoCIOT.NumeroCIOT = ciot.NumeroCIOT;
                        retornoCIOT.StatusCIOT = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;

                        var retornoIntegracao = new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = "CIOT autorizado com Sucesso.", Status = true, Objeto = retornoCIOT };
                        Servicos.Log.TratarErro("ConsultarCIOTPorProtocolo - Retorno: " + Newtonsoft.Json.JsonConvert.SerializeObject(retornoIntegracao));

                        return retornoIntegracao;
                    }
                    else if (ciot.Status == Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado)
                    {
                        retornoCIOT.StatusCIOT = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;

                        var retornoIntegracao = new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = ciot.MensagemRetorno, Status = true, Objeto = retornoCIOT };
                        Servicos.Log.TratarErro("ConsultarCIOTPorProtocolo - Retorno: " + Newtonsoft.Json.JsonConvert.SerializeObject(retornoIntegracao));

                        return retornoIntegracao;
                    }
                    else
                    {
                        retornoCIOT.StatusCIOT = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Pendente;

                        var retornoIntegracao = new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = ciot.DescricaoStatus, Status = true, Objeto = retornoCIOT };
                        Servicos.Log.TratarErro("ConsultarCIOTPorProtocolo - Retorno: " + Newtonsoft.Json.JsonConvert.SerializeObject(retornoIntegracao));

                        return retornoIntegracao;

                    }
                }
                else
                {
                    var retornoIntegracao = new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = retorno + "CIOT não localizado com o protocolo " + protocolo.ToString() + ". ", Status = false };
                    Servicos.Log.TratarErro("ConsultarCIOTPorProtocolo - Retorno: " + Newtonsoft.Json.JsonConvert.SerializeObject(retornoIntegracao));

                    return retornoIntegracao;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                var retornoIntegracao = new Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT>() { Mensagem = retorno + "Ocorreu uma falha genérica ao consultar CIOT. ", Status = false };
                Servicos.Log.TratarErro("ConsultarCIOTPorProtocolo - Retorno: " + Newtonsoft.Json.JsonConvert.SerializeObject(retornoIntegracao));

                return retornoIntegracao;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<int> EnviarXMLMDFe(string xml, string cnpjEmpresaPai, string cnpjEmpresaEmitente, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);
                Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unidadeDeTrabalho);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

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
                        return new Retorno<int>() { Mensagem = "A empresa administradora (" + cnpjEmpresaPai + ") não está vinculada ou não pode ter MDF-es para esta empresa (" + cnpjEmpresaEmitente + ").", Status = false };

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

                System.IO.MemoryStream arquivo = new MemoryStream(Encoding.UTF8.GetBytes(xml ?? ""));

                object retorno = servicoMDFe.GerarMDFeAnteriorAsync(arquivo, empresa.Codigo).GetAwaiter().GetResult();

                if (retorno != null)
                {
                    try
                    {
                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeIntegrado = repMDFe.BuscarPorCodigo(((Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais)retorno).Codigo);

                        if (mdfeIntegrado != null)
                            return new Retorno<int>() { Mensagem = "Integração realizada com sucesso.", Status = true, Objeto = mdfeIntegrado.Codigo };
                        else
                            return new Retorno<int>() { Mensagem = "MDFe não importado.", Status = false };
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        return new Retorno<int>() { Mensagem = retorno.ToString(), Status = false };
                    }
                }
                else
                {
                    return new Retorno<int>() { Mensagem = "MDF de transporte não importado.", Status = false };
                }

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "MDF-e de transporte não importado.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

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

        private bool AdicionarRegistroIntegrado(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, string numeroDaCarga, string numeroDaUnidade, string nomeArquivo, string arquivo, Dominio.Enumeradores.TipoArquivoIntegracao tipoArquivo, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, DateTime dataEncerramento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.IntegracaoMDFe repIntegracao = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);
                Dominio.Entidades.IntegracaoMDFe integracao = new Dominio.Entidades.IntegracaoMDFe();

                integracao.MDFe = mdfe;
                integracao.Arquivo = arquivo;
                integracao.NumeroDaCarga = numeroDaCarga;
                integracao.NumeroDaUnidade = numeroDaUnidade;
                integracao.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                integracao.TipoArquivo = tipoArquivo;
                integracao.NomeArquivo = nomeArquivo;
                integracao.Tipo = tipoIntegracao;
                integracao.GerouCargaEmbarcador = false;

                if (dataEncerramento != DateTime.MinValue)
                    integracao.DataEncerramento = dataEncerramento;

                repIntegracao.Inserir(integracao);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private string ValidarMDFe(Dominio.ObjetosDeValor.MDFe.MDFe mdfe)
        {
            string erros = string.Empty;

            if (mdfe == null)
                return "O MDF-e não pode ser nulo para a integração.";

            if (mdfe.NumeroCarga <= 0)
                erros += "Número da carga inválido. ";

            if (mdfe.Emitente == null)
                erros += "Emitente não pode ser nulo. ";

            if (mdfe.ValorTotalMercadoria <= 0)
                erros += "Valor total da mercadoria é obrigatório. ";

            if (mdfe.PesoBrutoMercadoria <= 0)
                erros += "Peso bruto da mercadoria é obrigatório. ";

            if (string.IsNullOrWhiteSpace(mdfe.UFCarregamento))
                erros += "UF de carregamento é obrigatório. ";

            if (mdfe.MunicipiosDeCarregamento == null || mdfe.MunicipiosDeCarregamento.Count() <= 0)
                erros += "Municípios de carregamento são obrigatórios. ";

            if (string.IsNullOrWhiteSpace(mdfe.UFDescarregamento))
                erros += "UF de descarregamento é obrigatório. ";

            if (mdfe.MunicipiosDeDescarregamento == null || mdfe.MunicipiosDeDescarregamento.Count() <= 0)
                erros += "Municípios de descarregamento são obrigatórios. ";
            else
            {
                int countCTe = 0;
                int countNFe = 0;

                foreach (Dominio.ObjetosDeValor.MDFe.MunicipioDescarregamento municipioDescarregamento in mdfe.MunicipiosDeDescarregamento)
                {
                    if (municipioDescarregamento.Documentos != null && municipioDescarregamento.Documentos.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.MDFe.DocumentoMunicipioDescarregamento documento in municipioDescarregamento.Documentos)
                        {
                            if (!string.IsNullOrWhiteSpace(documento.ChaveNFe))
                                countNFe = countNFe + 1;
                            if (!string.IsNullOrWhiteSpace(documento.ChaveCTe))
                                countCTe = countCTe + 1;
                        }
                    }
                }

                if (countCTe == 0 && countNFe == 0)
                    erros += "Nenhum documento enviado. ";

                if (countCTe > 0 && countNFe > 0)
                    erros += "Documentos inválidos, deve ser enviado CT-e OU NF-e. ";
            }

            if (mdfe.Veiculo == null)
            {
                erros += "Veículo é obrigatório. ";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(mdfe.Veiculo.Placa))
                    erros += "Placa do veículo é obrigatório. ";

                if (string.IsNullOrWhiteSpace(mdfe.Veiculo.Renavam))
                    erros += "RENAVAM do veículo (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoCarroceria))
                    erros += "Tipo de carroceria do veículo (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoPropriedade))
                    erros += "Tipo de propriedade do veículo (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoRodado))
                    erros += "Tipo de rodado do veículo (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoVeiculo))
                    erros += "Tipo de veículo do veículo (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                if (string.IsNullOrWhiteSpace(mdfe.Veiculo.UF))
                    erros += "UF do veículo (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                if (mdfe.Veiculo.Tara <= 0)
                    erros += "Tara do veículo (" + mdfe.Veiculo.Placa + ") é obrigatório. ";
            }

            if (mdfe.Reboques != null && mdfe.Reboques.Count() > 0)
            {
                foreach (Dominio.ObjetosDeValor.CTe.Veiculo veiculo in mdfe.Reboques)
                {
                    if (string.IsNullOrWhiteSpace(mdfe.Veiculo.Placa))
                        erros += "Placa do reboque é obrigatório. ";

                    if (string.IsNullOrWhiteSpace(mdfe.Veiculo.Renavam))
                        erros += "RENAVAM do reboque (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                    if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoCarroceria))
                        erros += "Tipo de carroceria do reboque (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                    if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoPropriedade))
                        erros += "Tipo de propriedade do reboque (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                    if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoRodado))
                        erros += "Tipo de rodado do reboque (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                    if (string.IsNullOrWhiteSpace(mdfe.Veiculo.TipoVeiculo))
                        erros += "Tipo de veículo do reboque (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                    if (string.IsNullOrWhiteSpace(mdfe.Veiculo.UF))
                        erros += "UF do reboque (" + mdfe.Veiculo.Placa + ") é obrigatório. ";

                    if (mdfe.Veiculo.Tara <= 0)
                        erros += "Tara do reboque (" + mdfe.Veiculo.Placa + ") é obrigatório. ";
                }
            }

            if (mdfe.ValesPedagio != null && mdfe.ValesPedagio.Count() > 0)
            {
                foreach (Dominio.ObjetosDeValor.MDFe.ValePedagio valePedagio in mdfe.ValesPedagio)
                {
                    if (string.IsNullOrWhiteSpace(valePedagio.NumeroComprovante))
                        erros += "Número do comprovante do vale pedágio é obrigatório. ";

                    if (string.IsNullOrWhiteSpace(valePedagio.CNPJFornecedor))
                        erros += "CNPJ do fornecedor do vale pedágio (" + valePedagio.NumeroComprovante + ") é obrigatório. ";
                }
            }

            return erros;
        }

        private string ValidarDadosParaAtualizacao(Dominio.ObjetosDeValor.CTe.Empresa empresa)
        {
            string erros = string.Empty;

            if (string.IsNullOrWhiteSpace(empresa.RNTRC))
                erros += "RNTRC é obrigatório ao atualizar a empresa para emissão do MDF-e. ";

            if (!string.IsNullOrWhiteSpace(empresa.RNTRC) && empresa.RNTRC.Length != 8)
                erros += "O campo RNTRC deve conter exatamente 8 caracteres. ";

            if (string.IsNullOrWhiteSpace(empresa.CEP))
                erros += "CEP é obrigatório ao atualizar a empresa para emissão do MDF-e. ";

            if (!string.IsNullOrWhiteSpace(empresa.CEP))
            {
                string cepNumeros = Utilidades.String.OnlyNumbers(empresa.CEP);
                if (cepNumeros.Length != 8)
                    erros += "O campo CEP deve conter exatamente 8 dígitos. ";
            }

            if (empresa.CodigoIBGECidade <= 0)
                erros += "Código IBGE da cidade é obrigatório ao atualizar a empresa para emissão do MDF-e. ";

            if (string.IsNullOrWhiteSpace(empresa.InscricaoEstadual))
                erros += "Inscrição Estadual é obrigatória ao atualizar a empresa para emissão do MDF-e. ";

            if (string.IsNullOrWhiteSpace(empresa.NomeFantasia))
                erros += "Nome Fantasia é obrigatório ao atualizar a empresa para emissão do MDF-e. ";

            if (string.IsNullOrWhiteSpace(empresa.RazaoSocial))
                erros += "Razão Social é obrigatória ao atualizar a empresa para emissão do MDF-e. ";
 
            return erros;
        }

        private string ValidarMDFe(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, bool validarVeiculos = true, Dominio.ObjetosDeValor.ValePedagioCompra valePedagioCompra = null, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas = null)
        {
            string erros = string.Empty;

            if (ctes != null && ctes.Count > 0)
            {

                IEnumerable<string> ufsCarregamento = (from obj in ctes select obj.LocalidadeInicioPrestacao.Estado.Sigla).Distinct();
                IEnumerable<string> ufsDescarregamento = (from obj in ctes select obj.LocalidadeTerminoPrestacao.Estado.Sigla).Distinct();

                Repositorio.VeiculoCTE repVeiculo = new Repositorio.VeiculoCTE(unitOfWork);
                List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculo.BuscarPorCTe(empresa.Codigo, (from obj in ctes select obj.Codigo).ToArray());

                if (ufsCarregamento.Count() > 1)
                    erros += "Os CT-es possuem mais de um Estado de início da prestação. ";

                if (ufsDescarregamento.Count() > 1)
                    erros += "Os CT-es possuem mais de um Estado de término da prestação. ";

                if (validarVeiculos && veiculos.Count() <= 0)
                    erros += "Não há veículo vinculado aos CT-es para uso na emissão do MDF-e. ";
            }
            else if (nfesGlobalizadas != null && nfesGlobalizadas.Count > 0)
            {
                //Validações das NFes 

                //Chave invalida

                //IBGE não encontrado
            }

            if (valePedagioCompra != null && string.IsNullOrWhiteSpace(valePedagioCompra.NomeRota))
            {
                if (string.IsNullOrWhiteSpace(valePedagioCompra.IBGEInicio))
                    erros += "IBGE Início para comprado vale pedágio não informado. ";

                if (string.IsNullOrWhiteSpace(valePedagioCompra.IBGEFim))
                    erros += "IBGE Fim para comprado vale pedágio não informado. ";
            }

            return erros;
        }

        private string ValidarMDFeDestinosDiferentes(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, bool validarVeiculos = true)
        {
            string erros = string.Empty;

            //IEnumerable<string> ufsCarregamento = (from obj in ctes select obj.LocalidadeInicioPrestacao.Estado.Sigla).Distinct();
            //IEnumerable<string> ufsDescarregamento = (from obj in ctes select obj.LocalidadeTerminoPrestacao.Estado.Sigla).Distinct();

            Repositorio.VeiculoCTE repVeiculo = new Repositorio.VeiculoCTE(unitOfWork);
            List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculo.BuscarPorCTe(empresa.Codigo, (from obj in ctes select obj.Codigo).ToArray());

            //if (ufsCarregamento.Count() > 1)
            //    erros += "Os CT-es possuem mais de um Estado de início da prestação. ";

            //if (ufsDescarregamento.Count() > 1)
            //    erros += "Os CT-es possuem mais de um Estado de término da prestação. ";

            if (validarVeiculos && veiculos.Count() <= 0)
                erros += "Não há veículo vinculado aos CT-es para uso na emissão do MDF-e. ";

            return erros;
        }

        private bool CancelarMDFeAnterior(string numeroCarga, int codigoMDFeAtual, string estadoDescarregamento, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unitOfWork);

                List<Dominio.Entidades.IntegracaoMDFe> listaIntegracaoMDFe = repIntegracaoMDFe.BuscarPorCarga(numeroCarga, Dominio.Enumeradores.StatusMDFe.Autorizado, codigoEmpresa);

                foreach (Dominio.Entidades.IntegracaoMDFe integracaoMDFe in listaIntegracaoMDFe)
                {
                    if (integracaoMDFe.MDFe.Codigo != codigoMDFeAtual && integracaoMDFe.MDFe.EstadoDescarregamento.Sigla == estadoDescarregamento)
                    {
                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(integracaoMDFe.MDFe.Empresa.FusoHorario);
                        DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                        if (fusoHorarioEmpresa != TimeZoneInfo.Local)
                            dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);

                        if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(integracaoMDFe.MDFe.SistemaEmissor).CancelarMdfe(integracaoMDFe.MDFe.Codigo, integracaoMDFe.MDFe.Empresa.Codigo, "MDFE CANCELADO POIS CARGA FOI AGRUPADA", unitOfWork, dataFuso))
                        {
                            AdicionarMDFeNaFilaDeConsulta(integracaoMDFe.MDFe);
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool VerificaStatusMDFeAnterior(string numeroCarga, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unitOfWork);

                List<Dominio.Entidades.IntegracaoMDFe> listaIntegracaoMDFe = repIntegracaoMDFe.BuscarPorCargaEmpresa(numeroCarga, codigoEmpresa);
                if (listaIntegracaoMDFe.Count() > 0)
                {
                    foreach (Dominio.Entidades.IntegracaoMDFe integracaoMDFe in listaIntegracaoMDFe)
                    {
                        if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Pendente || integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Enviado)
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool ValidaAverbacaoCTe(int codigoCTe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.SeguroCTE repSeguroCTe = new Repositorio.SeguroCTE(unitOfWork);
            List<Dominio.Entidades.SeguroCTE> segurosCTe = repSeguroCTe.BuscarPorCTe(codigoCTe);

            foreach (Dominio.Entidades.SeguroCTE seguro in segurosCTe)
            {
                if (!string.IsNullOrWhiteSpace(seguro.NumeroAverbacao))
                    return true;
            }

            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
            if (!string.IsNullOrWhiteSpace(svcMDFe.BuscarAverbacaoCTe(codigoCTe, codigoEmpresa, unitOfWork)))
                return true;

            return false;
        }

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceMDFe;
        }

        #endregion
    }
}
