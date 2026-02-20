using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Pedido
{
    public sealed class ColetaContainerAnexo
    {

        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        public ColetaContainerAnexo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        #region Métodos Privados

        private string ObterCaminhoArquivos()
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo).Name });
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public byte[] DownloadAnexo(Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo anexo)
        {
            string caminho = ObterCaminhoArquivos();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}.{anexo.ExtensaoArquivo}");
            byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

            return arquivoBinario;
        }

        public System.IO.MemoryStream DownloadAnexos(List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> anexos)
        {
            System.IO.MemoryStream fZip = new System.IO.MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);

            zipOStream.SetLevel(9);

            foreach (Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo anexo in anexos)
            {
                byte[] arquivoBinario = DownloadAnexo(anexo);

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

        public void ExcluirAnexo(Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo anexo)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic repositorioRic = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic(_unitOfWork);
            string caminho = ObterCaminhoArquivos();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}.{anexo.ExtensaoArquivo}");

            if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

            //if (_auditado != null)
            //    Auditoria.Auditoria.Auditar(anexo, null, $"Removeu o arquivo {anexo.NomeArquivo}.", unidadeTrabalho);
            if(anexo.ColetaContainerAnexoRic != null)
                repositorioRic.Deletar(anexo.ColetaContainerAnexoRic);
            
            repositorioColetaContainerAnexo.Deletar(anexo);
        }

        public void ExcluirAnexosPorColetaContainerECarga(Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer, int codigoCarga)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> anexos = repositorioColetaContainerAnexo.BuscarPorColetaContainerECarga(coletaContainer.Codigo, codigoCarga);

            foreach (Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo anexo in anexos)
                ExcluirAnexo(anexo);
        }

        #endregion Métodos Públicos
    }
}
