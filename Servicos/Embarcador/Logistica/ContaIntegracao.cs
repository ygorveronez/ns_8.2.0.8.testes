using System;
using System.Collections.Generic;
using System.Text;

namespace Servicos.Embarcador.Logistica
{

    public static class ContaIntegracao
    {

        #region Constantes

        private const char CHAR_SEPARADOR_CHAVE_VALOR = '=';
        private const char CHAR_SEPARADOR_PARAMETROS = ';';

        #endregion

        public static string ObterURLBase(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            string url = $"{contaIntegracao.Protocolo.ToString().ToLower()}://{contaIntegracao.Servidor}";
            if (contaIntegracao.Porta > 0) url += $":{contaIntegracao.Porta}";
            return url;
        }

        public static string ObterURL(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            string url = ObterURLBase(contaIntegracao);
            url += $"{contaIntegracao.URI}";
            return url;
        }

        public static string ObterURLPadrao(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            return contaIntegracao.URI;
        }

        public static List<KeyValuePair<string, string>> ObterListaParametrosAdicionais(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            return ObterListaParametrosAdicionais(contaIntegracao.ParametrosAdicionais);
        }

        public static List<KeyValuePair<string, string>> ObterListaParametrosAdicionais(string parametrosAdicionais)
        {
            List<KeyValuePair<string, string>> lista = new List<KeyValuePair<string, string>>();
            string[] parametros = parametrosAdicionais.Trim().Split(CHAR_SEPARADOR_PARAMETROS);
            int total = parametros.Length;
            for (int i = 0; i < total; i++)
            {
                string param = parametros[i].Trim();
                int pos = param.IndexOf(CHAR_SEPARADOR_CHAVE_VALOR);
                if (pos > 0)
                {
                    lista.Add(new KeyValuePair<string, string>(param.Substring(0, pos), param.Substring(pos+1)));
                }
            }
            return lista;
        }

        public static string ObterValorListaParametrosAdicionais(string key, List<KeyValuePair<string, string>> lista)
        {
            int total = lista.Count;
            for (int i = 0; i < total; i++)
            {
                if (lista[i].Key == key)
                {
                    return lista[i].Value;
                }
            }
            return string.Empty;
        }

        public static string ObterBasicToken(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            string usuario = contaIntegracao.Usuario;
            string senha = contaIntegracao.Senha;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(usuario + ":" + senha));
        }

    }

}
