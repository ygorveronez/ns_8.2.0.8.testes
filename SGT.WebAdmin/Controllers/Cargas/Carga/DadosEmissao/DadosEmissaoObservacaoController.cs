using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class DadosEmissaoObservacaoController : BaseController
    {
		#region Construtores

		public DadosEmissaoObservacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AtualizarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarObservacao) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = int.Parse(Request.Params("Carga"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                bool permitirAdicionarObservacaoNaEtapaUmDaCarga = carga.TipoOperacao?.ConfiguracaoCarga?.PermitirAdicionarObservacaoNaEtapaUmDaCarga ?? false;

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && carga.ExigeNotaFiscalParaCalcularFrete && !permitirAdicionarObservacaoNaEtapaUmDaCarga)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                string observacao = Request.Params("Observacao");
                string observacaoTerceiro = Request.Params("ObservacaoTerceiro");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
                {
                    cargaPedido.Pedido.ObservacaoCTe = observacao;
                    cargaPedido.Pedido.ObservacaoCTeTerceiro = observacaoTerceiro;

                    repPedido.Atualizar(cargaPedido.Pedido);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Pedido, null, "Atualizou a observação.", unitOfWork);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Atualizou a observação.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a observação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterInformacoesCargaDadosEmissaoObservacaoDadosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = int.Parse(Request.Params("Carga"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                if (cargaPedido != null)
                {
                    var retorno = new
                    {
                        Observacao = cargaPedido.Pedido.ObservacaoCTe,
                        ObservacaoTerceiro = cargaPedido.Pedido.ObservacaoCTeTerceiro
                    };

                    return new JsonpResult(retorno);
                }

                return new JsonpResult(null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados de observação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
