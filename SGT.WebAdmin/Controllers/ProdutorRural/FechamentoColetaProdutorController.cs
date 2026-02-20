using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ProdutorRural
{
    [CustomAuthorize("ProdutorRural/FechamentoColetaProdutor")]
    public class FechamentoColetaProdutorController : BaseController
    {
		#region Construtores

		public FechamentoColetaProdutorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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

        public async Task<IActionResult> ObterDetalheFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);

                Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);


                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor pedidoColetaProdutor = repPedidoColetaProdutor.BuscarPorCodigo(codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes = repCargaComposicaoFrete.BuscarPorPreCarga(pedidoColetaProdutor.Pedido.PreCarga.Codigo);

                var composicaoFrete = (from obj in cargaComposicaoFretes
                                       select new
                                       {
                                           obj.Formula,
                                           obj.ValoresFormula,
                                           obj.Descricao,
                                           obj.TipoCampoValor,
                                           obj.TipoParametro,
                                           DescricaoTipoCampoValor = obj.TipoCampoValor.ObterDescricao(),
                                           Valor = obj.Valor.ToString("n2"),
                                           ValorCalculado = obj.ValorCalculado.ToString("n2")
                                       }).ToList();

                return new JsonpResult(composicaoFrete);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaPedidos(true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                //if (propOrdenar == "Carga") propOrdenar = "Carga.CodigoCargaEmbarcador";
                //else if (propOrdenar == "Ocorrencia") propOrdenar = "CargaOcorrencia.NumeroOcorrencia";
                //else if (propOrdenar == "Destinatario") propOrdenar = "Destinatario.Nome";
                //else if (propOrdenar == "Tomador") propOrdenar = "Tomador.Nome";

                // Busca Dados
                List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> listaGrid = new List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>();
                int totalRegistros = 0;
                var lista = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor repFechamentoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor fechamentoColetaProdutor = repFechamentoColetaProdutor.BuscarPorCodigo(codigo);

                // Valida
                if (fechamentoColetaProdutor == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    fechamentoColetaProdutor.Codigo,
                    fechamentoColetaProdutor.Situacao,
                    DataInicial = fechamentoColetaProdutor.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = fechamentoColetaProdutor.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    fechamentoColetaProdutor.Numero,
                    Carga = fechamentoColetaProdutor.Carga?.Codigo ?? 0,
                    fechamentoColetaProdutor.MotivoCancelamento,
                    fechamentoColetaProdutor.SituacaoNoCancelamento,
                    fechamentoColetaProdutor.TipoTomador,
                    DescricaoTipoTomador = fechamentoColetaProdutor.DescricaoTipoTomador(),
                    Filial = fechamentoColetaProdutor.Filial != null ? new { fechamentoColetaProdutor.Filial.Codigo, fechamentoColetaProdutor.Filial.Descricao } : new { Codigo = 0, Descricao = "" },
                    Transportador = new { Codigo = fechamentoColetaProdutor.Empresa?.Codigo ?? 0, Descricao = fechamentoColetaProdutor.Empresa?.Descricao ?? "" },
                    Tomador = new { Codigo = fechamentoColetaProdutor.Tomador?.Codigo ?? 0, Descricao = fechamentoColetaProdutor.Tomador?.Nome ?? "" },
                    DescricaoSituacao = fechamentoColetaProdutor.Situacao.ObterDescricao()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> SolicitarCalculoFretePreCargasPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);

                bool apenasPendentes = Request.GetBoolParam("ApenasPendentes");

                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = repPedidoColetaProdutor.BuscarPreCargaParaProcessamento(apenasPendentes);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga in preCargas)
                {
                    preCarga.CalculandoFrete = true;
                    preCarga.PendenciaCalculoFrete = false;
                    preCarga.MotivoPendencia = "";
                    repPreCarga.Atualizar(preCarga);
                }

                string msg = "Solicitou reprocessamento dos fretes pendentes.";
                if (!apenasPendentes)
                    msg = "Solicitou reprocessamento de todos fretes.";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, new Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor(), msg, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o calculo das pre cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao

                Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor repFechamentoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor(unitOfWork);
                Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);


                int.TryParse(Request.Params("Codigo"), out int codigo);

                string motivoCancelamento = Request.Params("MotivoCancelamento");

                Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor fechamentoColetaProdutor = repFechamentoColetaProdutor.BuscarPorCodigo(codigo);

                if (fechamentoColetaProdutor.Carga != null && (fechamentoColetaProdutor.Carga.SituacaoCarga != SituacaoCarga.Cancelada && fechamentoColetaProdutor.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                    return new JsonpResult(false, true, "Não é permitido cancelar o fechamento enquanto a carga não estiver cancelada.");

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = (from obj in fechamentoColetaProdutor.PedidosFechamento select obj.PedidoColetaProdutor.Pedido.PreCarga).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos fechamentoColetaProdutorPedido in fechamentoColetaProdutor.PedidosFechamento.ToList())
                {
                    fechamentoColetaProdutorPedido.PedidoColetaProdutor.Situacao = SituacaoPedidoColetaProdutor.AgFechamento;
                    repPedidoColetaProdutor.Atualizar(fechamentoColetaProdutorPedido.PedidoColetaProdutor);
                }

                foreach (Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga in preCargas)
                {
                    preCarga.Carga = null;
                    preCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoGeracaoCarga;
                    repPreCarga.Atualizar(preCarga);
                }

                fechamentoColetaProdutor.MotivoCancelamento = motivoCancelamento;
                fechamentoColetaProdutor.SituacaoNoCancelamento = fechamentoColetaProdutor.Situacao;
                fechamentoColetaProdutor.Situacao = SituacaoFechamentoColetaProdutor.Cancelado;
                repFechamentoColetaProdutor.Atualizar(fechamentoColetaProdutor);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoColetaProdutor, null, "Solicitou o Cancelamento do Fechamento", unitOfWork);

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

        public async Task<IActionResult> CriarCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor repFechamentoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor(unitOfWork);
                Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);
                Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos repFechamentoColetaProdutorPedidos = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor fechamentoColetaProdutor = repFechamentoColetaProdutor.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from obj in fechamentoColetaProdutor.PedidosFechamento select obj.PedidoColetaProdutor.Pedido).ToList();

                if (pedidos.Any(obj => obj.PreCarga.PendenciaCalculoFrete || obj.PreCarga.CalculandoFrete))
                    return new JsonpResult(false, true, "Existem Pedidos que não estão com o frete calculado, antes de gerar a carga é necessário que todos os pedidos tenham valor de frete.");

                unitOfWork.Start();

                List<Dominio.Enumeradores.TipoTomador> tipoTomadors = (from obj in pedidos select obj.TipoTomador).Distinct().ToList();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = GerarCarga(pedidos.FirstOrDefault().PreCarga, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosGerados = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                foreach (Dominio.Enumeradores.TipoTomador tipoTomador in tipoTomadors)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTomador = (from obj in pedidos where obj.TipoTomador == tipoTomador select obj).ToList();

                    if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                    {
                        List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> outroEnderecosOrigem = (from obj in pedidosTomador select obj.EnderecoOrigem.ClienteOutroEndereco).Distinct().ToList();
                        foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco outroEndereco in outroEnderecosOrigem)
                        {
                            if (outroEndereco != null)
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosEnderecos = (from obj in pedidosTomador where obj.EnderecoOrigem.ClienteOutroEndereco.Codigo == outroEndereco.Codigo select obj).ToList();
                                pedidosGerados.Add(GerarPedidoCarga(carga, pedidosEnderecos, TipoServicoMultisoftware, unitOfWork));
                            }
                            else
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosEnderecos = (from obj in pedidosTomador where obj.EnderecoOrigem.ClienteOutroEndereco == null select obj).ToList();
                                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosEnderecos)
                                {
                                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidoEndereco = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                                    pedidoEndereco.Add(pedido);
                                    pedidosGerados.Add(GerarPedidoCarga(carga, pedidoEndereco, TipoServicoMultisoftware, unitOfWork));
                                }
                            }
                        }
                    }
                    else
                    {
                        pedidosGerados.Add(GerarPedidoCarga(carga, pedidosTomador, TipoServicoMultisoftware, unitOfWork));
                    }
                }
                GerarCargaPedidos(carga, pedidosGerados, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador);

                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = (from obj in fechamentoColetaProdutor.PedidosFechamento select obj.PedidoColetaProdutor.Pedido.PreCarga).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga in preCargas)
                {
                    preCarga.SituacaoPreCarga = SituacaoPreCarga.CargaGerada;
                    repPreCarga.Atualizar(preCarga);
                }

                foreach (Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos fechamentoColetaProdutorPedidos in fechamentoColetaProdutor.PedidosFechamento)
                {
                    fechamentoColetaProdutorPedidos.PedidoColetaProdutor.Situacao = SituacaoPedidoColetaProdutor.Fechada;
                    repPedidoColetaProdutor.Atualizar(fechamentoColetaProdutorPedidos.PedidoColetaProdutor);
                }

                fechamentoColetaProdutor.Situacao = SituacaoFechamentoColetaProdutor.AgEmissaoCarga;
                fechamentoColetaProdutor.Carga = carga;
                repFechamentoColetaProdutor.Atualizar(fechamentoColetaProdutor);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoColetaProdutor, null, "Solicitou a Criação da Carga", unitOfWork);
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    Codigo = fechamentoColetaProdutor.Codigo
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor repFechamentoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor(unitOfWork);
                Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);
                Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos repFechamentoColetaProdutorPedidos = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos(unitOfWork);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);


                DateTime dataInicio;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

                DateTime dataFim;
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                double tomador;
                double.TryParse(Request.Params("Tomador"), out tomador);

                int filial;
                int.TryParse(Request.Params("Filial"), out filial);

                int empresa;
                int.TryParse(Request.Params("Transportador"), out empresa);

                Enum.TryParse(Request.Params("TipoTomador"), out Dominio.Enumeradores.TipoTomador tipoTomador);

                // Busca os documentos selecionados
                List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> pedidos = BuscarDocumentosSelecionados(unitOfWork, out string erro);

                //if (pedidos.Any(obj => obj.Pedido != null && (obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)))
                //    erro += "Enquanto as notas estavam sendo selecionadas uma carga das notas selecionadas foi cancelada, por favor, refaça o processo.";

                if (pedidos.Any(obj => obj.Situacao != SituacaoPedidoColetaProdutor.AgFechamento))
                    erro += "Não é possível iniciar o fechamento.";


                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);
                if (pedidos.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum documento selecionado.");

                Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor fechamentoColetaProdutor = new Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor();

                if (dataInicio != DateTime.MinValue)
                    fechamentoColetaProdutor.DataInicial = dataInicio;

                if (dataFim != DateTime.MinValue)
                    fechamentoColetaProdutor.DataFinal = dataFim;

                fechamentoColetaProdutor.Numero = repFechamentoColetaProdutor.ObterProximoFechamento();
                fechamentoColetaProdutor.TipoTomador = tipoTomador;
                fechamentoColetaProdutor.Situacao = SituacaoFechamentoColetaProdutor.EmCriacao;

                if (tomador > 0)
                    fechamentoColetaProdutor.Tomador = repCliente.BuscarPorCPFCNPJ(tomador);

                if (filial > 0)
                    fechamentoColetaProdutor.Filial = repFilial.BuscarPorCodigo(filial);

                if (empresa > 0)
                    fechamentoColetaProdutor.Empresa = repEmpresa.BuscarPorCodigo(empresa);

                // Persiste dados
                repFechamentoColetaProdutor.Inserir(fechamentoColetaProdutor, Auditado);

                foreach (Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor documento in pedidos)
                {
                    Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos fechamentoColetaProdutorPedidos = new Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos();
                    fechamentoColetaProdutorPedidos.FechamentoColetaProdutor = fechamentoColetaProdutor;
                    fechamentoColetaProdutorPedidos.PedidoColetaProdutor = documento;
                    repFechamentoColetaProdutorPedidos.Inserir(fechamentoColetaProdutorPedidos);

                    documento.Situacao = SituacaoPedidoColetaProdutor.EmFechamento;
                    repPedidoColetaProdutor.Atualizar(documento);
                }

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    Codigo = fechamentoColetaProdutor.Codigo
                });
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaPedidos(false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                //if (propOrdenar == "Carga") propOrdenar = "Carga.CodigoCargaEmbarcador";
                //else if (propOrdenar == "Ocorrencia") propOrdenar = "CargaOcorrencia.NumeroOcorrencia";
                //else if (propOrdenar == "Destinatario") propOrdenar = "Destinatario.Nome";
                //else if (propOrdenar == "Tomador") propOrdenar = "Tomador.Nome";

                // Busca Dados
                List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> listaGrid = new List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>();
                int totalRegistros = 0;
                var lista = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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

        #endregion

        #region Métodos Privados

        private void GerarCargaPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            string retorno = serCarga.CriarCargaPedidosPorPedidos(ref carga, pedidos, tipoServicoMultisoftware, null, unitOfWork, configuracao, Auditado);

            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            carga.CargaFechada = true;
            Servicos.Log.TratarErro("19 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

            repCarga.Atualizar(carga);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            svcCarga.FecharCarga(carga, unitOfWork, tipoServicoMultisoftware, this.Cliente, true);
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido GerarPedidoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = pedidos.OrderByDescending(obj => obj.PesoTotal).FirstOrDefault();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidoBase.Clonar();
            pedidoBase.ControleNumeracao = pedidoBase.Codigo;
            repPedido.Atualizar(pedidoBase);

            pedido.ValorFreteNegociado = pedidos.Sum(obj => obj.ValorFreteNegociado);

            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = pedidoBase.EnderecoOrigem.Clonar();
            Utilidades.Object.DefinirListasGenericasComoNulas(enderecoOrigem);
            repPedidoEndereco.Inserir(enderecoOrigem);

            pedido.EnderecoOrigem = enderecoOrigem;
            pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
            
            //pedido.NotasFiscais.Clear();
            //pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscals = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscalBase in pedidoBase.NotasFiscais.ToList())
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota = xMLNotaFiscalBase.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(xmlNota);

                xmlNota.Valor = (from obj in pedidos select obj.NotasFiscais.Sum(o => o.Valor)).Sum();
                xmlNota.Peso = (from obj in pedidos select obj.NotasFiscais.Sum(o => o.Peso)).Sum();
                xmlNota.PesoLiquido = (from obj in pedidos select obj.NotasFiscais.Sum(o => o.PesoLiquido)).Sum();
                xmlNota.DataRecebimento = DateTime.Now;

                repXMLNotaFiscal.Inserir(xmlNota);
                notasFiscals.Add(xmlNota);
            }

            repPedido.Inserir(pedido);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedido.Produtos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProdutoTransbordo = pedidoProduto.Clonar();
                pedidoProdutoTransbordo.Pedido = pedido;
                repPedidoProduto.Inserir(pedidoProduto);
            }

            Utilidades.Object.DefinirListasGenericasComoNulas(pedido);
            pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota in notasFiscals)
                pedido.NotasFiscais.Add(xmlNota);

            repPedido.Atualizar(pedido);

            if (pedido.NotasFiscais.Count > 0)
                servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoFaturado, pedido, configuracao, Cliente);

            return pedido;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = preCarga.Pedidos.FirstOrDefault().TipoOperacao;

            int sequencialNumeroCarga = 0; ;
            if (ConfiguracaoEmbarcador.NumeroCargaSequencialUnico)
                sequencialNumeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            else
                sequencialNumeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, preCarga.Filial?.Codigo ?? 0);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga()
            {
                CodigoCargaEmbarcador = sequencialNumeroCarga.ToString(),
                NumeroSequenciaCarga = sequencialNumeroCarga,
                Empresa = preCarga.Empresa,
                ExigeNotaFiscalParaCalcularFrete = tipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? configuracaoEmbarcador.ExigirNotaFiscalParaCalcularFreteCarga,
                Filial = preCarga.Filial,
                ModeloVeicularCarga = preCarga.ModeloVeicularCarga,
                MotivoPendencia = "",
                MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia,
                NaoExigeVeiculoParaEmissao = tipoOperacao?.NaoExigeVeiculoParaEmissao ?? false,
                Rota = preCarga.Rota,
                SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                TipoDeCarga = preCarga.TipoDeCarga,
                TipoFreteEscolhido = TipoFreteEscolhido.Embarcador,
                TipoOperacao = tipoOperacao,
                Veiculo = null,
                LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = false,
                EmpresaFilialEmissora = null
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                carga.ExigeNotaFiscalParaCalcularFrete = true;

            repCarga.Inserir(carga);

            carga.Protocolo = carga.Codigo;

            new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(preCarga.Rota);

            repCarga.Atualizar(carga);

            return carga;

        }

        private Models.Grid.Grid GridPesquisaPedidos(bool exportar)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("CodigoTomador", false);
            grid.AdicionarCabecalho("CodigoTransportador", false);
            grid.AdicionarCabecalho("PendenciaCalculoFrete", false);
            grid.AdicionarCabecalho("MotivoPendencia", false);
            grid.AdicionarCabecalho("CalculandoFrete", false);
            grid.AdicionarCabecalho("Pré Carga", "Carga", 8, Models.Grid.Align.center, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Pedido", "NumeroPedido", 8, Models.Grid.Align.center, true);
            else
                grid.AdicionarCabecalho("Pedido", "Pedido", 8, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Empresa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data da Coleta", "DataColeta", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor ICMS", "ICMS", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 15, Models.Grid.Align.right, true);

            if (exportar)
            {
                grid.AdicionarCabecalho("Placa", "Placa", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Distância", "Distancia", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Endereço Coleta", "EnderecoColeta", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 15, Models.Grid.Align.right, true);
            }


            return grid;

        }

        private List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> BuscarDocumentosSelecionados(Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);
            List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> listaBusca = new List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>();

            // Valida filtros
            int transportador = 1;
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                int.TryParse(Request.Params("Transportador"), out transportador);

            int.TryParse(Request.Params("Filial"), out int filial);
            double.TryParse(Request.Params("Tomador"), out double tomador);

            //if (filial == 0 || tomador == 0)
            //{
            //    erro = "Filial e Tomador são obrigatórios.";
            //    return null;
            //}

            if (transportador == 0)
            {
                erro = "Transportador é obrigatório.";
                return null;
            }


            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);
            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisaDocumento(ref listaBusca, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PedidosNaoSelecionadas"));
                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    listaBusca.Remove(new Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor() { Codigo = (int)dynNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PedidosSelecionadas"));
                foreach (var dynSelecionada in listaSelecionadas)
                    listaBusca.Add(repPedidoColetaProdutor.BuscarPorCodigo((int)dynSelecionada.Codigo, false));
            }

            // Retorna lista
            return listaBusca;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Inicio", "DataInicio", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Fim", "DataFim", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, bool somenteAtivo, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor repFechamentoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor(unitOfWork);

            // Dados do filtro
            int transportador = 0;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                transportador = this.Empresa.Codigo;
            else
                int.TryParse(Request.Params("Transportador"), out transportador);


            DateTime dataInicio;
            DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);

            DateTime dataFim;
            DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

            string preCarga = Request.Params("PreCarga");

            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("Pedido"), out int pedido);
            int.TryParse(Request.Params("Origem"), out int origem);
            int.TryParse(Request.Params("Rementente"), out int rementente);
            int.TryParse(Request.Params("Situacao"), out int situacao);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor situacaoFechamentoColetaProdutor = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor)situacao;

            // Consulta
            List<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor> listaGrid = repFechamentoColetaProdutor.Consultar(numero, dataInicio, dataFim, transportador, filial, preCarga, pedido, origem, rementente, situacaoFechamentoColetaProdutor, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repFechamentoColetaProdutor.ContarConsulta(numero, dataInicio, dataFim, transportador, filial, preCarga, pedido, origem, rementente, situacaoFechamentoColetaProdutor);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            DataInicio = obj.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                            DataFim = obj.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                            Tomador = obj.Tomador?.Descricao ?? "",
                            Transportador = obj.Empresa?.Descricao ?? "",
                            Filial = obj.Filial?.Descricao ?? "",
                            DescricaoSituacao = obj.Situacao.ObterDescricao()
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Tomador") propOrdenar = "Tomador.Nome";
            else if (propOrdenar == "Transportador") propOrdenar = "Empresa.RazaoSocial";
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> listaGrid, ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {

            // Instancia repositorios
            Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);
            Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos repFechamentoColetaProdutorPedidos = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutorPedidos(unitOfWork);

            // Dados do filtro
            // Quando há um codigo, tras apenas os documentos daquela nfsmanual
            int.TryParse(Request.Params("Codigo"), out int fechamentoColetaProdutor);

            int transportador = 0;

            int.TryParse(Request.Params("Transportador"), out transportador);

            int.TryParse(Request.Params("Filial"), out int filial);
            double.TryParse(Request.Params("Tomador"), out double tomador);


            DateTime.TryParse(Request.Params("DataInicio"), out DateTime dataInicio);
            DateTime.TryParse(Request.Params("DataFim"), out DateTime dataFim);

            Enum.TryParse(Request.Params("TipoTomador"), out Dominio.Enumeradores.TipoTomador tipoTomador);

            bool.TryParse(Request.Params("SomenteFretePendente"), out bool somenteFretePendente);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor situacaoPedidoColetaProdutor = SituacaoPedidoColetaProdutor.AgFechamento;
            if (fechamentoColetaProdutor > 0)
            {
                listaGrid = repFechamentoColetaProdutorPedidos.Consultar(fechamentoColetaProdutor, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = repFechamentoColetaProdutorPedidos.ContarConsulta(fechamentoColetaProdutor);
            }
            else
            {
                listaGrid = repPedidoColetaProdutor.Consultar(dataInicio, dataFim, transportador, filial, tomador, situacaoPedidoColetaProdutor, tipoTomador, somenteFretePendente, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = repPedidoColetaProdutor.ContarConsulta(dataInicio, dataFim, transportador, filial, tomador, situacaoPedidoColetaProdutor, tipoTomador, somenteFretePendente);
            }

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            CodigoFilial = obj.Pedido.Filial?.Codigo ?? 0,
                            CodigoTomador = obj.Pedido.ObterTomador().Codigo,
                            CodigoTransportador = obj.Pedido.Empresa?.Codigo ?? 0,
                            Empresa = obj.Pedido.Empresa?.Descricao ?? "",
                            Placa = obj.Pedido.PreCarga?.Veiculo?.Placa ?? "",
                            Distancia = obj.Pedido.PreCarga?.Distancia.ToString("n2") ?? "",
                            EnderecoColeta = obj.Pedido.EnderecoOrigem?.Endereco ?? (obj.Pedido.Remetente?.Endereco ?? ""),
                            ModeloVeicular = obj.Pedido.ModeloVeicularCarga?.Descricao ?? "",
                            PendenciaCalculoFrete = obj.Pedido.PreCarga?.PendenciaCalculoFrete ?? false,
                            MotivoPendencia = obj.Pedido.PreCarga?.MotivoPendencia ?? "",
                            CalculandoFrete = obj.Pedido.PreCarga?.CalculandoFrete ?? false,
                            Carga = obj.Pedido.PreCarga?.NumeroPreCarga ?? "",
                            Pedido = obj.Pedido.NumeroPedidoEmbarcador,
                            NumeroPedido = obj.Pedido.Numero,
                            Remetente = obj.Pedido.Remetente.Descricao,
                            Origem = obj.Pedido.Origem.DescricaoCidadeEstado,
                            Destino = obj.Pedido.Destino.DescricaoCidadeEstado,
                            Tomador = obj.Pedido.DescricaoTipoTomador(),
                            DataColeta = obj.Pedido.DataInicialColeta.HasValue ? obj.Pedido.DataInicialColeta.Value.ToString("dd/MM/yyyy") : "",
                            ValorFrete = obj.Pedido.ValorFreteAReceber.ToString("n2"),
                            ICMS = obj.Pedido.ValorICMS.ToString("n2"),
                            DT_RowColor = (obj.Pedido.PreCarga?.PendenciaCalculoFrete ?? false) || (obj.Pedido.PreCarga?.CalculandoFrete ?? false) ? CorGrid.Danger : "",
                            //DT_FontColor = (obj.Pedido.PreCarga?.PendenciaCalculoFrete ?? false) || (obj.Pedido.PreCarga?.CalculandoFrete ?? false) ? "#FFFFFF" : ""
                        };

            return lista.ToList();
        }

        #endregion
    }
}
