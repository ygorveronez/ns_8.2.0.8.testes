using System;
using System.IO;
using System.Text;

namespace AlterarArquivoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Verifica se foram passados exatamente dois argumentos
            if (args.Length != 3)
            {
                Console.WriteLine("Por favor, forneça exatamente dois argumentos: caminho+arquivo webconfig, caminho+arquivo ASSMBLYS.txt e pathBK aonde o bk deve ser guardado");
            }
            else
            {
                string webconfig = args[0];
                string assemblys = args[1];
                string pathBK = args[2];

                AlterarConteudoArquivo(webconfig, assemblys, pathBK);
            }

            //Console.WriteLine("\nPressione qualquer tecla para sair...");
            //Console.ReadKey();
        }

        static void AlterarConteudoArquivo(string webconfig, string assemblys, string pathBK)
        {

            if (!System.IO.File.Exists(assemblys))
            {
                Console.WriteLine("Arquivo de assemblys não encontrado.");
                return;
            }
            try
            {
                string fileContent = System.IO.File.ReadAllText(webconfig);
                string novoConteudo = System.IO.File.ReadAllText(assemblys);

                // Definir as tags de início e fim
                string tagInicio = "<runtime>";
                string tagFim = "</runtime>";

                // Variáveis para armazenar as posições das tags
                int inicioIndex = -1;
                int fimIndex = -1;

                // Percorrer o conteúdo do arquivo caractere por caractere
                for (int i = 0; i < fileContent.Length; i++)
                {
                    if (inicioIndex == -1 && fileContent.Substring(i).StartsWith(tagInicio))
                    {
                        inicioIndex = i + tagInicio.Length;
                    }
                    else if (inicioIndex != -1 && fileContent.Substring(i).StartsWith(tagFim))
                    {
                        fimIndex = i;
                        break;
                    }
                }

                if (inicioIndex == -1 || fimIndex == -1)
                    return;

                // Construir o novo conteúdo do arquivo
                StringBuilder novoFileContent = new StringBuilder();
                novoFileContent.Append(fileContent.Substring(0, inicioIndex)); // Antes do conteúdo a substituir
                novoFileContent.Append(novoConteudo); // Novo conteúdo
                novoFileContent.Append(fileContent.Substring(fimIndex)); // Depois do conteúdo a substituir

                if (novoFileContent.ToString().Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "") != fileContent.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", ""))
                {
                    string BK = System.IO.Path.Combine(pathBK, $"WebConfig_BK{DateTime.Now.Year.ToString()}{DateTime.Now.Month.ToString()}{DateTime.Now.Day.ToString()}");
                    System.IO.File.WriteAllText(BK, fileContent.ToString());

                    if (!System.IO.File.Exists(BK))
                        return;

                    System.IO.File.WriteAllText(webconfig, novoFileContent.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro: " + ex.Message);
            }
        }


    }
}