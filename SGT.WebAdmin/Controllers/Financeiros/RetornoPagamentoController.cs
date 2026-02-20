using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using System.IO;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { "DownloadRelatorioRetornoPagamento" }, "Financeiros/RetornoPagamento")]
    public class RetornoPagamentoController : BaseController
    {
		#region Construtores

		public RetornoPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarRetorno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoBoletoConfiguracao = 0;
                int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConfiguracao);
                List<int> codigosRetornos = new List<int>();

                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                unitOfWork.Start();
                if (files.Count > 0 && codigoBoletoConfiguracao > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                    Servicos.DTO.CustomFile file = files[0];
                    
                    string fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(Servicos.FS.GetPath("C:\\Arquivos\\RetornoPagamento\\" + fileName));

                    string[] lines = Utilidades.IO.FileStorageService.Storage.ReadLines(Servicos.FS.GetPath(@"C:\\Arquivos\\RetornoPagamento\\" + fileName)).ToArray();
                    
                    string msgRetorno = string.Empty;
                    switch (boletoConfiguracao.BoletoTipoCNAB)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB240:
                            codigosRetornos = ProcessarRetornoCNAB240(lines, boletoConfiguracao, unitOfWork, ref msgRetorno, TipoServicoMultisoftware, codigoEmpresa, this.Usuario, fileName);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB400:
                            codigosRetornos = ProcessarRetornoCNAB400(lines, boletoConfiguracao, unitOfWork, ref msgRetorno, TipoServicoMultisoftware, codigoEmpresa, this.Usuario, fileName);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB240PIX:
                            codigosRetornos = ProcessarRetornoCNAB240PIX(lines, boletoConfiguracao, unitOfWork, ref msgRetorno, TipoServicoMultisoftware, codigoEmpresa, this.Usuario, fileName);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB500PIX:
                            codigosRetornos = ProcessarRetornoCNAB500(lines, boletoConfiguracao, unitOfWork, ref msgRetorno, TipoServicoMultisoftware, codigoEmpresa, this.Usuario, fileName);
                            break;
                        default:
                            codigosRetornos = ProcessarRetornoCNAB400(lines, boletoConfiguracao, unitOfWork, ref msgRetorno, TipoServicoMultisoftware, codigoEmpresa, this.Usuario, fileName); //padrão que estava antes
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                        return new JsonpResult(false, msgRetorno);

                    unitOfWork.CommitChanges();
                    if (codigosRetornos != null && codigosRetornos.Count > 0)
                    {
                        var retornoOBJ = new { CodigosRetornos = string.Join(", ", codigosRetornos) };

                        return new JsonpResult(retornoOBJ, true, "Sucesso.");
                    }
                    else
                        return new JsonpResult(null, true, "Sucesso.");
                }
                else
                {
                    return new JsonpResult(false, "Arquivo não encontrado ou configuração do banco não selecionado, por favor verifique!");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao processar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadRelatorioRetornoPagamento()
        {
            try
            {
                string codigosRetornos = Request.Params("CodigosRetornos");
                if (string.IsNullOrWhiteSpace(codigosRetornos))
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var pdf = ReportRequest.WithType(ReportType.RetornoPagamento)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigosRetornos", codigosRetornos)
                    .CallReport()
                    .GetContentFile();
                
                // Retorna o arquivo
                return Arquivo(pdf, "application/pdf", "Retorno de Pagamento " + DateTime.Now.ToString("ddMMyyyy hhMMss") + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
        }

        #endregion

        #region Métodos Privados

        private List<int> ProcessarRetornoCNAB400(string[] lines, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, Repositorio.UnitOfWork unitOfWork, ref string msgErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, Dominio.Entidades.Usuario usuario, string fileName)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno repPagamentoEletronicoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno(unitOfWork);
            msgErro = string.Empty;

            List<int> codigosRetornos = new List<int>();
            string vCodigoBanco = boletoConfiguracao.NumeroBanco;
            string vDataArquivo = "";
            string vComando = "";
            string vNossoNumero = "";
            string vDataVencimento = "";
            string vDataLiquidacao = "";
            string vValorRetorno = "";
            string vJuros = "";
            string vOutros = "";
            string vTarifa = "";
            string vValorRecebido = "";
            string vDataCredito = "";
            string vCodigoRejeicao = "";
            string vDataBaixa = "";
            string vRegistrada = "";
            string vSomenteTarifa = "";
            string vSituacaoAgendamento = "";
            int codigoTitulo = 0;
            int codigoRetorno = 0;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno retorno = null;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno comandoRetorno = null;

            bool primeiraLinha = true;
            string msgRetorno = string.Empty;

            foreach (string line in lines)
            {
                if (primeiraLinha)
                {
                    vDataArquivo = line.Substring(94, 6);
                    if (boletoConfiguracao.NumeroBanco != line.Substring(76, 3))
                    {
                        unitOfWork.Rollback();
                        msgErro = "O arquivo selecionado não é do mesmo banco!";
                        return null;
                    }
                    primeiraLinha = false;
                }
                else
                {
                    if (line.Substring(0, 1) != "9")
                    {
                        ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                        CarregarConteudoCNAB400(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref vSituacaoAgendamento, ref codigoTitulo);

                        vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                        codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                        codigosRetornos.Add(codigoRetorno);
                        if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)
                        {
                            if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                            {
                                unitOfWork.Rollback();
                                msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                                return null;
                            }
                        }
                        if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                        {
                            if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                            {
                                unitOfWork.Rollback();
                                msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                                return null;
                            }
                        }
                    }
                }
            }

            return codigosRetornos;
        }

        private List<int> ProcessarRetornoCNAB500(string[] lines, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, Repositorio.UnitOfWork unitOfWork, ref string msgErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, Dominio.Entidades.Usuario usuario, string fileName)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno repPagamentoEletronicoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno(unitOfWork);
            msgErro = string.Empty;

            List<int> codigosRetornos = new List<int>();
            string vCodigoBanco = boletoConfiguracao.NumeroBanco;
            string vDataArquivo = "";
            string vComando = "";
            string vNossoNumero = "";
            string vDataVencimento = "";
            string vDataLiquidacao = "";
            string vValorRetorno = "";
            string vJuros = "";
            string vOutros = "";
            string vTarifa = "";
            string vValorRecebido = "";
            string vDataCredito = "";
            string vCodigoRejeicao = "";
            string vDataBaixa = "";
            string vRegistrada = "";
            string vSomenteTarifa = "";
            string vSituacaoAgendamento = "";
            int codigoTitulo = 0;
            int codigoRetorno = 0;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno retorno = null;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno comandoRetorno = null;

            bool primeiraLinha = true;
            string msgRetorno = string.Empty;

            foreach (string line in lines)
            {
                if (primeiraLinha)
                {
                    vDataArquivo = line.Substring(94, 6);
                    if (boletoConfiguracao.NumeroBanco != line.Substring(76, 3))
                    {
                        unitOfWork.Rollback();
                        msgErro = "O arquivo selecionado não é do mesmo banco!";
                        return null;
                    }
                    primeiraLinha = false;
                }
                else
                {
                    if (line.Substring(0, 1) != "9")
                    {
                        ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                        CarregarConteudoCNAB500(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref vSituacaoAgendamento, ref codigoTitulo);

                        vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                        codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                        codigosRetornos.Add(codigoRetorno);
                        if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)
                        {
                            if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                            {
                                unitOfWork.Rollback();
                                msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                                return null;
                            }
                        }
                        if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                        {
                            if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                            {
                                unitOfWork.Rollback();
                                msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                                return null;
                            }
                        }
                    }
                }
            }

            return codigosRetornos;
        }

        private List<int> ProcessarRetornoCNAB240(string[] lines, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, Repositorio.UnitOfWork unitOfWork, ref string msgErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, Dominio.Entidades.Usuario usuario, string fileName)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno repPagamentoEletronicoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno(unitOfWork);
            msgErro = string.Empty;

            List<int> codigosRetornos = new List<int>();
            string vCodigoBanco = boletoConfiguracao.NumeroBanco;
            string vDataArquivo = "";
            string vComando = "";
            string vNossoNumero = "";
            string vDataVencimento = "";
            string vDataLiquidacao = "";
            string vValorRetorno = "";
            string vJuros = "";
            string vOutros = "";
            string vTarifa = "";
            string vValorRecebido = "";
            string vDataCredito = "";
            string vCodigoRejeicao = "";
            string vDataBaixa = "";
            string vRegistrada = "";
            string vSomenteTarifa = "";
            string vSituacaoAgendamento = "";
            int codigoTitulo = 0;
            int codigoRetorno = 0;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno retorno = null;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno comandoRetorno = null;

            bool primeiraLinha = true;
            string msgRetorno = string.Empty;

            foreach (string line in lines)
            {
                if (primeiraLinha)
                {
                    vDataArquivo = line.Substring(143, 8);
                    if (boletoConfiguracao.NumeroBanco != line.Substring(0, 3))
                    {
                        unitOfWork.Rollback();
                        msgErro = "O arquivo selecionado não é do mesmo banco!";
                        return null;
                    }
                }
                primeiraLinha = false;
                if (line.Substring(13, 1) == "G")
                {
                    ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                    CarregarConteudoComandoGRetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa);

                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);
                    vComando = "G";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, 0, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);

                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)//(string.IsNullOrWhiteSpace(retorno.Agendamento.Trim()) || retorno.Agendamento.Trim() == "00"))
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }
                if (line.Substring(13, 1) == "N" && line.Substring(14, 3) == "000")
                {
                    ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                    CarregarConteudoComandoNRetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);

                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                    vComando = "N";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)//(string.IsNullOrWhiteSpace(retorno.Agendamento.Trim()) || retorno.Agendamento.Trim() == "00"))
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;

                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }
                if (line.Substring(13, 1) == "A")
                {
                    ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                    CarregarConteudoComandoARetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                }
                if (line.Substring(13, 1) == "B")
                {
                    CarregarConteudoComandoBRetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa);

                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                    vComando = "B";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)//(string.IsNullOrWhiteSpace(retorno.Agendamento.Trim()) || retorno.Agendamento.Trim() == "00"))
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }
                if (line.Substring(13, 1) == "Z" && line.Substring(14, 3) == "000")
                {
                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                    vComando = "Z";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)//(string.IsNullOrWhiteSpace(retorno.Agendamento.Trim()) || retorno.Agendamento.Trim() == "00"))
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }
                if (line.Substring(13, 1) == "J" && (line.Substring(14, 3) == "000" || line.Substring(14, 3) == "009"))
                {
                    ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                    CarregarConteudoComandoJRetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref vSituacaoAgendamento);

                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                    vComando = "J";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, 0, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)//(string.IsNullOrWhiteSpace(retorno.Agendamento.Trim()) || retorno.Agendamento.Trim() == "00"))
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }
                if (line.Substring(13, 1) == "O" && line.Substring(14, 3) == "000")
                {
                    ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                    CarregarConteudoComandoORetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref vSituacaoAgendamento, ref codigoTitulo);

                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                    vComando = "O";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)//(string.IsNullOrWhiteSpace(retorno.Agendamento.Trim()) || retorno.Agendamento.Trim() == "00"))
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }
                if (line.Substring(7, 1) == "3" && line.Substring(13, 1) == "T")
                {
                    ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                    CarregarConteudoComandoTRetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo);
                }
                if (line.Substring(7, 1) == "3" && line.Substring(13, 1) == "U" && line.Substring(210, 3) == "756")
                {
                    CarregarConteudoComandoURetorno(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref vSituacaoAgendamento);

                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                    vComando = "U";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)//(string.IsNullOrWhiteSpace(retorno.Agendamento.Trim()) || retorno.Agendamento.Trim() == "00"))
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }
                if (line.Substring(4, 5) == "99999")
                {
                    //fim
                }
            }

            return codigosRetornos;
        }

        private List<int> ProcessarRetornoCNAB240PIX(string[] lines, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, Repositorio.UnitOfWork unitOfWork, ref string msgErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, Dominio.Entidades.Usuario usuario, string fileName)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno repPagamentoEletronicoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno(unitOfWork);
            msgErro = string.Empty;

            List<int> codigosRetornos = new List<int>();
            string vCodigoBanco = boletoConfiguracao.NumeroBanco;
            string vDataArquivo = "";
            string vComando = "";
            string vNossoNumero = "";
            string vDataVencimento = "";
            string vDataLiquidacao = "";
            string vValorRetorno = "";
            string vJuros = "";
            string vOutros = "";
            string vTarifa = "";
            string vValorRecebido = "";
            string vDataCredito = "";
            string vCodigoRejeicao = "";
            string vDataBaixa = "";
            string vRegistrada = "";
            string vSomenteTarifa = "";
            string vSituacaoAgendamento = "";
            int codigoTitulo = 0;
            int codigoRetorno = 0;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno retorno = null;
            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno comandoRetorno = null;

            bool primeiraLinha = true;
            string msgRetorno = string.Empty;

            foreach (string line in lines)
            {
                if (primeiraLinha)
                {
                    vDataArquivo = line.Substring(143, 8);
                    if (boletoConfiguracao.NumeroBanco != line.Substring(0, 3))
                    {
                        unitOfWork.Rollback();
                        msgErro = "O arquivo selecionado não é do mesmo banco!";
                        return null;
                    }
                }
                primeiraLinha = false;                
                if (line.Substring(13, 1) == "A")
                {
                    ZerarVariaveis(ref retorno, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);
                    CarregarConteudoComandoARetornoPix(line, ref vComando, ref vNossoNumero, ref vDataVencimento, ref vDataLiquidacao, ref vValorRetorno, ref vJuros, ref vOutros, ref vTarifa, ref vValorRecebido, ref vDataCredito, ref vCodigoRejeicao, ref vDataBaixa, ref vRegistrada, ref vSomenteTarifa, ref codigoTitulo, ref vSituacaoAgendamento);

                    vSituacaoAgendamento = BuscarRetornoComandoBanco(vSituacaoAgendamento.Trim(), boletoConfiguracao, unitOfWork, ref comandoRetorno);

                    vComando = "A";
                    codigoRetorno = InserirRetorno(boletoConfiguracao, ref repTitulo, ref repPagamentoEletronicoRetorno, ref retorno, vDataArquivo, vComando, vNossoNumero, vDataVencimento, vDataLiquidacao, vValorRetorno, vJuros, vOutros, vTarifa, vValorRecebido, vDataCredito, vCodigoRejeicao, vDataBaixa, vRegistrada, vSomenteTarifa, codigoTitulo, vSituacaoAgendamento, fileName);
                    codigosRetornos.Add(codigoRetorno);
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && retorno.DataLiquidacao.HasValue && retorno.ValorRetorno > 0 && comandoRetorno != null && comandoRetorno.ComandoDeLiquidacao)
                    {
                        if (!QuitarTitulo(ref unitOfWork, out msgRetorno, retorno.DataLiquidacao.Value, retorno.ValorRetorno, 0, retorno.Tarifa + retorno.Outros + retorno.Juros, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa, usuario, retorno.ValorRecebido))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível quitar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                    if (retorno.Titulo != null && retorno.Titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada && comandoRetorno != null && comandoRetorno.ComandoDeEstorno)
                    {
                        if (!EstornarTitulo(ref unitOfWork, out msgRetorno, retorno.Titulo, boletoConfiguracao, tipoServicoMultisoftware, codigoEmpresa))
                        {
                            unitOfWork.Rollback();
                            msgErro = "Não foi possível estornar o título " + retorno.Titulo.Codigo.ToString() + ". Erro: " + msgRetorno;
                            return null;
                        }
                    }
                }               
                if (line.Substring(4, 5) == "99999")
                {
                    //fim
                }
            }

            return codigosRetornos;
        }

        private bool QuitarTitulo(ref Repositorio.UnitOfWork unitOfWork, out string msgRetorno, DateTime dataBaixa, decimal valorRetorno, decimal descontos, decimal acrescimos, Dominio.Entidades.Embarcador.Financeiro.Titulo tituloRetorno, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, Dominio.Entidades.Usuario usuario, decimal valorRecebido)
        {
            msgRetorno = string.Empty;
            if (boletoConfiguracao.PlanoConta == null)
            {
                msgRetorno = "A Configuração do Banco não possui plano de contas selecionado.";
                return false;
            }

            decimal valorBaixa = 0;
            if (valorRecebido > 0)
                valorBaixa = valorRecebido;
            else
                valorBaixa = valorRetorno;

            Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorPlanoContas(boletoConfiguracao.PlanoConta.Codigo, codigoEmpresa);
            if (tipoPagamentoRecebimento == null)
            {
                msgRetorno = "Não possui cadastro de Tipo de Pagamento e Recebimento vinculado ao plano de contas da configuração do banco.";
                return false;
            }

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(tituloRetorno.Codigo);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();
            tituloBaixa.DataBaixa = dataBaixa;
            tituloBaixa.DataBase = dataBaixa;
            tituloBaixa.DataOperacao = DateTime.Now;
            tituloBaixa.Numero = 1;
            tituloBaixa.Observacao = "BAIXA GERADA AUTOMÁTICAMENTE PELO ARQUIVO DE RETORNO";
            tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
            tituloBaixa.Sequencia = 1;
            tituloBaixa.Valor = valorBaixa;
            tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
            tituloBaixa.Pessoa = titulo.Pessoa;
            tituloBaixa.ValorPendente = 0;
            tituloBaixa.TipoPagamentoRecebimento = tipoPagamentoRecebimento;
            tituloBaixa.Usuario = usuario;
            repTituloBaixa.Inserir(tituloBaixa);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento tituloBaixaTipoPagamentoRecebimento = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento();
            tituloBaixaTipoPagamentoRecebimento.TipoPagamentoRecebimento = tipoPagamentoRecebimento;
            tituloBaixaTipoPagamentoRecebimento.TituloBaixa = tituloBaixa;
            tituloBaixaTipoPagamentoRecebimento.Valor = valorBaixa;
            repTituloBaixaTipoPagamentoRecebimento.Inserir(tituloBaixaTipoPagamentoRecebimento);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
            tituloAgrupado.TituloBaixa = tituloBaixa;
            tituloAgrupado.Titulo = titulo;
            tituloAgrupado.DataBaixa = dataBaixa;
            tituloAgrupado.DataBase = dataBaixa;

            repTituloBaixaAgrupado.Inserir(tituloAgrupado);

            decimal valorDesconto = descontos, valorAcrescimo = acrescimos;
            decimal valorPendente = tituloBaixa.ValorPendente - valorBaixa - descontos + acrescimos;

            titulo.DataLiquidacao = tituloBaixa.DataBaixa;
            titulo.DataBaseLiquidacao = tituloBaixa.DataBaixa;
            titulo.Provisao = false;

            if (valorBaixa > 0)
            {
                titulo.ValorPago = valorBaixa;
                if (valorBaixa > titulo.ValorOriginal)
                    titulo.Acrescimo = (valorBaixa - titulo.ValorOriginal);
                else if (valorBaixa < titulo.ValorOriginal)
                    titulo.Desconto = (titulo.ValorOriginal - valorBaixa);
            }
            else
                titulo.ValorPago = 0;

            titulo.ValorPendente = 0;
            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;

            titulo.DataAlteracao = DateTime.Now;

            repTitulo.Atualizar(titulo);

            if (configuracaoFinanceiro.AtivarControleDespesas)
                servBaixaTituloPagar.GeraReverteMovimentacaoFinanceiraControleDespesas(tituloBaixa.Codigo, unitOfWork, tipoServicoMultisoftware, false);
            else if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out msgRetorno, tituloBaixa.Codigo, unitOfWork, _conexao.StringConexao, tipoServicoMultisoftware, false, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta, null, titulo.Codigo))
                return false;

            return true;
        }

        private bool EstornarTitulo(ref Repositorio.UnitOfWork unitOfWork, out string msgRetorno, Dominio.Entidades.Embarcador.Financeiro.Titulo tituloRetorno, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa)
        {
            msgRetorno = string.Empty;
            if (boletoConfiguracao.PlanoConta == null)
            {
                msgRetorno = "A Configuração do Banco não possui plano de contas selecionado.";
                return false;
            }

            Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unitOfWork);
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo repTituloBaixaAcrescimo = new Repositorio.Embarcador.Financeiro.TituloBaixaAcrescimo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaDesconto repTituloBaixaDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao repTituloBaixaNegociacao = new Repositorio.Embarcador.Financeiro.TituloBaixaNegociacao(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao repTituloBaixaIntegracao = new Repositorio.Embarcador.Financeiro.TituloBaixaIntegracao(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido repTituloBaixaConhecimentoRemovido = new Repositorio.Embarcador.Financeiro.TituloBaixaConhecimentoRemovido(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa baixa = repTituloBaixa.BuscarPorTitulo(tituloRetorno.Codigo);

            if (baixa == null)
            {
                msgRetorno = "Nenhuma baixa encontrada para o título que gerou estorno.";
                return false;
            }

            int codigo = baixa.Codigo;
            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.BuscarPorBaixaTitulo(codigo);
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa;
            tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTituloBaixa.BuscarTitulosPorCodigo(codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao> listaParcelas = repTituloBaixaNegociacao.BuscarPorTituloBaixa(codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAcrescimo> listaTituloBaixaAcrescimo = repTituloBaixaAcrescimo.BuscarPorBaixaTitulo(codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto> listaTituloBaixaDesconto = repTituloBaixaDesconto.BuscarPorBaixaTitulo(codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao> listaTituloBaixaIntegracao = repTituloBaixaIntegracao.BuscarPorBaixaTitulo(codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTituloBaixaAgrupado = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(codigo);

            if (codigo > 0)
            {
                if (repTituloBaixa.ContemParcelaQuitada(codigo))
                {
                    msgRetorno = "Esta baixa do título de estorno já possui parcela de negociação quitada.";
                    return false;
                }
                else
                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigo);
            }

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                string erro = "";
                if (tituloBaixa.SituacaoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada)
                    if (!servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out erro, codigo, unitOfWork, _conexao.StringConexao, tipoServicoMultisoftware, true, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta, null, tituloRetorno.Codigo))
                    {
                        msgRetorno = erro;
                        return false;
                    }
            }

            for (int i = 0; i < listaTitulos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulos[i];

                titulo.DataCancelamento = DateTime.Now.Date;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                titulo.DataAlteracao = DateTime.Now;
                titulo.TituloBaixaNegociacao = null;

                repTitulo.Atualizar(titulo);
            }

            for (int i = 0; i < listaTitulo.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];

                titulo.DataLiquidacao = null;
                titulo.DataBaseLiquidacao = null;
                //titulo.Desconto = 0;
                //titulo.Acrescimo = 0;
                titulo.ValorPago = 0;
                titulo.ValorPendente = listaTitulo[i].Saldo;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                titulo.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                titulo.DataAlteracao = DateTime.Now;

                repTitulo.Atualizar(titulo);
            }

            for (int i = 0; i < listaParcelas.Count; i++)
            {
                repTituloBaixaNegociacao.Deletar(listaParcelas[i]);
            }

            decimal acrescimoBaixa = 0;
            for (int i = 0; i < listaTituloBaixaAcrescimo.Count; i++)
            {
                acrescimoBaixa = acrescimoBaixa + listaTituloBaixaAcrescimo[i].Valor;
                repTituloBaixaAcrescimo.Deletar(listaTituloBaixaAcrescimo[i]);
            }

            decimal descontoBaixa = 0;
            for (int i = 0; i < listaTituloBaixaDesconto.Count; i++)
            {
                descontoBaixa = descontoBaixa + listaTituloBaixaDesconto[i].Valor;
                repTituloBaixaDesconto.Deletar(listaTituloBaixaDesconto[i]);
            }


            for (int i = 0; i < listaTituloBaixaIntegracao.Count; i++)
                repTituloBaixaIntegracao.Deletar(listaTituloBaixaIntegracao[i]);

            for (int i = 0; i < listaTituloBaixaAgrupado.Count; i++)
                repTituloBaixaAgrupado.Deletar(listaTituloBaixaAgrupado[i]);

            //Servicos.Auditoria.Auditoria.Auditar(Auditado, tituloBaixa, null, "Cancelou Baixa.", unitOfWork);        

            return true;
        }

        private int InserirRetorno(Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, ref Repositorio.Embarcador.Financeiro.Titulo repTitulo, ref Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno repPagamentoEletronicoRetorno, ref Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno retorno, string vDataArquivo, string vComando, string vNossoNumero, string vDataVencimento, string vDataLiquidacao, string vValorRetorno, string vJuros, string vOutros, string vTarifa, string vValorRecebido, string vDataCredito, string vCodigoRejeicao, string vDataBaixa, string vRegistrada, string vSomenteTarifa, int codigoTitulo, string situacaoAgendamento, string fileName)
        {
            DateTime dataConvertida;
            decimal valorConvertido;
            retorno.NomeArquivo = fileName;
            retorno.Comando = vComando;
            retorno.NossoNumero = vNossoNumero;//.TrimStart('0');
            retorno.CodigoBanco = boletoConfiguracao.NumeroBanco;
            retorno.DataImportacao = DateTime.Now.Date;
            retorno.Agendamento = situacaoAgendamento;

            if (!string.IsNullOrWhiteSpace(vDataArquivo))
            {
                if (boletoConfiguracao.BoletoTipoCNAB == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB240 || boletoConfiguracao.BoletoTipoCNAB == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB240PIX)
                    DateTime.TryParseExact(vDataArquivo, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
                else
                    DateTime.TryParseExact(vDataArquivo, "ddMMyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
                retorno.DataArquivo = dataConvertida;
            }
            else
                retorno.DataArquivo = null;

            if (!string.IsNullOrWhiteSpace(vDataVencimento))
            {
                DateTime.TryParseExact(vDataVencimento, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
                if (dataConvertida != DateTime.MinValue)
                    retorno.DataVencimento = dataConvertida;
            }
            else
                retorno.DataVencimento = null;

            if (!string.IsNullOrWhiteSpace(vDataLiquidacao))
            {
                DateTime.TryParseExact(vDataLiquidacao, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
                if (dataConvertida != DateTime.MinValue)
                    retorno.DataLiquidacao = dataConvertida;
            }
            else
                retorno.DataLiquidacao = null;

            if (!string.IsNullOrWhiteSpace(vDataCredito))
            {
                DateTime.TryParseExact(vDataCredito, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
                if (dataConvertida != DateTime.MinValue)
                    retorno.DataCredito = dataConvertida;
            }
            else
                retorno.DataCredito = null;

            if (!string.IsNullOrWhiteSpace(vDataBaixa))
            {
                DateTime.TryParseExact(vDataBaixa, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
                if (dataConvertida != DateTime.MinValue)
                    retorno.DataBaixa = dataConvertida;
            }
            else
                retorno.DataBaixa = null;

            if (!string.IsNullOrWhiteSpace(vValorRetorno))
            {
                valorConvertido = 0;
                vValorRetorno = vValorRetorno.TrimStart('0');
                decimal.TryParse(vValorRetorno, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
                retorno.ValorRetorno = valorConvertido;
            }

            if (!string.IsNullOrWhiteSpace(vJuros))
            {
                valorConvertido = 0;
                vJuros = vJuros.TrimStart('0');
                decimal.TryParse(vJuros, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
                retorno.Juros = valorConvertido;
            }

            if (!string.IsNullOrWhiteSpace(vOutros))
            {
                valorConvertido = 0;
                vOutros = vOutros.TrimStart('0');
                decimal.TryParse(vOutros, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
                retorno.Outros = valorConvertido;
            }

            if (!string.IsNullOrWhiteSpace(vTarifa))
            {
                valorConvertido = 0;
                vTarifa = vTarifa.TrimStart('0');
                decimal.TryParse(vTarifa, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
                retorno.Tarifa = valorConvertido;
            }

            if (!string.IsNullOrWhiteSpace(vValorRecebido))
            {
                valorConvertido = 0;
                vValorRecebido = vValorRecebido.TrimStart('0');
                decimal.TryParse(vValorRecebido, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
                retorno.ValorRecebido = valorConvertido;
            }

            retorno.CodigoRejeicao = vCodigoRejeicao;

            if (!string.IsNullOrWhiteSpace(vRegistrada))
            {
                if (vRegistrada == "1" || vRegistrada == "01")
                    retorno.Registrada = true;
                else
                    retorno.Registrada = false;
            }
            else
                retorno.Registrada = false;

            if (!string.IsNullOrWhiteSpace(vSomenteTarifa))
            {
                if (vSomenteTarifa == "1" || vSomenteTarifa == "01")
                    retorno.SomenteTarifa = true;
                else
                    retorno.SomenteTarifa = false;
            }
            else
                retorno.SomenteTarifa = false;

            retorno.Empresa = boletoConfiguracao.Empresa;
            retorno.Titulo = null;
            retorno.ValorTitulo = 0;
            if (codigoTitulo > 0)
            {
                retorno.Titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
                if (retorno.Titulo != null)
                    retorno.ValorTitulo = retorno.Titulo.ValorOriginal;
            }
            if (!string.IsNullOrWhiteSpace(retorno.NossoNumero) && retorno.Titulo == null)
            {
                retorno.Titulo = repTitulo.BuscarTituloAPagarPorNossoNumero(retorno.NossoNumero);
                if (retorno.Titulo == null)
                    retorno.Titulo = repTitulo.BuscarTituloAPagarPorNossoNumeroIniciaCom(retorno.NossoNumero);
                if (retorno.Titulo != null)
                    retorno.ValorTitulo = retorno.Titulo.ValorOriginal;
            }
            if (retorno.Titulo == null || !repPagamentoEletronicoRetorno.RetornoJaInserido(retorno.Titulo.Codigo, retorno.Comando, retorno.Agendamento))
                repPagamentoEletronicoRetorno.Inserir(retorno);

            return retorno.Codigo;
        }

        private string BuscarRetornoComandoBanco(string vComando, Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao, Repositorio.UnitOfWork unitOfWork, ref Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno comandoRetorno)
        {
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno repPagamentoEletronicoComandoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno(unitOfWork);
            comandoRetorno = repPagamentoEletronicoComandoRetorno.BuscarComando(boletoConfiguracao.Codigo, vComando);

            if (comandoRetorno != null)
                return "(" + comandoRetorno.Comando + ") " + comandoRetorno.Descricao + ". Ret. Original: " + vComando;
            else
                return "Comando não cadastrado. Ret. Original: " + vComando;
        }

        private void ZerarVariaveis(ref Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno retorno, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref int codigoTitulo, ref string vSituacaoAgendamento)
        {
            retorno = new Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoRetorno();
            vComando = "";
            vNossoNumero = "";
            vDataVencimento = "";
            vDataLiquidacao = "";
            vValorRetorno = "";
            vJuros = "";
            vOutros = "";
            vTarifa = "";
            vValorRecebido = "";
            vDataCredito = "";
            vCodigoRejeicao = "";
            vDataBaixa = "";
            vRegistrada = "";
            vSomenteTarifa = "";
            vSituacaoAgendamento = "";
            codigoTitulo = 0;
        }

        private void CarregarConteudoComandoGRetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa)
        {
            vComando = linha.Substring(15, 2);//Código de Movimento Remessa
            vNossoNumero = linha.Substring(17, 44);//Código de Barras
            vDataVencimento = linha.Substring(107, 8);//Data de Vencimento do Título
            vValorRetorno = linha.Substring(115, 15);//Valor Nominal do Título
            vValorRecebido = linha.Substring(115, 15);//Valor Nominal do Título
            vJuros = linha.Substring(189, 15);//Juros de Mora por Dia
        }

        private void CarregarConteudoComandoNRetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref int codigoTitulo, ref string vSituacaoAgendamento)
        {
            string numeroTitulo = linha.Substring(37, 20);
            int.TryParse(numeroTitulo.TrimStart('0'), out codigoTitulo);

            vComando = linha.Substring(13, 1);//Código de Movimento Remessa
            vNossoNumero = linha.Substring(14, 23);//Código de Barras
            vDataVencimento = linha.Substring(87, 8);//Data de Vencimento do Título
            vDataLiquidacao = linha.Substring(87, 8);//Data do Pagamento            
            vValorRetorno = linha.Substring(95, 15);//Valor Nominal do Título
            vValorRecebido = linha.Substring(95, 15);//Valor Nominal do Título
            vJuros = "";//Juros de Mora por Dia
            vSituacaoAgendamento = linha.Substring(230, 10);//Situação do Agendamento
            vSituacaoAgendamento = vSituacaoAgendamento.Trim();
        }

        private void CarregarConteudoComandoARetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref int codigoTitulo, ref string vSituacaoAgendamento)
        {
            string numeroTitulo = linha.Substring(73, 20);
            int.TryParse(numeroTitulo.TrimStart('0'), out codigoTitulo);
            vNossoNumero = linha.Substring(134, 20);//Nº do Docum. Atribuído pelo Banco
            vNossoNumero = vNossoNumero.TrimStart('0');
            vDataLiquidacao = linha.Substring(93, 8);//Data do Pagamento            
            vValorRetorno = linha.Substring(119, 15);//Valor do Pagamento
            vValorRecebido = linha.Substring(119, 15);//Valor do Pagamento
            vSituacaoAgendamento = linha.Substring(230, 10);//Situação do Agendamento
            vSituacaoAgendamento = vSituacaoAgendamento.Trim();
        }

        private void CarregarConteudoComandoARetornoPix(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref int codigoTitulo, ref string vSituacaoAgendamento)
        {
            string numeroTitulo = linha.Substring(73, 20);
            int.TryParse(numeroTitulo.TrimStart('0'), out codigoTitulo);
            vNossoNumero = linha.Substring(134, 20);//Nº do Docum. Atribuído pelo Banco
            vNossoNumero = vNossoNumero.TrimStart('0');
            vDataLiquidacao = linha.Substring(93, 8);//Data do Pagamento            
            vValorRetorno = linha.Substring(119, 15);//Valor do Pagamento
            vValorRecebido = linha.Substring(119, 15);//Valor do Pagamento
            vSituacaoAgendamento = linha.Substring(230, 10);//Situação do Agendamento
            vSituacaoAgendamento = vSituacaoAgendamento.Trim();            
        }

        private void CarregarConteudoComandoBRetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa)
        {
            vDataVencimento = linha.Substring(127, 8);//Data do Vencimento (Nominal)            
            vValorRetorno = linha.Substring(135, 15);//Valor do Documento (Nominal)
            vJuros = linha.Substring(180, 15);//Valor da Mora
            vOutros = linha.Substring(195, 15);//Valor da Multa
            vValorRecebido = linha.Substring(135, 15);//Valor do Documento (Nominal)                                    
        }

        private void CarregarConteudoComandoJRetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref string vSituacaoAgendamento)
        {
            vNossoNumero = linha.Substring(17, 44);//Código de Barras
            vDataVencimento = linha.Substring(91, 8);//Data do Vencimento (Nominal)
            vDataLiquidacao = linha.Substring(144, 8);//Data do Pagamento
            vValorRetorno = linha.Substring(99, 15);//Valor do Título (Nominal)
            vJuros = linha.Substring(99, 15);//Valor da Mora + Multa            
            vValorRecebido = linha.Substring(152, 15);//Valor do Pagamento        
            vSituacaoAgendamento = linha.Substring(230, 10);//Situação do Agendamento
            vSituacaoAgendamento = vSituacaoAgendamento.Trim();
        }

        private void CarregarConteudoComandoORetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref string vSituacaoAgendamento, ref int codigoTitulo)
        {
            string numeroTitulo = linha.Substring(142, 20);
            int.TryParse(numeroTitulo.TrimStart('0'), out codigoTitulo);

            vNossoNumero = linha.Substring(17, 44);//Código de Barras

            vDataVencimento = linha.Substring(99, 8);//Data do Vencimento (Nominal)
            vDataLiquidacao = linha.Substring(99, 8);//Data do Pagamento

            vValorRetorno = linha.Substring(107, 15);//Valor do Título (Nominal)
            vJuros = "";//Valor da Mora + Multa            
            vValorRecebido = linha.Substring(107, 15);//Valor do Pagamento        
            vSituacaoAgendamento = linha.Substring(230, 10);//Situação do Agendamento
            vSituacaoAgendamento = vSituacaoAgendamento.Trim();
        }

        private static void CarregarConteudoComandoTRetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref int codigoTitulo)
        {
            vComando = linha.Substring(15, 2);            
            vNossoNumero = linha.Substring(37, 20);
            vDataVencimento = linha.Substring(73, 8);            
            vValorRetorno = linha.Substring(81, 15);
            vTarifa = linha.Substring(198, 15);
        }

        private static void CarregarConteudoComandoURetorno(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref string vSituacaoAgendamento)
        {
            vComando = linha.Substring(15, 2);
            vSituacaoAgendamento = vComando;
            vValorRecebido = linha.Substring(77, 15);            
            vJuros = linha.Substring(17, 15);
            vDataLiquidacao = linha.Substring(137, 8);
            vDataCredito = linha.Substring(145, 8);                                                            
        }

        private static void CarregarConteudoCNAB400(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref string vSituacaoAgendamento, ref int codigoTitulo)
        {
            string numeroTitulo = linha.Substring(79, 10);
            int.TryParse(numeroTitulo.TrimStart('0'), out codigoTitulo);

            vComando = "400";
            vNossoNumero = linha.Substring(116, 10);
            vDataVencimento = linha.Substring(98, 8);
            vDataLiquidacao = linha.Substring(90, 8);
            vValorRetorno = linha.Substring(152, 13);
            vJuros = "0";
            vOutros = "0";
            vTarifa = "0";
            vValorRecebido = linha.Substring(152, 13);
            vDataCredito = linha.Substring(90, 8);
            vCodigoRejeicao = linha.Substring(165, 108);
            vDataBaixa = linha.Substring(90, 8);
            vRegistrada = "";
            vSomenteTarifa = "";
            vSituacaoAgendamento = linha.Substring(165, 108);
        }

        private static void CarregarConteudoCNAB500(string linha, ref string vComando, ref string vNossoNumero, ref string vDataVencimento, ref string vDataLiquidacao, ref string vValorRetorno, ref string vJuros, ref string vOutros, ref string vTarifa, ref string vValorRecebido, ref string vDataCredito, ref string vCodigoRejeicao, ref string vDataBaixa, ref string vRegistrada, ref string vSomenteTarifa, ref string vSituacaoAgendamento, ref int codigoTitulo)
        {
            string numeroTitulo = linha.Substring(79, 10);
            int.TryParse(numeroTitulo.TrimStart('0'), out codigoTitulo);

            vComando = "400";
            vNossoNumero = linha.Substring(116, 10);
            vDataVencimento = linha.Substring(98, 8);
            vDataLiquidacao = linha.Substring(90, 8);
            vValorRetorno = linha.Substring(152, 13);
            vJuros = "0";
            vOutros = "0";
            vTarifa = "0";
            vValorRecebido = linha.Substring(152, 13);
            vDataCredito = linha.Substring(90, 8);
            vCodigoRejeicao = linha.Substring(165, 108);
            vDataBaixa = linha.Substring(90, 8);
            vRegistrada = "";
            vSomenteTarifa = "";
            vSituacaoAgendamento = linha.Substring(165, 108);
        }

        #endregion
    }
}
