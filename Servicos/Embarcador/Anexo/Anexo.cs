using Dominio.Excecoes.Embarcador;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace Servicos.Embarcador.Anexo
{
    public class Anexo<TAnexo, TEntidadeAnexo>
        where TAnexo : Dominio.Entidades.Embarcador.Anexo.Anexo<TEntidadeAnexo>, new()
        where TEntidadeAnexo : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public Anexo(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public Anexo(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        public string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return $"{Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(TEntidadeAnexo).Name })}";
        }

        private bool IsPermitirAdicionarAnexo(TEntidadeAnexo entidade)
        {
            return true;
        }

        private bool IsPermitirExcluirAnexo(TEntidadeAnexo entidade)
        {
            return true;
        }

        private void PreecherInformacoesAdicionais(TAnexo anexo, Repositorio.UnitOfWork unitOfWork)
        {

        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public byte[] DownloadAnexo(TAnexo anexo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivos(unitOfWork);
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}.{anexo.ExtensaoArquivo}");
            byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

            return arquivoBinario;
        }

        public System.IO.MemoryStream DownloadAnexos(List<TAnexo> anexos, Repositorio.UnitOfWork unitOfWork)
        {
            System.IO.MemoryStream fZip = new System.IO.MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);

            zipOStream.SetLevel(9);

            foreach (TAnexo anexo in anexos)
            {
                byte[] arquivoBinario = DownloadAnexo(anexo, unitOfWork);

                ZipEntry entry = new ZipEntry(string.Concat(anexo.NomeArquivo))
                {
                    DateTime = DateTime.Now
                };

                zipOStream.PutNextEntry(entry);
                zipOStream.Write(arquivoBinario, 0, arquivoBinario.Length);
                zipOStream.CloseEntry();
            }

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public void ExcluirAnexo(TAnexo anexo)
        {
            Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(_unitOfWork);
            string caminho = ObterCaminhoArquivos(_unitOfWork);
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}.{anexo.ExtensaoArquivo}");

            if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, anexo.EntidadeAnexo, null, $"Removeu o arquivo {anexo.NomeArquivo}.", _unitOfWork);

            repositorioAnexo.Deletar(anexo);
        }

        public void ExcluirAnexos(TEntidadeAnexo entidadeAnexo)
        {
            Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(_unitOfWork);
            List<TAnexo> anexos = repositorioAnexo.BuscarPorEntidade(entidadeAnexo.Codigo);

            foreach (TAnexo anexo in anexos)
                ExcluirAnexo(anexo);
        }

        public void TrocarAnexos(TEntidadeAnexo entidadeAnexoAtual, TEntidadeAnexo entidadeAnexoNova)
        {
            Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(_unitOfWork);
            List<TAnexo> anexos = repositorioAnexo.BuscarPorEntidade(entidadeAnexoAtual.Codigo);
            foreach (TAnexo anexo in anexos)
            {
                anexo.EntidadeAnexo = entidadeAnexoNova;
                repositorioAnexo.Atualizar(anexo);
            }
        }

        public void AnexarArquivos(int codigo, IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexos)
        {
            try
            {
                Repositorio.RepositorioBase<TEntidadeAnexo> repositorioEntidade = new Repositorio.RepositorioBase<TEntidadeAnexo>(_unitOfWork);
                TEntidadeAnexo entidade = repositorioEntidade.BuscarPorCodigo(codigo, auditavel: false);

                if (entidade == null)
                    throw new ServicoException("Não foi possível encontrar o registro.");

                if (!IsPermitirAdicionarAnexo(entidade))
                    throw new ServicoException("Situação não permite adicionar arquivos.");

                if (anexos.Count <= 0)
                    throw new ServicoException("Nenhum arquivo selecionado para envio.");

                Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(_unitOfWork);
                string caminho = ObterCaminhoArquivos(_unitOfWork);

                for (int i = 0; i < anexos.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo arquivo = anexos[i];
                    string extensaoArquivo = System.IO.Path.GetExtension(arquivo.Nome).ToLower();
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string nomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.Nome)));
                    string descricao = arquivo.Descricao ?? string.Empty;

                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"), arquivo.Arquivo);

                    TAnexo anexo = new TAnexo()
                    {
                        EntidadeAnexo = entidade,
                        Descricao = descricao,
                        GuidArquivo = guidArquivo,
                        NomeArquivo = nomeArquivo
                    };

                    PreecherInformacoesAdicionais(anexo, _unitOfWork);

                    repositorioAnexo.Inserir(anexo);
                }

                _unitOfWork.CommitChanges();
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos
    }
}
