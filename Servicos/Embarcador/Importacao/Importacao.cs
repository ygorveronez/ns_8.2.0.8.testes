using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Excel;
using Newtonsoft.Json;
using Servicos.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Servicos.Embarcador.Importacao
{
    public class Importacao
    {

        /// <summary>
        /// Esse método cria uma lista de entidades para importação, tomando como base dados recebidos por request. Todas instancias são criadas como New
        /// </summary>
        public static List<T> PreencheObjetoImportacao<T>(HttpRequestBase request, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes, out string erro)
        {
            List<Dictionary<string, dynamic>> dicionario = null;
            List<T> listaImportacao = new List<T>();

            ImportarInformacoes(request, configuracoes, ref listaImportacao, ref dicionario, out erro, null);

            return listaImportacao;
        }



        /// <summary>
        /// Esse método cria uma lista de entidades para importação, tomando como base dados recebidos por request. Todas instancias são criadas como New
        /// </summary>
        public static RetornoImportacao ImportarInformacoes<T>(HttpRequestBase request, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes, ref List<T> listaEntidades, ref List<Dictionary<string, dynamic>> dadosDaLinha, out string erro)
        {
            return ImportarInformacoes(request, configuracoes, ref listaEntidades, ref dadosDaLinha, out erro, null);
        }

        /// <summary>
        /// Esse método cria uma lista de entidades para importação, tomando como base dados recebidos por request. Todas instancias são criadas com retorno de Lambda
        /// </summary>
        /// <param name="lambda">Função executada em tempo de execução para intanciar a classe, ex: Faz busca por T = repT.BuscaPorCodigo(dicionario["Codigo"]). Caso retorne nulo, sera criado uma instancia normalmente</param>
        public static RetornoImportacao ImportarInformacoes<T>(HttpRequestBase request, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes, ref List<T> listaEntidades, ref List<Dictionary<string, dynamic>> dadosDaLinha, out string erro, Func<Dictionary<string, dynamic>, T> lambda = null)
        {
            erro = string.Empty;

            // Recebe os dados
            string dados = request.Params["Dados"];
            int total = 0, importados = 0;

            // Propriedades obrigatorias
            List<string> propObrigatorias = ListaPropriedades(configuracoes, true);

            // Lista de Propriedades
            List<string> propriedades = ListaPropriedades(configuracoes, false);

            // Converte JSON
            List<DadosLinha> jsonDados = JsonConvert.DeserializeObject<List<DadosLinha>>(dados);

            // Guarda dados para reimportar
            List<DadosLinha> dadosReimportacao = new List<DadosLinha>();

            // Numero total de linha
            total = jsonDados.Count();

            // Dicionario de campos
            Dictionary<string, Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> dicionarioPropriedades = DicionarioPropriedades(configuracoes);

            for (var i = 0; i < jsonDados.Count(); i++)
            {
                // Auxiliar
                DadosLinha linha = jsonDados[i];

                // Dicionario da linha
                Dictionary<string, dynamic> dadosDaLinhaIterrada = new Dictionary<string, dynamic>();

                // Instancia a entidade
                // Se existe uma lambda, a instancia é obtida em runtime
                // Caso contrario, permanecerá com o construtor da classe

                // T ent = new T();
                T ent = default(T);
                ent = Activator.CreateInstance<T>();

                if (lambda != null)
                {
                    // T ent = lambda(); // lambda retornar a classe da importacao => T
                    T ent_lambda = lambda(DadosLinhaToDictionary(linha));
                    if (ent_lambda != null) ent = ent_lambda;
                }

                try
                {
                    // Preenche a entidade
                    PreencheObjetoImportacao(linha, dicionarioPropriedades, propObrigatorias, ref ent, ref dadosDaLinhaIterrada);
                }
                catch (Exception e)
                {
                    // Adiciona para reimportacao
                    dadosReimportacao.Add(linha);

                    // Mensagem já tratada
                    erro = e.Message;

                    // Para looping
                    break;
                }

                // Adiciona no retorno
                listaEntidades.Add(ent);
                if (dadosDaLinha != null)
                    dadosDaLinha.Add(dadosDaLinhaIterrada);
            }

            // Contagem de entidades criadas
            importados = listaEntidades.Count();

            return new RetornoImportacao
            {
                Total = total,
                Importados = 0,
                Reimportar = (dadosReimportacao.Count() > 0) ? ConverteReimportacao(dadosReimportacao) : null
            };
        }

        /// <summary>
        /// Esse método cria uma lista de entidades para importação, tomando como base dados recebidos por request. Todas instancias são criadas com retorno de Lambda
        /// </summary>
        /// <param name="lambda">Função executada em tempo de execução para intanciar a classe, ex: Faz busca por T = repT.BuscaPorCodigo(dicionario["Codigo"]). Caso retorne nulo, sera criado uma instancia normalmente</param>
        public static RetornoImportacao PreencherImportacaoManual<T>(HttpRequestBase request, Func<Dictionary<string, dynamic>, T> lambda)
        {
            return PreencherImportacaoManual(request, null, lambda);
        }

        /// <summary>
        /// Esse método cria uma lista de entidades para importação, tomando como base dados recebidos por request. Todas instancias são criadas com retorno de Lambda
        /// </summary>
        /// <param name="lambda">Função executada em tempo de execução para intanciar a classe, ex: Faz busca por T = repT.BuscaPorCodigo(dicionario["Codigo"]). Caso retorne nulo, sera criado uma instancia normalmente</param>
        public static RetornoImportacao PreencherImportacaoManual<T>(HttpRequestBase request, List<T> listaEntidades, Func<Dictionary<string, dynamic>, T> lambda)
        {
            string dados = request.Params["Dados"];
            int total = 0, importados = 0;

            List<DadosLinha> jsonDados = JsonConvert.DeserializeObject<List<DadosLinha>>(dados);

            List<RetonoLinha> retornoLinhas = new List<RetonoLinha>();

            total = jsonDados.Count();


            for (var i = 0; i < jsonDados.Count(); i++)
            {
                DadosLinha linha = jsonDados[i];
                RetonoLinha retonoLinha = new RetonoLinha()
                {
                    indice = i,
                    mensagemFalha = "",
                    processou = true
                };

                T ent = default(T);

                try
                {
                    ent = lambda(DadosLinhaToDictionary(linha));
                    importados++;
                }
                catch (BaseException impEx)
                {
                    retonoLinha.mensagemFalha = impEx.Message;
                    retonoLinha.processou = false;
                }
                catch (Exception)
                {
                    retonoLinha.mensagemFalha = "Falha ao processar linha, colunas fora do padrão de importação.";
                    retonoLinha.processou = false;
                }

                if (ent != null && listaEntidades != null)
                    listaEntidades.Add(ent);

                retornoLinhas.Add(retonoLinha);
            }

            return new RetornoImportacao
            {
                Total = total,
                Importados = importados,
                Retornolinhas = retornoLinhas
            };
        }

        public static Dictionary<string, List<string>> ObterValoresLinha(List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> colunas, List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas)
        {
            List<string> colunasPropriedade = colunas
                .Where(x => x.CampoEntidade)
                .Select(x => x.Propriedade)
                .ToList();

            Dictionary<string, HashSet<string>> propriedadesValoresTemp = new();

            foreach (var item in linhas)
            {
                foreach (var coluna in item.Colunas)
                {
                    if (colunasPropriedade.Contains(coluna.NomeCampo) && coluna.Valor != null)
                    {
                        string chave = coluna.NomeCampo;
                        string valor = coluna.Valor.ToString().Trim();

                        if (!propriedadesValoresTemp.ContainsKey(chave))
                            propriedadesValoresTemp[chave] = new HashSet<string>();

                        propriedadesValoresTemp[chave].Add(valor);
                    }
                }
            }

            return propriedadesValoresTemp.ToDictionary(
                par => par.Key,
                par => par.Value.ToList()
            );
        }


        private static Dictionary<string, dynamic> DadosLinhaToDictionary(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha)
        {
            /* Conveter os dados da linha para dicionario
             * Necessariamente para parametrizar no momento de criar uma instancia
             */
            Dictionary<string, dynamic> dadosDaColuna = new Dictionary<string, dynamic>();

            // Preenche as informacoes importadas
            for (var j = 0; j < linha.Colunas.Count(); j++)
            {
                // Dados da coluna
                DadosColuna coluna = linha.Colunas[j];

                // Vincula ao dicionario da linha
                dadosDaColuna[coluna.NomeCampo] = coluna.Valor;
            }
            return dadosDaColuna;
        }

        private static T PreencheObjetoImportacao<T>(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dictionary<string, Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> dicionarioPropriedades, List<string> propObrigatorias, ref T ent, ref Dictionary<string, dynamic> dadosDaColuna)
        {
            bool erroDeAcesso = false;
            try
            {
                /* Toda linha, é iterrado e removido das propriedades obrigatorias toda 
                 * vez que passar por uma
                 * Ocorre um erro, quando propObrigatorias.Count() é maior que 0
                 * Isso é, quando não foi selecionado TODAS propriedades obrigatórias
                 */

                // Preenche as informacoes importadas
                for (var j = 0; j < linha.Colunas.Count(); j++)
                {
                    // Dados da coluna
                    DadosColuna coluna = linha.Colunas[j];

                    // Auxiliar para acesso de propriedades
                    PropertyInfo prop;

                    // Verifica se o campo existe
                    Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao config;
                    if (dicionarioPropriedades.TryGetValue(coluna.NomeCampo, out config))
                    {
                        // Remove dos obrigatorios
                        if (propObrigatorias.Contains(coluna.NomeCampo))
                            propObrigatorias.Remove(coluna.NomeCampo);

                        // Seta valor da propriedade se existe e é necessário
                        if (!config.CampoInformacao)
                        {
                            prop = ent.GetType().GetProperty(coluna.NomeCampo, BindingFlags.Public | BindingFlags.Instance);
                            string typePropriedade = "";
                            if (prop != null)
                                typePropriedade = prop.PropertyType.Name;

                            dynamic valorConvertido = null;
                            switch (typePropriedade)
                            {
                                case "Decimal":
                                    decimal valorDecimal = 0;
                                    decimal.TryParse(coluna.Valor, out valorDecimal);
                                    valorConvertido = valorDecimal;
                                    break;
                                case "Int32":
                                    int valorInteiro = 0;
                                    int.TryParse(coluna.Valor, out valorInteiro);
                                    valorConvertido = valorInteiro;
                                    break;
                                case "DateTime":
                                    DateTime valorDateTime;
                                    DateTime.TryParse(coluna.Valor, out valorDateTime);
                                    valorConvertido = valorDateTime;
                                    break;
                                case "String":
                                    valorConvertido = coluna.Valor;
                                    break;
                                case "Nullable":
                                    valorConvertido = coluna.Valor;
                                    break;
                            }
                            prop.SetValue(ent, valorConvertido, null);
                        }
                    }
                    else
                    {
                        erroDeAcesso = true;
                        throw new Exception("Erro: " + coluna.NomeCampo + " não existe.");
                    }

                    // Vincula ao dicionario da linha
                    dadosDaColuna[coluna.NomeCampo] = coluna.Valor;
                }

                // Valida propriedades obrigatorias
                if (propObrigatorias.Count() > 0)
                    throw new Exception("Campos com * são obrigatórios.");
            }
            catch (Exception ex)
            {
                if (erroDeAcesso)
                    throw new Exception(ex.Message);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    throw new Exception("Ocorreu uma falha ao importar arquivo.");
                }
            }

            return ent;
        }

        public static List<List<string>> ConverteReimportacao(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas)
        {
            /* ConverteReimportacao
             * Pega os dados que causaram erro e retorna para reimportar
             */
            List<List<string>> novoJson = new List<List<string>>();

            for (var i = 0; i < linhas.Count(); i++)
            {
                List<string> novaLinha = new List<string>();

                for (var j = 0; j < linhas[i].Colunas.Count(); j++)
                    novaLinha.Add(linhas[i].Colunas[j].Valor);

                novoJson.Add(novaLinha);
            }

            return novoJson;
        }

        public static Dominio.ObjetosDeValor.ImportacaoArquivo.Importado ConverterArquivo(CustomFile arquivo, out string erro, bool removerArquivoAposProcessamento, Repositorio.UnitOfWork unitOfWork)
        {
            erro = string.Empty;

            string[] extensoesValidas = { ".csv", ".xls", ".xlsx", ".txt" };

            // Valida quantidade aruivos
            if (arquivo == null)
            {
                erro = "Nenhum arquivo recebido.";
                return null;
            }

            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterCaminhoArquivos(), "ImportacaoArquivos", typeof(Dominio.ObjetosDeValor.ImportacaoArquivo.Importado).Name });
            string extensaoArquivo = Path.GetExtension(arquivo.FileName).ToLower();
            string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}");

            if (!extensoesValidas.Contains(extensaoArquivo))
            {
                erro = "Extensão " + extensaoArquivo.Substring(1) + " inválida.";
                return null;
            }

            arquivo.SaveAs(caminhoCompleto);

            Dominio.ObjetosDeValor.ImportacaoArquivo.Importado obj = new Dominio.ObjetosDeValor.ImportacaoArquivo.Importado()
            {
                FileName = arquivo.FileName,
                SalvoComo = removerArquivoAposProcessamento ? string.Empty : $"{guidArquivo}{extensaoArquivo}",
                ContentLength = arquivo.Length,
                ContentType = arquivo.ContentType,
                Content = ConverteArquivoInput(arquivo.InputStream, extensaoArquivo),
            };

            if (removerArquivoAposProcessamento && Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompleto))
                Utilidades.IO.FileStorageService.Storage.Delete(caminhoCompleto);

            return obj;
        }

        #region ArquivosOffice

        private static object ConverteArquivoInput(Stream stream, string extensao)
        {
            // Remove o ponto
            extensao = extensao.Substring(1);

            // Chama o metodo referente a extensao
            if (extensao.Equals("csv") || extensao.Equals("txt"))
                return ConverteCSV(stream);
            else if (extensao.Equals("xls"))
                return ConverteXLS(stream);
            else if (extensao.Equals("xlsx"))
                return ConverteXLSX(stream);

            return "";
        }

        private static object ConverteCSV(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            StreamReader csvreader = new StreamReader(stream);

            var linhas = new List<List<string>>();

            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(';');
                bool possuiValor = false;
                List<string> celulas = new List<string>();

                for (int j = 0; j < values.Length; j++)
                {
                    string valorCelula = values[j];
                    if (!string.IsNullOrWhiteSpace(valorCelula) && !possuiValor)
                        possuiValor = true;
                    celulas.Add(valorCelula);
                }

                if (possuiValor)
                    linhas.Add(celulas);
            }

            csvreader.Close();

            return linhas;
        }

        private static object ConverteXLS(Stream stream)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            return ArquivosOffice(excelReader);
        }

        private static object ConverteXLSX(Stream stream)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            return ArquivosOffice(excelReader);
        }

        private static object ArquivosOffice(IExcelDataReader excelReader)
        {
            var linhas = new List<List<String>>();

            while (excelReader.Read())
            {
                int numeroDeCelulas = excelReader.FieldCount;
                List<String> celulas = new List<string>();
                bool possuiValor = false;

                for (var j = 0; j < numeroDeCelulas; j++)
                {

                    string valorCelula = excelReader.GetString(j);
                    if (!string.IsNullOrWhiteSpace(valorCelula) && !possuiValor)
                        possuiValor = true;
                    celulas.Add(valorCelula);
                }

                if (possuiValor)
                    linhas.Add(celulas);
            }

            excelReader.Close();

            return linhas;
        }
        #endregion

        private static List<string> ListaPropriedades(List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes, bool obrigatoria)
        {
            /* Itera as configuracoes fornecidas para listar apenas as obrigatorias
             */
            List<string> props = new List<string>();

            for (var i = 0; i < configuracoes.Count(); i++)
                if ((obrigatoria && configuracoes[i].Obrigatorio) || !obrigatoria)
                    props.Add(configuracoes[i].Propriedade);

            return props;
        }

        private static Dictionary<string, Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> DicionarioPropriedades(List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes)
        {
            /* Itera as configuracoes fornecidas para listar apenas as obrigatorias
             */
            Dictionary<string, Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> dicionario = new Dictionary<string, Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            for (var i = 0; i < configuracoes.Count(); i++)
                dicionario[configuracoes[i].Propriedade] = configuracoes[i];

            return dicionario;
        }
    }
}
