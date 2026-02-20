namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class OrdemServicoOrdemColeta
    {
        #region Cabecalho
        public string CabEndereco { get; set; }
        public string CabLocalidade { get; set; }
        public string CabCNPJ_IE { get; set; }
        public string CabTelefone { get; set; }
        public string CabNumeroPedido { get; set; }
        public string CabTipoOperacao { get; set; }
        public string CabDataEmissaoCarga { get; set; }
        #endregion

        #region CabPedido
        public string CabPedidoNumeroCarga { get; set; }
        public string CabPedidoOperacao { get; set; }
        public string CabPedidoMercadoria { get; set; }
        public string CabPedidoDestino { get; set; }
        public string CabPedidoCNPJ_Empresa_Filial { get; set; }
        public string CabPedidoPeso { get; set; }
        public string CabPedidoColeta { get; set; }
        public string CabPedidoTipoCarga { get; set; }
        #endregion

        #region Contratante
        public string ContRazaoSocial { get; set; }
        public string ContEndereco { get; set; }
        public string ContBairro { get; set; }
        public string ContMunicipio { get; set; }
        public string ContTelefone { get; set; }
        public string ContCEP { get; set; }
        public string ContUF { get; set; }
        #endregion

        #region DestinatarioCarga
        public string DestRazaoSocial { get; set; }
        public string DestEndereco { get; set; }
        public string DestBairro { get; set; }
        public string DestMunicipio { get; set; }
        public string DestTelefone { get; set; }
        public string DestCEP { get; set; }
        public string DestUF { get; set; }
        #endregion

        #region Coleta
        public string ColRazaoSocial { get; set; }
        public string ColEndereco { get; set; }
        public string ColBairro { get; set; }
        public string ColMunicipio { get; set; }
        public string ColTelefone { get; set; }
        public string ColCEP { get; set; }
        public string ColUF { get; set; }
        #endregion

        #region Entrega
        public string EntRazaoSocial { get; set; }
        public string EntEndereco { get; set; }
        public string EntBairro { get; set; }
        public string EntMunicipio { get; set; }
        public string EntTelefone { get; set; }
        public string EntCEP { get; set; }
        public string EntUF { get; set; }
        #endregion

        #region Transportador
        public string TraRazaoSocial { get; set; }
        public string TraMotorista { get; set; }
        public string TraPlacaCavalo { get; set; }
        public string TraRNTRCCavalo { get; set; }
        public string TraPlacaCarreta { get; set; }
        public string TraRNTRCCarreta { get; set; }
        public string TraModeloCavalo { get; set; }
        public string TraAnoCavalo { get; set; }
        public string TraChassiCarreta { get; set; }
        public string TraCNH { get; set; }
        public string TraUFCavalo { get; set; }
        public string TraChassiCavalo { get; set; }
        public string TraRenavanCarreta { get; set; }
        public string TraIdentidade { get; set; }
        public string TraCPF { get; set; }
        public string TraUFCarreta { get; set; }
        public string TraRenavanCavalo { get; set; }
        #endregion

        #region DetalhesCarga
        public string CarRota { get; set; }
        public string CarTarifa { get; set; }
        public string CarPercAdiantamento { get; set; }
        public string CarCartaoTransportador { get; set; }
        public string CarPedagio { get; set; }
        public string CarRazaoSocialEmpresaFilial { get; set; }
        #endregion

    }
}
