using Dominio.ObjetosDeValor.EDI.RPS;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Hangfire.Storage.Monitoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240;

namespace Servicos.Embarcador.Financeiro
{
    public class RemessaPagamento
    {
        public static bool GerarRemessaPagamento(int codigoPagamentoDigital, string stringConexao, ref MemoryStream arquivo, out string msgErro, out int numero, bool naoUtilizarDeafultParaPagamentoDeTributos, out string nomeArquivo)
        {
            nomeArquivo = "";
            msgErro = "";
            numero = 0;

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigoPagamentoDigital);
            if (pagamentoEletronico == null)
            {
                msgErro = "Pagamento não localizado.";
                return false;
            } 
            numero = pagamentoEletronico.Numero;

            if (pagamentoEletronico.ModalidadePagamentoEletronico == ModalidadePagamentoEletronico.PIX)
            {   
                bool sucesso = false;
                string mensagem = "";
                string msgErroPIX = "";
                int numeroCNAB = 0;

                switch (pagamentoEletronico.BoletoConfiguracao.BoletoTipoCNAB)
                {
                    case BoletoTipoCNAB.CNAB240PIX:
                        sucesso = GerarRemessaPagamentoPIX240(pagamentoEletronico.Codigo, stringConexao, ref arquivo, out msgErroPIX, out numeroCNAB, naoUtilizarDeafultParaPagamentoDeTributos);
                        msgErro = msgErroPIX;
                        break;
                    case BoletoTipoCNAB.CNAB500PIX:
                        sucesso = GerarRemessaPagamentoPIX(pagamentoEletronico.Codigo, stringConexao, ref arquivo, out msgErroPIX, out numeroCNAB, naoUtilizarDeafultParaPagamentoDeTributos);
                        msgErro = msgErroPIX;
                        break;
                    case BoletoTipoCNAB.CNAB750PIX:
                        //sucesso = GerarRemessaPagamentoPIX750(pagamentoEletronico.Codigo, stringConexao, ref arquivo, out msgErroPIX, out numeroCNAB, naoUtilizarDeafultParaPagamentoDeTributos);
                        msgErro = "Não implementado";
                        break;
                    default:
                        sucesso = GerarRemessaPagamentoPIX(pagamentoEletronico.Codigo, stringConexao, ref arquivo, out msgErroPIX, out numeroCNAB, naoUtilizarDeafultParaPagamentoDeTributos);
                        break;
                };

                return sucesso;
            }

            if (pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.BancoCECRED)
                nomeArquivo = "PGT_" + pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(7, '0') + pagamentoEletronico.BoletoConfiguracao.DigitoConta.PadLeft(1, '0') + "_" + pagamentoEletronico.DataGeracao.Value.ToString("ddMMyy") + "_" + numero.ToString().PadLeft(6, '0') + ".REM";

            if (pagamentoEletronico.BoletoConfiguracao.BoletoTipoCNAB == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoTipoCNAB.CNAB240)
            {
                if (GerarRemessaPagamentoCNAB240(pagamentoEletronico.Codigo, stringConexao, ref arquivo, out string msgErroCNAB, out int numeroCNAB, naoUtilizarDeafultParaPagamentoDeTributos))
                    return true;
                else
                {
                    msgErro = msgErroCNAB;
                    return false;
                }
            }
            else
            {
                if (GerarRemessaPagamentoCNAB400(pagamentoEletronico.Codigo, stringConexao, ref arquivo, out string msgErroCNAB, out int numeroCNAB, naoUtilizarDeafultParaPagamentoDeTributos))
                    return true;
                else
                {
                    msgErro = msgErroCNAB;
                    return false;
                }
            }
        }

        #region 240

        public static bool GerarRemessaPagamentoCNAB240(int codigoPagamentoDigital, string stringConexao, ref MemoryStream arquivo, out string msgErro, out int numero, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            msgErro = "";
            numero = 0;

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigoPagamentoDigital);
            if (pagamentoEletronico == null)
            {
                msgErro = "Pagamento não localizado.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulos = repPagamentoEletronicoTitulo.BuscarPorPagamento(codigoPagamentoDigital);

            if (titulos == null || titulos.Count == 0)
            {
                msgErro = "Nenhum título encontrado.";
                return false;
            }
            if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDC_TEDCip)
            {
                foreach (var titulo in titulos)
                {
                    if (string.IsNullOrWhiteSpace(titulo.Titulo.NossoNumero))
                    {
                        msgErro = "Existe títulos sem o nosso número informado (" + titulo.Titulo.Codigo.ToString("D") + "), favor edite antes de gerar a cobrança TDC TED Cip.";
                        return false;
                    }
                }
            }
            if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TT_TituloTerceiro)
            {
                foreach (var titulo in titulos)
                {
                    if (string.IsNullOrWhiteSpace(titulo.Titulo.NossoNumero))
                    {
                        msgErro = "Existe títulos sem o nosso número informado (" + titulo.Titulo.Codigo.ToString("D") + "), favor edite antes de gerar a cobrança para títulos de terceiro.";
                        return false;
                    }
                }
            }
            numero = pagamentoEletronico.Numero;
            bool layoutItau = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Itau;
            bool layoutSicoob = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bancoob;
            bool layoutCecred = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.BancoCECRED;
            string layoutArquivo = "089";
            if (layoutCecred)
                layoutArquivo = "088";
            try
            {
                if (titulos.Count() > 0)
                {
                    if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo && titulos.Any(o => !string.IsNullOrWhiteSpace(o.Titulo.NossoNumero) && o.Titulo.NossoNumero.Length != 44))
                    {
                        msgErro = "Existem títulos com o número do boleto inválido (diferente de 44 caracteres).";
                        return false;
                    }

                    List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosProprioBanco = null;
                    List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosOutrosBancos = null;

                    StreamWriter x;
                    arquivo = new MemoryStream();
                    int qtdLotes = 0;
                    int qtdRegistroLote = 0;
                    int qtdRegistros = 0;
                    int qtdContasConcil = 0;
                    decimal somaValores = 0;

                    Encoding utf8WithoutBom = new UTF8Encoding(false);
                    x = new StreamWriter(arquivo, utf8WithoutBom);

                    //Header
                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                        "0000" +//Lote de Serviço 4 a 7
                        "0" + //Tipo de Registro 8 a 8
                        (layoutItau ? "".PadRight(6, ' ') : "".PadRight(9, ' ')) + //Uso Exclusivo FEBRABAN / CNAB 9 a 14
                        (layoutItau ? "081" : "") + //LAYOUT DE ARQUIVO 15 a 17
                        "2" + //Tipo de Inscrição da Empresa - CNPJ 18 a 18
                        pagamentoEletronico.Empresa.CNPJ_SemFormato + //Número de Inscrição da Empresa 19 a 32
                        pagamentoEletronico.BoletoConfiguracao.NumeroConvenio.PadRight(20, ' ').Substring(0, 20) + //Código do Convênio no Banco 33 a 52
                        pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(5, '0').Substring(0, 5) + //Agência Mantenedora da Conta 53 a 57
                        pagamentoEletronico.BoletoConfiguracao.DigitoAgencia.PadLeft(1, ' ').Substring(0, 1) + //Dígito Verificador da Agência 58 a 58
                        pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(12, '0').Substring(0, 12) + //Número da Conta Corrente 59 a 70
                        (layoutItau ? " " : pagamentoEletronico.BoletoConfiguracao.DigitoConta.PadLeft(1, '0').Substring(0, 1)) + //Dígito Verificador da Conta 71 a 71
                        (layoutItau ? pagamentoEletronico.BoletoConfiguracao.DigitoConta.PadLeft(1, '0').Substring(0, 1) : " ") + //Dígito Verificador da Ag/Conta 72 a 72
                        pagamentoEletronico.Empresa.RazaoSocial.PadRight(30, ' ').Substring(0, 30) + //Nome da Empresa 73 a 102
                        pagamentoEletronico.BoletoConfiguracao.DescricaoBanco.PadRight(30, ' ').Substring(0, 30) + //Nome do Banco 103 a 132
                        "".PadRight(10, ' ') + //Uso Exclusivo FEBRABAN / CNAB 133 a 142
                        "1" + //Código Remessa / Retorno 143 a 143
                        pagamentoEletronico.DataGeracao.Value.ToString("ddMMyyyy") + //Data de Geração do Arquivo 144 a 151
                        pagamentoEletronico.DataGeracao.Value.ToString("hhMMss") + //Hora de Geração do Arquivo 152 a 157
                        (layoutItau ? "0".PadRight(9, '0') : pagamentoEletronico.Numero.ToString("D").PadLeft(6, '0').Substring(0, 6)) + //Número Seqüencial do Arquivo  158 a 166
                        (layoutItau ? ("00000" + "".PadRight(69, ' ')) // UNIDADE DE DENSIDADE  167 a 171 BRANCOS 172 a 240
                            : (layoutArquivo + //No da Versão do Layout do Arquivo
                            "01600" + //Densidade de Gravação do Arquivo
                            "".PadRight(20, ' ') + //Para Uso Reservado do Banco
                            "".PadRight(20, ' ') + //Para Uso Reservado da Empresa
                            "".PadRight(29, ' ')))));//Uso Exclusivo FEBRABAN / CNAB

                    if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos ||
                        pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo)
                    {
                        GerarRegistroHederLoteCC(titulos, pagamentoEletronico, ref qtdLotes, ref qtdRegistroLote, ref somaValores, ref qtdRegistros, ref x, true, naoUtilizarDeafultParaPagamentoDeTributos);

                    }
                    else if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDC_TEDCip ||
                        pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TT_TituloTerceiro)
                    {
                        titulosProprioBanco = titulos.Where(o => (o.Titulo.NossoNumero?.Substring(0, 3) ?? "") == pagamentoEletronico.BoletoConfiguracao.NumeroBanco).ToList();
                        titulosOutrosBancos = titulos.Where(o => (o.Titulo.NossoNumero?.Substring(0, 3) ?? "") != pagamentoEletronico.BoletoConfiguracao.NumeroBanco).ToList();

                        GerarRegistroHederLote(titulosProprioBanco, pagamentoEletronico, ref qtdLotes, ref qtdRegistroLote, ref somaValores, ref qtdRegistros, ref x, true);
                        GerarRegistroHederLote(titulosOutrosBancos, pagamentoEletronico, ref qtdLotes, ref qtdRegistroLote, ref somaValores, ref qtdRegistros, ref x, false);
                    }
                    else
                    {
                        titulosProprioBanco = titulos.Where(o => o.Titulo.Fornecedor.Banco != null && o.Titulo.Fornecedor.Banco.Numero > 0 && !string.IsNullOrWhiteSpace(o.Titulo.Fornecedor.Banco.Numero.ToString()) && o.Titulo.Fornecedor.Banco.Numero.ToString().PadLeft(3, '0') == pagamentoEletronico.BoletoConfiguracao.NumeroBanco).ToList();
                        titulosOutrosBancos = titulos.Where(o => o.Titulo.Fornecedor.Banco != null && o.Titulo.Fornecedor.Banco.Numero > 0 && !string.IsNullOrWhiteSpace(o.Titulo.Fornecedor.Banco.Numero.ToString()) && o.Titulo.Fornecedor.Banco.Numero.ToString().PadLeft(3, '0') != pagamentoEletronico.BoletoConfiguracao.NumeroBanco).ToList();

                        GerarRegistroHederLoteCC(titulosProprioBanco, pagamentoEletronico, ref qtdLotes, ref qtdRegistroLote, ref somaValores, ref qtdRegistros, ref x, true, naoUtilizarDeafultParaPagamentoDeTributos);
                        GerarRegistroHederLoteCC(titulosOutrosBancos, pagamentoEletronico, ref qtdLotes, ref qtdRegistroLote, ref somaValores, ref qtdRegistros, ref x, false, naoUtilizarDeafultParaPagamentoDeTributos);
                    }

                    //Trailer
                    qtdRegistros += 2;
                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação
                        "9999" +//Lote de Serviço
                        "9" + //Tipo de Registro
                        "".PadRight(9, ' ') + //Uso Exclusivo FEBRABAN / CNAB
                        qtdLotes.ToString("D").PadLeft(6, '0') + //Quantidade de Lotes do Arquivo
                        qtdRegistros.ToString("D").PadLeft(6, '0') + //Quantidade de Registros do Arquivo
                        (layoutItau ? "".PadLeft(6, ' ') : qtdContasConcil.ToString("D").PadLeft(6, '0')) + //Qtde de Contas p/ Conc. (Lotes)
                        "".PadRight(205, ' ')));//Uso Exclusivo FEBRABAN / CNAB

                    x.Flush();
                    arquivo.Position = 0;
                    string mensagemRetorno = string.Empty;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por FTP: " + ex.ToString());
                return false;
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }

            return true;
        }

        private static void GerarRegistroHederLoteCC(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosBanco, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico,
            ref int qtdLotes, ref int qtdRegistroLote, ref decimal somaValores, ref int qtdRegistros, ref StreamWriter x, bool boletosBancoProprio, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {

            bool modalidadeTributo = pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo;
            bool tibutoSemCodigoBarras = pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos;
            bool tibutoComCodigoBarras = pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo;
            bool bancoSantander = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Santander;
            bool layoutItau = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Itau;
            bool layoutSicoob = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bancoob;
            bool layoutCecred = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.BancoCECRED;

            string formaPagamento = "  ";
            if (layoutItau && tibutoComCodigoBarras)
                formaPagamento = "91";
            else if (layoutItau && tibutoSemCodigoBarras)
                formaPagamento = "16";
            else if (bancoSantander && !modalidadeTributo)
                formaPagamento = "03";
            else if (modalidadeTributo && tibutoComCodigoBarras)
                formaPagamento = "11";
            else if (modalidadeTributo && tibutoSemCodigoBarras)
                formaPagamento = "16";
            else if (boletosBancoProprio)
                formaPagamento = "01";
            else
                formaPagamento = "41";

            if (pagamentoEletronico.FormaLancamentoPagamentoEletronico.HasValue && pagamentoEletronico.FormaLancamentoPagamentoEletronico.Value != FormaLancamentoPagamentoEletronico.Padrao)
                formaPagamento = pagamentoEletronico.FormaLancamentoPagamentoEletronico.Value.ObterNumero();

            string tipoServico = "  ";
            if (pagamentoEletronico.TipoServicoPagamentoEletronico.HasValue && pagamentoEletronico.TipoServicoPagamentoEletronico.Value != TipoServicoPagamentoEletronico.Padrao)
                tipoServico = pagamentoEletronico.TipoServicoPagamentoEletronico.Value.ObterNumero();

            string versaoLayout = "";
            if (bancoSantander)
                versaoLayout = "031";
            else if (modalidadeTributo && layoutItau)
                versaoLayout = "030";
            else if (modalidadeTributo)
                versaoLayout = "012";
            else if (layoutItau)
                versaoLayout = "040";
            else
                versaoLayout = "045";

            //Registro Header de Lote
            if (titulosBanco != null && titulosBanco.Count > 0)
            {
                qtdLotes += 1;
                x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                    qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                    "1" + //Tipo de Registro 8 a 8
                    "C" + //Tipo da Operação 9 a 9
                    (!string.IsNullOrWhiteSpace(tipoServico) ? tipoServico : modalidadeTributo ? "22" : "20") + //Tipo do Serviço 10 a 11
                                                                                                                //(bancoSantander && !modalidadeTributo ? "03" : modalidadeTributo && tibutoComCodigoBarras ? "11" : modalidadeTributo && tibutoSemCodigoBarras ? "16" : boletosBancoProprio ? "01" : "41") + //Forma de Lançamento 12 a 13
                    formaPagamento + //Forma de Lançamento 12 a 13
                    versaoLayout + //Nº da Versão do Layout do Lote 14 a 16
                    "".PadRight(1, ' ') + //Uso Exclusivo da FEBRABAN/CNAB 17 a 17
                    "2" + //Tipo de Inscrição da Empresa - CNPJ 18 a 18
                    pagamentoEletronico.Empresa.CNPJ_SemFormato + //Número de Inscrição da Empresa 19 a 32
                    pagamentoEletronico.BoletoConfiguracao.NumeroConvenio.PadRight(20, ' ').Substring(0, 20) + //Código do Convênio no Banco 33 a 52
                    pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(5, '0').Substring(0, 5) + //Agência Mantenedora da Conta 53 a 57
                    pagamentoEletronico.BoletoConfiguracao.DigitoAgencia.PadLeft(1, ' ').Substring(0, 1) + //Dígito Verificador da Agência 58 a 58
                    pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(12, '0').Substring(0, 12) + //Número da Conta Corrente 59 a 70
                    (layoutItau ? " " : pagamentoEletronico.BoletoConfiguracao.DigitoConta.PadLeft(1, '0').Substring(0, 1)) + //Dígito Verificador da Conta 71 a 71
                    (layoutItau ? pagamentoEletronico.BoletoConfiguracao.DigitoConta.PadLeft(1, '0').Substring(0, 1) : " ") + //Dígito Verificador da Ag/Conta 72 a 72
                    pagamentoEletronico.Empresa.RazaoSocial.PadRight(30, ' ').Substring(0, 30) + //Nome da Empresa 73 a 102
                    "".PadRight(40, ' ') + //Mensagem 103 a 132 e 133 a 142
                    pagamentoEletronico.Empresa.Endereco.PadRight(30, ' ').Substring(0, 30) + //Nome da Rua, Av, Pça, Etc 143 a 172
                    (layoutItau ? pagamentoEletronico.Empresa.Numero.PadLeft(5, '0').Substring(0, 5) : pagamentoEletronico.Empresa.Numero.PadRight(5, ' ').Substring(0, 5)) + //Número do Local 173 a 177
                    pagamentoEletronico.Empresa.Complemento.PadRight(15, ' ').Substring(0, 15) + //Casa, Apto, Sala, Etc 178 a 192
                    pagamentoEletronico.Empresa.Localidade.Descricao.PadRight(20, ' ').Substring(0, 20) + //Nome da Cidade 193 a 212
                    Utilidades.String.OnlyNumbers(pagamentoEletronico.Empresa.CEP ?? "").Substring(0, 5) + //CEP 213 a 220
                    Utilidades.String.OnlyNumbers(pagamentoEletronico.Empresa.CEP ?? "").Substring(5, 3) + //Complemento do CEP 213 a 220
                    pagamentoEletronico.Empresa.Localidade.Estado.Sigla.PadRight(2, ' ') + //Sigla do Estado 221 a 222
                    (layoutItau ? "  " : "01") + //Indicativo de Forma de Pagamento do Serviço ITAU 223 A 230 E 231 A 240
                    "".PadRight(6, ' ') + //CNAB
                    "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno

                qtdRegistroLote = 0;
                somaValores = 0;
                foreach (var titulo in titulosBanco)
                {
                    qtdRegistroLote += 1;
                    somaValores += titulo.Titulo.Saldo;
                    if (modalidadeTributo)
                    {
                        if (tibutoComCodigoBarras)
                        {
                            string codigoBarras = Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero);


                            if (layoutItau)
                            {
                                //Registro O
                                x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                                   qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                                   "3" + //Tipo de Registro 8 a 8
                                   qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 13
                                   "O" + //Cód. Segmento do Registro Detalhe 14 a 14
                                   "000" + //TIPO DE MOVIMENTO 15 a 17
                                   codigoBarras.PadLeft(48, ' ').Substring(0, 48) + //CÓDIGO DE BARRAS 18 a 65
                                   Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)).PadRight(30, ' ').Substring(0, 30) +//Nome da Concessionária / Órgão 66 a 95
                                   titulo.Titulo.DataVencimento.Value.ToString("ddMMyyyy") + //Data do Vencimento (Nominal) 96 a 103
                                   "REA" + //TIPO DE MOEDA 104 a 106
                                   Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//QUANTIDADE DE MOEDA 107 a 121
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//VALOR PREVISTO DO PAGAMENTO 122 a 136
                                   titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy") + //Data do Pagamento 137 a 144
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//VALOR DE EFETIVAÇÃO DO PAGAMENTO 145 a 159
                                   "".PadRight(3, ' ') +//COMPLEMENTO DE REGISTRO 160 a 162
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(9, '0').Substring(0, 9) + //MERO DA NOTA FISCAL 163 a 171
                                   "".PadRight(3, ' ') +//COMPLEMENTO DE REGISTRO 172 a 174
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(20, '0').Substring(0, 20) + //Nº DOCTO ATRIBUÍDO PELA EMPRESA 175 a 194
                                   "".PadRight(21, ' ') +//COMPLEMENTO DE REGISTRO 195 a 215
                                   "".PadRight(15, ' ') +//NÚMERO ATRIBUÍDO PELO BANCO 216 a 230
                                   "".PadRight(10, ' ')));//CÓDIGO DE OCORRÊNCIAS P/ RETORNO 231 a 240
                            }
                            else
                            {
                                //Registro O
                                x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                                   qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                                   "3" + //Tipo de Registro 8 a 8
                                   qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 13
                                   "O" + //Cód. Segmento do Registro Detalhe 14 a 14
                                   "0" + //Tipo de Movimento 15 a 17
                                   "00" + //Código da Instrução p/ Movimento 15 a 17
                                   codigoBarras.PadLeft(44, '0').Substring(0, 44) + //CÓDIGO DE BARRAS 18 a 65
                                   Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)).PadRight(30, ' ').Substring(0, 30) +//Nome da Concessionária / Órgão 66 a 95
                                   titulo.Titulo.DataVencimento.Value.ToString("ddMMyyyy") + //Data do Vencimento (Nominal) 96 a 103
                                   titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy") + //Data do Pagamento
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do Pagamento
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(20, '0').Substring(0, 20) + //Nº do Docto Atribuído pela Empresa
                                   "".PadRight(20, ' ') +//Nº do Docum. Atribuído pelo Banco
                                   "".PadRight(68, ' ') +//Uso Exclusivo FEBRABAN/CNAB
                                   "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno
                            }
                        }
                        else
                        {
                            string codigoReceitaTributo = "";
                            string codigoIdentificacaoTributo = "";
                            string referencia = "";
                            string variacao = "";
                            string formatodata = "";
                            DateTime? periodoApracao = null;
                            Dominio.Entidades.Cliente contribuinte = null;

                            if (titulo.Titulo.Contribuinte != null)
                                contribuinte = titulo.Titulo.Contribuinte;
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos && titulo.Titulo.ContribuinteTributo != null)
                                contribuinte = titulo.Titulo.ContribuinteTributo;

                            if (titulo.Titulo.TributoCodigoReceita != null)
                                codigoReceitaTributo = titulo.Titulo.TributoCodigoReceita.CodigoIntegracao;
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos && bancoSantander)
                                codigoReceitaTributo = "001708";
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos)
                                codigoReceitaTributo = "1708  ";

                            if (titulo.Titulo.TributoVariacaoImposto != null)
                                variacao = titulo.Titulo.TributoVariacaoImposto.CodigoIntegracao;
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos)
                                variacao = "01";

                            if (bancoSantander && pagamentoEletronico.ModalidadePagamentoEletronico == ModalidadePagamentoEletronico.TRB_Tributos)
                            {
                                formatodata = "ddMMyyyy";
                                variacao = contribuinte != null ? contribuinte.Tipo == "J" ? "02" : "01" : "02";
                            }
                            else
                            {
                                formatodata = "yyyyMMdd";
                            }

                            if (titulo.Titulo.TributoTipoImposto != null)
                                codigoIdentificacaoTributo = titulo.Titulo.TributoTipoImposto.CodigoIntegracao;
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos)
                                codigoIdentificacaoTributo = "16";

                            if (titulo.Titulo.PeriodoApuracao.HasValue)
                                periodoApracao = titulo.Titulo.PeriodoApuracao;
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos)
                                periodoApracao = titulo.Titulo.DataEmissao;

                            if (!string.IsNullOrEmpty(titulo.Titulo.TributoReferencia))
                                referencia = titulo.Titulo.TributoReferencia;
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos && bancoSantander)
                                referencia = Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(17, '0').Substring(0, 17);
                            else if (!naoUtilizarDeafultParaPagamentoDeTributos)
                                referencia = Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(17, '0').Substring(0, 17);

                            if (layoutItau)
                            {
                                //Registro N
                                if (codigoIdentificacaoTributo == "01")
                                {
                                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                                       qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a7
                                       "3" + //Tipo de Registro 8 a 8
                                       qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 13
                                       "N" + //Cód. Segmento do Registro Detalhe 14 a 14
                                       "0" + //Tipo de Movimento 15 a 17
                                       "00" + //Código da Instrução p/ Movimento 15 a 17
                                       (!string.IsNullOrWhiteSpace(codigoIdentificacaoTributo) ? codigoIdentificacaoTributo.PadLeft(2, ' ').Substring(0, 2) : "  ") +//IDENTIFICAÇÃO DO TRIBUTO 18 a 19
                                       (!string.IsNullOrWhiteSpace(codigoReceitaTributo) ? codigoReceitaTributo.PadLeft(4, ' ').Substring(0, 4) : "    ") + //CÓDIGO DE PAGAMENTO 20 a 23
                                       (periodoApracao.HasValue && periodoApracao.Value > DateTime.MinValue ? periodoApracao.Value.ToString("MMyyyy") : "        ") + //MÊS E ANO DA COMPETÊNCIA 24 a 29
                                       (contribuinte != null ? contribuinte.CPF_CNPJ_SemFormato.PadLeft(14, '0') : "0".PadLeft(14, '0')) +//IDENTIFICAÇÃO CNPJ/CEI/NIT/PIS DO CONTRIBUINTE 30 a 43
                                       Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//VALOR PREVISTO DO PAGAMENTO DO INSS 44 a 57
                                       Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//VALOR DE OUTRAS ENTIDADES 58 a 71
                                       Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//ATUALIZAÇÃO MONETÁRIA 72 a 85
                                       Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//VALOR ARRECADADO 86 a 99
                                       titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy") + //DATA DA ARRECADAÇÃO/ EFETIVAÇÃO DO PAGAMENTO 100 a 107
                                       "".PadRight(8, ' ') +//COMPLEMENTO DO REGISTRO 108 a 115
                                       "".PadRight(50, ' ') +//INFORMAÇÕES COMPLEMENTARES 116 a 165
                                       (contribuinte != null ? Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(contribuinte.Nome)).PadRight(30, ' ').Substring(0, 30) : "".PadRight(30, ' ').Substring(0, 30)) +//NOME DO CONTRIBUINTE 166 a 195
                                       Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(20, '0').Substring(0, 20) + //Nº DOCTO ATRIBUÍDO PELA EMPRESA 196 a 215
                                       "".PadRight(15, ' ') + //NÚMERO ATRIBUÍDO PELO BANCO 213 a 230
                                       "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno 231 a 240
                                }
                                else
                                {
                                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                                       qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a7
                                       "3" + //Tipo de Registro 8 a 8
                                       qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 13
                                       "N" + //Cód. Segmento do Registro Detalhe 14 a 14
                                       "0" + //Tipo de Movimento 15 a 17
                                       "00" + //Código da Instrução p/ Movimento 15 a 17
                                       (!string.IsNullOrWhiteSpace(codigoIdentificacaoTributo) ? codigoIdentificacaoTributo.PadLeft(2, ' ').Substring(0, 2) : "  ") +//IDENTIFICAÇÃO DO TRIBUTO 18 a 19                                      
                                       (!string.IsNullOrWhiteSpace(codigoReceitaTributo) ? codigoReceitaTributo.PadLeft(4, ' ').Substring(0, 4) : "    ") + //CÓDIGO DE PAGAMENTO 20 a 23
                                       (contribuinte != null ? contribuinte.Tipo == "J" ? "2" : "1" : "2") + //TIPO DE INSCRIÇÃO DO CONTRIBUINTE 24 a 24
                                       (contribuinte != null ? contribuinte.CPF_CNPJ_SemFormato.PadLeft(14, '0') : "0".PadLeft(14, '0')) +//CPF OU CNPJ DO CONTRIBUINTE 25 a 38
                                       (periodoApracao.HasValue && periodoApracao.Value > DateTime.MinValue ? periodoApracao.Value.ToString("ddMMyyyy") : "        ") + //PERÍODO DE APURAÇÃO 39 a 46
                                       (!string.IsNullOrWhiteSpace(referencia) ? referencia.PadLeft(17, '0').Substring(0, 17) : "".PadRight(17, '0')) +//NÚMERO DE REFERÊNCIA 47 a 63
                                       Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//VALOR PRINCIPAL 64 a 77
                                       Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//VALOR DA MULTA 78 a 91
                                       Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//VALOR DOS JUROS/ENCARGOS 92 a 105
                                       Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//VALOR TOTAL A SER PAGO 106 a 119
                                       titulo.Titulo.DataVencimento.Value.ToString("ddMMyyyy") + //DATA DE VENCIMENTO 120 a 127
                                       titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy") + //DATA DO PAGAMENTO 128 a 135
                                       "".PadRight(30, ' ') +//COMPLEMENTO DE REGISTRO 136 a 165
                                       (contribuinte != null ? Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(contribuinte.Nome)).PadRight(30, ' ').Substring(0, 30) : "".PadRight(30, ' ').Substring(0, 30)) +//NOME DO CONTRIBUINTE 166 a 195
                                       Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(20, '0').Substring(0, 20) + //Nº DOCTO ATRIBUÍDO PELA EMPRESA 196 a 215
                                       "".PadRight(15, ' ') + //NÚMERO ATRIBUÍDO PELO BANCO 213 a 230
                                       "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno 231 a 240
                                }
                            }
                            else
                            {
                                //Registro N
                                x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                                   qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a7
                                   "3" + //Tipo de Registro 8 a 8
                                   qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 13
                                   "N" + //Cód. Segmento do Registro Detalhe 14 a 14
                                   "0" + //Tipo de Movimento 15 a 17
                                   "00" + //Código da Instrução p/ Movimento 15 a 17
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(20, '0').Substring(0, 20) + // Nº do Docto Atribuído pela Empresa
                                   "".PadRight(20, '0') +//Nº do Docum. Atribuído pelo Banco                               
                                                         //(titulo.Titulo.Contribuinte == null && naoUtilizarDeafultParaPagamentoDeTributos ? "".PadRight(30, ' ').Substring(0, 30) : Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.ContribuinteTributo.Nome)).PadRight(30, ' ').Substring(0, 30)) +//Nome do Contribuinte
                                   (contribuinte != null ? Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(contribuinte.Nome)).PadRight(30, ' ').Substring(0, 30) : "".PadRight(30, ' ').Substring(0, 30)) +//Nome do Contribuinte
                                   titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy") + //Data do Pagamento
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do Pagamento                                                                                                                                                                                              
                                                                                                                                        //(!string.IsNullOrWhiteSpace(titulo.Titulo.CodigoReceitaTributo) ? titulo.Titulo.CodigoReceitaTributo.PadLeft(6, ' ').Substring(0, 6) : naoUtilizarDeafultParaPagamentoDeTributos ? "      " : bancoSantander ? "001708" : "1708  ") + //Código da Receita do Tributo
                                   (!string.IsNullOrWhiteSpace(codigoReceitaTributo) ? codigoReceitaTributo.PadLeft(6, ' ').Substring(0, 6) : "      ") + //Código da Receita do Tributo
                                                                                                                                                          //"01" + //e Identificação do Contribuinte
                                   (!string.IsNullOrWhiteSpace(variacao) ? variacao.PadLeft(2, ' ').Substring(0, 2) : "  ") +//e Identificação do Contribuinte
                                                                                                                             //(titulo.Titulo.Contribuinte == null && naoUtilizarDeafultParaPagamentoDeTributos ? "0".PadLeft(14, '0') : titulo.Titulo.ContribuinteTributo.CPF_CNPJ_SemFormato.PadLeft(14, '0')) +//Identificação do Contribuinte
                                   (contribuinte != null ? contribuinte.CPF_CNPJ_SemFormato.PadLeft(14, '0') : "0".PadLeft(14, '0')) +//Nome do Contribuinte
                                                                                                                                      //(!string.IsNullOrWhiteSpace(titulo.Titulo.CodigoIdentificacaoTributo) ? titulo.Titulo.CodigoIdentificacaoTributo.PadLeft(2, ' ').Substring(0, 2) : naoUtilizarDeafultParaPagamentoDeTributos ? "  " : "16") +//Código de Identificação do Tributo
                                   (!string.IsNullOrWhiteSpace(codigoIdentificacaoTributo) ? codigoIdentificacaoTributo.PadLeft(2, ' ').Substring(0, 2) : "  ") +//Código de Identificação do Tributo
                                                                                                                                                                 //(titulo.Titulo.PeriodoApuracao.HasValue && titulo.Titulo.PeriodoApuracao.Value > DateTime.MinValue ? titulo.Titulo.PeriodoApuracao.Value.ToString("yyyyMMdd") : naoUtilizarDeafultParaPagamentoDeTributos ? "        " : titulo.Titulo.DataEmissao.Value.ToString("ddMMyyyy")) + //Período de Apuração
                                   (periodoApracao.HasValue && periodoApracao.Value > DateTime.MinValue ? periodoApracao.Value.ToString(formatodata) : "        ") + //Período de Apuração
                                                                                                                                                                    //(bancoSantander ? Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(17, '0').Substring(0, 17) : "".PadRight(17, '0')) +//Número de Referência
                                   (!string.IsNullOrWhiteSpace(referencia) ? referencia.PadLeft(17, '0').Substring(0, 17) : "".PadRight(17, '0')) +//Número de Referência
                                   Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor Principal
                                   "".PadRight(15, '0') +//Valor da Multa
                                   "".PadRight(15, '0') +//Juros / Encargos
                                   titulo.Titulo.DataVencimento.Value.ToString("ddMMyyyy") + //Data de Vencime
                                   "".PadRight(18, ' ') + //Uso Exclusivo FEBRABAN/CNAB
                                   "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno 231 a 240
                            }
                        }
                    }
                    else
                    {
                        string codigoFinalidadeTED = "00018";
                        if (titulo.Titulo.TipoMovimento != null && !string.IsNullOrWhiteSpace(titulo.Titulo.TipoMovimento.CodigoFinalidadeTED))
                            codigoFinalidadeTED = titulo.Titulo.TipoMovimento.CodigoFinalidadeTED;
                        else if (!string.IsNullOrWhiteSpace(pagamentoEletronico.BoletoConfiguracao.CodigoFinalidadeTED))
                            codigoFinalidadeTED = pagamentoEletronico.BoletoConfiguracao.CodigoFinalidadeTED;

                        string numeroConta = titulo.Titulo.Fornecedor.NumeroConta;
                        string digitoConta = pagamentoEletronico.BoletoConfiguracao.DigitoConta;
                        if (layoutCecred && numeroConta.Contains("-") && numeroConta.Split('-').Length >= 1)
                        {
                            digitoConta = numeroConta.Split('-')[1];
                            numeroConta = numeroConta.Split('-')[0];
                        }

                        //Registro A
                        x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                            qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                            "3" + //Tipo de Registro 8 a 8
                            qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 13
                            "A" + //digo de Segmento no Reg. Detalhe 14 a 14
                            "0" + //Tipo de Movimento 15 17
                            (pagamentoEletronico.BoletoConfiguracao.CodigoInstrucaoMovimento?.PadLeft(2, '0') ?? "00") + //Código da Instrução p/ Movimento 18 a 20
                            (layoutItau ? "008" : boletosBancoProprio == true ? "000" : "018") + //Código da Câmara Centralizadora 21 a 23
                            titulo.Titulo.Fornecedor.Banco?.Numero.ToString().PadLeft(3, '0') + //Código do Banco do Favorecido 24 a 43
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Fornecedor.Agencia).PadLeft(5, '0').Substring(0, 5) +//Ag. Mantenedora da Cta do Favor.
                            (bancoSantander || layoutItau ? " " : Utilidades.String.OnlyNumbers(titulo.Titulo.Fornecedor.DigitoAgencia ?? " ").PadLeft(1, ' ').Substring(0, 1)) +//Dígito Verificador da Agência
                            Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(numeroConta)).PadLeft(13, '0').Substring(0, 13) +//Número da Conta Corrente - AQUI ADICINEI COM A CONTA Dígito Verificador da Agência                        
                            (layoutItau ? digitoConta.PadLeft(1, ' ').Substring(0, 1) : " ") + //Dígito Verificador da AG/Conta
                            Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)).PadRight(30, ' ').Substring(0, 30) +//Nome do Favorecido
                            (!string.IsNullOrWhiteSpace(titulo.Titulo.NossoNumero) ? Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero).PadLeft(20, '0').Substring(0, 20) : Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString()).PadLeft(20, '0').Substring(0, 20)) +//Nº do Docum. Atribuído p/ Empresa
                            titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy") + //Data do Pagamento
                            (layoutItau ? "REA" : "BRL") + //Tipo da Moeda 102 a 104
                            "".PadRight(15, '0') + //Quantidade da Moeda 105 a 119
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do Pagamento 120 a 134
                            "".PadRight(20, ' ') +//Nº do Docum. Atribuído pelo Banco 135 a 149
                            (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDS_TEDSTR ? "".PadRight(8, '0') : titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy")) + //Data Real da Efetivação Pagto 155 a 162
                            (bancoSantander ? Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') : "".PadRight(15, '0')) + //Valor Real da Efetivação do Pagto 163 a 177
                                                                                                                                                                            //"C" + //Informação 2
                                                                                                                                                                            //"".PadRight(7, '0') + //Código Finalidade TED e Código Finalidade Complementar
                                                                                                                                                                            //"101" + //Uso Exclusivo FEBRABAN / CNAB
                            (layoutItau ?
                                ("".PadRight(14, '0') + //Nº NOTA FISCAL/CNPJ 178 a 191
                                "".PadRight(6, ' ') + //BRANCOS 192 a 197
                                Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString()).PadLeft(6, '0').Substring(0, 6) + //NDO DOCUMENTO 198 a 203
                                titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato.PadLeft(14, '0').Substring(0, 14) + //NDE INSCRIÇÃO 204 a 217
                                "01" + //TIPO DE IDENTIFICAÇÃO 218 a 218
                                "00010" + //FINALIDADE DO TED
                                "".PadRight(5, ' ')) //BRANCOS 219 a 229
                            : ("".PadRight(40, ' ') + //BRANCOS
                            (pagamentoEletronico.ModalidadePagamentoEletronico == ModalidadePagamentoEletronico.TDS_TEDSTR ? "  " :
                                pagamentoEletronico.TipoContaPagamentoEletronico == TipoContaPagamentoEletronico.TDSTransferencia ? "00" : "01") + //Compl. Tipo Serviço
                            (pagamentoEletronico.ModalidadePagamentoEletronico == ModalidadePagamentoEletronico.TDS_TEDSTR ? codigoFinalidadeTED : boletosBancoProprio == true ? "".PadRight(5, ' ') : "00010") + //Codigo finalidade da TED
                            (boletosBancoProprio == true ? "".PadRight(2, ' ') : titulo.Titulo.Fornecedor.TipoContaBanco == TipoContaBanco.Corrente ? "CC" : "PP") + //Compl. Finalidade de Pagamento
                            "".PadRight(3, ' '))) + //Uso Exclusivo FEBRABAN/CNAB
                            "0" + //Aviso ao Favorecido 230 a 230
                            "".PadRight(10, ' '))); // Códigos das Ocorrências p / Retorno 231 a 240

                        if (!layoutItau)
                        {
                            qtdRegistroLote += 1;

                            x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação
                                qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço
                                "3" + //Tipo de Registro
                                qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote
                                "B" + //digo de Segmento no Reg. Detalhe
                                "".PadRight(3, ' ') + //Uso Exclusivo FEBRABAN/CNAB
                                (titulo.Titulo.Fornecedor.Tipo == "J" ? "2" : "1") + //Tipo de Inscrição do Favorecido
                                titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato.PadLeft(14, '0') +//Nº de Inscrição do Favorecido
                                Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Endereco ?? "")).PadRight(30, ' ').Substring(0, 30) + //Nome da Rua, Av, Pça, Etc
                                Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Numero ?? "")).PadLeft(5, '0').Substring(0, 5) + //Nº do Local
                                Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Complemento ?? "")).PadRight(15, ' ').Substring(0, 15) + //Casa, Apto, Etc
                                Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Bairro ?? "")).PadRight(15, ' ').Substring(0, 15) + //Bairro
                                Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Localidade.Descricao ?? "")).PadRight(20, ' ').Substring(0, 20) + //Nome da Cidade
                                Utilidades.String.OnlyNumbers(titulo.Titulo.Fornecedor.CEP ?? "").Substring(0, 5) + //CEP
                                Utilidades.String.OnlyNumbers(titulo.Titulo.Fornecedor.CEP ?? "").Substring(5, 3) + //Complemento do CEP
                                titulo.Titulo.Fornecedor.Localidade.Estado.Sigla.PadRight(2, ' ') + //Sigla do Estado
                                titulo.Titulo.DataVencimento.Value.ToString("ddMMyyyy") +//Data do Vencimento (Nominal)
                                Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do Documento (Nominal)
                                "".PadRight(15, '0') + //Valor do Abatimento
                                Utilidades.String.OnlyNumbers(titulo.Titulo.Desconto.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do Desconto
                                "".PadRight(15, '0') + //Valor da Mora
                                Utilidades.String.OnlyNumbers(titulo.Titulo.Acrescimo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor da Multa
                                (titulo.Titulo.CodigoFavorecido ?? string.Empty).PadRight(15, ' ') + //Código/Documento do Favorecido
                                "0" + //Aviso ao Favorecido
                                (layoutItau ? "183   " : bancoSantander ? "183000" : "".PadRight(6, '0')) + //Uso Exclusivo para o SIAPE
                                (layoutItau ? "".PadRight(8, ' ') : "".PadRight(8, '0')))); //Código ISPB

                            if (pagamentoEletronico.DescricaoUsoEmpresaPagamentoEletronico != DescricaoUsoEmpresaPagamentoEletronico.Nenhum)
                            {
                                qtdRegistroLote += 1;

                                x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                                    qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                                    "3" + //Tipo de Registro 8 a 8                        
                                    (qtdRegistroLote).ToString("D").PadLeft(5, '0') +//Quantidade de Registros do Lote 9 a 13
                                    "5" + //Codigo de segmento do reg detalhe 14 a 14
                                    "".PadRight(3, ' ') + //Uso Exclusivo FEBRABAN/CNAB 15 a 17
                                    "".PadLeft(9, '0') + //Número da lista de debito 18 a 26
                                    "".PadLeft(6, '0') + //Horário do débito do pagamento 27 a 32
                                    "".PadLeft(5, '0') + //Complemento do tipo de servico 33 a 37
                                    "".PadLeft(5, '0') + //Mensage de segunda linha de extrato 38 a 42
                                    pagamentoEletronico.DescricaoUsoEmpresaPagamentoEletronico.ObterDescricao().PadRight(50, ' ') + //uso da empresa 43 a 92
                                    "5".PadLeft(3, '0') + //Tipo de documento 93 a 95
                                    "".PadLeft(15, '0') + //Numero do documento 96 a 110
                                    "".PadRight(2, ' ') + //Série do documento 111 a 112
                                    "".PadRight(15, ' ') + //Uso Exclusivo FEBRABAN/CNAB 113 a 127
                                    "".PadLeft(8, '0') + //Data emissao do documento 128 a 135
                                    "".PadRight(30, ' ') + //Nome reclamente ted 136 a 165
                                    "".PadRight(25, ' ') + //Numero proc ted 166 a 190
                                    "".PadLeft(15, '0') + //Pis pasep 191 a 205
                                    "".PadRight(25, ' ') + //Uso Exclusivo FEBRABAN/CNAB 136 a 230
                                    "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno                    
                            }
                        }
                    }
                }

                //Trailer do lote
                qtdRegistroLote += 2;
                if (layoutItau && tibutoComCodigoBarras)
                {
                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                        qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                        "5" + //Tipo de Registro 8 a 8
                        "".PadRight(9, ' ') + //Uso Exclusivo FEBRABAN/CNAB 9 a 17
                        qtdRegistroLote.ToString("D").PadLeft(6, '0') +//Quantidade de Registros do Lote 18 a 23
                        Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(18, '0') +//Quantidade de Registros do Lote  24 a 41
                        Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(15, '0') + //Somatória de Quantidade de Moedas 42 a 56                       
                        "".PadRight(184, ' ')));//BRANCOS 57 a 230 e 231 a 240
                }
                else if (layoutItau && tibutoSemCodigoBarras)
                {
                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                        qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                        "5" + //Tipo de Registro 8 a 8
                        "".PadRight(9, ' ') + //Uso Exclusivo FEBRABAN/CNAB 9 a 17
                        qtdRegistroLote.ToString("D").PadLeft(6, '0') +//Quantidade de Registros do Lote 18 a 23
                        Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//TOTAL VALOR PRINCIPAL 24 a 37
                        Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//TOTAL OUTRAS ENTIDAD. 38 a 51
                        Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//TOTAL VAL. ACRESCIMOS 52 a 65
                        Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(14, '0') +//TOTAL VALOR ARRECAD. 66 a 79                        
                        "".PadRight(161, ' ')));//BRANCOS 80 a 230 e 231 a 240
                }
                //else if (pagamentoEletronico.DescricaoUsoEmpresaPagamentoEletronico != DescricaoUsoEmpresaPagamentoEletronico.Nenhum)
                //{
                //    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                //        qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                //        "3" + //Tipo de Registro 8 a 8                        
                //        (qtdRegistroLote - 1).ToString("D").PadLeft(5, '0') +//Quantidade de Registros do Lote 9 a 13
                //        "5" + //Codigo de segmento do reg detalhe 14 a 14
                //        "".PadRight(3, ' ') + //Uso Exclusivo FEBRABAN/CNAB 15 a 17
                //        "".PadLeft(9, '0') + //Número da lista de debito 18 a 26
                //        "".PadLeft(6, '0') + //Horário do débito do pagamento 27 a 32
                //        "".PadLeft(5, '0') + //Complemento do tipo de servico 33 a 37
                //        "".PadLeft(5, '0') + //Mensage de segunda linha de extrato 38 a 42
                //        pagamentoEletronico.DescricaoUsoEmpresaPagamentoEletronico.ObterDescricao().PadRight(50, ' ') + //uso da empresa 43 a 92
                //        "5".PadLeft(3, '0') + //Tipo de documento 93 a 95
                //        "".PadLeft(15, '0') + //Numero do documento 96 a 110
                //        "".PadRight(2, ' ') + //Série do documento 111 a 112
                //        "".PadRight(15, ' ') + //Uso Exclusivo FEBRABAN/CNAB 113 a 127
                //        "".PadLeft(8, '0') + //Data emissao do documento 128 a 135
                //        "".PadRight(30, ' ') + //Nome reclamente ted 136 a 165
                //        "".PadRight(25, ' ') + //Numero proc ted 166 a 190
                //        "".PadLeft(15, '0') + //Pis pasep 191 a 205
                //        "".PadRight(25, ' ') + //Uso Exclusivo FEBRABAN/CNAB 136 a 230
                //        "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno                    

                //    qtdRegistroLote += 1;

                //    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                //        qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                //        "5" + //Tipo de Registro 8 a 8
                //        "".PadRight(9, ' ') + //Uso Exclusivo FEBRABAN/CNAB 9 a 17
                //        qtdRegistroLote.ToString("D").PadLeft(6, '0') +//Quantidade de Registros do Lote 18 a 23
                //        Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(18, '0') +//Quantidade de Registros do Lote  24 a 41
                //        "".PadLeft(18, '0') + //Somatória de Quantidade de Moedas
                //        (layoutItau ? "".PadLeft(6, ' ') : "".PadLeft(6, '0')) + //Número Aviso de Débito
                //        "".PadRight(165, ' ') + //Uso Exclusivo FEBRABAN/CNAB
                //        "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno
                //}
                else
                {
                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                        qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                        "5" + //Tipo de Registro 8 a 8
                        "".PadRight(9, ' ') + //Uso Exclusivo FEBRABAN/CNAB 9 a 17
                        qtdRegistroLote.ToString("D").PadLeft(6, '0') +//Quantidade de Registros do Lote 18 a 23
                        Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(18, '0') +//Quantidade de Registros do Lote  24 a 41
                        "".PadLeft(18, '0') + //Somatória de Quantidade de Moedas
                        (layoutItau ? "".PadLeft(6, ' ') : "".PadLeft(6, '0')) + //Número Aviso de Débito
                        "".PadRight(165, ' ') + //Uso Exclusivo FEBRABAN/CNAB
                        "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno
                }

                qtdRegistros += qtdRegistroLote;
            }
        }

        private static void GerarRegistroHederLote(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosBanco, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico,
            ref int qtdLotes, ref int qtdRegistroLote, ref decimal somaValores, ref int qtdRegistros, ref StreamWriter x, bool boletosBancoProprio)
        {
            bool bancoSantander = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Santander;
            bool layoutItau = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Itau;
            bool layoutBradesco = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bradesco;
            bool layoutSicoob = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bancoob;
            bool layoutCecred = pagamentoEletronico.BoletoConfiguracao.BoletoBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.BancoCECRED;
            //Registro Header de Lote
            if (titulosBanco != null && titulosBanco.Count > 0)
            {
                string versaoLote = "040";
                if (bancoSantander || layoutItau)
                    versaoLote = "030";
                if (layoutCecred)
                    versaoLote = "046";
                if (!string.IsNullOrWhiteSpace(pagamentoEletronico.BoletoConfiguracao.Modalidade))
                    versaoLote = pagamentoEletronico.BoletoConfiguracao.Modalidade;

                string indicativoFormaPagamento = "".PadRight(8, ' ');
                if (layoutCecred)
                    indicativoFormaPagamento = "01".PadRight(8, ' ');

                string formaPagamento = "";
                if (pagamentoEletronico.FormaLancamentoPagamentoEletronico.HasValue && pagamentoEletronico.FormaLancamentoPagamentoEletronico.Value != FormaLancamentoPagamentoEletronico.Padrao)
                    formaPagamento = pagamentoEletronico.FormaLancamentoPagamentoEletronico.Value.ObterNumero();

                string tipoServico = "  ";
                if (pagamentoEletronico.TipoServicoPagamentoEletronico.HasValue && pagamentoEletronico.TipoServicoPagamentoEletronico.Value != TipoServicoPagamentoEletronico.Padrao)
                    tipoServico = pagamentoEletronico.TipoServicoPagamentoEletronico.Value.ObterNumero();

                qtdLotes += 1;
                x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                    qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                    "1" + //Tipo de Registro 8 a 8
                    "C" + //Tipo da Operação 9 a 9
                    (!string.IsNullOrWhiteSpace(tipoServico) ? tipoServico : "20") + //Tipo do Serviço 10 a 11
                    (!string.IsNullOrWhiteSpace(formaPagamento) ? formaPagamento : boletosBancoProprio == true ? "30" : "31") + //Forma de Lançamento AQUI MUDA PARA 31 QNDO FOR DE OUTRO BANCO 12 a 13
                    versaoLote + //Nº da Versão do Layout do Lote 14 a 16
                    "".PadRight(1, ' ') + //Uso Exclusivo da FEBRABAN/CNAB 17 a 17
                    "2" + //Tipo de Inscrição da Empresa - CNPJ 18 a 18
                    pagamentoEletronico.Empresa.CNPJ_SemFormato + //Número de Inscrição da Empresa 19 a 32
                    (layoutItau ? "10".PadLeft(4, '0') + "".PadRight(16, ' ') : pagamentoEletronico.BoletoConfiguracao.NumeroConvenio.PadRight(20, ' ').Substring(0, 20)) + //Código do Convênio no Banco 33 a 36 e 37 a 52
                    pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(5, '0').Substring(0, 5) + //Agência Mantenedora da Conta 53 a 57
                    pagamentoEletronico.BoletoConfiguracao.DigitoAgencia.PadLeft(1, ' ').Substring(0, 1) + //Dígito Verificador da Agência 58 a 58
                    pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(12, '0').Substring(0, 12) + //Número da Conta Corrente 59 a 70
                    (layoutItau ? " " : pagamentoEletronico.BoletoConfiguracao.DigitoConta.PadLeft(1, '0').Substring(0, 1)) + //Dígito Verificador da Conta 71 a 71
                    (layoutItau ? pagamentoEletronico.BoletoConfiguracao.DigitoConta.PadLeft(1, '0').Substring(0, 1) : " ") + //Dígito Verificador da Ag/Conta 72 a 72
                    pagamentoEletronico.Empresa.RazaoSocial.PadRight(30, ' ').Substring(0, 30) + //Nome da Empresa 73 a 102
                    "".PadRight(40, ' ') + //Mensagem 103 a 132
                    pagamentoEletronico.Empresa.Endereco.PadRight(30, ' ').Substring(0, 30) + //Nome da Rua, Av, Pça, Etc 143 a 172
                    (layoutItau ? pagamentoEletronico.Empresa.Numero.PadLeft(5, '0').Substring(0, 5) : pagamentoEletronico.Empresa.Numero.PadRight(5, ' ').Substring(0, 5)) + //Número do Local 173 a 177
                    pagamentoEletronico.Empresa.Complemento.PadRight(15, ' ').Substring(0, 15) + //Casa, Apto, Sala, Etc 178 a 192
                    pagamentoEletronico.Empresa.Localidade.Descricao.PadRight(20, ' ').Substring(0, 20) + //Nome da Cidade 213 a 220
                    Utilidades.String.OnlyNumbers(pagamentoEletronico.Empresa.CEP ?? "").Substring(0, 5) + //CEP 213 a 220
                    Utilidades.String.OnlyNumbers(pagamentoEletronico.Empresa.CEP ?? "").Substring(5, 3) + //Complemento do CEP 213 a 220
                    pagamentoEletronico.Empresa.Localidade.Estado.Sigla.PadRight(2, ' ') + //Sigla do Estado 221 a 222
                    indicativoFormaPagamento + //Indicativo de forma de pagamento do serviço + CNAB 223 a 230
                    "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno 231 a 240

                qtdRegistroLote = 0;
                somaValores = 0;
                foreach (var titulo in titulosBanco)
                {
                    qtdRegistroLote += 1;
                    somaValores += titulo.Titulo.Saldo;
                    //Registro J
                    x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                        qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                        "3" + //Tipo de Registro 8 a 8
                        qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 15
                        "J" + //digo de Segmento no Reg. Detalhe 14 a 14
                        "0" + //Tipo de Movimento 15 a 17
                        (pagamentoEletronico.BoletoConfiguracao.CodigoInstrucaoMovimento?.PadLeft(2, '0') ?? "00") + //Código da Instrução p/ Movimento 15 a 17
                        Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero).PadRight(44, ' ').Substring(0, 44) +//Nº Seqüencial do Registro no Lote 18 a 20 e 21 a 21 e 22 a 22 e 23 a 26 e 27 a 36 e 37 a 61
                        Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)).PadRight(30, ' ').Substring(0, 30) +//Nome do Cedente 62 a 91
                        titulo.Titulo.DataVencimento.Value.ToString("ddMMyyyy") +//Data do Vencimento (Nominal) 92 a 99
                        Utilidades.String.OnlyNumbers((titulo.Titulo.ValorOriginal).ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do título 100 a 114
                        Utilidades.String.OnlyNumbers(titulo.Titulo.DescontoAplicadoNegociacao.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do Desconto + Abatimento 115 a 129
                        Utilidades.String.OnlyNumbers(titulo.Titulo.Acrescimo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor da Mora + Multa 130 a 144
                        titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy") + //Data do Pagamento 145 a 152
                        Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor do Pagamento 153 a 167
                        "".PadRight(15, '0') + //Quantidade da Moeda 168 a 182
                        Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")))).PadLeft(20, '0').Substring(0, 20) +//Nº do Docto Atribuído pela Empresa 183 a 202
                        "".PadRight(20, ' ') + //Nº do Docto Atribuído pelo Banco 183 a 202
                        (layoutItau ? "".PadRight(18, ' ') //203 a 215 e 216 a 230 e 231 a 240
                            : ("09" + //Código de Moeda
                            "".PadRight(6, ' ') + //Uso Exclusivo FEBRABAN/CNAB
                            "".PadRight(10, ' ')))));//Códigos das Ocorrências p/ Retorno

                    if (bancoSantander || layoutItau || layoutBradesco || layoutSicoob || layoutCecred)
                    {
                        if (!layoutItau)
                            qtdRegistroLote += 1;

                        x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação 1 a 3
                        qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço 4 a 7
                        "3" + //Tipo de Registro 8 a 8
                        qtdRegistroLote.ToString("D").PadLeft(5, '0') +//Nº Seqüencial do Registro no Lote 9 a 13
                        "J" + //Código Segmento do Registro Detalhe 14 a 14
                        (layoutItau ?
                            (pagamentoEletronico.BoletoConfiguracao.CodigoInstrucaoMovimento?.PadLeft(3, '0') ?? "000") :
                            " " + (pagamentoEletronico.BoletoConfiguracao.CodigoInstrucaoMovimento?.PadLeft(2, '0') ?? "00")
                        ) + //Código de Movimento Remessa 16 a 17
                        "52" + //Identificação Registro Opcional 18 a 19
                        "2" + //Tipo de Inscrição Sacado 20 a 20
                        pagamentoEletronico.Empresa.CNPJ_SemFormato.PadLeft(15, '0') + //CPF/NNPJ do Sacado 21 a 35
                        pagamentoEletronico.Empresa.RazaoSocial.PadRight(40, ' ').Substring(0, 40) + //Nome do Sacado 36 a 75
                        (titulo.Titulo.Fornecedor.Tipo == "J" ? "2" : "1") + //Tipo de Inscrição Cedente 76 a 76
                        titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato.PadLeft(15, '0') +//CPF/NNPJ do Cedente 77 a 91
                        Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)).PadRight(40, ' ').Substring(0, 40) + //Nome do Cedente 92 a 131
                        "2" + //Tipo de Inscrição Sacador 132 a 132
                        pagamentoEletronico.Empresa.CNPJ_SemFormato.PadLeft(15, '0') + //CPF/NNPJ do Sacador 133 a 147
                        pagamentoEletronico.Empresa.RazaoSocial.PadRight(40, ' ').Substring(0, 40) + //Nome do Sacador 148 a 187
                        " ".PadRight((layoutItau ? 53 : 53), ' ')));//Filler 188 a 240

                        if (layoutItau)
                            qtdRegistroLote += 1;
                    }
                }

                decimal quantidadeMoeda = 0;
                if (layoutCecred)
                    quantidadeMoeda = somaValores;

                //Trailer do lote
                qtdRegistroLote += 2;
                x.WriteLine(Utilidades.String.RemoveAccents(pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Código do Banco na Compensação
                    qtdLotes.ToString("D").PadLeft(4, '0') +//Lote de Serviço
                    "5" + //Tipo de Registro
                    "".PadRight(9, ' ') + //Uso Exclusivo FEBRABAN/CNAB
                    qtdRegistroLote.ToString("D").PadLeft(6, '0') +//Quantidade de Registros do Lote
                    Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(18, '0') +//Quantidade de Registros do Lote
                    Utilidades.String.OnlyNumbers(quantidadeMoeda.ToString("n2")).Replace(",", "").PadLeft(18, '0') + //Somatória de Quantidade de Moedas
                    (layoutItau ? "".PadLeft(6, ' ') : "".PadLeft(6, '0')) + //Número Aviso de Débito
                    "".PadRight(165, ' ') + //Uso Exclusivo FEBRABAN/CNAB
                    "".PadRight(10, ' ')));//Códigos das Ocorrências p/ Retorno

                qtdRegistros += qtdRegistroLote;
            }
        }

        #endregion

        #region 400

        public static bool GerarRemessaPagamentoCNAB400(int codigoPagamentoDigital, string stringConexao, ref MemoryStream arquivo, out string msgErro, out int numero, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            msgErro = "";
            numero = 0;

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigoPagamentoDigital);
            if (pagamentoEletronico == null)
            {
                msgErro = "Pagamento não localizado.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulos = repPagamentoEletronicoTitulo.BuscarPorPagamento(codigoPagamentoDigital);

            if (titulos == null || titulos.Count == 0)
            {
                msgErro = "Nenhum título encontrado.";
                return false;
            }
            numero = pagamentoEletronico.Numero;
            try
            {
                if (titulos.Count() > 0)
                {
                    if ((pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDC_TEDCip
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TT_TituloTerceiro
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo)
                        && titulos.Any(o => !string.IsNullOrWhiteSpace(o.Titulo.NossoNumero) && o.Titulo.NossoNumero.Length != 44))
                    {
                        msgErro = "Existem títulos com o número do boleto inválido (diferente de 44 caracteres).";
                        return false;
                    }
                    if ((pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CC_CreditoContaCorrente
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.DOC_DOCCompre
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCR_CreditoConta
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDS_TEDSTR)
                        && titulos.Any(o => o.Titulo.Fornecedor.Banco == null || string.IsNullOrWhiteSpace(o.Titulo.Fornecedor.Agencia) || string.IsNullOrWhiteSpace(o.Titulo.Fornecedor.NumeroConta)))
                    {
                        msgErro = "Existem fornecedor sem Dados Bancários cadastrado.";
                        return false;
                    }

                    StreamWriter x;
                    arquivo = new MemoryStream();
                    int qtdRegistros = 0;
                    decimal somaValores = 0;

                    Encoding utf8WithoutBom = new UTF8Encoding(false);
                    x = new StreamWriter(arquivo, utf8WithoutBom);

                    //Header

                    if (pagamentoEletronico.ModalidadePagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos
                        && pagamentoEletronico.ModalidadePagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo)
                        x.WriteLine(Utilidades.String.RemoveAccents(
                            "0" + //Cód. Registro 1 até 1
                            "1" + //Cód. Arquivo  2 até 2
                            "REMESSA".PadLeft(7, ' ').Substring(0, 7) + //Ident. Arquivo 3 até 9
                            "11" + //Cód. Serviço 10 até 11
                            "PAGTOS FORNECED".PadLeft(15, ' ').Substring(0, 15) + //Ident. Serviço 12 até 26
                            pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(8, '0').Substring(0, 8) + //Número Conta 27 até 34
                            "N" + //Valida trailler 35 a 35
                            "".PadRight(2, ' ') + //Brancos 36 a 37
                            pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(7, '0').Substring(0, 7) +//Cód. Agência 38 a 44
                            "".PadRight(2, ' ') + //Brancos 45 a 46
                            pagamentoEletronico.Empresa.RazaoSocial.PadRight(30, ' ').Substring(0, 30) + //Nome Cliente 47 a 76
                            pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//Bco. Deposit 77 a 79
                            pagamentoEletronico.BoletoConfiguracao.DescricaoBanco.PadRight(15, ' ').Substring(0, 15) + //Nome Banco 80 a 94
                            pagamentoEletronico.DataGeracao.Value.ToString("ddMMyy") + //Data Gravação 95 a 100
                            "".PadRight(1, ' ') + //Origem 101 a 101
                            "".PadRight(2, ' ') + //Terceiro 102 a 103
                            "".PadRight(284, ' ') + //Brancos 104 a 387
                            "N" + // Identificação CNPJ/CPF  388 a 388
                            pagamentoEletronico.Numero.ToString("D").PadLeft(6, '0').Substring(0, 6) + //Num.arquivo 389 a 394
                            "000001")); // Num. Seqüencial Registro 395 a 400

                    if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDC_TEDCip
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TT_TituloTerceiro)
                    {
                        GerarRegistroBoletoBancario(titulos, pagamentoEletronico, ref somaValores, ref qtdRegistros, ref x, true);
                    }
                    else if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CC_CreditoContaCorrente
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.DOC_DOCCompre
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCR_CreditoConta
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDS_TEDSTR)
                    {
                        GerarRegistroDOCTEDCC(titulos, pagamentoEletronico, ref somaValores, ref qtdRegistros, ref x, true);
                    }
                    else if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos
                        || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo)
                    {
                        GerarRegistroArrecadacoes(titulos, pagamentoEletronico, ref somaValores, ref qtdRegistros, ref x, true, naoUtilizarDeafultParaPagamentoDeTributos);
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Modalidade de pagamento não disponível para o layout CNAB400");
                        return false;
                    }

                    //Trailer
                    if (pagamentoEletronico.ModalidadePagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos
                       && pagamentoEletronico.ModalidadePagamentoEletronico != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo)
                        x.WriteLine(
                            Utilidades.String.RemoveAccents(
                                "9" + //Cód. Registro 1 a 1
                                "".PadRight(123, ' ') + //Brancos 2 a 124
                                Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Valor tota 125 a 139
                                "".PadRight(109, ' ') + //Brancos 140 a 248
                                Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Total Abatimento 249 a 263
                                "".PadRight(82, ' ') + //Brancos 264 a 345
                                Utilidades.String.OnlyNumbers(0.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Total Juros de Mora 346 a 360
                                "".PadRight(4, ' ') + //Brancos 361 a 364
                                Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//Total Valor Autorizado 365 a 379
                                "".PadRight(15, ' ') + //Brancos 380 a 394
                                qtdRegistros.ToString("D").PadLeft(6, '0') //Num. Seqüencial Registro 395 a 400
                                )
                            );

                    x.Flush();
                    arquivo.Position = 0;
                    string mensagemRetorno = string.Empty;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por FTP: " + ex.ToString());
                return false;
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }

            return true;
        }

        private static void GerarRegistroBoletoBancario(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosBanco, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico,
            ref decimal somaValores, ref int qtdRegistros, ref StreamWriter x, bool boletosBancoProprio)
        {

            if (titulosBanco != null && titulosBanco.Count > 0)
            {
                somaValores = 0;
                qtdRegistros = 2;
                foreach (var titulo in titulosBanco)
                {
                    somaValores += titulo.Titulo.Saldo;
                    string codigoBarras = Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero);
                    string codigoBanco = !string.IsNullOrWhiteSpace(codigoBarras) && codigoBarras.Length > 3 ? codigoBarras.Substring(0, 3) : string.Empty;

                    x.WriteLine(
                        Utilidades.String.RemoveAccents(
                            "1" + //Cód. Registro 1 a 1 
                            "01" + //Cód. Inscrição 2 a 3
                            pagamentoEletronico.BoletoConfiguracao.NumeroConvenio.PadLeft(14, '0').Substring(0, 14) + //Num. Inscrição 4 a 17
                            pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(8, '0').Substring(0, 8) + //Número Conta  18 a 25
                            "".PadRight(3, ' ') + //Brancos 26 a 28
                            pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(7, '0').Substring(0, 7) + //Cód. Agência 29 a 35
                            "".PadRight(2, ' ') + //Brancos 36 a 37
                            "".PadRight(25, ' ') + //Uso Exclusivo 38  a 62
                            titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato.PadLeft(14, '0') +//Cód. Fornecedor 63 a 76
                            "OUT" + //Tipo de Documento  77 a 79
                            titulo.Titulo.Codigo.ToString().PadRight(10, ' ').Substring(0, 10) +//Num. Compromisso 80 a 89
                            titulo.Titulo.Sequencia.ToString().PadRight(1, ' ').Substring(0, 1) +//Seq. Compromisso  90 a 90
                                                                                                 //"".PadRight(10, ' ') +//Num. Compromisso 80 a 89
                                                                                                 //"".PadRight(1, ' ') +//Seq. Compromisso  90 a 90
                            "".PadRight(17, ' ') +//Brancos 91 a 107
                            "C" + //Cód. Operação 108 a 108
                            "01" + //Cód. Ocorrência 109 a 110
                            titulo.Titulo.Codigo.ToString().PadRight(10, ' ') + //Seu Numero Compromisso 111 a 120
                            titulo.Titulo.DataVencimento.Value.ToString("ddMMyy") +//Vencimento 121 a 126
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(13, '0') +//Valor 127 a 138
                            "COB" + //Tipo Pagamento
                            codigoBanco.PadRight(3, '0') + //Bco. Destino 143 a 145
                            "0".PadRight(7, '0') +//Ag. Destino 146 a 152
                            "0".PadRight(3, '0') + //Zeros 153 a 155
                            "0".PadRight(10, '0') + //Conta Corrente 156 a 165
                            "".PadRight(20, ' ') + //Brancos 166 a 185
                            pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(7, '0').Substring(0, 7) + //Agência Pagamento 186 a 192
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString("D")).PadLeft(16, '0').Substring(0, 16) + //Nosso Número 193 a 208
                            "".PadRight(39, ' ') + //Brancos 209 a 247
                            codigoBanco.PadRight(3, '0') + //Bco. Portador 248 a 250
                            "0".PadRight(13, '0') + //Abatimento 251 a 263
                            Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)).PadRight(30, ' ').Substring(0, 30) + //Fornecedor 264 a 293
                            "".PadRight(10, ' ') + //Brancos 294 a 303
                            codigoBarras.PadLeft(44, '0').Substring(0, 44) + //Cód de Barras 304 a 347
                            "0".PadRight(13, '0') + //Juros de Mora 348 a 360
                            titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyy") + //Data Pagamento 361 a 366
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(13, '0') +//Vlr Autorizado 367 a 379
                            "R$".PadRight(4, ' ') +//Moeda 380 A 383 
                            "".PadRight(4, ' ') + //Carteira 384 A 387
                            "".PadRight(3, ' ') + //Espécie Documento 388 A 390
                            "".PadRight(4, ' ') + //Brancos 391 a 394
                            qtdRegistros.ToString("D").PadLeft(6, '0')//Numero Seqüencial Registro 395 a 400
                            )
                            );
                    qtdRegistros += 1;
                }
            }
        }

        private static void GerarRegistroDOCTEDCC(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosBanco, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico,
            ref decimal somaValores, ref int qtdRegistros, ref StreamWriter x, bool boletosBancoProprio)
        {

            if (titulosBanco != null && titulosBanco.Count > 0)
            {
                somaValores = 0;
                qtdRegistros = 2;
                string tipoPagamento = "";
                if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CC_CreditoContaCorrente
                    || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCR_CreditoConta)
                    tipoPagamento = "CC";
                else if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.DOC_DOCCompre)
                    tipoPagamento = "DOC";
                else if (pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TDS_TEDSTR
                    || pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos)
                    tipoPagamento = "TED";

                foreach (var titulo in titulosBanco)
                {
                    somaValores += titulo.Titulo.Saldo;
                    string codigoBarras = Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero);
                    string codigoBanco = !string.IsNullOrWhiteSpace(codigoBarras) && codigoBarras.Length > 3 ? codigoBarras.Substring(0, 3) : string.Empty;

                    x.WriteLine(
                        Utilidades.String.RemoveAccents(
                            "1" + //Cód. Registro 1 a 1 
                            "01" + //Cód. Inscrição 2 a 3
                            pagamentoEletronico.BoletoConfiguracao.NumeroConvenio.PadLeft(14, '0').Substring(0, 14) + //Num. Inscrição 4 a 17
                            pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(8, '0').Substring(0, 8) + //Número Conta  18 a 25
                            "".PadRight(3, ' ') + //Brancos 26 a 28
                            pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(7, '0').Substring(0, 7) + //Cód. Agência 29 a 35
                            "".PadRight(2, ' ') + //Brancos 36 a 37
                            "".PadRight(25, ' ') + //Uso Exclusivo 38  a 62
                            titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato.PadLeft(14, '0') +//Cód. Fornecedor 63 a 76
                            "OUT" + //Tipo de Documento  77 a 79
                            titulo.Titulo.Codigo.ToString().PadRight(10, ' ').Substring(0, 10) +//Num. Compromisso 80 a 89
                            titulo.Titulo.Sequencia.ToString().PadRight(1, ' ').Substring(0, 1) +//Seq. Compromisso  90 a 90
                            "".PadRight(16, ' ') +//Brancos 91 a 107
                            (titulo.Titulo.Fornecedor.Tipo == "J" ? "2" : "1") + //Ident. CNPJ /CPF 107 a 107
                            "C" + //Cód. Operação 108 a 108
                            "01" + //Cód. Ocorrência 109 a 110
                            titulo.Titulo.Codigo.ToString().PadRight(10, ' ').Substring(0, 10) + //Seu Numero Compromisso 111 a 120
                            titulo.Titulo.DataVencimento.Value.ToString("ddMMyy") +//Vencimento 121 a 126
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(13, '0') +//Valor 127 a 138
                            tipoPagamento.PadRight(3, ' ') +//Tipo Pagamento 140 a 142
                            (tipoPagamento == "CC" ? pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') : titulo.Titulo.Fornecedor.Banco?.Numero.ToString().PadLeft(3, '0')) + //Bco. Destino 143 a 145
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Fornecedor.Agencia).PadLeft(7, '0').Substring(0, 7) +//Ag. Destino 146 a 152
                            "0".PadRight(3, '0') + //Zeros 153 a 155
                            Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.NumeroConta)).PadLeft(10, '0').Substring(0, 10) +//Conta Corrente 156 a 165
                            titulo.Titulo.Fornecedor.Banco?.Descricao.ToString().PadLeft(20, ' ') + //Agência Destino 166 a 185
                            pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.PadLeft(7, '0').Substring(0, 7) + //Agência Pagamento 186 a 192
                            "".PadRight(16, ' ') + //Brancos 193 a 208
                            "999".PadLeft(8, '0') + //Código ISPB 209 a 216
                            "".PadRight(31, ' ') + //Brancos 217 a 247
                            "0".PadRight(16, '0') + //Zeros 248 a 263
                            Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)).PadRight(30, ' ').Substring(0, 30) + //Fornecedor 264 a 293
                            "".PadRight(54, ' ') + //Brancos 294 a 347
                            "0".PadRight(13, '0') + //Zeros 348 a 360
                            titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyy") + //Data Pagamento 361 a 366
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(13, '0') +//Vlr Autorizado 367 a 379
                            "R$".PadRight(4, ' ') +//Moeda 380 A 383 
                            "".PadRight(8, ' ') + //Brancos 384 A 391
                            "".PadRight(1, ' ') + //Dig Ag Dest 392 a 392
                            "".PadRight(2, ' ') + //Brancos 393 a 394
                            qtdRegistros.ToString("D").PadLeft(6, '0') //Numero Seqüencial Registro 395 a 400
                            )
                        );
                    qtdRegistros += 1;
                }
            }
        }

        private static void GerarRegistroArrecadacoes(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosBanco, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico,
            ref decimal somaValores, ref int qtdRegistros, ref StreamWriter x, bool boletosBancoProprio, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            bool tibutoSemCodigoBarras = pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.TRB_Tributos;
            bool tibutoComCodigoBarras = pagamentoEletronico.ModalidadePagamentoEletronico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoEletronico.CCT_ContasConsumoTributo;

            x.WriteLine(Utilidades.String.RemoveAccents(
                            "0" + //Cód. Registro 1 até 1
                            "01" + //COD. INSCRIÇÃO 2 a 3
                            pagamentoEletronico.Empresa.CNPJ_SemFormato.PadLeft(14, '0').Substring(0, 14) + //NUM. INSCRIÇÃO 4 a 17
                            "1" + //COD. MOVIMENTO 18 a 19
                            "ARRECADACAO" + //IDENT.ARQUIVO 19 a 29
                            (pagamentoEletronico.BoletoConfiguracao.NumeroAgencia.TrimStart('0').PadLeft(5, '0').Substring(0, 5) + pagamentoEletronico.BoletoConfiguracao.NumeroConta.PadLeft(9, '0').Substring(0, 9)) + //COD. EMPRESA 30 a 43
                            pagamentoEletronico.Empresa.RazaoSocial.PadRight(30, ' ').Substring(0, 30) + //NOME EMPRESA 44 a 73
                            pagamentoEletronico.BoletoConfiguracao.NumeroBanco.PadLeft(3, '0') +//COD. BANCO 74 a 76
                            pagamentoEletronico.BoletoConfiguracao.DescricaoBanco.PadRight(11, ' ').Substring(0, 11) + //NOME BANCO 77 a 87
                            "".PadRight(4, ' ') + //Brancos 88 a 91
                            pagamentoEletronico.DataGeracao.Value.ToString("yyyMMdd") + //DT. GRAVAÇÃO 92 a 99
                            "".PadRight(8, ' ') + //IDENTIFICAÇÃO SAFRA 100 a 107
                            "".PadRight(284, ' ') + //Brancos 108 a 391
                            pagamentoEletronico.Numero.ToString("D").PadLeft(3, '0').Substring(0, 3) + //Num.arquivo 392 a 394
                            "000001")); // Num. Seqüencial Registro 395 a 400

            if (titulosBanco != null && titulosBanco.Count > 0)
            {
                somaValores = 0;
                qtdRegistros = 2;

                foreach (var titulo in titulosBanco)
                {
                    somaValores += titulo.Titulo.Saldo;
                    string codigoBarras = !string.IsNullOrWhiteSpace(titulo.Titulo.NossoNumero) ? Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero) : string.Empty;
                    string linha = "";
                    if (tibutoSemCodigoBarras)
                    {
                        string codigoReceitaTributo = "";
                        DateTime? periodoApracao = null;
                        Dominio.Entidades.Cliente contribuinte = null;

                        if (titulo.Titulo.Contribuinte != null)
                            contribuinte = titulo.Titulo.Contribuinte;
                        else if (!naoUtilizarDeafultParaPagamentoDeTributos && titulo.Titulo.ContribuinteTributo != null)
                            contribuinte = titulo.Titulo.ContribuinteTributo;

                        if (titulo.Titulo.TributoCodigoReceita != null)
                            codigoReceitaTributo = titulo.Titulo.TributoCodigoReceita.CodigoIntegracao;
                        else if (!naoUtilizarDeafultParaPagamentoDeTributos)
                            codigoReceitaTributo = "1708";

                        if (titulo.Titulo.PeriodoApuracao.HasValue)
                            periodoApracao = titulo.Titulo.PeriodoApuracao;
                        else if (!naoUtilizarDeafultParaPagamentoDeTributos)
                            periodoApracao = titulo.Titulo.DataEmissao;

                        linha =
                            (
                                //(titulo.Titulo.Contribuinte == null && naoUtilizarDeafultParaPagamentoDeTributos ? " " : titulo.Titulo.ContribuinteTributo.Tipo) + //TIPO DEIDENTIFICADOR 98 a 98
                                (contribuinte != null ? contribuinte.Tipo : " ") + //TIPO DEIDENTIFICADOR 98 a 98
                                                                                   //(titulo.Titulo.Contribuinte == null && naoUtilizarDeafultParaPagamentoDeTributos ? "0".PadLeft(14, '0') : titulo.Titulo.ContribuinteTributo.CPF_CNPJ_SemFormato.PadLeft(14, '0').Substring(0, 14)) + //IDENTIFICAÇÃO  99 a 112
                                (contribuinte != null ? contribuinte.CPF_CNPJ_SemFormato.PadLeft(14, '0').Substring(0, 14) : "0".PadLeft(14, '0')) + //IDENTIFICAÇÃO  99 a 112
                                                                                                                                                     //(titulo.Titulo.PeriodoApuracao.HasValue && titulo.Titulo.PeriodoApuracao.Value > DateTime.MinValue ? titulo.Titulo.PeriodoApuracao.Value.ToString("yyyyMMdd") : naoUtilizarDeafultParaPagamentoDeTributos ? "        " : titulo.Titulo.DataVencimento.Value.ToString("yyyyMMdd")) + //APURAÇÃO 113 a 120
                                (periodoApracao.HasValue && periodoApracao.Value > DateTime.MinValue ? periodoApracao.Value.ToString("yyyyMMdd") : "        ") + //APURAÇÃO 113 a 120
                                "".PadRight(17, ' ') + //REF-DARF 121 a 137
                                                       //(!string.IsNullOrWhiteSpace(titulo.Titulo.CodigoReceitaTributo) ? titulo.Titulo.CodigoReceitaTributo.PadLeft(4, ' ').Substring(0, 4) : naoUtilizarDeafultParaPagamentoDeTributos ? "    " : "1708") + //COD-RECEITA 138 a 141
                                (!string.IsNullOrWhiteSpace(codigoReceitaTributo) ? codigoReceitaTributo.PadLeft(4, ' ').Substring(0, 4) : "    ") + //COD-RECEITA 138 a 141
                                Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') + // VR-DARF 142 a 156
                                "0".PadLeft(15, '0') + //VR-MULTA 157 a 171
                                "0".PadLeft(15, '0') + //VR-JUROS 172 a 186
                                titulo.Titulo.DataVencimento.Value.ToString("yyyyMMdd") + //VENCIMENTO 187 a 194
                                "".PadRight(100, ' ') + //OBSERVAÇÃO 195 a 294
                                "".PadRight(17, ' ') + //BRANCOS 294 a 311
                                "".PadRight(80, ' ') + //OCORRÊNCIA 312 a 391
                                pagamentoEletronico.Numero.ToString("D").PadLeft(3, '0').Substring(0, 3) + //SEQUÊNCIA 392 a 394
                                qtdRegistros.ToString("D").PadLeft(6, '0')//SEQUÊNCIA 395 a 400
                            );
                    }
                    else
                    {
                        linha =
                            "L" + //TIPO DE ENTR 98 a 98
                            codigoBarras.PadLeft(48, ' ').Substring(0, 48) + //CÓDIGO DE BARRASCAMPO OBRIGATÓRIO 99 a 146
                            "".PadRight(165, ' ') + //BRANCOS 147 a 311
                            "".PadRight(80, ' ') + //OCORRÊNCIA 312 a 391
                            pagamentoEletronico.Numero.ToString("D").PadLeft(3, '0').Substring(0, 3) + //SEQUÊNCIA 392 a 394
                            qtdRegistros.ToString("D").PadLeft(6, '0'); //SEQUÊNCIA 395 a 400
                    }

                    x.WriteLine(
                        Utilidades.String.RemoveAccents(
                            "1" + //COD.REGISTRO 1 a 1 
                            "01" + //TIPO DE OPERAÇÃO 2 a 3
                            "0".PadRight(7, '0') + //NÚMERO DE CONTROLE SAFRA 4 a 10
                            "0".PadRight(8, '0') + //NOVA DATA DE PAGAMENTO 11 a 18
                            "0".PadRight(2, '0') + //COD.OCORRÊNCIA 19 a 20
                            (tibutoSemCodigoBarras ? "02" : "06") +// TIPO PAGAMENTO 21 as 22
                            titulo.Titulo.Codigo.ToString().PadRight(10, ' ').Substring(0, 10) + //NÚM CONTROLE CLIENTE 23 a 32
                            titulo.PagamentoEletronico.DataPagamento.Value.ToString("yyyyMMdd") + //DATA DE PAGTO 33 a 40
                            Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//VALOR TOTAL DE PAGAMENTO 41 a 55
                            (titulo.Titulo.Contribuinte == null && naoUtilizarDeafultParaPagamentoDeTributos ? "".PadRight(30, ' ') : (Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.ContribuinteTributo.Nome)).PadRight(30, ' ').Substring(0, 30))) + //NOME 56 a 85
                            "0".PadRight(4, '0') + //DDD TELEFONE 86 a 89
                            "0".PadRight(8, '0') + //NRO TELEFONE 90 a 97
                            linha
                            )
                        );
                    qtdRegistros += 1;
                }
            }

            x.WriteLine(
                Utilidades.String.RemoveAccents(
                    "9" + //COD. REGISTRO 1 a 1
                    "".PadRight(367, ' ') + //BRANCOS 2 a 368
                    (qtdRegistros - 2).ToString("D").PadLeft(8, '0') + //QUANTIDADE 369 a 376
                    Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", "").PadLeft(15, '0') +//VALOR TOTAL 377 a 391
                    pagamentoEletronico.Numero.ToString("D").PadLeft(3, '0').Substring(0, 3) + //NÚM. ARQUIVO 392 a 394
                    qtdRegistros.ToString("D").PadLeft(6, '0') //NUM. SEQÜENCIAL 395 a 400
                    )
                );
        }

        #endregion

        #region PIX
        public static bool GerarRemessaPagamentoPIX(int codigoPagamentoDigital, string stringConexao, ref MemoryStream arquivo, out string msgErro, out int numero, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            msgErro = "";
            numero = 0;

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigoPagamentoDigital);
            if (pagamentoEletronico == null)
            {
                msgErro = "Pagamento não localizado.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulos = repPagamentoEletronicoTitulo.BuscarPorPagamento(codigoPagamentoDigital);

            if (titulos == null || titulos.Count == 0)
            {
                msgErro = "Nenhum título encontrado.";
                return false;
            }

            try
            {
                if (titulos.Count() > 0)
                {
                    StreamWriter x;
                    arquivo = new MemoryStream();
                    int qtdRegistros = 1;
                    decimal somaValores = 0;

                    Encoding utf8WithoutBom = new UTF8Encoding(false);
                    x = new StreamWriter(arquivo, utf8WithoutBom);


                    //Header
                    x.WriteLine(Utilidades.String.RemoveAccents(
                        PadLeft("0", 1, '0') + //(001 A 001) Identificação do Registro 
                        PadLeft(pagamentoEletronico.BoletoConfiguracao.NumeroConvenio ?? "", 8, '0') + //(002 A 009)  Código de Comunicação –Identificação da Empresa no Banco 
                        PadLeft("2", 1, '0') + //(010 A 010) Tipo de Inscrição da Empresa Pagadora
                        PadLeft(pagamentoEletronico.Empresa.CNPJ_SemFormato, 15, '0') + //(011 A 025) CNPJ/CPF – Base da Empresa Pagadora
                        PadRight(Utilidades.String.RemoveAllSpecialCharacters(pagamentoEletronico.Empresa.RazaoSocial ?? ""), 40) + //(026 A 065) Nome da Empresa Pagadora 
                        PadLeft("20", 2, '0') + // (066 A 067) Tipo de Serviço
                        PadLeft("1", 1, '0') +  // (068 A 068)Código de origem do arquivo
                        PadLeft(pagamentoEletronico.Numero.ToString() ?? "", 5, '0') +  // (069 A 073) Número da Remessa
                        PadLeft("", 5, '0') + //(074 A 078) Número do Retorno
                        PadLeft(pagamentoEletronico.DataGeracao.Value.ToString("yyyyMMdd"), 8, '0') + // (079 A 086) Data de gravação do arquivo 
                        PadLeft(pagamentoEletronico.DataGeracao.Value.ToString("HHmmss"), 6, '0') + // (087 A 092) Hora gravação do arquivo
                        PadRight("", 5) + // (093 A 097) Densidade de gravação do arquivo/fita
                        PadRight("", 3) + // (098 A 100) Unidade de densidade da gravação do arquivo / fita
                        PadRight("", 5) + // (101 A 105) Identificação Módulo Micro 
                        PadLeft("", 1, '0') + // (106 A 106) Tipo de Processamento
                        PadRight("", 74) + // (107 A 180) Reservado empresa
                        PadRight("", 80) + // (181 A 260) Reservado – Banco
                        PadRight("", 217) + // (261 A 477) Reservado – Banco
                        PadRight("", 9) + // (478 A 486) Reservado – Banco
                        PadRight("", 5) + // (487 A 491) Reservado – Banco
                        PadRight("Pix", 3) + // (492 A 494) Pix
                        PadLeft(qtdRegistros.ToString(), 6, '0')// (495 A 500) Número Sequencial de Registro
                        ));

                    //Transação
                    foreach (var titulo in titulos)
                    {
                        qtdRegistros++;

                        var tipoContaBanco = titulo.Titulo.Fornecedor.TipoContaBanco == TipoContaBanco.Poupança ? 3 : titulo.Titulo.Fornecedor.TipoContaBanco == TipoContaBanco.Salario ? 2 : 1;

                        var tipochavepix = "01";

                        switch (titulo.Titulo.Fornecedor.TipoChavePix)
                        {
                            case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Email:
                                tipochavepix = "02";
                                break;
                            case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.CPFCNPJ:
                                tipochavepix = "03";
                                break;
                            case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Aleatoria:
                                tipochavepix = "04";
                                break;
                        }

                        x.WriteLine((Utilidades.String.RemoveAccents(
                           PadRight("2", 1) + // (001 A 001) Identificação
                           PadRight((titulo.Titulo?.Fornecedor?.Tipo == "J" ? "2" : "1"), 1) + // (002 A 002) Tipo de Inscrição do Fornecedor
                           PadLeft(titulo.Titulo?.Fornecedor?.CPF_CNPJ_SemFormato ?? "", 15, '0') + // (003 A 017) CNPJ/CPF do fornecedor  
                           PadRight(Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo?.Fornecedor?.Nome ?? "")), 30) + // (018 A 047)  Nome do Fornecedor 
                           PadLeft(titulo.Titulo?.Fornecedor?.Banco?.Numero.ToString() ?? "", 3, '0') + // (048 A 050)  Código do Banco do Fornecedor 
                           PadLeft(titulo.Titulo?.Fornecedor?.Agencia?.ToString() ?? "", 5, '0') + // (051 A 055)  Código da Agência do Fornecedor
                           PadLeft(titulo.Titulo?.Fornecedor?.DigitoAgencia?.ToString() ?? "", 1, '0') + // (056 A 056) Dígito da Agência do Fornecedor 
                           PadLeft(titulo.Titulo?.Fornecedor?.NumeroConta?.ToString() ?? "", 22, '0') + // (057 A 076) Conta corrente do Fornecedor
                           PadLeft(tipoContaBanco.ToString(), 2, '0') + // (079 A 080) Tipo de conta do fornecedor
                           PadRight(titulo.Titulo?.NumeroFatura.ToString() ?? "", 16) + // (081 A 096)  Número do Pagamento
                           PadLeft(Utilidades.String.OnlyNumbers((titulo.Titulo.ValorOriginal).ToString("n2") ?? "").Replace(",", ""), 15, '0') + // (097 A 111)  Valor do Pagamento
                           PadLeft("45", 2, '0') + // (112 A 113) Modalidade 
                           PadRight(tipochavepix, 3) + // (114 A 116)  Forma de iniciação esquerda zero
                           PadLeft((pagamentoEletronico.DataPagamento.HasValue ? pagamentoEletronico.DataPagamento.Value.ToString("yyyyMMdd") : ""), 8, '0') + // (117 A 124)  Data para efetivação do pagamento
                           PadLeft("60746948", 8, '0') + // (125 A 132)  ISPB- Favorecido
                           PadRight("", 32) + // (133 A 164) Código de Identificador da Transação
                           PadRight("", 35) + // (165 A 199) TX ID
                           PadRight("", 30) + // (200 A 229) Identificação do pagamento
                           PadLeft(titulo.Titulo?.Codigo.ToString() ?? "", 35) + // (230 A 264) Uso da Empresa
                           PadRight(titulo.Titulo?.Fornecedor?.ChavePix ?? "", 80) + // (265 A 344) Chave Pix/URL
                           PadLeft((titulo.Titulo.DataVencimento.HasValue ? titulo.Titulo.DataVencimento.Value.ToString("yyyyMMdd") : ""), 8, '0') + // (345 A 352) Data de Vencimento
                           PadLeft(Utilidades.String.OnlyNumbers(titulo.Titulo?.ValorOriginal.ToString("n2") ?? "").Replace(",", ""), 15, '0') + // (353 A 367) Valor do Documento
                           PadLeft(Utilidades.String.OnlyNumbers(titulo.Titulo?.DescontoAplicadoNegociacao.ToString("n2") ?? "").Replace(",", ""), 15, '0') + // (368 A 382) Valor do Desconto
                           PadLeft(Utilidades.String.OnlyNumbers(titulo.Titulo?.Acrescimo.ToString("n2") ?? "").Replace(",", ""), 15, '0') + // (383 A 397) Valor do Acréscimo (Mora + Multa)
                           PadLeft("2", 1, '0') + // (398 A 398) TIPO -INCRIÇÃO DEVEDOR
                           PadLeft(pagamentoEletronico.Empresa?.CNPJ_SemFormato ?? "", 15, '0') + // (399 A 413) CNPJ/CPF do devedor
                           PadRight(Utilidades.String.RemoveAllSpecialCharacters(pagamentoEletronico.Empresa?.RazaoSocial ?? ""), 30) + // (414 A 443) Nome do Devedor
                           PadRight("01", 2) + // (444 A 445) Situação do Agendamento 
                           PadRight("", 2) +  // (446 A 447) Informação de Retorno
                           PadRight("", 2) +  // (448 A 449) Informação de Retorno
                           PadRight("", 2) +  // (450 A 451) Informação de Retorno
                           PadRight("", 2) +  // (452 A 453) Informação de Retorno
                           PadRight("", 2) +  // (454 A 455) Informação de Retorno
                           PadRight("0", 1) +  // (456 A 456) Tipo de Movimento
                           PadRight("0", 2) +  // (457 A 458) Código do Movimento 
                           PadRight("", 1) +  // (459 A 459) Nível da Informação de Retorno 
                           PadRight("", 20) +  // (460 A 479) Reservado banco
                           PadRight("", 7) +  // (480 A 486) Conta complementar
                           PadRight("", 8) +  // (487 A 494 ) Reservado banco
                           PadLeft(qtdRegistros.ToString(), 6, '0') // (495 A 500 ) Número sequencial do registro

                       )).ToUpper());


                        somaValores += titulo.Titulo.Saldo;
                    }


                    //Trailler
                    x.WriteLine(Utilidades.String.RemoveAccents(
                        PadLeft("9", 1, '0') + //(001 A 001) Identificação do Registro
                        PadLeft(qtdRegistros.ToString("D"), 6, '0') + //(002 A 007) Quantidade de Registro 
                        PadLeft(Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", ""), 17, '0') + //(008 A 024) Total dos valores de pagamento
                        PadRight("", 470) + //(025 A 494) Reserva
                        PadLeft("1", 6, '0') //(495 A 500) Número Sequencial
                        )); ;

                    x.Flush();
                    arquivo.Position = 0;
                    string mensagemRetorno = string.Empty;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por FTP: " + ex.ToString());
                return false;
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }

            return true;
        }

        public static bool GerarRemessaPagamentoPIX240(int codigoPagamentoDigital, string stringConexao, ref MemoryStream arquivo, out string msgErro, out int numero, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            msgErro = "";
            numero = 0;
            string nomebanco = "";

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigoPagamentoDigital);
            if (pagamentoEletronico == null)
            {
                msgErro = "Pagamento não localizado.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulos = repPagamentoEletronicoTitulo.BuscarPorPagamento(codigoPagamentoDigital);

            if (titulos == null || titulos.Count == 0)
            {
                msgErro = "Nenhum título encontrado.";
                return false;
            }

            switch (pagamentoEletronico.BoletoConfiguracao.BoletoBanco)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bradesco:
                    msgErro = "";
                    nomebanco = "Banco Bradesco S.A.";
                    break;
                default:
                    msgErro = "Banco não homologado.";
                    return false;
            };

            try
            {
                if (titulos.Count() > 0)
                {
                    StreamWriter x;
                    arquivo = new MemoryStream();
                    int qtdRegistros = 0;
                    decimal somaValores = 0;
                    int qtdLotes = 0;
                    int qtdRegistroLote = 0;

                    Encoding utf8WithoutBom = new UTF8Encoding(false);
                    x = new StreamWriter(arquivo, utf8WithoutBom);

                    //Header de Arquivo
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.Header header = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.Header
                    {
                        CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                        LoteServico = "0000",
                        TipoRegistro = "0",
                        UsoExclusivoFebraban1 = "",
                        TipoInscricaoEmpresa = "2",
                        NumeroInscricaoEmpresa = pagamentoEletronico.Empresa.CNPJ_SemFormato,
                        CodigoConvenioBanco = pagamentoEletronico.BoletoConfiguracao.NumeroConvenio,
                        AgenciaMantenedoraConta = pagamentoEletronico.BoletoConfiguracao.NumeroAgencia,
                        DigitoVerificadorAgencia = pagamentoEletronico.BoletoConfiguracao.DigitoAgencia,
                        NumeroContaCorrente = pagamentoEletronico.BoletoConfiguracao.NumeroConta,
                        DigitoVerificadorConta = pagamentoEletronico.BoletoConfiguracao.DigitoConta,
                        DigitoVerificadorAgConta = pagamentoEletronico.BoletoConfiguracao.DigitoAgenciaConta,
                        NomeEmpresa = pagamentoEletronico.Empresa.RazaoSocial,
                        NomeBanco = nomebanco,
                        UsoExclusivoFebraban2 = "",
                        CodigoRemessaRetorno = "1",
                        DataGeracaoArquivo = pagamentoEletronico.DataGeracao.Value.ToString("ddMMyyyy"),
                        HoraGeracaoArquivo = pagamentoEletronico.DataGeracao.Value.ToString("hhMMss"),
                        NumeroSequencialArquivo = pagamentoEletronico.Numero.ToString(),
                        NumeroVersaoLayoutArquivo = "089",
                        DensidadeGravacao = "1600",
                        IdentificacaoRemessaPIX = "PIX",
                        UsoReservadoBanco = "",
                        UsoReservadoEmpresa = "",
                        UsoExclusivoFebraban3 = ""
                    };

                    x.WriteLine(Utilidades.String.RemoveAccents(header.Formatar()));

                    // Lote
                    GerarRegistroHederLoteCCPIX240(titulos, pagamentoEletronico, ref qtdLotes, ref qtdRegistroLote, ref somaValores, ref qtdRegistros, ref x, false, false);

                    qtdRegistros += 2;
                    //Trailer de Arquivo
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.Trailer trailer = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.Trailer
                    {
                        CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                        LoteServico = "9999",
                        TipoRegistro = "9",
                        UsoExclusivoFebraban1 = "",
                        QuantidadeLotesArquivo = qtdLotes.ToString("D"),
                        QuantidadeRegistrosArquivo = qtdRegistros.ToString("D"),
                        QtdeContasConcil = "",
                        UsoExclusivoFebraban2 = ""
                    };
                    
                    x.WriteLine(Utilidades.String.RemoveAccents(trailer.Formatar()));

                    x.Flush();
                    arquivo.Position = 0;
                    string mensagemRetorno = string.Empty;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por FTP: " + ex.ToString());
                return false;
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }

            return true;
        }

        public static bool GerarRemessaPagamentoPIX750(int codigoPagamentoDigital, string stringConexao, ref MemoryStream arquivo, out string msgErro, out int numero, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            msgErro = "";
            numero = 0;

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigoPagamentoDigital);
            if (pagamentoEletronico == null)
            {
                msgErro = "Pagamento não localizado.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulos = repPagamentoEletronicoTitulo.BuscarPorPagamento(codigoPagamentoDigital);

            if (titulos == null || titulos.Count == 0)
            {
                msgErro = "Nenhum título encontrado.";
                return false;
            }

            switch (pagamentoEletronico.BoletoConfiguracao.BoletoBanco)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bradesco:
                    msgErro = "";
                    break;
                default:
                    msgErro = "Banco não homologado.";
                    return false;
            };

            try
            {
                if (titulos.Count() > 0)
                {
                    StreamWriter x;
                    arquivo = new MemoryStream();
                    int qtdRegistros = 0;
                    decimal somaValores = 0;
                    int qtdLotes = 0;
                    int qtdRegistroLote = 0;

                    Encoding utf8WithoutBom = new UTF8Encoding(false);
                    x = new StreamWriter(arquivo, utf8WithoutBom);

                    //Header de Arquivo
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.Header header = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.Header
                    {
                        TipoRegistro = "0",
                        Operacao = "1",
                        LiteralRemessa = "REMESSA",
                        CodigoServico = "02",
                        LiteralServiço = "PIX",
                        IspbParticipante = "Ver o que precisa ser preenchido nesse campo",
                        TipoPessoaRecebedor = pagamentoEletronico.Empresa.Tipo == "J" ? "02" : "01",
                        CpfCnpj = pagamentoEletronico.Empresa.CNPJ_SemFormato,
                        Agencia = pagamentoEletronico.BoletoConfiguracao.NumeroAgencia,
                        Conta = pagamentoEletronico.BoletoConfiguracao.NumeroConta,
                        TipoConta = "CACC",// CACC - Conta - Corrente / Conta de Pagamento || SVGS - Conta de Poupança
                        ChavePix = "",
                        DataGeracao = pagamentoEletronico.DataGeracao.Value.ToString("yyyyMMdd"),
                        CodigoConvenio = pagamentoEletronico.BoletoConfiguracao.NumeroConvenio,
                        ExclusivoPspRecebedor = "",
                        NomeRecebedor = pagamentoEletronico.Empresa.RazaoSocial,
                        Brancos = "",
                        NumeroSequencialRemessa = pagamentoEletronico.Numero.ToString(),
                        VersaoArquivo = "0002",
                        NumeroSequencialRegistro = "ver qual é o sequencial do registro"
                    };

                    x.WriteLine(Utilidades.String.RemoveAccents(header.Formatar()));

                    // Lote
                    GerarRegistroHederLoteCCPIX750(titulos, pagamentoEletronico, ref qtdLotes, ref qtdRegistroLote, ref somaValores, ref qtdRegistros, ref x, false, false);

                    //Trailer de Arquivo
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.Trailer trailer = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.Trailer
                    {
                        TipoRegistro = "9",
                        Brancos = "",
                        ValorTotal = Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", ""),
                        QtdeDetalhes = qtdRegistros.ToString("D"),
                        NumeroSequencial = "ver numero sequencial"
                    };                   

                    qtdRegistros += 2;
                    x.WriteLine(Utilidades.String.RemoveAccents(trailer.Formatar()));

                    x.Flush();
                    arquivo.Position = 0;
                    string mensagemRetorno = string.Empty;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível realizar a integração por FTP: " + ex.ToString());
                return false;
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }

            return true;
        }

        private static void GerarRegistroHederLoteCCPIX240(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosBanco, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico,
            ref int qtdLotes, ref int qtdRegistroLote, ref decimal somaValores, ref int qtdRegistros, ref StreamWriter x, bool boletosBancoProprio, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            string tipoServico = "  ";
            string versaoLayout = "";
            string codigoCamaraCentralizadora = "";

            switch (pagamentoEletronico.BoletoConfiguracao.BoletoBanco)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bradesco:
                    versaoLayout = "045";
                    tipoServico = "20";
                    codigoCamaraCentralizadora = "009";
                    break;
                default:
                    versaoLayout = "  ";
                    tipoServico = "20";
                    codigoCamaraCentralizadora = "018";
                    break;
            }

            if (titulosBanco != null && titulosBanco.Count > 0)
            {

                qtdLotes += 1;
                //Registro Header de Lote
                Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.HeaderLote headerLote = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.HeaderLote
                {
                    CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                    LoteServico = qtdLotes.ToString("D"),
                    TipoRegistro = "1",
                    TipoOperacao = "C",
                    TipoServico = tipoServico,
                    FormaLancamento = pagamentoEletronico.FormaLancamentoPagamentoEletronico.Value == FormaLancamentoPagamentoEletronico.PIXQRCode ? "47" : "45",
                    NumeroVersaoLayoutLote = versaoLayout,
                    UsoExclusivoFebraban1 = "",
                    TipoInscricaoEmpresa = "2",
                    NumeroInscricaoEmpresa = pagamentoEletronico.Empresa.CNPJ_SemFormato,
                    CodigoConvenioBanco = pagamentoEletronico.BoletoConfiguracao.NumeroConvenio,
                    AgenciaMantenedoraConta = pagamentoEletronico.BoletoConfiguracao.NumeroAgencia,
                    DigitoVerificadorAgencia = pagamentoEletronico.BoletoConfiguracao.DigitoAgencia,
                    NumeroContaCorrente = pagamentoEletronico.BoletoConfiguracao.NumeroConta,
                    DigitoVerificadorConta = pagamentoEletronico.BoletoConfiguracao.DigitoConta,
                    DigitoVerificadorAgConta = " ",
                    NomeEmpresa = pagamentoEletronico.Empresa.RazaoSocial,
                    Mensagem = "",
                    NomeRuaAvPcaEtc = pagamentoEletronico.Empresa.Endereco,
                    NumeroLocal = pagamentoEletronico.Empresa.Numero,
                    CasaAptoSalaEtc = pagamentoEletronico.Empresa.Complemento,
                    NomeCidade = pagamentoEletronico.Empresa.Localidade.Descricao,
                    CEP = Utilidades.String.OnlyNumbers(pagamentoEletronico.Empresa.CEP ?? ""),
                    ComplementoCEP = Utilidades.String.OnlyNumbers(pagamentoEletronico.Empresa.CEP ?? ""),
                    SiglaEstado = pagamentoEletronico.Empresa.Localidade.Estado.Sigla,
                    IndicativoFormaPagamentoServico = "01",
                    UsoExclusivoFebraban2 = "",
                    CodigosOcorrenciasRetorno = ""
                };

                x.WriteLine(Utilidades.String.RemoveAccents(headerLote.Formatar()));

                qtdRegistroLote = 0;
                somaValores = 0;
                foreach (var titulo in titulosBanco)
                {
                    qtdRegistroLote += 1;
                    somaValores += titulo.Titulo.Saldo;

                    string numeroConta = titulo.Titulo.Fornecedor.NumeroConta;
                    string digitoConta = pagamentoEletronico.BoletoConfiguracao.DigitoConta;

                    //Segmento A

                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.DetalheSegmentoA detalheSegmentoA = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.DetalheSegmentoA
                    {
                        CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                        LoteServico = qtdLotes.ToString("D"),
                        TipoRegistro = "3",
                        NumeroSequencialRegistroLote = qtdRegistroLote.ToString("D"),
                        CodigoSegmentoRegDetalhe = "A",
                        TipoMovimento = "0",
                        CodigoInstrucaoMovimento = pagamentoEletronico.BoletoConfiguracao?.CodigoInstrucaoMovimento ?? "",
                        CodigoCamaraCentralizadora = codigoCamaraCentralizadora,
                        CodigoBancoFavorecido = "",
                        AgMantenedoraCtaFavor = "",
                        DigitoVerificadorAgencia = "",
                        NumeroContaCorrente = "",
                        DigitoVerificadorConta = "",
                        DigitoVerificadorAgConta = "",
                        NomeFavorecido = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)),
                        NumeroDocumAtribuidoEmpresa = !string.IsNullOrWhiteSpace(titulo.Titulo.NossoNumero) ? Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero) : Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString()),
                        DataPagamento = titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy"),
                        TipoMoeda = "BRL",
                        QuantidadeMoeda = "",
                        ValorPagamento = Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", ""),
                        NumeroDocumAtribuidoBanco = "",
                        DataRealEfetivacaoPagto = "",
                        ValorRealEfetivacaoPagto = "",
                        OutrasInformacoes = "",
                        ComplTipoServico = "",
                        CodigoFinalidadeTED = "",
                        ComplFinalidadePagamento = "",
                        UsoExclusivoFebraban1 = "",
                        AvisoFavorecido = "",
                        CodigosOcorrenciasRetorno = ""
                    };

                    x.WriteLine(Utilidades.String.RemoveAccents(detalheSegmentoA.Formatar())); // Códigos das Ocorrências p / Retorno 231 a 240

                    // Segmento B 
                    var tipochavepix = "05";
                    switch (titulo.Titulo.Fornecedor?.TipoChavePix)
                    {
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Celular:
                            tipochavepix = "01";
                            break;
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Email:
                            tipochavepix = "02";
                            break;
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.CPFCNPJ:
                            tipochavepix = "03";
                            break;
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Aleatoria:
                            tipochavepix = "04";
                            break;
                    }

                    qtdRegistroLote += 1;


                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.DetalheSegmentoB detalheSegmentoB = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.DetalheSegmentoB
                    {
                        CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                        LoteServico = qtdLotes.ToString("D"),
                        TipoRegistro = "3",
                        NumeroSequencialRegistroLote = qtdRegistroLote.ToString("D"),
                        CodigoSegmentoRegDetalhe = "B",
                        FormaIniciacao = tipochavepix,
                        TipoInscricaoFavorecido = titulo.Titulo.Fornecedor.Tipo == "J" ? "2" : "1",
                        NumeroInscricaoFavorecido = titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato,
                        Informacao10 = "",
                        Informacao11 = "",
                        Informacao12 = titulo.Titulo.Fornecedor.ChavePix,
                        UsoExclusivoSIAPE = "",
                        CodigoISPB = ""
                    };

                    x.WriteLine(Utilidades.String.RemoveAccents(detalheSegmentoB.Formatar()));

                    if (pagamentoEletronico.FormaLancamentoPagamentoEletronico.Value == FormaLancamentoPagamentoEletronico.PIXQRCode)
                    {
                        qtdRegistroLote += 1;

                        //Segmento J-52 para PIX
                        Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.DetalheSegmentoJ52Pix detalheSegmentoJ52Pix = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.DetalheSegmentoJ52Pix
                        {
                            CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                            LoteServico = qtdLotes.ToString("D"),
                            TipoRegistro = "3",
                            NumeroSequencialRegistroLote = qtdRegistroLote.ToString("D"),
                            CodigoSegmentoRegDetalhe = "J",
                            UsoExclusivoFebraban1 = "",
                            CodigoMovimentoRemessa = pagamentoEletronico.BoletoConfiguracao.CodigoInstrucaoMovimento ?? "",
                            IdentificacaoRegistroOpcional = "52",
                            TipoInscricaoDevedor = "2",
                            NumeroInscricaoDevedor = pagamentoEletronico.Empresa.CNPJ_SemFormato,
                            NomeDevedor = Utilidades.String.RemoveAllSpecialCharacters(pagamentoEletronico.Empresa.RazaoSocial),
                            TipoInscricaoFavorecido = titulo.Titulo.Fornecedor.Tipo == "J" ? "2" : "1",
                            NumeroInscricaoFavorecido = titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato,
                            NomeFavorecido = Utilidades.String.RemoveAllSpecialCharacters(titulo.Titulo.Fornecedor.Nome),
                            UrlChaveEnderecamento = titulo.Titulo.Fornecedor.ChavePix,
                            CodigoIdentificacaoQrCode = ""
                        };

                        x.WriteLine(Utilidades.String.RemoveAccents(detalheSegmentoJ52Pix.Formatar()));
                    }
                }

                //Trailer do lote
                qtdRegistroLote += 2;

                Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.TrailerLote trailerLote = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab240.TrailerLote
                {
                    CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                    LoteServico = qtdLotes.ToString("D"),
                    TipoRegistro = "5",
                    UsoExclusivoFebraban1 = "",
                    QuantidadeRegistrosLote = qtdRegistroLote.ToString("D"),
                    SomatoriaValores = Utilidades.String.OnlyNumbers(somaValores.ToString("n2")).Replace(",", ""),
                    SomatoriaQuantidadeMoedas = "",
                    NumeroAvisoDebito = "",
                    UsoExclusivoFebraban2 = "",
                    CodigosOcorrenciasRetorno = ""
                };

                x.WriteLine(Utilidades.String.RemoveAccents(trailerLote.Formatar()));

                qtdRegistros += qtdRegistroLote;
            }
        }

        private static void GerarRegistroHederLoteCCPIX750(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> titulosBanco, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico,
            ref int qtdLotes, ref int qtdRegistroLote, ref decimal somaValores, ref int qtdRegistros, ref StreamWriter x, bool boletosBancoProprio, bool naoUtilizarDeafultParaPagamentoDeTributos)
        {
            string tipoServico = "  ";
            string versaoLayout = "";
            string codigoCamaraCentralizadora = "";

            switch (pagamentoEletronico.BoletoConfiguracao.BoletoBanco)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco.Bradesco:
                    versaoLayout = "045";
                    tipoServico = "20";
                    codigoCamaraCentralizadora = "009";
                    break;
                default:
                    versaoLayout = "  ";
                    tipoServico = "20";
                    codigoCamaraCentralizadora = "018";
                    break;
            }

            if (titulosBanco != null && titulosBanco.Count > 0)
            {
                qtdLotes += 1;
                qtdRegistroLote = 0;
                somaValores = 0;
                foreach (var titulo in titulosBanco)
                {
                    qtdRegistroLote += 1;
                    somaValores += titulo.Titulo.Saldo;

                    string numeroConta = titulo.Titulo.Fornecedor.NumeroConta;
                    string digitoConta = pagamentoEletronico.BoletoConfiguracao.DigitoConta;

                    //Segmento A
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.Detalhe detalhe = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.Detalhe
                    {
                        //CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                        //LoteServico = qtdLotes.ToString("D"),
                        //TipoRegistro = "3",
                        //NumeroSequencialRegistroLote = qtdRegistroLote.ToString("D"),
                        //CodigoSegmentoRegDetalhe = "A",
                        //TipoMovimento = "0",
                        //CodigoInstrucaoMovimento = pagamentoEletronico.BoletoConfiguracao?.CodigoInstrucaoMovimento ?? "",
                        //CodigoCamaraCentralizadora = codigoCamaraCentralizadora,
                        //CodigoBancoFavorecido = "",
                        //AgMantenedoraCtaFavor = "",
                        //DigitoVerificadorAgencia = "",
                        //NumeroContaCorrente = "",
                        //DigitoVerificadorConta = "",
                        //DigitoVerificadorAgConta = "",
                        //NomeFavorecido = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(titulo.Titulo.Fornecedor.Nome)),
                        //NumeroDocumAtribuidoEmpresa = !string.IsNullOrWhiteSpace(titulo.Titulo.NossoNumero) ? Utilidades.String.OnlyNumbers(titulo.Titulo.NossoNumero) : Utilidades.String.OnlyNumbers(titulo.Titulo.Codigo.ToString()),
                        //DataPagamento = titulo.PagamentoEletronico.DataPagamento.Value.ToString("ddMMyyyy"),
                        //TipoMoeda = "BRL",
                        //QuantidadeMoeda = "",
                        //ValorPagamento = Utilidades.String.OnlyNumbers(titulo.Titulo.Saldo.ToString("n2")).Replace(",", ""),
                        //NumeroDocumAtribuidoBanco = "",
                        //DataRealEfetivacaoPagto = "",
                        //ValorRealEfetivacaoPagto = "",
                        //OutrasInformacoes = "",
                        //ComplTipoServico = "",
                        //CodigoFinalidadeTED = "",
                        //ComplFinalidadePagamento = "",
                        //UsoExclusivoFebraban1 = "",
                        //AvisoFavorecido = "",
                        //CodigosOcorrenciasRetorno = ""
                    };

                    x.WriteLine(Utilidades.String.RemoveAccents(detalhe.Formatar())); // Códigos das Ocorrências p / Retorno 231 a 240

                    // Segmento B 
                    var tipochavepix = "05";
                    switch (titulo.Titulo.Fornecedor?.TipoChavePix)
                    {
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Celular:
                            tipochavepix = "01";
                            break;
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Email:
                            tipochavepix = "02";
                            break;
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.CPFCNPJ:
                            tipochavepix = "03";
                            break;
                        case Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Aleatoria:
                            tipochavepix = "04";
                            break;
                    }

                    qtdRegistroLote += 1;

                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.DetalheInformacoesAdicionais detalheInformacoesAdicionais = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.DetalheInformacoesAdicionais
                    {
                        //CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                        //LoteServico = qtdLotes.ToString("D"),
                        //TipoRegistro = "3",
                        //NumeroSequencialRegistroLote = qtdRegistroLote.ToString("D"),
                        //CodigoSegmentoRegDetalhe = "B",
                        //FormaIniciacao = tipochavepix,
                        //TipoInscricaoFavorecido = titulo.Titulo.Fornecedor.Tipo == "J" ? "2" : "1",
                        //NumeroInscricaoFavorecido = titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato,
                        //Informacao10 = "",
                        //Informacao11 = "",
                        //Informacao12 = titulo.Titulo.Fornecedor.ChavePix,
                        //UsoExclusivoSIAPE = "",
                        //CodigoISPB = ""
                    };

                    x.WriteLine(Utilidades.String.RemoveAccents(detalheInformacoesAdicionais.Formatar()));


                    qtdRegistroLote += 1;

                    //Segmento J-52 para PIX
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.DetalheDadosEspecificosCobrancaVencimento detalheDadosEspecificosCobrancaVencimento = new Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix.Cnab750.DetalheDadosEspecificosCobrancaVencimento
                    {
                        //CodigoBancoCompensacao = pagamentoEletronico.BoletoConfiguracao.NumeroBanco,
                        //LoteServico = qtdLotes.ToString("D"),
                        //TipoRegistro = "3",
                        //NumeroSequencialRegistroLote = qtdRegistroLote.ToString("D"),
                        //CodigoSegmentoRegDetalhe = "J",
                        //UsoExclusivoFebraban1 = "",
                        //CodigoMovimentoRemessa = pagamentoEletronico.BoletoConfiguracao.CodigoInstrucaoMovimento ?? "",
                        //IdentificacaoRegistroOpcional = "52",
                        //TipoInscricaoDevedor = "2",
                        //NumeroInscricaoDevedor = pagamentoEletronico.Empresa.CNPJ_SemFormato,
                        //NomeDevedor = Utilidades.String.RemoveAllSpecialCharacters(pagamentoEletronico.Empresa.RazaoSocial),
                        //TipoInscricaoFavorecido = titulo.Titulo.Fornecedor.Tipo == "J" ? "2" : "1",
                        //NumeroInscricaoFavorecido = titulo.Titulo.Fornecedor.CPF_CNPJ_SemFormato,
                        //NomeFavorecido = Utilidades.String.RemoveAllSpecialCharacters(titulo.Titulo.Fornecedor.Nome),
                        //UrlChaveEnderecamento = titulo.Titulo.Fornecedor.ChavePix,
                        //CodigoIdentificacaoQrCode = ""
                    };

                    x.WriteLine(Utilidades.String.RemoveAccents(detalheDadosEspecificosCobrancaVencimento.Formatar()));

                }                             

                qtdRegistros += qtdRegistroLote;
            }
        }

        private static string PadRight(string texto, int tamanho, char caractere = ' ')
        {
            if (texto.Length >= tamanho)
                return texto.Substring(0, tamanho);
            else
                return texto.PadRight(tamanho, caractere);
        }

        private static string PadLeft(string texto, int tamanho, char caractere = ' ')
        {
            if (texto.Length >= tamanho)
                return texto.Substring(0, tamanho);
            else
                return texto.PadLeft(tamanho, caractere);
        }
        #endregion
    }
}