using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "DownloadPlanilha", "PesquisaAutorizacoes", "DetalhesAutorizacao" }, "Fretes/ControleReajusteFretePlanilha")]
    public class ControleReajusteFretePlanilhaController : BaseController
    {
        #region Construtores

        public ControleReajusteFretePlanilhaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repAprovacaoAlcadaControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Justificativa", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> listaAutorizacao = repAprovacaoAlcadaControleReajusteFretePlanilha.ConsultarAutorizacoesPorControle(codigo, parametrosConsulta);
                grid.setarQuantidadeTotal(repAprovacaoAlcadaControleReajusteFretePlanilha.ContarConsultaAutorizacoesPorControle(codigo));

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.Situacao.ObterDescricao(),
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = obj.Descricao,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 Justificativa = obj.Motivo ?? string.Empty,
                                 DT_RowColor = obj.Situacao.ObterCorGrid()
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = repControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                // Valida
                if (controle == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    controle.Codigo,
                    controle.Situacao,
                    controle.Numero,
                    controle.Observacao,
                    controle.NomeArquivo,
                    TipoOperacao = new { controle.TipoOperacao.Codigo, controle.TipoOperacao.Descricao },
                    Filial = new { controle.Filial.Codigo, controle.Filial.Descricao },
                    Empresa = new { Codigo = controle.Empresa?.Codigo ?? 0, Descricao = controle.Empresa?.Descricao ?? "" },
                    Resumo = controle.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.SemRegra ? null : ResumoAutorizacao(controle, unitOfWork)
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = new Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha();

                // Preenche entidade com dados
                PreencheEntidade(ref controle, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(controle, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repControleReajusteFretePlanilha.Inserir(controle, Auditado);

                bool possuiRegras = VerificarRegrasAutorizacao(controle, TipoServicoMultisoftware, unitOfWork);
                if (!possuiRegras)
                    controle.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.SemRegra;
                else
                    controle.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.AgAprovacao;

                repControleReajusteFretePlanilha.Atualizar(controle);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    controle.Codigo
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = repControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                // Valida
                if (controle == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (controle.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                // Busca as regras
                bool possuiRegras = VerificarRegrasAutorizacao(controle, TipoServicoMultisoftware, unitOfWork);
                if (possuiRegras)
                    controle.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.AgAprovacao;

                // Persiste dados
                repControleReajusteFretePlanilha.Atualizar(controle);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    controle.Codigo,
                    PossuiRegra = possuiRegras,
                    Situacao = controle.Situacao,
                    Resumo = !possuiRegras ? null : ResumoAutorizacao(controle, unitOfWork)
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = repControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                // Valida
                if (controle == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (controle.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.AgAprovacao && controle.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.SemRegra)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Fiz isso pra conseguir enviar o arquivo logo após criar a solicitação
                if (DateTime.Now.Subtract(controle.DataCriacao).TotalMinutes > 5)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                if (!AdicionarPlanilha(ref controle, unitOfWork, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repControleReajusteFretePlanilha.Atualizar(controle);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadPlanilha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = repControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                // Valida
                if (controle == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!string.IsNullOrWhiteSpace(controle.Arquivo))
                {
                    byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(controle.Arquivo);
                    return Arquivo(pdf, "application/pdf", controle.NomeArquivo);
                }
                else
                {
                    return new JsonpResult(false, false, "Ainda não foi enviada a imagem da NFS gerada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Baixar da DANFSE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarFinalizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = repControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                // Valida
                if (controle.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Aprovado)
                    return new JsonpResult(false, true, "Só é possível finalizar quando a solicitação for Aprovada.");

                controle.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Finalizado;

                // Persiste dados
                repControleReajusteFretePlanilha.Atualizar(controle);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, controle, null, "Finalizou o reajuste.", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = repControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                // Valida
                if (controle.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.AgAprovacao)
                    return new JsonpResult(false, true, "Só é possível cancelar quando a situação for Ag. Aprovação.");

                controle.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha.Cancelado;

                // Persiste dados
                repControleReajusteFretePlanilha.Atualizar(controle);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, controle, null, "Cancelou o reajuste.", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Número").Tamanho(5).Align(Models.Grid.Align.right);
            grid.Prop("Filial").Nome("Filial").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Empresa").Nome("Transportador").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("TipoOperacao").Nome("Tipo de Operação").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);


            // Dados do filtro
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("Empresa"), out int empresa);
            int.TryParse(Request.Params("SituacaoControleReajusteFretePlanilha"), out int intSituacaoControleReajusteFretePlanilha);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha situacaoControleReajusteFretePlanilha = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleReajusteFretePlanilha)intSituacaoControleReajusteFretePlanilha;

            // Consulta
            List<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> listaGrid = repControleReajusteFretePlanilha.Consultar(numero, tipoOperacao, filial, empresa, situacaoControleReajusteFretePlanilha, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repControleReajusteFretePlanilha.ContarConsulta(numero, tipoOperacao, filial, empresa, situacaoControleReajusteFretePlanilha);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            Filial = obj.Filial.Descricao,
                            Empresa = obj.Empresa?.Descricao,
                            TipoOperacao = obj.TipoOperacao.Descricao,
                            Situacao = obj.DescricaoSituacao,
                        };

            return lista.ToList();
        }

        private bool AdicionarPlanilha(ref Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = "";
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            // Extrai dados
            Servicos.DTO.CustomFile file = arquivos[0];

            var reader = new BinaryReader(file.InputStream);
            byte[] pdfData = reader.ReadBytes((int)file.Length);
            string extensao = Path.GetExtension(file.FileName).ToLower();
            string nomeArquivo = Guid.NewGuid().ToString() + extensao;
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(CaminhoArquivo(unitOfWork), nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, pdfData);

            controle.Arquivo = caminho;
            controle.NomeArquivo = file.FileName;

            return true;
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
            int.TryParse(Request.Params("Empresa"), out int empresa);

            string obs = Request.Params("Observacao") ?? string.Empty;

            // Vincula dados
            controle.Filial = repFilial.BuscarPorCodigo(filial);
            controle.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
            controle.Numero = repControleReajusteFretePlanilha.BuscarProximoNumero();
            controle.Empresa = repEmpresa.BuscarPorCodigo(empresa);
            controle.Observacao = obs;
            controle.DataCriacao = DateTime.Now;
            controle.Usuario = this.Usuario;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, out string msgErro)
        {
            msgErro = "";

            if (controle.Filial == null)
            {
                msgErro = "Filial é obrigatório.";
                return false;
            }

            if (controle.TipoOperacao == null)
            {
                msgErro = "Tipo de Operação é obrigatório.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Filial") propOrdenar = "Filial.Descricao";
            else if (propOrdenar == "TipoOperacao") propOrdenar = "TipoOperacao.Descricao";
        }

        private bool VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> listaFiltrada = Servicos.Embarcador.Frete.ControleReajusteFretePlanilha.VerificarRegrasAutorizacao(controle, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.Frete.ControleReajusteFretePlanilha.CriarRegrasAutorizacao(listaFiltrada, controle, this.Usuario, tipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
                return true;
            }

            return false;
        }

        private dynamic ResumoAutorizacao(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(controle.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(controle.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(controle.Codigo);

            return new
            {
                Solicitante = controle.Usuario.Nome,
                DataSolicitacao = controle.DataCriacao.ToString("dd/MM/yyyy"),
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = controle.DescricaoSituacao,
            };
        }

        private string CaminhoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "ReajusteFretePorPlanilha" });
        }

        #endregion Métodos Privados
    }
}
