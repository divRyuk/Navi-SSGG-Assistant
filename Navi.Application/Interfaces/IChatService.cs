using Navi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navi.Domain.Entities;

namespace Navi.Application.Interfaces
{
    public interface IChatService
    {
        Task SaveMessageAsync(ChatMessage message);
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string sessionId);
    }
}

