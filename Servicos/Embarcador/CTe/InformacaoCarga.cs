using Repositorio;
using System;

namespace Servicos.Embarcador.CTe
{
    public class InformacaoCarga : ServicoBase
    {        
        public InformacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga ConverterInfomracaoCTeParaInformacaoCarga(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga informacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
            informacaoCarga.ValorTotalCarga = cte.ValorTotalMercadoria;
            informacaoCarga.ProdutoPredominante = cte.ProdutoPredominante;
            informacaoCarga.ValorCargaAverbacao = 0m;
            informacaoCarga.Container = "";
            informacaoCarga.DataEntregaContainer = DateTime.MinValue;
            informacaoCarga.NumeroLacreContainer = "";
            informacaoCarga.OutrasCaracteristicas = cte.OutrasCaracteristicasDaCarga;
            informacaoCarga.CaracteristicaTransporte = "";
            informacaoCarga.CaracteristicaServico = "";
            return informacaoCarga;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga ConverterInfomracaoCTeParaInformacaoCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga informacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
            informacaoCarga.ValorTotalCarga = cte.ValorTotalMercadoria;
            informacaoCarga.ProdutoPredominante = cte.ProdutoPredominante;
            informacaoCarga.ValorCargaAverbacao = cte.ValorCarbaAverbacao;
            informacaoCarga.Container = cte.Container;
            informacaoCarga.DataEntregaContainer = cte.DataPrevistaContainer.HasValue ? cte.DataPrevistaContainer.Value : DateTime.MinValue;
            informacaoCarga.NumeroLacreContainer = cte.LacreContainer;
            informacaoCarga.OutrasCaracteristicas = cte.OutrasCaracteristicasDaCarga;
            informacaoCarga.CaracteristicaTransporte = cte.CaracteristicaTransporte;
            informacaoCarga.CaracteristicaServico = cte.CaracteristicaServico;
            return informacaoCarga;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga ConverterDynamicParaInformacaoCarga(dynamic dynInformacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            decimal valorTotalCarga, valorCargaAverbacao;
            Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga informacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
            decimal.TryParse(dynInformacaoCarga.ValorTotalCarga.ToString(), out valorTotalCarga);
            decimal.TryParse(dynInformacaoCarga.ValorCargaAverbacao.ToString(), out valorCargaAverbacao);

            informacaoCarga.ValorTotalCarga = valorTotalCarga;
            informacaoCarga.ValorCargaAverbacao = valorCargaAverbacao;

            informacaoCarga.ProdutoPredominante = (string)dynInformacaoCarga.ProdutoPredominante;
            informacaoCarga.Container = (string)dynInformacaoCarga.Container;
            DateTime dataEntregaContainer = new DateTime();
            DateTime.TryParseExact((string)dynInformacaoCarga.DataEntregaContainer, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntregaContainer);
            informacaoCarga.DataEntregaContainer = dataEntregaContainer;
            informacaoCarga.NumeroLacreContainer = (string)dynInformacaoCarga.NumeroLacreContainer;
            informacaoCarga.OutrasCaracteristicas = (string)dynInformacaoCarga.OutrasCaracteristicas;
            informacaoCarga.CaracteristicaTransporte = (string)dynInformacaoCarga.CaracteristicaAdicionalTransporte;
            informacaoCarga.CaracteristicaServico = (string)dynInformacaoCarga.CaracteristicaAdicionalTransporte;

            return informacaoCarga;
        }

        public void SalvarInformacaoCargaCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga informacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            cte.ValorTotalMercadoria = informacaoCarga.ValorTotalCarga;
            cte.ProdutoPredominante = informacaoCarga.ProdutoPredominante;
            cte.ValorCarbaAverbacao = informacaoCarga.ValorCargaAverbacao;
            cte.CaracteristicaServico = informacaoCarga.CaracteristicaServico;
            cte.CaracteristicaTransporte = informacaoCarga.CaracteristicaTransporte;
            cte.Container = informacaoCarga.Container;

            if (informacaoCarga.DataEntregaContainer != DateTime.MinValue)
                cte.DataPrevistaContainer = informacaoCarga.DataEntregaContainer;

            cte.LacreContainer = informacaoCarga.NumeroLacreContainer;
            cte.OutrasCaracteristicasDaCarga = informacaoCarga.OutrasCaracteristicas;
            cte.ValorTotalMercadoria = informacaoCarga.ValorTotalCarga;
        }

        public void SalvarInformacaoCargaPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga informacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            preCTe.ValorTotalMercadoria = informacaoCarga.ValorTotalCarga;
            preCTe.ProdutoPredominante = informacaoCarga.ProdutoPredominante.Left(60);
            preCTe.CaracteristicaServico = informacaoCarga.CaracteristicaServico;
            preCTe.CaracteristicaTransporte = informacaoCarga.CaracteristicaTransporte;
            preCTe.Container = informacaoCarga.Container;
            if (informacaoCarga.DataEntregaContainer != DateTime.MinValue)
                preCTe.DataPrevistaContainer = informacaoCarga.DataEntregaContainer;

            preCTe.LacreContainer = informacaoCarga.NumeroLacreContainer;
            preCTe.OutrasCaracteristicasDaCarga = informacaoCarga.OutrasCaracteristicas;
            preCTe.ValorTotalMercadoria = informacaoCarga.ValorTotalCarga;
        }
    }
}
