namespace Utilidades
{
    public class Directory
    {
        public static string CriarCaminhoArquivos(string[] sufix)
        {
            string caminho = "";
            for (var i = 0; i < sufix.Length; i++)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, sufix[i]);

            return caminho.EndsWith("\\") ? caminho : caminho += "\\";
        }        
    }
}
