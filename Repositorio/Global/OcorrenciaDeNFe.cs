using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class OcorrenciaDeNFe : RepositorioBase<Dominio.Entidades.OcorrenciaDeNFe>
    {
        public OcorrenciaDeNFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.OcorrenciaDeNFe> ConsultarPorNFe(int codigoEmpresa, int codigoNFe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();

            var result = from obj in query
                         where
                            obj.NFe.Codigo == codigoNFe &&
                            obj.NFe.Empresa.Codigo == codigoEmpresa

                         orderby obj.DataDaOcorrencia descending
                         select obj;

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorNFe(int codigoEmpresa, int codigoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();

            var result = from obj in query
                         where
                            obj.NFe.Codigo == codigoNFe &&
                            obj.NFe.Empresa.Codigo == codigoEmpresa

                         orderby obj.DataDaOcorrencia descending
                         select obj;

            return result.Count();
        }

        public int ContarOcorrenciaNFe(int codigoNFe, int codigoOcorrencia, DateTime dataOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();
            var result = from obj in query where obj.NFe.Codigo == codigoNFe && obj.Ocorrencia.Codigo == codigoOcorrencia && obj.DataDaOcorrencia == dataOcorrencia select obj.Codigo;
            return result.Count();
        }

        public Dominio.Entidades.OcorrenciaDeNFe BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.NFe.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.OcorrenciaDeNFe> _Consultar(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, string numeroNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();

            var result = from obj in query where obj.NFe.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricaoTipoOcorrencia))
                result = result.Where(o => o.Ocorrencia.Descricao.Contains(descricaoTipoOcorrencia));

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.Observacao.Contains(observacao));

            if (!string.IsNullOrWhiteSpace(numeroNFe))
                result = result.Where(o => o.NFe.Numero == numeroNFe);

            return result;
        }

        public List<Dominio.Entidades.OcorrenciaDeNFe> Consultar(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, string numeroNFe, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, descricaoTipoOcorrencia, observacao, numeroNFe);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            result = result.Take(maximoRegistros);

            return result.OrderByDescending(o => o.DataDeCadastro).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, string numeroNFe)
        {
            var result = _Consultar(codigoEmpresa, descricaoTipoOcorrencia, observacao, numeroNFe);

            return result.Count();
        }

        private IQueryable<int> _FiltroOccorrenciaNFe(int codigoEmpresa, double cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();

            var result = from obj in query
                         where
                             obj.NFe.Empresa.Codigo == codigoEmpresa &&
                             obj.NFe.DataEmissao >= dataInicial &&
                             obj.NFe.DataEmissao < dataFinal.Date.AddDays(1) 
                         select obj;

            if (cpfCnpjRemetente > 0)
                result = result.Where(o => o.NFe.Emitente.CPF_CNPJ == cpfCnpjRemetente);

            return result.Select(o => o.NFe.Codigo);
        }

        public List<Dominio.Entidades.XMLNotaFiscalEletronica> BuscarNFesPorRemetente(int codigoEmpresa, double cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal)
        {
            var queryNFes = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();
            var query = _FiltroOccorrenciaNFe(codigoEmpresa, cpfCnpjRemetente, dataInicial, dataFinal);

            var result = from NFe in queryNFes where query.Distinct().Contains(NFe.Codigo) select NFe;

            return result
                         .Fetch(o => o.Emitente)
                         .Fetch(o => o.Empresa)
                         .ToList();
        }

        public List<double> BuscarRemetentesPorFiltro(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            var queryNFes = this.SessionNHiBernate.Query<Dominio.Entidades.XMLNotaFiscalEletronica>();
            var query = _FiltroOccorrenciaNFe(codigoEmpresa, 0, dataInicial, dataFinal);

            var result = from NFe in queryNFes where query.Distinct().Contains(NFe.Codigo) select NFe.Emitente.CPF_CNPJ;

            return result.ToList();
        }

        public Dominio.Entidades.OcorrenciaDeNFe BuscarUltimaOcorrenciaPorNFe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();

            var result = from o in query
                         where
                            o.NFe.Codigo == codigo
                         orderby o.DataDaOcorrencia descending
                         select o;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.OcorrenciaDeNFe> BuscarPorEmpresaTomadorData(int codigoEmpresa, double cnpjTomador, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();

            var result = from obj in query where obj.NFe.Empresa.Codigo == codigoEmpresa select obj;

            if (cnpjTomador > 0)
                result = result.Where(o => o.NFe.Emitente.CPF_CNPJ == cnpjTomador);

            if (dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
                result = result.Where(o => o.DataDeCadastro >= dataInicio && o.DataDeCadastro <= dataFim);

            return result.ToList();
        }

        public List<Dominio.Entidades.OcorrenciaDeNFe> BuscarPorEmpresaContratanteData(int codigoEmpresa, double cnpjContratante, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeNFe>();

            var result = from obj in query where obj.NFe.Empresa.Codigo == codigoEmpresa select obj;

            if (cnpjContratante > 0)
                result = result.Where(o => o.NFe.Contratante.CPF_CNPJ == cnpjContratante);

            if (dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
                result = result.Where(o => o.DataDeCadastro >= dataInicio && o.DataDeCadastro <= dataFim);

            return result.ToList();
        }

    }
}
