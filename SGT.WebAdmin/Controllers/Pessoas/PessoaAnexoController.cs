using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/Pessoa")]
    public class PessoaAnexoController : BaseController
    {
        #region Construtores

        public PessoaAnexoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                double cnpjCpf = Request.GetDoubleParam("Codigo");
                Repositorio.Cliente repositorioEntidade = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente entidade = repositorioEntidade.BuscarPorCPFCNPJ(cnpjCpf);

                if (entidade == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelEncontrarRegistro);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NenhumArquivoSelecionadoParaEnvio);

                Repositorio.Embarcador.Pessoas.PessoaAnexo repositorioAnexo = new Repositorio.Embarcador.Pessoas.PessoaAnexo(unitOfWork);
                string caminho = ObterCaminhoArquivos(unitOfWork);

                for (int i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile arquivo = arquivos[i];
                    string extensaoArquivo = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
                    string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                    arquivo.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"));

                    Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo anexo = new Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo()
                    {
                        Pessoa = entidade,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.FileName)))
                    };

                    repositorioAnexo.Inserir(anexo, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, entidade, null, Localization.Resources.Pessoas.Pessoa.AdicionouArquivo + anexo.NomeArquivo + ".", unitOfWork);
                }

                List<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo> anexos = repositorioAnexo.BuscarPorPessoa(entidade.CPF_CNPJ);

                var listaDinamicaAnexos = (
                    from anexo in anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo
                    }
                ).ToList();

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoAnexarOsArquivos);
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.PessoaAnexo repositorioAnexo = new Repositorio.Embarcador.Pessoas.PessoaAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelEncontrarRegistro);

                string caminho = ObterCaminhoArquivos(unitOfWork);
                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}{extensao}");
                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.Pessoa, null, Localization.Resources.Pessoas.Pessoa.RealizouDownloadDoArquivo + anexo.NomeArquivo + ".", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelEncontrarAnexo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoFazerDownloadDoAnexo);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.PessoaAnexo repositorioAnexo = new Repositorio.Embarcador.Pessoas.PessoaAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo);

                if (anexo == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelEncontrarRegistro);

                string caminho = ObterCaminhoArquivos(unitOfWork);
                string extensaoArquivo = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}{extensaoArquivo}");
                double cnpjPessoa = anexo.Pessoa.CPF_CNPJ;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Pessoa.NaoFoiPossivelEncontrarAnexo);
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.Pessoa, null, Localization.Resources.Pessoas.Pessoa.RemoveuArquivo + anexo.NomeArquivo + ".", unitOfWork);
                repositorioAnexo.Deletar(anexo, Auditado);

                List<Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo> anexos = repositorioAnexo.BuscarPorPessoa(cnpjPessoa);

                var listaDinamicaAnexos = (
                    from anexoC in anexos
                    select new
                    {
                        anexoC.Codigo,
                        anexoC.Descricao,
                        anexoC.NomeArquivo
                    }
                ).ToList();

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });

                //unitOfWork.CommitChanges();

                //return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoDeletarAnexo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos

        protected string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, Localization.Resources.Pessoas.Pessoa.Anexos, typeof(Dominio.Entidades.Embarcador.Pessoas.PessoaAnexo).Name });
        }

        #endregion
    }
}
