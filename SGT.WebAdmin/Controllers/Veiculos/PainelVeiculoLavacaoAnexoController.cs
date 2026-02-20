using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers.Anexo;
using System.IO;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/PainelVeiculo")]
    public class PainelVeiculoLavacaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo, Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao>
    {
        #region Construtores

        public PainelVeiculoLavacaoAnexoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Metódos Públicos

        public async Task<IActionResult> AdicionarLavacaoEAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigoVeiculo = Request.GetIntParam("Codigo");
                Servicos.DTO.CustomFile anexosAntesLavacao = HttpContext.GetFile("AnexoAntesLavacao");
                Servicos.DTO.CustomFile anexosDepoisLavacao = HttpContext.GetFile("AnexoDepoisLavacao");

                if (anexosAntesLavacao == null)
                    return new JsonpResult(false, true, "Nenhum anexo para antes da lavação selecionado para envio.");

                if (anexosDepoisLavacao == null)
                    return new JsonpResult(false, true, "Nenhum anexo para depois da lavação selecionado para envio.");

                Repositorio.Veiculo reposotorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoLavacao repositorioVeiculoLavacao = new Repositorio.Embarcador.Veiculos.VeiculoLavacao(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = reposotorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Log.TratarErro($"Iniciou 'AdicionarLavacaoEAnexos' -> |{codigoVeiculo}|", "VeiculoLavacaoAnexo");

                DateTime dataLavacao = Request.GetDateTimeParam("DataLavacao");

                Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao veiculoLavacao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao
                {
                    DataLavacao = dataLavacao,
                    Veiculo = veiculo,
                    NomeArquivoAntesLavacaoSumarizado = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(anexosAntesLavacao.FileName))),
                    NomeArquivoDepoisLavacaoSumarizado = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(anexosDepoisLavacao.FileName)))
                };

                unitOfWork.Start();

                repositorioVeiculoLavacao.Inserir(veiculoLavacao);

                InserirAnexo(anexosAntesLavacao, veiculoLavacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoLavacaoAnexoTipo.AntesLavacao, unitOfWork);
                InserirAnexo(anexosDepoisLavacao, veiculoLavacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoLavacaoAnexoTipo.DepoisLavacao, unitOfWork);

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao> veiculosLavacao = repositorioVeiculoLavacao.BuscarPorVeiculo(codigoVeiculo);

                var listaDinamica = (
                    from veiculoLavacaoAnexo in veiculosLavacao
                    select new
                    {
                        veiculoLavacaoAnexo.Codigo,
                        DataLavacao = veiculoLavacaoAnexo.DataLavacao.ToString("d"),
                        NomeArquivoAnexoAntesLavacao = veiculoLavacaoAnexo.NomeArquivoAntesLavacaoSumarizado,
                        NomeArquivoAnexoDepoisLavacao = veiculoLavacaoAnexo.NomeArquivoDepoisLavacaoSumarizado,
                        TipoAnexo = new { Codigo = 0, Descricao = string.Empty }
                    }
                ).ToList();

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Anexos = listaDinamica
                });
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirLavacaoEAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Veiculos.VeiculoLavacao repositorioVeiculoLavacao = new Repositorio.Embarcador.Veiculos.VeiculoLavacao(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao veiculoLavacaoveiculo = repositorioVeiculoLavacao.BuscarPorCodigo(codigo);

                if (veiculoLavacaoveiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                ExcluirAnexos(veiculoLavacaoveiculo, unitOfWork);

                repositorioVeiculoLavacao.Deletar(veiculoLavacaoveiculo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadLavacaoAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Veiculos.VeiculoLavacao repositorioVeiculoLavacao = new Repositorio.Embarcador.Veiculos.VeiculoLavacao(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao veiculoLavacaoveiculo = repositorioVeiculoLavacao.BuscarPorCodigo(codigo);

                if (veiculoLavacaoveiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Veiculos.VeiculoLavacaoAnexo repositorioVeiculoLavacaoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoLavacaoAnexo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo> veiculoLavacaoAnexos = repositorioVeiculoLavacaoAnexo.BuscarPorLavacao(veiculoLavacaoveiculo.Codigo);

                if (veiculoLavacaoAnexos == null || veiculoLavacaoAnexos.Count == 0)
                    return new JsonpResult(false, true, "Registros não encontrados");

                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                foreach (var item in veiculoLavacaoAnexos)
                {
                    string caminho = ObterCaminhoArquivos(unitOfWork);
                    string extensao = System.IO.Path.GetExtension(item.NomeArquivo).ToLower();
                    string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, item.GuidArquivo + extensao);
                    string nomeArquivo = item.TipoAnexoLavacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoLavacaoAnexoTipo.AntesLavacao ? item.EntidadeAnexo.NomeArquivoAntesLavacaoDescricao + extensao : item.EntidadeAnexo.NomeArquivoDepoisLavacaoDescricao + extensao; ;
                    if (Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                    {
                        byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo);
                        conteudoCompactar.Add(nomeArquivo, arquivoBinario);
                    }
                }

                if (conteudoCompactar?.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar as imagens para realizar o download.");

                MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

                arquivoCompactado.Dispose();

                if (arquivoCompactadoBinario == null)
                    return new JsonpResult(false, true, "Não foi possível gerar o arquivo.");

                return Arquivo(arquivoCompactadoBinario, "application/zip", $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.zip");

            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metódos Privados

        private void InserirAnexo(Servicos.DTO.CustomFile anexo, Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao veiculoLavacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoLavacaoAnexoTipo tipoAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo, Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo, Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao>(unitOfWork);

            string caminho = ObterCaminhoArquivos(unitOfWork);

            string extensaoArquivo = System.IO.Path.GetExtension(anexo.FileName).ToLower();
            string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}");
            anexo.SaveAs(caminhoCompleto);

            Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo veiculoLavacaoAnexo = new Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo()
            {
                EntidadeAnexo = veiculoLavacao,
                TipoAnexoLavacao = tipoAnexo,
                GuidArquivo = guidArquivo,
                NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(anexo.FileName)))
            };

            Servicos.Log.TratarErro($"Adicionou 'VeiculoLavacaoAnexo' -> |{caminhoCompleto}|", "VeiculoLavacaoAnexo");
            Servicos.Log.TratarErro($"'{guidArquivo}' Existe -> |{Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompleto)}|", "VeiculoLavacaoAnexo");

            repositorioAnexo.Inserir(veiculoLavacaoAnexo, Auditado);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoLavacaoAnexo, null, $"Adicionou o arquivo {veiculoLavacaoAnexo.NomeArquivo}.", unitOfWork);
        }

        private void ExcluirAnexos(Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao veiculoLavacaoveiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoLavacaoAnexo repositorioVeiculoLavacao = new Repositorio.Embarcador.Veiculos.VeiculoLavacaoAnexo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo> veiculoLavacaoAnexos = repositorioVeiculoLavacao.BuscarPorLavacao(veiculoLavacaoveiculo.Codigo);

            Servicos.Log.TratarErro($"Removeu 'VeiculoLavacao' -> |{veiculoLavacaoveiculo.Codigo}|", "VeiculoLavacaoAnexo");

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacaoAnexo veiculoLavacaoAnexo in veiculoLavacaoAnexos)
            {
                string caminho = ObterCaminhoArquivos(unitOfWork);
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{veiculoLavacaoAnexo.GuidArquivo}.{veiculoLavacaoAnexo.ExtensaoArquivo}");

                if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                if (Auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoLavacaoAnexo, null, $"Removeu o arquivo {veiculoLavacaoAnexo.NomeArquivo}.", unitOfWork);

                repositorioVeiculoLavacao.Deletar(veiculoLavacaoAnexo);
            }

        }

        #endregion

    }
}