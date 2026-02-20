using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PagamentoMotoristaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("pagamentomotorista.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros, codigoMotorista, codigoCTe, numeroInicial, numeroFinal;
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string status = Request.Params["Status"];

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.PagamentoMotorista repPagamento = new Repositorio.PagamentoMotorista(unitOfWork);

                List<Dominio.Entidades.PagamentoMotorista> pagamentos = repPagamento.Consultar(this.EmpresaUsuario.Codigo, codigoCTe, codigoMotorista, dataInicial, dataFinal, status, numeroInicial, numeroFinal, inicioRegistros, 50);
                int countPagamentos = repPagamento.ContarConsulta(this.EmpresaUsuario.Codigo, codigoCTe, codigoMotorista, dataInicial, dataFinal, status, numeroInicial, numeroFinal);

                var retorno = (from obj in pagamentos
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   DataPagamento = obj.DataPagamento.ToString("dd/MM/yyyy"),
                                   CTe = obj.Ctes != null && obj.Ctes.Count() > 0 ? string.Concat(obj.Ctes.FirstOrDefault().ConhecimentoDeTransporteEletronico.Numero.ToString(), " - ", obj.Ctes.FirstOrDefault().ConhecimentoDeTransporteEletronico.Serie.Numero.ToString()) : string.Empty, //string.Concat(obj.CTE.Numero, " - ", obj.CTE.Serie.Numero),
                                   Motorista = string.Concat(obj.Motorista.CPF, " - ", obj.Motorista.Nome),
                                   ValorFrete = obj.ValorFrete.ToString("n2")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|7", "Dt. Pgto.|15", "CT-e|15", "Motorista|40", "Valor Frete|13" }, countPagamentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os pagamentos de motoristas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.PagamentoMotorista repPagamento = new Repositorio.PagamentoMotorista(unitOfWork);
                Repositorio.PagamentoMotoristaCtes repCtes = new Repositorio.PagamentoMotoristaCtes(unitOfWork);

                Dominio.Entidades.PagamentoMotorista pagamento = repPagamento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (pagamento == null)
                    return Json<bool>(false, false, "Pagamento do motorista não encontrado.");

                List<Dominio.Entidades.PagamentoMotoristaCtes> ctes = repCtes.BuscarPorPagamento(pagamento.Codigo);

                var retorno = new
                {
                    pagamento.Codigo,
                    pagamento.Numero,
                    CodigoCTe = pagamento.CTE != null ? pagamento.CTE.Codigo : 0,
                    DescricaoCTe = pagamento.CTE != null ? string.Concat(pagamento.CTE.Numero, " - ", pagamento.CTE.Serie.Numero) : string.Empty,
                    CodigoMotorista = pagamento.Motorista.Codigo,
                    DescricaoMotorista = string.Concat(pagamento.Motorista.CPF, " - ", pagamento.Motorista.Nome),
                    DataPagamento = pagamento.DataPagamento.ToString("dd/MM/yyyy"),
                    DataRecebimento = pagamento.DataRecebimento.ToString("dd/MM/yyyy"),
                    ValorFrete = pagamento.ValorFrete.ToString("n2"),
                    ValorImpostoRenda = pagamento.ValorImpostoRenda.ToString("n2"),
                    ValorINSSSENAT = pagamento.ValorINSSSENAT.ToString("n2"),
                    ValorSESTSENAT = pagamento.ValorSESTSENAT.ToString("n2"),
                    ValorAdiantamento = pagamento.ValorAdiantamento.ToString("n2"),
                    ValorOutros = pagamento.ValorOutros.ToString("n2"),
                    ValorPedagio = pagamento.ValorPedagio.ToString("n2"),
                    SalarioMotorista = pagamento.SalarioMotorista.ToString("n2"),
                    pagamento.Deduzir,
                    pagamento.Observacao,
                    pagamento.Status,
                    CodigoVeiculo = pagamento.Veiculo != null ? pagamento.Veiculo.Codigo.ToString() : string.Empty,
                    Veiculo = pagamento.Veiculo != null ? pagamento.Veiculo.Placa : string.Empty,
                    Ctes = (from obj in ctes
                            select new Dominio.ObjetosDeValor.PagamentoMotoristaCtes()
                            {
                                Codigo = obj.Codigo,
                                CodigoCte = obj.ConhecimentoDeTransporteEletronico.Codigo,
                                NumeroCte = obj.ConhecimentoDeTransporteEletronico.Numero.ToString(),
                                SerieCte = obj.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),
                                ValorCte = obj.ConhecimentoDeTransporteEletronico.ValorAReceber,
                                ChaveCte = obj.ConhecimentoDeTransporteEletronico.Chave,
                                DescricaoCte = obj.ConhecimentoDeTransporteEletronico.Numero.ToString() + " - " + obj.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),
                                TomadorCte = obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.ConhecimentoDeTransporteEletronico.Remetente?.Nome ?? string.Empty :
                                    obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.ConhecimentoDeTransporteEletronico.Destinatario?.Nome ?? string.Empty :
                                    obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.ConhecimentoDeTransporteEletronico.Expedidor?.Nome ?? string.Empty :
                                    obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.ConhecimentoDeTransporteEletronico.Recebedor?.Nome ?? string.Empty :
                                    obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.ConhecimentoDeTransporteEletronico.OutrosTomador?.Nome ?? string.Empty : string.Empty,
                                Modelo = obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                Excluir = false
                            }).OrderBy(o => o.CodigoCte).ToList(),
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do pagamento do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo, codigoCTe, codigoMotorista, codigoVeiculo;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);

                DateTime dataPagamento, dataRecebimento;
                DateTime.TryParseExact(Request.Params["DataPagamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagamento);
                DateTime.TryParseExact(Request.Params["DataRecebimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataRecebimento);

                decimal valorFrete, valorINSSSENAT, valorSESTSENAT, valorImpostoRenda, valorAdiantamento, valorOutros = 0;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["INSSSENAT"], out valorINSSSENAT);
                decimal.TryParse(Request.Params["SESTSENAT"], out valorSESTSENAT);
                decimal.TryParse(Request.Params["IR"], out valorImpostoRenda);
                decimal.TryParse(Request.Params["INSSSENAT"], out valorINSSSENAT);
                decimal.TryParse(Request.Params["Adiantamento"], out valorAdiantamento);
                decimal.TryParse(Request.Params["ValorOutros"], out valorOutros);
                decimal.TryParse(Request.Params["ValorPedagio"], out decimal valorPedagio);
                decimal.TryParse(Request.Params["SalarioMotorista"], out decimal salarioMotorista);

                string status = Request.Params["Status"];

                Dominio.Enumeradores.OpcaoSimNao deduzir;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["Deduzir"], out deduzir);

                string observacao = Request.Params["Observacao"];

                Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = codigoVeiculo == 0 ? null : repVeiculo.BuscarPorCodigo(codigoVeiculo);

                Dominio.Entidades.PagamentoMotorista pagamento = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada!");

                    pagamento = repPagamentoMotorista.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de negada!");

                    pagamento = new Dominio.Entidades.PagamentoMotorista();
                    pagamento.Empresa = this.EmpresaUsuario;
                }

                pagamento.Status = status;
                pagamento.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista);
                pagamento.DataPagamento = dataPagamento;
                pagamento.DataRecebimento = dataRecebimento;
                pagamento.Deduzir = deduzir;
                pagamento.ValorFrete = valorFrete;
                pagamento.ValorImpostoRenda = valorImpostoRenda;
                pagamento.ValorINSSSENAT = valorINSSSENAT;
                pagamento.ValorSESTSENAT = valorSESTSENAT;
                pagamento.ValorAdiantamento = valorAdiantamento;
                pagamento.ValorOutros = valorOutros;
                pagamento.Observacao = observacao;
                pagamento.Veiculo = veiculo;
                pagamento.ValorPedagio = valorPedagio;
                pagamento.SalarioMotorista = salarioMotorista;

                if (pagamento.Codigo > 0)
                    repPagamentoMotorista.Atualizar(pagamento);
                else
                {
                    pagamento.Numero = repPagamentoMotorista.BuscarUltimoNumero(this.EmpresaUsuario.Codigo) + 1;
                    repPagamentoMotorista.Inserir(pagamento);
                }


                this.SalvarCtes(pagamento, unitOfWork);
                this.SalvarCliente(pagamento.Motorista, unitOfWork);
                this.SalvarMovimentoDoFinanceiro(pagamento.Codigo, unitOfWork);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o pagamento do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.PagamentoMotorista repPagamento = new Repositorio.PagamentoMotorista(unitOfWork);
                Repositorio.VeiculoCTE repVeiculo = new Repositorio.VeiculoCTE(unitOfWork);
                Repositorio.DadosCliente repDadoCliente = new Repositorio.DadosCliente(unitOfWork);

                Dominio.Entidades.PagamentoMotorista pagamento = repPagamento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculo.BuscarPorCTe(this.EmpresaUsuario.Codigo, pagamento.Ctes.FirstOrDefault().ConhecimentoDeTransporteEletronico.Codigo);
                Dominio.Entidades.Cliente proprietario = (from obj in veiculos where obj.Veiculo != null && obj.Veiculo.Proprietario != null select obj.Veiculo.Proprietario).FirstOrDefault();
                Dominio.Entidades.DadosCliente dadoCliente = proprietario != null ? repDadoCliente.Buscar(this.EmpresaUsuario.Codigo, proprietario.Codigo) : null;

                List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista> listaRecibo = new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista>();

                if (pagamento.Ctes != null && pagamento.Ctes.Count > 0)
                    pagamento.Ctes = pagamento.Ctes.OrderBy(o => o.ConhecimentoDeTransporteEletronico.Codigo).ToList();

                for (var i = 0; i < pagamento.Ctes.Count; i++)
                {
                    decimal saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio + pagamento.SalarioMotorista;
                    if (pagamento.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim)
                        saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio - pagamento.ValorINSSSENAT - pagamento.ValorImpostoRenda - pagamento.ValorSESTSENAT - pagamento.ValorAdiantamento - pagamento.ValorOutros;

                    Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista recibo = new Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista
                    {
                        Logomarca = Servicos.Imagem.GetFromPath(this.EmpresaUsuario.CaminhoLogoDacte, System.Drawing.Imaging.ImageFormat.Bmp),

                        Numero = pagamento.Numero,
                        Status = pagamento.DescricaoStatus,
                        BairroEmpresa = this.EmpresaUsuario.Bairro,
                        CEPEmpresa = this.EmpresaUsuario.CEP,
                        CidadeEmpresa = this.EmpresaUsuario.Localidade.Descricao,
                        CNPJEmpresa = this.EmpresaUsuario.CNPJ,
                        ComplementoEmpresa = this.EmpresaUsuario.Complemento,
                        EnderecoEmpresa = this.EmpresaUsuario.Endereco,
                        FoneEmpresa = this.EmpresaUsuario.Telefone,
                        IEEmpresa = this.EmpresaUsuario.InscricaoEstadual,
                        NomeEmpresa = this.EmpresaUsuario.RazaoSocial,
                        NumeroEmpresa = this.EmpresaUsuario.Numero,
                        UFEmpresa = this.EmpresaUsuario.Localidade.Estado.Sigla,

                        CidadeMotorista = pagamento.Motorista.Localidade != null ? pagamento.Motorista.Localidade.Descricao : string.Empty,
                        CNHMotorista = pagamento.Motorista.NumeroHabilitacao,
                        CPFMotorista = pagamento.Motorista.CPF,
                        NascimentoMotorista = pagamento.Motorista.DataNascimento,
                        //NascimentoMotorista = pagamento.Motorista.DataNascimento.HasValue ? pagamento.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                        PisPasep = pagamento.Motorista.PIS,
                        NomeMotorista = pagamento.Motorista.Nome,
                        UFMotorista = pagamento.Motorista.Localidade != null ? pagamento.Motorista.Localidade.Estado.Sigla : string.Empty,
                        EnderecoMotorista = pagamento.Motorista.Endereco,
                        PisPasepProprietario = dadoCliente != null && !string.IsNullOrWhiteSpace(dadoCliente.PIS) ? dadoCliente.PIS : string.Empty,

                        CidadeInicioPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeInicioPrestacao.Descricao,
                        UFInicioPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeInicioPrestacao.Estado.Sigla,
                        CidadeTerminoPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeTerminoPrestacao.Descricao,
                        UFTerminoPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeTerminoPrestacao.Estado.Sigla,
                        DataEmissaoCTe = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.DataEmissao.Value,
                        NumeroCTe = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.Numero.ToString(),
                        SerieCTe = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),

                        ValorFrete = pagamento.ValorFrete,
                        ValorINSS = pagamento.ValorINSSSENAT,
                        ValorIR = pagamento.ValorImpostoRenda,
                        ValorSESTSENAT = pagamento.ValorSESTSENAT,
                        ValorAdiantamento = pagamento.ValorAdiantamento,
                        ValorOutros = pagamento.ValorOutros,
                        ValorPedagio = pagamento.ValorPedagio,
                        SalarioMotorista = pagamento.SalarioMotorista,
                        SaldoAPagar = saldoAPagar,
                        SaldoAPagarDescricao = Utilidades.Conversor.DecimalToWords(saldoAPagar),

                        DescricaoDocumento = "Conhecimento",
                        AbreviaturaDescricaoDocumento = "CT-e",
                        Observacao = pagamento.Observacao
                    };

                    if (pagamento.Veiculo != null)
                    {
                        recibo.PlacaVeiculo = pagamento.Veiculo.Placa;
                        recibo.UFVeiculo = pagamento.Veiculo.Estado.Sigla;
                        recibo.DescricaoUFVeiculo = pagamento.Veiculo.Estado.Nome;

                        if (pagamento.Veiculo.Tipo == "T" && pagamento.Veiculo.Proprietario != null)
                        {
                            recibo.BairroProprietarioVeiculo = pagamento.Veiculo.Proprietario.Bairro;
                            recibo.CEPProprietarioVeiculo = pagamento.Veiculo.Proprietario.CEP;
                            recibo.CidadeProprietarioVeiculo = pagamento.Veiculo.Proprietario.Localidade.Descricao;
                            recibo.CPFCNPJProprietarioVeiculo = pagamento.Veiculo.Proprietario.CPF_CNPJ_SemFormato;
                            recibo.EnderecoProprietarioVeiculo = pagamento.Veiculo.Proprietario.Endereco;
                            recibo.IERGProprietarioVeiculo = pagamento.Veiculo.Proprietario.IE_RG;
                            recibo.NomeProprietarioVeiculo = pagamento.Veiculo.Proprietario.Nome;
                            recibo.UFProprietarioVeiculo = pagamento.Veiculo.Proprietario.Localidade.Estado.Sigla;
                        }
                    }
                    else
                    {

                        if (proprietario != null)
                        {
                            recibo.BairroProprietarioVeiculo = proprietario.Bairro;
                            recibo.CEPProprietarioVeiculo = proprietario.CEP;
                            recibo.CidadeProprietarioVeiculo = proprietario.Localidade.Descricao;
                            recibo.CPFCNPJProprietarioVeiculo = proprietario.CPF_CNPJ_SemFormato;
                            recibo.EnderecoProprietarioVeiculo = proprietario.Endereco;
                            recibo.IERGProprietarioVeiculo = proprietario.IE_RG;
                            recibo.NomeProprietarioVeiculo = proprietario.Nome;
                            recibo.UFProprietarioVeiculo = proprietario.Localidade.Estado.Sigla;
                        }

                        if (veiculos.Count > 0)
                        {
                            recibo.PlacaVeiculo = veiculos[0].Placa;
                            recibo.UFVeiculo = veiculos[0].Estado.Sigla;
                            recibo.DescricaoUFVeiculo = veiculos[0].Estado.Nome;
                        }
                    }

                    listaRecibo.Add(recibo);
                }

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Contrato", listaRecibo));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/ContratoPagamentoMotorista.rdlc", "PDF", null, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Contrato." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRecibo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.PagamentoMotorista repPagamento = new Repositorio.PagamentoMotorista(unitOfWork);
                Repositorio.VeiculoCTE repVeiculo = new Repositorio.VeiculoCTE(unitOfWork);
                Repositorio.DadosCliente repDadoCliente = new Repositorio.DadosCliente(unitOfWork);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                Dominio.Entidades.PagamentoMotorista pagamento = repPagamento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                Dominio.Entidades.DadosCliente dadoCliente = null;
                Dominio.Entidades.Cliente proprietario = null;

                if (pagamento.Veiculo != null)
                {
                    if (pagamento.Veiculo.Tipo == "T" && pagamento.Veiculo.Proprietario != null)
                    {
                        proprietario = pagamento.Veiculo.Proprietario;
                        dadoCliente = proprietario != null ? repDadoCliente.Buscar(this.EmpresaUsuario.Codigo, proprietario.Codigo) : null;
                    }
                }
                else
                {
                    List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculo.BuscarPorCTe(this.EmpresaUsuario.Codigo, pagamento.Ctes.FirstOrDefault().ConhecimentoDeTransporteEletronico.Codigo);
                    proprietario = (from obj in veiculos where obj.Veiculo != null && obj.Veiculo.Proprietario != null select obj.Veiculo.Proprietario).FirstOrDefault();
                    dadoCliente = proprietario != null ? repDadoCliente.Buscar(this.EmpresaUsuario.Codigo, proprietario.Codigo) : null;
                }

                List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista> listaRecibo = new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista>();

                if (pagamento.Ctes != null && pagamento.Ctes.Count > 0)
                    pagamento.Ctes = pagamento.Ctes.OrderBy(o => o.ConhecimentoDeTransporteEletronico.Codigo).ToList();

                for (var i = 0; i < pagamento.Ctes.Count; i++)
                {
                    decimal saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio + pagamento.SalarioMotorista;
                    if (pagamento.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim)
                        saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio - pagamento.ValorINSSSENAT - pagamento.ValorImpostoRenda - pagamento.ValorSESTSENAT - pagamento.ValorAdiantamento - pagamento.ValorOutros;

                    Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista recibo = new Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista
                    {
                        Numero = pagamento.Numero,
                        Status = pagamento.DescricaoStatus,
                        CEPEmpresa = this.EmpresaUsuario.CEP,
                        CidadeEmpresa = this.EmpresaUsuario.Localidade.Descricao,
                        CNPJEmpresa = this.EmpresaUsuario.CNPJ,
                        ComplementoEmpresa = this.EmpresaUsuario.Complemento,
                        EnderecoEmpresa = this.EmpresaUsuario.Endereco,
                        FoneEmpresa = this.EmpresaUsuario.Telefone,
                        IEEmpresa = this.EmpresaUsuario.InscricaoEstadual,
                        NomeEmpresa = this.EmpresaUsuario.RazaoSocial,
                        NumeroEmpresa = this.EmpresaUsuario.Numero,
                        UFEmpresa = this.EmpresaUsuario.Localidade.Estado.Sigla,

                        CidadeMotorista = pagamento.Motorista.Localidade != null ? pagamento.Motorista.Localidade.Descricao : string.Empty,
                        CNHMotorista = pagamento.Motorista.NumeroHabilitacao,
                        CPFMotorista = pagamento.Motorista.CPF,
                        NascimentoMotorista = pagamento.Motorista.DataNascimento,
                        //NascimentoMotorista = pagamento.Motorista.DataNascimento.HasValue ? pagamento.Motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                        NomeMotorista = pagamento.Motorista.Nome,
                        UFMotorista = pagamento.Motorista.Localidade != null ? pagamento.Motorista.Localidade.Estado.Sigla : string.Empty,
                        EnderecoMotorista = pagamento.Motorista.Endereco,
                        PisPasep = pagamento.Motorista.PIS,
                        PisPasepProprietario = dadoCliente != null && !string.IsNullOrWhiteSpace(dadoCliente.PIS) ? dadoCliente.PIS : string.Empty,

                        CidadeInicioPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeInicioPrestacao.Descricao,
                        UFInicioPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeInicioPrestacao.Estado.Sigla,
                        CidadeTerminoPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeTerminoPrestacao.Descricao,
                        UFTerminoPrestacao = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.LocalidadeTerminoPrestacao.Estado.Sigla,
                        DataEmissaoCTe = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.DataEmissao.Value,
                        NumeroCTe = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.Numero.ToString(),
                        SerieCTe = pagamento.Ctes[i].ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),

                        ValorFrete = pagamento.ValorFrete,
                        ValorINSS = pagamento.ValorINSSSENAT,
                        ValorIR = pagamento.ValorImpostoRenda,
                        ValorSESTSENAT = pagamento.ValorSESTSENAT,
                        ValorAdiantamento = pagamento.ValorAdiantamento,
                        ValorOutros = pagamento.ValorOutros,
                        SalarioMotorista = pagamento.SalarioMotorista,
                        SaldoAPagar = saldoAPagar,
                        SaldoAPagarDescricao = Utilidades.Conversor.DecimalToWords(saldoAPagar),
                        Observacao = pagamento.Observacao
                    };

                    if (proprietario != null)
                    {
                        recibo.CEPProprietarioVeiculo = proprietario.CEP;
                        recibo.CidadeProprietarioVeiculo = proprietario.Localidade.Descricao;
                        recibo.EnderecoProprietarioVeiculo = proprietario.Endereco;
                        recibo.NomeProprietarioVeiculo = proprietario.CPF_CNPJ_Formatado + " " + proprietario.Nome;
                        recibo.UFProprietarioVeiculo = proprietario.Localidade.Estado.Sigla;
                    }

                    listaRecibo.Add(recibo);
                }

                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Recibo", listaRecibo)
                };

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/ReciboPagamentoMotorista.rdlc", "PDF", null, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Recibo." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o recibo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterPagamentosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if ((this.UsuarioAdministrativo != null) || (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomePagamentosMotoristas))
                    return Json<bool>(false, false, "Pagamento motoristas sem configuração para exibição na pagina inicial.");

                Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unitOfWork);

                List<Dominio.Entidades.PagamentoMotorista> listaPagamentoMotorista = new List<Dominio.Entidades.PagamentoMotorista>();

                int diasAvisoVcto = 60;

                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.DiasParaAvisoVencimentos > 0)
                    diasAvisoVcto = this.EmpresaUsuario.Configuracao.DiasParaAvisoVencimentos;

                listaPagamentoMotorista = repPagamentoMotorista.BuscarParcelasPendentes(this.EmpresaUsuario.Codigo, DateTime.Today.AddDays(diasAvisoVcto));

                List<object> listaRetornoPagamentos = new List<object>();

                foreach (Dominio.Entidades.PagamentoMotorista pgto in listaPagamentoMotorista)
                {
                    listaRetornoPagamentos.Add(new
                    {
                        CodigoCriptografado = Servicos.Criptografia.Criptografar(pgto.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                        Codigo = pgto.Codigo,
                        Numero = pgto.Numero,
                        DataPagamento = pgto.DataPagamento.ToString("dd/MM/yyyy"),
                        Motorista = string.Concat(pgto.Motorista.CPF_Formatado, " ", pgto.Motorista.Nome),
                        ValorFrete = pgto.ValorFrete.ToString("n2")
                    });
                }

                return Json(listaRetornoPagamentos, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter pagamentos pendentes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult ObterPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoParcela"], "CT3##MULT1@#$S0FTW4R3"), out codigo);

                Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unitOfWork);

                Dominio.Entidades.DuplicataParcelas parcela = repDuplicataParcelas.BuscarPorCodigo(codigo);

                if (parcela != null)
                {
                    var retorno = new
                    {
                        Codigo = parcela.Codigo,
                        Numero = string.Concat(parcela.Duplicata.Numero, "/", parcela.Parcela),
                        Valor = parcela.Valor.ToString("n2"),
                        DataVencimento = parcela.DataVcto.ToString("dd/MM/yyyy"),
                        Cliente = parcela.Duplicata.Pessoa != null ? string.Concat(parcela.Duplicata.Pessoa.CPF_CNPJ_Formatado, " - ", parcela.Duplicata.Pessoa.Nome) : string.Empty
                    };

                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Parcela não encontrada.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter dados da parcela");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoPagamento"], "CT3##MULT1@#$S0FTW4R3"), out codigo);

                Repositorio.PagamentoMotorista repPagamento = new Repositorio.PagamentoMotorista(unitOfWork);
                Repositorio.PagamentoMotoristaCtes repCtes = new Repositorio.PagamentoMotoristaCtes(unitOfWork);

                Dominio.Entidades.PagamentoMotorista pagamento = repPagamento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (pagamento == null)
                    return Json<bool>(false, false, "Pagamento do motorista não encontrado.");

                List<Dominio.Entidades.PagamentoMotoristaCtes> ctes = repCtes.BuscarPorPagamento(pagamento.Codigo);

                var retorno = new
                {
                    pagamento.Codigo,
                    Numero = pagamento.Numero != null ? pagamento.Numero : 0,
                    CodigoCTe = pagamento.CTE != null ? pagamento.CTE.Codigo : 0,
                    DescricaoCTe = pagamento.CTE != null ? string.Concat(pagamento.CTE.Numero, " - ", pagamento.CTE.Serie.Numero) : string.Empty,
                    CodigoMotorista = pagamento.Motorista.Codigo,
                    DescricaoMotorista = string.Concat(pagamento.Motorista.CPF, " - ", pagamento.Motorista.Nome),
                    DataPagamento = pagamento.DataPagamento.ToString("dd/MM/yyyy"),
                    DataRecebimento = pagamento.DataRecebimento.ToString("dd/MM/yyyy"),
                    ValorFrete = pagamento.ValorFrete.ToString("n2"),
                    ValorImpostoRenda = pagamento.ValorImpostoRenda.ToString("n2"),
                    ValorINSSSENAT = pagamento.ValorINSSSENAT.ToString("n2"),
                    ValorSESTSENAT = pagamento.ValorSESTSENAT.ToString("n2"),
                    ValorAdiantamento = pagamento.ValorAdiantamento.ToString("n2"),
                    ValorOutros = pagamento.ValorOutros.ToString("n2"),
                    ValorPedagio = pagamento.ValorPedagio.ToString("n2"),
                    SalarioMotorista = pagamento.SalarioMotorista.ToString("n2"),
                    pagamento.Deduzir,
                    pagamento.Observacao,
                    pagamento.Status,
                    Ctes = (from obj in ctes
                            select new Dominio.ObjetosDeValor.PagamentoMotoristaCtes()
                            {
                                Codigo = obj.Codigo,
                                CodigoCte = obj.ConhecimentoDeTransporteEletronico.Codigo,
                                NumeroCte = obj.ConhecimentoDeTransporteEletronico.Numero.ToString(),
                                SerieCte = obj.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),
                                ValorCte = obj.ConhecimentoDeTransporteEletronico.ValorAReceber,
                                ChaveCte = obj.ConhecimentoDeTransporteEletronico.Chave,
                                DescricaoCte = obj.ConhecimentoDeTransporteEletronico.Numero.ToString() + " - " + obj.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),
                                TomadorCte = obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.ConhecimentoDeTransporteEletronico.Remetente.Nome : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.ConhecimentoDeTransporteEletronico.Destinatario.Nome : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.ConhecimentoDeTransporteEletronico.Expedidor.Nome : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.ConhecimentoDeTransporteEletronico.Recebedor.Nome : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.ConhecimentoDeTransporteEletronico.OutrosTomador.Nome : string.Empty,
                                Modelo = obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                Excluir = false
                            }).ToList(),
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do pagamento do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarCtes(Dominio.Entidades.PagamentoMotorista pagamento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Ctes"]))
            {
                List<Dominio.ObjetosDeValor.PagamentoMotoristaCtes> listaCtes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PagamentoMotoristaCtes>>(Request.Params["Ctes"]);

                if (listaCtes != null)
                {
                    Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unidadeDeTrabalho);
                    Repositorio.PagamentoMotoristaCtes repPagamentoMotoristaCtes = new Repositorio.PagamentoMotoristaCtes(unidadeDeTrabalho);

                    for (var i = 0; i < listaCtes.Count; i++)
                    {
                        Dominio.Entidades.PagamentoMotoristaCtes pagamentoMotoristaCte = repPagamentoMotoristaCtes.BuscarPorCodigo(listaCtes[i].CodigoCte, pagamento.Codigo);

                        if (!listaCtes[i].Excluir)
                        {
                            Repositorio.ConhecimentoDeTransporteEletronico repCtes = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCtes.BuscarPorCodigo(listaCtes[i].CodigoCte);

                            if (cte != null)
                            {
                                if (pagamentoMotoristaCte == null)
                                    pagamentoMotoristaCte = new Dominio.Entidades.PagamentoMotoristaCtes();

                                pagamentoMotoristaCte.PagamentoMotorista = pagamento;
                                pagamentoMotoristaCte.ConhecimentoDeTransporteEletronico = cte;

                                if (pagamentoMotoristaCte.Codigo > 0)
                                    repPagamentoMotoristaCtes.Atualizar(pagamentoMotoristaCte);
                                else
                                    repPagamentoMotoristaCtes.Inserir(pagamentoMotoristaCte);
                            }
                        }
                        else if (pagamentoMotoristaCte != null && pagamentoMotoristaCte.Codigo > 0)
                        {
                            repPagamentoMotoristaCtes.Deletar(pagamentoMotoristaCte);
                        }
                    }
                }
            }
        }

        private void SalvarCliente(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            double cpfCnpj = 0f;
            double.TryParse(Utilidades.String.OnlyNumbers(motorista.CPF), out cpfCnpj);

            if (cpfCnpj > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                    cliente = new Dominio.Entidades.Cliente();
                    cliente.Atividade = repAtividade.BuscarPorCodigo(7);
                    cliente.Bairro = motorista.Bairro;
                    cliente.CEP = motorista.CEP;
                    cliente.Complemento = motorista.Complemento;
                    cliente.CPF_CNPJ = cpfCnpj;
                    cliente.DataCadastro = DateTime.Now;
                    cliente.DataNascimento = motorista.DataNascimento;
                    cliente.Email = motorista.Email;
                    cliente.Endereco = motorista.Endereco;
                    cliente.EstadoRG = motorista.EstadoRG;
                    cliente.IE_RG = motorista.RG;
                    cliente.Localidade = motorista.Localidade != null ? motorista.Localidade : this.Usuario.Empresa.Localidade;
                    cliente.Nome = motorista.Nome;
                    cliente.OrgaoEmissorRG = motorista.OrgaoEmissorRG;
                    cliente.Sexo = motorista.Sexo;
                    cliente.Telefone1 = motorista.Telefone;
                    cliente.Tipo = motorista.CPF.Length == 14 ? "J" : "F";
                    cliente.Ativo = true;
                    repCliente.Inserir(cliente);
                }
            }
        }

        private void SalvarMovimentoDoFinanceiro(int codigoPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unitOfWork);

            Dominio.Entidades.PagamentoMotorista pagamento = repPagamentoMotorista.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPagamento);

            if (pagamento != null && pagamento.Empresa.Configuracao != null && pagamento.Empresa.Configuracao.PlanoPagamentoMotorista != null)
            {
                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                List<Dominio.Entidades.MovimentoDoFinanceiro> movimentos = repMovimento.BuscarPorPagamentoMotoristaCTe(pagamento.Empresa.Codigo, pagamento.Codigo);

                foreach (Dominio.Entidades.MovimentoDoFinanceiro movimento in movimentos)
                    repMovimento.Deletar(movimento);

                double cpfCnpjMotorista = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(pagamento.Motorista.CPF), out cpfCnpjMotorista);

                Dominio.Entidades.Cliente motorista = repCliente.BuscarPorCPFCNPJ(cpfCnpjMotorista);

                decimal valorPagamento = pagamento.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim ?
                    pagamento.ValorFrete + pagamento.ValorPedagio + pagamento.SalarioMotorista - pagamento.ValorSESTSENAT - pagamento.ValorINSSSENAT - pagamento.ValorImpostoRenda - pagamento.ValorAdiantamento:
                    pagamento.ValorFrete + pagamento.ValorPedagio;

                if (pagamento.ValorAdiantamento > 0)
                {
                    Dominio.Entidades.MovimentoDoFinanceiro movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                    movimento.Data = pagamento.DataPagamento;
                    movimento.DataPagamento = pagamento.DataRecebimento;
                    movimento.PagamentosMotoristasCTe = new List<Dominio.Entidades.PagamentoMotorista>();
                    movimento.PagamentosMotoristasCTe.Add(pagamento);
                    movimento.Empresa = pagamento.Empresa;
                    movimento.Observacao = string.Concat("Referente ao adiantamento do pagamento de motorista nº ", pagamento.Numero, ".");
                    movimento.Valor = pagamento.ValorAdiantamento;
                    movimento.Veiculo = pagamento.CTE?.Veiculos?.FirstOrDefault()?.Veiculo;
                    movimento.Pessoa = motorista;
                    movimento.PlanoDeConta = pagamento.Empresa.Configuracao.PlanoPagamentoMotorista;
                    movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                    movimento.Documento = pagamento.CTE != null ? string.Concat(pagamento.CTE.Numero, " - ", pagamento.CTE.Serie.Numero) : pagamento.Numero.ToString();

                    repMovimento.Inserir(movimento);
                }

                Dominio.Entidades.MovimentoDoFinanceiro movimentoFrete = new Dominio.Entidades.MovimentoDoFinanceiro();

                movimentoFrete.Data = pagamento.DataPagamento;
                movimentoFrete.DataPagamento = pagamento.DataRecebimento;
                movimentoFrete.PagamentosMotoristasCTe = new List<Dominio.Entidades.PagamentoMotorista>();
                movimentoFrete.PagamentosMotoristasCTe.Add(pagamento);
                movimentoFrete.Empresa = pagamento.Empresa;
                movimentoFrete.Observacao = string.Concat("Referente ao adiantamento do pagamento de motorista nº ", pagamento.Numero, ".");
                movimentoFrete.Valor = valorPagamento;
                movimentoFrete.Veiculo = pagamento.CTE?.Veiculos?.FirstOrDefault()?.Veiculo;
                movimentoFrete.Pessoa = motorista;
                movimentoFrete.PlanoDeConta = pagamento.Empresa.Configuracao.PlanoPagamentoMotorista;
                movimentoFrete.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                movimentoFrete.Documento = pagamento.CTE != null ? string.Concat(pagamento.CTE.Numero, " - ", pagamento.CTE.Serie.Numero) : pagamento.Numero.ToString();

                repMovimento.Inserir(movimentoFrete);
            }
        }

        #endregion
    }
}
