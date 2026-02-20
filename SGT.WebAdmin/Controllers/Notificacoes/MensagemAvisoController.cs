using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/MensagemAviso")]
    public class MensagemAvisoController : BaseController
    {
		#region Construtores

		public MensagemAvisoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string titulo = Request.Params("Titulo");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario?.Empresa?.EmpresaPai?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Inicial", "DataInicio", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Final", "DataFim", 12, Models.Grid.Align.left, true);
               

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho("Título", "Titulo", 45, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Exibir Em", "TipoServicoMultisoftware", 15, Models.Grid.Align.left, true);
                }
                else
                {
                    grid.AdicionarCabecalho("Título", "Titulo", 60, Models.Grid.Align.left, true);
                }

                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unidadeTrabalho);

                List<Dominio.Entidades.MensagemAviso> listaMensagemAviso = repMensagemAviso.Consultar(dataInicial, dataFinal, titulo, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, codigoEmpresa);

                grid.setarQuantidadeTotal(repMensagemAviso.ContarConsulta(dataInicial, dataFinal, titulo, situacao, codigoEmpresa));

                grid.AdicionaRows((from obj in listaMensagemAviso
                                   select new
                                   {
                                       obj.Codigo,
                                       DataInicio = obj.DataInicio.ToString("dd/MM/yyyy"),
                                       DataFim = obj.DataFim.ToString("dd/MM/yyyy"),
                                       obj.Titulo,
                                       obj.DescricaoAtivo,
                                       TipoServicoMultisoftware = obj.DescricaoTipoServicoMultisoftware
                                       
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarAtivas()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unidadeTrabalho);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario?.Empresa?.EmpresaPai?.Codigo ?? 0;

                List<Dominio.Entidades.MensagemAviso> mensagensAviso = repMensagemAviso.BuscarParaExibicao(DateTime.Now, TipoServicoMultisoftware, codigoEmpresa);

                return new JsonpResult((from obj in mensagensAviso
                                        select new
                                        {
                                            Mensagem = obj.Descricao,
                                            obj.Titulo,
                                            Anexos = (from anexo in obj.Anexos select new { 
                                                anexo.Codigo,
                                                anexo.Descricao
                                            }).ToList()
                                        }).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as mensagens de aviso.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
                Enum.TryParse(Request.Params("TipoServicoMultisoftware"), out tipoServicoMultisoftware);

                string mensagem = Request.Params("Mensagem");
                string titulo = Request.Params("Titulo");
                string observacao = Request.Params("Observacao");

                unidadeTrabalho.Start();
                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unidadeTrabalho);

                Dominio.Entidades.MensagemAviso mensagemAviso = new Dominio.Entidades.MensagemAviso
                {
                    Ativo = ativo,
                    DataFim = dataFinal,
                    DataInicio = dataInicial,
                    Descricao = mensagem,
                    Status = ativo ? "A" : "I",
                    Titulo = titulo,
                    TipoServicoMultisoftware = tipoServicoMultisoftware,
                    Observacao = observacao
                };

                repMensagemAviso.Inserir(mensagemAviso, Auditado);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new {
                    mensagemAviso.Codigo
                });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;
                Enum.TryParse(Request.Params("TipoServicoMultisoftware"), out tipoServicoMultisoftware);

                string mensagem = Request.Params("Mensagem");
                string titulo = Request.Params("Titulo");
                string observacao = Request.Params("Observacao"); ;

                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unidadeTrabalho);

                unidadeTrabalho.Start();

                Dominio.Entidades.MensagemAviso mensagemAviso = repMensagemAviso.BuscarPorCodigo(codigo);

                mensagemAviso.Ativo = ativo;
                mensagemAviso.DataFim = dataFinal;
                mensagemAviso.DataInicio = dataInicial;
                mensagemAviso.Descricao = mensagem;
                mensagemAviso.Status = ativo ? "A" : "I";
                mensagemAviso.Titulo = titulo;
                mensagemAviso.TipoServicoMultisoftware = tipoServicoMultisoftware;
                mensagemAviso.Observacao = observacao;


                repMensagemAviso.Atualizar(mensagemAviso, Auditado);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unidadeTrabalho);

                Dominio.Entidades.MensagemAviso mensagemAviso = repMensagemAviso.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    mensagemAviso.Ativo,
                    mensagemAviso.Codigo,
                    Mensagem = mensagemAviso.Descricao,
                    DataFinal = mensagemAviso.DataFim.ToString("dd/MM/yyyy"),
                    DataInicial = mensagemAviso.DataInicio.ToString("dd/MM/yyyy"),
                    mensagemAviso.Titulo,
                    mensagemAviso.Observacao,
                    mensagemAviso.TipoServicoMultisoftware,
                    Anexos = (from anexo in mensagemAviso.Anexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo,
                              }).ToList(),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.MensagemAviso repMensagemAviso = new Repositorio.MensagemAviso(unidadeTrabalho);

                Dominio.Entidades.MensagemAviso mensagemAviso = repMensagemAviso.BuscarPorCodigo(codigo);

                repMensagemAviso.Deletar(mensagemAviso);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);

                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
