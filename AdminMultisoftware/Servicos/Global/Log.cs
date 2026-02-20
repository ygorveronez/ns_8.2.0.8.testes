using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos
{
    public class Log
    {
        private static Log _Log = new Log();

        /// <summary>
        /// Grava exceção no arquivo de Log
        /// </summary>
        /// <param name="ex">Exceção capturada pelo catch</param>
        public static void TratarErro(Exception ex)
        {
            try
            {
                GravaConteudoNoArquivo(ex.ToString());
            }
            catch
            {
            }
        }

        /// <summary>
        /// Grava exceção no arquivo de Log com prefixo específico
        /// </summary>
        /// <param name="ex">Exceção capturada pelo catch</param>
        /// <param name="arquivo">Prefixo específico (PREFIXO-dd-mm-aaaa.txt)</param>
        public static void TratarErro(Exception ex, string arquivo)
        {
            try
            {
                GravaConteudoNoArquivo(ex.ToString(), arquivo);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        public static void TratarErro(string mensagem)
        {
            GravaConteudoNoArquivo(mensagem);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log com prefixo específico
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        /// <param name="arquivo">Prefixo específico (PREFIXO-dd-mm-aaaa.txt)</param>
        public static void TratarErro(string mensagem, string arquivo)
        {
            GravaConteudoNoArquivo(mensagem, arquivo);
        }

        private static void GravaConteudoNoArquivo(string mensagem, string prefixo = "")
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string arquivo = (string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-") + dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year + ".txt";
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                string file = System.IO.Path.Combine(path, arquivo);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                System.IO.StreamWriter strw = new System.IO.StreamWriter(file, true);
                try
                {
                    strw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    strw.WriteLine(mensagem);
                    strw.WriteLine();
                }
                catch
                {
                }
                finally
                {
                    strw.Close();
                }
            }
            catch
            {
            }
        }
    }
}
