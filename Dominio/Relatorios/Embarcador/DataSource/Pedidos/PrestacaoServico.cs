using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PrestacaoServico
    {
        #region Propriedades

        public bool Cotacao { get; set; }
        public decimal ValorFreteCotado { get; set; }
        public string CNPJEmpresa { get; set; }
        public string RazaoEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string RuaEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string NumeroEmpresa { get; set; }
        public string ComplementoEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string Motoristas { get; set; }
        public string Veiculos { get; set; }
        public string NumerosFrotaVeiculos { get; set; }
        public string VeiculosPedidos { get; set; }
        public string NumerosFrotaVeiculosPedido { get; set; }
        public string VeiculosCarga { get; set; }
        public string NumerosFrotaVeiculosCarga { get; set; }
        public string Produtos { get; set; }
        public string Modelo { get; set; }

        public decimal LimitePesoModeloVeicular { get; set; }
        public int Ajudantes { get; set; }

        // Pode ser remetente ou expedidor, tem case no SQL
        public double CnpjCpfRemetente { get; set; }
        public string TipoRemetente { get; set; }
        public string NomeRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string CidadeRemetente { get; set; }
        public string EstadoRemetente { get; set; }
        public string CidadeOrigemOriginal { get; set; }
        public string EstadoOrigemOriginal { get; set; }
        public string EnderecoOrigem { get; set; }
        public string BairroOrigem { get; set; }
        public string CEPOrigem { get; set; }
        public string ComplementoOrigem { get; set; }
        public string NumeroOrigem { get; set; }
        public string FoneOrigem { get; set; }
        public string CidadeOrigem { get; set; }
        public string EstadoOrigem { get; set; }

        // Pode ser destinatário ou receber, tem case no SQL
        public double CnpjCpfDestinatario { get; set; }
        public string TipoDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public string EnderecoDestino { get; set; }
        public string BairroDestino { get; set; }
        public string CEPDestino { get; set; }
        public string ComplementoDestino { get; set; }
        public string NumeroDestino { get; set; }
        public string FoneDestino { get; set; }
        public string CidadeDestino { get; set; }
        public string EstadoDestino { get; set; }

        // Atual remetente do pedido
        public double CnpjCpfRemetenteParticipante { get; set; }
        public string TipoRemetenteParticipante  { get; set; }
        public string NomeRemetenteParticipante  { get; set; }
        public string EnderecoRemetenteParticipante  { get; set; }
        public string BairroRemetenteParticipante  { get; set; }
        public string CEPRemetenteParticipante  { get; set; }
        public string ComplementoRemetenteParticipante  { get; set; }
        public string NumeroRemetenteParticipante  { get; set; }
        public string FoneRemetenteParticipante  { get; set; }
        public string CidadeRemetenteParticipante  { get; set; }
        public string EstadoRemetenteParticipante  { get; set; }

        // Atual destinatário do pedido
        public double CnpjCpfDestinatarioParticipante  { get; set; }
        public string TipoDestinatarioParticipante  { get; set; }
        public string NomeDestinatarioParticipante  { get; set; }
        public string EnderecoDestinatarioParticipante  { get; set; }
        public string BairroDestinatarioParticipante  { get; set; }
        public string CEPDestinatarioParticipante  { get; set; }
        public string ComplementoDestinatarioParticipante  { get; set; }
        public string NumeroDestinatarioParticipante  { get; set; }
        public string FoneDestinatarioParticipante  { get; set; }
        public string CidadeDestinatarioParticipante  { get; set; }
        public string EstadoDestinatarioParticipante  { get; set; }

        public bool EscoltaArmada { get; set; }
        public bool EscoltaMunicipal { get; set; }
        public bool Ajudante { get; set; }
        public string TipoCarga { get; set; }
        public string TipoOperacao { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoCTe { get; set; }
        public int QtdEntregas { get; set; }
        public int QtdVolumes { get; set; }
        public decimal PesoCarga { get; set; }
        public decimal PesoPaletes { get; set; }
        public decimal QtdPaletes { get; set; }
        public string Usuario { get; set; }
        public string Operador { get; set; }
        public DateTime DataCarregamento { get; set; }

        public string NomeLocalPaletizacao { get; set; }
        public string EnderecoLocalPaletizacao { get; set; }
        public string BairroLocalPaletizacao { get; set; }
        public string CEPLocalPaletizacao { get; set; }
        public string ComplementoLocalPaletizacao { get; set; }
        public string NumeroLocalPaletizacao { get; set; }
        public string FoneLocalPaletizacao { get; set; }
        public string CidadeLocalPaletizacao { get; set; }
        public string EstadoLocalPaletizacao { get; set; }

        public string DescricaoDataCarregamento
        {
            get
            {
                if (DataCarregamento != DateTime.MinValue)
                    return DataCarregamento.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
        public string DescricaoHoraCarregamento
        {
            get
            {
                if (DataCarregamento != DateTime.MinValue)
                    return DataCarregamento.ToString("HH:mm");
                else
                    return string.Empty;
            }
        }
        public DateTime DataPrevisaoFim { get; set; }
        public string DescricaoDataPrevisaoFim
        {
            get
            {
                if (DataPrevisaoFim != DateTime.MinValue)
                    return DataPrevisaoFim.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
        public string DescricaoHoraPrevisaoFim
        {
            get
            {
                if (DataPrevisaoFim != DateTime.MinValue)
                    return DataPrevisaoFim.ToString("HH:mm");
                else
                    return string.Empty;
            }
        }
        public DateTime DataPrevisaoInicio { get; set; }
        public string DescricaoDataPrevisaoInicio
        {
            get
            {
                if (DataPrevisaoInicio != DateTime.MinValue)
                    return DataPrevisaoInicio.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
        public string DescricaoHoraPrevisaoInicio
        {
            get
            {
                if (DataPrevisaoInicio != DateTime.MinValue)
                    return DataPrevisaoInicio.ToString("HH:mm");
                else
                    return string.Empty;
            }
        }
        public DateTime DataPrevisaoSaida { get; set; }
        public string DescricaoDataPrevisaoSaida
        {
            get
            {
                if (DataPrevisaoSaida != DateTime.MinValue)
                    return DataPrevisaoSaida.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
        public string DescricaoHoraPrevisaoSaida
        {
            get
            {
                if (DataPrevisaoSaida != DateTime.MinValue)
                    return DataPrevisaoSaida.ToString("HH:mm");
                else
                    return string.Empty;
            }
        }

        public string Numero { get; set; }
        public string NumeroCliente { get; set; }
        public int CodigoPedido { get; set; }
        public int ContemProduto { get; set; }
        public int ContemONUProduto { get; set; }
        public int ContemMotorista { get; set; }
        public int ContemImportacao { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataCarga { get; set; }
        public string DescricaoDataCarga
        {
            get
            {
                if (DataCarga != DateTime.MinValue)
                    return DataCarga.ToString("dd/MM/yyyy HH:mm");
                else
                    return string.Empty;
            }
        }
        
        public int TipoPagamento { get; set; }

        public string NomeContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.NomeOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.NomeRemetente;
                else
                    return this.NomeDestinatario;
            }
        }

        public string EnderecoContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.EnderedoOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.EnderecoOrigem;
                else
                    return this.EnderecoDestino;
            }
        }

        public string CEPContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.CEPOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.CEPOrigem;
                else
                    return this.CEPDestino;
            }
        }

        public string BairroContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.BairroOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.BairroOrigem;
                else
                    return this.BairroDestino;
            }
        }

        public string NumeroContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.NumeroOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.NumeroOrigem;
                else
                    return this.NumeroDestino;
            }
        }

        public string ComplementoContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.ComplementoOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.ComplementoOrigem;
                else
                    return this.ComplementoDestino;
            }
        }

        public string CidadeContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.CidadeOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.CidadeOrigem;
                else
                    return this.CidadeDestino;
            }
        }

        public string EstadoContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.EstadoOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.EstadoOrigem;
                else
                    return this.EstadoDestino;
            }
        }

        public string FoneContratante
        {
            get
            {
                if (this.TipoTomador == (int)Dominio.Enumeradores.TipoTomador.Outros)
                    return this.FoneOutroTomador;
                else if (this.TipoPagamento == (int)Dominio.Enumeradores.TipoPagamento.Pago)
                    return this.FoneOrigem;
                else
                    return this.FoneDestino;
            }
        }
        public int TipoTomador { get; set; }
        public string NomeOutroTomador { get; set; }
        public string EnderedoOutroTomador { get; set; }
        public string CidadeOutroTomador { get; set; }
        public string EstadoOutroTomador { get; set; }
        public string BairroOutroTomador { get; set; }
        public string CEPOutroTomador { get; set; }
        public string ComplementoOutroTomador { get; set; }
        public string NumeroOutroTomador { get; set; }
        public string FoneOutroTomador { get; set; }
        public string ObservacaoGeral { get; set; }
        public string CodigoPedidoCliente { get; set; }
        public DateTime DataColeta { get; set; }
        public string DatadeColeta
        {
            get
            {
                if (DataColeta != DateTime.MinValue)
                    return DataColeta.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
        public DateTime DataFinalColeta { get; set; }
        public string DataFinaldeColeta
        {
            get
            {
                if (DataFinalColeta != DateTime.MinValue)
                    return DataFinalColeta.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
        #endregion

        #region Propriedades com Regras

        public virtual string CnpjCpfRemetenteFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoRemetente))
                    return string.Empty;
                else if (TipoRemetente.Equals("E"))
                    return "00.000.000/0000-00";
                else
                    return TipoRemetente.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", CnpjCpfRemetente) : string.Format(@"{0:000\.000\.000\-00}", CnpjCpfRemetente);
            }
        }

        public virtual string CnpjCpfDestinatarioFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoDestinatario))
                    return string.Empty;
                else if (TipoDestinatario.Equals("E"))
                    return "00.000.000/0000-00";
                else
                    return TipoDestinatario.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", CnpjCpfDestinatario) : string.Format(@"{0:000\.000\.000\-00}", CnpjCpfDestinatario);
            }
        }

        #endregion
    }
}
