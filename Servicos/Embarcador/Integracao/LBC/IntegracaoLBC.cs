using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.LBC
{
    public class IntegracaoLBC
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoLBC(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarContratoFreteTransportador(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao integracaoPendenteContratoFreteTransportador)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repositorioContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(_unitOfWork);

            integracaoPendenteContratoFreteTransportador.NumeroTentativas++;
            integracaoPendenteContratoFreteTransportador.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoContratoFreteTransportador();
                Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete = ObterIntegracaoFrete(integracaoPendenteContratoFreteTransportador.ContratoFreteTransportador.Codigo, TipoIntegracaoFrete.ContratoFreteTransportador);
                Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao dadosRequisicao = ObterRequisicao(integracaoPendenteContratoFreteTransportador, integracaoFrete, configuracaoIntegracao);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebResponse retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao);
                jsonRetorno = ObterJsonRetorno(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);
                    Repositorio.Embarcador.Frete.StatusAssinaturaContrato repositorioStatusAssinaturaContrato = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(_unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracao dadosRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracao>(jsonRetorno);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracaoSituacao situacao = dadosRetorno.Situacoes.FirstOrDefault();

                    if ((situacao.Codigo == (int)HttpStatusCode.OK) || (situacao.Codigo == (int)HttpStatusCode.Created))
                    {
                        Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracaoItem contratoRetorno = situacao.Itens[0];

                        integracaoFrete.CodigoRetornoIntegracao = contratoRetorno.CodigoIntegracao;
                        integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                        integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = situacao.Mensagem;
                        integracaoPendenteContratoFreteTransportador.ContratoFreteTransportador.IDExterno = contratoRetorno.CodigoIntegracao;
                        integracaoPendenteContratoFreteTransportador.ContratoFreteTransportador.StatusAceiteContrato = repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao("D");

                        repositorioIntegracaoFrete.Atualizar(integracaoFrete);
                    }
                    else
                    {
                        integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = situacao.Mensagem;
                        integracaoPendenteContratoFreteTransportador.ContratoFreteTransportador.StatusAceiteContrato = repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao("J");
                    }

                    repositorioContratoTransporteFrete.Atualizar(integracaoPendenteContratoFreteTransportador.ContratoFreteTransportador);
                }
                else
                {
                    integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = "Problema ao integrar contrato";
                    integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(integracaoPendenteContratoFreteTransportador, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = excecao.Message;
                integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (WebException excecao)
            {
                string jsonRetornoExcecao = ObterJsonRetorno(excecao.Response);
                dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonRetornoExcecao);

                if (resposta?.jaggaerResponse != null)
                {
                    var mensagemRetornoExcecao = (resposta.jaggaerResponse.Value is string) ? resposta.jaggaerResponse : resposta.jaggaerResponse.message;

                    integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = mensagemRetornoExcecao ?? "Problema ao tentar integrar.";
                    integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(integracaoPendenteContratoFreteTransportador, jsonRequisicao, jsonRetornoExcecao, "json");
                }
                else
                {
                    integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = "Problema ao tentar integrar.";

                    servicoArquivoTransacao.Adicionar(integracaoPendenteContratoFreteTransportador, jsonRequisicao, jsonRetorno, "json");
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoPendenteContratoFreteTransportador.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendenteContratoFreteTransportador.ProblemaIntegracao = "Problema ao tentar integrar.";

                servicoArquivoTransacao.Adicionar(integracaoPendenteContratoFreteTransportador, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioContratoTransporteFreteIntegracao.Atualizar(integracaoPendenteContratoFreteTransportador);
        }

        public void IntegrarContratoTransporteFrete(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao contratoFreteTransportadorIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repositorioContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(_unitOfWork);

            contratoFreteTransportadorIntegracao.NumeroTentativas++;
            contratoFreteTransportadorIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoContratoTransporteFrete();
                Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicao dadosRequisicao = ObterRequisicao(contratoFreteTransportadorIntegracao);
                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebResponse retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao);
                jsonRetorno = ObterJsonRetorno(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    Repositorio.Embarcador.Frete.ContratoTransporteFrete repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(_unitOfWork);
                    Repositorio.Embarcador.Frete.StatusAssinaturaContrato repositorioStatusAssinaturaContrato = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(_unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracao dadosRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracao>(jsonRetorno);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracaoSituacao situacao = dadosRetorno.Situacoes.FirstOrDefault();

                    if ((situacao.Codigo == (int)HttpStatusCode.OK) || (situacao.Codigo == (int)HttpStatusCode.Created))
                    {
                        contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        contratoFreteTransportadorIntegracao.ProblemaIntegracao = situacao.Itens[0].Mensagem;
                        int.TryParse(situacao.Itens[0].CodigoIntegracao, out int codigoContrato);
                        contratoFreteTransportadorIntegracao.ContratoTransporteFrete.StatusAssinaturaContrato = contratoFreteTransportadorIntegracao.ContratoTransporteFrete.ContratoExternoID == 0 ? repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao("D") : repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao("R");
                        contratoFreteTransportadorIntegracao.ContratoTransporteFrete.ContratoExternoID = codigoContrato > 0 ? codigoContrato : contratoFreteTransportadorIntegracao.ContratoTransporteFrete.ContratoExternoID;
                        contratoFreteTransportadorIntegracao.ContratoTransporteFrete.UltimaData = contratoFreteTransportadorIntegracao.ContratoTransporteFrete.DataFim;
                        contratoFreteTransportadorIntegracao.ContratoTransporteFrete.UltimoValorPrevistoContrato = contratoFreteTransportadorIntegracao.ContratoTransporteFrete.ValorPrevistoContrato;
                        GerarIntegracaoAnexosContrato(contratoFreteTransportadorIntegracao);
                    }
                    else
                    {
                        contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        contratoFreteTransportadorIntegracao.ProblemaIntegracao = situacao.Itens[0].Mensagem;
                    }

                    repositorioContratoTransporteFrete.Atualizar(contratoFreteTransportadorIntegracao.ContratoTransporteFrete);
                }
                else
                {
                    contratoFreteTransportadorIntegracao.ProblemaIntegracao = "Problema ao integrar contrato";
                    contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(contratoFreteTransportadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                contratoFreteTransportadorIntegracao.ProblemaIntegracao = excecao.Message;
                contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(contratoFreteTransportadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (WebException excecao)
            {
                string jsonRetornoExcecao = ObterJsonRetorno(excecao.Response);
                dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonRetornoExcecao);

                if (resposta?.jaggaerResponse != null)
                {
                    var mensagemRetornoExcecao = (resposta.jaggaerResponse.Value is string) ? resposta.jaggaerResponse : resposta.jaggaerResponse.message;

                    contratoFreteTransportadorIntegracao.ProblemaIntegracao = mensagemRetornoExcecao ?? "Problema ao tentar integrar.";
                    contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(contratoFreteTransportadorIntegracao, jsonRequisicao, jsonRetornoExcecao, "json");
                }
                else
                {
                    contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    contratoFreteTransportadorIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";

                    servicoArquivoTransacao.Adicionar(contratoFreteTransportadorIntegracao, jsonRequisicao, jsonRetorno, "json");
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                contratoFreteTransportadorIntegracao.ProblemaIntegracao = excecao.Message;

                servicoArquivoTransacao.Adicionar(contratoFreteTransportadorIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioContratoTransporteFreteIntegracao.Atualizar(contratoFreteTransportadorIntegracao);
        }

        public void IntegrarContratoTransporteFreteAnexos(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao contratoFreteTransportadorIntegracao)
        {
            Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repostiorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(_unitOfWork);

            contratoFreteTransportadorIntegracao.NumeroTentativas++;
            contratoFreteTransportadorIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoContratoTransporteFreteAnexo();
                List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo> anexosNaoIntegrados = contratoFreteTransportadorIntegracao.ContratoTransporteFrete.Anexos.Where(a => !a.AnexoIntegrado).ToList();

                foreach (Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo anexoAIntegrar in anexosNaoIntegrados)
                    IntegrarContratoTransporteFreteAnexo(anexoAIntegrar, contratoFreteTransportadorIntegracao, configuracaoIntegracao);

                contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                contratoFreteTransportadorIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                contratoFreteTransportadorIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                contratoFreteTransportadorIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
            }

            repostiorioContratoFreteTransportador.Atualizar(contratoFreteTransportadorIntegracao);
        }

        public void IntegrarTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao tabelaFreteClienteIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioValores = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);
            Servicos.Embarcador.Frete.AjusteTabelaFrete servicoAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensEnviar = new List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();
            bool permitirAlterarSituacaoItensComProblemaIntegracao = true;

            tabelaFreteClienteIntegracao.DataIntegracao = DateTime.Now;
            tabelaFreteClienteIntegracao.NumeroTentativas += 1;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                if (tabelaFreteClienteIntegracao.TabelaFreteCliente.TabelaFrete.ParametroBase != TipoParametroBaseTabelaFrete.ModeloReboque)
                    throw new ServicoException("Tabela de frete sem parâmetro base por modelo de reboque", CodigoExcecao.NenhumRegistroEncontrado);

                Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoTabelaFreteCliente();
                List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens = repositorioValores.BuscarComValorInformadoPorParametrosTabelaFrete(tabelaFreteClienteIntegracao.TabelaFreteCliente.Codigo);
                List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesFrete = ObterIntegracoesFrete(itens);
                Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao dadosRequisicao = ObterRequisicao(tabelaFreteClienteIntegracao, itens, integracoesFrete, configuracaoIntegracao);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem itemIntegrar in dadosRequisicao.Itens)
                {
                    Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete = integracoesFrete.Where(Integracao => Integracao.Codigo == itemIntegrar.Codigo).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = itens.Where(o => o.Codigo == integracaoFrete.CodigoIntegracao).FirstOrDefault();
                    itensEnviar.Add(item);
                }

                jsonRequisicao = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                Log.TratarErro($"Código: {tabelaFreteClienteIntegracao.TabelaFreteCliente.Codigo} | Itens: {string.Join(", ", integracoesFrete.Select(o => o.Codigo))} | Data de Envio: {DateTime.Now}", "IntegracaoLBC");

                WebResponse retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao);
                jsonRetorno = ObterJsonRetorno(retornoRequisicao);

                Log.TratarErro($"Código: {tabelaFreteClienteIntegracao.TabelaFreteCliente.Codigo} | Itens: {string.Join(", ", integracoesFrete.Select(o => o.Codigo))} | Data de Retorno: {DateTime.Now}", "IntegracaoLBC");

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
                    Repositorio.Embarcador.Frete.StatusAssinaturaContrato repositorioStatusAssinaturaContrato = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(_unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracao dadosRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracao>(jsonRetorno);
                    List<string> mensagensErro = new List<string>();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracaoSituacao situacao in dadosRetorno.Situacoes)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.RetornoIntegracaoItem itemRetorno in situacao.Itens)
                        {
                            Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete = integracoesFrete.Where(Integracao => Integracao.Codigo == itemRetorno.Codigo).FirstOrDefault();
                            Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = itens.Where(o => o.Codigo == integracaoFrete.CodigoIntegracao).FirstOrDefault();
                            Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem itemIntegrado = dadosRequisicao.Itens.Where(o => o.Codigo == itemRetorno.Codigo).FirstOrDefault();
                            item.RetornoIntegracao = itemRetorno.Mensagem.Left(400);

                            if ((situacao.Codigo == (int)HttpStatusCode.OK) || (situacao.Codigo == (int)HttpStatusCode.Created))
                            {
                                item.PendenteIntegracao = false;

                                if (itemIntegrado.AguardarRetornoIntegracao)
                                {
                                    item.Situacao = SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoRetornoIntegracao;
                                    item.StatusAceiteValor = repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao("D");
                                }
                                else
                                {
                                    item.Situacao = SituacaoItemParametroBaseCalculoTabelaFrete.Ativo;
                                    item.StatusAceiteValor = repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao("A");
                                }

                                integracaoFrete.CodigoRetornoIntegracao = itemRetorno.CodigoIntegracao;

                                repositorioIntegracaoFrete.Atualizar(integracaoFrete);
                            }
                            else
                            {
                                item.Situacao = SituacaoItemParametroBaseCalculoTabelaFrete.ProblemaIntegracao;
                                item.StatusAceiteValor = repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao("J");

                                mensagensErro.Add($"{itemRetorno.Codigo} - {itemRetorno.Mensagem}");
                            }

                            repositorioValores.Atualizar(item);
                        }
                    }

                    if (mensagensErro.Count > 0)
                    {
                        permitirAlterarSituacaoItensComProblemaIntegracao = false;
                        tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        tabelaFreteClienteIntegracao.ProblemaIntegracao = $"Valores com problema na integração: {string.Join(" | ", mensagensErro)}";
                    }
                    else if (dadosRetorno.Situacoes.Length == 0)
                    {
                        tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        tabelaFreteClienteIntegracao.ProblemaIntegracao = "Integração sinalizou sucesso, mas não retornou dados";
                    }
                    else if (!dadosRequisicao.Itens.Any(o => o.AguardarRetornoIntegracao))
                    {
                        tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        tabelaFreteClienteIntegracao.ProblemaIntegracao = $"Integração realizada com sucesso";

                        servicoAjusteTabelaFrete.FinalizarAlteracoesVigenciaPendente(tabelaFreteClienteIntegracao.TabelaFreteCliente, alteracaoVigenciaAprovada: true);
                    }
                    else
                    {
                        tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                        tabelaFreteClienteIntegracao.ProblemaIntegracao = $"Integração realizada com sucesso e aguardando retorno";
                    }
                }
                else
                {
                    tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    tabelaFreteClienteIntegracao.ProblemaIntegracao = "A integração retornou com falhas";
                }

                servicoArquivoTransacao.Adicionar(tabelaFreteClienteIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                tabelaFreteClienteIntegracao.ProblemaIntegracao = excecao.Message;

                if (excecao.ErrorCode == CodigoExcecao.NenhumRegistroEncontrado)
                {
                    tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                    servicoAjusteTabelaFrete.FinalizarAlteracoesVigenciaPendente(tabelaFreteClienteIntegracao.TabelaFreteCliente, alteracaoVigenciaAprovada: true);
                }
                else
                    tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (WebException excecao)
            {
                string jsonRetornoExcecao = ObterJsonRetorno(excecao.Response);
                dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonRetornoExcecao);

                if (resposta?.jaggaerResponse != null)
                {
                    var mensagemRetornoExcecao = (resposta.jaggaerResponse.Value is string) ? resposta.jaggaerResponse : resposta.jaggaerResponse.message;

                    tabelaFreteClienteIntegracao.ProblemaIntegracao = mensagemRetornoExcecao ?? "Ocorreu uma falha ao realizar a integração";
                    tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    tabelaFreteClienteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração";
                }

                servicoArquivoTransacao.Adicionar(tabelaFreteClienteIntegracao, jsonRequisicao, jsonRetornoExcecao, "json");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tabelaFreteClienteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração";

                servicoArquivoTransacao.Adicionar(tabelaFreteClienteIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            if (tabelaFreteClienteIntegracao.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
            {
                tabelaFreteClienteIntegracao.TabelaFreteCliente.PendenteIntegracao = false;

                repositorioTabelaFreteCliente.Atualizar(tabelaFreteClienteIntegracao.TabelaFreteCliente);
            }
            else if (permitirAlterarSituacaoItensComProblemaIntegracao)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemEnviar in itensEnviar)
                {
                    itemEnviar.Situacao = SituacaoItemParametroBaseCalculoTabelaFrete.ProblemaIntegracao;
                    repositorioValores.Atualizar(itemEnviar);
                }
            }

            repositorioTabelaFreteIntegracao.Atualizar(tabelaFreteClienteIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private WebResponse EnviarRequisicao(Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao, string jsonRequisicao)
        {
            return EnviarRequisicao(configuracaoIntegracao, Encoding.UTF8.GetBytes(jsonRequisicao), contentType: "application/json", queryParams: "?language_code=PT");
        }

        private WebResponse EnviarRequisicao(Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao, byte[] byteArrayDadosRequisicao, string contentType, string queryParams)
        {
            WebRequest requisicao = WebRequest.Create($"{configuracaoIntegracao.Url}{queryParams}");

            requisicao.Method = "POST";
            requisicao.ContentLength = byteArrayDadosRequisicao.Length;
            requisicao.ContentType = contentType;
            requisicao.Headers["client_id"] = configuracaoIntegracao.ClientId;
            requisicao.Headers["client_secret"] = configuracaoIntegracao.ClientSecret;
            requisicao.Timeout = 150000;

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();

            streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            streamDadosRequisicao.Close();

            return requisicao.GetResponse();
        }

        private void GerarIntegracaoAnexosContrato(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao integracaoPendenteContratoFreteTransportador)
        {
            List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo> anexosNaoIntegrados = integracaoPendenteContratoFreteTransportador.ContratoTransporteFrete?.Anexos.Where(a => !a.AnexoIntegrado).ToList();

            if ((anexosNaoIntegrados?.Count ?? 0) == 0)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLBC = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.LBC);

            if (tipoIntegracaoLBC == null)
                return;

            Servicos.Embarcador.Frete.ContratoTransporteFrete servicosContratoTransporteFrete = new Frete.ContratoTransporteFrete(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo anexo in anexosNaoIntegrados)
                servicosContratoTransporteFrete.GerarIntegracaoAnexo(integracaoPendenteContratoFreteTransportador.ContratoTransporteFrete, tipoIntegracaoLBC);
        }

        private void IntegrarContratoTransporteFreteAnexo(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo anexoAIntegrar, Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao contratoFreteTransportadorIntegracao, Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo, Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo, Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>(_unitOfWork);
            Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo, Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo, Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>(_unitOfWork);

            StringBuilder mesagemRetorno = new StringBuilder($"{contratoFreteTransportadorIntegracao.ContratoTransporteFrete.NumeroContrato} - Anexo - ");
            byte[] contrato = servicoAnexo.DownloadAnexo(anexoAIntegrar, _unitOfWork);
            string queryParams = $"?externalContractId={contratoFreteTransportadorIntegracao.ContratoTransporteFrete.NumeroContrato}&language_code=PT&document_name={anexoAIntegrar.NomeArquivo}";
            WebResponse retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, contrato, contentType: "application/octet-stream", queryParams);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = ObterJsonRetorno(retornoRequisicao);

            if (IsRetornoSucesso(retornoRequisicao))
                mesagemRetorno.Append("Integrado com sucesso");
            else
            {
                HttpStatusCode statusRequisicao = ((HttpWebResponse)retornoRequisicao).StatusCode;

                if (statusRequisicao == HttpStatusCode.InternalServerError)
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                    mesagemRetorno.Append(resposta.statusDetails);
                }
                else if (statusRequisicao == HttpStatusCode.Unauthorized)
                    mesagemRetorno.Append("Erro ao tentar integrar arquivo Usuario não autorizado");
                else if (statusRequisicao == HttpStatusCode.NotFound)
                    mesagemRetorno.Append("Erro ao tentar integrar arquivo verifique a url");
            }

            anexoAIntegrar.AnexoIntegrado = true;

            repositorioAnexo.Atualizar(anexoAIntegrar);
            servicoArquivoTransacao.Adicionar(contratoFreteTransportadorIntegracao, jsonRequisicao, jsonRetorno, "json", mesagemRetorno.ToString());
        }

        private bool IsRetornoSucesso(WebResponse retornoRequisicao)
        {
            HttpStatusCode statusRequisicao = ((HttpWebResponse)retornoRequisicao).StatusCode;

            return (statusRequisicao == HttpStatusCode.OK) || (statusRequisicao == HttpStatusCode.Created);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoLBC repositorioIntegracaoLBC = new Repositorio.Embarcador.Configuracoes.IntegracaoLBC(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC configuracaoIntegracaoLBC = repositorioIntegracaoLBC.Buscar();

            if ((configuracaoIntegracaoLBC == null) || !configuracaoIntegracaoLBC.PossuiIntegracaoLBC)
                throw new ServicoException("Não existe configuração de integração disponível para a LBC");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLBC.UsuarioLBC) || string.IsNullOrWhiteSpace(configuracaoIntegracaoLBC.SenhaLBC))
                throw new ServicoException("O usuário e a senha devem estar preenchidos na configuração de integração da LBC");

            return configuracaoIntegracaoLBC;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoContratoFreteTransportador()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC configuracaoIntegracaoLBC = ObterConfiguracaoIntegracao();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLBC.URLIntegracaoLBCCustoFixo))
                throw new ServicoException("A URL não está configurada para a integração com a LBC");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao()
            {
                ClientId = configuracaoIntegracaoLBC.UsuarioLBC,
                ClientSecret = configuracaoIntegracaoLBC.SenhaLBC,
                Url = configuracaoIntegracaoLBC.URLIntegracaoLBCCustoFixo,
                UtilizarValorPadraoParaCamposNulos = configuracaoIntegracaoLBC.UtilizarValorPadraoParaCamposNulos
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoContratoTransporteFrete()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC configuracaoIntegracaoLBC = ObterConfiguracaoIntegracao();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLBC.URLIntegracaoLBC))
                throw new ServicoException("A URL não está configurada para a integração com a LBC");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao()
            {
                ClientId = configuracaoIntegracaoLBC.UsuarioLBC,
                ClientSecret = configuracaoIntegracaoLBC.SenhaLBC,
                Url = configuracaoIntegracaoLBC.URLIntegracaoLBC
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoContratoTransporteFreteAnexo()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC configuracaoIntegracaoLBC = ObterConfiguracaoIntegracao();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLBC.URLIntegracaoLBCAnexo))
                throw new ServicoException("A URL não está configurada para a integração com a LBC");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao()
            {
                ClientId = configuracaoIntegracaoLBC.UsuarioLBC,
                ClientSecret = configuracaoIntegracaoLBC.SenhaLBC,
                Url = configuracaoIntegracaoLBC.URLIntegracaoLBCAnexo
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoTabelaFreteCliente()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC configuracaoIntegracaoLBC = ObterConfiguracaoIntegracao();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoLBC.URLIntegracaoLBCTabelaFreteCliente))
                throw new ServicoException("A URL não está configurada para a integração com a LBC");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao()
            {
                ClientId = configuracaoIntegracaoLBC.UsuarioLBC,
                ClientSecret = configuracaoIntegracaoLBC.SenhaLBC,
                Url = configuracaoIntegracaoLBC.URLIntegracaoLBCTabelaFreteCliente,
                UtilizarValorPadraoParaCamposNulos = configuracaoIntegracaoLBC.UtilizarValorPadraoParaCamposNulos
            };
        }

        private Dominio.Entidades.Embarcador.Frete.IntegracaoFrete ObterIntegracaoFrete(int codigoIntegracao, TipoIntegracaoFrete tipo)
        {
            Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete = repositorioIntegracaoFrete.BuscarPorCodigoIntegracaoETipo(codigoIntegracao, tipo);

            if (integracaoFrete == null)
            {
                integracaoFrete = new Dominio.Entidades.Embarcador.Frete.IntegracaoFrete()
                {
                    CodigoIntegracao = codigoIntegracao,
                    Tipo = tipo
                };

                repositorioIntegracaoFrete.Inserir(integracaoFrete);
            }

            return integracaoFrete;
        }

        private List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> ObterIntegracoesFrete(List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens)
        {
            List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesFrete = new List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete>();

            foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itens)
                integracoesFrete.Add(ObterIntegracaoFrete(item.Codigo, TipoIntegracaoFrete.TabelaFreteCliente));

            return integracoesFrete;
        }

        private string ObterJsonRetorno(WebResponse retornoRequisicao)
        {
            string jsonDadosRetornoRequisicao;

            using (System.IO.Stream streamDadosRetornoRequisicao = retornoRequisicao.GetResponseStream())
            {
                System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                jsonDadosRetornoRequisicao = leitorDadosRetornoRequisicao.ReadToEnd();
                leitorDadosRetornoRequisicao.Close();
            }

            retornoRequisicao.Close();

            return jsonDadosRetornoRequisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao ObterRequisicao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao integracaoPendenteContratoFreteTransportador, Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete, Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repositorioContratoAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(_unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoCarga repositorioContratoTipoCarga = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = integracaoPendenteContratoFreteTransportador.ContratoFreteTransportador;
            Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = contratoFreteTransportador.CanaisEntrega.FirstOrDefault();
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> acordos = repositorioContratoAcordo.BuscarPorContrato(contratoFreteTransportador.Codigo);
            int totalVeiculosContrato = acordos.Sum(acordo => acordo.Quantidade);
            decimal valorPorViagem = (contratoFreteTransportador.QuantidadeMensalCargas > 0) ? Math.Round((contratoFreteTransportador.ValorMensal / contratoFreteTransportador.QuantidadeMensalCargas), decimals: 2) : 0m;
            string codigoIntegracaoTransportador = !string.IsNullOrWhiteSpace(contratoFreteTransportador.ContratoTransporteFrete?.Transportador?.CodigoIntegracao ?? string.Empty) ? (contratoFreteTransportador.ContratoTransporteFrete.Transportador.CodigoIntegracao.StartsWith("T") ? contratoFreteTransportador.ContratoTransporteFrete.Transportador.CodigoIntegracao.Substring(1) : contratoFreteTransportador.ContratoTransporteFrete.Transportador.CodigoIntegracao) : "";
            string codigoIntegracaoDestino = "BR";
            string codigoIntegracaoOrigem = "BR";
            string tipoClienteDestino = "Estado";
            string tipoClienteOrigem = "Estado";
            string valorPadraoCampoTexto = configuracaoIntegracao.UtilizarValorPadraoParaCamposNulos ? "" : null;

            integracaoPendenteContratoFreteTransportador.ModeloVeicular = acordos.Select(acordo => acordo.ModeloVeicular).FirstOrDefault();
            integracaoPendenteContratoFreteTransportador.TipoDeCarga = repositorioContratoTipoCarga.BuscarPrimeiroTipoCargaPorContrato(contratoFreteTransportador.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem itemIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem()
            {
                CodigoCircuito = (canalEntrega?.Circuito ?? false) ? canalEntrega.CodigoIntegracao ?? valorPadraoCampoTexto : valorPadraoCampoTexto,
                CodigoIntegracaoCanalEntrega = (canalEntrega?.Circuito ?? false) ? "Static Circuit" : canalEntrega?.Descricao ?? "",
                CodigoIntegracaoCanalVenda = "",
                CategoriaEquipamento = contratoFreteTransportador.ContratoTransporteFrete?.ModoContrato.ObterDescricaoLBC() ?? string.Empty,
                Codigo = integracaoFrete.Codigo,
                CapacidadeGrupo = (contratoFreteTransportador.CapacidadeOTM ?? false).ObterDescricao(),
                CodigoContratoTransportador = contratoFreteTransportador.ContratoTransporteFrete?.NumeroContrato.ToString() ?? string.Empty,
                CodigoIntegracaoDestino = codigoIntegracaoDestino,
                CodigoIntegracaoOrigem = codigoIntegracaoOrigem,
                CodigoIntegracaoTransportador = string.IsNullOrWhiteSpace(codigoIntegracaoTransportador) ? string.Empty : codigoIntegracaoTransportador.ToLong().ToString(),
                Compromisso = contratoFreteTransportador.PercentualRota > 0m ? contratoFreteTransportador.PercentualRota.ToString("n0") : valorPadraoCampoTexto,
                DataFimVigencia = contratoFreteTransportador.DataFinal,
                DataInicioVigencia = contratoFreteTransportador.DataInicial,
                DominioOTM = contratoFreteTransportador.DominioOTM?.ObterDescricao() ?? string.Empty,
                EstruturaTabela = contratoFreteTransportador.EstruturaTabela?.ObterDescricao() ?? string.Empty,
                FrequenciaContrato = contratoFreteTransportador.PeriodoAcordo.ObterDescricaoLBC(),
                GrupoCarga = contratoFreteTransportador.GrupoCarga?.ObterDescricao() ?? string.Empty,
                ModoContrato = contratoFreteTransportador.ContratoTransporteFrete?.ModoContrato.ObterDescricao() ?? string.Empty,
                MoedaValorPorViagem = "BRL",
                NivelCompromisso = contratoFreteTransportador.PercentualRota > 0m ? "Rota + Equipamento" : valorPadraoCampoTexto,
                NivelServico = "Padrão",
                NomeContratoTransportador = contratoFreteTransportador.ContratoTransporteFrete?.NomeContrato ?? string.Empty,
                Observacao = contratoFreteTransportador.Observacao,
                PaisDestino = contratoFreteTransportador.ContratoTransporteFrete?.Pais?.Nome,
                PaisOrigem = contratoFreteTransportador.ContratoTransporteFrete?.Pais?.Nome,
                PontoPlanejamentoTransporte = contratoFreteTransportador.PontoPlanejamentoTransporte?.ObterDescricao() ?? string.Empty,
                QuantidadeEntregas = contratoFreteTransportador.QuantidadeEntregas.ToString(),
                RazaoSocialTransportador = contratoFreteTransportador.ContratoTransporteFrete?.Transportador?.RazaoSocial ?? "",
                TipoCompromisso = contratoFreteTransportador.PercentualRota > 0m ? "Percentual" : valorPadraoCampoTexto,
                TipoDestino = tipoClienteDestino,
                TipoEquipamento = $"{integracaoPendenteContratoFreteTransportador.ModeloVeicular?.CodigoIntegracao ?? ""}{integracaoPendenteContratoFreteTransportador.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? ""}",
                TipoIntegracao = contratoFreteTransportador.TipoIntegracao?.ObterDescricao() ?? string.Empty,
                TipoOrigem = tipoClienteOrigem,
                TipoTaxa = contratoFreteTransportador.PercentualRota > 0m ? "Cativo" : "Spot",
                TotalVeiculosContrato = totalVeiculosContrato,
                UnidadeTempoCompromisso = contratoFreteTransportador.PercentualRota > 0m ? "Mensalmente" : valorPadraoCampoTexto,
                UOMCompromisso = contratoFreteTransportador.PercentualRota > 0m ? "Volume" : valorPadraoCampoTexto,
                ValorPorViagem = valorPorViagem
            };

            if (configuracaoIntegracao.UtilizarValorPadraoParaCamposNulos)
            {
                itemIntegrar.CodigoIntegracaoCanalVenda = "";
                itemIntegrar.MoedaValor = "";
                itemIntegrar.MoedaValorPorUnidade = "";
                itemIntegrar.TempoMaximoConexao = 0;
                itemIntegrar.TipoValorPorUnidade = "";
                itemIntegrar.Valor = 0m;
                itemIntegrar.ValorAdValorem = 0m;
                itemIntegrar.ValorGerenciamentoRisco = 0m;
                itemIntegrar.ValorPorEntrega = 0m;
                itemIntegrar.ValorPorUnidade = 0m;
                itemIntegrar.ValorPorUnidadeFracionado = 0m;
                itemIntegrar.Status = "";
                itemIntegrar.Messagem = "";
                itemIntegrar.Carregamento = "";
                itemIntegrar.dLoad = "";
                itemIntegrar.PlanoLiquidacao = "";
                itemIntegrar.Semana1 = 0;
                itemIntegrar.Semana2 = 0;
                itemIntegrar.Semana3 = 0;
                itemIntegrar.Semana4 = 0;
                itemIntegrar.EstVol = 0;
                itemIntegrar.UtilizadaPorUltimaVez = "";
                itemIntegrar.AwdPrty = "";
                itemIntegrar.ScacAlias = "";
                itemIntegrar.SapSC = "";
                itemIntegrar.FuelProg = "";
                itemIntegrar.Hazmat = "";
                itemIntegrar.LowAlt = "";
                itemIntegrar.Ttime = 0;
                itemIntegrar.SrvcAvl = new List<string>() { "" };
                itemIntegrar.IncStops = 0;
                itemIntegrar.PickStops = 0;
                itemIntegrar.DelStops = 0;
                itemIntegrar.StopCur = "";
                itemIntegrar.CostPStop = 0;
                itemIntegrar.ReturnPerc = 0;
                itemIntegrar.UnplannedCost = "";
                itemIntegrar.UnplannedCostType = "";
                itemIntegrar.FrFreight = 0;
                itemIntegrar.FrFuel = 0;
                itemIntegrar.FrTempCost = 0;
                itemIntegrar.FrTolls = 0;
                itemIntegrar.PuMinUnits = 0;
                itemIntegrar.PuMaxUnits = 0;
                itemIntegrar.PuTotalCost = 0;
                itemIntegrar.PuBaseCost = "";
                itemIntegrar.PuFixedCost = "";
                itemIntegrar.PuFuelCost = 0;
                itemIntegrar.PuMaxChrg = 0;
                itemIntegrar.ModDate = "";
                itemIntegrar.ModBy = "";
            }

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem> itensIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem>()
            {
                itemIntegrar
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao tabelaFreteClienteIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao()
            {
                Itens = itensIntegrar.ToArray()
            };

            return tabelaFreteClienteIntegrar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicao ObterRequisicao(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao contratoTransporteFreteIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicaoItem itemIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicaoItem()
            {
                IDContratoExterno = contratoTransporteFreteIntegracao.ContratoTransporteFrete.NumeroContrato.ToString(),
                AprovacaoAdicional = contratoTransporteFreteIntegracao.ContratoTransporteFrete.AprovacaoAdicionalRequerida ? "Sim" : "Não",
                NomeContrato = contratoTransporteFreteIntegracao.ContratoTransporteFrete.NomeContrato,
                ProcessoAprovacao = contratoTransporteFreteIntegracao.ContratoTransporteFrete.ProcessoAprovacao.ObterDescricao(),
                Cluster = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Cluster.ObterDescricaoInternacional(),
                Pais = contratoTransporteFreteIntegracao.ContratoTransporteFrete?.Pais?.Nome ?? string.Empty,
                Network = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Network.ObterDescricao(),
                Equipe = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Equipe.ObterDescricao(),
                Categoria = new string[] { contratoTransporteFreteIntegracao.ContratoTransporteFrete.Categoria.ObterDescricao() },
                SubCategoria = new string[] { contratoTransporteFreteIntegracao.ContratoTransporteFrete.SubCategoria.ObterDescricao() },
                ModoContrato = new List<string>() { contratoTransporteFreteIntegracao.ContratoTransporteFrete.ModoContrato.ObterDescricao() },
                RazaoSocial = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Transportador?.RazaoSocial ?? string.Empty,
                ConformidadeComRSP = contratoTransporteFreteIntegracao.ContratoTransporteFrete.ConformidadeComRSP ? "Sim" : "Não",
                CodigoIntegracaoTransportador = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Transportador != null ? (contratoTransporteFreteIntegracao.ContratoTransporteFrete.Transportador.CodigoIntegracao.StartsWith("T") ? contratoTransporteFreteIntegracao.ContratoTransporteFrete.Transportador.CodigoIntegracao.Replace("T", "").TrimStart('0') : contratoTransporteFreteIntegracao.ContratoTransporteFrete.Transportador.CodigoIntegracao.TrimStart('0')) : string.Empty,
                PessoaJuridica = contratoTransporteFreteIntegracao.ContratoTransporteFrete.PessoaJuridica.ObterDescricao() + ".",
                TipoContrato = contratoTransporteFreteIntegracao.ContratoTransporteFrete.TipoContrato.ObterDescricao(),
                HubNonHub = contratoTransporteFreteIntegracao.ContratoTransporteFrete.HubNonHub.ObterDescricao(),
                DominioOTM = contratoTransporteFreteIntegracao.ContratoTransporteFrete.DominioOTM.ObterDescricao(),
                DataInicial = contratoTransporteFreteIntegracao.ContratoTransporteFrete.DataInicio.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DataFinal = contratoTransporteFreteIntegracao.ContratoTransporteFrete.DataFim.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                Moeda = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Moeda.ObterSiglaEstrangeira(),
                ValorContrato = (contratoTransporteFreteIntegracao.ContratoTransporteFrete.ValorPrevistoContrato / 1000000).ToString("F").Replace(",", "."),
                Strategic = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Padrao == Dominio.Enumeradores.OpcaoSimNao.Sim ? "Sim" : "Não",
                TermosPagamento = contratoTransporteFreteIntegracao.ContratoTransporteFrete.TermosPagamento?.Descricao ?? string.Empty,
                ClausulaPenal = contratoTransporteFreteIntegracao.ContratoTransporteFrete.ClausulaPenal ? "Sim" : "Não",
                Observacao = contratoTransporteFreteIntegracao.ContratoTransporteFrete.Observacao,
                UsuarioContrato = contratoTransporteFreteIntegracao.ContratoTransporteFrete.UsuarioContrato?.Nome ?? string.Empty
            };

            if (contratoTransporteFreteIntegracao.ContratoTransporteFrete?.UltimaData != null && contratoTransporteFreteIntegracao.ContratoTransporteFrete.DataFim != contratoTransporteFreteIntegracao.ContratoTransporteFrete.UltimaData.Value)
            {
                itemIntegrar.DataFinalRevisao = contratoTransporteFreteIntegracao.ContratoTransporteFrete.DataFim.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                itemIntegrar.DataFinal = contratoTransporteFreteIntegracao.ContratoTransporteFrete.UltimaData.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }

            if (contratoTransporteFreteIntegracao.ContratoTransporteFrete.UltimoValorPrevistoContrato > 0 && contratoTransporteFreteIntegracao.ContratoTransporteFrete.UltimoValorPrevistoContrato != contratoTransporteFreteIntegracao.ContratoTransporteFrete.ValorPrevistoContrato)
            {
                itemIntegrar.ValorRevisao = (contratoTransporteFreteIntegracao.ContratoTransporteFrete.ValorPrevistoContrato / 1000000).ToString("F").Replace(",", ".");
                itemIntegrar.ValorContrato = (contratoTransporteFreteIntegracao.ContratoTransporteFrete.UltimoValorPrevistoContrato / 1000000).ToString("F").Replace(",", ".");
            }

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicaoItem> itensIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicaoItem>() {
                itemIntegrar
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicao contratoIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ContratoRequisicao()
            {
                Itens = itensIntegrar
            };

            return contratoIntegrar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao ObterRequisicao(Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao tabelaFreteClienteIntegracao, List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens, List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesFrete, Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            List<TipoParametroBaseTabelaFrete> tiposParametroBaseIntegrar = new List<TipoParametroBaseTabelaFrete>() { TipoParametroBaseTabelaFrete.TipoCarga, TipoParametroBaseTabelaFrete.Peso, TipoParametroBaseTabelaFrete.Distancia, TipoParametroBaseTabelaFrete.TipoEmbalagem };
            List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensPorTipoIntegrar = itens.Where(o => tiposParametroBaseIntegrar.Contains(o.TipoObjeto)).ToList();

            if (itensPorTipoIntegrar.Count == 0)
                throw new ServicoException("Não existem valores para serem integrados", CodigoExcecao.NenhumRegistroEncontrado);

            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargaPorTabelaFrete = tabelaFreteClienteIntegracao.TabelaFreteCliente.TabelaFrete.TiposCarga?.ToList() ?? new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            if (!itens.Any(o => o.TipoObjeto == TipoParametroBaseTabelaFrete.TipoCarga) && (tiposCargaPorTabelaFrete.Count == 0))
                throw new ServicoException("Não existem valores definidos por tipo de carga", CodigoExcecao.NenhumRegistroEncontrado);

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repositorioComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.PesoTabelaFrete repositorioPesoTabelaFrete = new Repositorio.Embarcador.Frete.PesoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioTabelaFreteClienteModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repositorioModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(_unitOfWork);

            List<int> codigosTiposCarga = itensPorTipoIntegrar.Where(o => o.TipoObjeto == TipoParametroBaseTabelaFrete.TipoCarga).Select(o => o.CodigoObjeto).Distinct().ToList();
            List<int> codigosModelosVeicularesCarga = itensPorTipoIntegrar.Select(o => o.ParametroBaseCalculo.CodigoObjeto).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesCarga = repositorioModeloVeicularCarga.BuscarPorCodigos(codigosModelosVeicularesCarga);
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargaPorItem = repositorioTipoCarga.BuscarPorCodigos(codigosTiposCarga);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem> itensIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem>();
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = tabelaFreteClienteIntegracao.TabelaFreteCliente;
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteAlteracaoVigenciaPendente = repositorioTabelaFreteCliente.BuscarPorAlteracaoVigenciaPendente(tabelaFreteCliente.Codigo);
            Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia = tabelaFreteClienteAlteracaoVigenciaPendente?.Vigencia ?? tabelaFreteCliente.Vigencia;
            Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componentesFreteAdValorem = repositorioComponenteFreteTabelaFrete.BuscarPorTabelaFrete(tabelaFreteCliente.TabelaFrete.Codigo, TipoComponenteFrete.ADVALOREM);
            Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componentesFreteGris = repositorioComponenteFreteTabelaFrete.BuscarPorTabelaFrete(tabelaFreteCliente.TabelaFrete.Codigo, TipoComponenteFrete.GRIS);
            List<Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete> pesosTabelaFrete = repositorioPesoTabelaFrete.BuscarPorTabelaFrete(tabelaFreteCliente.TabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga> dadosPorModelosVeicularesCarga = repositorioTabelaFreteClienteModeloVeicularCarga.BuscarPorTabelaFreteCliente(tabelaFreteCliente.Codigo);
            Dominio.Entidades.Localidade destino = tabelaFreteCliente.Destinos.FirstOrDefault();
            Dominio.Entidades.Localidade origem = tabelaFreteCliente.Origens.FirstOrDefault();
            Dominio.Entidades.Pais paisDestino = tabelaFreteCliente.PaisesDestino.FirstOrDefault();
            Dominio.Entidades.Pais paisOrigem = tabelaFreteCliente.PaisesOrigem.FirstOrDefault();
            Dominio.Entidades.Estado estadoDestino = tabelaFreteCliente.EstadosDestino.FirstOrDefault();
            Dominio.Entidades.Estado estadoOrigem = tabelaFreteCliente.EstadosOrigem.FirstOrDefault();
            Dominio.Entidades.Cliente clienteDestino = tabelaFreteCliente.ClientesDestino.Count() > 1 ? tabelaFreteCliente.ClientesDestino.Where(x => x.PossuiFilialCliente).FirstOrDefault() : tabelaFreteCliente.ClientesDestino.FirstOrDefault();
            Dominio.Entidades.Cliente clienteOrigem = tabelaFreteCliente.ClientesOrigem.Count() > 1 ? tabelaFreteCliente.ClientesOrigem.Where(x => x.PossuiFilialCliente).FirstOrDefault() : tabelaFreteCliente.ClientesOrigem.FirstOrDefault();
            Dominio.Entidades.Embarcador.Localidades.Regiao regiaoDestino = tabelaFreteCliente.RegioesDestino.FirstOrDefault();
            Dominio.Entidades.Embarcador.Localidades.Regiao regiaoOrigem = tabelaFreteCliente.RegioesOrigem.FirstOrDefault();
            string descricaoPaisDestino = (paisDestino ?? destino?.Pais ?? clienteDestino?.Localidade?.Pais ?? estadoDestino?.Pais ?? regiaoDestino?.LocalidadePolo?.Pais)?.Descricao ?? "BRASIL";
            string descricaoPaisOrigem = (paisOrigem ?? origem?.Pais ?? clienteOrigem?.Localidade?.Pais ?? estadoOrigem?.Pais ?? regiaoOrigem?.LocalidadePolo?.Pais)?.Descricao ?? "BRASIL";
            string codigoIntegracaoDestino = string.Empty;
            string codigoIntegracaoOrigem = string.Empty;
            string tipoClienteDestino = string.Empty;
            string tipoClienteOrigem = string.Empty;
            string valorPadraoCampoTexto = configuracaoIntegracao.UtilizarValorPadraoParaCamposNulos ? "" : null;
            decimal? valorPadraoCampoDecimal = configuracaoIntegracao.UtilizarValorPadraoParaCamposNulos ? (decimal?)0m : null;

            if (clienteDestino != null)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filialDestino = repositorioFilial.BuscarPorCNPJ(clienteDestino.CPF_CNPJ_SemFormato);

                if (filialDestino != null)
                    tipoClienteDestino = "Planta/DC";
                else if (clienteDestino.TipoClienteIntegracaoLBC.HasValue)
                    tipoClienteDestino = (clienteDestino.TipoClienteIntegracaoLBC == TipoClienteIntegracaoLBC.Nenhum) ? "Cliente" : clienteDestino.TipoClienteIntegracaoLBC.Value.ObterDescricao();
                else if (repositorioModalidadePessoas.ExistePorTipo(TipoModalidade.Fornecedor, clienteDestino.CPF_CNPJ))
                    tipoClienteDestino = "Fornecedor";
                else
                    tipoClienteDestino = "Cliente";

                if (filialDestino != null)
                    codigoIntegracaoDestino = filialDestino.CodigoFilialEmbarcador;
                else if (!string.IsNullOrWhiteSpace(clienteDestino.CodigoIntegracao))
                    codigoIntegracaoDestino = clienteDestino.CodigoIntegracao.TrimStart('C').TrimStart('F').TrimStart('0');
                else
                    codigoIntegracaoDestino = clienteDestino.Localidade.CodigoZonaTarifaria;
            }
            else if (destino != null)
            {
                tipoClienteDestino = "Cidade";
                codigoIntegracaoDestino = destino.CodigoZonaTarifaria;
            }
            else if (regiaoDestino != null)
            {
                tipoClienteDestino = "Cidade";
                codigoIntegracaoDestino = regiaoDestino.CodigoIntegracao;
            }
            else if (estadoDestino != null)
            {
                tipoClienteDestino = "Estado";
                codigoIntegracaoDestino = estadoDestino.Sigla;
            }

            if (clienteOrigem != null)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filialOrigem = repositorioFilial.BuscarPorCNPJ(clienteOrigem.CPF_CNPJ_SemFormato);

                if (filialOrigem != null)
                    tipoClienteOrigem = "Planta/DC";
                else if (clienteOrigem.TipoClienteIntegracaoLBC.HasValue)
                    tipoClienteOrigem = (clienteOrigem.TipoClienteIntegracaoLBC == TipoClienteIntegracaoLBC.Nenhum) ? "Cliente" : clienteOrigem.TipoClienteIntegracaoLBC.Value.ObterDescricao();
                else if (repositorioModalidadePessoas.ExistePorTipo(TipoModalidade.Fornecedor, clienteOrigem.CPF_CNPJ))
                    tipoClienteOrigem = "Fornecedor";
                else
                    tipoClienteOrigem = "Cliente";

                if (filialOrigem != null)
                    codigoIntegracaoOrigem = filialOrigem.CodigoFilialEmbarcador;
                else if (!string.IsNullOrWhiteSpace(clienteOrigem.CodigoIntegracao))
                    codigoIntegracaoOrigem = clienteOrigem.CodigoIntegracao.TrimStart('C').TrimStart('F').TrimStart('0');
                else
                    codigoIntegracaoOrigem = clienteOrigem.Localidade.CodigoZonaTarifaria;
            }
            else if (origem != null)
            {
                tipoClienteOrigem = "Cidade";
                codigoIntegracaoOrigem = origem.CodigoZonaTarifaria;
            }
            else if (regiaoOrigem != null)
            {
                tipoClienteOrigem = "Cidade";
                codigoIntegracaoOrigem = regiaoOrigem.CodigoIntegracao;
            }
            else if (estadoOrigem != null)
            {
                tipoClienteOrigem = "Estado";
                codigoIntegracaoOrigem = estadoOrigem.Sigla;
            }

            foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itensPorTipoIntegrar)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = modelosVeicularesCarga.Where(o => o.Codigo == item.ParametroBaseCalculo.CodigoObjeto).FirstOrDefault();
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemAdValorem = (componentesFreteAdValorem != null) ? itens.Where(o => o.TipoObjeto == TipoParametroBaseTabelaFrete.ComponenteFrete && o.ParametroBaseCalculo.Codigo == item.ParametroBaseCalculo.Codigo && o.CodigoObjeto == componentesFreteAdValorem.Codigo).FirstOrDefault() : null;
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemGris = (componentesFreteGris != null) ? itens.Where(o => o.TipoObjeto == TipoParametroBaseTabelaFrete.ComponenteFrete && o.ParametroBaseCalculo.Codigo == item.ParametroBaseCalculo.Codigo && o.CodigoObjeto == componentesFreteGris.Codigo).FirstOrDefault() : null;
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemNumeroEntrega = itens.Where(o => o.TipoObjeto == TipoParametroBaseTabelaFrete.NumeroEntrega && o.ParametroBaseCalculo.Codigo == item.ParametroBaseCalculo.Codigo).FirstOrDefault();
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga dadosPorModeloVeicularCarga = dadosPorModelosVeicularesCarga.Where(o => o.ModeloVeicularCarga.Codigo == modeloVeicularCarga.Codigo).FirstOrDefault();
                bool possuiValoresPendenteIntegracao = (item.PendenteIntegracao || (itemAdValorem?.PendenteIntegracao ?? false) || (itemGris?.PendenteIntegracao ?? false) || (itemNumeroEntrega?.PendenteIntegracao ?? false));
                bool possuiDadosPorModeloVeicularCargaPendenteIntegracao = (dadosPorModeloVeicularCarga?.PendenteIntegracao ?? false);
                bool itemPendenteIntegracao = ((item.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.ProblemaIntegracao) || tabelaFreteCliente.PendenteIntegracao || possuiValoresPendenteIntegracao || possuiDadosPorModeloVeicularCargaPendenteIntegracao);

                if (!itemPendenteIntegracao)
                    continue;

                if ((tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente == SituacaoIntegracaoTabelaFreteCliente.FalhaIntegracao) && (item.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoRetornoIntegracao))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaPorItem = (item.TipoObjeto == TipoParametroBaseTabelaFrete.TipoCarga) ? tiposCargaPorItem.Where(o => o.Codigo == item.CodigoObjeto).FirstOrDefault() : null;
                Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete pesoTabelaFrete = (item.TipoObjeto == TipoParametroBaseTabelaFrete.Peso) ? pesosTabelaFrete.Where(o => o.Codigo == item.CodigoObjeto).FirstOrDefault() : null;
                string codigoIntegracaoTransportador = !string.IsNullOrWhiteSpace(tabelaFreteCliente.Empresa?.CodigoIntegracao ?? string.Empty) ? (tabelaFreteCliente.Empresa.CodigoIntegracao.StartsWith("T") ? tabelaFreteCliente.Empresa.CodigoIntegracao.Substring(1) : tabelaFreteCliente.Empresa.CodigoIntegracao) : "";
                decimal percentualRota = dadosPorModeloVeicularCarga?.PercentualRota ?? 0m;
                decimal quantidadeEntregas = dadosPorModeloVeicularCarga?.QuantidadeEntregas ?? 0m;
                bool capacidadeOTM = dadosPorModeloVeicularCarga?.CapacidadeOTM ?? false;
                Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete = integracoesFrete.Where(Integracao => Integracao.CodigoIntegracao == item.Codigo).FirstOrDefault();
                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargaIntegrar;

                if (tipoCargaPorItem != null)
                    tiposCargaIntegrar = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>() { tipoCargaPorItem };
                else
                    tiposCargaIntegrar = tiposCargaPorTabelaFrete.ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tiposCargaIntegrar)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem itemIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicaoItem()
                    {
                        AguardarRetornoIntegracao = possuiValoresPendenteIntegracao,
                        CategoriaEquipamento = tabelaFreteCliente.ContratoTransporteFrete?.ModoContrato.ObterDescricaoLBC() ?? string.Empty,
                        Codigo = integracaoFrete.Codigo,
                        CapacidadeGrupo = capacidadeOTM.ObterDescricao(),
                        CodigoCircuito = (tabelaFreteCliente.CanalEntrega?.Circuito ?? false) ? tabelaFreteCliente.CanalEntrega.CodigoIntegracao ?? valorPadraoCampoTexto : valorPadraoCampoTexto,
                        CodigoContratoTransportador = tabelaFreteCliente.ContratoTransporteFrete?.NumeroContrato.ToString() ?? string.Empty,
                        CodigoIntegracaoCanalEntrega = (tabelaFreteCliente.CanalEntrega?.Circuito ?? false) ? "Static Circuit" : tabelaFreteCliente.CanalEntrega?.Descricao ?? "",
                        CodigoIntegracaoCanalVenda = tabelaFreteCliente.CanalVenda?.Descricao ?? "",
                        CodigoIntegracaoDestino = codigoIntegracaoDestino,
                        CodigoIntegracaoOrigem = codigoIntegracaoOrigem,
                        CodigoIntegracaoTransportador = string.IsNullOrWhiteSpace(codigoIntegracaoTransportador) ? string.Empty : codigoIntegracaoTransportador.ToLong().ToString(),
                        Compromisso = percentualRota > 0m ? percentualRota.ToString("n0") : valorPadraoCampoTexto,
                        DataFimVigencia = vigencia?.DataFinal,
                        DataInicioVigencia = vigencia?.DataInicial,
                        DominioOTM = tabelaFreteCliente.DominioOTM?.ObterDescricao() ?? string.Empty,
                        EstruturaTabela = tabelaFreteCliente.EstruturaTabela?.ObterDescricao() ?? string.Empty,
                        FrequenciaContrato = null,
                        GrupoCarga = tabelaFreteCliente.TipoGrupoCarga.ObterDescricao(),
                        ModoContrato = tabelaFreteCliente.ContratoTransporteFrete?.ModoContrato.ObterDescricao() ?? string.Empty,
                        NivelCompromisso = percentualRota > 0m ? "Rota + Equipamento" : valorPadraoCampoTexto,
                        NivelServico = "Padrão",
                        NomeContratoTransportador = tabelaFreteCliente.ContratoTransporteFrete?.NomeContrato ?? string.Empty,
                        Observacao = tabelaFreteCliente.ObservacaoInterna,
                        PaisDestino = descricaoPaisDestino,
                        PaisOrigem = descricaoPaisOrigem,
                        PontoPlanejamentoTransporte = tabelaFreteCliente.PontoPlanejamentoTransporte?.ObterDescricao() ?? string.Empty,
                        QuantidadeEntregas = quantidadeEntregas.ToString(),
                        RazaoSocialTransportador = tabelaFreteCliente.Empresa?.RazaoSocial ?? "",
                        TipoCompromisso = percentualRota > 0m ? "Percentual" : valorPadraoCampoTexto,
                        TipoDestino = tipoClienteDestino,
                        TipoEquipamento = $"{modeloVeicularCarga.CodigoIntegracao}{tipoCarga.CodigoTipoCargaEmbarcador}",
                        TipoIntegracao = tabelaFreteCliente.TipoIntegracao?.ObterDescricao() ?? string.Empty,
                        TipoOrigem = tipoClienteOrigem,
                        TipoTaxa = percentualRota > 0m ? "Cativo" : "Spot",
                        TotalVeiculosContrato = null,
                        UnidadeTempoCompromisso = percentualRota > 0m ? "Mensalmente" : valorPadraoCampoTexto,
                        UOMCompromisso = percentualRota > 0m ? "Volume" : valorPadraoCampoTexto,
                        ValorAdValorem = itemAdValorem?.Valor ?? valorPadraoCampoDecimal,
                        ValorGerenciamentoRisco = itemGris?.Valor ?? valorPadraoCampoDecimal,
                        ValorPorEntrega = itemNumeroEntrega?.Valor ?? valorPadraoCampoDecimal
                    };

                    if (configuracaoIntegracao.UtilizarValorPadraoParaCamposNulos)
                    {
                        itemIntegrar.MoedaValor = "";
                        itemIntegrar.MoedaValorPorUnidade = "";
                        itemIntegrar.MoedaValorPorViagem = "";
                        itemIntegrar.TempoMaximoConexao = 0;
                        itemIntegrar.TipoValorPorUnidade = "";
                        itemIntegrar.Valor = 0m;
                        itemIntegrar.ValorPorUnidade = 0m;
                        itemIntegrar.ValorPorUnidadeFracionado = 0m;
                        itemIntegrar.ValorPorViagem = 0m;
                        itemIntegrar.Status = "";
                        itemIntegrar.Messagem = "";
                        itemIntegrar.Carregamento = "";
                        itemIntegrar.dLoad = "";
                        itemIntegrar.PlanoLiquidacao = "";
                        itemIntegrar.Semana1 = 0;
                        itemIntegrar.Semana2 = 0;
                        itemIntegrar.Semana3 = 0;
                        itemIntegrar.Semana4 = 0;
                        itemIntegrar.EstVol = 0;
                        itemIntegrar.UtilizadaPorUltimaVez = "";
                        itemIntegrar.AwdPrty = "";
                        itemIntegrar.ScacAlias = "";
                        itemIntegrar.SapSC = "";
                        itemIntegrar.FuelProg = "";
                        itemIntegrar.Hazmat = "";
                        itemIntegrar.LowAlt = "";
                        itemIntegrar.Ttime = 0;
                        itemIntegrar.SrvcAvl = new List<string>() { "" };
                        itemIntegrar.IncStops = 0;
                        itemIntegrar.PickStops = 0;
                        itemIntegrar.DelStops = 0;
                        itemIntegrar.StopCur = "";
                        itemIntegrar.CostPStop = 0;
                        itemIntegrar.ReturnPerc = 0;
                        itemIntegrar.UnplannedCost = "";
                        itemIntegrar.UnplannedCostType = "";
                        itemIntegrar.FrFreight = 0;
                        itemIntegrar.FrFuel = 0;
                        itemIntegrar.FrTempCost = 0;
                        itemIntegrar.FrTolls = 0;
                        itemIntegrar.PuMinUnits = 0;
                        itemIntegrar.PuMaxUnits = 0;
                        itemIntegrar.PuTotalCost = 0;
                        itemIntegrar.PuBaseCost = "";
                        itemIntegrar.PuFixedCost = "";
                        itemIntegrar.PuFuelCost = 0;
                        itemIntegrar.PuMaxChrg = 0;
                        itemIntegrar.ModDate = "";
                        itemIntegrar.ModBy = "";
                    }

                    if (item.TipoObjeto == TipoParametroBaseTabelaFrete.TipoCarga)
                    {
                        itemIntegrar.MoedaValor = tabelaFreteCliente.ContratoTransporteFrete?.Moeda.ObterSiglaEstrangeira();
                        itemIntegrar.Valor = Math.Round(item.Valor, 2);
                    }
                    else
                    {
                        itemIntegrar.MoedaValorPorUnidade = tabelaFreteCliente.ContratoTransporteFrete?.Moeda.ObterSiglaEstrangeira();
                        itemIntegrar.ValorPorUnidade = item.Valor;

                        if (item.TipoObjeto == TipoParametroBaseTabelaFrete.Peso)
                            itemIntegrar.TipoValorPorUnidade = $"{pesoTabelaFrete.UnidadeMedida.Sigla} (Latam)";
                        else if (item.TipoObjeto == TipoParametroBaseTabelaFrete.Distancia)
                            itemIntegrar.TipoValorPorUnidade = "Quilômetros";
                        else if (item.TipoObjeto == TipoParametroBaseTabelaFrete.TipoEmbalagem)
                            itemIntegrar.TipoValorPorUnidade = "Caixa";
                    }

                    itensIntegrar.Add(itemIntegrar);
                }
            }

            if (itensIntegrar.Count == 0)
                throw new ServicoException("Não existem valores para serem integrados", CodigoExcecao.NenhumRegistroEncontrado);

            Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao tabelaFreteClienteIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.LBC.FreteRequisicao()
            {
                Itens = itensIntegrar.ToArray()
            };

            return tabelaFreteClienteIntegrar;
        }

        #endregion Métodos Privados
    }
}
