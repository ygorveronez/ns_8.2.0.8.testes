using System.Data;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize(new string[] { "BuscarProximosDados" }, "Faturas/Fatura", "SAC/AtendimentoCliente")]
    public class FaturaController : BaseController
    {
        #region Construtores

        public FaturaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                if (!ExecutarPesquisa(out string mensagemErro, out List<Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFatura> lista, out int count, parametrosConsulta, false, unitOfWork))
                    return new JsonpResult(false, true, mensagemErro);

                grid.setarQuantidadeTotal(count);
                grid.AdicionaRows(lista);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                if (!ExecutarPesquisa(out string mensagemErro, out List<Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFatura> lista, out int count, parametrosConsulta, true, unitOfWork))
                    return new JsonpResult(false, true, mensagemErro);
                grid.ObterParametrosConsulta();
                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> EnviarFaturaAgrupada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                //Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(_conexao.StringConexao);
                string stringConexao = _conexao.StringConexao;
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = TipoServicoMultisoftware;
                if (!ExecutarPesquisa(out string mensagemErro, out List<Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFatura> lista, out int count, null, false, unitOfWork, true))
                    return new JsonpResult(false, true, mensagemErro);
                if (lista != null && lista.Count > 0)
                {
                    List<double> cnpjsPessoas = lista.Select(p => (double)p.CNPJPessoa).Distinct().ToList();
                    List<int> codigosGrupoPessoas = lista.Select(p => (int)p.CodigoGrupoPessoa).Distinct().ToList();
                    if (cnpjsPessoas != null && cnpjsPessoas.Count > 0)
                    {
                        foreach (var cnpjPessoa in cnpjsPessoas)
                        {
                            if (cnpjPessoa > 0)
                            {
                                List<int> codigosFatura = lista.Where(p => (double)p.CNPJPessoa == cnpjPessoa).Select(p => (int)p.Codigo).Distinct().ToList();
                                Task.Factory.StartNew(() => Servicos.Embarcador.Fatura.Fatura.EnviarFaturaLote(codigosFatura, stringConexao, tipoServicoMultisoftware, null, null));
                                //servFatura.EnviarFaturaLote(codigosFatura, out string msgErro, unitOfWork.StringConexao, TipoServicoMultisoftware);
                            }
                        }
                    }
                    if (codigosGrupoPessoas != null && codigosGrupoPessoas.Count > 0)
                    {
                        foreach (var codigoGrupoPessoa in codigosGrupoPessoas)
                        {
                            if (codigoGrupoPessoa > 0)
                            {
                                List<int> codigosFatura = lista.Where(p => (int)p.CodigoGrupoPessoa == codigoGrupoPessoa).Select(p => (int)p.Codigo).Distinct().ToList();
                                Task.Factory.StartNew(() => Servicos.Embarcador.Fatura.Fatura.EnviarFaturaLote(codigosFatura, stringConexao, tipoServicoMultisoftware, null, null));
                                //servFatura.EnviarFaturaLote(codigosFatura, out string msgErro, unitOfWork.StringConexao, TipoServicoMultisoftware);
                            }
                        }
                    }
                }
                else
                    return new JsonpResult(false, true, "Nenhum tomador encontrado com pendência de envio dos dados de fatura.");

                return new JsonpResult(true, true, "Solcitação realizada com sucesso.");
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

        public async Task<IActionResult> GerarFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!this.Usuario.UsuarioAdministrador)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteIniciarFatura))
                        return new JsonpResult(false, "Seu usuário não possui permissão para iniciar uma fatura.");

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroDeResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoa);
                int.TryParse(Request.Params("NumeroPreFatura"), out int numeroPreFatura);
                int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
                int.TryParse(Request.Params("TipoCarga"), out int tipoCarga);
                int.TryParse(Request.Params("Transportador"), out int transportador);
                int.TryParse(Request.Params("PedidoViagemNavio"), out int pedidoViagemNavio);
                int.TryParse(Request.Params("TerminalOrigem"), out int terminalOrigem);
                int.TryParse(Request.Params("TerminalDestino"), out int terminalDestino);
                int.TryParse(Request.Params("Origem"), out int origem);
                int.TryParse(Request.Params("Destino"), out int destino);
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int paisOrigem = Request.GetIntParam("PaisOrigem");
                int codFilial = Request.GetIntParam("Filial");
                double cpfcnpjTomador = Request.GetDoubleParam("Tomador");
                int codCentroDeResultado = Request.GetIntParam("CentroDeResultado");

                long numeroFaturaOriginal = Request.GetLongParam("NumeroFaturaOriginal");

                decimal.TryParse(Request.Params("AliquotaICMS"), out decimal aliquotaICMS);

                bool.TryParse(Request.Params("GerarDocumentosAutomaticamente"), out bool gerarDocumentosAutomaticamente);
                bool naoUtilizarMoedaEstrangeira = Request.GetBoolParam("NaoUtilizarMoedaEstrangeira");
                bool gerarDocumentosApenasCanhotosAprovados = Request.GetBoolParam("GerarDocumentosApenasCanhotosAprovados");

                double.TryParse(Request.Params("Pessoa"), out double pessoa);

                string observacao = Request.Params("Observacao");
                string numeroBooking = Request.Params("NumeroBooking");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido> tiposOSConvertidos = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido>("TipoOSConvertido");

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataFatura"), out DateTime dataFatura);

                dataInicial = Request.GetDateTimeParam("DataInicial");
                dataFinal = Request.GetDateTimeParam("DataFinal");
                dataFatura = Request.GetDateTimeParam("DataFatura");

                Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacao);
                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);

                Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir fatura = new Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir();
                Dominio.Entidades.Embarcador.Fatura.Fatura _Fatura = codigo > 0 ? repFatura.BuscarPorCodigo(codigo, true) : null;
                fatura.Codigo = _Fatura != null ? _Fatura.Codigo : 0;
                fatura.DataInicial = dataInicial;
                fatura.DataFinal = dataFinal;
                fatura.DataFatura = dataFatura;
                fatura.NumeroPreFatura = numeroPreFatura;
                fatura.NumeroFaturaOriginal = numeroFaturaOriginal;
                fatura.NovoModelo = true;
                fatura.TipoPropostaMultimodal = tipoPropostaMultimodal;

                fatura.TipoOSConvertido = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido>();

                if (tiposOSConvertidos.Count() > 0)
                    fatura.TipoOSConvertido.AddRange(tiposOSConvertidos);

                if (gerarDocumentosAutomaticamente)
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas;
                else
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Documentos;

                fatura.Situacao = situacao;
                fatura.Observacao = observacao;
                //fatura.Usuario = this.Usuario;
                fatura.TipoPessoa = tipoPessoa;
                fatura.GerarDocumentosAutomaticamente = gerarDocumentosAutomaticamente;
                fatura.NaoUtilizarMoedaEstrangeira = naoUtilizarMoedaEstrangeira;
                fatura.GerarDocumentosApenasCanhotosAprovados = gerarDocumentosApenasCanhotosAprovados;

                fatura.AliquotaICMS = aliquotaICMS > 0m ? (decimal?)aliquotaICMS : null;

                var _TipoCarga = tipoCarga > 0 ? repTipoCarga.BuscarPorCodigo(tipoCarga) : null;
                fatura.TipoCarga = _TipoCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador { Codigo = _TipoCarga.Codigo } : null;

                var _Cliente = pessoa > 0D ? repCliente.BuscarPorCPFCNPJ(pessoa) : null;
                fatura.Cliente = _Cliente != null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa { CPFCNPJ = _Cliente.Codigo.ToString(), Codigo = _Cliente.Codigo.ToString() } : null;

                var _Tomador = cpfcnpjTomador > 0 ? repCliente.BuscarPorCPFCNPJ(cpfcnpjTomador) : null;
                fatura.Tomador = _Tomador != null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa { CPFCNPJ = _Tomador.Codigo.ToString(), Codigo = _Tomador.Codigo.ToString() } : null;

                var _GrupoPessoas = grupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(grupoPessoa) : null;
                fatura.GrupoPessoas = _GrupoPessoas != null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa { Codigo = _GrupoPessoas.Codigo } : null;

                var _TipoOperacao = tipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacao) : null;
                fatura.TipoOperacao = _TipoOperacao != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao { Codigo = _TipoOperacao.Codigo } : null;

                var _Transportador = transportador > 0 ? repEmpresa.BuscarPorCodigo(transportador) : null;
                fatura.Transportador = _Transportador != null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa { Codigo = _Transportador.Codigo } : null;

                var _PedidoViagemNavio = pedidoViagemNavio > 0 ? repPedidoViagemNavio.BuscarPorCodigo(pedidoViagemNavio) : null;
                fatura.PedidoViagemNavio = _PedidoViagemNavio != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.Viagem { Codigo = _PedidoViagemNavio.Codigo } : null;

                var _TerminalOrigem = terminalOrigem > 0 ? repTerminal.BuscarPorCodigo(terminalOrigem) : null;
                fatura.TerminalOrigem = _TerminalOrigem != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto { Codigo = _TerminalOrigem.Codigo } : null;

                var _TerminalDestino = terminalDestino > 0 ? repTerminal.BuscarPorCodigo(terminalDestino) : null;
                fatura.TerminalDestino = _TerminalDestino != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto { Codigo = _TerminalDestino.Codigo } : null;

                var _Origem = origem > 0 ? repLocalidade.BuscarPorCodigo(origem) : null;
                fatura.Origem = _Origem != null ? new Dominio.ObjetosDeValor.Localidade { Codigo = _Origem.Codigo } : null;

                var _PaisOrigem = paisOrigem > 0 ? repPais.BuscarPorCodigo(paisOrigem) : null;
                fatura.PaisOrigem = _PaisOrigem != null ? new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais { CodigoPais = _PaisOrigem.Codigo } : null;

                //fatura.Filial = codFilial > 0 ? repFilial.BuscarPorCodigo(codFilial) : null;
                fatura.CodigoFilial = codFilial > 0 ? repFilial.BuscarPorCodigo(codFilial).Codigo : -1;

                var _Destino = destino > 0 ? repLocalidade.BuscarPorCodigo(destino) : null;
                fatura.Destino = _Destino != null ? new Dominio.ObjetosDeValor.Localidade { Codigo = _Destino.Codigo } : null;

                var _Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
                fatura.Veiculo = _Veiculo != null ? new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo { Codigo = _Veiculo.Codigo } : null;

                fatura.NumeroBooking = numeroBooking;

                var _CentroDeResultado = codCentroDeResultado > 0 ? repCentroDeResultado.BuscarPorCodigo(codCentroDeResultado) : null;
                fatura.CentroDeResultado = _CentroDeResultado != null ? new Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado { Codigo = _CentroDeResultado.Codigo } : null;

                fatura.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");

                unitOfWork.Start();
                string msgErro = "";
                string msgAlert = "";

                if (!servFatura.GerarFatura(fatura, out msgErro, out msgAlert, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, this.Usuario))
                {
                    unitOfWork.Rollback();
                    if (msgAlert != "")
                        return new JsonpResult(false, true, msgAlert);
                    return new JsonpResult(false, msgErro);
                }
                else
                    unitOfWork.CommitChanges();

                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unitOfWork);
                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadPDFFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFatura = 0;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Codigo")), out codigoFatura);

                if (codigoFatura > 0)
                {
                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                    if (fatura != null && !string.IsNullOrWhiteSpace(fatura.CaminhoPDF))
                    {
                        try
                        {
                            if (Utilidades.IO.FileStorageService.Storage.Exists(fatura.CaminhoPDF))
                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fatura.CaminhoPDF), "application/x-pkcs12", fatura.Codigo.ToString() + ".pdf");
                            else
                                return new JsonpResult(false, false, "O arquivo da fatura " + fatura.CaminhoPDF + " não foi encontrado.");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);

                            return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do pdf da fatura.");
                        }
                    }
                    else
                        return new JsonpResult(false, false, "Esta fatura não possui o pdf disponível para download.");
                }
                return new JsonpResult(false, false, "Fatura não encontrada");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //obsoleta
        public async Task<IActionResult> oldGerarFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                if (!this.Usuario.UsuarioAdministrador)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteIniciarFatura))
                        return new JsonpResult(false, "Seu usuário não possui permissão para iniciar uma fatura.");

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoa);
                int.TryParse(Request.Params("NumeroPreFatura"), out int numeroPreFatura);
                int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
                int.TryParse(Request.Params("TipoCarga"), out int tipoCarga);
                int.TryParse(Request.Params("Transportador"), out int transportador);
                int.TryParse(Request.Params("PedidoViagemNavio"), out int pedidoViagemNavio);
                int.TryParse(Request.Params("TerminalOrigem"), out int terminalOrigem);
                int.TryParse(Request.Params("TerminalDestino"), out int terminalDestino);
                int.TryParse(Request.Params("Origem"), out int origem);
                int.TryParse(Request.Params("Destino"), out int destino);
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int paisOrigem = Request.GetIntParam("PaisOrigem");
                int codFilial = Request.GetIntParam("Filial");
                double cpfcnpjTomador = Request.GetDoubleParam("Tomador");

                long numeroFaturaOriginal = Request.GetLongParam("NumeroFaturaOriginal");

                decimal.TryParse(Request.Params("AliquotaICMS"), out decimal aliquotaICMS);

                bool.TryParse(Request.Params("GerarDocumentosAutomaticamente"), out bool gerarDocumentosAutomaticamente);
                bool naoUtilizarMoedaEstrangeira = Request.GetBoolParam("NaoUtilizarMoedaEstrangeira");
                bool gerarDocumentosApenasCanhotosAprovados = Request.GetBoolParam("GerarDocumentosApenasCanhotosAprovados");

                double.TryParse(Request.Params("Pessoa"), out double pessoa);

                string observacao = Request.Params("Observacao");
                string numeroBooking = Request.Params("NumeroBooking");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                //List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;
                //if (tiposPropostasMultimodal != null && tiposPropostasMultimodal.Count > 0)
                //{
                //    tipoPropostaMultimodal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();
                //    tipoPropostaMultimodal = tiposPropostasMultimodal.ToList();
                //}

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataFatura"), out DateTime dataFatura);

                dataInicial = Request.GetDateTimeParam("DataInicial");
                dataFinal = Request.GetDateTimeParam("DataFinal");
                dataFatura = Request.GetDateTimeParam("DataFatura");

                Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacao);
                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);

                if (dataFatura < DateTime.MinValue)
                    return new JsonpResult(false, true, "Por favor informe a data de emissão da fatura.");

                if (dataFatura > DateTime.Now.Date)
                    return new JsonpResult(false, true, "Por favor informe a data de emissão da fatura menor que a data atual.");

                if (dataInicial < DateTime.MinValue)
                    return new JsonpResult(false, true, "Por favor informe a data de inicial da fatura.");

                if (dataFatura < dataInicial)
                    return new JsonpResult(false, true, "Por favor informe a data de emissão maior que a data inicial.");

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = codigo > 0 ? repFatura.BuscarPorCodigo(codigo, true) : null;

                if (fatura != null && fatura.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas)
                    return new JsonpResult(false, true, "A fatura está em processo de lançamento de documentos, não sendo possível alterar a mesma.");

                unitOfWork.Start();

                if (fatura == null)
                {
                    fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura();
                    if (ConfiguracaoEmbarcador.GerarNumeracaoFaturaAnual)
                    {
                        int anoAtual = DateTime.Now.Year;
                        fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao(anoAtual);
                        anoAtual = (anoAtual % 100);
                        if (fatura.ControleNumeracao == 1 || (fatura.ControleNumeracao < ((anoAtual * 1000000) + 1)))
                            fatura.ControleNumeracao = (anoAtual * 1000000) + 1;
                    }
                    else
                        fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao();
                }

                fatura.DataInicial = dataInicial;
                fatura.DataFinal = dataFinal;
                fatura.DataFatura = dataFatura;
                fatura.NumeroPreFatura = numeroPreFatura;
                fatura.NumeroFaturaOriginal = numeroFaturaOriginal;
                fatura.NovoModelo = true;

                if (gerarDocumentosAutomaticamente)
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas;
                else
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Documentos;

                fatura.Situacao = situacao;
                fatura.Observacao = observacao;
                fatura.Usuario = this.Usuario;
                fatura.TipoPessoa = tipoPessoa;
                fatura.GerarDocumentosAutomaticamente = gerarDocumentosAutomaticamente;
                fatura.NaoUtilizarMoedaEstrangeira = naoUtilizarMoedaEstrangeira;
                fatura.GerarDocumentosApenasCanhotosAprovados = gerarDocumentosApenasCanhotosAprovados;

                fatura.AliquotaICMS = aliquotaICMS > 0m ? (decimal?)aliquotaICMS : null;
                fatura.TipoCarga = tipoCarga > 0 ? repTipoCarga.BuscarPorCodigo(tipoCarga) : null;
                fatura.Cliente = pessoa > 0D ? repCliente.BuscarPorCPFCNPJ(pessoa) : null;
                fatura.GrupoPessoas = grupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(grupoPessoa) : null;
                fatura.TipoOperacao = tipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacao) : null;
                fatura.Transportador = transportador > 0 ? repEmpresa.BuscarPorCodigo(transportador) : null;

                fatura.PedidoViagemNavio = pedidoViagemNavio > 0 ? repPedidoViagemNavio.BuscarPorCodigo(pedidoViagemNavio) : null;
                fatura.TerminalOrigem = terminalOrigem > 0 ? repTerminal.BuscarPorCodigo(terminalOrigem) : null;
                fatura.TerminalDestino = terminalDestino > 0 ? repTerminal.BuscarPorCodigo(terminalDestino) : null;

                fatura.Origem = origem > 0 ? repLocalidade.BuscarPorCodigo(origem) : null;
                fatura.PaisOrigem = paisOrigem > 0 ? repPais.BuscarPorCodigo(paisOrigem) : null;
                fatura.Filial = codFilial > 0 ? repFilial.BuscarPorCodigo(codFilial) : null;
                fatura.Tomador = cpfcnpjTomador > 0 ? repCliente.BuscarPorCPFCNPJ(cpfcnpjTomador) : null;
                fatura.Destino = destino > 0 ? repLocalidade.BuscarPorCodigo(destino) : null;
                fatura.NumeroBooking = numeroBooking;
                fatura.Veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    if (fatura.TipoOperacao == null && !configuracaoFinanceiro.NaoObrigarTipoOperacaoFatura)
                    {
                        //regra para Danone, rever, onde se o CNPJ informado for danone não deve obrigar tipo de operação, se não for deve informar o tipo de operação.
                        Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCNPJ(fatura.Cliente?.CPF_CNPJ_SemFormato ?? "");
                        if (fatura.GrupoPessoas != null || (fatura.GrupoPessoas == null && filial == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, "É obrigatório informar o tipo de operação.");
                        }
                    }
                }

                if (fatura.GrupoPessoas == null && fatura.Cliente == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Não foi selecionado nenhum grupo de pessoa ou cliente para a geração da fatura.");
                }

                if (tipoPessoa == 0)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Não foi selecionado nenhum tipo de pessoa para a geração da fatura.");
                }

                if (dataInicial == null || dataInicial == DateTime.MinValue)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Não foi selecionado a data inicial.");
                }

                if (dataFinal == null || dataFinal == DateTime.MinValue)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Não foi selecionado a data final.");
                }

                if (codigo > 0)
                {
                    repFatura.LimparCargasCtePorFatura(fatura.Codigo);
                    repFatura.Atualizar(fatura, Auditado);
                }
                else
                {
                    fatura.ImprimeObservacaoFatura = false;
                    fatura.Total = 0;
                    fatura.Numero = 0;
                    fatura.DataFatura = dataFatura;
                    fatura.DataCancelamentoFatura = null;

                    repFatura.Inserir(fatura, Auditado);
                }

                string tipoPropostaMultimodalAnterior = "";
                if (fatura.TipoPropostaMultimodal == null)
                    fatura.TipoPropostaMultimodal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();
                else
                {
                    tipoPropostaMultimodalAnterior = string.Join(", ", fatura.TipoPropostaMultimodal);
                    fatura.TipoPropostaMultimodal.Clear();
                }
                if (tipoPropostaMultimodal != null)
                {
                    foreach (var tipo in tipoPropostaMultimodal)
                    {
                        fatura.TipoPropostaMultimodal.Add(tipo);
                    }
                }
                repFatura.Atualizar(fatura);

                string tipoPropostaMultimodalAtual = string.Join(", ", fatura.TipoPropostaMultimodal);
                if (!tipoPropostaMultimodalAnterior.Equals(tipoPropostaMultimodalAtual))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Alterou Tipo da Proposta Multimodal de '" + tipoPropostaMultimodalAnterior + "' para '" + tipoPropostaMultimodalAtual + "'.", unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Gerou a Fatura.", unitOfWork);

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.IniciouFatura, this.Usuario);

                unitOfWork.CommitChanges();

                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
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

        public async Task<IActionResult> CancelarFatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo, true);

                if (fatura.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas)
                    return new JsonpResult(false, true, "A fatura ainda está em processo de lançamento de documentos, não sendo possível realizar o cancelamento.");

                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                    return new JsonpResult(false, true, "A situação atual da fatura não permite o cancelamento.");

                if (fatura.FaturaRecebidaDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar a operação para uma fatura recebida pela integração.");

                //List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> faturaDocumentos = repFaturaDocumento.BuscarPorFatura(fatura.Codigo);

                unitOfWork.Start();

                fatura.UsuarioCancelamento = this.Usuario;
                fatura.NotificadoOperador = false;
                fatura.SituacaoNoCancelamento = fatura.Situacao;
                fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmCancelamento;
                fatura.DataCancelamentoFatura = DateTime.Now;
                fatura.Duplicar = false;

                repFatura.Atualizar(fatura, Auditado);

                servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.CancelouFatura, this.Usuario);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Cancelou a fatura.", unitOfWork);

               ////List<TipoIntegracao> tiposIntegracaoAutorizados = new List<TipoIntegracao> { TipoIntegracao.Intercab, TipoIntegracao.EMP, TipoIntegracao.NFTP, TipoIntegracao.SAP_ESTORNO_FATURA };
               // //List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoAutorizados);
               // //if (tiposIntegracao?.Count > 0)
               // {
               //     foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
               //     {

               //         if (tipoIntegracao.Tipo == TipoIntegracao.Intercab)
               //         {
               //             Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
               //             Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

               //             if (integracaoIntercab != null && fatura.FaturaIntegracaComSucesso)
               //                 servFatura.AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
               //         }
               //         if (tipoIntegracao.Tipo == TipoIntegracao.EMP)
               //         {
               //             Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
               //             Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

               //             if (integracaoEMP != null && integracaoEMP.PossuiIntegracaoEMP && integracaoEMP.AtivarIntegracaoCancelamentoFaturaEMP)
               //                 servFatura.AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
               //         }
               //         if (tipoIntegracao.Tipo == TipoIntegracao.NFTP)
               //         {
               //             Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
               //             Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

               //             if (integracaoEMP != null && integracaoEMP.AtivarIntegracaoNFTPEMP)
               //                 servFatura.AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
               //         }
               //         if (tipoIntegracao.Tipo == TipoIntegracao.SAP_ESTORNO_FATURA)// deveria ser SAP_Fatura mas foi criado apenas como SAP
               //             servFatura.AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
               //     }
               // }

                unitOfWork.CommitChanges();

                var dynRetorno = servFatura.RetornaObjetoCompletoFatura(fatura.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar a fatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProximosDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarProximosDados();

                dynamic dynRetorno;
                if (fatura != null)
                {
                    dynRetorno = new
                    {
                        Numero = 0,//fatura.Numero + 1,
                        Data = fatura.DataFinal.HasValue ? fatura.DataFinal.Value.ToString("dd/MM/yyyy") : DateTime.Now.Date.ToString("dd/MM/yyyy")
                    };
                }
                else
                {
                    dynRetorno = new
                    {
                        Numero = 0,//1,
                        Data = string.Empty
                    };
                }
                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar próximos dados");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ObterFormaPagamentoPadraoFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo);

                bool possuiMoedaEstrangeira = fatura.NaoUtilizarMoedaEstrangeira ? false : repFaturaDocumento.PossuiMoedaEstrangeira(codigo);

                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento formaPagamento = null;

                if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                    formaPagamento = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.FormaPagamento;
                else if (fatura.ClienteTomadorFatura.NaoUsarConfiguracaoFaturaGrupo)
                    formaPagamento = fatura.ClienteTomadorFatura.FormaPagamento;
                else if (fatura.ClienteTomadorFatura.GrupoPessoas != null)
                    formaPagamento = fatura.ClienteTomadorFatura.GrupoPessoas.FormaPagamento;

                return new JsonpResult(new
                {
                    Codigo = formaPagamento?.Codigo ?? 0,
                    Descricao = formaPagamento?.Descricao ?? string.Empty,
                    PossuiMoedaEstrangeira = possuiMoedaEstrangeira
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao obter a forma de pagamento padrão para a fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ImportarPreFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);

                if (grupoPessoas == null)
                    return new JsonpResult(false, true, "Grupo de pessoas não encontrado.");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, true, "Selecione um arquivo para o envio.");

                Servicos.DTO.CustomFile file = files[0];
                bool importacaoXml = (file.ContentType == "text/xml");

                if (importacaoXml)
                    Servicos.Embarcador.Integracao.EDI.PREFAT.GerarPreFaturasXml(this.Usuario, grupoPessoas, file.InputStream, unidadeTrabalho, Auditado);
                else
                {
                    Dominio.Entidades.LayoutEDI layoutEDI = grupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.PREFAT).Select(o => o.LayoutEDI).FirstOrDefault();

                    if (layoutEDI == null)
                        return new JsonpResult(false, true, "O grupo de pessoas não possui um layout de EDI de pré fatura configurado.");

                    if (!Servicos.Embarcador.Integracao.EDI.PREFAT.GerarPreFaturas(out string erro, this.Usuario, grupoPessoas, this.Empresa, layoutEDI, file.InputStream, unidadeTrabalho, _conexao.StringConexao, Auditado))
                        return new JsonpResult(false, true, erro);
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);

                if (excecao.Message.Contains("Unexpected end of file while parsing"))
                    return new JsonpResult(false, false, "Existe um problema com a estrutura do arquivo xml, verificar se todas as tags estão fechadas.");

                return new JsonpResult(false, false, "Ocorreu uma falha ao importar a pré fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("Nº Fatura", "Numero", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Período", "DescricaoPeriodo", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Transportadora", "Transportadora", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("N° Carga", "NumeroCarga", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Vencimento", "PeriodoVencimento", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Emissão", "PeriodoEmissao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
            {
                grid.AdicionarCabecalho("Nº Boleto(s)", "NumeroBoletos", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("VVD", "PedidoViagemNavio", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Controles", "NumerosControle", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Fiscais", "NumerosFiscais", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CNPJPessoa", false);
                grid.AdicionarCabecalho("CodigoGrupoPessoa", false);
            }
            else
            {
                grid.AdicionarCabecalho("NumeroBoletos", false);
                grid.AdicionarCabecalho("PedidoViagemNavio", false);
                grid.AdicionarCabecalho("TerminalOrigem", false);
                grid.AdicionarCabecalho("TipoOperacao", false);
                grid.AdicionarCabecalho("NumerosControle", false);
                grid.AdicionarCabecalho("NumerosFiscais", false);
                grid.AdicionarCabecalho("CNPJPessoa", false);
                grid.AdicionarCabecalho("CodigoGrupoPessoa", false);
            }
            return grid;
        }

        private bool ExecutarPesquisa(out string mensagemErro, out List<Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFatura> lista, out int count, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportacao, Repositorio.UnitOfWork unitOfWork, bool pendenteEnvio = false)
        {
            mensagemErro = null;
            lista = null;
            count = 0;

            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura filtroPesquisaFatura = ObterFiltrosPesquisa();

            if (pendenteEnvio)
                filtroPesquisaFatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado;

            if (this.Usuario.AssociarUsuarioCliente && this.Usuario.Cliente != null)
                filtroPesquisaFatura.Pessoa = this.Usuario.Cliente.CPF_CNPJ;

            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

            count = repFatura.ContaConsulta(filtroPesquisaFatura);
            if (exportacao && count > 5000)
            {
                mensagemErro = "Exportação não permitida para mais de 5000 registros.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Fatura.Fatura> listaFatura = repFatura.Consulta(filtroPesquisaFatura, parametrosConsulta);

            lista = (from p in listaFatura
                     select new Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFatura
                     {
                         Codigo = p.Codigo,
                         Descricao = p.NumeroFaturaIntegracao.HasValue && p.NumeroFaturaIntegracao.Value > 0 ? p.NumeroFaturaIntegracao.Value : p.Numero,
                         Numero = p.NumeroFaturaIntegracao.HasValue && p.NumeroFaturaIntegracao.Value > 0 ? p.NumeroFaturaIntegracao.Value : p.Numero,
                         NumeroBoletos = p.NumeroBoletos,
                         DescricaoPeriodo = p.DescricaoPeriodo,
                         Pessoa = p.Cliente != null ? p.Cliente.Descricao : p.GrupoPessoas != null ? p.GrupoPessoas.Descricao : string.Empty,
                         DescricaoSituacao = p.DescricaoSituacao,
                         PeriodoVencimento = p.PeriodoVencimento,
                         PeriodoEmissao = p.DataFatura.ToString("dd/MM/yyyy"),
                         Valor = p.Total.ToString("n2"),
                         PedidoViagemNavio = p.PedidoViagemNavio?.Descricao ?? string.Empty,
                         TerminalOrigem = p.TerminalOrigem?.Descricao ?? string.Empty,
                         TipoOperacao = p.TipoOperacao?.Descricao ?? string.Empty,
                         NumerosControle = p.NumerosControle,
                         NumerosFiscais = p.NumerosFiscais,
                         CNPJPessoa = p.Cliente?.CPF_CNPJ ?? 0,
                         CodigoGrupoPessoa = p.GrupoPessoas?.Codigo ?? 0,
                         CodigoTomador = p.ClienteTomadorFatura?.CPF_CNPJ ?? 0,
                         Transportadora = p.Empresa?.Descricao ?? string.Empty,
                         NumeroCarga = string.Join(", ",p.Documentos?.ToArray().Select(x=>x.Documento?.NumeroCarga).Where(x=>!string.IsNullOrEmpty(x)).Distinct() ?? Enumerable.Empty<string>())
                     }).ToList();

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura()
            {
                NumeroFatura = Request.GetIntParam("NumeroFatura") > 0 ? Request.GetIntParam("NumeroFatura") : Request.GetIntParam("Descricao"),
                NumeroPreFatura = Request.GetIntParam("NumeroPreFatura"),
                Empresa = Request.GetIntParam("Empresa"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                Operador = Request.GetIntParam("Operador"),
                TerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                TerminalDestino = Request.GetIntParam("TerminalDestino"),
                PedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                Origem = Request.GetIntParam("Origem"),
                Destino = Request.GetIntParam("Destino"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                NumeroNota = Request.GetIntParam("NumeroNota"),
                CentroDeResultado = Request.GetIntParam("CentroDeResultado"),
                Pessoa = Request.GetDoubleParam("Pessoa"),
                Tomador = Request.GetDoubleParam("Tomador"),
                NumeroControleCliente = Request.GetStringParam("NumeroControleCliente"),
                NumeroReferenciaEDI = Request.GetStringParam("NumeroReferenciaEDI"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                DataFatura = Request.GetDateTimeParam("DataFatura"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                Etapa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura>("Etapa"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura>("Situacao"),
                TipoPessoa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa>("TipoPessoa"),
                TipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal"),
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoSituacao")
                propriedadeOrdenar = "Situacao";
            else if (propriedadeOrdenar == "DescricaoPeriodo")
                propriedadeOrdenar = "DataInicial";
            else if (propriedadeOrdenar == "Valor")
                propriedadeOrdenar = "Total";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
