using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class ParcelaDocumentoEntrada : RepositorioBase<Dominio.Entidades.ParcelaDocumentoEntrada>, Dominio.Interfaces.Repositorios.ParcelaDocumentoEntrada
    {
        public ParcelaDocumentoEntrada(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ParcelaDocumentoEntrada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaDocumentoEntrada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ParcelaDocumentoEntrada BuscarPorCodigo(int codigo, int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaDocumentoEntrada>();
            var result = from obj in query where obj.Codigo == codigo && obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ParcelaDocumentoEntrada> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaDocumentoEntrada>();
            var result = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ParcelaDocumentoEntrada> BuscarPorDocumentoEntrada(int[] codigosDocumentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaDocumentoEntrada>();

            var result = from obj in query where codigosDocumentos.Contains(obj.DocumentoEntrada.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ParcelaDocumentoEntrada> Consultar(int codigoEmpresa, int codigoDocumentoEntrada, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusDuplicata status, double cpfCnpjCliente, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaDocumentoEntrada>();

            var result = from obj in query where obj.DocumentoEntrada.Empresa.Codigo == codigoEmpresa && obj.Status == status && obj.DocumentoEntrada.Status == Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado select obj;

            if (codigoDocumentoEntrada > 0)
                result = result.Where(o => o.DocumentoEntrada.Codigo == codigoDocumentoEntrada);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento < dataFinal.AddDays(1).Date);

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.DocumentoEntrada.Fornecedor.CPF_CNPJ == cpfCnpjCliente);

            return result.OrderBy(o => o.DataVencimento)
                         .ThenBy(o => o.DocumentoEntrada.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoDocumentoEntrada, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.StatusDuplicata status, double cpfCnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaDocumentoEntrada>();

            var result = from obj in query where obj.DocumentoEntrada.Empresa.Codigo == codigoEmpresa && obj.Status == status && obj.DocumentoEntrada.Status == Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado select obj;

            if (codigoDocumentoEntrada > 0)
                result = result.Where(o => o.DocumentoEntrada.Codigo == codigoDocumentoEntrada);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVencimento < dataFinal.AddDays(1).Date);

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.DocumentoEntrada.Fornecedor.CPF_CNPJ == cpfCnpjCliente);

            return result.Count();
        }

        public List<Dominio.Entidades.ParcelaDocumentoEntrada> BuscarPorListaDeCodigos(int codigoEmpresa, List<int> listaCodigoDuplicatas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParcelaDocumentoEntrada>();

            var result = from obj in query where obj.DocumentoEntrada.Empresa.Codigo == codigoEmpresa && listaCodigoDuplicatas.Contains(obj.Codigo) select obj;

            return result.ToList();
        }
    }
}
