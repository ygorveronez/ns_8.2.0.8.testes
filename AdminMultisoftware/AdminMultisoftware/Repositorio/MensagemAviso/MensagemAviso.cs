using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminMultisoftware.Repositorio.MensagemAviso
{
    public class MensagemAviso : RepositorioBase<Dominio.Entidades.MensagemAviso.MensagemAviso>
    {
        public MensagemAviso() { }

        public MensagemAviso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.MensagemAviso.MensagemAviso> Consultar(DateTime dataInicial, DateTime dataFinal, string titulo, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso.MensagemAviso>();

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInicio >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFim < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(titulo))
                query = query.Where(o => o.Titulo.Contains(titulo));

            return query.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.MensagemAviso.MensagemAviso> BuscarParaExibicao(DateTime data, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso.MensagemAviso>();
            query = query.Where(o => o.Ativo && o.DataInicio.Date <= data.Date && o.DataFim.Date >= data.Date);

            if (tipoServicoMultisoftware != 0)
                query = query.Where(obj => obj.TipoServicoMultisoftware == tipoServicoMultisoftware || obj.TipoServicoMultisoftware == 0);

            return query.ToList();
        }

        public int ContarConsulta(DateTime dataInicial, DateTime dataFinal, string titulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso.MensagemAviso>();

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInicio >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFim < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(titulo))
                query = query.Where(o => o.Titulo.Contains(titulo));

            return query.Count();
        }
    }
}
