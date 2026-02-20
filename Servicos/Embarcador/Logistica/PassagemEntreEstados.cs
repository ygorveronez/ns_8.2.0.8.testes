using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class PassagemEntreEstados : ServicoBase
    {        
        public PassagemEntreEstados(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public void AdicionarPassagensEntreTodosOsEstadosPadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Servicos.Embarcador.Logistica.MapRequestApi(unitOfWork);
            Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);
            Repositorio.PassagemPercursoEstado repPassagemPercursoEstado = new Repositorio.PassagemPercursoEstado(unitOfWork);

            List<Dominio.Entidades.Localidade> capitais = BuscarTodasCapitais(unitOfWork);
            List<Dominio.Entidades.Localidade> destinos = BuscarTodasCapitais(unitOfWork);

            foreach (Dominio.Entidades.Localidade capital in capitais)
            {
                foreach (Dominio.Entidades.Localidade destino in destinos)
                {
                    try
                    {
                        if (capital.Codigo != destino.Codigo)
                        {
                            Dominio.Entidades.PercursoEstado percursoEstado = new Dominio.Entidades.PercursoEstado();
                            percursoEstado.EstadoOrigem = capital.Estado;
                            percursoEstado.EstadoDestino = destino.Estado;
                            percursoEstado.Empresa = empresa;
                            Dominio.Entidades.PercursoEstado percursoEstadoExistente = repPercursoEstado.Buscar(empresa.Codigo, percursoEstado.EstadoOrigem.Sigla, percursoEstado.EstadoDestino.Sigla);

                            List<string> divisas = serMapRequestAPI.BuscarDivisas(capital.Estado.Sigla);
                            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> Passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
                            bool valido = true;
                            if (!divisas.Contains(destino.Estado.Sigla) && percursoEstadoExistente == null)
                            {
                                bool gerouPercurso = false;
                                try
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Logistica.RouteMapRequestAPI route = serMapRequestAPI.BuscarRotaMapRequestApi(capital.Descricao + " " + capital.Estado.Sigla, destino.Descricao + " " + destino.Estado.Sigla, true);
                                    valido = route.valido;
                                    if (valido)
                                    {
                                        foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passageRota in route.UFPassagens)
                                        {
                                            if (passageRota.Sigla != capital.Estado.Sigla && passageRota.Sigla != destino.Estado.Sigla)
                                                Passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = passageRota.Sigla, Posicao = Passagens.Count() + 1 });
                                        }
                                        gerouPercurso = true;
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    Servicos.Log.TratarErro(ex1);
                                }

                                if (!gerouPercurso)
                                { //Se não gerou o percurso tenta o percuro ao contrário
                                    Dominio.ObjetosDeValor.Embarcador.Logistica.RouteMapRequestAPI route = serMapRequestAPI.BuscarRotaMapRequestApi(destino.Descricao + " " + destino.Estado.Sigla, capital.Descricao + " " + capital.Estado.Sigla, true);
                                    valido = route.valido;
                                    if (valido)
                                    {
                                        int ordemPassagem = route.UFPassagens.Count() + 1;
                                        foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passageRota in route.UFPassagens)
                                        {
                                            ordemPassagem = ordemPassagem - 1;
                                            if (passageRota.Sigla != capital.Estado.Sigla && passageRota.Sigla != destino.Estado.Sigla)
                                            {
                                                Passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = passageRota.Sigla, Posicao = ordemPassagem });
                                            }
                                        }
                                    }

                                }

                            }
                            if (percursoEstadoExistente == null && valido)
                            {
                                repPercursoEstado.Inserir(percursoEstado);
                                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passagem in Passagens)
                                {
                                    Dominio.Entidades.PassagemPercursoEstado passagemPercurso = new Dominio.Entidades.PassagemPercursoEstado();
                                    passagemPercurso.EstadoDePassagem = new Dominio.Entidades.Estado() { Sigla = passagem.Sigla };
                                    passagemPercurso.Ordem = passagem.Posicao;
                                    passagemPercurso.Percurso = percursoEstado;
                                    repPassagemPercursoEstado.Inserir(passagemPercurso);
                                }
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        Servicos.Log.TratarErro(ex1);//coveirão, caso ocorra alguma falha registra e continua.
                    }

                }
            }
        }

        private List<Dominio.Entidades.Localidade> BuscarTodasCapitais(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Localidade> capitais = new List<Dominio.Entidades.Localidade>();
            capitais.Add(BuscarCapital(1200401, unitOfWork));//Rio Branco AC
            capitais.Add(BuscarCapital(2704302, unitOfWork));//Maceió AL
            capitais.Add(BuscarCapital(1600303, unitOfWork));//Macapa AP
            capitais.Add(BuscarCapital(1302603, unitOfWork));//Manaus AM
            capitais.Add(BuscarCapital(2927408, unitOfWork));//Salvador BA
            capitais.Add(BuscarCapital(2304400, unitOfWork));//Fortaleza CE
            capitais.Add(BuscarCapital(5300108, unitOfWork));//Brasília DF
            capitais.Add(BuscarCapital(3205002, unitOfWork));//Vitória ES 3205309 (Vitória não da certo, testado com Serra 3205002)
            capitais.Add(BuscarCapital(5208707, unitOfWork));//Goiânia GO
            capitais.Add(BuscarCapital(2111300, unitOfWork));//São Luís MA
            capitais.Add(BuscarCapital(5103403, unitOfWork));//Cuiabá MT
            capitais.Add(BuscarCapital(5002704, unitOfWork));//Campo Grande MS
            capitais.Add(BuscarCapital(3106200, unitOfWork));//Belo Horizonte MG
            capitais.Add(BuscarCapital(1501402, unitOfWork));//Belém PA
            capitais.Add(BuscarCapital(2507507, unitOfWork));//João Pessoa PB
            capitais.Add(BuscarCapital(4106902, unitOfWork));//Curitiba PR
            capitais.Add(BuscarCapital(2611606, unitOfWork));//Recife PE 2611606 (Recife não calculou rota para origem, testado com Boa Viagem 2302404)
            capitais.Add(BuscarCapital(2211001, unitOfWork));//Teresina PI
            capitais.Add(BuscarCapital(3304557, unitOfWork));//Rio de Janeiro RJ
            capitais.Add(BuscarCapital(2408102, unitOfWork));//Natal RN
            capitais.Add(BuscarCapital(4314902, unitOfWork));//Porto Alegre RS
            capitais.Add(BuscarCapital(1100205, unitOfWork));//Porto Velho RO
            capitais.Add(BuscarCapital(1400100, unitOfWork));//Boa Vista RR
            capitais.Add(BuscarCapital(4205407, unitOfWork));//Florianópolis SC
            capitais.Add(BuscarCapital(3550308, unitOfWork));//São Paulo SP
            capitais.Add(BuscarCapital(2800308, unitOfWork));//Aracaju SE
            capitais.Add(BuscarCapital(1721000, unitOfWork));//Palmas TO
            return capitais;
        }

        private Dominio.Entidades.Localidade BuscarCapital(int ibge, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            return repLocalidade.BuscarPorCodigoIBGE(ibge);
        }
    }
}
