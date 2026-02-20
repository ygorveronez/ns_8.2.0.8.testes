using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento
{

    public abstract class AbstractThread
    {

        #region Atributos públicos

        public readonly string strNewLine = "\r\n";

        #endregion

        #region Atributos protegidos

        protected int nivelLog = 10;

        #endregion

        #region Métodos protegidos

        protected bool PossuiMonitoramento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            return configuracaoEmbarcador?.PossuiMonitoramento ?? false;
        }

        protected void SalvarPendenciasFila(string path, string prefixFilename, DateTime date, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> pendings)
        {
            int total = pendings?.Count ?? 0;
            if (total > 0 && !string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(prefixFilename))
            {
                string filename = Utilidades.IO.FileStorageService.Storage.Combine(path, prefixFilename + "-" + date.ToString("yyyyMMdd-HHmmss-ffffff") + ".json");
                string content = JsonConvert.SerializeObject(pendings);
                Utilidades.IO.FileStorageService.Storage.WriteAllText(filename, content);
            }
        }

        protected List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo> BuscarPendenciasFila(string path, int quantidadeRegistros = 0, List<string> arquivosNaoProcessar = null)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo> arquivosPendencias = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo>();

            Log("BuscarPendenciasFila", 6);
            DateTime inicio = DateTime.UtcNow, inicio1 = DateTime.UtcNow;

            // Lista de arquivos com as pendências
            List<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(path).ToList();
            int totalArquivos = arquivos.Count;
            Log($"BuscarPendencias com {totalArquivos} arquivos pendentes", inicio1, 6);

            if (totalArquivos > 0)
            {
                int totalPendencias = 0;

                // Ordena pelo nome para buscar os mais antigos primeiro
                arquivos.Sort();
                for (int i = 0; i < totalArquivos; i++)
                {
                    if (Utilidades.IO.FileStorageService.Storage.Exists(arquivos[i]))
                    {
                        // Se o arquivo já está na lista de arquivos para não processar, pula.
                        if (arquivosNaoProcessar != null && arquivosNaoProcessar.Exists(arquivo => arquivo == arquivos[i])) continue;

                        // Limite de pendências a processar
                        if (quantidadeRegistros == 0 || totalPendencias < quantidadeRegistros)
                        {

                            // Lê e processa o conteúdo do arquivo com as pendências
                            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> pendencias = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia>();
                            string content = LerConteudoArquivoAguardandoAcesso(arquivos[i], 5);
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                pendencias = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia>>(content);
                                totalPendencias += pendencias.Count();
                            }

                            arquivosPendencias.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo
                            {
                                NomeArquivo = arquivos[i],
                                CaminhoArquivo = arquivos[i],
                                Pendencias = pendencias
                            });
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            Log($"{arquivosPendencias.Count} arquivos de pendências carregados", inicio, 7);
            return arquivosPendencias;
        }

        protected int ContarPendenciasFila(string path)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo> arquivosPendencias = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo>();

            Log("ContarPendenciasFila", 6);
            DateTime inicio = DateTime.UtcNow;

            // Lista de arquivos com as pendências
            List<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(path).ToList();
            int totalArquivos = arquivos.Count;
            Log($"ContarPendenciasFila com {totalArquivos} arquivos pendentes", inicio, 6);
            return totalArquivos;
        }

        protected void CarregarNivelLogDoArquivo(string arquivo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(arquivo) && Utilidades.IO.FileStorageService.Storage.Exists(arquivo)) nivelLog = Int32.Parse(Utilidades.IO.FileStorageService.Storage.ReadAllText(arquivo));
            }
            catch
            {

            }
        }

        protected void Log(string mensagem, DateTime inicio, int level = 0, bool newLine = true)
        {
            if (level > nivelLog) return;
            Log(mensagem + " " + (DateTime.UtcNow - inicio).ToString(@"hh\:mm\:ss\,fff"), level, newLine);
        }

        protected void LogNomeArquivo(string mensagem, DateTime inicio, string nomeArquivo, int level = 0, bool newLine = true)
        {
            if (level > nivelLog) return;
            LogArquivo(mensagem + " " + (DateTime.UtcNow - inicio).ToString(@"hh\:mm\:ss\,fff"), level, newLine, nomeArquivo);
        }

        protected void Log(string mensagem, int level = 0, bool newLine = true)
        {
            if (level > nivelLog) return;

            string msg = "";
            if (level > 0) msg += "".PadLeft(level, ' ');
            msg += mensagem;
            if (newLine) msg += this.strNewLine;
            WriteLog(msg);
        }

        protected void LogArquivo(string mensagem, int level = 0, bool newLine = true, string nomeArquivo = "")
        {
            if (level > nivelLog) return;

            string msg = "";
            if (level > 0) msg += "".PadLeft(level, ' ');
            msg += mensagem;
            if (newLine) msg += this.strNewLine;
            WriteLogArquivo(msg, nomeArquivo);
        }

        protected void LogErro(string mensagem, int level = 0, bool newLine = true)
        {
            Log("Erro: " + mensagem, level, newLine);
        }

        protected void LogNewLine()
        {
            Servicos.Embarcador.Monitoramento.MonitoramentoUtils.GravarLogTracking(this.strNewLine, this.GetType().Name, false);
        }

        #endregion

        #region Métodos privados

        private string LerConteudoArquivoAguardandoAcesso(string filename, byte timeoutSeconds)
        {
            TimeSpan sleep = TimeSpan.FromMilliseconds(1);
            TimeSpan timeout = TimeSpan.FromSeconds(timeoutSeconds);
            DateTime inicio = DateTime.UtcNow;
            do
            {
                try
                {
                    string content = Utilidades.IO.FileStorageService.Storage.ReadAllText(filename);
                    return content;
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(sleep);
                }
            } while (DateTime.UtcNow - inicio < timeout);
            throw new Exception($"Acesso ao arquivo \"{filename}\" não permitido pois esta sendo usado por outro processo.");
        }

        private void WriteLog(string msg)
        {
            Servicos.Embarcador.Monitoramento.MonitoramentoUtils.GravarLogTracking(msg, this.GetType().Name);
        }

        private void WriteLogArquivo(string msg, string nomeArquivo)
        {
            Servicos.Embarcador.Monitoramento.MonitoramentoUtils.GravarLogTracking(msg, nomeArquivo);
        }

        #endregion

    }
}