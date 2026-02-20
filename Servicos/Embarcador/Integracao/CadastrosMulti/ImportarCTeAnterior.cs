using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.CadastrosMulti
{
    public class ImportarCTeAnterior
    {
        #region Métodos Globais
        public static bool ConsultarCargaPendenteConsultaCTeAnterior(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.WebService.Carga.DadosAverbacao serWSDadosAverbacao = new Servicos.WebService.Carga.DadosAverbacao(unitOfWork);
            ServicoSGT.CTe.CTeClient svcCTe = ObterClientCTe(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);

            IList<Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaCTeAnteriorPendenteIntegracao> ctesAnterioPentente = repCarga.BuscarCTeAnteriorPendenteIntegracao();

            for (int i = 0; i < ctesAnterioPentente.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(ctesAnterioPentente[i].CodigoCargaPedido);

                ServicoSGT.CTe.RetornoOfCTe_S2VakUsz retorno = svcCTe.BuscarCTePorChave(ctesAnterioPentente[i].Chave);
                if (!retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar CT-e pela chave: {retorno.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno.Mensagem;
                    return false;
                }
                else if (retorno.Objeto == null)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar CT-e pela chave: objeto não localizado", "ImportarCadastro");

                    mensagemErro = "Objeto não localizado";
                    return false;
                }
                else
                {
                    ServicoSGT.CTe.RetornoOfArrayOfDadosAverbacao_S2VakUsz retornoAverbacao = null;
                    try
                    {
                        retornoAverbacao = svcCTe.BuscarDadosAverbacao(ctesAnterioPentente[i].Chave);
                        if (!retornoAverbacao.Status)
                        {
                            Servicos.Log.TratarErro($"Falha ao buscar dados da averbação CT-e pela chave: {retornoAverbacao.Mensagem}", "ImportarCadastro");
                        }
                        else if (retornoAverbacao.Objeto == null)
                        {
                            Servicos.Log.TratarErro($"Falha ao buscar dados da averbação CT-e pela chave: objeto não localizado", "ImportarCadastro");
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "ImportarCadastro");
                    }

                    try
                    {
                        unitOfWork.Start();
                        serCargaCteParaSubContratacao.VincularCTeTerceiroACargaPedido(retorno.Objeto, cargaPedido, unitOfWork, tipoServicoMultisoftware);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, $"Inseriu CT-e anterior {ctesAnterioPentente[i].Chave} pelo serviço de integração.", unitOfWork);

                        if (retornoAverbacao != null && retornoAverbacao.Status && retornoAverbacao.Objeto != null && retornoAverbacao.Objeto.Length > 0)
                        {
                            foreach (var dadoAverbacao in retornoAverbacao.Objeto)
                            {
                                if (serWSDadosAverbacao.SalvarDadosAverbacaoCarga(dadoAverbacao, cargaPedido, unitOfWork))
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, $"Inseriu a averbação {dadoAverbacao.Protocolo} pelo serviço busca da integração.", unitOfWork);
                                else
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, $"Não foi possível inserir a averbação na carga pois não está cadastrado corretamente as apólices.", unitOfWork);
                            }
                        }
                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "ImportarCadastro");
                        unitOfWork.Rollback();
                    }
                    //if (!repCarga.ConterCTeAnteriorPendenteIntegracao(ctesAnterioPentente[i].CodigoCargaPedido))
                    //{
                    //    cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                    //    cargaPedido.Carga.NaoGerarMDFe = true;
                    //    cargaPedido.Carga.NaoExigeVeiculoParaEmissao = true;
                    //    cargaPedido.Carga.ProcessandoDocumentosFiscais = true;
                    //    cargaPedido.Carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                    //    cargaPedido.Carga.DataConfirmacaoDocumentosFiscais = DateTime.Now;

                    //    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, $"Avançou a carga devido ter recebido todos os CT-es anteriores via integração.", unitOfWork);

                    //    repCarga.Atualizar(cargaPedido.Carga);
                    //}

                    unitOfWork.FlushAndClear();
                }

            }

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ConsultarCTeParaComplementoOSMae(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Embarcador.CTe.CTe servicoCte = new Embarcador.CTe.CTe(unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaCTeParaComplemento> ctesParaComplemento = repCarga.BuscarCTeParaComplemento();

            for (int i = 0; i < ctesParaComplemento.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(ctesParaComplemento[i].CodigoCargaPedido);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(ctesParaComplemento[i].Chave);

                if (cte == null || cargaPedido == null)
                {
                    Servicos.Log.TratarErro($"Não foi localizado o ct-e para complemento", "ImportarCadastro");

                    mensagemErro = "Objeto não localizado";
                    return false;
                }
                else
                {
                    try
                    {
                        if (!repPedidoCTeParaSubContratacao.ContemPorCargaPedidoEChave(ctesParaComplemento[i].CodigoCargaPedido, ctesParaComplemento[i].Chave))
                        {
                            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = servicoCte.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, unitOfWork);

                            unitOfWork.Start();
                            serCargaCteParaSubContratacao.VincularCTeTerceiroACargaPedido(cteIntegracao, cargaPedido, unitOfWork, tipoServicoMultisoftware);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, $"Inseriu CT-e para o complemento {ctesParaComplemento[i].Chave} pelo serviço de integração.", unitOfWork);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "ImportarCadastro");
                        unitOfWork.Rollback();
                    }

                    if (!repCarga.ContemCTeParaComplemento(ctesParaComplemento[i].CodigoCargaPedido))
                    {
                        cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                        cargaPedido.Carga.NaoGerarMDFe = true;
                        if (!cargaPedido.Carga.CargaDestinadaCTeComplementar)
                            cargaPedido.Carga.NaoExigeVeiculoParaEmissao = true;

                        if (!cargaPedido.Carga.CargaRetornadaEtapaNFeManualmente)
                        {
                            cargaPedido.Carga.ProcessandoDocumentosFiscais = true;
                            cargaPedido.Carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                            cargaPedido.Carga.DataConfirmacaoDocumentosFiscais = DateTime.Now;

                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, $"Avançou a carga devido ter recebido todos os CT-es anteriores via integração.", unitOfWork);
                        }

                        repCarga.Atualizar(cargaPedido.Carga);
                    }

                    unitOfWork.FlushAndClear();
                }

            }

            mensagemErro = string.Empty;
            return true;
        }

        public static void IntegrarDocumentoAnterior(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem = "")
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            ServicoSGT.CTe.CTeClient svcCTe = ObterClientCTe(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);
            Servicos.WebService.Carga.DadosAverbacao serWSDadosAverbacao = new Servicos.WebService.Carga.DadosAverbacao(unitOfWork);

            InspectorBehavior inspectorCTe = new InspectorBehavior();
            svcCTe.Endpoint.EndpointBehaviors.Add(inspectorCTe);

            string mensagemErro = "";
            bool situacaoIntegracao = false;

            bool possuiIntegracaoCTeAnterior = repCargaPedido.PossuiIntegracaoCTeAnterior(cargaCargaIntegracao.Carga.Codigo);
            bool possuiIntegracaoSeguroAnterior = repCargaPedido.PossuiIntegracaoSeguroAnterior(cargaCargaIntegracao.Carga.Codigo);
            string numeroBooking = repCargaPedido.BuscarNumeroBooking(cargaCargaIntegracao.Carga.Codigo);
            string numeroOS = repCargaPedido.BuscarNumeroOS(cargaCargaIntegracao.Carga.Codigo);

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarCTePorCarga(cargaCargaIntegracao.Carga.Codigo);
                foreach (var cte in ctes)
                {
                    if (possuiIntegracaoCTeAnterior)
                    {
                        ServicoSGT.CTe.RetornoOfboolean retorno = svcCTe.EnviarCTeAnterior(serCTe.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, unitOfWork), cargaCargaIntegracao.Carga.CodigoCargaEmbarcador, numeroBooking, numeroOS);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = "Envio do CT-e " + cte.Chave + " com sucesso.";
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorCTe.LastRequestXML, "xml", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorCTe.LastResponseXML, "xml", unitOfWork);

                        if (!retorno.Status)
                        {
                            Servicos.Log.TratarErro($"Falha ao enviar CT-e anterior: {retorno.Mensagem}", "IntegrarDocumentoAnterior");

                            mensagemErro = retorno.Mensagem;
                            situacaoIntegracao = false;

                            arquivoIntegracao.Mensagem = mensagemErro.Length > 400 ? mensagemErro.Substring(0, 400) : mensagemErro;
                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                            cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                            cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                            repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                            return;
                        }

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                        cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                    }
                    if (possuiIntegracaoSeguroAnterior)
                    {
                        List<Dominio.Entidades.AverbacaoCTe> averbacoes = repAverbacaoCTe.BuscarPorCTe(cte.Codigo);
                        foreach (var averbacao in averbacoes)
                        {
                            ServicoSGT.CTe.RetornoOfboolean retorno = svcCTe.EnviarDadosAverbacao(serWSDadosAverbacao.ConverterDadosAverbacaoCTe(averbacao, enviarCTeApenasParaTomador, unitOfWork), cargaCargaIntegracao.Carga.CodigoCargaEmbarcador, numeroBooking, numeroOS);

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = "Envio da averbação " + averbacao.Protocolo + " com sucesso.";
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorCTe.LastRequestXML, "xml", unitOfWork);
                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorCTe.LastResponseXML, "xml", unitOfWork);

                            if (!retorno.Status)
                            {
                                Servicos.Log.TratarErro($"Falha ao enviar averbação anterior: {retorno.Mensagem}", "IntegrarDocumentoAnterior");

                                mensagemErro = retorno.Mensagem;
                                situacaoIntegracao = false;

                                arquivoIntegracao.Mensagem = mensagemErro.Length > 400 ? mensagemErro.Substring(0, 400) : mensagemErro;
                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                                return;
                            }

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                            cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                        }
                    }
                }

                situacaoIntegracao = true;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegrarDocumentoAnterior");
                Servicos.Log.TratarErro("Request: " + inspectorCTe.LastRequestXML, "IntegrarDocumentoAnterior");
                Servicos.Log.TratarErro("Response: " + inspectorCTe.LastResponseXML, "IntegrarDocumentoAnterior");

                mensagemErro = "Ocorreu uma falha ao comunicar com o serviço para integração de documento anterior.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorCTe.LastRequestXML, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorCTe.LastResponseXML, "xml", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
            else
            {
                cargaCargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }

        }
        #endregion

        #region Métodos Privados

        private static ServicoSGT.CTe.CTeClient ObterClientCTe(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "CTe.svc";

            ServicoSGT.CTe.CTeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        #endregion
    }
}
