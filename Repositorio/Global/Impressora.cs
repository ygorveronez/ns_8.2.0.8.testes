using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class Impressora : RepositorioBase<Dominio.Entidades.Impressora>, Dominio.Interfaces.Repositorios.Impressora
    {
        public Impressora(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Impressora> Buscar(int numeroUnidade, string status, string nomeImpressora, string documento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Impressora>();
            var result = from obj in query select obj;

            if (numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nomeImpressora))
                result = result.Where(o => o.NomeImpressora.Contains(nomeImpressora));

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(string.Empty) || o.Documento.Equals(documento));

            return result.ToList();
        }

        public Dominio.Entidades.Impressora BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Impressora>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Impressora BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Impressora>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) && obj.Status.Equals("A")  select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Impressora BuscarPorUnidade(int numeroUnidade, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Impressora>();
            var result = from obj in query where obj.NumeroDaUnidade == numeroUnidade select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Impressora BuscarPorUnidadeDocumento(int numeroUnidade, string codigoIntegracao, string documento, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Impressora>();
            var result = from obj in query where obj.NumeroDaUnidade == numeroUnidade select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(documento));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao.Equals(codigoIntegracao));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Impressora> Consultar(int numeroUnidade, string nomeImpressora, string status, string documento, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Impressora>();
            var result = from obj in query select obj;

            if (numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nomeImpressora))
                result = result.Where(o => o.NomeImpressora.Contains(nomeImpressora));

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(documento));

            return result.OrderBy(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int numeroUnidade, string nomeImpressora, string status, string documento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Impressora>();
            var result = from obj in query select obj;

            if (numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nomeImpressora))
                result = result.Where(o => o.NomeImpressora.Contains(nomeImpressora));

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(documento));

            return result.Count();
        }
    }
}