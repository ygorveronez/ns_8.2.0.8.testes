using Dominio.Entidades;
using Dominio.Entidades.Embarcador.Chamados;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize(new string[] { "BuscarDatasMotivoChamado", "BuscarCargaEntrega", "ObterSobrasAnalise" }, "Chamados/ChamadoOcorrencia", "Cargas/ControleEntrega", "Cargas/GestaoDevolucao")]
    public class ChamadoOcorrenciaController : BaseController
    {
        #region Construtores

        public ChamadoOcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ProcessarXmlNotaFiscalDevolucao()
        {
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.FalhaAoEnviarOArquivo);

                Servicos.DTO.CustomFile file = files[0];

                string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (extensao != ".xml")
                    return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.AExtensaoDoArquivoEInvalida);

                var objetoNFe = MultiSoftware.NFe.Servicos.Leitura.Ler(file.InputStream);
                if (objetoNFe == null)
                    return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OXMLInformadoNaoEUmaNFePorFavorVerifique);

                return new JsonpResult(new
                {
                    Chave = ((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)objetoNFe).protNFe.infProt.chNFe,
                    Numero = ((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)objetoNFe).NFe.infNFe.ide.nNF,
                    Serie = ((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)objetoNFe).NFe.infNFe.ide.serie,
                    DataEmissao = DateTime.Parse(((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)objetoNFe).NFe.infNFe.ide.dhEmi).ToString("dd/MM/yyyy"),
                    ValorTotalProdutos = ((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)objetoNFe).NFe.infNFe.total.ICMSTot.vProd,
                    ValorTotal = ((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)objetoNFe).NFe.infNFe.total.ICMSTot.vNF,
                    PesoDevolvido = ((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)objetoNFe).NFe.infNFe.transp.vol[0].pesoB
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.NaoFoiPossivelProcessarOXML);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(await ObterGridPesquisa(0));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaChamado()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaChamado());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTipoOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado;

                int.TryParse(Request.Params("MotivoChamado"), out int codigoMotivoChamado);
                int.TryParse(Request.Params("Chamado"), out int codigoChamado);

                int codigoCarga = 0;
                string codigoCargaEmbarcador = string.Empty;

                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivo = repMotivoChamado.BuscarPorCodigo(codigoMotivoChamado);
                if (motivo == null)
                    motivo = repMotivoChamado.BuscarPorChamado(codigoChamado);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = motivo?.TipoOcorrencia;


                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && configuracaoChamado.PermitirGerarAtendimentoPorPedido)
                {
                    chamado = VincularCargaNoAtendimentoPorPedido(unitOfWork, codigoChamado);

                    if (chamado != null)
                    {
                        codigoCarga = chamado.Carga.Codigo;
                        codigoCargaEmbarcador = chamado.Carga.CodigoCargaEmbarcador;
                    }
                }

                var retorno = new
                {
                    Codigo = tipoOcorrencia?.Codigo ?? 0,
                    Descricao = tipoOcorrencia?.Descricao ?? "",
                    OrigemOcorrencia = tipoOcorrencia?.OrigemOcorrencia ?? OrigemOcorrencia.PorCarga,
                    ComponenteCodigo = tipoOcorrencia?.ComponenteFrete?.Codigo ?? 0,
                    ComponenteDescricao = tipoOcorrencia?.ComponenteFrete?.Descricao ?? "",
                    CodigoCarga = codigoCarga,
                    CodigoCargaEmbarcador = codigoCargaEmbarcador,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoObterDetalhesDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Chamado.Chamado serChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Chamados.MotivoChamadoArvore repChamadoArvore = new Repositorio.Embarcador.Chamados.MotivoChamadoArvore(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Chamados.NivelAtendimento repNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repMotivoChamadoGatilhos = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(unitOfWork, cancellationToken);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork, cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = await repChamado.BuscarPorCodigoAsync(codigo);
                if (chamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoArvore> arvoreDisponivel = await repChamadoArvore.BuscarPorCodigoMotivoChamadoAsync(chamado.MotivoChamado.Codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise ultimaAnalise = await repChamadoAnalise.BuscarUltimaAnalisePorCodigoAsync(chamado.Codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repConfiguracao.BuscarConfiguracaoPadraoAsync();

                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analiseLiberacaoValor = await repChamadoAnalise.BuscarAprovacaoValorChamadoPorCodigoAsync(chamado.Codigo);

                bool possuiIntegracao = await repChamadoIntegracao.PossuiIntegracaoAsync(codigo);
                bool ultimaRespostaEmbarcador = false;
                Dominio.Entidades.Embarcador.Chamados.NivelAtendimento nivelAtendimento = await repNivelAtendimento.BuscarSeChamadoComNivelAtualAsync(chamado.Codigo, chamado?.Nivel ?? EscalationList.Nivel1);
                bool superouTempoLimiteDoNivel = false;

                if (nivelAtendimento != null)
                    superouTempoLimiteDoNivel = DateTime.Compare(DateTime.Now, (nivelAtendimento.DataLimite ?? DateTime.MinValue)) > 0;

                if (ultimaAnalise?.Autor != null && chamado.Responsavel != null)
                    ultimaRespostaEmbarcador = ultimaAnalise.Autor.Codigo == chamado.Responsavel.Codigo;

                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList gatilhoMotivoChamadoSetor = await repMotivoChamadoGatilhos.BuscarNivelPorMotivoChamadoAsync(chamado.MotivoChamado.Codigo, chamado?.Nivel ?? EscalationList.Nivel1);

                Usuario motoristaPrincipal = chamado.Carga?.Motoristas?.FirstOrDefault() ?? chamado.CargaEntrega?.Carga?.Motoristas?.FirstOrDefault() ?? null;

                if ((!chamado?.Carga?.Motoristas?.Any() ?? false) && motoristaPrincipal != null)
                    chamado.Carga?.Motoristas?.Add(motoristaPrincipal);

                List<Usuario> listaDeResponsaveisPorNivelFilial = new List<Usuario>();
                List<Usuario> listaUsuarioPorTransportador = await repUsuario.BuscarPorCodigosAsync(chamado?.Carga?.Empresa?.Operadores?.Select(o => o.Codigo).ToList() ?? new List<int>());
                List<Usuario> listaUsuarioResponsavelChamado = await repUsuario.BuscarUsuarioResposavelChamadoAsync(chamado.Cliente?.CPF_CNPJ ?? 0, chamado?.Nivel ?? EscalationList.SemNivel);

                if (listaUsuarioPorTransportador.Count > 0)
                    listaDeResponsaveisPorNivelFilial = listaUsuarioPorTransportador;
                else if (listaUsuarioResponsavelChamado.Count > 0)
                    listaDeResponsaveisPorNivelFilial = listaUsuarioResponsavelChamado;
                else
                    listaDeResponsaveisPorNivelFilial = await repUsuario.BuscarUsuarioResposavelChamadoNivelFilialAsync(chamado?.Carga?.Filial?.Codigo ?? 0, chamado?.Nivel ?? EscalationList.SemNivel, gatilhoMotivoChamadoSetor?.Setor?.Codigo ?? 0);

                if (chamado.XMLNotaFiscal != null)
                    chamado.XMLNotasFiscais.Add(chamado.XMLNotaFiscal);

                if (chamado.NovaMovimentacao)
                {
                    chamado.NovaMovimentacao = false;
                    await repChamado.AtualizarAsync(chamado);
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork, cancellationToken).BuscarConfiguracaoPadraoAsync();
                RegrasAnaliseChamados regrasAnalise = (await repRegrasAnaliseChamados.BuscarRegraPorMotivoChamadaAsync(chamado.MotivoChamado.Codigo, DateTime.Now)).Where(r => r.RegrasCargaDescarga?.Count > 0).FirstOrDefault();

                List<Usuario> aprovadores = regrasAnalise?.Aprovadores.Where(a => a.Codigo == this.Usuario.Codigo).ToList();
                var liberarAssumirChamado = true;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && regrasAnalise != null && (aprovadores?.Count == 0 && (analiseLiberacaoValor == null || (bool)analiseLiberacaoValor.LiberadoValorCargaDescarga)))
                    liberarAssumirChamado = false;

                string dataPrevisaoEntregaPedidos = await ObterDataPrevisaoEntregaPedidosAsync(chamado, unitOfWork, cancellationToken);

                var retorno = new
                {
                    chamado.Codigo,
                    MotivoChamado = chamado.MotivoChamado.Codigo,
                    chamado.Situacao,
                    CargaDevolucao = chamado.CargaDevolucao?.Codigo ?? 0,
                    chamado.ResponsavelOcorrencia,
                    CodigoResponsavel = chamado.Responsavel?.Codigo ?? 0,
                    CodigoAutor = chamado.Autor?.Codigo ?? 0,
                    Representante = chamado.Representante != null ? new { chamado.Representante.Codigo, chamado.Representante.Descricao } : null,
                    Responsavel = new { Codigo = chamado.Responsavel?.Codigo ?? 0, Descricao = chamado.Responsavel?.Descricao ?? string.Empty },
                    chamado.AosCuidadosDo,
                    chamado.GerarCargaDevolucao,
                    PodeEditar = (chamado.Responsavel?.Codigo ?? 0) == this.Usuario.Codigo || (chamado.Cliente?.CPF_CNPJ ?? 0) == (this.Usuario.Cliente?.CPF_CNPJ ?? 0),
                    PossuiResponsavel = chamado.Responsavel != null,
                    chamado.VeiculoCarregado,
                    chamado.Notificado,
                    PermiteAssumirChamadoMesmoSetor = chamado.Responsavel?.Setor?.PermitirAssumirChamadosDoMesmoSetor ?? false,
                    BloquearFinalizar = (chamado.MotivoChamado.ExigeValorNaLiberacao && configuracao.PermiteInformarChamadosNoLancamentoOcorrencia &&
                                         chamado.Situacao != SituacaoChamado.Finalizado && chamado.Situacao != SituacaoChamado.RecusadoPeloCliente),
                    UltimaResposta = ultimaRespostaEmbarcador ? ChamadoAosCuidadosDo.Embarcador : ChamadoAosCuidadosDo.Transporador,
                    ClienteEntrega = chamado.CargaEntrega?.Cliente?.Descricao ?? string.Empty,
                    CargaEntrega = chamado.CargaEntrega?.Codigo ?? 0,
                    Abertura = new
                    {
                        chamado.ResponsavelChamado,
                        Valor = chamado.Valor.ToString("n2"),
                        NumeroPallet = chamado.NumeroPallet.ToString("n2"),
                        QuantidadeItens = chamado.QuantidadeItens.ToString("n2"),
                        Carga = new { Codigo = chamado.Carga?.Codigo ?? 0, Descricao = chamado.Carga?.CodigoCargaEmbarcador ?? string.Empty },
                        Pedido = new { Codigo = chamado.Pedido?.Codigo ?? 0, Descricao = chamado.Pedido?.NumeroPedidoEmbarcador ?? string.Empty },
                        PedidoEmbarcador = new { Codigo = chamado.Pedido?.Codigo ?? 0, Descricao = chamado.Pedido?.NumeroPedidoEmbarcador ?? string.Empty },
                        MotivoChamado = new { chamado.MotivoChamado.Codigo, Descricao = chamado.MotivoChamado.DescricaoComTipo },
                        Cliente = new { Codigo = chamado.Cliente?.Codigo ?? 0, Descricao = chamado.Cliente?.Nome ?? "" },
                        Tomador = new { Codigo = chamado.Tomador?.Codigo ?? 0, Descricao = chamado.Tomador?.Nome ?? "" },
                        Destinatario = new { Codigo = chamado.Destinatario?.Codigo ?? 0, Descricao = chamado.Destinatario?.Nome ?? "" },
                        CargaEntrega = new { Codigo = chamado.CargaEntrega?.Codigo ?? 0, Descricao = chamado.CargaEntrega?.Descricao ?? string.Empty },
                        ResumoChamado = serChamado.ObterResumoChamado(chamado, unitOfWork, configuracao, TipoServicoMultisoftware),
                        chamado.Observacao,
                        chamado.TipoCliente,
                        DataReentrega = chamado.DataReentrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        chamado.RetencaoBau,
                        DataRetencaoInicio = chamado.DataRetencaoInicio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataRetencaoFim = chamado.DataRetencaoFim?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        chamado.TempoRetencao,
                        ValorReferencia = chamado.ValorReferencia.ToString("n2"),
                        ValorDesconto = chamado.ValorDesconto.ToString("n2"),
                        ClienteResponsavel = new { Codigo = chamado.ClienteResponsavel?.Codigo ?? 0, Descricao = chamado.ClienteResponsavel?.Descricao ?? string.Empty },
                        GrupoPessoasResponsavel = new { Codigo = chamado.GrupoPessoasResponsavel?.Codigo ?? 0, Descricao = chamado.GrupoPessoasResponsavel?.Descricao ?? string.Empty },
                        chamado.TipoPessoaResponsavel,
                        Motorista = new { Codigo = chamado.Motorista?.Codigo ?? 0, Descricao = chamado.Motorista?.Descricao ?? string.Empty },
                        NomeMotoristaCarga = motoristaPrincipal?.Descricao ?? string.Empty,
                        NumeroMotoristaCarga = ObterNumeroCelularCompleto(motoristaPrincipal),
                        CodigoCarga = chamado.Carga?.Codigo ?? 0,
                        chamado.PagoPeloMotorista,
                        SaldoDescontadoMotorista = chamado.PagoPeloMotorista || chamado.MotivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista ? chamado.SaldoDescontadoMotorista.ToString("n2") : string.Empty,
                        TotalPagamentoMotorista = chamado.PagoPeloMotorista || chamado.MotivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista ? chamado.TotalPagamentoMotorista(configuracao.NaoDescontarValorSaldoMotorista).ToString("n2") : string.Empty,
                        DataEntradaRaio = chamado.CargaEntrega?.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataSaidaRaio = chamado.CargaEntrega?.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        QuantidadeMotivoChamado = chamado.Quantidade,
                        ExibirQuantidadeMotivoChamado = chamado.MotivoChamado.InformarQuantidade,
                        RealMotivo = new { Codigo = chamado.RealMotivo?.Codigo ?? 0, Descricao = chamado.RealMotivo?.Descricao ?? "" },
                        NotaFiscal = new { Codigo = chamado.XMLNotaFiscal?.Codigo ?? 0, Descricao = chamado.XMLNotaFiscal?.Chave ?? string.Empty },
                        ValorDaCarga = chamado.Carga?.DadosSumarizados?.ValorTotalProdutos.ToString("n2") ?? string.Empty,
                        PesoDaCarga = chamado.Carga?.DadosSumarizados?.PesoTotal.ToString("n2") ?? string.Empty,
                        ApresentarValorPesoDaCarga = chamado.MotivoChamado?.TipoOcorrencia?.ApresentarValorPesoDaCarga ?? false,
                        ModeloVeicularCarga = new { Codigo = chamado.ModeloVeicularCarga?.Codigo ?? 0, Descricao = chamado.ModeloVeicularCarga?.Descricao ?? string.Empty },
                        NotasFiscais = (
                            from obj in chamado.XMLNotasFiscais
                            select new
                            {
                                obj.Codigo,
                                obj.Numero,
                                Emitente = obj.Emitente?.Descricao,
                                DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                            }
                        ).Distinct().ToList(),
                        TipoDevolucao = chamado.CargaEntrega?.TipoDevolucao ?? TipoColetaEntregaDevolucao.Total,
                        MotivoDaDevolucao = new { Codigo = chamado.MotivoDaDevolucao?.Codigo ?? 0, Descricao = chamado.MotivoDaDevolucao?.Descricao ?? string.Empty },
                        CausasMotivoChamado = new { Codigo = chamado.CausasMotivoChamado?.Codigo ?? 0, Descricao = chamado.CausasMotivoChamado?.Descricao ?? "" },
                        TiposCausadoresOcorrencia = new { Codigo = chamado.TiposCausadoresOcorrencia?.Codigo ?? 0, Descricao = chamado.TiposCausadoresOcorrencia?.Descricao ?? "" },
                    },
                    chamado.QuantidadeImagensEsperada,
                    PossuiIntegracao = possuiIntegracao,
                    Anexos = (
                        from obj in chamado.Anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                            obj.NotaFiscalServico
                        }
                    ).ToList(),
                    Datas = (
                        from obj in chamado.Datas
                        select new
                        {
                            obj.Codigo,
                            CodigoMotivoChamadoData = obj.MotivoChamadoData.Codigo,
                            obj.MotivoChamadoData.Obrigatorio,
                            obj.MotivoChamadoData.Descricao,
                            DataInicio = obj.DataInicio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                            DataFim = obj.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty
                        }
                    ).ToList(),
                    PerfisAcesso = (
                        from obj in chamado.PerfisAcesso
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    ).ToList(),
                    chamado.MotivoChamado.PermiteInserirJustificativaOcorrencia,
                    MotivoPossuiArvore = arvoreDisponivel?.Count > 0,
                    TempoDoChamadoExpirou = superouTempoLimiteDoNivel,
                    NivelAtendimento = chamado?.Nivel ?? EscalationList.SemNivel,
                    ResponsaveisPorNivel = (
                                            from obj in listaDeResponsaveisPorNivelFilial
                                            select new
                                            {
                                                NomeCompleto = obj?.Nome ?? string.Empty,
                                                Cargo = obj?.CargoSetorTurno?.Descricao ?? string.Empty,
                                                Nivel = obj?.NivelEscalationList ?? null,
                                                Email = obj?.Email ?? string.Empty,
                                                Celular = !string.IsNullOrWhiteSpace(obj?.Celular_Formatado) ? obj?.Celular_Formatado : obj?.Telefone_Formatado ?? string.Empty
                                            }).ToList(),
                    QuantidadeMotivoChamado = chamado.Quantidade,
                    chamado.InformarDadosChamadoFinalizadoComCusto,
                    chamado.NaoAssumirDataEntregaNota,
                    AguardandoTratativaDoCliente = chamado?.AguardandoTratativaDoCliente ?? false,
                    ConsiderarDiasUteis = chamado?.MotivoChamado.ConsiderarHorasDiasUteis ?? false,
                    ExigirMotivoDeDevolucao = chamado.MotivoChamado.TipoOcorrencia?.ExigirMotivoDeDevolucao ?? false,
                    DataLimiteTratativa = (nivelAtendimento?.DataLimite ?? DateTime.MinValue).ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                    LiberadoValorChamadoOcorrencia = analiseLiberacaoValor != null ? analiseLiberacaoValor?.LiberadoValorCargaDescarga : false,
                    LiberarOpcaoAprovarValor = aprovadores?.Count > 0 ? true : false,
                    LiberarAssumirChamado = liberarAssumirChamado,
                    BloquearEstornoAtendimentosFinalizadosPortalTransportador = (configuracaoChamado?.BloquearEstornoAtendimentosFinalizadosPortalTransportador ?? false) || chamado.MotivoChamado.BloquearEstornoAtendimentosFinalizadosPortalTransportador,
                    chamado.Estadia,
                    DataPrevisaoEntregaPedidos = dataPrevisaoEntregaPedidos,
                    HabilitarClassificacaoCriticos = chamado.MotivoChamado?.HabilitarClassificacaoCriticos ?? false,
                    Critico = chamado.Critico,
                    Gerencial = chamado.Gerencial != null ? new { Codigo = chamado.Gerencial.Codigo, Descricao = chamado.Gerencial.Conteudo } : null,
                    CausaProblema = chamado.CausaProblema != null ? new { Codigo = chamado.CausaProblema.Codigo, Descricao = chamado.CausaProblema.Conteudo } : null,
                    FUP = chamado.FUP
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReabrirChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);

                // Valida
                if (chamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (chamado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Cancelada)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.SoPossivelReabrirUmAtendimentoCancelado);

                if (!chamado.Analistas.Contains(this.Usuario))
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.UsuarioSemPermissaoParaEsseAtendimento);

                chamado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Aberto;
                repChamado.Atualizar(chamado, Auditado);

                Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoCancelarAtendimento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AssumirChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigo);

                if (chamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (chamado.Analistas == null)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.NaoFoiPossivelEncontrarAnalistas);

                if (chamado.Responsavel != null)
                {
                    if (chamado.Responsavel.Codigo == this.Usuario.Codigo)
                        return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.VoceJaResponsavelPorEsseAtendimento);

                    if (!(ConfiguracaoEmbarcador?.PermitirAssumirChamadoDeOutroResponsavel ?? false) && !(this.Usuario?.Setor?.PermitirAssumirChamadosDoMesmoSetor ?? false))
                        return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.JaExisteUmResponsavelParaEsseAtendimento);

                    if (this.Usuario?.Setor != null && this.Usuario.Setor.PermitirAssumirChamadosDoMesmoSetor && !this.Usuario.PermitirAssumirAtendimentoManeiraSobreposta)
                    {
                        if (chamado.Responsavel.Setor != this.Usuario.Setor)
                            return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.VoceSoPodeAssumirAtendimentosDeResponsaveisDoMesmoSetor);
                    }
                }
                else
                {
                    if (chamado.Analistas.Count > 0)
                    {
                        if (!chamado.Analistas.Contains(this.Usuario))
                        {
                            if ((chamado.Carga?.Filial?.SetorAtendimento != null && ((this.Usuario?.Setor ?? null) != chamado.Carga.Filial.SetorAtendimento)))
                                return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.SeuSetorUsuarioNaoPodeAssumirEsseAtendimento);
                        }

                    }
                    else if ((chamado.Carga?.Filial?.SetorAtendimento != null && ((this.Usuario?.Setor ?? null) != chamado.Carga.Filial.SetorAtendimento)))
                        return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.SeuSetorUsuarioNaoPodeAssumirEsseAtendimento);

                    chamado.DataPrimeiraVezAssumido = DateTime.Now;
                }

                unitOfWork.Start();

                chamado.Responsavel = this.Usuario;

                if (chamado.CargaEntrega != null)
                    chamado.NotificacaoMotoristaMobile = true;

                repositorioChamado.Atualizar(chamado);

                if (!string.IsNullOrWhiteSpace(chamado.CargaEntrega?.IdTrizy))
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarChamadoOcorrencia(chamado, EventoIntegracaoOcorrenciaTrizy.AssumirAtendimento, this.Usuario, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, Localization.Resources.Chamado.ChamadoOcorrencia.AssumiuResponsabilidadeDoAtendimento, unitOfWork);

                unitOfWork.CommitChanges();

                if (chamado.NotificacaoMotoristaMobile)
                {
                    dynamic conteudo = new
                    {
                        chamado.Codigo,
                        Analista = chamado.Responsavel?.Nome ?? "",
                        codigoMultisofware = Cliente.Codigo,
                    };

                    Servicos.Embarcador.Chamado.NotificacaoMobile notificacaoSrv = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, Cliente.Codigo);
                    notificacaoSrv.NotificarMotoristasChamadoEmAnalise(chamado, chamado.Carga.Motoristas.ToList(), conteudo);
                }

                new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAssumido(chamado, unitOfWork);

                return new JsonpResult(chamado.Codigo);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoAssumirAtendimento);
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

                Servicos.Embarcador.Chamado.Chamado srvChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado();

                PreencherObjetoChamado(objetoChamado, unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objetoChamado, this.Usuario, TipoServicoMultisoftware, Auditado, unitOfWork);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

                SalvarChamadoDatas(chamado, unitOfWork);
                SalvarNotasFiscais(chamado, configuracaoChamado, unitOfWork);
                SalvarNotasDeDevolucao(chamado, unitOfWork);
                SalvarProdutosNFDevolucao(chamado, configuracaoChamado, objetoChamado.DevolucaoParcial, unitOfWork);

                SalvarChamadoPerfisAcesso(chamado, unitOfWork);

                ValidarESalvaDiariaAutomatica(chamado, unitOfWork);

                VincularOcorrenciaNoAtendimentoAutomaticamente(chamado, unitOfWork);

                if (chamado.XMLNotasFiscais?.Count > 0 && (chamado?.Carga?.TipoOperacao?.GerarEntregaPorNotaFiscalCarga ?? false))
                    chamado.CargaEntrega = repCargaEntrega.BuscarPorCargaENotaFiscal(chamado.Carga.Codigo, chamado.XMLNotasFiscais.FirstOrDefault().Codigo);

                repChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();

                Task t = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Repositorio.UnitOfWork unitOfWorkTask = new Repositorio.UnitOfWork(unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.AtualizarAtual);
                        srvChamado.EnviarEmailChamadoAberto(chamado, unitOfWorkTask);
                        srvChamado.EnviarEmailChamadoAbertoDespesaMotorista(chamado, unitOfWorkTask, TipoServicoMultisoftware);

                        if (chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                        {
                            int codigoChamado = chamado.Codigo;
                            string stringConexao = unitOfWorkTask.StringConexao;
                            Servicos.Embarcador.Chamado.Chamado.EnviarEmailCargaDevolucao(codigoChamado, stringConexao);
                        }

                        Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWorkTask);
                    }
                    catch (Exception) { /* Enterra erro no envio dos emails*/}
                });

                return new JsonpResult(chamado.Codigo);
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

                if (excecao.InnerException != null && ((System.Data.SqlClient.SqlException)excecao.InnerException).Number == 2627 && excecao.InnerException.Message.Contains("UK_CARGA_ENTREGA_MOTIVO"))
                    return new JsonpResult(false, true, "Já existe um atendimento em aberto para a mesma carga, motivo e cliente! Favor finalizar o anterior antes de abrir o novo.");
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
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

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo, true);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

                PreencherChamado(chamado, unitOfWork);

                repChamado.Atualizar(chamado, Auditado);

                SalvarChamadoDatas(chamado, unitOfWork);
                SalvarNotasFiscais(chamado, configuracaoChamado, unitOfWork);
                SalvarNotasDeDevolucao(chamado, unitOfWork);

                if (chamado.XMLNotasFiscais?.Count > 0 && (chamado.Carga.TipoOperacao?.GerarEntregaPorNotaFiscalCarga ?? false))
                    chamado.CargaEntrega = repCargaEntrega.BuscarPorCargaENotaFiscal(chamado.Carga.Codigo, chamado.XMLNotasFiscais.FirstOrDefault().Codigo);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);

                return new JsonpResult(chamado.Codigo);
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
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoAtualizarOsDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Responder()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo);

                if (chamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Chamado.Chamado.ResponderChamado(chamado, TipoServicoMultisoftware, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarRegras(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                await unitOfWork.StartAsync(cancellationToken);

                // Instancia repositorios
                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork, TipoServicoMultisoftware, Auditado);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork, cancellationToken);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = await repositorioChamado.BuscarPorCodigoAsync(codigo);

                // Valida
                if (chamado == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (chamado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.SemRegra)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.SituacaoNaoPermiteEssaOperacao);

                // Vincula e notifica usuarios
                if (await servicoChamado.DefinirAnalistasChamado(chamado, this.Usuario))
                    chamado.Situacao = SituacaoChamado.Aberto;

                // Persiste dados
                await repositorioChamado.AtualizarAsync(chamado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                servicoChamado.EnviarEmailChamadoAberto(chamado, unitOfWork);

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoAtualizarOsDados);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorioDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigo);

                if (!chamado.GerarCargaDevolucao)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.NaoePossivelGerarRelatorioDeUmAtendimentoQueNaoDeDevolucao);

                byte[] arquivo = GerarRelatorioEmbarcador(chamado, out string msg);

                // Retorna o arquivo
                return Arquivo(arquivo, "application/pdf", "Devolução - " + chamado.Numero + ".pdf");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoGerarRelatorio);
            }
        }

        public async Task<IActionResult> BuscarCargaEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int.TryParse(Request.Params("CodigoCarga"), out int codigoCarga);
                double.TryParse(Request.Params("Cliente"), out double cliente);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorClienteECarga(codigoCarga, cliente);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Não encontrada entrega para cliente " + Request.Params("Cliente") + " na carga informada.");

                var retorno = new
                {
                    cargaEntrega.Codigo
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoEntrega);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfiguracoesChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repositorioConfiguracaoChamado.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    PermitirGerarAtendimentoPorPedido = configuracaoChamado?.PermitirGerarAtendimentoPorPedido ?? false,
                    PermitirAtualizarChamadoStatus = configuracaoChamado?.PermitirAtualizarChamadoStatus ?? false,
                    OcultarTomadorNoAtendimento = configuracaoChamado?.OcultarTomadorNoAtendimento ?? false,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmafalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesCargaChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.CargaSelecionadaNaoFoiEncontrada);

                decimal valorDescarga = repCargaComponentesFrete.BuscarTotalCargaPorCompomente(codigoCarga, TipoComponenteFrete.DESCARGA, null, false);

                var retorno = new
                {
                    ValorDescarga = valorDescarga > 0 ? valorDescarga.ToString("n2") : string.Empty,
                    PesoDaCarga = carga?.DadosSumarizados?.PesoTotal.ToString("n2") ?? string.Empty,
                    ValorDaCarga = carga?.DadosSumarizados?.ValorTotalProdutos.ToString("n2") ?? string.Empty
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoObterDetalhesDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarValorCargaDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.CargaSelecionadaNaoFoiEncontrada);

                (decimal valorCarga, decimal valorDescarga) = CalcularValoresCargaDescarga(carga);

                var retorno = new
                {
                    valorCarga = valorCarga.ToString("n2"),
                    valorDescarga = valorDescarga.ToString("n2")
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoObterDetalhesDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarValoresCargaDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoMotivoChamado = Request.GetIntParam("MotivoChamado");
                double cliente = Request.GetDoubleParam("Cliente");
                decimal valor = Request.GetDecimalParam("Valor");
                decimal valorCargaParam = Request.GetDecimalParam("ValorCarga");
                decimal valorDescargaParam = Request.GetDecimalParam("ValorDescarga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                MotivoChamado motivoChamado = codigoMotivoChamado > 0 ? repMotivoChamado.BuscarPorCodigo(codigoMotivoChamado) : null;
                List<RegrasAnaliseChamados> regrasAnalises = repRegrasAnaliseChamados.BuscarRegraPorMotivoChamada(motivoChamado.Codigo, DateTime.Now);
                List<ChamadoAnalise> chamadosAnalise = repChamadoAnalise.BuscarPorChamado(codigo);


                Dominio.Entidades.Embarcador.Chamados.Chamado chamadoOcorrencia = repChamado.BuscarPorCodigo(codigo);
                if (chamadoOcorrencia.Situacao != SituacaoChamado.Aberto)
                    return new JsonpResult(true, true, "");


                if (chamadosAnalise.Where(c => c.LiberadoValorCargaDescarga == true).Any())
                    return new JsonpResult(true, true, "");

                if (valorCargaParam == 0 && valorDescargaParam == 0)
                {
                    (decimal valorCarga, decimal valorDescarga) = CalcularValoresCargaDescarga(carga);

                    valorCargaParam = valorCarga;
                    valorDescargaParam = valorDescarga;
                }

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.CargaSelecionadaNaoFoiEncontrada);

                decimal valorCargaJaUtilizado = 0, valorDescargaJaUtilizado = 0;
                List<Chamado> chamadosCarga = repChamado.ChamadosMesmaCargaMotivoCliente(codigoCarga, codigoMotivoChamado, cliente, codigo);

                foreach (Chamado chamado in chamadosCarga)
                {
                    valorCargaJaUtilizado += chamado.Valor;
                    valorDescargaJaUtilizado += chamado.Valor;
                }

                var regrasCargaDescarga = regrasAnalises
                                          .Where(r => r.RegraPorCargaDescarga && r.RegrasCargaDescarga.Any())
                                          .SelectMany(o => o.RegrasCargaDescarga)
                                          .ToList();

                if (motivoChamado?.ValidaValorCarga ?? false)
                {
                    if ((valorCargaJaUtilizado + valor) > valorCargaParam)
                        return new JsonpResult(false, false, Localization.Resources.Chamado.ChamadoOcorrencia.SaldoValorCargaJaUtilizado);
                }
                else if (motivoChamado?.ValidaValorDescarga ?? false)
                {
                    if ((valorDescargaJaUtilizado + valor) > valorDescargaParam)
                        return new JsonpResult(false, false, Localization.Resources.Chamado.ChamadoOcorrencia.SaldoValorDescargaJaUtilizado);
                }

                foreach (RegrasChamadosCargaDescarga cargaDescarga in regrasCargaDescarga)
                {
                    if (cargaDescarga.ValidarValorCarga)
                    {
                        if (valorCargaParam > 0 && (valorCargaJaUtilizado + valor) >= valorCargaParam)
                            return new JsonpResult(false, false, Localization.Resources.Chamado.ChamadoOcorrencia.SaldoValorCargaJaUtilizado);
                    }

                    if (cargaDescarga.ValidarValorDescarga)
                    {
                        if ((valorDescargaJaUtilizado + valor) > valorDescargaParam)
                            return new JsonpResult(false, false, Localization.Resources.Chamado.ChamadoOcorrencia.SaldoValorDescargaJaUtilizado);
                    }

                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar valores carga e descarga !");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesPedidoChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Pedido");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repCargaPedido.BuscarPrimeiroPorPedidoComCargaAtivaAsync(codigoPedido);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Chamado.ChamadoOcorrencia.PedidoSelecionadoNaoFoiEncontrado);

                long clinteCodigo = (ConfiguracaoEmbarcador.ChamadoOcorrenciaUsaRemetente ? pedido.Remetente?.Codigo : pedido.Destinatario?.Codigo) ?? 0;
                string clienteNome = (ConfiguracaoEmbarcador.ChamadoOcorrenciaUsaRemetente ? pedido.Remetente?.Nome : pedido.Destinatario?.Nome) ?? string.Empty;
                Usuario motorista = pedido.Motoristas?.FirstOrDefault();

                var retorno = new
                {
                    Destinatario = new { Codigo = pedido.Destinatario?.Codigo ?? 0, Descricao = pedido.Destinatario?.Nome ?? string.Empty },
                    Tomador = new { Codigo = pedido.Tomador?.Codigo ?? 0, Descricao = pedido.Tomador?.Nome ?? string.Empty },
                    Motorista = new { Codigo = motorista?.Codigo ?? 0, Descricao = motorista?.Nome ?? string.Empty },
                    Cliente = new { Codigo = clinteCodigo, Descricao = clienteNome },
                    Carga = new { Codigo = configuracaoChamado.PermitirGerarAtendimentoPorPedido ? cargaPedido?.Carga?.Codigo ?? 0 : 0, Descricao = configuracaoChamado.PermitirGerarAtendimentoPorPedido ? cargaPedido?.Carga?.CodigoCargaEmbarcador ?? string.Empty : string.Empty },
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoObterDetalhesDoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDatasMotivoChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMotivoChamado = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.MotivoChamadoData repositorio = new Repositorio.Embarcador.Chamados.MotivoChamadoData(unitOfWork);
                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoData> datas = repositorio.BuscarAtivosPorMotivoChamado(codigoMotivoChamado);

                return new JsonpResult(
                    (
                        from obj in datas
                        select new
                        {
                            Codigo = 0,
                            CodigoMotivoChamadoData = obj.Codigo,
                            obj.Descricao,
                            obj.Obrigatorio
                        }
                    ).ToList()
                );
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoObterOsDadosDasDatasDoMotivoDeChamado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidaChamadoDuplicado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoMotivoChamado = Request.GetIntParam("MotivoChamado");

                decimal valor = Request.GetDecimalParam("Valor");
                double cliente = Request.GetDoubleParam("Cliente");
                double motorista = Request.GetDoubleParam("Motorista");

                if (repChamado.ContemChamadoMesmoMotorista(codigoMotivoChamado, motorista))
                    return new JsonpResult(new { ChamadoMesmoMotorista = true });

                if (repChamado.ContemChamadoMesmaCargaMotivoClienteValor(codigoCarga, codigoMotivoChamado, valor, cliente, codigo))
                    return new JsonpResult(new { AtendimentoDuplicado = true });

                return new JsonpResult(new { AtendimentoDuplicado = false, ChamadoMesmoMotorista = false });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoValidarAtendimentoDuplicado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSaldoDescontadoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista servicoAutorizacaoPagamentoMotorista = new Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista(unitOfWork);

                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoMotivoChamado = Request.GetIntParam("MotivoChamado");

                decimal valor = Request.GetDecimalParam("Valor");

                int codigoTipoPagamentoMotorista = codigoMotivoChamado > 0 && codigoMotorista > 0 ? repositorioMotivoChamado.BuscarCodigoTipoPagamentoMotoristaPorMotivoChamado(codigoMotivoChamado) : 0;

                return new JsonpResult(servicoAutorizacaoPagamentoMotorista.ObterSaldoDescontadoMotorista(codigoMotorista, codigoTipoPagamentoMotorista, valor, ConfiguracaoEmbarcador));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoBuscarSaldoDescontado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EscalarAtendimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoChamado = Request.GetIntParam("Codigo");
                EscalationList nivelAtendimentoAtual = Request.GetEnumParam<EscalationList>("NivelAtual");

                Repositorio.Embarcador.Chamados.NivelAtendimento repNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repGatilhosMotivo = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.NivelAtendimento nivelAtendimento = repNivelAtendimento.BuscarSeChamadoComNivelAtual(codigoChamado, nivelAtendimentoAtual);

                if (nivelAtendimento == null)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NivelDoChamadoNaoEncontrado);

                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList gatilho = repGatilhosMotivo.BuscarNivelPorMotivoChamado(nivelAtendimento.Chamado?.MotivoChamado?.Codigo ?? 0, nivelAtendimentoAtual.ObterProximoNivel());

                if (gatilho == null)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NaoEPossivelEscalarAtendimentoPorqueNaoTemProximoAtendimentoCadastrado, Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoEPossivelEscalar);

                if ((nivelAtendimento.Chamado?.MotivoChamado?.ValidarEscaladaAtendimentoUsuarioResponsavel ?? false) && !UsuarioAptoARealizarAcoesEscalada(codigoChamado, unitOfWork))
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.UsuarioNaoAptoRealizarEscalada, Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.UsuarioNaoAptoEscaladaParaRealizarEscalada);


                nivelAtendimento.Chamado.Nivel = nivelAtendimentoAtual.ObterProximoNivel();
                nivelAtendimento.DataCriacao = DateTime.Now;
                if (gatilho.MotivoChamado.ConsiderarHorasDiasUteis)
                {
                    nivelAtendimento.DataLimite = new Servicos.Embarcador.Chamado.Chamado(unitOfWork).AdicionarTempoDiaUtil(DateTime.Now, gatilho.Tempo);
                }
                else
                {
                    nivelAtendimento.DataLimite = DateTime.Now.AddMinutes(gatilho.Tempo);
                }
                nivelAtendimento.FoiNotificado = false;
                nivelAtendimento.Nivel = nivelAtendimentoAtual.ObterProximoNivel();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nivelAtendimento.Chamado, null, $"Subiu ao nivel {nivelAtendimento.Nivel.ObterDescricao()}", unitOfWork);

                repNivelAtendimento.Atualizar(nivelAtendimento);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoEPossivelEscalar || excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.UsuarioNaoAptoEscaladaParaRealizarEscalada)
            {
                return new JsonpResult(true, excecao.Message);
            }
            catch (ControllerException exception)
            {
                return new JsonpResult(false, exception.Message);
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro(exception);
                return new JsonpResult(false, exception.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RetornarNivelUm()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoChamado = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.NivelAtendimento repNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repGatilhosMotivo = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.NivelAtendimento nivelAtendimento = repNivelAtendimento.BuscarPrimeiroPorChamado(codigoChamado);

                if (nivelAtendimento == null)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NivelDoChamadoNaoEncontrado);

                Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList gatilho = repGatilhosMotivo.BuscarNivelPorMotivoChamado(nivelAtendimento.Chamado?.MotivoChamado?.Codigo ?? 0, EscalationList.Nivel1);

                if (gatilho == null)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NaoFoiPossivelEncontrarRegistro);

                if ((nivelAtendimento.Chamado?.MotivoChamado?.ValidarEscaladaAtendimentoUsuarioResponsavel ?? false) && !UsuarioAptoARealizarAcoesEscalada(codigoChamado, unitOfWork))
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.UsuarioNaoAptoRealizarEscalada, Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.UsuarioNaoAptoARetornarAoNivelUm);

                nivelAtendimento.Chamado.Nivel = EscalationList.Nivel1;
                nivelAtendimento.DataCriacao = DateTime.Now;
                if (gatilho.MotivoChamado.ConsiderarHorasDiasUteis)
                {
                    nivelAtendimento.DataLimite = new Servicos.Embarcador.Chamado.Chamado(unitOfWork).AdicionarTempoDiaUtil(DateTime.Now, gatilho.Tempo);
                }
                else
                {
                    nivelAtendimento.DataLimite = DateTime.Now.AddMinutes(gatilho.Tempo);
                }
                nivelAtendimento.FoiNotificado = false;
                nivelAtendimento.Nivel = EscalationList.Nivel1;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nivelAtendimento.Chamado, null, $"Retornou o atendimento para o nível 1 manualmente", unitOfWork);

                repNivelAtendimento.Atualizar(nivelAtendimento);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.UsuarioNaoAptoARetornarAoNivelUm)
            {
                return new JsonpResult(true, excecao.Message);
            }
            catch (ControllerException exception)
            {
                return new JsonpResult(false, exception.Message);
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro(exception);
                return new JsonpResult(false, exception.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterSobrasAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoChamado = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras repOcorrenciaSobras = new Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoChamado);

                if (chamado == null)
                    return new JsonpResult(false, "O registro não foi encontrado");

                List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> sobras = repOcorrenciaSobras.BuscarPorCargaEntrega(chamado.CargaEntrega?.Codigo ?? 0);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrencia = repositorioOcorrenciaColetaEntrega.BuscarPorCodigo(chamado?.CargaEntrega?.Codigo ?? 0, false);

                return new JsonpResult(
                    new
                    {
                        Sobras = (from sobra in sobras
                                  select new
                                  {
                                      sobra.Codigo,
                                      sobra.CodigoSobra,
                                      sobra.QuantidadeSobra
                                  }).ToList(),
                        PermiteSobras = true
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
        public async Task<IActionResult> ObterMotivoProcesso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tipoOcorrencias = repositorioTipoOcorrencia.Consultar(false, false, 0, 100);

                var retorno = (from tipoOcorrencia in tipoOcorrencias
                               select new
                               {
                                   tipoOcorrencia.Codigo,
                                   tipoOcorrencia.Descricao
                               });

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterGeneroAreaEnvolvida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.GeneroMotivoChamado repositorioGeneroMotivoChamado = new Repositorio.Embarcador.Chamados.GeneroMotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado repositorioAreaEnvolvidaMotivoChamado = new Repositorio.Embarcador.Chamados.AreaEnvolvidaMotivoChamado(unitOfWork);

                bool existeGeneroCadastrado = repositorioGeneroMotivoChamado.ExisteGeneroCadastrado();
                bool existeAreaEnvolvidaCadastrado = repositorioAreaEnvolvidaMotivoChamado.ExisteAreaEnvolvidaCadastrada();

                return new JsonpResult((existeGeneroCadastrado && existeAreaEnvolvidaCadastrado));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRelatorioAtendimento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unidadeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigoChamado);
                bool liberouOcorrencia = Request.GetBoolParam("LiberouOcorrencia");

                Servicos.Embarcador.Chamado.Chamado serChamado = new Servicos.Embarcador.Chamado.Chamado(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoChamado);

                string mensagemErro = string.Empty;

                byte[] pdf = ReportRequest.WithType(ReportType.ChamadoAtendimento)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("Codigo", chamado.Codigo.ToString())
                    .CallReport()
                    .GetContentFile();

                //Servicos.Embarcador.Chamado.Chamado.GerarRelatorioChamadoAtendimento(chamado.Codigo, unidadeTrabalho, out mensagemErro);

                if (pdf == null)
                    return new JsonpResult(true, false, mensagemErro);

                string fileName = "Atendimento - " + chamado.Numero + ".pdf";
                if (ConfiguracaoEmbarcador.SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado && liberouOcorrencia)
                    SalvarRelatorioAtendimentoNosAnexos(chamado, pdf, fileName, unidadeTrabalho);

                return Arquivo(pdf, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do atendimento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterSeExisteRegraUsuarioResponsavel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegraAtendimentoChamado = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(unitOfWork);

                bool ExisteRegraUsuarioResponsavel = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && repositorioRegraAtendimentoChamado.BuscarSeExisteRegra());

                var retorno = new
                {
                    ExisteRegraUsuarioResponsavel = ExisteRegraUsuarioResponsavel
                };

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

        [AllowAuthenticate]
        public async Task<IActionResult> AprovarValorCargaDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regraAnalise = new Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados();

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Chamados.ChamadoAnalise repositorioChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise();

                RegrasAnaliseChamados regrasAnalise = repRegrasAnaliseChamados.BuscarRegraPorMotivoChamada(chamado.MotivoChamado.Codigo, DateTime.Now).Where(r => r.RegrasCargaDescarga.Count > 0).FirstOrDefault();

                if (chamado == null)
                {
                    return new JsonpResult(false, true, "Chamado não encontrado, para aprovação!");
                }

                if (regrasAnalise.Aprovadores.Where(a => a.Codigo == this.Usuario.Codigo).Any())
                {
                    List<ChamadoAnalise> chamadosAnalises = repositorioChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                    if (chamadosAnalises.Any() && chamadosAnalises.Where(c => c.LiberadoValorCargaDescarga == true).Count() > 0)
                    {
                        return new JsonpResult(false, @$"Chamado já, possui aprovação, do usuário {chamadosAnalises.Where(c => c.LiberadoValorCargaDescarga == true)?.FirstOrDefault()?.Autor?.Nome}");
                    }

                    chamadoAnalise.Autor = this.Usuario;
                    chamadoAnalise.Observacao = "Aprovação chamado com valor maior que pedido";
                    chamadoAnalise.DataCriacao = DateTime.Now;
                    chamadoAnalise.LiberadoValorCargaDescarga = true;
                    chamadoAnalise.Chamado = chamado;

                    repositorioChamadoAnalise.Inserir(chamadoAnalise);
                    unitOfWork.CommitChanges();

                    return new JsonpResult(true, true, "");
                }
                else
                {
                    return new JsonpResult(false, true, "Usuário não tem permissão para aprovar valor");
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao aprovar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracoesMotivoChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMotivoChamado = Request.GetIntParam("CodigoMotivoChamado");
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = codigoMotivoChamado > 0 ? repositorioMotivoChamado.BuscarPorCodigo(codigoMotivoChamado) : null;
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamados = repositorioChamado.BuscarPorCargaEMotivo(codigoCarga, codigoMotivoChamado);

                var retorno = new
                {
                    PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga = motivoChamado?.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga ?? false,
                    ExisteChamadoComMesmoMotivoChamadoCarga = listaChamados != null && listaChamados.Count > 0,
                    NumerosChamadosExistentes = listaChamados != null && listaChamados.Count > 0 ? listaChamados.Select(obj => obj.Numero).ToList() : null,
                };

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

        [AllowAuthenticate]
        public async Task<IActionResult> AprovarMultiplasOcorrencias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RowIdGridOcorrencia> codigosOcorrenciasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RowIdGridOcorrencia>>(Request.Params("MultiplasOcorrenciasSelecionadas"));
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repCargaOcorrencia.BuscarPorCodigos(codigosOcorrenciasSelecionadas.Select(e => e.DT_RowID).ToList(), false);

                if (ocorrencias.Count == 0)
                    return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.NaoFoiPossivelEncontrarAsOcorrenciasSelecionadas);

                // Busca todas as regras das ocorrencias selecionadas

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repositorioCargaOcorrenciaAutorizacao);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes = servicoAutorizacaoOcorrencia.BuscarRegrasPorOcorrencias(ocorrencias, Usuario.Codigo);

                // Aprova todas as regras
                int quantidadeAprovadas = servicoAutorizacaoOcorrencia.AprovarMultiplasOcorrencias(ocorrenciasAutorizacoes, null, 0, 0, null);

                if (quantidadeAprovadas == 0)
                    return new JsonpResult(true, Localization.Resources.Chamado.ChamadoOcorrencia.NenhumaAlcadaPendenteDeAprovacaoParaOSeuUsuario);

                return new JsonpResult(new
                {
                    RegrasModificadas = quantidadeAprovadas,
                    RegrasExigemMotivo = ocorrenciasAutorizacoes.Count() - quantidadeAprovadas
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoAprovarAsOcorrencias);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReprovarMultiplasOcorrencias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RowIdGridOcorrencia> codigosOcorrenciasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RowIdGridOcorrencia>>(Request.Params("MultiplasOcorrenciasSelecionadas"));
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

                int codigoJustificativa = 0;
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia justificativa = repMotivoRejeicaoOcorrencia.BuscarPorCodigo(codigoJustificativa);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repCargaOcorrencia.BuscarPorCodigos(codigosOcorrenciasSelecionadas.Select(e => e.DT_RowID).ToList(), false);

                if (ocorrencias.Count == 0)
                    return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.NaoFoiPossivelEncontrarAsOcorrenciasSelecionadas);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, Auditado, TipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, ConfiguracaoEmbarcador, repositorioCargaOcorrenciaAutorizacao);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> ocorrenciasAutorizacoes = servicoAutorizacaoOcorrencia.BuscarRegrasPorOcorrencias(ocorrencias, Usuario.Codigo);

                int quantidadeReprovadas = servicoAutorizacaoOcorrencia.ReprovarMultiplasOcorrencias(ocorrenciasAutorizacoes, motivo, justificativa, null, null, 0, 0, null);

                if (quantidadeReprovadas == 0)
                    return new JsonpResult(true, Localization.Resources.Chamado.ChamadoOcorrencia.NenhumaAlcadaEncontradaParaOSeuUsuario);

                return new JsonpResult(new
                {
                    RegrasModificadas = quantidadeReprovadas
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.OcorreuUmaFalhaAoRejeitarAsOcorrencias);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = await ObterGridPesquisa(5000);
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

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarEstadia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");
                SimNao estadia = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao>("Estadia");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = await repositorioChamado.BuscarPorCodigoAsync(codigo);

                if (chamado == null)
                    return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.ChamadoNaoEncontrado);

                chamado.Estadia = estadia;
                await repositorioChamado.AtualizarAsync(chamado);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true, Localization.Resources.Chamado.ChamadoOcorrencia.EstadiaAtualizadaComSucesso);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Chamado.ChamadoOcorrencia.FalhaAoAtualizarEstadia);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherObjetoChamado(Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagem repositorioAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrenciaCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamadoCausas repositorioMotivoChamadoCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repositorioConfiguracaoChamado.BuscarConfiguracaoPadrao();


            int codigoCarga = Request.GetIntParam("Carga");
            int codigoPedido;
            int codigoMotivoChamado = Request.GetIntParam("MotivoChamado");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoGrupoPessoasResponsavel = Request.GetIntParam("GrupoPessoasResponsavel");
            int codigoRealMotivo = Request.GetIntParam("RealMotivo");
            int codigoCargaEntrega = Request.GetIntParam("CodigoCargaEntrega");
            int codigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga");
            int codigoMotivoDaDevolucao = Request.GetIntParam("MotivoDaDevolucao");
            int codigoTiposCausadoresOcorrencia = Request.GetIntParam("TiposCausadoresOcorrencia");
            int codigoCausasMotivoChamado = Request.GetIntParam("CausasMotivoChamado");

            double cpfCnpjCliente = Request.GetDoubleParam("Cliente");
            double cpfCnpjTomador = Request.GetDoubleParam("Tomador");
            double cpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");
            double cpfCnpjClienteResponsavel = Request.GetDoubleParam("ClienteResponsavel");

            bool devolucaoParcial = Request.GetBoolParam("TipoDevolucao");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                codigoPedido = Request.GetIntParam("PedidoEmbarcador");
            else
                codigoPedido = Request.GetIntParam("Pedido");

            DateTime.TryParse(Request.Params("DataRetencaoInicio"), out DateTime dataRetencaoInicio);
            DateTime.TryParse(Request.Params("DataRetencaoFim"), out DateTime dataRetencaoFim);
            DateTime.TryParse(Request.Params("DataReentrega"), out DateTime dataReentrega);

            Dominio.Enumeradores.TipoTomador tipoCliente = Dominio.Enumeradores.TipoTomador.Destinatario;
            if (Enum.TryParse(Request.Params("TipoCliente"), out Dominio.Enumeradores.TipoTomador tipoClienteAux))
                tipoCliente = tipoClienteAux;

            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = codigoMotivoChamado > 0 ? repositorioMotivoChamado.BuscarPorCodigo(codigoMotivoChamado) : null;
            if (motivoChamado == null)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.MotivoObrigatorio);

            if (motivoChamado.InformarQuantidade)
                chamado.Quantidade = Request.GetIntParam("QuantidadeMotivoChamado");

            if (chamado.Quantidade == 0 && (motivoChamado?.InformarQuantidade ?? false))
                throw new ControllerException("Obrigatório informar a Quantidade.");

            if (codigoRealMotivo == 0 && (motivoChamado?.ObrigarRealMotivo ?? false))
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NecessarioInformarRealMotivo);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = codigoCarga > 0 ? repositorioCarga.BuscarPorCodigo(codigoCarga) : null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                if (codigoPedido == 0)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.PedidoObrigatorio);

                carga = repositorioCarga.BuscarPorPedido(codigoPedido);

                if (carga == null && !motivoChamado.PermiteAtendimentoSemCarga)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NaoExisteCargaVinculadaPedido);

                chamado.Pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && configuracaoChamado.PermitirGerarAtendimentoPorPedido)
            {
                chamado.Pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);
            }
            else
            {
                if (carga == null && !motivoChamado.PermiteAtendimentoSemCarga)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.CargaObrigatoria);
            }

            if (carga?.SituacaoCarga == SituacaoCarga.Anulada || carga?.SituacaoCarga == SituacaoCarga.Cancelada)
                throw new ControllerException(string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.SituacaoDaCargaNaoPermiteSeguir, carga.SituacaoCarga.ObterDescricao()));

            if (carga?.TipoOperacao?.ConfiguracaoTipoOperacaoChamado?.NaoPermitirGerarAtendimento ?? false)
                throw new ControllerException(string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.TipoOperacaoCargaNaoPermiteAbrirAtendimento, carga.TipoOperacao.Descricao));

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = codigoModeloVeicularCarga > 0 ? repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga) : null;
            if (motivoChamado.ExigeInformarModeloVeicularAberturaChamado && modeloVeicularCarga == null)
                throw new ControllerException("Obrigatório informar o modelo veicular da entrega.");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                chamado.Empresa = this.Empresa;
            else
                chamado.Empresa = carga?.Empresa ?? Empresa;

            if (cpfCnpjCliente > 0)
                chamado.Cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

            if (cpfCnpjTomador > 0)
                chamado.Tomador = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

            if (cpfCnpjDestinatario > 0)
                chamado.Destinatario = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

            chamado.ClienteResponsavel = cpfCnpjClienteResponsavel > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjClienteResponsavel) : null;
            chamado.GrupoPessoasResponsavel = codigoGrupoPessoasResponsavel > 0 ? repositorioGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoasResponsavel) : null;
            chamado.TipoPessoaResponsavel = Request.GetNullableEnumParam<TipoPessoa>("TipoPessoaResponsavel");

            if (codigoMotorista > 0)
            {
                if (motivoChamado.NaoPermitirLancarAtendimentoSemAcertoAberto && !repositorioAcertoViagem.ContemAcertoEmAndamento(codigoMotorista))
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.MotoristaNaoContemAcertoAberto);

                chamado.Motorista = repositorioUsuario.BuscarPorCodigo(codigoMotorista);
            }

            if (chamado.Cliente == null && chamado.Destinatario != null && carga != null && (motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao || motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega || motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga))
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoPorCargaEClienteDestinatario(carga.Codigo, chamado.Destinatario.CPF_CNPJ);

                chamado.Cliente = cargaPedidos.Count > 0 ? cargaPedidos.FirstOrDefault().Pedido.Remetente : chamado.Cliente;
            }

            chamado.RealMotivo = codigoRealMotivo > 0 ? repTipoOcorrenciaCTe.BuscarPorCodigo(codigoRealMotivo) : null;

            if (chamado.Cliente == null && (!motivoChamado.ChamadoDeveSerAbertoPeloEmbarcador || motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao))
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.ClienteObrigatorio);

            if (chamado.Tomador == null && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.TomadorObrigatorio);

            if (chamado.Destinatario == null && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.DestinatarioObrigatorio);

            if (chamado.Empresa == null)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.EmpresaObrigatoria);

            chamado.Carga = carga;
            chamado.MotivoChamado = motivoChamado;
            chamado.Observacao = Request.GetNullableStringParam("Observacao");
            chamado.Valor = Request.GetDecimalParam("Valor");
            chamado.NumeroPallet = Request.GetDecimalParam("NumeroPallet");
            chamado.QuantidadeItens = Request.GetDecimalParam("QuantidadeItens");
            chamado.ResponsavelChamado = Request.GetEnumParam<ResponsavelChamado>("ResponsavelChamado");
            chamado.RetencaoBau = Request.GetBoolParam("RetencaoBau");
            chamado.DataReentrega = dataReentrega;
            chamado.DataRetencaoFim = dataRetencaoFim;
            chamado.DataRetencaoInicio = dataRetencaoInicio;
            chamado.TipoCliente = tipoCliente;
            chamado.ValorReferencia = Request.GetDecimalParam("ValorReferencia");
            chamado.ValorDesconto = Request.GetDecimalParam("ValorDesconto");
            chamado.PagoPeloMotorista = Request.GetBoolParam("PagoPeloMotorista");
            chamado.SaldoDescontadoMotorista = Request.GetDecimalParam("SaldoDescontadoMotorista");
            chamado.ModeloVeicularCarga = modeloVeicularCarga;
            chamado.TiposCausadoresOcorrencia = codigoTiposCausadoresOcorrencia > 0 ? repositorioTiposCausadoresOcorrencia.BuscarPorCodigo(codigoTiposCausadoresOcorrencia, false) : null;
            chamado.CausasMotivoChamado = codigoCausasMotivoChamado > 0 ? repositorioMotivoChamadoCausas.BuscarPorCodigo(codigoCausasMotivoChamado, false) : null;

            if (codigoMotivoDaDevolucao > 0)
                chamado.MotivoDaDevolucao = repositorioMotivoDevolucaoEntrega.BuscarPorCodigo(codigoMotivoDaDevolucao);

            if (devolucaoParcial)
                chamado.DevolucaoParcial = devolucaoParcial;

            if (carga != null && (chamado.Cliente != null || chamado.Destinatario != null))
                chamado.CargaEntrega = repCargaEntrega.BuscarPorClienteECarga(carga.Codigo, (chamado.Cliente ?? chamado.Destinatario).CPF_CNPJ);

            if (chamado.CargaEntrega == null && chamado.Carga != null)
                chamado.CargaEntrega = (chamado.Carga?.Pedidos?.Any(x => x.Recebedor != null) ?? true) ? repCargaEntrega.BuscarPorClienteRecebedor(chamado.Carga.Codigo, chamado.Carga.Pedidos.Where(o => o.Recebedor != null).Select(o => o.Recebedor.CPF_CNPJ).FirstOrDefault()) : null;

            if (configuracaoChamado.PermitirGerarAtendimentoPorPedido && chamado.Carga != null && chamado.Pedido != null)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidoVinculadosCarga = repositorioPedido.BuscarPedidosPorCarga(chamado.Carga.Codigo);

                bool ehPedidoViculadoNaCarga = listaPedidoVinculadosCarga.Any(p => p.Codigo == chamado.Pedido.Codigo);

                //Válida se o Pedido está vinculado há a carga informada
                if (!ehPedidoViculadoNaCarga)
                    throw new ControllerException($"O Pedido: {chamado.Pedido.NumeroPedidoEmbarcador} não está vinculado a Carga: {chamado.Carga.CodigoCargaEmbarcador}");
            }

        }

        private void PreencherChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Chamados/ChamadoOcorrencia");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamadoCausas repositorioMotivoChamadoCausas = new Repositorio.Embarcador.Chamados.MotivoChamadoCausas(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento repositorioTipoCriticidade = new Repositorio.Embarcador.Chamados.MotivoChamadoTipoCriticidadeAtendimento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repositorioConfiguracaoChamado.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            int codigoCarga = Request.GetIntParam("Carga");
            int codigoPedido;
            int codigoMotivoChamado = Request.GetIntParam("MotivoChamado");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoGrupoPessoasResponsavel = Request.GetIntParam("GrupoPessoasResponsavel");
            int codigoRealMotivo = Request.GetIntParam("RealMotivo");
            int codigoXMLNotaFiscal = Request.GetIntParam("NotaFiscal");

            double cpfCnpjCliente = Request.GetDoubleParam("Cliente");
            double cpfCnpjTomador = Request.GetDoubleParam("Tomador");
            double cpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");
            double cpfCnpjClienteResponsavel = Request.GetDoubleParam("ClienteResponsavel");
            int codigoTiposCausadoresOcorrencia = Request.GetIntParam("TiposCausadoresOcorrencia");
            int codigoCausasMotivoChamado = Request.GetIntParam("CausasMotivoChamado");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                codigoPedido = Request.GetIntParam("PedidoEmbarcador");
            else
                codigoPedido = Request.GetIntParam("Pedido");

            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamadoAnterior = chamado.MotivoChamado;
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = codigoMotivoChamado > 0 ? repositorioMotivoChamado.BuscarPorCodigo(codigoMotivoChamado) : null;
            if (motivoChamado == null)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.MotivoObrigatorio);


            Dominio.Entidades.Embarcador.Cargas.Carga carga = codigoCarga > 0 ? repositorioCarga.BuscarPorCodigo(codigoCarga) : null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                if (codigoPedido == 0)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.PedidoObrigatorio);

                carga = repositorioCarga.BuscarPorPedido(codigoPedido);

                if (carga == null && !motivoChamado.PermiteAtendimentoSemCarga)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NaoExisteCargaVinculadaPedido);

                chamado.Pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido() { Codigo = codigoPedido };
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && configuracaoChamado.PermitirGerarAtendimentoPorPedido)
            {
                if (carga != null)
                    chamado.Carga = carga;

                chamado.Pedido = codigoPedido > 0 ? repPedido.BuscarPorCodigo(codigoPedido) : null;
            }
            else
            {
                if (carga == null && !motivoChamado.PermiteAtendimentoSemCarga)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.CargaObrigatoria);
            }

            if (carga != null && !servicoCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
                chamado.VeiculoCarregado = true;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                chamado.Empresa = this.Empresa;
            else
                chamado.Empresa = carga?.Empresa ?? Empresa;

            if (cpfCnpjCliente > 0)
                chamado.Cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

            if (cpfCnpjTomador > 0)
                chamado.Tomador = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

            if (cpfCnpjDestinatario > 0)
                chamado.Destinatario = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

            chamado.ClienteResponsavel = cpfCnpjClienteResponsavel > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjClienteResponsavel) : null;
            chamado.GrupoPessoasResponsavel = codigoGrupoPessoasResponsavel > 0 ? repositorioGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoasResponsavel) : null;
            chamado.TipoPessoaResponsavel = Request.GetNullableEnumParam<TipoPessoa>("TipoPessoaResponsavel");
            chamado.XMLNotaFiscal = codigoXMLNotaFiscal > 0 ? repositorioXMLNotaFiscal.BuscarPorCodigo(codigoXMLNotaFiscal) : null;
            chamado.Motorista = codigoMotorista > 0 ? repositorioUsuario.BuscarPorCodigo(codigoMotorista) : null;

            chamado.RealMotivo = codigoRealMotivo > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoRealMotivo) : null;

            if (motivoChamadoAnterior.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao && motivoChamado.TipoMotivoAtendimento != TipoMotivoAtendimento.Devolucao)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NaoEPermitidoTrocarMotivoPorOutroQueNaoSejaDevolucao);

            if (chamado.Cliente == null && !motivoChamado.ChamadoDeveSerAbertoPeloEmbarcador)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.ClienteObrigatorio);

            if (chamado.Destinatario == null && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.DestinatarioObrigatorio);

            if (chamado.Tomador == null && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.TomadorObrigatorio);

            if (chamado.Empresa == null)
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.EmpresaObrigatoria);

            if (codigoRealMotivo == 0 && (motivoChamado?.ObrigarRealMotivo ?? false))
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NecessarioInformarRealMotivo);

            chamado.MotivoChamado = motivoChamado;
            chamado.Observacao = Utilidades.String.RemoveAllSpecialCharactersNotCommon(Request.GetStringParam("Observacao"));

            DateTime.TryParse(Request.Params("DataRetencaoInicio"), out DateTime dataRetencaoInicio);
            DateTime.TryParse(Request.Params("DataRetencaoFim"), out DateTime dataRetencaoFim);
            DateTime.TryParse(Request.Params("DataReentrega"), out DateTime dataReentrega);

            Dominio.Enumeradores.TipoTomador tipoCliente = Dominio.Enumeradores.TipoTomador.Destinatario;
            if (Enum.TryParse(Request.Params("TipoCliente"), out Dominio.Enumeradores.TipoTomador tipoClienteAux))
                tipoCliente = tipoClienteAux;

            chamado.RetencaoBau = Request.GetBoolParam("RetencaoBau");
            chamado.TipoCliente = tipoCliente;
            if (dataReentrega > DateTime.MinValue)
                chamado.DataReentrega = dataReentrega;
            if (dataRetencaoInicio > DateTime.MinValue)
                chamado.DataRetencaoInicio = dataRetencaoInicio;
            if (dataRetencaoFim > DateTime.MinValue)
                chamado.DataRetencaoFim = dataRetencaoFim;

            decimal valorAnterior = chamado.Valor;

            if (motivoChamado.ExigeValorNaLiberacao)
                chamado.Valor = Request.GetDecimalParam("Valor");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                chamado.Carga = carga;
                chamado.Valor = Request.GetDecimalParam("Valor");
                chamado.NumeroPallet = Request.GetDecimalParam("NumeroPallet");
                chamado.QuantidadeItens = Request.GetDecimalParam("QuantidadeItens");
                chamado.ResponsavelChamado = Request.GetEnumParam<ResponsavelChamado>("ResponsavelChamado");
                chamado.GerarCargaDevolucao = motivoChamado.GerarCargaDevolucaoSeAprovado;
                chamado.ValorReferencia = Request.GetDecimalParam("ValorReferencia");
                chamado.PagoPeloMotorista = Request.GetBoolParam("PagoPeloMotorista");
                chamado.SaldoDescontadoMotorista = Request.GetDecimalParam("SaldoDescontadoMotorista");
            }

            chamado.ValorDesconto = Request.GetDecimalParam("ValorDesconto");
            chamado.TiposCausadoresOcorrencia = codigoTiposCausadoresOcorrencia > 0 ? repositorioTiposCausadoresOcorrencia.BuscarPorCodigo(codigoTiposCausadoresOcorrencia, false) : null;
            chamado.CausasMotivoChamado = codigoCausasMotivoChamado > 0 ? repositorioMotivoChamadoCausas.BuscarPorCodigo(codigoCausasMotivoChamado, false) : null;

            bool critico = Request.GetBoolParam("Critico");
            int codigoGerencial = Request.GetIntParam("CodigoGerencial");
            int codigoCausaProblema = Request.GetIntParam("CodigoCausaProblema");
            string fup = Request.GetStringParam("FUP");

            chamado.Critico = critico;
            chamado.Gerencial = codigoGerencial > 0 ? repositorioTipoCriticidade.BuscarPorCodigoAsync(codigoGerencial).Result : null;
            chamado.CausaProblema = codigoCausaProblema > 0 ? repositorioTipoCriticidade.BuscarPorCodigoAsync(codigoCausaProblema).Result : null;
            chamado.FUP = fup;

            if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ChamadoOcorrencia_PermitirAlterarValorEMotivoChamado))
            {
                if (valorAnterior != chamado.Valor)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.VocenaoPossuiPermissaoParaAlteraroValorDoAtendimento);
                if (motivoChamadoAnterior.Codigo != motivoChamado.Codigo)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.VoceNaoPossuiPermissaoParaAlterarMotivoDoAtendimento);
            }

            if (carga != null && (chamado.Cliente != null || chamado.Destinatario != null))
                chamado.CargaEntrega = repCargaEntrega.BuscarPorClienteECarga(carga.Codigo, (chamado.Cliente ?? chamado.Destinatario).CPF_CNPJ);

            if (chamado.CargaEntrega != null)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;
                cargaEntrega.Initialize();
                cargaEntrega.DataEntradaRaio = Request.GetNullableDateTimeParam("DataEntradaRaio");
                cargaEntrega.DataSaidaRaio = Request.GetNullableDateTimeParam("DataSaidaRaio");

                if (configuracao.VisualizarDatasRaioNoAtendimento && cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue && cargaEntrega.DataEntradaRaio > cargaEntrega.DataSaidaRaio)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.DataDeChegadaNaoPodeSerMaiorQueDeSaida);

                repositorioCargaEntrega.Atualizar(cargaEntrega, Auditado);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);


                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = cargaEntrega.GetChanges();
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, alteracoes, Localization.Resources.Chamado.ChamadoOcorrencia.AlterouInformacoesEntrega, unitOfWork);

            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegraAtendimentoChamado = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(unitOfWork);

            int codigoPedido;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                codigoPedido = Request.GetIntParam("PedidoEmbarcador");
            else
                codigoPedido = Request.GetIntParam("Pedido");

            Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado()
            {
                CodigoCarga = Request.GetIntParam("CargaCodigo"),
                CodigoPedido = codigoPedido,
                CodigoCargaEmbarcador = Request.GetStringParam("Carga"),
                CodigosMotivoChamado = Request.GetListParam<int>("MotivoChamado"),
                CodigoResponsavel = Request.GetIntParam("Responsavel"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjCliente = Request.GetDoubleParam("Cliente"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CodigoGrupoPessoasCliente = Request.GetIntParam("GrupoPessoasCliente"),
                CodigoGrupoPessoasTomador = Request.GetIntParam("GrupoPessoasTomador"),
                CodigoGrupoPessoasDestinatario = Request.GetIntParam("GrupoPessoasDestinatario"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                NotaFiscal = Request.GetIntParam("NotaFiscal"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                SomenteValoresPendentes = Request.GetBoolParam("SomenteValoresPendentes"),
                SituacaoChamado = Request.GetEnumParam("SituacaoChamado", SituacaoChamado.Todas),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                CodigosOcorrencia = Request.GetListParam<int>("Ocorrencia"),
                ComOcorrencia = Request.GetEnumParam("ComOcorrencia", Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos),
                ModalTransporte = Request.GetEnumParam("TipoCobrancaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum),
                CodigoSetor = Request.GetIntParam("Setor"),
                ComDevolucao = Request.GetNullableBoolParam("ComDevolucao"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CpfCnpjClienteResponsavel = Request.GetDoubleParam("ClienteResponsavel"),
                CodigoGrupoPessoasResponsavel = Request.GetIntParam("GrupoPessoasResponsavel"),
                ComNotaFiscalServico = Request.GetNullableBoolParam("ComNotaFiscalServico"),
                ComResponsavel = Request.GetNullableBoolParam("ComResponsavel"),
                ComNovaMovimentacao = Request.GetNullableBoolParam("ComNovaMovimentacao"),
                NumeroLote = Request.GetIntParam("NumeroLote"),
                DataInicialAgendamentoPedido = Request.GetNullableDateTimeParam("DataInicialAgendamentoPedido"),
                DataFinalAgendamentoPedido = Request.GetNullableDateTimeParam("DataFinalAgendamentoPedido"),
                DataInicialColetaPedido = Request.GetNullableDateTimeParam("DataInicialColetaPedido"),
                DataFinalColetaPedido = Request.GetNullableDateTimeParam("DataFinalColetaPedido"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                ClienteComplementar = Request.GetListParam<double>("ClienteComplementar"),
                SomenteCargasCriticas = Request.GetBoolParam("SomenteCargasCriticas"),
                SomenteAtendimentoComMsgNaoLida = Request.GetBoolParam("SomenteAtendimentoComMsgNaoLida"),
                CanalVenda = Request.GetIntParam("CanalVenda"),
                SetorEscalationList = Request.GetListParam<int>("SetorEscalationList"),
                Vendedores = Request.GetListParam<int>("Vendedores"),
                MesoRegiao = Request.GetListParam<int>("MesoRegiao"),
                Regiao = Request.GetListParam<int>("Regiao"),
                UFDestino = Request.GetListParam<string>("UFDestino"),
                EscritorioVendas = Request.GetStringParam("EscritorioVendas"),
                Matriz = Request.GetStringParam("Matriz"),
                Parqueada = Request.GetNullableBoolParam("Parqueada"),
                CodigoTiposCausadoresOcorrencia = Request.GetIntParam("TiposCausadoresOcorrencia"),
                CodigoCausasMotivoCamado = Request.GetIntParam("CausasMotivoChamado"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;
            else
                filtrosPesquisa.CodigosTransportador = Request.GetListParam<int>("Transportador");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                filtrosPesquisa.AguardandoTratativaDoCliente = true;
                filtrosPesquisa.FornecedorLogado = this.Usuario.Cliente?.CPF_CNPJ ?? 0;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && repositorioRegraAtendimentoChamado.BuscarSeExisteRegra())
                filtrosPesquisa.CodigoResponsavelPorRegra = this.Usuario.Codigo;

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosFilialVenda = Request.GetListParam<int>("FilialVenda");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilialVenda = codigosFilialVenda.Count == 0 ? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork) : codigosFilialVenda;
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacao.Count == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTipoOperacao;

            return filtrosPesquisa;
        }

        private async Task<Models.Grid.Grid> ObterGridPesquisa(int limitePesquisa)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ConfiguracaoEmbarcador;

                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repositorioConfiguracaoChamado.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>(),
                    scrollHorizontal = true
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.MensagemNaoLida, "MensagemNaoLida", 7, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 7, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Carga, "Carga", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.TempoChamado, "TempoChamado", 5, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Pedido, "Pedido", 7, Models.Grid.Align.left, true);
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && configuracaoChamado.PermitirGerarAtendimentoPorPedido)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.PedidoEmbarcador, "PedidoEmbarcador", 7, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.MotivoChamado, "MotivoChamado", 6, Models.Grid.Align.left, true);

                if (!configuracaoEmbarcador.NaoExibirInfosAdicionaisGridPatio)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Ocorrencia, "NumeroOcorrencia", 7, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SituacaoOcorrencia, "SituacaoOcorrencia", 7, Models.Grid.Align.left, false);
                }

                if (configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.CargasAgrupadas, "CodigosAgrupadosCarga", 6, Models.Grid.Align.left, false, false, false, false, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.TipoMotivo, "TipoMotivo", 4, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.EmpresaFilial, "Transportador", 9, Models.Grid.Align.left, false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Transportador, "Transportador", 9, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Responsavel, "Responsavel", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SetorResponsavel, "SetorResponsavel", 6, Models.Grid.Align.left, true);

                if (!configuracaoEmbarcador.NaoExibirPessoasChamado)
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                        grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Cliente, "Cliente", 9, Models.Grid.Align.left, true);

                    if (!configuracaoEmbarcador.NaoExibirInfosAdicionaisGridPatio)
                        grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Tomador, "Tomador", 9, Models.Grid.Align.left, true);

                    if (!configuracaoEmbarcador.NaoExibirInfosAdicionaisGridPatio)
                        grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Destinatario, "Destinatario", 9, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataCriacao, "DataCriacao", 6, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataRetorno, "DataRetorno", 6, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("DataRetorno", false);

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 5, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.AosCuidadosDo, "AosCuidadosDo", 5, Models.Grid.Align.left, true);

                if (configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.OperadorContratouCarga, "OperadorContratouCarga", 6, Models.Grid.Align.left, false, false, false, false, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.PesoNotas, "PesoNotasFiscais", 6, Models.Grid.Align.left, false, false, false, false, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Pedidos, "NumeroPedidos", 6, Models.Grid.Align.left, false, false, false, false, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Vendedores, "Vendedores", 6, Models.Grid.Align.left, false, false, false, false, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Observacao, "Observacao", 7, Models.Grid.Align.left, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.TratativaDoChamado, "TratativaChamado", 7, Models.Grid.Align.left, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.TipoDeDevolucao, "TipoDevolucao", 7, Models.Grid.Align.left, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.TipoCarga, "TipoCarga", 7, Models.Grid.Align.left, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Veiculo, "Veiculo", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Origem, "Origem", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Destino, "Destino", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NotasFiscais, "NotasFiscais", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Filial, "Filial", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NumeroFrotaReboque, "NumeroFrotaReboque", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Analise, "Analise", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Motorista, "Motorista", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NumeroLote, "NumeroLote", 6, Models.Grid.Align.left, false, false);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataAgendamento, "DataAgendamento", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataColeta, "DataColeta", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.EscritorioVendas, "EscritorioVendas", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Matriz, "Matriz", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NumeroPedidoCliente, "NumeroPedidoCliente", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.CanalVenda, "CanalVenda", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte, "ModalTransporte", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.SetorEscalationList, "SetorEscalationList", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Mesoregiao, "Mesorregiao", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Regiao, "Regiao", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.UFDestino, "UFDestino", 6, Models.Grid.Align.left, false, false);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Parqueada, "Parqueada", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.CausadorOcorrencia, "TiposCausadoresOcorrencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.MotivoChamado.CausasMotivoAtendimento, "CausasMotivoChamado", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Estadia, "Estadia", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Senha, "SenhaDevolucao", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Critico, "Critico", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Gerencial, "Gerencial", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.CausaProblema, "CausaProblema", 6, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.FUP, "FUP", 6, Models.Grid.Align.left, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ChamadoOcorrencia/Pesquisa", "grid-pesquisa-chamados");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                parametrosConsulta.LimiteRegistros = limitePesquisa != 0 ? limitePesquisa : grid.limite;
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoOcorrencia repositorioChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repMotivoChamadoGatilhos = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(unitOfWork);
                Repositorio.Embarcador.Cargas.ChatMobileMensagem repositorioChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();


                int totalRegistros = repositorioChamado.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamado = (totalRegistros > 0) ? repositorioChamado.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.Chamado>();

                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> motivoChamadoGatilhosTempoList = repMotivoChamadoGatilhos.BuscarPorNiveisEMotivosChamado(listaChamado.Select(chamado => chamado.MotivoChamado.Codigo).ToList(), listaChamado.Select(chamado => chamado.Nivel).ToList());

                List<Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar> clienteComplementars = await repositorioCargaPedido.BuscarClientesComplementaresPorCargas(listaChamado.Select(chamado => chamado.Carga.Codigo).ToList());

                dynamic listaChamadoRetornar = new List<dynamic>();

                foreach (Chamado chamado in listaChamado)
                {

                    listaChamadoRetornar.Add(new
                    {
                        chamado.Codigo,
                        chamado.Numero,
                        Carga = chamado?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        CodigoCarga = chamado?.Carga?.Codigo ?? 0,
                        Cliente = chamado.Cliente?.Descricao ?? string.Empty,
                        Tomador = chamado.Tomador?.Descricao ?? string.Empty,
                        Destinatario = chamado.Destinatario?.Descricao ?? string.Empty,
                        Transportador = chamado.Empresa?.Descricao ?? string.Empty,
                        Responsavel = chamado.Responsavel?.Nome ?? string.Empty,
                        SetorResponsavel = chamado.Responsavel?.Setor?.Descricao ?? string.Empty,
                        MotivoChamado = chamado.MotivoChamado.Descricao,
                        TipoMotivo = chamado.MotivoChamado.TipoMotivoAtendimento.ObterDescricao(),
                        DataCriacao = chamado.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                        DataRetorno = chamado.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        TempoChamado = this.ObterTempoChamado(chamado),
                        AosCuidadosDo = chamado.DescricaoAosCuidadosDo,
                        Situacao = chamado.DescricaoSituacao,
                        NumeroOcorrencia = repositorioChamadoOcorrencia.BuscarNumerosOcorrenciasPorChamado(chamado.Codigo),
                        SituacaoOcorrencia = string.Join(", ", repositorioChamadoOcorrencia.BuscarOcorrenciasPorChamado(chamado.Codigo).Select(x => x.SituacaoOcorrencia.ObterDescricao()).ToArray()),
                        OperadorContratouCarga = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || chamado.Carga == null ? "" : chamado.Carga?.OperadorContratouCarga?.Descricao ?? chamado.Carga?.Operador?.Descricao ?? string.Empty,
                        CodigosAgrupadosCarga = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || chamado.Carga == null ? "" : string.Join(", ", chamado.Carga.CodigosAgrupados),
                        PesoNotasFiscais = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || chamado.Carga == null ? "" : repositorioPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(chamado.Carga.Codigo).ToString("n2"),
                        NumeroPedidos = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || chamado.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarNumeroPedidosPorCarga(chamado.Carga.Codigo)),
                        Vendedores = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || chamado.Carga == null ? "" : string.Join(", ", repositorioCargaPedido.BuscarNomeFuncionariosVendedoresPorCarga(chamado.Carga.Codigo)),
                        DT_RowColor = this.ConfigCor(chamado, configChamado, repositorioChamadoOcorrencia.BuscarOcorrenciasPorChamado(chamado.Codigo).OrderByDescending(x => x.Codigo).FirstOrDefault()?.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada),
                        chamado.Observacao,
                        TratativaChamado = chamado.DescricaoSituacao != "Aberto" ? chamado.TratativaDevolucao.ObterDescricaoTratativaDevolucao() : string.Empty,
                        TipoDevolucao = chamado.DescricaoSituacao != "Aberto" ? chamado.CargaEntrega?.TipoDevolucao.ObterDescricao() : string.Empty,
                        TipoCarga = chamado?.Carga?.TipoDeCarga?.Descricao ?? string.Empty,
                        Veiculo = chamado?.Carga?.DadosSumarizados?.Veiculos ?? chamado.Carga?.PlacasVeiculos ?? string.Empty,
                        Origem = chamado?.Carga?.DadosSumarizados?.Origens ?? string.Empty,
                        Destino = chamado?.Carga?.DadosSumarizados?.Destinos ?? string.Empty,
                        NotasFiscais = string.Join(", ", chamado.XMLNotasFiscais?.Select(o => o.Numero).ToList()) ?? string.Empty,
                        Filial = chamado?.Carga?.Filial?.Descricao ?? string.Empty,
                        AguardandoTratativaDoCliente = !chamado?.AguardandoTratativaDoCliente ?? false,
                        NumeroFrotaReboque = chamado.Carga?.DadosSumarizados?.NumeroFrotasVeiculos ?? string.Empty,
                        Analise = (chamado?.NovaMovimentacao ?? false) ? Localization.Resources.Cargas.ControleEntrega.NovaMovimentacao : string.Empty,
                        Motorista = chamado?.Carga?.Motoristas.FirstOrDefault()?.Nome,
                        NumeroLote = chamado.LoteChamadoOcorrencia?.NumeroLote.ToString() ?? string.Empty,
                        Pedido = RetornarPedidoChamado(configChamado, chamado, unitOfWork)?.NumeroPedidoEmbarcador ?? string.Empty,
                        PedidoEmbarcador = RetornarPedidoChamado(configChamado, chamado, unitOfWork)?.NumeroPedidoEmbarcador ?? string.Empty,
                        DataAgendamento = RetornarPedidoChamado(configChamado, chamado, unitOfWork)?.DataAgendamento?.ToDateTimeString() ?? string.Empty,
                        DataColeta = RetornarPedidoChamado(configChamado, chamado, unitOfWork)?.DataCarregamentoPedido?.ToDateTimeString() ?? string.Empty,
                        NumeroPedidoCliente = RetornarPedidoChamado(configChamado, chamado, unitOfWork)?.CodigoPedidoCliente ?? string.Empty,
                        EscritorioVendas = clienteComplementars.Where(clienteComplementar => clienteComplementar.CpfCnpjCliente == chamado.Cliente.CPF_CNPJ).Select(clienteComplementar => clienteComplementar.EscritorioVendas).FirstOrDefault() ?? string.Empty,
                        Matriz = clienteComplementars.Where(clienteComplementar => clienteComplementar.CpfCnpjCliente == chamado.Cliente.CPF_CNPJ).Select(clienteComplementar => clienteComplementar.Matriz).FirstOrDefault() ?? string.Empty,
                        CanalVenda = chamado.Pedido?.CanalVenda?.Descricao ?? string.Empty,
                        ModalTransporte = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || chamado.Carga == null ? "" : TipoCobrancaMultimodalHelper.ObterDescricao(repositorioCargaPedido.RetonrarCobrancaMultiModalCarga(chamado.Carga.Codigo)),
                        SetorEscalationList = motivoChamadoGatilhosTempoList.Where(motivo => motivo.MotivoChamado.Codigo == chamado.MotivoChamado.Codigo && motivo.Nivel == chamado.Nivel).FirstOrDefault()?.Setor?.Descricao ?? string.Empty,
                        Mesorregiao = chamado.Cliente?.MesoRegiao?.Descricao ?? string.Empty,
                        Regiao = chamado.Cliente?.Regiao?.Descricao ?? string.Empty,
                        UFDestino = chamado.Cliente?.Localidade?.Estado?.Descricao ?? string.Empty,
                        MensagemNaoLida = chamado.Carga != null ? repositorioChatMobileMensagem.BuscarPorCarga(chamado.Carga.Codigo).Any(x => x.MensagemLida == false) ? "Sim" : "Não" : string.Empty,
                        Parqueada = chamado.Carga != null ? chamado.Carga.Parqueada.HasValue ? chamado.Carga.Parqueada.Value ? "Sim" : "Não" : string.Empty : string.Empty,
                        TiposCausadoresOcorrencia = chamado.TiposCausadoresOcorrencia?.Descricao ?? string.Empty,
                        CausasMotivoChamado = chamado.CausasMotivoChamado?.Descricao ?? string.Empty,
                        Estadia = chamado.Estadia.ObterDescricao() ?? string.Empty,
                        SenhaDevolucao = chamado.SenhaDevolucao ?? string.Empty,
                        Critico = chamado.Critico,
                        Gerencial = chamado.Gerencial?.Conteudo ?? string.Empty,
                        CausaProblema = chamado.CausaProblema?.Conteudo ?? string.Empty,
                        FUP = chamado.FUP ?? string.Empty
                    });
                }

                grid.AdicionaRows(listaChamadoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Models.Grid.Grid ObterGridPesquisaChamado()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ConfiguracaoEmbarcador;

            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Carga, "Carga", 7, Models.Grid.Align.left, true);

                if (configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.CargasAgrupadas, "CodigosAgrupadosCarga", 9, Models.Grid.Align.left, false, false, false, false, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.NotaFiscal, "NumeroNotaFiscal", 9, Models.Grid.Align.left, false, false, false, false, true);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Filial, "Filial", 12, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Cliente, "Cliente", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Tomador, "Tomador", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Valor, "Valor", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Observacao, "Observacao", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.MotivoChamado, "MotivoChamado", 12, Models.Grid.Align.left, true);

                if (configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Veiculo, "Veiculo", 12, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.EmpresaFilial, "Transportador", 12, Models.Grid.Align.left, false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Transportador, "Transportador", 12, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataRetorno, "DataRetorno", 9, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamado filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                int totalRegistros = repositorioChamado.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamado = (totalRegistros > 0) ? repositorioChamado.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.Chamado>();

                var listaChamadoRetornar = (
                    from obj in listaChamado
                    select new
                    {
                        obj.Codigo,
                        CodigoCarga = obj.Carga?.Codigo ?? 0,
                        obj.Numero,
                        obj.Descricao,
                        Carga = obj.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        CodigosAgrupadosCarga = configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado && obj.Carga != null ? string.Join(",", obj.Carga.CodigosAgrupados) : "",
                        Cliente = obj.Cliente?.Nome ?? "",
                        Tomador = obj.Tomador?.Nome ?? "",
                        Destinatario = obj.Destinatario?.Nome ?? "",
                        DataRetorno = obj.DataRetorno?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        Filial = obj.Carga?.Filial?.Descricao ?? string.Empty,
                        NumeroNotaFiscal = configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado && obj.Carga != null ? string.Join(",", repositorioPedidoXMLNotaFiscal.ObterNumerosNotasPorCarga(obj.Carga.Codigo)) : "",
                        MotivoChamado = obj.MotivoChamado.Descricao,
                        Transportador = obj.Empresa?.Descricao ?? "",
                        Veiculo = configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado && obj.Carga != null ? obj.Carga.RetornarPlacas : "",
                        Valor = obj.Valor.ToString("n2"),
                        obj.Observacao
                    }
                ).ToList();

                grid.AdicionaRows(listaChamadoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            switch (propriedadeOrdenar)
            {
                case "Carga":
                    return "Carga.CodigoCargaEmbarcador";
                case "Cliente":
                    return "Cliente.Nome";
                case "Tomador":
                    return "Tomador.Nome";
                case "Destinatario":
                    return "Destinatario.Nome";
                case "Analise":
                    return "NovaMovimentacao";
                case "TempoChamado":
                    return "DataCriacao";
                case "MotivoChamado":
                    return "MotivoChamado.Descricao";
                case "TipoMotivo":
                    return "MotivoChamado.TipoMotivoAtendimento";
                case "Responsavel":
                    return "Responsavel.Nome";
                case "SetorResponsavel":
                    return "Responsavel.Setor.Descricao";
                case "TipoCarga":
                    return "Carga.TipoDeCarga.Descricao";
                case "Veiculo":
                    return "Carga.DadosSumarizados.Veiculos";
                case "Origem":
                    return "Carga.DadosSumarizados.Origens";
                default:
                    return propriedadeOrdenar;
            }
        }

        private string ObterTempoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            DateTime dataFinalizacao = DateTime.Now;

            if ((chamado.Situacao == SituacaoChamado.Finalizado || chamado.Situacao == SituacaoChamado.RecusadoPeloCliente) && chamado.DataFinalizacao.HasValue)
                dataFinalizacao = chamado.DataFinalizacao.Value;

            TimeSpan tempoTotal = (dataFinalizacao - chamado.DataCriacao);

            int dias = tempoTotal.Days;
            string tempo = "";

            if (dias > 0)
                tempo += $"{dias} dia{(dias > 1 ? "s" : "")} ";

            tempo += tempoTotal.ToString(@"hh\:mm");

            return tempo.Trim();
        }

        private byte[] GerarRelatorioEmbarcador(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, out string msg)
        {
            ReportResult result = ReportRequest.WithType(ReportType.DevolucaoMercadoria)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoChamado", chamado.Codigo.ToString())
                .CallReport();

            msg = result.ErrorMessage;

            return result.GetContentFile();
        }

        private string ConfigCor(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configChamado, bool ultimaOcorrenciaRejeitada)
        {
            if (chamado.Situacao == SituacaoChamado.AgIntegracao)
                return ClasseCorCustomizada.Fundo("#C2A4FF");

            if (ultimaOcorrenciaRejeitada || chamado.Situacao == SituacaoChamado.Cancelada)
                return ClasseCorCustomizada.Fundo("#FFD3D3");

            if (chamado.Situacao == SituacaoChamado.SemRegra)
                return ClasseCorCustomizada.Fundo("#FEE5AD");

            if (chamado.Situacao == SituacaoChamado.Aberto && chamado.Responsavel != null)
                return ClasseCorCustomizada.Fundo("#B3EBEF");

            if (chamado.Situacao == SituacaoChamado.Aberto && ((DateTime.Now - chamado.DataCriacao).Days >= 2))
                return ClasseCorCustomizada.Fundo("#FFC5A8");

            if (chamado.Situacao != SituacaoChamado.Aberto && chamado.Situacao != SituacaoChamado.EmTratativa)
                return ClasseCorCustomizada.Fundo("#C3E5A9");

            if (chamado.Situacao != SituacaoChamado.LiberadaOcorrencia)
                return ClasseCorCustomizada.Fundo("#AACFEB");

            if ((chamado.Situacao != SituacaoChamado.Finalizado && chamado.Situacao != SituacaoChamado.RecusadoPeloCliente))
                return ClasseCorCustomizada.Fundo("#CDCDCD");

            return string.Empty;
        }

        private string CorPorPrazoMultiCTe(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            if (chamado.Situacao != SituacaoChamado.Aberto && chamado.Situacao != SituacaoChamado.EmTratativa)
                return "";

            if (chamado.AosCuidadosDo == ChamadoAosCuidadosDo.Embarcador)
                return "";

            if (!chamado.DataRetorno.HasValue)
                return "";

            DateTime dataRetorno = chamado.DataRetorno.Value;

            DateTime aviso15Minutos = dataRetorno.AddMinutes(-15);
            DateTime aviso5Minutos = dataRetorno.AddMinutes(-5);

            if (aviso5Minutos < DateTime.Now)
                return CorGrid.Danger;

            if (aviso15Minutos < DateTime.Now)
                return CorGrid.Warning;

            return "";
        }

        private void SalvarChamadoDatas(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoData repChamadoData = new Repositorio.Embarcador.Chamados.ChamadoData(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamadoData repMotivoChamadoData = new Repositorio.Embarcador.Chamados.MotivoChamadoData(unitOfWork);

            dynamic dynDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Datas"));

            if (chamado.Datas != null && chamado.Datas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic data in dynDatas)
                    if (data.Codigo != null && data.Codigo != 0)
                        codigos.Add((int)data.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoData> motivoChamadoDataDeletar = (from obj in chamado.Datas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < motivoChamadoDataDeletar.Count; i++)
                    repChamadoData.Deletar(motivoChamadoDataDeletar[i]);
            }
            else
                chamado.Datas = new List<Dominio.Entidades.Embarcador.Chamados.ChamadoData>();

            foreach (dynamic data in dynDatas)
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoData chamadoData = data.Codigo != null && data.Codigo != 0 ? repChamadoData.BuscarPorCodigo((int)data.Codigo, false) : null;
                if (chamadoData == null)
                    chamadoData = new Dominio.Entidades.Embarcador.Chamados.ChamadoData();

                int codigoMotivoChamadoData = ((string)data.CodigoMotivoChamadoData).ToInt();
                chamadoData.DataInicio = ((string)data.DataInicio).ToNullableDateTime();
                chamadoData.DataFim = ((string)data.DataFim).ToNullableDateTime();

                if (chamadoData.DataInicio.HasValue && chamadoData.DataFim.HasValue && chamadoData.DataInicio > chamadoData.DataFim)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.DataInicialNaoPodeSerMaiorNoGrupoDeDatas);

                if (chamadoData.Codigo > 0)
                    repChamadoData.Atualizar(chamadoData);
                else
                {
                    chamadoData.Chamado = chamado;
                    chamadoData.MotivoChamadoData = repMotivoChamadoData.BuscarPorCodigo(codigoMotivoChamadoData, false);
                    repChamadoData.Inserir(chamadoData);
                }
            }
        }

        private void SalvarNotasFiscais(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            if (configuracaoOcorrencia.PermitirVinculoAutomaticoEntreOcorreciaEAtendimento)
                SalvarNotasFiscaisPelaCargaOuPedido(chamado, configuracaoChamado, unitOfWork);
            else
                SalvarNotasFiscaisFormaManualmente(chamado, configuracaoChamado, unitOfWork);

        }

        private void SalvarNotasFiscaisPelaCargaOuPedido(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal filtroPesquisaXMLNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal();
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            filtroPesquisaXMLNotaFiscal.CodigoCarga = chamado.Carga?.Codigo ?? 0;
            filtroPesquisaXMLNotaFiscal.CodigoPedido = chamado.Pedido?.Codigo ?? 0;
            filtroPesquisaXMLNotaFiscal.CodigoCliente = chamado.Cliente?.CPF_CNPJ ?? 0;

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repNotaFiscal.BuscarNotasPorCargaECliente(filtroPesquisaXMLNotaFiscal);

            if (notasFiscais == null || notasFiscais.Count <= 0)
                return;

            if (chamado.XMLNotasFiscais == null)
                chamado.XMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                    codigos.Add(notaFiscal.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDeletar = chamado.XMLNotasFiscais.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscalDeletar in notasFiscaisDeletar)
                    chamado.XMLNotasFiscais.Remove(notaFiscalDeletar);
            }

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
            {
                if (chamado.XMLNotasFiscais.Any(o => o.Codigo == notaFiscal.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscalObj = repNotaFiscal.BuscarPorCodigo((int)notaFiscal.Codigo);
                chamado.XMLNotasFiscais.Add(notaFiscalObj);
            }
        }

        private void SalvarNotasFiscaisFormaManualmente(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            dynamic notasFiscais = Request.Params("NotasFiscais") != null ? Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NotasFiscais")) : null;

            if (configuracaoChamado.ObrigatorioInformarNotaFiscalParaAberturaChamado && (notasFiscais == null || notasFiscais.Count <= 0))
                throw new ControllerException("Obrigatório informar notas fiscais para abertura de chamado.");

            if (notasFiscais == null || notasFiscais.Count <= 0)
                return;

            if (chamado.XMLNotasFiscais == null)
                chamado.XMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic notaFiscal in notasFiscais)
                    codigos.Add((int)notaFiscal.NFe.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDeletar = chamado.XMLNotasFiscais.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscalDeletar in notasFiscaisDeletar)
                    chamado.XMLNotasFiscais.Remove(notaFiscalDeletar);
            }

            foreach (dynamic notaFiscal in notasFiscais)
            {
                if (chamado.XMLNotasFiscais.Any(o => o.Codigo == (int)notaFiscal.NFe.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscalObj = repNotaFiscal.BuscarPorCodigo((int)notaFiscal.NFe.Codigo);
                chamado.XMLNotasFiscais.Add(notaFiscalObj);
            }
        }

        private void SalvarProdutosNFDevolucao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, bool devolucaoParcial, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

            dynamic itensDevolver = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensDevolver"));

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> notasDevolver = servicoControleEntrega.ConverterDevolucaoNotasFiscais(itensDevolver).GetAwaiter().GetResult();

            if (devolucaoParcial && notasDevolver.TrueForAll(nota => nota.DevolucaoTotal))
                devolucaoParcial = false;

            servicoControleEntrega.SalvarDevolucaoCargaEntrega(chamado.CargaEntrega, notasDevolver, servicoControleEntrega.ConverterDevolucaoProdutos(itensDevolver), chamado.MotivoChamado, chamado, ConfiguracaoEmbarcador, Auditado, chamado.TratativaDevolucao, devolucaoParcial, true, configuracaoChamado, TipoServicoMultisoftware, chamado.MotivoDaDevolucao, deveAtualizarSituacaoNota: false);

            if (chamado.CargaEntrega != null)
                chamado.CargaEntrega.DevolucaoParcial = devolucaoParcial;
        }

        private void SalvarChamadoPerfisAcesso(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);

            if (chamado.PerfisAcesso == null)
                chamado.PerfisAcesso = new List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();
            else
                chamado.PerfisAcesso.Clear();

            dynamic dynPerfisAcesso = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PerfisAcesso"));
            foreach (dynamic dynPerfilAcesso in dynPerfisAcesso)
            {
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = repPerfilAcesso.BuscarPorCodigo((int)dynPerfilAcesso.Codigo);
                chamado.PerfisAcesso.Add(perfilAcesso);
            }
        }

        private string ObterNumeroCelularCompleto(Usuario motorista)
        {
            if (motorista == null)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(motorista.Celular))
                return string.Empty;

            string celular = Utilidades.String.OnlyNumbers(motorista.Celular ?? string.Empty);

            bool numeroBrasileiro = motorista.Localidade == null || motorista.Localidade.Pais?.Abreviacao == "BR";

            return $"{(numeroBrasileiro ? "+55" : "")}{celular}";
        }

        private void ValidarESalvaDiariaAutomatica(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            if (chamado.MotivoChamado?.ObrigatorioTerDiariaAutomatica ?? false)
            {
                Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica = repDiariaAutomatica.BuscarPorCarga(chamado.Carga.Codigo);

                if (diariaAutomatica?.Chamado != null)
                {
                    List<SituacaoChamado> situacoesPermitidas = new List<SituacaoChamado> { SituacaoChamado.Finalizado, SituacaoChamado.Cancelada };
                    if (!situacoesPermitidas.Contains(diariaAutomatica.Chamado.Situacao))
                        throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.JaExisteUmchamadoRelacionadoDiariaAutomaticaDessaCarga);
                }

                if (diariaAutomatica == null)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NaoExisteUmaDiariaAutomaticaCadastradaParaEssaCarga);

                if (diariaAutomatica.LocalFreeTime != chamado.MotivoChamado.LocalFreeTime)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.NaoExisteUmaDiariaAutomaticaComMesmoTipoDeFreeTimeDoMotivoInformado);

                if (diariaAutomatica.Status != StatusDiariaAutomatica.Finalizada)
                    throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.ADiariaAutomaticaAindaNaoFoiFnalizada);

                if (chamado.MotivoChamado.DiasLimiteAberturaAposDiariaAutomatica > 0)
                {
                    double diasPassadosDepoisDaCarga = ObterDiasPassadosDepoisDaCarga(chamado.Carga, diariaAutomatica.LocalFreeTime);
                    if (diasPassadosDepoisDaCarga > chamado.MotivoChamado.DiasLimiteAberturaAposDiariaAutomatica)
                        throw new ControllerException(string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.OChamadoNaoPodeSerCriadoPorquejaSePassaramDesdeDataDaCarga, (int)diasPassadosDepoisDaCarga, chamado.MotivoChamado.DiasLimiteAberturaAposDiariaAutomatica));
                }

                diariaAutomatica.Chamado = chamado;
                repDiariaAutomatica.Atualizar(diariaAutomatica);
            }
        }

        private double ObterDiasPassadosDepoisDaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, LocalFreeTime localFreeTime)
        {
            if (carga.DataInicioViagem == null || carga.DataFimViagem == null)
            {
                throw new ControllerException(Localization.Resources.Chamado.ChamadoOcorrencia.EssaCargaNaoTemDataDeInicioOuDataDeFim);
            }

            switch (localFreeTime)
            {
                case LocalFreeTime.Coleta:
                    return (DateTime.Now - carga.DataInicioViagem).Value.TotalDays;
                default:
                    return (DateTime.Now - carga.DataFimViagem).Value.TotalDays;
            }
        }

        private void SalvarRelatorioAtendimentoNosAnexos(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, byte[] pdf, string fileName, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoAnexo repChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);

            string token = Guid.NewGuid().ToString().Replace("-", "");
            string extensao = ".pdf";
            string caminhoChamado = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), token + extensao);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoChamado, pdf);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoChamado))
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo()
                {
                    Chamado = chamado,
                    Descricao = "Atendimento " + chamado.Numero,
                    GuidArquivo = token,
                    NomeArquivo = fileName
                };

                repChamadoAnexo.Inserir(chamadoAnexo);
            }
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido RetornarPedidoChamado(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configChamado, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (chamado.Pedido == null && (configChamado?.VincularPrimeiroPedidoDoClienteAoAbrirChamado ?? false))
                return repositorioCargaPedido.BuscarCargaPedidoPorCargaEClienteDestinatario(chamado.Carga.Codigo, chamado.Cliente.Codigo)?.FirstOrDefault()?.Pedido;

            return chamado.Pedido;
        }

        private bool UsuarioAptoARealizarAcoesEscalada(int codigoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoAnalise repositorioChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);

            if (!repositorioChamadoAnalise.UsuarioAptoAEscalarAtendimento(codigoChamado, this.Usuario.Codigo))
                return false;

            return true;
        }
        private (decimal valorCarga, decimal valorDescarga) CalcularValoresCargaDescarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            decimal valorCarga = 0;
            decimal valorDescarga = 0;

            if (carga == null)
                return (valorCarga, valorDescarga);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                if (cargaPedido.Pedido.PossuiCarga || (cargaPedido.Pedido.ValorCarga ?? 0) > 0)
                    valorCarga += cargaPedido.Pedido.ValorCarga ?? 0;

                if (cargaPedido.Pedido.PossuiDescarga || (cargaPedido.Pedido.ValorDescarga ?? 0) > 0)
                    valorDescarga += cargaPedido.Pedido.ValorDescarga ?? 0;
            }

            return (valorCarga, valorDescarga);
        }

        private string ValidarValorCargaOuDescarga(Usuario motorista)
        {
            if (motorista == null)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(motorista.Celular))
                return string.Empty;

            string celular = Utilidades.String.OnlyNumbers(motorista.Celular ?? string.Empty);

            bool numeroBrasileiro = motorista.Localidade == null || motorista.Localidade.Pais?.Abreviacao == "BR";

            return $"{(numeroBrasileiro ? "+55" : "")}{celular}";
        }

        private void SalvarNotasDeDevolucao(Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            TipoMotivoAtendimento tipoMotivoAtendimento = chamado.MotivoChamado.TipoMotivoAtendimento;
            bool devolucaoParcial = Request.GetEnumParam("TipoDevolucao", TipoColetaEntregaDevolucao.Total) == TipoColetaEntregaDevolucao.Parcial;
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;

            if (tipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao || (devolucaoParcial && chamado.TratativaDevolucao == SituacaoEntrega.Reentergue) ||
                   (tipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga && (chamado.TratativaDevolucao == SituacaoEntrega.NaoEntregue || chamado.TratativaDevolucao == SituacaoEntrega.Rejeitado)))
            {
                SalvarNFeDevolucao(cargaEntrega, unitOfWork, chamado);
            }
        }

        private void SalvarNFeDevolucao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork, Chamado chamado)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repositorioControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            dynamic dynNFes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NFeDevolucaoAbertura"));

            if (cargaEntrega.NFesDevolucao?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic dado in dynNFes)
                {
                    int codigoConvertido = 0;
                    if (dado.Codigo != null && int.TryParse((string)dado.Codigo, out codigoConvertido))
                        codigos.Add(codigoConvertido);
                }

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> listaDeletar = (from obj in cargaEntrega.NFesDevolucao where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < listaDeletar.Count; i++)
                {
                    var controleNotaDevolucao = repCargaEntregaNFeDevolucao.BuscarControleNotaDevolucao(listaDeletar[i].Codigo);
                    repCargaEntregaNFeDevolucao.Deletar(listaDeletar[i]);
                    if (controleNotaDevolucao != null) repositorioControleNotaDevolucao.Deletar(controleNotaDevolucao);
                }

            }
            else
                cargaEntrega.NFesDevolucao = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();

            foreach (dynamic dados in dynNFes)
            {
                int codigo = ((string)dados.Codigo).ToInt();
                int codigoNFeOrigem = ((string)dados.CodigoNotaOrigem).ToInt();

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao nfe = codigo > 0 ? repCargaEntregaNFeDevolucao.BuscarPorCodigo((int)dados.Codigo) : null;

                nfe ??= new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao();

                nfe.ChaveNFe = Utilidades.String.OnlyNumbers((string)dados.Chave);

                if (!string.IsNullOrWhiteSpace(nfe.ChaveNFe) && !Utilidades.Validate.ValidarChaveNFe(nfe.ChaveNFe))
                    throw new ControllerException($"A chave {nfe.ChaveNFe} da NF-e de Devolução é inválida.");

                nfe.Numero = ((string)dados.Numero).ToInt();
                nfe.Serie = ((string)dados.Serie).ToInt();
                nfe.DataEmissao = ((string)dados.DataEmissao).ToNullableDateTime();
                nfe.ValorTotalProdutos = ((string)dados.ValorTotalProdutos).ToDecimal();
                nfe.ValorTotal = ((string)dados.ValorTotal).ToDecimal();
                nfe.PesoDevolvido = ((string)dados.PesoDevolvido).ToDecimal();
                if (codigoNFeOrigem > 0)
                    nfe.XMLNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(codigoNFeOrigem);

                if (!string.IsNullOrWhiteSpace(nfe.ChaveNFe) && (nfe.Numero == 0 || nfe.Serie == 0))
                {
                    nfe.Numero = Utilidades.Chave.ObterNumero(nfe.ChaveNFe);
                    nfe.Serie = Utilidades.Chave.ObterSerie(nfe.ChaveNFe).ToInt();
                }

                nfe.CargaEntrega = cargaEntrega;
                nfe.Chamado = chamado;

                if (nfe.Codigo > 0)
                    repCargaEntregaNFeDevolucao.Atualizar(nfe);
                else
                    repCargaEntregaNFeDevolucao.Inserir(nfe);
            }
        }

        private void VincularOcorrenciaNoAtendimentoAutomaticamente(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            if (configuracaoOcorrencia.PermitirVinculoAutomaticoEntreOcorreciaEAtendimento && chamado.Carga != null && chamado.XMLNotasFiscais != null)
            {

                // Faz o vinculo da Ocorrência através da Nota com o Atendimento de forma automática
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in chamado.XMLNotasFiscais)
                {
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                    //Retorna as Ocorrencias Vinculados a Nota Fiscal
                    IList<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia> litaOcorrencia = repOcorrencia.BuscarOcorrenciasPorCodigoNotaFiscaECarga(chamado.Carga.Codigo, nota.Codigo);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia ocorrenciaItem in litaOcorrencia)
                    {

                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(ocorrenciaItem.CodigoOcorrencia);

                        if (ocorrencia != null)
                        {
                            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
                            Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrencia = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
                            {
                                CargaOcorrencia = ocorrencia,
                                Chamado = chamado
                            };

                            repChamadoOcorrencia.Inserir(chamadoOcorrencia);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Verifica se o pedido do chamado tem alguma carga e faz o vinculo da carga no chamado
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="codigoChamado"></param>
        private Dominio.Entidades.Embarcador.Chamados.Chamado VincularCargaNoAtendimentoPorPedido(Repositorio.UnitOfWork unitOfWork, int codigoChamado)
        {
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoChamado);

            if (chamado.Pedido == null)
                return null;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorPedido(chamado.Pedido.Codigo);

            if (cargaPedido == null)
                return null;

            chamado.Carga = cargaPedido.Carga;
            repChamado.Atualizar(chamado);

            return chamado;

        }

        private async Task<string> ObterDataPrevisaoEntregaPedidosAsync(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);

            if (chamado.DataPrevisaoEntregaPedidos.HasValue)
                return chamado.DataPrevisaoEntregaPedidos.ToDateTimeString();

            if (!chamado.MotivoChamado.PermitirAtualizarInformacoesPedido)
                return null;

            DateTime? dataPrevisao = chamado.Carga != null
                ? await repCargaPedido.BuscarMaiorPrevisaoEntregaAsync(chamado.Carga.Codigo, chamado.Cliente.CPF_CNPJ)
                : null;

            return dataPrevisao.ToDateTimeString();
        }

        #endregion
    }
}
