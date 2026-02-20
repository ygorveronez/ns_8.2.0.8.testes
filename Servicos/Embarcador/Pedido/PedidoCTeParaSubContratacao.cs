using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public class PedidoCTeParaSubContratacao
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public PedidoCTeParaSubContratacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterRetornoImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
            {
                CodigoOutraAliquota = pedidoCTeParaSubContratacao.OutrasAliquotas?.Codigo ?? 0,
                CST = pedidoCTeParaSubContratacao.CSTIBSCBS,
                ClassificacaoTributaria = pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS,
                BaseCalculo = pedidoCTeParaSubContratacao.BaseCalculoIBSCBS,

                CodigoIndicadorOperacao = pedidoCTeParaSubContratacao.CodigoIndicadorOperacao,
                NBS = pedidoCTeParaSubContratacao.NBS,

                AliquotaIBSEstadual = pedidoCTeParaSubContratacao.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual,
                ValorIBSEstadual = pedidoCTeParaSubContratacao.ValorIBSEstadual,

                AliquotaIBSMunicipal = pedidoCTeParaSubContratacao.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal,
                ValorIBSMunicipal = pedidoCTeParaSubContratacao.ValorIBSMunicipal,

                AliquotaCBS = pedidoCTeParaSubContratacao.AliquotaCBS,
                PercentualReducaoCBS = pedidoCTeParaSubContratacao.PercentualReducaoCBS,
                ValorCBS = pedidoCTeParaSubContratacao.ValorCBS
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterRetornoImpostoIBSCBS(List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = pedidosCTeParaSubContratacao.FirstOrDefault();

            return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
            {
                CodigoOutraAliquota = pedidoCTeParaSubContratacao.OutrasAliquotas?.Codigo ?? 0,
                CST = pedidoCTeParaSubContratacao.CSTIBSCBS,
                ClassificacaoTributaria = pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS,
                BaseCalculo = pedidosCTeParaSubContratacao.Sum(obj => obj.BaseCalculoIBSCBS),

                CodigoIndicadorOperacao = pedidoCTeParaSubContratacao.CodigoIndicadorOperacao,
                NBS = pedidoCTeParaSubContratacao.NBS,

                AliquotaIBSEstadual = pedidoCTeParaSubContratacao.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual,
                ValorIBSEstadual = pedidosCTeParaSubContratacao.Sum(obj => obj.ValorIBSEstadual),

                AliquotaIBSMunicipal = pedidoCTeParaSubContratacao.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal,
                ValorIBSMunicipal = pedidosCTeParaSubContratacao.Sum(obj => obj.ValorIBSMunicipal),

                AliquotaCBS = pedidoCTeParaSubContratacao.AliquotaCBS,
                PercentualReducaoCBS = pedidoCTeParaSubContratacao.PercentualReducaoCBS,
                ValorCBS = pedidosCTeParaSubContratacao.Sum(obj => obj.ValorCBS)
            };
        }

        public void PreencherCamposImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS)
        {
            pedidoCTeParaSubContratacao.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutraAliquota);
            pedidoCTeParaSubContratacao.CSTIBSCBS = impostoIBSCBS.CST;
            pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
            pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
            pedidoCTeParaSubContratacao.NBS = impostoIBSCBS.NBS;
            pedidoCTeParaSubContratacao.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;

            pedidoCTeParaSubContratacao.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
            pedidoCTeParaSubContratacao.ValorIBSEstadual = Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);

            pedidoCTeParaSubContratacao.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
            pedidoCTeParaSubContratacao.ValorIBSMunicipal = Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);

            pedidoCTeParaSubContratacao.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
            pedidoCTeParaSubContratacao.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
            pedidoCTeParaSubContratacao.ValorCBS = Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinidaEValores(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            pedidoCTeParaSubContratacao.CSTIBSCBS = cargaPedido.CSTIBSCBS;
            pedidoCTeParaSubContratacao.CodigoIndicadorOperacao = cargaPedido.CodigoIndicadorOperacao;
            pedidoCTeParaSubContratacao.NBS = cargaPedido.NBS;
            pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS = cargaPedido.ClassificacaoTributariaIBSCBS;
            pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = cargaPedido.BaseCalculoIBSCBS;

            pedidoCTeParaSubContratacao.AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual;
            pedidoCTeParaSubContratacao.ValorIBSEstadual = cargaPedido.ValorIBSEstadual;

            pedidoCTeParaSubContratacao.AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal;
            pedidoCTeParaSubContratacao.ValorIBSMunicipal = cargaPedido.ValorIBSMunicipal;

            pedidoCTeParaSubContratacao.AliquotaCBS = cargaPedido.AliquotaCBS;
            pedidoCTeParaSubContratacao.PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS;
            pedidoCTeParaSubContratacao.ValorCBS = cargaPedido.ValorCBS;

            pedidoCTeParaSubContratacao.NBS = cargaPedido.NBS;
            pedidoCTeParaSubContratacao.CodigoIndicadorOperacao = cargaPedido.CodigoIndicadorOperacao;
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinidaEValores(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidosXMLNotaFiscal.FirstOrDefault();

            pedidoCTeParaSubContratacao.CSTIBSCBS = pedidoXMLNotaFiscal.CSTIBSCBS;
            pedidoCTeParaSubContratacao.NBS = pedidoXMLNotaFiscal.NBS;
            pedidoCTeParaSubContratacao.CodigoIndicadorOperacao = pedidoXMLNotaFiscal.CodigoIndicadorOperacao;
            pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS = pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS;
            pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = pedidosXMLNotaFiscal.Sum(obj => obj.BaseCalculoIBSCBS);

            pedidoCTeParaSubContratacao.AliquotaIBSEstadual = pedidoXMLNotaFiscal.AliquotaIBSEstadual;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual = pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual;
            pedidoCTeParaSubContratacao.ValorIBSEstadual = pedidosXMLNotaFiscal.Sum(obj => obj.ValorIBSEstadual);

            pedidoCTeParaSubContratacao.AliquotaIBSMunicipal = pedidoXMLNotaFiscal.AliquotaIBSMunicipal;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal = pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal;
            pedidoCTeParaSubContratacao.ValorIBSMunicipal = pedidosXMLNotaFiscal.Sum(obj => obj.ValorIBSMunicipal);

            pedidoCTeParaSubContratacao.AliquotaCBS = pedidoXMLNotaFiscal.AliquotaCBS;
            pedidoCTeParaSubContratacao.PercentualReducaoCBS = pedidoXMLNotaFiscal.PercentualReducaoCBS;
            pedidoCTeParaSubContratacao.ValorCBS = pedidosXMLNotaFiscal.Sum(obj => obj.ValorCBS);
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinidaFilialEmissora(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            pedidoCTeParaSubContratacao.CSTIBSCBS = cargaPedido.CSTIBSCBSFilialEmissora;
            pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS = cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora;

            pedidoCTeParaSubContratacao.AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadualFilialEmissora;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadualFilialEmissora;

            pedidoCTeParaSubContratacao.AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipalFilialEmissora;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora;

            pedidoCTeParaSubContratacao.AliquotaCBS = cargaPedido.AliquotaCBSFilialEmissora;
            pedidoCTeParaSubContratacao.PercentualReducaoCBS = cargaPedido.PercentualReducaoCBSFilialEmissora;
        }

        public void PreencherValoresImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS, bool arredondar = false)
        {
            pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = arredondar ? Math.Round(baseCalculo, 3, MidpointRounding.AwayFromZero) : baseCalculo;
            pedidoCTeParaSubContratacao.ValorIBSEstadual = arredondar ? Math.Round(valorIBSEstadual, 3, MidpointRounding.AwayFromZero) : valorIBSEstadual;
            pedidoCTeParaSubContratacao.ValorIBSMunicipal = arredondar ? Math.Round(valorIBSMunicipal, 3, MidpointRounding.AwayFromZero) : valorIBSMunicipal;
            pedidoCTeParaSubContratacao.ValorCBS = arredondar ? Math.Round(valorCBS, 3, MidpointRounding.AwayFromZero) : valorCBS;
        }

        public void ZerarCamposImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, bool apenasValores = false)
        {
            if (apenasValores)
            {
                pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = 0m;
                pedidoCTeParaSubContratacao.ValorIBSEstadual = 0m;
                pedidoCTeParaSubContratacao.ValorIBSMunicipal = 0m;
                pedidoCTeParaSubContratacao.ValorCBS = 0m;

                return;
            }

            pedidoCTeParaSubContratacao.SetarRegraOutraAliquota(0);
            pedidoCTeParaSubContratacao.CST = string.Empty;
            pedidoCTeParaSubContratacao.CodigoIndicadorOperacao = string.Empty;
            pedidoCTeParaSubContratacao.NBS = string.Empty;
            pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS = string.Empty;
            pedidoCTeParaSubContratacao.BaseCalculoIBSCBS = 0m;

            pedidoCTeParaSubContratacao.AliquotaIBSEstadual = 0m;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual = 0m;
            pedidoCTeParaSubContratacao.ValorIBSEstadual = 0m;

            pedidoCTeParaSubContratacao.AliquotaIBSMunicipal = 0m;
            pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal = 0m;
            pedidoCTeParaSubContratacao.ValorIBSMunicipal = 0m;

            pedidoCTeParaSubContratacao.AliquotaCBS = 0m;
            pedidoCTeParaSubContratacao.PercentualReducaoCBS = 0m;
            pedidoCTeParaSubContratacao.ValorCBS = 0m;
        }

        #endregion
    }
}
