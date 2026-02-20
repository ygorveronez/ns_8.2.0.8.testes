using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Creditos
{
    public class SolicitacaoCredito : RepositorioBase<Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito>
    {
        public SolicitacaoCredito(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito> Consultar(int codigo, int codSolicitado, int codRecebedor, string codigoCargaEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito situacao, int codTipoOcorrencia, int numeroOcorrencia, int codSolicitante, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito>();

            var result = from obj in query select obj;

            if (codigo > 0)
            {
                result = result.Where(obj => obj.Codigo == codigo);
            }
            else
            {
                if (codSolicitado > 0)
                    result = result.Where(obj => (obj.Solicitado.Codigo == codSolicitado || obj.Solicitado.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro || obj.Solicitado.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao));

                if (codRecebedor > 0)
                    result = result.Where(obj => (obj.Creditor.Codigo == codRecebedor));

                if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Todos)
                    result = result.Where(obj => obj.SituacaoSolicitacaoCredito == situacao);

                if (!string.IsNullOrWhiteSpace(codigoCargaEmbarcador))
                    result = result.Where(obj => obj.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador);

                if (codTipoOcorrencia > 0)
                {
                    var queryTipoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia >();
                    result = result.Where(o => (from obj in queryTipoOcorrencia where obj.TipoOcorrencia.Codigo == codTipoOcorrencia select obj.SolicitacaoCredito.Codigo).Contains(o.Codigo));
                }

                if (codSolicitante > 0)
                    result = result.Where(obj => obj.Solicitante.Codigo == codSolicitante);

                if (numeroOcorrencia > 0)
                {
                    var queryTipoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
                    result = result.Where(o => (from obj in queryTipoOcorrencia where obj.NumeroOcorrencia == numeroOcorrencia select obj.SolicitacaoCredito.Codigo).Contains(o.Codigo));
                }
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigo, int codSolicitado, int codRecebedor, string codigoCargaEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito situacao, int codTipoOcorrencia, int numeroOcorrencia, int codSolicitante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito>();

            var result = from obj in query select obj;

            if (codigo > 0)
            {
                result = result.Where(obj => obj.Codigo == codigo);
            }
            else
            {
                if (codSolicitado > 0)
                    result = result.Where(obj => (obj.Solicitado.Codigo == codSolicitado || obj.Solicitado.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro || obj.Solicitado.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao));

                if (codRecebedor > 0)
                    result = result.Where(obj => (obj.Creditor.Codigo == codRecebedor));

                if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Todos)
                    result = result.Where(obj => obj.SituacaoSolicitacaoCredito == situacao);

                if (!string.IsNullOrWhiteSpace(codigoCargaEmbarcador))
                    result = result.Where(obj => obj.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador);

                if (codTipoOcorrencia > 0)
                {
                    var queryTipoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
                    result = result.Where(o => (from obj in queryTipoOcorrencia where obj.TipoOcorrencia.Codigo == codTipoOcorrencia select obj.SolicitacaoCredito.Codigo).Contains(o.Codigo));
                }

                if (codSolicitante > 0)
                    result = result.Where(obj => obj.Solicitante.Codigo == codSolicitante);

                if (numeroOcorrencia > 0)
                {
                    var queryTipoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
                    result = result.Where(o => (from obj in queryTipoOcorrencia where obj.NumeroOcorrencia == numeroOcorrencia select obj.SolicitacaoCredito.Codigo).Contains(o.Codigo));
                }
            }
            return result.Count();
        }

    }
}
