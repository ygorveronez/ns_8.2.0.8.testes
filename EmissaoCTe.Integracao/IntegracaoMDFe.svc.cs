using AdminMultisoftware.Repositorio;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EmissaoCTe.Integracao
{
    public class IntegracaoMDFe : IIntegracaoMDFe
    {
        #region Métodos Públicos

        public Retorno<List<RetornoMDFe>> Buscar(Dominio.Enumeradores.StatusIntegracao statusIntegracao, int codigoEmpresaPai, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (token == "")
                    token = null;

                if (empresaPai == null)
                    return new Retorno<List<RetornoMDFe>>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<RetornoMDFe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (ConfigurationManager.AppSettings["ControlarConsultasImpressao"] == "SIM")
                {
                    if (!Servicos.StatusConsultaImpressaoUnidade.VerificarStatusConsultaImpressaoUnidade(numeroUnidade, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, true, unidadeDeTrabalho))
                    {
                        System.Threading.Thread.Sleep(5000);
                        return new Retorno<List<RetornoMDFe>>() { Mensagem = "Outra consulta esta em andamento para mesma Unidade e Documento.", Status = false };
                    }
                }

                int.TryParse(ConfigurationManager.AppSettings["RegistrosImpressaoMDFe"], out int registrosImpressao);

                if (registrosImpressao == 0)
                    registrosImpressao = 25;

                Repositorio.IntegracaoMDFe repIntegracao = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoMDFe> listaIntegracoes = repIntegracao.Buscar(codigoEmpresaPai, numeroUnidade.ToString(), numeroCarga.ToString(), registrosImpressao, statusIntegracao);

                Retorno<List<RetornoMDFe>> retorno = new Retorno<List<RetornoMDFe>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoMDFe> dadosRetorno = new List<RetornoMDFe>();

                foreach (Dominio.Entidades.IntegracaoMDFe integracao in listaIntegracoes)
                {
                    RetornoMDFe retornoMDFe = new RetornoMDFe()
                    {
                        CodigoMDFe = integracao.MDFe.Codigo,
                        ChaveMDFe = integracao.MDFe.Chave,
                        CNPJEmpresa = integracao.MDFe.Empresa.CNPJ,
                        Tipo = integracao.Tipo,
                        NomeArquivo = integracao.NomeArquivo,
                        Arquivo = integracao.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao ? integracao.Arquivo : string.Empty,
                        StatusMDFe = integracao.MDFe.Status,
                        DataEmissao = integracao.MDFe.DataEmissao.HasValue ? integracao.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        NumeroMDFe = integracao.MDFe.Numero,
                        SerieMDFe = integracao.MDFe.Serie.Numero,
                        MensagemRetorno = integracao.MDFe.MensagemStatus != null ? string.Concat(integracao.MDFe.MensagemStatus.CodigoDoErro, " - ", integracao.MDFe.MensagemStatus.MensagemDoErro) : integracao.MDFe.MensagemRetornoSefaz,
                        CargaPropria = integracao.MDFe.Empresa.Configuracao.TipoGeracaoCTeWS == Dominio.Enumeradores.TipoGeracaoCTeWS.GerarMDFe ? true : false
                    };

                    if (integracao.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado) //Validação para retornar XML e PDF apenas quando MDFe foir autorizado
                        this.ObterMDFeImpressao(ref retornoMDFe, integracao, Dominio.Enumeradores.TipoIntegracaoMDFe.Todos, tipoRetorno, unidadeDeTrabalho);

                    dadosRetorno.Add(retornoMDFe);
                }

                retorno.Objeto = dadosRetorno;

                if (ConfigurationManager.AppSettings["ControlarConsultasImpressao"] == "SIM")
                    Servicos.StatusConsultaImpressaoUnidade.VerificarStatusConsultaImpressaoUnidade(numeroUnidade, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, false, unidadeDeTrabalho);

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoMDFe>>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoMDFe>> BuscarPorTipoDeIntegracao(Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, int codigoEmpresaPai, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaPai);

                if (empresaPai == null)
                    return new Retorno<List<RetornoMDFe>>() { Mensagem = "Empresa pai não encontrada.", Status = false };

                if (empresaPai.Configuracao != null && empresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<RetornoMDFe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.IntegracaoMDFe repIntegracao = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoMDFe> listaIntegracoes = repIntegracao.Buscar(codigoEmpresaPai, numeroUnidade.ToString(), numeroCarga.ToString(), tipoIntegracao);

                Retorno<List<RetornoMDFe>> retorno = new Retorno<List<RetornoMDFe>> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                List<RetornoMDFe> dadosRetorno = new List<RetornoMDFe>();

                foreach (Dominio.Entidades.IntegracaoMDFe integracao in listaIntegracoes)
                {
                    RetornoMDFe retornoMDFe = new RetornoMDFe()
                    {
                        CodigoMDFe = integracao.MDFe.Codigo,
                        ChaveMDFe = integracao.MDFe.Chave,
                        CNPJEmpresa = integracao.MDFe.Empresa.CNPJ,
                        Tipo = integracao.Tipo,
                        NomeArquivo = integracao.NomeArquivo,
                        Arquivo = integracao.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao ? integracao.Arquivo : string.Empty,
                        StatusMDFe = integracao.MDFe.Status,
                        DataEmissao = integracao.MDFe.DataEmissao.HasValue ? integracao.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        NumeroMDFe = integracao.MDFe.Numero,
                        SerieMDFe = integracao.MDFe.Serie.Numero,
                        MensagemRetorno = integracao.MDFe.MensagemStatus != null ? string.Concat(integracao.MDFe.MensagemStatus.CodigoDoErro, " - ", integracao.MDFe.MensagemStatus.MensagemDoErro) : integracao.MDFe.MensagemRetornoSefaz,
                        CargaPropria = integracao.MDFe.Empresa.Configuracao.TipoGeracaoCTeWS == Dominio.Enumeradores.TipoGeracaoCTeWS.GerarMDFe ? true : false
                    };

                    this.ObterArquivosRetorno(ref retornoMDFe, integracao, tipoIntegracao, tipoRetorno, unidadeDeTrabalho);

                    dadosRetorno.Add(retornoMDFe);
                }

                retorno.Objeto = dadosRetorno;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoMDFe>>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<RetornoMDFe> BuscarPorCodigoMDFe(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token, string codificarUTF8)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                bool vCodificarUTF8 = false;
                if (string.IsNullOrWhiteSpace(codificarUTF8) || codificarUTF8 == "S")
                    vCodificarUTF8 = true;

                Repositorio.IntegracaoMDFe repIntegracao = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);
                List<Dominio.Entidades.IntegracaoMDFe> listaIntegracoes = repIntegracao.BuscarPorMDFeETipo(codigoMDFe, tipoIntegracao);

                Retorno<RetornoMDFe> retorno = new Retorno<RetornoMDFe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                if (listaIntegracoes.Count() > 0)
                {
                    if (listaIntegracoes[0].MDFe.Empresa.EmpresaPai.Configuracao != null && listaIntegracoes[0].MDFe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<RetornoMDFe>() { Mensagem = "Token de acesso inválido.", Status = false };

                    RetornoMDFe retornoMDFe = new RetornoMDFe()
                    {
                        CodigoMDFe = listaIntegracoes[0].MDFe.Codigo,
                        ChaveMDFe = listaIntegracoes[0].MDFe.Chave,
                        CNPJEmpresa = listaIntegracoes[0].MDFe.Empresa.CNPJ,
                        Tipo = listaIntegracoes[0].Tipo,
                        NomeArquivo = listaIntegracoes[0].NomeArquivo,
                        Arquivo = listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao ? listaIntegracoes[0].Arquivo : string.Empty,
                        StatusMDFe = listaIntegracoes[0].MDFe.Status,
                        DataEmissao = listaIntegracoes[0].MDFe.DataEmissao.HasValue ? listaIntegracoes[0].MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        NumeroMDFe = listaIntegracoes[0].MDFe.Numero,
                        SerieMDFe = listaIntegracoes[0].MDFe.Serie.Numero,
                        MensagemRetorno = listaIntegracoes[0].MDFe.MensagemStatus != null ? string.Concat(listaIntegracoes[0].MDFe.MensagemStatus.CodigoDoErro, " - ", listaIntegracoes[0].MDFe.MensagemStatus.MensagemDoErro) : listaIntegracoes[0].MDFe.MensagemRetornoSefaz,
                        CodigoRetorno = listaIntegracoes[0].MDFe.MensagemStatus != null ? (int?)listaIntegracoes[0].MDFe.MensagemStatus.CodigoDoErro : 
                                         listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? 100 : 
                                         this.ObterCodigoRetornoSefaz(listaIntegracoes[0].MDFe.MensagemRetornoSefaz),
                        DataHoraRecibo = listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && listaIntegracoes[0].MDFe.DataAutorizacao.HasValue ? listaIntegracoes[0].MDFe.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : 
                                         listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado && listaIntegracoes[0].MDFe.DataCancelamento.HasValue ? listaIntegracoes[0].MDFe.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss") :
                                         listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado && listaIntegracoes[0].MDFe.DataEncerramento.HasValue ? listaIntegracoes[0].MDFe.DataEncerramento.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        NumeroProtocolo = listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? (listaIntegracoes[0].MDFe.Protocolo ?? string.Empty) :
                                          listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ? (listaIntegracoes[0].MDFe.ProtocoloCancelamento ?? string.Empty) :
                                          listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ? (listaIntegracoes[0].MDFe.ProtocoloEncerramento ?? string.Empty) : string.Empty
                    };

                    this.ObterArquivosRetorno(ref retornoMDFe, listaIntegracoes[0], tipoIntegracao, tipoRetorno, unidadeDeTrabalho, vCodificarUTF8);

                    retorno.Objeto = retornoMDFe;
                }
                else
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                    if (mdfe != null)
                    {
                        if (mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                            return new Retorno<RetornoMDFe>() { Mensagem = "Token de acesso inválido.", Status = false };

                        RetornoMDFe retornoMDFe = new RetornoMDFe()
                        {
                            CodigoMDFe = mdfe.Codigo,
                            ChaveMDFe = mdfe.Chave,
                            CNPJEmpresa = mdfe.Empresa.CNPJ,
                            Tipo = Dominio.Enumeradores.TipoIntegracaoMDFe.Todos,
                            NomeArquivo = string.Empty,
                            Arquivo = string.Empty,
                            StatusMDFe = mdfe.Status,
                            DataEmissao = mdfe.DataEmissao.HasValue ? mdfe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                            NumeroMDFe = mdfe.Numero,
                            SerieMDFe = mdfe.Serie.Numero,
                            MensagemRetorno = mdfe.MensagemStatus != null ? string.Concat(mdfe.MensagemStatus.CodigoDoErro, " - ", mdfe.MensagemStatus.MensagemDoErro) : mdfe.MensagemRetornoSefaz,
                            CodigoRetorno = mdfe.MensagemStatus != null ? (int?)mdfe.MensagemStatus.CodigoDoErro : 
                                            mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? 100 : 
                                            this.ObterCodigoRetornoSefaz(mdfe.MensagemRetornoSefaz),
                            DataHoraRecibo = mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && mdfe.DataAutorizacao.HasValue ? mdfe.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm:ss") :
                                            mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado && mdfe.DataCancelamento.HasValue ? mdfe.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss") :
                                            mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado && mdfe.DataEncerramento.HasValue ? mdfe.DataEncerramento.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                        };

                        this.ObterArquivosRetorno(ref retornoMDFe, mdfe, Dominio.Enumeradores.TipoIntegracaoMDFe.Todos, tipoRetorno, unidadeDeTrabalho, vCodificarUTF8);

                        retorno.Objeto = retornoMDFe;
                    }
                }



                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoMDFe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave> BuscarStatusMDFePorChave(Dominio.ObjetosDeValor.MDFe.ConsultaStatusMDFePorChave consultaStatusMDFePorChave)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (consultaStatusMDFePorChave == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Parâmetros de consulta não informados.", Status = false };

                if (string.IsNullOrWhiteSpace(consultaStatusMDFePorChave.ChaveMDFe))
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Chave do MDF-e não informada.", Status = false };

                if (consultaStatusMDFePorChave.ChaveMDFe.Length != 44)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Chave de acesso inválida.", Status = false };

                if (string.IsNullOrWhiteSpace(consultaStatusMDFePorChave.CNPJEmpresa))
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "CNPJ da empresa não informado.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(consultaStatusMDFePorChave.CNPJEmpresa));

                if (empresa == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Empresa não encontrada.", Status = false };

                if (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != consultaStatusMDFePorChave.Token)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Token de acesso inválido.", Status = false };

                Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave retornoConsulta = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFe.ConsultarMdfeEmissorExterno(consultaStatusMDFePorChave.ChaveMDFe, unitOfWork);

                if (retornoConsulta == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Não foi possível consultar o status do MDF-e.", Status = false };

                Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave retornoStatus = new Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave
                {
                    Ambiente = retornoConsulta.Ambiente != null ? (int)retornoConsulta.Ambiente : 0,
                    CodigoStatus = retornoConsulta.CodigoStatus,
                    MensagemStatus = retornoConsulta.MensagemStatus
                };

                return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Retorno realizado com sucesso.", Status = true, Objeto = retornoStatus };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave>() { Mensagem = "Ocorreu uma falha ao consultar status do MDF-e.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<RetornoMDFe> BuscarPorCodigoMDFeAverbado(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoMDFe repIntegracao = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);
                List<Dominio.Entidades.IntegracaoMDFe> listaIntegracoes = repIntegracao.BuscarPorMDFeETipo(codigoMDFe, tipoIntegracao);

                Retorno<RetornoMDFe> retorno = new Retorno<RetornoMDFe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                if (listaIntegracoes.Count() > 0)
                {
                    if (listaIntegracoes[0].MDFe.Empresa.EmpresaPai.Configuracao != null && listaIntegracoes[0].MDFe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<RetornoMDFe>() { Mensagem = "Token de acesso inválido.", Status = false };

                    Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unidadeDeTrabalho);

                    svcAverbacao.ConsultarAverbacoes(codigoMDFe, unidadeDeTrabalho);

                    Repositorio.AverbacaoMDFe repAverbacao = new Repositorio.AverbacaoMDFe(unidadeDeTrabalho);

                    if (ConfigurationManager.AppSettings["RetornarApenasMDFeAverbado"] == "SIM")
                    {
                        if (repAverbacao.ContarPorMDFeTipoEStatus(listaIntegracoes[0].MDFe.Codigo, Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao, new Dominio.Enumeradores.StatusAverbacaoMDFe[] { Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso }) == 0)
                            return new Retorno<RetornoMDFe>() { Mensagem = "MDFe não está averbado.", Status = false };
                    }

                    RetornoMDFe retornoMDFe = new RetornoMDFe()
                    {
                        CodigoMDFe = listaIntegracoes[0].MDFe.Codigo,
                        ChaveMDFe = listaIntegracoes[0].MDFe.Chave,
                        CNPJEmpresa = listaIntegracoes[0].MDFe.Empresa.CNPJ,
                        Tipo = listaIntegracoes[0].Tipo,
                        NomeArquivo = listaIntegracoes[0].NomeArquivo,
                        Arquivo = listaIntegracoes[0].MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao ? listaIntegracoes[0].Arquivo : string.Empty,
                        StatusMDFe = listaIntegracoes[0].MDFe.Status,
                        DataEmissao = listaIntegracoes[0].MDFe.DataEmissao.HasValue ? listaIntegracoes[0].MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                        NumeroMDFe = listaIntegracoes[0].MDFe.Numero,
                        SerieMDFe = listaIntegracoes[0].MDFe.Serie.Numero,
                        MensagemRetorno = listaIntegracoes[0].MDFe.MensagemStatus != null ? string.Concat(listaIntegracoes[0].MDFe.MensagemStatus.CodigoDoErro, " - ", listaIntegracoes[0].MDFe.MensagemStatus.MensagemDoErro) : listaIntegracoes[0].MDFe.MensagemRetornoSefaz
                    };

                    this.ObterArquivosRetorno(ref retornoMDFe, listaIntegracoes[0], tipoIntegracao, tipoRetorno, unidadeDeTrabalho);

                    retorno.Objeto = retornoMDFe;
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoMDFe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<object> Alterar(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.StatusIntegracao statusIntegracao, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new Retorno<object>() { Mensagem = "MDF-e não encontrado.", Status = false };

                if (token == "")
                    token = null;

                if (mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<object>() { Mensagem = "Token de acesso inválido.", Status = false };

                Repositorio.IntegracaoMDFe repIntegracao = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.IntegracaoMDFe> integracoes = repIntegracao.Buscar(codigoMDFe, tipoIntegracao);

                if (integracoes.Count() > 0)
                {

                    foreach (Dominio.Entidades.IntegracaoMDFe integracao in integracoes)
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

        public Retorno<RetornoImpressora> IncluirImpressora(int numeroUnidade, string nomeImpressora, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (numeroUnidade == 0)
                    return new Retorno<RetornoImpressora>() { Mensagem = "Número da unidade inválido.", Status = false };

                if (token == "")
                    token = null;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(1);

                if (empresa != null)
                {
                    if (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<RetornoImpressora>() { Mensagem = "Token de acesso inválido.", Status = false };
                }

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unidadeDeTrabalho);

                Dominio.Entidades.Impressora impressora = repImpressora.BuscarPorUnidade(numeroUnidade, "A");

                var inserir = true;
                if (impressora != null)
                {
                    inserir = false;

                    string log = "";

                    if (!string.IsNullOrWhiteSpace(nomeImpressora) && impressora.NomeImpressora != nomeImpressora)
                    {
                        if (!string.IsNullOrWhiteSpace(log))
                            string.Concat(log, " ", log = "Impressora de " + impressora.NomeImpressora + " para " + nomeImpressora + " .");
                        else
                            log = "Impressora de " + impressora.NomeImpressora + " para " + nomeImpressora + " .";
                    }

                    if (!string.IsNullOrWhiteSpace(log))
                        impressora.Log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado via WebService " + log);
                }
                else
                {
                    impressora = new Dominio.Entidades.Impressora();

                    impressora.Log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido via WebService.");
                }

                impressora.NumeroDaUnidade = numeroUnidade;
                impressora.NomeImpressora = nomeImpressora;
                impressora.Status = "A";

                if (inserir)
                    repImpressora.Inserir(impressora);
                else
                    repImpressora.Atualizar(impressora);

                RetornoImpressora retornoImpressora = new RetornoImpressora()
                {
                    Codigo = impressora.Codigo,
                    NumeroUnidade = impressora.NumeroDaUnidade,
                    NomeImpressora = impressora.NomeImpressora,
                    Status = impressora.Status
                };

                if (inserir)
                    return new Retorno<RetornoImpressora>() { Mensagem = "Impressora incluida com sucesso.", Objeto = retornoImpressora, Status = true };
                else
                    return new Retorno<RetornoImpressora>() { Mensagem = "Impressora alterada com sucesso.", Objeto = retornoImpressora, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoImpressora>() { Mensagem = "Ocorreu uma falha ao incluir impressora.", Status = false };
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

        public Retorno<RetornoImpressora> AlterarImpressora(int codigo, string numeroUnidade, string nomeImpressora, string status, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (codigo == 0 && string.IsNullOrWhiteSpace(numeroUnidade))
                    return new Retorno<RetornoImpressora>() { Mensagem = "Para alterar é necessário passar o código ou o número da unidade não informado.", Status = false };

                int unidade = 0;
                if (!string.IsNullOrWhiteSpace(numeroUnidade))
                    int.TryParse(Utilidades.String.OnlyNumbers(numeroUnidade), out unidade);

                if (token == "")
                    token = null;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(1);

                if (empresa != null)
                {
                    if (empresa.Configuracao != null && empresa.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<RetornoImpressora>() { Mensagem = "Token de acesso inválido.", Status = false };
                }

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unidadeDeTrabalho);

                Dominio.Entidades.Impressora impressora = null;

                if (codigo > 0)
                    impressora = repImpressora.BuscarPorCodigo(codigo);
                else
                    impressora = repImpressora.BuscarPorUnidade(unidade, "A");

                if (impressora == null)
                {
                    if (codigo > 0)
                        return new Retorno<RetornoImpressora>() { Mensagem = "Nenhuma Impressora codigo " + codigo.ToString() + " cadastrada .", Status = false };
                    else
                        return new Retorno<RetornoImpressora>() { Mensagem = "Nenhuma Impressora Ativa para a unidade " + numeroUnidade + " .", Status = false };
                }

                string log = "";

                if (!string.IsNullOrWhiteSpace(status) && impressora.Status != status)
                {
                    if (status == "I")
                        log = "Status Inativo.";
                    else
                        log = "Status Ativo.";
                }

                if (!string.IsNullOrWhiteSpace(nomeImpressora) && impressora.NomeImpressora != nomeImpressora)
                {
                    if (!string.IsNullOrWhiteSpace(log))
                        string.Concat(log, " ", log = "Impressora de " + impressora.NomeImpressora + " para " + nomeImpressora + " .");
                    else
                        log = "Impressora de " + impressora.NomeImpressora + " para " + nomeImpressora + " .";
                }


                if (!string.IsNullOrWhiteSpace(status))
                    impressora.Status = status;

                if (!string.IsNullOrWhiteSpace(nomeImpressora))
                    impressora.NomeImpressora = nomeImpressora;

                impressora.Log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado via WebService " + log);

                repImpressora.Atualizar(impressora);

                RetornoImpressora retornoImpressora = new RetornoImpressora()
                {
                    Codigo = impressora.Codigo,
                    NumeroUnidade = impressora.NumeroDaUnidade,
                    NomeImpressora = impressora.NomeImpressora,
                    Status = impressora.Status
                };

                return new Retorno<RetornoImpressora>() { Mensagem = "Impressora alterada com sucesso.", Objeto = retornoImpressora, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoImpressora>() { Mensagem = "Ocorreu uma falha ao incluir impressora.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void ObterMDFeImpressao(ref RetornoMDFe retorno, Dominio.Entidades.IntegracaoMDFe integracaoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true)
        {
            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

            Servicos.ServicoMDFe.RetornoMDFe retornoMDFe = null;

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unidadeDeTrabalho);
                Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(integracaoMDFe.MDFe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);

                if (xmlMDFe != null && !string.IsNullOrWhiteSpace(xmlMDFe.XML))
                    retorno.XML = xmlMDFe.XML;
                else
                {
                    retornoMDFe = servicoMDFe.ObterDadosIntegradosMDFe(integracaoMDFe.MDFe.Codigo, integracaoMDFe.MDFe.Empresa.Codigo, unidadeDeTrabalho);
                    retorno.XML = retornoMDFe.XML;
                }
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao)
                {
                    string ambiente = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    if (!string.IsNullOrWhiteSpace(ambiente) && ambiente == "APTI")
                        retorno.PDF = servicoMDFe.ObterDAMDFE(integracaoMDFe.MDFe.Codigo, unidadeDeTrabalho); //Buscar no Oracle -> APTI
                    else
                        retorno.PDF = servicoMDFe.ObterDAMDFE(integracaoMDFe.MDFe.Codigo, integracaoMDFe.MDFe.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8);

                }
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.TXT)
            {
                retorno.TXT = this.ObterRetornoTxt(integracaoMDFe);
            }
        }

        private void ObterArquivosRetorno(ref RetornoMDFe retorno, Dominio.Entidades.IntegracaoMDFe integracaoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true)
        {
            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && (integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao || tipoIntegracao == Dominio.Enumeradores.TipoIntegracaoMDFe.Todos))
                {
                    retorno.XML = servicoMDFe.ObterStringXML(integracaoMDFe.MDFe, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao, unidadeDeTrabalho);
                }
                else if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado && (integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Encerramento || tipoIntegracao == Dominio.Enumeradores.TipoIntegracaoMDFe.Todos))
                {
                    retorno.XML = servicoMDFe.ObterStringXML(integracaoMDFe.MDFe, Dominio.Enumeradores.TipoXMLMDFe.Encerramento, unidadeDeTrabalho);
                }
                else if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado && (integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Cancelamento || tipoIntegracao == Dominio.Enumeradores.TipoIntegracaoMDFe.Todos))
                {
                    retorno.XML = servicoMDFe.ObterStringXML(integracaoMDFe.MDFe, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento, unidadeDeTrabalho);
                }
                if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia && integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao)
                {
                    retorno.XML = servicoMDFe.ObterStringXML(integracaoMDFe.MDFe, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao, unidadeDeTrabalho);
                }
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao)
                {
                    string ambiente = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    if (!string.IsNullOrWhiteSpace(ambiente) && ambiente == "APTI")
                        retorno.PDF = servicoMDFe.ObterDAMDFE(integracaoMDFe.MDFe.Codigo, unidadeDeTrabalho); //Buscar no Oracle -> APTI
                    else
                        retorno.PDF = servicoMDFe.ObterDAMDFE(integracaoMDFe.MDFe.Codigo, integracaoMDFe.MDFe.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8);

                }
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.TXT)
            {
                retorno.TXT = this.ObterRetornoTxt(integracaoMDFe);
            }
        }

        private void ObterArquivosRetorno(ref RetornoMDFe retorno, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, Repositorio.UnitOfWork unidadeDeTrabalho, bool codificarUTF8 = true)
        {
            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);

            Servicos.ServicoMDFe.RetornoMDFe retornoMDFe = null;
            Servicos.ServicoMDFe.RetornoEventoMDFe retornoEventoMDFe = null;

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    retornoMDFe = servicoMDFe.ObterDadosIntegradosMDFe(mdfe.Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho);
                    retorno.XML = retornoMDFe.XML;
                }
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                {
                    retornoEventoMDFe = servicoMDFe.ObterDadosIntegradosEventoMDFe(mdfe.CodigoIntegradorEncerramento, unidadeDeTrabalho);
                    retorno.XML = retornoEventoMDFe.XML;
                }
                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado)
                {
                    retornoEventoMDFe = servicoMDFe.ObterDadosIntegradosEventoMDFe(mdfe.CodigoIntegradorCancelamento, unidadeDeTrabalho);
                    retorno.XML = retornoEventoMDFe.XML;
                }
                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia)
                {
                    retornoMDFe = servicoMDFe.ObterDadosIntegradosMDFe(mdfe.Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho);
                    retorno.XML = retornoMDFe.XML;
                }
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    string ambiente = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                    if (!string.IsNullOrWhiteSpace(ambiente) && ambiente == "APTI")
                        retorno.PDF = servicoMDFe.ObterDAMDFE(mdfe.Codigo, unidadeDeTrabalho); //Buscar no Oracle -> APTI
                    else
                        retorno.PDF = servicoMDFe.ObterDAMDFE(mdfe.Codigo, mdfe.Empresa.Codigo, unidadeDeTrabalho, codificarUTF8);

                }
            }
        }

        private string ObterRetornoTxt(Dominio.Entidades.IntegracaoMDFe integracaoMDFe)
        {
            if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao)
                return string.Concat(integracaoMDFe.MDFe.Chave, ";",
                                     integracaoMDFe.MDFe.Protocolo, ";",
                                     integracaoMDFe.MDFe.DataAutorizacao.HasValue ? integracaoMDFe.MDFe.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty, ";",
                                     integracaoMDFe.NumeroDaCarga);
            else if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado && integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Cancelamento)
                return string.Concat(integracaoMDFe.MDFe.Chave, ";",
                                     integracaoMDFe.MDFe.ProtocoloCancelamento, ";",
                                     integracaoMDFe.MDFe.DataCancelamento.HasValue ? integracaoMDFe.MDFe.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty, ";",
                                     integracaoMDFe.NumeroDaCarga);
            else if (integracaoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado && integracaoMDFe.Tipo == Dominio.Enumeradores.TipoIntegracaoMDFe.Encerramento)
                return string.Concat(integracaoMDFe.MDFe.Chave, ";",
                                     integracaoMDFe.MDFe.ProtocoloEncerramento, ";",
                                     integracaoMDFe.MDFe.DataEncerramento.HasValue ? integracaoMDFe.MDFe.DataEncerramento.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty, ";",
                                     integracaoMDFe.NumeroDaCarga);
            else
                return string.Empty;
        }

        private int? ObterCodigoRetornoSefaz(string mensagemRetornoSefaz)
        {
            if (string.IsNullOrWhiteSpace(mensagemRetornoSefaz))
                return null;

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\s-\s(\d+)(?:\s-|:)");
            System.Text.RegularExpressions.Match match = regex.Match(mensagemRetornoSefaz);

            if (match.Success && match.Groups.Count > 1)
            {
                if (int.TryParse(match.Groups[1].Value, out int codigo))
                    return codigo;
            }

            return null;
        }

        #endregion
    }
}
