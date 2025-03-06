using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace HqCatalog.Business.Services
{
    public class NotificadorService : INotificador
    {
        private readonly List<Notificacao> _notificacoes;

        public NotificadorService()
        {
            _notificacoes = new List<Notificacao>();
        }

        public void Handle(Notificacao notificacao)
        {
            _notificacoes.Add(notificacao);
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }
    }
}
