using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class FaturaAvon : RepositorioBase<Dominio.Entidades.FaturaAvon>, Dominio.Interfaces.Repositorios.FaturaAvon
    {
        public FaturaAvon(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.FaturaAvon BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaAvon>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.FaturaAvon BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaAvon>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public int ContarFaturasPorManifesto(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaAvon>();

            var result = from obj in query where obj.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaAvon.Cancelada && obj.Manifestos.Contains(new Dominio.Entidades.ManifestoAvon() { Codigo = codigoManifesto }) select obj.Codigo;

            return result.Count();
        }

        public int ObterUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaAvon>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj.Numero;

            return result.Max(o => (int?)o) ?? 0;
        }

        public int ContarConsulta(int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaAvon>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.Count();
        }

        public List<Dominio.Entidades.FaturaAvon> Consultar(int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FaturaAvon>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.OrderByDescending(o => o.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }
    }
}
