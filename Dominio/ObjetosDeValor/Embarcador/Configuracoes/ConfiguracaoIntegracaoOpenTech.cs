namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public class ConfiguracaoIntegracaoOpenTech
    {
        public virtual string UsuarioOpenTech { get; set; }

        public virtual string SenhaOpenTech { get; set; }

        public virtual string DominioOpenTech { get; set; }

        public virtual int CodigoClienteOpenTech { get; set; }

        public virtual int CodigoPASOpenTech { get; set; }

        public virtual string URLOpenTech { get; set; }

        public virtual int CodigoProdutoPadraoOpentech { get; set; }

        public virtual int CodigoProdutoVeiculoComLocalizadorOpenTech { get; set; }

        public virtual bool CadastrarRotaCargaOpentech { get; set; }

        public virtual bool EnviarCodigoIntegracaoCentroCustoCargaOpenTech { get; set; }

        public virtual bool EnviarCodigoEmbarcadorProdutoOpentech { get; set; }

        public virtual bool EnviarDataPrevisaoEntregaDataCarregamentoOpentech { get; set; }

        public virtual bool EnviarDataAtualNaDataPrevisaoOpentech { get; set; }

        public virtual bool EnviarDataPrevisaoSaidaPedidoOpentech { get; set; }

        public virtual bool EnviarInformacoesRastreadorCavaloOpentech { get; set; }

        public virtual bool EnviarCodigoIntegracaoRotaCargaOpenTech { get; set; }

        public virtual bool EnviarNrfonecelBrancoOpenTech { get; set; }

        public virtual bool EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech { get; set; }

        public virtual bool EnviarValorNotasValorDocOpenTech { get; set; }

        public virtual bool CalcularPrevisaoEntregaComBaseDistanciaOpentech { get; set; }

        public virtual bool NotificarFalhaIntegracaoOpentech { get; set; }

        public virtual bool IntegrarRotaCargaOpentech { get; set; }

        public virtual bool IntegrarCargaOpenTechV10 { get; set; }

        public virtual bool IntegrarVeiculoMotorista { get; set; }

        public virtual bool ConsiderarLocalidadeProdutoIntegracaoEntrega { get; set; }

        public virtual bool EnviarValorDasNotasNoCampoValorDoc { get; set; }

        public virtual bool EnviarDataNFeNaDataPrevistaOpentech { get; set; }

        public virtual int CodigoProdutoColetaOpentech { get; set; }

        public virtual int CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador { get; set; }

        public virtual int CodigoProdutoColetaEmbarcadorOpentech { get; set; }

        public virtual int CodigoProdutoColetaTransportadorOpentech { get; set; }

        public virtual decimal ValorBaseOpenTech { get; set; }

        public virtual string EmailsNotificacaoFalhaIntegracaoOpentech { get; set; }

        public virtual bool AtualizarVeiculoMotoristaOpentech { get; set; }
    }
}