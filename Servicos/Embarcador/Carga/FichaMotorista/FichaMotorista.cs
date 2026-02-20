using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class FichaMotorista
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public FichaMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public byte[] ObterFichaMotoristaMergePDFs(byte[] pdf, int codigoMotorista, int codigoVeiculo, List<int> codigosReboques)
        {
            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            List<byte[]> arquivos = new List<byte[]>();

            if (pdf != null)
                arquivos.Add(pdf);

            Repositorio.Embarcador.Veiculos.VeiculoAnexo repVeiculoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoAnexo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo> veiculoAnexos = new List<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo>();

            for (int i = 0; i < codigosReboques.Count(); i++)
                veiculoAnexos = repVeiculoAnexo.BuscarCRLVPorCodigosVeiculo(codigosReboques);

            for (int i = 0; i < veiculoAnexos.Count(); i++)
            {
                string caminhoVeiculo = this.CaminhoArquivosVeiculo(_unitOfWork);
                string extensaoVeiculo = System.IO.Path.GetExtension(veiculoAnexos[i].NomeArquivo).ToLower();
                string arquivoVeiculo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoVeiculo, veiculoAnexos[i].GuidArquivo + extensaoVeiculo);

                byte[] bArquivoVeiculo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoVeiculo);
                if (bArquivoVeiculo != null)
                    arquivos.Add(bArquivoVeiculo);
            }

            Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo veiculoAnexo = codigoVeiculo > 0 ? repVeiculoAnexo.BuscarCRLVPorCodigoVeiculo(codigoVeiculo) : null;

            if (veiculoAnexo != null)
            {
                string caminhoVeiculo = this.CaminhoArquivosVeiculo(_unitOfWork);
                string extensaoVeiculo = System.IO.Path.GetExtension(veiculoAnexo.NomeArquivo).ToLower();
                string arquivoVeiculo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoVeiculo, veiculoAnexo.GuidArquivo + extensaoVeiculo);

                byte[] bArquivoVeiculo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoVeiculo);
                if (bArquivoVeiculo != null)
                    arquivos.Add(bArquivoVeiculo);
            }

            Repositorio.Embarcador.Usuarios.FuncionarioAnexo repMotoristaAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> funcionarioAnexos = repMotoristaAnexo.BuscarAnexosParaFichaMotorista(codigoMotorista);

            foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo funcionarioAnexo in funcionarioAnexos)
            {
                string caminhoMotorista = this.CaminhoArquivosMotorista(_unitOfWork);
                string extensaoMotorista = System.IO.Path.GetExtension(funcionarioAnexo.NomeArquivo).ToLower();
                string arquivoMotorista = Utilidades.IO.FileStorageService.Storage.Combine(caminhoMotorista, funcionarioAnexo.GuidArquivo + extensaoMotorista);

                byte[] bArquivoMotorista = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoMotorista);
                if (bArquivoMotorista != null)
                    arquivos.Add(bArquivoMotorista);
            }

            return Utilidades.File.MergeFiles(arquivos);
        }

        #endregion

        #region Métodos Privados

        private string CaminhoArquivosVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Veiculo");

            return caminho;
        }

        private string CaminhoArquivosMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Funcionario");

            return caminho;
        }

        #endregion
    }
}
