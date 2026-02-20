using Dominio.Excecoes.Embarcador;
using Servicos.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class OcorrenciaColetaEntregaAnexo
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        public OcorrenciaColetaEntregaAnexo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AdicionarAnexos(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrencia, IEnumerable<CustomFile> arquivos, Repositorio.UnitOfWork unitOfWork)
        {
            if (ocorrencia == null)
                return;

            if (arquivos == null || arquivos.Count() == 0)
                return;

            try
            {

                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(_unitOfWork);
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" });
                for (int i = 0; i < arquivos.Count(); i++)
                {
                    CustomFile arquivo = arquivos.ElementAt(i);
                    string extensaoArquivo = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                    string nomeArquivoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}.{extensaoArquivo.Replace(".", "")}");

                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(nomeArquivoCompleto, arquivo.GetBytes());

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo anexo = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo()
                    {
                        EntidadeAnexo = ocorrencia,
                        Descricao = string.Empty,
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.FileName)))
                    };

                    repAnexo.Inserir(anexo);
                }
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}