using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class WebServicesConsultaNFe : RepositorioBase<Dominio.Entidades.WebServicesConsultaNFe>
    {
        public WebServicesConsultaNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.WebServicesConsultaNFe> BuscarNaoBloqueadas(int consultas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebServicesConsultaNFe>();

            var result = from obj in query where obj.DataBloqueio == null && obj.Consultas < consultas select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.WebServicesConsultaNFe> BuscarPorNumeroDeConsultas(int consultas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebServicesConsultaNFe>();

            var result = from obj in query where obj.DataBloqueio == null && obj.Consultas >= consultas select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.WebServicesConsultaNFe> BuscarBloqueadasPorData(DateTime dataHora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebServicesConsultaNFe>();

            var result = from obj in query where obj.DataBloqueio <= dataHora select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.WebServicesConsultaNFe> BuscarNaoBloqueadas(int quantidadeConsultas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita tipoWebService)
        {
            IQueryable<Dominio.Entidades.WebServicesConsultaNFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.WebServicesConsultaNFe>();

            query = query.Where(obj => obj.DataBloqueio == null && obj.Consultas < quantidadeConsultas && obj.Tipo == tipoWebService);

            return query.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.WebServicesConsultaNFe> BuscarPorNumeroDeConsultas(int quantidadeConsultas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita tipoWebService)
        {
            IQueryable<Dominio.Entidades.WebServicesConsultaNFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.WebServicesConsultaNFe>();

            query = query.Where(obj => obj.DataBloqueio == null && obj.Consultas >= quantidadeConsultas && obj.Tipo == tipoWebService);

            return query.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.WebServicesConsultaNFe> BuscarBloqueadasPorData(DateTime dataHora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceConsultaReceita tipoWebService)
        {
            IQueryable<Dominio.Entidades.WebServicesConsultaNFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.WebServicesConsultaNFe>();

            query = query.Where(o => o.DataBloqueio <= dataHora && o.Tipo == tipoWebService);

            return query.ToList();
        }
    }
}
