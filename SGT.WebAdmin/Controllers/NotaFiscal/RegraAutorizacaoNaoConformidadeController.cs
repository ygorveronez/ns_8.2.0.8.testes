using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/RegraAutorizacaoNaoConformidade")]
    public class RegraAutorizacaoNaoConformidadeController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade>
    {
		#region Construtores

		public RegraAutorizacaoNaoConformidadeController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade regra)
        {
            List<Dominio.Entidades.Usuario> aprovadores = (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) ? regra.Aprovadores.ToList() : new List<Dominio.Entidades.Usuario>();
            List<Dominio.Entidades.Setor> setores = (regra.TipoAprovadorRegra == TipoAprovadorRegra.Setor) ? regra.Setores.ToList() : new List<Dominio.Entidades.Setor>();

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
                UsarRegraPorCFOP = regra.RegraPorCFOP,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorTipoNaoConformidade = regra.RegraPorNaoConformidade,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                Aprovadores = (from aprovador in aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                Setores = (from setor in setores select new { setor.Codigo, setor.Descricao }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasCFOP = (from alcada in regra.AlcadasCFOP select ObterRegraPorTipo<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaCFOP, Dominio.Entidades.CFOP>(alcada)).ToList(),
                AlcadasTipoNaoConformidade = (from alcada in regra.AlcadasNaoConformidade select ObterRegraPorTipo<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaNaoConformidade, Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                regra.Grupo,
                regra.SubGrupo,
                regra.Area
            });
        }

        protected override void PreencherAprovadores(Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade regra, Repositorio.UnitOfWork unitOfWork)
        {
            regra.Aprovadores = new List<Dominio.Entidades.Usuario>();
            regra.Setores = new List<Dominio.Entidades.Setor>();

            if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
                regra.Aprovadores = ObterAprovadores(regra, unitOfWork);
            else if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Setor)
                regra.Setores = ObterSetores(regra, unitOfWork);
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade>(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);
                Repositorio.CFOP repositorioCFOP = new Repositorio.CFOP(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade regraAutorizacaoNaoConformidade = new Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade();

                PreencherRegra(regraAutorizacaoNaoConformidade, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorCFOP = Request.GetBoolParam("UsarRegraPorCFOP");
                    regra.RegraPorNaoConformidade = Request.GetBoolParam("UsarRegraPorTipoNaoConformidade");
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                    regra.Grupo = Request.GetEnumParam<GrupoNC>("Grupo");
                    regra.SubGrupo = Request.GetEnumParam<SubGrupoNC>("SubGrupo");
                    regra.Area = Request.GetEnumParam<AreaNC>("Area");
                }));

                repositorioRegra.Inserir(regraAutorizacaoNaoConformidade, Auditado);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoNaoConformidade, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaCFOP, Dominio.Entidades.CFOP>(unitOfWork, regraAutorizacaoNaoConformidade, "AlcadasCFOP", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.CFOP cfop = repositorioCFOP.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = cfop ?? throw new ControllerException("CFOP não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoNaoConformidade, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaNaoConformidade, Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>(unitOfWork, regraAutorizacaoNaoConformidade, "AlcadasTipoNaoConformidade", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = naoConformidade ?? throw new ControllerException("Não Conformidade não encontrado.");
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade>(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);
                Repositorio.CFOP repositorioCFOP = new Repositorio.CFOP(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade regraAutorizacaoNaoConformidade = repositorioRegra.BuscarPorCodigo(codigo, auditavel: true);

                if (regraAutorizacaoNaoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegra(regraAutorizacaoNaoConformidade, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorCFOP = Request.GetBoolParam("UsarRegraPorCFOP");
                    regra.RegraPorNaoConformidade = Request.GetBoolParam("UsarRegraPorTipoNaoConformidade");
                    regra.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
                    regra.Grupo = Request.GetEnumParam<GrupoNC>("Grupo");
                    regra.SubGrupo = Request.GetEnumParam<SubGrupoNC>("SubGrupo");
                    regra.Area = Request.GetEnumParam<AreaNC>("Area");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacaoNaoConformidade, regraAutorizacaoNaoConformidade.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacaoNaoConformidade, regraAutorizacaoNaoConformidade.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaCFOP, Dominio.Entidades.CFOP>(unitOfWork, regraAutorizacaoNaoConformidade, regraAutorizacaoNaoConformidade.AlcadasCFOP, "AlcadasCFOP", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.CFOP cfop = repositorioCFOP.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = cfop ?? throw new ControllerException("CFOP não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AlcadaNaoConformidade, Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>(unitOfWork, regraAutorizacaoNaoConformidade, regraAutorizacaoNaoConformidade.AlcadasNaoConformidade, "AlcadasTipoNaoConformidade", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = naoConformidade ?? throw new ControllerException("Não Conformidade não encontrado.");
                }));

                repositorioRegra.Atualizar(regraAutorizacaoNaoConformidade, Auditado);

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

        #endregion
    }
}