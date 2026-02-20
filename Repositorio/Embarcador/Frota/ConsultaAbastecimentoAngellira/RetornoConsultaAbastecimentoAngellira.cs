using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira
{
    public class RetornoConsultaAbastecimentoAngellira : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira>
    {
        public RetornoConsultaAbastecimentoAngellira(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira> BuscarRetornosPendentesProcessamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira>();

            query = query.Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira BuscarRetorno(string placa, string cordenada, DateTime dataHora, int odometro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira>();

            query = query.Where(o => o.Placa == placa && o.Cordenada == cordenada && o.DataHora == dataHora && o.Odometro == odometro);

            return query.FirstOrDefault();
        }

        public bool ContemAbastecimentoEmComissao(int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngelLira>();
            var result = from obj in query where obj.Abastecimento.Codigo == codigoAbastecimento select obj;
            return result.Any();
        }
    }
}
