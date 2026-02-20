using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DuplicatasController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("duplicatas.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int numero, cte, inicioRegistros;
                int.TryParse(Request.Params["Numero"], out numero);
                int.TryParse(Request.Params["CTe"], out cte);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime dataLancamento, dataDocumento;
                DateTime.TryParseExact(Request.Params["DataLancamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLancamento);
                DateTime.TryParseExact(Request.Params["DataDocumento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataDocumento);

                string nomePessoa = Request.Params["NomePessoa"];
                string documento = Request.Params["Documento"];

                Dominio.Enumeradores.TipoDuplicata tipoAux;
                Dominio.Enumeradores.TipoDuplicata? tipo = null;
                if (Enum.TryParse<Dominio.Enumeradores.TipoDuplicata>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                Repositorio.Duplicata repDocumento = new Repositorio.Duplicata(unitOfWork);

                List<Dominio.Entidades.Duplicata> duplicatas = repDocumento.Consultar(this.EmpresaUsuario.Codigo, dataLancamento, dataDocumento, numero, nomePessoa, tipo, documento, cte, inicioRegistros, 50);

                int countDocumentos = repDocumento.ContarConsulta(this.EmpresaUsuario.Codigo, dataLancamento, dataDocumento, numero, nomePessoa, tipo, documento, cte);

                var retorno = (from obj in duplicatas
                               select new
                               {
                                   obj.Codigo,
                                   Numero = obj.Numero.ToString(),
                                   DataLancamento = obj.DataLancamento.ToString("dd/MM/yyyy"),
                                   Pessoa = obj.Pessoa.Nome,
                                   ValorTotal = obj.Valor.ToString("n2")
                               }).ToList();

                unitOfWork.Dispose();

                return Json(retorno, true, null, new string[] { "Codigo", "Numero|10", "Dt. Lancamento|15", "Pessoa|40", "Valor|15" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as duplicatas.");
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

                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unitOfWork);

                Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (duplicata == null)
                    return Json<bool>(false, false, "Duplicata não encontrado.");

                Repositorio.DuplicataParcelas repParcelas = new Repositorio.DuplicataParcelas(unitOfWork);
                Repositorio.DuplicataCtes repCtes = new Repositorio.DuplicataCtes(unitOfWork);

                List<Dominio.Entidades.DuplicataParcelas> parcelas = repParcelas.BuscarPorDuplicata(duplicata.Codigo);
                List<Dominio.Entidades.DuplicataCtes> ctes = repCtes.BuscarPorDuplicata(duplicata.Codigo);

                var retorno = new
                {
                    duplicata.Codigo,
                    Funcionario = duplicata.Funcionario != null ? duplicata.Funcionario.Nome : string.Empty,
                    duplicata.Numero,
                    duplicata.Tipo,
                    duplicata.Status,
                    DataLancamento = duplicata.DataLancamento.ToString("dd/MM/yyyy"),
                    duplicata.Documento,
                    DataDocumento = duplicata.DataDocumento.ToString("dd/MM/yyyy"),
                    Pessoa = duplicata.Pessoa.CPF_CNPJ_Formatado + " - " + duplicata.Pessoa.Nome,
                    CPFCNPJPessoa = duplicata.Pessoa.CPF_CNPJ,
                    Valor = duplicata.Valor.ToString("n2"),
                    Acrescimo = duplicata.Acrescimo.ToString("n2"),
                    Desconto = duplicata.Desconto.ToString("n2"),
                    Veiculo1 = duplicata.Veiculo1 != null ? duplicata.Veiculo1.Placa : string.Empty,
                    CodigoVeiculo1 = duplicata.Veiculo1 != null ? duplicata.Veiculo1.Codigo.ToString() : string.Empty,
                    Veiculo2 = duplicata.Veiculo2 != null ? duplicata.Veiculo2.Placa : string.Empty,
                    CodigoVeiculo2 = duplicata.Veiculo2 != null ? duplicata.Veiculo2.Codigo.ToString() : string.Empty,
                    Veiculo3 = duplicata.Veiculo3 != null ? duplicata.Veiculo3.Placa : string.Empty,
                    CodigoVeiculo3 = duplicata.Veiculo3 != null ? duplicata.Veiculo3.Codigo.ToString() : string.Empty,
                    Motorista = duplicata.Motorista != null ? duplicata.Motorista.CPF + " - " + duplicata.Motorista.Nome : string.Empty,
                    CodigoMotorista = duplicata.Motorista != null ? duplicata.Motorista.Codigo.ToString() : string.Empty,
                    PlanoDeConta = duplicata.PlanoDeConta != null ? duplicata.PlanoDeConta.Codigo + " - " + duplicata.PlanoDeConta.Conta + " - " + duplicata.PlanoDeConta.Descricao : string.Empty,
                    CodigoPlanoConta = duplicata.PlanoDeConta != null ? duplicata.PlanoDeConta.Codigo.ToString() : string.Empty,
                    duplicata.Observacao,

                    DadosBancarios = duplicata.DadosBancarios ?? string.Empty,
                    Embarcador = duplicata.Embarcador != null ? new { duplicata.Embarcador.Codigo, Descricao = duplicata.Embarcador.NomeCNPJ } : null,
                    TipoVeiculo = duplicata.TipoVeiculo != null ? new { duplicata.TipoVeiculo.Codigo, duplicata.TipoVeiculo.Descricao } : null,
                    AdicionaisCidadeOrigem = duplicata.AdicionaisCidadeOrigem?.Codigo ?? 0,
                    AdicionaisUFOrigem = duplicata.AdicionaisCidadeOrigem?.Estado.Sigla ?? string.Empty,
                    AdicionaisCidadeDestino = duplicata.AdicionaisCidadeDestino?.Codigo ?? 0,
                    AdicionaisUFDestino = duplicata.AdicionaisCidadeDestino?.Estado.Sigla ?? string.Empty,
                    AdicionaisVolumes = duplicata.AdicionaisVolumes,
                    AdicionaisPeso = duplicata.AdicionaisPeso.ToString("n4"),

                    Parcelas = (from obj in parcelas
                                select new Dominio.ObjetosDeValor.DuplicataParcelas()
                                {
                                    Parcela = obj.Parcela,
                                    Valor = Math.Round(obj.Valor, 2, MidpointRounding.ToEven),
                                    DataVcto = obj.DataVcto.ToString("dd/MM/yyyy"),
                                    ValorPgto = obj.ValorPgto,
                                    DataPgto = obj.DataPgto != null ? obj.DataPgto.ToString() : string.Empty,
                                    Status = obj.Status,
                                    ObservacaoBaixa = obj.ObservacaoBaixa != null ? obj.ObservacaoBaixa : string.Empty
                                }).ToList(),
                    Ctes = (from obj in ctes
                            select new Dominio.ObjetosDeValor.DuplicataCtes()
                            {
                                Codigo = obj.Codigo,
                                CodigoCte = obj.ConhecimentoDeTransporteEletronico.Codigo,
                                NumeroCte = obj.ConhecimentoDeTransporteEletronico.Numero.ToString(),
                                SerieCte = obj.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),
                                ValorCte = obj.ConhecimentoDeTransporteEletronico.ValorAReceber,
                                ChaveCte = obj.ConhecimentoDeTransporteEletronico.Chave,
                                DescricaoCte = obj.ConhecimentoDeTransporteEletronico.Numero.ToString() + " - " + obj.ConhecimentoDeTransporteEletronico.Serie.Numero.ToString(),
                                TomadorCte = obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? Utilidades.String.RemoveAllSpecialCharacters(obj.ConhecimentoDeTransporteEletronico.Remetente.Nome) : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? Utilidades.String.RemoveAllSpecialCharacters(obj.ConhecimentoDeTransporteEletronico.Destinatario.Nome) : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? Utilidades.String.RemoveAllSpecialCharacters(obj.ConhecimentoDeTransporteEletronico.Expedidor.Nome) : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? Utilidades.String.RemoveAllSpecialCharacters(obj.ConhecimentoDeTransporteEletronico.Recebedor.Nome) : obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? Utilidades.String.RemoveAllSpecialCharacters(obj.ConhecimentoDeTransporteEletronico.OutrosTomador.Nome) : string.Empty,
                                Modelo = obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                Excluir = false
                            }).ToList(),
                    DocumentoEntrada = duplicata.DocumentoEntrada != null ? duplicata.DocumentoEntrada.Codigo : 0
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes das duplicatas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCtesSemDuplicata()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

                string cnpjTomador = string.Empty;
                if (this.EmpresaUsuario.Configuracao.PermiteSelecionarCTeOutroTomador)
                    cnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);
                else
                    cnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Pessoa"]);
                string cnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string numeroDocumento = Request.Params["NumeroDocumento"];

                bool filtrarTodosCTes = Request.Params["FiltratTodosCTes"] == "true";

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Cliente clienteTomador = !string.IsNullOrWhiteSpace(cnpjTomador) ? repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjTomador)) : null;
                Dominio.Entidades.Cliente clienteRemetente = !string.IsNullOrWhiteSpace(cnpjRemetente) ? repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjTomador)) : null;
                Dominio.Entidades.Cliente clienteDestinatario = !string.IsNullOrWhiteSpace(cnpjDestinatario) ? repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjTomador)) : null;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarCtesSemDuplicata(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, clienteTomador != null ? clienteTomador.CPF_CNPJ_SemFormato : string.Empty, clienteRemetente != null ? clienteRemetente.CPF_CNPJ_SemFormato : string.Empty, clienteDestinatario != null ? clienteDestinatario.CPF_CNPJ_SemFormato : string.Empty, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Select(o => o.Codigo).ToArray(), numeroDocumento, filtrarTodosCTes, inicioRegistros, 50);
                int countCTes = repCTe.ContarCtesSemDuplicata(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, cnpjTomador, cnpjRemetente, cnpjDestinatario, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Select(o => o.Codigo).ToArray(), numeroDocumento, filtrarTodosCTes);

                var retorno = (from obj in listaCTes
                               select new
                               {
                                   obj.DescricaoStatus,
                                   obj.Codigo,
                                   obj.Chave,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   Modelo = obj.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                   DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   obj.DescricaoTipoServico,
                                   obj.DescricaoTipoCTE,
                                   Tomador = obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.Remetente.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.Destinatario.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.Expedidor.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.Recebedor.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.OutrosTomador.Nome : string.Empty,
                                   Valor = obj.ValorAReceber.ToString("n2")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Status", "Codigo", "Chave", "Núm.|10", "Série|7", "Doc|7", "Emissão|12", "Tipo|10", "Finalid.|10", "Tomador|20", "Valor a Receber|14" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SelecionarTodosCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

                string cnpjTomador = string.Empty;
                if (this.EmpresaUsuario.Configuracao.PermiteSelecionarCTeOutroTomador)
                    cnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);
                else
                    cnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Pessoa"]);
                string cnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string numeroDocumento = Request.Params["NumeroDocumento"];

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarCtesSemDuplicata(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial,
                    numeroFinal, cnpjTomador, cnpjRemetente, cnpjDestinatario, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Select(o => o.Codigo).ToArray(), numeroDocumento, false, 0, 0);

                var retorno = (from obj in listaCTes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   Modelo = obj.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                   Tomador = obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? obj.Remetente.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.Destinatario.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.Expedidor.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.Recebedor.Nome : obj.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.OutrosTomador.Nome : string.Empty,
                                   Valor = obj.ValorAReceber.ToString("n2")
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, numero, codigoVeiculo1, codigoVeiculo2, codigoVeiculo3, codigoMotorista, codigoPlano = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Numero"], out numero);
                int.TryParse(Request.Params["Veiculo1"], out codigoVeiculo1);
                int.TryParse(Request.Params["Veiculo2"], out codigoVeiculo2);
                int.TryParse(Request.Params["Veiculo3"], out codigoVeiculo3);
                int.TryParse(Request.Params["Motorista"], out codigoMotorista);
                int.TryParse(Request.Params["PlanoDeConta"], out codigoPlano);
                int.TryParse(Request.Params["TipoVeiculo"], out int tipoVeiculo);
                int.TryParse(Request.Params["AdicionaisCidadeOrigem"], out int adicionaisCidadeOrigem);
                int.TryParse(Request.Params["AdicionaisCidadeDestino"], out int adicionaisCidadeDestino);

                DateTime dataLancamento, dataDocumento;
                DateTime.TryParseExact(Request.Params["DataLancamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLancamento);
                DateTime.TryParseExact(Request.Params["DataDocumento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataDocumento);

                decimal valor, desconto, acrescimo;
                decimal.TryParse(Request.Params["Valor"], out valor);
                decimal.TryParse(Request.Params["Desconto"], out desconto);
                decimal.TryParse(Request.Params["Acrescimo"], out acrescimo);
                decimal.TryParse(Request.Params["AdicionaisPeso"], out decimal adicionaisPeso);
                decimal.TryParse(Request.Params["AdicionaisVolumes"], out decimal adicionaisVolumes);

                Dominio.Enumeradores.TipoDuplicata tipo;

                Enum.TryParse<Dominio.Enumeradores.TipoDuplicata>(Request.Params["Tipo"], out tipo);

                string status = Request.Params["Status"];
                string documento = Request.Params["Documento"];
                string observacao = Request.Params["Observacao"];
                string dadosBancarios = Request.Params["DadosBancarios"];

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Pessoa"]), out double cnpjPessoa);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Embarcador"]), out double embarcador);

                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(documento) && this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.NaoPermitirDuplciataMesmoDocumento)
                {
                    Dominio.Entidades.Duplicata duplicataPorDocumento = repDuplicata.BuscaPorDocumento(this.EmpresaUsuario.Codigo, documento, tipo);
                    if (duplicataPorDocumento != null && duplicataPorDocumento.Codigo != codigo)
                        return Json<bool>(false, false, "Já existe a duplicata " + duplicataPorDocumento.Numero + " lançada para o documento " + documento + ".");
                }

                Dominio.Entidades.Duplicata duplicata = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de Duplicata negada!");

                    duplicata = repDuplicata.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Duplicata negada!");

                    duplicata = new Dominio.Entidades.Duplicata();
                }

                duplicata.Empresa = this.EmpresaUsuario;
                duplicata.Tipo = tipo;
                duplicata.Status = status;
                duplicata.Numero = numero;
                duplicata.DataLancamento = dataLancamento;
                duplicata.Documento = documento;
                duplicata.DataDocumento = dataDocumento;
                duplicata.Observacao = observacao;
                duplicata.Valor = valor;
                duplicata.Desconto = desconto;
                duplicata.Acrescimo = acrescimo;

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.PlanoDeConta repPlano = new Repositorio.PlanoDeConta(unidadeDeTrabalho);
                Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);

                duplicata.Funcionario = repUsuario.BuscarPorCodigo(this.Usuario.Codigo);
                duplicata.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                duplicata.Pessoa = repPessoa.BuscarPorCPFCNPJ(cnpjPessoa);
                duplicata.Veiculo1 = repVeiculo.BuscarPorCodigo(codigoVeiculo1);
                duplicata.Veiculo2 = repVeiculo.BuscarPorCodigo(codigoVeiculo2);
                duplicata.Veiculo3 = repVeiculo.BuscarPorCodigo(codigoVeiculo3);
                duplicata.PlanoDeConta = repPlano.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlano);

                duplicata.DadosBancarios = dadosBancarios;

                duplicata.Embarcador = embarcador > 0 ? repPessoa.BuscarPorCPFCNPJ(embarcador) : null;
                duplicata.TipoVeiculo = repTipoVeiculo.BuscarPorCodigo(tipoVeiculo, this.EmpresaUsuario.Codigo);
                duplicata.AdicionaisCidadeOrigem = adicionaisCidadeOrigem > 0 ? repLocalidade.BuscarPorCodigo(adicionaisCidadeOrigem) : null;
                duplicata.AdicionaisCidadeDestino = adicionaisCidadeDestino > 0 ? repLocalidade.BuscarPorCodigo(adicionaisCidadeDestino) : null;
                duplicata.AdicionaisVolumes = (int)adicionaisVolumes;

                duplicata.AdicionaisPeso = adicionaisPeso;

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);

                if (codigo > 0)
                {
                    repDuplicata.Atualizar(duplicata);
                }
                else
                {
                    duplicata.Numero = repDuplicata.BuscarUltimoNumero(this.EmpresaUsuario.Codigo) + 1;
                    duplicata.Codigo = repDuplicata.BuscarUltimoCodigo(this.EmpresaUsuario.Codigo) + 1;

                    repDuplicata.Inserir(duplicata);
                }

                this.SalvarParcelas(duplicata, unidadeDeTrabalho);

                this.SalvarCtes(duplicata, unidadeDeTrabalho);

                Servicos.Duplicatas svcDuplicata = new Servicos.Duplicatas(unidadeDeTrabalho);

                if (duplicata.Status == "I")
                    svcDuplicata.DeletarMovimentoDoFinanceiro(duplicata, unidadeDeTrabalho);
                else
                    svcDuplicata.GerarMovimentoDoFinanceiro(duplicata.Empresa.Codigo, duplicata, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();
                unidadeDeTrabalho.Dispose();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                unidadeDeTrabalho.Dispose();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar Duplicata.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult Visualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unitOfWork);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                string tipoArquivo = Request.Params["TipoArquivo"];

                Dominio.Entidades.Duplicata duplicataVisualizar = repDuplicata.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicata dsDuplicataVisualizar = new Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicata()
                {
                    CodigoDuplicata = duplicataVisualizar.Codigo,
                    Numero = duplicataVisualizar.Numero,
                    Tipo = duplicataVisualizar.Tipo,
                    DataLancamento = duplicataVisualizar.DataLancamento,
                    Documento = duplicataVisualizar.Documento,
                    DataDocumento = duplicataVisualizar.DataDocumento,
                    CpfCnpjPessoa = duplicataVisualizar.Pessoa.CPF_CNPJ,
                    IePessoa = duplicataVisualizar.Pessoa.IE_RG,
                    NomePessoa = duplicataVisualizar.Pessoa.Nome,
                    EnderecoPessoa = duplicataVisualizar.Pessoa.Endereco + " - " + duplicataVisualizar.Pessoa.Numero,
                    MunicipioUf = duplicataVisualizar.Pessoa.Localidade.Descricao + "/" + duplicataVisualizar.Pessoa.Localidade.Estado.Sigla,
                    CpfMotorista = duplicataVisualizar.Motorista?.CPF ?? string.Empty,
                    NomeMotorista = duplicataVisualizar.Motorista?.Nome ?? string.Empty,
                    Veiculo1 = duplicataVisualizar.Veiculo1?.Placa ?? string.Empty,
                    Veiculo2 = duplicataVisualizar.Veiculo2?.Placa ?? string.Empty,
                    Veiculo3 = duplicataVisualizar.Veiculo3?.Placa ?? string.Empty,
                    Valor = duplicataVisualizar.Valor,
                    Acrescimo = duplicataVisualizar.Acrescimo,
                    Desconto = duplicataVisualizar.Desconto,
                    Total = (duplicataVisualizar.Valor + duplicataVisualizar.Acrescimo - duplicataVisualizar.Desconto),
                    Observacao = duplicataVisualizar.Observacao,

                    Embarcador = duplicataVisualizar.Embarcador?.Descricao ?? string.Empty,
                    TipoVeiculo = duplicataVisualizar.TipoVeiculo?.Descricao ?? string.Empty,
                    DadosBancarios = duplicataVisualizar.DadosBancarios,
                    LocalidadeOrigem = duplicataVisualizar.AdicionaisCidadeOrigem?.DescricaoCidadeEstado ?? string.Empty,
                    LocalidadeDestino = duplicataVisualizar.AdicionaisCidadeDestino?.DescricaoCidadeEstado ?? string.Empty,
                    AdicionaisPeso = duplicataVisualizar.AdicionaisPeso,
                    AdicionaisVolumes = duplicataVisualizar.AdicionaisVolumes,
                };

                List<ReportParameter> parametros = new List<ReportParameter>
                {
                    new ReportParameter("NomeEmpresa", this.EmpresaUsuario.RazaoSocial),
                    new ReportParameter("CnpjEmpresa", this.EmpresaUsuario.CNPJ),
                    new ReportParameter("IeEmpresa", this.EmpresaUsuario.InscricaoEstadual),
                    new ReportParameter("CidadeEmpresa", this.EmpresaUsuario.Localidade.Descricao + "/" + this.EmpresaUsuario.Localidade.Estado.Sigla),
                    new ReportParameter("TelefoneEmpresa", this.EmpresaUsuario.Telefone),
                    new ReportParameter("EnderecoEmpresa", this.EmpresaUsuario.Endereco + " - " + this.EmpresaUsuario.Numero),
                    new ReportParameter("BairroEmpresa", this.EmpresaUsuario.Bairro),
                    new ReportParameter("CepEmpresa", this.EmpresaUsuario.CEP),
                    new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte)
                };

                if (dsDuplicataVisualizar.Tipo == Dominio.Enumeradores.TipoDuplicata.AReceber)
                    parametros.Add(new ReportParameter("Assinatura", dsDuplicataVisualizar.NomePessoa));
                else
                    parametros.Add(new ReportParameter("Assinatura", this.EmpresaUsuario.RazaoSocial));

                List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicata> dataset = repDuplicata.VisualizarDuplicataSalva();
                dataset.Add(dsDuplicataVisualizar);
                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Duplicatas", dataset)
                };

                string nomeArquivo = "Relatorios/VisualizacaoDuplicata.rdlc";
                string nomeArquivoEmpresa = "Relatorios/VisualizacaoDuplicata" + this.EmpresaUsuario.CNPJ_SemFormato + ".rdlc";
                if (Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivoEmpresa)))
                    nomeArquivo = nomeArquivoEmpresa;

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb(nomeArquivo, tipoArquivo, parametros, dataSources, (object sender, SubreportProcessingEventArgs e) =>
                {
                    List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicataParcelas> listaParcelas = repDuplicata.VisualizarDuplicataParcelas(int.Parse(e.Parameters["CodigoDuplicata"].Values[0]));
                    e.DataSources.Add(new ReportDataSource("Parcelas", listaParcelas));

                    List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoDuplicataCTes> listaCtes = repDuplicata.VisualizarDuplicataCTes(int.Parse(e.Parameters["CodigoDuplicata"].Values[0]));
                    e.DataSources.Add(new ReportDataSource("CTes", listaCtes));
                });

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "VisualizacaoDuplicata." + arquivo.FileNameExtension);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterParcelasPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if ((this.UsuarioAdministrativo != null) || (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomeParcelaDuplicatas))
                    return Json<bool>(false, false, "Parcelas duplicatas sem configuração para exibição na pagina inicial.");

                Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unitOfWork);

                List<Dominio.Entidades.DuplicataParcelas> listaParcelas = new List<Dominio.Entidades.DuplicataParcelas>();
                int diasAvisoVcto = 60;

                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.DiasParaAvisoVencimentos > 0)
                    diasAvisoVcto = this.EmpresaUsuario.Configuracao.DiasParaAvisoVencimentos;

                listaParcelas = repDuplicataParcelas.BuscarParcelasPendentes(this.EmpresaUsuario.Codigo, DateTime.Today.AddDays(diasAvisoVcto));

                List<object> listaRetornoParcelas = new List<object>();

                foreach (Dominio.Entidades.DuplicataParcelas parcela in listaParcelas)
                {
                    listaRetornoParcelas.Add(new
                    {
                        CodigoCriptografado = Servicos.Criptografia.Criptografar(parcela.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                        CodigoDuplicata = parcela.Duplicata.Codigo,
                        NumeroDuplicata = string.Concat(parcela.Duplicata.Numero, "/", parcela.Parcela),
                        Tipo = parcela.Duplicata.Tipo.ToString("g"),
                        PessoaDuplicata = parcela.Duplicata.Pessoa.CPF_CNPJ_Formatado + " " + parcela.Duplicata.Pessoa.Nome,
                        CodigoParcela = parcela.Codigo,
                        VencimentoParcela = parcela.DataVcto.ToString("dd/MM/yyyy"),
                        ValorParcela = parcela.Valor.ToString("n2")
                    });
                }

                return Json(listaRetornoParcelas, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter parcelas pendentes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult ImportarXMLCTeDuplicata()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "Empresa sem configuração.");

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string nomeArquivo = System.IO.Path.GetFileName(file.FileName);
                    if (System.IO.Path.GetExtension(nomeArquivo).ToLower().Equals(".xml"))
                    {
                        string path = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoUploadXMLNotasFiscais"], this.EmpresaUsuario.Codigo.ToString());
                        
                        path = Utilidades.IO.FileStorageService.Storage.Combine(path, "CTe Anterior");

                        Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(path, nomeArquivo), file.InputStream);
                        
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        object retorno = svcCTe.GerarCTeAnterior(file.InputStream, this.EmpresaUsuario.Codigo, string.Empty, string.Empty, unitOfWork, null, true, false);

                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                            {
                                return Json<bool>(false, false, (string)retorno);
                            }
                            else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || retorno.GetType().Name == "ConhecimentoDeTransporteEletronicoProxy" || retorno.GetType().Name == "ConhecimentoDeTransporteEletronicoProxyForFieldInterceptor")
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteImportado = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(cteImportado?.Codigo ?? 0);

                                if (this.EmpresaUsuario.Configuracao.NaoPermitirDuplciataMesmoDocumento)
                                {
                                    Repositorio.DuplicataCtes repDuplicataCtes = new Repositorio.DuplicataCtes(unitOfWork);
                                    Dominio.Entidades.DuplicataCtes duplicataCte = repDuplicataCtes.BuscarPorCodigoCTe(cte.Codigo);
                                    if (duplicataCte != null && duplicataCte.Duplicata.Status == "A")
                                        return Json<bool>(false, false, "CT-e " + cte.Numero.ToString() + " - " + cte.Serie.Numero.ToString() + " esta na Duplicata " + duplicataCte.Duplicata.Numero.ToString() + ". Para importar novamente precisa remover da duplicata atual ou inativa-la.");
                                }

                                return Json(new
                                {
                                    Codigo = 0,
                                    CodigoCte = cte.Codigo,
                                    Numero = cte.Numero,
                                    DescricaoCte = cte.Numero.ToString() + " - " + cte.Serie.Numero.ToString(),
                                    Modelo = cte.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                    TomadorCte = cte.Tomador.Nome,
                                    ValorCte = cte.ValorAReceber
                                }, true);
                            }
                            else if (retorno.GetType().ToString() == "ConhecimentoDeTransporteEletronicoProxy")
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteImportado = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(cteImportado?.Codigo ?? 0);

                                if (this.EmpresaUsuario.Configuracao.NaoPermitirDuplciataMesmoDocumento)
                                {
                                    Repositorio.DuplicataCtes repDuplicataCtes = new Repositorio.DuplicataCtes(unitOfWork);
                                    Dominio.Entidades.DuplicataCtes duplicataCte = repDuplicataCtes.BuscarPorCodigoCTe(cte.Codigo);
                                    if (duplicataCte != null && duplicataCte.Duplicata.Status == "A")
                                        return Json<bool>(false, false, "CT-e " + cte.Numero.ToString() + " - " + cte.Serie.Numero.ToString() + " esta na Duplicata " + duplicataCte.Duplicata.Numero.ToString() + ". Para importar novamente precisa remover da duplicata atual ou inativa-la.");
                                }

                                return Json(new
                                {
                                    Codigo = 0,
                                    CodigoCte = cte.Codigo,
                                    Numero = cte.Numero,
                                    DescricaoCte = cte.Numero.ToString() + " - " + cte.Serie.Numero.ToString(),
                                    Modelo = cte.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                    TomadorCte = cte.Tomador?.Nome ?? string.Empty,
                                    ValorCte = cte.ValorAReceber
                                }, true);
                            }
                            else
                            {
                                return Json<bool>(false, false, "Conhecimento de transporte inválido.");
                            }
                        }
                        else
                        {
                            return Json(true, true);
                        }
                    }
                    else
                    {
                        return Json<bool>(false, false, string.Concat("A extensão do arquivo '", file.FileName, "' é inválida. Somente a extensão XML é aceita."));
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha importar CTe para a duplicata.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public ActionResult BaixarEDICaterpillar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unitOfWork);
                Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (duplicata == null)
                    return Json<bool>(false, false, "Duplicata não localizada.");

                System.IO.MemoryStream arquivo = this.GerarEDICaterpillar(duplicata, unitOfWork);

                string nomeArquivo = "EDI_DUP_" + duplicata.Numero.ToString();

                return Arquivo(arquivo, "text/plain", string.Concat(nomeArquivo, ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarParcelas(Dominio.Entidades.Duplicata duplicata, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Parcelas"]))
            {
                List<Dominio.ObjetosDeValor.DuplicataParcelas> parcelas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DuplicataParcelas>>(Request.Params["Parcelas"]);

                if (parcelas != null)
                {
                    Repositorio.DuplicataParcelas repParcelas = new Repositorio.DuplicataParcelas(unidadeDeTrabalho);

                    List<Dominio.Entidades.DuplicataParcelas> parcelasExcluir = repParcelas.BuscarPorDuplicata(duplicata.Codigo);
                    for (var i = 0; i < parcelasExcluir.Count; i++)
                        repParcelas.Deletar(parcelasExcluir[i]);

                    for (var i = 0; i < parcelas.Count; i++)
                    {
                        var parcela = new Dominio.Entidades.DuplicataParcelas();

                        parcela.Codigo = 0;
                        parcela.Duplicata = duplicata;
                        parcela.Parcela = parcelas[i].Parcela;
                        parcela.Valor = parcelas[i].Valor;
                        parcela.Status = parcelas[i].Status;

                        DateTime dataVcto;
                        DateTime.TryParseExact(parcelas[i].DataVcto, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVcto);
                        parcela.DataVcto = dataVcto;

                        if (parcela.Codigo > 0)
                            repParcelas.Atualizar(parcela);
                        else
                            repParcelas.Inserir(parcela);
                    }
                }
            }
        }

        private void SalvarCtes(Dominio.Entidades.Duplicata duplicata, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Ctes"]))
            {
                List<Dominio.ObjetosDeValor.DuplicataCtes> duplicataCtes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DuplicataCtes>>(Request.Params["Ctes"]);

                if (duplicataCtes != null)
                {
                    Repositorio.DuplicataCtes repDuplicataCtes = new Repositorio.DuplicataCtes(unidadeDeTrabalho);

                    for (var i = 0; i < duplicataCtes.Count; i++)
                    {
                        Dominio.Entidades.DuplicataCtes duplicataCte = repDuplicataCtes.BuscarPorCodigo(duplicataCtes[i].CodigoCte, duplicata.Codigo);

                        if (!duplicataCtes[i].Excluir)
                        {
                            Repositorio.ConhecimentoDeTransporteEletronico repCtes = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCtes.BuscarPorCodigo(duplicataCtes[i].CodigoCte);

                            if (cte != null)
                            {
                                if (duplicataCte == null)
                                    duplicataCte = new Dominio.Entidades.DuplicataCtes();

                                duplicataCte.Duplicata = duplicata;
                                duplicataCte.ConhecimentoDeTransporteEletronico = cte;

                                if (duplicataCte.Codigo > 0)
                                    repDuplicataCtes.Atualizar(duplicataCte);
                                else
                                    repDuplicataCtes.Inserir(duplicataCte);
                            }
                        }
                        else if (duplicataCte != null && duplicataCte.Codigo > 0)
                        {
                            repDuplicataCtes.Deletar(duplicataCte);
                        }
                    }
                }
            }
        }

        private System.IO.MemoryStream GerarEDICaterpillar(Dominio.Entidades.Duplicata duplicata, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (duplicata == null)
                return null;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);
            Repositorio.DuplicataParcelas repDuplicatasParcela = new Repositorio.DuplicataParcelas(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarPorDuplicata(duplicata.Empresa.Codigo, duplicata.Codigo);
            List<Dominio.Entidades.DuplicataParcelas> duplicataParcelas = repDuplicatasParcela.BuscarPorDuplicata(duplicata.Codigo);


            int quantidadeLinhas = 0;
            MemoryStream memoStream = new MemoryStream();
            StringBuilder RegistroEDI = new StringBuilder();
            string linhaFrete = string.Empty;
            string rfCaterp = string.Empty;
            string nrDeclimp = string.Empty;
            string nrMaster = string.Empty;
            string nrHouse = string.Empty;
            string tpContainer = string.Empty;

            //PreencherEspacoDireita(nota.Emitente.CPF_CNPJ_SemFormato, 14)
            //PreencherZeroEsquerda(nota.Numero.ToString(), 9)

            int linha = 0;
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTes)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteFreteLiquido = repComponentes.BuscarPorCTeDescricao(cte.Codigo, "Frete Liquido").FirstOrDefault();
                Dominio.Entidades.ComponentePrestacaoCTE componenteAdValorem = repComponentes.BuscarPorCTeDescricao(cte.Codigo, "Ad. Valorem").FirstOrDefault();
                Dominio.Entidades.ComponentePrestacaoCTE componentePedagio = repComponentes.BuscarPorCTeDescricao(cte.Codigo, "Pedágio").FirstOrDefault();
                Dominio.Entidades.InformacaoCargaCTE informacaoCarga = repInformacaoCarga.BuscarPorCTeUnidade(cte.Codigo, "01");

                if (cte.ObservacoesGerais != null && cte.ObservacoesGerais.Contains("EMBARQUE:")) //EMBARQUE:2019RFM00065
                {
                    int posicao = cte.ObservacoesGerais.IndexOf("EMBARQUE:");
                    int posicaoFim = posicao + 9 + 12;
                    if (posicao > -1 && posicaoFim > -1)
                    {
                        int inicio = posicao + 9;
                        int tamanho = posicaoFim - (posicao + 9);
                        rfCaterp = cte.ObservacoesGerais.Substring(inicio, tamanho).Replace(" ", "");
                    }
                }

                if (cte.ObservacoesGerais != null && cte.ObservacoesGerais.Contains("MASTER BL:")) //MASTER BL:SUDU29003AIARZ2L
                {
                    int posicao = cte.ObservacoesGerais.IndexOf("MASTER BL:");
                    int posicaoFim = posicao + 10 + 16;
                    if (posicao > -1 && posicaoFim > -1)
                    {
                        int inicio = posicao + 10;
                        int tamanho = posicaoFim - (posicao + 10);
                        nrMaster = cte.ObservacoesGerais.Substring(inicio, tamanho).Replace(" ", "");
                    }
                }

                if (cte.ObservacoesGerais != null && cte.ObservacoesGerais.Contains("DI:")) //DI:202003364659
                {
                    int posicao = cte.ObservacoesGerais.IndexOf("DI:");
                    int posicaoFim = posicao + 3 + 12;
                    if (posicao > -1 && posicaoFim > -1)
                    {
                        int inicio = posicao + 3;
                        int tamanho = posicaoFim - (posicao + 3);
                        nrDeclimp = cte.ObservacoesGerais.Substring(inicio, tamanho).Replace(" ", "");
                    }
                }

                decimal baseICMSLinhas = 0;
                decimal valorICMSLinhas = 0;

                linha = linha + 1;

                decimal baseICMSFrete = Math.Round((componenteFreteLiquido?.Valor ?? cte.ValorFrete), 2, MidpointRounding.ToEven);
                decimal valorICMSFrete = Math.Round(((componenteFreteLiquido?.Valor ?? cte.ValorFrete) * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                baseICMSLinhas += baseICMSFrete;
                valorICMSLinhas += valorICMSFrete;

                linhaFrete = "01";
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(duplicata.Documento, 30));
                linhaFrete = string.Concat(linhaFrete, duplicata.DataLancamento.ToString("dd/MM/yyyy"));
                linhaFrete = string.Concat(linhaFrete, duplicataParcelas.FirstOrDefault().DataVcto.ToString("dd/MM/yyyy"));
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(rfCaterp, 12));
                //nrDeclimp = string.Empty;
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrDeclimp, 12));
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrMaster, 18));
                nrHouse = string.Empty;
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrHouse, 18));
                linhaFrete = string.Concat(linhaFrete, PreencherZeroEsquerda(linha.ToString(), 4));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(componenteFreteLiquido?.Valor ?? cte.ValorFrete, 12, 2));
                linhaFrete = string.Concat(linhaFrete, "790");
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(componenteFreteLiquido?.Valor ?? cte.ValorFrete, 12, 2));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(informacaoCarga?.Quantidade ?? 0, 9, 5));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(informacaoCarga?.Quantidade ?? 0, 9, 5));
                linhaFrete = string.Concat(linhaFrete, "000");
                linhaFrete = string.Concat(linhaFrete, "000");
                tpContainer = string.Empty;
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(tpContainer, 20));
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(cte.Numero.ToString(), 30));
                linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(cte.Serie.Numero.ToString(), 4));
                linhaFrete = string.Concat(linhaFrete, cte.DataEmissao.Value.ToString("dd/MM/yyyy"));
                linhaFrete = string.Concat(linhaFrete, cte.Empresa.CNPJ);
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? baseICMSFrete : 0, 12, 2));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? cte.AliquotaICMS : 0, 3, 2));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? valorICMSFrete : 0, 12, 2));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? baseICMSFrete : 0, 12, 2));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? cte.AliquotaICMS : 0, 3, 2));
                linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? valorICMSFrete : 0, 12, 2));
                RegistroEDI.AppendLine(linhaFrete);
                quantidadeLinhas = quantidadeLinhas + 1;

                if (componenteAdValorem != null && componenteAdValorem.Valor > 0)
                {
                    decimal baseICMS = Math.Round(componenteAdValorem.Valor, 2, MidpointRounding.ToEven);
                    decimal valorICMS = Math.Round((componenteAdValorem.Valor * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    baseICMSLinhas += baseICMS;
                    valorICMSLinhas += valorICMS;

                    linha = linha + 1;
                    linhaFrete = "03";
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(duplicata.Documento, 30));
                    linhaFrete = string.Concat(linhaFrete, duplicata.DataLancamento.ToString("dd/MM/yyyy"));
                    linhaFrete = string.Concat(linhaFrete, duplicataParcelas.FirstOrDefault().DataVcto.ToString("dd/MM/yyyy"));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(rfCaterp, 12));
                    //nrDeclimp = string.Empty;
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrDeclimp, 12));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrMaster, 18));
                    nrHouse = string.Empty;
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrHouse, 18));
                    linhaFrete = string.Concat(linhaFrete, PreencherZeroEsquerda(linha.ToString(), 4));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(componenteAdValorem.Valor, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, "790");
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(componenteAdValorem.Valor, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(informacaoCarga?.Quantidade ?? 0, 9, 5));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(informacaoCarga?.Quantidade ?? 0, 9, 5));
                    linhaFrete = string.Concat(linhaFrete, "000");
                    linhaFrete = string.Concat(linhaFrete, "000");
                    tpContainer = string.Empty;
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(tpContainer, 20));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(cte.Numero.ToString(), 30));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(cte.Serie.Numero.ToString(), 4));
                    linhaFrete = string.Concat(linhaFrete, cte.DataEmissao.Value.ToString("dd/MM/yyyy"));
                    linhaFrete = string.Concat(linhaFrete, cte.Empresa.CNPJ);
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? baseICMS : 0, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? cte.AliquotaICMS : 0, 3, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? valorICMS : 0, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? baseICMS : 0, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? cte.AliquotaICMS : 0, 3, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? valorICMS : 0, 12, 2));
                    RegistroEDI.AppendLine(linhaFrete);
                    quantidadeLinhas = quantidadeLinhas + 1;

                }

                if (componentePedagio != null && componentePedagio.Valor > 0)
                {
                    decimal baseICMS = Math.Round(componentePedagio.Valor, 2, MidpointRounding.ToEven);
                    decimal valorICMS = Math.Round((componentePedagio.Valor * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    baseICMSLinhas += baseICMS;
                    valorICMSLinhas += valorICMS;

                    if (cte.BaseCalculoICMS != baseICMSLinhas)
                        baseICMS = baseICMS + (cte.BaseCalculoICMS - baseICMSLinhas);
                    if (cte.ValorICMS != valorICMSLinhas)
                        valorICMS = valorICMS + (cte.ValorICMS - valorICMSLinhas);

                    linha = linha + 1;

                    linhaFrete = "04";
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(duplicata.Documento, 30));
                    linhaFrete = string.Concat(linhaFrete, duplicata.DataLancamento.ToString("dd/MM/yyyy"));
                    linhaFrete = string.Concat(linhaFrete, duplicataParcelas.FirstOrDefault().DataVcto.ToString("dd/MM/yyyy"));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(rfCaterp, 12));
                    //nrDeclimp = string.Empty;
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrDeclimp, 12));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrMaster, 18));
                    nrHouse = string.Empty;
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(nrHouse, 18));
                    linhaFrete = string.Concat(linhaFrete, PreencherZeroEsquerda(linha.ToString(), 4));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(componentePedagio.Valor, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, "790");
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(componentePedagio.Valor, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(informacaoCarga?.Quantidade ?? 0, 9, 5));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(informacaoCarga?.Quantidade ?? 0, 9, 5));
                    linhaFrete = string.Concat(linhaFrete, "000");
                    linhaFrete = string.Concat(linhaFrete, "000");
                    tpContainer = string.Empty;
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(tpContainer, 20));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(cte.Numero.ToString(), 30));
                    linhaFrete = string.Concat(linhaFrete, PreencherEspacoDireita(cte.Serie.Numero.ToString(), 4));
                    linhaFrete = string.Concat(linhaFrete, cte.DataEmissao.Value.ToString("dd/MM/yyyy"));
                    linhaFrete = string.Concat(linhaFrete, cte.Empresa.CNPJ);
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? baseICMS : 0, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? cte.AliquotaICMS : 0, 3, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST != "60" ? valorICMS : 0, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? baseICMS : 0, 12, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? cte.AliquotaICMS : 0, 3, 2));
                    linhaFrete = string.Concat(linhaFrete, FormatarNumero(cte.CST == "60" ? valorICMS : 0, 12, 2));
                    RegistroEDI.AppendLine(linhaFrete);
                    quantidadeLinhas = quantidadeLinhas + 1;
                }
            }

            string linhaCabecalho = string.Concat("ALI", DateTime.Now.ToString("dd/MM/yyyyHH:mm"), quantidadeLinhas.ToString().PadLeft(5, '0'));
            RegistroEDI.Insert(0, linhaCabecalho + "\r\n");

            string arquivo = RegistroEDI.ToString();

            memoStream.Write(System.Text.Encoding.UTF8.GetBytes(arquivo), 0, arquivo.Length);

            memoStream.Position = 0;

            return memoStream;
        }

        protected string FormatarNumero(decimal valor, int quantidadeInteiros, int quantidadeDecimais)
        {
            string formato = string.Concat("{0:", new string('0', quantidadeInteiros), ".", new string('0', quantidadeDecimais), "}");

            string valorFormatado = string.Format(formato, valor).Replace(".", "").Replace(",", "").Replace("-", "").Replace("+", "");

            return valorFormatado;
        }

        private string PreencherEspacoDireita(string dado, int numeroCaracteres)
        {
            if (!string.IsNullOrWhiteSpace(dado))
            {
                if (dado.Length > numeroCaracteres)
                {
                    return dado.Remove(numeroCaracteres, (dado.Length - numeroCaracteres));
                }
                else
                {
                    if (dado.Length != numeroCaracteres)
                        return string.Concat(dado, new string(' ', numeroCaracteres - dado.Length));
                    else
                        return dado;
                }
            }
            return new string(' ', numeroCaracteres);
        }

        private string PreencherZeroEsquerda(string dado, int numeroCaracteres)
        {
            if (!string.IsNullOrWhiteSpace(dado))
            {
                if (dado.Length > numeroCaracteres)
                {
                    return dado.Remove(numeroCaracteres, (dado.Length - numeroCaracteres));
                }
                else if (dado.Length < numeroCaracteres)
                {
                    return string.Concat(new string('0', numeroCaracteres - dado.Length), dado);
                }
                else if (dado.Length == numeroCaracteres)
                    return dado;
            }
            return new string(' ', numeroCaracteres);
        }

        #endregion
    }
}