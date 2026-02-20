using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize(new string[] { "BuscarLocalArmazenamentoAtual", "VerificarSeExisteLocalArmazenagemNFeAtualComEspaco",
        "VerificarSeExisteLocalArmazenagemAvulsoAtualComEspaco" },
        "Canhotos/LocalArmazenamentoCanhoto")]
    public class LocalArmazenamentoCanhotoController : BaseController
    {
		#region Construtores

		public LocalArmazenamentoCanhotoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto tipoCanhoto = !string.IsNullOrEmpty(Request.Params("TipoCanhoto")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto)int.Parse(Request.Params("TipoCanhoto")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Todos;
                int filial = Request.GetIntParam("Filial");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Tipo de Canhoto", "TipoCanhoto", 15, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Data de Criação", "DataCriacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Capacidade", "CapacidadeArmazenagem", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Quant. Armazenada", "QuantidadeArmazenada", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Pacote Atual", "PacoteAtual", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Local Atual", "LocalAtual", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto> listaLocalArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.Consultar(descricao, tipoCanhoto, filial, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repLocalArmazenamentoCanhoto.ContarConsulta(descricao, tipoCanhoto, filial));

                var lista = (from p in listaLocalArmazenamentoCanhoto
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CapacidadeArmazenagem,
                                 p.QuantidadeArmazenada,
                                 PacoteAtual = p.DividirEmPacotesDe > 0 ? p.PacoteAtual.ToString() : "-",
                                 DataCriacao = p.DataCadastro.ToString("dd/MM/yyyy"),
                                 LocalAtual = p.LocalArmazenagemAtual ? "Sim" : "Não",
                                 TipoCanhoto = p.DescricaoTipoCanhoto,
                                 DT_RowColor = p.LocalArmazenagemAtual ? "#dff0d8" : ""
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = new Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto();
                bool localArmazenagemPadrao = bool.Parse(Request.Params("LocalArmazenagemAtual"));
                localArmazenamentoCanhoto.Descricao = Request.Params("Descricao");
                localArmazenamentoCanhoto.CapacidadeArmazenagem = int.Parse(Request.Params("CapacidadeArmazenagem"));
                localArmazenamentoCanhoto.Filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));

                int dividirEmPacotesDe = 0;
                int.TryParse(Request.Params("DividirEmPacotesDe"), out dividirEmPacotesDe);

                localArmazenamentoCanhoto.DividirEmPacotesDe = dividirEmPacotesDe;
                if (localArmazenamentoCanhoto.DividirEmPacotesDe > 0)
                    localArmazenamentoCanhoto.PacoteAtual = 1;

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    localArmazenamentoCanhoto.TipoCanhoto = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto)int.Parse(Request.Params("TipoCanhoto"));

                localArmazenamentoCanhoto.Observacao = Request.Params("Observacao");
                localArmazenamentoCanhoto.DataCadastro = DateTime.Now;

                if (localArmazenamentoCanhoto.DividirEmPacotesDe > 0)
                {
                    int result = localArmazenamentoCanhoto.CapacidadeArmazenagem % localArmazenamentoCanhoto.DividirEmPacotesDe;
                    if (result > 0)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A quantidade informada de cada pacote não é uma divisão exata da capacidade de armazenamento.");
                    }
                    if (dividirEmPacotesDe > localArmazenamentoCanhoto.CapacidadeArmazenagem)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é possível informar pacotes maiores que a capacidade de armazenagem.");
                    }
                }

                if (ConfiguracaoEmbarcador.ArmazenamentoCanhotoComFilial && localArmazenamentoCanhoto.Filial == null)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "É obrigatório informar uma filial.");
                }

                if (localArmazenagemPadrao)
                {
                    Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoAtual = null;

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();
                    else if (ConfiguracaoEmbarcador.ArmazenamentoCanhotoComFilial)
                        localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtualPorTipoFilial(localArmazenamentoCanhoto.TipoCanhoto.Value, localArmazenamentoCanhoto.Filial.Codigo); 
                    else
                        localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(localArmazenamentoCanhoto.TipoCanhoto.Value);

                    if (localArmazenamentoAtual != null)
                    {
                        localArmazenamentoAtual.Initialize();
                        localArmazenamentoAtual.LocalArmazenagemAtual = false;
                        repLocalArmazenamentoCanhoto.Atualizar(localArmazenamentoAtual, Auditado);
                    }
                }

                localArmazenamentoCanhoto.LocalArmazenagemAtual = localArmazenagemPadrao;
                repLocalArmazenamentoCanhoto.Inserir(localArmazenamentoCanhoto, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                bool localArmazenagemPadrao = bool.Parse(Request.Params("LocalArmazenagemAtual"));
                localArmazenamentoCanhoto.Descricao = Request.Params("Descricao");
                localArmazenamentoCanhoto.CapacidadeArmazenagem = int.Parse(Request.Params("CapacidadeArmazenagem"));
                localArmazenamentoCanhoto.Filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    localArmazenamentoCanhoto.TipoCanhoto = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto)int.Parse(Request.Params("TipoCanhoto"));

                localArmazenamentoCanhoto.Observacao = Request.Params("Observacao");

                int dividirEmPacotesDe = 0;
                int.TryParse(Request.Params("DividirEmPacotesDe"), out dividirEmPacotesDe);

                int dividirEmPacotesDeAtual = localArmazenamentoCanhoto.DividirEmPacotesDe;
                localArmazenamentoCanhoto.DividirEmPacotesDe = dividirEmPacotesDe;

                if (localArmazenamentoCanhoto.CapacidadeArmazenagem >= localArmazenamentoCanhoto.QuantidadeArmazenada)
                {
                    if (dividirEmPacotesDeAtual != localArmazenamentoCanhoto.DividirEmPacotesDe)
                    {
                        if (dividirEmPacotesDeAtual > 0 || localArmazenamentoCanhoto.QuantidadeArmazenada == 0)
                        {
                            if (localArmazenamentoCanhoto.PacoteAtual <= 1)
                            {
                                if (localArmazenamentoCanhoto.DividirEmPacotesDe > 0)
                                    localArmazenamentoCanhoto.PacoteAtual = 1;

                                if (localArmazenamentoCanhoto.QuantidadeArmazenada > localArmazenamentoCanhoto.DividirEmPacotesDe)
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, "A quantidade que se deseja dividir os pacotes não pode ser inferior a quantidade já informada no primeiro pacote de " + localArmazenamentoCanhoto.QuantidadeArmazenada + ".");
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "Não é possível alterar a quantidade de pacotes pois já existem pacotes de " + dividirEmPacotesDeAtual + " canhotos finalizados neste local de armazenamento.");
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "Não é possível informar quantidades de pacotes poís já foram enviados canhotos para este local de armazenamento sem quantidade de pacotes.");
                        }
                    }

                    if (ConfiguracaoEmbarcador.ArmazenamentoCanhotoComFilial && localArmazenamentoCanhoto.Filial == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É obrigatório informar uma filial.");
                    }

                    if (localArmazenamentoCanhoto.DividirEmPacotesDe > 0)
                    {
                        int result = localArmazenamentoCanhoto.CapacidadeArmazenagem % localArmazenamentoCanhoto.DividirEmPacotesDe;
                        if (result > 0)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "A quantidade informada de cada pacote não é uma divisão exata da capacidade de armazenamento.");
                        }
                    }
                    else
                        localArmazenamentoCanhoto.PacoteAtual = 0;

                    if (localArmazenagemPadrao)
                    {
                        Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoAtual = null;

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();
                        else if (ConfiguracaoEmbarcador.ArmazenamentoCanhotoComFilial)
                            localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtualPorTipoFilial(localArmazenamentoCanhoto.TipoCanhoto.Value, localArmazenamentoCanhoto.Filial.Codigo);
                        else
                            localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(localArmazenamentoCanhoto.TipoCanhoto.Value);

                        if (localArmazenamentoAtual != null && localArmazenamentoAtual.Codigo != localArmazenamentoCanhoto.Codigo)
                        {
                            localArmazenamentoAtual.Initialize();
                            localArmazenamentoAtual.LocalArmazenagemAtual = false;
                            repLocalArmazenamentoCanhoto.Atualizar(localArmazenamentoAtual, Auditado);
                        }
                    }

                    localArmazenamentoCanhoto.LocalArmazenagemAtual = localArmazenagemPadrao;
                    repLocalArmazenamentoCanhoto.Atualizar(localArmazenamentoCanhoto, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A capacidade de armazenagem (" + localArmazenamentoCanhoto.CapacidadeArmazenagem + ") não pode ser menor que a capacidade já armazenada (" + localArmazenamentoCanhoto.QuantidadeArmazenada + ")");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarLocalArmazenamentoAtual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoAtual = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto tipoCanhoto = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto)int.Parse(Request.Params("TipoCanhoto"));

                    localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(tipoCanhoto);
                }

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                if (localArmazenamentoAtual != null)
                {
                    return new JsonpResult(serCanhoto.RetornarDadosLocalArmazenamento(localArmazenamentoAtual));
                }
                else
                {
                    return new JsonpResult(false, true, "");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoAtual = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();
                else
                    localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(localArmazenamentoCanhoto.TipoCanhoto.Value);

                var dynLocalArmazenamentoCanhoto = new
                {
                    localArmazenamentoCanhoto.Codigo,
                    localArmazenamentoCanhoto.CapacidadeArmazenagem,
                    localArmazenamentoCanhoto.Descricao,
                    localArmazenamentoCanhoto.TipoCanhoto,
                    localArmazenamentoCanhoto.Observacao,
                    localArmazenamentoCanhoto.PacoteAtual,
                    localArmazenamentoCanhoto.DividirEmPacotesDe,
                    localArmazenamentoCanhoto.QuantidadeArmazenada,
                    localArmazenamentoCanhoto.LocalArmazenagemAtual,
                    LocalAtualJaExistente = localArmazenamentoAtual != null && localArmazenamentoCanhoto.Codigo != localArmazenamentoAtual.Codigo ? true : false,
                    Filial = new
                    {
                        Codigo = localArmazenamentoCanhoto.Filial?.Codigo ?? 0,
                        Descricao = localArmazenamentoCanhoto.Filial?.Descricao ?? string.Empty
                    }
                };
                return new JsonpResult(dynLocalArmazenamentoCanhoto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarSeExisteLocalArmazenagemAtualComEspaco()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();

                if (localArmazenamentoAtual != null && localArmazenamentoAtual.CapacidadeArmazenagem > localArmazenamentoAtual.QuantidadeArmazenada)
                {
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar local de armazenagem padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarSeExisteLocalArmazenagemNFeAtualComEspaco()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe);

                if (localArmazenamentoAtual != null && localArmazenamentoAtual.CapacidadeArmazenagem > localArmazenamentoAtual.QuantidadeArmazenada)
                {
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar local de armazenagem padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarSeExisteLocalArmazenagemAvulsoAtualComEspaco()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoAtual = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso);

                if (localArmazenamentoAtual != null && localArmazenamentoAtual.CapacidadeArmazenagem > localArmazenamentoAtual.QuantidadeArmazenada)
                {
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar local de armazenagem padrão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarPorCodigo(codigo);
                repLocalArmazenamentoCanhoto.Deletar(localArmazenamentoCanhoto, Auditado);
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
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
