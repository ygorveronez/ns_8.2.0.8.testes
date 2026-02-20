using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class OrdemColeta
    {
        public OrdemColeta() { }

        public OrdemColeta(Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer)
        {
            _janelaCarregamento = janelaCarregamento;
            _coletaContainer = coletaContainer;

            PreencherInformacoes();
        }

        public OrdemColeta(Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            _janelaCarregamento = janelaCarregamento;
            _cargaPedido = cargaPedido;

            PreencherInformacoes();
        }

        public OrdemColeta(Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres)
        {
            _janelaCarregamento = janelaCarregamento;
            _cargaPedido = cargaPedido;
            _cargaLacres = cargaLacres;

            PreencherInformacoes();
        }

        public OrdemColeta(Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer, List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres)
        {
            _janelaCarregamento = janelaCarregamento;
            _coletaContainer = coletaContainer;
            _cargaLacres = cargaLacres;

            PreencherInformacoes();
        }

        #region Propriedades Privadas

        private readonly Entidades.Embarcador.Cargas.CargaJanelaCarregamento _janelaCarregamento;
        private readonly Entidades.Embarcador.Cargas.CargaPedido _cargaPedido;
        private readonly Dominio.Entidades.Embarcador.Pedidos.ColetaContainer _coletaContainer;
        private readonly List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> _cargaLacres;

        #endregion

        #region Ordem Coleta

        public string Carga { get; set; }
        public string TipoCarga { get; set; }
        public string ModeloVeicular { get; set; }
        public string NumeroEntregas { get; set; }
        public string Peso { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Destinatario { get; set; }
        public string DataCarregamento { get; set; }
        public string PrevisaoEntrega { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoCarregamento { get; set; }

        public string ValorICMS { get; set; }
        public string ValorFrete { get; set; }
        public string ValorTotal { get; set; }

        public string Motorista { get; set; }
        public string Veiculo { get; set; }

        public string CodigosCargasEmbarcador { get; set; }

		public string TelefoneMotorista { get; set; }

		#endregion

		#region Ordem Coleta Container

		public string Remetente { get; set; }
        public string RemetenteCNPJ { get; set; }
        public string RemetenteIE { get; set; }
        public string Descarga { get; set; }
        public string DescargaEndereco { get; set; }
        public string DescargaCidade { get; set; }
        public string DescargaCNPJ { get; set; }
        public string DescargaIE { get; set; }
        public string QtdeContainer { get; set; }
        public string Booking { get; set; }
        public string Tara { get; set; }
        public string LacreSif { get; set; }
        public string Lacre { get; set; }
        public string AgenciaMaritima { get; set; }
        public string Mercadoria { get; set; }
        public string Temperatura { get; set; }
        public string Navio { get; set; }
        public string Porto { get; set; }
        public string LocalRetirada { get; set; }
        public string LocalDesova { get; set; }
        public string LocalEntrega { get; set; }
        public string MotoristaRG { get; set; }
        public string MotoristaCPF { get; set; }
        public string MotoristaCNH { get; set; }
        public string Contrato { get; set; }
        public string LocalEData { get; set; }
        public string DataColetaContainer { get; set; }

        #endregion

        #region Ordem de Carregamento

        public int CodigoPedido { get; set; }
        public string CentroCarregamento { get; set; }
        public string DataHoraEmissao { get; set; }
        public string DataHoraEntrega { get; set; }
        public string Transportadora { get; set; }
        public string CodigoDeBarras { get; set; }
        public string Endereco { get; set; }
        public string TotalPesoBruto { get; set; }
        public string TotalPesoLiquido { get; set; }

        public string NumeroPedido { get; set; }
        public string NumeroOrdemVenda { get; set; }

        #endregion

        #region Métodos Privados

        private void PreencherInformacoes()
        {
            Entidades.Usuario motorista = _janelaCarregamento.Carga.Motoristas?.FirstOrDefault();

            Carga = _janelaCarregamento.Carga.CodigoCargaEmbarcador;
            DataCarregamento = _janelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm");
            Veiculo = _janelaCarregamento.Carga?.PlacasVeiculos ?? "";
            Motorista = motorista?.Nome ?? "";
            MotoristaCPF = motorista?.CPF_CNPJ_Formatado ?? "";
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutImpressaoOrdemColeta layoutImpressaoOrdemColeta = _janelaCarregamento.Carga.TipoOperacao?.ConfiguracaoJanelaCarregamento?.LayoutImpressaoOrdemColeta ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutImpressaoOrdemColeta.LayoutPadrao;

            if (layoutImpressaoOrdemColeta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutImpressaoOrdemColeta.LayoutColetaContainer)
            {
                PreencherInformacoesColetaContainer();
            }
            else if (layoutImpressaoOrdemColeta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemCarregamento)
            {
                PreencherInformacoesOrdemCarregamento();
            } 
            else if (layoutImpressaoOrdemColeta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutImpressaoOrdemColeta.LayoutOrdemColetaAuxiliar)
			{
                PreencherInformacoesDocumentoAuxiliarOrdemColeta(motorista);
			}
            else
            {
                PreencherInformacoesPadroes();
            }
        }

        private void PreencherInformacoesColetaContainer()
        {
            ICollection<Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = _janelaCarregamento.Carga.Pedidos;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = _janelaCarregamento.Carga;
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres = _cargaLacres;

            Entidades.Usuario motorista = _janelaCarregamento.Carga.Motoristas?.FirstOrDefault();

            Entidades.Embarcador.Pedidos.Pedido pedido = (from o in cargaPedidos select o.Pedido).FirstOrDefault();
            ICollection<Entidades.Veiculo> veiculos = _janelaCarregamento.Carga.VeiculosVinculados;
            DateTime now = DateTime.Now;

            Remetente = carga.Filial?.Descricao ?? "";
            RemetenteCNPJ = carga.Filial?.CNPJ_Formatado ?? "";
            RemetenteIE = carga.Filial?.EmpresaEmissora?.InscricaoEstadual ?? "";

            Descarga = carga.Filial?.Descricao ?? "";
            DescargaEndereco = carga.Filial?.Localidade?.DescricaoCidadeEstadoPais ?? "";
            DescargaCidade = carga.Filial?.Localidade?.Descricao ?? "";
            DescargaCNPJ = carga.Filial?.CNPJ_Formatado ?? "";
            DescargaIE = carga.Filial?.EmpresaEmissora?.InscricaoEstadual ?? "";

            QtdeContainer = string.IsNullOrWhiteSpace(pedido.ModeloVeicularCarga?.Descricao) ? "" : $"1 x {pedido.ModeloVeicularCarga?.Descricao ?? ""} ";

            if (!string.IsNullOrWhiteSpace(_coletaContainer?.Container?.Numero))
                QtdeContainer += $"({_coletaContainer.Container.Numero})";

            DataColetaContainer = _coletaContainer?.DataColeta?.ToString("dd/MM/yyyy") ?? "";

            Booking = pedido.NumeroBooking ?? "";
            Tara = (from o in veiculos select $"{o.Tara} / {o.ModeloVeicularCarga.CapacidadePesoTransporte}").FirstOrDefault();
            LacreSif = "";

            if (cargaLacres != null && cargaLacres.Count > 0)
                Lacre = cargaLacres.FirstOrDefault().Numero ?? "";
            else
                Lacre = pedido.LacreContainerUm ?? "";

            AgenciaMaritima = pedido.ClienteDonoContainer?.Nome ?? "";
            Mercadoria = "";
            Temperatura = pedido.TipoOperacao?.TipoDeCargaPadraoOperacao?.FaixaDeTemperatura?.DescricaoVariancia ?? "";

            Navio = pedido.NavioViagem ?? "";
            Porto = pedido.PortoViagemDestino ?? "";

            LocalRetirada = pedido.PortoViagemOrigem ?? "";
            LocalDesova = _janelaCarregamento.Carga.Filial?.Descricao ?? "";
            LocalEntrega = carga.PortoDestino?.Descricao ?? "";
            MotoristaRG = motorista?.RG ?? "";
            MotoristaCPF = motorista?.CPF ?? "";
            MotoristaCNH = "";
            LocalEData = $"{pedido.Remetente?.Localidade?.DescricaoCidadeEstado ?? ""}, {now.Day} de {now.ToString("MMMM", new CultureInfo("pt-BR"))} de {now.Year}.";
            Contrato = pedido.NumeroEXP;
        }

        private void PreencherInformacoesOrdemCarregamento()
        {
            CodigoDeBarras = $"OE_{_janelaCarregamento.Carga.Codigo}";
            CentroCarregamento = _janelaCarregamento.CentroCarregamento?.Descricao;
            DataHoraEmissao = _janelaCarregamento.Carga.DataInicioEmissaoDocumentos?.ToString("dd/MM/yyyy HH:mm");
            DataHoraEntrega = _janelaCarregamento.Carga.DataEncerramentoCarga?.ToString("dd/MM/yyyy HH:mm");
            Transportadora = _janelaCarregamento.Carga.Empresa?.Descricao;
            Endereco = _janelaCarregamento.CentroCarregamento?.Filial?.Localidade?.Descricao;
            TotalPesoBruto = _janelaCarregamento.Carga.DadosSumarizados?.PesoTotal.ToString("n3");
            TotalPesoLiquido = _janelaCarregamento.Carga.DadosSumarizados?.PesoLiquidoTotal.ToString("n3");
            NumeroPedido = _cargaPedido.Pedido.NumeroPedidoEmbarcador;
            NumeroOrdemVenda = _cargaPedido.Pedido.NumeroOrdem;
            CodigoPedido = _cargaPedido.Pedido.Codigo;
        }

        private void PreencherInformacoesDocumentoAuxiliarOrdemColeta(Entidades.Usuario motorista)
		{
            Dominio.Entidades.Embarcador.Cargas.Carga carga = _janelaCarregamento.Carga;

            Transportadora = carga?.Empresa?.Descricao ?? string.Empty;
            Remetente = carga?.Filial?.Descricao ?? string.Empty;
            TipoCarga = carga?.TipoDeCarga?.Descricao ?? string.Empty;
            ModeloVeicular = carga?.ModeloVeicularCarga?.Descricao ?? "";
            TelefoneMotorista = motorista?.Telefone_Formatado ?? string.Empty;
            Observacao = _janelaCarregamento.ObservacaoTransportador ?? string.Empty;
        }

        private void PreencherInformacoesPadroes()
        {
            ICollection<Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = _janelaCarregamento.Carga.Pedidos;

            string destino = _janelaCarregamento.Carga.DadosSumarizados?.Destinos ?? string.Empty;
            List<string> numerosCargasOriginais = new List<string>();

            if (!string.IsNullOrWhiteSpace(_janelaCarregamento.Carga.ObservacaoLocalEntrega))
                destino += " (" + _janelaCarregamento.Carga.ObservacaoLocalEntrega + ")";

            DateTime? dataPrevisaoEntrega = (from o in cargaPedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega).FirstOrDefault();

            if ((_janelaCarregamento?.Carga?.CodigosAgrupados?.Count ?? 0) > 0)
                numerosCargasOriginais = _janelaCarregamento.Carga.CodigosAgrupados.ToList();

            TipoCarga = _janelaCarregamento.Carga.TipoDeCarga?.Descricao ?? "";
            ModeloVeicular = _janelaCarregamento.Carga.ModeloVeicularCarga?.Descricao ?? "";
            NumeroEntregas = _janelaCarregamento.Carga.DadosSumarizados?.NumeroEntregas.ToString();
            ValorFrete = _janelaCarregamento.Carga.ValorFrete.ToString();
            ValorICMS = _janelaCarregamento.Carga.ValorICMS.ToString();
            ValorTotal = _janelaCarregamento.Carga.ValorFreteAPagar.ToString();
            Peso = _janelaCarregamento.Carga.DadosSumarizados?.PesoTotal.ToString("n2");
            Origem = _janelaCarregamento.Carga.DadosSumarizados?.Origens ?? "";
            Destino = destino;
            Destinatario = _janelaCarregamento.Carga.DadosSumarizados?.DestinatariosReais ?? "";
            Observacao = _janelaCarregamento.ObservacaoTransportador ?? "";
            ObservacaoCarregamento = _janelaCarregamento.Carga?.Carregamento?.Observacao ?? "";
            PrevisaoEntrega = dataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "";
            CodigosCargasEmbarcador = string.Join(", " , numerosCargasOriginais);
            NumeroPedido = string.Join(", ", cargaPedidos.Select(o => o.Pedido.NumeroPedidoEmbarcador));
        }

        #endregion
    }
}
