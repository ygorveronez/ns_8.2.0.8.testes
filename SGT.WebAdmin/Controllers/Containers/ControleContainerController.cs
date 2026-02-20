using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Containers
{
    [CustomAuthorize("Containers/ControleContainer", "Containers/ContainerRedex")]
    public class ControleContainerController : BaseController
    {
		#region Construtores

		public ControleContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisa(unitOfWork, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisa(unitOfWork, false);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaLocalRetiradaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoTipoContainer", false);
                grid.AdicionarCabecalho("Local de Retirada", "Descricao", tamanho: 26m, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Tipo do Container", "TipoContainer", tamanho: 18m, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Quantidade", "QuantidadeFormatada", tamanho: 12m, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Quantidade Reservada", "QuantidadeReservadaFormatada", tamanho: 17m, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Quantidade Disponível", "QuantidadeDisponivelFormatada", tamanho: 17m, Models.Grid.Align.center, false, false, false, false, true);

                int codigoContainerTipo = Request.GetIntParam("ContainerTipo");
                double cpfCnpjLocal = Request.GetDoubleParam("Local");
                bool mostrarTodosLocais = Request.GetBoolParam("MostraTodosLocais");

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.LocalRetiradaContainer> locaisRetiradaContainer = repositorioColetaContainer.BuscarLocaisRetiradaContainer(cpfCnpjLocal, codigoContainerTipo, mostrarTodosLocais);
                int totalRegistros = locaisRetiradaContainer.Count;

                grid.AdicionaRows(locaisRetiradaContainer);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os locais de retirada de container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoColetaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisaHistoricos(unitOfWork, false);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarHistoricoColetaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisaHistoricos(unitOfWork, false);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterMultiplasEtapasDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoColetaContainer = Request.GetIntParam("CodigoColetaContainer");

                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetarContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetarContainer.BuscarPorCodigo(codigoColetaContainer);

                if (coletaContainer == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao obter o Container");

                return new JsonpResult(coletaContainer.Status.ObterSituacoesAnteriores());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os Status do Container");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarJustificativaColetaContainer()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigoColetaContainer = Request.GetIntParam("Codigo");
                int codigoJustificativaContainer = Request.GetIntParam("Justificativa");
                StatusColetaContainer statusColetaContainer = Request.GetEnumParam<StatusColetaContainer>("StatusContainer");

                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa repositorioColetaContainerJustificativa = new Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa(unitOfWork);
                Repositorio.Embarcador.Pedidos.JustificativaContainer repositorioJustificativaContainer = new Repositorio.Embarcador.Pedidos.JustificativaContainer(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCodigo(codigoColetaContainer, true);
                Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer justificativaContainer = repositorioJustificativaContainer.BuscarPorCodigo(codigoJustificativaContainer, true);

                if (coletaContainer == null)
                    return new JsonpResult(false, "Coleta container não encontrada.");

                if (repositorioColetaContainerJustificativa.PossuiRegistroPorColetaContainerEStatus(codigoColetaContainer, statusColetaContainer))
                    return new JsonpResult(true, false, "Já existe uma Justificativa cadastrada para está situação!");
                else
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa coletaContainerJustificativa = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa
                    {
                        ColetaContainer = coletaContainer,
                        Status = statusColetaContainer,
                        DataJustificativa = DateTime.Now,
                        Usuario = Usuario,
                        JustificativaContainer = justificativaContainer,
                        JustificativaDescritiva = Request.GetStringParam("JustificativaDescritiva")
                    };

                    repositorioColetaContainerJustificativa.Inserir(coletaContainerJustificativa);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Justificativa alterada com sucesso.");

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a justificativa da coleta container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterColetaContainerJustificativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisaHistoricosJustificativasContainer(unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorCodigo(codigo);

                if (coletaContainer != null)
                {
                    var dynRetorno = new
                    {
                        Codigo = coletaContainer.Codigo,
                        Container = coletaContainer.Container != null ? new { Codigo = coletaContainer.Container.Codigo, Descricao = coletaContainer.Container.Numero } : null,
                        LocalColeta = coletaContainer.LocalColeta != null ? new { Codigo = coletaContainer.LocalColeta.Codigo, Descricao = coletaContainer.LocalColeta.NomeCNPJ } : null,
                        StatusContainer = coletaContainer.Status,
                        DiasFreeTime = coletaContainer.FreeTime,
                        ValorDiaria = coletaContainer.ValorDiaria,
                        DataEmbarque = coletaContainer.DataEmbarque.HasValue ? coletaContainer.DataEmbarque.Value.ToString("dd/MM/yyyy") : "",
                        DataColeta = coletaContainer.DataColeta.HasValue ? coletaContainer.DataColeta.Value.ToString("dd/MM/yyyy") : ""
                    };

                    return new JsonpResult(dynRetorno, true, "Sucesso");
                }

                return new JsonpResult(null, true, "Sucesso");
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

        public async Task<IActionResult> AtualizarColetaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Containers/ControleContainer");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ControleContainer_PermiteMovimentarContainer))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCodigo(Request.GetIntParam("Codigo"));

                if (coletaContainer == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ControleContainer_PermiteMovimentarContainer))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametros = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                parametros.coletaContainer = coletaContainer;
                parametros.Status = Request.GetEnumParam<StatusColetaContainer>("StatusContainer");
                parametros.DiasFreeTime = Request.GetIntParam("DiasFreeTime");
                parametros.ValorDiaria = Request.GetDecimalParam("ValorDiaria");
                parametros.DataAtualizacao = DateTime.Now;
                parametros.OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.UsuarioInterno;
                parametros.InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.AtualizacaoControleContainer;
                parametros.Usuario = this.Usuario;

                if (Request.GetDoubleParam("LocalColeta") > 0)
                    parametros.LocalColeta = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("LocalColeta"));

                if (Request.GetIntParam("Container") > 0)
                {
                    var container = repositorioContainer.BuscarPorCodigo(Request.GetIntParam("Container"));

                    if (container != null && coletaContainer.Container != null && coletaContainer.Container.Codigo != container.Codigo)//esta alterando
                    {
                        //validar se a carga atual ou carga de coleta é do tipo operacao coleta container

                        var isColeta = false;
                        if (coletaContainer.CargaAtual != null && (coletaContainer.CargaAtual.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false))
                            isColeta = true;
                        else if (coletaContainer.CargaAtual == null && coletaContainer.CargaDeColeta != null && (coletaContainer.CargaDeColeta.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false))
                            isColeta = true;

                        if (isColeta)
                            return new JsonpResult(null, true, "Não é permitido a troca de container nesta tela. A alteração do container deverá ser feita pela tela: Gestão de Cargas - Cargas.");

                    }

                    parametros.Container = container;
                }


                if (parametros.Status == StatusColetaContainer.Porto)
                    parametros.DataEmbarque = Request.GetDateTimeParam("DataEmbarque");

                if (Request.GetDateTimeParam("DataColeta") != DateTime.MinValue)
                    parametros.DataColeta = Request.GetDateTimeParam("DataColeta");

                servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametros);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, parametros.coletaContainer, "Atualização da coleta Container realizada manualmente", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Atualização realizada.");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao Atualização a coleta container");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> MovimentarContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Containers/ControleContainer");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ControleContainer_PermiteMovimentarContainer))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametros = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                parametros.coletaContainer = repositorioColetaContainer.BuscarPorCodigo(Request.GetIntParam("Codigo"));

                bool transferirContainer = Request.GetBoolParam("TransferirContainerOutraCarga");
                string cargaTransferidaAntiga = parametros.coletaContainer?.CargaAtual?.CodigoCargaEmbarcador;
                int codigocargaTransferidaAntiga = parametros.coletaContainer?.CargaAtual?.Codigo ?? 0;

                Dominio.Entidades.Embarcador.Cargas.Carga cargaNova = new Dominio.Entidades.Embarcador.Cargas.Carga();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga = new Dominio.Entidades.Embarcador.Cargas.Carga();

                if (transferirContainer)
                {
                    int cargaNovaAserTransferida = Request.GetIntParam("CargaTransferida");
                    if (cargaNovaAserTransferida > 0 && parametros.coletaContainer != null)
                    {
                        cargaNova = repCarga.BuscarPorCodigo(cargaNovaAserTransferida);
                        cargaAntiga = repCarga.BuscarPorCodigo(codigocargaTransferidaAntiga);

                        //validar se tem retirada container na carga nova
                        Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainerCargaNova = repositorioRetiradaContainer.BuscarPorCarga(cargaNova.Codigo);
                        if (retiradaContainerCargaNova != null && retiradaContainerCargaNova.Container == null)
                        {
                            retiradaContainerCargaNova.Container = parametros.coletaContainer.Container;
                            retiradaContainerCargaNova.ColetaContainer = parametros.coletaContainer;
                            repositorioRetiradaContainer.Atualizar(retiradaContainerCargaNova);
                        }
                        else if (retiradaContainerCargaNova != null && retiradaContainerCargaNova.Container != null)
                            return new JsonpResult(false, true, $"Movimentação de transferência de container não permitida a carga : {cargaNova.CodigoCargaEmbarcador} já possuí container selecionado na retirada container ");

                        if (retiradaContainerCargaNova == null)
                        {
                            //vamos criar uma retiradaContainer para a carga nova..
                            retiradaContainerCargaNova = new Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer();
                            retiradaContainerCargaNova.Carga = cargaNova;
                            retiradaContainerCargaNova.Container = parametros.coletaContainer.Container;
                            retiradaContainerCargaNova.ColetaContainer = parametros.coletaContainer;
                            retiradaContainerCargaNova.Local = parametros.coletaContainer.LocalAtual;
                            retiradaContainerCargaNova.ContainerTipo = parametros.coletaContainer.Container.ContainerTipo;
                            repositorioRetiradaContainer.Inserir(retiradaContainerCargaNova);
                        }

                        Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainerCargaNovaAtual = repositorioColetaContainer.BuscarPorCargaAtual(cargaNova.Codigo);
                        if (coletaContainerCargaNovaAtual != null && coletaContainerCargaNovaAtual.Container == null)
                        {
                            coletaContainerCargaNovaAtual.Container = parametros.coletaContainer.Container;
                            coletaContainerCargaNovaAtual.CargaAtual = cargaNova;
                            repositorioColetaContainer.Atualizar(coletaContainerCargaNovaAtual);
                        }
                        else if (coletaContainerCargaNovaAtual != null && coletaContainerCargaNovaAtual.Container != null)
                            return new JsonpResult(false, true, $"Movimentação de transferência de container não permitida a carga de coleta : {cargaNova.CodigoCargaEmbarcador} já possuí container selecionado ");


                        Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainerCargaAntiga = repositorioRetiradaContainer.BuscarPorCarga(cargaAntiga.Codigo);
                        if (retiradaContainerCargaAntiga != null && retiradaContainerCargaAntiga.Container != null)
                        {
                            retiradaContainerCargaAntiga.Container = null;

                            if (retiradaContainerCargaAntiga.ColetaContainer != null)
                            {
                                retiradaContainerCargaAntiga.ColetaContainer.CargaAtual = null;
                                repositorioColetaContainer.Atualizar(retiradaContainerCargaAntiga.ColetaContainer);

                                retiradaContainerCargaAntiga.ColetaContainer = null;
                            }

                            repositorioRetiradaContainer.Atualizar(retiradaContainerCargaAntiga);
                        }

                        Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainerCargaAntiga = repositorioColetaContainer.BuscarPorCargaAtual(cargaAntiga.Codigo);
                        if (coletaContainerCargaAntiga != null && coletaContainerCargaAntiga.Container != null)
                        {
                            coletaContainerCargaAntiga.Container = null;
                            repositorioColetaContainer.Atualizar(coletaContainerCargaAntiga);
                        }

                        parametros.coletaContainer.CargaAtual = repCarga.BuscarPorCodigo(cargaNovaAserTransferida);
                    }
                }

                int codigoCargaExpRetirada = Request.GetIntParam("CargaExpRetirada");

                parametros.Status = Request.GetEnumParam<StatusColetaContainer>("Situacao");
                parametros.DataAtualizacao = Request.GetDateTimeParam("DataMovimentacao");
                parametros.Usuario = this.Usuario;
                parametros.OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.AlteracaoManual;
                parametros.InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.MovimentacaoControleContainer;

                if (parametros.Status == StatusColetaContainer.Porto)
                {
                    parametros.DataEmbarque = Request.GetDateTimeParam("DataEmbarque");
                    parametros.LocalEmbarque = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("LocalEmbarque"));
                    parametros.LocalAtual = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("LocalEmbarque"));
                }
                else if (parametros.Status == StatusColetaContainer.EmbarcadoNavio)
                {
                    parametros.DataEmbarqueNavio = Request.GetDateTimeParam("DataEmbarqueNavio");
                }
                else if (parametros.Status == StatusColetaContainer.EmAreaEsperaCarregado || parametros.Status == StatusColetaContainer.EmAreaEsperaVazio || parametros.Status == StatusColetaContainer.EmCarregamento)
                    parametros.LocalAtual = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("LocalAtual"));

                if (codigoCargaExpRetirada > 0 && parametros.coletaContainer != null)
                {
                    parametros.coletaContainer.CargaAtual = repCarga.BuscarPorCodigo(codigoCargaExpRetirada);

                    servicoColetaContainer.ValidarTipoOperacaoCargaDuplicado(parametros.coletaContainer);
                    repositorioColetaContainer.Atualizar(parametros.coletaContainer);
                }

                servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametros);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, parametros.coletaContainer, "Movimentação do Container realizada manualmente", unitOfWork);

                if (transferirContainer)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, parametros.coletaContainer, $"Transferiu container manualmente da carga: {cargaTransferidaAntiga} para a carga: {parametros.coletaContainer?.CargaAtual?.CodigoCargaEmbarcador}", unitOfWork);

                    if (cargaAntiga != null)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaAntiga, $"Transferiu container manualmente da carga: {cargaTransferidaAntiga} para a carga: {parametros.coletaContainer?.CargaAtual?.CodigoCargaEmbarcador}", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Movimentação realizada.");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao movimentar o container");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarJustificativaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return BuscarJustificativa(unitOfWork);
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

        public async Task<IActionResult> ExcluirJustificativaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ExcluirJustificativa(unitOfWork);
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

        public async Task<IActionResult> AtualizarJustificativaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return AtualizarJustificativa(unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar atualizar a Justificativa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private IActionResult ObterGridPesquisaHistoricos(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {

            int CodigoColetaContainer = Request.GetIntParam("Codigo");

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data Historico", "DataHistorico", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tempo Historico", "TempoHistorico", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Auditado", "Auditado", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Area Atual", "ClienteAtual", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status", "SituacaoContainer", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Origem Movimentação", "Origem", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Informação da Movimentação", "InformacaoOrigem", 15, Models.Grid.Align.left, false);

            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
            Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repColetaContainerHistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico> listColetaContainerHistoricos = repColetaContainerHistorico.BuscarPorColetaContainer(CodigoColetaContainer);

            dynamic lista = (from historico in listColetaContainerHistoricos
                             select new
                             {
                                 historico.Codigo,
                                 Carga = historico.ObterCodigoCargaEmbarcador(),
                                 Auditado = historico.Usuario != null ? historico.Usuario.Nome : "",
                                 ClienteAtual = historico.Local?.NomeCNPJ ?? "",
                                 SituacaoContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainerHelper.ObterDescricao(historico.Status),
                                 DataHistorico = historico.DataHistorico.ToString("dd/MM/yyyy HH:mm:ss"),
                                 TempoHistorico = historico.DataFimHistorico.HasValue && historico.DataFimHistorico != DateTime.MinValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(historico.DataFimHistorico.Value - historico.DataHistorico) : "",
                                 Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainerHelper.ObterDescricao(historico.OrigemMovimentacao),
                                 InformacaoOrigem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.InformacaoOrigemMovimentacaoContainerHelper.ObterDescricao(historico.InformacaoOrigemMovimentacao),
                             }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(listColetaContainerHistoricos.Count);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private IActionResult ObterGridPesquisaHistoricosJustificativasContainer(Repositorio.UnitOfWork unitOfWork)
        {

            int CodigoColetaContainer = Request.GetIntParam("Codigo");

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Justificativa", "JustificativaContainer", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Justificativa Descritiva", "JustificativaDescritiva", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Historico", "DataHistorico", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Auditado", "Auditado", 6, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status", "SituacaoContainer", 6, Models.Grid.Align.left, false);

            Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa repColetaContainerJustificativa = new Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa> listColetaContainerHistoricosjustificativas = repColetaContainerJustificativa.BuscarPorColetaContainer(CodigoColetaContainer);

            dynamic lista = (from o in listColetaContainerHistoricosjustificativas
                             select new
                             {
                                 o.Codigo,
                                 DataHistorico = o.DataJustificativa.ToString("dd/MM/yyyy HH:mm:ss"),
                                 Auditado = o.Usuario != null ? o.Usuario.Nome : "",
                                 SituacaoContainer = StatusColetaContainerHelper.ObterDescricao(o.Status),
                                 o.JustificativaDescritiva,
                                 JustificativaContainer = o.JustificativaContainer.Descricao
                             }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(listColetaContainerHistoricosjustificativas.Count);

            return new JsonpResult(grid);
        }

        private IActionResult BuscarJustificativa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("Codigo");

            Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa repositorioColetaContainerJustificativa = new Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa coletaContainerJustificativa = repositorioColetaContainerJustificativa.BuscarPorCodigo(codigo);

            if (coletaContainerJustificativa == null)
                return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

            var dynColetaContainerJustificativa = new
            {
                coletaContainerJustificativa.Codigo,
                Justificativa = new { Codigo = coletaContainerJustificativa.JustificativaContainer?.Codigo ?? 0, Descricao = coletaContainerJustificativa.JustificativaContainer?.Descricao ?? string.Empty },
                coletaContainerJustificativa.JustificativaDescritiva,
                SituacaoContainer = coletaContainerJustificativa.Status
            };

            return new JsonpResult(dynColetaContainerJustificativa);
        }

        private IActionResult ExcluirJustificativa(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();

            int codigo = Request.GetIntParam("Codigo");

            Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa repositorioColetaContainerJustificativa = new Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa coletaContainerJustificativa = repositorioColetaContainerJustificativa.BuscarPorCodigo(codigo);

            if (coletaContainerJustificativa == null)
                return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

            repositorioColetaContainerJustificativa.Deletar(coletaContainerJustificativa, Auditado);

            unitOfWork.CommitChanges();

            return new JsonpResult(true);
        }

        private IActionResult AtualizarJustificativa(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();

            int codigo = Request.GetIntParam("Codigo");
            int codigoJustificativaContainer = Request.GetIntParam("Justificativa");
            StatusColetaContainer statusColetaContainer = Request.GetEnumParam<StatusColetaContainer>("StatusContainer");

            Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa repositorioColetaContainerJustificativa = new Repositorio.Embarcador.Pedidos.ColetaContainerJustificativa(unitOfWork);
            Repositorio.Embarcador.Pedidos.JustificativaContainer repositorioJustificativaContainer = new Repositorio.Embarcador.Pedidos.JustificativaContainer(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ColetaContainerJustificativa coletaContainerJustificativa = repositorioColetaContainerJustificativa.BuscarPorCodigo(codigo);
            Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer justificativaContainer = repositorioJustificativaContainer.BuscarPorCodigo(codigoJustificativaContainer, true);

            if (coletaContainerJustificativa == null)
                return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

            coletaContainerJustificativa.JustificativaContainer = justificativaContainer;
            coletaContainerJustificativa.Status = statusColetaContainer;
            coletaContainerJustificativa.DataJustificativa = DateTime.Now;
            coletaContainerJustificativa.Usuario = Usuario;
            coletaContainerJustificativa.JustificativaDescritiva = Request.GetStringParam("JustificativaDescritiva");

            repositorioColetaContainerJustificativa.Atualizar(coletaContainerJustificativa, Auditado);

            unitOfWork.CommitChanges();

            return new JsonpResult(true);
        }

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtrosPesquisa = ObterFiltrosPesquisa();
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", false);
            grid.AdicionarCabecalho("CodigoTipoContainer", false);
            grid.AdicionarCabecalho("StatusContainer", false);
            grid.AdicionarCabecalho("Carga", "CargaEmbarcador", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Nº Carga Agrupada", "NumeroCargaAgrupada", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Container", "NumeroContainer", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo do Container", "TipoContainer", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status", "SituacaoContainer", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Movimentacao", "DataMovimentacao", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Coleta", "DataColeta", 6, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Porto", "DataPorto", 6, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Embarque No Navio", "DataEmbarqueNavio", 6, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Justificativa Descritiva", "JustificativaDescritiva", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Dias Free Time", "DiasFreeTime", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Dias de Posse", "DiasPosse", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Nº EXP", "NumeroEXP", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Pedido", "Pedido", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Filial", "FilialCargaAtual", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Excedeu Free Time", "ExcedeuFreeTime", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Area Coleta", "ClienteColeta", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Area Atual", "ClienteAtual", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor Diarias", "ValorDevido", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Área espera vazio", "AreaEsperaVazio", 10, Models.Grid.Align.left, false, false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ControleContainer/Pesquisa", "grid-controle-container");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repColetaContainer.ContarContainersPesquisa(filtrosPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer> coletaContainerEmPosseEmbarcador = (totalRegistros > 0) ? repColetaContainer.BuscarContainersPesquisa(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer>();

            dynamic lista = (from coletacontainer in coletaContainerEmPosseEmbarcador
                             select new
                             {
                                 coletacontainer.Codigo,
                                 Carga = coletacontainer.CargaAtual > 0 ? coletacontainer.CargaAtual : coletacontainer.Carga,
                                 coletacontainer.CargaEmbarcador,
                                 coletacontainer.NumeroContainer,
                                 coletacontainer.TipoContainer,
                                 NumeroBooking = !string.IsNullOrEmpty(coletacontainer.NumeroBooking) ? coletacontainer.NumeroBooking : coletacontainer.NumeroBookingAgrupada,
                                 Pedido = !string.IsNullOrEmpty(coletacontainer.Pedido) ? coletacontainer.Pedido : coletacontainer.PedidoAgrupado,
                                 NumeroEXP = !string.IsNullOrEmpty(coletacontainer.NumeroEXP) ? coletacontainer.NumeroEXP : coletacontainer.NumeroEXPAgrupada,
                                 FilialCargaAtual = !string.IsNullOrEmpty(coletacontainer.CNPJFilialCargaAtual) ? (!string.IsNullOrEmpty(coletacontainer.CNPJFilialCargaOrigem) ? coletacontainer.FilialCargaAtual + " (" + coletacontainer.CNPJFilialCargaAtual + ")" + " - " + coletacontainer.FilialCargaOrigem + " (" + coletacontainer.CNPJFilialCargaOrigem + ")" : coletacontainer.FilialCargaAtual + " (" + coletacontainer.CNPJFilialCargaAtual + ")") : "",
                                 DataColeta = coletacontainer.DataColeta != DateTime.MinValue ? coletacontainer.DataColeta.ToString("dd/MM/yyyy") : "",
                                 SituacaoContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainerHelper.ObterDescricao(coletacontainer.Status),
                                 StatusContainer = coletacontainer.Status,
                                 DataMovimentacao = coletacontainer.DataUltimaMovimentacao.ToString("dd/MM/yyyy"),
                                 ClienteColeta = coletacontainer.LocalColeta > 0 ? coletacontainer.ClienteLocalColeta + "(" + coletacontainer.LocalColeta + ")" : "",
                                 ClienteAtual = coletacontainer.LocalAtual > 0 ? coletacontainer.ClienteLocalAtual + "(" + coletacontainer.LocalAtual + ")" : "",
                                 LocalEmbarque = coletacontainer.LocalEmbarque > 0 ? coletacontainer.ClienteLocalEmbarque + "(" + coletacontainer.LocalEmbarque + ")" : "",
                                 ValorDevido = coletacontainer.ValorDevido > 0 ? coletacontainer.ValorDevido.ToString("N2") : "0,00",
                                 DiasPosse = coletacontainer.DiasEmPosse,
                                 coletacontainer.Justificativa,
                                 coletacontainer.JustificativaDescritiva,
                                 ExcedeuFreeTime = coletacontainer.DiasExcesso > 0 ? "Sim" : "Não",
                                 DataPorto = coletacontainer.DataEmbarque != DateTime.MinValue ? coletacontainer.DataEmbarque.ToString("dd/MM/yyyy") : "",
                                 DiasFreeTime = coletacontainer.FreeTime,
                                 NumeroCargaAgrupada = coletacontainer.NumeroCargaAgrupada,
                                 coletacontainer.AreaEsperaVazio,
                                 coletacontainer.CodigoTipoContainer,
                                 DataEmbarqueNavio = coletacontainer.DataEmbarqueNavio != DateTime.MinValue ? coletacontainer.DataEmbarqueNavio.ToString("d") : "",
                             }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer ObterFiltrosPesquisa()
        {

            Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                DataEmbarque = Request.GetDateTimeParam("DataEmbarque"),
                DataFinalColeta = Request.GetDateTimeParam("DataFinalColeta"),
                DataInicialColeta = Request.GetDateTimeParam("DataInicialColeta"),
                DataUltimaMovimentacao = Request.GetDateTimeParam("DataUltimaMovimentacao"),
                DiasPosse = Request.GetIntParam("DiasPosseInicio"),
                DiasPosseFim = Request.GetIntParam("DiasPosseFim"),
                LocalAtual = Request.GetDoubleParam("LocalAtual"),
                LocalColeta = Request.GetDoubleParam("LocalColeta"),
                LocalEsperaVazio = Request.GetDoubleParam("LocalEsperaVazio"),
                NumeroContainer = Request.GetStringParam("NumeroContainer"),
                SomenteExcedidos = Request.GetBoolParam("SomenteExcedidos"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                TipoContainer = Request.GetIntParam("TipoContainer"),
                FilialAtual = Request.GetIntParam("FilialAtual"),
                StatusContainer = Request.GetNullableEnumParam<StatusColetaContainer>("StatusContainer"),
                DataEmbarqueNavioInicial = Request.GetDateTimeParam("DataEmbarqueNavioInicial"),
                DataEmbarqueNavioFinal = Request.GetDateTimeParam("DataEmbarqueNavioFinal")
            };

            return filtrosPesquisa;
        }

        #endregion Métodos Privados
    }
}
