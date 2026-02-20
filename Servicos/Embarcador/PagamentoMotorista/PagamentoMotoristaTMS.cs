using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Terceiros;
using System;

namespace Servicos.Embarcador.PagamentoMotorista
{
    public class PagamentoMotoristaTMS
    {

        #region Métodos Públicos Estáticos

        public static void CalcularImpostos(ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((pagamentoMotoristaTMS.PagamentoMotoristaTipo?.ReterImpostoPagamentoMotorista ?? false) && pagamentoMotoristaTMS.Carga != null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repconfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repconfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(pagamentoMotoristaTMS.Carga.Codigo);

                if (contratoFrete == null)
                    throw new ControllerException("Tipo do Pagamento configurado para reter impostos e carga não possui contrato de frete.");

                if (pagamentoMotoristaTMS.Terceiro == null)
                    pagamentoMotoristaTMS.Terceiro = contratoFrete.TransportadorTerceiro;

                CalcularImpostosParametros calcularImpostosParametros = new CalcularImpostosParametros();
                calcularImpostosParametros.origemCalcularImposto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCalcularImposto.PagamentoMotoristaTMS;
                calcularImpostosParametros.codigoContratoFrete = contratoFrete.Codigo;
                calcularImpostosParametros.codigoPagamentoMotoristaTMS = pagamentoMotoristaTMS.Codigo;
                calcularImpostosParametros.cpfCnpjTerceiro = pagamentoMotoristaTMS.Terceiro.CPF_CNPJ;
                calcularImpostosParametros.codigoTipoTerceiro = contratoFrete.TipoTerceiro?.Codigo;
                calcularImpostosParametros.valorTotalParaCalculo = pagamentoMotoristaTMS.Valor;

                Dominio.ObjetosDeValor.Embarcador.Terceiros.CalcularImpostosRetorno calculoImpostosRetorno = Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(calcularImpostosParametros, contratoFrete, configuracaoTMS, configuracaoContratoFreteTerceiro, unitOfWork, tipoServicoMultisoftware);

                pagamentoMotoristaTMS.BaseCalculoINSS = calculoImpostosRetorno.BaseCalculoINSS;
                pagamentoMotoristaTMS.AliquotaINSS = calculoImpostosRetorno.AliquotaINSS;
                pagamentoMotoristaTMS.ValorINSS = calculoImpostosRetorno.ValorINSS;

                pagamentoMotoristaTMS.BaseCalculoSEST = calculoImpostosRetorno.BaseCalculoSEST;
                pagamentoMotoristaTMS.AliquotaSEST = calculoImpostosRetorno.AliquotaSEST;
                pagamentoMotoristaTMS.ValorSEST = calculoImpostosRetorno.ValorSEST;

                pagamentoMotoristaTMS.BaseCalculoSENAT = calculoImpostosRetorno.BaseCalculoSENAT;
                pagamentoMotoristaTMS.AliquotaSENAT = calculoImpostosRetorno.AliquotaSENAT;
                pagamentoMotoristaTMS.ValorSENAT = calculoImpostosRetorno.ValorSENAT;

                pagamentoMotoristaTMS.BaseCalculoIRRF = calculoImpostosRetorno.BaseCalculoIRRF;
                pagamentoMotoristaTMS.AliquotaIRRF = calculoImpostosRetorno.AliquotaIRRF;
                pagamentoMotoristaTMS.ValorIRRF = calculoImpostosRetorno.ValorIRRF;

                pagamentoMotoristaTMS.BaseCalculoIRRFSemDesconto = calculoImpostosRetorno.BaseCalculoIRRFSemDesconto;
                pagamentoMotoristaTMS.BaseCalculoIRRFSemAcumulo = calculoImpostosRetorno.BaseCalculoIRRFSemAcumulo;
                pagamentoMotoristaTMS.ValorIRRFSemDesconto = calculoImpostosRetorno.ValorIRRFSemDesconto;
                pagamentoMotoristaTMS.ValorIRRFPeriodo = calculoImpostosRetorno.ValorIRRFPeriodo;

                pagamentoMotoristaTMS.AliquotaINSSPatronal = calculoImpostosRetorno.AliquotaINSSPatronal;
                pagamentoMotoristaTMS.ValorINSSPatronal = calculoImpostosRetorno.ValorINSSPatronal;

                pagamentoMotoristaTMS.AliquotaCOFINS = calculoImpostosRetorno.AliquotaCOFINS;
                pagamentoMotoristaTMS.AliquotaPIS = calculoImpostosRetorno.AliquotaPIS;
                pagamentoMotoristaTMS.CodigoIntegracaoTributaria = calculoImpostosRetorno.CodigoIntegracaoTributaria;

                pagamentoMotoristaTMS.QuantidadeDependentes = calculoImpostosRetorno.QuantidadeDependentes;
                pagamentoMotoristaTMS.ValorPorDependente = calculoImpostosRetorno.ValorPorDependente;
                pagamentoMotoristaTMS.ValorTotalDependentes = calculoImpostosRetorno.ValorTotalDependentes;
            }
            else
            {
                pagamentoMotoristaTMS.BaseCalculoINSS = 0;
                pagamentoMotoristaTMS.AliquotaINSS = 0;
                pagamentoMotoristaTMS.ValorINSS = 0;

                pagamentoMotoristaTMS.BaseCalculoSEST = 0;
                pagamentoMotoristaTMS.AliquotaSEST = 0;
                pagamentoMotoristaTMS.ValorSEST = 0;

                pagamentoMotoristaTMS.BaseCalculoSENAT = 0;
                pagamentoMotoristaTMS.AliquotaSENAT = 0;
                pagamentoMotoristaTMS.ValorSENAT = 0;

                pagamentoMotoristaTMS.BaseCalculoIRRF = 0;
                pagamentoMotoristaTMS.AliquotaIRRF = 0;
                pagamentoMotoristaTMS.ValorIRRF = 0;

                pagamentoMotoristaTMS.BaseCalculoIRRFSemDesconto = 0;
                pagamentoMotoristaTMS.BaseCalculoIRRFSemAcumulo = 0;
                pagamentoMotoristaTMS.ValorIRRFSemDesconto = 0;
                pagamentoMotoristaTMS.ValorIRRFPeriodo = 0;

                pagamentoMotoristaTMS.AliquotaINSSPatronal = 0;
                pagamentoMotoristaTMS.ValorINSSPatronal = 0;

                pagamentoMotoristaTMS.AliquotaCOFINS = 0;
                pagamentoMotoristaTMS.AliquotaPIS = 0;
                pagamentoMotoristaTMS.CodigoIntegracaoTributaria = null;

                pagamentoMotoristaTMS.QuantidadeDependentes = 0;
                pagamentoMotoristaTMS.ValorPorDependente = 0;
                pagamentoMotoristaTMS.ValorTotalDependentes = 0;
            }
        }

        public static void AdicionarIntegracaoKMM(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvioKMM = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
            pagamentoMotoristaIntegracaoEnvioKMM.Data = DateTime.Now.Date;
            pagamentoMotoristaIntegracaoEnvioKMM.NumeroTentativas = 0;
            pagamentoMotoristaIntegracaoEnvioKMM.PagamentoMotoristaTMS = pagamentoMotorista;
            pagamentoMotoristaIntegracaoEnvioKMM.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            pagamentoMotoristaIntegracaoEnvioKMM.TipoIntegracaoPagamentoMotorista = TipoIntegracaoPagamentoMotorista.KMM;

            repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvioKMM);
        }

        #endregion
    }
}
