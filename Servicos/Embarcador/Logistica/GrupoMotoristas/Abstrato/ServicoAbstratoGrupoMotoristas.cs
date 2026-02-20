using Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica.GrupoMotoristas.Abstrato
{
    public abstract class ServicoAbstratoGrupoMotoristas : ServicoBase
    {
        protected readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        protected ServicoAbstratoGrupoMotoristas(UnitOfWork unitOfWork, CancellationToken cancelationToken = default, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null) : base(unitOfWork, cancelationToken)
        {
            _auditado = Auditado;
        }

        protected async Task<List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>> AtualizarRelacionamento(
            IRepositorioRelacionamentoGrupoMotoristas repositorio,
            IEnumerable<IRelacionamentoGrupoMotoristas> relacionamentosAtuais,
            IEnumerable<IRelacionamentoGrupoMotoristas> relacionamentosVindouros,
            Func<IRelacionamentoGrupoMotoristas, Task> inserir,
            Func<IRelacionamentoGrupoMotoristas, Task> IntegracaoalteracaoDeletar,
            Func<IRelacionamentoGrupoMotoristas, Task> IntegracaoalteracaoAdicionar)
        {
            var novos = relacionamentosVindouros.Where(o => o.CodigoRelacionamento == 0);
            var antigos = relacionamentosVindouros.Where(o => o.CodigoRelacionamento != 0);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new();

            if (relacionamentosAtuais.Count() > antigos.Count())
            {
                var aRemover = relacionamentosAtuais
                    .Where(o => !antigos.Any(a => a.CodigoRelacionamento == o.CodigoRelacionamento))
                    .ToList();

                for (int i = 0; i < aRemover.Count; i++)
                {
                    await repositorio.DeletarPorCodigoAsync(aRemover[i].CodigoRelacionamento, _auditado);

                    await IntegracaoalteracaoDeletar(aRemover[i]);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = repositorio.ObterNomeVerbosoDaEntidade(),
                        De = aRemover[i].Descricao,
                        Para = ""
                    });
                }
            }

            for (int i = 0; i < novos.Count(); i++)
            {
                await inserir(novos.ElementAt(i));

                await IntegracaoalteracaoAdicionar(novos.ElementAt(i));

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = repositorio.ObterNomeVerbosoDaEntidade(),
                    De = "",
                    Para = novos.ElementAt(i).Descricao
                });
            }

            return alteracoes;
        }
    }
}
