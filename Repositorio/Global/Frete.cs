using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class Frete : RepositorioBase<Dominio.Entidades.Frete>, Dominio.Interfaces.Repositorios.Frete
    {
        public Frete(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Frete> Consultar(int codigoEmpresa, string status, string nomeCliente, double cpfCnpjCliente, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Frete>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            if (!string.IsNullOrWhiteSpace(nomeCliente))
                result = result.Where(o => o.ClienteOrigem.Nome.Contains(nomeCliente) || o.ClienteOrigem.NomeFantasia.Contains(nomeCliente));
            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjCliente);
            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string status, string nomeCliente, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Frete>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            if (!string.IsNullOrWhiteSpace(nomeCliente))
                result = result.Where(o => o.ClienteOrigem.Nome.Contains(nomeCliente) || o.ClienteOrigem.NomeFantasia.Contains(nomeCliente));
            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjCliente);
            return result.Count();
        }

        public Dominio.Entidades.Frete BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Frete>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Frete BuscarPorOrigemEDestino(int codigoEmpresa, double cpfCnpjClienteOrigem, int codigoLocalidadeDestino, bool validarData = false, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento = null, Dominio.Enumeradores.TipoTomador? tipoCliente = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Frete>();

            var result = from obj in query
                         where obj.Status.Equals("A") &&
                               obj.Empresa.Codigo == codigoEmpresa &&
                               (obj.LocalidadeDestino.Codigo == codigoLocalidadeDestino || obj.LocalidadeDestino == null)
                         select obj;

            if (validarData)
                result = result.Where(o => (o.DataInicio == null || o.DataInicio <= DateTime.Now.Date) && (o.DataFim == null || o.DataFim >= DateTime.Now.Date));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            if (cpfCnpjClienteOrigem > 0)
            {
                result = result.Where(o => o.TipoCliente == tipoCliente && o.ClienteOrigem.CPF_CNPJ == cpfCnpjClienteOrigem);
            }
            else
                result = result.Where(o => o.ClienteOrigem == null);

            return result.OrderByDescending(o => o.LocalidadeDestino.Codigo).ThenByDescending(o => o.TipoPagamento).FirstOrDefault();
        }
    }
}
