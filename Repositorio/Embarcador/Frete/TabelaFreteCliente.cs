using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using LinqKit;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteCliente : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>
    {
        #region Atributos

        UnitOfWork unitOfWork;

        #endregion

        #region Construtores

        public TabelaFreteCliente(UnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public int QuantidadeDeTabelasSemOperacao(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, DateTime dataVigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query
                         where
                            obj.TabelaFrete.Ativo == true
                            && obj.Ativo == true
                            && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo
                            && (
                                obj.Vigencia.DataInicial <= dataVigencia.Date
                                && (
                                    obj.Vigencia.DataFinal >= dataVigencia.Date
                                    || !obj.Vigencia.DataFinal.HasValue
                                )
                            )
                         select obj;
            if (cliente != null && cliente.GrupoPessoas != null)
            {
                result = result.Where(obj => obj.ClienteOrigem.CPF_CNPJ == cliente.CPF_CNPJ || obj.TabelaFrete.GrupoPessoas.Codigo == cliente.GrupoPessoas.Codigo);
            }
            else
            {
                if (cliente != null)
                {
                    result = result.Where(obj => obj.ClienteOrigem.CPF_CNPJ == cliente.CPF_CNPJ);
                }
                else
                {
                    result = result.Where(obj => obj.TabelaFrete.GrupoPessoas.Codigo == grupoPessoas.Codigo);
                }

            }

            //result = result.Where(obj => obj.TipoOperacao == null);
            result = result.Where(o => !o.TiposOperacao.Any());

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarTabelasParaAlerta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
            var result = from obj in query where obj.Vigencia != null && obj.Ativo select obj;

            var queryAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Alerta>();
            var resultAlerta = from obj in queryAlerta where obj.TelaAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela.TabelaFrete select obj;

            result = result.Where(o => !resultAlerta.Any(c => c.CodigoEntidade == o.Codigo));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarNaoVinculados(int quantidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(obj => obj.VinculadoSemParar.Value == false || obj.VinculadoSemParar.HasValue == false);

            query = query.Where(obj => obj.Ativo == true);
            query = query.Where(o => o.Vigencia.DataInicial <= DateTime.Now.Date && (!o.Vigencia.DataFinal.HasValue || o.Vigencia.DataFinal.Value >= DateTime.Now.Date));

            return query
                .Take(quantidade)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarRotasAtivasVigentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            query = query.Where(obj => obj.Ativo == true);
            query = query.Where(o => o.Vigencia.DataInicial <= DateTime.Now.Date && (!o.Vigencia.DataFinal.HasValue || o.Vigencia.DataFinal.Value >= DateTime.Now.Date));

            return query
                .ToList();
        }

        public bool ExistemRotasAtivasVigentesSemVinculoSemParar()
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            query = query.Where(obj => obj.Ativo &&
                                       (!obj.VinculadoSemParar.HasValue || !obj.VinculadoSemParar.Value) &&
                                       obj.Vigencia.DataInicial <= DateTime.Now.Date &&
                                       (!obj.Vigencia.DataFinal.HasValue || obj.Vigencia.DataFinal.Value >= DateTime.Now.Date));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiParametroTipoCargaComValorPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(obj => obj.Ativo
                    && obj.Tipo == TipoTabelaFreteCliente.Calculo
                    && obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (query.Count() == 0)
                return false;

            var itens = query.Where(obj => obj.ParametrosBaseCalculo.Count > 0)
                .SelectMany(obj => obj.ItensBaseCalculo);

            if (itens.Count() == 0)
                return false;

            var result = itens.Where(obj => obj.TipoObjeto == TipoParametroBaseTabelaFrete.TipoCarga
                    && obj.Valor > 0);

            return result.Any();
        }

        public int QuantidadeDeTabelasComOperacao(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, DateTime dataVigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query
                         where
                            obj.Ativo == true
                            && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo
                            && (
                                obj.Vigencia.DataInicial <= dataVigencia.Date
                                && (
                                    obj.Vigencia.DataFinal >= dataVigencia.Date
                                    || !obj.Vigencia.DataFinal.HasValue
                                )
                            )
                         select obj;
            if (cliente != null && cliente.GrupoPessoas != null)
            {
                result = result.Where(obj => obj.ClienteOrigem.CPF_CNPJ == cliente.CPF_CNPJ || obj.TabelaFrete.GrupoPessoas.Codigo == cliente.GrupoPessoas.Codigo);
            }
            else
            {
                if (cliente != null)
                {
                    result = result.Where(obj => obj.ClienteOrigem.CPF_CNPJ == cliente.CPF_CNPJ);
                }
                else
                {
                    result = result.Where(obj => obj.TabelaFrete.GrupoPessoas.Codigo == grupoPessoas.Codigo);
                }
            }

            //result = result.Where(obj => obj.TipoOperacao != null || obj.TabelaFrete.TiposOperacao.Any());
            result = result.Where(obj => obj.TiposOperacao.Any() || obj.TabelaFrete.TiposOperacao.Any());

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarPorFrete(List<Dominio.Entidades.Localidade> origens, List<Dominio.Entidades.Localidade> destinos, DateTime dataVigencia, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query
                         where
                            (
                                obj.Origens.Count == origens.Count
                                || obj.Origens.Count == 0
                            )
                            && (
                                obj.Destinos.Count == destinos.Count
                                || obj.FreteValidoParaQualquerDestino
                                || (
                                    obj.Destinos.Count == 0
                                    && (
                                        obj.EstadoDestino != null
                                        || obj.RegiaoDestino != null
                                    )
                                )
                                || (
                                    obj.Origens.Count == 0
                                    && obj.Destinos.Count == 0
                                    && obj.EstadoDestino == null
                                    && obj.RegiaoDestino == null
                                )
                            )
                            && (
                                obj.Vigencia.DataInicial <= dataVigencia.Date
                                && (
                                    obj.Vigencia.DataFinal >= dataVigencia.Date
                                    || !obj.Vigencia.DataFinal.HasValue
                                )
                            )
                            && obj.Ativo == true
                            && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo
                         select obj;

            result = result.Where(o => o.TabelaFrete.Codigo == tabelaFrete.Codigo);

            foreach (Dominio.Entidades.Localidade origem in origens)
                result = result.Where(o => o.Origens.Contains(origem) || o.Origens.Count == 0);

            foreach (Dominio.Entidades.Localidade destino in destinos)
                if (destino.Regiao == null)
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null)) || o.EstadoDestino.Sigla == destino.Estado.Sigla);
                else
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null && o.RegiaoDestino == null)) || o.RegiaoDestino == destino.Regiao || o.EstadoDestino.Sigla == destino.Estado.Sigla);

            if (empresa != null)
                result = result.Where(obj => obj.Empresa.Codigo == empresa.Codigo || obj.Empresa == null);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarPorFreteDestinatario(List<Dominio.Entidades.Localidade> origens, List<Dominio.Entidades.Localidade> destinos, DateTime dataVigencia, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query
                         where (obj.Origens.Count == origens.Count || obj.Origens.Count == 0) &&
                               (obj.Destinos.Count == destinos.Count || obj.FreteValidoParaQualquerDestino || (obj.Destinos.Count == 0 && (obj.EstadoDestino != null || obj.RegiaoDestino != null)) || (obj.Origens.Count == 0 && obj.Destinos.Count == 0 && obj.EstadoDestino == null && obj.RegiaoDestino == null)) &&
                               (obj.Vigencia.DataInicial <= dataVigencia.Date && (obj.Vigencia.DataFinal >= dataVigencia.Date || !obj.Vigencia.DataFinal.HasValue)) &&
                               obj.Ativo == true &&
                               obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo
                         select obj;

            foreach (Dominio.Entidades.Localidade origem in origens)
                result = result.Where(o => o.Origens.Contains(origem) || o.Origens.Count == 0);

            foreach (Dominio.Entidades.Localidade destino in destinos)
                if (destino.Regiao == null)
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null)) || o.EstadoDestino.Sigla == destino.Estado.Sigla);
                else
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null && o.RegiaoDestino == null)) || o.RegiaoDestino == destino.Regiao || o.EstadoDestino.Sigla == destino.Estado.Sigla);

            result = result.Where(o => o.TabelaFrete.Codigo == tabelaFrete.Codigo);

            result = result.Where(o => o.ClienteDestino.CPF_CNPJ == destinatario.CPF_CNPJ);

            if (empresa != null)
                result = result.Where(obj => obj.Empresa.Codigo == empresa.Codigo || obj.Empresa == null);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarPorFreteRemetente(List<Dominio.Entidades.Localidade> origens, List<Dominio.Entidades.Localidade> destinos, DateTime dataVigencia, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query
                         where (obj.Origens.Count == origens.Count || obj.Origens.Count == 0) &&
                               (obj.Destinos.Count == destinos.Count || obj.FreteValidoParaQualquerDestino || (obj.Destinos.Count == 0 && (obj.EstadoDestino != null || obj.RegiaoDestino != null)) || (obj.Origens.Count == 0 && obj.Destinos.Count == 0 && obj.EstadoDestino == null && obj.RegiaoDestino == null)) &&
                               (obj.Vigencia.DataInicial <= dataVigencia.Date && (obj.Vigencia.DataFinal >= dataVigencia.Date || !obj.Vigencia.DataFinal.HasValue)) &&
                               obj.Ativo == true &&
                               obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo
                         select obj;

            result = result.Where(o => o.TabelaFrete.Codigo == tabelaFrete.Codigo);

            foreach (Dominio.Entidades.Localidade origem in origens)
                result = result.Where(o => o.Origens.Contains(origem) || o.Origens.Count == 0);

            foreach (Dominio.Entidades.Localidade destino in destinos)
                if (destino.Regiao == null)
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null)) || o.EstadoDestino.Sigla == destino.Estado.Sigla);
                else
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null && o.RegiaoDestino == null)) || o.RegiaoDestino == destino.Regiao || o.EstadoDestino.Sigla == destino.Estado.Sigla);

            if (empresa != null)
                result = result.Where(obj => obj.Empresa.Codigo == empresa.Codigo || obj.Empresa == null);

            result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == remetente.CPF_CNPJ);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarPorFrete(List<Dominio.Entidades.Localidade> origens, List<Dominio.Entidades.Localidade> destinos, DateTime dataVigencia, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query
                         where (obj.Origens.Count == origens.Count || obj.Origens.Count == 0) &&
                               (obj.Destinos.Count == destinos.Count || obj.FreteValidoParaQualquerDestino || (obj.Destinos.Count == 0 && (obj.EstadoDestino != null || obj.RegiaoDestino != null)) || (obj.Origens.Count == 0 && obj.Destinos.Count == 0 && obj.EstadoDestino == null && obj.RegiaoDestino == null)) &&
                               (obj.Vigencia.DataInicial <= dataVigencia.Date && (obj.Vigencia.DataFinal >= dataVigencia.Date || !obj.Vigencia.DataFinal.HasValue)) &&
                               obj.Ativo == true &&
                               obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo
                         select obj;

            foreach (Dominio.Entidades.Localidade origem in origens)
                result = result.Where(o => o.Origens.Contains(origem) || o.Origens.Count == 0);

            foreach (Dominio.Entidades.Localidade destino in destinos)
                if (destino.Regiao == null)
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null)) || o.EstadoDestino.Sigla == destino.Estado.Sigla);
                else
                    result = result.Where(o => (o.Destinos.Contains(destino) || (o.Destinos.Count == 0 && o.EstadoDestino == null && o.RegiaoDestino == null)) || o.RegiaoDestino == destino.Regiao || o.EstadoDestino.Sigla == destino.Estado.Sigla);

            result = result.Where(o => o.TabelaFrete.Codigo == tabelaFrete.Codigo);

            result = result.Where(o => o.ClienteOrigem.CPF_CNPJ == remetente.CPF_CNPJ);

            result = result.Where(o => o.ClienteDestino.CPF_CNPJ == destinatario.CPF_CNPJ);

            if (empresa != null)
                result = result.Where(obj => obj.Empresa.Codigo == empresa.Codigo || obj.Empresa == null);
            //result = result.Where(obj => (obj.Empresa.Codigo == empresa.Codigo || (empresa.Matriz.Any(mt => mt.Codigo == obj.Codigo))) || obj.Empresa == null);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente BuscarTabelaComMesmaIncidencia(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabela, Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete vigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            query = query.Where(obj => obj.Origens.Count() == tabela.Origens.Count() &&
                                obj.Destinos.Count() == tabela.Destinos.Count() &&
                                obj.Vigencia == vigencia &&
                                obj.EstadoDestino == tabela.EstadoDestino &&
                                obj.TipoOperacao == tabela.TipoOperacao &&
                                obj.ClienteOrigem == tabela.ClienteOrigem &&
                                obj.ClienteDestino == tabela.ClienteDestino &&
                                obj.RegiaoDestino == tabela.RegiaoDestino &&
                                obj.TabelaFrete == tabela.TabelaFrete &&
                                obj.RotaFrete == tabela.RotaFrete &&
                                obj.Empresa == tabela.Empresa &&
                                obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo);


            foreach (Dominio.Entidades.Localidade origem in tabela.Origens)
                query = query.Where(o => o.Origens.Contains(origem));

            foreach (Dominio.Entidades.Localidade destino in tabela.Destinos)
                query = query.Where(o => o.Destinos.Contains(destino));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente BuscarTabelaComMesmaIncidencia(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigem, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            query = query.Where(o => o.Vigencia.Codigo == tabelaFreteCliente.Vigencia.Codigo &&
                                     o.TabelaFrete.Codigo == tabelaFreteCliente.TabelaFrete.Codigo &&
                                     o.Ativo &&
                                     o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo);

            if (tabelaFreteCliente.CanalEntrega != null)
                query = query.Where(obj => obj.CanalEntrega.Codigo == tabelaFreteCliente.CanalEntrega.Codigo);
            else
                query = query.Where(obj => obj.CanalEntrega == null);

            if (tabelaFreteCliente.CanalVenda != null)
                query = query.Where(obj => obj.CanalVenda.Codigo == tabelaFreteCliente.CanalVenda.Codigo);
            else
                query = query.Where(obj => obj.CanalVenda == null);

            if (tabelaFreteCliente.Codigo > 0)
                query = query.Where(o => o.Codigo != tabelaFreteCliente.Codigo);

            if (tabelaFreteCliente.Empresa != null)
                query = query.Where(o => o.Empresa.Codigo == tabelaFreteCliente.Empresa.Codigo);
            else
                query = query.Where(o => o.Empresa == null);

            //if (tabelaFreteCliente.TipoOperacao != null)
            //    query = query.Where(o => o.TipoOperacao.Codigo == tabelaFreteCliente.TipoOperacao.Codigo);
            //else
            //    query = query.Where(o => o.TipoOperacao == null);

            if (tabelaFreteCliente.TiposOperacao != null && tabelaFreteCliente.TiposOperacao.Count > 0)
            {
                var tiposOperacoes = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tabelaFreteCliente.TiposOperacao)
                    tiposOperacoes = tiposOperacoes.And(o => o.TiposOperacao.Any(l => l.Codigo == tipoOperacao.Codigo));

                query = query.Where(tiposOperacoes);
            }
            else
                query = query.Where(o => o.TiposOperacao.Count == 0);

            if (tabelaFreteCliente.Fronteiras != null && tabelaFreteCliente.Fronteiras.Count > 0)
            {
                var fronteiras = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                foreach (Dominio.Entidades.Cliente fronteira in tabelaFreteCliente.Fronteiras)
                    fronteiras = fronteiras.And(o => o.Fronteiras.Any(l => l.CPF_CNPJ == fronteira.CPF_CNPJ));

                query = query.Where(fronteiras);
            }
            else
                query = query.Where(o => o.Fronteiras.Count == 0);

            if (tabelaFreteCliente.TiposCarga != null && tabelaFreteCliente.TiposCarga.Count > 0)
            {
                var tiposCargas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tabelaFreteCliente.TiposCarga)
                    tiposCargas = tiposCargas.And(o => o.TiposCarga.Any(l => l.Codigo == tipoCarga.Codigo));

                query = query.Where(tiposCargas);
            }
            else
                query = query.Where(o => o.TiposCarga.Count == 0);

            if (tabelaFreteCliente.TransportadoresTerceiros != null && tabelaFreteCliente.TransportadoresTerceiros.Count > 0)
            {
                var transportadoresTerceiros = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                foreach (Dominio.Entidades.Cliente transportadorTerceiro in tabelaFreteCliente.TransportadoresTerceiros)
                    transportadoresTerceiros = transportadoresTerceiros.And(o => o.TransportadoresTerceiros.Any(l => l.CPF_CNPJ == transportadorTerceiro.CPF_CNPJ));

                query = query.Where(transportadoresTerceiros);
            }
            else
                query = query.Where(o => o.TransportadoresTerceiros.Count == 0);

            if (tabelaFreteCliente.Origens != null && tabelaFreteCliente.Origens.Count > 0)
            {
                List<int> codigosOrigens = tabelaFreteCliente.Origens.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerOrigem)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerOrigem || o.Origens.Count == 1) && o.Origens.Any(l => codigosOrigens.Contains(l.Codigo)));
                }
                else
                {
                    var origensExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensExatas = origensExatas.And(o => !o.FreteValidoParaQualquerOrigem);
                    origensExatas = origensExatas.And(LocalidadesOrigemExatas(codigosOrigens));

                    var origensVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensVariaveis = origensVariaveis.And(o => o.FreteValidoParaQualquerOrigem);
                    origensVariaveis = origensVariaveis.And(LocalidadesOrigemVariaveis(codigosOrigens));

                    query = query.Where(origensExatas.Or(origensVariaveis));
                }
            }
            else
                query = query.Where(o => o.Origens.Count == 0);


            if (tabelaFreteCliente.ClientesOrigem != null && tabelaFreteCliente.ClientesOrigem.Count > 0)
            {
                List<double> cpfCnpjClientes = tabelaFreteCliente.ClientesOrigem.Select(o => o.CPF_CNPJ).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerOrigem)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerOrigem || o.ClientesOrigem.Count == 1) && o.ClientesOrigem.Any(l => cpfCnpjClientes.Contains(l.CPF_CNPJ)));
                }
                else
                {
                    var origensExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensExatas = origensExatas.And(o => !o.FreteValidoParaQualquerOrigem);
                    origensExatas = origensExatas.And(ClientesOrigemExatos(cpfCnpjClientes));

                    var origensVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensVariaveis = origensVariaveis.And(o => o.FreteValidoParaQualquerOrigem);
                    origensVariaveis = origensVariaveis.And(ClientesOrigemVariaveis(cpfCnpjClientes));

                    query = query.Where(origensExatas.Or(origensVariaveis));
                }
            }
            else
                query = query.Where(o => o.ClientesOrigem.Count == 0);

            if (tabelaFreteCliente.EstadosOrigem != null && tabelaFreteCliente.EstadosOrigem.Count > 0)
            {
                List<string> estados = tabelaFreteCliente.EstadosOrigem.Select(o => o.Sigla).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerOrigem)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerOrigem || o.EstadosOrigem.Count == 1) && o.EstadosOrigem.Any(l => estados.Contains(l.Sigla)));
                }
                else
                {
                    var origensExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensExatas = origensExatas.And(o => !o.FreteValidoParaQualquerOrigem);
                    origensExatas = origensExatas.And(EstadosOrigemExatos(estados));

                    var origensVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensVariaveis = origensVariaveis.And(o => o.FreteValidoParaQualquerOrigem);
                    origensVariaveis = origensVariaveis.And(EstadosOrigemVariaveis(estados));

                    query = query.Where(origensExatas.Or(origensVariaveis));
                }
            }
            else
                query = query.Where(o => o.EstadosOrigem.Count == 0);


            if (tabelaFreteCliente.RotasOrigem != null && tabelaFreteCliente.RotasOrigem.Count > 0)
            {
                List<int> rotas = tabelaFreteCliente.RotasOrigem.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerOrigem)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerOrigem || o.RotasOrigem.Count == 1) && o.RotasOrigem.Any(l => rotas.Contains(l.Codigo)));
                }
                else
                {
                    var origensExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensExatas = origensExatas.And(o => !o.FreteValidoParaQualquerOrigem);
                    origensExatas = origensExatas.And(RotasOrigemExatas(rotas));

                    var origensVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensVariaveis = origensVariaveis.And(o => o.FreteValidoParaQualquerOrigem);
                    origensVariaveis = origensVariaveis.And(RotasOrigemVariaveis(rotas));

                    query = query.Where(origensExatas.Or(origensVariaveis));
                }
            }
            else
                query = query.Where(o => o.RotasOrigem.Count == 0);

            if (cepsOrigem != null && cepsOrigem.Count > 0)
            {
                var faixaCEPsOrigem = PredicateBuilder.False<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem faixaCEP in cepsOrigem)
                    faixaCEPsOrigem = faixaCEPsOrigem.Or(o => o.CEPsOrigem.Any(cepCadastrado => (cepCadastrado.CEPInicial >= faixaCEP.CEPInicial && cepCadastrado.CEPInicial <= faixaCEP.CEPFinal) ||
                                                                                                (cepCadastrado.CEPFinal >= faixaCEP.CEPInicial && cepCadastrado.CEPFinal <= faixaCEP.CEPFinal) ||
                                                                                                (faixaCEP.CEPInicial >= cepCadastrado.CEPInicial && faixaCEP.CEPInicial <= cepCadastrado.CEPFinal) ||
                                                                                                (faixaCEP.CEPFinal >= cepCadastrado.CEPInicial && faixaCEP.CEPFinal <= cepCadastrado.CEPFinal)));

                query = query.Where(faixaCEPsOrigem);
            }
            else
                query = query.Where(o => o.CEPsOrigem.Count == 0);

            if (tabelaFreteCliente.PaisesOrigem != null && tabelaFreteCliente.PaisesOrigem.Count > 0)
            {
                List<int> paises = tabelaFreteCliente.PaisesOrigem.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerOrigem)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerOrigem || o.PaisesOrigem.Count == 1) && o.PaisesOrigem.Any(l => paises.Contains(l.Codigo)));
                }
                else
                {
                    var origensExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensExatas = origensExatas.And(o => !o.FreteValidoParaQualquerOrigem);
                    origensExatas = origensExatas.And(PaisesOrigemExatas(paises));

                    var origensVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensVariaveis = origensVariaveis.And(o => o.FreteValidoParaQualquerOrigem);
                    origensVariaveis = origensVariaveis.And(PaisesOrigemVariaveis(paises));

                    query = query.Where(origensExatas.Or(origensVariaveis));
                }
            }
            else
                query = query.Where(o => o.PaisesOrigem.Count == 0);

            if (tabelaFreteCliente.RegioesOrigem != null && tabelaFreteCliente.RegioesOrigem.Count > 0)
            {
                List<int> regioes = tabelaFreteCliente.RegioesOrigem.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerOrigem)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerOrigem || o.RegioesOrigem.Count == 1) && o.RegioesOrigem.Any(l => regioes.Contains(l.Codigo)));
                }
                else
                {
                    var origensExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensExatas = origensExatas.And(o => !o.FreteValidoParaQualquerOrigem);
                    origensExatas = origensExatas.And(RegioesOrigemExatos(regioes));

                    var origensVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    origensVariaveis = origensVariaveis.And(o => o.FreteValidoParaQualquerOrigem);
                    origensVariaveis = origensVariaveis.And(RegioesOrigemVariaveis(regioes));

                    query = query.Where(origensExatas.Or(origensVariaveis));
                }
            }
            else
                query = query.Where(o => o.RegioesOrigem.Count == 0);


            if (tabelaFreteCliente.Destinos != null && tabelaFreteCliente.Destinos.Count > 0)
            {
                List<int> codigosDestinos = tabelaFreteCliente.Destinos.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerDestino)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerDestino || o.Destinos.Count == 1) && o.Destinos.Any(l => codigosDestinos.Contains(l.Codigo)));
                }
                else
                {
                    var destinosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosExatos = destinosExatos.And(o => !o.FreteValidoParaQualquerDestino);
                    destinosExatos = destinosExatos.And(LocalidadesDestinoExatas(codigosDestinos));

                    var destinosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosVariaveis = destinosVariaveis.And(o => o.FreteValidoParaQualquerDestino);
                    destinosVariaveis = destinosVariaveis.And(LocalidadesDestinoVariaveis(codigosDestinos));

                    query = query.Where(destinosExatos.Or(destinosVariaveis));
                }
            }
            else
                query = query.Where(o => o.Destinos.Count == 0);

            if (tabelaFreteCliente.ClientesDestino != null && tabelaFreteCliente.ClientesDestino.Count > 0)
            {
                List<double> cpfCnpjClientes = tabelaFreteCliente.ClientesDestino.Select(o => o.CPF_CNPJ).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerDestino)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerDestino || o.ClientesDestino.Count == 1) && o.ClientesDestino.Any(l => cpfCnpjClientes.Contains(l.CPF_CNPJ)));
                }
                else
                {
                    var destinosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosExatos = destinosExatos.And(o => !o.FreteValidoParaQualquerDestino);
                    destinosExatos = destinosExatos.And(ClientesDestinoExatos(cpfCnpjClientes));

                    var destinosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosVariaveis = destinosVariaveis.And(o => o.FreteValidoParaQualquerDestino);
                    destinosVariaveis = destinosVariaveis.And(ClientesDestinoVariaveis(cpfCnpjClientes));

                    query = query.Where(destinosExatos.Or(destinosVariaveis));
                }
            }
            else
                query = query.Where(o => o.ClientesDestino.Count == 0);

            if (tabelaFreteCliente.EstadosDestino != null && tabelaFreteCliente.EstadosDestino.Count > 0)
            {
                List<string> estados = tabelaFreteCliente.EstadosDestino.Select(o => o.Sigla).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerDestino)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerDestino || o.EstadosDestino.Count == 1) && o.EstadosDestino.Any(l => estados.Contains(l.Sigla)));
                }
                else
                {
                    var destinosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosExatos = destinosExatos.And(o => !o.FreteValidoParaQualquerDestino);
                    destinosExatos = destinosExatos.And(EstadosDestinoExatos(estados));

                    var destinosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosVariaveis = destinosVariaveis.And(o => o.FreteValidoParaQualquerDestino);
                    destinosVariaveis = destinosVariaveis.And(EstadosDestinoVariaveis(estados));

                    query = query.Where(destinosExatos.Or(destinosVariaveis));
                }
            }
            else
                query = query.Where(o => o.EstadosDestino.Count == 0);

            if (tabelaFreteCliente.RegioesDestino != null && tabelaFreteCliente.RegioesDestino.Count > 0)
            {
                List<int> regioes = tabelaFreteCliente.RegioesDestino.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerDestino)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerDestino || o.RegioesDestino.Count == 1) && o.RegioesDestino.Any(l => regioes.Contains(l.Codigo)));
                }
                else
                {
                    var destinosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosExatos = destinosExatos.And(o => !o.FreteValidoParaQualquerDestino);
                    destinosExatos = destinosExatos.And(RegioesDestinoExatos(regioes));

                    var destinosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosVariaveis = destinosVariaveis.And(o => o.FreteValidoParaQualquerDestino);
                    destinosVariaveis = destinosVariaveis.And(RegioesDestinoVariaveis(regioes));

                    query = query.Where(destinosExatos.Or(destinosVariaveis));
                }
            }
            else
                query = query.Where(o => o.RegioesDestino.Count == 0);

            if (tabelaFreteCliente.RotasDestino != null && tabelaFreteCliente.RotasDestino.Count > 0)
            {
                List<int> rotas = tabelaFreteCliente.RotasDestino.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerDestino)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerDestino || o.RotasDestino.Count == 1) && o.RotasDestino.Any(l => rotas.Contains(l.Codigo)));
                }
                else
                {
                    var destinosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosExatos = destinosExatos.And(o => !o.FreteValidoParaQualquerDestino);
                    destinosExatos = destinosExatos.And(RotasDestinoExatos(rotas));

                    var destinosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosVariaveis = destinosVariaveis.And(o => o.FreteValidoParaQualquerDestino);
                    destinosVariaveis = destinosVariaveis.And(RotasDestinoVariaveis(rotas));

                    query = query.Where(destinosExatos.Or(destinosVariaveis));
                }
            }
            else
                query = query.Where(o => o.RotasDestino.Count == 0);

            if (cepsOrigem != null && cepsOrigem.Count > 0)
            {
                var faixaCEPsDestino = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem faixaCEP in cepsOrigem)
                    faixaCEPsDestino = faixaCEPsDestino.Or(o => o.CEPsDestino.Any(cepCadastrado => (cepCadastrado.CEPInicial >= faixaCEP.CEPInicial && cepCadastrado.CEPInicial <= faixaCEP.CEPFinal) ||
                                                                                                   (cepCadastrado.CEPFinal >= faixaCEP.CEPInicial && cepCadastrado.CEPFinal <= faixaCEP.CEPFinal) ||
                                                                                                   (faixaCEP.CEPInicial >= cepCadastrado.CEPInicial && faixaCEP.CEPInicial <= cepCadastrado.CEPFinal) ||
                                                                                                   (faixaCEP.CEPFinal >= cepCadastrado.CEPInicial && faixaCEP.CEPFinal <= cepCadastrado.CEPFinal)));

                query = query.Where(faixaCEPsDestino);
            }
            else
                query = query.Where(o => o.CEPsDestino.Count == 0);

            if (tabelaFreteCliente.PaisesDestino != null && tabelaFreteCliente.PaisesDestino.Count > 0)
            {
                List<int> paises = tabelaFreteCliente.PaisesDestino.Select(o => o.Codigo).ToList();

                if (tabelaFreteCliente.FreteValidoParaQualquerDestino)
                {
                    query = query.Where(o => (o.FreteValidoParaQualquerDestino || o.PaisesDestino.Count == 1) && o.PaisesDestino.Any(l => paises.Contains(l.Codigo)));
                }
                else
                {
                    var destinosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosExatos = destinosExatos.And(o => !o.FreteValidoParaQualquerDestino);
                    destinosExatos = destinosExatos.And(PaisesDestinoExatos(paises));

                    var destinosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                    destinosVariaveis = destinosVariaveis.And(o => o.FreteValidoParaQualquerDestino);
                    destinosVariaveis = destinosVariaveis.And(PaisesDestinoVariaveis(paises));

                    query = query.Where(destinosExatos.Or(destinosVariaveis));
                }
            }
            else
                query = query.Where(o => o.PaisesDestino.Count == 0);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente BuscarPorAlteracaoVigenciaPendente(int codigoTabelaFreteCliente)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(tabelaFreteCliente => tabelaFreteCliente.TabelaOriginaria.Codigo == codigoTabelaFreteCliente && tabelaFreteCliente.Tipo == TipoTabelaFreteCliente.AlteracaoVigenciaPendente);

            return consultaTabelaFreteCliente.FirstOrDefault();
        }

        public IList<int> BuscarTabelaClientePorCEP(int codigoTabelaFrete, int codigoVigencia, int codigoTipoOperacao, int empresa, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> faixaCEPsOrigem, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> faixaCEPsDestino)
        {
            string hql = $"SELECT TC.Codigo FROM TabelaFreteCliente TC WHERE TC.TabelaFrete.Codigo = {codigoTabelaFrete} and (TC.Tipo=0 or TC.Tipo=2 ) and TC.Vigencia.Codigo = {codigoVigencia} "; // SQL-INJECTION-SAFE

            if (empresa > 0)
                hql += $" AND EMP_CODIGO = {empresa}";

            string sqlCEPOrigem = string.Empty;
            foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaOrigen in faixaCEPsOrigem)
            {
                if (string.IsNullOrWhiteSpace(sqlCEPOrigem)) 
                    sqlCEPOrigem = $" EXISTS (SELECT  CO.TabelaFreteCliente.Codigo FROM TabelaFreteClienteCEPOrigem CO WHERE CO.TabelaFreteCliente.Codigo = TC.Codigo AND CO.CEPInicial = {faixaOrigen.CEPInicial} AND CO.CEPFinal = {faixaOrigen.CEPFinal} ) "; // SQL-INJECTION-SAFE
                else
                    sqlCEPOrigem += $" OR EXISTS (SELECT  CO.TabelaFreteCliente.Codigo FROM TabelaFreteClienteCEPOrigem CO WHERE CO.TabelaFreteCliente.Codigo = TC.Codigo AND CO.CEPInicial = {faixaOrigen.CEPInicial} AND CO.CEPFinal = {faixaOrigen.CEPFinal} ) "; // SQL-INJECTION-SAFE
            }

            hql += $" AND ({sqlCEPOrigem})";


            string sqlCEPDestino = string.Empty;
            foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaDestino in faixaCEPsDestino)
            {
                if (string.IsNullOrWhiteSpace(sqlCEPDestino))
                    sqlCEPDestino = $" EXISTS (SELECT  CD.TabelaFreteCliente.Codigo FROM TabelaFreteClienteCEPDestino CD WHERE CD.TabelaFreteCliente.Codigo = TC.Codigo AND CD.CEPInicial = {faixaDestino.CEPInicial} AND CD.CEPFinal = {faixaDestino.CEPFinal} ) "; // SQL-INJECTION-SAFE
                else
                    sqlCEPDestino += $" OR EXISTS (SELECT  CD.TabelaFreteCliente.Codigo FROM TabelaFreteClienteCEPDestino CD WHERE CD.TabelaFreteCliente.Codigo = TC.Codigo AND CD.CEPInicial = {faixaDestino.CEPInicial} AND CD.CEPFinal = {faixaDestino.CEPFinal} ) "; // SQL-INJECTION-SAFE
            }

            hql += $" AND ({sqlCEPDestino})";

            var query = this.SessionNHiBernate.CreateQuery(hql);

            return query.List<int>();
        }

        public IList<int> BuscarTabelasQueNaoPossuemParametroModeloReboqueOuTracao(int codigoObjeto, int codigoTabelaFrete)
        {
            string hql = $"SELECT tabela.Codigo from TabelaFreteCliente tabela WHERE tabela.TabelaFrete.Codigo = {codigoTabelaFrete} AND NOT EXISTS (SELECT 1 from ParametroBaseCalculoTabelaFrete parametro WHERE parametro.CodigoObjeto = {codigoObjeto} AND parametro.TabelaFrete.Codigo = tabela.Codigo)"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateQuery(hql);

            return query.List<int>();
        }

        public IList<int> BuscarCodigosTabelasQueNaoPossuemComponente(int codigoComponente, int codigoTabelaFrete, int codigoObjeto)
        {
            string hql = "";

            if (codigoObjeto > 0)
                hql = $"SELECT tabela.Codigo as Codigo from TabelaFreteCliente tabela WHERE tabela.TabelaFrete.Codigo = {codigoTabelaFrete} AND NOT EXISTS (SELECT 1 from ItemParametroBaseCalculoTabelaFrete item WHERE item.CodigoObjeto = {codigoComponente} AND item.ParametroBaseCalculo.TabelaFrete.Codigo = tabela.Codigo AND item.ParametroBaseCalculo.CodigoObjeto = {codigoObjeto})"; // SQL-INJECTION-SAFE
            else
                hql = $"SELECT tabela.Codigo as Codigo from TabelaFreteCliente tabela WHERE tabela.TabelaFrete.Codigo = {codigoTabelaFrete} AND NOT EXISTS (SELECT 1 from ItemParametroBaseCalculoTabelaFrete item WHERE item.CodigoObjeto = {codigoComponente} AND item.TabelaFrete.Codigo = tabela.Codigo)"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateQuery(hql);

            return query.List<int>();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente BuscarTabelaIgualParaImportacao(int codigoTabelaFrete, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> origens, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade> destinos, List<Dominio.Entidades.Estado> estadosOrigem, List<Dominio.Entidades.Estado> estadosDestinos, List<Dominio.Entidades.Cliente> clientesOrigem, List<Dominio.Entidades.Cliente> clientesDestino, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> cepsOrigem, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP> cepsDestino, List<Dominio.Entidades.RotaFrete> rotasOrigem, List<Dominio.Entidades.RotaFrete> rotasDestino, List<Dominio.Entidades.Cliente> fronteiras, int codigoVigencia, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao/*int codigoTipoOperacao*/, int empresa, int codigoCanalEntrega, List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && (o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo || o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao));

            if (codigoVigencia > 0)
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(obj => obj.Vigencia.Codigo == codigoVigencia);

            if (empresa > 0)
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(obj => obj.Empresa.Codigo == empresa);

            //if (codigoTipoOperacao > 0)
            //    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.TiposOperacao.Any(t => t.Codigo == codigoTipoOperacao));
            if (tiposOperacao?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.TiposOperacao.Count == tiposOperacao.Count);

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.TiposOperacao.Any(t => t.Codigo == tipoOperacao.Codigo));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.TiposOperacao.Count() == 0);

            if (origens?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Origens.Count == origens.Count);

                foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade origem in origens)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Origens.Any(l => l.Codigo == origem.Codigo));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Origens.Count() == 0);

            if (estadosOrigem?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.EstadosOrigem.Count == estadosOrigem.Count);

                foreach (Dominio.Entidades.Estado estado in estadosOrigem)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.EstadosOrigem.Any(l => l.CodigoIBGE == estado.Codigo));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.EstadosOrigem.Count() == 0);

            if (clientesOrigem?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.ClientesOrigem.Count == clientesOrigem.Count);

                foreach (Dominio.Entidades.Cliente clienteOrigem in clientesOrigem)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.ClientesOrigem.Contains(clienteOrigem));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.ClientesOrigem.Count() == 0);

            if (cepsOrigem?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CEPsOrigem.Count == cepsOrigem.Count);

                foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaCEP in cepsOrigem)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CEPsOrigem.Any(c => c.CEPInicial == faixaCEP.CEPInicial && c.CEPFinal == faixaCEP.CEPFinal));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CEPsOrigem.Count() == 0);

            consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.RegioesOrigem.Count() == 0);

            if (destinos?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Destinos.Count == destinos.Count);

                foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.Localidade destino in destinos)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Destinos.Any(l => l.Codigo == destino.Codigo));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Destinos.Count() == 0);

            if (estadosDestinos?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.EstadosDestino.Count == estadosDestinos.Count);

                foreach (Dominio.Entidades.Estado estado in estadosDestinos)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.EstadosDestino.Any(l => l.CodigoIBGE == estado.Codigo));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.EstadosDestino.Count() == 0);

            if (clientesDestino != null && clientesDestino.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.ClientesDestino.Count == clientesDestino.Count);

                foreach (Dominio.Entidades.Cliente clienteDestino in clientesDestino)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.ClientesDestino.Contains(clienteDestino));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.ClientesDestino.Count() == 0);

            if (rotasOrigem != null && rotasOrigem.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.RotasOrigem.Count == rotasOrigem.Count);

                foreach (Dominio.Entidades.RotaFrete rotaOrigem in rotasOrigem)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.RotasOrigem.Contains(rotaOrigem));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.RotasOrigem.Count() == 0);

            if (rotasDestino != null && rotasDestino.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.RotasDestino.Count == rotasDestino.Count);

                foreach (Dominio.Entidades.RotaFrete rotaDestino in rotasDestino)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.RotasDestino.Contains(rotaDestino));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.RotasDestino.Count() == 0);

            if (cepsDestino != null && cepsDestino.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CEPsDestino.Count == cepsDestino.Count);

                foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.ImportacaoTabelaFrete.FaixaCEP faixaCEP in cepsDestino)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CEPsDestino.Any(c => c.CEPInicial == faixaCEP.CEPInicial && c.CEPFinal == faixaCEP.CEPFinal));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CEPsDestino.Count() == 0);

            consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(obj => obj.RegioesDestino.Count() == 0);

            if (fronteiras?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Fronteiras.Count == fronteiras.Count);

                foreach (Dominio.Entidades.Cliente fronteira in fronteiras)
                    consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.Fronteiras.Contains(fronteira));
            }
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(obj => obj.Fronteiras.Count() == 0);

            if (codigoCanalEntrega > 0)
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CanalEntrega.Codigo == codigoCanalEntrega);
            else
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.CanalEntrega == null);

            if (tiposDeCarga?.Count > 0)
            {
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(o => o.TiposCarga.Count == tiposDeCarga.Count);

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in tiposDeCarga)
                    consultaTabelaFreteCliente.Where(o => o.TiposCarga.Any(t => t.Codigo == tipoDeCarga.Codigo));
            }
            else
                consultaTabelaFreteCliente.Where(o => o.TiposCarga.Count() == 0);

            return consultaTabelaFreteCliente.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente BuscarTabelaIgual(int codigoTabelaFrete, List<Dominio.Entidades.Localidade> origens, List<Dominio.Entidades.Localidade> destinos, List<Dominio.Entidades.Estado> estadosOrigem, List<Dominio.Entidades.Estado> estadosDestino, List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesOrigem, List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioesDestino, List<Dominio.Entidades.Cliente> clientesOrigem, List<Dominio.Entidades.Cliente> clientesDestino, List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP> cepsOrigem, List<Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP> cepsDestino, List<Dominio.Entidades.RotaFrete> rotasOrigem, List<Dominio.Entidades.RotaFrete> rotasDestino, List<Dominio.Entidades.Cliente> fronteiras, int codigoVigencia, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao, List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga, int codigoEmpresa, int codigoCanalEntrega, int codigoCanalVenda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            query = query.Where(obj => obj.Ativo && obj.TabelaFrete.Codigo == codigoTabelaFrete && (obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao));

            if (rotasOrigem != null && rotasOrigem.Count > 0)
            {
                query = query.Where(o => o.RotasOrigem.Count == rotasOrigem.Count);

                foreach (Dominio.Entidades.RotaFrete rotaOrigem in rotasOrigem)
                    query = query.Where(o => o.RotasOrigem.Contains(rotaOrigem));
            }
            else
                query = query.Where(obj => obj.RotasOrigem.Count() == 0);

            if (origens != null && origens.Count > 0)
            {
                query = query.Where(o => o.Origens.Count == origens.Count);

                foreach (Dominio.Entidades.Localidade origem in origens)
                    query = query.Where(o => o.Origens.Contains(origem));
            }
            else
                query = query.Where(obj => obj.Origens.Count() == 0);

            if (clientesOrigem != null && clientesOrigem.Count > 0)
            {
                query = query.Where(o => o.ClientesOrigem.Count == clientesOrigem.Count);

                foreach (Dominio.Entidades.Cliente clienteOrigem in clientesOrigem)
                    query = query.Where(o => o.ClientesOrigem.Contains(clienteOrigem));
            }
            else
                query = query.Where(obj => obj.ClientesOrigem.Count() == 0);

            if (estadosOrigem != null && estadosOrigem.Count > 0)
            {
                query = query.Where(o => o.EstadosOrigem.Count == estadosOrigem.Count);

                foreach (Dominio.Entidades.Estado estadoOrigem in estadosOrigem)
                    query = query.Where(o => o.EstadosOrigem.Contains(estadoOrigem));
            }
            else
                query = query.Where(obj => obj.EstadosOrigem.Count() == 0);

            if (regioesOrigem != null && regioesOrigem.Count > 0)
            {
                query = query.Where(o => o.RegioesOrigem.Count == regioesOrigem.Count);

                foreach (Dominio.Entidades.Embarcador.Localidades.Regiao regiaoOrigem in regioesOrigem)
                    query = query.Where(o => o.RegioesOrigem.Contains(regiaoOrigem));
            }
            else
                query = query.Where(obj => obj.RegioesOrigem.Count() == 0);

            if (cepsOrigem != null && cepsOrigem.Count > 0)
            {
                query = query.Where(o => o.CEPsOrigem.Count == cepsOrigem.Count);

                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP faixaCEP in cepsOrigem)
                    query = query.Where(o => o.CEPsOrigem.Any(c => c.CEPInicial == faixaCEP.CEPInicial && c.CEPFinal == faixaCEP.CEPFinal));
            }
            else
                query = query.Where(obj => obj.CEPsOrigem.Count() == 0);

            if (rotasDestino != null && rotasDestino.Count > 0)
            {
                query = query.Where(o => o.RotasDestino.Count == rotasDestino.Count);

                foreach (Dominio.Entidades.RotaFrete rotaDestino in rotasDestino)
                    query = query.Where(o => o.RotasDestino.Contains(rotaDestino));
            }
            else
                query = query.Where(obj => obj.RotasDestino.Count() == 0);

            if (destinos != null && destinos.Count > 0)
            {
                query = query.Where(o => o.Destinos.Count == destinos.Count);

                List<int> codigosDestinos = destinos.Select(o => o.Codigo).ToList();
                decimal totalRepeticoes = destinos.Count / 2000m;

                for (int i = 0; i < totalRepeticoes; i++)
                {
                    List<int> codigosDestinosFiltrar = codigosDestinos.Skip(i * 2000).Take(2000).ToList();

                    query = query.Where(o => o.Destinos.All(destino => codigosDestinosFiltrar.Contains(destino.Codigo)));
                }
            }
            else
                query = query.Where(obj => obj.Destinos.Count() == 0);

            if (clientesDestino != null && clientesDestino.Count > 0)
            {
                query = query.Where(o => o.ClientesDestino.Count == clientesDestino.Count);

                foreach (Dominio.Entidades.Cliente clienteDestino in clientesDestino)
                    query = query.Where(o => o.ClientesDestino.Contains(clienteDestino));
            }
            else
                query = query.Where(obj => obj.ClientesDestino.Count() == 0);

            if (estadosDestino != null && estadosDestino.Count > 0)
            {
                query = query.Where(o => o.EstadosDestino.Count == estadosDestino.Count);

                foreach (Dominio.Entidades.Estado estadoDestino in estadosDestino)
                    query = query.Where(o => o.EstadosDestino.Contains(estadoDestino));
            }
            else
                query = query.Where(obj => obj.EstadosDestino.Count() == 0);

            if (regioesDestino != null && regioesDestino.Count > 0)
            {
                query = query.Where(o => o.RegioesDestino.Count == regioesDestino.Count);

                foreach (Dominio.Entidades.Embarcador.Localidades.Regiao regiaoDestino in regioesDestino)
                    query = query.Where(o => o.RegioesDestino.Contains(regiaoDestino));
            }
            else
                query = query.Where(obj => obj.RegioesDestino.Count() == 0);

            if (cepsDestino != null && cepsDestino.Count > 0)
            {
                query = query.Where(o => o.CEPsDestino.Count == cepsDestino.Count);

                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.FaixaCEP faixaCEP in cepsDestino)
                    query = query.Where(o => o.CEPsDestino.Any(c => c.CEPInicial == faixaCEP.CEPInicial && c.CEPFinal == faixaCEP.CEPFinal));
            }
            else
                query = query.Where(obj => obj.CEPsDestino.Count() == 0);

            if (fronteiras?.Count > 0)
            {
                query = query.Where(o => o.Fronteiras.Count == fronteiras.Count);

                foreach (Dominio.Entidades.Cliente fronteira in fronteiras)
                    query = query.Where(o => o.Fronteiras.Contains(fronteira));
            }
            else
                query = query.Where(obj => obj.Fronteiras.Count() == 0);

            if (tiposOperacao.Count > 0)
            {
                query = query.Where(o => o.TiposOperacao.Count == tiposOperacao.Count);

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
                    query = query.Where(o => o.TiposOperacao.Contains(tipoOperacao));
            }
            else
                query = query.Where(obj => obj.TiposOperacao.Count() == 0);

            if (tiposDeCarga.Count > 0)
            {
                query = query.Where(o => o.TiposCarga.Count == tiposDeCarga.Count);

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in tiposDeCarga)
                    query = query.Where(o => o.TiposCarga.Contains(tipoDeCarga));
            }
            else
                query = query.Where(obj => obj.TiposCarga.Count() == 0);

            if (codigoVigencia > 0)
                query = query.Where(obj => obj.Vigencia.Codigo == codigoVigencia);

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            //if (codigoTipoOperacao > 0)
            //    query = query.Where(o => o.TiposOperacao.Any(t => t.Codigo == codigoTipoOperacao));

            if (codigoCanalEntrega > 0)
                query = query.Where(o => o.CanalEntrega.Codigo == codigoCanalEntrega);
            else
                query = query.Where(o => o.CanalEntrega == null);

            if (codigoCanalVenda > 0)
                query = query.Where(o => o.CanalVenda.Codigo == codigoCanalVenda);
            else
                query = query.Where(o => o.CanalVenda == null);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente BuscarPorTabelaFreteCliente(int codigoTabelaFrete, List<Dominio.Entidades.Localidade> origens, List<Dominio.Entidades.Localidade> destinos, Dominio.Entidades.Cliente clienteOrigem, Dominio.Entidades.Cliente clienteDestino, int codigoRegiaoDestino, string estadoDestino, int codigoVigencia, int codigoTipoOperacao, int empresa, int codRotaFrete, bool importacaoTabelaFreteCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            if (importacaoTabelaFreteCliente)
                result = result.Where(obj => obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao);
            else
                result = result.Where(obj => obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo);

            if (clienteOrigem != null)
                result = result.Where(obj => obj.ClienteOrigem.CPF_CNPJ == clienteOrigem.CPF_CNPJ);
            else
                result = result.Where(obj => obj.ClienteOrigem == null);

            if (clienteDestino != null)
                result = result.Where(obj => obj.ClienteDestino.CPF_CNPJ == clienteDestino.CPF_CNPJ);
            else
                result = result.Where(obj => obj.ClienteDestino == null);

            if (origens != null && origens.Count > 0)
            {
                result = result.Where(o => o.Origens.Count == origens.Count);

                foreach (Dominio.Entidades.Localidade origem in origens)
                    result = result.Where(o => o.Origens.Contains(origem));
            }
            else
                result = result.Where(obj => obj.Origens.Count() == 0);

            if (destinos != null && destinos.Count > 0)
            {
                result = result.Where(o => o.Destinos.Count == destinos.Count);

                foreach (Dominio.Entidades.Localidade destino in destinos)
                    result = result.Where(o => o.Destinos.Contains(destino));
            }
            else
                result = result.Where(obj => obj.Destinos.Count() == 0);

            if (!string.IsNullOrWhiteSpace(estadoDestino))
                result = result.Where(obj => obj.EstadoDestino.Sigla == estadoDestino);
            else
                result = result.Where(obj => obj.EstadoDestino == null);

            if (codigoRegiaoDestino > 0)
                result = result.Where(o => o.RegiaoDestino.Codigo == codigoRegiaoDestino);
            else
                result = result.Where(o => o.RegiaoDestino == null);

            if (codigoVigencia > 0)
                result = result.Where(obj => obj.Vigencia.Codigo == codigoVigencia);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            if (codigoTipoOperacao > 0)
                result = result.Where(o => o.TiposOperacao.Any(t => t.Codigo == codigoTipoOperacao));
            //result = result.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            if (codRotaFrete > 0)
                result = result.Where(o => o.RotaFrete.Codigo == codRotaFrete);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var result = MontarQueryConsulta(filtrosPesquisa);

            return result.OrderBy(propOrdenar + (dirOrdena == "asc" ? " ascending" : " descending"))
                .Skip(inicio)
                .Take(limite)
                .Fetch(obj => obj.TabelaFrete)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Vigencia)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa)
        {
            var result = MontarQueryConsulta(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> MontarQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo || o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao);

            if (filtrosPesquisa.SituacaoTabelaFrete.HasValue)
            {
                if (filtrosPesquisa.SituacaoTabelaFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.TabelaFrete.Ativo);
                else if (filtrosPesquisa.SituacaoTabelaFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(o => !o.TabelaFrete.Ativo);
            }

            if (filtrosPesquisa.CodigoTabelaFrete > 0)
                query = query.Where(o => o.TabelaFrete.Codigo == filtrosPesquisa.CodigoTabelaFrete);

            if (filtrosPesquisa.RotaFrete > 0)
                query = query.Where(obj => obj.RotasDestino.Any(o => o.Codigo == filtrosPesquisa.RotaFrete));

            if (filtrosPesquisa.PossuiRota != Dominio.ObjetosDeValor.Embarcador.Enumeradores.PossuiRota.Todos)
            {
                if (filtrosPesquisa.PossuiRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PossuiRota.Sim)
                    query = query.Where(obj => obj.RotasDestino.Any(h => h.Codigo > 0) || obj.RotasOrigem.Any(h => h.Codigo > 0));
                else if (filtrosPesquisa.PossuiRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PossuiRota.Nao)
                    query = query.Where(obj => !obj.RotasDestino.Any(h => h.Codigo > 0) || !obj.RotasOrigem.Any(h => h.Codigo > 0));
            }

            if (filtrosPesquisa.TipoPagamento != null)
                query = query.Where(o => o.TipoPagamento == filtrosPesquisa.TipoPagamento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                query = query.Where(o => o.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Vigencia > 0)
                query = query.Where(o => o.Vigencia.Codigo == filtrosPesquisa.Vigencia);

            if (filtrosPesquisa.SomenteEmVigencia)
                query = query.Where(o => o.Vigencia.DataInicial <= DateTime.Now.Date && (!o.Vigencia.DataFinal.HasValue || o.Vigencia.DataFinal.Value >= DateTime.Now.Date));

            if (filtrosPesquisa.Ativo.HasValue)
            {
                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo);
                else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(o => !o.Ativo);
            }

            if (filtrosPesquisa.SituacaoAlteracaoTabelaFrete.HasValue)
            {
                if (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFreteHelper.IsAlteracaoTabelaFreteClienteLiberada(filtrosPesquisa.SituacaoAlteracaoTabelaFrete.Value))
                    query = query.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo);
                else
                    query = query.Where(o =>
                        (o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao) &&
                        (o.SituacaoAlteracao == filtrosPesquisa.SituacaoAlteracaoTabelaFrete.Value)
                    );
            }

            if (filtrosPesquisa.CpfCnpjTransportadorTerceiro > 0d)
                query = query.Where(o => o.TransportadoresTerceiros.Any(tt => tt.CPF_CNPJ == filtrosPesquisa.CpfCnpjTransportadorTerceiro));

            if (filtrosPesquisa.CodigoCanalEntrega > 0)
                query = query.Where(o => o.CanalEntrega.Codigo == filtrosPesquisa.CodigoCanalEntrega);

            if (filtrosPesquisa.CodigoLocalidadeOrigemFiltro > 0)
                query = query.Where(o => o.Origens.Any(a => a.Codigo == filtrosPesquisa.CodigoLocalidadeOrigemFiltro));

            if (filtrosPesquisa.CodigoLocalidadeDestinoFiltro > 0)
                query = query.Where(o => o.Destinos.Any(l => l.Codigo == filtrosPesquisa.CodigoLocalidadeDestinoFiltro));

            if (filtrosPesquisa.CodigoRegiaoDestinoFiltro > 0)
                query = query.Where(obj => obj.RegioesDestino.Any(r => r.Codigo == filtrosPesquisa.CodigoRegiaoDestinoFiltro));

            if (filtrosPesquisa.CodigoRegiaoOrigemFiltro > 0)
                query = query.Where(obj => obj.RegioesOrigem.Any(r => r.Codigo == filtrosPesquisa.CodigoRegiaoOrigemFiltro));

            if (filtrosPesquisa.CodigoTipoOperacaoFiltro > 0)
                query = query.Where(o => o.TiposOperacao.Any(t => t.Codigo == filtrosPesquisa.CodigoTipoOperacaoFiltro));

            if (filtrosPesquisa.CodigoEmpresaFiltro > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresaFiltro);

            if (filtrosPesquisa.CpfCnpjRemetenteFiltro > 0)
                query = query.Where(obj => obj.ClientesOrigem.Any(c => c.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetenteFiltro));

            if (filtrosPesquisa.CpfCnpjDestinatarioFiltro > 0)
                query = query.Where(obj => obj.ClientesDestino.Any(c => c.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatarioFiltro));

            if (filtrosPesquisa.CpfCnpjTomadorFiltro > 0)
                query = query.Where(obj => obj.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorFiltro);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDestino))
                query = query.Where(o => o.EstadosDestino.Any(e => e.Sigla.Equals(filtrosPesquisa.EstadoDestino)) || o.Destinos.Any(destino => destino.Estado.Sigla.Equals(filtrosPesquisa.EstadoDestino))); ;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoOrigem))
                query = query.Where(o => o.EstadosOrigem.Any(e => e.Sigla.Equals(filtrosPesquisa.EstadoOrigem)) || o.Origens.Any(origem => origem.Estado.Sigla.Equals(filtrosPesquisa.EstadoOrigem)));

            if (filtrosPesquisa.ContratoTransporteFrete > 0)
                query = query.Where(o => o.ContratoTransporteFrete.Codigo == filtrosPesquisa.ContratoTransporteFrete);

            if (filtrosPesquisa.SituacaoIntegracaoTabelaFreteCliente.HasValue)
                query = query.Where(o => o.SituacaoIntegracaoTabelaFreteCliente == filtrosPesquisa.SituacaoIntegracaoTabelaFreteCliente.Value);

            if (filtrosPesquisa.CEPOrigem > 0)
                query = query.Where(o => o.CEPsOrigem.Any(c => c.CEPInicial <= filtrosPesquisa.CEPOrigem && c.CEPFinal >= filtrosPesquisa.CEPOrigem));

            if (filtrosPesquisa.CEPDestino > 0)
                query = query.Where(o => o.CEPsDestino.Any(c => c.CEPInicial <= filtrosPesquisa.CEPDestino && c.CEPFinal >= filtrosPesquisa.CEPDestino));

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> _BuscarTabelasParaAjuste(int codigoAjusteTabelaFrete, int diferenteDoCodigoDeAjuste, int codigoTabelaFrete, int codigoVigencia, List<double> remetente, List<double> destinatario, List<double> tomador, List<int> codigoLocalidadeOrigem, List<string> estadoOrigem, List<int> codigoLocalidadeDestino, List<string> estadoDestino, List<int> codigoRegiaoDestino, List<int> codigoTipoOperacao, List<int> rotaFreteOrigem, List<int> rotaFreteDestino, List<int> empresa, int codigoContratoTransporteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao? tipoPagamento, bool tabelaComCargaRealizada, bool utilizarBuscaNasLocalidadesPorEstadoOrigem, bool utilizarBuscaNasLocalidadesPorEstadoDestino, bool apenasRegistrosComValor = false, List<int> codigoCanalVenda = null, List<int> codigoCanalEntrega = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo && o.Ativo);

            if (codigoAjusteTabelaFrete > 0)
            {
                query = query.Where(o => o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete);
            }
            else
            {
                query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && o.Vigencia.Codigo == codigoVigencia);

                if (diferenteDoCodigoDeAjuste > 0)
                {
                    /* Caso aconteça um erro durante a criação do ajuste
                     * Esse filtro impede que ocorra a criação duplicada do ajuste
                     */
                    var subQuery = from o in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                                   where o.AjusteTabelaFrete.Codigo == diferenteDoCodigoDeAjuste
                                   select o.TabelaOriginaria;

                    query = query.Where(o => !subQuery.Contains(o));
                }

                if (remetente != null && remetente.Count > 0)
                    query = query.Where(o => o.ClientesOrigem.Any(obj => remetente.Contains(obj.CPF_CNPJ)));

                if (destinatario != null && destinatario.Count > 0)
                    query = query.Where(o => o.ClientesDestino.Any(obj => destinatario.Contains(obj.CPF_CNPJ)));

                if (tomador != null && tomador.Count > 0)
                    query = query.Where(o => tomador.Contains(o.Tomador.CPF_CNPJ));

                if (codigoLocalidadeOrigem != null && codigoLocalidadeOrigem.Count > 0)
                    query = query.Where(o => o.Origens.Any(l => codigoLocalidadeOrigem.Contains(l.Codigo)));

                if (estadoOrigem != null && estadoOrigem.Count > 0)
                {
                    if (!utilizarBuscaNasLocalidadesPorEstadoOrigem)
                        query = query.Where(o => o.EstadosOrigem.Any(l => estadoOrigem.Contains(l.Sigla)));
                    else
                        query = query.Where(o => o.EstadosOrigem.Any(l => estadoOrigem.Contains(l.Sigla)) || o.Origens.Any(d => estadoOrigem.Contains(d.Estado.Sigla)));
                }

                if (codigoLocalidadeDestino != null && codigoLocalidadeDestino.Count > 0)
                    query = query.Where(o => o.Destinos.Any(l => codigoLocalidadeDestino.Contains(l.Codigo)));

                if (estadoDestino != null && estadoDestino.Count > 0)
                {
                    if (!utilizarBuscaNasLocalidadesPorEstadoDestino)
                        query = query.Where(o => o.EstadosDestino.Any(l => estadoDestino.Contains(l.Sigla)));
                    else
                        query = query.Where(o => o.EstadosDestino.Any(l => estadoDestino.Contains(l.Sigla)) || o.Destinos.Any(d => estadoDestino.Contains(d.Estado.Sigla)));
                }

                if (codigoRegiaoDestino != null && codigoRegiaoDestino.Count > 0)
                    query = query.Where(o => o.RegioesDestino.Any(l => codigoRegiaoDestino.Contains(l.Codigo)));

                if (codigoTipoOperacao != null && codigoTipoOperacao.Count > 0)
                    query = query.Where(o => o.TiposOperacao.Any(l => codigoTipoOperacao.Contains(l.Codigo)));
                //query = query.Where(o => codigoTipoOperacao.Contains(o.TipoOperacao.Codigo));

                if (rotaFreteOrigem != null && rotaFreteOrigem.Count > 0)
                    query = query.Where(o => o.RotasOrigem.Any(l => rotaFreteOrigem.Contains(l.Codigo)));

                if (rotaFreteDestino != null && rotaFreteDestino.Count > 0)
                    query = query.Where(o => o.RotasDestino.Any(l => rotaFreteDestino.Contains(l.Codigo)));

                if (empresa != null && empresa.Count > 0)
                    query = query.Where(o => empresa.Contains(o.Empresa.Codigo));

                if (codigoContratoTransporteFrete > 0)
                    query = query.Where(o => o.ContratoTransporteFrete.Codigo == codigoContratoTransporteFrete);

                if (tipoPagamento.HasValue)
                    query = query.Where(o => o.TipoPagamento == tipoPagamento);

                if (codigoCanalVenda != null && codigoCanalVenda.Count > 0)
                    query = query.Where(o => codigoCanalVenda.Contains(o.CanalVenda.Codigo));

                if (codigoCanalEntrega != null && codigoCanalEntrega.Count > 0)
                    query = query.Where(o => codigoCanalEntrega.Contains(o.CanalEntrega.Codigo));

                if (tabelaComCargaRealizada)
                {
                    var subQueryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();

                    query = query.Where(o => subQueryCargas.Any(s => s.TabelaFreteCliente.Codigo == o.Codigo));
                }
            }

            return query;
        }

        public List<int> BuscarCodigoTabelasParaAjuste(int codigoAjusteTabelaFrete, int codigoAjustEmProcesamento, int codigoTabelaFrete, int codigoVigencia, List<double> remetente, List<double> destinatario, List<double> tomador, List<int> codigoLocalidadeOrigem, List<string> estadoOrigem, List<int> codigoLocalidadeDestino, List<string> estadoDestino, List<int> codigoRegiaoDestino, List<int> codigoTipoOperacao, List<int> rotaFreteOrigem, List<int> rotaFreteDestino, List<int> empresa, int codigoContratoTransporteFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao? tipoPagamento, bool tabelaComCargaRealizada, bool utilizarBuscaNasLocalidadesPorEstadoOrigem, bool utilizarBuscaNasLocalidadesPorEstadoDestino, bool apenasRegistrosComValor, List<int> codigoCanalVenda, List<int> codigoCanalEntrega)
        {
            var query = _BuscarTabelasParaAjuste(
                    codigoAjusteTabelaFrete,
                    codigoAjustEmProcesamento,
                    codigoTabelaFrete,
                    codigoVigencia,
                    remetente,
                    destinatario,
                    tomador,
                    codigoLocalidadeOrigem,
                    estadoOrigem,
                    codigoLocalidadeDestino,
                    estadoDestino,
                    codigoRegiaoDestino,
                    codigoTipoOperacao,
                    rotaFreteOrigem,
                    rotaFreteDestino,
                    empresa,
                    codigoContratoTransporteFrete,
                    tipoPagamento,
                    tabelaComCargaRealizada,
                    utilizarBuscaNasLocalidadesPorEstadoOrigem,
                    utilizarBuscaNasLocalidadesPorEstadoDestino,
                    apenasRegistrosComValor,
                    codigoCanalVenda,
                    codigoCanalEntrega
                );

            return query.Select(o => o.Codigo).Timeout(600).ToList();
        }

        public List<int> BuscarCodigosTabelasClienteOriginaisPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from o in query
                         where
                            o.TabelaFrete.Codigo == codigoTabelaFrete
                            && o.TabelaOriginaria == null
                         select o;

            return result.Select(o => o.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosTabelasClientePorTabelaFreteEOriginal(int codigoTabelaFrete, int codigoTabelaFreteClienteOriginaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from o in query
                         where
                            o.TabelaFrete.Codigo == codigoTabelaFrete
                            && o.TabelaOriginaria != null
                            && o.TabelaOriginaria.Codigo == codigoTabelaFreteClienteOriginaria
                         select o;

            return result.Select(o => o.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigoTabelasParaAjusteSemFiltro(int diferenteDoCodigoDeAjuste, int codigoTabelaFrete, int codigoVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoEmissao? tipoPagamento, bool tabelaComCargaRealizada, bool utilizarBuscaNasLocalidadesPorEstadoOrigem, bool utilizarBuscaNasLocalidadesPorEstadoDestino)
        {
            var query = _BuscarTabelasParaAjuste(
                    0,
                    diferenteDoCodigoDeAjuste,
                    codigoTabelaFrete,
                    codigoVigencia,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    0,
                    tipoPagamento,
                    tabelaComCargaRealizada,
                    utilizarBuscaNasLocalidadesPorEstadoOrigem,
                    utilizarBuscaNasLocalidadesPorEstadoDestino,
                    false,
                    null,
                    null
                );

            return query.Select(o => o.Codigo).Timeout(300).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarTabelasPorAjuste(int codigoAjusteTabelaFrete, bool apenasTabelasAplicaveisDeAlteracao = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from o in query
                         where
                            o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete
                            && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Ajuste
                            && o.Ativo
                         select o;

            if (apenasTabelasAplicaveisDeAlteracao)
                result = result.Where(o => o.AplicarAlteracoesDoAjuste == true);

            return result.Fetch(obj => obj.TabelaFrete).ToList();
        }

        public List<int> BuscarCodigosTabelasPorAjuste(int codigoAjusteTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from o in query
                         where
                            o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete
                            && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Ajuste
                            && o.Ativo
                         select o;

            return result.Select(o => o.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigosTabelasFreteClientePorAjuste(List<int> codigosTabelaFreteAjustes)
        {
            var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o => codigosTabelaFreteAjustes.Contains(o.AjusteTabelaFrete.Codigo));

            return consultaTabelaFreteClienteAlteracao.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarDestinoTabelasPorAjusteTabelaFrete(int codigoAjusteTabelaFrete)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o =>
                    o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete &&
                    o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Ajuste &&
                    o.Ativo
                );

            return consultaTabelaFreteCliente.Select(o => o.Destinos.Select(or => or.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public List<int> BuscarDestinoTabelasPorTabelaFreteAlteracao(int codigoTabelaFrete, int codigoTabelaFreteCliente)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o =>
                    o.TabelaFrete.Codigo == codigoTabelaFrete &&
                    (o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao || o.Codigo == codigoTabelaFreteCliente) &&
                    o.Ativo
                );

            return consultaTabelaFreteCliente.Select(o => o.Destinos.Select(or => or.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public List<int> BuscarOrigemTabelasPorAjusteTabelaFrete(int codigoAjusteTabelaFrete)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o =>
                    o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete &&
                    o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Ajuste &&
                    o.Ativo
                );

            return consultaTabelaFreteCliente.Select(o => o.Origens.Select(or => or.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public List<int> BuscarOrigemTabelasPorTabelaFreteAlteracao(int codigoTabelaFrete, int codigoTabelaFreteCliente)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o =>
                    o.TabelaFrete.Codigo == codigoTabelaFrete &&
                    (o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao || o.Codigo == codigoTabelaFreteCliente) &&
                    o.Ativo
                );

            return consultaTabelaFreteCliente.Select(o => o.Origens.Select(or => or.Codigo)).SelectMany(o => o).Distinct().ToList();
        }

        public IList<(int Codigo, string Descricao)> BuscarDadosModeloVeicularCargaPorParametroBaseComValorInformado(int codigoTabelaFreteCliente)
        {
            string sql = $@"
                SELECT DISTINCT ParametroBase.TBC_CODIGO_OBJETO Codigo, ModeloVeicularCarga.MVC_DESCRICAO Descricao
                  from T_TABELA_FRETE_CLIENTE TabelaCliente 
                  join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = TabelaCliente.TBF_CODIGO
                  join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO ParametroBase on ParametroBase.TFC_CODIGO = TabelaCliente.TFC_CODIGO
                  join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemParametroBase on ItemParametroBase.TBC_CODIGO = ParametroBase.TBC_CODIGO
                  join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO =  ParametroBase.TBC_CODIGO_OBJETO
                where TabelaCliente.TFC_CODIGO = {codigoTabelaFreteCliente}
                  AND (
                          ItemParametroBase.TPI_TIPO_OBJETO = {(int)TipoParametroBaseTabelaFrete.Peso} OR
                		  ItemParametroBase.TPI_TIPO_OBJETO = {(int)TipoParametroBaseTabelaFrete.TipoCarga} OR
                		  ItemParametroBase.TPI_TIPO_OBJETO = {(int)TipoParametroBaseTabelaFrete.Distancia}
                      )
                  AND TabelaFrete.TBF_PARAMETRO_BASE = 2
                  and ItemParametroBase.TPI_VALOR > 0";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int Codigo, string Descricao)).GetConstructors().FirstOrDefault()));

            return consulta.SetTimeout(600).List<(int Codigo, string Descricao)>();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarTabelasPorAjusteAplicacaoAjuste(int codigoAjusteTabelaFrete, DateTime dataProcessamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from o in query
                         where
                            o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete
                            && o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Ajuste
                            && (
                                !o.DataProcessamentoValores.HasValue
                                || o.DataProcessamentoValores.Value < dataProcessamento
                            )
                            && o.Ativo
                         select o;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarTodasPorTabela(int codigoTabela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            query = query.Where(o => o.TabelaFrete.Codigo == codigoTabela && o.Ativo);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarPorLicitacaoParticipacao(int codigo)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o => o.LicitacaoParticipacao.Codigo == codigo);

            return consultaTabelaFreteCliente.ToList();
        }

        public List<(int Codigo, string Descricao)> BuscarOpcoesPorLicitacaoParticipacao(int codigoLicitacaoParticipacao, int codigoTabelaFrete)
        {
            var consultaTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o => o.LicitacaoParticipacao.Codigo == codigoLicitacaoParticipacao);

            if (codigoTabelaFrete > 0)
                consultaTabelaFreteCliente = consultaTabelaFreteCliente.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);

            return consultaTabelaFreteCliente
                .Select(o => ValueTuple.Create(o.Codigo, $"{o.DescricaoOrigem} até {o.DescricaoDestino}"))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> ConsultarTabelasAgrupamento(string codigo, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var result = _ConsultarTabelasAgrupamento(codigo);

            if (!string.IsNullOrWhiteSpace(propOrdenar))
                result = result.OrderBy(propOrdenar + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicio > 0)
                result = result.Skip(inicio);

            if (limite > 0)
                result = result.Take(limite);

            return result.ToList();
        }

        public int ContarConsultarTabelasAgrupamento(string codigo)
        {
            var result = _ConsultarTabelasAgrupamento(codigo);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> _ConsultarTabelasAgrupamento(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            var result = from obj in query
                         where
                             obj.Ativo
                             && obj.CodigoIntegracao.Length > 1
                             && obj.CodigoIntegracao == codigo
                             && obj.TabelaFrete.Ativo
                             && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo
                             && obj.Vigencia.DataInicial <= DateTime.Now.Date && (!obj.Vigencia.DataFinal.HasValue || obj.Vigencia.DataFinal.Value >= DateTime.Now.Date)
                             && obj.Ativo
                         select obj;

            return result;
        }

        public void AtualizarSituacaoAlteracaoPorTabelaFreteAlteracao(int codigoTabelaFreteAlteracao, SituacaoAlteracaoTabelaFrete situacaoAlteracao)
        {
            System.Text.StringBuilder hql = new System.Text.StringBuilder();

            hql.Append("update TabelaFreteCliente tabelaFreteCliente ");
            hql.Append("   set tabelaFreteCliente.SituacaoAlteracao = :SituacaoAlteracao ");
            hql.Append(" where tabelaFreteCliente.Codigo in ( ");
            hql.Append("           select alteracao.TabelaFreteCliente.Codigo ");
            hql.Append("             from TabelaFreteClienteAlteracao alteracao ");
            hql.Append("            where alteracao.TabelaFreteAlteracao.Codigo = :CodigoTabelaFreteAlteracao ");
            hql.Append("       ) ");

            var query = this.SessionNHiBernate.CreateQuery(hql.ToString())
                .SetInt32("CodigoTabelaFreteAlteracao", codigoTabelaFreteAlteracao)
                .SetEnum("SituacaoAlteracao", situacaoAlteracao);

            query.SetTimeout(120).ExecuteUpdate();
        }

        public void AtualizarSituacaoIntegracaoPorAjusteTabelaFrete(int codigoAjusteTabelaFrete, SituacaoIntegracaoTabelaFreteCliente situacaoIntegracao)
        {
            System.Text.StringBuilder hql = new System.Text.StringBuilder();

            hql.Append("update TabelaFreteCliente tabelaFreteCliente ");
            hql.Append("   set tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente = :SituacaoIntegracao ");
            hql.Append(" where tabelaFreteCliente.Codigo in ( ");
            hql.Append("           select tabelaFreteClienteAjuste.TabelaOriginaria.Codigo ");
            hql.Append("             from TabelaFreteCliente tabelaFreteClienteAjuste ");
            hql.Append("            where tabelaFreteClienteAjuste.AjusteTabelaFrete.Codigo = :CodigoAjusteTabelaFrete ");
            hql.Append("       ) ");

            var query = this.SessionNHiBernate.CreateQuery(hql.ToString())
                .SetInt32("CodigoAjusteTabelaFrete", codigoAjusteTabelaFrete)
                .SetEnum("SituacaoIntegracao", situacaoIntegracao);

            query.SetTimeout(120).ExecuteUpdate();
        }

        public void AtualizarSituacaoIntegracaoPorTabelaFreteAlteracao(int codigoTabelaFreteAlteracao, SituacaoIntegracaoTabelaFreteCliente situacaoIntegracao)
        {
            System.Text.StringBuilder hql = new System.Text.StringBuilder();

            hql.Append("update TabelaFreteCliente tabelaFreteCliente ");
            hql.Append("   set tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente = :SituacaoIntegracao ");
            hql.Append(" where tabelaFreteCliente.Codigo in ( ");
            hql.Append("           select alteracao.TabelaFreteCliente.Codigo ");
            hql.Append("             from TabelaFreteClienteAlteracao alteracao ");
            hql.Append("            where alteracao.TabelaFreteAlteracao.Codigo = :CodigoTabelaFreteAlteracao ");
            hql.Append("       ) ");

            var query = this.SessionNHiBernate.CreateQuery(hql.ToString())
                .SetInt32("CodigoTabelaFreteAlteracao", codigoTabelaFreteAlteracao)
                .SetEnum("SituacaoIntegracao", situacaoIntegracao);

            query.SetTimeout(120).ExecuteUpdate();
        }

        public void BloquearAlteracaoPorTabelaFreteAlteracao(int codigoTabelaFreteAlteracao, SituacaoAlteracaoTabelaFrete situacaoAlteracao)
        {
            System.Text.StringBuilder hql = new System.Text.StringBuilder();

            hql.Append("update TabelaFreteCliente tabelaFreteCliente ");
            hql.Append("   set tabelaFreteCliente.Tipo = :Tipo, ");
            hql.Append("       tabelaFreteCliente.SituacaoAlteracao = :SituacaoAlteracao ");
            hql.Append(" where tabelaFreteCliente.Codigo in ( ");
            hql.Append("           select alteracao.TabelaFreteCliente.Codigo ");
            hql.Append("             from TabelaFreteClienteAlteracao alteracao ");
            hql.Append("            where alteracao.TabelaFreteAlteracao.Codigo = :CodigoTabelaFreteAlteracao ");
            hql.Append("       ) ");

            var query = this.SessionNHiBernate.CreateQuery(hql.ToString())
                .SetInt32("CodigoTabelaFreteAlteracao", codigoTabelaFreteAlteracao)
                .SetEnum("SituacaoAlteracao", situacaoAlteracao)
                .SetEnum("Tipo", TipoTabelaFreteCliente.Alteracao);

            query.SetTimeout(120).ExecuteUpdate();
        }

        public void LiberarAlteracaoPorTabelaFrete(int codigoTabelaFrete, SituacaoItemParametroBaseCalculoTabelaFrete situacaoItem)
        {
            string sqlAtualizarSituacaoItens = @"
                update ItemAtualizarSituacao
                   set Situacao = :NovaSituacao
                  from (
                           select Item.TPI_SITUACAO as Situacao
                             from T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM Item
                             left join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO ParametroBase on Item.TBC_CODIGO = ParametroBase.TBC_CODIGO
                            where Item.TPI_SITUACAO = :SituacaoAtual
                              and isnull(ParametroBase.TFC_CODIGO, Item.TFC_CODIGO) in (
                                      select TabelaFreteCliente.TFC_CODIGO
                                        from T_TABELA_FRETE_CLIENTE TabelaFreteCliente
                                       where TabelaFreteCliente.TBF_CODIGO = :CodigoTabelaFrete
                                  )
                       ) ItemAtualizarSituacao";

            this.SessionNHiBernate.CreateSQLQuery(sqlAtualizarSituacaoItens)
                .SetEnum("NovaSituacao", situacaoItem)
                .SetEnum("SituacaoAtual", SituacaoItemParametroBaseCalculoTabelaFrete.Aprovacao)
                .SetInt32("CodigoTabelaFrete", codigoTabelaFrete)
                .SetTimeout(120)
                .ExecuteUpdate();

            string sqlAtualizarSituacaoTabelaFreteCliente = @"
                update T_TABELA_FRETE_CLIENTE
                   set TFC_TIPO = :NovoTipo,
                       TFC_SITUACAO_ALTERACAO = :SituacaoAlteracao
                 where TBF_CODIGO = :CodigoTabelaFrete
                   and TFC_TIPO = :TipoAtual";

            this.SessionNHiBernate.CreateSQLQuery(sqlAtualizarSituacaoTabelaFreteCliente)
                .SetEnum("NovoTipo", TipoTabelaFreteCliente.Calculo)
                .SetEnum("SituacaoAlteracao", SituacaoAlteracaoTabelaFrete.NaoInformada)
                .SetInt32("CodigoTabelaFrete", codigoTabelaFrete)
                .SetEnum("TipoAtual", TipoTabelaFreteCliente.Alteracao)
                .SetTimeout(120)
                .ExecuteUpdate();
        }

        public void LiberarAlteracaoPorTabelaFreteAlteracao(int codigoTabelaFreteAlteracao, SituacaoItemParametroBaseCalculoTabelaFrete situacaoItem)
        {
            string sqlAtualizarSituacaoItens = @"
                update ItemAtualizarSituacao
                   set Situacao = :NovaSituacao
                  from (
                           select Item.TPI_SITUACAO as Situacao
                             from T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM Item
                             left join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO ParametroBase on Item.TBC_CODIGO = ParametroBase.TBC_CODIGO
                            where Item.TPI_SITUACAO = :SituacaoAtual
                              and isnull(ParametroBase.TFC_CODIGO, Item.TFC_CODIGO) in (
                                      select Alteracao.TFC_CODIGO
                                        from T_TABELA_FRETE_CLIENTE_ALTERACAO Alteracao
                                       where Alteracao.TFA_CODIGO = :CodigoTabelaFreteAlteracao
                                  )
                       ) ItemAtualizarSituacao";

            this.SessionNHiBernate.CreateSQLQuery(sqlAtualizarSituacaoItens)
                .SetEnum("NovaSituacao", situacaoItem)
                .SetEnum("SituacaoAtual", SituacaoItemParametroBaseCalculoTabelaFrete.Aprovacao)
                .SetInt32("CodigoTabelaFreteAlteracao", codigoTabelaFreteAlteracao)
                .SetTimeout(120)
                .ExecuteUpdate();

            string sqlAtualizarSituacaoTabelaFreteCliente = @"
                update T_TABELA_FRETE_CLIENTE
                   set TFC_TIPO = :Tipo,
                       TFC_SITUACAO_ALTERACAO = :SituacaoAlteracao
                 where TFC_CODIGO in (
                           select Alteracao.TFC_CODIGO
                             from T_TABELA_FRETE_CLIENTE_ALTERACAO Alteracao
                            where Alteracao.TFA_CODIGO = :CodigoTabelaFreteAlteracao
                       )";

            this.SessionNHiBernate.CreateSQLQuery(sqlAtualizarSituacaoTabelaFreteCliente)
                .SetEnum("Tipo", TipoTabelaFreteCliente.Calculo)
                .SetEnum("SituacaoAlteracao", SituacaoAlteracaoTabelaFrete.Aprovada)
                .SetInt32("CodigoTabelaFreteAlteracao", codigoTabelaFreteAlteracao)
                .SetTimeout(120)
                .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> BuscarTabelaFreteClienteComValorReajuste(List<int> codigosTabelaFrete)
        {
            var queryTabelaFreteCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
            var queryAjusteTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            var resultQueryTabelaFreteCliente = queryTabelaFreteCliente.Where(obj => codigosTabelaFrete.Contains(obj.AjusteTabelaFrete.Codigo));
            var resultQueryAjusteTabelaFrete = queryAjusteTabelaFrete.Where(o => resultQueryTabelaFreteCliente.Where(x => x.AjusteTabelaFrete.Codigo == o.Codigo).Any());

            return resultQueryAjusteTabelaFrete.ToList();
        }

        #endregion

        #region Métodos Públicos - SQL

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan> BuscarRelatorioConsultaTabelaFretePorRota(List<int> codigosTabelaFrete, DateTime dataVigencia)
        {
            string query = $@"SELECT TabelaFreteCliente.TFC_CODIGO Codigo,
                                   ItemTipoCarga.TPI_VALOR ValorTipoCarga,
                                   CONVERT(VARCHAR(10), Vigencia.TFV_DATA_FINAL, 103) DataFinal,
                                   ModeloReboque.MVC_DESCRICAO DescricaoParametroBase,
                                   TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracao
                            FROM t_tabela_frete_cliente TabelaFreteCliente
                            LEFT OUTER JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro ON TabelaFreteCliente.TFC_CODIGO = Parametro.TFC_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTipoCarga ON Parametro.TBC_CODIGO = ItemTipoCarga.TBC_CODIGO
                            AND ItemTipoCarga.TPI_TIPO_OBJETO = 1
                            LEFT OUTER JOIN T_TIPO_DE_CARGA ItemItemTipoCarga ON ItemItemTipoCarga.TCG_CODIGO = ItemTipoCarga.TPI_CODIGO_OBJETO
                            LEFT OUTER JOIN T_TABELA_FRETE_VIGENCIA Vigencia ON TabelaFreteCliente.TFV_CODIGO = Vigencia.TFV_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque ON Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO
                            LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloReboque ON PModeloReboque.MVC_CODIGO = ModeloReboque.MVC_CODIGO
                            WHERE TabelaFreteCliente.TFC_TIPO = 0
                              AND TabelaFreteCliente.TFC_ATIVO = 1
                              AND TabelaFreteCliente.TBF_CODIGO in ({string.Join(", ", codigosTabelaFrete)})
                              AND TabelaFreteCliente.TFC_CODIGO_INTEGRACAO is not null and TabelaFreteCliente.TFC_CODIGO_INTEGRACAO <> ''
                              AND ItemTipoCarga.TPI_VALOR > 0
                              AND (ModeloReboque.MVC_DESCRICAO not like '%carreta%' or ModeloReboque.MVC_DESCRICAO = 'CARRETA3E' or ModeloReboque.MVC_DESCRICAO like '%HAVAN%')
                              AND Vigencia.TFV_DATA_INICIAL <= '{dataVigencia:yyyy-MM-dd}' AND (Vigencia.TFV_DATA_FINAL IS NULL OR Vigencia.TFV_DATA_FINAL >= '{dataVigencia:yyyy-MM-dd}')
                            GROUP BY TabelaFreteCliente.TFC_CODIGO,
                                     TabelaFrete.TBF_DESCRICAO,
                                     ItemTipoCarga.TPI_CODIGO,
                                     ItemItemTipoCarga.TCG_DESCRICAO,
                                     ItemTipoCarga.TPI_VALOR,
                                     ItemTipoCarga.TPI_TIPO_VALOR,
                                     Vigencia.TFV_DATA_FINAL,
                                     ModeloReboque.MVC_DESCRICAO,
                                     TabelaFrete.TBF_PARAMETRO_BASE,
                                     TabelaFreteCliente.TFC_CODIGO_INTEGRACAO
                            ORDER BY CodigoIntegracao ASC";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);
            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan)));
            return nhQuery.SetTimeout(900).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan> BuscarRelatorioConsultaTabelaFretePorPedagio(int codigoTabelaFrete, DateTime dataVigencia)
        {
            string query = $@"SELECT Codigo, ValorComponente1, DataInicial, DescricaoParametroBase, CodigoIntegracao FROM (
                            SELECT TabelaFreteCliente.TFC_CODIGO Codigo,
                                   ValorComponente1.TPI_VALOR ValorComponente1,
                                   CONVERT(VARCHAR(10), Vigencia.TFV_DATA_INICIAL, 103) DataInicial,
                                   ModeloReboque.MVC_DESCRICAO DescricaoParametroBase,
                                   TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracao
                            FROM t_tabela_frete_cliente TabelaFreteCliente
                            LEFT OUTER JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro ON TabelaFreteCliente.TFC_CODIGO = Parametro.TFC_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ValorComponente1 ON ValorComponente1.TBC_CODIGO = Parametro.TBC_CODIGO
                            AND ValorComponente1.TPI_TIPO_OBJETO = 4
                            AND ValorComponente1.TPI_CODIGO_OBJETO = 3
                            LEFT OUTER JOIN T_TABELA_FRETE_VIGENCIA Vigencia ON TabelaFreteCliente.TFV_CODIGO = Vigencia.TFV_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque ON Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO
                            LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloReboque ON PModeloReboque.MVC_CODIGO = ModeloReboque.MVC_CODIGO
                            WHERE TabelaFreteCliente.TFC_TIPO = 0
                              AND TabelaFreteCliente.TFC_ATIVO = 1
                              AND TabelaFreteCliente.TBF_CODIGO = {codigoTabelaFrete}
                              AND TabelaFreteCliente.TFC_CODIGO_INTEGRACAO is not null and TabelaFreteCliente.TFC_CODIGO_INTEGRACAO <> ''
                              AND ValorComponente1.TPI_VALOR > 0
                              AND (ModeloReboque.MVC_DESCRICAO not like '%carreta%' or ModeloReboque.MVC_DESCRICAO = 'CARRETA3E' or ModeloReboque.MVC_DESCRICAO like '%HAVAN%')
                              AND Vigencia.TFV_DATA_INICIAL <= '{dataVigencia:yyyy-MM-dd}' AND (Vigencia.TFV_DATA_FINAL IS NULL OR Vigencia.TFV_DATA_FINAL >= '{dataVigencia:yyyy-MM-dd}')
                            GROUP BY TabelaFreteCliente.TFC_CODIGO,
                                     TabelaFrete.TBF_DESCRICAO,
                                     ValorComponente1.TPI_VALOR,
                                     ValorComponente1.TPI_TIPO_VALOR,
                                     ValorComponente1.TPI_CODIGO,
                                     Vigencia.TFV_DATA_INICIAL,
                                     ModeloReboque.MVC_DESCRICAO,
                                     TabelaFrete.TBF_PARAMETRO_BASE,
                                     TabelaFreteCliente.TFC_CODIGO_INTEGRACAO
                            UNION
                            SELECT TabelaFreteCliente.TFC_CODIGO Codigo,
                                   ValorComponente1.TPI_VALOR ValorComponente1,
                                   CONVERT(VARCHAR(10), Vigencia.TFV_DATA_INICIAL, 103) DataInicial,
                                   ModeloReboque.MVC_DESCRICAO DescricaoParametroBase,
                                   TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracao
                            FROM t_tabela_frete_cliente TabelaFreteCliente
                            LEFT OUTER JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro ON TabelaFreteCliente.TFC_CODIGO = Parametro.TFC_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ValorComponente1 ON ValorComponente1.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
                            AND ValorComponente1.TPI_TIPO_OBJETO = 4
                            AND ValorComponente1.TPI_CODIGO_OBJETO = 3
                            LEFT OUTER JOIN T_TABELA_FRETE_VIGENCIA Vigencia ON TabelaFreteCliente.TFV_CODIGO = Vigencia.TFV_CODIGO
                            LEFT OUTER JOIN T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque ON Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO
                            LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloReboque ON PModeloReboque.MVC_CODIGO = ModeloReboque.MVC_CODIGO
                            WHERE TabelaFreteCliente.TFC_TIPO = 0
                              AND TabelaFreteCliente.TFC_ATIVO = 1
                              AND TabelaFreteCliente.TBF_CODIGO = {codigoTabelaFrete}
                              AND TabelaFreteCliente.TFC_CODIGO_INTEGRACAO is not null and TabelaFreteCliente.TFC_CODIGO_INTEGRACAO <> ''
                              AND ValorComponente1.TPI_VALOR > 0
                              AND (ModeloReboque.MVC_DESCRICAO not like '%carreta%' or ModeloReboque.MVC_DESCRICAO = 'CARRETA3E')
                              AND Vigencia.TFV_DATA_INICIAL <= '{dataVigencia:yyyy-MM-dd}' AND (Vigencia.TFV_DATA_FINAL IS NULL OR Vigencia.TFV_DATA_FINAL >= '{dataVigencia:yyyy-MM-dd}')
                            GROUP BY TabelaFreteCliente.TFC_CODIGO,
                                     TabelaFrete.TBF_DESCRICAO,
                                     ValorComponente1.TPI_VALOR,
                                     ValorComponente1.TPI_TIPO_VALOR,
                                     ValorComponente1.TPI_CODIGO,
                                     Vigencia.TFV_DATA_INICIAL,
                                     ModeloReboque.MVC_DESCRICAO,
                                     TabelaFrete.TBF_PARAMETRO_BASE,
                                     TabelaFreteCliente.TFC_CODIGO_INTEGRACAO
                            ) AS T
                            ORDER BY CodigoIntegracao ASC";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);
            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan)));
            return nhQuery.SetTimeout(900).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavanModeloVeicular> BuscarRelatorioConsultaTabelaFretePorModeloVeicular(int codigoTabelaFrete, DateTime dataVigencia)
        {
            string query = $@"
            SELECT [Codigo],
                    [CodigoIntegracao],
                    [Quilometragem],

                    MAX(COALESCE([BITREM], 0)) [BITREM],
					MAX(COALESCE([PEDAGIOBITREM], 0)) [PEDAGIOBITREM],
                    MAX(COALESCE([CARRETA2E], 0)) [CARRETA2E],
					MAX(COALESCE([PEDAGIOCARRETA2E], 0)) [PEDAGIOCARRETA2E],
                    MAX(COALESCE([CARRETA3E], 0)) [CARRETA3E],
					MAX(COALESCE([PEDAGIOCARRETA3E], 0)) [PEDAGIOCARRETA3E],
                    MAX(COALESCE([TOCO], 0)) [TOCO],
					MAX(COALESCE([PEDAGIOTOCO], 0)) [PEDAGIOTOCO],
                    MAX(COALESCE([TRUCK], 0)) [TRUCK],
					MAX(COALESCE([PEDAGIOTRUCK], 0)) [PEDAGIOTRUCK],
                    MAX(COALESCE([CARRETA 6 EIXOS LS], 0)) [CARRETA6EIXOSLS],
					MAX(COALESCE([PEDAGIOCARRETA 6 EIXOS LS], 0)) [PEDAGIOCARRETA6EIXOSLS],
                    MAX(COALESCE([CARRETA 6 EIXOS VANDERLEIA], 0)) [CARRETA6EIXOSVANDERLEIA],
					MAX(COALESCE([PEDAGIOCARRETA 6 EIXOS VANDERLEIA], 0)) [PEDAGIOCARRETA6EIXOSVANDERLEIA],
                    MAX(COALESCE([CARRETA RODOTREM], 0)) [CARRETARODOTREM],
					MAX(COALESCE([PEDAGIOCARRETA RODOTREM], 0)) [PEDAGIOCARRETARODOTREM],
                    MAX(COALESCE([RODOTREM], 0)) [RODOTREM],
					MAX(COALESCE([PEDAGIORODOTREM], 0)) [PEDAGIORODOTREM],

                    MAX(COALESCE([BITREM HAVAN], 0)) [BITREMHAVAN],
					MAX(COALESCE([PEDAGIOBITREM HAVAN], 0)) [PEDAGIOBITREMHAVAN],					
					MAX(COALESCE([CARRETA HAVAN], 0)) [CARRETAHAVAN],
					MAX(COALESCE([PEDAGIOCARRETA HAVAN], 0)) [PEDAGIOCARRETAHAVAN],					
					MAX(COALESCE([CARRETA RODOTREM HAVAN], 0)) [CARRETARODOTREMHAVAN],
					MAX(COALESCE([PEDAGIOCARRETA RODOTREM HAVAN], 0)) [PEDAGIOCARRETARODOTREMHAVAN],					
					MAX(COALESCE([RODOTREM HAVAN], 0)) [RODOTREMHAVAN],
					MAX(COALESCE([PEDAGIORODOTREM HAVAN], 0)) [PEDAGIORODOTREMHAVAN],

                    [BairroOrigem],
                    [CidadeOrigem],
                    [UFOrigem],
                    [BairroDestino],
                    [CidadeDestino],
                    [UFDestino]
            FROM
                (SELECT tabs.[Codigo],
                        tabs.[CodigoIntegracao],
                        tabs.[Quilometragem],
                        tabs.[DescricaoModeloReboque],
						tabs.[DescricaoModeloReboquePedagio],
                        tabs.[ValorTipoCarga],
						tabs.[ValorPedagio],
                        tabs.[BairroOrigem],
                        tabs.[CidadeOrigem],
                        tabs.[UFOrigem],
                        tabs.[BairroDestino],
                        tabs.[CidadeDestino],
                        tabs.[UFDestino]
                FROM
                    (SELECT TabelaFreteCliente.TFC_CODIGO Codigo,
                            TabelaFrete.TBF_DESCRICAO TabelaFrete,
                            tabelafrete.TBF_CODIGO [Cód.tabela frete],
                            TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracao,
                            ISNULL(TabelaFreteCliente.TFC_QUILOMETRAGEM, 0) Quilometragem,
                            ModeloReboque.MVC_DESCRICAO DescricaoParametroBase,
                            TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase,
							(SELECT SUM(ItemComponenteFrete.TPI_VALOR)
							FROM T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemComponenteFrete
							JOIN T_TABELA_FRETE_COMPONENTE_FRETE TabelaFreteComponenteFrete ON TabelaFreteComponenteFrete.TFC_CODIGO = ItemComponenteFrete.TPI_CODIGO_OBJETO
							JOIN T_COMPONENTE_FRETE ComponentePedagio ON ComponentePedagio.CFR_CODIGO = TabelaFreteComponenteFrete.CFR_CODIGO WHERE Parametro.TBC_CODIGO = ItemComponenteFrete.TBC_CODIGO AND ItemComponenteFrete.TPI_TIPO_OBJETO = 4 AND ComponentePedagio.CFR_TIPO_COMPONENTE_FRETE = 2) ValorPedagio,
                            coalesce(ItemTipoCarga.TPI_VALOR, 0) [ValorTipoCarga],
                            cast(Vigencia.TFV_DATA_FINAL AS date) DataFinal,
                            cast(Vigencia.TFV_DATA_INICIAL AS date) DataInicial,

                            SUBSTRING((SELECT DISTINCT ', ' + cliente.CLI_BAIRRO
							FROM T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM clientesOrigem
							INNER JOIN T_CLIENTE cliente on cliente.CLI_CGCCPF = clientesOrigem.CLI_CGCCPF
							WHERE clientesOrigem.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('')), 3, 1000) AS BairroOrigem,
							SUBSTRING((SELECT DISTINCT ', ' + cliente.CLI_BAIRRO
							FROM T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO clientesDestino
							INNER JOIN T_CLIENTE cliente on cliente.CLI_CGCCPF = clientesDestino.CLI_CGCCPF
							WHERE clientesDestino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('')), 3, 1000) AS BairroDestino,
							
							SUBSTRING((SELECT DISTINCT ', ' + localidade.LOC_DESCRICAO
							FROM T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM clientesOrigem
							INNER JOIN T_CLIENTE cliente on cliente.CLI_CGCCPF = clientesOrigem.CLI_CGCCPF
							INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = cliente.LOC_CODIGO
							WHERE clientesOrigem.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('')), 3, 1000) AS CidadeOrigem,
							SUBSTRING((SELECT DISTINCT ', ' + localidade.LOC_DESCRICAO
							FROM T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO clientesDestino
							INNER JOIN T_CLIENTE cliente on cliente.CLI_CGCCPF = clientesDestino.CLI_CGCCPF
							INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = cliente.LOC_CODIGO
							WHERE clientesDestino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('')), 3, 1000) AS CidadeDestino,
							
							SUBSTRING((SELECT DISTINCT ', ' + localidade.UF_SIGLA
							FROM T_TABELA_FRETE_CLIENTE_CLIENTE_ORIGEM clientesOrigem
							INNER JOIN T_CLIENTE cliente on cliente.CLI_CGCCPF = clientesOrigem.CLI_CGCCPF
							INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = cliente.LOC_CODIGO
							WHERE clientesOrigem.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('')), 3, 1000) AS UFOrigem,
							SUBSTRING((SELECT DISTINCT ', ' + localidade.UF_SIGLA
							FROM T_TABELA_FRETE_CLIENTE_CLIENTE_DESTINO clientesDestino
							INNER JOIN T_CLIENTE cliente on cliente.CLI_CGCCPF = clientesDestino.CLI_CGCCPF
							INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = cliente.LOC_CODIGO
							WHERE clientesDestino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('')), 3, 1000) AS UFDestino,

                            ModeloReboque.MVC_DESCRICAO [DescricaoModeloReboque],
							'PEDAGIO' + ModeloReboque.MVC_DESCRICAO [DescricaoModeloReboquePedagio]
                    FROM T_TABELA_FRETE_CLIENTE TabelaFreteCliente
                    LEFT JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                    LEFT JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro ON Parametro.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
                    LEFT OUTER JOIN T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque ON Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO
                    LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloReboque ON PModeloReboque.MVC_CODIGO = ModeloReboque.MVC_CODIGO
                    LEFT JOIN T_TABELA_FRETE_VIGENCIA Vigencia ON Vigencia.TFV_CODIGO = TabelaFreteCliente.TFV_CODIGO
                    LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTipoCarga ON Parametro.TBC_CODIGO = ItemTipoCarga.TBC_CODIGO
                    AND ItemTipoCarga.TPI_TIPO_OBJETO = 1
                    LEFT OUTER JOIN T_TIPO_DE_CARGA ItemItemTipoCarga ON ItemItemTipoCarga.TCG_CODIGO = ItemTipoCarga.TPI_CODIGO_OBJETO
                    WHERE TabelaFreteCliente.TFC_ATIVO = 1
                    AND TabelaFreteCliente.TBF_CODIGO = {codigoTabelaFrete}
                    AND TabelaFreteCliente.TFC_TIPO = 0
                    AND Vigencia.TFV_DATA_INICIAL <= '{dataVigencia:yyyy-MM-dd}' AND (Vigencia.TFV_DATA_FINAL IS NULL OR Vigencia.TFV_DATA_FINAL >= '{dataVigencia:yyyy-MM-dd}')
                    GROUP BY TabelaFreteCliente.TFC_CODIGO,
							Parametro.TBC_CODIGO,
                            tabelafrete.TBF_CODIGO,
                            TabelaFrete.TBF_DESCRICAO,
                            TabelaFreteCliente.TFC_TIPO,
                            cast(Vigencia.TFV_DATA_FINAL AS date),
                            cast(Vigencia.TFV_DATA_INICIAL AS date),
                            tabelafrete.TBF_SITUACAO_ALTERACAO,
                            TabelaFreteCliente.TFC_CODIGO_INTEGRACAO,
                            TabelaFreteCliente.TFC_QUILOMETRAGEM,
                            ModeloReboque.MVC_DESCRICAO,
                            TabelaFrete.TBF_PARAMETRO_BASE,
                            ItemTipoCarga.TPI_VALOR) tabs)
                    tabs
						
						pivot (max([ValorTipoCarga])
                    FOR [DescricaoModeloReboque] in ([BITREM], [CARRETA2E], [CARRETA3E], [TOCO], [TRUCK], [CARRETA 6 EIXOS LS], [CARRETA 6 EIXOS VANDERLEIA], [CARRETA RODOTREM], [RODOTREM],
                                                     [BITREM HAVAN], [CARRETA HAVAN], [CARRETA RODOTREM HAVAN], [RODOTREM HAVAN])) pvt1
					   
						pivot (max([ValorPedagio])
                    FOR [DescricaoModeloReboquePedagio] in ([PEDAGIOBITREM], [PEDAGIOCARRETA2E], [PEDAGIOCARRETA3E], [PEDAGIOTOCO], [PEDAGIOTRUCK], [PEDAGIOCARRETA 6 EIXOS LS], [PEDAGIOCARRETA 6 EIXOS VANDERLEIA], [PEDAGIOCARRETA RODOTREM], [PEDAGIORODOTREM],
                                                            [PEDAGIOBITREM HAVAN], [PEDAGIOCARRETA HAVAN], [PEDAGIOCARRETA RODOTREM HAVAN], [PEDAGIORODOTREM HAVAN])) pvt2

					GROUP BY [Codigo],
                             [CodigoIntegracao],
                             [Quilometragem],
                             [BairroOrigem],
                             [CidadeOrigem],
                             [UFOrigem],
                             [BairroDestino],
                             [CidadeDestino],
                             [UFDestino]";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);
            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavanModeloVeicular)));
            return nhQuery.SetTimeout(900).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavanModeloVeicular>();
        }

        public IList<dynamic> BuscarRelatorioConsultaTabelaFretePagamentoTerceiros(int codigoTabelaFrete, DateTime dataVigencia)
        {
            string query = $@"
declare @colunas_pivot as nvarchar(max), @comando_sql as nvarchar(max)
set @colunas_pivot =
stuff((
select distinct ',' + quotename(cp.Componente) from
(select distinct CONCAT(1,'-',mv.MVC_DESCRICAO) [Componente] from T_MODELO_VEICULAR_CARGA mv
INNER JOIN T_TABELA_FRETE_MODELO_REBOQUE TFM ON TFM.MVC_CODIGO = mv.MVC_CODIGO
WHERE TFM.TBF_CODIGO = 51
UNION
SELECT Distinct CONCAT(2,'-',CF.CFR_DESCRICAO) FROM T_TABELA_FRETE_COMPONENTE_FRETE TCF
INNER JOIN T_COMPONENTE_FRETE CF ON CF.CFR_CODIGO = TCF.CFR_CODIGO
WHERE TCF.TBF_CODIGO =51
) cp
for xml path('')
), 1, 1, '')

--print @colunas_pivot

set @comando_sql = '
SELECT *
FROM
(SELECT tabs.[Codigo],
tabs.[CodigoIntegracao],
tabs.[DescricaoModeloReboque],
tabs.[ValorTipoCarga],
tabs.[CidadeOrigem],
tabs.[UFOrigem],
tabs.[CidadeDestino],
tabs.[UFDestino],
tabs.[Quilometragem]
FROM
(SELECT TabelaFreteCliente.TFC_CODIGO Codigo,
TabelaFrete.TBF_DESCRICAO TabelaFrete,
tabelafrete.TBF_CODIGO [Cód.tabela frete],
TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracao,
ISNULL(TabelaFreteCliente.TFC_QUILOMETRAGEM, 0) Quilometragem,

ModeloReboque.MVC_DESCRICAO DescricaoParametroBase,
TabelaFrete.TBF_PARAMETRO_BASE TipoParametroBase,
coalesce(ItemTipoCarga.TPI_VALOR, 0) [ValorTipoCarga],

cast(Vigencia.TFV_DATA_FINAL AS date) DataFinal,
cast(Vigencia.TFV_DATA_INICIAL AS date) DataInicial,

SUBSTRING((SELECT DISTINCT '', '' + localidade.LOC_DESCRICAO
FROM T_TABELA_FRETE_CLIENTE_ORIGEM origem
INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = origem.LOC_CODIGO
WHERE origem.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('''')), 3, 1000) AS CidadeOrigem,
SUBSTRING((SELECT DISTINCT '', '' + localidade.LOC_DESCRICAO
FROM T_TABELA_FRETE_CLIENTE_DESTINO destino
INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = destino.LOC_CODIGO
WHERE destino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('''')), 3, 1000) AS CidadeDestino,

SUBSTRING((SELECT DISTINCT '', '' + localidade.UF_SIGLA
FROM T_TABELA_FRETE_CLIENTE_ORIGEM origem
INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = origem.LOC_CODIGO
WHERE origem.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('''')), 3, 1000) AS UFOrigem,
SUBSTRING((SELECT DISTINCT '', '' + localidade.UF_SIGLA
FROM T_TABELA_FRETE_CLIENTE_DESTINO destino
INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = destino.LOC_CODIGO
WHERE destino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO FOR XML PATH('''')), 3, 1000) AS UFDestino,

case when ItemTipoCarga.TPI_TIPO_OBJETO = 2 then concat(1,''-'', ModeloReboque.MVC_DESCRICAO) else concat(2,''-'',compfrete.cfr_descricao) end [DescricaoModeloReboque]

FROM T_TABELA_FRETE_CLIENTE TabelaFreteCliente
LEFT JOIN T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
LEFT JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro ON Parametro.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO

LEFT JOIN T_TABELA_FRETE_VIGENCIA Vigencia ON Vigencia.TFV_CODIGO = TabelaFreteCliente.TFV_CODIGO
LEFT OUTER JOIN T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTipoCarga ON Parametro.TBC_CODIGO = ItemTipoCarga.TBC_CODIGO
AND (ItemTipoCarga.TPI_TIPO_OBJETO = 2 OR ItemTipoCarga.TPI_TIPO_OBJETO = 4)

LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloReboque ON ModeloReboque.MVC_CODIGO = ItemTipoCarga.TPI_CODIGO_OBJETO AND ItemTipoCarga.TPI_TIPO_OBJETO = 2
LEFT OUTER JOIN T_COMPONENTE_FRETE compfrete ON compfrete.cfr_CODIGO = ItemTipoCarga.TPI_CODIGO_OBJETO AND ItemTipoCarga.TPI_TIPO_OBJETO = 4

WHERE TabelaFreteCliente.TFC_ATIVO = 1
--AND TabelaFreteCliente.TFC_CODIGO = 125957
AND TabelaFreteCliente.TBF_CODIGO = {codigoTabelaFrete}
AND TabelaFreteCliente.TFC_TIPO = 0
AND Vigencia.TFV_DATA_INICIAL <= ''{dataVigencia:yyyy-MM-dd}'' AND (Vigencia.TFV_DATA_FINAL IS NULL OR Vigencia.TFV_DATA_FINAL >= ''{dataVigencia:yyyy-MM-dd}'')
GROUP BY 
compfrete.cfr_descricao,
ItemTipoCarga.TPI_TIPO_OBJETO,
TabelaFreteCliente.TFC_CODIGO,
Parametro.TBC_CODIGO,
tabelafrete.TBF_CODIGO,
TabelaFrete.TBF_DESCRICAO,
TabelaFreteCliente.TFC_TIPO,
cast(Vigencia.TFV_DATA_FINAL AS date),
cast(Vigencia.TFV_DATA_INICIAL AS date),
tabelafrete.TBF_SITUACAO_ALTERACAO,
TabelaFreteCliente.TFC_CODIGO_INTEGRACAO,
TabelaFreteCliente.TFC_QUILOMETRAGEM,
ModeloReboque.MVC_DESCRICAO,
TabelaFrete.TBF_PARAMETRO_BASE,
ItemTipoCarga.TPI_VALOR) tabs)
tabs

pivot (max([ValorTipoCarga])
FOR [DescricaoModeloReboque] in (' + @colunas_pivot + '))  pvt1

'
--print @comando_sql

execute(@comando_sql)";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);
            return nhQuery.SetTimeout(900).List<dynamic>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.TabelaFreteCliente.TabelaFreteClienteValores> BuscarPorIntegracaoMarisa(int codigoTabelaFrete)
        {
            string sql = $@"
                select TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE ValorPesoExcedente,
                       TabelaFrete.TBF_CODIGO_INTEGRACAO CodigoIntegracaoTabelaFrete,
                       TabelaFrete.TBF_FATOR_CUBAGEM FatorCubagem,
                       REPLICATE('0', 8-LEN(CepDestino.TCD_CEP_INICIAL)) + CONVERT(nvarchar(10), CepDestino.TCD_CEP_INICIAL) CepDestinoInicial,
                       REPLICATE('0', 8-LEN(CepDestino.TCD_CEP_FINAL)) + CONVERT(nvarchar(10), CepDestino.TCD_CEP_FINAL) CepDestinoFinal,
                       CepDestino.TCD_DIAS_UTEIS CepDestinoPrazoDiasUteis,    
                       ComponentePeso.TPI_VALOR ValorPeso,
                       ComponentePesoItem.TFP_PESO_INICIAL PesoInicial,
                       ComponentePesoItem.TFP_PESO_FINAL PesoFinal,
                       ComponenteAdValorem.TPI_VALOR ValorAdValorem,
                       ComponenteAdValorem.TPI_TIPO_VALOR TipoValorAdValorem,
                       ComponenteGris.TPI_VALOR ValorGris,
                       ComponenteGris.TPI_TIPO_VALOR TipoValorGris
                  from T_TABELA_FRETE_CLIENTE TabelaFreteCliente
                  join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                  left join T_TABELA_FRETE_CLIENTE_CEP_DESTINO CepDestino on CepDestino.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
                  left join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ComponentePeso on ComponentePeso.TFC_CODIGO = TabelaFreteCliente.TFC_Codigo and ComponentePeso.TPI_TIPO_OBJETO = 6
                  left join T_TABELA_FRETE_PESO ComponentePesoItem on ComponentePesoItem.TFP_CODIGO = ComponentePeso.TPI_CODIGO_OBJETO
                  left join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ComponenteAdValorem on ComponenteAdValorem.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and ComponenteAdValorem.TPI_TIPO_OBJETO = 4 and ComponenteAdValorem.TPI_CODIGO_OBJETO in (
                           select _componenteFreteAdValorem.TFC_CODIGO
                             from T_TABELA_FRETE_COMPONENTE_FRETE _componenteFreteAdValorem
                             join T_COMPONENTE_FRETE _componenteFrete on _componenteFrete.CFR_CODIGO = _componenteFreteAdValorem.CFR_CODIGO
                            where _componenteFreteAdValorem.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                              and (_componenteFrete.CFR_TIPO_COMPONENTE_FRETE = {(int)TipoComponenteFrete.ADVALOREM} or _componenteFrete.CFR_DESCRICAO like '%Ad Valorem%')
                       )
                  left join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ComponenteGris on ComponenteGris.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO and ComponenteGris.TPI_TIPO_OBJETO = 4 and ComponenteGris.TPI_CODIGO_OBJETO in (
                           select _componenteFreteGris.TFC_CODIGO
                             from T_TABELA_FRETE_COMPONENTE_FRETE _componenteFreteGris
                             join T_COMPONENTE_FRETE _componenteFrete on _componenteFrete.CFR_CODIGO = _componenteFreteGris.CFR_CODIGO
                            where _componenteFreteGris.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                              and (_componenteFrete.CFR_TIPO_COMPONENTE_FRETE = {(int)TipoComponenteFrete.GRIS} or _componenteFrete.CFR_DESCRICAO like '%gris%')
                       )
                 where TabelaFreteCliente.TFC_TIPO = {(int)TipoTabelaFreteCliente.Calculo}
                   and TabelaFrete.TBF_CODIGO = {codigoTabelaFrete}
                 group by TabelaFreteCliente.TFC_CODIGO, TabelaFreteCliente.TFC_VALOR_PESO_EXCEDENTE, TabelaFrete.TBF_CODIGO_INTEGRACAO, TabelaFrete.TBF_FATOR_CUBAGEM,
                       CepDestino.TCD_CEP_INICIAL, CepDestino.TCD_CEP_FINAL, CepDestino.TCD_DIAS_UTEIS,
                       ComponentePeso.TPI_VALOR, ComponentePesoItem.TFP_PESO_INICIAL, ComponentePesoItem.TFP_PESO_FINAL,
                       ComponenteAdValorem.TPI_VALOR, ComponenteAdValorem.TPI_TIPO_VALOR,
                       ComponenteGris.TPI_VALOR, ComponenteGris.TPI_TIPO_VALOR
                 order by TabelaFreteCliente.TFC_CODIGO, CepDestino.TCD_CEP_INICIAL;";

            var consultaConsultaTabelaFrete = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaConsultaTabelaFrete.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.TabelaFreteCliente.TabelaFreteClienteValores)));

            return consultaConsultaTabelaFrete.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.TabelaFreteCliente.TabelaFreteClienteValores>();
        }

        #endregion

        #region Cálculo de Frete 

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarTabelasAptasParaCalculo(int codigoTabelaFrete, List<double> remetentes, List<int> cepsOrigem, List<int> localidadesOrigem, List<string> estadosOrigem, List<int> regioesOrigem, List<int> paisesOrigem, List<double> destinatarios, List<int> cepsDestino, List<int> localidadesDestino, List<string> estadosDestino, List<int> regioesDestino, List<int> paisesDestino, List<int> rotas, DateTime dataVigencia, int empresa, int codigoCanalEntrega, int codigoCanalVenda)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            if (codigoTabelaFrete > 0)
                query = query.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);
            else
                query = query.Where(obj => obj.TabelaFrete.Ativo);

            query = query.Where(obj =>
                                (obj.Vigencia.DataInicial <= dataVigencia.Date && (obj.Vigencia.DataFinal >= dataVigencia.Date || !obj.Vigencia.DataFinal.HasValue)) &&
                                obj.Ativo &&
                                (obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Calculo || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Alteracao));

            if (empresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == empresa || obj.Empresa == null);

            if (codigoCanalEntrega > 0)
                query = query.Where(obj => obj.CanalEntrega.Codigo == codigoCanalEntrega || obj.CanalEntrega == null);

            if (codigoCanalVenda > 0)
                query = query.Where(obj => obj.CanalVenda.Codigo == codigoCanalVenda || obj.CanalVenda == null);

            //Origens
            query = query.Where(OrigensExatas(remetentes, cepsOrigem, localidadesOrigem, estadosOrigem, regioesOrigem, rotas, paisesOrigem).Or(OrigensVariaveis(remetentes, cepsOrigem, localidadesOrigem, estadosOrigem, regioesOrigem, rotas, paisesOrigem)));

            //Destinos
            query = query.Where(DestinosExatos(destinatarios, cepsDestino, localidadesDestino, estadosDestino, regioesDestino, rotas, paisesDestino).Or(DestinosVariaveis(destinatarios, cepsDestino, localidadesDestino, estadosDestino, regioesDestino, rotas, paisesDestino)));

            return query
              .Fetch(obj => obj.TabelaFrete)
              .Fetch(obj => obj.Vigencia)
              .Fetch(obj => obj.Empresa)
              .Fetch(obj => obj.CanalEntrega)
              .Fetch(obj => obj.CanalVenda)
              .WithOptions(o => { o.SetTimeout(120); })
              .ToList();
        }

        #endregion

        #region Queriables

        #region Origem

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> OrigensExatas(List<double> clientes, List<int> ceps, List<int> localidades, List<string> estados, List<int> regioes, List<int> rotas, List<int> paises)
        {
            var origensExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            origensExatas = origensExatas.And(o => !o.FreteValidoParaQualquerOrigem);

            Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> parametrosAdicionais = null;

            if (clientes.Count > 0)
                parametrosAdicionais = ClientesOrigemExatos(clientes);

            if (ceps.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = CEPsOrigemExatos(ceps);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(CEPsOrigemExatos(ceps));
            }

            if (localidades.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = LocalidadesOrigemExatas(localidades);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(LocalidadesOrigemExatas(localidades));
            }

            if (estados.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = EstadosOrigemExatos(estados);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(EstadosOrigemExatos(estados));
            }

            if (regioes.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RegioesOrigemExatos(regioes);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RegioesOrigemExatos(regioes));
            }

            if (rotas.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RotasOrigemExatas(rotas);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RotasOrigemExatas(rotas));
            }

            if (paises.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = PaisesOrigemExatas(paises);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(PaisesOrigemExatas(paises));
            }

            return origensExatas.And(parametrosAdicionais);
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> ClientesOrigemExatos(List<double> clientesOrigem)
        {
            var clientesExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            clientesExatos = clientesExatos.And(o => o.ClientesOrigem.Count == clientesOrigem.Count);

            var clientesExatosClientes = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (double clienteOrigem in clientesOrigem)
                clientesExatosClientes = clientesExatosClientes.And(o => o.ClientesOrigem.Any(l => l.CPF_CNPJ == clienteOrigem));

            clientesExatos = clientesExatos.And(clientesExatosClientes);

            return clientesExatos;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> CEPsOrigemExatos(List<int> cepsOrigem)
        {
            var cepsExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            cepsExatos = cepsExatos.And(o => o.CEPsOrigem.Count == cepsOrigem.Count);

            var cepsExatosCEPs = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int cepOrigem in cepsOrigem)
                cepsExatosCEPs = cepsExatosCEPs.And(o => o.CEPsOrigem.Any(l => l.CEPInicial <= cepOrigem && l.CEPFinal >= cepOrigem));

            cepsExatos = cepsExatos.And(cepsExatosCEPs);

            return cepsExatos;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> LocalidadesOrigemExatas(List<int> localidadesOrigem)
        {
            var localidadesExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            localidadesExatas = localidadesExatas.And(o => o.Origens.Count == localidadesOrigem.Count);

            var localidadesExatasLocalidades = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int origem in localidadesOrigem)
                localidadesExatasLocalidades = localidadesExatasLocalidades.And(o => o.Origens.Any(l => l.Codigo == origem));

            localidadesExatas = localidadesExatas.And(localidadesExatasLocalidades);

            return localidadesExatas;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> EstadosOrigemExatos(List<string> estadosOrigem)
        {
            var estadosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            estadosExatos = estadosExatos.And(o => o.EstadosOrigem.Count == estadosOrigem.Count);

            var estadosExatosEstados = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (string estadoOrigem in estadosOrigem)
                estadosExatosEstados = estadosExatosEstados.And(o => o.EstadosOrigem.Any(l => l.Sigla == estadoOrigem));

            estadosExatos = estadosExatos.And(estadosExatosEstados);

            return estadosExatos;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> PaisesOrigemExatas(List<int> paises)
        {
            var paisesExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            paisesExatas = paisesExatas.And(o => o.PaisesOrigem.Count == paises.Count);

            var paisesExatasPaises = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int pais in paises)
                paisesExatasPaises = paisesExatasPaises.And(o => o.PaisesOrigem.Any(l => l.Codigo == pais));

            paisesExatas = paisesExatas.And(paisesExatasPaises);

            return paisesExatas;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RegioesOrigemExatos(List<int> regioes)
        {
            var regioesExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            regioesExatas = regioesExatas.And(o => o.RegioesOrigem.Count == regioes.Count);

            var regioesExatasRegioes = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int regiaoOrigem in regioes)
                regioesExatasRegioes = regioesExatasRegioes.And(o => o.RegioesOrigem.Any(l => l.Codigo == regiaoOrigem));

            regioesExatas = regioesExatas.And(regioesExatasRegioes);

            return regioesExatas;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RotasOrigemExatas(List<int> rotas)
        {
            var rotasExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            rotasExatas = rotasExatas.And(o => o.RotasOrigem.Count == rotas.Count);

            var rotasExatasRotas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int rota in rotas)
                rotasExatasRotas = rotasExatasRotas.And(o => o.RotasOrigem.Any(l => l.Codigo == rota));

            rotasExatas = rotasExatas.And(rotasExatasRotas);

            return rotasExatas;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> OrigensVariaveis(List<double> clientes, List<int> ceps, List<int> localidades, List<string> estados, List<int> regioes, List<int> rotas, List<int> paises)
        {
            var origensVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            origensVariaveis = origensVariaveis.And(o => o.FreteValidoParaQualquerOrigem);

            Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> parametrosAdicionais = null;

            if (clientes.Count > 0)
                parametrosAdicionais = ClientesOrigemVariaveis(clientes);

            if (ceps.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = CEPsOrigemVariaveis(ceps);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(CEPsOrigemVariaveis(ceps));
            }

            if (localidades.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = LocalidadesOrigemVariaveis(localidades);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(LocalidadesOrigemVariaveis(localidades));
            }

            if (estados.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = EstadosOrigemVariaveis(estados);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(EstadosOrigemVariaveis(estados));
            }

            if (regioes.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RegioesOrigemVariaveis(regioes);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RegioesOrigemVariaveis(regioes));
            }

            if (rotas.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RotasOrigemVariaveis(rotas);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RotasOrigemVariaveis(rotas));
            }

            if (paises.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = PaisesOrigemVariaveis(paises);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(PaisesOrigemVariaveis(paises));
            }

            return origensVariaveis.And(parametrosAdicionais);
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> ClientesOrigemVariaveis(List<double> clientesOrigem)
        {
            var clientesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (double clienteOrigem in clientesOrigem)
                clientesVariaveis = clientesVariaveis.And(o => o.ClientesOrigem.Any(l => l.CPF_CNPJ == clienteOrigem));

            return clientesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> CEPsOrigemVariaveis(List<int> cepsOrigem)
        {
            var cepsVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int cepOrigem in cepsOrigem)
                cepsVariaveis = cepsVariaveis.And(o => o.CEPsOrigem.Any(l => l.CEPInicial <= cepOrigem && l.CEPFinal >= cepOrigem));

            return cepsVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> LocalidadesOrigemVariaveis(List<int> localidadesOrigem)
        {
            var localidadesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int origem in localidadesOrigem)
                localidadesVariaveis = localidadesVariaveis.And(o => o.Origens.Any(l => l.Codigo == origem));

            return localidadesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> EstadosOrigemVariaveis(List<string> estadosOrigem)
        {
            var estadosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (string estadoOrigem in estadosOrigem)
                estadosVariaveis = estadosVariaveis.And(o => o.EstadosOrigem.Any(l => l.Sigla == estadoOrigem));

            return estadosVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> PaisesOrigemVariaveis(List<int> paises)
        {
            var paisesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int pais in paises)
                paisesVariaveis = paisesVariaveis.And(o => o.PaisesOrigem.Any(l => l.Codigo == pais));

            return paisesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RegioesOrigemVariaveis(List<int> regioes)
        {
            var regioesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int regiaoOrigem in regioes)
                regioesVariaveis = regioesVariaveis.And(o => o.RegioesOrigem.Any(l => l.Codigo == regiaoOrigem));

            return regioesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RotasOrigemVariaveis(List<int> rotas)
        {
            var rotasVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int rota in rotas)
                rotasVariaveis = rotasVariaveis.And(o => o.RotasOrigem.Any(l => l.Codigo == rota));

            return rotasVariaveis;
        }

        #endregion

        #region Destino

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> DestinosExatos(List<double> clientes, List<int> ceps, List<int> localidades, List<string> estados, List<int> regioes, List<int> rotas, List<int> paises)
        {
            var destinosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            destinosExatos = destinosExatos.And(o => !o.FreteValidoParaQualquerDestino);

            Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> parametrosAdicionais = null;

            if (clientes.Count > 0)
                parametrosAdicionais = ClientesDestinoExatos(clientes);

            if (ceps.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = CEPsDestinoExatos(ceps);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(CEPsDestinoExatos(ceps));
            }

            if (localidades.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = LocalidadesDestinoExatas(localidades);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(LocalidadesDestinoExatas(localidades));
            }

            if (estados.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = EstadosDestinoExatos(estados);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(EstadosDestinoExatos(estados));
            }

            if (regioes.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RegioesDestinoExatos(regioes);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RegioesDestinoExatos(regioes));
            }

            if (rotas.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RotasDestinoExatos(rotas);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RotasDestinoExatos(rotas));
            }

            if (paises.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = PaisesDestinoExatos(paises);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(PaisesDestinoExatos(paises));
            }

            return destinosExatos.And(parametrosAdicionais);
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> ClientesDestinoExatos(List<double> clientesDestino)
        {
            var clientesExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            clientesExatos = clientesExatos.And(o => o.ClientesDestino.Count == clientesDestino.Count);

            var clientesExatosClientes = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (double clienteOrigem in clientesDestino)
                clientesExatosClientes = clientesExatosClientes.And(o => o.ClientesDestino.Any(l => l.CPF_CNPJ == clienteOrigem));

            clientesExatos = clientesExatos.And(clientesExatosClientes);

            return clientesExatos;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> CEPsDestinoExatos(List<int> cepsDestino)
        {
            var cepsExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            cepsExatos = cepsExatos.And(o => o.CEPsDestino.Count == cepsDestino.Count);

            var cepsExatosCEPs = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int cepDestino in cepsDestino)
                cepsExatosCEPs = cepsExatosCEPs.And(o => o.CEPsDestino.Any(l => l.CEPInicial <= cepDestino && l.CEPFinal >= cepDestino) ||
                                                         o.RotasDestino.Any(rd => rd.CEPsDestino.Any(l => l.CEPInicial <= cepDestino && l.CEPFinal >= cepDestino)));

            cepsExatos = cepsExatos.And(cepsExatosCEPs);

            return cepsExatos;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> LocalidadesDestinoExatas(List<int> localidadesDestino)
        {
            var localidadesExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            localidadesExatas = localidadesExatas.And(o => o.Destinos.Count == localidadesDestino.Count);

            var localidadesExatasLocalidades = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int destino in localidadesDestino)
                localidadesExatasLocalidades = localidadesExatasLocalidades.And(o => o.Destinos.Any(l => l.Codigo == destino));

            localidadesExatas = localidadesExatas.And(localidadesExatasLocalidades);

            return localidadesExatas;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> EstadosDestinoExatos(List<string> estadosDestino)
        {
            var estadosExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            estadosExatos = estadosExatos.And(o => o.EstadosDestino.Count == estadosDestino.Count);

            var estadosExatosEstados = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (string estadoDestino in estadosDestino)
                estadosExatosEstados = estadosExatosEstados.And(o => o.EstadosDestino.Any(l => l.Sigla == estadoDestino));

            estadosExatos = estadosExatos.And(estadosExatosEstados);

            return estadosExatos;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> PaisesDestinoExatos(List<int> paisesDestino)
        {
            var paisesExatos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            paisesExatos = paisesExatos.And(o => o.PaisesDestino.Count == paisesDestino.Count);

            var paisesExatosPaises = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int paisDestino in paisesDestino)
                paisesExatosPaises = paisesExatosPaises.And(o => o.PaisesDestino.Any(l => l.Codigo == paisDestino));

            paisesExatos = paisesExatos.And(paisesExatosPaises);

            return paisesExatos;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RegioesDestinoExatos(List<int> regioesDestino)
        {
            var regioesExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            regioesExatas = regioesExatas.And(o => o.RegioesDestino.Count == regioesDestino.Count);

            var regioesExatasRegioes = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int regiaoDestino in regioesDestino)
                regioesExatasRegioes = regioesExatasRegioes.And(o => o.RegioesDestino.Any(l => l.Codigo == regiaoDestino));

            regioesExatas = regioesExatas.And(regioesExatasRegioes);

            return regioesExatas;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RotasDestinoExatos(List<int> rotasDestino)
        {
            var rotasExatas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            rotasExatas = rotasExatas.And(o => o.RotasDestino.Count == rotasDestino.Count);

            var rotasExatasRotas = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int rotaDestino in rotasDestino)
                rotasExatasRotas = rotasExatasRotas.And(o => o.RotasDestino.Any(l => l.Codigo == rotaDestino));

            rotasExatas = rotasExatas.And(rotasExatasRotas);

            return rotasExatas;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> DestinosVariaveis(List<double> clientes, List<int> ceps, List<int> localidades, List<string> estados, List<int> regioes, List<int> rotas, List<int> paises)
        {
            var destinosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            destinosVariaveis = destinosVariaveis.And(o => o.FreteValidoParaQualquerDestino);

            Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> parametrosAdicionais = null;

            if (clientes.Count > 0)
                parametrosAdicionais = ClientesDestinoVariaveis(clientes);

            if (ceps.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = CEPsDestinoVariaveis(ceps);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(CEPsDestinoVariaveis(ceps));
            }

            if (localidades.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = LocalidadesDestinoVariaveis(localidades);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(LocalidadesDestinoVariaveis(localidades));
            }

            if (estados.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = EstadosDestinoVariaveis(estados);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(EstadosDestinoVariaveis(estados));
            }

            if (regioes.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RegioesDestinoVariaveis(regioes);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RegioesDestinoVariaveis(regioes));
            }

            if (rotas.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = RotasDestinoVariaveis(rotas);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(RotasDestinoVariaveis(rotas));
            }

            if (paises.Count > 0)
            {
                if (parametrosAdicionais == null)
                    parametrosAdicionais = PaisesDestinoVariaveis(paises);
                else
                    parametrosAdicionais = parametrosAdicionais.Or(PaisesDestinoVariaveis(paises));
            }

            return destinosVariaveis.And(parametrosAdicionais);
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> ClientesDestinoVariaveis(List<double> clientesDestino)
        {
            var clientesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (double clienteOrigem in clientesDestino)
                clientesVariaveis = clientesVariaveis.And(o => o.ClientesDestino.Any(l => l.CPF_CNPJ == clienteOrigem));

            return clientesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> CEPsDestinoVariaveis(List<int> cepsDestino)
        {
            var cepsVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int cepDestino in cepsDestino)
                cepsVariaveis = cepsVariaveis.And(o => o.CEPsDestino.Any(l => l.CEPInicial <= cepDestino && l.CEPFinal >= cepDestino) ||
                                                       o.RotasDestino.Any(rd => rd.CEPsDestino.Any(l => l.CEPInicial <= cepDestino && l.CEPFinal >= cepDestino)));

            return cepsVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> LocalidadesDestinoVariaveis(List<int> localidadesDestino)
        {
            var localidadesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int destino in localidadesDestino)
                localidadesVariaveis = localidadesVariaveis.And(o => o.Destinos.Any(l => l.Codigo == destino));

            return localidadesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> EstadosDestinoVariaveis(List<string> estadosDestino)
        {
            var estadosVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (string estadoDestino in estadosDestino)
                estadosVariaveis = estadosVariaveis.And(o => o.EstadosDestino.Any(l => l.Sigla == estadoDestino));

            return estadosVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> PaisesDestinoVariaveis(List<int> paisesDestino)
        {
            var paisesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int paisDestino in paisesDestino)
                paisesVariaveis = paisesVariaveis.And(o => o.PaisesDestino.Any(l => l.Codigo == paisDestino));

            return paisesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RegioesDestinoVariaveis(List<int> regioesDestino)
        {
            var regioesVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int regiaoDestino in regioesDestino)
                regioesVariaveis = regioesVariaveis.And(o => o.RegioesDestino.Any(l => l.Codigo == regiaoDestino));

            return regioesVariaveis;
        }

        private Expression<Func<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente, bool>> RotasDestinoVariaveis(List<int> rotasDestino)
        {
            var rotasVariaveis = PredicateBuilder.True<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

            foreach (int rotaDestino in rotasDestino)
                rotasVariaveis = rotasVariaveis.And(o => o.RotasDestino.Any(l => l.Codigo == rotaDestino));

            return rotasVariaveis;
        }

        #endregion

        #endregion

        #region Relatório de Consulta de Tabela de Frete

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return Consultar(filtrosPesquisa, propriedades, parametrosConsulta, out string query);
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, out string queryRetorno)
        {
            var consultaConsultaTabelaFrete = new ConsultaTabelaFreteCliente().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            queryRetorno = consultaConsultaTabelaFrete.QueryString;

            consultaConsultaTabelaFrete.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete)));

            IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = consultaConsultaTabelaFrete.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(this.unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int precision = configuracao.TabelaFretePrecisaoDinheiroDois ? 2 : 6;
            foreach (Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete consulta in lista)
            {
                consulta.SetNumeroCasasDecimais(precision);
            }

            return lista;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            
            var consultaConsultaTabelaFrete = new ConsultaTabelaFreteCliente().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaConsultaTabelaFrete.SetTimeout(300).UniqueResult<int>();
        }

        #endregion

        #region Exportação

        public IList<ExportacaoTabelaFrete> ObterValoresFreteExportacao()
        {
            string sql = @"select 
                           TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracao,
                           ModeloReboque.MVC_DESCRICAO DescricaoParametroBase, 
                           ItemTipoCarga.TPI_VALOR Valor,
                           CONVERT(VARCHAR(10), Vigencia.TFV_DATA_FINAL, 103) DataFinal
                           from t_tabela_frete_cliente TabelaFreteCliente 
                           left outer join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO 
                           left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro on TabelaFreteCliente.TFC_CODIGO = Parametro.TFC_CODIGO
                           left outer join T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque on Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO 
                           left outer join T_MODELO_VEICULAR_CARGA ModeloReboque on PModeloReboque.MVC_CODIGO = ModeloReboque.MVC_CODIGO
                           left outer join T_TABELA_FRETE_VIGENCIA Vigencia on TabelaFreteCliente.TFV_CODIGO = Vigencia.TFV_CODIGO
                           left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTipoCarga on Parametro.TBC_CODIGO = ItemTipoCarga.TBC_CODIGO and ItemTipoCarga.TPI_TIPO_OBJETO = 1 left outer join T_TIPO_DE_CARGA ItemItemTipoCarga on ItemItemTipoCarga.TCG_CODIGO = ItemTipoCarga.TPI_CODIGO_OBJETO
                           where TabelaFreteCliente.TFC_TIPO = 0 and TabelaFreteCliente.TFC_ATIVO = 1 and TabelaFreteCliente.TBF_CODIGO = 2 and ItemTipoCarga.TPI_VALOR > 0
                           group by TabelaFreteCliente.TFC_CODIGO_INTEGRACAO, ModeloReboque.MVC_DESCRICAO, ItemTipoCarga.TPI_VALOR, Vigencia.TFV_DATA_FINAL";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ExportacaoTabelaFrete)));

            return query.SetTimeout(600).List<ExportacaoTabelaFrete>();
        }

        public IList<ExportacaoTabelaFrete> ObterValoresPedagioExportacao()
        {
            string sql = @"select 
                           TabelaFreteCliente.TFC_CODIGO_INTEGRACAO CodigoIntegracao,
                           ModeloReboque.MVC_DESCRICAO DescricaoParametroBase, 
                           Pedagio.TPI_VALOR Valor,
                           CONVERT(VARCHAR(10), Vigencia.TFV_DATA_INICIAL, 103) DataInicial
                           from t_tabela_frete_cliente TabelaFreteCliente 
                           left outer join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO 
                           left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro on TabelaFreteCliente.TFC_CODIGO = Parametro.TFC_CODIGO
                           left outer join T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque on Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO 
                           left outer join T_MODELO_VEICULAR_CARGA ModeloReboque on PModeloReboque.MVC_CODIGO = ModeloReboque.MVC_CODIGO
                           left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM Pedagio on Pedagio.TBC_CODIGO = Parametro.TBC_CODIGO and Pedagio.TPI_TIPO_OBJETO = 4 and Pedagio.TPI_CODIGO_OBJETO = 3
                           left outer join T_TABELA_FRETE_VIGENCIA Vigencia on TabelaFreteCliente.TFV_CODIGO = Vigencia.TFV_CODIGO
                           left outer join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemTipoCarga on Parametro.TBC_CODIGO = ItemTipoCarga.TBC_CODIGO and ItemTipoCarga.TPI_TIPO_OBJETO = 1 left outer join T_TIPO_DE_CARGA ItemItemTipoCarga on ItemItemTipoCarga.TCG_CODIGO = ItemTipoCarga.TPI_CODIGO_OBJETO
                           where TabelaFreteCliente.TFC_TIPO = 0 and TabelaFreteCliente.TFC_ATIVO = 1 and TabelaFreteCliente.TBF_CODIGO = 2 and Pedagio.TPI_VALOR > 0
                           group by TabelaFreteCliente.TFC_CODIGO_INTEGRACAO, ModeloReboque.MVC_DESCRICAO, Pedagio.TPI_VALOR, Vigencia.TFV_DATA_INICIAL";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ExportacaoTabelaFrete)));

            return query.SetTimeout(600).List<ExportacaoTabelaFrete>();
        }

        #endregion

        #region Extração Massiva de Tabela de Frete

        public int ContarConsultaExtracaoMassiva(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaExtracaoMassivaTabelaFreteCliente().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(300).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> ConsultaExtracaoMassiva(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery query = new ConsultaExtracaoMassivaTabelaFreteCliente().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete)));

            return query.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete> ConsultaHistoricoParametrosBaseTabelaFrete(long codigoTabelaFrete)
        {
            string sql = @"WITH HistoricoTabelaFrete AS (SELECT HistoricoObjeto.HIO_CODIGO_OBJETO
									 , HistoricoObjeto.HIO_CODIGO
									 , HistoricoObjeto.HIO_DATA 
								  FROM T_HISTORICO_OBJETO HistoricoObjeto
								 WHERE HistoricoObjeto.HIO_CODIGO_OBJETO = :CODIGO_TABELA_FRETE 
								   AND HistoricoObjeto.HIO_OBJETO = 'TabelaFrete' 
                           )
                           SELECT HistoricoTabelaFrete.HIO_CODIGO_OBJETO CodigoTabelaFrete
                                , HistoricoPropriedade.HIP_DE AS DescricaoParametroBaseDe
  	                            , HistoricoPropriedade.HIP_PARA AS DescricaoParametroBasePara
                                , HistoricoTabelaFrete.HIO_DATA AS DataHistorico
                             FROM T_HISTORICO_OBJETO_PROPRIEDADE HistoricoObjetoPropriedade 
                             JOIN HistoricoTabelaFrete ON HistoricoTabelaFrete.HIO_CODIGO = HistoricoObjetoPropriedade.HIO_CODIGO 
                             JOIN T_HISTORICO_PROPRIEDADE HistoricoPropriedade ON HistoricoPropriedade.HIP_CODIGO = HistoricoObjetoPropriedade.HIP_CODIGO
                            WHERE UPPER(HistoricoPropriedade.HIP_PROPRIEDADE) = UPPER('ParametroBase')";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("CODIGO_TABELA_FRETE", codigoTabelaFrete);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete)));

            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete>();
        }

        #endregion
    }
}
