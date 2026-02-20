using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;



namespace SGT.WebAdmin.Controllers.Cargas.Carga.Documentos
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoDespesaController : BaseController
    {
		#region Construtores

		public CargaIntegracaoDespesaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaDespesa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repositorioPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                // Converte parametros
                int carga = 0;
                int.TryParse(Request.Params("Carga"), out carga);

                // Grid
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Motivo Atendimento", "MotivoAtendimento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo Pagamento", "TipoPagamento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Pagamento", "DataPagamento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Integradora", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação Despesa", "DescricaoSituacao", 10, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                // Busca os valores
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> pagamentos = repositorioPagamentoMotoristaTMS.Consultar(carga, parametrosConsulta);
                int totalRegistros = repositorioPagamentoMotoristaTMS.ContarConsulta(carga);
                
                // Formata retorno
                var lista = (from obj in pagamentos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Valor,
                                 MotivoAtendimento = obj.Chamado?.MotivoChamado.Descricao,
                                 TipoPagamento = obj.PagamentoMotoristaTipo.Descricao,
                                 DataPagamento = obj.DataPagamento.ToString("dd/MM/yyyy"),
                                 TipoIntegracao = obj.PagamentoMotoristaTipo.ObterTipoIntegracao,
                                 obj.DescricaoSituacao,
                                 DT_RowColor = CorFundoIntegracaoDespesa(obj),
                                 DT_FontColor = CorFonteIntegracaoDespesa(obj)
                             }).ToList();

                // Vincula à grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> LiberarComProblemaPagamentoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);


            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_LiberarPagamentoMotoristaRejeitado))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                // Valida informações
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                if (carga.PossuiPendencia)
                {
                    carga.LiberadoComProblemaPagamentoMotorista = true;
                    carga.PossuiPendencia = false;
                    carga.MotivoPendencia = "";
                    carga.AgIntegracaoPagamentoMotorista = false;
                    repCarga.Atualizar(carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Avançou Etapa com Pagamento Motorista Rejeitado.", unitOfWork);
                }
                unitOfWork.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDetalhesDespesa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PagamentoMotorista.DespesaViagem repositorioDespesaViagem = new Repositorio.Embarcador.PagamentoMotorista.DespesaViagem(unitOfWork);

                int codigoPagamento = Request.GetIntParam("Pagamento");

                List<(string Descricao, int Quantidade, decimal ValorUnitario, decimal ValorTotal)> despesas = repositorioDespesaViagem.BuscarPorPagamentoMotorista(codigoPagamento);
                
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Quantidade", "Quantidade", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Unitário", "ValorUnitario", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 15, Models.Grid.Align.center, true);

                var lista = (from obj in despesas
                             select new
                             {
                                obj.Descricao,
                                obj.Quantidade,
                                obj.ValorTotal,
                                obj.ValorUnitario
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(lista.Count());

                return new JsonpResult(grid);
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

        #region Métodos Privados

        private string CorFonteIntegracaoDespesa(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            if (pagamento.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao || pagamento.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Cancelada)
                return "#FFF";

            return "";
        }

        private string CorFundoIntegracaoDespesa(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            if (pagamento.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Cancelada)
                return "#777";

            if (pagamento.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Finalizada || pagamento.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (pagamento.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
            if (pagamento.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgInformacoes)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo;

            return "";
        }

        #endregion
    }
}
