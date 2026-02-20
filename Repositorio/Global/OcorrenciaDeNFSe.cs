using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class OcorrenciaDeNFSe : RepositorioBase<Dominio.Entidades.OcorrenciaDeNFSe>
    {
        public OcorrenciaDeNFSe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.OcorrenciaDeNFSe> ConsultarPorNFSe(int codigoEmpresa, int codigoNFSe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();
            
            var result = from obj in query
                         where 
                            obj.NFSe.Codigo == codigoNFSe && 
                            obj.NFSe.Empresa.Codigo == codigoEmpresa

                         orderby obj.DataDaOcorrencia descending
                         select obj;

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorNFSe(int codigoEmpresa, int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();

            var result = from obj in query
                         where
                            obj.NFSe.Codigo == codigoNFSe &&
                            obj.NFSe.Empresa.Codigo == codigoEmpresa

                         orderby obj.DataDaOcorrencia descending
                         select obj;

            return result.Count();
        }

        public int ContarOcorrenciaNFSe(int codigoNFSe, int codigoOcorrencia, DateTime dataOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();
            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe && obj.Ocorrencia.Codigo == codigoOcorrencia && obj.DataDaOcorrencia == dataOcorrencia select obj.Codigo;
            return result.Count();
        }

        public Dominio.Entidades.OcorrenciaDeNFSe BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();
            var result = from obj in query where obj.Codigo == codigo && obj.NFSe.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.OcorrenciaDeNFSe> _Consultar(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, int numeroNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();

            var result = from obj in query where obj.NFSe.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricaoTipoOcorrencia))
                result = result.Where(o => o.Ocorrencia.Descricao.Contains(descricaoTipoOcorrencia));

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.Observacao.Contains(observacao));

            if (numeroNFSe > 0)
                result = result.Where(o => o.NFSe.Numero == numeroNFSe);

            return result;
        }

        public List<Dominio.Entidades.OcorrenciaDeNFSe> Consultar(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, int numeroNFSe, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, descricaoTipoOcorrencia, observacao, numeroNFSe);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            result = result.Take(maximoRegistros);

            return result.OrderByDescending(o => o.DataDeCadastro).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, int numeroNFSe)
        {
            var result = _Consultar(codigoEmpresa, descricaoTipoOcorrencia, observacao, numeroNFSe);

            return result.Count();
        }

        private IQueryable<int> _FiltroOccorrenciaNFSe(int codigoEmpresa, string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusNFSe[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();

            var result = from obj in query
                         where
                             obj.NFSe.Empresa.Codigo == codigoEmpresa &&
                             obj.NFSe.DataEmissao >= dataInicial &&
                             obj.NFSe.DataEmissao < dataFinal.Date.AddDays(1) &&
                             obj.NFSe.Ambiente == tipoAmbiente &&
                             status.Contains(obj.NFSe.Status)
                             //obj.NFSe.Status == status
                         select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.NFSe.Tomador.CPF_CNPJ.Contains(cpfCnpjRemetente));

            return result.Select(o => o.NFSe.Codigo);
        }

        public List<Dominio.Entidades.NFSe> BuscarNFSesPorRemetente(int codigoEmpresa, string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusNFSe[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var queryNFSes = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();
            var query = _FiltroOccorrenciaNFSe(codigoEmpresa, cpfCnpjRemetente, dataInicial, dataFinal, status, tipoAmbiente);

            var result = from nfse in queryNFSes where query.Distinct().Contains(nfse.Codigo) select nfse;

            return result
                         .Fetch(o => o.Tomador)
                         .Fetch(o => o.Empresa)
                         .Fetch(o => o.Serie)
                         .ToList();
        }

        public List<string> BuscarRemetentesPorFiltro(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusNFSe[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var queryNFSes = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();
            var query = _FiltroOccorrenciaNFSe(codigoEmpresa, null, dataInicial, dataFinal, status, tipoAmbiente);

            var result = from nfse in queryNFSes where query.Distinct().Contains(nfse.Codigo) select nfse.Tomador.CPF_CNPJ;

            return result.ToList();
        }

        public Dominio.Entidades.OcorrenciaDeNFSe BuscarUltimaOcorrenciaPorNFSe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();

            var result = from o in query
                         where 
                            o.NFSe.Codigo == codigo
                         orderby o.DataDaOcorrencia descending
                         select o;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.OcorrenciaDeNFSe> BuscarPorEmpresaTomadorData(int codigoEmpresa, string cnpjTomador, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFSe>();

            var result = from obj in query where obj.NFSe.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                result = result.Where(o => o.NFSe.Tomador.CPF_CNPJ.Equals(cnpjTomador));

            if (dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
                result = result.Where(o => o.DataDeCadastro >= dataInicio && o.DataDeCadastro <= dataFim);

            return result.ToList();
        }

    }
}
