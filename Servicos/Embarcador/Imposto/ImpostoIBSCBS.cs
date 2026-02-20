using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Imposto
{
    public class ImpostoIBSCBS
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly ImpostoPisCofins _servicoPisCofins = new ImpostoPisCofins();

        #endregion

        #region Construtores

        public ImpostoIBSCBS(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void TesteCalculo()
        {
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS teste =
                CalcularImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS()
                {
                    BaseCalculo = 1000,
                    SiglaUF = "SC",
                    CodigoLocalidade = 76066,
                    CodigoTipoOperacao = 0
                });
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterImpostoIBSCBS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo)
        {
            if (parametroCalculo.Empresa?.OptanteSimplesNacional ?? false)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            CalcularDescontoBaseCalculo(parametroCalculo);

            return CalcularImpostoIBSCBS(parametroCalculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterImpostoIBSCBSComValoresArredondados(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo)
        {
            if (parametroCalculo.Empresa?.OptanteSimplesNacional ?? false)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            CalcularDescontoBaseCalculo(parametroCalculo);

            return ArredondarValoresImpostoIBSCBS(CalcularImpostoIBSCBS(parametroCalculo));
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterImpostoIBSCBSComTributacaoDefinidaFilialEmissora(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorParaBaseDeCalculo)
        {
            if (cargaPedido?.CargaOrigem?.EmpresaFilialEmissora?.OptanteSimplesNacional ?? false)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            valorParaBaseDeCalculo -= Math.Round(_servicoPisCofins.ObterValoresPisCofins(cargaPedido.CargaOrigem.EmpresaFilialEmissora, valorParaBaseDeCalculo), 2, MidpointRounding.AwayFromZero);

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota outrasAliquotasDefinida = ObterOutrasAliquotasIBSCBS(cargaPedido.OutrasAliquotas?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida parametroCalculo = new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida()
            {
                CST = cargaPedido.CSTIBSCBSFilialEmissora,
                ClassificacaoTributaria = cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora,
                BaseCalculo = valorParaBaseDeCalculo,
                AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadualFilialEmissora,
                PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadualFilialEmissora,
                AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipalFilialEmissora,
                PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora,
                AliquotaCBS = cargaPedido.AliquotaCBSFilialEmissora,
                PercentualReducaoCBS = cargaPedido.PercentualReducaoCBSFilialEmissora,
                ZerarBaseCalculo = ((cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla == "EX") && (outrasAliquotasDefinida?.Exportacao ?? false)) || (outrasAliquotasDefinida?.ZerarBase ?? false)
            };

            return CalcularValorIBSCBSComTributacaoDefinida(parametroCalculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterImpostoIBSCBSComTributacaoDefinida(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal valorParaBaseDeCalculo)
        {
            if (cargaPedido?.Carga?.Empresa?.OptanteSimplesNacional ?? false)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            valorParaBaseDeCalculo -= _servicoPisCofins.ObterValoresPisCofins(cargaPedido.Carga.Empresa, valorParaBaseDeCalculo);

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota outrasAliquotasDefinida = ObterOutrasAliquotasIBSCBS(cargaPedido.OutrasAliquotas?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida parametroCalculo = new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida()
            {
                CST = cargaPedido.CSTIBSCBS,
                CodigoIndicadorOperacao = cargaPedido.CodigoIndicadorOperacao,
                NBS = cargaPedido.NBS,
                ClassificacaoTributaria = cargaPedido.ClassificacaoTributariaIBSCBS,
                BaseCalculo = valorParaBaseDeCalculo,
                AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual,
                AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal,
                AliquotaCBS = cargaPedido.AliquotaCBS,
                PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS,
                ZerarBaseCalculo = ((cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla == "EX") && (outrasAliquotasDefinida?.Exportacao ?? false)) || (outrasAliquotasDefinida?.ZerarBase ?? false)
            };

            return CalcularValorIBSCBSComTributacaoDefinida(parametroCalculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterImpostoIBSCBSComTributacaoDefinida(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, decimal valorParaBaseDeCalculo)
        {
            Dominio.Entidades.Empresa empresa = pedidoCTeParaSubContratacao.CteSubContratacaoFilialEmissora ? pedidoCTeParaSubContratacao.CargaPedido.Carga.EmpresaFilialEmissora : pedidoCTeParaSubContratacao.CargaPedido.Carga.Empresa;

            if (empresa?.OptanteSimplesNacional ?? false)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            valorParaBaseDeCalculo -= _servicoPisCofins.ObterValoresPisCofins(empresa, valorParaBaseDeCalculo);

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota outrasAliquotasDefinida = ObterOutrasAliquotasIBSCBS(pedidoCTeParaSubContratacao.OutrasAliquotas?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida parametroCalculo = new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida()
            {
                CST = pedidoCTeParaSubContratacao.CSTIBSCBS,
                ClassificacaoTributaria = pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS,
                BaseCalculo = valorParaBaseDeCalculo,
                CodigoIndicadorOperacao = pedidoCTeParaSubContratacao.CodigoIndicadorOperacao,
                AliquotaIBSEstadual = pedidoCTeParaSubContratacao.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual,
                AliquotaIBSMunicipal = pedidoCTeParaSubContratacao.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal,
                AliquotaCBS = pedidoCTeParaSubContratacao.AliquotaCBS,
                PercentualReducaoCBS = pedidoCTeParaSubContratacao.PercentualReducaoCBS,
                ZerarBaseCalculo = ((pedidoCTeParaSubContratacao.CargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla == "EX") && (outrasAliquotasDefinida?.Exportacao ?? false)) || (outrasAliquotasDefinida?.ZerarBase ?? false)
            };

            return CalcularValorIBSCBSComTributacaoDefinida(parametroCalculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterImpostoIBSCBSComTributacaoDefinida(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, decimal valorParaBaseDeCalculo, int? codigoOutrasAliquotas = 0)
        {
            if (cte?.Empresa?.OptanteSimplesNacional ?? false)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            valorParaBaseDeCalculo -= _servicoPisCofins.ObterValoresPisCofins(cte.Empresa, valorParaBaseDeCalculo);

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota outrasAliquotasDefinida = ObterOutrasAliquotasIBSCBS(cte.OutrasAliquotas?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida parametroCalculo = new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida()
            {
                CST = cte.CSTIBSCBS,
                ClassificacaoTributaria = cte.ClassificacaoTributariaIBSCBS,
                BaseCalculo = valorParaBaseDeCalculo,
                CodigoIndicadorOperacao = cte.CodigoIndicadorOperacao,
                AliquotaIBSEstadual = cte.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = cte.PercentualReducaoIBSEstadual,
                AliquotaIBSMunicipal = cte.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = cte.PercentualReducaoIBSMunicipal,
                AliquotaCBS = cte.AliquotaCBS,
                PercentualReducaoCBS = cte.PercentualReducaoCBS,
                ZerarBaseCalculo = (cte.Destinatario.Exterior && (outrasAliquotasDefinida?.Exportacao ?? false)) || (outrasAliquotasDefinida?.ZerarBase ?? false)
            };

            return CalcularValorIBSCBSComTributacaoDefinida(parametroCalculo, codigoOutrasAliquotas: codigoOutrasAliquotas ?? 0);
        }

        public Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS ObterImpostoIBSCBSDoCTe(Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS retornoImpostoIBSCBS)
        {
            if (string.IsNullOrWhiteSpace(retornoImpostoIBSCBS.CST) || string.IsNullOrWhiteSpace(retornoImpostoIBSCBS.ClassificacaoTributaria))
                return null;

            return new Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS()
            {
                CST = retornoImpostoIBSCBS.CST,
                CodigoIndicadorOperacao = retornoImpostoIBSCBS.CodigoIndicadorOperacao,
                NBS = retornoImpostoIBSCBS.NBS,
                ClassificacaoTributaria = retornoImpostoIBSCBS.ClassificacaoTributaria,
                BaseCalculo = retornoImpostoIBSCBS.BaseCalculo,

                AliquotaIBSEstadual = retornoImpostoIBSCBS.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = retornoImpostoIBSCBS.PercentualReducaoIBSEstadual,
                AliquotaIBSEstadualEfetiva = CalcularAliquotaEfetiva(retornoImpostoIBSCBS.AliquotaIBSEstadual, retornoImpostoIBSCBS.PercentualReducaoIBSEstadual),
                ValorIBSEstadual = retornoImpostoIBSCBS.ValorIBSEstadual,

                AliquotaIBSMunicipal = retornoImpostoIBSCBS.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = retornoImpostoIBSCBS.PercentualReducaoIBSMunicipal,
                AliquotaIBSMunicipalEfetiva = CalcularAliquotaEfetiva(retornoImpostoIBSCBS.AliquotaIBSMunicipal, retornoImpostoIBSCBS.PercentualReducaoIBSMunicipal),
                ValorIBSMunicipal = retornoImpostoIBSCBS.ValorIBSMunicipal,

                AliquotaCBS = retornoImpostoIBSCBS.AliquotaCBS,
                PercentualReducaoCBS = retornoImpostoIBSCBS.PercentualReducaoCBS,
                AliquotaCBSEfetiva = CalcularAliquotaEfetiva(retornoImpostoIBSCBS.AliquotaCBS, retornoImpostoIBSCBS.PercentualReducaoCBS),
                ValorCBS = retornoImpostoIBSCBS.ValorCBS
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota ObterOutrasAliquotasIBSCBS(int codigoOutrasAliquotas)
        {
            return ObterGrupoInformacaoTributacao(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS() { CodigoOutrasAliquotas = codigoOutrasAliquotas });
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota ObterOutrasAliquotasIBSCBS(string outrasAliquotasCST)
        {
            return ObterGrupoInformacaoTributacao(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS() { OutrasAliquotasCST = outrasAliquotasCST });
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ArredondarValoresImpostoIBSCBS(Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS retornoImpostoIBSCBS)
        {
            retornoImpostoIBSCBS.ValorCBS = Math.Round(retornoImpostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
            retornoImpostoIBSCBS.ValorIBSEstadual = Math.Round(retornoImpostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);
            retornoImpostoIBSCBS.ValorIBSMunicipal = Math.Round(retornoImpostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);

            return retornoImpostoIBSCBS;
        }

        private Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS CalcularImpostoIBSCBS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo)
        {
            if (parametroCalculo.Empresa?.OptanteSimplesNacional ?? false)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            if (parametroCalculo.BaseCalculo <= 0 || parametroCalculo.CodigoLocalidade == 0 || string.IsNullOrWhiteSpace(parametroCalculo.SiglaUF))
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota outraAliquota = ObterGrupoInformacaoTributacao(parametroCalculo);

            if (outraAliquota == null)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            if (outraAliquota.ZerarBase || outraAliquota.Exportacao)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS
                {
                    CodigoOutraAliquota = outraAliquota.Codigo,
                    ClassificacaoTributaria = outraAliquota.ClassificacaoTributaria,
                    CST = outraAliquota.CST,
                    CodigoIndicadorOperacao = outraAliquota.CodigoIndicadorOperacao
                };

            if (outraAliquota.Impostos == null || outraAliquota.Impostos.Count == 0)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto impostoCBS = ObterDadosImpostoCBS(parametroCalculo, outraAliquota.Impostos);

            if (impostoCBS == null)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto impostoIBS = ObterDadosImpostoIBSMunicipal(parametroCalculo, outraAliquota.Impostos);

            if (impostoIBS == null)
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS();

            decimal baseCalculo = parametroCalculo.BaseCalculo;

            return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
            {
                CodigoOutraAliquota = outraAliquota.Codigo,
                CodigoIndicadorOperacao = outraAliquota.CodigoIndicadorOperacao,
                CST = outraAliquota.CST,
                ClassificacaoTributaria = outraAliquota.ClassificacaoTributaria,
                BaseCalculo = baseCalculo,

                AliquotaCBS = impostoCBS.Aliquota,
                PercentualReducaoCBS = impostoCBS.PercentualReducao,
                ValorCBS = CalcularValorImposto(baseCalculo, impostoCBS.Aliquota, impostoCBS.PercentualReducao),

                AliquotaIBSEstadual = impostoIBS.AliquotaUF,
                PercentualReducaoIBSEstadual = impostoIBS.PercentualReducaoUF,
                ValorIBSEstadual = CalcularValorImposto(baseCalculo, impostoIBS.AliquotaUF, impostoIBS.PercentualReducaoUF),

                AliquotaIBSMunicipal = impostoIBS.AliquotaMunicipio,
                PercentualReducaoIBSMunicipal = impostoIBS.PercentualReducaoMunicipio,
                ValorIBSMunicipal = CalcularValorImposto(baseCalculo, impostoIBS.AliquotaMunicipio, impostoIBS.PercentualReducaoMunicipio)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS CalcularValorIBSCBSComTributacaoDefinida(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBSComTributacaoDefinida parametroCalculo, int codigoOutrasAliquotas = 0)
        {
            decimal baseCalculo = parametroCalculo.ZerarBaseCalculo ? 0 : parametroCalculo.BaseCalculo;

            return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
            {
                CodigoOutraAliquota = codigoOutrasAliquotas,
                CST = parametroCalculo.CST,
                ClassificacaoTributaria = parametroCalculo.ClassificacaoTributaria,
                BaseCalculo = baseCalculo,

                CodigoIndicadorOperacao = parametroCalculo.CodigoIndicadorOperacao,
                NBS = parametroCalculo.NBS,

                AliquotaCBS = parametroCalculo.AliquotaCBS,
                PercentualReducaoCBS = parametroCalculo.PercentualReducaoCBS,
                ValorCBS = CalcularValorImposto(baseCalculo, parametroCalculo.AliquotaCBS, parametroCalculo.PercentualReducaoCBS),

                AliquotaIBSEstadual = parametroCalculo.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = parametroCalculo.PercentualReducaoIBSEstadual,
                ValorIBSEstadual = CalcularValorImposto(baseCalculo, parametroCalculo.AliquotaIBSEstadual, parametroCalculo.PercentualReducaoIBSEstadual),

                AliquotaIBSMunicipal = parametroCalculo.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = parametroCalculo.PercentualReducaoIBSMunicipal,
                ValorIBSMunicipal = CalcularValorImposto(baseCalculo, parametroCalculo.AliquotaIBSMunicipal, parametroCalculo.PercentualReducaoIBSMunicipal)
            };
        }

        private decimal CalcularValorImposto(decimal baseCalculo, decimal aliquota, decimal percentualReducao)
        {
            if (percentualReducao > 0)
                aliquota -= aliquota * (percentualReducao / 100);

            return baseCalculo * (aliquota / 100);
        }

        private decimal CalcularAliquotaEfetiva(decimal aliquota, decimal percentualReducao)
        {
            if (percentualReducao > 0)
                aliquota -= aliquota * (percentualReducao / 100);

            return aliquota;
        }

        private void CalcularDescontoBaseCalculo(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo)
        {
            if (!parametroCalculo.NaoReduzirPisCofins)
                parametroCalculo.BaseCalculo -= Math.Round(_servicoPisCofins.ObterValoresPisCofins(parametroCalculo.Empresa, parametroCalculo.BaseCalculo), 2, MidpointRounding.AwayFromZero);

            parametroCalculo.BaseCalculo -= parametroCalculo.ValoAbaterBaseCalculo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota ObterGrupoInformacaoTributacao(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo)
        {
            Servicos.Embarcador.Imposto.OutraAliquotaInstance servicoOutraAliquotaInstance = Servicos.Embarcador.Imposto.OutraAliquotaInstance.GetInstance(_unitOfWork);

            if ((servicoOutraAliquotaInstance.OutrasAliquotas?.Count ?? 0) == 0)
                return null;

            if (parametroCalculo.CodigoOutrasAliquotas > 0)
                return servicoOutraAliquotaInstance.OutrasAliquotas.Find(o => o.Codigo == parametroCalculo.CodigoOutrasAliquotas);

            if (!string.IsNullOrWhiteSpace(parametroCalculo.OutrasAliquotasCST))
                return servicoOutraAliquotaInstance.OutrasAliquotas.Find(o => o.CST == parametroCalculo.OutrasAliquotasCST);

            if (parametroCalculo.SiglaUF == "EX")
            {
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota outraAliquotaExportacao = servicoOutraAliquotaInstance.OutrasAliquotas.Find(o => o.Exportacao);
                if (outraAliquotaExportacao != null)
                    return outraAliquotaExportacao;
            }

            if (parametroCalculo.CodigoTipoOperacao == 0)
                return servicoOutraAliquotaInstance.OutrasAliquotas.Find(o => o.CodigoTipoOperacao == 0 && !o.Exportacao);

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota outraAliquota = servicoOutraAliquotaInstance.OutrasAliquotas.Find(o => o.CodigoTipoOperacao == parametroCalculo.CodigoTipoOperacao && !o.Exportacao);
            if (outraAliquota != null)
                return outraAliquota;

            return servicoOutraAliquotaInstance.OutrasAliquotas.Find(o => o.CodigoTipoOperacao == 0 && !o.Exportacao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto ObterDadosImpostoCBS(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo, List<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto> impostos)
        {
            return impostos.Find(o => o.TipoImposto == TipoImposto.CBS && o.DataVigenciaInicial <= DateTime.Now.Date && o.DataVigenciaFinal >= DateTime.Now.Date);
        }

        private Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto ObterDadosImpostoIBSEstadual(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo, List<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto> impostos)
        {
            return impostos.Find(o => o.TipoImposto == TipoImposto.IBS && o.SiglaUF == parametroCalculo.SiglaUF && o.CodigoLocalidade == 0 && o.DataVigenciaInicial <= DateTime.Now.Date && o.DataVigenciaFinal >= DateTime.Now.Date);
        }

        private Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto ObterDadosImpostoIBSMunicipal(Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS parametroCalculo, List<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto> impostos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto> impostosIBS = impostos.FindAll(o => o.TipoImposto == TipoImposto.IBS && o.DataVigenciaInicial <= DateTime.Now.Date && o.DataVigenciaFinal >= DateTime.Now.Date);

            Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto outraAliquotaImposto = impostosIBS.Find(o => o.CodigoLocalidade == parametroCalculo.CodigoLocalidade);
            if (outraAliquotaImposto != null)
                return outraAliquotaImposto;

            return ObterDadosImpostoIBSEstadual(parametroCalculo, impostosIBS);
        }

        #endregion
    }
}
