using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class MensagemAviso : RepositorioBase<Dominio.Entidades.MensagemAviso>, Dominio.Interfaces.Repositorios.MensagemAviso
    {
        public MensagemAviso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.MensagemAviso> Consultar(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string titulo, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicio >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFim < dataFinal.AddDays(1).Date);

            if (string.IsNullOrWhiteSpace(titulo))
                result = result.Where(o => o.Titulo.Contains(titulo));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string titulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicio >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFim < dataFinal.AddDays(1).Date);

            if (string.IsNullOrWhiteSpace(titulo))
                result = result.Where(o => o.Titulo.Contains(titulo));

            return result.Count();
        }

        public Dominio.Entidades.MensagemAviso BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MensagemAviso BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.MensagemAviso> BuscarParaExibicao(int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresaPai && obj.DataInicio <= DateTime.Now.Date && obj.DataFim >= DateTime.Now.Date && obj.Status == "A" && obj.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe select obj;           

            return result.ToList();
        }

        public List<Dominio.Entidades.MensagemAviso> Consultar(DateTime dataInicial, DateTime dataFinal, string titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenar, string dirOrdenar, int inicioRegistros, int maximoRegistros, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInicio >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFim < dataFinal.AddDays(1).Date);

            if (string.IsNullOrWhiteSpace(titulo))
                query = query.Where(o => o.Titulo.Contains(titulo));

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.OrderBy(propOrdenar + " " + dirOrdenar).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(DateTime dataInicial, DateTime dataFinal, string titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInicio >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFim < dataFinal.AddDays(1).Date);

            if (string.IsNullOrWhiteSpace(titulo))
                query = query.Where(o => o.Titulo.Contains(titulo));

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.Count();
        }

        public List<Dominio.Entidades.MensagemAviso> BuscarParaExibicao(DateTime data, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso>();

            query = query.Where(obj => obj.DataInicio <= data.Date && obj.DataFim >= data.Date && obj.Ativo && obj.TipoServicoMultisoftware == tipoServicoMultisoftware);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.ToList();
        }
    }
}
