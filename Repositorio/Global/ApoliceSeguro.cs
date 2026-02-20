using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ApoliceDeSeguro : RepositorioBase<Dominio.Entidades.ApoliceDeSeguro>, Dominio.Interfaces.Repositorios.ApoliceSeguro
    {
        public ApoliceDeSeguro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ApoliceDeSeguro BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ApoliceDeSeguro> Consultar(int codigoEmpresa, int codigoEmpresaPai, string nomeSeguradora, string numeroApolice, string ramo, string status, double cpfCnpjCliente, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa || o.Empresa.Codigo == codigoEmpresaPai);
            else
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjCliente);

            if (!string.IsNullOrWhiteSpace(nomeSeguradora))
                result = result.Where(o => o.NomeSeguradora.Contains(nomeSeguradora));

            if (!string.IsNullOrWhiteSpace(numeroApolice))
                result = result.Where(o => o.NumeroApolice.Contains(numeroApolice));

            if (!string.IsNullOrWhiteSpace(ramo))
                result = result.Where(o => o.Ramo.Contains(ramo));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoEmpresaPai, string nomeSeguradora, string numeroApolice, string ramo, string status, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa || o.Empresa.Codigo == codigoEmpresaPai);
            else
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjCliente);

            if (!string.IsNullOrWhiteSpace(nomeSeguradora))
                result = result.Where(o => o.NomeSeguradora.Contains(nomeSeguradora));

            if (!string.IsNullOrWhiteSpace(numeroApolice))
                result = result.Where(o => o.NumeroApolice.Contains(numeroApolice));

            if (!string.IsNullOrWhiteSpace(ramo))
                result = result.Where(o => o.Ramo.Contains(ramo));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.Entidades.ApoliceDeSeguro> ConsultarPorClienteEVigencia(int codigoEmpresa, int codigoEmpresaPai, double cpfCnpjCliente, string nomeSeguradora, string numeroApolice, string ramo, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query where (obj.Cliente.CPF_CNPJ == cpfCnpjCliente || obj.Cliente == null) && (obj.DataInicioVigencia.Value <= DateTime.Now.Date || obj.DataInicioVigencia == null) && (obj.DataFimVigencia.Value >= DateTime.Now.Date || obj.DataFimVigencia == null) && obj.Status.Equals("A") select obj;
            
            if (codigoEmpresa > 0 || codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa.Codigo == codigoEmpresa || o.Empresa.Codigo == codigoEmpresaPai));
            
            if (!string.IsNullOrWhiteSpace(nomeSeguradora))
                result = result.Where(o => o.NomeSeguradora.Contains(nomeSeguradora));
            
            if (!string.IsNullOrWhiteSpace(numeroApolice))
                result = result.Where(o => o.NumeroApolice.Contains(numeroApolice));
            
            if (!string.IsNullOrWhiteSpace(ramo))
                result = result.Where(o => o.Ramo.Contains(ramo));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorClienteEVigencia(int codigoEmpresa, int codigoEmpresaPai, double cpfCnpjCliente, string nomeSeguradora, string numeroApolice, string ramo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();

            var result = from obj in query where (obj.Cliente.CPF_CNPJ == cpfCnpjCliente || obj.Cliente == null) && (obj.DataInicioVigencia.Value <= DateTime.Now.Date || obj.DataInicioVigencia == null) && (obj.DataFimVigencia.Value >= DateTime.Now.Date || obj.DataFimVigencia == null) && obj.Status.Equals("A") select obj;
            
            if (codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa.Codigo == codigoEmpresa || o.Empresa.Codigo == codigoEmpresaPai));
            
            if (!string.IsNullOrWhiteSpace(nomeSeguradora))
                result = result.Where(o => o.NomeSeguradora.Contains(nomeSeguradora));
            
            if (!string.IsNullOrWhiteSpace(numeroApolice))
                result = result.Where(o => o.NumeroApolice.Contains(numeroApolice));
            
            if (!string.IsNullOrWhiteSpace(ramo))
                result = result.Where(o => o.Ramo.Contains(ramo));

            return result.Count();
        }

        public List<Dominio.Entidades.ApoliceDeSeguro> BuscarPorCliente(int codigoEmpresa, int codigoEmpresaPai, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query where (obj.Cliente.CPF_CNPJ == cpfCnpjCliente || obj.Cliente == null) && obj.DataInicioVigencia.Value <= DateTime.Now.Date && obj.DataFimVigencia >= DateTime.Now.Date && obj.Status.Equals("A") select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa.Codigo == codigoEmpresa || o.Empresa.Codigo == codigoEmpresaPai));
            
            return result.OrderByDescending(o => o.Cliente.CPF_CNPJ).ToList();
        }

        public List<Dominio.Entidades.ApoliceDeSeguro> BuscarPorEmpresaResponsavelVigencia(int codigoEmpresa, int responsavel, DateTime dataAtual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query where (obj.DataInicioVigencia.Value <= dataAtual || obj.DataInicioVigencia == null) && (obj.DataFimVigencia.Value >= dataAtual || obj.DataFimVigencia == null) && obj.Status.Equals("A") select obj;

            result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (responsavel > 0)
                result = result.Where(o => o.Responsavel == responsavel);

            return result.ToList();
        }

        public Dominio.Entidades.ApoliceDeSeguro BuscarApolicePorCliente(int codigoEmpresa, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpjCliente && obj.Status.Equals("A") && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.ApoliceDeSeguro BuscarPorEmpresaApolice(int codigoEmpresa, string numeroApolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApoliceDeSeguro>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.NumeroApolice == numeroApolice && obj.Status.Equals("A") select obj;

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }
    }
}
