using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosEmissao
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class DadosEmissaoRotaController : BaseController
    {
        #region Construtores

        public DadosEmissaoRotaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AtualizarRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarRota) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete repCargaPedidoRotaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoRotaFrete(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que esta ação seja executada.");

                if (carga.CalculandoFrete)
                    return new JsonpResult(false, true, "Não é possível atualizar as rotas enquanto a carga estiver em processo de cálculo de valores do frete.");

                if (!serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, "Não é possível alterar os dados da emissão na atual situação da carga (" + carga.DescricaoSituacaoCarga + ")");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                unitOfWork.Start();

                dynamic rotasFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasFrete"));

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
                {
                    if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal || cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                    {
                        repCargaPedidoRotaFrete.DeletarPorCargaPedido(cargaPedido.Codigo);

                        for (var i = 0; i < rotasFrete.Count; i++)
                        {
                            bool inserir = false;

                            Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigo((int)rotasFrete[i].Codigo);

                            if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                            {
                                cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;

                                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (cargaPedido.Pedido.Remetente?.GrupoPessoas != null && rotaFrete.GrupoPessoas != null && cargaPedido.Pedido.Remetente.GrupoPessoas.Codigo == rotaFrete.GrupoPessoas.Codigo))
                                    inserir = true;
                            }
                            else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                            {
                                cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;

                                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (cargaPedido.Pedido.Destinatario?.GrupoPessoas != null && rotaFrete.GrupoPessoas != null && cargaPedido.Pedido.Destinatario.GrupoPessoas.Codigo == rotaFrete.GrupoPessoas.Codigo))
                                    inserir = true;
                            }
                            else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                            {
                                cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (cargaPedido.Tomador?.GrupoPessoas != null && rotaFrete.GrupoPessoas != null && cargaPedido.Tomador.GrupoPessoas.Codigo == rotaFrete.GrupoPessoas.Codigo))
                                    inserir = true;
                            }

                            if (inserir)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete cargaPedidoRotaFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete()
                                {
                                    CargaPedido = cargaPedido,
                                    RotaFrete = rotaFrete
                                };

                                repCargaPedidoRotaFrete.Inserir(cargaPedidoRotaFrete);
                            }
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, null, "Atualizou a Rota.", unitOfWork);

                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }

                carga.DataInicioCalculoFrete = DateTime.Now;
                carga.CalculandoFrete = true;
                repCarga.Atualizar(carga);
                string retornoMontagem = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).CalcularFreteTodoCarregamento(carga);

                if (!string.IsNullOrWhiteSpace(retornoMontagem))
                    throw new ControllerException(retornoMontagem);

                Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.CalculandoFrete };

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Atualizou a rota.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RetornoFrete = retornoFrete });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
