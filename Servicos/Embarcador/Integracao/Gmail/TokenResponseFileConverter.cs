using Microsoft.AspNetCore.Http;
using Servicos.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Gmail
{
    public class TokenResponseFileConverter
    {
        public static string EncodeFileToBase64(Servicos.DTO.CustomFile file)
        {
            if (file == null || file.Length == 0 || file.InputStream == null)
                throw new ArgumentException("Arquivo inválido.");

            byte[] fileBytes = file.GetBytes();
            return Convert.ToBase64String(fileBytes);
        }

        public static string EncodeFileToBase64(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo de token não encontrado.", filePath);

            byte[] fileBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }

        public static string DecodeBase64ToFile(string base64String, string? outputDir = null)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                throw new ArgumentException("Base64 inválido.");

            outputDir ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "GoogleTokenStoreTemp");
            Directory.CreateDirectory(outputDir);

            string tokenFileName = "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user";
            string outputFilePath = Path.Combine(outputDir, tokenFileName);

            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            byte[] bytes = Convert.FromBase64String(base64String);
            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(outputFilePath, bytes);

            return outputFilePath;
        }
    }
}
