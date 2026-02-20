using System.Diagnostics;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Disponibilidade" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Disponibilidade.svc or Disponibilidade.svc.cs at the Solution Explorer and start debugging.
    public class Disponibilidade : IDisponibilidade
    {
        public Retorno<bool> Testar()
        {
            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<string> Versao()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart:00}";
            if (fvi.FilePrivatePart > 0)
                version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart:00}.{fvi.FileBuildPart}.{fvi.FilePrivatePart:00}";

            return Retorno<string>.CriarRetornoSucesso(version);
        }
    }
}