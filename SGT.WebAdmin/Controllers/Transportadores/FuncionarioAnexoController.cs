using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "DownloadAnexo" }, "Transportadores/Motorista", "Pessoas/Usuario")]
    public class FuncionarioAnexoController : BaseController
    {
        #region Construtores

        public FuncionarioAnexoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.FuncionarioAnexo repFuncionarioAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(unitOfWork);

                int codigo = Request.GetIntParam("Funcionario");

                if (!InsereArquivos(codigo, unitOfWork, out string erro))
                    return new JsonpResult(false, true, erro);

                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> anexos = repFuncionarioAnexo.BuscarPorCodigoUsuario(codigo);

                var dynAnexos = (from obj in anexos
                                 select new
                                 {
                                     obj.Codigo,
                                     NomeTela = obj.Usuario.Tipo == "M" ? "Motorista" : "Usuario",
                                     obj.Descricao,
                                     obj.NomeArquivo,
                                     TipoAnexoMotorista = obj.TipoAnexoMotorista.ObterDescricao(),
                                     ImprimirNaFicha = obj.ImprimeNaFichaMotorista ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao
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
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAnexarArquivo);
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
                Repositorio.Embarcador.Usuarios.FuncionarioAnexo repFuncionarioAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo FuncionarioAnexo = repFuncionarioAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (FuncionarioAnexo == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ErroAoBuscarOsDados);

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(FuncionarioAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, FuncionarioAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", FuncionarioAnexo.NomeArquivo);
                else
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarAnexo);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoFazerDownloadDoAnexo);
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
                string nomeTela = Request.Params("NomeTela");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/Usuario");
                if (!string.IsNullOrEmpty(nomeTela) && nomeTela.Equals("Motorista"))
                {
                    permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Motorista");
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Motorista_PermiteRemoverAnexos))
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.VoceNaoPossuiPermissaoParaRemoverOsAnexos);
                }
                else
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Usuario_PermiteRemoverAnexos))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.VoceNaoPossuiPermissaoParaRemoverOsAnexos);

                // Repositorios
                Repositorio.Embarcador.Usuarios.FuncionarioAnexo repFuncionarioAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo FuncionarioAnexo = repFuncionarioAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (FuncionarioAnexo == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ErroAoBuscarOsDados);

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(FuncionarioAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, FuncionarioAnexo.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ErroAoDeletarAnexo);
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, FuncionarioAnexo.Usuario, null, Localization.Resources.Transportadores.Transportador.ExcluiuAnexo + FuncionarioAnexo.NomeArquivo + ".", unitOfWork);
                repFuncionarioAnexo.Deletar(FuncionarioAnexo);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoFazerDownloadDoAnexo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Funcionario" });
        }

        private dynamic InsereArquivos(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Usuarios.FuncionarioAnexo repFuncionarioAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(unitOfWork);

            Dominio.Entidades.Usuario funcionario = repFuncionario.BuscarPorCodigo(codigo);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            string[] tipo = Request.TryGetArrayParam<string>("TipoAnexoMotorista");
            string[] imprimirNaFicha = Request.TryGetArrayParam<string>("ImprimirNaFicha") ?? new string[] { "false" };

            if (arquivos.Count <= 0)
            {
                erro = Localization.Resources.Transportadores.Transportador.NenhumArquivoSelecionadoParaEnvio;
                return false;
            }

            if (funcionario == null)
            {
                erro = Localization.Resources.Transportadores.Transportador.FuncionarioNaoLocalizadoParaAnexarArquivo;
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
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo funcionarioAnexo = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo
                {
                    Usuario = funcionario,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    TipoAnexoMotorista = tipo == null ? TipoAnexoMotorista.Outros : i < tipo.Length ? tipo[i].ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoMotorista>() : TipoAnexoMotorista.Outros,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo))),
                    ImprimeNaFichaMotorista = imprimirNaFicha == null ? false : i < imprimirNaFicha.Length ? imprimirNaFicha[i].ToBool() : false,
                };

                repFuncionarioAnexo.Inserir(funcionarioAnexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, funcionarioAnexo.Usuario, null, Localization.Resources.Transportadores.Transportador.AdicionouAnexo + funcionarioAnexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }

        #endregion
    }
}
