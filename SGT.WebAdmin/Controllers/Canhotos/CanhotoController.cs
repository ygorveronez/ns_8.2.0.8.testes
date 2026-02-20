using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositorio;
using Repositorio.Embarcador.Filiais;
using Servicos.Embarcador.Canhotos;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Drawing;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoCanhoto", "ConsultarPorLocalArmazenamento", "BuscarDetalhesCanhoto",
        "DownloadCanhoto", "DownloadCanhotoAvulso", "BuscarCanhotosAvulsosPorCarga", "ObterMiniaturas",
         "Consultar",  "ConsultarConhecimentoEletronico", "DownloadCanhoto","BuscarDetalhesDoCanhotoParaAuditoria", },
        "Canhotos/Canhoto", "Cargas/EncerramentoCarga", "Cargas/ControleEntrega", "Canhotos/BaixarCanhoto", "Pedidos/AcompanhamentoPedido", "Canhotos/BuscarConfiguracoesGeraisCanhoto")]
    public class CanhotoController : BaseController
    {
        #region Construtores

        public CanhotoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaParaVinculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Numero, "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Tipo, "DescricaoTipoCanhoto", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataEmissao, "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.NumeroDaCarga, "NumeroCarga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Transportador, "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.TipoDeCarga, "TipoCarga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoSituacao", 13, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhotoParaVinculo filtrosPesquisa = ObterFiltrosPesquisaParaVinculo();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarParaVinculo);
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                int totalRegistros = repositorioCanhoto.ContarConsultaParaVinculo(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = (totalRegistros > 0) ? repositorioCanhoto.ConsultarParaVinculo(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

                var canhotosRetornar = (
                    from canhoto in canhotos
                    select new
                    {
                        canhoto.Codigo,
                        Descricao = canhoto.Numero,
                        canhoto.Numero,
                        canhoto.DescricaoTipoCanhoto,
                        DataEmissao = canhoto.DataEmissao.ToString("dd/MM/yyyy"),
                        NumeroCarga = canhoto.Carga?.CodigoCargaEmbarcador ?? "",
                        Empresa = canhoto.Empresa?.RazaoSocial ?? Localization.Resources.Canhotos.Canhoto.NaoInformado,
                        TipoCarga = canhoto.Carga?.TipoDeCarga?.Descricao ?? "",
                        canhoto.DescricaoSituacao,
                    }
                ).ToList();

                grid.AdicionaRows(canhotosRetornar);
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
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Numero, "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Tipo, "DescricaoTipoCanhoto", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataEmissao, "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.NumeroDaCarga, "NumeroCarga", 10, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Transportador, "Empresa", 20, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("Empresa", false);

                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.TipoDeCarga, "TipoCarga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoSituacao", 13, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarParaVinculo);
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                int totalRegistros = 0;
                IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> lista = ExecutaPesquisa(ref totalRegistros, parametrosConsulta, unitOfWork);

                var canhotosRetornar = (
                    from canhoto in lista
                    select new
                    {
                        canhoto.Codigo,
                        Descricao = canhoto.Numero,
                        canhoto.Numero,
                        canhoto.DescricaoTipoCanhoto,
                        DataEmissao = canhoto.DataEmissao,
                        NumeroCarga = canhoto.NumeroCarga ?? "",
                        Empresa = canhoto.Empresa ?? Localization.Resources.Canhotos.Canhoto.NaoInformado,
                        TipoCarga = canhoto.TipoCarga ?? "",
                        canhoto.DescricaoSituacao,
                    }
                ).ToList();

                grid.AdicionaRows(canhotosRetornar);
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
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadCanhotoAvulso()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if ((TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) && (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe))
                    return new JsonpResult(false, false, Localization.Resources.Canhotos.Canhoto.NaoPossivelRealizarEstaOperacaoParaEsteTipoDeEmpresa);

                int codigoCanhoto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoCanhoto);

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);

                string mensagemErro = string.Empty;

                byte[] pdf = Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotoAvulso(canhoto.Codigo, unidadeTrabalho, out mensagemErro);

                if (pdf == null)
                    return new JsonpResult(true, false, mensagemErro);

                return Arquivo(pdf, "application/pdf", Localization.Resources.Canhotos.Canhoto.CanhotoAvulso + " " + canhoto.Numero.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoRealizarDownloadDoComprovanteDeEntrega);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCanhotosAvulsosPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.CanhotoAvulso repCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(unitOfWork);

                int carga = int.Parse(Request.Params("Carga"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Numero, "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataEmissao, "DataEmissao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Destinatario, "Destinatario", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Valor, "Valor", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Peso, "Peso", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Notas, "Notas", 30, Models.Grid.Align.left, true);


                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhotoAvulso.ConsultarPorCarga(carga, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCanhotoAvulso.ContarConsultaPorCarga(carga));
                var lista = (from p in canhotos
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 DataEmissao = p.DataEmissao.ToString("dd/MM/yyyy"),
                                 Destinatario = p.Destinatario.Nome,
                                 Valor = p.Valor.ToString("n2"),
                                 Peso = p.Peso.ToString("n2"),
                                 Notas = string.Join(", ", (from obj in p.CanhotoAvulso.PedidosXMLNotasFiscais select obj.XMLNotaFiscal.Numero + "-" + obj.XMLNotaFiscal.Serie).ToList()),
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoConsultarOsCanhotosDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);

                int CodigoCanhoto = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataHistorico, "DataHistorico", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Operador, "Operador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Observacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Observacao, "ObservacaoOperador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoSituacao", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Digitalizacao, "DescricaoDigitalizacao", 10, Models.Grid.Align.center, false);


                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> canhotosNotaFiscalHistoricos = repCanhotoHistorico.Consultar(CodigoCanhoto, "DataHistorico", "desc", grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCanhotoHistorico.ContarConsulta(CodigoCanhoto));
                var lista = (from p in canhotosNotaFiscalHistoricos
                             select new
                             {
                                 p.Codigo,
                                 DataHistorico = p.DataHistorico.ToString("dd/MM/yyyy HH:mm"),
                                 Operador = p.Usuario != null ? p.Usuario.Nome : "",
                                 Observacao = p.Observacao + (p.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente && p.LocalArmazenamentoCanhoto != null ? (" (" + p.LocalArmazenamentoCanhoto.Descricao + (p.PacoteArmazenado > 0 ? " (" + Localization.Resources.Canhotos.Canhoto.Pacote + " " + p.PacoteArmazenado.ToString() + "/" + Localization.Resources.Canhotos.Canhoto.Posicao + " " + p.PosicaoNoPacote.ToString() + ")" : string.Empty) + ")") : string.Empty),
                                 ObservacaoOperador = p.ObservacaoOperador,
                                 p.DescricaoSituacao,
                                 p.DescricaoDigitalizacao
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        public async Task<IActionResult> ConsultarPorLocalArmazenamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoLocalArmazenamento = 0;
                int.TryParse(Request.Params("Codigo"), out codigoLocalArmazenamento);

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarPorCodigo(codigoLocalArmazenamento);

                List<int> codigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
                codigosFiliais.AddRange(ObterListaCodigoFilialPermitidasOperadorCanhoto(unitOfWork));

                List<double> codigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("DataEnvioCanhoto", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("GuidNomeArquivo", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("SituacaoCanhoto", false);
                grid.AdicionarCabecalho("SituacaoDigitalizacaoCanhoto", false);

                if (localArmazenamentoCanhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.NumeroNFe, "Numero", 12, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Numero, "Numero", 12, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataEmissao, "DataEmissao", 15, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Emitente, "Emitente", 25, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Transportador, "Empresa", 25, Models.Grid.Align.left, true);

                if (localArmazenamentoCanhoto.DividirEmPacotesDe > 0)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Pacote, "PacoteArmazenado", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Posicao, "PosicaoNoPacote", 10, Models.Grid.Align.left, false);
                }

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena != "DataEnvioCanhoto")
                    propOrdena = "XMLNotaFiscal." + propOrdena;

                if (propOrdena == "DescricaoSituacao")
                    propOrdena = "SituacaoCanhoto";
                else if (propOrdena == "Emitente")
                    propOrdena += ".Nome";
                else if (propOrdena == "Empresa")
                    propOrdena += ".RazaoSocial";

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosNotaFiscal = repCanhoto.ConsultarLocalArmazenamento(codigoLocalArmazenamento, codigosFiliais, codigosRecebedores, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCanhoto.ContarLocalArmazenamento(codigoLocalArmazenamento, codigosFiliais, codigosRecebedores));

                var lista = (from canhoto in canhotosNotaFiscal
                             select new
                             {
                                 DataEnvioCanhoto = canhoto.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente ? canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy") : "",
                                 canhoto.Codigo,
                                 Numero = canhoto.Numero,
                                 canhoto.SituacaoCanhoto,
                                 canhoto.SituacaoDigitalizacaoCanhoto,
                                 Emitente = canhoto.Emitente != null ? canhoto.Emitente.Descricao : "",
                                 Empresa = canhoto.Empresa?.RazaoSocial ?? Localization.Resources.Canhotos.Canhoto.NaoInformado,
                                 DataEmissao = canhoto.DataEmissao.ToString("dd/MM/yyyy"),
                                 PacoteArmazenado = canhoto.PacoteArmazenado,
                                 PosicaoNoPacote = canhoto.PosicaoNoPacote,
                                 GuidNomeArquivo = canhoto.GuidNomeArquivo,
                                 Observacao = canhoto.Observacao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Canhoto/Consultar", "grid-canhoto");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                // Busca Dados
                int totalRegistros = 0;
                IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> lista = ExecutaPesquisa(ref totalRegistros, grid.ObterParametrosConsulta(), unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> ConfirmarCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro = ObterFiltrosPesquisa(unitOfWork);

                if (!filtro.SituacoesDigitalizacaoCanhoto.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao))
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.SoPossivelAprovarEmMassaCanhotosAguardandoAprovacao);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                // Busca Dados
                int totalRegistros = 0;
                int canhotosNaoAgAprovacao = 0;
                int erroProcessamento = 0;
                IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> lista = ExecutaPesquisa(ref totalRegistros, null, unitOfWork);

                foreach (Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos canhotoConsulta in lista)
                {
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(canhotoConsulta.Codigo);
                    if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                    {
                        try
                        {
                            unitOfWork.Start();

                            repCanhoto.SetarCanhotoDigitalizado(canhotoConsulta.Codigo);
                            serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.DigitalizacaoDoCanhotoAprovada, unitOfWork);

                            Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);

                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && canhoto.Carga != null)
                            {
                                Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorAceiteDoCanhoto;
                                int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteAceiteImagem(canhoto.Carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                                if (numero == 0)
                                    serCarga.SolicitarEncerramentoCarga(canhoto.Carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorAceiteDasImagensDosCanhotosDaCarga, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                            }

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.AceitouDeImagemDoCanhoto, unitOfWork);

                            unitOfWork.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            unitOfWork.Rollback();
                            erroProcessamento++;
                        }
                    }
                    else
                    {
                        canhotosNaoAgAprovacao++;
                    }
                }

                return new JsonpResult(new
                {
                    CanhotosNaoAgAprovacao = canhotosNaoAgAprovacao,
                    ErroProcessamento = erroProcessamento
                });
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

        public async Task<IActionResult> ConfirmarDigitalizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto srvCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                List<int> codigos = Request.GetListParam<int>("Codigos");
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCodigos(codigos);

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                    {
                        unitOfWork.Start();

                        canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado;
                        canhoto.DigitalizacaoIntegrada = false;
                        canhoto.DataDigitalizacao = DateTime.Now;
                        canhoto.DataAprovacaoDigitalizacao = DateTime.Now;
                        canhoto.UsuarioDigitalizacao = this.Usuario;
                        canhoto.SituacaoPgtoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Pendente;
                        canhoto.OrigemSituacaoDigitalizacaoCanhoto = OrigemSituacaoDigitalizacaoCanhoto.Manual;

                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                        Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, unitOfWork);
                        Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);

                        serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.ConfirmouDigitalizacaoDosCanhotos, unitOfWork);

                        repCanhoto.Atualizar(canhoto);

                        unitOfWork.CommitChanges();
                    }
                }
                return new JsonpResult(true);

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

        public async Task<IActionResult> ObterMiniaturas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");

                if (codigos.Count == 0)
                    return new JsonpResult(new { Imagens = new { } });

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto srvCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dictionary<int, int> ordem = new Dictionary<int, int>();

                for (int i = 0; i < codigos.Count; i++)
                    ordem.Add(codigos[i], i + 1);

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCodigos(codigos);
                // TODO: IList, não contem .Find
                List<Dominio.ObjetosDeValor.Embarcador.Canhoto.DataConfirmacaoCanhoto> dataConfirmacaoCanhotos = repCanhoto.ConsultaDataConfirmacaoCanhotos(codigos).ToList();

                canhotos.OrderBy(o => o.DataDigitalizacao < DateTime.Now);

                return new JsonpResult(new
                {
                    Imagens = (from canhoto in canhotos
                               select new
                               {
                                   Ordem = ordem[canhoto.Codigo],
                                   canhoto.Codigo,
                                   canhoto.Numero,
                                   Miniatura = !canhoto.IsPDF() ? srvCanhoto.ObterMiniatura(canhoto, unitOfWork) : null,
                                   ArquivoPDF = canhoto.IsPDF(),
                                   VisibilidadeRodape = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe),
                                   this.ConfiguracaoEmbarcador.ExigeAprovacaoDigitalizacaoCanhoto,
                                   Rejeitado = canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada,
                                   PendenteAprovacao = canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.AgAprovocao,
                                   Aprovado = canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado,
                                   AguardandoIntegracao = canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.AgIntegracao,
                                   ValidacaoEmbarcador = canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.ValidacaoEmbarcador,
                                   DataEntregaNotaCliente = canhoto.DataEntregaNotaCliente.HasValue ? canhoto.DataEntregaNotaCliente?.ToString("dd/MM/yyyy") : canhoto.DataEnvioCanhoto != DateTime.MinValue ? canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy") : string.Empty,
                                   DataDigitalizacao = canhoto.DataDigitalizacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   PermitirReverter = PermitirReverterCanhoto(canhoto.SituacaoDigitalizacaoCanhoto, canhoto?.DataDigitalizacao ?? DateTime.MinValue, unitOfWork),
                                   canhoto.GuidNomeArquivo,
                                   ImagemInteira = canhoto.Emitente?.DigitalizacaoCanhotoInteiro ?? false,
                                   DataConfirmacao = dataConfirmacaoCanhotos.Find(o => o.CodigoCanhoto == canhoto.Codigo)?.DataConfirmacaoFormatada,
                                   CodigoCargaEntrega = dataConfirmacaoCanhotos.Find(o => o.CodigoCanhoto == canhoto.Codigo)?.CodigoCargaEntrega,
                                   DT_RowColor = ObterCorLinhaCanhotoInteiro(canhoto.SituacaoDigitalizacaoCanhoto),
                                   PossuiIntegracaoComprovei = canhoto.PossuiIntegracaoComprovei,
                                   ValidacaoCanhotoComprovei = canhoto.ValidacaoCanhoto,
                                   ValidacaoNumeroComprovei = canhoto.ValidacaoNumero,
                                   ValidacaoEncontrouDataComprovei = canhoto.ValidacaoEncontrouData,
                                   ValidacaoAssinaturaComprovei = canhoto.ValidacaoAssinatura
                               }).ToList()
                });
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

        public async Task<IActionResult> BuscarDetalhesCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);

                string serie = string.Empty;

                if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe && !string.IsNullOrWhiteSpace(canhoto.XMLNotaFiscal?.Serie))
                    serie = "-" + canhoto.XMLNotaFiscal?.Serie;
                else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao && !string.IsNullOrWhiteSpace(canhoto.CTeSubcontratacao?.Serie))
                    serie = "-" + canhoto.CTeSubcontratacao.Serie;

                var dynCanhoto = new
                {
                    Chave = canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao ? canhoto.CTeSubcontratacao.ChaveAcesso : canhoto.XMLNotaFiscal != null ? canhoto.XMLNotaFiscal.Chave : "",
                    Numero = canhoto.Numero + serie,
                    TipoCanhoto = canhoto.TipoCanhoto,
                    DescricaoTipoCanhoto = canhoto.DescricaoTipoCanhoto,
                    DescricaoDigitalizacao = canhoto.DescricaoDigitalizacao,
                    Empresa = canhoto.Empresa != null ? canhoto.Empresa.RazaoSocial : Localization.Resources.Canhotos.Canhoto.NaoInformado,
                    Destinatario = canhoto.Destinatario.Nome,
                    NotasAvuso = canhoto.CanhotoAvulso != null ? string.Join(", ", (from obj in canhoto.CanhotoAvulso.PedidosXMLNotasFiscais select obj.XMLNotaFiscal.Numero + "-" + obj.XMLNotaFiscal.Serie).ToList()) : "",
                    DataEmissao = canhoto.DataEmissao.ToString("dd/MM/yyyy"),
                    DescricaoSituacao = canhoto.DescricaoSituacao,
                    canhoto.SituacaoCanhoto,
                    Valor = canhoto.Valor.ToString("n2"),
                    Peso = canhoto.Peso.ToString("n2"),
                    NaturezaOP = canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe ? canhoto.XMLNotaFiscal.NaturezaOP :
                                 canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao ? canhoto.CTeSubcontratacao.CFOP?.NaturezaDaOperacao?.Descricao : "",
                    canhoto.DescricaoModalidadeFrete,
                    Filial = canhoto.Filial != null ? canhoto.Filial.Descricao : "",
                    canhoto.SituacaoDigitalizacaoCanhoto,
                    Carga = canhoto.Carga != null ? canhoto.Carga.CodigoCargaEmbarcador : " - ",
                    Motoristas = retornarMotoristas(canhoto.MotoristasResponsaveis.ToList()),
                    Emitente = canhoto.Emitente != null ? canhoto.Emitente.Nome : "",
                    LocalArmazenamento = canhoto.LocalArmazenamentoCanhoto != null ? canhoto.LocalArmazenamentoCanhoto.Descricao : "",
                    canhoto.NomeArquivo,
                    canhoto.GuidNomeArquivo,
                    canhoto.Observacao,
                    canhoto.PacoteArmazenado,
                    canhoto.PosicaoNoPacote,
                    ChaveNota = "",
                    Justificativa = canhoto.Observacao,
                    canhoto.Latitude,
                    canhoto.Longitude,
                    MotivoRejeicaoDigitalizacao = canhoto.UltimaInconsistencia?.MotivoInconsistenciaDigitacao.Descricao ?? "",
                    canhoto.SituacaoPgtoCanhoto,
                    canhoto.ObservacaoRecebimentoFisico,
                    DataRecebimento = canhoto.DataRecebimento.HasValue ? canhoto.DataRecebimento.Value.ToString("dd/MM/yyyy") : "",
                    canhoto.NumeroProtocolo,
                };

                return new JsonpResult(dynCanhoto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
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
                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);

                var dynCanhoto = new
                {
                    canhoto.Codigo,
                    DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm"),
                    canhoto.NomeArquivo,
                    canhoto.GuidNomeArquivo,
                    canhoto.Observacao,
                    ChaveNota = "",
                    LocalArmazenamentoCanhoto = canhoto.LocalArmazenamentoCanhoto != null ? new { canhoto.LocalArmazenamentoCanhoto.Codigo, canhoto.LocalArmazenamentoCanhoto.Descricao } : null,
                    canhoto.PosicaoNoPacote,
                    canhoto.PacoteArmazenado,
                    canhoto.SituacaoDigitalizacaoCanhoto
                };

                return new JsonpResult(dynCanhoto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarImagemCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                if (files.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoFoiEncontradoUmArquivoParaArmazenamentoDoCanhoto);

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                DateTime? dataEnvioCanhoto = Request.GetNullableDateTimeParam("DataEnvioCanhoto");
                DateTime? dataEntregaNotaCliente = Request.GetNullableDateTimeParam("DataEntregaNotaCliente");
                bool? enviouImagemViaEdicaoImagem = Request.GetBoolParam("EnviouImagemViaEdicao");

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCargaCanhoto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioConfiguracaoCanhoto = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

                if (canhoto == null)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.CanhotoNaoFoiSelecionado);

                if ((configuracaoEmbarcador?.ObrigatorioInformarDataEnvioCanhoto ?? false) && !dataEnvioCanhoto.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataDeEnvioDaImagemDoCanhotoObrigatoria);

                if ((configuracaoEmbarcador?.ExigirDataEntregaNotaClienteCanhotos ?? false) && !dataEntregaNotaCliente.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataDeEntregaDaNotaAoClienteObrigatoria);

                if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado && !configuracaoCanhoto.PermitirAlterarImagemCanhotoDigitalizada)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.ImagemJaFoiDigitalizadaAnteriormente);

                if (configuracaoCanhoto.ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente && !canhoto.Carga.DataFimViagem.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPossivelInserirImagemDoCanhotoPorqueEntregaNaoEstaConfirmada);

                if (configuracaoCanhoto.TamanhoMaximoDaImagemDoCanhoto > 0)
                {
                    int tamanhoEmBytes = configuracaoCanhoto.TamanhoMaximoDaImagemDoCanhoto * 1000;
                    if (files[0].Length > tamanhoEmBytes)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.TamanhoDaImagemNaoDeveExcederKB, configuracaoCanhoto.TamanhoMaximoDaImagemDoCanhoto));
                }

                unitOfWork.Start();

                //Servicos.Log.TratarErro($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} Iniciou EnviarImagemCanhoto canhoto (código {canhoto.Codigo}).");

                string guidNomeArquivo = "", nomeArquivo = "";
                byte[] arquivo = null;
                if (files.Count > 1)
                    SalvarImagensCanhoto(canhoto, ref arquivo, ref guidNomeArquivo, ref nomeArquivo, unitOfWork);
                else
                    SalvarImagemCanhoto(canhoto, ref arquivo, ref guidNomeArquivo, ref nomeArquivo, unitOfWork);

                if (canhoto.XMLNotaFiscal != null)
                    servicoChamado.FinalizarAtendimentosEmAberto(new List<int> { canhoto.XMLNotaFiscal.Numero }, cargaEntrega: null, Auditado, "Canhoto Enviado", unitOfWork);

                ProcessarCanhotoComImagem(canhoto, guidNomeArquivo, nomeArquivo, dataEnvioCanhoto, dataEntregaNotaCliente, configuracaoEmbarcador, unitOfWork, arquivo);


                //Servicos.Log.TratarErro($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} Finalizou EnviarImagemCanhoto canhoto (código {canhoto.Codigo}).");

                if (enviouImagemViaEdicaoImagem ?? false)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, "Editou Imagem do canhoto", unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !ValidarDocumentoNaPasta(canhoto, unitOfWork))
                {
                    return new JsonpResult(false, true, "Canhoto enviado porem ocorreu erro ao salvar, necessario renviar canhoto");
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarImagemParaCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> BuscarSeExisteChamadoAbertoParaOCanhoto(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCanhoto = Request.GetIntParam("codigoCanhoto");
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork, cancellationToken);
                string canhoto = await repositorioCanhoto.BuscarSeExisteChamadoAbertoParaOCanhotoAsync(codigoCanhoto);

                return new JsonpResult(canhoto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> EnviarByteImagemParaMultiplosCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoFoiEncontradoUmArquivoParaArmazenamentoDoCanhoto);

                string tokenImagem = SalvarImagemCanhotoTemporario(unitOfWork);

                return new JsonpResult(tokenImagem);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarByteDaImagemParaMultiplosCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarImagemParaMultiplosCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            string tokenImagem = Request.GetStringParam("TokenImagem");
            if (string.IsNullOrWhiteSpace(tokenImagem))
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.ImagemNaoFoiEnviadaAnteriormente);

            try
            {
                int quantidadeCanhotosPermitida = 300;
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionados(unitOfWork, quantidadeCanhotosPermitida, out bool quantidadeValida, out int quantidadeSelecionada);

                if (canhotos == null)
                {
                    if (!quantidadeValida)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));
                }

                canhotos = canhotos.Where(o => o.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado).ToList();

                if (canhotos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NenhumCanhotoFoiSelecionado);

                if (!quantidadeValida)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                DateTime? dataEnvioCanhoto = Request.GetNullableDateTimeParam("DataEnvioCanhoto");
                DateTime? dataEntregaNotaCliente = Request.GetNullableDateTimeParam("DataEntregaNotaCliente");

                if ((configuracaoEmbarcador?.ObrigatorioInformarDataEnvioCanhoto ?? false) && !dataEnvioCanhoto.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataDeEnvioDaImagemDoCanhotoObrigatoria);

                if ((configuracaoEmbarcador?.ExigirDataEntregaNotaClienteCanhotos ?? false) && !dataEntregaNotaCliente.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataDeEntregaDaNotaAoClienteObrigatoria);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    string nomeArquivo = "";
                    CopiarImagemTemporariaProCanhoto(canhoto, tokenImagem, ref nomeArquivo, unitOfWork);

                    ProcessarCanhotoComImagem(canhoto, tokenImagem, nomeArquivo, dataEnvioCanhoto, dataEntregaNotaCliente, configuracaoEmbarcador, unitOfWork);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (!ValidarDocumentoNaPasta(canhoto, unitOfWork))
                        {
                            return new JsonpResult(false, true, $@"Canhoto {canhoto.Numero} enviado porem ocorreu erro ao salvar, necessario renviar canhoto novamente");
                        }
                        unitOfWork.CommitChanges();
                    }
                }

                ExcluirImagemCanhotoTemporario(tokenImagem, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                ExcluirImagemCanhotoTemporario(tokenImagem, unitOfWork);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                ExcluirImagemCanhotoTemporario(tokenImagem, unitOfWork);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarImagemParaMultiplosCanhotos);
            }
            finally
            {
                ExcluirImagemCanhotoTemporario(tokenImagem, unitOfWork);
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DescartarCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfigControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorCodigo(codigo);

                if (canhoto == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                int codigoMotivoInconsistenciaDigitacao = Request.GetIntParam("Motivo");
                string observacoes = Request.GetStringParam("Observacoes");
                Repositorio.Embarcador.Canhotos.MotivoInconsistenciaDigitacao repositorioMotivoInconsistenciaDigitacao = new Repositorio.Embarcador.Canhotos.MotivoInconsistenciaDigitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao motivoInconsistenciaDigitacao = repositorioMotivoInconsistenciaDigitacao.BuscarPorCodigo(codigoMotivoInconsistenciaDigitacao);

                if (motivoInconsistenciaDigitacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelEncontrarMotivoDaInconsistenciaDaDigitacao);

                if (!serCanhoto.ValidarSituacaoIntegracaoCanhoto(canhoto, unitOfWork))
                    return new JsonpResult(false, false, Localization.Resources.Canhotos.Canhoto.CanhotoComRegistroIntegracaoConfirmacao);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfigControleEntrega.BuscarPrimeiroRegistro();

                unitOfWork.Start();

                canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada;
                canhoto.DataEntregaNotaCliente = null;

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repNotaFiscal.BuscarPorCodigo(canhoto.XMLNotaFiscal?.Codigo ?? 0);
                if (configuracaoControleEntrega.RejeitarEntregaNotaFiscalAoRejeitarCanhoto && notaFiscal != null)
                {
                    notaFiscal.SituacaoEntregaNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Devolvida;
                    repNotaFiscal.Atualizar(notaFiscal);

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorNotaFiscalUltimaEntrega(notaFiscal.Codigo);
                    if (cargaEntrega != null)
                    {
                        cargaEntrega.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado;
                        cargaEntrega.DataConfirmacao = null;
                        repCargaEntrega.Atualizar(cargaEntrega, Auditado, null, Localization.Resources.Canhotos.Canhoto.RejeitouDigitalizacaoDoCanhoto + $" {observacoes}");
                    }
                }

                serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.RejeitouDigitalizacaoDoCanhoto + $" {observacoes}", unitOfWork);
                Repositorio.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto repositorioInconsistenciaDigitacaoCanhoto = new Repositorio.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto inconsistenciaDigitacaoCanhoto = new Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto()
                {
                    Canhoto = canhoto,
                    Data = DateTime.Now,
                    MotivoInconsistenciaDigitacao = motivoInconsistenciaDigitacao,
                    Numero = repositorioInconsistenciaDigitacaoCanhoto.BuscarProximoNumero(canhoto.Codigo),
                    Usuario = this.Usuario,
                    Observacoes = observacoes
                };

                repositorioInconsistenciaDigitacaoCanhoto.Inserir(inconsistenciaDigitacaoCanhoto);

                unitOfWork.CommitChanges();

                EnviarEmailInconsistenciaDigitacaoCanhotoResponsavelPrestacaoContasTransportadora(unitOfWork, inconsistenciaDigitacaoCanhoto);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaaoRejeitarImagem);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro($"ValidarCanhoto iniciado - {DateTime.Now}", "AprovacaoCanhotoHistoricoTempo");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                // Parametros
                int codigo = Request.GetIntParam("Codigo");
                DateTime? dataEntrega = Request.GetNullableDateTimeParam("DataEntrega");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarCanhotoXMLNotaFiscal(codigo);

                // Valida
                if (canhoto == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if ((configuracaoEmbarcador?.ExigirDataEntregaNotaClienteCanhotos ?? false) && !dataEntrega.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataDeEntregaDaNotaAoClienteObrigatoria);

                if (dataEntrega.HasValue && dataEntrega.Value > DateTime.Now)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataDeEntregaNaoPodeSerMaiorQueDataAtual);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                    xmlNotaFiscal = canhoto.XMLNotaFiscal;
                else if (canhoto.TipoCanhoto == TipoCanhoto.Avulso && canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Count > 0)
                    xmlNotaFiscal = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.FirstOrDefault().XMLNotaFiscal;

                DateTime? dataEmissaoNf = xmlNotaFiscal != null ? xmlNotaFiscal.DataEmissao : null;

                if (dataEmissaoNf.HasValue && dataEntrega.HasValue && dataEmissaoNf.Value.Date > dataEntrega.Value)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataDeEntregaNaoPodeSerInferiorDataDeEmissaoDaNotaFiscal);

                if (!serCanhoto.ValidarSituacaoIntegracaoCanhoto(canhoto, unitOfWork))
                    return new JsonpResult(false, false, Localization.Resources.Canhotos.Canhoto.CanhotoComRegistroIntegracaoConfirmacao);

                // Persiste dados
                unitOfWork.Start();

                if (canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPermitidoAprovarCanhotoNaAtualSituacao);

                canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado;
                canhoto.DigitalizacaoIntegrada = false;
                canhoto.DataAprovacaoDigitalizacao = DateTime.Now;
                canhoto.UsuarioDigitalizacao = this.Usuario;
                canhoto.SituacaoPgtoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Pendente;

                if (dataEntrega.HasValue)
                    canhoto.DataEntregaNotaCliente = dataEntrega;

                serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.ConfirmouDigitalizacaoDoCanhoto, unitOfWork);

                repCanhoto.Atualizar(canhoto);

                Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                Servicos.Log.TratarErro($"ValidarCanhoto finalizado - {DateTime.Now}", "AprovacaoCanhotoHistoricoTempo");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReverterCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigo);

                // Valida
                if (canhoto == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelReverterCanhotoQueNaoEstaDigitalizado);

                if (canhoto.SituacaoPgtoCanhoto == SituacaoPgtoCanhoto.Liberado)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPossivelReverterCanhotoComPagamentoLiberado);

                if (!serCanhoto.ValidarSituacaoIntegracaoCanhoto(canhoto, unitOfWork))
                    return new JsonpResult(false, false, Localization.Resources.Canhotos.Canhoto.CanhotoComRegistroIntegracaoConfirmacao);

                // Persiste dados
                unitOfWork.Start();
                canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao;
                canhoto.DataReversao = DateTime.Now;
                serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.ReverteuDigitalizacaoDoCanhoto, unitOfWork);
                repCanhoto.Atualizar(canhoto);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
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

        public async Task<IActionResult> RejeitarEnvio()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCargaCanhoto.BuscarPorCodigos(codigos);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {

                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                    {
                        unitOfWork.Start();

                        if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao ||
                            canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.ValidacaoEmbarcador)
                        {
                            canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada;
                            canhoto.MotivoRejeicaoDigitalizacao = Request.Params("MotivoRejeicaoDigitalizacao");
                            canhoto.DataUltimaModificacao = DateTime.Now;
                            repCargaCanhoto.Atualizar(canhoto);

                            Repositorio.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto repositorioInconsistenciaDigitacaoCanhoto = new Repositorio.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto(unitOfWork);

                            int codigoMotivoInconsistenciaDigitacao = Request.GetIntParam("Motivo");
                            Repositorio.Embarcador.Canhotos.MotivoInconsistenciaDigitacao repositorioMotivoInconsistenciaDigitacao = new Repositorio.Embarcador.Canhotos.MotivoInconsistenciaDigitacao(unitOfWork);
                            Dominio.Entidades.Embarcador.Canhotos.MotivoInconsistenciaDigitacao motivoInconsistenciaDigitacao = repositorioMotivoInconsistenciaDigitacao.BuscarPorCodigo(codigoMotivoInconsistenciaDigitacao);

                            string observacoes = Request.GetStringParam("Observacoes");

                            if (motivoInconsistenciaDigitacao != null)
                            {
                                Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto inconsistenciaDigitacaoCanhoto = new Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto()
                                {
                                    Canhoto = canhoto,
                                    Data = DateTime.Now,
                                    MotivoInconsistenciaDigitacao = motivoInconsistenciaDigitacao,
                                    Numero = repositorioInconsistenciaDigitacaoCanhoto.BuscarProximoNumero(canhoto.Codigo),
                                    Usuario = this.Usuario,
                                    Observacoes = observacoes
                                };

                                repositorioInconsistenciaDigitacaoCanhoto.Inserir(inconsistenciaDigitacaoCanhoto);
                            }

                            serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.DigitalizacaoDoCanhotoRejeitado + $" {observacoes}", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.RejeitouEnvioDeImagemDoCanhoto, unitOfWork);
                        }
                        else
                        {
                            throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoPermitidoInformarRejeitarSituacaoDaDigitalizacaoSuaAtualSituacao + " (" + canhoto.DescricaoSituacao + ").");
                        }

                        unitOfWork.CommitChanges();
                    }
                    return new JsonpResult(true);
                }
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);
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
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoRejeitarRecebimentoDaDigitalizacaoDoCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AceitarEnvio()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);

                AceitarEnvio(canhoto, unitOfWork, out string mensagemValidacao);

                if (string.IsNullOrEmpty(mensagemValidacao))
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, mensagemValidacao);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoConfirmarAceiteDaDigitalizacaoDoCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarTodosStatus(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto situacao;
                Enum.TryParse(Request.Params("Status"), out situacao);

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork, cancellationToken);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                int quantidadeCanhotosPermitida = 300;

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionados(unitOfWork, quantidadeCanhotosPermitida, out bool quantidadeValida, out int quantidadeSelecionada);

                if (!quantidadeValida)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaEntregaRealizada = repTipoDeOcorrenciaDeCTe.BuscarPorEntregaRealizada();

                dynamic dynretorno = null;

                List<int> codigosCTesFinalizarEnvioCanhotos = new List<int>();

                //Servicos.Log.TratarErro($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} Iniciou alteração status canhotos (count {canhotos.Count}).");

                Dominio.ObjetosDeValor.Embarcador.Canhoto.ObservacaoAlteracaoCanhoto observacaoAlteracaoCanhoto = ObterDadosObservacaoAlteracaoCanhoto();

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    canhoto.DataAlteracao = observacaoAlteracaoCanhoto.Data;
                    canhoto.CodigoRastreio = observacaoAlteracaoCanhoto.CodigoRastreio;

                    if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente && canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
                    {
                        canhoto.Usuario = Usuario;
                        canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EntregueMotorista;

                        serCanhoto.GerarHistoricoCanhoto(canhoto, Usuario, Localization.Resources.Canhotos.Canhoto.CanhotoEntreguePeloMotorista, unitOfWork, observacaoAlteracaoCanhoto.ObservacaoOperador);

                        string msgRetorno = string.Empty;
                        if (!serCanhoto.GerarOcorrenciaEntregaCanhoto(tipoDeOcorrenciaEntregaRealizada, canhoto, this.Usuario, string.Format(Localization.Resources.Canhotos.Canhoto.GeradoPartirDaEntregaPeloMotoristaDoCanhotoNumero, canhoto.Numero), unitOfWork, Auditado, ref msgRetorno, TipoServicoMultisoftware, this.Cliente, this.ConfiguracaoEmbarcador))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, msgRetorno);
                        }

                        codigosCTesFinalizarEnvioCanhotos.AddRange(ObterCodigosCTesFinalizarEnvioCanhoto(canhoto, unitOfWork));
                    }
                    else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EntregueMotorista && canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EntregueMotorista)
                    {
                        dynretorno = ConfirmarEnvioFisicoCanhoto(canhoto, unitOfWork, Usuario, observacaoAlteracaoCanhoto.ObservacaoOperador);
                        if (dynretorno.Pendencia)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, dynretorno.Mensagem);
                        }

                        codigosCTesFinalizarEnvioCanhotos.AddRange(ObterCodigosCTesFinalizarEnvioCanhoto(canhoto, unitOfWork));

                        if (canhoto.Carga != null)
                        {
                            if (!cargas.Contains(canhoto.Carga))
                                cargas.Add(canhoto.Carga);
                        }
                    }
                    else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente && canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente)
                    {
                        canhoto.Usuario = this.Usuario;
                        canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EnviadoCliente;

                        serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.CanhotoEnviadoAoCliente, unitOfWork, observacaoAlteracaoCanhoto.ObservacaoOperador);
                    }
                    else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EnviadoCliente && canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EnviadoCliente)
                    {
                        canhoto.Usuario = this.Usuario;
                        canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoCliente;

                        serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.CanhotoRecebidoPeloCliente, unitOfWork, observacaoAlteracaoCanhoto.ObservacaoOperador);
                    }

                    if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.EntregueMotorista)
                        repCanhoto.Atualizar(canhoto);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.AlterouMultiplosStatus, unitOfWork);
                }

                //Servicos.Log.TratarErro($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} FinalizarEnvioCanhotosCTe alteração status canhotos (count {canhotos.Count}).");

                FinalizarEnvioCanhotosCTe(codigosCTesFinalizarEnvioCanhotos, unitOfWork);

                //Servicos.Log.TratarErro($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} SolicitarEncerramentoCarga alteração status canhotos (count {canhotos.Count}).");

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorConfirmacaoDeEntregaDoCanhoto;
                    int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteEnvioCarga(carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                    if (numero == 0)
                        serCarga.SolicitarEncerramentoCarga(carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorConfirmacaoDeEntregaDosCanhotos, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                }

                unitOfWork.CommitChanges();

                //Servicos.Log.TratarErro($"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} Finalizou alteração status canhotos (count {canhotos.Count}).");

                return new JsonpResult(dynretorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarMultiplosCanhotosFisico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico))
                    return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaConfirmarRecebimentoCanhoto);


                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                    int quantidadeCanhotosPermitida = 300;

                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionados(unitOfWork, quantidadeCanhotosPermitida, out bool quantidadeValida, out int quantidadeSelecionada);

                    if (!quantidadeValida)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));

                    string retorno = VerificarSeNaoEsgotouCapacidadeLocalArmazenamentoMultiplaSelecao(canhotos, unitOfWork);

                    if (string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Start();

                        dynamic dynretorno = null;
                        foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                        {
                            if (canhoto.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente)
                            {
                                dynretorno = ConfirmarEnvioFisicoCanhoto(canhoto, unitOfWork, Usuario);
                                if (dynretorno.Pendencia)
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, dynretorno.Mensagem);
                                }

                                FinalizarEnvioCanhotosCTe(canhoto, unitOfWork);

                                if (canhoto.Carga != null)
                                {
                                    if (!cargas.Contains(canhoto.Carga))
                                        cargas.Add(canhoto.Carga);
                                }

                                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.EnviouMultiplosCanhotosFisicos, unitOfWork);
                            }
                        }

                        //sumariza as cargas para solicitar a liberação do pagamento para terceiros e solicitar o encerramento se necessário.
                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        {
                            Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorEnvioDeMultiplosCanhotosFisicos;
                            int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteEnvioCarga(carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                            if (numero == 0)
                                serCarga.SolicitarEncerramentoCarga(carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorConfirmacaoDeEntregaDosCanhotos, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                        }

                        unitOfWork.CommitChanges();
                        return new JsonpResult(dynretorno);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, retorno);
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarCanhotoExtraviado()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
                    {
                        serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.CanhotoExtraviado, unitOfWork);

                        canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Extraviado;

                        repCargaCanhoto.Atualizar(canhoto);

                        FinalizarEnvioCanhotosCTe(canhoto, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.InformouExtravioDoCanhoto, unitOfWork);

                        Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, TipoServicoMultisoftware, this.Cliente, unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPermitidoInformarQueCanhotoFoiExtraviadoEmSuaAtualSituacao + " (" + canhoto.DescricaoSituacao + ").");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);
                }


            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarCanhotoNaoEntregue(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repConfiguracao.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = await repCargaCanhoto.BuscarPorCodigoAsync(codigo);

                await unitOfWork.StartAsync(cancellationToken);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    throw new ControllerException(Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                if (canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao)
                    throw new ControllerException(string.Format(Localization.Resources.Canhotos.Canhoto.NaoPermitidoInformarQueCanhotoNaoFoiEntregueEmSuaAtualSituacao, canhoto.DescricaoDigitalizacao));

                canhoto.XMLNotaFiscal.SituacaoEntregaNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.NaoEntregue;
                await repCargaCanhoto.AtualizarAsync(canhoto);

                Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, TipoServicoMultisoftware, this.Cliente, unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, canhoto, Localization.Resources.Canhotos.Canhoto.InformouCanhotoNaoEntregue, unitOfWork, cancellationToken);
                await serCanhoto.GerarHistoricoCanhotoAsync(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.CanhotoNaoEntregue);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(true);

            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, true, excecao.Message);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> EnviarCanhotoFisico()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhoto_PermiteConfirmarRecebimentoCanhotoFisico))
                    return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaConfirmarRecebimentoCanhoto);

                int codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);
                Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioConfiguracaoCanhoto = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    if (canhoto.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente)
                    {
                        unitOfWork.Start();

                        canhoto.ObservacaoRecebimentoFisico = Request.GetStringParam("ObservacaoRecebimentoFisico");
                        canhoto.DataRecebimento = Request.GetNullableDateTimeParam("DataRecebimento");
                        canhoto.NumeroProtocolo = Request.GetIntParam("NumeroProtocolo");

                        DateTime? dataEntregaNotaCliente = Request.GetNullableDateTimeParam("DataEntregaCliente");
                        if (dataEntregaNotaCliente.HasValue)
                            canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente.Value;

                        if (configuracaoCanhoto.NaoPermitirReceberCanhotosNaoDigitalizados && canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                            return new JsonpResult(false, true, "Imagem do canhoto precisa estar digitalizada ");

                        dynamic retorno = ConfirmarEnvioFisicoCanhoto(canhoto, unitOfWork, Usuario);
                        if (retorno.Pendencia)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, retorno.Mensagem);
                        }

                        FinalizarEnvioCanhotosCTe(canhoto, unitOfWork);

                        if (canhoto.Carga != null)
                        {
                            Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoProEnvioDeCanhotoFisico;

                            int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteEnvioCarga(canhoto.Carga, TipoServicoMultisoftware, unitOfWork, Auditado);

                            if (numero == 0)
                                serCarga.SolicitarEncerramentoCarga(canhoto.Carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorConfirmacaoDaEntregaDosCanhotos, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.ConfirmouEnvioFisicoDoCanhoto, unitOfWork);

                        unitOfWork.CommitChanges();

                        return new JsonpResult(retorno);
                    }
                    else
                        return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPermitidoInformarRecebimentoDoCanhotoEmSuaAtualSituacao + " (" + canhoto.DescricaoSituacao + ").");
                }
                else
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarConhecimentoEletronico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chave = Utilidades.String.OnlyNumbers(Request.Params("ConhecimentoEletronico"));

                if (!Utilidades.Validate.ValidarChave(chave))
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.ChaveInformadaInvalida);

                string modelo = chave.Substring(20, 2);

                Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa retorno = new Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa();

                if (modelo == "57")
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(chave);

                    if (cte == null)
                        return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.CTeNaoEncontradoComChaveInformada);

                    retorno = new Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa()
                    {
                        Codigo = cte.Codigo,
                        Chave = chave,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.CTe
                    };
                }
                else if (modelo == "55")
                {
                    Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorChave(chave);

                    if (canhoto == null)
                        return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NFeNaoEncontradaComChaveInformada);

                    retorno = new Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa()
                    {
                        Codigo = canhoto.Codigo,
                        Chave = chave,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.NFe
                    };
                }
                else
                {
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.ChaveInformadaInvalida);
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoConsultarChave);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BaixarCanhotoFisico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                string dadosBaixa = (Request.Params("DadosBaixa") ?? string.Empty).Replace(" ", "");

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                string[] splitDados = dadosBaixa.Split('_');

                if (splitDados.Length > 1 && splitDados[0] == "AV") //QR Code do canhoto avulso
                {
                    canhoto = repCargaCanhoto.BuscarPorQRCodeAvulso(dadosBaixa);
                }
                else
                {
                    string chaveDocumento = Utilidades.String.OnlyNumbers(dadosBaixa);

                    bool chave = chaveDocumento.Length == 44;

                    if ((chave && !Utilidades.Validate.ValidarChave(chaveDocumento)) || (!chave && chaveDocumento.Length != 20))
                        return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.ChaveDoDocumentoInvalido);

                    if (chave)
                    {
                        string modelo = chaveDocumento.Substring(20, 2);

                        if (modelo == "57")
                        {
                            canhoto = repCargaCanhoto.BuscarPorChaveCTeSubcontratacao(dadosBaixa);

                            if (canhoto == null)
                                canhoto = repCargaCanhoto.BuscarPorChaveCTe(dadosBaixa);
                        }
                        else if (modelo == "55")
                        {
                            canhoto = repCargaCanhoto.BuscarPorChave(dadosBaixa);
                        }
                    }
                    else
                    {
                        int.TryParse(chaveDocumento.Substring(0, 10), out int codigoCarga);
                        int.TryParse(chaveDocumento.Substring(10, 10), out int codigoCTe);

                        canhoto = repCargaCanhoto.BuscarPorCTeCargaCTe(codigoCTe);
                    }
                }

                if (canhoto == null)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.CanhotoInformadoNaoFoiLocalizado);

                if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.EsteCanhotoJaFoiBaixado);

                unitOfWork.Start();

                dynamic retorno = ConfirmarEnvioFisicoCanhoto(canhoto, unitOfWork, Usuario);
                if (retorno.Pendencia)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, retorno.Mensagem);
                }

                FinalizarEnvioCanhotosCTe(canhoto, unitOfWork);

                if (canhoto.Carga != null)
                {
                    Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorBaixaDeCanhotoFisico;
                    int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteEnvioCarga(canhoto.Carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                    if (numero == 0)
                        serCarga.SolicitarEncerramentoCarga(canhoto.Carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorConfirmacaoDeEntregaDosCanhotos, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.BaixouCanhoto, unitOfWork);

                unitOfWork.CommitChanges();



                return new JsonpResult(retorno);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarMultiplasJustificativas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhoto_PermiteAdicionarJustificativa))
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaAdicionarJustificativa);

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                    int quantidadeCanhotosPermitida = 300;

                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionados(unitOfWork, quantidadeCanhotosPermitida, out bool quantidadeValida, out int quantidadeSelecionada);

                    if (!quantidadeValida)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));

                    unitOfWork.Start();

                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                    {
                        if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
                        {
                            EnviarJustificarivaCanhoto(canhoto, unitOfWork);
                            FinalizarEnvioCanhotosCTe(canhoto, unitOfWork);
                            if (canhoto.Carga != null)
                            {
                                if (!cargas.Contains(canhoto.Carga))
                                    cargas.Add(canhoto.Carga);
                            }
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.EnviouMultiplasJustificativas, unitOfWork);
                        }
                    }

                    //sumariza as cargas para solicitar a liberação do pagamento para terceiros e solicitar o encerramento se necessário.
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorEnvioDeMultiplasJustificativasDeCanhotos;
                        int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteEnvioCarga(carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                        if (numero == 0)
                            serCarga.SolicitarEncerramentoCarga(carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorConfirmacaoDeEntregaDosCanhotos, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                    }

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReverterJustificativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pallets/Devolucao");

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhotos_ReverterJustificativa_Canhotos))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);
                }

                if (canhoto.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPermitidoJustificarCanhotoEmSuaAtualSituacao + " (" + canhoto.DescricaoSituacao + ").");
                }

                if (canhoto.Carga != null && (canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada || canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPermitidoJustificarCanhotoQuandoCargaJaFoiEncerrada);
                }

                string observacao = Request.Params("Observacao");

                canhoto.DataReverterJustificativa = DateTime.Now;
                canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;
                canhoto.ObservacaoReverterJustificativa = observacao;
                canhoto.DataUltimaModificacao = DateTime.Now;

                unitOfWork.Start();

                repCargaCanhoto.Atualizar(canhoto);
                serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.JustificativaRevertida, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, canhoto.GetChanges(), Localization.Resources.Canhotos.Canhoto.ReverteuJustificativa, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
        }

        public async Task<IActionResult> Justificar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhoto_PermiteAdicionarJustificativa))
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.UsuarioNaoTemPermissaoParaAdicionarJustificativa);

                unitOfWork.Start();

                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    int codigo = int.Parse(Request.Params("Codigo"));
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo, true);

                    if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
                    {
                        EnviarJustificarivaCanhoto(canhoto, unitOfWork);
                        FinalizarEnvioCanhotosCTe(canhoto, unitOfWork);
                        if (canhoto.Carga != null)
                        {
                            Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoJustificativaDoCanhoto;
                            int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteEnvioCarga(canhoto.Carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                            if (numero == 0)
                                serCarga.SolicitarEncerramentoCarga(canhoto.Carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorConfirmacaoDeEntregaDosCanhotos, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, canhoto.GetChanges(), Localization.Resources.Canhotos.Canhoto.JustificouCanhoto, unitOfWork);

                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.NaoPermitidoJustificarCanhotoEmSuaAtualSituacao + " (" + canhoto.DescricaoSituacao + ").");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                bool? esperandoVinculo = Request.Params("EsperandoVinculo").ToNullableBool();

                if (esperandoVinculo != true)
                {
                    Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);
                    return ObterArquivoCanhoto(canhoto, unitOfWork);
                }
                else
                {
                    Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(unitOfWork);
                    Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo = repCanhotoEsperandoVinculo.BuscarPorCodigo(codigo);
                    return ObterArquivoCanhotoEsperandoVinculo(canhotoEsperandoVinculo, unitOfWork);
                }
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoRealizarDownloadDoCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                byte[] pdf = ReportRequest.WithType(ReportType.ComprovanteEntregaCanhoto)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("Codigo", Request.GetIntParam("Codigo").ToString())
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelGerarComprovanteDeEntrega);

                return Arquivo(pdf, "application/pdf", string.Format(Localization.Resources.Canhotos.Canhoto.ComprovanteDeEntrega, "Canhoto"));
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoRealizarImpressaoCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadCanhotosEmMassa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> listaCanhotos = repCanhoto.ConsultarArquivosDownloadEmMassa(filtrosPesquisa);

                if (listaCanhotos == null || listaCanhotos.Count == 0)
                    return new JsonpResult(false, true, "Registros não encontrados");

                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in listaCanhotos)
                {
                    string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);
                    string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                    string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
                    string nomeArquivo = canhoto.Numero + "_" + canhoto.Serie + "_" + canhoto.GuidNomeArquivo + extensao;
                    if (Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                    {
                        string nomeAbsolutoArquivoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);
                        byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo);
                        conteudoCompactar.Add(nomeArquivo, arquivoBinario);
                    }
                }

                if (conteudoCompactar?.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar as imagens para realizar o download.");

                MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

                arquivoCompactado.Dispose();

                if (arquivoCompactadoBinario == null)
                    return new JsonpResult(false, true, "Não foi possível gerar o arquivo.");

                return Arquivo(arquivoCompactadoBinario, "application/zip", $"Canhotos-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.zip");

            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataConfirmacaoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("CodigoCargaEntrega");
                DateTime dataEntrega = Request.GetDateTimeParam("DataEntrega");


                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repControleEntrega.BuscarPorCodigo(codigo);

                if (cargaEntrega == null)
                    return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelAlterarDataConfirmacaoEntrega);

                unitOfWork.Start();

                cargaEntrega.DataConfirmacao = dataEntrega;

                unitOfWork.CommitChanges();

                return new JsonpResult(true, Localization.Resources.Canhotos.Canhoto.DataEntregaAtualizada);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelAlterarDataConfirmacaoEntrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DisponibilizarCanhotoParaConsulta(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhoto_PermitirRetornarStatusCanhotoAPIDigitalizacao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                Repositorio.Embarcador.Canhotos.Canhoto repositorioConhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                List<int> codigos = Request.GetListParam<int>("Codigos");
                bool disponibilizar = Request.GetBoolParam("Disponibilizar");
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = await repositorioConhoto.BuscarPorCodigosAsync(codigos);

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    unitOfWork.Start();

                    canhoto.DigitalizacaoIntegrada = !disponibilizar;

                    servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, $"Canhoto {(!disponibilizar ? "" : "não")} disponibilizado para API", unitOfWork);

                    await repositorioConhoto.AtualizarAsync(canhoto);

                    unitOfWork.Flush();
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Falha ao alterar disponibilidade dos canhotos para consulta API");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Importações

        public async Task<IActionResult> ImportarDescarteCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilhaDescarte();
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };

                int contador = 0;
                string erro = string.Empty;
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;
                List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa>();

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveCanhoto = linha.Colunas?.Where(o => o.NomeCampo == "ChaveCanhoto").FirstOrDefault();
                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;
                        string chaveCanhoto = ((string)colChaveCanhoto?.Valor ?? string.Empty);
                        if (!string.IsNullOrWhiteSpace(chaveCanhoto))
                        {
                            canhoto = repCanhoto.BuscarPorChave(chaveCanhoto);
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroNota = linha.Colunas?.Where(o => o.NomeCampo == "Numero").FirstOrDefault();
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerie = linha.Colunas?.Where(o => o.NomeCampo == "Serie").FirstOrDefault();
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJEmitente = linha.Colunas?.Where(o => o.NomeCampo == "CNPJEmitente").FirstOrDefault();
                            int numero = 0;
                            int.TryParse(colNumeroNota?.Valor ?? "0", out numero);

                            string serie = colSerie?.Valor ?? "0";

                            double cnpjEmitente = 0;
                            double.TryParse(colCNPJEmitente?.Valor ?? "0", out cnpjEmitente);

                            canhoto = repCanhoto.BuscarPorNumeroSerieEmitenteNFe(numero, serie, cnpjEmitente);
                        }

                        if (canhoto == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.NaoForamEncontradosCanhotosParaOsDadosInformados, i));
                            continue;
                        }

                        //if (canhoto.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
                        //{
                        //    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A situação do canhoto não permite essa operação.", i));
                        //    continue;
                        //}
                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                        canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Cancelado;
                        canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Cancelada;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, Localization.Resources.Canhotos.Canhoto.CancelouPorImportacao, unitOfWork);
                        contador++;
                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                        if ((i % 10) == 0)
                        {
                            unitOfWork.FlushAndClear();
                        }
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoProcessarLinha, i));
                        continue;
                    }
                }

                retornoImportacao.Retorno = retorno;
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosCTePlanilha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;
                List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa>();

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveDocumento = linha.Colunas?.Where(o => o.NomeCampo == "ChaveDocumento").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroNota = linha.Colunas?.Where(o => o.NomeCampo == "NumeroNota").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerie = linha.Colunas?.Where(o => o.NomeCampo == "SerieNFe").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJEmitente = linha.Colunas?.Where(o => o.NomeCampo == "CNPJEmitente").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJTransportador = linha.Colunas?.Where(o => o.NomeCampo == "CNPJTransportador").FirstOrDefault();

                        string chaveDocumento = Utilidades.String.OnlyNumbers(((string)colChaveDocumento?.Valor ?? string.Empty));
                        int numeroNota = 0;
                        int.TryParse(colNumeroNota?.Valor ?? "0", out numeroNota);
                        double cnpjEmitente = 0;
                        double.TryParse(Utilidades.String.OnlyNumbers(colCNPJEmitente?.Valor ?? "0"), out cnpjEmitente);
                        string cnpjTransportador = Utilidades.String.OnlyNumbers(((string)colCNPJTransportador?.Valor ?? string.Empty));
                        string serie = colSerie?.Valor ?? "0";

                        if (!Utilidades.Validate.ValidarChave(chaveDocumento) && !configuracaoCanhoto.PermitirImportarDocumentosFiltroSemChaveNFe)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.ChaveInformadaInvalida, i));
                            continue;
                        }

                        if (chaveDocumento == string.Empty && (numeroNota == 0 || serie == "0" || cnpjEmitente == 0 || cnpjTransportador == ""))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Gerais.Geral.DadosInvalidos, i));
                            continue;
                        }

                        string modeloDocumento;
                        if (chaveDocumento != string.Empty)
                            modeloDocumento = chaveDocumento.Substring(20, 2);
                        else
                            modeloDocumento = "55";

                        if (modeloDocumento == "57")
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(chaveDocumento);

                            if (cte == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.CTeNaoEncontradoComChaveInformada, i));
                                continue;
                            }

                            if (!retorno.Any(o => o.Codigo == cte.Codigo && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.CTe))
                                retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa() { Codigo = cte.Codigo, Chave = chaveDocumento, Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.CTe });
                        }
                        else if (modeloDocumento == "55")
                        {
                            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto;
                            if (chaveDocumento != string.Empty)
                                canhoto = repCanhoto.BuscarPorChave(chaveDocumento);
                            else
                            {
                                canhoto = repCanhoto.BuscarPorNumeroSerieEmitenteNFeETransportador(numeroNota, serie, cnpjEmitente, cnpjTransportador);
                                numeroNota = 0;
                            }

                            if (canhoto == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.NFeNaoEncontradaComChaveInformada, i));
                                continue;
                            }

                            if (!retorno.Any(o => o.Codigo == canhoto.Codigo && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.NFe))
                                retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa() { Codigo = canhoto.Codigo, Chave = canhoto.XMLNotaFiscal.Chave, Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.NFe });
                        }
                        else
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.ChaveInformadaInvalida, i));
                        }
                        if (numeroNota > 0)
                        {
                            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorNumeroCanhoto(numeroNota);

                            if (canhoto == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.NumeroNotaNaoEncontradoComNumeroInformado, i));
                                continue;
                            }

                            if (!retorno.Any(o => o.Codigo == canhoto.Codigo))
                                retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa() { Codigo = canhoto.Codigo, NumeroNota = numeroNota });
                        }

                        contador++;

                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                        if ((i % 10) == 0)
                        {
                            unitOfWork.FlushAndClear();
                        }
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.OcorreUmaFalhaAoProcessarLinha, i));
                        continue;
                    }
                }

                retornoImportacao.Retorno = retorno;
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                int contador = 0;
                int totalLinhas = linhas.Count;

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveNFe = linha.Colunas?.Where(o => o.NomeCampo == "ChaveNFe").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumero = linha.Colunas?.Where(o => o.NomeCampo == "Numero").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroCarga = linha.Colunas?.Where(o => o.NomeCampo == "NumeroCarga").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJDestinatario = linha.Colunas?.Where(o => o.NomeCampo == "CNPJDestinatario").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJTransportador = linha.Colunas?.Where(o => o.NomeCampo == "CNPJTransportador").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoFilial = linha.Colunas?.Where(o => o.NomeCampo == "Filial").FirstOrDefault();

                        string chaveNFe = Utilidades.String.OnlyNumbers(((string)colChaveNFe?.Valor ?? string.Empty));
                        int numeroCanhoto = 0;
                        int.TryParse(colNumero?.Valor ?? "0", out numeroCanhoto);
                        string numeroCarga = colNumeroCarga?.Valor ?? string.Empty;
                        double cpfCNPJDestinatario = double.Parse(Utilidades.String.OnlyNumbers((string)colCNPJDestinatario?.Valor ?? string.Empty));
                        string CNPJTransportador = Utilidades.String.OnlyNumbers((string)colCNPJTransportador?.Valor ?? string.Empty);
                        string codigoFilial = colCodigoFilial?.Valor ?? string.Empty;

                        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        string erroLinha = ValidarInformacoesImportacao(chaveNFe, numeroCanhoto, numeroCarga, cpfCNPJDestinatario, CNPJTransportador, codigoFilial, repFilial, repCarga, repEmpresa, repCliente);

                        if (!string.IsNullOrWhiteSpace(erroLinha))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(erroLinha, i));
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorCodigoCargaEmbarcador(numeroCarga);
                        Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cpfCNPJDestinatario);
                        Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(CNPJTransportador);
                        Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(codigoFilial);
                        Dominio.Entidades.Cliente emitente = carga.Pedidos.Select(o => o.Pedido.Remetente).Where(o => o != null).FirstOrDefault();

                        Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                        Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);

                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = serNFe.CriarXMLNotaFiscal(emitente, destinatario, filial, chaveNFe, numeroCanhoto, unitOfWork);
                        serCanhoto.CriarCanhotoDevolucao(carga, destinatario, xmlNotaFiscal, filial, transportador, numeroCanhoto, unitOfWork);

                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });
                        contador++;

                        if ((i % 10) == 0)
                            unitOfWork.FlushAndClear();
                    }
                    catch (ServicoException ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.OcorreUmaFalhaAoProcessarLinha, i));
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.OcorreUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.Total = totalLinhas;
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarAtualizacaoCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                int contador = 0;
                int totalLinhas = linhas.Count;

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        unitOfWork.Start();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroNFe = linha.Colunas?.FirstOrDefault(o => o.NomeCampo == "NumeroNFe");
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSerieNFe = linha.Colunas?.FirstOrDefault(o => o.NomeCampo == "SerieNFe");
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaChaveNFe = linha.Colunas?.FirstOrDefault(o => o.NomeCampo == "ChaveNFe");
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCNPJEmitente = linha.Colunas?.FirstOrDefault(o => o.NomeCampo == "CNPJEmitente");
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSituacaoNota = linha.Colunas?.FirstOrDefault(o => o.NomeCampo == "SituacaoNota");
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroCanhotoAvulso = linha.Colunas?.FirstOrDefault(o => o.NomeCampo == "NumeroCanhotoAvulso");

                        Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                        Repositorio.Embarcador.Canhotos.CanhotoAvulso repositorioCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(unitOfWork);
                        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork).BuscarConfiguracaoPadrao();
                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = new Dominio.Entidades.Embarcador.Canhotos.Canhoto();

                        int numeroNFe = ((string)colunaNumeroNFe?.Valor).ToInt();
                        int situacaoNota = ((string)colunaSituacaoNota?.Valor).ToInt();
                        string erroLinha = string.Empty;
                        string observacao = "Confirmação da Digitalização realizada por Importação";
                        string chaveNFe = Utilidades.String.OnlyNumbers(((string)colunaChaveNFe?.Valor ?? string.Empty));
                        string serieNFe = colunaSerieNFe?.Valor ?? string.Empty;
                        string CNPJEmitente = Utilidades.String.OnlyNumbers((string)colunaCNPJEmitente?.Valor ?? string.Empty);
                        int.TryParse((string)colunaNumeroCanhotoAvulso?.Valor, out int numeroCanhotoAvulso);

                        if (numeroCanhotoAvulso == 0 && configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoPorImportacao)
                        {
                            if (!Utilidades.Validate.ValidarChave(chaveNFe))
                                erroLinha = Localization.Resources.Canhotos.Canhoto.ChaveInformadaInvalida;

                            if (numeroNFe == 0)
                                erroLinha = "Informe o número da NFe";

                            if (string.IsNullOrEmpty(serieNFe))
                                erroLinha = "Informe a série da NFe";

                            if (situacaoNota < 1 || situacaoNota > 5)
                                erroLinha = "Situação da Nota informada está inválida";

                            if (string.IsNullOrEmpty(CNPJEmitente))
                                erroLinha = "Informe o CNPJ do Emitente";

                            canhoto = repositorioCanhoto.BuscarPorNumeroSerieEmitenteNFe(numeroNFe, serieNFe, CNPJEmitente.ToDouble());

                            if (!string.IsNullOrWhiteSpace(erroLinha))
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(erroLinha, i));
                                continue;
                            }
                            if (canhoto == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Canhoto não localizado", i));
                                continue;
                            }

                            if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Canhoto já digitalizado", i));
                                continue;
                            }


                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repositorioNotaFiscal.BuscarPorChave(chaveNFe);

                            if (situacaoNota == 1)
                                notaFiscal.SituacaoEntregaNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.AgEntrega;

                            else if (situacaoNota == 2)
                                notaFiscal.SituacaoEntregaNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Entregue;

                            else if (situacaoNota == 3)
                                notaFiscal.SituacaoEntregaNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Devolvida;

                            else if (situacaoNota == 4)
                                notaFiscal.SituacaoEntregaNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.DevolvidaParcial;

                            else if (situacaoNota == 5)
                                notaFiscal.SituacaoEntregaNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.AgReentrega;

                            repositorioNotaFiscal.Atualizar(notaFiscal);
                        }
                        else
                        {
                            canhoto = repositorioCanhotoAvulso.BuscarCanhotoPorNumeroCanhotoAvulso(numeroCanhotoAvulso);

                            if (canhoto == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Canhoto Avulso não localizado", i));
                                continue;
                            }
                            if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Canhoto Avulso já Digitalizado", i));
                                continue;
                            }
                        }

                        if ((canhoto.Carga.TipoOperacao?.ConfiguracaoCanhoto?.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado ?? false) && servicoCanhoto.CanhotoPossuiCTeNaoAutorizado(canhoto))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(string.Format(Localization.Resources.Canhotos.Canhoto.CanhotoNaoPossuiCTeAutorizado, canhoto.XMLNotaFiscal.Numero.ToString()), i));
                            continue;
                        }

                        canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao;
                        canhoto.DataDigitalizacao = DateTime.Now;

                        AceitarEnvio(canhoto, unitOfWork, out string mensagemValidacao, observacao);

                        if (string.IsNullOrEmpty(mensagemValidacao))
                            unitOfWork.CommitChanges();
                        else
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(mensagemValidacao, i));
                            continue;
                        }

                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });
                        contador++;
                    }
                    catch (ServicoException ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.OcorreUmaFalhaAoProcessarLinha, i));
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.OcorreUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.Total = totalLinhas;
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ValidarInformacoesImportacao(string chaveNFe, int numeroCanhoto, string numeroCarga, double cpfCNPJDestinatario, string CNPJTransportador, string codigoFilial, Filial repFilial, Repositorio.Embarcador.Cargas.Carga repCarga, Repositorio.Empresa repEmpresa, Cliente repCliente)
        {
            if (!Utilidades.Validate.ValidarChave(chaveNFe))
                return Localization.Resources.Canhotos.Canhoto.ChaveInformadaInvalida;

            if (numeroCanhoto == 0)
                return Localization.Resources.Canhotos.Canhoto.NumeroSerieNaoFoiInformada;

            if (string.IsNullOrWhiteSpace(numeroCarga))
                return Localization.Resources.Canhotos.Canhoto.NumeroCargaNaoInformado;

            if (!repCarga.ExiteCargaPorCodigoEmbarcador(numeroCarga))
                return Localization.Resources.Canhotos.Canhoto.CargaNaoEncontrada;

            if (cpfCNPJDestinatario == 0)
                return Localization.Resources.Canhotos.Canhoto.CNPJDestinatarioNaoInformado;

            if (!repCliente.ExistePorCPFCNPJ(cpfCNPJDestinatario))
                return Localization.Resources.Canhotos.Canhoto.DestinatarioNaoEncontrado;

            if (string.IsNullOrWhiteSpace(CNPJTransportador))
                return Localization.Resources.Canhotos.Canhoto.CNPJTransportadorNaoInformado;

            if (!repEmpresa.ExistePorCNPJ(CNPJTransportador))
                return Localization.Resources.Canhotos.Canhoto.TransportadorNaoEncontrado;

            if (string.IsNullOrWhiteSpace(codigoFilial))
                return Localization.Resources.Canhotos.Canhoto.CodigoFilialNaoInformado;

            if (!repFilial.ValidarPorCodigoEmbarcador(codigoFilial, 0))
                return Localization.Resources.Canhotos.Canhoto.FilialNaoEncontrada;

            return string.Empty;
        }

        public async Task<IActionResult> ImportarDataEntregaCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao
                {
                    Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
                };

                int contador = 0;
                string erro = string.Empty;
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroSerie = linha.Colunas.Where(o => o.NomeCampo == "NumeroSerie").FirstOrDefault();
                        string numeroSerie = (string)colNumeroSerie?.Valor ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(numeroSerie))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.NumeroSerieNaoFoiInformada, i));
                            continue;
                        }

                        string[] numeroSerieSeparados = numeroSerie.Split('-');
                        if (numeroSerieSeparados.Length != 2)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.NumeroSerieSeparadoPorHifenPrecisamEstarInformados, i));
                            continue;
                        }

                        int numero = numeroSerieSeparados[0].Trim().ToInt();
                        string serie = numeroSerieSeparados[1].Trim();
                        if (numero == 0 || string.IsNullOrWhiteSpace(serie))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.NumeroOuSerieNaoInformado, i));
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEntrega = linha.Colunas.Where(o => o.NomeCampo == "DataEntrega").FirstOrDefault();
                        string dataEntrega = (string)colDataEntrega?.Valor ?? string.Empty;

                        DateTime dataEntregaNotaCliente = DateTime.MinValue;
                        if (!string.IsNullOrWhiteSpace(dataEntrega))
                            dataEntregaNotaCliente = dataEntrega.ToDateTime();

                        if (dataEntregaNotaCliente == DateTime.MinValue)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.DataDeEntregaNaoFoiInformada, i));
                            continue;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJEmitente = linha.Colunas.Where(o => o.NomeCampo == "CNPJEmitente").FirstOrDefault();
                        double.TryParse(colCNPJEmitente?.Valor ?? "0", out double cnpjEmitente);

                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorNumeroSerieEmitente(numero, serie, cnpjEmitente);
                        if (canhoto == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.NaoForamEncontradosCanhotosParaOsDadosInformados, i));
                            continue;
                        }

                        if (canhoto.SituacaoCanhoto != SituacaoCanhoto.Pendente)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.SituacaoDoCanhotoNaoPermiteEssaOperacao, i));
                            continue;
                        }

                        canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente.Date;
                        repCanhoto.Atualizar(canhoto);

                        servicoCanhoto.GerarHistoricoCanhoto(canhoto, Usuario, Localization.Resources.Canhotos.Canhoto.EnviouDataDaEntregaDaNotaAoClientePorImportacao, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.EnviouDataDeEntregaDaNotaAoClientePorImportacao, unitOfWork);

                        contador++;
                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                        if ((i % 10) == 0)
                        {
                            unitOfWork.FlushAndClear();
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoProcessarLinha, i));
                        continue;
                    }
                }

                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilha();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoDescarte()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilhaDescarte();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoDataEntrega()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilhaDataEntrega();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }
        public async Task<IActionResult> ConfiguracaoImportacaoAtualizarCanhotos()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoAtualizarCanhotos();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoCanhotos()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoCanhotos();
                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }

        public async Task<IActionResult> ValidaObrigatoriedadeDataEntregaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string chave = Request.GetStringParam("DadosBaixa");

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorChave(chave);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if ((canhoto?.XMLNotaFiscal?.Destinatario?.ObrigarInformarDataEntregaClienteAoBaixarCanhotos ?? false) && (configuracao?.ObrigatorioInformarDataEnvioCanhoto ?? false) && (!canhoto.DataEntregaNotaCliente.HasValue))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarOsCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarDataEntregaNotaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                DateTime dataEntregaNotaCliente = Request.GetDateTimeParam("DataEntregaNotaCliente");
                string chave = Request.GetStringParam("DadosBaixa");

                if (dataEntregaNotaCliente == DateTime.MinValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataNaoFoiSelecionada);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorChave(chave);

                canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente;
                repositorioCanhoto.Atualizar(canhoto);

                servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.EnviouDataDaEntregaDaNotaAoClienteSeparadaDaImagemDoCanhoto, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.EnviouDataDaEntregaDaNotaAoClienteSeparadaDaImagemDoCanhoto, unitOfWork);


                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarDataDaEntregaDaNotaAoCliente);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                DateTime dataEntregaNotaCliente = Request.GetDateTimeParam("DataEntregaNotaCliente");
                int codigoCanhoto = Request.GetIntParam("CodigoCanhoto");

                if (dataEntregaNotaCliente == DateTime.MinValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataNaoFoiSelecionada);

                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorCodigo(codigoCanhoto);

                canhoto.Initialize();
                canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente;
                await repositorioCanhoto.AtualizarAsync(canhoto);

                servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.EnviouDataDaEntregaDaNotaAoClienteSeparadaDaImagemDoCanhoto, unitOfWork);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.EnviouDataDaEntregaDaNotaAoClienteSeparadaDaImagemDoCanhoto, unitOfWork);

                if (ConfiguracaoEmbarcador.ExigirDataEntregaNotaClienteCanhotos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal = null;
                    if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                        XMLNotaFiscal = canhoto.XMLNotaFiscal;
                    else if (canhoto.TipoCanhoto == TipoCanhoto.Avulso && canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Count > 0)
                        XMLNotaFiscal = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.FirstOrDefault().XMLNotaFiscal;

                    if (XMLNotaFiscal != null && canhoto.Carga != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repCargaEntregaNotaFiscal.BuscarPorCargaENFe(canhoto.Carga.Codigo, XMLNotaFiscal.Codigo);
                        if (cargaEntregaNotaFiscal != null && cargaEntregaNotaFiscal.CargaEntrega.Situacao == SituacaoEntrega.NaoEntregue)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(canhoto.Latitude, canhoto.Longitude);

                            OrigemSituacaoEntrega origemSituacaoEntrega = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;

                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntregaNotaFiscal.CargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntregaNotaFiscal.CargaEntrega, canhoto.DataEntregaNotaCliente.Value, wayPoint, null, 0, "", ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, origemSituacaoEntrega, this.Cliente, unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
                            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaEntregaNotaFiscal.CargaEntrega, cargaEntregaNotaFiscal.CargaEntrega.GetChanges(), "Entrega confirmada via Upload Canhoto", unitOfWork);
                        }
                        else
                        {
                            cargaEntregaNotaFiscal.CargaEntrega.Initialize();
                            cargaEntregaNotaFiscal.CargaEntrega.DataConfirmacao = dataEntregaNotaCliente;
                            if (cargaEntregaNotaFiscal.CargaEntrega.DataFim < dataEntregaNotaCliente)
                                cargaEntregaNotaFiscal.CargaEntrega.DataFim = dataEntregaNotaCliente;
                            if (cargaEntregaNotaFiscal.CargaEntrega.DataInicio > dataEntregaNotaCliente)
                                cargaEntregaNotaFiscal.CargaEntrega.DataInicio = dataEntregaNotaCliente;
                            await repCargaEntrega.AtualizarAsync(cargaEntregaNotaFiscal.CargaEntrega);
                            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaEntregaNotaFiscal.CargaEntrega, cargaEntregaNotaFiscal.CargaEntrega.GetChanges(), "Alterou a data de confirmação via Upload Canhoto", unitOfWork);

                            new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, null).ProcessarRegrasDeQualidadeDeEntrega(cargaEntregaNotaFiscal.CargaEntrega);
                        }
                    }
                }

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarDataDaEntregaDaNotaAoCliente);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        #endregion

        public async Task<IActionResult> RemoverCanhotoLocalArmazenamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                int codigoCanhoto = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);

                unitOfWork.Start();

                if (!Servicos.Embarcador.Canhotos.Canhoto.RemoverCanhotoLocalArmazenamento(out string mensagem, this.Usuario, canhoto, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagem);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoRemoverCanhotoDoLocalDeArmazenamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReverterBaixaCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                int codigoCanhoto = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);

                unitOfWork.Start();

                if (!Servicos.Embarcador.Canhotos.Canhoto.ReverterBaixaCanhoto(out string mensagem, this.Usuario, canhoto, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagem);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoReverterBaixaDoCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarPgto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhotos_ControlarSituacaoPagamento))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorCodigo(codigo);

                if (canhoto == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (canhoto.SituacaoPgtoCanhoto != SituacaoPgtoCanhoto.Pendente)
                    throw new ControllerException(string.Format(Localization.Resources.Canhotos.Canhoto.SituacaoNaoPermiteLiberar, canhoto.SituacaoPgtoCanhoto.ObterDescricao()));

                if (canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                    throw new ControllerException(string.Format(Localization.Resources.Canhotos.Canhoto.NaoPermitidoLiberarPagamentoQuandoSituacaoDiferenteDeDigitalizacao, canhoto.DescricaoDigitalizacao));

                Dominio.Entidades.Embarcador.Cargas.Carga carga = canhoto.Carga;

                if (carga != null && !carga.CargaIntegradaEmbarcador && !carga.Pedidos.All(cargaPedido => cargaPedido.CargaPedidoIntegrada == true))
                    throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoPossivelLiberarPagamentoSecargaAindaNaoEstiverFinalizada);

                canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Liberado;
                canhoto.DataLiberacaoPagamento = DateTime.Now;
                canhoto.UsuarioLiberacaoPagamento = this.Usuario;
                repositorioCanhoto.Atualizar(canhoto);

                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.LiberouPagamentoDoCanhoto, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.LiberouPagamentoDoCanhoto, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoLiberarPagamentoDoCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarMultiplosPagamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhotos_ControlarSituacaoPagamento))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                int quantidadeCanhotosPermitida = 100000;
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionados(unitOfWork, quantidadeCanhotosPermitida, out bool quantidadeValida, out int quantidadeSelecionada);

                if (!quantidadeValida)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));

                unitOfWork.Start();

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                int canhotosLiberadoPagamento = 0;

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    if (canhoto.SituacaoPgtoCanhoto != SituacaoPgtoCanhoto.Pendente)
                        continue;

                    if (canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = canhoto.Carga;

                    if (carga != null && !carga.CargaIntegradaEmbarcador && !carga.Pedidos.All(cargaPedido => cargaPedido.CargaPedidoIntegrada == true))
                        continue;

                    canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Liberado;
                    canhoto.DataLiberacaoPagamento = DateTime.Now;
                    canhoto.UsuarioLiberacaoPagamento = this.Usuario;
                    repositorioCanhoto.Atualizar(canhoto);

                    servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.LiberouPagamentoDoCanhoto, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.LiberouPagamentoDoCanhoto, unitOfWork);

                    canhotosLiberadoPagamento++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    CanhotosLiberadoPagamento = canhotosLiberadoPagamento
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoLiberarPagamentoDeMultiplosCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarPgto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhotos_ControlarSituacaoPagamento))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorCodigo(codigo);

                if (canhoto == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (canhoto.SituacaoPgtoCanhoto != SituacaoPgtoCanhoto.Pendente)
                    throw new ControllerException(string.Format(Localization.Resources.Canhotos.Canhoto.SituacaoNaoPermiteRejeitar, canhoto.SituacaoPgtoCanhoto.ObterDescricao()));

                if (canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                    throw new ControllerException(string.Format(Localization.Resources.Canhotos.Canhoto.NaoPermitidoRejeitarPagamentoQuandoSituacaoDiferenteDeDigitalizado, canhoto.DescricaoDigitalizacao));

                canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Rejeitado;
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                repositorioCanhoto.Atualizar(canhoto);

                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.RejeitouPagamentoDoCanhotoDisponibilizadoParaNovaDigitalizacao, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.RejeitouPagamentoDoCanhotoDisponibilizadoParaNovaDigitalizacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoRejeitarPagamentoDoCanhoto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarMultiplosPagamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Canhotos/Canhoto");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Canhotos_ControlarSituacaoPagamento))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.AcaoInvalida);

                int quantidadeCanhotosPermitida = 300;
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionados(unitOfWork, quantidadeCanhotosPermitida, out bool quantidadeValida, out int quantidadeSelecionada);

                if (!quantidadeValida)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));

                unitOfWork.Start();

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                int canhotosRejeitadoPagamento = 0;

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    if (canhoto.SituacaoPgtoCanhoto != SituacaoPgtoCanhoto.Pendente)
                        continue;

                    if (canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                        continue;

                    canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Rejeitado;
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                    repositorioCanhoto.Atualizar(canhoto);

                    servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.RejeitouPagamentoDoCanhotoDisponibilizandoParaNovaDigitalizacao, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.RejeitouPagamentoDoCanhotoDisponibilizandoParaNovaDigitalizacao, unitOfWork);

                    canhotosRejeitadoPagamento++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    CanhotosRejeitadoPagamento = canhotosRejeitadoPagamento
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoRejeitarPagamentoDeMultiplosCanhotos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarMultiplasDataEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int quantidadeCanhotosPermitida = 300;
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosSelecionados(unitOfWork, quantidadeCanhotosPermitida, out bool quantidadeValida, out int quantidadeSelecionada);

                if (!quantidadeValida)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Canhotos.Canhoto.QuantidadeDeCanhotosSelecionadaUltrapassaQuantidadePermitidaSelecioneMenosCanhotos, quantidadeSelecionada, quantidadeCanhotosPermitida));

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                DateTime dataEntregaNotaCliente = Request.GetDateTimeParam("DataEntregaNotaCliente");
                if (dataEntregaNotaCliente == DateTime.MinValue)
                    return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.DataNaoFoiSelecionada);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente;
                    repositorioCanhoto.Atualizar(canhoto);

                    servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.EnviouDataDaEntregaDaNotaAoClienteSeparadaDaImagemDoCanhoto, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.EnviouDataDaEntregaDaNotaAoClienteSeparadaDaImagemDoCanhoto, unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoEnviarDataDaEntregaDaNotaAoCliente);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AuditarRegistro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                int codigoCanhoto = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);

                unitOfWork.Start();

                //Logica

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocurreu um error ao tentar auditora registro");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDetalhesDoCanhotoParaAuditoria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);

                var dynCanhoto = new
                {
                    canhoto.Codigo,
                    canhoto.Numero,
                    Chave = canhoto.ChaveCTe,
                    DataDigitalizacao = canhoto.DataDigitalizacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataRecebimentoFisico = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataAprovacao = canhoto.DataUltimaModificacao.ToString("dd/MM/yyyy") ?? string.Empty
                };

                return new JsonpResult(dynCanhoto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarAuditoria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);
                Dominio.ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto aprovadoAditoria = Dominio.ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto.Aprovado;
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                PreecherRegistroDeAuditoria(canhoto, aprovadoAditoria, unitOfWork);

                repCargaCanhoto.Atualizar(canhoto, Auditado);
                servicoCanhoto.GerarHistoricoCanhoto(canhoto, Usuario, "Canhoto Auditado Aprovado", unitOfWork);

                return new JsonpResult(true, true, "Canhoto Auditado com succeso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarAuditoria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCargaCanhoto.BuscarPorCodigo(codigo);
                Dominio.ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto reprovarAditoria = Dominio.ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto.Reprovado;

                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                PreecherRegistroDeAuditoria(canhoto, reprovarAditoria, unitOfWork);

                if (canhoto.MotivoRejeicaoAuditoria == null)
                    return new JsonpResult(false, "Para rejeitar precisa informar um motivo");

                repCargaCanhoto.Atualizar(canhoto, Auditado);
                servicoCanhoto.GerarHistoricoCanhoto(canhoto, Usuario, "Canhoto Auditado Rejeitado", unitOfWork);

                return new JsonpResult(true, true, "Rejeição feita com succeso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadFotosNotaEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorCodigo(codigo);

                if (canhoto == null)
                    return new JsonpResult(false, true, "Registro não encontrado");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregas = repositorioCargaEntrega.BuscarPorCarga(canhoto.Carga.Codigo);

                if (cargasEntregas?.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar a entrega a partir do canhoto.");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> cargaEntregaFotos = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntregas)
                    cargaEntregaFotos.AddRange(cargaEntrega.Fotos.ToList());

                if (cargaEntregaFotos?.Count == 0)
                    return new JsonpResult(false, true, "Não foi encontrado nenhuma foto pra realizar o download.");

                string caminhoImagens = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                //List<string> imagemNaoEncontrada = new List<string>();

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto cargaEntregaFoto in cargaEntregaFotos)
                {
                    string extensao = System.IO.Path.GetExtension(cargaEntregaFoto.NomeArquivo).ToLower();
                    string nomeGuidArquivo = $"{cargaEntregaFoto.GuidArquivo}-miniatura{extensao}";
                    string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoImagens, nomeGuidArquivo);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                    {
                        //imagemNaoEncontrada.Add(nomeGuidArquivo);

                        string nomeAbsolutoArquivoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminhoImagens, cargaEntregaFoto.NomeArquivo);
                        byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo);

                        conteudoCompactar.Add(nomeGuidArquivo, arquivoBinario);
                    }
                }

                if (conteudoCompactar?.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar as imagens para realizar o download.");

                MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

                arquivoCompactado.Dispose();

                if (arquivoCompactadoBinario == null)
                    return new JsonpResult(false, true, "Não foi possível gerar o arquivo.");

                return Arquivo(arquivoCompactadoBinario, "application/zip", $"Fotos-Nota-Entrega-Canhoto-{canhoto.Numero}.zip");

            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarConfiguracoesGeraisCanhoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioConfiguracaoCanhoto = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

                var configuracoesCanhoto = new
                {
                    PermitirAlterarImagemCanhotoDigitalizada = configuracaoCanhoto?.PermitirAlterarImagemCanhotoDigitalizada ?? false,
                    PermitirAprovarDigitalizacaoDeCanhotoRejeitado = configuracaoCanhoto?.PermitirAprovarDigitalizacaoDeCanhotoRejeitado ?? false,
                    ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente = configuracaoCanhoto?.ValidarSituacaoDigitalizacaoCanhotosAoSumarizarDocumentoFaturamento ?? false,
                    PermitirRetornarStatusCanhotoNaAPIDigitalizacao = configuracaoCanhoto?.PermitirRetornarStatusCanhotoNaAPIDigitalizacao ?? false
                };

                return new JsonpResult(configuracoesCanhoto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Falha ao buscar configurações para canhoto");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AceitarDigitalizacaoCanhotoReprovado(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");
                string observacao = Request.Params("Observacao");
                bool HouveFalhaNaValidacao = Request.GetBoolParam("HouveFalhaNaValidacao");
                bool FalhaIA_Comprovante = Request.GetBoolParam("FalhaIA_Comprovante");
                bool FalhaIA_NumeroDocumento = Request.GetBoolParam("FalhaIA_NumeroDocumento");
                bool FalhaIA_Data = Request.GetBoolParam("FalhaIA_Data");
                bool FalhaIA_Assinatura = Request.GetBoolParam("FalhaIA_Assinatura");

                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork, cancellationToken);
                Servicos.Embarcador.Canhotos.CanhotoIntegracao repositorioCanhotoIntegracao = new Servicos.Embarcador.Canhotos.CanhotoIntegracao(unitOfWork, TipoServicoMultisoftware, ClienteAcesso, cancellationToken);
                Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = await repositorioCanhoto.BuscarPorCodigosAsync(codigos);

                if (canhotos.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    await unitOfWork.StartAsync(cancellationToken);

                    canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado;
                    canhoto.SituacaoCanhoto = SituacaoCanhoto.Justificado;
                    canhoto.DataAprovacaoDigitalizacao = DateTime.Now;
                    canhoto.DigitalizacaoIntegrada = false;
                    canhoto.Observacao = observacao;
                    canhoto.SituacaoPgtoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Pendente;
                    canhoto.UsuarioDigitalizacao = this.Usuario;
                    canhoto.OrigemSituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoDigitalizacaoCanhoto.Manual;

                    await repositorioCanhotoIntegracao.GerarIntegracaoAceiteCanhotoAsync(canhoto, TipoServicoMultisoftware);
                    await servicoCanhoto.CanhotoLiberadoAsync(canhoto, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente);
                    await servicoCanhoto.FinalizarDigitalizacaoCanhotoAsync(canhoto, TipoServicoMultisoftware);

                    canhoto.MotivoRejeicaoDigitalizacao = "";
                    canhoto.DataUltimaModificacao = DateTime.Now;

                    canhoto.HouveFalhaNaValidacao = HouveFalhaNaValidacao;
                    canhoto.FalhaValidacaoCanhoto = HouveFalhaNaValidacao && FalhaIA_Comprovante;
                    canhoto.FalhaValidacaoNumero = HouveFalhaNaValidacao && FalhaIA_NumeroDocumento;
                    canhoto.FalhaValidacaoEncontrouData = HouveFalhaNaValidacao && FalhaIA_Data;
                    canhoto.FalhaValidacaoAssinatura = HouveFalhaNaValidacao && FalhaIA_Assinatura;

                    await repositorioCanhoto.AtualizarAsync(canhoto);
                    await servicoCanhoto.GerarHistoricoCanhotoAsync(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.DigitalizacaoDoCanhotoAprovada, observacao);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && canhoto.Carga != null)
                    {
                        Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorAceiteDoCanhoto;
                        int numero = servicoCanhoto.VerificarQuantidadeCanhotosPendenteAceiteImagem(canhoto.Carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                        if (numero == 0)
                            new Servicos.Embarcador.Carga.Carga(unitOfWork).SolicitarEncerramentoCarga(canhoto.Carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorAceiteDasImagensDosCanhotosDaCarga, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                    }

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, canhoto, canhoto.GetChanges(), Localization.Resources.Canhotos.Canhoto.AceitouDeImagemDoCanhoto, unitOfWork);

                    await unitOfWork.CommitChangesAsync(cancellationToken);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoConfirmarAceiteDaDigitalizacaoDoCanhoto);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarDigitalizacaoSelecaoCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                List<int> codigos = Request.GetListParam<int>("Codigos");
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCodigos(codigos);

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                    {
                        unitOfWork.Start();

                        canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado;
                        canhoto.DigitalizacaoIntegrada = false;
                        canhoto.DataDigitalizacao = DateTime.Now;
                        canhoto.DataAprovacaoDigitalizacao = DateTime.Now;
                        canhoto.UsuarioDigitalizacao = this.Usuario;
                        canhoto.SituacaoPgtoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Pendente;
                        canhoto.OrigemSituacaoDigitalizacaoCanhoto = OrigemSituacaoDigitalizacaoCanhoto.Manual;

                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                        Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, unitOfWork);
                        Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);

                        serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.ConfirmouDigitalizacaoDosCanhotos, unitOfWork);

                        repCanhoto.Atualizar(canhoto);

                        unitOfWork.CommitChanges();
                    }
                }
                return new JsonpResult(true);

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
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.ObservacaoAlteracaoCanhoto ObterDadosObservacaoAlteracaoCanhoto()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Canhoto.ObservacaoAlteracaoCanhoto
            {
                ObservacaoOperador = Request.GetStringParam("ObservacaoOperador"),
                Data = Request.GetNullableDateTimeParam("DataAlteracaoCanhoto"),
                CodigoRastreio = Request.GetStringParam("CodigoRastreio")
            };
        }
        private void EnviarEmailInconsistenciaDigitacaoCanhotoResponsavelPrestacaoContasTransportadora(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Canhotos.InconsistenciaDigitacaoCanhoto inconsistenciaDigitacaoCanhoto)
        {
            if (!string.IsNullOrWhiteSpace(inconsistenciaDigitacaoCanhoto.Canhoto.Filial?.Email))
            {
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail != null)
                {
                    string assunto = Localization.Resources.Canhotos.Canhoto.CanhotoComIrregularidadeNaDigitalizacao;
                    System.Text.StringBuilder mensagem = new System.Text.StringBuilder();

                    mensagem.Append(Localization.Resources.Canhotos.Canhoto.Ola + ", ").AppendLine().AppendLine();
                    mensagem.Append(string.Format(Localization.Resources.Canhotos.Canhoto.DigitalizacaoDoCanhotoNumeroFoiRejeitada, inconsistenciaDigitacaoCanhoto.Canhoto.Numero)).AppendLine();
                    mensagem.Append(Localization.Resources.Canhotos.Canhoto.Motivo + $": {inconsistenciaDigitacaoCanhoto.MotivoInconsistenciaDigitacao.Descricao}.").AppendLine().AppendLine();

                    if (!string.IsNullOrWhiteSpace(inconsistenciaDigitacaoCanhoto.Observacoes))
                        mensagem.Append(Localization.Resources.Canhotos.Canhoto.Observacoes + $": {inconsistenciaDigitacaoCanhoto.Observacoes}");

                    mensagem.Append(Localization.Resources.Canhotos.Canhoto.EmailEnviadoAutomaticamentePorFavorNaoResponda);

                    if (!string.IsNullOrWhiteSpace(configuracaoEmail.MensagemRodape))
                        mensagem.AppendLine().AppendLine().Append(configuracaoEmail.MensagemRodape.Replace("#qLinha#", "<br/>"));

                    try
                    {
                        Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, inconsistenciaDigitacaoCanhoto.Canhoto.Filial.Email, null, null, assunto, mensagem.ToString(), configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, null, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(string.Format(Localization.Resources.Canhotos.Canhoto.ErroAoEnviarEmailDeInconsistenciaNaDigitalizacaoDeCanhoto, excecao.ToString()));
                    }
                }
            }
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "DescricaoTipoCanhoto")
                propOrdena = "TipoCanhoto";

            if (propOrdena == "DescricaoDigitalizacao")
                propOrdena = "SituacaoDigitalizacaoCanhoto";

            if (propOrdena == "DescricaoSituacao")
                propOrdena = "SituacaoCanhoto";

            else if (propOrdena == "Emitente")
                propOrdena = "Emitente.Nome";

            else if (propOrdena == "Empresa")
                propOrdena += ".RazaoSocial";

            else if (propOrdena == "DataNotaFiscal")
                propOrdena = "XMLNotaFiscal.DataEmissao";

            else if (propOrdena == "DataDigitalizacao")
                propOrdena = "DataEnvioCanhoto";
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoLocalArmazenamento", false);
            grid.AdicionarCabecalho("DescricaoLocalArmazenamento", false);
            grid.AdicionarCabecalho("GuidNomeArquivo", false);
            grid.AdicionarCabecalho("Observacao", false);
            grid.AdicionarCabecalho("SituacaoCanhoto", false);
            grid.AdicionarCabecalho("SituacaoDigitalizacaoCanhoto", false);
            grid.AdicionarCabecalho("CargaEncerrada", false);
            grid.AdicionarCabecalho("ExibirOpcaoAlterarDataEntrega", false);

            grid.AdicionarCabecalho("PossuiIntegracaoComprovei", false);
            grid.AdicionarCabecalho("ValidacaoCanhotoComprovei", false);
            grid.AdicionarCabecalho("ValidacaoNumeroComprovei", false);
            grid.AdicionarCabecalho("ValidacaoEncontrouDataComprovei", false);
            grid.AdicionarCabecalho("ValidacaoAssinaturaComprovei", false);
            grid.AdicionarCabecalho("EnvioCanhotoFaturaHabilitado", false);
            grid.AdicionarCabecalho("CancelarAtendimentoAutomaticamente", false);
            grid.AdicionarCabecalho("SituacaoNotaFiscal", false);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Numero, "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Serie, "Serie", 10, Models.Grid.Align.center, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.CTe, "NumeroCTe", 10, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DocumentoOriginario, "NumeroDocumentoOriginario", 10, Models.Grid.Align.left, false, true);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Tipo, "DescricaoTipoCanhoto", 8, Models.Grid.Align.center, false);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataEmissao, "DataEmissaoDescricao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.ChaveDaNFe, "Chave", 20, Models.Grid.Align.left, false, false);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.NumeroCarga, "NumeroCarga", 10, Models.Grid.Align.center, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Emitente, "Emitente", 20, Models.Grid.Align.left, true);
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.ValorNFe, "Valor", 10, Models.Grid.Align.left, false, true);
                    grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Destinatario, "Destinatario", 25, Models.Grid.Align.left, true);
                }
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Motorista, "Motorista", 18, Models.Grid.Align.left, false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Transportador, "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.TipoDeCarga, "TipoCarga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataNota, "DataNotaFiscalDescricao", 10, Models.Grid.Align.left, false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Destinatario, "Destinatario", 20, Models.Grid.Align.left, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.CNPJDestinatario, "CNPJDestinatarioFormatado", 20, Models.Grid.Align.right, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Protocolo, "NumeroProtocolo", 5, Models.Grid.Align.left, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Filial, "Filial", 15, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.SituacaoCanhotoFisico, "DescricaoSituacao", 13, Models.Grid.Align.center, false);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Digitalizacao, "DescricaoDigitalizacao", 10, Models.Grid.Align.center, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.NumeroCarga, "NumeroCarga", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataBaixa, "DataEnvioCanhotoDescricao", 10, Models.Grid.Align.center, false);
            }

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataDigitalizacao, "DataDigitalizacaoDescricao", 10, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("NomeArquivo", false);

            if (ConfiguracaoEmbarcador.UtilizaPgtoCanhoto && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.SituacaoPagamento, "DescricaoSituacaoPgtoCanhoto", 10, Models.Grid.Align.left, false);

            if (ConfiguracaoCanhoto.PermitirRetornarStatusCanhotoNaAPIDigitalizacao)
                grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.CanhotodisponivelConsultaAPI, "DigitalizacaoIntegradaDescricao", 10, Models.Grid.Align.center, false);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.DataDeEntregaNoCliente, "DataEntregaClienteDescricao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.OrigemDigitalizacao, "DescricaoOrigemSituacaoDigitalizacaoCanhoto", 13, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.SituacaoNotaFiscal, "DescricaoSituacaoNotaFiscal", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.QuantidadeEnvios, "QuantidadeEnvioDigitalizacaoCanhotoFormatado", 10, Models.Grid.Align.center, false);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.CentroResultadoCarga, "CentroResultadoCarga", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Origem, "Origem", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Destino, "Destino", 10, Models.Grid.Align.center, true, false);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.EscritorioVendas, "EscritorioVendasComplementar", 10, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.Matriz, "MatrizComplementar", 10, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.TipoNotaFiscalIntegrada, "DescricaoTipoNotaFiscalIntegrada", 10, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.CanhotoValidadoIA, "DigitalizacaoCanhotoValidadoIA", 10, Models.Grid.Align.center, false);

            grid.AdicionarCabecalho(Localization.Resources.Canhotos.Canhoto.SituacaoIA, "DescricaoSituacaoIA", 13, Models.Grid.Align.center, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

            double emitente = Request.GetDoubleParam("Emitente");
            List<int> codigoCargaEmbarcador = Request.GetListParam<int>("CodigoCargaEmbarcador");
            List<int> codigosNumero = Request.GetListParam<int>("Numeros");
            int numero = Request.GetIntParam("Numero");

            List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa> listaDocumentos = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa>("ListaCTes");
            List<int> codigosConhecimentos = RetornaCodigosConhecimentos(listaDocumentos, unitOfWork);
            List<int> codigosCanhotos = RetornaCodigosCanhotos(listaDocumentos, unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                emitente = this.Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0;

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            codigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            codigosFilial.AddRange(ObterListaCodigoFilialPermitidasOperadorCanhoto(unitOfWork));

            List<int> codigosTiposOperacao = Request.GetListParam<int>("TipoOperacao");
            codigosTiposOperacao.AddRange(ObterListaCodigoTipoOperacaoPermitidosOperadorCanhoto(unitOfWork));

            List<int> codigosTiposCarga = Request.GetListParam<int>("TipoCarga");
            codigosTiposCarga.AddRange(ObterListaCodigoTipoCargaPermitidosOperadorCanhoto(unitOfWork));

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtroPesquisaCanhoto = new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto
            {
                CodigosConhecimentos = codigosConhecimentos,
                CodigosCanhotos = codigosCanhotos,
                TipoCanhoto = Request.GetEnumParam<TipoCanhoto>("TipoCanhoto"),
                TipoNotaFiscalIntegrada = Request.GetEnumParam<TipoNotaFiscalIntegrada>("TipoNotaFiscalIntegrada"),
                Situacoes = Request.GetListEnumParam<SituacaoCanhoto>("SituacaoCanhoto"),
                SituacoesDigitalizacaoCanhoto = Request.GetNullableListParam<SituacaoDigitalizacaoCanhoto>("SituacoesDigitalizacaoCanhoto"),
                CodigosCargaEmbarcador = codigoCargaEmbarcador,
                Carga = Request.GetIntParam("Carga"),
                Motorista = Request.GetIntParam("Motorista"),
                NumeroNFe = Request.GetIntParam("NumeroNFe"),
                NumeroCanhoto = Request.GetIntParam("NumeroCanhoto"),
                Pessoa = emitente,
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                Chave = Request.GetStringParam("Chave"),
                Numeros = codigosNumero,
                Numero = numero,
                Filiais = codigosFilial,
                Terceiro = Request.GetDoubleParam("Terceiro"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                DataInicioDigitalizacao = Request.GetNullableDateTimeParam("DataInicioDigitalizacao"),
                DataFimDigitalizacao = Request.GetNullableDateTimeParam("DataFimDigitalizacao"),
                Serie = Request.GetIntParam("Serie"),
                BaixarCanhotoAposAprovacaoDigitalizacao = ConfiguracaoEmbarcador.BaixarCanhotoAposAprovacaoDigitalizacao,
                CodigoCTe = Request.GetIntParam("CTe"),
                SituacaoPgtoCanhoto = Request.GetEnumParam<SituacaoPgtoCanhoto>("SituacaoPgtoCanhoto"),
                TiposOperacao = codigosTiposOperacao,
                TiposCarga = codigosTiposCarga,
                Destinatario = Request.GetListParam<double>("Destinatario").Count > 0 ? Request.GetListParam<double>("Destinatario") : Request.GetDoubleParam("Destinatario") > 0 ? new List<double>() { Request.GetDoubleParam("Destinatario") } : new List<double>(),
                TipoLocalPrestacao = Request.GetEnumParam<TipoLocalPrestacao>("TipoLocalPrestacao"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculo"),
                Usuario = Request.GetIntParam("Operador"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                SituacoesCarga = Request.GetListEnumParam<SituacaoCarga>("SituacaoCarga"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                NumeroDocumentoOriginario = Request.GetStringParam("NumeroDocumentoOriginario"),
                SituacoesNotaFiscal = Request.GetListEnumParam<SituacaoNotaFiscal>("SituacaoNotaFiscal"),
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                ExibirCanhotosSemVinculoComCarga = configuracaoCanhoto?.ExibirCanhotosSemVinculoComCarga ?? false,
                ClienteComplementar = Request.GetListParam<double>("ClienteComplementar"),
                DigitalizacaoIntegrada = Request.GetNullableBoolParam("DigitalizacaoIntegrada"),
                EscritorioVendas = Request.GetStringParam("EscritorioVendas"),
                Matriz = Request.GetStringParam("Matriz"),
                ValidacaoCanhoto = Request.GetNullableBoolParam("ValidacaoCanhoto"),
                TipoSituacaoIA = Request.GetNullableEnumParam<SituacaoIntegracao>("TipoSituacaoIA"),
                TipoRejeicaoPelaIA = Request.GetListEnumParam<TipoRejeicaoPelaIA>("TipoRejeicaoPelaIA"),
            };

            filtroPesquisaCanhoto.Empresas = (Usuario?.Empresa?.Codigo ?? 0) > 0 && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? repositorioEmpresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato) : Request.GetListParam<int>("Empresa");

            return filtroPesquisaCanhoto;
        }

        private Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhotoParaVinculo ObterFiltrosPesquisaParaVinculo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhotoParaVinculo()
            {
                CodigoEntrega = Request.GetIntParam("Entrega"),
                Numero = Request.GetIntParam("Numero")
            };
        }

        private string ObterPropriedadeOrdenarParaVinculo(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoTipoCanhoto")
                return "TipoCanhoto";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "SituacaoCanhoto";

            if (propriedadeOrdenar == "Empresa")
                return "Empresa.RazaoSocial";

            return propriedadeOrdenar;
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> ExecutaPesquisa(ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega repositorioConfiguracaoEntregaQualidade = new Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega = repositorioConfiguracaoEntregaQualidade.BuscarConfiguracaoPadraoAsync().Result;

            Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            totalRegistros = repCanhoto.ContarConsultaCanhotoSQLDynamic(filtrosPesquisa, configuracaoQualidadeEntrega);
            IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> listaGrid = (totalRegistros > 0) ? repCanhoto.ConsultarCanhotoSQLDynamic(filtrosPesquisa, parametrosConsulta, false, configuracaoQualidadeEntrega) : new List<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos>();

            return listaGrid;
        }


        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilha()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Canhotos.Canhoto.ChaveDoDocumento, Propriedade = "ChaveDocumento", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Canhotos.Canhoto.NumeroNota, Propriedade = "NumeroNota", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Canhotos.Canhoto.SerieDaNFe, Propriedade = "SerieNFe", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Canhotos.Canhoto.CNPJEmitente, Propriedade = "CNPJEmitente", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "CNPJ do Transportador", Propriedade = "CNPJTransportador", Tamanho = 100, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilhaDescarte()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Canhotos.Canhoto.ChaveDoCanhoto, Propriedade = "ChaveCanhoto", Tamanho = 100, Obrigatorio = false },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Canhotos.Canhoto.NumeroDaNFe, Propriedade = "Numero", Tamanho = 100, Obrigatorio = false },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Canhotos.Canhoto.SerieDaNFe, Propriedade = "Serie", Tamanho = 100, Obrigatorio = false },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Canhotos.Canhoto.CNPJEmitente, Propriedade = "CNPJEmitente", Tamanho = 100, Obrigatorio = false }
            };

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoCanhotos()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Canhotos.Canhoto.ChaveNFe, Propriedade = "ChaveNFe", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Canhotos.Canhoto.NumeroCanhotoImportacao, Propriedade = "Numero", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Canhotos.Canhoto.NumeroCarga, Propriedade = "NumeroCarga", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Canhotos.Canhoto.CNPJDestinatario, Propriedade = "CNPJDestinatario", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Canhotos.Canhoto.Transportador, Propriedade = "CNPJTransportador", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Canhotos.Canhoto.Filial, Propriedade = "Filial", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
            };

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoAtualizarCanhotos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork).BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            if (configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoPorImportacao)
            {
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número da NF", Propriedade = "NumeroNFe", Tamanho = 100, Obrigatorio = true });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Série da NF", Propriedade = "SerieNFe", Tamanho = 100, Obrigatorio = true });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Chave da NF", Propriedade = "ChaveNFe", Tamanho = 100, Obrigatorio = true });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "CNPJ Emitente", Propriedade = "CNPJEmitente", Tamanho = 100, Obrigatorio = true });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Situação da Nota", Propriedade = "SituacaoNota", Tamanho = 100, Obrigatorio = true });
            }
            if (configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoAvulsoPorImportacao)
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Número Canhoto Avulso", Propriedade = "NumeroCanhotoAvulso", Tamanho = 100, Obrigatorio = true });

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilhaDataEntrega()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Canhotos.Canhoto.NumeroSerie, Propriedade = "NumeroSerie", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Canhotos.Canhoto.DataEntregaAoCliente, Propriedade = "DataEntrega", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Canhotos.Canhoto.CNPJEmitente, Propriedade = "CNPJEmitente", Tamanho = 100, Obrigatorio = false }
            };

            return configuracoes;
        }

        private List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosSelecionados(Repositorio.UnitOfWork unitOfWork, int quantidadeValidar, out bool quantidadeValida, out int quantidadeSelecionada)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            bool todosSelecionados = Request.GetBoolParam("SelecionarTodos");

            if (todosSelecionados)
            {
                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro = ObterFiltrosPesquisa(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa> listaDocumentos = null;

                if (!string.IsNullOrWhiteSpace(Request.Params("ListaCTes")))
                    listaDocumentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa>>(Request.Params("ListaCTes"));

                filtro.CodigosConhecimentos = RetornaCodigosConhecimentos(listaDocumentos, unitOfWork);
                filtro.CodigosCanhotos = RetornaCodigosCanhotos(listaDocumentos, unitOfWork);

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.Consultar(filtro);

                dynamic listaCanhotosNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CanhotosNaoSelecionados"));
                foreach (dynamic dybCanhotoNaoSelecionado in listaCanhotosNaoSelecionados)
                    canhotos.Remove(new Dominio.Entidades.Embarcador.Canhotos.Canhoto() { Codigo = (int)dybCanhotoNaoSelecionado.Codigo });

                quantidadeSelecionada = canhotos.Count();

                if (quantidadeValidar > 0 && quantidadeSelecionada > quantidadeValidar)
                {
                    Servicos.Log.TratarErro($"{Usuario.Nome} - " + Localization.Resources.Canhotos.Canhoto.QuantidadeCanhotosInvalida + $": {quantidadeSelecionada}.", "Canhoto");

                    quantidadeValida = false;
                    return null;
                }

                quantidadeValida = true;

                return canhotos;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
                dynamic listaCanhotosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CanhotosSelecionados"));
                foreach (dynamic dynCanhotoSelecionado in listaCanhotosSelecionados)
                {
                    canhotos.Add(repCanhoto.BuscarPorCodigo((int)dynCanhotoSelecionado.Codigo));
                }

                quantidadeSelecionada = canhotos.Count;
                quantidadeValida = true;

                return canhotos;
            }


        }

        private void EnviarJustificarivaCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            string observacao = Request.Params("Observacao");
            canhoto.DataEnvioCanhoto = DateTime.Now;
            canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado;
            canhoto.Observacao = observacao;
            canhoto.DataUltimaModificacao = DateTime.Now;
            Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
            repCargaCanhoto.Atualizar(canhoto);


            serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.CanhotoJustificado, unitOfWork);
        }

        private string VerificarSeNaoEsgotouCapacidadeLocalArmazenamentoMultiplaSelecao(List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            if (canhotos.Any(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe) && (canhotos.Any(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)))
                return Localization.Resources.Canhotos.Canhoto.ParaFazerEnvioDeMultiplosCanhotosSelecioneApenasOsCanhotosDoMesmoTipoNFeOuAvulso;

            int quantidadeSelecionada = canhotos.Count;
            Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();
            else
                localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(canhotos.FirstOrDefault().TipoCanhoto);

            if (localArmazenamentoCanhoto != null)
            {
                int quantidadeRestante = localArmazenamentoCanhoto.CapacidadeArmazenagem - localArmazenamentoCanhoto.QuantidadeArmazenada;
                if (quantidadeSelecionada < quantidadeRestante)
                {
                    if (localArmazenamentoCanhoto.DividirEmPacotesDe > 0)
                    {
                        int posicaoPacote = repCargaCanhoto.BuscarProximaPosicaoPacote(localArmazenamentoCanhoto.Codigo, localArmazenamentoCanhoto.PacoteAtual);
                        quantidadeRestante = localArmazenamentoCanhoto.DividirEmPacotesDe - (posicaoPacote - 1);
                        if (quantidadeSelecionada > quantidadeRestante)
                            retorno = string.Format(Localization.Resources.Canhotos.Canhoto.VoceSelecionouCanhotosOqueExcedeCapacidadeRestanteDoPacoteDeCanhotosNoAtualQueDeRestantesPorFavorNaoInformeUmaQuantidadeSuperiorRestantePorVez, quantidadeSelecionada, quantidadeRestante);
                    }
                }
                else
                {
                    retorno = string.Format(Localization.Resources.Canhotos.Canhoto.VoceSelecionouCanhotosOqueExcedeCapacidadeRestanteDeArmazenamentoNoAtualLocalDosCanhotosQueDeRestantesPorFavorNaoInformeUmaQuantidadeSuperiorRestantePorVer, quantidadeSelecionada, quantidadeRestante);
                }
            }
            return retorno;
        }

        private dynamic ConfirmarEnvioFisicoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario = null, string observacaoOperador = null)
        {
            string mensagem = "";
            bool pendencia = false;
            dynamic localArmazenamentAtual = null;

            Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            string mensagemLimite = "";

            if (ConfiguracaoEmbarcador.PermiteBaixarCanhotoApenasComOcorrenciaEntrega && canhoto.CargaCTe?.CTe != null && !canhoto.CargaCTe.CTe.Ocorrencias.Any(o => o.Ocorrencia.EntregaRealizada))
            {
                return new
                {
                    Pendencia = pendencia,
                    Mensagem = Localization.Resources.Canhotos.Canhoto.NecessarioQueCanhotoTenhaUmaOcorrenciaDeEntregaRealizadaParaQueSejaPermitidaBaixaDoMesmo
                };
            }

            if (canhoto.LocalArmazenamentoCanhoto == null)
            {
                Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual();
                else
                    localArmazenamentoCanhoto = repLocalArmazenamentoCanhoto.BuscarLocalArmanemantoAtual(canhoto.TipoCanhoto);

                if (localArmazenamentoCanhoto != null)
                {
                    if (localArmazenamentoCanhoto.CapacidadeArmazenagem > localArmazenamentoCanhoto.QuantidadeArmazenada)
                    {
                        localArmazenamentoCanhoto.QuantidadeArmazenada++;
                        canhoto.LocalArmazenamentoCanhoto = localArmazenamentoCanhoto;

                        if (localArmazenamentoCanhoto.DividirEmPacotesDe > 0)
                        {
                            int posicaoPacote = repCargaCanhoto.BuscarProximaPosicaoPacote(localArmazenamentoCanhoto.Codigo, localArmazenamentoCanhoto.PacoteAtual);
                            if (posicaoPacote > localArmazenamentoCanhoto.DividirEmPacotesDe)//caso a quantidade aumente após o fechamento do local de armazenmento a posição não foi incrementada antecipadamente, por isso se incrementa aqui 
                            {
                                localArmazenamentoCanhoto.PacoteAtual++;
                                posicaoPacote = 1;
                            }

                            canhoto.PosicaoNoPacote = posicaoPacote;
                            canhoto.PacoteArmazenado = localArmazenamentoCanhoto.PacoteAtual;

                            if (posicaoPacote == localArmazenamentoCanhoto.DividirEmPacotesDe)
                            {
                                if (localArmazenamentoCanhoto.QuantidadeArmazenada < localArmazenamentoCanhoto.CapacidadeArmazenagem)
                                {
                                    localArmazenamentoCanhoto.PacoteAtual++;
                                    mensagemLimite = string.Format(Localization.Resources.Canhotos.Canhoto.PacoteDeCanhotosDeNumeroJaEstaComCanhotosArmazeneOsProximosCanhotosEmUmNovoPacoteComNumero, (localArmazenamentoCanhoto.PacoteAtual - 1), localArmazenamentoCanhoto.DividirEmPacotesDe, localArmazenamentoCanhoto.PacoteAtual);
                                }
                            }

                            if (localArmazenamentoCanhoto.QuantidadeArmazenada == localArmazenamentoCanhoto.CapacidadeArmazenagem)
                                mensagemLimite = string.Format(Localization.Resources.Canhotos.Canhoto.LocalDeArmazenamentoDeCanhotosAcabouDeFicarCheioCrieUmNovoLocalDeArmazenamentoParaArmazenarNovosCanhotos, localArmazenamentoCanhoto.Descricao);

                        }
                        repLocalArmazenamentoCanhoto.Atualizar(localArmazenamentoCanhoto);

                        localArmazenamentAtual = serCanhoto.RetornarDadosLocalArmazenamento(localArmazenamentoCanhoto);
                    }
                    else
                    {
                        mensagem = string.Format(Localization.Resources.Canhotos.Canhoto.NaoPossivelEnviarCanhotosParaLocalDeArmazenamentoPoisSeuLimitedeArmazenamentoCanhotosFisicosFoiAtingidoPorFavorCadastreUmNovoLocalDeArmazenamentoTenteNovamente, localArmazenamentoCanhoto.Descricao, localArmazenamentoCanhoto.CapacidadeArmazenagem);
                        pendencia = true;
                    }
                }
            }

            bool exigeCanhotoParaFaturamento = false;

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                canhoto.Usuario = usuario;
                canhoto.DataEnvioCanhoto = DateTime.Now;
                canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente;
                canhoto.DataUltimaModificacao = DateTime.Now;
                repCargaCanhoto.Atualizar(canhoto);

                if (canhoto.Emitente?.GrupoPessoas?.ExigeCanhotoFisico ?? false)
                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);

                serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.CanhotoFisicoEntreguePeloMotorista, unitOfWork, observacaoOperador);

                exigeCanhotoParaFaturamento = VerificarSeExigeCanhotoParaFaturamento(canhoto);
            }

            if (!string.IsNullOrWhiteSpace(mensagemLimite))
            {
                mensagem = mensagemLimite;
            }

            var retorno = new
            {
                Pendencia = pendencia,
                Mensagem = mensagem,
                LocalArmazenamentAtual = localArmazenamentAtual,
                ExigeCanhotoParaFaturamento = exigeCanhotoParaFaturamento
            };

            return retorno;
        }

        private bool VerificarSeExigeCanhotoParaFaturamento(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            Dominio.Entidades.Cliente tomador = null;

            if (canhoto.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                tomador = canhoto.Destinatario;
            else
                tomador = canhoto.Emitente;
            if (canhoto.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                return (canhoto.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.NaoGerarFaturaAteReceberCanhotos ?? false);
            else if (tomador.NaoUsarConfiguracaoFaturaGrupo)
                return (tomador.NaoGerarFaturaAteReceberCanhotos ?? false);
            else if (tomador.GrupoPessoas != null)
                return (tomador.GrupoPessoas.NaoGerarFaturaAteReceberCanhotos ?? false);

            return false;
        }

        private string retornarMotoristas(List<Dominio.Entidades.Usuario> motoristas)
        {
            string strMotoristas = "";

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                strMotoristas += motorista.Nome + ". ";
            }
            return strMotoristas;
        }

        private void excluirArquivoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
            string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
            Servicos.Log.TratarErro($"Caminho do canhoto {canhoto.Codigo} a ser deletado: {fileLocation}", "Canhoto");

            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
            {
                try
                {
                    Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                }
                catch (IOException ex)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    try
                    {
                        Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro($"Erro ao deletar arquivo canhoto {fileLocation}: {ex2.Message}");
                        throw;
                    }
                }
            }
        }

        private string retornarCaminhoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            return Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
        }

        private string retornarCorPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto situacao)
        {
            string cor = "";

            switch (situacao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Todas:
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente:
                    cor = "";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado:
                    cor = "#fcf8e3";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente:
                    cor = "#dff0d8";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Extraviado:
                    cor = "#031634";
                    break;
                default:
                    break;
            }
            return cor;
        }

        private string retornarCorFonte(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto situacao)
        {
            string cor = "";

            switch (situacao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Extraviado:
                    cor = "#FFF";
                    break;
                default:
                    break;
            }
            return cor;
        }

        private List<int> RetornaCodigosConhecimentos(List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa> listaDocumentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();

            if (listaDocumentos != null)
                listaCodigos = (from obj in listaDocumentos where obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.CTe select obj.Codigo).ToList();

            return listaCodigos;
        }

        private List<int> RetornaCodigosCanhotos(List<Dominio.ObjetosDeValor.Embarcador.Canhoto.CTeFiltroPesquisa> listaDocumentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();

            if (listaDocumentos != null)
                listaCodigos = (from obj in listaDocumentos where obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFiltroPesquisa.NFe select obj.Codigo).ToList();

            return listaCodigos;
        }

        private List<int> ObterCodigosCTesFinalizarEnvioCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unidadeTrabalho);

            List<int> ctes = new List<int>();

            if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeTrabalho);

                //ctes = canhoto.XMLNotaFiscal.CTEs.Distinct().ToList();
                ctes = repXMLNotaFiscal.BuscarCodigosCTesPorCodigo(canhoto.XMLNotaFiscal.Codigo);
            }
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao)
            {
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unidadeTrabalho);

                ctes = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarCodigosCTesPorCTeTerceiro(canhoto.CTeSubcontratacao.Codigo);

                //List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCteParaSubcontratacaoPedidoNotasFiscais = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCTeTerceiro(canhoto.CTeSubcontratacao.Codigo);

                //ctes = pedidoCteParaSubcontratacaoPedidoNotasFiscais.SelectMany(o => o.PedidoXMLNotaFiscal.CTes.Select(c => c.CargaCTe.CTe)).Distinct().ToList();
            }
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe)
                ctes = new List<int>() { canhoto.CargaCTe.CTe.Codigo };

            return ctes;
        }

        private void FinalizarEnvioCanhotosCTe(List<int> codigosCTes, Repositorio.UnitOfWork unitOfWork)
        {
            codigosCTes = codigosCTes.Distinct().ToList();

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            foreach (int codigoCTe in codigosCTes)
            {
                bool existeCanhotoPendentePorCTe = repCanhoto.VerificarSeExisteCanhotoCTePendentePorCTe(codigoCTe) || repCanhoto.VerificarSeExisteCanhotoNotaFiscalPendentePorCTe(codigoCTe);
                bool todosOsCanhotosEntreguesPeloMotorista = !repCanhoto.VerificarSeExisteCanhotoCTeNaoEntreguePeloMotoristaPorCTe(codigoCTe) && !repCanhoto.VerificarSeExisteCanhotoNotaFiscalNaoEntreguePeloMotoristaPorCTe(codigoCTe);

                if (!existeCanhotoPendentePorCTe || todosOsCanhotosEntreguesPeloMotorista)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.BuscarTodosPorCTe(codigoCTe);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                    {
                        if (!existeCanhotoPendentePorCTe)
                        {
                            DateTime dataEnvioUltimoCanhoto = repCanhoto.ObterUltimaDataEnvioCanhotoPorCTe(codigoCTe);

                            if (dataEnvioUltimoCanhoto != DateTime.MinValue)
                                documentoFaturamento.DataEnvioUltimoCanhoto = dataEnvioUltimoCanhoto;

                            documentoFaturamento.CanhotosRecebidos = true;
                        }

                        if (todosOsCanhotosEntreguesPeloMotorista && !documentoFaturamento.DataChegadaUltimoCanhoto.HasValue)
                            documentoFaturamento.DataChegadaUltimoCanhoto = DateTime.Now;

                        repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    }
                }
            }
        }

        private void FinalizarEnvioCanhotosCTe(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unidadeTrabalho);

            List<int> ctes;

            if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeTrabalho);

                //ctes = canhoto.XMLNotaFiscal.CTEs.Distinct().ToList();
                ctes = repXMLNotaFiscal.BuscarCodigosCTesPorCodigo(canhoto.XMLNotaFiscal.Codigo);
            }
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao)
            {
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unidadeTrabalho);

                ctes = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarCodigosCTesPorCTeTerceiro(canhoto.CTeSubcontratacao.Codigo);

                //List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCteParaSubcontratacaoPedidoNotasFiscais = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCTeTerceiro(canhoto.CTeSubcontratacao.Codigo);

                //ctes = pedidoCteParaSubcontratacaoPedidoNotasFiscais.SelectMany(o => o.PedidoXMLNotaFiscal.CTes.Select(c => c.CargaCTe.CTe)).Distinct().ToList();
            }
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe)
                ctes = new List<int>() { canhoto.CargaCTe.CTe.Codigo };
            else
                return;

            foreach (int codigoCTe in ctes)
            {
                bool existeCanhotoPendentePorCTe = repCanhoto.VerificarSeExisteCanhotoCTePendentePorCTe(codigoCTe) || repCanhoto.VerificarSeExisteCanhotoNotaFiscalPendentePorCTe(codigoCTe);
                bool todosOsCanhotosEntreguesPeloMotorista = !repCanhoto.VerificarSeExisteCanhotoCTeNaoEntreguePeloMotoristaPorCTe(codigoCTe) && !repCanhoto.VerificarSeExisteCanhotoNotaFiscalNaoEntreguePeloMotoristaPorCTe(codigoCTe);

                if (!existeCanhotoPendentePorCTe || todosOsCanhotosEntreguesPeloMotorista)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.BuscarTodosPorCTe(codigoCTe);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                    {
                        if (!existeCanhotoPendentePorCTe)
                        {
                            documentoFaturamento.DataEnvioUltimoCanhoto = repCanhoto.ObterUltimaDataEnvioCanhotoPorCTe(codigoCTe);
                            documentoFaturamento.CanhotosRecebidos = true;
                        }

                        if (todosOsCanhotosEntreguesPeloMotorista && !documentoFaturamento.DataChegadaUltimoCanhoto.HasValue)
                            documentoFaturamento.DataChegadaUltimoCanhoto = DateTime.Now;

                        repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    }
                }
            }
        }

        private void ProcessarCanhotoComImagem(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, string guidNomeArquivo, string nomeArquivo, DateTime? dataEnvioCanhoto, DateTime? dataEntregaNotaCliente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, byte[] arquivo = null)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configuracaoIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork).BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(canhoto.Destinatario.CPF_CNPJ);
            bool possuiFlagDoisOuMaisPaginas = clienteDescarga?.PossuiCanhotoDeDuasOuMaisPaginas ?? false;

            canhoto.Initialize();
            canhoto.GuidNomeArquivo = guidNomeArquivo;
            canhoto.NomeArquivo = nomeArquivo;
            canhoto.DataEnvioCanhoto = dataEnvioCanhoto.HasValue ? dataEnvioCanhoto.Value : DateTime.Now;
            canhoto.QuantidadeEnvioDigitalizacaoCanhoto = (canhoto.QuantidadeEnvioDigitalizacaoCanhoto > 0 ? canhoto.QuantidadeEnvioDigitalizacaoCanhoto + 1 : 1);
            canhoto.OrigemSituacaoDigitalizacaoCanhoto = null;

            string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);
            string caminhoCanhoto = Utilidades.IO.FileStorageService.Storage.Combine(caminho + nomeArquivo);

            var extensao = System.IO.Path.GetExtension(nomeArquivo).ToLower();
            string caminhoCanhotoDisco = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidNomeArquivo + extensao);

            if (string.IsNullOrWhiteSpace(caminhoCanhoto) || !Utilidades.IO.FileStorageService.Storage.Exists(caminhoCanhotoDisco))
                throw new ServicoException(Localization.Resources.Canhotos.Canhoto.ArquivoCanhotoNaoEncontrado);

            Servicos.Log.TratarErro($"Caminho do canhoto {canhoto.Codigo} enviado: {caminhoCanhotoDisco}", "Canhoto");

            if (dataEntregaNotaCliente.HasValue)
            {
                if (dataEntregaNotaCliente.Value > DateTime.Now)
                    throw new ServicoException(Localization.Resources.Canhotos.Canhoto.DataDeEntregaNaoPodeSerMaiorQueDataAtual);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                    xmlNotaFiscal = canhoto.XMLNotaFiscal;
                else if (canhoto.TipoCanhoto == TipoCanhoto.Avulso && canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Count > 0)
                    xmlNotaFiscal = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.FirstOrDefault().XMLNotaFiscal;

                DateTime? dataEmissaoNf = xmlNotaFiscal?.DataEmissao;

                if (dataEmissaoNf.HasValue && dataEmissaoNf.Value.Date > dataEntregaNotaCliente.Value)
                    throw new ServicoException(Localization.Resources.Canhotos.Canhoto.DataDeEntregaNaoPodeSerInferiorDataDeEmissaoDaNotaFiscal);

                canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente;
            }

            if ((canhoto.Carga?.TipoOperacao?.ConfiguracaoCanhoto?.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado ?? false) && servicoCanhoto.CanhotoPossuiCTeNaoAutorizado(canhoto))
                throw new ServicoException(string.Format(Localization.Resources.Canhotos.Canhoto.CanhotoNaoPossuiCTeAutorizado, canhoto?.XMLNotaFiscal?.Numero.ToString()));

            canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;
            canhoto.DataDigitalizacao = DateTime.Now;
            DateTime? dataFinalizacaoEntrega = canhoto.DataEntregaNotaCliente ?? canhoto.DataDigitalizacao;

            bool naoIntegrarCanhotoComNotaDevolvida = (configuracaoCanhoto.NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas && canhoto.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida);
            bool possuiIntegracaoComprovei = (configuracaoIntegracaoComprovei?.PossuiIntegracaoIACanhoto ?? false) &&
                configuracaoCanhoto.IntegrarCanhotosComValidadorIAComprovei &&
                (!canhoto.XMLNotaFiscal?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.PossuiCanhotoDeDuasOuMaisPaginas ?? true) &&
                !naoIntegrarCanhotoComNotaDevolvida;

            //MultiEmbarcador ou MultiTMS e
            //Não exige aprovação de digitalização do canhoto e
            //MultiTMS, ou possui integração com a IA da Comprovei, ou conseguiu obter o número do canhoto via OCR.
            if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                 TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) &&
                (!(configuracaoEmbarcador?.ExigeAprovacaoDigitalizacaoCanhoto ?? false)) &&
                !(possuiFlagDoisOuMaisPaginas || canhoto.TipoCanhoto == TipoCanhoto.Avulso) &&
                (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ||
                 possuiIntegracaoComprovei ||
                 ValidadNumeroCanhotoExiste(canhoto, unitOfWork, arquivo, caminhoCanhoto)))
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                canhoto.DigitalizacaoIntegrada = false;
                canhoto.UsuarioDigitalizacao = this.Usuario;

                Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, unitOfWork, possuiFlagDoisOuMaisPaginas);
                Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);

                //Tratamento para quando o canhoto permanecer na situação de digitalizado.
                if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
                    canhoto.OrigemSituacaoDigitalizacaoCanhoto = OrigemSituacaoDigitalizacaoCanhoto.OCR;

                if (configuracaoEmbarcador?.ConfirmarEntregaDigitilizacaoCanhoto ?? false)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal = null;
                    if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                        XMLNotaFiscal = canhoto.XMLNotaFiscal;
                    else if (canhoto.TipoCanhoto == TipoCanhoto.Avulso && canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Count > 0)
                        XMLNotaFiscal = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.FirstOrDefault().XMLNotaFiscal;

                    if (XMLNotaFiscal != null && canhoto.Carga != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repCargaEntregaNotaFiscal.BuscarPorCargaENFe(canhoto.Carga.Codigo, XMLNotaFiscal.Codigo);
                        if (cargaEntregaNotaFiscal != null)
                        {
                            cargaEntregaNotaFiscal.CargaEntrega.Initialize();
                            if (cargaEntregaNotaFiscal.CargaEntrega.Situacao == SituacaoEntrega.NaoEntregue)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(canhoto.Latitude, canhoto.Longitude);

                                OrigemSituacaoEntrega origemSituacaoEntrega = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                    OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;

                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntregaNotaFiscal.CargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);
                                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntregaNotaFiscal.CargaEntrega, dataFinalizacaoEntrega.Value, wayPoint, null, 0, "", configuracaoEmbarcador, TipoServicoMultisoftware, Auditado, origemSituacaoEntrega, this.Cliente, unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntregaNotaFiscal.CargaEntrega, cargaEntregaNotaFiscal.CargaEntrega.GetChanges(), "Entrega confirmada via Upload Canhoto", unitOfWork);
                            }
                            else if (ConfiguracaoEmbarcador.ExigirDataEntregaNotaClienteCanhotos)
                            {
                                cargaEntregaNotaFiscal.CargaEntrega.DataConfirmacao = dataEntregaNotaCliente;
                                if (cargaEntregaNotaFiscal.CargaEntrega.DataFim < dataEntregaNotaCliente)
                                    cargaEntregaNotaFiscal.CargaEntrega.DataFim = dataEntregaNotaCliente;
                                if (cargaEntregaNotaFiscal.CargaEntrega.DataInicio > dataEntregaNotaCliente)
                                    cargaEntregaNotaFiscal.CargaEntrega.DataInicio = dataEntregaNotaCliente;
                                repCargaEntrega.Atualizar(cargaEntregaNotaFiscal.CargaEntrega);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntregaNotaFiscal.CargaEntrega, cargaEntregaNotaFiscal.CargaEntrega.GetChanges(), "Alterou a data de confirmação via Upload Canhoto", unitOfWork);

                                new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, null).ProcessarRegrasDeQualidadeDeEntrega(cargaEntregaNotaFiscal.CargaEntrega);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!(configuracaoEmbarcador?.ExigeAprovacaoDigitalizacaoCanhoto ?? false) && !(possuiFlagDoisOuMaisPaginas || canhoto.TipoCanhoto == TipoCanhoto.Avulso || naoIntegrarCanhotoComNotaDevolvida))
                {
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                    canhoto.DigitalizacaoIntegrada = false;
                    canhoto.UsuarioDigitalizacao = this.Usuario;

                    Canhoto.CanhotoLiberado(canhoto, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                    CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, unitOfWork, possuiFlagDoisOuMaisPaginas);
                    Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);

                    //Tratamento para quando o canhoto permanecer na situação de digitalizado.
                    if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
                        canhoto.OrigemSituacaoDigitalizacaoCanhoto = OrigemSituacaoDigitalizacaoCanhoto.OCR;
                }
                else
                {
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracaoEmbarcador, unitOfWork);
                }
            }

            canhoto.DataUltimaModificacao = DateTime.Now;
            canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Portal;
            repositorioCargaCanhoto.Atualizar(canhoto);

            servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.ImagemDoCanhotoEnviadaDigitalizada, unitOfWork);

            if (dataEntregaNotaCliente.HasValue)
                servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.DataEntregaAoCliente + " " + dataEntregaNotaCliente.Value.ToString("dd/MM/yyyy HH:mm"), unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (canhoto.Carga != null)
                {
                    Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorEnvioDeImagemDoCanhoto;
                    int numero = servicoCanhoto.VerificarQuantidadeCanhotosPendenteAceiteImagem(canhoto.Carga, TipoServicoMultisoftware, unitOfWork, Auditado);

                    if (numero == 0)
                    {
                        Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                        servicoCarga.SolicitarEncerramentoCarga(canhoto.Carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorAceiteDasImagensDosCanhotosDaCarga, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                    }
                }
            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.EnviouImagemDoCanhotoManualmente, unitOfWork);
        }

        private void SalvarImagemCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, ref byte[] arquivo, ref string guidNomeArquivo, ref string nomeArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            if (canhoto == null)
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.CanhotoNaoFoiSelecionado);

            Servicos.DTO.CustomFile file = HttpContext.GetFile();
            string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
            nomeArquivo = System.IO.Path.GetFileName(file.FileName);

            if (!extensao.Equals(".jpg") && !extensao.Equals(".jpeg") && !extensao.Equals(".png") && !extensao.Equals(".pdf"))
                throw new ControllerException(Localization.Resources.Gerais.Geral.ExtensaoDoArquivoInvalida);

            excluirArquivoCanhoto(canhoto, unitOfWork);

            guidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
            string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);

            if (extensao.Equals(".pdf"))
            {
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidNomeArquivo + extensao);
                Servicos.Log.TratarErro($"Caminho do canhoto {canhoto.Codigo} a ser salvo: {fileLocation}", "Canhoto");

                using (Stream arquivoPDF = file.InputStream)
                {
                    using (System.IO.Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(fileLocation))
                    {
                        arquivoPDF.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                using (System.Drawing.Image t = System.Drawing.Image.FromStream(HttpContext.GetFile().InputStream))
                {
                    using (System.Drawing.Image novaImagem = Utilidades.Image.RedimensionarImagem(t, new Size(1300, 1300)))
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidNomeArquivo + extensao);
                        Servicos.Log.TratarErro($"Caminho do canhoto {canhoto.Codigo} a ser salvo: {fileLocation}", "Canhoto");

                        Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, novaImagem);

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            novaImagem.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arquivo = memoryStream.ToArray();
                        }
                    }
                }
            }
        }

        private void SalvarImagensCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, ref byte[] arquivo, ref string guidNomeArquivo, ref string nomeArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);

            guidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
            nomeArquivo = guidNomeArquivo + ".pdf";
            List<string> imagensBase64 = new();
            List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
            for (int i = 0; i < files.Count; i++)
            {
                Servicos.DTO.CustomFile file = files[i];
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!extensao.Equals(".jpg") && !extensao.Equals(".jpeg") && !extensao.Equals(".png") && !extensao.Equals(".pdf"))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.ExtensaoDoArquivoInvalida);

                using (System.Drawing.Image t = System.Drawing.Image.FromStream(file.InputStream))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        t.Save(ms, t.RawFormat);
                        imagensBase64.Add(System.Convert.ToBase64String(ms.ToArray()));
                    }
                }
            }

            if (imagensBase64.Count > 0)
            {
                excluirArquivoCanhoto(canhoto, unitOfWork);

                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo);
                Servicos.PDF.ConvertBase64ImagesToPdf(imagensBase64, fileLocation);
                //arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
            }
        }

        private string SalvarImagemCanhotoTemporario(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.DTO.CustomFile file = HttpContext.GetFile();
            string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();

            if (!extensao.Equals(".jpg") && !extensao.Equals(".jpeg") && !extensao.Equals(".png") && !extensao.Equals(".pdf"))
                throw new ControllerException(Localization.Resources.Gerais.Geral.ExtensaoDoArquivoInvalida);

            string guidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
            string caminho = ObterCaminhoArquivoTemporario(unitOfWork);

            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidNomeArquivo + extensao);

            if (extensao.Equals(".jpg") || extensao.Equals(".jpeg") || extensao.Equals(".png"))
            {
                using (var stream = HttpContext.GetFile().InputStream)
                {
                    using (var t = System.Drawing.Image.FromStream(stream))
                    {
                        using (var novaImagem = Utilidades.Image.RedimensionarImagem(t, new Size(1300, 1300)))
                        {
                            try
                            {
                                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, novaImagem);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Não foi possível gravar a imagem em '{fileLocation}'.", ex);
                            }
                        }
                    }
                }
            }
            else if (extensao.Equals(".pdf"))
            {
                using (var stream = HttpContext.GetFile().InputStream)
                {
                    try
                    {
                        using (var fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(fileLocation))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Não foi possível gravar o arquivo PDF em '{fileLocation}'.", ex);
                    }
                }
            }

            return guidNomeArquivo;
        }

        private void CopiarImagemTemporariaProCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, string tokenImagem, ref string nomeArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            if (canhoto == null)
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.CanhotoNaoFoiSelecionado);

            excluirArquivoCanhoto(canhoto, unitOfWork);

            string arquivo = ObterArquivoTemporario(tokenImagem, unitOfWork);
            if (string.IsNullOrWhiteSpace(arquivo))
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.ImagemEnviadaNaoFoiLocalizada);

            string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);

            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tokenImagem + Path.GetExtension(arquivo));
            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                Utilidades.IO.FileStorageService.Storage.Copy(arquivo, fileLocation);

            nomeArquivo = Path.GetFileName(arquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelCopiarImagem);
        }

        private void ExcluirImagemCanhotoTemporario(string tokenImagem, Repositorio.UnitOfWork unitOfWork)
        {
            string arquivo = ObterArquivoTemporario(tokenImagem, unitOfWork);

            if (!string.IsNullOrWhiteSpace(arquivo))
                Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
        }

        private string ObterArquivoTemporario(string tokenImagem, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoTemp = ObterCaminhoArquivoTemporario(unitOfWork);

            return Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoTemp, tokenImagem + ".*").FirstOrDefault();
        }

        private string ObterCaminhoArquivoTemporario(Repositorio.UnitOfWork unitOfWork)
        {
            return Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
        }

        private FileContentResult ObterArquivoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            if (canhoto == null)
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoExisteUmCanhotoEnviadoParaEsteDocumento);

            string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
            string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
            string caminhoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelBaixarCanhotoPoisEleNaoFoiLocalizado);

            byte[] bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

            if (bufferCanhoto == null)
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelBaixarCanhotoAtualizePaginaTenteNovamente);

            return Arquivo(bufferCanhoto, "application/jpg", System.IO.Path.GetFileName(caminhoOriginal));
        }

        private FileContentResult ObterArquivoCanhotoEsperandoVinculo(Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo, Repositorio.UnitOfWork unitOfWork)
        {
            if (canhotoEsperandoVinculo == null)
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoExisteUmCanhotoEnviadoParaEsteDocumento);

            Servicos.Embarcador.Canhotos.CanhotoEsperandoVinculo servicoCanhotoEsperandoVinculo = new Servicos.Embarcador.Canhotos.CanhotoEsperandoVinculo(unitOfWork);
            string caminhoCanhotoEsperandoVinculo = servicoCanhotoEsperandoVinculo.ObterCaminhoCanhotosEsperandoVinculo();
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotoEsperandoVinculo, canhotoEsperandoVinculo.NomeArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelBaixarCanhotoPoisEleNaoFoiLocalizado);

            byte[] bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

            if (bufferCanhoto == null)
                throw new ControllerException(Localization.Resources.Canhotos.Canhoto.NaoFoiPossivelBaixarCanhotoAtualizePaginaTenteNovamente);

            return Arquivo(bufferCanhoto, "application/jpg", canhotoEsperandoVinculo.NomeArquivo);
        }

        private bool PermitirReverterCanhoto(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto situacaoCanhoto, DateTime dataDigitalizacaoCanhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioConfiguracaoCanhoto = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

            DateTime dataAtual = DateTime.Now;

            if (!(situacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado))
                return false;

            if (dataDigitalizacaoCanhoto == DateTime.MinValue)
                return false;

            int diasDiferencia = (int)dataAtual.Subtract(dataDigitalizacaoCanhoto).TotalDays;

            if (configuracaoCanhoto.PrazoParaReverterDigitalizacaoAposAprovacao > 0 && diasDiferencia > configuracaoCanhoto.PrazoParaReverterDigitalizacaoAposAprovacao)
                return false;

            return true;
        }

        private string ObterCorLinhaCanhotoInteiro(SituacaoDigitalizacaoCanhoto situacaoDigitalizacaoCanhoto)
        {
            switch (situacaoDigitalizacaoCanhoto)
            {
                case SituacaoDigitalizacaoCanhoto.Digitalizado:
                    return CorGrid.Success;

                case SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada:
                    return CorGrid.Danger;

                default:
                    return CorGrid.Branco;
            }
        }

        private void PreecherRegistroDeAuditoria(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto situacaoAditoria, Repositorio.UnitOfWork unitOfWork)
        {
            canhoto.DescricaoSituacaoAuditoria = Request.GetStringParam("Descricao");
            canhoto.SituacaoAuditoriaCanhoto = situacaoAditoria;

            if (canhoto.SituacaoAuditoriaCanhoto == Dominio.ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto.Aprovado)
                canhoto.DataAprovacaoRejeicaoAuditoria = DateTime.Now;

            if (canhoto.SituacaoAuditoriaCanhoto == Dominio.ObjetosDeValor.Enumerador.SituacaoAuditoriaCanhoto.Reprovado)
            {
                Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria repositoriaMotivoRejeicao = new Repositorio.Embarcador.Canhotos.MotivoRejeicaoAuditoria(unitOfWork);
                int codigoMotivoRejeicao = Request.GetIntParam("MotivoRejeicao");
                canhoto.MotivoRejeicaoAuditoria = codigoMotivoRejeicao != null ? repositoriaMotivoRejeicao.BuscarPorCodigo(codigoMotivoRejeicao) : null;
                canhoto.DataRejeicaoAuditoria = DateTime.Now;
            }
        }

        private bool ValidadNumeroCanhotoExiste(Dominio.Entidades.Embarcador.Canhotos.Canhoto NumeroDoCanhoto, Repositorio.UnitOfWork unitOfWork, byte[] arquivo = null, string RutaArquivo = "")
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repconfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            bool canhotoUnilever = repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracao = repconfiguracaoCanhoto.BuscarConfiguracaoPadrao();
            string apiLink = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRLink;
            string apiKey = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRKey;

            LeitorOCR servicoLeitorOcr = new LeitorOCR(unitOfWork);
            servicoLeitorOcr.DefinirAPI(apiLink, apiKey);

            Servicos.Log.TratarErro($"Chegou a imagem com valor byte[{arquivo?.Length ?? 0}] Chave Api [{apiKey}] Url[{apiLink}]", "Canhoto");
            bool canhotoAvulso;
            string numeroObtidos = servicoLeitorOcr.ObterNumeroDocumento(RutaArquivo, canhotoUnilever, out canhotoAvulso, unitOfWork, arquivo);

            string[] numeroObtidosList = numeroObtidos.Split(',');
            for (int i = 0; i < numeroObtidosList.Length; i++)
            {
                if (numeroObtidosList[i].Trim().ToInt() != NumeroDoCanhoto.Numero)
                    continue;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, NumeroDoCanhoto, null, "Canhoto validado automaticamente  pelo leitor OCR", unitOfWork);
                return true;
            }

            if (configuracao != null && configuracao.RejeitarCanhotosNaoValidadosPeloOCR)
                throw new ControllerException("Imagem de canhoto não validado");

            return false;
        }

        private void AceitarEnvio(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork, out string mensagemValidacao, string observacaoOperador = null)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

            Repositorio.Embarcador.Canhotos.Canhoto repCargaCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                mensagemValidacao = Localization.Resources.Canhotos.Canhoto.AcaoInvalida;
                return;
            }

            if (!(configuracaoCanhoto?.PermitirAprovarDigitalizacaoDeCanhotoRejeitado ?? false) && canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
            {
                mensagemValidacao = Localization.Resources.Canhotos.Canhoto.NaoPermitidoInformarAceitarImagemDaDigitalizacaoSuaAtualSituacao + " (" + canhoto.DescricaoSituacao + ").";
                return;
            }

            canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado;
            canhoto.DataAprovacaoDigitalizacao = DateTime.Now;
            canhoto.DigitalizacaoIntegrada = false;
            canhoto.SituacaoPgtoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Pendente;
            canhoto.UsuarioDigitalizacao = this.Usuario;

            Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, unitOfWork, possuiFlagDoisOuMaisPaginas: false, aceitarEnvio: true);
            Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, this.Cliente);
            Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);

            canhoto.MotivoRejeicaoDigitalizacao = "";
            canhoto.DataUltimaModificacao = DateTime.Now;

            repCargaCanhoto.Atualizar(canhoto);

            serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.DigitalizacaoDoCanhotoAprovada, unitOfWork, observacaoOperador);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (canhoto.Carga != null)
                {
                    Auditado.Texto = Localization.Resources.Canhotos.Canhoto.EncerradoPorAceiteDoCanhoto;
                    int numero = serCanhoto.VerificarQuantidadeCanhotosPendenteAceiteImagem(canhoto.Carga, TipoServicoMultisoftware, unitOfWork, Auditado);
                    if (numero == 0)
                        serCarga.SolicitarEncerramentoCarga(canhoto.Carga.Codigo, Localization.Resources.Canhotos.Canhoto.EncerramentoAutomaticoPorAceiteDasImagensDosCanhotosDaCarga, WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);
                }
            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, Localization.Resources.Canhotos.Canhoto.AceitouDeImagemDoCanhoto, unitOfWork);

            mensagemValidacao = string.Empty;
        }

        private bool ValidarDocumentoNaPasta(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
            string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();

            string caminho = retornarCaminhoCanhoto(canhoto, unitOfWork);
            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
            {
                servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.ImagemDoCanhotoDigitalizadaEValidada, unitOfWork);
                return true;
            }
            else
            {
                servicoCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, Localization.Resources.Canhotos.Canhoto.ImagemDoCanhotoDigitalizadaNaoValidada, unitOfWork);
                return false;
            }

        }
    }

    #endregion
}

