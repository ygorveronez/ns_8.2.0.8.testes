using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SGT.WebAdmin.Controllers.Cargas.ConfirmarDocumentoEmLote
{
    [CustomAuthorize("Cargas/ConfirmarDocumentoEmLote")]
    public class ConfirmarDocumentoEmLoteController : BaseController
    {
		#region Construtores

		public ConfirmarDocumentoEmLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> ConfirmarEnvioDosDocumentosFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                object resultado = null;

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<int> codigosCarga = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos") ?? "[]");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                if (codigosCarga.Count > 0)
                    cargas = repositorioCarga.BuscarPorCodigos(codigosCarga);

                if (cargas.Count == 0)
                    return new JsonpResult(false, true, "Carga(s) não encontrada(s).");

                List<(string Erro, string Carga)> errosCarga = new List<(string Erro, string Carga)>();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    try
                    {
                        unitOfWork.Start();

                        carga.MensagemRetornoEtapaDocumento = string.Empty;
                        carga.UsuarioAvancoEtapaDocumentoLote = Usuario;
                        carga.AvancouCargaEtapaDocumentoLote = true;

                        resultado = servicoCarga.ConfirmarEnvioDosDocumentos(carga, false, false, TipoServicoMultisoftware, permissoesPersonalizadas, Auditado, WebServiceConsultaCTe, Usuario, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Confirmou o envio dos documentos pela tela de Emissão de CT-e Porto em Lote.", unitOfWork);

                        repositorioCarga.Atualizar(carga);

                        unitOfWork.CommitChanges();
                    }
                    catch (ServicoException ex)
                    {
                        unitOfWork.Rollback();

                        Dominio.Entidades.Embarcador.Cargas.Carga cargaCarregada = repositorioCarga.BuscarPorCodigo(carga.Codigo);

                        cargaCarregada.MensagemRetornoEtapaDocumento = ex.Message;

                        repositorioCarga.Atualizar(cargaCarregada);

                        unitOfWork.CommitChanges();

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"{ex.Message}", unitOfWork);

                        errosCarga.Add(ValueTuple.Create(ex.Message, carga.CodigoCargaEmbarcador));
                        continue;
                    }
                }

                if (errosCarga.Count > 0)
                {
                    StringBuilder mensagem = new StringBuilder();
                    for (int i = 0; i < errosCarga.Count; i++)
                    {
                        mensagem.AppendFormat("A carga {0} apresentou o erro: {1}", errosCarga[i].Carga, errosCarga[i].Erro);
                        if (i < errosCarga.Count - 1)
                        {
                            mensagem.Append(" ");
                        }
                    }

                    return new JsonpResult(false, true, mensagem.ToString());
                }

                return new JsonpResult(resultado);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o envio dos documentos fiscais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metódos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número da Carga", "Carga", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Número do Booking", "NumeroBooking", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("VVD", "VVD", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Porto de Origem", "PortoOrigem", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Porto de Destino", "PortoDestino", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação da Carga", "SituacaoCarga", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Mensagem Retorno", "MensagemRetorno", 25, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaEmissaoCTePortoLote filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int totalRegistros = repositorioCargaPedido.ContarConsultaCargasPedidos(filtrosPesquisa, parametrosConsulta);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.EmissaoCTePortoLoteCargaPedido> cargasPedidos = repositorioCargaPedido.BuscarCargasPedidosPagamentoProvedor(filtrosPesquisa, parametrosConsulta);

                var listaPagamentoProvedor = (
                                    from obj in cargasPedidos
                                    select new
                                    {
                                        Codigo = obj.Codigo,
                                        Carga = obj.Carga,
                                        NumeroBooking = obj.NumeroBooking,
                                        VVD = obj.VVD,
                                        PortoOrigem = obj.PortoOrigem,
                                        PortoDestino = obj.PortoDestino,
                                        Remetente = obj.Remetente,
                                        Destinatario = obj.Destinatario,
                                        MensagemRetorno = obj.MensagemRetorno,
                                        SituacaoCarga = obj.SituacaoCarga.ObterDescricao(),
                                    }
                                ).ToList();

                grid.AdicionaRows(listaPagamentoProvedor);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaEmissaoCTePortoLote ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaEmissaoCTePortoLote()
            {
                CodigoNavioViagemDirecao = Request.GetIntParam("NavioViagemDirecao"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "NumeroBooking")
                return "NumeroBooking";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
