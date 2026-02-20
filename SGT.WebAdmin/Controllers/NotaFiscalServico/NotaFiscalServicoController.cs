using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Data;

namespace SGT.WebAdmin.Controllers.NotaFiscalServico
{
    [CustomAuthorize(new string[] { "BuscarDadosEmpresa", "BuscarPorCodigo" }, "NotasFiscaisServicos/NotaFiscalServico")]
    public class NotaFiscalServicoController : BaseController
    {
        #region Construtores

        public NotaFiscalServicoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int numeroInicial, numeroFinal, serie, naturezaOperacao, empresa = 0, localidadeTransportador = 0;
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("Serie"), out serie);
                int.TryParse(Request.Params("NaturezaOperacao"), out naturezaOperacao);

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                string cnpjcpfPessoa = Request.Params("Pessoa");
                string protocolo = Request.Params("NumeroProtocolo");
                string status = Request.Params("Status");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;
                else
                    empresa = Request.GetIntParam("Transportador");

                localidadeTransportador = Request.GetIntParam("LocalidadePrincipalTransportador");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Nota", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Nº RPS", "NumeroRPS", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Último Retorno SEFAZ", "MensagemRetornoSefaz", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pessoa (Tomador)", "Pessoa", 32, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Natureza da Operação", "NaturezaOperacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "ValorAReceber", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("CodigoNaturezaOperacao", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Pessoa")
                    propOrdenar = "Remetente";
                else if (propOrdenar == "NaturezaOperacao")
                    propOrdenar = "NaturezaNFSe";
                else if (propOrdenar == "DescricaoStatus")
                    propOrdenar = "Status";

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaNFSe = repCTe.ConsultarNFSe(numeroInicial, numeroFinal, serie, protocolo, empresa, dataInicial, dataFinal, naturezaOperacao, cnpjcpfPessoa, this.Usuario.Empresa.TipoAmbiente, status, localidadeTransportador, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCTe.ContarConsultaNFSe(numeroInicial, numeroFinal, serie, protocolo, empresa, dataInicial, dataFinal, naturezaOperacao, cnpjcpfPessoa, this.Usuario.Empresa.TipoAmbiente, status, localidadeTransportador));
                var lista = (from p in listaNFSe
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 Serie = p.Serie?.Numero ?? 0,
                                 p.DataEmissao,
                                 p.DescricaoStatus,
                                 p.MensagemRetornoSefaz,
                                 Transportador = p.Empresa.Descricao,
                                 Pessoa = p.TomadorPagador?.Descricao ?? p.Remetente?.Descricao ?? string.Empty,
                                 NaturezaOperacao = p.NaturezaNFSe?.Descricao ?? string.Empty,
                                 p.Protocolo,
                                 ValorAReceber = p.ValorAReceber.ToString("n2"),
                                 p.Status,
                                 CodigoNaturezaOperacao = p.NaturezaNFSe?.Codigo ?? 0,
                                 NumeroRPS = p.RPS?.Numero ?? 0
                             }).ToList();
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

        public async Task<IActionResult> BuscarDadosEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = null;
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                }
                else
                {
                    empresa = repEmpresa.BuscarEmpresaPadraoEmissao();
                    codigoEmpresa = empresa?.Codigo ?? 0;
                }
                List<Dominio.Entidades.EmpresaSerie> listaSerie = repSerie.BuscarSeriesPorEmpresaTipo(codigoEmpresa, Dominio.Enumeradores.TipoSerie.NFSe);

                if (listaSerie.Count == 0 && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) //(listaSerie == null)
                    return new JsonpResult(false, "Série não encontrada. Por favor cadastre uma série antes de lançar uma NFS-e.");

                var retorno = new
                {
                    Serie = listaSerie != null && listaSerie.Count() == 1 ? new
                    {
                        Codigo = listaSerie[0].Codigo,
                        Descricao = listaSerie[0].Numero
                    } : null,
                    CidadeEmpresa = empresa?.Localidade != null ? new
                    {
                        Codigo = empresa?.Localidade.Codigo,
                        Descricao = empresa?.Localidade.DescricaoCidadeEstado
                    } : null,
                    Empresa = empresa != null ? new
                    {
                        Codigo = empresa?.Codigo,
                        Descricao = empresa?.RazaoSocial
                    } : null
                };
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> SalvarNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);

                dynamic dynNFSe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("NFSe"));
                List<int> codigoPedidoVenda = new List<int>();
                if (dynNFSe.NFSe.CodigoPedidoVenda != null)
                    codigoPedidoVenda = RetornaCodigoPedidoVenda(unitOfWork, (string)dynNFSe.NFSe.CodigoPedidoVenda);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorNFSe(dynNFSe.NFSe.Codigo != null ? (int)dynNFSe.NFSe.Codigo : 0);

                //if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                //return new JsonpResult(false, true, "NFS-e vinculada a um Faturamento Mensal, impossível Atualizar.");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ((double)dynNFSe.NFSe.Pessoa);
                Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("39");

                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

                unitOfWork.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = SalvarDadosNFSe(unitOfWork, dynNFSe, cte, cliente, modelo);
                SalvarParcelasNFSe(unitOfWork, dynNFSe, nfse);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfse, null, "Salvou NFS-e.", unitOfWork);
                unitOfWork.CommitChanges();

                if (codigoPedidoVenda.Count > 0)
                    if (!SalvarVinculoNotaPedidoVenda(nfse, codigoPedidoVenda, unitOfWork))
                        return new JsonpResult(false, "Problema ao salvar o Vínculo do Pedido com a Nota! Favor contatar o suporte imediatamente.");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar salvar a NFS-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EmitirNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);

                dynamic dynNFSe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("NFSe"));
                List<int> codigoPedidoVenda = new List<int>();
                if (dynNFSe.NFSe.CodigoPedidoVenda != null)
                    codigoPedidoVenda = RetornaCodigoPedidoVenda(unitOfWork, (string)dynNFSe.NFSe.CodigoPedidoVenda);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorNFSe(dynNFSe.NFSe.Codigo != null ? (int)dynNFSe.NFSe.Codigo : 0);

                //if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                //return new JsonpResult(false, true, "NFS-e vinculada a um Faturamento Mensal, impossível Atualizar.");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ((double)dynNFSe.NFSe.Pessoa);
                Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("39");

                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

                unitOfWork.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = SalvarDadosNFSe(unitOfWork, dynNFSe, cte, cliente, modelo);
                SalvarParcelasNFSe(unitOfWork, dynNFSe, nfse);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfse, null, "Emitiu NFS-e.", unitOfWork);

                unitOfWork.CommitChanges();

                if (codigoPedidoVenda.Count > 0)
                    if (!SalvarVinculoNotaPedidoVenda(nfse, codigoPedidoVenda, unitOfWork))
                        return new JsonpResult(false, "Problema ao salvar o Vínculo do Pedido com a Nota! Favor contatar o suporte imediatamente.");

                bool sucesso = servicoNFSe.EmitirNFSe(nfse.Codigo, unitOfWork);
                if (sucesso)
                    return new JsonpResult(true);
                else
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseRejeicao = repCTe.BuscarPorCodigo(nfse.Codigo);
                    return new JsonpResult(false, true, nfseRejeicao.MensagemRetornoSefaz);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir a NFS-e.");
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
                unitOfWork.Start();

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigo);

                if (nfse == null)
                    return new JsonpResult(false, "NFS-e não encontrado.");

                var retorno = new
                {
                    NFSe = ObterDetalhesNFSe(nfse),
                    Servicos = ObterItensNFSe(nfse, unitOfWork),
                    Valor = ObterValorNFSe(nfse),
                    Substituicao = ObterSubstituicaoNFSe(nfse),
                    Parcelas = ObterParcelamentoNFSe(nfse, unitOfWork)
                };

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> PesquisaNBS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoNBS = Utilidades.String.OnlyNumbers(Request.Params("Descricao"));
                string descricao = Request.Params("CodigoNBS");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("NBS", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "NumeroNBS", 60, Models.Grid.Align.left, true);

                Repositorio.Embarcador.NFS.NBS repositorioNBS = new Repositorio.Embarcador.NFS.NBS(unitOfWork);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Descricao")
                    propOrdenar = "Numero";
                else if (propOrdenar == "NumeroNBS")
                    propOrdenar = "Descricao";

                List<Dominio.Entidades.Embarcador.NFS.NBS> listaNBS = repositorioNBS.ConsultarNBS(codigoNBS, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repositorioNBS.ContarConsultaNBS(codigoNBS, descricao));

                var lista = (from p in listaNBS select new { p.Codigo, NumeroNBS = p.Descricao, Descricao = p.Numero }).ToList();

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

        public async Task<IActionResult> DuplicarNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoNFSe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFSe);

                Servicos.Cliente serCliente = new Servicos.Cliente(_conexao.StringConexao);
                Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.NFSeItem repNFSeItem = new Repositorio.NFSeItem(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigoNFSe);
                List<Dominio.Entidades.NFSeItem> itens = repNFSeItem.BuscarPorCTe(codigoNFSe);
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco endereco = null;

                Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico novaNFSe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
                novaNFSe.Codigo = 0;
                novaNFSe.Status = "S";
                novaNFSe.TipoAmbiente = nfse.Empresa.TipoAmbiente;
                novaNFSe.DataEmissao = DateTime.Now;
                novaNFSe.DataPrevistaEntrega = DateTime.Now;
                novaNFSe.DataAutorizacao = null;
                novaNFSe.DataCancelamento = null;
                novaNFSe.DataRetornoSefaz = null;
                novaNFSe.MensagemRetornoSefaz = null;
                novaNFSe.Protocolo = null;
                novaNFSe.Cancelado = "N";
                novaNFSe.LogIntegracao = "Gerado no servidor (MachineName: " + System.Environment.MachineName + "). Aplicação (" + System.Reflection.Assembly.GetExecutingAssembly().CodeBase + "). ";
                novaNFSe.TipoControle = repCTe.BuscarUltimoTipoControle() + 1;
                novaNFSe.Numero = serCTe.ObterProximoNumero(nfse, repCTe);

                novaNFSe.Empresa = nfse.Empresa;
                novaNFSe.ModalTransporte = nfse.ModalTransporte;
                novaNFSe.NaturezaDaOperacao = nfse.NaturezaDaOperacao;
                novaNFSe.CFOP = nfse.CFOP;
                novaNFSe.NaturezaNFSe = nfse.NaturezaNFSe;
                novaNFSe.ModeloDocumentoFiscal = nfse.ModeloDocumentoFiscal;
                novaNFSe.LocalidadeEmissao = nfse.LocalidadeEmissao;
                novaNFSe.LocalidadeInicioPrestacao = nfse.LocalidadeInicioPrestacao;
                novaNFSe.LocalidadeTerminoPrestacao = nfse.LocalidadeTerminoPrestacao;

                novaNFSe.Serie = nfse.Serie;
                novaNFSe.TipoImpressao = nfse.TipoImpressao;
                novaNFSe.TipoEmissao = nfse.TipoEmissao;
                novaNFSe.Versao = nfse.Versao;
                novaNFSe.SerieRPS = nfse.SerieRPS;

                novaNFSe.ValorPrestacaoServico = nfse.ValorPrestacaoServico;
                novaNFSe.ValorAReceber = nfse.ValorAReceber;
                novaNFSe.ValorPIS = nfse.ValorPIS;
                novaNFSe.ValorCOFINS = nfse.ValorCOFINS;
                novaNFSe.ValorIR = nfse.ValorIR;
                novaNFSe.ValorINSS = nfse.ValorINSS;
                novaNFSe.ValorCSLL = nfse.ValorCSLL;
                novaNFSe.ISSRetido = nfse.ISSRetido;
                novaNFSe.ValorISSRetido = nfse.ValorISSRetido;
                novaNFSe.ValorOutrasRetencoes = nfse.ValorOutrasRetencoes;
                novaNFSe.ValorDescontoIncondicionado = nfse.ValorDescontoIncondicionado;
                novaNFSe.ValorDescontoCondicionado = nfse.ValorDescontoCondicionado;
                novaNFSe.AliquotaISS = nfse.AliquotaISS;
                novaNFSe.BaseCalculoISS = nfse.BaseCalculoISS;
                novaNFSe.ValorISS = nfse.ValorISS;
                novaNFSe.SerieSubstituicao = nfse.SerieSubstituicao;
                novaNFSe.NumeroSubstituicao = nfse.NumeroSubstituicao;
                novaNFSe.ValorDeducoes = nfse.ValorDeducoes;
                novaNFSe.SistemaEmissor = nfse.SistemaEmissor;
                novaNFSe.BaseCalculoIBSCBS = nfse.BaseCalculoIBSCBS;
                novaNFSe.ValorCBS = nfse.ValorCBS;
                novaNFSe.ValorIBSEstadual = nfse.ValorIBSEstadual;
                novaNFSe.ValorIBSMunicipal = nfse.ValorIBSMunicipal;
                novaNFSe.AliquotaCBS = nfse.AliquotaCBS;
                novaNFSe.AliquotaIBSEstadual = nfse.AliquotaIBSEstadual;
                novaNFSe.AliquotaIBSMunicipal = nfse.AliquotaIBSMunicipal;
                novaNFSe.PercentualReducaoIBSEstadual = nfse.PercentualReducaoIBSEstadual;
                novaNFSe.PercentualReducaoIBSMunicipal = nfse.PercentualReducaoIBSMunicipal;
                novaNFSe.PercentualReducaoCBS = nfse.PercentualReducaoCBS;
                novaNFSe.OutrasAliquotas = nfse.OutrasAliquotas;
                novaNFSe.NBS = nfse.NBS;
                novaNFSe.CodigoIndicadorOperacao = nfse.CodigoIndicadorOperacao;
                novaNFSe.CSTIBSCBS = nfse.CST;
                novaNFSe.ClassificacaoTributariaIBSCBS = nfse.ClassificacaoTributariaIBSCBS;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(nfse.Remetente.CPF_CNPJ));
                cte.Remetente = serCliente.ObterClienteCTE(cliente, endereco);

                serCTe.ObterParticipante(ref novaNFSe, cte.Remetente, Dominio.Enumeradores.TipoTomador.Remetente, unitOfWork);

                repCTe.Inserir(novaNFSe);

                for (int i = 0; i < itens.Count; i++)
                {
                    Dominio.Entidades.NFSeItem novoItem = new Dominio.Entidades.NFSeItem();
                    novoItem = itens[i].Clonar();
                    novoItem.CTe = novaNFSe;

                    repNFSeItem.Inserir(novoItem);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfse, null, "Duplicou NFS-e para " + novaNFSe.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar duplicar a nota selecionada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);
                int codigoNFSe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFSe);

                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigoNFSe);
                unitOfWork.CommitChanges();

                bool sucesso = servicoNFSe.EmitirNFSe(codigoNFSe, unitOfWork);
                if (sucesso)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, nfse, null, "Enviou NFS-e.", unitOfWork);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, nfse.MensagemRetornoSefaz);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir a NFS-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoNFSe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFSe);

                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigoNFSe);

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoMensalClienteServico = repFaturamentoMensalClienteServico.BuscarPorNFSe(codigoNFSe);

                if (faturamentoMensalClienteServico != null && faturamentoMensalClienteServico.FaturamentoMensal.StatusFaturamentoMensal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.Finalizado)
                    return new JsonpResult(false, true, "Favor Finalizar o Faturamento Mensal que contêm essa NFS-e antes de Cancelar");

                unitOfWork.CommitChanges();

                bool sucesso = servicoNFSe.CancelarNFSe(codigoNFSe, unitOfWork);
                if (sucesso)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, nfse, null, "Cancelou NFS-e.", unitOfWork);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, nfse.RPS.MensagemRetorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar cancelar a nota selecionada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDANFSE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFSe;
                int.TryParse(Request.Params("Codigo"), out codigoNFSe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return new JsonpResult(true, false, "NFS-e não encontrada, atualize a página e tente novamente.");

                if (nfse.Status != "A" && nfse.Status != "C" && nfse.Status != "K")
                    return new JsonpResult(true, false, "O status da NFS-e não permite a geração da DANFSE.");

                if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para os download da DANFSE não está disponível. Contate o suporte técnico.");

                string nomeArquivo = nfse.Empresa.CNPJ + "_" + nfse.Numero.ToString() + "_" + nfse.Serie.Numero.ToString() + ".pdf";
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, nfse.Empresa.CNPJ, nomeArquivo) + ".pdf";

                byte[] pdf = null;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {
                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);

                    pdf = servicoNFSe.ObterDANFSECTe(nfse.Codigo);
                }
                else
                {
                    pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (pdf != null)
                    return Arquivo(pdf, "application/pdf", System.IO.Path.GetFileName(caminhoPDF));
                else
                    return new JsonpResult(false, false, "Não foi possível gerar a DANFSE, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da DANFSE");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoNFSe;
                int.TryParse(Request.Params("Codigo"), out codigoNFSe);

                if (codigoNFSe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigoNFSe);

                    if (nfse != null)
                    {
                        Servicos.CTe servicoNFSe = new Servicos.CTe(unitOfWork);

                        byte[] data = servicoNFSe.ObterXMLAutorizacao(nfse);

                        if (data != null)
                            return Arquivo(data, "text/xml", string.Concat("NFSe_", nfse.Numero, ".xml"));
                    }
                }

                return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CarregarPedidoVendaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                List<int> codigos = new List<int>();
                codigos = RetornaCodigosPedidos(unitOfWork);

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda = repPedidoVenda.BuscarPorCodigos(codigos);

                if (pedidosVenda == null || pedidosVenda.Count <= 0)
                    return new JsonpResult(false, "Pedido Venda não encontrado.");

                var retorno = new
                {
                    NFSe = ObterDetalhesPedidoVenda(pedidosVenda[0]),
                    Servicos = ObterItensPedidoVenda(pedidosVenda, codigos, unitOfWork),
                    Observacao = ObterObservacaoPedidoVenda(pedidosVenda),
                    Parcelas = ObterParcelasPedidoVenda(pedidosVenda, codigos, unitOfWork)
                };

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao carregar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarProtocoloNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigoNFSe);
                string protocoloNFSe = Request.Params("Protocolo");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigoNFSe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfse, null, "Solicitou a consulta da nota por Protocolo", unitOfWork);

                unitOfWork.CommitChanges();

                string retorno = servicoNFSe.ConsultarProtocoloPrefeituraNFSe(codigoNFSe, protocoloNFSe, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o protocolo de consulta da NFS-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EditarNumeroNFSeENumeroRPS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigoNFSe);
                int.TryParse(Request.Params("NumeroNFSe"), out int NumeroNFSe);
                int.TryParse(Request.Params("NumeroRPS"), out int NumeroRPS);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarPorCodigo(codigoNFSe);

                if (nfse != null)
                {
                    nfse.Numero = NumeroNFSe;
                    nfse.RPS.Numero = NumeroRPS;

                    repCTe.Atualizar(nfse, Auditado);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar atualizaar os número da NFSe ou RPS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico SalvarDadosNFSe(Repositorio.UnitOfWork unitOfWork, dynamic dynNFSe, Dominio.ObjetosDeValor.CTe.CTe cte, Dominio.Entidades.Cliente tomador, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal)
        {
            Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new Servicos.Embarcador.NotaFiscal.NotaFiscalProduto(unitOfWork);

            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.NaturezaNFSe repNaturezaDaOperacao = new Repositorio.NaturezaNFSe(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);

            int numeroDocumento = 0, codigoCTe = 0, codigoEmpresa;
            cte.CFOP = 5353;

            numeroDocumento = dynNFSe.NFSe.Numero != null ? (int)dynNFSe.NFSe.Numero : 0;
            codigoCTe = dynNFSe.NFSe.Codigo != null ? (int)dynNFSe.NFSe.Codigo : 0;
            codigoEmpresa = dynNFSe.NFSe.Empresa != null ? (int)dynNFSe.NFSe.Empresa : 0;

            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorCodigo(codigoCTe);

            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(codigoEmpresa);
            Dominio.Entidades.Localidade localidadePrestacao = repLocalidade.BuscarPorCodigo((int)dynNFSe.NFSe.CidadePrestacao);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (conhecimento != null)
            {
                cte.Remetente = serCliente.ObterClienteCTE(conhecimento.Remetente);
                cte.Expedidor = serCliente.ObterClienteCTE(conhecimento.Expedidor);
                cte.Recebedor = serCliente.ObterClienteCTE(conhecimento.Recebedor);
                cte.Tomador = serCliente.ObterClienteCTE(conhecimento.OutrosTomador);
                cte.Destinatario = serCliente.ObterClienteCTE(conhecimento.Destinatario);

                if (conhecimento.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                    cte.Remetente = serCliente.ObterClienteCTE(tomador, pedidoEndereco: null);
                else if (conhecimento.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                    cte.Expedidor = serCliente.ObterClienteCTE(tomador, pedidoEndereco: null);
                else if (conhecimento.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                    cte.Recebedor = serCliente.ObterClienteCTE(tomador, pedidoEndereco: null);
                else if (conhecimento.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    cte.Tomador = serCliente.ObterClienteCTE(tomador, pedidoEndereco: null);
                else
                    cte.Destinatario = serCliente.ObterClienteCTE(tomador, pedidoEndereco: null);

                cte.TipoTomador = conhecimento.TipoTomador;
                cte.IncluirISSNoFrete = conhecimento.IncluirISSNoFrete;
            }
            else
            {
                cte.Remetente = serCliente.ObterClienteCTE(tomador, pedidoEndereco: null);
                cte.Destinatario = serCliente.ObterClienteCTE(tomador, pedidoEndereco: null);
            }

            cte.Emitente = Servicos.Empresa.ObterEmpresaCTE(empresa);
            cte.CodigoIBGECidadeInicioPrestacao = localidadePrestacao.CodigoIBGE;
            cte.CodigoIBGECidadeTerminoPrestacao = localidadePrestacao.CodigoIBGE;

            DateTime dataEmissao = new DateTime();
            DateTime.TryParseExact((string)dynNFSe.NFSe.DataEmissao, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            cte.DataEmissao = dataEmissao.ToString("dd/MM/yyyy HH:mm:ss");

            Dominio.Entidades.Cliente cliTomador = null;
            cliTomador = tomador;

            cte.Serie = repSerie.BuscarPorCodigo(dynNFSe.NFSe.Serie != null ? (int)dynNFSe.NFSe.Serie : 0).Numero;
            cte.SerieRPS = transportadorConfiguracaoNFSe != null ? transportadorConfiguracaoNFSe.SerieRPS : string.Empty;

            cte.NaturezaNFSe = new Dominio.ObjetosDeValor.CTe.NaturezaNFSe();
            cte.NaturezaNFSe.CodigoInterno = repNaturezaDaOperacao.BuscarPorCodigo((int)dynNFSe.NFSe.NaturezaOperacao).Codigo;
            cte.ISS = new Dominio.ObjetosDeValor.CTe.ImpostoISS();
            cte.ItensNFSe = new List<Dominio.ObjetosDeValor.CTe.ItemNFSe>();

            bool issRetido = (int)dynNFSe.Valor.RetencaoISS == 1;

            decimal aliquotaPIS, aliquotaCOFINS, aliquotaISS, valorTotalServicos, basePIS, baseCOFINS, baseISS, valorISS, baseDeducao, valorOutrasRetencoes, valorDescontoIncondicional, valorDescontoCondicional, valorRetencaoISS,
                valorPIS, valorCOFINS, valorINSS, valorIR, valorCSLL;

            decimal.TryParse((string)dynNFSe.Valor.BasePIS, out basePIS);
            decimal.TryParse((string)dynNFSe.Valor.BaseCOFINS, out baseCOFINS);
            decimal.TryParse((string)dynNFSe.Valor.AliquotaPIS, out aliquotaPIS);
            decimal.TryParse((string)dynNFSe.Valor.AliquotaCOFINS, out aliquotaCOFINS);
            decimal.TryParse((string)dynNFSe.Valor.ValorPIS, out valorPIS);
            decimal.TryParse((string)dynNFSe.Valor.ValorCOFINS, out valorCOFINS);

            decimal.TryParse((string)dynNFSe.Valor.ValorTotalServicos, out valorTotalServicos);
            decimal.TryParse((string)dynNFSe.Valor.BaseDeducao, out baseDeducao);
            decimal.TryParse((string)dynNFSe.Valor.ValorINSS, out valorINSS);
            decimal.TryParse((string)dynNFSe.Valor.ValorIR, out valorIR);

            decimal.TryParse((string)dynNFSe.Valor.ValorCSLL, out valorCSLL);
            decimal.TryParse((string)dynNFSe.Valor.ValorOutrasRetencoes, out valorOutrasRetencoes);
            decimal.TryParse((string)dynNFSe.Valor.ValorDescontoIncondicional, out valorDescontoIncondicional);
            decimal.TryParse((string)dynNFSe.Valor.ValorDescontoCondicional, out valorDescontoCondicional);
            decimal.TryParse((string)dynNFSe.Valor.ValorRetencaoISS, out valorRetencaoISS);

            decimal.TryParse((string)dynNFSe.Valor.BaseISS, out baseISS);
            decimal.TryParse((string)dynNFSe.Valor.AliquotaISS, out aliquotaISS);
            decimal.TryParse((string)dynNFSe.Valor.ValorISS, out valorISS);

            cte.ISS.Aliquota = aliquotaISS;
            cte.ISS.BaseCalculo = baseISS;
            cte.ISS.Valor = valorISS;
            cte.ISS.ValorRetencao = valorRetencaoISS;
            cte.ISS.PercentualRetencao = conhecimento != null ? conhecimento.PercentualISSRetido : 0;

            cte.IBSCBS = new Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS();
            cte.IBSCBS.BaseCalculo = dynNFSe?.Valor?.BaseCalculoIBSCBS != null ? Convert.ToDecimal((string)dynNFSe.Valor.BaseCalculoIBSCBS) : 0m;

            cte.IBSCBS.AliquotaIBSEstadual = dynNFSe?.Valor?.AliquotaIBSEstadual != null ? Convert.ToDecimal((string)dynNFSe.Valor.AliquotaIBSEstadual) : 0m;
            cte.IBSCBS.PercentualReducaoIBSEstadual = dynNFSe?.Valor?.PercentualReducaoIBSEstadual != null ? Convert.ToDecimal((string)dynNFSe.Valor.PercentualReducaoIBSEstadual) : 0m;
            cte.IBSCBS.ValorIBSEstadual = dynNFSe?.Valor?.ValorIBSEstadual != null ? Convert.ToDecimal((string)dynNFSe.Valor.ValorIBSEstadual) : 0m;

            cte.IBSCBS.AliquotaIBSMunicipal = dynNFSe?.Valor?.AliquotaIBSMunicipal != null ? Convert.ToDecimal((string)dynNFSe.Valor.AliquotaIBSMunicipal) : 0m;
            cte.IBSCBS.PercentualReducaoIBSMunicipal = dynNFSe?.Valor?.PercentualReducaoIBSMunicipal != null ? Convert.ToDecimal((string)dynNFSe.Valor.PercentualReducaoIBSMunicipal) : 0m;
            cte.IBSCBS.ValorIBSMunicipal = dynNFSe?.Valor?.ValorIBSMunicipal != null ? Convert.ToDecimal((string)dynNFSe.Valor.ValorIBSMunicipal) : 0m;

            cte.IBSCBS.AliquotaCBS = dynNFSe?.Valor?.AliquotaCBS != null ? Convert.ToDecimal((string)dynNFSe.Valor.AliquotaCBS) : 0m;
            cte.IBSCBS.PercentualReducaoCBS = dynNFSe?.Valor?.PercentualReducaoCBS != null ? Convert.ToDecimal((string)dynNFSe.Valor.PercentualReducaoCBS) : 0m;
            cte.IBSCBS.ValorCBS = dynNFSe?.Valor?.ValorCBS != null ? Convert.ToDecimal((string)dynNFSe.Valor.ValorCBS) : 0m;

            cte.ValorDescontoIncondicionado = valorDescontoIncondicional;
            cte.ValorDescontoCondicionado = valorDescontoCondicional;
            cte.ValorDeducoes = baseDeducao;
            cte.ValorOutrasRetencoes = valorOutrasRetencoes;
            cte.ValorTotalPrestacaoServico = valorTotalServicos;
            cte.ValorAReceber = cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? valorTotalServicos - valorRetencaoISS : valorTotalServicos;
            cte.ValorFrete = conhecimento?.ValorFrete ?? valorTotalServicos;

            cte.ObservacoesGerais = !string.IsNullOrWhiteSpace((string)dynNFSe.NFSe.Observacao) ? (string)dynNFSe.NFSe.Observacao : string.Empty;
            decimal valorTotalIBPT = 0;
            decimal valorIPBTNacional = 0;
            decimal valorIPBTEstadual = 0;
            decimal valorIPBTMunicipal = 0;

            decimal baseTotalPis = 0;
            decimal baseTotalCofins = 0;
            decimal aliquotaTotalPis = 0;
            decimal aliquotaTotalCofins = 0;
            decimal valorTotalPis = 0;
            decimal valorTotalCofins = 0;

            foreach (var serv in dynNFSe.Servicos)
            {
                Dominio.ObjetosDeValor.CTe.ItemNFSe item = new Dominio.ObjetosDeValor.CTe.ItemNFSe();

                Dominio.Entidades.Localidade localidadeItem = repLocalidade.BuscarPorCodigo((int)serv.CodigoLocalidade);

                decimal descontoCondicionalItem, descontoIncondicionalItem, deducaoItem, baseISSItem, aliquotaISSItem, valorTotalItem, valorUnitario, valorISSItem, quantidade, baseItemPIS, baseItemCOFINS, aliquotaItemPIS, 
                    aliquotaItemCOFINS, valorItemPIS, valorItemCOFINS;

                decimal.TryParse((string)serv.DescontoCondicional, out descontoCondicionalItem);
                decimal.TryParse((string)serv.DescontoIncondicional, out descontoIncondicionalItem);
                decimal.TryParse((string)serv.Deducao, out deducaoItem);
                decimal.TryParse((string)serv.BCISS, out baseISSItem);
                decimal.TryParse((string)serv.AliquotaISS, out aliquotaISSItem);

                decimal.TryParse((string)serv.Qtd, out quantidade);
                decimal.TryParse((string)serv.ValorISS, out valorISSItem);
                decimal.TryParse((string)serv.ValorTotal, out valorTotalItem);
                decimal.TryParse((string)serv.ValorUnitario, out valorUnitario);

                string cstItemPIS = (string)serv.CSTPIS;
                string cstItemCOFINS = (string)serv.CSTCOFINS;
                decimal.TryParse((string)serv.BasePIS, out baseItemPIS);
                decimal.TryParse((string)serv.BaseCOFINS, out baseItemCOFINS);
                decimal.TryParse((string)serv.AliquotaPIS, out aliquotaItemPIS);
                decimal.TryParse((string)serv.AliquotaCOFINS, out aliquotaItemCOFINS);
                decimal.TryParse((string)serv.ValorPIS, out valorItemPIS);
                decimal.TryParse((string)serv.ValorCOFINS, out valorItemCOFINS);

                item.PIS = new Dominio.ObjetosDeValor.CTe.ImpostoPIS();
                item.PIS.CST = cstItemPIS;
                item.PIS.BaseCalculo = baseItemPIS;
                item.PIS.Aliquota = aliquotaItemPIS;
                item.PIS.Valor = valorItemPIS;

                item.COFINS = new Dominio.ObjetosDeValor.CTe.ImpostoCOFINS();
                item.COFINS.CST = cstItemCOFINS;
                item.COFINS.BaseCalculo = baseItemCOFINS;
                item.COFINS.Aliquota = aliquotaItemCOFINS;
                item.COFINS.Valor = valorItemCOFINS;

                baseTotalPis += baseItemPIS;
                baseTotalCofins += baseItemCOFINS;
                aliquotaTotalPis = aliquotaItemPIS;
                aliquotaTotalCofins = aliquotaItemCOFINS;
                valorTotalPis += valorItemPIS;
                valorTotalCofins += valorItemCOFINS;

                item.ValorDescontoIncondicionado = descontoIncondicionalItem;
                item.ValorDescontoCondicionado = descontoCondicionalItem;
                item.ValorDeducoes = deducaoItem;
                item.AliquotaISS = aliquotaISSItem;
                item.BaseCalculoISS = baseISSItem;
                item.CodigoIBGECidade = localidadeItem.CodigoIBGE;
                item.CodigoIBGECidadeIncidencia = localidadeItem.CodigoIBGE;
                item.CodigoPaisPrestacaoServico = localidadeItem.Pais.Codigo;
                item.Discriminacao = !string.IsNullOrWhiteSpace((string)serv.Discriminacao) ? (string)serv.Discriminacao : string.Empty;
                item.ExigibilidadeISS = (int)serv.CodigoExigibilidade;
                item.ISSInclusoValorTotal = false;
                item.Quantidade = quantidade;
                item.Servico = new Dominio.ObjetosDeValor.CTe.ServicoNFSe();
                item.Servico.CodigoInterno = (int)serv.CodigoServico;
                item.ServicoPrestadoNoPais = true;
                item.ValorServico = valorUnitario;
                item.ValorISS = valorISSItem;
                item.ValorTotal = valorTotalItem;

                item.IBSCBS = new Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS();

                item.IBSCBS.NBS = serv.NBS != null ? (string)serv.NBS : string.Empty;
                item.IBSCBS.CodigoIndicadorOperacao = serv.CodigoIndicadorOperacao != null ? (string)serv.CodigoIndicadorOperacao : string.Empty;
                item.IBSCBS.CST = serv.CSTIBSCBS != null ? (string)serv.CSTIBSCBS : string.Empty;
                item.IBSCBS.ClassificacaoTributaria = serv.ClassificacaoTributariaIBSCBS != null ? (string)serv.ClassificacaoTributariaIBSCBS : string.Empty;

                item.IBSCBS.BaseCalculo = serv.BaseCalculoIBSCBS != null ? Convert.ToDecimal((string)serv.BaseCalculoIBSCBS) : 0m;

                item.IBSCBS.AliquotaIBSEstadual = serv.AliquotaIBSEstadual != null ? Convert.ToDecimal((string)serv.AliquotaIBSEstadual) : 0m;
                item.IBSCBS.PercentualReducaoIBSEstadual = serv.PercentualReducaoIBSEstadual != null ? Convert.ToDecimal((string)serv.PercentualReducaoIBSEstadual) : 0m;
                item.IBSCBS.ValorIBSEstadual = serv.ValorIBSEstadual != null ? Convert.ToDecimal((string)serv.ValorIBSEstadual) : 0m;

                item.IBSCBS.AliquotaIBSMunicipal = serv.AliquotaIBSMunicipal != null ? Convert.ToDecimal((string)serv.AliquotaIBSMunicipal) : 0m;
                item.IBSCBS.PercentualReducaoIBSMunicipal = serv.PercentualReducaoIBSMunicipal != null ? Convert.ToDecimal((string)serv.PercentualReducaoIBSMunicipal) : 0m;
                item.IBSCBS.ValorIBSMunicipal = serv.ValorIBSMunicipal != null ? Convert.ToDecimal((string)serv.ValorIBSMunicipal) : 0m;

                item.IBSCBS.AliquotaCBS = serv.AliquotaCBS != null ? Convert.ToDecimal((string)serv.AliquotaCBS) : 0m;
                item.IBSCBS.PercentualReducaoCBS = serv.PercentualReducaoCBS != null ? Convert.ToDecimal((string)serv.PercentualReducaoCBS) : 0m;
                item.IBSCBS.ValorCBS = serv.ValorCBS != null ? Convert.ToDecimal((string)serv.ValorCBS) : 0m;

                cte.ItensNFSe.Add(item);

                if (empresa.CalculaIBPTNFe)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = repServico.BuscarPorCodigoServicoNFSe(item.Servico.CodigoInterno);
                    if (servico != null)
                    {
                        var listaServico = Utilidades.String.OnlyNumbers(servico.NumeroCodigoServico);
                        if (!string.IsNullOrWhiteSpace(listaServico))
                        {
                            valorTotalIBPT += Math.Round(serNotaFiscalProduto.RetornaValorIBPT(empresa.EmpresaPai?.Codigo ?? 0, empresa.Codigo, listaServico, item.ValorTotal, unitOfWork, 3), 2);
                            valorIPBTNacional += Math.Round(serNotaFiscalProduto.RetornaValorIBPT(empresa.EmpresaPai?.Codigo ?? 0, empresa.Codigo, listaServico, item.ValorTotal, unitOfWork, 0), 2);
                            valorIPBTEstadual += Math.Round(serNotaFiscalProduto.RetornaValorIBPT(empresa.EmpresaPai?.Codigo ?? 0, empresa.Codigo, listaServico, item.ValorTotal, unitOfWork, 1), 2);
                            valorIPBTMunicipal += Math.Round(serNotaFiscalProduto.RetornaValorIBPT(empresa.EmpresaPai?.Codigo ?? 0, empresa.Codigo, listaServico, item.ValorTotal, unitOfWork, 2), 2);
                        }
                    }
                }
            }

            if (empresa.CalculaIBPTNFe)
            {
                if (codigoCTe > 0)
                {
                    if (cte.ObservacoesGerais.Contains("Valor aproximado dos tributos") && cte.ObservacoesGerais.Contains(" - Fonte: IBPT"))
                    {
                        int posInicial = cte.ObservacoesGerais.IndexOf("Valor aproximado dos tributos");
                        int posFinal = cte.ObservacoesGerais.IndexOf(" - Fonte: IBPT") + 14;
                        cte.ObservacoesGerais = cte.ObservacoesGerais.Remove(posInicial, posFinal - posInicial);
                    }
                }

                if (valorTotalIBPT > 0)
                    cte.ObservacoesGerais += " Valor aproximado dos tributos com base na Lei 12.741/2012 - R$ " + valorTotalIBPT.ToString("n2") + " (" + ((valorTotalIBPT * 100) / cte.ValorTotalPrestacaoServico).ToString("n2") + " %) ";
                if (valorIPBTNacional > 0)
                    cte.ObservacoesGerais += " - Nacional R$ " + valorIPBTNacional.ToString("n2");
                if (valorIPBTEstadual > 0)
                    cte.ObservacoesGerais += " - Estadual R$ " + valorIPBTEstadual.ToString("n2");
                if (valorIPBTMunicipal > 0)
                    cte.ObservacoesGerais += " - Municipal R$ " + valorIPBTMunicipal.ToString("n2");
                if (valorTotalIBPT > 0)
                    cte.ObservacoesGerais += " - Fonte: IBPT";

                cte.ObservacoesGerais = cte.ObservacoesGerais.Trim();
            }

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCte.GerarCTePorObjeto(cte, codigoCTe, unitOfWork, "1", 0, "S", modeloDocumentoFiscal, numeroDocumento, TipoServicoMultisoftware);

            cteIntegrado.BasePIS = basePIS > 0 ? basePIS : baseTotalPis;
            cteIntegrado.BaseCOFINS = baseCOFINS > 0 ? baseCOFINS : baseTotalCofins;
            cteIntegrado.AliquotaPIS = aliquotaPIS > 0 ? aliquotaPIS : aliquotaTotalPis;
            cteIntegrado.AliquotaCOFINS = aliquotaCOFINS > 0 ? aliquotaCOFINS : aliquotaTotalCofins;
            cteIntegrado.ValorPIS = valorPIS > 0 ? valorPIS : valorTotalPis;
            cteIntegrado.ValorCOFINS = valorCOFINS > 0 ? valorCOFINS : valorTotalCofins;
            cteIntegrado.ValorCSLL = valorCSLL;
            cteIntegrado.ValorIR = valorIR;
            cteIntegrado.ValorINSS = valorINSS;
            cteIntegrado.ISSRetido = issRetido;

            if (!string.IsNullOrWhiteSpace((string)dynNFSe.Substituicao.Numero) && !string.IsNullOrWhiteSpace((string)dynNFSe.Substituicao.Serie))
            {
                cteIntegrado.NumeroSubstituicao = (int)dynNFSe.Substituicao.Numero;
                cteIntegrado.SerieSubstituicao = (string)dynNFSe.Substituicao.Serie;
            }
            else
            {
                cteIntegrado.NumeroSubstituicao = null;
                cteIntegrado.SerieSubstituicao = null;
            }

            repCTe.Atualizar(cteIntegrado, Auditado);

            if (cliTomador.Localidade.Codigo != empresa.Localidade.Codigo)
            {
                cteIntegrado.TomadorPagador.InscricaoMunicipal = "";
                repParticipanteCTe.Atualizar(cteIntegrado.TomadorPagador);
            }

            return cteIntegrado;

        }

        private Boolean SalvarParcelasNFSe(Repositorio.UnitOfWork unitOfWork, dynamic dynNFSe, Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse)
        {

            Repositorio.Embarcador.CTe.CTeParcela repParcelaNFSe = new Repositorio.Embarcador.CTe.CTeParcela(unitOfWork);
            if (nfse.Codigo > 0)
                repParcelaNFSe.DeletarPorNFSe(nfse.Codigo);

            foreach (var parc in dynNFSe.Parcelas)
            {
                Dominio.Entidades.Embarcador.CTe.CTeParcela parcelaNFSe = new Dominio.Entidades.Embarcador.CTe.CTeParcela();

                int sequencia = 0;
                sequencia = parc.Sequencia != null ? (int)parc.Sequencia : 0;

                DateTime dataEmissao = DateTime.Now;
                DateTime dataVencimento = new DateTime();
                DateTime.TryParseExact((string)parc.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

                decimal valorParcela;
                decimal.TryParse((string)parc.Valor, out valorParcela);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Enum.TryParse((string)parc.FormaTitulo, out formaTitulo);

                parcelaNFSe.Sequencia = sequencia;
                parcelaNFSe.DataEmissao = dataEmissao;
                parcelaNFSe.DataVencimento = dataVencimento;
                parcelaNFSe.Valor = valorParcela;
                parcelaNFSe.Forma = formaTitulo;

                parcelaNFSe.ConhecimentoDeTransporteEletronico = nfse;

                repParcelaNFSe.Inserir(parcelaNFSe);
            }

            return true;
        }

        private object ObterDetalhesNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse)
        {
            var retorno = new
            {
                Codigo = nfse.Codigo,
                Numero = nfse.Numero,
                Serie = new { Codigo = nfse.Serie.Codigo, Descricao = nfse.Serie.Numero },
                Empresa = new { Codigo = nfse.Empresa.Codigo, Descricao = nfse.Empresa.RazaoSocial },
                DataEmissao = nfse.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                Pessoa = new
                {
                    Codigo = nfse.TomadorPagador?.CPF_CNPJ ?? nfse.Remetente.CPF_CNPJ,
                    Descricao = nfse.TomadorPagador != null ? nfse.TomadorPagador.Nome + " (" + nfse.TomadorPagador.Localidade.DescricaoCidadeEstado + ")" : nfse.TomadorPagador.Nome + " (" + nfse.Remetente.Localidade.DescricaoCidadeEstado + ")"
                },
                CidadePrestacao = new { Codigo = nfse.LocalidadeInicioPrestacao.Codigo, Descricao = nfse.LocalidadeInicioPrestacao.DescricaoCidadeEstado },
                NaturezaOperacao = nfse.NaturezaNFSe != null ? new { Codigo = nfse.NaturezaNFSe.Codigo, Descricao = nfse.NaturezaNFSe.Descricao } : null,
                Observacao = nfse.ObservacoesGerais
            };

            return retorno;
        }

        private object ObterItensNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSeItem repNFSeItem = new Repositorio.NFSeItem(unitOfWork);
            List<Dominio.Entidades.NFSeItem> itens = repNFSeItem.BuscarPorCTe(nfse.Codigo);

            var retorno = (from obj in itens
                           select new
                           {
                               obj.Codigo,
                               CodigoServico = obj.Servico.Codigo,
                               obj.Servico.Descricao,
                               CodigoLocalidade = obj.Municipio.Codigo,
                               DescricaoLocalidade = obj.Municipio.Descricao + " - " + obj.Municipio.Estado.Sigla,
                               Qtd = obj.Quantidade.ToString("n2"),
                               ValorUnitario = obj.ValorServico.ToString("n2"),
                               Deducao = obj.ValorDeducoes.ToString("n2"),
                               DescontoIncondicional = obj.ValorDescontoIncondicionado.ToString("n2"),
                               DescontoCondicional = obj.ValorDescontoCondicionado.ToString("n2"),
                               ValorTotal = obj.ValorTotal.ToString("n2"),
                               BCISS = obj.BaseCalculoISS.ToString("n2"),
                               AliquotaISS = obj.AliquotaISS.ToString("n4"),
                               ValorISS = obj.ValorISS.ToString("n2"),

                               obj.CSTPIS,
                               obj.CSTCOFINS,
                               BasePIS = obj.BaseCalculoPIS.ToString("n2"),
                               BaseCOFINS = obj.BaseCalculoCOFINS.ToString("n2"),
                               AliquotaPIS = obj.AliquotaPIS.ToString("n4"),
                               AliquotaCOFINS = obj.AliquotaCOFINS.ToString("n4"),
                               ValorPIS = obj.ValorPIS.ToString("n2"),
                               ValorCOFINS = obj.ValorCOFINS.ToString("n2"),

                               CodigoExigibilidade = (int)obj.ExigibilidadeISS,
                               obj.Discriminacao,
                               NBS = obj.NBS ?? string.Empty,
                               CodigoIndicadorOperacao = obj.CodigoIndicadorOperacao ?? string.Empty,
                               CSTIBSCBS = obj.CSTIBSCBS ?? string.Empty,
                               ClassificacaoTributariaIBSCBS = obj.ClassificacaoTributariaIBSCBS ?? string.Empty,
                               BaseCalculoIBSCBS = obj.BaseCalculoIBSCBS.ToString("n2"),
                               AliquotaIBSEstadual = obj.AliquotaIBSEstadual.ToString("n4"),
                               PercentualReducaoIBSEstadual = obj.PercentualReducaoIBSEstadual.ToString("n4"),
                               ValorIBSEstadual = obj.ValorIBSEstadual.ToString("n2"),
                               AliquotaIBSMunicipal = obj.AliquotaIBSMunicipal.ToString("n4"),
                               PercentualReducaoIBSMunicipal = obj.PercentualReducaoIBSMunicipal.ToString("n4"),
                               ValorIBSMunicipal = obj.ValorIBSMunicipal.ToString("n2"),
                               AliquotaCBS = obj.AliquotaCBS.ToString("n4"),
                               PercentualReducaoCBS = obj.PercentualReducaoCBS.ToString("n4"),
                               ValorCBS = obj.ValorCBS.ToString("n2"),
                               AliquotaEfetivaIBSMunicipal = obj.AliquotaIBSMunicipal - (obj.AliquotaIBSMunicipal * (obj.PercentualReducaoIBSMunicipal / 100)),
                               AliquotaEfetivaIBSEstadual = obj.AliquotaIBSEstadual - (obj.AliquotaIBSEstadual * (obj.PercentualReducaoIBSEstadual / 100)),
                               AliquotaEfetivaCBS = obj.AliquotaCBS - (obj.AliquotaCBS * (obj.PercentualReducaoCBS / 100)),
                               ValorIBSMunicipalBruto = obj.BaseCalculoIBSCBS * obj.AliquotaIBSMunicipal,
                               ValorIBSEstadualBruto = obj.BaseCalculoIBSCBS * obj.AliquotaIBSEstadual,
                               ValorCBSBruto = obj.BaseCalculoIBSCBS * obj.AliquotaCBS
                           }).ToList();
            return retorno;
        }

        private object ObterValorNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse)
        {
            var retorno = new
            {
                ValorTotalServicos = nfse.ValorPrestacaoServico,
                BaseDeducao = nfse.ValorDeducoes,
                nfse.AliquotaIBSEstadual,
                nfse.AliquotaIBSMunicipal,
                nfse.ValorIBSEstadual,
                nfse.ValorIBSMunicipal,
                nfse.AliquotaCBS,
                nfse.ValorCBS,

                nfse.CSTPIS,
                nfse.CSTCOFINS,
                nfse.BasePIS,
                nfse.BaseCOFINS,
                nfse.AliquotaPIS,
                nfse.AliquotaCOFINS,
                
                nfse.ValorPIS,
                nfse.ValorCOFINS,
                nfse.ValorINSS,
                nfse.ValorIR,
                nfse.ValorCSLL,
                RetencaoISS = nfse.ISSRetido ? 1 : 0,
                ValorRetencaoISS = nfse.ValorISSRetido,
                nfse.ValorOutrasRetencoes,
                ValorDescontoIncondicional = nfse.ValorDescontoIncondicionado,
                ValorDescontoCondicional = nfse.ValorDescontoCondicionado,
                BaseISS = nfse.BaseCalculoISS,
                AliquotaISS = nfse.AliquotaISS.ToString("n4"),
                nfse.ValorISS,
                ValorTotalLiquido = nfse.ValorPrestacaoServico - nfse.ValorISSRetido
            };

            return retorno;
        }

        private object ObterSubstituicaoNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse)
        {
            var retorno = new
            {
                Numero = nfse.NumeroSubstituicao,
                Serie = nfse.SerieSubstituicao
            };

            return retorno;
        }

        private object ObterParcelamentoNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeParcela repParcelaNFSe = new Repositorio.Embarcador.CTe.CTeParcela(unitOfWork);
            List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas = repParcelaNFSe.BuscarPorNFSe(nfse.Codigo);

            var retorno = (from obj in parcelas
                           select new
                           {
                               obj.Codigo,
                               obj.Sequencia,
                               DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                               Parcela = obj.Sequencia,
                               Valor = obj.Valor.ToString("n2"),
                               DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                               FormaTitulo = obj.Forma
                           }).ToList();

            return retorno;
        }

        private bool SalvarVinculoNotaPedidoVenda(Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse, List<int> codigosPedidoVenda, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                for (int i = 0; i < codigosPedidoVenda.Count; i++)
                {
                    int codigoPedidoVenda = codigosPedidoVenda[i];

                    Repositorio.Embarcador.CTe.CTePedido repNFSePedido = new Repositorio.Embarcador.CTe.CTePedido(unitOfWork);
                    Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigoPedidoVenda);
                    Dominio.Entidades.Embarcador.CTe.CTePedido nfsePedido = new Dominio.Entidades.Embarcador.CTe.CTePedido();

                    nfsePedido.ConhecimentoDeTransporteEletronico = nfse;
                    nfsePedido.PedidoVenda = pedidoVenda;
                    repNFSePedido.Inserir(nfsePedido);
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private List<int> RetornaCodigosPedidos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Codigos")))
            {
                dynamic listaPedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Codigos"));
                if (listaPedidos != null)
                {
                    foreach (var pedido in listaPedidos)
                    {
                        listaCodigos.Add(int.Parse(Utilidades.String.OnlyNumbers((string)pedido.Codigo)));
                    }
                }
            }
            return listaCodigos;
        }

        private List<int> RetornaCodigoPedidoVenda(Repositorio.UnitOfWork unidadeDeTrabalho, string codigoPedidoVenda)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(codigoPedidoVenda))
            {
                dynamic listaPedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(codigoPedidoVenda);
                if (listaPedidos != null)
                {
                    foreach (var pedido in listaPedidos)
                    {
                        listaCodigos.Add(int.Parse(Utilidades.String.OnlyNumbers((string)pedido.Codigo)));
                    }
                }
            }
            return listaCodigos;
        }

        private object ObterDetalhesPedidoVenda(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda)
        {
            var retorno = new
            {
                Pessoa = new
                {
                    Codigo = pedidoVenda.Cliente.Codigo,
                    Descricao = pedidoVenda.Cliente.Nome + " (" + pedidoVenda.Cliente.Localidade.DescricaoCidadeEstado + ")"
                }
            };

            return retorno;
        }

        private object ObterItensPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda, List<int> codigosPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repPedidoVendaItens = new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unitOfWork);
            List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens> itens = repPedidoVendaItens.BuscarPorPedidos(codigosPedido);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);

            var listaServicos = itens.Where(obj => obj.Servico != null && obj.Servico.ServicoNFSe != null).GroupBy(
                                obj => new
                                {
                                    obj.Servico.ServicoNFSe.Codigo,
                                    obj.Servico.ServicoNFSe.Descricao
                                }).Select(obj => new
                                {
                                    obj.Key.Codigo,
                                    obj.Key.Descricao,
                                    Quantidade = obj.Sum(dc => dc.Quantidade),
                                    ValorUnitario = obj.Sum(dc => dc.ValorUnitario),
                                    ValorTotal = obj.Sum(dc => dc.ValorTotal),
                                    Aliquota = obj.Average(dc => dc.Servico.ServicoNFSe.Aliquota)
                                }).ToList();

            List<object> retorno = new List<object>();
            for (int i = 0; i < listaServicos.Count; i++)
            {
                var obj = listaServicos[i];
                retorno.Add(new
                {
                    obj.Codigo,
                    CodigoServico = obj.Codigo,
                    Descricao = obj.Descricao,
                    CodigoLocalidade = empresa.Localidade != null ? empresa.Localidade.Codigo : 0,
                    DescricaoLocalidade = empresa.Localidade != null ? empresa.Localidade.DescricaoCidadeEstado : string.Empty,
                    Qtd = obj.Quantidade.ToString("n2"),
                    ValorUnitario = obj.ValorUnitario.ToString("n2"),
                    Deducao = "0,00",
                    DescontoIncondicional = "0,00",
                    DescontoCondicional = "0,00",
                    ValorTotal = obj.ValorTotal.ToString("n2"),
                    BCISS = obj.ValorTotal.ToString("n2"),
                    AliquotaISS = obj.Aliquota.ToString("n2"),
                    ValorISS = (obj.ValorTotal * (obj.Aliquota / 100)).ToString("n2"),
                    CodigoExigibilidade = 1,
                    Discriminacao = ""
                });
            }

            return retorno;
        }

        private object ObterObservacaoPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda)
        {
            string ObservacaoNFSe = "";
            foreach (var pedidoVenda in pedidosVenda)
            {
                var placa = pedidoVenda.Veiculo != null ? pedidoVenda.Veiculo.Placa : string.Empty;
                if (placa != string.Empty)
                    placa = " - Placa do Veículo: " + placa;

                ObservacaoNFSe += " " + pedidoVenda.Observacao + " - Pedido/OS N: " + pedidoVenda.Numero + placa;
            }
            var retorno = new
            {
                ObservacaoNFSe = ObservacaoNFSe
            };
            return retorno;
        }

        private object ObterParcelasPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda, List<int> codigosPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaParcela repPedidoVendaParcela = new Repositorio.Embarcador.PedidoVenda.PedidoVendaParcela(unitOfWork);
            List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaParcela> parcelas = repPedidoVendaParcela.BuscarPorPedidos(codigosPedido);

            List<object> retorno = new List<object>();
            for (int i = 0; i < parcelas.Count; i++)
            {
                var obj = parcelas[i];
                retorno.Add(new
                {
                    obj.Codigo,
                    obj.Sequencia,
                    Parcela = obj.Sequencia,
                    Valor = obj.Valor.ToString("n2"),
                    DataEmissao = obj.PedidoVenda.DataEmissao.Value.ToString("dd/MM/yyyy"),
                    DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                    FormaTitulo = obj.Forma
                });
            }

            return retorno;
        }

        #endregion
    }
}
