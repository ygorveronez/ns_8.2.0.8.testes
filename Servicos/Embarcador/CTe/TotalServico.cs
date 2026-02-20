using Repositorio;
using System;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class TotalServico : ServicoBase
    {        
        public TotalServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ConverterCTeParaFreteValor(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.CTe.ComponenteFrete serComponenteFrete = new ComponenteFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            freteValor.FreteProprio = cte.ValorFreteSemICMS;
            freteValor.ValorPrestacaoServico = cte.ValorPrestacaoServico;
            freteValor.ValorTotalAReceber = cte.ValorAReceber;

            freteValor.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            freteValor.ICMS.IncluirICMSBC = true;
            freteValor.ICMS.PercentualInclusaoBC = 100;
            freteValor.ICMS.PercentualReducaoBC = cte.PercentualReducaoBaseCalculoICMS;
            freteValor.ICMS.ValorBaseCalculoICMS = cte.BaseCalculoICMS;
            freteValor.ICMS.Aliquota = cte.AliquotaICMS;
            freteValor.ICMS.ValorICMS = cte.ValorICMS;
            freteValor.ICMS.ValorCreditoPresumido = 0m;
            freteValor.ICMS.ExibirNaDacte = true;
            freteValor.ICMS.CST = cte.CST;
            freteValor.ICMS.SimplesNacional = cte.SimplesNacional;

            freteValor.ComponentesAdicionais = serComponenteFrete.ConverterComponenteCTeParaComponenteFrete(cte.CTeTerceiroComponentesFrete.ToList());

            return freteValor;
        }
        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ConverterCTeParaFreteValor(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.CTe.ComponenteFrete serComponenteFrete = new ComponenteFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            freteValor.FreteProprio = cte.ValorFrete;
            freteValor.ValorPrestacaoServico = cte.ValorPrestacaoServico;
            freteValor.ValorTotalAReceber = cte.ValorAReceber;

            freteValor.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            freteValor.ICMS.IncluirICMSBC = cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            freteValor.ICMS.PercentualInclusaoBC = cte.PercentualICMSIncluirNoFrete;
            freteValor.ICMS.PercentualReducaoBC = cte.PercentualReducaoBaseCalculoICMS;
            freteValor.ICMS.ValorBaseCalculoICMS = cte.BaseCalculoICMS;
            freteValor.ICMS.Aliquota = cte.AliquotaICMS;
            freteValor.ICMS.ValorICMS = cte.ValorICMS;
            freteValor.ICMS.ValorCreditoPresumido = cte.ValorPresumido;
            freteValor.ICMS.ExibirNaDacte = cte.ExibeICMSNaDACTE;
            freteValor.ICMS.CST = cte.CST;
            freteValor.ICMS.SimplesNacional = cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;

            freteValor.IBSCBS = new Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS
            {
                AliquotaCBS = cte.AliquotaCBS,
                AliquotaIBSEstadual = cte.AliquotaIBSEstadual,
                AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal,
                BaseCalculo = cte.BaseCalculoIBSCBS,
                ClassificacaoTributaria = cte.ClassificacaoTributariaIBSCBS,
                CST = cte.CSTIBSCBS,
                PercentualReducaoCBS = cte.PercentualReducaoCBS,
                PercentualReducaoIBSEstadual = cte.PercentualReducaoIBSEstadual,
                PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal,
                ValorCBS = cte.ValorCBS,
                ValorIBSEstadual = cte.ValorIBSEstadual,
                ValorIBSMunicipal = cte.ValorIBSMunicipal
            };

            if(cte.ValorTotalDocumentoFiscal > 0)
                 freteValor.ValorTotalDocumentoFiscal = cte.ValorTotalDocumentoFiscal;
            else if(cte.CSTIBSCBS != null)
            {
                decimal valorTotalDocumento = cte.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCSB = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterOutrasAliquotasIBSCBS(cte?.OutrasAliquotas?.Codigo ?? 0);
                if ((impostoIBSCSB?.SomarImpostosDocumento ?? false) || (cte?.OutrasAliquotas?.CalcularImpostoDocumento ?? false))
                    valorTotalDocumento = valorTotalDocumento + cte.ValorIBSMunicipal + cte.ValorIBSEstadual + cte.ValorCBS;

                freteValor.ValorTotalDocumentoFiscal = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero);
            }

            freteValor.ComponentesAdicionais = serComponenteFrete.ConverterComponenteCTeParaComponenteFrete(cte.ComponentesPrestacao.ToList());

            return freteValor;
        }


        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ConverterDynamicParaFreteValor(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.CTe.ComponenteFrete serComponenteFrete = new ComponenteFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            decimal valorFrete, valorPrestacaoServico, valorReceber, percentualICMSIncluir, percentualReducaoBaseCalculoICMS, baseCalculoICMS, aliquotaICMS, valorICMS, valorCredito;


            decimal.TryParse(dynCTe.TotalServico.ValorFrete.ToString(), out valorFrete);
            freteValor.FreteProprio = valorFrete;
            decimal.TryParse(dynCTe.TotalServico.ValorPrestacaoServico.ToString(), out valorPrestacaoServico);
            freteValor.ValorPrestacaoServico = valorPrestacaoServico;
            decimal.TryParse(dynCTe.TotalServico.ValorReceber.ToString(), out valorReceber);
            freteValor.ValorTotalAReceber = valorReceber;

            freteValor.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            freteValor.ICMS.IncluirICMSBC = ((string)dynCTe.TotalServico.IncluirICMSFrete).ToBool();
            decimal.TryParse(dynCTe.TotalServico.PercentualICMSIncluir.ToString(), out percentualICMSIncluir);
            freteValor.ICMS.PercentualInclusaoBC = percentualICMSIncluir;
            decimal.TryParse(dynCTe.TotalServico.PercentualReducaoBaseCalculoICMS.ToString(), out percentualReducaoBaseCalculoICMS);
            freteValor.ICMS.PercentualReducaoBC = percentualReducaoBaseCalculoICMS;
            decimal.TryParse(dynCTe.TotalServico.BaseCalculoICMS.ToString(), out baseCalculoICMS);
            freteValor.ICMS.ValorBaseCalculoICMS = baseCalculoICMS;
            decimal.TryParse(dynCTe.TotalServico.AliquotaICMS.ToString(), out aliquotaICMS);
            freteValor.ICMS.Aliquota = aliquotaICMS;
            decimal.TryParse(dynCTe.TotalServico.ValorICMS.ToString(), out valorICMS);
            freteValor.ICMS.ValorICMS = valorICMS;
            decimal.TryParse(dynCTe.TotalServico.ValorCredito.ToString(), out valorCredito);
            freteValor.ICMS.ValorCreditoPresumido = valorCredito;
            freteValor.ICMS.ExibirNaDacte = ((string)dynCTe.TotalServico.ExibirNaDACTE).ToBool();

            Dominio.Enumeradores.TipoICMS tipoICMS = (Dominio.Enumeradores.TipoICMS)dynCTe.TotalServico.ICMS;
            switch (tipoICMS)
            {
                case Dominio.Enumeradores.TipoICMS.ICMS_Normal_00:
                    freteValor.ICMS.CST = "00";
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Reducao_Base_Calculo_20:
                    freteValor.ICMS.CST = "20";
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Isencao_40:
                    freteValor.ICMS.CST = "40";
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Nao_Tributado_41:
                    freteValor.ICMS.CST = "41";
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Diferido_51:
                    freteValor.ICMS.CST = "51";
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60:
                    freteValor.ICMS.CST = "60";
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Outras_Situacoes_90:
                    freteValor.ICMS.CST = "91";
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90:
                    freteValor.ICMS.CST = "90";
                    break;
                case Dominio.Enumeradores.TipoICMS.Simples_Nacional:
                    freteValor.ICMS.SimplesNacional = true;
                    break;
                default:
                    break;
            }

            freteValor.ComponentesAdicionais = serComponenteFrete.ConverterDynamicParaComponenteFrete(dynCTe.Componentes);

            return freteValor;
        }

        public void SalvarTotaisCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor, Repositorio.UnitOfWork unitOfWork)
        {
            cte.ValorFrete = freteValor.FreteProprio;
            cte.ValorPrestacaoServico = freteValor.ValorPrestacaoServico;
            cte.ValorAReceber = freteValor.ValorTotalAReceber;
            cte.ExibeICMSNaDACTE = freteValor.ICMS.ExibirNaDacte;
            cte.IncluirICMSNoFrete = freteValor.ICMS.IncluirICMSBC ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.PercentualICMSIncluirNoFrete = freteValor.ICMS.PercentualInclusaoBC;
            cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.CST = freteValor.ICMS.CST;

            if (cte.CST == "40" || cte.CST == "41" || cte.CST == "51")
            {
                cte.AliquotaICMS = 0m;
                cte.PercentualReducaoBaseCalculoICMS = 0m;
                cte.BaseCalculoICMS = 0m;
                cte.ValorPresumido = 0m;
                cte.ValorICMS = 0m;
                cte.ValorICMSDevido = 0m;
            }
            else
            {
                cte.AliquotaICMS = freteValor.ICMS.Aliquota;
                cte.PercentualReducaoBaseCalculoICMS = freteValor.ICMS.PercentualReducaoBC;
                cte.BaseCalculoICMS = freteValor.ICMS.ValorBaseCalculoICMS;
                cte.ValorPresumido = freteValor.ICMS.ValorCreditoPresumido;
                cte.ValorICMS = freteValor.ICMS.ValorICMS;
            }
        }

        public void SalvarTotaisPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor, Repositorio.UnitOfWork unitOfWork)
        {

            preCTe.ValorFrete = freteValor.FreteProprio;
            preCTe.ValorPrestacaoServico = freteValor.ValorPrestacaoServico;
            preCTe.ValorAReceber = freteValor.ValorTotalAReceber;

            preCTe.IncluirICMSNoFrete = freteValor.ICMS.IncluirICMSBC ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            preCTe.PercentualICMSIncluirNoFrete = freteValor.ICMS.PercentualInclusaoBC;

            preCTe.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
            preCTe.CST = freteValor.ICMS.CST;
            preCTe.AliquotaICMS = freteValor.ICMS.Aliquota;
            preCTe.PercentualReducaoBaseCalculoICMS = freteValor.ICMS.PercentualReducaoBC;
            preCTe.BaseCalculoICMS = freteValor.ICMS.ValorBaseCalculoICMS;
            preCTe.ValorPresumido = freteValor.ICMS.ValorCreditoPresumido;
            preCTe.ValorICMS = freteValor.ICMS.ValorICMS;
            //preCTe.ValorICMSDevido = freteValor.ICMS.;//todo: ver esse valor

        }
    }
}
