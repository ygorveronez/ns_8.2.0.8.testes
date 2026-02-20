using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.OpenTech
{
    public class IntegracaoProdutoOpenTech
    {

        public static List<object> ObterProdutosOpenTech(Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            mensagemErro = null;

            var repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            var configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || configuracaoIntegracao.CodigoClienteOpenTech <= 0 || configuracaoIntegracao.CodigoPASOpenTech <= 0 || string.IsNullOrWhiteSpace(configuracaoIntegracao.DominioOpenTech) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaOpenTech) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLOpenTech) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioOpenTech))
                throw new ServicoException(Localization.Resources.Produtos.GrupoProduto.ConfiguracaoDeIntegracaoOpenTechInvalida);

            ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, configuracaoIntegracao?.URLOpenTech);

            var retornoLogin = EfetuarLogin(ref svcOpenTech, configuracaoIntegracao, unidadeTrabalho);

            if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
                throw new ServicoException($"Não foi possível realizar o login: {retornoLogin.ReturnDescription}");

            var retorno = svcOpenTech.sgrListaProdutos(retornoLogin.ReturnKey, configuracaoIntegracao.CodigoPASOpenTech, configuracaoIntegracao.CodigoClienteOpenTech);

            var produtos = new List<object>();

            if (retorno == null || retorno.ReturnDataset == null)
            {
                mensagemErro = $"Nenhum produto retornou na consulta: {retorno.ReturnDescription}";
                return produtos;
            }

            try
            {
                XElement xmlData = (XElement)retorno.ReturnDataset.Nodes[1].FirstNode;
                IEnumerable<XElement> produtosXml = xmlData.Elements("sgrTB");

                foreach (XElement produtoXml in produtosXml)
                {
                    string cdProd = produtoXml.Element("CDPROD")?.Value ?? "";
                    string dsProduto = produtoXml.Element("DSPRODUTO")?.Value ?? "";

                    produtos.Add(new { value = cdProd, text = $"{cdProd} - {dsProduto}" });
                }
            }
            catch (Exception ex)
            {
                mensagemErro = $"Erro ao processar produtos: {ex.Message}";
                return produtos;
            }

            return produtos;
        }

        public static List<object> ObterTipoSensoresOpenTech(Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null || configuracaoIntegracao.CodigoClienteOpenTech <= 0 || configuracaoIntegracao.CodigoPASOpenTech <= 0 ||
                string.IsNullOrWhiteSpace(configuracaoIntegracao.DominioOpenTech) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaOpenTech) ||
                string.IsNullOrWhiteSpace(configuracaoIntegracao.URLOpenTech) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioOpenTech))
            {
                mensagemErro = Localization.Resources.Produtos.GrupoProduto.ConfiguracaoDeIntegracaoOpenTechInvalida;

                return null;
            }

            ServicoOpenTech.sgrOpentechSoapClient svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoOpenTech.sgrOpentechSoapClient, ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, configuracaoIntegracao.URLOpenTech);

            ServicoOpenTech.sgrData retornoLogin = EfetuarLogin(ref svcOpenTech, configuracaoIntegracao, unidadeTrabalho);

            if (string.IsNullOrWhiteSpace(retornoLogin.ReturnKey))
            {
                mensagemErro = "Não foi possível realizar o login: " + retornoLogin.ReturnDescription;

                return null;
            }

            //ServicoOpenTech.sgrData retorno = svcOpenTech.sgrListaTiposSensoresTemperatura(retornoLogin.ReturnKey);
            ServicoOpenTech.sgrData retorno = svcOpenTech.sgrListaTiposSensoresTemperaturaCliente(retornoLogin.ReturnKey, configuracaoIntegracao.CodigoPASOpenTech, configuracaoIntegracao.CodigoClienteOpenTech);

            List<object> produtos = new List<object>();

            if (retorno != null && retorno.ReturnDataset != null)
            {
                try
                {
                    XElement xmlData = (XElement)retorno.ReturnDataset.Nodes[1].FirstNode;
                    IEnumerable<XElement> sensoresXml = xmlData.Elements("sgrTB");

                    if (sensoresXml != null)
                    {
                        foreach (XElement sensorXml in sensoresXml)
                        {
                            string cdTpSensTemp = sensorXml.Element("cdTpSensTemp")?.Value ?? "";
                            string dsSensor = sensorXml.Element("dsSensor")?.Value ?? "";

                            Servicos.Log.TratarErro("cdTpSensTemp: " + cdTpSensTemp + " dsSensor: " + dsSensor, "Opentech");

                            produtos.Add(new
                            {
                                value = cdTpSensTemp,
                                text = dsSensor
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    mensagemErro = $"Erro ao processar sensores: {ex.Message}";
                    return produtos;
                }
            }

            mensagemErro = null;

            return produtos;
        }

        private static ServicoOpenTech.sgrData EfetuarLogin(ref ServicoOpenTech.sgrOpentechSoapClient svcOpenTech, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            ServicoOpenTech.sgrData retornoLogin = svcOpenTech.sgrLogin(configuracaoIntegracao.UsuarioOpenTech, configuracaoIntegracao.SenhaOpenTech, configuracaoIntegracao.DominioOpenTech);

            return retornoLogin;
        }
    }
}
