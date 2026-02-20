using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using Utilidades.Extensions;


namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CargaJanelaCarregamentoTransportadorChecklist", "Logistica/JanelaCarregamentoTransportador", "Logistica/AcompanhamentoChecklist")]
    public class CargaJanelaCarregamentoTransportadorChecklistController : BaseController
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportadorChecklistController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> SalvarChecklist()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist repCargaJanelaCarregamentoTransportadorChecklist = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);

                int codigoCargaJanelaCarregamento = Request.GetIntParam("CodigoJanelaCarregamentoTransportador");
                int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                if (cargaJanelaCarregamentoTransportador == null)
                    throw new ControllerException("Carga não encontrada");

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    throw new ControllerException("Veiculo não encontrado");

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> listaCargaJanelaCarregamentoTransportadorChecklist = Request.Params("Checklist").FromJson<List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist>>();

                unitOfWork.Start();

                bool inserir = false;

                List<(int codigoCheclist, int ordem)> codigosRetornar = new List<(int codigoCheclist, int ordem)>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist cargaJanelaCarregamentoTransportadorChecklist in listaCargaJanelaCarregamentoTransportadorChecklist)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigo(cargaJanelaCarregamentoTransportadorChecklist.GrupoProduto.Codigo);

                    if (grupoProduto == null)
                        throw new ControllerException($"Groupo Produto da {cargaJanelaCarregamentoTransportadorChecklist.OrdemCargaChecklist.ObterDescricao()} não foi encontrado");

                    if (grupoProduto.NaoPermitirCarregamento)
                    {
                        throw new ControllerException($"Groupo Produto da {cargaJanelaCarregamentoTransportadorChecklist.OrdemCargaChecklist.ObterDescricao()} não é permitido carregamento");
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist checklist = repCargaJanelaCarregamentoTransportadorChecklist.BuscarPorCodigo(cargaJanelaCarregamentoTransportadorChecklist.CodigoChecklist, false);

                    inserir = false;

                    if (checklist == null)
                    {
                        checklist = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist
                        {
                            CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                            Veiculo = veiculo,
                        };

                        inserir = true;
                    }

                    checklist.DataChecklist = cargaJanelaCarregamentoTransportadorChecklist.DataChecklist.ToDateTime();
                    checklist.RegimeLimpeza = cargaJanelaCarregamentoTransportadorChecklist.RegimeLimpeza;
                    checklist.OrdemCargaChecklist = cargaJanelaCarregamentoTransportadorChecklist.OrdemCargaChecklist;
                    checklist.GrupoProduto = grupoProduto;

                    if (inserir)
                        repCargaJanelaCarregamentoTransportadorChecklist.Inserir(checklist);
                    else
                        repCargaJanelaCarregamentoTransportadorChecklist.Atualizar(checklist);

                    codigosRetornar.Add(new(checklist.Codigo, (int)checklist.OrdemCargaChecklist));

                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Inseriu = inserir,
                    Checklist = codigosRetornar
                });
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoChecklist");
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoChecklist");
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarChecklist);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosChecklistVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist servicoCargaJanelaCarregamentoTransportadorChecklist = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist(unitOfWork);

                int codigoCargaJanelaCarregamento = Request.GetIntParam("CodigoJanelaCarregamentoTransportador");
                int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                if (cargaJanelaCarregamentoTransportador == null)
                    throw new ControllerException("Janela Carregamento não encontrada");

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    throw new ControllerException("Veiculo não encontrado");

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> cargaJanelaCarregamentoTransportadorChecklist = servicoCargaJanelaCarregamentoTransportadorChecklist.ObterChecklist(codigoCargaJanelaCarregamento, codigoVeiculo);

                var retorno = new
                {
                    CargaJanelaCarregamentoTransportadorChecklist = cargaJanelaCarregamentoTransportadorChecklist
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoTransportadorChecklist");
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoTransportadorChecklist");
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarChecklist);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosAcompanhamentoChecklist()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist servicoCargaJanelaCarregamentoTransportadorChecklist = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist(unitOfWork);

                int codigoCargaJanelaCarregamento = Request.GetIntParam("CodigoJanelaCarregamentoTransportador");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                if (cargaJanelaCarregamentoTransportador == null)
                    throw new ControllerException("Janela Carregamento não encontrada");

                List<int> codigosVeiculo = repCargaJanelaCarregamentoTransportador.BuscarVeiculosPorCodigoCarga(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo);

                if (codigosVeiculo == null)
                    throw new ControllerException("Veiculos não encontrados");

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> cargaJanelaCarregamentoTransportadorChecklist = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist>();

                foreach (int codigoVeiculo in codigosVeiculo)
                {
                    cargaJanelaCarregamentoTransportadorChecklist.AddRange(servicoCargaJanelaCarregamentoTransportadorChecklist.ObterChecklist(codigoCargaJanelaCarregamento, codigoVeiculo));
                }

                var retorno = new
                {
                    CargaJanelaCarregamentoTransportadorChecklist = cargaJanelaCarregamentoTransportadorChecklist,
                    CodigosVeiculos = codigosVeiculo
                };

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoTransportadorChecklist");
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "CargaJanelaCarregamentoTransportadorChecklist");
                return new JsonpResult(false, Localization.Resources.Logistica.JanelaCarregamentoTransportador.OcorreuUmaFalhaAoSalvarChecklist);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados


        #endregion Métodos Privados


    }
}
