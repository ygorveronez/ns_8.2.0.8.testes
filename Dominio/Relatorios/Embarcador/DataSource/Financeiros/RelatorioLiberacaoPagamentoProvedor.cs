using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class RelatorioLiberacaoPagamentoProvedor
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroOS { get; set; }
        public DateTime DataCriacaoCarga { get; set; }
        public string TipoOperacao { get; set; }
        public string CidadeOrigemCarga { get; set; }
        public string EstadoOrigemCarga { get; set; }
        public string CidadeDestinoCarga { get; set; }
        public string EstadoDestinoCarga { get; set; }
        public string NomeFilialEmissoraCarga { get; set; }
        public string CNPJFilialEmissoraCarga { get; set; }
        public string NomeProvedor { get; set; }
        public string CNPJProvedor { get; set; }
        public string NomeTomadorCarga { get; set; }
        public string CNPJTomadorCarga { get; set; }
        public decimal ValorTotalEstimadoPagamento { get; set; }
        public EtapaLiberacaoPagamentoProvedor EtapaLiberacaoPagamentoProvedor { get; set; }
        public SituacaoLiberacaoPagamentoProvedor SituacaoLiberacaoPagamentoProvedor { get; set; }
        public int NumeroDocumentoProvedor { get; set; }
        public TipoDocumentoProvedor TipoDocumentoProvedor { get; set; }
        public bool IndicacaoLiberacaoOK { get; set; }
        public DateTime DataEmissaoDocumentoProvedor { get; set; }
        public string NumeroDocumentoCobrancaContraCliente { get; set; }
        public string TipoDocumentoCobrancaContraCliente { get; set; }
        public string ValorTotalDocumentoCobrancaContraCliente { get; set; }
        public string DataEmissaoDocumentoCobrancaContraCliente { get; set; }
        public decimal ValorTotalRealPagamentoCTe { get; set; }
        public decimal ValorTotalRealPagamentoNFSe { get; set; }
        public string JustificativaAprovacao { get; set; }
        public string UsuarioAlteracaoFrete { get; set; }
        public string NumerosDosMultiplosDocumentosProvedor { get; set; }
        public decimal AliquotaCTeProvedor { get; set; }
        public decimal ValorICMSProvedor { get; set; }

        #endregion

        #region Propriedades com Regras

        public string SituacaoLiberacaoPagamentoProvedorDescricao
        {
            get { return SituacaoLiberacaoPagamentoProvedor != SituacaoLiberacaoPagamentoProvedor.Todos ? SituacaoLiberacaoPagamentoProvedor.ObterDescricao() : string.Empty; }
        }

        public string EtapaLiberacaoPagamentoProvedorDescricao
        {
            get { return EtapaLiberacaoPagamentoProvedor != EtapaLiberacaoPagamentoProvedor.Todos ? EtapaLiberacaoPagamentoProvedor.ObterDescricao() : string.Empty; }
        }
        public string TipoDocumentoProvedorDescricao
        {
            get { return TipoDocumentoProvedor.ObterDescricao(); }
        }

        public string CNPJFilialEmissoraCargaFormatado
        {
            get { return CNPJFilialEmissoraCarga.ObterCnpjFormatado(); }
        }

        public string CNPJProvedorFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CNPJProvedor))
                {
                    return string.Empty;
                }

                var cnpjList = CNPJProvedor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var cnpjFormatadoList = cnpjList
                    .Select(cnpj => cnpj.Trim())
                    .Select(cnpj => cnpj.ObterCnpjFormatado())
                    .ToList();

                return string.Join(", ", cnpjFormatadoList);
            }
        }

        public string CNPJTomadorCargaFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CNPJTomadorCarga))
                {
                    return string.Empty;
                }

                var cnpjList = CNPJTomadorCarga.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var cnpjFormatadoList = cnpjList
                    .Select(cnpj => cnpj.Trim())
                    .Select(cnpj => cnpj.ObterCnpjFormatado())
                    .ToList();

                return string.Join(", ", cnpjFormatadoList);
            }
        }

        public string DataEmissaoDocumentoProvedorFormatada
        {
            get { return DataEmissaoDocumentoProvedor != DateTime.MinValue ? DataEmissaoDocumentoProvedor.ToString("d") : string.Empty; }
        }

        public string DataEmissaoDocumentoCobrancaContraClienteFormatada
        {
            get { return !string.IsNullOrWhiteSpace(DataEmissaoDocumentoCobrancaContraCliente) ? DataEmissaoDocumentoCobrancaContraCliente : string.Empty; }
        }

        public string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga.ToString("d"); }
        }

        public string TipoDocumentoCobrancaContraClienteDescricao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoDocumentoCobrancaContraCliente))
                    return string.Empty;

                var tokens = TipoDocumentoCobrancaContraCliente
                                .Split(',')
                                .Select(t => t.Trim())
                                .Where(t => !string.IsNullOrEmpty(t));

                var descricoes = new List<string>();

                foreach (var token in tokens)
                {
                    // se for número e existir no enum → converte; senão, mantém o texto original
                    if (int.TryParse(token, out int valorInt) &&
                        Enum.IsDefined(typeof(Dominio.Enumeradores.TipoDocumento), valorInt))
                    {
                        var tipoEnum = (Dominio.Enumeradores.TipoDocumento)valorInt;
                        descricoes.Add(tipoEnum.ObterDescricao()); 
                    }
                    else
                    {
                        descricoes.Add(token);   // “ABONO”, “OUT”, etc.
                    }
                }

                return string.Join(", ", descricoes);
            }
        }

        public string ValorTotalDocumentoCobrancaContraClienteFormatado
        {
            get
            {
                if (decimal.TryParse(ValorTotalDocumentoCobrancaContraCliente, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor))
                {
                    return valor.ToString("#,##0.00", CultureInfo.GetCultureInfo("pt-BR"));
                }
                return string.Empty;
            }
        }

        public string IndicacaoLiberacaoOKDescricao
        {
            get
            {
                return IndicacaoLiberacaoOK ? "Sim" : "Não";
            }
        }
    }
    #endregion
}