using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
    public class BemManutencao : RepositorioBase<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao>
    {
        public BemManutencao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Patrimonio.BemManutencao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao> Consultar(int codigoEmpresa, int codigoBem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao>();

            var result = from obj in query select obj;

            if (codigoBem > 0)
                result = result.Where(obj => obj.Bem.Codigo == codigoBem);

            if ((int)statusBem > 0)
                result = result.Where(obj => obj.Status == statusBem);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao> Consultar(int codigoEmpresa, int codigoBem, int motivoDefeito, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao>();

            var result = from obj in query select obj;

            if (motivoDefeito > 0)
            {
                result = result.Where(obj => obj.Bem.Codigo == codigoBem);
            }
            if (codigoBem > 0)
                result = result.Where(obj => obj.Bem.Codigo == codigoBem);

            if ((int)statusBem > 0)
                result = result.Where(obj => obj.Status == statusBem);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao> Consultar(int codigoEmpresa, int codigoBem, int motivoDefeito, DateTime? dataEntrega, DateTime? dataRetorno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao>();

            var result = from obj in query select obj;

            if (dataEntrega != null && dataRetorno != null && dataEntrega != DateTime.MinValue && dataRetorno != DateTime.MinValue)
            {
                result = result
                    .Where(obj => obj.DataEntrega >= dataEntrega)
                    .Where(x => x.DataRetorno <= dataRetorno);
            }

            if (motivoDefeito > 0)
            {
                result = result.Where(obj => obj.MotivoDefeito.Codigo == motivoDefeito);
            }
            if (codigoBem > 0)
                result = result.Where(obj => obj.Bem.Codigo == codigoBem);

            if ((int)statusBem > 0)
                result = result.Where(obj => obj.Status == statusBem);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }


        public int ContarConsulta(int codigoEmpresa, int codigoBem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBem statusBem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemManutencao>();

            var result = from obj in query select obj;

            if (codigoBem > 0)
                result = result.Where(obj => obj.Bem.Codigo == codigoBem);

            if ((int)statusBem > 0)
                result = result.Where(obj => obj.Status == statusBem);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }
    }
}
