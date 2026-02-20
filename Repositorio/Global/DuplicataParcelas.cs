using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class DuplicataParcelas : RepositorioBase<Dominio.Entidades.DuplicataParcelas>, Dominio.Interfaces.Repositorios.DuplicataParcelas
    {
        public DuplicataParcelas(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DuplicataParcelas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DuplicataParcelas BuscarPorParcela(int numero, int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Parcela == numero && obj.Duplicata.Codigo == codigoDuplicata select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DuplicataParcelas> BuscarPorDuplicata(int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Duplicata.Codigo == codigoDuplicata orderby obj.Parcela select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.DuplicataParcelas> ConsultarParcelasPendentes(int codigoEmpresa, Dominio.Enumeradores.TipoDuplicata tipo, DateTime dataInicial, DateTime dataFinal, double cpfCnpjCliente, string documento, int numeroCTe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Status == 0 && obj.Duplicata.Empresa.Codigo == codigoEmpresa select obj;

            result = result.Where(o => o.Duplicata.Tipo == tipo);

            result = result.Where(o => o.Duplicata.Status == "A");

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataVcto >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVcto < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Duplicata.Documento == documento);

            if (numeroCTe > 0)
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDocumentos where obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTe select obj.Duplicata.Codigo).Contains(o.Duplicata.Codigo));
            }

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.Duplicata.Pessoa.CPF_CNPJ == cpfCnpjCliente);

            return result.OrderBy(o => o.Duplicata.Numero)
                         .ThenBy(o => o.Parcela)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public int ContarParcelasPendentes(int codigoEmpresa, Dominio.Enumeradores.TipoDuplicata tipo, DateTime dataInicial, DateTime dataFinal, double cpfCnpjCliente, string documento, int numeroCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Status == 0 && obj.Duplicata.Empresa.Codigo == codigoEmpresa select obj;

            result = result.Where(o => o.Duplicata.Tipo == tipo);

            result = result.Where(o => o.Duplicata.Status == "A");

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataVcto >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVcto < dataFinal.AddDays(1).Date);

            if (cpfCnpjCliente > 0)
                result = result.Where(o => o.Duplicata.Pessoa.CPF_CNPJ == cpfCnpjCliente);

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Duplicata.Documento == documento);

            if (numeroCTe > 0)
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataCtes>();

                result = result.Where(o => (from obj in queryDocumentos where obj.ConhecimentoDeTransporteEletronico.Numero == numeroCTe select obj.Duplicata.Codigo).Contains(o.Duplicata.Codigo));
            }

            return result.Count();
        }

        public List<Dominio.Entidades.DuplicataParcelas> BuscarPorListaDeCodigos(int codigoEmpresa, List<int> listaCodigoParcelas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Duplicata.Empresa.Codigo == codigoEmpresa && listaCodigoParcelas.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.DuplicataParcelas> BuscarParcelasPendentes(int codigoEmpresa,  DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DuplicataParcelas>();

            var result = from obj in query where obj.Status == Dominio.Enumeradores.StatusDuplicata.Pendente && obj.Duplicata.Empresa.Codigo == codigoEmpresa select obj;

            result = result.Where(o => o.Duplicata.Status == "A");

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataVcto < dataFinal.AddDays(1).Date);

            return result.OrderBy(o => o.DataVcto)
                         .ToList();
        }
    }
}
