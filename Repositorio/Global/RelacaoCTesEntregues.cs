using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class RelacaoCTesEntregues : RepositorioBase<Dominio.Entidades.RelacaoCTesEntregues>
    {
        public RelacaoCTesEntregues(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RelacaoCTesEntregues BuscarPorCodigo(int codigo, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RelacaoCTesEntregues>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == empresa select obj;

            return result.FirstOrDefault();
        }

        public int UltimoNumero(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RelacaoCTesEntregues>();

            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;

            return result.Count() > 0 ? result.Max(o => o.Numero) : 0;
        }

        private IQueryable<Dominio.Entidades.RelacaoCTesEntregues> _Consultar(int empresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, double cliente, Dominio.Enumeradores.StatusRelacaoCTesEntregues? status, int numeroCTe, string descricao, string numeroControle)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RelacaoCTesEntregues>();

            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero > numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero < numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataBipagem.Date > dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataBipagem.Date <= dataFinal);

            if (cliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == cliente);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            if (numeroCTe > 0)
                result = result.Where(o => o.CTes.Any(c => c.CTe.Numero == numeroCTe));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numeroControle))
                result = result.Where(o => o.NumeroControle.Contains(numeroControle));

            return result;
        }

        public List<Dominio.Entidades.RelacaoCTesEntregues> Consultar(int empresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, double cliente, Dominio.Enumeradores.StatusRelacaoCTesEntregues? status, int numeroCTe, string descricao, string numeroControle, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(empresa, numeroInicial, numeroFinal, dataInicial, dataFinal, cliente, status, numeroCTe, descricao, numeroControle);

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int empresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, double cliente, Dominio.Enumeradores.StatusRelacaoCTesEntregues? status, int numeroCTe, string descricao, string numeroControle)
        {
            var result = _Consultar(empresa, numeroInicial, numeroFinal, dataInicial, dataFinal, cliente, status, numeroCTe, descricao, numeroControle);

            return result.Count();
        }


        /// <summary>
        /// Apenas um construtor para gerar relatórios
        /// Não apagar
        /// </summary>
        public List<Dominio.ObjetosDeValor.Relatorios.RelacaoCTesEntregues> InstanciaDataSource()
        {
            return new List<Dominio.ObjetosDeValor.Relatorios.RelacaoCTesEntregues>();
        }
    }
}
