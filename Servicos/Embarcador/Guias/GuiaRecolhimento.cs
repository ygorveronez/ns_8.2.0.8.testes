using System.IO;

namespace Servicos.Embarcador.Guias
{
    public class GuiaRecolhimento : ServicoBase
    {
        public GuiaRecolhimento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public static string CaminhoGuia(Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia, Repositorio.UnitOfWork unitOfWork = null)
        {
            string caminho;
            string path = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoGuia;

            if (string.IsNullOrEmpty(path) && unitOfWork != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoArquivo.BuscarPrimeiroRegistro();

                if (configuracaoArquivo != null)
                    path = configuracaoArquivo.Anexos;
            }


            caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, guia.NroGuia);

            return caminho;
        }

    }
}
