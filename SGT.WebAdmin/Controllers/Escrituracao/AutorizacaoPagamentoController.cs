using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Escrituracao/AutorizacaoPagamento")]
    public class AutorizacaoPagamentoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento,
        Dominio.Entidades.Embarcador.Escrituracao.Pagamento
    >
    {
		#region Construtores

		public AutorizacaoPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> ReprocessarPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigoPagamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repositorioPagamento.BuscarPorCodigo(codigoPagamento);

                if (pagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pagamento.Situacao != SituacaoPagamento.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Servicos.Embarcador.Escrituracao.PagamentoAprovacao servicoPagamentoAprovacao = new Servicos.Embarcador.Escrituracao.PagamentoAprovacao(unitOfWork, ClienteAcesso?.URLAcesso);

                servicoPagamentoAprovacao.CriarAprovacao(pagamento, TipoServicoMultisoftware);
                repositorioPagamento.Atualizar(pagamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = pagamento.Situacao != SituacaoPagamento.SemRegraAprovacao });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar os pagamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplosPagamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosPagamento = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> listaPagamento = repositorioPagamento.BuscarSemRegraAprovacaoPagamentoPorCodigos(codigosPagamento);
                Servicos.Embarcador.Escrituracao.PagamentoAprovacao servicoPagamentoAprovacao = new Servicos.Embarcador.Escrituracao.PagamentoAprovacao(unitOfWork, ClienteAcesso?.URLAcesso);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in listaPagamento)
                {
                    servicoPagamentoAprovacao.CriarAprovacao(pagamento, TipoServicoMultisoftware);

                    if (pagamento.Situacao != SituacaoPagamento.SemRegraAprovacao)
                    {
                        repositorioPagamento.Atualizar(pagamento);
                        totalRegrasReprocessadas++;
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegrasReprocessadas = totalRegrasReprocessadas });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar os pagamentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.Pagamento repositorio = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repositorio.BuscarPorCodigo(codigo);

                if (pagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaDocumentos = repositorioDocumentoFaturamento.BuscarPorPagamento(pagamento.Codigo, configuracaoEmbarcador.GerarSomenteDocumentosDesbloqueados);
                List<int> codigosLancamentosNFSManual = (from o in listaDocumentos where o.CargaPagamento == null && o.LancamentoNFSManual != null select o.LancamentoNFSManual.Codigo).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPagamento = (from o in listaDocumentos where o.CargaPagamento != null select o.CargaPagamento).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasLancamentoNFSManual = repositorioCargaDocumentoParaEmissaoNFSManual.BuscarCargasPorListaLancamento(codigosLancamentosNFSManual);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = cargasPagamento.Union(cargasLancamentoNFSManual).Distinct().OrderBy(o => o.CodigoCargaEmbarcador.ToLong()).ThenBy(o => o.CodigoCargaEmbarcador).ToList();
                List<int> codigosCargas = (from o in cargas select o.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCargas(codigosCargas);
                List<dynamic> documentosPorCarga = new List<dynamic>();
                decimal valorIcmsPagamento = 0m;

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    List<dynamic> destinatariosPorCarga = new List<dynamic>();
                    decimal valorFrete = 0m;
                    decimal valorFreteSemIcms = 0m;

                    List<Dominio.Entidades.Cliente> destinatariosDocumentosPorCarga = (
                        from o in listaDocumentos
                        where (
                            (
                                (o.CargaPagamento != null && o.CargaPagamento.Codigo == carga.Codigo) ||
                                (o.CargaPagamento == null && o.LancamentoNFSManual != null && o.LancamentoNFSManual.Documentos.Any(d => d.Carga.Codigo == carga.Codigo))
                            ) &&
                            o.CTe.Destinatario != null &&
                            o.CTe.Destinatario.Cliente != null
                        )
                        select o.CTe.Destinatario.Cliente
                    ).Distinct().ToList();

                    foreach (Dominio.Entidades.Cliente destinatario in destinatariosDocumentosPorCarga)
                    {
                        List<dynamic> documentosPorDestinatario = new List<dynamic>();

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorDestinatario = (
                            from o in cargaPedidos
                            where o.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ
                            select o
                        ).ToList();

                        List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaDocumentosPorCargaEDestinatario = (
                            from o in listaDocumentos
                            where (
                                (
                                    (o.CargaPagamento != null && o.CargaPagamento.Codigo == carga.Codigo) ||
                                    (o.CargaPagamento == null && o.LancamentoNFSManual != null && o.LancamentoNFSManual.Documentos.Any(d => d.Carga.Codigo == carga.Codigo))
                                ) && o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ
                            )
                            select o
                        ).ToList();

                        foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoPorDestinatario in listaDocumentosPorCargaEDestinatario)
                        {
                            documentosPorDestinatario.Add(new
                            {
                                Documento = $"{documentoPorDestinatario.Numero} - {(documentoPorDestinatario.EmpresaSerie?.Numero.ToString() ?? "")}",
                                Tipo = documentoPorDestinatario.ModeloDocumentoFiscal?.Abreviacao ?? "",
                                Ocorrencia = documentoPorDestinatario.CargaOcorrenciaPagamento?.NumeroOcorrencia.ToString() ?? "",
                                TipoOcorrencia = documentoPorDestinatario.CargaOcorrenciaPagamento?.TipoOcorrencia?.Descricao ?? "",
                                ValorFrete = documentoPorDestinatario.ValorDocumento.ToString("n2"),
                                ValorFreteSemIcms = (documentoPorDestinatario.ValorDocumento - documentoPorDestinatario.ValorICMS).ToString("n2"),
                                Origem = documentoPorDestinatario.CTe.LocalidadeInicioPrestacao?.Descricao ?? "",
                                Destino = documentoPorDestinatario.CTe.LocalidadeTerminoPrestacao?.Descricao ?? "",
                                FreteEmergencial = documentoPorDestinatario.Carga?.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada ? "Sim" : "Não"
                            });

                            valorFrete += documentoPorDestinatario.ValorDocumento;
                            valorFreteSemIcms += (documentoPorDestinatario.ValorDocumento - documentoPorDestinatario.ValorICMS);

                            valorIcmsPagamento += documentoPorDestinatario.ValorICMS;

                        }

                        decimal pesoLiquidoCargaPorDestinatario = cargaPedidosPorDestinatario.Sum(o => o.PesoLiquido);
                        decimal pesoLiquidoCargaPorDestinatarioEmTonelada = pesoLiquidoCargaPorDestinatario / 1000;
                        decimal valorFreteClientePorDestinatario = cargaPedidosPorDestinatario.Sum(o => o.ValorFreteFilialEmissora);
                        decimal valorFreteClienteSemIcmsPorDestinatario = (valorFreteClientePorDestinatario - cargaPedidosPorDestinatario.Sum(o => o.ValorICMSFilialEmissora));
                        decimal valorFreteClientePorDestinatarioETonelada = pesoLiquidoCargaPorDestinatarioEmTonelada > 0m ? Math.Round(valorFreteClientePorDestinatario / pesoLiquidoCargaPorDestinatarioEmTonelada, 2, MidpointRounding.AwayFromZero) : 0m;

                        destinatariosPorCarga.Add(new
                        {
                            CpfCnpjDestinatario = destinatario.CPF_CNPJ,
                            DescricaoDestinatario = destinatario.Descricao,
                            ValorFreteCliente = valorFreteClientePorDestinatario.ToString("n2"),
                            ValorFreteClienteSemIcms = valorFreteClienteSemIcmsPorDestinatario.ToString("n2"),
                            ValorFreteClientePorTonelada = valorFreteClientePorDestinatarioETonelada.ToString("n2"),
                            ListaDocumentos = documentosPorDestinatario
                        });
                    }

                    decimal capacidadeCarregamento = carga.ModeloVeicularCarga != null ? carga.ModeloVeicularCarga.CapacidadePesoTransporte + carga.ModeloVeicularCarga.ToleranciaPesoExtra : 0m;
                    decimal pesoCarga = carga.DadosSumarizados?.PesoTotal ?? 0m;
                    decimal pesoLiquidoCarga = carga.DadosSumarizados?.PesoLiquidoTotal ?? 0m;
                    decimal pesoLiquidoCargaEmTonelada = pesoLiquidoCarga / 1000;
                    decimal percentualOcupacao = capacidadeCarregamento > 0m ? (pesoCarga * 100) / capacidadeCarregamento : 0m;
                    decimal valorFretePorTonelada = pesoLiquidoCargaEmTonelada > 0m ? Math.Round(valorFrete / pesoLiquidoCargaEmTonelada, 2, MidpointRounding.AwayFromZero) : 0m;

                    documentosPorCarga.Add(new
                    {
                        CodigoCarga = carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        Peso = pesoLiquidoCarga.ToString("n2"),
                        ValorFrete = valorFrete.ToString("n2"),
                        ValorFreteSemIcms = valorFreteSemIcms.ToString("n2"),
                        ValorFretePorTonelada = valorFretePorTonelada.ToString("n2"),
                        PercentualOcupacao = percentualOcupacao > 0m ? percentualOcupacao.ToString("n2") : "",
                        Destinatarios = destinatariosPorCarga
                    });
                }

                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    NumeroPagamento = pagamento.Numero,
                    CodigoCargaEmbarcador = pagamento.Carga?.CodigoCargaEmbarcador ?? "",
                    Filial = pagamento.Filial?.Descricao,
                    SituacaoDescricao = pagamento.Situacao.ObterDescricao(),
                    pagamento.Situacao,
                    Transportador = pagamento.Empresa?.Descricao,
                    ValorPagamento = pagamento.ValorPagamento.ToString("n2"),
                    ValorPagamentoSemIcms = (pagamento.ValorPagamento - valorIcmsPagamento).ToString("n2"),
                    ListaDocumentosPorCarga = documentosPorCarga
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                SituacaoPagamento = Request.GetNullableEnumParam<SituacaoPagamento>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "Transportador")
                return "Empresa.RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Escrituracao.Pagamento origem)
        {
            return origem.Situacao == SituacaoPagamento.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repositorioAprovacaoAlcada = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(unitOfWork);

                pagamentos = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    pagamentos.Remove(new Dominio.Entidades.Embarcador.Escrituracao.Pagamento() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                pagamentos = new List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    pagamentos.Add(repositorioPagamento.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return (from pagamento in pagamentos select pagamento.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(descricao: "Número do Pagamento", propriedade: "NumeroPagamento", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Situação do Pagamento", propriedade: "SituacaoPagamento", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Valor do Pagamento", propriedade: "ValorPagamento", tamanho: 10, alinhamento: Models.Grid.Align.right);
                grid.AdicionarCabecalho(descricao: "Número da Carga", propriedade: "NumeroCarga", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Peso da Carga", propriedade: "PesoCarga", tamanho: 10, alinhamento: Models.Grid.Align.right);
                grid.AdicionarCabecalho(descricao: "Peso Líquido da Carga", propriedade: "PesoLiquidoCarga", tamanho: 10, alinhamento: Models.Grid.Align.right);
                grid.AdicionarCabecalho(descricao: "Valor do Frete da Carga", propriedade: "ValorFreteCarga", tamanho: 10, alinhamento: Models.Grid.Align.right);
                grid.AdicionarCabecalho(descricao: "Valor do Frete da Carga por Tonelada", propriedade: "ValorFreteCargaPorTonelada", tamanho: 10, alinhamento: Models.Grid.Align.right);
                grid.AdicionarCabecalho(descricao: "Percentual de Ocupação da Carga", propriedade: "PercentualOcupacaoCarga", tamanho: 10, alinhamento: Models.Grid.Align.right);
                grid.AdicionarCabecalho(descricao: "CPF/CNPJ do Destinatário", propriedade: "CpfCnpjDestinatario", tamanho: 10, alinhamento: Models.Grid.Align.center).UtilizarFormatoTexto(true);
                grid.AdicionarCabecalho(descricao: "Destinatário", propriedade: "DescricaoDestinatario", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Número do Documento", propriedade: "Documento", tamanho: 10, alinhamento: Models.Grid.Align.center).UtilizarFormatoTexto(true);
                grid.AdicionarCabecalho(descricao: "Tipo do Documento", propriedade: "TipoDocumento", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Origem", propriedade: "Origem", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Destino", propriedade: "Destino", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Ocorrência", propriedade: "Ocorrencia", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Tipo da Ocorrência", propriedade: "TipoOcorrencia", tamanho: 20, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Valor do Documento", propriedade: "ValorDocumento", tamanho: 10, alinhamento: Models.Grid.Align.right);

                Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repositorioAprovacaoPagamento = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { DirecaoOrdenar = "asc", PropriedadeOrdenar = "Numero" };
                int totalPagamentos = repositorioAprovacaoPagamento.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = totalPagamentos > 0 ? repositorioAprovacaoPagamento.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
                List<int> codigosPagamento = (from o in pagamentos select o.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaDocumentos = repositorioDocumentoFaturamento.BuscarPorPagamentos(codigosPagamento, configuracaoEmbarcador.GerarSomenteDocumentosDesbloqueados);
                List<int> codigosLancamentosNFSManual = (from o in listaDocumentos where o.CargaPagamento == null && o.LancamentoNFSManual != null select o.LancamentoNFSManual.Codigo).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> lancamentosNFSManual = repositorioCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentosNFsManual(codigosLancamentosNFSManual);
                List<dynamic> pagamentosretornar = new List<dynamic>();
                int totalPagamentosRetornar = 0;

                foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentos)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaDocumentosPorPagamento = (
                        from o in listaDocumentos
                        where (
                            (pagamento.LotePagamentoLiberado && o.PagamentoLiberacao.Codigo == pagamento.Codigo) ||
                            (!pagamento.LotePagamentoLiberado && o.Pagamento.Codigo == pagamento.Codigo)
                        )
                        select o
                    ).Distinct().ToList();

                    List<int> codigosLancamentosNFSManualPorPagamento = (from o in listaDocumentosPorPagamento where o.CargaPagamento == null && o.LancamentoNFSManual != null select o.LancamentoNFSManual.Codigo).Distinct().ToList();
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPagamento = (from o in listaDocumentosPorPagamento where o.CargaPagamento != null select o.CargaPagamento).Distinct().ToList();
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasLancamentoNFSManual = (from o in lancamentosNFSManual where codigosLancamentosNFSManualPorPagamento.Contains(o.Codigo) select o.Carga).ToList();
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = cargasPagamento.Union(cargasLancamentoNFSManual).Distinct().OrderBy(o => o.CodigoCargaEmbarcador.ToLong()).ThenBy(o => o.CodigoCargaEmbarcador).ToList();

                    if (cargas.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        {
                            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaDocumentosPorPagamentoECarga = (
                                from o in listaDocumentosPorPagamento
                                where (
                                    (o.CargaPagamento != null && o.CargaPagamento.Codigo == carga.Codigo) ||
                                    (o.CargaPagamento == null && o.LancamentoNFSManual != null && o.LancamentoNFSManual.Documentos.Any(d => d.Carga.Codigo == carga.Codigo))
                                )
                                select o
                            ).Distinct().ToList();

                            List<Dominio.Entidades.Cliente> destinatariosDocumentosPorPagamentoECarga = (
                                from o in listaDocumentosPorPagamentoECarga
                                where o.CTe.Destinatario.Cliente != null
                                select o.CTe.Destinatario.Cliente
                            ).Distinct().ToList();

                            decimal capacidadeCarregamento = carga.ModeloVeicularCarga != null ? carga.ModeloVeicularCarga.CapacidadePesoTransporte + carga.ModeloVeicularCarga.ToleranciaPesoExtra : 0m;
                            decimal pesoCarga = carga.DadosSumarizados?.PesoTotal ?? 0m;
                            decimal pesoLiquidoCarga = carga.DadosSumarizados?.PesoLiquidoTotal ?? 0m;
                            decimal pesoLiquidoCargaEmTonelada = pesoLiquidoCarga / 1000;
                            decimal percentualOcupacaoCarga = capacidadeCarregamento > 0m ? (pesoCarga * 100) / capacidadeCarregamento : 0m;
                            decimal valorFreteCarga = listaDocumentosPorPagamentoECarga.Where(o => o.CTe.Destinatario.Cliente != null).Sum(o => o.ValorDocumento);
                            decimal valorFreteCargaPorTonelada = pesoLiquidoCargaEmTonelada > 0m ? Math.Round(valorFreteCarga / pesoLiquidoCargaEmTonelada, 2, MidpointRounding.AwayFromZero) : 0m;

                            foreach (Dominio.Entidades.Cliente destinatario in destinatariosDocumentosPorPagamentoECarga)
                            {
                                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaDocumentosPorPagamentoCargaEDestinatario = (
                                    from o in listaDocumentosPorPagamentoECarga
                                    where o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ
                                    select o
                                ).ToList();

                                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoPorDestinatario in listaDocumentosPorPagamentoCargaEDestinatario)
                                {
                                    pagamentosretornar.Add(new
                                    {
                                        NumeroPagamento = pagamento.Numero,
                                        SituacaoPagamento = pagamento.Situacao.ObterDescricao(),
                                        ValorPagamento = pagamento.ValorPagamento.ToString("n2"),
                                        NumeroCarga = carga.CodigoCargaEmbarcador,
                                        PesoCarga = pesoCarga.ToString("n2"),
                                        PesoLiquidoCarga = pesoLiquidoCarga.ToString("n2"),
                                        ValorFreteCarga = valorFreteCarga.ToString("n2"),
                                        ValorFreteCargaPorTonelada = valorFreteCargaPorTonelada.ToString("n2"),
                                        PercentualOcupacaoCarga = percentualOcupacaoCarga > 0m ? percentualOcupacaoCarga.ToString("n2") : "",
                                        CpfCnpjDestinatario = destinatario.CPF_CNPJ,
                                        DescricaoDestinatario = destinatario.Descricao,
                                        Documento = $"{documentoPorDestinatario.Numero} - {(documentoPorDestinatario.EmpresaSerie?.Numero.ToString() ?? "")}",
                                        Destino = documentoPorDestinatario.CTe.LocalidadeTerminoPrestacao?.Descricao ?? "",
                                        Ocorrencia = documentoPorDestinatario.CargaOcorrenciaPagamento?.NumeroOcorrencia.ToString() ?? "",
                                        Origem = documentoPorDestinatario.CTe.LocalidadeInicioPrestacao?.Descricao ?? "",
                                        TipoDocumento = documentoPorDestinatario.ModeloDocumentoFiscal?.Abreviacao ?? "",
                                        TipoOcorrencia = documentoPorDestinatario.CargaOcorrenciaPagamento?.TipoOcorrencia?.Descricao ?? "",
                                        ValorDocumento = documentoPorDestinatario.ValorDocumento.ToString("n2")
                                    });

                                    totalPagamentosRetornar++;
                                }
                            }

                        }
                    }
                    else
                    {
                        pagamentosretornar.Add(new
                        {
                            NumeroPagamento = pagamento.Numero,
                            SituacaoPagamento = pagamento.Situacao.ObterDescricao(),
                            ValorPagamento = pagamento.ValorPagamento.ToString("n2"),
                            NumeroCarga = "",
                            PesoCarga = "",
                            PesoLiquidoCarga = "",
                            ValorFreteCarga = "",
                            ValorFreteCargaPorTonelada = "",
                            PercentualOcupacaoCarga = "",
                            CpfCnpjDestinatario = "",
                            DescricaoDestinatario = "",
                            Documento = "",
                            Destino = "",
                            Ocorrencia = "",
                            Origem = "",
                            TipoDocumento = "",
                            TipoOcorrencia = "",
                            ValorDocumento = ""
                        });

                        totalPagamentosRetornar++;
                    }
                }

                grid.AdicionaRows(pagamentosretornar);
                grid.setarQuantidadeTotal(totalPagamentosRetornar);

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

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número do Pagamento", propriedade: "Numero", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Número da Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(descricao: "Empresa/Filial", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                else
                {
                    grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                }

                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "AutorizacaoPagamento/Pesquisa", "grid-autorizacao-pagamentos");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repositorio = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();

                var lista = (
                    from pagamento in pagamentos
                    select new
                    {
                        pagamento.Codigo,
                        pagamento.Numero,
                        pagamento.Carga?.CodigoCargaEmbarcador,
                        Filial = pagamento.Filial?.Descricao,
                        Situacao = pagamento.Situacao.ObterDescricao(),
                        Transportador = pagamento.Empresa?.RazaoSocial
                    }
                ).ToList();

                grid.AdicionaRows(lista);
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

        protected override void PreencherDadosRejeicaoAprovacao(Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento aprovacao, Repositorio.UnitOfWork unitOfWork)
        {
            base.PreencherDadosRejeicaoAprovacao(aprovacao, unitOfWork);

            aprovacao.OrigemAprovacao.MotivoRejeicaoFechamentoPagamento = aprovacao.Motivo;
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Escrituracao.Pagamento origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao != SituacaoPagamento.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Servicos.Embarcador.Escrituracao.PagamentoAprovacao servicoPagamentoAprovacao = new Servicos.Embarcador.Escrituracao.PagamentoAprovacao(unitOfWork, ClienteAcesso?.URLAcesso);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoPagamentoAprovacao.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Pagamento aprovado", unitOfWork);
                Servicos.Embarcador.Escrituracao.Pagamento.FinalizarPagamento(origem, unitOfWork);
            }
            else
            {
                origem.Situacao = SituacaoPagamento.Reprovado;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Pagamento reprovado", unitOfWork);
            }

            repositorioPagamento.Atualizar(origem);
        }

        #endregion
    }
}