using System;

namespace Google.OrTools.Api.Services
{
    public class Errors
    {
        public static void GravaLog(Exception ex, string compl = "")
        {
            GravaLogEmTxt(ex?.ToString(), compl, true);
        }

        public static void GravaLogEmTxt(string conteudo, string compl = "", bool erro = false)
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

                //Se não existe a pasta cria a ela no startap da aplicação
                if (!(System.IO.Directory.Exists(path)))
                    System.IO.Directory.CreateDirectory(path);

                ExcluirArquivosLogsAntigos(path);

                //recebe a data atual para gravar o log
                string file = DateTime.Now.ToString("dd-MM-yyyy") + ".txt";

                path = System.IO.Path.Combine(path, file);

                var streamWriter = new System.IO.StreamWriter(path, true);
                try
                {
                    streamWriter.WriteLine(DateTime.Now.ToLongTimeString());
                    if (compl.Length > 0)
                        streamWriter.WriteLine("Complemento : " + compl);

                    if (erro)
                        streamWriter.WriteLine("EXCEPTION : ");

                    streamWriter.WriteLine(conteudo);
                    streamWriter.WriteLine();
                }
                catch (Exception ex)
                {
                    // Erro ao escrever no arquivo de log - registra no log geral
                    Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao escrever no arquivo de log do Google OrTools: {ex}", "CatchNoAction");
                }
                finally { streamWriter.Close(); }
            }
            catch (Exception ex)
            {
                // Erro ao abrir/criar arquivo de log
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao abrir arquivo de log do Google OrTools: {ex}", "CatchNoAction");
            }
        }

        private static void ExcluirArquivosLogsAntigos(string path)
        {
            try
            {
                var directory = new System.IO.DirectoryInfo(path);
                if (directory.Exists)
                {
                    var files = directory.GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].CreationTime < DateTime.Now.AddDays(-30))
                            files[i].Delete();
                    }
                    files = null;
                }
                directory = null;
                System.GC.Collect();
            }
            catch (Exception ex)
            {
                // Erro ao excluir arquivos de log antigos
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao excluir arquivos de log antigos do Google OrTools: {ex}", "CatchNoAction");
            }
        }
    }
}
