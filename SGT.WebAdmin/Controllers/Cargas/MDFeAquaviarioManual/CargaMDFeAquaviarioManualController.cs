using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MDFeAquaviarioManual
{
    [CustomAuthorize("Cargas/CargaMDFeManual", "Cargas/CargaMDFeAquaviarioManual", "Cargas/Carga")]
    public class CargaMDFeAquaviarioManualController : BaseController
    {
		#region Construtores

		public CargaMDFeAquaviarioManualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrigem, numeroCTe, codigoCarga, numeroMDFe, codigoTransportador;
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("CTe"), out numeroCTe);
                int.TryParse(Request.Params("MDFe"), out numeroMDFe);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);

                int.TryParse(Request.Params("TerminalOrigem"), out int terminalOrigem);
                int.TryParse(Request.Params("TerminalDestino"), out int terminalDestino);
                int.TryParse(Request.Params("PedidoViagemNavio"), out int pedidoViagemNavio);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoTransportador = this.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoMDFe", false);
                grid.AdicionarCabecalho("MDF-e", "MDFe", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Terminal Destino", "TerminalDestino", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Viagem", "PedidoViagemNavio", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Qtd. SVM", "QtdSVM", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Qtd. AAK", "QtdAAK", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Qtd. Total", "QtdTotalDocumentos", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Status", 9, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "MDFe")
                {
                    propOrdenar = "Numero";
                }

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaCargaMDFeManual = repCargaMDFeManual.ConsultarAquaviario(terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario, 0, 0, codigoOrigem, 0, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int countCargaMDFeManual = repCargaMDFeManual.ContarConsultaAquaviario(terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario, 0, 0, codigoOrigem, 0, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, situacao);

                grid.setarQuantidadeTotal(countCargaMDFeManual);

                var listaOrdenadaMDFeManual = (from p in listaCargaMDFeManual
                                               select new
                                               {
                                                   Codigo = p.CodigoMDFeManual,
                                                   CodigoMDFe = p.Codigo,
                                                   MDFe = p.Numero,
                                                   p.TerminalOrigem,
                                                   p.TerminalDestino,
                                                   PedidoViagemNavio = p.PedidoViagemNavio?.Descricao ?? "",
                                                   DataEmissao = p.DataEmissao.HasValue ? p.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                                   p.QtdSVM,
                                                   p.QtdAAK,
                                                   p.QtdTotalDocumentos,
                                                   Status = p.DescricaoStatus
                                               }).ToList();

                grid.AdicionaRows(listaOrdenadaMDFeManual);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaParaCancelamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrigem, numeroCTe, codigoCarga, numeroMDFe, codigoTransportador;
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("CTe"), out numeroCTe);
                int.TryParse(Request.Params("MDFe"), out numeroMDFe);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoTransportador = this.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("MDF-e", "MDFe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", false);
                grid.AdicionarCabecalho("Motorista", false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Veiculo", false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> listaCargaMDFeManual = repCargaMDFeManual.ConsultarCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario, 0, 0, codigoOrigem, 0, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int countCargaMDFeManual = repCargaMDFeManual.ContarConsultaCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario, 0, 0, codigoOrigem, 0, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador);

                grid.setarQuantidadeTotal(countCargaMDFeManual);

                grid.AdicionaRows((from p in listaCargaMDFeManual
                                   select new
                                   {
                                       p.Codigo,
                                       Transportador = p.Empresa.Descricao,
                                       Descricao = string.Join(", ", p.MDFeManualMDFes.Select(o => o.MDFe.Numero)) + " - " + p.Origem.DescricaoCidadeEstado + " até " + (!p.UsarListaDestinos() ? p.Destino?.DescricaoCidadeEstado ?? "" : string.Join(",", (from obj in p.Destinos orderby obj.Ordem select obj.Localidade.DescricaoCidadeEstado).ToList())),
                                       MDFe = string.Join(", ", p.MDFeManualMDFes.Select(o => o.MDFe.Numero)),
                                       Origem = p.Origem.DescricaoCidadeEstado,
                                       Destino = !p.UsarListaDestinos() ? p.Destino?.DescricaoCidadeEstado ?? "" : string.Join(",", (from obj in p.Destinos orderby obj.Ordem select obj.Localidade.DescricaoCidadeEstado).ToList()),
                                       Motorista = p.Motoristas != null && p.Motoristas.Count > 0 ? string.Join(", ", p.Motoristas.Select(o => o.Descricao)) : "",
                                       Veiculo = p.Veiculo == null ? p.Veiculo.Placa + (p.Reboques.Count > 0 ? (", " + string.Join(", ", p.Reboques.Select(o => o.Placa))) : string.Empty) : "",
                                       Situacao = p.DescricaoSituacao
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string erro = string.Empty;

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = null;

                if (!SalvarMDFeManual(out erro, ref cargaMDFeManual, false, unidadeTrabalho))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManual, null, "Salvou dados do MDF-e", unidadeTrabalho);

                return new JsonpResult(ObterDetalhesMDFeManual(cargaMDFeManual, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar o MDF-e manual.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Emitir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = codigo > 0 ? repCargaMDFeManual.BuscarPorCodigo(codigo) : null;

                string erro = string.Empty;
                if (!SalvarMDFeManual(out erro, ref cargaMDFeManual, true, unidadeTrabalho))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManual, null, "Solicitou emissão do MDF-e", unidadeTrabalho);

                return new JsonpResult(ObterDetalhesMDFeManual(cargaMDFeManual, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao emitir o MDF-e manual.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigo);

                if (cargaMDFeManual == null)
                    return new JsonpResult(false, true, "MDF-e manual não encontrado.");

                return new JsonpResult(ObterDetalhesMDFeManual(cargaMDFeManual, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar o MDF-e manual.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCTesAutomaticamente()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);
            try
            {
                int codigoPedidoViagemNavio;
                int.TryParse(Request.Params("PedidoViagemNavio"), out codigoPedidoViagemNavio);

                int cnpjPortoOrigem, cnpjPortoDestino;
                int.TryParse(Request.Params("PortoOrigem"), out cnpjPortoOrigem);
                int.TryParse(Request.Params("PortoDestino"), out cnpjPortoDestino);
                List<int> codigosTerminalsOrigem = new List<int>();
                codigosTerminalsOrigem = RetornaCodigosTerminaisOrigem(unidadeTrabalho);

                List<int> codigosTerminalsDestino = new List<int>();
                codigosTerminalsDestino = RetornaCodigosTerminaisDestino(unidadeTrabalho);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                string cnpjSeguradoraPadrao = string.Empty;
                string nomeSeguradoraPadrao = string.Empty;
                string apoliceSeguroPadrao = string.Empty;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                cargaCTes = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino);
                cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
                cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
                cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
                cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
                cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
                if (cnpjPortoDestino > 0)
                    cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem);

                cargaCTes.AddRange(cargaCTesTransbordoPerna1);
                cargaCTes.AddRange(cargaCTesTransbordoPerna2);
                cargaCTes.AddRange(cargaCTesTransbordoPerna3);
                cargaCTes.AddRange(cargaCTesTransbordoPerna4);
                cargaCTes.AddRange(cargaCTesTransbordoPerna5);
                cargaCTes.AddRange(cargaCTesTransbordoPortoDestino);

                cargaCTes = cargaCTes.Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesAdicionar = ValidarConhecimentosPendentesEmissao(cargaCTes, cnpjPortoOrigem, cnpjPortoDestino, unidadeTrabalho);

                if (cargaCTesAdicionar != null && cargaCTesAdicionar.Count > 0)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTesAdicionar.FirstOrDefault().CTe;

                    cnpjSeguradoraPadrao = cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.CNPJSeguro) ? cte.Empresa.Configuracao.CNPJSeguro :
                                           cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? cte.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                           cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? cte.Empresa.CNPJ : string.Empty;

                    nomeSeguradoraPadrao = cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.NomeSeguro) ? cte.Empresa.Configuracao.NomeSeguro.Length > 30 ? cte.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : cte.Empresa.Configuracao.NomeSeguro :
                                           cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? cte.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? cte.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : cte.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                           cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? cte.Empresa.RazaoSocial.Length > 30 ? cte.Empresa.RazaoSocial.Substring(0, 30) : cte.Empresa.RazaoSocial : string.Empty;

                    apoliceSeguroPadrao = cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.NumeroApoliceSeguro) ? cte.Empresa.Configuracao.NumeroApoliceSeguro.Length > 30 ? cte.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : cte.Empresa.Configuracao.NumeroApoliceSeguro :
                                          cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? cte.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 30 ? cte.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : cte.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                }

                var retorno = (from obj in cargaCTesAdicionar
                               select new
                               {
                                   obj.Codigo,
                                   CodigoOrigem = obj.CTe.LocalidadeInicioPrestacao.Codigo,
                                   Origem = obj.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                   CodigoDestino = obj.CTe.LocalidadeTerminoPrestacao.Codigo,
                                   CNPJSeguradora = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].CNPJSeguradora) ? obj.CTe.Seguros[0].CNPJSeguradora : cnpjSeguradoraPadrao,
                                   NomeSeguradora = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NomeSeguradora) ? obj.CTe.Seguros[0].NomeSeguradora : nomeSeguradoraPadrao,
                                   NumeroApolice = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NumeroApolice) ? obj.CTe.Seguros[0].NumeroApolice : apoliceSeguroPadrao,
                                   NumeroAverbacao = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 ? !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NumeroAverbacao) ? obj.CTe.Seguros[0].NumeroAverbacao : svcMDFe.BuscarAverbacaoCTe(obj.CTe.Codigo, obj.CTe.Empresa.Codigo, unidadeTrabalho) : svcMDFe.BuscarAverbacaoCTe(obj.CTe.Codigo, obj.CTe.Empresa.Codigo, unidadeTrabalho),
                                   TipoSeguro = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 ? obj.CTe.Seguros[0].Tipo == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente,
                                   CNPJEmpresa = obj.CTe.Empresa.CNPJ_SemFormato,
                                   CodigoCTE = obj.CTe.Codigo,
                                   Carga = obj.Carga.CodigoCargaEmbarcador,
                                   Numero = obj.CTe.Numero + " - " + obj.CTe.Serie.Numero,
                                   CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                   Serie = obj.CTe.Serie.Numero,
                                   Notas = string.Join(", ", obj.CTe.XMLNotaFiscais.Select(o => o.Numero.ToString())),
                                   Remetente = obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")",
                                   Destinatario = obj.CTe.Destinatario.Cliente.Descricao,
                                   Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                   ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                               }).ToList();

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar o MDF-e manual.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarMensagemSefaz()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = Request.GetIntParam("Numero");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = repCargaMDFeManualMDFe.BuscarPorMDFe(mdfe?.Codigo ?? 0);

                if (cargaMDFeManualMDFe == null)
                    return new JsonpResult(false, true, "Mensagem Retorno Sefaz não encontrado.");

                if (cargaMDFeManualMDFe?.CargaMDFeManual?.MDFeRecebidoDeIntegracao ?? false)
                    return new JsonpResult(false, false, "Esta opção não pode ser realizada para MDF'es recebido via integração.");

                var RetornoSefaz = cargaMDFeManualMDFe?.MDFe?.MensagemRetornoSefaz ?? string.Empty;

                return new JsonpResult(RetornoSefaz);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar Mensagem Retorno Sefaz.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = Request.GetIntParam("Numero");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho); 
                Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = repCargaMDFeManualMDFe.BuscarPorMDFe(mdfe?.Codigo ?? 0);

                if (cargaMDFeManualMDFe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if(cargaMDFeManualMDFe?.CargaMDFeManual?.MDFeRecebidoDeIntegracao ?? false)
                    return new JsonpResult(false, false, "Esta opção não pode ser realizada para MDF'es recebido via integração.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para realizar o download do XML.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLAutorizacao(mdfe, unidadeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDAMDFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = Request.GetIntParam("Numero");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = repCargaMDFeManualMDFe.BuscarPorMDFe(mdfe?.Codigo ?? 0);

                if (cargaMDFeManualMDFe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (cargaMDFeManualMDFe?.CargaMDFeManual?.MDFeRecebidoDeIntegracao ?? false)
                    return new JsonpResult(false, false, "Esta opção não pode ser realizada para MDF'es recebido via integração.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para o download do DAMDFE.");

                Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeTrabalho);

                byte[] arquivo = null;

                if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (arquivo == null)
                    arquivo = svcDAMDFE.Gerar(mdfe.Codigo);

                if (arquivo != null)
                    return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Chave, ".pdf"));
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o DAMDFE, atualize a página e tente novamente.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do DAMDFE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesMDFeManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> cargaMDFeManualDestinos = repDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino cargaMDFeManualDestino in cargaMDFeManualDestinos)
                destinos.Add(cargaMDFeManualDestino.Localidade);

            List<Dominio.Entidades.Localidade> localidadesMDFe = new List<Dominio.Entidades.Localidade>();
            localidadesMDFe.Add(cargaMDFeManual.Origem);
            foreach (Dominio.Entidades.Localidade destino in destinos)
                localidadesMDFe.Add(destino);

            var retorno = new
            {
                cargaMDFeManual.Codigo,
                cargaMDFeManual.Situacao,
                cargaMDFeManual.SituacaoCancelamento,
                cargaMDFeManual.UsarDadosCTe,
                cargaMDFeManual.UsarSeguroCTe,
                Empresa = new { cargaMDFeManual.Empresa.Codigo, Descricao = cargaMDFeManual.Empresa.Descricao },
                CTes = (from obj in cargaMDFeManual.CTes
                        select new
                        {
                            obj.Codigo,
                            CodigoCTE = obj.CTe.Codigo,
                            Carga = obj.Carga.CodigoCargaEmbarcador,
                            Numero = obj.CTe.Numero + " - " + obj.CTe.Serie.Numero,
                            CodigoEmpresa = obj.CTe.Empresa.Codigo,
                            Serie = obj.CTe.Serie.Numero,
                            Notas = string.Join(", ", obj.CTe.XMLNotaFiscais.Select(o => o.Numero.ToString())),
                            Remetente = obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")",
                            Destinatario = obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")",
                            Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                            ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                        }).ToList(),
                Cargas = (from obj in cargaMDFeManual.Cargas
                          select new
                          {
                              Codigo = obj.Codigo,
                              CodigoCargaEmbarcador = obj.CodigoCargaEmbarcador,
                              Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                              OrigemDestino = serCargaDadosSumarizados.ObterOrigemDestinos(obj, false, TipoServicoMultisoftware),
                              Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.Localidade.DescricaoCidadeEstado + " )" : string.Empty,
                              Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : "",
                              DataCarregamento = obj.DataCarregamentoCarga.HasValue ? obj.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy") : "",
                              NumeroCTes = obj.NumerosCTes
                          }).ToList(),
                Destino = cargaMDFeManual.Destino != null ? new
                {
                    Codigo = cargaMDFeManual.Destino.Codigo,
                    Descricao = cargaMDFeManual.Destino.DescricaoCidadeEstado
                } : new { Codigo = 0, Descricao = "" },
                Origem = new
                {
                    Codigo = cargaMDFeManual.Origem.Codigo,
                    Descricao = cargaMDFeManual.Origem.DescricaoCidadeEstado
                },
                PedidoViagemNavio = new
                {
                    Codigo = cargaMDFeManual.PedidoViagemNavio?.Codigo ?? 0,
                    Descricao = cargaMDFeManual.PedidoViagemNavio?.Descricao ?? ""
                },
                PortoOrigem = new
                {
                    Codigo = cargaMDFeManual.PortoOrigem?.Codigo ?? 0,
                    Descricao = cargaMDFeManual.PortoOrigem?.Descricao ?? ""
                },
                PortoDestino = new
                {
                    Codigo = cargaMDFeManual.PortoDestino?.Codigo ?? 0,
                    Descricao = cargaMDFeManual.PortoDestino?.Descricao ?? ""
                },
                TerminaisCarregamento = (from obj in cargaMDFeManual.TerminalCarregamento
                                         select new
                                         {
                                             Codigo = obj.Codigo,
                                             Descricao = obj.Descricao
                                         }).ToList(),
                TerminaisDescarregamento = (from obj in cargaMDFeManual.TerminalDescarregamento
                                            select new
                                            {
                                                Codigo = obj.Codigo,
                                                Descricao = obj.Descricao
                                            }).ToList(),
                Destinos = (from obj in cargaMDFeManualDestinos
                            select new
                            {
                                Codigo = obj.Localidade.Codigo,
                                Descricao = obj.Localidade.DescricaoCidadeEstado,
                                Posicao = obj.Ordem
                            }).ToList()
            };

            return retorno;
        }

        private bool SalvarMDFeManual(out string erro, ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, bool emitirMDFe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            int codigo, codigoOrigem, codigoEmpresa, codigoPedidoViagemNavio;
            int.TryParse(Request.Params("Codigo"), out codigo);
            int.TryParse(Request.Params("Origem"), out codigoOrigem);
            int.TryParse(Request.Params("PedidoViagemNavio"), out codigoPedidoViagemNavio);
            int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

            int portoOrigem, portoDestino;
            int.TryParse(Request.Params("PortoOrigem"), out portoOrigem);
            int.TryParse(Request.Params("PortoDestino"), out portoDestino);

            bool usarDadosCTe = false;
            bool.TryParse(Request.Params("UsarDadosCTe"), out usarDadosCTe);

            List<int> codigosTerminalsOrigem = new List<int>();
            codigosTerminalsOrigem = RetornaCodigosTerminaisOrigem(unidadeTrabalho);

            List<int> codigosTerminalsDestino = new List<int>();
            codigosTerminalsDestino = RetornaCodigosTerminaisDestino(unidadeTrabalho);

            bool usarSeguroCTe = false;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                codigoEmpresa = this.Empresa.Codigo;

            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);

            unidadeTrabalho.Start();


            cargaMDFeManual = codigo > 0 ? repCargaMDFeManual.BuscarPorCodigo(codigo) : null;

            if (cargaMDFeManual == null)
            {
                cargaMDFeManual = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual();
                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao;
            }

            if (cargaMDFeManual.MDFeManualMDFes != null && cargaMDFeManual.MDFeManualMDFes.Count > 0)
            {
                erro = "Não é possível salvar as informações pois o MDF-e já foi gerado.";
                unidadeTrabalho.Rollback();
                return false;
            }
            if (codigosTerminalsOrigem == null || codigosTerminalsOrigem.Count == 0)
            {
                erro = "Favor informe ao menos um terminal de carregamento.";
                unidadeTrabalho.Rollback();
                return false;
            }
            if (codigosTerminalsDestino == null || codigosTerminalsDestino.Count == 0)
            {
                erro = "Favor informe ao menos um terminal de descarregamento.";
                unidadeTrabalho.Rollback();
                return false;
            }
            cargaMDFeManual.TipoModalMDFe = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Aquaviario;
            cargaMDFeManual.PortoOrigem = repPorto.BuscarPorCodigo(portoOrigem);
            cargaMDFeManual.PortoDestino = repPorto.BuscarPorCodigo(portoDestino);
            cargaMDFeManual.Destino = cargaMDFeManual.PortoDestino?.Localidade;
            cargaMDFeManual.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemNavio);
            cargaMDFeManual.Origem = repLocalidade.BuscarPorCodigo(codigoOrigem);
            cargaMDFeManual.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            //cargaMDFeManual.UsarDadosCTe = usarDadosCTe;
            cargaMDFeManual.UsarDadosCTe = false;
            cargaMDFeManual.UsarSeguroCTe = usarSeguroCTe;

            if (cargaMDFeManual.Reboques == null)
                cargaMDFeManual.Reboques = new List<Dominio.Entidades.Veiculo>();
            else
                cargaMDFeManual.Reboques.Clear();

            SetarTerminais(cargaMDFeManual, unidadeTrabalho);

            if (cargaMDFeManual.Codigo > 0)
                repCargaMDFeManual.Atualizar(cargaMDFeManual);
            else
                repCargaMDFeManual.Inserir(cargaMDFeManual);

            erro = SalvarCargas(ref cargaMDFeManual, unidadeTrabalho);
            if (!string.IsNullOrWhiteSpace(erro))
            {
                unidadeTrabalho.Rollback();
                return false;
            }
            erro = SalvarCTes(ref cargaMDFeManual, unidadeTrabalho);
            if (!string.IsNullOrWhiteSpace(erro))
            {
                unidadeTrabalho.Rollback();
                return false;
            }

            repCargaMDFeManual.Atualizar(cargaMDFeManual);
            if ((cargaMDFeManual.CTes == null || cargaMDFeManual.CTes.Count <= 0) && (cargaMDFeManual.Cargas == null || cargaMDFeManual.Cargas.Count <= 0))
            {
                erro = "É necessário adicionar ao menos um CT-e ou uma Carga para salvar o MDF-e manual.";
                unidadeTrabalho.Rollback();
                return false;
            }

            //Contem conhecimentos pendentes
            if (cargaMDFeManual.TerminalCarregamento == null || cargaMDFeManual.TerminalCarregamento.Count() == 0)
            {
                erro = "É necessário adicionar ao menos um terminal de carregamento para salvar o MDF-e manual.";
                unidadeTrabalho.Rollback();
                return false;
            }
            if (cargaMDFeManual.PedidoViagemNavio == null)
            {
                erro = "É necessário informar um Navio/Viagem/Direção para salvar o MDF-e manual.";
                unidadeTrabalho.Rollback();
                return false;
            }
            if (ContemConhecimentoPendentesAutorizacao(codigoPedidoViagemNavio, portoOrigem, portoDestino, codigosTerminalsOrigem, codigosTerminalsDestino, unidadeTrabalho))
            {
                erro = "Existem CT-es pendentes de autorização, favor verifique os mesmos antes de gerar o MDF-e.<br/>";

                string entidades = RetornarConhecimentoPendentesAutorizacao(codigoPedidoViagemNavio, portoOrigem, portoDestino, codigosTerminalsOrigem, codigosTerminalsDestino, unidadeTrabalho);
                erro += entidades;

                unidadeTrabalho.Rollback();
                return false;
            }
            if (ContemCargaPendentesAutorizacao(codigoPedidoViagemNavio, portoOrigem, portoDestino, codigosTerminalsOrigem, codigosTerminalsDestino, unidadeTrabalho))
            {
                erro = "Existem Cargas pendentes de autorização, favor verifique os mesmos antes de gerar o MDF-e.<br/>";

                string entidades = RetornarCargaPendentesAutorizacao(codigoPedidoViagemNavio, portoOrigem, portoDestino, codigosTerminalsOrigem, codigosTerminalsDestino, unidadeTrabalho);
                erro += entidades;

                unidadeTrabalho.Rollback();
                return false;
            }

            if (ContemPedidosPendentesAutorizacao(codigoPedidoViagemNavio, portoOrigem, portoDestino, codigosTerminalsOrigem, codigosTerminalsDestino, unidadeTrabalho))
            {
                erro = "Existem Pedidos pendentes de realizar a sua Montagem, ou com a sua carga pendente de emissão. Favor verifique os mesmos antes de gerar o MDF-e.<br/>";

                string entidades = RetornarPedidosPendentesAutorizacao(codigoPedidoViagemNavio, portoOrigem, portoDestino, codigosTerminalsOrigem, codigosTerminalsDestino, unidadeTrabalho);
                erro += entidades;

                unidadeTrabalho.Rollback();
                return false;
            }

            if (ContemBookingPendenteEmissaoSVM(codigosTerminalsOrigem, codigosTerminalsDestino, codigoPedidoViagemNavio, unidadeTrabalho))
            {
                erro = "Existem Booking's pendentes de realizar o SVM, favor verifique os mesmos antes de gerar o MDF-e.<br/>";

                string entidades = RetornaListaBookingPendenteEmissaoSVM(codigosTerminalsOrigem, codigosTerminalsDestino, codigoPedidoViagemNavio, unidadeTrabalho);
                erro += entidades;

                unidadeTrabalho.Rollback();
                return false;
            }

            if (ContemCTesNaoVinculadosAoMDFe(cargaMDFeManual, codigosTerminalsOrigem, codigosTerminalsDestino, codigoPedidoViagemNavio, portoOrigem, portoDestino, unidadeTrabalho))
            {
                erro = "Existem CT-es autorizados e não vinculados neste MDF-e.<br/>";

                string entidades = RetornaListaCTesNaoVinculadosAoMDFe(cargaMDFeManual, codigosTerminalsOrigem, codigosTerminalsDestino, codigoPedidoViagemNavio, portoOrigem, portoDestino, unidadeTrabalho);
                erro += entidades;

                unidadeTrabalho.Rollback();
                return false;
            }

            unidadeTrabalho.CommitChanges();
            if (emitirMDFe)
            {
                if (!GerarMDFe(out erro, cargaMDFeManual.Codigo, unidadeTrabalho))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        private void SetarTerminais(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

            dynamic terminaisCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TerminaisCarregamento"));
            cargaMDFeManual.TerminalCarregamento = new List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            if (terminaisCarregamento != null && terminaisCarregamento.Count > 0)
            {
                foreach (var terminalCarregamento in terminaisCarregamento)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = repTipoTerminalImportacao.BuscarPorCodigo((int)terminalCarregamento.Codigo);

                    cargaMDFeManual.TerminalCarregamento.Add(terminal);
                }
            }

            dynamic terminaisDescarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TerminaisDescarregamento"));
            cargaMDFeManual.TerminalDescarregamento = new List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            if (terminaisDescarregamento != null && terminaisDescarregamento.Count > 0)
            {
                foreach (var terminalDescarregamento in terminaisDescarregamento)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = repTipoTerminalImportacao.BuscarPorCodigo((int)terminalDescarregamento.Codigo);

                    cargaMDFeManual.TerminalDescarregamento.Add(terminal);
                }
            }
        }

        private string SalvarCargas(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

            List<int> codigosCargas = JsonConvert.DeserializeObject<List<int>>(Request.Params("Cargas"));

            if (cargaMDFeManual.Cargas == null)
            {
                cargaMDFeManual.Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasDeletar = cargaMDFeManual.Cargas.Where(o => !codigosCargas.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaDeletar in cargasDeletar)
                    cargaMDFeManual.Cargas.Remove(cargaDeletar);
            }

            foreach (int codigoCarga in codigosCargas)
            {
                if (!cargaMDFeManual.Cargas.Any(o => o.Codigo == codigoCarga))
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                    cargaMDFeManual.Cargas.Add(carga);
                }
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualExiste = repCargaMDFeManual.VerificarCargaMDFeManualComCarga(codigoCarga);
                if (cargaMDFeManualExiste != null && cargaMDFeManualExiste.Codigo != cargaMDFeManual.Codigo)
                    return "Existem Cargas que estão alocadas em outro MDF-e manual";
            }
            return "";
        }

        private string SalvarCTes(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

            List<int> codigosCargaCTe = JsonConvert.DeserializeObject<List<int>>(Request.Params("CTes"));

            if (cargaMDFeManual.CTes == null)
            {
                cargaMDFeManual.CTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesDeletar = cargaMDFeManual.CTes.Where(o => !codigosCargaCTe.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeDeletar in cargaCTesDeletar)
                    cargaMDFeManual.CTes.Remove(cargaCTeDeletar);
            }

            foreach (int codigoCargaCTe in codigosCargaCTe)
            {
                if (!cargaMDFeManual.CTes.Any(o => o.Codigo == codigoCargaCTe))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                    cargaMDFeManual.CTes.Add(cargaCTe);

                    if (cargaCTe.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || cargaCTe.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada || cargaCTe.CTe.Status != "A")
                        if (cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                            return "Existe a carga e ou CT-e que não está autorizados:<br/>  Nº Controle(" + cargaCTe.CTe.NumeroControle + ") BK(" + cargaCTe.CTe.NumeroBooking + ") ainda não se encontra autorizado. <br/> ";
                }
            }
            return "";
        }

        private bool GerarMDFe(out string erro, int codigoMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {

            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualLacre repLacre = new Repositorio.Embarcador.Cargas.CargaMDFeManualLacre(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);

            Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoMDFeManual);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre> lacres = repLacre.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursos = repPercurso.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();
            if (cargaMDFeManual.UsarListaDestinos())
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> destinosMDF = repCargaMDFeManualDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino destino in destinosMDF)
                    destinos.Add(destino.Localidade);

            }
            else
                destinos.Add(cargaMDFeManual.Destino);

            bool exigeEDIFiscalMT = cargaMDFeManual.Empresa?.Configuracao?.ExigeEDIFiscalMT ?? false;

            if (exigeEDIFiscalMT && (percursos.Any(o => o.Estado.Sigla == "MT") || (cargaMDFeManual.Origem.Estado.Sigla != "MT" && destinos.Any(obj => obj.Estado.Sigla == "MT"))) && lacres.Count <= 0)
            {
                erro = "O MDF-e possui passagem pelo MT, sendo necessário informar um lacre.";
                return false;
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            string retorno = svcMDFe.EmitirMDFeAquaviario(cargaMDFeManual, configuracaoTMS, TipoServicoMultisoftware, WebServiceConsultaCTe, unidadeTrabalho);

            if (string.IsNullOrWhiteSpace(retorno))
            {
                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                repCargaMDFeManual.Atualizar(cargaMDFeManual);
                erro = string.Empty;
                return true;
            }
            else
            {
                erro = retorno;
                return false;
            }
        }

        private List<int> RetornaCodigosTerminaisOrigem(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTerminalOrigem")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTerminalOrigem"));
                if (listaTitulo != null)
                {
                    foreach (var titulo in listaTitulo)
                    {
                        listaCodigos.Add(int.Parse((string)titulo.Codigo));
                    }
                }
            }
            return listaCodigos;
        }

        private List<int> RetornaCodigosTerminaisDestino(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaTerminalDestino")))
            {
                dynamic listaTitulo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTerminalDestino"));
                if (listaTitulo != null)
                {
                    foreach (var titulo in listaTitulo)
                    {
                        listaCodigos.Add(int.Parse((string)titulo.Codigo));
                    }
                }
            }
            return listaCodigos;
        }

        private bool ContemConhecimentoPendentesAutorizacao(int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino,
          Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            cargaCTes = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem, false);

            cargaCTes.AddRange(cargaCTesTransbordoPerna1);
            cargaCTes.AddRange(cargaCTesTransbordoPerna2);
            cargaCTes.AddRange(cargaCTesTransbordoPerna3);
            cargaCTes.AddRange(cargaCTesTransbordoPerna4);
            cargaCTes.AddRange(cargaCTesTransbordoPerna5);
            cargaCTes.AddRange(cargaCTesTransbordoPortoDestino);

            return cargaCTes.Count > 0;
        }

        private string RetornarConhecimentoPendentesAutorizacao(int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino,
          Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            cargaCTes = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino, false);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem, false);

            cargaCTes.AddRange(cargaCTesTransbordoPerna1);
            cargaCTes.AddRange(cargaCTesTransbordoPerna2);
            cargaCTes.AddRange(cargaCTesTransbordoPerna3);
            cargaCTes.AddRange(cargaCTesTransbordoPerna4);
            cargaCTes.AddRange(cargaCTesTransbordoPerna5);
            cargaCTes.AddRange(cargaCTesTransbordoPortoDestino);

            cargaCTes = cargaCTes.Distinct().ToList();

            List<string> entidades = new List<string>();
            if (cargaCTes != null && cargaCTes.Count > 0)
            {
                foreach (var cargaCTe in cargaCTes)
                {
                    if (cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        string entidade = "Nº Controle (" + cargaCTe.CTe.NumeroControle + ") BK (" + cargaCTe.CTe.NumeroBooking + ") ainda não se encontra autorizado. <br/>";
                        if (!entidades.Contains(entidade))
                            entidades.Add(entidade);
                    }
                }
            }
            if (entidades.Count > 0)
                return string.Join(" ", entidades);
            else
                return "";
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ValidarConhecimentosPendentesEmissao(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, int codigoPortoOrigem, int codigoPortoDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> retornoCargaCTe = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (cargaCTes != null && cargaCTes.Count > 0)
            {
                foreach (var cte in cargaCTes)
                {
                    if (cte != null)
                    {
                        if (codigoPortoDestino > 0)
                        {
                            if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, codigoPortoOrigem, codigoPortoDestino))
                            {
                                retornoCargaCTe.Add(cte);
                            }
                        }
                        else
                        {
                            bool caminhoMDFeEncontrado = false;
                            if (cte.CTe.PortoPassagemUm != null)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, codigoPortoOrigem, cte.CTe.PortoPassagemUm.Codigo))
                                {
                                    caminhoMDFeEncontrado = true;
                                    retornoCargaCTe.Add(cte);
                                }
                            }
                            if (cte.CTe.PortoPassagemDois != null && !caminhoMDFeEncontrado)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, codigoPortoOrigem, cte.CTe.PortoPassagemDois.Codigo))
                                {
                                    caminhoMDFeEncontrado = true;
                                    retornoCargaCTe.Add(cte);
                                }
                            }
                            if (cte.CTe.PortoPassagemTres != null && !caminhoMDFeEncontrado)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, codigoPortoOrigem, cte.CTe.PortoPassagemTres.Codigo))
                                {
                                    caminhoMDFeEncontrado = true;
                                    retornoCargaCTe.Add(cte);
                                }
                            }
                            if (cte.CTe.PortoPassagemQuatro != null && !caminhoMDFeEncontrado)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, codigoPortoOrigem, cte.CTe.PortoPassagemQuatro.Codigo))
                                {
                                    caminhoMDFeEncontrado = true;
                                    retornoCargaCTe.Add(cte);
                                }
                            }
                            if (cte.CTe.PortoPassagemCinco != null && !caminhoMDFeEncontrado)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, codigoPortoOrigem, cte.CTe.PortoPassagemCinco.Codigo))
                                {
                                    caminhoMDFeEncontrado = true;
                                    retornoCargaCTe.Add(cte);
                                }
                            }
                            if (cte.CTe.PortoDestino != null && !caminhoMDFeEncontrado)
                            {
                                if (!repMDFe.VerificarSeExisteMDFeGerado(cte.CTe.Chave, codigoPortoOrigem, cte.CTe.PortoDestino.Codigo))
                                {
                                    caminhoMDFeEncontrado = true;
                                    retornoCargaCTe.Add(cte);
                                }
                            }
                        }
                    }
                }
            }

            return retornoCargaCTe;
        }

        private bool ContemCargaPendentesAutorizacao(int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino,
          Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            carga = repCargaCTe.ConsultarCargaPendenteParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarCargasPendenteParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem);

            carga.AddRange(cargaCTesTransbordoPerna1);
            carga.AddRange(cargaCTesTransbordoPerna2);
            carga.AddRange(cargaCTesTransbordoPerna3);
            carga.AddRange(cargaCTesTransbordoPerna4);
            carga.AddRange(cargaCTesTransbordoPerna5);
            carga.AddRange(cargaCTesTransbordoPortoDestino);

            return carga.Count > 0;
        }

        private string RetornarCargaPendentesAutorizacao(int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino,
          Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            carga = repCargaCTe.ConsultarCargaPendenteParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarCargasPendenteParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem);

            carga.AddRange(cargaCTesTransbordoPerna1);
            carga.AddRange(cargaCTesTransbordoPerna2);
            carga.AddRange(cargaCTesTransbordoPerna3);
            carga.AddRange(cargaCTesTransbordoPerna4);
            carga.AddRange(cargaCTesTransbordoPerna5);
            carga.AddRange(cargaCTesTransbordoPortoDestino);

            List<string> entidades = new List<string>();
            if (carga != null && carga.Count > 0)
            {
                foreach (var cargaCTe in carga)
                {
                    if (cargaCTe.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        string entidade = "Nº Carga (" + cargaCTe.CodigoCargaEmbarcador + ") ainda não se encontra autorizado. <br/>";
                        if (!entidades.Contains(entidade))
                            entidades.Add(entidade);
                    }
                }
            }
            if (entidades.Count > 0)
                return string.Join(" ", entidades);
            else
                return "";
        }

        private bool ContemPedidosPendentesAutorizacao(int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino,
          Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> carga = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            carga = repCargaCTe.ConsultarPedidosPendenteParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarPedidosPendenteParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem);

            carga.AddRange(cargaCTesTransbordoPerna1);
            carga.AddRange(cargaCTesTransbordoPerna2);
            carga.AddRange(cargaCTesTransbordoPerna3);
            carga.AddRange(cargaCTesTransbordoPerna4);
            carga.AddRange(cargaCTesTransbordoPerna5);
            carga.AddRange(cargaCTesTransbordoPortoDestino);

            return carga.Count > 0;
        }

        private string RetornarPedidosPendentesAutorizacao(int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino,
          Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> carga = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            carga = repCargaCTe.ConsultarPedidosPendenteParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarPedidosPendenteParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem);

            carga.AddRange(cargaCTesTransbordoPerna1);
            carga.AddRange(cargaCTesTransbordoPerna2);
            carga.AddRange(cargaCTesTransbordoPerna3);
            carga.AddRange(cargaCTesTransbordoPerna4);
            carga.AddRange(cargaCTesTransbordoPerna5);
            carga.AddRange(cargaCTesTransbordoPortoDestino);

            List<string> entidades = new List<string>();
            if (carga != null && carga.Count > 0)
            {
                foreach (var cargaCTe in carga)
                {
                    string strTomador = "";
                    if (cargaCTe.ObterTomador() != null)
                        strTomador = "Tomador (" + (cargaCTe.ObterTomador().GrupoPessoas?.Descricao ?? cargaCTe.ObterTomador().Descricao) + ")";
                    string entidade = "Nº Pedido (" + cargaCTe.Numero.ToString() + ") BK (" + cargaCTe.NumeroBooking + ") " + strTomador + " ainda não se encontra vinculado a alguma carga ou a carga está pendente de emissão. <br/>";
                    if (!entidades.Contains(entidade))
                        entidades.Add(entidade);
                }
            }
            if (entidades.Count > 0)
                return string.Join(" ", entidades);
            else
                return "";
        }

        private bool ContemBookingPendenteEmissaoSVM(List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino, int codigoPedidoViagemNavio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(codigoPedidoViagemNavio, codigosTerminalsOrigem, codigosTerminalsDestino, situacoesPermitidas);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(codigoPedidoViagemNavio, codigosTerminalsOrigem, codigosTerminalsDestino, situacoesPermitidas);

            List<string> listaBookings = new List<string>();
            if (cargaCTes != null && cargaCTes.Count > 0)
                return true;
            if (cargaCTesTransbordo != null && cargaCTesTransbordo.Count > 0)
                return true;

            return false;
        }

        private string RetornaListaBookingPendenteEmissaoSVM(List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino, int codigoPedidoViagemNavio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(codigoPedidoViagemNavio, codigosTerminalsOrigem, codigosTerminalsDestino, situacoesPermitidas);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(codigoPedidoViagemNavio, codigosTerminalsOrigem, codigosTerminalsDestino, situacoesPermitidas);

            List<string> listaBookings = new List<string>();
            foreach (var cargaCTe in cargaCTes)
            {
                if (!string.IsNullOrWhiteSpace(cargaCTe.CTe.NumeroBooking))
                {
                    if (!listaBookings.Contains(cargaCTe.CTe.NumeroBooking))
                    {
                        string entidade = "Booking (" + cargaCTe.CTe.NumeroBooking + ") não possui SVM gerado. <br/>";
                        listaBookings.Add(entidade);
                    }
                }
            }

            foreach (var cargaCTe in cargaCTesTransbordo)
            {
                if (!string.IsNullOrWhiteSpace(cargaCTe.CTe.NumeroBooking))
                {
                    if (!listaBookings.Contains(cargaCTe.CTe.NumeroBooking))
                    {
                        string entidade = "Booking (" + cargaCTe.CTe.NumeroBooking + ") não possui SVM gerado. <br/>";
                        listaBookings.Add(entidade);
                    }
                }
            }
            if (listaBookings.Count > 0)
                return string.Join(" ", listaBookings);
            else
                return "";
        }


        private bool ContemCTesNaoVinculadosAoMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino, int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            cargaCTes = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem);

            cargaCTes.AddRange(cargaCTesTransbordoPerna1);
            cargaCTes.AddRange(cargaCTesTransbordoPerna2);
            cargaCTes.AddRange(cargaCTesTransbordoPerna3);
            cargaCTes.AddRange(cargaCTesTransbordoPerna4);
            cargaCTes.AddRange(cargaCTesTransbordoPerna5);
            cargaCTes.AddRange(cargaCTesTransbordoPortoDestino);

            cargaCTes = cargaCTes.Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesAdicionar = ValidarConhecimentosPendentesEmissao(cargaCTes, cnpjPortoOrigem, cnpjPortoDestino, unitOfWork);
            if (cargaCTesAdicionar != null && cargaCTesAdicionar.Count > 0)
            {
                foreach (var cargaCTe in cargaCTesAdicionar)
                {
                    if (!cargaMDFeManual.CTes.Contains(cargaCTe))
                        return true;
                }
            }
            return false;
        }

        private string RetornaListaCTesNaoVinculadosAoMDFe(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, List<int> codigosTerminalsOrigem, List<int> codigosTerminalsDestino, int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna1 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna2 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna3 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna4 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPerna5 = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordoPortoDestino = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            cargaCTes = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviario(true, codigoPedidoViagemNavio, cnpjPortoOrigem, cnpjPortoDestino);
            cargaCTesTransbordoPerna1 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(1, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna2 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(2, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna3 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(3, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna4 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(4, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            cargaCTesTransbordoPerna5 = repCargaCTe.ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(5, true, codigoPedidoViagemNavio, codigosTerminalsOrigem, cnpjPortoOrigem, codigosTerminalsDestino, cnpjPortoDestino);
            if (cnpjPortoDestino > 0)
                cargaCTesTransbordoPortoDestino = repCargaCTe.ConsultarCTesParaEmissaoMDFeAquaviarioUltimaPerna(true, codigoPedidoViagemNavio, cnpjPortoDestino, cnpjPortoOrigem);

            cargaCTes.AddRange(cargaCTesTransbordoPerna1);
            cargaCTes.AddRange(cargaCTesTransbordoPerna2);
            cargaCTes.AddRange(cargaCTesTransbordoPerna3);
            cargaCTes.AddRange(cargaCTesTransbordoPerna4);
            cargaCTes.AddRange(cargaCTesTransbordoPerna5);
            cargaCTes.AddRange(cargaCTesTransbordoPortoDestino);

            cargaCTes = cargaCTes.Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesAdicionar = ValidarConhecimentosPendentesEmissao(cargaCTes, cnpjPortoOrigem, cnpjPortoDestino, unitOfWork);

            List<string> entidades = new List<string>();
            if (cargaCTesAdicionar != null && cargaCTesAdicionar.Count > 0)
            {
                foreach (var cargaCTe in cargaCTesAdicionar)
                {
                    if (!cargaMDFeManual.CTes.Contains(cargaCTe))
                    {
                        string entidade = "Nº Controle (" + cargaCTe.CTe.NumeroControle + ") BK (" + cargaCTe.CTe.NumeroBooking + ") está autorizado e não vinculado a este MDF-e. <br/>";
                        if (!entidades.Contains(entidade))
                            entidades.Add(entidade);
                    }
                }
            }
            if (entidades.Count > 0)
                return string.Join(" ", entidades);
            else
                return "";

        }

        #endregion
    }
}
