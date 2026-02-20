using Dominio.ObjetosDeValor.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.Electrolux
{
    public abstract class IntegracaoElectroluxBase
    {

        protected readonly Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux _configuracaoIntegracaoElectroluxRepositorio;
        protected Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux _configuracaoIntegracaoElectroluxDominio;
        protected HttpRequisicaoResposta _httpRequisicaoResposta;
        protected List<object> _result;

        protected IntegracaoElectroluxBase(Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoIntegracaoElectroluxDominio = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux();
            _configuracaoIntegracaoElectroluxRepositorio = new Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux(unitOfWork);
            _httpRequisicaoResposta = new HttpRequisicaoResposta();
            _result = new List<object>();
        }

        /// <summary>
        /// Formata a string informada para o tamanho máximo exigido pelo serviço
        /// Deixei com o nome curto para não "poluir" o código
        /// </summary>
        /// <param name="texto">Texto para formatação</param>
        /// <param name="tamanhoMaximo">Quantidade de caracteres</param>
        /// <returns>String formatada</returns>
        protected static string sf(string texto, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(texto) || string.IsNullOrWhiteSpace(texto))
                return string.Empty;
            else if (texto.Length > tamanhoMaximo)
                return texto.Substring(0, tamanhoMaximo);
            else
                return texto;
        }

        /// <summary>
        /// Ajusta DATETIME para o formato correto
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parentTag"></param>
        /// <param name="fieldTag"></param>
        /// <param name="formatType"></param>
        protected static void FormatXmlDateTime(XmlDocument doc, string parentTag, string fieldTag, string formatType)
        {
            XmlNodeList parentNodes = doc.GetElementsByTagName(parentTag);

            foreach (XmlNode parentNode in parentNodes)
            {
                XmlNode fieldNode = parentNode[fieldTag];

                if (fieldNode != null)
                {
                    if (formatType == "date")
                    {
                        DateTime dateValue;
                        if (DateTime.TryParse(fieldNode.InnerText, out dateValue))
                        {
                            fieldNode.InnerText = dateValue.ToString("yyyy-MM-dd");
                        }
                    }
                    else if (formatType == "time")
                    {
                        DateTime timeValue;
                        if (DateTime.TryParse(fieldNode.InnerText, out timeValue))
                        {
                            fieldNode.InnerText = timeValue.ToString("HH:mm:ss");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ajusta as tags de abertura e fechamento para atender ao formato
        /// </summary>
        /// <param name="xmlContent">xml Completo</param>
        /// <param name="prefix">prefixo</param>
        /// <param name="tagName">nome da tag</param>
        /// <returns>xml corrigido</returns>
        protected static string AjustarTag(string xmlContent, string prefix, string tagName)
        {            
            return xmlContent.Replace($"<{tagName}>", $"<{prefix}:{tagName}>").Replace($"</{tagName}>", $"</{prefix}:{tagName}>");
        }

        /// <summary>
        /// Retorna o valor padrão para um elemento informado, sem importar o tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="xpath"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Valor elemento</returns>
        protected static T GetValue<T>(XElement element, string xpath, T defaultValue)
        {
            var value = element.Element(xpath)?.Value;

            if (value == null)
                return defaultValue;

            try
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(value);
            }
            catch
            {
                return defaultValue;
            }
        }

    }

}
