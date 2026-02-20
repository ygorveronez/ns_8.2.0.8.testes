using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Ocorrencias
{
    public class OcorrenciaEntrega
    {

        #region Propriedades Publicas
        public DateTime DataOcorrencia { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime DataPosicao { get; set; }
        public DateTime DataPrevisaoReprogramadaDaEntrega { get; set; }
        public string GrupoOcorrencia { get; set; }

        public string TempoPercurso { get; set; }
        public decimal DistanciaAteDestino { get; set; }
        public string Pacote { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string RemetenteNome { get; set; }
        public string RemetenteLocalidade { get; set; }
        public string ClienteLocalidade { get; set; }
        public string DescricaoCompleta { get; set; }
        public string DescricaoTipoOcorrencia { get; set; }
        public int Volumes { get; set; }
        public short OrigemOcorrencia { get; set; }

        public int NumeroOcorrencia { get; set; }
        public string CodigoIntegracaoTomador { get; set; }
        public string Tomador { get; set; }
        public string GrupoPessoas { get; set; }
        public string NotasFiscais { get; set; }
        public string SerieNotasFiscais { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public string NomeCreditor { get; set; }
        public string Chamado { get; set; }
        public string Transportadora { get; set; }
        public string CodigoIntegracaoRemetentes { get; set; }
        public string Remetentes { get; set; }
        public string CodigoIntegracaoDestinatarios { get; set; }
        public string Destinatarios { get; set; }
        public string CNPJDestinatarios { get; set; }
        public string Cliente { get; set; }
        public string Expedidor { get; set; }
        public string Recebedor { get; set; }
        public DateTime DataCarga { get; set; }
        public string MotivoCancelamento { get; set; }
        public string Motorista { get; set; }
        public string Placa { get; set; }
        public string TipoVeiculo { get; set; }
        public string CodigoIntegracaoFilial { get; set; }
        public string CNPJFilial { get; set; }
        public string Filial { get; set; }
        public string TipoOperacaoCarga { get; set; }
        public string CargaPeriodo { get; set; }
        public string MesPeriodo { get; set; }
        public string AnoPeriodo { get; set; }
        public string CargaAgrupada { get; set; }
        public string NomeFantasiaDestinatarios { get; set; }
        public string Pedidos { get; set; }
        public string Destinos { get; set; }
        public DateTime DataCarregamento { get; set; }
        public double CPFCNPJCliente { get; set; }
        public string CPFMotorista { get; set; }
        public string Observacao { get; set; }
        public int Codigo { get; set; }
        public string CNPJEmpresa { get; set; }
        public DateTime DataPrevisaoEntregaAjustada { get; set; }


        #endregion

        #region Propriedades com Regras
        public string CPFMotoristaFormatado
        {
            get
            {
                if (!string.IsNullOrEmpty(CPFMotorista))
                {
                    string[] cpfs = CPFMotorista.Trim().Split(',');
                    string cpfsFormatados = string.Empty;
                    for (var i = 0; i < cpfs.Length; i++)
                    {
                        if (i > 0)
                            cpfsFormatados += ", ";

                        cpfsFormatados += cpfs[i].Trim().ObterCpfOuCnpjFormatado();
                    }

                    return cpfsFormatados;
                }
                else
                    return string.Empty;
            }
        }

        public string CPFCNPJClienteDescricao
        {
            get { return CPFCNPJCliente > 0d ? CPFCNPJCliente.ToString().ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        public string PedidosFormatado
        {
            get { return !string.IsNullOrEmpty(Pedidos) ? (Pedidos.Contains("_") ? Pedidos.Split('_')[1].ToString() : Pedidos) : Pedidos; }
        }
        public string CNPJFilialFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CNPJFilial) ? string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJFilial)) : string.Empty; }
        }

        public string DataCargaFormatada
        {
            get { return DataCarga != DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CNPJDestinatariosFormatado
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CNPJDestinatarios))
                {
                    string[] cnpjs = CNPJDestinatarios.Trim().Split(',');
                    string cnpjsFormatados = string.Empty;
                    for (var i = 0; i < cnpjs.Length; i++)
                    {
                        if (i > 0)
                            cnpjsFormatados += ", ";

                        cnpjsFormatados += cnpjs[i].ObterCpfOuCnpjFormatado();
                    }

                    return cnpjsFormatados;
                }
                else
                    return "";
            }
        }

        public string CNPJTransportadora
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CNPJEmpresa))
                    return string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJEmpresa));
                else
                    return "";
            }
        }


        public string DataOcorrenciaFormatada
        {
            get { return DataOcorrencia != DateTime.MinValue ? DataOcorrencia.ToString("dd/MM/yyyy HH:mm") : ""; }
        }
        public string DataPrevisaoReprogramadaDaEntregaFormatada
        {
            get { return DataPrevisaoReprogramadaDaEntrega != DateTime.MinValue ? DataPrevisaoReprogramadaDaEntrega.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataPosicaoFormatada
        {
            get { return DataPosicao != DateTime.MinValue ? DataPosicao.ToString("dd/MM/yyyy HH:mm") : ""; }
        }
        public string OrigemFormatada
        {
            get { return OrigemOcorrencia > 0 ? OrigemCriacaoOcorrenciaHelper.ObterDescricao((OrigemCriacaoOcorrencia)OrigemOcorrencia) : ""; }
        }

        public string DescricaoOcorrenciaFormatada
        {
            get
            {
                return DescricaoCompleta?
                .Replace("#NomeCliente", Cliente)?
                .Replace("#NumeroCarga", CodigoCargaEmbarcador)?
                .Replace("#NomeRemetente", RemetenteNome)?
                .Replace("#CidadeOrigem", RemetenteLocalidade)?
                .Replace("#CidadeDestino", ClienteLocalidade)?
                .Replace("#MotivoOcorrencia", DescricaoTipoOcorrencia ?? "");
            }
        }

        public string DistanciaCalculada
        {
            get { return (DistanciaAteDestino > 0 ? (DistanciaAteDestino / 1000).ToString("n3") : "0") + " KM"; }
        }

        public string DataPrevisaoEntregaAjustadaFormatada
        {
            get { return DataPrevisaoEntregaAjustada != DateTime.MinValue ? DataPrevisaoEntregaAjustada.ToString("dd/MM/yyyy HH:mm") : ""; }
        }
        #endregion
    }
}
