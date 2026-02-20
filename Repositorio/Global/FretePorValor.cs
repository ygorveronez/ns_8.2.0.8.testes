using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class FretePorValor : RepositorioBase<Dominio.Entidades.FretePorValor>, Dominio.Interfaces.Repositorios.FretePorValor
    {
        public FretePorValor(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.FretePorValor> Consultar(int codigoEmpresa, string status, string nomeCliente, double cpfCnpjCliente, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorValor>();
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
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorValor>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            if (!string.IsNullOrWhiteSpace(nomeCliente))
                result = result.Where(o => o.ClienteOrigem.Nome.Contains(nomeCliente) || o.ClienteOrigem.NomeFantasia.Contains(nomeCliente));
            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjCliente);
            return result.Count();
        }

        public Dominio.Entidades.FretePorValor BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorValor>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FretePorValor BuscarPorOrigemEDestino(int codigoEmpresa, double cpfCnpjClienteOrigem, int codigoLocalidadeDestino, bool validarData, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorValor>();

            var result = from obj in query where obj.Status.Equals("A") && obj.Empresa.Codigo == codigoEmpresa && obj.LocalidadeDestino.Codigo == codigoLocalidadeDestino select obj;

            if (cpfCnpjClienteOrigem > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjClienteOrigem);
            else
                result = result.Where(o => o.ClienteOrigem == null);

            if (validarData)
                result = result.Where(o => (o.DataInicio == null || o.DataInicio <= DateTime.Now.Date) && (o.DataFim == null || o.DataFim >= DateTime.Now.Date));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            return result.OrderByDescending(o => o.ClienteOrigem.CPF_CNPJ).FirstOrDefault();
        }

        public Dominio.Entidades.FretePorValor BuscarParaCalculo(int codigoEmpresa, double cpfCnpjClienteOrigem, int codigoLocalidadeDestino, bool validarData = false, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento = null, string codigoIntegracao = "", Dominio.Enumeradores.TipoTomador tipoCliente = Dominio.Enumeradores.TipoTomador.Remetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorValor>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") select obj;

            if (cpfCnpjClienteOrigem > 0)
               result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cpfCnpjClienteOrigem);
            else
                result = result.Where(o => o.ClienteOrigem == null);

            result = result.Where(o => o.TipoCliente == tipoCliente);

            if (codigoLocalidadeDestino > 0)
                result = result.Where(o => o.LocalidadeDestino.Codigo == codigoLocalidadeDestino);
            else
                result = result.Where(o => o.LocalidadeDestino == null);

            if (validarData)
                result = result.Where(o => (o.DataInicio == null || o.DataInicio <= DateTime.Now.Date) && (o.DataFim == null || o.DataFim >= DateTime.Now.Date));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            if (!string.IsNullOrEmpty(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao.Equals(codigoIntegracao));

            return result.OrderByDescending(o => o.ClienteOrigem.CPF_CNPJ).FirstOrDefault();
        }
    }
}
