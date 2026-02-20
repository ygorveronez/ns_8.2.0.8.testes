using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoPedidoPacote
{
    public class RelacaoPedidoPacote
    {
        #region Propriedades

        public string CodigoCargaEmbarcador { get; set; }

        public DateTime DataCarregamentoCarga { get; set; }

        public string Filial { get; set; }

        public int CodigoEmpresa { get; set; }

        public string RazaoSocialEmpresa { get; set; }

        public string DescricaoLocalidade { get; set; }

        public int CodigoIBGE { get; set; }

        public string AbreviacaoPais { get; set; }

        public string NomePais { get; set; }

        public string SiglaEstado { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public virtual string TransportadoraFormatada
        {
            get
            {
                return $"{(this.CodigoEmpresa > 0 ? this.CodigoEmpresa + " " : string.Empty)}{(string.IsNullOrWhiteSpace(this.RazaoSocialEmpresa) ? string.Empty : this.RazaoSocialEmpresa + " ")}{(!string.IsNullOrEmpty(DescricaoCidadeEstado) ? $"({this.DescricaoCidadeEstado})" : string.Empty)}";
            }
        }

        public virtual string DescricaoCidadeEstado
        {
            get
            {
                if (this.CodigoIBGE != 9999999 || this.NomePais == null)
                    return this.DescricaoLocalidade + " - " + this.SiglaEstado;
                else
                {
                    if (this.AbreviacaoPais != null)
                        return this.DescricaoLocalidade + " - " + this.AbreviacaoPais;
                    else
                        return this.DescricaoLocalidade + " - " + this.NomePais;
                }
            }
        }

        public string DataCarregamentoCargaFormatada
        {
            get { return DataCarregamentoCarga != DateTime.MinValue ? DataCarregamentoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion Propriedades Com Regras
    }
}
