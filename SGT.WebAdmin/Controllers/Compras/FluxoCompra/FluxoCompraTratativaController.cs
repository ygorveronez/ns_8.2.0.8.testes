using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Compras;

namespace SGT.WebAdmin.Controllers.Compras.FluxoCompra
{
    [CustomAuthorize("Compras/FluxoCompra")]
    public class FluxoCompraTratativaController : BaseController
    {
        #region Construtores

        public FluxoCompraTratativaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoCompra = Request.GetIntParam("Codigo");
                bool gravaAuditoria = Request.GetBoolParam("GravaAuditoria");

                var itensRepositorio = new Servicos.Embarcador.Compras.FluxoCompraTratativa(unitOfWork).BuscarPorFluxoCompra(codigoFluxoCompra);

                if (itensRepositorio == null)
                    return new JsonpResult(false, true, "Tratativas não encontradas.");

                dynamic retorno = new
                {
                    Tratativas = (from obj in itensRepositorio
                                  select new
                                  {
                                      obj.Codigo,
                                      Operador = obj.Usuario?.Nome,
                                      Data = obj.Data.ToString("dd/MM/yyyy"),
                                      Tratativa = obj.TextoLivre
                                  }).OrderByDescending(o => o.Data).ToList()
                };

                if (gravaAuditoria)
                {
                    Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                    Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorOrdemCompra(codigoFluxoCompra).FirstOrDefault();
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Visualizou a tratativa do fluxo de compra", unitOfWork);
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar tratativas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                new Servicos.Embarcador.Compras.FluxoCompraTratativa(unitOfWork).DeletarPorCodigo(codigo);
                return new JsonpResult(true, "Tratativa excluida com sucesso.");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir a tratativa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Inserir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                var codigoOrdemCompra = Request.GetIntParam("Codigo");
                var tratativa = Request.GetStringParam("Tratativa");
                bool concluirTratativa = Request.GetBoolParam("ConcluirTratativa");

                Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa ordemCompraTratativa = new Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa
                {
                    OrdemCompra = new Dominio.Entidades.Embarcador.Compras.OrdemCompra { Codigo = codigoOrdemCompra, SituacaoTratativa = (concluirTratativa ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTratativaFluxoCompra.Concluido : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTratativaFluxoCompra.Pendente) },
                    Usuario = Usuario,
                    TextoLivre = tratativa,
                    Data = DateTime.Now
                };

                new Servicos.Embarcador.Compras.FluxoCompraTratativa(unitOfWork).Inserir(ordemCompraTratativa);

                return DisparaNotificacao(codigoOrdemCompra, ordemCompraTratativa, unitOfWork);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir a tratativa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterStatusTratativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa tratativaconcluida = new Servicos.Embarcador.Compras.FluxoCompraTratativa(unitOfWork).ObterStatusTratativa(codigo);

                dynamic retorno;

                if (tratativaconcluida == null)
                    retorno = new { TratativaConcluida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTratativaFluxoCompra.Pendente };
                else
                    retorno = new { TratativaConcluida = tratativaconcluida.OrdemCompra.SituacaoTratativa };

                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar se já existe tratativa concluída.");
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        #endregion

        #region Métodos Privados 

        private JsonpResult DisparaNotificacao(int codigoOrdemCompra,
                                               Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa ordemCompraTratativa,
                                               Repositorio.UnitOfWork unitOfWork)
        {

            try
            {
                
                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorOrdemCompra(codigoOrdemCompra).FirstOrDefault();
                Servicos.Embarcador.Compras.FluxoCompraTratativa servicoFluxoCompraTratativa = new Servicos.Embarcador.Compras.FluxoCompraTratativa(unitOfWork);

                string mensagem = $"O Usuário {Usuario.Nome} incluiu uma nova tratativa no fluxo de compras N° {fluxoCompra.Numero}";

                List<Dominio.Entidades.Usuario> lstUsuariosTratativas  = servicoFluxoCompraTratativa.BuscarPorFluxoCompra(codigoOrdemCompra).Select(u => u.Usuario).Distinct().ToList();

                if (lstUsuariosTratativas.Count > 0)
                {
                    foreach (Dominio.Entidades.Usuario usuarioRecebeNotificacao in lstUsuariosTratativas)
                    {
                        if (Usuario.Codigo != usuarioRecebeNotificacao.Codigo)
                        {
                           GerarNotificacaoUsuario(mensagem, Usuario, usuarioRecebeNotificacao, unitOfWork);
                        }
                    }
                }
                else {
                    GerarNotificacaoUsuario(mensagem, Usuario, ordemCompraTratativa.OrdemCompra.Usuario, unitOfWork);
                }

                return new JsonpResult(true, true, "Tratativa inserida com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao disparar a notificação.");
            }

        }

        private void GerarNotificacaoUsuario(string mensagem, Dominio.Entidades.Usuario usuarioDiparaNotificacao, Dominio.Entidades.Usuario usuarioRecebeNotificacao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

            serNotificacao.GerarNotificacao(usuarioRecebeNotificacao,
                                            usuarioDiparaNotificacao,
                                            usuarioRecebeNotificacao.Codigo,
                                            "Compras/FluxoCompra",
                                            mensagem,
                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra,
                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SmartAdminBgColor.blue,
                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta,
                                            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork);
        }

        #endregion
    }
}
