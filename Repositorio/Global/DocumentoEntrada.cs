using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class DocumentoEntrada : RepositorioBase<Dominio.Entidades.DocumentoEntrada>, Dominio.Interfaces.Repositorios.DocumentoEntrada
    {
        public DocumentoEntrada(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DocumentoEntrada BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DocumentoEntrada BuscarPorParametros(int codigoEmpresa, int codigo, int numero, string serie, int codigoModelo, double cnpjFornecedor, Dominio.Enumeradores.StatusDocumentoEntrada? statusIgual, Dominio.Enumeradores.StatusDocumentoEntrada? statusDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            // Diferente
            if (codigo > 0)
                result = result.Where(o => o.Codigo != codigo);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(serie))
                result = result.Where(o => o.Serie.Equals(serie));

            if (codigoModelo > 0)
                result = result.Where(o => o.Modelo.Codigo == codigoModelo);

            if (cnpjFornecedor > 0)
                result = result.Where(o => o.Fornecedor.CPF_CNPJ == cnpjFornecedor);

            if (statusIgual != null)
                result = result.Where(o => o.Status == statusIgual);
            if (statusDiferente != null)
                result = result.Where(o => o.Status != statusDiferente);

            return result.FirstOrDefault();
        }

        public int BuscarUltimoNumeroLancamento(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = (from obj in query where obj.Empresa.Codigo == codigoEmpresa select (int?)obj.NumeroLancamento).Max();

            return result.HasValue ? result.Value : 0;
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataEntrada, DateTime dataEmissao, int numero, string nomeFornecedor, Dominio.Enumeradores.StatusDocumentoEntrada? status, int cfop)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(nomeFornecedor))
                result = result.Where(o => o.Fornecedor.Nome.Contains(nomeFornecedor));

            if (dataEntrada != DateTime.MinValue)
                result = result.Where(o => o.DataEntrada == dataEntrada.Date);

            if (dataEmissao != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao == dataEmissao);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            if (cfop > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.ItemDocumentoEntrada>();
                result = result.Where(o => (from obj in queryVeiculos where obj.CFOP.CodigoCFOP == cfop select obj.DocumentoEntrada.Codigo).Contains(o.Codigo));
            }

            return result.Count();
        }

        public List<Dominio.Entidades.DocumentoEntrada> Consultar(int codigoEmpresa, DateTime dataEntrada, DateTime dataEmissao, int numero, string nomeFornecedor, Dominio.Enumeradores.StatusDocumentoEntrada? status, int cfop, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(nomeFornecedor))
                result = result.Where(o => o.Fornecedor.Nome.Contains(nomeFornecedor));

            if (dataEntrada != DateTime.MinValue)
                result = result.Where(o => o.DataEntrada == dataEntrada.Date);

            if (dataEmissao != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao == dataEmissao);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            if (cfop > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.ItemDocumentoEntrada>();
                result = result.Where(o => (from obj in queryVeiculos where obj.CFOP.CodigoCFOP == cfop select obj.DocumentoEntrada.Codigo).Contains(o.Codigo));
            }

            return result.OrderByDescending(o => o.NumeroLancamento).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.DocumentoEntrada> BuscarPorStatusEModelos(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusDocumentoEntrada status, string[] modelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == status && modelos.Contains(obj.Modelo.Numero) && obj.DataEntrada >= dataInicial.Date && obj.DataEntrada < dataFinal.AddDays(1).Date select obj;

            return result.ToList();
        }

        public decimal BuscarTotalICMS(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusDocumentoEntrada status, string[] modelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == status && modelos.Contains(obj.Modelo.Numero) && obj.DataEntrada >= dataInicial.Date && obj.DataEntrada < dataFinal.AddDays(1).Date select obj;

            decimal? valor = result.Sum(o => (decimal?)o.ValorTotalICMS);

            return valor.HasValue ? valor.Value : 0m;
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosEntrada> Relatorio(int codigoEmpresa, int codigoVeiculo, double cpfCnpjFornecedor, DateTime dataInicial, DateTime dataFinal, DateTime dataEntradaInicial, DateTime dataEntradaFinal, int numeroInicial, int numeroFinal, Dominio.Enumeradores.StatusDocumentoEntrada? status, string ordenacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemDocumentoEntrada>();

            var result = from obj in query where obj.DocumentoEntrada.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoVeiculo > 0)
                result = result.Where(o => o.DocumentoEntrada.Veiculo.Codigo == codigoVeiculo);

            if (cpfCnpjFornecedor > 0f)
                result = result.Where(o => o.DocumentoEntrada.Fornecedor.CPF_CNPJ == cpfCnpjFornecedor);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DocumentoEntrada.DataEmissao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DocumentoEntrada.DataEmissao < dataFinal.AddDays(1));

            if (dataEntradaInicial != DateTime.MinValue)
                result = result.Where(o => o.DocumentoEntrada.DataEntrada >= dataEntradaInicial);

            if (dataEntradaFinal != DateTime.MinValue)
                result = result.Where(o => o.DocumentoEntrada.DataEntrada < dataEntradaFinal.AddDays(1));

            if (numeroInicial > 0)
                result = result.Where(o => o.DocumentoEntrada.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.DocumentoEntrada.Numero <= numeroFinal);

            if (status != null)
                result = result.Where(o => o.DocumentoEntrada.Status == status.Value);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosEntrada()
            {
                CodigoDocumentoEntrada = o.DocumentoEntrada.Codigo,
                CPFCNPJFornecedor = o.DocumentoEntrada.Fornecedor.CPF_CNPJ,
                DataEmissao = o.DocumentoEntrada.DataEmissao,
                DataEntrada = o.DocumentoEntrada.DataEntrada,
                NomeFornecedor = o.DocumentoEntrada.Fornecedor.Nome,
                Numero = o.DocumentoEntrada.Numero,
                Serie = o.DocumentoEntrada.Serie,
                Status = o.DocumentoEntrada.Status,
                ValorTotalDocumentoEntrada = o.DocumentoEntrada.ValorTotal,
                Veiculo = o.DocumentoEntrada.Veiculo.Placa,
                ValorTotalProdutos = o.DocumentoEntrada.ValorProdutos,
                ValorTotalDesconto = o.DocumentoEntrada.ValorTotalDesconto,
                ValorTotalOutrasDespesas = o.DocumentoEntrada.ValorTotalOutrasDespesas,
                ValorTotalFrete = o.DocumentoEntrada.ValorTotalFrete,
                ValorTotalBCICMS = o.DocumentoEntrada.BaseCalculoICMS,
                ValorTotalICMS = o.DocumentoEntrada.ValorTotalICMS,
                ValorTotalBCICMSST = o.DocumentoEntrada.BaseCalculoICMSST,
                ValorTotalICMSST = o.DocumentoEntrada.ValorTotalICMSST,
                ValorTotalIPI = o.DocumentoEntrada.ValorTotalIPI,
                ValorTotalPIS = o.DocumentoEntrada.ValorTotalPIS,
                ValorTotalCOFINS = o.DocumentoEntrada.ValorTotalCOFINS,
                EspercieDocumentoFiscal = o.DocumentoEntrada.Especie.Descricao,
                ModeloDocumentoFiscal = o.DocumentoEntrada.Modelo.Descricao,

                SequencialItem = o.Sequencial,
                CFOPItem = o.CFOP.CodigoCFOP,
                CSTItem = o.CST,
                ProdutoItem = o.Produto.Descricao,
                QuantidadeItem = o.Quantidade,
                ValorTotalItem = o.ValorTotal,
                ValorUnitarioItem = o.ValorUnitario,
                ValorDescontoItem = o.Desconto,
                ValorBCICMSItem = o.BaseCalculoICMS,
                AliquotaICMSItem = o.AliquotaICMS,
                ValorICMSItem = o.ValorICMS,
                CSTPISItem = o.CSTPIS,
                ValorPISItem = o.ValorPIS,
                CSTCOFINSItem = o.CSTCOFINS,
                ValorCOFINSItem = o.ValorCOFINS,
                CSTIPIItem = o.CSTIPI,
                ValorBCIPIItem = o.BaseCalculoIPI,
                AliquotaIPIItem = o.AliquotaIPI,
                ValorIPIItem = o.ValorIPI,
                ValorBCICMSSTItem = o.BaseCalculoICMSST,
                ValorICMSSTItem = o.ValorICMSST,
                ValorOutrasDespesasItem = o.OutrasDespesas,
                ValorFreteItem = o.ValorFrete

            }).OrderBy(ordenacao + " ascending").ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosEntradaCFOP> RelatorioCFOP(int codigoEmpresa, int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemDocumentoEntrada>();

            var result = from obj in query where obj.DocumentoEntrada.Empresa.Codigo == codigoEmpresa && obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioDocumentosEntradaCFOP()
            {
                CodigoDocumentoEntrada = o.DocumentoEntrada.Codigo,
                CFOPCodigo = o.CFOP.CodigoCFOP,
                CFOPDescricao = o.CFOP.Descricao,
                ValorTotalItem = o.ValorTotal,

            }).ToList();
        }

        public List<Dominio.Entidades.DocumentoEntrada> BuscarPorFornecedorNumeroSerieModelo(int codigoEmpresa, double cnpjFornecedor, int numero, string serie, int codigoModelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoEntrada>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status != Dominio.Enumeradores.StatusDocumentoEntrada.Cancelado select obj;

            if (cnpjFornecedor > 0f)
                result = result.Where(o => o.Fornecedor.CPF_CNPJ == cnpjFornecedor);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(serie))
                result = result.Where(o => o.Serie.Equals(serie));

            if (codigoModelo > 0)
                result = result.Where(o => o.Modelo.Codigo == codigoModelo);

            return result.ToList();
        }
    }
}
