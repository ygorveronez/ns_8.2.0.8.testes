using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos.AlteracaoPedido
{
    [CustomAuthorize(new string[] { }, "Pedidos/RegraAutorizacaoAlteracaoPedido")]
    public class RegraAutorizacaoAlteracaoPedidoController : RegraAutorizacao.RegraAutorizacaoController<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido>
    {
		#region Construtores

		public RegraAutorizacaoAlteracaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido regra)
        {
            return new JsonpResult(new
            {
                regra.Codigo,
                regra.NumeroAprovadores,
                Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                regra.Descricao,
                Status = regra.Ativo,
                regra.Observacoes,
                regra.PrioridadeAprovacao,
                UsarRegraPorFilial = regra.RegraPorFilial,
                UsarRegraPorTipoCarga = regra.RegraPorTipoCarga,
                UsarRegraPorTipoOperacao = regra.RegraPorTipoOperacao,
                UsarRegraPorTransportador = regra.RegraPorTransportador,
                Aprovadores = (from aprovador in regra.Aprovadores select new { aprovador.Codigo, aprovador.Nome }).ToList(),
                AlcadasFilial = (from alcada in regra.AlcadasFilial select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(alcada)).ToList(),
                AlcadasTipoCarga = (from alcada in regra.AlcadasTipoCarga select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(alcada)).ToList(),
                AlcadasTipoOperacao = (from alcada in regra.AlcadasTipoOperacao select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(alcada)).ToList(),
                AlcadasTransportador = (from alcada in regra.AlcadasTransportador select ObterRegraPorTipo<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTransportador, Dominio.Entidades.Empresa>(alcada)).ToList()
            });
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override IActionResult Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido>(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido regraAutorizacao = new Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                }));

                repositorioRegra.Inserir(regraAutorizacao, Auditado);

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoDeCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AdicionarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacao, "AlcadasTransportador", ((valorPropriedade, alcada) =>
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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido>(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido regraAutorizacao = repositorioRegra.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                regraAutorizacao.Initialize();

                PreencherRegra(regraAutorizacao, unitOfWork, ((regra) => {
                    regra.RegraPorFilial = Request.GetBoolParam("UsarRegraPorFilial");
                    regra.RegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
                    regra.RegraPorTipoOperacao = Request.GetBoolParam("UsarRegraPorTipoOperacao");
                    regra.RegraPorTransportador = Request.GetBoolParam("UsarRegraPorTransportador");
                }));

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasFilial, "AlcadasFilial", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = filial ?? throw new ControllerException("Filial não encontrada.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoCarga, "AlcadasTipoCarga", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoDeCarga.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoCarga ?? throw new ControllerException("Tipo de carga não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTipoOperacao, "AlcadasTipoOperacao", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = tipoOperacao ?? throw new ControllerException("Tipo de operação não encontrado.");
                }));

                AtualizarAlcadas<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AlcadaTransportador, Dominio.Entidades.Empresa>(unitOfWork, regraAutorizacao, regraAutorizacao.AlcadasTransportador, "AlcadasTransportador", ((valorPropriedade, alcada) =>
                {
                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo((int)valorPropriedade);

                    alcada.PropriedadeAlcada = transportador ?? throw new ControllerException("Transportador não encontrado.");
                }));

                repositorioRegra.Atualizar(regraAutorizacao, Auditado);

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

        #endregion
    }
}