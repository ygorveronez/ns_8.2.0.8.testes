using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class PagamentoAgregado
    {
        public int Numero { get; set; }
        public DateTime DataPagamento { get; set; }
        public string Observacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado Situacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado Status { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataInicialOcorrencia { get; set; }
        public DateTime DataFinalOcorrencia { get; set; }
        public string NomeProprietario { get; set; }
        public double CNPJProprietario { get; set; }
        public decimal ValorPagamentoDocumento { get; set; }
        public int NumeroDocumento { get; set; }
        public int SerieDocumento { get; set; }
        public decimal ValorReceberDocumento { get; set; }
        public string Placas { get; set; }
        public string Motoristas { get; set; }
        public string Cargas { get; set; }
        public string TipoOperacao { get; set; }
        public decimal Peso { get; set; }
        public int CodigoTitulo { get; set; }
        public DateTime VencimentoTitulo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo StatusTitulo { get; set; }
        public decimal ValorPendenteTitulo { get; set; }

        public string DescricaoVencimentoTitulo
        {
            get
            {
                if (VencimentoTitulo != DateTime.MinValue)
                    return VencimentoTitulo.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }
        public string DescricaoDataFinalOcorrencia
        {
            get
            {
                if (DataFinalOcorrencia != DateTime.MinValue)
                    return DataFinalOcorrencia.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }
        public string DescricaoDataInicialOcorrencia
        {
            get
            {
                if (DataInicialOcorrencia != DateTime.MinValue)
                    return DataInicialOcorrencia.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }
        public string DescricaoDataFinalEmissao
        {
            get
            {
                if (DataFinalEmissao != DateTime.MinValue)
                    return DataFinalEmissao.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }
        public string DescricaoDataInicialEmissao
        {
            get
            {
                if (DataInicialEmissao != DateTime.MinValue)
                    return DataInicialEmissao.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }
        public string DescricaoDataPagamento
        {
            get
            {
                if (DataPagamento != DateTime.MinValue)
                    return DataPagamento.ToString("dd/MM/yyyy");
                else
                    return "";
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.AgAprovacao:
                        return "Ag. Aprovação";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Finalizado:
                        return "Finalizado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada:
                        return "Rejeitado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.SemRegra:
                        return "Sem Regra";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Iniciada:
                        return "Iniciado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoStatusTitulo
        {
            get
            {
                switch (this.StatusTitulo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Atrazada:
                        return "Atrazado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto:
                        return "Em aberto";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada:
                        return "Quitado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado:
                        return "Cancelado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Bloqueado:
                        return "Bloqueado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicio:
                        return "Ag. Inicio";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicioDocumentos:
                        return "Ag. Inicio Documentos";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.EmProcesso:
                        return "Em Processo";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.Finalizado:
                        return "Finalizado";
                    default:
                        return "";
                }
            }
        }
    }
}
