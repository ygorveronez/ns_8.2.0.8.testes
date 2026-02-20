using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class RelatorioRequisicaoMercadoria
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string Colaborador { get; set; }
        public string FuncionarioRequisitado { get; set; }
        public string Empresa { get; set; }
        public string Motivo { get; set; }
        public int Situacao { get; set; }
        public int Tipo { get; set; }
        public string Produto { get; set; }
        public string GrupoProduto { get; set; }
        public decimal Quantidade { get; set; }
        public string SetorFuncionario { get; set; }

        public string DescricaoSituacao
        {
            get
            {
                if (Enum.TryParse(Situacao.ToString(), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria situacao))
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoriaHelper.ObterDescricao(situacao);
                else
                    return string.Empty;
            }
        }
        public string DescricaoTipo
        {
            get
            {
                if (Enum.TryParse(Tipo.ToString(), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria tipo))
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoriaHelper.ObterDescricao(tipo);
                else
                    return string.Empty;
            }
        }
    }
}
