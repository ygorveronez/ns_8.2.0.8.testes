using System;

namespace Servicos
{
    public class PagamentoMotorista
    {
        public static void CalcularImpostosMotorista(ref Dominio.Entidades.PagamentoMotorista pagamentoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PagamentoMotorista repPagamentoMotorista = new Repositorio.PagamentoMotorista(unitOfWork);
            Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);
            Repositorio.IRImpostoContratoFrete repIRImpostoContratoFrete = new Repositorio.IRImpostoContratoFrete(unitOfWork);
            Repositorio.INSSImpostoContratoFrete repINSSImpostoContratoFrete = new Repositorio.INSSImpostoContratoFrete(unitOfWork);

            DateTime dataInicial = pagamentoMotorista.DataPagamento.FirstDayOfMonth();
            DateTime dataFinal = pagamentoMotorista.DataPagamento.LastDayOfMonth();

            decimal valorFrete = pagamentoMotorista.ValorFrete;

            Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = repImpostoContratoFrete.BuscarPorEmpresaVigencia(pagamentoMotorista.Empresa.Codigo, DateTime.Now);

            if (impostoContratoFrete != null)
            {
                pagamentoMotorista.BaseAcumuladaINSS = (pagamentoMotorista.ValorFreteAcumulado * (impostoContratoFrete.PercentualBCINSS / 100));
                Dominio.Entidades.INSSImpostoContratoFrete iNSSImpostoContratoFrete = repINSSImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, pagamentoMotorista.BaseAcumuladaINSS);

                decimal baseCalculoINSS = valorFrete * (impostoContratoFrete.PercentualBCINSS / 100);
                //decimal baseINSSPaga = pagamentoMotorista.BaseAcumuladaINSS - baseCalculoINSS;

                if (iNSSImpostoContratoFrete != null)
                {
                    //decimal valorINSSJaPago = baseINSSPaga * (iNSSImpostoContratoFrete.PercentualAplicar / 100);
                    decimal valorINSSJaPago = pagamentoMotorista.ValorRetidoINSSAcumuladoContratado;
                    pagamentoMotorista.AliquotaINSS = iNSSImpostoContratoFrete.PercentualAplicar;
                    if (valorINSSJaPago < impostoContratoFrete.ValorTetoRetencaoINSS)
                    {                        
                        decimal valorINSS = baseCalculoINSS * (iNSSImpostoContratoFrete.PercentualAplicar / 100);
                        valorINSSJaPago += valorINSS;
                        if (valorINSSJaPago > impostoContratoFrete.ValorTetoRetencaoINSS)
                            pagamentoMotorista.ValorINSS = valorINSS - (valorINSSJaPago - impostoContratoFrete.ValorTetoRetencaoINSS);
                        else
                            pagamentoMotorista.ValorINSS = valorINSS;
                    }
                    else
                        pagamentoMotorista.ValorINSS = 0;

                    pagamentoMotorista.ValorINSS = Math.Round(pagamentoMotorista.ValorINSS, 2, MidpointRounding.ToEven);

                    pagamentoMotorista.AliquotaINSSContratante = iNSSImpostoContratoFrete.PercentualAplicarContratante;
                    pagamentoMotorista.ValorINSSContratante = Math.Round(baseCalculoINSS * (iNSSImpostoContratoFrete.PercentualAplicarContratante / 100), 2, MidpointRounding.ToEven);
                }               

                pagamentoMotorista.ValorDescontoIRRFDependentes = Math.Round(pagamentoMotorista.QuantidadeDependentes * impostoContratoFrete.ValorPorDependenteDescontoIRRF, 2, MidpointRounding.ToEven);

                pagamentoMotorista.AliquotaSEST = impostoContratoFrete.AliquotaSEST;
                pagamentoMotorista.ValorSEST = Math.Round((valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaSEST / 100), 2, MidpointRounding.ToEven);
                pagamentoMotorista.AliquotaSENAT = impostoContratoFrete.AliquotaSENAT;
                pagamentoMotorista.ValorSENAT = Math.Round((valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaSENAT / 100), 2, MidpointRounding.ToEven);

                pagamentoMotorista.ValorINSSSENAT = pagamentoMotorista.ValorINSS + pagamentoMotorista.ValorSEST;
                pagamentoMotorista.ValorSESTSENAT = pagamentoMotorista.ValorSEST + pagamentoMotorista.ValorSENAT;

                decimal valorINSSRetidoTotal = pagamentoMotorista.ValorRetidoINSSAcumuladoContratado + pagamentoMotorista.ValorINSS;
                decimal valorSESTSENATRetidoTotal = pagamentoMotorista.ValorRetidoSESTSENATAcumuladoContratado + pagamentoMotorista.ValorSESTSENAT; 
                pagamentoMotorista.BaseAcumuladaIR = (pagamentoMotorista.ValorFreteAcumulado * (impostoContratoFrete.PercentualBCIR / 100)) - (pagamentoMotorista.ValorDescontoIRRFDependentes + valorINSSRetidoTotal + valorSESTSENATRetidoTotal);
                Dominio.Entidades.IRImpostoContratoFrete irImpostoContratoFrete = repIRImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, pagamentoMotorista.BaseAcumuladaIR);
                if (irImpostoContratoFrete != null)
                {
                    pagamentoMotorista.AliquotaIR = irImpostoContratoFrete.PercentualAplicar;
                    pagamentoMotorista.ValorImpostoRenda = (pagamentoMotorista.BaseAcumuladaIR * (irImpostoContratoFrete.PercentualAplicar / 100)) - irImpostoContratoFrete.ValorDeduzir;
                    if (pagamentoMotorista.ValorImpostoRenda < 0m)
                        pagamentoMotorista.ValorImpostoRenda = 0m;
                    if (pagamentoMotorista.ValorImpostoRenda > 0 && pagamentoMotorista.ValorDeducaoImpostoRetidoFonteIRRF > 0)
                    {
                        pagamentoMotorista.ValorImpostoRenda = pagamentoMotorista.ValorImpostoRenda - pagamentoMotorista.ValorDeducaoImpostoRetidoFonteIRRF;
                        if (pagamentoMotorista.ValorImpostoRenda < 0m)
                            pagamentoMotorista.ValorImpostoRenda = 0m;
                    }

                    pagamentoMotorista.ValorImpostoRenda = Math.Round(pagamentoMotorista.ValorImpostoRenda, 2, MidpointRounding.ToEven);
                }

                bool contratadoAutonomo = Utilidades.Validate.ValidarCPF(pagamentoMotorista.Motorista.CPF);

                pagamentoMotorista.AliquotaINCRA = !contratadoAutonomo ? 0 : impostoContratoFrete.AliquotaINCRA;
                pagamentoMotorista.ValorINCRA = !contratadoAutonomo ? 0 : Math.Round((valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaINCRA / 100), 2, MidpointRounding.ToEven);
                pagamentoMotorista.AliquotaSalarioEducacao = !contratadoAutonomo ? 0 : impostoContratoFrete.AliquotaSalarioEducacao;
                pagamentoMotorista.ValorSalarioEducacao = !contratadoAutonomo ? 0 : Math.Round((valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaSalarioEducacao / 100), 2, MidpointRounding.ToEven);

                pagamentoMotorista.Status = "A";
                if (impostoContratoFrete.DataVigenciaInicio.HasValue && impostoContratoFrete.DataVigenciaInicio.Value > DateTime.MinValue)
                    pagamentoMotorista.Observacao = "Tabela vigência início " + impostoContratoFrete.DataVigenciaInicio.Value.ToString("dd/MM/yyyy");
                if (impostoContratoFrete.DataVigenciaFim.HasValue && impostoContratoFrete.DataVigenciaFim.Value > DateTime.MinValue)
                    pagamentoMotorista.Observacao = !string.IsNullOrWhiteSpace(pagamentoMotorista.Observacao) ? pagamentoMotorista.Observacao + " fim " + impostoContratoFrete.DataVigenciaFim.Value.ToString("dd/MM/yyyy") : "Tabela vigência fim " + impostoContratoFrete.DataVigenciaFim.Value.ToString("dd/MM/yyyy");

                repPagamentoMotorista.Atualizar(pagamentoMotorista);
            }
        }
    }
}
