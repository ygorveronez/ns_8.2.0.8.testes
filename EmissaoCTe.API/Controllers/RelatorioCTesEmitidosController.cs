using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioCTesEmitidosController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros, numeroInicial, numeroFinal, codigoSerie, codigoLocalidadeInicio, codigoLocalidadeFim, codigoUsuario, numeroCarga, numeroUnidade, fimRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["fimRegistros"], out fimRegistros);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out codigoSerie);
                int.TryParse(Request.Params["CodigoLocalidadeInicio"], out codigoLocalidadeInicio);
                int.TryParse(Request.Params["CodigoLocalidadeFim"], out codigoLocalidadeFim);
                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);
                int.TryParse(Request.Params["NumeroCarga"], out numeroCarga);
                int.TryParse(Request.Params["NumeroUnidade"], out numeroUnidade);

                if (fimRegistros == 0)
                    fimRegistros = 20;

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string cpfCnpjExpedidor = Utilidades.String.OnlyNumbers(Request.Params["Expedidor"]);
                string cpfCnpjRecebedor = Utilidades.String.OnlyNumbers(Request.Params["Recebedor"]);
                string cpfCnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);
                string duplicata = Utilidades.String.OnlyNumbers(Request.Params["Duplicata"]);
                string observacao = Request.Params["Observacao"];
                string placa = Request.Params["Veiculo"];
                string status = Request.Params["Status"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                string numeroNotaFiscal = Request.Params["NumeroNotaFiscal"];
                string ufInicio = Request.Params["UfInicio"];
                string ufFim = Request.Params["UfFim"];
                string icmsCTe = Request.Params["ICMSCTe"];
                string cstCTe = Request.Params["CSTCTe"];
                string nomeUsuario = Request.Params["NomeUsuario"];

                DateTime dataEmissaoInicial, dataEmissaoFinal, dataAutorizacaoInicial, dataAutorizacaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);


                bool importacao = false, exportacao = false, raizCNPJRemetente = false, raizCNPJExpedidor = false, raizCNPJRecebedor = false, raizCNPJDestinatario = false, raizCNPJTomador = false,
                    removerCliente = false;
                bool.TryParse(Request.Params["Importacao"], out importacao);
                bool.TryParse(Request.Params["Exportacao"], out exportacao);
                bool.TryParse(Request.Params["RaizCNPJRemetente"], out raizCNPJRemetente);
                bool.TryParse(Request.Params["RaizCNPJExpedidor"], out raizCNPJExpedidor);
                bool.TryParse(Request.Params["RaizCNPJRecebedor"], out raizCNPJRecebedor);
                bool.TryParse(Request.Params["RaizCNPJDestinatario"], out raizCNPJDestinatario);
                bool.TryParse(Request.Params["RaizCNPJTomador"], out raizCNPJTomador);
                bool.TryParse(Request.Params["RemoverCliente"], out removerCliente);

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out tipoCTe))
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Dominio.Enumeradores.StatusDuplicata? statusPagamento = null;
                Dominio.Enumeradores.StatusDuplicata statusPagamentoAux;
                if (Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["StatusPagamento"], out statusPagamentoAux))
                    statusPagamento = statusPagamentoAux;

                Dominio.Enumeradores.TipoServico? tipoServico = null;
                Dominio.Enumeradores.TipoServico tipoServicoAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["Servico"], out tipoServicoAux))
                    tipoServico = tipoServicoAux;

                Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe = null;
                if (Enum.TryParse(Request.Params["AverbacaoCTe"], out Dominio.Enumeradores.FiltroAverbacaoCTe averbacaoCTeAux))
                    averbacaoCTe = averbacaoCTeAux;

                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, codigoSerie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, placa, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, inicioRegistros, fimRegistros, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade, averbacaoCTe);
                int countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, codigoSerie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, placa, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade, averbacaoCTe);

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   cte.Codigo,
                                   PrazoCancelamento = string.Concat((cte.Status.Equals("A") && cte.DataRetornoSefaz != null && cte.DataRetornoSefaz.HasValue ? (168 - (DateTime.Now - cte.DataRetornoSefaz.Value).TotalHours) > 0 ? (168 - (DateTime.Now - cte.DataRetornoSefaz.Value).TotalHours) : 0 : 0).ToString("n0"), "h"),
                                   cte.Numero,
                                   Serie = cte.Serie.Numero,
                                   DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                   DataRetornoSefaz = cte.DataRetornoSefaz != null && cte.DataRetornoSefaz.HasValue ? cte.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                   cte.DescricaoTipoServico,
                                   cte.DescricaoTipoCTE,
                                   Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais != null ? cte.Remetente.Pais.Nome : "EXPORTACAO") : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                                   Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais != null ? cte.Destinatario.Pais.Nome : "EXPORTACAO") : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : string.Empty,
                                   Valor = string.Format("{0:n2}", cte.ValorFrete),
                                   cte.DescricaoStatus
                               }).ToList();

                unitOfWork.CommitChanges();

                return Json(retorno, true, null, new string[] { "Codigo", "Canc.|5", "Núm.|5", "Série|4", "Emissão|6", "Transmissão|6", "Tipo|6", "Finalid.|6", "Remetente|12", "Loc. Remet.|9", "Destinatário|12", "Loc. Destin.|9", "Valor|5", "Status|8" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataEmissaoInicial, dataEmissaoFinal, dataAutorizacaoInicial, dataAutorizacaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);

                Dominio.Enumeradores.TipoCTE tipoCTe;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out tipoCTe))
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Dominio.Enumeradores.StatusDuplicata? statusPagamento = null;
                Dominio.Enumeradores.StatusDuplicata statusPagamentoAux;
                if (Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["StatusPagamento"], out statusPagamentoAux))
                    statusPagamento = statusPagamentoAux;

                Dominio.Enumeradores.TipoServico? tipoServico = null;
                Dominio.Enumeradores.TipoServico tipoServicoAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["Servico"], out tipoServicoAux))
                    tipoServico = tipoServicoAux;

                int serie, numeroInicial, numeroFinal, codigoLocalidadeInicio, codigoLocalidadeFim, codigoUsuario, numeroCarga, numeroUnidade = -1;
                int.TryParse(Request.Params["Serie"], out serie);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["CodigoLocalidadeInicio"], out codigoLocalidadeInicio);
                int.TryParse(Request.Params["CodigoLocalidadeFim"], out codigoLocalidadeFim);
                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);
                int.TryParse(Request.Params["NumeroCarga"], out numeroCarga);
                int.TryParse(Request.Params["NumeroUnidade"], out numeroUnidade);

                bool somenteTracao = false, exibirNotasFiscais = false, importacao = false, exportacao = false, raizCNPJRemetente = false, raizCNPJExpedidor = false, raizCNPJRecebedor = false,
                    raizCNPJDestinatario = false, raizCNPJTomador = false, removerCliente = false;
                bool.TryParse(Request.Params["SomenteTracao"], out somenteTracao);
                bool.TryParse(Request.Params["ExibirNotasFiscais"], out exibirNotasFiscais);
                bool.TryParse(Request.Params["Importacao"], out importacao);
                bool.TryParse(Request.Params["Exportacao"], out exportacao);
                bool.TryParse(Request.Params["RaizCNPJRemetente"], out raizCNPJRemetente);
                bool.TryParse(Request.Params["RaizCNPJExpedidor"], out raizCNPJExpedidor);
                bool.TryParse(Request.Params["RaizCNPJRecebedor"], out raizCNPJRecebedor);
                bool.TryParse(Request.Params["RaizCNPJDestinatario"], out raizCNPJDestinatario);
                bool.TryParse(Request.Params["RaizCNPJTomador"], out raizCNPJTomador);
                bool.TryParse(Request.Params["RemoverCliente"], out removerCliente);

                string tipoArquivo = Request.Params["TipoArquivo"];
                string numeroNotaFiscal = Request.Params["NumeroNotaFiscal"];
                string veiculo = Request.Params["Veiculo"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string relatorio = Request.Params["Relatorio"];
                string status = Request.Params["Status"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjExpedidor = Utilidades.String.OnlyNumbers(Request.Params["Expedidor"]);
                string cpfCnpjRecebedor = Utilidades.String.OnlyNumbers(Request.Params["Recebedor"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string cpfCnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);
                string ufInicio = Request.Params["UfInicio"];
                string ufFim = Request.Params["UfFim"];
                string duplicata = Utilidades.String.OnlyNumbers(Request.Params["Duplicata"]);
                string icmsCTe = Request.Params["ICMSCTe"];
                string cstCTe = Request.Params["CSTCTe"];
                string nomeUsuario = Request.Params["NomeUsuario"];
                string observacao = Request.Params["Observacao"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                dynamic listaObjetos = null;
                List<ReportParameter> parametros = new List<ReportParameter>();

                if (relatorio.Contains("RelatorioCTesEmitidosComponentes2") || relatorio.Contains("RelatorioCTesEmitidosComponentes3") || relatorio.Contains("RelatorioCTesEmitidosComponentes4"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoComponentes(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, relatorio.Contains("RelatorioCTesEmitidosComponentes2") ? false : true, observacao, removerCliente, numeroCarga, numeroUnidade);

                    Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);

                    List<Dominio.Entidades.VeiculoCTE> veiculosCTe = repVeiculoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos>)listaObjetos select obj.CODIGO).ToArray());

                    for (var i = 0; i < listaObjetos.Count; i++)
                    {
                        string placas = string.Join("", (from obj in veiculosCTe where obj.CTE.Codigo == listaObjetos[i].CODIGO && obj.TipoVeiculo == "1" select obj.Placa));

                        if (!string.IsNullOrWhiteSpace(placas))
                            listaObjetos[i].PLACAS_ADICIONAIS += placas;
                    }
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosComponentes"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoComponentes(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, true, observacao, removerCliente, numeroCarga, numeroUnidade);

                    Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);

                    List<Dominio.Entidades.VeiculoCTE> veiculosCTe = repVeiculoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos>)listaObjetos select obj.CODIGO).ToArray());

                    for (var i = 0; i < listaObjetos.Count; i++)
                    {
                        string placas = string.Join("", (from obj in veiculosCTe where obj.CTE.Codigo == listaObjetos[i].CODIGO select obj.Placa));

                        if (!string.IsNullOrWhiteSpace(placas))
                            listaObjetos[i].PLACAS_ADICIONAIS += placas;
                    }
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosPorVeiculo"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoPorVeiculo(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade);

                    Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);

                    List<Dominio.Entidades.VeiculoCTE> veiculosCTe = repVeiculoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos>)listaObjetos select obj.CODIGO).ToArray());

                    for (var i = 0; i < listaObjetos.Count; i++)
                    {
                        string placas = string.Join("", (from obj in veiculosCTe where obj.CTE.Codigo == listaObjetos[i].CODIGO && !obj.Veiculo.Placa.Equals(listaObjetos[i].PLACA) select obj.Placa));

                        if (!string.IsNullOrWhiteSpace(placas))
                            listaObjetos[i].PLACAS_ADICIONAIS += placas;
                    }
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosSuamrizadoPorVeiculo"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoPorVeiculo(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade);

                    Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);
                    Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);

                    List<Dominio.Entidades.VeiculoCTE> veiculosCTe = repVeiculoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos>)listaObjetos select obj.CODIGO).Distinct().ToArray());
                    List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTE.BuscarPorCTes((from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos>)listaObjetos select obj.CODIGO).Distinct().ToArray());

                    for (var i = 0; i < listaObjetos.Count; i++)
                    {
                        var veiculoCTeDoCTe = (from obj in veiculosCTe
                                               where obj.CTE.Codigo == listaObjetos[i].CODIGO
                                               select obj);

                        string placas = string.Join("", veiculoCTeDoCTe.Where(obj => !obj.Veiculo.Placa.Equals(listaObjetos[i].PLACA)).Select(obj => obj.Placa));
                        //string placas = string.Join("", (from obj in veiculosCTe where obj.CTE.Codigo == listaObjetos[i].CODIGO && !obj.Veiculo.Placa.Equals(listaObjetos[i].PLACA) select obj.Placa));

                        if (!string.IsNullOrWhiteSpace(placas))
                            listaObjetos[i].PLACAS_ADICIONAIS += placas;

                        var motorista = veiculoCTeDoCTe.FirstOrDefault();
                        if (motorista != null)
                            listaObjetos[i].MOTORISTA = motorista.Motorista;

                        listaObjetos[i].NUMERO_DOCUMENTO = string.Join("", (from d in documentosCTe where d.CTE.Codigo == listaObjetos[i].CODIGO select !string.IsNullOrWhiteSpace(d.Serie) ? d.Numero + "/" + d.Serie : !string.IsNullOrWhiteSpace(d.ChaveNFE) ? d.Numero + "/" + d.ChaveNFE.Substring(22, 3) : d.Numero));
                    }
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosPorDocumento") || relatorio.Contains("RelatorioCTesEmitidosPorDocumentoPaisagem") || relatorio.Contains("RelatorioCTesEmitidosUnidades"))
                {
                    listaObjetos = repCTe.RelatorioSumarizadoPorDocumento(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade);
                    parametros.Add(new ReportParameter("PesoTotalCTes", (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento>)listaObjetos group obj by new { obj.CodigoCTe, obj.PesoCTe } into ctes select ctes.Key.PesoCTe).Sum().ToString("n2")));
                    parametros.Add(new ReportParameter("ValorTotalCTes", (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento>)listaObjetos group obj by new { obj.CodigoCTe, obj.ValorAReceber } into ctes select ctes.Key.ValorAReceber).Sum().ToString("n2")));
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosObsFiscoContribuinte"))
                {
                    listaObjetos = repCTe.RelatorioSumarizadoPorDocumentoObsFisco(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade);
                    parametros.Add(new ReportParameter("PesoTotalCTes", (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento>)listaObjetos group obj by new { obj.CodigoCTe, obj.PesoCTe } into ctes select ctes.Key.PesoCTe).Sum().ToString("n2")));
                    parametros.Add(new ReportParameter("ValorTotalCTes", (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesSumarizadoPorDocumento>)listaObjetos group obj by new { obj.CodigoCTe, obj.ValorAReceber } into ctes select ctes.Key.ValorAReceber).Sum().ToString("n2")));
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosPorDestinatário"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoPorDestinatario(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade);
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosPorTomador"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoPorTomador(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, dataAutorizacaoInicial, dataAutorizacaoFinal, observacao, removerCliente, numeroCarga, numeroUnidade);
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosFatura"))
                {
                    listaObjetos = repCTe.RelatorioCTesEmitidosFatura(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, dataAutorizacaoInicial, dataAutorizacaoFinal, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, "", observacao, removerCliente, numeroCarga, numeroUnidade);
                    tipoArquivo = "Excel";
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosCompletoComMDFe"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoComMDFe(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, dataAutorizacaoInicial, dataAutorizacaoFinal, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, "", observacao, removerCliente, numeroCarga, numeroUnidade);
                    tipoArquivo = "Excel";
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosCompletoComExpedidor"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoComExpedidor(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, dataAutorizacaoInicial, dataAutorizacaoFinal, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, "", observacao, removerCliente, numeroCarga, numeroUnidade);
                    tipoArquivo = "Excel";
                }
                else if (relatorio.Contains("RelatorioCTesEmitidosDuplicata"))
                {
                    listaObjetos = repCTe.RelatorioEmissaoDuplicata(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, dataAutorizacaoInicial, dataAutorizacaoFinal, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, "", observacao, removerCliente, numeroCarga, numeroUnidade);
                }
                else if (relatorio.Contains("RelatorioContratoYamaha"))
                {
                    listaObjetos = repCTe.RelatorioContratoYamaha(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, dataAutorizacaoInicial, dataAutorizacaoFinal, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, "", observacao, removerCliente, numeroCarga, numeroUnidade);
                    tipoArquivo = "Excel";

                    Repositorio.SubcontratacaoDocumentos repSubcontratacaoDocumentos = new Repositorio.SubcontratacaoDocumentos(unitOfWork);

                    for (var i = 0; i < listaObjetos.Count; i++)
                    {

                        List<Dominio.Entidades.SubcontratacaoDocumentos> documentos = repSubcontratacaoDocumentos.BuscarPorCodigo(listaObjetos[i].Codigo);

                        string chaves = string.Join(", ", (from obj in documentos where obj.Codigo == listaObjetos[i].Codigo select obj.Documento.Chave));

                        if (!string.IsNullOrWhiteSpace(chaves))
                            listaObjetos[i].ChaveCTeAnterior += chaves;
                    }

                }
                else
                {
                    listaObjetos = repCTe.RelatorioEmissao(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, serie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, veiculo, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, ufInicio, ufFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, dataAutorizacaoInicial, dataAutorizacaoFinal, icmsCTe, tipoServico, cstCTe, codigoUsuario, nomeUsuario, "", observacao, removerCliente, numeroCarga, numeroUnidade);
                }

                if (relatorio.Contains("RelatorioCTesEmitidosCompleto") && exibirNotasFiscais)
                {
                    for (var i = 0; i < listaObjetos.Count; i++)
                    {
                        Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);

                        List<Dominio.Entidades.DocumentosCTE> documentos = repDocumentoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, (from obj in (List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidos>)listaObjetos select obj.CODIGO).ToArray());

                        string numeros = string.Join(", ", (from obj in documentos where obj.CTE.Codigo == listaObjetos[i].CODIGO select obj.Numero));

                        if (!string.IsNullOrWhiteSpace(numeros))
                            listaObjetos[i].NOTAS_FISCAIS += numeros;
                    }
                }

                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.CNPJ_Formatado + " " + this.EmpresaUsuario.RazaoSocial));
                if (dataAutorizacaoInicial > DateTime.MinValue && dataAutorizacaoFinal > DateTime.MinValue)
                    parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataAutorizacaoInicial.ToString("dd/MM/yyyy"), " até ", dataAutorizacaoFinal.ToString("dd/MM/yyyy"))));
                else if (dataAutorizacaoInicial > DateTime.MinValue && dataAutorizacaoFinal == DateTime.MinValue)
                    parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataAutorizacaoInicial.ToString("dd/MM/yyyy"), " até ", DateTime.MaxValue.ToString("dd/MM/yyyy"))));
                else if (dataAutorizacaoInicial == DateTime.MinValue && dataAutorizacaoFinal > DateTime.MinValue)
                    parametros.Add(new ReportParameter("Periodo", string.Concat("De ", DateTime.MinValue.ToString("dd/MM/yyyy"), " até ", dataAutorizacaoFinal.ToString("dd/MM/yyyy"))));
                else
                    parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataEmissaoInicial.ToString("dd/MM/yyyy"), " até ", dataEmissaoFinal.ToString("dd/MM/yyyy"))));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("CTesEmitidos", listaObjetos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/" + relatorio + ".rdlc", tipoArquivo, parametros, dataSources, (object sender, SubreportProcessingEventArgs e) =>
                {
                    if (e.ReportPath.Contains("RelatorioCTesEmitidosCompleto_Veiculos"))
                    {
                        Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);
                        List<Dominio.Entidades.VeiculoCTE> listaVeiculos = repVeiculoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, int.Parse(e.Parameters["CodigoCTe"].Values[0]), somenteTracao ? "0" : string.Empty);
                        e.DataSources.Add(new ReportDataSource("VeiculosCTe", listaVeiculos));
                    }
                    else if (e.ReportPath.Contains("RelatorioCTesEmitidosPorVeiculo_Componentes"))
                    {
                        Repositorio.ComponentePrestacaoCTE repComponenteCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
                        List<Dominio.Entidades.ComponentePrestacaoCTE> listaComponentes = repComponenteCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, int.Parse(e.Parameters["CodigoCTe"].Values[0]));
                        e.DataSources.Add(new ReportDataSource("ComponentesCTe", listaComponentes));
                    }
                    else if (e.ReportPath.Contains("RelatorioCTesEmitidosComponentes_Componentes"))
                    {
                        Repositorio.ComponentePrestacaoCTE repComponenteCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
                        List<Dominio.Entidades.ComponentePrestacaoCTE> listaComponentes = repComponenteCTe.BuscarInformadosPorCTe(this.EmpresaUsuario.Codigo, int.Parse(e.Parameters["CodigoCTe"].Values[0]));
                        e.DataSources.Add(new ReportDataSource("ComponentesCTe", listaComponentes));
                    }
                });

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, relatorio + "." + arquivo.FileNameExtension);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadLoteXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros, numeroInicial, numeroFinal, codigoSerie, codigoLocalidadeInicio, codigoLocalidadeFim, codigoUsuario, numeroCarga, numeroUnidade = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out codigoSerie);
                int.TryParse(Request.Params["CodigoLocalidadeInicio"], out codigoLocalidadeInicio);
                int.TryParse(Request.Params["CodigoLocalidadeFim"], out codigoLocalidadeFim);
                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);
                int.TryParse(Request.Params["NumeroCarga"], out numeroCarga);
                int.TryParse(Request.Params["NumeroUnidade"], out numeroUnidade);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string cpfCnpjExpedidor = Utilidades.String.OnlyNumbers(Request.Params["Expedidor"]);
                string cpfCnpjRecebedor = Utilidades.String.OnlyNumbers(Request.Params["Recebedor"]);
                string cpfCnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);
                string duplicata = Utilidades.String.OnlyNumbers(Request.Params["Duplicata"]);
                string observacao = Request.Params["Observacao"];
                string placa = Request.Params["Veiculo"];
                string status = Request.Params["Status"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                string numeroNotaFiscal = Request.Params["NumeroNotaFiscal"];
                string icmsCTe = Request.Params["ICMSCTe"];
                string cstCTe = Request.Params["CSTCTe"];
                string ufInicio = Request.Params["UfInicio"];
                string ufFim = Request.Params["UfFim"];
                string nomeUsuario = Request.Params["NomeUsuario"];

                DateTime dataEmissaoInicial, dataEmissaoFinal, dataAutorizacaoInicial, dataAutorizacaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);

                bool importacao = false, exportacao = false, raizCNPJRemetente = false, raizCNPJExpedidor = false, raizCNPJRecebedor = false, raizCNPJDestinatario = false, raizCNPJTomador = false, removerCliente = false;
                bool.TryParse(Request.Params["Importacao"], out importacao);
                bool.TryParse(Request.Params["Exportacao"], out exportacao);
                bool.TryParse(Request.Params["RaizCNPJRemetente"], out raizCNPJRemetente);
                bool.TryParse(Request.Params["RaizCNPJExpedidor"], out raizCNPJExpedidor);
                bool.TryParse(Request.Params["RaizCNPJRecebedor"], out raizCNPJRecebedor);
                bool.TryParse(Request.Params["RaizCNPJDestinatario"], out raizCNPJDestinatario);
                bool.TryParse(Request.Params["RaizCNPJTomador"], out raizCNPJTomador);
                bool.TryParse(Request.Params["RemoverCliente"], out removerCliente);

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out tipoCTe))
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Dominio.Enumeradores.StatusDuplicata? statusPagamento = null;
                Dominio.Enumeradores.StatusDuplicata statusPagamentoAux;
                if (Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["StatusPagamento"], out statusPagamentoAux))
                    statusPagamento = statusPagamentoAux;

                Dominio.Enumeradores.TipoServico? tipoServico = null;
                Dominio.Enumeradores.TipoServico tipoServicoAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["Servico"], out tipoServicoAux))
                    tipoServico = tipoServicoAux;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<int> listaCodigosCtes = repCTe.BuscarListaCodigosCTEs(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, codigoSerie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, placa, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, cstCTe, dataAutorizacaoInicial, dataAutorizacaoFinal, ufInicio, ufFim, nomeUsuario, codigoUsuario, observacao, removerCliente, tipoServico, numeroCarga, numeroUnidade);

                int quantidadeDownloadLoteCTe = 0;
                int.TryParse(ConfigurationManager.AppSettings["QuantidadeDownloadLoteCTe"], out quantidadeDownloadLoteCTe);
                if (quantidadeDownloadLoteCTe == 0)
                    quantidadeDownloadLoteCTe = 500;

                if (listaCodigosCtes.Count > quantidadeDownloadLoteCTe)
                    return Json<bool>(false, false, string.Concat("Quantidade de conhecimentos para geração de lote inválida (", listaCodigosCtes.Count, "). É permitido o download de um lote de no máximo " + quantidadeDownloadLoteCTe.ToString() + " conhecimentos de transporte."));

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                return Arquivo(svcCTe.ObterLoteDeXML(listaCodigosCtes, this.EmpresaUsuario.Codigo), "application/zip", "LoteXML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadLoteDACTE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros, numeroInicial, numeroFinal, codigoSerie, codigoLocalidadeInicio, codigoLocalidadeFim, codigoUsuario, numeroCarga, numeroUnidade = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out codigoSerie);
                int.TryParse(Request.Params["CodigoLocalidadeInicio"], out codigoLocalidadeInicio);
                int.TryParse(Request.Params["CodigoLocalidadeFim"], out codigoLocalidadeFim);
                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);
                int.TryParse(Request.Params["NumeroCarga"], out numeroCarga);
                int.TryParse(Request.Params["NumeroUnidade"], out numeroUnidade);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string cpfCnpjExpedidor = Utilidades.String.OnlyNumbers(Request.Params["Expedidor"]);
                string cpfCnpjRecebedor = Utilidades.String.OnlyNumbers(Request.Params["Recebedor"]);
                string cpfCnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);
                string duplicata = Utilidades.String.OnlyNumbers(Request.Params["Duplicata"]);
                string observacao = Request.Params["Observacao"];

                DateTime dataEmissaoInicial, dataEmissaoFinal, dataAutorizacaoInicial, dataAutorizacaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
                DateTime.TryParseExact(Request.Params["DataAutorizacaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);

                bool importacao = false, exportacao = false, raizCNPJRemetente = false, raizCNPJExpedidor = false, raizCNPJRecebedor = false, raizCNPJDestinatario = false, raizCNPJTomador = false, removerCliente = false;
                bool.TryParse(Request.Params["Importacao"], out importacao);
                bool.TryParse(Request.Params["Exportacao"], out exportacao);
                bool.TryParse(Request.Params["RaizCNPJRemetente"], out raizCNPJRemetente);
                bool.TryParse(Request.Params["RaizCNPJExpedidor"], out raizCNPJExpedidor);
                bool.TryParse(Request.Params["RaizCNPJRecebedor"], out raizCNPJRecebedor);
                bool.TryParse(Request.Params["RaizCNPJDestinatario"], out raizCNPJDestinatario);
                bool.TryParse(Request.Params["RaizCNPJTomador"], out raizCNPJTomador);
                bool.TryParse(Request.Params["RemoverCliente"], out removerCliente);

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out tipoCTe))
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Dominio.Enumeradores.StatusDuplicata? statusPagamento = null;
                Dominio.Enumeradores.StatusDuplicata statusPagamentoAux;
                if (Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["StatusPagamento"], out statusPagamentoAux))
                    statusPagamento = statusPagamentoAux;

                Dominio.Enumeradores.TipoServico? tipoServico = null;
                Dominio.Enumeradores.TipoServico tipoServicoAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["Servico"], out tipoServicoAux))
                    tipoServico = tipoServicoAux;

                string placa = Request.Params["Veiculo"];
                string status = Request.Params["Status"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                string numeroNotaFiscal = Request.Params["NumeroNotaFiscal"];
                string icmsCTe = Request.Params["ICMSCTe"];
                string cstCTe = Request.Params["CSTCTe"];

                string ufInicio = Request.Params["UfInicio"];
                string ufFim = Request.Params["UfFim"];
                string nomeUsuario = Request.Params["NomeUsuario"];

                int diasDownloadDacte = 0;
                int.TryParse(ConfigurationManager.AppSettings["DiasDownloadDacte"], out diasDownloadDacte);

                if (diasDownloadDacte == 0)
                    diasDownloadDacte = 120;

                if (dataEmissaoInicial.Date < DateTime.Now.AddDays(-diasDownloadDacte).Date)
                    return Json<bool>(false, false, "A data de emissão inicial não deve ser menor do que " + DateTime.Now.AddDays(-diasDownloadDacte).ToString("dd/MM/yyyy") + " para realizar o download do lote de DACTEs.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<string> listaChavesCTes = repCTe.BuscarListaChavesCTes(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, status, tipoCTe, codigoSerie, cpfCnpjRemetente, cpfCnpjExpedidor, cpfCnpjRecebedor, cpfCnpjDestinatario, cpfCnpjTomador, numeroInicial, numeroFinal, nomeMotorista, cpfMotorista, placa, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Status.Equals("A")).Select(o => o.Codigo).ToArray(), tipoOcorrencia, numeroNotaFiscal, statusPagamento, codigoLocalidadeInicio, codigoLocalidadeFim, duplicata, importacao, exportacao, raizCNPJRemetente, raizCNPJExpedidor, raizCNPJRecebedor, raizCNPJDestinatario, raizCNPJTomador, icmsCTe, cstCTe, dataAutorizacaoInicial, dataAutorizacaoFinal, ufInicio, ufFim, nomeUsuario, codigoUsuario, observacao, removerCliente, tipoServico, numeroCarga, numeroUnidade);

                if (listaChavesCTes.Count <= 0)
                    return Json<bool>(false, false, "Nenhuma DACTE encontrada para o período selecionado.");

                int quantidadeDownloadLoteCTe = 0;
                int.TryParse(ConfigurationManager.AppSettings["QuantidadeDownloadLoteCTe"], out quantidadeDownloadLoteCTe);

                if (quantidadeDownloadLoteCTe > 0 && listaChavesCTes.Count > quantidadeDownloadLoteCTe)
                    return Json<bool>(false, false, "Somente é possível baixar " + quantidadeDownloadLoteCTe.ToString() + " DACTEs por vez, verifique filtro selecionado.");

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                return Arquivo(svcCTe.ObterLoteDeDACTE(listaChavesCTes, this.EmpresaUsuario.Codigo, unitOfWork), "application/zip", "LoteDACTE.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelEmitidosEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoEmpresa = 0;
                int.TryParse(Request.Params["Empresa"], out codigoEmpresa);
                string cnpjEmbarcadorUsuario = this.Usuario.CNPJEmbarcador ?? string.Empty;

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
                DateTime.TryParseExact(Request.Params["DataInicialAutorizacao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialAutorizacao);
                DateTime.TryParseExact(Request.Params["DataFinalAutorizacao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinalAutorizacao);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CnpjEmbarcador"]), out double cnpjEmbarcador);

                bool.TryParse(Request.Params["CnpjRaiz"], out bool todosCNPJRaiz);

                string tipoCliente = Request.Params["TipoCliente"];
                string tipoEmissao = Request.Params["TipoEmissao"];
                string tipoRelatorio = Request.Params["TipoRelatorio"];

                string tipoArquivo = Request.Params["Arquivo"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                Dominio.Entidades.Cliente cliente = cnpjEmbarcador > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjEmbarcador) : null;
                IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> listaCTes = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador>();

                if (tipoRelatorio == "")
                    listaCTes = repCTe.RelatorioCTesEmitidosPorEmbarcador(this.EmpresaUsuario.Codigo, codigoEmpresa, cliente != null ? cliente.CPF_CNPJ_SemFormato : null, dataInicialAutorizacao, dataFinalAutorizacao, dataInicial, dataFinal, todosCNPJRaiz, cnpjEmbarcadorUsuario, tipoEmissao, tipoCliente);
                else if (tipoRelatorio == "Sum")
                    listaCTes = repCTe.RelatorioCTesEmitidosPorEmbarcadorSumarizado(this.EmpresaUsuario.Codigo, codigoEmpresa, cliente != null ? cliente.CPF_CNPJ_SemFormato : null, dataInicialAutorizacao, dataFinalAutorizacao, dataInicial, dataFinal, todosCNPJRaiz, cnpjEmbarcadorUsuario, tipoEmissao, tipoCliente);

                List<ReportParameter> parametros = new List<ReportParameter>();

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("CTes", listaCTes));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCTesEmitidosPorEmbarcador" + tipoRelatorio + ".rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, string.Concat("RelatorioCTesEmitidosPorEmbarcador", tipoRelatorio, '.', arquivo.FileNameExtension.ToLower()));

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


    }
}
