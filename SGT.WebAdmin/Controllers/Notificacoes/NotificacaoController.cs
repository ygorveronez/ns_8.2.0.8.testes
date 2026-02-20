using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/Notificacao")]
    public class NotificacaoController : BaseController
    {
		#region Construtores

		public NotificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        // GET: Notificacao

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao situacaoNotificacao = !string.IsNullOrEmpty(Request.Params("Situacao")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao)int.Parse(Request.Params("Situacao")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas;
                Repositorio.Embarcador.Notificacoes.Notificacao repNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("URLPagina", false);
                grid.AdicionarCabecalho("SituacaoNotificacao", false);
                grid.AdicionarCabecalho("TipoNotificacao", false);
                grid.AdicionarCabecalho("CodigoObjetoNotificacao", false);
                grid.AdicionarCabecalho("Descrição", "Nota", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "DataNotificacao", 25, Models.Grid.Align.left, true);

                if (situacaoNotificacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas)
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);

                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);


                List<Dominio.Entidades.Embarcador.Notificacoes.Notificacao> notificacoes = repNotificacao.Consultar(this.Usuario.Codigo, situacaoNotificacao, dataInicio, dataFim, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotificacao.ContarConsulta(this.Usuario.Codigo, situacaoNotificacao, dataInicio, dataFim));
                var lista = (from obj in notificacoes
                             select new
                             {
                                 obj.Codigo,
                                 obj.DescricaoSituacao,
                                 obj.CodigoObjetoNotificacao,
                                 DataNotificacao = obj.DataNotificacao.ToString("dd/MM/yyyy HH:mm"),
                                 obj.SituacaoNotificacao,
                                 obj.URLPagina,
                                 obj.TipoNotificacao,
                                 obj.Nota,
                                 Icone = obj.Icone.ObterDescricao(),
                                 IconeCorFundo = "bg-color-" + obj.IconeCorFundo.ToString(),
                                 ReadClass = obj.SituacaoNotificacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Nova ? "unread" : ""
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarNovasNotificacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada repNotoficacaoQuantidadeNaoVisualizada = new Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada(unitOfWork);
                Repositorio.Embarcador.Notificacoes.Notificacao repNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(unitOfWork);
                int inicio = int.Parse(Request.Params("inicio"));
                int limite = int.Parse(Request.Params("limite"));
                List<Dominio.Entidades.Embarcador.Notificacoes.Notificacao> notificacoes = repNotificacao.Consultar(this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas, DateTime.MinValue, DateTime.MinValue, "DataNotificacao", "desc", inicio, limite);
                int total = repNotificacao.ContarConsulta(this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas, DateTime.MinValue, DateTime.MinValue);

                Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada notoficacaoQuantidadeNaoVisualizada = repNotoficacaoQuantidadeNaoVisualizada.BuscarPorUsuario(this.Usuario.Codigo);

                var retorno = new
                {
                    Total = total,
                    NaoVistas = notoficacaoQuantidadeNaoVisualizada != null ? notoficacaoQuantidadeNaoVisualizada.QuantidadeNaoVisualizada : 0,
                    Notificacoes = from obj in notificacoes
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.CodigoObjetoNotificacao,
                                       DataNotificacao = obj.DataNotificacao.ToString("dd/MM/yyyy HH:mm"),
                                       Data = obj.DataNotificacao.ToDateString(),
                                       Hora = obj.DataNotificacao.ToTimeString(),
                                       obj.SituacaoNotificacao,
                                       obj.TipoNotificacao,
                                       obj.URLPagina,
                                       obj.Nota,
                                       StatusVisual = obj.IconeCorFundo.ObterSatatusBootstrap(),
                                       Icone = obj.Icone.ObterDescricao(),
                                       IconeCorFundo = "bg-color-" + obj.IconeCorFundo.ToString(),
                                       ReadClass = obj.SituacaoNotificacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Nova ? "unread" : ""
                                   }
                };

                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as notificacoes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> BuscarNovasNotificacoesPortalCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada repNotoficacaoQuantidadeNaoVisualizada = new Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada(unitOfWork);
                Repositorio.Embarcador.Notificacoes.Notificacao repNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(unitOfWork);
                int inicio = int.Parse(Request.Params("inicio"));
                int limite = int.Parse(Request.Params("limite"));

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao> TiposNotificacao = new List<TipoNotificacao>();
                TiposNotificacao.Add(TipoNotificacao.ocorrenciaPedido);

                List<Dominio.Entidades.Embarcador.Notificacoes.Notificacao> notificacoes = repNotificacao.Consultar(this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas, DateTime.MinValue, DateTime.MinValue, "DataNotificacao", "desc", inicio, limite, TiposNotificacao);
                int total = repNotificacao.ContarConsulta(this.Usuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Todas, DateTime.MinValue, DateTime.MinValue, TiposNotificacao);

                Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada notoficacaoQuantidadeNaoVisualizada = repNotoficacaoQuantidadeNaoVisualizada.BuscarPorUsuario(this.Usuario.Codigo);

                var retorno = new
                {
                    Total = total,
                    NaoVistas = notoficacaoQuantidadeNaoVisualizada != null ? notoficacaoQuantidadeNaoVisualizada.QuantidadeNaoVisualizada : 0,
                    Notificacoes = from obj in notificacoes
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.CodigoObjetoNotificacao,
                                       DataNotificacao = obj.DataNotificacao.ToString("dd/MM/yyyy HH:mm"),
                                       Data = obj.DataNotificacao.ToDateString(),
                                       Hora = obj.DataNotificacao.ToTimeString(),
                                       obj.SituacaoNotificacao,
                                       obj.TipoNotificacao,
                                       obj.URLPagina,
                                       obj.Nota,
                                       StatusVisual = obj.IconeCorFundo.ObterSatatusBootstrap(),
                                       Icone = obj.Icone.ObterDescricao(),
                                       IconeCorFundo = "bg-color-" + obj.IconeCorFundo.ToString(),
                                       ReadClass = obj.SituacaoNotificacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Nova ? "unread" : ""
                                   }
                };

                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as notificacoes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SetarParaVisualizadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada repNotoficacaoQuantidadeNaoVisualizada = new Repositorio.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada notoficacaoQuantidadeNaoVisualizada = repNotoficacaoQuantidadeNaoVisualizada.BuscarPorUsuario(this.Usuario.Codigo);

                if (notoficacaoQuantidadeNaoVisualizada != null)
                {
                    notoficacaoQuantidadeNaoVisualizada.QuantidadeNaoVisualizada = 0;
                    repNotoficacaoQuantidadeNaoVisualizada.Atualizar(notoficacaoQuantidadeNaoVisualizada);
                }

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar a visualização das notificações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> MarcarNotificacaoComoLida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Notificacoes.Notificacao repNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao = repNotificacao.BuscarPorCodigo(codigo);
                if (notificacao != null && notificacao.Usuario.Codigo == this.Usuario.Codigo)
                {
                    notificacao.SituacaoNotificacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotificacao.Lida;
                    repNotificacao.Atualizar(notificacao);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Não foi possível confirmar a leitura da notificação.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar a leitura da notificação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DispararNotificacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNotificacao;
                int.TryParse(Request.Params("Notificacao"), out codigoNotificacao);

                Repositorio.Embarcador.Notificacoes.Notificacao repNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Notificacoes.Notificacao notificacao = repNotificacao.BuscarPorCodigo(codigoNotificacao);

                if (notificacao != null)
                {
                    Servicos.Embarcador.Hubs.Notificacao hubNotificacao = new Servicos.Embarcador.Hubs.Notificacao();
                    hubNotificacao.NotificarUsuario(notificacao);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao disparar a notificação.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DispararProcessamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int usuario;
                int.TryParse(Request.Params("Usuario"), out usuario);

                decimal percentual;
                decimal.TryParse(Request.Params("Percentual"), out percentual);

                string pagina = Request.Params("Pagina");

                int intTipoNotificacao = 0;
                int.TryParse(Request.Params("TipoNotificacao"), out intTipoNotificacao);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao tipoNotificacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao)intTipoNotificacao;

                Dominio.Entidades.Usuario usuarioInformar = null;

                if (usuario > 0)
                {
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                    usuarioInformar = repUsuario.BuscarPorCodigo(usuario);
                }

                Servicos.Embarcador.Hubs.Notificacao hubNotificacao = new Servicos.Embarcador.Hubs.Notificacao();
                hubNotificacao.InformarPercentualProcessado(tipoNotificacao, codigo, pagina, percentual, usuarioInformar);


                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao informar a alteração do percentual de processamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
