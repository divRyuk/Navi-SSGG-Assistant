using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;

namespace Navi.Infrastructure.Services
{
    public class InMemoryChatService : IChatService
    {
        private readonly List<ChatMessage> _chatHistory = new();

        public Task SaveMessageAsync(ChatMessage message)
        {
            _chatHistory.Add(message);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string sessionId)
        {
            var history = _chatHistory
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.Timestamp)
                .AsEnumerable();

            return Task.FromResult(history);
        }
    }
}

