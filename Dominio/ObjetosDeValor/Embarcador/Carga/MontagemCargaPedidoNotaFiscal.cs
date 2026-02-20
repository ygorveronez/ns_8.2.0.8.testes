using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga;

public class MontagemCargaPedidoNotaFiscal
{
    #region Propriedades Publicas

    public int Codigo { get; set; }
    public int CodigoPedido { get; set; }
    public int NumeroNota { get; set; }
    public string Serie { get; set; }
    public string ChaveNota { get; set; }
    public DateTime DataEmissao { get; set; }
    public double RemetenteCPFCNPJ { get; set; }
    public string RemetenteNome { get; set; }
    public double DestinatarioCPFCNPJ { get; set; }
    public string DestinatarioNome { get; set; }
    public string CidadeOrigem { get; set; }
    public string UFOrigem { get; set; }
    public string CidadeDestino { get; set; }
    public string UFDestino { get; set; }
    public decimal Peso { get; set; }
    public decimal PesoLiquido { get; set; }
    public decimal Cubagem { get; set; }
    public int Volumes { get; set; }
    public double ExpedidorCPFCNPJ { get; set; }
    public string ExpedidorNome { get; set; }
    public double RecebedorCPFCNPJ { get; set; }
    public string RecebedorNome { get; set; }
    public string Filial { get; set; }
    public string NumeroPedidoEmbarcador { get; set; }
    public string GrupoPessoa { get; set; }

    #endregion Propriedades Publicas

    #region Propriedades com Regras

    public string DataNota { get => DataEmissao.ToString("dd/MM/yyyy HH:mm"); }

    public string Remetente
    {
        get
        {
            string descricao = string.Empty;

            if (!string.IsNullOrWhiteSpace(RemetenteNome))
                descricao += RemetenteNome;

            if (!RemetenteCPFCNPJ.Equals(0))
                descricao += " (" + RemetenteCPFCNPJ.ToString().ObterCpfOuCnpjFormatado() + ")";

            return descricao;
        }
    }

    public string Destinatario
    {
        get
        {
            string descricao = string.Empty;

            if (!string.IsNullOrWhiteSpace(DestinatarioNome))
                descricao += DestinatarioNome;

            if (!DestinatarioCPFCNPJ.Equals(0))
                descricao += " (" + DestinatarioCPFCNPJ.ToString().ObterCpfOuCnpjFormatado() + ")";

            return descricao;
        }
    }

    public string Expedidor
    {
        get
        {
            string descricao = string.Empty;

            if (!string.IsNullOrWhiteSpace(ExpedidorNome))
                descricao += ExpedidorNome;

            if (!ExpedidorCPFCNPJ.Equals(0))
                descricao += " (" + ExpedidorCPFCNPJ.ToString().ObterCpfOuCnpjFormatado() + ")";

            return descricao;
        }
    }

    public string Recebedor
    {
        get
        {
            string descricao = string.Empty;

            if (!string.IsNullOrWhiteSpace(RecebedorNome))
                descricao += RecebedorNome;

            if (!RecebedorCPFCNPJ.Equals(0))
                descricao += " (" + RecebedorCPFCNPJ.ToString().ObterCpfOuCnpjFormatado() + ")";

            return descricao;
        }
    }

    public string Origem
    {
        get
        {
            string descricao = string.Empty;

            if (!string.IsNullOrWhiteSpace(CidadeOrigem))
                descricao += CidadeOrigem;

            if (!string.IsNullOrWhiteSpace(UFOrigem))
                descricao += $" - {UFOrigem}";

            return descricao;
        }
    }

    public string Destino
    {
        get
        {
            string descricao = string.Empty;

            if (!string.IsNullOrWhiteSpace(CidadeDestino))
                descricao += CidadeDestino;

            if (!string.IsNullOrWhiteSpace(UFDestino))
                descricao += $" - {UFDestino}";

            return descricao;
        }
    }

    public int DiasPendentes
    {
        get
        {
            return DataEmissao.DifferenceOfDaysBetween(DateTime.Now);
        }
    }

    #endregion Propriedades com Regras
}
