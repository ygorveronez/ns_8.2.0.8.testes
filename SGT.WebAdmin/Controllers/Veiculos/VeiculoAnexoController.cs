using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;


namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize(new string[] { "DownloadAnexo" }, "Veiculos/Veiculo")]
    public class VeiculoAnexoController : BaseController
    {
		#region Construtores

		public VeiculoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Veiculos.VeiculoAnexo repVeiculoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoAnexo(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("Veiculo"), out int codigo);

                if (!this.InsereArquivos(codigo, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo> anexos = repVeiculoAnexo.BuscarPorCodigoVeiculo(codigo);

                var dynAnexos = (from obj in anexos
                                 select new
                                 {
                                     obj.Codigo,
                                     obj.Descricao,
                                     obj.NomeArquivo,
                                    TipoAnexoVeiculo = obj.TipoAnexoVeiculo.ObterDescricao(),
                                 }).ToList();

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo a ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Veiculos.VeiculoAnexo repVeiculoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo VeiculoAnexo = repVeiculoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (VeiculoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(VeiculoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, VeiculoAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", VeiculoAnexo.NomeArquivo);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar anexo.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Veiculos/Veiculo");

                // Repositorios
                Repositorio.Embarcador.Veiculos.VeiculoAnexo repVeiculoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo VeiculoAnexo = repVeiculoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (VeiculoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (!(Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Veiculo_PermiteRemoverAnexos)))
                    return new JsonpResult(false, true, "Você não possui permissão para remover os anexos.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(VeiculoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, VeiculoAnexo.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, VeiculoAnexo.Veiculo, null, "Excluiu o anexo " + VeiculoAnexo.NomeArquivo + ".", unitOfWork);
                repVeiculoAnexo.Deletar(VeiculoAnexo);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Veiculo" });
        }

        private dynamic InsereArquivos(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoAnexo repVeiculoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoAnexo(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            string[] tipo = Request.TryGetArrayParam<string>("TipoAnexoVeiculo");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (veiculo == null)
            {
                erro = "Veículo não localizada para anexar arquivo.";
                return false;
            }

            for (var i = 0; i < arquivos.Count(); i++)
            {
                // Extrai dados
                Servicos.DTO.CustomFile file = arquivos[i];
                var nomeArquivo = file.FileName;
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = this.CaminhoArquivos(unitOfWork);

                // Salva na pasta
                file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                // Insere no banco
                Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo veiculoAnexo = new Dominio.Entidades.Embarcador.Veiculos.VeiculoAnexo
                {
                    Veiculo = veiculo,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    TipoAnexoVeiculo = i < tipo.Length ? tipo[i].ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoVeiculo>() : TipoAnexoVeiculo.Outros,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };

                repVeiculoAnexo.Inserir(veiculoAnexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoAnexo.Veiculo, null, "Adicionou o anexo " + veiculoAnexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }
    }
}
