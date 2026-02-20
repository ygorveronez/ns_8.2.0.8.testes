using Dominio.Excecoes.Embarcador;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Servicos.Embarcador.ReconhecimentoFacial
{
    public sealed class ReconhecimentoFacial
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string SUBSCRIPTION_KEY;
        private readonly string ENDPOINT;
        
        public ReconhecimentoFacial(string endpoint, string key, Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SUBSCRIPTION_KEY = key;
            ENDPOINT = endpoint;
        }
        
        public async Task<bool> AutenticarAsync(string imagemBase64, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Stream streamImagem = ObterStreamImagem(imagemBase64);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                 | SecurityProtocolType.Tls11
                                                 | SecurityProtocolType.Tls12
                                                 | SecurityProtocolType.Ssl3;

            IFaceClient cliente = new FaceClient(new ApiKeyServiceClientCredentials(SUBSCRIPTION_KEY)) { Endpoint = ENDPOINT };
            
            IList<DetectedFace> faces = await cliente.Face.DetectWithStreamAsync(streamImagem, recognitionModel: "recognition_04", detectionModel: "detection_03");
            
            IList<Guid?> fotosAlvos = await ObterFotosAlvo(cliente, codigoCarga, unitOfWork);
            
            IList<SimilarFace> similar = await cliente.Face.FindSimilarAsync(faces[0].FaceId.Value, null, null, fotosAlvos);
            
            return similar.Any(f => f.Confidence > 0.9);
        }
        
        private async Task<IList<Guid?>> ObterFotosAlvo(IFaceClient cliente, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            IList<Guid?> fotosAlvos = new List<Guid?>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristasCarga = ObterMotoristasCarga(codigoCarga);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in motoristasCarga)
            {
                string base64Motorista = ObterFotoMotorista(cargaMotorista.Motorista, unitOfWork);

                if (string.IsNullOrWhiteSpace(base64Motorista))
                    throw new ServicoException("Motorista sem imagem cadastrada na base de dados.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.MotoristaSemImagemCadastrada);

                Stream streamImagemMotorista = ObterStreamImagem(base64Motorista);
                List<DetectedFace> rostosMotorista = await ObterFacesImagemMotorista(cliente, streamImagemMotorista);

                fotosAlvos.Add(rostosMotorista[0].FaceId.Value);
            }

            return fotosAlvos;
        }

        private Stream ObterStreamImagem(string imagemBase64)
        {
            byte[] bytes = Convert.FromBase64String(imagemBase64);
            return new MemoryStream(bytes);
        }

        private string ObterFotoMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" });
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{motorista.Codigo}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> ObterMotoristasCarga(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristasCarga = repositorioCargaMotorista.BuscarPorCarga(codigoCarga);

            return motoristasCarga;
        }

        private async Task<List<DetectedFace>> ObterFacesImagemMotorista(IFaceClient cliente, Stream stream)
        {
            IList<DetectedFace> detectedFaces = await cliente.Face.DetectWithStreamAsync(stream, recognitionModel: "recognition_04", detectionModel: "detection_03");
            List<DetectedFace> sufficientQualityFaces = new List<DetectedFace>();
            
            foreach (DetectedFace detectedFace in detectedFaces)
                sufficientQualityFaces.Add(detectedFace);
            
            return sufficientQualityFaces.ToList();
        }
    }
}
