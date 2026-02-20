using System;

namespace Dominio.Relatorios.Embarcador.DataSource.MDFe
{
    /*
     * Representa uma linha da pesquisa para o relatório
     */
    public class MDFesAverbados
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public Dominio.Enumeradores.StatusAverbacaoMDFe Situacao { get; set; }
        public string Retorno { get; set; }
        public string EstadoCarregamento { get; set; }
        public string EstadoDescarregamento { get; set; }
        public string Carga { get; set; }
        public string TipoCarga { get; set; }
        public string TipoOperacao { get; set; }
        public string Veiculos { get; set; }
        public string Motoristas { get; set; }
        public string CNPJEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public string Seguradora { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao Averbadora { get; set; }
        public string Apolice { get; set; }
        public string Averbacao { get; set; }
        public DateTime DataAverbacao { get; set; }
        public Dominio.Enumeradores.StatusMDFe SituacaoMDFe { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal Peso { get; set; }

        public string DescricaoSituacao
        {
            get
            {
                return Dominio.Enumeradores.StatusAverbacaoMDFeHelper.Descricao(Situacao);                
            }
        }

        public string DescricaoSituacaoMDFe
        {
            get
            {
                return Dominio.Enumeradores.StatusMDFeHelper.ObterDescricao(SituacaoMDFe);
            }
        }

        public string DescricaoAverbadora
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacaoHelper.Descricao(Averbadora);
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataAverbacaoFormatada
        {
            get { return DataAverbacao != DateTime.MinValue ? DataAverbacao.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
