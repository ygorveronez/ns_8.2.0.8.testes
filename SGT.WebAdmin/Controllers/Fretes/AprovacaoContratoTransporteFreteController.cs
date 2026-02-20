using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/AprovacaoContratoTransporteFrete")]
    public class AprovacaoContratoTransporteFreteController : BaseController
    {
		#region Construtores

		public AprovacaoContratoTransporteFreteController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.AprovacaoContratoTransporteFrete repositorio = new Repositorio.Embarcador.Frete.AprovacaoContratoTransporteFrete(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAprovacaoContratoTransporteFrete filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número do Contrato", "NumeroContrato", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nome do Contrato", "NomeContrato", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Início", "DataInicio", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação Aprovação", "SituacaoAprovacao", 10, Models.Grid.Align.center, true);

                if (filtrosPesquisa.Ativo.HasValue)
                    grid.AdicionarCabecalho("Situação", "Ativo", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> contratos = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();

                var listaContratosRetornar = (
                      from contrato in contratos
                      select new
                      {
                          contrato.Codigo,
                          contrato.NumeroContrato,
                          contrato.NomeContrato,
                          DataInicio = contrato.DataInicio.ToString("dd/MM/yyyy"),
                          DataFim = contrato.DataFim.ToString("dd/MM/yyyy"),
                          SituacaoAprovacao = contrato.StatusAprovacaoTransportador.ObterDescricao(),
                          Ativo = contrato.Ativo.ObterDescricaoAtivo()
                      }
                  ).ToList();

                grid.AdicionaRows(listaContratosRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os contratos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoTransporteFrete = repContratoTransporteFrete.BuscarPorCodigo(codigo);

                if (contratoTransporteFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                bool bloquearCampos = contratoTransporteFrete.StatusAprovacaoTransportador == StatusAprovacaoTransportador.AguardandoAprovacao ? false : true;

                dynamic dynContratoTransporteFrete = new
                {
                    contratoTransporteFrete.Codigo,
                    contratoTransporteFrete.NumeroContrato,
                    contratoTransporteFrete.ContratoExternoID,
                    NomeContrato = contratoTransporteFrete.NomeContrato,
                    AprovacaoAdicionalRequerida = contratoTransporteFrete.AprovacaoAdicionalRequerida ? "Sim" : "Não",
                    Cluster = contratoTransporteFrete.Cluster.ObterDescricao(),
                    Pais = contratoTransporteFrete.Pais?.Descricao ?? string.Empty,
                    Network = contratoTransporteFrete.Network.ObterDescricao(),
                    Equipe = contratoTransporteFrete.Equipe.ObterDescricao(),
                    Categoria = contratoTransporteFrete.Categoria.ObterDescricao(),
                    SubCategoria = contratoTransporteFrete.SubCategoria.ObterDescricao(),
                    ModoContrato = contratoTransporteFrete.ModoContrato.ObterDescricao(),
                    Empresa = contratoTransporteFrete.Transportador?.Descricao ?? string.Empty,
                    ConformidadeComRSP = contratoTransporteFrete.ConformidadeComRSP ? "Sim" : "Não",
                    PessoaJuridica = contratoTransporteFrete.PessoaJuridica.ObterDescricao(),
                    TipoContrato = contratoTransporteFrete.TipoContrato.ObterDescricao(),
                    HubNonHub = contratoTransporteFrete.HubNonHub.ObterDescricao(),
                    DominioOTM = contratoTransporteFrete.DominioOTM.ObterDescricao(),
                    DataInicio = contratoTransporteFrete.DataInicio.ToString("dd/MM/yyyy"),
                    DataFim = contratoTransporteFrete.DataFim.ToString("dd/MM/yyyy"),
                    Moeda = contratoTransporteFrete.Moeda.ObterDescricao(),
                    ValorPrevistoContrato = contratoTransporteFrete.ValorPrevistoContrato.ToString("n2"),
                    Padrao = contratoTransporteFrete.Padrao == Dominio.Enumeradores.OpcaoSimNao.Sim ? "Sim" : "Não",
                    TermosPagamento = contratoTransporteFrete.TermosPagamento?.Descricao ?? string.Empty,
                    ClausulaPenal = contratoTransporteFrete.ClausulaPenal ? "Sim" : "Não",
                    contratoTransporteFrete.Observacao,
                    Usuario = contratoTransporteFrete.UsuarioContrato?.Descricao ?? string.Empty,
                    StatusAprovacaoTransportador = contratoTransporteFrete.StatusAprovacaoTransportador.ObterDescricao(),
                    StatusAssinaturaContrato = contratoTransporteFrete.StatusAssinaturaContrato?.Descricao ?? string.Empty,
                    ProcessoAprovacao = contratoTransporteFrete.ProcessoAprovacao.ObterDescricao(),
                    Ativo = contratoTransporteFrete.Ativo ? "Ativo" : "Inativo",
                    BloquearCampos = bloquearCampos,
                    Anexos = string.Join(", ", contratoTransporteFrete.Anexos?.Select(obj => obj.Descricao).ToList()) ?? string.Empty
                };

                return new JsonpResult(dynContratoTransporteFrete);
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

        public async Task<IActionResult> AprovarContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoContrato = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContrato = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contrato = repContrato.BuscarPorCodigo(codigoContrato);

                if (contrato == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoClonado = repContrato.BuscarClonePorCodigo(codigoContrato);

                contrato.StatusAprovacaoTransportador = StatusAprovacaoTransportador.Aprovado;

                repContrato.Atualizar(contrato, Auditado);
                repContrato.Deletar(contratoClonado, Auditado);

                CriarRegistroIntegracaoContratoTransporteFrete(contrato, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoContrato = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.ContratoTransporteFrete repContrato = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contrato = repContrato.BuscarPorCodigo(codigoContrato);
                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contratoClonado = repContrato.BuscarClonePorCodigo(codigoContrato);

                if (contrato == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o registro.");

                contrato = RetornarContratoEstadoOriginal(contrato, contratoClonado);

                contrato.StatusAprovacaoTransportador = StatusAprovacaoTransportador.Rejeitado;

                repContrato.Atualizar(contrato, Auditado);
                repContrato.Deletar(contratoClonado, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void CriarRegistroIntegracaoContratoTransporteFrete(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao respositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao existeTipoIntegracao = respositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.LBC);

            if (existeTipoIntegracao == null)
                return;

            new Servicos.Embarcador.Frete.ContratoTransporteFrete(unitOfWork).GerarIntegracaoContrato(contrato, existeTipoIntegracao);
        }

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAprovacaoContratoTransporteFrete ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAprovacaoContratoTransporteFrete()
            {
                NumeroContrato = Request.GetIntParam("NumeroContrato"),
                ContratoExternoID = Request.GetIntParam("ContratoExternoID"),
                Categoria = Request.GetNullableEnumParam<CategoriaContratoTransporte>("Categoria"),
                SubCategoria = Request.GetNullableEnumParam<SubCategoriaContratoTransporte>("SubCategoria"),
                Transportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Empresa.Codigo : Request.GetIntParam("Transportador"),
                PessoaJuridica = Request.GetNullableEnumParam<PessoaJuridicaContratoTransporte>("PessoaJuridica"),
                DatatInicio = Request.GetNullableDateTimeParam("DatatInicio"),
                DataFim = Request.GetNullableDateTimeParam("DataFim"),
                StatusAprovacaoTransportador = Request.GetNullableEnumParam<StatusAprovacaoTransportador>("StatusAprovacaoTransportador"),
                StatusAssinaturaContrato = Request.GetIntParam("StatusAssinaturaContrato")
            };
        }

        private Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete RetornarContratoEstadoOriginal(Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete original, Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete clone)
        {
            original.NumeroContrato = clone.NumeroContrato;
            original.ContratoExternoID = clone.ContratoExternoID;
            original.NomeContrato = clone.NomeContrato;
            original.AprovacaoAdicionalRequerida = clone.AprovacaoAdicionalRequerida;
            original.Cluster = clone.Cluster;
            original.Pais = clone.Pais;
            original.Network = clone.Network;
            original.Equipe = clone.Equipe;
            original.Categoria = clone.Categoria;
            original.SubCategoria = clone.SubCategoria;
            original.ModoContrato = clone.ModoContrato;
            original.Transportador = clone.Transportador;
            original.ConformidadeComRSP = clone.ConformidadeComRSP;
            original.PessoaJuridica = clone.PessoaJuridica;
            original.TipoContrato = clone.TipoContrato;
            original.HubNonHub = clone.HubNonHub;
            original.DominioOTM = clone.DominioOTM;
            original.DataInicio = clone.DataInicio;
            original.DataFim = clone.DataFim;
            original.Moeda = clone.Moeda;
            original.ValorPrevistoContrato = clone.ValorPrevistoContrato;
            original.Padrao = clone.Padrao;
            original.TermosPagamento = clone.TermosPagamento;
            original.ClausulaPenal = clone.ClausulaPenal;
            original.Observacao = clone.Observacao;
            original.UsuarioContrato = clone.UsuarioContrato;
            original.StatusAssinaturaContrato = clone.StatusAssinaturaContrato;
            original.ProcessoAprovacao = clone.ProcessoAprovacao;
            original.Ativo = clone.Ativo;

            return original;
        }

        #endregion


    }
}
