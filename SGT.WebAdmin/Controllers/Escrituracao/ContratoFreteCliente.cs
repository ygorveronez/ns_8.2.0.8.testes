using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/ContratoFreteCliente")]
    public class ContratoFreteClienteController : BaseController
    {
        #region Construtores

        public ContratoFreteClienteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente = new Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente();

                PreencherDados(contratoFreteCliente, unitOfWork);

                if (contratoFreteCliente.DataInicio > contratoFreteCliente.DataFim)
                    return new JsonpResult(true, Localization.Resources.Escrituracao.ContratoFreteCliente.VerifiqueDataContrato);

                if (repContratoFreteCliente.ValidarInformacoesCadastroContrato(contratoFreteCliente))
                    return new JsonpResult(true, Localization.Resources.Escrituracao.ContratoFreteCliente.ExisteRegistroNumeroContrato);

                repContratoFreteCliente.Inserir(contratoFreteCliente, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                int codigo = Request.GetIntParam("Codigo");
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/ContratoFreteCliente");

                Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente = repContratoFreteCliente.BuscarPorCodigo(codigo, true);

                if (contratoFreteCliente == null)
                    return new JsonpResult(false, "Registro não encontrado.");


                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                    return new JsonpResult(false, true, "Você não possui permissão para fazer essa alteração.");

                if (contratoFreteCliente.Fechado)
                    return new JsonpResult(false, true,"O contrato já foi fechado e não pode ser atualizado.");

                contratoFreteCliente.DataFim = Request.GetDateTimeParam("DataFimContrato");
                repContratoFreteCliente.Atualizar(contratoFreteCliente, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente = repContratoFreteCliente.BuscarPorCodigo(codigo, false);

                if (contratoFreteCliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    contratoFreteCliente.NumeroContrato,
                    contratoFreteCliente.Descricao,
                    ValorContrato = contratoFreteCliente.ValorContrato.ToString(),
                    Status = contratoFreteCliente.Fechado ? "Fechado" : "Aberto",
                    TipoOperacao = new
                    {
                        contratoFreteCliente.TipoOperacao.Codigo,
                        contratoFreteCliente.TipoOperacao.Descricao
                    },
                    Cliente = new
                    {
                        contratoFreteCliente.Cliente.Codigo,
                        contratoFreteCliente.Cliente.Descricao
                    },
                    DataInicioContrato = contratoFreteCliente.DataInicio.ToString("dd/MM/yyyy"),
                    DataFimContrato = contratoFreteCliente.DataFim.ToString("dd/MM/yyyy"),
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados 

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Numero do Contrato", "NumeroContrato", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status", "Status", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Cliente", "Cliente", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Início do Contrato", "DataInicioContrato", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Final do Contrato", "DataFinalContrato", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Fechado", false);


            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaContratoFreteCliente filtrosPesquisa = ObterFiltrosPesquisa();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repContratoFreteCliente.ContarConsulta(filtrosPesquisa);

            List<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> listaContratoFreteCliente = totalRegistros > 0 ? repContratoFreteCliente.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente>();

            var lista = (from p in listaContratoFreteCliente
                         select new
                         {
                             p.Codigo,
                             p.NumeroContrato,
                             p.Descricao,
                             Cliente = p.Cliente.Nome,
                             TipoOperacao = p.TipoOperacao.Descricao,
                             DataInicioContrato = p.DataInicio.ToString("dd/MM/yyyy"),
                             DataFinalContrato = p.DataFim.ToString("dd/MM/yyyy"),
                             p.Fechado,
                             Status = p.Fechado ? "Fechado" : "Aberto"
                         }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            contratoFreteCliente.NumeroContrato = Request.GetStringParam("NumeroContrato");
            contratoFreteCliente.Descricao = Request.GetStringParam("Descricao");
            contratoFreteCliente.ValorContrato = Request.GetDecimalParam("ValorContrato");
            contratoFreteCliente.SaldoAtual = Request.GetDecimalParam("ValorContrato");
            contratoFreteCliente.TipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"), false);
            contratoFreteCliente.Cliente = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Cliente"));
            contratoFreteCliente.DataInicio = Request.GetDateTimeParam("DataInicioContrato");
            contratoFreteCliente.DataFim = Request.GetDateTimeParam("DataFimContrato");
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaContratoFreteCliente ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaContratoFreteCliente()
            {
                Cliente = Request.GetDoubleParam("Cliente"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                NumeroContrato = Request.GetStringParam("NumeroContrato"),
                Descricao = Request.GetStringParam("Descricao"),
                ContratoFechado = Request.GetNullableBoolParam("ContratoFechado")
            };
        }
        #endregion
    }
}
