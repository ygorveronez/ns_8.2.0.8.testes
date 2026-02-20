using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Filiais
{
    public class SuprimentoDeGas : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas>
    {
        public SuprimentoDeGas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas> BuscarPorCliente(double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>()
                .Where(obj => obj.Cliente.CPF_CNPJ == cpfCnpjCliente);
            
            return query
                .Select(obj => obj.SuprimentoDeGas)
                .Fetch(obj => obj.TipoCargaPadrao)
                .Fetch(obj => obj.TipoOperacaoPadrao)
                .Fetch(obj => obj.ModeloVeicularPadrao)
                .Fetch(obj => obj.ProdutoPadrao)
                .Fetch(obj => obj.SupridorPadrao)
                .ToList();
        }
        
        public List<int> BuscarCodigosPorCliente(double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>()
                .Where(obj => obj.Cliente.CPF_CNPJ == cpfCnpjCliente);
            
            return query
                .Select(obj => obj.SuprimentoDeGas.Codigo)
                .ToList();
        }
        
        public Dominio.Entidades.Embarcador.Filiais.SuprimentoDeGas BuscarPorProdutoCliente(int codigoProduto, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>()
                .Where(obj => obj.Cliente.CPF_CNPJ == cpfCnpjCliente && obj.SuprimentoDeGas.ProdutoPadrao.Codigo == codigoProduto);
            
            return query
                .Select(obj => obj.SuprimentoDeGas)
                .FirstOrDefault();
        }
    }
}

