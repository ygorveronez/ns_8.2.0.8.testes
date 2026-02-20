using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Servicos.Embarcador.PagamentoMotorista
{
    public class PamCard : ServicoBase
    {        
        public PamCard(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public void EmitirPagamentoMotorista(int codigoPagamento, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            // Repositorios
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotorista.BuscarPorCodigo(codigoPagamento);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(codigoPagamento);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
           
            ServicoPamCard.WSTransacionalClient svcPamCard = this.ObterClientPamCard(configuracaoIntegracao, pagamento, unidadeDeTrabalho, out Servicos.Models.Integracao.InspectorBehavior inspector);

            // Gera campos para integracao
            if (pagamento.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > 0)
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
                List<ServicoPamCard.fieldTO> campos = this.GeraCamposPamCard(pagamento, configuracaoIntegracao, unidadeDeTrabalho);

                // Executa requisicao
                ServicoPamCard.execute execute = new ServicoPamCard.execute()
                {
                    arg0 = new ServicoPamCard.requestTO()
                    {
                        context = "InsertTrip",//context = "InsertFreightContract",
                        fields = campos.ToArray()
                    }
                };

                ServicoPamCard.executeResponse retorno = new ServicoPamCard.executeResponse();

                try
                {
                    retorno = svcPamCard.execute(execute);
                }
                catch (Exception ex)
                {
                    // Log de erro
                    Servicos.Log.TratarErro("Erro PANCARD" + ex.Message);

                    pagamentoEnvio.Retorno = Utilidades.String.Left(ex.Message, 300);
                    pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoEnvio.Data = DateTime.Now;

                    pagamento.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao;

                    repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
                    repPagamentoMotorista.Atualizar(pagamento);

                    return;
                }

                pagamentoEnvio.ArquivoEnvio = inspector.LastRequestXML;
                pagamentoEnvio.Data = DateTime.Now;
                pagamentoEnvio.TipoIntegracaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.PamCard;

                // Log de sucesso                

                pagamentoRetorno.CodigoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.codigo") select obj.value).FirstOrDefault();
                pagamentoRetorno.DescricaoRetorno = (from obj in retorno.@return where obj.key.Equals("mensagem.descricao") select obj.value).FirstOrDefault();
                pagamentoRetorno.Data = DateTime.Now;
                pagamentoRetorno.PagamentoMotoristaTMS = pagamento;
                pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                pagamentoRetorno.ArquivoRetorno = inspector.LastResponseXML;

                pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
                pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);

                pagamentoEnvio.Retorno = pagamentoRetorno.DescricaoRetorno;

                if (pagamentoRetorno.CodigoRetorno == "0")
                {
                    pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else
                {
                    pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pagamento.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao;
                }

                repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
            }
            else
            {
                SalvarPagamentoSemValor(ref pagamentoEnvio, ref pagamento);
            }

            repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            repPagamentoMotorista.Atualizar(pagamento);
            
        }

        public void EmitirPagamentoMotoristaPamcardCorporativo(int codigoPagamento, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotorista.BuscarPorCodigo(codigoPagamento);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(codigoPagamento);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            

            // Gera campos para integracao
            if (pagamento.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > 0)
            {
                string codigoRetorno = string.Empty;
                string mensagemRetorno;
                bool sucesso;
                string jsonRequisicao = string.Empty;
                string jsonRetorno = string.Empty;

                try
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
                    if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLPamcardCorporativoAutenticacao) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLPamcardCorporativo))
                        throw new ServicoException("Não foi configurado as URLs de integração com a Pamcard Corporativo.");

                    string accessToken = "", idToken = "";
                    AutenticacaoPamcardCorporativo(configuracaoIntegracao, pagamento, ref accessToken, ref idToken);

                    if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(idToken))
                        throw new ServicoException("Não retornou os tokens de integração.");

                    //Busca os cartões existentes para o motorista
                    string url = configuracaoIntegracao.URLPamcardCorporativo;
                    string urlBuscaCartoes = url + "/cartao/api/cartoes?cpf=" + pagamento.Motorista.CPF;

                    HttpClient requisicao = CriarRequisicaoPamcardCorporativo(urlBuscaCartoes, accessToken, idToken);
                    HttpResponseMessage retornoRequisicao = requisicao.GetAsync(urlBuscaCartoes).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.RetornoBuscaCartao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.RetornoBuscaCartao>(jsonRetorno);
                        if (retorno.Content.Count > 0)//Quando tiver cartões, faz a movimentação
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.RetornoBuscaCartaoContent cartao = retorno.Content[0];

                            string urlMovimentacaoCartao = url + "/movimentacao-cartao/api/movimentacoes";
                            requisicao = CriarRequisicaoPamcardCorporativo(urlMovimentacaoCartao, accessToken, idToken);

                            Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.MovimentacaoCartao movimentacaoCartao = ObterDadosMovimentacaoCartao(pagamento, cartao);
                            jsonRequisicao = JsonConvert.SerializeObject(movimentacaoCartao, Formatting.Indented);
                            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                            retornoRequisicao = requisicao.PostAsync(urlMovimentacaoCartao, conteudoRequisicao).Result;
                            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                            if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                            {
                                Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.RetornoMovimentacaoCartao retornoMovimentacaoCartao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.RetornoMovimentacaoCartao>(jsonRetorno);

                                codigoRetorno = retornoMovimentacaoCartao.Conteudo.Bilhete.IdBilhete;
                                mensagemRetorno = retornoMovimentacaoCartao.Conteudo.Descricao;
                                sucesso = true;
                            }
                            else
                                throw new ServicoException($"Retorno Movimentação do cartão: { (int)retornoRequisicao.StatusCode }");
                        }
                        else
                            throw new ServicoException("Nenhum cartão encontrado para o motorista no sistema da Pamcard Corporativo");
                    }
                    else
                        throw new ServicoException($"Retorno Busca Cartões: { (int)retornoRequisicao.StatusCode }");
                    
                    pagamentoRetorno.CodigoRetorno = codigoRetorno;
                    pagamentoRetorno.DescricaoRetorno = mensagemRetorno;
                    pagamentoRetorno.Data = DateTime.Now;
                    pagamentoRetorno.PagamentoMotoristaTMS = pagamento;
                    pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                    pagamentoRetorno.ArquivoRetorno = jsonRetorno;

                    pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unidadeDeTrabalho);
                    pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unidadeDeTrabalho);
                    repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
                }
                catch (ServicoException ex)
                {
                    mensagemRetorno = ex.Message;
                    sucesso = false;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Erro PANCARD" + ex.Message);
                    mensagemRetorno = Utilidades.String.Left(ex.Message, 300);
                    sucesso = false;
                }

                pagamentoEnvio.Retorno = mensagemRetorno;
                pagamentoEnvio.Data = DateTime.Now;
                pagamentoEnvio.ArquivoEnvio = jsonRequisicao;
                pagamentoEnvio.TipoIntegracaoPagamentoMotorista = TipoIntegracaoPagamentoMotorista.PamCardCorporativo;

                if (!sucesso)
                {
                    pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    //pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;

                    /*
                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        string msgRetorno = "";
                        AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamento.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, auditado, pagamento.Usuario, unidadeDeTrabalho, unidadeDeTrabalho.StringConexao, tipoServicoMultisoftware);
                    }
                    */
                }
            }
            else
            {
                SalvarPagamentoSemValor(ref pagamentoEnvio, ref pagamento);
            }

            repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            repPagamentoMotorista.Atualizar(pagamento);
           
        }

        #endregion

        #region Métodos Privados

        private void SalvarPagamentoSemValor(ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            pagamentoEnvio.ArquivoEnvio = "";
            pagamentoEnvio.Data = DateTime.Now;
            pagamentoEnvio.TipoIntegracaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.PamCard;
            pagamentoEnvio.Retorno = "Não foi enviado para Pamcard devido saldo do motorista";
            pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
        }

        private ServicoPamCard.WSTransacionalClient ObterClientPamCard(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Repositorio.UnitOfWork unitOfWork, out Servicos.Models.Integracao.InspectorBehavior inspectorBehavior)
        {
            ServicoPamCard.WSTransacionalClient svcPamCard = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoPamCard.WSTransacionalClient, ServicoPamCard.WSTransacional>(TipoWebServiceIntegracao.Pamcard_WSTransacional, out inspectorBehavior);

            Dominio.Entidades.Empresa empresa = ObterEmpresa(configuracaoIntegracao, pagamento);

            svcPamCard.ClientCredentials.ClientCertificate.Certificate = ObterCertificadoDigital(empresa);

            return svcPamCard;
        }

        private List<ServicoPamCard.fieldTO> ObterFavorecidos(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeDeTrabalho);
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido.qtde", value = "1" });
            if (pagamento.Terceiro != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadora = repModalidadeTerceiro.BuscarPorPessoa(pagamento.Terceiro.CPF_CNPJ);
                if (modalidadeTransportadora != null && modalidadeTransportadora.TipoFavorecidoCIOT == TipoFavorecidoCIOT.Transportador)
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido.cartao.numero", value = modalidadeTransportadora.NumeroCartao });
                else
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido.cartao.numero", value = pagamento.Motorista.NumeroCartao });
            }
            else
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido.cartao.numero", value = pagamento.Motorista.NumeroCartao });

            campos.AddRange(this.ObterMotorista(pagamento, unidadeDeTrabalho));

            return campos;
        }

        private List<ServicoPamCard.fieldTO> ObterMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeDeTrabalho);
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.tipo", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento.qtde", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento1.tipo", value = "2" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento1.numero", value = Utilidades.String.OnlyNumbers(pagamento.Motorista.CPF) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.tipo", value = "3" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.numero", value = Utilidades.String.OnlyNumbers(pagamento.Motorista.RG) });

            if (pagamento.Motorista.Localidade != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.uf", value = pagamento.Motorista.Localidade.Estado.Sigla });

            if (pagamento.Motorista.OrgaoEmissorRG != null)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.documento2.emissor.id", value = pagamento.Motorista.OrgaoEmissorRG.Value.ToString("d") });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.nome", value = Utilidades.String.Left(pagamento.Motorista.Nome, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.nacionalidade.id", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.naturalidade.ibge", value = string.Format("{0:0000000}", pagamento.Motorista.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.sexo", value = "M" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.logradouro", value = Utilidades.String.Left(pagamento.Motorista.Endereco, 40) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.numero", value = "0" });

            if (!string.IsNullOrWhiteSpace(pagamento.Motorista.Complemento))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.complemento", value = Utilidades.String.Left(pagamento.Motorista.Complemento, 15) });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.bairro", value = Utilidades.String.Left(pagamento.Motorista.Bairro, 30) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.cidade.ibge", value = string.Format("{0:0000000}", pagamento.Motorista.Localidade?.CodigoIBGE ?? 0) });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.endereco.cep", value = Utilidades.String.OnlyNumbers(pagamento.Motorista.CEP) });

            if (!string.IsNullOrWhiteSpace(pagamento.Motorista.Telefone) && pagamento.Motorista.Telefone.IndexOf(' ') > 0)
            {
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.telefone.ddd", value = "0" + pagamento.Motorista.Telefone?.Split(' ')[0]?.Replace("(", "").Replace(")", "") });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.telefone.numero", value = pagamento.Motorista.Telefone?.Split(' ')[1]?.Replace("-", "") });
            }

            if (!string.IsNullOrWhiteSpace(pagamento.Motorista.Email))
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.email", value = pagamento.Motorista.Email });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.meio.pagamento", value = "1" });
            if (pagamento.Terceiro != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadora = repModalidadeTerceiro.BuscarPorPessoa(pagamento.Terceiro.CPF_CNPJ);
                if (modalidadeTransportadora != null && modalidadeTransportadora.TipoFavorecidoCIOT == TipoFavorecidoCIOT.Transportador)
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.cartao.numero", value = modalidadeTransportadora.NumeroCartao });
                else
                    campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.cartao.numero", value = pagamento.Motorista.NumeroCartao });
            }
            else
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.cartao.numero", value = pagamento.Motorista.NumeroCartao });

            if (pagamento.Motorista.DataNascimento.HasValue)
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.favorecido1.data.nascimento", value = pagamento.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") });

            return campos;
        }

        private List<ServicoPamCard.fieldTO> GeraCamposPamCard(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            // Configuracao e Instancias
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Veiculo rpVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Veiculo veiculo = pagamento.Motorista != null ? rpVeiculo.BuscarPorMotorista(pagamento.Motorista.Codigo, "0") : null;

            string tipoDoVeiculo = "4";
            if (pagamento.Carga != null && pagamento.Carga?.Veiculo != null && pagamento.Carga?.Veiculo.TipoDoVeiculo != null && !string.IsNullOrWhiteSpace(pagamento.Carga?.Veiculo.TipoDoVeiculo.CodigoIntegracao))
                tipoDoVeiculo = pagamento.Carga?.Veiculo.TipoDoVeiculo.CodigoIntegracao;
            else if (veiculo != null && veiculo.TipoDoVeiculo != null && !string.IsNullOrWhiteSpace(veiculo.TipoDoVeiculo.CodigoIntegracao))
                tipoDoVeiculo = veiculo.TipoDoVeiculo.CodigoIntegracao;

            string placaVeiculo = "";
            if (pagamento.Carga != null && pagamento.Carga.Veiculo != null)
                placaVeiculo = pagamento.Carga?.Veiculo?.Placa;
            else if (veiculo != null)
                placaVeiculo = veiculo.Placa;

            // Campos para geracao da requisição
            List<ServicoPamCard.fieldTO> campos = new List<ServicoPamCard.fieldTO>();

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.id.cliente", value = pagamento.Numero.ToString("D") });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contratante.documento.numero", value = configuracaoIntegracao?.EmpresaFixaPamCard?.CNPJ ?? pagamento.Carga?.Empresa.CNPJ });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.contrato.numero", value = pagamento.Numero.ToString("D") });

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.data.partida", value = pagamento.DataPagamento.ToString("dd/MM/yyyy") });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.origem.cidade.ibge", value = string.Format("{0:0000000}", "0") });
            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.destino.cidade.ibge", value = string.Format("{0:0000000}", "0") });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento.qtde", value = "1" });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.numero", value = pagamento.Numero.ToString() });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.documento1.tipo", value = "3" });

            campos.AddRange(ObterFavorecidos(pagamento, unidadeDeTrabalho));

            //campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.carga.natureza", value = pagamento.PagamentoMotoristaTipo.CodigoIntegracao });
            if (pagamento.Valor > 0 && pagamento.DataSaldo.HasValue && pagamento.ValorAdiantamento > 0 && pagamento.DataAdiantamento.HasValue)
            {
                int quantidadeParcelas = 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = pagamento.DataAdiantamento.Value.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = pagamento.PagamentoMotoristaTipo.CodigoIntegracaoEfetivacaoAdiantamento });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".tipo", value = (string.IsNullOrWhiteSpace(pagamento.PagamentoMotoristaTipo.CodigoIntegracaoTipoParcelaAdiantamento) ? "6" : pagamento.PagamentoMotoristaTipo.CodigoIntegracaoTipoParcelaAdiantamento) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = pagamento.ValorAdiantamento.ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.id", value = "1" });

                quantidadeParcelas = 2;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = pagamento.DataSaldo.Value.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = pagamento.PagamentoMotoristaTipo.CodigoIntegracao });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".tipo", value = (string.IsNullOrWhiteSpace(pagamento.PagamentoMotoristaTipo.CodigoIntegracaoTipo) ? "6" : pagamento.PagamentoMotoristaTipo.CodigoIntegracaoTipo) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = (pagamento.Valor - pagamento.ValorAdiantamento).ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.id", value = "1" });
            }
            else
            {
                int quantidadeParcelas = 1;

                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela.qtde", value = quantidadeParcelas.ToString() });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".data", value = pagamento.DataPagamento.ToString("dd/MM/yyyy") });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".efetivacao.tipo", value = pagamento.PagamentoMotoristaTipo.CodigoIntegracao });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".status.id", value = "2" });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".tipo", value = (string.IsNullOrWhiteSpace(pagamento.PagamentoMotoristaTipo.CodigoIntegracaoTipo) ? "6" : pagamento.PagamentoMotoristaTipo.CodigoIntegracaoTipo) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".valor", value = pagamento.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista).ToString("0.00", cultura) });
                campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.parcela" + quantidadeParcelas.ToString() + ".favorecido.id", value = "1" });
            }

            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.categoria", value = tipoDoVeiculo });
            campos.Add(new ServicoPamCard.fieldTO() { key = "viagem.veiculo.placa", value = placaVeiculo });

            return campos;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.MovimentacaoCartao ObterDadosMovimentacaoCartao(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.RetornoBuscaCartaoContent cartao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.MovimentacaoCartao movimentacaoCartao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.MovimentacaoCartao()
            {
                Descricao = Utilidades.String.Left(pagamento.Observacao, 40),
                Valor = pagamento.Valor,
                Tipo = pagamento.PagamentoMotoristaTipo.DescricaoFormaPagamentoMotorista.ToUpper(),
                Cartao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.MovimentacaoCartaoDetalhe()
                {
                    IdCartao = cartao.IdCartao.ToString(),
                    IdContaCartao = cartao.IdContaCartao,
                    IdImpresso = cartao.IdImpressoCartao.ToString(),
                    Numero = cartao.PanFormatado,
                    Proprietario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.MovimentacaoCartaoDetalheProprietario()
                    {
                        Documento = cartao.Proprietario.Cnpj
                    },
                    TipoCartao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard.MovimentacaoCartaoDetalheTipoCartao()
                    {
                        Emissor = cartao.Emissor,
                        Modalidade = cartao.Modalidade
                    }
                }
            };

            if (pagamento.PagamentoMotoristaTipo.FormaPagamentoMotorista == FormaPagamentoMotorista.Carga)
            {
                movimentacaoCartao.TipoEfetivacao = pagamento.Carga != null ? "AUTOMATICA" : "MANUAL";
                movimentacaoCartao.DataAgendamento = pagamento.DataPagamento.ToString("yyyy-MM-dd");
            }
            else if (pagamento.PagamentoMotoristaTipo.FormaPagamentoMotorista == FormaPagamentoMotorista.Descarga)
            {
                movimentacaoCartao.TipoEfetivacao = null;
                movimentacaoCartao.DataAgendamento = null;
            }
            else
                throw new ServicoException("Forma de pagamento do Pamcard Corporativo não foi definido no cadastro");

            return movimentacaoCartao;
        }

        private Dominio.Entidades.Empresa ObterEmpresa(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            if (configuracaoIntegracao != null && configuracaoIntegracao.EmpresaFixaPamCard != null)
                return configuracaoIntegracao?.EmpresaFixaPamCard;
            else
                return pagamento.Carga?.Empresa;
        }

        private X509Certificate2 ObterCertificadoDigital(Dominio.Entidades.Empresa empresa)
        {
            return new X509Certificate2(empresa?.NomeCertificado, empresa?.SenhaCertificado, X509KeyStorageFlags.MachineKeySet);
        }

        private HttpClient CriarRequisicaoPamcardCorporativo(string url, string accessToken, string idToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(PamCard));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            requisicao.DefaultRequestHeaders.Add("idToken", idToken);

            return requisicao;
        }

        private HttpClient CriarRequisicaoAutenticacaoPamcardCorporativo(string url, Dominio.Entidades.Empresa empresa)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClientHandler handler = new HttpClientHandler();
            handler.ClientCertificates.Add(ObterCertificadoDigital(empresa));
            handler.PreAuthenticate = true;
            HttpClient requisicao = new HttpClient(handler);

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return requisicao;
        }

        private void AutenticacaoPamcardCorporativo(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, ref string accessToken, ref string idToken)
        {
            string url = configuracaoIntegracao.URLPamcardCorporativoAutenticacao;

            try
            {
                HttpClient requisicao = CriarRequisicaoAutenticacaoPamcardCorporativo(url, ObterEmpresa(configuracaoIntegracao, pagamento));

                StringContent conteudoRequisicao = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                dynamic retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                {
                    accessToken = (string)retorno.accessToken;
                    idToken = (string)retorno.idToken;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        #endregion
    }
}
