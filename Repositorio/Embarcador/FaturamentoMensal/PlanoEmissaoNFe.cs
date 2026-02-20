using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.FaturamentoMensal
{
    public class PlanoEmissaoNFe : RepositorioBase<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe>
    {
        public PlanoEmissaoNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor BuscarPlanoEmissao(int qtdTitulos, int qtdBoletos, int qtdNFe, int qtdNFSe, int? codigoEmpresa)
        {
            int qtdTotalDocumentos = 0;
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor>();
            var result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
            if (codigoEmpresa != null && codigoEmpresa > 0)
                result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);

            qtdTotalDocumentos = qtdTitulos + qtdBoletos + qtdNFe + qtdNFSe;
            result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == true && obj.PlanoEmissaoNFe.CobrancaNFe == true && obj.PlanoEmissaoNFe.CobrancaNFSe == true && obj.PlanoEmissaoNFe.CobrancaTitulo == true);
            if (result.Count() > 0)
                return result.FirstOrDefault();
            else
            {
                qtdTotalDocumentos = qtdNFe;
                result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                if (codigoEmpresa != null && codigoEmpresa > 0)
                    result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == false && obj.PlanoEmissaoNFe.CobrancaNFe == true && obj.PlanoEmissaoNFe.CobrancaNFSe == false && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                if (result.Count() > 0)
                    return result.FirstOrDefault();
                else
                {
                    qtdTotalDocumentos = qtdTitulos + qtdBoletos + qtdNFe;
                    result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                    if (codigoEmpresa != null && codigoEmpresa > 0)
                        result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                    result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == true && obj.PlanoEmissaoNFe.CobrancaNFe == true && obj.PlanoEmissaoNFe.CobrancaNFSe == false && obj.PlanoEmissaoNFe.CobrancaTitulo == true);
                    if (result.Count() > 0)
                        return result.FirstOrDefault();
                    else
                    {
                        qtdTotalDocumentos = qtdTitulos + qtdBoletos;
                        result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                        if (codigoEmpresa != null && codigoEmpresa > 0)
                            result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                        result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == true && obj.PlanoEmissaoNFe.CobrancaNFe == false && obj.PlanoEmissaoNFe.CobrancaNFSe == false && obj.PlanoEmissaoNFe.CobrancaTitulo == true);
                        if (result.Count() > 0)
                            return result.FirstOrDefault();
                        else
                        {
                            qtdTotalDocumentos = qtdTitulos;
                            result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                            if (codigoEmpresa != null && codigoEmpresa > 0)
                                result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                            result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == false && obj.PlanoEmissaoNFe.CobrancaNFe == false && obj.PlanoEmissaoNFe.CobrancaNFSe == false && obj.PlanoEmissaoNFe.CobrancaTitulo == true);
                            if (result.Count() > 0)
                                return result.FirstOrDefault();
                            else
                            {
                                qtdTotalDocumentos = qtdBoletos + qtdNFe + qtdNFSe;
                                result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                if (codigoEmpresa != null && codigoEmpresa > 0)
                                    result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == true && obj.PlanoEmissaoNFe.CobrancaNFe == true && obj.PlanoEmissaoNFe.CobrancaNFSe == true && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                                if (result.Count() > 0)
                                    return result.FirstOrDefault();
                                else
                                {
                                    qtdTotalDocumentos = qtdBoletos + qtdNFe;
                                    result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                    if (codigoEmpresa != null && codigoEmpresa > 0)
                                        result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                    result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == true && obj.PlanoEmissaoNFe.CobrancaNFe == true && obj.PlanoEmissaoNFe.CobrancaNFSe == false && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                                    if (result.Count() > 0)
                                        return result.FirstOrDefault();
                                    else
                                    {
                                        qtdTotalDocumentos = qtdBoletos + qtdNFSe;
                                        result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                        if (codigoEmpresa != null && codigoEmpresa > 0)
                                            result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                        result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == true && obj.PlanoEmissaoNFe.CobrancaNFe == false && obj.PlanoEmissaoNFe.CobrancaNFSe == true && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                                        if (result.Count() > 0)
                                            return result.FirstOrDefault();
                                        else
                                        {
                                            qtdTotalDocumentos = qtdNFe + qtdNFSe;
                                            result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                            if (codigoEmpresa != null && codigoEmpresa > 0)
                                                result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                            result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == false && obj.PlanoEmissaoNFe.CobrancaNFe == true && obj.PlanoEmissaoNFe.CobrancaNFSe == true && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                                            if (result.Count() > 0)
                                                return result.FirstOrDefault();
                                            else
                                            {
                                                qtdTotalDocumentos = qtdNFSe;
                                                result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                                if (codigoEmpresa != null && codigoEmpresa > 0)
                                                    result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                                result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == false && obj.PlanoEmissaoNFe.CobrancaNFe == false && obj.PlanoEmissaoNFe.CobrancaNFSe == true && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                                                if (result.Count() > 0)
                                                    return result.FirstOrDefault();
                                                else
                                                {
                                                    qtdTotalDocumentos = qtdBoletos;
                                                    result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                                    if (codigoEmpresa != null && codigoEmpresa > 0)
                                                        result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                                    result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == true && obj.PlanoEmissaoNFe.CobrancaNFe == false && obj.PlanoEmissaoNFe.CobrancaNFSe == false && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                                                    if (result.Count() > 0)
                                                        return result.FirstOrDefault();
                                                    else
                                                    {
                                                        qtdTotalDocumentos = qtdNFe;
                                                        result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                                        if (codigoEmpresa != null && codigoEmpresa > 0)
                                                            result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                                        result = result.Where(obj => qtdTotalDocumentos >= obj.QuantidadeInicial && qtdTotalDocumentos <= obj.QuantidadeFinal && obj.PlanoEmissaoNFe.CobrancaBoleto == false && obj.PlanoEmissaoNFe.CobrancaNFe == true && obj.PlanoEmissaoNFe.CobrancaNFSe == false && obj.PlanoEmissaoNFe.CobrancaTitulo == false);
                                                        if (result.Count() > 0)
                                                            return result.FirstOrDefault();
                                                        else
                                                        {
                                                            result = from obj in query where obj.PlanoEmissaoNFe.Ativo == true select obj;
                                                            if (codigoEmpresa != null && codigoEmpresa > 0)
                                                                result = result.Where(obj => obj.PlanoEmissaoNFe.Empresa.Codigo == codigoEmpresa);
                                                            result = result.Where(obj => obj.PlanoEmissaoNFe.ValorAdesao > 0);
                                                            if (result.Count() > 0)
                                                                return result.FirstOrDefault();
                                                            else
                                                                return null;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe> Consulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContaConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }
    }
}
