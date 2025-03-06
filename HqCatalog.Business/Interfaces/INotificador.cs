using HqCatalog.Business.Models;
using System.Collections.Generic;

namespace HqCatalog.Business.Interfaces
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}
