using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos
{
    public class Averbacao : ServicoBase
    {
        public Averbacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public bool ConsultarAverbacoes(int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    throw new Exception("CT-e é nulo para a consulta de averbações.");

                Repositorio.AverbacaoCTe repAverbacao = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);

                ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

                ServicoCTe.RetornoAverbacaoCTe retorno = svcCTe.ConsultarAverbacoesCTe(cte.CodigoCTeIntegrador);

                if (retorno.Info.Tipo == "ERRO")
                    return false;

                unidadeDeTrabalho.Start();

                List<Dominio.Entidades.AverbacaoCTe> averbacoesExistentes = repAverbacao.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                foreach (ServicoCTe.AverbacaoCTe averbacaoIntegrada in retorno.Averbacoes)
                {
                    var averbacao = (from obj in averbacoesExistentes where obj.CodigoIntegracao == averbacaoIntegrada.CodigoAverbacao select obj).FirstOrDefault();

                    if (averbacao == null)
                        averbacao = new Dominio.Entidades.AverbacaoCTe();

                    averbacao.CodigoIntegracao = averbacaoIntegrada.CodigoAverbacao;
                    averbacao.CodigoRetorno = averbacaoIntegrada.CodigoRetorno;
                    averbacao.CTe = cte;
                    averbacao.DataRetorno = averbacaoIntegrada.DataProtocolo;
                    averbacao.MensagemRetorno = averbacaoIntegrada.MensagemRetorno;
                    averbacao.Protocolo = averbacaoIntegrada.NumeroProtocolo;
                    averbacao.Status = averbacaoIntegrada.Status == "A" ? Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso : averbacaoIntegrada.Status == "I" ? Dominio.Enumeradores.StatusAverbacaoCTe.Pendente : Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = averbacaoIntegrada.Tipo == "A" ? Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao : Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    averbacao.SeguradoraAverbacao = averbacaoIntegrada.Seguradora == "A" ? Dominio.Enumeradores.IntegradoraAverbacao.ATM : averbacaoIntegrada.Seguradora == "B" ? Dominio.Enumeradores.IntegradoraAverbacao.Quorum : averbacaoIntegrada.Seguradora == "P" ? Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro : Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;
                    averbacao.Averbacao = averbacaoIntegrada.Averbacao;
                    if (string.IsNullOrWhiteSpace(averbacao.Averbacao) && averbacao.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao)
                        averbacao.Averbacao = averbacaoIntegrada.NumeroProtocolo;

                    if (averbacao.Codigo > 0)
                        repAverbacao.Atualizar(averbacao);
                    else
                        repAverbacao.Inserir(averbacao);
                }

                unidadeDeTrabalho.CommitChanges();

                if ((from obj in retorno.Averbacoes where obj.Status == "I" select obj).Any())
                    return false;
                else
                    return true;
            }
            catch
            {
                unidadeDeTrabalho.Rollback();

                throw;
            }
        }

        public bool Emitir(int codigoCTe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Entidades.ConfiguracaoAverbacaoClientes configuracaoCliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte == null)
            {
                Servicos.Log.TratarErro("CT-e " + codigoCTe.ToString() + " não localizado paraaverbação CTe");
                return false;
            }

            ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

            string seguradora = configuracaoCliente != null && configuracaoCliente.IntegradoraAverbacao != null ?
                                configuracaoCliente.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" :
                                configuracaoCliente.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" :
                                configuracaoCliente.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" :
                                configuracaoCliente.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ELT ? "E" : string.Empty : string.Empty;

            if (string.IsNullOrWhiteSpace(seguradora))
                seguradora = cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.SeguradoraAverbacao != null ?
                             cte.Empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" :
                             cte.Empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" :
                             cte.Empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" :
                             cte.Empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ELT ? "E" : string.Empty : string.Empty;

            if (!string.IsNullOrWhiteSpace(seguradora))
            {
                if (seguradora == "E") //ELT
                {
                    Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);

                    Dominio.Entidades.AverbacaoCTe averbacao = new Dominio.Entidades.AverbacaoCTe();
                    averbacao.CTe = cte;
                    averbacao.Tipo = tipo;
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                    averbacao.SeguradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.ELT;
                    repAverbacaoCTe.Inserir(averbacao);

                    if (tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao)
                        Servicos.Embarcador.Integracao.ELT.IntegracaoELT.AverbarDocumento(null, averbacao, unidadeDeTrabalho);
                    else
                        Servicos.Embarcador.Integracao.ELT.IntegracaoELT.CancelarAverbacaoDocumento(null, averbacao, unidadeDeTrabalho);

                    return false; //Retorna False para não adicionar na fila
                }
                else if (seguradora == "P") //PORTO SEGURO
                {
                    string usuarioPorto = configuracaoCliente != null && !string.IsNullOrWhiteSpace(configuracaoCliente.UsuarioAverbacao) ? configuracaoCliente.UsuarioAverbacao : !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.UsuarioSeguroATM) ? cte.Empresa.Configuracao.UsuarioSeguroATM : string.Empty;
                    string senhaPorto = configuracaoCliente != null && !string.IsNullOrWhiteSpace(configuracaoCliente.SenhaAverbacao) ? configuracaoCliente.SenhaAverbacao : !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.SenhaSeguroATM) ? cte.Empresa.Configuracao.SenhaSeguroATM : string.Empty;

                    Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);

                    Dominio.Entidades.AverbacaoCTe averbacao = new Dominio.Entidades.AverbacaoCTe();
                    averbacao.CTe = cte;
                    averbacao.Tipo = tipo;
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                    averbacao.SeguradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro;
                    repAverbacaoCTe.Inserir(averbacao);

                    int tentativasAverbar = 2;

                    if (tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao)
                        Servicos.Embarcador.Integracao.PortoSeguro.IntegracaoPortoSeguro.AverbarDocumento(null, averbacao, ref tentativasAverbar, unidadeDeTrabalho, usuarioPorto, senhaPorto);
                    else
                        Servicos.Embarcador.Integracao.PortoSeguro.IntegracaoPortoSeguro.CancelarAverbacaoDocumento(null, averbacao, ref tentativasAverbar, unidadeDeTrabalho, usuarioPorto, senhaPorto);

                    return false; //Retorna False para não adicionar na fila
                }
                else
                {
                    string usuarioATM = configuracaoCliente != null && !string.IsNullOrWhiteSpace(configuracaoCliente.UsuarioAverbacao) ? configuracaoCliente.UsuarioAverbacao : !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.UsuarioSeguroATM) ? cte.Empresa.Configuracao.UsuarioSeguroATM : string.Empty;
                    string senhaATM = configuracaoCliente != null && !string.IsNullOrWhiteSpace(configuracaoCliente.SenhaAverbacao) ? configuracaoCliente.SenhaAverbacao : !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.SenhaSeguroATM) ? cte.Empresa.Configuracao.SenhaSeguroATM : string.Empty;
                    string codigoATM = configuracaoCliente != null && !string.IsNullOrWhiteSpace(configuracaoCliente.CodigoAverbacao) ? configuracaoCliente.CodigoAverbacao : !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.CodigoSeguroATM) ? cte.Empresa.Configuracao.CodigoSeguroATM : string.Empty;
                    string tokenBradesco = configuracaoCliente != null && !string.IsNullOrWhiteSpace(configuracaoCliente.TokenAverbacao) ? configuracaoCliente.TokenAverbacao : !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.TokenAverbacaoBradesco) ? cte.Empresa.Configuracao.TokenAverbacaoBradesco : string.Empty;
                    string wsdlQuorum = !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.WsdlAverbacaoQuorum) ? cte.Empresa.Configuracao.WsdlAverbacaoQuorum : string.Empty;

                    ServicoCTe.ResultadoInteger retorno = svcCTe.ImportarAverbacao(cte.CodigoCTeIntegrador, tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao ? ServicoCTe.TipoAverbacao.Autorizacao : ServicoCTe.TipoAverbacao.Cancelamento, seguradora, codigoATM, usuarioATM, senhaATM, tokenBradesco, "N", wsdlQuorum);

                    if (retorno.Valor > 0)
                        return true;
                    else
                    {
                        Servicos.Log.TratarErro("Retorno tentativa averbação CTe " + cte.Chave + " " + retorno.Info.MensagemOriginal);
                        return false;
                    }
                }
            }
            else
                return false;

        }

        public bool EmitirEmpresaPai(int codigoCTe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte == null)
            {
                Servicos.Log.TratarErro("CT-e " + codigoCTe.ToString() + " não localizado paraaverbação CTe");
                return false;
            }

            ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

            string seguradora = cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao != null ?
                                cte.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" :
                                cte.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" :
                                cte.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : "" : "";

            if (!string.IsNullOrWhiteSpace(seguradora))
            {
                string usuarioATM = cte.Empresa.EmpresaPai.Configuracao.UsuarioSeguroATM;
                string senhaATM = cte.Empresa.EmpresaPai.Configuracao.SenhaSeguroATM;
                string codigoATM = cte.Empresa.EmpresaPai.Configuracao.CodigoSeguroATM;
                string tokenBradesco = cte.Empresa.EmpresaPai.Configuracao.TokenAverbacaoBradesco;
                string averbarComoEmbarcador = cte.Empresa.EmpresaPai.Configuracao.AverbarComoEmbarcador ? "S" : "N";
                string wsdlQuorum = !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.WsdlAverbacaoQuorum) ? cte.Empresa.EmpresaPai.Configuracao.WsdlAverbacaoQuorum : string.Empty;

                ServicoCTe.ResultadoInteger retorno = svcCTe.ImportarAverbacao(cte.CodigoCTeIntegrador, tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao ? ServicoCTe.TipoAverbacao.Autorizacao : ServicoCTe.TipoAverbacao.Cancelamento, seguradora, codigoATM, usuarioATM, senhaATM, tokenBradesco, averbarComoEmbarcador, wsdlQuorum);

                if (retorno.Valor > 0)
                    return true;
                else
                {
                    Servicos.Log.TratarErro("Retorno tentativa averbação empresa pai CTe " + cte.Chave + " " + retorno.Info.MensagemOriginal);
                    return false;
                }
            }
            else
                return false;
        }

        public bool AverbarCTeEmbarcador(Dominio.Entidades.AverbacaoCTe averbacao, Dominio.Enumeradores.TipoAverbacaoCTe tipo, ref int codigoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            // Repositorios
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unidadeDeTrabalho);
            Repositorio.Embarcador.Seguros.AverbacaoBradesco repAverbacaoBradesco = new Repositorio.Embarcador.Seguros.AverbacaoBradesco(unidadeDeTrabalho);
            Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unidadeDeTrabalho);

            // CTe
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = averbacao.CTe;

            // Apolcie
            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice = averbacao.ApoliceSeguroAverbacao.ApoliceSeguro;

            // Valida
            if (cte == null)
                throw new Exception("O CT-e não pode ser nulo para emitir uma averbação.");

            // Servico CTe
            ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

            // Define Seguradora
            string seguradora = apolice.SeguradoraAverbacao.ObterSigla();

            // Autenticação
            string usuarioATM = string.Empty;
            string senhaATM = string.Empty;
            string codigoATM = string.Empty;
            string tokenBradesco = string.Empty;
            string averbarComoEmbarcador = "N";
            string wsdlQuorum = string.Empty;

            //Instancia configurações
            if (apolice.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
            {
                Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);
                if (averbacaoATM == null) throw new Exception("Sem configuração de averbação ATM.");

                usuarioATM = averbacaoATM.Usuario;
                senhaATM = averbacaoATM.Senha;
                codigoATM = averbacaoATM.CodigoATM;
                averbarComoEmbarcador = averbacaoATM.AverbaComoEmbarcador ? "S" : "N";
            }
            else if (apolice.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco)
            {
                Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco averbacaoBradesco = repAverbacaoBradesco.BuscarPorApolice(apolice.Codigo);
                if (averbacaoBradesco == null) throw new Exception("Sem configuração de averbação Bradesco.");

                tokenBradesco = averbacaoBradesco.Token;
                wsdlQuorum = averbacaoBradesco.WSDLQuorum;
            }
            else if (apolice.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro)
            {
                Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacaoPortoSeguro = repAverbacaoPortoSeguro.BuscarPorApolice(apolice.Codigo);
                if (averbacaoPortoSeguro == null) throw new Exception("Sem configuração de averbação Porto Seguro.");

                usuarioATM = averbacaoPortoSeguro.Usuario;
                senhaATM = averbacaoPortoSeguro.Senha;
            }

            // Chama metodo de averbação
            ServicoCTe.ResultadoInteger retorno = svcCTe.ImportarAverbacao(cte.CodigoCTeIntegrador, tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao ? ServicoCTe.TipoAverbacao.Autorizacao : ServicoCTe.TipoAverbacao.Cancelamento, seguradora, codigoATM, usuarioATM, senhaATM, tokenBradesco, averbarComoEmbarcador, wsdlQuorum);

            // Verifica se requisitou com sucesso a averbação do cte            
            if (retorno.Valor > 0)
            {
                codigoIntegracao = retorno.Valor;
                return true;
            }
            else
            {
                Servicos.Log.TratarErro("Averbação Incompleta: " + retorno.Info.MensagemOriginal);
                return false;
            }
        }

        public bool VerificaAverbacao(int codigoCTe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte == null || cte.Empresa.Configuracao == null)
                return false;

            Repositorio.ConfiguracaoAverbacaoSerie repConfiguracaoAverbacaoSerie = new Repositorio.ConfiguracaoAverbacaoSerie(unidadeDeTrabalho);
            Dominio.Entidades.ConfiguracaoAverbacaoSerie configuracaoAverbacaoSerieNaoAverbar = repConfiguracaoAverbacaoSerie.BuscarPorSerie(cte.Serie.Codigo, true);

            if (configuracaoAverbacaoSerieNaoAverbar != null)
            {
                Servicos.Log.TratarErro("CTe " + cte.Chave + ": Série configurada para não averbar.", "Averbar");
                return false;
            }

            Dominio.Entidades.ProprietarioVeiculoCTe proprietarioTerceiro = cte.Veiculos != null && cte.Veiculos.Count > 0 ? cte.Veiculos.Where(o => o.Proprietario != null).Select(o => o.Proprietario).FirstOrDefault() : null;
            if (proprietarioTerceiro != null)
            {
                double.TryParse(proprietarioTerceiro.CPF_CNPJ, out double cnpjProprietario);
                Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(cte.Empresa.Codigo, cnpjProprietario);
                if (dadosCliente?.NaoAverbarQuandoTerceiro ?? false)
                {
                    Servicos.Log.TratarErro("CTe " + cte.Chave + ": Proprietario veiculo configurado para não averbar.", "Averbar");
                    return false;
                }
            }

            //Se CTe já possui  numero de averbação não averba (Situação da Parati)
            if (cte != null && cte.Seguros != null && cte.Seguros.Count > 0 && cte.Seguros.FirstOrDefault().NumeroAverbacao != null && cte.Seguros.FirstOrDefault().NumeroAverbacao != "")
            {
                Servicos.Log.TratarErro("CTe " + cte.Seguros.FirstOrDefault().NumeroAverbacao + ": CTe já possui número de averbação nos seguros.", "Averbar");
                return false;
            }

            //Se empresa Pai esta configurada para averbar como Embarcador solicita a averbação com os dados da empresa pai (APENAS PARA A ATM
            bool averbacaoEmpresaPai = false;
            if (cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null)
                averbacaoEmpresaPai = EmitirEmpresaPai(cte.Codigo, tipo, unidadeDeTrabalho);

            // Busca as configuracao da empresa
            //Servicos.Log.TratarErro("Averbacao CTe " + codigoCTe + " buscando configuracao empresa", "FilaCTe");
            Repositorio.ConfiguracaoAverbacaoClientes repAverbacao = new Repositorio.ConfiguracaoAverbacaoClientes(unidadeDeTrabalho);
            List<Dominio.Entidades.ConfiguracaoAverbacaoClientes> configAverbacao = repAverbacao.BuscarPorConfiguracao(cte.Empresa.Configuracao.Codigo);

            bool averbacaoEmpresa = false;
            if (configAverbacao != null && configAverbacao.Count > 0)
            {
                for (var i = 0; i < configAverbacao.Count; i++)
                {
                    if (!configAverbacao[i].NaoAverbar) //Somente configuradas para Averbar
                    {
                        switch (configAverbacao[i].TipoTomador)
                        {
                            // Verifica se Destinatario eh o mesmo que na configuracao
                            case Dominio.Enumeradores.TipoTomador.Destinatario:
                                if ((cte.Destinatario != null && !string.IsNullOrWhiteSpace(cte.Destinatario.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Destinatario.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Destinatario != null && !string.IsNullOrWhiteSpace(cte.Destinatario.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Destinatario.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                    averbacaoEmpresa = svcAverbacao.Emitir(cte.Codigo, tipo, configAverbacao[i], unidadeDeTrabalho);
                                break;

                            // Verifica se Expedidor eh o mesmo que na configuracao
                            case Dominio.Enumeradores.TipoTomador.Expedidor:
                                if ((cte.Expedidor != null && !string.IsNullOrWhiteSpace(cte.Expedidor.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Expedidor.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Expedidor != null && !string.IsNullOrWhiteSpace(cte.Expedidor.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Expedidor.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                    averbacaoEmpresa = svcAverbacao.Emitir(cte.Codigo, tipo, configAverbacao[i], unidadeDeTrabalho);
                                break;

                            // Verifica se Recebedor eh o mesmo que na configuracao
                            case Dominio.Enumeradores.TipoTomador.Recebedor:
                                if ((cte.Recebedor != null && !string.IsNullOrWhiteSpace(cte.Recebedor.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Recebedor.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Recebedor != null && !string.IsNullOrWhiteSpace(cte.Recebedor.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Recebedor.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                    averbacaoEmpresa = svcAverbacao.Emitir(cte.Codigo, tipo, configAverbacao[i], unidadeDeTrabalho);
                                break;

                            // Verifica se Remetente eh o mesmo que na configuracao
                            case Dominio.Enumeradores.TipoTomador.Remetente:
                                if ((cte.Remetente != null && !string.IsNullOrWhiteSpace(cte.Remetente.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Remetente.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Remetente != null && !string.IsNullOrWhiteSpace(cte.Remetente.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Remetente.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                    averbacaoEmpresa = svcAverbacao.Emitir(cte.Codigo, tipo, configAverbacao[i], unidadeDeTrabalho);
                                break;

                            // Verifica se Outros/Tomador eh o mesmo que na configuracao
                            case Dominio.Enumeradores.TipoTomador.Outros:
                                if ((cte.Tomador != null && !configAverbacao[i].RaizCNPJ && cte.Tomador.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Tomador != null && !configAverbacao[i].RaizCNPJ && cte.Tomador.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                    averbacaoEmpresa = svcAverbacao.Emitir(cte.Codigo, tipo, configAverbacao[i], unidadeDeTrabalho);
                                break;
                        }
                    }
                }
            }

            //Quando não localizar averbação específica utiliza a padrão
            if (!averbacaoEmpresa)
            {
                bool naoAverbar = false;

                if (configAverbacao != null && configAverbacao.Count > 0)
                {
                    for (var i = 0; i < configAverbacao.Count; i++)
                    {
                        if (configAverbacao[i].NaoAverbar) //Somente configuradas para não Averbar
                        {
                            switch (configAverbacao[i].TipoTomador)
                            {
                                // Verifica se Destinatario eh o mesmo que na configuracao
                                case Dominio.Enumeradores.TipoTomador.Destinatario:
                                    if ((cte.Destinatario != null && !string.IsNullOrWhiteSpace(cte.Destinatario.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Destinatario.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Destinatario != null && !string.IsNullOrWhiteSpace(cte.Destinatario.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Destinatario.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                        naoAverbar = true;
                                    break;

                                // Verifica se Expedidor eh o mesmo que na configuracao
                                case Dominio.Enumeradores.TipoTomador.Expedidor:
                                    if ((cte.Expedidor != null && !string.IsNullOrWhiteSpace(cte.Expedidor.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Expedidor.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Expedidor != null && !string.IsNullOrWhiteSpace(cte.Expedidor.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Expedidor.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                        naoAverbar = true;
                                    break;

                                // Verifica se Recebedor eh o mesmo que na configuracao
                                case Dominio.Enumeradores.TipoTomador.Recebedor:
                                    if ((cte.Recebedor != null && !string.IsNullOrWhiteSpace(cte.Recebedor.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Recebedor.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Recebedor != null && !string.IsNullOrWhiteSpace(cte.Recebedor.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Recebedor.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                        naoAverbar = true;
                                    break;

                                // Verifica se Remetente eh o mesmo que na configuracao
                                case Dominio.Enumeradores.TipoTomador.Remetente:
                                    if ((cte.Remetente != null && !string.IsNullOrWhiteSpace(cte.Remetente.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && cte.Remetente.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Remetente != null && !string.IsNullOrWhiteSpace(cte.Remetente.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && cte.Remetente.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                        naoAverbar = true;
                                    break;

                                // Verifica se Outros/Tomador eh o mesmo que na configuracao
                                case Dominio.Enumeradores.TipoTomador.Outros:
                                    if ((cte.Tomador != null && !configAverbacao[i].RaizCNPJ && cte.Tomador.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (cte.Tomador != null && configAverbacao[i].RaizCNPJ && cte.Tomador.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                                        naoAverbar = true;
                                    break;
                            }
                        }
                    }
                }

                if (!naoAverbar)
                {
                    averbacaoEmpresa = svcAverbacao.Emitir(cte.Codigo, tipo, null, unidadeDeTrabalho);
                }
            }

            if (averbacaoEmpresa || averbacaoEmpresaPai)
            {
                //Servicos.Log.TratarErro("Averbacao CTe " + codigoCTe + " averbacao enviada", "FilaCTe");
                return true;
            }
            else
            {
                //Servicos.Log.TratarErro("Averbacao CTe " + codigoCTe + " averbacao não enviada", "FilaCTe");
                return false;
            }
        }

        public bool VerificarAverbacaoEmbarcador(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.SistemaEmissor != TipoEmissorDocumento.NSTech)
                return false;

            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repositorioApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.AverbacaoCTe repositorioAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apoliceSeguros = ObterApolicesSeguroTransportador(cte.Empresa, unitOfWork);

            bool abriuTransacao = false;
            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abriuTransacao = true;
            }

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro in apoliceSeguros)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao()
                {
                    ApoliceSeguro = apoliceSeguro
                };

                repositorioApoliceSeguroAverbacao.Inserir(seguro);

                if (seguro.ApoliceSeguro.SeguradoraAverbacao == SeguradoraAverbacao.NaoDefinido)
                    continue;

                Dominio.Entidades.AverbacaoCTe averbaCTe = new Dominio.Entidades.AverbacaoCTe
                {
                    CTe = cte,
                    ApoliceSeguroAverbacao = seguro,
                    Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao,
                    SeguradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido,
                    Desconto = seguro.Desconto.HasValue && seguro.Desconto.Value > 0 ? cte.ValorAReceber * (seguro.Desconto.Value / 100) : 0,
                    Percentual = seguro.Desconto,
                };

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros && cte.Status == "A")
                    averbaCTe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao;
                else
                    averbaCTe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;

                repositorioAverbacaoCTe.Inserir(averbaCTe);

                if (seguro.ApoliceSeguro?.ValorFixoAverbacao > 0)
                {
                    cte.ValorCarbaAverbacao = seguro.ApoliceSeguro.ValorFixoAverbacao;

                    repositorioCTe.Atualizar(cte);
                }
            }

            if (abriuTransacao)
                unitOfWork.CommitChanges();

            return true;
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> ObterApolicesSeguroTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorAverbacao repositorioTransportadorAverbacao = new Repositorio.Embarcador.Transportadores.TransportadorAverbacao(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repositorioApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apoliceSeguros = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            if (empresa.UsarTipoOperacaoApolice)
                apoliceSeguros = repositorioTransportadorAverbacao.BuscarApolicePorTransportador(empresa.Codigo);

            if (apoliceSeguros.Count <= 0)
                apoliceSeguros = repositorioApoliceSeguro.BuscarVigentePorEmpresa(empresa.Codigo);

            return apoliceSeguros;
        }
    }
}
