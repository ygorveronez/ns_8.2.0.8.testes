using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class AcertoDeViagemController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("acertosdeviagens.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ObterAcertosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if ((this.UsuarioAdministrativo != null) || (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomeAcertoViagem))
                    return Json<bool>(false, false, "Acerto de viagem sem configuração para exibição na pagina inicial.");

                int diasAvisoVcto = 60;
                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.DiasParaAvisoVencimentos > 0)
                    diasAvisoVcto = this.EmpresaUsuario.Configuracao.DiasParaAvisoVencimentos;

                Repositorio.AcertoDeViagem repAcertoDeViagem = new Repositorio.AcertoDeViagem(unitOfWork);

                List<Dominio.Entidades.AcertoDeViagem> listaAcertoDeViagem = repAcertoDeViagem.BuscarParcelasPendentes(this.EmpresaUsuario.Codigo, DateTime.Today.AddDays(diasAvisoVcto));

                List<object> listaRetornoPagamentos = new List<object>();

                foreach (Dominio.Entidades.AcertoDeViagem acerto in listaAcertoDeViagem)
                {
                    listaRetornoPagamentos.Add(new
                    {
                        CodigoCriptografado = Servicos.Criptografia.Criptografar(acerto.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                        Codigo = acerto.Codigo,
                        Numero = acerto.Numero,
                        DataPagamento = acerto.DataVencimento.HasValue ? acerto.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                        Motorista = string.Concat(acerto.Motorista.CPF_Formatado, " ", acerto.Motorista.Nome),
                        Valor = (acerto.Comissao - acerto.Adiantamento + acerto.TotalDespesasPagasMotoristas).ToString("n2")
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
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string veiculo = Request.Params["Veiculo"];
                string motorista = Request.Params["Motorista"];
                string status = Request.Params["Status"];

                int.TryParse(Request.Params["Numero"], out int numero);
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                Repositorio.AcertoDeViagem repAcertoViagem = new Repositorio.AcertoDeViagem(unitOfWork);

                List<Dominio.Entidades.AcertoDeViagem> listaAcertosViagens = repAcertoViagem.Consultar(this.EmpresaUsuario.Codigo, status, motorista, veiculo, numero, inicioRegistros, 50);

                int countAcertosViagens = repAcertoViagem.ContarConsulta(this.EmpresaUsuario.Codigo, status, motorista, veiculo, numero);

                var retorno = (from obj in listaAcertosViagens
                               select
                                     new
                                     {
                                         obj.Codigo,
                                         obj.Numero,
                                         DataLancamento = obj.DataLancamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                         Placa = obj.Veiculo?.Placa ?? obj.Placa,
                                         NomeMotorista = obj.Motorista?.Nome ?? string.Empty,
                                         obj.DescricaoSituacao
                                     }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|10", "Data Lançamento|15", "Veículo|15", "Motorista|35", "Situacao|15" }, countAcertosViagens);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os acertos de viagens.");
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
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                Repositorio.AcertoDeViagem repAcertoViagem = new Repositorio.AcertoDeViagem(unitOfWork);
                Repositorio.AcertoDeViagemAnexos repAcertoDeViagemAnexos = new Repositorio.AcertoDeViagemAnexos(unitOfWork);

                if (codigo == 0)
                    int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoX"], "CT3##MULT1@#$S0FTW4R3"), out codigo);

                Dominio.Entidades.AcertoDeViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                List<Dominio.Entidades.AcertoDeViagemAnexos> anexos = repAcertoDeViagemAnexos.BuscarPorAcerto(codigo);

                if (acertoViagem != null)
                {
                    var retorno = new
                    {
                        acertoViagem.Numero,
                        acertoViagem.Adiantamento,
                        acertoViagem.Codigo,
                        DataFechamento = acertoViagem.DataFechamento != null ? acertoViagem.DataFechamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataLancamento = acertoViagem.DataLancamento != null ? acertoViagem.DataLancamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataVcto = acertoViagem.DataVencimento != null ? acertoViagem.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                        CodigoMotorista = acertoViagem.Motorista != null ? acertoViagem.Motorista.Codigo : 0,
                        DescricaoMotorista = acertoViagem.Motorista != null ? string.Concat(acertoViagem.Motorista.CPF, " - ", acertoViagem.Motorista.Nome) : string.Empty,
                        acertoViagem.Observacao,
                        acertoViagem.PercentualComissao,
                        CodigoVeiculo = acertoViagem.Veiculo != null ? acertoViagem.Veiculo.Codigo : 0,
                        DescricaoVeiculo = acertoViagem.Veiculo != null ? acertoViagem.Veiculo.Placa : acertoViagem.Placa,
                        TipoVeiculo = acertoViagem.Veiculo?.TipoDoVeiculo?.Codigo ?? 0,
                        acertoViagem.Situacao,
                        acertoViagem.Status,
                        acertoViagem.TotalDespesas,
                        acertoViagem.TotalReceitas,
                        acertoViagem.TipoComissao,
                        Anexos = (from anexo in anexos select RetornaDynAnexo(anexo)).ToList()
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Acerto de viagem não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do acerto de viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesDestinoCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.InformacaoCargaCTE repInfoCarga = new Repositorio.InformacaoCargaCTE(unitOfWork);
                Repositorio.DestinoDoAcertoDeViagem repDestinoAcertoViagem = new Repositorio.DestinoDoAcertoDeViagem(unitOfWork);

                int.TryParse(Request.Params["CodigoCTe"], out int codigoCTe);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                Dominio.Entidades.DestinoDoAcertoDeViagem acertoDoCTe = repDestinoAcertoViagem.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);

                if (cte == null)
                    return Json<bool>(false, false, "Não foi possível obter os detalhes do CT-e.");

                var retorno = new
                {
                    AcertoVinculado = (acertoDoCTe != null && this.EmpresaUsuario.Configuracao.BloquearDuplicidadeCTeAcerto) ? acertoDoCTe.NumeroAcerto : 0,
                    CodigoLocalidadeInicio = cte.LocalidadeInicioPrestacao.Codigo,
                    CodigoLocalidadeFim = cte.LocalidadeTerminoPrestacao.Codigo,
                    UFInicio = cte.LocalidadeInicioPrestacao.Estado.Sigla,
                    UFFim = cte.LocalidadeTerminoPrestacao.Estado.Sigla,
                    PesoTotal = repInfoCarga.ObterPesoTotal(cte.Codigo)
                };

                return Json(retorno, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do CT-e para o destino.");
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
                int codigoMotorista, codigoVeiculo, codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                DateTime dataLancamento;
                DateTime.TryParseExact(Request.Params["DataLancamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLancamento);
                DateTime.TryParseExact(Request.Params["DataVcto"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVcto);

                decimal adiantamento, comissao, totalReceitas, totalDespesas = 0;
                decimal.TryParse(Request.Params["Adiantamento"], out adiantamento);
                decimal.TryParse(Request.Params["Comissao"], out comissao);
                decimal.TryParse(Request.Params["TotalReceitas"], out totalReceitas);
                decimal.TryParse(Request.Params["TotalDespesas"], out totalDespesas);

                Dominio.Enumeradores.TipoComissao tipoComissao;
                Enum.TryParse<Dominio.Enumeradores.TipoComissao>(Request.Params["TipoDespesa"], out tipoComissao);

                string situacao = Request.Params["Situacao"];
                string status = Request.Params["Status"];
                string observacao = Request.Params["Observacao"];
                string descricaoVeiculo = Request.Params["DescricaoVeiculo"];

                Repositorio.AcertoDeViagem repAcertoDeViagem = new Repositorio.AcertoDeViagem(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Dominio.Entidades.AcertoDeViagem acertoDeViagem;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada!");

                    acertoDeViagem = repAcertoDeViagem.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                    if (acertoDeViagem.Situacao == "F")
                        return Json<bool>(false, false, "Acerto de Viagem já se encontra com situação Fechado.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada!");

                    acertoDeViagem = new Dominio.Entidades.AcertoDeViagem();
                    acertoDeViagem.Empresa = this.EmpresaUsuario;
                    acertoDeViagem.Numero = repAcertoDeViagem.BuscarProximoNumero(this.EmpresaUsuario.Codigo);
                }

                unidadeDeTrabalho.Start();

                acertoDeViagem.DataLancamento = dataLancamento;
                if (dataVcto > DateTime.MinValue)
                    acertoDeViagem.DataVencimento = dataVcto;
                else
                    acertoDeViagem.DataVencimento = null;
                acertoDeViagem.Adiantamento = adiantamento;
                acertoDeViagem.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista);
                acertoDeViagem.Observacao = observacao;
                acertoDeViagem.PercentualComissao = comissao;
                acertoDeViagem.Situacao = situacao;
                acertoDeViagem.TipoComissao = tipoComissao;

                if (situacao == "F")
                    acertoDeViagem.DataFechamento = DateTime.Now;

                acertoDeViagem.Status = status;
                acertoDeViagem.TotalDespesas = totalDespesas;
                acertoDeViagem.TotalReceitas = totalReceitas;

                if (codigoVeiculo > 0)
                {
                    acertoDeViagem.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                    acertoDeViagem.Placa = acertoDeViagem.Veiculo.Placa;
                }
                else
                {
                    acertoDeViagem.Veiculo = null;
                    acertoDeViagem.Placa = descricaoVeiculo;
                }

                if (codigo > 0)
                    repAcertoDeViagem.Atualizar(acertoDeViagem);
                else
                    repAcertoDeViagem.Inserir(acertoDeViagem);

                this.SalvarAbastecimentos(acertoDeViagem, unidadeDeTrabalho);
                this.SalvarDespesas(ref acertoDeViagem, unidadeDeTrabalho);
                repAcertoDeViagem.Atualizar(acertoDeViagem);

                acertoDeViagem.TotalVales = 0;

                this.SalvarVales(ref acertoDeViagem, unidadeDeTrabalho);

                acertoDeViagem.TotalReceitasCTe = 0;
                acertoDeViagem.TotalReceitasOutros = 0;

                this.SalvarDestinos(ref acertoDeViagem, unidadeDeTrabalho);

                if (acertoDeViagem.Situacao == "F" && !this.GerarMovimentos(acertoDeViagem, unidadeDeTrabalho, out string erroMovimento))
                {
                    unidadeDeTrabalho.Rollback();
                    return Json<bool>(false, false, erroMovimento);
                }

                unidadeDeTrabalho.CommitChanges();
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o acerto de viagem.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarKilometragem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            decimal kmMaxima = 0;
            try
            {
                string placa = Request.Params["Placa"];
                Repositorio.AcertoDeViagem repAcerto = new Repositorio.AcertoDeViagem(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                List<Dominio.Entidades.AcertoDeViagem> acertos = repAcerto.BuscarPorEmpresaPlaca(this.EmpresaUsuario.Codigo, placa);


                if (acertos != null && acertos.Count > 0)
                {
                    List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repAbastecimento.BuscarPorAcertos(acertos.Select(o => o.Codigo).ToList());

                    kmMaxima = listaAbastecimentos.Max(o => o.Kilometragem);
                }

                return Json(new { kmMaxima }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(new { kmMaxima }, true);
                //return Json<bool>(false, false, "Ocorreu uma falha ao obter a kilometragem do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult VisualizarAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.AcertoDeViagem repAcertoViagem = new Repositorio.AcertoDeViagem(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.DespesaDoAcertoDeViagem repDespesa = new Repositorio.DespesaDoAcertoDeViagem(unitOfWork);
                Repositorio.DestinoDoAcertoDeViagem repDestino = new Repositorio.DestinoDoAcertoDeViagem(unitOfWork);
                Repositorio.ValeDoAcertoDeViagem repVale = new Repositorio.ValeDoAcertoDeViagem(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                List<int> codigosAcertos = new List<int>() { codigo };

                Dominio.Entidades.AcertoDeViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                List<Dominio.Entidades.AcertoDeViagem> listaAcertos = new List<Dominio.Entidades.AcertoDeViagem>()
                {
                    acertoViagem
                };

                List<ReportParameter> parametros = new List<ReportParameter>
                {
                    new ReportParameter("NumeroAcerto", acertoViagem.Numero.ToString()),
                    new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial),
                    new ReportParameter("InscricaoEstadual", this.EmpresaUsuario.InscricaoEstadual),
                    new ReportParameter("CNPJ", this.EmpresaUsuario.CNPJ_Formatado),
                    new ReportParameter("Motorista", string.Concat(acertoViagem.Motorista.CPF, " - ", acertoViagem.Motorista.Nome)),
                    new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte),
                    new ReportParameter("Proprietario", acertoViagem.Veiculo.Proprietario != null? string.Concat(acertoViagem.Veiculo.Proprietario.CPF_CNPJ_Formatado, " - ", acertoViagem.Veiculo.Proprietario.Nome) : string.Empty),
                    new ReportParameter("Placa", acertoViagem.Veiculo.Placa)
                };

                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Acertos", listaAcertos)
                };

                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repAbastecimento.BuscarPorAcertos(codigosAcertos);
                List<Dominio.Entidades.DespesaDoAcertoDeViagem> listaDespesas = repDespesa.BuscarPorAcertos(codigosAcertos);
                List<Dominio.Entidades.DestinoDoAcertoDeViagem> listaDestinos = repDestino.BuscarPorAcertos(codigosAcertos);
                List<Dominio.Entidades.ValeDoAcertoDeViagem> listaVales = repVale.BuscarPorAcertos(codigosAcertos);

                string relatorio = "Relatorios/VisualizacaoAcertoDetalhadoTotais.rdlc";

                if (this.EmpresaUsuario.CNPJ == "18805855000155" || this.EmpresaUsuario.CNPJ == "12656321000128") //Relatório que soma as receitas e despesas nos totais do acerto
                    relatorio = "Relatorios/VisualizacaoAcertoDetalhado.rdlc";

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb(relatorio, "PDF", parametros, dataSources, (object sender, SubreportProcessingEventArgs e) =>
                {
                    if (e.ReportPath.Contains("Abastecimentos"))
                    {
                        e.DataSources.Add(new ReportDataSource("Abastecimentos", listaAbastecimentos));
                    }
                    else if (e.ReportPath.Contains("Despesas"))
                    {
                        e.DataSources.Add(new ReportDataSource("Despesas", listaDespesas));
                    }
                    else if (e.ReportPath.Contains("Destinos"))
                    {
                        e.DataSources.Add(new ReportDataSource("Destinos", listaDestinos));
                    }
                    else if (e.ReportPath.Contains("Vales"))
                    {
                        e.DataSources.Add(new ReportDataSource("Vales", listaVales));
                    }
                });

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, String.Concat("Acerto de Viagem " + acertoViagem.Numero.ToString() + ".", arquivo.FileNameExtension));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Anexar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Config extensoes validas
                string[] extensoesValidas = { ".pdf", ".jpg", ".jpeg", ".png" };

                // Converter dados
                int codigoAcerto = 0;
                int.TryParse(Request.Params["Codigo"], out codigoAcerto);

                // Busca entidades
                Repositorio.AcertoDeViagem repAcertoDeViagem = new Repositorio.AcertoDeViagem(unitOfWork);
                Repositorio.AcertoDeViagemAnexos repOcorrenciaDeCTeAnexos = new Repositorio.AcertoDeViagemAnexos(unitOfWork);

                Dominio.Entidades.AcertoDeViagem acerto = repAcertoDeViagem.BuscarPorCodigo(codigoAcerto, this.EmpresaUsuario.Codigo);

                if (acerto == null)
                    return Json<bool>(false, false, "Acerto de viagem não encontrado.");

                if (Request.Files.Count == 0)
                    return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");

                // Manipula arquivo
                HttpPostedFileBase file = Request.Files[0];

                // Valida extensao
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!extensoesValidas.Contains(extensao))
                    return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");

                // Inicia instancia
                unitOfWork.Start();

                // Insere
                string guidAqruivo = Guid.NewGuid().ToString().Replace("-", "");
                Dominio.Entidades.AcertoDeViagemAnexos anexo = new Dominio.Entidades.AcertoDeViagemAnexos();

                anexo.AcertoDeViagem = acerto;
                anexo.NomeArquivo = file.FileName;
                anexo.GuidArquivo = guidAqruivo;

                repOcorrenciaDeCTeAnexos.Inserir(anexo);

                // Salva na pasta configurada
                string caminho = this.CaminhoArquivo();
                string arquivoFisico = guidAqruivo + extensao;
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                Utilidades.IO.FileStorageService.Storage.SaveStream(arquivoFisico, file.InputStream);

                // Fecha instancia
                unitOfWork.CommitChanges();

                return Json(RetornaDynAnexo(anexo), true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Converter dados
                int codigo = 0;
                int codigoAcerto = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Acerto"], out codigoAcerto);

                // Busca entidades
                Repositorio.AcertoDeViagemAnexos repAcertoDeViagemAnexos = new Repositorio.AcertoDeViagemAnexos(unitOfWork);
                Dominio.Entidades.AcertoDeViagemAnexos anexo = repAcertoDeViagemAnexos.BuscarPorCodigoEAcerto(codigo, codigoAcerto, this.EmpresaUsuario.Codigo);

                if (anexo == null)
                    return Json<bool>(false, false, "Anexo não encontrado.");

                // Busca arquivo fisico
                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string caminho = this.CaminhoArquivo();
                string arquivoFisico = anexo.GuidArquivo + extensao;

                // Monta caminho absoluto
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                // Inicia instancia
                unitOfWork.Start();

                // Deleta registro
                repAcertoDeViagemAnexos.Deletar(anexo);

                // Deleta o arquivo fisico
                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivoFisico);

                // Fecha instancia
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return Json("Arquivo excluído com sucesso.", true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao excluir o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }
        
        [AcceptVerbs("GET")]        
        public ActionResult DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Converter dados
                int codigo = 0;
                int codigoAcerto = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Acerto"], out codigoAcerto);

                // Busca entidades
                Repositorio.AcertoDeViagemAnexos repAcertoDeViagemAnexos = new Repositorio.AcertoDeViagemAnexos(unitOfWork);
                Dominio.Entidades.AcertoDeViagemAnexos anexo = repAcertoDeViagemAnexos.BuscarPorCodigoEAcerto(codigo, codigoAcerto, this.EmpresaUsuario.Codigo);

                if (anexo == null)
                    return Json<bool>(false, false, "Anexo não encontrado.");

                // Busca arquivo fisico
                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string caminho = this.CaminhoArquivo();
                string arquivoFisico = anexo.GuidArquivo + extensao;

                // Monta caminho absoluto
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                // Arquivo fisico nao existe
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoFisico))
                    return Json<bool>(false, false, "Anexo não encontrado.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoFisico), MimeMapping.GetMimeMapping(arquivoFisico), anexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private bool GerarMovimentos(Dominio.Entidades.AcertoDeViagem acertoDeViagem, Repositorio.UnitOfWork unitOfWork, out string erroMovimento)
        {
            erroMovimento = "";

            Servicos.AcertoDeViagem srvAcertoDeViagem = new Servicos.AcertoDeViagem(acertoDeViagem, acertoDeViagem.DataLancamento.HasValue ? acertoDeViagem.DataLancamento.Value : DateTime.Today, unitOfWork);

            Repositorio.DestinoDoAcertoDeViagem repDestinoDoAcertoDeViagem = new Repositorio.DestinoDoAcertoDeViagem(unitOfWork);
            Repositorio.DespesaDoAcertoDeViagem repDespesaDoAcertoDeViagem = new Repositorio.DespesaDoAcertoDeViagem(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.ValeDoAcertoDeViagem repValeDoAcertoDeViagem = new Repositorio.ValeDoAcertoDeViagem(unitOfWork);

            Dominio.Entidades.ConfiguracaoEmpresa configuracao = acertoDeViagem.Empresa.Configuracao;

            if (configuracao.AcertoViagemContaReceitas != null)
            {
                List<Dominio.Entidades.DestinoDoAcertoDeViagem> destinos = repDestinoDoAcertoDeViagem.BuscarPorAcertoDeViagem(acertoDeViagem.Codigo);
                srvAcertoDeViagem.GerarMovimentoReceitas(configuracao.AcertoViagemContaReceitas, configuracao.AcertoViagemMovimentoReceitas, destinos);
            }

            if (configuracao.AcertoViagemContaDespesas != null)
            {
                List<Dominio.Entidades.DespesaDoAcertoDeViagem> despesas = repDespesaDoAcertoDeViagem.BuscarPorAcertoDeViagem(acertoDeViagem.Codigo);
                srvAcertoDeViagem.GerarMovimentoDespesas(configuracao.AcertoViagemContaDespesas, configuracao.AcertoViagemMovimentoDespesas, despesas);
            }

            if (configuracao.AcertoViagemContaDespesasAbastecimentos != null)
            {
                List<Dominio.Entidades.Abastecimento> abastecimentos = repAbastecimento.BuscarPorAcertoDeViagem(acertoDeViagem.Codigo);
                srvAcertoDeViagem.GerarMovimentoAbastecimentos(configuracao.AcertoViagemContaDespesasAbastecimentos, configuracao.AcertoViagemMovimentoDespesasAbastecimentos, abastecimentos);
            }

            if (configuracao.AcertoViagemContaDespesasPagamentosMotorista != null)
            {
                srvAcertoDeViagem.GerarMovimentoPagamentosMotorista(configuracao.AcertoViagemContaDespesasPagamentosMotorista);
            }

            if (configuracao.AcertoViagemContaDespesasAdiantamentosMotorista != null)
            {
                List<Dominio.Entidades.ValeDoAcertoDeViagem> vales = repValeDoAcertoDeViagem.BuscarPorAcertoDeViagem(acertoDeViagem.Codigo);
                srvAcertoDeViagem.GerarMovimentoAdiantamentosMotorista(configuracao.AcertoViagemContaDespesasAdiantamentosMotorista, configuracao.AcertoViagemMovimentoDespesasAdiantamentosMotorista, vales);
            }

            if (configuracao.AcertoViagemContaReceitasDevolucoesMotorista != null)
            {
                List<Dominio.Entidades.ValeDoAcertoDeViagem> vales = repValeDoAcertoDeViagem.BuscarPorAcertoDeViagem(acertoDeViagem.Codigo);
                srvAcertoDeViagem.GerarMovimentoDevolucoesMotorista(configuracao.AcertoViagemContaReceitasDevolucoesMotorista, configuracao.AcertoViagemMovimentoReceitasDevolucoesMotorista, vales);
            }
            else if (configuracao.AcertoViagemMovimentoDespesasAdiantamentosMotorista == Dominio.Enumeradores.TipoMovimentoAcerto.Detalhado)
            {
                erroMovimento = "Plano de Conta da Receita de Devoluções Motorista é obrigatório quando o Tipo de Movimento de Adiantamentos for detalhado";
                return false;
            }

            srvAcertoDeViagem.ProcessarMovimentos();

            return true;
        }

        private void SalvarDestinos(ref Dominio.Entidades.AcertoDeViagem acertoDeViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Destinos"]))
            {
                bool bloquearDuplicidadeCTeAcerto = this.EmpresaUsuario.Configuracao.BloquearDuplicidadeCTeAcerto;
                Repositorio.DestinoDoAcertoDeViagem repDestinoAcertoViagem = new Repositorio.DestinoDoAcertoDeViagem(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.TipoCarga repTipoCarga = new Repositorio.TipoCarga(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.DestinoAcertoDeViagem> destinos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DestinoAcertoDeViagem>>(Request.Params["Destinos"]);

                for (var i = 0; i < destinos.Count; i++)
                {
                    Dominio.Entidades.DestinoDoAcertoDeViagem destino = repDestinoAcertoViagem.BuscarPorCodigoEAcertoDeViagem(destinos[i].Codigo, acertoDeViagem.Codigo);
                    if (!destinos[i].Excluir)
                    {
                        if (destino == null)
                            destino = new Dominio.Entidades.DestinoDoAcertoDeViagem();
                        destino.AcertoDeViagem = acertoDeViagem;
                        destino.CTe = repCTe.BuscarPorId(destinos[i].CodigoCTe, this.EmpresaUsuario.Codigo);
                        Dominio.Entidades.DestinoDoAcertoDeViagem acertoCTe = repDestinoAcertoViagem.BuscarPorCTe(this.EmpresaUsuario.Codigo, destinos[i].CodigoCTe);

                        // Valida se CTe ja pertence a outro acerto
                        if (acertoCTe != null && bloquearDuplicidadeCTeAcerto && acertoCTe.AcertoDeViagem.Codigo != acertoDeViagem.Codigo)
                            continue;

                        if (destino.CTe != null)
                            acertoDeViagem.TotalReceitasCTe += decimal.Parse(destinos[i].ValorFrete);
                        else
                            acertoDeViagem.TotalReceitasOutros += decimal.Parse(destinos[i].ValorFrete);

                        DateTime dataFinal, dataInicial;
                        DateTime.TryParseExact(destinos[i].DataFinal, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                        DateTime.TryParseExact(destinos[i].DataInicial, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                        if (dataFinal != DateTime.MinValue)
                            destino.DataFinal = dataFinal;
                        else
                            destino.DataFinal = null;
                        if (dataInicial != DateTime.MinValue)
                            destino.DataInicial = dataInicial;
                        else
                            destino.DataInicial = null;
                        if (!string.IsNullOrWhiteSpace(destinos[i].CodigoCliente) && destinos[i].CodigoCliente != "0")
                            destino.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(destinos[i].CodigoCliente));
                        destino.Destino = repLocalidade.BuscarPorCodigo(destinos[i].MunicipioDestino);
                        destino.KilometragemFinal = destinos[i].KMFinal;
                        destino.KilometragemInicial = destinos[i].KMInicial;
                        destino.Observacao = destinos[i].Observacao;
                        destino.Origem = repLocalidade.BuscarPorCodigo(destinos[i].MunicipioOrigem);
                        destino.PesoCarga = decimal.Parse(destinos[i].Peso);
                        destino.TipoCarga = repTipoCarga.BuscarPorCodigo(destinos[i].CodigoTipoCarga, this.EmpresaUsuario.Codigo);
                        destino.ValorFrete = decimal.Parse(destinos[i].ValorFrete);
                        decimal.TryParse(destinos[i].ValorUnitario, out decimal valorUnitario);
                        destino.ValorUnitario = valorUnitario;
                        decimal.TryParse(destinos[i].OutrosDescontos, out decimal outrosDescontos);
                        destino.OutrosDescontos = outrosDescontos;
                        if (destino.Codigo > 0)
                            repDestinoAcertoViagem.Atualizar(destino);
                        else
                            repDestinoAcertoViagem.Inserir(destino);
                    }
                    else if (destino != null)
                    {
                        repDestinoAcertoViagem.Deletar(destino);
                    }
                }
            }
        }

        private void SalvarDespesas(ref Dominio.Entidades.AcertoDeViagem acertoDeViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Despesas"]))
            {
                Repositorio.DespesaDoAcertoDeViagem repDespesaAcertoViagem = new Repositorio.DespesaDoAcertoDeViagem(unidadeDeTrabalho);
                Repositorio.TipoDespesa repTipoDespesa = new Repositorio.TipoDespesa(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Servicos.Despesa svcDespesa = new Servicos.Despesa(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.DespesaAcertoDeViagem> despesas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DespesaAcertoDeViagem>>(Request.Params["Despesas"]);
                decimal valorPagoMotorista = 0;
                for (var i = 0; i < despesas.Count; i++)
                {
                    Dominio.Entidades.DespesaDoAcertoDeViagem despesa = repDespesaAcertoViagem.BuscarPorCodigoEAcertoDeViagem(despesas[i].Codigo, acertoDeViagem.Codigo);

                    if (!despesas[i].Excluir)
                    {
                        if (despesa == null)
                            despesa = new Dominio.Entidades.DespesaDoAcertoDeViagem();

                        despesa.AcertoDeViagem = acertoDeViagem;

                        DateTime data;
                        DateTime.TryParseExact(despesas[i].Data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                        if (data != DateTime.MinValue)
                            despesa.Data = data;
                        else
                            despesa.Data = null;

                        despesa.Descricao = despesas[i].Descricao;
                        despesa.Observacao = despesas[i].Observacao;
                        despesa.Paga = despesas[i].Paga;
                        despesa.Quantidade = decimal.Parse(despesas[i].Quantidade);
                        despesa.TipoDespesa = repTipoDespesa.BuscarPorCodigo(despesas[i].CodigoTipoDespesa, this.EmpresaUsuario.Codigo);
                        despesa.ValorUnitario = decimal.Parse(despesas[i].ValorUnitario);

                        if (string.IsNullOrWhiteSpace(despesas[i].CodigoFornecedor))
                        {
                            despesa.Fornecedor = null;
                            despesa.NomeFornecedor = despesas[i].DescricaoFornecedor;
                        }
                        else
                        {
                            double cpfCnpj = 0;
                            double.TryParse(Utilidades.String.OnlyNumbers(despesas[i].CodigoFornecedor), out cpfCnpj);
                            despesa.Fornecedor = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
                            despesa.NomeFornecedor = string.Empty;
                        }

                        if (despesa.Codigo > 0)
                            repDespesaAcertoViagem.Atualizar(despesa);
                        else
                            repDespesaAcertoViagem.Inserir(despesa);

                        if (despesa.Paga)
                            valorPagoMotorista += (decimal.Parse(despesas[i].ValorUnitario) * decimal.Parse(despesas[i].Quantidade));

                        if (acertoDeViagem.Empresa.Configuracao?.AcertoViagemContaDespesas == null)
                            svcDespesa.GerarMovimentoDoFinanceiro(despesa.Codigo);
                    }
                    else if (despesa != null)
                    {
                        svcDespesa.DeletarMovimentoDoFinanceiro(despesa.Codigo);
                        repDespesaAcertoViagem.Deletar(despesa);
                    }
                }
                acertoDeViagem.TotalDespesasPagasMotoristas = valorPagoMotorista;
            }
        }

        private void SalvarAbastecimentos(Dominio.Entidades.AcertoDeViagem acertoDeViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Abastecimentos"]))
            {
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unidadeDeTrabalho);

                Servicos.Abastecimento svcAbastecimento = new Servicos.Abastecimento(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.AbastecimentoAcertoDeViagem> abastecimentos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.AbastecimentoAcertoDeViagem>>(Request.Params["Abastecimentos"]);

                for (var i = 0; i < abastecimentos.Count; i++)
                {
                    Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigoEAcertoDeViagem(abastecimentos[i].Codigo, acertoDeViagem.Codigo);

                    if (!abastecimentos[i].Excluir)
                    {
                        if (abastecimento == null)
                            abastecimento = new Dominio.Entidades.Abastecimento();
                        else
                            abastecimento.Initialize();

                        abastecimento.AcertoDeViagem = acertoDeViagem;

                        DateTime data;
                        DateTime.TryParseExact(abastecimentos[i].Data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                        if (data != DateTime.MinValue)
                            abastecimento.Data = data;
                        else
                            abastecimento.Data = null;

                        abastecimento.Kilometragem = abastecimentos[i].KMFinal;
                        abastecimento.KilometragemAnterior = abastecimentos[i].KMInicial;
                        abastecimento.Litros = decimal.Parse(abastecimentos[i].Litros);
                        abastecimento.Media = decimal.Parse(abastecimentos[i].Media);
                        abastecimento.Motorista = acertoDeViagem.Motorista;
                        abastecimento.Situacao = acertoDeViagem.Situacao == "F" ? "F" : "A";
                        abastecimento.DataAlteracao = DateTime.Now;
                        abastecimento.Status = acertoDeViagem.Status;
                        abastecimento.ValorUnitario = decimal.Parse(abastecimentos[i].ValorUnitario);
                        abastecimento.Veiculo = acertoDeViagem.Veiculo;
                        abastecimento.Empresa = acertoDeViagem.Empresa;
                        abastecimento.Pago = abastecimentos[i].Pago;                        

                        if (string.IsNullOrWhiteSpace(abastecimentos[i].CodigoPosto))
                        {
                            abastecimento.Posto = null;
                            abastecimento.NomePosto = abastecimentos[i].DescricaoPosto;
                        }
                        else
                        {
                            double cpfCnpj = 0;
                            double.TryParse(Utilidades.String.OnlyNumbers(abastecimentos[i].CodigoPosto), out cpfCnpj);

                            abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
                            abastecimento.NomePosto = string.Empty;
                        }

                        if (abastecimento.Veiculo != null && abastecimento.Veiculo.KilometragemAtual < abastecimentos[i].KMFinal && abastecimento.Situacao == "F")
                        {
                            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                            abastecimento.Veiculo.KilometragemAtual = int.Parse(abastecimentos[i].KMFinal.ToString());

                            repVeiculo.Atualizar(abastecimento.Veiculo, Auditado, null, "Atualizada a Quilometragem Atual do Veículo via Abastecimentos em Acerto de Viagem");
                        }

                        if (abastecimento.Codigo > 0)
                            repAbastecimento.Atualizar(abastecimento, Auditado);
                        else
                            repAbastecimento.Inserir(abastecimento, Auditado);

                        if (abastecimento.Empresa.Configuracao?.AcertoViagemContaDespesasAbastecimentos == null)
                            svcAbastecimento.GerarMovimentoDoFinanceiro(abastecimento.Codigo);
                    }
                    else if (abastecimento != null)
                    {
                        abastecimento.Initialize();

                        svcAbastecimento.DeletarMovimentoDoFinanceiro(abastecimento.Codigo);
                        repAbastecimento.Deletar(abastecimento, Auditado);
                    }
                }
            }
        }

        private void SalvarVales(ref Dominio.Entidades.AcertoDeViagem acertoDeViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Vales"]))
            {
                Repositorio.ValeDoAcertoDeViagem repValeAcertoViagem = new Repositorio.ValeDoAcertoDeViagem(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.ValeAcertoDeViagem> vales = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ValeAcertoDeViagem>>(Request.Params["Vales"]);

                if (vales != null)
                {
                    for (var i = 0; i < vales.Count; i++)
                    {
                        Dominio.Entidades.ValeDoAcertoDeViagem vale = repValeAcertoViagem.BuscarPorCodigoEAcertoDeViagem(vales[i].Codigo, acertoDeViagem.Codigo);

                        if (!vales[i].Excluir)
                        {
                            if (vale == null)
                                vale = new Dominio.Entidades.ValeDoAcertoDeViagem();

                            vale.AcertoDeViagem = acertoDeViagem;

                            DateTime data;
                            DateTime.TryParseExact(vales[i].Data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                            vale.Data = data;
                            vale.Descricao = vales[i].Descricao;
                            vale.Observacao = vales[i].Observacao;
                            vale.Numero = vales[i].Numero;
                            vale.Tipo = vales[i].Tipo;
                            vale.Valor = decimal.Parse(vales[i].Valor);

                            acertoDeViagem.TotalVales += vale.Valor;

                            if (vale.Codigo > 0)
                                repValeAcertoViagem.Atualizar(vale);
                            else
                                repValeAcertoViagem.Inserir(vale);
                        }
                        else if (vale != null)
                        {
                            repValeAcertoViagem.Deletar(vale);
                        }
                    }
                }
            }
        }

        private string CaminhoArquivo()
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"], "Anexos", "Acerto");
        }
        private dynamic RetornaDynAnexo(Dominio.Entidades.AcertoDeViagemAnexos anexo)
        {
            return new
            {
                Codigo = anexo.Codigo,
                Nome = anexo.NomeArquivo,
                Acerto = anexo.AcertoDeViagem.Codigo
            };
        }

        #endregion
    }
}
