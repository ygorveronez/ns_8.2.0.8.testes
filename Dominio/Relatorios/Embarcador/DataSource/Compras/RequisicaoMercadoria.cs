using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class RequisicaoMercadoria
    {
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public string Motivo { get; set; }
        public string FantasiaEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string NomeColaborador { get; set; }
        public string NomeFuncionarioRequisitado { get; set; }
        public int CodigoItemBanco { get; set; }
        public int CodigoItem { get; set; }
        public string DescricaoItem { get; set; }
        public decimal QuantidadeItem { get; set; }
        public string Observacao { get; set; }
        public string Setor { get; set; }
        public DateTime DataAdmisao { get; set; }
        public string CidadeFuncionario { get; set; }
        public string EstadoFuncionario { get; set; }
        public string EnderecoFuncionario { get; set; }
        public string CEPFuncionario { get; set; }
        public string Bairro { get; set; }
        public string NumeroEnderecoFuncionario { get; set; }

        public string NumeroCA { get; set; }
        public bool ProdutoEPI { get; set; }
        public int ContemEPI { get; set; }

        public decimal CustoUnitario { get; set; }

        public decimal CustoTotal { get; set; }

        public string DataAdmisao_Formatado
        {
            get
            {
                if (this.DataAdmisao > DateTime.MinValue)
                    return this.DataAdmisao.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }
    }
}