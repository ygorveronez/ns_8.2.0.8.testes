using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class FretePorTipoDeVeiculo : RepositorioBase<Dominio.Entidades.FretePorTipoDeVeiculo>, Dominio.Interfaces.Repositorios.FretePorTipoDeVeiculo
    {
        public FretePorTipoDeVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarConsulta(int codigoEmpresa, string status, string tipoVeiculo, string clienteOrigem, double cnpjOrigem, string clienteDestino, double cnpjDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                result = result.Where(o => o.TipoVeiculo.Descricao.Contains(tipoVeiculo));

            if (!string.IsNullOrWhiteSpace(clienteOrigem))
                result = result.Where(o => o.ClienteOrigem.Nome.Contains(clienteOrigem) || o.ClienteOrigem.NomeFantasia.Contains(clienteOrigem) || o.CidadeOrigem.Descricao.Contains(clienteOrigem));

            if (!string.IsNullOrWhiteSpace(clienteDestino))
                result = result.Where(o => o.ClienteDestino.Nome.Contains(clienteDestino) || o.ClienteDestino.NomeFantasia.Contains(clienteDestino) || o.CidadeDestino.Descricao.Contains(clienteDestino));

            if (cnpjOrigem > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cnpjOrigem);

            if (cnpjDestino > 0)
                result = result.Where(o => o.ClienteDestino.CPF_CNPJ == cnpjDestino);

            return result.Count();
        }

        public List<Dominio.Entidades.FretePorTipoDeVeiculo> Consultar(int codigoEmpresa, string status, string tipoVeiculo, string clienteOrigem, double cnpjOrigem, string clienteDestino, double cnpjDestino, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa
                         select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                result = result.Where(o => o.TipoVeiculo.Descricao.Contains(tipoVeiculo));

            if (!string.IsNullOrWhiteSpace(clienteOrigem))
                result = result.Where(o => o.ClienteOrigem.Nome.Contains(clienteOrigem) || o.ClienteOrigem.NomeFantasia.Contains(clienteOrigem) || o.CidadeOrigem.Descricao.Contains(clienteOrigem));

            if (!string.IsNullOrWhiteSpace(clienteDestino))
                result = result.Where(o => o.ClienteDestino.Nome.Contains(clienteDestino) || o.ClienteDestino.NomeFantasia.Contains(clienteDestino) || o.CidadeDestino.Descricao.Contains(clienteDestino));

            if (cnpjOrigem > 0)
                result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == cnpjOrigem);

            if (cnpjDestino > 0)
                result = result.Where(o => o.ClienteDestino.CPF_CNPJ == cnpjDestino);

            return result.Fetch(o => o.TipoVeiculo)
                         .Fetch(o => o.ClienteDestino)
                         .Fetch(o => o.ClienteOrigem)
                         .Fetch(o => o.CidadeDestino)
                         .Fetch(o => o.CidadeOrigem)
                         .OrderBy(o => o.TipoVeiculo.Descricao)
                         .ThenBy(o => o.ClienteOrigem.Nome)
                         .ThenBy(o => o.ClienteDestino.Nome)
                         .ThenBy(o => o.CidadeOrigem.Descricao)
                         .ThenBy(o => o.CidadeDestino.Descricao)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public Dominio.Entidades.FretePorTipoDeVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FretePorTipoDeVeiculo BuscarPorOrigemDestinoETipoVeiculo(int codigoEmpresa, double cpfCnpjOrigem, double cpfCnpjDestino, int codigoTipoVeiculo, string status, bool validarData = false, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.ClienteOrigem.CPF_CNPJ == cpfCnpjOrigem &&
                               obj.TipoVeiculo.Codigo == codigoTipoVeiculo
                         select obj;

            if (cpfCnpjDestino > 0)
                result = result.Where(o => o.ClienteDestino.CPF_CNPJ == cpfCnpjDestino);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            if (validarData)
                result = result.Where(o => (o.DataInicial == null || o.DataInicial <= DateTime.Now.Date) && (o.DataFinal == null || o.DataFinal >= DateTime.Now.Date));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FretePorTipoDeVeiculo BuscarPorLocalidadeOrigemDestinoETipoVeiculo(int codigoEmpresa, int codigoOrigem, int codigoDestino, int codigoTipoVeiculo, string status, bool validarData = false, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.CidadeOrigem.Codigo == codigoOrigem &&
                               obj.CidadeDestino.Codigo == codigoDestino &&
                               obj.TipoVeiculo.Codigo == codigoTipoVeiculo
                         select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            if (validarData)
                result = result.Where(o => (o.DataInicial == null || o.DataInicial <= DateTime.Now.Date) && (o.DataFinal == null || o.DataFinal >= DateTime.Now.Date));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FretePorTipoDeVeiculo BuscarPorOrigemDestinoETipoVeiculo(int codigoEmpresa, double cpfCnpjOrigem, double cpfCnpjDestino, int codigoLocalidadeOrigem, int codigoLocalidadeDestino, int codigoTipoVeiculo, string status, bool validarData = false, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               ((cpfCnpjOrigem > 0 && obj.ClienteOrigem.CPF_CNPJ == cpfCnpjOrigem) || (codigoLocalidadeOrigem > 0 && obj.CidadeOrigem.Codigo == codigoLocalidadeOrigem)) &&
                               ((cpfCnpjDestino > 0 && obj.ClienteDestino.CPF_CNPJ == cpfCnpjDestino) || (codigoLocalidadeDestino > 0 && obj.CidadeDestino.Codigo == codigoLocalidadeDestino)) &&
                               obj.TipoVeiculo.Codigo == codigoTipoVeiculo
                         select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            if (validarData)
                result = result.Where(o => (o.DataInicial == null || o.DataInicial <= DateTime.Now.Date) && (o.DataFinal == null || o.DataFinal >= DateTime.Now.Date));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FretePorTipoDeVeiculo BuscarPorOrigemDestinoEDescricaoTipoVeiculo(int codigoEmpresa, double cpfCnpjOrigem, double cpfCnpjDestino, string descricaoTipoVeiculo, string status, bool validarData = false, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.ClienteOrigem.CPF_CNPJ == cpfCnpjOrigem &&
                               obj.TipoVeiculo.Descricao.Equals(descricaoTipoVeiculo)
                         select obj;

            if (cpfCnpjDestino > 0)
                result = result.Where(o => o.ClienteDestino.CPF_CNPJ == cpfCnpjDestino);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            if (validarData)
                result = result.Where(o => (o.DataInicial == null || o.DataInicial <= DateTime.Now.Date) && (o.DataFinal == null || o.DataFinal >= DateTime.Now.Date));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FretePorTipoDeVeiculo BuscarPorLocalidadeOrigemDestinoEDescricaoTipoVeiculo(int codigoEmpresa, int codigoOrigem, int codigoDestino, string descricaoTipoVeiculo, string status, bool validarData = false, Dominio.Enumeradores.TipoPagamentoFrete? tipoPagamento = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorTipoDeVeiculo>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa &&
                               obj.CidadeOrigem.Codigo == codigoOrigem &&
                               obj.CidadeDestino.Codigo == codigoDestino &&
                               obj.TipoVeiculo.Descricao.Equals(descricaoTipoVeiculo)
                         select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (tipoPagamento != null && tipoPagamento.Value != Dominio.Enumeradores.TipoPagamentoFrete.Todos)
                result = result.Where(o => o.TipoPagamento == tipoPagamento || o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);
            else
                result = result.Where(o => o.TipoPagamento == Dominio.Enumeradores.TipoPagamentoFrete.Todos);

            if (validarData)
                result = result.Where(o => (o.DataInicial == null || o.DataInicial <= DateTime.Now.Date) && (o.DataFinal == null || o.DataFinal >= DateTime.Now.Date));

            return result.FirstOrDefault();
        }


    }
}
