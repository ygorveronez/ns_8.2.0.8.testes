using System;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Abastecimento
{
    public class ImportacaoCTF : ServicoBase
    {
        public ImportacaoCTF(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public static bool ObterAbastecimentosCTFEmpresa(int codigoEmpresa, string stringConexao, out string msgErro)
        {
            msgErro = "";

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);            

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();            

            if (empresa == null || empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.LoginCTF) || string.IsNullOrWhiteSpace(empresa.Configuracao.SenhaCTF) || empresa.Configuracao.QtdRegistroCTF <= 0 || empresa.Configuracao.CodTemplateCTF <= 0)
            {
                msgErro = "Empresa sem configuração para integração com o CTF";
                return false;
            }

            if (configuracaoAbastecimento == null || configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto == null)
            {
                msgErro = "Não possui configuração para movimentação contábil de abastecimento.";
                return false;
            }

            int ponteiroAtual = empresa.Configuracao.PonteiroCTF;
            int qtdRegistros = empresa.Configuracao.QtdRegistroCTF;

            ServicoCTF.SoapLogin svcLogin = new ServicoCTF.SoapLogin();
            svcLogin.login = empresa.Configuracao.LoginCTF;
            svcLogin.senha = empresa.Configuracao.SenhaCTF;            

            ServicoCTF.ParametrosCopia svcParametrosCopia;
            string retorno = string.Empty;

            while (string.IsNullOrWhiteSpace(retorno))
            {
                svcParametrosCopia = new ServicoCTF.ParametrosCopia();
                svcParametrosCopia.CodTemplate = empresa.Configuracao.CodTemplateCTF;
                svcParametrosCopia.Ponteiro = ponteiroAtual;
                svcParametrosCopia.QtdRegistro = qtdRegistros;

                ServicoCTF.WsCopiaSoapClient svcConsultaCTF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoCTF.WsCopiaSoapClient, ServicoCTF.WsCopiaSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.CTF_WsCopia);

                svcConsultaCTF.ClientCredentials.UserName.UserName = empresa.Configuracao.LoginCTF;
                svcConsultaCTF.ClientCredentials.UserName.Password = empresa.Configuracao.SenhaCTF;
                
                retorno = svcConsultaCTF.RecuperarCopia(svcLogin, svcParametrosCopia);
                if (string.IsNullOrWhiteSpace(retorno))
                {
                    Servicos.Log.TratarErro("Nenhum abastecimento retornado.");
                    return true;
                }
                Dominio.ObjetosDeValor.Embarcador.Abastecimento.ABASTECIMENTOS raizXML;
                try
                {
                    unidadeTrabalho.Start();
                    raizXML = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Embarcador.Abastecimento.ABASTECIMENTOS>(retorno);
                    if (raizXML.ABASTECIMENTOSRow == null || raizXML.ABASTECIMENTOSRow.Count() <= 0)
                    {
                        retorno = string.Empty;
                        Servicos.Log.TratarErro("Nenhum abastecimento retornado.");
                        return true;
                    }

                    try
                    {
                        Servicos.Log.TratarErro(retorno, "ImportacaoCTF");
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "ImportacaoCTF");
                    }

                    foreach (var ctf in raizXML.ABASTECIMENTOSRow)
                    {
                        Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
                        double cnpjPosto = 0;
                        double.TryParse(ctf.CGC, out cnpjPosto);

                        int km = 0, indice = 0;
                        int.TryParse(ctf.KM, out km);
                        int.TryParse(ctf.INDICE, out indice);

                        decimal quantidade = 0, total = 0;
                        decimal.TryParse(ctf.QTD, out quantidade);
                        decimal.TryParse(ctf.TOTAL, out total);

                        Servicos.Log.TratarErro(("Valor Total: " + ctf.TOTAL + " Ind.: " + ctf.INDICE + " Num.: " + ctf.NUMABAST), "ImportacaoCTF");

                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(ctf.PLACA);

                        abastecimento.Data = DateTime.Parse(ctf.DATA_ABASTECIMENTO);
                        abastecimento.Motorista = null;
                        abastecimento.Kilometragem = km;
                        abastecimento.Litros = quantidade;
                        abastecimento.NomePosto = ctf.POSTO_FANTASIA;
                        abastecimento.Pago = false;
                        abastecimento.Situacao = "A";
                        abastecimento.DataAlteracao = DateTime.Now;
                        abastecimento.Status = "A";
                        abastecimento.ValorUnitario = total / quantidade;
                        abastecimento.Veiculo = veiculo;
                        abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(cnpjPosto);
                        abastecimento.Produto = repProduto.BuscarPorPostoTabelaDeValor(cnpjPosto, ctf.C);
                        abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;
                        abastecimento.Documento = "Ind.: " + ctf.INDICE + " Num.: " + ctf.NUMABAST;
                        abastecimento.TipoRecebimentoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento.CTF;

                        Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                        if (indice > ponteiroAtual)
                            ponteiroAtual = indice;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                        if (abastecimento.Produto != null)
                        {
                            if (abastecimento.Produto.CodigoNCM.StartsWith("310210"))
                                tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                            else
                                tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                        }

                        abastecimento.TipoAbastecimento = tipoAbastecimento;
                        Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unidadeTrabalho, veiculo, null, configuracaoTMS);
                        if (abastecimento.Veiculo != null && abastecimento.Veiculo.Tipo == "T")
                            abastecimento.Situacao = "I";

                        if (abastecimento.Veiculo != null && abastecimento.Veiculo.TipoVeiculo == "1" && abastecimento.Veiculo.Equipamentos != null && abastecimento.Veiculo.Equipamentos.Count > 0)
                            abastecimento.Equipamento = abastecimento.Veiculo.Equipamentos[0];

                        if (abastecimento.Veiculo != null && abastecimento.Posto != null && abastecimento.Produto != null)
                        {
                            if (!repAbastecimento.ContemAbastecimento(abastecimento.Veiculo.Codigo, abastecimento.Kilometragem, abastecimento.Documento, abastecimento.Litros, abastecimento.TipoAbastecimento))
                                repAbastecimento.Inserir(abastecimento);
                        }
                        else if (abastecimento.Veiculo == null)
                        {
                            Servicos.Log.TratarErro("Veículo: " + ctf.PLACA + " não cadastrado. Indice: " + ctf.INDICE);
                        }
                        else if (abastecimento.Posto == null && abastecimento.Veiculo != null)
                        {
                            Servicos.Log.TratarErro("Posto: " + ctf.POSTO_FANTASIA + " CNPJ: " + cnpjPosto.ToString() + " não cadastrado. Indice: " + ctf.INDICE);
                            abastecimento.Situacao = "I";
                            if (!repAbastecimento.ContemAbastecimento(abastecimento.Veiculo.Codigo, abastecimento.Kilometragem, abastecimento.Documento, abastecimento.Litros, abastecimento.TipoAbastecimento))
                                repAbastecimento.Inserir(abastecimento);
                        }
                        else if (abastecimento.Produto == null && abastecimento.Veiculo != null && abastecimento.Posto != null)
                        {
                            Servicos.Log.TratarErro("Posto: " + abastecimento.Posto.Nome + " CNPJ: " + cnpjPosto.ToString() + " Código de Integração: " + ctf.C + " não cadastrado. Indice: " + ctf.INDICE);
                            abastecimento.Situacao = "I";
                            if (!repAbastecimento.ContemAbastecimento(abastecimento.Veiculo.Codigo, abastecimento.Kilometragem, abastecimento.Documento, abastecimento.Litros, abastecimento.TipoAbastecimento))
                                repAbastecimento.Inserir(abastecimento);
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Abastecimento sem Veículo: " + ctf.PLACA + " sem Posto " + ctf.POSTO_FANTASIA + " CNPJ: " + cnpjPosto.ToString() + " e sem Produto. Indice: " + ctf.INDICE);
                        }

                        if (abastecimento.Codigo > 0)
                        {
                            Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unidadeTrabalho, veiculo, null, configuracaoTMS);
                            abastecimento.Integrado = false;
                            repAbastecimento.Atualizar(abastecimento);
                        }
                    }

                    Servicos.Log.TratarErro("Fim Importacao CTF : " + ponteiroAtual.ToString());
                    empresa.Configuracao.PonteiroCTF = ponteiroAtual;
                    repEmpresa.Atualizar(empresa);

                    unidadeTrabalho.CommitChanges();
                    retorno = string.Empty;
                }
                catch (Exception ex)
                {
                    unidadeTrabalho.Rollback();
                    Servicos.Log.TratarErro("Problemas ao importar abastecimento via CTF " + ex.Message);
                    return false;
                }
            }
            return true;
        }

    }
}
