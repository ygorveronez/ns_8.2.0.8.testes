using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class FreteFracionadoUnidade : RepositorioBase<Dominio.Entidades.FreteFracionadoUnidade>, Dominio.Interfaces.Repositorios.FreteFracionadoUnidade
    {
        public FreteFracionadoUnidade(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.FreteFracionadoUnidade> Consultar(int codigoEmpresa, string status, string nomeCliente, double cpfCnpjCliente, string cidade, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoUnidade>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nomeCliente))
                result = result.Where(o => o.ClienteOrigem.Nome.Contains(nomeCliente) || o.ClienteOrigem.NomeFantasia.Contains(nomeCliente));
            
            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjCliente);

            if (!string.IsNullOrWhiteSpace(cidade))
                result = result.Where(o => o.LocalidadeDestino.Descricao.Contains(cidade));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }


        public int ContarConsulta(int codigoEmpresa, string status, string nomeCliente, string cidade, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoUnidade>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nomeCliente))
                result = result.Where(o => o.ClienteOrigem.Nome.Contains(nomeCliente) || o.ClienteOrigem.NomeFantasia.Contains(nomeCliente));

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjCliente);

            if (!string.IsNullOrWhiteSpace(cidade))
                result = result.Where(o => o.LocalidadeDestino.Descricao.Contains(cidade));

            return result.Count();
        }

        public Dominio.Entidades.FreteFracionadoUnidade BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoUnidade>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.FreteFracionadoUnidade> Buscar(int codigoEmpresa, string status, double cpfCnpjCliente, Dominio.Enumeradores.TipoTomador? tipoCliente, int ibgeCidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoUnidade>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (cpfCnpjCliente > 0)
            {
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjCliente);

                if (tipoCliente != null)
                    result = result.Where(o => o.TipoCliente == tipoCliente);
            }
            else
                result = result.Where(o => o.ClienteOrigem == null);

            if (ibgeCidade > 0)
                result = result.Where(o => o.LocalidadeDestino.CodigoIBGE == ibgeCidade);
            else
                result = result.Where(o => o.LocalidadeDestino == null);

            return result.OrderBy(o => o.PesoDe).OrderBy(o => o.PesoAte).ToList();
        }

    }
}
