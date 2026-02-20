using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class PagamentoMotoristaMDFeController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("pagamentomotoristamdfe.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe, codigoMotorista, inicioRegistros;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string status = Request.Params["Status"];

                Repositorio.PagamentoMotoristaMDFe repPagamento = new Repositorio.PagamentoMotoristaMDFe(unitOfWork);

                List<Dominio.Entidades.PagamentoMotoristaMDFe> pagamentos = repPagamento.Consultar(this.EmpresaUsuario.Codigo, codigoCTe, codigoMotorista, dataInicial, dataFinal, status, inicioRegistros, 50);
                int countPagamentos = repPagamento.ContarConsulta(this.EmpresaUsuario.Codigo, codigoCTe, codigoMotorista, dataInicial, dataFinal, status);

                var retorno = (from obj in pagamentos
                               select new
                               {
                                   obj.Codigo,
                                   DataPagamento = obj.DataPagamento.ToString("dd/MM/yyyy"),
                                   MDFe = string.Concat(obj.MDFe.Numero, " - ", obj.MDFe.Serie.Numero),
                                   Motorista = string.Concat(obj.Motorista.CPF, " - ", obj.Motorista.Nome),
                                   ValorFrete = obj.ValorFrete.ToString("n2")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Dt. Pgto.|15", "MDF-e|15", "Motorista|40", "Valor Frete|15" }, countPagamentos);
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

                Repositorio.PagamentoMotoristaMDFe repPagamento = new Repositorio.PagamentoMotoristaMDFe(unitOfWork);
                Dominio.Entidades.PagamentoMotoristaMDFe pagamento = repPagamento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (pagamento == null)
                    return Json<bool>(false, false, "Pagamento do motorista não encontrado.");

                var retorno = new
                {
                    pagamento.Codigo,
                    pagamento.Status,
                    CodigoMDFe = pagamento.MDFe.Codigo,
                    DescricaoMDFe = string.Concat(pagamento.MDFe.Numero, " - ", pagamento.MDFe.Serie.Numero),
                    CodigoMotorista = pagamento.Motorista.Codigo,
                    DescricaoMotorista = string.Concat(pagamento.Motorista.CPF, " - ", pagamento.Motorista.Nome),
                    DataPagamento = pagamento.DataPagamento.ToString("dd/MM/yyyy"),
                    DataRecebimento = pagamento.DataRecebimento.ToString("dd/MM/yyyy"),
                    ValorFrete = pagamento.ValorFrete.ToString("n2"),
                    ValorImpostoRenda = pagamento.ValorImpostoRenda.ToString("n2"),
                    ValorINSSSENAT = pagamento.ValorINSSSENAT.ToString("n2"),
                    ValorSESTSENAT = pagamento.ValorSESTSENAT.ToString("n2"),
                    ValorAdiantamento = pagamento.ValorAdiantamento.ToString("n2"),
                    SalarioMotorista = pagamento.SalarioMotorista.ToString("n2"),
                    ValorPedagio = pagamento.ValorPedagio.ToString("n2"),
                    pagamento.Deduzir,
                    pagamento.Observacao
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
                int codigo, codigoMDFe, codigoMotorista;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                DateTime dataPagamento, dataRecebimento;
                DateTime.TryParseExact(Request.Params["DataPagamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagamento);
                DateTime.TryParseExact(Request.Params["DataRecebimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataRecebimento);

                decimal valorFrete, valorINSSSENAT, valorSESTSENAT, valorImpostoRenda, valorAdiantamento, salarioMotorista;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["INSSSENAT"], out valorINSSSENAT);
                decimal.TryParse(Request.Params["SESTSENAT"], out valorSESTSENAT);
                decimal.TryParse(Request.Params["IR"], out valorImpostoRenda);
                decimal.TryParse(Request.Params["INSSSENAT"], out valorINSSSENAT);
                decimal.TryParse(Request.Params["Adiantamento"], out valorAdiantamento);
                decimal.TryParse(Request.Params["SalarioMotorista"], out salarioMotorista);                
                decimal.TryParse(Request.Params["ValorPedagio"], out decimal valorPedagio);

                Dominio.Enumeradores.OpcaoSimNao deduzir;
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["Deduzir"], out deduzir);

                string observacao = Request.Params["Observacao"];
                string status = Request.Params["Status"];

                Repositorio.PagamentoMotoristaMDFe repPagamentoMotorista = new Repositorio.PagamentoMotoristaMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.PagamentoMotoristaMDFe pagamento = null;

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

                    pagamento = new Dominio.Entidades.PagamentoMotoristaMDFe();
                    pagamento.Empresa = this.EmpresaUsuario;
                }

                pagamento.Status = status;
                pagamento.MDFe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);
                pagamento.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista);
                pagamento.DataPagamento = dataPagamento;
                pagamento.DataRecebimento = dataRecebimento;
                pagamento.Deduzir = deduzir;
                pagamento.ValorFrete = valorFrete;
                pagamento.ValorImpostoRenda = valorImpostoRenda;
                pagamento.ValorINSSSENAT = valorINSSSENAT;
                pagamento.ValorSESTSENAT = valorSESTSENAT;
                pagamento.ValorAdiantamento = valorAdiantamento;
                pagamento.SalarioMotorista = salarioMotorista;
                pagamento.Observacao = observacao;
                pagamento.ValorPedagio = valorPedagio;

                if (pagamento.Codigo > 0)
                    repPagamentoMotorista.Atualizar(pagamento);
                else
                    repPagamentoMotorista.Inserir(pagamento);

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
                Repositorio.PagamentoMotoristaMDFe repPagamento = new Repositorio.PagamentoMotoristaMDFe(unitOfWork);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.PagamentoMotoristaMDFe pagamento = repPagamento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);
                Dominio.Entidades.VeiculoMDFe veiculo = repVeiculoMDFe.BuscarPorMDFe(pagamento.MDFe.Codigo);

                decimal saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio + pagamento.SalarioMotorista;
                if (pagamento.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim)
                    saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio - pagamento.ValorINSSSENAT - pagamento.ValorImpostoRenda - pagamento.ValorSESTSENAT - pagamento.ValorAdiantamento;

                Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista recibo = new Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista
                {
                    Logomarca = Servicos.Imagem.GetFromPath(this.EmpresaUsuario.CaminhoLogoDacte, System.Drawing.Imaging.ImageFormat.Bmp),

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

                    CidadeInicioPrestacao = pagamento.MDFe.EstadoCarregamento.Nome.Trim(),
                    UFInicioPrestacao = pagamento.MDFe.EstadoCarregamento.Sigla,
                    CidadeTerminoPrestacao = pagamento.MDFe.EstadoDescarregamento.Nome.Trim(),
                    UFTerminoPrestacao = pagamento.MDFe.EstadoDescarregamento.Sigla,
                    DataEmissaoCTe = pagamento.MDFe.DataEmissao.Value,
                    NumeroCTe = pagamento.MDFe.Numero.ToString(),
                    SerieCTe = pagamento.MDFe.Serie.Numero.ToString(),

                    ValorFrete = pagamento.ValorFrete,
                    ValorINSS = pagamento.ValorINSSSENAT,
                    ValorIR = pagamento.ValorImpostoRenda,
                    ValorSESTSENAT = pagamento.ValorSESTSENAT,
                    ValorAdiantamento = pagamento.ValorAdiantamento,
                    SalarioMotorista = pagamento.SalarioMotorista,
                    ValorPedagio = pagamento.ValorPedagio,
                    SaldoAPagar = saldoAPagar,
                    SaldoAPagarDescricao = Utilidades.Conversor.DecimalToWords(saldoAPagar),

                    CPFCNPJProprietarioVeiculo = veiculo.CPFCNPJProprietario,
                    IERGProprietarioVeiculo = veiculo.IEProprietario,
                    NomeProprietarioVeiculo = veiculo.NomeProprietario,
                    UFProprietarioVeiculo = veiculo.UFProprietario != null ? veiculo.UFProprietario.Sigla : string.Empty,

                    PlacaVeiculo = veiculo.Placa,
                    UFVeiculo = veiculo.UF.Sigla,
                    DescricaoUFVeiculo = veiculo.UF.Nome,

                    DescricaoDocumento = "Manifesto",
                    AbreviaturaDescricaoDocumento = "MDF-e",

                    Observacao = pagamento.Observacao
                };

                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Contrato", new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista>() { recibo })
                };


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
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.PagamentoMotorista pagamento = repPagamento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculo.BuscarPorCTe(this.EmpresaUsuario.Codigo, pagamento.CTE.Codigo);
                Dominio.Entidades.Cliente proprietario = (from obj in veiculos where obj.Veiculo != null && obj.Veiculo.Proprietario != null select obj.Veiculo.Proprietario).FirstOrDefault();

                decimal saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio + pagamento.SalarioMotorista;
                if (pagamento.Deduzir == Dominio.Enumeradores.OpcaoSimNao.Sim)
                    saldoAPagar = pagamento.ValorFrete + pagamento.ValorPedagio - pagamento.ValorINSSSENAT - pagamento.ValorImpostoRenda - pagamento.ValorSESTSENAT - pagamento.ValorAdiantamento;

                Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista recibo = new Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista
                {
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

                    CidadeInicioPrestacao = pagamento.CTE.LocalidadeInicioPrestacao.Descricao,
                    UFInicioPrestacao = pagamento.CTE.LocalidadeInicioPrestacao.Estado.Sigla,
                    CidadeTerminoPrestacao = pagamento.CTE.LocalidadeTerminoPrestacao.Descricao,
                    UFTerminoPrestacao = pagamento.CTE.LocalidadeTerminoPrestacao.Estado.Sigla,
                    DataEmissaoCTe = pagamento.CTE.DataEmissao.Value,
                    NumeroCTe = pagamento.CTE.Numero.ToString(),
                    SerieCTe = pagamento.CTE.Serie.Numero.ToString(),

                    ValorFrete = pagamento.ValorFrete,
                    ValorINSS = pagamento.ValorINSSSENAT,
                    ValorIR = pagamento.ValorImpostoRenda,
                    ValorSESTSENAT = pagamento.ValorSESTSENAT,
                    ValorAdiantamento = pagamento.ValorAdiantamento,
                    SalarioMotorista = pagamento.SalarioMotorista,
                    ValorPedagio = pagamento.ValorPedagio,
                    SaldoAPagar = saldoAPagar,
                    SaldoAPagarDescricao = Utilidades.Conversor.DecimalToWords(saldoAPagar)
                };

                if (proprietario != null)
                {
                    recibo.CEPProprietarioVeiculo = proprietario.CEP;
                    recibo.CidadeProprietarioVeiculo = proprietario.Localidade.Descricao;
                    recibo.EnderecoProprietarioVeiculo = proprietario.Endereco;
                    recibo.NomeProprietarioVeiculo = proprietario.Nome;
                    recibo.UFProprietarioVeiculo = proprietario.Localidade.Estado.Sigla;
                }

                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Recibo", new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoMotorista>() { recibo })
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
        public ActionResult ObterImpostosDaEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoMotorista = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                decimal valorFrete = 0;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);

                DateTime data;
                DateTime.TryParseExact(Request.Params["DataPagamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                var dataInicial = new DateTime(data.Year, data.Month, 1);
                var dataFinal = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));

                Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);

                Dominio.Entidades.ImpostoContratoFrete imposto = repImpostoContratoFrete.BuscarPorEmpresa(this.EmpresaUsuario.Codigo);

                if (imposto == null)
                    return Json<bool>(true, true);

                Repositorio.INSSImpostoContratoFrete repINSS = new Repositorio.INSSImpostoContratoFrete(unitOfWork);
                Repositorio.IRImpostoContratoFrete repIR = new Repositorio.IRImpostoContratoFrete(unitOfWork);
                Repositorio.PagamentoMotoristaMDFe repPagamentoMotoristaMDFe = new Repositorio.PagamentoMotoristaMDFe(unitOfWork);

                decimal valorTotalFrete = repPagamentoMotoristaMDFe.BuscarTotalFreteMotorista(codigo, this.Usuario.Empresa.Codigo, codigoMotorista, dataInicial, dataFinal);
                decimal valorTotalINSS = repPagamentoMotoristaMDFe.BuscarTotalINSSMotorista(codigo, this.Usuario.Empresa.Codigo, codigoMotorista, dataInicial, dataFinal);
                decimal valorTotalIR = repPagamentoMotoristaMDFe.BuscarTotalIRMotorista(codigo, this.Usuario.Empresa.Codigo, codigoMotorista, dataInicial, dataFinal);

                Dominio.Entidades.INSSImpostoContratoFrete faixaINSS = repINSS.BuscarPorImpostoEFaixa(imposto.Codigo, valorFrete);

                var baseCalculoIR = valorTotalFrete + valorFrete;
                if (imposto.PercentualBCIR > 0)
                    baseCalculoIR = (baseCalculoIR * (imposto.PercentualBCIR / 100)) - valorTotalINSS;
                Dominio.Entidades.IRImpostoContratoFrete faixaIR = repIR.BuscarPorImpostoEFaixa(imposto.Codigo, baseCalculoIR);

                var retorno = new
                {
                    imposto.AliquotaSENAT,
                    imposto.AliquotaSEST,
                    imposto.PercentualBCINSS,
                    imposto.PercentualBCIR,
                    imposto.ValorTetoRetencaoINSS,
                    ValorTotalINSS = valorTotalINSS,
                    PercentualINSSAplicar = faixaINSS != null ? faixaINSS.PercentualAplicar : 0,
                    ValorTotalIR = valorTotalIR,
                    PercentualIRAplicar = faixaIR != null ? faixaIR.PercentualAplicar : 0,
                    ValorIRDeduzir = faixaIR != null ? faixaIR.ValorDeduzir : 0,
                    ValorTotalFrete = valorTotalFrete
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os impostos da empresa. Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

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
            Repositorio.PagamentoMotoristaMDFe repPagamentoMotorista = new Repositorio.PagamentoMotoristaMDFe(unitOfWork);

            Dominio.Entidades.PagamentoMotoristaMDFe pagamento = repPagamentoMotorista.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPagamento);

            if (pagamento != null && pagamento.Empresa.Configuracao != null && pagamento.Empresa.Configuracao.PlanoPagamentoMotorista != null)
            {
                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unitOfWork);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                List<Dominio.Entidades.MovimentoDoFinanceiro> movimentos = repMovimento.BuscarPorPagamentoMotoristaMDFe(pagamento.Empresa.Codigo, pagamento.Codigo);

                foreach (Dominio.Entidades.MovimentoDoFinanceiro movimento in movimentos)
                    repMovimento.Deletar(movimento);

                Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(pagamento.MDFe.Codigo);
                Dominio.Entidades.Veiculo veiculo = veiculoMDFe != null ? repVeiculo.BuscarPorPlaca(pagamento.Empresa.Codigo, veiculoMDFe.Placa) : null;

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
                    movimento.PagamentosMotoristasMDFe = new List<Dominio.Entidades.PagamentoMotoristaMDFe>();
                    movimento.PagamentosMotoristasMDFe.Add(pagamento);
                    movimento.Empresa = pagamento.Empresa;
                    movimento.Observacao = string.Concat("Referente ao adiantamento do pagamento de motorista MDF-e nº ", pagamento.MDFe.Numero, " - ", pagamento.MDFe.Serie.Numero, ".");
                    movimento.Valor = pagamento.ValorAdiantamento;
                    movimento.Veiculo = veiculo;
                    movimento.Pessoa = motorista;
                    movimento.PlanoDeConta = pagamento.Empresa.Configuracao.PlanoPagamentoMotorista;
                    movimento.Documento = string.Concat(pagamento.MDFe.Numero, " - ", pagamento.MDFe.Serie.Numero);
                    movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;

                    repMovimento.Inserir(movimento);
                }

                Dominio.Entidades.MovimentoDoFinanceiro movimentoFrete = new Dominio.Entidades.MovimentoDoFinanceiro();

                movimentoFrete.Data = pagamento.DataPagamento;
                movimentoFrete.DataPagamento = pagamento.DataRecebimento;
                movimentoFrete.PagamentosMotoristasMDFe = new List<Dominio.Entidades.PagamentoMotoristaMDFe>();
                movimentoFrete.PagamentosMotoristasMDFe.Add(pagamento);
                movimentoFrete.Empresa = pagamento.Empresa;
                movimentoFrete.Observacao = string.Concat("Referente ao adiantamento do pagamento de motorista do MDF-e nº ", pagamento.MDFe.Numero, " - ", pagamento.MDFe.Serie.Numero, ".");
                movimentoFrete.Valor = valorPagamento;
                movimentoFrete.Veiculo = veiculo;
                movimentoFrete.Pessoa = motorista;
                movimentoFrete.PlanoDeConta = pagamento.Empresa.Configuracao.PlanoPagamentoMotorista;
                movimentoFrete.Documento = string.Concat(pagamento.MDFe.Numero, " - ", pagamento.MDFe.Serie.Numero);
                movimentoFrete.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;

                repMovimento.Inserir(movimentoFrete);
            }
        }

        #endregion
    }
}
