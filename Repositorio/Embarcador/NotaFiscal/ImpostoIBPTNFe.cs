using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ImpostoIBPTNFe : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>
    {
        public ImpostoIBPTNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarAliquotaNacionalNCM(int codigoEmpresaPai, int codigoEmpresa, string ncm)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>();
            var result = from obj in query where obj.NCM.Equals(ncm) select obj;
            if (codigoEmpresaPai > 0)
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai));
            else
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa));

            result = result.Where(obj => obj.VigenciaFim.Date >= DateTime.Now.Date);

            if (result.Count() > 0)
                return result.Select(obj => obj.NacionalFederal).FirstOrDefault();
            else
                return 0;
        }

        public decimal BuscarAliquotaEstadualNCM(int codigoEmpresaPai, int codigoEmpresa, string ncm)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>();
            var result = from obj in query where obj.NCM.Equals(ncm) select obj;
            if (codigoEmpresaPai > 0)
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai));
            else
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa));

            result = result.Where(obj => obj.VigenciaFim.Date >= DateTime.Now.Date);

            if (result.Count() > 0)
                return result.Select(obj => obj.Estadual).FirstOrDefault();
            else
                return 0;
        }

        public decimal BuscarAliquotaMunicipalNCM(int codigoEmpresaPai, int codigoEmpresa, string ncm)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>();
            var result = from obj in query where obj.NCM.Equals(ncm) select obj;
            if (codigoEmpresaPai > 0)
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai));
            else
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa));

            result = result.Where(obj => obj.VigenciaFim.Date >= DateTime.Now.Date);

            if (result.Count() > 0)
                return result.Select(obj => obj.Municipal).FirstOrDefault();
            else
                return 0;
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe BuscarPorEmpresaNCM(int codigoEmpresaPai, int codigoEmpresa, string ncm, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>();
            var result = from obj in query where obj.NCM.Equals(ncm) select obj;
            if (codigoEmpresaPai > 0)
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai));
            else
                result = result.Where(obj => (obj.Empresa.Codigo == codigoEmpresa));

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.VigenciaInicio == dataInicial && obj.VigenciaFim == dataFinal);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe> Consultar(int codigoEmpresaPai, string descricao, string ncm, int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consulta(codigoEmpresaPai, descricao, ncm, codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresaPai, string descricao, string ncm, int codigoEmpresa)
        {
            var result = Consulta(codigoEmpresaPai, descricao, ncm, codigoEmpresa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe> Consulta(int codigoEmpresaPai, string descricao, string ncm, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(ncm))
                result = result.Where(obj => obj.NCM.StartsWith(ncm));

            if (codigoEmpresa > 0 && codigoEmpresaPai > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai);
            else
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result;
        }

        #endregion
    }
}
