using System.Collections.Concurrent;

namespace KonkursBot.Services
{
    public class StateService
    {
        private static readonly ConcurrentDictionary<long, StateList> KeyValuesState = new();

        public Task SetUserState(long chatId, StateList state)
        {
            KeyValuesState[chatId] = state;
            return Task.CompletedTask;
        }

        public Task DeleteState(long Id)
        {
            KeyValuesState.TryRemove(Id, out _);
            return Task.CompletedTask;
        }

        public StateList? GetUserState(long Id)
        {
            KeyValuesState.TryGetValue(Id, out var state);
            return state;
        }
    }

    public enum StateList
    {
        register_choose_language,
        register_get_fullname,
        register_get_contact
    }
}
