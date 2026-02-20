/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zeus.Embarcador.ZeusNFe
{
    public static class Funcoes
    {

        /// <summary>
        ///     Obtém as propriedades de um determinado objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objeto"></param>
        /// <returns>Retorna um objeto Dictionary contendo o nome da propriedade e seu valor</returns>
        public static Dictionary<string, object> LerPropriedades<T>(T objeto) where T : class
        {
            //A função pode ser melhorada para trazer recursivamente as proprieades dos objetos filhos
            var dicionario = new Dictionary<string, object>();

            foreach (var attributo in objeto.GetType().GetProperties())
            {
                var value = attributo.GetValue(objeto, null);
                dicionario.Add(attributo.Name, value);
            }

            return dicionario;
        }

        /// <summary>
        ///     Obtém uma lista contendo os nomes das propriedades cujo valor não foi definido ou está vazio, de um determinado objeto
        ///     passado como parâmetro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objeto"></param>
        /// <returns>Retorna uma lista de strings</returns>
        public static List<string> ObterPropriedadesEmBranco<T>(T objeto)
        {
            return
                (from attributo in objeto.GetType().GetProperties()
                 let value = attributo.GetValue(objeto, null)
                 where value == null || string.IsNullOrEmpty(value.ToString())
                 select attributo.Name).ToList();
        }

        /// <summary>
        ///     Copia o valor das propriedades de um objeto para outro de estrutura igual
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objetoOrigem"></param>
        /// <param name="objetoDestino"></param>
        public static void CopiarPropriedades<T>(T objetoOrigem, T objetoDestino) where T : class
        {
            foreach (var attributo in objetoOrigem.GetType().GetProperties())
            {
                var propertyInfo = objetoDestino.GetType().GetProperty(attributo.Name, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                    propertyInfo.SetValue(objetoDestino, attributo.GetValue(objetoOrigem, null), null);
            }
        }

        /// <summary>
        ///     Abre o diálogo de busca de arquivo com o filtro configurado para arquivos do tipo ".xml"
        /// </summary>
        /// <returns></returns>
        public static string BuscarArquivoXml()
        {
            return BuscarArquivo("Selecione o arquivo XML", ".xml", "Arquivo XML (.xml)|*.xml");
        }

        public static string BuscarArquivoPdf()
        {
            return BuscarArquivo("Selecione o arquivo Pdf", ".pdf", "Arquivo Pdf (.pdf)|*.pdf");
        }

        /// <summary>
        ///     Abre o diálogo de busca de arquivo com o filtro configurado para arquivos do tipo ".pfx ou todos os arquivos (*.*)"
        /// </summary>
        /// <returns></returns>
        public static string BuscarArquivoCertificado()
        {
            return BuscarArquivo("Selecione o arquivo de Certificado", ".pfx", "Arquivos PFX (*.pfx)|*.pfx|Todos os Arquivos (*.*)|*.*");
        }

        /// <summary>
        ///     Abre o diálogo de busca de arquivo com o filtro configurado para arquivos do tipo "PNG, Bitmap, JPEG, JPG e GIF"
        /// </summary>
        /// <returns></returns>
        public static string BuscarImagem()
        {
            return BuscarArquivo("Selecione uma imagem", ".png", "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpeg)|*.jpeg|JPG (*.jpg)|*.jpg|GIF (*.gif)|*.gif");
        }

        /// <summary>
        ///     Abre o diálogo de busca de arquivo com com os dados passados no parâmetro
        /// </summary>
        /// <param name="arquivoPadrao">Nome do arquivo padrão a ser exibido no diálogo</param>
        /// <param name="extensaoPadrao">Extensão de arquivo padrão a ser exibida no diálogo</param>
        /// <param name="filtro">Filtro de extensões a ser exibido no diálogo</param>
        /// <returns></returns>
        public static string BuscarArquivo(string titulo, string extensaoPadrao, string filtro, string arquivoPadrao = null)
        {
            //var dlg = new OpenFileDialog
            //{
            //    Title = titulo,
            //    FileName = arquivoPadrao,
            //    DefaultExt = extensaoPadrao,
            //    Filter = filtro
            //};
            //dlg.ShowDialog();
            //return dlg.FileName;
            return string.Empty;
        }

        /// <summary>
        ///     Obtém informações de uma propriedade de um objeto.
        ///     <example>var propinfo = Funcoes.ObterPropriedadeInfo(_cfgServico, c => c.DiretorioSalvarXml);</example>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns>Retorna um objeto do tipo PropertyInfo com as informações da propriedade, como nome, tipo, etc</returns>
        public static PropertyInfo ObterPropriedadeInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("A expressão '{0}' se refere a um método, não a uma propriedade!", propertyLambda));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format("A expressão '{0}' se refere a um campo, não a uma propriedade!", propertyLambda));

            if (propInfo.ReflectedType != null && (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType)))
                throw new ArgumentException(string.Format("A expressão '{0}' refere-se a uma propriedade, mas não é do tipo {1}!", propertyLambda, type));

            return propInfo;
        }

        /// <summary>
        ///     Divide um lista do tipo <T> em listas menores do mesmo tipo
        /// </summary>
        /// <param name="quantidade">Tamanho da nova lista</param>
        /// <returns>Retorna uma lista de objetos do tipo <T> com o tamanho definido na função</returns>
        public static List<List<T>> DividirListaEmLotes<T>(this List<T> lista, int quantidade)
        {
            var lotes = new List<List<T>>();

            for (int i = 0; i < lista.Count; i += quantidade)
                lotes.Add(lista.GetRange(i, Math.Min(quantidade, lista.Count - i)));

            return lotes;
        }
    }
}
*/