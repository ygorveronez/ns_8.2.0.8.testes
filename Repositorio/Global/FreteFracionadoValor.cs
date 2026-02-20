using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class FreteFracionadoValor : RepositorioBase<Dominio.Entidades.FreteFracionadoValor>, Dominio.Interfaces.Repositorios.FreteFracionadoValor
    {
        public FreteFracionadoValor(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.FreteFracionadoValor> Consultar(int codigoEmpresa, string status, string nomeCliente, double cpfCnpjCliente, string cidade, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoValor>();
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
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoValor>();
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

        public Dominio.Entidades.FreteFracionadoValor BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoValor>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.FreteFracionadoValor> Buscar(int codigoEmpresa, string status, double cpfCnpjCliente, Dominio.Enumeradores.TipoTomador? tipoCliente, int ibgeCidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoValor>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (cpfCnpjCliente > 0)
            {
                result = result.Where(o => o.TipoCliente == tipoCliente && o.ClienteOrigem.CPF_CNPJ == cpfCnpjCliente);
            }
            else
                result = result.Where(o => o.ClienteOrigem == null);

            if (ibgeCidade > 0)
                result = result.Where(o => o.LocalidadeDestino.CodigoIBGE == ibgeCidade);
            else
                result = result.Where(o => o.LocalidadeDestino == null);

            return result.OrderBy(o => o.ValorDe).OrderBy(o => o.ValorAte).ToList();
        }

        public Dominio.Entidades.FreteFracionadoValor BuscarPorClienteELocalidadeDestino(int codigoEmpresa, double cnpjCliente, int codigoDestino, Dominio.Enumeradores.TipoTomador tipoCliente, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFracionadoValor>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.LocalidadeDestino.Codigo == codigoDestino
                         select obj;

            if (cnpjCliente > 0)
            {
                result = result.Where(o => o.TipoCliente == tipoCliente && o.ClienteOrigem.CPF_CNPJ == cnpjCliente);
            }
            else
                result = result.Where(o => o.ClienteOrigem == null);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.FirstOrDefault();
        }

    }
}
