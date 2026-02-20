using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Servicos.Embarcador.Integracao
{
    public class ArquivoIntegracao
    {
        #region Métodos Privados

        private static string ObterNomeArquivo(string nomeArquivo, string caminhoArquivosIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoArquivo = !string.IsNullOrWhiteSpace(caminhoArquivosIntegracao) ? caminhoArquivosIntegracao : ObterCaminhoArquivoIntegracao(unitOfWork);
            string nomeArquivoAbsoluto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, nomeArquivo);

            return nomeArquivoAbsoluto;
        }

        private static void SalvarArquivo(string conteudoArquivo, string nomeArquivo, string caminhoArquivosIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Utilidades.IO.FileStorageService.Storage.WriteAllText(ObterNomeArquivo(nomeArquivo, caminhoArquivosIntegracao, unitOfWork), conteudoArquivo);
        }

        private static void SalvarArquivo(byte[] conteudoArquivo, string nomeArquivo, string caminhoArquivosIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(ObterNomeArquivo(nomeArquivo, caminhoArquivosIntegracao, unitOfWork), conteudoArquivo);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public static byte[] CriarZip(List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> arquivos)
        {
            string caminhoArquivos = ObterCaminhoArquivoIntegracao();

            return CriarZip(arquivos, caminhoArquivos);
        }

        public static byte[] CriarZip(List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> arquivos, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoArquivos = ObterCaminhoArquivoIntegracao(unitOfWork);

            return CriarZip(arquivos, caminhoArquivos);
        }

        public static byte[] CriarZip(List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> arquivos, string caminhoArquivos)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivo in arquivos)
                    {
                        if (arquivo == null)
                            continue;

                        string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, arquivo.NomeArquivo);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                        {
                            ZipArchiveEntry file = archive.CreateEntry(arquivo.NomeArquivo);

                            using (Stream entryStream = file.Open())
                            using (StreamWriter streamWriter = new StreamWriter(entryStream))
                            {
                                ExtensaoArquivo extensaoArquivo = ExtensaoArquivoHelper.ObterExtensaoArquivo(arquivo.NomeArquivo);

                                if (extensaoArquivo.IsArquivoBinario())
                                {
                                    byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivo);

                                    streamWriter.BaseStream.Write(arquivoBinario, 0, arquivoBinario.Length);
                                }
                                else
                                    streamWriter.Write(Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoArquivo));
                            }
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream.ToArray();
            }
        }

        public static string RetornarArquivoTexto(Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivo)
        {
            string caminhoArquivos = ObterCaminhoArquivoIntegracao();

            if (arquivo == null)
                return "";

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, arquivo.NomeArquivo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                return Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoArquivo);
            
            return "";
        }

        public static string ObterCaminhoArquivoIntegracao(Repositorio.UnitOfWork unitOfWork = null)
        {
            string caminhoArquivo = null;
            if (unitOfWork != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo arquivo = repArquivo.BuscarPrimeiroRegistro();

                if (arquivo != null)
                    caminhoArquivo = arquivo.CaminhoArquivosIntegracao;

            }
            else
                caminhoArquivo = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

#if DEBUG
            caminhoArquivo = Servicos.FS.GetPath(@"C:\Arquivos");
#endif

            return caminhoArquivo;
        }

        public static Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao SalvarArquivoIntegracao(string nomeArquivo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Integracao.ArquivoIntegracao repositorioArquivoIntegracao = new Repositorio.Embarcador.Integracao.ArquivoIntegracao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = new Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao()
            {
                NomeArquivo = nomeArquivo
            };

            repositorioArquivoIntegracao.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        public static Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao SalvarArquivoIntegracao(string conteudoArquivo, string extensao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            return SalvarArquivoIntegracao(conteudoArquivo, Guid.NewGuid().ToString(), extensao, unidadeDeTrabalho, caminhoArquivosIntegracao: null);
        }

        public static Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao SalvarArquivoIntegracao(string conteudoArquivo, string extensao, Repositorio.UnitOfWork unidadeDeTrabalho, string caminhoArquivosIntegracao)
        {
            return SalvarArquivoIntegracao(conteudoArquivo, Guid.NewGuid().ToString(), extensao, unidadeDeTrabalho, caminhoArquivosIntegracao);
        }

        public static Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao SalvarArquivoIntegracao(string conteudoArquivo, string nome, string extensao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            return SalvarArquivoIntegracao(conteudoArquivo, nome, extensao, unidadeDeTrabalho, caminhoArquivosIntegracao: null);
        }

        public static Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao SalvarArquivoIntegracao(string conteudoArquivo, string nome, string extensao, Repositorio.UnitOfWork unidadeDeTrabalho, string caminhoArquivosIntegracao)
        {
            string nomeArquivo = $"{nome}{(extensao.Contains(".") ? extensao : "." + extensao)}";

            SalvarArquivo(conteudoArquivo, nomeArquivo, caminhoArquivosIntegracao, unidadeDeTrabalho);

            return SalvarArquivoIntegracao(nomeArquivo, unidadeDeTrabalho);
        }

        public static Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao SalvarArquivoIntegracao(byte[] conteudoArquivo, string extensao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            string nomeArquivo = $"{Guid.NewGuid().ToString()}{(extensao.Contains(".") ? extensao : "." + extensao)}";

            SalvarArquivo(conteudoArquivo, nomeArquivo, caminhoArquivosIntegracao: null, unidadeDeTrabalho);

            return SalvarArquivoIntegracao(nomeArquivo, unidadeDeTrabalho);
        }

        #endregion Métodos Públicos
    }
}
