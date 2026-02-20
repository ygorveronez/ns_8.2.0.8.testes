using System;
using System.Net;
using System.ServiceModel.Web;
//Wusing Confluent.SchemaRegistry.Serdes;
using Emissao.Integracao.Rest.Base;
using Emissao.Integracao.Rest.Class;

namespace Emissao.Integracao.Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Imposto" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Imposto.svc or Imposto.svc.cs at the Solution Explorer and start debugging.
    public class Imposto : IImposto
    {
        public Retorno<int> CalcularImpostoMotorista(Dominio.ObjetosDeValor.ImpostoMotorista.ParametrosCalculo parametrosCalculo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("CalcularImpostoMotorista " + (parametrosCalculo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(parametrosCalculo) : string.Empty));

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unitOfWork);

                Dominio.Entidades.Empresa empresa = ValidarToken();
                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "Acesso não permitido, contate a empresa administradora", Status = false };

                string cpfCnpjContratado = Utilidades.String.OnlyNumbers(parametrosCalculo.CPF_CNPJ_Contratado);
                if (!Utilidades.Validate.ValidarCPF(cpfCnpjContratado) && !Utilidades.Validate.ValidarCNPJ(cpfCnpjContratado))
                    return new Retorno<int>() { Mensagem = "CPF/CNPJ do Contratado  " + parametrosCalculo.CPF_CNPJ_Contratado + " não é valido.", Status = false };

                Dominio.Entidades.Usuario contratado = repMotorista.BuscarMotoristaPorCPFEEmpresa(cpfCnpjContratado, empresa.Codigo);
                if (contratado == null)
                {
                    contratado = new Dominio.Entidades.Usuario();
                    contratado.CPF = parametrosCalculo.CPF_CNPJ_Contratado;
                    contratado.Nome = parametrosCalculo.CPF_CNPJ_Contratado;
                    contratado.Status = "A";
                    contratado.Tipo = "M";
                    contratado.Localidade = empresa.Localidade;
                    repMotorista.Inserir(contratado);
                }

                Dominio.Entidades.PagamentoMotorista pagamentoMotorista = new Dominio.Entidades.PagamentoMotorista();
                pagamentoMotorista.Empresa = empresa;
                pagamentoMotorista.Motorista = contratado;
                pagamentoMotorista.ValorFrete = parametrosCalculo.ValorFreteBruto;
                //pagamentoMotorista.BaseAcumuladaIR = parametrosCalculo.BaseAcumuladaIRRF;
                //pagamentoMotorista.BaseAcumuladaINSS = parametrosCalculo.BaseAcumuladaINSS;
                pagamentoMotorista.ValorFreteAcumulado = parametrosCalculo.ValorFreteAcumulado;
                pagamentoMotorista.Status = "P";
                pagamentoMotorista.DataPagamento = DateTime.Today;
                pagamentoMotorista.DataRecebimento = DateTime.Today;

                pagamentoMotorista.ValorRetidoINSSAcumuladoContratado = parametrosCalculo.ValorRetidoINSSAcumuladoContratado;
                pagamentoMotorista.ValorDeducaoImpostoRetidoFonteIRRF = parametrosCalculo.DeducaoImpostoRetidoFonteIRRF;
                pagamentoMotorista.QuantidadeDependentes = parametrosCalculo.QuantidadeDependentes;
                pagamentoMotorista.ValorRetidoSESTSENATAcumuladoContratado = parametrosCalculo.ValorRetidoSESTSENATAcumuladoContratado;

                pagamentoMotorista.Observacao = "Gerado por integração em " + DateTime.Now.ToString("dd/MM/yyy hh:mm:ss");
                repPagamentoMotorista.Inserir(pagamentoMotorista);

                //Calcular impostos contrato
                Servicos.PagamentoMotorista.CalcularImpostosMotorista(ref pagamentoMotorista, unitOfWork);

                return new Retorno<int>() { Mensagem = "", Status = true, Objeto = pagamentoMotorista.Codigo };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Falha genérica ao integrar dados.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados> ConsultarImpostosPorProtocolo(string protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unitOfWork);

                Dominio.Entidades.Empresa empresa = ValidarToken();

                if (empresa == null)
                    return new Retorno<Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados>() { Mensagem = "Acesso não permitido, contate a empresa administradora", Status = false };

                int.TryParse(protocolo, out int codigo);
                if (codigo == 0)
                    return new Retorno<Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados>() { Mensagem = "Protocolo obrigatório.", Status = false };

                Dominio.Entidades.PagamentoMotorista pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(0, codigo);

                if (pagamentoMotorista == null)
                    return new Retorno<Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados>() { Mensagem = "Valores não localizados para o protocolo " + protocolo, Status = false };

                if (pagamentoMotorista.Status == "P")
                    Servicos.PagamentoMotorista.CalcularImpostosMotorista(ref pagamentoMotorista, unitOfWork);

                return new Retorno<Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados>
                {
                    Mensagem = "",
                    Status = true,
                    Objeto = new Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados()
                    {
                        Protocolo = pagamentoMotorista.Codigo,
                        AliquotaINSSContratado = pagamentoMotorista.AliquotaINSS,
                        ValorINSSContratado = pagamentoMotorista.ValorINSS,
                        AliquotaIRRFContratado = pagamentoMotorista.AliquotaIR,
                        ValorIRRFContratado = pagamentoMotorista.ValorImpostoRenda,
                        AliquotaSESTContratado = pagamentoMotorista.AliquotaSEST,
                        ValorSENATContratado = pagamentoMotorista.ValorSENAT,
                        AliquotaSENATContratado = pagamentoMotorista.AliquotaSENAT,
                        ValorSESTContratado = pagamentoMotorista.ValorSEST,

                        AliquotaINSSContratante = pagamentoMotorista.AliquotaINSSContratante,
                        ValorINSSContratante = pagamentoMotorista.ValorINSSContratante,
                        AliquotaSESTContratante = pagamentoMotorista.AliquotaSEST,
                        ValorSENATContratante = pagamentoMotorista.ValorSENAT,
                        AliquotaSENATContratante = pagamentoMotorista.AliquotaSENAT,
                        ValorSESTContratante = pagamentoMotorista.ValorSEST,
                        //AliquotaSalarioEducacaoContratante = pagamentoMotorista.AliquotaSalarioEducacao,
                        //ValorSalarioEducacaoContratante = pagamentoMotorista.ValorSalarioEducacao,
                        //AliquotaINCRAContratante = pagamentoMotorista.AliquotaINCRA,
                        //ValorINCRAContratante = pagamentoMotorista.ValorINCRA
                        DescricaoTabelaUtilizada = pagamentoMotorista.Observacao,
                        ValorDescontoIRRFDependentes = pagamentoMotorista.ValorDescontoIRRFDependentes
                    }
                };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<Dominio.ObjetosDeValor.ImpostoMotorista.ImpostosCalculados>() { Mensagem = "Falha genérica ao consultar dados.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS> CalcularICMS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoICMS parametrosCalculo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CartaCorrecao)), "CartaCorrecao");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeNormal>)), "CTesCarga");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>)), "Carga");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar>)), "Ocorrencia");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual)), "CTeManual");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual)), "CTeAnulacao");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeManual)), "CTeSubstituto");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeCancelamento>)), "CTeCancelamento");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>)), "CTesFatura");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>)), "CancelarCTesFatura");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(List<Dominio.ObjetosDeValor.Embarcador.Integracao.EMP.CTeComplementar>)), "CancelarOcorrencia");

                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.WebService.Carga.Protocolos)), "Protocolos");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.WebService.CTe.CTe)), "CTe");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.WebService.Carga.Protocolos)), "Protocolos");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS)), "RetornoCalculoICMS");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoICMS)), "ParametroCalculoICMS");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS)), "RetornoCalculoISS");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoISS)), "ParametroCalculoISS");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS)), "RetornoCalculoPISCOFINS");
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoPISCOFINS)), "ParametroCalculoPISCOFINS");

                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao), false), "CargaIntegracao");//lembrar de remover o dynamic do FreteRota
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.WebService.Carga.Protocolos)), "Protocolos");

                //Servicos.Log.TratarErro(AvroSerializer.Create<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>().WriterSchema.ToString(), "CargaIntegracao");
                //Servicos.Log.TratarErro(AvroSerializer.Create<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>().WriterSchema.ToString());                

                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                if (parametrosCalculo == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Nenhum dado enviado.", Status = false };

                Servicos.Log.TratarErro("CalcularICMS " + (parametrosCalculo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(parametrosCalculo) : string.Empty));

                if (!ValidarTokenTMS())
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Acesso não permitido, verifique o token de integração.", Status = false };

                Dominio.Entidades.Estado ufEmissor = repEstado.BuscarPorSigla(parametrosCalculo.UFEmissor);
                if (ufEmissor == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Não foi localizado nenhum estado emissor", Status = false };

                Dominio.Entidades.Estado ufDestino = repEstado.BuscarPorSigla(parametrosCalculo.UFDestino);
                if (ufDestino == null && parametrosCalculo.UFDestino != "EX")
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Não foi localizado nenhum estado de destino", Status = false };

                Dominio.Entidades.Estado ufOrigem = repEstado.BuscarPorSigla(parametrosCalculo.UFOrigem);
                if (ufOrigem == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Não foi localizado nenhum estado de origem", Status = false };

                Dominio.Entidades.Estado ufTomador = repEstado.BuscarPorSigla(parametrosCalculo.UFTomador);
                if (ufTomador == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Não foi localizado nenhum estado do tomador", Status = false };

                Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

                bool incluirICMS = false;
                decimal percentualInclusaoICMS = 100;

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMSMultiTMS(parametrosCalculo.CodigoAtividade, ufEmissor, Dominio.Enumeradores.OpcaoSimNao.Nao, ufOrigem, ufDestino, ufTomador, parametrosCalculo.UFDestino == "EX", ref incluirICMS, ref percentualInclusaoICMS, parametrosCalculo.ValorTotal, unitOfWork);

                if (regraICMS == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Não foi possível calcular impostos.", Status = false };

                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS retornoCalculo = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS();

                retornoCalculo.IDProposta = parametrosCalculo.IDProposta;

                decimal baseCalculoICMS = 0;
                decimal valorICMS = 0;

                if (parametrosCalculo.ValorTotal > 0)
                {
                    baseCalculoICMS = parametrosCalculo.ValorTotal;
                    if (regraICMS.ValorBaseCalculoICMS == 0)
                        baseCalculoICMS = 0;
                    if (baseCalculoICMS > 0 && regraICMS.PercentualReducaoBC > 0)
                        baseCalculoICMS -= decimal.Round(baseCalculoICMS * (regraICMS.PercentualReducaoBC / 100), 2, MidpointRounding.ToEven);

                    if (regraICMS.Aliquota > 0 && baseCalculoICMS > 0)
                        valorICMS = baseCalculoICMS * (regraICMS.Aliquota / 100);
                }

                retornoCalculo.Aliquota = regraICMS.Aliquota;
                retornoCalculo.ValorImposto = valorICMS;
                retornoCalculo.ValorTotalComImposto = baseCalculoICMS;

                return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "", Status = true, Objeto = retornoCalculo };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoICMS>() { Mensagem = "Falha genérica ao integrar dados.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS> CalcularISS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoISS parametrosCalculo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (parametrosCalculo == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS>() { Mensagem = "Nenhum dado enviado.", Status = false };

                Servicos.Log.TratarErro("CalcularISS " + (parametrosCalculo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(parametrosCalculo) : string.Empty));

                if (!ValidarTokenTMS())
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS>() { Mensagem = "Acesso não permitido, verifique o token de integração.", Status = false };

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Contabeis.CalculoISS repCalculoISS = new Repositorio.Embarcador.Contabeis.CalculoISS(unitOfWork);

                Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigoIBGE(parametrosCalculo.IBGEOrigem);
                if (origem == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS>() { Mensagem = "Não foi localizado nenhuma localidade de origem", Status = false };

                Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigoIBGE(parametrosCalculo.IBGEDestino);
                if (destino == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS>() { Mensagem = "Não foi localizado nenhuma localidade de destino", Status = false };

                Dominio.Entidades.Embarcador.Contabeis.CalculoISS issOrigem = repCalculoISS.BuscarTabela(origem.Codigo, parametrosCalculo.CodigoServicoOrigem);
                Dominio.Entidades.Embarcador.Contabeis.CalculoISS issDestino = repCalculoISS.BuscarTabela(destino.Codigo, parametrosCalculo.CodigoServicoDestino);

                if (issDestino == null && issDestino == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS>() { Mensagem = "Não foi localizado nenhuma tabela de imposto de ISS configurado com estes parâmetros.", Status = false };

                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS retornoCalculo = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS();

                retornoCalculo.IDProposta = parametrosCalculo.IDProposta;
                retornoCalculo.AliquotaDestino = issDestino?.Aliquota ?? 0m;
                retornoCalculo.AliquotaOrigem = issOrigem?.Aliquota ?? 0m;
                retornoCalculo.ValorISS = retornoCalculo.AliquotaDestino > 0 ? ((parametrosCalculo.ValorTotal * retornoCalculo.AliquotaDestino) / 100) : 0m;
                retornoCalculo.ValorTotalComImposto = retornoCalculo.ValorISS > 0 ? retornoCalculo.ValorISS + parametrosCalculo.ValorTotal : 0m;

                return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS>() { Mensagem = "", Status = true, Objeto = retornoCalculo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoISS>() { Mensagem = "Falha genérica ao integrar dados.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS> CalcularPISCOFINS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoPISCOFINS parametrosCalculo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (parametrosCalculo == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS>() { Mensagem = "Nenhum dado enviado.", Status = false };

                Servicos.Log.TratarErro("CalcularPISCOFINS " + (parametrosCalculo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(parametrosCalculo) : string.Empty));

                if (!ValidarTokenTMS())
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS>() { Mensagem = "Acesso não permitido, verifique o token de integração.", Status = false };

                Repositorio.Embarcador.Contabeis.CalculoPisCofins repCalculoPisCofins = new Repositorio.Embarcador.Contabeis.CalculoPisCofins(unitOfWork);
                Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins calculoPisCofins = repCalculoPisCofins.BuscarConfiguracaoCalculoPisCofins();

                if (calculoPisCofins == null)
                    return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS>() { Mensagem = "Não foi localizado nenhuma tabela de imposto de PIS/COFINS.", Status = false };

                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS retornoCalculo = new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS();

                retornoCalculo.AliquotaCOFINS = parametrosCalculo.Isento ? 0m : calculoPisCofins.AliquotaCOFINS;
                retornoCalculo.AliquotaPIS = parametrosCalculo.Isento ? 0m : calculoPisCofins.AliquotaPIS;
                retornoCalculo.IDProposta = parametrosCalculo.IDProposta;
                retornoCalculo.ValorTotalCOFINS = parametrosCalculo.Isento ? 0m : parametrosCalculo.ValorServico + (parametrosCalculo.ValorServico * (calculoPisCofins.AliquotaCOFINS / 100));
                retornoCalculo.ValorTotalPIS = parametrosCalculo.Isento ? 0m : parametrosCalculo.ValorServico + (parametrosCalculo.ValorServico * (calculoPisCofins.AliquotaPIS / 100));

                return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS>() { Mensagem = "", Status = true, Objeto = retornoCalculo };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoCalculoPISCOFINS>() { Mensagem = "Falha genérica ao integrar dados.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private Dominio.Entidades.Empresa ValidarToken()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(
                Conexao.StringConexao,
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection headers = request.Headers;
                string tokenBaerer = headers["Authorization"];
                if (string.IsNullOrWhiteSpace(tokenBaerer))
                    return null;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorTokenIntegracao(tokenBaerer);

                return empresa;
            }
        }

        private bool ValidarTokenTMS()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(
                Conexao.StringConexao,
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection headers = request.Headers;
                string tokenBaerer = headers["Authorization"];
                if (string.IsNullOrWhiteSpace(tokenBaerer))
                    return false;

                try
                {
                    Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
                    Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorToken(tokenBaerer);

                    if (integradora != null && integradora.Ativo)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return false;
                }
            }
        }

        #endregion


    }
}
