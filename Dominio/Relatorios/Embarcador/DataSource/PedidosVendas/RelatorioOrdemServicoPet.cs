using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Text;
using Utilidades.Extensions;

namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioOrdemServicoPet
    {
        public int Numero { get; set; }
        public decimal Peso { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataEntrega { get; set; }
        public int StatusOrdemServico { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacao { get; set; }

        public string FantasiaEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }

        public int CodigoItemBanco { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoServico { get; set; }
        public string CodigoItem { get; set; }
        public string DescricaoItem { get; set; }
        public string CodigoNCM { get; set; }
        public decimal QuantidadeItem { get; set; }
        public decimal ValorUnitarioItem { get; set; }
        public decimal ValorTotalItem { get; set; }

        public string PetNome { get; set; }
        public int SexoAnimal { get; set; }
        public DateTime DataNascimento { get; set; }
        public int PorteAnimal { get; set; }
        public string Especie { get; set; }
        public string Raca { get; set; }
        public string Cor { get; set; }

        public string NomePessoa { get; set; }
        public string FonePessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string EnderecoPessoa { get; set; }
        public string BairroPessoa { get; set; }
        public string CEPPessoa { get; set; }
        public string NumeroEnderecoPessoa { get; set; }
        public string CidadePessoa { get; set; }
        public string EstadoPessoa { get; set; }
        public string TipoPessoa { get; set; }

        public string NomeFuncionario { get; set; }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao.ToString("dd/MM/yyyy"); }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega.ToString("dd/MM/yyyy"); }
        }

        public string CNPJEmpresaFormatado
        {
            get { return CNPJEmpresa.ObterCpfOuCnpjFormatado(TipoEmpresa); }
        }

        public string CNPJPessoaFormatado
        {
            get { return CNPJPessoa.ToString().ObterCpfOuCnpjFormatado(TipoPessoa); }
        }

        public string SexoDescricao
        {
            get
            {
                switch (SexoAnimal)
                {
                    case 1: return Localization.Resources.Enumeradores.AnimalSexo.Macho;
                    case 2: return Localization.Resources.Enumeradores.AnimalSexo.Femea;
                    default: return Localization.Resources.Enumeradores.AnimalSexo.NaoInformado;
                }
            }
        }

        public string PorteDescricao
        {
            get
            {
                var porte = (Porte)PorteAnimal;
                return porte.ObterDescricao();
            }
        }

        public int Idade
        {
            get
            {
                DateTime now = DateTime.Today;
                int idade = now.Year - DataNascimento.Year;
                if (DataNascimento.AddYears(idade) > now)
                    idade--;
                return idade;
            }
        }
    }
}
