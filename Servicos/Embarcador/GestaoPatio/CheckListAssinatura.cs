using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;

namespace Servicos.Embarcador.GestaoPatio
{
    public class CheckListAssinatura
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _caminhoBase;

        #endregion Atributos Privados Somente Leitura

        #region Construtores

        public CheckListAssinatura(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _caminhoBase = ObterCaminhoArquivoAssinatura();
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ArmazenarVinculoAssinatura(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, string assinatura, TipoAssinaturaCheckListCarga tipoAssinatura)
        {
            ArmazenarArquivoAssinatura(assinatura, out string guid);
            SalvarVinculoAssinatura(checklist, guid, tipoAssinatura);
        }

        public void CopiarAssinatura(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinaturaAntiga)
        {
            string assinaturaBase64 = ObterBase64(assinaturaAntiga.GuidArquivo);

            if (!string.IsNullOrWhiteSpace(assinaturaBase64))
                ArmazenarVinculoAssinatura(checklist, assinaturaBase64, assinaturaAntiga.TipoAssinatura);
        }

        public void DeletarAssinaturas(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura repositorioChecklistAssinatura = new Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura> assinaturas = repositorioChecklistAssinatura.BuscarPorCheckList(checklist.Codigo);

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinatura in assinaturas)
            {
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(_caminhoBase, $"{assinatura.GuidArquivo}.png");

                if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                repositorioChecklistAssinatura.Deletar(assinatura);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void ArmazenarArquivoAssinatura(string assinatura, out string guid)
        {
            string extensao = ".png";

            if (assinatura.Contains(","))
                assinatura = assinatura.Split(',')[1];

            byte[] data = Convert.FromBase64String(assinatura);

            string token = Guid.NewGuid().ToString().Replace("-", "");
            guid = token;

            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(_caminhoBase, guid + extensao);

            using (MemoryStream ms = new MemoryStream(data))
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
            {
                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, image);
            }
        }

        private void SalvarVinculoAssinatura(Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, string guid, TipoAssinaturaCheckListCarga tipoAssinatura)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura repositorioChecklistAssinatura = new Repositorio.Embarcador.GestaoPatio.CheckListCargaAssinatura(_unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura assinatura = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaAssinatura
            {
                GuidArquivo = guid,
                NomeArquivo = tipoAssinatura.ObterNomeArquivo(),
                TipoAssinatura = tipoAssinatura,
                CheckList = checklist
            };

            repositorioChecklistAssinatura.Inserir(assinatura);
        }

        private string ObterBase64(string guidArquivo)
        {
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(_caminhoBase, $"{guidArquivo}.png");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private string ObterCaminhoArquivoAssinatura()
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CheckListCarga", "Assinaturas" });
        }

        #endregion Métodos Privados
    }
}
