using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ColaboradorSituacaoLancamento")]
    public class ColaboradorSituacaoLancamentoController : BaseController
    {
        #region Construtores

        public ColaboradorSituacaoLancamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Numero"), out int numero);
                int.TryParse(Request.Params("Colaborador"), out int colaborador);
                int.TryParse(Request.Params("ColaboradorSituacao"), out int colaboradorSituacao);

                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Todos;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Colaborador", "Colaborador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação Colaborador", "ColaboradorSituacao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoLancamento", 10, Models.Grid.Align.center, true);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento> lancamentos = repColaboradorLancamento.Consultar(numero, codigoEmpresa, descricao, colaborador, colaboradorSituacao, situacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repColaboradorLancamento.ContarConsulta(numero, codigoEmpresa, descricao, colaborador, colaboradorSituacao, situacao));

                var lista = (from p in lancamentos
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 p.Descricao,
                                 Colaborador = p.Colaborador?.Nome ?? string.Empty,
                                 ColaboradorSituacao = p.ColaboradorSituacao?.Descricao ?? string.Empty,
                                 SituacaoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaboradorHelper.ObterDescricao(p.SituacaoLancamento)
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento();

                PreencherColaboradorSituacaoLancamento(colaboradorLancamento, unitOfWork);
                repColaboradorLancamento.Inserir(colaboradorLancamento, Auditado);

                Servicos.Embarcador.Transportadores.Motorista.AtualizarStatusColaborador(unitOfWork, Auditado, false, colaboradorLancamento.Colaborador.Codigo);
                Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracaoSituacaoColaborador(unitOfWork, colaboradorLancamento.Codigo);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repColaboradorLancamento.BuscarPorCodigo(codigo, true);

                int diasFolgaAtual = (int)(colaboradorLancamento.DataFinal - colaboradorLancamento.DataInicial).TotalDays;

                PreencherColaboradorSituacaoLancamento(colaboradorLancamento, unitOfWork);
                repColaboradorLancamento.Atualizar(colaboradorLancamento, Auditado);

                Servicos.Embarcador.Transportadores.Motorista.AtualizarStatusColaborador(unitOfWork, Auditado, false, colaboradorLancamento.Colaborador.Codigo);
                Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracaoSituacaoColaborador(unitOfWork, colaboradorLancamento.Codigo);

                if (colaboradorLancamento.SituacaoLancamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Finalizado)
                    ReprocessarFinalizadoAlteracaoDataFinal(colaboradorLancamento, diasFolgaAtual, unitOfWork);


                ExcluirAnexos(unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repColaboradorLancamento.BuscarPorCodigo(codigo, false);

                var dynColaboradorSituacaoLancamento = new
                {
                    colaboradorLancamento.Codigo,
                    colaboradorLancamento.Numero,
                    colaboradorLancamento.Descricao,
                    DataLancamento = colaboradorLancamento.Data.ToString("dd/MM/yyyy"),
                    Situacao = colaboradorLancamento.SituacaoLancamento,
                    colaboradorLancamento.Observacao,
                    DataInicial = colaboradorLancamento.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = colaboradorLancamento.DataFinal.ToString("dd/MM/yyyy"),

                    Operador = colaboradorLancamento.Operador?.Nome ?? string.Empty,
                    Colaborador = colaboradorLancamento.Colaborador != null ? new { colaboradorLancamento.Colaborador.Codigo, colaboradorLancamento.Colaborador.Descricao } : null,
                    ColaboradorSituacao = colaboradorLancamento.ColaboradorSituacao != null ? new { colaboradorLancamento.ColaboradorSituacao.Codigo, colaboradorLancamento.ColaboradorSituacao.Descricao } : null,
                    ListaAnexos = colaboradorLancamento.Anexos != null ? (from obj in colaboradorLancamento.Anexos
                                                                          select new
                                                                          {
                                                                              obj.Codigo,
                                                                              DescricaoAnexo = obj.Descricao,
                                                                              Arquivo = obj.NomeArquivo
                                                                          }).ToList() : null
                };

                return new JsonpResult(dynColaboradorSituacaoLancamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repColaboradorLancamento.BuscarPorCodigo(codigo, true);

                if (colaboradorLancamento == null || colaboradorLancamento.Colaborador == null)
                    return new JsonpResult(true, false, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracoes.IntegracaoA52 repIntegracaoA52 = new Repositorio.Embarcador.Configuracoes.IntegracaoA52(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 integracao = repIntegracaoA52.BuscarPrimeiroRegistro();
                if (integracao?.IntegrarSituacaoMotorista ?? false)
                    return new JsonpResult(true, false, "Não foi possível excluir o registro com a integração com a A52 ativa.");

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(colaboradorLancamento.Colaborador.Codigo, true);
                usuario.SituacaoColaborador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando;

                if (colaboradorLancamento.ColaboradorSituacao.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Folga)
                    usuario.DiasFolgaRetirado -= (int)(colaboradorLancamento.DataFinal - colaboradorLancamento.DataInicial).TotalDays;

                repUsuario.Atualizar(usuario, Auditado);

                repColaboradorLancamento.Deletar(colaboradorLancamento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repColaboradorLancamento.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();

                bool existeLancamentoEmExecucao = repColaboradorLancamento.BuscarSeExisteLancamentoEmAndamentoPorColaborador(colaboradorLancamento.Colaborador.Codigo);

                if (!existeLancamentoEmExecucao)
                {
                    colaboradorLancamento.Colaborador.SituacaoColaborador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando;
                    repUsuario.Atualizar(colaboradorLancamento.Colaborador);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, colaboradorLancamento.Colaborador, null, "Alterou a situação para Trabalhando.", unitOfWork);
                }

                colaboradorLancamento.SituacaoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Cancelado;
                repColaboradorLancamento.Atualizar(colaboradorLancamento);

                Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracaoSituacaoColaborador(unitOfWork, colaboradorLancamento.Codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, colaboradorLancamento, null, "Cancelou o Lançamento.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao estornar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                unitOfWork.Start();

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo repColaboradorLancamentoAnexo = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo(unitOfWork);

                int codigoColaboradorSituacaoLancamento = 0;
                int.TryParse(Request.Params("CodigoColaboradorSituacaoLancamento"), out codigoColaboradorSituacaoLancamento);

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "ColaboradorSituacaoLancamento");
                
                for (var i = 0; i < files.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo colaboradorLancamentoAnexo = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo();

                    string descricao = Request.Params("DescricaoAnexo");
                    Servicos.DTO.CustomFile file = files[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    colaboradorLancamentoAnexo.CaminhoArquivo = caminho;
                    colaboradorLancamentoAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));
                    colaboradorLancamentoAnexo.Descricao = descricao;
                    colaboradorLancamentoAnexo.ColaboradorLancamento = repColaboradorLancamento.BuscarPorCodigo(codigoColaboradorSituacaoLancamento, false);

                    repColaboradorLancamentoAnexo.Inserir(colaboradorLancamentoAnexo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, colaboradorLancamentoAnexo.ColaboradorLancamento, $"Adicionou o anexo {colaboradorLancamentoAnexo.Descricao} ", unitOfWork);
                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
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
                int codigoAnexo = 0;
                int.TryParse(Request.Params("CodigoAnexo"), out codigoAnexo);

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo repColaboradorLancamentoAnexo = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo colaboradorLancamentoAnexo = repColaboradorLancamentoAnexo.BuscarPorCodigo(codigoAnexo, false);

                if (colaboradorLancamentoAnexo == null)
                    return new JsonpResult(false, "Anexo não encontrado no Banco de Dados.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(colaboradorLancamentoAnexo.CaminhoArquivo))
                    return new JsonpResult(false, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(colaboradorLancamentoAnexo.CaminhoArquivo), "image/jpeg", colaboradorLancamentoAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Anexo do Chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherColaboradorSituacaoLancamento(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);

            int.TryParse(Request.Params("Colaborador"), out int colaborador);
            int.TryParse(Request.Params("ColaboradorSituacao"), out int colaboradorSituacao);

            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            colaboradorLancamento.Descricao = descricao;
            if (colaboradorLancamento.Codigo == 0)
            {
                colaboradorLancamento.Data = DateTime.Now;
                colaboradorLancamento.Numero = repColaboradorLancamento.ProximoNumeroColaboradorLancamento(codigoEmpresa);
                colaboradorLancamento.SituacaoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Agendado;
            }
            colaboradorLancamento.Observacao = observacao;
            colaboradorLancamento.DataInicial = dataInicial;
            colaboradorLancamento.DataFinal = dataFinal;

            colaboradorLancamento.Operador = this.Usuario;
            colaboradorLancamento.ColaboradorSituacao = repColaboradorSituacao.BuscarPorCodigo(colaboradorSituacao);
            colaboradorLancamento.Colaborador = repFuncionario.BuscarPorCodigo(colaborador);
            colaboradorLancamento.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
        }

        private void ExcluirAnexos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo repColaboradorLancamentoAnexo = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo(unitOfWork);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamentoAnexo colaboradorLancamentoAnexo = repColaboradorLancamentoAnexo.BuscarPorCodigo((int)anexo.Codigo, true);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(colaboradorLancamentoAnexo.CaminhoArquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(colaboradorLancamentoAnexo.CaminhoArquivo);

                    repColaboradorLancamentoAnexo.Deletar(colaboradorLancamentoAnexo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, colaboradorLancamentoAnexo.ColaboradorLancamento, $"Excluiu o anexo {colaboradorLancamentoAnexo.Descricao} ", unitOfWork);
                }
            }
        }

        private void ReprocessarFinalizadoAlteracaoDataFinal(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento lancamento, int diasFolgaAtual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
            Dominio.Entidades.Usuario usuario = lancamento.Colaborador;

            int diasFolgaNovo = (int)(lancamento.DataFinal - lancamento.DataInicial).TotalDays;
            int diasAdicionar = diasFolgaNovo > diasFolgaAtual ? (diasFolgaNovo - diasFolgaAtual) : -(diasFolgaAtual - diasFolgaNovo);

            if (lancamento.DataFinal > DateTime.Now)
            {
                lancamento.SituacaoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Execucao;
                usuario.SituacaoColaborador = lancamento.ColaboradorSituacao.SituacaoColaborador;

                if (lancamento.ColaboradorSituacao.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Folga)
                    usuario.DiasFolgaRetirado += diasAdicionar;


                repUsuario.Atualizar(usuario);
                repColaboradorLancamento.Atualizar(lancamento);
            }
            else if (lancamento.DataFinal <= DateTime.Now)
            {
                usuario.SituacaoColaborador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando;
                if (lancamento.ColaboradorSituacao.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Folga)
                    usuario.DiasFolgaRetirado += diasAdicionar;

                repUsuario.Atualizar(usuario);
                repColaboradorLancamento.Atualizar(lancamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, "Alterou a situação para Trabalhando.", unitOfWork);
            }
        }

        #endregion
    }
}
