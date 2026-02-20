using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.CTe
{
    public class Cliente
    {
        public string CPFCNPJ { get; set; }

        public string CPFCNPJFormatado { get; set; }

        public string CGC { get; set; }

        public string NumeroDocumentoExportacao { get; set; }

        public int CodigoAtividade { get; set; }

        public string RGIE { get; set; }

        public string IM { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string Telefone1 { get; set; }

        public string Telefone2 { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Bairro { get; set; }

        public string Complemento { get; set; }

        public string CEP { get; set; }

        public string Emails { get; set; }

        public bool StatusEmails { get; set; }

        public string EmailsContato { get; set; }

        public bool StatusEmailsContato { get; set; }

        public string EmailsContador { get; set; }

        public bool StatusEmailsContador { get; set; }

        public int CodigoIBGECidade { get; set; }

        public string CodigoPais { get; set; }

        public bool Exportacao { get; set; }

        public bool ExigeEtiquetagem { get; set; }
        public bool ExigeQueSuasEntregasSejamAgendadas { get; set; }

        public string Cidade { get; set; }

        public string CodigoLocalidadeEmbarcador { get; set; }

        /// <summary>
        /// Apenas não atualiza os dados do endereço do cliente
        /// </summary>
        public bool NaoAtualizarEndereco { get; set; }

        /// <summary>
        /// Não atualiza qualquer dado do cliente
        /// </summary>
        public bool NaoAtualizarDadosCadastrais { get; set; }

        public string EmailTransportador;

        public string CodigoEndereco;

        public string EmailTransportadorStatus;

        public string CodigoCliente { get; set; }

        public double Codigo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string CodigGrupoPessoa { get; set; }
        public TipoPais Pais { get; set; }

        public string CodigoCategoria { get; set; }
        public string Observacao { get; set; }
        public bool? CompartilharAcessoEntreGrupoPessoas { get; set; }
        public string ContaContabil { get; set; }

        public bool TipoCliente { get; set; }
        public bool TipoFornecedor { get; set; }
        public bool TipoTransportador { get; set; }

        #region Modalidade Transportador
        public string RNTRC { get; set; }
        public DateTime? DataEmissaoRNTRC { get; set; }
        public DateTime? DataVencimentoRNTRC { get; set; }
        public TipoProprietarioVeiculo? TipoTransportadorTerceiro { get; set; }
        public bool ReterImpostosContratoFrete { get; set; }
        public int? DiasVencimentoAdiantamentoContratoFrete { get; set; }
        public int? DiasVencimentoSaldoContratoFrete { get; set; }
        public decimal PercentualAdiantamentoFretesTerceiro { get; set; }
        public decimal PercentualAbastecimentoFretesTerceiro { get; set; }
        #endregion

        /// <summary>
        /// Utilizado apenas para o ambiente MultiNFe
        /// </summary>
        public int CodigoEmpresa { get; set; }

        public decimal ValorMinimoCarga { get; set; }

        public string CodigoTipoDeCarga { get; set; }

        public bool GerarPedidoBloqueado { get; set; }
        public bool ExcecaoCheckinFilaH { get; set; }
        public int? RaioEmMetros { get; set; }
        public string TipoTerceiro { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE? CodigoIndicadorIE { get; set; }
        public string EmailStatus { get; set; }
        public string PISPASEP { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string EmailSecundario { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoEmail? TipoEmailSecundario { get; set; }
        public bool AtivarAcessoPortal { get; set; }
        public bool VisualizarApenasParaPedidoDesteTomador { get; set; }
        public int CodigoBanco { get; set; }
        public string Agencia { get; set; }
        public string Digito { get; set; }
        public string NumeroConta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }

        public bool TerceiroGerarCIOT { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT? TerceiroTipoCIOT { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT? TerceiroTipoFavorecidoCIOT { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? TerceiroTipoPagamentoCIOT { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT? TerceiroTipoQuitacaoCIOT { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoQuitacaoCIOT? TerceiroTipoAdiantamentoCIOT { get; set; }


        public string Contato { get; set; }
        public int ContatoTipo { get; set; }
        public bool ContatoAtivo { get; set; }
        public string ContatoEmail { get; set; }
        public string ContatoTelefone { get; set; }
        public string ContatoCPF { get; set; }
        public string ContatoCargo { get; set; }

        public string NomeSocio { get; set; }
        public string CPFSocio { get; set; }

    }
}
