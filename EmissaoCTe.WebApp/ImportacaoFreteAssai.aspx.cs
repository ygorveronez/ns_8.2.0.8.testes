using System;

namespace EmissaoCTe.WebApp
{
    public partial class ImportacaoFreteAssai : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnEnviarArquivo_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            System.IO.StreamReader reader = new System.IO.StreamReader(this.fupArquivo.FileContent);

            Repositorio.FretePorTipoDeVeiculo repFreteTipoVeiculo = new Repositorio.FretePorTipoDeVeiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(1);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                double cpfCnpjOrigem = double.Parse(Utilidades.String.OnlyNumbers(values[0]));
                double cpfCnpjDestino = double.Parse(Utilidades.String.OnlyNumbers(values[1]));
                string descricaoTipoVeiculo = values[2];
                decimal valorFrete = decimal.Parse(values[3], new System.Globalization.CultureInfo("pt-BR"));
                decimal valorPedagio = decimal.Parse(values[4], new System.Globalization.CultureInfo("pt-BR"));
                DateTime dataInicial = DateTime.ParseExact(values[5], "dd/MM/yyyy", null);
                DateTime dataFinal = DateTime.ParseExact(values[6], "dd/MM/yyyy", null);

                Dominio.Entidades.Cliente origem = repCliente.BuscarPorCPFCNPJ(cpfCnpjOrigem);
                Dominio.Entidades.Cliente destino = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestino);
                Dominio.Entidades.TipoVeiculo tipoVeiculo = repTipoVeiculo.BuscarPorDescricao(empresa.Codigo, descricaoTipoVeiculo);

                if (origem != null && destino != null && tipoVeiculo != null)
                {
                    Dominio.Entidades.FretePorTipoDeVeiculo frete = repFreteTipoVeiculo.BuscarPorOrigemDestinoETipoVeiculo(empresa.Codigo, cpfCnpjOrigem, cpfCnpjDestino, tipoVeiculo.Codigo, null, false);

                    if (frete == null)
                        frete = new Dominio.Entidades.FretePorTipoDeVeiculo();

                    frete.ClienteOrigem = origem;
                    frete.ClienteDestino = destino;
                    frete.DataFinal = dataFinal;
                    frete.DataInicial = dataInicial;
                    frete.Empresa = empresa;
                    frete.Status = "A";
                    frete.TipoVeiculo = tipoVeiculo;
                    frete.ValorFrete = valorFrete;
                    frete.ValorPedagio = valorPedagio;

                    if (frete.Codigo > 0)
                        repFreteTipoVeiculo.Atualizar(frete);
                    else
                        repFreteTipoVeiculo.Inserir(frete);
                }
            }

        }
    }
}