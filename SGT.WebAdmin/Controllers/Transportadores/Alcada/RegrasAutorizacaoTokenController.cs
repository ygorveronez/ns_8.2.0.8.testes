using Dominio.Entidades.Embarcador.Transportadores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores.Alcada
{
    [CustomAuthorize(new string[] { }, "Transportadores/RegrasAutorizacaoToken")]
    public class RegrasAutorizacaoTokenController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken>
    {
		#region Construtores

		public RegrasAutorizacaoTokenController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(RegraAutorizacaoToken regra)
        {
            List<Dominio.Entidades.Usuario> aprovadores = (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) ? regra.Aprovadores.ToList() : new List<Dominio.Entidades.Usuario>();

            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                regra.NumeroReprovadores,
                regra.EtapaAutorizacaoToken,
                regra.EnviarLinkParaAprovacaoPorEmail,
                AprovacaoAutomaticaAposDias = regra.PrazoAprovacaoAutomatica,
                regra.TipoDiasAprovacao,
                DiasPrazoAprovacao = regra.PrazoAprovacao,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                regra.TipoAprovadorRegra,
                Aprovadores = (from aprovador in aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList()
            });
        }

        protected override void PreencherAprovadores(Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken regra, Repositorio.UnitOfWork unitOfWork)
        {
            regra.Aprovadores = new List<Dominio.Entidades.Usuario>();

            if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
                regra.Aprovadores = ObterAprovadores(regra, unitOfWork);
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken>(unitOfWork);
                var regraAutorizacaoRecusaToken = new Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken();

                PreencherRegra(regraAutorizacaoRecusaToken, unitOfWork, ((regra) =>
                {
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                    regra.NumeroReprovadores = Request.GetIntParam("NumeroReprovadores");
                    regra.EtapaAutorizacaoToken = Request.GetEnumParam<EtapaAutorizacaoToken>("EtapaAutorizacaoToken");
                    regra.TipoDiasAprovacao = Request.GetEnumParam<TipoDiasAprovacao>("TipoDiasAprovacao");
                    regra.PrazoAprovacaoAutomatica = Request.GetIntParam("AprovacaoAutomaticaAposDias");
                    regra.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
                    regra.PrazoAprovacao = Request.GetNullableIntParam("DiasPrazoAprovacao");
                }));

                repositorioRegra.Inserir(regraAutorizacaoRecusaToken, Auditado);


                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public override IActionResult Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken>(unitOfWork);

                var regraAutorizacaoToken = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoToken == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoToken.Initialize();

                PreencherRegra(regraAutorizacaoToken, unitOfWork, ((regra) =>
                {
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                    regra.NumeroReprovadores = Request.GetIntParam("NumeroReprovadores");
                    regra.EtapaAutorizacaoToken = Request.GetEnumParam<EtapaAutorizacaoToken>("EtapaAutorizacaoToken");
                    regra.TipoDiasAprovacao = Request.GetEnumParam<TipoDiasAprovacao>("EtapaAutorizacaoToken");
                    regra.PrazoAprovacaoAutomatica = Request.GetIntParam("AprovacaoAutomaticaAposDias");
                    regra.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
                    regra.PrazoAprovacao = Request.GetNullableIntParam("DiasPrazoAprovacao");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoToken, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos Sobrescritos
    }
}