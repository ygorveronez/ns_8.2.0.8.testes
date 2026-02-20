using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class ClienteComissao : RepositorioBase<Dominio.Entidades.ClienteComissao>, Dominio.Interfaces.Repositorios.ClienteComissao
    {
        public ClienteComissao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ClienteComissao BuscaPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteComissao>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ClienteComissao> Consultar(int codigoEmpresa, string nomeParceiro, string nomeCidade, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteComissao>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(nomeParceiro))
                result = result.Where(o => o.Parceiro.Nome.Contains(nomeParceiro));

            if (!string.IsNullOrWhiteSpace(nomeCidade))
                result = result.Where(o => o.Localidade.Descricao.Contains(nomeCidade));

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string nomeParceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteComissao>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(nomeParceiro))
                result = result.Where(o => o.Parceiro.Nome.Contains(nomeParceiro));

            return result.Count();
        }

        public decimal BuscaValorComissao(int codigoEmpresa, double CNPJCPFParceiro, int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteComissao>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (CNPJCPFParceiro > 0)
                result = result.Where(o => o.Parceiro.CPF_CNPJ == CNPJCPFParceiro);

            if (codigoLocalidade > 0)
                result = result.Where(o => o.Localidade.Codigo == codigoLocalidade);

            return result.FirstOrDefault().PercentualComissao;
        }

        public decimal BuscaValorMinimo(int codigoEmpresa, double CNPJCPFParceiro, int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteComissao>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (CNPJCPFParceiro > 0)
                result = result.Where(o => o.Parceiro.CPF_CNPJ == CNPJCPFParceiro);

            if (codigoLocalidade > 0)
                result = result.Where(o => o.Localidade.Codigo == codigoLocalidade);

            return result.FirstOrDefault().MinimoComissao;
        }
                
        public Dominio.Entidades.ClienteComissao BuscaPorParceiroLocalidade(int codigoEmpresa, double CNPJCPFParceiro, int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ClienteComissao>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == Dominio.Enumeradores.StatusComissaoCliente.Ativo select obj;

            if (CNPJCPFParceiro > 0)
                result = result.Where(o => o.Parceiro.CPF_CNPJ == CNPJCPFParceiro);

            if (codigoLocalidade > 0)
                result = result.Where(o => o.Localidade.Codigo == codigoLocalidade);

            return result.FirstOrDefault();
        }

    }
}
