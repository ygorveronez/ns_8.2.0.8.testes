using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ContratoFreteTransportador")]
    public class ContratoFreteTransportadorAnexoController : BaseController
    {
        #region Construtores

        public ContratoFreteTransportadorAnexoController(Conexao conexao) : base(conexao) { }

        #endregion


        public async Task<IActionResult> PesquisaAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo repContratoFreteTransportadorAnexo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, true);

                // Dados do filtro
                int.TryParse(Request.Params("ContratoFreteTransportador"), out int codigo);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo> anexos = repContratoFreteTransportadorAnexo.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repContratoFreteTransportadorAnexo.ContarConsulta(codigo);
                var lista = from obj in anexos
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.NomeArquivo
                            };

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo repContratoFreteTransportadorAnexo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("ContratoFreteTransportador"), out int codigo);

                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                // Valida
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (contrato == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                // Em alguns casos, o contrato é aprovado automatico, então nesse momento, o contrato esta aprovado
                // Mas como não é possivel modificar anexos enquanto aprovado, é feita essa validação
                bool contratoRecemCriado = codigo == contrato.Codigo;

                if (contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao
                    && contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.SemRegra
                    && contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Novo
                    && (contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado && contratoRecemCriado))
                    return new JsonpResult(false, "Situação do Contrato não permite anexar arquivos.");

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
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo anexo = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo
                    {
                        ContratoFreteTransportador = contrato,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                    };

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, "Adicionou o arquivo " + anexo.NomeArquivo + ".", unitOfWork);
                    repContratoFreteTransportadorAnexo.Inserir(anexo, Auditado);
                }

                // Commita
                unitOfWork.CommitChanges();

                // Busca todos anexos
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo> anexos = repContratoFreteTransportadorAnexo.BuscarPorContrato(contrato.Codigo);

                // Retorna arquivos
                var dynAnexos = from obj in anexos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Descricao,
                                    obj.NomeArquivo
                                };

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo repContratoFreteTransportadorAnexo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo anexo = repContratoFreteTransportadorAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
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
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo repContratoFreteTransportadorAnexo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo anexos = repContratoFreteTransportadorAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.ContratoFreteTransportador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao
                    && anexos.ContratoFreteTransportador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.SemRegra
                    && anexos.ContratoFreteTransportador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Novo)
                    return new JsonpResult(false, "Situação do Contrato não permite excluir arquivos.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos.ContratoFreteTransportador, null, "Removeu o arquivo " + anexos.NomeArquivo + ".", unitOfWork);
                repContratoFreteTransportadorAnexo.Deletar(anexos, Auditado);

                // Commita
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "ContratoFreteTransportador" });
        }
    }
}
