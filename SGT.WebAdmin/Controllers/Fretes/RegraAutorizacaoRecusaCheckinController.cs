using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { }, "Fretes/RegraAutorizacaoRecusaCheckin")]
    public class RegraAutorizacaoRecusaCheckinController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin>
    {
		#region Construtores

		public RegraAutorizacaoRecusaCheckinController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin regra)
        {
            List<Dominio.Entidades.Usuario> aprovadores = (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) ? regra.Aprovadores.ToList() : new List<Dominio.Entidades.Usuario>();

            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                regra.TipoAprovadorRegra,
                Aprovadores = (from aprovador in aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorTransportador = regra.RegraPorTransportador,
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList(),
            });
        }

        protected override void PreencherAprovadores(Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin regra, Repositorio.UnitOfWork unitOfWork)
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

                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var regraAutorizacaoRecusaCheckin = new Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin();

                PreencherRegra(regraAutorizacaoRecusaCheckin, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                }));

                repositorioRegra.Inserir(regraAutorizacaoRecusaCheckin, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoRecusaCheckin, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoRecusaCheckin, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

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
                var repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin>(unitOfWork);
                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                var repositorioTransportador = new Repositorio.Empresa(unitOfWork);
                var regraAutorizacaoRecusaCheckin = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacaoRecusaCheckin == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacaoRecusaCheckin.Initialize();

                PreencherRegra(regraAutorizacaoRecusaCheckin, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoRecusaCheckin, regraAutorizacaoRecusaCheckin.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacaoRecusaCheckin, regraAutorizacaoRecusaCheckin.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoRecusaCheckin, Auditado);

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