using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize(new string[] { "Pedidos/Pedido", "Pedidos/DetalheEntrega" })]
    public class PedidoClienteController : BaseController
    {
        #region Construtores

        public PedidoClienteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoStatus", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.NotasFiscais, "Nota", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Canhotos, "DescricaoStatusCanhoto", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("StatusCanhoto", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int codigoPedido = Request.GetIntParam("Codigo");

                int totalRegistros = repositorioPedido.ContarConsultaPedidoNotasCliente(codigoPedido);
                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotasPortalCliente> listaPedido = totalRegistros > 0 ? repositorioPedido.ConsultarPedidoNotasCliente(codigoPedido, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotasPortalCliente>();

                grid.AdicionaRows((from o in listaPedido
                                   select new
                                   {
                                       o.Codigo,
                                       DescricaoStatus = o.Status?.ObterDescricao() ?? string.Empty,
                                       o.Status,
                                       o.Nota,
                                       DescricaoStatusCanhoto = o.StatusCanhoto?.ObterDescricao() ?? string.Empty,
                                       o.StatusCanhoto
                                   }).ToList());
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoStatus", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("CTes", "CTes", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.NumeroTransporte, "NumeroPedido", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("QuantidadeNotas", false);
                grid.AdicionarCabecalho("QuantidadeNotasEntregues", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Cliente, "Cliente", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Peso, "Peso", 8, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Emissao, "Emissao", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Entrega, "Entrega", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Canhotos, "Notas", 8, Models.Grid.Align.left);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.NotasFiscais, "NotasFiscais", 8, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int totalRegistros = repositorioPedido.ContarConsultaPedidosCliente(filtrosPesquisa);
                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPortalCliente> listaPedido = totalRegistros > 0 ? repositorioPedido.ConsultarPedidosCliente(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPortalCliente>();

                grid.AdicionaRows((
                    from o in listaPedido
                    select new
                    {
                        o.Codigo,
                        DescricaoStatus = o.Status?.ObterDescricaoPortalCliente() ?? "Não Realizado",
                        o.Status,
                        o.CTes,
                        NumeroPedido = o.NumeroPedido.Contains("_") ? o.NumeroPedido.Split('_')[1] : o.NumeroPedido,
                        Notas = $"{o.QuantidadeNotasEntregues} de {o.QuantidadeNotas}",
                        o.QuantidadeNotas,
                        o.QuantidadeNotasEntregues,
                        o.Cliente,
                        o.Destino,
                        Peso = o.Peso.ToString("n2") + " KG",
                        Emissao = o.Emissao?.ToDateString() ?? string.Empty,
                        Entrega = configuracaoTabelaFrete.PermitirInformarLeadTimeTabelaFreteCliente ? (o.Entrega ?? o.PrevisaoEntrega)?.ToDateString() : (o.Entrega?.ToDateString() ?? ""),
                        o.NotasFiscais
                    }).ToList()
                );

                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarDadosFiltroPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                List<SituacaoAcompanhamentoPedido> situacaoFiltro = new List<SituacaoAcompanhamentoPedido>() {
                    SituacaoAcompanhamentoPedido.AgColeta,
                    SituacaoAcompanhamentoPedido.EmTransporte,
                    SituacaoAcompanhamentoPedido.SaiuParaEntrega,
                    SituacaoAcompanhamentoPedido.EntregaParcial,
                    SituacaoAcompanhamentoPedido.Entregue,
                    SituacaoAcompanhamentoPedido.EntregaRejeitada,
                    SituacaoAcompanhamentoPedido.ProblemaNoTransporte };

                List<(SituacaoAcompanhamentoPedido Situacao, int Quantidade)> preenchimentoFiltros = new List<(SituacaoAcompanhamentoPedido Situacao, int Quantidade)>();

                foreach (SituacaoAcompanhamentoPedido situacao in situacaoFiltro)
                {
                    filtrosPesquisa.SituacaoAcompanhamentoPedido = situacao;

                    int quantidade = repositorioPedido.ContarConsultaPedidosCliente(filtrosPesquisa);

                    preenchimentoFiltros.Add(ValueTuple.Create(situacao, quantidade));
                }

                return new JsonpResult(new
                {
                    Filtros = preenchimentoFiltros.Select(o => new
                    {
                        o.Situacao,
                        o.Quantidade
                    })
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoAtualizarDadosFiltro);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosFiltroPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<SituacaoAcompanhamentoPedido> situacaoFiltro = Request.GetListEnumParam<SituacaoAcompanhamentoPedido>("Filtros");
                List<(SituacaoAcompanhamentoPedido Situacao, int Quantidade)> preenchimentoFiltros = new List<(SituacaoAcompanhamentoPedido Situacao, int Quantidade)>();

                foreach (SituacaoAcompanhamentoPedido situacao in situacaoFiltro)
                {
                    filtrosPesquisa.SituacaoAcompanhamentoPedido = situacao;

                    int quantidade = repositorioPedido.ContarConsultaPedidosCliente(filtrosPesquisa);

                    preenchimentoFiltros.Add(ValueTuple.Create(situacao, quantidade));
                }

                return new JsonpResult(new
                {
                    Filtros = preenchimentoFiltros.Select(o => new
                    {
                        o.Situacao,
                        o.Quantidade
                    })
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarDadosFiltro);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCanhotosNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoNota = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarCanhotosPorNF(codigoNota);

                return new JsonpResult((
                        from o in canhotos
                        select new
                        {
                            o.Codigo,
                            Miniatura = Base64ImagemAnexo(o, unitOfWork)
                        }
                    ).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarFeedback()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                if (cargaEntrega.DataAvaliacao.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.AvaliacaoDessePedidoJaFoiFeita);

                cargaEntrega.DataAvaliacao = DateTime.Now;
                cargaEntrega.ObservacaoAvaliacao = Request.GetStringParam("Observacao");
                cargaEntrega.AvaliacaoGeral = Request.GetNullableIntParam("Avaliacao");

                unitOfWork.Start();
                repCargaEntrega.Atualizar(cargaEntrega, Auditado);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoAtualizarDados);
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
                int codigoPedido = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                if (pedido == null)
                    return new JsonpResult(true, false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repPedido.BuscarCTesPorPedido(pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaBase = repCargaEntrega.BuscarPorPedidoRastreio(pedido.Codigo, pedido.Destinatario.CPF_CNPJ);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregasTransferencia = ObterControleEntregasTransferencia(pedido, unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregasRota = ObterControleEntregasRota(pedido, unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ocorrenciaColetaEntregasTransferencia = ObterOcorrenciasControleEntrega(cargaEntregasTransferencia, unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ocorrenciaColetaEntregasRota = ObterOcorrenciasControleEntrega(cargaEntregasRota, unitOfWork);

                Dominio.Entidades.Cliente destinatarioPedido = pedido.Destinatario;
                Dominio.Entidades.Cliente remetentePedido = pedido.Remetente;
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteTomador = remetentePedido != null ? ctes.Where(o => o.Tomador?.CPF_CNPJ == remetentePedido.CPF_CNPJ_SemFormato).FirstOrDefault() : null;

                return new JsonpResult(new
                {
                    pedido.Codigo,
                    Situacao = pedido.SituacaoAcompanhamentoPedido,

                    Detalhes = new
                    {
                        pedido.Codigo,
                        CTe = string.Join(", ", ctes.Select(o => o.Numero).Distinct()),
                        Cliente = destinatarioPedido.Nome,
                        Contato = destinatarioPedido.Email,
                        DocumentoCliente = destinatarioPedido.CPF_CNPJ_Formatado,
                        Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                        DataEntrega = cargaEntregaBase?.DataFim?.ToDateString() ?? string.Empty,
                        DataEmissao = cteTomador?.DataEmissao?.ToDateString() ?? string.Empty,
                        InformacoesLocalizacaoCliente = ObterInformacoesLocalizacaoCliente(pedido.Remetente)
                    },
                    Transferencia = new
                    {
                        CodigoPedido = pedido.Codigo,
                        PossuiEtapaTransferencia = cargaEntregasTransferencia.Count() > 0,
                        Ocorrencias = (
                           from o in ocorrenciaColetaEntregasTransferencia
                           select new
                           {
                               o.Codigo,
                               CodigoOcorrencia = o.TipoDeOcorrencia?.Codigo ?? 0,
                               Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(o.TipoDeOcorrencia, o.CargaEntrega.Carga, pedido.Destinatario, pedido.Remetente),
                               DataOcorrencia = o.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                           }
                        ).ToList(),
                        Polilinhas = ObterPolilinhasControleEntrega(cargaEntregasTransferencia, unitOfWork),
                        DataPrevista = cargaEntregasTransferencia.OrderByDescending(o => o.DataPrevista).FirstOrDefault()?.DataPrevista?.ToDateTimeString()
                    },
                    Rota = new
                    {
                        CodigoPedido = pedido.Codigo,
                        Ocorrencias = (
                           from o in ocorrenciaColetaEntregasRota
                           select new
                           {
                               o.Codigo,
                               CodigoOcorrencia = o.TipoDeOcorrencia?.Codigo ?? 0,
                               Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(o.TipoDeOcorrencia, cargaEntregasRota.FirstOrDefault()?.Carga, pedido.Destinatario, pedido.Remetente),
                               DataOcorrencia = o.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                           }
                        ).ToList(),
                        Polilinhas = ObterPolilinhasControleEntrega(cargaEntregasRota, unitOfWork),
                        DataPrevista = cargaEntregasRota.OrderByDescending(o => o.DataPrevista).FirstOrDefault()?.DataPrevista?.ToDateTimeString()
                    },
                    Entrega = new
                    {
                        CodigoPedido = pedido.Codigo,
                        Codigo = cargaEntregaBase?.Codigo ?? 0,
                        AvaliacaoEfetuada = cargaEntregaBase?.AvaliacaoGeral.HasValue ?? false,
                        Avaliacao = cargaEntregaBase?.AvaliacaoGeral ?? 0,
                        Observacao = cargaEntregaBase?.ObservacaoAvaliacao ?? string.Empty,
                        DataPrevista = cargaEntregasRota.OrderByDescending(o => o.DataPrevista).FirstOrDefault()?.DataPrevista?.ToDateTimeString(),
                        InformacoesUltimaPerna = ObterCoordenadasUltimaPerna(pedido.Remetente, cargaEntregasRota.OrderByDescending(o => o.DataPrevista).FirstOrDefault()),
                        Polilinhas = ObterPolilinhasControleEntrega(cargaEntregasRota, unitOfWork),
                        Ocorrencias = (
                           from o in ocorrenciaColetaEntregasRota
                           select new
                           {
                               o.Codigo,
                               CodigoOcorrencia = o.TipoDeOcorrencia?.Codigo ?? 0,
                               Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(o.TipoDeOcorrencia, cargaEntregaBase?.Carga, pedido.Destinatario, pedido.Remetente),
                               DataOcorrencia = o.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                           }
                        ).ToList(),
                    }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCargaEntregaDoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorPedido(codigoPedido);

                if (cargaEntrega == null)
                    return new JsonpResult(false, "Não foi encontrada nenhuma entrega para esse pedido.");

                return new JsonpResult(
                    new
                    {
                        CargaEntrega = new { Codigo = cargaEntrega.Codigo, Descricao = cargaEntrega.Codigo },
                        Carga = new { Codigo = cargaEntrega.Carga.Codigo, Descricao = cargaEntrega.Carga.CodigoCargaEmbarcador }
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

        [AllowAuthenticate]
        public async Task<IActionResult> SolicitarCotacaoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosPedidos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));
                TipoModalCotacaoEspecial tipoModal = JsonConvert.DeserializeObject<TipoModalCotacaoEspecial>(Request.Params("TipoModal"));

                if (codigosPedidos == null || codigosPedidos.Count == 0)
                    throw new ControllerException("Nenhum pedido selecionado.");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.CotacaoEspecial repCotacao = new Repositorio.Embarcador.Pedidos.CotacaoEspecial(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedidos.BuscarPorCodigos(codigosPedidos);

                if (pedidos == null || pedidos.Count != codigosPedidos.Count)
                    throw new ControllerException("Um ou mais pedidos não foram encontrados.");

                decimal pesoTotal = pedidos.Sum(p => p.PesoTotal);
                decimal valorTotalNotasFiscais = pedidos.Sum(p => p.ValorTotalNotasFiscais);

                Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial cotacao = new Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial
                {
                    Pedidos = pedidos,
                    DataSolicitacao = DateTime.Now,
                    PesoTotal = pesoTotal,
                    StatusCotacaoEspecial = StatusCotacaoEspecial.AguardandoAnalise,
                    TipoModal = tipoModal,
                    ValorTotalNotasFiscais = valorTotalNotasFiscais,
                    Usuario = this.Usuario
                };

                repCotacao.Inserir(cotacao);

                return new JsonpResult(true, "Cotação de frete solicitada com sucesso.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar cotação de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Método Público

        private string Base64Imagem(string caminho, string nomeArquivo, string guidArquivo)
        {
            string extensao = Path.GetExtension(nomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensao);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                return null;

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private string Base64ImagemAnexo(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = RetornarCaminhoCanhoto(canhoto, unitOfWork);
            return Base64Imagem(caminho, canhoto.NomeArquivo, canhoto.GuidNomeArquivo);
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoCliente()
            {
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                SituacaoAcompanhamentoPedido = Request.GetNullableEnumParam<SituacaoAcompanhamentoPedido>("Situacao"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                DescricaoFiltro = Request.GetStringParam("Filtro"),
                CodigosTiposOperacao = Request.GetListParam<int>("TipoOperacao")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {

                if (IsVisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas())
                {
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> listaGruposPessoas = Usuario.ClienteFornecedor?.GruposPessoas.ToList() ?? null;
                    if(listaGruposPessoas != null)
                        filtrosPesquisa.ListaCodigosGruposPessoaPortalAcesso = listaGruposPessoas.Select(x => x.Codigo).ToList();
                }
                else
                {
                    if (IsCompartilharAcessoEntreGrupoPessoas())
                        filtrosPesquisa.CodigoGrupoPessoaClienteFornecedor = Usuario.ClienteFornecedor?.GrupoPessoas?.Codigo ?? 0;
                }

                if (Usuario.ClienteFornecedor != null)
                {
                    filtrosPesquisa.CpfCnpjClienteFornecedor = Usuario.ClienteFornecedor.CPF_CNPJ;

                    if(ConfiguracaoGeralCarga.PadraoVisualizacaoOperadorLogistico)
                        filtrosPesquisa.CodigoVendedor = ObterVendedor(Usuario.ClienteFornecedor.CPF_CNPJ_SemFormato, unitOfWork);
                }
                    

                filtrosPesquisa.ListaVendedor = ObterListaCodigoVendedoresPermitidosOperadorLogistica(unitOfWork);

            }

            return filtrosPesquisa;
        }

        private string RetornarCaminhoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            return Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ObterControleEntregasTransferencia(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            return repCargaEntrega.BuscarTransferenciaPorPedido(pedido.Codigo, pedido.Destinatario.CPF_CNPJ);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ObterControleEntregasRota(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            return repCargaEntrega.BuscarEntregaPorPedido(pedido.Codigo, pedido.Destinatario.CPF_CNPJ);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ObterOcorrenciasControleEntrega(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> controleEntregas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ocorrenciaColetaEntregas = repOcorrenciaColetaEntrega.BuscarPorControleEntregas(controleEntregas.Select(o => o.Codigo).ToList());

            return ocorrenciaColetaEntregas;
        }

        private dynamic ObterPolilinhasControleEntrega(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> controleEntregas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            List<int> codigosCarga = controleEntregas.Select(o => o.Carga.Codigo).ToList();

            if (codigosCarga.Count() == 0)
                return null;

            List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos = repositorioMonitoramento.BuscarPorCargas(codigosCarga);
            List<int> codigosMonitoramento = monitoramentos.Select(o => o.Codigo).ToList();

            if (codigosMonitoramento.Count() == 0)
                return null;

            List<string> planejadas = repositorioMonitoramento.BuscarDadosPolilinhasPlanejadasPorMonitoramentos(codigosMonitoramento);
            List<string> realizadas = repositorioMonitoramento.BuscarDadosPolilinhasRealizadasPorMonitoramentos(codigosMonitoramento);

            return new
            {
                Planejadas = planejadas.Where(o => o != null && o != string.Empty),
                Realizadas = realizadas.Where(o => o != null && o != string.Empty)
            };
        }

        private dynamic ObterInformacoesLocalizacaoCliente(Dominio.Entidades.Cliente cliente)
        {
            string endereco = string.IsNullOrWhiteSpace(cliente?.EnderecoCompleto) ? "" : $"{cliente.EnderecoCompleto} - {cliente.Localidade?.DescricaoCidadeEstado}";

            return new
            {
                Endereco = endereco,
                Coordenadas = ObterCoordenadasCliente(cliente)
            };
        }

        private dynamic ObterCoordenadasCliente(Dominio.Entidades.Cliente cliente)
        {
            string latitude = string.IsNullOrWhiteSpace(cliente?.Latitude) ? "0" : cliente.Latitude;
            string longitude = string.IsNullOrWhiteSpace(cliente?.Longitude) ? "0" : cliente.Longitude;

            return new
            {
                Latitude = double.Parse(latitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture),
                Longitude = double.Parse(longitude.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture)
            };
        }

        private dynamic ObterCoordenadasUltimaPerna(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            Dominio.Entidades.Cliente ultimoDestinatarioCarga = cargaEntrega?.Cliente;

            return new
            {
                Remetente = ObterInformacoesLocalizacaoCliente(remetente),
                Destinatario = ObterInformacoesLocalizacaoCliente(ultimoDestinatarioCarga)
            };
        }

        #endregion

    }
}
