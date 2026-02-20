using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Filiais
{
    public sealed class Filial
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Filial(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private bool VerificarDescontoExcecaoFilial(Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Dominio.Entidades.Empresa transportador, List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaProdutos,  DateTime? dataCarregamento)
        {
            if (filial?.DescontosExcecao == null || filial.DescontosExcecao.Count == 0)
                return false;

            string hora = dataCarregamento?.ToString("HH:mm");
            DateTime? horaCarregamento = Convert.ToDateTime(hora);

            List<Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao> excecoes = (
                from descontoExcecao in filial.DescontosExcecao
                where
                    (descontoExcecao.Transportador == null || descontoExcecao.Transportador.Codigo == transportador?.Codigo) &&
                    (descontoExcecao.Produto == null || listaProdutos.Contains(descontoExcecao.Produto)) &&
                    (descontoExcecao.ModeloVeicularCarga == null || descontoExcecao.ModeloVeicularCarga.Codigo == modeloVeicularCarga?.Codigo) &&
                    ((string.IsNullOrWhiteSpace(descontoExcecao.HoraInicio) && string.IsNullOrWhiteSpace(descontoExcecao.HoraFim)) || (Convert.ToDateTime(descontoExcecao.HoraInicio) <= horaCarregamento && Convert.ToDateTime(descontoExcecao.HoraFim) >= horaCarregamento))
                select descontoExcecao
            ).ToList();

            return excecoes.Count > 0;
        }
        
        #endregion

        #region Métodos Públicos

        public void InformarValorDescontoFilialCarga(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Filiais.Filial filial, List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> listaProdutos, Dominio.Entidades.Empresa transportador, DateTime? dataCarregamento)
        {
            carga.DescontoFilial = 0;

            if ((filial.Descontos == null) || (filial.Descontos.Count == 0))
                return;

            Dominio.Entidades.Embarcador.Filiais.FilialDesconto desconto = (
                from filialDesconto in filial.Descontos
                where filialDesconto.ModeloVeicularCarga?.Codigo == modeloVeicularCarga?.Codigo && filialDesconto.TipoOperacao?.Codigo == carga.TipoOperacao?.Codigo
                select filialDesconto
            ).FirstOrDefault();

            if (desconto == null)
                return;

            if (VerificarDescontoExcecaoFilial(filial, modeloVeicularCarga, transportador, listaProdutos, dataCarregamento))
                return;

            carga.DescontoFilial = desconto.ValorDesconto;
        }

        public byte[] ObterPdfQrCodeEtapa(int codigoFilial, EtapaFluxoGestaoPatio etapa, TipoFluxoGestaoPatio tipo)
        {
            return ReportRequest.WithType(ReportType.EtapaQrCode)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoFilial", codigoFilial.ToString())
                .AddExtraData("Etapa", etapa.ToString())
                .AddExtraData("Tipo", tipo.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion
    }
}
